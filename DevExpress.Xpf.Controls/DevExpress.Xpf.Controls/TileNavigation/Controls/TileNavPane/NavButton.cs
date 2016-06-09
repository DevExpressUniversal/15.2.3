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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Navigation {
	interface ITileNavButton {
		NavButtonCollection OwnerCollection { get; set; }
		bool IsMain { get; }
	}
	[ContentProperty("Items")]
	public class NavButton : veButtonBase, INavElement, IFlyoutEventListener, ITileNavButton {
		#region static
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		protected static readonly DependencyPropertyKey IsFlyoutButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsFlyoutButtonVisibleProperty;
		public static readonly DependencyProperty ShowFlyoutButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty AllowSelectionProperty;
		public static readonly DependencyProperty IsMainProperty;
		public static readonly DependencyProperty ItemsProperty;
		static readonly DependencyPropertyKey ItemsPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		static readonly DependencyPropertyKey HasItemsPropertyKey;
		public static readonly DependencyProperty HasItemsProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsPresenterProperty;
		static NavButton() {
			Type ownerType = typeof(NavButton);
			var dProp = new DependencyPropertyRegistrator<NavButton>();
			dProp.Register("IsMain", ref IsMainProperty, false, OnIsMainChanged);
			dProp.RegisterReadonly("IsFlyoutButtonVisible", ref IsFlyoutButtonVisiblePropertyKey, ref IsFlyoutButtonVisibleProperty, false, null,
				OnCoerceIsFlyoutButtonVisible);
			dProp.Register("ShowFlyoutButton", ref ShowFlyoutButtonProperty, true, OnShowFlyoutButtonChanged);
			dProp.Register("AllowGlyphTheming", ref AllowGlyphThemingProperty, false);
			dProp.OverrideFrameworkMetadata(HorizontalAlignmentProperty, HorizontalAlignment.Stretch, (d, e) =>
			{
				((NavButton)d).OnHorizontalAlignmentChanged();
			});
			ItemsPropertyKey = DependencyProperty.RegisterReadOnly("Items", typeof(NavElementCollection), ownerType, new PropertyMetadata(null));
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			ItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<NavButton, NavElementBase>), ownerType, new UIPropertyMetadata(null));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(bool), ownerType, new PropertyMetadata(false));
			HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
			AllowSelectionProperty = DependencyProperty.Register("AllowSelection", typeof(bool), ownerType, new PropertyMetadata(false));
			ItemsPresenterProperty = DependencyProperty.RegisterAttached("ItemsPresenter", typeof(UIElement), typeof(NavButton));
		}
		protected virtual void OnHorizontalAlignmentChanged() {
			Panel parentPanel = Parent as Panel ?? VisualTreeHelper.GetParent(this) as Panel;
			if(parentPanel != null) parentPanel.InvalidateMeasure();
		}
		static void OnItemsSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((NavButton)d).OnItemsSourceChanged(e);
		}
		static void OnItemTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((NavButton)d).OnItemTemplateChanged(e);
		}
		static void OnIsMainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavButton)d).OnIsMainChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		internal static UIElement GetItemsPresenter(DependencyObject target) {
			return (UIElement)target.GetValue(ItemsPresenterProperty);
		}
		internal static void SetItemsPresenter(DependencyObject target, UIElement value) {
			target.SetValue(ItemsPresenterProperty, value);
		}
		private static void OnShowFlyoutButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NavButton navButton = d as NavButton;
			if(navButton != null)
				navButton.OnShowFlyoutButtonChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static object OnCoerceIsFlyoutButtonVisible(DependencyObject d, object value) {
			NavButton navButton = d as NavButton;
			if(navButton != null)
				return navButton.OnCoerceIsFlyoutButtonVisible((bool)value);
			else
				return value;
		}
		#endregion
		public NavButton() {
			DefaultStyleKey = typeof(NavButton);
			Items = new NavElementCollection(this);
			Items.CollectionChanged += OnItemsCollectionChanged;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			TileNavPane tileNavPane = GetTileNavPane();
			if(tileNavPane != null) {
				if(!tileNavPane.ContinuousNavigation) {
					IsChecked = false;
				} else {
					if(((IFlyoutProvider)tileNavPane).FlyoutControl == null || !((IFlyoutProvider)tileNavPane).FlyoutControl.IsOpen) {
						IsChecked = false;
					}
				}
			}
		}
		internal UIElement GetItemsPresenter() {
			UIElement attached = GetItemsPresenter(this);
			return attached ?? ItemsPresenter;
		}
		protected virtual UIElement CreateItemsPresenter() {
			var itemsPresenter = new  TileNavPaneBar();
			itemsPresenter.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("Items") { Source = this });
			itemsPresenter.SetBinding(ItemsControl.ItemContainerStyleProperty, new Binding("ItemStyle") { Source = this });
			return itemsPresenter;
		}
		void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			HasItems = Items.Count > 0;
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		private void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<NavButton, NavElementBase>.OnItemsSourcePropertyChanged(this,
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
			ItemsAttachedBehaviorCore<NavButton, NavElementBase>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(TileNavItem.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected virtual TileNavItem CreateItem() {
			return new TileNavItem();
		}
		protected virtual void OnIsMainChanged(bool oldValue, bool newValue) {
			SetMainButtonInOwnerCollection();
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		void SetMainButtonInOwnerCollection() {
			if(OwnerCollection != null && IsMain) OwnerCollection.SetMainButton(this);
		}
		private void OnIsCheckedChanged(bool value) {
			UpdateState(false);
		}
		protected override ControlControllerBase CreateController() {
			return new NavButtonController(this);
		}
		private TileNavPane GetTileNavPane() {
			if(TileNavPane != null) return TileNavPane;
			INavElement navElement = this;
			INavElement parent = navElement.NavParent;
			while(parent != null) {
				if(parent.TileNavPane != null) return parent.TileNavPane;
				parent = parent.NavParent;
			}
			return null;
		}
		protected virtual void OnShowFlyoutButtonChanged(bool oldValue, bool newValue) {
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		protected virtual object OnCoerceIsFlyoutButtonVisible(bool value) {
			return ShowFlyoutButton && (HasItems || IsMain);
		}
		#region INavElement Members
		INavElement INavElement.NavParent { get; set; }
		bool INavElement.AllowSelection {
			get { return (bool)GetValue(AllowSelectionProperty); }
			set { SetValue(AllowSelectionProperty, value); }
		}
		protected TileNavPane TileNavPane { get; private set; }
		TileNavPane INavElement.TileNavPane {
			get { return GetTileNavPane(); }
			set { TileNavPane = value; }
		}
		#endregion
		UIElement _ItemsPresenter;
		internal UIElement ItemsPresenter {
			get {
				if(_ItemsPresenter == null) {
					_ItemsPresenter = CreateItemsPresenter();
				}
				return _ItemsPresenter;
			}
		}
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
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 0, XtraSerializationFlags.None)]
		public NavElementCollection Items {
			get { return (NavElementCollection)GetValue(ItemsProperty); }
			internal set { SetValue(ItemsPropertyKey, value); }
		}
		public bool HasItems {
			get { return (bool)GetValue(HasItemsProperty); }
			internal set { SetValue(HasItemsPropertyKey, value); }
		}
		internal bool AllowSelection {
			get { return (bool)GetValue(AllowSelectionProperty); }
			set { SetValue(AllowSelectionProperty, value); }
		}
		public bool IsFlyoutButtonVisible {
			get { return (bool)GetValue(IsFlyoutButtonVisibleProperty); }
		}
		public bool ShowFlyoutButton {
			get { return (bool)GetValue(ShowFlyoutButtonProperty); }
			set { SetValue(ShowFlyoutButtonProperty, value); }
		}
		public bool AllowGlyphTheming {
			get { return (bool)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
		private bool _IsChecked;
		internal bool IsChecked {
			get { return _IsChecked; }
			set {
				if(_IsChecked == value) return;
				_IsChecked = value;
				OnIsCheckedChanged(value);
			}
		}
		public bool IsMain {
			get { return (bool)GetValue(IsMainProperty); }
			set { SetValue(IsMainProperty, value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); }
		}
		#region IFlyoutTarget Members
		void IFlyoutEventListener.OnFlyoutClosed() {
			IsChecked = false;
		}
		void IFlyoutEventListener.OnFlyoutOpened() {
			IsChecked = true;
		}
		void IFlyoutEventListener.OnMouseLeave() { }
		void IFlyoutEventListener.OnMouseEnter() { }
		WindowsUI.Internal.Flyout.FlyoutBase IFlyoutEventListener.Flyout { get { return null; } }
		#endregion
		class NavButtonController : ButtonBaseController {
			public new NavButton Control { get { return (NavButton)base.Control; } }
			public NavButtonController(NavButton control)
				: base(control) {
			}
			protected override void OnClick() {
				base.OnClick();
				Control.RaiseEvent(new RoutedEventArgs(TileNavPane.ClickEvent));
			}
			public override void UpdateState(bool useTransitions) {
				VisualStateManager.GoToState(Control, "EmptyCheckedState", useTransitions);
				VisualStateManager.GoToState(Control, Control.IsChecked ? "Checked" : "Unchecked", useTransitions);
				if(!Control.IsChecked || !Control.IsEnabled)
					base.UpdateState(useTransitions);
			}
		}
		#region ITileNavButton Members
		NavButtonCollection OwnerCollection;
		NavButtonCollection ITileNavButton.OwnerCollection {
			get { return OwnerCollection; }
			set { OwnerCollection = value; }
		}
		bool ITileNavButton.IsMain {
			get { return IsMain; }
		}
		#endregion
		#region ILogicalOwner Members
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
			if(!logicalChildren.Contains(child))
				logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
		#region IFlyoutEventListener Members
		void IFlyoutEventListener.OnFlyoutClosed(bool onClickThrough) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class NavButtonCollection : ObservableCollectionCore<NavButton> {
		internal TileNavPane Owner { get; private set; }
		internal NavButtonCollection(TileNavPane owner) {
			Owner = owner;
		}
		public void Add(params NavButton[] items) {
			Array.ForEach(items, Add);
		}
		protected override void InsertItem(int index, NavButton item) {
			base.InsertItem(index, item);
			((ITileNavButton)item).OwnerCollection = this;
			((INavElement)item).TileNavPane = Owner;
			if(item.IsMain) SetMainButton(item);
			item.Click += item_Click;
		}
		protected override void RemoveItem(int index) {
			var item = this[index];
			if(item != null) {
				item.Click -= item_Click;
				((ITileNavButton)item).OwnerCollection = null;
				((INavElement)item).TileNavPane = null;
			}
			base.RemoveItem(index);
		}
		void item_Click(object sender, EventArgs e) {
		}
		internal void SetMainButton(NavButton mainButton) {
			foreach(NavButton bt in this) {
				if(mainButton.Equals(bt)) continue;
				bt.IsMain = false;
			}
		}
	}
}
namespace DevExpress.Xpf.Navigation.Internal {
	public class SelectedNavButton : NavButton {
		static SelectedNavButton() {
			var dProp = new DependencyPropertyRegistrator<SelectedNavButton>();
			dProp.OverrideMetadata(AllowSelectionProperty, true);
			dProp.OverrideFrameworkMetadata(HorizontalAlignmentProperty, HorizontalAlignment.Left);
		}
		internal SelectedNavButton() {
			DefaultStyleKey = typeof(SelectedNavButton);
		}
		protected override void OnHorizontalAlignmentChanged() {
		}
		protected override UIElement CreateItemsPresenter() {
			return new TileNavPaneBar();
		}
	}
	public class NavButtonSeparator : SelectedNavButton {
		public NavButtonSeparator() {
			DefaultStyleKey = typeof(NavButtonSeparator);
		}
	}
	public class NavButtonPanel : SplitLayoutPanel {
		#region static
		public static readonly DependencyProperty GlyphSpaceProperty;
		static NavButtonPanel() {
			var dProp = new DependencyPropertyRegistrator<NavButtonPanel>();
			dProp.Register("GlyphSpace", ref GlyphSpaceProperty, 0d, OnGlyphSpaceChanged);
		}
		static void OnGlyphSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavButtonPanel)d).OnGlyphSpaceChanged((double)e.NewValue);
		}
		#endregion
		public double GlyphSpace {
			get { return (double)GetValue(GlyphSpaceProperty); }
			set { SetValue(GlyphSpaceProperty, value); }
		}
		void OnGlyphSpaceChanged(double value) {
			InvalidateMeasure();
		}
		protected override Size MeasureOverride(Size availableSize) {
			EnsureContent1Margin();
			return base.MeasureOverride(availableSize);
		}
		static bool IsPropertyHasDefaultOrInheritedValue(DependencyObject dObj, DependencyProperty property) {
			return System.Windows.DependencyPropertyHelper.GetValueSource(dObj, property).BaseValueSource <= BaseValueSource.Inherited;
		}
		void EnsureContent1Margin() {			
			switch(Content1Location) {
				case Dock.Bottom:
					if(IsPropertyHasDefaultOrInheritedValue(this, BottomContent1MarginProperty))
						SetCurrentValue(BottomContent1MarginProperty, new Thickness(0, GlyphSpace, 0, 0));
					return;
				case Dock.Left:
					if(IsPropertyHasDefaultOrInheritedValue(this, LeftContent1MarginProperty))
						SetCurrentValue(LeftContent1MarginProperty, new Thickness(0, 0, GlyphSpace, 0));
					return;
				case Dock.Right:
					if(IsPropertyHasDefaultOrInheritedValue(this, RightContent1MarginProperty))
						SetCurrentValue(RightContent1MarginProperty, new Thickness(GlyphSpace, 0, 0, 0));
					return;
				case Dock.Top:
					if(IsPropertyHasDefaultOrInheritedValue(this, TopContent1MarginProperty))
						SetCurrentValue(TopContent1MarginProperty, new Thickness(0, 0, 0, GlyphSpace));
					return;
			}
		}
	}
}
