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
namespace DevExpress.XtraSpreadsheet.Model {
	#region RadarChartStyle
	public enum RadarChartStyle {
		Standard,
		Marker,
		Filled
	}
	#endregion
	#region RadarChartView
	public class RadarChartView : ChartViewWithVaryColors {
		#region Static Members
		internal static ChartType GetChartType(RadarChartStyle style) {
			if (style == RadarChartStyle.Filled)
				return ChartType.RadarFilled;
			if (style == RadarChartStyle.Marker)
				return ChartType.RadarMarkers;
			return ChartType.Radar;
		}
		#endregion
		#region Fields
		RadarChartStyle radarStyle = RadarChartStyle.Standard;
		#endregion
		public RadarChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region RadarStyle
		public RadarChartStyle RadarStyle {
			get { return radarStyle; }
			set {
				if(radarStyle == value)
					return;
				SetRadarStyle(value);
			}
		}
		void SetRadarStyle(RadarChartStyle value) {
			RadarChartStylePropertyChangedHistoryItem historyItem = new RadarChartStylePropertyChangedHistoryItem(DocumentModel, this, radarStyle, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetRadarStyleCore(RadarChartStyle value) {
			this.radarStyle = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Radar; } }
		public override ChartType ChartType { get { return GetChartType(GetActualRadarStyle()); } }
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			RadarChartView result = new RadarChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			RadarChartView result = new RadarChartView(Parent);
			result.CopyFromWithoutSeries(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.OutsideEnd; }
		}
		public override ISeries CreateSeriesInstance() {
			RadarSeries result = new RadarSeries(this);
			result.SetMarkerStyle(RadarStyle);
			return result;
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			RadarChartView view = value as RadarChartView;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(RadarChartView value) {
			RadarStyle = value.RadarStyle;
		}
		public RadarChartStyle GetActualRadarStyle() {
			if (Series.Count == 0)
				return RadarStyle;
			RadarSeries series = (RadarSeries)Series[0];
			return series.GetActualRadarStyle(RadarStyle);
		}
	}
	#endregion
}
