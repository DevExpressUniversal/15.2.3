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
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Utils;
using DevExpress.Web.Mvc.Internal;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	public class HtmlEditorSettings: SettingsBase {
		public HtmlEditorSettings() {
			CssFiles = new HtmlEditorCssFileCollection();
			CustomDialogs = new MVCxHtmlEditorCustomDialogs();
			ImagesEditors = new EditorImages(null);
			ImagesFileManager = new FileManagerImages(null);
			PartsRoundPanelInternal = new RoundPanelParts(null);
			RenderIFrameForPopupElements = DefaultBoolean.Default;
			Settings = new ASPxHtmlEditorSettings(null);
			SettingsDialogs = new MVCxHtmlEditorDefaultDialogSettings(null);
			SettingsForms = new MVCxHtmlEditorFormsSettings(SettingsDialogs);
			SettingsHtmlEditing = new ASPxHtmlEditorHtmlEditingSettings(null);
			SettingsDialogFormElements = new MVCxHtmlEditorDialogFormElementSettings(SettingsDialogs);
			SettingsLoadingPanel = new ASPxHtmlEditorLoadingPanelSettings(null);
			SettingsResize = new HtmlEditorResizeSettings(null);
			SettingsSpellChecker = new ASPxHtmlEditorSpellCheckerSettings(null);
			SettingsText = new ASPxHtmlEditorTextSettings(null);
			StylesButton = new HtmlEditorButtonStyles(null);
			StylesContextMenu = new HtmlEditorMenuStyles(null);
			StylesDialogForm = new HtmlEditorDialogFormStyles(null);
			StylesEditors = new HtmlEditorEditorStyles(null);
			StylesRoundPanel = new HtmlEditorRoundPanelStyles(null);
			StylesSpellChecker = new HtmlEditorSpellCheckerStyles(null);
			StylesStatusBar = new HtmlEditorStatusBarStyles(null);
			StylesToolbars = new ToolbarsStyles(null);
			StylesFileManager = new HtmlEditorFileManagerStyles(null);
			StylesPasteOptionsBar = new HtmlEditorPasteOptionsBarStyles(null);
			Toolbars = new MVCxHtmlEditorToolbarCollection();
			RibbonTabs = new MVCxHtmlEditorRibbonTabCollection(null);
			RibbonContextTabCategories = new MVCxHtmlEditorRibbonContextTabCategoryCollection(null);
			SettingsValidation = new HtmlEditorValidationSettings(null);
			ContextMenuItems = new HtmlEditorContextMenuItemCollection(null);
			Placeholders = new HtmlEditorPlaceholderCollection(null);
			ClientEnabled = true;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public HtmlEditorView ActiveView { get; set; }
		public object CallbackRouteValues { get; set; }
		public object CustomDataActionRouteValues { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public object ExportRouteValues { get; set; }
		public HtmlEditorClientSideEvents ClientSideEvents { get { return (HtmlEditorClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientEnabled { get; set; }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public HtmlEditorCssFileCollection CssFiles { get; private set; }
		public MVCxHtmlEditorCustomDialogs CustomDialogs { get; private set; }
		public string Html { get; set; }
		[Obsolete("Use the StylesRoundPanel property to specify a background color and background image of a round panel."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RoundPanelParts PartsRoundPanel { get { return PartsRoundPanelInternal; } }
		protected internal RoundPanelParts PartsRoundPanelInternal { get; private set; }
		public DefaultBoolean RenderIFrameForPopupElements { get; set; }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public MVCxHtmlEditorToolbarCollection Toolbars { get; private set; }
		public MVCxHtmlEditorRibbonTabCollection RibbonTabs { get; private set; }
		public MVCxHtmlEditorRibbonContextTabCategoryCollection RibbonContextTabCategories { get; private set; }
		public HtmlEditorContextMenuItemCollection ContextMenuItems { get; private set; }
		public HtmlEditorToolbarMode ToolbarMode { get; set; }
		public string AssociatedRibbonName { get; set; }
		public HtmlEditorPlaceholderCollection Placeholders { get; private set; }
		public ASPxHtmlEditorSettings Settings { get; private set; }
		public MVCxHtmlEditorDefaultDialogSettings SettingsDialogs { get; private set; }
		public MVCxHtmlEditorFormsSettings SettingsForms { get; private set; }
		public ASPxHtmlEditorHtmlEditingSettings SettingsHtmlEditing { get; private set; }
		public ASPxHtmlEditorLoadingPanelSettings SettingsLoadingPanel { get; private set; }
		public HtmlEditorResizeSettings SettingsResize { get; private set; }
		public ASPxHtmlEditorSpellCheckerSettings SettingsSpellChecker { get; private set; }
		public ASPxHtmlEditorTextSettings SettingsText { get; private set; }
		public HtmlEditorValidationSettings SettingsValidation { get; private set; }
		public MVCxHtmlEditorDialogFormElementSettings SettingsDialogFormElements { get; private set; }
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog.SettingsImageUpload property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorImageUploadSettings SettingsImageUpload {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageUpload; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog.SettingsAudioUpload property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorAudioUploadSettings SettingsAudioUpload {
			get { return SettingsDialogs.InsertAudioDialog.SettingsAudioUpload; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog.SettingsFlashUpload property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorFlashUploadSettings SettingsFlashUpload {
			get { return SettingsDialogs.InsertFlashDialog.SettingsFlashUpload; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog.SettingsVideoUpload property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorVideoUploadSettings SettingsVideoUpload {
			get { return SettingsDialogs.InsertVideoDialog.SettingsVideoUpload; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorImageSelectorSettings SettingsImageSelector {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog.SettingsAudioSelector property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorAudioSelectorSettings SettingsAudioSelector {
			get { return SettingsDialogs.InsertAudioDialog.SettingsAudioSelector; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog.SettingsFlashSelector property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorFlashSelectorSettings SettingsFlashSelector {
			get { return SettingsDialogs.InsertFlashDialog.SettingsFlashSelector; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog.SettingsVideoSelector property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorVideoSelectorSettings SettingsVideoSelector {
			get { return SettingsDialogs.InsertVideoDialog.SettingsVideoSelector; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxHtmlEditorDocumentSelectorSettings SettingsDocumentSelector {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector; }
		}
		public HtmlEditorImages Images { get { return (HtmlEditorImages)ImagesInternal; } }
		public EditorImages ImagesEditors { get; private set; }
		public FileManagerImages ImagesFileManager { get; private set; }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public HtmlEditorStyles Styles { get { return (HtmlEditorStyles)StylesInternal; } }
		public HtmlEditorButtonStyles StylesButton { get; private set; }
		public HtmlEditorMenuStyles StylesContextMenu { get; private set; }
		public HtmlEditorDialogFormStyles StylesDialogForm { get; private set; }
		public HtmlEditorEditorStyles StylesEditors { get; private set; }
		public HtmlEditorRoundPanelStyles StylesRoundPanel { get; private set; }
		public HtmlEditorSpellCheckerStyles StylesSpellChecker { get; private set; }
		public HtmlEditorStatusBarStyles StylesStatusBar { get; private set; }
		public HtmlEditorFileManagerStyles StylesFileManager { get; private set; }
		public ToolbarsStyles StylesToolbars { get; private set; }
		public HtmlEditorPasteOptionsBarStyles StylesPasteOptionsBar { get; private set; }
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector.FolderCreating property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerFolderCreateEventHandler ImageSelectorFolderCreating {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector.FolderCreating; }
			set { SettingsDialogs.InsertImageDialog.SettingsImageSelector.FolderCreating = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemRenaming property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemRenameEventHandler ImageSelectorItemRenaming {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemRenaming; }
			set { SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemRenaming = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemDeleteEventHandler ImageSelectorItemDeleting {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting; }
			set { SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemDeleting property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemMoveEventHandler ImageSelectorItemMoving {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemMoving; }
			set { SettingsDialogs.InsertImageDialog.SettingsImageSelector.ItemMoving = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileUploading property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerFileUploadEventHandler ImageSelectorFileUploading {
			get { return SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileUploading; }
			set { SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileUploading = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FolderCreating property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerFolderCreateEventHandler DocumentSelectorFolderCreating {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FolderCreating; }
			set { SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FolderCreating = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemRenaming property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemRenameEventHandler DocumentSelectorItemRenaming {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemRenaming; }
			set { SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemRenaming = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemDeleting property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemDeleteEventHandler DocumentSelectorItemDeleting {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemDeleting; }
			set { SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemDeleting = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemMoving property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerItemMoveEventHandler DocumentSelectorItemMoving {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemMoving; }
			set { SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ItemMoving = value; }
		}
		[Obsolete("Use the SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileUploading property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FileManagerFileUploadEventHandler DocumentSelectorFileUploading {
			get { return SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileUploading; }
			set { SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileUploading = value; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new HtmlEditorClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override ImagesBase CreateImages() {
			return new HtmlEditorImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new HtmlEditorStyles(null);
		}
	}
	public class MVCxHtmlEditorDefaultDialogSettings : HtmlEditorDefaultDialogSettings {
		public MVCxHtmlEditorDefaultDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public new MVCxHtmlEditorInsertImageDialogSettings InsertImageDialog {
			get { return (MVCxHtmlEditorInsertImageDialogSettings)base.InsertImageDialog; } 
		}
		public new MVCxHtmlEditorInsertAudioDialogSettings InsertAudioDialog {
			get { return (MVCxHtmlEditorInsertAudioDialogSettings)base.InsertAudioDialog; }
		}
		public new MVCxHtmlEditorInsertFlashDialogSettings InsertFlashDialog {
			get { return (MVCxHtmlEditorInsertFlashDialogSettings)base.InsertFlashDialog; }
		}
		public new MVCxHtmlEditorInsertVideoDialogSettings InsertVideoDialog {
			get { return (MVCxHtmlEditorInsertVideoDialogSettings)base.InsertVideoDialog; }
		}
		public new MVCxHtmlEditorInsertMediaDialogSettingsBase InsertYouTubeVideoDialog {
			get { return (MVCxHtmlEditorInsertMediaDialogSettingsBase)base.InsertYouTubeVideoDialog; }
		}
		public new MVCxHtmlEditorInsertLinkDialogSettings InsertLinkDialog {
			get { return (MVCxHtmlEditorInsertLinkDialogSettings)base.InsertLinkDialog; }
		}
		public new MVCxHtmlEditorInsertTableDialogSettings InsertTableDialog {
			get { return (MVCxHtmlEditorInsertTableDialogSettings)base.InsertTableDialog; }
		}
		public new MVCxHtmlEditorPasteFromWordDialogSettings PasteFromWordDialog {
			get { return (MVCxHtmlEditorPasteFromWordDialogSettings)base.PasteFromWordDialog; }
		}
		public new MVCxHtmlEditorChangeElementPropertiesDialogSettings ChangeElementPropertiesDialog {
			get { return (MVCxHtmlEditorChangeElementPropertiesDialogSettings)base.ChangeElementPropertiesDialog; }
		}
		public new MVCxHtmlEditorInsertPlaceholderDialogSettings InsertPlaceholderDialog {
			get { return (MVCxHtmlEditorInsertPlaceholderDialogSettings)base.InsertPlaceholderDialog; }
		}
		protected override HtmlEditorInsertImageDialogSettings CreateInsertImageDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertImageDialogSettings(owner);
		}
		protected override HtmlEditorInsertAudioDialogSettings CreateInsertAudioDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertAudioDialogSettings(owner);
		}
		protected override HtmlEditorInsertFlashDialogSettings CreateInsertFlashDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertFlashDialogSettings(owner);
		}
		protected override HtmlEditorInsertVideoDialogSettings CreateInsertVideoDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertVideoDialogSettings(owner);
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase CreateInsertYouTubeVideoDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertMediaDialogSettingsBase(owner);
		}
		protected override HtmlEditorInsertLinkDialogSettings CreateInsertLinkDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertLinkDialogSettings(owner);
		}
		protected override HtmlEditorInsertTableDialogSettings CreateInsertTableDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertTableDialogSettings();
		}
		protected override HtmlEditorPasteFromWordDialogSettings CreatePasteFromWordDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorPasteFromWordDialogSettings();
		}
		protected override HtmlEditorDialogSettingsBase CreateInsertPlaceholderDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorInsertPlaceholderDialogSettings();
		}
		protected override HtmlEditorChangeElementPropertiestDialogSettings CreateChangeElementPropertiesDialogSettings(IPropertiesOwner owner) {
			return new MVCxHtmlEditorChangeElementPropertiesDialogSettings();
		}
	}
	public class MVCxHtmlEditorDialogFormElementSettings: HtmlEditorDialogFormElementSettings {
		MVCxHtmlEditorDefaultDialogSettings defaultSettingsDialogs;
		public MVCxHtmlEditorDialogFormElementSettings()
			: base(null) {
		}
		public MVCxHtmlEditorDialogFormElementSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public MVCxHtmlEditorDialogFormElementSettings(MVCxHtmlEditorDefaultDialogSettings defaultSettingsDialogs)
			: base(null) {
				this.defaultSettingsDialogs = defaultSettingsDialogs;
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertImageDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertImageDialogSettings InsertImageDialog {
			get { return (MVCxHtmlEditorInsertImageDialogSettings)GetDefaultDialogsSettings().InsertImageDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertAudioDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertAudioDialogSettings InsertAudioDialog {
			get { return (MVCxHtmlEditorInsertAudioDialogSettings)GetDefaultDialogsSettings().InsertAudioDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertFlashDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertFlashDialogSettings InsertFlashDialog {
			get { return (MVCxHtmlEditorInsertFlashDialogSettings)GetDefaultDialogsSettings().InsertFlashDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertVideoDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertVideoDialogSettings InsertVideoDialog {
			get { return (MVCxHtmlEditorInsertVideoDialogSettings)GetDefaultDialogsSettings().InsertVideoDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertYouTubeVideoDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertMediaDialogSettingsBase InsertYouTubeVideoDialog {
			get { return (MVCxHtmlEditorInsertMediaDialogSettingsBase)GetDefaultDialogsSettings().InsertYouTubeVideoDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertLinkDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertLinkDialogSettings InsertLinkDialog {
			get { return (MVCxHtmlEditorInsertLinkDialogSettings)GetDefaultDialogsSettings().InsertLinkDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.InsertTableDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorInsertTableDialogSettings InsertTableDialog {
			get { return (MVCxHtmlEditorInsertTableDialogSettings)GetDefaultDialogsSettings().InsertTableDialog; }
		}
		[Obsolete("This property is now obsolete. Use the SettingsDialogs.PasteFromWordDialog property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new MVCxHtmlEditorPasteFromWordDialogSettings PasteFromWordDialog {
			get { return (MVCxHtmlEditorPasteFromWordDialogSettings)GetDefaultDialogsSettings().PasteFromWordDialog; }
		}
		protected override HtmlEditorDefaultDialogSettings GetDefaultDialogsSettings() {
			return this.defaultSettingsDialogs ?? base.GetDefaultDialogsSettings();
		}
	}
	public class MVCxHtmlEditorInsertMediaDialogSettingsBase : HtmlEditorInsertMediaDialogSettingsBase, IMediaDialog {
		public MVCxHtmlEditorInsertMediaDialogSettingsBase()
			: base(null) {
		}
		public MVCxHtmlEditorInsertMediaDialogSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
	}
	public class MVCxHtmlEditorInsertMediaDialogSettings : HtmlEditorInsertMediaDialogSettings, IMediaDialog {
		public MVCxHtmlEditorInsertMediaDialogSettings()
			: base(null) {
		}
		public MVCxHtmlEditorInsertMediaDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
	}
	public class MVCxHtmlEditorInsertFlashDialogSettings : HtmlEditorInsertFlashDialogSettings, IMediaDialog {
		public MVCxHtmlEditorInsertFlashDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
		public new MVCxHtmlEditorFlashUploadSettings SettingsFlashUpload {
			get { return (MVCxHtmlEditorFlashUploadSettings)base.SettingsFlashUpload; }
		}
		public new MVCxHtmlEditorFlashSelectorSettings SettingsFlashSelector {
			get { return (MVCxHtmlEditorFlashSelectorSettings)base.SettingsFlashSelector; }
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new MVCxHtmlEditorFlashUploadSettings(Owner, SettingsFlashSelector);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new MVCxHtmlEditorFlashSelectorSettings(Owner);
		}
	}
	public class MVCxHtmlEditorInsertVideoDialogSettings : HtmlEditorInsertVideoDialogSettings, IMediaDialog {
		public MVCxHtmlEditorInsertVideoDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
		public new MVCxHtmlEditorVideoUploadSettings SettingsVideoUpload {
			get { return (MVCxHtmlEditorVideoUploadSettings)base.SettingsVideoUpload; }
		}
		public new MVCxHtmlEditorVideoSelectorSettings SettingsVideoSelector {
			get { return (MVCxHtmlEditorVideoSelectorSettings)base.SettingsVideoSelector; }
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new MVCxHtmlEditorVideoUploadSettings(Owner, SettingsVideoSelector);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new MVCxHtmlEditorVideoSelectorSettings(Owner);
		}
	}
	public class MVCxHtmlEditorInsertAudioDialogSettings : HtmlEditorInsertAudioDialogSettings, IMediaDialog {
		public MVCxHtmlEditorInsertAudioDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
		public new MVCxHtmlEditorAudioUploadSettings SettingsAudioUpload {
			get { return (MVCxHtmlEditorAudioUploadSettings)base.SettingsAudioUpload; }
		}
		public new MVCxHtmlEditorAudioSelectorSettings SettingsAudioSelector {
			get { return (MVCxHtmlEditorAudioSelectorSettings)base.SettingsAudioSelector; }
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new MVCxHtmlEditorAudioUploadSettings(Owner, SettingsAudioSelector);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new MVCxHtmlEditorAudioSelectorSettings(Owner);
		}
	}
	public class MVCxHtmlEditorInsertImageDialogSettings : HtmlEditorInsertImageDialogSettings, IMediaDialog {
		public MVCxHtmlEditorInsertImageDialogSettings()
			: base(null) {
		}
		public MVCxHtmlEditorInsertImageDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		string IMediaDialog.MoreOptionsSectionTemplateContent { get; set; }
		Action IMediaDialog.MoreOptionsSectionTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((IMediaDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((IMediaDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((IMediaDialog)this).BottomAreaTemplateContent = content;
		}
		public void SetMoreOptionsSectionTemplateContent(Action contentMethod) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContentMethod = contentMethod;
		}
		public void SetMoreOptionsSectionTemplateContent(string content) {
			((IMediaDialog)this).MoreOptionsSectionTemplateContent = content;
		}
		public new MVCxHtmlEditorImageSelectorSettings SettingsImageSelector {
			get { return (MVCxHtmlEditorImageSelectorSettings)base.SettingsImageSelector; }
		}
		public new MVCxHtmlEditorImageUploadSettings SettingsImageUpload {
			get { return (MVCxHtmlEditorImageUploadSettings)base.SettingsImageUpload; }
		}
		protected override ASPxHtmlEditorUploadSettingsBase CreateUploadSettings() {
			return new MVCxHtmlEditorImageUploadSettings(Owner, SettingsImageSelector);
		}
		protected override HtmlEditorSelectorSettings CreateSelectorSettings() {
			return new MVCxHtmlEditorImageSelectorSettings(Owner);
		}
	}
	public class MVCxHtmlEditorInsertLinkDialogSettings : HtmlEditorInsertLinkDialogSettings, ISimpleDialog {
		public MVCxHtmlEditorInsertLinkDialogSettings()
			: base(null) {
		}
		public MVCxHtmlEditorInsertLinkDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((ISimpleDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((ISimpleDialog)this).BottomAreaTemplateContent = content;
		}
		public new MVCxHtmlEditorDocumentSelectorSettings SettingsDocumentSelector {
			get { return (MVCxHtmlEditorDocumentSelectorSettings)base.SettingsDocumentSelector; }
		}
		protected override HtmlEditorDocumentSelectorSettings CreateSettingsDocumentSelector() {
			return new MVCxHtmlEditorDocumentSelectorSettings();
		}
	}
	public class MVCxHtmlEditorInsertTableDialogSettings : HtmlEditorInsertTableDialogSettings, ISimpleDialog {
		public MVCxHtmlEditorInsertTableDialogSettings()
			: base(null) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((ISimpleDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((ISimpleDialog)this).BottomAreaTemplateContent = content;
		}
	}
	public class MVCxHtmlEditorPasteFromWordDialogSettings : HtmlEditorPasteFromWordDialogSettings, ISimpleDialog {
		public MVCxHtmlEditorPasteFromWordDialogSettings()
			: base(null) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((ISimpleDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((ISimpleDialog)this).BottomAreaTemplateContent = content;
		}
	}
	public class MVCxHtmlEditorInsertPlaceholderDialogSettings : HtmlEditorDialogSettingsBase, ISimpleDialog {
		public MVCxHtmlEditorInsertPlaceholderDialogSettings()
			: base(null) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((ISimpleDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((ISimpleDialog)this).BottomAreaTemplateContent = content;
		}
	}
	public class MVCxHtmlEditorChangeElementPropertiesDialogSettings : HtmlEditorChangeElementPropertiestDialogSettings, ISimpleDialog {
		public MVCxHtmlEditorChangeElementPropertiesDialogSettings()
			: base(null) {
		}
		string ISimpleDialog.TopAreaTemplateContent { get; set; }
		Action ISimpleDialog.TopAreaTemplateContentMethod { get; set; }
		string ISimpleDialog.BottomAreaTemplateContent { get; set; }
		Action ISimpleDialog.BottomAreaTemplateContentMethod { get; set; }
		public void SetTopAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).TopAreaTemplateContentMethod = contentMethod;
		}
		public void SetTopAreaTemplateContent(string content) {
			((ISimpleDialog)this).TopAreaTemplateContent = content;
		}
		public void SetBottomAreaTemplateContent(Action contentMethod) {
			((ISimpleDialog)this).BottomAreaTemplateContentMethod = contentMethod;
		}
		public void SetBottomAreaTemplateContent(string content) {
			((ISimpleDialog)this).BottomAreaTemplateContent = content;
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public interface ISimpleDialog {
		string BottomAreaTemplateContent { get; set; }
		Action BottomAreaTemplateContentMethod { get; set; }
		string TopAreaTemplateContent { get; set; }
		Action TopAreaTemplateContentMethod { get; set; }
		ITemplate BottomAreaTemplate { get; set; }
		ITemplate TopAreaTemplate { get; set; }
	}
	public interface IMediaDialog : ISimpleDialog {
		string MoreOptionsSectionTemplateContent { get; set; }
		Action MoreOptionsSectionTemplateContentMethod { get; set; }
		ITemplate MoreOptionsSectionTemplate { get; set; }
	}
}
