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
using DevExpress.Data;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConfigureSqlParametersPage<TModel> : WizardPageBase<IConfigureParametersPageView, TModel>
		where TModel : ISqlDataSourceModel
	{
		readonly IWizardRunnerContext context;
		readonly IDataConnectionParametersService dataConnectionParametersService;
		readonly ICustomQueryValidator customQueryValidator;
		public override bool FinishEnabled { get { return true; } }
		public override bool MoveNextEnabled { get { return false; } }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Loading); } }
		IParameterService ParameterService { get; set; }
		IDBSchemaProvider DBSchemaProvider { get; set; }
		public ConfigureSqlParametersPage(IConfigureParametersPageView view, IWizardRunnerContext context, IParameterService parameterService, IDBSchemaProvider dbSchemaProvider, IDataConnectionParametersService connectionParametersService, ICustomQueryValidator validatior)
			: base(view) {
			customQueryValidator = validatior;
			dataConnectionParametersService = connectionParametersService;
			this.context = context;
			ParameterService = parameterService;
			DBSchemaProvider = dbSchemaProvider;
		}
		public override void Begin() {
			bool previewRowLimit = Model.Query is TableQuery;
			bool fixedParameters = Model.Query is StoredProcQuery;
			View.Initialize(Model.Query.Parameters.Select(p => new QueryParameter(p.Name, p.Type, p.Value)).ToList(), previewRowLimit, GetPreviewData, fixedParameters);
		}
		object GetPreviewData() {
			var previewContext = new PreviewContext(Model.Query, 
				Model.DataConnection, 
				GetSourceParameters(), 
				dataConnectionParametersService, 
				DBSchemaProvider, 
				customQueryValidator, 
				WaitFormActivator, 
				context.CreateExceptionHandler(ExceptionHandlerKind.Default, "Validation"), 
				context.CreateExceptionHandler(ExceptionHandlerKind.Connection), 
				ExceptionHandler);
			return SqlQueryHelper.LoadPreviewData(previewContext);
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = null;
			if(Model.Query == null)
				return true;
			if(Model.DataSchema != null)
				return true;
			SqlDataConnection dataConnection = Model.DataConnection;
			StoredProcQuery storedProcQuery = Model.Query as StoredProcQuery;
			if(storedProcQuery != null) {
				object schema;
				try { schema = dataConnection.GetStoredProcSchema(storedProcQuery.StoredProcName); }
				catch(Exception e) {
					ExceptionHandler.HandleException(e);
					return false;
				}
				if(schema != null) {
					Model.DataSchema = schema;
					return true;
				}
			}
			CustomSqlQuery customSqlQuery = Model.Query as CustomSqlQuery;
			if(customSqlQuery != null) {
				try {
					Model.DataSchema = dataConnection.GetCustomSqlSchema(customSqlQuery, View.GetParameters());
					return true;
				} catch {
				}
			}
			if(!View.ConfirmQueryExecution())
				return false;
			UIDataLoader dataLoader = new UIDataLoader(WaitFormActivator, ExceptionHandler);
			SqlQuery clonedQuery = GetQuery();
			SelectedDataEx selectedData = dataLoader.LoadData(Model.DataConnection, null, GetSourceParameters(), clonedQuery, false);
			if(selectedData == null)
				return false;
			HashSet<string> columns = new HashSet<string>();
			foreach(ColumnInfoEx columnInfoEx in selectedData.Schema) {
				string columnName = columnInfoEx.Name;
				if(string.IsNullOrEmpty(columnName) || columns.Add(columnName))
					continue;
				View.ShowDuplicatingColumnNameError(columnName);
				return false;
			}
			Model.DataSchema = new ResultTable(string.Empty, selectedData);
			return true;
		}
		SqlQuery GetQuery() {
			IEnumerable<QueryParameter> parameters = View.GetParameters().Select(QueryParameter.FromIParameter);
			SqlQuery clonedQuery = Model.Query.Clone();
			clonedQuery.Parameters.Clear();
			clonedQuery.Parameters.AddRange(parameters);
			return clonedQuery;
		}
		public override void Commit() {
			Model.Query.Parameters.Clear();
			Model.Query.Parameters.AddRange(View.GetParameters().Select(QueryParameter.FromIParameter));
		}
		IEnumerable<IParameter> GetSourceParameters() {
			return ParameterService != null ? ParameterService.Parameters : new List<IParameter>();
		}
	}
}
