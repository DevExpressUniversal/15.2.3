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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MoveOrCopySheetCommand
	public class MoveOrCopySheetCommand : SpreadsheetCommand {
		public MoveOrCopySheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MoveOrCopySheet; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheet; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetDescription; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms) {
					MoveOrCopySheetViewModel viewModel = new MoveOrCopySheetViewModel(Control, GetSheetNames());
					Control.ShowMoveOrCopySheetForm(viewModel);
				}
				else {
					IValueBasedCommandUIState<MoveOrCopySheetCommandParameters> valueBasedState = state as IValueBasedCommandUIState<MoveOrCopySheetCommandParameters>;
					if (valueBasedState == null)
						return;
					MoveOrCopySheetCommandParameters parameters = valueBasedState.Value;
					MoveOrCopySheet(parameters.BeforeVisibleSheetIndex, parameters.CreateCopy);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal List<string> GetSheetNames() {
			List<string> result = DocumentModel.GetVisibleSheetNames();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MoveOrCopySheetForm_MoveToEnd));
			return result;
		}
		protected internal void MoveOrCopySheet(int beforeVisibleSheetIndex, bool createCopy) {
			if (DocumentModel.SheetCount == 1 && !createCopy)
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				bool moveToEnd = false;
				int insertIndex = GetNewIndex(beforeVisibleSheetIndex, ref moveToEnd);
				List<Worksheet> selectedSheets = DocumentModel.GetSelectedSheets();
				Worksheet firstSelectedSheet = selectedSheets[0];
				string newActiveSheetName = createCopy ? GetCopySheetName(firstSelectedSheet.Name) : firstSelectedSheet.Name;
				if (insertIndex > DocumentModel.Sheets.IndexOf(firstSelectedSheet) && !createCopy) {
					for (int i = 0; i < selectedSheets.Count; i++) {
						Worksheet sheet = selectedSheets[i];
						Modify(insertIndex, moveToEnd, sheet, createCopy);
					}
				}
				else {
					for (int i = selectedSheets.Count - 1; i >= 0; i--) {
						Worksheet sheet = selectedSheets[i];
						Modify(insertIndex, moveToEnd, sheet, createCopy);
					}
				}
				SetActiveSheet(newActiveSheetName, selectedSheets);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		int GetNewIndex(int beforeVisibleSheetIndex, ref bool moveToEnd) {
			List<Worksheet> visibleSheets = DocumentModel.GetVisibleSheets();
			int count = visibleSheets.Count;
			if (beforeVisibleSheetIndex == count) {
				Worksheet last = visibleSheets[count - 1];
				moveToEnd = true;
				return DocumentModel.Sheets.IndexOf(last);
			}
			return DocumentModel.Sheets.IndexOf(visibleSheets[beforeVisibleSheetIndex]);
		}
		void Modify(int insertIndex, bool moveToEnd, Worksheet sheet, bool createCopy) {
			int index = GetIndex(insertIndex, moveToEnd, sheet, createCopy);
			if (createCopy) {
				Worksheet newSheet = DocumentModel.CreateWorksheet(GetCopySheetName(sheet.Name));
				DocumentModel.Sheets.Insert(index, newSheet);
				SourceTargetRangesForCopy pairs = new SourceTargetRangesForCopy(sheet, newSheet);
				CopyWorksheetOperation copyOperation = new CopyWorksheetOperation(pairs);
				copyOperation.Execute();
			}
			else
				DocumentModel.Sheets.Move(index, sheet);
		}
		string GetCopySheetName(string currentName) {
			int index = 2;
			for (; ; ) {
				string newName = String.Format("{0} ({1})", currentName, index);
				if (CheckValidName(newName))
					return newName;
				index++;
			}
		}
		bool CheckValidName(string newName) {
			foreach (Worksheet sheet in DocumentModel.Sheets) {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(sheet.Name, newName) == 0)
					return false;
			}
			return true;
		}
		int GetIndex(int insertedIndex, bool moveToEnd, Worksheet sheet, bool createCopy) {
			if (moveToEnd)
				return createCopy ? insertedIndex + 1 : insertedIndex;
			int sheetIndex = DocumentModel.Sheets.IndexOf(sheet);
			if (insertedIndex > sheetIndex)
				return createCopy ? insertedIndex : insertedIndex - 1;
			return insertedIndex;
		}
		void SetActiveSheet(string newActiveSheetName, List<Worksheet> selectedSheets) {
			foreach (Worksheet sheet in selectedSheets) {
				sheet.IsSelected = false;
			}
			DocumentModel.ActiveSheet = DocumentModel.Sheets[newActiveSheetName];
			DocumentModel.ActiveSheet.IsSelected = true;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<MoveOrCopySheetCommandParameters>();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
			ApplyWorkbookProtection(state, WorkbookProtection.LockStructure);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region MoveOrCopySheetCommandParameters
	public class MoveOrCopySheetCommandParameters {
		public MoveOrCopySheetCommandParameters() {
		}
		public MoveOrCopySheetCommandParameters(int beforeVisibleSheetIndex, bool createCopy) {
			BeforeVisibleSheetIndex = beforeVisibleSheetIndex;
			CreateCopy = createCopy;
		}
		#region Properties
		public int BeforeVisibleSheetIndex { get; set; }
		public bool CreateCopy { get; set; }
		#endregion
	}
	#endregion
	#region MoveOrCopySheetContextMenuItemCommand
	public class MoveOrCopySheetContextMenuItemCommand : MoveOrCopySheetCommand {
		public MoveOrCopySheetContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MoveOrCopySheetContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MoveOrCopySheetContextMenuItemDescription; } }
		#endregion
	}
	#endregion
}
