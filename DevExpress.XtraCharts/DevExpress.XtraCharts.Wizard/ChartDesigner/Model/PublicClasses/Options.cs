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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(RangeControlOptions)), TypeConverter(typeof(RangeControlOptionsTypeConverter))]
	public class RangeControlOptionsModel : DesignerChartElementModelBase {
		readonly RangeControlOptions rangeControlOptions;
		protected RangeControlOptions RangeControlOptions { get { return rangeControlOptions; } }
		protected internal override ChartElement ChartElement { get { return rangeControlOptions; } }
		[PropertyForOptions]
		public bool Visible {
			get { return rangeControlOptions.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions]
		public RangeControlViewType ViewType {
			get { return rangeControlOptions.ViewType; }
			set { SetProperty("ViewType", value); }
		}
		[PropertyForOptions]
		public ValueLevel ValueLevel {
			get { return rangeControlOptions.ValueLevel; }
			set { SetProperty("ValueLevel", value); }
		}
		[PropertyForOptions]
		public byte? SeriesTransparency {
			get { return rangeControlOptions.SeriesTransparency; }
			set { SetProperty("SeriesTransparency", value); }
		}
		public RangeControlOptionsModel(RangeControlOptions rangeControlOptions, CommandManager commandManager)
			: base(commandManager) {
			this.rangeControlOptions = rangeControlOptions;
		}
	}
	[
	ModelOf(typeof(FillOptionsBase)),
	GenerateHeritableProperties]
	public abstract class FillOptionsBaseModel : DesignerChartElementModelBase {
		readonly FillOptionsBase fillOptionsBase;
		protected FillOptionsBase FillOptionsBase { get { return fillOptionsBase; } }
		protected internal abstract FillMode FillMode { get; }
		protected internal abstract FillMode3D FillMode3D { get; }
		protected internal override ChartElement ChartElement { get { return fillOptionsBase; } }
		public FillOptionsBaseModel(FillOptionsBase fillOptionsBase, CommandManager commandManager)
			: base(commandManager) {
			this.fillOptionsBase = fillOptionsBase;
		}
	}
	[ModelOf(typeof(SolidFillOptions))]
	public class SolidFillOptionsModel : FillOptionsBaseModel {
		protected SolidFillOptions SolidFillOptions { get { return (SolidFillOptions)FillOptionsBase; } }
		protected internal override FillMode FillMode { get { return FillMode.Solid; } }
		protected internal override FillMode3D FillMode3D { get { return FillMode3D.Solid; } }
		public SolidFillOptionsModel(SolidFillOptions solidFillOptions, CommandManager commandManager)
			: base(solidFillOptions, commandManager) {
		}
	}
	[ModelOf(typeof(FillOptionsColor2Base))]
	public abstract class FillOptionsColor2BaseModel : FillOptionsBaseModel {
		protected FillOptionsColor2Base FillOptionsColor2Base { get { return (FillOptionsColor2Base)FillOptionsBase; } }
		[PropertyForOptions]
		public Color Color2 {
			get { return FillOptionsColor2Base.Color2; }
			set { SetProperty("Color2", value); }
		}
		public FillOptionsColor2BaseModel(FillOptionsColor2Base fillOptionsColor2Base, CommandManager commandManager)
			: base(fillOptionsColor2Base, commandManager) {
		}
	}
	[ModelOf(typeof(HatchFillOptions))]
	public class HatchFillOptionsModel : FillOptionsColor2BaseModel {
		protected HatchFillOptions HatchFillOptions { get { return (HatchFillOptions)FillOptionsBase; } }
		protected internal override FillMode FillMode { get { return FillMode.Hatch; } }
		protected internal override FillMode3D FillMode3D { get { return FillMode3D.Empty; } }
		[
		PropertyForOptions, TypeConverter(typeof(HatchStyleTypeConverter)),
		Editor(typeof(DevExpress.XtraCharts.Design.HatchStyleTypeEditor), typeof(UITypeEditor))
		]
		public HatchStyle HatchStyle {
			get { return HatchFillOptions.HatchStyle; }
			set { SetProperty("HatchStyle", value); }
		}
		public HatchFillOptionsModel(HatchFillOptions hatchFillOptions, CommandManager commandManager)
			: base(hatchFillOptions, commandManager) {
		}
	}
	[ModelOf(typeof(GradientFillOptionsBase))]
	public abstract class GradientFillOptionsBaseModel : FillOptionsColor2BaseModel {
		protected GradientFillOptionsBase GradientFillOptionsBase { get { return (GradientFillOptionsBase)FillOptionsBase; } }
		protected internal override FillMode FillMode { get { return FillMode.Gradient; } }
		protected internal override FillMode3D FillMode3D { get { return FillMode3D.Gradient; } }
		public GradientFillOptionsBaseModel(GradientFillOptionsBase gradientFillOptionsBase, CommandManager commandManager)
			: base(gradientFillOptionsBase, commandManager) {
		}
	}
	[ModelOf(typeof(RectangleGradientFillOptions))]
	public class RectangleGradientFillOptionsModel : GradientFillOptionsBaseModel {
		protected RectangleGradientFillOptions RectangleGradientFillOptions { get { return (RectangleGradientFillOptions)FillOptionsBase; } }
		[
		PropertyForOptions,
		Editor(typeof(RectangleGradientModeModelEditor), typeof(UITypeEditor))
		]
		public RectangleGradientMode GradientMode {
			get { return RectangleGradientFillOptions.GradientMode; }
			set { SetProperty("GradientMode", value); }
		}
		public RectangleGradientFillOptionsModel(RectangleGradientFillOptions rectangleGradientFillOptions, CommandManager commandManager)
			: base(rectangleGradientFillOptions, commandManager) {
		}
	}
	[ModelOf(typeof(PolygonGradientFillOptions))]
	public class PolygonGradientFillOptionsModel : GradientFillOptionsBaseModel {
		protected PolygonGradientFillOptions PolygonGradientFillOptions { get { return (PolygonGradientFillOptions)FillOptionsBase; } }
		[
		PropertyForOptions,
		Editor(typeof(PolygonGradientModeModelEditor), typeof(UITypeEditor))
		]
		public PolygonGradientMode GradientMode {
			get { return PolygonGradientFillOptions.GradientMode; }
			set { SetProperty("GradientMode", value); }
		}
		public PolygonGradientFillOptionsModel(PolygonGradientFillOptions polygonGradientFillOptions, CommandManager commandManager)
			: base(polygonGradientFillOptions, commandManager) {
		}
	}
	[ModelOf(typeof(ReductionStockOptions))]
	public class ReductionStockOptionsModel : DesignerChartElementModelBase {
		readonly ReductionStockOptions reductionStockOptions;
		protected ReductionStockOptions ReductionStockOptions { get { return reductionStockOptions; } }
		protected internal override ChartElement ChartElement { get { return reductionStockOptions; } }
		[PropertyForOptions]
		public Color Color {
			get { return ReductionStockOptions.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions]
		public StockLevel Level {
			get { return ReductionStockOptions.Level; }
			set { SetProperty("Level", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return ReductionStockOptions.Visible; }
			set { SetProperty("Visible", value); }
		}
		public ReductionStockOptionsModel(ReductionStockOptions reductionStockOptions, CommandManager commandManager)
			: base(commandManager) {
			this.reductionStockOptions = reductionStockOptions;
		}
	}
	[ModelOf(typeof(ScrollingOptions))]
	public class ScrollingOptionsModel : DesignerChartElementModelBase {
		readonly ScrollingOptions scrollingOptions;
		protected ScrollingOptions ScrollingOptions { get { return scrollingOptions; } }
		protected internal override ChartElement ChartElement { get { return scrollingOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseKeyboard {
			get { return ScrollingOptions.UseKeyboard; }
			set { SetProperty("UseKeyboard", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseMouse {
			get { return ScrollingOptions.UseMouse; }
			set { SetProperty("UseMouse", value); }
		}
		public ScrollingOptionsModel(ScrollingOptions scrollingOptions, CommandManager commandManager)
			: base(commandManager) {
			this.scrollingOptions = scrollingOptions;
		}
	}
	[ModelOf(typeof(RotationOptions))]
	public class RotationOptionsModel : DesignerChartElementModelBase {
		readonly RotationOptions rotationOptions;
		protected RotationOptions RotationOptions { get { return rotationOptions; } }
		protected internal override ChartElement ChartElement { get { return rotationOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseMouse {
			get { return RotationOptions.UseMouse; }
			set { SetProperty("UseMouse", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseTouchDevice {
			get { return RotationOptions.UseTouchDevice; }
			set { SetProperty("UseTouchDevice", value); }
		}
		public RotationOptionsModel(RotationOptions rotationOptions, CommandManager commandManager)
			: base(commandManager) {
			this.rotationOptions = rotationOptions;
		}
	}
	[ModelOf(typeof(ZoomingOptions))]
	public class ZoomingOptionsModel : DesignerChartElementModelBase {
		readonly ZoomingOptions zoomingOptions;
		protected ZoomingOptions ZoomingOptions { get { return zoomingOptions; } }
		protected internal override ChartElement ChartElement { get { return zoomingOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseKeyboard {
			get { return ZoomingOptions.UseKeyboard; }
			set { SetProperty("UseKeyboard", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseKeyboardWithMouse {
			get { return ZoomingOptions.UseKeyboardWithMouse; }
			set { SetProperty("UseKeyboardWithMouse", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseMouseWheel {
			get { return ZoomingOptions.UseMouseWheel; }
			set { SetProperty("UseMouseWheel", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseTouchDevice {
			get { return ZoomingOptions.UseTouchDevice; }
			set { SetProperty("UseTouchDevice", value); }
		}
		public ZoomingOptionsModel(ZoomingOptions zoomingOptions, CommandManager commandManager)
			: base(commandManager) {
			this.zoomingOptions = zoomingOptions;
		}
	}
	[ModelOf(typeof(ScrollingOptions2D))]
	public class ScrollingOptions2DModel : ScrollingOptionsModel {
		protected new ScrollingOptions2D ScrollingOptions { get { return (ScrollingOptions2D)base.ScrollingOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseScrollBars {
			get { return ScrollingOptions.UseScrollBars; }
			set { SetProperty("UseScrollBars", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool UseTouchDevice {
			get { return ScrollingOptions.UseTouchDevice; }
			set { SetProperty("UseTouchDevice", value); }
		}
		public ScrollingOptions2DModel(ScrollingOptions2D scrollingOptions, CommandManager commandManager)
			: base(scrollingOptions, commandManager) {
		}
	}
	[ModelOf(typeof(ZoomingOptions2D))]
	public class ZoomingOptions2DModel : ZoomingOptionsModel {
		protected new ZoomingOptions2D ZoomingOptions { get { return (ZoomingOptions2D)base.ZoomingOptions; } }
		[PropertyForOptions]
		public double AxisXMaxZoomPercent {
			get { return ZoomingOptions.AxisXMaxZoomPercent; }
			set { SetProperty("AxisXMaxZoomPercent", value); }
		}
		[PropertyForOptions]
		public double AxisYMaxZoomPercent {
			get { return ZoomingOptions.AxisYMaxZoomPercent; }
			set { SetProperty("AxisYMaxZoomPercent", value); }
		}
		public ZoomingOptions2DModel(ZoomingOptions2D scrollingOptions, CommandManager commandManager)
			: base(scrollingOptions, commandManager) {
		}
	}
	[TypeConverter(typeof(ChartRangeControlClientGridOptionsTypeConverter))]
	public abstract class ChartRangeControlClientGridOptionsModel : DesignerChartElementModelBase {
		readonly ChartRangeControlClientGridOptions rangeControlClientGridOptions;
		protected ChartRangeControlClientGridOptions RangeControlClientGridOptions { get { return rangeControlClientGridOptions; } }
		protected internal override ChartElement ChartElement { get { return rangeControlClientGridOptions; } }
		[PropertyForOptions, TypeConverter(typeof(EnumTypeConverter))]
		public ChartRangeControlClientGridMode GridMode {
			get { return RangeControlClientGridOptions.GridMode; }
			set { SetProperty("GridMode", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(EnumTypeConverter))]
		public ChartRangeControlClientSnapMode SnapMode {
			get { return RangeControlClientGridOptions.SnapMode; }
			set { SetProperty("SnapMode", value); }
		}
		[PropertyForOptions]
		public double GridSpacing {
			get { return RangeControlClientGridOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[PropertyForOptions]
		public double SnapSpacing {
			get { return RangeControlClientGridOptions.SnapSpacing; }
			set { SetProperty("SnapSpacing", value); }
		}
		[PropertyForOptions]
		public double GridOffset {
			get { return RangeControlClientGridOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		[PropertyForOptions]
		public double SnapOffset {
			get { return RangeControlClientGridOptions.SnapOffset; }
			set { SetProperty("SnapOffset", value); }
		}
		[PropertyForOptions]
		public string LabelFormat {
			get { return RangeControlClientGridOptions.LabelFormat; }
			set { SetProperty("LabelFormat", value); }
		}
		[PropertyForOptions]
		public IFormatProvider LabelFormatProvider {
			get { return RangeControlClientGridOptions.LabelFormatProvider; }
			set { SetProperty("LabelFormatProvider", value); }
		}
		public ChartRangeControlClientGridOptionsModel(ChartRangeControlClientGridOptions rangeControlClientGridOptions, CommandManager commandManager)
			: base(commandManager) {
			this.rangeControlClientGridOptions = rangeControlClientGridOptions;
		}
	} 
	[ModelOf(typeof(ChartRangeControlClientDateTimeGridOptions))]
	public class ChartRangeControlClientDateTimeGridOptionsModel : ChartRangeControlClientGridOptionsModel {
		protected new ChartRangeControlClientDateTimeGridOptions RangeControlClientGridOptions { get { return (ChartRangeControlClientDateTimeGridOptions)base.RangeControlClientGridOptions; } }
		[PropertyForOptions]
		public DateTimeGridAlignment GridAlignment {
			get { return RangeControlClientGridOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		[PropertyForOptions]
		public DateTimeGridAlignment SnapAlignment {
			get { return RangeControlClientGridOptions.SnapAlignment; }
			set { SetProperty("SnapAlignment", value); }
		}
		public ChartRangeControlClientDateTimeGridOptionsModel(ChartRangeControlClientDateTimeGridOptions rangeControlClientGridOptions, CommandManager commandManager)
			: base(rangeControlClientGridOptions, commandManager) {
		}
	}
	[ModelOf(typeof(ChartRangeControlClientNumericGridOptions))]
	public class ChartRangeControlClientNumericGridOptionsModel : ChartRangeControlClientGridOptionsModel {
		protected new ChartRangeControlClientNumericGridOptions RangeControlClientGridOptions { get { return (ChartRangeControlClientNumericGridOptions)base.RangeControlClientGridOptions; } }
		public ChartRangeControlClientNumericGridOptionsModel(ChartRangeControlClientNumericGridOptions rangeControlClientGridOptions, CommandManager commandManager)
			: base(rangeControlClientGridOptions, commandManager) {
		}
	}
	[ModelOf(typeof(ScrollBarOptions))]
	public class ScrollBarOptionsModel : DesignerChartElementModelBase {
		readonly ScrollBarOptions scrollBarOptions;
		protected ScrollBarOptions ScrollBarOptions { get { return scrollBarOptions; } }
		protected internal override ChartElement ChartElement { get { return scrollBarOptions; } }
		[
		PropertyForOptions,
		DependentUpon("EnableAxisXScrolling", -1),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool XAxisScrollBarVisible {
			get { return ScrollBarOptions.XAxisScrollBarVisible; }
			set { SetProperty("XAxisScrollBarVisible", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("EnableAxisYScrolling", -1),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool YAxisScrollBarVisible {
			get { return ScrollBarOptions.YAxisScrollBarVisible; }
			set { SetProperty("YAxisScrollBarVisible", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("XAxisScrollBarVisible")
		]
		public ScrollBarAlignment XAxisScrollBarAlignment {
			get { return ScrollBarOptions.XAxisScrollBarAlignment; }
			set { SetProperty("XAxisScrollBarAlignment", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("YAxisScrollBarVisible")
		]
		public ScrollBarAlignment YAxisScrollBarAlignment {
			get { return ScrollBarOptions.YAxisScrollBarAlignment; }
			set { SetProperty("YAxisScrollBarAlignment", value); }
		}
		public Color BackColor {
			get { return ScrollBarOptions.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		public Color BarColor {
			get { return ScrollBarOptions.BarColor; }
			set { SetProperty("BarColor", value); }
		}
		public Color BorderColor {
			get { return ScrollBarOptions.BorderColor; }
			set { SetProperty("BorderColor", value); }
		}
		public int BarThickness {
			get { return ScrollBarOptions.BarThickness; }
			set { SetProperty("BarThickness", value); }
		}
		public ScrollBarOptionsModel(ScrollBarOptions scrollBarOptions, CommandManager commandManager)
			: base(commandManager) {
			this.scrollBarOptions = scrollBarOptions;
		}
	}
	public abstract class ScaleOptionsBaseModel : DesignerChartElementModelBase {
		readonly ScaleOptionsBase scaleOptions;
		protected ScaleOptionsBase ScaleOptions { get { return scaleOptions; } }
		protected internal override ChartElement ChartElement { get { return scaleOptions; } }
		[PropertyForOptions]
		public ScaleMode ScaleMode {
			get { return ScaleOptions.ScaleMode; }
			set { SetProperty("ScaleMode", value); }
		}
		[PropertyForOptions]
		public AggregateFunction AggregateFunction {
			get { return ScaleOptions.AggregateFunction; }
			set { SetProperty("AggregateFunction", value); }
		}
		[PropertyForOptions]
		public ProcessMissingPointsMode ProcessMissingPoints {
			get { return ScaleOptions.ProcessMissingPoints; }
			set { SetProperty("ProcessMissingPoints", value); }
		}
		[PropertyForOptions("Behavior")]
		public double GridSpacing {
			get { return ScaleOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[PropertyForOptions("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool AutoGrid {
			get { return ScaleOptions.AutoGrid; }
			set { SetProperty("AutoGrid", value); }
		}
		[PropertyForOptions("Behavior")]
		public double GridOffset {
			get { return ScaleOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		public ScaleOptionsBaseModel(ScaleOptionsBase scaleOptions, CommandManager commandManager)
			: base(commandManager) {
			this.scaleOptions = scaleOptions;
		}
	}
	[ModelOf(typeof(NumericScaleOptions)), TypeConverter(typeof(NumericScaleOptionsTypeConverter))]
	public class NumericScaleOptionsModel : ScaleOptionsBaseModel {
		protected new NumericScaleOptions ScaleOptions { get { return (NumericScaleOptions)base.ScaleOptions; } }
		[PropertyForOptions]
		public NumericMeasureUnit MeasureUnit {
			get { return ScaleOptions.MeasureUnit; }
			set { SetProperty("MeasureUnit", value); }
		}
		[PropertyForOptions]
		public NumericGridAlignment GridAlignment {
			get { return ScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		[PropertyForOptions]
		public double CustomGridAlignment {
			get { return ScaleOptions.CustomGridAlignment; }
			set { SetProperty("CustomGridAlignment", value); }
		}
		[PropertyForOptions]
		public double CustomMeasureUnit {
			get { return ScaleOptions.CustomMeasureUnit; }
			set { SetProperty("CustomMeasureUnit", value); }
		}
		public NumericScaleOptionsModel(ScaleOptionsBase scaleOptions, CommandManager commandManager)
			: base(scaleOptions, commandManager) {
		}
	}
	[ModelOf(typeof(DateTimeScaleOptions)), TypeConverter(typeof(DateTimeScaleOptionsTypeConverter))]
	public class DateTimeScaleOptionsModel : ScaleOptionsBaseModel {
		WorkdaysOptionsModel workdaysOptionsModel;
		protected new DateTimeScaleOptions ScaleOptions { get { return (DateTimeScaleOptions)base.ScaleOptions; } }
		[PropertyForOptions]
		public DateTimeMeasureUnit MeasureUnit {
			get { return ScaleOptions.MeasureUnit; }
			set { SetProperty("MeasureUnit", value); }
		}
		[PropertyForOptions]
		public DateTimeGridAlignment GridAlignment {
			get { return ScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool WorkdaysOnly {
			get { return ScaleOptions.WorkdaysOnly; }
			set { SetProperty("WorkdaysOnly", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions("Behavior")]
		public WorkdaysOptionsModel WorkdaysOptions { get { return workdaysOptionsModel; } }
		public DateTimeScaleOptionsModel(DateTimeScaleOptions scaleOptions, CommandManager commandManager)
			: base(scaleOptions, commandManager) {
			Update();
		}
		protected override void AddChildren() {
			if(workdaysOptionsModel != null)
				Children.Add(workdaysOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.workdaysOptionsModel = new WorkdaysOptionsModel(ScaleOptions.WorkdaysOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(CrosshairAxisLabelOptions)), TypeConverter(typeof(ExpandableObjectConverter))]
	public class CrosshairAxisLabelOptionsModel : DesignerChartElementModelBase {
		readonly CrosshairAxisLabelOptions crosshairAxisLabelOptions;
		protected CrosshairAxisLabelOptions CrosshairAxisLabelOptions { get { return crosshairAxisLabelOptions; } }
		protected internal override ChartElement ChartElement { get { return crosshairAxisLabelOptions; } }
		[PropertyForOptions("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Visibility {
			get { return CrosshairAxisLabelOptions.Visibility; }
			set { SetProperty("Visibility", value); }
		}
		[
		PropertyForOptions("Behavior"),
		Editor(typeof(CrosshairAxisLabelModelPatternEditor), typeof(UITypeEditor))
		]
		public string Pattern {
			get { return CrosshairAxisLabelOptions.Pattern; }
			set { SetProperty("Pattern", value); }
		}
		[PropertyForOptions]
		public Color BackColor {
			get { return CrosshairAxisLabelOptions.BackColor; }
			set { SetProperty("BackColor", value); }
		}
		[PropertyForOptions]
		public Color TextColor {
			get { return CrosshairAxisLabelOptions.TextColor; }
			set { SetProperty("TextColor", value); }
		}
		[PropertyForOptions("Behavior"), TypeConverter(typeof(FontTypeConverter))]
		public Font Font {
			get { return CrosshairAxisLabelOptions.Font; }
			set { SetProperty("Font", value); }
		}
		public CrosshairAxisLabelOptionsModel(CrosshairAxisLabelOptions crosshairAxisLabelOptions, CommandManager commandManager)
			: base(commandManager) {
			this.crosshairAxisLabelOptions = crosshairAxisLabelOptions;
		}
	}
	[ModelOf(typeof(ScaleBreakOptions)), TypeConverter(typeof(ScaleBreakOptionsTypeConverter))]
	public class ScaleBreakOptionsModel : DesignerChartElementModelBase {
		readonly ScaleBreakOptions scaleBreakOptions;
		protected ScaleBreakOptions ScaleBreakOptions { get { return scaleBreakOptions; } }
		protected internal override ChartElement ChartElement { get { return scaleBreakOptions; } }
		[PropertyForOptions]
		public int SizeInPixels {
			get { return ScaleBreakOptions.SizeInPixels; }
			set { SetProperty("SizeInPixels", value); }
		}
		[PropertyForOptions]
		public ScaleBreakStyle Style {
			get { return ScaleBreakOptions.Style; }
			set { SetProperty("Style", value); }
		}
		[PropertyForOptions]
		public Color Color {
			get { return ScaleBreakOptions.Color; }
			set { SetProperty("Color", value); }
		}
		public ScaleBreakOptionsModel(ScaleBreakOptions scaleBreakOptions, CommandManager commandManager)
			: base(commandManager) {
			this.scaleBreakOptions = scaleBreakOptions;
		}
	}
	[ModelOf(typeof(AxisLabelResolveOverlappingOptions)), TypeConverter(typeof(AxisLabelResolveOverlappingOptionsTypeConverter))]
	public class AxisLabelResolveOverlappingOptionsModel : DesignerChartElementModelBase {
		readonly AxisLabelResolveOverlappingOptions resolveOverlappingOptions;
		protected AxisLabelResolveOverlappingOptions ResolveOverlappingOptions { get { return resolveOverlappingOptions; } }
		protected internal override ChartElement ChartElement { get { return resolveOverlappingOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool AllowStagger {
			get { return ResolveOverlappingOptions.AllowStagger; }
			set { SetProperty("AllowStagger", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool AllowRotate {
			get { return ResolveOverlappingOptions.AllowRotate; }
			set { SetProperty("AllowRotate", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool AllowHide {
			get { return ResolveOverlappingOptions.AllowHide; }
			set { SetProperty("AllowHide", value); }
		}
		[PropertyForOptions]
		public int MinIndent {
			get { return ResolveOverlappingOptions.MinIndent; }
			set { SetProperty("MinIndent", value); }
		}
		public AxisLabelResolveOverlappingOptionsModel(AxisLabelResolveOverlappingOptions resolveOverlappingOptions, CommandManager commandManager)
			: base(commandManager) {
			this.resolveOverlappingOptions = resolveOverlappingOptions;
		}
	}
	[ModelOf(typeof(WorkdaysOptions)), TypeConverter(typeof(ExpandableObjectConverter))]
	public class WorkdaysOptionsModel : DesignerChartElementModelBase {
		readonly WorkdaysOptions workdaysOptions;
		KnownDateCollectionModel holidaysModel;
		KnownDateCollectionModel exactWorkdaysModel;
		protected WorkdaysOptions WorkdaysOptions { get { return workdaysOptions; } }
		protected internal override ChartElement ChartElement { get { return workdaysOptions; } }
		[
		PropertyForOptions,
		TypeConverter(typeof(CollectionTypeConverter)),
		Editor(typeof(HolidayModelCollectionEditor), typeof(UITypeEditor))
		]
		public KnownDateCollectionModel Holidays { get { return holidaysModel; } }
		[
		PropertyForOptions,
		TypeConverter(typeof(CollectionTypeConverter)),
		Editor(typeof(HolidayModelCollectionEditor), typeof(UITypeEditor))
		]
		public KnownDateCollectionModel ExactWorkdays { get { return exactWorkdaysModel; } }
		[PropertyForOptions, Editor(typeof(WorkdaysEditor), typeof(UITypeEditor))]
		public Weekday Workdays {
			get { return WorkdaysOptions.Workdays; }
			set { SetProperty("Workdays", value); }
		}
		public WorkdaysOptionsModel(WorkdaysOptions workdaysOptions, CommandManager commandManager)
			: base(commandManager) {
			this.workdaysOptions = workdaysOptions;
			Update();
		}
		protected override void AddChildren() {
			if(holidaysModel != null)
				Children.Add(holidaysModel);
			if(exactWorkdaysModel != null)
				Children.Add(exactWorkdaysModel);
			base.AddChildren();
		}
		public override void Update() {
			this.holidaysModel = new KnownDateCollectionModel(WorkdaysOptions.Holidays, CommandManager, null);
			this.exactWorkdaysModel = new KnownDateCollectionModel(WorkdaysOptions.ExactWorkdays, CommandManager, null);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(TopNOptions)), TypeConverter(typeof(TopNOptionsTypeConverter))]
	public class TopNOptionsModel : DesignerChartElementModelBase {
		readonly TopNOptions topNOptions;
		protected TopNOptions TopNOptions { get { return topNOptions; } }
		protected internal override ChartElement ChartElement { get { return topNOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Enabled {
			get { return TopNOptions.Enabled; }
			set { SetProperty("Enabled", value); }
		}
		[PropertyForOptions]
		public TopNMode Mode {
			get { return TopNOptions.Mode; }
			set { SetProperty("Mode", value); }
		}
		[PropertyForOptions]
		public int Count {
			get { return TopNOptions.Count; }
			set { SetProperty("Count", value); }
		}
		[PropertyForOptions]
		public double ThresholdValue {
			get { return TopNOptions.ThresholdValue; }
			set { SetProperty("ThresholdValue", value); }
		}
		[PropertyForOptions]
		public double ThresholdPercent {
			get { return TopNOptions.ThresholdPercent; }
			set { SetProperty("ThresholdPercent", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowOthers {
			get { return TopNOptions.ShowOthers; }
			set { SetProperty("ShowOthers", value); }
		}
		[PropertyForOptions]
		public string OthersArgument {
			get { return TopNOptions.OthersArgument; }
			set { SetProperty("OthersArgument", value); }
		}
		public TopNOptionsModel(TopNOptions topNOptions, CommandManager commandManager)
			: base(commandManager) {
			this.topNOptions = topNOptions;
		}
	}
	[ModelOf(typeof(ToolTipOptions))]
	public class ToolTipOptionsModel : DesignerChartElementModelBase {
		readonly ToolTipOptions toolTipOptions;
		ToolTipPositionModel toolTipPositionModel;
		protected ToolTipOptions ToolTipOptions { get { return toolTipOptions; } }
		protected internal override ChartElement ChartElement { get { return toolTipOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowForSeries {
			get { return ToolTipOptions.ShowForSeries; }
			set { SetProperty("ShowForSeries", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowForPoints {
			get { return ToolTipOptions.ShowForPoints; }
			set { SetProperty("ShowForPoints", value); }
		}
		[PropertyForOptions,
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor(typeof(ToolTipPositionModelPickerEditor), typeof(UITypeEditor))]
		public ToolTipPositionModel ToolTipPosition {
			get { return toolTipPositionModel; }
			set { SetProperty("ToolTipPosition", value); }
		}
		public ToolTipOptionsModel(ToolTipOptions toolTipOptions, CommandManager commandManager)
			: base(commandManager) {
			this.toolTipOptions = toolTipOptions;
			Update();
		}
		protected override void AddChildren() {
			if(toolTipPositionModel != null)
				Children.Add(toolTipPositionModel);
			base.AddChildren();
		}
		public override void Update() {
			this.toolTipPositionModel = ModelHelper.CreateToolTipPositionModelInstance(ToolTipOptions.ToolTipPosition, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class ToolTipPositionModel : DesignerChartElementModelBase {
		readonly ToolTipPosition tooltipPosition;
		protected ToolTipPosition TooltipPosition { get { return tooltipPosition; } }
		protected internal override ChartElement ChartElement { get { return tooltipPosition; } }
		public ToolTipPositionModel(ToolTipPosition tooltipPosition, CommandManager commandManager)
			: base(commandManager) {
			this.tooltipPosition = tooltipPosition;
		}
	}
	[ModelOf(typeof(ToolTipMousePosition))]
	public class ToolTipMousePositionModel : ToolTipPositionModel {
		protected new ToolTipMousePosition TooltipPosition { get { return (ToolTipMousePosition)base.TooltipPosition; } }
		public ToolTipMousePositionModel(ToolTipMousePosition tooltipPosition, CommandManager commandManager)
			: base(tooltipPosition, commandManager) {
		}
	}
	public abstract class ToolTipPositionWithOffsetModel : ToolTipPositionModel {
		protected new ToolTipPositionWithOffset TooltipPosition { get { return (ToolTipPositionWithOffset)base.TooltipPosition; } }
		[PropertyForOptions]
		public int OffsetX {
			get { return TooltipPosition.OffsetX; }
			set { SetProperty("OffsetX", value); }
		}
		[PropertyForOptions]
		public int OffsetY {
			get { return TooltipPosition.OffsetY; }
			set { SetProperty("OffsetY", value); }
		}
		public ToolTipPositionWithOffsetModel(ToolTipPositionWithOffset tooltipPosition, CommandManager commandManager)
			: base(tooltipPosition, commandManager) {
		}
	}
	[ModelOf(typeof(ToolTipRelativePosition))]
	public class ToolTipRelativePositionModel : ToolTipPositionWithOffsetModel {
		protected new ToolTipRelativePosition TooltipPosition { get { return (ToolTipRelativePosition)base.TooltipPosition; } }
		public ToolTipRelativePositionModel(ToolTipRelativePosition tooltipPosition, CommandManager commandManager)
			: base(tooltipPosition, commandManager) {
		}
	}
	[ModelOf(typeof(ToolTipFreePosition))]
	public class ToolTipFreePositionModel : ToolTipPositionWithOffsetModel {
		DesignerChartElementModelBase dockTargetModel;
		protected new ToolTipFreePosition TooltipPosition { get { return (ToolTipFreePosition)base.TooltipPosition; } }
		[PropertyForOptions]
		public ToolTipDockCorner DockCorner {
			get { return TooltipPosition.DockCorner; }
			set { SetProperty("DockCorner", value); }
		}
		[Editor(typeof(DockTargetModelUITypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DockTargetModelTypeConverter))]
		public DesignerChartElementModelBase DockTarget {
			get { return dockTargetModel; }
			set { SetProperty("DockTarget", value); }
		}
		public ToolTipFreePositionModel(ToolTipFreePosition tooltipPosition, CommandManager commandManager)
			: base(tooltipPosition, commandManager) {
			Update();
		}
		public override void Update() {
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			this.dockTargetModel = chartModel.FindElementModel(TooltipPosition.DockTarget);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(CrosshairOptions)), TypeConverter(typeof(CrosshairOptionsTypeConverter))]
	public class CrosshairOptionsModel : DesignerChartElementModelBase {
		readonly CrosshairOptions crosshairOptions;
		LineStyleModel argumentLineStyleModel;
		LineStyleModel valueLineStyleModel;
		CrosshairLabelPositionModel commonLabelPositionModel;
		protected CrosshairOptions CrosshairOptions { get { return crosshairOptions; } }
		protected internal override ChartElement ChartElement { get { return crosshairOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowArgumentLabels {
			get { return CrosshairOptions.ShowArgumentLabels; }
			set { SetProperty("ShowArgumentLabels", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowValueLabels {
			get { return CrosshairOptions.ShowValueLabels; }
			set { SetProperty("ShowValueLabels", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowCrosshairLabels {
			get { return CrosshairOptions.ShowCrosshairLabels; }
			set { SetProperty("ShowCrosshairLabels", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowOnlyInFocusedPane {
			get { return CrosshairOptions.ShowOnlyInFocusedPane; }
			set { SetProperty("ShowOnlyInFocusedPane", value); }
		}
		[PropertyForOptions]
		public CrosshairLabelMode CrosshairLabelMode {
			get { return CrosshairOptions.CrosshairLabelMode; }
			set { SetProperty("CrosshairLabelMode", value); }
		}
		[PropertyForOptions]
		public CrosshairSnapMode SnapMode {
			get { return CrosshairOptions.SnapMode; }
			set { SetProperty("SnapMode", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowArgumentLine {
			get { return CrosshairOptions.ShowArgumentLine; }
			set { SetProperty("ShowArgumentLine", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowValueLine {
			get { return CrosshairOptions.ShowValueLine; }
			set { SetProperty("ShowValueLine", value); }
		}
		[PropertyForOptions]
		public Color ArgumentLineColor {
			get { return CrosshairOptions.ArgumentLineColor; }
			set { SetProperty("ArgumentLineColor", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions]
		public LineStyleModel ArgumentLineStyle { get { return argumentLineStyleModel; } }
		[PropertyForOptions]
		public Color ValueLineColor {
			get { return CrosshairOptions.ValueLineColor; }
			set { SetProperty("ValueLineColor", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions]
		public LineStyleModel ValueLineStyle { get { return valueLineStyleModel; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool HighlightPoints {
			get { return CrosshairOptions.HighlightPoints; }
			set { SetProperty("HighlightPoints", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowGroupHeaders {
			get { return CrosshairOptions.ShowGroupHeaders; }
			set { SetProperty("ShowGroupHeaders", value); }
		}
		[
		PropertyForOptions,
		Editor(typeof(GroupHeaderModelPatternEditor), typeof(UITypeEditor))
		]
		public string GroupHeaderPattern {
			get { return CrosshairOptions.GroupHeaderPattern; }
			set { SetProperty("GroupHeaderPattern", value); }
		}
		[PropertyForOptions,
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor(typeof(CrosshairLabelPositionModelPickerEditor), typeof(UITypeEditor))]
		public CrosshairLabelPositionModel CommonLabelPosition {
			get { return commonLabelPositionModel; }
			set { SetProperty("CommonLabelPosition", value); }
		}
		public CrosshairOptionsModel(CrosshairOptions crosshairOptions, CommandManager commandManager)
			: base(commandManager) {
			this.crosshairOptions = crosshairOptions;
			Update();
		}
		protected override void AddChildren() {
			if(argumentLineStyleModel != null)
				Children.Add(argumentLineStyleModel);
			if(valueLineStyleModel != null)
				Children.Add(valueLineStyleModel);
			if(commonLabelPositionModel != null)
				Children.Add(commonLabelPositionModel);
			base.AddChildren();
		}
		public override void Update() {
			this.argumentLineStyleModel = new LineStyleModel(CrosshairOptions.ArgumentLineStyle, CommandManager);
			this.valueLineStyleModel = new LineStyleModel(CrosshairOptions.ValueLineStyle, CommandManager);
			this.commonLabelPositionModel = ModelHelper.CreateCrosshairLabelPositionModelInstance(CrosshairOptions.CommonLabelPosition, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class CrosshairLabelPositionModel : DesignerChartElementModelBase {
		readonly CrosshairLabelPosition labelPosition;
		protected CrosshairLabelPosition LabelPosition { get { return labelPosition; } }
		protected internal override ChartElement ChartElement { get { return labelPosition; } }
		[PropertyForOptions]
		public int OffsetX {
			get { return LabelPosition.OffsetX; }
			set { SetProperty("OffsetX", value); }
		}
		[PropertyForOptions]
		public int OffsetY {
			get { return LabelPosition.OffsetY; }
			set { SetProperty("OffsetY", value); }
		}
		public CrosshairLabelPositionModel(CrosshairLabelPosition labelPosition, CommandManager commandManager)
			: base(commandManager) {
			this.labelPosition = labelPosition;
		}
	}
	[ModelOf(typeof(CrosshairMousePosition))]
	public class CrosshairMousePositionModel : CrosshairLabelPositionModel {
		protected new CrosshairMousePosition LabelPosition { get { return (CrosshairMousePosition)base.LabelPosition; } }
		public CrosshairMousePositionModel(CrosshairMousePosition seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
		}
	}
	[ModelOf(typeof(CrosshairFreePosition))]
	public class CrosshairFreePositionModel : CrosshairLabelPositionModel {
		DesignerChartElementModelBase dockTargetModel;
		protected new CrosshairFreePosition LabelPosition { get { return (CrosshairFreePosition)base.LabelPosition; } }
		[PropertyForOptions]
		public DockCorner DockCorner {
			get { return LabelPosition.DockCorner; }
			set { SetProperty("DockCorner", value); }
		}
		[Editor(typeof(DockTargetModelUITypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DockTargetModelTypeConverter))]
		public DesignerChartElementModelBase DockTarget {
			get { return dockTargetModel; }
			set { SetProperty("DockTarget", value); }
		}
		public CrosshairFreePositionModel(CrosshairFreePosition seriesLabel, CommandManager commandManager)
			: base(seriesLabel, commandManager) {
			Update();
		}
		public override void Update() {
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (chartModel == null)
				return;
			this.dockTargetModel = chartModel.FindElementModel(LabelPosition.DockTarget);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(TaskLinkOptions))]
	public class TaskLinkOptionsModel : DesignerChartElementModelBase {
		readonly TaskLinkOptions linkOptions;
		protected TaskLinkOptions LinkOptions { get { return linkOptions; } }
		protected internal override ChartElement ChartElement { get { return linkOptions; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return LinkOptions.Visible; }
			set { SetProperty("Visible", value); }
		}
		[PropertyForOptions]
		public int ArrowWidth {
			get { return LinkOptions.ArrowWidth; }
			set { SetProperty("ArrowWidth", value); }
		}
		[PropertyForOptions]
		public int ArrowHeight {
			get { return LinkOptions.ArrowHeight; }
			set { SetProperty("ArrowHeight", value); }
		}
		[PropertyForOptions]
		public int MinIndent {
			get { return LinkOptions.MinIndent; }
			set { SetProperty("MinIndent", value); }
		}
		[PropertyForOptions]
		public int Thickness {
			get { return LinkOptions.Thickness; }
			set { SetProperty("Thickness", value); }
		}
		[PropertyForOptions]
		public TaskLinkColorSource ColorSource {
			get { return LinkOptions.ColorSource; }
			set { SetProperty("ColorSource", value); }
		}
		[PropertyForOptions]
		public Color Color {
			get { return LinkOptions.Color; }
			set { SetProperty("Color", value); }
		}
		public TaskLinkOptionsModel(TaskLinkOptions linkOptions, CommandManager commandManager)
			: base(commandManager) {
			this.linkOptions = linkOptions;
		}
	}
}
