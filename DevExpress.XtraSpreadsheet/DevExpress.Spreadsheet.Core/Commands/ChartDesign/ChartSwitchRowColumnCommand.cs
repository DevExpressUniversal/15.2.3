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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartSwitchRowColumnCommand
	public class ChartSwitchRowColumnCommand : ModifyChartCommandBase {
		public ChartSwitchRowColumnCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartSwitchRowColumn; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartSwitchRowColumnCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartSwitchRowColumnCommandDescription; } }
		public override string ImageName { get { return "ChartSwitchRowColumn"; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			return false;
		}
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
		protected override bool CanModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			ChartReferencedRanges ranges = chart.GetReferencedRanges();
			return ranges.SeriesNameRanges.Count <= 1 && ranges.ValueRanges.Count == 1 && ranges.ArgumentRanges.Count <= 1;
		}
		protected override void ModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return;
			ChartReferencedRanges ranges = chart.GetReferencedRanges();
			ranges = SwitchRanges(ranges);
			if (ranges == null)
				return;
			IChartView view = chart.Views[0];
			ChartViewSeriesDirection direction = view.SeriesDirection == ChartViewSeriesDirection.Horizontal ? ChartViewSeriesDirection.Vertical : ChartViewSeriesDirection.Horizontal;
			chart.SeriesDirection = direction;
			InsertChartCommandBase command = InsertChartCommandBase.CreateCompatibleCommandId(Control, chart);
			if (command != null)
				command.CreateViewSeries(view, GetRange(ranges.ValueRanges), GetRange(ranges.ArgumentRanges), GetRange(ranges.SeriesNameRanges), direction);
		}
		CellRange GetRange(FormulaReferencedRanges ranges) {
			return InsertChartCommandBase.GetRange(ranges);
		}
		ChartReferencedRanges SwitchRanges(ChartReferencedRanges ranges) {
			if (ranges.SeriesNameRanges.Count >= 2)
				return null;
			if (ranges.ArgumentRanges.Count >= 2)
				return null;
			ChartReferencedRanges result = new ChartReferencedRanges();
			result.ArgumentRanges = ranges.SeriesNameRanges;
			result.SeriesNameRanges = ranges.ArgumentRanges;
			result.ValueRanges = ranges.ValueRanges;
			return result;
		}
	}
	#endregion
}
