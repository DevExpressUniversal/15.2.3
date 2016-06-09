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
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region RemoveSheetCommand
	public class RemoveSheetCommand : SpreadsheetMenuItemSimpleCommand {
		public RemoveSheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RemoveSheet; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveSheet; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveSheetDescription; } }
		public override string ImageName { get { return "RemoveSheet"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				List<Worksheet> selectedSheets = DocumentModel.GetSelectedSheets();
				if (InnerControl.AllowShowingForms && !CanRemove(selectedSheets)) 
					return;
				int nextActiveSheetIndex = GetNextActiveSheetIndex(selectedSheets);
				DocumentModel.SetActiveSheetIndex(nextActiveSheetIndex, false);
				int count = selectedSheets.Count;
				for (int i = 0; i < count; i++) {
					DocumentModel.Sheets.Remove(selectedSheets[i]);
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		bool CanRemove(List<Worksheet> selectedSheets) {
			int count = selectedSheets.Count;
			for (int i = 0; i < count; i++) {
				if (!selectedSheets[i].IsEmptySheet() && !ShowWarningMessage())
					return false;
			}
			return true;
		}
		protected virtual bool ShowWarningMessage() {
			string message = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DeleteSheetConfirmation);
			return Control.ShowOkCancelMessage(message);
		}
		int GetNextActiveSheetIndex(List<Worksheet> selectedSheets) {
			List<Worksheet> visibleSheets = DocumentModel.GetVisibleSheets();
			WorksheetCollection sheets = DocumentModel.Sheets;
			int sheetCount = sheets.Count;
			for (int i = DocumentModel.ActiveSheetIndex + 1; i < sheetCount; i++) {
				if (IsValidActiveSheetIndex(visibleSheets, selectedSheets, sheets[i]))
					return i;
			}
			for (int i = DocumentModel.ActiveSheetIndex - 1; i >= 0; i--) {
				if (IsValidActiveSheetIndex(visibleSheets, selectedSheets, sheets[i]))
					return i;
			}
			Exceptions.ThrowInternalException();
			return -1;
		}
		bool IsValidActiveSheetIndex(List<Worksheet> visibleSheets, List<Worksheet> selectedSheets, Worksheet sheet) {
			return !selectedSheets.Contains(sheet) && visibleSheets.Contains(sheet);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Worksheet.Delete, CanDeleteSheets());
			ApplyWorkbookProtection(state, WorkbookProtection.LockStructure);
		}
		protected internal bool CanDeleteSheets() {
			return !InnerControl.IsInplaceEditorActive && DocumentModel.GetSelectedSheets().Count < DocumentModel.GetVisibleSheets().Count;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region RemoveSheetContextMenuItemCommand
	public class RemoveSheetContextMenuItemCommand : RemoveSheetCommand {
		public RemoveSheetContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.RemoveSheetContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveSheetContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_RemoveSheetContextMenuItemDescription; } }
		#endregion
	}
	#endregion
}
