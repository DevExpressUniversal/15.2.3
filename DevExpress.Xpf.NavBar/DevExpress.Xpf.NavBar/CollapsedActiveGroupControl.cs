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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.NavBar {
	public enum ShowMode { DefaultItem, Items, All, MaximizedDefaultItem };
	public class CollapsedActiveGroupControl : Control{
		NavigationPaneView view;
		public static readonly DependencyProperty ShowModeProperty;
		public static readonly DependencyProperty ItemsProperty;
		public static readonly DependencyProperty DefaultItemProperty;
		public static readonly DependencyProperty DefaultIsCheckedProperty;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty CollapsedNavPaneItemsSourceProperty;		
		static CollapsedActiveGroupControl() {
			ItemsProperty = DependencyPropertyManager.Register("Items", typeof(ItemsControl), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(null, (d, e) => ((CollapsedActiveGroupControl)d).OnItemsChanged((ItemsControl)e.OldValue)));
			DefaultItemProperty = DependencyPropertyManager.Register("DefaultItem", typeof(Control), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(null, (d, e) => ((CollapsedActiveGroupControl)d).OnDefaultItemChanged((Control)e.OldValue)));
			ShowModeProperty = DependencyPropertyManager.Register("ShowMode", typeof(ShowMode), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(ShowMode.MaximizedDefaultItem, new PropertyChangedCallback(OnShowModeChanged)));
			DefaultIsCheckedProperty = DependencyPropertyManager.Register("DefaultIsChecked", typeof(bool), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(false));
			GroupProperty = DependencyPropertyManager.Register("Group", typeof(NavBarGroup), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((CollapsedActiveGroupControl)d).OnGroupChanged((NavBarGroup)e.OldValue, (NavBarGroup)e.NewValue))));
			CollapsedNavPaneItemsSourceProperty = DependencyPropertyManager.Register("CollapsedNavPaneItemsSource", typeof(IEnumerable<object>), typeof(CollapsedActiveGroupControl), new FrameworkPropertyMetadata(null));
		}
		static void OnShowModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CollapsedActiveGroupControl)d).OnShowModeChanged();
		}
		public IEnumerable<object> CollapsedNavPaneItemsSource {
			get { return (IEnumerable<object>)GetValue(CollapsedNavPaneItemsSourceProperty); }
			set { SetValue(CollapsedNavPaneItemsSourceProperty, value); }
		}
		public NavBarGroup Group {
			get { return (NavBarGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public ItemsControl Items {
			get { return (ItemsControl)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		public Control DefaultItem {
			get { return (Control)GetValue(DefaultItemProperty); }
			set { SetValue(DefaultItemProperty, value); }
		}
		public ShowMode ShowMode {
			get { return (ShowMode)GetValue(ShowModeProperty); }
			set { SetValue(ShowModeProperty, value); }
		}
		public bool DefaultIsChecked {
			get { return (bool)GetValue(DefaultIsCheckedProperty); }
			set { SetValue(DefaultIsCheckedProperty, value); }
		}		
		public NavigationPaneView View {
			get {
				if (view == null)
					view = LayoutHelper.FindParentObject<NavigationPaneView>(this as DependencyObject);
				return view;
			}
			set { view = value; }
		}
		public Control LegacyDefaultItem { get; set; }
		protected virtual void OnItemsChanged(ItemsControl oldValue) {
		}
		protected virtual void OnDefaultItemChanged(Control oldValue) {
		}
		protected virtual void OnDefaultItemActualIsVisibleChanged(bool oldValue) {
		}
		protected virtual void OnItemsActualIsVisibleChanged(bool oldValue) {
		}
		protected virtual void OnGroupChanged(NavBarGroup oldGroup, NavBarGroup newGroup) {
			if (newGroup == null) {
				ClearValue(CollapsedNavPaneItemsSourceProperty);
				ClearValue(ShowModeProperty);
			}
			else {
				SetBinding(CollapsedNavPaneItemsSourceProperty, new Binding("CollapsedNavPaneItemsSource") { Source = newGroup });
				SetBinding(ShowModeProperty, new Binding("NavPaneShowMode") { Source = newGroup });
			}
		}		
		protected virtual void OnShowModeChanged() {
			if (DefaultItem == null)
				return;
			((CollapsedActiveGroupDefaultElement)DefaultItem).IsChecked = false;
			switch (ShowMode) {
				case ShowMode.MaximizedDefaultItem:
					LegacyDefaultItem.Visibility = System.Windows.Visibility.Visible;
					DefaultItem.Visibility = System.Windows.Visibility.Collapsed;
					Items.Visibility = System.Windows.Visibility.Collapsed;
					break;
				case ShowMode.DefaultItem:
					DefaultItem.Visibility = System.Windows.Visibility.Visible;
					Items.Visibility = System.Windows.Visibility.Collapsed;
					LegacyDefaultItem.Visibility = System.Windows.Visibility.Collapsed;
					break;
				case ShowMode.Items:
					DefaultItem.Visibility = System.Windows.Visibility.Collapsed;
					Items.Visibility = System.Windows.Visibility.Visible;
					LegacyDefaultItem.Visibility = System.Windows.Visibility.Collapsed;
					break;
				case ShowMode.All:
					DefaultItem.Visibility = System.Windows.Visibility.Visible;
					Items.Visibility = System.Windows.Visibility.Visible;
					LegacyDefaultItem.Visibility = System.Windows.Visibility.Collapsed;
					break;
			}
		}
		protected internal void UpdateShowMode() {
			if (View == null || View.NavBar == null || View.NavBar.ActiveGroup == null)
				return;
			ShowMode = View.NavBar.ActiveGroup.NavPaneShowMode;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DefaultItem = LayoutHelper.FindElementByName(this, "defaultItem") as CollapsedActiveGroupDefaultElement;
			LegacyDefaultItem = LayoutHelper.FindElementByName(this, "legacyDefaultItem") as CollapsedActiveGroup;
			Items = LayoutHelper.FindElementByName(this, "items") as CollapsedActiveGroupItemsControl;
			UpdateShowMode();
			OnShowModeChanged();
			SetBinding(OpacityProperty, new Binding("NavBar.View.Expander.AnimationProgress") { Converter = new DoubleInvertConverter() { MaxValue = 1 } });
			SetBinding(DefaultIsCheckedProperty, new Binding("NavBar.View.IsPopupOpen") { Mode = BindingMode.TwoWay });
		}
	}
	public class CollapsedActiveGroupItemsPanel : StackPanel {
		public CollapsedActiveGroupControl ParentControl { get; set; }
		protected override Size MeasureOverride(Size constraint) {
			if (Orientation == Orientation.Horizontal)
				return MeasureHorizontal(constraint);
			return base.MeasureOverride(constraint);			
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if (Orientation == Orientation.Horizontal)
				return ArrangeHorizontal(arrangeSize);
			return base.ArrangeOverride(arrangeSize);
		}
		protected virtual Size MeasureHorizontal(Size constraint) {
			ParentControl.DefaultItem.Measure(constraint);
			Double width = 0;
			Double heigth = ParentControl.DefaultItem.DesiredSize.Height;
			for (int i = 0; i < Children.Count; i++) {
				Children[i].Opacity = 1;
				Children[i].IsHitTestVisible = true;
				Children[i].Measure(constraint);
				if (width + Children[i].DesiredSize.Width + ParentControl.DefaultItem.DesiredSize.Width <= constraint.Width) {
					width += Children[i].DesiredSize.Width;
					heigth = Math.Max(heigth, Children[i].DesiredSize.Height);
				}
				else {
					Children[i].Opacity = 0;
					Children[i].IsHitTestVisible = false;
				}
			}
			return new Size(width, heigth);
		}
		protected virtual Size ArrangeHorizontal(Size arrangeSize) {
			Point nextStart = new Point(arrangeSize.Width, 0);
			for (int i = 0; i < Children.Count; i++) {
				if (!Children[i].IsHitTestVisible && Children[i].Opacity == 0)
					continue;
				Children[i].Arrange(new Rect(
					new Point(nextStart.X - Children[i].DesiredSize.Width, 0), Children[i].DesiredSize
					));
				nextStart.X -= Children[i].DesiredSize.Width;
			}
			ParentControl.DefaultItem.Arrange(new Rect(new Point(nextStart.X - ParentControl.DefaultItem.DesiredSize.Width, 0), ParentControl.DefaultItem.DesiredSize));
			return arrangeSize;
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			ParentControl = LayoutHelper.FindParentObject<CollapsedActiveGroupControl>(this);
		}
	}
	public class CollapsedActiveGroupDefaultElement : CollapsedActiveGroupItem {		
		static CollapsedActiveGroupDefaultElement() {
			EventManager.RegisterClassHandler(typeof(CollapsedActiveGroupDefaultElement), MouseDownEvent, new MouseButtonEventHandler(OnLMouseDown), true);
		}
		public CollapsedActiveGroupDefaultElement() {
			AllowUncheck = true;
		}
		static void OnLMouseDown(object sender, MouseButtonEventArgs e) {
			var s = sender as CollapsedActiveGroupDefaultElement;
			if (s != null)
				s.OnLMouseDown(e);
		}
		void OnLMouseDown(MouseButtonEventArgs e) {
			if ((bool)IsChecked)
				e.Handled = true;			
		}
	}
	public class CollapsedActiveGroupItemsControl : ItemsControl {
		public static readonly DependencyProperty DefaultIsCheckedProperty;
		public static readonly DependencyProperty ShowModeProperty;
		public static readonly DependencyProperty DefaultElementProperty;
		static CollapsedActiveGroupItemsControl() {
			ShowModeProperty = DependencyPropertyManager.Register("ShowMode", typeof(ShowMode), typeof(CollapsedActiveGroupItemsControl), new FrameworkPropertyMetadata(ShowMode.MaximizedDefaultItem));
			DefaultIsCheckedProperty = DependencyPropertyManager.Register("DefaultIsChecked", typeof(bool), typeof(CollapsedActiveGroupItemsControl), new FrameworkPropertyMetadata(false));
			DefaultElementProperty = DependencyPropertyManager.Register("DefaultElement", typeof(CollapsedActiveGroupDefaultElement), typeof(CollapsedActiveGroupItemsControl), new FrameworkPropertyMetadata(null));
		}
		public ShowMode ShowMode {
			get { return (ShowMode)GetValue(ShowModeProperty); }
			set { SetValue(ShowModeProperty, value); }
		}
		public bool DefaultIsChecked {
			get { return (bool)GetValue(DefaultIsCheckedProperty); }
			set { SetValue(DefaultIsCheckedProperty, value); }
		}
		public CollapsedActiveGroupDefaultElement DefaultElement {
			get { return (CollapsedActiveGroupDefaultElement)GetValue(DefaultElementProperty); }
			set { SetValue(DefaultElementProperty, value); }
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new CollapsedActiveGroupItem();
		}
		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			CollapsedActiveGroupItem container = element as CollapsedActiveGroupItem;
			container.Group = DataContext as NavBarGroup;
			BindingOperations.SetBinding(container, CollapsedActiveGroupItem.IsCheckedProperty, new Binding("IsSelected") { Source = item, Mode = BindingMode.TwoWay });
			if (container != null) {
				container.Checked += OnContainerChecked;
				if (container.IsChecked.HasValue && container.IsChecked.Value)
					ContainerChecked(container);
			}				
		}
		CollapsedActiveGroupItem selectedItem;
		protected virtual void OnContainerChecked(object sender, RoutedEventArgs e) {
			var newItem = sender as CollapsedActiveGroupItem;
			ContainerChecked(newItem);
		}
		private void ContainerChecked(CollapsedActiveGroupItem newItem) {
			selectedItem.Do(x => x.Uncheck());
			selectedItem = newItem;
			((DataContext as NavBarGroup).NavBar.View as NavigationPaneView).CollapsedNavPaneSelectionStrategy.UpdateSelectedItem(DataContext as NavBarGroup);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			BindingOperations.ClearAllBindings(element);
			selectedItem = null;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
		}
	}
	public class CollapsedActiveGroupItem : ToggleButton {
		static CollapsedActiveGroupItem() {
			IsCheckedProperty.OverrideMetadata(typeof(CollapsedActiveGroupItem), new FrameworkPropertyMetadata(false, null, (d,e)=>((CollapsedActiveGroupItem)d).CoerceChecked((bool?)e)));
		}
		public CollapsedActiveGroupItem() {
			AllowUncheck = false;
		}
		readonly Locker uncheckLocker = new Locker();
		protected bool AllowUncheck { get; set; }
		public NavBarGroup Group { get; set; }
		object CoerceChecked(bool? newValue) {
			if (AllowUncheck)
				return newValue;
			if (uncheckLocker.IsLocked || !newValue.HasValue || newValue.Value == IsChecked)
				return newValue;
			return true;
		}
		public void Uncheck() {
			uncheckLocker.DoLockedAction(() => { IsChecked = false; });
		}
	}
	public class CollapsedActiveGroupItemContentPresenter : ContentControl {
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(CollapsedActiveGroupItemContentPresenter), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsPressedProperty =
			DependencyPropertyManager.Register("IsPressed", typeof(bool), typeof(CollapsedActiveGroupItemContentPresenter), new FrameworkPropertyMetadata(false));
	}
	[ValueConversion(typeof(NavBarGroup), typeof(NavBarItem))]
	public class NavBarGroupToNavBarItemConverter : MarkupExtension, IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var group = value as NavBarGroup;
			var item = new NavBarItem();
			if (group != null) {
				item.Content = "All Folders";
				item.ImageSource = group.ImageSource;
			}
			return item;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
