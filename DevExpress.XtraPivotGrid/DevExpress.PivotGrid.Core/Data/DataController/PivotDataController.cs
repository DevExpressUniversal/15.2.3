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
using System.Globalization;
using System.IO;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
#if SL
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Collections;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Data.PivotGrid {
	public interface IPivotClient {
		bool HasFilters(bool deferUpdates);
		bool HasGroupFilters(bool deferUpdates);
		bool HasSummaryFilters { get; }
		void SetFilteredValues(PivotFilteredValues[] values, bool deferUpdates);
		void SetGroupFilteredValues(List<PivotGroupFilteredValues> values, bool deferUpdates);
		void SetSummaryFilteredValues(PivotSummaryFilteredValues[] values);
		bool IsDesignMode { get; }
		bool IsUpdateLocked { get; }
		bool IsFilterAreaField(DataColumnInfo columnInfo);
		bool NeedCalcCustomSummary(DataColumnInfo columnInfo);
		void CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo);
		bool HasColumnVariation(DataColumnInfo columnInfo);
		bool HasRowVariation(DataColumnInfo columnInfo);
		bool HasCorrespondingField(DataColumnInfo columnInfo);
		void OnBeforePopulateColumns();
		void PopulateColumns();
		void UpdateLayout();
		void OnBeforeRefresh();
		string GetFieldCaption(DataColumnInfo columnInfo);
		CriteriaOperator PrefilterCriteria { get; }
	}
	public class PivotDataController : DataControllerBase, IEvaluatorDataAccess, ICalculationSource<GroupRowInfo, int>, ICalculationContext<GroupRowInfo, int> {
		const int SummaryIntervalCount = 50;
		PivotFilterAreaGroupHelperCache groupFilterHelper;
		ColumnPivotDataControllerArea columnArea;
		RowPivotDataControllerArea rowArea;
		DataControllerTotalSummaries totalValues;
		PivotSummaryExpressionEvaluators expEvaluators;
		VisibleListSourceRowCollection visibleListSourceRows;
		IPivotClient pivotClient;
		PivotSummaryItemCollection summaries;
		PivotStorageObjectValueComparer storageObjectComparer;
		bool cacheData = true; 
		bool caseSensitive = true;
		bool autoExpandGroups = true;
		public const string DataStreamSign = "PGDHLPER";
		public const string StreamSign = "PIVOTDC";
		ICustomObjectConverter customObjectConverter;
		public ICustomObjectConverter CustomObjectConverter {
			get { return customObjectConverter; }
			set { customObjectConverter = value; }
		}
		public PivotDataController() {
			this.columnArea = new ColumnPivotDataControllerArea(this);
			this.rowArea = new RowPivotDataControllerArea(this);
			this.summaries = new PivotSummaryItemCollection(this, new CollectionChangeEventHandler(OnSummaryCollectionChanged));
			this.visibleListSourceRows = new VisibleListSourceRowCollection(this);
			this.groupFilterHelper = new PivotFilterAreaGroupHelperCache(this);
			this.totalValues = new DataControllerTotalSummaries(this);
			this.expEvaluators = new PivotSummaryExpressionEvaluators();
			this.pivotClient = null;
			this.Columns = new PivotDataColumnInfoCollection();
			this.storageObjectComparer = new PivotStorageObjectValueComparer(this.CaseSensitive);
		}
		public override void Dispose() {
			PivotClient = null;
			base.Dispose();
		}
		public PivotDataControllerArea ColumnArea { get { return columnArea; } }
		public RowPivotDataControllerArea RowArea { get { return rowArea; } }
		public new IList ListSource {
			get { return base.ListSource; }
			set {
				BeginUpdate();
				try {
					SetListSource(value);
				} finally {
					EndUpdate();
				}
			}
		}
		public IPivotClient PivotClient { get { return pivotClient; } set { pivotClient = value; } }
		public PivotFilterAreaGroupHelperCache GroupFilterHelper { get { return groupFilterHelper; } }
		public PivotSummaryItemCollection Summaries { get { return summaries; } }
		public PivotSummaryExpressionEvaluators ExpEvaluators { get { return expEvaluators; } }
		public PivotDataColumnInfoCollection ExpColumns { get { return (PivotDataColumnInfoCollection)Columns; } }
		public bool CaseSensitive {
			get { return caseSensitive; }
			set {
				if(caseSensitive == value)
					return;
				caseSensitive = value;
				OnListSourceChanged();
			}
		}
		public bool AutoExpandGroups {
			get { return autoExpandGroups; }
			set { SetAutoExpandGroups(value, true); }
		}
		public void SetAutoExpandGroups(bool value, bool update) {
			if(value != autoExpandGroups) {
				autoExpandGroups = value;
				RowArea.GroupInfo.AutoExpandAllGroups = value;
				ColumnArea.GroupInfo.AutoExpandAllGroups = value;
				if(update)
					OnListSourceChanged();
			}
		}
		public bool SupportsUnboundColumns { get { return true; } }
		public bool IsDesignMode { get { return PivotClient != null ? PivotClient.IsDesignMode : false; } }
		public override bool IsUpdateLocked { get { return (PivotClient != null && PivotClient.IsUpdateLocked) || base.IsUpdateLocked; } }
		#region DataController protected methods
		public int GetListSourceRowByControllerRow(VisibleListSourceRowCollection visibleListRowCollection, int controllerRow) {
			return GetListSourceFromVisibleListSourceRowCollection(visibleListRowCollection, controllerRow);
		}
		public void SetVisibleListSourceCollection(VisibleListSourceRowCollection visibleListSourceRowCollection, int[] list, int count) {
			SetVisibleListSourceCollectionCore(visibleListSourceRowCollection, list, count);
		}
		public object GetRowValueFromHelper(GroupRowInfoCollection groupInfo, int childControllerRow, int columnIndex) {
			int listSource = GetListSourceRowIndex(groupInfo, childControllerRow);
			return Helper.GetRowValue(listSource, columnIndex);
		}
		public bool IsObjectEqualsFromHelper(GroupRowInfoCollection groupInfo, int childControllerRow, int columnIndex, object valueToCompare) {
			return ValueComparer.ObjectEquals(GetRowValueFromHelper(groupInfo, childControllerRow, columnIndex), valueToCompare);
		}
		public void CreateColumnStorages(DataColumnSortInfoCollection SortInfo, VisibleListSourceRowCollection VisibleListSourceRows) {
			SortInfo.CreateColumnStorages(VisibleListSourceRows, Helper);
		}
		public void ResetSortInfoCollection(DataColumnSortInfoCollection sortInfo) {
			ResetSortInfoCollectionCore(sortInfo);
		}
		public void VisibleListSourceCollectionQuickSort(PivotVisibleListSourceRowCollection visibleListSourceRowCollection,
						DataColumnSortInfoCollection sortInfo, int left, int right, bool useStorage) {
			if(CachedHelper != null)
				CachedHelper.EnsureStorageIsCreated(sortInfo);
			VisibleListSourceCollectionQuickSortCore(visibleListSourceRowCollection, sortInfo, left, right, useStorage);
		}
		public new void VisualClientUpdateLayout() {
			base.VisualClientUpdateLayout();
		}
		public new void DoGroupColumn(DataColumnSortInfoCollection sortInfo, GroupRowInfoCollection groupInfo, int controllerRow, int rowCount, GroupRowInfo parentGroup) {
			base.DoGroupColumn(sortInfo, groupInfo, controllerRow, rowCount, parentGroup);
		}
		#endregion
		public PivotGridDataHelper CachedHelper { get { return base.Helper as PivotGridDataHelper; } }
		public bool CacheData {
			get { return cacheData; }
			set {
				if(cacheData == value)
					return;
				cacheData = value;
				OnListSourceChanged();
			}
		}
		protected BaseDataControllerHelper BaseCreateHelper() {
			if(ListSource == null)
				return new PivotBaseDataControllerHelper(this);
#if !SL && !DXPORTABLE
			if(ListSource is System.Data.DataView)
				return new PivotDataViewDataControllerHelper(this);
#endif
#if DXWhidbey
			System.Windows.Forms.BindingSource bs = ListSource as System.Windows.Forms.BindingSource;
			try {
				if(bs != null && bs.SyncRoot is System.Data.DataView)
					return new PivotBindingSourceDataControllerHelper(this);
			} catch { }
#endif
			return new PivotListDataControllerHelper(this);
		}
		protected override BaseDataControllerHelper CreateHelper() {
			if(CacheData) {
				return new PivotGridDataHelper(BaseCreateHelper(), this);
			}
			return BaseCreateHelper();
		}
		protected override ValueComparer CreateValueComparer() {
			return new PivotValueComparer(this);
		}
		public new PivotValueComparerBase ValueComparer {
			get { return base.ValueComparer as PivotValueComparerBase; }
		}
		public PivotStorageObjectValueComparer StorageObjectComparer {
			get { return storageObjectComparer; }
		}
		public void UpdateStorage() {
			if(CachedHelper != null)
				CachedHelper.UpdateStorage();
		}
		public PivotDataControllerArea GetPivotArea(bool isColumn) {
			return isColumn ? ColumnArea : RowArea;
		}
		public PivotColumnInfo AddColumnToArea(bool isColumn, DataColumnInfo columnInfo, PivotSummaryType summaryType,
				ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, object sortByColumnField,
				PivotSummaryType sortbySummaryType, List<PivotSortByCondition> sortbyConditions,
				int topValueCount, bool showTopRowsAbsolute, bool showOthersValue, bool runningSummary,
				bool crossGroupRunningSummary) {
			return GetPivotArea(isColumn).Columns.Add(columnInfo, summaryType, sortOrder, sortbyColumn, sortByColumnField,
				sortbySummaryType, sortbyConditions, topValueCount, showTopRowsAbsolute, showOthersValue,
				runningSummary, crossGroupRunningSummary);
		}
		public void AddSummary(PivotSummaryItem summaryItem) {
			Summaries.Add(summaryItem);
		}
		public void AddExpressionEvaluator(PivotSummaryExpressionItem summaryItem, PivotSummaryExpressionEvaluator expEvaluator) {
			if(!ExpEvaluators.Contains(summaryItem)) {
				ExpEvaluators.Add(summaryItem, expEvaluator);
			}
		}
		public bool HasExpressionDependings(PivotSummaryItem summaryItem) {
			foreach(PivotSummaryItem item in Summaries) {
				PivotSummaryExpressionItem expressionItem = item as PivotSummaryExpressionItem;
				if(expressionItem != null && expressionItem.SummaryRelations != null && expressionItem.SummaryRelations.Contains(summaryItem))
					return true;
			}
			return false;
		}
		public void SafeRemoveSummaryItem(PivotGridFieldBase field) {
			int dataIndex = -1;
			for(int i = 0; i < Summaries.Count; i++)
				if(Summaries[i].Field == field) {
					dataIndex = i;
					break;
				}
			if(dataIndex < 0)
				return;
			PivotSummaryItem item = Summaries[dataIndex];
			if(HasExpressionDependings(item)) {
				Summaries.BeginUpdate();
				Summaries.RemoveAt(dataIndex);
				Summaries.Add(item);
				Summaries.EndUpdate();
			} else {
				Summaries.RemoveAt(dataIndex);
			}
		}
		public void MoveSummaryItem(PivotGridFieldBase field, int oldIndex, int newIndex) {
			int fieldIndex = -1;
			for(int i = 0; i < Summaries.Count; i++)
				if(Summaries[i].Field == field) {
					fieldIndex = i;
					break;
				}
			if(fieldIndex < 0 || fieldIndex == newIndex)
				return;
			oldIndex = fieldIndex;
			List<PivotSummaryItem> items = new List<PivotSummaryItem>();
			for(int i = 0; i < summaries.Count; i++)
				items.Add(Summaries[i]);
			PivotSummaryItem oldItem = Summaries[oldIndex];
			items.RemoveAt(oldIndex);
			items.Insert(newIndex, oldItem);
			Summaries.ClearAndAddRange(items.ToArray());
		}
		public bool HasCorrespondingField(DataColumnInfo column) {
			if(PivotClient == null)
				return true;
			return PivotClient.HasCorrespondingField(column);
		}
		public bool IsFilterAreaField(DataColumnInfo column) {
			if(PivotClient == null)
				return false;
			return PivotClient.IsFilterAreaField(column);
		}
		public void ChangeFieldExpanded(bool isColumn, int areaIndex, bool expanded) {
			GetPivotArea(isColumn).ChangeColumnExpanded(areaIndex, expanded);
		}
		public void ChangeFieldExpanded(bool isColumn, int areaIndex, bool expanded, object value) {
			GetPivotArea(isColumn).ChangeColumnExpanded(areaIndex, expanded, value);
		}
		public void ChangeExpanded(bool isColumn, int visibleIndex, bool expanded, bool recursive) {
			GetPivotArea(isColumn).ChangeExpanded(visibleIndex, expanded, recursive);
		}
		public void ChangeExpanded(bool isColumn, bool expanded) {
			GetPivotArea(isColumn).ChangeAllExpanded(expanded);
		}
		public void ClearAreaColumnsAndSummaries() {
			ColumnArea.Columns.Clear();
			RowArea.Columns.Clear();
			Summaries.Clear();
			ExpEvaluators.Clear();
		}
		public bool IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return IsObjectCollapsed(isColumn, GetPivotArea(isColumn).GetGroupRowInfo(visibleIndex));
		}
		bool IsObjectCollapsed(bool isColumn, GroupRowInfo groupRow) {
			if(groupRow == null)
				return false;
			return (groupRow.Level < GetPivotArea(isColumn).Columns.Count - 1) && !groupRow.Expanded;
		}
		public object GetFieldValue(bool isColumn, int columnRowIndex, int areaIndex) {
			return GetPivotArea(isColumn).GetValue(columnRowIndex, areaIndex);
		}
		public int GetVisibleIndexByValues(bool isColumn, object[] values) {
			return GetPivotArea(isColumn).GetVisibleIndexByValues(values);
		}
		public bool GetIsOthersFieldValue(bool isColumn, int visibleIndex, int areaIndex) {
			return GetPivotArea(isColumn).GetIsOthersValue(visibleIndex, areaIndex);
		}
		public int GetObjectLevel(bool isColumn, int visibleIndex) {
			GroupRowInfo groupRow = GetPivotArea(isColumn).GetGroupRowInfo(visibleIndex);
			return groupRow != null ? groupRow.Level : -1;
		}
		public int GetLevelCount(bool isColumn) {
			return GetPivotArea(isColumn).Columns.Count;
		}
		public int GetCellCount(bool isColumn) {
			return GetPivotArea(isColumn).VisibleCount;
		}
		public bool GetIsEmptyGroupFilter(PivotGroupFilterValuesCollection values, PivotColumnInfo[] columns) {
			PivotFilterAreaGroupHelper helper = GroupFilterHelper.GetHelper(columns);
			return GetIsEmptyGroupFilterCore(helper, values);
		}
		bool GetIsEmptyGroupFilterCore(DataControllerGroupHelperBase area, PivotGroupFilterValuesCollection values) {
			if(values.Count == 0)
				return true;
			PivotGroupFilterValuesCollection levelValues = values;
			PivotGroupFilterValue filterValue = null;
			int previousLevel = 0;
			foreach(GroupRowInfo row in area.GroupInfo) {
				int level = row.Level;
				if(levelValues.IsEmpty && level > previousLevel)
					continue;
				if(levelValues.IsEmpty || level != previousLevel) {
					levelValues = filterValue.GetLevelValues(level - 1);
					if(levelValues.IsEmpty)
						continue;
				}
				filterValue = levelValues[area.GetValue(row)];
				if(filterValue == null)
					return false;
				previousLevel = level;
			}
			return true;
		}
		public bool? ContainsGroupFilterValue(PivotGroupFilterValuesCollection values, object[] parentValues,
				object value, PivotColumnInfo[] columns) {
			PivotFilterAreaGroupHelper helper = GroupFilterHelper.GetHelper(columns);
			return ContainsGroupFilterValueCore(helper, values, parentValues, value);
		}
		bool? ContainsGroupFilterValueCore(DataControllerGroupHelperBase area, PivotGroupFilterValuesCollection values,
				object[] parentValues, object value) {
			if(values.IsEmpty)
				return false;
			PivotGroupFilterValuesCollection levelValues = values;
			PivotGroupFilterValue filterValue = null;
			bool levelFound = parentValues == null, valueFound = false, isPassing = false;
			int length = levelFound ? 0 : parentValues.Length, level = 0, previousLevel = 0;
			foreach(GroupRowInfo row in area.GroupInfo) {
				int subLevel = row.Level;
				if(valueFound && subLevel < level)
					return true;
				if((levelFound && subLevel < level) || (!levelFound && level >= length))
					return false;
				if(isPassing && subLevel >= previousLevel)
					continue;
				if(!levelFound) {
					if(subLevel != level)
						continue;
					if(object.Equals(parentValues[level], area.GetValue(row))) {
						if(levelValues.IsEmpty)
							return true;
						filterValue = levelValues[parentValues[level]];
						if(filterValue == null)
							return false;
						levelValues = filterValue.GetLevelValues(subLevel);
						levelFound = level == length - 1;
						level++;
					}
				}
				if(levelFound && !valueFound && subLevel == level && object.Equals(value, area.GetValue(row))) {
					if(levelValues.IsEmpty)
						return true;
					filterValue = levelValues[value];
					if(filterValue == null)
						return false;
					valueFound = true;
					level++;
					continue;
				}
				if(valueFound) {
					if(subLevel != previousLevel)
						levelValues = filterValue.GetLevelValues(subLevel - 1);
					isPassing = levelValues.IsEmpty;
					if(!isPassing) {
						filterValue = levelValues[area.GetValue(row)];
						if(filterValue == null)
							return null;
					}
					previousLevel = subLevel;
				}
			}
			return valueFound;
		}
		public List<object> GetUniqueGroupValues(object[] parentValues, PivotColumnInfo[] columns) {
			PivotFilterAreaGroupHelper helper = GroupFilterHelper.GetHelper(columns);
			return GetUniqueGroupValuesCore(helper, parentValues);
		}
		List<object> GetUniqueGroupValuesCore(PivotFilterAreaGroupHelper helper, object[] parentValues) {
			List<object> result = new List<object>();
			bool isFound = parentValues == null;
			int level = isFound ? -1 : 0, length = isFound ? 0 : parentValues.Length;
			foreach(GroupRowInfo row in helper.GroupInfo) {
				if((isFound && row.Level <= level) || level >= length)
					break;
				if(!isFound) {
					if(row.Level != level)
						continue;
					if(object.Equals(parentValues[level], helper.GetValue(row))) {
						isFound = level == length - 1;
						if(!isFound)
							level++;
					}
				} else if(row.Level == level + 1)
					result.Add(helper.GetValue(row));
			}
			return result;
		}
		public object[] GetUniqueFieldValues(int columnIndex, bool showBlanks) {
			return GetUniqueFieldFilteredValues(columnIndex, true, showBlanks);
		}
		public object[] GetVisibleFieldValues(int columnIndex, bool showBlanks) {
			return GetUniqueFieldFilteredValues(columnIndex, false, showBlanks);
		}
		object[] GetUniqueFieldFilteredValues(int columnIndex, bool includeFilteredOut, bool showBlanks) {
			return PivotFilterHelper.GetUniqueColumnValues(this, columnIndex, includeFilteredOut, showBlanks) ?? new object[0];
		}
		public object[] GetAvailableFieldValues(int columnIndex, bool deferUpdates) {
			return PivotFilterHelper.GetUniqueAvailableColumnValues(this, columnIndex, deferUpdates) ?? new object[0];
		}
		public object GetRowValue(int listSourceRow, string fieldName) {
			DataColumnInfo info = base.Helper.Columns[fieldName];
			if(info != null)
				return GetRowValue(listSourceRow, info);
			info = base.Helper.DetailColumns[fieldName];
			if(info != null)
				return base.Helper.GetRowValueDetail(listSourceRow, info);
			return null;
		}
		public object GetRowValue(int listSourceRow, DataColumnInfo column) {
			return column != null ? base.Helper.GetRowValue(listSourceRow, column.Index) : null;
		}
		public object GetRowValue(int listSourceRow, int columnIndex) {
			if(!IsColumnIndexValid(columnIndex))
				return null;
			return base.Helper.GetRowValue(listSourceRow, columnIndex);
		}
		public void SetRowValue(int listSourceRow, string fieldName, object value) {
			SetRowValue(listSourceRow, base.Helper.Columns[fieldName], value);
		}
		public void SetRowValue(int listSourceRow, DataColumnInfo column, object value) {
			if(column == null)
				return;
			SetRowValue(listSourceRow, column.Index, value);
		}
		public void SetRowValue(int listSourceRow, int columnIndex, object value) {
			if(!IsColumnIndexValid(columnIndex))
				return;
			base.Helper.SetRowValue(listSourceRow, columnIndex, value);
		}
		bool IsColumnIndexValid(int columnIndex) {
			return 0 <= columnIndex && columnIndex < base.Helper.Columns.Count;
		}
		public void EnsureStorageIsCreated(string fieldName) {
			DataColumnInfo columnInfo = base.Helper.Columns[fieldName];
			if(columnInfo != null)
				EnsureStorageIsCreated(columnInfo.Index);
		}
		public void EnsureStorageIsCreated(int columnIndex) {
			if(columnIndex >= 0 && columnIndex < Columns.Count)
				CachedHelper.EnsureStorageIsCreated(columnIndex);
		}
		public object GetNextOrPrevRowCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType, bool isNext) {
			rowVisibleIndex = RowArea.GetNextOrPrevVisibleIndex(rowVisibleIndex, isNext);
			if(rowVisibleIndex < 0)
				return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, summaryType);
		}
		public object GetNextOrPrevColumnCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType, bool isNext) {
			columnVisibleIndex = ColumnArea.GetNextOrPrevVisibleIndex(columnVisibleIndex, isNext);
			if(columnVisibleIndex < 0)
				return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, summaryType);
		}
		public object GetCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex, PivotSummaryType summaryType) {
			PivotSummaryValue summaryValue = GetCellSummaryValue(columnVisibleIndex, rowVisibleIndex, dataIndex);
			return summaryValue != null ? summaryValue.GetValue(summaryType) : null;
		}
		public object GetCellValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex) {
			if(Summaries.Count == 0)
				return null;
			return GetCellValue(columnVisibleIndex, rowVisibleIndex, dataIndex, Summaries[dataIndex].SummaryType);
		}
		public PivotSummaryValue GetCellSummaryValue(int columnVisibleIndex, int rowVisibleIndex, int dataIndex) {
			GroupRowInfo columnGroup = ColumnArea.GetGroupRowInfo(columnVisibleIndex),
				rowGroup = RowArea.GetGroupRowInfo(rowVisibleIndex);
			PivotSummaryValue summaryValue = GetCellSummaryValue(columnGroup, rowGroup, dataIndex);
			if(columnGroup == null || rowGroup == null)
				return summaryValue;
			if(summaryValue == null && columnGroup.Level < ColumnArea.SortInfo.Count && ColumnArea.SortInfo[columnGroup.Level].RunningSummary) {
				int absoluteIndex = ColumnArea.GetAbsoluteIndexByVisibleIndex(columnVisibleIndex) - 1;
				bool isCrossGroup = ColumnArea.SortInfo[columnGroup.Level].CrossGroupRunningSummary;
				GroupRowInfo parentGroup = absoluteIndex >= 0 ? ColumnArea.GroupInfo[absoluteIndex].ParentGroup : null;
				while(absoluteIndex >= 0) {
					GroupRowInfo groupInfo = ColumnArea.GroupInfo[absoluteIndex--];
					if(groupInfo.Level == columnGroup.Level && (isCrossGroup || groupInfo.ParentGroup == parentGroup)) {
						PivotSummaryValue prevValue = GetCellSummaryValue(groupInfo, rowGroup, dataIndex);
						if(prevValue != null)
							return prevValue;
					} else
						continue;
				}
				return null;
			}
			return summaryValue;
		}
		protected PivotSummaryValue GetCellSummaryValue(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, int dataIndex) {
			if(dataIndex < 0 || dataIndex >= Summaries.Count)
				return null;
			return GetCellSummaryValue(columnGroupRow, rowGroupRow, Summaries[dataIndex]);
		}
		PivotSummaryValue GetCellSummaryValue(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, PivotSummaryItem summaryItem) {
			if(summaryItem == null)
				return null;
			if(columnGroupRow == null && rowGroupRow == null) {
				PivotSummaryValue summaryValue = totalValues[summaryItem] as PivotSummaryValue;
				if(summaryValue == null)
					summaryValue = CalcTotalSummaryValue(summaryItem);
				return summaryValue;
			}
			VisibleListSourceRowCollection dummy;
			GroupRowInfo summaryRow = GetSummaryGroupRow(columnGroupRow, rowGroupRow, false, out dummy);
			return summaryRow != null ? summaryRow.GetSummaryValue(summaryItem) as PivotSummaryValue : null;
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(int columnVisibleIndex, int rowVisibleIndex) {
			return CreateDrillDownDataSource(columnVisibleIndex, rowVisibleIndex, PivotDrillDownDataSource.AllRows);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(int columnVisibleIndex, int rowVisibleIndex, int maxRowCount) {
			VisibleListSourceRowCollection cellVisibleListSourceRows = null;
			GroupRowInfo columnGroupRow = ColumnArea.GetGroupRowInfo(columnVisibleIndex);
			GroupRowInfo rowGroupRow = RowArea.GetGroupRowInfo(rowVisibleIndex);
			if(columnGroupRow == null && rowGroupRow == null)
				return CreateDrillDownDataSource(VisibleListSourceRows, null, maxRowCount);
			GroupRowInfo summaryRow = GetSummaryGroupRow(columnGroupRow, rowGroupRow, false, out cellVisibleListSourceRows);
			if(summaryRow == null)
				cellVisibleListSourceRows = null;
			return CreateDrillDownDataSource(cellVisibleListSourceRows, summaryRow, maxRowCount);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow) {
			return CreateDrillDownDataSourceCore(cellVisibleListSourceRows, groupRow, PivotDrillDownDataSource.AllRows);
		}
		public NativePivotDrillDownDataSource CreateDrillDownDataSource(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow, int maxRowCount) {
			return CreateDrillDownDataSourceCore(cellVisibleListSourceRows, groupRow, maxRowCount);
		}
		protected NativePivotDrillDownDataSource CreateDrillDownDataSourceCore(VisibleListSourceRowCollection cellVisibleListSourceRows, GroupRowInfo groupRow, int maxRowCount) {
			return new ClientPivotDrillDownDataSource(this, cellVisibleListSourceRows, groupRow, maxRowCount);
		}
		public int GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return GetPivotArea(isColumn).GetNextOrPrevVisibleIndex(visibleIndex, isNext);
		}
		public void ChangeFieldSortOrder(bool isColumn, int index) {
			GetPivotArea(isColumn).ChangeFieldSortOrder(index);
			GroupFilterHelper.Clear();  
		}
		public bool NeedCalcCustomSummary(DataColumnInfo columnInfo) {
			return PivotClient != null && PivotClient.NeedCalcCustomSummary(columnInfo);
		}
		public void CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo) {
			if(PivotClient != null) {
				PivotClient.CalcCustomSummary(customSummaryInfo);
			}
		}
		public bool HasColumnVariation(DataColumnInfo columnInfo) {
			return PivotClient != null ? PivotClient.HasColumnVariation(columnInfo) : false;
		}
		public bool HasRowVariation(DataColumnInfo columnInfo) {
			return PivotClient != null ? PivotClient.HasRowVariation(columnInfo) : false;
		}
		public bool SupportComparerCache(int column) {
			if(CachedHelper == null)
				return false;
			return CachedHelper.SupportComparerCache(column);
		}
		public bool HasComparerCache(int column) {
			if(CachedHelper == null)
				return false;
			return CachedHelper.HasComparerCache(column);
		}
		public void SetComparerCache(int column, int[] cache, bool isAscending) {
			if(CachedHelper == null)
				return;
			CachedHelper.SetComparerCache(column, cache, isAscending);
		}
		protected void ClearComparerCache(int column) {
			SetComparerCache(column, null, true);
		}
		void ClearComparerCache(PivotDataControllerArea area) {
			foreach(PivotColumnInfo column in area.Columns) {
				if(column.ShowTopRows <= 0)
					continue;
				ClearComparerCache(column.ColumnInfo.Index);
			}
		}
		public bool HasNullValues(int columnIndex) {
			EnsureStorageIsCreated(columnIndex);
			if(CachedHelper != null) {
				return CachedHelper.HasNullValue(columnIndex);
			}
			for(int i = 0; i < VisibleListSourceRows.VisibleRowCount; i++) {
				object value = Helper.GetRowValue(GetListSourceRowByControllerRow(VisibleListSourceRows, i), columnIndex);
				if(value == null || value == DBNull.Value)
					return true;
			}
			return false;
		}
		public void SaveDataToStream(Stream stream, bool compress) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(StreamSign);
			writer.Write(1);
			long startPosition = stream.Position;
			writer.Write(0L);
			writer.Write(0);
			int columnsCount = 0;
			for(int i = 0; i < Columns.Count; i++) {
				if(Columns[i].Unbound)
					continue;
				writer.Write(Columns[i].Name);
				writer.Write(Columns[i].Caption);
				writer.Write(Columns[i].Type.AssemblyQualifiedName);
				columnsCount++;
			}
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			writer.Write(columnsCount);
			stream.Position = endPosition;
