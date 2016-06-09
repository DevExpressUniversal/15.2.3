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

using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region LineChartView
	public class LineChartView : ChartViewWithGroupingAndDropLines, ISupportsHiLowLines, ISupportsUpDownBars {
		#region Fields
		ShapeProperties hiLowLinesProperties;
		ChartUpDownBars upDownBars;
		#endregion
		public LineChartView(IChart parent)
			: base(parent) {
			this.hiLowLinesProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.upDownBars = new ChartUpDownBars(parent);
		}
		#region Properties
		#region ShowHiLowLines
		public bool ShowHiLowLines {
			get { return Info.ShowHiLowLines; }
			set {
				if(ShowHiLowLines == value)
					return;
				SetPropertyValue(SetShowHiLowLinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowHiLowLinesCore(ChartViewInfo info, bool value) {
			info.ShowHiLowLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties HiLowLinesProperties { get { return hiLowLinesProperties; } }
		#region ShowMarker
		public bool ShowMarker {
			get { return Info.ShowMarker; }
			set {
				if(ShowMarker == value)
					return;
				SetPropertyValue(SetShowMarkerCore, value);
			}
		}
		DocumentModelChangeActions SetShowMarkerCore(ChartViewInfo info, bool value) {
			info.ShowMarker = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Smooth
		public bool Smooth {
			get { return Info.Smooth; }
			set {
				if(Smooth == value)
					return;
				SetPropertyValue(SetSmoothCore, value);
			}
		}
		DocumentModelChangeActions SetSmoothCore(ChartViewInfo info, bool value) {
			info.Smooth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowUpDownBars
		public bool ShowUpDownBars {
			get { return Info.ShowUpDownBars; }
			set {
				if(ShowUpDownBars == value)
					return;
				SetPropertyValue(SetShowUpDownBarsCore, value);
			}
		}
		DocumentModelChangeActions SetShowUpDownBarsCore(ChartViewInfo info, bool value) {
			info.ShowUpDownBars = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ChartUpDownBars UpDownBars { get { return upDownBars; } }
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Line; } }
		public override ChartType ChartType { get { return GetActualChartStyle(ShowMarker); } }
		internal ChartType GetActualChartStyle(bool showMarker) {
			if (Grouping == ChartGrouping.Standard)
				return showMarker ? ChartType.LineMarker : ChartType.Line;
			if (Grouping == ChartGrouping.Stacked)
				return showMarker ? ChartType.LineStackedMarker : ChartType.LineStacked;
			return showMarker ? ChartType.LineFullStackedMarker : ChartType.LineFullStacked;
		}
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			LineChartView result = new LineChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			LineChartView result = new LineChartView(Parent);
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
			LineSeries result = new LineSeries(this);
			if (Series.Count == 0)
				result.SetMarkerSymbol(ShowMarker);
			else {
				ISeriesWithMarker markerSeries = Series[0] as ISeriesWithMarker;
				if (markerSeries != null)
					result.Marker.Symbol = markerSeries.Marker.Symbol;
			}
			return result;
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ISupportsHiLowLines viewWithHiLowLines = value as ISupportsHiLowLines;
			if (viewWithHiLowLines != null)
				hiLowLinesProperties.CopyFrom(viewWithHiLowLines.HiLowLinesProperties);
			ISupportsUpDownBars viewWithUpDownBars = value as ISupportsUpDownBars;
			if (viewWithUpDownBars != null)
				upDownBars.CopyFrom(viewWithUpDownBars.UpDownBars);
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			HiLowLinesProperties.ResetToStyle();
			UpDownBars.ResetToStyle();
		}
	}
	#endregion
	#region Line3DChartView
	public class Line3DChartView : ChartViewWithGroupingAndDropLines, IChartViewWithGapDepth {
		int gapDepth = 150;
		public Line3DChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region GapDepth
		public int GapDepth {
			get { return gapDepth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapDepth");
				if(gapDepth == value)
					return;
				SetGapDepth(value);
			}
		}
		void SetGapDepth(int value) {
			ChartViewGapDepthPropertyChangedHistoryItem historyItem = new ChartViewGapDepthPropertyChangedHistoryItem(DocumentModel, this, gapDepth, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetGapDepthCore(int value) {
			this.gapDepth = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Line3D; } }
		public override ChartType ChartType { get { return ChartType.Line3D; } }
		public override bool Is3DView { get { return true; } }
		public override AxisGroupType AxesType { get { return Grouping == ChartGrouping.Standard ? AxisGroupType.CategoryValueSeries : AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			Line3DChartView result = new Line3DChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.Right; }
		}
		public override ISeries CreateSeriesInstance() {
			return new LineSeries(this);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			IChartViewWithGapDepth view = value as IChartViewWithGapDepth;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(IChartViewWithGapDepth value) {
			GapDepth = value.GapDepth;
		}
	}
	#endregion
}
