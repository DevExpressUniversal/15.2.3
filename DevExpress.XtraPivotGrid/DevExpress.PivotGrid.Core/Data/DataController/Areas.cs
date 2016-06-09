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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Data.PivotGrid {
	public abstract class PivotDataControllerArea : DataControllerGroupHelperBase {
		PivotColumnInfoCollection columns;
		PivotGroupRowsKeeper rowsKeeper;
		PivotVisibleIndexCollection visibleIndexes;
		DataColumnSortInfoCollection sortInfo;
		PivotSummaryItemCollection summaries;
		public PivotDataControllerArea(PivotDataController controller)
			: base(controller) {
			this.summaries = null;
			this.visibleIndexes = new PivotVisibleIndexCollection(Controller, GroupInfo);
			this.columns = new PivotColumnInfoCollection(Controller, new CollectionChangeEventHandler(OnColumnInfoCollectionChanged));
			GroupInfo.AutoExpandAllGroups = controller.AutoExpandGroups;
			this.rowsKeeper = new PivotGroupRowsKeeper(this);
		}
		public abstract bool IsColumn { get; }
		public int VisibleCount { get { return VisibleIndexes.Count; } }
		public PivotColumnInfoCollection Columns { get { return columns; } }
		protected internal override DataColumnSortInfoCollection SortInfo {
			get {
				if(this.sortInfo == null) {
					this.sortInfo = new DataColumnSortInfoCollection(Controller, null);
				}
				return sortInfo;
			}
		}
		protected internal override PivotSummaryItemCollection Summaries { get { return summaries; } }
		protected override void PrepareSummaryItems() {
			this.summaries = CreateSummaryItems(CreateSummaryColumns());
		}
		public override void DoRefresh() {
			UpdateColumns();
			base.DoRefresh();
		}
		void UpdateColumns() {
			if(Columns.Count == 0) return;
			PivotColumnInfo[] newColumns = new PivotColumnInfo[Columns.Count];
			for(int i = 0; i < Columns.Count; i++) {
				newColumns[i] = Columns[i].Clone(Controller.Columns[Columns[i].ColumnInfo.Name]);
			}
			Columns.ClearAndAddRangeSilent(newColumns);
		}
		public int GetVisibleIndexByValues(object[] values) {
			GroupRowInfo groupInfo = GetGroupRowByValues(values);
			return groupInfo != null ? VisibleIndexes.IndexOf(groupInfo.Handle) : -1;
		}
		public int GetControllerRowHandle(int visibleIndex) {
			if(!IsReady || visibleIndex < 0 || visibleIndex >= VisibleCountCore) return DataController.InvalidRow;
			return VisibleIndexes.GetHandle(visibleIndex);
		}
		public GroupRowInfo GetGroupRowInfo(int visibleIndex) {
			int controllerRowHandle = GetControllerRowHandle(visibleIndex);
			return GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerRowHandle);
		}
		public int GetAbsoluteIndexByVisibleIndex(int visibleIndex) {
			int handle = GetControllerRowHandle(visibleIndex);
			return GroupRowInfo.HandleToGroupIndex(handle);
		}
		public IList<GroupRowInfo> GetGroupRows(int startIndex, int endIndex) {
			List<GroupRowInfo> result = new List<GroupRowInfo>();
			if(startIndex == -1) {
				GroupInfo.GetChildrenGroups(null, result);
			} else {
				int maxLevel = this.Columns.Count - 1,
					expandedStartIndex = GetControllerRowHandle(startIndex),
					expandedEndIndex = GetControllerRowHandle(endIndex);
				for(int i = expandedStartIndex; i > expandedEndIndex; i--) {
					GroupRowInfo groupRow = GroupInfo.GetGroupRowInfoByControllerRowHandle(i);
					if(groupRow == null || groupRow.Level != maxLevel) continue;
					result.Add(groupRow);
				}
			}
			return result;
		}
		public int GetPrevVisibleIndex(int visibleIndex) {
			return GetNextOrPrevVisibleIndex(visibleIndex, false);
		}
		public int GetNextVisibleIndex(int visibleIndex) {
			return GetNextOrPrevVisibleIndex(visibleIndex, true);
		}
		public int GetNextOrPrevVisibleIndex(int visibleIndex, bool isNext) {
			GroupRowInfo groupRow = GetGroupRowInfo(visibleIndex);
			if(groupRow != null) {
				groupRow = isNext ? GetNextGroupRow(groupRow) : GetPrevGroupRow(groupRow);
			}
			return groupRow != null ? VisibleIndexes.IndexOf(groupRow.Handle) : -1;
		}
		public object GetValue(int visibleIndex) {
			return GetValue(GetGroupRowInfo(visibleIndex));
		}
		public object GetValue(int visibleIndex, int columnIndex) {
			GroupRowInfo groupRow = GetGroupRowByColumnStrictly(visibleIndex, columnIndex);
			return groupRow != null ? GetValue(groupRow) : null;
		}
		public bool IsColumnValueExpanded(int visibleIndex, int columnIndex) {
			GroupRowInfo groupRow = GetGroupRowByColumn(visibleIndex, columnIndex);
			return groupRow != null ? groupRow.Expanded : false;
		}
		protected GroupRowInfo GetGroupRowByColumn(int visibleIndex, int groupLevel) {
			if(groupLevel < 0 || groupLevel >= SortInfo.Count) return null;
			GroupRowInfo groupRow = GetGroupRowInfo(visibleIndex);
			if(groupRow == null) return null;
			while(groupLevel < groupRow.Level)
				groupRow = groupRow.ParentGroup;
			return groupRow;
		}
		protected GroupRowInfo GetGroupRowByColumnStrictly(int visibleIndex, int groupLevel) {
			GroupRowInfo groupRow = GetGroupRowByColumn(visibleIndex, groupLevel);
			if(groupRow == null || groupLevel != groupRow.Level) return null;
			return groupRow;
		}
		public bool GetIsOthersValue(int visibleIndex) {
			return GetIsOthersValue(GetGroupRowInfo(visibleIndex));
		}
		public bool GetIsOthersValue(int visibleIndex, int levelIndex) {
			return GetIsOthersValue(GetGroupRowByColumnStrictly(visibleIndex, levelIndex));
		}
		protected VisibleIndexCollection VisibleIndexes { get { return visibleIndexes; } }
		protected PivotGroupRowsKeeper RowsKeeper { get { return rowsKeeper; } }
		protected int VisibleCountCore { get { return VisibleIndexes.IsEmpty ? VisibleListSourceRowCount : VisibleIndexes.Count; } }
		protected override void OnGroupInfoRecreated() {
			base.OnGroupInfoRecreated();
			if(VisibleIndexes != null) visibleIndexes.SetGroupInfo(GroupInfo);
		}
		public int AlwaysVisibleLevelIndex {
			get { return GroupInfo.AlwaysVisibleLevelIndex; }
			set {
				if(value < -1) value = -1;
				if(AlwaysVisibleLevelIndex == value) return;
				GroupInfo.AlwaysVisibleLevelIndex = value;
				BuildVisibleIndexes();
			}
		}
		public void SaveGroupRowsColumns() {
			RowsKeeper.SaveColumns();
		}
		public void SaveGroupRowsState() {
			RowsKeeper.SaveRows();
		}
		public void SaveFieldsStateToStream(Stream stream) {
			RowsKeeper.WriteToStream(stream);
		}
		public void RestoreGroupRowsState() {
			RowsKeeper.Restore();
			BuildVisibleIndexes();
		}
		public void LoadFieldsStateFromStream(Stream stream) {
			RowsKeeper.ReadFromStream(stream);
			RestoreGroupRowsState();
		}
		public void WebSaveFieldsStateToStream(Stream stream) {
			RowsKeeper.WebWriteToStream(stream);
		}
		public void WebLoadFieldsStateFromStream(Stream stream) {
			RowsKeeper.WebReadFromStream(stream);
			BuildVisibleIndexes();
		}
		public void ChangeFieldSortOrder(int index) {
			if(index < 0 || index > Columns.Count) return;
			SortInfo.ChangeGroupSorting(index);
			Columns.ChangeSortOrder(index);
			if(Columns[index].ShowTopRows > 0) {
				Controller.DoRefresh();
			} else {
				GroupInfo.ReverseLevel(index);
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
				BuildRowsIndex();
			}
		}
		public void ClearGroupRowsState() {
			RowsKeeper.Clear();
			GroupInfo.Clear();
		}
		public void ExpandAll() {
			ChangeAllExpanded(true);
		}
		public void CollapseAll() {
			ChangeAllExpanded(false);
		}
		public void ChangeAllExpanded(bool expanded) {
			bool hasChanged = false;
			for(int i = 0; i < Columns.Count - 1; i++) {
				hasChanged |= GroupInfo.ChangeLevelExpanded(i, expanded);
			}
			if(hasChanged) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public bool IsRowExpanded(int visibleIndex) {
			GroupRowInfo groupRow = this.GetGroupRowInfo(visibleIndex);
			return groupRow == null ? false : groupRow.Expanded;
		}
		public void ExpandRow(int visibleIndex) {
			ExpandRow(visibleIndex, false);
		}
		public void ExpandRow(int visibleIndex, bool recursive) {
			ChangeExpanded(visibleIndex, true, recursive);
		}
		public void CollapseRow(int visibleIndex) {
			CollapseRow(visibleIndex, false);
		}
		public void CollapseRow(int visibleIndex, bool recursive) {
			ChangeExpanded(visibleIndex, false, recursive);
		}
		public object[] GetRowValues(int visibleIndex) {
			return GetGroupRowValues(GetGroupRowInfo(visibleIndex));
		}
		public void CollapseColumn(int columnIndex) {
			ChangeColumnExpanded(columnIndex, false);
		}
		public void ExpandColumn(int columnIndex) {
			ChangeColumnExpanded(columnIndex, true);
		}
		public void CollapseColumn(int columnIndex, object value) {
			ChangeColumnExpanded(columnIndex, false, value);
		}
		public void ExpandColumn(int columnIndex, object value) {
			ChangeColumnExpanded(columnIndex, true, value);
		}
		public virtual void ChangeColumnExpanded(int columnIndex, bool expanded) {
			if(columnIndex >= Columns.Count) return;
			if(GroupInfo.ChangeLevelExpanded(columnIndex, expanded)) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public void ChangeColumnExpanded(object[] values) {
			GroupRowInfo groupRow = GetGroupRowByValues(values);
			if(groupRow != null) ChangeColumnExpanded(groupRow, !groupRow.Expanded);
		}
		public void ChangeColumnExpanded(object[] values, bool expanded) {
			GroupRowInfo groupRow = GetGroupRowByValues(values);
			if(groupRow != null) ChangeColumnExpanded(groupRow, expanded);
		}
		protected void ChangeColumnExpanded(GroupRowInfo groupRow, bool expanded) {
			if(groupRow == null || groupRow.Expanded == expanded) return;
			groupRow.Expanded = expanded;
			BuildVisibleIndexes();
			VisualClientUpdateLayout();
		}
		public void ChangeColumnExpanded(int columnIndex, bool expanded, object value) {
			if(columnIndex >= Columns.Count - 1) return;
			ArrayList list = GetGroupRowHandlesByGroupRowValue(columnIndex, value);
			bool hasChanged = false;
			for(int i = 0; i < list.Count; i++)
				hasChanged |= GroupInfo.ChangeExpanded((int)list[i], expanded, false);
			if(hasChanged) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		public override void Reset() {
			Columns.Clear();
			RowsKeeper.Clear();
			base.Reset();
		}
		public void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, GroupRowInfo groupRow, GroupRowInfo filterGroup) {
			IList<GroupRowInfo> groupList = GetSummaryRows(groupRow);
			CreateSummaryGroups(groups, dataHelpers, groupList, filterGroup);
		}
		public void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, int startIndex, int endIndex, int filterStartIndex, int filterEndIndex) {
			IList<GroupRowInfo> groupList = GetGroupRows(startIndex, endIndex);
			CreateSummaryGroups(groups, dataHelpers, groupList, filterStartIndex, filterEndIndex);
		}
		protected virtual void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, IList<GroupRowInfo> groupList, int filterStartIndex, int filterEndIndex) {
			CreateSummaryGroups(groups, dataHelpers, groupList, null);	
		}
		protected virtual void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, IList<GroupRowInfo> groupList, GroupRowInfo filterGroup) {
			for(int i = 0; i < groupList.Count; i++) {
				groups.Add(groupList[i]);
				dataHelpers.Add(this);
			}
		}
		protected IList<GroupRowInfo> GetSummaryRows(GroupRowInfo groupRow) {
			List<GroupRowInfo> rowGroups = new List<GroupRowInfo>();
			int rowLevel = GroupInfo.LevelCount - 1;
			if(groupRow != null && groupRow.Level == rowLevel) {
				rowGroups.Add(groupRow);
			} else {
				GroupInfo.GetChildrenGroups(groupRow, rowGroups, rowLevel);
			}
			return rowGroups;
		}
		protected override void BuildVisibleIndexes() {
			if(GroupInfo.Count > 0)
				VisibleIndexes.BuildVisibleIndexes(VisibleCountCore, false, false);
			else
				VisibleIndexes.Clear();
		}
		protected override PivotColumnInfo[] CreateSummaryColumns() { return Columns.ToArray(); }
		protected virtual ArrayList GetGroupRowHandlesByGroupRowValue(int columnIndex, object value) {
			ArrayList list = new ArrayList();
			int columnInfoIndex = Columns[columnIndex].ColumnInfo.Index;
			for(int i = 0; i < GroupInfo.Count; i++) {
				if(GroupInfo[i].Level != columnIndex) continue;
				if(Controller.IsObjectEqualsFromHelper(GroupInfo, GroupInfo[i].ChildControllerRow, columnInfoIndex, value))
					list.Add(GroupInfo[i].Handle);
			}
			return list;
		}
		protected override void ClearVisialIndexesAndGroupInfo() {
			base.ClearVisialIndexesAndGroupInfo();
			VisibleIndexes.Clear();
			PrepareSortInfo();
		}
		protected virtual void PrepareSortInfo() {
			SortInfo.Clear();
			AddColumnsToSortInfo(Columns);
			GroupInfo.LastExpandableLevel = Columns.Count - 1;
		}
		public virtual void ChangeExpanded(int visibleIndex, bool expanded, bool recursive) {
			int groupRowHandle = GetControllerRowHandle(visibleIndex);
			if(GroupInfo.ChangeExpanded(groupRowHandle, expanded, recursive)) {
				BuildVisibleIndexes();
				VisualClientUpdateLayout();
			}
		}
		protected virtual void VisualClientUpdateLayout() {
			Controller.VisualClientUpdateLayout();
		}
		protected void OnColumnInfoCollectionChanged(object sender, CollectionChangeEventArgs e) {
			Controller.DoRefresh();
		}
	}
	public class ColumnPivotDataControllerArea : PivotDataControllerArea {
		public ColumnPivotDataControllerArea(PivotDataController controller) : base(controller) { }
		public override bool IsColumn { get { return true; } }
		protected override GroupRowInfo GetPrevColumnGroupRow(GroupRowInfo groupRow) {
			return GetPrevGroupRow(groupRow, 0);
		}
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return Columns[groupRow.Level];
		}
		protected override PivotGroupRowInfo GetColumnGroup(GroupRowInfo groupInfo) {
			return (PivotGroupRowInfo)groupInfo;
		}
	}
	public class RowPivotDataControllerArea : PivotDataControllerArea {
		PivotSummaryItemCollection rowGroupsSummaries;
		PivotColumnInfo[] summaryColumns;
		public RowPivotDataControllerArea(PivotDataController controller)
			: base(controller) {
			this.rowGroupsSummaries = null;
			this.summaryColumns = new PivotColumnInfo[0];
		}
		public override bool IsColumn { get { return false; } }
		protected override PivotGroupRowInfoCollection CreateGroupRowInfoCollection() {
			return new PivotGroupRowInfoCollection(Controller, SortInfo, VisibleListSourceRows);
		}
		public PivotSummaryItemCollection RowGroupsSummaries { get { return rowGroupsSummaries; } }
		public PivotColumnInfo[] SummaryColumns { get { return summaryColumns; } }
		public DataControllerRowGroupInfo GetControllerRowGroup(GroupRowInfo groupRow, bool firstPass) {
			DataControllerRowGroupInfo controllerGroupInfo = (groupRow as PivotGroupRowInfo).ControllerRowGroupInfo;
			return controllerGroupInfo != null ? controllerGroupInfo : CreateControllerGroupRow(groupRow, firstPass);
		}
		protected DataControllerRowGroupInfo CreateControllerGroupRow(GroupRowInfo groupRow, bool firstPass) {
			DataControllerRowGroupInfo controllerGroupInfo = new DataControllerRowGroupInfo(Controller, groupRow);
			SetControllerRowGroup(groupRow, controllerGroupInfo);
			controllerGroupInfo.DoRefresh(firstPass);
			return controllerGroupInfo;
		}
		protected void SetControllerRowGroup(GroupRowInfo groupRow, DataControllerRowGroupInfo value) {
			(groupRow as PivotGroupRowInfo).ControllerRowGroupInfo = value;
		}
		public void ClearControllerRowGroups() {
			for(int i = 0; i < GroupInfo.Count; i++) {
				SetControllerRowGroup(GroupInfo[i], null);
			}
		}
		protected PivotDataControllerArea ColumnArea { get { return Controller.ColumnArea; } }
		protected override PivotColumnInfo[] CreateSummaryColumns() {
			if(summaryColumns.Length > 0)
				return summaryColumns;
			ArrayList list = new ArrayList();
			for(int i = 0; i < Columns.Count; i++)
				list.Add(Columns[i]);
			for(int i = 0; i < ColumnArea.Columns.Count; i++)
				list.Add(ColumnArea.Columns[i]);
			summaryColumns = (PivotColumnInfo[])list.ToArray(typeof(PivotColumnInfo));
			return summaryColumns;
		}
		protected override void UpdateGroupSummaryCore() {
			summaryColumns = new PivotColumnInfo[0];
			base.UpdateGroupSummaryCore();
		}
		protected override int GetRowGroupCount() { return Columns.Count; }
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return groupRow.Level >= Columns.Count ? ColumnArea.Columns[groupRow.Level - Columns.Count] : null;
		}
		public override PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			if(groupRow.Level < Columns.Count) return Columns[groupRow.Level];
			return Columns.Count > 0 ? Columns[Columns.Count - 1] : null;
		}
		public override void DoRefresh() {
			base.DoRefresh();
			CreateGroupInfos();
		}
		protected void CreateGroupInfos() {
			if(ColumnArea.Columns.Count == 0) {
				this.rowGroupsSummaries = null;
			} else {
				this.rowGroupsSummaries = CreateSummaryItems(ColumnArea.Columns.ToArray());
			}
		}
		public override void UpdateGroupSummary() {
			base.UpdateGroupSummary();
			if(ColumnArea.Columns.Count > 0) {
				this.rowGroupsSummaries = CreateSummaryItems(ColumnArea.Columns.ToArray());
				for(int i = 0; i < GroupInfo.Count; i++) {
					SetControllerRowGroup(GroupInfo[i], null);
				}
			}
		}
		public GroupRowInfo GetSummaryGroupRow(GroupRowInfo groupRow, object[] values, bool firstPass,
				out VisibleListSourceRowCollection cellVisibleListSourceRows) {
			DataControllerRowGroupInfo controllerGroupInfo = GetControllerRowGroup(groupRow, firstPass);
			GroupRowInfo prevGroupRow = GetPrevGroupRow(groupRow, 0);
			if(prevGroupRow != null) {
				if(GetControllerRowGroup(prevGroupRow, firstPass) == null) {
					controllerGroupInfo.UpdateGroupSummary();
				}
			}
			cellVisibleListSourceRows = controllerGroupInfo.VisibleListSourceRows; 
			return controllerGroupInfo.GetGroupRowByValues(values);
		}
		protected override void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, IList<GroupRowInfo> groupList, GroupRowInfo filterGroup) {
			if(ColumnArea.Columns.Count > 0) {
				for(int i = 0; i < groupList.Count; i++)
					CreateSummaryGroups(groups, dataHelpers, groupList[i] as PivotGroupRowInfo, filterGroup);
			} else
				base.CreateSummaryGroups(groups, dataHelpers, groupList, filterGroup);
		}
		protected override void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, IList<GroupRowInfo> groupList, int filterStartIndex, int filterEndIndex) {
			if(ColumnArea.Columns.Count > 0) {
				for(int i = 0; i < groupList.Count; i++)
					CreateSummaryGroups(groups, dataHelpers, groupList[i] as PivotGroupRowInfo, filterStartIndex, filterEndIndex);
			} else
				base.CreateSummaryGroups(groups, dataHelpers, groupList, filterStartIndex, filterEndIndex);
		}
		void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, PivotGroupRowInfo pivotGroupRow, GroupRowInfo filterGroup) {
			GetControllerRowGroup(pivotGroupRow, false).CreateSummaryGroupRows(groups, dataHelpers, filterGroup);
		}
		void CreateSummaryGroups(ArrayList groups, ArrayList dataHelpers, PivotGroupRowInfo pivotGroupRow, int filterStartIndex, int filterEndIndex) {
			if(pivotGroupRow == null) return;
			if(filterStartIndex == -1) {
				groups.Add(pivotGroupRow);
				dataHelpers.Add(this);
				return;
			}
			GetControllerRowGroup(pivotGroupRow, false).CreateSummaryGroupRows(groups, dataHelpers, filterStartIndex, filterEndIndex);
		}
		protected override PivotGroupRowInfo GetRowGroup(GroupRowInfo groupInfo) {
			return (PivotGroupRowInfo)groupInfo;
		}
	}
}
