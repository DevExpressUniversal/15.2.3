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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DataAccess.Wizard;
namespace DevExpress.DashboardCommon.DataSourceWizard {
	public class SaveOlapConnectionPage<TModel> : WizardPageBase<ISaveConnectionPageView, TModel>
		where TModel : IDataComponentModelWithConnection {
		readonly IWizardRunnerContext context;
		IConnectionStorageService ConnectionStorageService { get; set; }
		public SaveOlapConnectionPage(ISaveConnectionPageView view, IWizardRunnerContext context, IConnectionStorageService connectionStorageService)
			: base(view) {
			this.context = context;
			ConnectionStorageService = connectionStorageService;
		}
		public override bool FinishEnabled { get { return true; } }
		public override bool MoveNextEnabled { get { return false; } }
		public override void Begin() {
			OlapDataConnection connection = (OlapDataConnection)Model.DataConnection;
			View.ConnectionName = Model.ConnectionName != null ? Model.ConnectionName : connection.Name;
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
			if(View.ShouldSaveConnectionString) {
				Model.ConnectionName = View.ConnectionName;
				Model.ShouldSaveConnection = SaveConnectionMethod.SaveToAppConfig;
			} else
				Model.ShouldSaveConnection = SaveConnectionMethod.Hardcode;
		}
		public virtual void ShowMessage(string message) { context.ShowMessage(message); }
	}
}
