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
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	public class FileManagerSettings : SettingsBase {
		SettingsLoadingPanel settingsLoadingPanel;
		FileManagerDetailsViewImages imagesDetailsView;
		FileManagerDetailsViewStyles stylesDetailsView;
		MVCxFileManagerSettings settings;
		FileManagerSettingsEditing settingsEditing;
		MVCxFileManagerSettingsFileList settingsFileList;
		FileManagerSettingsFolders settingsFolders;
		FileManagerSettingsPermissions settingsPermissions;
		FileManagerSettingsToolbar settingsToolbar;
		FileManagerSettingsContextMenu settingsContextMenu;
		FileManagerSettingsBreadcrumbs settingsBreadcrumbs;
		MVCxFileManagerSettingsUpload settingsUpload;
		public FileManagerSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			this.imagesDetailsView = new FileManagerDetailsViewImages(null);
			this.stylesDetailsView = new FileManagerDetailsViewStyles(null);
			this.settings = new MVCxFileManagerSettings() { RootFolder = string.Empty };
			this.settingsEditing = new FileManagerSettingsEditing(null);
			this.settingsFileList = new MVCxFileManagerSettingsFileList(null);
			this.settingsFolders = new FileManagerSettingsFolders(null);
			this.settingsPermissions = new FileManagerSettingsPermissions(null);
			this.settingsToolbar = new FileManagerSettingsToolbar(null);
			this.settingsContextMenu = new FileManagerSettingsContextMenu(null);
			this.settingsBreadcrumbs = new FileManagerSettingsBreadcrumbs(null);
			this.settingsUpload = new MVCxFileManagerSettingsUpload() { FileManagerSettings = this.settings };
		}
		public object CallbackRouteValues { get; set; }
		public object DownloadRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public FileManagerClientSideEvents ClientSideEvents { get { return (FileManagerClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public FileSystemProviderBase CustomFileSystemProvider { get; set; }
		public string CustomFileSystemProviderTypeName { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public MVCxFileManagerSettings Settings { get { return settings; } }
		public FileManagerSettingsEditing SettingsEditing { get { return settingsEditing; } }
		public MVCxFileManagerSettingsFileList SettingsFileList { get { return settingsFileList; } }
		public FileManagerSettingsFolders SettingsFolders { get { return settingsFolders; } }
		public FileManagerSettingsPermissions SettingsPermissions { get { return settingsPermissions; } }
		public FileManagerSettingsToolbar SettingsToolbar { get { return settingsToolbar; } }
		public FileManagerSettingsContextMenu SettingsContextMenu { get { return settingsContextMenu; } }
		public FileManagerSettingsBreadcrumbs SettingsBreadcrumbs { get { return settingsBreadcrumbs; } }
		public MVCxFileManagerSettingsUpload SettingsUpload { get { return settingsUpload; } }
		public FileManagerImages Images { get { return (FileManagerImages)ImagesInternal; } }
		public FileManagerDetailsViewImages ImagesDetailsView { get { return imagesDetailsView; } }
		public FileManagerStyles Styles { get { return (FileManagerStyles)StylesInternal; } }
		public FileManagerDetailsViewStyles StylesDetailsView { get { return stylesDetailsView; } }
		public FileManagerCustomErrorTextEventHandler CustomErrorText { get; set; }
		public FileManagerCustomFileInfoDisplayTextEventHandler CustomFileInfoDisplayText { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public FileManagerThumbnailCreateEventHandler CustomThumbnail { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		public FileManagerFileUploadEventHandler FileUploading { get; set; }
		public FileManagerFilesUploadedEventHandler FilesUploaded { get; set; }
		public FileManagerFolderCreateEventHandler FolderCreating { get; set; }
		public FileManagerFolderCreatedEventHandler FolderCreated { get; set; }
		public FileManagerItemRenameEventHandler ItemRenaming { get; set; }
		public FileManagerItemRenamedEventHandler ItemRenamed { get; set; }
		public FileManagerItemDeleteEventHandler ItemDeleting { get; set; }
		public FileManagerItemsDeletedEventHandler ItemsDeleted { get; set; }
		public FileManagerItemMoveEventHandler ItemMoving { get; set; }
		public FileManagerItemsMovedEventHandler ItemsMoved { get; set; }
		public FileManagerItemCopyEventHandler ItemCopying { get; set; }
		public FileManagerItemsCopiedEventHandler ItemsCopied { get; set; }
		public FileManagerDetailsViewCustomColumnDisplayTextEventHandler DetailsViewCustomColumnDisplayText { get; set; }
		public FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventHandler DetailsViewCustomColumnHeaderFilterFillItems { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new FileManagerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new FileManagerImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new FileManagerStyles(null);
		}
	}
	public class MVCxFileManagerSettings : DevExpress.Web.FileManagerSettings {
		public MVCxFileManagerSettings()
			: this(null) {
		}
		public MVCxFileManagerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string RootFolder { get; set; } 
	}
	public class MVCxFileManagerSettingsUpload : FileManagerSettingsUpload {
		public MVCxFileManagerSettingsUpload()
			: this(null) {
		}
		public MVCxFileManagerSettingsUpload(IPropertiesOwner owner)
			: base(owner) { 
			FileManagerSettings = new MVCxFileManagerSettings();
		}
		public new MVCxFileManagerValidationSettings ValidationSettings {
			get { return (MVCxFileManagerValidationSettings)base.ValidationSettings; }
		}
		protected internal MVCxFileManagerSettings FileManagerSettings { get; set; }
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return Owner != null ? new MVCxFileManagerValidationSettings(Owner) : new MVCxFileManagerValidationSettings(this);
		}
	}
	public class MVCxFileManagerValidationSettings : FileManagerValidationSettings {
		string[] allowedFileExtensions = new string[0];
		public MVCxFileManagerValidationSettings(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string[] AllowedFileExtensions {
			get { return UploadSettings != null ? UploadSettings.FileManagerSettings.AllowedFileExtensions : allowedFileExtensions; }
			set { allowedFileExtensions = value; }
		}
		protected MVCxFileManagerSettingsUpload UploadSettings {
			get { return Owner as MVCxFileManagerSettingsUpload; }
		}
	}
	public class MVCxFileManagerSettingsFileList : FileManagerSettingsFileList {
		public MVCxFileManagerSettingsFileList(IPropertiesOwner owner) : base(owner) { }
		public new MVCxFileManagerFileListDetailsViewSettings DetailsViewSettings {
			get { return (MVCxFileManagerFileListDetailsViewSettings)base.DetailsViewSettings; }
		}
		public new MVCxFileManagerFileListThumbnailsViewSettings ThumbnailsViewSettings {
			get { return (MVCxFileManagerFileListThumbnailsViewSettings)base.ThumbnailsViewSettings; }
		}
		protected override FileManagerFileListDetailsViewSettings CreateDetailsViewSettings() {
			return Owner != null ? new MVCxFileManagerFileListDetailsViewSettings(Owner) : new MVCxFileManagerFileListDetailsViewSettings(this);
		}
		protected override FileManagerFileListThumbnailsViewSettings CreateThumbnailsViewSettings() {
			return Owner != null ? new MVCxFileManagerFileListThumbnailsViewSettings(Owner) : new MVCxFileManagerFileListThumbnailsViewSettings(this);
		}
	}
	public class MVCxFileManagerFileListDetailsViewSettings : FileManagerFileListDetailsViewSettings {
		public MVCxFileManagerFileListDetailsViewSettings(IPropertiesOwner owner) : base(owner) { }
		public new MVCxFileManagerDetailsColumnCollection Columns {
			get { return (MVCxFileManagerDetailsColumnCollection)base.Columns; }
		}
		protected override FileManagerDetailsColumnCollection CreateColumnCollection() {
			return new MVCxFileManagerDetailsColumnCollection();
		}
	}
	public class MVCxFileManagerFileListThumbnailsViewSettings : FileManagerFileListThumbnailsViewSettings {
		public MVCxFileManagerFileListThumbnailsViewSettings(IPropertiesOwner owner) : base(owner) { }
		protected internal string ThumbnailViewItemTemplateContent { get; set; }
		protected internal Action<FileManagerThumbnailsViewItemTemplateContainer> ThumbnailViewItemTemplateContentMethod { get; set; }
		public void SetThumbnailViewItemTemplateContent(Action<FileManagerThumbnailsViewItemTemplateContainer> contentMethod) {
			ThumbnailViewItemTemplateContentMethod = contentMethod;
		}
		public void SetThumbnailViewItemTemplateContent(string content) {
			ThumbnailViewItemTemplateContent = content;
		}
	}
	public class MVCxFileManagerDetailsColumnCollection : FileManagerDetailsColumnCollection {
		public MVCxFileManagerDetailsColumnCollection() : base() { }
		public MVCxFileManagerDetailsColumnCollection(IWebControlObject owner) : base(owner) { }
		public new MVCxFileManagerDetailsColumn Add(FileInfoType fileInfoType, string caption) {
			return (MVCxFileManagerDetailsColumn)base.Add(fileInfoType, caption);
		}
		public void Add(Action<MVCxFileManagerDetailsColumn> method) {
			if(method != null)
				method(Add());
		}
		public MVCxFileManagerDetailsColumn Add() {
			MVCxFileManagerDetailsColumn column = new MVCxFileManagerDetailsColumn();
			Add(column);
			return column;
		}
		public void AddCustomColumn(Action<MVCxFileManagerDetailsCustomColumn> method) {
			if(method != null)
				method(AddCustomColumn());
		}
		public MVCxFileManagerDetailsCustomColumn AddCustomColumn() {
			MVCxFileManagerDetailsCustomColumn column = new MVCxFileManagerDetailsCustomColumn();
			Add(column);
			return column;
		}
		protected override FileManagerDetailsColumn CreateColumn(FileInfoType fileInfoType, string caption) {
			return new MVCxFileManagerDetailsColumn(fileInfoType, caption);
		}
		protected override FileManagerDetailsColumn CreateColumn(FileInfoType fileInfoType) {
			return new MVCxFileManagerDetailsColumn(fileInfoType);
		}
	}
	public class MVCxFileManagerDetailsColumn : FileManagerDetailsColumn {
		public MVCxFileManagerDetailsColumn() : base() { }
		public MVCxFileManagerDetailsColumn(FileInfoType fileInfoType) : base(fileInfoType) { }
		public MVCxFileManagerDetailsColumn(FileInfoType fileInfoType, string caption) : base(fileInfoType, caption) { }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<FileManagerDetailsViewItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		public void SetItemTemplateContent(Action<FileManagerDetailsViewItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		public override void Assign(CollectionItem source) {
			MVCxFileManagerDetailsColumn column = source as MVCxFileManagerDetailsColumn;
			if(column != null) {
				ItemTemplateContent = column.ItemTemplateContent;
				ItemTemplateContentMethod = column.ItemTemplateContentMethod;
			}
			base.Assign(source);
		}
	}
	public class MVCxFileManagerDetailsCustomColumn : FileManagerDetailsCustomColumn {
		public MVCxFileManagerDetailsCustomColumn() : base() { }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<FileManagerDetailsViewItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		public void SetItemTemplateContent(Action<FileManagerDetailsViewItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		public override void Assign(CollectionItem source) {
			MVCxFileManagerDetailsCustomColumn column = source as MVCxFileManagerDetailsCustomColumn;
			if(column != null) {
				ItemTemplateContent = column.ItemTemplateContent;
				ItemTemplateContentMethod = column.ItemTemplateContentMethod;
			}
			base.Assign(source);
		}
	}
}
