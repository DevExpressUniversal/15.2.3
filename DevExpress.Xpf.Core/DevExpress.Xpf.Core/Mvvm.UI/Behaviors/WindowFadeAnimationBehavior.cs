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

using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
namespace DevExpress.Mvvm.UI {
	[TargetTypeAttribute(typeof(Window)), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class WindowFadeAnimationBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty EnableAnimationProperty =
			DependencyProperty.RegisterAttached("EnableAnimation", typeof(bool), typeof(WindowFadeAnimationBehavior),
			new PropertyMetadata(false, OnEnableAnimationChanged));
		public static bool GetEnableAnimation(Window obj) {
			return (bool)obj.GetValue(EnableAnimationProperty);
		}
		public static void SetEnableAnimation(Window obj, bool value) {
			obj.SetValue(EnableAnimationProperty, value);
		}
		static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Window w = d as Window;
			BehaviorCollection col = Interaction.GetBehaviors(w);
			WindowFadeAnimationBehavior b = (WindowFadeAnimationBehavior)col.FirstOrDefault(x => x is WindowFadeAnimationBehavior);
			col.Remove(b);
			if((bool)e.NewValue)
				col.Add(new WindowFadeAnimationBehavior());
		}
		public static readonly DependencyProperty WindowProperty =
			DependencyProperty.Register("Window", typeof(Window), typeof(WindowFadeAnimationBehavior),
			new PropertyMetadata(null, (d, e) => ((WindowFadeAnimationBehavior)d).OnWindowChanged((Window)e.OldValue)));
		public static readonly DependencyProperty FadeInDurationProperty =
			DependencyProperty.Register("FadeInDuration", typeof(TimeSpan), typeof(WindowFadeAnimationBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0.2)));
		public static readonly DependencyProperty FadeOutDurationProperty =
			DependencyProperty.Register("FadeOutDuration", typeof(TimeSpan), typeof(WindowFadeAnimationBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0.2)));
		public Window Window {
			get { return (Window)GetValue(WindowProperty); }
			set { SetValue(WindowProperty, value); }
		}
		public TimeSpan FadeInDuration {
			get { return (TimeSpan)GetValue(FadeInDurationProperty); }
			set { SetValue(FadeInDurationProperty, value); }
		}
		public TimeSpan FadeOutDuration {
			get { return (TimeSpan)GetValue(FadeOutDurationProperty); }
			set { SetValue(FadeOutDurationProperty, value); }
		}
		protected Window ActualWindow { get { return Window ?? AssociatedObject as Window; } }
		Storyboard fadeInAnimaiton = null;
		protected override void OnAttached() {
			base.OnAttached();
			Initialize();
		}
		protected override void OnDetaching() {
			Uninitialize(ActualWindow);
			base.OnDetaching();
		}
		void OnWindowChanged(Window oldValue) {
			Uninitialize(AssociatedObject as Window);
			Uninitialize(oldValue);
			Initialize();
		}
		void Initialize() {
			Uninitialize(ActualWindow);
			if(ActualWindow == null) return;
			ActualWindow.Closing += OnWindowClosing;
			ActualWindow.Loaded += OnWindowLoaded;
		}
		void Uninitialize(Window window) {
			if(window == null) return;
			window.Closing -= OnWindowClosing;
			window.Loaded -= OnWindowLoaded;
		}
		void OnWindowLoaded(object sender, RoutedEventArgs e) {
			Window w = (Window)sender;
			w.Loaded -= OnWindowLoaded;
			if(FadeInDuration.TotalMilliseconds == 0) return;
			fadeInAnimaiton = CreateStoryboard(w, 0, 1, FadeInDuration);
			fadeInAnimaiton.Completed += (d, ee) => fadeInAnimaiton = null;
			fadeInAnimaiton.Begin();
		}
		void OnWindowClosing(object sender, CancelEventArgs e) {
			if(e.Cancel)
				return;
			if(fadeInAnimaiton != null) {
				fadeInAnimaiton.Stop();
				fadeInAnimaiton = null;
			}
			Window w = (Window)sender;
			w.Closing -= OnWindowClosing;
			if(FadeOutDuration.TotalMilliseconds == 0) return;
			Storyboard st = CreateStoryboard(w, 1, 0, FadeOutDuration);
			st.Completed += (d, ee) => w.Close();
			e.Cancel = true;
			st.Begin();
		}
		Storyboard CreateStoryboard(Window w, double from, double to, TimeSpan duration) {
			Storyboard storyboard = new Storyboard();
			DoubleAnimation animation = new DoubleAnimation() {
				From = from,
				To = to,
				Duration = duration
			};
			Storyboard.SetTarget(animation, w);
			Storyboard.SetTargetProperty(animation, new PropertyPath(FrameworkElement.OpacityProperty));
			animation.Freeze();
			storyboard.Children.Add(animation);
			return storyboard;
		}
	}
}
