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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.PivotGrid.OLAP {
	[Browsable(false)]
	public class OLAPDataSourceQueryExecutor : QueryExecutorBase<OLAPCubeColumn, IOLAPHelpersOwner>, IOLAPQueryExecutor, IOLAPDataSourceQueryOwner {
		readonly OLAPDataSourceQueryBase queryBuilder;
		readonly OLAPCellSetParserBase cellSetParser;
#if DEBUGTEST
		string lastQuery;
		protected internal string LastQuery {
			get { return lastQuery; }
		}
#endif
		protected new IOLAPMetadata Metadata { get { return (IOLAPMetadata)base.Metadata; } }
		OLAPMetadataColumns Columns { get { return Metadata.Columns; } }
		protected internal OLAPDataSourceQueryBase QueryBuilder { get { return queryBuilder; } }
		public OLAPDataSourceQueryExecutor(OLAPCellSetParserBase cellSetParser, QueryMetadata<OLAPCubeColumn> metadata)
			: base(metadata) {
			this.queryBuilder = CreateQueryBuilder();
			this.cellSetParser = cellSetParser;
		}
		OLAPDataSourceQueryBase CreateQueryBuilder() {
			string version = Metadata.ServerVersion;
			if(OLAPMetadataHelper.IsAS2000(version))
				return new OLAPDataSourceQuery2000(this);
			if(OLAPMetadataHelper.IsAS2005(version))
				return new OLAPDataSourceQuery2005(this);
			return new OLAPDataSourceQuery2008(this);
		}
		protected override CellSet<OLAPCubeColumn> QueryData(IQueryContext<OLAPCubeColumn> context) {
			IOLAPQueryContext olapContext = (IOLAPQueryContext)context;
			bool columnExpand = context.ColumnExpand,
				rowExpand = context.RowExpand;
			olapContext.ValidateColumnsRows();
			if(context.Areas.DataArea.Count == 0 && (!CurrentOwner.Options.UseDefaultMeasure || context.ColumnArea.Count == 0 && context.RowArea.Count == 0))
				return null;
			List<OLAPCubeColumn> filterArea = CurrentOwner.FilterHelper.GetFilteredColumns(PivotArea.FilterArea),
				columnRowFilters = CurrentOwner.FilterHelper.GetFilteredColumns(PivotArea.ColumnArea, PivotArea.RowArea);
			string mdxQuery = QueryBuilder.GetQueryString(CubeName, context.ColumnArea, context.RowArea, olapContext.ColumnRowCustomDefaultMemberColumns, context.ColumnTuples,
				context.RowTuples, context.Areas.DataArea.Count > 0 ? context.Areas.ServerSideDataArea : GetDefaultMeasure(),
				columnRowFilters, filterArea, columnExpand, rowExpand);
			if(mdxQuery == null)
				return null;
#if DEBUGTEST
			lastQuery = mdxQuery;
#endif
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					return cellSetParser.QueryData(command, context);
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QueryDataException, e, command.CommandText);
					if(TryHandleException(raisedException, CurrentOwner))
						return null;
					throw e;
				}
			}
		}
		List<OLAPCubeColumn> GetDefaultMeasure() {
			OLAPMetadataColumn metadata = new OLAPMetadataColumn(0, 0, OLAPDataTypeConverter.Convert(OLAPDataType.PropVariant), null,
											  new OLAPHierarchy("[Measures].defaultMember", "[Measures].defaultMember"), null, null, null, OLAPDataType.Variant, null, null);
			return new List<OLAPCubeColumn>() { new OLAPCubeColumn(metadata) };
		}
		protected override object[] QueryAvailableValues(OLAPCubeColumn column, bool deferUpdates, List<OLAPCubeColumn> customFilters) {
			return QueryVisibleOrAvailableValues(column, true, deferUpdates, customFilters).ToArray();
		}
		protected override List<object> QueryVisibleValues(OLAPCubeColumn column) {
			return QueryVisibleOrAvailableValues(column, false, false, null);
		}
		List<object> QueryVisibleOrAvailableValues(OLAPCubeColumn column, bool queryAvailableValues, bool deferUpdates, List<OLAPCubeColumn> customFilters) {
			if(!Connected || IsDesignMode)
				return new List<object>();
			List<OLAPCubeColumn> columnRowFilters = MDX.GetAllFilteredColumns(deferUpdates, customFilters, CurrentOwner.FilterHelper);
			string mdxQuery = queryAvailableValues ?
				QueryBuilder.GetAvailableValuesQuery(column, CubeName, columnRowFilters, CurrentOwner.Areas.ServerSideDataArea, Columns.NonAggregatables, deferUpdates) :
				QueryBuilder.GetVisibleValuesQuery(column, CubeName, columnRowFilters, CurrentOwner.Areas.ServerSideDataArea, Columns.NonAggregatables, queryAvailableValues, deferUpdates);
			if(string.IsNullOrEmpty(mdxQuery))
				return new List<object>();
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					return cellSetParser.QueryVisibleOrAvailableValues(command, column);
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QueryVisibleValuesException, e, command.CommandText, column.UniqueName);
					if(TryHandleException(raisedException, column.Owner))
						return new List<object>();
					throw e;
				}
			}
		}
		protected override bool QueryNullValues(IQueryMetadataColumn column) {
			if(!Connected || IsDesignMode || column.IsMeasure)
				return false;
			OLAPMetadataColumn olapColumn = (OLAPMetadataColumn)column;
			string mdxQuery = QueryBuilder.GetNullValuesQueryString(CubeName, olapColumn.UniqueName, olapColumn.Hierarchy.UniqueName);
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					List<object> vals = new List<object>(cellSetParser.QueryValue(command));
					return vals.Count == 1 && Convert.ToInt32(vals[0]) > 0;
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QueryNullValuesException, e, command.CommandText, olapColumn.UniqueName);
					if(TryHandleException(raisedException))
						return false;
					throw e;
				}
			}
		}
		protected void QueryMembers(OLAPCubeColumn column, string[] uniqueNames) {
			OLAPMetadataColumn meta = column.Metadata;
			if(!Connected || IsDesignMode)
				return;
			List<OLAPMetadataColumn> nonaggregatables = Columns.NonAggregatables;
			if(uniqueNames != null && uniqueNames.Length > meta.Cardinality * 3 / 4 || uniqueNames != null && uniqueNames.Length == 0)
				uniqueNames = null; 
			int req = 100000;
			if(uniqueNames == null && meta.Cardinality > req + 100) {
				int reqCount = Convert.ToInt32(Math.Ceiling(((double)meta.Cardinality + 100) / (double)req));
				for(int i = 0; i < reqCount; i++)
					QueryMembersCore(nonaggregatables, column, uniqueNames, (i + 1) * req, req);
			} else
				QueryMembersCore(nonaggregatables, column, uniqueNames, -1, -1);
			if(uniqueNames == null || uniqueNames.Length == 0)
				meta.AllMembersLoaded = true;
		}
		void QueryMembersCore(List<OLAPMetadataColumn> nonaggregatables, OLAPCubeColumn olapColumn, string[] newMembers, int head, int tail) {
			string mdxQuery = QueryBuilder.GetMembersQueryString(CubeName, olapColumn.Metadata, olapColumn, newMembers, nonaggregatables, head, tail, Metadata.CanCreateMember());
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					cellSetParser.QueryMembers(command, olapColumn.Metadata, olapColumn, null);
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QueryMembersException, e, command.CommandText, olapColumn.UniqueName);
					if(TryHandleException(raisedException))
						return;
					throw e;
				}
			}
		}
		protected void QueryColumnProperties() {
		}
		protected void QueryChildMembers(OLAPMetadataColumn meta, OLAPCubeColumn childColumn, OLAPMember member, bool queryMemberValueFast) {
			if(!Connected || IsDesignMode)
				return;
			List<OLAPMetadataColumn> nonaggregatables = Columns.NonAggregatables;
			if(meta == null)
				return;
			string mdxQuery = QueryBuilder.GetChildMembersQueryString(member, meta, childColumn, CubeName, nonaggregatables, queryMemberValueFast);
#if DEBUGTEST
			lastQuery = mdxQuery;
#endif
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					cellSetParser.QueryMembers(command, meta, childColumn, (OLAPChildMember)member);
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QueryMembersException, e, command.CommandText, member.Column.UniqueName);
					if(TryHandleException(raisedException))
						return;
					throw e;
				}
			}
		}
		protected OLAPMember[] QuerySortMembers(OLAPCubeColumn column, IEnumerable<QueryMember> members) {
			if(!Connected || IsDesignMode)
				return new OLAPMember[0];
			bool allColumnMembers = false;
			OLAPMember[] cached = null;
			if(members != null)
				cached = members.Cast<OLAPMember>().ToArray();
			int memberCount;
			if(cached == null) {
				memberCount = 0;
				allColumnMembers = true;
			} else {
				memberCount = cached.Length;
				if(memberCount == 0)
					return new OLAPMember[0];
			}
			bool sortOnServer = OLAPSortedMembersWriter.SortOnServer(column.SortMode, column.TopValueCount > 0, column.SortBySummary != null, false);
			List<OLAPMember> notLoaded = GetNotLoadedSortProperties(column, cached, memberCount);
			bool containsNotLoadedSort = notLoaded.Count > 0;
			if(memberCount == 0 || sortOnServer || containsNotLoadedSort) {
				string mdxQuery = QueryBuilder.GetSortedMembersQueryString(CubeName, containsNotLoadedSort ? notLoaded.ToArray() : cached, column, Columns.NonAggregatables, column.Metadata.Owner.CanCreateMember());
				using(IOLAPCommand command = CreateCommand(mdxQuery)) {
					command.CommandTimeout = QueryTimeout;
					try {
						OLAPMember[] ccached = cellSetParser.QueryMembers(command, column.Metadata, column, null).ToArray();
						if(!containsNotLoadedSort)
							cached = ccached;
						if(allColumnMembers)
							column.Metadata.AllMembersLoaded = true;
					} catch(Exception e) {
						OLAPException raisedException = new OLAPException(OLAPException.QuerySortedMembersException, e, command.CommandText, column.UniqueName);
						if(TryHandleException(raisedException, column.Owner))
							return new OLAPMember[0];
						throw e;
					}
				}
			}
			if(!sortOnServer)
				Array.Sort(cached, SortHelper.GetResultComparer<OLAPCubeColumn, QueryMember>(CurrentOwner.GetCustomFieldSort(new EmptyCustomSortHelper(), column), column, column.GetByMemberComparer(CurrentOwner.GetCustomFieldText(column)), (a, b, c) => new CustomComparer<OLAPCubeColumn>(a, b, c)));
			return cached;
		}
		List<OLAPMember> GetNotLoadedSortProperties(OLAPCubeColumn column, OLAPMember[] cached, int memberCount) {
			List<OLAPMember> notLoaded = new List<OLAPMember>();
			if(column.SortMode == PivotSortMode.Key || column.SortMode == PivotSortMode.ID || !string.IsNullOrEmpty(column.ActualSortProperty)) {
				Func<OLAPMember, bool> sp = ((OLAPMetadata)Metadata).GetCheckSortProperty(column);
				for(int i = 0; i < memberCount; i++) {
					OLAPMember member = cached[i];
					if(!sp(member))
						notLoaded.Add(member);
				}
			}
			return notLoaded;
		}
		protected override IDataTable QueryDrillDown(QueryMember[] columnMembers, QueryMember[] rowMembers,
											OLAPCubeColumn measure, int maxRowCount, List<string> customColumns) {
			OLAPAreas areas = CurrentOwner.Areas;
			if(columnMembers == null || rowMembers == null)
				throw new ArgumentException("null argument");
			List<string> filters = new List<string>();
			if(measure != null)
				filters.Add(measure.UniqueName);
			CreateFilters(filters, columnMembers);
			CreateFilters(filters, rowMembers);
			foreach(OLAPCubeColumn column in CurrentOwner.FilterHelper.GetFilteredColumns(PivotArea.FilterArea)) {
				OLAPFilterValues filterValues = CurrentOwner.FilterHelper.GetIncludedDrillDownFilterValues(column);
				if(filterValues == null)
					continue;
				if(filterValues.GetMemberCount() > 1 || filterValues.GetMemberCount() == -1)
					filterValues = CurrentOwner.FilterHelper.GetFilterValues(null, column, true, false);
				if(!filterValues.IsSingleValueFilter || !filterValues.IsIncluded) {
					OLAPException raisedException = new OLAPException(PivotGridLocalizer.GetString(PivotGridStringId.OLAPDrillDownFilterException), null, null, null, true);
					if(TryHandleException(raisedException, CurrentOwner, true))
						return null;
				}
				filters.Add(filterValues.GetSingleMember());
			}
			foreach(OLAPCubeColumn column in CurrentOwner.FilterHelper.GetFilteredColumns(PivotArea.ColumnArea, PivotArea.RowArea))
				CurrentOwner.FilterHelper.GetFilterValues(null, column, true, false);
			List<string> drilldownColumns;
			if(customColumns != null && customColumns.Count > 0)
				drilldownColumns = customColumns;
			else {
				drilldownColumns = new List<string>(areas.ColumnArea.Count + areas.RowArea.Count + 1);
				CreateDrillDownColumns(drilldownColumns, areas.ColumnArea, false);
				CreateDrillDownColumns(drilldownColumns, areas.RowArea, false);
				CreateDrillDownColumns(drilldownColumns, areas.FilterArea, true);
				if(measure != null)
					CreateDrillDownColumn(drilldownColumns, measure.Metadata);
			}
			string mdxQuery = QueryBuilder.GetDrillDownQueryString(CubeName, filters, drilldownColumns, maxRowCount);
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				IDisposable dis;
				try {
					IOLAPRowSet result = cellSetParser.QueryDrillDown(command);
					dis = result as IDisposable;
					IDataTable table = GetDrillDown(result);
					if(dis != null)
						dis.Dispose();
					return table;
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(PivotGridLocalizer.GetString(PivotGridStringId.DrillDownException), e, command.CommandText, drilldownColumns, true);
					if(TryHandleException(raisedException, CurrentOwner))
						return null;
					throw e;
				}
			}
		}
		protected PivotOLAPKPIValue QueryKPIValue(string kpiName) {
			if(!Connected || IsDesignMode)
				return null;
			PivotOLAPKPIMeasures measures = Metadata.GetKPIMeasures(kpiName);
			if(measures == null)
				return null;
			string mdxQuery = QueryBuilder.GetKPIValueQuery(kpiName, CubeName);
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				try {
					return cellSetParser.QueryKPIValue(command, measures, kpiName, (OLAPMetadata)Metadata);
				} catch(Exception e) {
					OLAPException raisedException = new OLAPKPIException(e, command.CommandText, kpiName);
					if(TryHandleException(raisedException))
						return null;
					throw e;
				}
			}
		}
		protected bool IsDesignMode { get { return Metadata.IsDesignMode; } }
		protected bool Connected { get { return Metadata.Connected; } }
		protected int QueryTimeout { get { return Metadata.QueryTimeout; } }
		protected string CubeName { get { return Metadata.CubeName; } }
		protected IOLAPCommand CreateCommand(string mdx) {
			return Metadata.CreateCommand(mdx);
		}
		protected void CreateDrillDownColumns(List<string> drilldownColumns, List<OLAPCubeColumn> columns, bool filteredOnly) {
			for(int i = 0; i < columns.Count; i++) {
				if(filteredOnly && !columns[i].Filtered)
					continue;
				CreateDrillDownColumn(drilldownColumns, columns[i].Metadata);
			}
		}
		protected void CreateDrillDownColumn(List<string> drilldownColumns, OLAPMetadataColumn column) {
			if(!column.IsMeasure)
				drilldownColumns.Add("MemberValue(" + column.DrillDownColumn + ")");
			else
				drilldownColumns.Add(column.DrillDownColumn);
		}
		protected void CreateFilters(List<string> filters, QueryMember[] members) {
			OLAPMember lastMember = null;
			foreach(OLAPMember member in members) {
				if(lastMember == null || !lastMember.Column.IsParent(member.Column))
					filters.Add(member.UniqueName);
				else
					filters[filters.Count - 1] = member.UniqueName;
				lastMember = member;
			}
		}
		protected IDataTable GetDrillDown(IOLAPRowSet rowSet) {
			List<OLAPCubeColumn[]> returnColumns = GetReturnColumns(rowSet);
			List<IColumn> columns = new List<IColumn>();
			List<object[]> rows = new List<object[]>();
			Dictionary<int, int> columnIndexes = new Dictionary<int, int>();
			List<int> dataIndexes = new List<int>();
			for(int i = 0; i < rowSet.ColumnCount; i++) {
				int index = DataTableWrapper.GetColumnIndex(columns, rowSet.GetColumnName(i));
				if(index >= 0) {
					columnIndexes[i] = index;
				} else {
					columns.Add(new ColumnWrapper(rowSet.GetColumnName(i), Metadata.GetColumnCaptionByDrilldownName(rowSet.GetColumnName(i)), rowSet.GetColumnType(i)));
					columnIndexes[i] = columns.Count - 1;
					dataIndexes.Add(i);
				}
			}
			int realColumnCount = dataIndexes.Count;
			OLAPDrillDownFilter.IsRowFitDelegate filter = new OLAPDrillDownFilter(CurrentOwner.FilterHelper, returnColumns, columnIndexes).GetFilter();
			while(rowSet.NextRow()) {
				object[] row = new object[realColumnCount];
				for(int i = 0; i < realColumnCount; i++)
					row[i] = rowSet.GetCellValue(dataIndexes[i]);
				if(filter == null || filter(row))
					rows.Add(row);
			}
			return new DataTableWrapper(columns, rows);
		}
		List<OLAPCubeColumn[]> GetReturnColumns(IOLAPRowSet rowSet) {
			List<OLAPCubeColumn[]> res = new List<OLAPCubeColumn[]>();
			for(int i = 0; i < rowSet.ColumnCount; i++) {
				object columns = Columns.GetByDrillDownColumn(rowSet.GetColumnName(i));
				if(columns == null) {
					res.Add(null);
				} else {
					OLAPMetadataColumn column = columns as OLAPMetadataColumn;
					OLAPCubeColumn cubeColumn = column == null ? null : CurrentOwner.CubeColumns[column.UniqueName];
					if(column != null) {
						res.Add(cubeColumn.Filtered ? new OLAPCubeColumn[] { cubeColumn } : null);
					} else {
						List<OLAPMetadataColumn> list = (List<OLAPMetadataColumn>)columns;
						List<OLAPCubeColumn> cubeColumns = new List<OLAPCubeColumn>();
						for(int j = list.Count - 1; j >= 0; j--) {
							cubeColumn = CurrentOwner.CubeColumns[list[j].UniqueName];
							if(cubeColumn.Filtered)
								cubeColumns.Add(cubeColumn);
						}
						if(cubeColumns.Count > 0)
							res.Add(cubeColumns.ToArray());
						else
							res.Add(null);
					}
				}
			}
			return res;
		}
		protected override void QueryAggregations(IList<AggregationLevel> aggregationLevels) {
			if(!Connected || IsDesignMode) {
				AggregationLevel.SetErrorValue(aggregationLevels);
				return;
			}
			List<AggregationItemValue> actions = new List<AggregationItemValue>();
			string mdxQuery = QueryBuilder.GetCustomSummaryString(CurrentOwner.Areas, CubeName, aggregationLevels, actions);
			using(IOLAPCommand command = CreateCommand(mdxQuery)) {
				command.CommandTimeout = QueryTimeout;
				try {
					List<object> list = new List<object>(cellSetParser.QueryValue(command));
					for(int i = 0; i < actions.Count; i++) {
						AggregationItemValue item = actions[i];
						switch(item.SummaryType) {
							case SummaryItemTypeEx.TopPercent:
							case SummaryItemTypeEx.BottomPercent: {
									item.SetValue(new object[] { list[i] });
									break;
								}
							case SummaryItemTypeEx.Top:
							case SummaryItemTypeEx.Bottom: {
									object[] result = new object[Convert.ToInt32(item.SummaryArgument)];
									result[result.Length - 1] = list[i];
									item.SetValue(result);
									break;
								}
							default: {
									item.SetValue(list[i]);
									break;
								}
						}
					}
				} catch(Exception e) {
					OLAPException raisedException = new OLAPException(OLAPException.QuerySortedMembersException, e, command.CommandText, string.Empty);
					if(!TryHandleException(raisedException))
						throw e;
					foreach(AggregationItemValue item in actions)
						item.SetValue(PivotCellValue.ErrorValue);
				}
			}
		}
		#region IOLAPQueryExecutor Members
		void IOLAPQueryExecutor.QueryChildMembers(OLAPCubeColumn column, OLAPMember member) {
			this.QueryChildMembers(member.Column.ChildColumn, column, member, Metadata.CanCreateMember());
		}
		void IOLAPQueryExecutor.QueryMembers(OLAPCubeColumn column, string[] uniqueNames) {
			this.QueryMembers(column, uniqueNames);
		}
		OLAPMember[] IOLAPQueryExecutor.QuerySortMembers(IOLAPHelpersOwner owner, OLAPCubeColumn column, IEnumerable<QueryMember> members) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return this.QuerySortMembers(column, members);
		}
		PivotOLAPKPIValue IOLAPQueryExecutor.QueryKPIValue(string kpiName) {
			return this.QueryKPIValue(kpiName);
		}
		#endregion
		IOLAPFilterHelper IOLAPDataSourceQueryOwner.FilterHelper {
			get { return CurrentOwner.FilterHelper; }
		}
		bool IOLAPDataSourceQueryOwner.DimensionPropertiesSupported {
			get { return Metadata.DimensionPropertiesSupported; }
		}
		PivotGridOptionsOLAP IOLAPDataSourceQueryOwner.Options {
			get { return CurrentOwner.Options; }
		}
	}
