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
using System.Linq;
using DevExpress.Xpo.DB;
namespace DevExpress.XtraReports.Data {
	static class DBTableExtension {
		public static DataTable ConvertToDataTable(this DBTable dbTable, IList<string> fillingColumns) {
			System.Diagnostics.Debug.Assert(fillingColumns == null || fillingColumns.Count != 0);
			DataTable dataTable = new DataTable(dbTable.Name) { Locale = CultureInfo.InvariantCulture };
			var selectedColumns = fillingColumns == null ? dbTable.Columns : dbTable.Columns.Where(x => fillingColumns.Contains(x.Name));
			foreach(DBColumn column in selectedColumns)
				if(!column.Name.Contains("."))
					dataTable.Columns.Add(column.ConvertToDataColumn());
			dataTable.PrimaryKey = dataTable.Columns.OfType<DataColumn>().Where(x => dbTable.PrimaryKey != null && dbTable.PrimaryKey.Columns.Contains(x.ColumnName)).ToArray();
			return dataTable;
		}
	}
	static class DBColumnExtension {
		public static DataColumn ConvertToDataColumn(this DBColumn dbColumn) {
			DataColumn dataColumn = new DataColumn(dbColumn.Name) {
				DataType = DBColumn.GetType(dbColumn.ColumnType)
			};
			if(dataColumn.DataType == typeof(string)) {
				dataColumn.MaxLength = (dbColumn.Size == 0) ? -1 : dbColumn.Size;
			}
			return dataColumn;
		}
	}
	static class SelectStatementResultRowExtension {
		public static DataRow ConvertToDataRow(this SelectStatementResultRow dbRow, DataTable dataTable) {
			DataRow dataRow = dataTable.NewRow();
			for(int index = 0; index < dbRow.Values.Length; index++)
				dataRow[index] = dbRow.Values[index] ?? DBNull.Value;
			return dataRow;
		}
	}
}
