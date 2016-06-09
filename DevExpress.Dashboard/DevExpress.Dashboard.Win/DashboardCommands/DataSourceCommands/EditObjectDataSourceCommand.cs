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
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.UI.ObjectBinding;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Commands;
using System;
using System.ComponentModel.Design;
using System.Xml.Linq;
namespace DevExpress.DashboardWin.Commands {
	public class EditObjectDataSourceCommand : DashboardEditDataSourceCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditObjectDataSource; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandEditObjectDataSourceCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandEditObjectDataSourceDescription; } }
		public override string ImageName { get { return "EditDataSource"; } }
		public EditObjectDataSourceCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new EditDataSourceHistoryItem(dataSource, PreviousDataSourceState);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = DataSource != null && DataSource is DashboardObjectDataSource;
		}
		protected override bool RunAction() {
			DashboardObjectDataSource objectDataSource = DataSource as DashboardObjectDataSource;
			bool changed = false;
			if(objectDataSource != null) {
				IServiceProvider serviceProvider = Control.ServiceProvider;
				IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
				IParameterService parameterService = serviceProvider.RequestServiceStrictly<IParameterService>();
				ISolutionTypesProvider solutionTypesProvider = serviceProvider.RequestServiceStrictly<ISolutionTypesProvider>();
				DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(guiContext.LookAndFeel, guiContext.Win32Window);
				changed = objectDataSource.EditDataSource(solutionTypesProvider, wizardRunnerContext, parameterService);
			}
			return changed;
		}
	}
}
