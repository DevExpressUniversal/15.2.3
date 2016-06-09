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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public static class DockLayoutManagerExtension {
		public static void BeginUpdate(this DockLayoutManager manager) {
			((ISupportBatchUpdate)manager).Do(x => x.BeginUpdate());
		}
		public static void EndUpdate(this DockLayoutManager manager) {
			((ISupportBatchUpdate)manager).Do(x => x.EndUpdate());
		}
		internal static string GetThemeName(DockLayoutManager manager) {
			return DevExpress.Xpf.Bars.BarManagerHelper.GetThemeName(manager);
		}
		public static void Update(this DockLayoutManager manager) {
			if(manager == null) return;
			manager.Update();
		}
		public static void AddToLogicalTree(this DockLayoutManager manager, FloatingContainer container, object content) {
			if(manager == null) return;
			DockLayoutManager.AddLogicalChild(manager, container);
			DockLayoutManager.AddLogicalChild(manager, content as FrameworkElement);
			if(content is IControlHost) {
				FrameworkElement[] elements = ((IControlHost)content).GetChildren();
				for(int i = 0; i < elements.Length; i++) {
					if(elements[i] != null)
						DockLayoutManager.AddLogicalChild(manager, elements[i]);
				}
			}
		}
		public static void RemoveFromLogicalTree(this DockLayoutManager manager, FloatingContainer container, object content) {
			if(manager == null) return;
			DockLayoutManager.RemoveLogicalChild(manager, container);
			DockLayoutManager.RemoveLogicalChild(manager, content as FrameworkElement);
			if(content is IControlHost) {
				FrameworkElement[] elements = ((IControlHost)content).GetChildren();
				for(int i = 0; i < elements.Length; i++) {
					if(elements[i] != null)
						DockLayoutManager.RemoveLogicalChild(manager, elements[i]);
				}
			}
		}
		internal static IUIElement FindUIScope(this DependencyObject dObj) {
			if(dObj is DockLayoutManager)
				return null;
			if(dObj is AutoHideTray)
				return ((IUIElement)dObj).Scope;
			if(dObj is FloatPanePresenter)
				return ((IUIElement)dObj).Scope;
			while(dObj != null) {
				dObj = GetVisualOrLogicalParent(dObj);
				if(dObj is IUIElement)
					return dObj as IUIElement;
				if(dObj is FloatPanePresenter.FloatingContentPresenter)
					return ((FloatPanePresenter.FloatingContentPresenter)dObj).Container as IUIElement;
			}
			return null;
		}
		static DependencyObject GetVisualOrLogicalParent(DependencyObject dObj) {
			DependencyObject parent = dObj is Visual ? VisualTreeHelper.GetParent(dObj) : null;
			return parent ?? LogicalTreeHelper.GetParent(dObj);
		}
		internal static IUIElement GetRootUIScope(this IUIElement element) {
			while(element != null && !(element.Scope is DockLayoutManager))
				element = element.Scope;
			return element;
		}
		internal static void Update(this LayoutGroup group, bool shouldUpdateLayout) {
			DockLayoutManager manager = GetManager(group);
			if(manager != null && !manager.IsDisposing) manager.Update(shouldUpdateLayout);
		}
		internal static void Update(this LayoutGroup group) {
			DockLayoutManager manager = GetManager(group);
			if(manager != null && !manager.IsDisposing) manager.Update();
		}
		static DockLayoutManager GetManager(this LayoutGroup group) {
			while(group != null) {
				if(group.Manager != null)
					return group.Manager;
				group = group.Parent;
			}
			return null;
		}
		internal static IUIElement GetManager(this IUIElement element) {
			while(element != null) {
				if(element is DockLayoutManager)
					break;
				if(element.Scope == null) {
					element = GetParentIUIElement(element as DependencyObject);
				}
				else element = element.Scope;
			}
			return element;
		}
		internal static IUIElement GetParentIUIElement(this DependencyObject dObj) {
			if(dObj != null) {
				do {
					dObj = GetVisualParent(dObj);
					if(dObj is IUIElement)
						return dObj as IUIElement;
				} while(dObj != null);
			}
			return null;
		}
		static DependencyObject GetVisualParent(DependencyObject child) {
			if(child is FloatPanePresenter.FloatingContentPresenter)
				child = ((FloatPanePresenter.FloatingContentPresenter)child).Container;
			return VisualTreeHelper.GetParent(child);
		}
		public static ILayoutElement GetViewElement(this DockLayoutManager container, IUIElement element) {
			IView view = GetView(container, GetRootUIScope(element));
			return (view != null) ? view.GetElement(element) : null;
		}
		public static IView GetView(this DockLayoutManager container, LayoutGroup group) {
			if(group == null) return null;
			IUIElement element = group.GetUIElement<IUIElement>();
			return GetView(container, GetRootUIScope(element));
		}
		public static IView GetView(this DockLayoutManager container, IUIElement element) {
			if(element != null && container != null && container.ViewAdapter != null) {
				foreach(IView view in container.ViewAdapter.Views) {
					if(view.RootKey == element) return view;
				}
			}
			return null;
		}
		public static UIElement GetUIElement(this DockLayoutManager container, BaseLayoutItem item) {
			return (item != null) ? item.GetUIElement<IUIElement>() as UIElement : null;
		}
		public static T GetUIElement<T>(this DockLayoutManager container, BaseLayoutItem item) where T : class {
			return (item != null) ? item.GetUIElement<T>() : null;
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static UIElement GetLastUIElement(this DockLayoutManager container, BaseLayoutItem item) {
				if (item != null) {
					var elements = item.UIElements.GetElements();
					return elements != null && elements.Length > 0 ? elements[elements.Length - 1] as UIElement : null;
				}
				return null;
		}
		public static BaseLayoutItem GetItem(this DockLayoutManager container, string name) {
			return BaseLayoutItemCollection.FindItem(GetItems(container), name);
		}
		public static BaseLayoutItem[] GetItems(this DockLayoutManager container) {
			if(container == null) return new BaseLayoutItem[0];
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			if(container.LayoutRoot != null)
				container.LayoutRoot.Accept(items.Add);
			if(container.FloatGroups != null)
				container.FloatGroups.Accept((fGroup) => fGroup.Accept(items.Add));
			if(container.AutoHideGroups != null)
				container.AutoHideGroups.Accept((ahGroup) => ahGroup.Accept(items.Add));
			if(container.ClosedPanels != null)
				container.ClosedPanels.Accept(items.Add);
			if(container.DecomposedItems!=null)
				container.DecomposedItems.Accept(items.Add);
			List<BaseLayoutItem> result = new List<BaseLayoutItem>();
			foreach(BaseLayoutItem item in items) {
				result.Add(item);
				LayoutPanel panel = item as LayoutPanel;
				if(panel != null && panel.Layout != null)
					panel.Layout.Accept(result.Add);
			}
			return result.ToArray();
		}
		public static LayoutElementHitInfo CalcHitInfo(this DockLayoutManager container, Point point) {
			if(container == null) return LayoutElementHitInfo.Empty;
			LayoutElementHitInfo result = LayoutElementHitInfo.Empty;
			IView view = container.ViewAdapter.GetView(point);
			if(view != null) {
				Point viewPoint = view.ScreenToClient(point);
				return view.Adapter.CalcHitInfo(view, viewPoint);
			}
			return result;
		}
		public static void InvalidateView(this DockLayoutManager container, LayoutGroup group) {
			if(group == null) return;
			IUIElement element = group.UIElements.GetElement<IUIElement>() ?? group;
			if(element != null) {
				IView view = GetView(container, GetRootUIScope(element));
				if(view != null) view.Invalidate();
			}
		}
		public static void HideView(this DockLayoutManager container, LayoutGroup group) {
			HideView(container, group, true);
		}
		public static void HideView(this DockLayoutManager container, LayoutGroup group, bool immediately) {
			if(group == null) return;
			IUIElement element = group.GetUIElement<IUIElement>() ?? group;
			if(element != null) {
				using(new LogicalTreeLocker(container, new BaseLayoutItem[] { group })) {
					IView view = GetView(container, GetRootUIScope(element));
					if(view != null && view.Type == HostType.AutoHide) {
						container.ViewAdapter.ActionService.Hide(view, immediately);
					}
				}
			}
		}
		internal static bool IsViewCreated(this DockLayoutManager container, LayoutGroup group) {
			if(group == null) return false;
			IUIElement element = group.GetUIElement<IUIElement>() ?? group;
			return (element != null) && GetView(container, element) != null;
		}
		internal static Comparison<FloatGroup> GetFloatGroupComparison(this DockLayoutManager container) {
			return delegate(FloatGroup g1, FloatGroup g2) {
				if(g1 != g2) {
					IView view1 = container.GetView(g1.UIElements.GetElement<FloatPanePresenter>());
					IView view2 = container.GetView(g2.UIElements.GetElement<FloatPanePresenter>());
					if(view1 == null || view2 == null) return 0;
					return view2.ZOrder.CompareTo(view1.ZOrder);
				}
				return 0;
			};
		}
		internal static bool IsInContainer(DependencyObject dObj) {
			return LayoutHelper.FindParentObject<DockLayoutManager>(dObj) != null;
		}
		internal static bool IsInFloatingContainer(DependencyObject dObj) {
			return LayoutHelper.FindParentObject<FloatPanePresenter.FloatingContentPresenter>(dObj) != null;
		}
		internal static void RaiseItemEvent(this DockLayoutManager container, BaseLayoutItem item, RoutedEvent routedEvent) {
			ItemEventArgs ea = new ItemEventArgs(item)
			{
				RoutedEvent = routedEvent,
				Source = container
			};
			container.RaiseEvent(ea);
		}
		internal static bool RaiseItemCancelEvent(this DockLayoutManager container, BaseLayoutItem item, RoutedEvent routedEvent) {
			ItemCancelEventArgs ea = new ItemCancelEventArgs(item)
			{
				RoutedEvent = routedEvent,
				Source = container
			};
			container.RaiseEvent(ea);
			return ea.Cancel;
		}
		internal static bool RaiseItemDockingEvent(this DockLayoutManager container, RoutedEvent routedEvent,
			BaseLayoutItem item, Point pt, BaseLayoutItem target, DockType type, bool isHiding) {
			DockItemDockingEventArgs ea = new DockItemDockingEventArgs(item, pt, target, type, isHiding)
			{
				RoutedEvent = routedEvent,
				Source = container
			};
			container.RaiseEvent(ea);
			return ea.Cancel;
		}
		internal static bool RaiseItemSelectionChangingEvent(this DockLayoutManager container, BaseLayoutItem item, bool selected) {
			LayoutItemSelectionChangingEventArgs ea = new LayoutItemSelectionChangingEventArgs(item, selected) { Source = container };
			container.RaiseEvent(ea);
			return ea.Cancel;
		}
		internal static void RaiseItemSelectionChangedEvent(this DockLayoutManager container, BaseLayoutItem item, bool selected) {
			container.RaiseEvent(
					new LayoutItemSelectionChangedEventArgs(item, selected) { Source = container }
				);
		}
		internal static void RaiseItemSizeChangedEvent(this DockLayoutManager container, BaseLayoutItem item, bool isWidth, GridLength value, GridLength prevValue) {
			container.RaiseEvent(
					new LayoutItemSizeChangedEventArgs(item, isWidth, value, prevValue) { Source = container }
				);
		}
		internal static bool RaiseDockItemDraggingEvent(this DockLayoutManager container, BaseLayoutItem item, Point screenLocation) {
			DockItemDraggingEventArgs ea = new DockItemDraggingEventArgs(screenLocation, item);
			container.RaiseEvent(ea);
			return ea.Cancel;
		}
		internal static void RaiseDockItemActivatedEvent(this DockLayoutManager container, BaseLayoutItem item, BaseLayoutItem oldItem) {
			DockItemActivatedEventArgs ea = new DockItemActivatedEventArgs(item, oldItem) { Source = container };
			InvokeHelper.BeginInvoke(container, new Action<RoutedEventArgs>(container.RaiseEvent), InvokeHelper.Priority.Normal, new object[] { ea });
		}
		internal static void RaiseShowingDockHintsEvent(this DockLayoutManager container, BaseLayoutItem item, BaseLayoutItem target, DockHintsConfiguration state = null) {
			ShowingDockHintsEventArgs ea = new ShowingDockHintsEventArgs(item, target) { DockHintsConfiguration = state ?? new DockHintsConfiguration(), };
			container.RaiseEvent(ea);
		}
		internal static bool RaiseShowingTabHintsEvent(this DockLayoutManager container, BaseLayoutItem item, BaseLayoutItem target, int insertIndex) {
			LayoutGroup group = target as LayoutGroup;
			if(group != null && group.Items.IsValidIndex(insertIndex)) target = group[insertIndex];
			ShowingDockHintsEventArgs ea = new ShowingDockHintsEventArgs(item, target) { DockHintsConfiguration = new DockHintsConfiguration(), };
			container.RaiseEvent(ea);
			return ea.DockHintsConfiguration.GetIsVisible(DockHint.TabHeader);
		}
		internal static BaseLayoutItem RaiseBeforeItemAddedEvent(this DockLayoutManager container, object item, BaseLayoutItem target) {
			BeforeItemAddedEventArgs ea = new BeforeItemAddedEventArgs(item, target);
			container.RaiseEvent(ea);
			return ea.Cancel ? null : ea.Target;
		}
		internal static bool RaiseDockOperationStartingEvent(this DockLayoutManager container, DockOperation dockOperation, BaseLayoutItem item, BaseLayoutItem target = null) {
			DockOperationStartingEventArgs ea = new DockOperationStartingEventArgs(item, target, dockOperation) { Source = container };
			container.RaiseEvent(ea);
			return ea.Cancel;
		}
		internal static void RaiseDockOperationCompletedEvent(this DockLayoutManager container, DockOperation dockOperation, BaseLayoutItem item) {
			DockOperationCompletedEventArgs ea = new DockOperationCompletedEventArgs(item, dockOperation) { Source = container };
			container.RaiseEvent(ea);
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static void EnsureCustomizationRoot(this DockLayoutManager manager) {
			if(manager.CustomizationController.CustomizationRoot == null)
				manager.CustomizationController.CustomizationRoot = manager.LayoutRoot;
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static void EnsureCustomizationRoot(this DockLayoutManager manager, LayoutGroup root) {
			manager.CustomizationController.CustomizationRoot = root;
		}
		internal static bool ProcessPanelNavigation(this DockLayoutManager manager, BaseLayoutItem activeItem, bool forward) {
			if(activeItem != null && !(activeItem is LayoutPanel)) {
				activeItem = activeItem.GetRoot().ParentPanel;
			}
			if(activeItem != null) {
				BaseLayoutItem[] items = manager.GetItems();
				ObservableCollection<BaseLayoutItem> panels = new ObservableCollection<BaseLayoutItem>();
				foreach(BaseLayoutItem itm in items) {
					if(itm.IsClosed) continue;
					if(itm.ItemType == LayoutItemType.Document || itm.ItemType == LayoutItemType.Panel)
						panels.Add(itm);
				}
				if(panels.Count > 0) {
					int index = panels.IndexOf(activeItem) + (forward ? 1 : -1);
					if(index >= panels.Count) index = 0;
					if(index < 0) index = panels.Count - 1;
					manager.Activate(panels[index]);
					return true;
				}
			}
			return false;
		}
		public static T FindName<T>(this DockLayoutManager manager, string name) where T : class {
			return LayoutHelper.FindElementByName(manager, name) as T;
		}
		internal static double GetAvailableAutoHideSize(this DockLayoutManager container, bool horz) {
			Rect bounds = container.GetAutoHideResizeBounds();
			return bounds.IsEmpty ?
				double.NaN :
				horz ? bounds.Width : bounds.Height;
		}
		internal static Rect GetAutoHideResizeBounds(this DockLayoutManager container) {
			var autoHideLayer = container.AutoHideLayer;
			if(autoHideLayer == null || autoHideLayer.ActualWidth == 0 || autoHideLayer.ActualHeight == 0) return Rect.Empty;
			if(container.AutoHideMode == AutoHideMode.Inline) {
				var point = DevExpress.Xpf.Docking.Platform.CoordinateHelper.PointToScreen(container, autoHideLayer, new Point());
				var size = new Size(autoHideLayer.ActualWidth, autoHideLayer.ActualHeight);
				return new Rect(point, size);
			}
			else {
				IView rootView = container.GetView(container.LayoutRoot);
				if(rootView == null) return Rect.Empty;
				if(rootView.LayoutRoot == null) rootView.EnsureLayoutRoot();
				rootView.LayoutRoot.EnsureBounds();
				Rect result = ElementHelper.GetScreenRect(rootView);
				RectHelper.Inflate(ref result, -25, -12.5);
				return result;
			}
		}
		internal static void Deactivate(this DockLayoutManager container, BaseLayoutItem item) {
			if(container.IsActivationLocked) return;
			if(container.ActiveDockItem == item) container.ActiveDockItem = null;
			if(container.ActiveMDIItem == item) container.ActiveMDIItem = null;
			if(container.ActiveLayoutItem == item) container.ActiveLayoutItem = null;
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
		public static ICustomizationController GetCustomizationController(this DockLayoutManager container) {
			return container.CustomizationController;
		}
	}
}
