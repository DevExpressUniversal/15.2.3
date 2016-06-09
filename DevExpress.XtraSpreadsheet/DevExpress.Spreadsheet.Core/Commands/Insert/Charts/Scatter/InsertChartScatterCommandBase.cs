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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartScatterCommandBase (abstract class)
	public abstract class InsertChartScatterCommandBase : InsertChartCommandBase {
		protected InsertChartScatterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract ScatterChartStyle ScatterStyle { get; }
		protected override ChartLayoutModifier Preset { get { return ChartScatterPresets.Instance.DefaultModifier; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Scatter; } }
		protected override void CreateAxes(Chart chart) {
			CreateTwoPrimaryAxes(chart, true, true);
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			return new ScatterSeries(view);
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			ScatterChartView view = new ScatterChartView(parent);
			view.ScatterStyle = GetActualScatterStyle();
			return view;
		}
		protected override void SetupSeries(SeriesBase series, IDataReference arguments, CellRange valuesRange, CellRange nameRange) {
			base.SetupSeries(series, arguments, valuesRange, nameRange);
			SetupScatterSeries((ScatterSeries)series);
		}
		protected override void SetupSeries(SeriesBase series, IDataReference arguments, IDataReference values, IChartText seriesText) {
			base.SetupSeries(series, arguments, values, seriesText);
			SetupScatterSeries((ScatterSeries)series);
		}
		void SetupScatterSeries(ScatterSeries scatterSeries) {
			scatterSeries.Smooth = ScatterStyle == ScatterChartStyle.Smooth || ScatterStyle == ScatterChartStyle.SmoothMarker;
			if (ScatterStyle == ScatterChartStyle.Marker)
				scatterSeries.ShapeProperties.Outline.Fill = DrawingFill.None;
			if (ScatterStyle == ScatterChartStyle.Line || ScatterStyle == ScatterChartStyle.Smooth)
				scatterSeries.Marker.Symbol = MarkerStyle.None;
		}
		ScatterChartStyle GetActualScatterStyle() {
			switch (ScatterStyle) {
				case ScatterChartStyle.Line:
				case ScatterChartStyle.LineMarker:
				case ScatterChartStyle.Marker:
					return ScatterChartStyle.LineMarker;
				case ScatterChartStyle.Smooth:
				case ScatterChartStyle.SmoothMarker:
					return ScatterChartStyle.SmoothMarker;
			}
			return ScatterChartStyle.None;
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			ScatterChartView view = chartView as ScatterChartView;
			return view != null && view.GetActualScatterStyle() == ScatterStyle;
		}
	}
	#endregion
}
