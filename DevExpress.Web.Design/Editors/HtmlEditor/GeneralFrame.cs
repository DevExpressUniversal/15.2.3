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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Web.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class HtmlEditorGeneralFrame : EditFrameBase {
		const string ConfirmGeneratePresetMessage = "Are you sure you want to generate an HTML Editor from preset? \nAll editor settings will be recreated according to the selected preset. ";
		protected const int GroupItemHeight = 34;
		const int TopPanelHeight = 150;
		const float FontSizeGroupItems = 9.5f;
		Size MinimumFrameSize = new Size(720, 630);
		PresetsModel model;
		Size DefaultButtonSize = new Size(97, 30);
		RadioGroup toolbarModeRadioGroup;
		RadioGroup presetsRadioGroup;
		PictureEdit previewImage;
		LabelControl descriptionLabel;
		public HtmlEditorGeneralFrame()
			: base() {
			MinimumSize = MinimumFrameSize;
			Load += (s, e) => {
				Dock = DockStyle.Fill;
				CreateLayout();
				PresetsRadioGroup.Location = new Point(ToolbarModeRadioGroup.Left, ToolbarModeRadioGroup.Top + ToolbarModeRadioGroup.Height + 20);
			};
		}
		protected override string FrameName { get { return "HtmlEditorPresetsFrame"; } }
		PresetsModel Model {
			get {
				if(model == null)
					model = (PresetsModel)DesignerItem.Tag;
				return model;
			}
		}
		XtraPanel TopPanel { get; set; }
		XtraPanel BottomPanel { get; set; }
		SimpleButton GenerateButton { get; set; }
		LabelControl BottomSeparator { get; set; }
		LabelControl LabelCaptionBottomPanel { get; set; }
		CheckPropertiesDescriptor CheckProperties { get; set; }
		RadioGroup ToolbarModeRadioGroup {
			get {
				if(toolbarModeRadioGroup == null) {
					toolbarModeRadioGroup = CreateRadioGroup(BottomPanel, DockStyle.None, "RadioGroupToolbarMode");
					toolbarModeRadioGroup.Font = new Font(toolbarModeRadioGroup.Font.FontFamily, FontSizeGroupItems);
					toolbarModeRadioGroup.EditValueChanged += (s, e) => { OnRadioGroupToolbarModeChanged((HtmlEditorToolbarMode)ToolbarModeRadioGroup.EditValue); };
				}
				return toolbarModeRadioGroup;
			}
		}
		RadioGroup PresetsRadioGroup {
			get {
				if(presetsRadioGroup == null) {
					presetsRadioGroup = CreateRadioGroup(BottomPanel, DockStyle.None, "PresetsRadioGroup");
					presetsRadioGroup.Font = new Font(presetsRadioGroup.Font.FontFamily, FontSizeGroupItems);
					presetsRadioGroup.EditValueChanged += (s, e) => { OnRadioGroupPresetTypeChanged((PresetTypes)PresetsRadioGroup.EditValue); };
				}
				return presetsRadioGroup;
			}
		}
		PictureEdit PreviewImage {
			get {
				if(previewImage == null)
					previewImage = new PictureEdit();
				return previewImage;
			}
		}
		LabelControl DescriptionLabel {
			get {
				if(descriptionLabel == null)
					descriptionLabel = new LabelControl();
				return descriptionLabel;
			}
		}
		RadioGroup CreateRadioGroup(Control parent, DockStyle dock, string name) {
			var result = new RadioGroup() { Name = name };
			result.BorderStyle = BorderStyles.NoBorder;
			result.Parent = parent;
			result.Dock = dock;
			return result;
		}
		void CreateLayout() {
			SuspendLayout();
			CreateBottomPanel();
			CreateTopPanel();
			CreateBottomPanelControls();
			CreateSeparators();
			CreatePresetsToolbarModeRadioGroup();
			CreateTabsCheckBoxGroup();
			ResumeLayout();
		}
		void CreateSeparators() {
			var width = MainPanel.Left + PreviewImage.Right - 40;
			BottomSeparator = CreateSeparator(BottomPanel, new Point(40, 0), width);
		}
		void CreateBottomPanelControls() {
			LabelCaptionBottomPanel = CreateLabelCaption(BottomPanel, "LabelCaptionBottomPanel", "Recreate ASPxHtmlEditor with Preset", new Point(40, 20));
			PreviewImage.Parent = BottomPanel;
			PreviewImage.BackColor = Color.Transparent;
			PreviewImage.Location = new Point(200, LabelCaptionBottomPanel.Bottom + 20);
			PreviewImage.Size = new Size(490, 285);
			PreviewImage.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			PreviewImage.Padding = new System.Windows.Forms.Padding(10);
			DescriptionLabel.Parent = BottomPanel;
			DescriptionLabel.AutoSizeMode = LabelAutoSizeMode.Vertical;
			GenerateButton = DesignTimeFormHelper.CreateButton(BottomPanel, new Size(160, 40), new Point(0, 0), 6, "Generate Preset", OnClickGenerate);
			GenerateButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
		}
		void OnClickGenerate(object sender, EventArgs e) {
			var result = ConfirmMessageBox.Show(ConfirmGeneratePresetMessage, false, Model.ServiceProvider);
			if(result.Dialogresult == System.Windows.Forms.DialogResult.OK) {
				Model.GeneratePreset();
				NavigateToToolbar();
			}
		}
		void NavigateToToolbar() {
			var commonForm = (WrapperEditorForm)this.ParentForm;
			if(commonForm != null)
				commonForm.NavigateToNavBarItem("Toolbars");
		}
		void UpdateGenerateButtonEnabled() {
			GenerateButton.Enabled = Model.CanGeneratePreset;
		}
		void CreateTopPanel() {
			TopPanel = DesignTimeFormHelper.CreatePanel(MainPanel, "TopPanel", DockStyle.Top);
			TopPanel.Height = TopPanelHeight;
		}
		void CreateBottomPanel() {
			BottomPanel = DesignTimeFormHelper.CreatePanel(MainPanel, "BottomPanel", DockStyle.Fill);
		}
		void CreatePresetsToolbarModeRadioGroup() {
			ToolbarModeRadioGroup.Location = new Point(LabelCaptionBottomPanel.Left + 8, LabelCaptionBottomPanel.Bottom + 9);
			ToolbarModeRadioGroup.Size = new Size(470, 260);
			Model.VisibleToolbarModes.ForEach(m => ToolbarModeRadioGroup.Properties.Items.Add(new RadioGroupItem(m.ToolbarMode, m.Description)));
			UpdateRadioGroupSize(ToolbarModeRadioGroup);
			ToolbarModeRadioGroup.EditValue = Model.SelectedToolbarModel.ToolbarMode;
		}
		protected void CreateTabsCheckBoxGroup() {
			CheckProperties = new CheckPropertiesDescriptor(Model);
			CheckProperties.Create(TopPanel);
		}
		LabelControl CreateSeparator(Control parent, Point location, int width) {
			var separator = new LabelControl();
			separator.AutoSizeMode = LabelAutoSizeMode.None;
			separator.BorderStyle = BorderStyles.NoBorder;
			separator.LineOrientation = LabelLineOrientation.Horizontal;
			separator.LineVisible = true;
			separator.Margin = new Padding();
			separator.Location = location;
			separator.Name = "Separator";
			separator.Size = new Size(width, 8);
			separator.Parent = parent;
			separator.BringToFront();
			return separator;
		}
		CheckEdit CreateCheckEdit(Control parent, string name, string text, int itemIndex, Point location) {
			var checkBox = new CheckEdit() { Name = name, Text = text, Tag = itemIndex };
			checkBox.Parent = parent;
			checkBox.Font = new Font(checkBox.Font.FontFamily, FontSizeGroupItems);
			checkBox.Height = 30;
			checkBox.Width = DesignTimeFormHelper.GetTextWidth(checkBox, text, checkBox.Font) + 32;
			checkBox.Location = location;
			checkBox.BringToFront();
			return checkBox;
		}
		LabelControl CreateLabelCaption(Control parent, string name, string text, Point location) {
			var labelCaption = DesignTimeFormHelper.CreateLabel(parent, name, text, location);
			labelCaption.Font = new Font(labelCaption.Font.FontFamily, 10.5f, FontStyle.Bold);
			labelCaption.Width = DesignTimeFormHelper.GetTextWidth(labelCaption, labelCaption.Text, labelCaption.Font) + 10;
			return labelCaption;
		}
		void OnRadioGroupToolbarModeChanged(HtmlEditorToolbarMode toolbarMode) {
			Model.SetSelectedModel(toolbarMode);
			var selectedToolbarMode = Model.SelectedToolbarModel;
			PresetsRadioGroup.Properties.Items.Clear();
			selectedToolbarMode.VisiblePresets.ForEach(p => PresetsRadioGroup.Properties.Items.Add(new RadioGroupItem(p.PresetType, p.Caption)));
			PresetsRadioGroup.EditValue = selectedToolbarMode.SelectedPreset.PresetType;
			OnRadioGroupPresetTypeChanged(selectedToolbarMode.SelectedPreset.PresetType);
			UpdateGenerateButtonEnabled();
			RecalcLayout();
		}
		void RecalcLayout() {
			UpdateRadioGroupSize(ToolbarModeRadioGroup);
			UpdateRadioGroupSize(PresetsRadioGroup);
			DescriptionLabel.Location = new Point(PreviewImage.Left, PreviewImage.Bottom + 15);
			DescriptionLabel.Size = new Size(PreviewImage.Width, Height - DescriptionLabel.Top);
			GenerateButton.Location = new Point(DescriptionLabel.Right - GenerateButton.Width, DescriptionLabel.Bottom + 20);
			BottomSeparator.Top = GenerateButton.Bottom + 10;
		}
		void UpdateRadioGroupSize(RadioGroup radioGroup) {
			var width = PreviewImage.Left - radioGroup.Left - 8;
			radioGroup.Width = width;
			radioGroup.Size = new Size(radioGroup.Width, radioGroup.Properties.Items.Count * GroupItemHeight);
		}
		void OnRadioGroupPresetTypeChanged(PresetTypes presetType) {
			Model.SetSelectedPreset(presetType);
			ReloadPreviewImage();
			UpdatePresetDescription();
			UpdateGenerateButtonEnabled();
			RecalcLayout();
		}
		void UpdatePresetDescription() {
			DescriptionLabel.Text = Model.SelectedPreset.Description;
		}
		void ReloadPreviewImage() {
			var stream = GetType().Assembly.GetManifestResourceStream(Model.SelectedPreset.ImageUrl);
			if(stream == null)
				return;
			var image = Image.FromStream(stream);
			PreviewImage.MinimumSize = image.Size;
			PreviewImage.Image = image;
		}
	}
	public enum PresetTypes { Simple, Standard, Advanced, None };
	public class PresetsModel : CheckPropertiesModel {
		List<ToolbarModel> toolbarModes;
		ASPxHtmlEditor htmlEditor;
		public PresetsModel(ASPxHtmlEditor htmlEditor) 
			: base(htmlEditor, "Tabs") {
			FillToolbarModes();
			SetDefaultSelectedToolbarModel();
		}
		public IServiceProvider ServiceProvider { get { return HtmlEditor != null ? HtmlEditor.Site : null; } }
		ASPxHtmlEditor HtmlEditor {
			get {
				if(htmlEditor == null)
					htmlEditor = (ASPxHtmlEditor)WebControl;
				return htmlEditor;
			}
		}
		public ToolbarModel SelectedToolbarModel { get; private set; }
		public PresetDescriptor SelectedPreset { get { return SelectedToolbarModel.SelectedPreset; } }
		public List<ToolbarModel> ToolbarModes {
			get {
				if(toolbarModes == null)
					toolbarModes = new List<ToolbarModel>();
				return toolbarModes;
			}
		}
		public List<ToolbarModel> VisibleToolbarModes { get { return ToolbarModes.Where(t => !string.IsNullOrEmpty(t.Description)).ToList(); } }
		public void ResetModel() {
			SetSelectedModel(HtmlEditorToolbarMode.None);
			SetSelectedPreset(PresetTypes.None);
		}
		public void SetSelectedModel(HtmlEditorToolbarMode toolbarMode) {
			SelectedToolbarModel = ToolbarModes.FirstOrDefault(t => t.ToolbarMode == toolbarMode);
		}
		public void SetSelectedPreset(PresetTypes presetType) {
			SelectedToolbarModel.SetSelectedPreset(presetType);
		}
		public bool CanGeneratePreset { get { return SelectedToolbarModel.ToolbarMode != HtmlEditorToolbarMode.None; } }
		public void GeneratePreset() {
			ApplyDefaultSettings(HtmlEditor);
			if(SelectedToolbarModel.ToolbarMode == HtmlEditorToolbarMode.Menu) {
				if(SelectedPreset.PresetType == PresetTypes.Simple)
					ApplySimpleMenuSettings(HtmlEditor);
				else if(SelectedPreset.PresetType == PresetTypes.Standard)
					ApplyStandardMenuSettings(HtmlEditor);
				else if(SelectedPreset.PresetType == PresetTypes.Advanced)
					ApplyAdvancedMenuSettings(HtmlEditor);
			} else if(SelectedToolbarModel.ToolbarMode == HtmlEditorToolbarMode.Ribbon) {
				if(SelectedPreset.PresetType == PresetTypes.Standard)
					ApplyStandardRibbonSettings(HtmlEditor);
				else if(SelectedPreset.PresetType == PresetTypes.Advanced)
					ApplyAdvancedRibbonSettings(HtmlEditor);
			}
			SetSelectedModel(HtmlEditorToolbarMode.None);
		}
		void FillToolbarModes() {
			ToolbarModes.Add(new ToolbarModel(HtmlEditorToolbarMode.Menu, "Bars"));
			ToolbarModes.Add(new ToolbarModel(HtmlEditorToolbarMode.Ribbon, "Ribbon"));
			ToolbarModes.Add(new ToolbarModel(HtmlEditorToolbarMode.None, string.Empty));
		}
		void SetDefaultSelectedToolbarModel() {
			SelectedToolbarModel = ToolbarModes.FirstOrDefault(t => t.ToolbarMode == HtmlEditorToolbarMode.None);
		}
		protected override void FillTabsCheckBoxGroup() {
			SettingsCheckGroup.Add(HtmlEditor, "Design View", "Settings.AllowDesignView");
			SettingsCheckGroup.Add(HtmlEditor, "HTML View", "Settings.AllowHtmlView");
			SettingsCheckGroup.Add(HtmlEditor, "Preview", "Settings.AllowPreview");
			SettingsCheckGroup.Add(HtmlEditor, "Allow Context Menu", "Settings.AllowContextMenu", HtmlEditor.Settings.AllowContextMenu.GetType());
			SettingsCheckGroup.Add(HtmlEditor, "Tag Inspector", "Settings.ShowTagInspector");
		}
		protected void ApplyDefaultSettings(ASPxHtmlEditor htmlEditor) {
			ASPxHtmlEditor defaultHtmlEditor = new ASPxHtmlEditor();
			htmlEditor.CopyBaseAttributes(defaultHtmlEditor);
			htmlEditor.ActiveView = defaultHtmlEditor.ActiveView;
			htmlEditor.EnableCallbackAnimation = defaultHtmlEditor.EnableCallbackAnimation;
			htmlEditor.ClientSideEvents.Assign(defaultHtmlEditor.ClientSideEvents);
			htmlEditor.ClientEnabled = defaultHtmlEditor.ClientEnabled;
			htmlEditor.ClientVisible = defaultHtmlEditor.ClientVisible;
			htmlEditor.CssFiles.Assign(defaultHtmlEditor.CssFiles);
			htmlEditor.CustomDialogs.Assign(defaultHtmlEditor.CustomDialogs);
			htmlEditor.Html = defaultHtmlEditor.Html;
			htmlEditor.Images.CopyFrom(defaultHtmlEditor.Images);
			htmlEditor.ImagesEditors.CopyFrom(defaultHtmlEditor.ImagesEditors);
			htmlEditor.ImagesFileManager.CopyFrom(defaultHtmlEditor.ImagesFileManager);
			htmlEditor.PartsRoundPanelInternal.Assign(defaultHtmlEditor.PartsRoundPanelInternal);
			htmlEditor.SaveStateToCookies = defaultHtmlEditor.SaveStateToCookies;
			htmlEditor.SaveStateToCookiesID = defaultHtmlEditor.SaveStateToCookiesID;
			htmlEditor.ToolbarMode = defaultHtmlEditor.ToolbarMode;
			htmlEditor.AssociatedRibbonID = defaultHtmlEditor.AssociatedRibbonID;
			htmlEditor.Settings.Assign(defaultHtmlEditor.Settings);
			htmlEditor.SettingsDialogs.Assign(defaultHtmlEditor.SettingsDialogs);
			htmlEditor.SettingsForms.Assign(defaultHtmlEditor.SettingsForms);
			htmlEditor.SettingsHtmlEditing.Assign(defaultHtmlEditor.SettingsHtmlEditing);
			htmlEditor.SettingsDialogs.InsertImageDialog.Assign(defaultHtmlEditor.SettingsDialogs.InsertImageDialog);
			htmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageUpload.Assign(defaultHtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageUpload);
			htmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload.Assign(defaultHtmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload);
			htmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload.Assign(defaultHtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload);
			htmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload.Assign(defaultHtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload);
			htmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.Assign(defaultHtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector);
			htmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.Assign(defaultHtmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector);
			htmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.Assign(defaultHtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector);
			htmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.Assign(defaultHtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector);
			htmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.Assign(defaultHtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector);
			htmlEditor.SettingsLoadingPanel.Assign(defaultHtmlEditor.SettingsLoadingPanel);
			htmlEditor.SettingsResize.Assign(defaultHtmlEditor.SettingsResize);
			htmlEditor.SettingsSpellChecker.Assign(defaultHtmlEditor.SettingsSpellChecker);
			htmlEditor.SettingsText.Assign(defaultHtmlEditor.SettingsText);
			htmlEditor.SettingsValidation.Assign(defaultHtmlEditor.SettingsValidation);
#pragma warning disable 618
			htmlEditor.SettingsDialogFormElements.Assign(defaultHtmlEditor.SettingsDialogFormElements);
#pragma warning restore 618
			htmlEditor.Styles.CopyFrom(defaultHtmlEditor.Styles);
			htmlEditor.StylesButton.CopyFrom(defaultHtmlEditor.StylesButton);
			htmlEditor.StylesContextMenu.CopyFrom(defaultHtmlEditor.StylesContextMenu);
			htmlEditor.StylesDialogForm.CopyFrom(defaultHtmlEditor.StylesDialogForm);
			htmlEditor.StylesEditors.CopyFrom(defaultHtmlEditor.StylesEditors);
			htmlEditor.StylesRoundPanel.CopyFrom(defaultHtmlEditor.StylesRoundPanel);
			htmlEditor.StylesSpellChecker.CopyFrom(defaultHtmlEditor.StylesSpellChecker);
			htmlEditor.StylesStatusBar.CopyFrom(defaultHtmlEditor.StylesStatusBar);
			htmlEditor.StylesFileManager.CopyFrom(defaultHtmlEditor.StylesFileManager);
			htmlEditor.StylesToolbars.CopyFrom(defaultHtmlEditor.StylesToolbars);
			htmlEditor.StylesPasteOptionsBar.CopyFrom(defaultHtmlEditor.StylesPasteOptionsBar);
			htmlEditor.ContextMenuItems.Assign(defaultHtmlEditor.ContextMenuItems);
			htmlEditor.AccessibilityCompliant = defaultHtmlEditor.AccessibilityCompliant;
			htmlEditor.RightToLeft = defaultHtmlEditor.RightToLeft;
			htmlEditor.Toolbars.Assign(defaultHtmlEditor.Toolbars);
			htmlEditor.RibbonTabs.Assign(defaultHtmlEditor.RibbonTabs);
			htmlEditor.Shortcuts.Assign(defaultHtmlEditor.Shortcuts);
			htmlEditor.Placeholders.Assign(defaultHtmlEditor.Placeholders);
		}
		protected void ApplySimpleMenuSettings(ASPxHtmlEditor htmlEditor) {
			htmlEditor.ToolbarMode = HtmlEditorToolbarMode.Menu;
			htmlEditor.Settings.AllowHtmlView = false;
			htmlEditor.Settings.AllowPreview = false;
			htmlEditor.SettingsDialogs.InsertImageDialog.ShowStyleSettingsSection = false;
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.Tags = new string[] { "a", "b", "strong", "em", "i", "span", "p", "div", "ol", "ul", "li", "img", "font" };
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.TagFilterMode = HtmlEditorFilterMode.WhiteList;
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.Attributes = new string[] { "alt", "align", "src", "href", "title", "style", "width", "height" };
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.AttributeFilterMode = HtmlEditorFilterMode.WhiteList;
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.StyleAttributes = new string[] { "width", "height", "font-size", "font-family", "color", "text-align", "float" };
			htmlEditor.SettingsHtmlEditing.ContentElementFiltering.StyleAttributeFilterMode = HtmlEditorFilterMode.WhiteList;
			HtmlEditorToolbar toolbar = htmlEditor.Toolbars.Add("SimpleToolbar");
			toolbar.Items.Add(new ToolbarBoldButton());
			toolbar.Items.Add(new ToolbarItalicButton());
			toolbar.Items.Add(new ToolbarUnderlineButton());
			toolbar.Items.Add(new ToolbarStrikethroughButton());
			toolbar.Items.Add(new ToolbarInsertUnorderedListButton(true));
			toolbar.Items.Add(new ToolbarInsertOrderedListButton());
			toolbar.Items.Add(new ToolbarInsertLinkDialogButton());
			toolbar.Items.Add(new ToolbarUnlinkButton());
			toolbar.Items.Add(new ToolbarInsertImageDialogButton());
			toolbar.Items.Add(new ToolbarRemoveFormatButton(true));
			toolbar.Items.Add(new ToolbarFontColorButton(true));
		}
		protected void ApplyStandardMenuSettings(ASPxHtmlEditor htmlEditor) {
			htmlEditor.ToolbarMode = HtmlEditorToolbarMode.Menu;
			htmlEditor.Toolbars.CreateDefaultToolbars();
		}
		protected void ApplyStandardRibbonSettings(ASPxHtmlEditor htmlEditor) {
			htmlEditor.ToolbarMode = HtmlEditorToolbarMode.Ribbon;
			htmlEditor.RibbonTabs.CreateDefaultRibbonTabs();
		}
		protected void ApplyCommonAdvancedSettings(ASPxHtmlEditor htmlEditor) {
			htmlEditor.Settings.ShowTagInspector = true;
			htmlEditor.Settings.AllowCustomColorsInColorPickers = true;
			htmlEditor.Settings.AllowInsertDirectImageUrls = true;
			htmlEditor.Settings.AllowScriptExecutionInPreview = true;
			htmlEditor.SettingsHtmlEditing.EnablePasteOptions = true;
			htmlEditor.SettingsHtmlEditing.AllowFormElements = true;
			htmlEditor.SettingsHtmlEditing.AllowHTML5MediaElements = true;
			htmlEditor.SettingsHtmlEditing.AllowIdAttributes = true;
			htmlEditor.SettingsHtmlEditing.AllowIFrames = true;
			htmlEditor.SettingsHtmlEditing.AllowObjectAndEmbedElements = true;
			htmlEditor.SettingsHtmlEditing.AllowScripts = true;
			htmlEditor.SettingsHtmlEditing.AllowStyleAttributes = true;
			htmlEditor.SettingsHtmlEditing.AllowYouTubeVideoIFrames = true;
			htmlEditor.SettingsHtmlEditing.EnterMode = HtmlEditorEnterMode.BR;
			htmlEditor.SettingsHtmlEditing.PasteMode = HtmlEditorPasteMode.MergeFormatting;
			htmlEditor.SettingsHtmlEditing.AllowEditFullDocument = true;
			htmlEditor.Settings.SettingsHtmlView.EnableAutoCompletion = true;
			htmlEditor.Settings.SettingsHtmlView.EnableTagAutoClosing = true;
			htmlEditor.Settings.SettingsHtmlView.HighlightActiveLine = true;
			htmlEditor.Settings.SettingsHtmlView.HighlightMatchingTags = true;
			htmlEditor.Settings.SettingsHtmlView.ShowCollapseTagButtons = true;
			htmlEditor.Settings.SettingsHtmlView.ShowLineNumbers = true;
			htmlEditor.SettingsResize.AllowResize = true;
			htmlEditor.Placeholders.Add("Placeholder1");
			htmlEditor.Placeholders.Add("Placeholder2");
			htmlEditor.Placeholders.Add("Placeholder3");
		}
		protected void ApplyAdvancedMenuSettings(ASPxHtmlEditor htmlEditor) {
			ApplyCommonAdvancedSettings(htmlEditor);
			htmlEditor.ToolbarMode = HtmlEditorToolbarMode.Menu;
			HtmlEditorToolbar toolbar1 = htmlEditor.Toolbars.Add();
			toolbar1.Items.Add(new ToolbarCutButton());
			toolbar1.Items.Add(new ToolbarCopyButton());
			toolbar1.Items.Add(new ToolbarPasteButton());
			toolbar1.Items.Add(new ToolbarPasteFromWordButton());
			toolbar1.Items.Add(new ToolbarUndoButton(true));
			toolbar1.Items.Add(new ToolbarRedoButton());
			toolbar1.Items.Add(new ToolbarRemoveFormatButton(true));
			toolbar1.Items.Add(new ToolbarSuperscriptButton(true));
			toolbar1.Items.Add(new ToolbarSubscriptButton());
			toolbar1.Items.Add(new ToolbarInsertOrderedListButton(true));
			toolbar1.Items.Add(new ToolbarInsertUnorderedListButton());
			toolbar1.Items.Add(new ToolbarIndentButton(true));
			toolbar1.Items.Add(new ToolbarOutdentButton());
			toolbar1.Items.Add(new ToolbarInsertLinkDialogButton(true));
			toolbar1.Items.Add(new ToolbarUnlinkButton(true));
			ToolbarTableOperationsDropDownButton tableOperationsButton = new ToolbarTableOperationsDropDownButton(true);
			tableOperationsButton.CreateDefaultItems();
			toolbar1.Items.Add(tableOperationsButton);
			ToolbarExportDropDownButton exportButton = new ToolbarExportDropDownButton(true);
			exportButton.CreateDefaultItems();
			toolbar1.Items.Add(exportButton);
			toolbar1.Items.Add(new ToolbarPrintButton());
			toolbar1.Items.Add(new ToolbarFullscreenButton(true));
			toolbar1.Items.Add(new ToolbarFindAndReplaceDialogButton(true));
			HtmlEditorToolbar toolbar2 = htmlEditor.Toolbars.Add();
			ToolbarParagraphFormattingEdit paragraphFormattingEditButton = new ToolbarParagraphFormattingEdit();
			paragraphFormattingEditButton.CreateDefaultItems();
			toolbar2.Items.Add(paragraphFormattingEditButton);
			ToolbarFontNameEdit fontNameEditButton = new ToolbarFontNameEdit();
			fontNameEditButton.CreateDefaultItems();
			toolbar2.Items.Add(fontNameEditButton);
			ToolbarFontSizeEdit fontSizeEditButton = new ToolbarFontSizeEdit();
			fontSizeEditButton.CreateDefaultItems();
			toolbar2.Items.Add(fontSizeEditButton);
			toolbar2.Items.Add(new ToolbarBoldButton(true));
			toolbar2.Items.Add(new ToolbarItalicButton());
			toolbar2.Items.Add(new ToolbarUnderlineButton());
			toolbar2.Items.Add(new ToolbarStrikethroughButton());
			toolbar2.Items.Add(new ToolbarJustifyLeftButton(true));
			toolbar2.Items.Add(new ToolbarJustifyCenterButton());
			toolbar2.Items.Add(new ToolbarJustifyRightButton());
			toolbar2.Items.Add(new ToolbarJustifyFullButton());
			toolbar2.Items.Add(new ToolbarBackColorButton(true));
			toolbar2.Items.Add(new ToolbarFontColorButton());
			HtmlEditorToolbar toolbar3 = htmlEditor.Toolbars.Add();
			toolbar3.Items.Add(new ToolbarInsertImageDialogButton());
			toolbar3.Items.Add(new ToolbarInsertFlashDialogButton());
			toolbar3.Items.Add(new ToolbarInsertAudioDialogButton());
			toolbar3.Items.Add(new ToolbarInsertVideoDialogButton());
			toolbar3.Items.Add(new ToolbarInsertYouTubeVideoDialogButton());
			toolbar3.Items.Add(new ToolbarInsertPlaceholderDialogButton(true));
		}
		protected void ApplyAdvancedRibbonSettings(ASPxHtmlEditor htmlEditor) {
			ApplyCommonAdvancedSettings(htmlEditor);
			htmlEditor.ToolbarMode = HtmlEditorToolbarMode.Ribbon;
			HtmlEditorDefaultRibbon defaultRibbon = new HtmlEditorDefaultRibbon(htmlEditor);
			HEHomeRibbonTab homeRibbonTab = new HEHomeRibbonTab();
			homeRibbonTab.Groups.Add(defaultRibbon.CreateUndoGroup());
			homeRibbonTab.Groups.Add(defaultRibbon.CreateClipboardGroup());
			homeRibbonTab.Groups.Add(defaultRibbon.CreateFontGroup());
			homeRibbonTab.Groups.Add(defaultRibbon.CreateParagraphGroup());
			homeRibbonTab.Groups.Add(defaultRibbon.CreateEditingGroup());
			htmlEditor.RibbonTabs.Add(homeRibbonTab);
			HEInsertRibbonTab insertRibbonTab = new HEInsertRibbonTab();
			insertRibbonTab.Groups.Add(defaultRibbon.CreateImagesGroup());
			insertRibbonTab.Groups.Add(defaultRibbon.CreateMediaGroup());
			insertRibbonTab.Groups.Add(defaultRibbon.CreateLinksGroup());
			RibbonGroup placeholdersRibbonGroup = new RibbonGroup("Placeholders");
			placeholdersRibbonGroup.Items.Add(new HEInsertPlaceholderDialogRibbonCommand(RibbonItemSize.Large));
			insertRibbonTab.Groups.Add(placeholdersRibbonGroup);
			htmlEditor.RibbonTabs.Add(insertRibbonTab);
			HEViewRibbonTab viewRibbonTab = new HEViewRibbonTab();
			viewRibbonTab.Groups.Add(defaultRibbon.CreateViewsGroup());
			htmlEditor.RibbonTabs.Add(viewRibbonTab);
			RibbonTab exportRibbonTab = new RibbonTab("Export");
			RibbonGroup commonRibbonGroup = new RibbonGroup("Common");
			RibbonDropDownButtonItem exportToRibbonDropDownButtonItem = new RibbonDropDownButtonItem("Export To", "Export To", RibbonItemSize.Large);
			exportToRibbonDropDownButtonItem.LargeImage.IconID = "export_exportfile_32x32";
			exportToRibbonDropDownButtonItem.SmallImage.IconID = "export_exportfile_16x16";
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToRtfDropDownRibbonCommand());
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToPdfDropDownRibbonCommand());
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToTxtDropDownRibbonCommand());
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToDocxDropDownRibbonCommand());
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToOdtDropDownRibbonCommand());
			exportToRibbonDropDownButtonItem.Items.Add(new HEExportToMhtDropDownRibbonCommand());
			commonRibbonGroup.Items.Add(exportToRibbonDropDownButtonItem);
			commonRibbonGroup.Items.Add(new HEPrintRibbonCommand("Save As", RibbonItemSize.Large));
			exportRibbonTab.Groups.Add(commonRibbonGroup);
			htmlEditor.RibbonTabs.Add(exportRibbonTab);
		}
	}
	public class ToolbarModel {
		List<PresetDescriptor> presetTypes;
		List<PresetDescriptor> visiblePresets;
		public const string PresetNotSelectedImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorNoPresetSelected.png";
		public const string StandartRibbonPresetImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorStandartRibbonPreset.png";
		public const string LiteBarsPresetImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorLiteBarsPreset.png";
		public const string StandartBarsPresetImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorStandartBarsPreset.png";
		public const string FullBarsPresetImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorFullBarsPreset.png";
		public const string FullRibbonPresetImageResource = "DevExpress.Web.Design.Images.HtmlEditor.HtmlEditorFullRibbonPreset.png";
		public ToolbarModel(HtmlEditorToolbarMode toolbarMode, string description) {
			ToolbarMode = toolbarMode;
			Description = description;
			FillPresets();
			SelectedPreset = Presets[0];
		}
		public HtmlEditorToolbarMode ToolbarMode { get; private set; }
		public string Description { get; set; }
		public PresetDescriptor SelectedPreset { get; private set; }
		public List<PresetDescriptor> Presets {
			get {
				if(presetTypes == null)
					presetTypes = new List<PresetDescriptor>();
				return presetTypes;
			}
		}
		public List<PresetDescriptor> VisiblePresets {
			get {
				if(visiblePresets == null)
					visiblePresets = Presets.Where(p => !string.IsNullOrEmpty(p.Caption)).ToList();
				return visiblePresets;
			}
		}
		public void SetSelectedPreset(PresetTypes presetType) {
			SelectedPreset = Presets.FirstOrDefault(p => p.PresetType == presetType);
		}
		void FillPresets() {
			if(ToolbarMode == HtmlEditorToolbarMode.Menu) {
				Presets.Add(new PresetDescriptor(PresetTypes.Simple, "Simple", "This preset generates a simple HTML Editor with only design view enabled. The editor's toolbar contains a small item set. The content element filtering is enabled, therefore elements, which cannot be added using the provided items, are automatically removed from the content.", LiteBarsPresetImageResource));
				Presets.Add(new PresetDescriptor(PresetTypes.Standard, "Standard", "This preset generates an HTML Editor with default toolbar item set and default features.", StandartBarsPresetImageResource));
				Presets.Add(new PresetDescriptor(PresetTypes.Advanced, "Advanced (full)", "This preset generates a comprehensive HTML Editor with enabled resizing, tag inspector and paste options features. The editor's toolbar contains an extended item set, including the insert media content buttons, insert placeholder button, export button, and print button.", FullBarsPresetImageResource));
			} else if(ToolbarMode == HtmlEditorToolbarMode.Ribbon) {
				Presets.Add(new PresetDescriptor(PresetTypes.Standard, "Standard", "This preset generates an HTML Editor with default ribbon item set and default features.", StandartRibbonPresetImageResource));
				Presets.Add(new PresetDescriptor(PresetTypes.Advanced, "Advanced (full)", "This preset generates a comprehensive HTML Editor with enabled resizing, tag inspector and paste options features. The editor's ribbon contains an extended item set, including the insert media content buttons, insert placeholder button, export button, and print button.", FullRibbonPresetImageResource));
			} else if(ToolbarMode == HtmlEditorToolbarMode.None)
				Presets.Add(new PresetDescriptor(PresetTypes.None, string.Empty, string.Empty, PresetNotSelectedImageResource));
		}
	}
	public class PresetDescriptor {
		public PresetDescriptor(PresetTypes presetType, string caption, string description, string imageUrl) {
			PresetType = presetType;
			Caption = caption;
			Description = description;
			ImageUrl = imageUrl;
		}
		public PresetTypes PresetType { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
	}
}
