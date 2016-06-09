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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum RangeAreaLabelKind {
		OneLabel,
		TwoLabels,
		MaxValueLabel,
		MinValueLabel,
		Value1Label,
		Value2Label
	}
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel))
	]
	public class RangeAreaSeries2D : XYSeries2D, ISupportTransparency, ISupportSeriesBorder, IGeometryHolder {		
		public static readonly DependencyProperty Marker1ModelProperty = DependencyPropertyManager.Register("Marker1Model",
			typeof(Marker2DModel), typeof(RangeAreaSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty Marker1SizeProperty = DependencyPropertyManager.Register("Marker1Size",
			typeof(int), typeof(RangeAreaSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSeries2D.MarkerSizeValidation);
		public static readonly DependencyProperty Marker1VisibleProperty = DependencyPropertyManager.Register("Marker1Visible",
			typeof(bool), typeof(RangeAreaSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty Marker2ModelProperty = DependencyPropertyManager.Register("Marker2Model",
		   typeof(Marker2DModel), typeof(RangeAreaSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty Marker2SizeProperty = DependencyPropertyManager.Register("Marker2Size",
			typeof(int), typeof(RangeAreaSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSeries2D.MarkerSizeValidation);
		public static readonly DependencyProperty Marker2VisibleProperty = DependencyPropertyManager.Register("Marker2Visible",
			typeof(bool), typeof(RangeAreaSeries2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty TransparencyProperty = DependencyPropertyManager.Register("Transparency",
			typeof(double), typeof(RangeAreaSeries2D), new PropertyMetadata(0.0, ChartElementHelper.Update), AreaSeries2D.ValidateTransparency);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Marker2DAnimationBase), typeof(RangeAreaSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
			typeof(Area2DAnimationBase), typeof(RangeAreaSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		public static readonly DependencyProperty Border1Property = DependencyPropertyManager.Register("Border1",
			typeof(SeriesBorder), typeof(RangeAreaSeries2D), new PropertyMetadata(null, Border1Changed));
		static readonly DependencyPropertyKey ActualBorder1PropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder1",
			typeof(SeriesBorder), typeof(RangeAreaSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty ActualBorder1Property = ActualBorder1PropertyKey.DependencyProperty;
		public static readonly DependencyProperty Border2Property = DependencyPropertyManager.Register("Border2",
		   typeof(SeriesBorder), typeof(RangeAreaSeries2D), new PropertyMetadata(null, Border2Changed));
		static readonly DependencyPropertyKey ActualBorder2PropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorder2",
			typeof(SeriesBorder), typeof(RangeAreaSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty ActualBorder2Property = ActualBorder2PropertyKey.DependencyProperty;
		public static readonly DependencyProperty Value2DataMemberProperty = DependencyPropertyManager.Register("Value2DataMember",
			typeof(string), typeof(RangeAreaSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty Value2Property = DependencyPropertyManager.RegisterAttached("Value2",
			typeof(double), typeof(RangeAreaSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty DateTimeValue2Property = DependencyPropertyManager.RegisterAttached("DateTimeValue2",
			typeof(DateTime), typeof(RangeAreaSeries2D), new PropertyMetadata(DateTime.MinValue, SeriesPoint.Update));
		public static readonly DependencyProperty LabelKindProperty = DependencyPropertyManager.RegisterAttached("LabelKind",
			typeof(RangeAreaLabelKind), typeof(RangeAreaSeries2D), new PropertyMetadata(RangeAreaLabelKind.TwoLabels, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelValueSeparatorProperty = DependencyPropertyManager.Register("LabelValueSeparator",
			typeof(string), typeof(RangeAreaSeries2D), new PropertyMetadata(", ", LabelValueSeparatorPropertyChanged));
		public static readonly DependencyProperty LegendValueSeparatorProperty = DependencyPropertyManager.Register("LegendValueSeparator",
			typeof(string), typeof(RangeAreaSeries2D), new PropertyMetadata(", ", LegendValueSeparatorPropertyChanged));
		public static readonly DependencyProperty MinValueAngleProperty = DependencyPropertyManager.RegisterAttached("MinValueAngle",
		   typeof(double), typeof(RangeAreaSeries2D), new PropertyMetadata(270.0, ChartElementHelper.Update));
		public static readonly DependencyProperty MaxValueAngleProperty = DependencyPropertyManager.RegisterAttached("MaxValueAngle",
		   typeof(double), typeof(RangeAreaSeries2D), new PropertyMetadata(90.0, ChartElementHelper.Update));
		static void LabelValueSeparatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.UpdateLabelTextPattern();
			ChartElementHelper.Update(d, e);
		}
		static void LegendValueSeparatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Series series = d as Series;
			if (series != null)
				series.UpdateLegendTextPattern();
			ChartElementHelper.Update(d, e);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker1Model"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel Marker1Model {
			get { return (Marker2DModel)GetValue(Marker1ModelProperty); }
			set { SetValue(Marker1ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker1Size"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Marker1Size {
			get { return (int)GetValue(Marker1SizeProperty); }
			set { SetValue(Marker1SizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker1Visible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Marker1Visible {
			get { return (bool)GetValue(Marker1VisibleProperty); }
			set { SetValue(Marker1VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DTransparency"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double Transparency {
			get { return (double)GetValue(TransparencyProperty); }
			set { SetValue(TransparencyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DAnimationBase PointAnimation {
			get { return (Marker2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Area2DAnimationBase SeriesAnimation {
			get { return (Area2DAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DBorder1"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesBorder Border1 {
			get { return (SeriesBorder)GetValue(Border1Property); }
			set { SetValue(Border1Property, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DBorder2"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public SeriesBorder Border2 {
			get { return (SeriesBorder)GetValue(Border2Property); }
			set { SetValue(Border2Property, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker2Model"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel Marker2Model {
			get { return (Marker2DModel)GetValue(Marker2ModelProperty); }
			set { SetValue(Marker2ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker2Size"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Marker2Size {
			get { return (int)GetValue(Marker2SizeProperty); }
			set { SetValue(Marker2SizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DMarker2Visible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Marker2Visible {
			get { return (bool)GetValue(Marker2VisibleProperty); }
			set { SetValue(Marker2VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DValue2DataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string Value2DataMember {
			get { return (string)GetValue(Value2DataMemberProperty); }
			set { SetValue(Value2DataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DLabelValueSeparator"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string LabelValueSeparator {
			get { return (string)GetValue(LabelValueSeparatorProperty); }
			set { SetValue(LabelValueSeparatorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeAreaSeries2DLegendValueSeparator"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string LegendValueSeparator {
			get { return (string)GetValue(LegendValueSeparatorProperty); }
			set { SetValue(LegendValueSeparatorProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SeriesBorder ActualBorder1 {
			get { return (SeriesBorder)GetValue(ActualBorder1Property); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SeriesBorder ActualBorder2 {
			get { return (SeriesBorder)GetValue(ActualBorder2Property); }
		}
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static double GetMinValueAngle(SeriesLabel label) {
			return (double)label.GetValue(MinValueAngleProperty);
		}
		public static void SetMinValueAngle(SeriesLabel label, double value) {
			label.SetValue(MinValueAngleProperty, value);
		}
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static double GetMaxValueAngle(SeriesLabel label) {
			return (double)label.GetValue(MaxValueAngleProperty);
		}
		public static void SetMaxValueAngle(SeriesLabel label, double value) {
			label.SetValue(MaxValueAngleProperty, value);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetValue2(SeriesPoint point) {
			return (double)point.GetValue(Value2Property);
		}
		public static void SetValue2(SeriesPoint point, double value) {
			point.SetValue(Value2Property, value);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static DateTime GetDateTimeValue2(SeriesPoint point) {
			return (DateTime)point.GetValue(DateTimeValue2Property);
		}
		public static void SetDateTimeValue2(SeriesPoint point, DateTime dateTimeValue) {
			point.SetValue(DateTimeValue2Property, dateTimeValue);
		}
		[
		Category(Categories.Layout),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public static RangeAreaLabelKind GetLabelKind(SeriesLabel label) {
			return (RangeAreaLabelKind)label.GetValue(LabelKindProperty);
		}
		public static void SetLabelKind(SeriesLabel label, RangeAreaLabelKind value) {
			label.SetValue(LabelKindProperty, value);
		}
		static void Border1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeAreaSeries2D series = d as RangeAreaSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorder1PropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, e.NewValue as SeriesBorder, series);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		static void Border2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeAreaSeries2D series = d as RangeAreaSeries2D;
			if (series != null) {
				SeriesBorder newBorder = e.NewValue as SeriesBorder;
				series.SetValue(ActualBorder2PropertyKey, newBorder == null ? SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness) : newBorder);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SeriesBorder, newBorder, series);
				series.UpdateAdditionalGeometryAppearance();
			}
		}
		const int DefaultLineThickness = 1;
		internal Color Border2Color {
			get {
				SeriesBorder border = Border2;
				if (border != null) {
					SolidColorBrush brush = border.Brush;
					if (brush != null)
						return brush.Color;
				}
				return BaseColor;
			}
		}
		protected internal override bool IsLabelConnectorItemVisible { get { return ActualLabel.ConnectorVisible; } }
		protected internal override bool LabelsResolveOverlappingSupported { get { return true; } }
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram2D); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Size; } }
		protected internal override bool ArePointsVisible { get { return Marker1Visible || Marker2Visible; } }
		protected internal override bool HasAdditionalGeometryBottomStrip { get { return true; } }
		protected override int PointDimension { get { return 2; } }
		protected override string DefaultLegendTextPattern { get { return "{" + PatternUtils.Value1Placeholder + "}, {" + PatternUtils.Value2Placeholder + "}"; } }
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipRangeValueToStringConverter(this); } }
		protected override Type PointInterfaceType {
			get { return typeof(IRangePoint); }
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected override bool HasInvisibleMarkers { get { return !Marker1Visible || !Marker2Visible; } }
		public RangeAreaSeries2D() {
			DefaultStyleKey = typeof(RangeAreaSeries2D);
			this.SetValue(ActualBorder1PropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness));
			this.SetValue(ActualBorder2PropertyKey, SeriesWithMarkerHelper.CreateDefaultBorder(DefaultLineThickness));
		}
		#region IGeometryHolder
		GeometryStripCreator IGeometryHolder.CreateStripCreator() {
			return new RangeAreaGeometryStripCreator();
		}
		#endregion
		#region ISupportSeriesBorder implementation
		SeriesBorder ISupportSeriesBorder.Border { get { return Border1; } }
		SeriesBorder ISupportSeriesBorder.ActualBorder { get { return ActualBorder1; } }
		#endregion
		double GetLabelAngle(IRangePoint rangePoint, SeriesLabel label, RangeValueLevel valueLevel) {
			double angle;
			if (valueLevel == RangeValueLevel.Value1)
				angle = rangePoint.Value1 >= rangePoint.Value2 ? GetMaxValueAngle(label) : GetMinValueAngle(label);
			else
				angle = rangePoint.Value1 >= rangePoint.Value2 ? GetMinValueAngle(label) : GetMaxValueAngle(label);
			return angle;
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			IRangePoint rangePoint = (IRangePoint)refinedPoint;
			yield return rangePoint.Min;
			yield return rangePoint.Max;
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return ColorUtils.GetNotTransparentBrush(brush);
		}
		protected internal override SolidColorBrush MixColor(Color color) {
			return ColorUtils.GetTransparentBrush(ColorUtils.MixWithDefaultColor(color), Transparency);
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalRangeAreaSeriesGeometry(this);
		}
		protected internal override bool IsPointValueVisible(RangeValueLevel valueLevel) {
			return valueLevel == RangeValueLevel.Value1 ? Marker1Visible : Marker2Visible;
		}		
		protected internal override double[] GetPointValues(SeriesPoint point) {
			return new double[] { point.NonAnimatedValue, GetValue2(point) };
		}
		protected internal override void SetPointValues(SeriesPoint seriesPoint, double[] values, DateTime[] dateTimeValues) {
			base.SetPointValues(seriesPoint, values, dateTimeValues);
			if (values != null && values.Length > 1)
				SetValue2(seriesPoint, values[1]);
			if (dateTimeValues != null && dateTimeValues.Length > 1)
				SetDateTimeValue2(seriesPoint, dateTimeValues[1]);
		}
		protected internal override DateTime[] GetPointDateTimeValues(SeriesPoint point) {
			return new DateTime[] { point.DateTimeValue, GetDateTimeValue2(point) };
		}
		protected internal override double[] GetAnimatedPointValues(SeriesPoint point) {
			return ValueScaleTypeInternal == ScaleType.Numerical ? new double[] { point.Value, GetValue2(point) } : point.InternalValues;
		}
		protected internal override SeriesPointItem[] CreateSeriesPointItems(RefinedPoint refinedPoint, SeriesPointData seriesPointData) {
			return new SeriesPointItem[] { new SeriesPointItem(this, seriesPointData, RangeValueLevel.Value1),
										   new SeriesPointItem(this, seriesPointData, RangeValueLevel.Value2)};
		}
		protected internal override RangeValueLevel GetValueLevelForLabel(RefinedPoint refinedPoint) {
			RangeAreaLabelKind labelPosition = RangeAreaSeries2D.GetLabelKind(ActualLabel);
			switch (labelPosition) {
				case RangeAreaLabelKind.TwoLabels:
				case RangeAreaLabelKind.OneLabel:
				case RangeAreaLabelKind.Value1Label:
					return RangeValueLevel.Value1;
				case RangeAreaLabelKind.Value2Label:
					return RangeValueLevel.Value2;
				case RangeAreaLabelKind.MaxValueLabel:
					return ((IRangePoint)refinedPoint).Value1 >= ((IRangePoint)refinedPoint).Value2 ? RangeValueLevel.Value1 : RangeValueLevel.Value2;
				case RangeAreaLabelKind.MinValueLabel:
					return ((IRangePoint)refinedPoint).Value1 < ((IRangePoint)refinedPoint).Value2 ? RangeValueLevel.Value1 : RangeValueLevel.Value2;
				default:
					ChartDebug.Fail("Unknown range area label kind.");
					return RangeValueLevel.Value1;
			}
		}
		protected internal override string[] GetLabelsTexts(RefinedPoint refinedPoint) {
			string labelText = string.Empty;
			if (ActualLabel.Formatter == null) {
				PatternParser patternParser;
				RangeAreaLabelKind labelKind = GetLabelKind(ActualLabel);
				switch (labelKind) {
					case RangeAreaLabelKind.TwoLabels:
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder), this);
						patternParser.SetContext(refinedPoint, this);
						string labelText1 = patternParser.GetText();
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value2Placeholder), this);
						patternParser.SetContext(refinedPoint, this);
						string labelText2 = patternParser.GetText();
						return new string[] { labelText1, labelText2 };
					case RangeAreaLabelKind.OneLabel:
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder, PatternUtils.Value2Placeholder, LabelValueSeparator), this);
						break;
					case RangeAreaLabelKind.Value1Label:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value1Placeholder), this);
						break;
					case RangeAreaLabelKind.Value2Label:
						patternParser = new PatternParser(
							PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, PatternUtils.Value2Placeholder), this);
						break;
					case RangeAreaLabelKind.MinValueLabel:
						string minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), this);
						break;
					case RangeAreaLabelKind.MaxValueLabel:
						string maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, maxValuePlaceholder), this);
						break;
					default:
						ChartDebug.Fail("Unexpected RangeArea label kind.");
						return new string[0];
				}
				patternParser.SetContext(refinedPoint, this);
				labelText = patternParser.GetText();
			}
			else
				labelText = ActualLabel.Formatter.GetDataLabelText(refinedPoint.SeriesPoint);
			return new string[] { labelText };
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Marker2DFadeInAnimation();
		}
		protected internal override SeriesAnimationBase CreateDefaultSeriesAnimation() {
			return new Area2DStretchFromNearAnimation();
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			if (pointItem.PointItemPresentation.CanLayout)
				SeriesWithMarkerHelper.CompletePointLayout(pointItem, Diagram, GetActualPointAnimation(), IsAxisXReversed, IsAxisYReversed);
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxContinuousSeriesRangeValues(point, range, isHorizontalCrosshair, crosshairPaneInfo, snapMode);
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is SeriesBorder) {
				UpdateAdditionalGeometryAppearance();
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected override Series CreateObjectForClone() {
			return new RangeAreaSeries2D();
		}
		protected override IList<string> GetValueDataMembers() {
			return new string[] { ValueDataMember, Value2DataMember };
		}
		protected override ISeriesPoint CreateSeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			SeriesPoint point = (SeriesPoint)base.CreateSeriesPoint(argument, internalArgument, values, internalValues, tag, hint, color);
			if (values[0] is double)
				SetValue2(point, (double)values[1]);
			else if (values[0] is DateTime)
				SetDateTimeValue2(point, (DateTime)values[1]);
			return point;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			RangeAreaSeries2D rangeAreaSeries = series as RangeAreaSeries2D;
			if (rangeAreaSeries != null) {
				if (CopyPropertyValueHelper.IsValueSet(rangeAreaSeries, Marker1ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeAreaSeries, Marker1ModelProperty))
						Marker1Model = rangeAreaSeries.Marker1Model.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Marker1SizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Marker1VisibleProperty);
				if (CopyPropertyValueHelper.IsValueSet(rangeAreaSeries, Marker2ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeAreaSeries, Marker2ModelProperty))
						Marker2Model = rangeAreaSeries.Marker2Model.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Marker2SizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Marker2VisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, TransparencyProperty);
				if (CopyPropertyValueHelper.IsValueSet(rangeAreaSeries, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeAreaSeries, PointAnimationProperty))
						PointAnimation = rangeAreaSeries.PointAnimation.CloneAnimation() as Marker2DAnimationBase;
				if (CopyPropertyValueHelper.IsValueSet(rangeAreaSeries, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeAreaSeries, SeriesAnimationProperty))
						SeriesAnimation = rangeAreaSeries.SeriesAnimation.CloneAnimation() as Area2DAnimationBase;
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Border1Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Border2Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, Value2DataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, LabelValueSeparatorProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeAreaSeries, LegendValueSeparatorProperty);
				if (Label != null && rangeAreaSeries.Label != null) {
					RangeAreaSeries2D.SetMinValueAngle(Label, RangeAreaSeries2D.GetMinValueAngle(rangeAreaSeries.Label));
					RangeAreaSeries2D.SetMaxValueAngle(Label, RangeAreaSeries2D.GetMaxValueAngle(rangeAreaSeries.Label));
					RangeAreaSeries2D.SetLabelKind(Label, RangeAreaSeries2D.GetLabelKind(rangeAreaSeries.Label));
				}
			}
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value_1 || valueLevel == ValueLevelInternal.Value_2;
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			if (valueLevel == RangeValueLevel.Value1)
				return Marker1Model;
			else
				return Marker2Model;
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			IRangePoint rangePoint = labelItem.RefinedPoint;
			if (rangePoint == null)
				return null;
			if (RangeAreaSeries2D.GetLabelKind(ActualLabel) == RangeAreaLabelKind.OneLabel) {
				Point? centerPoint = RangeArea2DHelper.CalculateLabelAnchorPoint(this, mapping, labelItem, new MinMaxValues(rangePoint.Value1, rangePoint.Value2), transform);
				return centerPoint.HasValue ? new XYSeriesLabel2DLayout(labelItem, mapping, centerPoint.Value, GRect2D.Empty) : null;
			}
			else {
				double value = labelItem.PointItem.ValueLevel == RangeValueLevel.Value1 ? rangePoint.Value1 : rangePoint.Value2;
				Point anchorPoint = transform.Transform(mapping.GetRoundedDiagramPoint(rangePoint.Argument, value));
				SeriesLabel label = labelItem.Label;
				int indent = label.Indent;
				double angle = GetLabelAngle(rangePoint, label, labelItem.PointItem.ValueLevel);
				return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, indent,
					MathUtils.Degree2Radian(NormalizeLabelAngle(angle)), GraphicsUtils.ConvertRect(SeriesWithMarkerHelper.CalcAnchorHole(anchorPoint, indent)));
			}
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			SeriesWithMarkerHelper.FillMarkerAnimationKinds(animationKinds);
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			SeriesWithMarkerHelper.FillAreaAnimationKinds(animationKinds);
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return CreatePointItemLayout(mapping, pointItem);
		}
		protected internal override Transform CreateSeriesAnimationTransform(IMapping mapping) {
			Area2DAnimationBase areaAnimation = GetActualSeriesAnimation() as Area2DAnimationBase;
			return areaAnimation == null ? null :
				areaAnimation.CreateAnimatedTransformation(mapping.Viewport, mapping.GetDiagramPoint(0.0, 0.0), IsAxisXReversed, IsAxisYReversed, mapping.Rotated, SeriesProgress.ActualProgress);
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			IRangePoint rangePoint = pointItem.RefinedPoint;
			if (RangeAreaSeries2D.GetLabelKind(ActualLabel) == RangeAreaLabelKind.OneLabel && inLabel) {
				MinMaxValues values = rangePoint == null ? new MinMaxValues(rangePoint.Value2) : new MinMaxValues(rangePoint.Value1, rangePoint.Value2);
				double centerValue = MathUtils.CalculateCenterValueByRange(((IAxisData)ActualAxisY).VisualRange, values);
				return Double.IsNaN(centerValue) ? new Point() : transform.Transform(mapping.GetRoundedDiagramPoint(rangePoint.Argument, centerValue));
			}
			else {
				double value = pointItem.ValueLevel == RangeValueLevel.Value1 ? rangePoint.Value1 : rangePoint.Value2;
				Point anchorPoint = transform.Transform(mapping.GetRoundedDiagramPoint(rangePoint.Argument, value));
				return anchorPoint;
			}
		}
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			IRangePoint pointInfo = pointItem.RefinedPoint;
			int markerSize = pointItem.ValueLevel == RangeValueLevel.Value1 ? Marker1Size : Marker2Size;
			double value = pointItem.ValueLevel == RangeValueLevel.Value1 ? pointInfo.Value1 : pointInfo.Value2;
			return SeriesWithMarkerHelper.CreateSeriesPointLayout(mapping, pointItem, markerSize, value);
		}
		protected override SeriesContainer CreateContainer() {
			return new RangeSeriesContainer(this);
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			string valueFormat = pointOptionsContainer.ConstructValueFormat(valueScaleType);
			string separator = string.Empty;
			if (pointOptionsContainer is LegendPointOptionsContainer)
				return "{" + PatternUtils.Value1Placeholder + valueFormat + "}" + LegendValueSeparator + "{" + PatternUtils.Value2Placeholder + valueFormat + "}";
			return "{" + PatternUtils.ValuePlaceholder + valueFormat + "}";
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.RangeViewPointPatterns;
		}
		protected internal override bool IsPointItemHidden(SeriesPointItem pointItem) {
			switch (pointItem.ValueLevel) {
				case RangeValueLevel.TwoValues:
					return false;
				case RangeValueLevel.Value1:
					return !Marker1Visible;
				case RangeValueLevel.Value2:
					return !Marker2Visible;
			}
			return base.IsPointItemHidden(pointItem);
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Marker2DAnimationBase))
				return;
			PointAnimation = value as Marker2DAnimationBase;
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is Area2DAnimationBase))
				return;
			SeriesAnimation = value as Area2DAnimationBase;
		}		
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public abstract class RangeSeriesValueToStringConverter : ValueToStringConverter {
		public static object[] SortValues(object[] values) {
			object[] sortedValues = new object[2];
			Array.Copy(values, 0, sortedValues, 0, 2);
			Array.Sort(sortedValues);
			return sortedValues;
		}
		readonly string separator;
		readonly bool isLegendConverter;
		protected abstract bool ShouldSortValues { get; }
		protected abstract bool IsOneLabel { get; }
		protected bool IsLegendConverter { get { return isLegendConverter; } }
		public RangeSeriesValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, bool isLegendConverter, string separator) : base(numericOptions, dateTimeOptions) {
			this.separator = separator;
			this.isLegendConverter = isLegendConverter;
		}
		public override string ConvertTo(object[] values) {
			if (ShouldSortValues)
				values = SortValues(values);
			if (isLegendConverter || IsOneLabel)
				return GetValueText(values[0]) + separator + GetValueText(values[1]);
			return base.ConvertTo(values);
		}
	}
	public class RangeAreaValueToStringConverter : RangeSeriesValueToStringConverter {
		readonly RangeAreaLabelKind labelKind;
		protected override bool ShouldSortValues { get { return labelKind == RangeAreaLabelKind.MinValueLabel || labelKind == RangeAreaLabelKind.MaxValueLabel; } }
		protected override bool IsOneLabel { get { return labelKind == RangeAreaLabelKind.OneLabel; } }
		public RangeAreaValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, RangeAreaLabelKind labelKind, string separator)
			: this(numericOptions, dateTimeOptions, labelKind, false, separator) {
		}
		public RangeAreaValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, RangeAreaLabelKind labelKind, bool isLegendConverter, string separator)
			: base(numericOptions, dateTimeOptions, isLegendConverter, separator) {
				this.labelKind = labelKind;
		}
		protected override object GetValue(object[] values) {
			switch (labelKind) {
				case RangeAreaLabelKind.TwoLabels:
				case RangeAreaLabelKind.Value1Label:
				case RangeAreaLabelKind.MinValueLabel:
					return values[0];
				case RangeAreaLabelKind.Value2Label:
				case RangeAreaLabelKind.MaxValueLabel:
					return values[1];
				default:
					ChartDebug.Fail("Unknown range area label kind.");
					return values[0];
			}
		}
	}
	public class RangeAreaValue2ToStringConverter : ValueToStringConverter {
		public RangeAreaValue2ToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions) : base(numericOptions, dateTimeOptions) {
		}
		protected override object GetValue(object[] values) {
			return values[1];
		}
	}
}
