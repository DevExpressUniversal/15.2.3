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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Editors {
#if !SL
	public class ProgressBarHighlightConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if (values.Length != 4)
				return null;
			if (values[0] == null || !typeof(Brush).IsAssignableFrom(values[0].GetType()))
				return null;
			for (int i = 1; i < 4; i++)
				if (!(values[i] is double))
					return null;
			Brush brush = (Brush)values[0];
			double d = (double)values[1];
			double num2 = (double)values[2];
			double accelerateRatio = (double)(values[3]);
			if (accelerateRatio <= 0)
				accelerateRatio = 1;
			if ((((d <= 0.0) || double.IsInfinity(d)) || (double.IsNaN(d) || (num2 <= 0.0))) || (double.IsInfinity(num2) || double.IsNaN(num2))) {
				return null;
			}
			DrawingBrush brush2 = new DrawingBrush();
			double width = d * 2.0;
			brush2.Viewport = brush2.Viewbox = new Rect(-d, 0.0, width, num2);
			brush2.ViewportUnits = brush2.ViewboxUnits = BrushMappingMode.Absolute;
			brush2.TileMode = TileMode.None;
			brush2.Stretch = Stretch.None;
			DrawingGroup group = new DrawingGroup();
			DrawingContext context = group.Open();
			context.DrawRectangle(brush, null, new Rect(-d, 0.0, d, num2));
			TimeSpan keyTime = TimeSpan.FromSeconds(width / 200.0 / accelerateRatio);
			TimeSpan span2 = TimeSpan.FromSeconds(1.0 / accelerateRatio);
			DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
			animation.BeginTime = new TimeSpan?(TimeSpan.Zero);
			animation.Duration = new Duration(keyTime + span2);
			animation.RepeatBehavior = RepeatBehavior.Forever;
			animation.KeyFrames.Add(new LinearDoubleKeyFrame(width, keyTime));
			TranslateTransform transform = new TranslateTransform();
			transform.BeginAnimation(TranslateTransform.XProperty, animation);
			brush2.Transform = transform;
			context.Close();
			brush2.Drawing = group;
			return brush2;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
	}
#endif
	public class ProgressBarMarqueeHorizontalAnimationControl : Control {
		public ProgressBarMarqueeHorizontalAnimationControl() {
			DefaultStyleKey = typeof(ProgressBarMarqueeHorizontalAnimationControl);
		}
	}
	public class ProgressBarMarqueeVerticalAnimationControl : Control {
		public ProgressBarMarqueeVerticalAnimationControl() {
			DefaultStyleKey = typeof(ProgressBarMarqueeVerticalAnimationControl);
		}
	}
	abstract public class AnimationElementBase : Canvas {
		Storyboard storyboard;
		ITargetChangedHelper<bool> IsVisibleChangedHelper { get; set; } 
		public static readonly DependencyProperty AccelerateRatioProperty;
		static AnimationElementBase() {
			Type ownerType = typeof(AnimationElementBase);
			AccelerateRatioProperty = DependencyPropertyManager.Register("AccelerateRatio", typeof(double), ownerType, new PropertyMetadata((d, e) => ((AnimationElementBase)d).AccelerateRatioChanged((double)e.NewValue)));
		}
		public AnimationElementBase() {
			Loaded += OnLoaded;
			SizeChanged += OnSizeChanged;
#if !SL
			IsVisibleChangedHelper = new EventToEventHelper<bool>();
			IsVisibleChanged += (d, e) => IsVisibleChangedHelper.RaiseTargetChanged((bool)e.NewValue);
#else
			IsVisibleChangedHelper = new BindingToEventHelper<bool>(this, VisibilityProperty, (value) => (Visibility)value == Visibility.Visible);
#endif
			IsVisibleChangedHelper.TargetChanged += (d, e) => OnIsVisibleChanged(e.Value);
			DevExpress.Xpf.Core.FrameworkElementHelper.SetIsClipped(this, true);
		}
		public double AccelerateRatio {
			get { return (double)GetValue(AccelerateRatioProperty); }
			set { SetValue(AccelerateRatioProperty, value); }
		}
		void AccelerateRatioChanged(double ratio) {
			StartAnimation();
		}
		void OnIsVisibleChanged(bool isVisible) {
			if (isVisible)
				StartAnimation();
			else 
				StopAnimation();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			StartAnimation();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			StartAnimation();
		}
		protected Duration GetAnimationDuration(double lenght) {
			double accelerateRatio = AccelerateRatio <= 0d ? 1d : AccelerateRatio;
			return new Duration(TimeSpan.FromSeconds(lenght / 100d / accelerateRatio));
		}
		protected void StopAnimation() {
			storyboard.Do(x => x.Stop());
			storyboard.Do(x => x.Children.Clear());
		}
		protected void StartAnimation() {
			StopAnimation();
			if(Children.Count == 0)
				return;
			VerifyStoryboard();
			FrameworkElement content = (FrameworkElement)Children[0];
			UpdateContent(content);
			Canvas.SetTop(content, 0);
			DoubleAnimation animation = CreateAnimation();
			Storyboard.SetTarget(animation, content);
			storyboard.Children.Add(animation);
			storyboard.Begin();
		}
		void VerifyStoryboard() {
			if (storyboard == null)
				storyboard = new Storyboard();
		}
		protected abstract DoubleAnimation CreateAnimation();
		protected abstract void UpdateContent(FrameworkElement content);
	}
	public class HorizontalAnimationElement : AnimationElementBase {
		double ElementWidth { get { return ActualWidth / 5; } }
		public HorizontalAnimationElement() {
		}
		protected override DoubleAnimation CreateAnimation() {
			double totalWidth = ActualWidth;
			DoubleAnimation animation = new DoubleAnimation {
				From = -ElementWidth,
				To = totalWidth,
				RepeatBehavior = RepeatBehavior.Forever,
				Duration = GetAnimationDuration(ActualWidth)
			};
			Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));
			return animation;
		}
		protected override void UpdateContent(FrameworkElement content) {
			double elementWidth = ActualWidth / 5;
			content.Width = ElementWidth;
			content.Height = ActualHeight;
		}
		}
	public class VerticalAnimationElement : AnimationElementBase {
		double ElementHeight { get { return ActualHeight / 5; } }
		public VerticalAnimationElement() {
		}
		protected override DoubleAnimation CreateAnimation() {
			double totalHeight = ActualHeight;
			DoubleAnimation animation = new DoubleAnimation {
				From = totalHeight,
				To = -ElementHeight,
				RepeatBehavior = RepeatBehavior.Forever,
				Duration = GetAnimationDuration(totalHeight)
			};
			Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
			return animation;
		}
		protected override void UpdateContent(FrameworkElement content) {
			content.Width = ActualWidth;
			content.Height = ElementHeight;
		}
	}
}
