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

using DevExpress.Utils.Zip;
using System;
using System.IO;
using System.Xml;
namespace DevExpress.Office.Utils {
	#region XmlPackageReader
	public class XmlPackageReader : IDisposable {
		#region Fields
		PackageFileCollection packageFiles;
		#endregion
		#region Properties
		public PackageFileCollection PackageFiles { get { return packageFiles; } }
		#endregion
		public void OpenPackage(Stream stream) {
			InternalZipFileCollection files = InternalZipArchive.Open(stream);
			int count = files.Count;
			packageFiles = new PackageFileCollection();
			for (int i = 0; i < count; i++) {
				InternalZipFile file = files[i];
				packageFiles.Add(new PackageFile(file.FileName.Replace('\\', '/'), file.FileDataStream, (int)file.UncompressedSize));
			}
		}
		public XmlReader GetPackageFileXmlReader(string fileName, XmlReaderSettings settings) {
			return GetPackageFileXmlReader(fileName, settings, false);
		}
		public XmlReader GetPackageFileXmlReader(string fileName, XmlReaderSettings settings, bool createSeekableStream) {
			Stream stream = GetPackageFileStream(fileName, createSeekableStream);
			if (stream == null)
				return null;
			return XmlReader.Create(stream, settings);
		}
		public Stream GetPackageFileStream(string fileName) {
			return GetPackageFileStream(fileName, false);
		}
		Stream GetPackageFileStream(string fileName, bool returnSeekableStream) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			if (returnSeekableStream) {
				MemoryStream stream = file.SeekableStream;
				stream.Seek(0, SeekOrigin.Begin);
				return stream;
			}
			return file.Stream;
		}
		public PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			fileName = fileName.Replace('\\', '/');
			if (fileName.StartsWith("/", StringComparison.Ordinal))
				fileName = fileName.Substring(1);
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return file;
			}
			return null;
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && packageFiles != null) {
				int count = packageFiles.Count;
				for (int i = 0; i < count; i++)
					this.packageFiles[i].Stream.Dispose();
				this.packageFiles.Clear();
				this.packageFiles = null;
			}
		}
		#endregion
	}
	#endregion
}
