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
using System.Linq;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[PredefinedFileSystemProvider]
	public class PhysicalFileSystemProvider : FileSystemProviderBase {
		public PhysicalFileSystemProvider(string rootFolder)
			: base(rootFolder) { }
		public override string GetRelativeFolderPath(FileManagerFolder folder, System.Web.UI.IUrlResolutionService rs) {
			string url = UrlUtils.ResolvePhysicalPath(rs, Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
			return url == "./" ? string.Empty : url.EndsWith("/") ? url : url + "/";
		}
		public override bool Exists(FileManagerFolder folder) {
			return Directory.Exists(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
		}
		public override bool Exists(FileManagerFile file) {
			return File.Exists(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
		}
#if !SL
	[DevExpressWebLocalizedDescription("PhysicalFileSystemProviderRootFolderDisplayName")]
#endif
		public override string RootFolderDisplayName {
			get {
				return FileManagerDataHelper.GetRootFolderName(GetResolvedRootFolderPath());
			}
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			DirectoryInfo pDir = new DirectoryInfo(Path.Combine(GetResolvedRootFolderPath(), parentFolder.RelativeName));
			foreach(DirectoryInfo dir in pDir.GetDirectories()) {
				yield return new FileManagerFolder(this, Path.Combine(parentFolder.RelativeName, dir.Name));
			}
		}
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			DirectoryInfo dir = new DirectoryInfo(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
			foreach(FileInfo f in dir.GetFiles()) {
				yield return new FileManagerFile(this, Path.Combine(folder.RelativeName, f.Name));
			}
		}
		public override long GetLength(FileManagerFile file) {
			return new FileInfo(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName)).Length;
		}
		public override Stream ReadFile(FileManagerFile file) {
			return new System.IO.FileStream(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			return File.GetLastWriteTime(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
		}
		public override DateTime GetLastWriteTime(FileManagerFolder folder) {
			return Directory.GetLastWriteTime(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
		}
		public override void DeleteFile(FileManagerFile file) {
			try {
				File.Delete(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			try {
				Directory.Delete(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), true);
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			try {
				File.Move(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, newParentFolder, file.Name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			try {
				Directory.Move(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, newParentFolder, folder.Name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			try {
				File.Copy(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, newParentFolder, file.Name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			try {
				string sourcePath = Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName);
				string destinationPath = Path.Combine(GetResolvedRootFolderPath(), newParentFolder.RelativeName, folder.Name);
				string[] subDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
				Directory.CreateDirectory(destinationPath);
				foreach(string dirPath in subDirectories)
					Directory.CreateDirectory(Path.Combine(destinationPath, dirPath.Substring(sourcePath.Length + 1)));
				foreach(string filePath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
					File.Copy(filePath, Path.Combine(destinationPath, filePath.Substring(sourcePath.Length + 1)));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void RenameFile(FileManagerFile file, string name) {
			try {
				File.Move(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, file.Folder, name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			try {
				Directory.Move(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, folder.Parent, name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			try {
				Directory.CreateDirectory(Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, parent, name).RelativeName));
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			try {
				string path = Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, folder, fileName).RelativeName);
				using(FileStream fileStream = File.Create(path)) {
					CommonUtils.CopyStream(content, fileStream);
				}
			}
			catch(Exception e) {
				SetError(e);
			}
		}
		public string GetResolvedRootFolderPath() {
			return UrlUtils.ResolvePhysicalPath(RootFolder);
		}
		void SetError(Exception exc) {
			if(exc is FileNotFoundException)
				throw new FileManagerIOException(FileManagerErrors.FileNotFound, exc);
			if(exc is UnauthorizedAccessException)
				throw new FileManagerIOException(FileManagerErrors.AccessDenied, exc);
			if(exc is DirectoryNotFoundException)
				throw new FileManagerIOException(FileManagerErrors.FolderNotFound, exc);
			if(exc is PathTooLongException)
				throw new FileManagerIOException(FileManagerErrors.PathToLong, exc);
			if(exc is IOException) {
				try { 
					switch(GetHRForException(exc)) { 
						case -2147024864:
							throw new FileManagerIOException(FileManagerErrors.UsedByAnotherProcess, exc);
						case -2146232800:
						case -2147024891:
							throw new FileManagerIOException(FileManagerErrors.AccessDenied, exc);
						case -2147024713:
							throw new FileManagerIOException(FileManagerErrors.AlreadyExists, exc);
					}
				}
				catch { }
				throw new FileManagerIOException(FileManagerErrors.UnspecifiedIO, exc);
			}
			throw new FileManagerException(FileManagerErrors.Unspecified);
		}
		static int GetHRForException(System.Exception exc) {
			return System.Runtime.InteropServices.Marshal.GetHRForException(exc);
		}
		static string MakeRelativePath(string fromPath, string toPath) {
			Uri fromUri = new Uri(fromPath.EndsWith("\\") ? fromPath : fromPath + "\\");
			Uri toUri = new Uri(toPath.EndsWith("\\") ? toPath : toPath + "\\");
			return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
		}
	}
}
