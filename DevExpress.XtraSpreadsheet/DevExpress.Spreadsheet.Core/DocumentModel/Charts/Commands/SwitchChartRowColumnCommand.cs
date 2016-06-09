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

namespace DevExpress.XtraSpreadsheet.Model {
	public class SwitchChartRowColumnCommand : ModifyChartRangesCommand {
		ChartViewSeriesDirection direction;
		public SwitchChartRowColumnCommand(Chart chart, IErrorHandler errorHandler)
			: base(chart.Worksheet, errorHandler) {
			Chart = chart;
		}
		protected internal override void ExecuteCore() {
			SetDirection();
			SetRanges();
		}
		protected override ChartSeriesRangeModelBase CreateSeriesModel(CellRange seriesRange, ChartViewType viewType) {
			return ChartRangesCalculator.CreateModel(DataRange, seriesRange, viewType, direction);
		} 
		void SetDirection() {
			direction = Chart.SeriesDirection == ChartViewSeriesDirection.Horizontal ? ChartViewSeriesDirection.Vertical : ChartViewSeriesDirection.Horizontal;
		}
		protected internal override bool Validate() {
			if (Chart.Views.Count <= 0)
				return false;
			ChartReferencedRanges ranges = Chart.GetReferencedRanges();
			if (ranges.SeriesNameRanges.Count > 1 || ranges.ValueRanges.Count != 1 || ranges.ArgumentRanges.Count > 1)
				return false;
			DataRange = GetChartCellRange(ranges);
			return DataRange != null;
		}
		CellRange GetChartCellRange(ChartReferencedRanges ranges) {
			CellRange values = ranges.ValueRanges[0].CellRange as CellRange;
			if (values == null)
				return null;
			CellRange arguments = null;
			if (ranges.ArgumentRanges.Count > 0)
				arguments = ranges.ArgumentRanges[0].CellRange as CellRange;
			CellRange seriesNames = null;
			if (ranges.SeriesNameRanges.Count > 0)
				seriesNames = ranges.SeriesNameRanges[0].CellRange as CellRange;
			if (AreNeighbors(values, arguments) && AreNeighbors(values, seriesNames)) {
				CellPosition topLeft = values.TopLeft;
				CellPosition bottomRight = values.BottomRight;
				if (arguments != null) {
					topLeft = CellPosition.UnionPosition(topLeft, arguments.TopLeft, true);
					bottomRight = CellPosition.UnionPosition(bottomRight, arguments.BottomRight, false);
				}
				if (seriesNames != null) {
					topLeft = CellPosition.UnionPosition(topLeft, seriesNames.TopLeft, true);
					bottomRight = CellPosition.UnionPosition(bottomRight, seriesNames.BottomRight, false);
				}
				return new CellRange(values.Worksheet, topLeft, bottomRight);
			}
			return null;
		}
		bool AreNeighbors(CellRange range1, CellRange range2) {
			if (range1 == null || range2 == null)
				return true;
			if (range1.TopLeft.Column == range2.TopLeft.Column && range1.BottomRight.Column == range2.BottomRight.Column)
				return ((range1.BottomRight.Row + 1) == range2.TopLeft.Row) || ((range2.BottomRight.Row + 1) == range1.TopLeft.Row);
			if (range1.TopLeft.Row == range2.TopLeft.Row && range1.BottomRight.Row == range2.BottomRight.Row)
				return ((range1.BottomRight.Column + 1) == range2.TopLeft.Column) || ((range2.BottomRight.Column + 1) == range1.TopLeft.Column);
			return false;
		}
	}
}