#if !SL
			if(!CacheData)
				CacheData = true;
			CachedHelper.SaveToStream(stream, compress);
#endif
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			ColumnArea.SaveFieldsStateToStream(stream);
			RowArea.SaveFieldsStateToStream(stream);
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			ColumnArea.LoadFieldsStateFromStream(stream);
			RowArea.LoadFieldsStateFromStream(stream);
		}
		public void WebLoadCollapsedStateFromStream(Stream stream) {
			ColumnArea.WebLoadFieldsStateFromStream(stream);
			RowArea.WebLoadFieldsStateFromStream(stream);
		}
		public void WebSaveCollapsedStateToStream(Stream stream) {
			ColumnArea.WebSaveFieldsStateToStream(stream);
			RowArea.WebSaveFieldsStateToStream(stream);
		}
		public GroupRowInfo GetSummaryGroupRow(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, bool firstPass) {
			VisibleListSourceRowCollection dummy;
			return GetSummaryGroupRow(columnGroupRow, rowGroupRow, firstPass, out dummy);
		}
		GroupRowInfo GetSummaryGroupRow(GroupRowInfo columnGroupRow, GroupRowInfo rowGroupRow, bool firstPass,
				out VisibleListSourceRowCollection cellVisibleListSourceRows) {
			cellVisibleListSourceRows = null;
			if(rowGroupRow == null && columnGroupRow == null)
				return null;
			if(rowGroupRow == null) {
				cellVisibleListSourceRows = ColumnArea.VisibleListSourceRows;
				return columnGroupRow;
			}
			if(columnGroupRow == null) {
				cellVisibleListSourceRows = RowArea.VisibleListSourceRows;
				return rowGroupRow;
			}
			return RowArea.GetSummaryGroupRow(rowGroupRow, ColumnArea.GetGroupRowValues(columnGroupRow),
							firstPass, out cellVisibleListSourceRows);
		}
		public PivotSummaryValue CalcTotalSummaryValue(PivotSummaryItem summary) {
			PivotSummaryValue summaryValue = summary.CreateSummaryValue(ValueComparer);
			switch(summary.CalculationMode) {
				case SummaryItemCalculationMode.Traditional: {
						CalcTotalSummaryValueCore(summary, summaryValue);
						break;
					}
				case SummaryItemCalculationMode.Expression: {
						CalcTotalSummaryValueExpression((PivotSummaryExpressionItem)summary, summaryValue);
						break;
					}
				case SummaryItemCalculationMode.AggregateExpression: {
						CalcGroupRowSummaryAggregateCore(null, VisibleListSourceRows, (PivotCustomAggregateSummaryItem)summary, summaryValue);
						break;
					}
			}
			if(NeedCalcCustomSummary(summary.ColumnInfo))
				CalcCustomSummary(new PivotCustomSummaryInfo(null, VisibleListSourceRows, summary, summaryValue, null));
			totalValues[summary] = summaryValue;
			return summaryValue;
		}
		void CalcTotalSummaryValueCore(PivotSummaryItem summary, PivotSummaryValue summaryValue) {
			if(ColumnArea.GroupInfo.Count > 0)
				ColumnArea.CalcParentGroupRowSummary(null, summary, summaryValue);
			else {
				if(RowArea.GroupInfo.Count > 0) {
					RowArea.CalcParentGroupRowSummary(null, summary, summaryValue);
				} else {
					CalcLastLevelGroupRowSummary(VisibleListSourceRows, summary, summaryValue, 0, VisibleListSourceRows.VisibleRowCount);
				}
			}
		}
		void CalcTotalSummaryValueExpression(PivotSummaryExpressionItem summaryItem, PivotSummaryValue summaryValue) {
			if(summaryItem.HasBadRelations || !ExpEvaluators.Contains(summaryItem)) {
				summaryValue.SetErrorValue();
				return;
			} else {
				try {
					CachedHelper.State = PivotGridDataControllerState.SummaryExpressionCalculating;
					ExpressionEvaluator ev = ExpEvaluators[summaryItem];
					ev.DataAccess = totalValues;
					GroupRowInfo fakeRowInfo = new GroupRowInfo();
					object res = ev.Evaluate(fakeRowInfo);
					if(res == PivotSummaryValue.ErrorValue)
						summaryValue.SetErrorValue();
					else {
						res = summaryItem.ConvertExpressionValue(res);
						summaryValue.AddValue(res);
					}
				} catch {
					summaryValue.SetErrorValue();
				} finally {
					CachedHelper.State = PivotGridDataControllerState.UndefState;
				}
			}
		}
		public void CalcLastLevelGroupRowSummary(VisibleListSourceRowCollection visibleListRows, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, int startIndex, int count) {
			object value = null;
			decimal numericValue = 0;
			int colIndex = GetColumnIndex(summaryItem.ColumnInfo);
			for(int n = startIndex; n < startIndex + count; n++) {
				value = Helper.GetRowValue(GetListSourceRowByControllerRow(visibleListRows, n), colIndex);
				object convertedValue = summaryItem.ConvertValue(value, out numericValue);
				if(Object.Equals(convertedValue, PivotSummaryValue.ErrorValue))
					summaryValue.SetSummaryError();
				summaryValue.AddValue(value, numericValue);
			}
		}
		internal void CalcGroupRowSummaryAggregateCore(GroupRowInfo groupRow, VisibleListSourceRowCollection rows, PivotCustomAggregateSummaryItem summaryItem, PivotSummaryValue summaryValue) {
			int rowCount = groupRow != null ? groupRow.ChildControllerRowCount : rows != null ? rows.VisibleRowCount : 0;
			int startIndex = groupRow != null ? groupRow.ChildControllerRow : 0;
			summaryValue.AddValue(summaryItem.Calculate(GetListSourceRowsData(rows, summaryItem, summaryValue, startIndex, rowCount)));
		}
		IEnumerable<object> GetListSourceRowsData(VisibleListSourceRowCollection rows, PivotSummaryItem summaryItem, PivotSummaryValue summaryValue, int startIndex, int count) {
			int colIndex = GetColumnIndex(summaryItem.ColumnInfo);
			for(int n = startIndex; n < startIndex + count; n++)
				yield return Helper.GetRowValue(GetListSourceRowByControllerRow(rows, n), colIndex);
		}
		protected internal VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		protected override void EndUpdateCore(bool sortUpdate) {
			base.EndUpdateCore(sortUpdate);
			if(!IsUpdateLocked)
				DoRefresh(false);
		}
		protected override void Reset() {
			RowArea.Reset();
			ColumnArea.Reset();
			this.totalValues.Clear();
			this.ExpEvaluators.Clear();
			this.summaries.Clear();
		}
		protected bool IsRefreshInProgress { get { return refreshInProgress != 0; } }
		int refreshInProgress = 0;
		public override void PopulateColumns() {
			if(PivotClient != null)
				PivotClient.OnBeforePopulateColumns();
			base.PopulateColumns();
		}
		public void ClientPopulateColumns() {
			DoSaveGroupRowsState();
			if(CachedHelper != null) {
				CachedHelper.ClearStorage();
			}
			if(PivotClient != null)
				PivotClient.PopulateColumns();
			if(CachedHelper != null) {
				CachedHelper.RefreshData();
			}
		}
		protected virtual void DoRefreshAreas() {
			ColumnArea.DoRefresh();
			RowArea.DoRefresh();
		}
		bool isGroupRowsStateSaved = false;
		protected void DoSaveGroupRowsState() {
			if(!isGroupRowsStateSaved && ListSourceRowCount > 0) {
				isGroupRowsStateSaved = true;
				ColumnArea.SaveGroupRowsState();
				RowArea.SaveGroupRowsState();
			}
		}
		protected override void DoRefresh(bool useRowsKeeper) {
			if(IsUpdateLocked)
				return;
			this.refreshInProgress++;
			DoSaveGroupRowsState();
			try {
				this.totalValues.Clear();
				GroupFilterHelper.Clear();
				OnBeforeRefresh();
				DoFilterRows();
				DoRefreshAreas();
				DoCrossAreaRefresh();
				if(DoFilterBySummaries()) {
					DoRefreshAreas();
					DoCrossAreaRefresh();
				}
				ColumnArea.RestoreGroupRowsState();
				RowArea.RestoreGroupRowsState();
				if(this.ListSource != null) {
					ColumnArea.SaveGroupRowsColumns();
					RowArea.SaveGroupRowsColumns();
				}
			} finally {
				this.isGroupRowsStateSaved = false;
				this.refreshInProgress--;
				if(PivotClient != null)
					PivotClient.UpdateLayout();
			}
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			base.OnBindingListChanged(e);
			switch(e.ListChangedType) {
				case ListChangedType.Reset: {
						PopulateColumns();
						break;
					}
			}
			OnBindingListChangedCore(e);
		}
		protected virtual void OnBindingListChangedCore(ListChangedEventArgs e) {
			if(IsRefreshInProgress || CacheData)
				return;
			DoRefresh();
		}
		protected virtual void OnBeforeRefresh() {
			if(pivotClient != null)
				pivotClient.OnBeforeRefresh();
		}
		protected virtual void DoFilterRows() {
			if(PivotClient == null || (ListSourceRowCount == 0) ||
				(!PivotClient.HasFilters(false) && !PivotClient.HasGroupFilters(false) && FilterExpressionEvaluator == null)) {
					VisibleListSourceRows.ClearAndForceNonIdentity();
				return;
			}
			int filteredRowsCount;
			int[] filteredRows = GetFilteredRows(-1, out filteredRowsCount, false);
			SetVisibleListSourceCollection(VisibleListSourceRows, filteredRows, filteredRowsCount);
		}
		bool DoFilterBySummaries() {
			if(PivotClient == null || (ListSourceRowCount == 0) ||
				(VisibleListSourceRows.VisibleRowCount == 0) || !PivotClient.HasSummaryFilters)
				return false;
			int filteredRowsCount;
			int[] filteredRows = GetFilteredBySummaryRows(out filteredRowsCount);
			SetVisibleListSourceCollection(VisibleListSourceRows, filteredRows, filteredRowsCount);
			return true;
		}
		int[] GetFilteredBySummaryRows(out int filteredRowsCount) {
			const int BadIndex = -1;
			int[] filteredRows = new int[ListSourceRowCount];
			for(int i = 0; i < ListSourceRowCount; i++) {
				filteredRows[i] = BadIndex;
			}
			for(int i = 0; i < VisibleListSourceRows.VisibleRowCount; i++) {
				int index = VisibleListSourceRows.GetListSourceRow(i);
				filteredRows[index] = index;
			}
			PivotSummaryFilteredValues[] filteredValues = GetSummaryFilteredValues();
			CachedHelper.State = PivotGridDataControllerState.FilteringRows;
			try {
				PivotSummaryFilterManager.ForEachVisibleCrossGroupRow(RowArea, ColumnArea, delegate(GroupRowInfo rowGroup, GroupRowInfo columnGroup) {
					VisibleListSourceRowCollection visibleRows;
					GroupRowInfo summaryRow = GetSummaryGroupRow(columnGroup, rowGroup, false, out visibleRows);
					if(summaryRow == null || IsFitBySummary(rowGroup, columnGroup, filteredValues))
						return;
					int startIndex = summaryRow.ChildControllerRow;
					for(int i = startIndex; i < startIndex + summaryRow.ChildControllerRowCount; i++) {
						int visibleIndex = GetListSourceRowByControllerRow(visibleRows, i);
						filteredRows[visibleIndex] = BadIndex;
					}
				});
			} catch {
			} finally {
				CachedHelper.State = PivotGridDataControllerState.UndefState;
			}
			foreach(PivotSummaryItem summaryItem in Summaries) {
				if(summaryItem.Field != null && summaryItem.Field.CanFilterBySummary)
					summaryItem.IntervalsCache.Calculate(false, SummaryIntervalCount, ValueComparer);
			}
			int[] newFilteredRows = new int[ListSourceRowCount];
			filteredRowsCount = 0;
			for(int i = 0; i < filteredRows.Length; i++) {
				if(filteredRows[i] == BadIndex)
					continue;
				newFilteredRows[filteredRowsCount++] = filteredRows[i];
			}
			return newFilteredRows;
		}
		public PivotSummaryInterval GetSummaryInterval(int summaryIndex, bool visibleValuesOnly, bool customLevel,
				int rowLevel, int columnLevel) {
			if(summaryIndex < 0 || summaryIndex >= Summaries.Count)
				return PivotSummaryInterval.Empty;
			int row = !customLevel ? RowArea.GroupInfo.LevelCount - 1 : rowLevel,
				column = !customLevel ? ColumnArea.GroupInfo.LevelCount - 1 : columnLevel;
			PivotSummaryItem summaryItem = Summaries[summaryIndex];
			PivotSummaryInterval interval;
			if(summaryItem.IntervalsCache.TryGetValue(visibleValuesOnly, row, column, out interval))
				return interval;
			PivotSummaryFilterManager.ForEachVisibleCrossGroupRow(RowArea, ColumnArea, delegate(GroupRowInfo rowGroup, GroupRowInfo columnGroup) {
				GetCachedSummaryValue(summaryItem, rowGroup, columnGroup, visibleValuesOnly);
			});
			summaryItem.IntervalsCache.Calculate(visibleValuesOnly, SummaryIntervalCount, ValueComparer);
			if(!summaryItem.IntervalsCache.TryGetValue(visibleValuesOnly, row, column, out interval))
				return PivotSummaryInterval.Empty;
			return interval;
		}
		public int[] GetFilteredRows(int ignoreColumnIndex, out int filteredRowsCount, bool deferUpdates) {
			ExpressionEvaluator eval = FilterExpressionEvaluator;
			bool hasFilters = PivotClient.HasFilters(deferUpdates),
				hasGroupFilters = PivotClient.HasGroupFilters(deferUpdates),
				hasPrefilter = eval != null;
			int[] filteredRows = new int[ListSourceRowCount];
			filteredRowsCount = 0;
			PivotFilteredValues[] filteredValues = hasFilters ? GetFilteredValues(deferUpdates) : null;
			List<PivotGroupFilteredValues> groupFilteredValues = hasGroupFilters ? GetGroupFilteredValues(deferUpdates) : null;
			try {
				if(filteredValues != null)
					EnsureColumnStorage(filteredValues);
				CachedHelper.State = PivotGridDataControllerState.FilteringRows;
				for(int i = 0; i < ListSourceRowCount; i++) {
					if((hasFilters ? IsRowFit(i, filteredValues, ignoreColumnIndex) : true) &&
						(hasGroupFilters ? IsRowFit(i, groupFilteredValues) : true) &&
						(hasPrefilter ? eval.Fit(i) : true)) {
						filteredRows[filteredRowsCount++] = i;
					}
				}
			} catch {
			} finally {
				CachedHelper.State = PivotGridDataControllerState.UndefState;
			}
			return filteredRows;
		}
		void EnsureColumnStorage(object[] filteredValues) {
			for(int i = 0; i < Columns.Count; i++) {
				if(filteredValues[i] != null)
					EnsureStorageIsCreated(i);
			}
		}
		PivotFilteredValues[] GetFilteredValues(bool deferUpdates) {
			PivotFilteredValues[] filteredValues = new PivotFilteredValues[Columns.Count];
			for(int i = 0; i < filteredValues.Length; i++) {
				filteredValues[i] = null;
			}
			PivotClient.SetFilteredValues(filteredValues, deferUpdates);
			return filteredValues;
		}
		PivotSummaryFilteredValues[] GetSummaryFilteredValues() {
			PivotSummaryFilteredValues[] filteredValues = new PivotSummaryFilteredValues[Summaries.Count]; 
			for(int i = 0; i < filteredValues.Length; i++) {
				filteredValues[i] = null;
			}
			PivotClient.SetSummaryFilteredValues(filteredValues);
			return filteredValues;
		}
		List<PivotGroupFilteredValues> GetGroupFilteredValues(bool deferUpdates) {
			List<PivotGroupFilteredValues> res = new List<PivotGroupFilteredValues>();
			PivotClient.SetGroupFilteredValues(res, deferUpdates);
			return res;
		}
		protected virtual bool IsRowFit(int listSourceRow, PivotFilteredValues[] filteredValues) {
			return IsRowFit(listSourceRow, filteredValues, -1);
		}
		protected bool IsRowFit(int listSourceRow, PivotFilteredValues[] filteredValues, int ignoreColumnIndex) {
			for(int i = 0; i < Columns.Count; i++) {
				if(i == ignoreColumnIndex)
					continue;
				if(filteredValues[i] == null)
					continue;
				if(!filteredValues[i].IsValueFit(GetRowValueFromHelper(listSourceRow, i)))
					return false;
			}
			return true;
		}
		protected virtual bool IsRowFit(int listSourceRow, List<PivotGroupFilteredValues> filteredValues) {
			for(int i = 0; i < filteredValues.Count; i++) {
				if(!filteredValues[i].IsRowFit(GetRowValueFromHelper, listSourceRow))
					return false;
			}
			return true;
		}
		protected bool IsFitBySummary(GroupRowInfo rowGroup, GroupRowInfo columnGroup, PivotSummaryFilteredValues[] filteredValues) {
			int rowLevel = rowGroup != null ? rowGroup.Level : -1,
				columnLevel = columnGroup != null ? columnGroup.Level : -1;
			bool isLastLevel = (rowLevel == RowArea.GroupInfo.LevelCount - 1) && (columnLevel == ColumnArea.GroupInfo.LevelCount - 1);
			bool result = true;
			for(int i = 0; i < filteredValues.Length; i++) {
				PivotSummaryFilteredValues values = filteredValues[i];
				object value = GetCachedSummaryValue(Summaries[i], rowGroup, columnGroup, false);
				if(!result || values == null || (values.Mode == PivotSummaryFilterMode.LastLevel && !isLastLevel)
					|| (values.Mode == PivotSummaryFilterMode.SpecificLevel && (values.RowLevel != rowLevel || values.ColumnLevel != columnLevel)))
					continue;
				if(!values.IsValueFit(value, ValueComparer))
					result = false;
			}
			return result;
		}
		object GetCachedSummaryValue(PivotSummaryItem summaryItem, GroupRowInfo rowGroup, GroupRowInfo columnGroup, bool isVisibleSummary) {
			PivotSummaryValue summaryValue = GetCellSummaryValue(columnGroup, rowGroup, summaryItem);
			if(summaryValue == null)
				return null;
			object value = summaryValue.GetValue(summaryItem.SummaryType);
			PivotGridCustomValues customValuesDic = value as PivotGridCustomValues;
			if(customValuesDic != null)
				customValuesDic.TryGetValue(summaryItem.Name, out value);
			if(value == null || value is PivotErrorValue)
				return null;
			object[] rowBranch = GetLevelBranch(rowGroup, false),
			   columnBranch = GetLevelBranch(columnGroup, true);
			summaryItem.IntervalsCache.AddValue(isVisibleSummary, rowBranch, columnBranch, value);
			return value;
		}
		object[] GetLevelBranch(GroupRowInfo group, bool isColumn) {
			if(group == null)
				return null;
			PivotDataControllerArea area = GetPivotArea(isColumn);
			object[] branch = new object[group.Level + 1];
			for(int i = branch.Length - 1; i >= 0; i--) {
				if(group == null)
					break;
				branch[i] = area.GetValue(group);
				group = group.ParentGroup;
			}
			return branch;
		}
		protected object GetRowValueFromHelper(int listSourceRow, int columnIndex) {
			return Helper.GetRowValue(listSourceRow, columnIndex);
		}
		protected ExpressionEvaluator FilterExpressionEvaluator {
			get {
				ExpressionEvaluator filterExpressionEvaluator = null;
				if(PivotClient != null && !ReferenceEquals(PivotClient.PrefilterCriteria, null))
					filterExpressionEvaluator = CreateExpressionEvaluator(PivotClient.PrefilterCriteria);
				return filterExpressionEvaluator;
			}
		}
		ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator expression) {
			if(!IsReady)
				return null;
			if(ReferenceEquals(expression, null))
				return null;
			try {
				ExpressionEvaluator ev = new ExpressionEvaluator(GetFilterDescriptorCollection(), expression, CaseSensitive);
				ev.DataAccess = this;
				return ev;
			} catch {
				return null;
			}
		}
		public PropertyDescriptorCollection GetDescriptorCollection() {
			List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
			for(int n = 0; n < Summaries.Count; n++) {
				pds.Add(new SummaryPropertyDescriptor(Summaries[n].ColumnInfo.PropertyDescriptor, Summaries[n]));
			}
			for(int n = 0; n < Columns.Count; n++) {
				if(Summaries.Contains(Columns[n]))
					continue;
				pds.Add(Columns[n].PropertyDescriptor);
			}
			return new PropertyDescriptorCollection(pds.ToArray());
		}
		protected
