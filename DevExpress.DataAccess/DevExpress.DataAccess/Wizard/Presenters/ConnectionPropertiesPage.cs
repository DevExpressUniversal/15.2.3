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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConnectionPropertiesPage<TModel> : WizardPageBase<IConnectionPropertiesPageView, TModel> 
		where TModel : ISqlDataSourceModel 
	{
		readonly IWizardRunnerContext context;
		readonly SqlWizardOptions options;
		SqlDataConnection dataConnection;
		public ConnectionPropertiesPage(IConnectionPropertiesPageView view, IWizardRunnerContext context, IDataConnectionParametersService dataConnectionParametersService, IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections, SqlWizardOptions options)
			: base(view) {
			this.context = context;
			this.options = options;
			DataConnectionParametersProvider = dataConnectionParametersService;
			ConnectionStorageService = connectionStorageService;
			DataConnections = dataConnections;
		}
		public override bool FinishEnabled { get { return false; } }
		public override bool MoveNextEnabled {
			get {
				return true;
			}
		}
		protected DataConnectionParametersBase ModelDataConnectionParameters { get { return Model.DataConnection != null ? Model.DataConnection.CreateDataConnectionParameters() : null; } }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Connection); } }
		IDataConnectionParametersService DataConnectionParametersProvider { get; set; }
		IEnumerable<SqlDataConnection> DataConnections { get; set; }
		DataConnectionParametersBase DataConnectionParameters { get { return dataConnection != null ? dataConnection.CreateDataConnectionParameters() : null; } }
		IConnectionStorageService ConnectionStorageService { get; set; }
		public override void Begin() {
			View.InitControls();
			View.ConnectionName = Model.ConnectionName;
			View.SetConnections(DataConnections);
			View.Changed += ViewOnChanged;
			if(Model.DataConnection != null) {
				dataConnection = Model.DataConnection;
				View.DataConnectionParameters = Model.DataConnection.CreateDataConnectionParameters();
			}
		}
		void ViewOnChanged(object sender, EventArgs eventArgs) { RaiseChanged(); }
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			DataConnectionParametersBase dataConnectionParametersBase = View.DataConnectionParameters;
			if(DataConnectionParametersComparer.Equals(ModelDataConnectionParameters, dataConnectionParametersBase))
				return OpenConnection(dataConnection);
			SqlDataConnection newConnection = new SqlDataConnection(View.ConnectionName, dataConnectionParametersBase);
			if(OpenConnection(newConnection)) {
				if(dataConnection != null)
					dataConnection.Close();
				dataConnection = newConnection;
				return true;
			}
			newConnection.Dispose();
			return false;
		}
		public override void Commit() {
			if(!Equals(ModelDataConnectionParameters, DataConnectionParameters))
				Model.Query = null;
			Model.ConnectionName = View.ConnectionName;
			Model.DataConnection = dataConnection;
			Model.ShouldSaveConnection = ConnectionStorageService.CanSaveConnection ? SaveConnectionMethod.SaveToAppConfig : SaveConnectionMethod.Hardcode;
			if(AlwaysSaveCredentials)
				Model.ShouldSaveConnection |= SaveConnectionMethod.KeepCredentials;
		}
		public override Type GetNextPageType() {
			IConnectionPage dataConnectionParametersBase = View.DataConnectionParameters;
			if(ConnectionStorageService.CanSaveConnection)
				return typeof(SaveConnectionPage<TModel>);
			if(AlwaysSaveCredentials)
				return typeof(ConfigureQueryPage<TModel>);
			if(dataConnectionParametersBase != null && !dataConnectionParametersBase.AuthType && !(dataConnectionParametersBase is XmlFileConnectionParameters))
				return typeof(SaveConnectionPage<TModel>);
			return typeof(ConfigureQueryPage<TModel>);
		}
		protected bool AlwaysSaveCredentials { get { return options.HasFlag(SqlWizardOptions.AlwaysSaveCredentials); } }
#if DEBUGTEST
		protected virtual 
#endif
		bool OpenConnection(SqlDataConnection connection) {
			return ConnectionHelper.OpenConnection(connection, ExceptionHandler, WaitFormActivator, DataConnectionParametersProvider);
		}
	}
}
