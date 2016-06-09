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

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
using System;
namespace DevExpress.XtraReports.Data {
	public class DbConnectionDataProvider : IDataProvider, IDisposable {
		#region Fields & Properties
		readonly DataSchemaProvider dataSchemaProvider;
		readonly DataProvider dataProvider;
		readonly ITypeSpecificsService typeSpecificsService = new TypeSpecificsService();
		readonly List<IDisposable> disposableObjectsList = new List<IDisposable>();
		bool disposed = false;
		protected DataSchemaProvider DataSchemaProvider { get { return dataSchemaProvider; } }
		protected DataProvider DataProvider { get { return dataProvider; } }
		#endregion
		#region ctor
		public DbConnectionDataProvider(DataSchemaProvider dataSchemaProvider, DataProvider dataProvider, IDisposable[] disposableObjectCollection) {
			Guard.ArgumentNotNull(dataSchemaProvider, "dataSchemaProvider");
			Guard.ArgumentNotNull(dataProvider, "dataProvider");
			if(disposableObjectCollection.Length > 0) {
				this.disposableObjectsList.AddRange(disposableObjectCollection);
			}
			this.dataSchemaProvider = dataSchemaProvider;
			this.dataProvider = dataProvider;
		}
		#endregion
		public IEnumerable<ColumnInfo> GetColumns(string dataMember) {
			Guard.ArgumentIsNotNullOrEmpty(dataMember, "dataMember");
			using(DataSet ds = GetMemberDataSchema(dataMember)) {
				DataTable dbTable = ds.Tables[dataMember];
				List<ColumnInfo> list = new List<ColumnInfo>();
				if(dbTable != null) {
					foreach(DataColumn column in dbTable.Columns) {
						ColumnInfo columnInfo = new ColumnInfo()
						{
							Name = column.ColumnName,
							DisplayName = column.Caption,
							TypeSpecifics = typeSpecificsService.GetTypeSpecifics(column.DataType),
						};
						list.Add(columnInfo);
					}
				}
				return list;
			}
		}
		protected virtual DataSet GetMemberDataSchema(string dataMember) {
			ColumnsByTableDictionary dictionary = new ColumnsByTableDictionary();
			dictionary.Add(dataMember, null);
			DataSet ds = CreateDataSet(dataSchemaProvider.FillTablesViewsMergedDataSchema(dictionary));
			return ds;
		}
		public object GetData(string dataMember) {
			Guard.ArgumentIsNotNullOrEmpty(dataMember, "dataMember");
			DataSet dataSet = GetMemberDataSchema(dataMember);
			dataProvider.Fill(dataSet);
			return dataSet;
		}
		public IEnumerable<TableInfo> GetTables() {
			return GetDataMembersInfo(dataSchemaProvider.SchemaExplorer.GetStorageTablesList(false), false);
		}
		public IEnumerable<TableInfo> GetViews() {
			return GetDataMembersInfo(dataSchemaProvider.GetStorageViewsList(), true);
		}
		public bool TablesOrViewsSupported {
			get { return true; }
		}
		protected virtual string BuildDisplayName (string name) {
			return name;
		} 
		IEnumerable<TableInfo> GetDataMembersInfo(string[] dataMembers, bool isView) {
			List<TableInfo> list = new List<TableInfo>();
			foreach(string dataMemberName in dataMembers) {
				list.Add(
					new TableInfo()
					{
						Name = dataMemberName,
						DisplayName = BuildDisplayName(dataMemberName),
						DataMemberType = isView ? DataMemberType.View : DataMemberType.Table
					});
			}
			return list;
		}		
		protected DataSet CreateDataSet(string dataSchema) {
			DataSet dataSet = new DataSet();
			using(StringReader reader = new StringReader(dataSchema)) {
				dataSet.ReadXmlSchema(reader);
			}
			return dataSet;
		}		
		#region Disposing
		~DbConnectionDataProvider() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					disposableObjectsList.ForEach(x => x.Dispose());
				}
				disposed = true;
			}
		}
		#endregion
	}
}
