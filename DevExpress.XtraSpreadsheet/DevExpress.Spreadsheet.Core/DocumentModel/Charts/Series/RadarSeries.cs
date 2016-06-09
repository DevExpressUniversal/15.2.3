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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class RadarSeries : SeriesWithDataLabelsAndPoints, ISeriesWithMarker {
		#region Fields
		Marker marker;
		#endregion
		public RadarSeries(IChartView view)
			: base(view) {
			this.marker = new Marker(Parent);
		}
		#region Properties
		public Marker Marker { get { return marker; } }
		#endregion
		#region ISeries
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Radar; } }
		public override ChartType ChartType {
			get {
				RadarChartView view = View as RadarChartView;
				return RadarChartView.GetChartType(GetActualRadarStyle(view.RadarStyle));
			} 
		}
		public override ISeries CloneTo(IChartView view) {
			RadarSeries result = new RadarSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if(view == null)
				return false;
			return view.ViewType == ChartViewType.Radar;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			ISeriesWithMarker series = value as ISeriesWithMarker;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(ISeriesWithMarker value) {
			this.marker.CopyFrom(value.Marker);
		}
		public void SetMarkerStyle(RadarChartStyle style) {
			Marker.Symbol = style == RadarChartStyle.Standard ? MarkerStyle.None : MarkerStyle.Auto;
		}
		public override void ResetToStyle() {
			Marker.ResetToStyle(GetMarkerSymbolToReset());
			base.ResetToStyle();
		}
		protected override MarkerStyle GetMarkerSymbolToReset() {
			return Marker.Symbol != MarkerStyle.None ? MarkerStyle.Auto : MarkerStyle.None;
		}
		protected override bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return (dataPoint.Marker.Symbol == Marker.Symbol) && (Marker.Symbol == MarkerStyle.Auto || Marker.Symbol == MarkerStyle.None) && dataPoint.ShapeProperties.IsAutomatic;
		}
		protected override bool IsLinesOnly { get { return ((RadarChartView)View).RadarStyle != RadarChartStyle.Filled; } }
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			return
				position == DataLabelPosition.Default ||
				position == DataLabelPosition.OutsideEnd;
		}
		internal RadarChartStyle GetActualRadarStyle(RadarChartStyle viewRadarChartStyle) {
			if (viewRadarChartStyle == RadarChartStyle.Filled)
				return viewRadarChartStyle;
			return Marker.Symbol == MarkerStyle.None ? RadarChartStyle.Standard : RadarChartStyle.Marker;
		}
	}
}
