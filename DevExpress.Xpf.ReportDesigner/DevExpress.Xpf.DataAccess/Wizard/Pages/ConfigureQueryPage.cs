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
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpo.DB;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ConfigureQueryPage : DataSourceWizardPage, IConfigureQueryPageView {
		public static ConfigureQueryPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ConfigureQueryPage(model));
		}
		protected ConfigureQueryPage(DataSourceWizardModelBase model) : base(model) { }
		readonly Lazy<IEnumerable<BooleanViewModel<SinglePropertyViewModel<bool>>>> options = BooleanViewModel.CreateList(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureQuery_Query), DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureQuery_StoredProcedure), false, _ => SinglePropertyViewModel<bool>.Create(true));
		public IEnumerable<BooleanViewModel<SinglePropertyViewModel<bool>>> Options { get { return options.Value; } }
		[RaiseChanged]
		public virtual bool AllowCustomSql { get; protected set; }
		public virtual bool UseStoredProcedure { get; set; }
		protected void OnUseStoredProcedureChanged() {
			queryType = UseStoredProcedure ? QueryType.StoredProcedure : QueryType.TableOrCustomSql;
			if(queryTypeChanged != null)
				queryTypeChanged(this, EventArgs.Empty);
			RaiseChanged();
		}
		public virtual IEnumerable<string> StoredProcedures { get; protected set; }
		public virtual int SelectedStoredProcedureIndex { get; set; }
		protected void OnSelectedStoredProcedureIndexChanged() {
			if(SelectedStoredProcedureIndex == -1)
				return;
			if(storedProcedureChanged != null)
				storedProcedureChanged(this, EventArgs.Empty);
		}
		bool needToRaiseSqlChanged = true;
		string IConfigureQueryPageView.SqlString {
			get { return CustomSqlString; }
			set {
				needToRaiseSqlChanged = false;
				CustomSqlString = value;
				needToRaiseSqlChanged = true;
			}
		}
		public virtual string CustomSqlString { get; set; }
		protected void OnCustomSqlStringChanged() {
			if(needToRaiseSqlChanged)
				sqlStringChanged.Do(x => sqlStringChanged(this, EventArgs.Empty));
		}
		QueryBuilderRunnerBase IConfigureQueryPageView.CreateQueryBuilderRunner(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IParameterService parameterService) {
			if(parameterService == null)
				return new QueryBuilderRunner(model.Parameters.DoWithQueryBuilderDialogService, model.Parameters.DoWithMessageBoxService, schemaProvider, connection);
			else
				return new QueryBuilderRunner(model.Parameters.DoWithQueryBuilderDialogService, model.Parameters.DoWithMessageBoxService, schemaProvider, connection, parameterService);
		}
		void IConfigureQueryPageView.Initialize(bool allowCustomSql, bool storedProceduresSupported) {
			AllowCustomSql = allowCustomSql;
			Options.Last().Properties.Value = storedProceduresSupported;
		}
		void IConfigureQueryPageView.InitializeStoredProcedures(IEnumerable<string> items) {
			StoredProcedures = items;
		}
		QueryType queryType;
		QueryType IConfigureQueryPageView.QueryType {
			get { return queryType; }
			set { queryType = value; }
		}
		EventHandler queryTypeChanged;
		event EventHandler IConfigureQueryPageView.QueryTypeChanged {
			add { queryTypeChanged += value; }
			remove { queryTypeChanged -= value; }
		}
		EventHandler runQueryBuilder;
		event EventHandler IConfigureQueryPageView.RunQueryBuilder {
			add { runQueryBuilder += value; }
			remove { runQueryBuilder -= value; }
		}
		EventHandler sqlStringChanged;
		event EventHandler IConfigureQueryPageView.SqlStringChanged {
			add { sqlStringChanged += value; }
			remove { sqlStringChanged -= value; }
		}
		EventHandler storedProcedureChanged;
		event EventHandler IConfigureQueryPageView.StoredProcedureChanged {
			add { storedProcedureChanged += value; }
			remove { storedProcedureChanged -= value; }
		}
		public void RunQueryBuilder() {
			if(runQueryBuilder != null)
				runQueryBuilder(this, EventArgs.Empty);
		}
	}
}
