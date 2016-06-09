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
using System.IO;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Utils.DateHelpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Data.Utils;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridNativeDataSource : IPivotListDataSource, IDataControllerData2, IPivotDataControllerSort, IPivotClient, IUniqueFieldNameGenerator {
		public static object OthersValue { get { return DataControllerGroupHelperBase.OthersValue; } }
		static ColumnSortOrder GetColumnSortOrder(PivotGridFieldBase field) {
			return field.SortOrder == PivotSortOrder.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
		}
		IPivotGridDataSourceOwner owner;
		PivotDataController dataController;
		PivotGridData pivotGridData;
		UnboundColumnInfo nullableColumn;
		UnboundColumnInfoCollection unboundColumns;
		DateTime refreshDate;
		Dictionary<DataColumnInfo, PivotGridFieldBase> fieldsByColumns;
		Dictionary<DataColumnInfo, List<PivotGridFieldBase>> customSummaryColumnInfos;
		bool needPopulateColumns;
		Dictionary<PivotGridGroup, PivotColumnInfo[]> groupColumns;
		#region PivotGridData properties
		protected PivotGridData GridData { get { return pivotGridData; } }
		protected PivotGridFieldCollectionBase Fields { get { return GridData.Fields; } }
		protected PivotGridGroupCollection Groups { get { return GridData.Groups; } }
		protected PivotGridOptionsDataField OptionsDataField { get { return GridData.OptionsDataField; } }
		protected int DataFieldCount { get { return GridData.DataFieldCount; } }
		protected PivotGridFieldBase GetFieldByArea(PivotArea area, int index) { return GridData.GetFieldByArea(area, index); }
		protected bool IsDesignMode { get { return Owner != null ? Owner.IsDesignMode : false; } }
		#endregion
		public PivotGridNativeDataSource(PivotGridData pivotGridData) {
			this.pivotGridData = pivotGridData;
			Owner = pivotGridData;
			this.dataController = CreateDataController();
			DataController.DataClient = this;
			DataController.SortClient = this;
			DataController.PivotClient = this;
			DataController.ListSourceChanged += delegate(object sender, EventArgs e) {
				RaiseListSourceChanged();
			};
			this.nullableColumn = new UnboundColumnInfo("PivotGridNullableColumn", UnboundColumnType.Integer, true);
			this.unboundColumns = new UnboundColumnInfoCollection();
			this.refreshDate = DateTime.Today;
			this.fieldsByColumns = new Dictionary<DataColumnInfo, PivotGridFieldBase>();
			this.customSummaryColumnInfos = new Dictionary<DataColumnInfo, List<PivotGridFieldBase>>();
			this.needPopulateColumns = false;
			this.allowRunningSummary = new Dictionary<PivotArea, bool>();
		}
		public PivotDataController DataController { get { return dataController; } }
		public IPivotGridDataSourceOwner Owner { get { return owner; } set { owner = value; } }
		protected virtual PivotDataController CreateDataController() {
			return new PivotDataController();
		}
		protected DataColumnInfo GetColumnInfo(PivotGridFieldBase field) {
			if(field == null || field.ColumnHandle < 0)
				return null;
			return DataController.Columns[field.ColumnHandle];
		}
		PivotGridFieldBase GetFieldByPivotColumnInfo(DataColumnInfo columnInfo) {
			return columnInfo != null ? columnInfo.Tag as PivotGridFieldBase : null;
		}
		DateTime RefreshDate { get { return refreshDate; } set { refreshDate = value; } }
		void AddFieldIntoDataController(PivotGridFieldBase field, DataColumnInfo columnInfo) {
			if(!field.IsSummaryExpressionDataField)
				FieldsByColumns[columnInfo] = field;
			if(field.GroupIntervalColumnHandle > -1)
				FieldsByColumns[DataController.Columns[field.GroupIntervalColumnHandle]] = field;
			if(field.IsColumnOrRow) {
				DataColumnInfo sortedByColumn;
				PivotSummaryType sortBySummaryType;
				PivotGridFieldBase sfield;
				GetSortedByInfo(field.SortBySummaryInfo, out sortedByColumn, out sortBySummaryType, out sfield);
				if(sortedByColumn != null && sortBySummaryType == PivotSummaryType.Custom)
					AddCustomSummaryInfo(sfield, sortedByColumn);
				bool runningSummary = ShouldCalculateRunningSummary && field.RunningTotal;
				if(runningSummary && !allowRunningSummary[field.Area]) {
					GridData.OnInternalProblem();
					runningSummary = false;
				}
				PivotColumnInfo pivotColumnInfo = DataController.AddColumnToArea(
														field.IsColumn, columnInfo, field.SummaryType,
														GetColumnSortOrder(field), sortedByColumn, sfield,
														sortBySummaryType, GetSortbyConditions(field),
														field.TopValueCount, field.TopValueType == PivotTopValueType.Absolute,
														field.TopValueShowOthers, runningSummary,
														GridData.OptionsData.AllowCrossGroupVariation);
				if(pivotColumnInfo != null)
					pivotColumnInfo.Tag = field;
				if(field.GetTotalsVisibility() == PivotTotalsVisibility.CustomTotals && field.CustomTotals.Contains(PivotSummaryType.Custom)) {
					foreach(PivotGridFieldBase dataField in GridData.GetFieldsByArea(PivotArea.DataArea, false)) {
						DataColumnInfo dataColumn = DataController.Columns[GetFieldName(dataField)];
						if(dataColumn != null)
							AddCustomSummaryInfo(dataField, dataColumn);
					}
				}
			}
			if(field.Area == PivotArea.DataArea) {
				PivotSummaryItem summaryItem;
				if(field.IsSummaryExpressionDataField) {
					PivotSummaryExpressionItem summaryExpressionItem = PivotSummaryItem.CreateSummaryExpressionItem(columnInfo, field, GetMaxColumnGroupLevel(field), GetMaxRowGroupLevel(field));
					DataController.AddExpressionEvaluator(summaryExpressionItem,
														new PivotSummaryExpressionEvaluator(DataController, PatchExpressionNameToFieldName(false, field.ExpressionOperator)));
					summaryItem = summaryExpressionItem;
				} else {
					summaryItem = PivotSummaryItem.CreateSummaryItem(columnInfo, field);
					if(field.SummaryType == PivotSummaryType.Custom)
						AddCustomSummaryInfo(field, columnInfo);
				}
				DataController.AddSummary(summaryItem);
			}
		}
		int GetMaxColumnGroupLevel(PivotGridFieldBase dataField) {
			return GetMaxGroupLevel(true, dataField);
		}
		int GetMaxRowGroupLevel(PivotGridFieldBase dataField) {
			return GetMaxGroupLevel(false, dataField);
		}
		int GetMaxGroupLevel(bool isColumn, PivotGridFieldBase dataField) {
			int maxLevel = -2; 
			List<PivotGridFieldBase> relatedFields = GetRelatedFields(dataField);
			if(relatedFields == null || relatedFields.Count == 0)
				return maxLevel;
			foreach(PivotGridFieldBase relatedField in relatedFields) {
				if(relatedField.Area == (isColumn ? PivotArea.ColumnArea : PivotArea.RowArea))
					maxLevel = Math.Max(maxLevel, relatedField.AreaIndex);
			}
			return maxLevel;
		}
		void SetSummaryRelations() {
			List<PivotSummaryItem> mixedExprItems = new List<PivotSummaryItem>();
			for(int i = 0; i < DataController.Summaries.Count; i++) {
				PivotSummaryExpressionItem expSummary = DataController.Summaries[i] as PivotSummaryExpressionItem;
				if(expSummary == null)
					continue;
				if(expSummary.Field.MixedSummaryLevelCriteriaVisitor != null)
					foreach(var pair in expSummary.Field.MixedSummaryLevelCriteriaVisitor.SummaryLevel)
						foreach(var criteria in pair.Value.SummaryCriterias) {
							if(criteria.SummaryType == PivotSummaryType.Custom)
								mixedExprItems.Add(new PivotCustomAggregateSummaryItem(DataController.Columns[pair.Value.DataSourceLevelName], null, criteria.SummaryLevelName, criteria.SummaryType, criteria.CustomAggregate));
							else
								mixedExprItems.Add(new PivotSummaryItem(DataController.Columns[pair.Value.DataSourceLevelName], null, criteria.SummaryLevelName, criteria.SummaryType));
						}
				List<PivotGridFieldBase> relatedFields = GetRelatedFields(expSummary.Field);
				if(HasFilterAreaField(relatedFields)) {
					expSummary.HasBadRelations = true;
					continue;
				}
				relatedFields = GetDataFields(relatedFields);
				if(relatedFields != null) {
					foreach(PivotGridFieldBase dataField in relatedFields) {
						expSummary.AddRelatedSummary(GetSummaryByField(dataField));
					}
				}
			}
			foreach(PivotSummaryItem item in mixedExprItems) {
				DataController.AddSummary(item);
			}
		}
		PivotSummaryItem GetSummaryByField(PivotGridFieldBase field) {
			return GetSummaryByField(field, field.SummaryType);
		}
		PivotSummaryItem GetSummaryByField(PivotGridFieldBase field, PivotSummaryType summaryType) {
			for(int i = 0; i < DataController.Summaries.Count; i++) {
				PivotSummaryItem summaryItem = DataController.Summaries[i];
				if(summaryItem.Name == field.ExpressionFieldName && summaryItem.SummaryType == summaryType)
					return summaryItem;
			}
			return null;
		}
		int GetSummaryIndexByField(PivotGridFieldBase field) {
			PivotSummaryItem summaryItem = GetSummaryByField(field);
			return DataController.Summaries.IndexOf(summaryItem);
		}
		List<PivotGridFieldBase> GetRelatedFields(PivotGridFieldBase field) {
			if(field == null || !field.IsSummaryExpressionDataField || field.ExpressionColumnNames == null)
				return null;
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
			foreach(string name in field.ExpressionColumnNames) {
				PivotGridFieldBase theField = Fields[name];
				if(theField != null)
					fields.Add(theField);
			}
			if(fields.Count == 0)
				return null;
			return fields;
		}
		bool HasFilterAreaField(List<PivotGridFieldBase> fields) {
			if(fields == null)
				return false;
			foreach(PivotGridFieldBase field in fields) {
				if(field.Area == PivotArea.FilterArea)
					return true;
			}
			return false;
		}
		List<PivotGridFieldBase> GetDataFields(List<PivotGridFieldBase> fields) {
			if(fields == null)
				return null;
			List<PivotGridFieldBase> dataFileds = new List<PivotGridFieldBase>();
			foreach(PivotGridFieldBase field in fields) {
				if(field.Area == PivotArea.DataArea)
					dataFileds.Add(field);
			}
			if(dataFileds.Count == 0)
				return null;
			return dataFileds;
		}
		protected virtual bool ShouldCalculateRunningSummary {
			get { return (Capabilities & PivotDataSourceCaps.RunningTotals) == PivotDataSourceCaps.RunningTotals && shouldCalculateRunningSummaryInternal; }
		}
		List<PivotSortByCondition> GetSortbyConditions(PivotGridFieldBase field) {
			PivotGridFieldSortConditionCollection list = field.SortBySummaryInfo.Conditions;
			if(list == null || list.Count == 0)
				return null;
			List<PivotSortByCondition> res = new List<PivotSortByCondition>(list.Count);
			for(int i = 0; i < list.Count; i++) {
				PivotGridFieldBase conditionField = list[i].Field;
				if(conditionField == null || !conditionField.IsColumnOrRow || conditionField.Area == field.Area || conditionField.AreaIndex < 0)
					continue;
				DataColumnInfo columnInfo = DataController.Columns[GetFieldName(conditionField)];
				if(columnInfo == null)
					continue;
				res.Add(new PivotSortByCondition(columnInfo, list[i].Value, conditionField.AreaIndex));
			}
			res.Sort(GetSortbyConditionsComp);
			return res;
		}
		int GetSortbyConditionsComp(PivotSortByCondition x, PivotSortByCondition y) {
			return Comparer<int>.Default.Compare(x.Level, y.Level);
		}
		void GetSortedByInfo(PivotGridFieldSortBySummaryInfo sortBySummaryInfo, out DataColumnInfo sortedByColumn, out PivotSummaryType sortBySummaryType, out PivotGridFieldBase field) {
			sortedByColumn = null;
			sortBySummaryType = sortBySummaryInfo.SummaryType;
			field = sortBySummaryInfo.Field;
			if(field != null) {
				sortedByColumn = DataController.Columns[GetFieldName(sortBySummaryInfo.Field)];
				sortBySummaryType = sortBySummaryInfo.CustomTotalSummaryType.HasValue ? sortBySummaryInfo.CustomTotalSummaryType.Value
					: sortBySummaryInfo.Field.SummaryType;
			}
			if(sortedByColumn == null && !string.IsNullOrEmpty(sortBySummaryInfo.FieldName)) {
				sortedByColumn = DataController.Columns[sortBySummaryInfo.FieldName];
				if(sortBySummaryInfo.Owner != null && sortBySummaryInfo.Owner.Data != null) {
					if(sortedByColumn == null) {
						field = sortBySummaryInfo.Owner.Data.Fields.GetFieldByName(sortBySummaryInfo.FieldName);
						if(field != null && field.ColumnHandle > -1 && field.ColumnHandle < DataController.Columns.Count) {
							sortedByColumn = DataController.Columns[field.ColumnHandle];
							sortBySummaryType = field.SummaryType;
						}
					} else {
						if(field == null) {
							field = sortBySummaryInfo.Owner.Data.Fields.GetFieldByFieldName(sortBySummaryInfo.FieldName);
							if(field != null && sortBySummaryInfo.Owner.Data.Fields.GetSameFieldNameCount(field) != 1)
								field = null;
						}
					}
				}
			}
		}
		void AddCustomSummaryInfo(PivotGridFieldBase field, DataColumnInfo columnInfo) {
			if(!CustomSummaryColumnInfos.ContainsKey(columnInfo))
				CustomSummaryColumnInfos[columnInfo] = new List<PivotGridFieldBase>();
			if(!CustomSummaryColumnInfos[columnInfo].Contains(field))
				CustomSummaryColumnInfos[columnInfo].Add(field);
		}
		Dictionary<DataColumnInfo, PivotGridFieldBase> FieldsByColumns { get { return fieldsByColumns; } }
		Dictionary<DataColumnInfo, List<PivotGridFieldBase>> CustomSummaryColumnInfos { get { return customSummaryColumnInfos; } }
		void ClearFieldsAndSummaries() {
			FieldsByColumns.Clear();
			CustomSummaryColumnInfos.Clear();
			DataController.ClearAreaColumnsAndSummaries();
		}
		protected UnboundColumnInfo NullableColumn { get { return nullableColumn; } }
		protected UnboundColumnInfoCollection UnboundColumns {
			get {
				unboundColumns.Clear();
				unboundColumns.Add(NullableColumn);
				if(!DataController.SupportsUnboundColumns)
					return unboundColumns;
				foreach(PivotGridFieldBase field in Fields) {
					if(!field.IsUnbound)
						continue;
					string expression = field.UnboundExpression;
					if(field.UnboundExpressionMode == UnboundExpressionMode.UseAggregateFunctions) {
						MixedSummaryLevelCriteriaVisitor visitor = field.MixedSummaryLevelCriteriaVisitor;
						if(visitor != null) {
							expression = CriteriaOperator.ToString(field.ExpressionOperator);
							foreach(KeyValuePair<CriteriaOperator, MixedSummaryLevelCriteriaVisitor.DataSourceCriteria> pair in visitor.SummaryLevel) {
								UnboundColumnInfo unboundInfo2 = new UnboundColumnInfo(pair.Value.DataSourceLevelName, UnboundColumnType.Object, false, pair.Key.ToString());
								unboundInfo2.RequireValueConversion = false;
								unboundColumns.Add(unboundInfo2);
							}
						}
					}
					UnboundColumnInfo unboundInfo = new UnboundColumnInfo(GetFieldName(field), field.ActualUnboundType, false, expression);
					if(field.GroupInterval != PivotGroupInterval.Default)
						unboundInfo.RequireValueConversion = false;
					unboundColumns.Add(unboundInfo);
				}
				return unboundColumns;
			}
		}
		int unboundId = 0;
		public string GetFieldName(PivotGridFieldBase field) {
			return field.GetFieldName(ref unboundId);
		}
		protected internal object GetGroupIntervalValue(PivotGridFieldBase field, object value) {
			if(value == null)
				return value;
			if(field.GroupInterval == PivotGroupInterval.Custom)
				return GridData.GetCustomGroupInterval(field, value);
			if(value is DevExpress.Data.UnboundErrorObject)
				return PivotSummaryValue.ErrorValue;
			return GroupIntervalHelper.GetValue(field.GroupInterval, value, field.GroupIntervalNumericRange, RefreshDate);
		}
		protected CriteriaOperator PatchExpressionNameToFieldName(bool patchDataFields, CriteriaOperator expr) {
			return new OperatorNameToFieldNamePatcher(GridData.Fields, patchDataFields).Patch(expr);
		}
		#region IDataControllerSort implementation
		string[] IDataControllerSort.GetFindByPropertyNames() { return new string[0]; }
		bool IPivotDataControllerSort.RequireSortCell(DataColumnInfo column) {
			PivotGridFieldBase field = GetFieldByPivotColumnInfo(column);
			if(field == null)
				return false;
			return field.ActualSortMode == PivotSortMode.DisplayText || field.ActualSortMode == PivotSortMode.Custom ||
				((field.ActualSortMode == PivotSortMode.Value || field.ActualSortMode == PivotSortMode.Default)
					&& (field.GroupInterval == PivotGroupInterval.DateDayOfWeek));
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) { return false; }
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value, string columnName) { return string.Empty; }
		int? IPivotDataControllerSort.SortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			return DataControllerSortCell(listSourceRow1, listSourceRow2, value1, value2, sortColumn, sortOrder);
		}
		int? IPivotDataControllerSort.SortGroupCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) {
			return DataControllerSortCell(listSourceRow1, listSourceRow2, value1, value2, sortColumn, ColumnSortOrder.Ascending);
		}
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) {
			if(!((IPivotDataControllerSort)this).RequireSortCell(sortColumn))
				return null;
			var cmp = ((IPivotDataControllerSort)this).SortGroupCell(listSourceRow1, listSourceRow2, value1, value2, sortColumn);
			if(cmp.HasValue)
				return cmp.Value == 0;
			return null;
		}
		void IDataControllerSort.BeforeSorting() { }
		void IDataControllerSort.AfterSorting() { }
		void IDataControllerSort.BeforeGrouping() { }
		void IDataControllerSort.AfterGrouping() { }
		DevExpress.Data.Helpers.ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() {
			throw new NotSupportedException();
		}
		DevExpress.Data.Helpers.ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			throw new NotSupportedException();
		}
		DevExpress.Data.Helpers.ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			throw new NotSupportedException();
		}
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) { }
		int? DataControllerSortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			PivotGridFieldBase field = GetFieldByPivotColumnInfo(sortColumn);
			if(field == null)
				return null;
			return DataControllerSortCell(listSourceRow1, listSourceRow2, value1, value2, sortOrder, field);
		}
		int? DataControllerSortCell(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			switch(field.ActualSortMode) {
				case PivotSortMode.DisplayText:
					return DataControllerSortCellByDisplayText(value1, value2, sortOrder, field);
				case PivotSortMode.Custom:
					return DataControllerSortCellCustom(listSourceRow1, listSourceRow2, value1, value2, sortOrder, field);
				case PivotSortMode.Default:
				case PivotSortMode.Value:
					if(field.GroupInterval == PivotGroupInterval.DateDayOfWeek)
						if(value1 != null && value2 != null)
							return DateHelper.CompareDayOfWeek((DayOfWeek)value1, (DayOfWeek)value2);
					return null;
				default:
					return null;
			}
		}
		int? DataControllerSortCellCustom(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			PivotSortOrder pivotSortOrder = sortOrder == ColumnSortOrder.Ascending ? PivotSortOrder.Ascending : PivotSortOrder.Descending;
			return GridData.GetCustomSortRowsAccess(listSourceRow1, listSourceRow2, value1, value2, field, pivotSortOrder);
		}
		int DataControllerSortCellByDisplayText(object value1, object value2, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			string st1 = GridData.GetCustomFieldValueText(field, value1);
			string st2 = GridData.GetCustomFieldValueText(field, value2);
			int result = DataController.ValueComparer.Compare(st1, st2);
			if(sortOrder != ColumnSortOrder.Ascending) {
				result *= -1;
			}
			return result;
		}
		#endregion
		#region IDataControllerData implementation
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object expValue) {
			if(column == null)
				return expValue;
			PivotGridFieldBase field = column.Tag as PivotGridFieldBase;
			if(field == null)
				return expValue;
			if(field.UnboundType != UnboundColumnType.Bound) {
				object value = GridData.GetUnboundValueAccess(field, listSourceRow1, expValue);
				if(field.GroupInterval != PivotGroupInterval.Default) {
					value = GetGroupIntervalValue(field, value);
				}
				return value;
			}
			if(field.GroupIntervalColumnHandle > -1) {
				return GetGroupIntervalValue(field, DataController.GetRowValue(listSourceRow1, field.GroupIntervalColumnHandle));
			}
			return expValue;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return UnboundColumns;
		}
		#endregion
		#region IPivotClient implementation
		bool IPivotClient.HasFilters(bool deferUpdates) {
			for(int i = 0; i < Fields.Count; i++) {
				if(HasFieldFilter(Fields[i], Fields[i].FilterValues.GetActual(deferUpdates)))
					return true;
			}
			return false;
		}
		bool IPivotClient.HasGroupFilters(bool deferUpdates) {
			foreach(PivotGridGroup group in Groups) {
				if(HasGroupFilter(group, deferUpdates)) 
					return true;
			}
			return false;
		}
		bool IPivotClient.HasSummaryFilters {
			get { return this.HasSummaryFilters; }
		}
		bool HasSummaryFilters {
			get {
				for(int i = 0; i < Fields.Count; i++) {
					if(HasSummaryFilter(Fields[i]))
						return true;
				}
				return false;
			}
		}
		void IPivotClient.SetFilteredValues(PivotFilteredValues[] filteredValues, bool deferUpdates) {
			for(int i = 0; i < Fields.Count; i++) {
				IFieldFilter filter = Fields[i].FilterValues.GetActual(deferUpdates);
				if(HasFieldFilter(Fields[i], filter)) {
					filteredValues[Fields[i].ColumnHandle] = new PivotFilteredValues(filter.HashTable,
						filter.FilterType, filter.ShowBlanks);
				}
			}
		}
		void IPivotClient.SetGroupFilteredValues(List<PivotGroupFilteredValues> values, bool deferUpdates) {
			foreach(PivotGridGroup group in Groups) {
				if(HasGroupFilter(group, deferUpdates)) {
					IGroupFilter filter = group.FilterValues.GetActual(deferUpdates);
					values.Add(new PivotGroupFilteredValues(filter.FilterType, filter.Values, group.GetColumnHandles()));
				}
			}
		}
		void IPivotClient.SetSummaryFilteredValues(PivotSummaryFilteredValues[] values) {
			if(values.Length == 0)
				return;
			foreach(PivotGridFieldBase field in Fields) {
				if(!HasSummaryFilter(field))
					continue;
				int index = GetSummaryIndexByField(field),
					rowLevel = GetFieldLevel(field.SummaryFilter.RowField),
					columnLevel = GetFieldLevel(field.SummaryFilter.ColumnField);
				values[index] = new PivotSummaryFilteredValues(field.SummaryFilter, rowLevel, columnLevel);
			}
		}
		int GetFieldLevel(PivotGridFieldBase field) {
			if(field == null || !Fields.Contains(field))
				return -1;
			return field.AreaIndex;
		}
		bool IPivotClient.IsDesignMode { get { return IsDesignMode; } }
		bool IPivotClient.IsUpdateLocked { get { return GridData.IsLockUpdate; } }
		bool IPivotClient.NeedCalcCustomSummary(DataColumnInfo columnInfo) {
			return CustomSummaryColumnInfos.ContainsKey(columnInfo);
		}
		void IPivotClient.CalcCustomSummary(PivotCustomSummaryInfo customSummaryInfo) {
			IEnumerable<PivotGridFieldBase> fieldList = CustomSummaryColumnInfos[customSummaryInfo.DataColumn].Where((field) => field == null || field.ExpressionFieldName == customSummaryInfo.SummaryItem.Name);
			PivotGridCustomValues customValues = new PivotGridCustomValues();
			foreach(PivotGridFieldBase field in fieldList) {
				customSummaryInfo.SummaryValue.CustomValue = null;
				GridData.OnCalcCustomSummaryAccess(field, customSummaryInfo);
				customValues[field] = customSummaryInfo.SummaryValue.CustomValue;
			}
			customSummaryInfo.SummaryValue.CustomValue = customValues;
		}
		bool IPivotClient.HasColumnVariation(DataColumnInfo columnInfo) {
			if(OptionsDataField.Area == PivotDataArea.RowArea)
				return false;
			return HasVariation(columnInfo);
		}
		bool IPivotClient.HasRowVariation(DataColumnInfo columnInfo) {
			if(OptionsDataField.Area != PivotDataArea.RowArea)
				return false;
			return HasVariation(columnInfo);
		}
		bool HasVariation(DataColumnInfo columnInfo) {
			for(int i = 0; i < DataFieldCount; i++) {
				PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
				if(field.ColumnHandle != columnInfo.Index)
					continue;
				if(field.SummaryDisplayType == PivotSummaryDisplayType.AbsoluteVariation ||
					field.SummaryDisplayType == PivotSummaryDisplayType.PercentVariation)
					return true;
			}
			return false;
		}
		bool IPivotClient.HasCorrespondingField(DataColumnInfo columnInfo) {
			return FieldsByColumns.ContainsKey(columnInfo);
		}
		bool IPivotClient.IsFilterAreaField(DataColumnInfo columnInfo) {
			PivotGridFieldBase field;
			if(!FieldsByColumns.TryGetValue(columnInfo, out field))
				return false;
			return field.Area == PivotArea.FilterArea;
		}
		void IPivotClient.OnBeforePopulateColumns() {
			MakeComplexColumnList();
		}
		void IPivotClient.PopulateColumns() {
			if(!GridData.CanDoRefresh) {
				this.needPopulateColumns = true;
			}
			PivotGridFieldReadOnlyCollection sortedFields = Owner.GetSortedFields();
			BindColumnsCore(sortedFields, GetUnboundExpressionDependingFields(sortedFields));
			RaiseDataChanged();
		}
		void IPivotClient.UpdateLayout() {
			RaiseLayoutChanged();
			if(DataController != null) {
				IList<AggregationLevel> calcs = Owner.GetAggregations();
				new AggregationLevelsCalculator<GroupRowInfo, int>(DataController).Calculate(calcs);
			}
		}
		void IPivotClient.OnBeforeRefresh() {
			foreach(PivotGridFieldBase field in Fields) {
				field.FilterValues.CalculateSavedIsEmpty(GetUniqueFieldValues);
			}
		}
		string IPivotClient.GetFieldCaption(DataColumnInfo columnInfo) {
			PivotGridFieldBase field = FieldsByColumns[columnInfo] as PivotGridFieldBase;
			return field != null ? field.Caption : string.Empty;
		}
		CriteriaOperator IPivotClient.PrefilterCriteria {
			get {
				CriteriaOperator res = GridData.Prefilter.Enabled ? GridData.Prefilter.Criteria : null;
				return PatchExpressionNameToFieldName(true, res);
			}
		}
		bool HasFieldFilter(PivotGridFieldBase field, IFieldFilter filter) {
			return field.CanApplyFilter && filter.HasFilter && !IsGroupFilterAllowed(field.Group) && field.ColumnHandle > -1;
		}
		bool HasGroupFilter(PivotGridGroup group, bool deferUpdates) {
			return IsGroupFilterAllowed(group) && group.FilterValues.GetActual(deferUpdates).HasFilter && group.Count > 0 && group[0].CanApplyFilter;
		}
		bool HasSummaryFilter(PivotGridFieldBase field) {
			return field.CanApplySummaryFilter && field.ColumnHandle > -1 && !field.SummaryFilter.IsEmpty;
		}
		bool IsGroupFilterAllowed(PivotGridGroup group) {
			return group != null && group.IsFilterAllowed;
		}
		#endregion
		#region IDisposable implementation
		public virtual void Dispose() {
			this.dataController.Dispose();
			this.dataController = null;
		}
		#endregion
		#region IPivotListDataSource implementation
		PivotListDataSourceEventHandler listSourceChanged;
		protected virtual void RaiseListSourceChanged() {
			if(listSourceChanged != null)
				listSourceChanged(this);
		}
		public virtual IList ListSource { get { return DataController.ListSource; } }
		public virtual void SetListSource(IList value) {
			if(ListSource == value)
				return;
			DataController.ListSource = value;
		}
		event PivotListDataSourceEventHandler IPivotListDataSource.ListSourceChanged {
			add { listSourceChanged += value; }
			remove { listSourceChanged -= value; }
		}
		object IPivotListDataSource.GetListSourceRowValue(int listSourceRow, string fieldName) {
			return DataController.GetRowValue(listSourceRow, fieldName);
		}
		PivotSummaryValue IPivotListDataSource.GetCellSummaryValue(int columnIndex, int rowIndex, int dataIndex) {
			return DataController.GetCellSummaryValue(columnIndex, rowIndex, dataIndex);
		}
		PivotDrillDownDataSource IPivotListDataSource.GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			return DataController.CreateDrillDownDataSource(visibleListSourceRows, groupRow);
		}
		int IPivotListDataSource.CompareValues(object val1, object val2) {
			return DataController.ValueComparer.Compare(val1, val2);
		}
		#endregion
		#region IPivotGridDataSource implementation
		EventHandler<PivotDataSourceEventArgs> dataChanged, layoutChanged;
		protected void RaiseDataChanged() {
			this.dataChanged.SafeRaise(this, new PivotDataSourceEventArgs(this));
		}
		protected void RaiseLayoutChanged() {
			this.layoutChanged.SafeRaise(this, new PivotDataSourceEventArgs(this));
		}
		event EventHandler<PivotDataSourceEventArgs> IPivotGridDataSource.DataChanged {
			add { dataChanged += value; }
			remove { dataChanged -= value; }
		}
		event EventHandler<PivotDataSourceEventArgs> IPivotGridDataSource.LayoutChanged {
			add { layoutChanged += value; }
			remove { layoutChanged -= value; }
		}
		bool IPivotListDataSource.CaseSensitive {
			get { return DataController.CaseSensitive; }
			set { DataController.CaseSensitive = value; }
		}
		bool IPivotGridDataSource.AutoExpandGroups {
			get { return DataController.AutoExpandGroups; }
			set { DataController.AutoExpandGroups = value; }
		}
		void IPivotGridDataSource.SetAutoExpandGroups(bool value, bool reloadData) {
			DataController.SetAutoExpandGroups(value, reloadData);
		}
		ICustomObjectConverter IPivotGridDataSource.CustomObjectConverter {
			get { return DataController.CustomObjectConverter; }
			set { DataController.CustomObjectConverter = value; }
		}
		public virtual PivotDataSourceCaps Capabilities {
			get {
				PivotDataSourceCaps res = PivotDataSourceCaps.Prefilter | PivotDataSourceCaps.RunningTotals;
				if(DataController.SupportsUnboundColumns)
					res |= PivotDataSourceCaps.UnboundColumns;
				return res;
			}
		}
		bool IPivotGridDataSource.ShouldCalculateRunningSummary {
			get { return ShouldCalculateRunningSummary; }
		}
		void IPivotGridDataSource.ReloadData() {
			this.needPopulateColumns = true;
			RaiseDataChanged();
		}
		void IPivotGridDataSource.RetrieveFields(PivotArea area, bool visible) {
			RetrieveFieldsCore(area, visible);
		}
		void IPivotGridDataSource.DoRefresh() {
			PivotGridFieldReadOnlyCollection sortedFields = Owner.GetSortedFields();
			DataController.BeginUpdate();
			ClearGroupColumns();
			RefreshDate = DateTime.Today;
			List<PivotGridFieldBase> forcedFields = GetUnboundExpressionDependingFields(sortedFields);
			this.needPopulateColumns = this.needPopulateColumns
				|| ShouldForcePopulateColumns(forcedFields);
			if(DataController.Columns.Count == 0 || this.needPopulateColumns) {
				this.needPopulateColumns = false;
				DataController.PopulateColumns();
			}
			try {
				BindColumnsCore(sortedFields, forcedFields);
			} finally {
				DataController.CancelUpdate();
			}
			if(!GridData.LockRefresh)
				DataController.DoRefresh();
		}
		Dictionary<string, PivotArea> prevDependingFieldsAreaByName;
		Dictionary<string, PivotArea> PrevDependingFieldsAreaByName {
			get { return prevDependingFieldsAreaByName; }
			set { prevDependingFieldsAreaByName = value; }
		}
		bool ShouldForcePopulateColumns(List<PivotGridFieldBase> forcedFields) {
			if(!forcedFields.Any((f) => f.IsProcessOnSummaryLevel) && !GridData.OptionsData.IsProcessExpressionOnSummaryLevel || PrevDependingFieldsAreaByName == null)
				return false;
			if(PrevDependingFieldsAreaByName.Count != forcedFields.Count)
				return true;
			foreach(PivotGridFieldBase field in forcedFields) {
				if(!PrevDependingFieldsAreaByName.ContainsKey(field.ExpressionFieldName))
					return true;
				PivotArea prevArea = PrevDependingFieldsAreaByName[field.ExpressionFieldName];
				if(field.Area == prevArea)
					continue;
				if(field.Area == PivotArea.DataArea || prevArea == PivotArea.DataArea)
					return true;
			}
			return false;
		}
		Dictionary<string, PivotArea> GetDependingFieldsAreaByName(List<PivotGridFieldBase> forcedFields) {
			Dictionary<string, PivotArea> result = new Dictionary<string, PivotArea>();
			foreach(PivotGridFieldBase field in forcedFields)
				result.Add(field.ExpressionFieldName, field.Area);
			return result;
		}
		protected virtual void BindColumnsCore(PivotGridFieldReadOnlyCollection sortedFields, List<PivotGridFieldBase> forcedFields) {
			if(DataController.Columns.Count == 0)
				return;
			DataController.BeginUpdate();
			if(DataController.Columns[NullableColumn.Name] != null) {
				DataController.Columns[NullableColumn.Name].Visible = false;
			}
			try {
				ClearFieldsAndSummaries();
				PrepareRunningSummarySupport(sortedFields);
				foreach(PivotGridFieldBase field in sortedFields) {
					SetColumnHandles(field);
					DataColumnInfo columnInfo = GetFieldColumnInfo(field);
					if(!field.Visible && !forcedFields.Contains(field)
						&& field.FilterValues.FilterType == PivotFilterType.Excluded
						&& field.FilterValues.Count == 0
						&& !(field.Data.Prefilter.Contains(field) && field.IsUnbound))
						continue;
					if(columnInfo == null)
						continue;
					AddFieldIntoDataController(field, columnInfo);
				}
				SetSummaryRelations();
			} finally {
				DataController.CancelUpdate();
				DataController.UpdateStorage();
				PrevDependingFieldsAreaByName = GetDependingFieldsAreaByName(forcedFields);
			}
		}
		Dictionary<PivotArea, bool> allowRunningSummary;
		bool shouldCalculateRunningSummaryInternal;
		void PrepareRunningSummarySupport(PivotGridFieldReadOnlyCollection sortedFields) {
			shouldCalculateRunningSummaryInternal = true;
			allowRunningSummary[PivotArea.ColumnArea] = true;
			allowRunningSummary[PivotArea.RowArea] = true;
			int runningTotalCalculationDataFieldCount = 0;
			foreach(PivotGridFieldBase field in sortedFields) {
				if(!field.Visible)
					continue;
				if(field.Area == PivotArea.DataArea && HasRunningTotalsCalculation(field.Calculations))
					runningTotalCalculationDataFieldCount++;
				if(!field.IsColumnOrRow)
					continue;
				if(field.TopValueCount != 0)
					allowRunningSummary[field.Area] = false;
			}
			if(runningTotalCalculationDataFieldCount > 0)
				shouldCalculateRunningSummaryInternal = false;
		}
		bool HasRunningTotalsCalculation(IList<PivotCalculationBase> calculations) {
			foreach(PivotCalculationBase calculation in calculations)
				if(calculations is PivotRunningTotalCalculationBase)
					return true;
			return false;
		}
		List<PivotGridFieldBase> GetUnboundExpressionDependingFields(PivotGridFieldReadOnlyCollection sortedFields) {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			bool proc = !GridData.OptionsData.IsProcessExpressionOnSummaryLevel;
			foreach(PivotGridFieldBase sortedField in sortedFields) {
				if(proc && !sortedField.IsProcessOnSummaryLevel)
					continue;
				if(!sortedField.IsUnboundExpression || sortedField.ExpressionColumnNames == null)
					continue;
				List<PivotGridFieldBase> dependingFields = sortedFields.FindAll(delegate(PivotGridFieldBase field) {
					return sortedField.ExpressionColumnNames.Contains(field.ExpressionFieldName);
				});
				if(dependingFields != null)
					res.AddRange(dependingFields);
			}
			return GetUnique(res);
		}
		List<PivotGridFieldBase> GetSortBySummaryDependingFields(PivotGridFieldReadOnlyCollection sortedFields) {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			foreach(PivotGridFieldBase sortedField in sortedFields) {
				List<PivotGridFieldBase> dependingFields = sortedFields.FindAll(delegate(PivotGridFieldBase field) {
					return sortedField.SortBySummaryInfo.Field == field;
				});
				if(dependingFields != null)
					res.AddRange(dependingFields);
			}
			return GetUnique(res);
		}
		List<PivotGridFieldBase> GetUnique(List<PivotGridFieldBase> list) {
			list.Sort(delegate(PivotGridFieldBase x, PivotGridFieldBase y) {
				return Comparer<int>.Default.Compare(x.GetHashCode(), y.GetHashCode());
			});
			for(int i = list.Count - 1; i > 0; i--) {
				if(object.ReferenceEquals(list[i], list[i - 1]))
					list.RemoveAt(i);
			}
			return list;
		}
		DataColumnInfo GetFieldColumnInfo(PivotGridFieldBase field) {
			DataColumnInfo columnInfo = DataController.Columns[GetFieldName(field)];
			if(columnInfo == null)
				columnInfo = DataController.Columns[NullableColumn.Name];
			if(columnInfo == null)
				return null;
			if(field.IsProcessOnSummaryLevel || GridData.OptionsData.IsProcessExpressionOnSummaryLevel)
				DataController.ExpColumns.AddExpNameHash(field.ExpressionFieldName, columnInfo);
			columnInfo.Tag = field;
			return columnInfo;
		}
		void SetColumnHandles(PivotGridFieldBase field) {
			DataColumnInfo columnInfo = DataController.Columns[GetFieldName(field)];
			field.SetColumnHandle(columnInfo != null ? columnInfo.Index : -1);
			if(field.GroupInterval != PivotGroupInterval.Default) {
				columnInfo = DataController.Columns[field.FieldName];
				field.GroupIntervalColumnHandle = columnInfo != null ? columnInfo.Index : -1;
			} else {
				field.GroupIntervalColumnHandle = -1;
			}
		}
		Type IPivotGridDataSource.GetFieldType(PivotGridFieldBase field, bool raw) {
			Type columnType = GetFieldTypeCore(field);
			if(raw || columnType == null || field.Area != PivotArea.DataArea)
				return columnType;
			switch(field.SummaryType) {
				case PivotSummaryType.Average:
				case PivotSummaryType.Sum:
					return typeof(decimal);
				case PivotSummaryType.Count:
					return typeof(int);
				case PivotSummaryType.Max:
				case PivotSummaryType.Min:
					return columnType;
				case PivotSummaryType.StdDev:
				case PivotSummaryType.StdDevp:
				case PivotSummaryType.Var:
				case PivotSummaryType.Varp:
					return typeof(double);
				case PivotSummaryType.Custom:
					return typeof(object);
				default:
					throw new Exception("Unknown summary type.");
			}
		}
		bool IPivotGridDataSource.IsFieldTypeCheckRequired(PivotGridFieldBase field) {
			return true;
		}
		Type GetFieldTypeCore(PivotGridFieldBase field) {
			if(field.IsUnbound)
				return typeof(object);
			DataColumnInfo columnInfo = GetColumnInfo(field);
			Type res = columnInfo != null ? columnInfo.Type : null;
			if(res == null)
				return res;
			Type underlyingType = Nullable.GetUnderlyingType(res);
			if(underlyingType != null)
				res = underlyingType;
			return res;
		}
		bool IPivotGridDataSource.ChangeFieldSortOrder(PivotGridFieldBase field) {
			if(field.ColumnHandle > -1 && field.ColumnHandle < DataController.Columns.Count) {
				if(field.Group != null)
					ClearGroupColumns();
				DataController.ChangeFieldSortOrder(field.Area == PivotArea.ColumnArea, field.AreaIndex);
				return true;
			}
			return false;
		}
		bool IPivotGridDataSource.ChangeFieldSummaryType(PivotGridFieldBase field, PivotSummaryType oldSummaryType) {
			try {
				if(field.SummaryType == PivotSummaryType.Custom || HasSummaryFilters)
					return false;
				PivotGridFieldReadOnlyCollection sortedFields = Owner.GetSortedFields();
				List<PivotGridFieldBase> dependingFields = GetUnboundExpressionDependingFields(sortedFields);
				if(dependingFields.Contains(field))
					return false;
				dependingFields = GetSortBySummaryDependingFields(sortedFields);
				if(dependingFields.Contains(field))
					return false;
				return true;
			} finally {
				PivotSummaryItem summaryItem = GetSummaryByField(field, oldSummaryType);
				if(summaryItem != null)
					summaryItem.ChangeSummaryType(field.SummaryType);
				field.SummaryFilter.Clear();
			}
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			return DataController.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}
		PivotCellValue IPivotGridDataSource.GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			return GetCellValue(columnIndex, rowIndex, dataIndex, summaryType);
		}
		int IPivotGridDataSource.GetVisibleIndexByValues(bool isColumn, object[] values) {
			return DataController.GetVisibleIndexByValues(isColumn, values);
		}
		int IPivotGridDataSource.GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return DataController.GetNextOrPrevVisibleIndex(isColumn, visibleIndex, isNext);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return DataController.IsObjectCollapsed(isColumn, visibleIndex);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, object[] values) {
			return DataController.IsObjectCollapsed(isColumn, DataController.GetVisibleIndexByValues(isColumn, values));
		}
		bool IPivotGridDataSource.ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			DataController.ChangeExpanded(isColumn, visibleIndex, expanded, false);
			return true;
		}
		bool IPivotGridDataSource.ChangeExpandedAll(bool isColumn, bool expanded) {
			DataController.ChangeExpanded(isColumn, expanded);
			return true;
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(!field.IsColumnOrRow)
				throw new Exception("Cannot expand data and filter fields");
			DataController.ChangeFieldExpanded(field.Area == PivotArea.ColumnArea, field.AreaIndex, expanded);
			return true;
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(!field.IsColumnOrRow)
				throw new Exception("Cannot expand data and filter fields");
			DataController.ChangeFieldExpanded(field.Area == PivotArea.ColumnArea, field.AreaIndex, expanded, value);
			return true;
		}
		object IPivotGridDataSource.GetFieldValue(bool isColumn, int columnRowIndex, int areaIndex) {
			return DataController.GetFieldValue(isColumn, columnRowIndex, areaIndex);
		}
		bool IPivotGridDataSource.GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex) {
			return DataController.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		object[] IPivotGridDataSource.GetUniqueFieldValues(PivotGridFieldBase field) {
			return GetUniqueFieldValues(field);
		}
		object[] IPivotGridDataSource.GetSortedUniqueValues(PivotGridFieldBase field) {
			return GetSortedUniqueFieldValues(field);
		}
		object[] IPivotGridDataSource.GetAvailableFieldValues(PivotGridFieldBase field, bool deferUpdates, ICustomFilterColumnsProvider customFilters) {
			return GetAvailableFieldValuesCore(field, deferUpdates);
		}
		List<object> IPivotGridDataSource.GetVisibleFieldValues(PivotGridFieldBase field) {
			return GetVisibleFieldValuesCore(field, false);
		}
		List<object> IPivotGridDataSource.GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			return GetSortedUniqueGroupValuesCore(group, parentValues);
		}
		bool IPivotGridDataSource.GetIsEmptyGroupFilter(PivotGridGroup group) {
			return GetIsEmptyGroupFilterCore(group);
		}
		bool? IPivotGridDataSource.IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			return ContainsGroupFilterValueCore(group, parentValues, value);
		}
		PivotSummaryInterval IPivotGridDataSource.GetSummaryInterval(PivotGridFieldBase dataField, bool visibleValuesOnly,
				bool customLevel, PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			int summaryIndex = GetSummaryIndexByField(dataField),
				rowLevel = GetFieldLevel(rowField),
				columnLevel = GetFieldLevel(columnField);
			return DataController.GetSummaryInterval(summaryIndex, visibleValuesOnly, customLevel, rowLevel, columnLevel);
		}
		bool IPivotGridDataSource.HasNullValues(PivotGridFieldBase field) {
			if(field.UnboundExpressionMode == UnboundExpressionMode.UseAggregateFunctions && field.UnboundType != UnboundColumnType.Bound)
				return string.IsNullOrEmpty(field.UnboundExpression);
			return field.ColumnHandle < 0 ? false : DataController.HasNullValues(field.ColumnHandle);
		}
		bool IPivotGridDataSource.HasNullValues(string dataMember) {
			DataColumnInfo column = dataController.Columns[dataMember];
			if(column == null)
				return false;
			return DataController.HasNullValues(column.Index);
		}
		int IPivotGridDataSource.GetCellCount(bool isColumn) { return DataController.GetCellCount(isColumn); }
		int IPivotGridDataSource.GetObjectLevel(bool isColumn, int columnRowIndex) { return DataController.GetObjectLevel(isColumn, columnRowIndex); }
		void IPivotGridDataSource.SaveCollapsedStateToStream(Stream stream) {
			DataController.SaveCollapsedStateToStream(stream);
		}
		void IPivotGridDataSource.WebSaveCollapsedStateToStream(Stream stream) {
			DataController.WebSaveCollapsedStateToStream(stream);
		}
		void IPivotGridDataSource.SaveDataToStream(Stream stream, bool compressed) {
			DataController.SaveDataToStream(stream, compressed);
		}
		void IPivotGridDataSource.LoadCollapsedStateFromStream(Stream stream) {
			DataController.LoadCollapsedStateFromStream(stream);
		}
		void IPivotGridDataSource.WebLoadCollapsedStateFromStream(Stream stream) {
			DataController.WebLoadCollapsedStateFromStream(stream);
		}
		bool IPivotGridDataSource.IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			return IsAreaAllowedCore(field, area);
		}
		string[] IPivotGridDataSource.GetFieldList() { return GetFieldListCore(); }
		string GetFieldCaption(string fieldName) {
			return GetFieldCaptionCore(fieldName);
		}
		string IPivotGridDataSource.GetFieldCaption(string fieldName) { return GetFieldCaption(fieldName); }
		string IPivotGridDataSource.GetLocalizedFieldCaption(string fieldName) { return GetFieldCaption(fieldName); }
		bool IPivotGridDataSource.IsUnboundExpressionValid(PivotGridFieldBase field) {
			if(string.IsNullOrEmpty(field.UnboundExpression))
				return true;
			if(field.IsColumnOrRow)
				return DataController.GetRowValue(0, field.ColumnHandle) != PivotSummaryValue.ErrorValue;
			else
				return DataController.GetCellValue(-1, -1, field.AreaIndex) != PivotSummaryValue.ErrorValue;
		}
		bool IPivotGridDataSource.IsFieldReadOnly(PivotGridFieldBase field) {
			if(field.ColumnHandle >= 0 && field.ColumnHandle < DataController.Columns.Count)
				return DataController.Columns[field.ColumnHandle].ReadOnly;
			return false;
		}
		void IPivotGridDataSource.OnInitialized() {
		}
		void IPivotGridDataSource.HideDataField(PivotGridFieldBase field, int dataIndex) {
			DataController.SafeRemoveSummaryItem(field);
		}
		void IPivotGridDataSource.MoveDataField(PivotGridFieldBase field, int oldIndex, int newIndex) {
			if(oldIndex >= 0 && oldIndex < DataController.Summaries.Count && newIndex >= 0 && newIndex < DataController.Summaries.Count)
				DataController.MoveSummaryItem(field, oldIndex, newIndex);
		}
		#endregion
		protected virtual PivotCellValue GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			object value = DataController.GetCellValue(columnIndex, rowIndex, dataIndex, summaryType);
			if(summaryType == PivotSummaryType.Custom) {
				PivotGridCustomValues customValues = value as PivotGridCustomValues;
				if(customValues != null) {
					PivotGridFieldBase field = GridData.GetFieldByArea(PivotArea.DataArea, dataIndex);
					if(!field.IsSummaryExpressionDataField)
						value = customValues[field];
				}
			}
			return new PivotCellValue(value);
		}
		protected virtual List<object> GetSortedUniqueGroupValuesCore(PivotGridGroup group, object[] parentValues) {
			return DataController.GetUniqueGroupValues(parentValues, GetColumns(group));
		}
		protected bool GetIsEmptyGroupFilterCore(PivotGridGroup group) {
			return DataController.GetIsEmptyGroupFilter(group.FilterValues.Values, GetColumns(group));
		}
		protected bool? ContainsGroupFilterValueCore(PivotGridGroup group, object[] parentValues, object value) {
			return DataController.ContainsGroupFilterValue(group.FilterValues.Values, parentValues, value, GetColumns(group));
		}
		void ClearGroupColumns() {
			if(groupColumns != null)
				groupColumns.Clear();
		}
		internal PivotColumnInfo[] GetColumns(PivotGridGroup group) {
			if(groupColumns == null)
				groupColumns = new Dictionary<PivotGridGroup, PivotColumnInfo[]>();
			PivotColumnInfo[] res;
			if(groupColumns.TryGetValue(group, out res))
				return res;
			int count = group.Count;
			res = new PivotColumnInfo[count];
			bool useSortOrder = !IsFilterGroup(group);
			for(int i = 0; i < count; i++) {
				res[i] = GetColumn(group[i], useSortOrder);
			}
			groupColumns.Add(group, res);
			return res;
		}
		PivotColumnInfo GetColumn(PivotGridFieldBase field, bool useSortOrder) {
			ColumnSortOrder columnSortOrder = useSortOrder ? GetColumnSortOrder(field) : ColumnSortOrder.Ascending;
			return PivotColumnInfo.CreatePivotColumnInfo(GetFieldColumnInfo(field), columnSortOrder, field);
		}
		bool IsFilterGroup(PivotGridGroup group) {
			return !group.Visible || group.Area == PivotArea.FilterArea;
		}
		protected virtual object[] GetUniqueFieldValuesCore(PivotGridFieldBase field, bool sortValues) {
			object[] res = DataController.GetUniqueFieldValues(field.ColumnHandle, field.Group != null || field.FilterValues.ShowBlanks);
			if(sortValues)
				Array.Sort(res, new UniqueValuesComparer(this, field));
			return res;
		}
		object[] GetUniqueFieldValues(PivotGridFieldBase field) {
			return GetUniqueFieldValuesCore(field, false);
		}
		object[] GetSortedUniqueFieldValues(PivotGridFieldBase field) {
			return GetUniqueFieldValuesCore(field, true);
		}
		object[] GetAvailableFieldValuesCore(PivotGridFieldBase field, bool deferUpdates) {
			object[] result = DataController.GetAvailableFieldValues(field.ColumnHandle, deferUpdates);
			Array.Sort(result, new UniqueValuesComparer(this, field));
			return result;
		}
		protected virtual List<object> GetVisibleFieldValuesCore(PivotGridFieldBase field, bool sortValues) {
			object[] res = DataController.GetVisibleFieldValues(field.ColumnHandle, field.Group != null || field.FilterValues.ShowBlanks);
			if(sortValues)
				Array.Sort(res, new UniqueValuesComparer(this, field));
			return new List<object>(res);
		}
		class UniqueValuesComparer : IComparer {
			PivotGridNativeDataSource ds;
			PivotGridFieldBase field;
			ColumnSortOrder sortOrder;
			public UniqueValuesComparer(PivotGridNativeDataSource ds, PivotGridFieldBase field) {
				this.ds = ds;
				this.field = field;
				this.sortOrder = GetColumnSortOrder(field);
			}
			#region IComparer Members
			public int Compare(object x, object y) {
				if(field.ActualSortMode == PivotSortMode.Default || field.ActualSortMode == PivotSortMode.Value) {
					return CompareDefault(x, y);
				}
				int? dcsc = ds.DataControllerSortCell(-1, -1, x, y, sortOrder, field);
				if(dcsc.HasValue)
					return dcsc.Value;
				return CompareDefault(x, y);
			}
			int CompareDefault(object x, object y) {
				int res = Comparer.Default.Compare(x, y);
				if(sortOrder == ColumnSortOrder.Descending)
					res = -res;
				return res;
			}
			#endregion
		}
		protected virtual bool IsAreaAllowedCore(PivotGridFieldBase field, PivotArea area) {
			return true;
		}
		protected virtual void RetrieveFieldsCore(PivotArea area, bool visible) {
			if(DataController.Columns.Count == 0)
				DataController.PopulateColumns();
			else
				DataController.RePopulateColumns();
			for(int i = 0; i < DataController.Columns.Count; i++) {
				if(DataController.Columns[i].Name != NullableColumn.Name && !DataController.Columns[i].Unbound)
					RetrieveField(DataController.Columns[i], area, visible);
			}
		}
		protected void CreateField(PivotArea area, string name, string caption, bool visible) {
			Owner.CreateField(area, name, caption, String.Empty, visible);
		}
		protected virtual void RetrieveField(DataColumnInfo columnInfo, PivotArea area, bool visible) {
			CreateField(area, columnInfo.Name, null, visible);
		}
		protected virtual string[] GetFieldListCore() {
			if(DataController.Columns.Count == 0)
				return null;
			List<string> list = new List<string>();
			for(int n = 0; n < DataController.Columns.Count; n++) {
				if(!DataController.Columns[n].Unbound)
					list.Add(DataController.Columns[n].Name);
			}
			return list.ToArray();
		}
		protected virtual string GetFieldCaptionCore(string fieldName) {
			if(DataController.Columns.Count == 0)
				return null;
			DataColumnInfo column = DataController.Columns[fieldName];
			if(column == null || column == DataController.Columns[NullableColumn.Name])
				return null;
#if !DXPORTABLE
			if(column.PropertyDescriptor == null)
				return column.Caption;
			AnnotationAttributes columnAnnotationAttributes = new DevExpress.Data.Utils.AnnotationAttributes(column.PropertyDescriptor);
			string name = DevExpress.Data.Utils.AnnotationAttributes.GetColumnCaption(columnAnnotationAttributes);
			if(name == null)
				return column.Caption;
			return name;
#else
		return column.Caption;
#endif
		}
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties {
			get { return !IsDesignMode; }
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			for(int i = 0; i < missingComplexColumns.Count; i++) {
				string columnName = missingComplexColumns[i].FieldName;
				if(DataController.Columns[columnName] == null)
					res.Add(columnName);
			}
			return res;
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter {
			get {
				return false; 
			}
		}
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return true;	
		}
		void MakeComplexColumnList() {
			missingComplexColumns.Clear();
			for(int i = 0; i < GridData.Fields.Count; i++) {
				if(GridData.Fields[i].IsComplex)
					missingComplexColumns.Add(GridData.Fields[i]);
			}
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(collection == null || GridData.Fields.Count == 0)
				return collection;
			for(int i = 0; i < GridData.Fields.Count; i++) {
				PivotGridFieldBase field = GridData.Fields[i];
				if(field.UnboundType == UnboundColumnType.Bound && collection.Find(field.FieldName, false) == null) {
					collection.Find(field.FieldName, true);	
				}
				if(field.IsComplex && collection.Find(field.FieldName, false) != null) {
					missingComplexColumns.Remove(field);
				}
			}
			return collection;
		}
		List<PivotGridFieldBase> missingComplexColumns = new List<PivotGridFieldBase>();
		#endregion
		int tempfieldnamecounter = 0; 
		string IUniqueFieldNameGenerator.Generate(string prefix) {
			tempfieldnamecounter++;
			while(Fields.GetFieldByNameOrDataControllerColumnName(prefix + tempfieldnamecounter) != null)
				tempfieldnamecounter++;
			return prefix + tempfieldnamecounter;
		}
	}
}
