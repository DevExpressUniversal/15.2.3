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

using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel.DesignService;
using System.Linq;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.EntityFramework;
using System;
using System.Reflection;
using DevExpress.XtraReports.Configuration;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.Wizard;
namespace DevExpress.XtraReports.Wizards3 {
	public class XtraReportModelUI {
		readonly IConnectionStorageService connectionService;
		public XtraReportModelUI(IConnectionStorageService connectionService) {
			this.connectionService = connectionService;
		}
		public XtraReportModel CreateReportModel(IServiceProvider serviceProvider) {
			byte[] key = GetType().Assembly.GetName().GetPublicKeyToken();
			ISolutionTypesProvider solutionTypesProvider = new RuntimeSolutionTypesProvider(() => EntityServiceHelper.GetTypes(Assembly.GetEntryAssembly(), type => {
				byte[] key2 = type.Assembly.GetName().GetPublicKeyToken();
				return !key.SequenceEqual<byte>(key2);
			}));
			IConnectionStringsProvider connectionStringsProvider = new RuntimeConnectionStringsProvider();
			IParameterService parameterService = serviceProvider.GetService(typeof(IParameterService)) as IParameterService;
			ILookAndFeelService lookAndFeelServ = serviceProvider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			var dbSchemaProvider = (IDBSchemaProvider)serviceProvider.GetService(typeof(IDBSchemaProvider)) ?? new DBSchemaProvider();
			var repositoryItemsProvider = (IRepositoryItemsProvider)serviceProvider.GetService(typeof(IRepositoryItemsProvider));
			SqlWizardOptions options = (serviceProvider.GetService(typeof(ISqlWizardOptionsProvider)) as ISqlWizardOptionsProvider).GetSqlWizardOptions();
			var client = new DataSourceWizardClientUI(connectionService ?? new ConnectionStorageService(),
				parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider) {
				RepositoryItemsProvider = repositoryItemsProvider,
				PropertyGridServices = serviceProvider,
				DataSourceTypes = DataSourceTypes.All,
				Options = options
			};
			ICustomQueryValidator validator = (ICustomQueryValidator)serviceProvider.GetService(typeof(ICustomQueryValidator));
			if(validator != null)
				client.CustomQueryValidator = validator;
			DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeelServ.LookAndFeel, owner);
			var runner = new XtraReportWizardRunner<XtraReportModel, IDataSourceWizardClientUI>(wizardRunnerContext);
			IWizardCustomizationService serv = serviceProvider.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
			if (runner.Run(client, new XtraReportModel(), customization => {
				serv.CustomizeReportWizardSafely(customization);
			}))
				return runner.WizardModel;
			return null;
		}
	}
}
