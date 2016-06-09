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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ScatterSeries : SeriesWithMarkerAndSmooth {
		public ScatterSeries(IChartView view)
			: base(view) {
		}
		#region ISeries
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Scatter; } }
		public override ChartType ChartType { get { return ScatterChartView.GetScatterChartType(GetScatterStyle()); } }
		public override ISeries CloneTo(IChartView view) {
			ScatterSeries result = new ScatterSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if(view == null)
				return false;
			return view.ViewType == ChartViewType.Scatter;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		protected override bool IsLinesOnly { get {  return true; } }
		public void SetScatterStyle(ScatterChartStyle style) {
			Smooth = style == ScatterChartStyle.Smooth || style == ScatterChartStyle.SmoothMarker;
			if (style == ScatterChartStyle.Marker)
				ShapeProperties.Outline.Fill = DrawingFill.None;
			else if(ShapeProperties.Outline.Fill.FillType == DrawingFillType.None)
				ShapeProperties.Outline.Fill = DrawingFill.Automatic;
			if (style == ScatterChartStyle.Line || style == ScatterChartStyle.Smooth)
				Marker.Symbol = MarkerStyle.None;
			else if (Marker.Symbol == MarkerStyle.None)
				Marker.Symbol = MarkerStyle.Auto;
		}
		public ScatterChartStyle GetScatterStyle() {
			if (Smooth)
				return Marker.Symbol != MarkerStyle.None ? ScatterChartStyle.SmoothMarker : ScatterChartStyle.Smooth;
			if (Marker.Symbol != MarkerStyle.None)
				return ShapeProperties.Outline.Fill.FillType == DrawingFillType.None ? ScatterChartStyle.Marker : ScatterChartStyle.LineMarker;
			return ScatterChartStyle.Line;
		}
		protected internal ScatterChartStyle GetActualScatterStyle() {
			if (Marker.Symbol != MarkerStyle.None) {
				if (ShapeProperties.Outline.Fill.FillType == DrawingFillType.None)
					return ScatterChartStyle.Marker;
				return Smooth ? ScatterChartStyle.SmoothMarker : ScatterChartStyle.LineMarker;
			}
			return Smooth ? ScatterChartStyle.Smooth : ScatterChartStyle.Line;
		}
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			return
				position == DataLabelPosition.Default ||
				position == DataLabelPosition.Left ||
				position == DataLabelPosition.Top ||
				position == DataLabelPosition.Right ||
				position == DataLabelPosition.Bottom ||
				position == DataLabelPosition.Center;
		}
	}
}
