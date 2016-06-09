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
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using System.Linq;
using DevExpress.Compatibility.System;
using System.Threading;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryDataSource<TColumn> : IDataSourceHelpersOwner<TColumn>, ICloneable, IComparable, IPartialUpdaterOwner<TColumn>, IQueryDataSource where TColumn : QueryColumn {
		PivotQueryDataSourceExceptionEventHandler queryException;
		readonly QueryColumns<TColumn> cubeColumns;
		readonly QueryAreas<TColumn> areas;
		IPivotGridDataSourceOwner owner;
		UniqueValues<TColumn> uniqueValues;
		IQueryFilterHelper filterHelper;
		bool autoExpandGroups;
		bool isFullyExpanded;
		protected QueryDataSource() {
			this.cubeColumns = CreateCubeColumns();
			this.areas = CreateAreas();
			this.autoExpandGroups = false;
		}
		protected QueryDataSource(IPivotGridDataSourceOwner owner)
			: this() {
			this.owner = owner;
		}
		protected abstract QueryColumns<TColumn> CreateCubeColumns();
		protected abstract QueryAreas<TColumn> CreateAreas();
		protected bool Contains(PivotGridFieldBase field) {
			return cubeColumns.ContainsKey(field);
		}
		protected bool Contains(string fieldName) {
			return cubeColumns.ContainsKey(fieldName);
		}
		#region IDisposable Members
		bool disposed;
		protected bool Disposed { get { return disposed; } }
		~QueryDataSource() {
			if(!Disposed) {
				Dispose(false);
				disposed = true;
			}
		}
		public void Dispose() {
			if(!Disposed) {
				Dispose(true);
				GC.SuppressFinalize(this);
				disposed = true;
			}
		}
		protected virtual void Dispose(bool disposing) {
		}
		#endregion
		#region properties
		protected abstract PivotDataSourceCaps Capabilities {
			get;
		}
		public virtual bool AutoExpandGroups {
			get { return autoExpandGroups; }
			set {
				if(value != autoExpandGroups) {
					autoExpandGroups = value;
					if(Metadata.Connected)
						ChangeExpandedAll(autoExpandGroups);
				}
			}
		}
		public virtual void SetAutoExpandGroups(bool value, bool reloadData) { }
		protected IPivotGridDataSourceOwner Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				SetOwner(value);
			}
		}
		protected virtual void SetOwner(IPivotGridDataSourceOwner value) {
			owner = value;
			if(value != null)
				Metadata.RegisterOwner(this);
			else
				Metadata.UnregisterOwner(this);
		}
		public virtual bool ShouldCalculateRunningSummary {
			get { return false; }
		}
		internal bool IsDesignMode { get { return Owner != null ? Owner.IsDesignMode : false; } }
		protected internal QueryColumns<TColumn> CubeColumns { get { return cubeColumns; } }
		protected internal QueryAreas<TColumn> Areas { get { return areas; } }
		protected internal List<TColumn> DataArea { get { return Areas.DataArea; } }
		protected internal AreaFieldValues ColumnValues { get { return Areas.ColumnValues; } }
		protected internal AreaFieldValues RowValues { get { return Areas.RowValues; } }
		protected internal UniqueValues<TColumn> UniqueValues {
			get {
				if(uniqueValues == null)
					uniqueValues = CreateUniqueValues();
				return uniqueValues;
			}
		}
		protected IQueryFilterHelper FilterHelper {
			get {
				if(filterHelper == null)
					filterHelper = CreateFilterHelper();
				return filterHelper;
			}
			set {
				if(value != null)
					return;
				filterHelper = value;
			}
		}
		protected abstract QueryMetadata<TColumn> Metadata { get; }
		protected abstract IQueryFilterHelper CreateFilterHelper();
		protected abstract UniqueValues<TColumn> CreateUniqueValues();
		protected internal AreaFieldValues GetFieldValues(bool isColumn) {
			return areas.GetFieldValues(isColumn);
		}
		#endregion
		#region events
		public event EventHandler<PivotDataSourceEventArgs> DataChanged;
		public event EventHandler<PivotDataSourceEventArgs> LayoutChanged;
		protected void RaiseDataChanged() {
			DataChanged.SafeRaise(this, new PivotDataSourceEventArgs(this));
		}
		bool layoutLocked;
		protected void LockLayoutChanged() {
			layoutLocked = true;
		}
		protected void UnLockLayoutChanged() {
			layoutLocked = false;
		}
		protected void RaiseLayoutChanged() {
			if(!layoutLocked)
				LayoutChanged.SafeRaise(this, new PivotDataSourceEventArgs(this));
		}
		#endregion
		#region Expand/collapse support
		bool IPivotGridDataSource.ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			isFullyExpanded = false;
			if(visibleIndex < 0 || visibleIndex >= GetFieldValues(isColumn).Count)
				return false;
			bool isCollapsed = Areas.IsObjectCollapsedCore(isColumn, visibleIndex);
			if(expanded && isCollapsed) {
				return ExpandCore(isColumn, visibleIndex);
			}
			if(!expanded && !isCollapsed) {
				Areas.Collapse(isColumn, visibleIndex);
				RaiseLayoutChanged();
			}
			return true;
		}
		protected virtual bool ChangeExpandedAll(bool expanded) {
			isFullyExpanded = expanded;
			return ChangeExpandedAll(true, expanded) && ChangeExpandedAll(false, expanded);
		}
		bool IPivotGridDataSource.ChangeExpandedAll(bool isColumn, bool expanded) {
			isFullyExpanded = expanded;
			return ChangeExpandedAll(isColumn, expanded);
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			isFullyExpanded = false;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			AreaFieldValues fieldValues = GetFieldValues(isColumn);
			ICollection<GroupInfo> groups = fieldValues.GetAllGroupsByValue(field.AreaIndex, value);
			return ChangeFieldExpandedCore(expanded, isColumn, groups);
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			isFullyExpanded = false;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			AreaFieldValues fieldValues = GetFieldValues(isColumn);
			ICollection<GroupInfo> indexes = fieldValues.GetAllGroupsByLevel(field.AreaIndex);
			return ChangeFieldExpandedCore(expanded, isColumn, indexes);
		}
		bool IPivotGridDataSource.ChangeFieldSortOrder(PivotGridFieldBase field) {
			if(!Contains(field))
				return false;
			DoRefreshCore();
			return true;
		}
		bool IPivotGridDataSource.ChangeFieldSummaryType(PivotGridFieldBase field, PivotSummaryType oldSummaryType) {
			return false;
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return Areas.IsObjectCollapsedCore(isColumn, visibleIndex);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, object[] values) {
			int visibleIndex = GetFieldValues(isColumn).GetIndex(values);
			if(visibleIndex < 0)
				return false;
			return Areas.IsObjectCollapsedCore(isColumn, visibleIndex);
		}
		protected internal virtual bool ExpandCore(bool isColumn, int visibleIndex) {
			GetFieldValues(isColumn).BeginUpdate();
			try {
				return ExpandCore(isColumn, GetFieldValues(isColumn)[visibleIndex]);
			} finally {
				GetFieldValues(isColumn).EndUpdate();
			}
		}
		protected bool ExpandCore(bool isColumn, params GroupInfo[] groups) {
			GroupInfo[] columns = isColumn ? groups : ColumnValues.ToArray(),
							rows = isColumn ? RowValues.ToArray() : groups;
			return QueryData(columns, rows, isColumn, !isColumn);
		}
		protected virtual bool ChangeExpandedAll(bool isColumn, bool expanded) {
			IList<TColumn> area = Areas.GetArea(isColumn);
			AreaFieldValues fieldValues = GetFieldValues(isColumn);
			bool result = true;
			if(expanded) {
				for(int i = 0; i < area.Count - 1; i++)
					if(!ChangeFieldExpandedCore(expanded, isColumn, fieldValues.GetAllGroupsByLevel(i)))
						result = false;
			} else {
				ChangeFieldExpandedCore(expanded, isColumn, fieldValues.GetAllGroupsByLevel(0));
			}
			return result;
		}
		protected internal virtual bool ChangeFieldExpandedCore(bool expanded, bool isColumn, ICollection<GroupInfo> groups) { 
			GroupInfo[] filteredValues = new GroupInfo[groups.Count];
			int count = 0;
			int maxLevel = Areas.GetArea(isColumn).Count - 1;
			AreaFieldValues values = GetFieldValues(isColumn);
			foreach(GroupInfo group in groups) {
				if(expanded == (maxLevel > group.Level && group.GetChildren() == null)) {
					filteredValues[count] = group;
					count++;
				}
			}
			if(count == 0)
				return true;
			Array.Resize(ref filteredValues, count);
			bool result = true;
			if(expanded) {
				GetFieldValues(isColumn).BeginUpdate();
				try {
					result = ExpandGroupParts(isColumn, filteredValues);
				} finally {
					GetFieldValues(isColumn).EndUpdate();
				}
			} else {
				for(int i = count - 1; i >= 0; i--)
					Areas.Collapse(isColumn, filteredValues);
			}
			return result;
		}
		 protected bool ExpandGroupParts(bool isColumn, GroupInfo[] groups) {
			return ExpandGroups(isColumn, groups);
		}
		protected bool ExpandGroupParts(bool isColumn, AreaFieldValues fieldValues, List<int> filteredIndexes) {
			GroupInfo[] groups = new GroupInfo[filteredIndexes.Count];
			for(int i = 0; i < filteredIndexes.Count; i++)
				groups[i] = fieldValues[filteredIndexes[i]];
			return ExpandGroups(isColumn, groups);
		}
		const int maxGroupCount = 100;
		protected virtual bool ExpandGroups(bool isColumn, GroupInfo[] groups) {
			bool result = true;
			for(int i = 0; i < groups.Length; i += maxGroupCount) {
				int groupCount = Math.Min(maxGroupCount, groups.Length - i);
				GroupInfo[] groupsPart = new GroupInfo[groupCount];
				for(int j = 0; j < groupsPart.Length; j++)
					groupsPart[j] = groups[i + j];
				if(!ExpandCore(isColumn, groupsPart))
					result = false;
			}
			return result;
		}
		#endregion
		int IPivotGridDataSource.GetCellCount(bool isColumn) {
			return GetFieldValues(isColumn).Count;
		}
		PivotCellValue IPivotGridDataSource.GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			return Areas.GetCellValue(columnIndex, rowIndex, dataIndex, summaryType);
		}
		object IPivotGridDataSource.GetFieldValue(bool isColumn, int visibleIndex, int areaIndex) {
			AreaFieldValues values = GetFieldValues(isColumn);
			if(visibleIndex < -1 || visibleIndex >= values.Count)
				return null;
			GroupInfo group = values[visibleIndex, areaIndex];
			return group == null ? null : group.Member.Value;
		}
		bool IPivotGridDataSource.GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex) {
			AreaFieldValues values = GetFieldValues(isColumn);
			if(visibleIndex < -1 || visibleIndex >= values.Count)
				return false;
			GroupInfo group = values[visibleIndex, levelIndex];
			return group == null ? false : group.Member.IsOthers;
		}
		int IPivotGridDataSource.GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return GetFieldValues(isColumn).GetNextOrPrevIndex(visibleIndex, isNext);
		}
		int IPivotGridDataSource.GetObjectLevel(bool isColumn, int visibleIndex) {
			AreaFieldValues values = GetFieldValues(isColumn);
			if(visibleIndex < 0 || visibleIndex >= values.Count)
				return -1;
			return values[visibleIndex].Level;
		}
		int IPivotGridDataSource.GetVisibleIndexByValues(bool isColumn, object[] values) {
			return GetFieldValues(isColumn).GetIndex(values);
		}
		List<object> IPivotGridDataSource.GetVisibleFieldValues(PivotGridFieldBase field) {
			TColumn column;
			if(!CubeColumns.TryGetValue(field, out column))
				return new List<object>();
			return QueryVisibleValues(this, column);
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			return GetDrillDownDataSourceCore(columnIndex, rowIndex, dataIndex, maxRowCount, null);
		}
		protected PivotDrillDownDataSource GetDrillDownDataSourceCore(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns) {
			if(columnIndex < -1 || columnIndex >= ColumnValues.Count ||
				rowIndex < -1 || rowIndex >= RowValues.Count ||
				dataIndex < 0 || dataIndex != 0 && dataIndex >= DataArea.Count)
				return null;
			return QueryDrilldown(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
		}
		bool IPivotGridDataSource.GetIsEmptyGroupFilter(PivotGridGroup group) {
			if(group.FilterValues.FilterType == PivotFilterType.Excluded)
				return group.FilterValues.Count == 0;
			else
				return false;   
		}
		bool? IPivotGridDataSource.IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			return UniqueValues.IsGroupFilterValueChecked(group, parentValues, value);
		}
		PivotSummaryInterval IPivotGridDataSource.GetSummaryInterval(PivotGridFieldBase dataField, bool visibleValuesOnly,
				bool customLevel, PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			throw new NotImplementedException("The operation is not supported.");
		}
		List<object> IPivotGridDataSource.GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			if(group == null || group.Count == 0)
				return new List<object>();
			if(parentValues == null || parentValues.Length == 0)
				return new List<object>(UniqueValues.GetSortedUniqueValues(group[0]));
			return UniqueValues.GetSortedUniqueGroupValues(group, parentValues);
		}
		object[] IPivotGridDataSource.GetUniqueFieldValues(PivotGridFieldBase field) {
			return UniqueValues.GetUniqueValues(field);
		}
		object[] IPivotGridDataSource.GetSortedUniqueValues(PivotGridFieldBase field) {
			return UniqueValues.GetSortedUniqueValues(field).ToArray();
		}
		object[] IPivotGridDataSource.GetAvailableFieldValues(PivotGridFieldBase field, bool deferUpdates, ICustomFilterColumnsProvider customFilters) {
			TColumn column;
			if(!CubeColumns.TryGetValue(field, out column))
				return new object[0];
			List<TColumn> columns = new List<TColumn>();
			if(deferUpdates)
				foreach(PivotGridFieldBase field0 in customFilters.Fields) {
					TColumn column0 = CubeColumns[field0];
					if(column0 != null && !column0.IsMeasure)
						columns.Add(column0);
				}
			return QueryAvailableValues(this, column, deferUpdates, columns);
		}
		void IPivotGridDataSource.OnInitialized() {
			Metadata.OnInitialized(this);
			OnInitialized();
		}
		protected virtual void OnInitialized() {
			if(Metadata.Connected && CubeColumns.Count == 0 && Metadata.Connected && CubeColumns.Count == 0)
				RaiseDataChanged();
		}
		void IPivotGridDataSource.DoRefresh() {
			DoRefreshCore();
		}
		void DoRefreshCore() {
			LockLayoutChanged();
			CollapsedState columnState = Areas.SaveCollapsedState(true),
								 rowState = Areas.SaveCollapsedState(false);
			if(cubeColumns.Count == 0 && !IsDesignMode)
				PopulateColumns();
			if(Metadata.Columns.Count == 0 || IsDesignMode) {
				Owner.GetSortedFields();
				UnLockLayoutChanged();
				RaiseLayoutChanged();
				return;
			}
			bool anyAreaFullyCollapsed = rowState.Count > 1 && RowValues.Count > 1 && rowState.IsFullyCollapsed || columnState.Count > 1 && ColumnValues.Count > 1 && columnState.IsFullyCollapsed;
			bool needQueryFullExpand = columnState.IsFullyExpanded && rowState.IsFullyExpanded || AutoExpandGroups && isFullyExpanded && !anyAreaFullyCollapsed;
			PartialUpdaterBase<TColumn> updater = UpdateCubeColumnsState(needQueryFullExpand, rowState, columnState);
			needQueryFullExpand = needQueryFullExpand || (rowState.IsFullyExpanded || areas.GetArea(false).Count < 2) && (columnState.IsFullyExpanded || areas.GetArea(true).Count < 2);
			switch(updater.UpdateType) {
				case DataSourceUpdateType.Full: {
						Areas.ClearCells();
						if(needQueryFullExpand) {
							QueryFullyExpanded();
						} else {
							QueryData(new GroupInfo[0], new GroupInfo[0], false, false);
							Areas.LoadCollapsedState(true, columnState);
							Areas.LoadCollapsedState(false, rowState);
						}
						IList<AggregationLevel> calcs = Owner.GetAggregations();
						if(calcs != null && calcs.Count > 0) {
							CalculateAggregations(calcs);
						}
						break;
					}
				case DataSourceUpdateType.Partial: {
						updater.DoPartialUpdate();
						break;
					}
			}
			if(updater.UpdateType != DataSourceUpdateType.Full) {
				IList<AggregationLevel> calcs = Owner.GetAggregations();
				if(calcs != null && calcs.Any(a => a.Any((b) => b.Any((c) => !c.IsCalculated))))
					CalculateAggregations(calcs);
			}
			UnLockLayoutChanged();
			RaiseLayoutChanged();
		}
		void CalculateAggregations(IList<AggregationLevel> calcs) {
			if(NeedRequestCalculationsFromServer(calcs))
				Metadata.QueryCustomSummary(this, calcs);
			else
				new AggregationLevelsCalculator<GroupInfo, IQueryMetadataColumn>(Areas).Calculate(calcs);
		}
		protected virtual bool NeedRequestCalculationsFromServer(IList<AggregationLevel> calcs) {
			return !(isFullyExpanded && AutoExpandGroups || Areas.RowArea.Count <= 1 && areas.ColumnArea.Count <= 1);
		}
		protected virtual PartialUpdaterBase<TColumn> UpdateCubeColumnsState(bool needExpand, CollapsedState row, CollapsedState column) {
			PivotGridFieldReadOnlyCollection sortedFields = Owner.GetSortedFields();
			bool changed = EnsureFilterHelper();
			changed = FilterHelper.BeforeSpreadAreas(sortedFields) || changed;
			PartialUpdaterBase<TColumn> updater = Areas.SpreadFields(sortedFields, needExpand, row, column);
			changed = FilterHelper.AfterSpreadAreas(sortedFields) || changed;
			changed = changed || Areas.ColumnArea.Count != 0 && Areas.ColumnValues.Count == 0 || Areas.RowArea.Count != 0 && Areas.RowValues.Count == 0 || Areas.DataArea.Count != 0 && Areas.Cells.Count == 0;
			if(changed)
				updater.ForceUpdate();
			return updater;
		}
		void ReloadData() {
			CubeColumns.Clear();
			Metadata.Columns.Clear();
			PopulateColumns();
			Areas.Clear();
			RaiseDataChanged();
		}
		protected virtual void QueryFullyExpanded() {
			QueryData(new GroupInfo[0], new GroupInfo[0], false, false);
			if(AutoExpandGroups)
				ChangeExpandedAll(true);
		}
		void IPivotGridDataSource.ReloadData() {
			ReloadData();
		}
		void IPivotGridDataSource.HideDataField(PivotGridFieldBase field, int dataIndex) {
			DoRefreshCore();
		}
		void IPivotGridDataSource.MoveDataField(PivotGridFieldBase field, int oldIndex, int newIndex) {
			DoRefreshCore();
		}
		void IPivotGridDataSource.RetrieveFields(PivotArea area, bool visible) {
			if(cubeColumns.Count == 0)
				PopulateColumns();
			foreach(KeyValuePair<string, MetadataColumnBase> column in Metadata.Columns) {
				Owner.CreateField(column.Value.IsMeasure ? PivotArea.DataArea : area,
					column.Key, null, column.Value.DisplayFolder, visible);
			}
		}
		string IPivotGridDataSource.GetLocalizedFieldCaption(string fieldName) {
			return GetColumnCaption(fieldName);
		}
		string IPivotGridDataSource.GetFieldCaption(string fieldName) {
			return Metadata.GetFieldCaption(fieldName);
		}
		protected abstract void PopulateColumns();
		string[] IPivotGridDataSource.GetFieldList() {
			if(cubeColumns.Count == 0)
				PopulateColumns();
			return Metadata.Columns.GetFieldList().ToArray();
		}
		Type IPivotGridDataSource.GetFieldType(PivotGridFieldBase field, bool raw) {
			return GetFieldTypeCore(field);
		}
		protected virtual Type GetFieldTypeCore(PivotGridFieldBase field) {
			string name = CubeColumns.GetFieldCubeColumnsName(field);
			TColumn column;
			if(CubeColumns.TryGetValue(name, out column))
				return column.Metadata.SafeDataType;
			MetadataColumnBase metaColumn;
			if(Metadata.Columns.TryGetValue(name, out metaColumn))
				return metaColumn.SafeDataType;
			return typeof(object);
		}
		bool IPivotGridDataSource.HasNullValues(PivotGridFieldBase field) {
			TColumn column;
			if(CubeColumns.TryGetValue(field, out column))
				return Metadata.HasNullValues(this, column.Metadata);
			else {
				MetadataColumnBase metaColumn = Metadata.Columns[field.FieldName];
				if(metaColumn != null)
					return Metadata.HasNullValues(this, metaColumn);
				else
					return false;
			}
		}
		bool IPivotGridDataSource.HasNullValues(string dataMember) {
			MetadataColumnBase metaColumn = Metadata.Columns[dataMember];
			if(metaColumn != null)
				return Metadata.HasNullValues(this, metaColumn);
			else
				return false;
		}
		bool IPivotGridDataSource.IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			return IsAreaAllowedCore(field, area);
		}
		protected virtual bool IsAreaAllowedCore(PivotGridFieldBase field, PivotArea area) {
			return true;
		}
		bool IPivotGridDataSource.IsFieldReadOnly(PivotGridFieldBase field) { return IsFieldReadonlyCore(); }
		protected virtual bool IsFieldReadonlyCore() {
			return true;
		}
		bool IPivotGridDataSource.IsFieldTypeCheckRequired(PivotGridFieldBase field) {
			return IsFieldTypeCheckRequiredCore(field);
		}
		protected virtual bool IsFieldTypeCheckRequiredCore(PivotGridFieldBase field) {
			return false;
		}
		bool IPivotGridDataSource.IsUnboundExpressionValid(PivotGridFieldBase field) {
			return IsUnboundExpressionValidCore(field);
		}
		protected abstract bool IsUnboundExpressionValidCore(PivotGridFieldBase field);
		void IQueryDataSource.LoadCollapsedState(bool isColumn, CollapsedState state) {
			Areas.LoadCollapsedState(isColumn, state);
		}
		void IPivotGridDataSource.LoadCollapsedStateFromStream(Stream stream) {
			Areas.LoadCollapsedStateFromStream(stream, AutoExpandGroups);
		}
		void IPivotGridDataSource.SaveCollapsedStateToStream(Stream stream) {
			Areas.SaveCollapsedStateToStream(stream);
		}
		void IPivotGridDataSource.SaveDataToStream(Stream stream, bool compressed) {
			throw new Exception("The operation is not supported.");
		}
		void IPivotGridDataSource.WebLoadCollapsedStateFromStream(Stream stream) {
			Areas.LoadCollapsedStateFromStream(stream, AutoExpandGroups);
		}
		void IPivotGridDataSource.WebSaveCollapsedStateToStream(Stream stream) {
			Areas.SaveCollapsedStateToStream(stream);
		}
		PivotDataSourceCaps IPivotGridDataSource.Capabilities {
			get { return Capabilities; }
		}
		ICustomObjectConverter IPivotGridDataSource.CustomObjectConverter { get { return GetCustomObjectConverterCore(); } set { ; } }
		protected virtual ICustomObjectConverter GetCustomObjectConverterCore() {
			return null;
		}
		IPivotGridDataSourceOwner IPivotGridDataSource.Owner { get { return Owner; } set { Owner = value; } }
		#region IDataSourceDataSourceHelpersOwner Members
		bool IDataSourceHelpersOwner<TColumn>.HandleException(QueryHandleableException ex) { return this.RaiseQueryException(ex); }
		QueryColumns<TColumn> IDataSourceHelpersOwner<TColumn>.CubeColumns { get { return cubeColumns; } }
		Dictionary<TColumn, PivotGridFieldBase> IDataSourceHelpersOwner<TColumn>.FieldsByColumns { get { return Areas.FieldsByColumns; } }
		void IDataSourceHelpersOwner<TColumn>.ChangeFieldExpanded(bool expanded, bool isColumn, ICollection<GroupInfo> groups) {
			ChangeFieldExpandedCore(expanded, isColumn, groups);
		}
		IQueryFilterHelper IDataSourceHelpersOwner<TColumn>.FilterHelper { get { return FilterHelper; } }
		bool IDataSourceHelpersOwner<TColumn>.Connected {
			get { return Metadata.Connected; }
		}
		TColumn IDataSourceHelpersOwner<TColumn>.CreateColumn(IQueryMetadataColumn column, PivotGridFieldBase field) {
			return CreateColumnCore(column, field);
		}
		protected abstract TColumn CreateColumnCore(IQueryMetadataColumn column, PivotGridFieldBase field);
		bool IDataSourceHelpersOwner<TColumn>.IsDesignMode { get { return IsDesignMode; } }
		IQueryMetadata IDataSourceHelpersOwner<TColumn>.Metadata { get { return Metadata; } }
		#endregion
		#region queries
		internal virtual object[] QueryAvailableValues(IDataSourceHelpersOwner<TColumn> owner, TColumn column, bool deferUpdates, List<TColumn> customFilters) {
			if(!Metadata.Connected)
				return new object[0];
			return Metadata.QueryAvailableValues(owner, column, deferUpdates, customFilters);
		}
		internal virtual List<object> QueryVisibleValues(IDataSourceHelpersOwner<TColumn> owner, TColumn column) {
			if(!Metadata.Connected)
				return new List<object>();
			return Metadata.QueryVisibleValues(owner, column);
		}
		internal virtual PivotDrillDownDataSource QueryDrilldown(int columnIndex, int rowIndex, int dataIndex,
																	int maxRowCount, List<string> customColumns) {
			if(!Metadata.Connected)
				return null;
			QueryMember[] columnMembers = columnIndex >= 0 ? ColumnValues.GetHierarchyMembers(columnIndex) : new QueryMember[0],
				rowMembers = rowIndex >= 0 ? RowValues.GetHierarchyMembers(rowIndex) : new QueryMember[0];
			return new QueryDrillDownDataSource(Metadata.QueryDrillDown(this, columnMembers, rowMembers, Areas.DataArea.Count == 0 ? null : Areas.DataArea[dataIndex],
							maxRowCount, customColumns));
		}
		protected internal virtual bool QueryData(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand) {
			if(!Metadata.Connected)
				return false;
			IQueryContext<TColumn> context = CreateQueryContext(columns, rows, columnExpand, rowExpand);
			CellSet<TColumn> cellSet = Metadata.QueryData(this, context);
			if(cellSet == null)
				return false;
			if(cellSet == null) {
				if(rowExpand || columnExpand) {
					return false;
				} else {
					Areas.Cells.Clear();
					return true;
				}
			}
			context.PreParseResult(cellSet);
			if(cellSet.IsDataEmpty)
				return false;
			else
				Areas.Cells.ReadData(cellSet); 
			context.PerformSorting();
			RaiseLayoutChanged();
			return true;
		}
		protected abstract IQueryContext<TColumn> CreateQueryContext(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand);
		#endregion
		object ICloneable.Clone() {
			QueryDataSource<TColumn> clone = CreateInstance();
			if(clone != null) {
				Metadata.RegisterOwner(clone);
			}
			return clone;
		}
		protected abstract QueryDataSource<TColumn> CreateInstance();
		protected internal CriteriaOperator PatchCriteria(CriteriaOperator criteria) {
			Dictionary<string, string> all = new Dictionary<string, string>();
			ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
			criteria.Accept(visitor);
			foreach(string name in visitor.ColumnNames)
				if(CubeColumns.ContainsKey(name)) {
					all.Add(name, name);
				} else {
					PivotGridFieldBase field = ((PivotGridData)Owner).Fields.GetFieldByName(name);
					MetadataColumnBase column = null;
					if(field != null && Metadata.Columns.TryGetValue(CubeColumns.GetFieldCubeColumnsName(field), out column))
						all.Add(name, MetadataColumnBase.GetOriginalColumn(Metadata.Columns, column).UniqueName);
					else
						all.Add(name, field == null ? name : CubeColumns.GetFieldCubeColumnsName(field));
				}
			return new QueryCriteriaOperatorVisitor(all).Process(criteria);
		}
		CriteriaOperator IDataSourceHelpersOwner<TColumn>.PatchCriteria(CriteriaOperator criteria) {
			return PatchCriteria(criteria);
		}
		CancellationToken IDataSourceHelpersOwner<TColumn>.CancellationToken { get { return owner == null ? CancellationToken.None : owner.CancellationToken; } }
		Func<IQueryMemberProvider, IQueryMemberProvider, int?> IDataSourceHelpersOwner<TColumn>.GetCustomFieldSort(ICustomSortHelper helper, TColumn column) {
			PivotGridFieldBase field;
			if(areas.FieldsByColumns.TryGetValue(column, out field))
				return (x, y) => owner.GetCustomFieldSort(x, y, field, helper);
			else
				return (x, y) => null;
		}
		Func<object, string> IDataSourceHelpersOwner<TColumn>.GetCustomFieldText(TColumn column) {
			PivotGridFieldBase field;
			if(areas.FieldsByColumns.TryGetValue(column, out field))
				return (val) => owner.GetCustomFieldText(field, val);
			else
				return val => null;
		}
		int IComparable.CompareTo(object value) { 
			QueryDataSource<TColumn> dataSource = value as QueryDataSource<TColumn>;
			return dataSource == null ? -1 : object.ReferenceEquals(Metadata, dataSource.Metadata) ? 0 : -1;
		}
		protected virtual bool EnsureFilterHelper() {
			return false;
		}
		#region IPartialUpdaterOwner
		QueryColumns<TColumn> IPartialUpdaterOwner<TColumn>.CubeColumns { 
			get { return cubeColumns; }
		}
		QueryAreas<TColumn> IPartialUpdaterOwner<TColumn>.Areas {
			get { return areas; }
		}
		void IPartialUpdaterOwner<TColumn>.ExpandAll() {
			ChangeExpandedAll(true);
		}
		void IPartialUpdaterOwner<TColumn>.QueryData(GroupInfo[] columnGroups, GroupInfo[] rowGroups, bool columnExpand, bool rowExpand) {
			QueryData(columnGroups, rowGroups, columnExpand, rowExpand);
		}
		#endregion
		string IQueryDataSource.SaveColumns() {
			return this.SaveColumns();
		}
		protected internal virtual string SaveColumns() {
			return Metadata.SaveColumns();
		}
		void IQueryDataSource.RestoreColumns(string savedColumns) {
			if(Metadata.Columns.Count > 0)
				return;
			Metadata.RestoreColumns(savedColumns);
			UpdateCubeColumnsState(false, null, null);
		}
		string IQueryDataSource.SaveFieldValuesAndCellsToString() {
			if(Metadata.Columns.Count == 0)
				return string.Empty;
			return Areas.SaveFieldValuesAndCellsToString();
		}
		void IQueryDataSource.RestoreFieldValuesAndCellsFromString(string stateString) {
			if(string.IsNullOrEmpty(stateString))
				return;
			if(Metadata.Columns.Count == 0)
				throw new Exception("MetadataColumns.Count == 0");
			Areas.RestoreFieldValuesAndCellsFromString(stateString);
			isFullyExpanded = RowValues.IsFullyExpanded() && ColumnValues.IsFullyExpanded();
		}
		event PivotQueryDataSourceExceptionEventHandler IQueryDataSource.QueryException {
			add { queryException += value; }
			remove { queryException -= value; }
		}
		int IQueryDataSource.GetLevelCount(bool isColumn) {
			return Areas.GetArea(isColumn).Count;
		}
		PivotDrillDownDataSource IQueryDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns) {
			return GetDrillDownDataSourceCore(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
		}
		string IQueryDataSource.GetColumnDisplayFolder(string columnName) {
			return Metadata.GetColumnDisplayFolder(columnName);
		}
		string GetColumnCaption(string columnName) {
			return Metadata.GetColumnCaption(columnName);
		}
		string IQueryDataSource.GetColumnCaption(string columnName) {
			return GetColumnCaption(columnName);
		}
		protected virtual bool RaiseQueryException(QueryHandleableException ex) {
			if(queryException != null)
				return queryException(this, ex);
			return false;
		}
	}
}
