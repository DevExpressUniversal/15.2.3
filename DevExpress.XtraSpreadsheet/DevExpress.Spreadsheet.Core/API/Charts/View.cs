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

using System.Collections.Generic;
namespace DevExpress.Spreadsheet.Charts {
	#region BubbleSizeRepresents
	public enum BubbleSizeRepresents {
		Area = DevExpress.XtraSpreadsheet.Model.SizeRepresentsType.Area,
		Width = DevExpress.XtraSpreadsheet.Model.SizeRepresentsType.Width
	}
	#endregion
	#region OfPieSplitType
	public enum OfPieSplitType {
		Auto = DevExpress.XtraSpreadsheet.Model.OfPieSplitType.Auto,
		Custom = DevExpress.XtraSpreadsheet.Model.OfPieSplitType.Custom,
		Percent = DevExpress.XtraSpreadsheet.Model.OfPieSplitType.Percent,
		Position = DevExpress.XtraSpreadsheet.Model.OfPieSplitType.Position,
		Value = DevExpress.XtraSpreadsheet.Model.OfPieSplitType.Value
	}
	#endregion
	public interface UpDownBarsOptions {
		bool Visible { get; set; }
		int GapWidth { get; set; }
		ShapeFormat UpBars { get; }
		ShapeFormat DownBars { get; }
		void ResetToMatchStyle();
	}
	public interface ChartView {
		ChartType ViewType { get; }
		SeriesCollection Series { get; }
		DataLabelOptions DataLabels { get; }
		int BubbleScale { get; set; }
		bool ShowNegativeBubbles { get; set; }
		BubbleSizeRepresents SizeRepresents { get; set; }
		int FirstSliceAngle { get; set; }
		int HoleSize { get; set; }
		int GapDepth { get; set; }
		int GapWidth { get; set; }
		int Overlap { get; set; }
		OfPieSplitType SplitType { get; set; }
		double SplitPosition { get; set; }
		int[] SecondPiePoints { get; set; }
		int SecondPieSize { get; set; }
		bool VaryColors { get; set; }
		ChartLineOptions DropLines { get; }
		ChartLineOptions HighLowLines { get; }
		ChartLineOptions SeriesLines { get; }
		UpDownBarsOptions UpDownBars { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.API.Internal;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	#region NativeUpDownBarsOptions
	partial class NativeUpDownBarsOptions : NativeObjectBase, UpDownBarsOptions {
		readonly Model.ISupportsUpDownBars modelView;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeFormat nativeUpBarsFormat;
		NativeShapeFormat nativeDownBarsFormat;
		public NativeUpDownBarsOptions(Model.ISupportsUpDownBars modelView, NativeWorkbook nativeWorkbook) {
			this.modelView = modelView;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeUpBarsFormat != null)
				nativeUpBarsFormat.IsValid = value;
			if (nativeDownBarsFormat != null)
				nativeDownBarsFormat.IsValid = value;
		}
		public bool Visible {
			get {
				CheckValid();
				return modelView != null ? modelView.ShowUpDownBars : false;
			}
			set {
				CheckValid();
				if (modelView != null)
					modelView.ShowUpDownBars = value;
			}
		}
		public int GapWidth {
			get {
				CheckValid();
				return modelView != null ? modelView.UpDownBars.GapWidth : 0;
			}
			set {
				CheckValid();
				if(modelView != null)
					modelView.UpDownBars.GapWidth = value;
			}
		}
		public ShapeFormat UpBars {
			get {
				CheckValid();
				if (modelView == null)
					return null;
				if (nativeUpBarsFormat == null)
					nativeUpBarsFormat = new NativeShapeFormat(modelView.UpDownBars.UpBarsProperties, nativeWorkbook);
				return nativeUpBarsFormat;
			}
		}
		public ShapeFormat DownBars {
			get {
				CheckValid();
				if (modelView == null)
					return null;
				if (nativeDownBarsFormat == null)
					nativeDownBarsFormat = new NativeShapeFormat(modelView.UpDownBars.DownBarsProperties, nativeWorkbook);
				return nativeDownBarsFormat;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			if (modelView == null)
				return;
			modelView.UpDownBars.DocumentModel.BeginUpdate();
			try {
				modelView.UpDownBars.ResetToStyle();
			}
			finally {
				modelView.UpDownBars.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region NativeChartView
	partial class NativeChartView : NativeObjectBase, ChartView {
		#region Fields
		readonly Model.IChartView modelView;
		readonly NativeChart nativeChart;
		NativeViewSeriesCollection series;
		NativeDataLabelOptions dataLabels;
		NativeChartLineOptions dropLines;
		NativeChartLineOptions highLowLines;
		NativeSeriesLinesOptions seriesLines;
		NativeUpDownBarsOptions upDownBars;
		#endregion
		public NativeChartView(Model.IChartView modelView, NativeChart nativeChart)
			: base() {
			this.modelView = modelView;
			this.nativeChart = nativeChart;
		}
		NativeWorkbook NativeWorkbook { get { return nativeChart.NativeWorkbook; } }
		#region ChartView Members
		public ChartType ViewType {
			get {
				CheckValid();
				return (ChartType)modelView.ChartType;
			}
		}
		public SeriesCollection Series {
			get {
				CheckValid();
				if (series == null)
					series = new NativeViewSeriesCollection(modelView, nativeChart);
				return series;
			}
		}
		public DataLabelOptions DataLabels {
			get {
				CheckValid();
				Model.ChartViewWithDataLabels view = modelView as Model.ChartViewWithDataLabels;
				if (view == null)
					return null;
				if (dataLabels == null)
					dataLabels = new NativeDataLabelOptions(view.DataLabels, NativeWorkbook);
				return dataLabels;
			}
		}
		#region BubbleView properties
		public int BubbleScale {
			get {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				return view != null ? view.BubbleScale : 100;
			}
			set {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				if (view != null)
					view.BubbleScale = value;
			}
		}
		public bool ShowNegativeBubbles {
			get {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				return view != null ? view.ShowNegBubbles : true;
			}
			set {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				if (view != null)
					view.ShowNegBubbles = value;
			}
		}
		public BubbleSizeRepresents SizeRepresents {
			get {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				return view != null ? (BubbleSizeRepresents)view.SizeRepresents : BubbleSizeRepresents.Area;
			}
			set {
				CheckValid();
				Model.BubbleChartView view = modelView as Model.BubbleChartView;
				if (view != null)
					view.SizeRepresents = (Model.SizeRepresentsType)value;
			}
		}
		#endregion
		#region Pie/Doughnut properties
		public int FirstSliceAngle {
			get {
				CheckValid();
				Model.ChartViewWithSlice view = modelView as Model.ChartViewWithSlice;
				return view != null ? view.FirstSliceAngle : 0;
			}
			set {
				CheckValid();
				Model.ChartViewWithSlice view = modelView as Model.ChartViewWithSlice;
				if (view != null) {
					value = value % 360;
					if (value < 0)
						value += 360;
					view.FirstSliceAngle = value;
				}
			}
		}
		public int HoleSize {
			get {
				CheckValid();
				Model.DoughnutChartView view = modelView as Model.DoughnutChartView;
				return view != null ? view.HoleSize : 0;
			}
			set {
				CheckValid();
				Model.DoughnutChartView view = modelView as Model.DoughnutChartView;
				if (view != null)
					view.HoleSize = value;
			}
		}
		#endregion
		#region Bar/Bar3D view properties
		public int GapDepth {
			get {
				CheckValid();
				Model.IChartViewWithGapDepth view = modelView as Model.IChartViewWithGapDepth;
				return view != null ? view.GapDepth : 150;
			}
			set {
				CheckValid();
				Model.IChartViewWithGapDepth view = modelView as Model.IChartViewWithGapDepth;
				if (view != null)
					view.GapDepth = value;
			}
		}
		public int GapWidth {
			get {
				CheckValid();
				Model.IChartViewWithGapWidth view = modelView as Model.IChartViewWithGapWidth;
				return view != null ? view.GapWidth : 150;
			}
			set {
				CheckValid();
				Model.IChartViewWithGapWidth view = modelView as Model.IChartViewWithGapWidth;
				if (view != null)
					view.GapWidth = value;
			}
		}
		public int Overlap {
			get {
				CheckValid();
				Model.BarChartView view = modelView as Model.BarChartView;
				return view != null ? view.Overlap : 0;
			}
			set {
				CheckValid();
				Model.BarChartView view = modelView as Model.BarChartView;
				if (view != null)
					view.Overlap = value;
			}
		}
		#endregion
		#region OfPieView properties
		public OfPieSplitType SplitType {
			get {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				return view != null ? (OfPieSplitType)view.SplitType : OfPieSplitType.Auto;
			}
			set {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				if (view != null)
					view.SplitType = (Model.OfPieSplitType)value;
			}
		}
		public double SplitPosition {
			get {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				return view != null ? view.SplitPos : 1.0;
			}
			set {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				if (view != null)
					view.SplitPos = value;
			}
		}
		public int[] SecondPiePoints {
			get {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				if (view != null)
					return view.SecondPiePoints.InnerList.ToArray();
				return new int[0];
			}
			set {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				if (view != null) {
					view.DocumentModel.BeginUpdate();
					try {
						view.SecondPiePoints.Clear();
						if (value != null) {
							foreach (int point in value)
								view.SecondPiePoints.Add(point);
						}
					}
					finally {
						view.DocumentModel.EndUpdate();
					}
				}
			}
		}
		public int SecondPieSize {
			get {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				return view != null ? view.SecondPieSize : 75;
			}
			set {
				CheckValid();
				Model.OfPieChartView view = modelView as Model.OfPieChartView;
				if (view != null)
					view.SecondPieSize = value;
			}
		}
		#endregion
		#region VaryColors
		public bool VaryColors {
			get {
				CheckValid();
				Model.ChartViewWithVaryColors view = modelView as Model.ChartViewWithVaryColors;
				return view != null ? view.VaryColors : false;
			}
			set {
				CheckValid();
				Model.ChartViewWithVaryColors view = modelView as Model.ChartViewWithVaryColors;
				if (view != null)
					view.VaryColors = value;
			}
		}
		#endregion
		#region Lines and UpDownBars
		public ChartLineOptions DropLines {
			get {
				CheckValid();
				Model.ISupportsDropLines view = modelView as Model.ISupportsDropLines;
				if (view == null)
					return null;
				if (dropLines == null)
					dropLines = new NativeChartLineOptions(new DropLinesAdapter(view), NativeWorkbook);
				return dropLines; 
			}
		}
		public ChartLineOptions HighLowLines {
			get {
				CheckValid();
				Model.ISupportsHiLowLines view = modelView as Model.ISupportsHiLowLines;
				if (view == null)
					return null;
				if (highLowLines == null)
					highLowLines = new NativeChartLineOptions(new HiLowLinesAdapter(view), NativeWorkbook);
				return highLowLines;
			}
		}
		public ChartLineOptions SeriesLines {
			get { 
				CheckValid();
				Model.ISupportsSeriesLines view = modelView as Model.ISupportsSeriesLines;
				if (view == null || !view.IsSeriesLinesApplicable)
					return null;
				if (seriesLines == null)
					seriesLines = new NativeSeriesLinesOptions(view.SeriesLines, NativeWorkbook);
				return seriesLines;
			}
		}
		public UpDownBarsOptions UpDownBars {
			get {
				CheckValid();
				Model.ISupportsUpDownBars view = modelView as Model.ISupportsUpDownBars;
				if (view == null)
					return null;
				if (upDownBars == null)
					upDownBars = new NativeUpDownBarsOptions(view, NativeWorkbook);
				return upDownBars;
			}
		}
		#endregion
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (series != null)
				series.IsValid = value;
			if (dataLabels != null)
				dataLabels.IsValid = value;
			if (dropLines != null)
				dropLines.IsValid = value;
			if (highLowLines != null)
				highLowLines.IsValid = value;
			if (seriesLines != null)
				seriesLines.IsValid = value;
			if (upDownBars != null)
				upDownBars.IsValid = value;
		}
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeChartView other = obj as NativeChartView;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelView, other.modelView);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelView.GetHashCode();
		}
	}
	#endregion
}
