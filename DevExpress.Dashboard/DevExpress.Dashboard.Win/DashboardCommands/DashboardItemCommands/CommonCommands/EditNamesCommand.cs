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
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Commands {
	public class EditNamesCommand : DashboardItemCommand<DashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditNames; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandEditNamesCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandEditNamesDescription; } }
		public override string ImageName { get { return "EditNames"; } }
		public EditNamesCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			DashboardItem dashboardItem = DashboardItem;
			if(dashboardItem != null) {
				UserLookAndFeel lookAndFeel = Control.LookAndFeel;
				var historyItem = new EditNamesHistoryItem(dashboardItem, ((DashboardDesigner)Control).AllowEditComponentName);
				using (EditNamesForm nameForm = new EditNamesForm(historyItem)) {
					nameForm.LookAndFeel.ParentLookAndFeel = lookAndFeel;
					if (nameForm.ShowDialog(Control.FindForm()) == DialogResult.OK) {
						try {
							historyItem.Redo(Control);
							Control.History.Add(historyItem);
						}
						catch (Exception ex) {
							XtraMessageBox.Show(lookAndFeel, Control.ParentForm, ex.Message,
								DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
				}
			}
		}
	}
}
