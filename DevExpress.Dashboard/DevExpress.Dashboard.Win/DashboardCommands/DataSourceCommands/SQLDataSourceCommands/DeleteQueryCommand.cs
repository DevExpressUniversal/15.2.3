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
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Wizard;
namespace DevExpress.DashboardWin.Commands {
	public class DeleteQueryCommand : EditQueryCommandBase {
		string detetingQueryName = string.Empty;
		public override DashboardCommandId Id { get { return DashboardCommandId.DeleteQuery; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandDeleteQueryCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandDeleteQueryDescription; } }
		public override string ImageName { get { return "DeleteQuery"; } }
		public DeleteQueryCommand(DashboardDesigner control)
			: base(control) { 
		}
		protected override bool RunActionCore(DashboardSqlDataSource sqlDataSource, UserLookAndFeel userLookAndFeel, IWin32Window win32Window, IDBSchemaProvider dbSchemaProvider,
			IParameterService parameterService, IServiceProvider propertyGridServices, ICustomQueryValidator queryValidator, SqlWizardOptions wizardOptions) {
			detetingQueryName = Query.Name;
			bool queryDeleted = sqlDataSource.Queries.Remove(Query);
			if(queryDeleted) {
				IEnumerable<string> calcFieldsToRemove = sqlDataSource.CalculatedFields.Where(calcField => calcField.DataMember == detetingQueryName).Select(calcField => calcField.Name).ToArray();
				foreach(string calcFieldToRemove in calcFieldsToRemove)
					sqlDataSource.CalculatedFields.Remove(calcFieldToRemove);
			}
			return queryDeleted;
		}
		protected override EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource) {
			return new DeleteQueryHistoryItem(DataSource, PreviousDataSourceState, detetingQueryName);
		}
	}
}
