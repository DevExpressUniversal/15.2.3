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
	public abstract class CircularMarkerAnimationBase : SeriesPointAnimationBase {
		public virtual Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, double progress) {
			return markerBounds;
		}
	}
	public class CircularMarkerWidenAnimation : CircularMarkerAnimationBase {
		protected internal override string AnimationName { get { return "Widen"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new ElasticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerWidenAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, double progress) {
			return AnimationHelper.CreateWidenAnimatedMarkerBounds(markerBounds, viewport, false, false, false, progress);
		}
	}
	public class CircularMarkerFadeInAnimation : CircularMarkerAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(CircularMarkerFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularMarkerFadeInAnimationMode"),
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
			return new CircularMarkerFadeInAnimation();
		}
	}
	public abstract class CircularMarkerLinearSlideAnimationBase : CircularMarkerAnimationBase {
		protected abstract double CalculateStartX(Rect markerBounds, Rect viewport);
		protected abstract double CalculateStartY(Rect markerBounds, Rect viewport);
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return markerBounds;
			double startX = CalculateStartX(markerBounds, viewport);
			double startY = CalculateStartY(markerBounds, viewport);
			double finalX = markerBounds.X;
			double fianlY = markerBounds.Y;
			double x = startX + (finalX - startX) * progress;
			double y = startY + (fianlY - startY) * progress;
			double width = markerBounds.Width;
			double height = markerBounds.Height;
			return new Rect(x, y, width, height);
		}
	}
	public class CircularMarkerSlideFromLeftCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			return viewport.X - markerBounds.Width / 2 - markerBounds.Width;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			return viewport.Height / 2 - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideFromLeftCenterAnimation();
		}
	}
	public class CircularMarkerSlideFromRightCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			return viewport.Width;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			return viewport.Height / 2 - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideFromRightCenterAnimation();
		}
	}
	public class CircularMarkerSlideFromTopCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			return viewport.Width / 2 - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			return viewport.Y - markerBounds.Width;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideFromTopCenterAnimation();
		}
	}
	public class CircularMarkerSlideFromBottomCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			return viewport.Width / 2 - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			return viewport.Height;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideFromBottomCenterAnimation();
		}
	}
	public class CircularMarkerSlideFromCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			return viewport.Width / 2 - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			return viewport.Height / 2 - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideFromCenterAnimation();
		}
	}
	public class CircularMarkerSlideToCenterAnimation : CircularMarkerLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide To Center"; } }
		double CalculateDistance(Rect markerBounds, Rect viewport) {
			double deltaX = viewport.Width / 2 - markerBounds.X;
			double deltaY = viewport.Height / 2 - markerBounds.Y;
			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}
		protected override double CalculateStartX(Rect markerBounds, Rect viewport) {
			double distance = CalculateDistance(markerBounds, viewport);
			return viewport.Width / 2 * (1 + (markerBounds.X - viewport.Width / 2) / distance);
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport) {
			double distance = CalculateDistance(markerBounds, viewport);
			return viewport.Height / 2 *(1 + (markerBounds.Y - viewport.Height / 2)/ distance);
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularMarkerSlideToCenterAnimation();
		}
	}
}
