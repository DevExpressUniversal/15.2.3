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

using DevExpress.DashboardWin.DataSourceWizard;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using System;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.DataSourceWizard {
	public class ConfigureOlapParametersPage<TModel> : WizardPageBase<IConfigureOlapParametersPageView, TModel>
		where TModel : IDataComponentModelWithConnection {
		public override bool FinishEnabled { get { return !ConnectionStorageService.CanSaveConnection;   } }
		public override bool MoveNextEnabled { get { return ConnectionStorageService.CanSaveConnection;  } }
		IDataConnectionParametersService DataConnectionParametersProvider { get; set; }
		IEnumerable<SqlDataConnection> DataConnections { get; set; }
		IConnectionStorageService ConnectionStorageService { get; set; }
		public ConfigureOlapParametersPage(IConfigureOlapParametersPageView view, IDataConnectionParametersService dataConnectionParametersService, IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections)
			: base(view) {
			DataConnectionParametersProvider = dataConnectionParametersService;
			ConnectionStorageService = connectionStorageService;
			DataConnections = dataConnections;
		}
		public override void Begin() {
			IDataConnection dataConnection = Model.DataConnection;
			if(dataConnection != null)
				((ConfigureOlapParametersPageView)View).ApplyParameters(((OlapDataConnection)dataConnection).ConnectionString);
		}
		public override bool Validate(out string errorMessage) { 
			errorMessage = null;
			try {
				View.GetConnection();
			} catch {
				errorMessage = "Error";
				return false;
			}
			return true;
		}
		public override void Commit() {
			try {
				OlapDataConnection connection = View.GetConnection();
				Model.DataConnection = connection;
				Model.ConnectionName = connection.Name;
			} catch {
				Model.DataConnection = null;
			}
		}
		public override Type GetNextPageType() {
			if(ConnectionStorageService.CanSaveConnection)
				return typeof(SaveOlapConnectionPage<TModel>);
			return null;
		}
	}
}
