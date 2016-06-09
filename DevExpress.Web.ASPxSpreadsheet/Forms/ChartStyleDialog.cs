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

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class ModifyChartStyleDialog : SpreadsheetDialogBase {
		protected const string ChartPresetStyleImagePrefix = "ChartPresetStyle",
							   DefaultButtonGroup = "dxChartsBtn",
							   ButtonClickFunctionTemplate = "function(s, e) {{ ASPx.SpreadsheetDialog.SetChartStyle('{0}'); }}";
		protected string[] UrlArray;
		protected ASPxHiddenField HiddenField { get; private set; }
		protected HtmlTable StylesTable { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.ModifyChartStyleDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxModifyChartStyleForm";
		}
		protected override void PopulateContentArea(Control container) {
			using(SpreadsheetChartStyles chartsStyles = new SpreadsheetChartStyles(Spreadsheet))
				UrlArray = chartsStyles.GetChartPresetStylesGallery();
			WebControl divContainer = RenderUtils.CreateDiv(SpreadsheetStyles.ModifyChartStylePanelCssClass);
			container.Controls.Add(divContainer);
			StylesTable = new HtmlTable();
			divContainer.Controls.Add(StylesTable);
			FillChartsPresetStyles();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			HiddenField = DialogUtils.CreateHiddenField("dxHiddenField");
			HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
			Controls.Add(HiddenField);
		}
		protected void FillChartsPresetStyles() {
			HtmlTableRow row = new HtmlTableRow();
			HtmlTableCell cell = null;
			for(int i = 0; i < SpreadsheetChartStyles.PresetsCount; i++) {
				if(i != 0 && i % 8 == 0) {
					StylesTable.Rows.Add(row);
					row = new HtmlTableRow();
				}
				ASPxButton chartPresetBtn = CreatedChartButton(i);
				cell = new HtmlTableCell();
				cell.Controls.Add(chartPresetBtn);
				row.Cells.Add(cell);
			}
			StylesTable.Rows.Add(row);
		}
		protected ASPxButton CreatedChartButton(int stylePresetId) {
			string imageName = ChartPresetStyleImagePrefix + stylePresetId.ToString();
			ASPxButton chartButton = new ASPxButton();
			chartButton.GroupName = DefaultButtonGroup;
			chartButton.AutoPostBack = false;
			SpreadsheetImages ssImageProperties = Spreadsheet.Images;
			chartButton.Image.Url = UrlArray[stylePresetId];
			stylePresetId++;
			chartButton.ToolTip = "Style " + stylePresetId.ToString();
			chartButton.ClientSideEvents.Click = string.Format(ButtonClickFunctionTemplate, stylePresetId.ToString());
			chartButton.ClientSideEvents.Init = "function(s, e) { ASPx.SpreadsheetDialog.InitializeChartStyle(); }";
			return chartButton;
		}
	}
}
