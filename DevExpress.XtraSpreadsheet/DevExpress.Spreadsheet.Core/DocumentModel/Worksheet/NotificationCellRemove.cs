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
using DevExpress.Office.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region EventArgs
	public class SheetRangeInsertingEventArgs : EventArgs {
		public SheetRangeInsertingEventArgs(InsertRangeNotificationContext notificationContext)
			: base() {
			NotificationContext = notificationContext;
		}
		public InsertRangeNotificationContext NotificationContext { get; private set; }
	}
	public class SheetRangeRemovingEventArgs : EventArgs {
		public SheetRangeRemovingEventArgs(RemoveRangeNotificationContext notificationContext)
			: base() {
			NotificationContext = notificationContext;
		}
		public RemoveRangeNotificationContext NotificationContext { get; private set; }
	}
	#endregion
	public enum NotificationChecks {
		ArrayFormula = 1,
		AutoFilter = 2,
		MergedCells = 4,
		PivotTable = 8,
		ProtectedCells = 16,
		RangeBounds = 32,
		Table = 64,
		All = 127,
	}
	partial class Worksheet {
		public event EventHandler<SheetRangeInsertingEventArgs> RangeInserting;
		public event EventHandler<SheetRangeRemovingEventArgs> RangeRemoving;
		protected void RaiseRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (SheetId != notificationContext.Range.SheetId)
				return;
			if (RangeInserting != null)
				RangeInserting(this, new SheetRangeInsertingEventArgs(notificationContext));
		}
		protected void RaiseRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			if (SheetId != notificationContext.Range.SheetId)
				return;
			if (RangeRemoving != null)
				RangeRemoving(this, new SheetRangeRemovingEventArgs(notificationContext));
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			DataContext.PushCurrentWorksheet(this);
			try {
				DefinedNames.OnRangeInserting(notificationContext);
				ConditionalFormattings.OnRangeInserting(notificationContext);
				DataValidations.OnRangeInserting(notificationContext);
				if (SheetId == notificationContext.Range.SheetId) {
					AutoFilter.OnRangeInserting(notificationContext);
					SharedFormulas.OnRangeInserting(notificationContext);
					ArrayFormulaRanges.OnRangeInserting(notificationContext);
					Hyperlinks.OnRangeInserting(notificationContext);
					MergedCells.OnRangeInserting(notificationContext);
					Tables.OnRangeInserting(notificationContext);
					PivotTables.OnRangeInserting(notificationContext);
					Comments.OnRangeInserting(notificationContext);
					ProtectedRanges.OnRangeInserting(notificationContext);
					WebRanges.OnRangeInserting(notificationContext);
					CellTags.OnRangeInserting(notificationContext);
					SparklineGroups.OnRangeInserting(notificationContext);
					IgnoredErrors.OnRangeInserting(notificationContext);
					DrawingObjects.OnRangeInserting(notificationContext);
				}
				if (notificationContext.Mode == InsertCellMode.ShiftCellsRight)
					OnRangeInsertingShiftRight(notificationContext);
				else
					OnRangeInsertingShiftDown(notificationContext);
				RaiseRangeInserting(notificationContext);
			}
			finally {
				DataContext.PopCurrentWorksheet();
			}
		}
		protected internal CellRange GetSourceFormatRange(CellRange cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode) {
			if (formatMode == InsertCellsFormatMode.ClearFormat)
				return null;
			int top = cellRange.TopLeft.Row;
			int left = cellRange.TopLeft.Column;
			int bottom = cellRange.BottomRight.Row;
			int right = cellRange.BottomRight.Column;
			if (mode == InsertCellMode.ShiftCellsRight) {
				if (formatMode == InsertCellsFormatMode.FormatAsPrevious)
					right = left = left - 1;
				else
					right = left = right + 1;
			}
			if (mode == InsertCellMode.ShiftCellsDown) {
				if (formatMode == InsertCellsFormatMode.FormatAsPrevious)
					top = bottom = top - 1;
				else
					top = bottom = bottom + 1;
			}
			if (top < 0 || bottom < 0 || left < 0 || right < 0)
				return null;
			if (cellRange.RangeType == CellRangeType.IntervalRange) {
				if (mode == InsertCellMode.ShiftCellsRight)
					return CellIntervalRange.CreateColumnInterval(cellRange.Worksheet, left, PositionType.Absolute, left, PositionType.Absolute);
				else
					return CellIntervalRange.CreateRowInterval(cellRange.Worksheet, bottom, PositionType.Absolute, bottom, PositionType.Absolute);
			}
			return new CellRange(cellRange.Worksheet, new CellPosition(left, top), new CellPosition(right, bottom));
		}
		void OnRangeInsertingShiftRight(InsertRangeNotificationContext notificationContext) {
			OnRangeInsertingShiftRightCellsUpdate(notificationContext);
			OnRangeInsertingShiftRightFormulaUpdate(notificationContext);
		}
		void OnRangeInsertingShiftRightCellsUpdate(InsertRangeNotificationContext notificationContext) {
			CellRange insertableRange = notificationContext.Range;
			if (insertableRange.Worksheet.SheetId != SheetId)
				return;
			CellIntervalRange cellIntervalRange = insertableRange as CellIntervalRange;
			if (cellIntervalRange != null) {
				if (cellIntervalRange.IsColumnInterval) {
					Columns.InsertRange(insertableRange.TopLeft.Column, insertableRange.BottomRight.Column - insertableRange.TopLeft.Column + 1, notificationContext.FormatMode);
					ColumnBreaks.OnRangeInserting(insertableRange.TopLeft.Column, insertableRange.BottomRight.Column);
					columnGroupCache = null;
				}
				else
					Exceptions.ThrowInvalidOperationException("RowInterval can't inserting by InsertCellMode.ShiftCellsRight mode.");
			}
			CellRange shiftedRange = new CellRange(insertableRange.Worksheet,
				new CellPosition(insertableRange.TopLeft.Column, insertableRange.TopLeft.Row),
				new CellPosition(MaxColumnCount - 1, insertableRange.BottomRight.Row));
			List<ICellBase> shiftedRightCells = new List<ICellBase>();
			foreach (ICellBase cellInfo in this.GetExistingCells()) {
				if (shiftedRange.ContainsCell(cellInfo))
					shiftedRightCells.Add(cellInfo);
			}
			for (int i = shiftedRightCells.Count - 1; i >= 0; i--)
				shiftedRightCells[i].OffsetColumnIndex(insertableRange.Width);
			ApplyFormat(GetSourceFormatRange(insertableRange, InsertCellMode.ShiftCellsRight, notificationContext.FormatMode), insertableRange);
		}
		void OnRangeInsertingShiftRightFormulaUpdate(InsertRangeNotificationContext notificationContext) {
			foreach (ICellBase cellInfo in this.GetExistingCells()) {
				ICell cell = cellInfo as ICell;
				if (cell != null)
					cell.OnRangeInsertingShiftRight(notificationContext);
			}
		}
		void OnRangeInsertingShiftDown(InsertRangeNotificationContext notificationContext) {
			OnRangeInsertingShiftDownCellsUpdate(notificationContext);
			OnRangeInsertingShiftDownFormulaUpdate(notificationContext);
		}
		void OnRangeInsertingShiftDownCellsUpdate(InsertRangeNotificationContext notificationContext) {
			CellRange insertableRange = notificationContext.Range;
			if (insertableRange.Worksheet.SheetId != SheetId)
				return;
			CellIntervalRange cellIntervalRange = insertableRange as CellIntervalRange;
			if (cellIntervalRange != null) {
				if (cellIntervalRange.IsRowInterval) {
					Rows.InsertRowsShiftDown(insertableRange.TopLeft.Row, insertableRange.Height);
					RowBreaks.OnRangeInserting(insertableRange.TopLeft.Row, insertableRange.BottomRight.Row);
					rowGroupCache = null;
				}
				else
					Exceptions.ThrowInvalidOperationException("ColumnInterval can't removing by RemoveCellMode.ShiftCellsUp mode.");
			}
			CellRange shiftedRange = new CellRange(insertableRange.Worksheet,
					new CellPosition(insertableRange.TopLeft.Column, insertableRange.TopLeft.Row),
					new CellPosition(insertableRange.BottomRight.Column, MaxRowCount - 1));
			List<ICellBase> shiftedDownCells = new List<ICellBase>();
			HashSet<int> newRowIndices = new HashSet<int>();
			foreach (ICellBase cellInfo in shiftedRange.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (cell == null)
					continue;
				shiftedDownCells.Add(cell);
				if (!newRowIndices.Contains(cell.RowIndex))
					newRowIndices.Add(cell.RowIndex);
			}
			bool needChangeRow = insertableRange.RangeType != CellRangeType.IntervalRange;
			if (needChangeRow) {
				int shiftedRangeHeight = insertableRange.Height;
				foreach (int index in newRowIndices)
					rows.GetRow(index + shiftedRangeHeight);
			}
			for (int i = shiftedDownCells.Count - 1; i >= 0; i--)
				shiftedDownCells[i].OffsetRowIndex(insertableRange.Height, needChangeRow);
			ApplyFormat(GetSourceFormatRange(insertableRange, InsertCellMode.ShiftCellsDown, notificationContext.FormatMode), insertableRange);
		}
		void OnRangeInsertingShiftDownFormulaUpdate(InsertRangeNotificationContext notificationContext) {
			foreach (ICellBase cellInfo in this.GetExistingCells()) {
				ICell cell = cellInfo as ICell;
				if (cell != null)
					cell.OnRangeInsertingShiftDown(notificationContext);
			}
		}
		void ApplyFormat(CellRange source, CellRange target) {
			if (source == null)
				return;
			ModelPasteSpecialFlags formattingWithoutConditionalFormatMergedCellsTables
				= (ModelPasteSpecialFlags.FormatAndStyle & ~ModelPasteSpecialFlags.OtherFormats) | ModelPasteSpecialFlags.RowHeight;
			var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(source, target);
			var copyOperation = new Model.CopyOperation.RangeCopyOperation(ranges, formattingWithoutConditionalFormatMergedCellsTables);
			copyOperation.SuppressChecks = true;
			copyOperation.Execute();
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			DataContext.PushCurrentWorksheet(this);
			DataContext.PushSetValueShouldAffectSharedFormula(false);
			try {
				DefinedNames.OnRangeRemoving(notificationContext);
				SharedFormulas.OnRangeRemoving(notificationContext);
				DataValidations.OnRangeRemoving(notificationContext);
				if (SheetId == notificationContext.Range.SheetId) {
					AutoFilter.OnRangeRemoving(notificationContext);
					Tables.OnRangeRemoving(notificationContext);
					PivotTables.OnRangeRemoving(notificationContext);
					ArrayFormulaRanges.OnRangeRemoving(notificationContext);
					Hyperlinks.OnRangeRemoving(notificationContext);
					MergedCells.OnRangeRemoving(notificationContext);
					Comments.OnRemoving(notificationContext);
					ProtectedRanges.OnRemoving(notificationContext);
					WebRanges.OnRangeRemoving(notificationContext);
					CellTags.OnRangeRemoving(notificationContext);
					DrawingObjects.OnRangeRemoving(notificationContext);
					ConditionalFormattings.OnRangeRemoving(notificationContext);
					SparklineGroups.OnRangeRemoving(notificationContext);
					IgnoredErrors.OnRangeRemoving(notificationContext);
				}
				RemoveCellMode mode = notificationContext.Mode;
				if (mode == RemoveCellMode.ShiftCellsLeft)
					OnRangeRemovingShiftLeft(notificationContext);
				else if (mode == RemoveCellMode.ShiftCellsUp)
					OnRangeRemovingShiftUp(notificationContext);
				else if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
					OnRangeRemovingNoShift(notificationContext);
				else
					OnRangeRemovingDefault(notificationContext);
				SharedFormulas.RemoveMarkedItems();
				RaiseRangeRemoving(notificationContext);
			}
			finally {
				DataContext.PopSetValueShouldAffectSharedFormula();
				DataContext.PopCurrentWorksheet();
			}
		}
		IEnumerable<ICellBase> GetExistingCells(CellRangeBase cellRange) {
			return cellRange.GetExistingCellsEnumerable(true);
		}
		protected internal List<CellRange> GetFilteredRanges(CellRangeBase range) {
			CellIntervalRange boundRange = CellIntervalRange.CreateRowInterval(range.Worksheet, range.TopLeft.Row, PositionType.Relative, range.BottomRight.Row, PositionType.Relative);
			List<CellRange> result = new List<CellRange>();
			List<Table> rangeTables = Tables.GetItems(boundRange, true);
			foreach (Table table in rangeTables) {
				if (table.HasAutoFilter && table.AutoFilter.IsNonDefault) {
					CellRange tableDataRange = table.AutoFilter.GetDataRange();
					if(tableDataRange.Intersects(boundRange))
						result.Add(tableDataRange);
				}
			}
			if (AutoFilter != null && AutoFilter.IsNonDefault) {
				CellRange filterDataRange = AutoFilter.GetDataRange();
				if (filterDataRange.Intersects(boundRange))
					result.Add(filterDataRange);
			}
			return result;
		}
		protected internal bool IsRowFiltered(int rowIndex, List<CellRange> filteredRanges) {
			if (filteredRanges != null) {
				foreach (CellRange range in filteredRanges) {
					if (rowIndex >= range.TopRowIndex && rowIndex <= range.BottomRowIndex)
						return true;
				}
			}
			return false;
		}
		void OnRangeRemovingDefault(RemoveRangeNotificationContext notificationContext) {
			CellRange removableRange = notificationContext.Range;
			if (SheetId != removableRange.SheetId)
				return;
			List<CellRange> filteredRanges = GetFilteredRanges(removableRange);
			foreach (ICell cell in GetExistingCells(removableRange)) {
				if (IsRowFiltered(cell.RowIndex, filteredRanges) && !IsRowVisible(cell.RowIndex))
					continue;
				CellContentSnapshot snapshot;
				if (cell.HasContent) {
					snapshot = new CellContentSnapshot(cell);
					if (notificationContext.ShouldClearCells)
						cell.ClearContent();
					else
						UpdateCellValue(cell);
				}
				else
					snapshot = null;
				if (notificationContext.ClearFormat)
					cell.ClearFormat();
				if (snapshot != null)
					Workbook.InternalAPI.RaiseCellValueChanged(snapshot);
			}
		}
		void UpdateCellValue(ICell cell) {
			Table table = Tables.TryGetItem(cell.Position);
			if (table != null) {
				CellRange headerRange = table.TryGetHeadersRowRange();
				if (headerRange != null && headerRange.ContainsCell(cell.Key)) {
					int columnPosition = cell.ColumnIndex - headerRange.LeftColumnIndex;
					string columnName = table.Columns[columnPosition].Name;
					HistoryItem historyItem = new CellValueChangedHistoryItem(this, cell, cell.Value, columnName);
					Workbook.History.Add(historyItem);
					historyItem.Execute();
					return;
				}
			}
			cell.ClearContent();
		}
		void OnRangeRemovingShiftLeft(RemoveRangeNotificationContext notificationContext) {
			CellRange removableRange = notificationContext.Range;
			if (removableRange.Worksheet.SheetId == SheetId) {
				CellIntervalRange cellIntervalRange = removableRange as CellIntervalRange;
				if (cellIntervalRange != null) {
					if (cellIntervalRange.IsColumnInterval) {
						int leftColumnIndex = cellIntervalRange.LeftColumnIndex;
						int rightColumnIndex = cellIntervalRange.RightColumnIndex;
						foreach (Row row in rows) 
							foreach (ICell cell in row.Cells.GetExistingCells(leftColumnIndex, rightColumnIndex, true)) 
								row.Cells.RemoveAtColumnIndex(cell.ColumnIndex);
						Columns.RemoveRange(leftColumnIndex, rightColumnIndex - leftColumnIndex + 1);
						ColumnBreaks.OnRangeRemoving(leftColumnIndex, rightColumnIndex);
						columnGroupCache = null;
					}
					else
						Exceptions.ThrowInvalidOperationException("RowInterval can't removing by RemoveCellMode.ShiftCellsLeft mode.");
				}
				else 
					RemoveAtRangeColumnIndexes(removableRange);
			}
			CellRange shiftedRange = new CellRange(removableRange.Worksheet,
				new CellPosition(removableRange.BottomRight.Column + 1, removableRange.TopLeft.Row),
				new CellPosition(MaxColumnCount - 1, removableRange.BottomRight.Row));
			foreach (ICellBase cellInfo in GetExistingCells()) {
				ICell cell = cellInfo as ICell;
				if (cell == null)
					continue;
				cell.OnRangeRemovingShiftLeft(notificationContext);
				if (shiftedRange.ContainsCell(cell))
					cell.OffsetColumnIndex(-removableRange.Width);
			}
		}
		void RemoveAtRangeColumnIndexes(CellRange removableRange) {
			foreach (ICellBase cell in GetExistingCells(removableRange)) {
				Row row = Rows[cell.RowIndex];
				row.Cells.RemoveAtColumnIndex(cell.ColumnIndex);
			}
		}
		void OnRangeRemovingShiftUp(RemoveRangeNotificationContext notificationContext) {
			CellRange removableRange = notificationContext.Range;
			if (removableRange.Worksheet.SheetId == SheetId) {
				CellIntervalRange cellIntervalRange = removableRange as CellIntervalRange;
				if (cellIntervalRange != null) {
					if (cellIntervalRange.IsRowInterval) {
						int topRowIndex = cellIntervalRange.TopRowIndex;
						int bottomRowIndex = cellIntervalRange.BottomRowIndex;
						Rows.RemoveRange(topRowIndex, bottomRowIndex - topRowIndex + 1);
						RowBreaks.OnRangeRemoving(topRowIndex, bottomRowIndex);
						rowGroupCache = null;
					}
					else
						Exceptions.ThrowInvalidOperationException("ColumnInterval can't removing by RemoveCellMode.ShiftCellsUp mode.");
				}
				else
					RemoveAtRangeColumnIndexes(removableRange);
			}
			CellRange shiftedRange =
				new CellRange(removableRange.Worksheet,
					new CellPosition(removableRange.TopLeft.Column, removableRange.TopLeft.Row),
					new CellPosition(removableRange.BottomRight.Column, MaxRowCount - 1));
			List<ICellBase> shiftedUpCells = new List<ICellBase>();
			HashSet<int> newRowIndices = new HashSet<int>();
			foreach (ICellBase cellInfo in GetExistingCells()) {
				ICell cell = cellInfo as ICell;
				if (cell == null)
					continue;
				cell.OnRangeRemovingShiftUp(notificationContext);
				if (shiftedRange.ContainsCell(cell) && removableRange.Worksheet.SheetId == SheetId) {
					shiftedUpCells.Add(cell);
					if (!newRowIndices.Contains(cell.RowIndex))
						newRowIndices.Add(cell.RowIndex);
				}
			}
			bool needChangeRow = removableRange.RangeType != CellRangeType.IntervalRange && removableRange.Worksheet.SheetId == SheetId;
			if (needChangeRow) {
				int shiftedRangeHeight = removableRange.Height;
				foreach (int index in newRowIndices)
					rows.GetRow(index - shiftedRangeHeight);
			}
			foreach (ICellBase cell in shiftedUpCells)
				cell.OffsetRowIndex(-removableRange.Height, needChangeRow);
		}
		void OnRangeRemovingNoShift(RemoveRangeNotificationContext notificationContext) {
			foreach (ICellBase cellInfo in GetExistingCells()) {
				ICell cell = cellInfo as ICell;
				if (cell != null)
					cell.OnRangeRemovingNoShift(notificationContext);
			}
		}
		internal IModelErrorInfo CanRangeRemove(CellRangeBase cellRange, RemoveCellMode mode, NotificationChecks checks) {
			if ((checks & NotificationChecks.AutoFilter) > 0)
				if (!AutoFilter.CanRangeRemove(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.AutoFilterCannotBeChanged);
			if ((checks & NotificationChecks.Table) > 0)
				if (!tables.CanRangeRemove(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.TableCanNotBeChanged);
			if ((checks & NotificationChecks.ArrayFormula) > 0)
				if (!arrayFormulaRangesList.CanRangeRemove(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			if ((checks & NotificationChecks.MergedCells) > 0) {
				IModelErrorInfo mergedCellsResult = mergedCells.CheckCanRangeRemove(cellRange, mode);
				if (mergedCellsResult != null)
					return mergedCellsResult;
			}
			if ((checks & NotificationChecks.ProtectedCells) > 0)
				if (RangeRemoveIsPrevented(cellRange))
					return new ModelErrorInfo(ModelErrorType.AttemptToRemoveRangeWithLockedCells);
			if ((checks & NotificationChecks.PivotTable) > 0) {
				IModelErrorInfo pivotTablesResult = PivotTables.CanRangeRemove(cellRange, mode);
				if (pivotTablesResult != null)
					return pivotTablesResult;
			}
			return null;
		}
		bool RangeRemoveIsPrevented(CellRangeBase cellRange) {
			if (Properties.Protection.SheetLocked)
				foreach (ICell cell in cellRange.GetExistingCellsEnumerable())
					if (cell.ActualProtection.Locked)
						return true;
			return false;
		}
		internal IModelErrorInfo CanRangeInsert(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode, NotificationChecks checks) {
			if ((checks & NotificationChecks.AutoFilter) > 0)
				if (!AutoFilter.CanRangeInsert(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.AutoFilterCannotBeChanged);
			if ((checks & NotificationChecks.Table) > 0)
				if (!tables.CanRangeInsert(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.TableCanNotBeChanged);
			if ((checks & NotificationChecks.ArrayFormula) > 0)
				if (!arrayFormulaRangesList.CanRangeInsert(cellRange, mode))
					return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			if ((checks & NotificationChecks.MergedCells) > 0) {
				IModelErrorInfo mergedCellsResult = mergedCells.CheckCanRangeInsert(cellRange, mode, formatMode);
				if (mergedCellsResult != null)
					return mergedCellsResult;
			}
			if ((checks & NotificationChecks.PivotTable) > 0) {
				IModelErrorInfo pivotTablesResult = PivotTables.CanRangeInsert(cellRange, mode);
				if (pivotTablesResult != null)
					return pivotTablesResult;
			}
			if ((checks & NotificationChecks.RangeBounds) > 0)
				if (mode == InsertCellMode.ShiftCellsDown && !CanRangeInsertToTheBottom(cellRange) ||
					mode == InsertCellMode.ShiftCellsRight && !CanRangeInsertToTheLeft(cellRange))
					return new ModelErrorInfo(ModelErrorType.CanNotShiftNonBlankCellsOffOfTheSheet);
			return null;
		}
		bool CanRangeInsertToTheBottom(CellRangeBase cellRange) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)cellRange;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					if (!CanRangeInsertToTheBottom(innerRange))
						return false;
				return true;
			}
			CellPosition topLeft = new CellPosition(cellRange.TopLeft.Column, IndicesChecker.MaxRowCount - cellRange.Height);
			CellPosition bottomRight = new CellPosition(cellRange.BottomRight.Column, IndicesChecker.MaxRowCount - 1);
			CellRange downRange = new CellRange(this, topLeft, bottomRight);
			foreach (ICellBase cell in downRange.GetExistingCellsEnumerable())
				return false;
			return true;
		}
		bool CanRangeInsertToTheLeft(CellRangeBase cellRange) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)cellRange;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					if (!CanRangeInsertToTheLeft(innerRange))
						return false;
				return true;
			}
			CellPosition topLeft = new CellPosition(IndicesChecker.MaxColumnCount - cellRange.Width, cellRange.TopLeft.Row);
			CellPosition bottomRight = new CellPosition(IndicesChecker.MaxColumnCount - 1, cellRange.BottomRight.Row);
			CellRange leftRange = new CellRange(this, topLeft, bottomRight);
			foreach (ICellBase cell in leftRange.GetExistingCellsEnumerable())
				return false;
			return true;
		}
	}
	public static class NotificationsHelper {
		public static bool UpDownShiftAffectsRange(CellRange actionRange, CellRange modelItemRange) {
			return actionRange.TopLeft.Row <= modelItemRange.BottomRight.Row &&
				   actionRange.TopLeft.Column <= modelItemRange.BottomRight.Column &&
				   actionRange.BottomRight.Column >= modelItemRange.TopLeft.Column;
		}
		public static bool LeftRightShiftAffectsRange(CellRange actionRange, CellRange modelItemRange) {
			return actionRange.TopLeft.Column <= modelItemRange.BottomRight.Column &&
				   actionRange.TopLeft.Row <= modelItemRange.BottomRight.Row &&
				   actionRange.BottomRight.Row >= modelItemRange.TopLeft.Row;
		}
		public static bool RangeHeightIsCovered(CellRange actionRange, CellRange modelItemRange) {
			return actionRange.TopLeft.Row <= modelItemRange.TopLeft.Row &&
				   actionRange.BottomRight.Row >= modelItemRange.BottomRight.Row;
		}
		public static bool RangeWidthIsCovered(CellRange actionRange, CellRange modelItemRange) {
			return actionRange.TopLeft.Column <= modelItemRange.TopLeft.Column &&
				   actionRange.BottomRight.Column >= modelItemRange.BottomRight.Column;
		}
	}
}
