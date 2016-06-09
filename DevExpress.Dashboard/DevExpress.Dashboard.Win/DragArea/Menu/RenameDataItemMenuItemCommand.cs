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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	public class RenameDataItemMenuItemCommand : DataItemMenuItemCommand {
		public override string Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandDataItemRename); } }
		public RenameDataItemMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem)
			: base(designer, dashboardItem, dataItem) {
		}
		public override void Execute() {
			RenameDataItemHistoryItem historyItem = new RenameDataItemHistoryItem(DashboardItem, DataItem);
			using (RenameDataItemForm form = new RenameDataItemForm(historyItem)) {
				UserLookAndFeel lookAndFeel = Designer.LookAndFeel;
				form.LookAndFeel.ParentLookAndFeel = lookAndFeel;
				if (form.ShowDialog(Designer.FindForm()) == DialogResult.OK) {
					try {
						historyItem.Redo(Designer);
						Designer.History.Add(historyItem);
					}
					catch (Exception ex) {
						XtraMessageBox.Show(lookAndFeel, Designer.ParentForm, ex.Message,
							DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}							   
			}
		}
	}
}
