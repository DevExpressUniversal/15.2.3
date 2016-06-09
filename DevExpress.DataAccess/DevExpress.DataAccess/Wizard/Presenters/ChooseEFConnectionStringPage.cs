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
using DevExpress.Data.Entity;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseEFConnectionStringPage<TModel> : WizardPageBase<IChooseEFConnectionStringPageView, TModel>
		where TModel : IEFDataSourceModel {
		#region Inner classes
		public class ConnectionStringNamedItemWrapper : IConnectionStringInfo, INamedItem {
			public class EqualityComparer : IEqualityComparer<ConnectionStringNamedItemWrapper> {
				#region IEqualityComparer<ConnectionStringNamedItemWrapper> Members
				public bool Equals(ConnectionStringNamedItemWrapper x, ConnectionStringNamedItemWrapper y) {
					if(x.Location != y.Location)
						return false;
					if(!string.Equals(x.Name, y.Name, StringComparison.InvariantCulture))
						return false;
					if(!string.Equals(x.ProviderName, y.ProviderName, StringComparison.InvariantCulture))
						return false;
					if(!string.Equals(x.RunTimeConnectionString, y.RunTimeConnectionString, StringComparison.InvariantCulture))
						return false;
					return true;
				}
				public int GetHashCode(ConnectionStringNamedItemWrapper obj) {
					return obj.Name != null ? obj.Name.GetHashCode() : 0;
				}
				#endregion
			}
			IConnectionStringInfo source;
			public DataConnectionLocation Location {
				get { return this.source.Location; }
			}
			public string Name {
				get { return this.source.Name; }
			}
			public string ProviderName {
				get { return this.source.ProviderName; }
			}
			public string RunTimeConnectionString {
				get { return this.source.RunTimeConnectionString; }
			}
			string INamedItem.Name {
				get {
					if(this.source.Location == DataConnectionLocation.ServerExplorer)
						return string.Format("{0}{1}", this.source.Name, DataAccessLocalizer.GetString(DataAccessStringId.ConnectionStringPostfixServerExplorer));
					return this.source.Name;
				}
				set { throw new NotSupportedException(); }
			}
			public ConnectionStringNamedItemWrapper(IConnectionStringInfo connectionStringInfo) {
				this.source = connectionStringInfo;
			}
		}
		#endregion
		readonly IWizardRunnerContext context;
		protected bool shouldChooseStoredProcedure = false;
		protected List<ConnectionStringNamedItemWrapper> connections;
		EFDataConnection dataConnection;
		public override bool MoveNextEnabled { get { return ShouldShowNextPage; } }
		public override bool FinishEnabled { get { return !ShouldShowNextPage; } }
		IConnectionStorageService ConnectionStorageService { get; set; }
		IConnectionStringInfo SelectedConnectionInfo {
			get { return this.connections.SingleOrDefault(c => ((INamedItem)c).Name == View.ExistingConnectionName); }
		}
		protected bool ShouldShowConnectionPropertiesPage {
			get { return View.ShouldCreateNewConnection || SelectedConnectionInfo.Location == DataConnectionLocation.ServerExplorer && ConnectionStorageService.CanSaveConnection; }
		}
		protected bool ShouldShowNextPage {
			get {
				return ShouldShowConnectionPropertiesPage || this.shouldChooseStoredProcedure;
			}
		}
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Connection); } }
		IConnectionStringsProvider ConnectionStringsProvider { get; set; }
		public ChooseEFConnectionStringPage(IChooseEFConnectionStringPageView view, IWizardRunnerContext context, IConnectionStringsProvider connectionStringsProvider, IConnectionStorageService connectionStorageService)
			: base(view) {
			this.context = context;
			ConnectionStringsProvider = connectionStringsProvider;
			ConnectionStorageService = connectionStorageService;
		}
		public override void Begin() {
			View.Initialize();
			this.dataConnection = Model.DataConnection;
			View.Changed -= View_Changed;
			if(ConnectionStringsProvider != null) {
				IEnumerable<IConnectionStringInfo> connectionInfos = ConnectionStringsProvider.GetConnections().Union(ConnectionStringsProvider.GetConfigFileConnections());
				this.connections = connectionInfos.Select(csi => new ConnectionStringNamedItemWrapper(csi)).Distinct(new ConnectionStringNamedItemWrapper.EqualityComparer()).ToList();
				if(this.connections.Count > 0) {
					View.SetConnections(this.connections);
					if(Model.ConnectionStringLocation == DataConnectionLocation.SettingsFile || Model.ConnectionStringLocation == DataConnectionLocation.ServerExplorer) {
						INamedItem connection = this.connections.FirstOrDefault(c => c.Name == Model.ConnectionParameters.ConnectionStringName);
						if(connection != null)
							View.SetSelectedConnection(connection);
					} else {
						if(!string.IsNullOrEmpty(Model.ConnectionParameters.ConnectionString))
							View.SetSelectedConnection(null);
						else
							ApplySelectedConnectionString();
					}
				}
			}
			this.shouldChooseStoredProcedure = this.dataConnection.SourceMethods.Length > 0;
			RaiseChanged();
			View.Changed += View_Changed;
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			if(View.ShouldCreateNewConnection)
				return true;
			EmptyExceptionHandler exceptionHandler = new EmptyExceptionHandler();
			EFDataConnection oldConnection = this.dataConnection;
			if(oldConnection.IsConnected && EFConnectionParameters.EqualityComparer.Equals(oldConnection.ConnectionParameters, Model.ConnectionParameters))
				return true;
			EFDataConnection newConnection = new EFDataConnection(Model.ConnectionName, new EFConnectionParameters(Model.ConnectionParameters)) {
				SolutionTypesProvider = oldConnection.SolutionTypesProvider,
				ConnectionStringsProvider = oldConnection.ConnectionStringsProvider,
			};
			if(ConnectionHelper.OpenConnection(newConnection, exceptionHandler, WaitFormActivator)) {
				oldConnection.Close();
				this.dataConnection = newConnection;
			} else {
				Exception exception = new InvalidOperationException("Cannot open a connection");
				ExceptionHandler.HandleException(exception);
				return false;
			}
			if(exceptionHandler.Exception != null) {
				ExceptionHandler.HandleException(exceptionHandler.Exception);
				return false;
			}
			return true;
		}
		public override void Commit() {
			if(!EFConnectionParameters.EqualityComparer.Equals(Model.ConnectionParameters, Model.DataConnection.ConnectionParameters))
				Model.StoredProceduresInfo = null;
			Model.DataConnection = this.dataConnection;
			if(FinishEnabled)
				Model.StoredProceduresInfo = null;
		}
		public override Type GetNextPageType() {
			return ShouldShowConnectionPropertiesPage 
				? typeof(ConfigureEFConnectionStringPage<TModel>) 
				: typeof(ConfigureEFStoredProceduresPage<TModel>);
		}
		void View_Changed(object sender, EventArgs e) {
			if(!View.ShouldCreateNewConnection)
				ApplySelectedConnectionString();
			else {
				Model.ConnectionParameters.ConnectionString = string.Empty;
				Model.ConnectionParameters.ConnectionStringName = string.Empty;
				Model.ConnectionStringLocation = DataConnectionLocation.None;
			}
			RaiseChanged();
		}
		void ApplySelectedConnectionString() {
			IConnectionStringInfo info = SelectedConnectionInfo;
			if(info.Location == DataConnectionLocation.ServerExplorer) {
				Model.ConnectionParameters.ConnectionString = info.RunTimeConnectionString;
				Model.ConnectionParameters.ConnectionStringName = string.Empty;
			} else {
				Model.ConnectionParameters.ConnectionString = string.Empty;
				Model.ConnectionParameters.ConnectionStringName = info.Name;
			}
			Model.ConnectionName = info.Name;
			Model.ConnectionStringLocation = info.Location;
		}
	}
}
