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
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards3;
using DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages.DataSourcePresenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Wizards.Builder;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.Parameters;
using DevExpress.Data;
using System.Linq;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard {
	public class ReportWizardParameterService : IParameterService {
		readonly ParameterCollection parameterCollection;
		public ReportWizardParameterService(ParameterCollection parameterCollection) {
			this.parameterCollection = parameterCollection;
		}
		void IParameterService.AddParameter(IParameter parameter) {
			var configuredParameter = parameter as Parameter;
			parameterCollection.Add(new Parameter() {
				Name = configuredParameter.Name,
				Description = configuredParameter.Description,
				Type = parameter.Type,
				Value = parameter.Value
			});
		}
		bool IParameterService.CanCreateParameters {
			get { return true; }
		}
		string IParameterService.AddParameterString { get { return "New Report Parameter..."; } }
		IParameter IParameterService.CreateParameter(Type type) {
			return new Parameter() { Type = type, Name = ParametersHelper.GetNewName(parameterCollection, "parameter") };
		}
		string IParameterService.CreateParameterString { get { return "Report ParameterService"; } }
		IEnumerable<IParameter> IParameterService.Parameters { get { return parameterCollection; } }
	}
	public class ReportWizardModel {
		readonly XtraReport report;
		readonly bool enableCustomSql;
		public static ReportWizardModel Create(WizardModelParameters parameters, bool enableCustomSql) {
			return ViewModelSource.Create(() => new ReportWizardModel(parameters, enableCustomSql));
		}
		public readonly WizardModelParameters Parameters;
		readonly IParameterService parameterService;
		XtraReportModel reportModel;
		protected internal IParameterService ParameterService {
			get { return parameterService; }
		}
		public XtraReportModel ReportModel {
			get { return reportModel ?? (ReportModel = new XtraReportModel()); }
			internal set { reportModel = value; }
		}
		public event ReportWizardCompletedEventHandler ReportWizardCompleted;
		protected ReportWizardModel(WizardModelParameters parameters, bool enableCustomSql) {
			Parameters = parameters;
			report = new XtraReport();
			this.enableCustomSql = enableCustomSql;
			parameterService = new ReportWizardParameterService(report.Parameters);
		}
		protected internal virtual void StartDataSourceWizard(WizardController controller) {
			var parameters = new DataSourceWizardModelParameters(controller,
				Parameters.DoWithMessageBoxService,
				Parameters.DoWithSplashScreenService,
				Parameters.DoWithOpenFileDialogService,
				Parameters.DoWithQueryBuilderDialogService,
				Parameters.DoWithPreviewDialogService,
				Parameters.DoWithPasswordDialogService,
				Parameters.DoWithChooseEFStoredProceduresDialogService);
			var model = ReportDataSourceWizardModel.Create(this, parameters, enableCustomSql);
		}
		protected internal void Finish() {
			var builder = new ReportBuilder();
			builder.Build(report, ReportModel);
			if (ReportWizardCompleted != null)
				ReportWizardCompleted(this, new ReportWizardCompletedEventArgs(report));
		}
	}
	public delegate void ReportWizardCompletedEventHandler(object sender, ReportWizardCompletedEventArgs e);
	public class ReportWizardCompletedEventArgs : EventArgs {
		public XtraReport Result { get; private set; }
		public ReportWizardCompletedEventArgs(XtraReport result) {
			GuardHelper.ArgumentNotNull(result, "result");
			Result = result;
		}
	}
	public class ReportDataSourceWizardModel : DataSourceWizardModel {
		public static ReportDataSourceWizardModel Create(ReportWizardModel reportWizardModel, DataSourceWizardModelParameters parameters, bool enableCustomSql) {
			return ViewModelSource.Create(() => new ReportDataSourceWizardModel(reportWizardModel, parameters, enableCustomSql));
		}
		public ReportWizardModel ReportWizardModel { get; private set; }
		protected ReportDataSourceWizardModel(ReportWizardModel reportWizardModel, DataSourceWizardModelParameters parameters, bool enableCustomSql) 
			: base(parameters, enableCustomSql, null, reportWizardModel.ParameterService, reportWizardModel.ReportModel) {
			ReportWizardModel = reportWizardModel;
		}
		protected override void NavigateToNextPage() {
			base.NavigateToNextPage();
			ReportWizardModel.ReportModel = wizard.CurrentPage.Model as XtraReportModel;
		}
		protected override bool Finish() {
			bool completed = false;
			EventHandler onWizardCompleted = (s, e) => {
				ReportWizardModel.ReportModel = wizard.CurrentPage.Model as XtraReportModel;
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
		protected override void RegisterPages() {
			base.RegisterPages();
			RegisterPage(() => new ChooseDataSourceTypePage<IDataSourceModel>(ChooseDataSourceTypePageEx.Create(this), WizardRunnerContext,  SqlDataConnections, SolutionTypesProvider));
			RegisterPage(() => new ConfigureQueryPage<IDataSourceModel>(ConfigureSqlQueryPageEx.Create(this), WizardRunnerContext, DevExpress.DataAccess.Wizard.SqlWizardOptions.None, DBSchemaProvider, null, null));
			RegisterPage(() => new ConfigureSqlParametersPage<IDataSourceModel>(ConfigureSqlParametersPageEx.Create(this, reportParameters), WizardRunnerContext, null, null, DataConnectionParametersService, CustomQueryValidator));
			RegisterPage<ChooseEFConnectionStringPage<IDataSourceModel>, ChooseEFConnectionStringPagePresenter<IDataSourceModel>>(
				() => new ChooseEFConnectionStringPagePresenter<IDataSourceModel>(ChooseEFConnectionStringPage.Create(this), WizardRunnerContext, ConnectionStringsProvider, ConnectionStorageService));
			RegisterPage<ConfigureEFConnectionStringPage<IDataSourceModel>, ConfigureEFConnectionStringPageEx<IDataSourceModel>>(
				() => new ConfigureEFConnectionStringPageEx<IDataSourceModel>(ConfigureEFConnectionStringPage.Create(this), WizardRunnerContext, ConnectionStorageService));
			RegisterPage<ConfigureEFStoredProceduresPage<IDataSourceModel>, ConfigureEFStoredProceduresPageEx<IDataSourceModel>>(
				() => new ConfigureEFStoredProceduresPageEx<IDataSourceModel>(null, WizardRunnerContext, ParameterService));
			RegisterPage(()=> new ChooseObjectBindingModePage<IDataSourceModel>(ChooseObjectBindingModePageEx.Create(this), DevExpress.DataAccess.Wizard.OperationMode.Both));
			RegisterPage(() => new ChooseObjectConstructorPage<IDataSourceModel>(ChooseObjectConstructorPageEx.Create(this), DevExpress.DataAccess.Wizard.OperationMode.Both));
			RegisterPage(() => new ChooseObjectMemberPage<IDataSourceModel>(ChooseObjectMemberPageEx.Create(this), WizardRunnerContext, DevExpress.DataAccess.Wizard.OperationMode.Both));
			RegisterPage(() => new ChooseObjectTypePage<IDataSourceModel>(ChooseObjectTypePageEx.Create(this), WizardRunnerContext, DevExpress.DataAccess.Wizard.OperationMode.Both));
			RegisterPage(() => new ObjectConstructorParametersPage<IDataSourceModel>(ObjectConstructorParametersPageEx.Create(this, reportParameters)));
			RegisterPage(() => new ConfigureExcelFileColumnsPage<IDataSourceModel>(ConfigureExcelFileColumnsPageEx.Create(this)));
		}
	}
}
