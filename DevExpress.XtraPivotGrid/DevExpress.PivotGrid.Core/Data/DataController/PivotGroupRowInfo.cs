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
using System.Collections.Generic;
using DevExpress.Data.Helpers;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Data.PivotGrid {
	public class DataControllerRowGroupInfo : DataControllerGroupHelperBase {
		GroupRowInfo parentGroupRow;
		PivotColumnInfo[] summaryColumns;
		public DataControllerRowGroupInfo(PivotDataController controller, GroupRowInfo parentGroupRow)
			: base(controller) {
			this.parentGroupRow = parentGroupRow;
		}
		public GroupRowInfo ParentGroupRow { get { return parentGroupRow; } }
		protected PivotDataControllerArea ColumnArea { get { return Controller.ColumnArea; } }
		protected RowPivotDataControllerArea RowArea { get { return Controller.RowArea; } }
		protected internal override PivotSummaryItemCollection Summaries { get { return RowArea.RowGroupsSummaries; } }
		protected override PivotColumnInfo[] CreateSummaryColumns() {
			if(summaryColumns != null && summaryColumns.Length > 0)
				return summaryColumns;
			ArrayList list = new ArrayList();
			for(int i = 0; i < ColumnArea.Columns.Count; i++)
				list.Add(ColumnArea.Columns[i]);
			summaryColumns = (PivotColumnInfo[])list.ToArray(typeof(PivotColumnInfo));
			return summaryColumns;
		}
		public void DoRefresh(bool firstPass) {
			DoRefresh();
			DoAddOthers(firstPass);
		}
		public void DoAddOthers(bool firstPass) {
			bool showTopRows;
			IComparer<GroupRowInfo>[] comparers = Controller.GetComparers(ColumnArea, RowArea, firstPass, out showTopRows);
			if(comparers != null || showTopRows) {
				bool refreshRequired = DoConditionalSortSummaryAndAddOthers(comparers, firstPass);
				if(refreshRequired && !firstPass)
					throw new Exception("Unexpected refresh required");
			}
		}
		protected override void DoConditionSortListCore(GroupRowInfo parentGroup, PivotColumnInfo pivotColumnInfo, List<GroupRowInfo> list, IComparer<GroupRowInfo> comparer) {
			GroupRowInfo columnAreaGroup = GetColumnAreaGroup(parentGroup);
			object[] childColumnAreaValues = GetColumnAreaValues(columnAreaGroup);
			comparer = new PivotGroupValueComparer(this, pivotColumnInfo.SortOrder, childColumnAreaValues);
			base.DoConditionSortListCore(parentGroup, pivotColumnInfo, list, comparer);
		}
		object[] GetColumnAreaValues(GroupRowInfo columnAreaGroup) {
			PivotDataControllerArea columnArea = ColumnArea;
			List<GroupRowInfo> childColumnAreaGroups = new List<GroupRowInfo>();
			columnArea.GroupInfo.GetChildrenGroups(columnAreaGroup, childColumnAreaGroups);
			object[] childColumnAreaValues = new object[childColumnAreaGroups.Count];
			for(int i = 0; i < childColumnAreaGroups.Count; i++) {
				childColumnAreaValues[i] = columnArea.GetValue(childColumnAreaGroups[i]);
			}
			return childColumnAreaValues;
		}
		GroupRowInfo GetColumnAreaGroup(GroupRowInfo parentGroup) {
			if(parentGroup == null)
				return null;
			object[] parentValues = GetGroupValues(parentGroup);
			return ColumnArea.GetGroupRowByValues(parentValues);
		}
		object[] GetGroupValues(GroupRowInfo group) {
			object[] res = new object[group.Level + 1];
			while(group != null) {
				res[group.Level] = GetValue(group);
				group = group.ParentGroup;
			}
			return res;
		}
		protected override void DoSortRows() {
			if(!IsSorted) return;
			int[] list = new int[ParentGroupRow.ChildControllerRowCount];
			for(int i = 0; i < list.Length; i++) {
				list[i] = RowArea.GetListSourceRowByControllerRow(ParentGroupRow.ChildControllerRow + i);
			}
			Controller.SetVisibleListSourceCollection(VisibleListSourceRows, list, list.Length);
			DoSortRowsCore();
		}
		protected override int GetGroupedCount(GroupRowInfoCollection groups, GroupRowInfo parentGroup, bool useCountCache, int level, PivotColumnInfo pivotColumnInfo, IList<GroupRowInfo> list, bool firstPass) {
			int groupedCount = base.GetGroupedCount(groups, parentGroup, useCountCache, level, pivotColumnInfo, list, firstPass);
			if(!summaryColumns[level].ShowOthersValue)
				return groupedCount;
			object[] parentValues = ColumnArea.GetGroupRowValues(parentGroup);
			object[] values = new object[parentValues.Length + 1];
			Array.Copy(parentValues, values, parentValues.Length);
			for(int i = 0; i < groupedCount; i++) {
				GroupRowInfo group = (GroupRowInfo)list[i];
				values[values.Length - 1] = GetValue(group);
				GroupRowInfo columnGroup = ColumnArea.GetGroupRowByValues(values);
				if(columnGroup == null || ColumnArea.GetIsOthersValue(columnGroup))
					return i;
			}
			return groupedCount;
		}
		public override PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return ColumnArea.Columns[groupRow.Level];
		}
		public override PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			return RowArea.GetRowColumnByGroupRow(ParentGroupRow);
		}
		protected internal override DataColumnSortInfoCollection SortInfo { get { return ColumnArea.SortInfo; } }
		GroupRowInfo GetLastGroupRow(int level) {
			for(int i = GroupInfo.Count - 1; i >= 0; i--) {
				if(GroupInfo[i].Level == level)
					return GroupInfo[i];
			}
			return null;
		}
		public void CreateSummaryGroupRows(ArrayList groups, ArrayList dataHelpers, int filterStartIndex, int filterEndIndex) {
			if(filterStartIndex == -1) {
				groups.Add(null);
				dataHelpers.Add(this);
				return;
			}
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			GroupInfo.GetChildrenGroups(null, list, GroupInfo.LevelCount - 1);
			for(int i = 0; i < list.Count; i++) {
				GroupRowInfo groupInfo = (GroupRowInfo)list[i];
				int visibleIndex = ColumnArea.GetVisibleIndexByValues(GetGroupRowValues(groupInfo));
				if(visibleIndex >= filterStartIndex && visibleIndex < filterEndIndex) {
					groups.Add(list[i]);
					dataHelpers.Add(this);
				}
			}
		}
		public void CreateSummaryGroupRows(ArrayList groups, ArrayList dataHelpers, GroupRowInfo filterGroup) {
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			GroupInfo.GetChildrenGroups(null, list, GroupInfo.LevelCount - 1);
			object[] filterValues = filterGroup != null ? ColumnArea.GetGroupRowValues(filterGroup) : null;
			for(int i = 0; i < list.Count; i++) {
				if(IsGroupFit(list[i] as GroupRowInfo, filterValues)) {
					groups.Add(list[i]);
					dataHelpers.Add(this);
				}
			}
		}
		bool IsGroupFit(GroupRowInfo groupRow, object[] filterValues) {
			if(filterValues == null) return true;
			object[] values = GetGroupRowValues(groupRow);
			for(int i = 0; i < filterValues.Length; i++) {
				if(!Controller.IsEqualGroupValues(values[i], filterValues[i])) return false;
			}
			return true;
		}
		protected override PivotGroupRowInfo GetRowGroup(GroupRowInfo groupInfo) {
			return (PivotGroupRowInfo)ParentGroupRow;
		}
		protected override PivotGroupRowInfo GetColumnGroup(GroupRowInfo groupInfo) {
			return (PivotGroupRowInfo)groupInfo;
		}
	}
	public class PivotGroupRowInfo : GroupRowInfo {
		DataControllerRowGroupInfo controllerRowGroupInfo = null;
		public PivotGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroup)
			: base(level, childControllerRow, parentGroup) {
		}
		public DataControllerRowGroupInfo ControllerRowGroupInfo {
			get { return controllerRowGroupInfo; }
			set { controllerRowGroupInfo = value; }
		}
		public bool HasSummary { get { return Summary != null; } }
	}
	public class PivotGroupRowInfoCollection : GroupRowInfoCollection {
		public PivotGroupRowInfoCollection(DataControllerBase controller, DataColumnSortInfoCollection sortInfo,
			VisibleListSourceRowCollection visibleListSourceRows)
			: base(controller, sortInfo, visibleListSourceRows) {
		}
		protected override GroupRowInfo CreateGroupRowInfo(byte level, int childControllerRow, GroupRowInfo parentGroupRow) {
			return new PivotGroupRowInfo(level, childControllerRow, parentGroupRow);
		}
		public override void ReverseLevel(int level) {
			ReverseLevelCore(level);
		}
	}
	public class PivotVisibleIndexCollection : VisibleIndexCollection {
		public PivotVisibleIndexCollection(DataControllerBase controller, GroupRowInfoCollection groupInfo) : base(controller, groupInfo) { }
		internal void SetGroupInfo(GroupRowInfoCollection newGroupInfo) { GroupInfo = newGroupInfo; }
	}
}
