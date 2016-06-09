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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Commands;
using DevExpress.DataAccess.UI.Excel;
using DevExpress.DataAccess.Excel;
namespace DevExpress.DashboardWin.Commands {
	class EditExcelDataSourceCommand : DashboardEditDataSourceCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditExcelDataSource; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandEditExcelDataSourceCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandEditExcelDataSourceDescription; } }
		public override string ImageName { get { return "EditDataSource"; } }
		public EditExcelDataSourceCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new EditDataSourceHistoryItem(dataSource, PreviousDataSourceState);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = DataSource != null && DataSource is DashboardExcelDataSource;
		}
		protected override bool RunAction() {
			DashboardExcelDataSource excelDataSource = DataSource as DashboardExcelDataSource;
			bool changed = false;
			if(excelDataSource != null) {
				IServiceProvider serviceProvider = Control.ServiceProvider;
				IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
				changed = excelDataSource.EditDataSource(guiContext.LookAndFeel, guiContext.Win32Window);
			}
			return changed;
		}
	}
}
