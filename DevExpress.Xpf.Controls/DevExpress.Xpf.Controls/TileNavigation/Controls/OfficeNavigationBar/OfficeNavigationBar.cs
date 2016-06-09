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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Navigation.NavigationBar.Customization;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FlyoutControl = DevExpress.Xpf.Editors.Flyout.FlyoutControl;
namespace DevExpress.Xpf.Navigation {
	public enum CustomizationButtonVisibility { ShowBeforeItems, ShowAfterItems, Hidden }
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class OfficeNavigationBar : veSelector, IFlyoutProvider, ILogicalOwner {
		#region static
		public const int DefaultPeekFormShowDelay = 700;
		public const int DefaultPeekFormHideDelay = 400;
		public static readonly DependencyProperty ItemSpacingProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty IsCompactProperty;
		public static readonly DependencyProperty MaxItemCountProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CustomButtonsAttachedBehaviorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CustomButtonsSourceProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CustomButtonTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CustomButtonTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CustomButtonStyleProperty;
		public static readonly DependencyProperty ButtonsAlignmentProperty;
		static readonly DependencyPropertyKey ButtonsAlignmentPropertyKey;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemOrientationProperty;
		public static readonly DependencyProperty AllowItemDragDropProperty;
		public static readonly DependencyProperty CustomizationButtonVisibilityProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsCustomizationButtonVisibleProperty;
		public static readonly DependencyProperty NavigationClientProperty;
		public static readonly DependencyProperty ShowPeekFormOnItemHoverProperty;
		public static readonly DependencyProperty PeekFormShowDelayProperty;
		public static readonly DependencyProperty PeekFormHideDelayProperty;
		static OfficeNavigationBar() {
			Type ownerType = typeof(OfficeNavigationBar);
			ItemSpacingProperty = DependencyProperty.Register("ItemSpacing", typeof(double), ownerType);
			IsCompactProperty = DependencyProperty.Register("IsCompact", typeof(bool), ownerType, new PropertyMetadata(false, OnIsCompactChanged));
			MaxItemCountProperty = DependencyProperty.Register("MaxItemCount", typeof(int), ownerType, new PropertyMetadata(-1));
			CustomButtonsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("CustomButtonsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<OfficeNavigationBar, NavigationBarButton>), ownerType, new UIPropertyMetadata(null));
			CustomButtonsSourceProperty = DependencyProperty.Register("CustomButtonsSource", typeof(IEnumerable), ownerType, new PropertyMetadata(null, new PropertyChangedCallback(OnCustomButtonsSourcePropertyChanged)));
			CustomButtonTemplateProperty = DependencyProperty.Register("CustomButtonTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, OnCustomButtonTemplatePropertyChanged));
			CustomButtonTemplateSelectorProperty = DependencyProperty.Register("CustomButtonTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, OnCustomButtonTemplatePropertyChanged));
			CustomButtonStyleProperty = DependencyProperty.Register("CustomButtonStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnCustomButtonTemplatePropertyChanged));
			ButtonsAlignmentPropertyKey = DependencyProperty.RegisterReadOnly("ButtonsAlignment", typeof(Dock), ownerType, new PropertyMetadata(Dock.Right));
			ButtonsAlignmentProperty = ButtonsAlignmentPropertyKey.DependencyProperty;
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), ownerType, new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
			ItemOrientationProperty = DependencyProperty.Register("ItemOrientation", typeof(Orientation), ownerType, new PropertyMetadata(Orientation.Horizontal));
			AllowItemDragDropProperty = DependencyProperty.Register("AllowItemDragDrop", typeof(bool), ownerType);
			CustomizationButtonVisibilityProperty = DependencyProperty.Register("CustomizationButtonVisibility", typeof(CustomizationButtonVisibility), ownerType, new PropertyMetadata(CustomizationButtonVisibility.ShowAfterItems, OnCustomizationButtonVisibilityChanged));
			IsCustomizationButtonVisibleProperty = DependencyProperty.Register("IsCustomizationButtonVisible", typeof(bool), ownerType, new PropertyMetadata(true));
			NavigationClientProperty = DependencyProperty.Register("NavigationClient", typeof(INavigatorClient), ownerType, new PropertyMetadata(null, new PropertyChangedCallback(OnNavigationClientChanged)));
			ShowPeekFormOnItemHoverProperty = DependencyProperty.Register("ShowPeekFormOnItemHover", typeof(bool), ownerType, new PropertyMetadata(true));
			PeekFormShowDelayProperty = DependencyProperty.Register("PeekFormShowDelay", typeof(int), ownerType, new PropertyMetadata(DefaultPeekFormShowDelay));
			PeekFormHideDelayProperty = DependencyProperty.Register("PeekFormHideDelay", typeof(int), ownerType, new PropertyMetadata(DefaultPeekFormHideDelay));
		}
		protected static void OnCustomButtonsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnCustomButtonsSourceChanged(e);
		}
		protected static void OnCustomButtonTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnCustomButtonTemplateChanged(e);
		}
		protected static void OnNavigationClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnNavigationClientChanged((INavigatorClient)e.OldValue, (INavigatorClient)e.NewValue);
		}
		private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnIsCompactChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
		}
		private static void OnCustomizationButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((OfficeNavigationBar)d).OnCustomizationButtonVisibilityChanged((CustomizationButtonVisibility)e.OldValue, (CustomizationButtonVisibility)e.NewValue);
		}
		#endregion
		internal NavigationBarThemeDependentValuesProvider ValuesProvider;
		public OfficeNavigationBar() {
			DefaultStyleKey = typeof(OfficeNavigationBar);
			new FlyoutManager(this).Register(this, new InternalFlyoutEventListener() { Owner = this });
			ValuesProvider = new NavigationBarThemeDependentValuesProvider();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			AddValuesProvider();
		}
		void AddValuesProvider() {
			if(ValuesProvider.Parent == this)
				return;
			((ILogicalOwner)this).AddChild(ValuesProvider);
			AddVisualChild(ValuesProvider);
		}
		protected override int VisualChildrenCount {
			get { return base.VisualChildrenCount + 1; }
		}
		protected override Visual GetVisualChild(int index) {
			if(index < base.VisualChildrenCount)
				return base.GetVisualChild(index);
			return ValuesProvider;
		}
		protected override ISelectorItem CreateSelectorItem() {
			return new NavigationBarItem();
		}
		protected override void PrepareSelectorItem(ISelectorItem selectorItem, object item) {
			base.PrepareSelectorItem(selectorItem, item);
			NavigationBarItem barItem = selectorItem as NavigationBarItem;
			if(barItem != null) {
				barItem.IsCompact = IsCompact;
				barItem.SetBinding(NavigationBarItem.OrientationProperty, new Binding("ItemOrientation") { Source = this });
				INavigationItem navigationItem = item as INavigationItem;
				if(NavigationClient != null && navigationItem != null) {
					barItem.SetBinding(NavigationBarItem.DataContextProperty, new Binding("DataContext") { Source = navigationItem });
					barItem.SetBinding(NavigationBarItem.PeekFormTemplateProperty, new Binding("PeekFormTemplate") { Source = navigationItem });
					barItem.SetBinding(NavigationBarItem.PeekFormTemplateSelectorProperty, new Binding("PeekFormTemplateSelector") { Source = navigationItem });
				}
			}
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is NavigationBarItem;
		}
		protected override void ClearSelectorItem(ISelectorItem selectorItem, object item) {
			base.ClearSelectorItem(selectorItem, item);
			if(selectorItem == _FlyoutEventListener) _FlyoutEventListener = null;
			NavigationBarItem barItem = selectorItem as NavigationBarItem;
			if(barItem != null) {
				barItem.ClearValue(NavigationBarItem.IsCompactPropertyKey);
				barItem.ClearValue(NavigationBarItem.OrientationProperty);
				INavigationItem navigationItem = item as INavigationItem;
				if(navigationItem != null) {
					barItem.ClearValue(NavigationBarItem.DataContextProperty);
					barItem.ClearValue(NavigationBarItem.PeekFormTemplateProperty);
					barItem.ClearValue(NavigationBarItem.PeekFormTemplateSelectorProperty);
				}
			}
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			NavigationBarItemsPanel panel = itemsPanel as NavigationBarItemsPanel;
			if(panel != null) {
				panel.SetBinding(NavigationBarItemsPanel.ItemSpacingProperty, new Binding("ItemSpacing") { Source = this });
				panel.SetBinding(NavigationBarItemsPanel.MaxItemCountProperty, new Binding("MaxItemCount") { Source = this });
				panel.SetBinding(NavigationBarItemsPanel.OrientationProperty, new Binding("Orientation") { Source = this });
				panel.SetBinding(NavigationBarItemsPanel.AllowItemMovingProperty, new Binding("AllowItemDragDrop") { Source = this });
				panel.Owner = this;
			}
		}
		protected override void ReleaseItemsPanelCore(Panel itemsPanel) {
			NavigationBarItemsPanel panel = itemsPanel as NavigationBarItemsPanel;
			if(panel != null) {
				panel.ClearValue(NavigationBarItemsPanel.ItemSpacingProperty);
				panel.ClearValue(NavigationBarItemsPanel.MaxItemCountProperty);
				panel.ClearValue(NavigationBarItemsPanel.OrientationProperty);
				panel.ClearValue(NavigationBarItemsPanel.AllowItemMovingProperty);
				panel.Owner = null;
			}
			base.ReleaseItemsPanelCore(itemsPanel);
		}
		ItemsControl PartButtonsItemsControl;
		FlyoutControl PartFlyoutControl;
		Panel PartVisualTreeHost;
		protected override void ClearTemplateChildren() {
			base.ClearTemplateChildren();
			if(PartButtonsItemsControl != null && !LayoutTreeHelper.IsTemplateChild(PartButtonsItemsControl, this)) {
				PartButtonsItemsControl.ItemsSource = null;
			}
			if(PartFlyoutControl != null) UnsubscribeFlyoutEvents();
			if(PartVisualTreeHost != null) PartVisualTreeHost.Children.Remove(CustomizationHelper.BarManager);
		}
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartButtonsItemsControl = GetTemplateChild("PART_ButtonsItemsControl") as ItemsControl;
			PartFlyoutControl = GetTemplateChild("PART_FlyoutControl") as FlyoutControl;
			PartVisualTreeHost = GetTemplateChild("PART_VisualTreeHost") as Panel;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartButtonsItemsControl != null) {
				CompositeCollection cc = new CompositeCollection();
				cc.Add(new CollectionContainer() { Collection = DefaultButtons });
				cc.Add(new CollectionContainer() { Collection = CustomButtons });
				PartButtonsItemsControl.ItemsSource = cc;
			}
			if(PartFlyoutControl != null) {
				UnsubscribeFlyoutEvents();
				SubscribeFlyoutEvents();
			}
			if(PartVisualTreeHost != null) PartVisualTreeHost.Children.Add(CustomizationHelper.BarManager);
			UpdateVisualState();
		}
		private void SubscribeFlyoutEvents() {
			PartFlyoutControl.LostKeyboardFocus += OnPartFlyoutControlLostKeyboardFocus;
		}
		private void UnsubscribeFlyoutEvents() {
			PartFlyoutControl.LostKeyboardFocus -= OnPartFlyoutControlLostKeyboardFocus;
		}
		void OnPartFlyoutControlLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			if(_FlyoutEventListener != null) {
				if(!PartFlyoutControl.IsMouseOver && !PartFlyoutControl.IsKeyboardFocused && !PartFlyoutControl.IsKeyboardFocusWithin)
					_FlyoutEventListener.OnMouseLeave();
			}
		}
		private void OnCustomButtonsSourceChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<OfficeNavigationBar, NavigationBarButton>.OnItemsSourcePropertyChanged(this,
					e,
					CustomButtonsAttachedBehaviorProperty,
					CustomButtonTemplateProperty,
					CustomButtonTemplateSelectorProperty,
					CustomButtonStyleProperty,
					control => control.CustomButtons,
					category => new NavigationBarButton());
			}
		}
		private void OnCustomButtonTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<OfficeNavigationBar, NavigationBarButton>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				CustomButtonsAttachedBehaviorProperty);
		}
		void UpdateButtonsAlignment() {
			bool isHorz = Orientation == System.Windows.Controls.Orientation.Horizontal;
			bool before = CustomizationButtonVisibility == CustomizationButtonVisibility.ShowBeforeItems;
			Dock align = isHorz ? (before ? Dock.Left : Dock.Right) : (before ? Dock.Top : Dock.Bottom);
			SetValue(ButtonsAlignmentPropertyKey, align);
		}
		protected virtual void OnCustomizationButtonVisibilityChanged(CustomizationButtonVisibility oldValue, CustomizationButtonVisibility newValue) {
			SetValue(IsCustomizationButtonVisibleProperty, newValue != Navigation.CustomizationButtonVisibility.Hidden);
			UpdateButtonsAlignment();
		}
		protected virtual void OnNavigationClientChanged(INavigatorClient oldValue, INavigatorClient newValue) {
			if(oldValue != null) {
				ClearValue(ItemsSourceProperty);
				oldValue.PropertyChanged -= OnNavigatorClientPropertyChanged;
				if(oldValue.MenuActions != null) oldValue.MenuActions.Remove(CustomizationHelper.NavigationOptionsBarItem);
				oldValue.IsAttached = false;
			}
			if(newValue != null) {
				if(newValue.Items != null)
					ItemsSource = newValue.Items;
				SelectedItem = newValue.SelectedItem;
				if(newValue.AcceptsCompactNavigation)
					IsCompact = newValue.Compact;
				newValue.PropertyChanged += OnNavigatorClientPropertyChanged;
				if(ItemTemplateSelector == null) ItemTemplateSelector = this.FindResource<DataTemplateSelector>("navigationItemTemplateSelector");
				if(newValue.MenuActions != null)
					newValue.MenuActions.Add(CustomizationHelper.NavigationOptionsBarItem);
				newValue.IsAttached = true;
			}
			UpdateCompactState();
		}
		void OnNavigatorClientPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "SelectedItem") {
				SelectedItem = NavigationClient.SelectedItem;
			}
			if(e.PropertyName == "Compact") {
				IsCompact = NavigationClient.Compact;
			}
		}
		protected override void OnSelectedItemChanged(object oldValue, object newValue) {
			base.OnSelectedItemChanged(oldValue, newValue);
			if(NavigationClient != null) {
				INavigationItem navigationItem = newValue as INavigationItem;
				NavigationClient.SelectedItem = navigationItem;
				if(navigationItem != null && !IsSelectionLocked)
					CommandHelper.ExecuteCommand(navigationItem);
			}
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			CustomizationHelper.InvalidateState();
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(
						new IEnumerator[] { base.LogicalChildren, logicalChildrenCore.GetEnumerator() });
			}
		}
		protected virtual void OnIsCompactChanged(bool oldValue, bool newValue) {
			if(NavigationClient != null) {
				NavigationClient.Compact = newValue;
				UpdateCompactState();
			}
			foreach(var item in Items) {
				var container = ItemContainerGenerator.ContainerFromItem(item) as NavigationBarItem;
				if(container != null) container.IsCompact = newValue;
			}
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			UpdateButtonsAlignment();
			UpdateOrientationState();
		}
		private void UpdateOrientationState() {
			VisualStateManager.GoToState(this, Orientation.ToString(), false);
		}
		private void UpdateCompactState() {
			VisualStateManager.GoToState(this, IsCompact && NavigationClient != null && NavigationClient.AcceptsCompactNavigation ? "Compact" : "NonCompact", false);
		}
		private void UpdateVisualState() {
			UpdateOrientationState();
			UpdateCompactState();
		}
		internal void LockSelection() {
			SelectionLocker.Lock();
		}
		internal void UnlockSelection() {
			SelectionLocker.Unlock();
		}
		protected override bool CanChangeSelection() {
			return !SelectionLocker.IsLocked && base.CanChangeSelection();
		}
		Locker _SelectionLocker;
		Locker SelectionLocker {
			get {
				if(_SelectionLocker == null) _SelectionLocker = new Locker();
				return _SelectionLocker;
			}
		}
		internal int ActualHiddenItemsCount {
			get {
				NavigationBarItemsPanel panel = PartItemsPanel as NavigationBarItemsPanel;
				return panel != null ? panel.ActualHiddenItemsCount : 0;
			}
		}
		private NavigationBarCustomizationHelper _CustomizationHelper;
		internal NavigationBarCustomizationHelper CustomizationHelper {
			get {
				if(_CustomizationHelper == null) _CustomizationHelper = new NavigationBarCustomizationHelper(this);
				return _CustomizationHelper;
			}
		}
		public CustomizationButtonVisibility CustomizationButtonVisibility {
			get { return (CustomizationButtonVisibility)GetValue(CustomizationButtonVisibilityProperty); }
			set { SetValue(CustomizationButtonVisibilityProperty, value); }
		}
		public bool AllowItemDragDrop {
			get { return (bool)GetValue(AllowItemDragDropProperty); }
			set { SetValue(AllowItemDragDropProperty, value); }
		}
		public Orientation ItemOrientation {
			get { return (Orientation)GetValue(ItemOrientationProperty); }
			set { SetValue(ItemOrientationProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public Dock ButtonsAlignment {
			get { return (Dock)GetValue(ButtonsAlignmentProperty); }
			private set { SetValue(ButtonsAlignmentPropertyKey, value); }
		}
		public int PeekFormShowDelay {
			get { return (int)GetValue(PeekFormShowDelayProperty); }
			set { SetValue(PeekFormShowDelayProperty, value); }
		}
		public int PeekFormHideDelay {
			get { return (int)GetValue(PeekFormHideDelayProperty); }
			set { SetValue(PeekFormHideDelayProperty, value); }
		}
		public bool ShowPeekFormOnItemHover {
			get { return (bool)GetValue(ShowPeekFormOnItemHoverProperty); }
			set { SetValue(ShowPeekFormOnItemHoverProperty, value); }
		}
		public INavigatorClient NavigationClient {
			get { return (INavigatorClient)GetValue(NavigationClientProperty); }
			set { SetValue(NavigationClientProperty, value); }
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		internal bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public int MaxItemCount {
			get { return (int)GetValue(MaxItemCountProperty); }
			set { SetValue(MaxItemCountProperty, value); }
		}
		IEnumerable CustomButtonsSource {
			get { return (IEnumerable)GetValue(CustomButtonsSourceProperty); }
			set { SetValue(CustomButtonsSourceProperty, value); }
		}
		DataTemplate CustomButtonTemplate {
			get { return (DataTemplate)GetValue(CustomButtonTemplateProperty); }
			set { SetValue(CustomButtonTemplateProperty, value); }
		}
		DataTemplateSelector CustomButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CustomButtonTemplateSelectorProperty); }
			set { SetValue(CustomButtonTemplateSelectorProperty, value); }
		}
		Style CustomButtonStyle {
			get { return (Style)GetValue(CustomButtonStyleProperty); }
			set { SetValue(CustomButtonStyleProperty, value); }
		}
		Collection<NavigationBarButton> _CustomButtons;
		[Bindable(true), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		Collection<NavigationBarButton> CustomButtons {
			get {
				if(_CustomButtons == null) _CustomButtons = new NavigationBarButtonCollection();
				return _CustomButtons;
			}
		}
		NavigationBarButton _CustomizationButton;
		NavigationBarButton CustomizationButton {
			get {
				if(_CustomizationButton == null) {
					_CustomizationButton = new NavigationBarCustomizationButton();
					_CustomizationButton.Command = ShowCustomizationMenuCommand;
					DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(_CustomizationButton, CustomizationHelper.NavigationBarCustomizationMenu);
					_CustomizationButton.SetBinding(UIElement.VisibilityProperty, new Binding("IsCustomizationButtonVisible") { Source = this, Converter = new BoolToVisibilityConverter() });
				}
				return _CustomizationButton;
			}
		}
		Collection<NavigationBarButton> _DefaultButtons;
		Collection<NavigationBarButton> DefaultButtons {
			get {
				if(_DefaultButtons == null) {
					_DefaultButtons = new NavigationBarButtonCollection();
					_DefaultButtons.Add(CustomizationButton);
				}
				return _DefaultButtons;
			}
		}
		DelegateCommand _ShowCustomizationMenuCommand;
		DelegateCommand ShowCustomizationMenuCommand {
			get {
				if(_ShowCustomizationMenuCommand == null)
					_ShowCustomizationMenuCommand = DelegateCommandFactory.Create(() => { ContextMenuManager.ShowElementContextMenu(CustomizationButton); });
				return _ShowCustomizationMenuCommand;
			}
		}
		#region IFlyoutProvider Members
		Editors.Flyout.FlyoutControl IFlyoutProvider.FlyoutControl {
			get { return PartFlyoutControl; }
		}
		Editors.Flyout.FlyoutPlacement IFlyoutProvider.Placement {
			get { return Orientation == System.Windows.Controls.Orientation.Horizontal ? Editors.Flyout.FlyoutPlacement.Top : Editors.Flyout.FlyoutPlacement.Right; }
		}
		IFlyoutEventListener _FlyoutEventListener;
		IFlyoutEventListener IFlyoutProvider.FlyoutEventListener {
			get { return _FlyoutEventListener; }
			set { _FlyoutEventListener = value; }
		}
		#endregion
		#region ILogicalOwner Members
		List<object> logicalChildrenCore = new List<object>();
		public new void AddChild(object child) {
			if(!logicalChildrenCore.Contains(child)) logicalChildrenCore.Add(child);
			AddLogicalChild(child);
		}
		public void RemoveChild(object child) {
			if(logicalChildrenCore.Contains(child)) logicalChildrenCore.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
		class InternalFlyoutEventListener : IFlyoutEventListener {
			public IFlyoutProvider Owner { get; set; }
			#region IFlyoutEventListener Members
			public void OnFlyoutClosed() {
			}
			public void OnFlyoutOpened() {
			}
			public void OnMouseLeave() {
			}
			public void OnMouseEnter() {
			}
			public DevExpress.Xpf.WindowsUI.Internal.Flyout.FlyoutBase Flyout {
				get { return Owner.FlyoutEventListener.Flyout; }
			}
			#endregion
			#region IFlyoutEventListener Members
			public void OnFlyoutClosed(bool onClickThrough) {
				throw new NotImplementedException();
			}
			#endregion
		}
	}
}
