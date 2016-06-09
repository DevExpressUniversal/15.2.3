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
using System.Diagnostics;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region EditingFillCommandBase (abstract class)
	public abstract class EditingFillCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected EditingFillCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.Selection; } }
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				PerformFillOperation();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsFillPossible();
			state.Visible = true;
			state.Checked = false;
		}
		protected virtual void PerformFillOperation() {
			if (!IsFillAllowed()) {
				InnerControl.ShowReadOnlyObjectMessage();
				return;
			}
			if (!CheckPivotTable())
				foreach (CellRange range in Selection.SelectedRanges)
					PerformFillOperation(range);
		}
		bool CheckPivotTable() {
			PivotTableCollection collection = DocumentModel.ActiveSheet.PivotTables;
			if (collection.Count > 0)
				if (ContainsIntersectsPivotTable(collection))
					return true;
				else 
					collection.RemoveItems(collection.GetRangesItems(Selection.SelectedRanges, true));
			return false;
		}
		bool IsFillAllowed() {
			if (!Protection.SheetLocked)
				return true;
			foreach (CellRange range in Selection.SelectedRanges)
				if (!IsFillAllowed(range))
					return false;
			return true;
		}
		bool IsFillAllowed(CellRange range) {
			RangeFillManager manager = CreateRangeFillManager();
			manager.CheckPermissionMode = true;
			manager.IsFillAllowed = true;
			manager.PerformFillOperation(range);
			return manager.IsFillAllowed;
		}
		bool ContainsIntersectsPivotTable(PivotTableCollection collection) {
			IModelErrorInfo error = collection.CanRangesRemove(Selection.SelectedRanges, true, RemoveCellMode.ShiftCellsLeft);
			if (error != null) {
				ErrorHandler.HandleError(error);
				return true;
			}
			return false;
		}
		bool IsFillPossible() {
			if (InnerControl.IsAnyInplaceEditorActive || ActiveSheet.Selection.IsDrawingSelected)
				return false;
			foreach (CellRange range in Selection.SelectedRanges)
				if (IsFillPossible(range))
					return true;
			return false;
		}
		bool IsFillPossible(CellRange range) {
			RangeFillManager manager = CreateRangeFillManager();
			return manager.IsFillPossible(range);
		}
		void PerformFillOperation(CellRange range) {
			RangeFillManager manager = CreateRangeFillManager();
			manager.PerformFillOperation(range);
		}
		protected abstract RangeFillManager CreateRangeFillManager();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	internal delegate void PerformFillOperationAction(ICell sourceCell, CellPosition targetPosition);
	public abstract class RangeFillManager {
		public bool CheckPermissionMode { get; set; }
		public bool IsFillAllowed { get; set; }
		public bool IsFillPossible(CellRange range) {
			return (CalculateSourceRow(range) != null ||
				CalculateSourceColumn(range) != null ||
				CalculateSourceRange(range) != null);
		}
		public void PerformFillOperation(CellRange range) {
			Row sourceRow = CalculateSourceRow(range);
			if (sourceRow != null)
				PerformFillOperation(sourceRow, range);
			Column sourceColumn = CalculateSourceColumn(range);
			if (sourceColumn != null)
				PerformFillOperation(sourceColumn, range);
			CellRange sourceRange = CalculateSourceRange(range);
			if (sourceRange != null)
				PerformFillOperation(sourceRange, range);
		}
		void PerformFillOperation(CellRange sourceRange, CellRange range) {
			Debug.Assert(sourceRange.Width == 1 || sourceRange.Height == 1);
			if (sourceRange.Width == 1)
				PerformHorizontalFillOperation(sourceRange, range);
			else if (sourceRange.Height == 1)
				PerformVerticalFillOperation(sourceRange, range);
		}
		void PerformHorizontalFillOperation(CellRange sourceRange, CellRange range) {
			Debug.Assert(sourceRange.Width == 1);
			Worksheet sheet = (Worksheet)sourceRange.Worksheet;
			int columnIndex = sourceRange.TopLeft.Column;
			int startRowIndex = sourceRange.TopLeft.Row;
			int endRowIndex = sourceRange.BottomRight.Row;
			for (int i = startRowIndex; i <= endRowIndex; i++) {
				ICell sourceCell = sheet.GetCellForFormatting(columnIndex, i);
				CellRange targetRange = CalculateVectorTargetRange(sourceCell, range);
				PerformFillOperation(sourceCell, targetRange);
				if (CheckPermissionMode && !IsFillAllowed)
					return;
			}
		}
		void PerformVerticalFillOperation(CellRange sourceRange, CellRange range) {
			Debug.Assert(sourceRange.Height == 1);
			Worksheet sheet = (Worksheet)sourceRange.Worksheet;
			int rowIndex = sourceRange.TopLeft.Row;
			int startColumnIndex = sourceRange.TopLeft.Column;
			int endColumnIndex = sourceRange.BottomRight.Column;
			for (int i = startColumnIndex; i <= endColumnIndex; i++) {
				ICell sourceCell = sheet.GetCellForFormatting(i, rowIndex);
				CellRange targetRange = CalculateVectorTargetRange(sourceCell, range);
				PerformFillOperation(sourceCell, targetRange);
				if (CheckPermissionMode && !IsFillAllowed)
					return;
			}
		}
		void PerformFillOperation(ICell sourceCell, CellRange targetRange) {
			CellPosition topLeft = targetRange.TopLeft;
			CellPosition bottomRight = targetRange.BottomRight;
			int startRowIndex = topLeft.Row;
			int endRowIndex = bottomRight.Row;
			int startColumnIndex = topLeft.Column;
			int endColumnIndex = bottomRight.Column;
			for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++) {
				for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++) {
					PerformFillOperation(sourceCell, new CellPosition(columnIndex, rowIndex));
					if (CheckPermissionMode && !IsFillAllowed)
						return;
				}
			}
		}
		protected virtual void PerformFillOperation(ICell sourceCell, CellPosition targetPosition) {
			if (sourceCell.Position.EqualsPosition(targetPosition))
				return;
			Worksheet sheet = sourceCell.Worksheet;
			ICell targetCell = (ICell)sheet.TryGetCell(targetPosition.Column, targetPosition.Row);
			if (targetCell == null && !sourceCell.ShouldUseInLayout)
				return;
			if (CheckPermissionMode) {
				if (targetCell == null)
					targetCell = new FakeCell(targetPosition, sheet);
				if (targetCell.ActualProtection.Locked)
					IsFillAllowed = false;
			}
			else {
				if (targetCell == null)
					targetCell = sheet[targetPosition];
				PerformFillOperation(sourceCell, targetCell);
			}
		}
		void PerformFillOperation(ICell sourceCell, ICell targetCell) {
			targetCell.CellCopyFormatSameDocumentModels(sourceCell);
			if (sourceCell.HasFormula) {
			}
			else {
				targetCell.Value = sourceCell.Value;
			}
		}
		void PerformFillOperation(Column sourceColumn, CellRange range) {
			PerformFillColumnOperation(sourceColumn, range.TopLeft.Column, range.BottomRight.Column);
		}
		void PerformFillColumnOperation(Column sourceColumn, int startTargetColumnIndex, int endTargetColumnIndex) {
			IList<Column> targetColumns = sourceColumn.Sheet.Columns.GetColumnRangesEnsureExist(startTargetColumnIndex, endTargetColumnIndex);
			foreach (Column column in targetColumns) {
				PerformFillColumnOperation(sourceColumn, column);
				if (CheckPermissionMode && !IsFillAllowed)
					return;
			}
		}
		void PerformFillColumnOperation(Column sourceColumn, Column targetColumn) {
			if (CheckPermissionMode) {
				if (targetColumn.ActualProtection.Locked)
					IsFillAllowed = false;
			}
			else
				targetColumn.CopyFrom(sourceColumn);
		}
		void PerformFillOperation(Row sourceRow, CellRange range) {
			int startRowIndex = range.TopLeft.Row;
			int endRowIndex = range.BottomRight.Row;
			for (int i = startRowIndex; i <= endRowIndex; i++) {
				if (i != sourceRow.Index) {
					PerformFillRowOperation(sourceRow, i);
					if (CheckPermissionMode && !IsFillAllowed)
						return;
				}
			}
		}
		void PerformFillRowOperation(Row sourceRow, int targetRowIndex) {
			Worksheet sheet = (Worksheet)sourceRow.Sheet;
			Row actualTargetRow = sheet.Rows.TryGetRow(targetRowIndex);
			Row targetRow = actualTargetRow;
			if (targetRow == null)
				targetRow = new Row(targetRowIndex, sheet);
			if (sourceRow.HasSameFormatting(targetRow))
				return;
			if (CheckPermissionMode) {
				if (targetRow.ActualProtection.Locked)
					IsFillAllowed = false;
			}
			else {
				if (actualTargetRow == null)
					actualTargetRow = sheet.Rows[targetRowIndex];
				actualTargetRow.CopyFrom(sourceRow);
			}
		}
		protected abstract CellRange CalculateSourceRange(CellRange range);
		protected abstract Row CalculateSourceRow(CellRange range);
		protected abstract Column CalculateSourceColumn(CellRange range);
		protected abstract CellRange CalculateVectorTargetRange(ICell sourceCell, CellRange range);
	}
}
