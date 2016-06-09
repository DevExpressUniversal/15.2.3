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
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConfigureQueryPage<TModel> : WizardPageBase<IConfigureQueryPageView, TModel>
		where TModel : ISqlDataSourceModel {
		struct StoredProcedureViewInfo {
			public DBStoredProcedure Value;
			public string DisplayName;
			public StoredProcedureViewInfo(DBStoredProcedure value, string displayName) {
				Value = value;
				DisplayName = displayName;
			}
		}
		const int maxLineLength = 70;
		readonly IWizardRunnerContext context;
		readonly SqlWizardOptions options;
		readonly IDBSchemaProvider dbSchemaProvider;
		readonly IParameterService parameterService;
		QueryType queryType;
		DBSchema dbSchema;
		TableQuery tableQuery;
		string sqlText;
		bool sqlTextModified;
		object dataSchema;
		StoredProcedureViewInfo[] storedProcedures;
		List<QueryParameter> originalParameters;
		readonly ICustomQueryValidator customQueryValidator;
		public ConfigureQueryPage(IConfigureQueryPageView view, IWizardRunnerContext context, SqlWizardOptions options,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, ICustomQueryValidator customQueryValidator) : base(view) {
			this.context = context;
			this.options = options;
			this.dbSchemaProvider = dbSchemaProvider;
			this.parameterService = parameterService;
			this.customQueryValidator = customQueryValidator;
		}
		public override bool FinishEnabled { get { return IsSchemaReady; } }
		public override bool MoveNextEnabled { get { return GetResult() != null && !IsSchemaReady; } }
		protected bool IsSchemaReady {
			get {
				if(DataSchema == null)
					return false;
				SqlQuery result = GetResult();
				return result != null && result.Parameters.Count == 0;
			}
		}
		protected bool SqlTextModified { get { return sqlTextModified; } }
		protected string SqlText {
			get { return sqlText; }
			set {
				if(value == sqlText)
					return;
				sqlText = value;
				UpdateSqlEditor();
			}
		}
		protected object DataSchema { get { return dataSchema; } }
		protected string StoredProcedureName { get; set; }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		bool SchemaLoaded { get { return dbSchema != null; } }
		public DBSchema DBSchema { get { return dbSchema; } }
		public override void Begin() {
			dbSchema = null;
			bool isStoredProcSupported = Model.DataConnection.IsStoredProceduresSupported();
			View.Initialize(
				options.HasFlag(SqlWizardOptions.EnableCustomSql) && Model.DataConnection.IsSqlDataStore,
				isStoredProcSupported);
			StoredProcQuery storedProcQuery = Model.Query as StoredProcQuery;
			if(storedProcQuery != null) {
				queryType = QueryType.StoredProcedure;
				InitializeStoredProcedures();
				SetStoredProcQuery(storedProcQuery);
			}
			else {
				queryType = QueryType.TableOrCustomSql;
				SetTableOrCustomSqlQuery(Model.Query);
			}
			View.QueryType = queryType;
			View.QueryTypeChanged += View_Changed;
			View.RunQueryBuilder += View_RunQueryBuilder;
			View.SqlStringChanged += TableQueryControl_Changed;
			View.StoredProcedureChanged += SpQueryControl_Changed;
		}
		#region Overrides of WizardPageBase<IConfigureQueryPageView,TModel>
		public override bool Validate(out string errorMessage) {
			errorMessage = null;
			var query = GetResult();
			return SqlQueryHelper.Validate(query, Model.DataConnection.ConnectionParameters, customQueryValidator, context.CreateExceptionHandler(ExceptionHandlerKind.Default, "Validation"));
		}
		#endregion
		public override void Commit() {
			Model.Query = GetResult();
			Model.DataSchema = DataSchema;
			Model.SqlQueryText = SqlText;
			View.QueryTypeChanged -= View_Changed;
			View.RunQueryBuilder -= View_RunQueryBuilder;
			View.SqlStringChanged -= TableQueryControl_Changed;
			View.StoredProcedureChanged -= SpQueryControl_Changed;
		}
		public override Type GetNextPageType() { return typeof(ConfigureSqlParametersPage<TModel>); }
		protected virtual SqlQuery GetResult() {
			switch(queryType) {
				case QueryType.TableOrCustomSql: {
					if(!sqlTextModified)
						return tableQuery;
					var result = new CustomSqlQuery();
					result.Sql = sqlText;
					if(tableQuery != null)
						result.Parameters.AddRange(tableQuery.Parameters);
					return result;
				}
				case QueryType.StoredProcedure: {
					if(StoredProcedureName == null)
						return null;
					StoredProcQuery result = new StoredProcQuery(StoredProcedureName, StoredProcedureName);
					if(originalParameters != null)
						result.Parameters.AddRange(originalParameters);
					if(DBSchema != null)
						StoredProcParametersHelper.SyncParams(
							new SqlStoredProcInfo(result.Name, result.Parameters), DBSchema);
					return result;
				}
				default:
					throw new InvalidOperationException();
			}
		}
		protected virtual void SetTableOrCustomSqlQuery(SqlQuery value) {
			if(value == tableQuery)
				return;
			CustomSqlQuery customSql = value as CustomSqlQuery;
			if(customSql != null) {
				tableQuery = new TableQuery();
				tableQuery.Parameters.AddRange(customSql.Parameters);
				sqlText = customSql.Sql;
				sqlTextModified = true;
			}
			else {
				tableQuery = (TableQuery)value;
				sqlText = GetSql(tableQuery);
				sqlTextModified = false;
				dataSchema = tableQuery.GetDataSchema(dbSchema);
			}
			UpdateSqlEditor();
		}
		protected virtual void SetStoredProcQuery(StoredProcQuery value) {
			if(value != null) {
				StoredProcedureName = value.StoredProcName;
				originalParameters = value.Parameters;
				UpdateSelectedIndex();
			} else {
				StoredProcedureName = null;
				originalParameters = null;
				View.SelectedStoredProcedureIndex = -1;
			}
		}
		void UpdateSelectedIndex() {
			View.SelectedStoredProcedureIndex = Array.FindIndex(storedProcedures, info => info.Value.Name == StoredProcedureName);
		}
		protected void UpdateSqlEditor() { View.SqlString = sqlTextModified ? sqlText : AutoSqlWrapHelper.AutoSqlTextWrap(sqlText, maxLineLength); }
		protected virtual void RunQueryBuilder() {
			SqlQuery query = GetResult();
			if(query is CustomSqlQuery && !options.HasFlag(SqlWizardOptions.EnableCustomSql)) {
				if(!context.Confirm(DataAccessLocalizer.GetString(DataAccessStringId.ConfigureQueryPage_CustomSqlWillBeLost)))
					return;
				query = null;
			}
			LoadDbSchema();
			if(dbSchema == null)
				return;
			if(dbSchema.Tables.Length == 0 && dbSchema.Views.Length == 0 && !Model.DataConnection.IsSqlDataStore) {
				ShowMessage(DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderNoTablesAndViews));
				return;
			}
			QueryBuilderRunnerBase qbr = View.CreateQueryBuilderRunner(dbSchemaProvider, Model.DataConnection, parameterService);
			SqlQuery result = qbr.Edit(query);
			if(result == null)
				return;
			TableQuery tQuery = result as TableQuery;
			if(tQuery != null) {
				tableQuery = tQuery;
				sqlText = GetSql(tQuery);
				sqlTextModified = false;
				dataSchema = tableQuery.GetDataSchema(dbSchema);
			}
			else {
				sqlText = ((CustomSqlQuery)result).Sql;
				sqlTextModified = true;
				dataSchema = null;
			}
			UpdateSqlEditor();
			RaiseChanged();
		}
		protected virtual string GetSql(TableQuery tableQuery) {
			if(tableQuery == null)
				return null;
			LoadDbSchema();
			ISqlGeneratorFormatter sqlFormatter = Model.DataConnection.SqlGeneratorFormatter;
			return sqlFormatter != null ? tableQuery.BuildSql(dbSchema, sqlFormatter) : null;
		}
		protected virtual void InitializeStoredProcedures() {
			LoadDbSchema();
			storedProcedures = DBSchema.StoredProcedures == null
				? new StoredProcedureViewInfo[0]
				: DBSchema.StoredProcedures.OrderBy(sp => sp.Name)
					.Select(
						sp =>
							new StoredProcedureViewInfo(sp,
								string.Format("{0}({1})", sp.Name,
									string.Join(", ", sp.Arguments.Select(arg => arg.Name)))))
					.ToArray();
			View.InitializeStoredProcedures(storedProcedures.Select(info => info.DisplayName));
			UpdateSelectedIndex();
		}
		protected virtual void ShowMessage(string message) { context.ShowMessage(message); }
		protected void LoadDbSchema() {
			if(SchemaLoaded)
				return;
			UIDataLoader loader = new UIDataLoader(WaitFormActivator, ExceptionHandler);
			dbSchema = loader.LoadSchema(dbSchemaProvider, Model.DataConnection, new string[0]);
		}
		void View_Changed(object sender, EventArgs e) {
			queryType = View.QueryType;
			if(queryType == QueryType.StoredProcedure)
				InitializeStoredProcedures();
			RaiseChanged();
		}
		void View_RunQueryBuilder(object sender, EventArgs e) {
			if(queryType == QueryType.TableOrCustomSql)
				RunQueryBuilder();
		}
		void SpQueryControl_Changed(object sender, EventArgs e) {
			if(View.SelectedStoredProcedureIndex >= 0)
				StoredProcedureName = storedProcedures[View.SelectedStoredProcedureIndex].Value.Name;
			RaiseChanged();
		}
		void TableQueryControl_Changed(object sender, EventArgs e) {
			sqlTextModified = true;
			sqlText = View.SqlString;
			dataSchema = null;
			RaiseChanged();
		}
	}
	public enum QueryType {
		TableOrCustomSql,
		StoredProcedure
	}
}
