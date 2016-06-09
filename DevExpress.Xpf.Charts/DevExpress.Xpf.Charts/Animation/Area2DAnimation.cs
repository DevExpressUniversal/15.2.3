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
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class AreaStacked2DFadeInAnimation : SeriesPointAnimationBase {
		public static readonly DependencyProperty ModeProperty = DependencyPropertyManager.Register("Mode",
			typeof(FadeInMode), typeof(AreaStacked2DFadeInAnimation), new FrameworkPropertyMetadata(FadeInMode.Auto, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AreaStacked2DFadeInAnimationMode"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
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
			return new AreaStacked2DFadeInAnimation();
		}
	}
	public abstract class Area2DAnimationBase : SeriesAnimationBase {
		public virtual Transform CreateAnimatedTransformation(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return new MatrixTransform() { Matrix = Matrix.Identity }; 
		}
	}
	public abstract class Area2DPredefinedAnimationBase : Area2DAnimationBase {
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected abstract Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress);
		public override Transform CreateAnimatedTransformation(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return new ScaleTransform() { ScaleX = 0, ScaleY = 0 };
			if (progress == 1.0)
				return null;
			return CreateAnimatedTransformationInternal(viewport, pointOfOrigin, axisXReverse, axisYReverse, diagramRotated, progress);
		}
	}
	public class Area2DGrowUpAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Grow Up"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return new ScaleTransform() { ScaleY = progress, CenterY = pointOfOrigin.Y };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DGrowUpAnimation();
		}
	}
	public class Area2DStretchFromNearAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Stretch From Near"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return new ScaleTransform() { ScaleX = progress };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DStretchFromNearAnimation();
		}
	}
	public class Area2DStretchFromFarAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Stretch From Far"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if(diagramRotated)
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Height };
			else
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Width };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DStretchFromFarAnimation();
		}
	}
	public class Area2DStretchOutAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Stretch Out"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if(diagramRotated)
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Height / 2 };
			else
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Width / 2 };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DStretchOutAnimation();
		}
	}
	public class Area2DDropFromNearAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Drop From Near"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { Y = -viewport.Width * (1.0 - progress) };
			else
				return new TranslateTransform() { Y = -viewport.Height * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DDropFromNearAnimation();
		}
	}
	public class Area2DDropFromFarAnimation : Area2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Drop From Far"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { Y = viewport.Width * (1.0 - progress)};
			else
				return new TranslateTransform() { Y = viewport.Height * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DDropFromFarAnimation();
		}
	}
	public class Area2DUnwindAnimation : Area2DAnimationBase, IUnwindAnimation {
		public static readonly DependencyProperty UnwindDirectionProperty = DependencyPropertyManager.Register("UnwindDirection",
			typeof(UnwindDirection), typeof(Area2DUnwindAnimation), new PropertyMetadata(UnwindDirection.LeftToRight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Area2DUnwindAnimationUnwindDirection"),
#endif
		Category(Categories.Behavior)
		]
		public UnwindDirection UnwindDirection {
			get { return (UnwindDirection)GetValue(UnwindDirectionProperty); }
			set { SetValue(UnwindDirectionProperty, value); }
		}
		protected internal override string AnimationName { get { return "Unwind"; } }
		protected internal override bool ShouldAnimateAdditionalGeometry { get { return false; } }
		protected internal override bool ShouldAnimateClipBounds { get { return true; } }
		public virtual Rect CreateAnimatedClipBounds(Rect clipBounds, double progress) {
			return UnwindAnimationHelper.CreateAnimatedClipBounds(clipBounds, progress, UnwindDirection);
		}
		protected override void Assign(AnimationBase animation) {
			base.Assign(animation);
			Area2DUnwindAnimation unwindAnimation = animation as Area2DUnwindAnimation;
			if (unwindAnimation != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, unwindAnimation, UnwindDirectionProperty);
		}
		protected override ChartDependencyObject CreateObject() {
			return new Area2DUnwindAnimation();
		}
	}
}
