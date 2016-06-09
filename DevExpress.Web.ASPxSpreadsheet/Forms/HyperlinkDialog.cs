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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class InsertHyperlinkDialog : SpreadsheetDialogWithChoice {
		protected const string UrlTextBoxId = "txbURL",
							   MailTextBoxId = "txbEmailTo",
							   SubjectTextBoxId = "txbSubject",
							   DisplayTextTextBoxId = "txbText",
							   ToolTipTextBoxId = "txbToolTip";
		protected ASPxLabel CaptionUrl { get; private set; }
		protected ASPxButtonEdit Url { get; private set; }
		protected ASPxLabel CaptionMail { get; private set; }
		protected ASPxTextBox Mail { get; private set; }
		protected ASPxLabel CaptionSubject { get; private set; }
		protected ASPxTextBox Subject { get; private set; }
		protected ASPxLabel CaptionLinkProperty { get; private set; }
		protected ASPxLabel CaptionDisplayText { get; private set; }
		protected ASPxTextBox DisplayText { get; private set; }
		protected ASPxLabel CaptionToolTip { get; private set; }
		protected ASPxTextBox ToolTip { get; private set; }
		protected ASPxButton ChangeButton { get; private set; }
		protected SpreadsheetInsertLinkDialogSettings DialogSettings {
			get { return Spreadsheet.SettingsDialogs.InsertLinkDialog; }
		}
		protected override bool IsDialogContainsChoiceSection {
			get {
				return DialogSettings.ShowEmailAddressSection;
			}
		}
		protected override string GetDialogCssClassName() {
			return "dxssDlgInsertLinkForm";
		}
		protected override string GetContentTableID() {
			return "dxInsertLinkForm";
		}
		protected override string GetRoundPanelID() {
			return "rpInsertLink";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			#region URLArea
			HtmlTableRow tableRow = new HtmlTableRow();
			tableRow.ID = Spreadsheet.ClientID + "_dxeURLArea";
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionUrl = new ASPxLabel() { ID = "lblUrl", AssociatedControlID = UrlTextBoxId };
			tableCell.Controls.Add(CaptionUrl);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			Url = new ASPxButtonEdit();
			tableCell.Controls.Add(Url);
			Url.ID = UrlTextBoxId;
			Url.ClientInstanceName = GetControlClientInstanceName("_dxeTxbURL");
			Url.Width = Unit.Percentage(100);
			Url.AutoCompleteType = AutoCompleteType.Disabled;
			Url.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			Url.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			Url.ValidationSettings.SetFocusOnError = true;
			Url.ValidationSettings.ValidateOnLeave = false;
			Url.ValidationSettings.ValidationGroup = "_dxeTxbURLGroup";
			Url.ValidationSettings.RequiredField.IsRequired = true;
			Url.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			#endregion
			#region EMailArea
			tableRow = new HtmlTableRow();
			tableRow.ID = Spreadsheet.ClientID + "_dxeEmailAreaEmail";
			tableRow.Style.Add("display", "none");
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionMail = new ASPxLabel() { ID = "lblEmailTo", AssociatedControlID = MailTextBoxId };
			tableCell.Controls.Add(CaptionMail);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			Mail = new ASPxTextBox();
			tableCell.Controls.Add(Mail);
			Mail.ID = MailTextBoxId;
			Mail.ClientInstanceName = GetControlClientInstanceName("_dxeTxbEmailTo");
			Mail.Width = Unit.Percentage(100);
			Mail.AutoCompleteType = AutoCompleteType.Disabled;
			Mail.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			Mail.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			Mail.ValidationSettings.SetFocusOnError = true;
			Mail.ValidationSettings.ValidateOnLeave = false;
			Mail.ValidationSettings.ValidationGroup = "_dxeTxbEmailToGroup";
			Mail.ValidationSettings.RegularExpression.ValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
			Mail.ValidationSettings.RequiredField.IsRequired = true;
			Mail.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			#endregion
			#region SubjectArea
			tableRow = new HtmlTableRow();
			tableRow.ID = Spreadsheet.ClientID + "_dxeEmailAreaSubject";
			tableRow.Style.Add("display", "none");
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableCell.Style.Add("padding-bottom", "12px"); 
			tableRow.Cells.Add(tableCell);
			CaptionSubject = new ASPxLabel() { ID = "lblSubject", AssociatedControlID = SubjectTextBoxId };
			tableCell.Controls.Add(CaptionSubject);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableCell.Style.Add("padding-bottom", "12px"); 
			tableRow.Cells.Add(tableCell);
			Subject = new ASPxTextBox();
			tableCell.Controls.Add(Subject);
			Subject.ID = SubjectTextBoxId;
			Subject.ClientInstanceName = GetControlClientInstanceName("_dxeTxbSubject");
			Subject.Width = Unit.Percentage(100);
			Subject.AutoCompleteType = AutoCompleteType.Disabled;			
			#endregion
			#region CaptionArea
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgDisplayPropertiesCell");
			tableCell.ColSpan = 2;
			tableRow.Cells.Add(tableCell);
			CaptionLinkProperty = new ASPxLabel() { ID = "lblLinkDisplay", AssociatedControlID = SubjectTextBoxId };
			tableCell.Controls.Add(CaptionLinkProperty);
			#endregion
			#region DisplayTextArea
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionDisplayText = new ASPxLabel() { ID = "lblText", AssociatedControlID = DisplayTextTextBoxId };
			tableCell.Controls.Add(CaptionDisplayText);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			DisplayText = new ASPxTextBox();
			tableCell.Controls.Add(DisplayText);
			DisplayText.ID = DisplayTextTextBoxId;
			DisplayText.ClientInstanceName = GetControlClientInstanceName("_dxeTxbText");
			DisplayText.Width = Unit.Percentage(100);
			DisplayText.AutoCompleteType = AutoCompleteType.Disabled;
			#endregion
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			tableCell.ColSpan = 2;
			tableRow.Cells.Add(tableCell);
			#region ToolTipArea
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionToolTip = new ASPxLabel() { ID = "lblToolTip", AssociatedControlID = ToolTipTextBoxId };
			tableCell.Controls.Add(CaptionToolTip);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			ToolTip = new ASPxTextBox();
			tableCell.Controls.Add(ToolTip);
			ToolTip.ID = ToolTipTextBoxId;
			ToolTip.ClientInstanceName = GetControlClientInstanceName("_dxeTxbToolTip");
			ToolTip.Width = Unit.Percentage(100);
			ToolTip.AutoCompleteType = AutoCompleteType.Disabled;
			#endregion
		}
		#region PopulateChoiceArea
		protected override ASPxRadioButton CreateFirstChoiceElement() {
			ASPxRadioButton firstChoice = new ASPxRadioButton();
			firstChoice.ID = "rblUrl";
			firstChoice.GroupName = "InsertLinkGroup";
			firstChoice.Checked = true;
			firstChoice.ClientInstanceName = GetControlClientInstanceName("_dxeRblUrl");
			firstChoice.ClientSideEvents.CheckedChanged = "ASPx.SpreadsheetDialog.SectionTypeChanged";
			return firstChoice;
		}
		protected override ASPxRadioButton CreateSecondChoiceElement() {
			ASPxRadioButton secondChoice = new ASPxRadioButton();
			secondChoice.ID = "rblEmail";
			secondChoice.GroupName = "InsertLinkGroup";
			secondChoice.ClientInstanceName = GetControlClientInstanceName("_dxeRblEmail");
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
			ChangeButton.ID = "btnChange";
			ChangeButton.AutoPostBack = false;
			ChangeButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			ChangeButton.CausesValidation = false;
			ChangeButton.ClientInstanceName = GetControlClientInstanceName("_dxeBtnChange");
			ChangeButton.ClientSideEvents.Init = GetDefaultSubmitButtonInitEventHandler();
			container.Controls.Add(ChangeButton);
		}
		#endregion
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RoundPanel.Style.Add("width", "340px"); 
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			CaptionMail.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_Email) + DialogUtils.LabelEndMark;
			CaptionUrl.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_URL) + DialogUtils.LabelEndMark;
			CaptionSubject.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_Subject) + DialogUtils.LabelEndMark;
			CaptionLinkProperty.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_DisplayProperties);
			CaptionDisplayText.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_Text) + DialogUtils.LabelEndMark;
			CaptionToolTip.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_ToolTip) + DialogUtils.LabelEndMark;
			Url.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
			Mail.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
			Mail.ValidationSettings.RegularExpression.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.Invalid_EMail);
			ChangeButton.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonChange);
			if(IsDialogContainsChoiceSection) {
				SecondChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_Email);
				FirstChoice.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink_URL);
			}
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { SubmitButton, CancelButton, ChangeButton };
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			ASPxEditBase[] baseList = base.GetChildDxEdits();
			List<ASPxEditBase> baseCollection = new List<ASPxEditBase>(baseList);
			baseCollection.Add(Url);
			baseCollection.Add(Mail);
			baseCollection.Add(Subject);
			baseCollection.Add(DisplayText);
			baseCollection.Add(ToolTip);
			return baseCollection.ToArray();
		}
	}
}
