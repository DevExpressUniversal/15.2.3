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
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseConnectionPage<TModel> : WizardPageBase<IChooseConnectionPageView, TModel> 
		where TModel : ISqlDataSourceModel 
	{
		readonly SqlWizardOptions options;
		readonly IWizardRunnerContext context;
		public override bool FinishEnabled { get { return false; } }
		public override bool MoveNextEnabled { get { return true; } }
		protected IEnumerable<SqlDataConnection> DataConnections { get; private set; }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Connection); } }
		IDataConnectionParametersService DataConnectionParametersService { get; set; }
		IConnectionStorageService ConnectionStorageService { get; set; }
		public ChooseConnectionPage(IChooseConnectionPageView view, IWizardRunnerContext context, IDataConnectionParametersService dataConnectionParametersService, 
			IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections, SqlWizardOptions options)
			: base(view) {
			this.context = context;
			this.options = options;
			DataConnectionParametersService = dataConnectionParametersService;
			ConnectionStorageService = connectionStorageService;
			DataConnections = dataConnections;
		}
		protected SqlDataConnection DataConnection {
			get { return DataConnections.First(c => ((INamedItem)c).Name == View.ExistingConnectionName); }
		}
		protected bool AlwaysSaveCredentials { get { return options.HasFlag(SqlWizardOptions.AlwaysSaveCredentials); } }
		protected bool DisableNewConnections { get { return options.HasFlag(SqlWizardOptions.DisableNewConnections); } }
		public override void Begin() {
			View.Changed += View_Changed;
			View.SetSelectedConnection(Model.DataConnection);
		}
		void View_Changed(object sender, EventArgs e) {
			RaiseChanged();
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			if(View.ShouldCreateNewConnection)
				return true;
			return ConnectionHelper.OpenConnection(DataConnection, ExceptionHandler, WaitFormActivator, DataConnectionParametersService);
		}
		public override void Commit() {
			View.Changed -= View_Changed;
			if(View.ShouldCreateNewConnection && Model.DataConnection != null && Model.DataConnection.StoreConnectionNameOnly)
				Model.DataConnection = new SqlDataConnection(Model.DataConnection.Name, Model.DataConnection.CreateDataConnectionParameters());
			SqlDataConnection dataConnection = DataConnection;
			if(dataConnection == null)
				return;
			if(!View.ShouldCreateNewConnection) {
				Model.DataConnection = dataConnection;
				Model.ConnectionName = dataConnection.Name;
				Model.ShouldSaveConnection = dataConnection.StoreConnectionNameOnly || ConnectionStorageService.CanSaveConnection
					? SaveConnectionMethod.SaveToAppConfig
					: SaveConnectionMethod.Hardcode;
				if(AlwaysSaveCredentials)
					Model.ShouldSaveConnection |= SaveConnectionMethod.KeepCredentials;
			}
		}
		public override Type GetNextPageType() {
			if(!DisableNewConnections && View.ShouldCreateNewConnection)
				return typeof(ConnectionPropertiesPage<TModel>);
			SqlDataConnection dataConnection = DataConnection;
			if(dataConnection != null && dataConnection.StoreConnectionNameOnly)
				return typeof(ConfigureQueryPage<TModel>);
			if(ConnectionStorageService.CanSaveConnection)
				return typeof(SaveConnectionPage<TModel>);
			if(!AlwaysSaveCredentials && dataConnection != null && !((IConnectionPage)dataConnection.ConnectionParameters).AuthType && dataConnection.ProviderKey != InMemoryDataStore.XpoProviderTypeString)
				return typeof(SaveConnectionPage<TModel>);
			return typeof(ConfigureQueryPage<TModel>);
		}
	}
}
