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
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#if DEMO
namespace DevExpress.Xpf.MapDemoBase.Helpers {
#else
namespace DevExpress.Xpf.DemoBase.Helpers {
#endif
	static class VisibilityHelper {
		static readonly TimeSpan Time = TimeSpan.FromMilliseconds(200);
		#region Dependency Properties
		public static readonly DependencyProperty IsVisibleProperty;
		static VisibilityHelper() {
			Type ownerType = typeof(VisibilityHelper);
			IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool?), ownerType, new PropertyMetadata(null, RaiseIsVisibleChanged));
		}
		#endregion
		public static bool GetIsVisible(FrameworkElement d) { return (bool)d.GetValue(IsVisibleProperty); }
		public static void SetIsVisible(FrameworkElement d, bool v) { d.SetValue(IsVisibleProperty, v); }
		static void RaiseIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			bool? oldValue = (bool?)e.OldValue;
			bool? newValue = (bool?)e.NewValue;
			if(newValue == null) return;
			bool useAnimation = oldValue != null;
			FrameworkElement button = (FrameworkElement)d;
			if((bool)newValue) {
				Canvas.SetZIndex(button, 99);
				if(useAnimation) {
					Storyboard sb = GetFadeInStoryboard(button);
					sb.Begin();
				} else {
					button.Opacity = 1.0;
				}
				button.IsHitTestVisible = true;
			} else {
				Canvas.SetZIndex(button, 0);
				if(useAnimation) {
					Storyboard sb = GetFadeOutStoryboard(button);
					sb.Begin();
				} else {
					button.Opacity = 0.0;
				}
				button.IsHitTestVisible = false;
			}
		}
		static Storyboard GetFadeInStoryboard(FrameworkElement target) {
			ExponentialEase fadeIn = new ExponentialEase() { EasingMode = EasingMode.EaseIn };
			DoubleAnimation animation = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = new Duration(Time), FillBehavior = FillBehavior.HoldEnd, EasingFunction = fadeIn };
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
			Storyboard sb = new Storyboard();
			sb.Children.Add(animation);
			EventHandler completed = null;
			completed = (s, e) => {
				sb.Completed -= completed;
				sb.Stop();
				target.Opacity = 1.0;
			};
			sb.Completed += completed;
			Storyboard.SetTarget(sb, target);
			return sb;
		}
		static Storyboard GetFadeOutStoryboard(FrameworkElement target) {
			DoubleAnimation animation = new DoubleAnimation() { From = 0.0, Duration = new Duration(new TimeSpan(0)), BeginTime = Time };
			Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
			Storyboard sb = new Storyboard();
			sb.Children.Add(animation);
			EventHandler completed = null;
			completed = (s, e) => {
				sb.Completed -= completed;
				sb.Stop();
				target.Opacity = 0.0;
			};
			sb.Completed += completed;
			Storyboard.SetTarget(sb, target);
			return sb;
		}
	}
}
