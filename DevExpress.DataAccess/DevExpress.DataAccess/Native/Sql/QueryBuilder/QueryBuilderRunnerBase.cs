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
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public abstract class QueryBuilderRunnerBase {
		readonly IDBSchemaProvider schemaProvider;
		readonly SqlDataConnection connection;
		protected readonly IParameterService parameterService;
		readonly ICustomQueryValidator customQueryValidator;
		protected QueryBuilderRunnerBase(IDBSchemaProvider schemaProvider, SqlDataConnection connection) : this(schemaProvider, connection, null, null) {}
		protected QueryBuilderRunnerBase(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IParameterService parameterService, ICustomQueryValidator customQueryValidator) {
			this.parameterService = parameterService;
			this.schemaProvider = schemaProvider;
			this.connection = connection;
			this.customQueryValidator = customQueryValidator;
		}
		public SqlQuery Edit(SqlQuery query) {
			if(query == null)
				return Create();
			if(query is StoredProcQuery)
				throw new ArgumentException("Either TableQuery or CustomSqlQuery required.");
			SqlQuery oldQuery = query;
			QueryBuilderModel model = new QueryBuilderModel(query.Clone());
			return EditCore(model) ?? oldQuery;
		}
		public SqlQuery Create() {
			QueryBuilderModel model = new QueryBuilderModel(new TableQuery());
			return EditCore(model);
		}
		protected abstract IQueryBuilderView CreateView(QueryBuilderViewModel vm);
		SqlQuery EditCore(QueryBuilderModel model) {
			QueryBuilderViewModel vm = new QueryBuilderViewModel(model);
			using (IQueryBuilderView view = CreateView(vm)) {
				vm.Initialize(this.schemaProvider, this.connection, this.parameterService != null ? this.parameterService.Parameters : null, customQueryValidator);
				DBSchema dbSchema = this.schemaProvider.GetSchema(this.connection);
				SqlQuery result = null;
				view.Ok += (s, e) => {
					result = GetResult(model);
					string error = Validate(result, dbSchema);
					if (error == null) {
						view.Stop();
					}
					else {
						result = null;
						view.ShowError(error);
					}
				};
				view.Start();
				return result;
			}
		}
		SqlQuery GetResult(QueryBuilderModel model) {
			if(model.SqlEditing)
				return model.CustomSqlQuery;
			return model.TableQuery;
		}
		string Validate(SqlQuery query, DBSchema schema) {
			try {
				query.Validate(customQueryValidator, connection.ConnectionParameters, schema);
				return null;
			}
			catch(ValidationException ex) {
				if(ex is NoTablesValidationException || ex is NoColumnsValidationException)
					return DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderNothingSelected);
				if(ex is FilterByColumnOfMissingTableValidationException || ex is FilterByMissingInSchemaColumnValidationExpression)
					return ex.Message + Environment.NewLine + DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderInvalidFilter);
				return ex.Message;
			}
		}
	}
}
