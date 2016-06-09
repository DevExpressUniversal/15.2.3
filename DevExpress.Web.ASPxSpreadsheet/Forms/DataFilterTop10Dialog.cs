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
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class DataFilterTop10Dialog : SpreadsheetDialogBase {
		protected ASPxLabel ShowCaption { get; set; }
		protected ASPxComboBox FilterOrder { get; set; }
		protected ASPxComboBox FilterType { get; set; }
		protected ASPxSpinEdit FilterValue { get; set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.Top10FilterDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxTop10FilterForm";
		}
		protected override void PopulateContentArea(Control container) {
			CreateShowRowsLabel(container);
			HtmlTable table = new HtmlTable();
			table.Attributes.Add("class", SpreadsheetStyles.DialogContentTableCssClass);
			container.Controls.Add(table);
			InstantiateEditors();
			CreateFilterRow(table);
		}
		protected virtual void InstantiateEditors() {
			FilterOrder = new ASPxComboBox() { 
				ID = "cbFilterOrder", 
				ClientInstanceName = GetControlClientInstanceName("_cbFilterOrder"),
				Width = Unit.Pixel(150)
			};
			FilterValue = new ASPxSpinEdit() { 
				ID = "cbFilterValue", 
				ClientInstanceName = GetControlClientInstanceName("_cbFilterValue"),
				MinValue = 0,
				MaxValue = 500,
				Width = Unit.Pixel(80)
			};
			FilterValue.ValidationSettings.RequiredField.IsRequired = true;
			FilterValue.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.None;
			FilterValue.ValidationSettings.ValidationGroup = "_dxSeFilterTop10ValueValidationGroup";
			FilterType = new ASPxComboBox() { 
				ID = "cbFilterType", 
				ClientInstanceName = GetControlClientInstanceName("_cbFilterType"),
				Width = Unit.Pixel(150)
			};
		}
		void CreateFilterRow(HtmlTable container) {
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(FilterOrder);
			tableCell = DialogUtils.CreateSeparatorCell();
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(FilterValue);
			tableCell = DialogUtils.CreateSeparatorCell();
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			tableCell.Controls.Add(FilterType);
			tableRow.Cells.Add(tableCell);
		}
		void CreateShowRowsLabel(Control container) {
			ShowCaption = new ASPxLabel() {
				CssClass = SpreadsheetStyles.DialogCaptionCellCssClass,
				EncodeHtml = false
			};
			container.Controls.Add(ShowCaption);
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			ShowCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FilterTop10_Show) + DialogUtils.LabelEndMark;
		}
	}
}
