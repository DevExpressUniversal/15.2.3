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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
#if !SL
	public class SchedulerScrollBar : ScrollBar {
		static SchedulerScrollBar() {
			EventManager.RegisterClassHandler(typeof(SchedulerScrollBar), CommandManager.PreviewExecutedEvent, new RoutedEventHandler(OnScrollBarCommandPreviewExecuted));
			EventManager.RegisterClassHandler(typeof(SchedulerScrollBar), CommandManager.ExecutedEvent, new RoutedEventHandler(OnScrollBarCommandExecuted));
		}
		static void OnScrollBarCommandPreviewExecuted(object target, RoutedEventArgs e) {
			SchedulerScrollBar scrollBar = target as SchedulerScrollBar;
			ICommand command = GetExecutedCommand(e);
			if (command != null && scrollBar != null)
				scrollBar.OnScrollBarCommandPreviewExecuted(command);
		}
		static void OnScrollBarCommandExecuted(object target, RoutedEventArgs e) {
			SchedulerScrollBar scrollBar = target as SchedulerScrollBar;
			ICommand command = GetExecutedCommand(e);
			if (command != null && scrollBar != null)
				scrollBar.OnScrollBarCommandExecuted(command);
		}
		static ICommand GetExecutedCommand(RoutedEventArgs e) {
			RepeatButton button = e.OriginalSource as RepeatButton;
			if (button == null && button.Command != null)
				return null;
			return button.Command;
		}
		SchedulerControl control;
		public SchedulerScrollBar() {
			DefaultStyleKey = typeof(SchedulerScrollBar);
			this.Loaded += new RoutedEventHandler(SchedulerScrollBar_Loaded);
			this.Unloaded += new RoutedEventHandler(SchedulerScrollBar_Unloaded);
		}
		void OnScrollBarCommandExecuted(ICommand command) {
			if (IsDecrimentValueCommand(command))
				RaiseEvent(new ScrollEventArgs(ScrollEventType.EndScroll, Value));
			if (IsIncrementValueCommand(command))
				RaiseEvent(new ScrollEventArgs(ScrollEventType.EndScroll, Value));
		}
		void OnScrollBarCommandPreviewExecuted(ICommand command) {
			if (IsDecrimentValueCommand(command) && Value == Minimum)
				RaiseEvent(new ScrollEventArgs(ScrollEventType.SmallDecrement, Minimum));
			if (IsIncrementValueCommand(command) && Value == Maximum)
				RaiseEvent(new ScrollEventArgs(ScrollEventType.SmallIncrement, Maximum));
		}
		bool IsIncrementValueCommand(ICommand command) {
			return Object.ReferenceEquals(command, ScrollBar.LineDownCommand) || Object.ReferenceEquals(command, ScrollBar.LineRightCommand);
		}
		bool IsDecrimentValueCommand(ICommand command) {
			return Object.ReferenceEquals(command, ScrollBar.LineUpCommand) || Object.ReferenceEquals(command, ScrollBar.LineLeftCommand);
		}
		void SchedulerScrollBar_Loaded(object sender, RoutedEventArgs e) {
			control = GetOuterSchedulerControl();
			if (control != null)
				control.SetValue(SchedulerControl.DateTimeScrollBarProperty, new ScrollBarWrapper(control, this));
		}
		void SchedulerScrollBar_Unloaded(object sender, RoutedEventArgs e) {
			if (control != null)
				control.SetValue(SchedulerControl.DateTimeScrollBarProperty, null);
		}
		protected virtual SchedulerControl GetOuterSchedulerControl() {
			DependencyObject current = this;
			while (current != null) {
				current = VisualTreeHelper.GetParent(current);
				SchedulerControl control = current as SchedulerControl;
				if (control != null)
					return control;
			}
			return null;
		}
	}
	#region ScrollBarWrapper
	public class ScrollBarWrapper : DependencyObject, IDisposable {
		static ScrollBarWrapper() {
			EventManager.RegisterClassHandler(typeof(ScrollBar), CommandManager.PreviewExecutedEvent, new RoutedEventHandler(OnScrollBarCommandPreviewExecuted));
			EventManager.RegisterClassHandler(typeof(ScrollBar), CommandManager.ExecutedEvent, new RoutedEventHandler(OnScrollBarCommandExecuted));
		}
		static void OnScrollBarCommandPreviewExecuted(object target, RoutedEventArgs e) {
			ScrollBar scrollBar = target as ScrollBar;
			ScrollBarWrapper wrapper = SchedulerScrollBarManager.GetScrollBarWrapper(scrollBar);
			if (wrapper != null)
				wrapper.OnScrollBarCommandPreviewExecuted(e);
		}
		static void OnScrollBarCommandExecuted(object target, RoutedEventArgs e) {
			ScrollBar scrollBar = target as ScrollBar;
			ScrollBarWrapper wrapper = SchedulerScrollBarManager.GetScrollBarWrapper(scrollBar);
			if (wrapper != null)
				wrapper.OnScrollBarCommandExecuted(e);
		}
		ScrollBar scrollBar;
		SchedulerControl control;
		public ScrollBarWrapper(SchedulerControl control, ScrollBar scrollBar) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(scrollBar, "scrollBar");
			this.control = control;
			this.scrollBar = scrollBar;
			SubscribeToScrollBarEvents();
		}
		#region Scroll event
		ScrollEventHandler onScroll;
		public event ScrollEventHandler Scroll { add { onScroll += value; } remove { onScroll -= value; } }
		protected virtual void OnScroll(object sender, ScrollEventArgs e) {
			ScrollEventHandler handler = onScroll;
			if (handler != null)
				handler(sender, e);
		}
		#endregion
		#region Properties
		public ScrollBar ScrollBar { get { return scrollBar; } }
		public SchedulerControl Control { get { return control; } }
		public double Value { get { return scrollBar.Value; } set { scrollBar.Value = value; } }
		public double Minimum { get { return scrollBar.Minimum; } set { scrollBar.Minimum = value; } }
		public double Maximum { get { return scrollBar.Maximum; } set { scrollBar.Maximum = value; } }
		public double SmallChange { get { return scrollBar.SmallChange; } set { scrollBar.SmallChange = value; } }
		public double LargeChange { get { return scrollBar.LargeChange; } set { scrollBar.LargeChange = value; } }
		public double ViewportSize { get { return scrollBar.ViewportSize; } set { scrollBar.ViewportSize = value; } }
		public Visibility Visibility { get { return scrollBar.Visibility; } set { scrollBar.Visibility = value; } }
		#endregion
		protected virtual void SubscribeToScrollBarEvents() {
			this.scrollBar.Scroll += OnScroll;
		}
		protected virtual void UnsubscribeToScrollBarEvents() {
			this.scrollBar.Scroll -= OnScroll;
		}
		protected virtual void OnScrollBarCommandPreviewExecuted(RoutedEventArgs e) {
			ICommand command = GetExecutedCommand(e);
			if (command == null)
				return;
			if (Value == Minimum && IsDecrimentValueCommand(command))
				RaiseScrollChangedEvent(scrollBar, ScrollEventType.SmallDecrement);
			else if (Value == Maximum && IsIncrementValueCommand(command))
				RaiseScrollChangedEvent(scrollBar, ScrollEventType.SmallIncrement);
		}
		protected virtual void OnScrollBarCommandExecuted(RoutedEventArgs e) {
			ICommand command = GetExecutedCommand(e);
			if (command != null)
				RaiseEndScrollEvent(scrollBar);
		}
		protected virtual ICommand GetExecutedCommand(RoutedEventArgs e) {
			RepeatButton button = e.OriginalSource as RepeatButton;
			return button != null ? button.Command : null;
		}
		void RaiseScrollChangedEvent(ScrollBar scrollBar, ScrollEventType type) {
			scrollBar.RaiseEvent(new ScrollEventArgs(type, scrollBar.Value));
		}
		void RaiseEndScrollEvent(ScrollBar scrollBar) {
			scrollBar.RaiseEvent(new ScrollEventArgs(ScrollEventType.EndScroll, scrollBar.Value));
		}
		bool IsIncrementValueCommand(ICommand command) {
			return Object.ReferenceEquals(command, ScrollBar.LineDownCommand) || Object.ReferenceEquals(command, ScrollBar.LineRightCommand);
		}
		bool IsDecrimentValueCommand(ICommand command) {
			return Object.ReferenceEquals(command, ScrollBar.LineUpCommand) || Object.ReferenceEquals(command, ScrollBar.LineLeftCommand);
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollBar != null) {
					UnsubscribeToScrollBarEvents();
					this.scrollBar = null;
				}
			}
		}
		#endregion
	}
	#endregion
	public class SchedulerScrollViewer : ScrollViewer, ILogicalOwner {
		const string VerticalScrollBarTemplateName = "PART_VerticalScrollBar";
		public static readonly DependencyProperty VerticalScrollBarSizeGroupProperty;
		static SchedulerScrollViewer() {
			VerticalScrollBarSizeGroupProperty = DependencyPropertyHelper.RegisterProperty<SchedulerScrollViewer, string>("VerticalScrollBarSizeGroup", null);
		}
		public SchedulerScrollViewer() {
			DefaultStyleKey = typeof(SchedulerScrollViewer);
		}
		#region VerticalScrollBarSizeGroup
		public string VerticalScrollBarSizeGroup {
			get { return (string)GetValue(VerticalScrollBarSizeGroupProperty); }
			set { SetValue(VerticalScrollBarSizeGroupProperty, value); }
		}
		#endregion
		public FrameworkElement GetVerticalScrollBar() {
			return (FrameworkElement)GetTemplateChild(VerticalScrollBarTemplateName);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = false;
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			e.Handled = false;
		}
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		#endregion
	}
