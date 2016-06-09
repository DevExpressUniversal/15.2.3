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

using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.ChartDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using DevExpress.Charts.Native;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.Utils;
using System.Linq;
using DevExpress.Data.PivotGrid;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using System.Globalization;
#if SL
using PropertyDescriptor =  DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPivotGrid {
	class ChartDataEnumerator : IEnumerator {
		readonly PivotChartDataSourceBase chartDataSource;
		PivotChartDataSourceBase ChartDataSource { get { return chartDataSource; } }
		ICollection Collection { get { return (ICollection)ChartDataSource; } }
		IList List { get { return (IList)ChartDataSource; } }
		int index;
		int Index { get { return index; } set { index = value; } }
		public ChartDataEnumerator(PivotChartDataSourceBase chartDataSource) {
			this.chartDataSource = chartDataSource;
			Reset();
		}
		#region IEnumerator Members
		object IEnumerator.Current { get { return List[Index]; } }
		bool IEnumerator.MoveNext() {
			if(!ChartDataSource.IsChartDataValid)
				throw new Exception("Invalid enumerator");
			if(Collection.Count == 0 || Index == Collection.Count - 1)
				return false;
			Index++;
			return true;
		}
		public void Reset() { Index = -1; }
		#endregion
	}
	class ByColumnsRowComparer : IComparer<PivotChartDataSourceRowBase> {
		#region IComparer<PivotChartDataSourceRowBase> Members
		int IComparer<PivotChartDataSourceRowBase>.Compare(PivotChartDataSourceRowBase a, PivotChartDataSourceRowBase b) {
			if(a.Cell.X < b.Cell.X)
				return -1;
			if(a.Cell.X > b.Cell.X)
				return 1;
			if(a.Cell.Y < b.Cell.Y)
				return -1;
			if(a.Cell.Y > b.Cell.Y)
				return 1;
			return 0;
		}
		#endregion
	}
	class ByRowsRowComparer : IComparer<PivotChartDataSourceRowBase> {
		#region IComparer<PivotChartDataSourceRowBase> Members
		int IComparer<PivotChartDataSourceRowBase>.Compare(PivotChartDataSourceRowBase a, PivotChartDataSourceRowBase b) {
			if(a.Cell.Y < b.Cell.Y)
				return -1;
			if(a.Cell.Y > b.Cell.Y)
				return 1;
			if(a.Cell.X < b.Cell.X)
				return -1;
			if(a.Cell.X > b.Cell.X)
				return 1;
			return 0;
		}
		#endregion
	}
	class PivotChartCellsLevel {
		int columnLevel, rowLevel;
		public PivotChartCellsLevel(int columnLevel, int rowLevel) {
			this.columnLevel = columnLevel;
			this.rowLevel = rowLevel;
		}
		public int ColumnLevel { get { return columnLevel; } }
		public int RowLevel { get { return rowLevel; } }
	}
	public class PivotChartDataSourceBase : IPivotGrid, ITypedList, IBindingList {
		readonly PivotGridData data;
		bool isBuildingDataSource;
		List<PivotChartDataSourceRowBase> dataSourceCells;
		PropertyDescriptorCollection chartProps;
		Type cellValueType;
		Type argumentValueType;
		Type seriesValueType;
		int lockListChangedCounter;
		bool isMaxSeriesCountEnabled;
		bool isMaxPointCountBySeriesEnabled;
		bool buildDateTimeMiddleValue;
		DateTimeValueBuilder argumentDateTimeValueBuilder, seriesDateTimeValueBuilder;
		DateTimeMeasureUnitNative? minDateTimeMeasureUnit;
		Dictionary<DateTime, DateTimeMeasureUnitNative> dateTimeMeasureUnitByArgument;
		Dictionary<int, object> argumentFieldValuesHash;
		Dictionary<int, object> seriesFieldValuesHash;
		Dictionary<int, int> pointsCountBySeriesIndex;
		Dictionary<int, object> seriesIndexesCache;
		bool autoProvideDataByColumns;
		public PivotChartDataSourceBase(PivotGridData data) {
			this.data = data;
			this.isBuildingDataSource = false;
			this.dataSourceCells = null;
			this.chartProps = null;
			this.cellValueType = null;
			this.shouldBuildArgumentDateTimeValue = false;
			this.shouldBuildSeriesDateTimeValue = false;
			this.minDateTimeMeasureUnit = new DateTimeMeasureUnitNative?();
			this.dateTimeMeasureUnitByArgument = new Dictionary<DateTime, DateTimeMeasureUnitNative>();
			this.argumentFieldValuesHash = new Dictionary<int, object>();
			this.seriesFieldValuesHash = new Dictionary<int, object>();
			this.pointsCountBySeriesIndex = new Dictionary<int, int>();
			this.seriesIndexesCache = new Dictionary<int, object>();
			this.lockListChangedCounter = 0;
			this.isMaxSeriesCountEnabled = true;
			this.isMaxPointCountBySeriesEnabled = true;
			this.buildDateTimeMiddleValue = false;
			this.autoProvideDataByColumns = Options.ProvideDataByColumns;
		}
		event EventHandler dataBuilded;
		public event EventHandler DataBuilded {
			add { dataBuilded += value; }
			remove { dataBuilded -= value; }
		}
		void RaiseDataBuilded() {
			if(dataBuilded != null)
				dataBuilded(this, null);
		}
		public bool IsListChangedLocked { get { return lockListChangedCounter > 0; } }
		protected PivotGridData Data { get { return data; } }
		public virtual PivotGridOptionsChartDataSourceBase Options { get { return Data.OptionsChartDataSource; } }
		bool ProvideDataByColumns { get { return Options.AutoTransposeChart ? autoProvideDataByColumns : Options.ProvideDataByColumns; } }
		public PropertyDescriptorCollection ChartProps { get { return chartProps; } }
		internal Type CellValueType {
			get {
				if(cellValueType == null)
					cellValueType = GetCellValueTypeCore();
				return cellValueType;
			}
		}
		public Type ValuesValueType {
			get { return CellValueType; }
		}
		protected Type GetCellValueTypeCore() {
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(PivotArea.DataArea, false);
			Type res = null;
			for(int i = 0; i < fields.Count; i++) {
				Type fieldType = Data.GetFieldType(fields[i], false);
				if((res != null && res != fieldType) || fieldType == typeof(object) || fieldType == null)
					return typeof(decimal);
				res = fieldType;
			}
			return res != null ? res : typeof(decimal);
		}
		public Type ArgumentValueType {
			get {
				if(argumentValueType == null) {
					if(Options.FieldValuesProvideMode == PivotChartFieldValuesProvideMode.Value || !ContainsAnyArgumentTotals)
						argumentValueType = GetFieldValueType(true);
					else
						argumentValueType = typeof(string);
				}
				return argumentValueType;
			}
		}
		internal Type ColumnValueType {
			get { return GetIsArgument(true) ? ArgumentValueType : SeriesValueType; }
		}
		public Type SeriesValueType {
			get {
				if(seriesValueType == null) {
					if(!ContainsAnySeriesTotals)
						seriesValueType = GetFieldValueType(false);
					else
						seriesValueType = typeof(string);
				}
				return seriesValueType;
			}
		}
		internal Type RowValueType {
			get { return GetIsArgument(false) ? ArgumentValueType : SeriesValueType; }
		}
		bool ContainsAnyArgumentTotals {
			get { return ProvideDataByColumns ? Options.ContainsAnyRowTotals : Options.ContainsAnyColumnTotals; }
		}
		bool ContainsAnySeriesTotals {
			get { return ProvideDataByColumns ? Options.ContainsAnyColumnTotals : Options.ContainsAnyRowTotals; }
		}
		bool shouldBuildArgumentDateTimeValue;
		bool shouldBuildSeriesDateTimeValue;
		void SetShouldBuildDateTimeValue(bool isArgument) {
			if(isArgument)
				shouldBuildArgumentDateTimeValue = true;
			else
				shouldBuildSeriesDateTimeValue = true;
		}
		void ResetShouldBuildDateTimeValue(bool isArgument) {
			if(isArgument)
				shouldBuildArgumentDateTimeValue = false;
			else
				shouldBuildSeriesDateTimeValue = false;
		}
		bool ShouldBuildDateTimeValue(bool isArgument) {
			return isArgument ? shouldBuildArgumentDateTimeValue : shouldBuildSeriesDateTimeValue;
		}
		protected Type GetFieldValueType(bool isArgument) {
			if(Options.FieldValuesProvideMode == PivotChartFieldValuesProvideMode.DisplayText)
				return typeof(string);
			bool isColumn = GetIsColumn(isArgument);
			List<PivotGridFieldBase> fields = GetVisibleFields(isColumn);
			if(fields.Count == 0 || HasSomeDataFields(isColumn))
				return typeof(string);
			ResetShouldBuildDateTimeValue(isArgument);
			if(CanBuildDateTime(fields)) {
				SetShouldBuildDateTimeValue(isArgument);
				return typeof(DateTime);
			} else {
				if(fields.Count == 1) {
					PivotGridFieldBase aloneField = fields[0];
					if(aloneField.TopValueCount > 0 && aloneField.TopValueShowOthers)
						return typeof(string);
					if(aloneField.GroupInterval == PivotGroupInterval.Default) {
						Type type = Data.GetFieldType(aloneField);
						return (type == null) ? typeof(string) : type;
					} else {
						if(Options.FieldValuesProvideMode == PivotChartFieldValuesProvideMode.Default)
							return typeof(string);
						else {
							return DateTimeValueBuilderHelper.ValueTypeByGroupInterval[aloneField.GroupInterval];
						}
					}
				}
			}
			return typeof(string);
		}
		protected List<PivotGridFieldBase> GetVisibleFields(bool isColumn) {
			PivotArea area = isColumn ? PivotArea.ColumnArea : PivotArea.RowArea;
			List<PivotGridFieldBase> fields = Data.GetFieldsByArea(area, false);
			for(int i = fields.Count - 1; i >= 0; i--) {
				if(!fields[i].Visible)
					fields.Remove(fields[i]);
			}
			return fields;
		}
		bool CanBuildDateTime(List<PivotGridFieldBase> fields) {
			if(Options.FieldValuesProvideMode == PivotChartFieldValuesProvideMode.DisplayText)
				return false;
			if(fields.Count == 0)
				return false;
			string fieldName = fields[0].FieldName;
			bool containsFullDateTime = false;
			if(!IsFieldDateTimeBuildable(fields[0]))
				return false;
			foreach(PivotGridFieldBase field in fields) {
				if(field.GroupInterval == PivotGroupInterval.Default) {
					containsFullDateTime = true;
					if(Data.GetFieldType(field) != typeof(DateTime))
						return false;
				} else if(!DateTimeValueBuilderHelper.IsDateTimeGroupInterval[field.GroupInterval]) {
					return false;
				}
				if(field.FieldName != fieldName)
					return false;
			}
			return containsFullDateTime ? true : CheckGroupIntervalDeps(fields);
		}
		bool HasSomeDataFields(bool isColumn) {
			bool isDataFieldColumn = Data.DataField.Area == PivotArea.ColumnArea;
			return isDataFieldColumn == isColumn && VisibleDataFieldsCount(Data.GetFieldsByArea(PivotArea.DataArea, true)) > 1;
		}
		int VisibleDataFieldsCount(List<PivotGridFieldBase> dataFields) {
			if(dataFields == null)
				return 0;
			int count = 0;
			foreach(PivotGridFieldBase field in dataFields) {
				if(field.Options.ShowValues)
					count++;
			}
			return count;
		}
		bool IsFieldDateTimeBuildable(PivotGridFieldBase field) {
			if(field.GroupInterval == PivotGroupInterval.Default) {
				if(Data.GetFieldType(field) != typeof(DateTime))
					return false;
			} else {
				if(!DateTimeValueBuilderHelper.IsGroupIntervalWithYear[field.GroupInterval])
					return false;
			}
			if(field.TopValueCount > 0 && field.TopValueShowOthers)
				return false;
			return true;
		}
		bool CheckGroupIntervalDeps(List<PivotGridFieldBase> fields) {
			if(fields == null || fields.Count == 0)
				return false;
			List<PivotGroupInterval> allGroupIntervals = new List<PivotGroupInterval>();
			foreach(PivotGridFieldBase field in fields)
				allGroupIntervals.Add(field.GroupInterval);
			DateTimeMeasureUnitComparer comparer = new DateTimeMeasureUnitComparer();
			List<PivotGroupInterval> minIntervals = new List<PivotGroupInterval>();
			DateTimeMeasureUnitNative? minMeasure = DateTimeValueBuilderHelper.DateTimeMeasureUnitByGroupInterval[allGroupIntervals[0]];
			foreach(PivotGroupInterval interval in allGroupIntervals) {
				DateTimeMeasureUnitNative? measure = DateTimeValueBuilderHelper.DateTimeMeasureUnitByGroupInterval[interval];
				int compareResult = comparer.Compare(measure, minMeasure);
				if(compareResult <= 0) {
					if(compareResult < 0) {
						minMeasure = measure;
						minIntervals.Clear();
					}
					minIntervals.Add(interval);
				}
			}
			foreach(PivotGroupInterval groupInterval in minIntervals)
				if(CheckGroupIntervalDepsCore(groupInterval, allGroupIntervals))
					return true;
			return false;
		}
		bool CheckGroupIntervalDepsCore(PivotGroupInterval gi, List<PivotGroupInterval> all) {
			List<PivotGroupInterval> deps = DateTimeValueBuilderHelper.GroupIntervalDeps[gi];
			if(deps.Count == 0)
				return true;
			foreach(PivotGroupInterval dep in deps) {
				if(all.Contains(dep) && CheckGroupIntervalDepsCore(dep, all))
					return true;
			}
			return false;
		}
		string GetChartText(PivotFieldValueItem item) {
			string result = item.DisplayText;
			if(item.StartLevel == 0)
				return result;
			while((item = this.GetParentItem(item.IsColumn, item)) != null)
				result = item.GetCustomText(false) + " | " + result;
			return result;
		}
		object GetChartValue(PivotFieldValueItem item) {
			bool isArgument = GetIsArgument(item.IsColumn);
			if(ShouldBuildDateTimeValue(isArgument)) {
				DateTimeValue dateTimeValue = BuildDateTimeValue(item, isArgument);
				if(dateTimeValue == null)
					return DBNull.Value;
				AddDateTimeValueToHash(dateTimeValue);
				return dateTimeValue.Value;
			}
			return item.Value;
		}
		void AddDateTimeValueToHash(DateTimeValue dateTimeValue) {
			DateTimeMeasureUnitNative? measureUnit = dateTimeValue.MeasureUnit;
			if(measureUnit.HasValue)
				dateTimeMeasureUnitByArgument[dateTimeValue.Value] = measureUnit.Value;
			if(!minDateTimeMeasureUnit.HasValue)
				minDateTimeMeasureUnit = measureUnit;
			int compareResult = (new DateTimeMeasureUnitComparer()).Compare(measureUnit, minDateTimeMeasureUnit);
			if(compareResult < 0)
				minDateTimeMeasureUnit = measureUnit;
		}
		void ClearFieldValuesHashes() {
			minDateTimeMeasureUnit = new DateTimeMeasureUnitNative?();
			dateTimeMeasureUnitByArgument = new Dictionary<DateTime, DateTimeMeasureUnitNative>();
			seriesFieldValuesHash = new Dictionary<int, object>();
			argumentFieldValuesHash = new Dictionary<int, object>();
			pointsCountBySeriesIndex = new Dictionary<int, int>();
			ClearSeriesIndexesCache();
			InvalidateDateTimeValueBuilder();
		}
		void ClearSeriesIndexesCache() {
			seriesIndexesCache = new Dictionary<int, object>();
		}
		DateTimeValueBuilder ArgumentDateTimeValueBuilder {
			get {
				if(argumentDateTimeValueBuilder == null)
					argumentDateTimeValueBuilder = DateTimeValueBuilder.CreateInstance(BuildDateTimeMiddleValue);
				return argumentDateTimeValueBuilder;
			}
		}
		DateTimeValueBuilder SeriesDateTimeValueBuilder {
			get {
				if(seriesDateTimeValueBuilder == null)
					seriesDateTimeValueBuilder = DateTimeValueBuilder.CreateInstance(false);
				return seriesDateTimeValueBuilder;
			}
		}
		void InvalidateDateTimeValueBuilder() {
			argumentDateTimeValueBuilder = null;
			seriesDateTimeValueBuilder = null;
		}
		internal bool BuildDateTimeMiddleValue {
			get { return buildDateTimeMiddleValue; }
			set {
				buildDateTimeMiddleValue = value;
				ClearFieldValuesHashes();
			}
		}
		DateTimeValue BuildDateTimeValue(PivotFieldValueItem item, bool isArgument) {
			DateTimeValueBuilder builder = isArgument || item.Field == null || item.Field.GroupInterval != PivotGroupInterval.Default ? ArgumentDateTimeValueBuilder : SeriesDateTimeValueBuilder;
			lock (builder) {
				builder.Clear();
				do {
					builder.AddValue(item.Field.GroupInterval, item.Value);
				} while ((item = this.GetParentItem(item.IsColumn, item)) != null);
				return builder.GetValue();
			}
		}
		public PivotChartDataSourceRowBase GetRowItem(int index) {
			return dataSourceCells[index];
		}
		public object GetChartCellValue(int columnLastLevelIndex, int rowLastLevelIndex) {
			PivotGridCellItem cellItem = this.CreateCellItem(columnLastLevelIndex, rowLastLevelIndex);
			if(cellItem == null)
				return null;
			object value = GetChartCellValueCore(cellItem);
			value = GetCustomChartDataSourceDataEventResult(PivotChartItemType.CellItem, null, cellItem, value);
			if(value != null && value != DBNull.Value && Options.ProvideCellValuesAsType != null)
				value = TryCast(value, Options.ProvideCellValuesAsType);
			return value;
		}
		object GetCustomChartDataSourceDataEventResult(PivotChartItemType itemType, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			PivotChartItemDataMember itemDataMember = PivotChartItemTypeEnumExtensions.ToItemDataMember(itemType, ProvideDataByColumns);
			return Data.GetCustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		object TryCast(object value, Type targetType) {
			try {
				return Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
			} catch(InvalidCastException) {
				return NativePivotDrillDownDataSource.GetDefaultValue(targetType);
			}
		}
		object GetChartCellValueCore(PivotGridCellItem cellItem) {
			object res = cellItem.Value;
			if(res == null)
				return DBNull.Value;
			else {
				try {
					return Convert.ChangeType(res, CellValueType, CultureInfo.CurrentCulture);
				} catch {
					return DBNull.Value;
				}
			}
		}
		Dictionary<int, object> GetFieldValuesHash(bool isArgument) {
			if(isArgument)
				return argumentFieldValuesHash;
			return seriesFieldValuesHash;
		}
		object GetChartFieldValueCached(bool isArgument, int lastLevelIndex) {
			Dictionary<int, object> hash = GetFieldValuesHash(isArgument);
			if(hash.ContainsKey(lastLevelIndex))
				return hash[lastLevelIndex];
			object value = GetChartFieldValue(isArgument, lastLevelIndex);
			hash.Add(lastLevelIndex, value);
			return value;
		}
		object GetChartFieldValue(bool isArgument, Point cell) {
			return GetChartFieldValueCached(isArgument, GetLastLevelIndex(isArgument, cell));
		}
		int GetLastLevelIndex(bool isArgument, Point cell) {
			return isArgument ^ ProvideDataByColumns ? cell.X : cell.Y;
		}
		public object GetArgumentValue(Point cell) {
			return GetChartFieldValue(true, cell);
		}
		public object GetSeriesValue(Point cell) {
			return GetChartFieldValue(false, cell);
		}
		bool GetIsColumn(bool isArgument) {
			return isArgument ^ ProvideDataByColumns;
		}
		bool GetIsArgument(bool isColumn) {
			return isColumn ^ ProvideDataByColumns;
		}
		PivotChartItemType GetChartItemType(bool isArgument) {
			return GetIsColumn(isArgument) ? PivotChartItemType.ColumnItem : PivotChartItemType.RowItem;
		}
		object GetChartFieldValue(bool isArgument, int lastLevelIndex) {
			PivotFieldValueItem item = GetLastLevelItem(GetIsColumn(isArgument), lastLevelIndex);
			if(item == null)
				return null;
			PivotChartItemType itemType = GetChartItemType(isArgument);
			Type type = isArgument ? ArgumentValueType : SeriesValueType;
			Type customType = GetIsColumn(isArgument) ? Options.ProvideColumnFieldValuesAsType : Options.ProvideRowFieldValuesAsType;
			object value = GetChartFieldValueCore(item, type);
			if(value != null && value != DBNull.Value)
				value = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
			value = GetCustomChartDataSourceDataEventResult(itemType, item, null, value);
			if(customType != null && value != null && value != DBNull.Value)
				value = Convert.ChangeType(value, customType, CultureInfo.CurrentCulture);
			return value;
		}
		object GetChartFieldValueCore(PivotFieldValueItem item, Type type) {
			object value;
			if(type == typeof(string) || item.IsTotal)
				value = GetChartText(item);
			else
				value = GetChartValue(item);
			return value;
		}
		int CellsCount {
			get {
				EnsureChartData();
				return dataSourceCells.Count;
			}
		}
		bool IsChartDataSourceCellsValid { get { return dataSourceCells != null; } }
		public bool IsChartPropsValid { get { return chartProps != null; } }
		public bool IsChartDataValid { get { return IsChartDataSourceCellsValid && IsChartPropsValid; } }
		public bool InvalidateChartData(bool force) {
			if(!force && (isBuildingDataSource || !Data.IsControlReady))
				return false;
			InvalidateChartDataCore();
			return true;
		}
		public void InvalidateChartData() {
			if(InvalidateChartData(false))
				RaiseListChanged();
		}
		void InvalidateChartDataCore() {
			InvalidateChartDataSourceCellsCore();
			InvalidateChartProps();
		}
		void InvalidateChartProps() {
			chartProps = null;
			argumentValueType = null;
			seriesValueType = null;
		}
		public bool InvalidateChartDataSourceCells() {
			if(isBuildingDataSource || !Data.IsControlReady || !IsChartDataSourceCellsValid)
				return false;
			InvalidateChartDataSourceCellsCore();
			RaiseListChangedReset();
			return true;
		}
		public virtual void OnCellSelectionChanged() { }
		public virtual void OnFocusedCellChanged() { }
		void InvalidateChartDataSourceCellsCore() {
			dataSourceCells = null;
			cellValueType = null;
			ClearFieldValuesHashes();
		}
		public void EnsureChartData() {
			if(!IsChartDataValid) {
				BuildChartData();
				RaiseDataBuilded();
			}
		}
		void BuildChartData() {
			isBuildingDataSource = true;
			if(!IsChartDataSourceCellsValid)
				BuildChartDataSourceCells();
			if(!IsChartPropsValid)
				BuildChartProps();
			isBuildingDataSource = false;
		}
		void BuildChartDataSourceCells() {
			EnsureIsCalculated();
			autoProvideDataByColumns = Options.ProvideDataByColumns;
			CreateChartDataSourceCells();
			if(ShouldTranspose(dataSourceCells)) {
				InvalidateChartDataSourceCellsCore();
				autoProvideDataByColumns = !autoProvideDataByColumns;
				CreateChartDataSourceCells();
				InvalidateChartProps();
			}
		}
		void BuildChartProps() {
			this.EnsureIsCalculated();
			CreatePropertyDescriptors();
		}
		void CreatePropertyDescriptors() {
			if(chartProps == null)
				chartProps = new PropertyDescriptorCollection(null);
			else
				chartProps.Clear();
			chartProps.Add(new PivotChartDescriptor(this, ProvideDataByColumns ? PivotChartDescriptorType.Series : PivotChartDescriptorType.Argument));
			chartProps.Add(new PivotChartDescriptor(this, ProvideDataByColumns ? PivotChartDescriptorType.Argument : PivotChartDescriptorType.Series));
			chartProps.Add(new PivotChartDescriptor(this, PivotChartDescriptorType.Value));
		}
		void CreateChartDataSourceCells() {
			dataSourceCells = GetDataSourceCells();
		}
		List<PivotChartDataSourceRowBase> GetDataSourceCells() {
			PivotGridDataAsync data = Data as PivotGridDataAsync;
			if(data != null && data.IsLocked)
				return CreateDataSourceRowsCollection();
			List<PivotChartDataSourceRowBase> cells = GetCells();
			cells = PrepareCells(cells);
			Data.RaiseCustomChartDataSourceRows(cells);
			return cells;
		}
		protected virtual PivotChartDataSourceRowBase CreateDataSourceRow(Point point) {
			return new PivotChartDataSourceRowBase(this, point);
		}
		protected List<PivotChartDataSourceRowBase> CreateDataSourceRowsCollection() {
			return new List<PivotChartDataSourceRowBase>();
		}
		protected virtual List<PivotChartDataSourceRowBase> GetCells() {
			return AllCells;
		}
		protected List<PivotChartDataSourceRowBase> AllCells {
			get {
				List<PivotChartDataSourceRowBase> cells = CreateDataSourceRowsCollection();
				for(int i = 0; i < ColumnCount; i++) {
					for(int j = 0; j < RowCount; j++)
						cells.Add(CreateDataSourceRow(new Point(i, j)));
				}
				return cells;
			}
		}
		void SortForMaxCount(List<PivotChartDataSourceRowBase> cells) {
			if(IsMaxSeriesCountEnabled || IsMaxPointCountBySeriesEnabled)
				cells.Sort(GetCellsComparer());
		}
		IComparer<PivotChartDataSourceRowBase> GetCellsComparer() {
			if(ProvideDataByColumns)
				return new ByColumnsRowComparer();
			return new ByRowsRowComparer();
		}
		List<PivotChartDataSourceRowBase> PrepareCells(List<PivotChartDataSourceRowBase> cells) {
			PivotChartCellsLevel cellsLowerLevel = GetCellsLevel(cells);
			SortForMaxCount(cells);
			return PrepareCellsCore(cells, cellsLowerLevel);
		}
		List<PivotChartDataSourceRowBase> PrepareCellsCore(List<PivotChartDataSourceRowBase> cells, PivotChartCellsLevel cellsLowerLevel) {
			List<PivotChartDataSourceRowBase> newCells = new List<PivotChartDataSourceRowBase>();
			for(int i = 0; i < cells.Count; i++) {
				var cell = cells[i];
				if(ShouldProvideFieldValue(cell, cellsLowerLevel) && ShouldProvideCell(cell))
					newCells.Add(cell);
			}
			return newCells;
		}
		bool ShouldTranspose(List<PivotChartDataSourceRowBase> cells) {
			if(!Options.AutoTransposeChart)
				return false;
			bool shouldTranspose = false;
			bool canBuildDateTimeOnColumns = CanBuildDateTime(GetVisibleFields(true));
			bool canBuildDateTimeOnRows = CanBuildDateTime(GetVisibleFields(false));
			bool canBuildDataTime = (canBuildDateTimeOnColumns || canBuildDateTimeOnRows);
			int seriesCount = GetAvailableSeriesCount(cells);
			IDictionary<object, int> pointCountInSeries = GetAvailablePointCountInSeries(cells);
			List<int> pointCounts = new List<int>(pointCountInSeries.Values);
			int maxArgumentCount = (pointCounts.Count == 0) ? 0 : pointCounts.Max();
			if(!canBuildDataTime) {
				if(seriesCount > maxArgumentCount)
					shouldTranspose = true;
				else if(seriesCount == maxArgumentCount && ProvideDataByColumns != Options.ProvideDataByColumns)
					shouldTranspose = true;
			}
			if(seriesCount > maxArgumentCount && !canBuildDataTime)
				shouldTranspose = true;
			return shouldTranspose;
		}
		PivotChartCellsLevel GetCellsLevel(IList<PivotChartDataSourceRowBase> cells) {
			if(Options.DataProvideMode == PivotChartDataProvideMode.UseCustomSettings)
				return null;
			int rowLevel = -2;
			int columnLevel = -2;
			int rowMaxColumnLevel = -2;
			int columnMaxRowLevel = -2;
			for(int i = 0; i < cells.Count; i++) {
				PivotChartDataSourceRowBase row = cells[i];
				if(row.ColumnItem.FieldLevel >= columnLevel) {
					if(row.ColumnItem.FieldLevel == columnLevel)
						columnMaxRowLevel = Math.Max(row.RowItem.FieldLevel, columnMaxRowLevel);
					else
						columnMaxRowLevel = row.RowItem.FieldLevel;
					columnLevel = row.ColumnItem.FieldLevel;
				}
				if(row.RowItem.FieldLevel >= rowLevel) {
					if(row.RowItem.FieldLevel == rowLevel)
						rowMaxColumnLevel = Math.Max(row.ColumnItem.FieldLevel, rowMaxColumnLevel);
					else
						rowMaxColumnLevel = row.ColumnItem.FieldLevel;
					rowLevel = row.RowItem.FieldLevel;
				}
			}
			bool containsCells = cells.Any<PivotChartDataSourceRowBase>((row) => {
				return row.ColumnItem.FieldLevel == columnLevel && row.RowItem.FieldLevel == rowLevel;
			});
			if(!containsCells) {
				switch(Options.DataProvidePriority) {
					case PivotChartDataProvidePriority.Columns:
						columnLevel = rowMaxColumnLevel;
						break;
					case PivotChartDataProvidePriority.Rows:
						rowLevel = columnMaxRowLevel;
						break;
				}
			}
			return new PivotChartCellsLevel(columnLevel, rowLevel);
		}
		bool ShouldProvideFieldValue(PivotChartDataSourceRowBase row, PivotChartCellsLevel cellsLowerLevel) {
			bool result = true;
			if(cellsLowerLevel != null) {
				if(ColumnValueType != typeof(string) || RowValueType != typeof(string)) {
					if(ColumnValueType != typeof(string))
						result = result && (row.RowItem.FieldLevel == cellsLowerLevel.RowLevel) && !ShouldRemoveItem(true, row.ColumnItem.ValueType);
					if(RowValueType != typeof(string))
						result = result && (row.ColumnItem.FieldLevel == cellsLowerLevel.ColumnLevel) && !ShouldRemoveItem(false, row.RowItem.ValueType);
					return result;
				} else {
					return row.ColumnItem.FieldLevel == cellsLowerLevel.ColumnLevel && row.RowItem.FieldLevel == cellsLowerLevel.RowLevel;
				}
			}
			return !ShouldRemoveItem(true, row.ColumnItem.ValueType) &&
					!ShouldRemoveItem(false, row.RowItem.ValueType);
		}
		bool ShouldRemoveItem(bool isColumn, PivotGridValueType valueType) {
			bool isArgument = GetIsArgument(isColumn);
			if(valueType != PivotGridValueType.Value && Options.FieldValuesProvideMode == PivotChartFieldValuesProvideMode.Value && isArgument)
				return true;
			switch(valueType) {
				case PivotGridValueType.Total:
					return (isColumn && !Options.ProvideColumnTotals) || (!isColumn && !Options.ProvideRowTotals);
				case PivotGridValueType.CustomTotal:
					return (isColumn && !Options.ProvideColumnCustomTotals) || (!isColumn && !Options.ProvideRowCustomTotals);
				case PivotGridValueType.GrandTotal:
					return (isColumn && !Options.ProvideColumnGrandTotals) || (!isColumn && !Options.ProvideRowGrandTotals);
			}
			return false;
		}
		bool IsMaxSeriesCountEnabled {
			get { return isMaxSeriesCountEnabled; }
			set {
				isMaxSeriesCountEnabled = value;
				ClearSeriesIndexesCache();
			}
		}
		bool IsMaxPointCountBySeriesEnabled {
			get { return isMaxPointCountBySeriesEnabled; }
			set {
				isMaxPointCountBySeriesEnabled = value;
				ClearSeriesIndexesCache();
			}
		}
		protected bool ShouldProvideCell(PivotChartDataSourceRowBase rowItem) {
			int columnIndex = rowItem.Cell.X,
				rowIndex = rowItem.Cell.Y;
			if(!Options.ProvideEmptyCells && rowItem.CellItem.Value == null)
				return false;
			int seriesIndex = Options.ProvideDataByColumns ? columnIndex : rowIndex;
			if(IsMaxSeriesCountEnabled) {
				if(Options.MaxAllowedSeriesCount > 0) {
					if(seriesIndexesCache.Keys.Count > Options.MaxAllowedSeriesCount)
						return false;
					if(!seriesIndexesCache.ContainsKey(seriesIndex))
						seriesIndexesCache.Add(seriesIndex, null);
					if(seriesIndexesCache.Keys.Count > Options.MaxAllowedSeriesCount)
						return false;
				}
			}
			if(IsMaxPointCountBySeriesEnabled) {
				if(Options.MaxAllowedPointCountInSeries > 0) {
					int count = 0;
					if(pointsCountBySeriesIndex.ContainsKey(seriesIndex))
						count = pointsCountBySeriesIndex[seriesIndex];
					pointsCountBySeriesIndex[seriesIndex] = ++count;
					if(count > Options.MaxAllowedPointCountInSeries)
						return false;
				}
			}
			return true;
		}
		internal IDictionary<object, int> AvailablePointCountInSeries {
			get {
				if(Options.MaxAllowedPointCountInSeries <= 0)
					return null;
				IsMaxPointCountBySeriesEnabled = false;
				List<PivotChartDataSourceRowBase> cells = GetDataSourceCells();
				IsMaxPointCountBySeriesEnabled = true;
				return GetAvailablePointCountInSeries(cells);
			}
		}
		IDictionary<object, int> GetAvailablePointCountInSeries(IList<PivotChartDataSourceRowBase> cells) {
			IDictionary<object, int> result = new NullableDictionary<object, int>();
			foreach(PivotChartDataSourceRowBase cell in cells) {
				object value = GetChartFieldValue(false, cell.Cell);
				int count = 0;
				if(result.ContainsKey(value))
					count = result[value];
				result[value] = ++count;
			}
			return result;
		}
		internal int AvailableSeriesCount {
			get {
				if(Options.MaxAllowedSeriesCount <= 0)
					return 0;
				IsMaxSeriesCountEnabled = false;
				IsMaxPointCountBySeriesEnabled = false;
				List<PivotChartDataSourceRowBase> cells = GetDataSourceCells();
				IsMaxSeriesCountEnabled = true;
				IsMaxPointCountBySeriesEnabled = true;
				return GetAvailableSeriesCount(cells);
			}
		}
		int GetAvailableSeriesCount(IList<PivotChartDataSourceRowBase> cells) {
			IDictionary<object, bool> hash = new NullableDictionary<object, bool>();
			foreach(PivotChartDataSourceRowBase cell in cells) {
				hash[GetChartFieldValue(false, cell.Cell)] = true;
			}
			return hash.Count;
		}
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			for(int i = 0; i < ((ICollection)this).Count; i++) {
				if(index + i >= array.Length)
					break;
				array.SetValue(((IList)this)[i], index + i);
			}
		}
		public int Count {
			get { return CellsCount; }
		}
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return null; } }
		#endregion
		#region IList Members
		public int Add(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		public void Clear() {
			throw new Exception("The method or operation is not implemented.");
		}
		public bool Contains(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		public int IndexOf(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		public void Insert(int index, object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		public bool IsFixedSize { get { return true; } }
		public bool IsReadOnly { get { return true; } }
		public void Remove(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		public void RemoveAt(int index) {
			throw new Exception("The method or operation is not implemented.");
		}
		public object this[int index] {
			get {
				EnsureChartData();
				if(index < 0 || index >= CellsCount)
					return null;
				return PivotChartDataSourceRowItem.CreateRow(index, this);
			}
			set {
				throw new Exception("The method or operation is not implemented.");
			}
		}
		#endregion
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new Exception("AddIndex : The method or operation is not implemented.");
		}
		object IBindingList.AddNew() {
			throw new Exception("AddNew : The method or operation is not implemented.");
		}
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new Exception("ApplySort : The method or operation is not implemented.");
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new Exception("Find : The method or operation is not implemented.");
		}
		bool IBindingList.IsSorted { get { return false; } }
		ListChangedEventHandler listChanged;
		public event ListChangedEventHandler ListChanged {
			add { this.listChanged += value; }
			remove { this.listChanged -= value; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new Exception("RemoveIndex : The method or operation is not implemented.");
		}
		void IBindingList.RemoveSort() {
			throw new Exception("RemoveSort : The method or operation is not implemented.");
		}
		ListSortDirection IBindingList.SortDirection { get { return ListSortDirection.Ascending; } }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
		public void RaiseListChanged() {
			if(!Data.CanRaiseListChanged)
				return;
			RaiseListChangedProps();
			RaiseListChangedReset();
		}
		public void RaiseListChangedProps() {
			if(!IsListChangedLocked)
				RaiseListChangedPropsCore();
		}
		void RaiseListChangedPropsCore() {
			if(listChanged != null)
				listChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0));
			if(dataChangedEventHandler != null)
				dataChangedEventHandler(this, new DataChangedEventArgs(DataChangedType.Reset));
		}
		public void RaiseListChangedReset() {
			if(!IsListChangedLocked)
				RaiseListChangedResetCore();
		}
		void RaiseListChangedResetCore() {
			if(listChanged != null)
				listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
			if(dataChangedEventHandler != null)
				dataChangedEventHandler(this, new DataChangedEventArgs(DataChangedType.ItemsChanged));
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			EnsureChartData();
			return new ChartDataEnumerator(this);
		}
		#endregion
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			EnsureChartData();
			return listAccessors != null && listAccessors.Length > 0 ? new PropertyDescriptorCollection(null) : this.ChartProps;
		}
		public virtual string GetListName(PropertyDescriptor[] listAccessors) {
			return "PivotChartDataSourceBase";
		}
		#endregion
		#region IPivotGrid Members
		DataChangedEventHandler dataChangedEventHandler;
		event DataChangedEventHandler IChartDataSource.DataChanged {
			add { dataChangedEventHandler += value; }
			remove { dataChangedEventHandler -= value; }
		}
		IList<string> IPivotGrid.ArgumentColumnNames {
			get {
				List<PivotGridFieldBase> fields = GetVisibleFields(GetIsColumn(true));
				List<string> columnNames = new List<string>();
				foreach(var field in fields)
					columnNames.Add(field.HeaderDisplayText);
				if(columnNames.Count == 0)
					return null;
				return columnNames;
			}
		}
		IList<string> IPivotGrid.ValueColumnNames {
			get {
				List<PivotGridFieldBase> fields = Data.GetFieldsByArea(PivotArea.DataArea, false);
				List<string> columnNames = new List<string>();
				foreach(var field in fields) {
					if(field.Visible)
						columnNames.Add(field.HeaderDisplayText);
				}
				if(columnNames.Count == 0)
					return null;
				return columnNames;
			}
		}
		bool IPivotGrid.RetrieveDataByColumns {
			get { return Options.ProvideDataByColumns; }
			set { Options.ProvideDataByColumns = value; }
		}
		public virtual bool SelectionSupported { get { return false; } }
		public virtual bool SelectionOnly { get { return false; } set { } }
		public virtual bool SinglePageSupported { get { return false; } }
		public virtual bool SinglePageOnly { get { return true; } set { } }
		bool IPivotGrid.RetrieveColumnTotals {
			get { return Options.ProvideColumnTotals; }
			set { Options.ProvideColumnTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnGrandTotals {
			get { return Options.ProvideColumnGrandTotals; }
			set { Options.ProvideColumnGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnCustomTotals {
			get { return Options.ProvideColumnCustomTotals; }
			set { Options.ProvideColumnCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveRowTotals {
			get { return Options.ProvideRowTotals; }
			set { Options.ProvideRowTotals = value; }
		}
		bool IPivotGrid.RetrieveRowGrandTotals {
			get { return Options.ProvideRowGrandTotals; }
			set { Options.ProvideRowGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveRowCustomTotals {
			get { return Options.ProvideRowCustomTotals; }
			set { Options.ProvideRowCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveEmptyCells {
			get { return Options.ProvideEmptyCells; }
			set { Options.ProvideEmptyCells = value; }
		}
		int IPivotGrid.MaxAllowedSeriesCount {
			get { return Options.MaxAllowedSeriesCount; }
			set { Options.MaxAllowedSeriesCount = value; }
		}
		int IPivotGrid.MaxAllowedPointCountInSeries {
			get { return Options.MaxAllowedPointCountInSeries; }
			set { Options.MaxAllowedPointCountInSeries = value; }
		}
		public virtual int UpdateDelay { get { return -1; } set { ; } }
		string IChartDataSource.ValueDataMember { get { return PivotChartDescriptor.ValuesColumn; } }
		string IChartDataSource.ArgumentDataMember { get { return PivotChartDescriptor.ArgumentsColumn; } }
		string IChartDataSource.SeriesDataMember { get { return PivotChartDescriptor.SeriesColumn; } }
		DefaultBoolean IPivotGrid.RetrieveFieldValuesAsText {
			get {
				switch(Options.FieldValuesProvideMode) {
					case PivotChartFieldValuesProvideMode.DisplayText:
						return DefaultBoolean.True;
					case PivotChartFieldValuesProvideMode.Value:
						return DefaultBoolean.False;
					case PivotChartFieldValuesProvideMode.Default:
						return DefaultBoolean.Default;
				}
				return DefaultBoolean.Default;
			}
			set {
				switch(value) {
					case DefaultBoolean.True:
						Options.FieldValuesProvideMode = PivotChartFieldValuesProvideMode.DisplayText;
						break;
					case DefaultBoolean.False:
						Options.FieldValuesProvideMode = PivotChartFieldValuesProvideMode.Value;
						break;
					case DefaultBoolean.Default:
						Options.FieldValuesProvideMode = PivotChartFieldValuesProvideMode.Default;
						break;
				}
			}
		}
		bool IPivotGrid.RetrieveDateTimeValuesAsMiddleValues {
			get { return BuildDateTimeMiddleValue; }
			set { BuildDateTimeMiddleValue = value; }
		}
		DateTimeMeasureUnitNative? IChartDataSource.DateTimeArgumentMeasureUnit {
			get { return minDateTimeMeasureUnit; }
		}
		IDictionary<DateTime, DateTimeMeasureUnitNative> IPivotGrid.DateTimeMeasureUnitByArgument {
			get { return dateTimeMeasureUnitByArgument; }
		}
		int IPivotGrid.AvailableSeriesCount { get { return AvailableSeriesCount; } }
		IDictionary<object, int> IPivotGrid.AvailablePointCountInSeries { get { return AvailablePointCountInSeries; } }
		void IPivotGrid.LockListChanged() {
			lockListChangedCounter++;
		}
		void IPivotGrid.UnlockListChanged() {
			lockListChangedCounter--;
		}
		#endregion
		#region VisualItems wrapper
		protected PivotVisualItemsBase VisualItems { get { return Data.VisualItems; } }
		protected int RowCount { get { return GetCount(false); } }
		protected int ColumnCount { get { return GetCount(true); } }
		protected virtual void EnsureIsCalculated() {
			VisualItems.EnsureIsCalculated();
		}
		protected virtual PivotFieldValueItem GetParentItem(bool isColumn, PivotFieldValueItem item) {
			return VisualItems.GetParentItem(isColumn, item);
		}
		protected virtual object GetCellValue(int columnIndex, int rowIndex) {
			return VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		protected internal virtual PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex) {
			return VisualItems.CreateCellItem(columnIndex, rowIndex);
		}
		protected internal virtual PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex) {
			return VisualItems.GetLastLevelItem(isColumn, lastLevelIndex);
		}
		protected virtual int GetCount(bool isColumn) {
			return isColumn ? VisualItems.ColumnCount : VisualItems.RowCount;
		}
		#endregion
	}
}
