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
using System.Linq;
using System.Text;
using DevExpress.Web;
using System.Text.RegularExpressions;
using System.Collections;
using DevExpress.Web.Internal;
using System.IO;
namespace DevExpress.Web.Internal {
	[PredefinedFileSystemProvider]
	public class RestrictedAccessFileSystemProvider : FileSystemProviderBase, IEquatable<FileSystemProviderBase> {
		FileSystemProviderBase provider;
		ASPxFileManager fileManager;
		FileManagerAccessModelItem rootFolderAccessModelItem;
		public RestrictedAccessFileSystemProvider(FileSystemProviderBase provider, ASPxFileManager fileManager)
			:base(fileManager.Settings.RootFolder) {
			this.provider = provider;
			this.fileManager = fileManager;
		}
		public FileSystemProviderBase Provider { get { return provider; } }
		protected ASPxFileManager FileManager { get { return fileManager; } }
		protected Collection<FileManagerAccessRuleBase> AccessRules { get { return FileManager.SettingsPermissions.AccessRules; } }
		protected FileManagerSettingsEditing Editing { get { return FileManager.SettingsEditing; } }
		protected internal string Role { get { return FileManager.SettingsPermissions.Role; } }
		protected FileManagerAccessModelItem RootFolderAccessModelItem { 
			get {
				if(rootFolderAccessModelItem == null)
					CreateAccessModel();
				return rootFolderAccessModelItem; 
			}
		}
		public override string RootFolderDisplayName { get { return Provider.RootFolderDisplayName; } }
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			foreach(FileManagerFile file in Provider.GetFiles(folder)) {
				if(CanBrowse(file))
					yield return new FileManagerFile(this, file.RelativeName, file.Id);
			}
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			foreach(FileManagerFolder folder in Provider.GetFolders(parentFolder)) {
				if(CanBrowse(folder))
					yield return new FileManagerFolder(this, folder.RelativeName, folder.Id);
			}
		}
		public override long GetLength(FileManagerFile file) {
			if(!CanBrowse(file))
				throw new FileManagerAccessException();
			return Provider.GetLength(file);
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			if(!CanCreate(parent, name))
				throw new FileManagerAccessException();
			Provider.CreateFolder(parent, name);
		}
		public override void DeleteFile(FileManagerFile file) {
			if(!CanDelete(file))
				throw new FileManagerAccessException();
			Provider.DeleteFile(file);
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			if(!CanDelete(folder))
				throw new FileManagerAccessException();
			Provider.DeleteFolder(folder);
		}
		public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			if(!CanMove(file, newParentFolder))
				throw new FileManagerAccessException();
			Provider.MoveFile(file, newParentFolder);
		}
		public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			if(!CanCopy(file, newParentFolder))
				throw new FileManagerAccessException();
			Provider.CopyFile(file, newParentFolder);
		}
		public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			if(!CanMove(folder, newParentFolder))
				throw new FileManagerAccessException();
			Provider.MoveFolder(folder, newParentFolder);
		}
		public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			if(!CanCopy(folder, newParentFolder))
				throw new FileManagerAccessException();
			Provider.CopyFolder(folder, newParentFolder);
		}
		public override System.IO.Stream ReadFile(FileManagerFile file) {
			if(!CanBrowse(file))
				throw new FileManagerAccessException();
			return Provider.ReadFile(file);
		}
		public override void RenameFile(FileManagerFile file, string name) {
			if(!CanRename(file))
				throw new FileManagerAccessException();
			Provider.RenameFile(file, name);
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			if(!CanRename(folder))
				throw new FileManagerAccessException();
			Provider.RenameFolder(folder, name);
		}
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			FileManagerFile file = new FileManagerFile(this, folder, fileName);
			if(!CanUpload(file))
				throw new FileManagerAccessException();
			Provider.UploadFile(folder, fileName, content);
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			return Provider.GetLastWriteTime(file);
		}
		public override DateTime GetLastWriteTime(FileManagerFolder folder) {
			return Provider.GetLastWriteTime(folder);
		}
		public override string GetRelativeFolderPath(FileManagerFolder folder, System.Web.UI.IUrlResolutionService rs) {
			return Provider.GetRelativeFolderPath(folder, rs);
		}
		public override bool Exists(FileManagerFile file) {
			return Provider.Exists(file);
		}
		public override bool Exists(FileManagerFolder folder) {
			return Provider.Exists(folder);
		}
		public bool CanBrowse(FileManagerFile file) {
			bool result = !IsHiddenInvalidExtensionFile(file) && CanBrowse(file.Folder);
			if(result) {
				foreach(FileManagerFileAccessRule rule in AccessRules.FindAll(FindCurrentFilesRules)) {
					if(IsAppliedRule(rule, file) && rule.Browse != Rights.Default)
						result = rule.Browse == Rights.Allow;
				}
			}
			return result;
		}
		public bool CanBrowse(FileManagerFolder folder) {
			return IsHiddenAspNetFolder(folder) ? false : FindAccessModelItem(folder).CanBrowse;
		}
		public bool CanUpload(FileManagerFolder folder) {
			return FileManager.SettingsUpload.Enabled && FindAccessModelItem(folder).CanUpload;
		}
		public bool CanUpload(FileManagerFile file) {
			return CanUpload(file.Folder) && CanEdit(file);
		}
		public bool CanRename(FileManagerFile file) {
			return Editing.AllowRename && CanEdit(file);
		}
		public bool CanRename(FileManagerFolder folder) {
			return folder.Parent != null && Editing.AllowRename && CanEdit(folder, true);
		}
		public bool CanMove(FileManagerFile file, FileManagerFolder newParentFolder) {
			return CanMove(file) && CanAddChild(newParentFolder);
		}
		public bool CanMove(FileManagerFile file) {
			return Editing.AllowMove && CanEdit(file);
		}
		public bool CanCopy(FileManagerFile file, FileManagerFolder newParentFolder) {
			return CanCopy(file) && CanAddChild(newParentFolder);
		}
		public bool CanCopy(FileManagerFile file) {
			return Editing.AllowCopy && CanEdit(file);
		}
		public bool CanMove(FileManagerFolder folder) {
			return folder.Parent != null && Editing.AllowMove && CanEdit(folder, true);
		}
		public bool CanMove(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			return folder.Parent != null && CanMove(folder) && CanAddChild(newParentFolder);
		}
		public bool CanCopy(FileManagerFolder folder) {
			return Editing.AllowCopy && CanEdit(folder, true);
		}
		public bool CanCopy(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			return CanCopy(folder) && CanAddChild(newParentFolder);
		}
		public bool CanCreate(FileManagerFolder folder) {
			return Editing.AllowCreate && CanAddChild(folder);
		}
		public bool CanCreate(FileManagerFolder folder, string name) {
			return CanCreate(folder) && CanEdit(new FileManagerFolder(this, folder, name), false);
		}
		public bool CanDelete(FileManagerFolder folder) {
			return folder.Parent != null && Editing.AllowDelete && CanEdit(folder, true);
		}
		public bool CanDelete(FileManagerFile file) {
			return Editing.AllowDelete && CanEdit(file);
		}
		public bool CanDownload(FileManagerFile file) {
			bool result = Editing.AllowDownload && FindAccessModelItem(file.Folder).FindOrDefault("*").CanBrowse;
			if(result) {
				foreach(FileManagerFileAccessRule rule in AccessRules.FindAll(FindCurrentFilesRules)) {
					if(IsAppliedRule(rule, file)) {
						if(rule.Download != Rights.Default)
							result = rule.Download == Rights.Allow;
					}
				}
			}
			return result;
		}
		bool CanEdit(FileManagerFile file) {
			FileManagerAccessModelItem item = FindAccessModelItem(file.Folder);
			bool result = item.FindOrDefault("*").CanEdit;
			if(result) {
				foreach(FileManagerFileAccessRule rule in AccessRules.FindAll(FindCurrentFilesRules)) {
					if(IsAppliedRule(rule, file)) {
						if(rule.Edit != Rights.Default)
							result = rule.Edit == Rights.Allow;
						if(rule.Browse != Rights.Default)
							result = rule.Browse == Rights.Allow;
					}
				}
			}
			return result;
		}
		bool CanEdit(FileManagerFolder folder, bool checkChildRules) {
			FileManagerAccessModelItem item = FindAccessModelItem(folder);
			return checkChildRules ? item.CanEdit && !item.HasUneditableChild() : item.CanEdit;
		}
		internal bool CanAddChild(FileManagerFolder folder) {
			return CanEdit(new FileManagerFolder(this, folder, "*"), false);
		}
		protected bool IsHiddenAspNetFolder(FileManagerFolder folder) {
			return FileManager.SettingsFolders.HideAspNetFolders && IsAspNetFolder(folder);
		}
		protected bool IsHiddenInvalidExtensionFile(FileManagerFile file) {
			if(FileManager.Settings.AllowedFileExtensions.Length == 0)
				return false;
			string extension = Path.GetExtension(file.Name);
			return Array.FindIndex<string>(FileManager.Settings.AllowedFileExtensions, delegate(string ext) {
				return ext.Equals(extension, StringComparison.InvariantCultureIgnoreCase);
			}) == -1;
		}
		protected internal void ResetAccessModel() {
			this.rootFolderAccessModelItem = null;
		}
		protected void CreateAccessModel() {
			this.rootFolderAccessModelItem = new FileManagerAccessModelItem(null, "", true, true, true);
			if(FileManager.SettingsUpload.AllowedFolderInternal == FileManagerAllowedFolder.SpecificOnly) {
				RootFolderAccessModelItem.CanUpload = false;
				FileManagerFolder allowUploadFolder = new FileManagerFolder(this, FileManager.SettingsUpload.AllowedFolderPathInternal);
				FindAccessModelItem(allowUploadFolder.RelativeName, true).CanUploadCore = true;
			}
			foreach(FileManagerFolderAccessRule rule in AccessRules.FindAll(FindCurrentFoldersRules)) {
				FileManagerAccessModelItem item = FindAccessModelItem(rule.Path, true);
				if(rule.EditContents != Rights.Default)
					item.SetEditRightsRecursively(rule.EditContents == Rights.Allow);
				if(rule.Edit != Rights.Default)
					item.CanEdit = rule.Edit == Rights.Allow;
				if(rule.Browse != Rights.Default)
					item.CanBrowse = rule.Browse == Rights.Allow;
				if(rule.Upload != Rights.Default)
					item.CanUpload = rule.Upload == Rights.Allow;
			}
		}
		protected FileManagerAccessModelItem FindAccessModelItem(FileManagerFolder folder) {
			return FindAccessModelItem(folder.RelativeName, false);
		}
		protected FileManagerAccessModelItem FindAccessModelItem(string folderPath, bool createIfNotExists) {
			string[] paths = folderPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			FileManagerAccessModelItem item = RootFolderAccessModelItem;
			for(int i = 0; i < paths.Length; i++) {
				if(FileManagerItem.DotsPathRegex.IsMatch(paths[i]))
					throw new FileManagerException(FileManagerErrors.Unspecified);
				item = createIfNotExists ? item.FindOrCreate(paths[i]) : item.FindOrDefault(paths[i]);
			}
			return item;
		}
		bool IsPatternAppliedToPath(string pattern, string path) {
			string regexPattern = "^" + Regex.Escape(pattern.Trim('\\')).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
			Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			return regex.IsMatch(path);
		}
		bool IsAppliedRule(FileManagerFileAccessRule rule, FileManagerFile file) {
			return IsPatternAppliedToPath(rule.Path, file.RelativeName);
		}
		internal bool IsAppliedToChildFileRule(FileManagerAccessRuleBase rule, FileManagerFolder folder) {
			string pathPattern = Path.GetDirectoryName(rule.Path);
			bool result = IsPatternAppliedToPath(pathPattern, folder.RelativeName);
			if(!result) {
				string fileNamePattern = Path.GetFileName(rule.Path);
				int asteriskIndex = fileNamePattern.IndexOf("*");
				if(asteriskIndex > -1) {
					asteriskIndex = rule.Path.LastIndexOf('\\') + 1 + asteriskIndex;
					pathPattern = rule.Path.Substring(0, asteriskIndex + 1);
					result = IsPatternAppliedToPath(pathPattern, folder.RelativeName);
				}
			}
			return result;
		}
		bool IsAspNetFolder(FileManagerFolder folder) {
			return new ArrayList(new string[] { "App_Code", "App_Data", "App_Themes", "Bin", "App_GlobalResources", "App_LocalResources", "App_WebReferences", "App_Browsers" })
				.Contains(folder.Name);
		}
		bool FindCurrentFoldersRules(FileManagerAccessRuleBase rule) {
			return rule is FileManagerFolderAccessRule && IsRoleMatch(rule) && !rule.Path.Contains("*");
		}
		bool FindCurrentFilesRules(FileManagerAccessRuleBase rule) {
			return rule is FileManagerFileAccessRule && IsRoleMatch(rule);
		}
		bool IsRoleMatch(FileManagerAccessRuleBase rule) {
			return string.IsNullOrEmpty(rule.Role) || rule.Role.Equals(Role, StringComparison.InvariantCultureIgnoreCase);
		}
		internal IEnumerable<FileManagerAccessRuleBase> GetChildFilesRules(FileManagerFolder folder) {
			return AccessRules.FindAll(FindCurrentFilesRules).Where(rule => IsAppliedToChildFileRule(rule, folder));
		}
		#region IEquatable<FileSystemProviderBase> Members
		public bool Equals(FileSystemProviderBase other) {
			return base.Equals(other) || Provider.Equals(other);
		}
		#endregion
	}
	public class FileManagerAccessModelItem {
		bool canEdit;
		bool canUpload;
		bool canBrowse;
		List<FileManagerAccessModelItem> childs;
		FileManagerAccessModelItem parent;
		FileManagerAccessModelItem defaultItem;
		string name;
		public FileManagerAccessModelItem(FileManagerAccessModelItem parent, string name, bool canEdit, bool canBrowse, bool canUpload) {
			this.name = name;
			this.canEdit = canEdit;
			this.canUpload = canUpload;
			this.canBrowse = canBrowse;
			this.parent = parent;
			this.childs = new List<FileManagerAccessModelItem>();
			if(name != "*" || parent == null) {
				this.defaultItem = new FileManagerAccessModelItem(this, "*", canEdit, canBrowse, canUpload);
				Childs.Add(DefaultItem);
			}
		}
		public FileManagerAccessModelItem DefaultItem { get { return defaultItem != null ? defaultItem : this; } }
		public string Name { get { return name; } }
		public bool CanEdit {
			get { return canEdit && CanBrowse; }
			set {
				canEdit = value;
				SetEditRightsRecursively(canEdit);
			}
		}
		public bool CanBrowse {
			get { return canBrowse; }
			set {
				canBrowse = value;
				SetBrowseRightsRecursively(canBrowse);
			}
		}
		public bool CanUpload {
			get { return canUpload && DefaultItem.CanEdit; }
			set {
				canUpload = value;
				SetUploadRightsRecursively(canUpload);
			}
		}
		public bool CanUploadCore { get { return canUpload; } set { canUpload = value; } }
		protected FileManagerAccessModelItem Parent { get { return parent; } }
		protected List<FileManagerAccessModelItem> Childs { get { return childs; } }
		public bool HasUneditableChild() {
			foreach(FileManagerAccessModelItem item in Childs) {
				if(!item.CanEdit || !item.CanBrowse)
					return true;
				if(item.HasUneditableChild())
					return true;
			}
			return false;
		}
		public FileManagerAccessModelItem FindOrCreate(string name) {
			FileManagerAccessModelItem item = Find(name);
			if(item == null) {
				item = new FileManagerAccessModelItem(this, name, DefaultItem.CanEdit, DefaultItem.CanBrowse, DefaultItem.CanUploadCore);
				Childs.Add(item);
			}
			return item;
		}
		public FileManagerAccessModelItem FindOrDefault(string name) {
			if(Parent != null && Name == "*")
				return this;
			FileManagerAccessModelItem item = Find(name);
			return item == null ? DefaultItem : item;
		}
		public void SetEditRightsRecursively(bool canEdit) {
			foreach(FileManagerAccessModelItem item in Childs) {
				item.CanEdit = canEdit;
			}
		}
		public void SetBrowseRightsRecursively(bool canBrowse) {
			foreach(FileManagerAccessModelItem item in Childs) {
				item.CanBrowse = canBrowse;
			}
		}
		public void SetUploadRightsRecursively(bool canUpload) {
			foreach(FileManagerAccessModelItem item in Childs) {
				item.CanUpload = canUpload;
			}
		}
		FileManagerAccessModelItem Find(string name) {
			return Childs.Find(delegate(FileManagerAccessModelItem item) {
				return item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
			});
		}
	}
}
