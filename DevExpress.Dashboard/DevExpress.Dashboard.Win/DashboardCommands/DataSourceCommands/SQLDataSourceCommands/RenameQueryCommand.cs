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
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using System;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Commands {
	public class RenameQueryCommand : EditQueryCommandBase {
		string previousQueryName = string.Empty;
		SqlQuery query;
		public override DashboardCommandId Id { get { return DashboardCommandId.RenameQuery; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandRenameQueryCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandRenameQueryDescription; } }
		public override string ImageName { get { return "RenameQuery"; } }
		public RenameQueryCommand(DashboardDesigner control)
			: base(control) { 
		}
		protected override bool RunActionCore(DashboardSqlDataSource sqlDataSource, UserLookAndFeel userLookAndFeel, IWin32Window win32Window, IDBSchemaProvider dbSchemaProvider,
			IParameterService parameterService, IServiceProvider propertyGridServices, ICustomQueryValidator queryValidator, SqlWizardOptions wizardOptions) {
			using(DashboardRenameForm form = new DashboardRenameForm(Control, new QueryRenameFormModel(Query, Control))) {
				if(form.ShowDialog() == DialogResult.OK)
					return true;
			}
			return false;
		}
		protected override void SavePreviousDataSourceState() {
			base.SavePreviousDataSourceState();
			previousQueryName = Query.Name;
			query = Query;
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new RenameQueryHistoryItem(DataSource, PreviousDataSourceState, previousQueryName, query.Name);
		}
	}
}
