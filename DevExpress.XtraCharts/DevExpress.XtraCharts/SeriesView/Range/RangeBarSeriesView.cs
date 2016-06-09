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
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum TimeSpanFormat {
		Standard,
		TotalDays,
		TotalHours,
		TotalMinutes,
		TotalSeconds,
		TotalMilliseconds,
	}
	public abstract class RangeBarSeriesView : BarSeriesView {
		const int DefaultMarkerSize = 10;
		const int ValueCount = 2;
		const int LegendMarkerShadowSize = 1;
		const DefaultBoolean DefaultMinValueMarkerVisibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultMaxValueMarkerVisibility = DefaultBoolean.Default;
		Marker maxValueMarker;
		Marker minValueMarker;
		DefaultBoolean minValueMarkerVisibility = DefaultMinValueMarkerVisibility;
		DefaultBoolean maxValueMarkerVisibility = DefaultMaxValueMarkerVisibility;
		protected override int PixelsPerArgument { get { return 40; } }
		protected internal override int PointDimension { get { return ValueCount; } }
		protected internal override ValueLevel[] SupportedValueLevels {
			get { return new ValueLevel[] { ValueLevel.Value_1, ValueLevel.Value_2 }; }
		}
		protected internal virtual bool ActualMinValueMarkerVisible {
			get {
				if (MinValueMarkerVisibility == DefaultBoolean.Default)
					return false;
				else
					return MinValueMarkerVisibility == DefaultBoolean.True;
			}
		}
		protected internal virtual bool ActualMaxValueMarkerVisible {
			get {
				if (MaxValueMarkerVisibility == DefaultBoolean.Default)
					return false;
				else
					return MaxValueMarkerVisibility == DefaultBoolean.True;
			}
		}
		protected internal override string DefaultPointToolTipPattern {
			get {
				string argumentPattern = "{A" + GetDefaultArgumentFormat() + "}";
				string valuePattern = " : " + "{V1" + GetDefaultFormat(Series.ValueScaleType) + "} : {V2" + GetDefaultFormat(Series.ValueScaleType) + "}";
				return argumentPattern + valuePattern;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesViewMinValueMarker"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesView.MinValueMarker"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker MinValueMarker { get { return this.minValueMarker; } }		
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesViewMaxValueMarker"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesView.MaxValueMarker"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker MaxValueMarker { get { return this.maxValueMarker; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesViewMinValueMarkerVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesView.MinValueMarkerVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean MinValueMarkerVisibility {
			get { return minValueMarkerVisibility; }
			set {
				if (value != minValueMarkerVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					minValueMarkerVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangeBarSeriesViewMaxValueMarkerVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangeBarSeriesView.MaxValueMarkerVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean MaxValueMarkerVisibility {
			get { return maxValueMarkerVisibility; }
			set {
				if (value != maxValueMarkerVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					maxValueMarkerVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		public RangeBarSeriesView() : base() {
			this.minValueMarker = new Marker(this, DefaultMarkerSize);
			this.maxValueMarker = new Marker(this, DefaultMarkerSize);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "MinValueMarker":
					return ShouldSerializeMinValueMarker();
				case "MaxValueMarker":
					return ShouldSerializeMaxValueMarker();
				case "MinValueMarkerVisibility":
					return ShouldSerializeMinValueMarkerVisibility();
				case "MaxValueMarkerVisibility":
					return ShouldSerializeMaxValueMarkerVisibility();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeMinValueMarker() {
			return MinValueMarker.ShouldSerialize();
		}
		bool ShouldSerializeMaxValueMarker() {
			return MaxValueMarker.ShouldSerialize();
		}
		bool ShouldSerializeMinValueMarkerVisibility() {
			return minValueMarkerVisibility != DefaultMinValueMarkerVisibility;
		}
		void ResetMinValueMarkerVisibility() {
			MinValueMarkerVisibility = DefaultMinValueMarkerVisibility;
		}
		bool ShouldSerializeMaxValueMarkerVisibility() {
			return maxValueMarkerVisibility != DefaultMaxValueMarkerVisibility;
		}
		void ResetMaxValueMarkerVisibility() {
			MaxValueMarkerVisibility = DefaultMaxValueMarkerVisibility;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeMinValueMarker() ||
				ShouldSerializeMaxValueMarker() ||
				ShouldSerializeMinValueMarkerVisibility() ||
				ShouldSerializeMaxValueMarkerVisibility();
		}
		#endregion
		void RenderMarker(IRenderer renderer, Marker marker, Color color, Color color2, IPolygon polygon) {
			if (polygon == null)
				return;
			CustomBorder border = marker.Border;
			Color automaticBorderColor = HitTestColors.MixColors(Color.FromArgb(20, 0, 0, 0), color);
			Color borderColor = BorderHelper.CalculateBorderColor(border, automaticBorderColor);
			int borderThickness = BorderHelper.CalculateBorderThickness(border, 1);
			Color markerColor = marker.Color.IsEmpty ? color : marker.Color;
			marker.Render(renderer, polygon, markerColor, color2, borderColor, borderThickness);
		}
		void RenderMarkerShadow(IRenderer renderer, IPolygon markerPolygon, Shadow shadow) {
			if (markerPolygon == null)
				return;
			markerPolygon.RenderShadow(renderer, shadow, -1);
		}
		void CalculateMarkerPoints(XYDiagramMappingBase diagramMapping, BarData barData, out GRealPoint2D maxMarkerPoint, out GRealPoint2D minMarkerPoint) {
			DiagramPoint minPoint = barData.GetScreenPoint(barData.Argument, barData.ActualValue, diagramMapping);
			DiagramPoint maxPoint = barData.GetScreenPoint(barData.Argument, barData.ZeroValue, diagramMapping);
			maxMarkerPoint = new GRealPoint2D(minPoint.X, minPoint.Y);
			minMarkerPoint = new GRealPoint2D(maxPoint.X, maxPoint.Y);
		}
		void CalculateRageBarValues(double value1, double value2, out double zeroValue, out double actualValue) {
			if (value1 >= value2) {
				actualValue = value1;
				zeroValue = value2;
			}
			else {
				actualValue = value2;
				zeroValue = value1;
			}
		}
		protected BarData CreateRangeBarData(double argument, double value1, double value2, double width, double displayOffset, int fixedOffset) {
			double zeroValue, actualValue;
			CalculateRageBarValues(value1, value2, out zeroValue, out actualValue);
			return new BarData(argument, zeroValue, actualValue, width, displayOffset, fixedOffset);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new RangeBarDrawOptions(this);
		}
		protected override System.Collections.Generic.IEnumerable<double> GetCrosshairValues(RefinedPoint pointInfo) {
			IRangePoint rangePoint = pointInfo;
			yield return rangePoint.Min;
			yield return rangePoint.Max;
		}
		protected override void CalculateAnnotationAnchorPointLayout(Annotation annotation, XYDiagramAnchorPointLayoutList anchorPointLayoutList, RefinedPointData pointData) {
			AnnotationHelper.CalculateAchorPointLayoutForCenterBarPoint(annotation, this, anchorPointLayoutList, pointData);
		}
		protected override SeriesPointLayout CalculateSeriesPointLayoutInternal(XYDiagramMappingBase diagramMapping, RefinedPointData pointData, BarData barData) {
			RectangleF diagramRect, screenRect, gradientRect;
			barData.CalculateBarRects(diagramMapping, out diagramRect, out screenRect, out gradientRect);
			GRealPoint2D maxMarkerPoint, minMarkerPoint;
			CalculateMarkerPoints(diagramMapping, barData, out maxMarkerPoint, out minMarkerPoint);
			RangeBarDrawOptions drawOptions = (RangeBarDrawOptions)pointData.DrawOptions;
			IPolygon minMarkerPolygon = drawOptions.MinValueMarkerVisible ? drawOptions.MinValueMarker.CalculatePolygon(minMarkerPoint, false) : null;
			IPolygon maxMarkerPolygon = drawOptions.MaxValueMarkerVisible ? drawOptions.MaxValueMarker.CalculatePolygon(maxMarkerPoint, false) : null;
			return new RangeBarSeriesPointLayout(pointData, diagramRect, screenRect, gradientRect, minMarkerPolygon, maxMarkerPolygon);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value_1 || valueLevel == ValueLevelInternal.Value_2;
		}
		internal override string LabelPatternToLegendPattern() {
			if (SeriesBase != null && Label != null)
				return PatternUtils.ReplacePlaceholder(Label.ActualTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value2Placeholder, PatternUtils.Value1Placeholder);
			else
				return base.LabelPatternToLegendPattern();
		}
		protected internal override PointOptions CreatePointOptions() {
			return new RangeBarPointOptions();
		}
		protected internal override BarData CreateBarData(RefinedPoint pointInfo) {
			IRangePoint rangeBarPoint = (IRangePoint)pointInfo;
			return CreateRangeBarData(pointInfo.Argument, rangeBarPoint.Min, rangeBarPoint.Max, BarWidth, 0.0, 0);
		}
		protected internal override ToolTipPointDataToStringConverter CreateToolTipValueToStringConverter() {
			return new ToolTipRangeValueToStringConverter(Series);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.RangeViewPointPatterns;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			base.Render(renderer, mappingBounds, pointLayout, drawOptions);
			RangeBarDrawOptions rangeDrawOptions = (RangeBarDrawOptions)drawOptions;
			RangeBarSeriesPointLayout rangeLayout = (RangeBarSeriesPointLayout)pointLayout;
			RenderMarker(renderer, rangeDrawOptions.MinValueMarker, rangeDrawOptions.Color, rangeDrawOptions.ActualColor2, rangeLayout.MinMarkerPolygon);
			RenderMarker(renderer, rangeDrawOptions.MaxValueMarker, rangeDrawOptions.Color, rangeDrawOptions.ActualColor2, rangeLayout.MaxMarkerPolygon);
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			base.RenderShadow(renderer, mappingBounds, pointLayout, drawOptions);
			RangeBarDrawOptions rangeDrawOptions = (RangeBarDrawOptions)drawOptions;
			RangeBarSeriesPointLayout rangeLayout = (RangeBarSeriesPointLayout)pointLayout;
			RenderMarkerShadow(renderer, rangeLayout.MinMarkerPolygon, rangeDrawOptions.Shadow);
			RenderMarkerShadow(renderer, rangeLayout.MaxMarkerPolygon, rangeDrawOptions.Shadow);
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			BarData barData = pointData.GetBarData(this);
			return barData.CalculateAnchorPointForCenterLabelPosition(mapping.Container);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new RangeBarSeriesLabel();
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
				return CrosshairManager.CalculateMinMaxBarRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo.Pane, snapMode);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			RangeBarSeriesView view = (RangeBarSeriesView)obj;
			return
				this.maxValueMarker.Equals(view.maxValueMarker) &&
				this.minValueMarker.Equals(view.minValueMarker);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangeBarSeriesView view = obj as RangeBarSeriesView;
			if(view == null)
				return;
			this.maxValueMarker.Assign(view.maxValueMarker);
			this.minValueMarker.Assign(view.minValueMarker);
			this.minValueMarkerVisibility = view.minValueMarkerVisibility;
			this.maxValueMarkerVisibility = view.maxValueMarkerVisibility;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RangeBarSeriesPointLayout : BarSeriesPointLayout {
		IPolygon minMarkerPolygon;
		IPolygon maxMarkerPolygon;
		public IPolygon MaxMarkerPolygon { get { return this.maxMarkerPolygon; } }
		public IPolygon MinMarkerPolygon { get { return this.minMarkerPolygon; } }
		public RangeBarSeriesPointLayout(RefinedPointData pointData, RectangleF diagramRect, RectangleF screenRect, RectangleF gradientRect, IPolygon minMarkerPolygon, IPolygon maxMarkerPolygon) : base(pointData, diagramRect, screenRect, gradientRect) {
			this.minMarkerPolygon = minMarkerPolygon;
			this.maxMarkerPolygon = maxMarkerPolygon;
		}
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			IHitRegion minMarkerRegion = GraphicUtils.MakeHitRegion(this.minMarkerPolygon);
			hitRegion.Union(minMarkerRegion);
			IHitRegion maxMarkerRegion = GraphicUtils.MakeHitRegion(this.maxMarkerPolygon);
			hitRegion.Union(maxMarkerRegion);
			return hitRegion;
		}
	}
}
