#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using DevExpress.Office;
using DevExpress.Office.Crypto.Agile;
using DevExpress.Office.Utils;
using DevExpress.XtraExport.Xlsx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region XlsxImporter (abstract class)
	public abstract class XlsxImporter : DestinationXmImporter, IDisposable {
		string documentRootFolder;
		Stack<OpenXmlRelationCollection> documentRelationsStack;
		XmlPackageReader packageReader;
		#region Properties
		public Stack<OpenXmlRelationCollection> DocumentRelationsStack { get { return documentRelationsStack; } }
		public OpenXmlRelationCollection DocumentRelations { get { return documentRelationsStack.Peek(); } }
		public string DocumentRootFolder { get { return documentRootFolder; } }
		public abstract string EncryptionPassword { get; }
		#endregion
		public void Import(Stream stream) {
			try {
				Prepare();
				ImportWorkbook(stream);
			}
			finally {
			}
		}
		void Prepare() {
			this.documentRelationsStack = new Stack<OpenXmlRelationCollection>();
		}
		protected internal void ImportWorkbook(Stream stream) {
			OpenPackage(stream);
			OpenXmlRelationCollection rootRelations = ImportRelations("_rels/.rels");
			string fileName = OpenXmlImportRelationHelper.LookupRelationTargetByType(rootRelations, XlsxPackageBuilder.OfficeDocumentNamespace, String.Empty, "workbook.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader == null)
				ThrowInvalidFile("Can't get reader for workbook.xml");
			if (!ReadToRootElement(reader, "workbook", XlsxPackageBuilder.SpreadsheetNamespaceConst))
				ThrowInvalidFile("Can't find root element \"workbook\"");
			this.documentRootFolder = Path.GetDirectoryName(fileName);
			this.documentRelationsStack.Push(ImportRelations(documentRootFolder + "/_rels/" + Path.GetFileName(fileName) + ".rels"));
			ImportMainDocument(reader, stream);
		}
		#region OpenPackage
		protected void OpenPackage(Stream stream) {
			long position = CalculateInitialStreamPosition(stream);
			OpenPackageCore(stream);
			if (packageReader.PackageFiles.Count <= 0 && SeekToInitialStreamPosition(stream, position)) {
				Stream dataStream = OpenEncryptedStream(stream);
				if (dataStream != null)
					OpenPackageCore(dataStream);
				else
					throw new InvalidFileException("Wrong file format!");
			}
		}
		Stream OpenEncryptedStream(Stream stream) {
			try {
				PackageFileReader reader = new PackageFileReader(stream);
				Stream encryptedInfoStream = reader.GetCachedPackageFileStream(@"\EncryptionInfo");
				if (encryptedInfoStream == null)
					return null;
				EncryptionSession session = EncryptionSession.LoadFromStream(encryptedInfoStream, true);
				bool unlocked = session.UnlockWithPassword(EncryptionPassword);
				if (!unlocked)
					throw new EncryptedFileException(EncryptedFileError.WrongPassword, "Wrong password or encryption method!");
				Stream encryptedDataStream = reader.GetCachedPackageFileStream(@"\EncryptedPackage");
				if (encryptedDataStream == null)
					return null;
				return session.GetEncryptedStream(encryptedDataStream);
			}
			catch {
				return null;
			}
		}
		long CalculateInitialStreamPosition(Stream stream) {
			try {
				return stream.Position;
			}
			catch {
				return -1;
			}
		}
		bool SeekToInitialStreamPosition(Stream stream, long position) {
			try {
				if (stream.CanSeek && position >= 0) {
					stream.Seek(position, SeekOrigin.Begin);
					return true;
				}
				else
					return false;
			}
			catch {
				return false;
			}
		}
		#endregion
		#region Packages
		protected internal void OpenPackageCore(Stream stream) {
			packageReader = new XmlPackageReader();
			packageReader.OpenPackage(stream);
		}
		protected internal XmlReader GetPackageFileXmlReaderBasedSeekableStream(string fileName) {
			return packageReader.GetPackageFileXmlReader(fileName, CreateXmlReaderSettings(), true);
		}
		protected internal XmlReader GetPackageFileXmlReader(string fileName) {
			return packageReader.GetPackageFileXmlReader(fileName, CreateXmlReaderSettings());
		}
		protected internal XmlReader GetPackageFileXmlReader(string fileName, XmlReaderSettings settings) {
			return packageReader.GetPackageFileXmlReader(fileName, settings);
		}
		protected internal Stream GetPackageFileStream(string fileName) {
			return packageReader.GetPackageFileStream(fileName);
		}
		protected internal PackageFile GetPackageFile(string fileName) {
			return packageReader.GetPackageFile(fileName);
		}
		protected internal void DisposePackageFiles() {
			if (packageReader != null) {
				packageReader.Dispose();
				packageReader = null;
			}
		}
		#endregion
		#region Conversion and Parsing utilities
		public override bool ConvertToBool(string value) {
			if (value == "1" || value == "on" || value == "true" || value == "t")
				return true;
			if (value == "0" || value == "off" || value == "false" || value == "f")
				return false;
			ThrowInvalidFile(string.Format("Can't convert {0} to bool", value));
			return false;
		}
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		#endregion
		protected internal OpenXmlRelationCollection ImportRelations(string fileName) {
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				return ImportRelationsCore(reader);
			else
				return new OpenXmlRelationCollection();
		}
		protected internal OpenXmlRelationCollection ImportRelationsCore(XmlReader reader) {
			OpenXmlRelationCollection result = new OpenXmlRelationCollection();
			if (!ReadToRootElement(reader, "Relationships", XlsxPackageBuilder.PackageRelsNamespace))
				return result;
			ImportContent(reader, new RelationshipsDestination(this, result));
			return result;
		}
		protected internal string CalculateRelationTarget(OpenXmlRelation relation, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.CalculateRelationTarget(relation, rootFolder, defaultFileName);
		}
		protected internal string LookupRelationTargetByType(OpenXmlRelationCollection relations, string type, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.LookupRelationTargetByType(relations, type, rootFolder, defaultFileName);
		}
		protected internal string LookupRelationTargetById(OpenXmlRelationCollection relations, string id, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.LookupRelationTargetById(relations, id, rootFolder, defaultFileName);
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
			}
		}
		#endregion
	}
	#endregion
}
