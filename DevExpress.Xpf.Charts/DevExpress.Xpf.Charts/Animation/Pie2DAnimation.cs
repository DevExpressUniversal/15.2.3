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
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Pie2DSeriesPointAnimationBase : SeriesPointAnimationBase {
		public virtual double CalculateAnimatedRadius(double radius, Rect viewport, double progress) { return radius; }
		public virtual double CalculateAnimatedSweepAngle(double sweepAngle, double progress) { return sweepAngle; }
		public virtual Point CalculateAnimatedPieCenter(Point pieCenter, double radius, double angle, Rect viewport, double progress) { return pieCenter; }
	}
	public class Pie2DGrowUpAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Grow Up"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DGrowUpAnimation();
		}
		public override double CalculateAnimatedRadius(double radius, Rect viewport, double progress) {
			return radius * progress;
		}
	}
	public class Pie2DPopUpAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Pop Up"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DPopUpAnimation();
		}
		double CorrectProgress(double progress) {
			return progress < 0.5 ? progress : 1 - progress;
		}
		public override double CalculateAnimatedRadius(double radius, Rect viewport, double progress) {
			return radius + 0.3 * radius * CorrectProgress(progress);
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			return sweepAngle + 0.2 * sweepAngle * CorrectProgress(progress);
		}
	}
	public class Pie2DDropInAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Drop In"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DDropInAnimation();
		}
		public override Point CalculateAnimatedPieCenter(Point pieCenter, double radius, double angle, Rect viewport, double progress) {
			double animatedRadius = 0.5 * (radius + Math.Sqrt(viewport.Height * viewport.Height + viewport.Width * viewport.Width)) * (1.0 - progress);
			Point result = new Point(pieCenter.X + Math.Cos(MathUtils.Degree2Radian(angle)) * animatedRadius,
				pieCenter.Y + Math.Sin(MathUtils.Degree2Radian(angle)) * animatedRadius);
			return result;
		}
	}
	public class Pie2DWidenAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Widen"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DWidenAnimation();
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			return sweepAngle * progress;
		}
	}
	public class Pie2DFlyInAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Fly In"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DFlyInAnimation();
		}
		public override Point CalculateAnimatedPieCenter(Point pieCenter, double radius, double angle, Rect viewport, double progress) {
			double correctedProgress = progress > 0.5 ? 1 : progress * 2;
			double animatedRadius = 0.5 * (radius + Math.Sqrt(viewport.Height * viewport.Height + viewport.Width * viewport.Width)) * (1.0 - correctedProgress);
			Point result = new Point(pieCenter.X + Math.Cos(MathUtils.Degree2Radian(angle)) * animatedRadius,
				pieCenter.Y + Math.Sin(MathUtils.Degree2Radian(angle)) * animatedRadius);
			return result;
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			if (progress < 0.5)
				return 1;
			return sweepAngle * (progress - 0.5) * 2;
		}
	}
	public class Pie2DBurstAnimation : Pie2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Burst"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DBurstAnimation();
		}
		public override double CalculateAnimatedRadius(double radius, Rect viewport, double progress) {
			if (progress > 0.5)
				return radius;
			return radius * progress * 2;
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			if (progress < 0.5)
				return 1;
			return sweepAngle * (progress - 0.5) * 2;
		}
	}
	public class Pie2DFadeInAnimation : Pie2DSeriesPointAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(Pie2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Pie2DFadeInAnimationMode"),
#endif
		Category(Categories.Behavior)
		]
		public FadeInMode Mode {
			get { return (FadeInMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}
		protected internal override string AnimationName { get { return "Fade In"; } }
		protected internal override bool ShouldAnimateSeriesPointOpacity(Series series) {
			return FadeInAnimationHelper.ShouldAnimateSeriesPointOpacity(series, Mode);
		}
		protected internal override bool ShouldAnimateSeriesLabelOpacity(Series series) {
			return FadeInAnimationHelper.ShouldAnimateSeriesLabelOpacity(series, Mode);
		}
		protected internal override bool ShouldAnimateSeriesPointLayout { get { return false; } }
		protected override double LabelDurationPortion { get { return 1.0; } }
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DFadeInAnimation();
		}
	}
	public abstract class Pie2DSeriesAnimationBase : SeriesAnimationBase {
		public virtual double CalculateAnimatedRadius(double radius, double progress) { return radius; }
		public virtual double CalculateAnimatedSweepAngle(double sweepAngle, double progress) { return sweepAngle; }
		public virtual double CalculateAnimatedRotation(double rotation, double progress) { return rotation; }
	}
	public class Pie2DZoomInAnimation : Pie2DSeriesAnimationBase {
		protected internal override string AnimationName { get { return "Zoom In"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DZoomInAnimation();
		}
		public override double CalculateAnimatedRadius(double radius, double progress) {
			return radius * progress;
		}
	}
	public class Pie2DFanAnimation : Pie2DSeriesAnimationBase {
		protected internal override string AnimationName { get { return "Fan"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DFanAnimation();
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			return sweepAngle * progress;
		}
	}
	public class Pie2DFanZoomInAnimation : Pie2DSeriesAnimationBase {
		protected internal override string AnimationName { get { return "Fan Zoom In"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 4, EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DFanZoomInAnimation();
		}
		public override double CalculateAnimatedSweepAngle(double sweepAngle, double progress) {
			return sweepAngle * progress;
		}
		public override double CalculateAnimatedRadius(double radius, double progress) {
			return radius * progress;
		}
	}
	public class Pie2DSpinAnimation : Pie2DSeriesAnimationBase {
		public static readonly DependencyProperty DirectionProperty = DependencyPropertyManager.Register("Direction",
			typeof(PieSweepDirection), typeof(Pie2DSpinAnimation), new FrameworkPropertyMetadata(PieSweepDirection.Counterclockwise, NotifyPropertyChanged));
		public static readonly DependencyProperty TurnsNumberProperty = DependencyPropertyManager.Register("TurnsNumber",
			typeof(double), typeof(Pie2DSpinAnimation), new FrameworkPropertyMetadata(1.5, NotifyPropertyChanged), ValidateTurnsNumber);
		static bool ValidateTurnsNumber(object turnsNumber) {
			return (double)turnsNumber > 0;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Pie2DSpinAnimationDirection"),
#endif
		Category(Categories.Behavior)
		]
		public PieSweepDirection Direction {
			get { return (PieSweepDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Pie2DSpinAnimationTurnsNumber"),
#endif
		Category(Categories.Behavior)
		]
		public double TurnsNumber {
			get { return (double)GetValue(TurnsNumberProperty); }
			set { SetValue(TurnsNumberProperty, value); }
		}
		protected internal override string AnimationName { get { return "Spin"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut };
		}
		protected override void Assign(AnimationBase animation) {
			base.Assign(animation);
			Pie2DSpinAnimation spinAnimation = animation as Pie2DSpinAnimation;
			if (spinAnimation != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, spinAnimation, DirectionProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, spinAnimation, TurnsNumberProperty);
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new Pie2DSpinAnimation();
		}
		public override double CalculateAnimatedRotation(double rotation, double progress) {
			double offset = TurnsNumber * 360.0 * (1 - progress);
			return Direction == PieSweepDirection.Counterclockwise ? rotation -  offset: rotation + offset;
		}
	}
	public class Pie2DSpinZoomInAnimation : Pie2DSpinAnimation {
		protected internal override string AnimationName { get { return "Spin Zoom In"; } }
		public override double CalculateAnimatedRadius(double radius, double progress) {
			return radius * progress;
		}		
	}
}
