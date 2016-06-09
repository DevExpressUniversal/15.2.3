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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_HeadersPanelPresenter", Type = typeof(ItemsPresenter))]
	[TemplatePart(Name = "PART_SelectedPage", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "PART_ControlBox", Type = typeof(BaseControlBoxControl))]
	public abstract class LayoutTabControl : psvContentSelectorControl<BaseLayoutItem> {
		#region static
		public static readonly DependencyProperty LayoutItemProperty;
		public static readonly DependencyProperty CaptionLocationProperty;
		public static readonly DependencyProperty CaptionOrientationProperty;
		public static readonly DependencyProperty TabHeaderLayoutTypeProperty;
		public static readonly DependencyProperty IsAutoFillHeadersProperty;
		public static readonly DependencyProperty ScrollIndexProperty;
		public static readonly DependencyProperty DestroyContentOnTabSwitchingProperty;
		public static readonly DependencyProperty TabContentCacheModeProperty;
		public static readonly DependencyProperty ActualTabContentCacheModeProperty;
		public static readonly DependencyProperty ShowTabForSinglePageProperty;
		static LayoutTabControl() {
			var dProp = new DependencyPropertyRegistrator<LayoutTabControl>();
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((LayoutTabControl)dObj).OnLayoutItemChanged((BaseLayoutItem)e.NewValue));
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, e) => ((LayoutTabControl)dObj).OnCaptionLocationChanged((CaptionLocation)e.NewValue));
			dProp.Register("CaptionOrientation", ref CaptionOrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((LayoutTabControl)dObj).OnCaptionOrientationChanged((Orientation)e.NewValue));
			dProp.Register("TabHeaderLayoutType", ref TabHeaderLayoutTypeProperty, TabHeaderLayoutType.Default,
				(dObj, e) => ((LayoutTabControl)dObj).OnTabHeaderLayoutTypeChanged((TabHeaderLayoutType)e.NewValue));
			dProp.Register("IsAutoFillHeaders", ref IsAutoFillHeadersProperty, false,
				(dObj, e) => ((LayoutTabControl)dObj).OnIsAutoFillHeadersChanged((bool)e.NewValue));
			dProp.Register("ScrollIndex", ref ScrollIndexProperty, (int)0,
				(dObj, e) => ((LayoutTabControl)dObj).OnScrollIndexChanged((int)e.NewValue));
			dProp.Register("ShowTabForSinglePage", ref ShowTabForSinglePageProperty, true,
				(dObj, e) => ((LayoutTabControl)dObj).OnShowTabForSinglePageChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("DestroyContentOnTabSwitching", ref DestroyContentOnTabSwitchingProperty, true,
				(dObj, e) => ((LayoutTabControl)dObj).OnTabCacheModeChanged());
			dProp.Register("TabContentCacheMode", ref TabContentCacheModeProperty, TabContentCacheMode.None,
				(dObj, e) => ((LayoutTabControl)dObj).OnTabCacheModeChanged());
			dProp.Register("ActualTabContentCacheMode", ref ActualTabContentCacheModeProperty, TabContentCacheMode.None,
				(dObj, e) => ((LayoutTabControl)dObj).OnActualTabCacheModeChanged((TabContentCacheMode)e.OldValue, (TabContentCacheMode)e.NewValue));
		}
		#endregion static
		protected override void OnDispose() {
			if(PartFastRenderPanel != null) {
				PartFastRenderPanel.Dispose();
				PartFastRenderPanel = null;
			}
			ClearValue(LayoutItemProperty);
			base.OnDispose();
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		public LayoutGroup LayoutGroup { get { return LayoutItem as LayoutGroup; } }
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item) {
			OnLayoutGroupChanged(item as LayoutGroup);
		}
		protected virtual void OnLayoutGroupChanged(LayoutGroup group) {
			if(group == null) {
				ClearValue(DockLayoutManager.LayoutItemProperty);
				if(!IsDisposing)
					ClearItemsSource();
				ClearGroupBindings(group);
			}
			else {
				SetValue(DockLayoutManager.LayoutItemProperty, group);
				SetValue(ItemsSourceProperty, group.Items);
				SetGroupBindings(group);
			}
		}
		protected virtual void SetGroupBindings(LayoutGroup group) {
			BindingHelper.SetBinding(this, SelectedIndexProperty, group, LayoutGroup.SelectedTabIndexProperty, System.Windows.Data.BindingMode.TwoWay);
			BindingHelper.SetBinding(this, TabHeaderLayoutTypeProperty, group, LayoutGroup.TabHeaderLayoutTypeProperty);
			BindingHelper.SetBinding(this, CaptionLocationProperty, group, LayoutGroup.CaptionLocationProperty);
			BindingHelper.SetBinding(this, CaptionOrientationProperty, group, LayoutGroup.CaptionOrientationProperty);
			BindingHelper.SetBinding(this, IsAutoFillHeadersProperty, group, LayoutGroup.TabHeadersAutoFillProperty);
			BindingHelper.SetBinding(this, ScrollIndexProperty, group, LayoutGroup.TabHeaderScrollIndexProperty);
			BindingHelper.SetBinding(this, DestroyContentOnTabSwitchingProperty, LayoutGroup, LayoutGroup.DestroyContentOnTabSwitchingProperty);
			BindingHelper.SetBinding(this, TabContentCacheModeProperty, LayoutGroup, LayoutGroup.TabContentCacheModeProperty);
			BindingHelper.SetBinding(this, ItemContainerStyleProperty, group, LayoutGroup.TabItemContainerStyleProperty);
			BindingHelper.SetBinding(this, ItemContainerStyleSelectorProperty, group, LayoutGroup.TabItemContainerStyleSelectorProperty);
			if(group is TabbedGroup)
				BindingHelper.SetBinding(this, ShowTabForSinglePageProperty, group, TabbedGroup.ShowTabForSinglePageProperty);
		}
		protected virtual void ClearGroupBindings(LayoutGroup group) {
			BindingHelper.ClearBinding(this, SelectedIndexProperty);
			BindingHelper.ClearBinding(this, TabHeaderLayoutTypeProperty);
			BindingHelper.ClearBinding(this, CaptionLocationProperty);
			BindingHelper.ClearBinding(this, CaptionOrientationProperty);
			BindingHelper.ClearBinding(this, IsAutoFillHeadersProperty);
			BindingHelper.ClearBinding(this, ScrollIndexProperty);
			BindingHelper.ClearBinding(this, DestroyContentOnTabSwitchingProperty);
			BindingHelper.ClearBinding(this, TabContentCacheModeProperty);
			BindingHelper.ClearBinding(this, ItemContainerStyleProperty);
			BindingHelper.ClearBinding(this, ItemContainerStyleSelectorProperty);
			BindingHelper.ClearBinding(this, ShowTabForSinglePageProperty);
		}
		public Orientation CaptionOrientation {
			get { return (Orientation)GetValue(CaptionOrientationProperty); }
			set { SetValue(CaptionOrientationProperty, value); }
		}
		public CaptionLocation CaptionLocation {
			get { return (Docking.CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
		public bool IsAutoFillHeaders {
			get { return (bool)GetValue(IsAutoFillHeadersProperty); }
			set { SetValue(IsAutoFillHeadersProperty, value); }
		}
		public int ScrollIndex {
			get { return (int)GetValue(ScrollIndexProperty); }
			set { SetValue(ScrollIndexProperty, value); }
		}
		public TabHeaderLayoutType TabHeaderLayoutType {
			get { return (TabHeaderLayoutType)GetValue(TabHeaderLayoutTypeProperty); }
			set { SetValue(TabHeaderLayoutTypeProperty, value); }
		}
		public bool DestroyContentOnTabSwitching {
			get { return (bool)GetValue(DestroyContentOnTabSwitchingProperty); }
			set { SetValue(DestroyContentOnTabSwitchingProperty, value); }
		}
		public TabContentCacheMode TabContentCacheMode {
			get { return (TabContentCacheMode)GetValue(TabContentCacheModeProperty); }
			set { SetValue(TabContentCacheModeProperty, value); }
		}
		public TabContentCacheMode ActualTabContentCacheMode {
			get { return (TabContentCacheMode)GetValue(ActualTabContentCacheModeProperty); }
			set { SetValue(ActualTabContentCacheModeProperty, value); }
		}
		public bool ShowTabForSinglePage {
			get { return (bool)GetValue(ShowTabForSinglePageProperty); }
			set { SetValue(ShowTabForSinglePageProperty, value); }
		}
		protected override void PrepareSelectorItem(psvSelectorItem selectorItem, BaseLayoutItem item) {
			base.PrepareSelectorItem(selectorItem, item);
			TabbedPaneItem tabItem = selectorItem as TabbedPaneItem;
			if(tabItem != null) {
				tabItem.CaptionOrientation = CaptionOrientation;
				tabItem.CaptionLocation = CaptionLocation;
			}
			item.EnsureTemplate();
		}
		protected virtual void OnCaptionLocationChanged(CaptionLocation captionLocation) {
			if(PartHeadersPanel != null)
				PartHeadersPanel.Orientation = HeadersPanelHelper.GetOrientation(CaptionLocation);
			UpdateItemsLocation();
		}
		protected virtual void OnCaptionOrientationChanged(Orientation orientation) {
			UpdateItemsOrientation();
		}
		protected virtual void OnIsAutoFillHeadersChanged(bool autoFill) {
			if(PartHeadersPanel != null)
				PartHeadersPanel.IsAutoFillHeaders = autoFill;
		}
		protected virtual void OnScrollIndexChanged(int index) {
			if(PartHeadersPanel != null)
				PartHeadersPanel.ScrollIndex = index;
		}
		protected virtual void OnTabHeaderLayoutTypeChanged(TabHeaderLayoutType type) {
			if(PartHeadersPanel != null)
				PartHeadersPanel.TabHeaderLayoutType = type;
		}
		void UpdateItemsOrientation() {
			for(int i = 0; i < Items.Count; i++) {
				TabbedPaneItem item = ItemContainerGenerator.ContainerFromIndex(i) as TabbedPaneItem;
				if(item != null)
					item.CaptionOrientation = CaptionOrientation;
			}
		}
		void UpdateItemsLocation() {
			for(int i = 0; i < Items.Count; i++) {
				TabbedPaneItem item = ItemContainerGenerator.ContainerFromIndex(i) as TabbedPaneItem;
				if(item != null)
					item.CaptionLocation = CaptionLocation;
			}
		}
		protected abstract IView GetView(DockLayoutManager container);
		protected void CheckSelectionInGroup() {
			LayoutGroup group = DockLayoutManager.GetLayoutItem(this) as LayoutGroup;
			if(group != null) {
				int index = SelectedIndex;
				group.CoerceValue(LayoutGroup.SelectedItemProperty);
				TabbedGroup tGroup = group as TabbedGroup;
				if(tGroup != null && tGroup.SelectedTabIndex != index)
					tGroup.SelectedTabIndex = index;
			}
		}
		protected virtual void OnShowTabForSinglePageChanged(bool oldValue, bool newValue) {
			UpdatePanelMeasure();
		}
		void UpdatePanelMeasure() {
			if(PartHeadersPanel != null)
				PartHeadersPanel.AllowChildrenMeasure = !HasSinglePage || ShowTabForSinglePage;
		}
		public ItemsPresenter PartHeadersPresenter { get; private set; }
		public FrameworkElement PartSelectedPage { get; private set; }
		public BaseControlBoxControl PartControlBox { get; private set; }
		public TabHeadersPanel PartHeadersPanel { get; private set; }
		public LayoutTabFastRenderPanel PartFastRenderPanel { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartHeadersPresenter = GetTemplateChild("PART_HeadersPanelPresenter") as ItemsPresenter;
			if(PartHeadersPresenter != null) {
				PartHeadersPresenter.SizeChanged += new SizeChangedEventHandler(PartHeadersPanel_SizeChanged);
			}
			PartSelectedPage = GetTemplateChild("PART_SelectedPage") as FrameworkElement;
			PartControlBox = GetTemplateChild("PART_ControlBox") as BaseControlBoxControl;
			if(PartFastRenderPanel != null && !LayoutItemsHelper.IsTemplateChild(PartFastRenderPanel, this)) {
				PartFastRenderPanel.Dispose();
			}
			PartFastRenderPanel = GetTemplateChild("PART_FastRenderPanel") as LayoutTabFastRenderPanel;
			if(PartFastRenderPanel != null) {
				PartFastRenderPanel.Initialize(this, ActualTabContentCacheMode);
			}
			UpdateVisualState();
		}
		void PartHeadersPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
			PartHeadersPresenter.SizeChanged -= new SizeChangedEventHandler(PartHeadersPanel_SizeChanged);
			PartHeadersPanel = LayoutItemsHelper.GetTemplateChild<TabHeadersPanel>(PartHeadersPresenter);
			if(PartHeadersPanel != null) {
				EnsureItemsPanelCore(PartHeadersPanel);
			}
		}
		bool HasSinglePage { get { return Items.Count == 1; } }
		protected virtual void UpdateVisualState() {
			VisualStateManager.GoToState(this, (HasSinglePage && !ShowTabForSinglePage) ? "HeaderHidden" : "HeaderVisible", false);
		}
		protected virtual void EnsureItemsPanelCore(TabHeadersPanel partHeadersPanel) {
			PartHeadersPanel.Orientation = HeadersPanelHelper.GetOrientation(CaptionLocation);
			PartHeadersPanel.TabHeaderLayoutType = TabHeaderLayoutType;
			PartHeadersPanel.IsAutoFillHeaders = IsAutoFillHeaders;
			PartHeadersPanel.ScrollIndex = ScrollIndex;
			UpdatePanelMeasure();
		}
		protected void EnsureSelectedContent() {
			if(SelectedContent != null)
				SelectedContent.SelectTemplate();
		}
		protected override void OnSelectedItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnSelectedItemChanged(item, oldItem);
			if(PartHeadersPanel != null) PartHeadersPanel.InvalidateMeasure();
		}
		protected override void OnSelectedContentChanged(BaseLayoutItem newValue, BaseLayoutItem oldValue) {
			base.OnSelectedContentChanged(newValue, oldValue);
			if(Container != null) Container.InvalidateView(LayoutGroup);
			if(oldValue != null) {
				if(ActualTabContentCacheMode == Core.TabContentCacheMode.None) {
					using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { oldValue })) {
						oldValue.ClearTemplate();
					}
				}
			}
			if(newValue != null) {
				newValue.SelectTemplate();
			}
			RaiseSelectedContentChanged(oldValue, newValue);
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if(ActualTabContentCacheMode == Core.TabContentCacheMode.CacheAllTabs) {
				foreach(var x in Items) {
					BaseLayoutItem item = x as BaseLayoutItem;
					if(item != null) item.SelectTemplate();
				}
			}
			RaiseItemsChanged(e);
			UpdatePanelMeasure();
			UpdateVisualState();
		}
		public event LayoutTabControlSelectionChangedEventHandler SelectionChanged;
		protected virtual void RaiseSelectedContentChanged(object oldContent, object content) {
			if(SelectionChanged == null) return;
			SelectionChanged(this, new LayoutTabControlSelectionChangedEventArgs(oldContent, content));
		}
		public event NotifyCollectionChangedEventHandler ItemsChanged;
		protected virtual void RaiseItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(ItemsChanged == null) return;
			ItemsChanged(this, e);
		}
		protected virtual void OnTabCacheModeChanged() {
			if(!DestroyContentOnTabSwitching) ActualTabContentCacheMode = Core.TabContentCacheMode.CacheTabsOnSelecting;
			else ActualTabContentCacheMode = TabContentCacheMode;
		}
		protected virtual void OnActualTabCacheModeChanged(TabContentCacheMode oldValue, TabContentCacheMode newValue) {
			if(PartFastRenderPanel != null)
				PartFastRenderPanel.Initialize(this, newValue);
		}
		protected override void ClearSelectorItem(psvSelectorItem selectorItem) {
			if(selectorItem.LayoutItem != null) selectorItem.LayoutItem.ClearTemplate();
			base.ClearSelectorItem(selectorItem);
		}
	}
	public delegate void LayoutTabControlSelectionChangedEventHandler(object sender, LayoutTabControlSelectionChangedEventArgs e);
	public class LayoutTabControlSelectionChangedEventArgs : EventArgs {
		public object OldContent { get; private set; }
		public object NewContent { get; private set; }
		public LayoutTabControlSelectionChangedEventArgs(object oldContent, object newContent) {
			OldContent = oldContent;
			NewContent = newContent;
		}
	}
	public class LayoutTabFastRenderPanel : psvPanel {
		public LayoutTabControl Owner { get; private set; }
		TabContentCacheMode tabContentCacheMode;
		public TabContentCacheMode TabContentCacheMode {
			get { return tabContentCacheMode; }
			set {
				if(tabContentCacheMode == value) return;
				tabContentCacheMode = value;
				OnTabContentCacheModeChanged();
			}
		}
		protected override void OnDispose() {
			UnsubscribeOwner();
			ClearChildren();
			base.OnDispose();
		}
		private void ClearChildren() {
			ClearItemsHash();
			Children.Clear();
			SelectedContent = null;
		}
		void ClearItemsHash() {
			foreach(var cp in itemsHash.Values) {
				cp.ClearValue(FastRenderPanelContentControl.ContentProperty);
				Children.Remove(cp);
			}
			itemsHash.Clear();
		}
		void OnTabContentCacheModeChanged() {
			ClearChildren();
		}
		protected virtual void UnsubscribeOwner() {
			if(Owner == null) return;
			Owner.ItemsChanged -= Owner_ItemsChanged;
			Owner.SelectionChanged -= Owner_SelectionChanged;
		}
		protected virtual void SubscribeOwner() {
			if(Owner == null) return;
			Owner.ItemsChanged += Owner_ItemsChanged;
			Owner.SelectionChanged += Owner_SelectionChanged;
		}
		public void Initialize(LayoutTabControl owner, TabContentCacheMode cacheMode) {
			TabContentCacheMode = cacheMode;
			if(Owner != owner) {
				UnsubscribeOwner();
				Owner = owner;
				SubscribeOwner();
			}
			SyncItems(null, Owner.Items);
			SyncSelection(null, Owner.SelectedContent);
		}
		void Owner_SelectionChanged(object sender, LayoutTabControlSelectionChangedEventArgs e) {
			SyncItems(null, Owner.Items);
			SyncSelection(e.OldContent, e.NewContent);
		}
		void Owner_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				ClearChildren();
				return;
			}
			SyncItems(e.OldItems, e.NewItems);
			SyncSelection(null, Owner.SelectedContent);
		}
		Dictionary<object, FastRenderPanelContentControl> itemsHash = new Dictionary<object, FastRenderPanelContentControl>();
		void SyncItems(IList oldItems, IList newItems) {
			if(oldItems != null) {
				foreach(object item in oldItems) {
					if(itemsHash.ContainsKey(item)) {
						var cp = itemsHash[item];
						cp.ClearValue(FastRenderPanelContentControl.ContentProperty);
						Children.Remove(cp);
						itemsHash.Remove(item);
					}
				}
			}
			if(newItems != null) {
				foreach(object item in newItems) {
					if(!itemsHash.ContainsKey(item)) {
						FastRenderPanelContentControl presenter = new FastRenderPanelContentControl()
						{
							Content = item,
							ContentTemplate = Owner.SelectedContentTemplate
						};
						itemsHash.Add(item, presenter);
						Children.Add(presenter);
						if(TabContentCacheMode == TabContentCacheMode.CacheAllTabs) {
							presenter.Measure(DevExpress.Xpf.Core.Native.SizeHelper.Zero);
						}
						presenter.Visibility = System.Windows.Visibility.Collapsed;
					}
				}
			}
		}
		FastRenderPanelContentControl SelectedContent;
		void SyncSelection(object oldContent, object content) {
			if(content == null) {
				if(SelectedContent != null)
					SelectedContent = null;
				return;
			}
			var newSelectedContent = itemsHash[content];
			if(SelectedContent != null) {
				psvPanel.SetZIndex(SelectedContent, 0);
				if(SelectedContent != newSelectedContent)
					SelectedContent.Visibility = System.Windows.Visibility.Collapsed;
			}
			SelectedContent = newSelectedContent;
			psvPanel.SetZIndex(SelectedContent, 1);
			SelectedContent.Visibility = System.Windows.Visibility.Visible;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(SelectedContent != null) SelectedContent.Arrange(new Rect(new Point(), finalSize));
			else base.ArrangeOverride(finalSize);
			return finalSize;
		}
		Size lastMeasureAvailableSize = new Size();
		protected override Size MeasureOverride(Size availableSize) {
			if(SelectedContent == null) return base.MeasureOverride(availableSize);
			if(TabContentCacheMode == Core.TabContentCacheMode.None) {
				SelectedContent.Measure(availableSize);
			}
			else {
				if(lastMeasureAvailableSize == availableSize)
					SelectedContent.Measure(availableSize);
				else
					foreach(UIElement child in Children)
						child.Measure(availableSize);
			}
			lastMeasureAvailableSize = availableSize;
			return SelectedContent.DesiredSize;
		}
		class FastRenderPanelContentControl : ContentControl {
			static FastRenderPanelContentControl() {
			}
			private static object CoerceFlowDirection(DependencyObject d, object baseValue) {
				return FlowDirection.LeftToRight;
			}
			public FastRenderPanelContentControl() {
				Focusable = false;
				DevExpress.Xpf.Core.Native.ContentControlHelper.SetContentIsNotLogical(this, true);
			}
		}
	}
}
