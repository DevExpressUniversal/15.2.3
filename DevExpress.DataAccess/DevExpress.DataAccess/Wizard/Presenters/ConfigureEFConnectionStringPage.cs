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
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConfigureEFConnectionStringPage<TModel> : WizardPageBase<IConfigureEFConnectionStringPageView, TModel>
		where TModel : IEFDataSourceModel {
		readonly IWizardRunnerContext context;
		EFDataConnection dataConnection;
		protected bool shouldChooseStoredProcedure = false;
		public ConfigureEFConnectionStringPage(IConfigureEFConnectionStringPageView view, IWizardRunnerContext context, IConnectionStorageService connectionStorageService)
			: base(view) {
			this.context = context;
			ConnectionStorageService = connectionStorageService;
		}
		public override bool FinishEnabled {
			get { return !this.shouldChooseStoredProcedure; }
		}
		public override bool MoveNextEnabled {
			get { return this.shouldChooseStoredProcedure; }
		}
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Connection); } }
		IConnectionStorageService ConnectionStorageService { get; set; }
		public override void Begin() {
			this.dataConnection = Model.DataConnection;
			View.ConnectionName = this.dataConnection.Name;
			View.SetCanSaveToStorage(ConnectionStorageService.CanSaveConnection);
			string connectionString = this.dataConnection.ConnectionParameters.ConnectionString;
			if(this.dataConnection != null && !string.IsNullOrEmpty(connectionString))
				View.ConnectionString = connectionString;
			this.shouldChooseStoredProcedure = this.dataConnection.SourceMethods.Length > 0;
			View.ShouldSaveConnectionString = false;
			View.ConnectionParametersChanged += OnViewConnectionParametersChanged;
			RaiseChanged();
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			if(View.ShouldSaveConnectionString) {
				if(string.IsNullOrWhiteSpace(View.ConnectionName)) {
					errorMessage = DataAccessLocalizer.GetString(DataAccessStringId.WizardEmptyConnectionNameMessage);
					ShowMessage(errorMessage);
					return false;
				}
				if(ConnectionStorageService.Contains(View.ConnectionName)) {
					errorMessage = DataAccessLocalizer.GetString(DataAccessStringId.WizardDataConnectionNameExistsMessage);
					ShowMessage(errorMessage);
					return false;
				}
			}
			EFDataConnection oldConnection = this.dataConnection;
			EFDataConnection newConnection = new EFDataConnection(Model.ConnectionParameters.ConnectionStringName, new EFConnectionParameters(Model.ConnectionParameters)) {
				SolutionTypesProvider = oldConnection.SolutionTypesProvider,
				ConnectionStringsProvider = oldConnection.ConnectionStringsProvider
			};
			if(ConnectionHelper.OpenConnection(newConnection, ExceptionHandler, WaitFormActivator)) {
				oldConnection.Close();
				this.dataConnection = newConnection;
				return true;
			}
			newConnection.Dispose();
			return false;
		}
		public override void Commit() {
			if(!EFConnectionParameters.EqualityComparer.Equals(Model.ConnectionParameters, Model.DataConnection.ConnectionParameters))
				Model.StoredProceduresInfo = null;
			Model.DataConnection = this.dataConnection;
			if(View.ShouldSaveConnectionString) {
				Model.ConnectionName = View.ConnectionName;
				Model.ShouldSaveConnection = SaveConnectionMethod.SaveToAppConfig;
			} else {
				Model.ConnectionParameters.ConnectionStringName = string.Empty;
				Model.ShouldSaveConnection = SaveConnectionMethod.Hardcode;
			}
			if(FinishEnabled)
				Model.StoredProceduresInfo = null;
		}
		public override Type GetNextPageType() {
			return typeof(ConfigureEFStoredProceduresPage<TModel>);
		}
		protected virtual void ShowMessage(string message) { context.ShowMessage(message); }
		void OnViewConnectionParametersChanged(object sender, EventArgs eventArgs) {
			Model.ConnectionParameters.ConnectionString = View.ConnectionString;
			if(View.ShouldSaveConnectionString)
				Model.ConnectionParameters.ConnectionStringName = View.ConnectionName;
		}
	}
}
