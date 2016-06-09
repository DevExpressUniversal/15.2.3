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

using DevExpress.DashboardWin.Native;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.DataSourceWizard {
	public class ChooseOlapConnectionPage<TModel> : WizardPageBase<IChooseConnectionPageView, TModel>
		where TModel : IDataSourceModel {
		SqlDataConnection dataConnection;
		public override bool FinishEnabled { 
			get {
				return !View.ShouldCreateNewConnection;
			}
		}
		public override bool MoveNextEnabled { get { return true; } }
		IDataConnectionParametersService DataConnectionParametersService { get; set; }
		IConnectionStorageService ConnectionStorageService { get; set; }
		protected IEnumerable<SqlDataConnection> DataConnections { get; private set; }
		public ChooseOlapConnectionPage(IChooseConnectionPageView view, IDataConnectionParametersService dataConnectionParametersService,
			IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections)
			: base(view) {
			DataConnectionParametersService = dataConnectionParametersService;
			ConnectionStorageService = connectionStorageService;
			DataConnections = dataConnections;
		}
		protected SqlDataConnection DataConnection { get { return dataConnection; } set { dataConnection = value; } }
		public override void Begin() {
			View.Changed += View_Changed;
			IDataConnection dataConnection = ((IDataComponentModelWithConnection)Model).DataConnection;
			View.SetSelectedConnection((OlapDataConnection)dataConnection);
		}
		void View_Changed(object sender, EventArgs e) {
			RaiseChanged();
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			if(View.ShouldCreateNewConnection)
				return true;
			DataConnection = DataConnections.First(c => ((DataAccess.Native.INamedItem)c).Name == View.ExistingConnectionName);
			OlapConnectionparametersControl olapControl = new OlapConnectionparametersControl();
			olapControl.ApplyParameters(this.dataConnection.ConnectionString);
			OlapDataConnection connection = new OlapDataConnection(dataConnection.Name, new OlapConnectionParameters(olapControl.GetConnectionString()));
			try {
				connection.CreateDataStore(DataConnectionParametersService);
			} catch {
				return false;
			}
			return connection.IsConnected;
		}
		public override void Commit() {
			View.Changed -= View_Changed;
			if(!View.ShouldCreateNewConnection && this.dataConnection != null) {
				((IDataComponentModelWithConnection)Model).DataConnection = new OlapDataConnection(this.dataConnection.Name) { StoreConnectionNameOnly = true };
				Model.ConnectionName = dataConnection.Name;
			}
		}
		public override Type GetNextPageType() {
			if(Model.DataSourceType == DashboardDataSourceType.Olap) {
				if(View.ShouldCreateNewConnection)
					return typeof(ConfigureOlapParametersPage<TModel>);
				if(ConnectionStorageService.CanSaveConnection && this.dataConnection != null && !this.dataConnection.StoreConnectionNameOnly)
					return typeof(SaveOlapConnectionPage<TModel>);
			}
			return base.GetNextPageType();
		}
	}
}
