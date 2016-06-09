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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core {
	public enum TabbedWindowMode { Normal, Compact }
	public class DXTabbedWindow : DXWindow, ICloneable {
		public static readonly DependencyProperty TabbedWindowModeProperty = DependencyProperty.Register("TabbedWindowMode", typeof(TabbedWindowMode), typeof(DXTabbedWindow),
			new FrameworkPropertyMetadata(TabbedWindowMode.Compact, (d, e) => ((DXTabbedWindow)d).OnTabbedWindowModeChanged()));
		public static readonly DependencyProperty HeaderIndentInNormalStateProperty =
			DependencyProperty.Register("HeaderIndentInNormalState", typeof(double), typeof(DXTabbedWindow), new PropertyMetadata(14d));
		public static readonly DependencyProperty HeaderIndentInMaximizedStateProperty =
			DependencyProperty.Register("HeaderIndentInMaximizedState", typeof(double), typeof(DXTabbedWindow), new PropertyMetadata(4d));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty CaptionActualWidthProperty = DependencyProperty.Register("CaptionActualWidth", typeof(double), typeof(DXTabbedWindow),
			new FrameworkPropertyMetadata(0d, (d, e) => ((DXTabbedWindow)d).OnCaptionAndButtonContainerActualWidthChanged()));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ButtonContainerActualWidthProperty = DependencyProperty.Register("ButtonContainerActualWidth", typeof(double), typeof(DXTabbedWindow),
			new FrameworkPropertyMetadata(0d, (d, e) => ((DXTabbedWindow)d).OnCaptionAndButtonContainerActualWidthChanged()));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyPropertyKey TabHeaderSizePropertyKey =
			DependencyProperty.RegisterReadOnly("TabHeaderSize", typeof(double), typeof(DXTabbedWindow), new PropertyMetadata(0d));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty TabHeaderSizeProperty = TabHeaderSizePropertyKey.DependencyProperty;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public double TabHeaderSize { get { return (double)GetValue(TabHeaderSizeProperty); } private set { SetValue(TabHeaderSizePropertyKey, value); } }
		DXTabControl tabControl;
		TabPanelContainer tabPanel;
		public DXTabControl TabControl {
			get { return tabControl; }
			private set {
				if(tabControl == value) return;
				var old = tabControl;
				tabControl = value;
				OnTabControlChanged(old);
			}
		}
		TabPanelContainer TabPanel {
			get { return tabPanel; }
			set {
				if(tabPanel == value) return;
				var old = tabPanel;
				tabPanel = value;
				OnTabPanelChanged(old);
			}
		}
		public TabbedWindowMode TabbedWindowMode { get { return (TabbedWindowMode)GetValue(TabbedWindowModeProperty); } set { SetValue(TabbedWindowModeProperty, value); } }
		public double HeaderIndentInNormalState { get { return (double)GetValue(HeaderIndentInNormalStateProperty); } set { SetValue(HeaderIndentInNormalStateProperty, value); } }
		public double HeaderIndentInMaximizedState { get { return (double)GetValue(HeaderIndentInMaximizedStateProperty); } set { SetValue(HeaderIndentInMaximizedStateProperty, value); } }
		double CaptionActualWidth { get { return (double)GetValue(CaptionActualWidthProperty); } }
		double ButtonContainerActualWidth { get { return (double)GetValue(ButtonContainerActualWidthProperty); } }
		Size NormalStateSize { get; set; }
		static DXTabbedWindow() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXTabbedWindow), new FrameworkPropertyMetadata(typeof(DXTabbedWindow)));
		}
		public DXTabbedWindow() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SizeChanged += OnSizeChanged;
		}
		void OnCaptionAndButtonContainerActualWidthChanged() {
			if(TabbedWindowMode == TabbedWindowMode.Normal)
				TabControl.Do(x => x.SetValue(DXTabControl.PanelIndentProperty, new Thickness(0, 0, 0, 0)));
			else
				TabControl.Do(x => x.SetValue(DXTabControl.PanelIndentProperty, new Thickness(CaptionActualWidth, 0, ButtonContainerActualWidth, 0)));
		}
		void OnTabbedWindowModeChanged() {
			OnCaptionAndButtonContainerActualWidthChanged();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(WindowState == WindowState.Normal)
				NormalStateSize = e.NewSize;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			TabControl = null;
			SubscribeLayoutUpdated();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			NormalStateSize = new Size(Width, Height);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			TabControl = null;
			UnsubscribeLayoutUpdated();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			SubscribeLayoutUpdated();
		}
		void SubscribeLayoutUpdated() {
			UnsubscribeLayoutUpdated();
			LoadTabControl();
			if(TabControl == null)
				LayoutUpdated += OnLayoutUpdated;
		}
		void UnsubscribeLayoutUpdated() {
			LayoutUpdated -= OnLayoutUpdated;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LoadTabControl();
			if(TabControl != null)
				UnsubscribeLayoutUpdated();
			if(TabHeaderSize.IsZero())
				OnTabPanelSizeChanged(null, null);
		}
		void OnTabControlLayoutUpdated(object sender, EventArgs e) {
			LoadTabPanel();
			if(TabPanel != null)
				TabControl.LayoutUpdated -= OnTabControlLayoutUpdated;
		}
		void LoadTabControl() {
			TabControl = LayoutTreeHelper.GetVisualChildren(this).OfType<DXTabControl>().FirstOrDefault();
		}
		void LoadTabPanel() {
			TabPanel = TabControl.Return(x => x.TabPanel, () => null);
		}
		void OnTabControlChanged(DXTabControl old) {
			old.Do(x => x.LayoutUpdated -= OnTabControlLayoutUpdated);
			TabPanel = null;
			if(TabControl == null) return;
			OnCaptionAndButtonContainerActualWidthChanged();
			LoadTabPanel();
			if(TabPanel == null)
				TabControl.LayoutUpdated += OnTabControlLayoutUpdated;
		}
		void OnTabPanelChanged(TabPanelContainer old) {
			old.Do(x => x.SizeChanged -= OnTabPanelSizeChanged);
			TabPanel.Do(x => x.SizeChanged += OnTabPanelSizeChanged);
			OnTabPanelSizeChanged(null, null);
		}
		void OnTabPanelSizeChanged(object sender, SizeChangedEventArgs e) {
			if(TabControl == null || TabControl.View == null) {
				TabHeaderSize = 0d;
				return;
			}
			if(TabControl.View.HeaderLocation == HeaderLocation.Top)
				TabHeaderSize = TabPanel.ActualHeight;
			else TabHeaderSize = 0d;
		}
		Size GetTitleSize() {
			var headerElement = LayoutTreeHelper.GetVisualChildren(this).OfType<FrameworkElement>().FirstOrDefault(x => x.Name == "PART_Header");
			if(headerElement == null)
				return new Size();
			return new Size(headerElement.ActualWidth, headerElement.ActualHeight);
		}
		internal static DXTabbedWindow CloneCore(Window baseWindow) {
			DXTabbedWindow res = new DXTabbedWindow();
			res.Background = baseWindow.Background;
			res.BorderBrush = baseWindow.BorderBrush;
			res.BorderThickness = baseWindow.BorderThickness;
			res.Foreground = baseWindow.Foreground;
			res.FontFamily = baseWindow.FontFamily;
			res.FontSize = baseWindow.FontSize;
			res.FontStretch = baseWindow.FontStretch;
			res.FontStyle = baseWindow.FontStyle;
			res.FontWeight = baseWindow.FontWeight;
			res.Height = baseWindow.Height;
			res.Width = baseWindow.Width;
			res.MaxHeight = baseWindow.MaxHeight;
			res.MaxWidth = baseWindow.MaxWidth;
			res.MinHeight = baseWindow.MinHeight;
			res.MinWidth = baseWindow.MinWidth;
			res.AllowsTransparency = baseWindow.AllowsTransparency;
			res.Cursor = baseWindow.Cursor;
			res.FlowDirection = baseWindow.FlowDirection;
			res.SnapsToDevicePixels = baseWindow.SnapsToDevicePixels;
			res.UseLayoutRounding = baseWindow.UseLayoutRounding;
			res.ResizeMode = baseWindow.ResizeMode;
			res.ShowInTaskbar = baseWindow.ShowInTaskbar;
			res.SizeToContent = baseWindow.SizeToContent;
			res.WindowStyle = baseWindow.WindowStyle;
			res.Language = baseWindow.Language;
			res.Icon = baseWindow.Icon;
			res.Tag = baseWindow.Tag;
			res.Title = baseWindow.Title;
			res.ToolTip = baseWindow.ToolTip;
			res.Topmost = baseWindow.Topmost;
			res.WindowState = WindowState.Normal;
			DXWindow baseDXWindow = baseWindow as DXWindow;
			if(baseDXWindow != null) {
				res.ShowTitle = baseDXWindow.ShowTitle;
				res.ShowIcon = baseDXWindow.ShowIcon;
				res.SmallIcon = baseDXWindow.SmallIcon;
			}
			DXTabbedWindow baseDXTabbedWindow = baseWindow as DXTabbedWindow;
			if(baseDXTabbedWindow != null) {
				res.TabbedWindowMode = baseDXTabbedWindow.TabbedWindowMode;
				res.HeaderIndentInNormalState = baseDXTabbedWindow.HeaderIndentInNormalState;
				res.HeaderIndentInMaximizedState = baseDXTabbedWindow.HeaderIndentInMaximizedState;
				res.Width = baseDXTabbedWindow.NormalStateSize.Width;
				res.Height = baseDXTabbedWindow.NormalStateSize.Height;
				var sz = baseDXTabbedWindow.GetTitleSize();
				res.Left = baseDXTabbedWindow.Left + sz.Height;
				res.Top = baseDXTabbedWindow.Top + sz.Height;
			}
			return res;
		}
		object ICloneable.Clone() {
			return CloneCore(this);
		}
		protected override void OnStateChanged(EventArgs e) {
			base.OnStateChanged(e);
		}
	}
}
namespace DevExpress.Xpf.Core.Native {
	public class DXTabbedWindowHeaderDecorator : Decorator {
		public static readonly DependencyProperty TabbedWindowModeProperty = DependencyProperty.Register("TabbedWindowMode", typeof(TabbedWindowMode), typeof(DXTabbedWindowHeaderDecorator), new PropertyMetadata(TabbedWindowMode.Compact, (d, e) => ((DXTabbedWindowHeaderDecorator)d).OnTabbedWindowModeChanged()));
		public TabbedWindowMode TabbedWindowMode { get { return (TabbedWindowMode)GetValue(TabbedWindowModeProperty); } set { SetValue(TabbedWindowModeProperty, value); } }
		public new FrameworkElement Child   { get { return (FrameworkElement)base.Child; } set { base.Child = value; } }
		public DXTabbedWindowHeaderDecorator() {
			OnTabbedWindowModeChanged();
		}
		protected override Size MeasureOverride(Size constraint) {
			var res = base.MeasureOverride(constraint);
			return TabbedWindowMode == TabbedWindowMode.Compact && ChildDesiredSizeIsZero() ? new Size() : res;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			return base.ArrangeOverride(arrangeSize);
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			OnTabbedWindowModeChanged();
		}
		void OnTabbedWindowModeChanged() {
			HorizontalAlignment = TabbedWindowMode == TabbedWindowMode.Compact ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
			Child.Do(x => FloatingContainerHeaderPanel.SetEnableLayoutCorrection(x, TabbedWindowMode != TabbedWindowMode.Compact));
		}
		bool ChildDesiredSizeIsZero() {
			if(Child == null) return true;
			double w = Child.DesiredSize.Width;
			double m = Child.Margin.Left + Child.Margin.Right;
			return (w - m).IsZero();
		}
	}
	public class DXTabbedWindowMarginConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var tabHeaderSize = (double)values[0];
			var windowMode = (TabbedWindowMode)values[1];
			if(windowMode == TabbedWindowMode.Normal)
				return new Thickness();
			return new Thickness(0, -(double)tabHeaderSize, 0, 0);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DXTabbedWindowHeaderSizeConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var tabHeaderSize = (double)values[0];
			var headerIndentInNormalState = (double)values[1];
			var headerIndentInMaximizedState = (double)values[2];
			var windowState = (WindowState)values[3];
			var windowMode = (TabbedWindowMode)values[4];
			if(windowMode == TabbedWindowMode.Normal)
				return tabHeaderSize;
			var coef = windowState == WindowState.Maximized ? headerIndentInMaximizedState : headerIndentInNormalState;
			return tabHeaderSize + coef;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
