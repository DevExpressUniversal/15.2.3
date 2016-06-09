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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using System;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SwiftPlotSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotSeriesView : SwiftPlotSeriesViewBase, ILineSeriesView {
		const int DefaultLineThickness = 1;
		const bool DefaultAntialiasing = false;
		LineStyle lineStyle;
		bool antialiasing = DefaultAntialiasing;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSwiftPlot); } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IXYPoint);
			}
		}
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return true;
			}
		}
		protected override int PixelsPerArgument { get { return 2; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotSeriesViewLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotSeriesView.LineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle LineStyle { get { return lineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotSeriesViewAntialiasing"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotSeriesView.Antialiasing"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool Antialiasing {
			get { return antialiasing; }
			set {
				if (value != antialiasing) {
					SendNotification(new ElementWillChangeNotification(this));
					antialiasing = value;
					RaiseControlChanged();
				}
			}
		}
		public SwiftPlotSeriesView() : base() {
			lineStyle = new LineStyle(this, DefaultLineThickness);
		}
		#region ILineSeriesView implementation
		LineStyle ILineSeriesView.LineStyle { get { return lineStyle; } }
		bool ILineSeriesView.MarkerVisible { get { return false; } }
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "LineStyle")
				return ShouldSerializeLineStyle();
			if(propertyName == "Antialiasing")
				return ShouldSerializeAntialiasing();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineStyle() {
			return LineStyle.ShouldSerialize();
		}
		bool ShouldSerializeAntialiasing() {
			return antialiasing != DefaultAntialiasing;
		}
		void ResetAntialiasing() {
			Antialiasing = DefaultAntialiasing;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLineStyle() ||
				ShouldSerializeAntialiasing();
		}
		#endregion
		int GetLineThickness(SwiftPlotDrawOptions drawOptions, SelectionState selectionState) {
			return GraphicUtils.CorrectThicknessBySelectionState(drawOptions.LineStyle.Thickness, selectionState);
		}
		Color GetLineColor(SwiftPlotDrawOptions drawOptions, SelectionState selectionState) {
			return GraphicUtils.CorrectColorBySelectionState(drawOptions.Color, selectionState);
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYPoint)point).Value);
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			SwiftPlotDrawOptions lineDrawOptions = seriesDrawOptions as SwiftPlotDrawOptions;
			if (lineDrawOptions != null)
				LineSeriesViewPainter.RenderLineLegendMarker(renderer, bounds, lineDrawOptions.LineStyle, GetLineColor(lineDrawOptions, selectionState), LineCap.Flat, null);
		}
		protected override ChartElement CreateObjectForClone() {
			return new SwiftPlotSeriesView();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new SwiftPlotDrawOptions(this);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		internal override SeriesHitTestState CreateHitTestState() {
			return new SeriesHitTestState();
		}
		protected internal override void CalculateAnnotationsAnchorPointsLayout(XYDiagramAnchorPointLayoutList anchorPointLayoutList) {
			IRefinedSeries seriesInfo = anchorPointLayoutList.SeriesData.RefinedSeries;
			for (int index = seriesInfo.MinVisiblePointIndex; index <= seriesInfo.MaxVisiblePointIndex; index++) {
				RefinedPoint refinedPoint = seriesInfo.Points[index];
				IValuePoint valueRefindedPoint = refinedPoint;
				SeriesPoint seriesPoint = refinedPoint.SeriesPoint as SeriesPoint;
				if (!refinedPoint.IsEmpty && seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					foreach (Annotation annotation in seriesPoint.Annotations) {
						XYDiagramMappingBase mapping = anchorPointLayoutList.GetMapping(refinedPoint.Argument, valueRefindedPoint.Value, annotation.ScrollingSupported);
						if (mapping != null)
							anchorPointLayoutList.Add(new AnnotationLayout(annotation, mapping.GetScreenPointNoRound(refinedPoint.Argument, valueRefindedPoint.Value), refinedPoint));
					}
				}
				foreach (RefinedPoint childeRefinedPoint in refinedPoint.Children) {
					seriesPoint = childeRefinedPoint.SeriesPoint as SeriesPoint;
					if (!childeRefinedPoint.IsEmpty && seriesPoint != null && seriesPoint.Annotations.Count > 0) {
						foreach (Annotation annotation in seriesPoint.Annotations) {
							XYDiagramMappingBase mapping = anchorPointLayoutList.GetMapping(refinedPoint.Argument, valueRefindedPoint.Value, annotation.ScrollingSupported);
							if (mapping != null)
								anchorPointLayoutList.Add(new AnnotationLayout(annotation, mapping.GetScreenPointNoRound(refinedPoint.Argument, valueRefindedPoint.Value), refinedPoint));
						}
					}
				}
			}
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			base.CalculateMinMaxPointRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo, snapMode);
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(XYDiagramMappingBase diagramMapping, SeriesLayout seriesLayout) {
			SwiftPlotDiagramMapping swiftPlotMapping = diagramMapping as SwiftPlotDiagramMapping;
			if (swiftPlotMapping == null)
				return null;
			List<Point[]> strips = new List<Point[]>();
			List<Point> points = new List<Point>();
			bool isFirtsPoint = true;
			Point lastPoint = Point.Empty;
			IRefinedSeries seriesInfo = seriesLayout.SeriesData.RefinedSeries;
			for (int index = seriesInfo.MinVisiblePointIndex; index <= seriesInfo.MaxVisiblePointIndex; index++) {
				IValuePoint refinedPoint = seriesInfo.Points[index];
				if (refinedPoint.IsEmpty) {
					isFirtsPoint = true;
					if (points.Count > 0) {
						strips.Add(points.ToArray());
						points = new List<Point>();
					}
				}
				else {
					Point point = swiftPlotMapping.GetRoundedScreenPoint(refinedPoint.Argument, refinedPoint.Value);
					if (isFirtsPoint || point != lastPoint) {
						points.Add(point);
						lastPoint = point;
						isFirtsPoint = false;
					}
				}
			}
			if (points.Count > 0)
				strips.Add(points.ToArray());
			return new SwiftPlotWholeSerieslayout(seriesLayout, strips, GetLineThickness((SwiftPlotDrawOptions)seriesLayout.SeriesData.DrawOptions, seriesLayout.SeriesData.SelectionState), diagramMapping.InflatedBounds);
		}
		protected internal override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			SwiftPlotWholeSerieslayout lineLayout = layout as SwiftPlotWholeSerieslayout;
			SwiftPlotDrawOptions lineDrawOptions = layout.SeriesLayout.SeriesData.DrawOptions as SwiftPlotDrawOptions;
			if (lineDrawOptions != null && lineLayout != null && lineLayout.Strips.Count > 0) {
				Color lineColor = GetLineColor(lineDrawOptions, layout.SeriesLayout.SeriesData.SelectionState);
				int lineThickness = GetLineThickness(lineDrawOptions, layout.SeriesLayout.SeriesData.SelectionState);
				renderer.EnableAntialiasing(antialiasing);
				foreach (Point[] points in lineLayout.Strips)
					renderer.DrawLines(points, lineColor, lineThickness, lineStyle);
				renderer.RestoreAntialiasing();
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ILineSeriesView view = obj as ILineSeriesView;
			if(view != null)
				lineStyle.Assign(view.LineStyle);
			SwiftPlotSeriesView swiftPlotView = obj as SwiftPlotSeriesView;
			if(swiftPlotView != null)
				antialiasing = swiftPlotView.antialiasing;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			SwiftPlotSeriesView view = (SwiftPlotSeriesView)obj;
			return lineStyle.Equals(view.lineStyle) && antialiasing == view.antialiasing;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class SwiftPlotWholeSerieslayout : LineWholeSeriesLayoutBase {
		List<Point[]> strips;
		protected override bool IsStripsNullOrEmpty { get { return strips == null || strips.Count == 0; } }
		public List<Point[]> Strips { get { return strips; } }
		public SwiftPlotWholeSerieslayout(SeriesLayout seriesLayout, List<Point[]> strips, int lineThickness, Rectangle bounds) : base(seriesLayout, lineThickness, bounds, true) {
			this.strips = strips;
		}
		protected override void FillHitTestingGraphicsPath(GraphicsPath path) {
			foreach(Point[] points in strips) {
				if(points.Length > 1) {
					GraphicsPath pointsPath = new GraphicsPath();
					pointsPath.AddLines(points);
					path.AddPath(pointsPath, false);
				}
			}
		}
	}
}
