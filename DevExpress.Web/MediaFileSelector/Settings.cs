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
using System.Web.UI;
namespace DevExpress.Web.Internal {
	public enum MediaFileSelectorPreviewType {
		NotSpecified = -1,
		Audio = 1,
		Object = 2,
		Image = 3,
		Video = 4
	}
	public class MediaFileSelectorSettings : PropertiesBase {
		protected MediaFileSelector MediaFileSelector { get { return (MediaFileSelector)Owner; } }
		public MediaFileSelectorSettings(IPropertiesOwner owner)
			: base(owner) {
			UploadValidationSettings = new UploadControlValidationSettings(owner);
			FileManagerCommonSettings = new FileManagerSettings(owner);
			FileManagerEditingSettings = new FileManagerSettingsEditing(owner);
			FileManagerFoldersSettings = new FileManagerSettingsFolders(owner);
			FileManagerToolbarSettings = new FileManagerSettingsToolbar(owner);
			FileManagerUploadSettings = new FileManagerSettingsUpload(owner);
			FileManagerPermissionSettings = new FileManagerSettingsPermissions(owner);
			FileManagerFileListSettings = new FileManagerSettingsFileList(owner);
			FileManagerBreadcrumbsSettings = new FileManagerSettingsBreadcrumbs(owner);
			FileManagerSettingsAmazon = new FileManagerAmazonProviderSettings(owner);
			FileManagerSettingsDropbox = new FileManagerDropBoxProviderSettings(owner);
			FileManagerSettingsAzure = new FileManagerAzureProviderSettings(owner);
			FileManagerSettingsDataSource = new FileManagerSettingsDataSource(owner);
			FileManagerClientSideEvents = new FileManagerClientSideEvents();
		}
		public bool UseAbsoluteUrls {
			get { return GetBoolProperty("UseAbsoluteUrls", false); }
			set {
				SetBoolProperty("UseAbsoluteUrls", false, value);
			}
		}
		public bool AllowUploadTab {
			get { return GetBoolProperty("AllowUploadTab", true); }
			set { 
				SetBoolProperty("AllowUploadTab", true, value); 
				MediaFileSelector.LayoutChanged(); 
			}
		}
		public bool AllowGalleryTab {
			get { return GetBoolProperty("AllowGalleryTab", true); }
			set {
				SetBoolProperty("AllowGalleryTab", true, value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public bool AllowURLTab {
			get { return GetBoolProperty("AllowURLTab", true); }
			set {
				SetBoolProperty("AllowURLTab", true, value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public bool ShowSaveToServerCheckBox {
			get { return GetBoolProperty("ShowSaveToServerCheckBox", true); }
			set {
				SetBoolProperty("ShowSaveToServerCheckBox", true, value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public MediaFileSelectorPreviewType PreviewType {
			get { return (MediaFileSelectorPreviewType)GetEnumProperty("PreviewType", MediaFileSelectorPreviewType.NotSpecified); }
			set { SetEnumProperty("PreviewType", MediaFileSelectorPreviewType.NotSpecified, value); }
		}
		public string UploadFolder {
			get { return GetStringProperty("UploadFolder", "~/"); }
			set { SetStringProperty("UploadFolder", "~/", value); }
		}
		public string UploadFolderUrlPath {
			get { return GetStringProperty("UploadFolderUrlPath", ""); }
			set { SetStringProperty("UploadFolderUrlPath", "", value); }
		}
		public string PreviewText {
			get { return GetStringProperty("PreviewText", "Preview"); }
			set { SetStringProperty("PreviewText", "Preview", value); }
		}
		public string AllowedFileExtensionsText {
			get { return GetStringProperty("AllowedFileExtensionsText", "Allowed file types"); }
			set { SetStringProperty("AllowedFileExtensionsText", "Allowed file types", value); }
		}
		public string MaximumUploadFileSizeText  {
			get { return GetStringProperty("MaximumUploadFileSizeText", "Maximum upload file size"); }
			set { SetStringProperty("MaximumUploadFileSizeText", "Maximum upload file size", value); }
		}
		public string PreviewUploadTipText {
			get { return GetStringProperty("PreviewUploadTipText", ""); }
			set { SetStringProperty("PreviewUploadTipText", "", value); }
		}
		public string UploadTabText {
			get { return GetStringProperty("UploadTabText", "From local computer"); }
			set {
				SetStringProperty("UploadTabText", "From local computer", value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public string GalleryTabText {
			get { return GetStringProperty("GalleryTabText", "From the gallery"); }
			set {
				SetStringProperty("GalleryTabText", "From the gallery", value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public string URLTabText {
			get { return GetStringProperty("URLTabText", "From URL"); }
			set {
				SetStringProperty("URLTabText", "From URL", value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public string SaveToServerText {
			get { return GetStringProperty("SaveToServerText", "Save to server"); }
			set {
				SetStringProperty("SaveToServerText", "Save to server", value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public string URLTabRegularExpression {
			get { return GetStringProperty("URLTabRegularExpression", ""); }
			set { SetStringProperty("URLTabRegularExpression", "", value); }
		}
		public string URLTabRegularExpressionErrorText {
			get { return GetStringProperty("URLTabRegularExpressionErrorText", ""); }
			set { SetStringProperty("URLTabRegularExpressionErrorText", "", value); }
		}
		public string URLTabNullText {
			get { return GetStringProperty("URLTabNullText", ""); }
			set { SetStringProperty("URLTabNullText", "", value); }
		}
		public string URLTabRequiredErrorText {
			get { return GetStringProperty("URLTabRequiredErrorText", "Required"); }
			set { SetStringProperty("URLTabRequiredErrorText", "Required", value); }
		}
		public string UploadTabRequiredErrorText {
			get { return GetStringProperty("UploadTabRequiredErrorText", "Required"); }
			set { SetStringProperty("UploadTabRequiredErrorText", "Required", value); }
		}
		public string GalleryTabRequiredErrorText {
			get { return GetStringProperty("GalleryTabRequiredErrorText", "Required"); }
			set { SetStringProperty("GalleryTabRequiredErrorText", "Required", value); }
		}
		public UploadControlUploadMode UploadMode {
			get { return (UploadControlUploadMode)GetEnumProperty("UploadMode", UploadControlUploadMode.Auto); }
			set { SetEnumProperty("UploadMode", UploadControlUploadMode.Auto, value); }
		}
		public string FileManagerRootFolderUrlPath {
			get { return GetStringProperty("FileManagerRootFolderUrlPath", "~/"); }
			set { SetStringProperty("FileManagerRootFolderUrlPath", "~/", value); }
		}
		public string AdvancedUploadModeTemporaryFolder {
			get { return GetStringProperty("AdvancedUploadModeTemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder); }
			set { SetStringProperty("AdvancedUploadModeTemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder, value); }
		}
		public int AdvancedUploadModePacketSize {
			get { return GetIntProperty("AdvancedUploadModePacketSize", ASPxUploadControl.DefaultPacketSizeValue); }
			set { SetIntProperty("AdvancedUploadModePacketSize", ASPxUploadControl.DefaultPacketSizeValue, value); }
		}
		public FileManagerProviderType FileManagerProviderType {
			get { return (FileManagerProviderType)GetEnumProperty("FileManagerProviderType", FileManagerProviderType.NotSet); }
			set {
				SetEnumProperty("FileManagerProviderType", FileManagerProviderType.NotSet, value);
				MediaFileSelector.LayoutChanged();
			}
		}
		public string FileManagerCustomFileSystemProviderTypeName {
			get { return GetStringProperty("FileManagerCustomFileSystemProviderTypeName", string.Empty); }
			set {
				SetStringProperty("FileManagerCustomFileSystemProviderTypeName", string.Empty, value);
				MediaFileSelector.LayoutChanged();
			}
		}
		FileSystemProviderBase fileManagerCustomFileSystemProvider;
		public FileSystemProviderBase FileManagerCustomFileSystemProvider {
			get { return fileManagerCustomFileSystemProvider; }
			set {
				fileManagerCustomFileSystemProvider = value;
				MediaFileSelector.LayoutChanged();
			}
		}
		public FileManagerThumbnailCreateEventHandler FileManagerCustomThumbnail { get; set; }
		public FileManagerFolderCreateEventHandler FileManagerFolderCreating { get; set; }
		public FileManagerItemDeleteEventHandler FileManagerItemDeleting { get; set; }
		public FileManagerItemMoveEventHandler FileManagerItemMoving { get; set; }
		public FileManagerItemRenameEventHandler FileManagerItemRenaming { get; set; }
		public FileManagerItemCopyEventHandler FileManagerItemCopying { get; set; }
		public FileManagerFileUploadEventHandler FileManagerFileUploading { get; set; }
		public UploadControlValidationSettings UploadValidationSettings { get; private set; }
		public FileManagerSettings FileManagerCommonSettings { get; private set; }
		public FileManagerSettingsEditing FileManagerEditingSettings { get; private set; }
		public FileManagerSettingsFolders FileManagerFoldersSettings { get; private set; }
		public FileManagerSettingsToolbar FileManagerToolbarSettings { get; private set; }
		public FileManagerSettingsUpload FileManagerUploadSettings { get; private set; }
		public FileManagerSettingsPermissions FileManagerPermissionSettings { get; private set; }
		public FileManagerSettingsFileList FileManagerFileListSettings { get; private set; }
		public FileManagerSettingsBreadcrumbs FileManagerBreadcrumbsSettings { get; private set; }
		public FileManagerAmazonProviderSettings FileManagerSettingsAmazon { get; private set; }
		public FileManagerAzureProviderSettings FileManagerSettingsAzure { get; private set; }
		public FileManagerDropBoxProviderSettings FileManagerSettingsDropbox { get; private set; }
		public FileManagerSettingsDataSource FileManagerSettingsDataSource { get; private set; }
		public FileManagerClientSideEvents FileManagerClientSideEvents { get; private set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			MediaFileSelectorSettings src = source as MediaFileSelectorSettings;
			if(src != null) {
				AdvancedUploadModePacketSize = src.AdvancedUploadModePacketSize;
				AdvancedUploadModeTemporaryFolder = src.AdvancedUploadModeTemporaryFolder;
				UseAbsoluteUrls = src.UseAbsoluteUrls;
				AllowUploadTab = src.AllowUploadTab;
				AllowGalleryTab = src.AllowGalleryTab;
				AllowURLTab = src.AllowURLTab;
				PreviewType = src.PreviewType;
				UploadTabText = src.UploadTabText;
				GalleryTabText = src.GalleryTabText;
				URLTabText = src.URLTabText;
				SaveToServerText = src.SaveToServerText;
				UploadFolder = src.UploadFolder;
				UploadFolderUrlPath = src.UploadFolderUrlPath;
				URLTabRegularExpression = src.URLTabRegularExpression;
				URLTabRequiredErrorText = src.URLTabRequiredErrorText;
				UploadTabRequiredErrorText = src.UploadTabRequiredErrorText;
				GalleryTabRequiredErrorText = src.GalleryTabRequiredErrorText;
				ShowSaveToServerCheckBox = src.ShowSaveToServerCheckBox;
				PreviewText = src.PreviewText;
				AllowedFileExtensionsText = src.AllowedFileExtensionsText;
				MaximumUploadFileSizeText = src.MaximumUploadFileSizeText;
				PreviewUploadTipText = src.PreviewUploadTipText;
				URLTabRegularExpressionErrorText = src.URLTabRegularExpressionErrorText;
				URLTabNullText = src.URLTabNullText;
				UploadMode = src.UploadMode;
				FileManagerRootFolderUrlPath = src.FileManagerRootFolderUrlPath;
				FileManagerCustomThumbnail = src.FileManagerCustomThumbnail;
				FileManagerFolderCreating = src.FileManagerFolderCreating;
				FileManagerItemDeleting = src.FileManagerItemDeleting;
				FileManagerItemMoving = src.FileManagerItemMoving;
				FileManagerItemRenaming = src.FileManagerItemRenaming;
				FileManagerFileUploading = src.FileManagerFileUploading;
				FileManagerItemCopying = src.FileManagerItemCopying;
				UploadValidationSettings.Assign(src.UploadValidationSettings);
				FileManagerClientSideEvents.Assign(src.FileManagerClientSideEvents);
				FileManagerCommonSettings.Assign(src.FileManagerCommonSettings);
				FileManagerEditingSettings.Assign(src.FileManagerEditingSettings);
				FileManagerFoldersSettings.Assign(src.FileManagerFoldersSettings);
				FileManagerToolbarSettings.Assign(src.FileManagerToolbarSettings);
				FileManagerUploadSettings.Assign(src.FileManagerUploadSettings);
				FileManagerPermissionSettings.Assign(src.FileManagerPermissionSettings);
				FileManagerFileListSettings.Assign(src.FileManagerFileListSettings);
				FileManagerBreadcrumbsSettings.Assign(src.FileManagerBreadcrumbsSettings);
				FileManagerProviderType = src.FileManagerProviderType;
				FileManagerCustomFileSystemProviderTypeName = src.FileManagerCustomFileSystemProviderTypeName;
				FileManagerCustomFileSystemProvider = src.FileManagerCustomFileSystemProvider;
				FileManagerSettingsAmazon.Assign(src.FileManagerSettingsAmazon);
				FileManagerSettingsAzure.Assign(src.FileManagerSettingsAzure);
				FileManagerSettingsDropbox.Assign(src.FileManagerSettingsDropbox);
				FileManagerSettingsDataSource.Assign(src.FileManagerSettingsDataSource);
			}
		}
	}
}
