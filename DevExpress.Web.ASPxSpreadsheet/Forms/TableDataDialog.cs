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
using DevExpress.Web.ASPxSpreadsheet.Localization;
using System.Web.UI;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class TableSelectDataDialog : SpreadsheetDialogBase {
		protected const string TableDataRangeTextBoxId = "txbTableDataRange";
		protected const string TableHasHeadersCheckboxId = "chbTableHasHeaders";
		protected ASPxTextBox TableDataRange { get; private set; }
		protected ASPxLabel Caption { get; private set; }
		protected ASPxCheckBox TableHasHeaders { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxssDlgTableSelectData";
		}
		protected override string GetContentTableID() {
			return "dxTableSelectData";
		}
		protected override void PopulateContentArea(Control container) {
			HtmlTable table = new HtmlTable();
			table.Attributes.Add("class", SpreadsheetStyles.DialogContentTableCssClass);
			container.Controls.Add(table);
			HtmlTableRow tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			Caption = new ASPxLabel() { ID = "lblTableDataRange", AssociatedControlID = TableDataRangeTextBoxId };
			tableCell.Controls.Add(Caption);
			tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			TableDataRange = new ASPxTextBox();
			tableCell.Controls.Add(TableDataRange);
			TableDataRange.ID = TableDataRangeTextBoxId;
			TableDataRange.ClientInstanceName = GetControlClientInstanceName("_dxTxbTableDataRange");
			TableDataRange.Width = Unit.Percentage(100);
			TableDataRange.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			TableDataRange.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			TableDataRange.ValidationSettings.SetFocusOnError = true;
			TableDataRange.ValidationSettings.ValidateOnLeave = false;
			TableDataRange.ValidationSettings.ValidationGroup = "_dxTxbTableDataRangeValidationGroup";
			TableDataRange.ValidationSettings.RequiredField.IsRequired = true;
			TableDataRange.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			TableHasHeaders = new ASPxCheckBox() {
				ID = TableHasHeadersCheckboxId,
				ClientInstanceName = GetControlClientInstanceName("_chbTableHasHeaders")
			};
			tableCell.Controls.Add(TableHasHeaders);
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			Caption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.TableSelectData_Caption);
			TableDataRange.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
			TableHasHeaders.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.TableSelectData_TableHasHeaders);
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { TableDataRange, TableHasHeaders };
		}
	}
}
