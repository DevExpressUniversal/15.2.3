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

using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Editors.Flyout.Native {
	public class FlyoutAnimatorBase {
		public virtual Storyboard GetOpenAnimation(FlyoutBase flyout, Point offset) {
			return null;
		}
		public virtual Storyboard GetCloseAnimation(FlyoutBase flyout) {
			return null;
		}
		public virtual Storyboard GetMoveAnimation(FlyoutBase flyout, Point from, Point to) {
			return null;
		}
	}
	public class FlyoutAnimator : FlyoutAnimatorBase {
		const string TranslateName = "FlyoutTranslateName";
		public virtual Duration GetDuration(FlyoutBase flyout) {
			return flyout.AnimationDuration;
		}
		public override Storyboard GetOpenAnimation(FlyoutBase flyout, Point offset) {
			TranslateTransform transform = GetMoveTransform(flyout);
			if (transform == null)
				return null;
			Storyboard storyboard = GetMoveAnimation(flyout, offset, new Point(0, 0));
			if (storyboard == null)
				return null;
			Timeline fadeOutAnimation = new DoubleAnimation {
				From = 0.0,
				To = 1.0,
				Duration = GetDuration(flyout),
				EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4d }
			};
			storyboard.Children.Add(fadeOutAnimation);
			Storyboard.SetTarget(fadeOutAnimation, flyout.RenderGrid);
			Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
			return storyboard;
		}
		public override Storyboard GetCloseAnimation(FlyoutBase flyout) {
			Timeline closeAnimation = new DoubleAnimation {
				From = 1.0,
				To = 0.0,
				Duration = GetDuration(flyout),
				EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4d }
			};
			Storyboard storyboard = CreateStoryboard(closeAnimation);
			Storyboard.SetTarget(closeAnimation, flyout.RenderGrid);
			Storyboard.SetTargetProperty(closeAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
			return storyboard;
		}
		public override Storyboard GetMoveAnimation(FlyoutBase flyout, Point from, Point to) {
			TranslateTransform transform = GetMoveTransform(flyout);
			if (transform == null)
				return null;
			Timeline animationX = GetMoveAnimation(from.X, to.X, GetDuration(flyout));
			Timeline animationY = GetMoveAnimation(from.Y, to.Y, GetDuration(flyout));
			Storyboard storyboard = CreateStoryboard(animationX, animationY);
			Storyboard.SetTargetProperty(animationX, new PropertyPath(TranslateTransform.XProperty));
			Storyboard.SetTargetProperty(animationY, new PropertyPath(TranslateTransform.YProperty));
			Storyboard.SetTargetName(animationX, TranslateName);
			Storyboard.SetTargetName(animationY, TranslateName);
			return storyboard;
		}
		TranslateTransform GetMoveTransform(FlyoutBase flyout) {
			if (flyout.RenderGrid == null)
				return null;
			TranslateTransform transform = flyout.RenderGrid.RenderTransform as TranslateTransform;
			if (transform == null) {
				transform = new TranslateTransform();
				INameScope nameScope = NameScope.GetNameScope(flyout);
				if (nameScope == null) {
					nameScope = new NameScope();
					NameScope.SetNameScope(flyout, nameScope);
				}
				if (nameScope.FindName(TranslateName) != null) {
					nameScope.UnregisterName(TranslateName);
				}
				flyout.RegisterName(TranslateName, transform);
				flyout.RenderGrid.RenderTransform = transform;
			}
			return transform;
		}
		Timeline GetMoveAnimation(double from, double to, Duration duration) {
			return new DoubleAnimation {
				From = from,
				To = to,
				Duration = duration,
				EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4d }
			};
		}
		Storyboard CreateStoryboard(params Timeline[] timelines) {
			Storyboard storyboard = new Storyboard();
			timelines.Do(x => Array.ForEach(x, timeline => storyboard.Children.Add(timeline)));
			return storyboard;
		}
	}
}
