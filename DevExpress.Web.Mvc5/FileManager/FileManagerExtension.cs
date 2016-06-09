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
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public class FileManagerExtension : ExtensionBase {
		const string DownloadCommandArgument = "DXMVCFileManagerDownloadArgument";
		public FileManagerExtension(FileManagerSettings settings)
			: base(settings) {
		}
		public FileManagerExtension(FileManagerSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxFileManager Control {
			get { return (MVCxFileManager)base.Control; }
		}
		protected internal new FileManagerSettings Settings {
			get { return (FileManagerSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.DownloadRouteValues = Settings.DownloadRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.CustomFileSystemProvider = Settings.CustomFileSystemProvider;
			Control.CustomFileSystemProviderTypeName = Settings.CustomFileSystemProviderTypeName;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsEditing.Assign(Settings.SettingsEditing);
			Control.SettingsFileList.Assign(Settings.SettingsFileList);
			Control.SettingsFolders.Assign(Settings.SettingsFolders);
			Control.SettingsPermissions.Assign(Settings.SettingsPermissions);
			Control.SettingsToolbar.Assign(Settings.SettingsToolbar);
			Control.SettingsContextMenu.Assign(Settings.SettingsContextMenu);
			Control.SettingsBreadcrumbs.Assign(Settings.SettingsBreadcrumbs);
			Control.SettingsUpload.Assign(Settings.SettingsUpload);
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagesDetailsView.CopyFrom(Settings.ImagesDetailsView);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesDetailsView.CopyFrom(Settings.StylesDetailsView);
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.CustomErrorText += Settings.CustomErrorText;
			Control.CustomFileInfoDisplayText += Settings.CustomFileInfoDisplayText;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.CustomThumbnail += Settings.CustomThumbnail;
			Control.FileUploading += Settings.FileUploading;
			Control.FilesUploaded += Settings.FilesUploaded;
			Control.FolderCreating += Settings.FolderCreating;
			Control.FolderCreated += Settings.FolderCreated;
			Control.ItemCopying += Settings.ItemCopying;
			Control.ItemsCopied += Settings.ItemsCopied;
			Control.ItemRenaming += Settings.ItemRenaming;
			Control.ItemRenamed += Settings.ItemRenamed;
			Control.ItemDeleting += Settings.ItemDeleting;
			Control.ItemsDeleted += Settings.ItemsDeleted;
			Control.ItemMoving += Settings.ItemMoving;
			Control.ItemsMoved += Settings.ItemsMoved;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
			Control.DetailsViewCustomColumnDisplayText += Settings.DetailsViewCustomColumnDisplayText;
			Control.DetailsViewCustomColumnHeaderFilterFillItems += Settings.DetailsViewCustomColumnHeaderFilterFillItems;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.SettingsFileList.ThumbnailsViewSettings.ItemTemplate = ContentControlTemplate<FileManagerThumbnailsViewItemTemplateContainer>.Create(
				Settings.SettingsFileList.ThumbnailsViewSettings.ThumbnailViewItemTemplateContent, Settings.SettingsFileList.ThumbnailsViewSettings.ThumbnailViewItemTemplateContentMethod, typeof(FileManagerThumbnailsViewItemTemplateContainer));
			MVCxFileManagerDetailsColumnCollection columns = Settings.SettingsFileList.DetailsViewSettings.Columns;
			for(int i = 0; i < columns.Count; i++) {
				MVCxFileManagerDetailsColumn detailColumn = columns[i] as MVCxFileManagerDetailsColumn;
				if(detailColumn != null)
					Control.SettingsFileList.DetailsViewSettings.Columns[i].ItemTemplate = ContentControlTemplate<FileManagerDetailsViewItemTemplateContainer>.Create(
						detailColumn.ItemTemplateContent, detailColumn.ItemTemplateContentMethod, typeof(FileManagerDetailsViewItemTemplateContainer));
				MVCxFileManagerDetailsCustomColumn customColumn = columns[i] as MVCxFileManagerDetailsCustomColumn;
				if(customColumn != null)
					Control.SettingsFileList.DetailsViewSettings.Columns[i].ItemTemplate = ContentControlTemplate<FileManagerDetailsViewItemTemplateContainer>.Create(
						customColumn.ItemTemplateContent, customColumn.ItemTemplateContentMethod, typeof(FileManagerDetailsViewItemTemplateContainer));
			}
		}
		public FileManagerExtension BindToFolder(string rootFolder) {
			PhysicalFileSystemProvider fileSystemProvider = CreateFileSystemProvider(rootFolder);
			return BindToFileSystemProvider(fileSystemProvider);
		}
		public FileManagerExtension BindToFileSystemProvider(FileSystemProviderBase fileSystemProvider) {
			Control.Settings.RootFolder = fileSystemProvider.RootFolder;
			MVCxDataSourceFileSystemProvider dsProvider = fileSystemProvider as MVCxDataSourceFileSystemProvider;
			if(dsProvider != null && dsProvider.DataSource != null)
				dsProvider.DataHelper = new DataHelperCore(Control, dsProvider.DataSource, string.Empty, string.Empty);
			Control.CreateRestrictedAccessFileSystemProvider(fileSystemProvider);
			return this;
		}
		#region Obsolete download methods
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the DownloadFiles(settings) method instead.")]
		public static FileStreamResult DownloadFiles(string name, FileManagerSettingsPermissions settingsPermissions) {
			return DownloadFiles(name, string.Empty, settingsPermissions, null);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the DownloadFiles(settings, rootFolder) method instead.")]
		public static FileStreamResult DownloadFiles(string name, string rootFolder, FileManagerSettingsPermissions settingsPermissions) {
			return DownloadFiles(name, rootFolder, settingsPermissions, null);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the DownloadFiles(settings, fileSystemProvider) method instead.")]
		public static FileStreamResult DownloadFiles(string name, FileSystemProviderBase fileSystemProvider, FileManagerSettingsPermissions settingsPermissions) {
			var settings = new FileManagerSettings { Name = name };
			settings.SettingsPermissions.Assign(settingsPermissions);
			return DownloadFiles(settings, fileSystemProvider);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the DownloadFiles(settings, rootFolder, fileDownloadingDelegate) method instead.")]
		public static FileStreamResult DownloadFiles(string name, string rootFolder, FileManagerSettingsPermissions settingsPermissions,
			FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			var settings = new FileManagerSettings();
			settings.SettingsPermissions.Assign(settingsPermissions);
			return DownloadFiles(settings, CreateFileSystemProvider(rootFolder), fileDownloadingDelegate);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the DownloadFiles(settings, fileSystemProvider, fileDownloadingDelegate) method instead.")]
		public static FileStreamResult DownloadFiles(string name, FileSystemProviderBase fileSystemProvider, FileManagerSettingsPermissions settingsPermissions,
			FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			var settings = new FileManagerSettings() { Name = name };
			settings.SettingsPermissions.Assign(settingsPermissions);
			return DownloadFiles(settings, fileSystemProvider, fileDownloadingDelegate);
		}
		#endregion
		public static FileStreamResult DownloadFiles(string name, string rootFolder) {
			return DownloadFiles(name, rootFolder, (FileManagerFileDownloadingEventHandler)null);
		}
		public static FileStreamResult DownloadFiles(string name, string rootFolder, FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			return DownloadFiles(name, CreateFileSystemProvider(rootFolder), fileDownloadingDelegate);
		}
		public static FileStreamResult DownloadFiles(string name, FileSystemProviderBase fileSystemProvider) {
			return DownloadFiles(name, fileSystemProvider, (FileManagerFileDownloadingEventHandler)null);
		}
		public static FileStreamResult DownloadFiles(string name, FileSystemProviderBase fileSystemProvider, FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			return DownloadFiles(new FileManagerSettings { Name = name }, fileSystemProvider, fileDownloadingDelegate);
		}
		public static FileStreamResult DownloadFiles(FileManagerSettings settings) {
			return DownloadFiles(settings, string.Empty);
		}
		public static FileStreamResult DownloadFiles(FileManagerSettings settings, string rootFolder) {
			return DownloadFiles(settings, rootFolder, null);
		}
		public static FileStreamResult DownloadFiles(FileManagerSettings settings, string rootFolder, FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			return DownloadFiles(settings, CreateFileSystemProvider(rootFolder), fileDownloadingDelegate);
		}
		public static FileStreamResult DownloadFiles(FileManagerSettings settings, FileSystemProviderBase fileSystemProvider) {
			return DownloadFiles(settings, fileSystemProvider, null);
		}
		public static FileStreamResult DownloadFiles(FileManagerSettings settings, FileSystemProviderBase fileSystemProvider, FileManagerFileDownloadingEventHandler fileDownloadingDelegate) {
			var extension = new FileManagerExtension(settings);
			if(fileDownloadingDelegate != null)
				extension.Control.FileDownloading += fileDownloadingDelegate;
			if(fileSystemProvider != null)
				extension.BindToFileSystemProvider(fileSystemProvider);
			try {
				extension.PrepareControl();
				extension.LoadPostData();
				return CreateDownloadCommand(extension).GetFileStreamResult();
			}
			finally {
				extension.DisposeControl();
			}
		}
		public static string GetCloudDownloadUrl(FileManagerSettings settings, CloudFileSystemProviderBase cloudFileSystemProvider) {
			var extension = new FileManagerExtension(settings);
			if(cloudFileSystemProvider != null)
				extension.BindToFileSystemProvider(cloudFileSystemProvider);
			try {
				extension.PrepareControl();
				extension.LoadPostData();
				return CreateDownloadCommand(extension).GetCloudDownloadUrl();
			}
			finally {
				extension.DisposeControl();
			}
		}
		static MVCxFileManagerDownloadCommand CreateDownloadCommand(FileManagerExtension extension) {
			string downloadArgument = extension.PostDataCollection[DownloadCommandArgument];
			int separatorPos = downloadArgument.IndexOf(FileManagerCommandsHelper.ArgumentSeparator);
			string commandArgs = separatorPos > -1 ? downloadArgument.Substring(separatorPos + 1) : string.Empty;
			return new MVCxFileManagerDownloadCommand(extension.Control, commandArgs);
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			RenderUtils.LoadPostDataRecursive(Control, PostDataCollection);
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxFileManager();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			string uploadControlID = PostDataCollection[RenderUtils.UploadingCallbackQueryParamName] ??
				PostDataCollection[RenderUtils.HelperUploadingCallbackQueryParamName];
			if(!string.IsNullOrEmpty(uploadControlID)) {
				LoadPostData();
				MVCxFileManagerUploadControl uploadControl = (MVCxFileManagerUploadControl)Control.Helper.UploadControl;
				uploadControl.EnsureUploaded();
			}
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		static PhysicalFileSystemProvider CreateFileSystemProvider(string rootFolder) {
			return new PhysicalFileSystemProvider(rootFolder ?? string.Empty);
		}
	}
}
