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
	public abstract class Stock2DAnimationBase : SeriesPointAnimationBase {
		public virtual Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return stockBounds;
		}
	}
	public abstract class Stock2DSlideAnimationBase : Stock2DAnimationBase {
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
	}
	public class Stock2DSlideFromLeftAnimation : Stock2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Stock2DSlideFromLeftAnimation();
		}
		public override Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromLeftAnimatedBounds(stockBounds, diagramRotated, progress); 
		}
	}
	public class Stock2DSlideFromRightAnimation : Stock2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Stock2DSlideFromRightAnimation();
		}
		public override Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromRightAnimatedBounds(stockBounds, viewport, diagramRotated, progress);
		}
	}
	public class Stock2DSlideFromTopAnimation : Stock2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Stock2DSlideFromTopAnimation();
		}
		public override Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromTopAnimatedBounds(stockBounds, viewport, diagramRotated, progress);
		}
	}
	public class Stock2DSlideFromBottomAnimation : Stock2DSlideAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom"; } }
		protected override ChartDependencyObject CreateObject() {
			return new Stock2DSlideFromBottomAnimation();
		}
		public override Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return AnimationHelper.CreateSlideFromBottomAnimatedBounds(stockBounds, diagramRotated, progress);
		}
	}
	public class Stock2DExpandAnimation : Stock2DAnimationBase {
		protected internal override string AnimationName { get { return "Expand"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new ElasticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Stock2DExpandAnimation();
		}
		public override Rect CreateAnimatedStockBounds(Rect stockBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return stockBounds;
			double x = stockBounds.X;
			double y = stockBounds.Y + stockBounds.Height / 2 - stockBounds.Height * progress / 2;
			double width = stockBounds.Width;
			double height = stockBounds.Height * progress;
			return new Rect(x, y, width, height);
		}
	}
	public class Stock2DFadeInAnimation : Stock2DAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(Stock2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Stock2DFadeInAnimationMode"),
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
			return new Stock2DFadeInAnimation();
		}
	}
}
