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
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using System;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Commands {
	public abstract class EditQueryCommandBase : DashboardEditDataSourceCommand {
		ParameterChangesCollection parametersChanges;
		protected SqlQuery Query { 
			get {
				SqlDataSource sqlDataSource = DataSource as SqlDataSource;
				if(sqlDataSource != null && !string.IsNullOrEmpty(DataMember)) 
					return sqlDataSource.Queries[DataMember];
				return null;
			}
		}
		protected ParameterChangesCollection ParametersChanges { get { return parametersChanges; } }
		protected EditQueryCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override void SavePreviousDataSourceState() {
			base.SavePreviousDataSourceState();
			parametersChanges = new ParameterChangesCollection(Dashboard.Parameters);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = Query != null;
			state.Visible = DataSource != null && DataSource is DashboardSqlDataSource;
		}
		protected override bool RunAction() {
			DashboardSqlDataSource sqlDataSource = DataSource as DashboardSqlDataSource;
			bool changed = false;
			if(sqlDataSource != null) {
				IServiceProvider serviceProvider = Control.ServiceProvider;
				var guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
				var dbSchemaProviderFactory = serviceProvider.RequestServiceStrictly<IDBSchemaProviderFactory>();
				var dbSchemaProvider = dbSchemaProviderFactory.CreateDBSchemaProvider();
				var parameterService = serviceProvider.RequestServiceStrictly<IParameterService>();
				var queryValidator = serviceProvider.RequestServiceStrictly<ICustomQueryValidator>();
				var editContext = serviceProvider.RequestServiceStrictly<IDataSourceEditContext>();
				var settingsProvider = serviceProvider.RequestServiceStrictly<IDashboardDataSourceWizardSettingsProvider>();
				editContext.BeginEditDataSource(sqlDataSource);
				try {
					changed = RunActionCore(sqlDataSource, guiContext.LookAndFeel, guiContext.Win32Window, dbSchemaProvider, parameterService, serviceProvider, queryValidator,
						settingsProvider.Settings.ToSqlWizardOptions());
				}
				finally {
					editContext.EndEditDataSource();
				}
				parametersChanges.ApplyChanges(Dashboard.Parameters);
			}
			return changed;
		}
		protected abstract bool RunActionCore(DashboardSqlDataSource sqlDataSource, UserLookAndFeel userLookAndFeel, IWin32Window win32Window, IDBSchemaProvider dbSchemaProvider, 
			IParameterService parameterService, IServiceProvider propertyGridServices, ICustomQueryValidator queryValidator, SqlWizardOptions wizardOptions);
	}
}
