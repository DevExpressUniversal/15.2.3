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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils.Commands;
using System.Xml.Linq;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using System.ComponentModel.Design;
using System;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.UI.Wizard;
using System.Collections.Generic;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
namespace DevExpress.DashboardWin.Commands {
	public class EditSqlConnectionCommand : DashboardEditDataSourceCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditSqlConnection; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandEditConnectionCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandEditConnectionDescription; } }
		public override string ImageName { get { return "Connection"; } }
		public EditSqlConnectionCommand(DashboardDesigner control)
			: base(control) { 
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new EditConnectionHistoryItem(dataSource, PreviousDataSourceState);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = DataSource != null && DataSource is DashboardSqlDataSource;
		}
		protected override bool RunAction() {
			SqlDataSource sqlDataSource = DataSource as SqlDataSource;
			bool changed = false;
			if(sqlDataSource != null) {
				IServiceProvider serviceProvider = Control.ServiceProvider;
				IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
				IConnectionStorageService connectionStorageService = serviceProvider.RequestServiceStrictly<IConnectionStorageService>();
				IDashboardDataSourceWizardCustomization wizardCustomization = serviceProvider.RequestService<IDashboardDataSourceWizardCustomization>();
				IDashboardDataSourceWizardSettingsProvider settingsProvider = serviceProvider.RequestService<IDashboardDataSourceWizardSettingsProvider>();
				DashboardDataSourceWizardSettings settings = settingsProvider.Settings;
				IEnumerable<ProviderLookupItem> providers = DataSourceExecutor.CreateSqlProvidersList(settings);
				changed = sqlDataSource.ConfigureConnection<DashboardDataSourceModel>(
					new ConfigureConnectionContext {
						LookAndFeel = guiContext.LookAndFeel,
						Owner = guiContext.Win32Window,
						ConnectionStorageService = connectionStorageService
					}, (e) => {
						if(wizardCustomization != null)
							wizardCustomization.CustomizeDataSourceWizard(e);
						else {
							List<ProviderLookupItem> wizardProviders =
								((List<ProviderLookupItem>)e.Resolve(typeof(List<ProviderLookupItem>)));
							wizardProviders.Clear();
							wizardProviders.AddRange(providers);
						}
					});
			}
			return changed;
		}
	}
}
