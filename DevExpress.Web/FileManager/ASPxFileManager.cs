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
using System.Text;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web;
using DevExpress.Web.Design;
using System.Collections;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxFileManager"),
	Designer("DevExpress.Web.Design.ASPxFileManagerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxFileManager.bmp"),
	ToolboxData(@"<{0}:ASPxFileManager runat=""server""><Settings ThumbnailFolder=""~\Thumb\"" RootFolder=""~\""></Settings></{0}:ASPxFileManager>")]
	public class ASPxFileManager : ASPxDataWebControl, IRequiresLoadPostDataControl, IParentSkinOwner, IControlDesigner {
		protected internal const string
			ScriptName = WebScriptsResourcePath + "FileManager.js",
			UploadControlScriptName = WebScriptsResourcePath + "Upload.js";
		static readonly object FolderCreatingEventKey = new object();
		static readonly object FolderCreatedEventKey = new object();
		static readonly object ItemRenamingEventKey = new object();
		static readonly object ItemRenamedEventKey = new object();
		static readonly object ItemDeletingEventKey = new object();
		static readonly object ItemsDeletedEventKey = new object();
		static readonly object ItemMovingEventKey = new object();
		static readonly object ItemsMovedEventKey = new object();
		static readonly object ItemCopyingEventKey = new object();
		static readonly object ItemsCopiedEventKey = new object();
		static readonly object FileUploadingEventKey = new object();
		static readonly object FilesUploadedEventKey = new object();
		static readonly object CustomThumbnailEventKey = new object();
		static readonly object CustomErrorTextEventKey = new object();
		static readonly object FileDownloadingEventKey = new object();
		static readonly object SelectedFileOpenedEventKey = new object();
		static readonly object CustomFileInfoDisplayTextKey = new object();
		static readonly object DetailsViewCustomColumnDisplayTextEventKey = new object();
		static readonly object DetailsViewCustomColumnHeaderFilterFillItemsEventKey = new object();
		static readonly object CustomCallbackEventKey = new object();
		static readonly object CloudProviderRequestEventKey = new object();
		FileManagerHelper helper;
		FileManagerCommand currentCommand;
		FileManagerControl control;
		Image noThumbnailImage;
		Image folderDefaultThumbnailImage;
		Image thumbnailCheckBoxImage;
		Image breadCrumbsUpButtonImage;
		Image breadCrumbsUpButtonDisabledImage;
		Image breadCrumbsSeparatorImage;
		EditorImages editorImages;
		FileManagerSettings settings;
		FileManagerSettingsFileList settingsFileList;
		FileManagerSettingsEditing settingsEditing;
		FileManagerSettingsFolders settingsFolders;
		FileManagerSettingsToolbar settingsToolbar;
		FileManagerSettingsContextMenu settingsContextMenu;
		FileManagerSettingsBreadcrumbs settingsBreadcrumbs;
		FileManagerSettingsUpload settingsUpload;
		FileManagerAmazonProviderSettings amazonSettings;
		FileManagerAzureProviderSettings azureSettings;
		FileManagerDropBoxProviderSettings dropboxSettings;
		FileManagerSettingsDataSource settingsDataSource;
		FileManagerSettingsPermissions settingsPermissions;
		RestrictedAccessFileSystemProvider fileSystemProvider;
		FileSystemProviderBase customFileSystemProvider;
		FileManagerContextMenuStyles stylesContextMenu;
		FileManagerDetailsViewStyles stylesDetailsView;
		FileManagerDetailsViewImages imagesDetailsView;
		FileManagerFolder selectedFolder;
		public ASPxFileManager()
			: this(null) {
		}
		protected ASPxFileManager(ASPxWebControl owner)
			: base(owner) {
			this.helper = new FileManagerHelper(this);
			this.settings = CreateSettings();
			this.settingsFileList = new FileManagerSettingsFileList(this);
			this.settingsEditing = new FileManagerSettingsEditing(this);
			this.settingsFolders = new FileManagerSettingsFolders(this);
			this.settingsToolbar = new FileManagerSettingsToolbar(this);
			this.settingsBreadcrumbs = new FileManagerSettingsBreadcrumbs(this);
			this.settingsContextMenu = new FileManagerSettingsContextMenu(this);
			this.settingsUpload = CreateSettingsUpload();
			this.settingsDataSource = new FileManagerSettingsDataSource(this);
			this.amazonSettings = new FileManagerAmazonProviderSettings(this);
			this.azureSettings = new FileManagerAzureProviderSettings(this);
			this.dropboxSettings = new FileManagerDropBoxProviderSettings(this);
			this.settingsPermissions = new FileManagerSettingsPermissions(this);
			this.stylesContextMenu = new FileManagerContextMenuStyles(this);
			this.stylesDetailsView = new FileManagerDetailsViewStyles(this);
			this.imagesDetailsView = new FileManagerDetailsViewImages(this);
			EnableCallBacksInternal = true;
		}
		protected new internal string GetClientInstanceName() {
			return base.GetClientInstanceName();
		}
		protected internal FileManagerHelper Helper { get { return helper; } }
		protected internal FileManagerCommand CurrentCommand { get { return currentCommand; } set { currentCommand = value; } }
		protected internal FileManagerControl Control { get { return control; } }
		protected internal Image NoThumbnailImage { get { return noThumbnailImage; } }
		protected internal Image FolderDefaultThumbnailImage { get { return folderDefaultThumbnailImage; } }
		protected internal Image ThumbnailCheckBoxImage { get { return thumbnailCheckBoxImage; } }
		protected internal Image BreadCrumbsUpButtonImage { get { return breadCrumbsUpButtonImage; } }
		protected internal Image BreadCrumbsUpButtonDisabledImage { get { return breadCrumbsUpButtonDisabledImage; } }
		protected internal Image BreadCrumbsSeparatorImage { get { return breadCrumbsSeparatorImage; } }
		protected internal FileManagerFolders Folders { get { return Control != null ? Control.Container.Folders : null; } }
		protected internal FileManagerGridView FilesGridView { get { return Control != null ? Control.Container.FilesGridView : null; } }
		protected internal FileManagerItems Items { get { return Control != null ? Control.Container.ItemsControl : null; } }
		protected internal FileManagerFolders FolderBrowserFolders { get { return Control.FolderBrowserPopup.FolderBrowserFolders; } }
		protected internal EditorImages EditorImages {
			get {
				if(this.editorImages == null)
					this.editorImages = new EditorImages(this);
				return this.editorImages;
			}
		}
		protected internal RestrictedAccessFileSystemProvider FileSystemProvider { 
			get {
				if(fileSystemProvider == null)
					CreateFileSystemProvider();
				return fileSystemProvider;
			}
			set { fileSystemProvider = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerCustomFileSystemProviderTypeName"),
#endif
		Category("Data"), TypeConverter("DevExpress.Web.Design.FileManagerFileSystemProviderTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string CustomFileSystemProviderTypeName
		{
			get { return GetStringProperty("CustomFileSystemProviderTypeName", string.Empty); }
			set
			{
				ResetFileSystemProvider();
				SetStringProperty("CustomFileSystemProviderTypeName", string.Empty, value);
				LayoutChanged();
			}
		}
		[Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public FileSystemProviderBase CustomFileSystemProvider {
			get { return customFileSystemProvider; }
			set {
				ResetFileSystemProvider();
				customFileSystemProvider = value;
				LayoutChanged();
			}
		}
		protected internal string ThumbnailsFolderPath {
			get {
				return !string.IsNullOrEmpty(Settings.ThumbnailFolder)
					? MapPath(Settings.ThumbnailFolder)
					: Settings.ThumbnailFolder;
			}
		}
		public string GetAppRelativeRootFolder() {
			return FileSystemProvider.GetRelativeFolderPath(new FileManagerFolder(FileSystemProvider, string.Empty), this);
		}
		protected internal virtual string GetRootFolderRelativePath(string rootFolderPath) {
			return GetAppRelativeRootFolder();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerStyles Styles {
			get { return (FileManagerStyles)StylesInternal; }
		}
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerContextMenuStyles StylesContextMenu {
			get { return stylesContextMenu; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerStylesDetailsView"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerDetailsViewStyles StylesDetailsView { get { return stylesDetailsView; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),		
		MergableProperty(false), AutoFormatDisable]
		public FileManagerClientSideEvents ClientSideEvents {
			get { return (FileManagerClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerEnableCallBacks"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true)]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set {
				EnableCallBacksInternal = value;
				base.AutoPostBackInternal = !EnableCallBacks;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettings"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettings Settings {
			get { return settings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsFileList"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsFileList SettingsFileList
		{
			get { return settingsFileList; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsEditing"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsEditing SettingsEditing {
			get { return settingsEditing; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsFolders"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsFolders SettingsFolders {
			get { return settingsFolders; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsToolbar"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsToolbar SettingsToolbar {
			get { return settingsToolbar; }
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsContextMenu SettingsContextMenu {
			get { return settingsContextMenu; }
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsBreadcrumbs SettingsBreadcrumbs {
			get { return settingsBreadcrumbs; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsUpload"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsUpload SettingsUpload {
			get { return settingsUpload; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[DefaultValue(FileManagerProviderType.NotSet), Category("Settings"), AutoFormatDisable]
		public FileManagerProviderType ProviderType {
			get { return (FileManagerProviderType)GetEnumProperty("ProviderType", FileManagerProviderType.NotSet); }
			set {
				if(ProviderType == value)
					return;
				SetEnumProperty("ProviderType", FileManagerProviderType.NotSet, value);
				LayoutChanged();
			}
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerAmazonProviderSettings SettingsAmazon {
			get {
				if(amazonSettings == null)
					amazonSettings = new FileManagerAmazonProviderSettings(this);
				return amazonSettings;
			}
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerAzureProviderSettings SettingsAzure {
			get {
				if(azureSettings == null)
					azureSettings = new FileManagerAzureProviderSettings(this);
				return azureSettings;
			}
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerDropBoxProviderSettings SettingsDropbox {
			get {
				if(dropboxSettings == null)
					dropboxSettings = new FileManagerDropBoxProviderSettings(this);
				return dropboxSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerImages Images {
			get { return (FileManagerImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerImagesDetailsView"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerDetailsViewImages ImagesDetailsView
		{
			get { return imagesDetailsView; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsDataSource"),
#endif
Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsDataSource SettingsDataSource
		{
			get { return settingsDataSource; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSettingsPermissions"),
#endif
Category("Security"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsPermissions SettingsPermissions
		{
			get { return settingsPermissions; }
		}		
		[Browsable(false)]
		public override object DataSource {
			get { return base.DataSource; }
			set {
				if(base.DataSource != null && base.DataSource != value)
					ResetFileSystemProvider(false);
				base.DataSource = value;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxFileManagerDataSourceID")]
#endif
public override string DataSourceID {
			get { return base.DataSourceID; }
			set {
				ResetFileSystemProvider(false);
				base.DataSourceID = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft
		{
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler FolderCreating
		{
			add { Events.AddHandler(FolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(FolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerFolderCreated"),
#endif
		Category("Action")]
		public event FileManagerFolderCreatedEventHandler FolderCreated {
			add { Events.AddHandler(FolderCreatedEventKey, value); }
			remove { Events.RemoveHandler(FolderCreatedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler ItemRenaming
		{
			add { Events.AddHandler(ItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(ItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemRenamed"),
#endif
		Category("Action")]
		public event FileManagerItemRenamedEventHandler ItemRenamed {
			add { Events.AddHandler(ItemRenamedEventKey, value); }
			remove { Events.RemoveHandler(ItemRenamedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler ItemDeleting
		{
			add { Events.AddHandler(ItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(ItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemsDeleted"),
#endif
		Category("Action")]
		public event FileManagerItemsDeletedEventHandler ItemsDeleted {
			add { Events.AddHandler(ItemsDeletedEventKey, value); }
			remove { Events.RemoveHandler(ItemsDeletedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler ItemMoving
		{
			add { Events.AddHandler(ItemMovingEventKey, value); }
			remove { Events.RemoveHandler(ItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemsMoved"),
#endif
		Category("Action")]
		public event FileManagerItemsMovedEventHandler ItemsMoved {
			add { Events.AddHandler(ItemsMovedEventKey, value); }
			remove { Events.RemoveHandler(ItemsMovedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler ItemCopying
		{
			add { Events.AddHandler(ItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(ItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerItemsCopied"),
#endif
		Category("Action")]
		public event FileManagerItemsCopiedEventHandler ItemsCopied {
			add { Events.AddHandler(ItemsCopiedEventKey, value); }
			remove { Events.RemoveHandler(ItemsCopiedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler FileUploading
		{
			add { Events.AddHandler(FileUploadingEventKey, value); }
			remove { Events.RemoveHandler(FileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerFilesUploaded"),
#endif
		Category("Action")]
		public event FileManagerFilesUploadedEventHandler FilesUploaded {
			add { Events.AddHandler(FilesUploadedEventKey, value); }
			remove { Events.RemoveHandler(FilesUploadedEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerFileDownloading"),
#endif
		Category("Action")]
		public event FileManagerFileDownloadingEventHandler FileDownloading
		{
			add { Events.AddHandler(FileDownloadingEventKey, value); }
			remove { Events.RemoveHandler(FileDownloadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerSelectedFileOpened"),
#endif
		Category("Action")]
		public event FileManagerFileOpenedEventHandler SelectedFileOpened
		{
			add { Events.AddHandler(SelectedFileOpenedEventKey, value); }
			remove { Events.RemoveHandler(SelectedFileOpenedEventKey, value); }
		}
		[Category("Action")]
		public event CallbackEventHandlerBase CustomCallback {
			add { Events.AddHandler(CustomCallbackEventKey, value); }
			remove { Events.RemoveHandler(CustomCallbackEventKey, value); }
		}
		[Category("Action")]
		public event FileManagerCloudProviderRequestEventHandler CloudProviderRequest {
			add { Events.AddHandler(CloudProviderRequestEventKey, value); }
			remove { Events.RemoveHandler(CloudProviderRequestEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerCustomThumbnail"),
#endif
		Category("Appearance")]
		public event FileManagerThumbnailCreateEventHandler CustomThumbnail
		{
			add { Events.AddHandler(CustomThumbnailEventKey, value); }
			remove { Events.RemoveHandler(CustomThumbnailEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerCustomErrorText"),
#endif
		Category("Appearance")]
		public event FileManagerCustomErrorTextEventHandler CustomErrorText
		{
			add { Events.AddHandler(CustomErrorTextEventKey, value); }
			remove { Events.RemoveHandler(CustomErrorTextEventKey, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFileManagerCustomFileInfoDisplayText"),
#endif
		Category("Appearance")]
		public event FileManagerCustomFileInfoDisplayTextEventHandler CustomFileInfoDisplayText
		{
			add { Events.AddHandler(CustomFileInfoDisplayTextKey, value); }
			remove { Events.RemoveHandler(CustomFileInfoDisplayTextKey, value); }
		}
		[Category("Appearance")]
		public event FileManagerDetailsViewCustomColumnDisplayTextEventHandler DetailsViewCustomColumnDisplayText {
			add { Events.AddHandler(DetailsViewCustomColumnDisplayTextEventKey, value); }
			remove { Events.RemoveHandler(DetailsViewCustomColumnDisplayTextEventKey, value); }
		}
		[Category("Appearance")]
		public event FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventHandler DetailsViewCustomColumnHeaderFilterFillItems {
			add { Events.AddHandler(DetailsViewCustomColumnHeaderFilterFillItemsEventKey, value); }
			remove { Events.RemoveHandler(DetailsViewCustomColumnHeaderFilterFillItemsEventKey, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FileManagerFolder SelectedFolder {
			get {
				string selectedFolderPath = Helper.Data.SelectedFolderPath;
				if(this.selectedFolder == null || selectedFolder.RelativeName != selectedFolderPath) {
					this.selectedFolder = new FileManagerFolder(FileSystemProvider, selectedFolderPath, Helper.Data.GetFolderIdPath(selectedFolderPath));
					Helper.Edit.ValidateFoldersChain(this.selectedFolder);
				}
				return this.selectedFolder;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FileManagerFile SelectedFile {
			get {
				if(Helper.ClientState.State != null) {
					if(Settings.EnableMultiSelect) {
						var fileName = Helper.ClientState.GetFocusedItemName(false);
						if(!string.IsNullOrEmpty(fileName)) {
							Helper.Edit.ValidateName(fileName);
							return new FileManagerFile(FileSystemProvider, FileManagerItem.PathCombine(SelectedFolder.RelativeName, fileName));
						}
					}
				}
				var files = SelectedFiles;
				return files.Length > 0 ? files[0] : null;
			}
			set {
				if(value == null)
					SelectedFiles = null;
				else
					SelectedFiles = new FileManagerFile[] { value };
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FileManagerFile[] SelectedFiles {
			get {
				if(Helper.Data.ForcedlySelectedFiles != null)
					return Helper.Data.ForcedlySelectedFiles;
				if(Helper.ClientState.State != null) {
					ClientItemInfo[] filesInfo = Helper.ClientState.GetSelectedFilesInfo();
					if(filesInfo.Length > 0)
						return filesInfo.Select(f => {
							Helper.Edit.ValidateName(f.Name);
							return new FileManagerFile(FileSystemProvider, Path.Combine(SelectedFolder.RelativeName, f.Name), f.Id);
						}).ToArray();
				}
				return new FileManagerFile[0];
			}
			set {
				if(value == null)
					Helper.Data.ForcedlySelectedFiles = new FileManagerFile[0];
				else {
					if(!Settings.EnableMultiSelect && value.Length > 1)
						throw new Exception("Set the Settings.EnableMultiSelect property to True to use the SelectedFiles collection with more than one item");
					Helper.Data.ForcedlySelectedFiles = value.Select(f => new FileManagerFile(FileSystemProvider, f.RelativeName, f.IdInternal)).ToArray();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), UrlProperty]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		public void Refresh() {
			LayoutChanged();
			EnsureChildControls();
			Helper.ClientState.SyncCurrentPath();
			if(FilesGridView != null) {
				FilesGridView.DataSource = Helper.Data.GetItemsList(true);
				FilesGridView.DataBind();
			}
		}
		protected internal virtual string MapPath(string virtualPath) {
			return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
		}
		protected override StylesBase CreateStyles() {
			return new FileManagerStyles(this);
		}
		protected internal AppearanceStyleBase GetUploadPanelElementStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFontFrom(Styles.UploadPanel);
			return style;
		}
		internal bool HasThumbnailsViewFileAreaItemTemplate { get { return settingsFileList.ThumbnailsViewSettings.ItemTemplate != null; } }
		internal bool IsThumbnailsViewFileAreaItemTemplate { get { return settingsFileList.View == FileListView.Thumbnails && HasThumbnailsViewFileAreaItemTemplate; } }
		protected override ImagesBase CreateImages() {
			return new FileManagerImages(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new FileManagerClientSideEvents();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				Settings,
				SettingsFileList,
				SettingsFolders,
				SettingsToolbar,
				SettingsContextMenu,
				SettingsBreadcrumbs,
				SettingsUpload,
				SettingsEditing,
				SettingsDataSource,
				SettingsPermissions,
				StylesContextMenu,
				StylesDetailsView,
				ImagesDetailsView
			});
		}
		protected internal bool HasClientState() {
			return !NeedLoadClientState();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[UniqueID]);
		}
		protected internal override string SaveClientState() {
			EnsureChildControls();
			return Control != null ? Control.Container.SaveClientState() : string.Empty;
		}
		protected internal override void LoadClientState(string state) {
			EnsureChildControls();
			Control.Container.LoadClientState(state);
		}
		protected override bool HasContent() {
			return true;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.control = CreateFileManagerControl();
			Controls.Add(Control);
			this.noThumbnailImage = new Image();
			NoThumbnailImage.Visible = false;
			Controls.Add(NoThumbnailImage);
			if(Settings.EnableMultiSelect)
				CreateImage(out this.thumbnailCheckBoxImage);
			if(SettingsFileList.ShowFolders && SettingsEditing.AllowCreate)
				CreateImage(out this.folderDefaultThumbnailImage);
			if(SettingsBreadcrumbs.Visible) {
				CreateImage(out this.breadCrumbsSeparatorImage);
				if(settingsBreadcrumbs.ShowParentFolderButton) {
					CreateImage(out this.breadCrumbsUpButtonImage);
					CreateImage(out this.breadCrumbsUpButtonDisabledImage);
				}
			}
		}
		void CreateImage(out Image image) {
			image = new Image();
			image.Visible = false;
			Controls.Add(image);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetVisibility(Control, IsClientVisible(), true);
			base.PrepareControlHierarchy();
			if(IsRootFolderUnspecified())
				throw new ArgumentException(ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorRootFolderNotSpecified));
		}
		protected internal bool IsRootFolderUnspecified() {
			return !DesignMode && !FileSystemProvider.Exists(new FileManagerFolder(FileSystemProvider, string.Empty));
		}
#pragma warning disable 618
		protected internal virtual bool IsFilterAvailable() {
			return SettingsToolbar.ShowFilterBox;
		}
		protected internal virtual bool IsItemCreatingAvailable() {
			return SettingsToolbar.ShowCreateButton && SettingsEditing.AllowCreate && (SettingsFolders.Visible || settingsFileList.ShowFolders);
		}
		protected internal virtual bool IsItemDeletingAvailable() {
			return SettingsToolbar.ShowDeleteButton && SettingsEditing.AllowDelete;
		}
		protected internal virtual bool IsItemDownloadAvailable() {
			return SettingsToolbar.ShowDownloadButton && SettingsEditing.AllowDownload;
		}
		protected internal virtual bool IsItemMovingAvailable() {
			return SettingsToolbar.ShowMoveButton && SettingsEditing.AllowMove && (SettingsFolders.Visible || settingsFileList.ShowFolders);
		}
		protected internal virtual bool IsItemCopyAvailable() {
			return SettingsToolbar.ShowCopyButton && SettingsEditing.AllowCopy && (SettingsFolders.Visible || settingsFileList.ShowFolders);
		}
		protected internal virtual bool IsItemRenamingAvailable() {
			return SettingsToolbar.ShowRenameButton && SettingsEditing.AllowRename;
		}
		protected internal virtual bool IsRefreshAvailable() {
			return SettingsToolbar.ShowRefreshButton;
		}
#pragma warning restore 618
		internal bool IsVirtualScrollingEnabled() {
			return SettingsFileList.PageSize > 0;
		}
		protected internal virtual ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new FileManagerUploadControl(owner, this);
		}
		protected virtual FileManagerControl CreateFileManagerControl() {
			return new FileManagerControl(this);
		}
		protected virtual FileManagerSettings CreateSettings() {
			return new FileManagerSettings(this);
		}
		protected virtual FileManagerSettingsUpload CreateSettingsUpload() {
			return new FileManagerSettingsUpload(this);
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxTreeView), ASPxTreeView.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridScriptResourceName);
			RegisterIncludeScript(typeof(ASPxGridView), ASPxGridView.GridViewScriptResourceName);
			RegisterIncludeScript(typeof(FileManagerUploadControl), UploadControlScriptName);
			RegisterIncludeScript(typeof(ASPxFileManager), ScriptName);
		}
		protected string GetNoThumbnailImageRender() {
			return GetImageRender(NoThumbnailImage, Helper.GetNoThumbnailImage());
		}
		protected string GetFolderThumbnailImageRender() {
			return GetImageRender(FolderDefaultThumbnailImage, Helper.GetFolderImage());
		}
		protected internal string GetBreadCrumbsUpButtonImageRender() {
			return GetImageRender(BreadCrumbsUpButtonImage, Helper.GetBreadCrumbsUpButtonImage());
		}
		protected internal string GetBreadCrumbsSeparatorImageRender() {
			return GetImageRender(BreadCrumbsSeparatorImage, Helper.GetBreadCrumbsSeparatorImage());
		}
		string GetImageRender(Image image, ImagePropertiesBase properties) {
			return GetImageRender(image, properties, string.Empty);
		}
		string GetImageRender(Image image, ImagePropertiesBase properties, string cssClass) {
			image.Visible = true;
			properties.AssignToControl(image, DesignMode);
			image.CssClass = RenderUtils.CombineCssClasses(image.CssClass, cssClass);
			string render = RenderUtils.GetRenderResult(image);
			image.Visible = false;
			return render;
		}
		protected internal string GetThumbnailCheckBoxRender() {
			var checkBoxOwner = new FileManagerThumbnailCheckBoxOwner(this);
			var checkBox = new InternalCheckboxControl(checkBoxOwner);
			return RenderUtils.GetRenderResult(checkBox);
		}
		List<InternalCheckBoxImageProperties> GetCheckImages() {
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] {
				GetCheckImage(CheckState.Checked), 
				GetCheckImage(CheckState.Unchecked)
			});
		}
		internal InternalCheckBoxImageProperties GetCheckImage(CheckState checkState) {
			var result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			if(checkState == CheckState.Checked) {
				imageName = EditorImages.CheckBoxCheckedImageName;
				result.MergeWith(Images.ThumbnailsCheckBoxChecked);
				result.MergeWith(EditorImages.CheckBoxChecked);
			} else {
				imageName = InternalCheckboxControl.CheckBoxUncheckedImageName;
				result.MergeWith(Images.ThumbnailsCheckBoxUnchecked);
				result.MergeWith(EditorImages.CheckBoxUnchecked);
			}
			result.MergeWith(EditorImages.GetImageProperties(this.Page, imageName));
			EditorImages.UpdateSpriteUrl(result, this.Page, InternalCheckboxControl.WebSpriteControlName, typeof(ASPxWebControl), InternalCheckboxControl.DesignModeSpriteImagePath);
			return result;
		}
		internal string GetCurrentFolderId() {
			return string.IsNullOrEmpty(SelectedFolder.Id) ? string.Empty : SelectedFolder.Id;
		}
		protected internal bool IsThumbnailsViewMode {
			get { return SettingsFileList.View == FileListView.Thumbnails; }
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(settingsFileList.View == FileListView.Details) {
				stb.AppendLine(string.Format("{0}.viewMode = {1};", localVarName, (int)FileListView.Details));
				if(SettingsFileList.DetailsViewSettings.Columns.Any(c => c.FileInfoType == FileInfoType.FileName && c.ItemTemplate != null))
					stb.AppendLine(string.Format("{0}.nameColumnHasTemplate = true;", localVarName));
			}
			if(Settings.EnableMultiSelect) {
				stb.AppendFormat("{0}.allowMultiSelect = true;", localVarName);
				stb.AppendFormat("{0}.thumbnailCheckBox = {1};", localVarName, HtmlConvertor.ToJSON(GetThumbnailCheckBoxRender()));
				stb.AppendFormat("{0}.checkBoxImageProperties = {1};", localVarName, ImagePropertiesSerializer.GetImageProperties(GetCheckImages(), this));
				stb.AppendFormat("{0}.icbFocusedStyle = {1};\n", localVarName, InternalCheckboxControl.SerializeFocusedStyle(Styles.GetDefaultThumbnailCheckBoxFocusStyle(), this));
			}
			if(SettingsContextMenu.Enabled)
				stb.AppendLine(string.Format("{0}.enableContextMenu = true;", localVarName));
			if(IsThumbnailsViewFileAreaItemTemplate)
				stb.AppendLine(string.Format("{0}.isThumbnailsViewFileAreaItemTemplate = true;", localVarName));
			stb.AppendLine(
				string.Format(
					"{0}.SetStyles({1});",
					localVarName,
					HtmlConvertor.ToJSON(Helper.GetClientScriptStylesObject())
				)
			);
			if(Settings.FilterDelay != Settings.GetDefaultFilterDelay())
				stb.AppendFormat("{0}.filterDelay={1};\n", localVarName, Settings.FilterDelay);
			var items = Helper.Data.GetItemsClientHashtable(false); 
			if(settingsFileList.View == FileListView.Thumbnails) {
				stb.AppendLine(string.Format("{0}.noThumbnailImage={1};", localVarName,
					HtmlConvertor.ToJSON(GetNoThumbnailImageRender())
				));
				stb.AppendLine(string.Format("{0}.customThumbnails = {1};", localVarName,
					HtmlConvertor.ToJSON(Helper.Data.CustomThumbnails)
				));
			}
			if(IsVirtualScrollingEnabled()) {
				stb.AppendLine(string.Format("{0}.setVirtScrollConfig({1}, {2}, {3}, {4});", 
					localVarName, Helper.Data.VirtScrollItemIndex, Helper.Data.ItemsCount, SettingsFileList.PageSize, Helper.Data.VirtScrollPageItemsCount));
			}
			if(CurrentCommand != null && !(CurrentCommand is FileManagerGetFileListCommand) && !(CurrentCommand is FileManagerServerProcessFileOpenedCommand))
				stb.AppendLine(string.Format("{0}.ClearItems(true);", localVarName));
			stb.AppendLine(string.Format("{0}.CreateItems({1});", localVarName, HtmlConvertor.ToJSON(items)));
			stb.AppendLine(string.Format("{0}.currentFolderId = {1};", localVarName, HtmlConvertor.ToScript(GetCurrentFolderId())));
			if(SettingsUpload.Enabled) {
				stb.AppendLine(string.Format("{0}.uploadText = \"{1}\";", localVarName, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadButton)));
				stb.AppendLine(string.Format("{0}.cancelUploadText = \"{1}\";", localVarName, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_CancelButton)));
				if(!SettingsUpload.ShowUploadPanel)
					stb.AppendLine(string.Format("{0}.hideUploadPanel = true;", localVarName));
				if(SelectedFolder != null && FileSystemProvider.CanUpload(SelectedFolder)) {
					stb.AppendLine(string.Format("{0}.allowUploadToCurrentFolder = true;", localVarName));
					if(SettingsUpload.AdvancedModeSettings.EnableClientAccessRuleValidation) {
						var filesRules = Helper.Data.GetFilesAccessRulesClientArray(SelectedFolder);
						if(filesRules != null)
							stb.AppendLine(string.Format("{0}.UpdateFilesRules({1});", localVarName, HtmlConvertor.ToJSON(filesRules)));
					}
				}
			}
			else
				stb.AppendLine(string.Format("{0}.allowUpload = false;", localVarName));
			if(SettingsEditing.AllowRename && (SettingsFileList.View != FileListView.Details || SettingsFileList.DetailsViewSettings.ColumnsInternal.Any(c => c.FileInfoType != FileInfoType.Thumbnail)))
				stb.AppendLine(string.Format("{0}.allowRename = true;", localVarName));
			if(SettingsEditing.AllowMove && (SettingsFolders.Visible || settingsFileList.ShowFolders))
				stb.AppendLine(string.Format("{0}.allowMove = true;", localVarName));
			if(SettingsEditing.AllowCopy && (SettingsFolders.Visible || settingsFileList.ShowFolders))
				stb.AppendLine(string.Format("{0}.allowCopy = true;", localVarName));
			if(!SettingsFolders.Visible) {
				if(SelectedFolder != null) 
					stb.AppendLine(string.Format("{0}.currentPath = {1};", localVarName, HtmlConvertor.ToScript(SelectedFolder.RelativeName)));
				stb.AppendLine(string.Format("{0}.foldersHidden = true;", localVarName));
				stb.AppendLine(string.Format("{0}.rootFolderName = {1};", localVarName, HtmlConvertor.ToScript(FileSystemProvider.RootFolderDisplayName)));
			}
			if(settingsFileList.ShowFolders) {
				if(SettingsEditing.AllowCreate)
					stb.AppendLine(string.Format("{0}.folderThumbnailImageRender = {1};", localVarName, HtmlConvertor.ToJSON(GetFolderThumbnailImageRender())));
				stb.AppendLine(string.Format("{0}.showFoldersInFileArea = true;", localVarName));
			}
			if(settingsFileList.ShowParentFolder)
				stb.AppendLine(string.Format("{0}.showParentFolder = true;", localVarName));
			if(SettingsBreadcrumbs.Visible) {
				stb.AppendLine(string.Format("{0}.breadCrumbsEnabled = true;", localVarName));
				stb.AppendLine(string.Format("{0}.breadCrumbsSeparatorImage = {1};", localVarName, HtmlConvertor.ToJSON(GetBreadCrumbsSeparatorImageRender())));
				if(SettingsBreadcrumbs.ShowParentFolderButton) {
					stb.AppendLine(string.Format("{0}.showBreadCrumbsFolderUpButton = true;", localVarName));
					stb.AppendLine(string.Format("{0}.breadCrumbsUpButtonImage = {1};", localVarName, HtmlConvertor.ToJSON(GetBreadCrumbsUpButtonImageRender())));
				}
				stb.AppendLine(string.Format("{0}.createBreadCrumbs();", localVarName));
			}
			if(!SettingsToolbar.ShowPath)
				stb.AppendLine(string.Format("{0}.showPath = false;", localVarName));
			else if(Settings.UseAppRelativePath)
				stb.AppendLine(string.Format("{0}.showAppRelativePath = true;", localVarName));
			if(SettingsEditing.AllowDelete) {
				stb.AppendLine(string.Format("{0}.deleteConfirmText = \"{1}\";", localVarName, ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_DeleteConfirm)));
				stb.AppendLine(string.Format("{0}.allowDelete = true;", localVarName));
			}
			if(!string.IsNullOrEmpty(Helper.Edit.DownloadError))
				stb.AppendLine(string.Format("{0}.downloadError = {1};", localVarName, Helper.Edit.DownloadError));
			if(SettingsEditing.AllowCreate && (SettingsFolders.Visible || SettingsFileList.ShowFolders)) {
				stb.AppendLine(string.Format("{0}.allowCreate = true;", localVarName));
				stb.AppendLine(string.Format("{0}.newFolderName = \"{1}\";", localVarName, ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_NewFolderName)));
			}
			if(!Helper.GetUploadIsValid())
				stb.AppendLine(string.Format("{0}.uploadErrorText = \"{1}\";", localVarName, Helper.GetUploadErrorText()));
			if(SelectedFolder != null) 
				stb.AppendLine(string.Format("{0}.UpdateFolderRights(\"{1}\");", localVarName, Helper.Data.GetClientFolderRightsScript(SelectedFolder)));
			if(Events[SelectedFileOpenedEventKey] != null)
				stb.AppendLine(string.Format("{0}.processOpenedEventOnServer = true;", localVarName));
			if(CurrentCommand != null && !(CurrentCommand is FileManagerGetFileListCommand))
				stb.AppendLine(string.Format("{0}.ProcessCommandResult({1});", localVarName, HtmlConvertor.ToJSON(GetCallbackResult())));
			else if(Helper.Data.ForcedlySelectedFiles != null)
				stb.AppendLine(string.Format("{0}.selectedItems = {1};", localVarName, HtmlConvertor.ToJSON(Helper.Data.GetVisibleForcedlySelectedFilesIds())));
		}
		protected override Hashtable GetClientObjectState() {
			return Helper.ClientState.State;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientFileManager";
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			RaisePostBackEventCore(eventArgument);
			base.RaisePostBackEvent(eventArgument);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			RaisePostBackEventCore(eventArgument);
		}
		protected void RaisePostBackEventCore(string arg) {
			if(!base.IsEnabled()) return; 
			EnsureChildControls();
			var linkedSeparatorPosition = arg.IndexOf(FileManagerCommandsHelper.LinkedArgumentSeparator);
			var linkedArg = linkedSeparatorPosition > -1 ? arg.Substring(linkedSeparatorPosition + FileManagerCommandsHelper.LinkedArgumentSeparator.Length) : null;
			if(!string.IsNullOrEmpty(linkedArg))
				arg = arg.Substring(0, arg.Length - linkedArg.Length - FileManagerCommandsHelper.LinkedArgumentSeparator.Length);
			int separatorPos = arg.IndexOf(FileManagerCommandsHelper.ArgumentSeparator);
			FileManagerCommandId commandId = (FileManagerCommandId)Int32.Parse(separatorPos < 0 ? arg : arg.Substring(0, separatorPos));
			string commandArgs = separatorPos > -1 ? arg.Substring(separatorPos + FileManagerCommandsHelper.ArgumentSeparator.Length) : string.Empty;
			CurrentCommand = FileManagerCommandsHelper.CreateCommand(this, commandId, commandArgs);
			CurrentCommand.Execute();
			if(!string.IsNullOrEmpty(linkedArg)) {
				separatorPos = linkedArg.IndexOf(FileManagerCommandsHelper.ArgumentSeparator);
				commandId = (FileManagerCommandId)Int32.Parse(separatorPos < 0 ? linkedArg : linkedArg.Substring(0, separatorPos));
				if(commandId == FileManagerCommandId.GridView)
					FilesGridView.RaiseCallbackEventCore(separatorPos > -1 ? linkedArg.Substring(separatorPos + 1) : string.Empty);
			}
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			BeginRendering();
			try {
				return CurrentCommand.GetCallbackResult();
			}
			finally {
				EndRendering();
			}
		}
		protected internal virtual string GetCallbackContentControlResult() {
			FileManagerItems items = GetCallbackResultControl();
			return items != null ? items.GetCallbackResultCore() : string.Empty;
		}
		protected virtual FileManagerItems GetCallbackResultControl() {
			return Items;
		}
		protected internal string HandleCallbackException(Exception ex) {
			string errorText = IsFileManagerException(ex) ? ex.Message : ProcessCallbackException(ex);
			return RaiseCustomErrorText(ex, errorText);
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			EnsureChildControls();
			Hashtable clientObjectState = LoadClientObjectState(postCollection);
			if(clientObjectState == null) return false;
			Helper.ClientState.SyncClientState(clientObjectState);
			if(FilesGridView != null && !IsCallback)
				FilesGridView.DataBind();
			return false;
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected internal virtual bool IsNeedToAddCallbackCommandResult() {
			return IsCallback;
		}
		protected internal FileManagerActionEventArgsBase RaiseFolderCreating(string name, FileManagerFolder parentFolder) {
			FileManagerFolderCreateEventHandler handler = (FileManagerFolderCreateEventHandler)Events[FolderCreatingEventKey];
			FileManagerFolderCreateEventArgs args = new FileManagerFolderCreateEventArgs(name, parentFolder);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseFolderCreated(FileManagerFolder folder, FileManagerFolder parentFolder) {
			FileManagerFolderCreatedEventHandler handler = (FileManagerFolderCreatedEventHandler)Events[FolderCreatedEventKey];
			FileManagerFolderCreatedEventArgs args = new FileManagerFolderCreatedEventArgs(folder, parentFolder);
			if(handler != null)
				handler(this, args);
		}
		protected internal FileManagerActionEventArgsBase RaiseItemRenaming(string newName, FileManagerItem item) {
			FileManagerItemRenameEventHandler handler = (FileManagerItemRenameEventHandler)Events[ItemRenamingEventKey];
			FileManagerItemRenameEventArgs args = new FileManagerItemRenameEventArgs(newName, item);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseItemRenamed(string oldName, FileManagerItem item) {
			FileManagerItemRenamedEventHandler handler = (FileManagerItemRenamedEventHandler)Events[ItemRenamedEventKey];
			FileManagerItemRenamedEventArgs args = new FileManagerItemRenamedEventArgs(oldName, item);
			if(handler != null)
				handler(this, args);
		}
		protected internal FileManagerActionEventArgsBase RaiseItemDeleting(FileManagerItem item) {
			FileManagerItemDeleteEventHandler handler = (FileManagerItemDeleteEventHandler)Events[ItemDeletingEventKey];
			FileManagerItemDeleteEventArgs args = new FileManagerItemDeleteEventArgs(item);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseItemsDeleted(FileManagerItem[] items) {
			FileManagerItemsDeletedEventHandler handler = (FileManagerItemsDeletedEventHandler)Events[ItemsDeletedEventKey];
			FileManagerItemsDeletedEventArgs args = new FileManagerItemsDeletedEventArgs(items);
			if(handler != null)
				handler(this, args);
		}
		protected internal FileManagerActionEventArgsBase RaiseItemMoving(FileManagerFolder destinationFolder, FileManagerItem item) {
			FileManagerItemMoveEventHandler handler = (FileManagerItemMoveEventHandler)Events[ItemMovingEventKey];
			FileManagerItemMoveEventArgs args = new FileManagerItemMoveEventArgs(destinationFolder, item);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseItemsMoved(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			FileManagerItemsMovedEventHandler handler = (FileManagerItemsMovedEventHandler)Events[ItemsMovedEventKey];
			FileManagerItemsMovedEventArgs args = new FileManagerItemsMovedEventArgs(sourceFolder, items);
			if(handler != null)
				handler(this, args);
		}
		protected internal FileManagerActionEventArgsBase RaiseItemCopying(FileManagerFolder destinationFolder, FileManagerItem item) {
			FileManagerItemCopyEventHandler handler = (FileManagerItemCopyEventHandler)Events[ItemCopyingEventKey];
			FileManagerItemCopyEventArgs args = new FileManagerItemCopyEventArgs(destinationFolder, item);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseItemsCopied(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			FileManagerItemsCopiedEventHandler handler = (FileManagerItemsCopiedEventHandler)Events[ItemsCopiedEventKey];
			FileManagerItemsCopiedEventArgs args = new FileManagerItemsCopiedEventArgs(sourceFolder, items);
			if(handler != null)
				handler(this, args);
		}
		protected internal FileManagerFileUploadEventArgs RaiseFileUploading(FileManagerFile file, Stream stream) {
			FileManagerFileUploadEventHandler handler = (FileManagerFileUploadEventHandler)Events[FileUploadingEventKey];
			FileManagerFileUploadEventArgs args = new FileManagerFileUploadEventArgs(file, stream);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseFilesUploaded(FileManagerFile[] files) {
			FileManagerFilesUploadedEventHandler handler = (FileManagerFilesUploadedEventHandler)Events[FilesUploadedEventKey];
			FileManagerFilesUploadedEventArgs args = new FileManagerFilesUploadedEventArgs(files);
			if(handler != null)
				handler(this, args);
		}
		protected internal ImageProperties RaiseThumbnailCreate(FileManagerItem item, bool isParentFolder) {
			FileManagerThumbnailCreateEventHandler handler = (FileManagerThumbnailCreateEventHandler)Events[CustomThumbnailEventKey];
			FileManagerThumbnailCreateEventArgs args = new FileManagerThumbnailCreateEventArgs(item, isParentFolder);
			if(handler != null)
				handler(this, args);
			return args.ThumbnailImage;
		}
		protected internal string RaiseCustomErrorText(Exception e, string errorText) {
			FileManagerCustomErrorTextEventHandler handler = (FileManagerCustomErrorTextEventHandler)Events[CustomErrorTextEventKey];
			FileManagerCustomErrorTextEventArgs args = new FileManagerCustomErrorTextEventArgs(e, errorText);
			if(handler != null)
				handler(this, args);
			return args.ErrorText;
		}
		protected internal FileManagerFileDownloadingEventArgs RaiseDownloading(FileManagerFile file, Stream stream) {
			FileManagerFileDownloadingEventHandler handler = (FileManagerFileDownloadingEventHandler)Events[FileDownloadingEventKey];
			FileManagerFileDownloadingEventArgs args = new FileManagerFileDownloadingEventArgs(file, stream);
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseSelectedFileOpened(FileManagerFile file) {
			FileManagerFileOpenedEventHandler handler = (FileManagerFileOpenedEventHandler)Events[SelectedFileOpenedEventKey];
			FileManagerFileOpenedEventArgs args = new FileManagerFileOpenedEventArgs(file);
			if(handler != null)
				handler(this, args);
		}
		protected internal void RaiseCustomCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = (CallbackEventHandlerBase)Events[CustomCallbackEventKey];
			if(handler != null)
				handler(this, e);
		}
		protected internal string RaiseCustomFileInfoDisplayText(FileManagerFile file, FileInfoType fileInfoType, string displayText, out bool encodeHtml) {
			FileManagerCustomFileInfoDisplayTextEventHandler handler = (FileManagerCustomFileInfoDisplayTextEventHandler)Events[CustomFileInfoDisplayTextKey];
			FileManagerCustomFileInfoDisplayTextEventArgs args = new FileManagerCustomFileInfoDisplayTextEventArgs(file, fileInfoType, displayText);
			if(handler != null)
				handler(this, args);
			encodeHtml = args.EncodeHtml;
			return args.DisplayText;
		}
		protected internal FileManagerDetailsViewCustomColumnDisplayTextEventArgs RaiseDetailsViewColumnDisplayText(FileManagerDetailsViewCustomColumnDisplayTextEventArgs e) {
			FileManagerDetailsViewCustomColumnDisplayTextEventHandler handler = (FileManagerDetailsViewCustomColumnDisplayTextEventHandler)Events[DetailsViewCustomColumnDisplayTextEventKey];
			FileManagerDetailsViewCustomColumnDisplayTextEventArgs args = e;
			if(handler != null)
				handler(this, args);
			return args;
		}
		protected internal void RaiseDetailsViewCustomColumnHeaderFilterFillItems(FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs e) {
			FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventHandler handler = (FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventHandler)Events[DetailsViewCustomColumnHeaderFilterFillItemsEventKey];
			FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs args = e;
			if(handler != null)
				handler(this, args);
		}
		protected internal void RaiseCloudProviderRequest(FileManagerCloudProviderRequestEventArgs e) {
			FileManagerCloudProviderRequestEventHandler handler = (FileManagerCloudProviderRequestEventHandler)Events[CloudProviderRequestEventKey];
			FileManagerCloudProviderRequestEventArgs args = e;
			if(handler != null)
				handler(this, args);
		}
		public override void DataBind() {
			CreateFileSystemProvider();
			LayoutChanged();
			if(!string.IsNullOrEmpty(this.DataSourceID))
				RaiseDataBinding();
			RaiseDataBound();
		}
		protected virtual void CreateFileSystemProvider() {
			FileSystemProviderBase provider = CustomFileSystemProvider;
			if(!DesignMode && provider == null && !string.IsNullOrEmpty(CustomFileSystemProviderTypeName))
				provider = (FileSystemProviderBase)System.Activator.CreateInstance(System.Web.Compilation.BuildManager.GetType(CustomFileSystemProviderTypeName, true, true), Settings.RootFolder);
			if(provider == null && ProviderType != FileManagerProviderType.NotSet) {
				switch(ProviderType) {
					case (FileManagerProviderType.Custom):
						if(!DesignMode && provider == null && !string.IsNullOrEmpty(CustomFileSystemProviderTypeName))
							provider = (FileSystemProviderBase)System.Activator.CreateInstance(System.Web.Compilation.BuildManager.GetType(CustomFileSystemProviderTypeName, true, true), Settings.RootFolder);
						break;
					case (FileManagerProviderType.DataSource):
						if(!DesignMode && provider == null && (!string.IsNullOrEmpty(DataSourceID) || DataSource != null))
							provider = new DataSourceFileSystemProvider(Settings.RootFolder);
						break;
					case (FileManagerProviderType.Physical):
						if(provider == null)
							provider = new PhysicalFileSystemProvider(Settings.RootFolder);
						break;
					case (FileManagerProviderType.Amazon):
						var amazonProvider = new AmazonFileSystemProvider(Settings.RootFolder);
						amazonProvider.AccessKeyID = SettingsAmazon.AccessKeyID;
						amazonProvider.BucketName = SettingsAmazon.BucketName;
						amazonProvider.SecretAccessKey = SettingsAmazon.SecretAccessKey;
						amazonProvider.Region = SettingsAmazon.Region;
						provider = amazonProvider;
						break;
					case (FileManagerProviderType.Azure):
						var azureProvider = new AzureFileSystemProvider(Settings.RootFolder);
						azureProvider.AccessKey = SettingsAzure.AccessKey;
						azureProvider.ContainerName = SettingsAzure.ContainerName;
						azureProvider.StorageAccountName = SettingsAzure.StorageAccountName;
						provider = azureProvider;
						break;
					case (FileManagerProviderType.Dropbox):
						var dropboxProvider = new DropboxFileSystemProvider(Settings.RootFolder);
						dropboxProvider.AccessTokenValue = SettingsDropbox.AccessTokenValue;
						provider = dropboxProvider;
						break;
				}
			} else {
				if(!DesignMode && provider == null && (!string.IsNullOrEmpty(DataSourceID) || DataSource != null))
					provider = new DataSourceFileSystemProvider(Settings.RootFolder);
				if(provider == null)
					provider = new PhysicalFileSystemProvider(Settings.RootFolder);
			}
			DataSourceFileSystemProvider dsProvider = provider as DataSourceFileSystemProvider;
			if(dsProvider != null)
				SetupDataSourceFileSystemProvider(dsProvider);
			CloudFileSystemProviderBase cloudProvider = provider as CloudFileSystemProviderBase;
			if(cloudProvider != null)
				cloudProvider.RequestEvent += cloudProvider_RequestEvent;
			CreateRestrictedAccessFileSystemProvider(provider);
		}
		void cloudProvider_RequestEvent(FileManagerCloudProviderRequestEventArgs e) {
			RaiseCloudProviderRequest(e);
		}
		protected void SetupDataSourceFileSystemProvider(DataSourceFileSystemProvider provider) {
			provider.DataHelper = new DataHelperCore(this, DataSource, DataSourceID, DataMember);
			provider.IsFolderFieldName = SettingsDataSource.IsFolderFieldName;
			provider.KeyFieldName = SettingsDataSource.KeyFieldName;
			provider.ParentKeyFieldName = SettingsDataSource.ParentKeyFieldName;
			provider.NameFieldName = SettingsDataSource.NameFieldName;
			provider.LastWriteTimeFieldName = SettingsDataSource.LastWriteTimeFieldName;
			provider.FileBinaryContentFieldName = SettingsDataSource.FileBinaryContentFieldName;
		}
		protected void CreateRestrictedAccessFileSystemProvider(FileSystemProviderBase provider) {
			RestrictedAccessFileSystemProvider rafsProvider = new RestrictedAccessFileSystemProvider(provider, this);
			SettingsPermissions.AccessRules.RegisterFileSystemProvider(rafsProvider);
			this.fileSystemProvider = rafsProvider;
		}
		protected internal void ResetFileSystemProvider() {
			ResetFileSystemProvider(IsNeedResetToInitalFolder());
		}
		protected internal void ResetFileSystemProvider(bool toInitialFolder) {
			if(this.fileSystemProvider != null) {
				this.fileSystemProvider = null;
				Helper.ClientState.ResetClientState();
				ResetFileList(toInitialFolder);
				Helper.Data.ResetVirtScrollState();
			}
		}
		protected internal void ResetFileList(bool toInitialFolder) {
			Helper.Data.ItemListCache = null;
			if(toInitialFolder)
				Helper.Data.NeedResetToInitialFolder = true;
		}
		protected virtual bool IsNeedResetToInitalFolder() {
			return true;
		}
		internal bool IsClientUploadAccessRulesValidationEnabled() {
			return SettingsUpload.Enabled && SettingsUpload.AdvancedModeSettings.EnableClientAccessRuleValidation 
				&& SelectedFolder != null && FileSystemProvider.CanUpload(SelectedFolder);
		}
		protected internal static bool IsFileManagerException(Exception ex) {
			return typeof(FileManagerException).IsAssignableFrom(ex.GetType());
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.FileManagerCommonFormDesigner"; } }
	}
}
