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
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum Bubble2DLabelPosition {
		Center = 0,
		Outside = 1
	}
	public enum BubbleLabelValueToDisplay {
		Weight = 0,
		Value = 1,
		ValueAndWeight = 2
	}
	public class BubbleSeries2D : MarkerSeries2D, ISupportMarker2D, ISupportTransparency, IXYWSeriesView {
		public static readonly DependencyProperty WeightProperty = DependencyPropertyManager.RegisterAttached("Weight",
			typeof(double), typeof(BubbleSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update, CoerceWeight));
		public static readonly DependencyProperty ValueToDisplayProperty = DependencyPropertyManager.RegisterAttached("ValueToDisplay",
			typeof(BubbleLabelValueToDisplay), typeof(BubbleSeries2D), new PropertyMetadata(BubbleLabelValueToDisplay.Weight, PointOptions.ValueToDisplayPropertyChanged));
		public static readonly DependencyProperty LabelPositionProperty = DependencyPropertyManager.RegisterAttached("LabelPosition",
			typeof(Bubble2DLabelPosition), typeof(BubbleSeries2D), new PropertyMetadata(Bubble2DLabelPosition.Center, ChartElementHelper.Update));
		public static readonly DependencyProperty WeightDataMemberProperty = DependencyPropertyManager.Register("WeightDataMember",
			typeof(string), typeof(BubbleSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty MarkerModelProperty = DependencyPropertyManager.Register("MarkerModel",
			typeof(Marker2DModel), typeof(BubbleSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MinSizeProperty = DependencyPropertyManager.Register("MinSize",
			typeof(double), typeof(BubbleSeries2D), new PropertyMetadata(0.3, SizePropertyChanged), ValidateSize);
		public static readonly DependencyProperty MaxSizeProperty = DependencyPropertyManager.Register("MaxSize",
			typeof(double), typeof(BubbleSeries2D), new PropertyMetadata(0.9, SizePropertyChanged), ValidateSize);
		public static readonly DependencyProperty TransparencyProperty = DependencyPropertyManager.Register("Transparency",
			typeof(double), typeof(BubbleSeries2D), new PropertyMetadata(0.0, TransparencyChanged), ValidateTransparency);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Marker2DAnimationBase), typeof(BubbleSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		static bool ValidateSize(object size) {
			return (double)size > 0;
		}
		static void SizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e);
		}
		static bool ValidateTransparency(object transparency) {
			return (double)transparency >= 0 && (double)transparency <= 1;
		}
		static void TransparencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BubbleSeries2D series = d as BubbleSeries2D;
			if (series != null)
				series.UpdatePointDatasOpacity();
		}
		static object CoerceWeight(DependencyObject d, object baseValue) {
			double weight = (double)baseValue;
			if (double.IsInfinity(weight))
				return double.NaN;
			else
				return baseValue;
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetWeight(SeriesPoint point) {
			return (double)point.GetValue(WeightProperty);
		}
		public static void SetWeight(SeriesPoint point, double weight) {
			point.SetValue(WeightProperty, weight);
		}
		[Obsolete(ObsoleteMessages.ValueToDisplayProperty)]
		public static BubbleLabelValueToDisplay GetValueToDisplay(PointOptions options) {
			return (BubbleLabelValueToDisplay)options.GetValue(ValueToDisplayProperty);
		}
		[Obsolete(ObsoleteMessages.ValueToDisplayProperty)]
		public static void SetValueToDisplay(PointOptions options, BubbleLabelValueToDisplay valueToDisplay) {
			options.SetValue(ValueToDisplayProperty, valueToDisplay);
		}
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public static Bubble2DLabelPosition GetLabelPosition(SeriesLabel label) {
			return (Bubble2DLabelPosition)label.GetValue(LabelPositionProperty);
		}
		public static void SetLabelPosition(SeriesLabel label, Bubble2DLabelPosition position) {
			label.SetValue(LabelPositionProperty, position);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DWeightDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string WeightDataMember {
			get { return (string)GetValue(WeightDataMemberProperty); }
			set { SetValue(WeightDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MarkerModel {
			get { return (Marker2DModel)GetValue(MarkerModelProperty); }
			set { SetValue(MarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DMinSize"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MinSize {
			get { return (double)GetValue(MinSizeProperty); }
			set { SetValue(MinSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DMaxSize"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MaxSize {
			get { return (double)GetValue(MaxSizeProperty); }
			set { SetValue(MaxSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DTransparency"),
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
	DevExpressXpfChartsLocalizedDescription("BubbleSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DAnimationBase PointAnimation {
			get { return (Marker2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		protected internal override ResolveOverlappingMode LabelsResolveOverlappingMode {
			get {
				if (BubbleSeries2D.GetLabelPosition(ActualLabel) == Bubble2DLabelPosition.Center)
					if (ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint ||
						ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAroundPoint)
						return ResolveOverlappingMode.Default;
				return ActualLabel.ResolveOverlappingMode;
			}
		}
		protected internal override bool IsLabelConnectorItemVisible { get { return BubbleSeries2D.GetLabelPosition(ActualLabel) == Bubble2DLabelPosition.Outside && ActualLabel.ConnectorVisible; } }
		protected internal override bool ArePointsVisible { get { return true; } }
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipBubbleValueToStringConverter(this); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Hatch; } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.WeightPlaceholder + "}"; } }
		protected override Type PointInterfaceType {
			get { return typeof(IXYWPoint); }
		}
		protected override int PixelsPerArgument { get { return 40; } }
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value || valueLevel == ValueLevelInternal.Weight;
		}
		public BubbleSeries2D() {
			DefaultStyleKey = typeof(BubbleSeries2D);
		}
		#region ISupportMarker2D implementation
		bool ISupportMarker2D.MarkerVisible {
			get { return true; }
			set { throw new InvalidOperationException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyUsage), "MarkerVisible", "BubbleSeries2D")); }
		}
		int ISupportMarker2D.MarkerSize {
			get { throw new InvalidOperationException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyUsage), "MarkerSize", "BubbleSeries2D")); }
			set { throw new InvalidOperationException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyUsage), "MarkerSize", "BubbleSeries2D")); }
		}
		#endregion
		#region IXYWSeriesView implementation
		double IXYWSeriesView.GetSideMargins(double min, double max) {
			return MaxSize;
		}
		#endregion
		void UpdatePointDatasOpacity() {
			foreach (SeriesPointData seriesPointData in Item.SeriesPointDataList) {
				ISeriesPointData pointData = seriesPointData as ISeriesPointData;
				if (pointData != null)
					pointData.Opacity = (Transparency >= 0 && Transparency <= 1.0) ? 1.0 - Transparency : 1.0;
				else
				ChartDebug.Fail("ISeriesPointItem is expected.");
			}
		}
		protected override Series CreateObjectForClone() {
			return new BubbleSeries2D();
		}
		protected override ISeriesPoint CreateSeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			SeriesPoint point = (SeriesPoint)base.CreateSeriesPoint(argument, internalArgument, values, internalValues, tag, hint, color);
			SetWeight(point, (double)values[1]);
			return point;
		}
		protected override IList<string> GetValueDataMembers() {
			return new string[] { ValueDataMember, WeightDataMember };
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BubbleSeries2D bubbleSeries2D = series as BubbleSeries2D;
			if (bubbleSeries2D != null) {
				if (Label != null && bubbleSeries2D.Label != null)
					BubbleSeries2D.SetLabelPosition(Label, BubbleSeries2D.GetLabelPosition(bubbleSeries2D.Label));
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries2D, MaxSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries2D, MinSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries2D, TransparencyProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries2D, WeightDataMemberProperty);
				if (CopyPropertyValueHelper.IsValueSet(bubbleSeries2D, MarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, bubbleSeries2D, MarkerModelProperty))
						MarkerModel = bubbleSeries2D.MarkerModel.CloneModel();
				if (CopyPropertyValueHelper.IsValueSet(bubbleSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, bubbleSeries2D, PointAnimationProperty))
						PointAnimation = bubbleSeries2D.PointAnimation.CloneAnimation() as Marker2DAnimationBase;
			}
		}
		protected internal override SolidColorBrush MixColor(Color color) {
			return ColorUtils.GetTransparentBrush(ColorUtils.MixWithDefaultColor(color), Transparency);
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return ColorUtils.GetNotTransparentBrush(brush);
		}
		protected internal override double[] GetPointValues(SeriesPoint point) {
			return new double[] { point.NonAnimatedValue, GetWeight(point) };
		}
		protected internal override void SetPointValues(SeriesPoint seriesPoint, double[] values, DateTime[] dateTimeValues) {
			base.SetPointValues(seriesPoint, values, dateTimeValues);
			if (values != null && values.Length > 1)
				SetWeight(seriesPoint, values[1]);
		}
		protected internal override double[] GetAnimatedPointValues(SeriesPoint point) {
			return new double[] { point.Value, GetWeight(point) };
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return MarkerModel;
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			Point anchorPoint = mapping.GetRoundedDiagramPoint(labelItem.RefinedPoint.Argument, ((IXYWPoint)labelItem.RefinedPoint).Value);
			anchorPoint = transform.Transform(anchorPoint);
			double markerSize = CalculateMarkerSize(mapping, labelItem.RefinedPoint);
			double halfMarkerSize = markerSize / 2.0;
			if (BubbleSeries2D.GetLabelPosition(labelItem.Label) == Bubble2DLabelPosition.Outside) {
				double angle = MathUtils.Degree2Radian(NormalizeLabelAngle(GetAngle(labelItem.Label)));
				double labelIndent = halfMarkerSize + labelItem.Label.Indent;
				Point labelPoint = new Point(anchorPoint.X + labelIndent * Math.Cos(angle), anchorPoint.Y + labelIndent * Math.Sin(angle));
				if (!mapping.Viewport.Contains(anchorPoint))
					anchorPoint = CorrectLabelLayoutPointWithViewport(mapping.Viewport, anchorPoint, labelPoint);
				if (!mapping.Viewport.Contains(labelPoint))
					labelPoint = CorrectLabelLayoutPointWithViewport(mapping.Viewport, labelPoint, anchorPoint);
				labelIndent = MathUtils.CalcDistance(labelPoint, anchorPoint);
				GRect2D excludedRectangle = GraphicsUtils.ConvertRect(SeriesWithMarkerHelper.CalcAnchorHole(anchorPoint, labelIndent));
				return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, labelIndent, angle, excludedRectangle);
			}
			else {
				Rect validRectangle = MathUtils.StrongRound(new Rect(anchorPoint.X - halfMarkerSize, anchorPoint.Y - halfMarkerSize, markerSize, markerSize));
				return new XYSeriesLabel2DLayout(labelItem, mapping, anchorPoint, GraphicsUtils.ConvertRect(validRectangle));
			}
		}
		Point CorrectLabelLayoutPointWithViewport(Rect viewport, Point startPoint, Point finishPoint) {
			IntersectionInfo intersection = GeometricUtils.CalcLineSegmentWithRectIntersection(new GRealPoint2D(startPoint.X, startPoint.Y),
				new GRealPoint2D(finishPoint.X, finishPoint.Y), new GRealPoint2D(viewport.Left, viewport.Bottom), new GRealPoint2D(viewport.Right, viewport.Top));
			return intersection.HasIntersection ? new Point(intersection.IntersectionPoint.X, intersection.IntersectionPoint.Y) : startPoint;
		}
		protected override int CalculateMarkerSize(PaneMapping mapping, RefinedPoint refinedPoint) {
			const int minSizeInDeviceIndependentUnits = 2;
			return Math.Max(mapping.AxisXMapping.GetRoundedInterval(((IXYWPoint)refinedPoint).Size), minSizeInDeviceIndependentUnits);
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Marker2DWidenAnimation();
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			FillMarkerAnimationKinds(animationKinds);
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IXYWPoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IXYWPoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYWPoint)point).Value);
		}
		protected override SeriesContainer CreateContainer() {
			return new XYWSeriesContainer(this);
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			BubbleLabelValueToDisplay valueToDisplay = (BubbleLabelValueToDisplay)pointOptionsContainer.PointOptions.GetValue(ValueToDisplayProperty);
			string textPattern = pointOptionsContainer.ConstructValuePattern(valueScaleType);
			switch (valueToDisplay) {
				case BubbleLabelValueToDisplay.Value:
					break;
				case BubbleLabelValueToDisplay.Weight:
					textPattern = PatternUtils.ReplacePlaceholder(textPattern, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
					break;
				case BubbleLabelValueToDisplay.ValueAndWeight:
					textPattern = PatternUtils.ReplacePlaceholder(textPattern, PatternUtils.ValuePlaceholder, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
					break;
			}
			return textPattern;
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.BubbleViewPointPatterns;
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Marker2DAnimationBase))
				return;
			PointAnimation = value as Marker2DAnimationBase;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class BubbleValueToStringConverter : ValueToStringConverter {
		public const string Separator = ", ";
		BubbleLabelValueToDisplay valueToDisplay;
		public BubbleValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions,
			BubbleLabelValueToDisplay valueToDisplay)
			: base(numericOptions, dateTimeOptions) {
			this.valueToDisplay = valueToDisplay;
		}
		public override string ConvertTo(object[] values) {
			switch (valueToDisplay) {
				case BubbleLabelValueToDisplay.Value:
					return GetValueText(values[0]);
				case BubbleLabelValueToDisplay.Weight:
					return GetValueText(values[1]);
				case BubbleLabelValueToDisplay.ValueAndWeight:
					return GetValueText(values[0]) + Separator + GetValueText(values[1]);
				default:
					ChartDebug.Fail("Unknown BubbleLabelValueToDisplay value.");
					return string.Empty;
			}
		}
	}
}
