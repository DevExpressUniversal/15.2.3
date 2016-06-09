#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DashboardCommon.Native {
	[Obsolete("The SqlDataProviderBase class is obsolete now. Use the DashboardSqlDataSource classe instead.")]
	public class SqlDataProviderBase : DataProviderBase {
		protected bool IncludeTableNamesInSchema { get { return false; } }
		protected internal override bool HasSelectedColumns {
			get {
				SelectStatement selectStatement = DataSelection.CreateSelectStatement();
				return selectStatement != null && selectStatement.Operands.Count > 0;
			}
		}
		bool CanExecutePreview { get { return DataPreview == null; } }
		public SqlDataProviderBase(string connectionName, DataConnectionParametersBase connectionParameters, string sqlQuery)
			: this(new SqlDataConnection(connectionName, connectionParameters), sqlQuery) {
		}
		public SqlDataProviderBase(SqlDataConnection dataConnection, string sqlQuery)
			: this(dataConnection) {
			Guard.ArgumentIsNotNullOrEmpty(sqlQuery, "sqlQuery");
			SqlQuery = sqlQuery;
		}
		public SqlDataProviderBase(SqlDataConnection dataConnection)
			: base(dataConnection) {
		}
		public SqlDataProviderBase() {
		}
#if DEBUGTEST
		internal SqlDataProviderBase(object data, object dataPreview)
			: base(data, dataPreview) {
		}
#endif
		SelectedData GetSelectedData() {
			return GetSelectedData(false);
		}
		SelectedData GetSelectedData(bool isPreview) {
			SelectStatement selectStatement = DataSelection != null ? DataSelection.CreateSelectStatement() : null;
			if(selectStatement == null)
				return null;
			selectStatement.TopSelectedRecords = isPreview ? TopSelectedRecordsForPreview : 0;
			return SelectData(string.Empty, selectStatement);
		}
		object CreateDataView(SelectedData selectedData) {
			return CreateDataView(selectedData.ResultSet[0].Rows);
		}
		DataView CreateDataView(SelectStatementResultRow[] rows) {
			Guard.ArgumentNotNull(rows, "selectedData");
			if(!DataSelection.IsEmpty) {
				DataView dataView = new DataView(rows);
				foreach(DataColumn dataColumn in DataSelection.SelectedColumns) {
					string columnName = IncludeTableNamesInSchema ? string.Format("{0}.{1}", dataColumn.DataTable.UniqueName, dataColumn.Column.Name) : dataColumn.ActualName;
					dataView.AddColumn(columnName, DBColumn.GetType(dataColumn.Column.ColumnType));
				}
				return dataView;
			}
			return null;
		}
		protected virtual object CreateDataSchema() {
			if(IsCustomSql && IsSqlDataProvider)
				return null;
			return CreateDataView(new SelectStatementResultRow[0]);
		}
		protected override void SchemaChanged() {
			base.SchemaChanged();
			DataSchema = CreateDataSchema();
		}
	}
}
