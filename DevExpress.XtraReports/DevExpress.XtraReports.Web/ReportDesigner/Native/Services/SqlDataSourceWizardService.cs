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

using System;
using System.IO;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraPrinting.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using System.ServiceModel;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native;
using System.Text;
using DevExpress.DataAccess.Localization;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class SqlDataSourceWizardService : ISqlDataSourceWizardService {
		readonly IDataSourceWizardDBSchemaProviderFactory dbBSchemaProviderFactory;
		readonly IDataSourceWizardConnectionStringsProvider connectionStringsProvider;
		readonly ISqlDataSourceWizardCustomizationService dataSourceWizardCustomizationService;
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		public SqlDataSourceWizardService(IDataSourceWizardDBSchemaProviderFactory dbBSchemaProviderFactory, IDataSourceWizardConnectionStringsProvider connectionStringsProvider, ISqlDataSourceWizardCustomizationService dataSourceWizardCustomizationService) {
			this.dbBSchemaProviderFactory = dbBSchemaProviderFactory;
			this.connectionStringsProvider = connectionStringsProvider;
			this.dataSourceWizardCustomizationService = dataSourceWizardCustomizationService;
		}
		#region ISqlDataSourceWizardService
		public SqlDataSourceStructure GetSqlDataSourceStructure(DataSourceModel request) {
			var sqlDataSource = RestoreSqlDataSource(request.DataSourceBase64);
			return new SqlDataSourceStructure { DataSourceStructure = SqlQueryJsonSerializer.GenerateSqlDataSourceJson(sqlDataSource) };
		}
		public DBSchemaModel GetDBSchema(DBSchemaRequest request) {
			using(SqlDataSource dataSource = RestoreSqlDataSource(request.DataSourceBase64, request.ConnectionName)) {
				using(var stream = new MemoryStream()) {
					OpenConnection(dataSource.Connection);
					DBSchema dbSchema = string.IsNullOrEmpty(request.TableName) ?
						GetDbSchema(dataSource.Connection) : GetDbSchema(dataSource.Connection, new string[] { request.TableName });
					dbSchema.SaveToXml(stream);
					stream.Position = 0;
					JsonXMLConverter converter = new JsonXMLConverter(new string[]{
						"DBTable", 
						"DBColumn",
						"DBForeignKey",
						"DBStoredProcedure",
						"DBStoredProcedureArgument",
						"DBTableWithAlias",
						"DBColumnWithAlias"
					});
					return new DBSchemaModel { DBSchemaJSON = converter.XmlToJson(System.Xml.Linq.XDocument.Load(stream).Root) };
				}
			}
		}
		public SelectStatementModel GetSelectStatement(SelectStatementRequest request) {
			TableQuery tableQuery = null;
			return GetSelectStatement(request, out tableQuery);
		}
		public SelectStatementModel GetSelectStatement(SelectStatementRequest request, out TableQuery tableQuery, bool validateQueryByExecution = false) {
			var query = SqlQueryJsonSerializer.CreateSqlQueryFromJson(request.SqlQueryJSON);
			tableQuery = query as TableQuery;
			if(tableQuery == null)
				return new SelectStatementModel { SqlSelectStatement = string.Empty };
			ValidateTableQuery(tableQuery);
			using(SqlDataSource dataSource = RestoreSqlDataSource(request.DataSourceBase64, request.ConnectionName)) {
				dataSource.Queries.Clear();
				dataSource.Queries.Add(query);
				OpenConnection(dataSource.Connection);
				DBSchema schema = GetDbSchema(dataSource.Connection, tableQuery);
				ValidateTableQuery(tableQuery, schema);
				if(validateQueryByExecution) ValidateQueryByExecution(dataSource.Connection, tableQuery, schema);
				return new SelectStatementModel { SqlSelectStatement = CreateSelectStatement(tableQuery, schema) };
			}
		}
		public WizardGeneratedDataSourceModel CreateSqlDataSource(WizardSqlDataSourceModel request) {
			SqlDataSource dataSource;
			CustomSqlQuery changedQuery = null;
			if(string.IsNullOrEmpty(request.DataSourceBase64)) {
				dataSource = new SqlDataSource(connectionStringsProvider.GetDataConnectionParameters(request.ConnectionString));
			} else {
				dataSource = RestoreSqlDataSource(request.DataSourceBase64);
				if(!string.IsNullOrEmpty(request.MasterDetailRelationsJSON)) {
					MasterDetailInfo[] relations = SqlQueryJsonSerializer.CreateRelationsFromJson(request.MasterDetailRelationsJSON);
					try {
						DevExpress.DataAccess.Native.Sql.MasterDetailEditorHelper.ValidateRelations(dataSource, relations);
					} catch(Exception e) {
						ProcessException(e, null, typeof(DevExpress.DataAccess.Native.Sql.InvalidConditionException), typeof(RelationException));
					}
					dataSource.Relations.Clear();
					dataSource.Relations.AddRange(relations);
				} else if(dataSource.Queries.Count > request.SqlQueryIndex) {
					changedQuery = dataSource.Queries[request.SqlQueryIndex] as CustomSqlQuery;
					dataSource.Queries.RemoveAt(request.SqlQueryIndex);
				}
			}
			if(!string.IsNullOrEmpty(request.SqlQueryJSON)) {
				SqlQuery query = SqlQueryJsonSerializer.CreateSqlQueryFromJson(request.SqlQueryJSON);
				var newQuery = query as CustomSqlQuery;
				if(newQuery != null) {
					bool wasSqlChanged = changedQuery == null || string.Compare(changedQuery.Sql, newQuery.Sql) != 0;
					if(wasSqlChanged && this.dataSourceWizardCustomizationService.IsCustomSqlDisabled) {
						throw new FaultException("Custom sql query editing is disabled");
					}
					if(this.dataSourceWizardCustomizationService.CustomQueryValidator != null) {
						var message = DataAccessLocalizer.GetString(DataAccessStringId.CustomSqlQueryValidationException);
						if(!this.dataSourceWizardCustomizationService.CustomQueryValidator.Validate(dataSource.ConnectionParameters, newQuery.Sql, ref message)) {
							throw new FaultException(message);
						}
					}
				}
				query.Name = dataSource.Queries.GenerateUniqueName(query);
				dataSource.Queries.Insert(request.SqlQueryIndex, query);
			}
			try {
				dataSource.RebuildResultSchema();
			} catch(Exception e) {
				ProcessException(e, "An error occurred while rebuilding a data source schema.", typeof(RelationException), typeof(ValidationException));
				throw;
			}
			var json = DataSourcesJSContentGeneratorLogic.GenerateDataSourceInfoJson(dataSource, null);
			var dataSourceInfo = new DataSourceInfo {
				Id = Guid.NewGuid().ToString("N"),
				Name = DataSourceFieldListServiceLogic.GetDataSourceDisplayName(dataSource),
				Data = json
			};
			return new WizardGeneratedDataSourceModel { DataSource = dataSourceInfo };
		}
		public string GetDataPreview(SelectStatementRequest request) {
			var query = SqlQueryJsonSerializer.CreateSqlQueryFromJson(request.SqlQueryJSON);
			var tableQuery = query as TableQuery;
			if(tableQuery == null)
				return string.Empty;
			ValidateTableQuery(tableQuery);
			using(SqlDataSource dataSource = RestoreSqlDataSource(request.DataSourceBase64, request.ConnectionName)) {
				dataSource.Queries.Clear();
				dataSource.Queries.Add(query);
				OpenConnection(dataSource.Connection);
				return GenerateDataPreviewJSON(dataSource.Connection, tableQuery);
			}
		}
		#endregion
		DBSchema GetDbSchema(SqlDataConnection dataConnection, TableQuery tableQuery) {
			return GetDbSchema(dataConnection, tableQuery.Tables.ConvertAll<string>(table => table.Name).ToArray());
		}
		DBSchema GetDbSchema(SqlDataConnection dataConnection, string[] tableList = null) {
			var schemaProvider = dbBSchemaProviderFactory.Create();
			try {
				DBSchema schema;
				if(tableList != null && tableList.Length > 0) {
					schema = dataConnection.GetDBSchema(tableList);
					foreach(var table in schema.Tables) {
						schemaProvider.LoadColumns(dataConnection, table);
					}
					foreach(var view in schema.Views) {
						schemaProvider.LoadColumns(dataConnection, view);
					}
				} else {
					schema = schemaProvider.GetSchema(dataConnection);
				}
				return schema;
			} catch(Exception e) {
				ProcessException(e, "Unable to get a database schema.");
				throw;
			}
		}
		SqlDataSource RestoreSqlDataSource(string dataSourceBase64, string connectionName = null) {
			SqlDataSource dataSource;
			if(string.IsNullOrEmpty(dataSourceBase64)) {
				dataSource = new SqlDataSource(connectionStringsProvider.GetDataConnectionParameters(connectionName));
			} else {
				dataSource = new SqlDataSource();
				dataSource.Base64 = dataSourceBase64;
			}
			return dataSource;
		}
		void OpenConnection(SqlDataConnection connection) {
			try {
				connection.Open();
			} catch(Exception e) {
				ProcessException(e, "Unable to open a database.");
				throw;
			}
		}
		string CreateSelectStatement(TableQuery tableQuery, DBSchema schema) {
			try {
				return tableQuery.GetSql(schema);
			} catch(Exception e) {
				ProcessException(e, "select statement creation failed.");
				throw;
			}
		}
		SelectedDataEx ExecuteQuery(SqlDataConnection connection, TableQuery tableQuery, DBSchema schema) {
			SelectedDataEx result = null;
			try {
				var executor = new QueryExecutor(connection, schema);
				result =  executor.Execute(tableQuery, true, new System.Threading.CancellationToken());
			} catch(Exception exception) {
				AggregateException aex = exception as AggregateException;
				if(aex != null) {
					aex.Flatten();
					if(aex.InnerExceptions.Count == 1)
						exception = aex.GetBaseException();
				}
				throw new FaultException(exception.Message);
			}
			return result;
		}
		void ValidateQueryByExecution(SqlDataConnection connection, TableQuery tableQuery, DBSchema schema) {
			var oldTop = tableQuery.Top;
			tableQuery.Top = 1;
			ExecuteQuery(connection, tableQuery, schema);
			tableQuery.Top = oldTop;
		}
		string GenerateDataPreviewJSON(SqlDataConnection connection, TableQuery tableQuery) {
			DBSchema schema = GetDbSchema(connection, tableQuery);
			ValidateTableQuery(tableQuery, schema);
			return GenerateDataPreviewJSON(ExecuteQuery(connection, tableQuery, schema));
		}
		public static string GenerateDataPreviewJSON(SelectedDataEx data, int maxRowsCount = 100) {
			var sb = new StringBuilder("{");
			JsonGenerator.AppendQuotedValue(sb, "schema", true);
			sb.Append('[');
			foreach(var item in data.Schema) {
				sb.Append('{');
				JsonGenerator.AppendQuotedValue(sb, "name", true);
				JsonGenerator.AppendQuotedValue(sb, item.Name);
				sb.Append(',');
				JsonGenerator.AppendQuotedValue(sb, "type", true);
				JsonGenerator.AppendQuotedValue(sb, item.Type.ToString());
				sb.Append('}');
				sb.Append(',');
			}
			sb.Remove(sb.Length - 1, 1);
			sb.Append("],");
			JsonGenerator.AppendQuotedValue(sb, "values", true);
			if(data.Lists[0].Count == 0) {
				sb.Append("null");
			} else {
				sb.Append('[');
				for(var i = 0; i < data.Lists[0].Count && i < maxRowsCount; i++) {
					sb.Append('[');
					foreach(var list in data.Lists) {
						JsonGenerator.AppendQuotedValue(sb, (list[i] != null ? list[i].ToString() : "null"));
						sb.Append(',');
					}
					sb.Remove(sb.Length - 1, 1);
					sb.Append(']');
					sb.Append(',');
				}
				sb.Remove(sb.Length - 1, 1);
				sb.Append("]");
			}
			sb.Append('}');
			return sb.ToString();
		}
		void ValidateTableQuery(TableQuery query, DBSchema schema = null) {
			try {
				if(schema == null) query.Validate();
				else query.Validate(schema);
			} catch(ValidationException ex) {
				if(ex is NoTablesValidationException || ex is NoColumnsValidationException)
					throw new FaultException(DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderNothingSelected));
				throw new FaultException(ex.Message);
			}
		}
		void ProcessException(Exception originalException, string customMessage, params Type[] authorizedExceptionTypes) {
			if(originalException is FaultException) return;
			Logger.Error(originalException.ToString());
			if(Array.IndexOf(authorizedExceptionTypes, originalException.GetType()) >= 0) throw new FaultException(originalException.Message);
			if(!string.IsNullOrEmpty(customMessage)) throw new FaultException(customMessage);
		}
	}
}
