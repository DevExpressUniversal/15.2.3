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
using System.ComponentModel;
using DevExpress.Data.Helpers;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Data;
#else
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Data.Selection {
	public class SelectionController : IDisposable {
		SelectedRowsCollection selectedRows;
		SelectedGroupsCollection selectedGroupRows;
		DataController controller;
		int lockAddRemoveActions = 0;
		int selectionLockCount = 0;
		bool actuallyChanged = false;
		List<SelectedRowsCollection> selectionCollections;
		List<SelectedRowsCollection> SelectionCollections {
			get {
				if(selectionCollections == null)
					selectionCollections = CreateSelectionCollections();
				return selectionCollections;
			}
		}
		protected virtual List<SelectedRowsCollection> CreateSelectionCollections() {
			return new List<SelectedRowsCollection>() { selectedRows };
		}
		public SelectionController(DataController controller) {
			this.controller = controller;
			this.selectedRows = new SelectedRowsCollection(this);
			this.selectedGroupRows = new SelectedGroupsCollection(this);
		}
		public virtual void Dispose() {
			this.controller = null;
			this.selectedGroupRows.Dispose();
			this.selectedRows.Dispose();
			this.selectedGroupRows = null;
			this.selectedRows = null;
			this.selectionCollections = null;
		}
		protected internal DataController Controller { get { return controller; } }
		protected internal SelectedRowsCollection SelectedRows { get { return selectedRows; } }
		protected SelectedGroupsCollection SelectedGroupRows { get { return selectedGroupRows; } }
		protected internal void RaiseSelectionChanged() {
			OnSelectionChanged(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, DevExpress.Data.DataController.InvalidRow));
		}
		internal void LockAddRemoveAction() {
			this.lockAddRemoveActions++;
		}
		internal void UnLockAddRemoveAction() {
			this.lockAddRemoveActions--;
		}
		protected internal void OnSelectionChanged(SelectionChangedEventArgs e) {
			this.actuallyChanged = true;
			if(this.selectionLockCount != 0) return;
			Controller.RaiseSelectionChanged(e);
		}
		public void SetActuallyChanged() {
			if(IsSelectionLocked) actuallyChanged=true;
		}
		public void BeginSelection() {
			if(this.selectionLockCount ++ == 0) actuallyChanged = false;
		}
		public bool IsSelectionLocked { get { return this.selectionLockCount != 0; } }
		public void CancelSelection() {
			this.selectionLockCount --;
		}
		public void EndSelection() {
			if(--this.selectionLockCount == 0 && actuallyChanged) 
				OnSelectionChanged(SelectionChangedEventArgs.Refresh);
		}
		public void RaiseChanged() {
			if(this.selectionLockCount != 0)
				this.actuallyChanged = true;
			else
				OnSelectionChanged(SelectionChangedEventArgs.Refresh);
		}
		public int Count { get { return SelectedRows.Count + SelectedGroupRows.Count; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool GetSelectedByListSource(int listSourceRow) {
			return SelectedRows.GetSelected(listSourceRow);
		}
		public bool GetSelected(int controllerRow) {
			if(Controller.IsGroupRowHandle(controllerRow)) {
				return SelectedGroupRows.GetRowSelected(controllerRow);
			}
			return SelectedRows.GetRowSelected(controllerRow);
		}
		internal object GetSelectionObject(int controllerRow) {
			if(Controller.IsGroupRowHandle(controllerRow)) {
				return SelectedGroupRows.GetRowObjectByControllerRow(controllerRow);
			}
			return SelectedRows.GetRowObjectByControllerRow(controllerRow);
		}
		public object GetSelectedObject(int controllerRow) {
			if(Controller.IsGroupRowHandle(controllerRow)) {
				return SelectedGroupRows.GetRowSelectedObject(controllerRow);
			}
			return SelectedRows.GetRowSelectedObject(controllerRow);
		}
		public void SetSelected(int controllerRow, bool selected) {
			SetSelected(controllerRow, selected, true);
		}
		public void SetSelected(int controllerRow, bool selected, object selectionObject) {
			if(Controller.IsGroupRowHandle(controllerRow)) {
				SelectedGroupRows.SetRowSelected(controllerRow, selected, selectionObject);
				return;
			}
			SelectedRows.SetRowSelected(controllerRow, selected, selectionObject);
		}
		public void SelectAll() {
			BeginSelection();
			try {
				Clear();
				for(int n = 0; n < Controller.VisibleCount; n++) {
					SetSelected(Controller.GetControllerRowHandle(n), true);
				}
			}
			finally {
				EndSelection();
			}
		}
		protected internal int InternalClear() {
			int prevCount = Count;
			BeginSelection();
			try {
				Clear();
			} finally {
				CancelSelection();
			}
			return prevCount;
		}
		public virtual void Clear() {
			if(Count == 0) return;
			BeginSelection();
			try {
				SelectedGroupRows.Clear();
				SelectedRows.Clear();
			}
			finally {
				EndSelection();
			}
		}
		internal void SetListSourceRowSelected(int listSourceRow, bool selected, object selectionObject) {
			SelectedRows.SetRowSelected(Controller.GetControllerRow(listSourceRow), selected, selectionObject);
		}
		public class SelectionInfo {
			public int SelectedCount;
			public int SelectedDataRows, SelectedGroupRows, SelectedRowsCRC, SelectedGroupRowsCRC;
			public bool Equals(SelectionInfo info) {
				if(info == null) return false;
				return this.SelectedCount == info.SelectedCount && this.SelectedDataRows == info.SelectedDataRows
					&& this.SelectedGroupRows == info.SelectedGroupRows && this.SelectedRowsCRC == info.SelectedRowsCRC &&
					this.SelectedGroupRowsCRC == info.SelectedGroupRowsCRC;
			}
		}
		public SelectionInfo GetSelectionInfo() {
			SelectionInfo res = new SelectionInfo();
			res.SelectedCount = Count;
			res.SelectedDataRows = SelectedRows.Count;
			res.SelectedGroupRows = SelectedGroupRows.Count;
			res.SelectedRowsCRC = SelectedRows.CalcCRC();
			res.SelectedGroupRowsCRC = SelectedGroupRows.CalcCRC();
			return res;
		}
		public int[] GetSelectedRows() {
			if(Count == 0) return new int[0];
			int[] res = new int[Count];
			SelectedRows.CopyToArray(res, 0);
			for(int n = SelectedRows.Count - 1; n >= 0 ; n--) {
				res[n] = Controller.GetControllerRow(res[n]);
			}
			SelectedGroupRows.CopyToArray(res, SelectedRows.Count);
			Array.Sort(res);
			return res;
		}
		protected int[] GetNonGroupedSelectedRows() {
			int[] rows = GetSelectedRows();
			if(rows == null) return new int[0];
			Array.Sort(rows);
			return rows;
		}
		void ProcessRows(Dictionary<int, List<int>> groups, int[] rows, Dictionary<int, int> nonSelectedGroups) {
			for(int n = 0; n < rows.Length; n++) {
				int handle = rows[n];
				if(handle == DataController.InvalidRow) continue;
				if(nonSelectedGroups != null && !Controller.IsGroupRowHandle(handle)) {
					GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByControllerRowHandle(handle);
						if(group != null) {
						if(groups.ContainsKey(group.Handle) || nonSelectedGroups.ContainsKey(group.Handle)) continue;
							handle = group.Handle;
							nonSelectedGroups[handle] = 0;
					}
				}
				if(Controller.IsGroupRowHandle(handle)) {
					GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByHandle(handle);
					List<int> list = null;
					if(Controller.GroupInfo.IsLastLevel(group)) {
						for(int k = 0; k < rows.Length; k++) {
							if(group.ContainsControllerRow(rows[k])) {
								if(list == null) list = new List<int>();
								list.Add(rows[k]);
								rows[k] = DataController.InvalidRow;
							}
						}
						if(list != null && list.Count > 0) list.Sort();
					}
					rows[n] = DataController.InvalidRow;
					groups[handle] = list;
				}
			}
		}
		public virtual int[] GetNormalizedSelectedRowsEx() {
			if(!Controller.IsGrouped || Count == 0) return GetNonGroupedSelectedRows();
			int n = 0;
			int[] rows = GetSelectedRows();
			Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();
			ProcessRows(groups, rows, null);
			if(groups.Count == 0) {
				Array.Sort(rows);
				return rows;
			}
			Dictionary<int, int> nonSelectedGroups = new Dictionary<int, int>();
			ProcessRows(groups, rows, nonSelectedGroups);
			int[] groupRows = new int[groups.Count];
			int i = 0;
			foreach(var obj in groups.Keys)
				groupRows[i++] = (int)obj;
			Array.Sort(groupRows, new ReverseComparer());
			List<int> resultRows = new List<int>();
			for(n = 0; n < groupRows.Length; n++) {
				int groupHandle = groupRows[n];
				if(!nonSelectedGroups.ContainsKey(groupHandle))
					resultRows.Add(groupHandle);
				GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByHandle(groupHandle);
				if(group != null && Controller.GroupInfo.IsLastLevel(group)) {
					List<int> list;
					groups.TryGetValue(groupHandle, out list);
					if(list != null) resultRows.AddRange(list);
				}
			}
			return resultRows.ToArray();
		}
		public virtual int[] GetNormalizedSelectedRowsEx2() {
			if(!Controller.IsGrouped || Count == 0) return GetNonGroupedSelectedRows();
			int[] rows = GetSelectedRows();
			bool hasGroupRows = false;
			for(int r = 0; r < rows.Length; r++) {
				if(rows[r] < 0) {
					hasGroupRows = true;
					break;
				}
			}
			if(!hasGroupRows) return GetNonGroupedSelectedRows();
			Array.Sort(rows);
			Dictionary<int, bool> selectedGroups = new Dictionary<int, bool>();
			List<int> res = new List<int>();
			GroupRowInfo lastGroup = null;
			for(int n = 0; n < rows.Length; n++) {
				int row = rows[n];
				if(row < 0) continue;
				GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByControllerRowHandle(row);
				if(group != null) {
					if(group != lastGroup) AddGroup(res, group, selectedGroups);
					lastGroup = group;
				}
				res.Add(row);
			}
			return res.ToArray();
		}
		void AddGroup(List<int> rows, GroupRowInfo group, Dictionary<int, bool> selectedGroups) {
			int[] groups = new int[group.Level + 1];
			if(group.Level == 0) {
				if(!selectedGroups.ContainsKey(group.Handle)) rows.Add(group.Handle);
				return;
			}
			while(group != null) {
				if(!selectedGroups.ContainsKey(group.Handle)) groups[group.Level] = group.Handle;
				group = group.ParentGroup;
			}
			rows.AddRange(groups);
		} 
		public virtual int[] GetNormalizedSelectedRows() {
			if(!Controller.IsGrouped || Count == 0) return GetNonGroupedSelectedRows();
			int n = 0;
			int[] rows = GetSelectedRows();
			Dictionary<int, int> groups = new Dictionary<int, int>();
			for(n = 0; n < rows.Length; n++) {
				int handle = rows[n];
				GroupRowInfo group = Controller.GroupInfo.GetGroupRowInfoByControllerRowHandle(handle);
				if(group == null) continue;
				int lastHandle = group.Handle;
				while(group.ParentGroup != null) {
					group = group.ParentGroup;
					lastHandle = group.Handle;
				}
				if(Controller.IsGroupRowHandle(lastHandle)) groups[lastHandle] = 1;
			}
			int[] groupRows = new int[groups.Count];
			n = 0;
			foreach (int entry in groups.Keys)
				groupRows[n ++] = entry;
			Array.Sort(groupRows, new ReverseComparer());
			return groupRows;
		}
		class ReverseComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				return Comparer.Default.Compare(b, a);
			}
		}
		internal void OnReplaceGroupSelection(GroupRowInfo oldGroupInfo, GroupRowInfo newGroupInfo) { SelectedGroupRows.OnReplaceGroupSelection(oldGroupInfo, newGroupInfo); }
		internal void OnGroupDeleted(GroupRowInfo groupInfo) { SelectedGroupRows.OnGroupDeleted(groupInfo); }
		internal void OnItemMoved(int oldListSourceRow, int newListSourceRow) {	
			if(this.lockAddRemoveActions > 0) return;
			BeginSelection();
			try {
				SelectionCollections.ForEach(collection => collection.OnItemMoved(oldListSourceRow, newListSourceRow));
			}
			finally {
				EndSelection();
			}
		}
		internal void OnItemAdded(int listSourceRow) {
			if(this.lockAddRemoveActions > 0) return;
			SelectionCollections.ForEach(collection => collection.OnItemAdded(listSourceRow));
		}
		internal void OnItemDeleted(int listSourceRow) {
			if(this.lockAddRemoveActions > 0) return;
			BeginSelection();
			try {
				SelectionCollections.ForEach(collection => collection.OnItemDeleted(listSourceRow));
			}
			finally {
				EndSelection();
			}
		}
		internal void OnItemFilteredOut(int listSourceRow) {
			SelectionCollections.ForEach(collection => collection.OnItemFilteredOut(listSourceRow));
		}
	}
