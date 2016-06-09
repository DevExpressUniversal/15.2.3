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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ScatterChartStyle
	public enum ScatterChartStyle {
		None,
		Marker,
		Line,
		LineMarker,
		Smooth,
		SmoothMarker
	}
	#endregion
	#region ScatterChartView
	public class ScatterChartView : ChartViewWithVaryColors {
		#region Static Members
		internal static ChartType GetScatterChartType(ScatterChartStyle style) {
			if (style == ScatterChartStyle.Line)
				return ChartType.ScatterLine;
			if (style == ScatterChartStyle.LineMarker)
				return ChartType.ScatterLineMarkers;
			if (style == ScatterChartStyle.Smooth)
				return ChartType.ScatterSmooth;
			if (style == ScatterChartStyle.SmoothMarker)
				return ChartType.ScatterSmoothMarkers;
			return ChartType.ScatterMarkers;
		}
		#endregion
		#region Fields
		ScatterChartStyle scatterStyle = ScatterChartStyle.Marker;
		#endregion
		public ScatterChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region ScatterStyle
		public ScatterChartStyle ScatterStyle {
			get { return scatterStyle; }
			set {
				if(scatterStyle == value)
					return;
				SetScatterStyle(value);
			}
		}
		void SetScatterStyle(ScatterChartStyle value) {
			ScatterChartStylePropertyChangedHistoryItem historyItem = new ScatterChartStylePropertyChangedHistoryItem(DocumentModel, this, scatterStyle, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetScatterStyleCore(ScatterChartStyle value) {
			this.scatterStyle = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Scatter; } }
		public override ChartType ChartType { get { return GetScatterChartType(GetActualScatterStyle()); } }
		public override AxisGroupType AxesType { get { return AxisGroupType.XY; } }
		public override IChartView CloneTo(IChart parent) {
			ScatterChartView result = new ScatterChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			ScatterChartView result = new ScatterChartView(Parent);
			result.CopyFromWithoutSeries(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.Right; }
		}
		public override ISeries CreateSeriesInstance() {
			ScatterSeries result = new ScatterSeries(this);
			result.SetScatterStyle(GetActualScatterStyle());
			return result;
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ScatterChartView view = value as ScatterChartView;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(ScatterChartView value) {
			ScatterStyle = value.ScatterStyle;
		}
		public ScatterChartStyle GetActualScatterStyle() {
			if (Series.Count == 0)
				return ScatterStyle;
			ScatterSeries series = (ScatterSeries)Series[0];
			return series.GetScatterStyle();
		}
	}
	#endregion
}
