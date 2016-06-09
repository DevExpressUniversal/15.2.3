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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.UI.ObjectBinding;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils.Commands;
using System.ComponentModel.Design;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.Entity.ProjectModel;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Native;
using System.Xml.Linq;
using System;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Commands {
	public class EditEFDataSourceCommand : DashboardEditDataSourceCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditEFDataSource; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandEditEFDataSourceCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandEditEFDataSourceDescription; } }
		public override string ImageName { get { return "EditDataSource"; } }
		public EditEFDataSourceCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new EditDataSourceHistoryItem(dataSource, PreviousDataSourceState);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = DataSource != null && DataSource is DashboardEFDataSource;
		}
		protected override bool RunAction() {
			DashboardEFDataSource EFDataSource = DataSource as DashboardEFDataSource;
			bool changed = false;
			if(EFDataSource != null) {
				IServiceProvider serviceProvider = Control.ServiceProvider;
				IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
				ISolutionTypesProvider solutionTypesProvider = serviceProvider.RequestServiceStrictly<ISolutionTypesProvider>();
				IConnectionStringsProvider connectionStringsProvider = serviceProvider.RequestServiceStrictly<IConnectionStringsProvider>();
				IConnectionStorageService connectionStorageService = serviceProvider.RequestServiceStrictly<IConnectionStorageService>();
				IParameterService parameterService = serviceProvider.RequestServiceStrictly<IParameterService>();
				DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(guiContext.LookAndFeel, guiContext.Win32Window);
				changed = EFDataSourceUIHelper.Configure(EFDataSource, 
					wizardRunnerContext, 
					solutionTypesProvider, 
					connectionStringsProvider, 
					connectionStorageService, 
					parameterService);
			}
			return changed;
		}
	}
}