#if !SL && !DXPORTABLE
	public class DataReaderWrapper : IOLAPRowSet, IDisposable {
		IDisposable disp;
		public static IOLAPRowSet Wrap(System.Data.IDataReader reader) {
			return new DataReaderWrapper(reader);
		}
		public static IOLAPRowSet Wrap(System.Data.DataTable dataTable) {
			return Wrap(dataTable.CreateDataReader());
		}
		System.Data.IDataReader reader;
		DataReaderWrapper(System.Data.IDataReader reader) {
			disp = reader as IDisposable;
			this.reader = reader;
		}
		#region IOLAPRowSet Members
		int IOLAPRowSet.ColumnCount {
			get { return this.reader.FieldCount; }
		}
		Type IOLAPRowSet.GetColumnType(int index) {
			return this.reader.GetFieldType(index);
		}
		string IOLAPRowSet.GetColumnName(int index) {
			return this.reader.GetName(index);
		}
		object IOLAPRowSet.GetCellValue(int columnIndex) {
			return this.reader.GetValue(columnIndex);
		}
		string IOLAPRowSet.GetCellStringValue(int columnIndex) {
			return this.reader.GetValue(columnIndex) as string;
		}
		bool IOLAPRowSet.NextRow() {
			return this.reader.Read();
		}
		#endregion
		void IDisposable.Dispose() {
			if(disp != null)
				disp.Dispose();
		}
	}
