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
	public class InsertVideoForm : HtmlEditorInsertMediaDialog {
		protected ASPxCheckBox AutoPlayCheckBox { get; private set; }
		protected ASPxCheckBox LoopCheckBox { get; private set; }
		protected ASPxCheckBox ShowControlsCheckBox { get; private set; }
		protected ASPxComboBox PreloadComboBox { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertVideoDialog";
		}
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			base.PopulateSettingsFormLayout(result);
			PreloadComboBox = result.Items.CreateComboBox("Preload", buffer: Editors);
			result.AddEmptyItems(1);
			result.Items.CreateTextBox("Poster", buffer: Editors);
			result.AddEmptyItems(1, 2);
			AutoPlayCheckBox = result.Items.CreateCheckBox("AutoPlay", buffer: Editors, colSpan: 2);
			LoopCheckBox = result.Items.CreateCheckBox("Loop", buffer: Editors, colSpan: 2);
			ShowControlsCheckBox = result.Items.CreateCheckBox("ShowControls", buffer: Editors, colSpan: 2);
			ShowControlsCheckBox.Checked = true;
		}
		protected override void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			HtmlEditor.RaiseVideoSelectorFileUploading(e);
		}
		protected override void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			HtmlEditor.RaiseVideoSelectorFolderCreating(e);
		}
		protected override void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			HtmlEditor.RaiseVideoSelectorItemDeleting(e);
		}
		protected override void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			HtmlEditor.RaiseVideoSelectorItemMoving(e);
		}
		protected override void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			HtmlEditor.RaiseVideoSelectorItemRenaming(e);
		}
		protected override void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			HtmlEditor.RaiseVideoSelectorItemCopying(e);
		}
		protected override void UploadControl_FileUploadCompleting(object sender, FileSavingEventArgs e) {
			HtmlEditor.RaiseVideoFileSaving(e);
		}
		protected override void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) {
			HtmlEditor.RaiseVideoSelectorCustomThumbnail(e);
		}
		public override InsertMediaSourceType InsertMediaSourceType {
			get { return InsertMediaSourceType.Video; }
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertVideoDialog;
		}
		protected override HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorMediaDialogLocalizationSettings() {
				EmptyResourcePreview = ASPxHtmlEditorStringId.InsertVideo_Preview,
				EnterUrl = ASPxHtmlEditorStringId.InsertVideo_EnterUrl,
				LocalSourceRadioButton = ASPxHtmlEditorStringId.InsertVideo_FromLocal,
				SaveToServer = ASPxHtmlEditorStringId.InsertVideo_SaveToServer,
				UploadFile = ASPxHtmlEditorStringId.InsertVideo_UploadInstructions,
				WebSourceRadioButton = ASPxHtmlEditorStringId.InsertVideo_FromWeb,
				ShowMoreOptions = ASPxHtmlEditorStringId.InsertVideo_MoreOptions,
				PopupHeader = ASPxHtmlEditorStringId.SelectVideo,
				Margins = ASPxHtmlEditorStringId.InsertVideo_Margins,
				TopMargin = ASPxHtmlEditorStringId.InsertVideo_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.InsertVideo_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.InsertVideo_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.InsertVideo_MarginRight,
				Border = ASPxHtmlEditorStringId.InsertVideo_Border,
				BorderWidth = ASPxHtmlEditorStringId.InsertVideo_BorderWidth,
				BorderStyle = ASPxHtmlEditorStringId.InsertVideo_BorderStyle,
				BorderColor = ASPxHtmlEditorStringId.InsertVideo_BorderColor,
				CssClassName = ASPxHtmlEditorStringId.InsertVideo_CssClass,
				StyleSettingsTab = ASPxHtmlEditorStringId.InsertVideo_StyleSettingsTabName,
				CommonSettingsTab = ASPxHtmlEditorStringId.InsertVideo_CommonSettingsTabName,
				WidthLabel = ASPxHtmlEditorStringId.InsertVideo_Width,
				HeightLabel = ASPxHtmlEditorStringId.InsertVideo_Height,
				PixelLabel = ASPxHtmlEditorStringId.InsertVideo_Pixels,
				PositionLabel = ASPxHtmlEditorStringId.InsertVideo_Position,
				PreviewText = ASPxHtmlEditorStringId.InsertVideo_PreviewText,
				GalleryTabText = ASPxHtmlEditorStringId.InsertVideo_GalleryTabText,
				AllowedFileExtensionsText = ASPxHtmlEditorStringId.InsertVideo_AllowedFileExtensionsText,
				MaximumUploadFileSizeText = ASPxHtmlEditorStringId.InsertVideo_MaximumUploadFileSizeText
			};
		}
		protected override void PrepareDefaultSettingsSection() {
			SettingsFormLayout.LocalizeField("Poster", ASPxHtmlEditorStringId.InsertVideo_Poster, GetHelpLink(ASPxHtmlEditorStringId.InsertVideo_PosterHelpText));
			SettingsFormLayout.LocalizeField("Preload", ASPxHtmlEditorStringId.InsertVideo_Preload, GetHelpLink(ASPxHtmlEditorStringId.InsertVideo_PreloadHelpText));
			ShowControlsCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo_ShowControls);
			AutoPlayCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo_AutoPlay);
			LoopCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo_Loop);
			PositionComboBox.Items.Add("", "");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PositionLeft, "left");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PositionCenter, "center");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PositionRight, "right");
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PreloadNone, "none").Selected = true;
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PreloadMetadata, "metadata");
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertVideo_PreloadAuto, "auto");
			SettingsFormLayout.EncodeHtml = false;
		}
	}
}
