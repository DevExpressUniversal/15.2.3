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
using DevExpress.DashboardCommon.DataSourceWizard;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataAccess.Wizard;
namespace DevExpress.DashboardWin.Native {
	public static class OlapDataSourceUIHelper {
		internal class OlapWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IDashboardDataSourceWizardClientUI> where TModel : IDataSourceModel, new() {
			Type startPage = typeof(ChooseOlapConnectionPage<TModel>);
			public OlapWizardRunner(IWizardRunnerContext context)
				: base(context) { }
			protected override Type StartPage { get { return startPage; } }
			protected override string WizardTitle { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionEditorTitle);; } }
			protected override WizardPageFactoryBase<TModel, IDashboardDataSourceWizardClientUI> CreatePageFactory(IDashboardDataSourceWizardClientUI client) {
				this.startPage = client.DataConnections.Count() > 0 ?
					typeof(ChooseOlapConnectionPage<TModel>) :
					typeof(ConfigureOlapParametersPage<TModel>);
				return new DashboardWizardPageFactory<TModel, IDashboardDataSourceWizardClientUI>(client);
			}
		}
		public static bool ConfigureConnection(this DashboardOlapDataSource olapDataSource, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(olapDataSource, "olapDataSource");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			DashboardDataSourceModel model = new DashboardDataSourceModel();
			model.DataSourceType = DashboardDataSourceType.Olap;
			((IDataComponentModelWithConnection)model).DataConnection = new OlapDataConnection(olapDataSource.ConnectionName, new OlapConnectionParameters(olapDataSource.Connection.ConnectionString));
			var guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			var wizardRunnerContext = new DefaultWizardRunnerContext(guiContext.LookAndFeel, guiContext.Win32Window);
			var runner = new OlapWizardRunner<DashboardDataSourceModel>(wizardRunnerContext);
			var connectionStorageService = serviceProvider.RequestService<IConnectionStorageService>();
			if (connectionStorageService == null)
				connectionStorageService = olapDataSource.ConnectionStorageService;
			var connectionStringsProvider = serviceProvider.RequestService<IConnectionStringsProvider>();
			if (connectionStringsProvider == null)
				connectionStringsProvider = olapDataSource.ConnectionStringsProvider;
			var client = new DashboardDataSourceWizardClientUI(connectionStorageService, connectionStringsProvider);
			if(runner.Run(client, model)) {
				IDataComponentModelWithConnection dataComponentModel = (IDataComponentModelWithConnection)runner.WizardModel;
				IDataConnection dataConnection = dataComponentModel.DataConnection;
				DataSourceExecutor.SaveConnection(dataComponentModel, serviceProvider);
				olapDataSource.ConnectionName = ((IDataComponentModelWithConnection)runner.WizardModel).ConnectionName;
				olapDataSource.StoreConnectionNameOnly = dataConnection.StoreConnectionNameOnly;
				if(!dataConnection.StoreConnectionNameOnly)
					olapDataSource.ConnectionString = ((OlapDataConnection)dataConnection).ConnectionString;
				return true;
			}
			return false;
		}
	}
}
