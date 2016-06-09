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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Internal;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking {
	[ContentProperty("Items")]
	public class LayoutGroup : BaseLayoutItem, IGeneratorHost, ILogicalOwner {
		#region static
		public static readonly DependencyProperty IsLayoutRootProperty;
		internal static readonly DependencyPropertyKey IsLayoutRootPropertyKey;
		public static readonly DependencyProperty HasAccentProperty;
		public static readonly DependencyProperty ControlItemsHostProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty LayoutItemIntervalProperty;
		public static readonly DependencyProperty LayoutGroupIntervalProperty;
		public static readonly DependencyProperty DockItemIntervalProperty;
		static readonly DependencyPropertyKey ActualLayoutItemIntervalPropertyKey;
		static readonly DependencyPropertyKey ActualLayoutGroupIntervalPropertyKey;
		static readonly DependencyPropertyKey ActualDockItemIntervalPropertyKey;
		public static readonly DependencyProperty ActualLayoutItemIntervalProperty;
		public static readonly DependencyProperty ActualLayoutGroupIntervalProperty;
		public static readonly DependencyProperty ActualDockItemIntervalProperty;
		public static readonly DependencyProperty DestroyContentOnTabSwitchingProperty;
		public static readonly DependencyProperty TabContentCacheModeProperty;
		public static readonly DependencyProperty AllowSplittersProperty;
		public static readonly DependencyProperty HasSingleItemProperty;
		static readonly DependencyPropertyKey HasSingleItemPropertyKey;
		public static readonly DependencyProperty GroupBorderStyleProperty;
		static readonly DependencyPropertyKey IsSplittersEnabledPropertyKey;
		public static readonly DependencyProperty IsSplittersEnabledProperty;
		public static readonly DependencyProperty AllowExpandProperty;
		public static readonly DependencyProperty ExpandedProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		internal static readonly DependencyPropertyKey IsExpandedPropertyKey;
		public static readonly DependencyProperty GroupTemplateProperty;
		public static readonly DependencyProperty GroupTemplateSelectorProperty;
		public static readonly DependencyProperty ActualGroupTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualGroupTemplateSelectorPropertyKey;
		public static readonly DependencyProperty DestroyOnClosingChildrenProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasNotCollapsedItemsProperty;
		static readonly DependencyPropertyKey HasNotCollapsedItemsPropertyKey;
		public static readonly DependencyProperty HasVisibleItemsProperty;
		static readonly DependencyPropertyKey HasVisibleItemsPropertyKey;
		public static readonly DependencyProperty CaptionOrientationProperty;
		public static readonly RoutedEvent SelectedItemChangedEvent;
		static readonly DependencyPropertyKey SelectedItemPropertyKey;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedTabIndexProperty;
		public static readonly DependencyProperty TabHeaderLayoutTypeProperty;
		public static readonly DependencyProperty TabHeadersAutoFillProperty;
		public static readonly DependencyProperty TabHeaderHasScrollProperty;
		static readonly DependencyPropertyKey TabHeaderHasScrollPropertyKey;
		public static readonly DependencyProperty TabHeaderScrollIndexProperty;
		static readonly DependencyPropertyKey TabHeaderScrollIndexPropertyKey;
		public static readonly DependencyProperty TabHeaderMaxScrollIndexProperty;
		static readonly DependencyPropertyKey TabHeaderMaxScrollIndexPropertyKey;
		public static readonly DependencyProperty TabHeaderCanScrollPrevProperty;
		static readonly DependencyPropertyKey TabHeaderCanScrollPrevPropertyKey;
		public static readonly DependencyProperty TabHeaderCanScrollNextProperty;
		static readonly DependencyPropertyKey TabHeaderCanScrollNextPropertyKey;
		public static readonly DependencyProperty IsAnimatedProperty;
		static readonly DependencyPropertyKey IsAnimatedPropertyKey;
		public static readonly DependencyProperty ItemsAppearanceProperty;
		public static readonly DependencyProperty VisiblePagesCountProperty;
		static readonly DependencyPropertyKey VisiblePagesCountPropertyKey;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemCaptionTemplateProperty;
		public static readonly DependencyProperty ItemContentTemplateProperty;
		public static readonly DependencyProperty ItemCaptionTemplateSelectorProperty;
		public static readonly DependencyProperty ItemContentTemplateSelectorProperty;
		public static readonly DependencyProperty TabItemContainerStyleProperty;
		public static readonly DependencyProperty TabItemContainerStyleSelectorProperty;
		public static readonly DependencyProperty LastChildFillProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty OwnerGroupProperty;
		static LayoutGroup() {
			var dProp = new DependencyPropertyRegistrator<LayoutGroup>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(ShowCaptionProperty, false);
			dProp.OverrideMetadata(AllowFloatProperty, false);
			dProp.RegisterAttached("OwnerGroup", ref OwnerGroupProperty, (LayoutGroup)null);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal,
				(dObj, e) => ((LayoutGroup)dObj).OnOrientationChanged((Orientation)e.NewValue));
			dProp.Register("DestroyOnClosingChildren", ref DestroyOnClosingChildrenProperty, true, null,
				(dObj, value) => ((LayoutGroup)dObj).CoerceDestroyOnClosingChildren((bool)value));
			dProp.RegisterReadonly("IsLayoutRoot", ref IsLayoutRootPropertyKey, ref IsLayoutRootProperty, false,
				(dObj, e) => ((LayoutGroup)dObj).OnIsLayoutRootChanged((bool)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceIsLayoutRoot((bool)value));
			dProp.Register("HasAccent", ref HasAccentProperty, (bool?)null,
				(dObj, e) => ((LayoutGroup)dObj).OnHasAccentChanged((bool?)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceHasAccent((bool?)value));
			dProp.Register("ControlItemsHost", ref ControlItemsHostProperty, (bool?)null,
				(dObj, e) => ((LayoutGroup)dObj).OnControlItemsHostChanged((bool?)e.NewValue));
			dProp.Register("LayoutItemInterval", ref LayoutItemIntervalProperty, double.NaN,
				(dObj, e) => ((LayoutGroup)dObj).OnLayoutItemIntervalChanged((double)e.NewValue));
			dProp.Register("LayoutGroupInterval", ref LayoutGroupIntervalProperty, double.NaN,
				(dObj, e) => ((LayoutGroup)dObj).OnLayoutGroupIntervalChanged((double)e.NewValue));
			dProp.Register("DockItemInterval", ref DockItemIntervalProperty, double.NaN,
				(dObj, e) => ((LayoutGroup)dObj).OnDockItemIntervalChanged((double)e.NewValue));
			dProp.RegisterReadonly("ActualLayoutItemInterval", ref ActualLayoutItemIntervalPropertyKey, ref ActualLayoutItemIntervalProperty, 0.0,
				(dObj, e) => ((LayoutGroup)dObj).OnActualLayoutItemIntervalChanged((double)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceActualLayoutItemInterval((double)value));
			dProp.RegisterReadonly("ActualLayoutGroupInterval", ref ActualLayoutGroupIntervalPropertyKey, ref ActualLayoutGroupIntervalProperty, 0.0,
				(dObj, e) => ((LayoutGroup)dObj).OnActualLayoutGroupIntervalChanged((double)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceActualLayoutGroupInterval((double)value));
			dProp.RegisterReadonly("ActualDockItemInterval", ref ActualDockItemIntervalPropertyKey, ref ActualDockItemIntervalProperty, 0.0,
				(dObj, e) => ((LayoutGroup)dObj).OnActualDockItemIntervalChanged((double)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceActualDockItemInterval((double)value));
			dProp.Register("DestroyContentOnTabSwitching", ref DestroyContentOnTabSwitchingProperty, true);
			dProp.Register("TabContentCacheMode", ref TabContentCacheModeProperty, TabContentCacheMode.None);
			dProp.Register("AllowSplitters", ref AllowSplittersProperty, (bool?)null,
				(dObj, e) => ((LayoutGroup)dObj).OnAllowSplittersChanged((bool?)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceAllowSplitters((bool?)value));
			dProp.RegisterReadonly("IsSplittersEnabled", ref IsSplittersEnabledPropertyKey, ref IsSplittersEnabledProperty, true);
			dProp.Register("GroupBorderStyle", ref GroupBorderStyleProperty, GroupBorderStyle.NoBorder,
				(dObj, e) => ((LayoutGroup)dObj).OnGroupBorderStyleChanged((GroupBorderStyle)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceGroupBorderStyle((GroupBorderStyle)value));
			dProp.Register("AllowExpand", ref AllowExpandProperty, true,
				(dObj, e) => ((LayoutGroup)dObj).OnAllowExpandChanged((bool)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceAllowExpand((bool)value));
			dProp.Register("Expanded", ref ExpandedProperty, true,
				(dObj, e) => ((LayoutGroup)dObj).OnExpandedChanged((bool)e.NewValue));
			dProp.RegisterReadonly("IsExpanded", ref IsExpandedPropertyKey, ref IsExpandedProperty, true,
				(dObj, e) => ((LayoutGroup)dObj).OnIsExpandedChanged((bool)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceIsExpanded((bool)value));
			dProp.RegisterReadonly("HasSingleItem", ref HasSingleItemPropertyKey, ref HasSingleItemProperty, false,
				(dObj, e) => ((LayoutGroup)dObj).OnHasSingleItemChanged((bool)e.NewValue));
			dProp.Register("GroupTemplate", ref GroupTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((LayoutGroup)dObj).OnGroupTemplateChanged());
			dProp.Register("GroupTemplateSelector", ref GroupTemplateSelectorProperty, (DataTemplateSelector)new DefaultTemplateSelector(),
				(dObj, e) => ((LayoutGroup)dObj).OnGroupTemplateChanged());
			dProp.RegisterReadonly("ActualGroupTemplateSelector", ref ActualGroupTemplateSelectorPropertyKey, ref ActualGroupTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.RegisterReadonly("HasNotCollapsedItems", ref HasNotCollapsedItemsPropertyKey, ref HasNotCollapsedItemsProperty, false,
				(dObj, ea) => ((LayoutGroup)dObj).OnHasNotCollapsedItemsChanged((bool)ea.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceHasNotCollapsedItems((bool)value));
			dProp.RegisterReadonly("HasVisibleItems", ref HasVisibleItemsPropertyKey, ref HasVisibleItemsProperty, false, null,
				(dObj, value) => ((LayoutGroup)dObj).CoerceHasVisibleItems((bool)value));
			dProp.Register("CaptionOrientation", ref CaptionOrientationProperty, Orientation.Horizontal);
			dProp.RegisterReadonly("SelectedItem", ref SelectedItemPropertyKey, ref SelectedItemProperty, (BaseLayoutItem)null,
				(dObj, ea) => ((LayoutGroup)dObj).OnSelectedItemChanged((BaseLayoutItem)ea.NewValue, (BaseLayoutItem)ea.OldValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceSelectedItem((BaseLayoutItem)value));
			dProp.Register("SelectedTabIndex", ref SelectedTabIndexProperty, -1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, ea) => ((LayoutGroup)dObj).OnSelectedTabIndexChanged((int)ea.NewValue, (int)ea.OldValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceSelectedTabIndex((int)value));
			dProp.Register("TabHeaderLayoutType", ref TabHeaderLayoutTypeProperty, TabHeaderLayoutType.Default,
				(dObj, ea) => ((LayoutGroup)dObj).OnTabHeaderLayoutTypeChanged((TabHeaderLayoutType)ea.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceTabHeaderLayoutType((TabHeaderLayoutType)value));
			dProp.Register("TabHeadersAutoFill", ref TabHeadersAutoFillProperty, false,
				(dObj, ea) => ((LayoutGroup)dObj).OnTabHeadersAutoFillChanged((bool)ea.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceTabHeadersAutoFill((bool)value));
			dProp.RegisterReadonly("TabHeaderHasScroll", ref TabHeaderHasScrollPropertyKey, ref TabHeaderHasScrollProperty, false,
				(dObj, ea) => ((LayoutGroup)dObj).OnTabHeaderHasScrollChanged((bool)ea.NewValue));
			dProp.RegisterReadonly("TabHeaderScrollIndex", ref TabHeaderScrollIndexPropertyKey, ref TabHeaderScrollIndexProperty, 0, null,
				(dObj, value) => ((LayoutGroup)dObj).CoerceTabHeaderScrollIndex((int)value));
			dProp.RegisterReadonly("TabHeaderMaxScrollIndex", ref TabHeaderMaxScrollIndexPropertyKey, ref TabHeaderMaxScrollIndexProperty, -1, null,
				(dObj, value) => ((LayoutGroup)dObj).CoerceTabHeaderMaxScrollIndex((int)value));
			dProp.RegisterReadonly("TabHeaderCanScrollNext", ref TabHeaderCanScrollNextPropertyKey, ref TabHeaderCanScrollNextProperty, false);
			dProp.RegisterReadonly("TabHeaderCanScrollPrev", ref TabHeaderCanScrollPrevPropertyKey, ref TabHeaderCanScrollPrevProperty, false);
			dProp.RegisterReadonly("IsAnimated", ref IsAnimatedPropertyKey, ref IsAnimatedProperty, false,
				(dObj, e) => ((LayoutGroup)dObj).OnIsAnimatedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ItemsAppearance", ref ItemsAppearanceProperty, (Appearance)null,
				(dObj, e) => ((LayoutGroup)dObj).OnItemsAppearanceChanged((Appearance)e.NewValue),
				(dObj, value) => ((LayoutGroup)dObj).CoerceItemsAppearance((Appearance)value));
			dProp.RegisterReadonly("VisiblePagesCount", ref VisiblePagesCountPropertyKey, ref VisiblePagesCountProperty, 0, null,
				(dObj, value) => ((LayoutGroup)dObj).CoerceVisiblePagesCount((int)value));
			dProp.Register("ItemsSource", ref ItemsSourceProperty, (IEnumerable)null,
				(dObj, e) => ((LayoutGroup)dObj).OnItemsSourceChanged((IEnumerable)e.NewValue, (IEnumerable)e.OldValue));
			dProp.Register("ItemStyle", ref ItemStyleProperty, (Style)null,
				 (dObj, e) => ((LayoutGroup)dObj).OnItemStyleChanged((Style)e.NewValue, (Style)e.OldValue));
			dProp.Register("TabItemContainerStyle", ref TabItemContainerStyleProperty, (Style)null);
			dProp.Register("TabItemContainerStyleSelector", ref TabItemContainerStyleSelectorProperty, (StyleSelector)null);
			dProp.Register("ItemCaptionTemplate", ref ItemCaptionTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((LayoutGroup)dObj).OnItemTemplatePropertyChanged());
			dProp.Register("ItemContentTemplate", ref ItemContentTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((LayoutGroup)dObj).OnItemTemplatePropertyChanged());
			dProp.Register("LastChildFill", ref LastChildFillProperty, true);
			dProp.Register("ItemCaptionTemplateSelector", ref ItemCaptionTemplateSelectorProperty, (DataTemplateSelector)null,
			(dObj, e) => ((LayoutGroup)dObj).OnItemTemplatePropertyChanged());
			dProp.Register("ItemContentTemplateSelector", ref ItemContentTemplateSelectorProperty, (DataTemplateSelector)null,
				(dObj, e) => ((LayoutGroup)dObj).OnItemTemplatePropertyChanged());
			SelectedItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Direct, typeof(SelectedItemChangedEventHandler), typeof(LayoutGroup));
		}
		internal static LayoutGroup GetOwnerGroup(DependencyObject target) {
			return (LayoutGroup)target.GetValue(OwnerGroupProperty);
		}
		internal static void SetOwnerGroup(DependencyObject target, LayoutGroup value) {
			target.SetValue(OwnerGroupProperty, value);
		}
		#endregion static
		protected internal override IUIElement FindUIScopeCore() {
			return ParentPanel ?? base.FindUIScopeCore();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			RegisterView();
			foreach(BaseLayoutItem item in Items) item.OnParentLoaded();
			TabHeaderScrollIndexLocker.Unlock();
		}
		protected override void OnUnloaded() {
			foreach(BaseLayoutItem item in Items) item.OnParentUnloaded();
			if(fClearTemplateRequested) ClearTemplateCore();
			base.OnUnloaded();
		}
		protected bool fClearTemplateRequested;
		protected internal override void ClearTemplate() {
			if(fClearTemplateRequested) return;
			fClearTemplateRequested = true;
			Dispatcher.BeginInvoke(new Action(() => {
				if(fClearTemplateRequested) ClearTemplateCore();
			}));
			if(VisualParent == null) 
				this.ClearValue(DockLayoutManager.UIScopeProperty);
		}
		protected internal override void ClearTemplateCore() {
			fClearTemplateRequested = false;
			base.ClearTemplateCore();
		}
		protected internal override void SelectTemplate() {
			if(fClearTemplateRequested) {
				if(PartMultiTemplateControl != null && PartMultiTemplateControl.LayoutItem == this)
					PartMultiTemplateControl.ClearTemplateIfNeeded(this);
				fClearTemplateRequested = false;
			}
			base.SelectTemplate();
		}
		protected virtual void RegisterView() {
			DockLayoutManager scope = ((IUIElement)this).Scope as DockLayoutManager;
			if(scope != null)
				scope.RegisterViewIfNeeded(this);
		}
		bool MovePinnedPanel(int index, LayoutPanel panel) {
			if(panel == null || !panel.IsPinnedTab) return false;
			bool pinnedLeft = panel.TabPinLocation != TabHeaderPinLocation.Far;
			int desiredIndex = pinnedLeft ? index : index - Items.Count(x => !x.IsPinnedTab) - PinnedLeftItems.Count;
			var affectedItems = pinnedLeft ? PinnedLeftItems : PinnedRightItems;
			if(affectedItems.IsValidIndex(desiredIndex)) {
				int indexInCollection = affectedItems.IndexOf(panel);
				if(desiredIndex == indexInCollection) return false;
				affectedItems.Move(indexInCollection, desiredIndex);
				panel.NotifyViewPinStatusChanged();
				return true;
			}
			return false;
		}
		internal bool MoveItem(int index, BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null && panel.IsPinnedTab) return MovePinnedPanel(index, panel);
			int oldIndex = Items.IndexOf(item);
			if(oldIndex != index && Items.Count > 1) {
				if(index == Items.Count)
					index = Items.Count - 1;
				Items.Move(oldIndex, index);
				return true;
			}
			return false;
		}
		internal int IndexFromItem(BaseLayoutItem item) {
			return Items.IndexOf(item);
		}
		internal int TabIndexFromItem(BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null && panel.IsPinnedTab) {
				if(panel.TabPinLocation == TabHeaderPinLocation.Far) {
					return Items.Count(x => !x.IsPinnedTab) + PinnedLeftItems.Count + PinnedRightItems.IndexOf(panel);
				}
				else {
					return PinnedLeftItems.IndexOf(panel);
				}
			}
			else return Items.Where(x => !x.IsPinnedTab).ToList().IndexOf(item) + PinnedLeftItems.Count;
		}
		internal void OnItemPinStatusChanged(LayoutPanel panel) {
			bool pinStatus = panel.Pinned;
			if(pinStatus) AddToPinned(panel);
			else {
				RemoveFromPinned(panel);
				if(Items.Contains(panel))
					Items.Move(Items.IndexOf(panel), panel.TabPinLocation == TabHeaderPinLocation.Far ? Items.Count - 1 : 0);
			}
		}
		void AddToPinned(LayoutPanel panel) {
			RemoveFromPinned(panel);
			bool pinnedLeft = panel.TabPinLocation != TabHeaderPinLocation.Far;
			bool pinnedRight = panel.TabPinLocation == TabHeaderPinLocation.Far;
			if(pinnedLeft) PinnedLeftItems.Add(panel);
			if(pinnedRight) PinnedRightItems.Add(panel);
		}
		void RemoveFromPinned(LayoutPanel panel) {
			PinnedLeftItems.Remove(panel);
			PinnedRightItems.Remove(panel);
		}
		internal bool ContainsPinnedItem(LayoutPanel panel) {
			return PinnedLeftItems.Contains(panel) || PinnedRightItems.Contains(panel);
		}
		protected virtual DataTemplateSelector CreateDefaultItemTemplateSelector() {
			return new DefaultItemTemplateSelectorWrapper(GroupTemplateSelector, GroupTemplate);
		}
		protected virtual BaseLayoutItem CoerceSelectedItem(BaseLayoutItem item) {
			if(!IsTabHost) return item;
			return IsValid(SelectedTabIndex) ? Items[SelectedTabIndex] : null;
		}
		protected virtual void OnSelectedTabIndexChanged(int index, int oldIndex) {
			CoerceValue(SelectedItemProperty);
			foreach(BaseLayoutItem item in Items) {
				item.CoerceValue(IsCloseButtonVisibleProperty);
			}
		}
		protected virtual object CoerceSelectedTabIndex(int index) {
			if(!IsTabHost) return index;
			if(IsValid(index) && Items[index].IsVisible && moveItemLock == 0) return index;
			BaseLayoutItem selectedItem = SelectedItem != null && SelectedItem.IsVisible ? SelectedItem : null;
			return selectedItem != null && Items.Contains(selectedItem) ?
				Items.IndexOf(selectedItem) :
				VisiblePagesCount > 0 ? Items.IndexOf(VisiblePages[0]) : -1;
		}
		LockHelper _TabHeaderScrollIndexLocker;
		internal LockHelper TabHeaderScrollIndexLocker {
			get {
				if(_TabHeaderScrollIndexLocker == null) _TabHeaderScrollIndexLocker = new LockHelper(UpdateTabHeaderScrollIndex);
				return _TabHeaderScrollIndexLocker;
			}
		}
		void UpdateTabHeaderScrollIndex() {
			if(SelectedItem != null) {
				if(SelectedItem.IsPinnedTab) return;
				TabHeaderScrollIndex = Items.Where(x => !x.IsPinnedTab).ToList().IndexOf(SelectedItem);
			}
			else TabHeaderScrollIndex = -1;
		}
		protected virtual void OnSelectedItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			if(oldItem != null)
				oldItem.CoerceValue(IsSelectedItemProperty);
			if(item != null) {
				item.CoerceValue(IsSelectedItemProperty);
				item.CoerceValue(IsCloseButtonVisibleProperty);
			}
			RaiseSelectedItemChanged(new SelectedItemChangedEventArgs(item, oldItem));
			if(IsLoaded && !TabHeaderScrollIndexLocker) UpdateTabHeaderScrollIndex();
			else TabHeaderScrollIndexLocker.LockOnce();
			CoerceValue(IsCloseButtonVisibleProperty);
			if(IsTabHost)
				OnLayoutChanged();
		}
		void RaiseSelectedItemChanged(SelectedItemChangedEventArgs ea) {
			if(SelectedItemChanged != null) {
				SelectedItemChanged(this, ea);
			}
		}
		public bool ScrollNext() {
			if(!TabHeaderHasScroll) return false;
			TabHeaderScrollIndex++;
			return true;
		}
		public bool ScrollPrev() {
			if(!TabHeaderHasScroll) return false;
			TabHeaderScrollIndex--;
			return true;
		}
		protected virtual bool CoerceTabHeadersAutoFill(bool value) {
			return value;
		}
		protected virtual int CoerceTabHeaderScrollIndex(int index) {
			return Math.Max(0, Math.Min(TabHeaderMaxScrollIndex, index));
		}
		protected virtual int CoerceTabHeaderMaxScrollIndex(int index) {
			if(index == -1) return Items.Count;
			return index;
		}
		protected virtual TabHeaderLayoutType CoerceTabHeaderLayoutType(TabHeaderLayoutType value) {
			if(value != TabHeaderLayoutType.Default) return value;
			return TabHeaderLayoutType.Trim;
		}
		protected virtual void OnTabHeaderHasScrollChanged(bool hasScroll) {
			if(GroupBorderStyle == GroupBorderStyle.Tabbed) OnLayoutChanged();
		}
		protected virtual void OnTabHeaderLayoutTypeChanged(TabHeaderLayoutType type) {
			if(GroupBorderStyle == GroupBorderStyle.Tabbed) OnLayoutChanged();
			CoerceValue(TabHeadersAutoFillProperty);
		}
		protected virtual void OnTabHeadersAutoFillChanged(bool autoFill) {
			if(GroupBorderStyle == GroupBorderStyle.Tabbed) OnLayoutChanged();
		}
		protected virtual void OnGroupTemplateChanged() {
			ActualGroupTemplateSelector = CreateDefaultItemTemplateSelector();
		}
		protected virtual void OnAllowExpandChanged(bool allow) {
			CoerceValue(IsExpandedProperty);
		}
		protected virtual void OnExpandedChanged(bool expanded) {
			CoerceValue(IsExpandedProperty);
		}
		protected virtual bool CoerceIsExpanded(bool value) {
			return AllowExpand ? Expanded : value;
		}
		protected virtual void OnIsExpandedChanged(bool expanded) {
			CoerceValue(ItemHeightProperty);
			Manager.Do(x => x.InvalidateView(this.GetRoot()));
			if(!IsAutoHidden && GroupBorderStyle == GroupBorderStyle.GroupBox) {
				CoerceValue(MinHeightProperty);
				CoerceValue(ActualMinSizeProperty);
				if(Manager != null)
					Manager.Update();
			}
			RaiseVisualChanged();
		}
		protected override void OnTabCaptionWidthChanged(double width) {
			base.OnTabCaptionWidthChanged(width);
			AcceptItems((item) => item.CoerceValue(TabCaptionWidthProperty));
		}
		protected virtual void OnIsAnimatedChanged(bool oldValue, bool newValue) {
			if(newValue) IsAnimatedLockHelper.Lock();
			else IsAnimatedLockHelper.Unlock();
		}
		LockHelper isAnimatedLockHelper;
		internal LockHelper IsAnimatedLockHelper {
			get {
				if(isAnimatedLockHelper == null) isAnimatedLockHelper = new LockHelper();
				return isAnimatedLockHelper;
			}
		}
		ObservableCollection<BaseLayoutItem> PinnedLeftItems { get; set; }
		ObservableCollection<BaseLayoutItem> PinnedRightItems { get; set; }
		internal PlaceHolderHelper PlaceHolderHelper { get; private set; }
		internal virtual bool HasPlaceHolders { get { return PlaceHolderHelper != null && PlaceHolderHelper.HasPlaceHolders; } }
		protected internal LayoutPanel ParentPanel { get; set; }
		LockHelper selectedTabIndexLockHelper;
		LockHelper itemTemplateLockHelper;
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupIsAnimated")]
#endif
		public bool IsAnimated {
			get { return (bool)GetValue(IsAnimatedProperty); }
			internal set { SetValue(IsAnimatedPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderLayoutType"),
#endif
 XtraSerializableProperty, Category("TabHeader")]
		public TabHeaderLayoutType TabHeaderLayoutType {
			get { return (TabHeaderLayoutType)GetValue(TabHeaderLayoutTypeProperty); }
			set { SetValue(TabHeaderLayoutTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeadersAutoFill"),
#endif
 XtraSerializableProperty, Category("TabHeader")]
		public bool TabHeadersAutoFill {
			get { return (bool)GetValue(TabHeadersAutoFillProperty); }
			set { SetValue(TabHeadersAutoFillProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderHasScroll")]
#endif
		public bool TabHeaderHasScroll {
			get { return (bool)GetValue(TabHeaderHasScrollProperty); }
			internal set { SetValue(TabHeaderHasScrollPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderScrollIndex")]
#endif
		public int TabHeaderScrollIndex {
			get { return (int)GetValue(TabHeaderScrollIndexProperty); }
			internal set { SetValue(TabHeaderScrollIndexPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderMaxScrollIndex")]
#endif
		public int TabHeaderMaxScrollIndex {
			get { return (int)GetValue(TabHeaderMaxScrollIndexProperty); }
			internal set { SetValue(TabHeaderMaxScrollIndexPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderCanScrollPrev")]
#endif
		public bool TabHeaderCanScrollPrev {
			get { return (bool)GetValue(TabHeaderCanScrollPrevProperty); }
			internal set { SetValue(TabHeaderCanScrollPrevPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabHeaderCanScrollNext")]
#endif
		public bool TabHeaderCanScrollNext {
			get { return (bool)GetValue(TabHeaderCanScrollNextProperty); }
			internal set { SetValue(TabHeaderCanScrollNextPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupShowScrollPrevButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowScrollPrevButton {
			get { return (bool)GetValue(ShowScrollPrevButtonProperty); }
			set { SetValue(ShowScrollPrevButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupShowScrollNextButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowScrollNextButton {
			get { return (bool)GetValue(ShowScrollNextButtonProperty); }
			set { SetValue(ShowScrollNextButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupIsScrollPrevButtonVisible")]
#endif
		public bool IsScrollPrevButtonVisible {
			get { return (bool)GetValue(IsScrollPrevButtonVisibleProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupIsScrollNextButtonVisible")]
#endif
		public bool IsScrollNextButtonVisible {
			get { return (bool)GetValue(IsScrollNextButtonVisibleProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupSelectedTabIndex"),
#endif
 Category("TabHeader")]
		public int SelectedTabIndex {
			get { return (int)GetValue(SelectedTabIndexProperty); }
			set { SetValue(SelectedTabIndexProperty, value); }
		}
		int serializableSelectedTabPageIndexCore = -1;
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int SerializableSelectedTabPageIndex {
			get { return SelectedTabIndex; }
			set { serializableSelectedTabPageIndexCore = value; }
		}
		protected internal int GetSerializableSelectedTabPageIndex() {
			return serializableSelectedTabPageIndexCore;
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupSelectedItem")]
#endif
		public BaseLayoutItem SelectedItem {
			get { return (BaseLayoutItem)GetValue(SelectedItemProperty); }
			internal set { SetValue(SelectedItemPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupSelectedItemChanged")]
#endif
		public event SelectedItemChangedEventHandler SelectedItemChanged;
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupGroupTemplate"),
#endif
 Category("Content")]
		public DataTemplate GroupTemplate {
			get { return (DataTemplate)GetValue(GroupTemplateProperty); }
			set { SetValue(GroupTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupGroupTemplateSelector"),
#endif
 Category("Content")]
		public DataTemplateSelector GroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupTemplateSelectorProperty); }
			set { SetValue(GroupTemplateSelectorProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplateSelector ActualGroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupTemplateSelectorProperty); }
			private set { SetValue(ActualGroupTemplateSelectorPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupIsExpanded")]
#endif
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			internal set { SetValue(IsExpandedPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupAllowExpand"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowExpand {
			get { return (bool)GetValue(AllowExpandProperty); }
			set { SetValue(AllowExpandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupExpanded"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public bool Expanded {
			get { return (bool)GetValue(ExpandedProperty); }
			set { SetValue(ExpandedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupOrientation"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double ActualLayoutItemInterval {
			get { return (double)GetValue(ActualLayoutItemIntervalProperty); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double ActualLayoutGroupInterval {
			get { return (double)GetValue(ActualLayoutGroupIntervalProperty); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double ActualDockItemInterval {
			get { return (double)GetValue(ActualDockItemIntervalProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupLayoutItemInterval"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public double LayoutItemInterval {
			get { return (double)GetValue(LayoutItemIntervalProperty); }
			set { SetValue(LayoutItemIntervalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupLayoutGroupInterval"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public double LayoutGroupInterval {
			get { return (double)GetValue(LayoutGroupIntervalProperty); }
			set { SetValue(LayoutGroupIntervalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupDockItemInterval"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public double DockItemInterval {
			get { return (double)GetValue(DockItemIntervalProperty); }
			set { SetValue(DockItemIntervalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupAllowSplitters"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public bool? AllowSplitters {
			get { return (bool?)GetValue(AllowSplittersProperty); }
			set { SetValue(AllowSplittersProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupIsSplittersEnabled")]
#endif
		public bool IsSplittersEnabled {
			get { return (bool)GetValue(IsSplittersEnabledProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupHasSingleItem")]
#endif
		public bool HasSingleItem {
			get { return (bool)GetValue(HasSingleItemProperty); }
			private set { SetValue(HasSingleItemPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupGroupBorderStyle"),
#endif
		XtraSerializableProperty, Category("Content")]
		public virtual GroupBorderStyle GroupBorderStyle {
			get { return (GroupBorderStyle)GetValue(GroupBorderStyleProperty); }
			set { SetValue(GroupBorderStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupDestroyOnClosingChildren"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool DestroyOnClosingChildren {
			get { return (bool)GetValue(DestroyOnClosingChildrenProperty); }
			set { SetValue(DestroyOnClosingChildrenProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupHasAccent"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public bool? HasAccent {
			get { return (bool?)GetValue(HasAccentProperty); }
			set { SetValue(HasAccentProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLayoutRoot {
			get { return (bool)GetValue(IsLayoutRootProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupHasVisibleItems")]
#endif
		public bool HasVisibleItems {
			get { return (bool)GetValue(HasVisibleItemsProperty); }
			private set { SetValue(HasVisibleItemsPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal bool HasNotCollapsedItems {
			get { return (bool)GetValue(HasNotCollapsedItemsProperty); }
			private set { SetValue(HasNotCollapsedItemsPropertyKey, value); }
		}
		internal bool AcceptDock {
			get { return !HasNotCollapsedItems || !Items.HasVisibleStarItems(); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupCaptionOrientation"),
#endif
 Category("Caption")]
		public Orientation CaptionOrientation {
			get { return (Orientation)GetValue(CaptionOrientationProperty); }
			set { SetValue(CaptionOrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupItemsAppearance"),
#endif
 Category("Caption")]
		public Appearance ItemsAppearance {
			get { return (Appearance)GetValue(ItemsAppearanceProperty); }
			set { SetValue(ItemsAppearanceProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupItemsSource")]
#endif
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabItemContainerStyle")]
#endif
		public Style TabItemContainerStyle {
			get { return (Style)GetValue(TabItemContainerStyleProperty); }
			set { SetValue(TabItemContainerStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupTabItemContainerStyleSelector")]
#endif
		public StyleSelector TabItemContainerStyleSelector {
			get { return (StyleSelector)GetValue(TabItemContainerStyleSelectorProperty); }
			set { SetValue(TabItemContainerStyleSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupItemCaptionTemplate")]
#endif
		public DataTemplate ItemCaptionTemplate {
			get { return (DataTemplate)GetValue(ItemCaptionTemplateProperty); }
			set { SetValue(ItemCaptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupItemContentTemplate")]
#endif
		public DataTemplate ItemContentTemplate {
			get { return (DataTemplate)GetValue(ItemContentTemplateProperty); }
			set { SetValue(ItemContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupItemCaptionTemplateSelector")]
#endif
		public DataTemplateSelector ItemCaptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemCaptionTemplateSelectorProperty); }
			set { SetValue(ItemCaptionTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupItemContentTemplateSelector")]
#endif
		public DataTemplateSelector ItemContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty); }
			set { SetValue(ItemContentTemplateSelectorProperty, value); }
		}
		Appearance actualItemsAppearance;
		internal Appearance ActualItemsAppearance {
			get {
				if(actualItemsAppearance == null) {
					actualItemsAppearance = new Appearance();
					actualItemsAppearance.Changed += OnActualItemsAppearanceChanged;
				}
				return actualItemsAppearance;
			}
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupDestroyContentOnTabSwitching"),
#endif
 Category("Behavior"), XtraSerializableProperty]
		[Obsolete("Use the TabContentCacheMode property instead.")]
		public bool DestroyContentOnTabSwitching {
			get { return (bool)GetValue(DestroyContentOnTabSwitchingProperty); }
			set { SetValue(DestroyContentOnTabSwitchingProperty, value); }
		}
		[XtraSerializableProperty, Category("Behavior")]
		public TabContentCacheMode TabContentCacheMode {
			get { return (TabContentCacheMode)GetValue(TabContentCacheModeProperty); }
			set { SetValue(TabContentCacheModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupControlItemsHost"),
#endif
 Category("Behavior")]
		public bool? ControlItemsHost {
			get { return (bool?)GetValue(ControlItemsHostProperty); }
			set { SetValue(ControlItemsHostProperty, value); }
		}
		public bool LastChildFill {
			get { return (bool)GetValue(LastChildFillProperty); }
			set { SetValue(LastChildFillProperty, value); }
		}
		protected internal override void OnAppearanceObjectPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnAppearanceObjectPropertyChanged(e);
			InvalidateActualItemsAppearance();
		}
		protected override Thickness CoerceActualMargin(Thickness value) {
			if(MathHelper.AreEqual(Margin, new Thickness(double.NaN))) {
				if(IsControlItemsRoot() && GroupBorderStyle == GroupBorderStyle.NoBorder)
					return DockLayoutManagerParameters.LayoutRootMargin;
				if(IsDockingItemsRoot() && GroupBorderStyle == GroupBorderStyle.NoBorder) {
					if(IsMDIItemsRoot()) return new Thickness(0);
					return DockLayoutManagerParameters.DockingRootMargin;
				}
				return new Thickness(0);
			}
			return Margin;
		}
		protected bool IsControlItemsRoot() {
			return Parent == null && Items.Count > 0
				&& Items.ContainsOnlyControlItemsOrItsHosts();
		}
		protected bool IsDockingItemsRoot() {
			return Parent == null && Items.Count > 0
				&& !Items.ContainsOnlyControlItemsOrItsHosts();
		}
		protected bool IsMDIItemsRoot() {
			return Parent == null && Items.Count == 1
				&& (Items[0] is DocumentGroup) && !((DocumentGroup) Items[0]).IsTabbed;
		}
		protected virtual bool? CoerceHasAccent(bool? value) {
			if(value.HasValue) return value;
			return GroupBorderStyle != GroupBorderStyle.NoBorder;
		}
		protected virtual bool? CoerceAllowSplitters(bool? value) {
			LayoutGroup root = this.GetRoot();
			DockLayoutManager manager = this.FindDockLayoutManager();
			if(manager != null && manager.IsCustomization) return true;
			if(value.HasValue) return value;
			if(root != null && !root.Items.ContainsLayoutControlItemOrGroup()) return true;
			return false;
		}
		protected virtual bool CoerceAllowExpand(bool value) {
			return (GroupBorderStyle == GroupBorderStyle.GroupBox) && value;
		}
		protected override void OnIsTabPageChanged() {
			base.OnIsTabPageChanged();
			CoerceValue(GroupBorderStyleProperty);
		}
		protected virtual GroupBorderStyle CoerceGroupBorderStyle(GroupBorderStyle value) {
			if(IsInitializedAsDockingElement || IsTabPage)
				return GroupBorderStyle.NoBorder;
			return value;
		}
		protected virtual void OnGroupBorderStyleChanged(GroupBorderStyle style) {
			CoerceValue(HasAccentProperty);
			CoerceValue(AllowExpandProperty);
			CoerceValue(DestroyOnClosingChildrenProperty);
			CoerceValue(SelectedItemProperty);
			foreach(BaseLayoutItem item in Items) {
				item.CoerceValue(IsTabPageProperty);
			}
			AcceptItems((item) => {
				if(item is LayoutGroup)
					item.CoerceValue(GroupBorderStyleProperty);
			});
			OnLayoutChanged();
		}
		protected override void OnIsControlItemsHostChanged(bool value) {
			OnLayoutChanged();
			if(value && !Items.ContainsOnlyLayoutControlItemOrGroup())
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.InconsistentLayout));
		}
		protected override bool CoerceIsControlItemsHost(bool value) {
			if(ControlItemsHost.HasValue) return ControlItemsHost.Value;
			if(ParentPanel != null) return true;
			bool parentValue = Parent != null && Parent.IsControlItemsHost;
			return parentValue || value || Items.ContainsLayoutControlItem();
		}
		protected virtual void OnControlItemsHostChanged(bool? value) {
			CoerceValue(IsControlItemsHostProperty);
		}
		protected override Size CalcMinSizeValue(Size value) {
			if(!IsExpanded)
				return value;
			Size[] minSizes, maxSizes;
			Items.CollectConstraints(out minSizes, out maxSizes);
			bool fHorz = (Orientation == Orientation.Horizontal);
			Size groupMinSize = IgnoreOrientation ? CalcGroupMinSize(minSizes) : CalcGroupMinSize(minSizes, fHorz);
			return new Size(Math.Max(groupMinSize.Width, value.Width), Math.Max(groupMinSize.Height, value.Height));
		}
		protected override Size CalcMaxSizeValue(Size value) {
			Size[] minSizes, maxSizes;
			Items.CollectConstraints(out minSizes, out maxSizes);
			bool fHorz = (Orientation == Orientation.Horizontal);
			Size groupMinSize = IgnoreOrientation ? CalcGroupMinSize(minSizes) : CalcGroupMinSize(minSizes, fHorz);
			Size groupMaxSize = MathHelper.MeasureMaxSize(maxSizes, fHorz);
			return MathHelper.MeasureSize(groupMinSize, groupMaxSize, value);
		}
		Size CalcGroupMinSize(Size[] minSizes) {
			return MathHelper.MeasureMinSize(minSizes);
		}
		protected virtual Size CalcGroupMinSize(Size[] minSizes, bool fHorz) {
			Size groupMinSize = MathHelper.MeasureMinSize(minSizes, fHorz);
			groupMinSize = CalcMinSizeWitnIntervals(fHorz, groupMinSize);
			groupMinSize = CalcMinSizeWitnMargins(groupMinSize, ActualMargin);
			return groupMinSize;
		}
		Size CalcMinSizeWitnIntervals(bool fHorz, Size minSize) {
			double intervals = Items.GetIntervals();
			return new Size(fHorz ? minSize.Width + intervals : minSize.Width, fHorz ? minSize.Height : minSize.Height + intervals);
		}
		Size CalcMinSizeWitnMargins(Size minSize, Thickness margin) {
			return new Size(margin.Left + minSize.Width + margin.Right, margin.Top + minSize.Height + margin.Bottom);
		}
		protected override void OnActualMarginChanged(Thickness value) {
			base.OnActualMarginChanged(value);
			CoerceValue(ActualMinSizeProperty);
		}
		public LayoutGroup() {
			CoerceValue(IsCaptionVisibleProperty);
			CoerceValue(DestroyOnClosingChildrenProperty);
			CoerceValue(ActualLayoutItemIntervalProperty);
			CoerceValue(ActualLayoutGroupIntervalProperty);
			CoerceValue(ActualDockItemIntervalProperty);
			CoerceValue(AllowExpandProperty);
			if(!isInDesignTime)
				CoerceValue(TabHeaderLayoutTypeProperty);
		}
		PlaceHolderHelper CreatePlaceHolderHelper() {
			return new PlaceHolderHelper(this);
		}
		protected override void OnCreate() {
			base.OnCreate();
			Items = CreateItems();
			PinnedLeftItems = new ObservableCollection<BaseLayoutItem>();
			PinnedRightItems = new ObservableCollection<BaseLayoutItem>();
			PlaceHolderHelper = CreatePlaceHolderHelper();
			Items.CollectionChanged += OnItemsCollectionChanged;
			if(CanCreateItemsInternal())
				ItemsInternal = CreateItemsInternal();
			ActualGroupTemplateSelector = new DefaultItemTemplateSelectorWrapper(GroupTemplateSelector, GroupTemplate);
			EnsureItemsAppearance();
			VisiblePages = new ObservableCollection<BaseLayoutItem>();
			selectedTabIndexLockHelper = new LockHelper(() => { CoerceValue(SelectedTabIndexProperty); });
			itemTemplateLockHelper = new LockHelper(ResetItemsSource);
		}
		void EnsureItemsAppearance() {
			CoerceValue(ItemsAppearanceProperty);
		}
		protected override GridLength CoerceHeight(GridLength value) {
			if(value.IsStar && !IsExpanded)
				return new GridLength(1, GridUnitType.Auto);
			return base.CoerceHeight(value);
		}
		protected virtual void OnHasSingleItemChanged(bool hasSingleItem) { }
		protected virtual void OnHasAccentChanged(bool? hasAccent) {
			OnLayoutChanged();
		}
		protected virtual void OnAllowSplittersChanged(bool? allow) {
			SetValue(IsSplittersEnabledPropertyKey, allow ?? true);
			AcceptItems((item) => {
				if(item is LayoutGroup)
					item.CoerceValue(AllowSplittersProperty);
			});
			if(ItemsInternal != null)
				foreach(object item in ItemsInternal) {
					if(item is Splitter)
						((Splitter)item).IsEnabled = IsSplittersEnabled;
				}
		}
		protected bool IsValid(int index) {
			return (index >= 0) && (index < Items.Count);
		}
		protected internal virtual bool IgnoreOrientation {
			get { return GroupBorderStyle == GroupBorderStyle.Tabbed; }
		}
		protected virtual bool CanCreateItemsInternal() { return true; }
		protected virtual BaseLayoutItemCollection CreateItems() {
			return new BaseLayoutItemCollection(this);
		}
		protected virtual ObservableCollection<object> CreateItemsInternal() {
			return new ObservableCollection<object>();
		}
		protected virtual void OnIsLayoutRootChanged(bool value) {
			if(!isInDesignTime)
				CoerceValue(AllowSplittersProperty);
		}
		protected virtual bool CoerceIsLayoutRoot(bool value) {
			return Parent == null && CalcIsLayoutRoot();
		}
		bool CalcIsLayoutRoot() {
			return IsControlItemsHost || Items.ContainsNestedControlItemHostItems();
		}
		protected virtual void OnActualLayoutItemIntervalChanged(double interval) {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void OnActualLayoutGroupIntervalChanged(double interval) {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void OnActualDockItemIntervalChanged(double interval) {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void OnLayoutItemIntervalChanged(double interval) {
			CoerceValue(ActualLayoutItemIntervalProperty);
		}
		protected virtual void OnLayoutGroupIntervalChanged(double interval) {
			CoerceValue(ActualLayoutGroupIntervalProperty);
		}
		protected virtual void OnDockItemIntervalChanged(double interval) {
			CoerceValue(ActualDockItemIntervalProperty);
		}
		protected virtual double CoerceActualLayoutItemInterval(double value) {
			if(!double.IsNaN(LayoutItemInterval)) return LayoutItemInterval;
			return Orientation == Orientation.Horizontal ?
				DockLayoutManagerParameters.LayoutItemIntervalHorz : DockLayoutManagerParameters.LayoutItemIntervalVert;
		}
		protected virtual double CoerceActualLayoutGroupInterval(double value) {
			if(!double.IsNaN(LayoutGroupInterval)) return LayoutGroupInterval;
			return Orientation == Orientation.Horizontal ?
				DockLayoutManagerParameters.LayoutGroupIntervalHorz : DockLayoutManagerParameters.LayoutGroupIntervalVert;
		}
		protected virtual double CoerceActualDockItemInterval(double value) {
			if(!double.IsNaN(DockItemInterval)) return DockItemInterval;
			return Orientation == Orientation.Horizontal ?
				DockLayoutManagerParameters.DockingItemIntervalHorz : DockLayoutManagerParameters.DockingItemIntervalVert;
		}
		protected virtual bool CoerceDestroyOnClosingChildren(bool value) {
			return (GroupBorderStyle == GroupBorderStyle.NoBorder) && value;
		}
		protected override string CoerceCaptionFormat(string captionFormat) {
			if(!string.IsNullOrEmpty(captionFormat)) return captionFormat;
			return DockLayoutManagerParameters.LayoutGroupCaptionFormat;
		}
		protected virtual void OnOrientationChanged(Orientation orientation) {
			CoerceValue(ActualLayoutItemIntervalProperty);
			CoerceValue(ActualLayoutGroupIntervalProperty);
			CoerceValue(ActualDockItemIntervalProperty);
			AcceptItems((item) => {
				if(item is SeparatorItem)
					item.CoerceValue(SeparatorItem.OrientationProperty);
				if(item is LayoutSplitter)
					item.CoerceValue(LayoutSplitter.OrientationProperty);
			});
			OnLayoutChanged();
		}
		protected override void OnCaptionWidthChanged(double value) {
			base.OnCaptionWidthChanged(value);
			NotifyItems();
		}
		protected override void OnCaptionAlignModeChanged(CaptionAlignMode oldValue, CaptionAlignMode value) {
			base.OnCaptionAlignModeChanged(oldValue, value);
			NotifyItems();
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(IsControlItemsHostProperty);
			CoerceValue(IsLayoutRootProperty);
			CoerceValue(ActualMarginProperty);
			CoerceValue(GroupBorderStyleProperty);
			InvalidateActualItemsAppearance();
		}
		protected internal override void OnParentItemsChanged() {
			base.OnParentItemsChanged();
			CoerceValue(IsControlItemsHostProperty);
		}
		protected override double OnCoerceMinHeight(double value) {
			return GroupBorderStyle == GroupBorderStyle.GroupBox && !IsExpanded ? 0d : base.OnCoerceMinHeight(value);
		}
		internal int moveItemLock;
		protected internal virtual bool GetIsDocumentHost() {
			return Manager != null && Manager.DockingStyle != DockingStyle.Default && Items.IsDocumentHost(true);
		}
		protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateHasSingleItemProperty();
			EnsureLogicalChildren(e, true);
			if(ItemsInternal != null) UpdateItemsInternal(e);
			CoerceValue(IsControlItemsHostProperty);
			this.GetRoot().NotifyItems();
			OnLayoutChanged();
			if(IsTabHost) {
				UpdateVisiblePages();
				if(e.Action == NotifyCollectionChangedAction.Move) {
					CoerceValue(SelectedTabIndexProperty);
					return;
				}
				if(e.Action == NotifyCollectionChangedAction.Add && (SelectedTabIndex >= e.NewStartingIndex || SelectedTabIndex < 0))
					if(!IsInitializing) {
						if(!SelectedTabIndexLocker)
							SelectedTabIndex++;
						else SelectedTabIndexLocker.AddUnlockAction(() => CoerceValue(SelectedTabIndexProperty));
					}
				if(e.Action == NotifyCollectionChangedAction.Remove)
					if(SelectedTabIndex >= e.OldStartingIndex && SelectedTabIndex > 0)
						SelectedTabIndex--;
					else
						CoerceValue(SelectedTabIndexProperty);
				if(e.Action == NotifyCollectionChangedAction.Reset)
					CoerceValue(SelectedTabIndexProperty);
				CoerceValue(SelectedItemProperty);
			}
			if(IsUpdating && e.NewItems != null) {
				foreach(BaseLayoutItem item in e.NewItems) {
					PrepareContainerForItem(item);
				}
			}
			EnsureLogicalChildren(e, false);
			RaiseWeakItemsChanged(EventArgs.Empty);
		}
		void EnsureLogicalChildren(NotifyCollectionChangedEventArgs e, bool onAdd) {
			bool isReset = e.Action == NotifyCollectionChangedAction.Reset;
			IEnumerable oldItems = isReset ? logicalChildren.ToList() : e.OldItems;
			IEnumerable newItems = isReset ? Items : e.NewItems;
			if(!onAdd)
				RemoveLogicalChildren(oldItems);
			if(onAdd)
				AddLogicalChildren(newItems);
		}
		protected virtual bool CanAddLogicalChild { get { return true; } }
		protected void AddLogicalChildren(IEnumerable children) {
			if(!CanAddLogicalChild || children == null) return;
			foreach(DependencyObject item in children) {
				var parent = LogicalTreeHelper.GetParent(item);
				if(parent != null) {
					if(parent is ILogicalOwner && parent != this) {
						((ILogicalOwner)parent).RemoveChild(item);
					}
					else continue;
				}
				AddLogicalChild(item);
			}
		}
		protected void RemoveLogicalChildren(IEnumerable children) {
			if(children == null) return;
			foreach(DependencyObject item in children) {
				RemoveLogicalChild(item);
			}
		}
		internal override void PrepareForModification(bool isDeserializing) {
			base.PrepareForModification(isDeserializing);
			if(IsTemplateApplied && !EnvironmentHelper.IsNet45OrNewer)
				RemoveLogicalChildren(Items);
		}
		protected override void UnlockLogicalTreeCore() {
			base.UnlockLogicalTreeCore();
			if(!EnvironmentHelper.IsNet45OrNewer)
				AddLogicalChildren(Items);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return new MergedEnumerator(
					new IEnumerator[] { 
						logicalChildren.GetEnumerator(),
						base.LogicalChildren,
					});
			}
		}
		List<object> logicalChildren = new List<object>();
		protected internal new void AddLogicalChild(object child) {
			if(!logicalChildren.Contains(child)) logicalChildren.Add(child);
			base.AddLogicalChild(child);
		}
		protected internal new void RemoveLogicalChild(object child) {
			logicalChildren.Remove(child);
			base.RemoveLogicalChild(child);
		}
		Bars.Native.WeakList<EventHandler> handlersWeakItemsChanged = new Bars.Native.WeakList<EventHandler>();
		internal event EventHandler WeakItemsChanged {
			add { handlersWeakItemsChanged.Add(value); }
			remove { handlersWeakItemsChanged.Remove(value); }
		}
		void RaiseWeakItemsChanged(EventArgs args) {
			foreach(EventHandler e in handlersWeakItemsChanged)
				e(this, args);
		}
		protected internal virtual void BeforeItemAdded(BaseLayoutItem item) { }
		protected internal virtual void AfterItemRemoved(BaseLayoutItem item) {
			PlaceHolderHelper.Remove(item);
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null)
				RemoveFromPinned(panel);
		}
		protected internal virtual void AfterItemAdded(int index, BaseLayoutItem item) {
			PlaceHolderHelper.InsertItem(index, item);
		}
		protected void UpdateHasSingleItemProperty() {
			HasSingleItem = (Items.Count == 1 && (Items[0].ItemType == LayoutItemType.TabPanelGroup || Items[0] is LayoutPanel));
		}
		protected void NotifyItems() {
			foreach(BaseLayoutItem item in Items) {
				item.OnParentItemsChanged();
				if(item is LayoutGroup)
					((LayoutGroup)item).NotifyItems();
			}
		}
		protected void UpdateItemsInternal(NotifyCollectionChangedEventArgs e) {
			if(e != null && e.Action == NotifyCollectionChangedAction.Reset) {
				ItemsInternal.Clear();
			}
			if(e != null && e.OldItems != null) {
				foreach(BaseLayoutItem item in e.OldItems) {
					if(!Items.Contains(item)) RemoveItemFromItemsInternal(item);
				}
			}
			for(int i = 0; i < Items.Count; i++) {
				AddItemInItemsInternal(Items[i], i);
			}
		}
		protected virtual Splitter CreateSplitterItem(LayoutGroup group) {
			return new Splitter(group);
		}
		protected void AddItemInItemsInternal(BaseLayoutItem item, int index) {
			if(!ItemsInternal.Contains(item)) {
				int insertIndex = GetIndexInItemsInternal(index);
				if(insertIndex >= ItemsInternal.Count) ItemsInternal.Add(item);
				else ItemsInternal.Insert(insertIndex, item);
				if(insertIndex - 1 >= 0 && !(ItemsInternal[insertIndex - 1] is Splitter)) {
					Splitter splitter = CreateSplitterItem(this);
					if(!IsSplittersEnabled) splitter.IsEnabled = false;
					ItemsInternal.Insert(insertIndex, splitter);
					return;
				}
				if(insertIndex + 1 <= ItemsInternal.Count - 1 && !(ItemsInternal[insertIndex + 1] is Splitter)) {
					Splitter sizerItem = CreateSplitterItem(this);
					if(!IsSplittersEnabled) sizerItem.IsEnabled = false;
					ItemsInternal.Insert(insertIndex + 1, sizerItem);
				}
			}
		}
		protected void RemoveItemFromItemsInternal(BaseLayoutItem item) {
			int index = ItemsInternal.IndexOf(item);
			Splitter lSplitter = GetSplitter(index - 1);
			Splitter rSplitter = GetSplitter(index + 1);
			if(lSplitter != null || rSplitter != null) {
				if(index == 0) {
					ItemsInternal.Remove(rSplitter);
				}
				else {
					ItemsInternal.Remove(lSplitter);
				}
			}
			ItemsInternal.Remove(item);
		}
		Splitter GetSplitter(int index) {
			if(index >= 0 && index < ItemsInternal.Count)
				return ItemsInternal[index] as Splitter;
			return null;
		}
		protected int GetIndexInItemsInternal(int index) {
			if(index == 0) return 0;
			if(index == 1 && ItemsInternal.Count == 1) return 1;
			return index * 2 - 1;
		}
		int lockLayoutChanging;
		protected void OnLayoutChanged() {
			LogicalTreeLockHelper.DoWhenUnlocked(OnLayoutChangedAsync);
		}
		protected void OnLayoutChangedAsync() {
			if(IsInitializing || lockLayoutChanging > 0) return;
			lockLayoutChanging++;
			OnLayoutChangedCore();
			if(Parent != null) Parent.OnLayoutChanged();
			else RaiseLayoutChangedCore();
			--lockLayoutChanging;
		}
		protected virtual void OnLayoutChangedCore() {
			CoerceValue(IsLayoutRootProperty);
			CoerceValue(GroupBorderStyleProperty);
			CoerceValue(ActualMarginProperty);
			CoerceValue(ActualMinSizeProperty);
			CoerceValue(ActualMaxSizeProperty);
			CoerceValue(HasNotCollapsedItemsProperty);
			CoerceValue(HasVisibleItemsProperty);
			UpdateButtons();
		}
		protected internal override void UpdateButtons() {
			CoerceValue(IsCloseButtonVisibleProperty);
			CoerceValue(IsScrollPrevButtonVisibleProperty);
			CoerceValue(IsScrollNextButtonVisibleProperty);
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			IsInitializedAsDockingElement = !Items.ContainsOnlyControlItemsOrItsHosts();
			OnLayoutChanged();
			CoerceValue(SelectedTabIndexProperty);
		}
		protected internal bool IsInitializedAsDockingElement { get; private set; }
		public event EventHandler LayoutChanged;
		protected void RaiseLayoutChangedCore() {
			if(LayoutChanged != null)
				LayoutChanged(this, EventArgs.Empty);
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.Group;
		}
		public void Add(BaseLayoutItem item) {
			Items.Add(item);
		}
		public void Add(params BaseLayoutItem[] items) {
			AddRange(items);
		}
		public bool Remove(BaseLayoutItem item) {
			return Items.Remove(item);
		}
		public void Insert(int index, BaseLayoutItem item) {
			Items.Insert(index, item);
		}
		public void AddRange(BaseLayoutItem[] items) {
			Array.ForEach(items, Add);
		}
		public void Clear() {
			BaseLayoutItem[] items = Items.ToArray();
			for(int i = 0; i < items.Length; i++) Remove(items[i]);
		}
		public BaseLayoutItem this[string name] {
			get { return Items[name]; }
		}
		public BaseLayoutItem this[int index] {
			get { return Items[index]; }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("LayoutGroupItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseLayoutItemCollection Items { get; private set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ObservableCollection<object> ItemsInternal { get; private set; }
		protected internal bool IsUngroupped { get; set; }
		internal bool IsRootGroup { get; set; }
		protected internal virtual bool IsTabHost { get { return GroupBorderStyle == GroupBorderStyle.Tabbed; } }
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutGroupVisiblePagesCount")]
#endif
		public int VisiblePagesCount {
			get { return (int)GetValue(VisiblePagesCountProperty); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ObservableCollection<BaseLayoutItem> VisiblePages { get; private set; }
		protected internal virtual bool HasItems { get { return Items.Count > 0; } }
		public int GetChildrenCount() {
			int result = 0;
			AcceptItems((item) => result++);
			return result;
		}
		public override void Accept(IVisitor<BaseLayoutItem> visitor) {
			base.Accept(visitor);
			AcceptItems(visitor.Visit);
		}
		public override void Accept(VisitDelegate<BaseLayoutItem> visit) {
			base.Accept(visit);
			AcceptItems(visit);
		}
		void AcceptItems(VisitDelegate<BaseLayoutItem> visit) {
			foreach(BaseLayoutItem item in Items) item.Accept(visit);
		}
		protected override BaseLayoutItem[] GetNodesCore() {
			return Items.ToArray();
		}
		protected internal override void SetHidden(bool value, LayoutGroup customizationRoot) {
			base.SetHidden(value, customizationRoot);
			foreach(BaseLayoutItem item in Items) {
				item.SetHidden(value, item.Parent);
			}
			CoerceValue(IsLayoutRootProperty);
		}
		protected internal virtual int GetVisibleItemsCount() {
			int count = 0;
			foreach(BaseLayoutItem item in Items) {
				if(item.IsVisible) count++;
			}
			return count;
		}
		protected internal virtual BaseLayoutItem[] GetVisibleItems() {
			List<BaseLayoutItem> visibleItems = new List<BaseLayoutItem>();
			foreach(BaseLayoutItem item in Items) {
				if(item.IsVisible) visibleItems.Add(item);
			}
			return visibleItems.ToArray();
		}
		protected virtual int CoerceVisiblePagesCount(int value) {
			return VisiblePages.Count;
		}
		protected void UpdateVisiblePages() {
			VisiblePages.Clear();
			foreach(BaseLayoutItem item in Items) {
				if(item.IsVisible)
					VisiblePages.Add(item);
			}
			CoerceValue(VisiblePagesCountProperty);
		}
		protected internal virtual void OnItemVisibilityChanged(BaseLayoutItem item, Visibility visibility) {
		}
		protected internal virtual void OnItemIsVisibleChanged(BaseLayoutItem item) {
			OnLayoutChanged();
			this.GetRoot().NotifyItems();
			DockLayoutManagerExtension.Update(this, false);
			if(IsTabHost) {
				int index = VisiblePages.IndexOf(item);
				UpdateVisiblePages();
				if(!selectedTabIndexLockHelper.IsLocked) {
					if(!item.IsVisible) {
						if(SelectedItem == item) {
							if(index > 0) index--;
							BaseLayoutItem next = VisiblePagesCount > 0 ? VisiblePages[index] : null;
							SelectedTabIndex = Items.IndexOf(next);
						}
					}
					else {
						if(VisiblePagesCount == 1) SelectedTabIndex = Items.IndexOf(VisiblePages[0]);
					}
					CoerceValue(SelectedItemProperty);
				}
			}
		}
		protected override void OnIsVisibleChanged(bool isVisible) {
			base.OnIsVisibleChanged(isVisible);
			using(selectedTabIndexLockHelper.Lock()) {
				foreach(BaseLayoutItem item in Items) {
					item.CoerceValue(IsVisibleProperty);
				}
			}
		}
		protected virtual bool CoerceHasNotCollapsedItems(bool value) {
			return VisibilityHelper.ContainsNotCollapsedItems(this);
		}
		protected virtual void OnHasNotCollapsedItemsChanged(bool hasVisibleItems) {
			DockLayoutManagerExtension.Update(this, false);
		}
		protected override bool CoerceIsCloseButtonVisible(bool visible) {
			return false;
		}
		protected virtual bool HasTabHeader() {
			return GroupBorderStyle == GroupBorderStyle.Tabbed;
		}
		protected bool HasScrollableHeader() {
			return TabHeaderHasScroll &&
				(TabHeaderLayoutType == TabHeaderLayoutType.Default ||
				TabHeaderLayoutType == TabHeaderLayoutType.Scroll);
		}
		protected override bool CoerceIsScrollPrevButtonVisible(bool visible) {
			return HasTabHeader() && HasScrollableHeader() && ShowScrollPrevButton;
		}
		protected override bool CoerceIsScrollNextButtonVisible(bool visible) {
			return HasTabHeader() && HasScrollableHeader() && ShowScrollNextButton;
		}
		protected virtual bool CoerceHasVisibleItems(bool hasVisibleItems) {
			return VisibilityHelper.HasVisibleItems(this);
		}
		int lockItemsAppearanceUpdateCounter;
		int itemsAppearanceUpdatesCount;
		bool IsItemsAppearanceUpdateLocked {
			get { return lockItemsAppearanceUpdateCounter > 0; }
		}
		void LockItemsAppearanceUpdate() {
			if(!IsItemsAppearanceUpdateLocked)
				itemsAppearanceUpdatesCount = 0;
			lockItemsAppearanceUpdateCounter++;
		}
		void UnlockItemsAppearanceUpdate() {
			if(--lockItemsAppearanceUpdateCounter == 0) {
				if(itemsAppearanceUpdatesCount > 0) OnActualItemsAppearanceChanged();
			}
		}
		protected virtual void OnItemsAppearanceChanged(Appearance newValue) {
			InvalidateActualItemsAppearance();
			newValue.Owner = this;
		}
		internal void InvalidateActualItemsAppearance() {
			LockItemsAppearanceUpdate();
			AppearanceHelper.UpdateAppearance(ActualItemsAppearance, Parent != null ? Parent.ItemsAppearance : null, ItemsAppearance);
			UnlockItemsAppearanceUpdate();
		}
		void OnActualItemsAppearanceChanged(object sender, EventArgs e) {
			itemsAppearanceUpdatesCount++;
			if(IsItemsAppearanceUpdateLocked) return;
			try {
				OnActualItemsAppearanceChanged();
			}
			finally {
				itemsAppearanceUpdatesCount = 0;
			}
		}
		Appearance _DefaultItemsAppearance;
		internal Appearance DefaultItemsAppearance {
			get {
				if(_DefaultItemsAppearance == null) _DefaultItemsAppearance = new Appearance();
				return _DefaultItemsAppearance;
			}
		}
		protected virtual object CoerceItemsAppearance(Appearance value) {
			return value ?? DefaultItemsAppearance;
		}
		protected virtual void OnActualItemsAppearanceChanged() {
			AcceptItems((item) => {
				item.CoerceValue(ActualAppearanceProperty);
				if(item is LayoutGroup) ((LayoutGroup)item).InvalidateActualItemsAppearance();
			});
		}
		protected override void OnIsCaptionVisibleChanged(bool isCaptionVisible) {
			base.OnIsCaptionVisibleChanged(isCaptionVisible);
			RaiseVisualChanged();
		}
		protected override void OnHasCaptionChanged(bool hasCaption) {
			base.OnHasCaptionChanged(hasCaption);
			RaiseVisualChanged();
		}
		protected override void OnHasCaptionTemplateChanged(bool hasCaptionTemplate) {
			base.OnHasCaptionTemplateChanged(hasCaptionTemplate);
			RaiseVisualChanged();
		}
		public override void BeginInit() {
			base.BeginInit();
			BeginUpdate();
		}
		public override void EndInit() {
			base.EndInit();
			EndUpdate();
		}
		int updateCount;
		bool IsUpdating { get { return updateCount > 0; } }
		internal void BeginUpdate() {
			updateCount++;
		}
		internal void EndUpdate() {
			updateCount--;
		}
		LockHelper SelectedTabIndexLocker = new LockHelper();
		protected virtual void OnItemsSourceChanged(IEnumerable value, IEnumerable oldValue) {
			BeginUpdate();
			using(SelectedTabIndexLocker.Lock()) {
				if((value == null)) {
					Items.ClearItemsSource();
				}
				else {
					Items.SetItemsSource(value);
				}
			}
			EndUpdate();
		}
		protected virtual void OnItemStyleChanged(Style value, Style oldValue) {
			if(IsInitializing) {
				foreach(BaseLayoutItem item in Items) {
					PrepareContainerForItem(item);
				}
			}
			OnItemTemplatePropertyChanged();
		}
		protected virtual void OnItemTemplatePropertyChanged() {
			if(!itemTemplateLockHelper.IsLocked) {
				itemTemplateLockHelper.Lock();
				Core.Native.BackgroundHelper.DoWithDispatcher(Dispatcher, () => { itemTemplateLockHelper.Unlock(); });
			}
		}
		void ResetItemsSource() {
			if(ItemsSource == null) return;
			BeginUpdate();
			Items.ResetItemsSource();
			EndUpdate();
		}
		protected virtual bool IsItemItsOwnContainer(object item) {
			return item is ContentItem;
		}
		protected virtual void PrepareContainerForItem(BaseLayoutItem item, object content) {
			PrepareContainerForItem(item);
			item.PrepareContainer(content);
		}
		protected virtual void PrepareContainerForItem(BaseLayoutItem item) {
			if(item == null) return;
			if(IsUpdating) {
				PrepareContainerForItemCore(item);
			}
		}
		internal void PrepareContainerForItemCore(BaseLayoutItem item) {
			if(item.CaptionTemplate == null && ItemCaptionTemplate != null)
				item.CaptionTemplate = ItemCaptionTemplate;
			if(item.CaptionTemplateSelector == null && ItemCaptionTemplateSelector != null)
				item.CaptionTemplateSelector = ItemCaptionTemplateSelector;
			ContentItem contentItem = item as ContentItem;
			if(contentItem != null) {
				if(contentItem.ContentTemplate == null && ItemContentTemplate != null)
					contentItem.ContentTemplate = ItemContentTemplate;
				if(contentItem.ContentTemplateSelector == null && ItemContentTemplateSelector != null)
					contentItem.ContentTemplateSelector = ItemContentTemplateSelector;
			}
			if(ItemStyle != null)
				item.Style = ItemStyle;
		}
		protected virtual BaseLayoutItem GetContainerForItemCore(object content, DataTemplate itemTemplate = null, DataTemplateSelector itemTemplateSelector = null) {
			BaseLayoutItem container;
			if(IsControlItemsHost)
				container = Manager != null ? Manager.CreateLayoutControlItem() : new LayoutControlItem();
			else
				container = Manager != null ? Manager.CreateLayoutPanel() : new LayoutPanel();
			return container;
		}
		BaseLayoutItem CreateItem(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			DataTemplate template;
			DataTemplateSelector templateSelector = itemTemplateSelector;
			if(templateSelector != null)
				template = templateSelector.SelectTemplate(item, this);
			else
				template = itemTemplate;
			if(template == null) return null;
			BaseLayoutItem loadedItem = null;
			DependencyObject loadedContent = template.LoadContent();
			if(loadedContent != null) {
				if(loadedContent is BaseLayoutItem) {
					loadedItem = (BaseLayoutItem)loadedContent;
				}
				else if(loadedContent is ContentControl) {
					loadedItem = (BaseLayoutItem)((ContentControl)loadedContent).Content;
					((ContentControl)loadedContent).Content = null;
				}
				else if(loadedContent is ContentPresenter) {
					loadedItem = (BaseLayoutItem)((ContentPresenter)loadedContent).Content;
					((ContentPresenter)loadedContent).Content = null;
				}
			}
			return loadedItem;
		}
		protected internal virtual BaseLayoutItem GetContainerForItem(object item, DataTemplate itemTemplate = null, DataTemplateSelector itemTemplateSelector = null) {
			BaseLayoutItem container = null;
			if(IsItemItsOwnContainer(item)) {
				container = item as BaseLayoutItem;
			}
			if(container == null && (itemTemplate != null || itemTemplateSelector != null)) {
				container = CreateItem(item, itemTemplate, itemTemplateSelector);
			}
			if(container == null)
				container = GetContainerForItemCore(item);
			PrepareContainerForItem(container, item);
			return container;
		}
		protected virtual void ClearContainerCore(BaseLayoutItem container) {
			ContentItem contentItem = container as ContentItem;
			if(contentItem != null) {
				DisposeHelper.DisposeVisualTree(contentItem.Content as DependencyObject);
				contentItem.ClearValue(ContentItem.ContentProperty);
			}
		}
		protected internal virtual void ClearContainerForItem(BaseLayoutItem container, object item) {
			if(container == null) return;
			if(container is LayoutPanel && Manager != null) {
				Manager.DockController.RemovePanel((LayoutPanel)container);
			}
			else
				Remove(container);
			ClearContainerCore(container);
		}
		protected virtual LayoutGroup GetContainerHost(ContentItem container) {
			return this;
		}
		protected virtual void AddContainer(ContentItem container) {
			var containerHost = GetContainerHost(container);
			containerHost.Add(container);
		}
		protected virtual void InsertContainer(int index, ContentItem container) {
			var containerHost = GetContainerHost(container);
			containerHost.Insert(index, container);
		}
		protected virtual DependencyObject GenerateContainerForItem(object item, DataTemplate itemTemplate = null, DataTemplateSelector itemTemplateSelector = null) {
			BeginUpdate();
			ContentItem container = GetContainerForItem(item, itemTemplate, itemTemplateSelector) as ContentItem;
			if(container != null) {
				AddContainer(container);
			}
			EndUpdate();
			return container;
		}
		DependencyObject LinkContainerToItem(DependencyObject oldContainer, object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			ContentItem container = null;
			BaseLayoutItem oldItem = oldContainer as BaseLayoutItem;
			if(oldItem != null && oldItem.Parent != null) {
				LayoutGroup actualHost = oldItem.Parent;
				BeginUpdate();
				actualHost.BeginUpdate();
				int index = actualHost.Items.IndexOf(oldItem);
				container = GetContainerForItem(item, itemTemplate, itemTemplateSelector) as ContentItem;
				if(container != null) {
					actualHost.InsertContainer(index, container);
					ClearContainerForItem(oldItem, item);
				}
				EndUpdate();
				actualHost.EndUpdate();
			}
			return container;
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new UIAutomation.LayoutGroupAutomationPeer(this);
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
		#region IGeneratorHost Members
		DependencyObject IGeneratorHost.GenerateContainerForItem(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			return GenerateContainerForItem(item, itemTemplate, itemTemplateSelector);
		}
		DependencyObject IGeneratorHost.LinkContainerToItem(DependencyObject container, object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			return LinkContainerToItem(container, item, itemTemplate, itemTemplateSelector);
		}
		void IGeneratorHost.ClearContainer(DependencyObject container, object item) {
			ClearContainerForItem(container as BaseLayoutItem, item);
		}
		#endregion
		class DefaultTemplateSelector : DefaultItemTemplateSelectorWrapper.DefaultItemTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				GroupPaneContentPresenter presenter = container as GroupPaneContentPresenter;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && presenter != null && presenter.Owner != null)
					switch(group.GroupBorderStyle) {
						case GroupBorderStyle.Group: return presenter.Owner.GroupTemplate;
						case GroupBorderStyle.GroupBox: return presenter.Owner.GroupBoxTemplate;
						case GroupBorderStyle.NoBorder: return presenter.Owner.NoBorderTemplate;
						case GroupBorderStyle.Tabbed: return presenter.Owner.TabbedTemplate;
					}
				return null;
			}
		}
	}
	public class BaseLayoutItemCollection : BaseItemsCollection<BaseLayoutItem> {
		protected readonly LayoutGroup Owner;
		public BaseLayoutItemCollection(LayoutGroup owner) {
			Owner = owner;
		}
		protected virtual void BeforeItemAdded(BaseLayoutItem item) {
			CheckItemRules(item);
			Owner.BeforeItemAdded(item);
		}
		protected virtual void OnItemAdded(BaseLayoutItem item) {
			item.Parent = Owner;
		}
		protected virtual void OnItemRemoved(BaseLayoutItem item) {
			item.Parent = null;
			Owner.AfterItemRemoved(item);
		}
		void ClearItemsCore() {
			BaseLayoutItem[] items = Items.ToArray();
			base.ClearItems();
			for(int i = 0; i < items.Length; i++)
				OnItemRemoved(items[i]);
		}
		protected override void ClearItems() {
			CheckIsUsingItemsSource();
			ClearItemsCore();
		}
		protected virtual void NotifyItemInserted(BaseLayoutItem item, int index) {
			OnItemAdded(item);
			if(Owner != null)
				Owner.AfterItemAdded(index, item);
		}
		protected virtual void NotifyItemRemoved(BaseLayoutItem item) {
			if(Owner != null)
				Owner.AfterItemRemoved(item);
		}
		void InsertItemCore(int index, BaseLayoutItem item) {
			DockLayoutManager manager = GetManager();
			using(new UpdateBatch(manager)) {
				using(item.ParentLockHelper.Lock()) {
					BeforeItemAdded(item);
					base.InsertItem(index, item);
					NotifyItemInserted(item, index);
				}
			}
		}
		protected override void InsertItem(int index, BaseLayoutItem item) {
			CheckIsUsingItemsSource();
			InsertItemCore(index, item);
		}
		DockLayoutManager GetManager() {
			return (Owner != null) ? Owner.GetRoot().Manager : null;
		}
		void RemoveItemCore(BaseLayoutItem item) {
			int index = IndexOf(item);
			if(index != -1)
				RemoveItemAt(index);
		}
		void RemoveItemAt(int index) {
			BaseLayoutItem item = ((index < 0) || (index >= Count)) ? null : this[index];
			if(item != null) item.ParentLockHelper.Lock();
			base.RemoveItem(index);
			if(item != null) {
				OnItemRemoved(item);
				NotifyItemRemoved(item);
				item.ParentLockHelper.Unlock();
			}
		}
		protected override void RemoveItem(int index) {
#if !SILVERLIGHT
			Owner.Do((x) => x.BeginLayoutChange());
#endif
			CheckIsUsingItemsSource();
			RemoveItemAt(index);
#if !SILVERLIGHT
			Owner.Do((x) => x.EndLayoutChange());
#endif
		}
		public BaseLayoutItem this[string name] {
			get { return FindItem(Items, name); }
		}
		public static BaseLayoutItem FindItem(IList<BaseLayoutItem> items, string name) {
			foreach(BaseLayoutItem item in items) {
				if(item.Name == name) return item;
			}
			return null;
		}
		internal double GetIntervals() {
			double intervals = 0.0;
			for(int i = 1; i < Items.Count; i++) {
				intervals += (double)Owner.GetValue(IntervalHelper.GetTargetProperty(Items[i - 1], Items[i]));
			}
			return intervals;
		}
		internal void CollectConstraints(out Size[] minSizes, out Size[] maxSizes) {
			minSizes = new Size[Items.Count];
			maxSizes = new Size[Items.Count];
			for(int i = 0; i < minSizes.Length; i++) {
				if(Items[i].IsVisible) {
					minSizes[i] = Items[i].ActualMinSize;
					maxSizes[i] = Items[i].ActualMaxSize;
				}
			}
		}
		internal bool HasVisibleStarItems() {
			bool isHorizontal = Owner.Orientation == Orientation.Horizontal;
			return this.Count(x => LayoutItemsHelper.IsActuallyVisibleInTree(x) && (isHorizontal ? x.ItemWidth.IsStar : x.ItemHeight.IsStar)) != 0;
		}
		internal bool ContainsNonEmptyDocumentGroups() {
			foreach(BaseLayoutItem item in Items) {
				if(item.Parent is DocumentGroup) return true;
				if(item is LayoutGroup && ((LayoutGroup)item).Items.ContainsNonEmptyDocumentGroups()) return true;
			}
			return false;
		}
		internal bool ContainsOnlyControlItemsOrItsHosts() {
			foreach(BaseLayoutItem item in Items) {
				if(LayoutItemsHelper.IsLayoutItem(item)) continue;
				if(item.ItemType == LayoutItemType.Panel) return false;
				if(item.ItemType == LayoutItemType.Document) return false;
				if(item.IsControlItemsHost) continue;
				if(item.ItemType == LayoutItemType.Group) {
					LayoutGroup group = item as LayoutGroup;
					if(group.Items.Count == 0) continue;
					if(group.Items.ContainsOnlyControlItemsOrItsHosts()) return true;
				}
				return false;
			}
			return true;
		}
		internal bool AllowDockToDocumentGroup() {
			foreach(BaseLayoutItem item in Items) {
				if(item is LayoutPanel && !((LayoutPanel)item).AllowDockToDocumentGroup) return false;
				if(item.ItemType == LayoutItemType.Group) {
					LayoutGroup group = item as LayoutGroup;
					if(group.Items.Count == 0) continue;
					if(!group.Items.AllowDockToDocumentGroup()) return false;
				}
			}
			return true;
		}
		internal bool ContainsLayoutControlItem() {
			foreach(BaseLayoutItem item in Items) {
				if(LayoutItemsHelper.IsLayoutItem(item)) return true;
			}
			return false;
		}
		internal bool ContainsNestedControlItemHostItems() {
			foreach(BaseLayoutItem item in Items) {
				if(item.IsControlItemsHost) return true;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.Items.Count > 0)
					if(group.Items.ContainsNestedControlItemHostItems()) return true;
			}
			return false;
		}
		internal bool ContainsOnlyLayoutControlItemOrGroup() {
			foreach(BaseLayoutItem item in Items) {
				if(LayoutItemsHelper.IsLayoutItem(item)) continue;
				if(item.ItemType == LayoutItemType.Group) continue;
				return false;
			}
			return true;
		}
		internal bool ContainsLayoutControlItemOrGroup() {
			foreach(BaseLayoutItem item in Items) {
				if(LayoutItemsHelper.IsLayoutItem(item)) return true;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.Items.Count > 0)
					if(group.Items.ContainsLayoutControlItemOrGroup()) return true;
			}
			return false;
		}
		internal bool IsDocumentHost() {
			bool hasDocumentGroup = false;
			foreach(BaseLayoutItem item in Items) {
				if(item is LayoutPanel) return false;
				if(item is DocumentGroup) hasDocumentGroup = true;
			}
			return hasDocumentGroup;
		}
		internal bool IsDocumentHost(bool ignoreLayoutPanels) {
			bool hasDocumentGroup = false;
			if(ignoreLayoutPanels) {
				foreach(BaseLayoutItem item in Items) {
					if(item is DocumentGroup) hasDocumentGroup = true;
				}
			}
			else {
				hasDocumentGroup = IsDocumentHost();
			}
			return hasDocumentGroup;
		}
		bool CanDockToDocumentGroup(BaseLayoutItem item) {
			return item is LayoutPanel && ((LayoutPanel)item).AllowDockToDocumentGroup;
		}
		protected virtual void CheckItemRules(BaseLayoutItem item) {
			if(item.ItemType == LayoutItemType.ControlItem && Owner.ItemType != LayoutItemType.Group)
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.ItemCanNotBeHosted));
			if(Owner.IsControlItemsHost) {
				if(item.ItemType == LayoutItemType.Panel || item.ItemType == LayoutItemType.Document)
					throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.InconsistentLayout));
				if(item.ItemType == LayoutItemType.DocumentPanelGroup || item.ItemType == LayoutItemType.TabPanelGroup)
					throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.InconsistentLayout));
			}
			if(item.ItemType != LayoutItemType.Panel && Owner.ItemType == LayoutItemType.TabPanelGroup)
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.WrongPanel));
			if(Owner.ItemType == LayoutItemType.DocumentPanelGroup && !CanDockToDocumentGroup(item))
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.WrongDocument));
			if(item.ItemType == LayoutItemType.FloatGroup)
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.FloatGroupsCollection));
			if(item.ItemType == LayoutItemType.AutoHideGroup)
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.AutoHideGroupsCollection));
		}
		internal void ResetItemsSource() {
			OnResetItemsSource();
		}
		#region ItemsSource
		Dictionary<object, object> collectionHash = new Dictionary<object, object>();
		protected override void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			using(new UpdateBatch(Owner.Manager)) {
				Owner.BeginUpdate();
				base.OnItemsSourceCollectionChanged(sender, e);
				Owner.EndUpdate();
			}
		}
		protected override void OnAddToItemsSource(IEnumerable newItems, int startingIndex = 0) {
			if(newItems == null) return;
			foreach(object item in newItems) {
				BaseLayoutItem container = Owner.GetContainerForItem(item);
				collectionHash.Add(item, container);
				InsertItemCore(startingIndex++, container);
			}
		}
		Locker removeFromItemsSourceLocker = new Locker();
		protected override void OnRemoveFromItemsSource(IEnumerable oldItems) {
			foreach(object item in oldItems) {
				if(collectionHash.ContainsKey(item)) {
					BaseLayoutItem layoutItem = collectionHash[item] as BaseLayoutItem;
					using(removeFromItemsSourceLocker.Lock()) {
						Owner.ClearContainerForItem(layoutItem, item);
					}
					RemoveItemCore(layoutItem);
					collectionHash.Remove(item);
				}
			}
		}
		protected override void InvalidateItemsSource() {
			ClearItemsCore();
			collectionHash.Clear();
		}
		protected override void OnResetItemsSource() {
			InvalidateItemsSource();
			OnAddToItemsSource(ItemsSource);
		}
		protected override void OnItemReplacedInItemsSource(IList oldItems, IList newItems, int newStartingIndex) {
			OnRemoveFromItemsSource(oldItems);
			OnAddToItemsSource(newItems, newStartingIndex);
		}
		protected override void OnItemMovedInItemsSource(int oldStartingIndex, int newStartingIndex) {
			Move(oldStartingIndex, newStartingIndex);
		}
		protected override void OnCurrentChanged(object sender) { }
		void CheckIsUsingItemsSource() {
			if(IsUsingItemsSource && !removeFromItemsSourceLocker) {
				throw new InvalidOperationException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.ItemsSourceInUse));
			}
		}
		#endregion
		protected override void MoveItem(int oldIndex, int newIndex) {
			if(Owner != null) Owner.moveItemLock++;
			base.MoveItem(oldIndex, newIndex);
			if(Owner != null) Owner.moveItemLock--;
		}
	}
	public class LayoutGroupCollection : ObservableCollection<LayoutGroup> { }
	static class LayoutGroupCollectionExtensions {
		public static void Purge(this LayoutGroupCollection collection) {
			var items = collection.ToArray();
			foreach(var item in items) {
				if(!item.HasPlaceHolders) {
					PlaceHolderHelper.ClearPlaceHolder(item);
					collection.Remove(item);
				}
			}
		}
	}
}
