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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid {
	public class ReadOnlyGridSortInfoCollection : ReadOnlyObservableCollection<GridSortInfo> {
		public ReadOnlyGridSortInfoCollection(ObservableCollection<GridSortInfo> list)
			: base(list) {
		}
		public GridSortInfo this[string name] { get { return GridSortInfo.GetSortInfoByFieldName(this, name); } }
	}
	public abstract class SortInfoCollectionBase : ObservableCollectionCore<GridSortInfo> {
		protected int fGroupCount = 0;
		string defaultSorting;
		ListSortDirection defaultSortOrder = ListSortDirection.Ascending;
		protected bool LockVerifying { get; set; }
		internal string DefaultSorting {
			get { return defaultSorting; }
			set {
				if(value == DefaultSorting)
					return;
				defaultSorting = value;
				VerifyDefaultSorting();
			}
		}
		internal ListSortDirection DefaultSortOrder {
			get { return defaultSortOrder; }
			set {
				if(defaultSortOrder == value)
					return;
				defaultSortOrder = value;
				VerifyDefaultSorting();
			}
		}
		internal int GroupCountCore { get { return fGroupCount; } }
		internal int GroupCountInternal {
			get { return fGroupCount < Count ? fGroupCount : Count; }
			set {
				if(value < 0)
					value = 0;
				if(fGroupCount == value)
					return;
				fGroupCount = value;
				OnChanged();
			}
		}
		public GridSortInfo this[string name] { get { return GridSortInfo.GetSortInfoByFieldName(this, name); } }
		public void ClearAndAddRange(params string[] fieldNames) { ClearAndAddRange(0, fieldNames); }
		public void ClearAndAddRange(int groupCount, params string[] fieldNames) {
			GridSortInfo[] items = new GridSortInfo[fieldNames.Length];
			for(int n = 0; n < fieldNames.Length; n++) {
				items[n] = new GridSortInfo() { FieldName = fieldNames[n] };
			}
			ClearAndAddRangeCore(groupCount, items);
		}
		public void ClearAndAddRange(params GridSortInfo[] items) {
			ClearAndAddRangeCore(0, items);
		}
		internal void ClearAndAddRangeCore(int groupCount, params GridSortInfo[] items) {
			BeginUpdate();
			try {
				Clear();
				AddRange(items);
				GroupCountInternal = groupCount;
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(params GridSortInfo[] items) {
			BeginUpdate();
			try {
				foreach(GridSortInfo item in items) {
					if(IsValid(item))
						Add(item);
				}
				VerifyDefaultSorting();
			}
			finally {
				EndUpdate();
			}
		}
		public void ClearSorting() {
			if(GroupCountInternal < Count) {
				BeginUpdate();
				LockVerifying = true;
				try {
					while(GroupCountInternal < Count)
						RemoveAt(Count - 1);
				}
				finally {
					LockVerifying = false;
					VerifyDefaultSorting();
					EndUpdate();
				}
			}
		}
		internal void SortByColumn(string fieldName, ColumnSortOrder direction, int sortIndex) {
			if(string.IsNullOrEmpty(fieldName))
				return;
			GridSortInfo info = this[fieldName];
			BeginUpdate();
			LockVerifying = true;
			try {
				if(info == null)
					info = new GridSortInfo() { FieldName = fieldName };
				else {
					if(IndexOf(info) < GroupCountInternal)
						GroupCountInternal--;
					Remove(info);
				}
				if(sortIndex > -1 && direction != ColumnSortOrder.None) {
					info.SortOrder = GridSortInfo.GetSortDirectionBySortOrder(direction);
					int actualSortIndex = sortIndex + GroupCountInternal;
					if(actualSortIndex > Count - 1)
						actualSortIndex = Count;
					Insert(actualSortIndex, info);
				}
			} finally {
				LockVerifying = false;
				VerifyDefaultSorting();
				EndUpdate();
			}
		}
		protected internal void OnColumnHeaderClick(string name, bool isShift, bool isCtrl) {
			if(string.IsNullOrEmpty(name))
				return;
			if(isShift) {
				OnColumnHeaderClickAddSort(name, ListSortDirection.Ascending);
				return;
			}
			if(isCtrl) {
				OnColumnHeaderClickRemoveSort(name);
				return;
			}
			GridSortInfo sortInfo = this[name];
			BeginUpdate();
			if(sortInfo == null) {
				sortInfo = new GridSortInfo() { FieldName = name };
			}
			else {
				sortInfo.ChangeSortOrder();
			}
			try {
				LockVerifying = true;
				for(int i = Count - 1; i >= GroupCountInternal; i--) {
					RemoveAt(i);
				}
				if(this[sortInfo.FieldName] == null) {
					Add(sortInfo);
				}
				LockVerifying = false;
				VerifyDefaultSorting();
			}
			finally {
				EndUpdate();
			}
		}
		internal void OnColumnHeaderClickAddSort(string name, ListSortDirection defaultDirection) {
			GridSortInfo sortInfo = this[name];
			BeginUpdate();
			try {
				if(sortInfo == null) {
					sortInfo = new GridSortInfo() { FieldName = name, SortOrder = defaultDirection };
					Add(sortInfo);
					VerifyDefaultSorting();
				}
				else {
					if(IndexOf(sortInfo) >= GroupCountInternal) {
						Remove(sortInfo);
						sortInfo.ChangeSortOrder();
						Add(sortInfo);
					}
					else {
						sortInfo.ChangeSortOrder();
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		internal void OnColumnHeaderClickRemoveSort(string name) {
			GridSortInfo sortInfo = this[name];
			if(sortInfo != null && IndexOf(sortInfo) >= GroupCountInternal)
				Remove(sortInfo);
			VerifyDefaultSorting();
		}
		protected override void InsertItem(int index, GridSortInfo item) {
			item.Owner = this;
			base.InsertItem(index, item);
		}
		protected override void RemoveItem(int index) {
			GridSortInfo info = this[index];
			info.Owner = null;
			base.RemoveItem(index);
			VerifyDefaultSorting();
		}
		protected virtual bool IsValid(GridSortInfo item) {
			return !string.IsNullOrEmpty(item.FieldName);
		}
		internal void VerifyDefaultSorting() {
			if(LockVerifying || string.IsNullOrEmpty(DefaultSorting))
				return;
			LockVerifying = true;
			try {
				GridSortInfo sortInfo = this[DefaultSorting];
				if(sortInfo != null) {
					int index = IndexOf(sortInfo);
					if(index >= GroupCountInternal && index < Count - 1)
						Remove(sortInfo);
					else
						return;
				}
				Add(new GridSortInfo(DefaultSorting, DefaultSortOrder));
			} finally {
				LockVerifying = false;
			}
		}
		internal void OnChanged() {
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}
