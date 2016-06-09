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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class ModifyChartLayoutDialog : SpreadsheetDialogWithRoundPanel {
		protected const string DefaultButtonGroup = "dxChartsBtn";
		protected const string ButtonClickFunctionTemplate = "function(s, e) {{ ASPx.SpreadsheetDialog.SetChartLayoutId('{0}'); }}";
		protected ASPxPanel ChartLayout { get; private set; }
		protected ASPxHiddenField HiddenField { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.ModifyChartLayoutDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxModifyChartLayoutForm";
		}
		protected override string GetRoundPanelID() {
			return "rpModifyChartLayout";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			ChartLayout = new ASPxPanel();
			ChartLayout.ClientInstanceName = GetControlClientInstanceName("_dxPanelChartPresets");
			ChartLayout.ID = "dxContenPanel";
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableRow.Cells.Add(tableCell);
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartColumnClusteredPresets.Instance.Presets, "ColumnClustered"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartColumnStackedPresets.Instance.Presets, "ColumnStacked"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartBarClusteredPresets.Instance.Presets, "BarClustered"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartBarStackedPresets.Instance.Presets, "BarStacked"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartLinePresets.Instance.Presets, "Line"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartPiePresets.Instance.Presets, "Pie"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartAreaPresets.Instance.Presets, "Area"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartScatterPresets.Instance.Presets, "Scatter"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartRadarPresets.Instance.Presets, "Radar"));
			ChartLayout.Controls.Add(CreatePresetViewContainer(ChartStockPresets.Instance.Presets, "Stock"));
			tableCell.Controls.Add(ChartLayout);
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			HiddenField = DialogUtils.CreateHiddenField("dxHiddenField");
			HiddenField.ClientInstanceName = GetControlClientInstanceName("_dxHiddenField");
			Controls.Add(HiddenField);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RenderUtils.AppendDefaultDXClassName(ChartLayout, SpreadsheetStyles.ModifyChartLayoutPanelCssClass);
		}
		protected WebControl CreatePresetViewContainer(IList<ChartLayoutPreset> presetCollection, string chartPresetType) {
			WebControl container = CreateDivContainer(chartPresetType);
			foreach(ChartLayoutPreset charPreset in presetCollection) {
				ASPxButton chartPresetBtn = CreatedChartButton(charPreset.ImageName);
				container.Controls.Add(chartPresetBtn);
			}
			return container;
		}
		protected WebControl CreateDivContainer(string chartPresetType) {
			WebControl container = RenderUtils.CreateDiv();
			container.CssClass = SpreadsheetStyles.PresetChartContainer;
			container.Attributes.Add("view", chartPresetType);
			return container;
		}
		protected ASPxButton CreatedChartButton(string imageName) {
			ASPxButton chartButton = new ASPxButton();
			chartButton.GroupName = DefaultButtonGroup;
			chartButton.AutoPostBack = false;
			chartButton.CssClass = SpreadsheetStyles.DialogChartButton;
			SpreadsheetImages ssImageProperties = Spreadsheet.Images;
			chartButton.Image.CopyFrom(ssImageProperties.GetImageProperties(Spreadsheet.Page, imageName));
			chartButton.ClientSideEvents.Click = string.Format(ButtonClickFunctionTemplate, imageName);
			return chartButton;
		}
	}
}
