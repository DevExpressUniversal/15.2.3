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
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[PredefinedFileSystemProvider]
	public abstract class FileSystemProviderBase {
		static char[] Separators = new char[] { '\\', '/' };
		string rootFolder;
		public FileSystemProviderBase(string rootFolder) {
			this.rootFolder = rootFolder;
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileSystemProviderBaseRootFolder")]
#endif
		public string RootFolder { get { return rootFolder; } }
		public virtual IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			throw new NotImplementedException();
		}
		public virtual IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			throw new NotImplementedException();
		}
		public virtual FileManagerFolder GetParentFolder(string childId, string childRelativeName) {
			return null;
		}
		public virtual Stream ReadFile(FileManagerFile file) {
			throw new NotImplementedException();
		}
		public virtual DateTime GetLastWriteTime(FileManagerFile file) {
			return DateTime.Now;
		}
		public virtual DateTime GetLastWriteTime(FileManagerFolder folder) {
			var files = folder.GetFiles();
			if(files.Length == 0)
				return DateTime.Now;
			var result = DateTime.MinValue;
			foreach(var file in files) {
				var fileLastWriteTime = GetLastWriteTime(file);
				if(fileLastWriteTime > result)
					result = fileLastWriteTime;
			}
			return result;
		}
		public virtual string GetThumbnailUrl(FileManagerFile file) {
			return null;
		}
		public virtual Stream GetThumbnail(FileManagerFile file) {
			return null;
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileSystemProviderBaseRootFolderDisplayName")]
#endif
public virtual string RootFolderDisplayName {
			get {
				return FileManagerDataHelper.GetRootFolderName(RootFolder);
			}
		}
		public virtual string GetRelativeFolderPath(FileManagerFolder folder, IUrlResolutionService rs) {
			return folder.RelativeName.Replace('\\', '/');
		}
		public virtual bool Exists(FileManagerFolder folder) {
			return true;
		}
		public virtual bool Exists(FileManagerFile file) {
			return true;
		}
		public virtual long GetLength(FileManagerFile file) {
			return 0;
		}
		public virtual void CreateFolder(FileManagerFolder parent, string name) {
			throw new NotImplementedException();
		}
		public virtual void RenameFile(FileManagerFile file, string name) {
			throw new NotImplementedException();
		}
		public virtual void RenameFolder(FileManagerFolder folder, string name) {
			throw new NotImplementedException();
		}
		public virtual void DeleteFile(FileManagerFile file) {
			throw new NotImplementedException();
		}
		public virtual void DeleteFolder(FileManagerFolder folder) {
			throw new NotImplementedException();
		}
		public virtual void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			throw new NotImplementedException();
		}
		public virtual void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			throw new NotImplementedException();
		}
		public virtual void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			throw new NotImplementedException();
		}		
		public virtual void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			throw new NotImplementedException();
		}
		public virtual void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			throw new NotImplementedException();
		}
		public virtual string GetDetailsCustomColumnDisplayText(FileManagerDetailsColumn column) {
			return string.Empty;
		}
	}
}
namespace DevExpress.Web.Internal {
	public class PredefinedFileSystemProviderAttribute : Attribute { }
}
