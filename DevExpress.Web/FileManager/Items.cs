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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using DevExpress.Web.Internal;
using System.Web.UI;
using DevExpress.Web.Design;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class FileManagerFolder : FileManagerItem, IEquatable<FileManagerFolder> {
		internal const string CreateHelperFolderTypeName = "aspxFMCreateHelperFolder";
		public FileManagerFolder(FileSystemProviderBase provider, string relativeName)
			: base(provider, relativeName) { }
		public FileManagerFolder(FileSystemProviderBase provider, string relativeName, string id)
			: base(provider, relativeName, id) { }
		public FileManagerFolder(FileSystemProviderBase provider, string relativeName, string[] idPath)
			: base(provider, relativeName, idPath) { }
		public FileManagerFolder(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name)
			: base(provider, parentFolder, name) { }
		public FileManagerFolder(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name, string id)
			: base(provider, parentFolder, name, id) { }
		protected internal static FileManagerFolder Create(FileSystemProviderBase provider, FileManagerFolder parentFolder, string folderName, string[] idPath) {
			string relativeName = GetRelativeName(parentFolder, folderName);
			return new FileManagerFolder(provider, relativeName, idPath);
		}
		protected override string GetName() {
			if(IsParentFolderItem)
				return "..";
			if(string.IsNullOrEmpty(RelativeName))
				return Provider.RootFolderDisplayName;
			else {
				int sepIndex = RelativeName.LastIndexOf('\\');
				return sepIndex > -1 ? RelativeName.Remove(0, sepIndex + 1) : RelativeName;
			}
		}
		protected override DateTime GetLastWriteTime() { return IsCreateHelperItem ? DateTime.Now : Provider.GetLastWriteTime(this); }
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerFolderParent")]
#endif
		public FileManagerFolder Parent { get { return GetParent(); } }
		internal bool IsParentFolderItem { get; set; }
		internal bool IsCreateHelperItem { get; set; }
		public FileManagerFolder[] GetFolders() {
			return new List<FileManagerFolder>(Provider.GetFolders(this)).ToArray();
		}
		public FileManagerFile[] GetFiles() {
			return new List<FileManagerFile>(Provider.GetFiles(this)).ToArray();
		}
		public bool Equals(FileManagerFolder other) {
			return base.Equals((FileManagerItem)other);
		}
	}
	public class FileManagerFile : FileManagerItem, IEquatable<FileManagerFile> {
		public FileManagerFile(FileSystemProviderBase provider, string relativeName)
			: base(provider, relativeName) { }
		public FileManagerFile(FileSystemProviderBase provider, string relativeName, string id)
			: base(provider, relativeName, id) { }
		public FileManagerFile(FileSystemProviderBase provider, string relativeName, string[] idPath)
			: base(provider, relativeName, idPath) { }
		public FileManagerFile(FileSystemProviderBase provider, FileManagerFolder parentFolder, string fileName)
			: base(provider, parentFolder, fileName) { }
		public FileManagerFile(FileSystemProviderBase provider, FileManagerFolder parentFolder, string fileName, string id)
			: base(provider, parentFolder, fileName, id) { }
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerFileFolder")]
#endif
		public FileManagerFolder Folder { get { return GetParent(); } }
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerFileExtension")]
#endif
		public string Extension { get { return GetExtension(Name); } }
		protected internal static FileManagerFile Create(FileSystemProviderBase provider, FileManagerFolder parentFolder, string fileName, string[] idPath) {
			string relativeName = GetRelativeName(parentFolder, fileName);
			return new FileManagerFile(provider, relativeName, idPath);
		}
		protected override string GetName() {
			var name = string.Empty;
			try {
				name = Path.GetFileName(RelativeName);
			} catch {
				var pos = RelativeName.Length;
				while(--pos >= 0) {
					char ch = RelativeName[pos];
					if(Array.IndexOf(FileManagerItem.Separators, ch) != -1)
						return RelativeName.Substring(pos + 1);
				}
				name = RelativeName;
			}
			return name;
		}
		protected override DateTime GetLastWriteTime() { return Provider.GetLastWriteTime(this); }
		protected override long GetLength() { return Provider.GetLength(this); }
		public bool Equals(FileManagerFile other) {
			return base.Equals((FileManagerItem)other);
		}
		internal static string GetExtension(string name) {
			var extension = string.Empty;
			try {
				extension = Path.GetExtension(name);
			} catch {
				int pos = name.Length - 1;
				while(--pos >= 0) {
					if(name[pos] == '.')
						return name.Substring(pos);
				}
			}
			return extension; 
		}
	}
	public abstract class FileManagerItem: IEquatable<FileManagerItem> {
		internal static char[] Separators = new char[] { '\\', '/' };
		internal static System.Text.RegularExpressions.Regex DotsPathRegex = new System.Text.RegularExpressions.Regex(@"^[\.]+$");
		string relativeName;
		string id;
		FileSystemProviderBase provider;
		protected internal bool HasId { get; set; }
		protected internal string IdInternal {
			get { return HasId ? Id : string.Empty; }
		}
		protected FileManagerItem(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name)
			: this(provider, parentFolder, name, string.Empty) { }
		protected FileManagerItem(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name, string id)
			: this(provider, GetRelativeName(parentFolder, name), id) { }
		protected FileManagerItem(FileSystemProviderBase provider, string relativeName)
			: this(provider, relativeName, string.Empty) { }
		protected FileManagerItem(FileSystemProviderBase provider, string relativeName, string[] idPath)
			: this(provider, relativeName, idPath.Length > 0 ? idPath[idPath.Length - 1] : string.Empty) {
				IdPath = idPath;
		}
		protected FileManagerItem(FileSystemProviderBase provider, string relativeName, string id) {
			if(relativeName.Contains(@"\\") || relativeName.Contains(@":\"))
				throw new FileManagerException(FileManagerErrors.Unspecified);
			this.relativeName = relativeName.Trim(Separators).Replace('/', '\\');
			HasId = id != string.Empty;
			this.id = HasId ? id : this.relativeName;
			foreach(var pathPart in this.relativeName.Split(Separators)) {
				if(DotsPathRegex.IsMatch(pathPart))
					throw new FileManagerException(FileManagerErrors.Unspecified);
			}
			this.provider = provider;
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerItemRelativeName")]
#endif
		public string RelativeName { get { return this.relativeName; } }
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerItemFullName")]
#endif
		public string FullName {
			get { return PathCombine(Provider.RootFolder, RelativeName).Replace('/', '\\'); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerItemName")]
#endif
		public string Name { get { return GetName(); } }
		public string Id { get { return this.id; } }
		public DateTime LastWriteTime { get { return GetLastWriteTime(); } }
		public long Length { get { return GetLength(); } }
		public string ThumbnailUrl { get; internal set; }
		protected string[] IdPath { get; set; }
		internal Hashtable Script { get; set; }
		internal bool ClientInvisible { get; set; }
		protected virtual long GetLength() { return 0; }
		protected abstract string GetName();
		protected abstract DateTime GetLastWriteTime();
		protected internal FileSystemProviderBase Provider { get { return provider; } }
		protected internal static void ThrowWrongIdPathLengthException() {
			throw new FileManagerException(FileManagerErrors.WrongIdPathLength);
		}
		protected static string GetRelativeName(FileManagerFolder parentFolder, string name) {
			return PathCombine(parentFolder.RelativeName, name.Trim(Separators));
		}
		protected FileManagerFolder GetParent() {
			if(IdPath != null) {
				if(IdPath.Length != GetPathItemCount(RelativeName))
					ThrowWrongIdPathLengthException();
				if(IdPath.Length > 1) {
					string[] parentIdPath = new string[IdPath.Length - 1];
					Array.Copy(IdPath, 0, parentIdPath, 0, IdPath.Length - 1);
					return new FileManagerFolder(Provider, GetParentName(), parentIdPath);
				}
				return null;
			}
			FileManagerFolder parent = Provider.GetParentFolder(Id, RelativeName);
			if(parent != null)
				return parent;
			return string.IsNullOrEmpty(RelativeName)
				? null
				: new FileManagerFolder(Provider, GetParentName());
		}
		protected internal static int GetPathItemCount(string path) {
			if(string.IsNullOrEmpty(path))
				return 1;
			int pathPartCount = path.Split(Separators).Length;
			return pathPartCount + 1;
		}
		protected internal string GetParentName() {
			return GetParentName(RelativeName);
		}
		protected internal static string GetParentName(string path) {
			int separatorLastIndex = path.LastIndexOfAny(Separators);
			return separatorLastIndex > -1
				? path.Substring(0, separatorLastIndex)
				: string.Empty;
		}
		public override string ToString() {
			return FullName;
		}
		internal static string PathCombine(string path1, string path2) {
			var path = string.Empty;
			try {
				path = Path.Combine(path1, path2);
			} catch {
				if(string.IsNullOrEmpty(path1))
					path = path2;
				else if(string.IsNullOrEmpty(path2))
					path = path1;
				else if(Array.IndexOf(Separators, path1[path1.Length - 1]) > -1)
					path = path1 + path2;
				else
					path = path1 + Separators[0] + path2;
			}
			return path;
		}
		#region IEquatable<FileManagerItem> Members
		public bool Equals(FileManagerItem other) {
			if(RelativeName != other.RelativeName)
				return false;
			RestrictedAccessFileSystemProvider restrProv1 = Provider as RestrictedAccessFileSystemProvider;
			RestrictedAccessFileSystemProvider restrProv2 = other.Provider as RestrictedAccessFileSystemProvider;
			if(restrProv1 != null && restrProv2 != null)
				return restrProv1 == restrProv2;
			if(restrProv1 != null)
				return restrProv1.Equals(other.Provider);
			if(restrProv2 != null)
				return restrProv2.Equals(Provider);
			return Provider.Equals(other.Provider);
		}
		#endregion
	}
	public abstract class ToolbarItemCollectionBase<T> : Collection<T> where T : FileManagerToolbarItemBase {
		public ToolbarItemCollectionBase()
			: base() {
		}
		public ToolbarItemCollectionBase(IWebControlObject owner)
			: base(owner) {
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class FileManagerToolbarItemCollection : ToolbarItemCollectionBase<FileManagerToolbarItemBase> {
		public FileManagerToolbarItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public FileManagerToolbarItemBase FindByCommandName(string commandName) {
			return Find(delegate(FileManagerToolbarItemBase item) {
				return item.CommandName == commandName;
			});
		}
		public void CreateDefaultItems() {
			AddRange(GetDefaultItems());
		}
		static internal List<FileManagerToolbarItemBase> GetDefaultItems() {
			return new List<FileManagerToolbarItemBase>() {
				new FileManagerToolbarCreateButton(),
				new FileManagerToolbarRenameButton(),
				new FileManagerToolbarMoveButton(),
				new FileManagerToolbarCopyButton(),
				new FileManagerToolbarDeleteButton(),
				new FileManagerToolbarRefreshButton(),
				new FileManagerToolbarDownloadButton(),
				new FileManagerToolbarUploadButton()
			};
		}
	}
	public abstract class FileManagerToolbarItemBase : CollectionItem {
		ImagePropertiesBase image;
		[DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", string.Empty, value);
				LayoutChanged();
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public virtual string ToolTip {
			get { return GetStringProperty("ToolTip", GetDefaultToolTip()); }
			set {
				SetStringProperty("ToolTip", "", value);
				LayoutChanged();
			}
		}
		[DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", false); }
			set {
				SetBoolProperty("BeginGroup", false, value);
				LayoutChanged();
			}
		}
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string CommandName {
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		[Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ImagePropertiesBase Image {
			get {
				if(image == null)
					image = new ImagePropertiesBase(this);
				return image;
			}
		}
		protected virtual string GetDefaultToolTip() {
			return string.Empty;
		}
		protected internal virtual bool GetClientEnabled() {
			return false;
		}
		protected internal virtual string GetResourceImageName() {
			return string.Empty;
		}
		protected internal ImagePropertiesBase GetImageProperties(FileManagerHelper helper) {
			string resourceName = GetResourceImageName();
			if(string.IsNullOrEmpty(resourceName))
				return Image;
			else {
				ImagePropertiesBase image = helper.GetImage(resourceName);
				image.CopyFrom(Image);
				return image;
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerToolbarItemBase src = source as FileManagerToolbarItemBase;
			if(src != null) {
				Text = src.Text;
				ToolTip = src.ToolTip;
				BeginGroup = src.BeginGroup;
				CommandName = src.CommandName;
				Image.Assign(src.Image);
			}
		}
	}
	public class FileManagerToolbarCustomButton : FileManagerToolbarItemBase {
		MenuItemStyle itemStyle = null;
		[DefaultValue(false), NotifyParentProperty(true)]
		public bool Checked {
			get { return GetBoolProperty("Checked", false); }
			set {
				if(Checked != value) {
					SetBoolProperty("Checked", false, value);
					LayoutChanged();
				}
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string GroupName {
			get { return GetStringProperty("GroupName", ""); }
			set {
				if(GroupName != value)
					SetStringProperty("GroupName", "", value);
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				if(Enabled != value) {
					SetBoolProperty("Enabled", true, value);
					LayoutChanged();
				}
			}
		}
		[DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				if(Visible != value) {
					SetBoolProperty("Visible", true, value);
					LayoutChanged();
				}
			}
		}
		[DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientEnabled {
			get { return GetBoolProperty("ClientEnabled", true); }
			set {
				if(ClientEnabled != value) {
					SetBoolProperty("ClientEnabled", true, value);
					LayoutChanged();
				}
			}
		}
		[DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ClientVisible {
			get { return GetBoolProperty("ClientVisible", true); }
			set {
				if(ClientVisible != value) {
					SetBoolProperty("ClientVisible", true, value);
					LayoutChanged();
				}
			}
		}
		[Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuItemStyle ItemStyle {
			get {
				if(itemStyle == null)
					itemStyle = new MenuItemStyle();
				return itemStyle;
			}
		}
		protected internal override bool GetClientEnabled() {
			return ClientEnabled;
		}
		public override void Assign(CollectionItem source) {		   
			if(source is FileManagerToolbarCustomButton) {
				FileManagerToolbarCustomButton src = source as FileManagerToolbarCustomButton;
				Enabled = src.Enabled;
				Visible = src.Visible;
				ClientEnabled = src.ClientEnabled;
				ClientVisible = src.ClientVisible;
				Checked = src.Checked;
				GroupName = src.GroupName;
				ItemStyle.Assign(src.ItemStyle);
			}
			base.Assign(source);
		}
	}
	public class FileManagerToolbarCustomDropDownButton : FileManagerToolbarCustomButton {
		FileManagerToolbarItemCollection items = null;
		ItemImageProperties popOutImage = null;
		MenuStyle subMenuStyle = null;
		[DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool DropDownMode {
			get { return GetBoolProperty("DropDownMode", false); }
			set { SetBoolProperty("DropDownMode", false, value); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerToolbarItemCollection Items {
			get {
				if(items == null)
					items = new FileManagerToolbarItemCollection(this);
				return items;
			}
		}
		[Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ItemImageProperties PopOutImage {
			get {
				if(popOutImage == null)
					popOutImage = new ItemImageProperties(this);
				return popOutImage;
			}
		}
		[Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuStyle SubMenuStyle {
			get {
				if(subMenuStyle == null)
					subMenuStyle = new MenuStyle();
				return subMenuStyle;
			}
		}
		public override void Assign(CollectionItem source) {
			if(source is FileManagerToolbarCustomDropDownButton) {
				FileManagerToolbarCustomDropDownButton src = source as FileManagerToolbarCustomDropDownButton;
				DropDownMode = src.DropDownMode;
				PopOutImage.Assign(src.PopOutImage);
				SubMenuStyle.Assign(src.SubMenuStyle);
				Items.Assign(src.Items);
			}
			base.Assign(source);
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class FileManagerToolbarCreateButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Create"; }
		}
		[DefaultValue(true), NotifyParentProperty(true)]
		public override bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", true); }
			set {
				SetBoolProperty("BeginGroup", true, value);
				LayoutChanged();
			}
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbCreate);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.CreateButtonImageName;
		}
	}
	public class FileManagerToolbarRenameButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Rename"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbRename);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.RenameButtonImageName;
		}
	}
	public class FileManagerToolbarMoveButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Move"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbMove);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.MoveButtonImageName;
		}
	}
	public class FileManagerToolbarCopyButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Copy"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbCopy);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.CopyButtonImageName;
		}
	}
	public class FileManagerToolbarDeleteButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Delete"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbDelete);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.DeleteButtonImageName;
		}
	}
	public class FileManagerToolbarRefreshButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Refresh"; }
		}
		[DefaultValue(true), NotifyParentProperty(true)]
		public override bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", true); }
			set {
				SetBoolProperty("BeginGroup", true, value);
				LayoutChanged();
			}
		}
		protected internal override bool GetClientEnabled() {
			return true;
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbRefresh);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.RefreshButtonImageName;
		}
	}
	public class FileManagerToolbarDownloadButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Download"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_TbDownload);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.DownloadButtonImageName;
		}
	}
	public class FileManagerToolbarUploadButton : FileManagerToolbarItemBase {
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CommandName {
			get { return "Upload"; }
		}
		protected override string GetDefaultToolTip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadButton);
		}
		protected internal override string GetResourceImageName() {
			return FileManagerImages.UploadButtonImageName;
		}
		protected internal override bool GetClientEnabled() {
			return true;
		}
	}
}
