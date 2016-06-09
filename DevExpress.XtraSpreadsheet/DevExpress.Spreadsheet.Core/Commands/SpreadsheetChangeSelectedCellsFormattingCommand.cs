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
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region SpreadsheetChangeSelectedCellsFormattingCommand (abstract class)
	public abstract class SpreadsheetChangeSelectedCellsFormattingCommandBase : SpreadsheetCommand {
		protected SpreadsheetChangeSelectedCellsFormattingCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.Selection; } }
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				ModifyDocumentModel(state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
		protected internal virtual void ModifyDocumentModel(ICommandUIState state) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				ModifyDocumentModelCore(state);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ModifyDocumentModelCore(ICommandUIState state) {
			SheetViewSelection selection = Selection;
			IList<CellRange> ranges = selection.SelectedRanges;
			List<CellRange> processedRanges = new List<CellRange>();
			for (int i = ranges.Count - 1; i >= 0; i--) {
				ModifyRange(ranges[i], processedRanges, state);
				processedRanges.Add(ranges[i]);
			}
		}
		protected internal void ModifyRange(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			for (int i = processedRanges.Count - 1; i >= 0; i--)
				if (processedRanges[i].ContainsRange(range))
					return;
			bool entireRowsSelected = range.TopLeft.Column == 0 && range.BottomRight.Column == IndicesChecker.MaxColumnCount - 1;
			bool entireColumnsSelected = range.TopLeft.Row == 0 && range.BottomRight.Row == IndicesChecker.MaxRowCount - 1;
			if (entireRowsSelected && entireColumnsSelected) {
				ModifyEntireSheet(range, processedRanges, state);
				return;
			}
			if (entireColumnsSelected) {
				ModifyColumns(range, processedRanges, state);
				return;
			}
			if (entireRowsSelected) {
				ModifyRows(range, processedRanges, state);
				return;
			}
			ModifyAllCells(range, processedRanges, state);
		}
		protected IEnumerator<Row> GetExistingRows(Worksheet sheet) {
			return sheet.Rows.GetExistingRows().GetEnumerator();
		}
		protected virtual void CreateRowsCellsInterceptWithColumnCells(Worksheet sheet, int leftColumnIndex, int rightColumnIndex) {
			IEnumerator<Row> existingRows = GetExistingRows(sheet);
			while (existingRows.MoveNext()) {
				if (existingRows.Current.ApplyStyle)
					for (int currentColumnIndex = leftColumnIndex; currentColumnIndex <= rightColumnIndex; currentColumnIndex++)
						sheet.GetCellOrCreate(currentColumnIndex, existingRows.Current.Index);
			}
		}
		protected internal abstract void ModifyEntireSheet(CellRange range, IList<CellRange> processedRanges, ICommandUIState state);
		protected internal abstract void ModifyColumns(CellRange range, IList<CellRange> processedRanges, ICommandUIState state);
		protected internal abstract void ModifyRows(CellRange range, IList<CellRange> processedRanges, ICommandUIState state);
		protected internal abstract void ModifyAllCells(CellRange range, IList<CellRange> processedRanges, ICommandUIState state);
	}
	#endregion
	public abstract class SpreadsheetChangeSelectedCellsFormattingCommand<TSetValueAccessor> : SpreadsheetChangeSelectedCellsFormattingCommandBase
		where TSetValueAccessor : class {
		protected SpreadsheetChangeSelectedCellsFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ModifyEntireSheet(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			Worksheet sheet = Selection.Sheet;
			ModifyRowsCore(GetExistingRows(sheet), GetExistingRowsAccessors, sheet, state);
			IEnumerator<TSetValueAccessor> columnAccessor = GetColumnsAccessors(sheet, 0, IndicesChecker.MaxColumnCount - 1);
			ModifyDocumentModelItems(columnAccessor, state);
			ModifyExistingCells(range, state);
		}
		protected internal override void ModifyColumns(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			Worksheet sheet = Selection.Sheet;
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			CreateRowsCellsInterceptWithColumnCells(sheet, leftColumn, rightColumn);
			ModifyDocumentModelItems(GetColumnsAccessors(sheet, leftColumn, rightColumn), state);
			ModifyExistingCells(range, state);
		}
		protected internal override void ModifyRows(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			Worksheet sheet = Selection.Sheet;
			ModifyRowsCore(GetRowsAccessorsCore(sheet, range.TopLeft.Row, range.BottomRight.Row), GetRowsAccessors, sheet, state);
			ModifyExistingCells(range, state);
		}
		protected internal override void ModifyAllCells(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			ModifyDocumentModelItems(GetRangeAllCellsAccessors(Selection.Sheet, range), state);
		}
		void ModifyExistingCells(CellRange range, ICommandUIState state) {
			ModifyDocumentModelItems(GetRangeExistingCellsAccessors(Selection.Sheet, range), state);
		}
		protected internal IEnumerator<TSetValueAccessor> GetColumnsAccessors(Worksheet sheet, int nearColumnIndex, int farColumnIndex) {
			IEnumerator<Column> enumerator = GetColumnsAccessorsCore(sheet, nearColumnIndex, farColumnIndex);
			return new EnumeratorConverter<Column, TSetValueAccessor>(enumerator, GetColumnAccessor);
		}
		protected internal IEnumerator<TSetValueAccessor> GetRowsAccessors(IEnumerator<Row> rows, Worksheet sheet) {
			return new EnumeratorConverter<Row, TSetValueAccessor>(rows, GetRowAccessor);
		}
		protected internal virtual IEnumerator<Column> GetColumnsAccessorsCore(Worksheet sheet, int nearColumnIndex, int farColumnIndex) {
			return sheet.Columns.GetColumnRangesEnsureExist(nearColumnIndex, farColumnIndex).GetEnumerator();
		}
		protected internal virtual IEnumerator<Row> GetRowsAccessorsCore(Worksheet sheet, int nearRowIndex, int farRowIndex) {
			return sheet.Rows.GetAllRowsEnumerator(nearRowIndex, farRowIndex, false);
		}
		protected internal virtual IEnumerator<TSetValueAccessor> GetRangeAllCellsAccessors(Worksheet sheet, CellRange range) {
			IEnumerator<ICellBase> enumerator = GetRangeAllCellsAccessorsCore(range);
			return new EnumeratorConverter<ICellBase, TSetValueAccessor>(enumerator, GetCellAccessorCore);
		}
		protected internal virtual IEnumerator<ICellBase> GetRangeAllCellsAccessorsCore(CellRange range) {
			return range.GetAllCellsEnumerator();
		}
		protected internal virtual IEnumerator<TSetValueAccessor> GetRangeExistingCellsAccessors(Worksheet sheet, CellRange range) {
			return new EnumeratorConverter<ICellBase, TSetValueAccessor>(range.GetExistingCellsEnumerator(false), GetCellAccessorCore);
		}
		delegate IEnumerator<TSetValueAccessor> GetRowsAccessor(IEnumerator<Row> rows, Worksheet sheet);
		void ModifyRowsCore(IEnumerator<Row> rows, GetRowsAccessor accessor, Worksheet sheet, ICommandUIState state) {
			ModifyDocumentModelItems(accessor(rows, sheet), state);
		}
		IEnumerator<TSetValueAccessor> GetExistingRowsAccessors(IEnumerator<Row> existingRows, Worksheet sheet) {
			return new EnumeratorConverter<Row, TSetValueAccessor>(existingRows, GetRowAccessor);
		}
		TSetValueAccessor GetCellAccessorCore(ICellBase cell) {
			return GetCellAccessor((ICell)cell);
		}
		protected internal abstract TSetValueAccessor GetCellAccessor(ICell cell);
		protected internal abstract TSetValueAccessor GetColumnAccessor(Column column);
		protected internal abstract TSetValueAccessor GetRowAccessor(Row row);
		protected internal abstract void ModifyDocumentModelItems(IEnumerator<TSetValueAccessor> enumerator, ICommandUIState state);
	}
	#region SpreadsheetChangeSelectedCellsFormattingCommand<TValueAccessor, TValueType> (abstract class)
	public abstract class SpreadsheetChangeSelectedCellsFormattingCommand<TSetValueAccessor, TValueType> : SpreadsheetChangeSelectedCellsFormattingCommand<TSetValueAccessor>
		where TSetValueAccessor : class {
		protected SpreadsheetChangeSelectedCellsFormattingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ModifyDocumentModelItems(IEnumerator<TSetValueAccessor> enumerator, ICommandUIState state) {
			IValueBasedCommandUIState<TValueType> valueBasedState = state as IValueBasedCommandUIState<TValueType>;
			if (valueBasedState == null)
				return;
			TValueType value = GetNewValue(valueBasedState);
			for (; ; ) {
				if (!enumerator.MoveNext())
					break;
				SetValue(enumerator.Current, value);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Visible && state.Enabled) {
				ICell cell = GetActiveCell();
				TValueType cellValue = GetActiveCellValue(cell);
				UpdateUIState(state, cellValue);
				IValueBasedCommandUIState<TValueType> valueBasedState = state as IValueBasedCommandUIState<TValueType>;
				if (valueBasedState != null)
					UpdateUIStateValue(valueBasedState, cellValue);
			}
		}
		protected ICell GetActiveCell() {
			CellPosition position = Selection.ActiveCell;
			return Selection.Sheet.GetCellForFormatting(position);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<TValueType>();
		}
		protected internal virtual void UpdateUIState(ICommandUIState state, TValueType activeCellValue) {
		}
		protected internal virtual void UpdateUIStateValue(IValueBasedCommandUIState<TValueType> state, TValueType activeCellValue) {
			state.Value = activeCellValue;
		}
		protected internal virtual TValueType GetNewValue(IValueBasedCommandUIState<TValueType> state) {
			return state.Value;
		}
		protected internal abstract TValueType GetActiveCellValue(ICell cell);
		protected internal abstract void SetValue(TSetValueAccessor accessor, TValueType value);
	}
	#endregion
}
