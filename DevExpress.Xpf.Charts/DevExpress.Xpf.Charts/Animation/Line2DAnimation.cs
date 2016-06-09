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
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Line2DAnimationBase : SeriesAnimationBase {
		public virtual Transform CreateAnimatedTransformation(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) { 
			return new MatrixTransform() { Matrix = Matrix.Identity };
		}
	}
	public abstract class Line2DPredefinedAnimationBase : Line2DAnimationBase {
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
	public class Line2DSlideFromLeftAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Left"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { Y = -viewport.Width * (1.0 - progress) };
			else
				return new TranslateTransform() { X = -viewport.Width * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DSlideFromLeftAnimation();
		}
	}
	public class Line2DSlideFromRightAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Right"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { Y = viewport.Width * (1.0 - progress) };
			else
				return new TranslateTransform() { X = viewport.Width * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DSlideFromRightAnimation();
		}
	}
	public class Line2DSlideFromTopAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Top"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { X = viewport.Height * (1.0 - progress) };
			else
				return new TranslateTransform() { Y = viewport.Height * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DSlideFromTopAnimation();
		}
	}
	public class Line2DSlideFromBottomAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Slide From Bottom"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new TranslateTransform() { X = -viewport.Height * (1.0 - progress) };
			else
				return new TranslateTransform() { Y = -viewport.Height * (1.0 - progress) };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DSlideFromBottomAnimation();
		}
	}
	public class Line2DUnwrapVerticallyAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Unwrap Vertically"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return null;
		}
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated) {
				if (progress < 0.5)
					return new ScaleTransform() { ScaleX = 0, ScaleY = progress * 2, CenterX = viewport.Height / 2, CenterY = viewport.Width / 2 };
				else
					return new ScaleTransform() { ScaleX = (progress - 0.5) * 2, ScaleY = 1, CenterX = viewport.Height / 2, CenterY = viewport.Width / 2 };
			}
			else {
				if (progress < 0.5)
					return new ScaleTransform() { ScaleX = progress * 2, ScaleY = 0, CenterX = viewport.Width / 2, CenterY = viewport.Height / 2 };
				else
					return new ScaleTransform() { ScaleX = 1, ScaleY = (progress - 0.5) * 2, CenterX = viewport.Width / 2, CenterY = viewport.Height / 2 };
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DUnwrapVerticallyAnimation();
		}
	}
	public class Line2DUnwrapHorizontallyAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Unwrap Horizontally"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return null;
		}
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated) {
				if (progress < 0.5)
					return new ScaleTransform() { ScaleX = progress * 2, ScaleY = 0, CenterX = viewport.Height / 2, CenterY = viewport.Width / 2 };
				else
					return new ScaleTransform() { ScaleX = 1, ScaleY = (progress - 0.5) * 2, CenterX = viewport.Height / 2, CenterY = viewport.Width / 2 };
			}
			else {
				if (progress < 0.5)
					return new ScaleTransform() { ScaleX = 0, ScaleY = progress * 2, CenterX = viewport.Width / 2, CenterY = viewport.Height / 2 };
				else
					return new ScaleTransform() { ScaleX = (progress - 0.5) * 2, ScaleY = 1, CenterX = viewport.Width / 2, CenterY = viewport.Height / 2 };
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DUnwrapHorizontallyAnimation();
		}
	}
	public class Line2DBlowUpAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Blow Up"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (diagramRotated)
				return new ScaleTransform() { ScaleX = progress, ScaleY = progress, CenterX = viewport.Height / 2, CenterY = viewport.Width / 2 };
			else
				return new ScaleTransform() { ScaleX = progress, ScaleY = progress, CenterX = viewport.Width / 2, CenterY = viewport.Height / 2 };				
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DBlowUpAnimation();
		}
	}
	public class Line2DStretchFromNearAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Stretch From Near"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			return new ScaleTransform() { ScaleX = progress };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DStretchFromNearAnimation();
		}
	}
	public class Line2DStretchFromFarAnimation : Line2DPredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Stretch From Far"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if(diagramRotated)
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Height };
			else
				return new ScaleTransform() { ScaleX = progress, CenterX = viewport.Width };
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DStretchFromFarAnimation();
		}
	}
	public class Line2DUnwindAnimation : Line2DAnimationBase, IUnwindAnimation {
		public static readonly DependencyProperty UnwindDirectionProperty = DependencyPropertyManager.Register("UnwindDirection",
			typeof(UnwindDirection), typeof(Line2DUnwindAnimation), new PropertyMetadata(UnwindDirection.LeftToRight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Line2DUnwindAnimationUnwindDirection"),
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
			Line2DUnwindAnimation unwindAnimation = animation as Line2DUnwindAnimation;
			if (unwindAnimation != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, unwindAnimation, UnwindDirectionProperty);
		}
		protected override ChartDependencyObject CreateObject() {
			return new Line2DUnwindAnimation();
		}
	}
}
