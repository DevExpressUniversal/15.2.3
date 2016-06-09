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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.DataAccess.Native;
using Locker = DevExpress.Xpf.Core.Locker;
using DevExpress.Utils;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public sealed class DataSourceWizardModelParameters {
		public DataSourceWizardModelParameters(
			WizardController controller,
			Action<Action<IMessageBoxService>> doWithMessageBoxServiceCallback,
			Action<Action<ISplashScreenService>> doWithSplashScreenServiceCallback,
			Action<Action<IOpenFileDialogService>> doWithOpenFileDialogServiceCallback,
			Action<Action<IDialogService>> doWithQueryBuilderDialogServiceCallback,
			Action<Action<IDialogService>> doWithPreviewDialogServiceCallback,
			Action<Action<IDialogService>> doWithPasswordDialogServiceCallback,
			Action<Action<IDialogService>> doWithChooseEFStoredProceduresDialogServiceCallback) {
			Controller = controller;
			DoWithMessageBoxService = doWithMessageBoxServiceCallback;
			DoWithSplashScreenService = doWithSplashScreenServiceCallback;
			DoWithOpenFileDialogService = doWithOpenFileDialogServiceCallback;
			DoWithQueryBuilderDialogService = doWithQueryBuilderDialogServiceCallback;
			DoWithPreviewDialogService = doWithPreviewDialogServiceCallback;
			DoWithPasswordDialogService = doWithPasswordDialogServiceCallback;
			DoWithChooseEFStoredProceduresDialogService = doWithChooseEFStoredProceduresDialogServiceCallback;
		}
		public readonly WizardController Controller;
		public readonly Action<Action<IMessageBoxService>> DoWithMessageBoxService;
		public readonly Action<Action<ISplashScreenService>> DoWithSplashScreenService;
		public readonly Action<Action<IOpenFileDialogService>> DoWithOpenFileDialogService;
		public readonly Action<Action<IDialogService>> DoWithQueryBuilderDialogService;
		public readonly Action<Action<IDialogService>> DoWithPreviewDialogService;
		public readonly Action<Action<IDialogService>> DoWithPasswordDialogService;
		public readonly Action<Action<IDialogService>> DoWithChooseEFStoredProceduresDialogService;
	}
	public abstract class DataSourceWizardModelBase : IWizardView, IWizardPageFactory<IDataSourceModel> {
		readonly protected WizardEngine wizard;
		readonly IDataSourceWizardExtensions extensions;
		readonly bool enableCustomSql;
		readonly IParameterService parameterService;
		readonly protected HierarchyCollection<IParameter, IParameterService> reportParameters;
		protected DataSourceWizardModelBase(DataSourceWizardModelParameters parameters, bool enableCustomSql, IDataSourceWizardExtensions extensions, IParameterService parameterService, IDataSourceModel dataSourceModel) {
			Guard.ArgumentNotNull(parameterService, "parameterService");
			Parameters = parameters;
			this.extensions = extensions;
			this.enableCustomSql = enableCustomSql;
			this.parameterService = parameterService;
			this.reportParameters = new HierarchyCollection<IParameter, IParameterService>(parameterService, (parameter, pService) => pService.AddParameter(parameter), null, parameterService.Parameters, false);
			this.wizard = new WizardEngine(this, dataSourceModel);
			RegisterPages();
			wizard.SetStartPage(typeof(ChooseDataSourceTypePage<IDataSourceModel>));
		}
		public readonly DataSourceWizardModelParameters Parameters;
		protected virtual void RegisterPages() {
			RegisterPage(() => new ChooseDataSourceTypePage<IDataSourceModel>(ChooseDataSourceTypePage.Create(this), WizardRunnerContext, SqlDataConnections, SolutionTypesProvider));
			RegisterPage(() => new ChooseConnectionPage<IDataSourceModel>(ChooseConnectionPage.Create(SqlDataConnections, this), WizardRunnerContext, DataConnectionParametersService, ConnectionStorageService, SqlDataConnections, SqlWizardOptions.None));
			RegisterPage(() => new ConnectionPropertiesPage<IDataSourceModel>(ConnectionPropertiesPage.Create(this), WizardRunnerContext, DataConnectionParametersService, ConnectionStorageService, SqlDataConnections, SqlWizardOptions.None));
			RegisterPage(() => new ChooseEFContextPage<IDataSourceModel>(ChooseEFContextPage.Create(this), WizardRunnerContext, SolutionTypesProvider, ConnectionStringsProvider));
			RegisterPage(() => new ChooseEFConnectionStringPage<IDataSourceModel>(ChooseEFConnectionStringPage.Create(this), WizardRunnerContext, ConnectionStringsProvider, ConnectionStorageService));
			RegisterPage(() => new ConfigureEFConnectionStringPage<IDataSourceModel>(ConfigureEFConnectionStringPage.Create(this), WizardRunnerContext, ConnectionStorageService));
			RegisterPage(() => new ChooseEFDataMemberPage<IDataSourceModel>(ChooseEFDataMemberPage.Create(this), WizardRunnerContext));
			RegisterPage(() => new ChooseObjectAssemblyPage<IDataSourceModel>(ChooseObjectAssemblyPage.Create(this), WizardRunnerContext, SolutionTypesProvider));
			RegisterPage(() => new ChooseObjectTypePage<IDataSourceModel>(ChooseObjectTypePage.Create(this), WizardRunnerContext, OperationMode.Both));
			RegisterPage(() => new ConfigureQueryPage<IDataSourceModel>(ConfigureQueryPage.Create(this), WizardRunnerContext, enableCustomSql ? SqlWizardOptions.EnableCustomSql :SqlWizardOptions.None, DBSchemaProvider, parameterService, CustomQueryValidator));
			RegisterPage(() => new ChooseObjectMemberPage<IDataSourceModel>(ChooseObjectMemberPage.Create(this), WizardRunnerContext, OperationMode.Both));
			RegisterPage(() => new ChooseObjectBindingModePage<IDataSourceModel>(ChooseObjectBindingModePage.Create(this), OperationMode.Both));
			RegisterPage(() => new ChooseObjectConstructorPage<IDataSourceModel>(ChooseObjectConstructorPage.Create(this), OperationMode.Both));
			RegisterPage(() => new ObjectConstructorParametersPage<IDataSourceModel>(ObjectConstructorParametersPage.Create(this, reportParameters)));
			RegisterPage(() => new ConfigureSqlParametersPage<IDataSourceModel>(ConfigureSqlParametersPage.Create(this, reportParameters), WizardRunnerContext, parameterService, DBSchemaProvider, DataConnectionParametersService, CustomQueryValidator));
			RegisterPage(() => new ChooseFilePage<IDataSourceModel>(ChooseFilePage.Create(this), WizardRunnerContext));
			RegisterPage(() => new ChooseFileOptionsPage<IDataSourceModel>(ChooseFileOptionsPage.Create(this), WizardRunnerContext, ExcelSchemaProvider));
			RegisterPage(() => new ChooseExcelFileDataRangePage<IDataSourceModel>(ChooseExcelFileDataRangePage.Create(this), WizardRunnerContext, ExcelSchemaProvider));
			RegisterPage(() => new ConfigureExcelFileColumnsPage<IDataSourceModel>(ConfigureExcelFileColumnsPage.Create(this)));
			RegisterPage(() => new ObjectMemberParametersPage<IDataSourceModel>(ObjectMemberParametersPage.Create(this, reportParameters), OperationMode.Both));
			RegisterPage(() => new ConfigureEFStoredProceduresPage<IDataSourceModel>(ConfigureEFStoredProceduresPage.Create(this, reportParameters), WizardRunnerContext, ParameterService));
			RegisterPage(() => new SaveConnectionPage<IDataSourceModel>(SaveConnectionPage.Create(this), WizardRunnerContext, ConnectionStorageService));
		}
		protected internal virtual void NavigateToPreviousPage() {
			navigateLock.DoLockedAction(() => {
				if(previous != null)
					previous(this, EventArgs.Empty);
			});
		}
		void IWizardView.EnablePrevious(bool enable) { }
		EventHandler previous;
		event EventHandler IWizardView.Previous {
			add { previous += value; }
			remove { previous -= value; }
		}
		protected internal virtual void NavigateToNextPage() {
			if(next != null)
				next(this, EventArgs.Empty);
		}
		bool isNextButtonEnabled;
		void IWizardView.EnableNext(bool enable) {
			isNextButtonEnabled = enable;
			ForAllPages(x => x.CanGoForward = isNextButtonEnabled);
		}
		EventHandler next;
		event EventHandler IWizardView.Next {
			add { next += value; }
			remove { next -= value; }
		}
		internal bool Cancel() {
			bool cancelled = false;
			EventHandler onWizardCancelled = (s, e) => cancelled = true;
			wizard.Cancelled += onWizardCancelled;
			try {
				if(cancel != null)
					cancel(this, EventArgs.Empty);
			} finally {
				wizard.Cancelled -= onWizardCancelled;
			}
			return cancelled;
		}
		EventHandler cancel;
		event EventHandler IWizardView.Cancel {
			add { cancel += value; }
			remove { cancel -= value; }
		}
		protected internal virtual bool Finish() {
			bool completed = false;
			EventHandler onWizardCompleted = (s, e) => {
				var dataSource = new DataComponentCreator().CreateDataComponent(wizard.CurrentPage.Model);
				if(extensions != null)
					extensions.AddNewDataSouce(dataSource);
				completed = true;
			};
			wizard.Completed += onWizardCompleted;
			try {
				RaiseFinish();
			} finally {
				wizard.Completed -= onWizardCompleted;
			}
			return completed;
		}
		bool isFinishButtonEnabled;
		void IWizardView.EnableFinish(bool enable) {
			isFinishButtonEnabled = enable;
			ForAllPages(x => x.CanFinish = isFinishButtonEnabled);
		}
		EventHandler finish;
		event EventHandler IWizardView.Finish {
			add { finish += value; }
			remove { finish -= value; }
		}
		readonly List<WeakReference> pages = new List<WeakReference>();
		internal void AddPage(DataSourceWizardPage page) {
			pages.Add(new WeakReference(page));
			page.CanGoForward = isNextButtonEnabled;
			page.CanFinish = isFinishButtonEnabled;
		}
		void ForAllPages(Action<DataSourceWizardPage> action) {
			foreach(var page in pages.Select(x => (DataSourceWizardPage)x.Target).ToList().Where(x => x != null))
				action(page);
		}
		readonly Locker navigateLock = new Locker();
		void IWizardView.SetPageContent(object content) {
			navigateLock.DoIfNotLocked(() => {
				Parameters.Controller.NavigateTo(content);
			});
		}
		void IWizardView.ShowError(string error) { }
		IEnumerable<SqlDataConnection> sqlDataConnections;
		protected IEnumerable<SqlDataConnection> SqlDataConnections {
			get {
				if(sqlDataConnections == null)
					sqlDataConnections = GetSqlDataConnections();
				return sqlDataConnections;
			}
		}
		protected abstract IEnumerable<SqlDataConnection> GetSqlDataConnections();
		protected IParameterService ParameterService { get { return parameterService; } }
		ISolutionTypesProvider solutionTypesProvider;
		protected ISolutionTypesProvider SolutionTypesProvider {
			get {
				if(solutionTypesProvider == null)
					solutionTypesProvider = GetSolutionTypesProvider();
				return solutionTypesProvider;
			}
		}
		protected abstract ISolutionTypesProvider GetSolutionTypesProvider();
		IDataConnectionParametersService dataConnectionParametersService;
		protected IDataConnectionParametersService DataConnectionParametersService {
			get {
				if(dataConnectionParametersService == null)
					dataConnectionParametersService = GetDataConnectionParametersService();
				return dataConnectionParametersService;
			}
		}
		protected abstract IDataConnectionParametersService GetDataConnectionParametersService();
		IConnectionStorageService connectionStorageService;
		protected IConnectionStorageService ConnectionStorageService {
			get {
				if(connectionStorageService == null)
					connectionStorageService = GetConnectionStorageService();
				return connectionStorageService;
			}
		}
		protected abstract IConnectionStorageService GetConnectionStorageService();
		IConnectionStringsProvider connectionStringsProvider;
		protected IConnectionStringsProvider ConnectionStringsProvider {
			get {
				if(connectionStringsProvider == null)
					connectionStringsProvider = GetConnectionStringsProvider();
				return connectionStringsProvider;
			}
		}
		protected abstract IConnectionStringsProvider GetConnectionStringsProvider();
		IDBSchemaProvider dbSchemaProvider;
		protected IDBSchemaProvider DBSchemaProvider {
			get {
				if(dbSchemaProvider == null)
					dbSchemaProvider = GetDBSchemaProvider();
				return dbSchemaProvider;
			}
		}
		protected abstract IDBSchemaProvider GetDBSchemaProvider();
		IExcelSchemaProvider excelSchemaProvider;
		protected IExcelSchemaProvider ExcelSchemaProvider {
			get {
				if(excelSchemaProvider == null)
					excelSchemaProvider = GetExcelSchemaProvider();
				return excelSchemaProvider;
			}
		}
		protected abstract IExcelSchemaProvider GetExcelSchemaProvider();
		IWizardRunnerContext wizardRunnerContext;
		protected IWizardRunnerContext WizardRunnerContext {
			get {
				if(wizardRunnerContext == null)
					wizardRunnerContext = GetWizardRunnerContext();
				return wizardRunnerContext;
			}
		}
		protected abstract IWizardRunnerContext GetWizardRunnerContext();
		ICustomQueryValidator customQueryValidator;
		protected ICustomQueryValidator CustomQueryValidator {
			get {
				if(customQueryValidator == null)
					customQueryValidator = GetCustomQueryValidator();
				return customQueryValidator;
			}
		}
		protected abstract ICustomQueryValidator GetCustomQueryValidator();
		protected void RegisterPage<T>(Func<T> pageFactory) where T : IWizardPage<IDataSourceModel> {
			pageFactory = pageFactory.Memoize();
			pagesFactories[typeof(T)] = () => pageFactory();
		}
		public void RegisterPage<TBase, TResult>(Func<TResult> pageFactory)
			where TBase : IWizardPage<IDataSourceModel>
			where TResult : TBase {
			pageFactory = pageFactory.Memoize();
			pagesFactories[typeof(TBase)] = () => pageFactory();
		}
		readonly Dictionary<Type, Func<IWizardPage<IDataSourceModel>>> pagesFactories = new Dictionary<Type, Func<IWizardPage<IDataSourceModel>>>();
		IWizardPage<IDataSourceModel> IWizardPageFactory<IDataSourceModel>.GetPage(Type pageType) {
			return pagesFactories[pageType]();
		}
		protected void RaiseFinish() {
			if(finish != null)
				finish(this, EventArgs.Empty);
		}
		sealed protected class WizardEngine : Wizard<IDataSourceModel> {
			public WizardEngine(DataSourceWizardModelBase owner, IDataSourceModel dataSourceModel) : base(owner, dataSourceModel ?? new DataSourceModel(), owner) { }
		}
	}
}
