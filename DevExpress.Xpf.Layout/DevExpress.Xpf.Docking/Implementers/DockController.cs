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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Internal;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class DockController : DockControllerBase, IDockController2010 {
		public DockController(DockLayoutManager container)
			: base(container) {
		}
		protected override FloatGroup BoxToFloatGroupCore(BaseLayoutItem item) {
			if(Container.FloatingDocumentContainer == FloatingDocumentContainer.DocumentHost && item is DocumentPanel) {
				item = DockControllerHelper.BoxIntoDocumentHost(item, Container);
			}
			return base.BoxToFloatGroupCore(item);
		}
		protected override bool DockToSideCore(BaseLayoutItem itemToDock, BaseLayoutItem target, DockType type) {
			LayoutGroup targetGroup = target as LayoutGroup;
			if(targetGroup != null && !(target is TabbedGroup)) {
				return DockInExistingGroup(itemToDock, targetGroup, type);
			}
			targetGroup = target.Parent;
			Orientation neededOrientation = type.ToOrientation();
			bool result = false;
			if(targetGroup.GetIsDocumentHost() && itemToDock.ItemType != LayoutItemType.DocumentPanelGroup) {
				result = BoxAndDockInNewGroup(itemToDock, target, type);
			}
			else {
				bool canDockInExistingGroup = (targetGroup.IgnoreOrientation || targetGroup.Orientation == neededOrientation)
				&& !target.IsFloatingRootItem && !targetGroup.IsControlItemsHost;
				if(!canDockInExistingGroup) {
					result = DockInNewGroup(itemToDock, target, type);
				}
				else {
					int index = targetGroup.Items.IndexOf(target);
					InsertType insertType = type.ToInsertType();
					int targetPosition = insertType == InsertType.Before ? index : index + 1;
					if(itemToDock.Parent == targetGroup && itemToDock.Parent.Items.IndexOf(itemToDock) < targetPosition) targetPosition--;
					result = DockInExistingGroup(itemToDock, targetGroup, targetPosition, neededOrientation);
				}
			}
			return result;
		}
		bool DockInExistingGroup(BaseLayoutItem item, LayoutGroup targetGroup, int targetPosition, Orientation neededOrientation) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, targetGroup })) {
				if(item is DocumentPanel)
					item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
				return (targetGroup != null) && InsertItemInGroup(targetGroup, item, targetPosition);
			}
		}
		protected bool BoxAndDockInNewGroup(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				LayoutGroup behindGroup = target.Parent;
				AssertionException.IsNotNull(behindGroup);
				BoxGroupItemsInNewGroup(behindGroup, type.ToOrientation(), Container);
				bool result = InsertItemInGroup(behindGroup, item, type.ToInsertType() == InsertType.Before ? 0 : behindGroup.Items.Count);
				return result;
			}
		}
		public bool DockAsDocument(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			using(new LogicalTreeLocker(Container, item, target)) {
				using(new ActivateBatch(Container)) {
					DocumentGroup dGroup = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
					ClearPlaceHolder(item);
					return Dock(dGroup, target, type);
				}
			}
		}
	}
	public class DockControllerBase : IDockController, IActiveItemOwner, ILockOwner {
		bool isDisposingCore = false;
		DockLayoutManager containerCore;
		BaseLayoutItem activeItemCore;
		ActiveItemHelper ActivationHelper;
		public DockControllerBase(DockLayoutManager container) {
			containerCore = container;
			ActivationHelper = new ActiveItemHelper(this);
		}
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected void OnDisposing() {
			Ref.Dispose(ref ActivationHelper);
			containerCore = null;
			activeItemCore = null;
		}
		public DockLayoutManager Container {
			get { return containerCore; }
		}
		public bool Rename(BaseLayoutItem item) {
			return Container.RenameHelper.Rename(item);
		}
		public T CreateCommand<T>(BaseLayoutItem item) where T : DockControllerCommand, new() {
			return new T() { Controller = this, Item = item };
		}
		void RemovePanelCore(LayoutPanel panel) {
			Container.ClosedPanels.Remove(panel);
			DockLayoutManager.RemoveLogicalChild(Container, panel);
			panel.Manager = null;
			Container.Dispatcher.BeginInvoke(new Action(() =>
				DestroyPanelContent(panel)
				), System.Windows.Threading.DispatcherPriority.Render);
			ClearPlaceHolder(panel);
		}
		public void RemovePanel(LayoutPanel panel) {
			if(panel == null) return;
			using(new NotificationBatch(Container)) {
				LayoutGroup root = panel.GetRoot();
				DockControllerHelper.SetClosingBehavior(panel, ClosingBehavior.ImmediatelyRemove);
				try {
					if(!panel.IsClosed) CloseCore(panel, false);
					RemovePanelCore(panel);
				}
				finally {
					panel.ClearValue(DockControllerHelper.ClosingBehaviorProperty);
				}
				NotificationBatch.Action(Container, root);
			}
		}
		protected void ClearPlaceHolder(BaseLayoutItem panel) {
			LayoutGroup[] affectedGroups = PlaceHolderHelper.GetAffectedGroups(panel);
			PlaceHolderHelper.ClearPlaceHolder(panel);
			Array.ForEach(affectedGroups, (group) => TryUngroupGroup(group));
		}
		protected void ClearPlaceHolder(BaseLayoutItem panel, BaseLayoutItem dockTarget) {
			LayoutGroup affectedGroup = PlaceHolderHelper.GetAffectedGroup(panel);
			PlaceHolderHelper.ClearPlaceHolder(panel);
			if(affectedGroup != dockTarget) {
				using(new LogicalTreeLocker(Container, affectedGroup)) {
					TryUngroupGroup(affectedGroup);
				}
			}
		}
		static void DestroyPanelContent(LayoutPanel panel) {
			DisposeHelper.DisposeVisualTree(panel.Content as DependencyObject);
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void RemoveItem(BaseLayoutItem item) {
			if(item == null) return;
			using(new NotificationBatch(Container)) {
				object notificationTarget = item;
				LayoutGroup parent = item.Parent;
				if(parent != null) {
					notificationTarget = item.GetRoot();
					parent.Remove(item);
					TryUngroupGroup(parent);
				}
				else {
					if(item is LayoutGroup && ((LayoutGroup)item).ParentPanel != null) {
						notificationTarget = ((LayoutGroup)item).ParentPanel.GetRoot();
						((LayoutGroup)item).ParentPanel.Content = null;
					}
					else {
						if(item == Container.LayoutRoot) {
							notificationTarget = Container;
							Container.LayoutRoot = null;
						}
						if(item.IsClosed) {
							Container.ClosedPanels.Remove(item as LayoutPanel);
						}
						if(item is AutoHideGroup) {
							Container.AutoHideGroups.Remove(item as AutoHideGroup);
						}
						if(item is FloatGroup) {
							Container.FloatGroups.Remove(item as FloatGroup);
						}
					}
				}
				NotificationBatch.Action(Container, notificationTarget);
				Container.Update();
			}
		}
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public void AddItem(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			if(item == null || target == null || type == DockType.None) return;
			using(new NotificationBatch(Container)) {
				if(type != DockType.Fill) {
					if(target.Parent == null) return;
					LayoutGroup parent = target.Parent;
					int index = parent.Items.IndexOf(target);
					if(parent.Orientation == type.ToOrientation() || parent.IgnoreOrientation) {
						InsertType insertType = type.ToInsertType();
						if(insertType == InsertType.After) index++;
						if(!(parent is DocumentGroup) && item is DocumentPanel) {
							item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
						}
						InsertCore(parent, item, index);
					}
					else {
						DockInNewGroup(item, target, type);
					}
				}
				if(type == DockType.Fill && target is LayoutGroup) {
					if(!(target is DocumentGroup) && item is DocumentPanel) {
						item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
					}
					((LayoutGroup)target).Add(item);
				}
				Container.Update();
				NotificationBatch.Action(Container, item.GetRoot());
			}
		}
		public LayoutPanel AddPanel(Point floatLocation, Size floatSize) {
			LayoutPanel panel = Container.CreateLayoutPanel();
			panel.FloatSize = floatSize;
			FloatGroup floatGroup = DockControllerHelper.BoxIntoFloatGroup(panel, Container);
			floatGroup.FloatLocation = floatLocation;
			Container.FloatGroups.Add(floatGroup);
			Container.Update();
			return panel;
		}
		public LayoutPanel AddPanel(DockType type) {
			LayoutPanel panel = Container.CreateLayoutPanel();
			if(type != DockType.Fill && type != DockType.None) {
				DockInExistingGroup(panel, Container.LayoutRoot, type);
				Container.Update();
			}
			return panel;
		}
		public DocumentPanel AddDocumentPanel(DocumentGroup group) {
			if(group == null) return null;
			DocumentPanel panel = Container.CreateDocumentPanel();
			group.Items.Add(panel);
			CheckMDIState(group, panel);
			return panel;
		}
		public DocumentPanel AddDocumentPanel(Point floatLocation, Size floatSize) {
			DocumentPanel panel = Container.CreateDocumentPanel();
			panel.FloatSize = floatSize;
			FloatGroup floatGroup = DockControllerHelper.BoxIntoFloatGroup(panel, Container);
			floatGroup.FloatLocation = floatLocation;
			Container.FloatGroups.Add(floatGroup);
			return panel;
		}
		public DocumentPanel AddDocumentPanel(DocumentGroup group, Uri uri) {
			if(group == null) return null;
			DocumentPanel panel = Container.CreateDocumentPanel();
			panel.Content = uri;
			group.Items.Add(panel);
			CheckMDIState(group, panel);
			return panel;
		}
		public DocumentPanel AddDocumentPanel(Point floatLocation, Size floatSize, Uri uri) {
			DocumentPanel panel = Container.CreateDocumentPanel();
			panel.Content = uri;
			panel.FloatSize = floatSize;
			FloatGroup floatGroup = DockControllerHelper.BoxIntoFloatGroup(panel, Container);
			floatGroup.FloatLocation = floatLocation;
			Container.FloatGroups.Add(floatGroup);
			return panel;
		}
		void CheckMDIState(DocumentGroup dGroup, DocumentPanel panel) {
			if(dGroup.IsTabbed) return;
			if(dGroup.IsMaximized)
				MDIStateHelper.SetMDIState(panel, MDIState.Maximized);
		}
		public DocumentGroup AddDocumentGroup(DockType type) {
			DocumentGroup group = Container.CreateDocumentGroup();
			if(type != DockType.Fill && type != DockType.None) {
				DockInExistingGroup(group, Container.LayoutRoot, type);
				Container.Update();
			}
			return group;
		}
		public bool Dock(BaseLayoutItem item) {
			if(item == null || !item.AllowDock) return false;
			if(item is LayoutGroup) {
				LayoutGroup ownerGroup = LayoutGroup.GetOwnerGroup(item);
				if(ownerGroup is AutoHideGroup) item = ownerGroup;
			}
			if(OnDockOperationStarting(DockOperation.Dock, item)) return false;
			DockSituation situation = item.GetLastDockSituation();
			LayoutGroup root = item.GetRoot();
			DockType type = DockControllerHelper.GetDockTypeInContainer(Container, item);
			BaseLayoutItem target = Container.LayoutRoot;
			BaseLayoutItem itemToDock = GetItemToDock(item);
			LayoutGroup targetRoot = null;
			PlaceHolder placeHolder = itemToDock != null ? PlaceHolderHelper.GetPlaceHolder(itemToDock, PlaceHolderState.Docked) : null;
			if(placeHolder != null && PlaceHolderHelper.CanRestoreLayoutHierarchy(itemToDock)) {
				type = DockType.None;
				targetRoot = placeHolder.Parent.GetRoot();
			}
			else {
				if(situation != null && situation.DockTarget != null) {
					targetRoot = situation.DockTarget.GetRoot();
					bool fValidTarget = (situation.Root == targetRoot) && Container.IsViewCreated(targetRoot);
					if((targetRoot != root) && fValidTarget) {
						type = situation.Type;
						target = situation.DockTarget;
					}
				}
			}
			if(root is FloatGroup && situation != null && situation.Root == targetRoot) {
				if(situation.Width.IsAuto)
					item.ItemWidth = GridLength.Auto;
				if(situation.Height.IsAuto)
					item.ItemHeight = GridLength.Auto;
			}
			return DockCore(item, target, type);
		}
		public bool Dock(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			if(item == null || target == null) return false;
			if(!target.AllowDockToCurrentItem && type == DockType.Fill) return false;
			DockLayoutManager linkedManager = GetLinkedManager(target);
			AssertionException.IsNotNull(linkedManager);
			if(linkedManager != Container) return linkedManager.DockController.Dock(item, target, type);
			if(OnDockOperationStarting(DockOperation.Dock, item, target)) return false;
			return DockCore(item, target, type);
		}
		void EnsureDockItemState(BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			if(item.Closed && !Container.ClosedPanels.Contains(item)) item.Closed = false;
			if(panel != null) {
				if(panel.AutoHidden && !panel.IsAutoHidden) panel.AutoHidden = false;
			}
		}
		bool DockCore(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			if(!item.AllowDock) return false;
			bool result = false;
			LayoutGroup owner = LayoutGroup.GetOwnerGroup(target); 
			if(owner != null) target = owner;
			using(new NotificationBatch(Container)) {
				using(new UpdateBatch(Container)) {
					using(new ActivateBatch(Container)) {
						using(new LogicalTreeLocker(Container, item, target)) {
							EnsureDockItemState(item); 
							LayoutGroup parentGroup = item.Parent;
							FloatGroup floatRoot = null;
							AutoHideGroup autoHideRoot = null;
							BaseLayoutItem itemToDock = GetItemToDock(item, ref floatRoot, ref autoHideRoot);
							LayoutGroup targetRoot = target.GetRoot();
							LayoutGroup affectedGroup = PlaceHolderHelper.GetAffectedGroup(item);
							if(!target.IsFloating && type != DockType.None) {
								target.IsDockingTarget = true;
								try {
									ClearPlaceHolder(itemToDock, target);
								}
								finally {
									target.IsDockingTarget = false;
								}
							}
							bool focus = HasFocus(itemToDock);
							bool wasHidden = DockControllerHelper.CheckHideView(Container, item.GetRoot());
							switch(type) {
								case DockType.None:
									result = DockToPlaceHolder(itemToDock);
									break;
								case DockType.Fill:
									result = DockToFillCore(itemToDock, target);
									break;
								default:
									result = DockToSideCore(itemToDock, target, type);
									break;
							}
							if(result) {
								NotificationBatch.Action(Container, parentGroup.GetRoot());
								Container.DecomposedItems.Purge();
								if(target != parentGroup)
									TryUngroupGroup(parentGroup);
								if(floatRoot != null)
									TryUngroupGroup(floatRoot);
								if(autoHideRoot != null) {
									TryUngroupGroup(autoHideRoot);
									LayoutGroup.SetOwnerGroup(itemToDock, null);
									if(itemToDock is LayoutGroup)
										Container.DecomposedItems.Remove((LayoutGroup)itemToDock);
								}
								if(affectedGroup != null) TryUngroupGroup(affectedGroup);
								OnDockComplete(itemToDock, type, wasHidden);
								if(focus)
									ActivationHelper.FocusPanelItem(itemToDock);
								DockControllerHelper.CheckUpdateView(Container, targetRoot);
								Container.Update();
								OnDockOperationComplete(itemToDock, DockOperation.Dock);
								NotificationBatch.Action(Container, itemToDock.GetRoot());
							}
						}
					}
				}
				return result;
			}
		}
		bool OnDockOperationStarting(DockOperation dockOperation, BaseLayoutItem item, BaseLayoutItem target = null) {
			return Container.RaiseDockOperationStartingEvent(dockOperation, item, target);
		}
		void OnDockOperationComplete(BaseLayoutItem itemToDock, DockOperation dockOperation) {
			Container.RaiseDockOperationCompletedEvent(dockOperation, itemToDock);
		}
		protected virtual bool DockToFillCore(BaseLayoutItem itemToDock, BaseLayoutItem target) {
			LayoutGroup targetGroup = target as LayoutGroup;
			if(targetGroup == null && target.Parent is TabbedGroup)
				targetGroup = target.Parent;
			bool result = (targetGroup != null && (targetGroup is TabbedGroup || !targetGroup.IsControlItemsHost)) ?
				FillExistingGroup(itemToDock, targetGroup) : FillNewTabbedGroup(itemToDock, target);
			return result;
		}
		protected virtual bool DockToSideCore(BaseLayoutItem itemToDock, BaseLayoutItem target, DockType type) {
			LayoutGroup targetGroup = target as LayoutGroup;
			if(targetGroup == null && target.Parent.Orientation == type.ToOrientation() && target.Parent.Items.Count == 1)
				targetGroup = target.Parent;
			bool result = targetGroup != null && !(targetGroup is TabbedGroup) ?
				DockInExistingGroup(itemToDock, targetGroup, type) : DockInNewGroup(itemToDock, target, type);
			return result;
		}
		public bool Insert(LayoutGroup group, BaseLayoutItem item, int index) {
			return InsertCore(group, item, index);
		}
		protected bool InsertCore(LayoutGroup group, BaseLayoutItem item, int index) {
			if(item == null || group == null || index == -1) return false;
			LayoutGroup owner = LayoutGroup.GetOwnerGroup(group); 
			if(owner != null) group = owner;
			LayoutGroup parentGroup = item.Parent;
			LayoutGroup targetRoot = group.GetRoot();
			using(new UpdateBatch(Container)) {
				if(parentGroup == group) {
					bool res = group.MoveItem(index, item);
					if(res) {
						Container.InvalidateView(targetRoot);
						Container.Update();
					}
					return res;
				}
				if(item.ItemType != LayoutItemType.ControlItem) {
					if(!item.AllowDock) return false;
				}
				else {
					if(!item.AllowMove) return false;
				}
				FloatGroup floatRoot = null;
				AutoHideGroup autoHideRoot = null;
				BaseLayoutItem itemToDock = GetItemToDock(item, ref floatRoot, ref autoHideRoot);
				DockControllerHelper.CheckHideView(Container, parentGroup);
				if(!group.IsFloating)
					ClearPlaceHolder(itemToDock, group);
				bool result = group is TabbedGroup && itemToDock is LayoutGroup ?
					FillExistingGroup((LayoutGroup)itemToDock, group, index) :
					InsertItemInGroup(group, itemToDock, index);
				if(result) {
					TryUngroupGroup(parentGroup);
					if(floatRoot != null)
						TryUngroupGroup(floatRoot);
					if(autoHideRoot != null)
						TryUngroupGroup(autoHideRoot);
					Container.InvalidateView(targetRoot);
					Container.Update();
				}
				return result;
			}
		}
		public bool Close(BaseLayoutItem item) {
			return CloseCore(item);
		}
		void AddPlaceHolder(BaseLayoutItem item, LayoutGroup group) {
			if(group.PlaceHolderHelper.Contains(item)) {
				group.PlaceHolderHelper.AddPlaceHolderForItem(item);
			}
		}
		bool CloseCore(BaseLayoutItem item, bool shouldRaiseEvent = true) {
			if(item == null || item.IsClosed || !item.AllowClose) return false;
			if(shouldRaiseEvent && (RaiseItemCancelEvent(item, DockLayoutManager.DockItemClosingEvent) || OnDockOperationStarting(DockOperation.Close, item))) return false;
			if(item is IClosable && !((IClosable)item).CanClose()) return false;
			try {
				item.IsClosing = true;
				if(item.IsAutoHidden)
					Container.HideView(item.Parent);
				var affectedItems = DockControllerHelper.GetAffectedItems(item);
				bool closed = false;
				BaseLayoutItem[] lockedItems = affectedItems.ToArray();
				using(new LogicalTreeLocker(Container, lockedItems)) {
					using(new NotificationBatch(Container)) {
						NotificationBatch.Action(Container, item.GetRoot());
						if(item is FloatGroup) {
							closed = CloseFloatGroup(item as FloatGroup);
						}
						if(item is AutoHideGroup) {
							closed = CloseAutoHideGroup(item as AutoHideGroup);
						}
						if(!closed) {
							LayoutGroup group = item.Parent;
							if(group == null) return false;
							AssertionException.IsNotNull(group);
							LayoutGroup root = item.GetRoot();
							if(group.Items.Count == 1) {
								if(group is FloatGroup) {
									closed = group.DestroyOnClosingChildren && CloseFloatGroup(group as FloatGroup);
								}
								AutoHideGroup ahGroup = group as AutoHideGroup;
								if(ahGroup != null) {
									bool canClose = !ahGroup.HasPersistentGroups;
									closed = canClose && group.DestroyOnClosingChildren && CloseAutoHideGroup(group as AutoHideGroup);
								}
							}
							closed = closed || CloseItem(item, group);
							DockControllerHelper.CheckUpdateView(Container, root);
						}
						if(closed) {
							Container.RaiseEvent(new DockItemClosedEventArgs(item, affectedItems));
							OnDockOperationComplete(item, DockOperation.Close);
							if(item is IClosable) ((IClosable)item).OnClosed();
						}
						Container.Update();
						NotificationBatch.Action(Container, item);
						return closed;
					}
				}
			}
			finally {
				item.IsClosing = false;
			}
		}
		public bool CloseAllButThis(BaseLayoutItem item) {
			if(item == null || item.Parent == null || item.Parent.Items.Count <= 1) return false;
			BaseLayoutItem[] items = new BaseLayoutItem[item.Parent.Items.Count];
			item.Parent.Items.CopyTo(items, 0);
			bool result = true;
			foreach(BaseLayoutItem node in items) {
				if(node != item) result = result & this.CloseEx(node);
			}
			return result;
		}
		bool HasFocus(BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			return panel != null && panel.Control != null && panel.Control.IsFocused;
		}
		protected virtual FloatGroup BoxToFloatGroupCore(BaseLayoutItem item) {
			return DockControllerHelper.BoxIntoFloatGroup(item, Container);
		}
		public FloatGroup Float(BaseLayoutItem item) {
			if(item == null || item.Parent == null || !item.AllowFloat) return null;
			if(OnDockOperationStarting(DockOperation.Float, item)) return null;
			if(item.IsAutoHidden)
				Container.HideView(item.Parent);
			DockSituation savedSituation = DockSituation.GetDockSituation(item);
			LayoutGroup group = item.Parent;
			BaseLayoutItem firstOtherItem = GetFirstOtherItem(item, group.GetItems());
			FloatGroup floatGroup = null;
			BaseLayoutItem layout = GetLayout(item);
			BaseLayoutItem[] lockItems = new BaseLayoutItem[] { item, group, layout };
			using(new LogicalTreeLocker(Container, lockItems)) {
				using(new NotificationBatch(Container)) {
					using(new UpdateBatch(Container)) {
						using(new ActivateBatch(Container)) {
							BaseLayoutItem nextItem = GetNextItemToFocus(item);
							bool focus = HasFocus(item) && nextItem != null;
							bool result = item.AllowFloat;
							if(result && !group.IsFloating) AddPlaceHolder(item, group);
							Size floatSize = item.FloatSize;
							if(result && group.Remove(item)) {
								if(focus) {
									ActivationHelper.FocusPanelItem(nextItem);
								}
								BaseLayoutItem dockTarget = group;
								NotificationBatch.Action(Container, dockTarget.GetRoot());
								if(TryUngroupGroup(group) != group && !(dockTarget is AutoHideGroup))
									dockTarget = firstOtherItem;
								item.UpdateDockSituation(savedSituation, dockTarget);
								DockControllerHelper.CheckUpdateView(Container, savedSituation.Root);
								floatGroup = BoxToFloatGroupCore(item);
								if(floatGroup.FloatSize != floatSize) floatGroup.FloatSize = floatSize;
								floatGroup.Manager = Container;
								Container.FloatGroups.Add(floatGroup);
								if(focus) {
									ActivationHelper.FocusPanelItem(item);
								}
								Container.Update();
								OnDockOperationComplete(item, DockOperation.Float);
								NotificationBatch.Action(Container, item.GetRoot());
							}
							return floatGroup;
						}
					}
				}
			}
		}
		BaseLayoutItem GetLayout(BaseLayoutItem item) {
			BaseLayoutItem layout = GetPanelLayout(item as LayoutPanel);
			if(item is LayoutGroup) {
				layout = GetPanelLayout(((LayoutGroup)item).SelectedItem as LayoutPanel);
			}
			return layout;
		}
		LayoutGroup GetPanelLayout(LayoutPanel panel) {
			return panel != null && panel.IsControlItemsHost ? panel.Layout : null;
		}
		protected void OnDockComplete(BaseLayoutItem item, DockType type, bool wasHidden) {
			if(item.IsClosed) {
				using(new LogicalTreeLocker(Container, item)) {
					Container.ClosedPanels.Remove((LayoutPanel)item);
				}
			}
			item.UpdateDockSituation(item.Parent, type);
			if(!wasHidden) item.UpdateAutoHideSituation();
		}
		BaseLayoutItem GetOriginalItemForAutoHideGroup(AutoHideGroup autoHideRoot) {
			DockSituation situation = autoHideRoot.GetLastDockSituation();
			BaseLayoutItem originalItem = situation != null ? situation.OriginalItem : null;
			if(originalItem == null) {
				var placeHolder = PlaceHolderHelper.GetPlaceHolder(autoHideRoot, PlaceHolderState.Unset);
				if(placeHolder != null)
					originalItem = placeHolder.Parent;
			}
			return originalItem;
		}
		BaseLayoutItem GetItemToDock(BaseLayoutItem item, ref FloatGroup floatRoot, ref AutoHideGroup autoHideRoot) {
			using(new LogicalTreeLocker(Container, item)) {
				BaseLayoutItem itemToDock = item;
				floatRoot = item.GetRoot() as FloatGroup;
				if(floatRoot != null) {
					itemToDock = GetFloatGroupContent(item);
					SaveFloatSize(floatRoot, itemToDock);
				}
				autoHideRoot = item as AutoHideGroup;
				if(autoHideRoot == null) {
					autoHideRoot = LayoutGroup.GetOwnerGroup(item) as AutoHideGroup;
				}
				if(autoHideRoot != null) {
					BaseLayoutItem[] items = autoHideRoot.GetItems();
					BaseLayoutItem originalItem = GetOriginalItemForAutoHideGroup(autoHideRoot);
					bool needInitialization = !(originalItem is TabbedGroup);
					if(items.Length == 1 && needInitialization)
						itemToDock = items[0];
					else {
						Container.HideView(autoHideRoot);
						TabbedGroup tabGroup = needInitialization ? Container.CreateTabbedGroup() : (TabbedGroup)originalItem;
						if(needInitialization)
							((ISupportInitialize)tabGroup).BeginInit();
						tabGroup.Manager = Container;
						tabGroup.AddRange(items);
						tabGroup.SelectedTabIndex = autoHideRoot.SelectedTabIndex;
						tabGroup.UpdateDockSituation(autoHideRoot.GetLastDockSituation(), Container.LayoutRoot);
						AutoHideGroup.SetAutoHideType(tabGroup, AutoHideGroup.GetAutoHideType(autoHideRoot));
						if(needInitialization)
							((ISupportInitialize)tabGroup).EndInit();
						itemToDock = tabGroup;
						PlaceHolderHelper.ClearPlaceHolder(autoHideRoot);
						autoHideRoot.HasPersistentGroups = false;
					}
				}
				return itemToDock;
			}
		}
		BaseLayoutItem GetItemToDock(BaseLayoutItem item) {
			BaseLayoutItem itemToDock = item;
			FloatGroup fGroup = item as FloatGroup;
			if(fGroup != null && fGroup.Items.Count == 1) {
				itemToDock = fGroup.Items[0];
			}
			AutoHideGroup autoHideRoot = item as AutoHideGroup;
			if(autoHideRoot != null) {
				BaseLayoutItem[] items = autoHideRoot.GetItems();
				BaseLayoutItem originalItem = GetOriginalItemForAutoHideGroup(autoHideRoot);
				if(originalItem is TabbedGroup)
					itemToDock = originalItem;
				else {
					if(items.Length == 1)
						itemToDock = items[0];
				}
			}
			return itemToDock;
		}
		void SaveFloatSize(FloatGroup floatRoot, BaseLayoutItem itemToDock) {
			Thickness margin = new Thickness(0);
			FloatPanePresenter presenter = Container.GetUIElement(floatRoot) as FloatPanePresenter;
			if(presenter != null) {
				margin = ((FloatPanePresenter.FloatingContentPresenter)presenter.Element).GetFloatingMargin();
			}
			itemToDock.FloatSize = new Size(
					Math.Max(1, floatRoot.FloatSize.Width - (margin.Left + margin.Right)),
					Math.Max(1, floatRoot.FloatSize.Height - (margin.Top + margin.Bottom))
				);
		}
		BaseLayoutItem GetFirstOtherItem(BaseLayoutItem item, BaseLayoutItem[] itemsBeforeUngroup) {
			return Array.Find(itemsBeforeUngroup, (i) => i != item);
		}
		BaseLayoutItem GetNextItemToFocus(BaseLayoutItem item, BaseLayoutItem[] items) {
			if(!items.Contains(item)) return null;
			int index = Array.FindIndex(items, Array.IndexOf(items, item) + 1, (itm) => itm is LayoutPanel && !item.IsAutoHidden);
			if(index == -1)
				index = Array.FindIndex(items, 0, Array.IndexOf(items, item), (itm) => itm is LayoutPanel && !item.IsAutoHidden);
			return index != -1 ? items[index] : null;
		}
		BaseLayoutItem GetNextItem(BaseLayoutItem item, LayoutGroup parentGroup) {
			bool isMDI = parentGroup is DocumentGroup && ((DocumentGroup)parentGroup).MDIStyle == MDIStyle.MDI;
			return GetNextItem(item, parentGroup.GetItems(), isMDI);
		}
		BaseLayoutItem GetNextItem(BaseLayoutItem item, BaseLayoutItem[] items, bool sortByZIndex) {
			if(sortByZIndex)
				items = LayoutItemsHelper.SortByZIndex(items);
			return GetNextItem(item, items);
		}
		BaseLayoutItem GetNextItem(BaseLayoutItem item, BaseLayoutItem[] items) {
			BaseLayoutItem next = null, prev = null;
			for(int i = 0; i < items.Length; i++) {
				if(items[i] != item) continue;
				if(i - 1 >= 0) prev = items[i - 1];
				if(i + 1 < items.Length) next = items[i + 1];
				break;
			}
			return prev ?? next;
		}
		BaseLayoutItem GetNextItemToFocus(BaseLayoutItem item) {
			if(item == null && item.Parent == null) return null;
			BaseLayoutItem next = GetNextItemToFocus(item, item.Parent.Items.ToArray());
			return next ?? GetNextItemToFocus(item, Container.GetItems());
		}
		DockType GetDockType(BaseLayoutItem item, LayoutGroup parentGroup) {
			if(parentGroup is TabbedGroup) return DockType.Fill;
			bool before = (double)parentGroup.Items.IndexOf(item) < (double)parentGroup.Items.Count * 0.5;
			bool fHorz = parentGroup.Orientation == Orientation.Horizontal;
			return before ? (fHorz ? DockType.Left : DockType.Top) : (fHorz ? DockType.Right : DockType.Bottom);
		}
		void RaiseItemEvent(BaseLayoutItem item, RoutedEvent routedEvent) {
			Container.RaiseItemEvent(item, routedEvent);
		}
		bool RaiseItemCancelEvent(BaseLayoutItem item, RoutedEvent routedEvent) {
			return Container.RaiseItemCancelEvent(item, routedEvent);
		}
		int lockActivateCounter = 0;
		public void Activate(BaseLayoutItem item) {
			Activate(item, true);
		}
		void LockActivate() {
			lockActivateCounter++;
		}
		void UnlockActivate() {
			lockActivateCounter--;
		}
		public void Activate(BaseLayoutItem item, bool focus) {
			if(lockActivateCounter > 0 || item == null || !item.AllowActivate || item.IsClosed || !item.IsVisible) return;
			if(item is LayoutGroup && ((LayoutGroup)item).IsUngroupped) return;
			lockActivateCounter++;
			try {
				if(ActiveItem != item) {
					if(RaiseItemCancelEvent(item, DockLayoutManager.DockItemActivatingEvent)) {
						item.InvokeCancelActivation(ActiveItem);
						return;
					}
				}
				ActivationHelper.ActivateItem(item, focus);
			}
			finally { lockActivateCounter--; }
		}
		public BaseLayoutItem ActiveItem {
			get { return activeItemCore; }
			set {
				if(ActiveItem == value) return;
				SetActiveItemCore(value);
			}
		}
		void SetActiveItemCore(BaseLayoutItem value) {
			SetActive(false);
			BaseLayoutItem oldItem = activeItemCore;
			activeItemCore = value;
			SetActive(true);
			Container.isDockItemActivation++;
			Container.ActiveDockItem = ActiveItem;
			Container.isDockItemActivation--;
			RaiseActiveItemChanged(value, oldItem);
		}
		void SetActive(bool value) {
			if(ActiveItem != null) {
				ActiveItem.SetActive(value);
				if(value && lockActivateCounter == 0) 
					ActivationHelper.SelectInGroup(ActiveItem);
			}
		}
		void RaiseActiveItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			Container.RaiseDockItemActivatedEvent(item, oldItem);
		}
		BaseLayoutItem GetFloatGroupContent(BaseLayoutItem item) {
			FloatGroup fGroup = item as FloatGroup;
			if(fGroup != null) {
				if(fGroup.Items.Count > 1) {
					BoxGroupItemsInNewGroup(fGroup, fGroup.Orientation, Container);
				}
				item = fGroup.Items[0];
			}
			return item;
		}
		protected virtual bool DockToPlaceHolder(BaseLayoutItem itemToDock) {
			bool result = RestoreToPlaceHolder(itemToDock, false);
			if(result) ClearPlaceHolder(itemToDock);
			return result;
		}
		protected bool DockInNewGroup(BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				LayoutGroup behindGroup = target.Parent;
				AssertionException.IsNotNull(behindGroup);
				int index = behindGroup.Items.IndexOf(target);
				if(item.Parent == behindGroup) {
					if(behindGroup.Items.IndexOf(item) < index) index--;
				}
				behindGroup.Items.Remove(target);
				LayoutGroup newGroup = DockControllerHelper.BoxIntoGroup(target, type.ToOrientation(), behindGroup.IsSplittersEnabled, Container);
				if(item is DocumentPanel)
					item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
				bool result = InsertItemInGroup(newGroup, item, type.ToInsertType() == InsertType.Before ? 0 : 1);
				behindGroup.Items.Insert(index, newGroup);
				return result;
			}
		}
		protected bool DockInExistingGroup(BaseLayoutItem item, LayoutGroup target, DockType type) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				LayoutGroup group = target;
				Orientation neededOrientation = type.ToOrientation();
				InsertType insertType = type.ToInsertType();
				int targetPosition = insertType == InsertType.Before ? 0 : 1;
				if((target.IgnoreOrientation || target.Orientation == neededOrientation || target.Items.Count <= 1) && !group.IsControlItemsHost) {
					if(target.Orientation != neededOrientation) target.Orientation = neededOrientation;
					targetPosition = type.ToInsertType() == InsertType.Before ? 0 : group.Items.Count;
				}
				else
					BoxGroupItemsInNewGroup(target, neededOrientation, Container);
				if(item is DocumentPanel)
					item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
				return (group != null) && InsertItemInGroup(group, item, targetPosition);
			}
		}
		bool DockInExistingGroup(BaseLayoutItem item, LayoutGroup target, int targetPosition) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				if(item is DocumentPanel && target.ItemType != LayoutItemType.DocumentPanelGroup)
					item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
				return (target != null) && InsertItemInGroup(target, item, targetPosition);
			}
		}
		protected void BoxGroupItemsInNewGroup(LayoutGroup target, Orientation neededOrientation, DockLayoutManager manager) {
			LayoutGroup newGroup = manager.CreateLayoutGroup();
			((ISupportInitialize)newGroup).BeginInit();
			newGroup.Orientation = target.Orientation;
			BaseLayoutItem[] existingItems = target.GetItems();
			for(int i = 0; i < existingItems.Length; i++) {
				target.Items.Remove(existingItems[i]);
				newGroup.Items.Add(existingItems[i]);
			}
			((ISupportInitialize)newGroup).EndInit();
			target.Orientation = neededOrientation;
			target.Add(newGroup);
			Container.Update();
		}
		bool FillNewTabbedGroup(BaseLayoutItem item, BaseLayoutItem target) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				AssertionException.IsNotNull(target.Parent);
				bool result = (item.Parent == null) || item.Parent.Remove(item);
				if(result) {
					int index = target.Parent.Items.IndexOf(target);
					LayoutGroup behindGroup = target.Parent;
					behindGroup.Items.Remove(target);
					TabbedGroup newGroup = target.ItemType == LayoutItemType.Document ?
						DockControllerHelper.BoxIntoDocumentGroup(target, Container) :
						DockControllerHelper.BoxIntoTabbedGroup(target, Container);
					newGroup.AddRange(DockControllerHelper.Decompose(item));
					behindGroup.Items.Insert(index, newGroup);
					ActivationHelper.SelectInGroup(item, newGroup);
				}
				return result;
			}
		}
		bool FillExistingGroup(BaseLayoutItem item, LayoutGroup target) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, target })) {
				BaseLayoutItem selected = item;
				LayoutGroup gr = item as LayoutGroup;
				if(gr != null) {
					var active = gr.GetNestedPanels().Where(x => x.IsActive).FirstOrDefault();
					if(active != null) selected = active;
				}
				bool result = (item.Parent == null) || item.Parent.Remove(item);
				if(result) {
					if(item is DocumentPanel && target.ItemType == LayoutItemType.Group) {
						item = DockControllerHelper.BoxIntoDocumentGroup(item, Container);
						target.Add(item);
					}
					else {
						if(target is TabbedGroup)
							target.AddRange(DockControllerHelper.Decompose(item));
						else
							target.Add(item);
					}
					ActivationHelper.SelectInGroup(selected, target);
				}
				return result;
			}
		}
		bool FillExistingGroup(LayoutGroup group, LayoutGroup target, int index) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { group, target })) {
				bool result = (group.Parent == null) || group.Parent.Remove(group);
				if(result) {
					BaseLayoutItem selected = group.SelectedItem;
					BaseLayoutItem[] items = DockControllerHelper.Decompose(group);
					foreach(BaseLayoutItem item in items) {
						target.Insert(index++, item);
					}
					if(selected != null)
						ActivationHelper.SelectInGroup(selected, target);
				}
				return result;
			}
		}
		protected bool InsertItemInGroup(LayoutGroup group, BaseLayoutItem item, int index, bool forceSizeUpdate = false) {
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, group })) {
				return DockControllerHelper.InsertItemInGroup(group, item, index, forceSizeUpdate);
			}
		}
		bool CloseItem(BaseLayoutItem item, LayoutGroup parentGroup) {
			if(parentGroup == null) return false;
			DockSituation savedSituation = DockSituation.GetDockSituation(item);
			BaseLayoutItem firstOtherItem = GetFirstOtherItem(item, parentGroup.GetItems());
			BaseLayoutItem selectedItem = parentGroup.SelectedItem;
			BaseLayoutItem nextItem = selectedItem == null || selectedItem == item ?
				GetNextItem(item, parentGroup) : selectedItem;
			if(nextItem == null && item.IsFloating) {
				var itemsToClose = new List<BaseLayoutItem> { item };
				nextItem = Container.GetItems()
					.Except(itemsToClose)
					.Where((x) => x is LayoutPanel && x.IsFloating)
					.OrderBy((x) => ((LayoutPanel)x).LastActivationDateTime, ListSortDirection.Descending)
					.FirstOrDefault();
			}
			bool result = parentGroup.Items.Contains(item);
			if(result && item is LayoutPanel) AddPlaceHolder(item, parentGroup);
			if(result) {
				using(parentGroup.TabHeaderScrollIndexLocker.LockOnce()) {
					if(!(parentGroup is AutoHideGroup)) {
						Container.Activate(nextItem);
					}
					if(parentGroup.Remove(item)) {
						SetClosed(item, parentGroup, result);
						BaseLayoutItem dockTarget = parentGroup;
						if(TryUngroupGroup(parentGroup) != parentGroup) dockTarget = firstOtherItem;
						if(item.IsClosed)
							item.UpdateDockSituation(savedSituation, dockTarget);
					}
				}
			}
			return result;
		}
		void SetClosed(BaseLayoutItem item, bool closed) {
			SetClosed(item, item as LayoutGroup, closed);
		}
		void SetClosed(BaseLayoutItem item, LayoutGroup root, bool closed) {
			FloatGroup floatRoot = root as FloatGroup;
			FloatGroup floatGroup = item as FloatGroup ?? floatRoot;
			IEnumerable<BaseLayoutItem> nestedItems = null;
			if(floatGroup != null) {
				nestedItems = floatGroup.GetNestedPanels();
				foreach(var nestedItem in nestedItems) {
					if(nestedItem.Parent != null) AddPlaceHolder(nestedItem, nestedItem.Parent);
				}
			}
			BaseLayoutItem[] itemsToClose = DockControllerHelper.Decompose(item);
			if(nestedItems != null) {
				foreach(var nestedItem in nestedItems) {
					LayoutGroup nestedGroup = nestedItem as LayoutGroup;
					if(nestedGroup != null && !Container.DecomposedItems.Contains(nestedGroup)) Container.DecomposedItems.Add(nestedGroup);
				}
			}
			if(item is LayoutGroup) {
				LayoutGroup group = (LayoutGroup)item;
				group.IsUngroupped = true;
				if(!Container.DecomposedItems.Contains(group))
					Container.DecomposedItems.Add(group);
			}
			Point restoreOffset = Container.GetRestoreOffset();
			for(int i = 0; i < itemsToClose.Length; i++) {
				LayoutPanel panel = (LayoutPanel)itemsToClose[i];
				bool closedViaCommand = false;
				if(floatGroup != null) {
					panel.UpdateDockSituation(null, null);
					panel.FloatLocationBeforeClose = floatGroup.FloatLocation;
					panel.FloatSizeBeforeClose = floatGroup.FloatSize;
					panel.FloatOffsetBeforeClose = restoreOffset;
					closedViaCommand = panel.ExecuteCloseCommand();
				}
				if(!closedViaCommand) {
					if(DockControllerHelper.GetActualClosingBehavior(Container, panel) == ClosingBehavior.ImmediatelyRemove) {
						RemovePanelCore(panel);
					}
					else Container.ClosedPanels.Add(panel);
				}
			}
			if(itemsToClose.Contains(Container.ActiveDockItem)) Container.ActiveDockItem = null;
			if(itemsToClose.Contains(Container.ActiveMDIItem)) Container.ActiveMDIItem = null;
		}
		bool CloseFloatGroup(FloatGroup floatGroup) {
			DockLayoutManager linkedManager = GetLinkedManager(floatGroup);
			AssertionException.IsNotNull(linkedManager);
			if(linkedManager != Container) return linkedManager.DockController.Close(floatGroup);
			bool result = Container.FloatGroups.Contains(floatGroup) && Container.FloatGroups.Remove(floatGroup);
			if(result) {
				var itemsToClose = floatGroup.GetNestedPanels();
				var lastActiveItem = Container.GetItems()
					.Except(itemsToClose)
					.Where((x) => x is LayoutPanel && x.IsFloating)
					.OrderBy((x) => ((LayoutPanel)x).LastActivationDateTime, ListSortDirection.Descending)
					.FirstOrDefault();
				SetClosed(floatGroup, result);
				Container.Activate(lastActiveItem);
			}
			return result;
		}
		bool CloseAutoHideGroup(AutoHideGroup autoHideGroup) {
			bool result = Container.AutoHideGroups.Contains(autoHideGroup) && Container.AutoHideGroups.Remove(autoHideGroup);
			if(result) {
				if(autoHideGroup.HasItems) AddPlaceHolder(autoHideGroup[0], autoHideGroup);
				SetClosed(autoHideGroup, result);
			}
			return result;
		}
		LayoutGroup TryUngroupGroup(LayoutGroup group) {
			if(group == null) return null;
			if(!group.DestroyOnClosingChildren || group.IsDockingTarget) return group;
			LayoutGroup parentGroup = group.Parent;
			switch(group.Items.Count) {
				case 0:
					AutoHideGroup ahGroup = group as AutoHideGroup;
					if(ahGroup != null) {
						if(ahGroup.HasPersistentGroups) return group;
						CloseAutoHideGroup(ahGroup);
					}
					else if(group is FloatGroup) {
						Action ungroupAction = new Action(() => { CloseFloatGroup(group as FloatGroup); });
						if(Container.ViewAdapter.IsInEvent && Container.IsFloating)
							Container.DelayedExecuteEnqueue(ungroupAction);
						else
							ungroupAction();
					}
					else {
						if(group.HasPlaceHolders && parentGroup != null) {
							AddPlaceHolder(group, parentGroup);
							group.IsUngroupped = true;
							if(!Container.DecomposedItems.Contains(group))
								Container.DecomposedItems.Add(group);
						}
						if(!CloseItem(group, parentGroup))
							break;
					}
					return parentGroup;
				case 1:
					if(!group.IsFloating && (group.HasPlaceHolders || (parentGroup != null && parentGroup.HasPlaceHolders))) return group;
					if(group is DocumentGroup) {
						if(!group.IsFloatingRootItem) return group;
						if(group.IsFloatingRootItem && Container.FloatingDocumentContainer == FloatingDocumentContainer.DocumentHost) return group;
					}
					BaseLayoutItem nestedItem = group.Items[0];
					if(nestedItem is LayoutControlItem)
						return group;
					if(parentGroup != null) {
						if(parentGroup.IgnoreOrientation || parentGroup.Orientation == Orientation.Horizontal) {
							if(!nestedItem.ItemWidth.IsAbsolute)
								nestedItem.ItemWidth = group.ItemWidth;
						}
						else {
							if(!nestedItem.ItemHeight.IsAbsolute)
								nestedItem.ItemHeight = group.ItemHeight;
						}
					}
					if(nestedItem.ItemType != LayoutItemType.TabPanelGroup)
						TryUngroupGroup(nestedItem as LayoutGroup);
					LayoutGroup unboxResult = DockControllerHelper.Unbox(Container, group);
					if(unboxResult is FloatGroup && unboxResult[0] is LayoutGroup)
						TryUngroupGroup((LayoutGroup)unboxResult[0]);
					return unboxResult;
			}
			return group;
		}
		DockLayoutManager GetLinkedManager(BaseLayoutItem target) {
			DockLayoutManager targetManager = target.Return(x => x.Manager, () => null);
			return targetManager != null && !targetManager.IsDisposing ? targetManager : Container;
		}
		public bool Hide(BaseLayoutItem item, AutoHideGroup target) {
			DockLayoutManager linkedManager = GetLinkedManager(target);
			AssertionException.IsNotNull(linkedManager);
			if(linkedManager != Container) return linkedManager.DockController.Hide(item, target);
			return HideCore(item, target, target.DockType, false);
		}
		public bool Hide(BaseLayoutItem item) {
			return HideCore(item, SWC.Dock.Left, true);
		}
		public bool Hide(BaseLayoutItem item, Dock dock) {
			return HideCore(item, dock, false);
		}
		protected bool HideCore(BaseLayoutItem item, AutoHideGroup target, Dock dock, bool calcDock) {
			AssertionException.IsTrue(item != null);
			if(item == null || (item.IsAutoHidden && item.Parent == target) || !item.AllowHide || item is AutoHideGroup) return false;
			if(item.IsClosed && !Restore(item)) return false;
			if(item is FloatGroup) {
				FloatGroup fg = item as FloatGroup;
				item = GetFloatGroupContent(item);
				item.FloatSize = fg.FloatSize;
			}
			if(!item.AllowHide || RaiseItemCancelEvent(item, DockLayoutManager.DockItemHidingEvent) || OnDockOperationStarting(DockOperation.Hide, item)) return false;
			DockSituation savedSituation = calcDock ? DockSituation.GetDockSituation(item) :
				DockSituation.GetDockSituation(item, dock);
			LayoutGroup group = item.Parent;
			AssertionException.IsNotNull(group);
			BaseLayoutItem firstOtherItem = GetFirstOtherItem(item, group.GetItems());
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item, group })) {
				using(new NotificationBatch(Container)) {
					using(new ActivateBatch(Container)) {
						using(new UpdateBatch(Container)) {
							if(calcDock) AddPlaceHolder(item, group);
							else ClearPlaceHolder(item);
							AutoHideType actualAutoHideType = AutoHideGroup.GetAutoHideType(item);
							if(group.Remove(item)) {
								BaseLayoutItem dockTarget = group;
								NotificationBatch.Action(Container, dockTarget.GetRoot());
								if(TryUngroupGroup(group) != group) dockTarget = firstOtherItem;
								item.UpdateDockSituation(savedSituation, dockTarget);
								if(!calcDock) item.UpdateAutoHideSituation(dock.ToAutoHideType());
								AutoHideType autoHideType = calcDock ? CalcAutoHideType(item, actualAutoHideType) : AutoHideType.Default;
								Dock dockTo = autoHideType != AutoHideType.Default ? autoHideType.ToDock() : savedSituation.DesiredDock;
								AutoHideGroup newGroup = target;
								if(newGroup != null && !newGroup.IsUngroupped)
									newGroup.Add(item);
								else {
									newGroup = DockControllerHelper.BoxIntoAutoHideGroup(item, dockTo, Container);
									newGroup.UpdateDockSituation(savedSituation, dockTarget, item);
									if(item is TabbedGroup) {
										LayoutGroup.SetOwnerGroup(item, newGroup);
										Container.DecomposedItems.Add((LayoutGroup)item);
										PlaceHolderHelper.AddFakePlaceHolderForItem((TabbedGroup)item, newGroup);
										newGroup.HasPersistentGroups = !((TabbedGroup)item).DestroyOnClosingChildren;
									}
									Container.AutoHideGroups.Add(newGroup);
								}
								DockControllerHelper.CheckUpdateView(Container, savedSituation.Root);
								DockControllerHelper.CheckUpdateView(Container, newGroup.GetRoot());
								RaiseItemEvent(item, DockLayoutManager.DockItemHiddenEvent);
								Container.Update();
								OnDockOperationComplete(item, DockOperation.Hide);
								NotificationBatch.Action(Container, newGroup);
								return true;
							}
						}
					}
				}
				return false;
			}
		}
		protected bool HideCore(BaseLayoutItem item, Dock dock, bool calcDock) {
			return HideCore(item, null, dock, calcDock);
		}
		AutoHideType CalcAutoHideType(BaseLayoutItem item, AutoHideType actualAutoHideType) {
			DockSituation last = item.GetLastDockSituation();
			return actualAutoHideType != AutoHideType.Default ? actualAutoHideType : (last != null ? last.AutoHideType : AutoHideType.Default);
		}
		bool RestoreToPlaceHolder(BaseLayoutItem item, bool isRestore = false) {
			PlaceHolder placeHolder = PlaceHolderHelper.GetPlaceHolderForDockOperation(item, isRestore);
			if(placeHolder == null) return false;
			var parent = placeHolder.Parent;
			bool restoreAsFloating = !parent.IsInTree() && placeHolder.IsFloating;
			bool res = restoreAsFloating ? RestoreToFloatingPlaceHolder(item, placeHolder) : RestoreToPlaceHolder(item, placeHolder);
			PlaceHolderHelper.ClearPlaceHolder(item, placeHolder);
			return res;
		}
		bool RestoreToPlaceHolder(BaseLayoutItem item, PlaceHolder placeHolder) {
			bool res;
			LayoutGroup targetGroup = PlaceHolderHelper.RestoreLayoutHierarchy(Container, item, placeHolder.DockState);
			int index = PlaceHolderHelper.GetDockIndex(item, placeHolder);
			res = DockInExistingGroup(item, targetGroup, index);
			if(res) {
				LayoutGroup itemRoot = item.GetRoot();
				AutoHideGroup ahRoot = itemRoot as AutoHideGroup;
				if(ahRoot != null && !Container.AutoHideGroups.Contains(ahRoot)) {
					Container.AutoHideGroups.Add(ahRoot);
					Container.DecomposedItems.Remove(ahRoot);
				}
				FloatGroup fRoot = itemRoot as FloatGroup;
				if(fRoot != null && !Container.FloatGroups.Contains(fRoot)) {
					Container.FloatGroups.Add(fRoot);
					Container.DecomposedItems.Remove(fRoot);
				}
				item.UpdateDockSituation(item.Parent, DockType.None);
			}
			return res;
		}
		bool RestoreToFloatingPlaceHolder(BaseLayoutItem item, PlaceHolder placeHolder) {
			bool res = true;
			LayoutGroup restoreRoot = PlaceHolderHelper.GetRestoreRoot(item, placeHolder.DockState);
			FloatGroup floatGroup = restoreRoot as FloatGroup;
			if(floatGroup == null) {
				floatGroup = Container.CreateFloatGroup();
				Container.DecomposedItems.Add(floatGroup);
			}
			if(!Container.FloatGroups.Contains(floatGroup) && Container.DecomposedItems.Contains(floatGroup)) {
				Container.DecomposedItems.Remove(floatGroup);
				LayoutPanel panel = (LayoutPanel)item;
				if(Container.FloatingDocumentContainer == FloatingDocumentContainer.DocumentHost && item is DocumentPanel) {
					item = DockControllerHelper.BoxIntoDocumentHost(item, Container);
				}
				((ISupportInitialize)floatGroup).BeginInit();
				floatGroup.Items.Add(item);
				floatGroup.FloatLocation = panel.FloatLocationBeforeClose;
				if(!MathHelper.IsEmpty(panel.FloatSizeBeforeClose)) floatGroup.FloatSize = panel.FloatSizeBeforeClose;
				((ISupportInitialize)floatGroup).EndInit();
				Container.CheckFloatGroupRestoreBounds(floatGroup, panel.FloatOffsetBeforeClose);
				Container.FloatGroups.Add(floatGroup);
			}
			else res = RestoreToPlaceHolder(item, placeHolder);
			return res;
		}
		public bool Restore(BaseLayoutItem item) {
			bool result = item.IsClosed && item is LayoutPanel && item.AllowRestore;
			if(!result) return false;
			if(RaiseItemCancelEvent(item, DockLayoutManager.DockItemRestoringEvent) || OnDockOperationStarting(DockOperation.Restore, item)) return false;
			using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { item })) {
				using(new NotificationBatch(Container)) {
					if(Container.ClosedPanels.Remove((LayoutPanel)item)) {
						if(!RestoreToPlaceHolder(item, true)) {
							DockSituation situation = item.GetLastDockSituation();
							bool canRestore = situation != null && CanRestoreDockSituation(situation) && situation.Type != DockType.None;
							if(canRestore) {
								DockType type = situation.Type;
								BaseLayoutItem dockTarget = situation.DockTarget;
								result = Dock(item, dockTarget, type);
							}
							else {
								FloatGroup floatGroup = BoxToFloatGroupCore(item);
								LayoutPanel panel = (LayoutPanel)item;
								floatGroup.FloatLocation = panel.FloatLocationBeforeClose;
								if(!MathHelper.IsEmpty(panel.FloatSizeBeforeClose))
									floatGroup.FloatSize = panel.FloatSizeBeforeClose;
								Container.CheckFloatGroupRestoreBounds(floatGroup, panel.FloatOffsetBeforeClose);
								Container.FloatGroups.Add(floatGroup);
								PlaceHolderHelper.ClearPlaceHolder(item, PlaceHolderState.Floating);
							}
						}
						RaiseItemEvent(item, DockLayoutManager.DockItemRestoredEvent);
						Container.Update();
						OnDockOperationComplete(item, DockOperation.Restore);
						NotificationBatch.Action(Container, item.GetRoot());
					}
					return result;
				}
			}
		}
		protected bool CanRestoreDockSituation(DockSituation situation) {
			if(situation == null || situation.DockTarget == null) return false;
			if(situation.Root == situation.DockTarget.GetRoot()) {
				if(situation.Root is FloatGroup) {
					return Container.FloatGroups.Contains((FloatGroup)situation.Root);
				}
				if(situation.Root is AutoHideGroup) {
					return Container.AutoHideGroups.Contains((AutoHideGroup)situation.Root);
				}
				return true;
			}
			return false;
		}
		public bool CreateNewDocumentGroup(DocumentPanel document, Orientation orientation) {
			return CreateNewDocumentGroup((LayoutPanel)document, orientation);
		}
		public bool CreateNewDocumentGroup(LayoutPanel document, Orientation orientation) {
			if(document == null || document.Parent == null || !(document.Parent is DocumentGroup) || document.Parent.Items.Count <= 1) return false;
			DocumentGroup parent = (DocumentGroup)document.Parent;
			using(new UpdateBatch(Container)) {
				using(new LogicalTreeLocker(Container, new BaseLayoutItem[] { document, parent })) {
					parent.Remove(document);
					DocumentGroup newGroup = DockControllerHelper.BoxIntoDocumentGroup(document, Container);
					if(parent.Parent != null) {
						LayoutGroup parentGroup = parent.Parent;
						int index = parentGroup.Items.IndexOf(parent);
						if(parentGroup.Orientation != orientation) {
							LayoutGroup resultGroup = Container.CreateLayoutGroup(orientation);
							parentGroup.Remove(parent);
							resultGroup.AddRange(new BaseLayoutItem[] { parent, newGroup });
							InsertItemCore(parentGroup, resultGroup, index);
						}
						else
							InsertItemCore(parentGroup, newGroup, index + 1);
					}
				}
				DockControllerHelper.CheckUpdateView(Container, document.GetRoot());
				Container.Update();
				return true;
			}
		}
		bool InsertItemCore(LayoutGroup group, BaseLayoutItem item, int insertIndex) {
			return InsertItemInGroup(group, item, insertIndex, true);
		}
		public bool MoveToDocumentGroup(DocumentPanel document, bool next) {
			return MoveToDocumentGroup((LayoutPanel)document, next);
		}
		public bool MoveToDocumentGroup(LayoutPanel document, bool next) {
			if(document == null) return false;
			DocumentGroup target = next ?
				DockControllerHelper.GetNextNotEmptyDocumentGroup(document.Parent) :
				DockControllerHelper.GetPreviousNotEmptyDocumentGroup(document.Parent);
			if(target == null) return false;
			Dock(document, target, DockType.Fill);
			DockControllerHelper.CheckUpdateView(Container, document.GetRoot());
			Container.Update();
			return true;
		}
		#region ILockOwner Members
		void ILockOwner.Lock() {
			LockActivate();
		}
		void ILockOwner.Unlock() {
			UnlockActivate();
		}
		#endregion
	}
	public class ActiveItemHelper : IDisposable {
		IActiveItemOwner Owner;
		public ActiveItemHelper(IActiveItemOwner owner) {
			Owner = owner;
		}
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Owner = null;
			}
			GC.SuppressFinalize(this);
		}
		public void ActivateItem(BaseLayoutItem item, bool focus) {
			if(item is LayoutGroup)
				item = GetFirstItemInGroup(item as LayoutGroup);
			Owner.ActiveItem = item;
			if (Bars.PopupMenuManager.TopPopup != null)
				focus = false;
			if(item is LayoutPanel)
				FocusPanelItem((LayoutPanel)item, focus);
			else
				FocusLayoutItem(item, focus);
		}
		public void SelectInGroup(BaseLayoutItem item) {
			if(!item.IsActive) return;
			if(item is LayoutGroup)
				item = GetFirstItemInGroup(item as LayoutGroup);
			if(item != null)
				SelectInGroup(item, item.Parent);
		}
		public void SelectInGroup(BaseLayoutItem item, LayoutGroup group) {
			if(group == null) return;
			DockLayoutManager container = Owner != null ? Owner.Container : null;
			using(new LogicalTreeLocker(container, new BaseLayoutItem[] { group.Parent ?? group })) {
				TrySelectTabPage(item, group);
				TrySelectAutoHidePage(item, group);
			}
		}
		void TrySelectAutoHidePage(BaseLayoutItem item, LayoutGroup group) {
			AutoHideGroup aGroup = group as AutoHideGroup;
			if(aGroup != null) {
				int selectedIndex = aGroup.Items.IndexOf(item);
				if(selectedIndex != -1) {
					aGroup.SelectedTabIndex = selectedIndex;
					AutoHideTrayHeadersGroup headersGroup = LayoutItemsHelper.GetVisualChild<AutoHideTrayHeadersGroup>(group);
					AutoHideTray tray = headersGroup != null ? headersGroup.Tray : null;
					if(tray != null && (!tray.IsExpanded || tray.HotItem != item)) tray.DoExpand(item);
				}
			}
		}
		void TrySelectTabPage(BaseLayoutItem item, LayoutGroup group) {
			int selectedIndex = group.Items.IndexOf(item);
			if(group.IsTabHost && selectedIndex != -1) {
				group.SelectedTabIndex = selectedIndex;
			}
			while(group != null) {
				if(group.GroupBorderStyle == GroupBorderStyle.Tabbed)
					group.SelectedTabIndex = group.Items.IndexOf(item);
				item = group;
				group = group.Parent;
			}
		}
		BaseLayoutItem GetFirstItemInGroup(LayoutGroup group) {
			BaseLayoutItem item = group;
			while(group != null && group.Items.Count > 0) {
				if(group.Items[0] is LayoutGroup && group.GroupBorderStyle != GroupBorderStyle.Tabbed) {
					group = group.Items[0] as LayoutGroup;
					continue;
				}
				item = group.Items[0];
				if(group is TabbedGroup || group.GroupBorderStyle == GroupBorderStyle.Tabbed)
					item = group.SelectedItem;
				break;
			}
			return item;
		}
		void FocusLayoutItem(BaseLayoutItem item, bool focus) {
			if(item != null) {
				SelectInGroup(item, item.Parent);
				if(focus && item is LayoutControlItem && ((LayoutControlItem)item).FocusContentOnActivating && !Owner.Container.IsCustomization)
					KeyboardFocusHelper.FocusElement(((LayoutControlItem)item).Control);
			}
		}
		void FocusPanelItem(LayoutPanel panelItem, bool focus) {
			if(panelItem != null) {
				SelectInGroup(panelItem, panelItem.Parent);
				if(!focus) return;
				focus = panelItem.FocusContentOnActivating;
				if(!panelItem.IsControlItemsHost) {
					if(focus)
						KeyboardFocusHelper.FocusElement(panelItem.Control ?? panelItem.ContentPresenter, Owner.Container.IsFloating);
				}
				else {
					FocusLayoutItem(GetFirstItemInGroup(panelItem.Layout), focus);
				}
			}
		}
		internal void FocusPanelItem(BaseLayoutItem item) {
			FocusPanelItem(item as LayoutPanel, true);
		}
		internal void RestoreKeyboardFocus(BaseLayoutItem item) {
			KeyboardFocusHelper.RestoreKeyboardFocus(item);
		}
	}
	public static class DockControllerHelper {
		public static readonly DependencyProperty ClosingBehaviorProperty =
			DependencyProperty.RegisterAttached("ClosingBehavior", typeof(ClosingBehavior), typeof(DockControllerHelper), new PropertyMetadata(ClosingBehavior.Default));
		public static ClosingBehavior GetClosingBehavior(DependencyObject obj) {
			return (ClosingBehavior)obj.GetValue(ClosingBehaviorProperty);
		}
		public static void SetClosingBehavior(DependencyObject obj, ClosingBehavior value) {
			obj.SetValue(ClosingBehaviorProperty, value);
		}
		public static bool InsertItemInGroup(LayoutGroup group, BaseLayoutItem item, int index, bool forceSizeUpdate = false) {
			bool result = (item.Parent == null) || item.Parent.Remove(item);
			if(result) {
				bool fHorz = !group.IgnoreOrientation && group.Orientation == Orientation.Horizontal;
				GridLength length = new GridLength(DockPreviewCalculator.GetStarDockSize(group), GridUnitType.Star);
				DockSituation situation = item.GetLastDockSituation();
				if(situation != null) {
					if(situation.Width.IsAbsolute)
						item.ItemWidth = situation.Width;
					if(situation.Height.IsAbsolute)
						item.ItemHeight = situation.Height;
					if(situation.DockTarget == group && situation.TheSameLengths(group)) {
						length = fHorz ? situation.Width : situation.Height;
						forceSizeUpdate = true;
					}
				}
				GridLength itemLength = fHorz ? item.ItemWidth : item.ItemHeight;
				if(!itemLength.IsAuto) {
					if(!itemLength.IsAbsolute) {
						if(forceSizeUpdate)
							item.SetValue(fHorz ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty, length);
					}
					else {
						GridLength groupLength = fHorz ? group.ItemWidth : group.ItemHeight;
						if(groupLength.IsAbsolute && groupLength.Value < itemLength.Value * 1.5) {
							groupLength = new GridLength(groupLength.Value + itemLength.Value, GridUnitType.Pixel);
							group.SetValue(fHorz ? BaseLayoutItem.ItemWidthProperty : BaseLayoutItem.ItemHeightProperty, groupLength);
						}
					}
				}
				group.Items.Insert(Math.Min(index, group.Items.Count), item);
			}
			return result;
		}
		public static bool CheckHideView(DockLayoutManager container, LayoutGroup group) {
			if(container == null) return false;
			bool canHide = (group is AutoHideGroup);
			if(canHide) container.HideView(group);
			return canHide;
		}
		public static bool CheckUpdateView(DockLayoutManager container, LayoutGroup group) {
			if(container == null) return false;
			bool canUpdate = (group is FloatGroup || group is AutoHideGroup);
			if(canUpdate) container.InvalidateView(group);
			return canUpdate;
		}
		public static LayoutGroup Unbox(DockLayoutManager container, LayoutGroup group) {
			LayoutGroup behindGroup = group.Parent;
			if(behindGroup == null) {
				if(group.Items.Count == 1 && !group.IsLayoutRoot) {
					LayoutGroup nestedGroup = group[0] as LayoutGroup;
					bool canUnbox = CanUnbox(group, nestedGroup);
					if(canUnbox) {
						if(group.Orientation != nestedGroup.Orientation)
							group.Orientation = nestedGroup.Orientation;
						UnboxCore(nestedGroup, group, nestedGroup.GetItems());
					}
				}
				return group;
			}
			BaseLayoutItem[] nestedItems = group.GetItems();
			if(behindGroup.IgnoreOrientation || behindGroup.Orientation == @group.Orientation || nestedItems.Length == 1) {
				UnboxCore(group, behindGroup, nestedItems);
				CheckUpdateView(container, behindGroup);
			}
			return behindGroup;
		}
		static bool CanUnbox(LayoutGroup group, LayoutGroup nestedGroup) {
			if(nestedGroup == null) return false;
			bool skipUnbox = !nestedGroup.DestroyOnClosingChildren || nestedGroup.IsControlItemsHost || nestedGroup is TabbedGroup || nestedGroup.IsDockingTarget;
			return !skipUnbox;
		}
		static void UnboxCore(LayoutGroup group, LayoutGroup behindGroup, BaseLayoutItem[] nestedItems) {
			int index = behindGroup.Items.IndexOf(group);
			for(int i = 0; i < nestedItems.Length; i++) {
				group.Remove(nestedItems[i]);
			}
			behindGroup.Items.Remove(group);
			group.IsUngroupped = true;
			for(int i = 0; i < nestedItems.Length; i++) {
				behindGroup.Insert(i + index, nestedItems[i]);
			}
			if(behindGroup is FloatGroup && behindGroup.HasSingleItem)
				UpdateFloatGroup(behindGroup[0], (FloatGroup)behindGroup);
		}
		static void UpdateFloatGroup(BaseLayoutItem item, FloatGroup floatGroup) {
			ClearSize(item);
		}
		public static BaseLayoutItem[] Decompose(BaseLayoutItem item) {
			List<BaseLayoutItem> panels = new List<BaseLayoutItem>();
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			item.Accept(
				delegate (BaseLayoutItem i) {
					items.Add(i);
					if(i is LayoutPanel)
						panels.Add(i);
				});
			foreach(BaseLayoutItem node in items) {
				if(node.Parent != null)
					node.Parent.Remove(node);
			}
			return panels.ToArray();
		}
		public static IEnumerable<BaseLayoutItem> GetAffectedItems(BaseLayoutItem item) {
			List<BaseLayoutItem> panels = new List<BaseLayoutItem>();
			item.Accept(
					delegate(BaseLayoutItem i) {
						if(i is LayoutPanel) panels.Add(i);
					}
				);
			return panels;
		}
		static void ClearSize(BaseLayoutItem item, bool force = false) {
			if(force) {
				if(!item.ItemWidth.IsStar)
					item.ClearValue(BaseLayoutItem.ItemWidthProperty);
				if(!item.ItemHeight.IsStar)
					item.ClearValue(BaseLayoutItem.ItemHeightProperty);
			}
		}
		public static LayoutGroup BoxIntoGroup(BaseLayoutItem[] items, Orientation orientation) {
			LayoutGroup group = new LayoutGroup();
			((ISupportInitialize)group).BeginInit();
			group.GroupBorderStyle = GroupBorderStyle.Group;
			group.Orientation = orientation;
			((ISupportInitialize)group).EndInit();
			BoxIntoGroupCore(items, group);
			return group;
		}
		static void BoxIntoGroupCore(BaseLayoutItem[] items, LayoutGroup group) {
			group.ItemHeight = items[0].ItemHeight;
			group.ItemWidth = items[0].ItemWidth;
			group.AddRange(items);
		}
		static void BoxIntoGroupCore(BaseLayoutItem item, LayoutGroup group) {
			group.ItemWidth = item.ItemWidth;
			group.ItemHeight = item.ItemHeight;
			ClearSize(item);
			group.Items.Add(item);
		}
		static void DecomposeAndBoxIntoGroup(BaseLayoutItem item, LayoutGroup group) {
			group.ItemWidth = item.ItemWidth;
			group.ItemHeight = item.ItemHeight;
			ClearSize(item);
			group.AddRange(Decompose(item));
		}
		public static LayoutGroup BoxIntoGroup(BaseLayoutItem item, Orientation orientation, DockLayoutManager manager) {
			LayoutGroup group = manager.CreateLayoutGroup();
			((ISupportInitialize)group).BeginInit();
			group.Orientation = orientation;
			((ISupportInitialize)group).EndInit();
			BoxIntoGroupCore(item, group);
			return group;
		}
		public static LayoutGroup BoxIntoGroup(BaseLayoutItem item, Orientation orientation, bool allowSplitters, DockLayoutManager manager) {
			LayoutGroup group = BoxIntoGroup(item, orientation, manager);
			if(!DesignerProperties.GetIsInDesignMode(group))
				group.AllowSplitters = allowSplitters;
			return group;
		}
		public static FloatGroup BoxIntoFloatGroup(BaseLayoutItem item, DockLayoutManager manager) {
			FloatGroup floatGroup = manager.CreateFloatGroup();
			((ISupportInitialize)floatGroup).BeginInit();
			floatGroup.Items.Add(item);
			var floatSize = MathHelper.IsEmpty(item.FloatSize) ? item.GetSize() : item.FloatSize;
			floatGroup.FitSizeToContent(floatSize);
			((ISupportInitialize)floatGroup).EndInit();
			ClearSize(item, true);
			return floatGroup;
		}
		public static TabbedGroup BoxIntoTabbedGroup(BaseLayoutItem item, DockLayoutManager manager) {
			TabbedGroup tabbedGroup = manager.CreateTabbedGroup();
			((ISupportInitialize)tabbedGroup).BeginInit();
			BoxIntoGroupCore(item, tabbedGroup);
			((ISupportInitialize)tabbedGroup).EndInit();
			return tabbedGroup;
		}
		public static LayoutGroup BoxIntoDocumentHost(BaseLayoutItem item, DockLayoutManager manager) {
			LayoutGroup host = manager.CreateLayoutGroup();
			DocumentGroup documentGroup = manager.CreateDocumentGroup();
			((ISupportInitialize)host).BeginInit();
			((ISupportInitialize)documentGroup).BeginInit();
			documentGroup.IsAutoGenerated = true;
			DecomposeAndBoxIntoGroup(item, documentGroup);
			object lastShowMode = item.GetValue(DocumentGroup.LastClosePageButtonShowModeInternalProperty);
			if(!object.Equals(lastShowMode, ClosePageButtonShowMode.Default))
				documentGroup.ClosePageButtonShowMode = (ClosePageButtonShowMode)lastShowMode;
			host.Add(documentGroup);
			((ISupportInitialize)documentGroup).EndInit();
			((ISupportInitialize)host).EndInit();
			return host;
		}
		public static DocumentGroup BoxIntoDocumentGroup(BaseLayoutItem item, DockLayoutManager manager) {
			DocumentGroup documentGroup = manager.CreateDocumentGroup();
			((ISupportInitialize)documentGroup).BeginInit();
			documentGroup.IsAutoGenerated = true;
			DecomposeAndBoxIntoGroup(item, documentGroup);
			object lastShowMode = item.GetValue(DocumentGroup.LastClosePageButtonShowModeInternalProperty);
			if(!object.Equals(lastShowMode, ClosePageButtonShowMode.Default))
				documentGroup.ClosePageButtonShowMode = (ClosePageButtonShowMode)lastShowMode;
			((ISupportInitialize)documentGroup).EndInit();
			return documentGroup;
		}
		public static AutoHideGroup BoxIntoAutoHideGroup(BaseLayoutItem item, Dock type, DockLayoutManager manager) {
			AutoHideGroup autoHideGroup = manager.CreateAutoHideGroup();
			((ISupportInitialize)autoHideGroup).BeginInit();
			autoHideGroup.DockType = type;
			autoHideGroup.Items.Add(item);
			if(item is LayoutGroup)
				AutoHideGroup.SetAutoHideType(autoHideGroup, AutoHideGroup.GetAutoHideType(item));
			ClearSize(item);
			((ISupportInitialize)autoHideGroup).EndInit();
			return autoHideGroup;
		}
		static bool IsRootSingleChild(BaseLayoutItem item) {
			DockSituation ds = item.GetLastDockSituation();
			if(ds != null) {
				LayoutGroup group = ds.DockTarget as LayoutGroup;
				if(group != null && group.Parent == null)
					return group.Items.Count == 0;
			}
			return false;
		}
		public static T CreateCommand<T>(DockLayoutManager container, BaseLayoutItem item) where T : DockControllerCommand, new() {
			IDockController controller = GetDockController(container);
			return controller != null ? controller.CreateCommand<T>(item) : null;
		}
		static IDockController GetDockController(DockLayoutManager container) {
			return container != null ? container.DockController : null;
		}
		public static DockType GetDockTypeInContainer(DockLayoutManager container, BaseLayoutItem item) {
			LayoutGroup root = item.GetRoot();
			if(root is AutoHideGroup) {
				return ((AutoHideGroup)root).DockType.ToDockType();
			}
			if(root is FloatGroup) {
				Rect containerRect = CoordinateHelper.GetContainerRect(container);
				Point center = CoordinateHelper.GetCenter(
						new Rect(((FloatGroup)root).FloatLocation, ((FloatGroup)root).FloatSize)
					);
				return DropTypeHelper.CalcCenterDropInfo(containerRect, center).DockType;
			}
			else return DockType.Fill;
		}
		public static DocumentGroup GetPreviousNotEmptyDocumentGroup(BaseLayoutItem item) {
			if(item != null && item.Parent != null) {
				LayoutGroup parent = item.Parent;
				int index = parent.Items.IndexOf(item);
				for(int i = index - 1; i >= 0; i--) {
					DocumentGroup group = parent.Items[i] as DocumentGroup;
					if(group != null && group.Items.Count > 0) return group;
				}
			}
			return null;
		}
		public static DocumentGroup GetNextNotEmptyDocumentGroup(BaseLayoutItem item) {
			if(item != null && item.Parent != null) {
				LayoutGroup parent = item.Parent;
				int index = parent.Items.IndexOf(item);
				for(int i = index + 1; i < parent.Items.Count; i++) {
					DocumentGroup group = parent.Items[i] as DocumentGroup;
					if(group != null && group.Items.Count > 0) return group;
				}
			}
			return null;
		}
		internal static ClosingBehavior GetActualClosingBehavior(DockLayoutManager container, BaseLayoutItem item) {
			ClosingBehavior value = GetClosingBehavior(item);
			if(value == ClosingBehavior.Default) {
				value = new HierarchyPropertyValue<ClosingBehavior>(BaseLayoutItem.ClosingBehaviorProperty, ClosingBehavior.Default).
					Get(item, item.ClosingBehavior);
			}
			return value != ClosingBehavior.Default || container == null ? value : container.ClosingBehavior;
		}
	}
	static class DockControllerExtension {
		public static bool CloseEx(this IDockController controller, BaseLayoutItem item) {
			if(item == null) return false;
			return item.ExecuteCloseCommand() || controller.Close(item);
		}
		public static bool DockAsDocument(this IDockController2010 controller, BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			return controller.DockAsDocument(item, target, type);
		}
		public static bool DockAsDocument(this IDockController controller, BaseLayoutItem item, BaseLayoutItem target, DockType type) {
			return controller is IDockController2010 ? DockAsDocument((IDockController2010)controller, item, target, type) :
				controller.Dock(item, target, type);
		}
		public static bool Insert(this IDockController controller, LayoutGroup group, BaseLayoutItem item, int index, bool isReordering) {
			if(isReordering && (item.Parent.TabIndexFromItem(item) == index || index < 0)) return false;
			DockOperation dockOperation = isReordering ? DockOperation.Reorder : DockOperation.Dock;
			if(controller.Container.RaiseDockOperationStartingEvent(dockOperation, item, group)) return false;
			bool res = controller.Insert(group, item, index);
			if(res) controller.Container.RaiseDockOperationCompletedEvent(dockOperation, item);
			return res;
		}
	}
	#region Commands
	public abstract class DockControllerCommand : ICommand {
		internal DockControllerCommand() { }
		protected abstract void ExecuteCore(IDockController controller, BaseLayoutItem item);
		protected abstract bool CanExecuteCore(BaseLayoutItem item);
		protected internal IDockController Controller { get; set; }
		protected internal BaseLayoutItem Item { get; set; }
		#region ICommand
		event EventHandler CanExecuteChangedCore;
		event EventHandler ICommand.CanExecuteChanged {
			add { CanExecuteChangedCore += value; }
			remove { CanExecuteChangedCore -= value; }
		}
		protected void RaiseCanExecuteChanged() {
			if(CanExecuteChangedCore != null)
				CanExecuteChangedCore(this, EventArgs.Empty);
		}
		bool ICommand.CanExecute(object parameter) {
			if(Controller == null || Item == null) return false;
			return CanExecuteCore(Item);
		}
		void ICommand.Execute(object parameter) {
			if(Controller == null) return;
			ExecuteCore(Controller, Item);
		}
		#endregion ICommand
		#region static
		static DockControllerCommand() {
			Activate = new DockControllerCommandLink("Activate", new ActivateCommand());
			Close = new DockControllerCommandLink("Close", new CloseCommand());
			Dock = new DockControllerCommandLink("Dock", new DockCommand());
			Float = new DockControllerCommandLink("Float", new FloatCommand());
			Hide = new DockControllerCommandLink("Hide", new HideCommand());
			Restore = new DockControllerCommandLink("Restore", new RestoreCommand());
			CloseActive = new DockControllerCommandLink("CloseActive", new CloseActiveCommand());
		}
		public static RoutedCommand Activate { get; private set; }
		public static RoutedCommand Close { get; private set; }
		public static RoutedCommand Dock { get; private set; }
		public static RoutedCommand Float { get; private set; }
		public static RoutedCommand Hide { get; private set; }
		public static RoutedCommand Restore { get; private set; }
		internal static RoutedCommand CloseActive { get; private set; }
		internal static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			IDockController controller = GetDockController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			BaseLayoutItem item = e.Parameter as BaseLayoutItem;
			DockControllerCommand command = ((DockControllerCommandLink)e.Command).Command;
			e.CanExecute = controller != null && item != null && command.CanExecuteCore(item);
		}
		internal static void Executed(object sender, ExecutedRoutedEventArgs e) {
			IDockController controller = GetDockController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			BaseLayoutItem item = e.Parameter as BaseLayoutItem;
			if(controller != null && item != null) {
				DockControllerCommand command = ((DockControllerCommandLink)e.Command).Command;
				command.ExecuteCore(controller, item);
			}
		}
		static IDockController GetDockController(DockLayoutManager container) {
			return container != null ? container.DockController : null;
		}
		#endregion static
		class DockControllerCommandLink : RoutedCommand {
			public DockControllerCommandLink(string name, DockControllerCommand command) :
				base(name, typeof(DockControllerCommand)) {
				Command = command;
			}
			public DockControllerCommand Command { get; private set; }
		}
	}
	public class ActivateCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.AllowActivate;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.Activate(item);
		}
	}
	public class CloseCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return !item.IsClosed && item.AllowClose;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.CloseEx(item);
		}
	}
	class CloseActiveCommand : CloseCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return base.CanExecuteCore(item) && item.IsActive;
		}
	}
	public class CloseAllButThisCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return !item.IsClosed && item.Parent != null && item.Parent.Items.Count > 1;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.CloseAllButThis(item);
		}
	}
	public class DockCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.IsFloating || item.IsAutoHidden && item.AllowDock;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			if(CanDock(controller.Container, item))
				controller.Dock(item);
		}
		protected bool CanDock(DockLayoutManager container, BaseLayoutItem item) {
			DockSituation situation = item.GetLastDockSituation();
			DockType type = DockControllerHelper.GetDockTypeInContainer(container, item);
			BaseLayoutItem target = container.LayoutRoot;
			if(situation != null && situation.DockTarget != null) {
				LayoutGroup targetRoot = situation.DockTarget.GetRoot();
				bool fValidTarget = (situation.Root == targetRoot) && container.IsViewCreated(targetRoot);
				LayoutGroup root = item.GetRoot();
				if((targetRoot != root) && fValidTarget) {
					type = situation.Type;
					target = situation.DockTarget;
				}
			}
			return !container.RaiseItemDockingEvent(DockLayoutManager.DockItemStartDockingEvent, item,
				CoordinateHelper.ZeroPoint, target, type, false);
		}
	}
	public class FloatCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return !item.IsFloating && item.AllowFloat;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			DockLayoutManager container = controller.Container;
			var view = container.GetView(item.GetRoot());
			var viewElement = container.GetViewElement(item);
			if(container != null && view != null && viewElement != null)
				container.ViewAdapter.ContextActionService.DoContextAction(view, viewElement, ContextAction.Float);
			else controller.Float(item);
		}
	}
	public class HideCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return !item.IsAutoHidden && item.AllowHide;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			if(item.IsFloating && controller.Container.RaiseItemDockingEvent(DockLayoutManager.DockItemStartDockingEvent, item,
				CoordinateHelper.ZeroPoint, null, DockType.None, true)) return;
			controller.Hide(item);
		}
	}
	public class RestoreCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.IsClosed && item.AllowRestore;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.Restore(item);
		}
	}
	public class ExpandCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			return group != null && group.AllowExpand;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			LayoutGroup groupToExpandCollapse = item as LayoutGroup;
			if(groupToExpandCollapse != null) {
				groupToExpandCollapse.Expanded = !groupToExpandCollapse.Expanded;
				LayoutView view = controller.Container.GetView(groupToExpandCollapse) as LayoutView;
				if(view != null) view.AdornerHelper.InvalidateDockingHintsAdorner();
			}
		}
	}
	public class NewHorizontalDocumentGroupCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.Parent != null && item.Parent is DocumentGroup && item.Parent.Items.Count > 1;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.CreateNewDocumentGroup(item as LayoutPanel, Orientation.Horizontal);
		}
	}
	public class NewVerticalDocumentGroupCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.Parent != null && item.Parent is DocumentGroup && item.Parent.Items.Count > 1;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.CreateNewDocumentGroup(item as LayoutPanel, Orientation.Vertical);
		}
	}
	public class MoveToPreviousDocumentGroupCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.Parent != null && item.Parent is DocumentGroup;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.MoveToDocumentGroup(item as LayoutPanel, false);
		}
	}
	public class MoveToNextDocumentGroupCommand : DockControllerCommand {
		protected override bool CanExecuteCore(BaseLayoutItem item) {
			return item.Parent != null && item.Parent is DocumentGroup; ;
		}
		protected override void ExecuteCore(IDockController controller, BaseLayoutItem item) {
			controller.MoveToDocumentGroup(item as LayoutPanel, true);
		}
	}
	#endregion Commands
}
