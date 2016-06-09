#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel;
using DevExpress.XtraRichEdit;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowDeleteTableCellsFormCommand
	public class ShowDeleteTableCellsFormCommand : ShowInsertDeleteTableCellsFormCommandBase {
		public ShowDeleteTableCellsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowDeleteTableCellsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowDeleteTableCellsForm; } }
		#endregion
		protected internal override InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand() {
			return DocumentModel.CommandsCreationStrategy.CreateTableCellsCommand(Control);
		}
		protected internal override TableCellOperation GetTableCellsOperation() {
			return TableCellOperation.ShiftToTheHorizontally;
		}
		protected internal override void ShowInsertDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			Control.ShowDeleteTableCellsForm(parameters, callback, callbackData);
		}
	}
	#endregion
	#region ShowDeleteTableCellsFormMenuCommand
	public class ShowDeleteTableCellsFormMenuCommand : ShowDeleteTableCellsFormCommand {
		public ShowDeleteTableCellsFormMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowDeleteTableCellsFormMenuCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowDeleteTableCellsFormMenuItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowDeleteTableCellsFormMenuCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableCellsMenuItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowDeleteTableCellsFormMenuCommandImageName")]
#endif
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled == false) {
				state.Visible = false;
				return;
			}
			SelectedCellsCollection cellsCollenction = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			state.Enabled = state.Enabled && !cellsCollenction.IsSelectedEntireTableRows() && !cellsCollenction.IsSelectedEntireTableColumns();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
