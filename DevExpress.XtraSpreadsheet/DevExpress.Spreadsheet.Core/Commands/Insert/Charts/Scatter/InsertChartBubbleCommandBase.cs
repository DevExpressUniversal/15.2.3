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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartBubbleCommandBase (abstract)
	public abstract class InsertChartBubbleCommandBase : InsertChartCommandBase {
		protected InsertChartBubbleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool Is3D { get; }
		protected override ChartLayoutModifier Preset { get { return ChartScatterPresets.Instance.DefaultModifier; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Bubble; } }
		protected override void CreateAxes(Chart chart) {
			CreateTwoPrimaryAxes(chart, true, true);
		}
		protected override void CreateHorizontalSeries(CellRange seriesRange, IDataReference arguments, IChartView view, CellRange seriesNamesRange) {
			int firstRow = seriesRange.TopLeft.Row;
			int lastRow = seriesRange.BottomRight.Row;
			int index = 0;
			for (int i = firstRow; i <= lastRow; i += 2, index++) {
				if (i + 1 > lastRow)
					break;
				CellRange range = CreateSeriesRange(seriesRange.Worksheet, seriesRange.TopLeft.Column, i, seriesRange.BottomRight.Column, i);
				CellRange sizeRange = CreateSeriesRange(seriesRange.Worksheet, seriesRange.TopLeft.Column, i + 1, seriesRange.BottomRight.Column, i + 1);
				CellRange nameRange = CreateHorizontalSeriesNameRange(seriesNamesRange, i);
				SeriesBase series = CreateBubbleSeries(range, nameRange, sizeRange, arguments, view);
				AddSeries(view, series, index);
			}
		}
		protected override void CreateVerticalSeries(CellRange seriesRange, IDataReference arguments, IChartView view, CellRange seriesNamesRange) {
			int firstColumn = seriesRange.TopLeft.Column;
			int lastColumn = seriesRange.BottomRight.Column;
			int index = 0;
			for (int i = firstColumn; i <= lastColumn; i += 2, index++) {
				if (i + 1 > lastColumn)
					break;
				CellRange range = CreateSeriesRange(seriesRange.Worksheet, i, seriesRange.TopLeft.Row, i, seriesRange.BottomRight.Row);
				CellRange sizeRange = CreateSeriesRange(seriesRange.Worksheet, i + 1, seriesRange.TopLeft.Row, i + 1, seriesRange.BottomRight.Row);
				CellRange nameRange = CreateVerticalSeriesNameRange(seriesNamesRange, i);
				SeriesBase series = CreateBubbleSeries(range, nameRange, sizeRange, arguments, view);
				AddSeries(view, series, index);
			}
		}
		BubbleSeries CreateBubbleSeries(CellRange valuesRange, CellRange nameRange, CellRange sizeRange, IDataReference arguments, IChartView view) {
			BubbleSeries series = (BubbleSeries)CreateSeries(valuesRange, nameRange, arguments, view);
			ChartDataReference sizeDataReference = new ChartDataReference(DocumentModel, view.SeriesDirection, true);
			sizeDataReference.SetRange(sizeRange);
			series.BubbleSize = sizeDataReference;
			return series;
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			BubbleSeries series = new BubbleSeries(view);
			series.Bubble3D = Is3D;
			return series;
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			BubbleChartView view = new BubbleChartView(parent);
			view.Bubble3D = Is3D;
			return view;
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			BubbleChartView view = chartView as BubbleChartView;
			return view != null && view.Bubble3D == Is3D && AreSeriesCompatible(view.Series);
		}
		protected bool AreSeriesCompatible(SeriesCollection seriesCollection) {
			int count = seriesCollection.Count;
			for (int i = 0; i < count; i++) {
				BubbleSeries series = seriesCollection[i] as BubbleSeries;
				if (series == null)
					return false;
				if (series.Bubble3D != Is3D)
					return false;
			}
			return true;
		}
	}
	#endregion
}
