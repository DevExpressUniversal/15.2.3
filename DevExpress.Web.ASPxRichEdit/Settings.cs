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

using DevExpress.Web;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Web.UI;
using System;
namespace DevExpress.Web.ASPxRichEdit {
	public class ASPxRichEditSettingsBase : PropertiesBase, IPropertiesOwner {
		public ASPxRichEditSettingsBase(IPropertiesOwner owner)
			: base(owner) { }
		protected ASPxRichEdit RichEdit { get { return (ASPxRichEdit)Owner; } }
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class ASPxRichEditSettings : ASPxRichEditSettingsBase {
		ASPxRichEditBehaviorSettings behavior;
		ASPxRichEditDocumentCapabilitiesSettings documentCapabilities;
		ASPxRichEditHorizontalRulerSettings horizontalRuler;
		ASPxRichEditBookmarkSettings bookmarks;
		public ASPxRichEditSettings(ASPxRichEdit owner)
			: base(owner) {
			behavior = new ASPxRichEditBehaviorSettings();
			documentCapabilities = new ASPxRichEditDocumentCapabilitiesSettings();
			horizontalRuler = new ASPxRichEditHorizontalRulerSettings();
			bookmarks = new ASPxRichEditBookmarkSettings();
		}
		[Localizable(false), AutoFormatDisable, NotifyParentProperty(true), DefaultValue(RichEditUnit.Inch)]
		public RichEditUnit Unit {
			get { return (RichEditUnit)GetEnumProperty("Unit", RichEditUnit.Inch); }
			set { SetEnumProperty("Unit", RichEditUnit.Inch, value); }
		}
		[Category("Behavior"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxRichEditBehaviorSettings Behavior { get { return behavior; } }
		[Category("Behavior"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxRichEditDocumentCapabilitiesSettings DocumentCapabilities { get { return documentCapabilities; } }
		[Category("Behavior"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxRichEditHorizontalRulerSettings HorizontalRuler { get { return horizontalRuler; } }
		[Category("Behavior"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxRichEditBookmarkSettings Bookmarks { get { return bookmarks; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			ASPxRichEditSettings src = source as ASPxRichEditSettings;
			if(src != null) {
				Unit = src.Unit;
				Behavior.Assign(src.Behavior);
				DocumentCapabilities.Assign(src.DocumentCapabilities);
				HorizontalRuler.Assign(src.HorizontalRuler);
				Bookmarks.Assign(src.Bookmarks);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				((ISettingsWithExternalStateManager)Behavior).StateManager,
				((ISettingsWithExternalStateManager)DocumentCapabilities).StateManager,
				((ISettingsWithExternalStateManager)HorizontalRuler).StateManager,
				((ISettingsWithExternalStateManager)Bookmarks).StateManager,
			});
		}
	}
	public class RichEditDocumentSelectorSettings : ASPxRichEditSettingsBase {
		RichEditFileManagerCommonSettings commonSettings;
		RichEditFileManagerEditingSettings editingSettings;
		RichEditFileManagerFoldersSettings foldersSettings;
		FileManagerSettingsToolbar toolbarSettings;
		RichEditDocumentSelectorUploadSettings uploadSettings;
		FileManagerSettingsPermissions permissionSettings;
		FileManagerSettingsFileList fileListSettings;
		FileManagerAmazonProviderSettings amazonSettings;
		FileManagerAzureProviderSettings azureSettings;
		FileManagerDropBoxProviderSettings dropboxSettings;
		FileManagerSettingsDataSource settingsDataSource;
		public RichEditDocumentSelectorSettings(IPropertiesOwner owner)
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
		protected virtual RichEditFileManagerCommonSettings CreateCommonSettings() {
			return new RichEditDocumentSelectorCommonSettings(Owner);
		}
		protected virtual RichEditFileManagerEditingSettings CreateEditingSettings() {
			return new RichEditFileManagerEditingSettings(Owner);
		}
		protected virtual RichEditFileManagerFoldersSettings CreateFoldersSettings() {
			return new RichEditFileManagerFoldersSettings(Owner);
		}
		protected virtual FileManagerSettingsToolbar CreateToolbarSettings() {
			return new FileManagerSettingsToolbar(Owner);
		}
		protected virtual RichEditDocumentSelectorUploadSettings CreateUploadSettings() {
			return new RichEditDocumentSelectorUploadSettings(Owner);
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
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsCommonSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditFileManagerCommonSettings CommonSettings { get { return commonSettings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsEditingSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditFileManagerEditingSettings EditingSettings { get { return editingSettings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsFoldersSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditFileManagerFoldersSettings FoldersSettings { get { return foldersSettings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsToolbarSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsToolbar ToolbarSettings { get { return toolbarSettings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsUploadSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditDocumentSelectorUploadSettings UploadSettings { get { return uploadSettings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorSettingsPermissionSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsPermissions PermissionSettings { get { return permissionSettings; } }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsFileList FileListSettings { get { return fileListSettings; } }
		[Localizable(false), AutoFormatDisable, DefaultValue(""), NotifyParentProperty(true),
		Obsolete("This property is now obsolete. Use the ASPxRichEdit.WorkDirectory property instead.")]
		public string RootFolderUrlPath {
			get { return GetStringProperty("RootFolderUrlPath", ""); }
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
				RichEditDocumentSelectorSettings src = source as RichEditDocumentSelectorSettings;
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
				PermissionSettings, FileListSettings };
		}
	}
	public class RichEditFileManagerCommonSettings : FileManagerSettings {
		internal static string[] DefaultAllowedExtensions = new string[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
		public RichEditFileManagerCommonSettings(IPropertiesOwner owner) : base(owner) { }
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
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
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
	public class RichEditDocumentSelectorCommonSettings : RichEditFileManagerCommonSettings {
		static new string[] DefaultAllowedExtensions = new string[] { ".doc", ".docx", ".epub", "html", "htm", ".mht", ".mhtml", ".odt", ".txt", ".rtf", "xml" };
		public RichEditDocumentSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
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
	public class RichEditFileManagerEditingSettings : FileManagerSettingsEditing {
		public RichEditFileManagerEditingSettings(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowDownload {
			get { return false; }
		}
	}
	public class RichEditFileManagerFoldersSettings : FileManagerSettingsFolders {
		public RichEditFileManagerFoldersSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditFileManagerFoldersSettingsShowLockedFolderIcons"),
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
	public class RichEditFileManagerValidationSettings : FileManagerValidationSettings {
		const long MaxFileSizeDefaultValue = 30 * 1024 * 1024;
		public RichEditFileManagerValidationSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditFileManagerValidationSettingsMaxFileSize"),
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
	public class RichEditDocumentSelectorUploadSettings : FileManagerSettingsUpload {
		public RichEditDocumentSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("RichEditDocumentSelectorUploadSettingsEnabled"),
#endif
		DefaultValue(false)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		public new RichEditFileManagerValidationSettings ValidationSettings {
			get { return base.ValidationSettings as RichEditFileManagerValidationSettings; }
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new RichEditFileManagerValidationSettings(this);
		}
		protected override bool GetDefaultEnabled() {
			return false;
		}
	}
	public class RichEditDialogSettings : ASPxRichEditSettingsBase {
		public RichEditDialogSettings(IPropertiesOwner owner)
			: base(owner) {
			InsertPictureDialog = CreateInsertPictureDialogSettings(owner);
			InsertLinkDialog = CreateInsertLinkDialogSettings(owner);
			SaveFileDialog = CreateSaveFileDialogSettings(owner);
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public RichEditInsertPictureDialogSettings InsertPictureDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public RichEditInsertLinkDialogSettings InsertLinkDialog { get; private set; }
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public RichEditSaveFileDialogSettings SaveFileDialog { get; private set; }
		protected virtual RichEditInsertPictureDialogSettings CreateInsertPictureDialogSettings(IPropertiesOwner owner) {
			return new RichEditInsertPictureDialogSettings(owner);
		}
		protected virtual RichEditInsertLinkDialogSettings CreateInsertLinkDialogSettings(IPropertiesOwner owner) {
			return new RichEditInsertLinkDialogSettings(owner);
		}
		protected virtual RichEditSaveFileDialogSettings CreateSaveFileDialogSettings(IPropertiesOwner owner) {
			return new RichEditSaveFileDialogSettings(owner);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var settigns = source as RichEditDialogSettings;
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
	public class RichEditInsertLinkDialogSettings : ASPxRichEditSettingsBase {
		public RichEditInsertLinkDialogSettings(IPropertiesOwner owner)
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
				var settings = source as RichEditInsertLinkDialogSettings;
				if(settings != null) {
					ShowEmailAddressSection = settings.ShowEmailAddressSection;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class RichEditInsertPictureDialogSettings : ASPxRichEditSettingsBase {
		public RichEditInsertPictureDialogSettings(IPropertiesOwner owner)
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
				var settings = source as RichEditInsertPictureDialogSettings;
				if(settings != null) {
					ShowFileUploadSection = settings.ShowFileUploadSection;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class RichEditSaveFileDialogSettings : ASPxRichEditSettingsBase {
		public RichEditSaveFileDialogSettings(IPropertiesOwner owner)
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
				var settings = source as RichEditSaveFileDialogSettings;
				if(settings != null) {
					DisplaySectionMode = settings.DisplaySectionMode;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public enum SaveFileDialogDisplaySectionMode {
		ShowAllSections,
		ShowServerSection,
		ShowDownloadSection
	}
}
