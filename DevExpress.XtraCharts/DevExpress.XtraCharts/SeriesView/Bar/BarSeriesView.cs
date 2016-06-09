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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	public abstract class BarSeriesView : SeriesViewColorEachSupportBase, IBarSeriesView, ITransparableView, ISupportBorderVisibility {
		const int legendMarkerShadowSize = 1;
		const double DefaultBarWidth = 0.6;
		static internal Color CalculateBorderColor(BarDrawOptions drawOptions) {
			Color automaticBorderColor = HitTestColors.MixColors(Color.FromArgb(100, 0, 0, 0), drawOptions.Color);
			automaticBorderColor = Color.FromArgb(90, automaticBorderColor);
			return BorderHelper.CalculateBorderColor(drawOptions.Border, automaticBorderColor);
		}
		readonly byte DefaultOpacity = ConvertBetweenOpacityAndTransparency(0);
		double barWidth = DefaultBarWidth;
		byte opacity;
		RectangularBorder border;
		RectangleFillStyle fillStyle;
		BarSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance != null ? actualAppearance.BarSeriesViewAppearance : null;
			}
		}
		internal RectangleFillStyle ActualFillStyle {
			get {
				if(this.fillStyle.FillMode != FillMode.Empty)
					return this.fillStyle;
				else
					return Appearance.FillStyle;
			}
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected internal override Color ActualColor {
			get {
				Color actualColor = base.ActualColor;
				if (PaletteColorUsed)
					actualColor = ConvertToTransparentColor(actualColor, opacity);
				return actualColor;
			}
		}
		protected internal override Color ActualColor2 { 
			get { 
				Color color = Color2;
				return color.IsEmpty ? ConvertToTransparentColor(PaletteEntry.Color2, opacity) : color; 
			}
		}
		Color Color2 { 
			get {
				FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
				return options == null ? Color.Empty : options.Color2;
			}
			set {
				FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
				if (options != null)
					options.SetColor2(value);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(XYDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesViewBarWidth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesView.BarWidth"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public double BarWidth {
			get { return barWidth; }
			set {
				if (value != barWidth) {
					if(value <= 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectBarWidth));
					SendNotification(new ElementWillChangeNotification(this));
					barWidth = value;
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesViewBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesView.Border"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangularBorder Border { get { return border; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesViewFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesView.FillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BarSeriesViewTransparency"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BarSeriesView.Transparency"),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public byte Transparency {
			get { return ConvertBetweenOpacityAndTransparency(opacity); }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				opacity = ConvertBetweenOpacityAndTransparency(value);
				if (!Loading)
					SyncColorsAndTransparency(opacity);
				RaiseControlChanged();
			}
		}
		public BarSeriesView() : base() {
			opacity = DefaultOpacity;
			border = new InsideRectangularBorder(this, true, Color.Empty);
			fillStyle =  new RectangleFillStyle(this);
		}
		#region ISupportBorderVisibility implementation
		bool ISupportBorderVisibility.BorderVisible {
			get { return Appearance != null ? Appearance.ShowBorder : true; } }
		#endregion
		#region ITransparableView
		void ITransparableView.AssignTransparency(ITransparableView view) {
			opacity = ConvertBetweenOpacityAndTransparency(view.Transparency);
		}
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "BarWidth")
				return ShouldSerializeBarWidth();
			if(propertyName == "Transparency")
				return ShouldSerializeTransparency();
			if(propertyName == "Border")
				return ShouldSerializeBorder();
			if(propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBarWidth() {
			return this.barWidth != DefaultBarWidth;
		}
		void ResetBarWidth() {
			BarWidth = DefaultBarWidth;
		}
		bool ShouldSerializeTransparency() {
			return opacity != DefaultOpacity;
		}
		void ResetTransparency() {
			Transparency = ConvertBetweenOpacityAndTransparency(DefaultOpacity);
		}
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBarWidth() ||
				ShouldSerializeTransparency() ||
				ShouldSerializeBorder() ||
				ShouldSerializeFillStyle();
		}
		#endregion
		Color GetBorderColor(BarDrawOptions drawOptions) {
			if(drawOptions.Border.Color == Color.Empty && Appearance.BorderColor != Color.Empty)
				return Appearance.BorderColor;
			return CalculateBorderColor(drawOptions);
		}
		void RenderBar(IRenderer renderer, RectangleF rect, RectangleF gradientRect, BarDrawOptions barDrawOptions, int borderThickness, bool rotateGradient, SelectionState selectionState) {
			bool isSmallBar = rect.Width < 3 || rect.Height < 3;
			Color borderColor = GetBorderColor(barDrawOptions);
			if (isSmallBar && barDrawOptions.FillStyle.FillMode == FillMode.Solid) {
				Color color = HitTestColors.MixColors(borderColor, barDrawOptions.Color);
				if (barDrawOptions.Color.A != 255) {
					byte a = (byte)((barDrawOptions.Color.A + borderColor.A) / 2);
					color = Color.FromArgb(a, color);
				}
				renderer.FillRectangle(rect, color);
				RenderBarSelection(renderer, rect, selectionState);
			}
			else {
				RectangleFillStyle actualFillStyle = (RectangleFillStyle)barDrawOptions.FillStyle;
				bool shouldRotateGradient = actualFillStyle.FillMode == FillMode.Gradient && rotateGradient;
				RectangleGradientMode gradientMode = RectangleGradientFillOptions.DefaultGradientMode;
				if (shouldRotateGradient) {
					gradientMode = ((RectangleGradientFillOptions)actualFillStyle.Options).GradientMode;
					((RectangleGradientFillOptions)actualFillStyle.Options).RotateGradientMode();
				}
				actualFillStyle.Render(renderer, rect, gradientRect, barDrawOptions.Color, barDrawOptions.ActualColor2);
				if (shouldRotateGradient)
					((RectangleGradientFillOptions)actualFillStyle.Options).RestoreGradientOrientation(gradientMode);
				RenderBarSelection(renderer, rect, selectionState);
				if (isSmallBar)
					renderer.FillRectangle(rect, borderColor);
				else
					barDrawOptions.Border.Render(renderer, rect, borderThickness, borderColor);
			}
		}
		void RenderBarSelection(IRenderer renderer, RectangleF rect, SelectionState selectionState) {
			if (selectionState != SelectionState.Normal) {
				IHitTest hitTest = Series as IHitTest;
				if (hitTest != null)
					renderer.FillRectangle(rect, hitTest.State.HatchStyle, HitTestState.GetPointHatchColor(Chart.SeriesSelectionMode, selectionState));
			}
		}
		protected abstract void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData);
		protected virtual SeriesPointLayout CalculateSeriesPointLayoutInternal(XYDiagramMappingBase diagramMapping, RefinedPointData pointData, BarData barData) {
			RectangleF diagramRect, screenRect, gradientRect;
			barData.CalculateBarRects(diagramMapping, out diagramRect, out screenRect, out gradientRect);
			return new BarSeriesPointLayout(pointData, diagramRect, screenRect, gradientRect);
		}
		protected override void SyncColorsAndTransparency(byte opacity) {
			base.SyncColorsAndTransparency(opacity);
			if (!Color2.IsEmpty)
				Color2 = Color.FromArgb(opacity, Color2);
		}
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			return new Rectangle(bounds.Location, Shadow.DecreaseSize(bounds.Size, legendMarkerShadowSize));
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new BarDrawOptions(this);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed && fillStyle != null) {
				fillStyle.Dispose();
				fillStyle = null;
			}
			if (this.border != null)
				border.Dispose();
			base.Dispose(disposing);
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			BarDrawOptions barDrawOptions = (BarDrawOptions)seriesPointDrawOptions;
			barDrawOptions.Shadow.Render(renderer, bounds, legendMarkerShadowSize);
			RenderBar(renderer, bounds, bounds, barDrawOptions, BorderHelper.CalculateBorderThickness(barDrawOptions.Border, 1), false, selectionState);
		}
		protected override void RenderCrosshairMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions) {
			BarDrawOptions barDrawOptions = (BarDrawOptions)seriesPointDrawOptions;
			barDrawOptions.Shadow.Render(renderer, bounds, legendMarkerShadowSize);
			RenderBar(renderer, bounds, bounds, barDrawOptions, BorderHelper.CalculateBorderThickness(barDrawOptions.Border, 1), false, SelectionState.Normal);
		}
		protected internal virtual BarData CreateBarData(RefinedPoint pointInfo) {
			ISideBySidePoint barPoint = pointInfo;
			return new BarData(pointInfo.Argument, 0.0, barPoint.Value, BarWidth, 0.0, 0);
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			return CalculateSeriesPointLayoutInternal(diagramMapping, pointData, pointData.GetBarData(this));
		}
		protected internal override HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			if (seriesView is BarSeriesView) {
				BarData barData = ((BarSeriesView)seriesView).CreateBarData(refinedPoint);
				RectangleF diagramRect, screenRect, gradientRect;
				barData.CalculateBarRects(diagramMapping, out diagramRect, out screenRect, out gradientRect);
				return new HighlightedBarPointLayout(screenRect);
			}
			else
				return null;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			BarDrawOptions barDrawOptions = drawOptions as BarDrawOptions;
			BarSeriesPointLayout barPointLayout = pointLayout as BarSeriesPointLayout;
			if (drawOptions == null || barPointLayout == null)
				return;
			int borderThickness = barDrawOptions.Border.GetActualThicknessBasedOnAppearance(this);
			RenderBar(renderer, (RectangleF)barPointLayout.ScreenRect, (RectangleF)barPointLayout.GradientRect, barDrawOptions, borderThickness, ((XYDiagram)Series.Chart.Diagram).Rotated, pointLayout.PointData.SelectionState);
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			BarDrawOptions barDrawOptions = drawOptions as BarDrawOptions;
			BarSeriesPointLayout barPointLayout = pointLayout as BarSeriesPointLayout;
			if (barDrawOptions == null || barPointLayout == null)
				return null;
			Rectangle inflatedMappingBounds = GraphicUtils.InflateRect(mappingBounds, EdgeGeometry.MaxCrosswiseStep, EdgeGeometry.MaxCrosswiseStep);
			Rectangle intersection = Rectangle.Intersect(inflatedMappingBounds, (Rectangle)(ZPlaneRectangle)barPointLayout.ScreenRect);
			if (intersection.IsEmpty)
				return null;
			return barDrawOptions.Shadow.CreateGraphicsCommand((ZPlaneRectangle)intersection);
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			BarDrawOptions barDrawOptions = drawOptions as BarDrawOptions;
			BarSeriesPointLayout barPointLayout = pointLayout as BarSeriesPointLayout;
			if (barDrawOptions == null || barPointLayout == null)
				return;
			Rectangle inflatedMappingBounds = GraphicUtils.InflateRect(mappingBounds, EdgeGeometry.MaxCrosswiseStep, EdgeGeometry.MaxCrosswiseStep);
			Rectangle intersection = Rectangle.Intersect(inflatedMappingBounds, MathUtils.StrongRound(barPointLayout.ScreenRect));
			if (intersection.IsEmpty)
				return;
			barDrawOptions.Shadow.Render(renderer, intersection);
		}
		protected internal override void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
			HighlightedBarPointLayout barPointLayout = pointLayout as HighlightedBarPointLayout;
			Color color = HitTestColors.SelectHatch;
			if (barPointLayout != null) {
				int borderThickness = Border.ActualThickness;
				RectangleF rect = new RectangleF(barPointLayout.Rectangle.X + borderThickness, barPointLayout.Rectangle.Y + borderThickness,
					barPointLayout.Rectangle.Width - borderThickness * 2, barPointLayout.Rectangle.Height - borderThickness * 2);
				renderer.FillRectangle(rect, System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal, color);
			}
		}
		protected internal override void CalculateAnnotationsAnchorPointsLayout(XYDiagramAnchorPointLayoutList anchorPointLayoutList) {
			foreach (RefinedPointData pointData in anchorPointLayoutList.SeriesData) {
				SeriesPoint seriesPoint = pointData.SeriesPoint as SeriesPoint;
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					foreach (Annotation annotation in seriesPoint.Annotations)
						CalculateAnnotationAnchorPointLayout(annotation, anchorPointLayoutList, pointData);
				}
				foreach (RefinedPoint child in pointData.RefinedPoint.Children) {
					seriesPoint = child.SeriesPoint as SeriesPoint;
					if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
						foreach (Annotation annotation in seriesPoint.Annotations)
							CalculateAnnotationAnchorPointLayout(annotation, anchorPointLayoutList, pointData);
					}
				}
			}
		}
		protected internal override Color GetPointColor(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor(pointIndex, pointsCount));
		}
		protected internal override Color GetPointColor2(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor2(pointIndex, pointsCount));
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
									   CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxBarRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo.Pane, snapMode);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ITransparableView transparableView = obj as ITransparableView;
			if (transparableView == null)
				return;
			((ITransparableView)this).AssignTransparency(transparableView);
			IBarSeriesView view = obj as IBarSeriesView;
			if (view == null)
				return;
			barWidth = view.BarWidth;
			BarSeriesView barView = obj as BarSeriesView;
			if (barView == null)
				return;
			border.Assign(barView.border);
			fillStyle.Assign(barView.fillStyle);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			BarSeriesView view = (BarSeriesView)obj;
			return barWidth == view.barWidth && border.Equals(view.border) && 
				fillStyle.Equals(view.FillStyle) && opacity.Equals(view.opacity);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class BarData : IInterimValueCalculator {
		readonly double argument;
		readonly double zeroValue;
		readonly double actualValue;
		readonly double width;
		readonly double displayOffset;
		readonly int fixedOffset;
		readonly double leftArgument;
		readonly double rightArgument;		
		public double Argument { get { return argument; } }
		public double ZeroValue { get { return zeroValue; } }
		public double ActualValue { get { return actualValue; } }
		public BarData(double argument, double zeroValue, double actualValue, double width, double displayOffset, int fixedOffset) {
			this.argument = argument;
			this.zeroValue = zeroValue;
			this.actualValue = actualValue;
			this.width = width;
			this.displayOffset = displayOffset;
			this.fixedOffset = fixedOffset;
			leftArgument = argument - width / 2;
			rightArgument = argument + width / 2;
		}
		int IInterimValueCalculator.CalculateInterimValue(AxisIntervalLayout intervalLayout, double value, ITransformation transformation) {
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(intervalLayout, transformation);
			return (int)mapping.GetInterimCoord(value + displayOffset, false, true) + fixedOffset;
		}
		double GetInterimX(double argument, AxisIntervalLayout layoutX, ITransformation transformation) {
			return ((IInterimValueCalculator)this).CalculateInterimValue(layoutX, argument, transformation);
		}
		DiagramPoint GetInterimPoint(double argument, double value, XYDiagramMappingBase diagramMapping) {
			double interimX = GetInterimX(argument, diagramMapping.LayoutX, diagramMapping.AxisX.ScaleTypeMap.Transformation);
			AxisIntervalLayoutMapping mapping = new AxisIntervalLayoutMapping(diagramMapping.LayoutY, diagramMapping.AxisY.ScaleTypeMap.Transformation);
			double interimY = mapping.GetInterimCoord(value, false, true);
			return new DiagramPoint(interimX, interimY);
		}
		AxisIntervalLayout FindIntervalLayoutX(XYDiagramMappingContainer mappingContainer) {
			foreach(AxisIntervalLayout layoutX in mappingContainer.IntervalsLayoutX) {
				double interimX = GetInterimX(argument, layoutX, mappingContainer.AxisX.ScaleTypeMap.Transformation);
				if(interimX >= layoutX.Start && interimX <= layoutX.End)
					return layoutX;
			}
			return null;
		}
		double CalculateInterimYAnchorPoint(XYDiagramMappingContainer mappingContainer, Size textSize, int indent, int borderThickness, int interimMin, int interimMax, BarSeriesLabelPosition position) {
			int textOffset = mappingContainer.Rotated ? textSize.Width : textSize.Height;
			int doubleOffset = (indent + borderThickness) * 2 + textOffset;
			double offset = doubleOffset / 2.0;
			if ((position == BarSeriesLabelPosition.TopInside && zeroValue <= ActualValue) ||
				(position == BarSeriesLabelPosition.BottomInside && zeroValue > ActualValue)) {
				if (mappingContainer.Rotated && !mappingContainer.AxisY.ActualReverse ||
					!mappingContainer.Rotated && mappingContainer.AxisY.ActualReverse)
					offset--;
				return interimMax - offset;
			}
			else {
				if (mappingContainer.Rotated && mappingContainer.AxisY.ActualReverse ||
					!mappingContainer.Rotated && !mappingContainer.AxisY.ActualReverse)
					offset--;
				return interimMin + offset;
			}
		}
		public DiagramPoint? CalculateAnchorPointForCenterLabelPosition(XYDiagramMappingContainer mappingContainer) {
			return CalculateAnchorPointForInsideLabelPosition(mappingContainer, BarSeriesLabelPosition.Center, 0, Size.Empty, 0);
		}
		public DiagramPoint? CalculateAnchorPointForInsideLabelPosition(XYDiagramMappingContainer mappingContainer, BarSeriesLabelPosition position, int indent, Size textSize, int borderThickness) {
			AxisIntervalLayout layoutX = FindIntervalLayoutX(mappingContainer);
			if (layoutX == null)
				return null;
			double interimX = GetInterimX(argument, layoutX, mappingContainer.AxisX.ScaleTypeMap.Transformation);
			IntMinMaxValues interimValues = mappingContainer.CalculateMinMaxInterimY(null, true, zeroValue, actualValue);
			if (interimValues == null)
				return null;
			if (position == BarSeriesLabelPosition.TopInside || position == BarSeriesLabelPosition.BottomInside) {
				double interimY = CalculateInterimYAnchorPoint(mappingContainer, textSize, indent, borderThickness, interimValues.MinValue, interimValues.MaxValue, position);
				return XYDiagramMappingHelper.InterimPointToScreenPoint(new DiagramPoint(interimX, interimY), mappingContainer);
			}
			else
				ChartDebug.Assert(position == BarSeriesLabelPosition.Center, "Incorrect position");
			DiagramPoint interimAnchorPoint = new DiagramPoint(interimX, (interimValues.MinValue + interimValues.MaxValue) / 2.0);
			return XYDiagramMappingHelper.InterimPointToScreenPoint(interimAnchorPoint, mappingContainer);
		}
		public DiagramPoint? CalculateAnchorPointForToolTipTopPosition(XYDiagramMappingContainer mappingContainer) {
			AxisIntervalLayout layoutX = FindIntervalLayoutX(mappingContainer);
			if (layoutX == null)
				return null;
			double interimX = GetInterimX(argument, layoutX, mappingContainer.AxisX.ScaleTypeMap.Transformation);
			IntMinMaxValues interimValues = mappingContainer.CalculateMinMaxInterimY(null, true, zeroValue, actualValue);
			if (interimValues == null)
				return null;
			DiagramPoint interimAnchorPoint = new DiagramPoint(interimX, interimValues.MaxValue);
			return XYDiagramMappingHelper.InterimPointToScreenPoint(interimAnchorPoint, mappingContainer);
		}
		public DiagramPoint? CalculateAnchorPointForInsideBarPositionWithScrolling(XYDiagramMappingContainer mappingContainer, SeriesLabelBase seriesLabel, BarSeriesLabelPosition position, int indent, Size textSize) {
			if (mappingContainer.MappingForScrolling == null)
				return null;
			BarSeriesView view = seriesLabel.SeriesBase.View as BarSeriesView;
			if (view == null)
				return null;
			if (position == BarSeriesLabelPosition.TopInside || position == BarSeriesLabelPosition.BottomInside) {
				MinMaxValues barMinMax = new MinMaxValues(ZeroValue, ActualValue);
				barMinMax.Intersection(mappingContainer.MappingForScrolling.AxisY.WholeRangeData);				
				if (Double.IsNaN(barMinMax.Max))
					return null;
				double interimX = GetInterimX(Argument, mappingContainer.MappingForScrolling.LayoutX, mappingContainer.AxisX.ScaleTypeMap.Transformation);
				DiagramPoint maxInterimPoint = mappingContainer.MappingForScrolling.GetInterimPoint(Argument, barMinMax.Max, false, true);
				DiagramPoint minInterimPoint = mappingContainer.MappingForScrolling.GetInterimPoint(Argument, barMinMax.Min, false, true);
				double interimY = CalculateInterimYAnchorPoint(mappingContainer, textSize, indent, seriesLabel.Border.Thickness, (int)minInterimPoint.Y, (int)maxInterimPoint.Y, position);
				return XYDiagramMappingHelper.InterimPointToScreenPoint(new DiagramPoint(interimX, interimY), mappingContainer);
			}
			else
				ChartDebug.Assert(position == BarSeriesLabelPosition.Center, "Incorrect position");
			MinMaxValues values = new MinMaxValues(ZeroValue, ActualValue);
			double centerValue = MinMaxValues.Intersection(mappingContainer.MappingForScrolling.AxisY.WholeRangeData, values).CalculateCenter();
			return Double.IsNaN(centerValue) ? null : (DiagramPoint?)GetScreenPoint(Argument, centerValue, mappingContainer.MappingForScrolling);
		}
		public DiagramPoint GetScreenPoint(double argument, double value, XYDiagramMappingBase diagramMapping) {
			DiagramPoint interimPoint = GetInterimPoint(argument, value, diagramMapping);
			return XYDiagramMappingHelper.InterimPointToScreenPoint(interimPoint, diagramMapping.Container);
		}
		public void CalculateBarRects(XYDiagramMappingBase diagramMapping, out RectangleF diagramRect, out RectangleF screenRect, out RectangleF gradientRect) {
			DiagramPoint interimPoint1 = LargeScaleHelper.Validate(GetInterimPoint(leftArgument, actualValue, diagramMapping));
			DiagramPoint interimPoint2 = LargeScaleHelper.Validate(GetInterimPoint(rightArgument, zeroValue, diagramMapping));
			DiagramPoint screenPoint1 = XYDiagramMappingHelper.InterimPointToScreenPoint(interimPoint1, diagramMapping.Container);
			DiagramPoint screenPoint2 = XYDiagramMappingHelper.InterimPointToScreenPoint(interimPoint2, diagramMapping.Container);
			diagramRect = MathUtils.MakeRectangle((PointF)interimPoint1, (PointF)interimPoint2);
			screenRect = MathUtils.MakeRectangle((PointF)screenPoint1, (PointF)screenPoint2, 1);
			screenRect = RectangleF.Intersect(diagramMapping.InflatedBounds, screenRect);
			gradientRect = RectangleF.Union(GetTotalRect(diagramMapping.Container), screenRect);
		}
		public RectangleF GetTotalRect(XYDiagramMappingContainer mappingContainer) {
			IntMinMaxValues interimValuesX = mappingContainer.CalculateMinMaxInterimX(this, false, leftArgument, rightArgument);
			IntMinMaxValues interimValuesY = mappingContainer.CalculateMinMaxInterimY(null, false, zeroValue, actualValue);
			if (interimValuesX == null || interimValuesY == null)
				return RectangleF.Empty;
			DiagramPoint minInterimPoint = new DiagramPoint(interimValuesX.MinValue, interimValuesY.MinValue);
			DiagramPoint maxInterimPoint = new DiagramPoint(interimValuesX.MaxValue, interimValuesY.MaxValue);
			DiagramPoint minScreenPoint = LargeScaleHelper.Validate(XYDiagramMappingHelper.InterimPointToScreenPoint(minInterimPoint, mappingContainer));
			DiagramPoint maxScreenPoint = LargeScaleHelper.Validate(XYDiagramMappingHelper.InterimPointToScreenPoint(maxInterimPoint, mappingContainer));
			return MathUtils.MakeRectangle((PointF)minScreenPoint, (PointF)maxScreenPoint);			
		}
		public XYDiagramMappingBase GetMappingForExtremeLabelPosition(XYDiagramMappingContainer mappingContainer, double value) {
			AxisIntervalLayout layoutX = FindIntervalLayoutX(mappingContainer);
			if(layoutX == null)
				return null;
			AxisIntervalLayout layoutY = mappingContainer.GetIntervalLayoutY(value);
			if(layoutY == null)
				return null;
			return mappingContainer.CreateDiagramMapping(layoutX, layoutY);
		}
		public XYDiagramMappingBase GetMappingForTopLabelPosition(XYDiagramMappingContainer mappingContainer) {
			return GetMappingForExtremeLabelPosition(mappingContainer, actualValue);
		}
	}
	public class BarSeriesPointLayout : SeriesPointLayout {
		RectangleF diagramRect;
		RectangleF screenRect;
		RectangleF gradientRect;
		public RectangleF DiagramRect { get { return diagramRect; } }
		public RectangleF ScreenRect { get { return screenRect; } }
		public RectangleF GradientRect { get { return gradientRect; } }
		public BarSeriesPointLayout(RefinedPointData pointData, RectangleF diagramRect, RectangleF screenRect, RectangleF gradientRect) : base(pointData) {
			this.diagramRect = diagramRect;
			this.screenRect = screenRect;
			this.gradientRect = gradientRect;
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			hitRegion.Union(new HitRegion(MathUtils.StrongRound(this.screenRect)));
			return hitRegion;
		}
	}
}
