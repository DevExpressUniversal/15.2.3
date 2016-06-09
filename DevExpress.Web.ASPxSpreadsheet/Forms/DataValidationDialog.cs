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

using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	class DataValidationDialog : SpreadsheetDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected ASPxLabel ValidationCriteria { get; private set; }
		protected ASPxFormLayout SettingsFormLayout { get; private set; }
		protected ASPxLabel Allow { get; private set; }
		protected ASPxComboBox Type { get; private set; }
		protected ASPxCheckBox IgnoreBlank { get; private set; }
		protected ASPxCheckBox InCellDropdown { get; private set; }
		protected ASPxLabel Data { get; private set; }
		protected ASPxComboBox Operator { get; private set; }
		protected ASPxLabel Formula1Label { get; private set; }
		protected ASPxTextBox Formula1 { get; private set; }
		protected ASPxLabel Formula2Label { get; private set; }
		protected ASPxTextBox Formula2 { get; private set; }
		protected ASPxCheckBox CanApplyChangesToAllCells { get; private set; }
		protected ASPxFormLayout InputMessageFromLayout { get; private set; }
		protected ASPxCheckBox ShowInputMessageCheckBox { get; private set; }
		protected ASPxLabel InputMessageDescriptionLabel { get; private set; }
		protected ASPxLabel InputMessageTitleLabel { get; private set; }
		protected ASPxLabel InputMessageLabel { get; private set; }
		protected ASPxTextBox InputMessageTitleTextBox { get; private set; }
		protected ASPxMemo InputMessageMemo { get; private set; }
		protected ASPxFormLayout ErrorAlertFormLayout { get; private set; }
		protected ASPxCheckBox ShowErrorMessageCheckBox { get; private set; }
		protected ASPxLabel ErrorMessageDescriptionLabel { get; private set; }
		protected ASPxLabel ErrorMessageStyleLabel { get; private set; }
		protected ASPxComboBox ErrorStyleComboBox { get; private set; }
		protected ASPxLabel ErrorMessageTitleLabel { get; private set; }
		protected ASPxTextBox ErrorMessageTitleTextBox { get; private set; }
		protected ASPxLabel ErrorMessageLabel { get; private set; }
		protected ASPxMemo ErrorMessageMemo { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.DataValidationDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxDataValidationForm";
		}
		protected string GetFormulaLabelTexts() {
			var texts = new Hashtable();
			texts["Value"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationValue);
			texts["Minimum"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationMinimum);
			texts["Maximum"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationMaximum);
			texts["Source"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationSource);
			texts["StartDate"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationStartDate);
			texts["Date"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationDate);
			texts["EndDate"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationEndDate);
			texts["StartTime"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationStartTime);
			texts["Time"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationTime);
			texts["EndTime"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationEndTime);
			texts["Formula"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationFormula);
			texts["Length"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationLength);
			return HtmlConvertor.ToJSON(texts);
		}
		protected string GetFormulaErrors() {
			var msgs = new Hashtable();
			msgs["FormulaIsEmpty"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationFormulaIsEmpty);
			msgs["BothFormulasAreEmpty"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationBothFormulasAreEmpty);
			msgs["MinGreaterThanMax"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationMinGreaterThanMax);
			msgs["DefinedNameNotFound"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationDefinedNameNotFound);
			msgs["InvalidNonnumericValue"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidNonnumericValue);
			msgs["InvalidDecimalValue"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidDecimalValue);
			msgs["InvalidNegativeValue"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidNegativeValue);
			msgs["UnionRangeNotAllowed"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationUnionRangeNotAllowed);
			msgs["InvalidReference"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidReference);
			msgs["MoreThanOneCellInRange"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationMoreThanOneCellInRange);
			msgs["MustBeRowOrColumnRange"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationMustBeRowOrColumnRange);
			msgs["InvalidDate"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidDate);
			msgs["InvalidTime"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationInvalidTime);
			msgs["Failed"] = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationFailed);
			return HtmlConvertor.ToJSON(msgs);
		}
		protected override void PopulateContentArea(Control container) {
			PageControl = new ASPxPageControl();
			PageControl.TabPages.Add("Settings", "Settings");
			PageControl.TabPages.Add("Input Message", "InputMessage");
			PageControl.TabPages.Add("Error Alert", "ErrorAlert");
			PopulateSettingsTab();
			PopulateInputMessageTab();
			PopulateErrorAlertTab();
			container.Controls.Add(PageControl);
		}
		protected ASPxLabel CreateLabel(string id, string clientInstanceName, string text) {
			return new ASPxLabel() {
				ID = id,
				ClientInstanceName = GetControlClientInstanceName(clientInstanceName),
				Text = text
			};
		}
		protected ASPxComboBox CreateComboBox(string id, string clientInstanceName) {
			return new ASPxComboBox() {
				ID = id,
				ClientInstanceName = GetControlClientInstanceName(clientInstanceName),
				DropDownRows = 8,
				SelectedIndex = 0,
				IncrementalFilteringMode = IncrementalFilteringMode.None
			};
		}
		protected ASPxCheckBox CreateCheckBox(string id, string clientInstanceName, string text) {
			return new ASPxCheckBox() {
				ID = id,
				ClientInstanceName = GetControlClientInstanceName(clientInstanceName),
				Text = text,
			};
		}
		protected ASPxTextBox CreateTextBox(string id, string clientInstanceName) {
			return new ASPxTextBox() {
				ID = id,
				ClientInstanceName = GetControlClientInstanceName(clientInstanceName),
				Width = Unit.Percentage(100)
			};
		}
		protected ASPxMemo CreateMemo(string id, string clientInstanceName) {
			return new ASPxMemo() {
				ID = id,
				ClientInstanceName = GetControlClientInstanceName(clientInstanceName),
				Width = Unit.Percentage(100),
				Height = 90
			};
		}
		protected void CreateSettingsEditors() {
			ValidationCriteria = CreateLabel("lblValidationCriteria", "_dxLblValidationCriteria", "Validation criteria");
			Allow = CreateLabel("lblAllow", "_dxLblAllow", "Allow:");
			Type = CreateComboBox("cbxType", "_dxCbxType");
			IgnoreBlank = CreateCheckBox("cbIgnoreBlank", "_dxCbIgnoreBlank", "Ignore blank");
			InCellDropdown = CreateCheckBox("cbInCellDropdown", "_dxCbInCellDropdown", "In-cell dropdown");
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationAnyValue), (int)DataValidationType.None);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationWholeNumber), (int)DataValidationType.Whole);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationDecimal), (int)DataValidationType.Decimal);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationList), (int)DataValidationType.List);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationDate), (int)DataValidationType.Date);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationTime), (int)DataValidationType.Time);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationTextLength), (int)DataValidationType.TextLength);
			Type.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationCustom), (int)DataValidationType.Custom);
			Operator = CreateComboBox("cbxOperator", "_dxCbxOperator");
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationBetween), (int)DataValidationOperator.Between);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationNotBetween), (int)DataValidationOperator.NotBetween);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationEqual), (int)DataValidationOperator.Equal);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationNotEqual), (int)DataValidationOperator.NotEqual);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThan), (int)DataValidationOperator.GreaterThan);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationLessThan), (int)DataValidationOperator.LessThan);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationGreaterThanOrEqual), (int)DataValidationOperator.GreaterThanOrEqual);
			Operator.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationLessThanOrEqual), (int)DataValidationOperator.LessThanOrEqual);
			Data = CreateLabel("lblData", "_dxLblData", "Data:");
			Formula1Label = CreateLabel("lblFormula1", "_dxLblFormula1", "");
			Formula1Label.JSProperties["cpTexts"] = GetFormulaLabelTexts();
			Formula1 = CreateTextBox("tbFormula1", "_dxTbFormula1");
			Formula2Label = CreateLabel("lblFormula2", "_dxLblFormula2", "");
			Formula2 = CreateTextBox("tbFormula2", "_dxTbFormula2");
			CanApplyChangesToAllCells = CreateCheckBox("cbCanApplyChangesToAllCells", "_dxCbCanApplyChangesToAllCells", "Apply these changes to all other cells with the same settings");
		}
		protected void CreateInputMessageEditors() {
			ShowInputMessageCheckBox = CreateCheckBox("cbShowInputMessage", "_dxCbShowInputMessage", "Show input message when cell is selected");
			InputMessageDescriptionLabel = CreateLabel("lblInputMessageDescription", "_dxLblInputMessageDescription", "When cell is selected, show this input message:");
			InputMessageDescriptionLabel.CssClass = "dxssDlgDataValidationDescription";
			InputMessageTitleLabel = CreateLabel("lblTitle", "_dxLblTitle", "Title:");
			InputMessageLabel = CreateLabel("lblInputMessage", "_dxLblInputMessage", "Input message:");
			InputMessageTitleTextBox = CreateTextBox("tbTitle", "_dxTbTitle");
			InputMessageMemo = CreateMemo("mInputMessage", "_dxMInputMessage");
		}
		protected void CreateErrorAlertEditors() {
			ShowErrorMessageCheckBox = CreateCheckBox("cbShowErrorMessage", "_dxCbShowErrorMessage", "Show error alert after invalid data is entered");
			ErrorMessageDescriptionLabel = CreateLabel("lblErrorAlertDescription", "_dxLblErrorAlertDescription", "When user enters invalid data, show this error alert:");
			ErrorMessageDescriptionLabel.CssClass = "dxssDlgDataValidationDescription";
			ErrorMessageStyleLabel = CreateLabel("lblStyle", "_dxLblStyle", "Style:");
			ErrorStyleComboBox = CreateComboBox("cbxStyle", "_dxCbxStyle");
			ErrorStyleComboBox.Width = Unit.Percentage(100);
			ErrorStyleComboBox.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationStop), (int)DataValidationErrorStyle.Stop);
			ErrorStyleComboBox.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationWarning), (int)DataValidationErrorStyle.Warning);
			ErrorStyleComboBox.Items.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_DataValidationInformation), (int)DataValidationErrorStyle.Information);
			ErrorMessageTitleLabel = CreateLabel("lblErrorMessageTitle", "_dxLblErrorMessageTitle", "Title:");
			ErrorMessageTitleTextBox = CreateTextBox("tbErrorMessageTitle", "_dxTbErrorMessageTitle");
			ErrorMessageLabel = CreateLabel("lblErrorMessageTitle", "_dxLblErrorMessage", "Error message:");
			ErrorMessageMemo = CreateMemo("mErrorMessage", "_dxMErrorMessage");
		}
		protected void CreateSettingsFormLayout() {
			SettingsFormLayout = new ASPxFormLayout();
			SettingsFormLayout.ColCount = 2;
			SettingsFormLayout.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			SettingsFormLayout.Height = Unit.Percentage(100);
			SettingsFormLayout.Width = Unit.Percentage(100);
			SettingsFormLayout.Styles.LayoutGroup.CssClass = "fullHeight";
			LayoutItem validationCriteriaItem = new LayoutItem();
			validationCriteriaItem.Controls.Add(ValidationCriteria);
			validationCriteriaItem.ColSpan = 2;
			validationCriteriaItem.ParentContainerStyle.Paddings.PaddingLeft = 0;
			LayoutItem allowItem = new LayoutItem();
			allowItem.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			allowItem.Controls.Add(Allow);
			allowItem.Controls.Add(Type);
			LayoutItem ignoreBlankItem = new LayoutItem();
			ignoreBlankItem.VerticalAlign = FormLayoutVerticalAlign.Bottom;
			ignoreBlankItem.Controls.Add(IgnoreBlank);
			ignoreBlankItem.Controls.Add(InCellDropdown);
			LayoutItem dataItem = new LayoutItem();
			dataItem.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			dataItem.Controls.Add(Data);
			dataItem.Controls.Add(Operator);
			LayoutItem InCellDropdownItem = new LayoutItem();
			InCellDropdownItem.VerticalAlign = FormLayoutVerticalAlign.Top;
			InCellDropdownItem.Controls.Add(InCellDropdown);
			LayoutItem formula1Item = new LayoutItem();
			formula1Item.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			formula1Item.ColSpan = 2;
			formula1Item.Controls.Add(Formula1Label);
			formula1Item.Controls.Add(Formula1);
			LayoutItem formula2Item = new LayoutItem();
			formula2Item.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			formula2Item.ColSpan = 2;
			formula2Item.Controls.Add(Formula2Label);
			formula2Item.Controls.Add(Formula2);
			LayoutItem CanApplyChangesToAllCellsItem = new LayoutItem();
			CanApplyChangesToAllCellsItem.Controls.Add(CanApplyChangesToAllCells);
			CanApplyChangesToAllCellsItem.ColSpan = 2;
			CanApplyChangesToAllCellsItem.ParentContainerStyle.CssClass = "fullHeight";
			CanApplyChangesToAllCellsItem.VerticalAlign = FormLayoutVerticalAlign.Bottom;
			CanApplyChangesToAllCellsItem.ParentContainerStyle.Paddings.PaddingLeft = 0;
			SettingsFormLayout.Items.Add(validationCriteriaItem);
			SettingsFormLayout.Items.Add(allowItem);
			SettingsFormLayout.Items.Add(ignoreBlankItem);
			SettingsFormLayout.Items.Add(dataItem);
			SettingsFormLayout.Items.Add(InCellDropdownItem);
			SettingsFormLayout.Items.Add(formula1Item);
			SettingsFormLayout.Items.Add(formula2Item);
			SettingsFormLayout.Items.Add(CanApplyChangesToAllCellsItem);
		}
		protected void CreateInputMessageFormLayout() {
			InputMessageFromLayout = new ASPxFormLayout();
			InputMessageFromLayout.ColCount = 2;
			InputMessageFromLayout.Height = Unit.Percentage(100);
			InputMessageFromLayout.Width = Unit.Percentage(100);
			InputMessageFromLayout.Styles.LayoutGroup.CssClass = "fullHeight";
			InputMessageFromLayout.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			LayoutItem showInputMessageCheckBoxItem = new LayoutItem();
			showInputMessageCheckBoxItem.Controls.Add(ShowInputMessageCheckBox);
			LayoutItem titleItem = new LayoutItem();
			titleItem.ColSpan = 2;
			titleItem.Controls.Add(InputMessageTitleLabel);
			titleItem.Controls.Add(InputMessageTitleTextBox);
			LayoutItem messageItem = new LayoutItem();
			messageItem.ColSpan = 2;
			messageItem.Height = Unit.Percentage(100);
			InputMessageMemo.Height = Unit.Percentage(90);
			messageItem.Controls.Add(InputMessageLabel);
			messageItem.Controls.Add(InputMessageMemo);
			InputMessageFromLayout.Items.Add(showInputMessageCheckBoxItem);
			InputMessageFromLayout.Items.Add(titleItem);
			InputMessageFromLayout.Items.Add(messageItem);
		}
		protected void CreateErrorAlertFormLayout() {
			ErrorAlertFormLayout = new ASPxFormLayout();
			ErrorAlertFormLayout.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			ErrorAlertFormLayout.Height = Unit.Percentage(100);
			ErrorAlertFormLayout.Width = Unit.Percentage(100);
			ErrorAlertFormLayout.Styles.LayoutGroup.CssClass = "fullHeight";
			LayoutItem showErrorMessageCheckBoxItem = new LayoutItem();
			showErrorMessageCheckBoxItem.Controls.Add(ShowErrorMessageCheckBox);
			LayoutItem styleItem = new LayoutItem();
			styleItem.Controls.Add(ErrorMessageStyleLabel);
			styleItem.Controls.Add(ErrorStyleComboBox);
			LayoutItem titleItem = new LayoutItem();
			titleItem.Controls.Add(ErrorMessageTitleLabel);
			titleItem.Controls.Add(ErrorMessageTitleTextBox);
			LayoutItem errorMessageItem = new LayoutItem();
			errorMessageItem.Height = Unit.Percentage(100);
			ErrorMessageMemo.Height = Unit.Percentage(90);
			errorMessageItem.Controls.Add(ErrorMessageLabel);
			errorMessageItem.Controls.Add(ErrorMessageMemo);
			ErrorAlertFormLayout.Items.Add(showErrorMessageCheckBoxItem);
			ErrorAlertFormLayout.Items.Add(styleItem);
			ErrorAlertFormLayout.Items.Add(titleItem);
			ErrorAlertFormLayout.Items.Add(errorMessageItem);
		}
		protected void PopulateSettingsTab() {
			CreateSettingsEditors();
			CreateSettingsFormLayout();
			PageControl.TabPages[0].Controls.Add(SettingsFormLayout);
		}
		protected void PopulateInputMessageTab() {
			CreateInputMessageEditors();
			CreateInputMessageFormLayout();
			PageControl.TabPages[1].Controls.Add(InputMessageFromLayout);
		}
		protected void PopulateErrorAlertTab() {
			CreateErrorAlertEditors();
			CreateErrorAlertFormLayout();
			PageControl.TabPages[2].Controls.Add(ErrorAlertFormLayout);
		}
	}
}
