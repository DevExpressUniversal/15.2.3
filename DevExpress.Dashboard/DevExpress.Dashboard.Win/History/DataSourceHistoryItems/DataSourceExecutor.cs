#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Entity;
using DevExpress.DataAccess;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon.DataSourceWizard;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Utils.Design;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Wizard.Presenters;
namespace DevExpress.DashboardWin.Native {
	public static class DataSourceExecutor {
		public static IEnumerable<ProviderLookupItem> CreateSqlProvidersList(DashboardDataSourceWizardSettings settings) {
			IEnumerable<ProviderLookupItem> providers = ProviderLookupItem.GetPredefinedItems();
			if(settings != null)
				providers = providers.Where(pli =>
								   pli.ProviderKey == "MSSqlServer" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.MSSqlServer) ||
								   pli.ProviderKey == "Access97" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Access) ||
								   pli.ProviderKey == "Access2007" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Access) ||
								   pli.ProviderKey == "MSSqlServerCE" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.MSSqlServerCE) ||
								   pli.ProviderKey == "Oracle" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Oracle) ||
								   pli.ProviderKey == "Amazon Redshift" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Redshift) ||
								   pli.ProviderKey == "BigQuery" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.BigQuery) ||
								   pli.ProviderKey == "Teradata" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Teradata) ||
								   pli.ProviderKey == "Firebird" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Firebird) ||
								   pli.ProviderKey == "DB2" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.DB2) ||
								   pli.ProviderKey == "MySql" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.MySql) ||
								   pli.ProviderKey == "Pervasive" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Pervasive) ||
								   pli.ProviderKey == "Postgres" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Postgres) ||
								   pli.ProviderKey == "Advantage" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Advantage) ||
								   pli.ProviderKey == "Ase" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Ase) ||
								   pli.ProviderKey == "Asa" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.Asa) ||
								   pli.ProviderKey == "SQLite" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.SQLite) ||
								   pli.ProviderKey == "VistaDB" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.VistaDB) ||
								   pli.ProviderKey == "VistaDB5" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.VistaDB) ||
								   pli.ProviderKey == "InMemorySetFull" && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.XmlFile) ||
								   pli.ProviderKey == DataConnectionParametersRepository.CustomConStrTag && settings.AvailableSqlDataProviders.HasFlag(DashboardSqlDataProvider.CustomConnectionString)).ToList();
			return providers;
		}
		public static bool RunNewDataConnectionWizard(IServiceProvider serviceProvider) {
			IDashboardDataSource dataSource = AddNewDataSource(serviceProvider);
			if (dataSource != null) {
				IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
				IDashboardParameterService parameterService = serviceProvider.RequestServiceStrictly<IDashboardParameterService>();
				ParameterChangesCollection parametersChanged = new ParameterChangesCollection(parameterService.ParameterCollection);
				NewDataSourceHistoryItem historyItem = new NewDataSourceHistoryItem(dataSource, parameterService.ParameterCollection, parametersChanged);
				historyService.RedoAndAdd(historyItem);
				return true;
			}
			return false;
		}		
		public static IDashboardDataSource AddNewDataSource(IServiceProvider serviceProvider) {
			var settingsProvider = serviceProvider.RequestService<IDashboardDataSourceWizardSettingsProvider>();
			DashboardDataSourceWizardSettings settings = settingsProvider.Settings;
			var client = CreateDataSourceWizardClient(serviceProvider);
			IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			var runner = new DashboardDataSourceWizardRunner<DashboardDataSourceModel>(guiContext.LookAndFeel, guiContext.Win32Window, settings != null ? settings.ShowDataSourceNamePage : false);
			IDashboardDataSourceWizardCustomization customization = serviceProvider.RequestService<IDashboardDataSourceWizardCustomization>();
			IDashboardDataSource dataSource = null;
			DashboardDataSourceModel dataSourceModel= new DashboardDataSourceModel();
			if(runner.Run(client, dataSourceModel, c => {
				DataSourceTypes dataSourceTypes = (DataSourceTypes)c.Resolve(typeof(DataSourceTypes));
				ISupportedDataSourceTypesService supportedDataSourceTypesService = (ISupportedDataSourceTypesService)c.Resolve(typeof(ISupportedDataSourceTypesService));
				if(!supportedDataSourceTypesService.IsObjectTypesSupported) {
					dataSourceTypes.Remove(DataSourceType.Object);
					dataSourceTypes.Remove(DataSourceType.Entity);
				}
				if(!supportedDataSourceTypesService.IsXmlSchemaTypeSupported)
					dataSourceTypes.Remove(DashboardDataSourceType.XmlSchema);
				if(settings.ShowDataSourceNamePage) {
					c.StartPage = typeof(DashboardChooseDataSourceNamePage<DashboardDataSourceModel>);
				}
				else if(dataSourceTypes.Count == 1) {
					var dataSourceType = dataSourceTypes.First();
					c.Model.DataSourceType = dataSourceType;
					IEnumerable<SqlDataConnection> dataConnections = (IEnumerable<SqlDataConnection>)c.Resolve(typeof(IEnumerable<SqlDataConnection>));
					c.StartPage = DashboardDataSourceTypeHelper.GetNextPageType<DashboardDataSourceModel>(dataSourceType, dataConnections.Any());
				}
				if(customization != null)
					customization.CustomizeDataSourceWizard(c);
			})) {
				SaveConnection((IDataComponentModelWithConnection)runner.WizardModel, serviceProvider);
				dataSource = (IDashboardDataSource)new DashboardDataComponentCreator().CreateDataComponent(runner.WizardModel);
			}
			return dataSource;
		}
		public static string GetOlapConnectionString(IServiceProvider serviceProvider, string connectionString) {
			var client = CreateDataSourceWizardClient(serviceProvider);
			IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			var runner = new DashboardOlapParametersWizardRunner<DashboardDataSourceModel>(guiContext.LookAndFeel, guiContext.Win32Window);
			DashboardDataSourceModel dataSourceModel = new DashboardDataSourceModel();
			dataSourceModel.DataSourceType = DashboardDataSourceType.Olap;
			OlapDataConnection dataConnection = new OlapDataConnection();
			dataConnection.ConnectionString = connectionString;
			((IDataComponentModelWithConnection)dataSourceModel).DataConnection = dataConnection;
			if(runner.Run(client, dataSourceModel)) {
				IDataConnection connection = ((IDataComponentModelWithConnection)runner.WizardModel).DataConnection;
				connectionString = ((OlapDataConnection)connection).ConnectionString;
			}
			return connectionString;
		}
		public static bool ShouldChangeDataProcessingMode(IServiceProvider serviceProvider) {
			IDashboardErrorMessageService errorMessageService = serviceProvider.RequestServiceStrictly<IDashboardErrorMessageService>();
			return errorMessageService.ShowMessage(
				DashboardWinLocalizer.GetString(DashboardWinStringId.MessageDataProcessingModeChanging),
				DashboardWinLocalizer.GetString(DashboardWinStringId.MessageBoxConfirmationTitle),
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.None) == DialogResult.OK;
		}
		public static void SaveConnection(IDataComponentModelWithConnection dataComponentModel, IServiceProvider serviceProvider) {
			DataComponentCreator.SaveConnectionIfShould(dataComponentModel, serviceProvider.RequestServiceStrictly<IConnectionStorageService>());
		}
		static DashboardDataSourceWizardClientUI CreateDataSourceWizardClient(IServiceProvider serviceProvider) {
			var connectionStorageService = serviceProvider.RequestServiceStrictly<IConnectionStorageService>();
			var parameterService = serviceProvider.RequestServiceStrictly<IParameterService>();
			var solutionTypesProvider = serviceProvider.RequestServiceStrictly<ISolutionTypesProvider>();
			var connectionStringsProvider = serviceProvider.RequestServiceStrictly<IConnectionStringsProvider>();
			var dbSchemaProviderFactory = serviceProvider.RequestServiceStrictly<IDBSchemaProviderFactory>();
			IDBSchemaProvider dbSchemaProvider = dbSchemaProviderFactory.CreateDBSchemaProvider();
			var dataSourceNameCreationService = serviceProvider.RequestServiceStrictly<IDataSourceNameCreationService>();
			var dataSourceTypesService = serviceProvider.RequestServiceStrictly<ISupportedDataSourceTypesService>();
			var DTEService = serviceProvider.RequestService<IDTEService>();
			var settingsProvider = serviceProvider.RequestServiceStrictly<IDashboardDataSourceWizardSettingsProvider>();
			DashboardDataSourceWizardSettings settings = settingsProvider.Settings;
			var dataSourceTypes = new DataSourceTypes();
			SqlWizardOptions options = settings.ToSqlWizardOptions();
			ICustomQueryValidator customQueryValidator = serviceProvider.RequestServiceStrictly<ICustomQueryValidator>();
			IEnumerable<ProviderLookupItem> providers = CreateSqlProvidersList(settings);
			if(settings != null) {
				if(settings.AvailableDataSourceTypes.HasFlag(DashboardDesignerDataSourceType.Sql))
					dataSourceTypes.Add(DataSourceType.Xpo);
				if(settings.AvailableDataSourceTypes.HasFlag(DashboardDesignerDataSourceType.EF))
					dataSourceTypes.Add(DataSourceType.Entity);
				if(settings.AvailableDataSourceTypes.HasFlag(DashboardDesignerDataSourceType.Object))
					dataSourceTypes.Add(DataSourceType.Object);
				if(settings.AvailableDataSourceTypes.HasFlag(DashboardDesignerDataSourceType.Excel))
					dataSourceTypes.Add(DataSourceType.Excel);
				if(settings.AvailableDataSourceTypes.HasFlag(DashboardDesignerDataSourceType.Olap))
					dataSourceTypes.Add(DashboardDataSourceType.Olap);
				dataSourceTypes.Add(DashboardDataSourceType.XmlSchema);
				if(settings.AlwaysSaveCredentials)
					options |= SqlWizardOptions.AlwaysSaveCredentials;
				if(settings.DisableNewConnections)
					options |= SqlWizardOptions.DisableNewConnections;
			}
			DashboardDataSourceWizardClientUI client = new DashboardDataSourceWizardClientUI(connectionStorageService, parameterService, solutionTypesProvider, connectionStringsProvider,
				dbSchemaProvider, dataSourceNameCreationService, dataSourceTypesService, DTEService, serviceProvider, options, dataSourceTypes, customQueryValidator);
			((ISqlDataSourceWizardClient)client).DataProviders.Clear();
			((ISqlDataSourceWizardClient)client).DataProviders.AddRange(providers);
			return client;
		}
	}
}
