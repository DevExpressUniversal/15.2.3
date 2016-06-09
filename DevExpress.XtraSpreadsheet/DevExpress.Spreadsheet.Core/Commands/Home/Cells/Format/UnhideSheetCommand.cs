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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region UnhideSheetCommand
	public class UnhideSheetCommand : SpreadsheetCommand {
		public UnhideSheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.UnhideSheet; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideSheet; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideSheetDescription; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms) {
					UnhideSheetViewModel viewModel = new UnhideSheetViewModel(Control);
					Control.ShowUnhideSheetForm(viewModel);
				}
				else
					UnhideSheet(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal void UnhideSheet(string name) {
			if (String.IsNullOrEmpty(name))
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				DocumentModel.Sheets[name].VisibleState = SheetVisibleState.Visible;
				DocumentModel.ActiveSheet = DocumentModel.Sheets[name];
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			IValueBasedCommandUIState<string> state = new DefaultValueBasedCommandUIState<string>();
			if (!InnerControl.AllowShowingForms) {
				List<string> hiddenSheets = DocumentModel.GetHiddenSheetNames();
				if (hiddenSheets.Count <= 0)
					state.Value = String.Empty;
				else
					state.Value = hiddenSheets[0];
			}
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Worksheet.Unhide, CanUnhideSheets());
			ApplyWorkbookProtection(state, WorkbookProtection.LockStructure);
		}
		protected internal bool CanUnhideSheets() {
			return !InnerControl.IsInplaceEditorActive && DocumentModel.GetHiddenSheets().Count > 0;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region UnhideSheetContextMenuItemCommand
	public class UnhideSheetContextMenuItemCommand : UnhideSheetCommand {
		public UnhideSheetContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.UnhideSheetContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideSheetContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideSheetContextMenuItemDescription; } }
		#endregion
	}
	#endregion
}
