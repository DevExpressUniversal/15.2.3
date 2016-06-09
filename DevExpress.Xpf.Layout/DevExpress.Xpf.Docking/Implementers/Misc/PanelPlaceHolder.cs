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
using System.ComponentModel;
using System.Linq;
using DevExpress.Utils.Serializing;
namespace DevExpress.Xpf.Docking.Internal {
	public enum PlaceHolderState {Docked, AutoHidden, Floating, Unset}
	public class PlaceHolderCollection : List<PlaceHolder> {
	}
	class PlaceHolderInfoHelper {
		PlaceHolderCollection dockInfo = new PlaceHolderCollection();
		public PlaceHolder Find(Predicate<PlaceHolder> match) {
			return dockInfo.Find(match);
		}
		public int Count { get { return dockInfo.Count; } }
		public bool HasItems() {
			return Count > 0;
		}
		public void ForeEach(Action<PlaceHolder> action) {
			dockInfo.ForEach(action);
		}
		public void Add(PlaceHolder placeHolder) {
			dockInfo.Add(placeHolder);
		}
		public void Remove(PlaceHolder ph) {
			dockInfo.Remove(ph);
		}
		public void Clear() {
			dockInfo.Clear();
		}
		public PlaceHolder FirstOrDefault() {
			return dockInfo.FirstOrDefault();
		}
		public PlaceHolder FirstOrDefault(Func<PlaceHolder, bool> predicate) {
			return dockInfo.FirstOrDefault(predicate);
		}
		public bool Contains(PlaceHolder placeHolder) {
			return dockInfo.Contains(placeHolder);
		}
		internal PlaceHolder this[int index] {
			get { return dockInfo[index]; }
		}
		internal PlaceHolderCollection AsCollection() {
			return dockInfo;
		}
	}
	public class PlaceHolder : ISerializableItem, ISupportInitialize {
		public LayoutGroup Parent { get; internal set; }
		public BaseLayoutItem Owner { get; internal set; }
		string parentName;
		string ownerName;
		int index;
		int _hash;
		[XtraSerializableProperty]
		public PlaceHolderState DockState { get; set; }
		[XtraSerializableProperty]
		public string ParentName {
			get { return Parent != null ? Parent.Name : parentName; }
			set { parentName = value; }
		}
		[XtraSerializableProperty]
		public string OwnerName {
			get { return Owner != null ? Owner.Name : ownerName; }
			set { ownerName = value; }
		}
		[XtraSerializableProperty]
		public int Index {
			get { return Parent != null ? Parent.PlaceHolderHelper.IndexOf(this) : GetRestoredIndex(); }
			set { index = value; }
		}
		int GetRestoredIndex() {
			return index;
		}
		internal PlaceHolder() {
		}
		public PlaceHolder(BaseLayoutItem owner, LayoutGroup parent) {
			DevExpress.Xpf.Layout.Core.Base.AssertionException.IsNotNull(owner);
			DevExpress.Xpf.Layout.Core.Base.AssertionException.IsNotNull(parent);
			Owner = owner;
			Parent = parent;
			DockState = owner.IsFloating ? PlaceHolderState.Floating : owner.IsAutoHidden ? PlaceHolderState.AutoHidden : PlaceHolderState.Docked;
			InvalidateHash();
		}
		public int GetDockIndex() {
			int indexInParentCollection = -1;
			if(Parent != null)
				indexInParentCollection = Parent.PlaceHolderHelper.IndexOf(this);
			return indexInParentCollection == -1 ? GetRestoredIndex() : indexInParentCollection;
		}
		void InvalidateHash() {
			if(Parent != null && Owner != null)
				_hash = (int)DockState ^ Parent.GetHashCode() ^ Owner.GetHashCode();
		}
		public override bool Equals(object obj) {
			PlaceHolder info = obj as PlaceHolder;
			if(info == null) return false;
			return _hash == info._hash;
		}
		public override int GetHashCode() {
			return _hash;
		}
		public static bool operator ==(PlaceHolder left, PlaceHolder right) {
			return object.Equals(left, right);
		}
		public static bool operator !=(PlaceHolder left, PlaceHolder right) {
			return !object.Equals(left, right);
		}
		public bool IsFloating { get { return DockState == PlaceHolderState.Floating; } }
		public bool IsAutoHidden { get { return DockState == PlaceHolderState.AutoHidden; } }
		public bool IsDocked { get { return DockState == PlaceHolderState.Docked; } }
		#region ISerializableItem Members
		string ISerializableItem.Name { get; set; }
		string ISerializableItem.TypeName { get { return typeof(PlaceHolder).Name; } }
		string ISerializableItem.ParentName { get; set; }
		string ISerializableItem.ParentCollectionName { get; set; }
		#endregion
		#region ISupportInitialize Members
		public void BeginInit() {
		}
		public void EndInit() {
			InvalidateHash();
		}
		#endregion
	}
	class PlaceHolderHelper {
		#region static
		public static PlaceHolder GetPlaceHolderForDockOperation(BaseLayoutItem item, bool isRestore) {
			PlaceHolder placeHolder = PlaceHolderHelper.PlaceHolderToDock(item, isRestore);
			return placeHolder != null && PlaceHolderHelper.CanRestoreLayoutHierarchy(item, placeHolder.DockState) ? placeHolder : null;
		}
		static PlaceHolder PlaceHolderToDock(BaseLayoutItem item, bool isRestore) {
			PlaceHolder placeHolder = null;
			if(isRestore) {
				placeHolder = item.DockInfo.Find((d) => d.IsAutoHidden);
				if(placeHolder == null) {
					placeHolder = item.DockInfo.Find((d) => d.IsFloating);
				}
			}
			if(placeHolder == null) placeHolder = item.DockInfo.Find((d) => d.IsDocked);
			return placeHolder;
		}
		public static PlaceHolder GetPlaceHolder(BaseLayoutItem itemToDock, PlaceHolderState state = PlaceHolderState.Docked) {
			return itemToDock.DockInfo.Find((di) => di.DockState == state);
		}
		public static int GetPlaceHolderCount(BaseLayoutItem item) {
			return item.DockInfo.Count;
		}
		public static bool HasItems(BaseLayoutItem item) {
			return item.DockInfo.Count > 0;
		}
		public static LayoutGroup[] GetAffectedGroups(BaseLayoutItem panel) {
			List<LayoutGroup> groups = new List<LayoutGroup>();
			panel.DockInfo.ForeEach(ph => groups.Add(ph.Parent));
			return groups.ToArray();
		}
		public static LayoutGroup GetAffectedGroup(BaseLayoutItem panel, PlaceHolderState state = PlaceHolderState.Docked) {
			PlaceHolder ph = panel.DockInfo.Find((di) => di.DockState == state);
			return ph != null ? ph.Parent : null;
		}
		public static void ClearPlaceHolder(BaseLayoutItem item, PlaceHolder placeHolder) {
			if(placeHolder == null) return;
			LayoutGroup placeHolderParent = placeHolder.Parent;
			item.DockInfo.Remove(placeHolder);
			if(placeHolderParent != null)
				placeHolderParent.PlaceHolderHelper.Remove(placeHolder);
		}
		public static void ClearPlaceHolder(BaseLayoutItem item, PlaceHolderState state) {
			PlaceHolder placeHolder = GetPlaceHolder(item, state);
			ClearPlaceHolder(item, placeHolder);
		}
		public static void ClearPlaceHolder(BaseLayoutItem itemToDock) {
			itemToDock.DockInfo.ForeEach(placeHolder =>
			{
				LayoutGroup placeHolderParent = placeHolder.Parent;
				if(placeHolderParent != null)
					placeHolderParent.PlaceHolderHelper.Remove(placeHolder);
			});
			itemToDock.DockInfo.Clear();
		}
		internal static int GetDockIndex(BaseLayoutItem itemToDock, PlaceHolder ph) {
			return PlaceHolderHelper.GetDockIndex(ph);
		}
		internal static LayoutGroup RestoreLayoutHierarchy(DockLayoutManager container, BaseLayoutItem item, PlaceHolderState desiredState = PlaceHolderState.Docked) {
			PlaceHolder ph = item.DockInfo.Find((di) => di.DockState == desiredState);
			if(ph == null || ph.DockState == PlaceHolderState.Unset) return null;
			LayoutGroup parent = ph.Parent;
			var root = parent.GetRoot();
			if(root.IsInTree()) return parent;
			RestoreLayoutHierarchyCore(container, parent);
			return parent;
		}
		static void RestoreLayoutHierarchyCore(DockLayoutManager container, LayoutGroup group) {
			PlaceHolder placeHolder = PlaceHolderHelper.FirstOrDefault(group);
			if(placeHolder != null && placeHolder.DockState != PlaceHolderState.Unset) {
				LayoutGroup parent = placeHolder.Parent;
				RestoreLayoutHierarchyCore(container, parent);
				int index = PlaceHolderHelper.GetDockIndex(placeHolder);
				DockControllerHelper.InsertItemInGroup(parent, group, index);
				PlaceHolderHelper.ClearPlaceHolder(group);
			}
			group.IsUngroupped = false;
			container.DecomposedItems.Remove(group);
		}
		internal static bool CanRestoreLayoutHierarchy(BaseLayoutItem item, PlaceHolderState desiredState = PlaceHolderState.Docked) {
			if(item == null) return false;
			PlaceHolder ph = item.DockInfo.Find((di) => di.DockState == desiredState);
			if(ph == null || ph.DockState == PlaceHolderState.Unset) return false;
			LayoutGroup parent = ph.Parent;
			return parent != null && CanRestoreLayoutHierarchyCore(parent);
		}
		static bool CanRestoreLayoutHierarchyCore(LayoutGroup group) {
			PlaceHolder placeHolder = PlaceHolderHelper.FirstOrDefault(group);
			if(placeHolder == null || placeHolder.DockState == PlaceHolderState.Unset) {
				LayoutGroup root = group.GetRoot();
				return root is AutoHideGroup || root is FloatGroup || root.IsRootGroup;
			}
			return CanRestoreLayoutHierarchyCore(placeHolder.Parent);
		}
		internal static LayoutGroup GetRestoreRoot(BaseLayoutItem item, PlaceHolderState desiredState = PlaceHolderState.Docked) {
			if(item == null) return null;
			PlaceHolder ph = item.DockInfo.Find((di) => di.DockState == desiredState);
			if(ph == null || ph.DockState == PlaceHolderState.Unset) return null;
			LayoutGroup parent = ph.Parent;
			return parent != null ? GetRestoreRoot(parent) : null;
		}
		static LayoutGroup GetRestoreRoot(LayoutGroup group) {
			PlaceHolder placeHolder = PlaceHolderHelper.FirstOrDefault(group);
			if(placeHolder == null || placeHolder.DockState == PlaceHolderState.Unset) {
				return group.GetRoot();
			}
			return GetRestoreRoot(placeHolder.Parent);
		}
		static PlaceHolder FirstOrDefault(BaseLayoutItem item) {
			return item.DockInfo.FirstOrDefault();
		}
		public static int GetDockIndex(PlaceHolder placeHolder) {
			LayoutGroup placeHolderParent = placeHolder.Parent;
			PlaceHolderHelper helper = placeHolderParent.PlaceHolderHelper;
			int index = 0;
			int index1 = helper.IndexOf(placeHolder);
			if(index1 == -1) index1 = placeHolder.GetDockIndex();
			BaseLayoutItem _item = null;
			for(int i = index1; i < helper.Count; i++) {
				if(helper[i] is BaseLayoutItem) {
					_item = (BaseLayoutItem)helper[i];
					index = placeHolderParent.Items.IndexOf(_item);
					break;
				}
			}
			if(_item == null) {
				for(int i = index1 - 1; i >= 0; i--) {
					if((helper[i] is BaseLayoutItem)) {
						_item = (BaseLayoutItem)helper[i];
						index = placeHolderParent.Items.IndexOf(_item) + 1;
						break;
					}
				}
			}
			return index;
		}
		static internal void AddPlaceHolderForItem(LayoutGroup parent, BaseLayoutItem item, PlaceHolder ph, int index) {
			if(!item.DockInfo.Contains(ph))
				item.DockInfo.Add(ph);
			if(parent != null)
				parent.PlaceHolderHelper.Insert(index, ph);
		}
		static internal void AddFakePlaceHolderForItem(LayoutGroup parent, BaseLayoutItem item) {
			PlaceHolder ph = new PlaceHolder(item, parent) { DockState = PlaceHolderState.Unset };
			if(!item.DockInfo.Contains(ph))
				item.DockInfo.Add(ph);
			if(parent != null)
				parent.PlaceHolderHelper.Insert(0, ph);
		}
		#endregion
		public LayoutGroup Owner { get; private set; }
		public PlaceHolderHelper(LayoutGroup owner) {
			Owner = owner;
		}
		ObservableCollection<object> itemsInternal = new ObservableCollection<object>();
		int Count { get { return itemsInternal.Count; } }
		public bool HasPlaceHolders { get { return itemsInternal.FirstOrDefault((fe) => fe is PlaceHolder) != null; } }
		public void Remove(object item) {
			itemsInternal.Remove(item);
		}
		public void Insert(int index, object item) {
			if(!itemsInternal.Contains(item)) {
				if(!(index >= 0 && index < Count)) index = Count;
				itemsInternal.Insert(index, item);
			}
		}
		public void Add(object item) {
			if(!itemsInternal.Contains(item))
				itemsInternal.Add(item);
		}
		public int IndexOf(object item) {
			return itemsInternal.IndexOf(item);
		}
		public bool Contains(object item) {
			return itemsInternal.Contains(item);
		}
		object this[int index] {
			get { return itemsInternal[index]; }
		}
		public void AddPlaceHolderForItem(BaseLayoutItem item) {
			if(GetPlaceHolderForItem(item) != null) return;			
			LayoutGroup group = item.Parent;
			PlaceHolder placeHolder = new PlaceHolder(item, group);
			if(!item.DockInfo.Contains(placeHolder))
				item.DockInfo.Add(placeHolder);
			Insert(IndexOf(item), placeHolder);
		}
		public void InsertItem(int index, BaseLayoutItem item) {
			LayoutGroup layoutGroup = Owner;
			PlaceHolder placeHolder = GetPlaceHolderForItem(item);
			if(placeHolder == null || placeHolder.Parent != layoutGroup) {
				if(index < layoutGroup.Items.Count - 1) {
					BaseLayoutItem currentItem = layoutGroup.Items[index + 1];
					index = layoutGroup.PlaceHolderHelper.IndexOf(currentItem);
					layoutGroup.PlaceHolderHelper.Insert(index, item);
				}
				else
					layoutGroup.PlaceHolderHelper.Add(item);
			}
			else {
				index = layoutGroup.PlaceHolderHelper.IndexOf(placeHolder);
				layoutGroup.PlaceHolderHelper.Insert(index, item);
				layoutGroup.PlaceHolderHelper.Remove(placeHolder);
			}
		}
		PlaceHolder GetPlaceHolderForItem(BaseLayoutItem item) {
			return (PlaceHolder)itemsInternal.FirstOrDefault(obj => obj is PlaceHolder && ((PlaceHolder)obj).Owner == item);
		}
	}
}
