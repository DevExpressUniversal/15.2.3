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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Wizard.Native {
	public class PreviewContext {
		public PreviewContext(SqlQuery query, SqlDataConnection connection, IEnumerable<IParameter> sourceParameters, IDataConnectionParametersService connectionParametersService, IDBSchemaProvider dbSchemaProvider, ICustomQueryValidator validator, IWaitFormActivator waitFormActivator, IExceptionHandler validationtExceptionHandler, IExceptionHandler connectionExceptionHandler, IExceptionHandler loaderExceptionHandler) {
			Query = query;
			Connection = connection;
			SourceParameters = sourceParameters;
			ConnectionParametersService = connectionParametersService;
			DBSchemaProvider = dbSchemaProvider;
			Validator = validator;
			WaitFormActivator = waitFormActivator;
			ValidationtExceptionHandler = validationtExceptionHandler;
			ConnectionExceptionHandler = connectionExceptionHandler;
			LoaderExceptionHandler = loaderExceptionHandler;
		}
		public SqlQuery Query { get; private set; }
		public SqlDataConnection Connection { get; private set; }
		public IEnumerable<IParameter> SourceParameters { get; private set; }
		public IDataConnectionParametersService ConnectionParametersService { get; private set; }
		public IDBSchemaProvider DBSchemaProvider { get; private set; }
		public ICustomQueryValidator Validator { get; private set; }
		public IWaitFormActivator WaitFormActivator { get; private set; }
		public IExceptionHandler ValidationtExceptionHandler { get; private set; }
		public IExceptionHandler ConnectionExceptionHandler { get; private set; }
		public IExceptionHandler LoaderExceptionHandler { get; private set; }
	}
	public static class SqlQueryHelper {
		public static object LoadPreviewData(PreviewContext context) {
			Guard.ArgumentNotNull(context, "context");
			var sqlQuery = context.Query;
			if(sqlQuery == null || !Validate(sqlQuery, context.Connection.ConnectionParameters, context.Validator, context.ValidationtExceptionHandler)) {
				return null;
			}
			SqlDataConnection connection = context.Connection;
			if(!ConnectionHelper.OpenConnection(connection, context.ConnectionExceptionHandler, context.WaitFormActivator, context.ConnectionParametersService)) {
				return null;
			}
			UIDataLoader dataLoader = new UIDataLoader(context.WaitFormActivator, context.LoaderExceptionHandler);
			DBSchema dbSchema = DBSchema(sqlQuery as TableQuery, connection, dataLoader, context.DBSchemaProvider);
			SelectedDataEx selectedData = dataLoader.LoadData(connection, dbSchema, context.SourceParameters, sqlQuery, true);
			return selectedData == null ? null : new ResultTable(string.Empty, selectedData);
		}
		public static bool Validate(SqlQuery query, DataConnectionParametersBase connectionParameter, ICustomQueryValidator queryValidator, IExceptionHandler exceptionHandler) {
			if(query is CustomSqlQuery) {
				try {
					query.Validate(queryValidator, connectionParameter, null);
					return true;
				}
				catch(ValidationException e) {
					exceptionHandler.HandleException(e);
					return false;
				}
			}
			return true;
		}
		static DBSchema DBSchema(TableQuery tableQuery, SqlDataConnection connection, UIDataLoader dataLoader, IDBSchemaProvider dbSchemaProvider) {
			if(tableQuery != null) {
				var objectNames = tableQuery.Tables.Select(t => t.Name).ToArray();
				return dataLoader.LoadSchema(dbSchemaProvider, connection, objectNames);
			}
			return null;
		}
	}
}