#if DXCommon
		internal
#endif
 override PropertyDescriptorCollection GetFilterDescriptorCollection() {
			List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
			for(int n = 0; n < Columns.Count; n++) {
				pds.Add(Columns[n].PropertyDescriptor);
				PivotGridFieldBase field = Columns[n].Tag as PivotGridFieldBase;
				if(field != null && (field.UnboundType == UnboundColumnType.Bound || field.IsProcessOnSummaryLevel)) {
					if(!string.IsNullOrEmpty(field.UniqueName))
						pds.Add(new ExpNamePropertyDescriptor(Columns[n].PropertyDescriptor, field.UniqueName));
				}
			}
			foreach(KeyValuePair<string, DataColumnInfo> a in ((IEnumerable<KeyValuePair<string, DataColumnInfo>>)ExpColumns))
				if(pds.Find((f) => f.Name == a.Key) == null)
					pds.Add(new ExpNamePropertyDescriptor(a.Value.PropertyDescriptor, a.Key));
			return new PropertyDescriptorCollection(pds.ToArray());
		}
		protected virtual void DoCrossAreaRefresh() {
			ClearComparerCache(RowArea);
			ClearComparerCache(ColumnArea);
			bool rowDoRefresh = DoCrossAreaRefresh(RowArea, ColumnArea, true),
				columnDoRefresh = DoCrossAreaRefresh(ColumnArea, RowArea, true);
			RowArea.ClearControllerRowGroups();
			if(rowDoRefresh || columnDoRefresh) {
				int[] rows = null;
				if(rowDoRefresh && !columnDoRefresh)
					rows = RowArea.GroupInfo.VisibleListSourceRows.ToArray();
				if(!rowDoRefresh && columnDoRefresh)
					rows = ColumnArea.GroupInfo.VisibleListSourceRows.ToArray();
				if(rowDoRefresh && columnDoRefresh) {
					int[] rowAreaRows = RowArea.GroupInfo.VisibleListSourceRows.ToArray(),
						columnAreaRows = ColumnArea.GroupInfo.VisibleListSourceRows.ToArray();
					rows = IntersectArrays(rowAreaRows, columnAreaRows);
				}
				SetVisibleListSourceCollection(VisibleListSourceRows, rows, rows.Length);
				DoRefreshAreas();
				rowDoRefresh = DoCrossAreaRefresh(RowArea, ColumnArea, false);
				columnDoRefresh = DoCrossAreaRefresh(ColumnArea, RowArea, false);
				if(rowDoRefresh || columnDoRefresh)
					throw new Exception("Double entrance");
				RowArea.ClearControllerRowGroups();
			}
		}
		internal static int[] IntersectArrays(int[] array1, int[] array2) {
			Array.Sort<int>(array1);
			Array.Sort<int>(array2);
			List<int> res = new List<int>();
			int i1 = 0, i2 = 0;
			while(i1 < array1.Length && i2 < array2.Length) {
				int cmp = Comparer<int>.Default.Compare(array1[i1], array2[i2]);
				switch(cmp) {
					case 0:
						res.Add(array1[i1]);
						i1++;
						i2++;
						break;
					case 1:
						i2++;
						break;
					case -1:
						i1++;
						break;
				}
			}
			return res.ToArray();
		}
		protected bool DoCrossAreaRefresh(PivotDataControllerArea area, PivotDataControllerArea secondArea, bool firstPass) {
			return DoCrossAreaSort(area, secondArea, firstPass);
		}
		protected bool DoCrossAreaSort(PivotDataControllerArea area, PivotDataControllerArea secondArea, bool firstPass) {
			IComparer<GroupRowInfo>[] comparers = null;
			bool createOthers;
			comparers = GetComparers(area, secondArea, firstPass, out createOthers);
			if(comparers != null || createOthers) {
				return area.DoConditionalSortSummaryAndAddOthers(comparers, firstPass);
			}
			return false;
		}
		public IComparer<GroupRowInfo>[] GetComparers(PivotDataControllerArea area, PivotDataControllerArea secondArea, bool firstPass,
									out bool showToRows) {
			IComparer<GroupRowInfo>[] comparers = null;
			showToRows = false;
			for(int i = 0; i < area.Columns.Count; i++) {
				showToRows |= area.Columns[i].ShowTopRows > 0;
				IComparer<GroupRowInfo> comparer = GetSortComparer(area, secondArea, area.Columns[i], firstPass);
				if(comparer != null) {
					if(comparers == null)
						comparers = new IComparer<GroupRowInfo>[area.Columns.Count];
					comparers[i] = comparer;
				}
			}
			return comparers;
		}
		IComparer<GroupRowInfo> GetSortComparer(PivotDataControllerArea area, PivotDataControllerArea secondArea, PivotColumnInfo columnInfo,
							bool firstPass) {
			if(columnInfo.SortbyColumn == null)
				return null;
			IComparer<GroupRowInfo> comparer = null;
			bool hasNullValueCondition = false;
			if(columnInfo.ContainsSortSummaryConditions) {
				GroupRowInfo sortbyGroup = GetSortByGroup(secondArea, columnInfo.SortbyConditions, ref hasNullValueCondition);
				PivotSummaryItem summaryItem = RowArea.RowGroupsSummaries.TryGetValue(columnInfo.SortbyColumn);
				if(sortbyGroup != null && summaryItem != null) {
					comparer = new PivotConditionalGroupSummaryComparer(this, columnInfo.SortByColumnField as PivotGridFieldBase,
						sortbyGroup, columnInfo.SortOrder == ColumnSortOrder.Ascending, area == ColumnArea,
						summaryItem, columnInfo.SortbySummaryType, firstPass);
				}
			}
			if(comparer == null && columnInfo.ContainsSortSummary && (!columnInfo.ContainsSortSummaryConditions || hasNullValueCondition)) {
				comparer = new PivotGroupSummaryComparer(columnInfo.SortByColumnField as PivotGridFieldBase,
					this, area.Summaries.TryGetValue(columnInfo.SortbyColumn), columnInfo.SortOrder, columnInfo.SortbySummaryType);
			}
			return comparer;
		}
		GroupRowInfo GetSortByGroup(PivotDataControllerArea secondArea, List<PivotSortByCondition> sortedList, ref bool hasNullValueCondition) {
			int condIndex = 0,
				level = sortedList[condIndex].Level;
			object value = sortedList[condIndex].Value;
			for(int i = 0; i < secondArea.GroupInfo.Count; i++) {
				if(value == null)
					hasNullValueCondition = true;
				GroupRowInfo groupRow = secondArea.GroupInfo[i];
				if(groupRow.Level < level)
					break;
				if(groupRow.Level == level && ValueComparer.Compare(secondArea.GetValue(groupRow), value) == 0) {
					if(condIndex == sortedList.Count - 1)
						return groupRow;
					else {
						condIndex++;
						level = sortedList[condIndex].Level;
						value = sortedList[condIndex].Value;
					}
				}
			}
			return null;
		}
		protected void DoCrossAreaFiltering(PivotDataControllerArea area, PivotDataControllerArea secondArea) {
			bool visibleListSourceRowsChanged = false;
			for(int i = 0; i < area.Columns.Count; i++) {
				bool changed = DoFilterColumnTopRows(area, i);
				if(changed)
					area.DoRefresh();
				visibleListSourceRowsChanged |= changed;
			}
			if(visibleListSourceRowsChanged) {
				secondArea.DoRefresh();
			}
		}
		protected virtual bool DoFilterColumnTopRows(PivotDataControllerArea area, int columnIndex) {
			PivotColumnInfo columnInfo = area.Columns[columnIndex];
			if(columnInfo.ShowTopRows <= 0 || columnInfo.ShowOthersValue)
				return false;
			int listRowCount = 0;
			int[] listRows = new int[VisibleListSourceRows.VisibleRowCount];
			ArrayList topGroupRows = GetTopGroupRows(area.GroupInfo, columnIndex, columnInfo);
			for(int i = 0; i < topGroupRows.Count; i++) {
				GroupRowInfo groupRow = topGroupRows[i] as GroupRowInfo;
				for(int j = 0; j < groupRow.ChildControllerRowCount; j++)
					listRows[listRowCount++] = groupRow.ChildControllerRow + j;
			}
			if(listRowCount < VisibleListSourceRows.VisibleRowCount) {
				for(int i = 0; i < listRowCount; i++)
					listRows[i] = area.GetListSourceRowByControllerRow(listRows[i]);
				SetVisibleListSourceCollection(VisibleListSourceRows, listRows, listRowCount);
				return true;
			} else
				return false;
		}
		protected ArrayList GetTopGroupRows(GroupRowInfoCollection groupRows, int level, PivotColumnInfo columnInfo) {
			ArrayList list = new ArrayList();
			ArrayList levelList = new ArrayList();
			for(int i = 0; i < groupRows.Count; i++) {
				if(groupRows[i].Level == level) {
					levelList.Add(groupRows[i]);
				}
				if(groupRows[i].Level < level) {
					CopyTopGroupRows(levelList, list, columnInfo);
				}
			}
			CopyTopGroupRows(levelList, list, columnInfo);
			return list;
		}
		static void CopyTopGroupRows(ArrayList source, ArrayList destination, PivotColumnInfo columnInfo) {
			int count = columnInfo.GetTopRowsCount(source.Count);
			for(int i = 0; i < count; i++) {
				destination.Add(source[i]);
			}
			source.Clear();
		}
		protected override bool IsEqualNonNullValues(object val1, object val2) {
			string x = val1 as string,
				y = val2 as string;
			if (x != null && y != null)
#if !SL && !DXPORTABLE
				return String.Compare(x, y, !CaseSensitive, CultureInfo.CurrentCulture) == 0;
#else
				return CultureInfo.CurrentCulture.CompareInfo.Compare(x, y, CaseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase) == 0;
#endif
			return val1.Equals(val2);
		}
		public virtual void UpdateGroupSummary() {
			if(IsUpdateLocked)
				return;
			this.totalValues.Clear();
			ColumnArea.UpdateGroupSummary();
			RowArea.UpdateGroupSummary();
		}
		void OnSummaryCollectionChanged(object sender, CollectionChangeEventArgs e) {
			UpdateGroupSummary();
		}
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			SummaryPropertyDescriptor spd = descriptor as SummaryPropertyDescriptor;
			if(CachedHelper.State != PivotGridDataControllerState.FilteringRows) {
				PivotGridFieldBase field = Columns[descriptor.Name].Tag as PivotGridFieldBase;
				if(field != null && field.IsProcessOnSummaryLevel) {
					if(spd != null || field.Area == PivotArea.DataArea && field.ExpressionFieldName == descriptor.Name && field.Data.OptionsData.IsProcessExpressionOnSummaryLevel)
						throw new ExpressionException(ExpressionExceptionType.OrdinaryFieldAccessDataField);
					DataColumnInfo column = ExpColumns.GetColumnByExpName(field.ExpressionFieldName);
					if(column != null)
						return GetRowValue((int)theObject, column.Index);
					else
						throw new ExpressionException(ExpressionExceptionType.ColumnNotExists);
				}
			}
			return GetRowValue((int)theObject, Columns[descriptor.Name].Index);
		}
		#endregion
		IPivotDataControllerSort PivotSortClient { get { return (IPivotDataControllerSort)fSortClient; } }
		internal void FillRequireSortCell(DataColumnSortInfoCollection sortInfo) {
			int sortCount = sortInfo.Count;
			for(int n = 0; n < sortCount; n++) {
				DataColumnSortInfo info = sortInfo[n];
				sortInfo[n].RequireOnCellCompare = (this.fSortClient != null && this.PivotSortClient.RequireSortCell(info.ColumnInfo)) ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
			}
		}
		public bool ForceHeapSort { get; set; }
		protected void VisibleListSourceCollectionQuickSortCore(PivotVisibleListSourceRowCollection visibleListSourceRowCollection,
	DataColumnSortInfoCollection sortInfo, int left, int right, bool useStorage) {
			DataColumnSortInfoCollection sortInfoClone = sortInfo.Clone();
			FillRequireSortCell(sortInfoClone);
			visibleListSourceRowCollection.QuickSort(sortInfoClone.ToArray(), left, right, -1, useStorage);
		}
		internal int CompareRecords(DataColumnSortInfo[] sortInfo, int listSourceRow1, int listSourceRow2, bool useStorage) {
			int res = 0;
			if(listSourceRow1 == listSourceRow2)
				return 0;
			if(sortInfo.Length == 0)
				return listSourceRow1.CompareTo(listSourceRow2);
			int sortCount = sortInfo.Length;
			for(int n = 0; n < sortCount; n++) {
				DataColumnSortInfo info = sortInfo[n];
				ColumnSortOrder sortOrder = info.SortOrder;
				bool requireOnCellCompare = false;
				if(info.RequireOnCellCompare == DevExpress.Utils.DefaultBoolean.Default)
					requireOnCellCompare = (this.fSortClient != null && this.PivotSortClient.RequireSortCell(info.ColumnInfo));
				else
					requireOnCellCompare = info.RequireOnCellCompare == DevExpress.Utils.DefaultBoolean.True;
				if(useStorage) {
					var comparer = info.ColumnInfo.GetStorageComparer();
					int? clientSortCell = null;
					if(requireOnCellCompare) {
						clientSortCell = this.PivotSortClient.SortCell(listSourceRow1, listSourceRow2,
							comparer.GetNullableRecordValue(listSourceRow1), comparer.GetNullableRecordValue(listSourceRow2), info.ColumnInfo, sortOrder);
						if(clientSortCell.HasValue && clientSortCell.Value != 0)
							return clientSortCell.Value;
					}
					res = comparer.CompareRecords(listSourceRow1, listSourceRow2);
				} else {
					object v1 = Helper.GetRowValue(listSourceRow1, info.ColumnInfo.Index, null);
					object v2 = Helper.GetRowValue(listSourceRow2, info.ColumnInfo.Index, null);
					int? clientSortCell = null;
					if(requireOnCellCompare) {
						clientSortCell = this.PivotSortClient.SortCell(listSourceRow1, listSourceRow2,
							v1, v2, info.ColumnInfo, sortOrder);
						if(clientSortCell.HasValue && clientSortCell.Value != 0)
							return clientSortCell.Value;
					} 
					res = ValueComparer.Compare(v1, v2);
				}
				if(res == 0) {
					if(n == sortCount - 1)
						res = (listSourceRow1 == listSourceRow2 ? 0 : (listSourceRow1 < listSourceRow2 ? -1 : 1));
					if(res == 0)
						continue;
				}
				return sortOrder == ColumnSortOrder.Ascending ? res : -res;
			}
			return res;
		}
		object ICalculationSource<GroupRowInfo, int>.GetValue(GroupRowInfo column, GroupRowInfo row, int data) {
			PivotSummaryValue cell = GetCellSummaryValue(column, row, data);
			if(cell == null || Summaries.Count <= data)
				return null;			
			PivotSummaryItem sItem = Summaries[data];
			if(sItem.SummaryType != PivotSummaryType.Custom)
				return cell.GetValue(sItem.SummaryType);
			else {
				PivotGridCustomValues dic = cell.GetValue(sItem.SummaryType) as PivotGridCustomValues;
				object value;
				if(dic == null || !dic.TryGetValue(sItem.Name, out value))
					return null;
				else
					return value;
			}
		}
		ICalculationSource<GroupRowInfo, int> ICalculationContext<GroupRowInfo, int>.GetValueProvider() {
			return this;
		}
		int ICalculationContext<GroupRowInfo, int>.GetData(int index) {
			return index;
		}
		IEnumerable<GroupRowInfo> ICalculationContext<GroupRowInfo, int>.EnumerateFullLevel(bool isColumn, int level) {
			if(level == -1)
				yield return null;
			else {
				PivotDataControllerArea area = isColumn ? ColumnArea : RowArea;
				for(int j = 0; j < area.GroupInfo.Count; j++) {
					GroupRowInfo rowInfo = area.GroupInfo[j];
					if(rowInfo == null || rowInfo.Level != level)
						continue;
					yield return rowInfo;
				}
			}
		}
		object ICalculationContext<GroupRowInfo, int>.GetValue(GroupRowInfo groupInfo) {
			return groupInfo.GroupValue;
		}
		object ICalculationContext<GroupRowInfo, int>.GetDisplayValue(GroupRowInfo groupInfo) {
			return groupInfo.GroupValue;
		}
	}
	interface IPivotDataControllerSort : IDataControllerSort {
		bool RequireSortCell(DataColumnInfo column);
		int? SortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder);
		int? SortGroupCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn);
	}
	class PivotSummaryFilterManager {
		internal delegate void CrossGroupRowAction(GroupRowInfo rowGroup, GroupRowInfo columnGroup);
		internal static void ForEachVisibleCrossGroupRow(PivotDataControllerArea rowArea, PivotDataControllerArea columnArea,
				CrossGroupRowAction action) {
			int rowCount = rowArea.VisibleCount,
				columnCount = columnArea.VisibleCount;
			GroupRowInfo rowGroup = null;
			bool isRowsNonEmpty = (rowCount != 0);
			int row = 0;
			do {
				if(isRowsNonEmpty) {  
					rowGroup = rowArea.GetGroupRowInfo(row);
					action(rowGroup, null);
					ForEachCollapsedChild(rowArea, null, rowGroup, null, action);
				}
				for(int column = 0; column < columnCount; column++) {
					GroupRowInfo columnGroup = columnArea.GetGroupRowInfo(column);
					if(row == 0) {	
						action(null, columnGroup);
						ForEachCollapsedChild(columnArea, null, columnGroup, null, action);
					}
					if(isRowsNonEmpty) {
						action(rowGroup, columnGroup);
						ForEachCollapsedChild(rowArea, columnArea, rowGroup, columnGroup, action);
						ForEachCollapsedChild(columnArea, rowArea, columnGroup, rowGroup, action);
					}
				}
			} while(++row < rowCount);
		}
		static void ForEachCollapsedChild(PivotDataControllerArea area, PivotDataControllerArea otherArea,
				GroupRowInfo areaGroup, GroupRowInfo otherAreaGroup, CrossGroupRowAction action) {
			if(area == null || areaGroup == null || areaGroup.Expanded)
				return;
			for(int i = areaGroup.Index + 1; i < area.GroupInfo.Count; i++) {
				GroupRowInfo groupInfo = area.GroupInfo[i];
				if(groupInfo.Level <= areaGroup.Level)
					break;
				if(area.IsColumn) {
					action(otherAreaGroup, groupInfo);
				} else {
					action(groupInfo, otherAreaGroup);
				}
				ForEachCollapsedChild(otherArea, null, otherAreaGroup, groupInfo, action);
			}
		}
	}
}
