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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using System.Collections.Specialized;
using System.Collections;
namespace DevExpress.XtraReports.Data {
	public class DataSchemaProvider {
		public static string IsViewExtendedPropertyName {
			get { return "IsView"; }
		}
		readonly IDataStoreSchemaExplorer schemaExplorer;
		public IDataStoreSchemaExplorer SchemaExplorer {
			get { return schemaExplorer; }
		}
		string[] tableNames;
		string[] viewNames;
		string[] TableNames {
			get {
				if(tableNames == null)
					tableNames = SchemaExplorer.GetStorageTablesList(false);
				return tableNames;
			}
		}
		string[] ViewNames {
			get {
				if(viewNames == null) {
					string[] tableAndViewNames = SchemaExplorer.GetStorageTablesList(true);
					viewNames = tableAndViewNames.Except(TableNames).ToArray();
				}
				return viewNames;
			}
		}
		public DataSchemaProvider(IDataStoreSchemaExplorer schemaExplorer) {
			Guard.ArgumentNotNull(schemaExplorer, "schemaExplorer");
			this.schemaExplorer = schemaExplorer;
		}
		public DataSchema FillDataSchema() {
			return FillDataSchema(null);
		}
		public DataSchema FillDataSchema(ColumnsByTableDictionary fillingColumnsByTable) {
			CollectionNullOrNotEmpty(fillingColumnsByTable, "fillingColumnsByTable");
			string tablesDataSchema = FillTablesDataSchema(fillingColumnsByTable);
			string viewsDataSchema = FillViewsDataSchema(fillingColumnsByTable);
			return new DataSchema(tablesDataSchema, viewsDataSchema);
		}
		public string FillTablesDataSchema() {
			return FillTablesDataSchema(null);
		}
		public string FillTablesDataSchema(ColumnsByTableDictionary fillingColumnsByTable) {
			CollectionNullOrNotEmpty(fillingColumnsByTable, "fillingColumnsByTable");
			using(DataSet tables = CreateTablesDataSet(fillingColumnsByTable)) {
				return GetXmlSchema(tables);
			}
		}
		public string FillViewsDataSchema() {
			return FillViewsDataSchema(null);
		}
		public string FillViewsDataSchema(ColumnsByTableDictionary fillingColumnsByTable) {
			CollectionNullOrNotEmpty(fillingColumnsByTable, "fillingColumnsByTable");
			using(DataSet views = CreateViewsDataSet(fillingColumnsByTable)) {
				return GetXmlSchema(views);
			}
		}
		public string FillTablesViewsMergedDataSchema() {
			return FillTablesViewsMergedDataSchema(null);
		}
		public string FillTablesViewsMergedDataSchema(ColumnsByTableDictionary fillingColumnsByTable) {
			CollectionNullOrNotEmpty(fillingColumnsByTable, "fillingColumnsByTable");
			using(DataSet tables = CreateTablesDataSet(fillingColumnsByTable)) {
				using(DataSet views = CreateViewsDataSet(fillingColumnsByTable)) {
					tables.Merge(views);
					return GetXmlSchema(tables);
				}
			}
		}
		static void CollectionNullOrNotEmpty<TItem>(ICollection<TItem> collection, string argumentName) {
			if(collection != null && collection.Count == 0)
				throw new ArgumentException(string.Format("{0} should be equal to null or not empty", argumentName));
		}
		protected DataSet CreateTablesDataSet(ColumnsByTableDictionary fillingColumnsByTable) {
			return CreateDataSet(TableNames, fillingColumnsByTable);
		}
		protected DataSet CreateViewsDataSet(ColumnsByTableDictionary fillingColumnsByTable) {
			var ds = CreateDataSet(ViewNames, fillingColumnsByTable);
			foreach(DataTable table in ds.Tables) {
				table.ExtendedProperties.Add(IsViewExtendedPropertyName, true);
			}
			return ds;
		}
		public string[] GetStorageViewsList() {
			return ViewNames;
		}
		protected virtual DataSet CreateDataSet(string[] tableNames, ColumnsByTableDictionary fillingColumnsByTable) {
			string[] selectedTableNames = fillingColumnsByTable == null
				? tableNames
				: tableNames.Where(x => fillingColumnsByTable.ContainsKey(x)).ToArray();
			DBTable[] selectedDbTables = SchemaExplorer.GetStorageTables(selectedTableNames);
			DataSet dataSet = new DataSet() { Locale = CultureInfo.InvariantCulture };
			if(selectedDbTables.Length != 0) {
				foreach(DBTable dbTable in selectedDbTables) {
					IList<string> fillingColumns = (fillingColumnsByTable == null) ? null : fillingColumnsByTable[dbTable.Name];
					CollectionNullOrNotEmpty(fillingColumns, "The list of filled columns");
					DataTable dataTable = CreateDataTable(dbTable, fillingColumns);
					dataSet.Tables.Add(dataTable);
				}
				ExtractDataRelations(dataSet, selectedDbTables);
			}
			return dataSet;
		}
		static DataTable CreateDataTable(DBTable dbTable, IList<string> fillingColumns) {
			return dbTable.ConvertToDataTable(fillingColumns);
		}
		static void ExtractDataRelations(DataSet dataSet, IList<DBTable> dbTables) {
			IList<DBRelation> dbRelations = DBRelationExtractor.ExtractDbRelationsGroupedByDbTable(dbTables);
			if(dbRelations.Count == 0)
				return;
			List<DBRelation> tempRelations = new List<DBRelation>();
			string tempTableName = dbRelations[0].ParentTable;
			foreach(DBRelation dbRelation in dbRelations) {
				if(!dataSet.Tables.Contains(dbRelation.ParentTable) || !dataSet.Tables.Contains(dbRelation.ChildTable))
					continue;
				if(!string.Equals(dbRelation.ParentTable, tempTableName)) {
					tempRelations = new List<DBRelation>();
					tempTableName = dbRelation.ParentTable;
				}
				DataTable parentDataTable = dataSet.Tables[dbRelation.ParentTable];
				DataTable childDataTable = dataSet.Tables[dbRelation.ChildTable];
				int relationDataColumnsCount = dbRelation.ParentColumns.Count;
				IEnumerable<DataColumn> parentDataColumns = parentDataTable.Columns.OfType<DataColumn>();
				IEnumerable<DataColumn> childDataColumns = childDataTable.Columns.OfType<DataColumn>();
				List<DataColumn> parentRelationDataColumns = new List<DataColumn>(relationDataColumnsCount);
				List<DataColumn> childrenRelationDataColumns = new List<DataColumn>(relationDataColumnsCount);
				for(int i = 0; i < relationDataColumnsCount; i++) {
					AddColumn(parentRelationDataColumns, parentDataColumns, dbRelation.ParentColumns, i);
					AddColumn(childrenRelationDataColumns, childDataColumns, dbRelation.ChildColumns, i);
				}
				if(parentRelationDataColumns.Count != relationDataColumnsCount || childrenRelationDataColumns.Count != relationDataColumnsCount)
					continue;
				if(parentDataTable.TableName.Equals(childDataTable.TableName) && ColumnCollectionsAreEqual(parentRelationDataColumns, childrenRelationDataColumns))
					continue; 
				string dataRelationName = GetDataRelationName(parentDataTable, childDataTable);
				DataRelation childDataRelation = new DataRelation(dataRelationName, parentRelationDataColumns.ToArray(), childrenRelationDataColumns.ToArray(), false);
				if(IsParentTableContaintsChildRelation(tempRelations, dbRelation))
					continue;
				parentDataTable.ChildRelations.Add(childDataRelation);
				tempRelations.Add(dbRelation);
			}
		}
		static void AddColumn(List<DataColumn> destination,IEnumerable<DataColumn> source, StringCollection columnCollection, int index) {
			DataColumn dataColumn = source.Where(x => x.ColumnName == columnCollection[index]).SingleOrDefault();
			if(dataColumn != null)
				destination.Add(dataColumn);
		}
		static bool ColumnCollectionsAreEqual(ICollection<DataColumn> collection1, ICollection<DataColumn> collection2) {
			if(collection1.Count != collection2.Count)
				return false; 
			foreach(DataColumn item in collection1) {
				if(!collection2.Contains(item))
					return false; 
			}
			foreach(DataColumn item in collection2) {
				if(!collection1.Contains(item))
					return false; 
			}
			return true; 
		}
		static bool IsParentTableContaintsChildRelation(IList<DBRelation> tableRelation, DBRelation dbRelation) {
			foreach(DBRelation relation in tableRelation) {
				if(dbRelation.Equals(relation))
					return true;
			}
			return false;
		}
		static string GetDataRelationName(DataTable parentDataTable, DataTable childDataTable) {
			string dataRelationName = string.Format("{0}{1}", parentDataTable.TableName, childDataTable.TableName);
			int dataRelationNumber = 0;
			while(parentDataTable.ChildRelations.Contains(GetOrderDataRelationName(dataRelationName, dataRelationNumber)))
				dataRelationNumber++;
			return GetOrderDataRelationName(dataRelationName, dataRelationNumber);
		}
		static string GetOrderDataRelationName(string dataRelationName, int dataRelationNumber) {
			string dataRelationNumberString = dataRelationNumber == 0 ? string.Empty : string.Format(" #{0}", dataRelationNumber + 1);
			return dataRelationName + dataRelationNumberString;
		}
		public static string GetXmlSchema(DataSet dataSet) {
			using(StringWriter stringWriter = new StringWriter()) {
				using(XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter)) {
					dataSet.WriteXmlSchema(xmlWriter);
					return stringWriter.ToString();
				}
			}
		}
	}
}
