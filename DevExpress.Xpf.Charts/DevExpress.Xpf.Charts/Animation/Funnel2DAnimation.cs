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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Funnel2DSeriesPointAnimationBase : SeriesPointAnimationBase {
		public virtual Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			return pointBounds;
		}
	}
	public class Funnel2DWidenAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Widen"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new ElasticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DWidenAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			if (progress == 0.0)
				return new Rect(0, 0, 0, 0);
			if (progress == 1.0)
				return pointBounds;
			double delta = pointBounds.Width / 8 - pointBounds.Width * progress / 8;
			return new Rect(pointBounds.X + delta, pointBounds.Y, pointBounds.Width - delta * 2, pointBounds.Height);
		}
	}
	public class Funnel2DGrowUpAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Grow Up"; } }
		public Funnel2DGrowUpAnimation() {
			PointOrder = PointAnimationOrder.Inverted;
		}
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DGrowUpAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			if (progress == 0.0)
				return new Rect(0, 0, 0, 0);
			if (progress == 1.0)
				return pointBounds;
			return new Rect(pointBounds.X, pointBounds.Y + pointBounds.Height * (1 - progress), pointBounds.Width, pointBounds.Height * progress);
		}
	}
	public class Funnel2DSlideFromLeftAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DSlideFromLeftAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			return AnimationHelper.CreateSlideFromLeftAnimatedBounds(pointBounds, false, progress);
		}
	}
	public class Funnel2DSlideFromRightAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DSlideFromRightAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			return AnimationHelper.CreateSlideFromRightAnimatedBounds(pointBounds, viewport, false, progress);
		}
	}
	public class Funnel2DSlideFromTopAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new BackEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DSlideFromTopAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			return AnimationHelper.CreateSlideFromBottomAnimatedBounds(pointBounds, false, progress);
		}
	}
	public class Funnel2DSlideFromBottomAnimation : Funnel2DSeriesPointAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom"; } }
		public Funnel2DSlideFromBottomAnimation() {
			PointOrder = PointAnimationOrder.Inverted;
		}
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new BackEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DSlideFromBottomAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			return AnimationHelper.CreateSlideFromTopAnimatedBounds(pointBounds, viewport, false, progress);
		}
	}
	public class Funnel2DFadeInAnimation : Funnel2DSeriesPointAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(Funnel2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Funnel2DFadeInAnimationMode"),
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
			return new Funnel2DFadeInAnimation();
		}
	}
	public class Funnel2DFlyInAnimation : Funnel2DSeriesPointAnimationBase {
		Dictionary<Rect, bool> layoutAnimationSide2 = new Dictionary<Rect, bool>();
		bool fromLeft = true;
		protected internal override string AnimationName { get { return "Fly In"; } }
		public Funnel2DFlyInAnimation() {
			PointOrder = PointAnimationOrder.Inverted;
		}
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Funnel2DGrowUpAnimation();
		}
		public override Rect CalculateAnimatedBounds(Rect pointBounds, Rect viewport, double progress) {
			if (!layoutAnimationSide2.ContainsKey(pointBounds)) {
				layoutAnimationSide2[pointBounds] = fromLeft;
				fromLeft = !fromLeft;
			}
			if (progress == 0.0)
				return new Rect(0, 0, 0, 0);
			if (progress == 1.0)
				return pointBounds;
			Rect animatedBounds;
			if (layoutAnimationSide2[pointBounds])
				animatedBounds = AnimationHelper.CreateSlideFromLeftAnimatedBounds(pointBounds, false, progress);
			else
				animatedBounds = AnimationHelper.CreateSlideFromRightAnimatedBounds(pointBounds, viewport, false, progress);
			if (progress < 0.75)
				return new Rect(animatedBounds.X, animatedBounds.Y + animatedBounds.Height / 2, animatedBounds.Width, 2);
			else {
				double tempProgress = (progress - 0.75) * 4;
				return new Rect(animatedBounds.X, animatedBounds.Y + animatedBounds.Height / 2 * (1 - tempProgress), animatedBounds.Width, animatedBounds.Height * tempProgress);
			}
		}
	}
}