#else
	public class ScrollBarWrapper : DependencyObject, IDisposable {
		ScrollBar scrollBar;
		SchedulerControl control;
		public ScrollBarWrapper(SchedulerControl control, ScrollBar scrollBar) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(scrollBar, "scrollBar");
			this.control = control;
			this.scrollBar = scrollBar;
			this.scrollBar.LayoutUpdated += OnScrollBarLayoutUpdated;
			SubscribeToScrollBarEvents();
			this.scrollBar.ApplyTemplate();
			AssignElements();
		}
	#region Scroll event
		ScrollEventHandler onScroll;
		public event ScrollEventHandler Scroll { add { onScroll += value; } remove { onScroll -= value; } }
		protected virtual void OnScroll(object sender, ScrollEventArgs e) {
			ScrollEventHandler handler = onScroll;
			if (handler != null)
				handler(sender, e);
		}
		#endregion
		public SchedulerControl Control { get { return control; } }
		public ScrollBar ScrollBar { get { return scrollBar; } }
		public double Value { get { return scrollBar.Value; } set { scrollBar.Value = value; } }
		public double Minimum { get { return scrollBar.Minimum; } set { scrollBar.Minimum = value; } }
		public double Maximum { get { return scrollBar.Maximum; } set { scrollBar.Maximum = value; } }
		public double SmallChange { get { return scrollBar.SmallChange; } set { scrollBar.SmallChange = value; } }
		public double LargeChange { get { return scrollBar.LargeChange; } set { scrollBar.LargeChange = value; } }
		public double ViewportSize { get { return scrollBar.ViewportSize; } set { scrollBar.ViewportSize = value; } }
		public Visibility Visibility { get { return scrollBar.Visibility; } set { scrollBar.Visibility = value; } }
		protected RepeatButton ElementHorizontalLargeIncrease { get; set; }
		protected RepeatButton ElementHorizontalLargeDecrease { get; set; }
		protected RepeatButton ElementHorizontalSmallIncrease { get; set; }
		protected RepeatButton ElementHorizontalSmallDecrease { get; set; }
		protected RepeatButton ElementVerticalLargeIncrease { get; set; }
		protected RepeatButton ElementVerticalLargeDecrease { get; set; }
		protected RepeatButton ElementVerticalSmallIncrease { get; set; }
		protected RepeatButton ElementVerticalSmallDecrease { get; set; }
		void OnScrollBarLayoutUpdated(object sender, EventArgs e) {
			if (this.scrollBar != null) {
				this.scrollBar.LayoutUpdated -= OnScrollBarLayoutUpdated;
				AssignElements();
				SubscribeToElementEvents();
			}
		}
		void AssignElements() {
			ElementHorizontalLargeIncrease = LayoutHelper.FindElementByName(scrollBar, "HorizontalLargeIncrease") as RepeatButton;
			ElementHorizontalLargeDecrease = LayoutHelper.FindElementByName(scrollBar, "HorizontalLargeDecrease") as RepeatButton;
			ElementHorizontalSmallIncrease = LayoutHelper.FindElementByName(scrollBar, "HorizontalSmallIncrease") as RepeatButton;
			ElementHorizontalSmallDecrease = LayoutHelper.FindElementByName(scrollBar, "HorizontalSmallDecrease") as RepeatButton;
			ElementVerticalLargeIncrease = LayoutHelper.FindElementByName(scrollBar, "VerticalLargeIncrease") as RepeatButton;
			ElementVerticalLargeDecrease = LayoutHelper.FindElementByName(scrollBar, "VerticalLargeDecrease") as RepeatButton;
			ElementVerticalSmallIncrease = LayoutHelper.FindElementByName(scrollBar, "VerticalSmallIncrease") as RepeatButton;
			ElementVerticalSmallDecrease = LayoutHelper.FindElementByName(scrollBar, "VerticalSmallDecrease") as RepeatButton;
		}
		void SubscribeToElementEvents() {
			ElementHorizontalLargeIncrease.Click += OnLargeIncrease;
			ElementHorizontalLargeDecrease.Click += OnLargeDecrease;
			ElementHorizontalSmallIncrease.Click += OnSmallIncrease;
			ElementHorizontalSmallDecrease.Click += OnSmallDecrease;
			ElementVerticalLargeIncrease.Click += OnLargeIncrease;
			ElementVerticalLargeDecrease.Click += OnLargeDecrease;
			ElementVerticalSmallIncrease.Click += OnSmallIncrease;
			ElementVerticalSmallDecrease.Click += OnSmallDecrease;
		}
		void UnsubscribeToElementEvents() {
			ElementHorizontalLargeIncrease.Click -= OnLargeIncrease;
			ElementHorizontalLargeDecrease.Click -= OnLargeDecrease;
			ElementHorizontalSmallIncrease.Click -= OnSmallIncrease;
			ElementHorizontalSmallDecrease.Click -= OnSmallDecrease;
			ElementVerticalLargeIncrease.Click -= OnLargeIncrease;
			ElementVerticalLargeDecrease.Click -= OnLargeDecrease;
			ElementVerticalSmallIncrease.Click -= OnSmallIncrease;
			ElementVerticalSmallDecrease.Click -= OnSmallDecrease;
		}
		void OnSmallDecrease(object sender, System.Windows.RoutedEventArgs e) {
			if (Value == Minimum)
				OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, Value));
			OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.EndScroll, Value));
		}
		void OnSmallIncrease(object sender, System.Windows.RoutedEventArgs e) {
			if (Value == Maximum)
				OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, Value));
			OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.EndScroll, Value));
		}
		void OnLargeDecrease(object sender, System.Windows.RoutedEventArgs e) {
			if (Value == Minimum)
				OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.LargeDecrement, Value));
			OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.EndScroll, Value));
		}
		void OnLargeIncrease(object sender, System.Windows.RoutedEventArgs e) {
			if (Value == Maximum)
				OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.LargeIncrement, Value));
			OnScroll(scrollBar, new ScrollEventArgs(ScrollEventType.EndScroll, Value));
		}
		protected virtual void SubscribeToScrollBarEvents() {
			this.scrollBar.Scroll += OnScroll;
		}
		protected virtual void UnsubscribeToScrollBarEvents() {
			this.scrollBar.Scroll -= OnScroll;
		}
	#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollBar != null) {
					UnsubscribeToScrollBarEvents();
					UnsubscribeToElementEvents();
					this.scrollBar = null;
				}
			}
		}
		#endregion
	}
