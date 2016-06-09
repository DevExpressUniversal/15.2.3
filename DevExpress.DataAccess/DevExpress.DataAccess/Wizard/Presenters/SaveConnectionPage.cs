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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class SaveConnectionPage<TModel> : WizardPageBase<ISaveConnectionPageView, TModel> 
		where TModel : ISqlDataSourceModel 
	{
		readonly IWizardRunnerContext context;
		IConnectionStorageService ConnectionStorageService { get; set; }
		public SaveConnectionPage(ISaveConnectionPageView view, IWizardRunnerContext context, IConnectionStorageService connectionStorageService)
			: base(view) {
			this.context = context;
			ConnectionStorageService = connectionStorageService;
		}
		public override bool FinishEnabled { get { return false; } }
		public override bool MoveNextEnabled { get { return true; } }
		public override void Begin() {
			if(Model.ConnectionName == null)
				Model.ConnectionName = Model.DataConnection.Name;
			View.ConnectionName = Model.ConnectionName;
			SqlDataConnection connection = Model.DataConnection;
			View.SetCanSaveToStorage(ConnectionStorageService.CanSaveConnection);
			View.SetConnectionUsesServerAuth(connection != null && !connection.HasConnectionString && connection.ConnectionParameters != null && !((Xpo.Helpers.IConnectionPage)connection.ConnectionParameters).AuthType);
			View.ShouldSaveConnectionString = Model.ShouldSaveConnection.HasFlag(SaveConnectionMethod.SaveToAppConfig);
		}
		public override bool Validate(out string errorMessage) {
			if(!View.ShouldSaveConnectionString) {
				errorMessage = null;
				return true;
			}
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
			return base.Validate(out errorMessage);
		}
		public override void Commit() {
			if (View.ShouldSaveConnectionString) {
				Model.ConnectionName = View.ConnectionName;
				Model.ShouldSaveConnection = SaveConnectionMethod.SaveToAppConfig;
			}
			else
				Model.ShouldSaveConnection = SaveConnectionMethod.Hardcode;
			if (View.ShouldSaveCredentials)
				Model.ShouldSaveConnection |= SaveConnectionMethod.KeepCredentials;
			else {
				((Xpo.Helpers.IConnectionPage)(Model.DataConnection).ConnectionParameters).UserName = null;
				((Xpo.Helpers.IConnectionPage)(Model.DataConnection).ConnectionParameters).Password = null;
			}
		}
		public override Type GetNextPageType() {
			return typeof(ConfigureQueryPage<TModel>);
		}
		public virtual void ShowMessage(string message) { context.ShowMessage(message); }
	}
}
