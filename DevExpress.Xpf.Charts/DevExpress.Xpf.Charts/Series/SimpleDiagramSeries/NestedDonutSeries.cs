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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class NestedDonutSeries2D : PieSeries2D, ISupportSeriesGroups, INestedDoughnutSeriesView {
		#region Nested class: DonutLayoutCalculator
		class DonutLayoutCalculator {
			const double RoundAngle = 360.0;
			IRefinedSeries RefinedSeries { get; set; }
			NestedDonutSeries2D Series { get; set; }
			INestedDoughnutRefinedSeries NestedDonutRefinedSeries { get; set; }
			internal double OuterRadiusOfOuterDonutPx { get; private set; }
			internal double OuterRadiusPx { get; private set; }
			internal double InnerRadiusPx { get; private set; }
			internal bool CalculationSuccess { get; private set; }
			internal Pie Pie { get; private set; }
			internal double RotationCcwDeg { get; private set; }
			public DonutLayoutCalculator(IRefinedSeries refinedSeries, bool isOuterDonut, double halfOfMinSideOfAvailableSpace, bool useSeriesAnimation = true) {
				RefinedSeries = refinedSeries;
				NestedDonutRefinedSeries = (INestedDoughnutRefinedSeries)refinedSeries;
				Series = (NestedDonutSeries2D)refinedSeries.Series;
				Calculate(isOuterDonut, halfOfMinSideOfAvailableSpace, useSeriesAnimation);
			}
			void Calculate(bool isOuterDonut, double halfOfMinSideOfAvailableSpace, bool useSeriesAnimation) {
				OuterRadiusOfOuterDonutPx = CalculateNonExplodedRadiusOfOuterDonut(halfOfMinSideOfAvailableSpace, NestedDonutRefinedSeries.ExplodedFactor);
				double holeRadiusPx = NestedDonutRefinedSeries.HoleRadius * OuterRadiusOfOuterDonutPx;
				double sumOfDonutsWidthsPx = OuterRadiusOfOuterDonutPx - NestedDonutRefinedSeries.TotalGroupIndentInPixels - holeRadiusPx;
				if (isOuterDonut)
					OuterRadiusPx = OuterRadiusOfOuterDonutPx;
				else
					OuterRadiusPx = OuterRadiusOfOuterDonutPx - NestedDonutRefinedSeries.StartOffset * sumOfDonutsWidthsPx - NestedDonutRefinedSeries.StartOffsetInPixels;
				double donutWidthPx = MathUtils.IsNumericDouble(NestedDonutRefinedSeries.NormalizedWeight) ? NestedDonutRefinedSeries.NormalizedWeight * sumOfDonutsWidthsPx : 0.0;
				double radiusDiff = OuterRadiusPx - donutWidthPx;
				InnerRadiusPx = radiusDiff < 0 ? 0 : radiusDiff; 
				CalculationSuccess = InnerRadiusPx >= 0 && (OuterRadiusPx - InnerRadiusPx) > 1 && OuterRadiusOfOuterDonutPx >= OuterRadiusPx;
				var seriesAnimation = (Pie2DSeriesAnimationBase)Series.GetActualSeriesAnimation();
				if (seriesAnimation != null && CalculationSuccess && useSeriesAnimation)
					CalculateAnimatedValues(seriesAnimation);
				else {
					Pie = new Pie(RefinedSeries, Series.SweepDirection, RoundAngle);
					RotationCcwDeg = Series.ActualRotation;
				}
			}
			double CalculateNonExplodedRadiusOfOuterDonut(double maxPossibleRadius, double maxExplodedFraction) {
				double radius = maxPossibleRadius / (1 + maxExplodedFraction);
				return radius > 0 ? radius : 0;
			}
			void CalculateAnimatedValues(Pie2DSeriesAnimationBase seriesAnimation) {
				double animationProgress = Series.SeriesProgress.Progress;
				OuterRadiusOfOuterDonutPx = seriesAnimation.CalculateAnimatedRadius(OuterRadiusOfOuterDonutPx, animationProgress);
				OuterRadiusPx = seriesAnimation.CalculateAnimatedRadius(OuterRadiusPx, animationProgress);
				InnerRadiusPx = seriesAnimation.CalculateAnimatedRadius(InnerRadiusPx, animationProgress);
				double sweepAngleDeg = seriesAnimation.CalculateAnimatedSweepAngle(RoundAngle, animationProgress);
				Pie = new Pie(RefinedSeries, Series.SweepDirection, sweepAngleDeg);
				RotationCcwDeg = seriesAnimation.CalculateAnimatedRotation(Series.ActualRotation, animationProgress);
			}
		}
		#endregion
		public const double DefaultInnerIndent = 5.0;
		public const double DefaultWeight = 1.0;
		#region Dependency Properties
		public static readonly DependencyProperty GroupProperty =
			DependencyPropertyManager.Register("Group", typeof(object), typeof(NestedDonutSeries2D), new PropertyMetadata(GroupPropertyChanged));
		public static readonly DependencyProperty InnerIndentProperty =
			DependencyPropertyManager.Register("InnerIndent", typeof(double), typeof(NestedDonutSeries2D), new PropertyMetadata(DefaultInnerIndent, InnerIndentPropertyChanged, CoerceInnerIndent));
		public static readonly DependencyProperty WeightProperty =
			DependencyPropertyManager.Register("Weight", typeof(double), typeof(NestedDonutSeries2D), new PropertyMetadata(DefaultWeight, WeightPropertyChanged, CoerceWeight));
		#endregion
		static void GroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "Group"));
		}
		static void InnerIndentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "InnerIndent"));
		}
		static object CoerceInnerIndent(DependencyObject d, object baseValue) {
			var indent = (double)baseValue;
			if (indent < 0 || !MathUtils.IsNumericDouble(indent))
				return 0.0;
			else
				return indent;
		}
		static void WeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "Weight"));
		}
		static object CoerceWeight(DependencyObject d, object baseValue) {
			var weight = (double)baseValue;
			if (weight < 0 || !MathUtils.IsNumericDouble(weight))
				return 0.0;
			else
				return weight;
		}
		bool? isOutside = null;
		protected override bool NeedSeriesGroupsInteraction {
			get { return true; }
		}
		protected internal bool? IsOuter { get { return isOutside; } }
		[
		Category(Categories.Behavior),
		TypeConverter(typeof(ObjectTypeConverter)),
		XtraSerializableProperty]
		public object Group {
			get { return GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public double InnerIndent {
			get { return (double)GetValue(InnerIndentProperty); }
			set { SetValue(InnerIndentProperty, value); }
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty]
		public double Weight {
			get { return (double)GetValue(WeightProperty); }
			set { SetValue(WeightProperty, value); }
		}
		public NestedDonutSeries2D() {
			DefaultStyleKey = typeof(NestedDonutSeries2D);
		}
		#region ISupportSeriesGroups implementation
		object ISupportSeriesGroups.SeriesGroup {
			get { return GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		#endregion
		#region INestedDoughnutSeriesView implementation
		double INestedDoughnutSeriesView.ExplodedDistancePercentage {
			get { return FindMaximumExplodedDistanceFraction() * 100.0; }
		}
		double INestedDoughnutSeriesView.HoleRadiusPercent {
			get { return HoleRadiusPercent; }
		}
		double INestedDoughnutSeriesView.InnerIndent {
			get { return InnerIndent; }
		}
		bool? INestedDoughnutSeriesView.IsOutside {
			get { return isOutside; }
			set { isOutside = value; }
		}
		double INestedDoughnutSeriesView.Weight {
			get { return Weight; }
		}
		bool INestedDoughnutSeriesView.HasExplodedPoints(IRefinedSeries refinedSeries) {
			double maxExplodedDistanceToRadiusFraction = FindMaximumExplodedDistanceFraction();
			return maxExplodedDistanceToRadiusFraction != 0.0;
		}
		#endregion
		double FindMaximumExplodedDistanceFraction() {
			double maxExplodedDistance = 0.0;
			foreach (SeriesPoint sp in Points) {
				double currExplDist = PieSeries.GetExplodedDistance(sp);
				if (maxExplodedDistance < currExplDist)
					maxExplodedDistance = currExplDist;
			}
			return maxExplodedDistance;
		}
		void SetNullLayoutForPointItems() {
			foreach (SeriesPointItem pointItem in Item.AllPointItems)
				pointItem.Layout = null;
		}
		protected override Series CreateObjectForClone() {
			return new NestedDonutSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			NestedDonutSeries2D nestedDonutSeries2D = series as NestedDonutSeries2D;
			if (nestedDonutSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, nestedDonutSeries2D, GroupProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, nestedDonutSeries2D, InnerIndentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, nestedDonutSeries2D, WeightProperty);
			}
		}
		protected override SeriesInteractionContainer CreateSeriesGroupsContainer() {
			return new NestedDoughnutInteractionContainer(this);
		}
		protected override void SeriesProgressChanged() {
			if (GetActualSeriesAnimation() != null && PointsPanel != null)
				PointsPanel.InvalidateMeasure();
		}
		protected internal override void CreateSeriesPointsLayout(Point donutCenter, double halfOfMinSideOfAvailableSpace) {
			if (!isOutside.HasValue)
				throw new InvalidOperationException("Core calculations must have been performed before. IsOuter status must be known.");
			DonutLayoutCalculator calc = new DonutLayoutCalculator(Item.RefinedSeries, isOutside.Value, halfOfMinSideOfAvailableSpace);
			if (!calc.CalculationSuccess) {
				SetNullLayoutForPointItems();
				return;
			}
			foreach (SeriesPointItem pointItem in Item.AllPointItems) {
				ISeriesPoint seriesPoint = pointItem.RefinedPoint != null ? pointItem.RefinedPoint.SeriesPoint : null;
				Slice slice = seriesPoint != null ? calc.Pie[seriesPoint] : null;
				if (slice == null || slice.IsEmpty) {
					pointItem.Layout = null;
					continue;
				}				
				Point centerOfSegment = donutCenter;
				if (isOutside.Value && Points.Count > 1) {
					double explodedDistancePx = PieSeries.GetExplodedDistance((SeriesPoint)seriesPoint) * calc.OuterRadiusPx;
					if (explodedDistancePx > 0)
						centerOfSegment = donutCenter.MoveByAngle(explodedDistancePx, slice.MedianAngle - calc.RotationCcwDeg);
				}
				Point relativeDonutCenter = new Point(calc.OuterRadiusOfOuterDonutPx, calc.OuterRadiusOfOuterDonutPx);
				DonutSegment segment = new DonutSegment(centerOfSegment, relativeDonutCenter, calc.InnerRadiusPx, calc.OuterRadiusPx, slice, SweepDirection);
				segment.RotateCCW(calc.RotationCcwDeg);
				pointItem.Layout = new NestedDonutSeries2DPointLayout(Viewport, segment, calc.OuterRadiusOfOuterDonutPx);
			}
		}
		protected internal override PieSeriesLabel2DLayout CreateSeriesLabelLayout(Point donutCenter, double halfOfMinSideOfAvailableSpace, Rect labelBounds, SeriesLabelItem labelItem) {
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(labelItem.PointItem.RefinedPoint.SeriesPoint);
			if (seriesPoint == null || labelItem.PointItem.RefinedPoint.IsEmpty)
				return null;
			if (!isOutside.HasValue)
				throw new InvalidOperationException("Core calculation should be performed before. IsOutside status must be known.");
			DonutLayoutCalculator calculator = new DonutLayoutCalculator(Item.RefinedSeries, isOutside.Value, halfOfMinSideOfAvailableSpace, useSeriesAnimation: false);
			if (!calculator.CalculationSuccess)
				return null;
			if (isOutside.Value == true)
				return base.CreateSeriesLabelLayout(donutCenter, halfOfMinSideOfAvailableSpace, labelBounds, labelItem);
			Pie pie = new Pie(Item.RefinedSeries, SweepDirection, 360.0);
			Slice slice = pie[seriesPoint];
			if (slice == null || slice.IsEmpty)
				return null;
			double angle = slice.MedianAngle - ActualRotation;
			double distance = (calculator.OuterRadiusPx - calculator.InnerRadiusPx) / 2 + calculator.InnerRadiusPx;
			Point anchorPoint = donutCenter.MoveByAngle(distance, angle);
			return new PieSeriesLabel2DLayout(labelItem, anchorPoint, anchorPoint, angle);
		}
		protected override PieSeriesLabel2DLayout CalculateLabelLayotForInsidePosition(Point centerPie, SeriesLabelItem labelItem, SeriesPoint seriesPoint, double radius, double angle, double halfOfMinSideOfAvailableSpace) {
			DonutLayoutCalculator calc = new DonutLayoutCalculator(Item.RefinedSeries, isOutside.Value, halfOfMinSideOfAvailableSpace);
			Point centerPoint = centerPie.MoveByAngle((radius - calc.InnerRadiusPx) / 2 + calc.InnerRadiusPx + radius * PieSeries.GetExplodedDistance(seriesPoint), angle);
			return new PieSeriesLabel2DLayout(labelItem, centerPoint, centerPoint, angle);
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			var layout = (NestedDonutSeries2DPointLayout)pointItem.Layout;
			if (layout == null)
				return;
			Pie2DSeriesPointAnimationBase animation = (Pie2DSeriesPointAnimationBase)GetActualPointAnimation();
			if (animation == null)
				return;
			double progress = pointItem.PointProgress.ActualProgress;
			Point center = animation.CalculateAnimatedPieCenter(layout.FinalCenter, layout.FinalOuterRadius, layout.FinalMedianAngleDeg, layout.Viewport, progress);
			double innerRadius = animation.CalculateAnimatedRadius(layout.FinalInnerRadius, layout.Viewport, progress);
			double outerRadius = animation.CalculateAnimatedRadius(layout.FinalOuterRadius, layout.Viewport, progress);
			double animatedOuterRadiusOfOuterDonut = animation.CalculateAnimatedRadius(layout.FinalOuterRadiusOfOuterDonut, layout.Viewport, progress);
			double arcAngle = animation.CalculateAnimatedSweepAngle(layout.ArcAngleDeg, progress);
			double startAngleDeg = layout.FinalMedianAngleDeg + 0.5 * arcAngle;
			double endAngleDeg = layout.FinalMedianAngleDeg - 0.5 * arcAngle;
			Point relativeCenter = new Point(animatedOuterRadiusOfOuterDonut, animatedOuterRadiusOfOuterDonut);
			DonutSegment animatedDonutSegment = new DonutSegment(center, relativeCenter, innerRadius, outerRadius, startAngleDeg, endAngleDeg);
			layout.ChangeDuringPointAnimation(animatedDonutSegment, animatedOuterRadiusOfOuterDonut);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.FullStackedGroupViewPointPatterns;
		}
	}
}
