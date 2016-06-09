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
using System.IO;
using System.IO.Compression;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Utils;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Localization;
using PivotCellValue = DevExpress.XtraPivotGrid.Data.PivotCellValue;
using PivotGridFieldReadOnlyCollection = DevExpress.XtraPivotGrid.Data.PivotGridFieldReadOnlyCollection;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryAreas<TColumn> : ICellTableOwner<TColumn>, ISortContext<TColumn, GroupInfo, IQueryMetadataColumn> where TColumn : QueryColumn {
		readonly List<TColumn> columnArea, rowArea, dataArea, serverSideDataArea, additionalClientCalculatedFields, filterArea;
		readonly Dictionary<TColumn, PivotGridFieldBase> fieldsByColumns;
		readonly CellTable<TColumn> cells;
		readonly AreaFieldValues columnValues, rowValues;
		readonly IDataSourceHelpersOwner<TColumn> owner;
		readonly PivotValueComparerBase valueComparer;
		protected QueryAreas(IDataSourceHelpersOwner<TColumn> owner) {
			this.owner = owner;
			this.columnArea = new List<TColumn>();
			this.rowArea = new List<TColumn>();
			this.dataArea = new List<TColumn>();
			this.serverSideDataArea = new List<TColumn>();
			this.additionalClientCalculatedFields = new List<TColumn>();
			this.filterArea = new List<TColumn>();
			this.fieldsByColumns = new Dictionary<TColumn, PivotGridFieldBase>();
			this.cells = CreateCellTable();
			this.columnValues = new AreaFieldValues();
			this.rowValues = new AreaFieldValues();
			this.valueComparer = new PivotValueComparerBase();
		}
		protected abstract CellTable<TColumn> CreateCellTable();
		protected IDataSourceHelpersOwner<TColumn> Owner { get { return owner; } }
		protected PivotValueComparerBase ValueComparer { get { return valueComparer; } }
		protected IQueryFilterHelper FilterHelper { get { return Owner.FilterHelper; } }
		protected QueryColumns<TColumn> CubeColumns { get { return Owner.CubeColumns; } }
		public List<TColumn> ColumnArea { get { return columnArea; } }
		public List<TColumn> RowArea { get { return rowArea; } }
		public List<TColumn> DataArea { get { return dataArea; } }
		public List<TColumn> ServerSideDataArea { get { return serverSideDataArea; } }
		public List<TColumn> AdditionalClientCalculatedFields { get { return additionalClientCalculatedFields; } }
		public bool ContainsDataColumn(TColumn dataColumn) {
			return dataArea.Contains(dataColumn) || serverSideDataArea.Contains(dataColumn) || additionalClientCalculatedFields.Contains(dataColumn);
		}
		public List<TColumn> FilterArea { get { return filterArea; } }
		public Dictionary<TColumn, PivotGridFieldBase> FieldsByColumns { get { return fieldsByColumns; } }
		public CellTable<TColumn> Cells { get { return cells; } }
		public AreaFieldValues ColumnValues { get { return columnValues; } }
		public AreaFieldValues RowValues { get { return rowValues; } }
		public List<TColumn> GetArea(int areaIndex) {
			return GetArea((PivotArea)areaIndex);
		}
		public List<TColumn> GetArea(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea:
					return ColumnArea;
				case PivotArea.DataArea:
					return DataArea;
				case PivotArea.FilterArea:
					return FilterArea;
				case PivotArea.RowArea:
					return RowArea;
				default:
					return null;
			}
		}
		public IList<TColumn> GetArea(bool isColumn) {
			return isColumn ? ColumnArea : RowArea;
		}
		public AreaFieldValues GetFieldValues(bool isColumn) {
			return isColumn ? ColumnValues : RowValues;
		}
		public void Clear() {
			ClearFields();
			ClearCells();
			FilterHelper.ClearCache();
		}
		void ClearFields() {
			ColumnArea.Clear();
			RowArea.Clear();
			FilterArea.Clear();
			DataArea.Clear();
			ServerSideDataArea.Clear();
			AdditionalClientCalculatedFields.Clear();
			Cells.MeasureMap.Clear();
		}
		public void ClearCells() {
			Cells.Clear();
			ColumnValues.Clear();
			RowValues.Clear();
		}
		public List<TColumn> GetVisibleColumns() {
			List<TColumn> res = new List<TColumn>();
			res.AddRange(DataArea);
   			res.AddRange(ServerSideDataArea);
			res.AddRange(ColumnArea);
			res.AddRange(RowArea);
			res.AddRange(FilterArea);
			return res;
		}
		internal PartialUpdaterBase<TColumn> SpreadFields(PivotGridFieldReadOnlyCollection sortedFields, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column) {
			return CreatePartialUpdater(sortedFields, GetCurrentAreas(), GetCurrentMetadatas(), CreateAreasByFields(sortedFields), (IPartialUpdaterOwner<TColumn>)owner, isDataSourceFullyExpanded, row, column);
		}
		protected abstract PartialUpdaterBase<TColumn> CreatePartialUpdater(PivotGridFieldReadOnlyCollection sortedFields, AreasState<TColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<TColumn> newAreas, IPartialUpdaterOwner<TColumn> owner, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column);
		AreasState<TColumn> GetCurrentAreas() {
			return new AreasState<TColumn>(this);
		}
		List<IQueryMetadataColumn>[] GetCurrentMetadatas() {
			List<IQueryMetadataColumn>[] list = new List<IQueryMetadataColumn>[4];
			for(int i = 0; i < 4; i++)
				list[i] = new List<IQueryMetadataColumn>(DXListExtensions.ConvertAll<TColumn, IQueryMetadataColumn>(GetArea(i), (c) => c.Metadata));
			return list;
		}
		internal void SetColumnsToAreas(AreasState<TColumn> areas) {
			ClearFields();
			for(int i = 0; i < areas.Length; i++)
				for(int j = 0; j < areas[i].Count; j++)
					GetArea(i).Add(areas[i][j]);
			ServerSideDataArea.AddRange(areas.ServerSideDataArea.Where((c) => c.HasMeasureData));
			AdditionalClientCalculatedFields.AddRange(areas.AdditionalClientCalculatedFields);
			foreach(TColumn column in DataArea)
				Cells.AddUnboundFieldToMap(column.UniqueName, column.Metadata);
			foreach(TColumn column in ServerSideDataArea)
				Cells.AddUnboundFieldToMap(column.UniqueName, column.Metadata);
		}
		AreasState<TColumn> CreateAreasByFields(PivotGridFieldReadOnlyCollection sortedFields) {
			List<TColumn>[] areas = new List<TColumn>[Helpers.GetEnumValues(typeof(PivotArea)).Length];
			for(int i = 0; i < areas.Length; i++)
				areas[i] = new List<TColumn>();
			FieldsByColumns.Clear();
			QueryColumns<TColumn> cubeColumns = Owner.CubeColumns;
			List<TColumn> columnRowFilterFields = new List<TColumn>();
			for(int i = 0; i < sortedFields.Count; i++) {
				PivotGridFieldBase field = sortedFields[i];
				TColumn column = GetColumnByField(field);
				bool isError = false;
				if(column == null) {
					if(NeedAddErrorColumnIfInvalid(field) && field.Visible) {
						KeyValuePair<TColumn, PivotGridFieldBase> error = GetErrorColumn(field.Area);
						field = error.Value;
						column = error.Key;
						isError = true;
					} else {
						continue;
					}
				}
				if(!field.IsOLAPSummaryVariation && !(Owner.Owner.OptionsOLAP.UsePrefilter && field.Area == PivotArea.FilterArea && field.UnboundType == Data.UnboundColumnType.Bound) &&
					(column.IsMeasure == (field.Area == PivotArea.DataArea)) && !(FieldsByColumns.ContainsKey(column) && column.IsMeasure && (AllowDuplicatedMeasures() || isError)))
					FieldsByColumns.Add(column, field);
				if(!field.Visible && field.Area != PivotArea.FilterArea)
					continue;
				areas[(int)field.Area].Add(column);
				if(field.Area != PivotArea.DataArea && !column.IsMeasure && (field.Area != PivotArea.FilterArea ||
						!PivotGridFieldBase.IsFilterEmptyFast(field)))
					columnRowFilterFields.Add(column);
			}
			DoAdditionalSpread(columnRowFilterFields, areas);
			return new AreasState<TColumn>(areas);
		}
		protected abstract bool NeedAddErrorColumnIfInvalid(PivotGridFieldBase field);
		KeyValuePair<TColumn, PivotGridFieldBase>? errorMeasure;
		KeyValuePair<TColumn, PivotGridFieldBase> GetErrorColumn(PivotArea area) {
			TColumn errorColumn;
			PivotGridFieldBase errorField;
			if(area == PivotArea.DataArea && !ReferenceEquals(errorMeasure, null))
				return errorMeasure.Value;
			errorField = new PivotGridFieldBase();
			errorField.Area = area;
			errorField.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			errorField.UnboundExpression = new OperandValue(PivotGridLocalizer.GetString(PivotGridStringId.ValueError)).ToString();
			errorColumn = CreateErrorColumn(errorField);
			KeyValuePair<TColumn, PivotGridFieldBase> res = new KeyValuePair<TColumn, PivotGridFieldBase>(errorColumn, errorField);
			if(area == PivotArea.DataArea)
				errorMeasure = res;
			return res;
		}
		protected abstract bool AllowDuplicatedMeasures();
		protected abstract TColumn GetColumnByField(PivotGridFieldBase field);
		protected abstract TColumn CreateErrorColumn(PivotGridFieldBase field);
		protected virtual void DoAdditionalSpread(List<TColumn> columnRowFilterFields, List<TColumn>[] areas) { }
		public PivotCellValue GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			if(columnIndex < -1 || columnIndex >= ColumnValues.Count ||
				rowIndex < -1 || rowIndex >= RowValues.Count ||
				dataIndex < 0 || dataIndex >= DataArea.Count)
				return null;
			TColumn dataField = DataArea[dataIndex];
			if(summaryType == PivotSummaryType.Custom && !(dataField.Metadata is IUnboundMetadataColumn))
				throw new Exception("The PivotGrid doesn't support custom summaries in Server mode.");
			if(!GetActualProcessModeOnQuery(summaryType, dataField, ColumnValues[columnIndex], RowValues[rowIndex]))
				return new PivotCellValue(new SummaryCalculator().CalculateSummary(EnumerateChildValues(columnIndex, rowIndex, dataIndex), summaryType));
			else
				return GetCellValueCore(ColumnValues[columnIndex], RowValues[rowIndex], dataField);
		}
		protected virtual bool GetActualProcessModeOnQuery(PivotSummaryType summaryType, TColumn dataField, GroupInfo column, GroupInfo row) {
			return summaryType == dataField.SummaryType; 
		}
		protected virtual PivotCellValue GetCellValueCore(GroupInfo column, GroupInfo row, TColumn data) {
			return Cells.GetPivotCellValue(column, row, data.Metadata);
		}
		IEnumerable<object> EnumerateChildValues(int columnIndex, int rowIndex, int dataIndex) {
			IQueryMetadataColumn data = DataArea[dataIndex].Metadata;
			foreach(GroupInfo row in RowValues.EnumerateChildren(rowIndex))
				foreach(GroupInfo column in columnValues.EnumerateChildren(columnIndex)) {
					yield return Cells.GetValue(column, row, data);
				}
		}
		public void Collapse(bool isColumn, ICollection<GroupInfo> groups) {
			List<GroupInfo> removedChildren = GetFieldValues(isColumn).RemoveChildren(groups);
			if(isColumn)
				Cells.RemoveItems(removedChildren);
			else
				Cells.RemoveRows(removedChildren);
		}
		public void Collapse(bool isColumn, int visibleIndex) {
			List<GroupInfo> removedChildren = GetFieldValues(isColumn).RemoveChildren(visibleIndex);
			if(isColumn)
				Cells.RemoveItems(removedChildren);
			else
				Cells.RemoveRows(removedChildren);
		}
		public List<QueryMember> GetSortBySummaryMembers(PivotGridFieldBase field) {
			List<QueryMember> res = new List<QueryMember>();
			if(field.SortBySummaryInfo.Conditions.Count > 0) {
				IList<TColumn> conditionArea = GetArea(!field.IsColumn);
				QueryMember[] conditionMembers = new QueryMember[conditionArea.Count];
				List<PivotGridFieldSortCondition> sortedConditions = field.SortBySummaryInfo.Conditions.GetSortedConditions();
				for(int i = 0; i < sortedConditions.Count; i++) {
					PivotGridFieldSortCondition condition = sortedConditions[i];
					if(condition.Field == null)
						continue;
					TColumn conditionColumn = (TColumn)CubeColumns[condition.Field];
					if(!condition.Field.IsColumnOrRow || condition.Field.Area == field.Area || conditionColumn == null)
						continue;
					QueryMember conditionMember = GetMemberByCondition(conditionColumn.Metadata, condition);
					int conditionMemberIndex = conditionArea.IndexOf(conditionColumn);
					if(conditionColumn != null && !conditionColumn.IsMeasure && conditionMember != null && conditionMemberIndex >= 0) {
						conditionMembers[conditionMemberIndex] = conditionMember;
					}
				}
				for(int i = 0; i < conditionMembers.Length; i++) {
					if(conditionMembers[i] != null)
						res.Add(conditionMembers[i]);
				}	
			}
			return res;
		}
		protected abstract QueryMember GetMemberByCondition(IQueryMetadataColumn conditionColumn, PivotGridFieldSortCondition condition);
		protected internal abstract QueryMember GetMemberByValue(IQueryMetadataColumn conditionColumn, object value);
		CollapsedStateManager<TColumn> collapsedStateKeeper;
		protected CollapsedStateManager<TColumn> CollapsedStateKeeper {
			get {
				if(collapsedStateKeeper == null)
					collapsedStateKeeper = CreateCollapsedStateManager();
				return collapsedStateKeeper;
			}
		}
		protected virtual CollapsedStateManager<TColumn> CreateCollapsedStateManager() {
			return new CollapsedStateManager<TColumn>(this, owner);
		}
		public CollapsedState SaveCollapsedState(bool isColumn) {
			return CollapsedStateKeeper.SaveCollapsedState(isColumn);
		}
		public void LoadCollapsedState(bool isColumn, CollapsedState state) {
			CollapsedStateKeeper.LoadCollapsedState(isColumn, state);
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			CollapsedStateKeeper.SaveCollapsedStateToStream(stream);
		}
		public void LoadCollapsedStateFromStream(Stream stream, bool autoExpandGroups) {
			CollapsedStateKeeper.LoadCollapsedStateFromStream(stream, autoExpandGroups);
		}
		public string SaveFieldValuesAndCellsToString() {
			return PivotGridSerializeHelper.ToBase64StringDeflateBuffered(SaveFieldValuesAndCellsToStringCore);
		}
		void SaveFieldValuesAndCellsToStringCore(TypedBinaryWriter writer) {
			List<TColumn> visibleColumns = GetVisibleColumns();
			Dictionary<QueryMember, int> rowMembersIndexes = new Dictionary<QueryMember, int>();
			Dictionary<QueryMember, int> columnMembersIndexes = new Dictionary<QueryMember, int>();
			Dictionary<IQueryMetadataColumn, int> columnIndexes = new Dictionary<IQueryMetadataColumn, int>(CubeColumns.Count);
			writer.Write(visibleColumns.Count);
			for(int i = 0; i < visibleColumns.Count; i++) {
				TColumn column = visibleColumns[i];
				columnIndexes[column.Metadata] = i;
				IQueryAliasColumn queryAliasColumn = column.Metadata as IQueryAliasColumn;
				if(queryAliasColumn != null) {
					columnIndexes[CubeColumns[queryAliasColumn.OriginalColumn].Metadata] = i;
				}
			}
			int count = 0;
			for(int i = 0; i < RowArea.Count; i++) {
				foreach(QueryMember member in GetColumnMembersForSavingToStream(writer, RowArea[i]))
					rowMembersIndexes.Add(member, count++);
			}
			count = 0;
			for(int i = 0; i < ColumnArea.Count; i++) {
				foreach(QueryMember member in GetColumnMembersForSavingToStream(writer, ColumnArea[i]))
					columnMembersIndexes.Add(member, count++);
			}
			RowValues.SaveToStream(writer, columnIndexes, rowMembersIndexes);
			ColumnValues.SaveToStream(writer, columnIndexes, columnMembersIndexes);
			Cells.SaveToStream(writer, ColumnValues.GetGroupIndexesList(), RowValues.GetGroupIndexesDictionary(), columnIndexes);
		}
		protected abstract IList<QueryMember> GetColumnMembersForSavingToStream(TypedBinaryWriter writer, TColumn column);
		protected abstract IList<QueryMember> GetColumnMembersForRestoringFromStream(TypedBinaryReader reader, TColumn column);
		public void RestoreFieldValuesAndCellsFromString(string stateString) {
			List<TColumn> visibleColumns = GetVisibleColumns();
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(stateString))) {
				using(DeflateStream decompressor = new DeflateStream(stream, CompressionMode.Decompress)) {
					using(TypedBinaryReader reader = new TypedBinaryReader(decompressor)) {
						int columnsCount = reader.ReadInt32();
						List<IQueryMetadataColumn> columnIndexes = new List<IQueryMetadataColumn>(columnsCount);
						List<QueryMember> rowMembersIndexes = new List<QueryMember>();
						List<QueryMember> columnMembersIndexes = new List<QueryMember>();
						for(int i = 0; i < columnsCount; i++) {
							IQueryMetadataColumn column = visibleColumns[i].Metadata;
							IQueryAliasColumn queryAliasColumn = column as IQueryAliasColumn;
							if(queryAliasColumn != null)
								columnIndexes.Add(CubeColumns.Owner.Metadata.Columns[queryAliasColumn.OriginalColumn]);
							else
								columnIndexes.Add(column);
						}
						for(int i = 0; i < RowArea.Count; i++)
							rowMembersIndexes.AddRange(GetColumnMembersForRestoringFromStream(reader, RowArea[i]));
						for(int i = 0; i < ColumnArea.Count; i++)
							columnMembersIndexes.AddRange(GetColumnMembersForRestoringFromStream(reader, ColumnArea[i]));
						RowValues.LoadFromStream(reader, columnIndexes, rowMembersIndexes, Math.Max(1, RowArea.Count) + 1);
						ColumnValues.LoadFromStream(reader, columnIndexes, columnMembersIndexes, Math.Max(1, ColumnArea.Count) + 1);
						Cells.LoadFromStream(reader, ColumnValues.GetGroups(), RowValues.GetGroups(), columnIndexes);
					}
				}
			}
		}
		protected virtual void RestoreColumnCore(TypedBinaryReader reader, MetadataColumnBase column) { }
		public List<TColumn> GetUniqueMeasureColumns(params TColumn[] dataColumns) {
			return GetUniqueMeasureColumns((IEnumerable<TColumn>)dataColumns);
		}
		public List<TColumn> GetUniqueMeasureColumns(IEnumerable<TColumn> dataColumns) {
			List<TColumn> cols = new List<TColumn>();
			foreach(TColumn col in dataColumns)
				AddExpressionColumns(cols, col);
			return cols;
		}
		void AddExpressionColumns(List<TColumn> list, TColumn column) {
			if(column == null)
				return;
			IUnboundSummaryLevelMetadataColumn unboundColumn = column.Metadata as IUnboundSummaryLevelMetadataColumn;
			if(unboundColumn != null) {
				if(object.ReferenceEquals(unboundColumn.Criteria, null))
					return;
				ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
				unboundColumn.Criteria.Accept(visitor);
				foreach(string name in visitor.ColumnNames) {
					TColumn cubeColumn = null;
					if(Owner.Metadata.Columns.ContainsKey(name) && Owner.CubeColumns.TryGetValue(name, out cubeColumn) && !list.Contains(cubeColumn))
						list.Add(cubeColumn);
					else
						AddExpressionColumns(list, Owner.CubeColumns[name]);
				}
			} else {
				TColumn cubeColumn = MetadataColumnBase.GetOriginalColumn(Owner.CubeColumns, column);
				if(!list.Contains(cubeColumn))
					list.Add(cubeColumn);
			}
		}
		List<TColumn> ICellTableOwner<TColumn>.GetUniqueMeasureColumns() {
			return ServerSideDataArea;
		}
		public bool IsObjectCollapsedCore(bool isColumn, int visibleIndex) {
			return IsObjectCollapsedCore(visibleIndex, GetFieldValues(isColumn), GetArea(isColumn).Count - 1);
		}
		public bool IsObjectCollapsedCore(int visibleIndex, AreaFieldValues values, int maxLevel) {
			int currLevel = values[visibleIndex].Level;
			return currLevel < maxLevel && (visibleIndex == values.Count - 1 || values[visibleIndex + 1].Level <= currLevel);
		}
		ICalculationSource<GroupInfo, IQueryMetadataColumn> ICalculationContext<GroupInfo, IQueryMetadataColumn>.GetValueProvider() {
			return cells;
		}
		GroupInfo ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.GetSortByObject(List<QueryMember> members, bool isColumn) {
			return this.GetFieldValues(isColumn).GetClosestGroupInfo(members);
		}
		IQueryMetadataColumn ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.GetData(TColumn column) {
			return ContainsDataColumn(column) ? column.Metadata : null;
		}
		bool  ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.IsValidData(IQueryMetadataColumn data) {
			return data != null;
		}
		List<TColumn> ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.GetDataArea() {
			return DataArea;
		}
		Func<IQueryMemberProvider, IQueryMemberProvider, int?> ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.GetCustomFieldSort(TColumn column) {
			return owner.GetCustomFieldSort(new CustomSortHelper<TColumn, GroupInfo, IQueryMetadataColumn>(this, this, owner), column);
		}
		Func<object, string> ISortContext<TColumn, GroupInfo, IQueryMetadataColumn>.GetCustomFieldText(TColumn column) {
			return owner.GetCustomFieldText(column);
		}
		IQueryMetadataColumn ICalculationContext<GroupInfo, IQueryMetadataColumn>.GetData(int index) {
			return dataArea.Count <= index || index < 0 ? null : dataArea[index].Metadata;
		}
		IEnumerable<GroupInfo> ICalculationContext<GroupInfo, IQueryMetadataColumn>.EnumerateFullLevel(bool isColumn, int level) {
			return GetFieldValues(isColumn).EnumerateLevel(level);
		}
		object ICalculationContext<GroupInfo, IQueryMetadataColumn>.GetValue(GroupInfo groupInfo) {
			return groupInfo == null ? null : groupInfo.Member.Value;
		}
		object ICalculationContext<GroupInfo, IQueryMetadataColumn>.GetDisplayValue(GroupInfo groupInfo) {
			return GetDisplayValueCore(groupInfo);
		}
		protected virtual object GetDisplayValueCore(GroupInfo groupInfo) {
			return groupInfo == null ? null : groupInfo.Member.Value;
		}
	}
}
