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
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxHtmlEditor;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Mvc.Internal;
	using System.ComponentModel;
	public class HtmlEditorExtension: ExtensionBase {
		const char CustomDataCallbackPrefix = 'd';
		static Control callbackResultDummyControl = new Control();
		static Dictionary<string, string> errorTextsStaticObj = new Dictionary<string, string>();
		public HtmlEditorExtension(HtmlEditorSettings settings)
			: base(settings) {
		}
		public HtmlEditorExtension(HtmlEditorSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxHtmlEditor Control {
			get { return (MVCxHtmlEditor)base.Control; }
		}
		protected internal new HtmlEditorSettings Settings {
			get { return (HtmlEditorSettings)base.Settings; }
		}
		internal static Dictionary<string, string> ErrorTexts {
			get {
				if(Context == null) return errorTextsStaticObj;
				return HttpUtils.GetContextObject<Dictionary<string, string>>("DXHtmlEditorsErrorTexts");
			}
		}
		public static RibbonTab[] DefaultRibbonTabs {
			get { return new HtmlEditorDefaultRibbon(null).DefaultRibbonTabs; }
		}
		public static RibbonContextTabCategory[] DefaultRibbonContextTabCategories {
			get { return new HtmlEditorDefaultRibbon(null).DefaultRibbonContextTabCategories; }
		}
		public HtmlEditorExtension Bind(object value) {
			if(value != null)
				Control.Html = value.ToString();
			return this;
		}
		public HtmlEditorExtension Bind(object dataObject, string propertyName) {
			return Bind(ReflectionUtils.GetPropertyValue(dataObject, propertyName));
		}
		public static HtmlEditorView GetActiveView(string name) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "View");
			extension.LoadPostData();
			return extension.Control.ActiveView;
		}
		public static string GetHtml(string name) {
			return GetHtml(name, null);
		}
		public static string GetHtml(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings) {
			bool dummy;
			return GetHtml(name, htmlEditingSettings, null, null, out dummy);
		}
		public static string GetHtml(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings, HtmlEditorValidationSettings validationSettings, out bool isValid) {
			return GetHtml(name, htmlEditingSettings, validationSettings, null, out isValid);
		}
		public static string GetHtml(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings, HtmlEditorValidationSettings validationSettings,
			EventHandler<HtmlEditorValidationEventArgs> validationDelegate, out bool isValid) {
			return GetHtml(name, htmlEditingSettings, validationSettings, validationDelegate, null, out isValid);
		}
		public static string GetHtml(string name, ASPxHtmlEditorHtmlEditingSettings htmlEditingSettings, HtmlEditorValidationSettings validationSettings,
			EventHandler<HtmlEditorValidationEventArgs> validationDelegate, EventHandler<HtmlCorrectingEventArgs> htmlCorrectionDelegate, out bool isValid) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			if(htmlEditingSettings != null)
				extension.Control.SettingsHtmlEditing.Assign(htmlEditingSettings);
			if(validationSettings != null)
				extension.Control.SettingsValidation.Assign(validationSettings);
			if(validationDelegate != null)
				extension.Control.Validation += validationDelegate;
			if(htmlCorrectionDelegate != null)
				extension.Control.HtmlCorrecting += htmlCorrectionDelegate;
			try {
				if(extension.IsPostDataLoaded)
					extension.LoadPostedHtml();
				else
					extension.LoadPostData();
				extension.Control.Validate();
				isValid = extension.Control.IsValid;
				if(!extension.Control.IsValid)
					ErrorTexts[name] = extension.Control.ErrorText;
				else if(ErrorTexts.ContainsKey(name))
					ErrorTexts.Remove(name);
			}
			finally {
				if(validationDelegate != null)
					extension.Control.Validation -= validationDelegate;
			}
			return extension.Control.Html;
		}
		void LoadPostedHtml() {
			Control.ExecuteLoadPostedHtml();
		}
		public static IEnumerable<string> GetCssFiles(string name) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.LoadPostData();
			return extension.Control.CssFiles.ConvertAll<string>(f => f.FilePath);
		}
		public static void SaveUploadedImage(string name, HtmlEditorImageSelectorSettings imageSelectorSettings) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.Control.SettingsDialogs.InsertImageDialog.SettingsImageSelector.Assign(imageSelectorSettings);
			extension.Control.UploadFileToFileManager("insertimagedialog");
		}
		public static void SaveUploadedDocument(string name, HtmlEditorDocumentSelectorSettings documentSelectorSettings) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.Control.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.Assign(documentSelectorSettings);
			extension.Control.UploadFileToFileManager("insertlinkdialog");
		}
		public static void SaveUploadedFlash(string name, HtmlEditorFlashSelectorSettings flashSelectorSettings) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.Control.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.Assign(flashSelectorSettings);
			extension.Control.UploadFileToFileManager("insertflashdialog");
		}
		public static void SaveUploadedVideo(string name, HtmlEditorVideoSelectorSettings videoSelectorSettings) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.Control.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.Assign(videoSelectorSettings);
			extension.Control.UploadFileToFileManager("insertvideodialog");
		}
		public static void SaveUploadedAudio(string name, HtmlEditorAudioSelectorSettings audioSelectorSettings) {
			HtmlEditorExtension extension = (HtmlEditorExtension)ExtensionManager.GetExtension(ExtensionType.HtmlEditor, name, ExtensionCacheMode.Request, "Html");
			extension.Control.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.Assign(audioSelectorSettings);
			extension.Control.UploadFileToFileManager("insertaudiodialog");
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This method is now obsolete. Use the SaveUploadedFile method instead.")]
		public static void SaveUploadedImage(string name, UploadControlValidationSettings validationSettings, string uploadImageFolder) {
			SaveUploadedFile(name, validationSettings, uploadImageFolder, string.Empty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This method is now obsolete. Use the SaveUploadedFile method instead.")]
		public static void SaveUploadedImage(string name, UploadControlValidationSettings validationSettings, string uploadImageFolder, string fileName) {
			SaveUploadedFile(name, validationSettings, uploadImageFolder, fileName);
		}
		public static void SaveUploadedFile(string name, UploadControlValidationSettings validationSettings, string uploadFolder) {
			SaveUploadedFile(name, validationSettings, uploadFolder, string.Empty);
		}
		public static void SaveUploadedFile(string name, UploadControlValidationSettings validationSettings, string uploadFolder, FileSavingEventHandler onFileUploadComplete) {
			SaveUploadedFile(name, validationSettings, uploadFolder, string.Empty, onFileUploadComplete);
		}
		public static void SaveUploadedFile(string name, UploadControlValidationSettings validationSettings, string uploadFolder, string fileName) {
			SaveUploadedFile(name, validationSettings, uploadFolder, fileName, null);
		}
		public static void SaveUploadedFile(string name, UploadControlValidationSettings validationSettings, string uploadFolder, string fileName, FileSavingEventHandler onFileUploadComplete) {
			SaveUploadedFile(name, validationSettings, uploadFolder, "", fileName, onFileUploadComplete);
		}
		public static void SaveUploadedFile(string name, UploadControlValidationSettings validationSettings, string uploadFolder, string uploadFolderUrlPath, string fileName, FileSavingEventHandler onFileUploadComplete) {
			EventHandler<FileUploadCompleteEventArgs> fileUploadComplete = (object sender, FileUploadCompleteEventArgs e) => {
				MvcUrlResolutionService.Initialize();
				ExecuteOnRenderMode(() => {
					MediaFileSelectorMainControl.OnFileUploadComplete(sender, e, onFileUploadComplete, uploadFolder, fileName, uploadFolderUrlPath);
				});
			};
			UploadControlExtension.GetUploadedFiles(GetUploadControlID(), validationSettings, fileUploadComplete);
		}
		static void ExecuteOnRenderMode(Action action, MvcRenderMode renderMode = MvcRenderMode.Render) {
			MvcRenderMode savedRenderMode = MvcUtils.RenderMode;
			try {
				MvcUtils.RenderMode = renderMode;
				action();
			} finally {
				MvcUtils.RenderMode = savedRenderMode;
			}
		}
		protected static string GetUploadControlID() {
			string helperParamName = DevExpress.Web.Internal.RenderUtils.HelperUploadingCallbackQueryParamName;
			string uploadParamName = DevExpress.Web.Internal.RenderUtils.UploadingCallbackQueryParamName;
			return HttpContext.Current.Request.Params[helperParamName] ?? HttpContext.Current.Request.Params[uploadParamName];
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.OnWidthSet(true, Settings.Width);
			Control.ActiveView = Settings.ActiveView;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomDataActionRouteValues = Settings.CustomDataActionRouteValues;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.ExportRouteValues = Settings.ExportRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientEnabled = Settings.ClientEnabled;
			Control.ClientVisible = Settings.ClientVisible;
			Control.CssFiles.Assign(Settings.CssFiles);
			Control.CustomDialogs.Assign(Settings.CustomDialogs);
			Control.Html = Settings.Html;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagesEditors.CopyFrom(Settings.ImagesEditors);
			Control.ImagesFileManager.CopyFrom(Settings.ImagesFileManager);
			Control.PartsRoundPanelInternal.Assign(Settings.PartsRoundPanelInternal);
			Control.RenderIFrameForPopupElements = Settings.RenderIFrameForPopupElements;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.ToolbarMode = Settings.ToolbarMode;
			Control.AssociatedRibbonID = Settings.AssociatedRibbonName;
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsDialogs.Assign(Settings.SettingsDialogs);
			Control.SettingsForms.Assign(Settings.SettingsForms);
			Control.SettingsHtmlEditing.Assign(Settings.SettingsHtmlEditing);
			Control.SettingsDialogs.InsertImageDialog.SettingsImageUpload.Assign(Settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload);
			Control.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload.Assign(Settings.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload);
			Control.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload.Assign(Settings.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload);
			Control.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload.Assign(Settings.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.SettingsResize.Assign(Settings.SettingsResize);
			Control.SettingsSpellChecker.Assign(Settings.SettingsSpellChecker);
			Control.SettingsText.Assign(Settings.SettingsText);
			Control.SettingsValidation.Assign(Settings.SettingsValidation);
			Control.SettingsDialogs.InsertImageDialog.SettingsImageSelector.Assign(Settings.SettingsDialogs.InsertImageDialog.SettingsImageSelector);
			Control.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.Assign(Settings.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector);
			Control.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.Assign(Settings.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector);
			Control.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.Assign(Settings.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector);
			Control.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.Assign(Settings.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector);
#pragma warning disable 618
			Control.SettingsDialogFormElements.Assign(Settings.SettingsDialogFormElements);
#pragma warning restore 618
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesButton.CopyFrom(Settings.StylesButton);
			Control.StylesContextMenu.CopyFrom(Settings.StylesContextMenu);
			Control.StylesDialogForm.CopyFrom(Settings.StylesDialogForm);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesRoundPanel.CopyFrom(Settings.StylesRoundPanel);
			Control.StylesSpellChecker.CopyFrom(Settings.StylesSpellChecker);
			Control.StylesStatusBar.CopyFrom(Settings.StylesStatusBar);
			Control.StylesFileManager.CopyFrom(Settings.StylesFileManager);
			Control.StylesToolbars.CopyFrom(Settings.StylesToolbars);
			Control.StylesPasteOptionsBar.CopyFrom(Settings.StylesPasteOptionsBar);
			Control.ContextMenuItems.Assign(Settings.ContextMenuItems);
			Control.Placeholders.Assign(Settings.Placeholders);
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			if(Settings.Toolbars.CollectionHasBeenChanged)
				Control.Toolbars.Assign(Settings.Toolbars);
			else
				Control.CreateDefaultToolbars(Control.IsRightToLeft());
			if(Settings.RibbonTabs.CollectionHasBeenChanged)
				Control.RibbonTabs.Assign(Settings.RibbonTabs);
			else if(Control.ToolbarMode == HtmlEditorToolbarMode.Ribbon)
				Control.CreateDefaultRibbonTabs(true);
			if(Settings.RibbonContextTabCategories.CollectionHasBeenChanged)
				Control.RibbonContextTabCategories.Assign(Settings.RibbonContextTabCategories);
			else if(Control.ToolbarMode == HtmlEditorToolbarMode.Ribbon)
				Control.CreateDefaultRibbonContextTabCategories(true);
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			CreateDialogTemplates(Control.SettingsDialogs.InsertImageDialog, Settings.SettingsDialogs.InsertImageDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertAudioDialog, Settings.SettingsDialogs.InsertAudioDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertFlashDialog, Settings.SettingsDialogs.InsertFlashDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertVideoDialog, Settings.SettingsDialogs.InsertVideoDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertYouTubeVideoDialog, Settings.SettingsDialogs.InsertYouTubeVideoDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertLinkDialog, Settings.SettingsDialogs.InsertLinkDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertTableDialog, Settings.SettingsDialogs.InsertTableDialog);
			CreateDialogTemplates(Control.SettingsDialogs.PasteFromWordDialog, Settings.SettingsDialogs.PasteFromWordDialog);
			CreateDialogTemplates(Control.SettingsDialogs.ChangeElementPropertiesDialog, Settings.SettingsDialogs.ChangeElementPropertiesDialog);
			CreateDialogTemplates(Control.SettingsDialogs.InsertPlaceholderDialog, Settings.SettingsDialogs.InsertPlaceholderDialog);
			for(var i = 0; i < Settings.RibbonTabs.Count; i++)
				CreateRibbonTemplateItem(Settings.RibbonTabs[i], Control.RibbonTabs[i]);
			for(var i = 0; i < Settings.RibbonContextTabCategories.Count; i++) {
				var sourceTabCategory = Settings.RibbonContextTabCategories[i];
				var destinationTabCategory = Control.RibbonContextTabCategories[i];
				for(var j = 0; j < sourceTabCategory.Tabs.Count; j++)
					CreateRibbonTemplateItem(sourceTabCategory.Tabs[j], destinationTabCategory.Tabs[j]);
			}
		}
		void CreateRibbonTemplateItem(RibbonTab sourceTab, RibbonTab destinationTab) {
			for(var groupIndex = 0; groupIndex < sourceTab.Groups.Count; groupIndex++) {
				var sourceGroup = sourceTab.Groups[groupIndex];
				var destinationGroup = destinationTab.Groups[groupIndex];
				for(var itemIndex = 0; itemIndex < sourceGroup.Items.Count; itemIndex++) {
					var sourceItem = sourceGroup.Items[itemIndex] as MVCxRibbonTemplateItem;
					var destinationItem = destinationGroup.Items[itemIndex] as RibbonTemplateItem;
					if(sourceItem != null && destinationTab != null)
						destinationItem.Template = ContentControlTemplate<RibbonTemplateItemControl>.Create(
							sourceItem.Content, sourceItem.ContentMethod,
							typeof(RibbonTemplateItemControl));
				}
			}
		}
		void CreateDialogTemplates(IMediaDialog controlDialog, IMediaDialog settingsDialog) {
			controlDialog.MoreOptionsSectionTemplate =
				ContentControlTemplate.Create(settingsDialog.MoreOptionsSectionTemplateContent, settingsDialog.MoreOptionsSectionTemplateContentMethod, true);
			CreateDialogTemplates((ISimpleDialog)controlDialog, (ISimpleDialog)settingsDialog);
		}
		void CreateDialogTemplates(ISimpleDialog controlDialog, ISimpleDialog settingsDialog) {
			controlDialog.TopAreaTemplate = ContentControlTemplate.Create(settingsDialog.TopAreaTemplateContent, settingsDialog.TopAreaTemplateContentMethod, true);
			controlDialog.BottomAreaTemplate = ContentControlTemplate.Create(settingsDialog.BottomAreaTemplateContent, settingsDialog.BottomAreaTemplateContentMethod, true);
		}
		protected override Control GetCallbackResultControl() {
			return callbackResultDummyControl;
		}
		protected override void RenderCallbackResultControl() {
			Control.RenderCallbackResultControl();
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			if(!string.IsNullOrEmpty(GetUploadControlID())) {
				string uploadFolder;
				ASPxHtmlEditorUploadValidationSettingsBase validationSettigns;
				if(TryGetUploadSettings(out validationSettigns, out uploadFolder))
					SaveUploadedFile(Control.ID, validationSettigns, uploadFolder);
			}
			Control.RestoreWidth();
			ASPxHtmlEditor editor = Control as ASPxHtmlEditor;
			if(editor != null && ErrorTexts.ContainsKey(editor.ID)) {
				editor.IsValid = false;
				editor.ErrorText = ErrorTexts[editor.ID];
			}
		}
		protected internal bool TryGetUploadSettings(out ASPxHtmlEditorUploadValidationSettingsBase validationSetting, out string uploadFolder) {
			validationSetting = null;
			uploadFolder = null;
			bool result = false;
			string dialogName = Control.CurrentDialogName;
			if(string.Equals(dialogName, "insertaudiodialog")) {
				validationSetting = Settings.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload.ValidationSettings;
				uploadFolder = Settings.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload.UploadFolder;
				result = true;
			}
			if(string.Equals(dialogName, "insertvideodialog")) {
				validationSetting = Settings.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload.ValidationSettings;
				uploadFolder = Settings.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload.UploadFolder;
				result = true;
			}
			if(string.Equals(dialogName, "insertflashdialog")) {
				validationSetting = Settings.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload.ValidationSettings;
				uploadFolder = Settings.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload.UploadFolder;
				result = true;
			}
			if(string.Equals(dialogName, "insertimagedialog")) {
				validationSetting = Settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload.ValidationSettings;
				uploadFolder = Settings.SettingsDialogs.InsertImageDialog.SettingsImageUpload.UploadFolder;
				result = true;
			}
			return result;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxHtmlEditor(ViewContext);
		}
		public static ActionResult Export(HtmlEditorSettings settings, HtmlEditorExportFormat format) {
			return Export(settings, format, string.Empty);
		}
		public static ActionResult Export(HtmlEditorSettings settings, HtmlEditorExportFormat format, bool saveAsFile) {
			return Export(settings, format, string.Empty, saveAsFile);
		}
		public static ActionResult Export(HtmlEditorSettings settings, HtmlEditorExportFormat format, string fileName) {
			return Export(settings, format, fileName, true);
		}
		static ActionResult Export(HtmlEditorSettings settings, HtmlEditorExportFormat format, string fileName, bool saveAsFile) {
			string fileFormat = format.ToString().ToLower();
			HtmlEditorExtension extension = GetExtension(settings);
			return ExportUtils.Export(extension, s => Export(extension, s, format), fileName, saveAsFile, fileFormat);
		}
		public static void Export(HtmlEditorSettings settings, Stream outputStream, HtmlEditorExportFormat format) {
			Export(GetExtension(settings), outputStream, format);
		}
		static void Export(HtmlEditorExtension extension, Stream outputStream, HtmlEditorExportFormat format) {
			extension.Control.Export(format, outputStream);
		}
		static HtmlEditorExtension GetExtension(HtmlEditorSettings settings) {
			HtmlEditorExtension extension = ExtensionsFactory.InstanceInternal.HtmlEditor(settings);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
		public static void Import(string filePath, Action<string, IEnumerable<string>> onImport) {
			Import(filePath, false, onImport);
		}
		public static void Import(string filePath, bool useInlineStyles, Action<string, IEnumerable<string>> onImport) {
			Import(filePath, useInlineStyles, null, onImport);
		}
		public static void Import(string filePath, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			Import(filePath, false, contentFolder, onImport);
		}
		public static void Import(string filePath, bool useInlineStyles, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			Import(HtmlEditorImportHelper.ParseImportFormat(filePath), filePath, useInlineStyles, contentFolder, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, string filePath, Action<string, IEnumerable<string>> onImport) {
			Import(format, filePath, false, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles, Action<string, IEnumerable<string>> onImport) {
			Import(format, filePath, useInlineStyles, null, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, string filePath, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			Import(format, filePath, false, contentFolder, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			using(FileStream input = File.OpenRead(UrlUtils.ResolvePhysicalPath(filePath))) {
				Import(format, input, useInlineStyles, contentFolder, onImport);
			}
		}
		public static void Import(HtmlEditorImportFormat format, Stream inputStream, Action<string, IEnumerable<string>> onImport) {
			Import(format, inputStream, false, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, Stream inputStream, bool useInlineStyles, Action<string, IEnumerable<string>> onImport) {
			Import(format, inputStream, useInlineStyles, null, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, Stream inputStream, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			Import(format, inputStream, false, contentFolder, onImport);
		}
		public static void Import(HtmlEditorImportFormat format, Stream inputStream, bool useInlineStyles, string contentFolder, Action<string, IEnumerable<string>> onImport) {
			MvcUrlResolutionService.Initialize();
			HtmlEditorImportHelper.Import(
				MvcUtils.MvcUrlResolutionService,
				format,
				inputStream,
				contentFolder,
				null,
				useInlineStyles,
				(html, cssFiles) => onImport(html, cssFiles)
			);
		}
		public new static ContentResult GetCustomDataCallbackResult(object data) {
			if(string.IsNullOrEmpty(MvcUtils.CallbackArgument) || MvcUtils.CallbackArgument[0] != CustomDataCallbackPrefix) {
				throw new InvalidOperationException(
					"A client MVCxClientHtmlEditor.PerformDataCallback function must be called in order to execute the GetCustomDataCallbackResult server method.");
			}
			return ExtensionBase.GetCustomDataCallbackResult(data);
		}
	}
}
