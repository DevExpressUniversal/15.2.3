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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IXlsChartViewContainer
	public interface IXlsChartViewContainer {
		void Add(XlsChartViewBuilder item);
	}
	#endregion
	#region XlsChartViewBuilder
	public class XlsChartViewBuilder : XlsChartLineFormatContainer, IXlsChartBuilder, IXlsChartDataFormatContainer, IXlsChartTextContainer, IXlsChartDefaultTextContainer {
		#region Fields
		ChartViewType viewType = ChartViewType.None;
		XlsChartDataFormat dataFormat;
		List<int> secondPiePoints;
		readonly XlsChartDataLabelExtContent dataLabelExtContent = new XlsChartDataLabelExtContent();
		readonly XlsChartDefaultTextCollection defaultText = new XlsChartDefaultTextCollection();
		int defaultTextId;
		#endregion
		public XlsChartViewBuilder()
			: base() {
			this.dataFormat = null;
			this.secondPiePoints = new List<int>();
		}
		#region Properties
		public ChartViewType ViewType {
			get { return viewType; }
			set { viewType = value; }
		}
		public bool VaryColors { get; set; }
		public int ViewIndex { get; set; }
		public int ZOrder { get; set; }
		public BarChartDirection BarDirection { get; set; }
		public BarChartGrouping BarGrouping { get; set; }
		public int GapWidth { get; set; }
		public int Overlap { get; set; }
		public ChartGrouping Grouping { get; set; }
		public int FirstSliceAngle { get; set; }
		public int DoughnutHoleSize { get; set; }
		public bool ShowLeaderLines { get; set; }
		public int BubbleScale { get; set; }
		public SizeRepresentsType SizeRepresents { get; set; }
		public bool ShowNegBubbles { get; set; }
		public RadarChartStyle RadarStyle { get; set; }
		public bool ShowCatLabels { get; set; }
		public bool Wireframe { get; set; }
		public bool Shade3D { get; set; }
		public ChartOfPieType OfPieType { get; set; }
		public OfPieSplitType SplitType { get; set; }
		public double SplitPos { get; set; }
		public int SecondPieSize { get; set; }
		public List<int> SecondPiePoints { get { return secondPiePoints; } }
		public int XRotation { get; set; }
		public int YRotation { get; set; }
		public int Perspective { get; set; }
		public int Height { get; set; }
		public int Depth { get; set; }
		public int GapDepth { get; set; }
		public bool HasPerspective { get; set; }
		public bool AutoHeight { get; set; }
		public bool Walls2D { get; set; }
		public bool NotPieChart { get; set; }
		public XlsChartDataFormat DataFormat { get { return dataFormat; } }
		public IXlsChartViewContainer Container { get; set; }
		public XlsChartDataLabelExtContent DataLabelExtContent { get { return dataLabelExtContent; } }
		public XlsChartDefaultTextCollection DefaultText { get { return defaultText; } }
		public int DefaultTextId {
			get { return defaultTextId; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "DefaultTextId");
				defaultTextId = value;
			}
		}
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (Container != null) {
				ViewIndex = contentBuilder.CurrentViewIndex++;
				Container.Add(this);
			}
		}
		public void Build(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			IChartView view = CreateChartView(contentBuilder);
			if (view != null) {
				CreateSeries(contentBuilder, view);
				contentBuilder.CurrentChart.Views.Add(view);
				if (view.Is3DView)
					SetupView3D(contentBuilder.CurrentChart.View3D);
				SetupDropLines(view as ISupportsDropLines);
				SetupHiLowLines(view as ISupportsHiLowLines);
				SetupSeriesLines(view as ISupportsSeriesLines);
				SetupDataLabels(contentBuilder, view as ChartViewWithDataLabels);
				SetupDataFormat(view);
				SetupScatterStyle(view as ScatterChartView);
			}
		}
		#endregion
		#region IXlsChartDataFormatContainer Members
		void IXlsChartDataFormatContainer.Add(XlsChartDataFormat dataFormat) {
			this.dataFormat = dataFormat;
		}
		#endregion
		IChartView CreateChartView(XlsContentBuilder contentBuilder) {
			switch (ViewType) {
				case ChartViewType.Area: return CreateAreaChartView(contentBuilder);
				case ChartViewType.Area3D: return CreateArea3DChartView(contentBuilder);
				case ChartViewType.Bar: return CreateBarChartView(contentBuilder);
				case ChartViewType.Bar3D: return CreateBar3DChartView(contentBuilder);
				case ChartViewType.Bubble: return CreateBubbleChartView(contentBuilder);
				case ChartViewType.Doughnut: return CreateDoughnutChartView(contentBuilder);
				case ChartViewType.Line: return CreateLineChartView(contentBuilder);
				case ChartViewType.Line3D: return CreateLine3DChartView(contentBuilder);
				case ChartViewType.OfPie: return CreateOfPieChartView(contentBuilder);
				case ChartViewType.Pie: return CreatePieChartView(contentBuilder);
				case ChartViewType.Pie3D: return CreatePie3DChartView(contentBuilder);
				case ChartViewType.Radar: return CreateRadarChartView(contentBuilder);
				case ChartViewType.Scatter: return CreateScatterChartView(contentBuilder);
				case ChartViewType.Surface: return CreateSurfaceChartView(contentBuilder);
				case ChartViewType.Surface3D: return CreateSurface3DChartView(contentBuilder);
			}
			return null;
		}
		IChartView CreateBarChartView(XlsContentBuilder contentBuilder) {
			BarChartView view = new BarChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.BarDirection = BarDirection;
			view.Grouping = BarGrouping;
			view.GapWidth = GapWidth;
			view.Overlap = Overlap;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, BarDirection == BarChartDirection.Column ? AxisPosition.Bottom : AxisPosition.Left);
			SetupAxisPosition(view.Axes, 1, BarDirection == BarChartDirection.Column ? AxisPosition.Left : AxisPosition.Bottom);
			return view;
		}
		IChartView CreateBar3DChartView(XlsContentBuilder contentBuilder) {
			Bar3DChartView view = new Bar3DChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.BarDirection = BarDirection;
			view.Grouping = BarGrouping;
			view.GapWidth = GapWidth;
			view.GapDepth = GapDepth;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, BarDirection == BarChartDirection.Column ? AxisPosition.Bottom : AxisPosition.Left);
			SetupAxisPosition(view.Axes, 1, BarDirection == BarChartDirection.Column ? AxisPosition.Left : AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 2, AxisPosition.Bottom);
			return view;
		}
		IChartView CreateLineChartView(XlsContentBuilder contentBuilder) {
			LineChartView view = new LineChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Grouping = Grouping;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			return view;
		}
		IChartView CreateLine3DChartView(XlsContentBuilder contentBuilder) {
			Line3DChartView view = new Line3DChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Grouping = ChartGrouping.Standard;
			view.GapDepth = GapDepth;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			SetupAxisPosition(view.Axes, 2, AxisPosition.Bottom);
			return view;
		}
		IChartView CreatePieChartView(XlsContentBuilder contentBuilder) {
			PieChartView view = new PieChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.VaryColors = VaryColors;
			view.FirstSliceAngle = FirstSliceAngle;
			view.DataLabels.ShowLeaderLines = ShowLeaderLines;
			return view;
		}
		IChartView CreatePie3DChartView(XlsContentBuilder contentBuilder) {
			Pie3DChartView view = new Pie3DChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.VaryColors = VaryColors;
			view.DataLabels.ShowLeaderLines = ShowLeaderLines;
			return view;
		}
		IChartView CreateDoughnutChartView(XlsContentBuilder contentBuilder) {
			DoughnutChartView view = new DoughnutChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.VaryColors = VaryColors;
			view.FirstSliceAngle = FirstSliceAngle;
			view.HoleSize = DoughnutHoleSize;
			view.DataLabels.ShowLeaderLines = ShowLeaderLines;
			return view;
		}
		IChartView CreateAreaChartView(XlsContentBuilder contentBuilder) {
			AreaChartView view = new AreaChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Grouping = Grouping;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			return view;
		}
		IChartView CreateArea3DChartView(XlsContentBuilder contentBuilder) {
			Area3DChartView view = new Area3DChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Grouping = Grouping;
			view.GapDepth = GapDepth;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			SetupAxisPosition(view.Axes, 2, AxisPosition.Bottom);
			return view;
		}
		IChartView CreateBubbleChartView(XlsContentBuilder contentBuilder) {
			BubbleChartView view = new BubbleChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.BubbleScale = BubbleScale;
			view.SizeRepresents = SizeRepresents;
			view.ShowNegBubbles = ShowNegBubbles;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			return view;
		}
		IChartView CreateScatterChartView(XlsContentBuilder contentBuilder) {
			ScatterChartView view = new ScatterChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			return view;
		}
		IChartView CreateRadarChartView(XlsContentBuilder contentBuilder) {
			RadarChartView view = new RadarChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.RadarStyle = RadarStyle;
			view.VaryColors = VaryColors;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			return view;
		}
		IChartView CreateSurfaceChartView(XlsContentBuilder contentBuilder) {
			SurfaceChartView view = new SurfaceChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Wireframe = Wireframe;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			SetupAxisPosition(view.Axes, 2, AxisPosition.Bottom);
			return view;
		}
		IChartView CreateSurface3DChartView(XlsContentBuilder contentBuilder) {
			Surface3DChartView view = new Surface3DChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.Wireframe = Wireframe;
			SetupAxisPosition(view.Axes, 0, AxisPosition.Bottom);
			SetupAxisPosition(view.Axes, 1, AxisPosition.Left);
			SetupAxisPosition(view.Axes, 2, AxisPosition.Bottom);
			return view;
		}
		IChartView CreateOfPieChartView(XlsContentBuilder contentBuilder) {
			OfPieChartView view = new OfPieChartView(contentBuilder.CurrentChart);
			view.Axes = contentBuilder.CurrentAxisGroup;
			view.OfPieType = OfPieType;
			view.SplitType = SplitType;
			view.SplitPos = SplitPos;
			view.SecondPieSize = SecondPieSize;
			view.GapWidth = GapWidth;
			view.SecondPiePoints.InnerList.AddRange(SecondPiePoints);
			return view;
		}
		void SetupAxisPosition(AxisGroup axisGroup, int index, AxisPosition position) {
			if (index < 0 || index >= axisGroup.Count)
				return;
			AxisBase axis = axisGroup[index];
			AxisBase crossesAxis = axis.CrossesAxis;
			if (axis.Crosses == AxisCrosses.Max)
				position = GetOpositePosition(position);
			if (crossesAxis.Scaling.Orientation == AxisOrientation.MaxMin)
				position = GetOpositePosition(position);
			axis.Position = position;
		}
		AxisPosition GetOpositePosition(AxisPosition position) {
			switch (position) {
				case AxisPosition.Left: return AxisPosition.Right;
				case AxisPosition.Top: return AxisPosition.Bottom;
				case AxisPosition.Right: return AxisPosition.Left;
				case AxisPosition.Bottom:
				default:
					return AxisPosition.Top;
			}
		}
		void SetupView3D(View3DOptions options) {
			bool bar3dOrPie3d = (ViewType == ChartViewType.Bar3D && BarDirection == BarChartDirection.Bar) || ViewType == ChartViewType.Pie3D;
			options.XRotation = XRotation;
			options.YRotation = YRotation;
			options.RightAngleAxes = bar3dOrPie3d? true : !HasPerspective;
			options.Perspective = Perspective;
			options.AutoHeight = false;			
			options.HeightPercent = Height;
			options.DepthPercent = Depth;
		}
		void CreateSeries(XlsContentBuilder contentBuilder, IChartView view) {
			List<XlsChartSeriesBuilder> seriesFormats = new List<XlsChartSeriesBuilder>();
			foreach (XlsChartSeriesBuilder builder in contentBuilder.SeriesFormats) {
				if (builder.ViewIndex == ZOrder && !builder.Trendline.Apply)
					seriesFormats.Add(builder);
			}
			for (int i = 0; i < seriesFormats.Count; i++) {
				XlsChartSeriesBuilder builder = seriesFormats[i];
				SeriesBase series = builder.CreateSeries(contentBuilder, view) as SeriesBase;
				if (series != null)
					view.Series.AddCore(series);
			}
			SetupSeriesDirection(view);
		}
		void SetupSeriesDirection(IChartView view) {
			bool firstDirection = true;
			foreach (ISeries series in view.Series) {
				ChartDataReference dataRef = series.Values as ChartDataReference;
				if (dataRef != null) {
					if (firstDirection) {
						firstDirection = false;
						view.Parent.SeriesDirection = dataRef.SeriesDirection;
					}
					else
						dataRef.SeriesDirection = view.SeriesDirection;
				}
				dataRef = series.Arguments as ChartDataReference;
				if (dataRef != null)
					dataRef.SeriesDirection = view.SeriesDirection;
				if (series.SeriesType == ChartSeriesType.Bubble) {
					BubbleSeries bubbleSeries = series as BubbleSeries;
					dataRef = bubbleSeries.BubbleSize as ChartDataReference;
					if (dataRef != null)
						dataRef.SeriesDirection = view.SeriesDirection;
				}
			}
		}
		void SetupDropLines(ISupportsDropLines view) {
			if (view == null)
				return;
			XlsChartShapeFormat props = ShapeFormats.FindBy(XlsChartDefs.DropLinesContext);
			if (props != null) {
				view.ShowDropLines = true;
				props.SetupShapeProperties(view.DropLinesProperties);
			}
			else {
				XlsChartLineFormat format = LineFormats[XlsChartDefs.DropLinesIndex];
				if (format.Apply) {
					view.ShowDropLines = format.LineStyle != XlsChartLineStyle.None;
					format.SetupShapeProperties(view.DropLinesProperties);
				}
			}
		}
		void SetupHiLowLines(ISupportsHiLowLines view) {
			if (view == null)
				return;
			XlsChartShapeFormat props = ShapeFormats.FindBy(XlsChartDefs.HiLowLinesContext);
			if (props != null) {
				view.ShowHiLowLines = true;
				props.SetupShapeProperties(view.HiLowLinesProperties);
			}
			else {
				XlsChartLineFormat format = LineFormats[XlsChartDefs.HiLowLinesIndex];
				if (format.Apply) {
					view.ShowHiLowLines = format.LineStyle != XlsChartLineStyle.None;
					format.SetupShapeProperties(view.HiLowLinesProperties);
				}
			}
		}
		void SetupSeriesLines(ISupportsSeriesLines view) {
			if (view == null || !view.IsSeriesLinesApplicable)
				return;
			XlsChartShapeFormat props = ShapeFormats.FindBy(XlsChartDefs.SeriesLinesContext);
			DocumentModel documentModel = ((ChartViewBase)view).DocumentModel;
			if (props != null) {
				ShapeProperties shapeProperties = new ShapeProperties(documentModel);
				props.SetupShapeProperties(shapeProperties);
				view.SeriesLines.Add(shapeProperties);
			}
			else {
				XlsChartLineFormat format = LineFormats[XlsChartDefs.SeriesLinesIndex];
				if (format.Apply) {
					ShapeProperties shapeProperties = new ShapeProperties(documentModel);
					format.SetupShapeProperties(shapeProperties);
					view.SeriesLines.Add(shapeProperties);
				}
			}
		}
		void SetupDataLabels(XlsContentBuilder contentBuilder, ChartViewWithDataLabels view) {
			if (view == null)
				return;
			XlsChartShapeFormat props = ShapeFormats.FindBy(XlsChartDefs.LeaderLinesContext);
			if (props != null) {
				view.DataLabels.ShowLeaderLines = true;
				props.SetupShapeProperties(view.DataLabels.LeaderLinesProperties);
			}
			else {
				XlsChartLineFormat format = LineFormats[XlsChartDefs.LeaderLinesIndex];
				if (format.Apply) {
					view.DataLabels.ShowLeaderLines = format.LineStyle != XlsChartLineStyle.None;
					format.SetupShapeProperties(view.DataLabels.LeaderLinesProperties);
				}
			}
			foreach (ISeries series in view.Series)
				SetupViewDataLabels(contentBuilder, series as SeriesWithDataLabelsAndPoints);
			view.DataLabels.Apply = true;
		}
		void SetupViewDataLabels(XlsContentBuilder contentBuilder, SeriesWithDataLabelsAndPoints series) {
			if (series == null)
				return;
			if (dataFormat != null && dataFormat.DataLabelFormat.Apply) {
				dataFormat.SetupDataLabels(series);
				series.DataLabels.Apply = true;
			}
			XlsChartTextBuilder text = defaultText.FindBy(defaultTextId);
			if (dataLabelExtContent != null && text != null) {
				dataLabelExtContent.SetupDataLabelsFromExtContent(series.DataLabels);
				series.DataLabels.Apply = true;
			}
		}
		void SetupDataFormat(IChartView view) {
			if (dataFormat != null)
				dataFormat.SetupView(view);
		}
		void SetupScatterStyle(ScatterChartView view) {
			if (view == null || view.Series.Count == 0)
				return;
			ScatterSeries series = view.Series[0] as ScatterSeries;
			view.ScatterStyle = series.Smooth ? ScatterChartStyle.SmoothMarker : ScatterChartStyle.LineMarker;
		}
		protected override int GetIndexByObjContext(int objContext) {
			switch(objContext) {
				case XlsChartDefs.DropLinesContext:
					return XlsChartDefs.DropLinesIndex;
				case XlsChartDefs.HiLowLinesContext:
					return XlsChartDefs.HiLowLinesIndex;
				case XlsChartDefs.LeaderLinesContext:
					return XlsChartDefs.LeaderLinesIndex;
				case XlsChartDefs.SeriesLinesContext:
					return XlsChartDefs.SeriesLinesIndex;
			}
			return objContext;
		}
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			XlsChartDefaultText defText = new XlsChartDefaultText(defaultTextId, item);
			defaultText.Add(defText);
		}
	}
	#endregion
	#region XlsChartDataLabelExtContent
	public class XlsChartDataLabelExtContent {
		#region Properties
		public bool Apply { get; set; }
		public bool ShowSeriesName { get; set; }
		public bool ShowCategoryName { get; set; }
		public bool ShowValue { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowBubbleSizes { get; set; }
		public string Separator { get; set; }
		#endregion
		public void SetupDataLabelsFromExtContent(DataLabelBase dataLabel) {
			if (!Apply)
				return;
			dataLabel.BeginUpdate();
			try {
				dataLabel.ShowCategoryName = ShowCategoryName;
				dataLabel.ShowPercent = ShowPercent;
				dataLabel.ShowSeriesName = ShowSeriesName;
				dataLabel.ShowValue = ShowValue;
				dataLabel.ShowBubbleSize = ShowBubbleSizes;
				dataLabel.Separator = String.IsNullOrEmpty(Separator) ? DataLabelBase.DefaultSeparator : Separator;
			}
			finally {
				dataLabel.EndUpdate();
			}
		}
		public void SetupDataLabelsForAreaOrFilledRadar(DataLabelBase dataLabel) {
			if (!Apply)
				return;
			dataLabel.BeginUpdate();
			try {
				dataLabel.ShowCategoryName = ShowCategoryName;
				dataLabel.Separator = String.IsNullOrEmpty(Separator) ? DataLabelBase.DefaultSeparator : Separator;
			}
			finally {
				dataLabel.EndUpdate();
			}
		}
	}
	#endregion
}
