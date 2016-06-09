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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Data {
	public enum DataObjectCompareResult {
		Equal,
		EqualExceptForAliases,
		NotEqual
	};
	public class AliasChangeInfo {
		public string OldDataMember { get; set; }
		public string NewDataMember { get; set; }
		public AliasChangeInfo(string oldDataMember, string newDataMember) {
			OldDataMember = oldDataMember;
			NewDataMember = newDataMember;
		}
	}
	public class SchemaLoadingExceptionInfo {
		public SchemaLoadingExceptionInfo(string tableName, string columnName) {
			TableName = tableName;
			ColumnName = columnName;
		}
		public string TableName { get; set; }
		public string ColumnName { get; set; }
	}
	public class SchemaLoadingExceptionsInfo : List<SchemaLoadingExceptionInfo> {
		public override string ToString() {
			StringBuilder exceptionStringBuilder = new StringBuilder();
			foreach(SchemaLoadingExceptionInfo info in this.Distinct())
				exceptionStringBuilder.Append(string.Format(", {0}.{1}", info.TableName, info.ColumnName));
			exceptionStringBuilder.Remove(0, 1);
			return exceptionStringBuilder.ToString();
		}
	}
	public class DataSelection : List<DataTable> {
		static void CollectReferencedTables(DataTable table, List<DataTable> referencedTables) {
			referencedTables.Add(table);
			if(table.References.ActionType == ActionType.MasterDetailRelation)
				return;
			table.References.ForEach(r => CollectReferencedTables(r.ParentDataTable, referencedTables));
		}
		static void SetDefaultAliasesForCustomDBTable(DataTable datatable, DBTable table) {
			if(datatable != null && table != null) {
				foreach(DBColumn column in table.Columns) {
#pragma warning disable 612, 618
					DBColumnWithAlias customColumn = column as DBColumnWithAlias;
					if(customColumn != null)
						datatable[column.Name].Alias = customColumn.Alias;
#pragma warning restore 612, 618
				}
			}
		}
		internal const string XmlSelection = "Selection";
		internal const string XmlFilters = "Filters";
		readonly FilterInfoCollection filters;
#pragma warning disable 612, 618
		readonly DataProviderBase dataProvider;
#pragma warning restore 612, 618
		readonly Dictionary<QueryOperand, DataColumn> operands = new Dictionary<QueryOperand, DataColumn>();
		readonly List<DataColumn> selectedColumns = new List<DataColumn>();
		public bool IsEmpty { get { return Count == 0; } }
#pragma warning disable 612, 618
		public DataProviderBase DataProvider { get { return this.dataProvider; } }
#pragma warning restore 612, 618
		public Dictionary<QueryOperand, DataColumn> Operands { get { return this.operands; } }
		public List<DataColumn> SelectedColumns { get { return this.selectedColumns; } }
		public FilterInfoCollection Filters { get { return this.filters; } }
		IAliasFormatter AliasFormatter
		{
			get
			{
				if(DataProvider == null)
					return null;
				if(DataProvider.DataConnection == null)
					return null;
				return DataProvider.DataConnection.SqlGeneratorFormatter as IAliasFormatter;
			}
		}
		int MaxTableAliasLength { get { return AliasFormatter != null ? AliasFormatter.MaxTableAliasLength : 0; } }
		int MaxColumnAliasLength { get { return AliasFormatter != null ? AliasFormatter.MaxColumnAliasLength : 0; } }
#pragma warning disable 612, 618
		public DataSelection(DataProviderBase dataProvider) {
#pragma warning restore 612, 618
			Guard.ArgumentNotNull(dataProvider, "dataProvider");
			this.dataProvider = dataProvider;
			this.filters = new FilterInfoCollection(this.dataProvider);
		}
#pragma warning disable 612, 618
		public DataSelection(DataProviderBase dataProvider, XElement element)
#pragma warning restore 612, 618
			: this(dataProvider) {
			foreach(XElement tableElement in element.Elements(DataTable.XmlTable))
				Add(new DataTable(this, tableElement));
			foreach(XElement filtersElement in element.Elements(XmlFilters))
				foreach(XElement filterElement in filtersElement.Elements(FilterInfoPropertyNames.XmlFilter)) {
					string groupName = XmlHelperBase.GetAttributeValue(filterElement, FilterInfoPropertyNames.XmlTableName);
					string filterString = XmlHelperBase.GetAttributeValue(filterElement, FilterInfoPropertyNames.XmlFilterString);
					if(string.IsNullOrEmpty(groupName)) {
						var groups = GetTableGroups();
						if(groups.Count > 0)
							groupName = groups.Keys.First();
					}
					this.filters.Add(groupName, filterString);
				}
		}
		public IEnumerable<DataTable> GetTablesForRelation(string currentTable) {
			return this.DataProvider.GetTablesForRelation(currentTable);
		}
		public DataObjectCompareResult CompareWith(DataSelection dataSelection) {
			if(Count != dataSelection.Count)
				return DataObjectCompareResult.NotEqual;
			DataObjectCompareResult totalResult = DataObjectCompareResult.Equal;
			for(int i = 0; i < dataSelection.Count; i++) {
				DataTable table1 = this[i];
				DataTable table2 = dataSelection[i];
				DataObjectCompareResult result = table1.CompareWith(table2);
				if(result == DataObjectCompareResult.NotEqual)
					return DataObjectCompareResult.NotEqual;
				if(result == DataObjectCompareResult.EqualExceptForAliases)
					totalResult = DataObjectCompareResult.EqualExceptForAliases;
			}
			return totalResult;
		}
		public List<AliasChangeInfo> GetChangeList(DataSelection dataSelection) {
			List<AliasChangeInfo> aliasChangeInfo = new List<AliasChangeInfo>();
			if(Count == dataSelection.Count)
				for(int i = 0; i < dataSelection.Count; i++) {
					DataTable table1 = this[i];
					DataTable table2 = dataSelection[i];
					table1.FillChangeList(table2, aliasChangeInfo);
				}
			return aliasChangeInfo;
		}
		public IEnumerable<DataReference> GetReferences(DBTable table) {
			List<DataReference> references = new List<DataReference>();
			references.AddRange(GetMasterDetailReferences(table));
			references.AddRange(GetJoinReferences(table));
			return references;
		}
		public IEnumerable<DataReference> GetMasterDetailReferences(DBTable table) {
			List<DataReference> references = new List<DataReference>();
			foreach(DataTable dataTable in this) {
				foreach(DBForeignKey foreignKey in table.ForeignKeys)
					if(foreignKey.PrimaryKeyTable == dataTable.Table.Name)
						references.AddRange(CreateReferences(foreignKey.Columns, table, foreignKey.PrimaryKeyTableKeyColumns, dataTable));
			}
			return references;
		}
		public IEnumerable<DataReference> GetJoinReferences(DBTable table) {
			List<DataReference> references = new List<DataReference>();
			foreach(DataTable dataTable in this) {
				foreach(DBForeignKey foreignKey in dataTable.Table.ForeignKeys)
					if(foreignKey.PrimaryKeyTable == table.Name)
						references.AddRange(CreateReferences(foreignKey.PrimaryKeyTableKeyColumns, table, foreignKey.Columns, dataTable));
			}
			return references;
		}
		public Type GetActionType(DBTable table) {
			if(GetMasterDetailReferences(table).Any()) {
				return DataProvider.ActionType;
			}
			return typeof(JoinType);
		}
		public string GetUniqueName(DBTable table) {
			int index = 0;
#pragma warning disable 612, 618
			DBTableWithAlias tableWithAlias = table as DBTableWithAlias;
			string tableName = tableWithAlias == null || string.IsNullOrEmpty(tableWithAlias.Alias) ? table.Name : tableWithAlias.Alias;
#pragma warning restore 612, 618
			for(;;) {
				string uniqueName = index != 0 ? String.Format("{0}_{1}", tableName, index) : tableName;
				if(MaxTableAliasLength > 0 && uniqueName.Length > MaxTableAliasLength) {
					tableName = tableName.Substring(0, MaxTableAliasLength - 1 - index.ToString().Length);
					index = 1;
					continue;
				}
				if(!Exists(t => t.UniqueName == uniqueName))
					return uniqueName;
				index++;
			}
		}
		public bool CanSelectTable(DBTable table) {
			Guard.ArgumentNotNull(table, "table");
			return IsEmpty || GetReferences(table).Any();
		}
		public DataTable AddTableByReference(DataColumn dataColumn) {
			Guard.ArgumentNotNull(dataColumn, "dataColumn");
			DataTable dataTable;
			IEnumerable<DataReference> references = GetReferences(dataColumn);
			dataTable = new DataTable(this, dataColumn.ForeignKeyTable, GetUniqueName(dataColumn.ForeignKeyTable), references);
			SetDefaultAliasesForCustomDBTable(dataTable, dataColumn.ForeignKeyTable);
			Add(dataTable);
			return dataTable;
		}
		public DataTable AddTable(DBTable table, DataReferences references) {
			Guard.ArgumentNotNull(table, "table");
			DataTable dataTable = IsEmpty ? new DataTable(this, table, GetUniqueName(table)) : new DataTable(this, table, GetUniqueName(table), references);
			SetDefaultAliasesForCustomDBTable(dataTable, table);
			Add(dataTable);
			return dataTable;
		}
		public DataTable AddTable(DBTable table) {
			Guard.ArgumentNotNull(table, "table");
			DataTable dataTable;
			if(IsEmpty)
				dataTable = new DataTable(this, table, GetUniqueName(table));
			else {
				IEnumerable<DataReference> references = GetReferences(table);
				if(!references.Any())
					throw new ArgumentException(string.Format("'{0}' table can't be selected, because no references have been found", table.Name));
				dataTable = new DataTable(this, table, GetUniqueName(table), references);
			}
			SetDefaultAliasesForCustomDBTable(dataTable, table);
			Add(dataTable);
			return dataTable;
		}
		public List<DataTable> GetDataTablesToRemove(DataTable dataTableToRemove) {
			Guard.ArgumentNotNull(dataTableToRemove, "dataTable");
			if(Count == 1)
				return new List<DataTable>() {dataTableToRemove};
			SortedDictionary<int, List<DataTable>> reachableTables = new SortedDictionary<int, List<DataTable>>();
			if(dataTableToRemove.References != null)
				foreach(DataReference reference in dataTableToRemove.References)
					CalculateReachableTablesFrom(reference.ParentDataTable, dataTableToRemove, reachableTables);
			foreach(DataTable table in this)
				foreach(DataReference reference in table.References)
					if(reference.ParentDataTable == dataTableToRemove)
						CalculateReachableTablesFrom(table, dataTableToRemove, reachableTables);
			foreach(List<DataTable> remainingTables in reachableTables.Values) {
				List<DataTable> removedTables = new List<DataTable>();
				foreach(DataTable table in this)
					if(!remainingTables.Contains(table))
						removedTables.Add(table);
				return removedTables;
			}
			return new List<DataTable>() {dataTableToRemove};
		}
		public void RemoveDataTables(IList<DataTable> dataTables) {
			foreach(DataTable dataTable in dataTables) {
				Remove(dataTable);
				foreach(DataColumn dataColumn in dataTable.SelectedColumns)
					OnColumnDeselected(dataColumn);
			}
			foreach(DataTable dataTable in this) {
				int i = 0;
				while(i < dataTable.References.Count) {
					DataReference reference = dataTable.References[i];
					if(dataTables.Contains(reference.ParentDataTable))
						dataTable.References.Remove(reference);
					else
						i++;
				}
			}
		}
		public SelectStatement CreateSelectStatement() {
			if(!IsEmpty) {
				string name = GetFilterTableName(this[0].Alias);
				return GetTableGroupSelectStatement(name);
			}
			return null;
		}
		public IEnumerable<DataColumn> GetTableGroupsDataColumns(string tableGroupName) {
			Dictionary<string, IEnumerable<DataTable>> dataTablesList = GetTableGroups();
			IEnumerable<DataTable> dataTables = dataTablesList[tableGroupName];
			List<DataColumn> dataColumns = new List<DataColumn>();
#pragma warning disable 612, 618
			foreach(DataTable table in dataTables) {
				SelectedColumns.ForEach(delegate(DataColumn c) {
					if(c.DataTable == table)
						dataColumns.Add(c);
				});
			}
#pragma warning restore 612, 618
			return dataColumns;
		}
		public SelectStatement GetTableGroupSelectStatement(string tableGroupName) {
			Dictionary<string, IEnumerable<DataTable>> dataTablesList = GetTableGroups();
			if(dataTablesList != null && dataTablesList.Keys.Count > 0 && tableGroupName != null && dataTablesList.ContainsKey(tableGroupName)) {
				this.operands.Clear();
				IEnumerable<DataTable> tables = dataTablesList[tableGroupName];
				IEnumerable<DataColumn> dataColumns = GetTableGroupsDataColumns(tableGroupName);
				SelectStatement selectStatement = tables.Count() > 1 ? CreateAndPrepareSelectStatement(tables, dataColumns) : CreateSelectStatementCore(tables.First(), dataColumns);
				FilterInfo filter = this.filters[tableGroupName];
				if(filter != null)
					selectStatement.Condition = FilterHelper.GetFilter(filter.FilterString, DataProvider.Parameters);
				return selectStatement;
			}
			return null;
		}
		[SuppressMessage("Microsoft.Design", "CA1006")]
		public Dictionary<string, IEnumerable<DataTable>> GetTableGroups() {
#pragma warning disable 612, 618
			Dictionary<string, IEnumerable<DataTable>> dataTablesList = new Dictionary<string, IEnumerable<DataTable>>();
			foreach(DataTable table in this) {
				if(table.References.ActionType == ActionType.MasterDetailRelation) {
					dataTablesList.Add(table.Alias, new List<DataTable>() {table});
				} else {
					List<DataTable> referencedTables = new List<DataTable>();
					CollectReferencedTables(table, referencedTables);
					List<DataTable> result = null;
					foreach(List<DataTable> tables in dataTablesList.Values) {
						if(tables.Intersect(referencedTables).Any()) {
							result = tables;
							break;
						}
					}
					if(result != null) {
						referencedTables.ForEach(delegate(DataTable dataTable) {
							if(!result.Contains(dataTable)) {
								result.Add(dataTable);
							}
						});
					} else {
						dataTablesList.Add(referencedTables[0].Alias, referencedTables);
					}
				}
			}
			Dictionary<string, IEnumerable<DataTable>> resultedTableGroups = new Dictionary<string, IEnumerable<DataTable>>();
			foreach(var group in dataTablesList) {
				bool emptyGroup = false;
				foreach(DataTable dataTable in group.Value) {
					if(dataTable.HasSelectedColumns) {
						emptyGroup = false;
						break;
					}
				}
				if(!emptyGroup)
					resultedTableGroups.Add(group.Key, group.Value);
			}
#pragma warning restore 612, 618
			return resultedTableGroups;
		}
		public string GetSqlQuery(string tableName) {
			string name = GetFilterTableName(tableName);
			SelectStatement selectStatement = GetTableGroupSelectStatement(name);
			return this.DataProvider.CreateAutoSql(selectStatement);
		}
		public string GetFilterTableName(string tableName) {
			return (string)GetResult(tableName, i => i.Key);
		}
		public IEnumerable<DataTable> GetTables(string tableName) {
			IEnumerable<DataTable> result = GetResult(tableName, i => i.Value) as IEnumerable<DataTable>;
			if(result != null)
				return result;
			return this;
		}
		public void OnColumnSelected(DataColumn dataColumn) {
			if(this.selectedColumns.Contains(dataColumn))
				throw new DataSourceWizardException("The specified column has already been selected");
			Dictionary<string, IEnumerable<DataTable>> groups = GetTableGroups();
			if(groups.Count != 0) {
				string groupName = groups.FirstOrDefault(g => g.Value.Any(t => t == dataColumn.DataTable)).Key;
				if(groupName != null) {
					IEnumerable<DataColumn> groupColumns = GetTableGroupsDataColumns(groupName);
					if(groupColumns.Any(_dataColumn => _dataColumn.ActualName == dataColumn.ActualName)) {
						string actualName = string.Format("{0}_{1}", dataColumn.DataTable.Alias, dataColumn.ActualName);
						int index = 0;
						for(;;) {
							string uniqueName = index != 0 ? string.Format("{0}_{1}", actualName, index) : actualName;
							index++;
							if(MaxColumnAliasLength > 0 && uniqueName.Length > MaxColumnAliasLength) {
								actualName = actualName.Substring(0, actualName.Length - 1);
								index = 1;
								continue;
							}
							if(!this.selectedColumns.Any(_dataColumn => _dataColumn.ActualName == uniqueName)) {
								dataColumn.SetAlias(uniqueName, false);
								break;
							}
						}
					}
				}
			}
			IEnumerable<DataColumn> orphans = this.selectedColumns.Where(col => !Contains(col.DataTable)).ToList();
			this.selectedColumns.Clear();
			foreach(DataColumn column in this.SelectMany(table => table.Columns.Where(column => column.Selected)))
				this.selectedColumns.Add(column);
			this.selectedColumns.AddRange(orphans);
			if(!Contains(dataColumn.DataTable))
				this.selectedColumns.Add(dataColumn);
		}
		public void OnColumnDeselected(DataColumn dataColumn) {
			if(!this.selectedColumns.Contains(dataColumn))
				throw new DataSourceWizardException("The specified column is not selected");
			this.selectedColumns.Remove(dataColumn);
		}
		public XElement SaveToXml() {
			XElement element = new XElement(XmlSelection);
			foreach(DataTable dataTable in this)
				element.Add(dataTable.SaveToXml());
			if(Filters.Count > 0) {
				XElement filtersElement = new XElement(XmlFilters);
				element.Add(filtersElement);
				foreach(FilterInfo filter in Filters)
					filtersElement.Add(filter.SaveToXml());
			}
			return element;
		}
		public void OnTableChanged() {
			DataProvider.OnDataSelectionChanged();
		}
		public void OnColumnAliasChanged(DataColumn dataColumn) {
			this.dataProvider.OnColumnOrTableAliasChanged();
		}
		public void OnTableAliasChanged(DataTable dataTable) {
			this.dataProvider.OnColumnOrTableAliasChanged();
		}
		public DataSelection Clone() {
			DataSelection dataSelection = new DataSelection(this.dataProvider);
			foreach(DataTable dataTable in this) {
				List<DataReference> references = new List<DataReference>();
				foreach(DataReference reference in dataTable.References) {
					references.Add(new DataReference(dataSelection, reference.KeyColumn, reference.KeyDBTable, reference.OperatorType, reference.ParentKeyColumn, reference.ParentDataTable.Alias));
				}
				DataTable newDataTable = new DataTable(dataSelection, dataTable.Table, dataTable.UniqueName, references);
				newDataTable.References.SetActionType(dataTable.References.ActionType, false);
				newDataTable.SetAlias(dataTable.Alias, false);
				foreach(DataColumn column in dataTable.Columns) {
					if(column.Column != null) {
						newDataTable[column.Column.Name].SetSelected(column.Selected, false);
						newDataTable[column.Column.Name].SetAlias(column.Alias, false);
					}
				}
				dataSelection.Add(newDataTable);
			}
			dataSelection.filters.AddRange(this.filters.Select(fi => fi.Clone()).ToArray());
			return dataSelection;
		}
		public bool HasJoinOnColumn(DataColumn dataColumn, DBTable table) {
			foreach(DataTable dataTable in this)
				if(dataTable.Table.Equals(table))
					foreach(DataReference dataReference in dataTable.References)
						if(dataReference.ParentKeyColumn == dataColumn.Column.Name && dataReference.ParentDataTable == dataColumn.DataTable)
							return true;
			foreach(DataReference dataReference in dataColumn.DataTable.References)
				if(dataReference.KeyColumn == dataColumn.Column.Name && dataReference.ParentDataTable.Table.Equals(table))
					return true;
			return false;
		}
		public bool IsTableAliasExists(string alias) {
			return this.Any(t => t.Alias == alias);
		}
		internal void LoadDataObjects() {
			SchemaLoadingExceptionsInfo exceptions = new SchemaLoadingExceptionsInfo();
			foreach(DataTable table in this)
				exceptions.AddRange(table.LoadDataObjects());
			if(exceptions.Count > 0) {
				string exceptionText = string.Join("\n", exceptions.GroupBy(e => e.TableName).Select(p => string.Format("The \"{0}\" table does not contain the specified columns: \"{1}\".", p.Key, string.Join("\", \"", p.Select(c => c.ColumnName)))));
				throw new InvalidOperationException(exceptionText);
			}
		}
		void CalculateReachableTablesFrom(DataTable dataTable, DataTable dataTableToRemove, SortedDictionary<int, List<DataTable>> reachableTables) {
			List<DataTable> list = new List<DataTable>();
			list.Add(dataTable);
			AddReferencedTablesToList(dataTable, dataTableToRemove, list);
			int minIndex = int.MaxValue;
			foreach(DataTable table in list) {
				int index = IndexOf(table);
				if(index < minIndex)
					minIndex = index;
			}
			if(!reachableTables.ContainsKey(minIndex))
				reachableTables.Add(minIndex, list);
			else
				reachableTables[minIndex].AddRange(list);
		}
		void AddReferencedTablesToList(DataTable dataTable, DataTable dataTableToRemove, List<DataTable> list) {
			foreach(DataReference reference in dataTable.References)
				if(!list.Contains(reference.ParentDataTable) && reference.ParentDataTable != dataTableToRemove) {
					list.Add(reference.ParentDataTable);
					AddReferencedTablesToList(reference.ParentDataTable, dataTableToRemove, list);
				}
			foreach(DataTable table in this)
				if(!list.Contains(table) && table != dataTableToRemove && table.References != null)
					foreach(DataReference reference in table.References)
						if(reference.ParentDataTable == dataTable) {
							list.Add(table);
							AddReferencedTablesToList(table, dataTableToRemove, list);
						}
		}
		object GetResult(string tableName, Function<object, KeyValuePair<string, IEnumerable<DataTable>>> func) {
			Dictionary<string, IEnumerable<DataTable>> tableGroups = GetTableGroups();
			foreach(KeyValuePair<string, IEnumerable<DataTable>> tables in tableGroups) {
				foreach(DataTable item in tables.Value) {
					if(item.Alias == tableName)
						return func(tables);
				}
			}
			return null;
		}
		IEnumerable<DataReference> CreateReferences(StringCollection keys, DBTable dbTable, StringCollection parentKeys, DataTable parentDataTable) {
			if(keys.Count != parentKeys.Count)
				throw new DataSourceWizardException("The keys.Count and parentKeys.Count properties have different values.");
			List<DataReference> references = new List<DataReference>();
			for(int i = 0; i < keys.Count; i++)
				references.Add(new DataReference(this, keys[i], dbTable, parentKeys[i], parentDataTable));
			return references;
		}
		IEnumerable<DataReference> GetReferences(DataColumn dataColumn) {
			foreach(DBForeignKey foreignKey in dataColumn.DataTable.Table.ForeignKeys)
				if(foreignKey.Columns.Contains(dataColumn.Column.Name)) {
					DBTable dbTable = DataProvider.FindTable(table => table.Name == foreignKey.PrimaryKeyTable);
					return CreateReferences(foreignKey.PrimaryKeyTableKeyColumns, dbTable, foreignKey.Columns, dataColumn.DataTable);
				}
			throw new DataSourceWizardException("The column contains an inconsistent foreign key.");
		}
		SelectStatement CreateAndPrepareSelectStatement(IEnumerable<DataTable> tables, IEnumerable<DataColumn> dataColumns) {
			DataTable first = tables.First();
			SelectStatement selectStatement = CreateSelectStatementCore(first, dataColumns);
			foreach(DataTable dataTable in tables)
				dataTable.PrepareSelectStatement(selectStatement);
			return selectStatement;
		}
		SelectStatement CreateSelectStatementCore(DataTable table, IEnumerable<DataColumn> dataColumns) {
			SelectStatement selectStatement = new SelectStatement(table.Table, table.Alias);
			foreach(DataColumn dataColumn in dataColumns) {
				string alias = dataColumn.DataTable.Alias;
				DBColumn column = dataColumn.Column;
				if(column != null) {
					QueryOperand operand = new QueryOperand(column.Name, alias, column.ColumnType);
					Operands.Add(operand, dataColumn);
					selectStatement.Operands.Add(operand);
				}
			}
			return selectStatement;
		}
	}
}
