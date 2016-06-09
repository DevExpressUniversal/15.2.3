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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(FunnelSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FunnelSeriesView : FunnelSeriesViewBase, ISimpleDiagram2DSeriesView {
		const bool DefaultAlignToCenter = false;
		const bool DefaultHeightToWidthRatioAuto = true;
		CustomBorder border;
		PolygonFillStyle fillStyle;
		bool alignToCenter = DefaultAlignToCenter;
		bool heightToWidthRatioAuto = DefaultHeightToWidthRatioAuto;
		new FunnelSeriesLabel Label { get { return (FunnelSeriesLabel)base.Label; } }
		FunnelSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance.FunnelSeriesViewAppearance;
			}
		}
		Color BorderColorFromAppearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				if (actualAppearance != null)
					return actualAppearance.FunnelSeriesViewAppearance.BorderColor;
				else
					return Color.Empty;
			}
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnFunnel); } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return true; } }
		protected internal override int ActualBorderThickness { get { return Border.ActualThickness; } }
		protected internal override bool IsSupportedToolTips { get { return true; } }
		protected internal override bool ActualHeightToWidthRatioAuto {
			get { return heightToWidthRatioAuto; }
		}		
		protected override bool Is3DView { get { return false; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IValuePoint);
			}
		}
		internal PolygonFillStyle ActualFillStyle {
			get {
				if (fillStyle.FillMode != FillMode.Empty)
					return fillStyle;
				else
					return Appearance.FillStyle;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(SimpleDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesView.Border"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border { get { return border; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesView.FillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle FillStyle { get { return this.fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewAlignToCenter"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesView.AlignToCenter"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool AlignToCenter {
			get { return alignToCenter; }
			set {
				if (alignToCenter == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				this.alignToCenter = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("FunnelSeriesViewHeightToWidthRatioAuto"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesView.HeightToWidthRatioAuto"),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool HeightToWidthRatioAuto {
			get { return heightToWidthRatioAuto; }
			set {
				if (heightToWidthRatioAuto == value)
					return;
				SendNotification(new ElementWillChangeNotification(this));
				heightToWidthRatioAuto = value;
				RaiseControlChanged();
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.FunnelSeriesView.Color"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color Color { get { return Color.Empty; } set { } }
		public FunnelSeriesView() : base() {
			this.border = new CustomBorder(this, true, Color.Empty);
			this.fillStyle = new PolygonFillStyle(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Border")
				return ShouldSerializeBorder();
			if (propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			if (propertyName == "AlignToCenter")
				return ShouldSerializeAlignToCenter();
			if (propertyName == "HeightToWidthRatioAuto")
				return ShouldSerializeHeightToWidthRatioAuto();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		bool ShouldSerializeAlignToCenter() {
			return alignToCenter != DefaultAlignToCenter;
		}
		void ResetAlignToCenter() {
			AlignToCenter = DefaultAlignToCenter;
		}
		bool ShouldSerializeHeightToWidthRatioAuto() {
			return heightToWidthRatioAuto != DefaultHeightToWidthRatioAuto;
		}
		void ResetHeightToWidthRatioAuto() {
			HeightToWidthRatioAuto = DefaultHeightToWidthRatioAuto;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBorder() ||
				ShouldSerializeFillStyle() ||
				ShouldSerializeAlignToCenter() ||
				ShouldSerializeHeightToWidthRatioAuto();
		}
		#endregion
		LineStrip CorrectFunnelPolygon(FunnelSeriesPointLayout layout) {
			LineStrip vertices = new LineStrip();
			vertices.Add(layout.LeftUpPoint);
			vertices.Add(new GRealPoint2D(layout.RightUpPoint.X + 1, layout.RightUpPoint.Y));
			GRealPoint2D? p = Funnel2DLayoutCalculator.CalcIntersectionPoint(
				new GRealPoint2D(layout.RightUpPoint.X + 1, layout.RightUpPoint.Y),
				new GRealPoint2D(layout.RightDownPoint.X + 1, layout.RightDownPoint.Y), 
				layout.RightDownPoint.Y + 1);
			if (p.HasValue)
				vertices.Add(p.Value);
			else
				vertices.Add(layout.RightDownPoint);
			p = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.LeftUpPoint, layout.LeftDownPoint, layout.LeftDownPoint.Y + 1);
			if (p.HasValue)
				vertices.Add(p.Value);
			else
				vertices.Add(layout.LeftDownPoint);
			return vertices;
		}
		LineStrip CorrectLegendMarkerPolygon(VariousPolygon polygon) {
			if (polygon.Vertices.Count < 6)
				return polygon.Vertices;
			LineStrip vertices = new LineStrip();
			vertices.Add(polygon.Vertices[0]);
			vertices.Add(new GRealPoint2D(polygon.Vertices[1].X + 1, polygon.Vertices[1].Y));
			vertices.Add(new GRealPoint2D(polygon.Vertices[2].X + 1, polygon.Vertices[2].Y));
			vertices.Add(new GRealPoint2D(polygon.Vertices[3].X + 1, polygon.Vertices[3].Y + 1));
			vertices.Add(new GRealPoint2D(polygon.Vertices[4].X, polygon.Vertices[4].Y + 1));
			vertices.Add(polygon.Vertices[5]);
			return vertices;
		}
		DiagramPoint? CalculateAchorPoint(SeriesPointLayout pointLayout) {
			FunnelSeriesPointLayout funnelLayout = pointLayout as FunnelSeriesPointLayout;
			if (funnelLayout == null) {
				ChartDebug.Fail("FunnelSeriesPointLayout expected.");
				return null;
			}
			return new DiagramPoint((funnelLayout.LeftUpPoint.X + funnelLayout.RightUpPoint.X) / 2,
						(funnelLayout.RightUpPoint.Y + funnelLayout.RightDownPoint.Y) / 2);
		}
		GraphicsCommand CreateFunnelGraphicsCommand(FunnelSeriesPointLayout layout, FunnelDrawOptions funnelDrawOptions, Color color) {
			if (layout == null)
				return null;
			GraphicsCommand command = new SimpleAntialiasingGraphicsCommand(PixelOffsetMode.HighQuality);
			RectangleF polygonRect = layout.Polygon.Rect;
			bool isLastPoint = layout.PointOrder == PointOrder.Last || layout.PointOrder == PointOrder.Single;
			double height = PointDistance > 0 || isLastPoint ? polygonRect.Height + 1 : polygonRect.Height;
			GraphicsCommand clipping = new ClippingGraphicsCommand((ZPlaneRectangle)(new RectangleF(polygonRect.Location, new SizeF(polygonRect.Width + 1, (float)height))));
			command.AddChildCommand(clipping);
			FillOptionsBase options = funnelDrawOptions.FillStyle.Options;
			LineStrip vertices = CorrectFunnelPolygon(layout);
			if (!layout.CheckWidth())
				clipping.AddChildCommand(new SolidLineGraphicsCommand(new DiagramPoint(layout.LeftUpPoint.X, layout.LeftUpPoint.Y), new DiagramPoint(layout.LeftDownPoint.X, layout.LeftDownPoint.Y), color, 1));
			if (options != null)
				clipping.AddChildCommand(options.CreateGraphicsCommand(vertices, (ZPlaneRectangle)polygonRect, color, funnelDrawOptions.ActualColor2));
			else
				clipping.AddChildCommand(new SolidPolygonGraphicsCommand(vertices, color));
			return command;
		}
		void RenderFunnel(IRenderer renderer, FunnelSeriesPointLayout layout, FunnelDrawOptions funnelDrawOptions, Color color) {
			if (layout == null)
				return;
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.HighQuality);
			RectangleF polygonRect = layout.Polygon.Rect;
			bool isLastPoint = layout.PointOrder == PointOrder.Last || layout.PointOrder == PointOrder.Single;
			double height = PointDistance > 0 || isLastPoint ? polygonRect.Height + 1 : polygonRect.Height;
			RectangleF clip = new RectangleF(polygonRect.Location, new SizeF(polygonRect.Width + 1, (float)height));
			renderer.SetClipping((RectangleF)clip, CombineMode.Intersect);
			FillOptionsBase options = funnelDrawOptions.FillStyle.Options;
			LineStrip vertices = CorrectFunnelPolygon(layout);
			if (!layout.CheckWidth())
				renderer.DrawLine(new Point((int)layout.LeftUpPoint.X, (int)layout.LeftUpPoint.Y), new Point((int)layout.LeftDownPoint.X, (int)layout.LeftDownPoint.Y), color, 1);
			if (options != null)
				options.Render(renderer, vertices, (RectangleF)polygonRect, color, funnelDrawOptions.ActualColor2);
			else
				renderer.FillPolygon(vertices, color);
			renderer.RestoreClipping();
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		GraphicsCommand CreateBorderGraphicsCommand(FunnelSeriesPointLayout layout, FunnelDrawOptions funnelDrawOptions, Rectangle mappingBounds) {
			Color borderColor = BorderHelper.CalculateBorderColor(funnelDrawOptions.Border, BorderColorFromAppearance);
			int actualBorderThickness = funnelDrawOptions.Border.ActualThickness;
			if (actualBorderThickness == 1)
				return new BoundedPolygonGraphicsCommand(layout.Polygon.Vertices, borderColor, actualBorderThickness);
			GraphicsCommand command = new ContainerGraphicsCommand();
			ClippingGraphicsCommand clippingCommand;
			using (GraphicsPath path = layout.Polygon.GetPath())
				clippingCommand = new ClippingGraphicsCommand(new Region(path));
			command.AddChildCommand(clippingCommand);
			ContainerGraphicsCommand antiAliasCommand = new ContainerGraphicsCommand();
			clippingCommand.AddChildCommand(antiAliasCommand);
			ContainerGraphicsCommand borderAntiAlias = new ContainerGraphicsCommand();
			command.AddChildCommand(borderAntiAlias);
			GraphicsCommand flankSidesCommand = CreateFlankSidesCommand(layout, mappingBounds, actualBorderThickness, borderColor);
			antiAliasCommand.AddChildCommand(flankSidesCommand);
			antiAliasCommand.AddChildCommand(CreateHorizontalSidesCommand(layout, actualBorderThickness, borderColor));
			borderAntiAlias.AddChildCommand(new BoundedPolygonGraphicsCommand(layout.Polygon.Vertices, borderColor, 1));
			return command;
		}
		void RenderBorder(IRenderer renderer, FunnelSeriesPointLayout layout, FunnelDrawOptions funnelDrawOptions, Rectangle mappingBounds) {
			Color borderColor = BorderHelper.CalculateBorderColor(funnelDrawOptions.Border, BorderColorFromAppearance);
			int actualBorderThickness = funnelDrawOptions.Border.ActualThickness;
			if (actualBorderThickness == 1)
				renderer.DrawPolygon(layout.Polygon.Vertices, borderColor, actualBorderThickness);
			else {
				using (GraphicsPath path = layout.Polygon.GetPath()) {
					renderer.SetClipping(path, CombineMode.Intersect);
					RenderFlankSides(renderer, layout, mappingBounds, actualBorderThickness, borderColor);
					RenderHorizontalSides(renderer, layout, actualBorderThickness, borderColor);
					renderer.DrawPolygon(layout.Polygon.Vertices, borderColor, 1);
					renderer.RestoreClipping();
				}
			}
		}
		GraphicsCommand CreateFlankSidesCommand(FunnelSeriesPointLayout layout, Rectangle mappingBounds, int actualBorderThickness, Color borderColor) {
			GraphicsCommand command = new ContainerGraphicsCommand();
			GRealPoint2D? point1 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.LeftUpPoint, layout.LeftDownPoint, mappingBounds.Bottom);
			GRealPoint2D? point2 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.LeftUpPoint, layout.LeftDownPoint, mappingBounds.Top);
			if (!point1.HasValue)
				point1 = layout.LeftDownPoint;
			if (!point2.HasValue)
				point2 = layout.LeftUpPoint;
			command.AddChildCommand(new SolidLineGraphicsCommand(new DiagramPoint(point1.Value.X, point1.Value.Y), new DiagramPoint(point2.Value.X, point2.Value.Y), borderColor, actualBorderThickness * 2 - 1));
			point1 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.RightUpPoint, layout.RightDownPoint, mappingBounds.Bottom);
			point2 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.RightUpPoint, layout.RightDownPoint, mappingBounds.Top);
			if (!point1.HasValue)
				point1 = layout.RightDownPoint;
			if (!point2.HasValue)
				point2 = layout.RightUpPoint;
			command.AddChildCommand(new SolidLineGraphicsCommand(new DiagramPoint(point1.Value.X, point1.Value.Y), new DiagramPoint(point2.Value.X, point2.Value.Y), borderColor, actualBorderThickness * 2 - 1));
			return command;
		}
		void RenderFlankSides(IRenderer renderer, FunnelSeriesPointLayout layout, Rectangle mappingBounds, int actualBorderThickness, Color borderColor) {
			GRealPoint2D? point1 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.LeftUpPoint, layout.LeftDownPoint, mappingBounds.Bottom);
			GRealPoint2D? point2 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.LeftUpPoint, layout.LeftDownPoint, mappingBounds.Top);
			if (!point1.HasValue)
				point1 = layout.LeftDownPoint;
			if (!point2.HasValue)
				point2 = layout.LeftUpPoint;
			renderer.DrawLine(new Point((int)point1.Value.X, (int)point1.Value.Y), new Point((int)point2.Value.X, (int)point2.Value.Y), borderColor, actualBorderThickness * 2 - 1);
			point1 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.RightUpPoint, layout.RightDownPoint, mappingBounds.Bottom);
			point2 = Funnel2DLayoutCalculator.CalcIntersectionPoint(layout.RightUpPoint, layout.RightDownPoint, mappingBounds.Top);
			if (!point1.HasValue)
				point1 = layout.RightDownPoint;
			if (!point2.HasValue)
				point2 = layout.RightUpPoint;
			renderer.DrawLine(new Point((int)point1.Value.X, (int)point1.Value.Y), new Point((int)point2.Value.X, (int)point2.Value.Y), borderColor, actualBorderThickness * 2 - 1);
		}
		GraphicsCommand CreateHorizontalSidesCommand(FunnelSeriesPointLayout layout, int actualBorderThickness, Color borderColor) {
			GraphicsCommand command = new ContainerGraphicsCommand();
			int bottomBorderThickness = actualBorderThickness;
			int topBorderThickness = actualBorderThickness;
			bool even = actualBorderThickness % 2 == 0;
			if (layout.PointOrder == PointOrder.First || layout.PointOrder == PointOrder.Single || PointDistance > 0)
				topBorderThickness = actualBorderThickness * 2 - 1;
			else if (even)
				topBorderThickness = actualBorderThickness - 1;
			if (layout.PointOrder == PointOrder.Last || layout.PointOrder == PointOrder.Single || PointDistance > 0)
				bottomBorderThickness = actualBorderThickness * 2 - 1;
			else if (even)
				bottomBorderThickness = actualBorderThickness + 1;
			command.AddChildCommand(new SolidLineGraphicsCommand(new DiagramPoint(layout.LeftUpPoint.X, layout.LeftUpPoint.Y), new DiagramPoint(layout.RightUpPoint.X, layout.RightUpPoint.Y), borderColor, topBorderThickness));
			command.AddChildCommand(new SolidLineGraphicsCommand(new DiagramPoint(layout.LeftDownPoint.X, layout.LeftDownPoint.Y), new DiagramPoint(layout.RightDownPoint.X, layout.RightDownPoint.Y), borderColor, bottomBorderThickness));
			return command;
		}
		void RenderHorizontalSides(IRenderer renderer, FunnelSeriesPointLayout layout, int actualBorderThickness, Color borderColor) {
			int bottomBorderThickness = actualBorderThickness;
			int topBorderThickness = actualBorderThickness;
			bool even = actualBorderThickness % 2 == 0;
			if (layout.PointOrder == PointOrder.First || layout.PointOrder == PointOrder.Single || PointDistance > 0)
				topBorderThickness = actualBorderThickness * 2 - 1;
			else if (even)
				topBorderThickness = actualBorderThickness - 1;
			if (layout.PointOrder == PointOrder.Last || layout.PointOrder == PointOrder.Single || PointDistance > 0)
				bottomBorderThickness = actualBorderThickness * 2 - 1;
			else if (even)
				bottomBorderThickness = actualBorderThickness + 1;
			renderer.DrawLine(new Point((int)layout.LeftUpPoint.X, (int)layout.LeftUpPoint.Y), new Point((int)layout.RightUpPoint.X, (int)layout.RightUpPoint.Y), borderColor, topBorderThickness);
			renderer.DrawLine(new Point((int)layout.LeftDownPoint.X, (int)layout.LeftDownPoint.Y), new Point((int)layout.RightDownPoint.X, (int)layout.RightDownPoint.Y), borderColor, bottomBorderThickness);
		}
		GraphicsCommand CreateHatchedGraphicsCommand(SelectionState selectionState, LineStrip vertices) {
			SeriesHitTestState hitState = Series.HitState;
			if (selectionState != SelectionState.Normal)
				return new HatchedPolygonGraphicsCommand(vertices, HatchStyle.WideUpwardDiagonal, hitState.GetPointHatchColor(Chart.SeriesSelectionMode, selectionState));
			return null;
		}
		void RenderHatched(IRenderer renderer, SelectionState selectionState, LineStrip vertices) {
			if (selectionState != SelectionState.Normal)
				renderer.FillPolygon(vertices, HatchStyle.WideUpwardDiagonal, HitTestState.GetPointHatchColor(Chart.SeriesSelectionMode, selectionState));
		}
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		}
		protected override GraphicsCommand CreateGraphicsCommand(FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds) {
			FunnelDrawOptions funnelDrawOptions = drawOptions as FunnelDrawOptions;
			if (funnelDrawOptions == null || layout == null)
				return null;
			Color color = funnelDrawOptions.Color;
			if (layout.IsNegativeValuePresents) {
				color = Color.FromArgb(40, drawOptions.Color);
				funnelDrawOptions.FillStyle.FillMode = FillMode.Solid;
			}
			GraphicsCommand rootCommand = new ContainerGraphicsCommand();
			rootCommand.AddChildCommand(CreateFunnelGraphicsCommand(layout, funnelDrawOptions, color));
			GraphicsCommand command = new SimpleAntialiasingGraphicsCommand();
			command.AddChildCommand(CreateHatchedGraphicsCommand(layout.PointData.SelectionState, layout.Polygon.Vertices));
			if (funnelDrawOptions.Border.ActualVisibility)
				command.AddChildCommand(CreateBorderGraphicsCommand(layout, funnelDrawOptions, mappingBounds));
			rootCommand.AddChildCommand(command);
			return rootCommand;
		}
		protected override void Render(IRenderer renderer, FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds) {
			FunnelDrawOptions funnelDrawOptions = drawOptions as FunnelDrawOptions;
			if (funnelDrawOptions == null || layout == null)
				return;
			Color color = funnelDrawOptions.Color;
			if (layout.IsNegativeValuePresents) {
				color = Color.FromArgb(40, drawOptions.Color);
				funnelDrawOptions.FillStyle.FillMode = FillMode.Solid;
			}
			RenderFunnel(renderer, layout, funnelDrawOptions, color);
			renderer.EnableAntialiasing(true);
			RenderHatched(renderer, layout.PointData.SelectionState, layout.Polygon.Vertices);
			if (funnelDrawOptions.Border.ActualVisibility)
				RenderBorder(renderer, layout, funnelDrawOptions, mappingBounds);
			renderer.RestoreAntialiasing();
		}
		protected override void RenderLegendMarker(IRenderer renderer, VariousPolygon polygon, SimpleDiagramDrawOptionsBase drawOptions, SelectionState selectionState) {
			FunnelDrawOptions funnelDrawOptions = drawOptions as FunnelDrawOptions;
			if (funnelDrawOptions == null)
				return;
			renderer.EnableAntialiasing(true);
			LineStrip vertices = CorrectLegendMarkerPolygon(polygon);
			renderer.SetPixelOffsetMode(PixelOffsetMode.HighQuality);
			FillOptionsBase options = funnelDrawOptions.FillStyle.Options;
			if (options != null)
				options.Render(renderer, vertices, (RectangleF)polygon.Rect, funnelDrawOptions.Color, funnelDrawOptions.ActualColor2);
			else
				renderer.FillPolygon(vertices, funnelDrawOptions.Color);
			renderer.RestorePixelOffsetMode();
			RenderHatched(renderer, selectionState, polygon.Vertices);
			if (funnelDrawOptions.Border.ActualVisibility)
				renderer.DrawPolygon(polygon.Vertices, funnelDrawOptions.Border.Color, 1);
			renderer.RestoreAntialiasing();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IValuePoint)point).Value);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new FunnelDrawOptions(this);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new FunnelSeriesLabel();
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		}
		protected internal override bool CalculateBounds(RefinedSeriesData seriesData, Rectangle outerBounds, out Rectangle innerBounds, out Rectangle labelBounds) {
			innerBounds = outerBounds;
			labelBounds = outerBounds;
			if (!outerBounds.AreWidthAndHeightPositive())
				return false;
			Size maximumLabelSize = Label.CalculateMaximumSizeConsiderIndent(seriesData);
			if (maximumLabelSize.Height > 0) {
				innerBounds.Offset(0, maximumLabelSize.Height / 2);
				innerBounds.Height -= maximumLabelSize.Height / 2 + 1;
			}
			innerBounds = CorrectBoundsBySizeAndRatio(innerBounds);
			if (!innerBounds.AreWidthAndHeightPositive())
				return false;
			innerBounds = Label.CorrectFunnelBounds(seriesData, innerBounds, outerBounds);
			labelBounds = innerBounds;
			labelBounds.Width += maximumLabelSize.Width;
			if (Label.Position == FunnelSeriesLabelPosition.LeftColumn)
				labelBounds.Offset(-maximumLabelSize.Width, 0);
			labelBounds = Rectangle.Intersect(labelBounds, outerBounds);
			return innerBounds.AreWidthAndHeightPositive();
		}
		protected internal override int CalculateHeightOfPolygon(int height, List<RefinedPointData> filteredPointsData, out int actualPointDistance, out int residueHeightOfPolygon, out int residuePointDistance) {
			int heightOfPolygon = base.CalculateHeightOfPolygon(height, filteredPointsData, out actualPointDistance, out residueHeightOfPolygon, out residuePointDistance);
			int count = filteredPointsData.Count;
			if (heightOfPolygon == 0 || count <= 0)
				return heightOfPolygon;
			int halfLabelHeight = 0;
			if (filteredPointsData[0].LabelViewData.Length != 0)
				halfLabelHeight = filteredPointsData[0].LabelViewData[0].TextSize.Height / 2 + Label.Border.ActualThickness;
			if (count == 1 || Label.Position == FunnelSeriesLabelPosition.Center || heightOfPolygon >= halfLabelHeight)
				return heightOfPolygon;
			residueHeightOfPolygon = 0;
			residuePointDistance = 0;
			int newHeight = height - halfLabelHeight;
			if (newHeight < (count - 1))
				return 0;
			int tempPointDistance = actualPointDistance = PointDistance == 0 ? 0 : PointDistance + 1;
			if (PointDistance != 0 && (newHeight - (count - 1)) < ((count - 1) * tempPointDistance)) {
				actualPointDistance = (newHeight - (count - 1)) / (count - 1);
				residuePointDistance = (newHeight - (count - 1)) % (count - 1);
				heightOfPolygon = 1;
			}
			else {
				int dividend = newHeight - (count - 1) * tempPointDistance;
				heightOfPolygon = dividend / (count - 1);
				residueHeightOfPolygon = dividend % (count - 1);
			}
			return heightOfPolygon;
		}
		protected override ChartElement CreateObjectForClone() {
			return new FunnelSeriesView();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISimpleDiagram2DSeriesView view = obj as ISimpleDiagram2DSeriesView;
			if (view == null)
				return;
			this.border.Assign(view.Border);
			this.fillStyle.Assign(view.FillStyle);
			FunnelSeriesView seriesView = obj as FunnelSeriesView;
			if (seriesView == null)
				return;
			this.alignToCenter = seriesView.alignToCenter;
			this.heightToWidthRatioAuto = seriesView.heightToWidthRatioAuto;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			FunnelSeriesView view = (FunnelSeriesView)obj;
			return
				border.Equals(view.border) &&
				fillStyle.Equals(view.fillStyle) &&
				alignToCenter == view.alignToCenter &&
				heightToWidthRatioAuto == view.heightToWidthRatioAuto;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
