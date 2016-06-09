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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Data.Details;
using System.Linq;
using System.Threading;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Collections;
using DevExpress.Compatibility.System.Data;
#if !SL
using System.Data;
using System.Windows.Forms;
#else
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data {
	public class ServerModeDataController : ServerModeDataControllerBase {
		protected override VisibleIndexCollection CreateVisibleIndexCollection() { return new ServerModeDataControllerVisibleIndexCollection(this); }
		protected override GroupRowInfoCollection CreateGroupRowInfoCollection() { return new ServerDataControllerGroupRowInfoCollection(this); }
		public override bool AllowNew { get { return true; } }
		public override bool IsReady { get { return ListSourceEx != null; } }
		public override IList GetAllFilteredAndSortedRows(Function<bool> callBackMethod) {
			if(ListSourceEx == null) return new List<object>();
			return ListSourceEx.GetAllFilteredAndSortedRows();
		}
		public override bool PrefetchAllData(Function<bool> callbackMethod) {
#if SILVERLIGHT
			return false;
#else
			if(ListSourceEx == null) return true;
			int attempts = 0;
			while(true) {
				if(!ListSourceEx.PrefetchRows(null, CancellationToken.None)) {
					if(attempts++ > 3) return false;
					Thread.Sleep(100);
					continue;
				}
				return true;
			}
#endif
		}
		protected override MasterRowInfoCollection CreateMasterRowCollection() { return new ServerModeMasterRowInfoCollection(this); }
		protected override SelectedRowsKeeper CreateSelectionKeeper() { return new ServerModeCurrentAndSelectedRowsKeeper(this, false); }
		protected override FilterHelper CreateFilterHelper() { return new ServerModeDataControllerFilterHelper(this); }
		public override ListSourceRowsKeeper CreateControllerRowsKeeper() { return new ServerModeListSourceRowsKeeper(this, CreateSelectionKeeper()); }
		protected override IList GetListSource() {
			if(DataSource == null) return null;
			IListSource ls = DataSource as IListSource;
			if(ls != null) return ls.GetList();
#if !SL
			DataTable table = DataSource as DataTable;
			if(table != null) return table.DefaultView;
#endif
			IList list = DataSource as IList;
			return list;
		}
		public IListServer ListSourceEx { get { return ListSource as IListServer; } }
		public IListServerCaps ListSourceEx2 { get { return ListSource as IListServerCaps; } }
		public override bool CanSort { get { return ListSourceEx2 == null || ListSourceEx2.CanSort; } }
		public override bool CanGroup { get { return ListSourceEx2 == null || ListSourceEx2.CanGroup; } }
		public override bool CanFilter { get { return ListSourceEx2 == null || ListSourceEx2.CanFilter; } }
		protected override bool ProcessListServerAction(string fieldName, ColumnServerActionType action, out bool res) {
			res = true;
			if(ListSourceEx2 == null) return false;
			switch(action) {
				case ColumnServerActionType.Filter: res = ListSourceEx2.CanFilter; break;
				case ColumnServerActionType.Group: res = ListSourceEx2.CanGroup; break;
				case ColumnServerActionType.Sort: res = ListSourceEx2.CanSort; break; 
			}
			if(!res) return true;
			return false;
		}
		public override int FindRowByBeginWith(string columnName, string text) {
			DataColumnInfo colInfo = Columns[columnName];
			if(ListSourceEx == null || colInfo == null || colInfo.Unbound) return InvalidRow;
			int listIndex = ListSourceEx.FindIncremental(DescriptorToCriteria(colInfo), text, -1, false, false, false);
			return GetControllerRow(listIndex);
		}
		public override int FindRowByValue(string columnName, object value, params OperationCompleted[] completed) {
			DataColumnInfo colInfo = Columns[columnName];
			if(colInfo == null) {
				if(string.IsNullOrEmpty(columnName))
					return FindRowByRowValue(value);
				return InvalidRow;
			}
			if(colInfo.Unbound && !colInfo.UnboundWithExpression) return base.FindRowByValue(columnName, value);
			if(ListSourceEx != null) {
				int listIndex = ListSourceEx.LocateByValue(DescriptorToCriteria(colInfo), value, -1, false);
				return GetControllerRow(listIndex);
			}
			return InvalidRow;
		}
		protected override object GetCurrentControllerRowObject() {
			return GetRowKey(CurrentControllerRow);
		}
		public override int FindRowByRowValue(object value) {
			if(ListSourceEx != null) {
				if(value != null && !(value is DBNull))
					return GetControllerRow(ListSourceEx.IndexOf(value));
			}
			return InvalidRow;
		}
		public override int FindIncremental(string text, int columnHandle, int startRowHandle, bool down, bool ignoreStartRow, bool allowLoop, CompareIncrementalValue compareValue, params OperationCompleted[] completed) {
			DataColumnInfo colInfo = Columns[columnHandle];
			if(ListSourceEx == null || colInfo == null || colInfo.Unbound) return InvalidRow;
			int row = ListSourceEx.FindIncremental(DescriptorToCriteria(colInfo), text, GetListSourceRowIndex(startRowHandle), !down, ignoreStartRow, allowLoop);
			return row < 0 ? InvalidRow : row;
		}
		protected override void DoSortRows() {
			if(IsDataSyncInProgress && DataSync != null) {
				if(DataSync.ResetCache()) return;
			}
			if(ListSourceEx != null)
				ListSourceEx.Apply(ClientUserSubstituteFilter(UnboundCriteriaInliner.Process(FilterCriteria, Columns)),
					AsyncServerModeDataController.GetSortCollection(this), SortInfo.GroupCount, 
					AsyncServerModeDataController.ListSourceSummaryItemsToServerModeSummaryDescriptors(GroupSummary.GetSummaryItems(AllowSortUnbound)), 
					AsyncServerModeDataController.ListSourceSummaryItemsToServerModeSummaryDescriptors(TotalSummary.GetSummaryItems(AllowSortUnbound)));
		}
		protected override void DoGroupRows() {
			if(!IsGrouped || ListSourceEx == null) return;
			List<ListSourceGroupInfo> root = ListSourceEx.GetGroupInfo(null);
			if(root.Count == 0) return;
			CreateGroupInfo(root, GroupInfo, null);
			GroupInfo.UpdateIndexes();
			BuildVisibleIndexes();
		}
		protected override void ChangeExpandedLevel(int groupLevel, bool expanded, bool recursive) {
			throw new NotImplementedException("ServerMode doesn't support Expand/Collapse group levels");
		}
		protected override void ChangeExpanded(int groupRowHandle, bool expanded, bool recursive) {
			ServerModeGroupRowInfo sgroup = (ServerModeGroupRowInfo)GroupInfo.GetGroupRowInfoByHandle(groupRowHandle);
			if(sgroup == null) return;
			if(sgroup.ListGroupInfo == null || (expanded && !sgroup.ChildrenReady)) RequestChildren(sgroup);
			base.ChangeExpanded(groupRowHandle, expanded, recursive);
		}
		protected override void ChangeAllExpanded(bool expanded) {
			if(!IsGrouped) return;
			ResetRowsKeeperEx();
			if(expanded) {
				bool prev = GroupInfo.AutoExpandAllGroups;
				try {
					GroupInfo.AutoExpandAllGroups = true;
					GroupInfo.Clear();
					DoGroupRows();
				}
				finally {
					GroupInfo.AutoExpandAllGroups = false;
				}
				VisualClientUpdateLayout();
			}
			else {
				base.ChangeAllExpanded(false);
			}
		}
		protected internal override void RestoreGroupExpanded(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = (ServerModeGroupRowInfo)group;
			RequestChildren(sgroup).Expanded = true;
		}
		protected internal override void MakeGroupRowVisible(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = (ServerModeGroupRowInfo)group;
			RequestChildren(sgroup).Expanded = true;
		}
		protected virtual ServerModeGroupRowInfo RequestChildren(ServerModeGroupRowInfo sgroup) {
			if(sgroup.ChildrenReady || ListSourceEx == null) return sgroup;
			if(sgroup.ListGroupInfo == null) {
				if(sgroup.ParentGroup == null) return sgroup;
				ServerModeGroupRowInfo parent = RequestChildren((ServerModeGroupRowInfo)sgroup.ParentGroup);
				sgroup = (ServerModeGroupRowInfo)GroupInfo[parent.Index + 1];
				if(sgroup.ChildrenReady) return sgroup;
			}
			List<ListSourceGroupInfo> list = ListSourceEx.GetGroupInfo(sgroup.ListGroupInfo);
			if(list == null || list.Count == 0) return sgroup;
			List<GroupRowInfo> insertList = new List<GroupRowInfo>();
			CreateGroupInfo(list, insertList, sgroup);
			((ServerDataControllerGroupRowInfoCollection)GroupInfo).UpdateChildren(sgroup, insertList);
			sgroup.ChildrenReady = true;
			VisibleIndexes.SetDirty();
			return sgroup;
		}
		void CreateGroupInfo(List<ListSourceGroupInfo> list, IList destination, ServerModeGroupRowInfo parentGroup) {
			byte level = (byte)(parentGroup == null ? 0 : (parentGroup.Level + 1));
			int startIndex = parentGroup == null ? 0 : parentGroup.ChildControllerRow;
			bool finalLevel = SortInfo.GroupCount == level + 1;
			List<ServerModeGroupRowInfo> tempDestination = new List<ServerModeGroupRowInfo>();
			for(int n = 0; n < list.Count; n++) {
				ListSourceGroupInfo linfo = list[n];
				ServerModeGroupRowInfo group = new ServerModeGroupRowInfo(level, startIndex, parentGroup, linfo);
				group.ChildControllerRowCount = linfo.ChildDataRowCount;
				group.ChildrenReady = finalLevel || GroupInfo.AutoExpandAllGroups || level <= GroupInfo.AlwaysVisibleLevelIndex;
				group.Expanded = GroupInfo.AutoExpandAllGroups || level <= GroupInfo.AlwaysVisibleLevelIndex;
				tempDestination.Add(group);
				startIndex += group.ListGroupInfo.ChildDataRowCount;
			}
			SummarySortInfo sortInfo = SummarySortInfo.GetByLevel(parentGroup == null ? 0 : parentGroup.Level + 1);
			if(sortInfo != null) {
				foreach(GroupRowInfo rowInfo in tempDestination) {
					RequestSummary(rowInfo);
				}
				tempDestination.Sort(new GroupSummaryComparer(this, sortInfo));
			}
			for(int n = 0; n < tempDestination.Count; n++) {
				ServerModeGroupRowInfo group = tempDestination[n];
				destination.Add(group);
				CreateChildren(destination, level, group.ChildControllerRow, group.ListGroupInfo, group);
			}
		}
		void CreateChildren(IList destination, byte level, int startIndex, ListSourceGroupInfo linfo, ServerModeGroupRowInfo group) {
			if(level + 1 <= GroupInfo.AlwaysVisibleLevelIndex || GroupInfo.AutoExpandAllGroups) {
				List<ListSourceGroupInfo> childList = ListSourceEx.GetGroupInfo(group.ListGroupInfo);
				CreateGroupInfo(childList, destination, group);
			}
			else {
				for(int g = level + 1; g < SortInfo.GroupCount; g++) {
					ServerModeGroupRowInfo child = new ServerModeGroupRowInfo((byte)g, startIndex, group, null);
					child.ChildControllerRowCount = linfo.ChildDataRowCount;
					group = child;
					destination.Add(group);
				}
			}
		}
		protected override void CalcTotalSummary() {
			if(IsUpdateLocked) return;
			if(ListSourceEx == null) {
				return;
			}
			TotalSummary.IsDirty = false;
			foreach(SummaryItem item in TotalSummary) item.SummaryValue = null;
			if(TotalSummary.ActiveCount == 0) return;
			List<object> res = ListSourceEx.GetTotalSummary();
			if(res != null && res.Count == TotalSummary.Count) {
				for(int n = 0; n < res.Count; n++) {
					TotalSummary[n].SummaryValue = res[n];
				}
			}
			for(int n = 0; n < TotalSummary.Count; n++) {
				if(!TotalSummary[n].GetAllowExternalCalculate(AllowSortUnbound))
					CalcTotalSummaryItem(TotalSummary[n]);
			}
		}
		protected override GroupRowInfo RequestSummary(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = group as ServerModeGroupRowInfo;
			if(sgroup == null) return sgroup;
			if(sgroup.ListGroupInfo == null) sgroup = RequestChildren(sgroup);
			if(sgroup.ListGroupInfo == null || sgroup.ListGroupInfo.Summary == null) return sgroup;
			if(sgroup.IsSummaryReady) return sgroup;
			sgroup.IsSummaryReady = true;
			sgroup.SetSummary(GroupSummary, sgroup.ListGroupInfo.Summary);
			return sgroup;
		}
		public override void UpdateGroupSummary(List<SummaryItem> changedItems) {
			if(GroupSummary.Count > 0 && SortInfo.GroupCount > 0) {
				for(int n = 0; n < GroupInfo.Count; n++) {
					GroupInfo[n].ClearSummary();
				}
				DoRefresh(); 
			}
			base.UpdateGroupSummary(changedItems);
		}
	}
	public class ServerModeDataControllerFilterHelper : DataControllerFilterHelper {
		public ServerModeDataControllerFilterHelper(ServerModeDataController controller) : base(controller) { }
		public new ServerModeDataController Controller { get { return (ServerModeDataController)base.Controller; } }
		public override object[] GetUniqueColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed, bool implyNullLikeEmptyStringWhenFiltering) {
			if(maxCount == 0) return null;
			if(Controller.ListSourceEx == null && !Controller.IsColumnValid(column)) return null;
			DataColumnInfo colInfo = Controller.Columns[column];
			if(colInfo.Unbound) {
				if(!colInfo.UnboundWithExpression || !Controller.AllowSortUnbound) return new object[0];
			}
			try {
				CriteriaOperator expression = ServerModeDataController.DescriptorToCriteria(colInfo);
				if(roundDataTime && (colInfo.Type.Equals(typeof(DateTime)) || colInfo.Type.Equals(typeof(DateTime?)))) {
					expression = new FunctionOperator(FunctionOperatorType.GetDate, expression);
				}
				var rv = Controller.ListSourceEx.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
				if(implyNullLikeEmptyStringWhenFiltering)
					return rv;
				else
					return rv.Where(o => o != null).ToArray();
			} catch {
				return new object[0];
			}
		}
	}
	public class ServerModeGroupRowInfo : GroupRowInfo {
		ListSourceGroupInfo listGroupInfo;
		public bool ChildrenReady = false, IsSummaryReady = false;
		public ServerModeGroupRowInfo() : this(0, 0, null, null) { }
		public ServerModeGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroup, ListSourceGroupInfo listGroupInfo) :
				base(level, childControllerRow, parentGroup) {
			this.listGroupInfo = listGroupInfo;
		}
		public ListSourceGroupInfo ListGroupInfo { get { return listGroupInfo; } set { listGroupInfo = value; } }
		public override void ClearSummary() {
			this.IsSummaryReady = false;
			base.ClearSummary();
		}
		public override object GroupValue { get { return ListGroupInfo == null ? null : ListGroupInfo.GroupValue; } }
