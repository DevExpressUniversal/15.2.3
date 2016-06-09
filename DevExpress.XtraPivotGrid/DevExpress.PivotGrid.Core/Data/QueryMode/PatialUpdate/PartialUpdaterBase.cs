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
using System.Linq;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.QueryMode {
	public class AreasState<TColumn> where TColumn : QueryColumn {
		List<TColumn>[] stateCore;
		List<TColumn> serverSideDataArea;
		public int Length { get { return 4; } }
		public List<TColumn> this[int index] { get { return stateCore[index]; } }
		public List<TColumn> this[PivotArea area] { get { return this[(int)area]; } }
		public List<TColumn> ServerSideDataArea {
			get {
				if(serverSideDataArea == null)
					throw new InvalidOperationException();
				return serverSideDataArea;
			}
			set {
				serverSideDataArea = value;
			}
		}
		public List<TColumn> AdditionalClientCalculatedFields { get; set; }
 		public IEnumerable<TColumn> ColumnRowArea { get { return this[PivotArea.RowArea].Concat(this[PivotArea.ColumnArea]); }  }
		public AreasState(QueryAreas<TColumn> areas) {
			stateCore = new List<TColumn>[4];
			for(int i = 0; i < 4; i++)
				stateCore[i] = new List<TColumn>(areas.GetArea(i));
			serverSideDataArea = new List<TColumn>(areas.ServerSideDataArea);
		}
		public AreasState(List<TColumn>[] areas) {
			this.stateCore = areas;
		}
	}
	public abstract class PartialUpdaterBase<TColumn> where TColumn : QueryColumn {
		AreasState<TColumn> oldAreas;
		AreasState<TColumn> newAreas;
		bool needFullUpdate;
		List<UpdaterBase<TColumn>> updates = new List<UpdaterBase<TColumn>>();
		IPartialUpdaterOwner<TColumn> owner;
		bool isDataSourceFullyExpanded;
		bool removeEmptyData;
		QueryAreas<TColumn> areas;
		CollapsedState row, column;
		PivotGridFieldReadOnlyCollection sortedFields;
		Dictionary<TColumn, IQueryMetadataColumn> oldMetas = new Dictionary<TColumn,IQueryMetadataColumn>();
		protected IPartialUpdaterOwner<TColumn> Owner { get { return owner; } }
		protected QueryAreas<TColumn> Areas { get { return areas; } }
		public DataSourceUpdateType UpdateType {
			get {
				if(needFullUpdate)
					return DataSourceUpdateType.Full;
				if(updates.Count > 0)
					return DataSourceUpdateType.Partial;
				return DataSourceUpdateType.None;
			}
		}
		protected PartialUpdaterBase(PivotGridFieldReadOnlyCollection sortedFields, AreasState<TColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<TColumn> newAreas, IPartialUpdaterOwner<TColumn> owner, bool isDataSourceFullyExpanded, CollapsedState row, CollapsedState column)
			: this(sortedFields, oldAreas, oldMetas, newAreas, owner, isDataSourceFullyExpanded, false, row, column) {
		}
		public PartialUpdaterBase(PivotGridFieldReadOnlyCollection sortedFields, AreasState<TColumn> oldAreas, List<IQueryMetadataColumn>[] oldMetas, AreasState<TColumn> newAreas, IPartialUpdaterOwner<TColumn> owner, bool isDataSourceFullyExpanded, bool removeEmptyData, CollapsedState row, CollapsedState column) {
			this.sortedFields = sortedFields;
			this.oldAreas = oldAreas;
			this.newAreas = newAreas;
			this.owner = owner;
			this.areas = owner.Areas;
			for(int i = 0; i < oldMetas.Length; i++)
				for(int j = 0; j < oldMetas[i].Count; j++)
					this.oldMetas[oldAreas[i][j]] = oldMetas[i][j]; 
			this.isDataSourceFullyExpanded = isDataSourceFullyExpanded;
			this.removeEmptyData = removeEmptyData;
			this.row = row;
			this.column = column;
			needFullUpdate = CalculateIsNeedUpdate();
		}
		public void ForceUpdate() {
			needFullUpdate = true;
		}
		bool CalculateIsNeedUpdate() {
			List<TColumn> additionalData = new List<TColumn>(GetAdditionalMeasureColumns(newAreas.ColumnRowArea));
			bool needRefresh = UpdateDataAreaCriterias(newAreas[PivotArea.DataArea]) || UpdateDataAreaCriterias(additionalData);			
			newAreas.ServerSideDataArea = areas.GetUniqueMeasureColumns(newAreas[PivotArea.DataArea].Concat(additionalData));
			newAreas.AdditionalClientCalculatedFields = new List<TColumn>(additionalData.Where((c) => {
																								IUnboundMetadataColumn unb = c.Metadata as IUnboundMetadataColumn;
																								return unb != null && !unb.IsServer;
																							}));
			needRefresh = UpdateColumnServerSideDataCriterias() || needRefresh;
			owner.Areas.SetColumnsToAreas(newAreas);
			needRefresh = UpdateSortOrder() || needRefresh;
			needRefresh = SetColumnsAdditionalProperties() || needRefresh;
			if(needRefresh)
				return true;
			if(CalculateDataAreaChange())
				return true;
			if(CalculateFilterAreaChange())
				return true;
			if(CalculateColumnRowAreaNeedUpdate(false))
				return true;
			if(CalculateColumnRowAreaNeedUpdate(true))
				return true;
			if(isDataSourceFullyExpanded)
				updates.Add(new AutoExpandUpdateAction<TColumn>(owner));
			return needRefresh;
		}
		protected virtual IEnumerable<TColumn> GetAdditionalMeasureColumns(IEnumerable<TColumn> rowColumnArea) {
			foreach(TColumn column in rowColumnArea) {
				PivotGridFieldBase field;
				if(areas.FieldsByColumns.TryGetValue(column, out field) && field != null) {
					TColumn measureColumn = GetSortByColumn(field);
					PivotGridFieldBase dataField;
					if(measureColumn != null && areas.FieldsByColumns.TryGetValue(measureColumn, out dataField) && dataField != null &&
							measureColumn.IsMeasure && !dataField.Visible && dataField.Area == PivotArea.DataArea)
						yield return measureColumn;
				}
			}
		}
		bool SetColumnsAdditionalProperties() {
			bool res = false;
			foreach(PivotGridFieldBase field in sortedFields) {
				TColumn column;
				if(owner.CubeColumns.TryGetValue(field, out column)) {
					if(!(field.Visible || field.Area == PivotArea.DataArea && (areas.ServerSideDataArea.Contains(column) || areas.DataArea.Contains(column)))) {
						if(field.Area == PivotArea.FilterArea && field.CanApplyFilter)
							column.Assign(field, false);						
						continue;
					}
					if(!column.Equals(field)) {
						column.Assign(field, false);
						res = true;
					}
					if(field.Area == PivotArea.FilterArea)
						UpdateColumnSortOrder(column, field, false);
					res |= SetColumnAdditionalProperties(field, column);
				}
			}
			return res;
		}
		protected abstract bool SetColumnAdditionalProperties(PivotGridFieldBase field, TColumn column);
		bool UpdateSortOrder() {
			bool result = false;
			foreach(TColumn column in newAreas[(int)PivotArea.ColumnArea])
				if(UpdateColumnSortOrder(column))
					result = true;
			foreach(TColumn column in newAreas[(int)PivotArea.RowArea])
				if(UpdateColumnSortOrder(column))
					result = true;
			return result;
		}
		bool UpdateColumnSortOrder(TColumn column) {
			if(column.IsMeasure)
				return false;
			return UpdateColumnSortOrder(column, areas.FieldsByColumns[column], true);
		}
		public virtual bool UpdateColumnSortOrder(TColumn column, PivotGridFieldBase field, bool update) {
			TColumn sortByColumn = GetSortByColumn(field);
			List<QueryMember> sortBySummaryMembers = areas.GetSortBySummaryMembers(field);
			column.SortBySummaryMembersExpanded = sortByColumn == null || sortBySummaryMembers.Count == 0 || IsValuesExpanded(!field.IsColumn, sortBySummaryMembers);
			if(column.SortMode != field.ActualSortMode || column.SortBySummary != sortByColumn || !ListComparer.ListsEqual(column.SortBySummaryMembers, sortBySummaryMembers)
				|| column.SortMode == PivotSortMode.DimensionAttribute && column.ActualSortProperty != field.SortByAttribute) {
				column.SortBySummary = sortByColumn;
				column.SortMode = field.ActualSortMode;
				column.SortBySummaryMembers.Clear();
				column.SortBySummaryMembers.AddRange(sortBySummaryMembers);
				column.SortOrder = field.SortOrder;
				if(!SortOnClient(field.ActualSortMode, field.TopValueCount > 0, sortByColumn != null, column.SortBySummaryMembersExpanded))
					return true;
				if(!update)
					return false;
				updates.Add(CreateSortModeUpdater(field));
			} else {
				if(field.SortOrder != column.SortOrder) {
					column.SortOrder = field.SortOrder;
					if(!update)
						return false;
					if(field.TopValueCount == 0)
						updates.Add(new SortOrderUpdater<TColumn>(owner, field.IsColumn, field.AreaIndex));
					else {
						if(!column.TopValueHiddenOthersShowedInTotal)
							return true;
						updates.Add(new TopValueSortOrderUpdater<TColumn>(owner, field.IsColumn, field.AreaIndex));
					}
				}
			}
			return false;
		}
		internal virtual SortModeUpdater<TColumn> CreateSortModeUpdater(PivotGridFieldBase field) {
			return new SortModeUpdater<TColumn>(owner, field.IsColumn, field.AreaIndex);
		}
		public TColumn GetSortByColumn(PivotGridFieldBase field) {
			QueryColumns<TColumn> cubeColumns = owner.CubeColumns;
			string name = null;
			if(field.SortBySummaryInfo.Field != null)
				name = cubeColumns.GetFieldCubeColumnsName(field.SortBySummaryInfo.Field);
			if(string.IsNullOrEmpty(name))
				name = field.SortBySummaryInfo.FieldName;
			TColumn column = null;
			if(!string.IsNullOrEmpty(name) && !cubeColumns.TryGetValue(name, out column))
				column = cubeColumns.Values.FirstOrDefault((c) => c.Name == name && c.SummaryType == field.SortBySummaryInfo.SummaryType);
			if(column == null || !column.IsMeasure)
				return null;
			return column;
		}
		bool IsValuesExpanded(bool isColumn, List<QueryMember> sortBySummaryMembers) {
			if(isDataSourceFullyExpanded)
				return true;
			CollapsedState state = isColumn ? column : row;
			return state == null ? false : state.IsExpanded(sortBySummaryMembers);
		}
		protected abstract bool SortOnClient(PivotSortMode sortMode, bool hasTopn, bool hasSortBy, bool hasResolvedSortBySummary);
	   bool UpdateDataAreaCriterias(List<TColumn> dataColumns) {
			bool needUpdate = false;
			foreach(TColumn newColumn in dataColumns) {
				IUnboundMetadataColumn unboundMetadata = newColumn.Metadata as IUnboundMetadataColumn;
				if(unboundMetadata != null)
					if(unboundMetadata.UpdateCriteria(areas.FieldsByColumns[newColumn]))
						if(unboundMetadata.IsServer)
							needUpdate = true;
						else
							updates.Add(new RemoveDataFieldUpdater<TColumn>(owner, newColumn.Metadata));
			}
			return needUpdate;
		}
		bool UpdateColumnServerSideDataCriterias() {
			bool needUpdate = false;
			foreach(TColumn newcolumn in newAreas.ServerSideDataArea) {
				IUnboundMetadataColumn unboundmetadata = newcolumn.Metadata as IUnboundMetadataColumn;
				if(unboundmetadata != null)
					if(unboundmetadata.UpdateCriteria(areas.FieldsByColumns[newcolumn]))
						needUpdate = true;
			}
			if(UpdateCriterias(PivotArea.FilterArea))
				needUpdate = true;
			if(UpdateCriterias(PivotArea.ColumnArea))
				needUpdate = true;
			if(UpdateCriterias(PivotArea.RowArea))
				needUpdate = true;
			return needUpdate;
		}
		bool UpdateCriterias(PivotArea pivotArea) {
			bool needUpdate = false;
			foreach(TColumn column in newAreas[pivotArea]) {
				IUnboundMetadataColumn unbound = column.Metadata as IUnboundMetadataColumn;
				if(unbound != null && unbound.UpdateCriteria(owner.Areas.FieldsByColumns[column]))
					needUpdate = true;
			}
			return needUpdate;
		}
		private bool CalculateFilterAreaChange() {
			int filterAreaIndex = (int)PivotArea.FilterArea;
			List<TColumn> newColumns = newAreas[filterAreaIndex];
			List<TColumn> oldColumns = oldAreas[filterAreaIndex];
			foreach(TColumn oldColumn in oldColumns)
				if(NeedUpdateOnVisibleChange(oldColumn, false))
					return true;
			foreach(TColumn newColumn in newColumns)
				if(NeedUpdateOnVisibleChange(newColumn, true))
					return true;
			return false;
		}
		bool CalculateColumnRowAreaNeedUpdate(bool isColumn) {
			int areaIndex = (int)(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea);
			List<TColumn> oldArea = oldAreas[areaIndex];
			List<TColumn> newArea = newAreas[areaIndex];
			int count = Math.Min(oldArea.Count, newArea.Count);
			int diffIndex = count;
			for(int i = 0; i < count; i++)
				if(oldArea[i] != newArea[i]) {
					diffIndex = i;
					break;
				}
			if(diffIndex == 0 && (oldArea.Count != 0 || newArea.Count != 0))
				return true;
			for(int i = diffIndex; i < oldArea.Count; i++)
				if(NeedUpdateOnVisibleChange(oldArea[i], false))
					return true;
			for(int i = diffIndex; i < newArea.Count; i++)
				if(NeedUpdateOnVisibleChange(newArea[i], true))
					return true;
			if(CheckIfLastLevelSumsOnlyVisible(oldArea, newArea, diffIndex, isColumn))
				return true;
			if(diffIndex > 0 && diffIndex < oldArea.Count)
				updates.Add(new RemoveFieldValuesLevelUpdater<TColumn>(owner, isColumn, diffIndex));
			return false;
		}
		bool CheckIfLastLevelSumsOnlyVisible(List<TColumn> oldArea, List<TColumn> newArea, int diffIndex, bool isColumn) {
			if(areas.ServerSideDataArea.Count == 0)
				return false;
			if(areas.GetFieldValues(isColumn).ForAnyGroup((c) => c.Level >= diffIndex) && oldArea.Count > newArea.Count && diffIndex == newArea.Count && NeedRefreshOnColumnHide(oldArea, diffIndex - 1))
				return true;
			return false;
		}
		protected abstract bool NeedRefreshOnColumnHide(List<TColumn> area, int level);
		bool NeedUpdateOnVisibleChange(TColumn column, bool isNewColumn) {
			AreasState<TColumn> areas = isNewColumn ? oldAreas : newAreas;
			if(areas[PivotArea.ColumnArea].Contains(column) || areas[PivotArea.RowArea].Contains(column) || areas[PivotArea.FilterArea].Contains(column))
				return false;
			return UpdateAllOnColumnVisibleChange(column);
		}
		bool CalculateDataAreaChange() {
			return CalculateDataAreaChange(oldAreas.ServerSideDataArea, areas.ServerSideDataArea);
		}
		protected virtual bool CalculateDataAreaChange(List<TColumn> oldColumns, List<TColumn> newColumns) {
			foreach(TColumn newColumn in newColumns) {
				if(!oldColumns.Contains(newColumn) && !(newColumn is IUnboundSummaryLevelMetadataColumn))
					return true;
			}
			bool needCleanFieldValuesCells = false;
			foreach(TColumn oldColumn in oldColumns)
				if(!newColumns.Contains(oldColumn)) {
					IQueryMetadataColumn oldMetaColumn;
					if(!oldMetas.TryGetValue(oldColumn, out oldMetaColumn))
						oldMetaColumn = oldColumn.Metadata;
					updates.Add(new RemoveDataFieldUpdater<TColumn>(owner, oldMetaColumn));
					needCleanFieldValuesCells = true;
				}
			if(removeEmptyData && needCleanFieldValuesCells)
				updates.Add(new RemoveEmptyFieldValuesUpdater<TColumn>(owner));
			return false;
		}
		protected virtual bool UpdateAllOnColumnVisibleChange(TColumn column) {
			return false;
		}
		internal void DoPartialUpdate() {
			foreach(UpdaterBase<TColumn> updater in updates)
				updater.Update();
		}
	}
	public enum DataSourceUpdateType { None, Partial, Full }
}
