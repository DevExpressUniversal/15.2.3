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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public enum LayoutOptions {
		Fixed,
		Fill,
		CalcSize
	}
	public class NavigationPanePanelBase : Panel {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavigationPanePanelBase), new FrameworkPropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));
		private static void OnOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			InvalidateMeasure();
		}
		protected NavigationPaneView NavigationPaneView { get; set; }
		protected virtual void UpdateNavigationPaneView() {
			if(NavigationPaneView == null) {
				NavigationPaneView = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<NavigationPaneView>(this);
				if(NavigationPaneView != null) {
					OnNavigationPaneViewChanged();
				}
			}
		}
		protected virtual void OnNavigationPaneViewChanged() {
			UpdateOrientationBinding();
		}
		protected void UpdateOrientationBinding() {
			Binding binding = new Binding("Orientation");
			binding.Source = NavigationPaneView;
			SetBinding(OrientationProperty, binding);
		}
		protected Size GetSize(Size size) {
			if(Orientation == Orientation.Vertical)
				return size;
			return new Size(size.Height, size.Width);
		}
		protected Size CreateSize(double width, double height) {
			if(Orientation == Orientation.Vertical)
				return new Size(width, height);
			return new Size(height, width);
		}
		protected double GetHeight(Size size) {
			if(Orientation == Orientation.Vertical)
				return size.Height;
			return size.Width;
		}
		protected double GetWidth(Size size) {
			if(Orientation == Orientation.Vertical)
				return size.Width;
			return size.Height;
		}
		protected Rect CreateRect(double x, double y, double width, double height) {
			if(Orientation == Orientation.Vertical)
				return new Rect(x, y, width, height);
			return new Rect(y, x, height, width);
		}
	}
	public class NavigationPanePanel : NavigationPanePanelBase {
		public static readonly DependencyProperty HeaderProperty = DependencyPropertyManager.Register("Header", typeof(FrameworkElement), typeof(NavigationPanePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHeaderChanged)));
		public static readonly DependencyProperty ActiveGroupProperty = DependencyPropertyManager.Register("ActiveGroup", typeof(FrameworkElement), typeof(NavigationPanePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveGroupChanged)));
		public static readonly DependencyProperty SplitterProperty = DependencyPropertyManager.Register("Splitter", typeof(FrameworkElement), typeof(NavigationPanePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSplitterChanged)));
		public static readonly DependencyProperty GroupsControlProperty = DependencyPropertyManager.Register("GroupsControl", typeof(NavPaneGroupButtonPanelControl), typeof(NavigationPanePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupsControlChanged)));
		public static readonly DependencyProperty OverflowPanelProperty = DependencyPropertyManager.Register("OverflowPanel", typeof(FrameworkElement), typeof(NavigationPanePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnOverflowPanelChanged)));
		private static void OnHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnHeaderChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
		}
		private static void OnActiveGroupChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnActiveGroupChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
		}
		private static void OnSplitterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnSplitterChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
		}
		private static void OnGroupsControlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnGroupsControlChanged((NavPaneGroupButtonPanelControl)e.OldValue, (NavPaneGroupButtonPanelControl)e.NewValue);
		}
		private static void OnOverflowPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			NavigationPanePanel navigationPanePanel = o as NavigationPanePanel;
			if(navigationPanePanel != null)
				navigationPanePanel.OnOverflowPanelChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
		}
		public FrameworkElement Header {
			get { return (FrameworkElement)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public FrameworkElement ActiveGroup {
			get { return (FrameworkElement)GetValue(ActiveGroupProperty); }
			set { SetValue(ActiveGroupProperty, value); }
		}
		public FrameworkElement Splitter {
			get { return (FrameworkElement)GetValue(SplitterProperty); }
			set { SetValue(SplitterProperty, value); }
		}
		public NavPaneGroupButtonPanelControl GroupsControl {
			get { return (NavPaneGroupButtonPanelControl)GetValue(GroupsControlProperty); }
			set { SetValue(GroupsControlProperty, value); }
		}
		public FrameworkElement OverflowPanel {
			get { return (FrameworkElement)GetValue(OverflowPanelProperty); }
			set { SetValue(OverflowPanelProperty, value); }
		}
		protected virtual void OnHeaderChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		protected virtual void OnActiveGroupChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		protected virtual void OnSplitterChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		protected virtual void OnGroupsControlChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		protected virtual void OnOverflowPanelChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			Children.Remove(oldValue);
			if(newValue != null)
				Children.Add(newValue);
		}
		protected override void OnNavigationPaneViewChanged() {
			base.OnNavigationPaneViewChanged();
			if(NavigationPaneView != null)
				NavigationPaneView.Panel = this;
		}
		protected override Size MeasureOverride(Size availableSize) {
			UpdateNavigationPaneView();
			double availableHeight = GetHeight(availableSize);
			double availableWidth = GetWidth(availableSize);
			double totalHeight = 0;
			double width = 0;
			if(Header != null) {
				Header.Measure(CreateSize(availableWidth, double.PositiveInfinity));
				availableHeight -= GetHeight(Header.DesiredSize);
				availableHeight = Math.Max(availableHeight, 0);
				totalHeight += GetHeight(Header.DesiredSize);
				width = GetWidth(Header.DesiredSize);
			}
			if(Splitter != null) {
				Splitter.Measure(CreateSize(availableWidth, double.PositiveInfinity));
				availableHeight -= GetHeight(Splitter.DesiredSize);
				availableHeight = Math.Max(availableHeight, 0);
				totalHeight += GetHeight(Splitter.DesiredSize);
				width = Math.Max(width, GetWidth(Splitter.DesiredSize));
			}
			if(OverflowPanel != null) {
				OverflowPanel.Measure(CreateSize(availableWidth, double.PositiveInfinity));
				availableHeight -= GetHeight(OverflowPanel.DesiredSize);
				availableHeight = Math.Max(availableHeight, 0);
				totalHeight += GetHeight(OverflowPanel.DesiredSize);
				width = Math.Max(width, GetWidth(OverflowPanel.DesiredSize));
			}
			if(GroupsControl != null) {
				double activeGroupMinHeight = ActiveGroup != null? GetHeight(new Size(ActiveGroup.MinWidth, ActiveGroup.MinHeight)) : 0.0;
				GroupsControl.Measure(CreateSize(availableWidth, Math.Max(0,availableHeight - activeGroupMinHeight)));
				availableHeight -= GetHeight(GroupsControl.DesiredSize);
				availableHeight = Math.Max(availableHeight, 0);
				totalHeight += GetHeight(GroupsControl.DesiredSize);
				width = Math.Max(width, GetWidth(GroupsControl.DesiredSize));
			}
			if(ActiveGroup != null) {
				ActiveGroup.Measure(CreateSize(availableWidth, availableHeight));
				availableHeight -= GetHeight(ActiveGroup.DesiredSize);
				totalHeight += GetHeight(ActiveGroup.DesiredSize);
				width = Math.Max(width, GetWidth(ActiveGroup.DesiredSize));
			}
			return CreateSize(width, totalHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double availableHeight = GetHeight(finalSize);
			double finalHeight = availableHeight;
			double availableWidth = GetWidth(finalSize);
			double headerHeight = 0;
			if(Header != null) {
				headerHeight = GetHeight(Header.DesiredSize);
				Header.Arrange(CreateRect(0, 0, availableWidth, headerHeight));
				availableHeight -= headerHeight;
			}
			double overflowPanelHeight = 0;
			if(OverflowPanel != null) {
				overflowPanelHeight = GetHeight(OverflowPanel.DesiredSize);
				OverflowPanel.Arrange(CreateRect(0, finalHeight - overflowPanelHeight, availableWidth, overflowPanelHeight));
				availableHeight -= overflowPanelHeight;
			}
			double groupsControlHeight = 0;
			if(GroupsControl != null) {
				groupsControlHeight = GetHeight(GroupsControl.DesiredSize);
				GroupsControl.Arrange(CreateRect(0, finalHeight - overflowPanelHeight - groupsControlHeight, availableWidth, groupsControlHeight));
				availableHeight -= groupsControlHeight;
			}
			double splitterHeight = 0;
			if(Splitter != null) {
				splitterHeight = GetHeight(Splitter.DesiredSize);
				Splitter.Arrange(CreateRect(0, finalHeight - overflowPanelHeight - groupsControlHeight - splitterHeight, availableWidth, splitterHeight));
				availableHeight -= splitterHeight;
			}
			if(ActiveGroup != null) {
				ActiveGroup.Arrange(CreateRect(0, headerHeight, availableWidth, availableHeight));
			}
			return finalSize;
		}
	}
	public class NavPaneDefaultIcon : ContentControl {
		static NavPaneDefaultIcon() {
			FocusableProperty.OverrideMetadata(typeof(NavPaneDefaultIcon), new FrameworkPropertyMetadata(false));
		}
		public NavPaneDefaultIcon() {
			this.SetDefaultStyleKey(typeof(NavPaneDefaultIcon));
		}
	}
	[TemplateVisualState(Name = "WithSplitter", GroupName = "SplitterBehaviorStates"), TemplateVisualState(Name = "WithoutSplitter", GroupName = "SplitterBehaviorStates")]
	public class NavPaneOverflowPanel : ContentControl {
		internal static readonly DependencyProperty IsSplitterVisibleProperty;
		internal static readonly DependencyProperty ItemsControlGroupCountProperty;
		public static readonly DependencyProperty ShowBorderProperty;		
		static NavPaneOverflowPanel() {
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavPaneOverflowPanel), new FrameworkPropertyMetadata(true, (d, e) => ((NavPaneOverflowPanel)d).OnShowBorderChanged((bool)e.OldValue)));
			IsSplitterVisibleProperty = DependencyPropertyManager.Register("IsSplitterVisible", typeof(bool), typeof(NavPaneOverflowPanel), new PropertyMetadata(true, (d, e) => ((NavPaneOverflowPanel)d).OnIsSplitterVisiblePropertyChanged()));
			ItemsControlGroupCountProperty = DependencyPropertyManager.Register("ItemsControlGroupCount", typeof(int), typeof(NavPaneOverflowPanel), new PropertyMetadata(0, (d, e) => ((NavPaneOverflowPanel)d).OnItemsControlGroupCountPropertyChanged()));
		}
		public NavPaneOverflowPanel() {
			this.SetDefaultStyleKey(typeof(NavPaneOverflowPanel));
		}
		internal bool IsSplitterVisible {
			get { return (bool)GetValue(IsSplitterVisibleProperty); }
			set { SetValue(IsSplitterVisibleProperty, value); }
		}
		internal int ItemsControlGroupCount {
			get { return (int)GetValue(ItemsControlGroupCountProperty); }
			set { SetValue(ItemsControlGroupCountProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBindings();
		}
		void OnItemsControlGroupCountPropertyChanged() {
			UpdateSplitterBehaviorStates();
		}
		void OnIsSplitterVisiblePropertyChanged() {
			UpdateSplitterBehaviorStates();
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			UpdateShowBorderStates();
		}
		void SetBindings() {
			SetBinding(IsSplitterVisibleProperty, new Binding("View.IsSplitterVisible"));
			SetBinding(ItemsControlGroupCountProperty, new Binding("View.ItemsControlGroupCount"));
			SetBinding(ShowBorderProperty, new Binding("View.ShowBorder"));
		}
		void UpdateSplitterBehaviorStates() {
			VisualStateManager.GoToState(this, (!IsSplitterVisible && ItemsControlGroupCount == 0 && ShowBorder) ? "WithoutSplitter" : "WithSplitter", false);
		}
		void UpdateShowBorderStates() {
			VisualStateManager.GoToState(this, ShowBorder ? "WithBorder" : "WithoutBorder", false);
		}
	}
}
