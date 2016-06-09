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
using System.Data;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.XtraReports.Data {
	public class DataProvider {
		readonly IDataStore dataStore;
		readonly IDataStoreSchemaExplorer schemaExplorer;
		public static string DefaultTableAlias {
			get { return "TableAlias"; }
		}
		public IDataStoreSchemaExplorer SchemaExplorer {
			get { return schemaExplorer; }
		}
		public IDataStore DataStore {
			get { return dataStore; }
		}
		public DataProvider(IDataStoreSchemaExplorer schemaExplorer, IDataStore dataStore) {
			Guard.ArgumentNotNull(schemaExplorer, "schemaExplorer");
			Guard.ArgumentNotNull(dataStore, "dataStore");
			this.schemaExplorer = schemaExplorer;
			this.dataStore = dataStore;
		}
		public void Fill(DataSet dataSet) {
			Fill(dataSet, null);
		}
		public void Fill(DataSet dataSet, FilteringOperatorsByTableDictionary filteringOperatorByTable) {
			Guard.ArgumentNotNull(dataSet, "dataSet");
			CollectionNullOrNotEmpty(filteringOperatorByTable, "filteringOperatorByTable");
			FillDataSet(dataSet, filteringOperatorByTable);
		}
		public void Fill(DataTable dataTable) {
			Fill(dataTable, null);
		}
		public void Fill(DataTable dataTable, IList<CriteriaOperator> filteringOperators) {
			Guard.ArgumentNotNull(dataTable, "dataTable");
			CollectionNullOrNotEmpty(filteringOperators, "filteringOperators");
			DBTable[] dbTables = SchemaExplorer.GetStorageTables(dataTable.TableName);
			System.Diagnostics.Debug.Assert(dbTables.Length == 1);
			DBTable dbTable = dbTables.First();
			TurnOffEnforceConstraintsActionContainer(dataTable.DataSet, () => FillDataTable(dataTable, dbTable, filteringOperators));
		}
		static void CollectionNullOrNotEmpty<TItem>(ICollection<TItem> collection, string argumentName) {
			if(collection != null && collection.Count == 0)
				throw new ArgumentException(string.Format("{0} should be equal to null or not empty", argumentName));
		}
		void FillDataSet(DataSet dataSet, FilteringOperatorsByTableDictionary filteringOperatorByTable) {
			string[] dataTableNames = dataSet.Tables.OfType<DataTable>().Select(x => x.TableName).ToArray();
			DBTable[] dbTables = SchemaExplorer.GetStorageTables(dataTableNames);
			System.Diagnostics.Debug.Assert(dataSet.Tables.Count == dbTables.Length);
			TurnOffEnforceConstraintsActionContainer(dataSet, () => {
				foreach(DataTable dataTable in dataSet.Tables) {
					DBTable dbTable = dbTables.First(x => x.Name == dataTable.TableName);
					System.Diagnostics.Debug.Assert(dbTable != null);
					IList<CriteriaOperator> filteringOperators = (filteringOperatorByTable != null && filteringOperatorByTable.ContainsKey(dataTable.TableName)) 
						? filteringOperatorByTable[dataTable.TableName] 
						: null;
					CollectionNullOrNotEmpty(filteringOperators, "filteringOperators");
					FillDataTable(dataTable, dbTable, filteringOperators);
				}
			});
		}
		protected void FillDataTable(DataTable dataTable, DBTable dbTable, IList<CriteriaOperator> filteringOperators) {
			IEnumerable<string> columnNames = dataTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName);
			System.Diagnostics.Debug.Assert(columnNames.Count() != 0);
			SelectedData selectedData = SelectDataFromTable(dbTable, columnNames, filteringOperators);
			SelectStatementResult selectResult = selectedData.ResultSet.First();
			foreach(var dbRow in selectResult.Rows)
				dataTable.Rows.Add(dbRow.ConvertToDataRow(dataTable));
		}
		protected static void TurnOffEnforceConstraintsActionContainer(DataSet dataSet, Action fillAction) {
			bool enforceConstraintsOldValue = dataSet.EnforceConstraints;
			dataSet.EnforceConstraints = false;
			fillAction();
			dataSet.EnforceConstraints = enforceConstraintsOldValue;
		}
		SelectedData SelectDataFromTable(DBTable dbTable, IEnumerable<string> columnNames, IList<CriteriaOperator> filteringOperators) {
			IEnumerable<DBColumn> dbColumns = dbTable.Columns.Where(x => columnNames.Contains(x.Name));
			SelectStatement selectStatement = CreateSelectStatement(dbTable, dbColumns, filteringOperators);
			return DataStore.SelectData(selectStatement);
		}
		static SelectStatement CreateSelectStatement(DBTable dbTable, IEnumerable<DBColumn> dbColumns, IList<CriteriaOperator> filteringOperators) {
			SelectStatement selectStatement = new SelectStatement(dbTable, DefaultTableAlias);
			foreach(var dbColumn in dbColumns)
				selectStatement.Operands.Add(new QueryOperand(dbColumn, DefaultTableAlias));
			if(filteringOperators != null && filteringOperators.Count != 0)
				selectStatement.Condition = GroupOperator.And(filteringOperators);
			return selectStatement;
		}
	}
}
