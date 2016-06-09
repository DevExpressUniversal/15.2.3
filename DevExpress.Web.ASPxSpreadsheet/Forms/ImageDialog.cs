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
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class InsertImageDialog : SpreadsheetDialogWithChoice {
		protected const string ImageUrlTextBoxId = "txbInsertImageUrl",
								ImageUploadId = "uplImage";
		protected ASPxButton ChangeButton { get; private set; }
		protected ASPxLabel CaptionImageUrl { get; private set; }
		protected ASPxButtonEdit ImageUrl { get; private set; }
		protected ASPxLabel CaptionBrowser { get; private set; }
		protected ASPxUploadControl ImageUpload;
		protected ASPxHiddenField HiddenField { get; private set; }
		protected SpreadsheetInsertPictureDialogSettings DialogSettings {
			get { return Spreadsheet.SettingsDialogs.InsertPictureDialog; }
		}
		protected override bool IsDialogContainsChoiceSection {
			get {
				return DialogSettings.ShowFileUploadSection;
			}
		}
		protected override string GetDialogCssClassName() {
			return "dxssDlgInsertImageForm";
		}
		protected override string GetContentTableID() {
			return "dxInsertImageForm";
		}
		protected override string GetRoundPanelID() {
			return "rpInsertImage";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			#region ImageUrl
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Style.Add("text-align", "left");
			tableRow.Cells.Add(tableCell);
			CaptionImageUrl = new ASPxLabel() { ID = "lblImageUrl", AssociatedControlID = ImageUrlTextBoxId };
			tableCell.Controls.Add(CaptionImageUrl);
			WebControl webControl = RenderUtils.CreateDiv("dxssDlgCaptionIndent");
			tableCell.Controls.Add(webControl);
			ImageUrl = new ASPxButtonEdit();
			ImageUrl.ID = ImageUrlTextBoxId;
			ImageUrl.ClientInstanceName = GetControlClientInstanceName("_dxeTbxInsertImageUrl");
			ImageUrl.AutoCompleteType = AutoCompleteType.Disabled;
			ImageUrl.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			ImageUrl.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			ImageUrl.ValidationSettings.SetFocusOnError = true;
			ImageUrl.ValidationSettings.ValidateOnLeave = false;
			ImageUrl.ValidationSettings.ValidationGroup = "_dxeTbxInsertImageUrlGroup";
			ImageUrl.ValidationSettings.Display = Display.Dynamic;
			ImageUrl.ValidationSettings.RequiredField.IsRequired = true;
			ImageUrl.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			ImageUrl.ClientSideEvents.TextChanged = "function(s, e) { ASPx.SSInsertImageSrcValueChanged(s.GetText()); }";
			tableCell.Controls.Add(ImageUrl);
			#endregion
			#region PreView
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgImagePreview");
			tableRow.Cells.Add(tableCell);
			HtmlTable previewTable = DialogUtils.CreateTable();
			previewTable.Style.Add("width", "100%"); 
			tableCell.Controls.Add(previewTable);
			tableRow = new HtmlTableRow();
			previewTable.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgImagePreviewCell");
			tableRow.Cells.Add(tableCell);
			webControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			webControl.CssClass = "dxssPreviewText";
			webControl.Controls.Add(new LiteralControl(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_ImagePreview)));
			tableCell.Controls.Add(webControl);
			Image previewImage = RenderUtils.CreateImage();
			previewImage.CssClass = "dxssPreviewImage";
			previewImage.Style.Add("display", "none");
			previewImage.Style.Add("width", "180px");
			previewImage.Style.Add("height", "100px");
			previewImage.Attributes.Add("alt", ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_ImagePreview));
			previewImage.Attributes.Add("src", EmptyImageProperties.GetEmptyImageUrl(Page));
			tableCell.Controls.Add(previewImage);
			#endregion
			if(IsDialogContainsChoiceSection) {
				tableRow = new HtmlTableRow();
				tableRow.Style.Add("display", "none");
				container.Rows.Add(tableRow);
				tableCell = new HtmlTableCell();
				tableCell.Style.Add("text-align", "left");
				tableRow.Cells.Add(tableCell);
				CaptionBrowser = new ASPxLabel() { ID = "lblBrowse", AssociatedControlID = ImageUploadId };
				tableCell.Controls.Add(CaptionBrowser);
				webControl = RenderUtils.CreateDiv("dxssDlgCaptionIndent");
				tableCell.Controls.Add(webControl);
				ImageUpload = Spreadsheet.CreateUploadControl();
				ImageUpload.ID = ImageUploadId;
				tableCell.Controls.Add(ImageUpload);
				ImageUpload.ClientInstanceName = GetControlClientInstanceName("_dxeUplImage");
				ImageUpload.ShowClearFileSelectionButton = false;
				ImageUpload.UploadMode = UploadControlUploadMode.Standard;
				ImageUpload.FileUploadComplete += ImageUpload_FileUploadComplete;
				ImageUpload.ClientSideEvents.FileUploadComplete = "ASPx.SSImageUploadComplete";
				ImageUpload.ClientSideEvents.FilesUploadComplete = "ASPx.SSImageUploadComplete";
				ImageUpload.ClientSideEvents.FilesUploadStart = "ASPx.SSImageUploadStart";
			}
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			HiddenField = new ASPxHiddenField();
			HiddenField.ID = "dxHiddenField";
			HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
			HiddenField.SyncWithServer = false;
			Controls.Add(HiddenField);
		}
		public void ImageUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs args) {
			try {
				using(MemoryStream stream = new MemoryStream(args.UploadedFile.FileBytes)) {
					args.CallbackData = Spreadsheet.AddImage(stream, args.UploadedFile.FileName);
				}
			} catch(Exception e) {
				args.IsValid = false;
				args.ErrorText = e.Message;
			}
		}
		#region PopulateChoiceArea
		protected override ASPxRadioButton CreateFirstChoiceElement() {
			ASPxRadioButton firstChoice = new ASPxRadioButton();
			firstChoice.ID = "rblFromTheWeb";
			firstChoice.GroupName = "InsertImageFormGroup";
			firstChoice.Checked = true;
			firstChoice.ClientInstanceName = GetControlClientInstanceName("_dxeRblImageFromTheWeb");
			firstChoice.ClientSideEvents.CheckedChanged = "ASPx.SpreadsheetDialog.SectionTypeChanged";
			return firstChoice;
		}
		protected override ASPxRadioButton CreateSecondChoiceElement() {
			var secondChoice = new ASPxRadioButton();
			secondChoice.ID = "rblFromThisComputer";
			secondChoice.GroupName = "InsertImageFormGroup";
			secondChoice.ClientInstanceName = GetControlClientInstanceName("_dxeRblImageFromThisComputer");
			secondChoice.ClientSideEvents.CheckedChanged = "ASPx.SpreadsheetDialog.SectionTypeChanged";
			return secondChoice;
		}
		#endregion
		#region PopulateFooteerArea
		protected override void InitializeMiddleButtons(Control container) {
			InitializeChangeButton(container);
		}
		protected void InitializeChangeButton(Control container) {
			ChangeButton = new ASPxButton();
			ChangeButton.ID = "btnChangeImage";
			ChangeButton.AutoPostBack = false;
			ChangeButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			ChangeButton.CausesValidation = false;
			ChangeButton.ClientVisible = false;
			ChangeButton.ClientInstanceName = GetControlClientInstanceName("_dxeBtnChangeImage");
			ChangeButton.ClientSideEvents.Init = GetDefaultSubmitButtonInitEventHandler();
			container.Controls.Add(ChangeButton);
		}
		#endregion
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RoundPanel.Style.Add("width", "315px"); 
			ImageUrl.Width = Unit.Percentage(100);
			if(IsDialogContainsChoiceSection) {
				ImageUpload.Width = Unit.Percentage(100);
			}
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			CaptionImageUrl.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_EnterUrl) + DialogUtils.LabelEndMark;
			HiddenField.Add("RequiredFieldError", ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError));
			ImageUrl.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
			if(IsDialogContainsChoiceSection) {
				SecondChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_FromLocal);
				FirstChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_FromWeb);
				CaptionBrowser.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage_UploadInstructions) + DialogUtils.LabelEndMark;
			}
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { SubmitButton, CancelButton, ChangeButton };
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			ASPxEditBase[] baseList = base.GetChildDxEdits();
			List<ASPxEditBase> baseCollection = new List<ASPxEditBase>(baseList);
			baseCollection.Add(ImageUrl);
			return baseCollection.ToArray();
		}
	}
}