#if DEBUG
		public override string ToString() {
			return string.Format("{0}: Level:{1} Start:{2} Count:{3}, ChildrenReady:{4}", Index, Level, ChildControllerRow, ChildControllerRowCount, ChildrenReady);
		}
#endif
		internal void SetSummary(SummaryItemCollection groupSummary, List<object> summaryValues) {
			for(int n = 0; n < Math.Min(groupSummary.Count, summaryValues.Count); n++) {
				SetSummaryValue(groupSummary[n], summaryValues[n]);
			}
		}
	}
	public class ListSourceGroupInfo {
		int level;
		object groupValue;
		int childDataRowCount;
		public ListSourceGroupInfo() {
			this.level = this.childDataRowCount = 0;
		}
		public int Level { get { return level; } set { level = value; } }
		public int ChildDataRowCount { get { return childDataRowCount; } set { childDataRowCount = value; } }
		public object GroupValue { get { return groupValue; } set { groupValue = value; } }
		public object AuxValue { get; set; }
		public virtual List<object> Summary { get { return null; } }
	}
	public class ServerDataControllerGroupRowInfoCollection : DataControllerGroupRowInfoCollection {
		public ServerDataControllerGroupRowInfoCollection(DataController controller) : base(controller) { }
		protected override GroupRowInfo CreateGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroupRow) {
			throw new NotImplementedException();
		}
		public override GroupRowInfo Add(byte level, int ChildControllerRow, GroupRowInfo parentGroup) {
			throw new NotImplementedException();
		}
		internal void UpdateChildren(ServerModeGroupRowInfo sgroup, List<GroupRowInfo> insertList) {
			int index = sgroup.Index;
			if(index < 0 || index >= Count || this[index] != sgroup) 
				index = IndexOf(sgroup);
			if(index == -1) return;
			int removeToIndex = index;
			while(removeToIndex + 1 < Count && this[removeToIndex + 1].Level > sgroup.Level) {
				removeToIndex++;
			}
			if(removeToIndex != index) ListCore.RemoveRange(index + 1, removeToIndex - index);
			if(insertList.Count > 1)
				ListCore.InsertRange(index + 1, insertList);
			else {
				Insert(index + 1, insertList[0]);
			}
			UpdateIndexes(index);
		}
	}
	public class ListSourceSummaryItem {
		SummaryItemType summaryType;
		DataColumnInfo info;
		internal ListSourceSummaryItem(SummaryItem item) : this(item.ColumnInfo == null ? null : item.ColumnInfo, item.SummaryType) {
		}
		public ListSourceSummaryItem() : this(null, SummaryItemType.None) { }
		public ListSourceSummaryItem(DataColumnInfo info, SummaryItemType summaryType) {
			this.info = info;
			this.summaryType = summaryType;
		}
		public SummaryItemType SummaryType {
			get { return summaryType; }
			set { summaryType = value; }
		}
		public DataColumnInfo Info {
			get { return info; }
			set { info = value; }
		}
	}
	public class ServerModeListSourceRowsKeeper : ListSourceRowsKeeper {
		public ServerModeListSourceRowsKeeper(ServerModeDataController controller, SelectedRowsKeeper rowsKeeper) : base(controller, rowsKeeper) { 
		}
		protected override GroupedRowsKeeperEx CreateGroupRowsKeeper() {
			return new ServerModeGroupedRowsKeeperEx(Controller);
		}
		protected new ServerModeDataController Controller { get { return (ServerModeDataController)base.Controller; } }
		protected override void RestoreSelectionCore(int count) {
			RestoreRegularRowsSelection();
		}
		protected virtual void RestoreRegularRowsSelection() {
			if(Controller.ListSourceEx == null) return;
			foreach(var enLevel in SelectionHash.Levels) {
				int level = enLevel.Key;
				var rows = enLevel.Value;
				if(level == BaseRowsKeeper.DataRowsLevel) {
					foreach(var entry in rows) {
						if(entry.Key == BaseRowsKeeper.NullObject) continue;
						int index = Controller.ListSourceEx.GetRowIndexByKey(entry.Key);
						if(index >= 0)
							SelectionHash.RestoreCore(index, level, entry.Value);
					}
				}
			}
		}
		protected override object ExGetGroupRowKeyCore(GroupRowInfo group) {
			ServerModeGroupRowInfo sgroup = group as ServerModeGroupRowInfo; 
			if(sgroup == null || sgroup.ListGroupInfo == null) return null; 
			return GroupHashEx.GetGroupRowKeyEx(group);
		}
	}
	public class ServerModeMasterRowInfoCollection : MasterRowInfoCollection {
		public ServerModeMasterRowInfoCollection(DataController controller) : base(controller) { }
		public override MasterRowInfo Find(int listSourceRow) {
			if(Count == 0) return null;
			object key = Controller.Helper.GetRowKey(listSourceRow);
			return FindByKey(key);
		}   
	}
	public class ServerModeGroupedRowsKeeperEx : GroupedRowsKeeperEx {
		public ServerModeGroupedRowsKeeperEx(DataController controller) : base(controller) { }
		public override bool AllExpanded {
			get {
				if(!base.AllExpanded) return false;
				if(RecordsCount > Controller.VisibleListSourceRowCount / 2) return true;
				return false;
			}
		}
		protected override bool GetAllRecordsSelected() {
			int groupCount = Controller.GroupInfo.Count;
			if(groupCount == 0) return false;
			if(Controller.VisibleCount < groupCount) return false;
			foreach(GroupRowInfo info in Controller.GroupInfo) {
				if(!info.Expanded) return false;
			}
			return true;
		}
	}
	public interface IListServerCaps {
		bool CanFilter { get; }
		bool CanGroup { get; }
		bool CanSort { get; }
	}
}
