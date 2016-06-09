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
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
using DevExpress.Data;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
namespace DevExpress.Xpf.Grid {
	public class GridSortInfoCollection : SortInfoCollectionBase {
		public GridSortInfoCollection() {
		}
		public virtual void ClearAndAddRange(int groupCount, params GridSortInfo[] items) {
			ClearAndAddRangeCore(groupCount, items);
		}
		public void GroupByColumn(string fieldName) {
			GroupByColumn(fieldName, GroupCount);
		}
		public void GroupByColumn(string fieldName, int index, ColumnSortOrder sortOrder = ColumnSortOrder.Ascending) {
			if(string.IsNullOrEmpty(fieldName))
				return;
			if(index < 0 || sortOrder == ColumnSortOrder.None) {
				UngroupByColumn(fieldName);
				return;
			}
			GridSortInfo sortInfo = this[fieldName];
			int newGroupCount = GroupCount;
			if(sortInfo == null || IndexOf(sortInfo) >= GroupCount) {
				newGroupCount++;
			}
			BeginUpdate();
			try {
				ListSortDirection sortDirection = GridSortInfo.GetSortDirectionBySortOrder(sortOrder);
				if(sortInfo != null) {
					if(index > IndexOf(sortInfo))
						index--;
					LockVerifying = true;
					Remove(sortInfo);
					sortInfo.SortOrder = sortDirection;
					LockVerifying = false;
				}
				else {
					sortInfo = new GridSortInfo(fieldName, sortDirection);
				}
				index = Math.Min(newGroupCount - 1, index);
				Insert(index, sortInfo);
				this.fGroupCount = newGroupCount;
			} finally {
				EndUpdate();
			}
		}
		public void UngroupByColumn(string fieldName) {
			if(string.IsNullOrEmpty(fieldName))
				return;
			GridSortInfo sortInfo = this[fieldName];
			if(sortInfo == null || IndexOf(sortInfo) >= GroupCount)
				return;
			BeginUpdate();
			try {
				this.fGroupCount = Math.Max(GroupCount - 1, 0);
				Remove(sortInfo);
			} finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridSortInfoCollectionGroupCount"),
#endif
 DefaultValue(0)]
		public int GroupCount {
			get { return GroupCountInternal; }
			set { GroupCountInternal = value; }
		}
		protected internal void OnGroupColumnMove(string name, int index, bool fromGroup, bool toGroup) {
			if(!fromGroup && !toGroup)
				return;
			if(fromGroup && !toGroup) {
				UngroupByColumn(name);
				return;
			}
			ColumnSortOrder sortOrder = ColumnSortOrder.Ascending;
			if(this[name] != null) sortOrder = this[name].GetSortOrder();
			if(!fromGroup && toGroup) {
				GroupByColumn(name, index, sortOrder);
				return;
			}
			if(fromGroup && toGroup) {
				GroupByColumn(name, index, sortOrder);
			}
		}
	}
	public class GridGroupSummarySortInfoCollection : ObservableCollectionCore<GridGroupSummarySortInfo> {
		internal GridDataProviderBase Owner { get; private set; }
		public GridGroupSummarySortInfoCollection(GridDataProviderBase owner) {
			Owner = owner;
		}
		public void ClearAndAddRange(params GridGroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos);
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(params GridGroupSummarySortInfo[] items) {
			BeginUpdate();
			try {
				foreach(GridGroupSummarySortInfo item in items) {
					Add(item);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override void InsertItem(int index, GridGroupSummarySortInfo item) {
			item.Owner = this;
			base.InsertItem(index, item);
		}
		protected override void RemoveItem(int index) {
			GridGroupSummarySortInfo info = this[index];
			info.Owner = null;
			base.RemoveItem(index);
		}
		internal void Sync(IList<GridSortInfo> sortList, int groupCount) {
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					GridGroupSummarySortInfo item = this[n];
					if(Owner.GroupSummary.IndexOf(item.SummaryItem) == -1 || !IsGroupedColumn(item.FieldName, sortList, groupCount))
						RemoveSafe(item);
				}
			}
			finally {
				CancelUpdate();
			}
		}
		bool IsGroupedColumn(string fieldName, IList<GridSortInfo> sortList, int groupCount) {
			for(int i = 0; i < groupCount; i++) {
				if(fieldName == sortList[i].FieldName)
					return true;
			}
			return false;
		}
		internal void ClearCore() {
			BeginUpdate();
			try {
				Clear();
			}
			finally {
				CancelUpdate();
			}
		}
	}
}