#endif
	#region SchedulerScrollBarManager
	public class SchedulerScrollBarManager : DependencyObject {
		#region BindToSchedulerControl
		public static readonly DependencyProperty BindToSchedulerControlProperty = CreateBindToSchedulerControlProperty();
		static DependencyProperty CreateBindToSchedulerControlProperty() {
			return DependencyProperty.RegisterAttached("BindToSchedulerControl", typeof(Boolean), typeof(SchedulerScrollBarManager), new PropertyMetadata(false, OnBindToSchedulerControlChanged));
		}
		static void OnBindToSchedulerControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ScrollBar scrollBar = d as ScrollBar;
			if (scrollBar == null)
				return;
			OnBindToSchedulerControlChanged(scrollBar, (bool)e.NewValue);
		}
		public static bool GetBindToSchedulerControl(UIElement element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(BindToSchedulerControlProperty);
		}
		public static void SetBindToSchedulerControl(UIElement element, bool value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(BindToSchedulerControlProperty, value);
		}
		#endregion
		#region ScrollBarWrapper
		public static readonly DependencyProperty ScrollBarWrapperProperty = CreateScrollBarWrapperProperty();
		static DependencyProperty CreateScrollBarWrapperProperty() {
			return DependencyProperty.RegisterAttached("ScrollBarWrapper", typeof(ScrollBarWrapper), typeof(SchedulerScrollBarManager), new PropertyMetadata(null));
		}
		public static ScrollBarWrapper GetScrollBarWrapper(UIElement element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (ScrollBarWrapper)element.GetValue(ScrollBarWrapperProperty);
		}
		public static void SetScrollBarWrapper(UIElement element, ScrollBarWrapper value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ScrollBarWrapperProperty, value);
		}
		#endregion
		static void OnBindToSchedulerControlChanged(ScrollBar scrollBar, bool bindToSchedulerControl) {
			if (bindToSchedulerControl)
				SubscribeToScrollBarEvents(scrollBar);
			else
				UnsubscribeFromScrollBarEvents(scrollBar);
		}
		static void SubscribeToScrollBarEvents(ScrollBar scrollBar) {
			scrollBar.Loaded += OnScrollBarLoaded;
			scrollBar.Unloaded += OnScrollBarUnloaded;
		}
		static void UnsubscribeFromScrollBarEvents(ScrollBar scrollBar) {
			scrollBar.Loaded -= OnScrollBarLoaded;
			scrollBar.Unloaded -= OnScrollBarUnloaded;
		}
		static SchedulerControl GetParentSchedulerControl(DependencyObject child) {
			while (child != null) {
				DependencyObject parent = VisualTreeHelper.GetParent(child);
				SchedulerControl control = parent as SchedulerControl;
				if (control != null)
					return control;
				child = parent;
			}
			return null;
		}
		static void BindScrollBarToSchedulerControl(SchedulerControl control, ScrollBar scrollBar) {
			ScrollBarWrapper wrapper = new ScrollBarWrapper(control, scrollBar);
			control.SetValue(SchedulerControl.DateTimeScrollBarProperty, wrapper);
			SetScrollBarWrapper(scrollBar, wrapper);
		}
		static void UnbindScrollBarFromSchedulerControl(SchedulerControl control, ScrollBar scrollBar) {
			if (control.DateTimeScrollBar != null && Object.ReferenceEquals(control.DateTimeScrollBar.ScrollBar, scrollBar))
				control.ClearValue(SchedulerControl.DateTimeScrollBarProperty);
			ScrollBarWrapper wrapper = GetScrollBarWrapper(scrollBar);
			XtraSchedulerDebug.Assert(Object.ReferenceEquals(wrapper.ScrollBar, scrollBar));
			wrapper.Dispose();
			SetScrollBarWrapper(scrollBar, null);
		}
		static void OnScrollBarLoaded(object sender, System.Windows.RoutedEventArgs e) {
			ScrollBar scrollBar = sender as ScrollBar;
			SchedulerControl control = GetParentSchedulerControl(scrollBar);
			if (control != null)
				BindScrollBarToSchedulerControl(control, scrollBar);
		}
		static void OnScrollBarUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			ScrollBar scrollBar = sender as ScrollBar;
			ScrollBarWrapper wrapper = GetScrollBarWrapper(scrollBar);
			if (wrapper != null)
				UnbindScrollBarFromSchedulerControl(wrapper.Control, scrollBar);
		}
	}
	#endregion
}