#if !SL && !DXPORTABLE
	public class CurrencySelectionController : SelectionController {
		public CurrencySelectionController(DataController controller) : base(controller) { }
		protected new internal CurrencyDataController Controller { get { return base.Controller as CurrencyDataController; } }
		int[] CheckNormalizedSelectedRows(int[] rows) {
			if(rows == null || rows.Length == 0) {
				if(!Controller.IsValidControllerRowHandle(Controller.CurrentControllerRow)) return new int[0];
				rows = new int[] { Controller.CurrentControllerRow };
			}
			return rows;
		}
		public override int[] GetNormalizedSelectedRows() { 
			return CheckNormalizedSelectedRows(base.GetNormalizedSelectedRows());
		}
		public override int[] GetNormalizedSelectedRowsEx() { 
			return CheckNormalizedSelectedRows(base.GetNormalizedSelectedRowsEx());
		}
	}
#endif
	public abstract class BaseSelectionCollection : IDisposable {
		Dictionary<object, object> rows;
		SelectionController selectionController;
		protected BaseSelectionCollection(SelectionController selectionController) {
			this.selectionController = selectionController;
		}
		protected DataController Controller { get { return SelectionController == null ? null : SelectionController.Controller; } }
		protected SelectionController SelectionController { get { return selectionController; } }
		public int Count { get { return rows == null ? 0 : rows.Count; } }
		protected Dictionary<object, object> Rows { 
			get {
				if (rows == null) rows = new Dictionary<object, object>();
				return rows;
			}
		}
		public virtual void Dispose() {
			this.selectionController = null;
			Clear();
		}
		public void Clear() {
			if(Count == 0) return;
			Rows.Clear();
			OnSelectionChanged(SelectionChangedEventArgs.Refresh);
		}
		protected virtual void OnSelectionChanged(SelectionChangedEventArgs e) {
			if(SelectionController != null) SelectionController.OnSelectionChanged(e);
		}
		protected internal abstract object GetRowObjectByControllerRow(int controllerRow);
		public object GetRowSelectedObject(int controllerRow) {
			object row = GetRowObjectByControllerRow(controllerRow);
			return row == null ? null : GetSelectedObject(row);
		}
		public bool GetRowSelected(int controllerRow) {
			object row = GetRowObjectByControllerRow(controllerRow);
			return row == null ? false : GetSelected(row);
		}
		public void SetRowSelected(int controllerRow, bool selected, object selectionObject) {
			object row = GetRowObjectByControllerRow(controllerRow);
			if(row == null) return;
			SetSelected(controllerRow, row, selected, selectionObject);
		}
		protected object GetSelectedObject(object row) {
			if(Count == 0) return null;
			object result;
			Rows.TryGetValue(row, out result);
			return result;
		}
		protected internal bool GetSelected(object row) {
			if(Count == 0) return false;
			return Rows.ContainsKey(row);
		}
		protected void SetSelected(int controllerRow, object row, bool selected, object selectionObject) {
			if(GetSelected(row) == selected) {
				if(selected && selectionObject != null) {
					if(Object.Equals(GetSelectedObject(row), selectionObject)) return;
				} else
					return;
			}
			if(!selected) {
				Rows.Remove(row);
			} else {
				SetSelectionObject(row, selectionObject);
			}
			OnSelectionChanged(new SelectionChangedEventArgs(selected ? CollectionChangeAction.Add : CollectionChangeAction.Remove, controllerRow));
		}
		protected virtual void SetSelectionObject(object row, object selectionObject) {
			Rows[row] = selectionObject;
		}
		protected internal virtual int CalcCRC() {
			return 0xffff;
		}
	}
	public class SelectedGroupsCollection : BaseSelectionCollection {
		public SelectedGroupsCollection(SelectionController selectionController) : base(selectionController) { }
		protected internal override object GetRowObjectByControllerRow(int controllerRow) {
			return Controller.GroupInfo.GetGroupRowInfoByHandle(controllerRow);
		}
		internal void OnGroupDeleted(GroupRowInfo groupInfo) { 
			SetSelected(0, groupInfo, false, null);
		}
		internal void OnReplaceGroupSelection(GroupRowInfo oldGroupInfo, GroupRowInfo newGroupInfo) { 
			if(!GetSelected(oldGroupInfo)) return;
			if(newGroupInfo == null) {
				OnGroupDeleted(oldGroupInfo);
				return;
			}
			Rows.Remove(oldGroupInfo);
			Rows[newGroupInfo] = null;
		}
		public void CopyToArray(int[] array, int startIndex) {
			if(Count == 0) return;
			GroupRowInfo[] groups = new GroupRowInfo[Count];
			Rows.Keys.CopyTo(groups, 0);
			for(int n = 0; n < groups.Length; n ++) {
				array[n + startIndex] = groups[n].Handle;
			}
		}
		protected internal override int CalcCRC() {
			uint res = 0xabcd9211;
			foreach(GroupRowInfo i in Rows.Keys) {
				res += (uint)i.Handle ^ (uint)0x10009653;
			}
			return (int)res;
		}
	}
	public class SelectedRowsCollection : BaseSelectionCollection, IIndexRenumber {
		public SelectedRowsCollection(SelectionController selectionController) : base(selectionController) { }
		protected internal override object GetRowObjectByControllerRow(int controllerRow) {
			int row = Controller.GetListSourceRowIndex(controllerRow);
			if(row == DataController.InvalidRow) return null;
			return row;
		}
		void SetListSourceRowSelected(int listSourceRow, bool selected, object selectionObject) {
			SetSelected(Controller.GetControllerRow(listSourceRow), listSourceRow, selected, selectionObject);
		}
		protected internal void OnItemMoved(int oldListSourceRow, int newListSourceRow) {
			if(Count == 0) return;
			bool isRowSelected = GetSelected(oldListSourceRow);
			object selection = null;
			if(isRowSelected) {
				Rows.TryGetValue(oldListSourceRow, out selection);
				Rows.Remove(oldListSourceRow);
			}
			RenumberIndexes(oldListSourceRow, newListSourceRow);
			if(isRowSelected) 
				SetListSourceRowSelected(newListSourceRow, true, selection);
		}
		protected internal void OnItemAdded(int listSourceRow) { RenumberIndexes(listSourceRow, true); }
		protected internal virtual void OnItemDeleted(int listSourceRow) {
			SetListSourceRowSelected(listSourceRow, false, null);
			RenumberIndexes(listSourceRow, false);
		}
		protected internal virtual void OnItemFilteredOut(int listSourceRow) {
			SetListSourceRowSelected(listSourceRow, false, null);
		}
		void RenumberIndexes(int listSourceRow, bool increment) {
			PrepareReindex(increment);
			IndexRenumber.RenumberIndexes(this, listSourceRow, increment, Controller.ListSourceRowCount - 1);
		}
		void RenumberIndexes(int oldListSourceRow, int newListSourceRow) {
			PrepareReindex(oldListSourceRow > newListSourceRow);
			IndexRenumber.RenumberIndexes(this, oldListSourceRow, newListSourceRow);
		}
		public int[] CopyToArray(int length) {
			int[] res = new int[length];
			if(Count > 0){
				int i = 0;
				foreach(int value in Rows.Keys) {
					res[i] = value;
					i++;
				}
			}
			return res;
		}
		protected internal override int CalcCRC() {
			uint res = 0xabcd9211;
			foreach(int i in Rows.Keys) {
				res += (uint)i ^ (uint)0x10009653;
			}
			return (int)res;
		}
		public int[] CopyToArray() { return CopyToArray(Count); }
		public void CopyToArray(int[] array, int startIndex) {
			if(Count > 0) {
				int i = 0;
				foreach(int value in Rows.Keys) {
					array[i + startIndex] = value;
					i++;
				}
			}
		}
		int[] indexes = null;
		void PrepareReindex(bool increment) {
			indexes = CopyToArray();
			Array.Sort<int>(indexes);
			if(!increment) Array.Reverse(indexes);
		}
		#region IIndexRenumber Members
		int IIndexRenumber.GetCount() { return Count; }
		int IIndexRenumber.GetValue(int pos) { return indexes[pos]; }
		void IIndexRenumber.SetValue(int pos, int val) {
			int oldPos = indexes[pos];
			object selection;
			Rows.TryGetValue(oldPos, out selection);
			Rows.Remove(oldPos);
			SetSelectionObject(val, selection);
		}
		#endregion
	}
}
