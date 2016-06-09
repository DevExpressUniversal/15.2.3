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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum PieSweepDirection {
		Clockwise = PointsSweepDirection.Clockwise,
		Counterclockwise = PointsSweepDirection.Counterclockwise,
	}
	public class PieSeries2D : PieSeries {
		#region Dependency properties
		public static readonly DependencyProperty RotationProperty = DependencyPropertyManager.Register("Rotation",
			typeof(double), typeof(PieSeries2D), new FrameworkPropertyMetadata(0.0, ChartElementHelper.Update), new ValidateValueCallback(RotationValidation));
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(Pie2DModel), typeof(PieSeries2D), new FrameworkPropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty SweepDirectionProperty = DependencyPropertyManager.Register("SweepDirection",
			typeof(PieSweepDirection), typeof(PieSeries2D), new FrameworkPropertyMetadata(PieSweepDirection.Counterclockwise, ChartElementHelper.Update));
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Pie2DSeriesPointAnimationBase), typeof(PieSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		public static readonly DependencyProperty SeriesAnimationProperty = DependencyPropertyManager.Register("SeriesAnimation",
			typeof(Pie2DSeriesAnimationBase), typeof(PieSeries2D), new PropertyMetadata(null, SeriesAnimationPropertyChanged));
		#endregion
		static bool RotationValidation(object rotation) {
			return !Double.IsNaN((double)rotation) && !Double.IsInfinity((double)rotation);
		}
		Rect viewport;
		protected override bool Is3DView { 
			get { return false; } 
		}
		internal Rect Viewport { 
			get { return viewport; } 
			set { viewport = value; } 
		}
		internal PieSeries2DPresentation Presentation {
			get;
			set;
		}
		internal double ActualRotation { get { return SweepDirection == PieSweepDirection.Counterclockwise ? Rotation : -Rotation; } }
		protected internal override Type SupportedDiagramType { 
			get { return typeof(SimpleDiagram2D); } 
		}
		protected internal override FadeInMode AutoFadeInMode { 
			get { return GetActualSeriesAnimation() != null ? FadeInMode.Labels : FadeInMode.PointsAndLabels; } 
		}
		protected internal override VisualSelectionType SupportedSelectionType { 
			get { return VisualSelectionType.Hatch; } 
		}
		protected internal override bool ArePointsVisible {
			get { return true; } 
		}
		protected internal override bool IsLabelConnectorItemVisible {
			get { return PieSeries.GetLabelPosition(ActualLabel) != PieLabelPosition.Inside ? ActualLabel.ConnectorVisible : false; }
		}
		protected internal Panel PointsPanel {
			get;
			set;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries2DRotation"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double Rotation {
			get { return (double)GetValue(RotationProperty); }
			set { SetValue(RotationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries2DModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Pie2DModel Model {
			get { return (Pie2DModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries2DSweepDirection"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public PieSweepDirection SweepDirection {
			get { return (PieSweepDirection)GetValue(SweepDirectionProperty); }
			set { SetValue(SweepDirectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Pie2DSeriesPointAnimationBase PointAnimation {
			get { return (Pie2DSeriesPointAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PieSeries2DSeriesAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Pie2DSeriesAnimationBase SeriesAnimation {
			get { return (Pie2DSeriesAnimationBase)GetValue(SeriesAnimationProperty); }
			set { SetValue(SeriesAnimationProperty, value); }
		}
		public PieSeries2D() {
			DefaultStyleKey = typeof(PieSeries2D);
		}
		double CalculateNonExplodedPieRadius(double maxRadius) {
			double maxExplodedFraction = GetMaxExplodedDistance();
			return CalculateNonExplodedPieRadius(maxRadius, maxExplodedFraction);
		}
		double CalculateNonExplodedPieRadius(double maxRadius, double maxExplodedFraction) {
			double radius = Math.Floor(maxRadius / (1 + maxExplodedFraction));
			return radius > 0 ? radius : 0;
		}
		protected override bool ProcessChanging(ChartUpdate updateInfo) {
			return base.ProcessChanging(updateInfo) && (updateInfo.Change & ChartElementChange.Diagram3DOnly) == 0;
		}
		protected override Series CreateObjectForClone() {
			return new PieSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			PieSeries2D pieSeries2D = series as PieSeries2D;
			if (pieSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, pieSeries2D, RotationProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, pieSeries2D, SweepDirectionProperty);
				if (CopyPropertyValueHelper.IsValueSet(pieSeries2D, ModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pieSeries2D, ModelProperty))
						Model = pieSeries2D.Model.CloneModel();
				if (CopyPropertyValueHelper.IsValueSet(pieSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pieSeries2D, PointAnimationProperty))
						PointAnimation = pieSeries2D.PointAnimation.CloneAnimation() as Pie2DSeriesPointAnimationBase;
				if (CopyPropertyValueHelper.IsValueSet(pieSeries2D, SeriesAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pieSeries2D, SeriesAnimationProperty))
						SeriesAnimation = pieSeries2D.SeriesAnimation.CloneAnimation() as Pie2DSeriesAnimationBase;
			}
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Pie2DGrowUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DPopUpAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DDropInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DFlyInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DWidenAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DBurstAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DFadeInAnimation)));
		}
		protected override void FillPredefinedSeriesAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Pie2DFanAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DFanZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DSpinAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DSpinZoomInAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Pie2DZoomInAnimation)));
			base.FillPredefinedSeriesAnimationKinds(animationKinds);
		}
		protected override void SeriesProgressChanged() {
			Pie2DSeriesAnimationBase animation = GetActualSeriesAnimation() as Pie2DSeriesAnimationBase;
			if (animation != null && PointsPanel != null) {
				PointsPanel.InvalidateMeasure();
			}
		}
		protected internal virtual void CreateSeriesPointsLayout(Point centerPie, double maxRadius) {
			if (Diagram == null) {
				ChartDebug.Fail("Can't create series points layout because the Diagram is null.");
				return;
			}
			IRefinedSeries refinedSeries = Diagram.ViewController.GetRefinedSeries(this);
			if (refinedSeries == null)
				return;
			centerPie = MathUtils.StrongRound(centerPie);
			double maxRadiusRounded = MathUtils.StrongRound(CalculateNonExplodedPieRadius(maxRadius));
			Pie2DSeriesAnimationBase animation = SeriesAnimation as Pie2DSeriesAnimationBase;
			double rotation = ActualRotation;
			double sweepAngle = 360.0;
			if (animation != null) {
				maxRadiusRounded = MathUtils.StrongRound(animation.CalculateAnimatedRadius(maxRadiusRounded, SeriesProgress.ActualProgress));
				rotation = animation.CalculateAnimatedRotation(rotation, SeriesProgress.ActualProgress);
				sweepAngle = animation.CalculateAnimatedSweepAngle(sweepAngle, SeriesProgress.ActualProgress);
			}
			Pie pie = new Pie(refinedSeries, SweepDirection, sweepAngle);
			foreach (SeriesPointItem pointItem in Item.AllPointItems) {
				ISeriesPoint seriesPoint = pointItem.RefinedPoint != null ? pointItem.RefinedPoint.SeriesPoint : null;
				Slice slice = seriesPoint != null ? pie[seriesPoint] : null;
				if (slice != null && !slice.IsEmpty) {
					double explodedDistance = PieSeries.GetExplodedDistance(SeriesPoint.GetSeriesPoint(seriesPoint)) * maxRadiusRounded;					
					Point point = centerPie;
					Point clipCenterPoint = point;
					if (explodedDistance > 0) {
						point = centerPie.MoveByAngle(explodedDistance, slice.MedianAngle - ActualRotation);
						point = MathUtils.StrongRound(point);
						clipCenterPoint = centerPie.MoveByAngle(explodedDistance, slice.MedianAngle - ActualRotation);
					}
					pointItem.Layout = new PieSeries2DPointLayout(viewport, point, maxRadiusRounded, HoleRadiusPercent, rotation, slice.StartAngle, slice.FinishAngle, clipCenterPoint);
				}
				else
					pointItem.Layout = null;
			}
		}
		protected internal virtual PieSeriesLabel2DLayout CreateSeriesLabelLayout(Point centerPie, double maxRadius, Rect labelBounds, SeriesLabelItem labelItem) {
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(labelItem.PointItem.RefinedPoint.SeriesPoint);
			if (seriesPoint == null || labelItem.PointItem.RefinedPoint.IsEmpty)
				return null;
			double radius = MathUtils.StrongRound(CalculateNonExplodedPieRadius(maxRadius));
			Pie pie = new Pie(Diagram.ViewController.GetRefinedSeries(this), SweepDirection);
			Slice slice = pie[seriesPoint];
			if (slice == null || slice.IsEmpty)
				return null;
			double angle = MathUtils.NormalizeDegree(slice.MedianAngle - ActualRotation);
			switch (PieSeries.GetLabelPosition(ActualLabel)) {
				case PieLabelPosition.Outside:
					return new PieSeriesLabel2DLayout(labelItem, centerPie.MoveByAngle(radius * (1 + PieSeries.GetExplodedDistance(seriesPoint)), angle), ActualLabel.Indent, MathUtils.Degree2Radian(angle));
				case PieLabelPosition.Inside:
					return CalculateLabelLayotForInsidePosition(centerPie, labelItem, seriesPoint, radius, angle, maxRadius);
				case PieLabelPosition.TwoColumns: {
						Point anchorPoint = centerPie.MoveByAngle(radius * (1 + PieSeries.GetExplodedDistance(seriesPoint)), angle);
						Point endPoint = centerPie.MoveByAngle(ActualLabel.Indent + radius * (1 + PieSeries.GetExplodedDistance(seriesPoint)), angle);
						if (Math.Cos(MathUtils.Degree2Radian(angle)) >= 0)
							return new PieSeriesLabel2DLayout(labelItem, anchorPoint, new Point(labelBounds.Right - labelItem.LabelSize.Width * 0.5, endPoint.Y), angle);
						else
							return new PieSeriesLabel2DLayout(labelItem, anchorPoint, new Point(labelBounds.Left + labelItem.LabelSize.Width * 0.5, endPoint.Y), angle);
					}
				default:
					goto case PieLabelPosition.Outside;
			}
		}
		protected virtual PieSeriesLabel2DLayout CalculateLabelLayotForInsidePosition(Point centerPie, SeriesLabelItem labelItem, SeriesPoint seriesPoint, double radius, double angle, double halfOfMinSideOfAvailableSpace) {
			double holeRadius = radius * HoleRadiusPercent / 100;
			Point centerPoint = centerPie.MoveByAngle((radius - holeRadius) / 2 + holeRadius + radius * PieSeries.GetExplodedDistance(seriesPoint), angle);
			return new PieSeriesLabel2DLayout(labelItem, centerPoint, centerPoint, angle);
		}
		protected internal Point CalculateToolTipPoint(SeriesPointItem pointItem) {
			Point location = new Point();
			PieSeries2DPointLayout pointLayout = pointItem.Layout as PieSeries2DPointLayout;
			if (pointLayout != null) {
				double centerAngle = (pointLayout.InitialFinishAngle + pointLayout.InitialStartAngle) / 2.0 - pointLayout.Rotation;
				Rect pieRect = LayoutHelper.GetRelativeElementRect(pointItem.PointItemPresentation, pointItem.Series.Diagram.ChartControl);
				location = new Point(pieRect.Width / 2.0, pieRect.Height / 2.0).MoveByAngle(pointLayout.InitialRadius, centerAngle);
				location.X += pieRect.Left;
				location.Y += pieRect.Top;
			}
			return location;
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			PieSeries2DPointLayout layout = pointItem.Layout as PieSeries2DPointLayout;
			if (layout != null) {
				Pie2DSeriesPointAnimationBase animation = GetActualPointAnimation() as Pie2DSeriesPointAnimationBase;
				Point center = layout.InitialPieCenter;
				double radius = layout.InitialRadius;
				double midleAngle = 0.5 * (layout.InitialStartAngle + layout.InitialFinishAngle);
				double sweepAngle = Math.Abs(layout.InitialFinishAngle - layout.InitialStartAngle);
				if (animation != null) {
					double progress = pointItem.PointProgress.ActualProgress;
					center = animation.CalculateAnimatedPieCenter(center, layout.InitialRadius, MathUtils.NormalizeDegree(midleAngle - layout.Rotation), layout.Viewport, progress);
					radius = MathUtils.StrongRound(animation.CalculateAnimatedRadius(radius, layout.Viewport, progress));
					sweepAngle = animation.CalculateAnimatedSweepAngle(sweepAngle, progress);
				}
				layout.Complete(center, radius, midleAngle + 0.5 * sweepAngle, midleAngle - 0.5 * sweepAngle);
			}
		}
		protected internal override List<SeriesPointItem> GetPointItemsForLabels() {
			List<SeriesPointItem> pointItems = new List<SeriesPointItem>();
			if (LabelsVisibility)
				foreach (SeriesPointItem pointItem in Item.AllPointItems)
					if (!string.IsNullOrEmpty(pointItem.PresentationData.LabelText))
						pointItems.Add(pointItem);
			return pointItems;
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return Model;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Pie2DFlyInAnimation();
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Pie2DSeriesPointAnimationBase))
				return;
			PointAnimation = value as Pie2DSeriesPointAnimationBase;
		}
		public override SeriesAnimationBase GetSeriesAnimation() { return SeriesAnimation; }
		public override void SetSeriesAnimation(SeriesAnimationBase value) {
			if (value != null && !(value is Pie2DSeriesAnimationBase))
				return;
			SeriesAnimation = value as Pie2DSeriesAnimationBase;
		}
	}
}
