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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class InsertImageForm : HtmlEditorInsertMediaDialogBase {
		protected bool ShowSaveFileToServerButton { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.ShowSaveFileToServerButton && HtmlEditor.Settings.AllowInsertDirectImageUrls; } }
		protected ASPxCheckBox WrapCheckBox { get; private set; }
		protected ASPxTextBox DescriptionTextBox { get; private set; }
		protected ASPxComboBox SizeComboBox { get; private set; }
		protected ASPxCheckBox CreateThumbnailCheckBox { get; private set; }
		protected ASPxTextBox ImageNameTextBox { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertImageDialog";
		}
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			result.AlignItemCaptionsInAllGroups = true;
			SizeComboBox = result.Items.CreateComboBox("Sizes", colSpan: result.ColCount);
			LayoutGroup group = result.Items.CreateGroup("SizeGroup");
			group.ClientVisible = false;
			group.ColCount = 4;
			group.CssClass = "dxhe-insertImageSizeGroup";
			CreateWidthItem(group.Items);
			CreateContsrainItem(group.Items);
			CreateResetImageItem(group.Items);
			CreateHeightItem(group.Items);
			CreateThumbnailCheckBox = group.Items.CreateCheckBox("CreateThumbnail", colSpan: group.ColCount, showCaption: true);
			CreateImageNameItem(group.Items);
			result.Items.Add<EmptyLayoutItem>("", "EmptyItem").ColSpan = result.ColCount;
			CreatePositionItem(result.Items, result.ColCount);
			WrapCheckBox = result.Items.CreateCheckBox("Wrap", colSpan: result.ColCount);
			result.AddEmptyItems(result.ColCount);
			DescriptionTextBox = result.Items.CreateTextBox("Description", location: LayoutItemCaptionLocation.Top, colSpan: result.ColCount);
		}
		void CreateContsrainItem(LayoutItemCollection items) {
			LayoutItem contsrainItem = items.CreateItem("ContsrainItem", CreateConstrainTable());
			contsrainItem.ShowCaption = DefaultBoolean.False;
			contsrainItem.RowSpan = 2;
		}
		void CreateResetImageItem(LayoutItemCollection items) {
			Image image = RenderUtils.CreateImage();
			image.ID = GetClientInstanceName("ResetImage");
			image.Style.Add("cursor", "pointer");
			image.ClientIDMode = System.Web.UI.ClientIDMode.Static;
			var imageProperties = HtmlEditor.GetInsertImageDialogResetImageProperties();
			imageProperties.AssignToControl(image, false);
			LayoutItem resetImageItem = items.CreateItem("ResetItem", image);
			resetImageItem.RowSpan = 2;
			resetImageItem.ShowCaption = DefaultBoolean.False;
		}
		void CreateImageNameItem(LayoutItemCollection items) {
			ImageNameTextBox = new ASPxTextBox();
			ImageNameTextBox.ClientInstanceName = GetClientInstanceName("ThumbnailName");
			ImageNameTextBox.CssClass = DialogsHelper.FullWidthCssClass;
			LayoutItem ImageNameTextBoxItem = items.CreateItem("ThumbnailName", ImageNameTextBox);
			ImageNameTextBoxItem.ColSpan = 4;
			ImageNameTextBoxItem.ShowCaption = DefaultBoolean.False;
			ImageNameTextBoxItem.RequiredMarkDisplayMode = FieldRequiredMarkMode.Hidden;
		}
		protected override void UploadControl_FileUploadCompleting(object sender, FileSavingEventArgs e) {
			HtmlEditor.RaiseImageFileSaving(e);
		}
		protected override void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			HtmlEditor.RaiseImageSelectorFileUploading(e);
		}
		protected override void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			HtmlEditor.RaiseImageSelectorFolderCreating(e);
		}
		protected override void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			HtmlEditor.RaiseImageSelectorItemDeleting(e);
		}
		protected override void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			HtmlEditor.RaiseImageSelectorItemMoving(e);
		}
		protected override void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			HtmlEditor.RaiseImageSelectorItemRenaming(e);
		}
		protected override void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			HtmlEditor.RaiseImageSelectorItemCopying(e);
		}
		protected override void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) {
			HtmlEditor.RaiseImageSelectorCustomThumbnail(e);
		}
		Control CreateConstrainTable() {
			var table = RenderUtils.CreateTable();
			AddTableCell(table, CreateImage(HtmlEditor.GetInsertImageDialogConstrainProportionsTop()));
			AddTableCell(table, CreateSwitcherOn(), CreateSwitcherOff());
			AddTableCell(table, CreateImage(HtmlEditor.GetInsertImageDialogConstrainProportionsBottom()));
			return table;
		}
		Image CreateSwitcherOn() {
			Image imgSwitcherOn = CreateImage(HtmlEditor.GetInsertImageDialogConstrainProportionsMiddleOn());
			imgSwitcherOn.Attributes.Add("onclick", "ASPx.HtmlEditorClasses.Utils.OnConstrainSizeClick(event)");
			imgSwitcherOn.Style.Add("cursor", "pointer");
			return imgSwitcherOn;
		}
		Image CreateSwitcherOff() {
			Image imgSwitcherOff = CreateImage(HtmlEditor.GetInsertImageDialogConstrainProportionsMiddleOff());
			imgSwitcherOff.Attributes.Add("onclick", "ASPx.HtmlEditorClasses.Utils.OnConstrainSizeClick(event)");
			imgSwitcherOff.Style.Add("display", "none");
			imgSwitcherOff.Style.Add("cursor", "pointer");
			return imgSwitcherOff;
		}
		Image CreateImage(ImageProperties properties) {
			Image imgSwitcherOn = RenderUtils.CreateImage();
			properties.AssignToControl(imgSwitcherOn, false);
			return imgSwitcherOn;
		}
		void AddTableCell(InternalTable table, params Control[] cellControls) {
			var tr = RenderUtils.CreateTableRow();
			var td = RenderUtils.CreateTableCell();
			foreach(var control in cellControls)
				td.Controls.Add(control);
			table.Rows.Add(tr);
			tr.Cells.Add(td);
		}
		public override InsertMediaSourceType InsertMediaSourceType {
			get { return InsertMediaSourceType.Image; }
		}
		protected override void AssignMediaFileSelectorSettings() {
			base.AssignMediaFileSelectorSettings();
			MediaFileSelector.Settings.ShowSaveToServerCheckBox = ShowSaveFileToServerButton;
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertImageDialog;
		}
		protected override HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorMediaDialogLocalizationSettings() {
				EmptyResourcePreview = ASPxHtmlEditorStringId.InsertImage_Preview,
				EnterUrl = ASPxHtmlEditorStringId.InsertImage_EnterUrl,
				LocalSourceRadioButton = ASPxHtmlEditorStringId.InsertImage_FromLocal,
				SaveToServer = ASPxHtmlEditorStringId.InsertImage_SaveToServer,
				UploadFile = ASPxHtmlEditorStringId.InsertImage_UploadInstructions,
				WebSourceRadioButton = ASPxHtmlEditorStringId.InsertImage_FromWeb,
				ShowMoreOptions = ASPxHtmlEditorStringId.InsertImage_MoreOptions,
				PopupHeader = ASPxHtmlEditorStringId.SelectImage,
				Margins = ASPxHtmlEditorStringId.ImageProps_Margins,
				TopMargin = ASPxHtmlEditorStringId.ImageProps_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.ImageProps_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.ImageProps_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.ImageProps_MarginRight,
				Border = ASPxHtmlEditorStringId.ImageProps_Border,
				BorderWidth = ASPxHtmlEditorStringId.ImageProps_BorderWidth,
				BorderStyle = ASPxHtmlEditorStringId.ImageProps_BorderStyle,
				BorderColor = ASPxHtmlEditorStringId.ImageProps_BorderColor,
				CssClassName = ASPxHtmlEditorStringId.ImageProps_CssClass,
				PixelLabel = ASPxHtmlEditorStringId.ImageProps_Pixels,
				HeightLabel = ASPxHtmlEditorStringId.ImageProps_Height,
				PositionLabel = ASPxHtmlEditorStringId.ImageProps_Position,
				WidthLabel = ASPxHtmlEditorStringId.ImageProps_Width,
				StyleSettingsTab = ASPxHtmlEditorStringId.InsertImage_StyleSettingsTabName,
				CommonSettingsTab = ASPxHtmlEditorStringId.InsertImage_CommonSettingsTabName,
				PreviewText = ASPxHtmlEditorStringId.InsertImage_PreviewText,
				GalleryTabText = ASPxHtmlEditorStringId.InsertImage_GalleryTabText,
				AllowedFileExtensionsText = ASPxHtmlEditorStringId.InsertImage_AllowedFileExtensionsText,
				MaximumUploadFileSizeText = ASPxHtmlEditorStringId.InsertImage_MaximumUploadFileSizeText
			};
		}
		public override object GetCssClassItemsDataSource() {
			return HtmlEditor.SettingsDialogs.InsertImageDialog.CssClassItems;
		}
		protected override void PrepareDefaultSettingsSection() {
			SettingsFormLayout.LocalizeField("Sizes", ASPxHtmlEditorStringId.ImageProps_Size);
			SettingsFormLayout.LocalizeField("Description", ASPxHtmlEditorStringId.ImageProps_Description);
			WrapCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertImage_UseFloat);
			ImageNameTextBox.ValidationSettings.SetupRequiredSettings();
			CreateThumbnailCheckBox.Text = GetHelpLink(ASPxHtmlEditorStringId.ImageProps_CreateThumbnail, ASPxHtmlEditorStringId.ImageProps_CreateThumbnailTooltip);
			CreateThumbnailCheckBox.EncodeHtml = false;
			ImageNameTextBox.Caption = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ImageProps_NewImageName);
			PositionComboBox.Items.Add("", "");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.ImageProps_PositionLeft, "left");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.ImageProps_PositionCenter, "center");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.ImageProps_PositionRight, "right");
			SizeComboBox.AddItem(ASPxHtmlEditorStringId.ImageProps_OriginalSize, "original").Selected = true;
			SizeComboBox.AddItem(ASPxHtmlEditorStringId.ImageProps_CustomSize, "custom").Selected = false;
		}
	}
}
