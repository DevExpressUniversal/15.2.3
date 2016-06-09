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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
namespace DevExpress.DataAccess.Sql {
	public class PositiveIntegerTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			var s = value as string;
			if(s != null) {
				int intValue;
				if(!int.TryParse(s, out intValue) || intValue < 0)
					throw new ArgumentException(DataAccessLocalizer.GetString(DataAccessStringId.PositiveIntegerError));
				return intValue;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class SkipTypeConverter : PositiveIntegerTypeConverter {
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			var parsedValue = base.ConvertFrom(context, culture, value);
			var query = (TableQuery)context.Instance;
			if(query.Sorting.Count == 0)
				throw new Exception(DataAccessLocalizer.GetString(DataAccessStringId.SkipWithoutSortingPropertyGridError));
			return parsedValue;
		}
	}
	public sealed class TableQuery : SqlQuery {
		#region static
		static void ValidateTableWithSchema(DBSchema schema, TableInfo table, string tableActualName, Dictionary<string, DBTable> dbTables, out DBTable dbTable) {
			dbTable = null;
			foreach(DBTable item in schema.Tables.Union(schema.Views))
				if(StringExtensions.CompareInvariantCultureIgnoreCase(item.Name, table.Name) == 0) {
					dbTable = item;
					dbTables.Add(tableActualName, dbTable);
					break;
				}
			if(dbTable == null)
				throw new TableNotInSchemaValidationException(table.Name);
		}
		static void ValidateColumnName(ColumnInfo column, HashSet<string> columnActualNames, string tableActualName) {
			if(column == null)
				throw new ColumnNullValidationException();
			if(string.IsNullOrEmpty(column.Name))
				throw new UnnamedColumnValidationException();
			string columnActualName = column.ActualName.ToLowerInvariant();
			if(columnActualNames.Contains(columnActualName))
				throw new DuplicatingColumnNamesValidationException(tableActualName, columnActualName);
			columnActualNames.Add(columnActualName);
		}
		static void ValidateColumnWithSchema(DBTable dbTable, TableInfo table, ColumnInfo column) {
			DBColumn dbColumn = null;
			foreach(DBColumn item in dbTable.Columns)
				if(StringExtensions.CompareInvariantCultureIgnoreCase(item.Name, column.Name) == 0) {
					dbColumn = item;
					break;
				}
			if(dbColumn == null)
				throw new ColumnNotInSchemaValidationException(table.Name, column.Name);
		}
		static void ValidateRelationKeyColumns(RelationInfo rel, DBTable parentDbTable, DBTable nestedDbTable) {
			HashSet<string> parentTableColumns = new HashSet<string>();
			HashSet<string> nestedTableColumns = new HashSet<string>();
			foreach(DBColumn col in parentDbTable.Columns)
				parentTableColumns.Add(col.Name);
			foreach(DBColumn col in nestedDbTable.Columns)
				nestedTableColumns.Add(col.Name);
			foreach(RelationColumnInfo keyColumn in rel.KeyColumns) {
				if(object.ReferenceEquals(keyColumn, null))
					throw new RelationColumnNullValidationException();
				if(!parentTableColumns.Contains(keyColumn.ParentKeyColumn, StringExtensions.ComparerInvariantCultureIgnoreCase))
					throw new RelationColumnNotInSchemaValidationException(rel.ParentTable, keyColumn.ParentKeyColumn);
				if(!nestedTableColumns.Contains(keyColumn.NestedKeyColumn, StringExtensions.ComparerInvariantCultureIgnoreCase))
					throw new RelationColumnNotInSchemaValidationException(rel.NestedTable, keyColumn.NestedKeyColumn);
			}
		}
		static void ValidateTableName(TableInfo table, HashSet<string> tableActualNames, out string tableActualName) {
			if(string.IsNullOrEmpty(table.Name)) {
				tableActualName = null;
				throw new UnnamedTableValidationException();
			}
			tableActualName = table.ActualName.ToLowerInvariant();
			if(tableActualNames.Contains(tableActualName)) {
				throw new DuplicatingTableNamesValidationException(tableActualName);
			}
			tableActualNames.Add(tableActualName);
		}
		static void ValidateRelationTables(RelationInfo rel, HashSet<string> tableActualNames, HashSet<string> relRoots, out string parentTable, out string nestedTable) {
			if(string.IsNullOrEmpty(rel.NestedTable) || string.IsNullOrEmpty(rel.ParentTable)) {
				parentTable = null;
				nestedTable = null;
				throw new IncompleteRelationValidationException(rel);
			}
			string wrongTable = null;
			nestedTable = rel.NestedTable.ToLowerInvariant();
			parentTable = rel.ParentTable.ToLowerInvariant();
			if(!tableActualNames.Contains(parentTable))
				wrongTable = parentTable;
			else if(!tableActualNames.Contains(nestedTable))
				wrongTable = nestedTable;
			if(wrongTable != null)
				throw new RelationTableNotSelectedValidationException(wrongTable);
			relRoots.Remove(nestedTable);
		}
		static void ValidateRelationWithSchema(RelationInfo rel, Dictionary<string, DBTable> dbTables, string parentTable, string nestedTable) {
			DBTable parentDbTable = dbTables[parentTable];
			DBTable nestedDbTable = dbTables[nestedTable];
			ValidateRelationKeyColumns(rel, parentDbTable, nestedDbTable);
		}		
		#endregion
		readonly TableInfoList tables;
		readonly RelationInfoList relations;
		string filterString;
		readonly SortingInfoList sorting;
		readonly GroupingInfoList grouping;
		int top;
		int skip;
		string groupFilterString;
		public TableQuery() : this(string.Empty) { }
		public TableQuery(string queryName) : base(queryName) {
			this.tables = new TableInfoList();
			this.relations = new RelationInfoList();
			this.sorting = new SortingInfoList();
			grouping = new GroupingInfoList();
		}
		internal TableQuery(TableQuery other) : base(other) {
			this.filterString = other.filterString;
			groupFilterString = other.groupFilterString;
			this.tables = new TableInfoList(other.tables.Count);
			foreach(TableInfo table in other.tables)
				this.tables.Add(new TableInfo(table));
			this.relations = new RelationInfoList(other.relations.Count);
			foreach(RelationInfo rel in other.relations)
				this.relations.Add(new RelationInfo(rel));
			this.sorting = new SortingInfoList(other.Sorting.Count);
			foreach(SortingInfo sortingInfo in other.Sorting)
				this.Sorting.Add(new SortingInfo(sortingInfo));
			grouping = new GroupingInfoList(other.grouping);
			top = other.top;
			skip = other.skip;
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false)
		]
		public TableInfoList Tables { get { return tables; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false)
		]
		public RelationInfoList Relations { get { return relations; } }
		[DefaultValue(null)]
#if !DXPORTABLE
		[Editor("DevExpress.DataAccess.UI.Native.Sql.FilterStringEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.TableQuery.FilterString")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("TableQueryFilterString")]
#endif
		[LocalizableCategory(Localization.DataAccessStringId.QueryPropertyGridTableSelectionCategoryName)]
		public string FilterString { get { return filterString; } set { filterString = value; } }
		[DefaultValue(null)]
#if !DXPORTABLE
		[Editor("DevExpress.DataAccess.UI.Native.Sql.GroupFilterStringEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.TableQuery.GroupFilterString")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("TableQueryGroupFilterString")]
#endif
		[LocalizableCategory(Localization.DataAccessStringId.QueryPropertyGridTableSelectionCategoryName)]
		public string GroupFilterString { get { return groupFilterString; } set { groupFilterString = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public SortingInfoList Sorting { get { return sorting; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public GroupingInfoList Grouping { get { return grouping; } }
		[DefaultValue(0)]
		[TypeConverter(typeof(PositiveIntegerTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.TableQuery.Top")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("TableQueryTop")]
#endif
		[LocalizableCategory(Localization.DataAccessStringId.QueryPropertyGridTableSelectionCategoryName)]
		public int Top { get { return top; } set { top = value; } }
		[DefaultValue(0)]
		[TypeConverter(typeof(SkipTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.TableQuery.Skip")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("TableQuerySkip")]
#endif
		[LocalizableCategory(Localization.DataAccessStringId.QueryPropertyGridTableSelectionCategoryName)]
		public int Skip { get { return skip; } set { skip = value; } }
		public TableInfo AddTable(string name) {
			TableInfo result = new TableInfo(name);
			this.tables.Add(result);
			return result;
		}
		public RelationInfo AddRelation(string parentTable, string nestedTable, string parentKeyColumn, string nestedKeyColumn) {
			RelationInfo result = new RelationInfo(parentTable, nestedTable, parentKeyColumn, nestedKeyColumn);
			this.relations.Add(result);
			return result;
		}
		public TableInfo FindTable(string actualName) {
			foreach(TableInfo table in this.tables)
				if(StringExtensions.CompareInvariantCultureIgnoreCase(table.ActualName, actualName) == 0)
					return table;
			return null;
		}
		public override void Validate() {
			ValidateCore(false, null);
		}
		public override void Validate(DBSchema schema) {
			Guard.ArgumentNotNull(schema, "schema");
			ValidateCore(true, schema);
		}
		public string GetSql(DBSchema schema) {
			return this.BuildSql(schema, this.DataSource.Connection.SqlGeneratorFormatter);
		}
		#region Validate core
		void ValidateCore(bool hasSchema, DBSchema schema) {
			HashSet<string> tableActualNames;
			Dictionary<string, DBTable> dbTables;
			if(this.tables.Count == 0)
				throw new NoTablesValidationException();
			ValidateTables(hasSchema, schema, out tableActualNames, out dbTables);
			ValidateRelations(hasSchema, tableActualNames, dbTables);
			ValidateFilterString(hasSchema, tableActualNames, dbTables);
			ValidateSkipN();
			ValidateSorting(hasSchema, dbTables);
			ValidateAggregation();
			ValidateGrouping();
			ValidateHaving(tableActualNames);
		}
		void ValidateTables(bool hasSchema, DBSchema schema, out HashSet<string> tableActualNames, out Dictionary<string, DBTable> dbTables) {
			tableActualNames = new HashSet<string>();
			dbTables = new Dictionary<string, DBTable>();
			bool anyColumnSelected = false;
			foreach(TableInfo table in this.tables) {
				if(object.ReferenceEquals(table, null)) {
					throw new TableNullValidationException();
				}
				string tableActualName;
				DBTable dbTable = null;
				ValidateTableName(table, tableActualNames, out tableActualName);
				if(hasSchema)
					ValidateTableWithSchema(schema, table, tableActualName, dbTables, out dbTable);
				anyColumnSelected |= table.SelectedColumns.Count != 0;
				HashSet<string> columnActualNames = new HashSet<string>();
				foreach(ColumnInfo column in table.SelectedColumns) {
					ValidateColumnName(column, columnActualNames, tableActualName);
					if(hasSchema)
						ValidateColumnWithSchema(dbTable, table, column);
				}
			}
			if(!anyColumnSelected)
				throw new NoColumnsValidationException();
		}
		void ValidateRelations(bool hasSchema, HashSet<string> tableActualNames, Dictionary<string, DBTable> dbTables) {
			HashSet<string> relRoots = new HashSet<string>(tableActualNames);
			foreach(RelationInfo rel in this.relations) {
				if(rel == null) {
					throw new RelationNullValidationException();
				}
				string parentTable;
				string nestedTable;
				ValidateRelationTables(rel, tableActualNames, relRoots, out parentTable, out nestedTable);
				if(rel.KeyColumns.Count == 0) {
					throw new NoRelationColumnsValidationException(rel);
				}
				if(hasSchema)
					ValidateRelationWithSchema(rel, dbTables, parentTable, nestedTable);
			}
			if(relRoots.Count == 0)
				throw new CircularRelationsValidationException();
			if(relRoots.Count > 1)
				throw new TablesNotRelatedValidationException(relRoots.ToArray());
		}
		void ValidateFilterString(bool hasSchema, HashSet<string> tableActualNames, Dictionary<string, DBTable> dbTables) {
			CriteriaOperator condition = CriteriaOperator.Parse(FilterString);
			FilterStringValidatorVisitor visitor = new FilterStringValidatorVisitor(tableActualNames, hasSchema ? dbTables : null);
			visitor.Process(condition);
		}
		void ValidateSorting(bool hasSchema, Dictionary<string, DBTable> dbTables) {
			Dictionary<string, HashSet<string>> allMetColumns = new Dictionary<string, HashSet<string>>();
			foreach(SortingInfo sortingInfo in Sorting) {
				HashSet<string> tableMetColumns;
				if(!allMetColumns.TryGetValue(sortingInfo.Table, out tableMetColumns))
					allMetColumns.Add(sortingInfo.Table, tableMetColumns = new HashSet<string>());
				else if(tableMetColumns.Contains(sortingInfo.Column))
					throw new SortingBySameColumnTwiceValidationException(sortingInfo.Table, sortingInfo.Column);
				tableMetColumns.Add(sortingInfo.Column);
				if(hasSchema) {
					DBTable dbTable;
					if(!dbTables.TryGetValue(sortingInfo.Table.ToLowerInvariant(), out dbTable))
						throw new TableNotInSchemaValidationException(sortingInfo.Table);
					if(
						!dbTable.Columns.Any(
							column => string.Equals(column.Name, sortingInfo.Column, StringComparison.Ordinal)))
						throw new ColumnNotInSchemaValidationException(sortingInfo.Table, sortingInfo.Column);
				}
			}
		}
		void ValidateAggregation() {
			bool? aggregated = null;
			if(grouping.Any())
				aggregated = true;
			foreach(TableInfo table in tables) {
				foreach(ColumnInfo column in table.SelectedColumns) {
					if(grouping.Any(info => GroupingInfo.EqualityComparer.Equals(info, table.Name, column.Name))) {
						if(aggregated == false)
							throw new PartialAggregationValidationException();
						aggregated = true;
						if(column.Aggregation != AggregationType.None)
							throw new GroupByAggregateColumnValidationException(table.Name, column.Name);
					}
					else {
						if(column.Aggregation == AggregationType.None) {
							if(aggregated == true)
								throw new PartialAggregationValidationException();
							aggregated = false;
						}
						else {
							if(aggregated == false)
								throw new PartialAggregationValidationException();
							aggregated = true;
							if(!column.HasAlias)
								throw new AggregationWithoutAliasValidationException();
						}
					}
				}
			}
		}
		void ValidateGrouping() {
			if(Grouping.Count == 0) {
				if(!string.IsNullOrEmpty(GroupFilterString))
					throw new HavingWithoutGroupByValidationException();
			}
			else {
				if(Tables.All(table => table.SelectedColumns.All(column => column.Aggregation == AggregationType.None)))
					throw new GroupByWithoutAggregateValidationException();
			}
		}
		void ValidateSkipN() {
			if(Skip != 0 && Sorting.Count == 0)
				throw new SkipWithoutSortingValidationException();
		}
		void ValidateHaving(HashSet<string> tableActualNames) {
			CriteriaOperator condition = CriteriaOperator.Parse(GroupFilterString);
			FilterStringValidatorVisitor visitor = new FilterStringValidatorVisitor(tableActualNames,
				Tables.ToDictionary(t => t.ActualName.ToLowerInvariant(),
					t => {
						DBTable dbTable = new DBTable(t.ActualName);
						foreach(ColumnInfo column in t.SelectedColumns)
							dbTable.AddColumn(new DBColumn(column.ActualName, false, null, 0, DBColumnType.Unknown));
						return dbTable;
					}));
			visitor.Process(condition);
		}
		#endregion
		internal override bool EqualsCore(SqlQuery obj) {
			TableQuery other = (TableQuery)obj;
			if(Top != other.Top || Skip != other.Skip)
				return false;
			if(!string.Equals(filterString, other.filterString, StringComparison.Ordinal) || !string.Equals(groupFilterString, other.groupFilterString, StringComparison.Ordinal))
				return false;
			int tableCount = this.Tables.Count;
			int relationCount = this.Relations.Count;
			if(other.Tables.Count != tableCount || other.Relations.Count != relationCount)
				return false;
			for(int i = 0; i < tableCount; i++)
				if(!TableInfo.EqualityComparer.Equals(this.Tables[i], other.Tables[i]))
					return false;
			for(int i = 0; i < relationCount; i++)
				if(!RelationInfo.EqualityComparer.Equals(this.Relations[i], other.Relations[i]))
					return false;
			if(!GroupingInfoList.EqualityComparer.Equals(this.Grouping, other.Grouping))
				return false;
			return true;
		}
		internal override SqlQuery Clone() { return new TableQuery(this); }
	}
}
