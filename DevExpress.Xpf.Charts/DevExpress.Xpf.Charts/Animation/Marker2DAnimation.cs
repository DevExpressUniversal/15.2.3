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
	public abstract class Marker2DAnimationBase : SeriesPointAnimationBase {
		public virtual Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return markerBounds;
		}
	}
	public class Marker2DWidenAnimation : Marker2DAnimationBase {
		protected internal override string AnimationName { get { return "Widen"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new ElasticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DWidenAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateWidenAnimatedMarkerBounds(markerBounds, viewport, axisXReverse, axisYReverse, diagramRotated, progress);
		}
	}
	public abstract class Marker2DSlideAnimationBase : Marker2DAnimationBase {
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
	}
	public class Marker2DSlideFromLeftAnimation : Marker2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromLeftAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromLeftAnimatedBounds(markerBounds, diagramRotated, progress); 
		}
	}
	public class Marker2DSlideFromRightAnimation : Marker2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromRightAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromRightAnimatedBounds(markerBounds, viewport, diagramRotated, progress); 
		}
	}
	public class Marker2DSlideFromTopAnimation : Marker2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromTopAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromTopAnimatedBounds(markerBounds, viewport, diagramRotated, progress); 
		}
	}
	public class Marker2DSlideFromBottomAnimation : Marker2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromBottomAnimation();
		}
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromBottomAnimatedBounds(markerBounds, diagramRotated, progress); 
		}
	}
	public abstract class Marker2DLinearSlideAnimationBase : Marker2DSlideAnimationBase {
		protected abstract double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated);
		protected abstract double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated);
		public override Rect CreateAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return markerBounds;
			double startX = CalculateStartX(markerBounds, viewport, diagramRotated);
			double startY = CalculateStartY(markerBounds, viewport, diagramRotated);
			double finalX = markerBounds.X + markerBounds.Width / 2;
			double fianlY = markerBounds.Y + markerBounds.Height / 2;
			double w = diagramRotated ? markerBounds.Width : markerBounds.Height;
			double h = diagramRotated ? markerBounds.Height : markerBounds.Width;
			double x = (startX + (finalX - startX) * progress) - w / 2;
			double y = (startY + (fianlY - startY) * progress) - h / 2;
			double width = markerBounds.Width;
			double height = markerBounds.Height;
			return new Rect(x, y, width, height);
		}
	}
	public class Marker2DSlideFromLeftCenterAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Height / 2 : viewport.X - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.X - markerBounds.Height / 2 : viewport.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromLeftCenterAnimation();
		}
	}
	public class Marker2DSlideFromRightCenterAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Height / 2 : viewport.Width + markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Width + markerBounds.Height / 2 : viewport.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromRightCenterAnimation();
		}
	}
	public class Marker2DSlideFromTopCenterAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Height + markerBounds.Width / 2 : viewport.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Width / 2 : viewport.Height + markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromTopCenterAnimation();
		}
	}
	public class Marker2DSlideFromBottomCenterAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom Center"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Y - markerBounds.Width / 2 : viewport.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Width / 2 : viewport.Y - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromBottomCenterAnimation();
		}
	}
	public class Marker2DSlideFromLeftTopCornerAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left-Top Corner"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Height + markerBounds.Width / 2 : viewport.X - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.X - markerBounds.Height / 2 : viewport.Height + markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromLeftTopCornerAnimation();
		}
	}
	public class Marker2DSlideFromRightTopCornerAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right-Top Corner"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Height + markerBounds.Width / 2 : viewport.Width + markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Width + markerBounds.Height / 2 : viewport.Height + markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromRightTopCornerAnimation();
		}
	}
	public class Marker2DSlideFromRightBottomCornerAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right-Bottom Corner"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Y - markerBounds.Width / 2 : viewport.Width + markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Width + markerBounds.Height / 2 : viewport.Y - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromRightBottomCornerAnimation();
		}
	}
	public class Marker2DSlideFromLeftBottomCornerAnimation : Marker2DLinearSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left-Bottom Corner"; } }
		protected override double CalculateStartX(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.Y - markerBounds.Width / 2 : viewport.X - markerBounds.Width / 2;
		}
		protected override double CalculateStartY(Rect markerBounds, Rect viewport, bool diagramRotated) {
			return diagramRotated ? viewport.X - markerBounds.Height / 2 : viewport.Y - markerBounds.Height / 2;
		}
		protected override ChartDependencyObject CreateObject() {
			return new Marker2DSlideFromLeftBottomCornerAnimation();
		}
	}
	public class Marker2DFadeInAnimation : Marker2DAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(Marker2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
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
			return new Marker2DFadeInAnimation();
		}
	}
}
