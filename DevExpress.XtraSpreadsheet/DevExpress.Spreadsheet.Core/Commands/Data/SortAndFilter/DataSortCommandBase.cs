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
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region DataSortOrFilterAccessor
	public class DataSortOrFilterAccessor {
		#region Fields
		readonly DocumentModel documentModel;
		CellRange entireRange;
		CellRange sortOrFilterRange;
		SortState sortState;
		AutoFilterBase filter;
		#endregion
		public DataSortOrFilterAccessor(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		public SheetViewSelection Selection { get { return ActiveSheet.Selection; } }
		public Worksheet ActiveSheet { get { return documentModel.ActiveSheet; } }
		public CellRange EntireRange { get { return entireRange; } }
		public SortState SortState { get { return sortState; } }
		public AutoFilterBase Filter { get { return filter; } }
		public CellRange SortOrFilterRange { get { return sortOrFilterRange; } }
		public bool ForSortCommand { get; set; }
		#endregion
		public CellRange GetSortOrFilterRange() {
			this.sortState = null;
			this.entireRange = null;
			this.filter = null;
			CellRange activeRange = Selection.ActiveRange;
			Table activeTable = Selection.TryGetActiveTable();
			if (activeTable != null) {
				bool shouldSkipTableFilter = ForSortCommand && !activeTable.Range.ContainsRange(activeRange);
				if (!shouldSkipTableFilter)
					return GetRangesFromTableFilter(activeTable);
			}
			SheetAutoFilter sheetFilter = ActiveSheet.AutoFilter;
			if (sheetFilter != null && sheetFilter.Enabled) {
				CellRange dataRange = sheetFilter.GetDataRange();
				bool shouldSkipSheetFilter = ForSortCommand && !sheetFilter.Range.ContainsRange(activeRange);
				if (dataRange != null && !shouldSkipSheetFilter)
					return GetRangesFromSheetFilter(sheetFilter, dataRange);
			}
			SortOrFilterRangeHelper helper = new SortOrFilterRangeHelper(activeRange);
			return helper.GetSheetSortOrFilterRange();
		}
		CellRange GetRangesFromTableFilter(Table activeTable) {
			this.sortState = activeTable.AutoFilter.SortState;
			this.entireRange = activeTable.HasTotalsRow ? activeTable.Range.GetResized(0, 0, 0, -1) : activeTable.Range.Clone();
			this.filter = activeTable.AutoFilter;
			CellRange result = activeTable.GetDataRange();
			this.sortOrFilterRange = CreateSortOrFilterRange(result);
			return result;
		}
		CellRange GetRangesFromSheetFilter(SheetAutoFilter sheetFilter, CellRange dataRange) {
			this.sortState = sheetFilter.SortState;
			this.entireRange = sheetFilter.Range.Clone();
			this.filter = sheetFilter;
			this.sortOrFilterRange = CreateSortOrFilterRange(dataRange);
			return dataRange;
		}
		protected CellRange CreateSortOrFilterRange(CellRange range) {
			if (EntireRange == null)
				return null;
			CellPosition activeCell = Selection.ActiveCell;
			return new CellRange(range.Worksheet, new CellPosition(activeCell.Column, EntireRange.TopLeft.Row), new CellPosition(activeCell.Column, range.BottomRight.Row));
		}
		public AutoFilterBase GetFilter() {
			Table activeTable = Selection.TryGetActiveTable();
			if (activeTable != null)
				return activeTable.AutoFilter;
			return ActiveSheet.AutoFilter;
		}
	}
	#endregion
	#region SortOrFilterRangeHelper
	public class SortOrFilterRangeHelper {
		#region Fields
		readonly Worksheet sheet;
		readonly CellRange range;
		#endregion
		public SortOrFilterRangeHelper(CellRange range) {
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentNotNull(range.Worksheet, "sheet");
			this.sheet = (Worksheet)range.Worksheet;
			this.range = range;
		}
		public CellRange GetSheetSortOrFilterRange() {
			if (range.CellCount == 1)
				return CalculateExtendedRangeOneCellCase(range);
			CellRange intersection = range.Intersection(sheet.GetUsedRange(true));
			if (intersection == null)
				return intersection;
			return CalculateExtendedRange(intersection, ExtendDown);
		}
		CellRange CalculateExtendedRangeOneCellCase(CellRange range) {
			ICell cell = sheet.TryGetCell(range.LeftColumnIndex, range.TopRowIndex);
			if (cell == null || !cell.HasContent) {
				CellUnion initialExtendedRanges = GetInitialRanges(range);
				if (initialExtendedRanges.InnerCellRanges.Count == 0)
					return null;
				range = initialExtendedRanges.GetCoveredRange();
			}
			return CalculateExtendedRange(range, ExtendAll);
		}
		CellUnion GetInitialRanges(CellRange range) {
			CellUnion initialRanges = new CellUnion(sheet, new List<CellRangeBase>());
			AddPosition(initialRanges, PositionUp(range, range.LeftColumnIndex));
			AddPosition(initialRanges, PositionDown(range, range.LeftColumnIndex));
			AddPosition(initialRanges, PositionLeft(range, range.TopRowIndex));
			AddPosition(initialRanges, PositionRight(range, range.TopRowIndex));
			AddPosition(initialRanges, PositionUpLeft(range));
			AddPosition(initialRanges, PositionUpRight(range));
			AddPosition(initialRanges, PositionDownLeft(range));
			AddPosition(initialRanges, PositionDownRight(range));
			return initialRanges;
		}
		void AddPosition(CellUnion initialRanges, CellPosition position) {
			if (CanExpand(position))
				initialRanges.InnerCellRanges.Add(new CellRange(sheet, position, position));
		}
		delegate CellRange ExtendRange(CellRange range);
		CellRange CalculateExtendedRange(CellRange range, ExtendRange extend) {
			for (; ; ) {
				CellRange extendedRange = extend(range);
				if (range.CellCount == extendedRange.CellCount)
					return extendedRange;
				range = extendedRange;
			}
		}
		#region Extend
		CellRange ExtendAll(CellRange range) {
			range = ExtendUpLeft(range);
			range = ExtendUpRight(range);
			range = ExtendDownLeft(range);
			range = ExtendDownRight(range);
			range = ExtendUp(range);
			range = ExtendDown(range);
			range = ExtendLeft(range);
			range = ExtendRight(range);
			return range;
		}
		#region CellPositions
		CellPosition PositionUpRight(CellRange range) {
			return new CellPosition(range.RightColumnIndex + 1, range.TopRowIndex - 1);
		}
		CellPosition PositionUpLeft(CellRange range) {
			return new CellPosition(range.LeftColumnIndex - 1, range.TopRowIndex - 1);
		}
		CellPosition PositionDownRight(CellRange range) {
			return new CellPosition(range.RightColumnIndex + 1, range.BottomRowIndex + 1);
		}
		CellPosition PositionDownLeft(CellRange range) {
			return new CellPosition(range.LeftColumnIndex - 1, range.BottomRowIndex + 1);
		}
		CellPosition PositionUp(CellRange range, int column) {
			return new CellPosition(column, range.TopRowIndex - 1);
		}
		CellPosition PositionDown(CellRange range, int column) {
			return new CellPosition(column, range.BottomRowIndex + 1);
		}
		CellPosition PositionLeft(CellRange range, int row) {
			return new CellPosition(range.LeftColumnIndex - 1, row);
		}
		CellPosition PositionRight(CellRange range, int row) {
			return new CellPosition(range.RightColumnIndex + 1, row);
		}
		#endregion
		CellRange ExtendUpRight(CellRange range) {
			for (; ; ) {
				CellPosition nextPosition = PositionUpRight(range);
				if (!CanExpand(nextPosition))
					return range;
				CellPosition topLeft = new CellPosition(range.LeftColumnIndex, nextPosition.Row);
				CellPosition bottomRight = new CellPosition(nextPosition.Column, range.BottomRowIndex);
				range = new CellRange(sheet, topLeft, bottomRight);
			}
		}
		CellRange ExtendDownRight(CellRange range) {
			for (; ; ) {
				CellPosition nextPosition = PositionDownRight(range);
				if (!CanExpand(nextPosition))
					return range;
				range = new CellRange(sheet, range.TopLeft, nextPosition);
			}
		}
		CellRange ExtendDownLeft(CellRange range) {
			for (; ; ) {
				CellPosition nextPosition = PositionDownLeft(range);
				if (!CanExpand(nextPosition))
					return range;
				CellPosition topLeft = new CellPosition(nextPosition.Column, range.TopRowIndex);
				CellPosition bottomRight = new CellPosition(range.RightColumnIndex, nextPosition.Row);
				range = new CellRange(sheet, topLeft, bottomRight);
			}
		}
		CellRange ExtendUpLeft(CellRange range) {
			for (; ; ) {
				CellPosition nextPosition = PositionUpLeft(range);
				if (!CanExpand(nextPosition))
					return range;
				range = new CellRange(sheet, nextPosition, range.BottomRight);
			}
		}
		CellRange ExtendUp(CellRange range) {
			int start = range.TopLeft.Column;
			int end = range.BottomRight.Column;
			for (int i = start; i <= end; i++) {
				for (; ; ) {
					CellPosition nextPosition = PositionUp(range, i);
					if (!CanExpand(nextPosition))
						break;
					range = new CellRange(sheet, new CellPosition(range.LeftColumnIndex, nextPosition.Row), range.BottomRight);
				}
			}
			return range;
		}
		CellRange ExtendDown(CellRange range) {
			int start = range.TopLeft.Column;
			int end = range.BottomRight.Column;
			for (int i = start; i <= end; i++) {
				for (; ; ) {
					CellPosition nextPosition = PositionDown(range, i);
					if (!CanExpand(nextPosition))
						break;
					range = new CellRange(sheet, range.TopLeft, new CellPosition(range.RightColumnIndex, nextPosition.Row));
				}
			}
			return range;
		}
		CellRange ExtendLeft(CellRange range) {
			int start = range.TopLeft.Row;
			int end = range.BottomRight.Row;
			for (int i = start; i <= end; i++) {
				for (; ; ) {
					CellPosition nextPosition = PositionLeft(range, i);
					if (!CanExpand(nextPosition))
						break;
					range = new CellRange(sheet, new CellPosition(nextPosition.Column, range.TopRowIndex), range.BottomRight);
				}
			}
			return range;
		}
		CellRange ExtendRight(CellRange range) {
			int start = range.TopLeft.Row;
			int end = range.BottomRight.Row;
			for (int i = start; i <= end; i++) {
				for (; ; ) {
					CellPosition nextPosition = PositionRight(range, i);
					if (!CanExpand(nextPosition))
						break;
					range = new CellRange(sheet, range.TopLeft, new CellPosition(nextPosition.Column, range.BottomRowIndex));
				}
			}
			return range;
		}
		bool CanExpand(CellPosition nextPosition) {
			if (!nextPosition.IsValid)
				return false;
			ICell cell = sheet.TryGetCell(nextPosition.Column, nextPosition.Row);
			return cell != null && cell.HasContent && sheet.Tables.TryGetItem(nextPosition) == null;
		}
		#endregion
	}
	#endregion
	#region DataSortCommandBase (abstract class)
	public abstract class DataSortCommandBase : SpreadsheetMenuItemSimpleCommand {
		readonly DataSortOrFilterAccessor accessor;
		protected DataSortCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.accessor = new DataSortOrFilterAccessor(DocumentModel);
			accessor.ForSortCommand = true;
		}
		protected SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.Selection; } }
		protected abstract bool IsDescending { get; }
		protected internal override void ExecuteCore() {
			if (Selection.SelectedRanges.Count > 1) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_UnableToSortMultipleSelection));
				return;
			}
			PivotTable activePivotTable = Selection.TryGetActivePivotTable();
			if (activePivotTable != null) {
				PivotZone zone = activePivotTable.CalculationInfo.GetPivotZoneByCellPosition(Selection.ActiveCell);
				int fieldIndex = zone.GetActiveSortFieldIndex();
				ApplyPivotTableSortCommand command = new ApplyPivotTableSortCommand(activePivotTable, IsDescending, fieldIndex, ErrorHandler);
				command.Execute();
				return;
			}
			if (Selection.ContainsPivotTableInActiveRange()) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged));
				return;
			}
			CellRange range = accessor.GetSortOrFilterRange();
			if (range == null) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_UnableToSortSelection));
				return;
			}
			if (ActiveSheet.IsProtected && !DocumentModel.CheckRangeAccess(range)) {
				InnerControl.ShowReadOnlyObjectMessage();
				return;
			}
			if (ActiveSheet.MergedCells.RangeContainsMergedCellRangesOfDifferentSize(range)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_UnableToSortMergedCells));
				return;
			}
			if (ActiveSheet.ArrayFormulaRanges.CheckMultiCellArrayFormulasInRange(range)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray));
				return;
			}
			SortState sortState = accessor.SortState;
			DocumentModel.BeginUpdate();
			try {
				if (sortState == null)
					sortState = new SortState(ActiveSheet);
				else
					sortState.Clear();
				SetupSortState(sortState, range);
				sortState.Apply(ErrorHandler);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void SetupSortState(SortState sortState, CellRange range) {
			SortCondition condition = new SortCondition(ActiveSheet, GetReferenceRange(range));
			condition.Descending = IsDescending;
			sortState.SortConditions.Add(condition);
			sortState.SortRange = range.Clone();
		}
		CellRange GetReferenceRange(CellRange range) {
			CellRange sortOrFilterRange = accessor.SortOrFilterRange;
			if (sortOrFilterRange == null) {
				int columnIndex = ActiveSheet.Selection.ActiveCell.Column - range.TopLeft.Column;
				return range.GetSubColumnRange(columnIndex, columnIndex);
			}
			return sortOrFilterRange.Clone();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool enabled = !Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive;
			if (enabled)
				enabled &= !HasPivotTableRestriction();
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, enabled);
			ApplyActiveSheetProtection(state, !Protection.SortLocked);
			if (state.Enabled)
				state.Enabled = IsSortPossibleForSelection();
		}
		bool HasPivotTableRestriction() {
			bool hasPivotTableRestriction = false;
			PivotZone selectedPivotZone = Selection.TryGetActivePivotZone();
			if (selectedPivotZone != null)
				hasPivotTableRestriction = selectedPivotZone.GetActiveFieldIndex() < 0;
			return hasPivotTableRestriction;
		}
		bool IsSortPossibleForSelection() {
			CellRange range = Selection.ActiveRange;
			List<Table> tables = ActiveSheet.Tables.GetItems(range, true);
			if (tables.Count == 0)
				return true;
			if (tables.Count != 1)
				return false;
			return tables[0].Range.ContainsRange(range);
		}
	}
	#endregion
}
