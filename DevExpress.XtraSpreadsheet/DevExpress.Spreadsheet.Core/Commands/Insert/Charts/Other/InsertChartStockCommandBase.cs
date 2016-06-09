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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartStockCommandBase (abstract class)
	public abstract class InsertChartStockCommandBase : InsertChartCommandBase {
		protected InsertChartStockCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool ShowUpDownBars { get; }
		protected abstract bool ShowHiLowLines { get; }
		protected override ChartLayoutModifier Preset { get { return ChartStockPresets.Instance.DefaultModifier; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Stock; } }
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			LineSeries series = new LineSeries(view);
			series.ShapeProperties.Outline.Fill = DrawingFill.None;
			series.Marker.Symbol = MarkerStyle.None;
			return series;
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			StockChartView view = new StockChartView(parent);
			view.ShowUpDownBars = ShowUpDownBars;
			view.ShowHiLowLines = ShowHiLowLines;
			return view;
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			StockChartView view = chartView as StockChartView;
			return view != null && view.ShowUpDownBars == this.ShowUpDownBars && view.ShowHiLowLines == this.ShowHiLowLines;
		}
	}
	#endregion
	#region InsertChartStockVolumeCommandBase (abstract class)
	public abstract class InsertChartStockVolumeCommandBase : InsertChartStockCommandBase {
		protected InsertChartStockVolumeCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract ChartType TargetChartType { get; }
		protected internal override void ExecuteCore() {
			CellRange range = Selection.ActiveRange;
			if (range == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				CreateChartCommand command = new CreateChartCommand(ActiveSheet, ErrorHandler, TargetChartType, range);
				if (!command.Execute())
					return;
				Chart chart = command.Chart;
				chart.BeginUpdate();
				try {
					CalculateChartPosition(chart.DrawingObject);
					SetupChart(chart);
					ApplyPreset(chart);
				}
				finally {
					chart.EndUpdate();
				}
				ActiveSheet.InsertChart(chart);
				ActiveSheet.Selection.SetSelectedDrawingIndex(chart.IndexInCollection);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void ChangeChartTypeCore(Chart chart) {
			chart.BeginUpdate();
			try {
				Model.ChangeChartTypeCommand command = new Model.ChangeChartTypeCommand(ErrorHandler, chart, TargetChartType);
				command.Execute();
			}
			finally {
				chart.EndUpdate();
			}
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			return null;
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			return null;
		}
	}
	#endregion
}
