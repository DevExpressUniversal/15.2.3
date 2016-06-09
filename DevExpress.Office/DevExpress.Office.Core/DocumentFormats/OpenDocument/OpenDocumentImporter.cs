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

using DevExpress.Office.Utils;
using DevExpress.Utils.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace DevExpress.Office {
	public abstract class OpenDocumentImporterBase : DestinationAndXmlBasedImporter, IDisposable {
		PackageFileCollection packageFiles;
		readonly Dictionary<string, OfficeNativeImage> packageImages;
		protected OpenDocumentImporterBase(IDocumentModel documentModel)
			: base(documentModel) {
			this.packageImages = new Dictionary<string, OfficeNativeImage>();
		}
		public PackageFileCollection PackageFiles { get { return packageFiles; } }
		public Dictionary<string, OfficeNativeImage> PackageImageStreams { get { return packageImages; } }
		public override string RelationsNamespace {
			get { return String.Empty; }
		}
		public override string DocumentRootFolder { get; set; }
		public override OpenXmlRelationCollection DocumentRelations {
			get { return null; }
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected internal virtual void DisposePackageFiles() {
			if (PackageFiles == null)
				return;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++)
				PackageFiles[i].Stream.Dispose();
			PackageFiles.Clear();
			this.packageFiles = null;
		}
		#endregion
		#region Packages
		public virtual void OpenPackage(Stream stream) {
			this.packageFiles = GetPackageFiles(stream);
		}
		protected internal virtual PackageFileCollection GetPackageFiles(Stream stream) {
			InternalZipFileCollection files = InternalZipArchive.Open(stream);
			int count = files.Count;
			PackageFileCollection result = new PackageFileCollection();
			for (int i = 0; i < count; i++) {
				InternalZipFile file = files[i];
				result.Add(new PackageFile(file.FileName.Replace('\\', '/'), file.FileDataStream, (int)file.UncompressedSize));
			}
			return result;
		}
		public virtual XmlReader GetPackageFileXmlReader(string fileName) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			return CreateXmlReader(stream);
		}
		public virtual Stream GetPackageFileStream(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.Stream;
		}
		protected internal virtual PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			fileName = fileName.Replace('\\', '/');
			if (fileName.StartsWith("/"))
				fileName = fileName.Substring(1);
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return file;
			}
			return null;
		}
		#endregion
		#region ThrowInvalidFile
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid OpenDocument file");
		}
		#endregion
		protected override void PrepareOfficeTheme() {
		}
		protected internal override XmlReaderSettings CreateXmlReaderSettings() {
			XmlReaderSettings result = base.CreateXmlReaderSettings();
			result.IgnoreWhitespace = false;
			return result;
		}
		protected internal override void BeforeImportMainDocument() {
			base.BeforeImportMainDocument();
			ApplyDefaultProperties();
			ImportStyles();
		}
		#region Styles
		protected internal virtual void ImportStyles() {
			string fileName = "styles.xml";
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportStylesCore(reader);
		}
		protected internal virtual void ImportStylesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "document-styles", OfficeOpenDocumentHelper.OfficeNamespace))
				return;
			this.DestinationStack = new Stack<Destination>();
			ImportContent(reader, CreateRootStyleDestination());
		}
		#endregion
		protected internal virtual void ApplyDefaultProperties() {
		}
		#region LookUpImage
		public virtual OfficeImage LookUpImageByFileName(string fileName) {
			OfficeNativeImage rootImage;
			if (packageImages.TryGetValue(fileName, out rootImage))
				return new OfficeReferenceImage(DocumentModel, rootImage);
			Stream stream = LookupImageStreamByFileName(fileName);
			if (stream == null)
				return null;
			else {
				try {
					stream.Seek(0, SeekOrigin.Begin);
					OfficeReferenceImage image = DocumentModel.CreateImage(stream);
					packageImages.Add(fileName, image.NativeRootImage);
					return image;
				}
				catch {
					return null;
				}
			}
		}
		protected internal virtual Stream LookupImageStreamByFileName(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			byte[] bytes = new byte[file.StreamLength];
			file.Stream.Read(bytes, 0, bytes.Length);
			return new MemoryStream(bytes);
		}
		#endregion
		#region Conversion and Parsing utilities
		public override bool ConvertToBool(string value) {
			bool result = false;
			if (Boolean.TryParse(value, out result))
				return result;
			ThrowInvalidFile();
			return false;
		}
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			throw new InvalidOperationException();
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			throw new InvalidOperationException();
		}
		#endregion
		protected internal override void ProcessCurrentDestination(XmlReader reader) {
			if (DestinationStack.Count > 0) 
				base.ProcessCurrentDestination(reader);
		}
		protected internal abstract Destination CreateRootStyleDestination();
	}
}
