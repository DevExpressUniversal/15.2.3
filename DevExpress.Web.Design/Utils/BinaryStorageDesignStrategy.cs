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
using System.IO;
using System.Web.UI.Design;
using DevExpress.Web.Design;
namespace DevExpress.Web.Internal {
	public class DesignModeStorageStrategy : StorageStrategy {
		public const int CacheClearingInterval = 10;
		public const int CacheExpirationTime = 20;
		public const string ResourcesFolderName = "BinaryData";
		public static DateTime LastCacheClearing = DateTime.MinValue;
		public override bool CanStoreData(ASPxWebControlBase control) {
			return true;
		}
		public override string GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType, string contentDisposition) {
			IWebApplication webApplication = (IWebApplication)control.Page.Site.GetService(typeof(IWebApplication));
			if(webApplication != null) {
				string fileName = GetControlUniqueName(control);
				string uniqueToken = Guid.NewGuid().ToString();
				string physicalPath = EnvDTEHelper.GetPhysicalPathByUrl("~/", webApplication);
				if(DesignUtils.IsWebApplication(control.Page.Site))
					physicalPath += "\\";
				physicalPath += RenderUtils.DefaultTempDirectory;
				FileUtils.EnsureDirectoryInFileSystemCreated(physicalPath, true);
				physicalPath += "\\" + ResourcesFolderName + "\\";
				FileUtils.EnsureDirectoryInFileSystemCreated(physicalPath, false);
				ClearFileCache(physicalPath);
				DeletePreviousFileVersion(physicalPath, fileName);
				physicalPath += uniqueToken + fileName;
				string fileExtension = GetFileExtention(mimeType);
				physicalPath += fileExtension;
				using(FileStream stream = new FileStream(physicalPath, FileMode.OpenOrCreate)) {
					stream.Write(content, 0, content.Length);
				}
				return "~/" + RenderUtils.DefaultTempDirectory + "/" + ResourcesFolderName + "/" + uniqueToken + fileName + fileExtension;
			}
			return String.Empty;
		}
		public override bool ProcessRequest(string key) {
			return false;
		}
		private void DeletePreviousFileVersion(string physicalPath, string fileName) {
			string[] files = Directory.GetFiles(physicalPath);
			foreach(string file in files) {
				if(file.EndsWith(fileName) || file.Contains(fileName + '.')) {
					try {
						File.Delete(file);
					}
					catch { }
					return;
				}
			}
		}
		private static void ClearFileCache(string physicalPath) {
			DateTime now = DateTime.Now;
			if(now - LastCacheClearing < TimeSpan.FromMinutes(CacheClearingInterval)) return;
			string[] files = Directory.GetFiles(physicalPath);
			foreach(string file in files) {
				if(now - File.GetCreationTime(file) > TimeSpan.FromMinutes(CacheExpirationTime)) {
					try {
						File.Delete(file);
					}
					catch { }
				}
			}
			LastCacheClearing = now;
		}
		private static string GetFileExtention(string mimeType) {
			string fileExtension = "";
			if(mimeType.StartsWith("image/"))
				fileExtension = "." + mimeType.Substring("image/".Length);
			return fileExtension;
		}
	}
}
