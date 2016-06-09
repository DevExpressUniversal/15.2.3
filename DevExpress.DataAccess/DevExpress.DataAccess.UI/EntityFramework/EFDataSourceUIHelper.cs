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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.DataAccess.UI.EntityFramework {
	public static class EFDataSourceUIHelper {
		#region inner classes
		class ConfigureWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IEFDataSourceWizardClientUI> where TModel : IEFDataSourceModel, new() {
			public ConfigureWizardRunner(IWizardRunnerContext context) : base(context) {
			}
			protected override Type StartPage {
				get {
					return typeof(ChooseEFContextPage<TModel>);
				}
			}
			protected override string WizardTitle {
				get {
					return DataAccessUILocalizer.GetString(DataAccessUIStringId.EFDataSourceEditorTitle);
				}
			}
			protected override WizardPageFactoryBase<TModel, IEFDataSourceWizardClientUI> CreatePageFactory(IEFDataSourceWizardClientUI client) {
				return new EFWizardPageFactory<TModel, IEFDataSourceWizardClientUI>(client);
			}
		}
		class EditConnectionWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IEFDataSourceWizardClientUI> where TModel : IEFDataSourceModel, new() {
			public EditConnectionWizardRunner(IWizardRunnerContext context) : base(context) {
			}
			protected override Type StartPage {
				get {
					return typeof(ChooseEFContextPage<TModel>);
				}
			}
			protected override string WizardTitle {
				get {
					return DataAccessUILocalizer.GetString(DataAccessUIStringId.EFConnectionEditorTitle);
				}
			}
			protected override WizardPageFactoryBase<TModel, IEFDataSourceWizardClientUI> CreatePageFactory(IEFDataSourceWizardClientUI client) {
				return new EditConnectionWizardPageFactory<TModel>(client);
			}
		}
		class EditConnectionWizardPageFactory<TModel> : EFWizardPageFactory<TModel, IEFDataSourceWizardClientUI> where TModel : IEFDataSourceModel, new() {
			public EditConnectionWizardPageFactory(IEFDataSourceWizardClientUI client)
				: base(client) {
			}
			protected override void RegisterDependencies(IEFDataSourceWizardClientUI client) {
				base.RegisterDependencies(client);
				Container.RegisterType<ChooseEFContextPage<TModel>>();
				Container.RegisterType<ChooseEFConnectionStringPage<TModel>, ChooseEFConnectionStringPageEx<TModel>>();
				Container.RegisterType<ConfigureEFConnectionStringPage<TModel>, ConfigureEFConnectionStringPageEx<TModel>>();
			}
		}
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is created through late-bound reflection methods such as CreateInstance.")]
		class ChooseEFConnectionStringPageEx<TModel> : ChooseEFConnectionStringPage<TModel>
			where TModel : IEFDataSourceModel {
			public ChooseEFConnectionStringPageEx(IChooseEFConnectionStringPageView view, IWizardRunnerContext context, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService)
				: base(view, context, connectionStringsProvider, connectionStorageService) {
			}
			public override bool MoveNextEnabled { get { return ShouldShowConnectionPropertiesPage; } }
			public override bool FinishEnabled { get { return !ShouldShowConnectionPropertiesPage; } }
		}
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is created through late-bound reflection methods such as CreateInstance.")]
		class ConfigureEFConnectionStringPageEx<TModel> : ConfigureEFConnectionStringPage<TModel>
			where TModel : IEFDataSourceModel {
			public ConfigureEFConnectionStringPageEx(IConfigureEFConnectionStringPageView view, IWizardRunnerContext context, IConnectionStorageService connectionStorageService)
				: base(view, context, connectionStorageService) {
			}
			public override bool MoveNextEnabled { get { return false; } }
			public override bool FinishEnabled { get { return true; } }
		}
		#endregion        
		public static EntityFrameworkModelHelper CreateEntityFrameworkModelHelper(UserLookAndFeel lookAndFeel, IWin32Window owner, ISolutionTypesProvider solutionTypesProvider) {
			return CreateEntityFrameworkModelHelper(new DefaultWizardRunnerContext(lookAndFeel, owner), solutionTypesProvider);
		}
		public static EntityFrameworkModelHelper CreateEntityFrameworkModelHelper(IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider) {
			IExceptionHandler exceptionHandler = context.CreateExceptionHandler(ExceptionHandlerKind.Default);
			IWaitFormActivator waitFormActivator = context.WaitFormActivator;
			return EntityFrameworkModelHelper.Create(solutionTypesProvider, exceptionHandler, waitFormActivator);
		}
		public static bool Configure(this EFDataSource dataSource, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService, IParameterService parameterService) {
			return Configure(dataSource, context, solutionTypesProvider, connectionStringsProvider, connectionStorageService, parameterService, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool Configure(this EFDataSource dataSource, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			return Configure(dataSource, context, solutionTypesProvider, connectionStringsProvider, connectionStorageService, parameterService, repositoryItemsProvider, _ => { });
		}
		public static bool Configure(this EFDataSource dataSource, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<EFDataSourceModel>> callback) {
			Guard.ArgumentNotNull(dataSource, "dataSource");			
			EntityFrameworkModelHelper modelHelper = CreateEntityFrameworkModelHelper(context, solutionTypesProvider);
			if(modelHelper == null)
				return false;
			EFDataSourceModel model = new EFDataSourceModel(dataSource.Connection) {
				StoredProceduresInfo = dataSource.StoredProcedures.ToArray(),
				ModelHelper = modelHelper
			};
			DataSourceWizardRunnerBase<EFDataSourceModel, IEFDataSourceWizardClientUI> runner = new ConfigureWizardRunner<EFDataSourceModel>(context);
			EFDataSourceWizardClientUI dataSourceWizardClient = new EFDataSourceWizardClientUI(parameterService, solutionTypesProvider, connectionStringsProvider, connectionStorageService) { RepositoryItemsProvider = repositoryItemsProvider };
			if(!runner.Run(dataSourceWizardClient, model, callback)) 
				return false;
			EFDataSourceModel wizardModel = runner.WizardModel;
			EFDataConnection dataConnection = wizardModel.DataConnection;
			dataSource.ConnectionParameters = dataConnection.ConnectionParameters;
			dataSource.Connection.ConnectionStringsProvider = dataConnection.ConnectionStringsProvider;
			dataSource.Connection.SolutionTypesProvider = dataConnection.SolutionTypesProvider;
			dataSource.StoredProcedures.Clear();
			if(wizardModel.StoredProceduresInfo != null)
				dataSource.StoredProcedures.AddRange(wizardModel.StoredProceduresInfo);
			DataComponentCreator.SaveConnectionIfShould(wizardModel, connectionStorageService);
			return true;
		}
		public static bool EditConnection(this EFDataSource dataSource, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService, IParameterService parameterService) {
			return EditConnection(dataSource, context, solutionTypesProvider, connectionStringsProvider, connectionStorageService, parameterService, _ => { });
		}
		public static bool EditConnection(this EFDataSource dataSource, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService, IParameterService parameterService, Action<IWizardCustomization<EFDataSourceModel>> callback) {
			Guard.ArgumentNotNull(dataSource, "dataSource");			
			EntityFrameworkModelHelper modelHelper = CreateEntityFrameworkModelHelper(context, solutionTypesProvider);
			if(modelHelper == null)
				return false;
			EFDataSourceModel model = dataSource.Connection != null ? new EFDataSourceModel(dataSource.Connection) : new EFDataSourceModel();
			model.ModelHelper = modelHelper;
			EditConnectionWizardRunner<EFDataSourceModel> runner = new EditConnectionWizardRunner<EFDataSourceModel>(context);
			EFDataSourceWizardClientUI client = new EFDataSourceWizardClientUI(parameterService, solutionTypesProvider, connectionStringsProvider, connectionStorageService);
			if(!runner.Run(client, model, callback))
				return false;
			EFDataSourceModel wizardModel = runner.WizardModel;
			EFDataConnection dataConnection = wizardModel.DataConnection;
			dataSource.ConnectionParameters = dataConnection.ConnectionParameters;
			if(dataSource.Connection != null) {
				dataSource.Connection.ConnectionStringsProvider = dataConnection.ConnectionStringsProvider;
				dataSource.Connection.SolutionTypesProvider = dataConnection.SolutionTypesProvider;
			}
			DataComponentCreator.SaveConnectionIfShould(wizardModel, connectionStorageService);
			return true;
		}
	}
}
