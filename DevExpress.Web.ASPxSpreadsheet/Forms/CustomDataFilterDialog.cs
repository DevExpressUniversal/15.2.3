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

using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class CustomDataFilterDialog : SpreadsheetDialogBase {
		const string FilterDialogOperatorComboBoxCssClass = SpreadsheetStyles.CssClassPrefix + "DlgFilterOperatorComboBox";
		protected ASPxLabel ShowRowsCaption { get; set; }
		protected ASPxLabel QuestionSignDescription { get; set; }
		protected ASPxLabel StarSignDescription { get; set; }
		protected HtmlTableRow FirstRow { get; set; }
		protected ASPxComboBox FirstFilterOperator { get; set; }
		protected ASPxDropDownEditBase FirstFilterValue { get; set; }
		protected HtmlTableRow SecondRow { get; set; }
		protected ASPxComboBox SecondFilterOperator { get; set; }
		protected ASPxDropDownEditBase SecondFilterValue { get; set; }
		protected ASPxRadioButtonList AndOrOperator { get; set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.CustomDataFilterDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxCustomDataFilterForm";
		}
		protected override void PopulateContentArea(Control container) {
			CreateShowRowsLabel(container);
			HtmlTable table = new HtmlTable();
			table.Attributes.Add("class", SpreadsheetStyles.DialogContentTableCssClass);
			container.Controls.Add(table);
			InstantiateFilterOperatorEditor();
			InstantiateFilterValueEditor();
			FirstRow = CreateFilterRow(FirstFilterOperator, FirstFilterValue);
			table.Rows.Add(FirstRow);
			CreateRadioButtonList(table);
			SecondRow = CreateFilterRow(SecondFilterOperator, SecondFilterValue);
			table.Rows.Add(SecondRow);
			CreateFooterLabels(container);
		}
		protected virtual void InstantiateFilterOperatorEditor() {
			FirstFilterOperator = new ASPxComboBox() { 
				ID = "cbFilterOperator1", 
				ClientInstanceName = GetControlClientInstanceName("_cbFilterOperator1"),
				CssClass = FilterDialogOperatorComboBoxCssClass
			};
			SecondFilterOperator = new ASPxComboBox() { 
				ID = "cbFilterOperator2", 
				ClientInstanceName = GetControlClientInstanceName("_cbFilterOperator2"),
				CssClass = FilterDialogOperatorComboBoxCssClass
			};
		}
		protected virtual void InstantiateFilterValueEditor() {
			FirstFilterValue = new ASPxComboBox() { ID = "cbFilterValue1", ClientInstanceName = GetControlClientInstanceName("_cbFilterValue1") };
			SecondFilterValue = new ASPxComboBox() { ID = "cbFilterValue2", ClientInstanceName = GetControlClientInstanceName("_cbFilterValue2") };
			FirstFilterValue.AllowUserInput = true;
			SecondFilterValue.AllowUserInput = true;
			(FirstFilterValue as ASPxComboBox).DropDownStyle = DropDownStyle.DropDown;
			(SecondFilterValue as ASPxComboBox).DropDownStyle = DropDownStyle.DropDown;
		}
		void CreateRadioButtonList(HtmlTable container) {
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			AndOrOperator = new ASPxRadioButtonList() { ID = "rblAndOr", ClientInstanceName = GetControlClientInstanceName("_rblAndOr") };
			AndOrOperator.RepeatDirection = RepeatDirection.Horizontal;
			AndOrOperator.Border.BorderWidth = 0;
			AndOrOperator.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.CustomFilter_OperatorAnd));
			AndOrOperator.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.CustomFilter_OperatorOr));
			AndOrOperator.SelectedIndex = 0;
			tableCell.Controls.Add(AndOrOperator);
		}
		protected HtmlTableRow CreateFilterRow(ASPxComboBox filterOperator, ASPxDropDownEditBase filterValue) {
			HtmlTableRow tableRow = new HtmlTableRow();
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(filterOperator);
			tableCell = DialogUtils.CreateSeparatorCell();
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(filterValue);
			filterValue.Width = Unit.Pixel(220);
			return tableRow;
		}
		void AddLabel(Control container, ASPxLabel label) {
			label.CssClass = SpreadsheetStyles.DialogCaptionCellCssClass;
			label.EncodeHtml = false;
			container.Controls.Add(label);
		}
		void CreateShowRowsLabel(Control container) {
			ShowRowsCaption = new ASPxLabel() { ID = "lblShowRows" };
			AddLabel(container, ShowRowsCaption);
		}
		void CreateFooterLabels(Control container) {
			QuestionSignDescription = new ASPxLabel() { ID = "lblQuestionDesc" };
			AddLabel(container, QuestionSignDescription);
			StarSignDescription = new ASPxLabel() { ID = "lblStarDesc" };
			AddLabel(container, StarSignDescription);
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			ShowRowsCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.CustomFilter_ShowRows) + DialogUtils.LabelEndMark;
			QuestionSignDescription.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.CustomFilter_QuestionSignDescription);
			StarSignDescription.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.CustomFilter_StarSignDescription);
		}
	}
	public class CustomDateTimeFilterDialog : CustomDataFilterDialog {
		const string FilterDialogDateEditCssClass = SpreadsheetStyles.CssClassPrefix + "FilterDateEdit";
		protected ASPxDateEdit FirstFilterDate { get; set; }
		protected ASPxDateEdit SecondFilterDate { get; set; }
		protected override void InstantiateFilterValueEditor() {
			base.InstantiateFilterValueEditor();
			FirstFilterDate = new ASPxDateEdit() {
				ID = "deFilterDate1",
				ClientInstanceName = GetControlClientInstanceName("_deFilterDate1"),
				Width = Unit.Pixel(10),
				AllowUserInput = false
			};
			FirstFilterDate.DropDownButton.Image.IconID = "scheduling_calendar_16x16";
			RenderUtils.AppendDefaultDXClassName(FirstFilterDate, FilterDialogDateEditCssClass);
			SecondFilterDate = new ASPxDateEdit() {
				ID = "deFilterDate2",
				ClientInstanceName = GetControlClientInstanceName("_deFilterDate2"),
				Width = Unit.Pixel(10),
				AllowUserInput = false
			};
			SecondFilterDate.DropDownButton.Image.IconID = "scheduling_calendar_16x16";
			RenderUtils.AppendDefaultDXClassName(SecondFilterDate, FilterDialogDateEditCssClass);
		}
		protected override void PopulateContentArea(Control container) {
			base.PopulateContentArea(container);
			CreateDateEdit(FirstRow, FirstFilterDate);
			CreateDateEdit(SecondRow, SecondFilterDate);
		}
		protected void CreateDateEdit(HtmlTableRow tableRow, ASPxDateEdit dateEdit) {
			HtmlTableCell tableCell = DialogUtils.CreateSeparatorCell();
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			tableCell.Controls.Add(dateEdit);
			tableRow.Cells.Add(tableCell);
		}
	}
}
