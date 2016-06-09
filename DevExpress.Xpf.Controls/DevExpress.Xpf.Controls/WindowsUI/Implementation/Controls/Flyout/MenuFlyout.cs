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
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal.Flyout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
namespace DevExpress.Xpf.WindowsUI {
	[ContentProperty("Items")]
	public class MenuFlyout : FlyoutBase, ILogicalOwner {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		internal static readonly RoutedEvent IsSelectedChangedEvent;
		static MenuFlyout() {
			Type ownerType = typeof(MenuFlyout);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			ItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<MenuFlyout, MenuFlyoutItem>), ownerType, new UIPropertyMetadata(null));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			IsSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsSelectedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(MenuFlyout));
			EventManager.RegisterClassHandler(typeof(MenuFlyout), MenuFlyout.IsSelectedChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsSelectedChanged));
		}
		static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((MenuFlyout)d).OnItemsSourceChanged(e);
		}
		static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((MenuFlyout)d).OnItemTemplateChanged(e);
		}
		private static void OnIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) {
			MenuFlyoutItem newSelectedMenuItem = e.OriginalSource as MenuFlyoutItem;
			if(newSelectedMenuItem != null) {
				MenuFlyout menu = (MenuFlyout)sender;
				if(e.NewValue) {
					if((menu.CurrentSelection != newSelectedMenuItem) && (menu.Items.Contains(newSelectedMenuItem))) {
						menu.CurrentSelection = newSelectedMenuItem;
					}
				}
				else {
					if(menu.CurrentSelection == newSelectedMenuItem) {
						menu.CurrentSelection = null;
					}
				}
				e.Handled = true;
			}
		}
		#endregion
		#region properties
		public object ItemsSource {
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		MenuFlyoutItemCollection _Items;
		public MenuFlyoutItemCollection Items {
			get {
				if(_Items == null) {
					_Items = new MenuFlyoutItemCollection() { Owner = this };
				}
				return _Items;
			}
		}
		private MenuFlyoutItem _currentSelection;
		internal MenuFlyoutItem CurrentSelection {
			get {
				return _currentSelection;
			}
			set {
				bool wasFocused = false;
				if(_currentSelection != null) {
					wasFocused = _currentSelection.IsKeyboardFocused;
				}
				_currentSelection = value;
				if(_currentSelection != null) {
					if(wasFocused) {
						_currentSelection.Focus();
					}
				}
			}
		}
		bool HasItems { get { return Items.Count > 0; } }
		#endregion
		public MenuFlyout() {
		}
		private void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<MenuFlyout, MenuFlyoutItem>.OnItemsSourcePropertyChanged(this,
					e,
					ItemsAttachedBehaviorProperty,
					ItemTemplateProperty,
					ItemTemplateSelectorProperty,
					ItemStyleProperty,
					control => control.Items,
					control => control.CreateItem(),
					null,
					(item) => InitItem(item));
			}
		}
		private void OnItemTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<MenuFlyout, MenuFlyoutItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(MenuFlyoutItem.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected virtual MenuFlyoutItem CreateItem() {
			return new MenuFlyoutItem();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if(!HasCapture && !IsMouseOver && CurrentSelection != null && !CurrentSelection.IsKeyboardFocused) {
				CurrentSelection = null;
			}
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if(!IsKeyboardFocusWithin && !IsMenuMode) {
				if(CurrentSelection != null) {
					CurrentSelection = null;
				}
			}
		}
		internal override void KeyboardLeaveMenuMode() {
			if(IsMenuMode) {
				IsMenuMode = false;
			}
			else {
				CurrentSelection = null;
				RestorePreviousFocus();
			}
			base.KeyboardLeaveMenuMode();
		}
		internal override void OnLeaveMenuMode() {
			if(CurrentSelection != null) {
				CurrentSelection = null;
			}
		}
		FrameworkElement FindFocusable(int startIndex, int direction, out int foundIndex){
			if(HasItems) {
				int count = Items.Count;
				while(startIndex >= 0 && startIndex < count) {
					FrameworkElement element = Items[startIndex];
					if(element != null && element.Focusable) {
						foundIndex = startIndex;
						return element;
					}
					startIndex += direction;
				}
			}
			foundIndex = -1;
			return null;
		}
		void NavigateToItem(FrameworkElement item, int index) {
			if(index == -1 || item == null) return;
			item.BringIntoView();
			item.Focus();
		}
		void NavigateToStart() {
			if(HasItems) {
				int foundIndex;
				FrameworkElement item = FindFocusable(0, 1, out foundIndex);
				NavigateToItem(item, foundIndex);
			}
		}
		void NavigateToEnd() {
			if(HasItems) {
				int foundIndex;
				FrameworkElement item = FindFocusable(Items.Count - 1, -1, out foundIndex);
				NavigateToItem(item, foundIndex);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(!e.Handled && this.IsOpen) {
				switch(e.Key) {
					case Key.Up:
						if(CurrentSelection == null) {
							NavigateToEnd();
							e.Handled = true;
						}
						return;
					case Key.Down:
						if(CurrentSelection == null) {
							NavigateToStart();
							e.Handled = true;
						}
						return;
				}
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if((!e.Handled && this.IsOpen) && (e.Key == Key.Apps)) {
				base.KeyboardLeaveMenuMode();
				e.Handled = true;
			}
		}
		#region ILogicalOwner Members
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalHost.GetEnumerator()); }
		}
		List<object> logicalHost = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			if(!logicalHost.Contains(child))
				logicalHost.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalHost.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
	}
	public class MenuFlyoutItem : ClickableBase, ICommandSource {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty StaysOpenOnClickProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		static MenuFlyoutItem() {
			Type ownerType = typeof(MenuFlyoutItem);
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType, new PropertyMetadata(false, OnIsSelectedChanged));
			StaysOpenOnClickProperty = DependencyProperty.Register("StaysOpenOnClick", typeof(bool), ownerType, new PropertyMetadata(false));
			CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), ownerType, new PropertyMetadata(null, OnCommandChanged));
			CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), ownerType, new PropertyMetadata(null));
			CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), ownerType, new PropertyMetadata(null));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MenuFlyoutItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(MenuFlyoutItem), new FrameworkPropertyMetadata(null));
			InputMethod.IsInputMethodSuspendedProperty.OverrideMetadata(typeof(MenuFlyoutItem), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
		}
		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MenuFlyoutItem menuFlyoutItem = d as MenuFlyoutItem;
			if(menuFlyoutItem != null)
				menuFlyoutItem.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MenuFlyoutItem menuFlyoutItem = d as MenuFlyoutItem;
			if(menuFlyoutItem != null)
				menuFlyoutItem.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		#endregion
		#region props
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			private set { SetValue(IsSelectedProperty, value); }
		}
		public bool StaysOpenOnClick {
			get { return (bool)GetValue(StaysOpenOnClickProperty); }
			set { SetValue(StaysOpenOnClickProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		bool _CanExecute = true;
		private bool CanExecute {
			get { return _CanExecute; }
			set {
				if(value != _CanExecute) {
					_CanExecute = value;
					CoerceValue(IsEnabledProperty);
				}
			}
		}
		protected override bool IsEnabledCore {
			get { return base.IsEnabledCore && CanExecute; }
		}
		#endregion
		public MenuFlyoutItem() {
			DefaultStyleKey = typeof(MenuFlyoutItem);
		}
		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue) {
			Controller.UpdateState(true);
			RaiseEvent(new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue, MenuFlyout.IsSelectedChangedEvent));
		}
		private void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			if(oldCommand != null) {
				UnhookCommand(oldCommand);
			}
			if(newCommand != null) {
				HookCommand(newCommand);
			}
		}
		private void UnhookCommand(ICommand command) {
			command.CanExecuteChanged -= OnCanExecuteChanged;
			UpdateCanExecute();
		}
		private void HookCommand(ICommand command) {
			command.CanExecuteChanged += OnCanExecuteChanged;
			UpdateCanExecute();
		}
		private void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		private void UpdateCanExecute() {
			CanExecute = Command != null ? CommandHelper.CanExecuteCommand(this) : true;
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			IsSelected = IsKeyboardFocusWithin;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(!base.IsKeyboardFocusWithin) {
				base.Focus();
			}
			if(!this.IsSelected) {
				base.SetCurrentValue(IsSelectedProperty, true);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.Key) {
				case Key.Return:
				case Key.Space:
					Controller.InvokeClick();
					e.Handled = true;
					break;
			}
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			Controller.UpdateState(true);
		}
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
			Controller.UpdateState(true);
		}
		protected override ControlControllerBase CreateController() {
			return new MenuFlyoutItemController(this);
		}
		protected override void OnClick() {
			base.OnClick();
			if(!StaysOpenOnClick) {
				FlyoutBase flyout = FlyoutBase.GetFlyout(this);
				if(flyout.Service != null) {
					flyout.Service.Hide(flyout);
				}
				else {
					if(flyout != null && flyout.IsOpen) flyout.IsOpen = false;
				}
			}
			CommandHelper.ExecuteCommand(this);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsSelected = false;
			if(base.IsKeyboardFocusWithin) {
				ItemsControl control = ItemsControl.ItemsControlFromItemContainer(this);
				if(control != null) {
					control.Focus();
				}
			}
		}
		class MenuFlyoutItemController : ClickableController {
			public new MenuFlyoutItem Control { get { return base.Control as MenuFlyoutItem; } }
			public MenuFlyoutItemController(IControl control)
				: base(control) {
			}
			public override void UpdateState(bool useTransitions) {
				string stateName;
				if(Control.IsEnabled)
					if(IsMouseLeftButtonDown)
						stateName = "Pressed";
					else
						if(IsMouseEntered || FocusHelper.IsKeyboardFocusWithin(Control))
							stateName = "MouseOver";
						else
							stateName = "Normal";
				else
					stateName = "Disabled";
				VisualStateManager.GoToState(Control, stateName, useTransitions);
			}
		}
	}
	public class MenuFlyoutItemCollection : ObservableCollection<FrameworkElement> {
		internal ILogicalOwner Owner { get; set; }
		public MenuFlyoutItemCollection() {
		}
		protected override void RemoveItem(int index) {
			object item = this[index];
			Owner.Do(x => x.RemoveChild(item));
			base.RemoveItem(index);
		}
		protected override void InsertItem(int index, FrameworkElement item) {
			Owner.Do(x => x.AddChild(item));
			base.InsertItem(index, item);
		}
	}
	public class MenuFlyoutSeparator : Control {
		static MenuFlyoutSeparator() {
			Type ownerType = typeof(MenuFlyoutSeparator);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class MenuFlyoutItemContentPresenter : veContentControl {
	}
	public class MenuFlyoutPresenter : ItemsControl {
		static MenuFlyoutPresenter() {
		}
		public MenuFlyoutPresenter() {
			DefaultStyleKey = typeof(MenuFlyoutPresenter);
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is MenuFlyoutItem || item is MenuFlyoutSeparator;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new MenuFlyoutItem();
		}
	}
}
