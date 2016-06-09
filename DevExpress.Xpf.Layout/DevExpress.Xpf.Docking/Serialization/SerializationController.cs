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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking.Internal;
using SWRoutedEventArgs = System.Windows.RoutedEventArgs;
namespace DevExpress.Xpf.Docking {
	public class RestoreLayoutOptions {
		public static readonly DependencyProperty AddNewPanelsProperty;
		public static readonly DependencyProperty RemoveOldPanelsProperty;
		public static readonly DependencyProperty AddNewLayoutControlItemsProperty;
		public static readonly DependencyProperty RemoveOldLayoutControlItemsProperty;
		public static readonly DependencyProperty AddNewLayoutGroupsProperty;
		public static readonly DependencyProperty RemoveOldLayoutGroupsProperty;
		public static readonly DependencyProperty FloatPanelsRestoreOffsetProperty;
		public static readonly DependencyProperty DockLayoutManagerRestoreOffsetProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty AsyncRestoreLayoutProperty;
		static RestoreLayoutOptions() {
			var dProp = new DependencyPropertyRegistrator<RestoreLayoutOptions>();
			dProp.RegisterAttached<bool>("AddNewPanels", ref AddNewPanelsProperty, true);
			dProp.RegisterAttached<bool>("RemoveOldPanels", ref RemoveOldPanelsProperty, true);
			dProp.RegisterAttached<bool>("AddNewLayoutControlItems", ref AddNewLayoutControlItemsProperty, true);
			dProp.RegisterAttached<bool>("RemoveOldLayoutControlItems", ref RemoveOldLayoutControlItemsProperty, true);
			dProp.RegisterAttached<bool>("AddNewLayoutGroups", ref AddNewLayoutGroupsProperty, false);
			dProp.RegisterAttached<bool>("RemoveOldLayoutGroups", ref RemoveOldLayoutGroupsProperty, false);
			dProp.RegisterAttached<Point>("FloatPanelsRestoreOffset", ref FloatPanelsRestoreOffsetProperty, new Point(double.NaN, double.NaN));
			dProp.RegisterAttached<Point>("DockLayoutManagerRestoreOffset", ref DockLayoutManagerRestoreOffsetProperty, new Point(double.NaN, double.NaN));
			dProp.RegisterAttached("AsyncRestoreLayout", ref AsyncRestoreLayoutProperty, false);
		}
		public static bool GetAddNewPanels(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewPanelsProperty);
		}
		public static void SetAddNewPanels(DependencyObject obj, bool value) {
			obj.SetValue(AddNewPanelsProperty, value);
		}
		public static bool GetRemoveOldPanels(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldPanelsProperty);
		}
		public static void SetRemoveOldPanels(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldPanelsProperty, value);
		}
		public static bool GetAddNewLayoutControlItems(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewLayoutControlItemsProperty);
		}
		public static void SetAddNewLayoutControlItems(DependencyObject obj, bool value) {
			obj.SetValue(AddNewLayoutControlItemsProperty, value);
		}
		public static bool GetRemoveOldLayoutControlItems(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldLayoutControlItemsProperty);
		}
		public static void SetRemoveOldLayoutControlItems(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldLayoutControlItemsProperty, value);
		}
		public static bool GetAddNewLayoutGroups(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewLayoutGroupsProperty);
		}
		public static void SetAddNewLayoutGroups(DependencyObject obj, bool value) {
			obj.SetValue(AddNewLayoutGroupsProperty, value);
		}
		public static bool GetRemoveOldLayoutGroups(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldLayoutGroupsProperty);
		}
		public static void SetRemoveOldLayoutGroups(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldLayoutGroupsProperty, value);
		}
		[DevExpress.Utils.Serializing.XtraSerializableProperty]
		public static Point GetFloatPanelsRestoreOffset(DependencyObject obj) {
			return (Point)obj.GetValue(FloatPanelsRestoreOffsetProperty);
		}
		public static void SetFloatPanelsRestoreOffset(DependencyObject obj, Point value) {
			obj.SetValue(FloatPanelsRestoreOffsetProperty, value);
		}
		[DevExpress.Utils.Serializing.XtraSerializableProperty]
		public static Point GetDockLayoutManagerRestoreOffset(DependencyObject obj) {
			return (Point)obj.GetValue(DockLayoutManagerRestoreOffsetProperty);
		}
		public static void SetDockLayoutManagerRestoreOffset(DependencyObject obj, Point value) {
			obj.SetValue(DockLayoutManagerRestoreOffsetProperty, value);
		}
		internal static bool GetAsyncRestoreLayout(DependencyObject dObj) {
			return (bool)dObj.GetValue(AsyncRestoreLayoutProperty);
		}
		internal static void SetAsyncRestoreLayout(DependencyObject obj, bool value) {
			obj.SetValue(AsyncRestoreLayoutProperty, value);
		}
	}
	public class SerializationController : ISerializationController {
		bool isDisposingCore = false;
		DockLayoutManager containerCore;
		public const string SpecialNameSignature = "$";
		public const string HiddenItemsSignature = SpecialNameSignature + "HiddenItems";
		public const string ClosedPanelsSignature = SpecialNameSignature + "ClosedPanels";
		public const string FloatGroupsSignature = SpecialNameSignature + "FloatGroups";
		public const string AutoHideGroupsSignature = SpecialNameSignature + "AutoHideGroups";
		public const string DecomposedItemsSignature = SpecialNameSignature + "DecomposedItems";
		public SerializationController(DockLayoutManager container) {
			this.containerCore = container;
			SubscribeEvents();
		}
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				this.isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		void SubscribeEvents() {
			DXSerializer.AddStartSerializingHandler(Container, OnStartSerializing);
			DXSerializer.AddEndSerializingHandler(Container, OnEndSerializing);
			DXSerializer.AddStartDeserializingHandler(Container, OnStartDeserializing);
			DXSerializer.AddEndDeserializingHandler(Container, OnEndDeserializing);
		}
		void UnSubscribeEvents() {
			DXSerializer.RemoveStartSerializingHandler(Container, OnStartSerializing);
			DXSerializer.RemoveEndSerializingHandler(Container, OnEndSerializing);
			DXSerializer.RemoveStartDeserializingHandler(Container, OnStartDeserializing);
			DXSerializer.RemoveEndDeserializingHandler(Container, OnEndDeserializing);
		}
		protected bool IsDisposing {
			get { return isDisposingCore; }
		}
		int startDeserializing = 0;
		public bool IsDeserializing {
			get { return startDeserializing > 0; }
		}
		protected void OnDisposing() {
			UnSubscribeEvents();
			ResetSerializableItems();
			containerCore = null;
		}
		public DockLayoutManager Container {
			get { return containerCore; }
		}
		protected virtual string GetAppName() {
			return "DockLayoutManager";
		}
		#region Save/Restore
		public void SaveLayout(object path) {
			DXSerializer.SerializeSingleObject(Container, path, GetAppName());
		}
		public void RestoreLayout(object path) {
			DXSerializer.DeserializeSingleObject(Container, path, GetAppName());
		}
		protected virtual void OnStartSerializing(object sender, RoutedEventArgs e) {
			BeginSaveLayout();
		}
		protected virtual void OnEndSerializing(object sender, RoutedEventArgs e) {
			EndSaveLayout();
		}
		protected virtual void OnStartDeserializing(object sender, StartDeserializingEventArgs e) {
			BeginRestoreLayout();
		}
		protected virtual void OnEndDeserializing(object sender, EndDeserializingEventArgs e) {
			EndRestoreLayout();
		}
		internal void BeginSaveLayout() {
			CollectSerializableItems();
			SaveFloatPanelsRestoreOffset();
		}
		internal void EndSaveLayout() {
			ResetSerializableItems();
		}
		internal void BeginRestoreLayout() {
			startDeserializing++;
			Container.PrepareLayoutForModification();
			CollectSerializableItems();
			Items.Clear();
		}
		DevExpress.Xpf.Core.Locker restoreLocker = new DevExpress.Xpf.Core.Locker();
		System.Windows.Threading.DispatcherOperation restoreOperation = null;
		internal void EndRestoreLayout() {
			restoreLocker.Lock();
			if(RestoreLayoutOptions.GetAsyncRestoreLayout(Container))
				InvokeEndRestoreLayout();
			else
				EndRestoreLayoutCore();
		}
		void InvokeEndRestoreLayout() {
			Action restoreAction = new Action(EndRestoreLayoutCore);
			if(restoreOperation != null) restoreOperation.Abort();
			restoreOperation = Container.Dispatcher.BeginInvoke(restoreAction);
		}
		private void EndRestoreLayoutCore() {
			if(!restoreLocker.IsLocked || IsDisposing) return;
			restoreLocker.Unlock();
			using(new LogicalTreeLocker(Container, Container.GetItems())) {
				RestoreItems();
			}
			startDeserializing--;
			UpdateContainer();
			ResetSerializableItems();
		}
		void UpdateContainer() {
			Container.OnLayoutRestored();
			RestoreSelectedTabs(TabIndexes);
			Container.InvalidateVisual();
		}
		void SaveFloatPanelsRestoreOffset() {
			if(Container.GetRealFloatingMode() == Core.FloatingMode.Window)
				SaveFloatPanelsRestoreOffsetCore();
		}
		void SaveFloatPanelsRestoreOffsetCore() {
			Point offset = Container.GetRestoreOffset();
			RestoreLayoutOptions.SetFloatPanelsRestoreOffset(Container, offset);
		}
		#endregion Save/Restore
		public SerializableItemCollection Items { get; set; }
		protected void CollectSerializableItems() {
			Items = new SerializableItemCollection();
			if(Container.LayoutRoot != null)
				Container.LayoutRoot.Accept(GetPrepareParentCollectionName<BaseLayoutItem>(null));
			Container.ClosedPanels.Accept(GetPrepareParentCollectionName<LayoutPanel>(ClosedPanelsSignature));
			Container.LayoutController.HiddenItems.Accept(GetPrepareHiddenItem(HiddenItemsSignature));
			Container.FloatGroups.Accept(GetPrepareGroupItems<FloatGroup>(FloatGroupsSignature));
			Container.AutoHideGroups.Accept(GetPrepareGroupItems<AutoHideGroup>(AutoHideGroupsSignature));
			Container.DecomposedItems.Accept(GetPrepareDecomposedItem(DecomposedItemsSignature));
			PrepareSerializableItems();
		}
		protected void ResetSerializableItems() {
			Items = null;
			NamedItems = null;
			NamedControls = null;
			NewItems = null;
			TabIndexes = null;
		}
		VisitDelegate<T> GetPrepareGroupItems<T>(string collectionName) where T : LayoutGroup {
			var visit = GetPrepareParentCollectionName<BaseLayoutItem>(collectionName);
			return (group) => group.Accept(visit);
		}
		VisitDelegate<BaseLayoutItem> GetPrepareHiddenItem(string collectionName) {
			return delegate(BaseLayoutItem item) {
				item.Accept(GetPrepareParentCollectionName<BaseLayoutItem>(null));
				item.ParentCollectionName = collectionName;
			};
		}
		VisitDelegate<LayoutGroup> GetPrepareDecomposedItem(string collectionName) {
			return delegate(LayoutGroup item) {
				item.Accept(GetPrepareParentCollectionName<BaseLayoutItem>(null));
				item.ParentCollectionName = collectionName;
			};
		}
		VisitDelegate<T> GetPrepareParentCollectionName<T>(string collectionName) where T : class, ISerializableItem {
			return delegate(T item) {
				item.ParentCollectionName = collectionName;
				LayoutPanel panel = item as LayoutPanel;
				if(panel != null && panel.Layout != null) {
					panel.Layout.Accept(GetPrepareParentCollectionName<BaseLayoutItem>(null));
				}
				Items.Add(item);
			};
		}
		internal Dictionary<string, ISerializableItem> NamedItems { get; private set; }
		internal Dictionary<string, object> NamedControls { get; private set; }
		internal List<ISerializableItem> NewItems { get; private set; }
		protected void RestoreItems() {
			BeginDeserializeItems(Items);
			Container.DisposeFloatContainers();
			var removedItems = Container.GetItems().Except(Items);
			SerializationControllerHelper.ClearLayoutRoot(Container);
			SerializationControllerHelper.ClearItemsCollection(Container.ClosedPanels);
			SerializationControllerHelper.ClearHiddenItemsCollection(Container.LayoutController.HiddenItems);
			SerializationControllerHelper.ClearGroupsCollection(Container.FloatGroups);
			SerializationControllerHelper.ClearGroupsCollection(Container.AutoHideGroups);
			SerializationControllerHelper.ClearGroupsCollection(Container.DecomposedItems);
			BeginInitItems(NewItems);
			ProcessNewlyCreatedItems(NewItems);
			TabIndexes = SaveSelectedTabs();
			RestoreControls();
			RestoreLayoutRelations();
			InvalidateLayoutItems();
			RestorePlaceHolders();
			EndInitItems(NewItems);
			ProcessRemovedItems(removedItems);
			EndDeserializeItems(Items);
		}
		void ProcessRemovedItems(IEnumerable<ISerializableItem> RemovedItems) {
			foreach(var item in RemovedItems) {
				BaseLayoutItem bItem = item as BaseLayoutItem;
				if(bItem.IsLogicalTreeLocked)
					bItem.UnlockLogicalTree();
			}
		}
		void RestoreOwnedGroupRelations(PlaceHolder ph) {
			TabbedGroup tabGroup = ph.Parent as TabbedGroup;
			AutoHideGroup ahGroup = ph.Owner as AutoHideGroup;
			if(tabGroup != null && ahGroup != null) {
				LayoutGroup.SetOwnerGroup(tabGroup, ahGroup);
				ahGroup.HasPersistentGroups = !tabGroup.DestroyOnClosingChildren;
			}
		}
		void RestorePlaceHolders() {
			IEnumerable<ISerializableItem> phs = Items.Where(x => x is PlaceHolder).OrderBy(x => ((PlaceHolder)x).Index);
			foreach(ISerializableItem item in phs) {
				PlaceHolder ph = item as PlaceHolder;
				ph.BeginInit();
				ISerializableItem owner = null;
				ISerializableItem parent = null;
				int index = ph.Index;
				if(!string.IsNullOrEmpty(ph.OwnerName))
					NamedItems.TryGetValue(ph.OwnerName, out owner);
				if(!string.IsNullOrEmpty(ph.ParentName))
					NamedItems.TryGetValue(ph.ParentName, out parent);
				if(owner is BaseLayoutItem) {
					ph.Owner = (BaseLayoutItem)owner;
					if(parent is LayoutGroup)
						ph.Parent = (LayoutGroup)parent;
					PlaceHolderHelper.AddPlaceHolderForItem(parent as LayoutGroup, (BaseLayoutItem)owner, ph, index);
					if(ph.DockState == PlaceHolderState.Unset)
						RestoreOwnedGroupRelations(ph);
				}
				ph.EndInit();
			}
		}
		void BeginInitItems(List<ISerializableItem> newItems) {
			if(newItems == null) return;
			foreach(ISerializableItem item in newItems) {
				if(item is ISupportInitialize)
					((ISupportInitialize)item).BeginInit();
			}
		}
		void EndInitItems(List<ISerializableItem> newItems) {
			if(newItems == null) return;
			foreach(ISerializableItem item in newItems) {
				if(item is ISupportInitialize)
					((ISupportInitialize)item).EndInit();
			}
		}
		void BeginDeserializeItems(List<ISerializableItem> items) {
			if(items == null)
				return;
			foreach(ISerializableItem item in items) {
				if(item is BaseLayoutItem)
					((BaseLayoutItem)item).isDeserializing = true;
			}
		}
		void EndDeserializeItems(List<ISerializableItem> items) {
			if(items == null)
				return;
			foreach(ISerializableItem item in items) {
				BaseLayoutItem bItem = item as BaseLayoutItem;
				if(bItem != null) {
					bItem.isDeserializing = false;
					bItem.AttachedSerializationController = null;
				}
			}
		}
		protected void PrepareSerializableItems() {
			NamedItems = new Dictionary<string, ISerializableItem>();
			List<ISerializableItem> itemsWithInvalidNames = new List<ISerializableItem>();
			foreach(ISerializableItem item in Items) {
				if(!IsInvalidName(item.Name)) NamedItems.Add(item.Name, item);
				else itemsWithInvalidNames.Add(item);
			}
			foreach(ISerializableItem item in itemsWithInvalidNames) {
				EnsureUniqueName(NamedItems, item);
			}
			UpdateParentNames();
			CollectControls();
		}
		protected void UpdateParentNames() {
			foreach(ISerializableItem item in Items) {
				ISerializableItem parent = GetSerializableParent(item);
				item.ParentName = (parent != null) ? parent.Name : null;
				if(parent != null && item.ParentCollectionName != HiddenItemsSignature)
					item.ParentCollectionName = null;
			}
		}
		ISerializableItem GetSerializableParent(ISerializableItem item) {
			BaseLayoutItem bItem = item as BaseLayoutItem;
			LayoutGroup group = item as LayoutGroup;
			ISerializableItem parent = null;
			if(bItem != null) {
				parent = bItem.Parent;
				if(group != null && (parent == null) && (group.ParentPanel != null)) {
					parent = group.ParentPanel;
				}
			}
			return parent;
		}
		protected void CollectControls() {
			NamedControls = new Dictionary<string, object>();
			foreach(ISerializableItem item in Items) {
				if(item is LayoutPanel) {
					NamedControls.Add(item.Name, ((LayoutPanel)item).Control);
				}
				if(item is LayoutControlItem) {
					NamedControls.Add(item.Name, ((LayoutControlItem)item).Control);
				}
			}
		}
		protected void RestoreControls() {
			foreach(ISerializableItem item in Items) {
				LayoutPanel panelItem = item as LayoutPanel;
				if(panelItem != null) {
					object content;
					if(NamedControls.TryGetValue(item.Name, out content)) {
						UIElement control = content as UIElement;
						if(panelItem.Control != control) panelItem.Content = control;
					}
				}
				LayoutControlItem controlItem = item as LayoutControlItem;
				if(controlItem != null) {
					object content;
					if(NamedControls.TryGetValue(item.Name, out content)) {
						UIElement control = content as UIElement;
						if(controlItem.Content != control) controlItem.Content = control;
					}
				}
			}
		}
		protected void InvalidateLayoutItems() {
			foreach(ISerializableItem item in Items) {
				BaseLayoutItem bItem = item as BaseLayoutItem;
				if(bItem != null) {
					bItem.ClearValue(BaseLayoutItem.HasDesiredCaptionWidthPropertyKey);
					bItem.ClearValue(BaseLayoutItem.DesiredCaptionWidthPropertyKey);
					LayoutControlItem cItem = item as LayoutControlItem;
					if(cItem != null) {
						cItem.CoerceValue(LayoutControlItem.CaptionToControlDistanceProperty);
						cItem.CoerceValue(LayoutControlItem.ActualCaptionMarginProperty);
					}
					LayoutGroup group = item as LayoutGroup;
					if(group != null) {
						group.CoerceValue(LayoutGroup.ActualDockItemIntervalProperty);
						group.CoerceValue(LayoutGroup.ActualLayoutItemIntervalProperty);
						group.CoerceValue(LayoutGroup.ActualLayoutGroupIntervalProperty);
						group.CoerceValue(LayoutGroup.TabHeaderLayoutTypeProperty);
						group.CoerceValue(LayoutGroup.AllowSplittersProperty);
					}
					bItem.CoerceValue(BaseLayoutItem.CaptionFormatProperty);
					bItem.CoerceValue(BaseLayoutItem.ActualCaptionProperty);
					bItem.CoerceValue(BaseLayoutItem.TabCaptionProperty);
					bItem.CoerceValue(BaseLayoutItem.AppearanceProperty);
					PlaceHolderHelper.ClearPlaceHolder(bItem);
				}
			}
		}
		protected Dictionary<LayoutGroup, int> TabIndexes { get; private set; }
		protected void RestoreLayoutRelations() {
			List<ISerializableItem> restored = new List<ISerializableItem>();
			List<ISerializableItem> notRestored = new List<ISerializableItem>();
			LayoutGroup root = null;
			foreach(ISerializableItem item in Items) {
				if(item is FloatGroup) {
					if(item.ParentCollectionName == FloatGroupsSignature) {
						Container.FloatGroups.Add((FloatGroup)item);
						restored.Add(item);
						continue;
					}
				}
				if(item is AutoHideGroup) {
					if(item.ParentCollectionName == AutoHideGroupsSignature) {
						Container.AutoHideGroups.Add((AutoHideGroup)item);
						restored.Add(item);
						continue;
					}
				}
				if(item.ParentCollectionName == HiddenItemsSignature) {
					if(item is BaseLayoutItem) {
						ISerializableItem parent;
						if(!string.IsNullOrEmpty(item.ParentName) && NamedItems.TryGetValue(item.ParentName, out parent)) {
							Container.LayoutController.HiddenItems.Add((BaseLayoutItem)item, (LayoutGroup)parent);
						}
						else Container.LayoutController.HiddenItems.Add((BaseLayoutItem)item);
						restored.Add(item);
						continue;
					}
				}
				if(item.ParentCollectionName == ClosedPanelsSignature) {
					if(item is LayoutPanel) {
						Container.ClosedPanels.Add((LayoutPanel)item);
						restored.Add(item);
						continue;
					}
				}
				if(item.ParentCollectionName == DecomposedItemsSignature) {
					if(item is LayoutGroup) {
						Container.DecomposedItems.Add((LayoutGroup)item);
						restored.Add(item);
						continue;
					}
				}
				if(string.IsNullOrEmpty(item.ParentCollectionName)) {
					if(string.IsNullOrEmpty(item.ParentName)) {
						if(root == null && item is LayoutGroup) {
							root = (LayoutGroup)item;
							restored.Add(item);
							continue;
						}
					}
					else {
						ISerializableItem parent;
						if(NamedItems.TryGetValue(item.ParentName, out parent)) {
							if(parent is LayoutGroup && item is BaseLayoutItem) {
								((LayoutGroup)parent).Add((BaseLayoutItem)item);
								restored.Add(item);
								continue;
							}
							if(parent is LayoutPanel && item is LayoutGroup) {
								((LayoutPanel)parent).Content = item;
								restored.Add(item);
								continue;
							}
						}
					}
				}
				notRestored.Add(item);
			}
			ProcessNotRestoredItems(notRestored);
			Container.LayoutRoot = root;
			CheckRestoredItems(restored);
		}
		Dictionary<LayoutGroup, int> SaveSelectedTabs() {
			Dictionary<LayoutGroup, int> tabIndexes = new Dictionary<LayoutGroup, int>();
			Items.Accept(
					delegate(ISerializableItem item) {
						LayoutGroup group = item as LayoutGroup;
						if(group != null && group.IsTabHost)
							tabIndexes.Add(group, group.GetSerializableSelectedTabPageIndex());
					}
				);
			return tabIndexes;
		}
		void RestoreSelectedTabs(Dictionary<LayoutGroup, int> tabIndexes) {
			foreach(KeyValuePair<LayoutGroup, int> pair in tabIndexes) {
				if(pair.Key.IsTabHost)
					pair.Key.SelectedTabIndex = pair.Value;
			}
		}
		protected void ProcessNewlyCreatedItems(List<ISerializableItem> newItems) {
			if(newItems == null) return;
			foreach(ISerializableItem item in newItems) {
				EnsureUniqueName(NamedItems, item);
			}
		}
		protected virtual void CheckRestoredItems(List<ISerializableItem> restored) {
			if(Container.IsInDesignTime) return;
			bool addNewPanels = RestoreLayoutOptions.GetAddNewPanels(Container);
			bool addNewLayoutControlItems = RestoreLayoutOptions.GetAddNewLayoutControlItems(Container);
			bool addNewLayoutGroups = RestoreLayoutOptions.GetAddNewLayoutGroups(Container);
			foreach(var pair in NamedItems) {
				LayoutPanel oldPanel = pair.Value as LayoutPanel;
				if(oldPanel != null && !restored.Contains(oldPanel)) {
					if(addNewPanels)
						Container.ClosedPanels.Add(oldPanel);
					continue;
				}
				LayoutControlItem oldItem = pair.Value as LayoutControlItem;
				if(oldItem != null && !restored.Contains(oldItem)) {
					if(addNewLayoutControlItems)
						Container.LayoutController.HiddenItems.Add(oldItem);
					continue;
				}
				LayoutGroup group = pair.Value as LayoutGroup;
				if(group != null && !restored.Contains(group)) {
					if(addNewLayoutGroups)
						Container.LayoutController.HiddenItems.Add(group);
					else
						group.IsUngroupped = true;
				}
			}
		}
		protected virtual void ProcessNotRestoredItems(List<ISerializableItem> notRestored) { }
		protected bool IsInvalidName(string name) {
			return string.IsNullOrEmpty(name) || name.StartsWith(SpecialNameSignature);
		}
		protected void EnsureUniqueName(Dictionary<string, ISerializableItem> namedItems, ISerializableItem item) {
			string name = item.Name;
			bool invalid = IsInvalidName(name);
			if(invalid || namedItems.ContainsKey(name)) {
				AssignUniqueName(item, namedItems.Keys);
			}
			namedItems.Add(item.Name, item);
		}
		protected void AssignUniqueName(ISerializableItem item, ICollection<string> names) {
			Container.RaiseEvent(
					new RequestUniqueNameEventArgs(item, names) { Source = Container }
				);
			string newName = item.Name;
			if(string.IsNullOrEmpty(newName) || names.Contains(newName)) {
				item.Name = UniqueNameHelper.GetUniqueName("dockItem", names, 1);
			}
		}
		public virtual void OnClearCollection(XtraItemRoutedEventArgs e) { }
		public virtual object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			ISerializableItem resultItem = null;
			if(e.CollectionName == "SerializableDockSituation") {
				resultItem = new PlaceHolder();
				Items.Add(resultItem);
			}
			if(e.CollectionName == "Items") {
				XtraPropertyInfo info = e.Item.ChildProperties["Name"];
				XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
				if(info == null || infoType == null) return null;
				resultItem = CreateItem(info, infoType);
				if(resultItem != null) {
					if(NewItems == null) NewItems = new List<ISerializableItem>();
					NewItems.Add(resultItem);
					Items.Add(resultItem);
				}
			}
			return resultItem;
		}
		public virtual object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			if(e.CollectionName != "Items") return null;
			XtraPropertyInfo info = e.Item.ChildProperties["Name"];
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			if(info == null || infoType == null) return null;
			ISerializableItem item = FindItem(NamedItems, (string)info.Value, (string)infoType.Value);
			if(item != null)
				Items.Add(item);
			return item;
		}
		protected ISerializableItem FindItem(Dictionary<string, ISerializableItem> namedItems, string name, string type) {
			ISerializableItem item;
			if(namedItems.TryGetValue(name, out item)) {
				if(item.TypeName != type) {
					namedItems.Remove(name);
					item = null;
				}
			}
			return item;
		}
		protected ISerializableItem CreateItem(XtraPropertyInfo info, XtraPropertyInfo infoType) {
			string typeStr = (string)infoType.Value;
			string prefix = "DevExpress.Xpf.Docking.";
			if(typeStr.StartsWith(prefix))
				typeStr = typeStr.Remove(0, prefix.Length);
			if(CanRemoveOldItem(info, typeStr))
				return null;
			return CreateItemByType(info, typeStr);
		}
		protected virtual bool CanRemoveOldItem(XtraPropertyInfo info, string typeStr) {
			if(Container.IsInDesignTime) return false;
			switch(typeStr) {
				case "LayoutPanel":
				case "DocumentPanel":
					return RestoreLayoutOptions.GetRemoveOldPanels(Container);
				case "LayoutControlItem":
					return RestoreLayoutOptions.GetRemoveOldLayoutControlItems(Container);
				case "LayoutGroup":
					return RestoreLayoutOptions.GetRemoveOldLayoutGroups(Container);
			}
			return false;
		}
		protected virtual BaseLayoutItem CreateItemByType(XtraPropertyInfo info, string typeStr) {
			BaseLayoutItem newItem = null;
			switch(typeStr) {
				case "LayoutControlItem": newItem = Container.CreateLayoutControlItem(); break;
				case "LayoutPanel": newItem = Container.CreateLayoutPanel(); break;
				case "LayoutGroup": newItem = Container.CreateLayoutGroup(); break;
				case "FloatGroup": newItem = Container.CreateFloatGroup(); break;
				case "AutoHideGroup": newItem = Container.CreateAutoHideGroup(); break;
				case "TabbedGroup": newItem = Container.CreateTabbedGroup(); break;
				case "DocumentGroup": newItem = Container.CreateDocumentGroup(); break;
				case "DocumentPanel": newItem = Container.CreateDocumentPanel(); break;
				case "LayoutSplitter": newItem = Container.CreateLayoutSplitter(); break;
				case "LabelItem": newItem = Container.CreateLabelItem(); break;
				case "EmptySpaceItem": newItem = Container.CreateEmptySpaceItem(); break;
				case "SeparatorItem": newItem = Container.CreateSeparatorItem(); break;
			}
			if(newItem != null) {
				newItem.AttachedSerializationController = new WeakReference(this);
				newItem.Manager = Container;
				newItem.PrepareForModification(true);
			}
			return newItem;
		}
		public T CreateCommand<T>(object path) where T : SerializationControllerCommand, new() {
			return new T() { Controller = this, Path = path };
		}
		public const string DefaultID = "DockLayoutManager";
	}
	public class BaseLayoutItemSerializationProvider : SerializationProvider {
		protected override void OnCustomGetSerializableProperties(DependencyObject dObj, CustomGetSerializablePropertiesEventArgs e) {
			base.OnCustomGetSerializableProperties(dObj, e);
			DXSerializable dxSerializable = new DXSerializable();
			e.SetPropertySerializable("Name", dxSerializable);
			e.SetPropertySerializable("MinHeight", dxSerializable);
			e.SetPropertySerializable("MinWidth", dxSerializable);
			e.SetPropertySerializable("MaxWidth", dxSerializable);
			e.SetPropertySerializable("MaxHeight", dxSerializable);
			e.SetPropertySerializable("Tag", dxSerializable);
			if(((BaseLayoutItem)dObj).PreventCaptionSerialization) e.SetPropertySerializable("Caption", null);
		}
	}
	public class DXDockingSerializationProvider : SerializationProvider {
		protected override void OnCustomGetSerializableChildren(DependencyObject dObj, CustomGetSerializableChildrenEventArgs e) {
			DockLayoutManager manager = dObj as DockLayoutManager;
			if(manager != null) {
				BaseLayoutItem[] items = manager.GetItems();
				foreach(BaseLayoutItem item in items) {
					LayoutPanel panel = item as LayoutPanel;
					if(panel != null && panel.Control != null)
						e.Children.Add(panel.Control);
					LayoutControlItem cItem = item as LayoutControlItem;
					if(cItem != null && cItem.Control != null)
						e.Children.Add(cItem.Control);
				}
			}
		}
		protected override void OnClearCollection(XtraItemRoutedEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			Controller.OnClearCollection(e);
		}
		protected override object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			return Controller.OnCreateCollectionItem(e);
		}
		protected override object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			return Controller.OnFindCollectionItem(e);
		}
		static ISerializationController GetController(object obj) {
			return SerializationControllerHelper.GetSerializationController(obj as DependencyObject);
		}
		void RaiseEvent(SWRoutedEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			if(Controller != null) Controller.Container.RaiseEvent(e);
		}
		protected override bool OnAllowProperty(AllowPropertyEventArgs e) {
			if(e.Source is BaseLayoutItem) {
				RaiseEvent(e);
				return e.Allow;
			}
			else
				return base.OnAllowProperty(e);
		}
	}
	#region Commands
	public abstract class SerializationControllerCommand : ICommand {
		internal SerializationControllerCommand() { }
		protected internal ISerializationController Controller { get; set; }
		protected internal object Path { get; set; }
		protected abstract void ExecuteCore(ISerializationController controller, object path);
		protected abstract bool CanExecuteCore(object path);
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
			if(Controller == null) return false;
			return CanExecuteCore(Path);
		}
		void ICommand.Execute(object parameter) {
			if(Controller == null) return;
			ExecuteCore(Controller, Path);
		}
		#endregion ICommand
		#region static
		static SerializationControllerCommand() {
			SaveLayout = new SerializationControllerCommandLink("SaveLayout", new SaveLayoutCommand());
			RestoreLayout = new SerializationControllerCommandLink("RestoreLayout", new RestoreLayoutCommand());
		}
		public static RoutedCommand SaveLayout { get; private set; }
		public static RoutedCommand RestoreLayout { get; private set; }
		internal static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			ISerializationController controller = GetSerializationController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			SerializationControllerCommand command = ((SerializationControllerCommandLink)e.Command).Command;
			e.CanExecute = (controller != null && e.Parameter != null) && command.CanExecuteCore(e.Parameter);
		}
		internal static void Executed(object sender, ExecutedRoutedEventArgs e) {
			ISerializationController controller = GetSerializationController(DockLayoutManager.GetDockLayoutManager(sender as DependencyObject));
			if(controller != null && e.Parameter != null) {
				SerializationControllerCommand command = ((SerializationControllerCommandLink)e.Command).Command;
				command.ExecuteCore(controller, e.Parameter);
			}
		}
		static ISerializationController GetSerializationController(DockLayoutManager container) {
			return (container != null) ? container.SerializationController : null;
		}
		#endregion static
		class SerializationControllerCommandLink : RoutedCommand {
			public SerializationControllerCommandLink(string name, SerializationControllerCommand command) :
				base(name, typeof(SerializationControllerCommand)) {
				Command = command;
			}
			public SerializationControllerCommand Command { get; private set; }
		}
	}
	public class SaveLayoutCommand : SerializationControllerCommand {
		protected override bool CanExecuteCore(object path) {
			return path != null;
		}
		protected override void ExecuteCore(ISerializationController controller, object path) {
			controller.SaveLayout(path);
		}
	}
	public class RestoreLayoutCommand : SerializationControllerCommand {
		protected override bool CanExecuteCore(object path) {
			return path != null;
		}
		protected override void ExecuteCore(ISerializationController controller, object path) {
			controller.RestoreLayout(path);
		}
	}
	#endregion Commands
	public static class SerializationControllerHelper {
		public static ISerializationController GetSerializationController(DependencyObject dObj) {
			ISerializationController attached = GetSerializationController(dObj as BaseLayoutItem);
			if(attached != null) return attached;
			DockLayoutManager container = GetContainer(dObj);
			return GetSerializationController(container);
		}
		static DockLayoutManager GetContainer(DependencyObject dObj) {
			DockLayoutManager container = dObj as DockLayoutManager;
			if(container == null) {
				if(dObj is BaseLayoutItem) container = ((BaseLayoutItem)dObj).FindDockLayoutManager();
				if(container == null)
					container = (dObj != null) ? DockLayoutManager.GetDockLayoutManager(dObj) : null;
			}
			return container;
		}
		static ISerializationController GetSerializationController(BaseLayoutItem item) {
			if(item == null) return null;
			return item.AttachedSerializationController != null ? item.AttachedSerializationController.Target as ISerializationController : null;
		}
		static ISerializationController GetSerializationController(DockLayoutManager container) {
			return (container != null) ? container.SerializationController : null;
		}
		public static T CreateCommand<T>(DockLayoutManager container, object path) where T : SerializationControllerCommand, new() {
			ISerializationController controller = GetSerializationController(container);
			return (controller != null) ? controller.CreateCommand<T>(path) : null;
		}
		public static void ClearLayoutRoot(DockLayoutManager container) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			container.LayoutRoot.Accept(items.Add);
			container.LayoutRoot = null;
			Decompose(items);
		}
		public static void ClearHiddenItemsCollection(HiddenItemsCollection collection) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			collection.Accept((item) => item.Accept(items.Add));
			collection.Clear();
			Decompose(items);
		}
		public static void ClearItemsCollection<T>(ObservableCollection<T> collection) where T : BaseLayoutItem {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			collection.Accept(items.Add);
			collection.Clear();
			Decompose(items);
		}
		public static void ClearGroupsCollection<T>(ObservableCollection<T> collection) where T : LayoutGroup {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			collection.Accept((group) => group.Accept(items.Add));
			collection.Clear();
			Decompose(items);
		}
		static void Decompose(List<BaseLayoutItem> items) {
			foreach(BaseLayoutItem item in items) {
				if(item.Parent != null)
					item.Parent.Remove(item);
				LayoutPanel panel = item as LayoutPanel;
				if(panel != null && panel.Layout != null) {
					ClearLayoutGroup(panel.Layout);
				}
			}
		}
		static void ClearLayoutGroup(LayoutGroup group) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			group.Accept(items.Add);
			Decompose(items);
		}
	}
}
