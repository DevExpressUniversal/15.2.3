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
	public class InsertYoutubeVideoForm : HtmlEditorInsertMediaDialog {
		protected ASPxCheckBox ShowControlsCheckBox { get; private set; }
		protected ASPxCheckBox ConfidentModeCheckBox { get; private set; }
		protected ASPxCheckBox ShowSameVideosCheckBox { get; private set; }
		protected ASPxCheckBox ShowVideoNameCheckBox { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxhe-insertYouTubeVideoDialog";
		}
		protected override void PopulateSettingsFormLayout(DialogFormLayoutBase result) {
			base.PopulateSettingsFormLayout(result);
			ShowSameVideosCheckBox = result.Items.CreateCheckBox("ShowSameVideos", buffer: Editors, colSpan: 2);
			ShowSameVideosCheckBox.Checked = true;
			ShowControlsCheckBox = result.Items.CreateCheckBox("ShowControls", buffer: Editors, colSpan: 2);
			ShowControlsCheckBox.Checked = true;
			ShowVideoNameCheckBox = result.Items.CreateCheckBox("ShowVideoName", buffer: Editors, colSpan: 2);
			ShowVideoNameCheckBox.Checked = true;
			ConfidentModeCheckBox = result.Items.CreateCheckBox("ConfidentMode", buffer: Editors, colSpan: 2);
			ConfidentModeCheckBox.EncodeHtml = false;
		}
		public override InsertMediaSourceType InsertMediaSourceType {
			get { return InsertMediaSourceType.YouTube; }
		}
		protected override HtmlEditorInsertMediaDialogSettingsBase GetMediaDialogSettings() {
			return HtmlEditor.SettingsDialogs.InsertYouTubeVideoDialog;
		}
		protected override HtmlEditorMediaDialogLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorMediaDialogLocalizationSettings() {
				EmptyResourcePreview = ASPxHtmlEditorStringId.InsertYouTubeVideo_Preview,
				EnterUrl = ASPxHtmlEditorStringId.InsertYouTubeVideo_EnterUrl,
				ShowMoreOptions = ASPxHtmlEditorStringId.InsertYouTubeVideo_MoreOptions,
				Margins = ASPxHtmlEditorStringId.InsertYouTubeVideo_Margins,
				TopMargin = ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginRight,
				Border = ASPxHtmlEditorStringId.InsertYouTubeVideo_Border,
				BorderWidth = ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderWidth,
				BorderStyle = ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderStyle,
				BorderColor = ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderColor,
				CssClassName = ASPxHtmlEditorStringId.InsertYouTubeVideo_CssClass,
				StyleSettingsTab = ASPxHtmlEditorStringId.InsertYouTubeVideo_StyleSettingsTabName,
				CommonSettingsTab = ASPxHtmlEditorStringId.InsertYouTubeVideo_CommonSettingsTabName,
				HeightLabel = ASPxHtmlEditorStringId.InsertYouTubeVideo_Height,
				WidthLabel = ASPxHtmlEditorStringId.InsertYouTubeVideo_Width,
				PixelLabel = ASPxHtmlEditorStringId.InsertYouTubeVideo_Pixels,
				PositionLabel = ASPxHtmlEditorStringId.InsertYouTubeVideo_Position
			};
		}
		protected override void PrepareDefaultSettingsSection() {
			ShowControlsCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowControls);
			ConfidentModeCheckBox.Text = GetHelpLink(ASPxHtmlEditorStringId.InsertYouTubeVideo_SecureMode, ASPxHtmlEditorStringId.InsertYouTubeVideo_SecureModeHelpText);
			ShowSameVideosCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowSameVideos);
			ShowVideoNameCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowVideoName);
			PositionComboBox.Items.Add("", "");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionLeft, "left");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionCenter, "center");
			PositionComboBox.AddItem(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionRight, "right");
		}
	}
}
