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
using System.Windows.Controls;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.WindowsUI.Base;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI.Internal {
	internal class ScrollViewerBehavior {
		#region static
		public static DependencyProperty HorizontalOffsetProperty;
		public static DependencyProperty VerticalOffsetProperty;
		static ScrollViewerBehavior() {
			var dProp = new DependencyPropertyRegistrator<ScrollViewerBehavior>();
			dProp.RegisterAttached("HorizontalOffset", ref HorizontalOffsetProperty, 0d, OnHorizontalOffsetChanged);
			dProp.RegisterAttached("VerticalOffset", ref VerticalOffsetProperty, 0d, OnVerticalOffsetChanged);
		}
		public static void SetHorizontalOffset(FrameworkElement target, double value) {
			target.SetValue(HorizontalOffsetProperty, value);
		}
		public static double GetHorizontalOffset(FrameworkElement target) {
			return (double)target.GetValue(HorizontalOffsetProperty);
		}
		static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			ScrollViewer scrollViewer = target as ScrollViewer;
			if(scrollViewer != null) {
				scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
			}
		}
		public static void SetVerticalOffset(FrameworkElement target, double value) {
			target.SetValue(VerticalOffsetProperty, value);
		}
		public static double GetVerticalOffset(FrameworkElement target) {
			return (double)target.GetValue(VerticalOffsetProperty);
		}
		static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) {
			ScrollViewer scrollViewer = target as ScrollViewer;
			if(scrollViewer != null) {
				scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
			}
		}
		#endregion
	}
	public class ConstrainedSizeBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty ConstrainedWidthProperty;
		public static readonly DependencyProperty ConstrainedHeightProperty;
		static ConstrainedSizeBehavior() {
			var dProp = new DependencyPropertyRegistrator<ConstrainedSizeBehavior>();
			dProp.RegisterAttached("ConstrainedWidth", ref ConstrainedWidthProperty, double.NaN, OnConstraintChanged);
			dProp.RegisterAttached("ConstrainedHeight", ref ConstrainedHeightProperty, double.NaN, OnConstraintChanged);
		}
		public static double GetConstrainedHeight(FrameworkElement obj) {
			return (double)obj.GetValue(ConstrainedHeightProperty);
		}
		public static void SetConstrainedHeight(FrameworkElement obj, double value) {
			obj.SetValue(ConstrainedHeightProperty, value);
		}
		public static double GetConstrainedWidth(FrameworkElement obj) {
			return (double)obj.GetValue(ConstrainedWidthProperty);
		}
		public static void SetConstrainedWidth(FrameworkElement obj, double value) {
			obj.SetValue(ConstrainedWidthProperty, value);
		}
		private static void OnConstraintChanged(object sender, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = sender as FrameworkElement;
			if(element != null) {
				double width = GetConstrainedWidth(element);
				double height = GetConstrainedHeight(element);
				if(width != double.NaN && height != double.NaN) {
					double value = Math.Min(width, height);
					element.Width = value;
					element.Height = value;
				}
			}
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
			AssociatedObject.Loaded += AssociatedObject_Loaded;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}
		void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e) {
			SetConstraints();
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			SetConstraints();
		}
		void SetConstraints() {
			FrameworkElement element = AssociatedObject;
			if(element != null) {
				double width = element.ActualWidth;
				double height = element.ActualHeight;
				if(!double.IsInfinity(width) && !double.IsInfinity(height)) {
					double value = Math.Min(width, height);
					element.Width = value;
					element.Height = value;
				}
			}
		}
	}
	public class AnimatedPanelBehavior :  Behavior<FrameworkElement> {
		#region static
		public static readonly DependencyProperty WidthProperty;
		static AnimatedPanelBehavior() {
			var dProp = new DependencyPropertyRegistrator<AnimatedPanelBehavior>();
			dProp.Register("Width", ref WidthProperty, double.NaN,
				(d, e) => ((AnimatedPanelBehavior)d).OnWidthChanged((double)e.OldValue, (double)e.NewValue));
		}
		#endregion
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		void OnWidthChanged(double oldValue, double newValue) {
			if(AssociatedObject != null) AssociatedObject.Width = newValue;
		}
		protected override void OnAttached() {
			base.OnAttached();
			if(AssociatedObject != null) AssociatedObject.Width = Width;
		}
	}
}
