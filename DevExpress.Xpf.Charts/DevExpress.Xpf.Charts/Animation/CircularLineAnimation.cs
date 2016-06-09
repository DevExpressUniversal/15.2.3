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
	public abstract class CircularLineAnimationBase : SeriesAnimationBase {
		public virtual Transform CreateAnimatedTransformation(Rect viewport, Point pointOfOrigin, double progress) {
			return new MatrixTransform() { Matrix = Matrix.Identity };
		}
	}
	public abstract class CircularLinePredefinedAnimationBase : CircularLineAnimationBase {
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new QuinticEase() { EasingMode = EasingMode.EaseOut };
		}
		protected abstract Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, double progress);
		public override Transform CreateAnimatedTransformation(Rect viewport, Point pointOfOrigin, double progress) {
			if (progress == 0.0)
				return new ScaleTransform() { ScaleX = 0, ScaleY = 0 };
			if (progress == 1.0)
				return null;
			return CreateAnimatedTransformationInternal(viewport, pointOfOrigin, progress);
		}
	}
	public class CircularLineZoomInAnimation : CircularLinePredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Zoom In"; } }
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, double progress) {
			return new ScaleTransform() { ScaleY = progress, ScaleX = progress, CenterY = pointOfOrigin.Y, CenterX = pointOfOrigin.X };
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularLineZoomInAnimation();
		}
	}
	public class CircularLineSpinAnimation : CircularLinePredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Spin"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut };
		}
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, double progress) {
			return new RotateTransform() { Angle = progress * 360, CenterY = pointOfOrigin.Y, CenterX = pointOfOrigin.X };
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularLineSpinAnimation();
		}
	}
	public class CircularLineSpinZoomInAnimation : CircularLinePredefinedAnimationBase {
		protected internal override string AnimationName { get { return "Spin Zoom in"; } }
		protected override IEasingFunction GetDefaultEasingFunction() {
			return new PowerEase() { Power = 3, EasingMode = EasingMode.EaseOut };
		}
		protected override Transform CreateAnimatedTransformationInternal(Rect viewport, Point pointOfOrigin, double progress) {
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new RotateTransform() { Angle = progress * 360, CenterY = pointOfOrigin.Y, CenterX = pointOfOrigin.X });
			transform.Children.Add(new ScaleTransform() { ScaleY = progress, ScaleX = progress, CenterY = pointOfOrigin.Y, CenterX = pointOfOrigin.X });
			return transform;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularLineSpinZoomInAnimation();
		}
	}
	public class CircularLineUnwindAnimation : CircularLineAnimationBase, IUnwindAnimation {
		public static readonly DependencyProperty UnwindDirectionProperty = DependencyPropertyManager.Register("UnwindDirection",
			typeof(UnwindDirection), typeof(CircularLineUnwindAnimation), new PropertyMetadata(UnwindDirection.LeftToRight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularLineUnwindAnimationUnwindDirection"),
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
			CircularLineUnwindAnimation unwindAnimation = animation as CircularLineUnwindAnimation;
			if (unwindAnimation != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, unwindAnimation, UnwindDirectionProperty);
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircularLineUnwindAnimation();
		}
	}
}
