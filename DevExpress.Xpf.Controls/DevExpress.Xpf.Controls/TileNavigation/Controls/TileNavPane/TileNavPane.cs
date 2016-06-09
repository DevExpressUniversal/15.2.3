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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using FlyoutControl = DevExpress.Xpf.Editors.Flyout.FlyoutControl;
namespace DevExpress.Xpf.Navigation {
	[ContentProperty("Categories")]
	[StyleTypedProperty(Property = "GroupHeaderStyle", StyleTargetType = typeof(TileBarGroupHeader))]
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class TileNavPane : Control, IFlyoutProvider, ILogicalOwner {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CategoryAttachedBehaviorProperty;
		public static readonly DependencyProperty CategoriesSourceProperty;
		public static readonly DependencyProperty CategoryTemplateProperty;
		public static readonly DependencyProperty CategoryTemplateSelectorProperty;
		public static readonly DependencyProperty CategoryStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty NavButtonsAttachedBehaviorProperty;
		public static readonly DependencyProperty NavButtonsSourceProperty;
		public static readonly DependencyProperty NavButtonTemplateProperty;
		public static readonly DependencyProperty NavButtonTemplateSelectorProperty;
		public static readonly DependencyProperty NavButtonStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty SelectedElementProperty;
		internal static readonly RoutedEvent ClickEvent;
		public static readonly DependencyProperty TileNavGroupHeaderProperty;
		public static readonly DependencyProperty GroupHeaderTemplateProperty;
		public static readonly DependencyProperty GroupHeaderStyleProperty;
		public static readonly DependencyProperty ContinuousNavigationProperty;
		public static readonly DependencyProperty CloseOnOuterClickProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FlyoutSourceTypeProperty;
		public static readonly DependencyProperty FlyoutShowModeProperty;
		public static readonly DependencyProperty FlyoutShowDirectionProperty;
		public static readonly DependencyProperty ShowItemShadowProperty;
		static TileNavPane() {
			Type ownerType = typeof(TileNavPane);
			CategoryAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("CategoryAttachedBehavior", typeof(ItemsAttachedBehaviorCore<TileNavPane, TileNavCategory>), ownerType, new UIPropertyMetadata(null));
			CategoriesSourceProperty = DependencyProperty.Register("CategoriesSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoriesSourcePropertyChanged)));
			CategoryTemplateProperty = DependencyProperty.Register("CategoryTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			CategoryTemplateSelectorProperty = DependencyProperty.Register("CategoryTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			CategoryStyleProperty = DependencyProperty.Register("CategoryStyle", typeof(Style), ownerType, new PropertyMetadata(null, new PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			NavButtonsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("NavButtonsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<TileNavPane, NavButton>), ownerType, new UIPropertyMetadata(null));
			NavButtonsSourceProperty = DependencyProperty.Register("NavButtonsSource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnNavButtonsSourcePropertyChanged)));
			NavButtonTemplateProperty = DependencyProperty.Register("NavButtonTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnNavButtonTemplatePropertyChanged)));
			NavButtonTemplateSelectorProperty = DependencyProperty.Register("NavButtonTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnNavButtonTemplatePropertyChanged)));
			NavButtonStyleProperty = DependencyProperty.Register("NavButtonStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnNavButtonTemplatePropertyChanged)));
			SelectedElementProperty = DependencyProperty.Register("SelectedElement", typeof(NavElementBase), ownerType, new PropertyMetadata(null, OnSelectedElementChanged));
			TileNavGroupHeaderProperty = DependencyProperty.RegisterAttached("TileNavGroupHeader", typeof(object), ownerType, new PropertyMetadata(null, OnTileNavGroupHeaderChanged));
			GroupHeaderTemplateProperty = DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), ownerType);
			GroupHeaderStyleProperty = DependencyProperty.Register("GroupHeaderStyle", typeof(Style), ownerType);
			ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(EventHandler), ownerType);
			ContinuousNavigationProperty = DependencyProperty.Register("ContinuousNavigation", typeof(bool), typeof(TileNavPane), new PropertyMetadata(false));
			CloseOnOuterClickProperty = DependencyProperty.Register("CloseOnOuterClick", typeof(bool), typeof(TileNavPane), new PropertyMetadata(true, OnCloseOnOuterClickChanged));
			FlyoutSourceTypeProperty = DependencyProperty.RegisterAttached("FlyoutSourceType", typeof(FlyoutSourceType), typeof(TileNavPane), new FrameworkPropertyMetadata(FlyoutSourceType.FromTileBar, FrameworkPropertyMetadataOptions.Inherits));
			FlyoutShowModeProperty = DependencyProperty.Register("FlyoutShowMode", typeof(FlyoutShowMode), typeof(TileNavPane), new PropertyMetadata(FlyoutShowMode.Adorner, OnFlyoutShowModeChanged));
			FlyoutShowDirectionProperty = DependencyProperty.Register("FlyoutShowDirection", typeof(FlyoutShowDirection), ownerType, new PropertyMetadata(FlyoutShowDirection.Default, OnFlyoutShowDirectionChanged));
			ShowItemShadowProperty = DependencyProperty.Register("ShowItemShadow", typeof(bool), typeof(TileNavPane), new PropertyMetadata(false));
		}
		static void OnCategoriesSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnCategoriesSourceChanged(e);
		}
		static void OnCategoryTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnCategoryTemplateChanged(e);
		}
		static void OnNavButtonsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnNavButtonsSourceChanged(e);
		}
		static void OnNavButtonTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnNavButtonTemplateChanged(e);
		}
		public static object GetTileNavGroupHeader(DependencyObject element) {
			return element.GetValue(TileNavGroupHeaderProperty);
		}
		public static void SetTileNavGroupHeader(DependencyObject element, object value) {
			element.SetValue(TileNavGroupHeaderProperty, value);
		}
		static void OnTileNavGroupHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileBarItemsPanel.SetGroupHeader(d, e.NewValue);
		}
		static void OnSelectedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnSelectedElementChanged((NavElementBase)e.OldValue, (NavElementBase)e.NewValue);
		}
		static void OnCloseOnOuterClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var tileNavPane = d as TileNavPane;
			if(tileNavPane.FlyoutManager == null) return;
			tileNavPane.FlyoutManager.CloseOnTopElementMouseDown = (bool)e.NewValue;
		}
		static void OnFlyoutShowModeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			((TileNavPane)dObj).OnFlyoutShowModeChanged(e.OldValue, e.NewValue);
		}					
		static void OnFlyoutShowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TileNavPane)d).OnFlyoutShowDirectionChanged((FlyoutShowDirection)e.OldValue, (FlyoutShowDirection)e.NewValue);
		}
		internal static FlyoutSourceType GetFlyoutSourceType(DependencyObject target) {
			return (FlyoutSourceType)target.GetValue(FlyoutSourceTypeProperty);
		}
		internal static void SetFlyoutSourceType(DependencyObject target, FlyoutSourceType value) {
			target.SetValue(FlyoutSourceTypeProperty, value);
		}
		#endregion
		public TileNavPane() {
			DefaultStyleKey = typeof(TileNavPane);
			this.AddHandler(TileNavPane.ClickEvent, new RoutedEventHandler(OnNavElementClick));
		}
		internal FlyoutControl PartFlyoutControl { get; private set; }
		ItemsControl PartHeadersItemsControl;
		FlyoutDecorator PartFlyoutDecorator;
		public override void OnApplyTemplate() {
			if(PartHeadersItemsControl != null)
				PartHeadersItemsControl.ClearValue(ItemsControl.ItemsSourceProperty);
			base.OnApplyTemplate();
			EnsureFlyoutControl();
			PartHeadersItemsControl = GetTemplateChild("PART_HeadersItemsControl") as ItemsControl;
			if(PartHeadersItemsControl != null) {
				CompositeCollection cc = new CompositeCollection();
				cc.Add(new CollectionContainer() { Collection = NavButtons });
				cc.Add(new CollectionContainer() { Collection = DefaultButtons });
				PartHeadersItemsControl.ItemsSource = cc;
			}
		}
		void EnsureFlyoutControl(bool onApplyTemplate = true) {
			if(PartFlyoutControl != null) {
				PartFlyoutControl.Content = null;
				PartFlyoutControl = null;
			}
			PartFlyoutDecorator = GetTemplateChild("PART_TileNavFlyoutDecorator") as FlyoutDecorator;
			if(PartFlyoutDecorator != null) {
				PartFlyoutDecorator.FlyoutShowMode = FlyoutShowMode;
				if(onApplyTemplate)
					PartFlyoutDecorator.SizeChanged += tileNavFlyoutDecorator_SizeChanged;
				else
					PartFlyoutControl = PartFlyoutDecorator.ActualFlyoutControl;
			} else {
				PartFlyoutControl = GetTemplateChild("PART_FlyoutControl") as FlyoutControl;
			}
			EnsureFlyoutControlProperties();
		}
		void tileNavFlyoutDecorator_SizeChanged(object sender, SizeChangedEventArgs e) {
			PartFlyoutDecorator.SizeChanged -= tileNavFlyoutDecorator_SizeChanged;
			PartFlyoutControl = PartFlyoutDecorator.ActualFlyoutControl;
			EnsureFlyoutControlProperties();
		}
		void EnsureFlyoutControlProperties() {
			if(PartFlyoutControl != null) {
				PartFlyoutControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
				PartFlyoutControl.Settings = new DevExpress.Xpf.Editors.Flyout.FlyoutSettings() { Placement = ((IFlyoutProvider)this).Placement };
				PartFlyoutControl.StaysOpen = true;
				PartFlyoutControl.AlwaysOnTop = true;
				PartFlyoutControl.PlacementTarget = this;
				PartFlyoutControl.AllowMoveAnimation = false;
				PartFlyoutControl.Content = PartTileNavPaneContentControl;
			}
		}
		private void OnCategoriesSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<TileNavPane, TileNavCategory>.OnItemsSourcePropertyChanged(this,
					e,
					CategoryAttachedBehaviorProperty,
					CategoryTemplateProperty,
					CategoryTemplateSelectorProperty,
					CategoryStyleProperty,
					control => control.Categories,
					(control) => control.CreateCategory(),
					null,
					(item) => InitCategory(item)
					);
			}
		}
		private void OnCategoryTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<TileNavPane, TileNavCategory>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				CategoryAttachedBehaviorProperty);
		}
		private void OnNavButtonsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<TileNavPane, NavButton>.OnItemsSourcePropertyChanged(this,
					e,
					NavButtonsAttachedBehaviorProperty,
					NavButtonTemplateProperty,
					NavButtonTemplateSelectorProperty,
					NavButtonStyleProperty,
					control => { return control.NavButtons; },
					(control) => control.CreateNavButton(),
					null,
					(item) => InitNavButton(item)
					);
			}
		}
		private void OnNavButtonTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<TileNavPane, NavButton>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				NavButtonsAttachedBehaviorProperty);
		}
		protected virtual void InitNavButton(FrameworkElement item) {
			item.SetBinding(NavButton.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected NavButton CreateNavButton() {
			return new NavButton();
		}
		protected virtual void InitCategory(FrameworkElement item) {
			item.SetBinding(TileNavCategory.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected TileNavCategory CreateCategory() {
			return new TileNavCategory();
		}
		internal void OnNavElementClick(object sender, RoutedEventArgs e) {
			INavElement element = e.OriginalSource as INavElement;
			if(element == null) return;
			e.Handled = true;
			NavButton navButton = element as NavButton;
			if(navButton != null && PartFlyoutControl != null) {
				UpdateFlyout(navButton);
				return;
			}
			CloseFlyoutInternal();
		}
		private void UpdateNavButtonsCheckState(NavButton checkedButton) {
			foreach(var button in DefaultButtons) {
				if (button != checkedButton)
					button.IsChecked = false;
			}
			foreach(var button in NavButtons) {
				if(button != checkedButton)
					button.IsChecked = false;
			}
		}
		private void UpdateFlyout(NavButton navButton) {
			if(navButton == null) return;
			UpdateNavButtonsCheckState(navButton);
			IFlyoutEventListener flyoutOwner = FlyoutManager.GetFlyoutTarget(PartFlyoutControl);
			if(flyoutOwner == navButton) {
				if(PartFlyoutControl.IsOpen)
					CloseFlyoutInternal(true);
				else
					ShowFlyout(navButton);
			} else {
				UIElement itemsPresenter = navButton.GetItemsPresenter();
				SetFlyoutSourceType(itemsPresenter, FlyoutSourceType.FromTileNavPane);
				(itemsPresenter as TileBar).Do(x => x.SetBinding(TileBar.FlyoutShowModeProperty, new Binding("FlyoutShowMode") { Source = this }));
				(itemsPresenter as TileBar).Do(x => x.SetBinding(TileBar.FlyoutShowDirectionProperty, new Binding("FlyoutShowDirection") { Source = this }));
				(itemsPresenter as TileBar).Do(x => x.SetBinding(TileBar.ShowItemShadowProperty, new Binding("ShowItemShadow") { Source = this }));
				if(navButton.IsMain) {
					if(Categories.Count <= 0)
						CloseFlyoutInternal();
					else {
						FlyoutManager.CloseAllExcept(this);
						PartTileNavPaneContentControl.Content = itemsPresenter;
						BindingOperations.SetBinding(itemsPresenter, ItemsControl.ItemsSourceProperty, new Binding("Categories") { Source = this });
						BindingOperations.SetBinding(itemsPresenter, ItemsControl.ItemContainerStyleProperty, new Binding("CategoryStyle") { Source = this });
						ShowFlyout(navButton);
					}
				} else {
					if(!navButton.HasItems)
						CloseFlyoutInternal(true);
					else {
						FlyoutManager.CloseAllExcept(this);
						PartTileNavPaneContentControl.Content = itemsPresenter;
						ShowFlyout(navButton);
					}
				}
			}
		}
		private void ShowFlyout(INavElement element) {
			FlyoutManager.Show(this, element as IFlyoutEventListener);
		}
		private void CloseFlyoutInternal(bool forceClose = false) {
			if(!ContinuousNavigation || forceClose)
				FlyoutManager.CloseAll();
		}
		public void CloseFlyout() {
			CloseFlyoutInternal(true);
		}
		void UpdateDefaultButtonsCollection(NavElementBase element) {
			if(DesignerProperties.GetIsInDesignMode(this)) return;
			DefaultButtons.BeginUpdate();
			DefaultButtons.Clear();
			INavElement navParent = element;
			while(navParent != null) {
				SelectedNavButton button = new SelectedNavButton();
				button.SetBinding(NavButton.ContentProperty, new Binding("Content") { Source = navParent });
				button.SetBinding(NavButton.ContentTemplateProperty, new Binding("ContentTemplate") { Source = navParent });
				button.SetBinding(NavButton.ContentTemplateSelectorProperty, new Binding("ContentTemplateSelector") { Source = navParent });
				if(navParent.HasItems) {
					button.SetBinding(NavButton.ItemsSourceProperty, new Binding("Items") { Source = navParent });
					button.SetBinding(NavButton.ItemStyleProperty, new Binding("ItemStyle") { Source = navParent });
					if(navParent is TileNavCategory) NavButton.SetItemsPresenter(button, ((TileNavCategory)navParent).PartItemsControl);
					if(navParent is TileNavItem) NavButton.SetItemsPresenter(button, ((TileNavItem)navParent).PartItemsControl);						
					if(ContinuousNavigation && element == navParent && (element is TileNavCategory || element is TileNavItem)) {
						UpdateFlyout(button);
					}
				}
				DefaultButtons.Insert(0, button);
				DefaultButtons.Insert(0, new NavButtonSeparator());
				navParent = navParent.NavParent;
			}
			var mainButton = NavButtons.FirstOrDefault(x => ((NavButton)x).IsMain);
			if(mainButton == null || !mainButton.IsVisible) {
				if(DefaultButtons.Count > 0 && DefaultButtons[0] is NavButtonSeparator) DefaultButtons.RemoveAt(0);
			}						
			DefaultButtons.EndUpdate();
		}
		internal void UpdateFlayout() {
			UpdateDefaultButtonsCollection(SelectedElement);
		}
		protected virtual void OnSelectedElementChanged(NavElementBase oldValue, NavElementBase newValue) {
			UpdateDefaultButtonsCollection(newValue);
			if (!DesignerProperties.GetIsInDesignMode(this))
				if(oldValue != null) oldValue.IsSelected = false;
			RaiseSelectedElementChanged(SelectedElement);
		}
		bool SetSelectedElement(NavElementBase element) {
			if(element == null || SelectedElement == element) return false;
			SelectedElement = element;
			return true;
		}
		internal void SetSelectedElement(NavElementBase element, bool isSelected) {
			if(element == null) return;
			if(isSelected)
				SetSelectedElement(element);
			else
				if(SelectedElement == element) SelectedElement = null;
		}
		protected void RaiseSelectedElementChanged(NavElementBase element) {
			TileNavPaneSelectionChangedEventArgs e = new TileNavPaneSelectionChangedEventArgs() { SelectedElement = element };
			TileNavPaneSelectionChangedEventHandler handler = SelectionChanged;
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnFlyoutShowModeChanged(object oldValue, object newValue) {
			CloseAndEnsureFlyout();
		}
		protected virtual void OnFlyoutShowDirectionChanged(object oldValue, object newValue) {
			CloseAndEnsureFlyout();
		}
		void CloseAndEnsureFlyout() {
			if(PartFlyoutControl == null)
				return;
			FlyoutManager.CloseAll();
			EnsureFlyoutControl(false);
			UpdateNavButtonsCheckState(null);
		}
		public event TileNavPaneSelectionChangedEventHandler SelectionChanged;
		ContentControl _PartTileNavPaneContentControl;
		ContentControl PartTileNavPaneContentControl {
			get {
				if(_PartTileNavPaneContentControl == null) {
					_PartTileNavPaneContentControl = new TileNavPaneContentControl();
				}
				return _PartTileNavPaneContentControl;
			}
		}
		private FlyoutManager _FlyoutManager;
		internal FlyoutManager FlyoutManager {
			get {
				if(_FlyoutManager == null) {
					_FlyoutManager = new FlyoutManager(this);
					FlyoutManager.SetFlyoutManager(this, _FlyoutManager);
				}
				return _FlyoutManager;
			}
		}
		public object CategoriesSource {
			get { return GetValue(CategoriesSourceProperty); }
			set { SetValue(CategoriesSourceProperty, value); }
		}
		public DataTemplate CategoryTemplate {
			get { return (DataTemplate)GetValue(CategoryTemplateProperty); }
			set { SetValue(CategoryTemplateProperty, value); }
		}
		public DataTemplateSelector CategoryTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CategoryTemplateSelectorProperty); }
			set { SetValue(CategoryTemplateSelectorProperty, value); }
		}
		public Style CategoryStyle {
			get { return (Style)GetValue(CategoryStyleProperty); }
			set { SetValue(CategoryStyleProperty, value); }
		}
		TileNavCategoryCollection _Categories;
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		public TileNavCategoryCollection Categories {
			get {
				if(_Categories == null) _Categories = new TileNavCategoryCollection(this);
				return _Categories;
			}
		}
		public IEnumerable NavButtonsSource {
			get { return (IEnumerable)GetValue(NavButtonsSourceProperty); }
			set { SetValue(NavButtonsSourceProperty, value); }
		}
		public DataTemplate NavButtonTemplate {
			get { return (DataTemplate)GetValue(NavButtonTemplateProperty); }
			set { SetValue(NavButtonTemplateProperty, value); }
		}
		public DataTemplateSelector NavButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(NavButtonTemplateSelectorProperty); }
			set { SetValue(NavButtonTemplateSelectorProperty, value); }
		}
		public Style NavButtonStyle {
			get { return (Style)GetValue(NavButtonStyleProperty); }
			set { SetValue(NavButtonStyleProperty, value); }
		}
		NavButtonCollection _NavButtons;
		[Bindable(true), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		public NavButtonCollection NavButtons {
			get {
				if(_NavButtons == null) _NavButtons = new NavButtonCollection(this);
				return _NavButtons;
			}
		}
		NavButtonCollection _DefaultButtons;
		NavButtonCollection DefaultButtons {
			get {
				if(_DefaultButtons == null) _DefaultButtons = new NavButtonCollection(this);
				return _DefaultButtons;
			}
		}
		public NavElementBase SelectedElement {
			get { return (NavElementBase)GetValue(SelectedElementProperty); }
			private set { SetValue(SelectedElementProperty, value); }
		}
		public DataTemplate GroupHeaderTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		public Style GroupHeaderStyle {
			get { return (Style)GetValue(GroupHeaderStyleProperty); }
			set { SetValue(GroupHeaderStyleProperty, value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return Categories.ToList().GetEnumerator(); }
		}
		public bool ContinuousNavigation {
			get { return (bool)GetValue(ContinuousNavigationProperty); }
			set { SetValue(ContinuousNavigationProperty, value); }
		}
		public bool CloseOnOuterClick {
			get { return (bool)GetValue(CloseOnOuterClickProperty); }
			set { SetValue(CloseOnOuterClickProperty, value); }
		}
		public FlyoutShowMode FlyoutShowMode {
			get { return (FlyoutShowMode)GetValue(FlyoutShowModeProperty); }
			set { SetValue(FlyoutShowModeProperty, value); }
		}
		public bool ShowItemShadow {
			get { return (bool)GetValue(ShowItemShadowProperty); }
			set { SetValue(ShowItemShadowProperty, value); }
		}
		public FlyoutShowDirection FlyoutShowDirection {
			get { return (FlyoutShowDirection)GetValue(FlyoutShowDirectionProperty); }
			set { SetValue(FlyoutShowDirectionProperty, value); }
		}
		#region IFlyoutProvider Members
		FlyoutControl IFlyoutProvider.FlyoutControl {
			get { return PartFlyoutControl; }
		}
		Editors.Flyout.FlyoutPlacement IFlyoutProvider.Placement {
			get {
				return FlyoutShowDirection == FlyoutShowDirection.Default ? Editors.Flyout.FlyoutPlacement.Bottom : Editors.Flyout.FlyoutPlacement.Top;
			}
		}
		IFlyoutEventListener IFlyoutProvider.FlyoutEventListener {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		#endregion
	}
}
