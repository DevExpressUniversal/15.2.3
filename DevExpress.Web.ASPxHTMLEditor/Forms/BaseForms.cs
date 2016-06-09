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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public interface IStyleSettingsOwner {
		object GetCssClassItemsDataSource();
		HtmlEditorDialogStyleLocalizationSettings LocalizationSettings { get; }
	}
	public class HtmlEditorDialogStyleLocalizationSettings {
		public ASPxHtmlEditorStringId BorderColor { get; set; }
		public ASPxHtmlEditorStringId Border { get; set; }
		public ASPxHtmlEditorStringId BorderWidth { get; set; }
		public ASPxHtmlEditorStringId BorderStyle { get; set; }
		public ASPxHtmlEditorStringId BottomMargin { get; set; }
		public ASPxHtmlEditorStringId CssClassName { get; set; }
		public ASPxHtmlEditorStringId LeftMargin { get; set; }
		public ASPxHtmlEditorStringId Margins { get; set; }
		public ASPxHtmlEditorStringId RightMargin { get; set; }
		public ASPxHtmlEditorStringId TopMargin { get; set; }
		public ASPxHtmlEditorStringId PixelLabel { get; set; }
	}
	public class HtmlEditorMediaDialogLocalizationSettings : HtmlEditorDialogStyleLocalizationSettings {
		public ASPxHtmlEditorStringId CommonSettingsTab { get; set; }
		public ASPxHtmlEditorStringId StyleSettingsTab { get; set; }
		public ASPxHtmlEditorStringId EmptyResourcePreview { get; set; }
		public ASPxHtmlEditorStringId EnterUrl { get; set; }
		public ASPxHtmlEditorStringId LocalSourceRadioButton { get; set; }
		public ASPxHtmlEditorStringId SaveToServer { get; set; }
		public ASPxHtmlEditorStringId ShowMoreOptions { get; set; }
		public ASPxHtmlEditorStringId WebSourceRadioButton { get; set; }
		public ASPxHtmlEditorStringId UploadFile { get; set; }
		public ASPxHtmlEditorStringId PopupHeader { get; set; }
		public ASPxHtmlEditorStringId WidthLabel { get; set; }
		public ASPxHtmlEditorStringId HeightLabel { get; set; }
		public ASPxHtmlEditorStringId PositionLabel { get; set; }
		public ASPxHtmlEditorStringId PreviewText { get; set; }
		public ASPxHtmlEditorStringId GalleryTabText { get; set; }
		public ASPxHtmlEditorStringId AllowedFileExtensionsText { get; set; }
		public ASPxHtmlEditorStringId MaximumUploadFileSizeText { get; set; }
	}
	public enum InsertMediaSourceType {
		Audio,
		Video,
		Flash,
		Image,
		YouTube
	}
	[ToolboxItem(false)]
	public class HtmlEditorDialogFormLayout : DialogFormLayoutBase {
		public override string GetClientInstanceName(string name) {
			return "dxheMediaDialog" + name;
		}
		public override string GetControlCssPrefix() {
			return "dxhe-";
		}
		public override string GetLocalizedText(Enum value) {
			return ASPxHtmlEditorLocalizer.GetString((ASPxHtmlEditorStringId)value);
		}
	}
	[ToolboxItem(false)]
	public class MediaContentStyleSettingsControl : HtmlEditorUserControl {
		public const string MarginsItemName = "Margins",
							TopMarginItemName = "TopMargin",
							BottomMarginItemName = "BottomMargin",
							LeftMarginItemName = "LeftMargin",
							RightMarginItemName = "RightMargin",
							BorderItemName = "Border",
							BorderStyleItemName = "BorderStyle",
							BorderWidthItemName = "BorderWidth",
							BorderColorItemName = "BorderColor",
							CssClassNameItemName = "CssClassName";
		protected IStyleSettingsOwner Owner { get; private set; }
		protected DialogFormLayoutBase StyleSettingsFormLayout { get; private set; }
		protected ASPxComboBox CssClassNameComboBox { get; private set; }
		protected List<ASPxEditBase> Editors { get; private set; }
		protected ASPxComboBox BorderStyleComboBox { get; private set; }
		public MediaContentStyleSettingsControl(IStyleSettingsOwner owner)
			: base() {
			Owner = owner;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Editors = new List<ASPxEditBase>();
			ASPxHtmlEditorStringId pixelLabelStringId = Owner.LocalizationSettings.PixelLabel;
			LayoutGroup currentGroup;
			StyleSettingsFormLayout = new HtmlEditorDialogFormLayout();
			Controls.Add(StyleSettingsFormLayout);
			currentGroup = StyleSettingsFormLayout.Items.CreateGroup(MarginsItemName, true);
			currentGroup.ColCount = 2;
			currentGroup.Items.CreateSpinEdit(TopMarginItemName, buffer: Editors, pixelLabelText: pixelLabelStringId);
			currentGroup.Items.CreateSpinEdit(RightMarginItemName, buffer: Editors, pixelLabelText: pixelLabelStringId);
			currentGroup.Items.CreateSpinEdit(BottomMarginItemName, buffer: Editors, pixelLabelText: pixelLabelStringId);
			currentGroup.Items.CreateSpinEdit(LeftMarginItemName, buffer: Editors, pixelLabelText: pixelLabelStringId);
			currentGroup = StyleSettingsFormLayout.Items.CreateGroup(BorderItemName, true);
			currentGroup.ColCount = 2;
			currentGroup.Items.CreateSpinEdit(BorderWidthItemName, buffer: Editors, pixelLabelText: pixelLabelStringId);
			currentGroup.Items.CreateEditor<ASPxColorEdit>(BorderColorItemName, buffer: Editors, colSpan: currentGroup.ColCount).EnableCustomColors =
				HtmlEditor.Settings.AllowCustomColorsInColorPickers;
			BorderStyleComboBox = currentGroup.Items.CreateComboBox(BorderStyleItemName, buffer: Editors, colSpan: currentGroup.ColCount);
			StyleSettingsFormLayout.Items.Add<EmptyLayoutItem>("");
			currentGroup = StyleSettingsFormLayout.Items.CreateGroup();
			CssClassNameComboBox = currentGroup.Items.CreateComboBox(CssClassNameItemName, buffer: Editors);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			StyleSettingsFormLayout.ApplyCommonSettings();
			StyleSettingsFormLayout.AlignItemCaptionsInAllGroups = true;
			RenderUtils.AppendDefaultDXClassName(StyleSettingsFormLayout, "dxhe-mediaDialogStyleSettings");
			StyleSettingsFormLayout.LocalizeField(BorderColorItemName, Owner.LocalizationSettings.BorderColor);
			StyleSettingsFormLayout.LocalizeField(BorderItemName, Owner.LocalizationSettings.Border);
			StyleSettingsFormLayout.LocalizeField(BorderWidthItemName, Owner.LocalizationSettings.BorderWidth);
			StyleSettingsFormLayout.LocalizeField(BorderStyleItemName, Owner.LocalizationSettings.BorderStyle);
			StyleSettingsFormLayout.LocalizeField(BottomMarginItemName, Owner.LocalizationSettings.BottomMargin);
			StyleSettingsFormLayout.LocalizeField(CssClassNameItemName, Owner.LocalizationSettings.CssClassName);
			StyleSettingsFormLayout.LocalizeField(LeftMarginItemName, Owner.LocalizationSettings.LeftMargin);
			StyleSettingsFormLayout.LocalizeField(MarginsItemName, Owner.LocalizationSettings.Margins);
			StyleSettingsFormLayout.LocalizeField(RightMarginItemName, Owner.LocalizationSettings.RightMargin);
			StyleSettingsFormLayout.LocalizeField(TopMarginItemName, Owner.LocalizationSettings.TopMargin);
			CssClassNameComboBox.DropDownStyle = DropDownStyle.DropDown;
			PopulateCssItems(Owner.GetCssClassItemsDataSource());
			BorderStyleComboBox.Items.Add("", "").Selected = true;
			BorderStyleComboBox.Items.Add("None", "none");
			BorderStyleComboBox.Items.Add("Hidden", "hidden");
			BorderStyleComboBox.Items.Add("Dotted", "dotted");
			BorderStyleComboBox.Items.Add("Dashed", "dashed");
			BorderStyleComboBox.Items.Add("Solid", "solid");
			BorderStyleComboBox.Items.Add("Double", "double");
			BorderStyleComboBox.Items.Add("Groove", "groove");
			BorderStyleComboBox.Items.Add("Ridge", "ridge");
			BorderStyleComboBox.Items.Add("Inset", "inset");
			BorderStyleComboBox.Items.Add("Outset", "outset");
			BorderStyleComboBox.Items.Add("Initial", "initial");
			BorderStyleComboBox.Items.Add("Inherit", "inherit");
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return Editors.ToArray();
		}
		void PopulateCssItems(object dataSource) {
			CssClassNameComboBox.TextField = "Text";
			CssClassNameComboBox.ValueField = "CssClass";
			CssClassNameComboBox.DataSource = dataSource;
			CssClassNameComboBox.DataBind();
		}
	}
	[ToolboxItem(false)]
	public abstract class HtmlEditorDialogBase : HtmlEditorUserControl {
		const string SubmitButtonId = "SubmitButton";
		protected ASPxPanel MainPanel { get; private set; }
		protected ASPxLoadingPanel LoadingPanel { get; private set; }
		protected ASPxPanel FormLayoutWrapper { get; private set; }
		protected DialogFormLayoutBase MainFormLayout { get; private set; }
		protected ASPxButton SubmitButton { get; private set; }
		protected ASPxButton CancelButton { get; private set; }
		protected virtual List<ASPxButton> ButtonsList { get; set; }
		protected List<ASPxEditBase> Editors = new List<ASPxEditBase>();
		protected string GetClientInstanceName(string name) {
			return "dxheMediaDialog" + name;
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			PopulateEditList(Editors);
			return Editors.ToArray();
		}
		protected internal void PrepareHierarchy() {
			CreateControlHierarchy();
			PrepareChildControls();
		}
		protected virtual void PopulateEditList(List<ASPxEditBase> list) {
		}
		protected void PrepareColorEditControls(params ASPxColorEdit[] colorEditControls) {
			foreach(ASPxColorEdit colorEditControl in colorEditControls) {
				if(colorEditControl != null)
					colorEditControl.EnableCustomColors = HtmlEditor.Settings.AllowCustomColorsInColorPickers;
			}
		}
		protected override ASPxButton[] GetChildDxButtons() {
			ButtonsList = new List<ASPxButton>() { SubmitButton, CancelButton };
			PopulateButtonList(ButtonsList);
			return ButtonsList.ToArray();
		}
		protected virtual void PopulateButtonList(List<ASPxButton> list) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainPanel = new ASPxPanel();
			Controls.Add(MainPanel);
			LoadingPanel = new ASPxLoadingPanel();
			MainPanel.Controls.Add(LoadingPanel);
			FormLayoutWrapper = new ASPxPanel();
			MainPanel.Controls.Add(FormLayoutWrapper);
			SubmitButton = CreateDialogButton("ASPx.HtmlEditorClasses.Utils.OnDialogSubmitButtonInit", true);
			SubmitButton.ID = SubmitButtonId;
			SubmitButton.CausesValidation = false;
			CancelButton = CreateDialogButton("ASPx.HtmlEditorClasses.Utils.OnDialogCloseButtonInit", false);
			CancelButton.CausesValidation = false;
			MainFormLayout = new HtmlEditorDialogFormLayout();
			FormLayoutWrapper.Controls.Add(MainFormLayout);
			LayoutGroup group = MainFormLayout.Items.CreateGroup();
			group.CssClass = "dxhe-dialogContentGroup";
			group.GroupBoxDecoration = GroupBoxDecoration.None;
			LayoutItem topTemplateItem = group.InsertTemplate(GetTopAreaTemplate());
			PopulateContentGroup(group);
			if(topTemplateItem != null)
				topTemplateItem.ColSpan = group.ColCount;
			group.InsertTemplate(GetBottomAreaTemplate());
			group.GroupBoxStyle.CssClass = "dxhe-dialogFirstLGB";
			List<Control> bottomControls = new List<Control>();
			bottomControls.Add(SubmitButton);
			bottomControls.Add(CancelButton);
			PopulateBottomItemControls(bottomControls);
			MainFormLayout.Items.InsertTableView(controls: bottomControls.ToArray()).CssClass = "dxhe-dialogButtonsContainer";
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainPanel.CssClass = "dxhe-dialogPreparing";
			MainPanel.EnableClientSideAPI = true;
			MainPanel.ClientSideEvents.Init = "ASPx.HtmlEditorClasses.Utils.OnDialogPanelInit";
			LoadingPanel.CssClass = "dxhe-dialogLoadingPanel";
			FormLayoutWrapper.DefaultButton = SubmitButtonId;
			FormLayoutWrapper.CssClass = "dxhe-dialogWrapperPanel";
			MainFormLayout.ClientInstanceName = GetClientInstanceName("FormLayout");
			RenderUtils.AppendDefaultDXClassName(MainFormLayout, GetDialogCssClassName());
			MainFormLayout.ApplyCommonSettings();
		}
		protected abstract void PopulateContentGroup(LayoutGroup group);
		protected abstract string GetDialogCssClassName();
		protected virtual void PopulateBottomItemControls(List<Control> controls) {
		}
		protected virtual ITemplate GetTopAreaTemplate() {
			return null;
		}
		protected virtual ITemplate GetBottomAreaTemplate() {
			return null;
		}
		ASPxButton CreateDialogButton(string initFunc, bool isSubmitButton) {
			ASPxButton result = new ASPxButton();
			result.CssClass = "dxheDlgFooterBtn";
			result.AutoPostBack = false;
			result.Text = ASPxHtmlEditorLocalizer.GetString(isSubmitButton ? ASPxHtmlEditorStringId.ButtonOk : ASPxHtmlEditorStringId.ButtonCancel);
			result.ClientSideEvents.Init = initFunc;
			result.ClientEnabled = !isSubmitButton;
			return result;
		}
	}
	[ToolboxItem(false)]
	public abstract class HtmlEditorDialogWithTemplates : HtmlEditorDialogBase {
		protected override ITemplate GetTopAreaTemplate() {
			return GetDialogSettings().TopAreaTemplate;
		}
		protected override ITemplate GetBottomAreaTemplate() {
			return GetDialogSettings().BottomAreaTemplate;
		}
		protected abstract HtmlEditorDialogSettingsBase GetDialogSettings();
	}
	[ToolboxItem(false)]
	public abstract class HtmlEditorInsertMediaDialogBase : HtmlEditorDialogWithTemplates, IStyleSettingsOwner {
		public const string YouTubeRegexp = @"^(https?\:\/\/)?(www\.)?((youtube(-nocookie)?\.com\/((watch\?v\=|embed\/|v\/)))|(youtu\.be\/))([^?\.]+)(?:.*)$";
		const bool MoreOptionsCheckedDefaultValue = false;
		HtmlEditorMediaDialogLocalizationSettings localizationSettings;
		public bool HasMoreOptionsSection { get { return GetMediaDialogSettings().ShowMoreOptionsButton; } }
		public bool HasDefaultSettings { get { return HasMoreOptionsSection && GetMediaDialogSettings().MoreOptionsSectionTemplate == null; } }
		public bool HasStylesSettings { get { return HasDefaultSettings && GetMediaDialogSettings().ShowStyleSettingsSection; } }
		public HtmlEditorMediaDialogLocalizationSettings LocalizationSettings {
			get {
				if(localizationSettings == null)
					localizationSettings = CreateLocalizationSettings();
				return localizationSettings;
			}
		}
		public abstract InsertMediaSourceType InsertMediaSourceType { get; }
		protected abstract HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings();
		protected abstract HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings();
		protected abstract void PopulateSettingsFormLayout(DialogFormLayoutBase result);
		protected abstract void PrepareDefaultSettingsSection();
		public ASPxHtmlEditorUploadSettingsBase GetUploadSettings() {
			return GetMediaDialogSettings().SettingsUploadInternal;
		}
		public virtual object GetCssClassItemsDataSource() {
			return GetMediaDialogSettings().CssClassItems;
		}
		protected HtmlEditorSelectorSettings GetSelectorSettings() {
			return GetMediaDialogSettings().SettingsSelectorInternal;
		}
		protected virtual void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) { }
		protected virtual void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) { }
		protected virtual void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) { }
		protected virtual void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) { }
		protected virtual void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) { }
		protected virtual void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) { }
		protected virtual void UploadControl_FileUploadCompleting(object sender, FileSavingEventArgs e) { }
		protected virtual void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) { }
		protected ASPxPageControl PageControl { get; private set; }
		protected MediaFileSelector MediaFileSelector { get; private set; }
		protected DialogFormLayoutBase SettingsFormLayout { get; private set; }
		protected MediaContentStyleSettingsControl StyleSettingsControl { get; private set; }
		protected ASPxCheckBox MoreOptionsCheckbox { get; private set; }
		protected ASPxLabel WidthLabel { get; private set; }
		protected ASPxLabel HeightLabel { get; private set; }
		protected ASPxComboBox PositionComboBox { get; private set; }
		protected void CreateWidthItem(LayoutItemCollection items) {
			items.CreateSpinEdit("Width", buffer: Editors);
			WidthLabel = items.CreateLabel(buffer: Editors);
		}
		protected void CreateHeightItem(LayoutItemCollection items) {
			items.CreateSpinEdit("Height", buffer: Editors);
			HeightLabel = items.CreateLabel(buffer: Editors);
		}
		protected void CreatePositionItem(LayoutItemCollection items, int colSpan = 1) {
			PositionComboBox = items.CreateComboBox("Position", buffer: Editors, colSpan: colSpan);
		}
		protected override HtmlEditorDialogSettingsBase GetDialogSettings() {
			return GetMediaDialogSettings();
		}
		protected override void PopulateContentGroup(LayoutGroup group) {
			group.ColCount = 2;
			MediaFileSelector = CreateMediaFileSelector();
			AssignMediaFileSelectorSettings();
			var mediaFileSelectorItem = group.Items.CreateItem("", MediaFileSelector);
			mediaFileSelectorItem.Width = Unit.Percentage(100);
			mediaFileSelectorItem.ShowCaption = DefaultBoolean.False;
			var div = new Panel();
			div.CssClass = "dxhe-dialogSettings";
			var item = group.Items.CreateItem("settingsGroup", div);
			item.ShowCaption = DefaultBoolean.False;
			item.ClientVisible = MoreOptionsCheckedDefaultValue;
			AddTemplateToControl(div, GetMediaDialogSettings().MoreOptionsSectionTemplate);
			if(HasDefaultSettings)
				CreateDefaultSettings(div);
		}
		private void CreateDefaultSettings(Panel div) {
			PageControl = new ASPxPageControl();
			div.Controls.Add(PageControl);
			SettingsFormLayout = CreateSettingsFormLayout();
			SettingsFormLayout.ApplyCommonSettings();
			PageControl.AddTab("Common Settings", "mainSettings", SettingsFormLayout);
			if(HasStylesSettings) {
				StyleSettingsControl = new MediaContentStyleSettingsControl(this);
				StyleSettingsControl.InitializeAsUserControl(this.Page);
				PageControl.AddTab("Style Settings", "styleSettings", StyleSettingsControl);
			}
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			if(!HasMoreOptionsSection)
				return;
			MoreOptionsCheckbox = new ASPxCheckBox();
			controls.Insert(0, MoreOptionsCheckbox);
		}
		protected virtual MediaFileSelector CreateMediaFileSelector() {
			return HtmlEditor.CreateMediaFileSelector();
		}
		protected DialogFormLayoutBase CreateSettingsFormLayout() {
			var result = new HtmlEditorDialogFormLayout();
			result.CssClass = "dxhe-mediaDialogMainSettings";
			result.ClientInstanceName = GetClientInstanceName("MainSettings");
			PopulateSettingsFormLayout(result);
			return result;
		}
		protected sealed override void PrepareChildControls() {
			base.PrepareChildControls();
			HtmlEditorInsertMediaDialogSettingsBase dialogSettings = GetMediaDialogSettings();
			RenderUtils.AppendDefaultDXClassName(MainFormLayout, "dxhe-mediaDialog");
			MainFormLayout.ClientInstanceName = GetClientInstanceName("MainFormLayout");
			if(HasMoreOptionsSection) {
				MoreOptionsCheckbox.ClientInstanceName = GetClientInstanceName("MoreOptions");
				MoreOptionsCheckbox.Checked = MoreOptionsCheckedDefaultValue;
				MoreOptionsCheckbox.Text = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.ShowMoreOptions);
			}
			if(HasDefaultSettings) {
				RenderUtils.AppendDefaultDXClassName(SettingsFormLayout, "dxhe-mediaDialogMainSettings");
				SettingsFormLayout.ApplyCommonSettings();
				PageControl.Width = Unit.Percentage(100);
				PageControl.Height = Unit.Percentage(100);
				PageControl.LocalizeField("mainSettings", LocalizationSettings.CommonSettingsTab);
				PageControl.ClientInstanceName = GetClientInstanceName("SettingsPageControl");
				if(HasStylesSettings)
					PageControl.LocalizeField("styleSettings", LocalizationSettings.StyleSettingsTab);
				SettingsFormLayout.LocalizeField("Position", LocalizationSettings.PositionLabel);
				SettingsFormLayout.LocalizeField("Width", LocalizationSettings.WidthLabel);
				SettingsFormLayout.LocalizeField("Height", LocalizationSettings.HeightLabel);
				WidthLabel.Text = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.PixelLabel);
				HeightLabel.Text = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.PixelLabel);
				PrepareDefaultSettingsSection();
			}
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { SubmitButton, CancelButton };
		}
		protected string GetHelpLink(ASPxHtmlEditorStringId text, ASPxHtmlEditorStringId helpText) {
			return ASPxHtmlEditorLocalizer.GetString(text) + GetHelpLink(helpText);
		}
		protected string GetHelpLink(ASPxHtmlEditorStringId stringId) {
			string helpText = ASPxHtmlEditorLocalizer.GetString(stringId);
			string cssPostfix = SettingsFormLayout.Styles.GetCssPostFix();
			return string.Format(" <span class='dxhe-dialogHelpItem'>[<a title='{0}' class='dxbButton_{1} dxbButtonSys' href='javascript:;'>?</a>]</span>", helpText, cssPostfix);
		}
		protected virtual void AssignMediaFileSelectorSettings() {
			MediaFileSelector.ClientInstanceName = GetClientInstanceName("SourceSelectControl");
			MediaFileSelector.Settings.PreviewType = GetPreviewType();
			MediaFileSelector.Settings.UseAbsoluteUrls = HtmlEditor.SettingsHtmlEditing.ResourcePathMode == ResourcePathMode.Absolute;
			HtmlEditorInsertMediaDialogSettingsBase dialogSettings = GetMediaDialogSettings();
			MediaFileSelector.Settings.AllowURLTab = dialogSettings.ShowInsertFromWebSectionInternal;
			MediaFileSelector.Settings.ShowSaveToServerCheckBox = dialogSettings.ShowSaveFileToServerButtonInternal;
			MediaFileSelector.Settings.AllowUploadTab = dialogSettings.ShowFileUploadSectionInternal;
			if(InsertMediaSourceType == InsertMediaSourceType.YouTube) {
				MediaFileSelector.Settings.ShowSaveToServerCheckBox = false;
				MediaFileSelector.Settings.AllowUploadTab = false;
				MediaFileSelector.Settings.AllowGalleryTab = false;
				MediaFileSelector.Settings.URLTabNullText = "http://www.youtube.com/...";
				MediaFileSelector.Settings.URLTabRegularExpression = YouTubeRegexp;
				MediaFileSelector.Settings.URLTabRegularExpressionErrorText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ValidationErrorText);
			} else {
				MediaFileSelector.Settings.PreviewUploadTipText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.PreviewText);
				MediaFileSelector.Settings.PreviewText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.EmptyResourcePreview);
				MediaFileSelector.Settings.GalleryTabText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.GalleryTabText); 
				MediaFileSelector.Settings.AllowedFileExtensionsText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.AllowedFileExtensionsText);
				MediaFileSelector.Settings.MaximumUploadFileSizeText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.MaximumUploadFileSizeText);
				MediaFileSelector.Settings.UploadTabText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.LocalSourceRadioButton);
				MediaFileSelector.Settings.SaveToServerText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.SaveToServer);
				MediaFileSelector.Settings.URLTabText = ASPxHtmlEditorLocalizer.GetString(LocalizationSettings.WebSourceRadioButton);
				MediaFileSelector.StylesFileManager.CopyFrom(HtmlEditor.StylesFileManager);
				MediaFileSelector.StylesFileManagerControl.CopyFrom(HtmlEditor.StylesFileManager.Control);
				MediaFileSelector.ImagesFileManager.CopyFrom(HtmlEditor.ImagesFileManager);
				var selectorSettings = GetSelectorSettings();
				MediaFileSelector.Settings.FileManagerClientSideEvents.Assign(selectorSettings.ClientSideEvents);
				MediaFileSelector.Settings.AllowGalleryTab = selectorSettings.Enabled;
				MediaFileSelector.Settings.FileManagerCommonSettings.Assign(selectorSettings.CommonSettings);
				MediaFileSelector.Settings.FileManagerEditingSettings.Assign(selectorSettings.EditingSettings);
				MediaFileSelector.Settings.FileManagerFoldersSettings.Assign(selectorSettings.FoldersSettings);
				MediaFileSelector.Settings.FileManagerToolbarSettings.Assign(selectorSettings.ToolbarSettings);
				MediaFileSelector.Settings.FileManagerUploadSettings.Assign(selectorSettings.UploadSettings);
				MediaFileSelector.Settings.FileManagerPermissionSettings.Assign(selectorSettings.PermissionSettings);
				MediaFileSelector.Settings.FileManagerFileListSettings.Assign(selectorSettings.FileListSettings);
				MediaFileSelector.Settings.FileManagerBreadcrumbsSettings.Assign(selectorSettings.BreadcrumbsSettings);
				MediaFileSelector.Settings.FileManagerRootFolderUrlPath = selectorSettings.RootFolderUrlPath;
				MediaFileSelector.Settings.FileManagerSettingsAmazon.Assign(selectorSettings.SettingsAmazon);
				MediaFileSelector.Settings.FileManagerSettingsAzure.Assign(selectorSettings.SettingsAzure);
				MediaFileSelector.Settings.FileManagerSettingsDropbox.Assign(selectorSettings.SettingsDropbox);
				MediaFileSelector.Settings.FileManagerSettingsDataSource.Assign(selectorSettings.SettingsDataSource);
				MediaFileSelector.Settings.FileManagerCustomFileSystemProvider = selectorSettings.CustomFileSystemProvider;
				MediaFileSelector.Settings.FileManagerCustomFileSystemProviderTypeName = selectorSettings.CustomFileSystemProviderTypeName;
				MediaFileSelector.Settings.FileManagerProviderType = selectorSettings.ProviderType;
				MediaFileSelector.Settings.FileManagerFolderCreating = new FileManagerFolderCreateEventHandler(FileManager_FolderCreating);
				MediaFileSelector.Settings.FileManagerItemDeleting = new FileManagerItemDeleteEventHandler(FileManager_ItemDeleting);
				MediaFileSelector.Settings.FileManagerItemCopying = new FileManagerItemCopyEventHandler(FileManager_ItemCopying);
				MediaFileSelector.Settings.FileManagerItemMoving = new FileManagerItemMoveEventHandler(FileManager_ItemMoving);
				MediaFileSelector.Settings.FileManagerItemRenaming = new FileManagerItemRenameEventHandler(FileManager_ItemRenaming);
				MediaFileSelector.Settings.FileManagerFileUploading = new FileManagerFileUploadEventHandler(FileManager_FileUploading);
				MediaFileSelector.Settings.FileManagerCustomThumbnail = new FileManagerThumbnailCreateEventHandler(FileManager_CustomThumbnail);
				MediaFileSelector.UploadControlFileUploadCompleting += UploadControl_FileUploadCompleting;
				var uploadSettings = GetUploadSettings();
				MediaFileSelector.Settings.UploadFolder = uploadSettings.UploadFolder;
				MediaFileSelector.Settings.UploadValidationSettings.Assign(uploadSettings.ValidationSettingsInternal);
				MediaFileSelector.Settings.UploadMode = uploadSettings.UseAdvancedUploadMode ? UploadControlUploadMode.Auto : UploadControlUploadMode.Standard;
				MediaFileSelector.Settings.AdvancedUploadModePacketSize = uploadSettings.AdvancedUploadModePacketSize;
				MediaFileSelector.Settings.AdvancedUploadModeTemporaryFolder = uploadSettings.AdvancedUploadModeTemporaryFolder;
				MediaFileSelector.Settings.UploadFolderUrlPath = uploadSettings.UploadFolderUrlPath;
			}
		}
		MediaFileSelectorPreviewType GetPreviewType() {
			switch(InsertMediaSourceType) {
				case Internal.InsertMediaSourceType.Audio:
					return MediaFileSelectorPreviewType.Audio;
				case Internal.InsertMediaSourceType.Flash:
					return MediaFileSelectorPreviewType.Object;
				case Internal.InsertMediaSourceType.Image:
					return MediaFileSelectorPreviewType.Image;
				case Internal.InsertMediaSourceType.Video:
					return MediaFileSelectorPreviewType.Video;
				default:
					return MediaFileSelectorPreviewType.NotSpecified;
			}
		}
		HtmlEditorDialogStyleLocalizationSettings IStyleSettingsOwner.LocalizationSettings { get { return LocalizationSettings; } }
		object IStyleSettingsOwner.GetCssClassItemsDataSource() {
			return GetCssClassItemsDataSource();
		}
	}
	public abstract class HtmlEditorInsertMediaDialog : HtmlEditorInsertMediaDialogBase {
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			result.ColCount = 2;
			CreateWidthItem(result.Items);
			CreateHeightItem(result.Items);
			CreatePositionItem(result.Items);
			result.AddEmptyItems(1, 2);
		}
	}
	public static class HtmlEditorDialogsHelper {
		public const string DialogMainValidationGroup = "mediaDialogValidationGroup";
		public static void LocalizeField(this ASPxPageControl control, string fieldName, ASPxHtmlEditorStringId stringId) {
			control.TabPages.FindByName(fieldName).Text = ASPxHtmlEditorLocalizer.GetString(stringId);
		}
		public static void SetupRequiredSettings(this ValidationSettings validationSettings) {
			validationSettings.ValidationGroup = DialogMainValidationGroup;
			validationSettings.RequiredField.IsRequired = true;
			validationSettings.Display = Display.Dynamic;
			validationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			validationSettings.ErrorTextPosition = ErrorTextPosition.Right;
			validationSettings.RequiredField.ErrorText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RequiredFieldError);
		}
		public static ListEditItem AddItem(this ASPxComboBox comboBox, ASPxHtmlEditorStringId stringId, string value) {
			return comboBox.Items.Add(ASPxHtmlEditorLocalizer.GetString(stringId), value);
		}
	}
}
