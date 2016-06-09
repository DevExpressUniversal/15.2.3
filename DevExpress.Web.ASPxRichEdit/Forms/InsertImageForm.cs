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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class InsertImageForm : RichEditDialogBase {
		protected RichEditInsertPictureDialogSettings SettingsInsertPictureDialog {
			get { return RichEdit.SettingsDialogs.InsertPictureDialog; }
		}
		protected DialogFormLayoutBase ContentLayout { get; private set; }
		protected ASPxRoundPanel ContentPanel { get; private set; }
		protected ASPxTextBox ImageUrlTextBox { get; private set; }
		protected ASPxLabel PreviewTextLabel { get; private set; }
		protected ASPxUploadControl UploadControl { get; private set; }
		protected ASPxRadioButtonList NavigationList { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			if(SettingsInsertPictureDialog.ShowFileUploadSection)
				NavigationList = group.Items.CreateEditor<ASPxRadioButtonList>("RblNavigation", showCaption: false, buffer: Editors);
			ContentPanel = new ASPxRoundPanel();
			ContentPanel.ShowHeader = false;
			LayoutItem content = group.Items.CreateItem("generalContent", ContentPanel);
			content.ShowCaption = DefaultBoolean.False;
			ContentLayout = new RichEditDialogFormLayout();
			ContentLayout.CssClass = "dxreDlgInsertImagePanel";
			ContentLayout.ApplyCommonSettings();
			ContentLayout.ClientInstanceName = GetClientInstanceName("InsertImageFormLayout");
			ImageUrlTextBox = ContentLayout.Items.CreateTextBox("TxbInsertImageUrl", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			ContentLayout.Items.CreateItem("ImagePreview", CreatePreviewTable()).ShowCaption = DefaultBoolean.False;
			if(SettingsInsertPictureDialog.ShowFileUploadSection) {
				UploadControl = CreateUploadControl();
				UploadControl.ShowClearFileSelectionButton = false;
				UploadControl.FileUploadComplete += new EventHandler<FileUploadCompleteEventArgs>(uplImage_FileUploadComplete);
				UploadControl.ClientInstanceName = GetClientInstanceName("UplImage");
				UploadControl.CssClass = "dx-justification";
				LayoutItem upload = ContentLayout.Items.CreateItem("UploadControl", UploadControl);
				upload.ClientVisible = false;
				upload.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			}
			ContentPanel.Controls.Add(ContentLayout);
			ASPxHiddenField hidden = new ASPxHiddenField();
			hidden.ClientInstanceName = GetClientInstanceName("HiddenField");
			ContentPanel.Controls.Add(hidden);
		}
		Control CreatePreviewTable() {
			var table = RenderUtils.CreateTable();
			var tr = RenderUtils.CreateTableRow();
			var td = RenderUtils.CreateTableCell();
			PreviewTextLabel = new ASPxLabel();
			PreviewTextLabel.ClientIDMode = ClientIDMode.Static;
			PreviewTextLabel.ID = RichEdit.ClientID + "_dxInsertImagePreviewText";
			td.Controls.Add(PreviewTextLabel);
			ASPxImage previewImage = new ASPxImage();
			previewImage.ClientIDMode = ClientIDMode.Static;
			previewImage.ID = RichEdit.ClientID + "_dxInsertImagePreviewImage";
			previewImage.ClientVisible = false;
			td.Controls.Add(previewImage);
			table.Rows.Add(tr);
			tr.Cells.Add(td);
			table.CssClass = "dxreDlgImagePreview";
			return table;
		}
		protected virtual ASPxUploadControl CreateUploadControl() {
			return new ASPxUploadControl();
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			if(SettingsInsertPictureDialog.ShowFileUploadSection)
				PrepareNavigation();
			SetupValidationSettings();
		}
		void PrepareNavigation() {
			NavigationList.CssClass = "dxre-dialogRadioNavigation";
			NavigationList.RepeatColumns = 2;
			NavigationList.RepeatDirection = RepeatDirection.Horizontal;
			NavigationList.Border.BorderStyle = BorderStyle.None;
			NavigationList.EnableFocusedStyle = false;
			NavigationList.ValueType = typeof(int);
			NavigationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertImage_FromWeb), 0);
			NavigationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertImage_FromLocal), 1);
			NavigationList.Value = 0;
		}
		protected override void Localize() {
			ContentLayout.LocalizeField("TxbInsertImageUrl", ASPxRichEditStringId.InsertImage_EnterUrl);
			if(SettingsInsertPictureDialog.ShowFileUploadSection)
				ContentLayout.LocalizeField("UploadControl", ASPxRichEditStringId.InsertImage_UploadInstructions);
			PreviewTextLabel.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertImage_ImagePreview);
		}
		protected void SetupValidationSettings() {
			ImageUrlTextBox.ValidationSettings.ValidationGroup = "_dxeTbxInsertImageUrlGroup";
			ImageUrlTextBox.ValidationSettings.SetupDefaultSettings();
			if(SettingsInsertPictureDialog.ShowFileUploadSection)
				UploadControl.ValidationSettings.AllowedFileExtensions = new string[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
		}
		protected void uplImage_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			try {
				using(MemoryStream stream = new MemoryStream(e.UploadedFile.FileBytes)) {
					e.CallbackData = RichEdit.AddImageToCache(stream);
				}
			} catch(Exception exc) {
				e.IsValid = false;
				e.ErrorText = exc.Message;
			}
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgInsertImageForm";
		}
	}
}
