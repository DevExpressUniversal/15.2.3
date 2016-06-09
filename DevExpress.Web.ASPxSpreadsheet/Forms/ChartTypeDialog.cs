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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class ChangeChartTypeDialog : SpreadsheetDialogWithRoundPanel {
		protected ASPxPageControl ChartsCatigories { get; private set; }
		protected ASPxHiddenField HiddenField { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.ChangeChartTypeDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxChangeChartType";
		}
		protected override string GetRoundPanelID() {
			return "rpChangeChartType";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			ChartsCatigories = new ASPxPageControl();
			ChartsCatigories.TabPosition = TabPosition.Left;
			ChartsCatigories.TabStyle.HorizontalAlign = HorizontalAlign.Left;
			ChartsCatigories.ID = "pcChartsCatigories";
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(ChartsCatigories);
			ChartsCatigories.TabPages.Add(new STColumnTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STLineTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STPieTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STBarTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STAreaTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STScatterTabPage(Spreadsheet));
			ChartsCatigories.TabPages.Add(new STOtherTabPage(Spreadsheet));
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			HiddenField = DialogUtils.CreateHiddenField("dxHiddenField");
			HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
			Controls.Add(HiddenField);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RenderUtils.AppendDefaultDXClassName(ChartsCatigories, SpreadsheetStyles.DialogPageControlCssClass);
		}
	}
}
