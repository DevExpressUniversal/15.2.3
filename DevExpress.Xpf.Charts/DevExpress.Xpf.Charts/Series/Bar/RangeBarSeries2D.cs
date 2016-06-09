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
	public enum RangeBarLabelKind {
		OneLabel,
		TwoLabels,
		MaxValueLabel,
		MinValueLabel
	}
	public abstract class RangeBarSeries2D : BarSeries2DBase {
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(RangeBar2DModel), typeof(RangeBarSeries2D), new PropertyMetadata(ModelPropertyChanged));
		public static readonly DependencyProperty Value2DataMemberProperty = DependencyPropertyManager.Register("Value2DataMember",
		   typeof(string), typeof(RangeBarSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty Value2Property = DependencyPropertyManager.RegisterAttached("Value2",
			typeof(double), typeof(RangeBarSeries2D), new PropertyMetadata(Double.NaN,  SeriesPoint.Update));
		public static readonly DependencyProperty DateTimeValue2Property = DependencyPropertyManager.RegisterAttached("DateTimeValue2",
			typeof(DateTime), typeof(RangeBarSeries2D), new PropertyMetadata(DateTime.MinValue, SeriesPoint.Update));
		public static readonly DependencyProperty MinMarkerModelProperty = DependencyPropertyManager.Register("MinMarkerModel",
			typeof(Marker2DModel), typeof(RangeBarSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MinMarkerSizeProperty = DependencyPropertyManager.Register("MinMarkerSize",
			typeof(int), typeof(RangeBarSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSeries2D.MarkerSizeValidation);
		public static readonly DependencyProperty MinMarkerVisibleProperty = DependencyPropertyManager.Register("MinMarkerVisible",
			typeof(bool), typeof(RangeBarSeries2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty MaxMarkerModelProperty = DependencyPropertyManager.Register("MaxMarkerModel",
		   typeof(Marker2DModel), typeof(RangeBarSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MaxMarkerSizeProperty = DependencyPropertyManager.Register("MaxMarkerSize",
			typeof(int), typeof(RangeBarSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSeries2D.MarkerSizeValidation);
		public static readonly DependencyProperty MaxMarkerVisibleProperty = DependencyPropertyManager.Register("MaxMarkerVisible",
			typeof(bool), typeof(RangeBarSeries2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelKindProperty = DependencyPropertyManager.RegisterAttached("LabelKind",
			typeof(RangeBarLabelKind), typeof(RangeBarSeries2D), new PropertyMetadata(RangeBarLabelKind.TwoLabels, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelValueSeparatorProperty = DependencyPropertyManager.Register("LabelValueSeparator",
			typeof(string), typeof(RangeBarSeries2D), new PropertyMetadata(", ", LabelValueSeparatorPropertyChanged));
		public static readonly DependencyProperty LegendValueSeparatorProperty = DependencyPropertyManager.Register("LegendValueSeparator",
			typeof(string), typeof(RangeBarSeries2D), new PropertyMetadata(", ", LegendValueSeparatorPropertyChanged));
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
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public RangeBar2DModel Model {
			get { return (RangeBar2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DValue2DataMember"),
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
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMinMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MinMarkerModel {
			get { return (Marker2DModel)GetValue(MinMarkerModelProperty); }
			set { SetValue(MinMarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMinMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int MinMarkerSize {
			get { return (int)GetValue(MinMarkerSizeProperty); }
			set { SetValue(MinMarkerSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMinMarkerVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool MinMarkerVisible {
			get { return (bool)GetValue(MinMarkerVisibleProperty); }
			set { SetValue(MinMarkerVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMaxMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MaxMarkerModel {
			get { return (Marker2DModel)GetValue(MaxMarkerModelProperty); }
			set { SetValue(MaxMarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMaxMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int MaxMarkerSize {
			get { return (int)GetValue(MaxMarkerSizeProperty); }
			set { SetValue(MaxMarkerSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DMaxMarkerVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool MaxMarkerVisible {
			get { return (bool)GetValue(MaxMarkerVisibleProperty); }
			set { SetValue(MaxMarkerVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DLabelValueSeparator"),
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
	DevExpressXpfChartsLocalizedDescription("RangeBarSeries2DLegendValueSeparator"),
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
		public static RangeBarLabelKind GetLabelKind(SeriesLabel label) {
			return (RangeBarLabelKind)label.GetValue(LabelKindProperty);
		}
		public static void SetLabelKind(SeriesLabel label, RangeBarLabelKind value) {
			label.SetValue(LabelKindProperty, value);
		}
		static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeBarSeries2D series = d as RangeBarSeries2D;
			if (series != null)
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as Bar2DModel, e.NewValue as Bar2DModel, series);
			ChartElementHelper.Update(d, e);
		}
		protected internal override bool IsLabelConnectorItemVisible { get { return RangeBarSeries2D.GetLabelKind(ActualLabel) != RangeBarLabelKind.OneLabel && ActualLabel.ConnectorVisible; } }
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipRangeValueToStringConverter(this); } }
		protected override int PointDimension { get { return 2; } }
		protected override string DefaultLegendTextPattern { get { return "{" + PatternUtils.Value1Placeholder + "}, {" + PatternUtils.Value2Placeholder + "}"; } }
		protected override Type PointInterfaceType {
			get { return typeof(IRangePoint); }
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected override bool HasInvisibleMarkers { get { return !MinMarkerVisible || !MaxMarkerVisible; } }
		BarSeries2DPointLayout GetBarLayout(SeriesPointItem pointItem) {
			SeriesPointItem barPointItem = null;
			foreach (SeriesPointItem item in pointItem.SeriesPointData.PointItems)
				if (item.ValueLevel == RangeValueLevel.TwoValues)
					barPointItem = item;
			if (barPointItem == null)
				return null;
			return barPointItem.Layout as BarSeries2DPointLayout;
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			IRangePoint rangePoint = (IRangePoint)refinedPoint;
			yield return rangePoint.Min;
			yield return rangePoint.Max;
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
		protected internal override bool IsPointValueVisible(RangeValueLevel valueLevel) {
			if (valueLevel == RangeValueLevel.TwoValues)
				return true;
			return valueLevel == RangeValueLevel.Value1 ? MinMarkerVisible : MaxMarkerVisible;
		}
		protected internal override bool IsNegativeBar(SeriesPointItem pointItem) {
			return false;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			if (pointItem.ValueLevel == RangeValueLevel.TwoValues)
				base.CompletePointLayout(pointItem);
			else {
				XYDiagram2D diagramXY = Diagram as XYDiagram2D;
				MarkerSeries2DPointLayout layout = pointItem.Layout as MarkerSeries2DPointLayout;
				BarSeries2DPointLayout barlayout = GetBarLayout(pointItem);
				if (diagramXY == null || layout == null || barlayout == null || pointItem.RefinedPoint == null)
					return;
				Rect markerBounds = layout.InitialBounds;
				Bar2DAnimationBase animation = GetActualPointAnimation() as Bar2DAnimationBase;
				if (animation != null) {
					double progress = pointItem.PointProgress.ActualProgress;
					if (pointItem.ValueLevel == RangeValueLevel.Value1)
						markerBounds = animation.CreateAnimatedMinMarkerBounds(markerBounds, barlayout.InitialBounds, layout.Viewport, false, IsAxisXReversed, IsAxisYReversed, diagramXY.Rotated, progress);
					else
						markerBounds = animation.CreateAnimatedMaxMarkerBounds(markerBounds, barlayout.InitialBounds, layout.Viewport, false, IsAxisXReversed, IsAxisYReversed, diagramXY.Rotated, progress);
				}
				Transform pointTransform = SeriesWithMarkerHelper.CreatepointTransform(markerBounds, diagramXY);
				layout.Complete(markerBounds, pointTransform);
			}
		}
		protected internal override SeriesPointItem[] CreateSeriesPointItems(RefinedPoint pointInfo, SeriesPointData seriesPointData) {
			return new SeriesPointItem[] { new SeriesPointItem(this, seriesPointData, RangeValueLevel.TwoValues),
										   new SeriesPointItem(this, seriesPointData, RangeValueLevel.Value1),
										   new SeriesPointItem(this, seriesPointData, RangeValueLevel.Value2) };
		}
		protected internal override RangeValueLevel GetValueLevelForLabel(RefinedPoint refinedPoint) {
			RangeBarLabelKind labelPosition = RangeBarSeries2D.GetLabelKind(ActualLabel);
			switch (labelPosition) {
				case RangeBarLabelKind.TwoLabels:
					return RangeValueLevel.Value1;
				case RangeBarLabelKind.OneLabel:
					return RangeValueLevel.TwoValues;
				case RangeBarLabelKind.MaxValueLabel:
					return RangeValueLevel.Value2;
				case RangeBarLabelKind.MinValueLabel:
					return RangeValueLevel.Value1;
				default:
					ChartDebug.Fail("Unknown range bar label kind.");
					return RangeValueLevel.Value1;
			}
		}
		protected internal override string[] GetLabelsTexts(RefinedPoint refinedPoint) {
			string labelText = string.Empty;
			if (ActualLabel.Formatter == null) {
				PatternParser patternParser;
				RangeBarLabelKind labelKind = GetLabelKind(ActualLabel);
				switch (labelKind) {
					case RangeBarLabelKind.TwoLabels:
						string minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), this);
						patternParser.SetContext(refinedPoint, this);
						string minText = patternParser.GetText();
						string maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, maxValuePlaceholder), this);
						patternParser.SetContext(refinedPoint, this);
						string maxText = patternParser.GetText();
						return new string[] { minText, maxText };
					case RangeBarLabelKind.OneLabel:
						minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						maxValuePlaceholder = PatternUtils.GetMaxValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder, maxValuePlaceholder, LabelValueSeparator), this);
						break;
					case RangeBarLabelKind.MinValueLabel:
						minValuePlaceholder = PatternUtils.GetMinValuePlaceholder(refinedPoint, (Scale)ValueScaleTypeInternal);
						patternParser = new PatternParser(PatternUtils.ReplacePlaceholder(ActualLabelTextPattern, PatternUtils.ValuePlaceholder, minValuePlaceholder), this);
						break;
					case RangeBarLabelKind.MaxValueLabel:
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
		protected override bool IsDownwardBar(RefinedPoint pointInfo) {
			return IsAxisYReversed ? true : false;
		}
		protected override double GetLowValue(RefinedPoint pointInfo) {
			IRangePoint rangePoint = (IRangePoint)pointInfo;
			return rangePoint.Min;
		}
		protected override double GetHighValue(RefinedPoint pointInfo) {
			IRangePoint rangePoint = (IRangePoint)pointInfo;
			return rangePoint.Max;
		}
		protected override void CorrectZeroHeight(ref double bound1, ref double bound2) {
			if (bound1 == bound2)
				bound1 += 1.0;
		}
		protected override double GetLabelAngle(RangeValueLevel valueLevel) {
			return valueLevel == RangeValueLevel.Value1 ? Math.PI / 2 : -Math.PI / 2;
		}
		protected override double GetPointValue(RangeValueLevel valueLevel, Bar2D bar) {
			return valueLevel == RangeValueLevel.Value1 ? bar.Bottom : bar.Top;
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			if (valueLevel == RangeValueLevel.Value1)
				return MinMarkerModel;
			else if (valueLevel == RangeValueLevel.Value2)
				return MaxMarkerModel;
			else
				return Model;
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				CustomRangeBar2DModel model = sender as CustomRangeBar2DModel;
				if (model != null) {
					ChartElementHelper.Update(this);
					return true;
				}
			}
			return base.PerformWeakEvent(managerType, sender, e);
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			if (labelItem.RefinedPoint == null)
				return null;
			return CalculateSeriesLabelLayout(labelItem, mapping, transform, RangeBarSeries2D.GetLabelKind(ActualLabel) == RangeBarLabelKind.OneLabel);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			RangeBarSeries2D rangeBarSeries = series as RangeBarSeries2D;
			if (rangeBarSeries != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, Value2DataMemberProperty);
				if (CopyPropertyValueHelper.IsValueSet(rangeBarSeries, ModelProperty) && CopyPropertyValueHelper.VerifyValues(this, rangeBarSeries, ModelProperty))
					Model = rangeBarSeries.Model.CloneModel();
				if (CopyPropertyValueHelper.IsValueSet(rangeBarSeries, MinMarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeBarSeries, MinMarkerModelProperty))
						MinMarkerModel = rangeBarSeries.MinMarkerModel.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, MinMarkerSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, MinMarkerVisibleProperty);
				if (CopyPropertyValueHelper.IsValueSet(rangeBarSeries, MaxMarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, rangeBarSeries, MaxMarkerModelProperty))
						MaxMarkerModel = rangeBarSeries.MaxMarkerModel.CloneModel();
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, MaxMarkerSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, MaxMarkerVisibleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, LabelValueSeparatorProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, rangeBarSeries, LegendValueSeparatorProperty);
			}
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			string valueFormat = pointOptionsContainer.ConstructValueFormat(valueScaleType); 
			string separator = string.Empty;
			if (pointOptionsContainer is LegendPointOptionsContainer)
				return "{" + PatternUtils.Value1Placeholder + valueFormat + "}" + LegendValueSeparator + "{" + PatternUtils.Value2Placeholder + valueFormat + "}";
			return "{" + PatternUtils.ValuePlaceholder + valueFormat + "}";
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Max(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(rangePoint.Value1, rangePoint.Value2);
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			IRangePoint rangePoint = (IRangePoint)point;
			return Math.Min(Math.Abs(rangePoint.Value1), Math.Abs(rangePoint.Value2));
		}
		protected internal override MinMaxValues CalculateMinMaxPointRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram,
			CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			return CrosshairManager.CalculateMinMaxBarRangeValues(point, range, isHorizontalCrosshair, diagram, crosshairPaneInfo.Pane, snapMode);
		}
		protected override SeriesContainer CreateContainer() {
			return new RangeSeriesContainer(this);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value_1 || valueLevel == ValueLevelInternal.Value_2;
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.RangeViewPointPatterns;
		}
		protected internal override bool IsPointItemHidden(SeriesPointItem pointItem) {
			switch (pointItem.ValueLevel) {
				case RangeValueLevel.TwoValues:
					return false;
				case RangeValueLevel.Value1:
					return !MinMarkerVisible;
				case RangeValueLevel.Value2:
					return !MaxMarkerVisible;
			}
			return base.IsPointItemHidden(pointItem);
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class RangeBarValueToStringConverter : RangeSeriesValueToStringConverter {
		readonly RangeBarLabelKind labelKind;
		protected override bool ShouldSortValues { get { return true; } }
		protected override bool IsOneLabel { get { return labelKind == RangeBarLabelKind.OneLabel; } }
		public RangeBarValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, RangeBarLabelKind labelKind, string separator)
			: this(numericOptions, dateTimeOptions, labelKind, false, separator) {
		}
		public RangeBarValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, RangeBarLabelKind labelKind, bool isLegendConverter, string separator)
			: base(numericOptions, dateTimeOptions, isLegendConverter, separator) {
			this.labelKind = labelKind;
		}
		protected override object GetValue(object[] values) {
			switch (labelKind) {
				case RangeBarLabelKind.TwoLabels:
				case RangeBarLabelKind.MinValueLabel:
					return values[0];
				case RangeBarLabelKind.MaxValueLabel:
					return values[1];
				default:
					ChartDebug.Fail("Unknown range bar label kind.");
					return values[0];
			}
		}
	}
	public class RangeBarValue2ToStringConverter : ValueToStringConverter {
		public RangeBarValue2ToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions)
			: base(numericOptions, dateTimeOptions) {
		}
		protected override object GetValue(object[] values) {
			values = RangeSeriesValueToStringConverter.SortValues(values);
			return values[1];
		}
	}
}