#endif
	public class OLAPDrillDownFilter {
		public delegate bool IsRowFitDelegate(object[] row);
		readonly List<OLAPCubeColumn[]> returnColumns;
		readonly Dictionary<OLAPCubeColumn, FilterCache> filterValuesCache;
		readonly IOLAPFilterHelper filterHelper;
		readonly Dictionary<int, int> columnIndexes;
		public OLAPDrillDownFilter(IOLAPFilterHelper filterHelper, List<OLAPCubeColumn[]> returnColumns, Dictionary<int, int> columnIndexes) {
			this.filterHelper = filterHelper;
			this.returnColumns = returnColumns;
			this.filterValuesCache = new Dictionary<OLAPCubeColumn, FilterCache>();
			this.columnIndexes = columnIndexes;
		}
		public bool RequiresFilter {
			get {
				for(int i = 0; i < returnColumns.Count; i++) {
					if(returnColumns[i] != null)
						return true;
				}
				return false;
			}
		}
		public IsRowFitDelegate GetFilter() {
			if(!RequiresFilter)
				return null;
			else
				return IsRowFit;
		}
		FilterCache CreateFilterCache(OLAPCubeColumn column) {
			OLAPFilterValues filterValues = filterHelper.GetFilterValues(null, column, true, false);
			return filterValues != null ? new FilterCache(filterValues) : null;
		}
		bool IsRowFit(object[] row) {
			if(row.Length > columnIndexes.Count)
				throw new ArgumentException("Invalid row");
			for(int i = 0; i < columnIndexes.Count; i++) {
				if(!IsValueFit(i, row[columnIndexes[i]]))
					return false;
			}
			return true;
		}
		bool IsValueFit(int columnIndex, object value) {
			OLAPCubeColumn[] columns = returnColumns[columnIndex];
			if(columns == null)
				return true;
			for(int i = 0; i < columns.Length; i++) {
				if(!IsValueFitCore(columns[i], value))
					return false;
			}
			return true;
		}
		bool IsValueFitCore(OLAPCubeColumn column, object value) {
			FilterCache cache;
			while(column.ParentColumn != null)
				column = column.ParentColumn;
			if(!filterValuesCache.TryGetValue(column, out cache)) {
				cache = CreateFilterCache(column);
				filterValuesCache.Add(column, cache);
			}
			return cache == null || cache.IsValueFit(value);
		}
		class FilterCache {
			readonly bool isIncluded;
			readonly NullableDictionary<object, object> cache;
			public FilterCache(OLAPFilterValues filterValues) {
				this.isIncluded = filterValues.IsIncluded;
				ReadOnlyCollection<OLAPMember> filters = filterValues.GetProcessedMembers();
				this.cache = new NullableDictionary<object, object>(filters.Count);
				for(int i = 0; i < filters.Count; i++) {
					OLAPChildMember member = filters[i] as OLAPChildMember;
					if(member != null)
						while(member != null) {
							if(!this.cache.Contains(member.Value))
								this.cache.Add(member.Value, null);
							member = filterValues.IsIncluded ? member.ParentMember : null;
						} else {
						OLAPMember oMember = filters[i];
						if(!this.cache.Contains(oMember.Value))
							this.cache.Add(oMember.Value, null);
					}
				}
			}
			public bool IsValueFit(object value) {
				bool res = this.cache.Contains(value);
				return isIncluded ? res : !res;
			}
		}
	}
}
