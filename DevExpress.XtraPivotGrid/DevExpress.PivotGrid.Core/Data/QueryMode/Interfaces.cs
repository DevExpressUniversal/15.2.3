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
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.QueryMode {
	public interface IActionProvider<TColumn, TCellSet>
		where TColumn : QueryColumn
		where TCellSet : CellSet<TColumn> {
		List<TColumn> GetColumns(List<TColumn> area, GroupInfo[] groups);
		List<QueryTuple> GetTuples(List<TColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups);
		void ParseFieldValues(TCellSet cellSet, IQueryContext<TColumn> context, bool isColumn);
	}
	public interface IQueryAliasColumn {
		string OriginalColumn { get; }
	}
	public interface IQueryFilterHelper {
		bool BeforeSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields);
		bool AfterSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields);
		IEnumerable<QueryColumn> GetAdditionalFilteredColumns();
		void ClearCache();
	}
	interface IUnboundMetadataColumn {
		bool IsServer { get; }
		bool UpdateCriteria(PivotGridFieldBase field);
	}
	interface IUnboundSummaryLevelMetadataColumn : IUnboundMetadataColumn {
		CriteriaOperator Criteria { get; }
		Type DataType { get; }
		object EvaluateValue(MeasuresStorage storage);
		PivotCellValue EvaluatePivotCellValue(MeasuresStorage storage);
	}
	public interface IQueryMetadata {
		QueryMetadataColumns Columns { get; }
		QueryMember LoadMember(IQueryMetadataColumn queryColumn, TypedBinaryReader reader);
		void SaveMember(QueryMember member, TypedBinaryWriter writer);
	}
	public interface IDataSourceHelpersOwner<AreasType> : IPivotGridDataSource where AreasType : QueryColumn {
		Dictionary<AreasType, PivotGridFieldBase> FieldsByColumns { get; }
		IQueryFilterHelper FilterHelper { get; }
		QueryColumns<AreasType> CubeColumns { get; }
		CancellationToken CancellationToken { get; }
		bool Connected { get; }
		bool IsDesignMode { get; }
		void ChangeFieldExpanded(bool expanded, bool isColumn, ICollection<GroupInfo> groups);
		IQueryMetadata Metadata { get; }
		CriteriaOperator PatchCriteria(CriteriaOperator criteria);
		AreasType CreateColumn(IQueryMetadataColumn column, PivotGridFieldBase field);
		Func<IQueryMemberProvider, IQueryMemberProvider, int?> GetCustomFieldSort(ICustomSortHelper helper, AreasType column);
		Func<object, string> GetCustomFieldText(AreasType column);
		bool HandleException(QueryHandleableException ex);
	}
	public interface IPartialUpdaterOwner<TColumn> where TColumn : QueryColumn {
		QueryAreas<TColumn> Areas { get; }
		QueryColumns<TColumn> CubeColumns { get; }
		void ExpandAll();
		void QueryData(GroupInfo[] columnGroups, GroupInfo[] rowGroups, bool columnExpand, bool rowExpand);
	}
	public interface IColumn {
		string Name { get; }
		string DisplayName { get; }
		Type ColumnType { get; }
	}
	public interface IDataTable {
		IList<IColumn> Columns { get; }
		IList<object[]> Rows { get; }
	}
	public interface IQueryExecutor<AreasType> where AreasType : QueryColumn {
		CellSet<AreasType> QueryData(IDataSourceHelpersOwner<AreasType> owner, IQueryContext<AreasType> context);
		List<object> QueryVisibleValues(IDataSourceHelpersOwner<AreasType> owner, AreasType column);
		object[] QueryAvailableValues(IDataSourceHelpersOwner<AreasType> owner, AreasType column, bool deferUpdates, List<AreasType> customFilters);
		bool QueryNullValues(IDataSourceHelpersOwner<AreasType> owner, IQueryMetadataColumn column);
		IDataTable QueryDrillDown(IDataSourceHelpersOwner<AreasType> owner, QueryMember[] columnMembers, QueryMember[] rowMembers,
								   AreasType measure, int maxRowCount, List<string> customColumns);
		void QueryAggregations(IDataSourceHelpersOwner<AreasType> owner, IList<AggregationLevel> aggregationLevels);
	}
	public interface IQueryMemberCollection {
		IQueryMetadataColumn Column { get; }
		int Count { get; }
	}
	public interface ICellTableOwner<TColumn> where TColumn : QueryColumn {
		List<TColumn> GetUniqueMeasureColumns();
	}
	public interface IQueryContext<AreasType> where AreasType : QueryColumn {
		AreaFieldValues RowValues { get; }
		AreaFieldValues ColumnValues { get; }
		List<QueryTuple> GetTuples(bool isColumn);
		List<QueryTuple> ColumnTuples { get; }
		List<QueryTuple> RowTuples { get; }
		bool ColumnExpand { get; }
		bool RowExpand { get; }
		List<AreasType> GetArea(bool isColumn);
		List<AreasType> ColumnArea { get; }
		List<AreasType> RowArea { get; }
		IDataSourceHelpersOwner<AreasType> Owner { get; }
		QueryAreas<AreasType> Areas { get; }
		void PreParseResult(CellSet<AreasType> queryResult);
		void PerformSorting();
		bool HandleException(QueryHandleableException exception);
	}
	class ColumnWrapper : IColumn {
		string name, displayName;
		Type type;
		public ColumnWrapper(string Name, string displayName, Type Type) {
			this.name = Name;
			this.displayName = displayName;
			this.type = Type;
		}
		#region IColumn Members
		string IColumn.Name { get { return name; } }
		string IColumn.DisplayName { get { return displayName; } }
		Type IColumn.ColumnType { get { return type; } }
		#endregion
	}
	public class DataTableWrapper : IDataTable {
		IList<IColumn> columns;
		IList<object[]> rows;
		public DataTableWrapper(IList<IColumn> columns, IList<object[]> rows) {
			this.columns = columns;
			this.rows = rows;
		}
		#region IDataTable Members
		IList<IColumn> IDataTable.Columns { get { return columns; } }
		IList<object[]> IDataTable.Rows { get { return rows; } }
		#endregion
		public static int GetColumnIndex(IList<IColumn> columns, string name) {
			for(int i = 0; i < columns.Count; i++)
				if(object.Equals(columns[i].Name, name))
					return i;
			return -1;
		}
	}
	public class QueryHandleableException : Exception {
		protected bool isResponce;
		public QueryHandleableException() : base() { }
		public QueryHandleableException(string message) : base(message) { }
		protected internal QueryHandleableException(string message, Exception innerException) : base(message, innerException) { }
		protected internal QueryHandleableException(string message, Exception innerException, bool isResponce) : base(message, innerException) {
			this.isResponce = isResponce;
		}
		protected internal bool IsNullReference {
			get { return this.InnerException is NullReferenceException; }
		}
		protected internal bool IsResponse {
			get { return isResponce || this.InnerException is IOLAPResponseException; }
		}
	}
}
