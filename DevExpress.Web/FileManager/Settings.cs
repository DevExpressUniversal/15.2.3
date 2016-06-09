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
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.Web;
namespace DevExpress.Web {
	public abstract class FileManagerSettingsBase : PropertiesBase, IPropertiesOwner {
		public FileManagerSettingsBase(IPropertiesOwner owner) : base(owner) { }
		protected new ASPxWebControl Owner { get { return (ASPxWebControl)base.Owner; } }
		public void Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class FileManagerSettings : FileManagerSettingsBase {
		public FileManagerSettings(IPropertiesOwner owner) : base(owner) { }
		ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsRootFolder"),
#endif
		Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public virtual string RootFolder {
			get { return GetStringProperty("RootFolder", GetDefaultRootFolder()); }
			set {
				if(value == RootFolder) return;
				SetStringProperty("RootFolder", GetDefaultRootFolder(), value);
				if(FileManager != null)
					FileManager.ResetFileSystemProvider();
				Changed();
			}
		}
		protected virtual string GetDefaultRootFolder() {
			return "";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsInitialFolder"),
#endif
		Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public virtual string InitialFolder {
			get { return GetStringProperty("InitialFolder", GetDefaultInitialFolder()); }
			set {
				if(value == InitialFolder) return;
				SetStringProperty("InitialFolder", GetDefaultInitialFolder(), value);
				if(FileManager != null)
					FileManager.ResetFileSystemProvider();
				Changed();
			}
		}
		protected virtual string GetDefaultInitialFolder() {
			return "";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsThumbnailFolder"),
#endif
		Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public virtual string ThumbnailFolder {
			get { return GetStringProperty("ThumbnailFolder", GetDefaultThumbnailFolder()); }
			set { SetStringProperty("ThumbnailFolder", GetDefaultThumbnailFolder(), value); }
		}
		protected virtual string GetDefaultThumbnailFolder() {
			return "";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsThumbnailSize"),
#endif
		NotifyParentProperty(true), AutoFormatEnable]
		[DefaultValue(typeof(Unit), ""),
		Obsolete("This property is now obsolete. Use SettingsFileList.ThumbnailsViewSettings.ThumbnailWidth, SettingsFileList.ThumbnailsViewSettings.ThumbnailHeight, SettingsFileList.DetailsViewSettings.ThumbnailWidth, and SettingsFileList.DetailsViewSettings.ThumbnailHeight properties instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Unit ThumbnailSize {
			get { return GetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize()); }
			set { SetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize(), value); }
		}
		protected virtual Unit GetDefaultThumbnailSize() {
			return Unit.Empty;
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue(1200)]
		public int FilterDelay {
			get { return GetIntProperty("FilterDelay", GetDefaultFilterDelay()); }
			set { SetIntProperty("FilterDelay", GetDefaultFilterDelay(), value); }
		}
		internal int GetDefaultFilterDelay() {
			return 1200;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUseAppRelativePath"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool UseAppRelativePath {
			get { return GetBoolProperty("UseAppRelativePath", GetDefaultUseAppRelativePath()); }
			set { SetBoolProperty("UseAppRelativePath", GetDefaultUseAppRelativePath(), value); }
		}
		protected virtual bool GetDefaultUseAppRelativePath() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsAllowedFileExtensions"),
#endif
		AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public virtual string[] AllowedFileExtensions {
			get { return (string[])GetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions()); }
			set {
				if(!CommonUtils.AreEqualsArrays(AllowedFileExtensions, value)) {
					SetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions(), value);
					if(FileManager != null) {
						FileManager.ResetFileSystemProvider();
						FileManager.LayoutChanged();
					}
				}
			}
		}
		protected virtual string[] GetDefaultAllowedFileExtensions() {
			return new string[0];
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEnableMultiSelect"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public bool EnableMultiSelect
		{
			get { return GetBoolProperty("EnableMultiSelect", false); }
			set
			{
				if (EnableMultiSelect == value) return;
				SetBoolProperty("EnableMultiSelect", false, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettings src = source as FileManagerSettings;
			if(src != null) {
				RootFolder = src.RootFolder;
				InitialFolder = src.InitialFolder;
#pragma warning disable 618
				ThumbnailSize = src.ThumbnailSize;
#pragma warning restore 618
				FilterDelay = src.FilterDelay;
				ThumbnailFolder = src.ThumbnailFolder;
				UseAppRelativePath = src.UseAppRelativePath;
				AllowedFileExtensions = new string[src.AllowedFileExtensions.Length];
				src.AllowedFileExtensions.CopyTo(AllowedFileExtensions, 0);
				EnableMultiSelect = src.EnableMultiSelect;
			}
		}
	}
	public class FileManagerSettingsEditing : FileManagerSettingsBase {
		public FileManagerSettingsEditing(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowCreate"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowCreate {
			get { return GetBoolProperty("AllowCreate", GetDefaultAllowCreate()); }
			set {
				if(value == AllowCreate)
					return;
				SetBoolProperty("AllowCreate", GetDefaultAllowCreate(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowCreate() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowRename"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowRename {
			get { return GetBoolProperty("AllowRename", GetDefaultAllowRename()); }
			set {
				if(value == AllowRename) return;
				SetBoolProperty("AllowRename", GetDefaultAllowRename(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowRename() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowMove"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowMove {
			get { return GetBoolProperty("AllowMove", GetDefaultAllowMove()); }
			set {
				if(value == AllowMove) return;
				SetBoolProperty("AllowMove", GetDefaultAllowMove(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowMove() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowDelete"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowDelete {
			get { return GetBoolProperty("AllowDelete", GetDefaultAllowDelete()); }
			set {
				if(value == AllowDelete) return;
				SetBoolProperty("AllowDelete", GetDefaultAllowDelete(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowDelete() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowDownload"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowDownload {
			get { return GetBoolProperty("AllowDownload", GetDefaultAllowDownload()); }
			set {
				if(value == AllowDownload) return;
				SetBoolProperty("AllowDownload", GetDefaultAllowDownload(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowDownload() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingAllowCopy"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool AllowCopy
		{
			get { return GetBoolProperty("AllowCopy", GetDefaultAllowCopy()); }
			set
			{
				if (value == AllowCopy) return;
				SetBoolProperty("AllowCopy", GetDefaultAllowCopy(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultAllowCopy() {
			return false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingTemporaryFolder"),
#endif
		Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("~/FileManagerTemp")]
		public virtual string TemporaryFolder
		{
			get { return GetStringProperty("TemporaryFolder", GetDefaultTemporaryFolder()); }
			set
			{
				if (value == TemporaryFolder) return;
				SetStringProperty("TemporaryFolder", GetDefaultTemporaryFolder(), value);
				Changed();
			}
		}
		protected virtual string GetDefaultTemporaryFolder() {
			return "~/FileManagerTemp";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsEditingDownloadedArchiveName"),
#endif
		Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("files")]
		public virtual string DownloadedArchiveName
		{
			get { return GetStringProperty("DownloadedArchiveName", GetDefaultDownloadedArchiveName()); }
			set
			{
				if (value == DownloadedArchiveName) return;
				SetStringProperty("DownloadedArchiveName", GetDefaultDownloadedArchiveName(), value);
				Changed();
			}
		}
		protected virtual string GetDefaultDownloadedArchiveName() {
			return "files";
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsEditing src = source as FileManagerSettingsEditing;
			if(src != null) {
				AllowCreate = src.AllowCreate;
				AllowRename = src.AllowRename;
				AllowMove = src.AllowMove;
				AllowDelete = src.AllowDelete;
				AllowDownload = src.AllowDownload;
				AllowCopy = src.AllowCopy;
				TemporaryFolder = src.TemporaryFolder;
				DownloadedArchiveName = src.DownloadedArchiveName;
			}
		}
	}
	public class FileManagerSettingsFolders : FileManagerSettingsBase {
		public FileManagerSettingsFolders(IPropertiesOwner owner) : base(owner) { }
		ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersVisible"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool Visible
		{
			get { return GetBoolProperty("ShowFolderContainer", GetDefaultVisible()); }
			set
			{
				if (Visible != value)
				{
					SetBoolProperty("ShowFolderContainer", GetDefaultVisible(), value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersHideAspNetFolders"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool HideAspNetFolders {
			get { return GetBoolProperty("HideAspNetFolders", true); }
			set {
				if(value == HideAspNetFolders) return;
				if(FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetBoolProperty("HideAspNetFolders", true, value); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersShowFolderIcons"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowFolderIcons {
			get { return GetBoolProperty("ShowFolderIcons", GetDefaultShowFolderIcons()); }
			set {
				if(value == ShowFolderIcons) return;
				SetBoolProperty("ShowFolderIcons", GetDefaultShowFolderIcons(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowFolderIcons() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersShowLockedFolderIcons"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowLockedFolderIcons {
			get { return GetBoolProperty("ShowLockedFolderIcons", GetDefaultShowLockedFolderIcons()); }
			set {
				if(value == ShowLockedFolderIcons) return;
				SetBoolProperty("ShowLockedFolderIcons", GetDefaultShowLockedFolderIcons(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowLockedFolderIcons() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersEnableCallBacks"),
#endif
Category("Behavior"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool EnableCallBacks
		{
			get { return GetBoolProperty("EnableCallBacks", false); }
			set
			{
				SetBoolProperty("EnableCallBacks", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersShowTreeLines"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable, NotifyParentProperty(true)]
		public bool ShowTreeLines
		{
			get { return GetBoolProperty("ShowTreeLines", true); }
			set
			{
				SetBoolProperty("ShowTreeLines", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFoldersShowExpandButtons"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable, NotifyParentProperty(true)]
		public bool ShowExpandButtons
		{
			get { return GetBoolProperty("ShowExpandButtons", true); }
			set
			{
				SetBoolProperty("ShowExpandButtons", true, value);
				Changed();
			}
		}
		protected virtual bool GetDefaultVisible() {
			return true;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsFolders src = source as FileManagerSettingsFolders;
			if(src != null) {
				HideAspNetFolders = src.HideAspNetFolders;
				ShowFolderIcons = src.ShowFolderIcons;
				ShowLockedFolderIcons = src.ShowLockedFolderIcons;
				EnableCallBacks = src.EnableCallBacks;
				Visible = src.Visible;
				ShowTreeLines = src.ShowTreeLines;
				ShowExpandButtons = src.ShowExpandButtons;
			}
		}
	}
	public class FileManagerSettingsToolbar : FileManagerSettingsBase {
		FileManagerToolbarItemCollection items = null;
		public FileManagerSettingsToolbar(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowPath"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowPath {
			get { return GetBoolProperty("ShowPath", GetDefaultShowPath()); }
			set {
				if(value == ShowPath) return;
				SetBoolProperty("ShowPath", GetDefaultShowPath(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowPath() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowCreateButton"),
#endif
		Obsolete("To display the create button, add an object of the FileManagerToolbarCreateButton type to the SettingsToolbar.Items collection. If the collection is empty, the create button is displayed provided that the SettingsEditing.AllowCreate property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowCreateButton {
			get { return GetBoolProperty("ShowCreateButton", GetDefaultShowCreateButton()); }
			set {
#pragma warning disable 618
				if(value == ShowCreateButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowCreateButton", GetDefaultShowCreateButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowCreateButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowRenameButton"),
#endif
		Obsolete("To display the rename button, add an object of the FileManagerToolbarRenameButton type to the SettingsToolbar.Items collection. If the collection is empty, the rename button is displayed provided that the SettingsEditing.AllowRename property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowRenameButton {
			get { return GetBoolProperty("ShowRenameButton", GetDefaultShowRenameButton()); }
			set {
#pragma warning disable 618
				if(value == ShowRenameButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowRenameButton", GetDefaultShowRenameButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowRenameButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowMoveButton"),
#endif
		Obsolete("To display the move button, add an object of the FileManagerToolbarMoveButton type to the SettingsToolbar.Items collection. If the collection is empty, the move button is displayed provided that the SettingsEditing.AllowMove property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowMoveButton {
			get { return GetBoolProperty("ShowMoveButton", GetDefaultShowMoveButton()); }
			set {
#pragma warning disable 618
				if(value == ShowMoveButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowMoveButton", GetDefaultShowMoveButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowMoveButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowDeleteButton"),
#endif
		Obsolete("To display the delete button, add an object of the FileManagerToolbarDeleteButton type to the SettingsToolbar.Items collection. If the collection is empty, the delete button is displayed provided that the SettingsEditing.AllowDelete property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowDeleteButton {
			get { return GetBoolProperty("ShowDeleteButton", GetDefaultShowDeleteButton()); }
			set {
#pragma warning disable 618
				if(value == ShowDeleteButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowDeleteButton", GetDefaultShowDeleteButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowDeleteButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowRefreshButton"),
#endif
		Obsolete("To display the refresh button, add an object of the FileManagerToolbarRefreshButton type to the SettingsToolbar.Items collection. If the collection is empty, the refresh button is displayed."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowRefreshButton {
			get { return GetBoolProperty("ShowRefreshButton", GetDefaultShowRefreshButton()); }
			set {
#pragma warning disable 618
				if(value == ShowRefreshButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowRefreshButton", GetDefaultShowRefreshButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowRefreshButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowFilterBox"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowFilterBox {
			get { return GetBoolProperty("ShowFilterBox", GetDefaultShowFilterBox()); }
			set {
				if(value == ShowFilterBox) return;
				SetBoolProperty("ShowFilterBox", GetDefaultShowFilterBox(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowFilterBox() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowDownloadButton"),
#endif
		Obsolete("To display the download button, add an object of the FileManagerToolbarDownloadButton type to the SettingsToolbar.Items collection. If the collection is empty, the download button is displayed provided that the SettingsEditing.AllowDownload property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowDownloadButton
		{
			get { return GetBoolProperty("ShowDownloadButton", GetDefaultDownloadButton()); }
			set {
#pragma warning disable 618
				if (value == ShowDownloadButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowDownloadButton", GetDefaultDownloadButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultDownloadButton() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsToolbarShowCopyButton"),
#endif
		Obsolete("To display the copy button, add an object of the FileManagerToolbarCopyButton type to the SettingsToolbar.Items collection. If the collection is empty, the copy button is displayed provided that the SettingsEditing.AllowCopy property is set to true."),
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowCopyButton
		{
			get { return GetBoolProperty("ShowCopyButton", GetDefaultShowCopyButton()); }
			set {
#pragma warning disable 618
				if (value == ShowCopyButton) return;
#pragma warning restore 618
				SetBoolProperty("ShowCopyButton", GetDefaultShowCopyButton(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowCopyButton() {
			return true;
		}
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public bool Visible {
			get { return GetBoolProperty("ShowToolbar", true); }
			set {
				if(Visible != value) {
					SetBoolProperty("ShowToolbar", true, value);
					Changed();
				}
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerToolbarItemCollection Items {
			get {
				if(items == null)
					items = new FileManagerToolbarItemCollection(this.Owner);
				return items;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsToolbar src = source as FileManagerSettingsToolbar;
			if(src != null) {
#pragma warning disable 618
				ShowPath = src.ShowPath;
				ShowFilterBox = src.ShowFilterBox;
				ShowCreateButton = src.ShowCreateButton;
				ShowRenameButton = src.ShowRenameButton;
				ShowMoveButton = src.ShowMoveButton;
				ShowDeleteButton = src.ShowDeleteButton;
				ShowRefreshButton = src.ShowRefreshButton;
				ShowDownloadButton = src.ShowDownloadButton;
				ShowCopyButton = src.ShowCopyButton;
				Visible = src.Visible;
				Items.Assign(src.Items);
#pragma warning restore 618
			}
		}
	}
	public class FileManagerSettingsContextMenu : FileManagerSettingsBase {
		private FileManagerToolbarItemCollection items = null;
		public FileManagerSettingsContextMenu(IPropertiesOwner owner) : base(owner) { }   
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				if(value == Enabled)
					return;
				SetBoolProperty("Enabled", true, value);
				Changed();
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerToolbarItemCollection Items {
			get {
				if(items == null)
					items = new FileManagerToolbarItemCollection(this.Owner);
				return items;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsContextMenu src = source as FileManagerSettingsContextMenu;
			if(src != null) {
				Enabled = src.Enabled;
				Items.Assign(src.Items);
			}
		}
	}
	public class FileManagerSettingsBreadcrumbs : FileManagerSettingsBase {
		public FileManagerSettingsBreadcrumbs(IPropertiesOwner owner) : base(owner) { }
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool Visible {
			get { return GetBoolProperty("Visible", GetDefaultVisible()); }
			set {
				if(value == Visible)
					return;
				SetBoolProperty("Visible", GetDefaultVisible(), value);
				Changed();
			}
		}
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowParentFolderButton {
			get { return GetBoolProperty("ShowParentFolderButton", true); }
			set {
				if(value == ShowParentFolderButton)
					return;
				SetBoolProperty("ShowParentFolderButton", true, value);
				Changed();
			}
		}
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(BreadcrumbsPosition.Top)]
		public virtual BreadcrumbsPosition Position {
			get { return (BreadcrumbsPosition)GetEnumProperty("BreadCrumbsPosition", GetDefaultPosition()); }
			set {
				if(value == Position)
					return;
				SetEnumProperty("BreadCrumbsPosition", GetDefaultPosition(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultVisible() {
			return false;
		}
		protected virtual BreadcrumbsPosition GetDefaultPosition() {
			return BreadcrumbsPosition.Top;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsBreadcrumbs src = source as FileManagerSettingsBreadcrumbs;
			if(src != null) {
				Visible = src.Visible;
				ShowParentFolderButton = src.ShowParentFolderButton;
				Position = src.Position;
			}
		}
	}
	public enum FileManagerAllowedFolder {
		Any,
		SpecificOnly
	}
	public class FileManagerSettingsUpload : FileManagerSettingsBase {
		FileManagerValidationSettings validationSettings;
		FileManagerUploadAdvancedModeSettings advancedModeSettings;
		public FileManagerSettingsUpload(IPropertiesOwner owner) : base(owner) { }
		protected ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadEnabled"),
#endif
		AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool Enabled {
			get { return GetBoolProperty("Enabled", GetDefaultEnabled()); }
			set {
				if(value == Enabled)
					return;
				SetBoolProperty("Enabled", GetDefaultEnabled(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultEnabled() {
			return true;
		}
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool ShowUploadPanel {
			get { return GetBoolProperty("ShowUploadPanel", GetDefaultShowUploadPanel()); }
			set {
				if(value == ShowUploadPanel)
					return;
				SetBoolProperty("ShowUploadPanel", GetDefaultShowUploadPanel(), value);
				Changed();
			}
		}
		protected virtual bool GetDefaultShowUploadPanel() {
			return true;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadNullText"),
#endif
		Category("Settings"), DefaultValue(""), AutoFormatEnable, Localizable(true), NotifyParentProperty(true)]
		public string NullText {
			get { return GetStringProperty("NullText", ""); }
			set { SetStringProperty("NullText", "", value); }
		}
		[DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string DialogTriggerID {
			get { return GetStringProperty("DialogTriggerID", ""); }
			set { SetStringProperty("DialogTriggerID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadUseAdvancedUploadMode"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public bool UseAdvancedUploadMode
		{
			get { return GetBoolProperty("UseAdvancedUploadMode", true); }
			set {
				SetBoolProperty("UseAdvancedUploadMode", true, value);
				if(!value) {
					if(AdvancedModeSettings.EnableMultiSelect) 
						AdvancedModeSettings.EnableMultiSelect = false;
					if(AdvancedModeSettings.EnableClientAccessRuleValidation)
						AdvancedModeSettings.EnableClientAccessRuleValidation = false;
				}
			}
		}
		[Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public bool AutoStartUpload {
			get { return GetBoolProperty("AutoStartUpload", false); }
			set { SetBoolProperty("AutoStartUpload", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadAllowedFolder"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(FileManagerAllowedFolder.Any), NotifyParentProperty(true),
		Obsolete("This property is now obsolete. Use the SettingsPermissions.AccessRules property instead.")]
		public virtual FileManagerAllowedFolder AllowedFolder { get { return AllowedFolderInternal; } set { AllowedFolderInternal = value; } }
		protected internal virtual FileManagerAllowedFolder AllowedFolderInternal {
			get { return (FileManagerAllowedFolder)GetEnumProperty("AllowedFolder", GetDefaultAllowedFolder()); }
			set {
				if(AllowedFolderInternal == value) return;
				if(FileManager != null)
					FileManager.FileSystemProvider.ResetAccessModel();
				SetEnumProperty("AllowedFolder", GetDefaultAllowedFolder(), value);
				Changed();
			}
		}
		protected virtual FileManagerAllowedFolder GetDefaultAllowedFolder() {
			return FileManagerAllowedFolder.Any;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadAllowedFolderPath"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Obsolete("This property is now obsolete. Use the SettingsPermissions.AccessRules property instead.")]
		public virtual string AllowedFolderPath { get { return AllowedFolderPathInternal; } set { AllowedFolderPathInternal = value; } }
		protected internal virtual string AllowedFolderPathInternal {
			get { return GetStringProperty("AllowedFolderPath", GetDefaultAllowedFolderPath()); }
			set {
				if(AllowedFolderPathInternal == value) return;
				if(FileManager != null)
					FileManager.FileSystemProvider.ResetAccessModel();
				SetStringProperty("AllowedFolderPath", GetDefaultAllowedFolderPath(), value);
				Changed();
			}
		}
		protected virtual string GetDefaultAllowedFolderPath() {
			return string.Empty;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadValidationSettings"),
#endif
		Category("Settings"), AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public virtual FileManagerValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal; }
		}
		protected FileManagerValidationSettings ValidationSettingsInternal {
			get {
				if(this.validationSettings == null)
					this.validationSettings = CreateValidationSettings();
				return this.validationSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsUploadAdvancedModeSettings"),
#endif
		Category("Settings"), AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public virtual FileManagerUploadAdvancedModeSettings AdvancedModeSettings
		{
			get
			{
				if (this.advancedModeSettings == null)
					this.advancedModeSettings = new FileManagerUploadAdvancedModeSettings(FileManager);
				return this.advancedModeSettings;
			}
		}
		protected virtual FileManagerValidationSettings CreateValidationSettings() {
			return new FileManagerValidationSettings(Owner);
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsUpload src = source as FileManagerSettingsUpload;
			if(src != null) {
				Enabled = src.Enabled;
				ShowUploadPanel = src.ShowUploadPanel;
				AllowedFolderInternal = src.AllowedFolderInternal;
				AllowedFolderPathInternal = src.AllowedFolderPathInternal;
				NullText = src.NullText;
				DialogTriggerID = src.DialogTriggerID;
				UseAdvancedUploadMode = src.UseAdvancedUploadMode;
				AutoStartUpload = src.AutoStartUpload;
				ValidationSettingsInternal.Assign(src.ValidationSettingsInternal);
				AdvancedModeSettings.Assign(src.AdvancedModeSettings);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ValidationSettingsInternal, AdvancedModeSettings });
		}
	}
	public class FileManagerValidationSettings : UploadControlValidationSettings {
		public FileManagerValidationSettings(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string[] AllowedFileExtensions {
			get {
				ASPxFileManager fm = Owner as ASPxFileManager;
				if(fm != null)
					return fm.Settings.AllowedFileExtensions;
				return new string[0];
			}
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowErrors {
			get { return false; }
			set { }
		}
	}
	public class FileManagerUploadAdvancedModeSettings : UploadAdvancedModeSettings {
		public FileManagerUploadAdvancedModeSettings() :base() { }
		public FileManagerUploadAdvancedModeSettings(IPropertiesOwner owner) : base(owner) { }
		ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerUploadAdvancedModeSettingsEnableMultiSelect")]
#endif
		public override bool EnableMultiSelect {
			get { return base.EnableMultiSelect; }
			set {
				if(base.EnableMultiSelect == value)
					return;
				base.EnableMultiSelect = value;
				if(value)
					ForceUseAdvancedUploadMode();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerUploadAdvancedModeSettingsEnableClientAccessRuleValidation"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		RefreshProperties(RefreshProperties.Repaint)]
		public virtual bool EnableClientAccessRuleValidation
		{
			get { return GetBoolProperty("EnableClientAccessRuleValidation", false); }
			set
			{
				SetBoolProperty("EnableClientAccessRuleValidation", false, value);
				if (value)
					ForceUseAdvancedUploadMode();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerUploadAdvancedModeSettingsEnableDragAndDrop"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public override bool EnableDragAndDrop 
		{
			get { return base.EnableDragAndDrop; }
			set {
				if(base.EnableDragAndDrop == value)
					return;
				base.EnableDragAndDrop = value;
				if(value) {
					ForceUseAdvancedUploadMode();
				}
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerUploadAdvancedModeSettings src = source as FileManagerUploadAdvancedModeSettings;
			if(src != null)
				EnableClientAccessRuleValidation = src.EnableClientAccessRuleValidation;
		}
		protected override bool GetEnableDragAndDropDefaultValue() {
			return true;
		}
		void ForceUseAdvancedUploadMode() {
			if(FileManager != null && !FileManager.SettingsUpload.UseAdvancedUploadMode)
				FileManager.SettingsUpload.UseAdvancedUploadMode = true;
		}
	}
	public class FileManagerSettingsDataSource : FileManagerSettingsBase {
		public FileManagerSettingsDataSource() : this(null) { }
		public FileManagerSettingsDataSource(IPropertiesOwner owner) : base(owner) { }
		ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceKeyFieldName"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string KeyFieldName
		{
			get { return GetStringProperty("KeyFieldName", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("KeyFieldName", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceParentKeyFieldName"),
#endif
		DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string ParentKeyFieldName
		{
			get { return GetStringProperty("ParentKeyFieldName", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("ParentKeyFieldName", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceIsFolderFieldName"),
#endif
DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string IsFolderFieldName
		{
			get { return GetStringProperty("IsFolderFieldName", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("IsFolderFieldName", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceNameFieldName"),
#endif
DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string NameFieldName
		{
			get { return GetStringProperty("NameFieldName", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("NameFieldName", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceFileBinaryContentFieldName"),
#endif
DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string FileBinaryContentFieldName
		{
			get { return GetStringProperty("FileBinaryContent", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("FileBinaryContent", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsDataSourceLastWriteTimeFieldName"),
#endif
DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string LastWriteTimeFieldName
		{
			get { return GetStringProperty("LastWriteTimeFieldName", ""); }
			set
			{
				if (FileManager != null)
					FileManager.ResetFileSystemProvider();
				SetStringProperty("LastWriteTimeFieldName", "", value);
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsDataSource src = source as FileManagerSettingsDataSource;
			if(src != null) {
				KeyFieldName = src.KeyFieldName;
				ParentKeyFieldName = src.ParentKeyFieldName;
				IsFolderFieldName = src.IsFolderFieldName;
				NameFieldName = src.NameFieldName;
				FileBinaryContentFieldName = src.FileBinaryContentFieldName;
				LastWriteTimeFieldName = src.LastWriteTimeFieldName;
			}
		}
	}
	public class FileManagerSettingsPermissions : FileManagerSettingsBase {
		AccessRulesCollection accessRules;
		public FileManagerSettingsPermissions(IPropertiesOwner owner) 
			: base(owner) {
			this.accessRules = new AccessRulesCollection(FileManager);
		}
		protected ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsPermissionsAccessRules"),
#endif
		Category("Security"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)), 
		NotifyParentProperty(true)]
		public AccessRulesCollection AccessRules
		{
			get { return accessRules; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsPermissionsRole"),
#endif
		Category("Security"), Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string Role
		{
			get { return GetStringProperty("Role", string.Empty); }
			set
			{
				if (FileManager != null && value != Role)
				{
					FileManager.FileSystemProvider.ResetAccessModel();
					Changed();
				}
				SetStringProperty("Role", string.Empty, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsPermissions src = source as FileManagerSettingsPermissions;
			if(src != null) {
				Role = src.Role;
				AccessRules.Assign(src.AccessRules);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				AccessRules
			});
		}
	}
	public enum FileListView {
		Thumbnails = 0,
		Details = 1,
	}
	public class FileManagerFileListThumbnailsViewSettings : FileManagerSettingsBase {
		ITemplate itemTemplate;
		public FileManagerFileListThumbnailsViewSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListThumbnailsViewSettingsThumbnailSize"),
#endif
		Obsolete("This property is now obsolete. Use ThumbnailWidth and ThumbnailHeight properties instead."),
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailSize
		{
			get { return GetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize()); }
			set
			{
				SetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		[NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailWidth {
			get { return GetUnitProperty("ThumbnailWidth", GetDefaultThumbnailSize()); }
			set {
				SetUnitProperty("ThumbnailWidth", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		[NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailHeight {
			get { return GetUnitProperty("ThumbnailHeight", GetDefaultThumbnailSize()); }
			set {
				SetUnitProperty("ThumbnailHeight", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(FileManagerThumbnailsViewItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return itemTemplate; }
			set {
				if(itemTemplate == value)
					return;
				itemTemplate = value;
				Changed();
			}
		}
		protected virtual Unit GetDefaultThumbnailSize() {
			return Unit.Empty;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerFileListThumbnailsViewSettings src = source as FileManagerFileListThumbnailsViewSettings;
			if(src != null) {
#pragma warning disable 618
				ThumbnailSize = src.ThumbnailSize;
#pragma warning restore 618
				ThumbnailWidth = src.ThumbnailWidth;
				ThumbnailHeight = src.ThumbnailHeight;
				ItemTemplate = src.ItemTemplate;
			}
		}
	}
	public class FileManagerFileListDetailsViewSettings : FileManagerSettingsBase {
		FileManagerDetailsColumnCollection columns;
		public FileManagerFileListDetailsViewSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsThumbnailSize"),
#endif
		Obsolete("This method is now obsolete. Use ThumbnailWidth and ThumbnailHeight properties instead."),
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailSize
		{
			get { return GetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize()); }
			set
			{
				SetUnitProperty("ThumbnailSize", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		[NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailWidth {
			get { return GetUnitProperty("ThumbnailWidth", GetDefaultThumbnailSize()); }
			set {
				SetUnitProperty("ThumbnailWidth", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		[NotifyParentProperty(true), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public virtual Unit ThumbnailHeight {
			get { return GetUnitProperty("ThumbnailHeight", GetDefaultThumbnailSize()); }
			set {
				SetUnitProperty("ThumbnailHeight", GetDefaultThumbnailSize(), value);
				Changed();
			}
		}
		protected virtual Unit GetDefaultThumbnailSize() {
			return Unit.Empty;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsShowHeaderFilterButton"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowHeaderFilterButton
		{
			get { return GetBoolProperty("ShowHeaderFilterButton", false); }
			set
			{
				SetBoolProperty("ShowHeaderFilterButton", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsAllowColumnSort"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowColumnSort
		{
			get { return GetBoolProperty("AllowColumnSort", true); }
			set
			{
				SetBoolProperty("AllowColumnSort", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsAllowColumnDragDrop"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowColumnDragDrop
		{
			get { return GetBoolProperty("AllowColumnDragDrop", true); }
			set
			{
				SetBoolProperty("AllowColumnDragDrop", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsAllowColumnResize"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowColumnResize
		{
			get { return GetBoolProperty("AllowColumnResize", false); }
			set
			{
				SetBoolProperty("AllowColumnResize", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileListDetailsViewSettingsColumns"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), Themeable(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerDetailsColumnCollection Columns
		{
			get
			{
				if (columns == null)
					columns = CreateColumnCollection();
				return columns;
			}
		}
		protected virtual FileManagerDetailsColumnCollection CreateColumnCollection() {
			return new FileManagerDetailsColumnCollection();
		}
		internal IEnumerable<FileManagerDetailsColumn> ColumnsInternal {
			get {
				if(Columns.Count == 0)
					return Columns.GetDefaultColumns();
				return Columns;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerFileListDetailsViewSettings src = source as FileManagerFileListDetailsViewSettings;
			if(src != null) {
				ShowHeaderFilterButton = src.ShowHeaderFilterButton;
				AllowColumnSort = src.AllowColumnSort;
				AllowColumnDragDrop = src.AllowColumnDragDrop;
				AllowColumnResize = src.AllowColumnResize;
#pragma warning disable 618
				ThumbnailSize = src.ThumbnailSize;
#pragma warning restore 618
				ThumbnailWidth = src.ThumbnailWidth;
				ThumbnailHeight = src.ThumbnailHeight;
				Columns.Assign(src.Columns);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				Columns
			});
		}
	}
	public class FileManagerSettingsFileList : FileManagerSettingsBase {
		FileManagerFileListDetailsViewSettings detailsViewSettings;
		FileManagerFileListThumbnailsViewSettings thumbnailsViewSettings;
		public FileManagerSettingsFileList(IPropertiesOwner owner) : base(owner) { }
		protected ASPxFileManager FileManager { get { return Owner as ASPxFileManager; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFileListView"),
#endif
		DefaultValue(FileListView.Thumbnails), NotifyParentProperty(true), AutoFormatDisable]
		public FileListView View
		{
			get { return (FileListView)GetEnumProperty("View", FileListView.Thumbnails); }
			set
			{
				if (View == value)
					return;
				SetEnumProperty("View", FileListView.Thumbnails, value);
				if (FileManager != null)
					FileManager.ResetFileList(false);
				Changed();
			}
		}
		[DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool ShowFolders {
			get { return GetBoolProperty("ShowFolders", GetDefaultShowFolders()); }
			set {
				SetBoolProperty("ShowFolders", GetDefaultShowFolders(), value);
				Changed();
			}
		}
		[DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool ShowParentFolder {
			get { return GetBoolProperty("ShowParenFolder", GetDefaultShowParentFolder()); }
			set {
				SetBoolProperty("ShowParenFolder", GetDefaultShowParentFolder(), value);
				Changed();
			}
		}
		[AutoFormatDisable, NotifyParentProperty(true), DefaultValue(300)]
		public int PageSize {
			get { return GetIntProperty("PageSize", 300); }
			set {
				if(PageSize == value)
					return;
				SetIntProperty("PageSize", 300, value);
				if(FileManager != null)
					FileManager.ResetFileList(false);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFileListDetailsViewSettings"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Themeable(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerFileListDetailsViewSettings DetailsViewSettings
		{
			get
			{
				if (detailsViewSettings == null)
					detailsViewSettings = CreateDetailsViewSettings();
				return detailsViewSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerSettingsFileListThumbnailsViewSettings"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Themeable(false), NotifyParentProperty(true), AutoFormatDisable]
		public FileManagerFileListThumbnailsViewSettings ThumbnailsViewSettings
		{
			get
			{
				if(thumbnailsViewSettings == null)
					thumbnailsViewSettings = CreateThumbnailsViewSettings();
				return thumbnailsViewSettings;
			}
		}
		protected virtual FileManagerFileListDetailsViewSettings CreateDetailsViewSettings() {
			return new FileManagerFileListDetailsViewSettings(this);
		}
		protected virtual FileManagerFileListThumbnailsViewSettings CreateThumbnailsViewSettings() {
			return new FileManagerFileListThumbnailsViewSettings(this);
		}
		protected virtual bool GetDefaultShowParentFolder() {
			return false;
		}
		protected virtual bool GetDefaultShowFolders() {
			return false;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerSettingsFileList src = source as FileManagerSettingsFileList;
			if(src != null) {
				View = src.View;
				ShowFolders = src.ShowFolders;
				ShowParentFolder = src.ShowParentFolder;
				PageSize = src.PageSize;
				DetailsViewSettings.Assign(src.DetailsViewSettings);
				ThumbnailsViewSettings.Assign(src.ThumbnailsViewSettings);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { DetailsViewSettings, ThumbnailsViewSettings };
		}
	}
	public class FileManagerAmazonProviderSettings : FileManagerSettingsBase {
		public FileManagerAmazonProviderSettings(IPropertiesOwner owner) : base(owner) {
			AccessKeyID = string.Empty;
			SecretAccessKey = string.Empty;
			BucketName = string.Empty;
			Region = string.Empty;
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string AccessKeyID { get; set; }
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string SecretAccessKey { get; set; }
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string BucketName { get; set; }
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string Region { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerAmazonProviderSettings src = source as FileManagerAmazonProviderSettings;
			if(src != null) {
				AccessKeyID = src.AccessKeyID;
				SecretAccessKey = src.SecretAccessKey;
				BucketName = src.BucketName;
				Region = src.Region;
			}
		}
	}
	public class FileManagerAzureProviderSettings : FileManagerSettingsBase {
		public FileManagerAzureProviderSettings(IPropertiesOwner owner) : base(owner) {
			StorageAccountName = string.Empty;
			AccessKey = string.Empty;
			ContainerName = string.Empty;
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string StorageAccountName { get; set; }
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string AccessKey { get; set; }
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string ContainerName { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerAzureProviderSettings src = source as FileManagerAzureProviderSettings;
			if(src != null) {
				StorageAccountName = src.StorageAccountName;
				AccessKey = src.AccessKey;
				ContainerName = src.ContainerName;
			}
		}
	}
	public class FileManagerDropBoxProviderSettings : FileManagerSettingsBase {
		public FileManagerDropBoxProviderSettings(IPropertiesOwner owner) : base(owner) {
			AccessTokenValue = string.Empty;
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue("")]
		public string AccessTokenValue { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FileManagerDropBoxProviderSettings src = source as FileManagerDropBoxProviderSettings;
			if(src != null)
				AccessTokenValue = src.AccessTokenValue;
		}
	}
	public enum FileInfoType {
		FileName = 0,
		LastWriteTime = 1,
		Size = 2,
		Thumbnail = 3
	}
	public class FileManagerDetailsColumn : CollectionItem {
		GridViewCellStyle cellStyle;
		GridViewHeaderStyle headerStyle;
		ITemplate itemTemplate;
		bool isCaptionLocked = false;
		public FileManagerDetailsColumn() 
			: this(FileInfoType.FileName) { }
		public FileManagerDetailsColumn(FileInfoType fileInfoType)
			: this(fileInfoType, string.Empty) { }
		public FileManagerDetailsColumn(FileInfoType fileInfoType, string caption) {
			Caption = caption;
			FileInfoType = fileInfoType;
			cellStyle = new GridViewCellStyle();
			headerStyle = new GridViewHeaderStyle();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnFileInfoType"),
#endif
		DefaultValue(FileInfoType.FileName), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, RefreshProperties(RefreshProperties.All)]
		public virtual FileInfoType FileInfoType
		{
			get { return (FileInfoType)GetEnumProperty("FileInfoType", FileInfoType.FileName); }
			set
			{
				SetEnumProperty("FileInfoType", FileInfoType.FileName, value);
				if (!isCaptionLocked)
					SetStringProperty("Caption", string.Empty, FileManagerDataHelper.GetFileInfoTypeCaption(value));
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnCaption"),
#endif
		Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable, RefreshProperties(RefreshProperties.All)]
		public string Caption
		{
			get { return GetStringProperty("Caption", string.Empty); }
			set
			{
				isCaptionLocked = value != "";
				if (Caption == value) return;
				SetStringProperty("Caption", string.Empty, (value == null) ? string.Empty : value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnWidth"),
#endif
		Category("Appearance"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit Width
		{
			get { return GetUnitProperty("Width", Unit.Empty); }
			set
			{
				if (value == Width) return;
				SetUnitProperty("Width", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnAllowDragDrop"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowDragDrop
		{
			get { return (DefaultBoolean)GetEnumProperty("AllowDragDrop", DefaultBoolean.Default); }
			set
			{
				if (AllowDragDrop == value) return;
				SetEnumProperty("AllowDragDrop", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnAllowSort"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowSort
		{
			get { return (DefaultBoolean)GetEnumProperty("AllowSort", DefaultBoolean.Default); }
			set
			{
				if (AllowSort == value) return;
				SetEnumProperty("AllowSort", DefaultBoolean.Default, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnSortOrder"),
#endif
		Category("Data"), DefaultValue(ColumnSortOrder.None), NotifyParentProperty(true)]
		public virtual ColumnSortOrder SortOrder
		{
			get { return (ColumnSortOrder)GetEnumProperty("SortOrder", ColumnSortOrder.None); }
			set
			{
				if (SortOrder == value) return;
				SetEnumProperty("SortOrder", ColumnSortOrder.None, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnVisibleIndex"),
#endif
		Category("Appearance"), DefaultValue(-1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int VisibleIndex
		{
			get { return GetIntProperty("VisibleIndex", -1); }
			set
			{
				if (VisibleIndex == value) return;
				SetIntProperty("VisibleIndex", -1, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnFixedStyle"),
#endif
		Category("Behavior"), DefaultValue(GridViewColumnFixedStyle.None), NotifyParentProperty(true)]
		public GridViewColumnFixedStyle FixedStyle
		{
			get { return (GridViewColumnFixedStyle)GetEnumProperty("FixedStyle", GridViewColumnFixedStyle.None); }
			set
			{
				if (value == FixedStyle) return;
				SetEnumProperty("FixedStyle", GridViewColumnFixedStyle.None, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnCellStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DevExpress.Web.GridViewCellStyle CellStyle { get { return cellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsColumnHeaderStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DevExpress.Web.GridViewHeaderStyle HeaderStyle { get { return headerStyle; } }
		internal string CaptionInternal {
			get { return string.IsNullOrEmpty(Caption) ? FileInfoType.ToString() : Caption; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(FileManagerDetailsViewItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ItemTemplate {
			get { return itemTemplate; }
			set {
				if(ItemTemplate == value)
					return;
				itemTemplate = value;
				TemplatesChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerDetailsColumn src = source as FileManagerDetailsColumn;
			if(src != null) {
				FileInfoType = src.FileInfoType;
				Caption = src.Caption;
				Width = src.Width;
				AllowDragDrop = src.AllowDragDrop;
				AllowSort = src.AllowSort;
				SortOrder = src.SortOrder;
				VisibleIndex = src.VisibleIndex;
				CellStyle.Assign(src.CellStyle);
				HeaderStyle.Assign(src.HeaderStyle);
				FixedStyle = src.FixedStyle;
				ItemTemplate = src.ItemTemplate;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { HeaderStyle, CellStyle };
		}
		public override string ToString() {
			return FileInfoType.ToString() + " Column";
		}
	}
	public class FileManagerDetailsCustomColumn : FileManagerDetailsColumn {		
		[DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string Name {
			get { return GetStringProperty("Name", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(Name == value) return;
				SetStringProperty("Name", string.Empty, value);
			}
		}
		[DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowHeaderFilterButton {
			get { return (DefaultBoolean)GetIntProperty("ShowHeaderFilter", (int)DefaultBoolean.Default); }
			set {
				if(ShowHeaderFilterButton == value)
					return;
				SetIntProperty("ShowHeaderFilter", (int)DefaultBoolean.Default, (int)value);
				LayoutChanged();
			}
		}
		[Browsable(false)]
		public override FileInfoType FileInfoType { get; set; }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			FileManagerDetailsCustomColumn src = source as FileManagerDetailsCustomColumn;
			if(src != null) {
				Name = src.Name;
				ShowHeaderFilterButton = src.ShowHeaderFilterButton;
			}
		}
		public override string ToString() {
			return  "Custom Column";
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))] 
	public class FileManagerDetailsColumnCollection : Collection<FileManagerDetailsColumn> {
		public FileManagerDetailsColumnCollection()
			: base() {
		}
		public FileManagerDetailsColumnCollection(IWebControlObject owner)
			: base(owner) {
		}
		public FileManagerDetailsColumn Add(FileInfoType fileInfoType, string caption) {
			FileManagerDetailsColumn column = CreateColumn(fileInfoType, caption);
			Add(column);
			return column;
		}
		public void CreateDefaultColumns() {
			AddRange(GetDefaultColumns());
		}
		protected virtual FileManagerDetailsColumn CreateColumn(FileInfoType fileInfoType, string caption) {
			return new FileManagerDetailsColumn(fileInfoType, caption);
		}
		protected virtual FileManagerDetailsColumn CreateColumn(FileInfoType fileInfoType) {
			return new FileManagerDetailsColumn(fileInfoType);
		}
		protected FileManagerDetailsColumn CreateColumn(FileInfoType fileInfoType, int visibleIndex) {
			FileManagerDetailsColumn column = CreateColumn(fileInfoType);
			column.VisibleIndex = visibleIndex;
			return column;
		}
		protected internal List<FileManagerDetailsColumn> GetDefaultColumns() {
			return new List<FileManagerDetailsColumn>() {
				CreateColumn(FileInfoType.Thumbnail, 0),
				CreateColumn(FileInfoType.FileName, 1),
				CreateColumn(FileInfoType.LastWriteTime, 2),
				CreateColumn(FileInfoType.Size, 3)
			};
		}
	}
	public enum FileManagerProviderType {
		NotSet = 0,
		Custom = 1,
		DataSource = 2,
		Physical = 3,
		Dropbox = 4,
		Azure = 5,
		Amazon = 6
	}
	public enum BreadcrumbsPosition {
		Top = 0,
		Bottom = 1
	}
}
