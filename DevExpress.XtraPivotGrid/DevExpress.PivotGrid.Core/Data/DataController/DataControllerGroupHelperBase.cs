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
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
#if !SL
#if DXRESTRICTED
using PD = DevExpress.Compatibility.System.ComponentModel;
#else
using PD = System.ComponentModel;
#endif
#else
using DevExpress.Xpf.Collections;
using PD = DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.PivotGrid {
	class ChildGroupsCountCache : IDisposable {
#if !SL && !DXPORTABLE
		SortedList<int, int>[] childGroupsCount;
#else
		Dictionary<int, int>[] childGroupsCount;
#endif
		int level0GroupsCount;
		public ChildGroupsCountCache(int columnCount, GroupRowInfoCollection groups) {
#if !SL && !DXPORTABLE
			this.childGroupsCount = new SortedList<int, int>[columnCount];
#else
			this.childGroupsCount = new Dictionary<int, int>[columnCount];
#endif
			for(int i = 0; i < columnCount; i++)
#if !SL && !DXPORTABLE
				this.childGroupsCount[i] = new SortedList<int, int>();
#else
				this.childGroupsCount[i] = new Dictionary<int, int>();
#endif
			this.level0GroupsCount = groups.GetTotalGroupsCountByLevel(0);
			for(int i = 0; i < groups.Count; i++) {
				GroupRowInfo row = groups[i];
				int childCount = groups.GetChildrenGroupCount(row);
				for(int j = 0; j < row.ChildControllerRowCount; j++) {
					int listIndex = groups.VisibleListSourceRows.GetListSourceRow(row.ChildControllerRow + j);
					this.childGroupsCount[row.Level].Add(listIndex, childCount);
				}
			}
		}
		public int this[int level, int listIndex] {
			get {
				if(level == -1)
					return this.level0GroupsCount;
				else
					return this.childGroupsCount[level][listIndex];
			}
		}
		#region IDisposable Members
		protected bool IsDisposed { get { return this.childGroupsCount == null; } }
		public void Dispose() {
			if(IsDisposed) return;
			for(int i = 0; i < childGroupsCount.Length; i++)
				childGroupsCount[i] = null;
			childGroupsCount = null;
			GC.SuppressFinalize(this);
		}
		~ChildGroupsCountCache() {
			if(!IsDisposed) Dispose();
		}
		#endregion
	}
	public class PivotVisibleListSourceRowCollection: VisibleListSourceRowCollection {
		public PivotVisibleListSourceRowCollection(PivotDataController controller) : base(controller) {
		}
		protected new PivotDataController controller { get { return (PivotDataController)base.controller; } }
		#region Pivot records hack
		protected int this[int index] {
			get { return GetListSourceRow(index); }
			set { GetMapperForSetRange().SetValue(index, value); }
		}
		protected PivotVisibleListSourceRowCollection records { get { return this; } }
		#endregion Pivot records hack
		public void Sort(DataColumnSortInfoCollection sortInfo) {
			if(VisibleRowCount == 0 || sortInfo == null || controller == null) return;
			this.controller.FillRequireSortCell(sortInfo);
			if(controller.ForceHeapSort || !QuickSort(sortInfo.ToArray(), 0, VisibleRowCount - 1, 0, true))
				HeapSort(sortInfo.ToArray());
		}
		void HeapSort(DataColumnSortInfo[] sortInfo) {
			int n;
			int i;
			n = VisibleRowCount;
			for(i = n / 2; i > 0; i--) {
				DownHeap(sortInfo, i, n);
			}
			do {
				Swap(0, n - 1);
				n = n - 1;
				DownHeap(sortInfo, 1, n);
			} while(n > 1);
		}
		void Swap(int i, int j) {
			int r = this.records[i];
			this.records[i] = this.records[j];
			this.records[j] = r;
		}
		void DownHeap(DataColumnSortInfo[] sortInfo, int k, int n) {
			int j;
			bool loop = true;
			while((k <= n / 2) && loop) {
				j = k + k;
				if(j < n) {
					if(this.controller.CompareRecords(sortInfo, this.records[j - 1], this.records[j], true) < 0) {
						j++;
					}
				}
				if(this.controller.CompareRecords(sortInfo, this.records[k - 1], this.records[j - 1], true) >= 0) {
					loop = false;
				} else {
					Swap(k - 1, j - 1);
					k = j;
				}
			}
		}
		const int MaxQuickSortDepth = 500;
		protected internal bool QuickSort(DataColumnSortInfo[] sortInfo, int left, int right, int recurse, bool useStorage) {
			if(recurse > MaxQuickSortDepth) return false;
			int i, j;
			int record;
			do {
				i = left;
				j = right;
				record = this.records[i + j >> 1];
				do {
					while(i <= j && this.controller.CompareRecords(sortInfo, this.records[i], record, useStorage) < 0) i++;
					while(j >= 0 && this.controller.CompareRecords(sortInfo, this.records[j], record, useStorage) > 0) j--;
					if(i <= j) {
						int r = this.records[i];
						this.records[i] = this.records[j];
						this.records[j] = r;
						i++;
						j--;
					}
				} while(i <= j);
				if(left < j) {
					if(!QuickSort(sortInfo, left, j, recurse < 0 ? -1 : recurse + 1, useStorage)) return false;
				}
				left = i;
			} while(i < right);
			return true;
		}
	}
	public abstract class DataControllerGroupHelperBase : IEvaluatorDataAccess {
		PivotDataController controller;
		GroupRowInfoCollection groupInfo;
		PivotVisibleListSourceRowCollection visibleListSourceRows;
		Hashtable othersRows;
		protected internal readonly static object OthersValue = "XtraPivotGridOthersValue";
		protected internal readonly static object NullValue = new object();
		public DataControllerGroupHelperBase(PivotDataController controller) {
			this.controller = controller;
			this.visibleListSourceRows = new PivotVisibleListSourceRowCollection(Controller);
			this.groupInfo = CreateGroupRowInfoCollection();
			this.othersRows = null;
		}
		public PivotDataController Controller { get { return controller; } }
		public IDataControllerSort SortClient { get { return Controller.SortClient; } }
		public int GetListSourceRowByControllerRow(int row) { return Controller.GetListSourceRowByControllerRow(VisibleListSourceRows, row); }
		protected internal abstract DataColumnSortInfoCollection SortInfo { get; }
		protected abstract PivotColumnInfo[] CreateSummaryColumns();
		protected internal abstract PivotSummaryItemCollection Summaries { get; }
		protected virtual PivotGroupRowInfoCollection CreateGroupRowInfoCollection() {
			return new PivotGroupRowInfoCollection(Controller, SortInfo, VisibleListSourceRows);
		}
		public GroupRowInfoCollection GroupInfo {
			get { return groupInfo; }
			set {
				this.groupInfo = value;
				OnGroupInfoRecreated();
			}
		}
		public PivotVisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		public object GetValue(GroupRowInfo groupRow) {
			if(groupRow == null) return null;
			if(GetIsOthersValue(groupRow)) return OthersValue;
			DataColumnInfo columnInfo = SortInfo[groupRow.Level].ColumnInfo;
			if(columnInfo == null) return null;
			return GetValueCore(groupRow, columnInfo);
		}
		public object GetValue(GroupRowInfo groupRow, DataColumnInfo columnInfo) {
			if(groupRow == null) return null;
			if(GetIsOthersValue(groupRow)) return OthersValue;
			return GetValueCore(groupRow, columnInfo);
		}
		object GetValueCore(GroupRowInfo groupRow, DataColumnInfo columnInfo) {
			int controllerRow = groupRow.ChildControllerRow + groupRow.ChildControllerRowCount - 1; 
			return Controller.GetRowValueFromHelper(GroupInfo, controllerRow, columnInfo.Index);
		}
		public bool GetIsOthersValue(GroupRowInfo groupRow) {
			if(groupRow == null || this.othersRows == null) return false;
			return this.othersRows.Contains(groupRow);
		}
		public bool IsSorted { get { return SortInfo.Count > 0; } }
		public bool IsGrouped { get { return SortInfo.GroupCount > 0; } }
		public virtual void Reset() {
			Controller.ResetSortInfoCollection(SortInfo);
			this.GroupInfo.ClearSummary();
		}
		public virtual void DoRefresh() {
			ClearVisialIndexesAndGroupInfo();
			DoSortRows();
			DoGroupRows();
			CalcSummary();
			BuildVisibleIndexes();
		}
		protected virtual void ClearVisialIndexesAndGroupInfo() {
			this.othersRows = null;
			this.rootIndex = null;
			this.rowsIndex = null;
			VisibleListSourceRows.Clear();
			GroupInfo.Clear();
		}
		protected virtual void DoSortRows() {
			if(!IsSorted) return;
			VisibleListSourceRows.Assign(Controller.VisibleListSourceRows.ToArray());
			DoSortRowsCore();
		}
		protected void DoSortRowsCore() {
			if(!Controller.CacheData) {
				Controller.CreateColumnStorages(SortInfo, VisibleListSourceRows);
			}
			DoSortRowsCore(SortInfo, 0, VisibleListSourceRows.VisibleRowCount - 1, true);
			if(!Controller.CacheData) {
				SortInfo.ClearColumnStorages();
			}
		}
		protected virtual void DoSortRowsCore(DataColumnSortInfoCollection sortInfo, int left, int right, bool useStorage) {
			if(left >= right) return;
			if(SortClient != null) SortClient.BeforeSorting();
			try {
				if(!useStorage || left != 0 || right != VisibleListSourceRows.VisibleRowCount - 1)
					Controller.VisibleListSourceCollectionQuickSort(VisibleListSourceRows, sortInfo, left, right, useStorage);
				else {
					if(Controller.CachedHelper != null)
						Controller.CachedHelper.EnsureStorageIsCreated(sortInfo);
					VisibleListSourceRows.Sort(sortInfo.Clone());
				}
			} finally {
				if(SortClient != null) SortClient.AfterSorting();
			}
		}
		protected virtual void DoGroupRows() {
			if(!IsGrouped) return;
			DoGroupRowsCore(0, VisibleListSourceRowCount, null);
			DoSetHelperCompareCache();
		}
		protected virtual void DoGroupRowsCore(int controllerRow, int rowCount, GroupRowInfo parentGroupRow) {
			if(SortClient != null) SortClient.BeforeGrouping();
			try {
				Controller.DoGroupColumn(SortInfo, GroupInfo, controllerRow, rowCount, parentGroupRow);
				GroupInfo.UpdateIndexes();
				BuildRowsIndex();
				BuildVisibleIndexes();
			} finally {
				if(SortClient != null) SortClient.AfterGrouping();
			}
		}
		Hashtable rootIndex;
		Hashtable[] rowsIndex;
		protected void BuildRowsIndex() {
			if (controller.CaseSensitive)
				this.rootIndex = new Hashtable();
			else
				this.rootIndex = new Hashtable(StringExtensions.ComparerInvariantCultureIgnoreCase);
			rowsIndex = new Hashtable[GroupInfo.Count];
			int start = 0;
			BuildRowsIndex(ref start, 0, rootIndex);
		}
		void BuildRowsIndex(ref int start, int level, Hashtable list) {
			int columnInfoIndex = SortInfo[level].ColumnInfo.Index;
			int count = GroupInfo.Count;
			for(; start < count; start++) {
				GroupRowInfo gri = GroupInfo[start];
				if(gri.Level == level) {
					object value = GetValue(gri);
					if(value == null) value = NullValue;
					if(!list.Contains(value)) list.Add(value, start);
				}
				if(gri.Level > level) {
					Hashtable child = new Hashtable();
					rowsIndex[start - 1] = child;
					BuildRowsIndex(ref start, level + 1, child);
					start--;
				}
				if(gri.Level < level)
					break;
			}
		}
		protected void DoSetHelperCompareCache() {
			if(!IsGrouped || (ListSourceRowCount != VisibleListSourceRows.VisibleRowCount)) return;
			int columnIndex = SortInfo[0].ColumnInfo.Index;
			if(!Controller.SupportComparerCache(columnIndex) || Controller.HasComparerCache(columnIndex)) return;
			int[] cache = new int[ListSourceRowCount];
			List<GroupRowInfo> rootGroups = new List<GroupRowInfo>();
			GroupInfo.GetChildrenGroups(null, rootGroups);
			for(int i = rootGroups.Count - 1; i >= 0; i--) {
				GroupRowInfo prevGroup = (i > 0) ? rootGroups[i - 1] : null;
				DoSetHelperComparerArrayCacheByGroup(rootGroups[i], prevGroup, cache);
			}
			Controller.SetComparerCache(columnIndex, cache, SortInfo[0].SortOrder == ColumnSortOrder.Ascending);
		}
		void DoSetHelperComparerArrayCacheByGroup(GroupRowInfo groupRow, GroupRowInfo prevGroup, int[] cache) {
			int startControllerRow = (prevGroup == null) ? 0 : prevGroup.ChildControllerRow + prevGroup.ChildControllerRowCount;
			int endControllerRow = groupRow.ChildControllerRow + groupRow.ChildControllerRowCount - 1;
			for(int i = startControllerRow; i <= endControllerRow; i++) {
				int listRowIndex = Controller.GetListSourceRowIndex(GroupInfo, i);
				cache[listRowIndex] = groupRow.Index;
			}
		}
		protected virtual void BuildVisibleIndexes() {
		}
		protected internal GroupRowInfo GetSummaryGroupRow(GroupRowInfo groupRow, object[] values) {
			if(groupRow == null) return null;
			int count = GroupInfo.GetTotalChildrenGroupCount(groupRow);
			int equaledLevel = -1;
			int i = 0;
			while(i < count) {
				GroupRowInfo summaryGroupRow = GroupInfo[groupRow.Index + 1 + i];
				if(Controller.IsEqualGroupValues(
					values[summaryGroupRow.Level - groupRow.Level - 1], GetValue(summaryGroupRow))) {
					if(values.Length == summaryGroupRow.Level - groupRow.Level)
						return summaryGroupRow;
					equaledLevel = summaryGroupRow.Level;
					i++;
				} else {
					if(equaledLevel >= summaryGroupRow.Level) return null;
					i += GroupInfo.GetTotalChildrenGroupCount(summaryGroupRow) + 1;
				}
			}
			return null;
		}
		protected virtual void UpdateGroupSummaryCore() {
			GroupInfo.ClearSummary();
			PrepareSummaryItems();
			CalcGroupSummaries(0, GroupInfo.Count - 1);
		}
		protected virtual void PrepareSummaryItems() { }
		protected PivotSummaryItemCollection CreateSummaryItems(PivotColumnInfo[] columns) {
			PivotSummaryItemCollection summaries = new PivotSummaryItemCollection(Controller, null);
			summaries.ClearAndAddRange(Controller.Summaries);
			AddSummariesItems(summaries, columns);
			return summaries;
		}
		protected void AddSummariesItems(PivotSummaryItemCollection summaries, PivotColumnInfo[] columns) {
			for(int i = 0; i < columns.Length; i++) {
				if(columns[i].SortbyColumn != null && !summaries.Contains(columns[i].SortbyColumn)) {
					summaries.Add(PivotSummaryItem.CreateSummaryItem(columns[i].SortbyColumn, columns[i].SummaryType));
				}
			}
		}
		protected void CalcGroupSummaries(int startGroupIndex, int endGroupIndex) {
			CalcNotExpressionSummaries(startGroupIndex, endGroupIndex);
			CalcExpressionSummaries(startGroupIndex, endGroupIndex);
		}
		void CalcNotExpressionSummaries(int startGroupIndex, int endGroupIndex) {
			List<PivotSummaryItem> notExpSummaries = new List<PivotSummaryItem>();
			foreach(PivotSummaryItem summary in Summaries)
				if(summary.CalculationMode != SummaryItemCalculationMode.Expression)
					notExpSummaries.Add(summary);
			for(int i = 0; i < notExpSummaries.Count; i++)
				CalcGroupSummary(notExpSummaries[i], startGroupIndex, endGroupIndex);
		}
		Stack<PivotSummaryExpressionItem> expCalcStack;
		Dictionary<PivotSummaryExpressionItem, bool> isExpCalculated;
		void CalcExpressionSummaries(int startGroupIndex, int endGroupIndex) {
			isExpCalculated = new Dictionary<PivotSummaryExpressionItem, bool>();
			expCalcStack = new Stack<PivotSummaryExpressionItem>();
			List<PivotSummaryExpressionItem> expSummaries = new List<PivotSummaryExpressionItem>();
			foreach(PivotSummaryItem summary in Summaries) {
				PivotSummaryExpressionItem expSummary = summary as PivotSummaryExpressionItem;
				if(expSummary != null) {
					expSummaries.Add(expSummary);
					isExpCalculated[expSummary] = false;
				}
			}
			for(int i = 0; i < expSummaries.Count; i++) {
				expCalcStack.Clear();
				PivotSummaryExpressionItem summary = expSummaries[i];
				if(!isExpCalculated[summary])
					CalcExpressionSummary(summary, startGroupIndex, endGroupIndex);
			}
		}
		bool CalcExpressionSummary(PivotSummaryExpressionItem expSummary, int startGroupIndex, int endGroupIndex) {
			if(expSummary.HasSummaryRelations) {
				for(int i = 0; i < expSummary.SummaryRelations.Count; i++) {
					PivotSummaryItem relatedSummary = expSummary.SummaryRelations[i];
					PivotSummaryExpressionItem expRelatedSummary = relatedSummary as PivotSummaryExpressionItem;
					if(expRelatedSummary == null)
						continue;
					if(relatedSummary == null || expCalcStack.Contains(expRelatedSummary) && !isExpCalculated[expRelatedSummary])
						return false;
					bool isCalculated;
					if(!isExpCalculated.TryGetValue(expRelatedSummary, out isCalculated) || !isCalculated) {
						expCalcStack.Push(expRelatedSummary);
						bool isSuccess = CalcExpressionSummary(expRelatedSummary, startGroupIndex, endGroupIndex);
						if(!isSuccess) {
							SetErrorGroupRowSummary(expRelatedSummary, startGroupIndex, endGroupIndex);
							isExpCalculated[expRelatedSummary] = true;
							if(expCalcStack.Count != 0) {
								expCalcStack.Pop();
							} else {
								SetErrorGroupRowSummary(expSummary, startGroupIndex, endGroupIndex);
								isExpCalculated[expSummary] = true;
							}
							return false;
						}
					}
				}
			}
			CalcGroupSummary(expSummary, startGroupIndex, endGroupIndex);
			isExpCalculated[expSummary] = true;
			return true;
		}
		void SetErrorGroupRowSummary(PivotSummaryExpressionItem summaryItem, int startGroupIndex, int endGroupIndex) {
			summaryItem.HasBadRelations = true;
			for(int i = endGroupIndex; i >= startGroupIndex; i--) {
				if(!RequireCalcGroupRowSummary(GroupInfo[i])) continue;
				PivotSummaryValue summaryValue = summaryItem.CreateSummaryValue(Controller.ValueComparer);
				GroupInfo[i].SetSummaryValue(summaryItem, summaryValue);
				summaryValue.SetErrorValue();
			}
		}
		protected void CalcGroupSummary(PivotSummaryItem summaryItem, int startGroupIndex, int endGroupIndex) {
			bool needCalcCustomSummary = Controller.NeedCalcCustomSummary(summaryItem.ColumnInfo);
			for(int i = endGroupIndex; i >= startGroupIndex; i--) {
				CalcGroupRowSummary(GroupInfo[i], summaryItem, needCalcCustomSummary);
			}
			CalcRunningSummaries(summaryItem, startGroupIndex, endGroupIndex, needCalcCustomSummary);
		}
		PivotSummaryValue GetSummaryValue(GroupRowInfo groupRow, PivotSummaryItem summaryItem) {
			return groupRow != null ? groupRow.GetSummaryValue(summaryItem) as PivotSummaryValue : null;
		}
		protected virtual GroupRowInfo GetPrevColumnGroupRow(GroupRowInfo groupRow) {
			if(groupRow.Level < GetRowGroupCount()) return null;
			return GetPrevGroupRow(groupRow, GetRowGroupCount());
		}
		protected virtual int GetRowGroupCount() { return 0; }
		protected GroupRowInfo GetPrevGroupRow(GroupRowInfo groupRow) {
			return GetPrevGroupRow(groupRow, 0);
		}
		protected GroupRowInfo GetNextGroupRow(GroupRowInfo groupRow) {
			return GetNextGroupRow(groupRow, 0);
		}
		protected GroupRowInfo GetPrevGroupRow(GroupRowInfo groupRow, int groupRowCount) {
			for(int i = groupRow.Index - 1; i >= 0; i--) {
				if(GroupInfo[i].Level < groupRowCount) return null;
				if(GroupInfo[i].Level == groupRow.Level)
					return GroupInfo[i];
			}
			return null;
		}
		protected GroupRowInfo GetNextGroupRow(GroupRowInfo groupRow, int groupRowCount) {
			for(int i = groupRow.Index + 1; i < GroupInfo.Count; i++) {
				if(GroupInfo[i].Level < groupRowCount) return null;
				if(GroupInfo[i].Level == groupRow.Level)
					return GroupInfo[i];
			}
			return null;
		}
		protected void CalcGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, bool needCalcCustomSummary) {
			if(!RequireCalcGroupRowSummary(groupRow))
				return;
			needCalcCustomSummary &= !SortInfo[groupRow.Level].RunningSummary;
			PivotSummaryValue summaryValue = summaryItem.CreateSummaryValue(Controller.ValueComparer);
			groupRow.SetSummaryValue(summaryItem, summaryValue);
			switch(summaryItem.CalculationMode) {
				case SummaryItemCalculationMode.Expression: {
						CalcGroupRowSummaryExpression(groupRow, (PivotSummaryExpressionItem)summaryItem, summaryValue);
						break;
					}
				case SummaryItemCalculationMode.Traditional: {
						CalcGroupRowSummaryCore(groupRow, summaryItem, summaryValue, needCalcCustomSummary);
						break;
					}
				case SummaryItemCalculationMode.AggregateExpression: {
						Controller.CalcGroupRowSummaryAggregateCore(groupRow, VisibleListSourceRows, (PivotCustomAggregateSummaryItem)summaryItem, summaryValue);
						break;
					}
			}
		}
		void CalcGroupRowSummaryCore(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, bool needCalcCustomSummary) {
			if(GroupInfo.IsLastLevel(groupRow)) {
				if(SortInfo[groupRow.Level].RunningSummary)
					CalcLastLevelGroupRowSummaryIgnoringRunnings(groupRow, summaryItem, summaryValue);
				else
					CalcLastLevelGroupRowSummary(groupRow, summaryItem, summaryValue);
			} else
				CalcParentGroupRowSummary(groupRow, summaryItem, summaryValue);
			if(needCalcCustomSummary)
				CalcCustomSummary(groupRow, summaryItem);
		}
		protected void CalcCustomSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem) {
			Controller.CalcCustomSummary(CreateCustomSummaryInfo(summaryItem, groupRow));
		}
		int GetGroupLevel(PivotGroupRowInfo groupInfo) {
			return groupInfo != null ? groupInfo.Level : -1;
		}
		protected virtual PivotGroupRowInfo GetColumnGroup(GroupRowInfo groupInfo) {
			return null;
		}
		protected virtual PivotGroupRowInfo GetRowGroup(GroupRowInfo groupInfo) {
			return null;
		}
		bool GetIsLevelError(GroupRowInfo groupRow, PivotSummaryExpressionItem summaryItem) {
			return GetGroupLevel(GetColumnGroup(groupRow)) < summaryItem.MaxColumnGroupLevel ||
							GetGroupLevel(GetRowGroup(groupRow)) < summaryItem.MaxRowGroupLevel;
		}
		void CalcGroupRowSummaryExpression(GroupRowInfo groupRow, PivotSummaryExpressionItem summaryItem, PivotSummaryValue summaryValue) {
			if(summaryItem.HasBadRelations || !Controller.ExpEvaluators.Contains(summaryItem) || GetIsLevelError(groupRow, summaryItem)) {
				summaryValue.SetErrorValue();
				return;
			} else {
				try {
					Controller.CachedHelper.State = PivotGridDataControllerState.SummaryExpressionCalculating;
					ExpressionEvaluator ev = Controller.ExpEvaluators[summaryItem];
					ev.DataAccess = this;
					object res = ev.Evaluate(groupRow);
					if(res == PivotSummaryValue.ErrorValue)
						summaryValue.SetErrorValue();
					else {
						res = summaryItem.ConvertExpressionValue(res);
						summaryValue.AddValue(res);
					}
				} catch {
					summaryValue.SetErrorValue();
				} finally {
					Controller.CachedHelper.State = PivotGridDataControllerState.UndefState;
				}
			}
		}
		protected virtual bool RequireCalcGroupRowSummary(GroupRowInfo groupRow) { return true; }
		public PivotCustomSummaryInfo CreateCustomSummaryInfo(PivotSummaryItem summaryItem, GroupRowInfo groupRow) {
			PivotCustomSummaryInfo info = new PivotCustomSummaryInfo(this, VisibleListSourceRows, summaryItem, (PivotSummaryValue)groupRow.GetSummaryValue(summaryItem), groupRow);
			info.ColColumn = GetColColumnByGroupRow(groupRow);
			info.RowColumn = GetRowColumnByGroupRow(groupRow);
			return info;
		}
		public virtual PivotColumnInfo GetColColumnByGroupRow(GroupRowInfo groupRow) {
			return null;
		}
		public virtual PivotColumnInfo GetRowColumnByGroupRow(GroupRowInfo groupRow) {
			return null;
		}
		public object[] GetGroupRowValues(GroupRowInfo groupRow) {
			if(groupRow == null) return new object[0];
			object[] values = new object[groupRow.Level + 1];
			while(groupRow != null) {
				values[groupRow.Level] = GetValue(groupRow);
				groupRow = groupRow.ParentGroup;
			}
			return values;
		}
		protected void CalcLastLevelGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			Controller.CalcLastLevelGroupRowSummary(VisibleListSourceRows, summaryItem, summaryValue, groupRow.ChildControllerRow, groupRow.ChildControllerRowCount);
		}
		protected void CalcLastLevelGroupRowSummaryIgnoringRunnings(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			int startIndex = groupRow.ChildControllerRow;
			int count = groupRow.ChildControllerRowCount;
			GroupRowInfo prevGroup = FindPrevGroupRow(groupRow);
			if(prevGroup != null) {
				startIndex = prevGroup.ChildControllerRow + prevGroup.ChildControllerRowCount;
				count = count + groupRow.ChildControllerRow - startIndex;
			}
			Controller.CalcLastLevelGroupRowSummary(VisibleListSourceRows, summaryItem, summaryValue, startIndex, count);
		}
		public void CalcParentGroupRowSummary(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			if(GroupInfo.Count == 0) return;
			if(groupRow == null && SortInfo[0].RunningSummary)
				CalcTotalRunningSummary(summaryItem, summaryValue);
			else
				CalcParentGroupRowSummaryCore(groupRow, summaryItem, summaryValue);
		}
		void CalcParentGroupRowSummaryCore(GroupRowInfo groupRow, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			int startIndex = groupRow != null ? groupRow.Index + 1 : 0;
			int level = groupRow != null ? groupRow.Level + 1 : 0;
			for(int i = startIndex; i < GroupInfo.Count; i++) {
				GroupRowInfo childGroupRow = GroupInfo[i];
				if(childGroupRow.Level < level && groupRow != null) break;
				if(childGroupRow.Level == level) {
					summaryValue.AddValue((PivotSummaryValue)childGroupRow.GetSummaryValue(summaryItem));
				}
			}
		}
		protected void CalcRunningSummaries(PivotSummaryItem summaryItem, int startGroupIndex, int endGroupIndex, bool needCalcCustomSummary) {
			CalcRunningSummariesCore(summaryItem);
			CalcRunningCustomSummaries(summaryItem, startGroupIndex, endGroupIndex, needCalcCustomSummary);
		}
		protected void CalcRunningCustomSummaries(PivotSummaryItem summaryItem, int startGroupIndex, int endGroupIndex, bool needCalcCustomSummary) {
			for(int i = endGroupIndex; i >= startGroupIndex; i--) {
				GroupRowInfo groupRow = GroupInfo[i];
				if(!RequireCalcGroupRowSummary(groupRow)) return;
				if(needCalcCustomSummary && SortInfo[groupRow.Level].RunningSummary)
					CalcCustomSummary(groupRow, summaryItem);
			}
		}
		void CalcTotalRunningSummary(PivotSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			GroupRowInfo lastGroupRow = null;
			for(int i = GroupInfo.Count - 1; i >= 0; i--) {
				if(GroupInfo[i].Level == 0) {
					lastGroupRow = GroupInfo[i];
					break;
				}
			}
			if(lastGroupRow != null)
				summaryValue.AddValue((PivotSummaryValue)lastGroupRow.GetSummaryValue(summaryItem));
		}
		void CalcRunningSummariesCore(PivotSummaryItem summaryItem) {
			for(int i = 0; i < GroupInfo.Count; i++) {
				GroupRowInfo groupRow = GroupInfo[i];
				int level = groupRow.Level;
				if(!SortInfo[level].RunningSummary) continue;
				GroupRowInfo prevGroupRow = FindPrevGroupRow(groupRow);
				if(prevGroupRow == null) continue;
				if(groupRow.ParentGroup != prevGroupRow.ParentGroup && !SortInfo[level].CrossGroupRunningSummary) continue;
				PivotSummaryValue summaryValue = (PivotSummaryValue)groupRow.GetSummaryValue(summaryItem);
				summaryValue.AddValue((PivotSummaryValue)prevGroupRow.GetSummaryValue(summaryItem));
			}
		}
		GroupRowInfo FindPrevGroupRow(GroupRowInfo groupRow) {
			int startIndex = groupRow.Index - 1;
			for(int i = startIndex; i >= 0; i--) {
				if(GroupInfo[i].Level == groupRow.Level)
					return GroupInfo[i];
			}
			return null;
		}
		protected virtual void CalcSummary() {
			UpdateGroupSummaryCore();
		}
		ChildGroupsCountCache childGroupsCountCache;
		bool hasRunningSummaryColumn;
		public bool DoConditionalSortSummaryAndAddOthers(IComparer<GroupRowInfo>[] comparers, bool firstPass) {
			PivotColumnInfo[] summaryColumns = CreateSummaryColumns();
			hasRunningSummaryColumn = HasRunningSummaryColumn(summaryColumns);
			GroupRowInfoCollection groups = CreateGroupRowInfoCollection();
			groups.AutoExpandAllGroups = GroupInfo.AutoExpandAllGroups;
			bool useCountCache = !firstPass && childGroupsCountCache != null;
			bool refreshRequired = DoConditionalSortSummary(summaryColumns, groups, null, comparers, useCountCache, firstPass);
			groups.UpdateIndexes();
			this.childGroupsCountCache = firstPass && refreshRequired ? new ChildGroupsCountCache(summaryColumns.Length, groups) : null;
			GroupInfo = groups;
			BuildRowsIndex();
			BuildVisibleIndexes();
			UpdateVisibleListSourceRows(groups, useCountCache, refreshRequired);
			return refreshRequired;
		}
		void UpdateVisibleListSourceRows(GroupRowInfoCollection groups, bool useCountCache, bool refreshRequired) {
			if(refreshRequired && !useCountCache) {
				int[] newRows;
				int rowCount;
				GetNewVisibleRows(groups, out newRows, out rowCount);
				if(rowCount < groups.VisibleListSourceRows.VisibleRowCount) {
					groups.VisibleListSourceRows.Init(newRows, rowCount, groups.VisibleListSourceRows.AppliedFilterExpression, groups.VisibleListSourceRows.HasUserFilter);
				} else {
					throw new Exception("Unnecessary refresh");
				}
			}
		}
		bool HasRunningSummaryColumn(PivotColumnInfo[] summaryColumns) {
			foreach(PivotColumnInfo pivotColumnInfo in summaryColumns) {
				if(pivotColumnInfo.RunningSummary)
					return true;
			}
			return false;
		}
		bool HasTopValuesColumn(PivotColumnInfo[] summaryColumns) {
			foreach(PivotColumnInfo pivotColumnInfo in summaryColumns) {
				if(pivotColumnInfo.ShowTopRows != 0)
					return true;
			}
			return false;
		}
		protected bool DoConditionalSortSummary(PivotColumnInfo[] summaryColumns, GroupRowInfoCollection groups, GroupRowInfo parentGroup, IComparer<GroupRowInfo>[] comparers, bool useCountCache, bool firstPass) {
			if(parentGroup != null && GroupInfo.IsLastLevel(parentGroup)) return false;
			int level = parentGroup == null ? 0 : parentGroup.Level + 1;
			PivotColumnInfo pivotColumnInfo = summaryColumns[level];
			List<GroupRowInfo> list = new List<GroupRowInfo>();
			GroupInfo.GetChildrenGroups(parentGroup, list);
			if(!pivotColumnInfo.RunningSummary) {
				IComparer<GroupRowInfo> comparer = comparers != null ? comparers[level] : null;
				DoConditionSortListCore(parentGroup, pivotColumnInfo, list, comparer);
			}
			bool doRefresh = false;
			int groupedCount = GetGroupedCount(groups, parentGroup, useCountCache, level, pivotColumnInfo, list, firstPass);
			doRefresh = groupedCount < list.Count && !pivotColumnInfo.ShowOthersValue;
			for(int n = 0; n < groupedCount; n++) {
				GroupRowInfo row = (GroupRowInfo)list[n];
				groups.Add(row);
				doRefresh |= DoConditionalSortSummary(summaryColumns, groups, row, comparers, useCountCache, firstPass);
				GroupRowInfo lastChild = groups[groups.Count - 1];
				if(row != lastChild)
					row.ChildControllerRowCount = lastChild.ChildControllerRow + lastChild.ChildControllerRowCount - row.ChildControllerRow;
			}
			if(pivotColumnInfo.ShowOthersValue)
				doRefresh |= AddOthersGroup(summaryColumns, groups, list, groupedCount, comparers, useCountCache, firstPass);
			return doRefresh;
		}
		protected virtual void DoConditionSortListCore(GroupRowInfo parentGroup, PivotColumnInfo pivotColumnInfo, List<GroupRowInfo> list, IComparer<GroupRowInfo> comparer) {
			if(pivotColumnInfo.SortbyColumn != null && comparer != null) {
				list.Sort(comparer);
				if(!hasRunningSummaryColumn) {
					MoveVisualListBlocks(list, parentGroup);
					SetChildControllerRow(list, parentGroup);
				}
			}
		}
		protected virtual int GetGroupedCount(GroupRowInfoCollection groups, GroupRowInfo parentGroup, bool useCountCache, int level, PivotColumnInfo pivotColumnInfo, IList<GroupRowInfo> list, bool firstPass) {
			int topRowCount = pivotColumnInfo.GetTopRowsCount(list.Count),
				groupedCount = topRowCount > 0 && topRowCount < list.Count ? topRowCount : list.Count;
			if(useCountCache && !pivotColumnInfo.ShowOthersValue) {
				int groupedCountFromCache = GetGroupCountFromCache(groups, parentGroup);
				if(groupedCountFromCache < groupedCount)
					throw new Exception("Corrupted groupCount cache");
				groupedCount = Math.Min(list.Count, groupedCountFromCache);
			}
			if(!useCountCache && !firstPass && !pivotColumnInfo.ShowOthersValue && !pivotColumnInfo.ShowTopRowsAbsolute && topRowCount > 0)
				return list.Count;
			return groupedCount;
		}
		int GetGroupCountFromCache(GroupRowInfoCollection groups, GroupRowInfo parentGroup) {
			if(parentGroup == null)
				return childGroupsCountCache[-1, 0];
			int listIndex = groups.VisibleListSourceRows.GetListSourceRow(parentGroup.ChildControllerRow);
			return childGroupsCountCache[parentGroup.Level, listIndex];
		}
		void GetNewVisibleRows(GroupRowInfoCollection groups, out int[] newRows, out int rowCount) {
			newRows = new int[groups.VisibleListSourceRows.VisibleRowCount];
			rowCount = 0;
			int offset = 0;
			for(int i = 0; i < groups.Count; i++) {
				GroupRowInfo row = groups[i],
					nextRow = i != groups.Count - 1 ? groups[i + 1] : null;
				if(nextRow == null || nextRow.Level <= row.Level) {
					offset = row.ChildControllerRow - rowCount;
					int firstIndex = row.ChildControllerRow, lastIndex = row.ChildControllerRow + row.ChildControllerRowCount;
					for(int j = firstIndex; j < lastIndex; j++)
						newRows[j - offset] = VisibleListSourceRows.GetListSourceRow(j);
					rowCount += row.ChildControllerRowCount;
				}
				row.ChildControllerRow -= offset;
			}
		}
		protected virtual void OnGroupInfoRecreated() { }
		public void MoveVisualListBlocks(IList<GroupRowInfo> groupList, GroupRowInfo parentGroup) {
			int[] records = new int[parentGroup != null ? parentGroup.ChildControllerRowCount : VisibleListSourceRows.VisibleRowCount];
			int currentControllerRowPos = 0;
			for(int i = 0; i < groupList.Count; i++) {
				GroupRowInfo row = groupList[i];
				MoveVisualListForGroup(row, records, currentControllerRowPos);
				currentControllerRowPos += row.ChildControllerRowCount;
			}
			VisibleListSourceRows.SetRange(parentGroup != null ? parentGroup.ChildControllerRow : 0, records);
		}
		void MoveVisualListForGroup(GroupRowInfo groupRow, int[] records, int newPosition) {
			for(int i = 0; i < groupRow.ChildControllerRowCount; i++) {
				records[newPosition + i] = VisibleListSourceRows.GetListSourceRow(groupRow.ChildControllerRow + i);
			}
		}
		public void SetChildControllerRow(IList<GroupRowInfo> groupList, GroupRowInfo parentGroup) {
			int childControllerRow = parentGroup != null ? parentGroup.ChildControllerRow : 0;
			for(int i = 0; i < groupList.Count; i++) {
				GroupRowInfo row = groupList[i];
				row.ChildControllerRow = childControllerRow;
				childControllerRow += row.ChildControllerRowCount;
				if(!GroupInfo.IsLastLevel(row)) {
					List<GroupRowInfo> childList = new List<GroupRowInfo>();
					GroupInfo.GetChildrenGroups(row, childList);
					SetChildControllerRow(childList, row);
				}
			}
		}
		protected virtual bool AddOthersGroup(PivotColumnInfo[] summaryColumns, GroupRowInfoCollection groups, List<GroupRowInfo> groupRowList, int groupedCount, IComparer<GroupRowInfo>[] comparers, bool useCountCache, bool firstPass) {
			if(groupedCount >= groupRowList.Count) return false;
			GroupRowInfo groupRow = GetOthersGroupRowInfo(groupRowList, groupedCount);
			if(this.othersRows == null)
				this.othersRows = new Hashtable();
			this.othersRows.Add(groupRow, groupRow);
			OthersGroupChangeVisibleListSource(groupRow);
			OthersGroupChangeGroupInfo(groupRow, summaryColumns);
			groups.Add(groupRow);
			return DoConditionalSortSummary(summaryColumns, groups, groupRow, comparers, useCountCache, firstPass);
		}
		GroupRowInfo GetOthersGroupRowInfo(IList<GroupRowInfo> groupRowList, int groupedCount) {
			GroupRowInfo groupRow = groupRowList[groupedCount];
			int childRowCount = groupRow.ChildControllerRowCount;
			for(int i = groupedCount + 1; i < groupRowList.Count; i++) {
				GroupRowInfo testRow = groupRowList[i];
				childRowCount += testRow.ChildControllerRowCount;
				if(testRow.ChildControllerRow < groupRow.ChildControllerRow)
					groupRow = testRow;
			}
			groupRow.ChildControllerRowCount = childRowCount;
			return groupRow;
		}
		void OthersGroupChangeVisibleListSource(GroupRowInfo groupRow) {
			DataColumnSortInfoCollection sortInfo = new DataColumnSortInfoCollection(Controller);
			for(int i = groupRow.Level + 1; i < SortInfo.Count; i++)
				sortInfo.Add(SortInfo[i].ColumnInfo, SortInfo[i].SortOrder);
			if(sortInfo.Count > 0) {
				DoSortRowsCore(sortInfo, groupRow.ChildControllerRow, groupRow.ChildControllerRow + groupRow.ChildControllerRowCount - 1, false);
			}
		}
		void OthersGroupChangeGroupInfo(GroupRowInfo groupRow, PivotColumnInfo[] summaryColumns) {
			int insertedCount = 0;
			if(!GroupInfo.IsLastLevel(groupRow)) {
				int deletedIndex = groupRow.Index + 1;
				while(deletedIndex < GroupInfo.Count) {
					if(GroupInfo[deletedIndex].Level < groupRow.Level) break;
					GroupInfo.RemoveAt(deletedIndex);
				}
				int prevCount = GroupInfo.Count;
				DoGroupRowsCore(groupRow.ChildControllerRow, groupRow.ChildControllerRowCount, groupRow);
				insertedCount = GroupInfo.Count - prevCount;
				GroupInfo.MoveFromEndToMiddle(prevCount, insertedCount, deletedIndex);
			}
			CalcGroupSummaries(groupRow.Index, groupRow.Index + insertedCount);
		}
		public virtual void UpdateGroupSummary() {
			UpdateGroupSummaryCore();
		}
		protected internal void AddColumnsToSortInfo(PivotColumnInfoCollection columns) {
			for(int i = 0; i < columns.Count; i++) {
				DataColumnSortInfo sortInfo = SortInfo.Add(columns[i].ColumnInfo, columns[i].SortOrder);
				sortInfo.RunningSummary = columns[i].RunningSummary;
				sortInfo.CrossGroupRunningSummary = columns[i].CrossGroupRunningSummary;
			}
			SortInfo.GroupCount = SortInfo.Count;
		}
		protected bool IsReady { get { return Controller.IsReady; } }
		protected int ListSourceRowCount { get { return Controller.ListSourceRowCount; } }
		protected int VisibleListSourceRowCount { get { return VisibleListSourceRows.VisibleRowCount; } }
		public GroupRowInfo GetGroupRowByValues(object[] values) {
			if(values == null || values.Length < 1 || values.Length > GroupInfo.LevelCount) return null;
			return GetGroupRowByValues(0, rootIndex, values);
		}
		public GroupRowInfo GetGroupRowByValues(int level, Hashtable list, object[] values) {
			object value = values[level];
			object start = list[value == null ? NullValue : value];
			if(start == null)
				return null;
			if(level == values.Length - 1)
				return GroupInfo[(int)start];
			return GetGroupRowByValues(level + 1, rowsIndex[((int)start)], values);
		}
		object IEvaluatorDataAccess.GetValue(PD.PropertyDescriptor descriptor, object theObject) {
			GroupRowInfo rowInfo = (GroupRowInfo)theObject;
			SummaryPropertyDescriptor spd = descriptor as SummaryPropertyDescriptor;
			if(spd != null) {
				PivotSummaryItem summaryItem = spd.SummaryItem;
				object value = ((PivotSummaryValue)(rowInfo.GetSummaryValue(summaryItem))).GetValue(summaryItem.SummaryType);
				if(summaryItem.SummaryType == PivotSummaryType.Custom && (value is PivotGridCustomValues))
					((PivotGridCustomValues)value).TryGetValue(summaryItem.Name, out value);
				return value;
			} else {
				return GetValue(rowInfo, Controller.Columns[descriptor.Name]);
			}
		}
	}
}
