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
using DevExpress.DataAccess.Localization;
namespace DevExpress.DataAccess.Sql {
	public class ValidationException : Exception {
		protected ValidationException(DataAccessStringId stringId) 
			: base(DataAccessLocalizer.GetString(stringId)) { }
		protected ValidationException(DataAccessStringId formatStringId, params object[] args)
			: base(string.Format(DataAccessLocalizer.GetString(formatStringId), args)) { }
		protected ValidationException(string message) : base(message) {
		}
	}
	#region TableQuery errors
	#region Tables & Columns
	public class TableNotInSchemaValidationException : ValidationException {
		readonly string tableName;
		public TableNotInSchemaValidationException(string tableName)
			: base(DataAccessStringId.TableNotInSchemaValidationException, tableName) {
			this.tableName = tableName;
		}
		public string TableName { get { return tableName; } }
	}
	public class ColumnNullValidationException : ValidationException {
		public ColumnNullValidationException() : base(DataAccessStringId.ColumnNullValidationException) { }
	}
	public class UnnamedColumnValidationException : ValidationException {
		public UnnamedColumnValidationException() : base(DataAccessStringId.UnnamedColumnValidationException) { }
	}
	public class DuplicatingColumnNamesValidationException : ValidationException {
		readonly string tableActualName;
		readonly string columnActualName;
		public DuplicatingColumnNamesValidationException(string tableActualName, string columnActualName)
			: base(DataAccessStringId.DuplicatingColumnNamesValidationException, tableActualName, columnActualName) {
			this.tableActualName = tableActualName;
			this.columnActualName = columnActualName;
		}
		public string TableActualName { get { return tableActualName; } }
		public string ColumnActualName { get { return columnActualName; } }
	}
	public class ColumnNotInSchemaValidationException : ValidationException {
		readonly string tableName;
		readonly string columnName;
		public ColumnNotInSchemaValidationException(string tableName, string columnName)
			: base(DataAccessStringId.ColumnNotInSchemaValidationException, tableName, columnName) {
			this.tableName = tableName;
			this.columnName = columnName;
		}
		public string TableName { get { return tableName; } }
		public string ColumnName { get { return columnName; } }
	}
	public class UnnamedTableValidationException : ValidationException {
		public UnnamedTableValidationException() : base(DataAccessStringId.UnnamedTableValidationException) { }
	}
	public class DuplicatingTableNamesValidationException : ValidationException {
		readonly string tableActualName;
		public DuplicatingTableNamesValidationException(string tableActualName)
			: base(DataAccessStringId.DuplicatingTableNamesValidationException, tableActualName) {
			this.tableActualName = tableActualName;
		}
		public string TableActualName { get { return tableActualName; } }
	}
	public class NoTablesValidationException : ValidationException {
		public NoTablesValidationException() : base(DataAccessStringId.NoTablesValidationException) { }
	}
	public class TableNullValidationException : ValidationException {
		public TableNullValidationException() : base(DataAccessStringId.TableNullValidationException) { }
	}
	public class NoColumnsValidationException : ValidationException {
		public NoColumnsValidationException() : base(DataAccessStringId.NoColumnsValidationException) { }
	}
	#endregion
	#region Relations
	public class RelationColumnNullValidationException : ValidationException {
		public RelationColumnNullValidationException() : base(DataAccessStringId.RelationColumnNullValidationException) { }
	}
	public class RelationColumnNotInSchemaValidationException : ValidationException {
		readonly string tableName;
		readonly string columnName;
		public RelationColumnNotInSchemaValidationException(string tableName, string columnName)
			: base(DataAccessStringId.RelationColumnNotInSchemaValidationException, tableName, columnName) {
			this.tableName = tableName;
			this.columnName = columnName;
		}
		public string TableName { get { return tableName; } }
		public string ColumnName { get { return columnName; } }
	}
	public class IncompleteRelationValidationException : ValidationException {
		readonly RelationInfo relation;
		public IncompleteRelationValidationException(RelationInfo relation)
			: base(DataAccessStringId.IncompleteRelationValidationException, relation) {
			this.relation = relation;
		}
		public RelationInfo Relation { get { return relation; } }
	}
	public class RelationTableNotSelectedValidationException : ValidationException {
		readonly string tableActualName;
		public RelationTableNotSelectedValidationException(string tableActualName)
			: base(DataAccessStringId.RelationTableNotSelectedValidationException, tableActualName) {
			this.tableActualName = tableActualName;
		}
		public string TableActualName { get { return tableActualName; } }
	}
	public class RelationNullValidationException : ValidationException {
		public RelationNullValidationException() : base(DataAccessStringId.RelationNullValidationException) { }
	}
	public class NoRelationColumnsValidationException : ValidationException {
		readonly RelationInfo relation;
		public NoRelationColumnsValidationException(RelationInfo relation)
			: base(DataAccessStringId.NoRelationColumnsValidationException, relation) {
			this.relation = relation;
		}
		public RelationInfo Relation { get { return relation; } }
	}
	public class CircularRelationsValidationException : ValidationException {
		public CircularRelationsValidationException() : base(DataAccessStringId.CircularRelationsValidationException) { }
	}
	public class TablesNotRelatedValidationException : ValidationException {
		readonly string[] relRoots;
		public TablesNotRelatedValidationException(string[] relRoots)
			: base(DataAccessStringId.TablesNotRelatedValidationException, string.Join("\", \"", relRoots)) {
			this.relRoots = relRoots;
		}
		public IEnumerable<string> RelRoots { get { return relRoots; } }
	}
	#endregion
	#region Filter
	public class FilterByColumnOfMissingTableValidationException : ValidationException {
		readonly string column;
		public FilterByColumnOfMissingTableValidationException(string column) 
			: base(DataAccessStringId.FilterByColumnOfMissingTableValidationException, column) {
			this.column = column;
		}
		public string Column { get { return column; } }
	}
	public class FilterByMissingInSchemaColumnValidationExpression : ValidationException {
		readonly string table;
		readonly string column;
		public FilterByMissingInSchemaColumnValidationExpression(string table, string column) 
			: base(DataAccessStringId.FilterByMissingInSchemaColumnValidationExpression, table, column) {
			this.table = table;
			this.column = column;
		}
		public string Table { get { return table; } }
		public string Column { get { return column; } }
	}
	public class FilterByAmbiguousColumnValidationException : ValidationException {
		readonly string column;
		readonly string[] tables;
		public FilterByAmbiguousColumnValidationException(string column, string[] tables)
			: base(
				DataAccessStringId.FilterByAmbiguousColumnValidationException, column,
				string.Join(", ", tables.Select(name => string.Format("[{0}]", name)))) {
			this.column = column;
			this.tables = tables;
		}
		public string Column { get { return column; } }
		public IEnumerable<string> Tables { get { return tables; } }
	}
	#endregion
	#region Sorting
	public class SortingBySameColumnTwiceValidationException : ValidationException {
		readonly string columnName;
		readonly string tableName;
		public SortingBySameColumnTwiceValidationException(string tableName, string columnName)
			: base(DataAccessStringId.SortingBySameColumnTwiceValidationException, tableName, columnName) {
			this.tableName = tableName;
			this.columnName = columnName;
		}
		public string TableName { get { return tableName; } }
		public string ColumnName { get { return columnName; } }
	}
	#endregion
	#region Aggregation
	public class AggregationWithoutAliasValidationException : ValidationException {
		public AggregationWithoutAliasValidationException() : base(DataAccessStringId.AggregationWithoutAliasValidationException) { }
	}
	public class PartialAggregationValidationException : ValidationException {
		public PartialAggregationValidationException() : base(DataAccessStringId.PartialAggregationValidationException) { }
	}
	public class GroupByAggregateColumnValidationException : ValidationException {
		readonly string table;
		readonly string column;
		public GroupByAggregateColumnValidationException(string table, string column)
			: base(DataAccessStringId.GroupByAggregateColumnValidationException, table, column) {
			this.table = table;
			this.column = column;
		}
		public string Table { get { return table; } }
		public string Column { get { return column; } }
	}
	#endregion
	#region Grouping
	public class HavingWithoutGroupByValidationException : ValidationException {
		public HavingWithoutGroupByValidationException() : base(DataAccessStringId.HavingWithoutGroupByValidationException) { }
	}
	public class GroupByWithoutAggregateValidationException : ValidationException {
		public GroupByWithoutAggregateValidationException() : base(DataAccessStringId.GroupByWithoutAggregateValidationException) { }
	}
	#endregion
	public class SkipWithoutSortingValidationException : ValidationException {
		public SkipWithoutSortingValidationException() : base(DataAccessStringId.SkipWithoutSortingValidationException) { }
	}
	#endregion
	#region CustomSqlQuery errors
	public class SqlStringEmptyValidationException : ValidationException {
		public SqlStringEmptyValidationException() : base(DataAccessStringId.SqlStringEmptyValidationException) { }
	}
	#endregion
	#region StoredProcQuery errors
	public class StoredProcNameNullValidationException : ValidationException {
		public StoredProcNameNullValidationException() : base(DataAccessStringId.StoredProcNameNullValidationException) { }
	}
	public class StoredProcNotInSchemaValidationException : ValidationException {
		readonly string storedProcName;
		public StoredProcNotInSchemaValidationException(string storedProcName)
			: base(DataAccessStringId.StoredProcNotInSchemaValidationException, storedProcName) {
			this.storedProcName = storedProcName;
		}
		public string StoredProcName { get { return storedProcName; } }
	}
	public class StoredProcParamCountValidationException : ValidationException {
		readonly int expected;
		readonly int actual;
		public StoredProcParamCountValidationException(int actual, int expected)
			: base(DataAccessStringId.StoredProcParamCountValidationException, actual, expected) {
			this.expected = expected;
			this.actual = actual;
		}
		public int Expected { get { return expected; } }
		public int Actual { get { return actual; } }
	}
	public class StoredProcParamNullValidationException : ValidationException {
		public StoredProcParamNullValidationException() : base(DataAccessStringId.StoredProcParamNullValidationException) { }
	}
	public class StoredProcParamNameValidationException : ValidationException {
		readonly string actual;
		readonly string expected;
		public StoredProcParamNameValidationException(string actual, string expected)
			: base(DataAccessStringId.StoredProcParamNameValidationException, actual, expected) {
			this.actual = actual;
			this.expected = expected;
		}
		public string Actual { get { return actual; } }
		public string Expected { get { return expected; } }
	}
	public class StoredProcParamTypeValidationException : ValidationException {
		readonly string actual;
		readonly string expected;
		public StoredProcParamTypeValidationException(string actual, string expected)
			: base(DataAccessStringId.StoredProcParamTypeValidationException, actual, expected) {
			this.actual = actual;
			this.expected = expected;
		}
		public string Actual { get { return actual; } }
		public string Expected { get { return expected; } }
	}
	#endregion
	public class CustomSqlQueryValidationException : ValidationException {
		public CustomSqlQueryValidationException() : base(DataAccessStringId.CustomSqlQueryValidationException) {
		}
		public CustomSqlQueryValidationException(string message) : base(message) {
		}
	}
}
