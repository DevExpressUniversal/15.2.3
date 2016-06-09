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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using DevExpress.Web.Localization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Web.Internal {
	public static class BinaryImageTempFolderHelper {
		static readonly int StorageKeyLength = MD5.Create().HashSize / 4;
		static readonly int FileMinLength = TemporaryFileNamePrefix.Length + StorageKeyLength + TempFileExtension.Length;
		public const string TemporaryFileNamePrefix = "dxbiupload_";
		public const string TemporaryFolderCleanerKey = "aspxDXSTorageTemporaryFolderCleaner";
		public const string TempFileExtension = ".tmp";
		static readonly object tempFoldersDictionaryLock = new object();
		static HashSet<string> tempFolders;
		internal static HashSet<string> TempFolders {
			get {
				lock(tempFoldersDictionaryLock) {
					if(tempFolders == null)
						tempFolders = new HashSet<string>();
					return tempFolders;
				}
			}
		}
		public static void SafeRemoveTempFilesHavingNoToken(string folderPath) {
			Cache cache = HttpUtils.GetCache();
			if(cache == null)
				return;
			var tempFolder = new DirectoryInfo(folderPath);
			tempFolder.GetFiles(TemporaryFileNamePrefix + "*" + TempFileExtension)
				.Where(x => !(cache[ExtractKey(x.Name)] is UploadToken))
				.ForEach(SafeDelete);
		}
		static void SafeDelete(FileInfo file) {
			try {
				file.Delete();
			} catch {
			}
		}
		static string ExtractKey(string filename) {
			if(filename.StartsWith(TemporaryFileNamePrefix) && filename.Length >= FileMinLength)
				return filename.Substring(TemporaryFileNamePrefix.Length, StorageKeyLength);
			return String.Empty;
		}
		public static void EnsureTemporaryFolderExists(string path) {
			string physicalPath = UrlUtils.ResolvePhysicalPath(path);
			if(!Directory.Exists(physicalPath))
				Directory.CreateDirectory(physicalPath);
		}
	}
	public class UploadToken {
		static object openFileLock = new object();
		static object saveFileLock = new object();
		static BinaryFormatter binaryFormatter;
		static BinaryFormatter BinaryFormatter {
			get { return binaryFormatter ?? (binaryFormatter = new BinaryFormatter()); }
		}
		protected internal TimeSpan ExpirationTime { get; private set; }
		protected internal DateTime LastAccessedTime { get; private set; }
		public string Path { get; private set; }
		public UploadToken(string path, int expirationTime) {
			Path = path;
			ExpirationTime = TimeSpan.FromMinutes(expirationTime);
			LastAccessedTime = DateTime.UtcNow;
		}
		public bool Expired() {			
			return LastAccessedTime < DateTime.UtcNow - ExpirationTime;
		}
		public BinaryStorageData GetBinaryStorageData() {
			BinaryStorageData ret = null;
			TryDo(() => {
				lock(openFileLock) {
					using(FileStream fs = new FileStream(Path, FileMode.Open))
						ret = BinaryFormatter.Deserialize(fs) as BinaryStorageData;
					ResetLastAccessTime();
				}
			});
			return ret;
		}
		public bool TrySaveBinaryStorageData(BinaryStorageData data) {
			return TryDo(() => {
				lock(saveFileLock) {
					using(FileStream fs = new FileStream(Path, FileMode.Create))
						BinaryFormatter.Serialize(fs, data);
					ResetLastAccessTime();
				}
			});
		}
		public void DeleteFile() {
			TryDo(() => File.Delete(Path));
		}
		void ResetLastAccessTime() { LastAccessedTime = DateTime.UtcNow; }
		static bool TryDo(Action action) {
			try {
				action();
				return true;
			} catch {
				return false;
			}
		}
	}
}
