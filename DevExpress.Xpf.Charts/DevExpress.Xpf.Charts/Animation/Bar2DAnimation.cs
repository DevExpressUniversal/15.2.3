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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Bar2DAnimationBase : SeriesPointAnimationBase {
		Rect CreateAnimatedMarkerBounds(Rect markerBounds, bool isMinMarker, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return markerBounds;
			double deltaX = markerBounds.X - barBounds.X;
			if (axisYReverse)
				isMinMarker = !isMinMarker;
			double deltaY = isMinMarker ? markerBounds.Y - barBounds.Y : markerBounds.Y - barBounds.Bottom;
			Rect animatedBarBounds = CreateAnimatedBarBounds(barBounds, viewport, isNegativeBar, axisXReverse, axisYReverse, diagramRotated, progress);
			double animatedMarkerBoundsY = isMinMarker ? animatedBarBounds.Y + deltaY : animatedBarBounds.Bottom + deltaY;
			Rect animatedMarkerBounds = new Rect(animatedBarBounds.X + deltaX, animatedMarkerBoundsY, markerBounds.Width, markerBounds.Height);
			return animatedMarkerBounds;
		}
		public virtual Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return barBounds;
		}
		public virtual Rect CreateAnimatedMinMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return CreateAnimatedMarkerBounds(markerBounds, true, barBounds, viewport, isNegativeBar, axisXReverse, axisYReverse, diagramRotated, progress);
		}
		public virtual Rect CreateAnimatedMaxMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return CreateAnimatedMarkerBounds(markerBounds, false, barBounds, viewport, isNegativeBar, axisXReverse, axisYReverse, diagramRotated, progress);
		}
	}
	public class Bar2DGrowUpAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Grow Up"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DGrowUpAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return barBounds;
			double x = barBounds.X;
			double y = ((isNegativeBar && !axisYReverse) || (!isNegativeBar && axisYReverse)) ? barBounds.Y + barBounds.Height * (1.0 - progress) : barBounds.Y;
			double width = barBounds.Width;
			double height = barBounds.Height * progress;
			return new Rect(x, y, width, height);
		}
		public override Rect CreateAnimatedMinMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return markerBounds;
		}
		public override Rect CreateAnimatedMaxMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return base.CreateAnimatedMaxMarkerBounds(markerBounds, barBounds, viewport, isNegativeBar, axisXReverse, axisYReverse, diagramRotated, progress);
		}
	}
	public class Bar2DDropInAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Drop In"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DDropInAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return barBounds;
			bool isReverseBar = (isNegativeBar && !axisYReverse) || (!isNegativeBar && axisYReverse);
			double x = barBounds.X;
			double y = isReverseBar ?
				barBounds.Y - (barBounds.Y + barBounds.Height) * (1.0 - progress) :
				barBounds.Y + ((diagramRotated ? viewport.Width : viewport.Height) - barBounds.Y) * (1.0 - progress);
			double width = barBounds.Width;
			double height = barBounds.Height;
			return new Rect(x, y, width, height);
		}
	}
	public class Bar2DBounceAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Bounce"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new BounceEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DBounceAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return barBounds;
			double x, y;
			if (diagramRotated) {
				x = barBounds.X + (viewport.Height - barBounds.X) * (1.0 - progress);
				y = barBounds.Y;
			}
			else {
				x = barBounds.X;
				y = barBounds.Y + (viewport.Height - barBounds.Y) * (1.0 - progress);
			}
			double width = barBounds.Width;
			double height = barBounds.Height;
			return new Rect(x, y, width, height);
		}
	}
	public class Bar2DSlideFromLeftAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DSlideFromLeftAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromLeftAnimatedBounds(barBounds, diagramRotated, progress); 
		}
	}
	public class Bar2DSlideFromRightAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DSlideFromRightAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromRightAnimatedBounds(barBounds, viewport, diagramRotated, progress); 
		}
	}
	public class Bar2DSlideFromTopAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new BackEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DSlideFromTopAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromTopAnimatedBounds(barBounds, viewport, diagramRotated, progress); 
		}
	}
	public class Bar2DSlideFromBottomAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new BackEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DSlideFromBottomAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromBottomAnimatedBounds(barBounds, diagramRotated, progress); 
		}
	}
	public class Bar2DWidenAnimation : Bar2DAnimationBase {
		protected internal override string AnimationName { get { return "Widen"; } }
		Rect CalculateBounds(Rect bounds, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return bounds;
			double x = bounds.X + bounds.Width / 2 - bounds.Width * progress / 2;
			double y = bounds.Y;
			double width = bounds.Width * progress;
			double height = bounds.Height;
			return new Rect(x, y, width, height);
		}
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new ElasticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Bar2DWidenAnimation();
		}
		public override Rect CreateAnimatedBarBounds(Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return CalculateBounds(barBounds, progress);
		}
		public override Rect CreateAnimatedMinMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return CalculateBounds(markerBounds, progress);
		}
		public override Rect CreateAnimatedMaxMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return CalculateBounds(markerBounds, progress);
		}
	}
	public class Bar2DFadeInAnimation : Bar2DAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(Bar2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Bar2DFadeInAnimationMode"),
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
			return new Bar2DFadeInAnimation();
		}
		public override Rect CreateAnimatedMinMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return markerBounds;
		}
		public override Rect CreateAnimatedMaxMarkerBounds(Rect markerBounds, Rect barBounds, Rect viewport, bool isNegativeBar, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return markerBounds;
		}
	}
}
