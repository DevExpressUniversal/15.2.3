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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Line3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Line3DSeriesView : XYDiagram3DSeriesViewBase, IGeometryStripCreator, IGeometryHolder {
		static bool CalcJointPoint(Line line1, Line line2, out DiagramPoint point) {
			Intersection intersection = IntersectionUtils.CalcLinesIntersection2D(line1.V1, line1.V2, line2.V1, line2.V2,
				true, Diagram3D.Epsilon, out point);
			switch (intersection) {
				case Intersection.Yes:
					point.Z = line1.V2.Z;
					return true;
				case Intersection.No:
					double angle = Math.Abs(MathUtils.CalcAngleBetweenLines2D(line1.V1, line1.V2, line2.V1, line2.V2, Diagram3D.Epsilon));
					bool result = ComparingUtils.CompareDoubles(angle, Math.PI / 2, Diagram3D.Epsilon) <= 0 &&
						IntersectionUtils.CalcLinesIntersection2D(line1.V1, line1.V2, line2.V1, line2.V2, false, Diagram3D.Epsilon, out point) == Intersection.Yes;
					point.Z = line1.V2.Z;
					return result;
				default:
					return false;
			}
		}
		protected static List<SeriesPointLayout> ExtractSeriesPointViewInfo(List<SeriesPointLayout> pointsLayout, ref int index) {
			List<SeriesPointLayout> result = new List<SeriesPointLayout>();
			for (; index < pointsLayout.Count; index++) {
				View3DSeriesPointLayout pointLayout = pointsLayout[index] as View3DSeriesPointLayout;
				if (pointLayout != null)
					result.Add(pointLayout);
				else if (result.Count > 0)
					return result;
			}
			return result;
		}
		protected static void CalcLines(DiagramPoint point1, DiagramPoint point2, float offset, out Line topLine, out Line bottomLine) {
			double z = point1.Z;
			float length = (float)MathUtils.CalcLength2D(point1, point2);
			PointF[] points = new PointF[] { new PointF(0, -offset), new PointF(length, -offset), new PointF(0, offset), new PointF(length, offset) };
			MathUtils.CalcTranslateMatrix(point1, point2, Diagram3D.Epsilon).TransformPoints(points);
			bottomLine = new Line(new DiagramPoint(points[0].X, points[0].Y, z), new DiagramPoint(points[1].X, points[1].Y, z));
			topLine = new Line(new DiagramPoint(points[2].X, points[2].Y, z), new DiagramPoint(points[3].X, points[3].Y, z));
		}
		protected static void CalcJoints(ref Line line1, ref Line line2, out DiagramPoint jointPoint, out Line jointLine, bool revertJointLine) {
			if (CalcJointPoint(line1, line2, out jointPoint)) {
				double angle = MathUtils.CalcAngleBetweenLines2D(line1.V1, jointPoint, line1.V1, line1.V2, Diagram3D.Epsilon);
				line1 = ComparingUtils.CompareDoubles(Math.Abs(angle), Math.PI, Diagram3D.Epsilon) == 0 ?
					new Line(jointPoint, line1.V1) : new Line(line1.V1, jointPoint);
				angle = MathUtils.CalcAngleBetweenLines2D(jointPoint, line2.V2, line2.V1, line2.V2, Diagram3D.Epsilon);
				line2 = ComparingUtils.CompareDoubles(Math.Abs(angle), Math.PI, Diagram3D.Epsilon) == 0 ?
					new Line(line2.V2, jointPoint) : new Line(jointPoint, line2.V2);
				jointLine = null;
			}
			else {
				jointLine = revertJointLine ? new Line(line2.V1, line1.V2) : new Line(line1.V2, line2.V1);
				jointPoint = MathUtils.CalcIntervalCenter2D(line1.V2, line2.V1, Diagram3D.Epsilon);
			}
		}
		const double DefaultLineWidth = 0.6;
		const int DefaultLineThickness = 5;
		int lineThickness = DefaultLineThickness;
		double lineWidth = DefaultLineWidth;
		readonly LineStyle markerlineStyle, legendLineStyle;
		protected override int PixelsPerArgument { get { return 20; } }
		protected LineStyle LegendLineStyle { get { return legendLineStyle; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IXYPoint);
			}
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnLine3D); } }
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal virtual bool InterlacedPoints { get { return false; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(XYDiagram3D); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Line3DSeriesViewLineThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Line3DSeriesView.LineThickness"),
		XtraSerializableProperty
		]
		public int LineThickness {
			get { return lineThickness; }
			set {
				if (value != lineThickness) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineThickness));
					SendNotification(new ElementWillChangeNotification(this));
					lineThickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Line3DSeriesViewLineWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Line3DSeriesView.LineWidth"),
		XtraSerializableProperty
		]
		public double LineWidth {
			get { return lineWidth; }
			set {
				if (value != lineWidth) {
					if (value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineWidth));
					SendNotification(new ElementWillChangeNotification(this));
					lineWidth = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Obsolete("The MarkerLineVisible property is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public bool MarkerLineVisible { get { return false; } set { } }
		[
		Obsolete("The MarkerLineStyle property is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle MarkerLineStyle { get { return markerlineStyle; } }
		[
		Obsolete("The MarkerLineColor property is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public Color MarkerLineColor { get { return Color.Empty; } set { } }
		public Line3DSeriesView() : base() {
			markerlineStyle = new LineStyle(this, 1, false);
			legendLineStyle = new LineStyle(this, 1, true);
		}
		#region IGeometryStripCreator implementation
		IGeometryStrip IGeometryStripCreator.CreateStrip() {
			return CreateStripInternal();
		}
		#endregion
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return CreateStripCreator();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LineThickness":
					return ShouldSerializeLineThickness();
				case "LineWidth":
					return ShouldSerializeLineWidth();
				case "MarkerLineVisible":
					return ShouldSerializeMarkerLineVisible();
				case "MarkerLineStyle":
					return ShouldSerializeMarkerLineStyle();
				case "MarkerLineColor":
					return ShouldSerializeMarkerLineColor();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineThickness() {
			return lineThickness != DefaultLineThickness;
		}
		void ResetLineThickness() {
			LineThickness = DefaultLineThickness;
		}
		bool ShouldSerializeLineWidth() {
			return lineWidth != DefaultLineWidth;
		}
		void ResetLineWidth() {
			LineWidth = DefaultLineWidth;
		}
		bool ShouldSerializeMarkerLineVisible() {
			return false;
		}
		bool ShouldSerializeMarkerLineStyle() {
			return false;
		}
		bool ShouldSerializeMarkerLineColor() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLineThickness() || ShouldSerializeLineWidth();
		}
		#endregion
		List<PlanePolygon> CreateLineSegmentPolygons(XYDiagram3DCoordsCalculator coordsCalculator, LineStrip strip, List<SeriesPointLayout> pointsLayout, bool interlacedPoints, double lineWidth, int seriesWeight, int lineThickness, Color color) {
			if (strip.Count == 0)
				return new List<PlanePolygon>();
			double z = coordsCalculator.CalcSeriesOffset(Series);
			if (strip.IsEmpty)
				return CreatePolygonsForSinglePoint(coordsCalculator, strip[0], z, pointsLayout[0], lineWidth, seriesWeight, lineThickness, color);
			float halfThickness = (float)(lineThickness / 2.0);
			List<PlanePolygon> polygons = new List<PlanePolygon>();
			DiagramPoint previousPoint = new DiagramPoint(strip[0].X, strip[0].Y, z);
			Line previousTopLine = null;
			Line previousBottomLine = null;
			bool leftVisible = true;
			for (int i = 1; i < strip.Count; i++) {
				DiagramPoint point = new DiagramPoint(strip[i].X, strip[i].Y, z);
				if (!MathUtils.ArePointsEquals2D(point, previousPoint, Diagram3D.Epsilon)) {
					Line topLine, bottomLine;
					CalcLines(previousPoint, point, halfThickness, out topLine, out bottomLine);
					if (previousTopLine != null && previousBottomLine != null) {
						DiagramPoint topJointPoint, bottomJointPoint;
						Line topJointLine, bottomJointLine;
						CalcJoints(ref previousTopLine, ref topLine, out topJointPoint, out topJointLine, true);
						CalcJoints(ref previousBottomLine, ref bottomLine, out bottomJointPoint, out bottomJointLine, false);
						bool rightVisible = false;
						if (topJointLine != null && bottomJointLine != null) {
							double angle = MathUtils.CalcAngleBetweenLines2D(previousTopLine.V1, previousTopLine.V2,
								topLine.V1, topLine.V2, Diagram3D.Epsilon);
							if (ComparingUtils.CompareDoubles(Math.Abs(angle), Math.PI, Diagram3D.Epsilon) == 0)
								rightVisible = true;
						}
						else if (topJointLine != null) {
							List<PlanePolygon> jointPolygons = SeriesView3DHelper.GetPolygons(coordsCalculator,
								bottomJointPoint, topJointLine, true, false, lineWidth, seriesWeight, color);
							polygons.AddRange(jointPolygons);
						}
						else if (bottomJointLine != null) {
							List<PlanePolygon> jointPolygons = SeriesView3DHelper.GetPolygons(coordsCalculator,
								topJointPoint, bottomJointLine, true, false, lineWidth, seriesWeight, color);
							polygons.AddRange(jointPolygons);
						}
						List<PlanePolygon> polygonsToAdd = SeriesView3DHelper.GetPolygons(coordsCalculator,
							previousTopLine, previousBottomLine, leftVisible, rightVisible, lineWidth, seriesWeight, color);
						polygons.AddRange(polygonsToAdd);
						leftVisible = false;
					}
					previousTopLine = topLine;
					previousBottomLine = bottomLine;
				}
				previousPoint = point;
			}
			if (previousBottomLine != null && previousTopLine != null)
				polygons.AddRange(SeriesView3DHelper.GetPolygons(coordsCalculator,
					previousTopLine, previousBottomLine, leftVisible, true, lineWidth, seriesWeight, color));
			return polygons;
		}
		protected List<PlanePolygon> CreatePolygonsForSinglePoint(XYDiagram3DCoordsCalculator coordsCalculator, GRealPoint2D point, double z, SeriesPointLayout layout, double lineWidth, int seriesWeight, int lineThickness, Color color) {
			double halfThickness = lineThickness / 2.0;
			double xMin = point.X - halfThickness;
			double xMax = point.X + halfThickness;
			double yMin = point.Y - halfThickness;
			double yMax = point.Y + halfThickness;
			return SeriesView3DHelper.GetPolygons(coordsCalculator,
				new Line(new DiagramPoint(xMin, yMax, z), new DiagramPoint(xMax, yMax, z)),
				new Line(new DiagramPoint(xMin, yMin, z), new DiagramPoint(xMax, yMin, z)),
				true, true, lineWidth, seriesWeight, color);
		}
		protected virtual GeometryStripCreator CreateStripCreator() {
			return new LineGeometryStripCreator(false);
		}
		protected virtual List<PlanePolygon> CalculatePolygons(XYDiagram3DCoordsCalculator coordsCalculator, XYDiagram3DSeriesLayout seriesLayout, IList<IGeometryStrip> strips) {
			double lineWidth = coordsCalculator.CalcSeriesWidth(this);
			int seriesWeight = coordsCalculator.GetSeriesWeight(Series);
			Color color = seriesLayout.SeriesData.DrawOptions.Color;
			bool interlacedPoints = InterlacedPoints;
			int pointIndex = 0;
			List<PlanePolygon> result = new List<PlanePolygon>();
			foreach (LineStrip strip in StripsUtils.MapLineStrips(coordsCalculator, strips)) {
				List<PlanePolygon> polygons = CreateLineSegmentPolygons(coordsCalculator, strip,
					ExtractSeriesPointViewInfo(seriesLayout, ref pointIndex), interlacedPoints, lineWidth, seriesWeight, lineThickness, color);
				result.AddRange(polygons);
			}
			return result;
		}
		protected virtual IGeometryStrip CreateStripInternal() {
			return new LineStrip();
		}
		protected override DiagramPoint? CalculateAnnotationAnchorPoint(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, IAxisRangeData axisRangeY) {
			MinMaxValues values = GetSeriesPointValues(pointData.RefinedPoint);
			return coordsCalculator.Project(coordsCalculator.GetDiagramPoint(Series, ((IXYPoint)pointData.RefinedPoint).Argument, values.Max, false));
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions lineDrawOptions = seriesPointDrawOptions as Line3DDrawOptions;
			if (lineDrawOptions == null)
				return;
			int lineLevel = bounds.Top + (bounds.Height - 1) / 2;
			renderer.EnableAntialiasing(legendLineStyle.AntiAlias);
			renderer.DrawLine(new Point((int)bounds.Left, (int)lineLevel), new Point((int)bounds.Right, (int)lineLevel), lineDrawOptions.Color, 1, legendLineStyle, LineCap.Round);
			renderer.RestoreAntialiasing();
		}
		protected override ChartElement CreateObjectForClone() {
			return new Line3DSeriesView();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IXYPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IXYPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYPoint)point).Value);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Line3DSeriesLabel();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new Line3DDrawOptions(this);
		}
		protected internal override double GetSeriesDepth() {
			return lineWidth;
		}
		protected internal virtual MinMaxValues GetSeriesPointValues(RefinedPoint refinedPoint) {
			return new MinMaxValues(0.0, ((IXYPoint)refinedPoint).Value);
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData) {
			return new View3DSeriesPointLayout(pointData);
		}
		protected internal override WholeSeriesViewData CalculateWholeSeriesViewData(RefinedSeriesData seriesData, GeometryCalculator geometryCalculator) {
			return new LineAndAreaWholeSeriesViewData(geometryCalculator.CreateStrips(seriesData.RefinedSeries));
		}
		protected internal override XYDiagram3DWholeSeriesLayout CalculateWholeSeriesLayout(XYDiagram3DCoordsCalculator coordsCalculator, SeriesLayout seriesLayout) {
			XYDiagram3DSeriesLayout layout = seriesLayout as XYDiagram3DSeriesLayout;
			LineAndAreaWholeSeriesViewData viewData = layout != null ? layout.SeriesData.WholeViewData as LineAndAreaWholeSeriesViewData : null;
			return viewData == null ? null : new Line3DWholeSeriesLayout(CalculatePolygons(coordsCalculator, layout, viewData.Strips));
		}
		protected internal override void FillPrimitivesContainer(XYDiagram3DWholeSeriesLayout layout, PrimitivesContainer container) {
			Line3DWholeSeriesLayout lineLayout = layout as Line3DWholeSeriesLayout;
			if (lineLayout != null)
				container.Polygons.AddRange(lineLayout.Polygons);
		}
		internal override SeriesHitTestState CreateHitTestState() {
			return new SeriesHitTestState();
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Line3DSeriesView view = obj as Line3DSeriesView;
			if (view == null)
				return;
			lineThickness = view.lineThickness;
			lineWidth = view.lineWidth;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			Line3DSeriesView view = (Line3DSeriesView)obj;
			return lineThickness == view.lineThickness && lineWidth == view.lineWidth;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class Line3DWholeSeriesLayout : XYDiagram3DWholeSeriesLayout {
		List<PlanePolygon> polygons;
		public List<PlanePolygon> Polygons { get { return polygons; } }
		public Line3DWholeSeriesLayout(List<PlanePolygon> polygons) {
			this.polygons = polygons;
		}
	}
}
