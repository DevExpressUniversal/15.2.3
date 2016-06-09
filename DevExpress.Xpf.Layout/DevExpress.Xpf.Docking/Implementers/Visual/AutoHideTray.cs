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
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SWC = System.Windows.Controls;
using SWI = System.Windows.Input;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class AutoHideTray : psvItemsControl, IUIElement {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty DockTypeProperty;
		static readonly DependencyPropertyKey DockTypePropertyKey;
		public static readonly DependencyProperty IsExpandedProperty;
		readonly static DependencyPropertyKey IsExpandedPropertyKey;
		readonly static DependencyPropertyKey HotItemPropertyKey;
		public static readonly DependencyProperty HotItemProperty;
		public static readonly DependencyProperty IsTopProperty;
		public static readonly DependencyProperty IsLeftProperty;
		public static readonly DependencyProperty IsRightProperty;
		public static readonly DependencyProperty IsBottomProperty;
		readonly static DependencyPropertyKey IsTopPropertyKey;
		readonly static DependencyPropertyKey IsLeftPropertyKey;
		readonly static DependencyPropertyKey IsRightPropertyKey;
		readonly static DependencyPropertyKey IsBottomPropertyKey;
		readonly static RoutedEvent ExpandedEvent;
		readonly static RoutedEvent CollapsedEvent;
		readonly static RoutedEvent PanelClosedEvent;
		readonly static RoutedEvent HotItemChangedEvent;
		readonly static RoutedEvent PanelResizingEvent;
		readonly static RoutedEvent PanelMaximizedEvent;
		readonly static RoutedEvent PanelRestoredEvent;
		static AutoHideTray() {
			var dProp = new DependencyPropertyRegistrator<AutoHideTray>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.RegisterAttached("Orientation", ref OrientationProperty, Orientation.Vertical, OnOrientationChanged);
			dProp.RegisterReadonly("DockType", ref DockTypePropertyKey, ref DockTypeProperty, SWC.Dock.Left,
				(dObj, e) => ((AutoHideTray)dObj).OnDockTypeChanged((SWC.Dock)e.NewValue));
			dProp.RegisterReadonly("IsExpanded", ref IsExpandedPropertyKey, ref IsExpandedProperty, false,
				(dObj, e) => ((AutoHideTray)dObj).OnIsExpandedChanged((bool)e.NewValue));
			dProp.RegisterReadonly("HotItem", ref HotItemPropertyKey, ref HotItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((AutoHideTray)dObj).OnHotItemChanged((BaseLayoutItem)e.NewValue, (BaseLayoutItem)e.OldValue));
			dProp.RegisterDirectEvent<RoutedEventHandler>("Expanded", ref ExpandedEvent);
			dProp.RegisterDirectEvent<RoutedEventHandler>("Collapsed", ref CollapsedEvent);
			dProp.RegisterDirectEvent<RoutedEventHandler>("PanelClosed", ref PanelClosedEvent);
			dProp.RegisterDirectEvent<HotItemChangedEventHandler>("HotItemChanged", ref HotItemChangedEvent);
			dProp.RegisterDirectEvent<PanelResizingEventHandler>("PanelResizing", ref PanelResizingEvent);
			dProp.RegisterDirectEvent<RoutedEventHandler>("PanelMaximized", ref PanelMaximizedEvent);
			dProp.RegisterDirectEvent<RoutedEventHandler>("PanelRestored", ref PanelRestoredEvent);
			dProp.RegisterAttachedReadonlyInherited("IsTop", ref  IsTopPropertyKey, ref IsTopProperty, false);
			dProp.RegisterAttachedReadonlyInherited("IsLeft", ref  IsLeftPropertyKey, ref IsLeftProperty, true);
			dProp.RegisterAttachedReadonlyInherited("IsRight", ref  IsRightPropertyKey, ref IsRightProperty, false);
			dProp.RegisterAttachedReadonlyInherited("IsBottom", ref  IsBottomPropertyKey, ref IsBottomProperty, false);
		}
		public static Orientation GetOrientation(DependencyObject obj) {
			return (Orientation)obj.GetValue(OrientationProperty);
		}
		public static void SetOrientation(DependencyObject obj, Orientation value) {
			obj.SetValue(OrientationProperty, value);
		}
		public static bool GetIsLeft(DependencyObject obj) {
			return (bool)obj.GetValue(IsLeftProperty);
		}
		public static bool GetIsRight(DependencyObject obj) {
			return (bool)obj.GetValue(IsRightProperty);
		}
		public static bool GetIsTop(DependencyObject obj) {
			return (bool)obj.GetValue(IsTopProperty);
		}
		public static bool GetIsBottom(DependencyObject obj) {
			return (bool)obj.GetValue(IsBottomProperty);
		}
		static void SetIsTop(DependencyObject obj, bool value) {
			obj.SetValue(IsTopPropertyKey, value);
		}
		static void SetIsBottom(DependencyObject obj, bool value) {
			obj.SetValue(IsBottomPropertyKey, value);
		}
		static void SetIsLeft(DependencyObject obj, bool value) {
			obj.SetValue(IsLeftPropertyKey, value);
		}
		static void SetIsRight(DependencyObject obj, bool value) {
			obj.SetValue(IsRightPropertyKey, value);
		}
		static void InvalidateView(AutoHideTray tray) {
			IView view = tray.Container.GetView(tray);
			if(view != null) view.Invalidate();
		}
		static void OnOrientationChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			UIElement uiElement = dObj as UIElement;
			if(uiElement != null) {
				uiElement.InvalidateMeasure();
				uiElement.InvalidateArrange();
			}
		}
		#endregion static
		#region IUIElement
		IUIElement IUIElement.Scope {
			get { return DockLayoutManager.GetDockLayoutManager(this); }
		}
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		public AutoHideTray() {
			DockPane.SetHitTestType(this, HitTestType.PageHeaders);
		}
		protected override void OnDispose() {
			Ref.Dispose(ref hitCache);
			UnSubscribe();
			View = null;
			base.OnDispose();
		}
		protected override void OnLoaded() {
			Subscribe();
		}
		protected override void OnUnloaded() {
			UnSubscribe();
		}
		protected override void OnHasItemsChanged(bool hasItems) {
			EnsureVisibility(hasItems);
		}
		protected virtual void OnDockTypeChanged(SWC.Dock value) {
			SetIsLeft(this, value == SWC.Dock.Left);
			SetIsRight(this, value == SWC.Dock.Right);
			SetIsTop(this, value == SWC.Dock.Top);
			SetIsBottom(this, value == SWC.Dock.Bottom);
		}
		protected virtual void OnIsExpandedChanged(bool value) {
			if(IsDisposing) return;
			InvalidateView(this);
			if(value) RaiseExpanded();
			else RaiseCollapsed();
		}
		protected virtual void OnHotItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			InvalidateView(this);
			bool isExpanded = IsExpanded;
			if(isExpanded) IsExpanded = false;
			RaiseHotItemChanged(item, oldItem);
			if(isExpanded) IsExpanded = true;
		}
		protected AutoHideView View { get; private set; }
		public bool IsHorizontal {
			get { return GetOrientation(this) == Orientation.Horizontal; }
		}
		protected bool ExpandOnMouseDown {
			get { return (Container != null) && GetRealAutoHideMode() == AutoHideExpandMode.MouseDown; }
		}
		AutoHideExpandMode GetRealAutoHideMode() {
			return Container.IsInDesignTime ? AutoHideExpandMode.MouseDown : Container.AutoHideExpandMode;
		}
		IList<UIElement> subscriptions;
		bool EnsureUnSubscribed(UIElement element) {
			if(subscriptions == null)
				subscriptions = new List<UIElement>();
			return !subscriptions.Contains(element);
		}
		void SubscribeTrayMouseMove(UIElement element) {
			if(element == null) return;
			if(EnsureUnSubscribed(element)) {
				Mouse.AddPreviewMouseMoveHandler(element, new MouseEventHandler(TrayMouseMove));
				subscriptions.Add(element);
			}
		}
		void UnSubscribeTrayMouseMove(UIElement element) {
			if(element == null) return;
			if(subscriptions != null)
				subscriptions.Remove(element);
			Mouse.RemovePreviewMouseMoveHandler(element, new MouseEventHandler(TrayMouseMove));
		}
		protected void Subscribe() {
			SubscribeTrayMouseMove(this);
			SubscribeTrayMouseMove(Panel);
			SubscribeTrayMouseMove(Container);
			Mouse.AddMouseEnterHandler(this, new MouseEventHandler(TrayMouseEnter));
		}
		protected void UnSubscribe() {
			UnSubscribeTrayMouseMove(this);
			UnSubscribeTrayMouseMove(Panel);
			UnSubscribeTrayMouseMove(Container);
			Mouse.RemoveMouseEnterHandler(this, new MouseEventHandler(TrayMouseEnter));
		}
		protected StackPanel PartHeaderGroupsPanel { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			EnsureVisibility(HasItems);
			if(Panel != null)
				DockLayoutManager.Ensure(Panel);
			if(Container != null) {
				View = Container.GetView(this) as AutoHideView;
				if(View != null)
					View.Initialize((IUIElement)this);
			}
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			PartHeaderGroupsPanel = itemsPanel as StackPanel;
			if(PartHeaderGroupsPanel != null)
				PartHeaderGroupsPanel.Orientation = AutoHideTray.GetOrientation(this);
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is BaseLayoutItem || item is AutoHideTrayHeadersGroup;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new AutoHideTrayHeadersGroup(this);
		}
		protected override void PrepareContainer(DependencyObject element, object item) {
			AutoHideTrayHeadersGroup headersGroup = element as AutoHideTrayHeadersGroup;
			if(headersGroup != null && DockLayoutManagerExtension.IsInContainer(this)) {
				headersGroup.SetValue(OrientationProperty, GetOrientation(this));
				headersGroup.EnsureLayoutItem(item as AutoHideGroup);
			}
			BaseLayoutItem layoutItem = LayoutItemData.ConvertToBaseLayoutItem(element);
			if(layoutItem != null)
				layoutItem.SelectTemplate();
		}
		protected override void ClearContainer(DependencyObject element) {
			AutoHideTrayHeadersGroup headersGroup = element as AutoHideTrayHeadersGroup;
			if(headersGroup != null)
				headersGroup.Dispose();
			BaseLayoutItem layoutItem = LayoutItemData.ConvertToBaseLayoutItem(element);
			if(layoutItem != null) {
				layoutItem.ClearTemplate();
				AutoHideGroup ahGroup = layoutItem as AutoHideGroup;
				if(ahGroup != null && ahGroup.Items.Contains(HotItem))
					ClosePanelCore();
			}
		}
		public event RoutedEventHandler Expanded {
			add { base.AddHandler(ExpandedEvent, value); }
			remove { base.RemoveHandler(ExpandedEvent, value); }
		}
		public event RoutedEventHandler Collapsed {
			add { base.AddHandler(CollapsedEvent, value); }
			remove { base.RemoveHandler(CollapsedEvent, value); }
		}
		public event RoutedEventHandler PanelClosed {
			add { base.AddHandler(PanelClosedEvent, value); }
			remove { base.RemoveHandler(PanelClosedEvent, value); }
		}
		public event HotItemChangedEventHandler HotItemChanged {
			add { base.AddHandler(HotItemChangedEvent, value); }
			remove { base.RemoveHandler(HotItemChangedEvent, value); }
		}
		public event PanelResizingEventHandler PanelResizing {
			add { base.AddHandler(PanelResizingEvent, value); }
			remove { base.RemoveHandler(PanelResizingEvent, value); }
		}
		public event RoutedEventHandler PanelMaximized {
			add { base.AddHandler(PanelMaximizedEvent, value); }
			remove { base.RemoveHandler(PanelMaximizedEvent, value); }
		}
		public event RoutedEventHandler PanelRestored {
			add { base.AddHandler(PanelRestoredEvent, value); }
			remove { base.RemoveHandler(PanelRestoredEvent, value); }
		}
		public SWC.Dock DockType {
			get { return (SWC.Dock)GetValue(DockTypeProperty); }
			internal set { SetValue(DockTypePropertyKey, value); }
		}
		public bool IsAnimated { get; internal set; }
		public BaseLayoutItem[] GetItems() {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			foreach(AutoHideGroup group in Items) {
				items.AddRange(group.GetItems());
			}
			return items.ToArray();
		}
		int lockPanelChanging = 0;
		protected virtual void RaiseExpanded() {
			if(lockPanelChanging > 0) return;
			lockPanelChanging++;
			RaisePanelEvent(ExpandedEvent);
			lockPanelChanging--;
		}
		protected virtual void RaiseCollapsed() {
			if(lockPanelChanging > 0) return;
			lockPanelChanging++;
			RaisePanelEvent(CollapsedEvent);
			lockPanelChanging--;
		}
		protected virtual void RaiseHotItemChanged(BaseLayoutItem value, BaseLayoutItem prev) {
			if(lockPanelChanging > 0) return;
			lockPanelChanging++;
			base.RaiseEvent(
					new HotItemChangedEventArgs(value, prev) { RoutedEvent = HotItemChangedEvent, Source = this }
				);
			lockPanelChanging--;
		}
		protected void RaisePanelEvent(RoutedEvent routedEvent) {
			base.RaiseEvent(
					new RoutedEventArgs() { RoutedEvent = routedEvent, Source = this }
				);
		}
		protected virtual void RaisePanelResizing(double size) {
			base.RaiseEvent(
					new PanelResizingEventArgs(size) { RoutedEvent = PanelResizingEvent, Source = this }
				);
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			private set { SetValue(IsExpandedPropertyKey, value); }
		}
		public BaseLayoutItem HotItem {
			get { return (BaseLayoutItem)GetValue(HotItemProperty); }
			private set { SetValue(HotItemPropertyKey, value); }
		}
		public AutoHidePane Panel { get; private set; }
		protected internal virtual void EnsureAutoHidePanel(AutoHidePane trayPanel) {
			Panel = trayPanel;
		}
		public void DoMaximize(BaseLayoutItem item) {
			if(!IsExpanded) DoExpand(item);
			else Maximize(item);
		}
		void Maximize(BaseLayoutItem item) {
			if(!HasItems) return;
			LockAutoHide();
			HotItem = item;
			Container.CollapseOtherViews(item);
			RaisePanelEvent(PanelMaximizedEvent);
			UnlockAutoHide();
		}
		void Restore(BaseLayoutItem item) {
			if(!HasItems) return;
			LockAutoHide();
			Container.CollapseOtherViews(item);
			RaisePanelEvent(PanelRestoredEvent);
			UnlockAutoHide();
		}
		public void DoRestore(BaseLayoutItem item) {
			if(IsExpanded) Restore(item);
			else DoExpand(item);
		}
		Locker ExpandLocker = new Locker();
		public void DoExpand(BaseLayoutItem item) {
			if(!HasItems) return;
			if(ExpandLocker) return;
			ExpandLocker.Lock();
			LockAutoHide();
			HotItem = item;
			Container.CollapseOtherViews(item);
			if(!IsExpanded)
				IsExpanded = true;
			UnlockAutoHide();
			ExpandLocker.Unlock();
		}
		public void DoResizePanel(double size) {
			if(!HasItems) return;
			LockAutoHide();
			RaisePanelResizing(size);
			UnlockAutoHide();
		}
		public void DoCollapseIfPossible(bool force = false) {
			bool canHide = (Panel.CanHideCurrentItem && Container.AutoHideMode != AutoHideMode.Inline) || force;
			if(canHide) DoCollapse();
		}
		WeakReference lastCollapsedItem;
		public void DoCollapse(BaseLayoutItem itemToCollapse = null) {
			if(!HasItems) return;
			LockAutoHide();
			if(IsExpanded) {
				IsExpanded = false;
				lastCollapsedItem = new WeakReference(itemToCollapse);
			}
			UnlockAutoHide();
		}
		public void DoClosePanel() {
			if(!HasItems) return;
			ClosePanelCore();
		}
		protected void ClosePanelCore() {
			lockPanelChanging++;
			IsExpanded = false;
			RaisePanelEvent(PanelClosedEvent);
			HotItem = null;
			lockPanelChanging--;
		}
		static int lockAutoHide = 0;
		public bool IsAutoHideLocked { get { return lockAutoHide > 0; } }
		public void LockAutoHide() { lockAutoHide++; }
		public void UnlockAutoHide() { --lockAutoHide; }
		protected void TrayMouseEnter(object sender, SWI.MouseEventArgs e) {
			if (View!= null)
				View.EnsureLayoutRoot();
			lastCollapsedItem = null;
		}
		protected void TrayMouseMove(object sender, SWI.MouseEventArgs e) {
			if(ExpandOnMouseDown|| IsAutoHideLocked || Container.ViewAdapter.IsInEvent) return;
			LockAutoHide();
			AutoHideByMouse(e);
			UnlockAutoHide();
		}
		protected void AutoHideByMouse(SWI.MouseEventArgs e) {
			HitTestResult trayHit = GetHit(this, e);
			HitTestResult panelHit = GetHit(Panel, e);
			if(trayHit == null && panelHit == null && IsExpanded && Panel.CanCollapse && Container.AutoHideMode != AutoHideMode.Inline) {
				DoCollapse();
				lastCollapsedItem = null;
			}
			if(trayHit != null && panelHit == null) {
				DependencyObject hitObj = trayHit.VisualHit;
				if(hitObj != null) {
					BaseLayoutItem item = GetHitItem(hitObj);
					if(item != null && !(item is AutoHideGroup)) {
						if(!IsExpanded) {
							if(lastCollapsedItem.Return(x => x.Target, () => null) == item) return;
							Container.ViewAdapter.ProcessAction(ViewAction.Hiding);
							DoExpand(item);
						}
						else
							if(Panel.CanHideCurrentItem) HotItem = item;
					}
				}
				lastCollapsedItem = null;
			}
		}
		static BaseLayoutItem GetHitItem(DependencyObject hitObj) {
			return DockLayoutManager.GetLayoutItem(hitObj);
		}
		void EnsureVisibility(bool hasItems) {
			Visibility value = VisibilityHelper.Convert(hasItems);
			this.Visibility = value;
			if(Panel != null) {
				Panel.Visibility = value;
				Panel.IsCollapsed = true;
			}
		}
		Docking.Platform.HitTestHelper.HitCache hitCache;
		HitTestResult GetHit(UIElement element, SWI.MouseEventArgs e) {
			return Docking.Platform.HitTestHelper.HitTest(element, e.GetPosition(element), ref hitCache);
		}
	}
}
