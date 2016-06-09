#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
namespace DevExpress.XtraReports.Web.QueryBuilder.Native.Services {
	public class QueryBuilderModelGenerator: IQueryBuilderModelGenerator {
		private string[] tableQueryCollections = new string[] { "Query", "Table", "Column", "Relation", "KeyColumn", "Parameter", "View", "Field", "OrderBy", "GroupBy" };
		public QueryBuilderModel Generate(string connectionName) {
			return this.Generate(connectionName, null, null);
		}
		public QueryBuilderModel Generate(string connectionName, DataConnectionParametersBase connectionParameters, TableQuery tableQuery) {
			var dataSource = connectionParameters == null ? new DevExpress.DataAccess.Sql.SqlDataSource(connectionName) : new DevExpress.DataAccess.Sql.SqlDataSource(connectionParameters);
			var query = tableQuery == null ? new TableQuery() : tableQuery;
			return new QueryBuilderModel {
				TableQueryModelJson = new JsonXMLConverter(tableQueryCollections).XmlToJson(QuerySerializer.SaveToXml(query, null)),
				SqlDataSourceBase64 = dataSource.Base64
			};
		}
	}
}
