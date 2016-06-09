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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region CreateDefinedNamesFromSelectionCommand
	public class CreateDefinedNamesFromSelectionCommand : SpreadsheetCommand {
		public CreateDefinedNamesFromSelectionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormulasCreateDefinedNamesFromSelection; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_CreateDefinedNamesFromSelection; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_CreateDefinedNamesFromSelectionDescription; } }
		public override string ImageName { get { return "DefinedNameCreateFromSelection"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			DocumentModel.BeginUpdate();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CreateDefinedNamesFromSelectionViewModel> valueBasedState = state as IValueBasedCommandUIState<CreateDefinedNamesFromSelectionViewModel>;
				if (valueBasedState == null)
					return;
				PopulateViewModel(valueBasedState.Value);
				if (InnerControl.AllowShowingForms)
					Control.ShowCreateDefinedNamesFromSelectionForm(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
				DocumentModel.EndUpdate();
			}
		}
		void PopulateViewModel(CreateDefinedNamesFromSelectionViewModel viewModel) {
			CellRange topRange = GetTopRowRange();
			if (topRange != null)
				viewModel.UseTopRow = CanCreateDefinedName(topRange.TopLeft, 0, 0) || CanCreateDefinedName(topRange.TopLeft, 1, 0);
			CellRange leftRange = GetLeftColumnRange();
			if (leftRange != null)
				viewModel.UseLeftColumn = (!viewModel.UseTopRow && CanCreateDefinedName(leftRange.TopLeft, 0, 0)) || CanCreateDefinedName(leftRange.TopLeft, 0, 1);
			if (!viewModel.UseTopRow) {
				CellRange bottomRange = GetBottomRowRange();
				if (bottomRange != null)
					viewModel.UseBottomRow = CanCreateDefinedName(bottomRange.BottomLeft, 0, 0) || CanCreateDefinedName(bottomRange.BottomLeft, 1, 0); ;
			}
			if (!viewModel.UseLeftColumn) {
				CellRange rightRange = GetRightColumnRange();
				if (rightRange != null)
					viewModel.UseRightColumn = (!viewModel.UseBottomRow && CanCreateDefinedName(rightRange.BottomRight, 0, 0)) || CanCreateDefinedName(rightRange.BottomRight, 0, -1);
			}
		}
		public void ApplyChanges(CreateDefinedNamesFromSelectionViewModel viewModel) {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (IsInvalidViewModel(selectedRange, viewModel.UseTopRow, viewModel.UseBottomRow, viewModel.UseLeftColumn, viewModel.UseRightColumn)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidCreateFromSelectionRange));
				return;
			}
			if (viewModel.UseTopRow)
				CreateDefinedNamesFromRangeValues(viewModel, GetTopRowRange(), CreateDataRange(selectedRange, 1, (viewModel.UseBottomRow ? -1 : 0), false), false);
			if (viewModel.UseBottomRow)
				CreateDefinedNamesFromRangeValues(viewModel, GetBottomRowRange(), CreateDataRange(selectedRange, (viewModel.UseTopRow ? 1 : 0), -1, false), false);
			if (viewModel.UseLeftColumn)
				CreateDefinedNamesFromRangeValues(viewModel, GetLeftColumnRange(), CreateDataRange(selectedRange, 1, (viewModel.UseRightColumn ? -1 : 0), true), true);
			if (viewModel.UseRightColumn)
				CreateDefinedNamesFromRangeValues(viewModel, GetRightColumnRange(), CreateDataRange(selectedRange, (viewModel.UseLeftColumn ? 1 : 0), -1, true), true);
		}
		bool IsInvalidViewModel(CellRange selectedRange, bool useTopRow, bool useBottomRow, bool useLeftColumn, bool useRightColumn) {
			return
				(selectedRange.Width == 1 && (useLeftColumn || useRightColumn)) ||
				(selectedRange.Height == 1 && (useTopRow || useBottomRow)) ||
				(selectedRange.Width == 2 && useLeftColumn && useRightColumn) ||
				(selectedRange.Height == 2 && useTopRow && useBottomRow);
		}
		#region Get(*)Range
		CellRange GetTopRowRange() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (selectedRange.Height <= 1)
				return null;
			return new CellRange(selectedRange.Worksheet, selectedRange.TopLeft, selectedRange.TopRight);
		}
		CellRange GetBottomRowRange() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (selectedRange.Height <= 1)
				return null;
			return new CellRange(selectedRange.Worksheet, selectedRange.BottomLeft, selectedRange.BottomRight);
		}
		CellRange GetLeftColumnRange() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (selectedRange.Width <= 1)
				return null;
			return new CellRange(selectedRange.Worksheet, selectedRange.TopLeft, selectedRange.BottomLeft);
		}
		CellRange GetRightColumnRange() {
			CellRange selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			if (selectedRange.Width <= 1)
				return null;
			return new CellRange(selectedRange.Worksheet, selectedRange.TopRight, selectedRange.BottomRight);
		}
		bool CanCreateDefinedName(CellPosition position, int horizontalOffset, int verticalOffset) {
			return CanCreateDefinedNameFromCellValue(new CellPosition(position.Column + horizontalOffset, position.Row + verticalOffset));
		}
		int CalculatePossibleDefinedNameCount(CellRange range, int maxCount) {
			if (range == null)
				return 0;
			int result = 0;
			foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
				if (CanCreateDefinedNameFromCellValue(cellBase.Value)) {
					result++;
					if (result >= maxCount)
						return result;
				}
			}
			return result;
		}
		bool CanCreateDefinedNameFromCellValue(CellPosition position) {
			ICell cell = ActiveSheet.TryGetCell(position.Column, position.Row);
			if (cell == null)
				return false;
			return CanCreateDefinedNameFromCellValue(cell.Value);
		}
		bool CanCreateDefinedNameFromCellValue(VariantValue value) {
			if (!value.IsText)
				return false;
			string text = value.GetTextValue(DocumentModel.SharedStringTable);
			if (String.IsNullOrEmpty(DocumentModel.CheckDefinedNameCore(text, -1, text)))
				return true;
			return String.IsNullOrEmpty(DocumentModel.CheckDefinedNameCore("_" + text, -1, text));
		}
		#endregion
		void CreateDefinedNamesFromRangeValues(CreateDefinedNamesFromSelectionViewModel viewModel, CellRange nameRange, CellRange dataRange, bool horizontalOrientation) {
			if (nameRange == null || dataRange == null)
				return;
			foreach (ICellBase cellBase in nameRange.GetExistingCellsEnumerable())
				CreateDefinedNameFromCellValue(viewModel, cellBase, dataRange, horizontalOrientation);
		}
		void CreateDefinedNameFromCellValue(CreateDefinedNamesFromSelectionViewModel viewModel, ICellBase nameCell, CellRange dataRange, bool horizontalOrientation) {
			VariantValue value = nameCell.Value;
			if (!value.IsText)
				return;
			string name = value.GetTextValue(DocumentModel.SharedStringTable);
			string errorMessage = DocumentModel.CheckDefinedNameCore(name, -1, String.Empty);
			bool duplicateName = (errorMessage == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists));
			if (!String.IsNullOrEmpty(errorMessage) && !duplicateName) {
				name = name + "_";
				errorMessage = DocumentModel.CheckDefinedNameCore(name, -1, String.Empty);
				duplicateName = (errorMessage == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists));
				if (!String.IsNullOrEmpty(errorMessage) && !duplicateName)
					return;
			}
			DefineNameViewModel nameViewModel = new DefineNameViewModel(Control);
			nameViewModel.Name = name;
			CellRange range = CreateNameRange(viewModel, nameCell.Position, dataRange, horizontalOrientation);
			nameViewModel.Reference = "=" + CellRangeToString.GetReferenceCommon(range, DocumentModel.DataContext.UseR1C1ReferenceStyle, range.TopLeft, PositionType.Absolute, PositionType.Absolute, true);
			nameViewModel.OriginalName = name;
			nameViewModel.Comment = String.Empty;
			nameViewModel.ScopeIndex = 0;
			nameViewModel.NewNameMode = !duplicateName;
			DefineNameCommand command = new DefineNameCommand(Control);
			command.ApplyChanges(nameViewModel);
		}
		CellRange CreateDataRange(CellRange range, int nearOffset, int farOffset, bool horizontalOrientation) {
			if (horizontalOrientation)
				return new CellRange(range.Worksheet,
					new CellPosition(range.TopLeft.Column + nearOffset, range.TopLeft.Row, PositionType.Absolute, PositionType.Absolute),
					new CellPosition(range.BottomRight.Column + farOffset, range.BottomRight.Row, PositionType.Absolute, PositionType.Absolute));
			else
				return new CellRange(range.Worksheet,
					new CellPosition(range.TopLeft.Column, range.TopLeft.Row + nearOffset, PositionType.Absolute, PositionType.Absolute),
					new CellPosition(range.BottomRight.Column, range.BottomRight.Row + farOffset, PositionType.Absolute, PositionType.Absolute));
		}
		CellRange CreateNameRange(CreateDefinedNamesFromSelectionViewModel viewModel, CellPosition namePosition, CellRange dataRange, bool horizontalOrientation) {
			if (horizontalOrientation) {
				int nameCellRow = namePosition.Row;
				bool useTopRow = viewModel.UseTopRow;
				bool useBottomRow = viewModel.UseBottomRow;
				if ((nameCellRow == dataRange.TopLeft.Row && useTopRow) ||
					(nameCellRow == dataRange.BottomRight.Row && useBottomRow))
					return dataRange.GetResized(0, (useTopRow ? 1 : 0), 0, (useBottomRow ? -1 : 0));
				return new CellRange(dataRange.Worksheet,
					new CellPosition(dataRange.TopLeft.Column, nameCellRow, PositionType.Absolute, PositionType.Absolute),
					new CellPosition(dataRange.BottomRight.Column, nameCellRow, PositionType.Absolute, PositionType.Absolute));
			}
			else {
				int nameCellColumn = namePosition.Column;
				bool useLeftColumn = viewModel.UseLeftColumn;
				bool useRightColumn = viewModel.UseRightColumn;
				if ((nameCellColumn == dataRange.TopLeft.Column && useLeftColumn) ||
					(nameCellColumn == dataRange.TopRight.Column && useRightColumn))
					return dataRange.GetResized((useLeftColumn ? 1 : 0), 0, (useRightColumn ? -1 : 0), 0);
				return new CellRange(dataRange.Worksheet,
					new CellPosition(nameCellColumn, dataRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute),
					new CellPosition(nameCellColumn, dataRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute));
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<CreateDefinedNamesFromSelectionViewModel> state = new DefaultValueBasedCommandUIState<CreateDefinedNamesFromSelectionViewModel>();
			state.Value = CreateViewModel();
			return state;
		}
		protected internal virtual CreateDefinedNamesFromSelectionViewModel CreateViewModel() {
			return new CreateDefinedNamesFromSelectionViewModel(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			SheetViewSelection selection = ActiveSheet.Selection;
			bool isEnabled = !InnerControl.IsAnyInplaceEditorActive && !selection.IsDrawingSelected && selection.SelectedRanges.Count == 1;
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, isEnabled);
			ApplyActiveSheetProtection(state);
		}
	}
	#endregion
}
