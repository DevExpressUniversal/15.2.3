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
	#region InsertChartRadarCommandBase (abstract class)
	public abstract class InsertChartRadarCommandBase : InsertChartCommandBase {
		protected InsertChartRadarCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract RadarChartStyle RadarStyle { get; }
		protected override ChartLayoutModifier Preset { get { return ChartRadarPresets.Instance.DefaultModifier; } }
		protected abstract bool HasMarkers { get; }
		protected override ChartViewType ViewType { get { return ChartViewType.Radar; } }
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			RadarSeries series = new RadarSeries(view);
			series.Marker.Symbol = HasMarkers ? MarkerStyle.Auto : MarkerStyle.None;
			return series;
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			RadarChartView view = new RadarChartView(parent);
			view.RadarStyle = RadarStyle;
			return view;
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			RadarChartView view = chartView as RadarChartView;
			return view != null && view.RadarStyle == this.RadarStyle && AreSeriesHaveMarkers(view.Series) == HasMarkers;
		}
		protected bool AreSeriesHaveMarkers(SeriesCollection seriesCollection) {
			int count = seriesCollection.Count;
			for (int i = 0; i < count; i++) {
				RadarSeries series = seriesCollection[i] as RadarSeries;
				if (series.Marker.Symbol == MarkerStyle.None)
					return false;
			}
			return true;
		}
	}
	#endregion
}
