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

using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.UIInteraction;
namespace DevExpress.Xpf.Docking.Platform {
	public class LayoutViewUIInteractionListener : UIInteractionServiceListener {
		public LayoutView View {
			get { return ServiceProvider as LayoutView; }
		}
		public override void OnActivate() {
			View.Container.CustomizationController.CloseMenu();
		}
		public override bool OnActiveItemChanging(ILayoutElement element) {
			var dockLayoutElement = ((IDockLayoutElement)element);
			View.Container.RenameHelper.CancelRenamingAndResetClickedState();
			return dockLayoutElement.AllowActivate;
		}
		public override bool OnActiveItemChanged(ILayoutElement element) {
			BaseLayoutItem itemToActivate = ((IDockLayoutElement)element).Item;
			LayoutGroup activationRoot = View.Container.ActivateCore(itemToActivate);
			if(activationRoot != null)
				View.Container.CustomizationController.CustomizationRoot = activationRoot;
			return itemToActivate != null && itemToActivate.IsActive;
		}
		public override bool OnClickPreviewAction(LayoutElementHitInfo clickInfo) {
			if(!clickInfo.InHeader || KeyHelper.IsCtrlPressed || KeyHelper.IsShiftPressed)
				return View.Container.RenameHelper.CancelRenamingAndResetClickedState();
			return false;
		}
		public override bool OnClickAction(LayoutElementHitInfo clickInfo) {
			DockLayoutElementHitInfo info = clickInfo as DockLayoutElementHitInfo;
			if(info != null) {
				IDockLayoutElement dockLayoutElement = info.Element as IDockLayoutElement;
				if(info.InHeader)
					return RenameItemCore(dockLayoutElement);
				if(info.InCloseButton)
					return CloseItemCore(dockLayoutElement.Item);
				if(info.InPinButton)
					return PinItemCore(dockLayoutElement.Item);
				if(info.InExpandButton)
					return ExpandItemCore(dockLayoutElement.Item);
				if(info.InMaximizeButton)
					return MaximizeItemCore(dockLayoutElement.Item);
				if(info.InMinimizeButton)
					return MinimizeItemCore(dockLayoutElement.Item);
				if(info.InRestoreButton)
					return RestoreItemCore(dockLayoutElement.Item);
				if(info.InDropDownButton)
					return ShowMenuCore(dockLayoutElement.Item);
				if(info.InScrollNextButton)
					return ScrollNextCore(dockLayoutElement.Item);
				if(info.InScrollPrevButton)
					return ScrollPrevCore(dockLayoutElement.Item);
				if(info.InCollapseButton) 
					return ExpandItemCore(dockLayoutElement.Item);
				if(info.InHideButton) {
					return HideCore(dockLayoutElement.Item);
				}
			}
			return false;
		}
		protected virtual bool ScrollPrevCore(BaseLayoutItem scrollTarget) {
			LayoutGroup group = scrollTarget as LayoutGroup;
			return (group != null) && group.ScrollPrev();
		}
		protected virtual bool ScrollNextCore(BaseLayoutItem scrollTarget) {
			LayoutGroup group = scrollTarget as LayoutGroup;
			return (group != null) && group.ScrollNext();
		}
		protected virtual bool RenameItemCore(IDockLayoutElement item) {
			if(!View.Container.RenameHelper.CancelRenaming())
				return View.Container.RenameHelper.RenameByClick(item);
			return false;
		}
		protected virtual bool HideCore(BaseLayoutItem item) {
			LayoutPanel panelToHide = item as LayoutPanel;
			if(panelToHide != null) panelToHide.AutoHideExpandState = Base.AutoHideExpandState.Hidden;
			return panelToHide != null;
		}
		protected virtual bool ExpandItemCore(BaseLayoutItem item) {
			LayoutGroup groupToExpandCollapse = item as LayoutGroup;
			LayoutPanel panelToExpandCollapse = item as LayoutPanel;
			if(groupToExpandCollapse != null) {
				groupToExpandCollapse.Expanded = !groupToExpandCollapse.Expanded;
				View.AdornerHelper.InvalidateDockingHintsAdorner();
			}
			if(panelToExpandCollapse != null) {
				bool isExpanded = panelToExpandCollapse.AutoHideExpandState == Base.AutoHideExpandState.Expanded;
				panelToExpandCollapse.AutoHideExpandState = isExpanded ? Base.AutoHideExpandState.Visible : Base.AutoHideExpandState.Expanded;
			}
			return groupToExpandCollapse != null || panelToExpandCollapse != null;
		}
		protected virtual bool MaximizeItemCore(BaseLayoutItem item) {
			return View.Container.MDIController.Maximize(GetDocument(item) ?? item);
		}
		protected virtual bool MinimizeItemCore(BaseLayoutItem item) {
			return View.Container.MDIController.Minimize(GetDocument(item));
		}
		protected virtual bool RestoreItemCore(BaseLayoutItem item) {
			return View.Container.MDIController.Restore(GetDocument(item) ?? item);
		}
		protected static DocumentPanel GetDocument(BaseLayoutItem item) {
			DocumentPanel document = item as DocumentPanel;
			if(item is DocumentGroup)
				document = ((DocumentGroup)item).SelectedItem as DocumentPanel;
			if(item is FloatGroup)
				document = ((FloatGroup)item)[0] as DocumentPanel;
			return document;
		}
		protected virtual bool CloseItemCore(BaseLayoutItem itemToClose) {
			TabbedGroup tabGroup = itemToClose as TabbedGroup;
			if(tabGroup != null && !(tabGroup.ItemType == LayoutItemType.TabPanelGroup && tabGroup.IsFloatingRootItem))
				itemToClose = tabGroup.SelectedItem;
			return View.Container.DockController.CloseEx(itemToClose);
		}
		protected virtual bool PinItemCore(BaseLayoutItem item) {
			if(item == null) return false;
			if(PinDocument(item)) return true;
			return item.IsAutoHidden ? UnPinItem(item) : PinItem(item);
		}
		protected bool PinDocument(BaseLayoutItem item) {
			return item.IsTabDocument && item.ToggleTabPinStatus();
		}
		protected bool UnPinItem(BaseLayoutItem item) {
			if(!item.AllowDock)
				return View.Container.DockController.Float(item) != null;
			if(item.IsAutoHidden) item = item.Parent;
			return View.Container.DockController.Dock(item);
		}
		protected bool PinItem(BaseLayoutItem item) {
			if(item.Parent is TabbedGroup) item = item.Parent;
			if(item.IsFloating && View.Container.RaiseItemDockingEvent(DockLayoutManager.DockItemStartDockingEvent, item,
				CoordinateHelper.ZeroPoint, null, DockType.None, true)) return false;
			return View.Container.DockController.Hide(item);
		}
		protected override bool IsFloatingElement(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return (item is FloatGroup) || item.IsFloatingRootItem;
		}
		protected override bool IsMDIDocument(ILayoutElement element) {
			return element is MDIDocumentElement;
		}
		protected override bool IsControlItemElement(ILayoutElement element) {
			return element is ControlItemElement;
		}
		protected override bool DoControlItemDoubleClick(ILayoutElement element) {
			LayoutControlItem item = ((IDockLayoutElement)element).Item as LayoutControlItem;
			return (item != null) && SelectAllInControl(item);
		}
		bool SelectAllInControl(LayoutControlItem item) {
#if !SILVERLIGHT
			System.Windows.Input.RoutedCommand command = System.Windows.Input.ApplicationCommands.SelectAll;
			if(command.CanExecute(null, item.Control)) {
				command.Execute(null, item.Control);
				return true;
			}
#endif
			return false;
		}
		protected override bool MaximizeElementOnDoubleClick(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return View.Container.MDIController.Maximize(GetDocument(item) ?? item);
		}
		protected override bool RestoreElementOnDoubleClick(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return View.Container.MDIController.Restore(GetDocument(item) ?? item);
		}
		protected override bool IsMaximized(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			if(item is FloatGroup) return ((FloatGroup)item).IsMaximized;
			var panel = (GetDocument(((IDockLayoutElement)element).Item) ?? item) as LayoutPanel;
			return (panel != null) && panel.IsMaximized;
		}
		FloatingHelper helper;
		protected override IView GetFloatingView(ILayoutElement element) {
			helper = CreateFloatingHelper();
			return helper.GetFloatingView(element);
		}
		protected override void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) {
			helper.InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
		}
		protected virtual FloatingHelper CreateFloatingHelper() {
			return new FloatingHelper(View);
		}
		protected override bool CanFloatElementOnDoubleClick(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return item.FloatOnDoubleClick;
		}
		public override bool OnMenuAction(LayoutElementHitInfo clickInfo) {
			View.Container.RenameHelper.CancelRenamingAndResetClickedState();
			if(clickInfo.Element == null || !(clickInfo.Element is IDockLayoutElement)) return false;
			BaseLayoutItem menuTarget = ((IDockLayoutElement)clickInfo.Element).Item;
			return ShowMenuCore(menuTarget);
		}
		public override bool OnMiddleButtonClickAction(LayoutElementHitInfo clickInfo) {
			if(clickInfo.Element == null || !(clickInfo.Element is IDockLayoutElement) || !clickInfo.InHeader) return false;
			BaseLayoutItem clickTarget = ((IDockLayoutElement)clickInfo.Element).Item;
			return OnMiddleButtonClickActionCore(clickTarget);
		}
		protected virtual bool OnMiddleButtonClickActionCore(BaseLayoutItem clickTarget) {
			return clickTarget != null && clickTarget.IsTabPage && LayoutItemsHelper.IsDockItem(clickTarget) ?
				CloseItemCore(clickTarget) : false;
		}
		protected virtual bool ShowMenuCore(BaseLayoutItem menuTarget) {
			return new MenuHelper(View.Container).ShowMenu(menuTarget);
		}
		protected virtual void ShowItemsSelectorMenu(UIElement source, BaseLayoutItem[] items) {
			View.Container.CustomizationController.ShowItemSelectorMenu(source, items);
		}
		protected virtual void ShowItemContextMenu(BaseLayoutItem item) {
			View.Container.CustomizationController.ShowContextMenu(item);
		}
		protected virtual void ShowLayoutControlItemContextMenu(BaseLayoutItem item) {
			View.Container.CustomizationController.ShowControlItemContextMenu(item);
		}
	}
	public class FloatingViewUIInteractionListener : LayoutViewUIInteractionListener {
		public override void OnActivate() {
			base.OnActivate();
			VisualElements.FloatPanePresenter presenter = View.RootKey as VisualElements.FloatPanePresenter;
			if(presenter != null) presenter.Activate(View.Container);
		}
		protected override bool DockElementOnDoubleClick(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			DockSituation savedSituation = item.GetLastDockSituation();
			return savedSituation != null && savedSituation.DockTarget is AutoHideGroup ?
				View.Container.DockController.Hide(item, savedSituation.DockTarget as AutoHideGroup) :
				View.Container.DockController.Dock(item);
		}
		protected override bool CanMaximizeOrRestore(ILayoutElement element) {
			BaseLayoutItem item = ((IDockLayoutElement)element).Item;
			return item.IsMaximizable;
		}
	}
	public class FloatingViewNativeInteractionListener : FloatingViewUIInteractionListener {
		public new FloatingView View {
			get { return ServiceProvider as FloatingView; }
		}
		protected override bool MaximizeItemCore(BaseLayoutItem item) {
			return MaximizeView();
		}
		protected override bool MaximizeElementOnDoubleClick(ILayoutElement element) {
			return MaximizeView();
		}
		bool MaximizeView() {
			var fGroup = View.FloatGroup;
			var e = fGroup.UIElements.GetElement<DevExpress.Xpf.Docking.VisualElements.FloatingWindowPresenter>();
			return e.With(x => x.Window).Return(x => x.MaximizeOrRestore(true), null);
		}
		protected override bool RestoreElementOnDoubleClick(ILayoutElement element) {
			return RestoreView();
		}
		protected override bool RestoreItemCore(BaseLayoutItem item) {
			return RestoreView();
		}
		bool RestoreView() {
			var fGroup = View.FloatGroup;
			var e = fGroup.UIElements.GetElement<DevExpress.Xpf.Docking.VisualElements.FloatingWindowPresenter>();
			return e.With(x => x.Window).Return(x => x.MaximizeOrRestore(false), null);
		}
	}
	public class AutoHideViewUIInteractionListener : LayoutViewUIInteractionListener {
		public override bool OnMenuAction(LayoutElementHitInfo clickInfo) {
			AutoHideTrayElement trayElement = clickInfo.Element as AutoHideTrayElement;
			if(trayElement != null) {
				VisualElements.AutoHideTray tray = trayElement.Element as VisualElements.AutoHideTray;
				ShowItemsSelectorMenu(tray, tray.GetItems());
				return true;
			}
			else return base.OnMenuAction(clickInfo);
		}
		protected override FloatingHelper CreateFloatingHelper() {
			return new AutoHideFloatingHelper(View);
		}
		public override bool OnActiveItemChanging(ILayoutElement element) {
			if(TryCollapseAutoHideTray(element)) return false;
			return base.OnActiveItemChanging(element);
		}
		bool TryCollapseAutoHideTray(ILayoutElement element) {
			BaseLayoutItem itemToActivate = ((IDockLayoutElement)element).Item;
			AutoHideTrayElement trayElement = element as AutoHideTrayElement;
			if(itemToActivate == null && trayElement != null)
				CollapseAutoHideTray(trayElement.Tray);
			else {
				if(View.Container.CanAutoHideOnMouseDown) {
					var headerElement = element as AutoHidePaneHeaderItemElement;
					if(headerElement != null) {
						if(headerElement.Tray != null && itemToActivate == headerElement.Tray.HotItem) {
							if(CollapseAutoHideTray(headerElement.Tray, itemToActivate)) return true;
						}
					}
				}
			}
			return false;
		}
		bool CollapseAutoHideTray(VisualElements.AutoHideTray tray, BaseLayoutItem itemToCollapse = null) {
			bool res = tray != null && tray.IsExpanded;
			if(res) tray.DoCollapse(itemToCollapse);
			return res;
		}
	}
	class MenuHelper {
		DockLayoutManager Manager;
		public MenuHelper(DockLayoutManager manager) {
			this.Manager = manager;
		}
		public bool ShowMenu(BaseLayoutItem menuTarget) {
			if(Manager == null || menuTarget == null || !menuTarget.AllowContextMenu) return false;
			if(Manager.IsInDesignTime) return false;
			if(menuTarget is LayoutControlItem || menuTarget is FixedItem) {
				ShowLayoutControlItemContextMenu(menuTarget);
				return true;
			}
			LayoutGroup group = menuTarget as LayoutGroup;
			if(group != null) {
				ShowItemsSelectorMenu(group.GetUIElement<IUIElement>() as UIElement, group.GetItems());
			}
			else ShowItemContextMenu(menuTarget);
			return true;
		}
		void ShowItemsSelectorMenu(UIElement source, BaseLayoutItem[] items) {
			Manager.CustomizationController.ShowItemSelectorMenu(source, items);
		}
		void ShowItemContextMenu(BaseLayoutItem item) {
			Manager.CustomizationController.ShowContextMenu(item);
		}
		void ShowLayoutControlItemContextMenu(BaseLayoutItem item) {
			Manager.CustomizationController.ShowControlItemContextMenu(item);
		}
	}
}
