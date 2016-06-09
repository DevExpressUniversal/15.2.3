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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class InsertAudioForm : HtmlEditorInsertMediaDialog {
		protected ASPxCheckBox AutoPlayCheckBox { get; private set; }
		protected ASPxCheckBox LoopCheckBox { get; private set; }
		protected ASPxCheckBox ShowControlsCheckBox { get; private set; }
		protected ASPxComboBox PreloadComboBox { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertAudioDialog";
		}
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			base.PopulateSettingsFormLayout(result);
			PreloadComboBox = result.Items.CreateComboBox("Preload", buffer: Editors);
			result.AddEmptyItems(1, 2);
			AutoPlayCheckBox = result.Items.CreateCheckBox("AutoPlay", buffer: Editors, colSpan: 2);
			LoopCheckBox = result.Items.CreateCheckBox("Loop", buffer: Editors, colSpan: 2);
			ShowControlsCheckBox = result.Items.CreateCheckBox("ShowControls", buffer: Editors, colSpan: 2);
			ShowControlsCheckBox.Checked = true;
		}
		protected override void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			HtmlEditor.RaiseAudioSelectorFileUploading(e);
		}
		protected override void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			HtmlEditor.RaiseAudioSelectorFolderCreating(e);
		}
		protected override void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			HtmlEditor.RaiseAudioSelectorItemDeleting(e);
		}
		protected override void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			HtmlEditor.RaiseAudioSelectorItemMoving(e);
		}
		protected override void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			HtmlEditor.RaiseAudioSelectorItemRenaming(e);
		}
		protected override void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			HtmlEditor.RaiseAudioSelectorItemCopying(e);
		}
		protected override void UploadControl_FileUploadCompleting(object sender, FileSavingEventArgs e) {
			HtmlEditor.RaiseAudioFileSaving(e);
		}
		public override InsertMediaSourceType InsertMediaSourceType {
			get { return InsertMediaSourceType.Audio; }
		}
		protected override void FileManager_CustomThumbnail(object sender, FileManagerThumbnailCreateEventArgs e) {
			HtmlEditor.RaiseAudioSelectorCustomThumbnail(e);
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertAudioDialog;
		}
		protected override HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorMediaDialogLocalizationSettings() {
				EmptyResourcePreview = ASPxHtmlEditorStringId.InsertAudio_Preview,
				EnterUrl = ASPxHtmlEditorStringId.InsertAudio_EnterUrl,
				LocalSourceRadioButton = ASPxHtmlEditorStringId.InsertAudio_FromLocal,
				SaveToServer = ASPxHtmlEditorStringId.InsertAudio_SaveToServer,
				UploadFile = ASPxHtmlEditorStringId.InsertAudio_UploadInstructions,
				WebSourceRadioButton = ASPxHtmlEditorStringId.InsertAudio_FromWeb,
				ShowMoreOptions = ASPxHtmlEditorStringId.InsertAudio_MoreOptions,
				PopupHeader = ASPxHtmlEditorStringId.SelectAudio,
				Margins = ASPxHtmlEditorStringId.InsertAudio_Margins,
				TopMargin = ASPxHtmlEditorStringId.InsertAudio_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.InsertAudio_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.InsertAudio_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.InsertAudio_MarginRight,
				Border = ASPxHtmlEditorStringId.InsertAudio_Border,
				BorderWidth = ASPxHtmlEditorStringId.InsertAudio_BorderWidth,
				BorderColor = ASPxHtmlEditorStringId.InsertAudio_BorderColor,
				BorderStyle = ASPxHtmlEditorStringId.InsertAudio_BorderStyle,
				CssClassName = ASPxHtmlEditorStringId.InsertAudio_CssClass,
				StyleSettingsTab = ASPxHtmlEditorStringId.InsertAudio_StyleSettingsTabName,
				CommonSettingsTab = ASPxHtmlEditorStringId.InsertAudio_CommonSettingsTabName,
				WidthLabel = ASPxHtmlEditorStringId.InsertAudio_Width,
				HeightLabel = ASPxHtmlEditorStringId.InsertAudio_Height,
				PixelLabel = ASPxHtmlEditorStringId.InsertAudio_Pixels,
				PositionLabel = ASPxHtmlEditorStringId.InsertAudio_Position,
				PreviewText = ASPxHtmlEditorStringId.InsertAudio_PreviewText,
				GalleryTabText = ASPxHtmlEditorStringId.InsertAudio_GalleryTabText,
				AllowedFileExtensionsText = ASPxHtmlEditorStringId.InsertAudio_AllowedFileExtensionsText,
				MaximumUploadFileSizeText = ASPxHtmlEditorStringId.InsertAudio_MaximumUploadFileSizeText
			};
		}
		protected override void PrepareDefaultSettingsSection() {
			SettingsFormLayout.LocalizeField("Preload", ASPxHtmlEditorStringId.InsertAudio_Preload, GetHelpLink(ASPxHtmlEditorStringId.InsertAudio_PreloadHelpText));
			ShowControlsCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio_ShowControls);
			AutoPlayCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio_AutoPlay);
			LoopCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio_Loop);
			PositionComboBox.Items.Add("", "");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PositionLeft, "left");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PositionCenter, "center");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PositionRight, "right");
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PreloadNone, "none").Selected = true;
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PreloadMetadata, "metadata");
			PreloadComboBox.AddItem(ASPxHtmlEditorStringId.InsertAudio_PreloadAuto, "auto");
			SettingsFormLayout.EncodeHtml = false;
		}
	}
}
