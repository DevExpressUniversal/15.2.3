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
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet {
	public enum SaveFileDialogDisplaySectionMode {
		ShowAllSections,
		ShowServerSection,
		ShowDownloadSection
	}
	public class ASPxSpreadsheetSettingsBase : PropertiesBase, IPropertiesOwner {
		public ASPxSpreadsheetSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected ASPxSpreadsheet Spreadsheet {
			get { return (ASPxSpreadsheet)Owner; }
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class SpreadsheetDialogSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetDialogSettings(IPropertiesOwner owner)
			: base(owner) {
			InsertPictureDialog = CreateInsertPictureDialogSettings(owner);
			InsertLinkDialog = CreateInsertLinkDialogSettings(owner);
			SaveFileDialog = CreateSaveFileDialogSettings(owner);
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpreadsheetInsertPictureDialogSettings InsertPictureDialog { get; private set; }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDialogSettingsInsertLinkDialog"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpreadsheetInsertLinkDialogSettings InsertLinkDialog { get; private set; }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDialogSettingsSaveFileDialog"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpreadsheetSaveFileDialogSettings SaveFileDialog { get; private set; }
		protected virtual SpreadsheetInsertPictureDialogSettings CreateInsertPictureDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetInsertPictureDialogSettings(owner);
		}
		protected virtual SpreadsheetInsertLinkDialogSettings CreateInsertLinkDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetInsertLinkDialogSettings(owner);
		}
		protected virtual SpreadsheetSaveFileDialogSettings CreateSaveFileDialogSettings(IPropertiesOwner owner) {
			return new SpreadsheetSaveFileDialogSettings(owner);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settigns = source as SpreadsheetDialogSettings;
				if(settigns != null) {
					InsertPictureDialog.Assign(settigns.InsertPictureDialog);
					InsertLinkDialog.Assign(settigns.InsertLinkDialog);
					SaveFileDialog.Assign(settigns.SaveFileDialog);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				InsertPictureDialog, InsertLinkDialog, SaveFileDialog
			});
		}
	}
	public class SpreadsheetInsertLinkDialogSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetInsertLinkDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowEmailAddressSection {
			get { return GetBoolProperty("ShowEmailAddressSection", true); }
			set { SetBoolProperty("ShowEmailAddressSection", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as SpreadsheetInsertLinkDialogSettings;
				if(settings != null) {
					ShowEmailAddressSection = settings.ShowEmailAddressSection;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class SpreadsheetInsertPictureDialogSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetInsertPictureDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowFileUploadSection {
			get { return GetBoolProperty("ShowFileUploadSection", true); }
			set { SetBoolProperty("ShowFileUploadSection", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as SpreadsheetInsertPictureDialogSettings;
				if(settings != null) {
					ShowFileUploadSection = settings.ShowFileUploadSection;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class SpreadsheetSaveFileDialogSettings : ASPxSpreadsheetSettingsBase {
		public SpreadsheetSaveFileDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[NotifyParentProperty(true), DefaultValue(SaveFileDialogDisplaySectionMode.ShowAllSections)]
		public SaveFileDialogDisplaySectionMode DisplaySectionMode {
			get { return (SaveFileDialogDisplaySectionMode)GetEnumProperty("DisplaySectionMode", SaveFileDialogDisplaySectionMode.ShowAllSections); }
			set {
				SetEnumProperty("DisplaySectionMode", SaveFileDialogDisplaySectionMode.ShowAllSections, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settings = source as SpreadsheetSaveFileDialogSettings;
				if(settings != null) {
					DisplaySectionMode = settings.DisplaySectionMode;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxSpreadsheetLoadingPanelSettings : SettingsLoadingPanel {
		public ASPxSpreadsheetLoadingPanelSettings(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
	}
	public class SpreadsheetDocumentSelectorSettings : ASPxSpreadsheetSettingsBase {
		SpreadsheetFileManagerCommonSettings commonSettings;
		SpreadsheetFileManagerEditingSettings editingSettings;
		SpreadsheetFileManagerFoldersSettings foldersSettings;
		FileManagerSettingsToolbar toolbarSettings;
		SpreadsheetFileManagerUploadSettings uploadSettings;
		FileManagerSettingsPermissions permissionSettings;
		FileManagerSettingsFileList fileListSettings;
		FileManagerAmazonProviderSettings amazonSettings;
		FileManagerAzureProviderSettings azureSettings;
		FileManagerDropBoxProviderSettings dropboxSettings;
		FileManagerSettingsDataSource settingsDataSource;
		public SpreadsheetDocumentSelectorSettings(IPropertiesOwner owner)
			: base(owner) {
			this.commonSettings = CreateCommonSettings();
			this.editingSettings = CreateEditingSettings();
			this.foldersSettings = CreateFoldersSettings();
			this.toolbarSettings = CreateToolbarSettings();
			this.uploadSettings = CreateUploadSettings();
			this.permissionSettings = CreatePermissionsSettings();
			this.fileListSettings = CreateFileListSettings();
			this.amazonSettings = CreateAmazonProviderSettings();
			this.azureSettings = CreateAzureProviderSettings();
			this.dropboxSettings = CreateDropBoxProviderSettings();
			this.settingsDataSource = CreateDataSourceSettings();
		}
		protected virtual SpreadsheetFileManagerCommonSettings CreateCommonSettings() {
			return new SpreadsheetDocumentSelectorCommonSettings(Owner);
		}
		protected virtual SpreadsheetFileManagerEditingSettings CreateEditingSettings() {
			return new SpreadsheetFileManagerEditingSettings(Owner);
		}
		protected virtual SpreadsheetFileManagerFoldersSettings CreateFoldersSettings() {
			return new SpreadsheetFileManagerFoldersSettings(Owner);
		}
		protected virtual FileManagerSettingsToolbar CreateToolbarSettings() {
			return new FileManagerSettingsToolbar(Owner);
		}
		protected virtual SpreadsheetFileManagerUploadSettings CreateUploadSettings() {
			return new SpreadsheetFileManagerUploadSettings(Owner);
		}
		protected virtual FileManagerSettingsPermissions CreatePermissionsSettings() {
			return new FileManagerSettingsPermissions(Owner);
		}
		protected virtual FileManagerSettingsFileList CreateFileListSettings() {
			return new FileManagerSettingsFileList(Owner);
		}
		protected virtual FileManagerAmazonProviderSettings CreateAmazonProviderSettings() {
			return new FileManagerAmazonProviderSettings(Owner);
		}
		protected virtual FileManagerAzureProviderSettings CreateAzureProviderSettings() {
			return new FileManagerAzureProviderSettings(Owner);
		}
		protected virtual FileManagerDropBoxProviderSettings CreateDropBoxProviderSettings() {
			return new FileManagerDropBoxProviderSettings(Owner);
		}
		protected virtual FileManagerSettingsDataSource CreateDataSourceSettings() {
			return new FileManagerSettingsDataSource(Owner);
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsCommonSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerCommonSettings CommonSettings { get { return commonSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsEditingSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerEditingSettings EditingSettings { get { return editingSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsFoldersSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerFoldersSettings FoldersSettings { get { return foldersSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsToolbarSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsToolbar ToolbarSettings { get { return toolbarSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsUploadSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerUploadSettings UploadSettings { get { return uploadSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsPermissionSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsPermissions PermissionSettings { get { return permissionSettings; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetDocumentSelectorSettingsFileListSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsFileList FileListSettings { get { return fileListSettings; } }
		[Localizable(false), AutoFormatDisable, DefaultValue(""), NotifyParentProperty(true),
		Obsolete("This property is now obsolete. Use the ASPxSpreadsheet.WorkDirectory property instead.")]
		public string RootFolderUrlPath {
			get {
				return GetStringProperty("RootFolderUrlPath", "");
			}
			set {
				if(value == RootFolderUrlPath)
					return;
				SetStringProperty("RootFolderUrlPath", "", value);
			}
		}
		[ DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string CustomCommand {
			get { return GetStringProperty("CustomCommand", ""); }
			set { SetStringProperty("CustomCommand", "", value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				SpreadsheetDocumentSelectorSettings src = source as SpreadsheetDocumentSelectorSettings;
				if(src != null) {
					CommonSettings.Assign(src.CommonSettings);
					EditingSettings.Assign(src.EditingSettings);
					FoldersSettings.Assign(src.FoldersSettings);
					ToolbarSettings.Assign(src.ToolbarSettings);
					UploadSettings.Assign(src.UploadSettings);
					PermissionSettings.Assign(src.PermissionSettings);
					FileListSettings.Assign(src.FileListSettings);
					CustomCommand = src.CustomCommand;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { CommonSettings, EditingSettings, FoldersSettings, ToolbarSettings, UploadSettings, 
				PermissionSettings, FileListSettings};
		}
	}
	public class SpreadsheetFileManagerCommonSettings : FileManagerSettings {
		internal static string[] DefaultAllowedExtensions = new string[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
		public SpreadsheetFileManagerCommonSettings(IPropertiesOwner owner) : base(owner) { }
		public override string[] AllowedFileExtensions {
			get { return base.AllowedFileExtensions; }
			set { base.AllowedFileExtensions = value; }
		}
		protected bool ShouldSerializeAllowedFileExtensions() {
			return !System.Linq.Enumerable.SequenceEqual(AllowedFileExtensions, DefaultAllowedExtensions);
		}
		protected void ResetAllowedFileExtensions() {
			AllowedFileExtensions = DefaultAllowedExtensions;
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return DefaultAllowedExtensions;
		}
		[Browsable(false), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableMultiSelect {
			get { return false; }
		}
		[Browsable(false), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string RootFolder {
			get {
				return base.RootFolder;
			}
			set {
				base.RootFolder = value;
			}
		}
	}
	public class SpreadsheetDocumentSelectorCommonSettings : SpreadsheetFileManagerCommonSettings {
		internal static new string[] DefaultAllowedExtensions = new string[] { ".xlsx", ".xlsm", ".xls", ".xltx", ".xltm", ".xlt", ".txt", ".csv" };
		public SpreadsheetDocumentSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
		public override string[] AllowedFileExtensions {
			get { return base.AllowedFileExtensions; }
			set { base.AllowedFileExtensions = value; }
		}
		protected new bool ShouldSerializeAllowedFileExtensions() {
			return !System.Linq.Enumerable.SequenceEqual(AllowedFileExtensions, DefaultAllowedExtensions);
		}
		protected new void ResetAllowedFileExtensions() {
			AllowedFileExtensions = DefaultAllowedExtensions;
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return DefaultAllowedExtensions;
		}
	}
	public class SpreadsheetFileManagerEditingSettings : FileManagerSettingsEditing {
		public SpreadsheetFileManagerEditingSettings(IPropertiesOwner owner) : base(owner) { }
		protected ASPxSpreadsheet Spreadsheet {
			get { return (ASPxSpreadsheet)Owner; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowDownload {
			get {
				return false;
			}
		}
	}
	public class SpreadsheetFileManagerFoldersSettings : FileManagerSettingsFolders {
		public SpreadsheetFileManagerFoldersSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFileManagerFoldersSettingsShowLockedFolderIcons"),
#endif
		DefaultValue(false)]
		public override bool ShowLockedFolderIcons {
			get { return base.ShowLockedFolderIcons; }
			set { base.ShowLockedFolderIcons = value; }
		}
		protected override bool GetDefaultShowLockedFolderIcons() {
			return false;
		}
	}
	public class SpreadsheetFileManagerValidationSettings : FileManagerValidationSettings {
		const long MaxFileSizeDefaultValue = 30 * 1024 * 1024;
		public SpreadsheetFileManagerValidationSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFileManagerValidationSettingsMaxFileSize"),
#endif
		DefaultValue(MaxFileSizeDefaultValue), AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true)]
		public override long MaxFileSize {
			get { return base.MaxFileSize; }
			set { base.MaxFileSize = value; }
		}
		protected override long GetMaxFileSizeDefaultValue() {
			return MaxFileSizeDefaultValue;
		}
	}
	public class SpreadsheetFileManagerUploadSettings : FileManagerSettingsUpload {
		public SpreadsheetFileManagerUploadSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetFileManagerUploadSettingsEnabled"),
#endif
		DefaultValue(false)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		public new SpreadsheetFileManagerValidationSettings ValidationSettings {
			get { return base.ValidationSettings as SpreadsheetFileManagerValidationSettings; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new SpreadsheetFileManagerValidationSettings(this);
		}
		protected override bool GetDefaultEnabled() {
			return false;
		}
	}
}
