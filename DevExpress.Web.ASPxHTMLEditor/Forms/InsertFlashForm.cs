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

using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class InsertFlashForm : HtmlEditorInsertMediaDialog {
		protected ASPxCheckBox AllowFullscreenCheckBox { get; private set; }
		protected ASPxCheckBox AutoPlayCheckBox { get; private set; }
		protected ASPxCheckBox EnableFlashMenuCheckBox { get; private set; }
		protected ASPxCheckBox LoopCheckBox { get; private set; }
		protected ASPxComboBox QualityComboBox { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertFlashDialog";
		}
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			base.PopulateSettingsFormLayout(result);
			QualityComboBox = result.Items.CreateComboBox("Quality", buffer: Editors);
			result.AddEmptyItems(1, 2);
			AutoPlayCheckBox = result.Items.CreateCheckBox("AutoPlay", buffer: Editors, colSpan: 2);
			EnableFlashMenuCheckBox = result.Items.CreateCheckBox("EnableFlashMenu", buffer: Editors, colSpan: 2);
			LoopCheckBox = result.Items.CreateCheckBox("Loop", buffer: Editors, colSpan: 2);
			AllowFullscreenCheckBox = result.Items.CreateCheckBox("AllowFullscreen", buffer: Editors, colSpan: 2);
			AllowFullscreenCheckBox.Checked = true;
			AutoPlayCheckBox.Checked = true;
			EnableFlashMenuCheckBox.Checked = true;
			LoopCheckBox.Checked = true;
		}
		protected override void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			HtmlEditor.RaiseFlashSelectorFileUploading(e);
		}
		protected override void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			HtmlEditor.RaiseFlashSelectorFolderCreating(e);
		}
		protected override void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			HtmlEditor.RaiseFlashSelectorItemDeleting(e);
		}
		protected override void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			HtmlEditor.RaiseFlashSelectorItemMoving(e);
		}
		protected override void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			HtmlEditor.RaiseFlashSelectorItemRenaming(e);
		}
		protected override void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			HtmlEditor.RaiseFlashSelectorItemCopying(e);
		}
		protected override void UploadControl_FileUploadCompleting(object sender, FileSavingEventArgs e) {
			HtmlEditor.RaiseFlashFileSaving(e);
		}
		protected override void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) {
			HtmlEditor.RaiseFlashSelectorCustomThumbnail(e);
		}
		public override InsertMediaSourceType InsertMediaSourceType {
			get { return InsertMediaSourceType.Flash; }
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertFlashDialog;
		}
		protected override HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorMediaDialogLocalizationSettings() {
				EmptyResourcePreview = ASPxHtmlEditorStringId.InsertFlash_Preview,
				EnterUrl = ASPxHtmlEditorStringId.InsertFlash_EnterUrl,
				LocalSourceRadioButton = ASPxHtmlEditorStringId.InsertFlash_FromLocal,
				SaveToServer = ASPxHtmlEditorStringId.InsertFlash_SaveToServer,
				UploadFile = ASPxHtmlEditorStringId.InsertFlash_UploadInstructions,
				WebSourceRadioButton = ASPxHtmlEditorStringId.InsertFlash_FromWeb,
				ShowMoreOptions = ASPxHtmlEditorStringId.InsertFlash_MoreOptions,
				PopupHeader = ASPxHtmlEditorStringId.SelectFlash,
				Margins = ASPxHtmlEditorStringId.InsertFlash_Margins,
				TopMargin = ASPxHtmlEditorStringId.InsertFlash_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.InsertFlash_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.InsertFlash_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.InsertFlash_MarginRight,
				Border = ASPxHtmlEditorStringId.InsertFlash_Border,
				BorderWidth = ASPxHtmlEditorStringId.InsertFlash_BorderWidth,
				BorderColor = ASPxHtmlEditorStringId.InsertFlash_BorderColor,
				BorderStyle = ASPxHtmlEditorStringId.InsertFlash_BorderStyle,
				CssClassName = ASPxHtmlEditorStringId.InsertFlash_CssClass,
				StyleSettingsTab = ASPxHtmlEditorStringId.InsertFlash_StyleSettingsTabName,
				CommonSettingsTab = ASPxHtmlEditorStringId.InsertFlash_CommonSettingsTabName,
				WidthLabel = ASPxHtmlEditorStringId.InsertFlash_Width,
				HeightLabel = ASPxHtmlEditorStringId.InsertFlash_Height,
				PixelLabel = ASPxHtmlEditorStringId.InsertFlash_Pixels,
				PositionLabel = ASPxHtmlEditorStringId.InsertFlash_Position,
				PreviewText = ASPxHtmlEditorStringId.InsertFlash_PreviewText,
				GalleryTabText = ASPxHtmlEditorStringId.InsertFlash_GalleryTabText,
				AllowedFileExtensionsText = ASPxHtmlEditorStringId.InsertFlash_AllowedFileExtensionsText,
				MaximumUploadFileSizeText = ASPxHtmlEditorStringId.InsertFlash_MaximumUploadFileSizeText
			};
		}
		protected override void PrepareDefaultSettingsSection() {
			SettingsFormLayout.LocalizeField("Quality", ASPxHtmlEditorStringId.InsertFlash_Quality);
			AllowFullscreenCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash_AllowFullscreen);
			AutoPlayCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash_AutoPlay);
			EnableFlashMenuCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash_EnableFlashMenu);
			LoopCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash_Loop);
			PositionComboBox.Items.Add("", "");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_PositionLeft, "left");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_PositionCenter, "center");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_PositionRight, "right");
			QualityComboBox.Items.Add("", "");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityBest, "best");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityHigh, "high");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityAutoHigh, "autohigh");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityMedium, "medium");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityLow, "low");
			QualityComboBox.AddItem(ASPxHtmlEditorStringId.InsertFlash_QualityAutoLow, "autolow");
		}
	}
}
