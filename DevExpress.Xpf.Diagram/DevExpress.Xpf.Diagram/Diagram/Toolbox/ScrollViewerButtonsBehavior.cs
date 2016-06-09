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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Diagram {
	public class ScrollViewerButtonsBehavior : Behavior<ScrollViewer> {
		public static readonly DependencyProperty ScrollUpButtonVisibilityProperty =
			DependencyProperty.Register("ScrollUpButtonVisibility", typeof(Visibility), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty ScrollDownButtonVisibilityProperty =
			DependencyProperty.Register("ScrollDownButtonVisibility", typeof(Visibility), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty IsCompactProperty =
			DependencyProperty.Register("IsCompact", typeof(bool), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(false, (d, e) => ((ScrollViewerButtonsBehavior)d).OnIsCompactChanged()));
		public static readonly DependencyProperty ViewerViewportWidthProperty =
			DependencyProperty.Register("ViewerViewportWidth", typeof(double), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(0.0));
		static readonly DependencyProperty ScrollableHeightProperty =
			DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(0.0, (d, e) => ((ScrollViewerButtonsBehavior)d).OnScrollableHeightChanged()));
		static readonly DependencyProperty InternalViewportWidthProperty =
			DependencyProperty.Register("InternalViewportWidth", typeof(double), typeof(ScrollViewerButtonsBehavior), new PropertyMetadata(0.0, (d, e) => ((ScrollViewerButtonsBehavior)d).OnInternalViewportWidthChanged(e)));
		public Visibility ScrollUpButtonVisibility {
			get { return (Visibility)GetValue(ScrollUpButtonVisibilityProperty); }
			set { SetValue(ScrollUpButtonVisibilityProperty, value); }
		}
		public Visibility ScrollDownButtonVisibility {
			get { return (Visibility)GetValue(ScrollDownButtonVisibilityProperty); }
			set { SetValue(ScrollDownButtonVisibilityProperty, value); }
		}
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public double ViewerViewportWidth {
			get { return (double)GetValue(ViewerViewportWidthProperty); }
			set { SetValue(ViewerViewportWidthProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			AssociatedObject.ScrollChanged += ScrollViewer_ScrollingChanged;
			AssociatedObject.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
			BindingOperations.SetBinding(this, ScrollableHeightProperty, new Binding("ScrollableHeight") { Source = AssociatedObject, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, InternalViewportWidthProperty, new Binding("ViewportWidth") { Source = AssociatedObject, Mode = BindingMode.OneWay });
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.ScrollChanged -= ScrollViewer_ScrollingChanged;
			AssociatedObject.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
			BindingOperations.ClearBinding(this, ScrollableHeightProperty);
			BindingOperations.ClearBinding(this, InternalViewportWidthProperty);
		}
		void ScrollViewer_ScrollingChanged(object sender, ScrollChangedEventArgs e) {
			CheckScrollButtonsVisibility();
		}
		void OnScrollableHeightChanged() {
			CheckScrollButtonsVisibility();
		}
		void OnIsCompactChanged() {
			if (AssociatedObject == null) return;
			if (IsCompact) {
				AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			} else {
				AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			}
			CheckScrollButtonsVisibility();
		}
		void CheckScrollButtonsVisibility() {
			if (AssociatedObject == null) return;
			if (IsCompact) {
				ScrollUpButtonVisibility = (double)GetValue(ScrollableHeightProperty) == 0 || AssociatedObject.VerticalOffset == 0 ? Visibility.Collapsed : Visibility.Visible;
				ScrollDownButtonVisibility = (double)GetValue(ScrollableHeightProperty) == 0 || AssociatedObject.VerticalOffset == AssociatedObject.ScrollableHeight ? Visibility.Collapsed : Visibility.Visible;
			} else {
				ScrollUpButtonVisibility = Visibility.Collapsed;
				ScrollDownButtonVisibility = Visibility.Collapsed;
			}
		}
		void OnInternalViewportWidthChanged(DependencyPropertyChangedEventArgs e) {
			SetValue(ViewerViewportWidthProperty, e.NewValue);
		}
		void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			e.Handled = true;
			var mouseEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
			mouseEventArgs.RoutedEvent = UIElement.MouseWheelEvent;
			AssociatedObject.RaiseEvent(mouseEventArgs);
		}
	}
}
