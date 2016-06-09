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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public interface IOLAPMemberSource {
		string UniqueName { get; }
		string Caption { get; }
		object GetValue(Type dataType);
	}
	#region
	public interface IOLAPCollection : System.Collections.IEnumerable {
		int Count { get; }
	}
	public interface IOLAPCollection<TEntity> : IOLAPCollection, IEnumerable<TEntity> where TEntity : IOLAPEntity {
		TEntity this[int index] { get; }
	}
	public interface IOLAPCell : IOLAPEntity {
		object Value { get; }
		string FormatString { get; }
		int Locale { get; }
	}
	public interface ITupleCollection : IEnumerable<IOLAPTuple> {
		int ReadedCount { get; }
	}
	public interface IOLAPAxis : IOLAPNamedEntity {
		ITupleCollection Tuples { get; }
	}
	public interface IOLAPTuple {
		int Count { get; }
		OLAPMember this[int index] { get; }
		OLAPMember Single();
	}
	public interface IOLAPCellSet {
		IEnumerable<IOLAPCell> Cells { get; }
		ITupleCollection GetColumnAxis(AxisColumnsProviderBase axisColumnsProvider);
		ITupleCollection GetRowAxis(AxisColumnsProviderBase axisColumnsProvider);
		void OnParsed();
	}
	public interface IOLAPRowSet {
		int ColumnCount { get; }
		Type GetColumnType(int index);
		string GetColumnName(int index);
		object GetCellValue(int columnIndex);
		string GetCellStringValue(int columnIndex);
		bool NextRow();
	}
	#endregion
	#region OLAP Connection
	public interface IOLAPConnection : IDisposable {
		string Database { get; }
		string ServerVersion { get; }
		IOLAPCommand CreateCommand(string mdx);
		void Open();
		void Close(bool endSession);
	}
	#endregion
	#region OLAP Command
	public interface IOLAPCommand : IDisposable {
		string CommandText { get; set; }
		int CommandTimeout { get; set; }
		IOLAPConnection Connection { get; }
	}
	#endregion
	#region OlapQueryExecutor
	public interface IOLAPQueryExecutor : IQueryExecutor<OLAPCubeColumn> {
		void QueryChildMembers(OLAPCubeColumn childColumn, OLAPMember member);
		void QueryMembers(OLAPCubeColumn column, string[] uniqueNames);
		OLAPMember[] QuerySortMembers(IOLAPHelpersOwner owner, OLAPCubeColumn column, IEnumerable<QueryMember> members);
		PivotOLAPKPIValue QueryKPIValue(string kpiName);
	}
	public interface IOLAPHelpersOwner : IPivotOLAPDataSource, IDataSourceHelpersOwner<OLAPCubeColumn> {
		OLAPAreas Areas { get; }
		AreaFieldValues ColumnValues { get; }
		AreaFieldValues RowValues { get; }
		PivotGridOptionsOLAP Options { get; }
		OLAPUniqueValues UniqueValues { get; }
		new IOLAPMetadata Metadata { get; }
		new IOLAPFilterHelper FilterHelper { get; }
		new OLAPCubeColumns CubeColumns { get; }
		void ClearState(bool layoutChanged);
		void OnSuccessConnected();
		bool IsLocked { get; }
		Dictionary<string, Dictionary<string, string>> GetExpressionNamesByHierarchies(CriteriaOperator criteria);
	}
	#endregion QueryExecutor
	public interface IOLAPFilterHelper : IQueryFilterHelper {
		bool IsFiltered(OLAPCubeColumn column, bool deferUpdates);
		bool HasGroupFilter(OLAPCubeColumn filter, bool deferUpdates);
		OLAPFilterValues GetFilterValues(QueryTuple parentTuple, OLAPCubeColumn column, bool includeChildValues, bool deferUpdates);
		OLAPFilterValues GetIncludedDrillDownFilterValues(OLAPCubeColumn column);
		OLAPFilterValues GetIncludedFieldFilterValues(OLAPCubeColumn filter, bool deferUpdates);
		bool IsIncludedFilter_SQL2000(OLAPCubeColumn filter, bool deferUpdates);
		OLAPFilterValues GetCompleteGroupFilterValues_SQL2000(OLAPCubeColumn filter, bool includeMiddleValues, bool deferUpdates);
		bool IsFilteredUsingWhereClause(OLAPCubeColumn filter, List<OLAPCubeColumn> measures);
		OLAPCubeColumn GetColumn(OLAPMetadataColumn metadata);
		List<OLAPCubeColumn> GetFilteredColumns(params PivotArea[] areas);
	}
	public interface IOLAPQueryContext : IQueryContext<OLAPCubeColumn> {
		List<OLAPCubeColumn> ColumnRowCustomDefaultMemberColumns { get; }
		Action<OLAPCubeColumn, string[]> QueryMembers { get; }
		void ValidateColumnsRows();
	}
	public interface IOLAPDataSourceQueryOwner {
		IOLAPFilterHelper FilterHelper { get; }
		bool DimensionPropertiesSupported { get; }
		PivotGridOptionsOLAP Options { get; }
	}
	public interface IOLAPMetadata : IQueryMetadata {
		new OLAPMetadataColumns Columns { get; }
		OLAPHierarchies Hierarchies { get; }
		bool Connected { get; }
		int QueryTimeout { get; }
		string CubeName { get; }
		string SessionID { get; set; }
		string ServerVersion { get; }
		bool IsLocked { get; }
		bool IsDesignMode { get; }
		bool DimensionPropertiesSupported { get; }
		bool IsGT2005 { get; }
		IOLAPCommand CreateCommand(string mdx);
		PivotOLAPKPIMeasures GetKPIMeasures(string kpiName);
		string GetColumnCaptionByDrilldownName(string name);
		void QueryMembers(OLAPCubeColumn queryColumn, params string[] uniqueName);
		void QueryChildMembers(OLAPCubeColumn column, QueryMember lastMember);
		void FetchMemberProperties(OLAPMember member);
		OLAPMember[] QuerySortMembers(IOLAPHelpersOwner owner, OLAPCubeColumn childGroupColumn, IEnumerable<QueryMember> unsortedMembers);
		bool CanCreateMember();
	}
	public interface IOLAPEditableMemberCollection : IQueryMemberCollection, IEnumerable<OLAPMember> {
		IList<OLAPMember> GetMembersByValue(bool recursive, int level, object value);
		OLAPMember GetMemberByUniqueLevelValue(string uniqueLevelValue);
		void Add(QueryMember member);
		void AddRange(IEnumerable<QueryMember> childMembers);
		bool Remove(QueryMember member);
	}
}
