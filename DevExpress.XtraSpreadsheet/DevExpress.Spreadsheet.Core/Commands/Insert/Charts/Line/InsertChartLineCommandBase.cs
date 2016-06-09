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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartLineCommandBase (abstract class)
	public abstract class InsertChartLineCommandBase : InsertChartCommandBase {
		protected InsertChartLineCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract ChartGrouping Grouping { get; }
		protected abstract bool ShowMarkers { get; }
		protected override ChartLayoutModifier Preset { get { return ChartLinePresets.Instance.DefaultModifier; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Line; } }
		protected override void CreateAxes(Chart chart) {
			base.CreateAxes(chart);
			ApplyPercentFormatOnValueAxis(chart, Grouping == ChartGrouping.PercentStacked);
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			LineSeries series = new LineSeries(view);
			series.Marker.Symbol = ShowMarkers ? MarkerStyle.Auto : MarkerStyle.None;
			return series;
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			LineChartView view = new LineChartView(parent);
			view.Grouping = Grouping;
			view.ShowMarker = ShowMarkers;
			return view;
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			LineChartView view = chartView as LineChartView;
			return view != null && view.Grouping == this.Grouping && (view.ShowMarker == this.ShowMarkers || AreSeriesHaveMarkers(view.Series) == ShowMarkers);
		}
		protected bool AreSeriesHaveMarkers(SeriesCollection seriesCollection) {
			int count = seriesCollection.Count;
			for (int i = 0; i < count; i++) {
				LineSeries series = seriesCollection[i] as LineSeries;
				if (series.Marker.Symbol == MarkerStyle.None)
					return false;
			}
			return true;
		}
	}
	#endregion
}
