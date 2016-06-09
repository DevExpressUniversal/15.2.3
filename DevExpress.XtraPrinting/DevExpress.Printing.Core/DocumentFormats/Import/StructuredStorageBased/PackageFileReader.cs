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

using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Reader;
namespace DevExpress.Office.Utils {
	#region PackageFileReader
	public class PackageFileReader : IDisposable {
		#region Fields
		const string pathDelimiter = @"\";
		StructuredStorageReader structuredStorageReader;
		PackageFileCollection packageFiles;
		PackageFileStreams packageFileStreams;
		Dictionary<string, VirtualStreamBinaryReader> packageFileReaders;
		#endregion
		public PackageFileReader(Stream stream) : this(stream, false) {
		}
		public PackageFileReader(Stream stream, bool keepOpen) {
			Guard.ArgumentNotNull(stream, "stream");
			this.packageFiles = GetPackageFiles(stream, keepOpen);
			this.packageFileStreams = new PackageFileStreams();
			this.packageFileReaders = new Dictionary<string, VirtualStreamBinaryReader>();
		}
		#region Properties
		protected internal PackageFileCollection PackageFiles { get { return packageFiles; } }
		protected internal PackageFileStreams PackageFileStreams { get { return packageFileStreams; } }
		protected internal Dictionary<string, VirtualStreamBinaryReader> PackageFileReaders { get { return packageFileReaders; } }
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
				DisposePackageFileReaders();
				if (this.structuredStorageReader != null) {
					this.structuredStorageReader.Dispose();
					this.structuredStorageReader = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~PackageFileReader() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void DisposePackageFiles() {
			if (PackageFiles == null) return;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFiles[i].Stream.Dispose();
			}
			PackageFiles.Clear();
			this.packageFiles = null;
		}
		protected internal virtual void DisposePackageFileReaders() {
			if (PackageFileReaders == null) return;
			foreach (string key in PackageFileReaders.Keys) {
				IDisposable disposable = PackageFileReaders[key];
				disposable.Dispose();
			}
			PackageFileReaders.Clear();
			this.packageFileReaders = null;
		}
		public virtual VirtualStreamBinaryReader GetCachedPackageFileReader(string fileName) {
			string name = fileName.StartsWith(pathDelimiter) ? fileName : pathDelimiter + fileName;
			VirtualStreamBinaryReader reader;
			if (PackageFileReaders.TryGetValue(name, out reader)) {
				reader.BaseStream.Seek(0, SeekOrigin.Begin);
				return reader;
			}
			Stream stream = GetCachedPackageFileStream(name);
			if (stream == null)
				return null;
			reader = new VirtualStreamBinaryReader(stream);
			PackageFileReaders.Add(name, reader);
			return reader;
		}
		protected internal virtual PackageFileCollection GetPackageFiles(Stream stream, bool keepOpen) {
			this.structuredStorageReader = new StructuredStorageReader(stream, keepOpen);
			PackageFileCollection result = new PackageFileCollection();
			ICollection<DirectoryEntry> streamEntries = structuredStorageReader.AllStreamEntries;
			foreach (DirectoryEntry entry in streamEntries) {
				string name = entry.Path;
				Stream storageStream = structuredStorageReader.GetStream(name);
				result.Add(new PackageFile(name, storageStream, (int)storageStream.Length));
			}
			return result;
		}
		protected internal virtual PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return file;
			}
			return null;
		}
		public virtual Stream GetCachedPackageFileStream(string fileName) {
			Stream stream;
			if (PackageFileStreams.TryGetValue(fileName, out stream)) {
				stream.Seek(0, SeekOrigin.Begin);
				return stream;
			}
			stream = CreatePackageFileStreamCopy(fileName);
			if (stream == null) return null;
			PackageFileStreams.Add(fileName, stream);
			return stream;
		}
		protected internal virtual Stream CreatePackageFileStreamCopy(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null) return null;
			return file.Stream;
		}
		protected internal virtual Stream GetPackageFileStream(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null) return null;
			return file.Stream;
		}
		public IEnumerable<string> EnumerateFiles(string storageName, bool topStorageOnly) {
			List<string> result = new List<string>();
			if(PackageFiles == null)
				return result;
			string name = storageName.StartsWith(pathDelimiter) ? storageName : pathDelimiter + storageName;
			if(!name.EndsWith(pathDelimiter))
				name += pathDelimiter;
			int count = PackageFiles.Count;
			for(int i = 0; i < count; i++) {
				string fileName = PackageFiles[i].FileName;
				if(fileName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase) && (!topStorageOnly || !fileName.Substring(name.Length).Contains(pathDelimiter)))
					result.Add(fileName);
			}
			return result;
		}
	}
	#endregion
}
