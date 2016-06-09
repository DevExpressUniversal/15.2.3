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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections;
using DevExpress.Utils;
using System.IO;
using System.Text.RegularExpressions;
using ICell = DevExpress.XtraSpreadsheet.Model.ICell;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	#region CellsForCopyEnumerable
	public class CellsForCopyEnumerable : IEnumerable<ICell> {
		readonly CellRange rangeSource;
		readonly CellRange rangeTarget;
		ICell sourceCellCurrent;
		ICell targetCellCurrent;
		CellKey currentSourceCellKey;
		CellKey currentTargetCellKey;
		RowsForCopyEnumerable rowsEnumerable;
		bool isReversedCellsEnumerator = false;
		bool isReversedRowsEnumerator = false;
		bool cutMode = false;
		public CellsForCopyEnumerable(CellRange source, CellRange target, bool cutMode) 
			: this(source, target, cutMode, false) {
		}
		public CellsForCopyEnumerable(CellRange source, CellRange target, bool cutMode, bool shouldSkipFilteredRows) {
			this.rangeSource = source;
			this.rangeTarget = target;
			this.cutMode = cutMode;
			this.isReversedCellsEnumerator = CalculateIsReversedHorizontalEnumerator(source, target);
			this.isReversedRowsEnumerator = CalculateIsReversedVertinalEnumerator(source, target);
#if !INCLUDECOPYTESTS
#endif
			this.rowsEnumerable = new RowsForCopyEnumerable(source, target, isReversedRowsEnumerator, shouldSkipFilteredRows);
		}
		Worksheet WorksheetSource { get { return rangeSource.Worksheet as Worksheet; } }
		public ICell SourceCell { get { return sourceCellCurrent; } }
		public ICell TargetCell { get { return targetCellCurrent; } }
		public CellKey CurrentSourceCellkey { get { return currentSourceCellKey; } }
		public CellKey CurrentTargetCellKey { get { return currentTargetCellKey; } }
		public IEnumerator<ICell> GetEnumerator() {
			IEnumerator<Row> en = rowsEnumerable.GetEnumerator();
			while (en.MoveNext()) {
				Row sourceRowCurrent = rowsEnumerable.SourceObjectCurrent;
				Row targetRowCurrent = rowsEnumerable.TargetObjectCurrent;
				if (sourceRowCurrent == null)
					sourceRowCurrent = new Row(rowsEnumerable.SourceObjectIndexCurrent, WorksheetSource);
				CellsInRowForCopyEnumerable cellsInRowEnumerable = new CellsInRowForCopyEnumerable(rangeSource, rangeTarget,
						sourceRowCurrent,
						targetRowCurrent,
						rowsEnumerable.SourceObjectIndexCurrent,
						rowsEnumerable.TargetObjectIndexCurrent, isReversedCellsEnumerator);
				cellsInRowEnumerable.CutMode = cutMode;
				cellsInRowEnumerable.IsSheetProtected = WorksheetSource.IsProtected;
				IEnumerator<ICell> cellsInRowEnumerator = cellsInRowEnumerable.GetEnumerator();
				while (cellsInRowEnumerator.MoveNext()) {
					sourceCellCurrent = cellsInRowEnumerable.SourceObjectCurrent;
					targetCellCurrent = cellsInRowEnumerable.TargetObjectCurrent;
					currentSourceCellKey = cellsInRowEnumerable.CurrentSourceCellkey;
					currentTargetCellKey = cellsInRowEnumerable.CurrentTargetCellKey;
					yield return sourceCellCurrent;
				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (this as IEnumerable<ICell>).GetEnumerator();
		}
		bool CalculateIsReversedHorizontalEnumerator(CellRange sourceRange, CellRange targetRange) {
			if (sourceRange.TopLeft.Column < targetRange.TopLeft.Column
				&& sourceRange.BottomRight.Column < targetRange.BottomRight.Column
				&& sourceRange.Intersects(targetRange))
				return true;
			return false;
		}
		bool CalculateIsReversedVertinalEnumerator(CellRange sourceRange, CellRange targetRange) {
			if (sourceRange.TopLeft.Row < targetRange.TopLeft.Row
				&& sourceRange.BottomRight.Row < targetRange.BottomRight.Row
				&& sourceRange.Intersects(targetRange))
				return true;
			return false;
		}
	}
	#endregion
	#region SourceTargetObjectPairForCopyEnumerable (abstract)
	public abstract class SourceTargetObjectPairForCopyEnumerable<T> : IEnumerable<T> where T : class {
		readonly CellRange rangeSource;
		readonly CellRange rangeTarget;
		int sourceObjectIndexCurrent;
		int targetObjectIndexCurrent;
		T sourceObjectCurrent;
		T targetObjectCurrent;
		readonly int sourceActualFirstObjectIndex;
		readonly int targetActualFirstObjectIndex;
		IOrderedEnumerator<T> sourceObjectEnumerator;
		IOrderedEnumerator<T> targetObjectEnumerator;
		bool sourceValid;
		bool targetValid;
		readonly bool isReversedOrder;
		int deletedSourceObjectIndex;
		protected SourceTargetObjectPairForCopyEnumerable(CellRange source, CellRange target, bool reversed) {
			this.rangeSource = source;
			this.rangeTarget = target;
			this.isReversedOrder = reversed;
			this.sourceActualFirstObjectIndex = isReversedOrder ? SourceFar : SourceNear;
			this.targetActualFirstObjectIndex = isReversedOrder ? TargetFar : TargetNear;
			this.sourceObjectIndexCurrent = Int32.MinValue;  
			this.targetObjectIndexCurrent = Int32.MinValue; 
			this.sourceObjectCurrent = null;
			this.targetObjectCurrent = null;
			this.CutMode = false;
		}
		public CellRange RangeSource { [System.Diagnostics.DebuggerStepThrough] get { return rangeSource; } }
		public CellRange RangeTarget { [System.Diagnostics.DebuggerStepThrough] get { return rangeTarget; } }
		public Worksheet WorksheetSource { [System.Diagnostics.DebuggerStepThrough] get { return rangeSource.Worksheet as Worksheet; } }
		public Worksheet WorksheetTarget { [System.Diagnostics.DebuggerStepThrough] get { return rangeTarget.Worksheet as Worksheet; } }
		public int SourceObjectIndexCurrent { [System.Diagnostics.DebuggerStepThrough] get { return sourceObjectIndexCurrent; } }
		public int TargetObjectIndexCurrent { [System.Diagnostics.DebuggerStepThrough] get { return targetObjectIndexCurrent; } }
		public T SourceObjectCurrent { [System.Diagnostics.DebuggerStepThrough] get { return sourceObjectCurrent; } }
		public T TargetObjectCurrent { [System.Diagnostics.DebuggerStepThrough] get { return targetObjectCurrent; } }
		public bool OnSameWorksheet { [System.Diagnostics.DebuggerStepThrough] get { return Object.ReferenceEquals(WorksheetSource, WorksheetTarget); } }
		public bool IsReversedOrder { [System.Diagnostics.DebuggerStepThrough] get { return isReversedOrder; } }
		public bool CutMode { get; set; }
		protected abstract int SourceNear { [System.Diagnostics.DebuggerStepThrough] get; }
		protected abstract int SourceFar { [System.Diagnostics.DebuggerStepThrough] get; }
		protected abstract int TargetNear { [System.Diagnostics.DebuggerStepThrough] get; }
		protected abstract int TargetFar { [System.Diagnostics.DebuggerStepThrough] get; }
		public IEnumerator<T> GetEnumerator() {
			CreateInnerEnumerators();
			int sourceDistance = 0;
			int targetDistance = 0;
			 deletedSourceObjectIndex = Int32.MinValue;
			while (sourceValid || targetValid) {
				T sourceNearestObject = (sourceValid) ? sourceObjectEnumerator.Current : null;
				T targetNearestObject = (targetValid) ? targetObjectEnumerator.Current : null;
				sourceDistance = (sourceValid)
					? Math.Abs(GetCurrentItemIndex(sourceNearestObject, sourceValid, sourceObjectEnumerator) - sourceActualFirstObjectIndex)
					: Int32.MaxValue;
				targetDistance = (targetValid)
					? Math.Abs(GetCurrentItemIndex(targetNearestObject, targetValid, targetObjectEnumerator) - targetActualFirstObjectIndex)
					: Int32.MaxValue;
				int sign = isReversedOrder ? -1 : 1;
				if (sourceDistance < targetDistance) {
					sourceObjectIndexCurrent = GetCurrentItemIndex(sourceNearestObject, sourceValid, sourceObjectEnumerator);
					targetObjectIndexCurrent = targetActualFirstObjectIndex + sign * sourceDistance;
					sourceObjectCurrent = sourceNearestObject;
					targetObjectCurrent = null;
					bool sourceObjectValidForYield = ValidateSourceObject(sourceObjectCurrent, sourceValid);
					deletedSourceObjectIndex = IndexOfDeletedSourceObject();
					this.sourceValid = sourceObjectEnumerator.MoveNext();
					if (sourceObjectValidForYield)
						CreateTargetObjectAndUpdateEnumerator();
					else
						if (IsSourceRowHasNoCells(sourceObjectCurrent) && ! UseSourceRowContinuousEnumerator)
							continue;
				}
				else if (sourceDistance > targetDistance) {
					targetObjectIndexCurrent = GetCurrentItemIndex(targetNearestObject, targetValid, targetObjectEnumerator);
					sourceObjectIndexCurrent = sourceActualFirstObjectIndex + sign * targetDistance;
					if (ShouldCreateFakeSourceObject())
						sourceObjectCurrent = CreateFakeSourceObject(sourceObjectIndexCurrent);
					else
						sourceObjectCurrent = null;
					targetObjectCurrent = targetNearestObject;
					if (targetValid && targetObjectCurrent == null) 
						CreateTargetObjectAndUpdateEnumerator();
					targetValid = targetObjectEnumerator.MoveNext();
				}
				else if (sourceDistance == targetDistance) {
					sourceObjectIndexCurrent = GetCurrentItemIndex(sourceNearestObject, sourceValid, sourceObjectEnumerator);
					targetObjectIndexCurrent = GetCurrentItemIndex(targetNearestObject, targetValid, targetObjectEnumerator);
					sourceObjectCurrent = sourceNearestObject;
					targetObjectCurrent = targetNearestObject;
					deletedSourceObjectIndex = IndexOfDeletedSourceObject();
					if (targetValid && targetObjectCurrent == null) 
						CreateTargetObjectAndUpdateEnumerator();
					sourceValid = sourceObjectEnumerator.MoveNext();
					targetValid = targetObjectEnumerator.MoveNext();
				}
				BeforeYieldResult(sourceObjectCurrent, sourceObjectIndexCurrent, targetObjectCurrent, targetObjectIndexCurrent);
				yield return sourceObjectCurrent;
				if (CutMode && deletedSourceObjectIndex >= 0) {
					int modelIndex = GetSourceObjectModelIndex(deletedSourceObjectIndex);
					bool shouldShiftTargetEnumerator = ShouldShiftSourceEnumeratorCore &&  
						targetValid 
						&& modelIndex <= TargetFar;
					sourceObjectEnumerator.OnObjectDeleted(deletedSourceObjectIndex); 
					if (shouldShiftTargetEnumerator)
						targetObjectEnumerator.OnObjectDeleted(deletedSourceObjectIndex);
					DeleteSourceObject(deletedSourceObjectIndex);
					deletedSourceObjectIndex = Int32.MinValue;
				}
			}
		}
		protected abstract int GetSourceObjectModelIndex(int deletedSourceObjectIndex);
		protected abstract void DeleteSourceObject(int deletedSourceObjectIndex);
		protected virtual int IndexOfDeletedSourceObject() {
			return Int32.MinValue;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (this as IEnumerable<T>).GetEnumerator();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected void CreateInnerEnumerators() {
			IList<IOrderedEnumerator<T>> sourceEnumerators = CreateEnumeratorsSource();
			IList<IOrderedEnumerator<T>> targetEnumerators = CreateEnumeratorsTarget();
			sourceObjectEnumerator = JoinEnumerators(sourceEnumerators);
			targetObjectEnumerator = JoinEnumerators(targetEnumerators);
			this.sourceValid = (sourceObjectEnumerator != null) ? sourceObjectEnumerator.MoveNext() : false;
			this.targetValid = (targetObjectEnumerator != null) ? targetObjectEnumerator.MoveNext() : false;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		IOrderedEnumerator<T> JoinEnumerators(IList<IOrderedEnumerator<T>> sourceEnumerators) {
			int count = sourceEnumerators.Count;
			IOrderedEnumerator<T> result = sourceEnumerators.FirstOrDefault<IOrderedEnumerator<T>>();
			for (int index = 1; index < count; index++) {
				IOrderedEnumerator<T> second = sourceEnumerators[index];
				IOrderedEnumerator<T> joined = new JoinedOrderedEnumerator<T>(result, second);
				result = joined;
			}
			return result;
		}
		void CreateTargetObjectAndUpdateEnumerator() {
			int missingTargetObjectIndex = targetObjectIndexCurrent;
			bool shouldShiftSourceEnumerator = ShouldShiftSourceEnumeratorCore 
				&& sourceValid && missingTargetObjectIndex <= sourceObjectEnumerator.CurrentValueOrder;
			this.targetObjectCurrent = GetTargetObjectByModelIndex(missingTargetObjectIndex); 
			if (shouldShiftSourceEnumerator) 
				sourceObjectEnumerator.OnObjectInserted(missingTargetObjectIndex);
			targetObjectEnumerator.OnObjectInserted(missingTargetObjectIndex);
			if (ShouldShiftSourceEnumeratorCore &&
				deletedSourceObjectIndex >= 0 && missingTargetObjectIndex <= sourceObjectIndexCurrent) {
				deletedSourceObjectIndex++;
			}
		}
		int GetCurrentItemIndex(T current, bool isValid, IOrderedEnumerator<T> enumerator) {
			if (current != null)
				return GetObjectModelIndex(current);
			if (isValid && current == null) {
				return enumerator.CurrentValueOrder;
			}
			return Int32.MaxValue;
		}
		protected abstract void BeforeYieldResult(T sourceObjectCurrent, int sourceObjectIndexCurrent, T targetObjectCurrent, int targetObjectIndexCurrent);
		protected abstract bool ValidateSourceObject(T sourceObjectCurrent, bool sourceValid);
		protected abstract bool IsSourceRowHasNoCells(T sourceObjectCurrent);
		protected abstract int GetObjectModelIndex(T current);
		protected abstract T GetTargetObjectByModelIndex(int targetObjectIndex);
		protected abstract bool ShouldShiftSourceEnumeratorCore { get; }
		protected abstract bool ShouldCreateFakeSourceObject();
		protected abstract T CreateFakeSourceObject(int modelIndex);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected abstract IList<IOrderedEnumerator<T>> CreateEnumeratorsSource();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected abstract IList<IOrderedEnumerator<T>> CreateEnumeratorsTarget();
		protected abstract bool UseSourceRowContinuousEnumerator { get; }
	}
	#endregion
	public class RowsForCopyEnumerable : SourceTargetObjectPairForCopyEnumerable<Row> {
		bool yieldAnyExistingSourceObject;
		readonly bool someSourceColumnsExistsInSourceRange; 
		List<CellRange> filteredRanges;
		public RowsForCopyEnumerable(CellRange source, CellRange target, bool reversed)
			: this(source, target, reversed, false) {
		}
		public RowsForCopyEnumerable(CellRange source, CellRange target, bool reversed, bool shouldSkipFilteredRows)
			: base(source, target, reversed) {
			yieldAnyExistingSourceObject = false;
			this.someSourceColumnsExistsInSourceRange = ShouldCreateContinuousEnumerator(source);
			this.filteredRanges = shouldSkipFilteredRows ? WorksheetTarget.GetFilteredRanges(target) : null;
		}
		protected override int SourceNear { [System.Diagnostics.DebuggerStepThrough] get { return RangeSource.TopLeft.Row; } }
		protected override int SourceFar { [System.Diagnostics.DebuggerStepThrough] get { return RangeSource.BottomRight.Row; } }
		protected override int TargetNear { [System.Diagnostics.DebuggerStepThrough] get { return RangeTarget.TopLeft.Row; } }
		protected override int TargetFar { [System.Diagnostics.DebuggerStepThrough] get { return RangeTarget.BottomRight.Row; } }
		public bool YieldAnyExistingSourceRow { get { return yieldAnyExistingSourceObject; } set { yieldAnyExistingSourceObject = value; } }
		protected override bool ShouldShiftSourceEnumeratorCore { get { return OnSameWorksheet; } }
		bool HasFilteredRows { get { return filteredRanges != null && filteredRanges.Count > 0; } }
		static bool ShouldCreateContinuousEnumerator(CellRange range) {
			Model.CellIntervalRange intervalRange = range as Model.CellIntervalRange;
			bool isWholeWorksheetInterval = intervalRange != null && intervalRange.IsWholeWorksheetRange();
			if (range.IsColumnRangeInterval() || isWholeWorksheetInterval)
				return false;
			IEnumerator<IColumnRange> existingColumns = (range.Worksheet as Worksheet).Columns.
				GetExistingColumnRangesEnumerator(range.TopLeft.Column, range.BottomRight.Column, false);
			bool isValid = existingColumns.MoveNext();
			int defaultFormatIndex = range.Worksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			while (isValid && existingColumns.Current.FormatIndex == defaultFormatIndex)
				isValid = existingColumns.MoveNext();
			return isValid;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected override IList<IOrderedEnumerator<Row>> CreateEnumeratorsSource() {
			List<IOrderedEnumerator<Row>> result = new List<IOrderedEnumerator<Row>>();
			IOrderedEnumerator<Row> existingRows = WorksheetSource.Rows.GetExistingRowsEnumerator(SourceNear, SourceFar, IsReversedOrder, ShouldProcessRow) as ExistingRowsEnumerator<Row>;
			result.Add(existingRows);
			if (someSourceColumnsExistsInSourceRange) {
				IOrderedEnumerator<Row> fakeRows = HasFilteredRows ? 
					new ContinuousRowsEnumerator(WorksheetSource, SourceNear, SourceFar, IsReversedOrder, ShouldProcessRow) :
					new ContinuousRowsEnumeratorFakeRowsAsNull(WorksheetSource, SourceNear, SourceFar, IsReversedOrder, null);
				result.Add(fakeRows);
			}
			return result;
		}
		protected override bool UseSourceRowContinuousEnumerator { get { return someSourceColumnsExistsInSourceRange; } }
		protected override IList<IOrderedEnumerator<Row>> CreateEnumeratorsTarget() {
			List<IOrderedEnumerator<Row>> result = new List<IOrderedEnumerator<Row>>();
			IOrderedEnumerator<Row> existingRows = WorksheetTarget.Rows.GetExistingRowsEnumerator(TargetNear, TargetFar, IsReversedOrder, ShouldProcessRow) as ExistingRowsEnumerator<Row>;
			result.Add(existingRows);
			bool shouldCreateContiniousEnumerator = ShouldCreateContinuousEnumerator(RangeTarget);
			if (shouldCreateContiniousEnumerator) {
				IOrderedEnumerator<Row> fakeRows = HasFilteredRows ? 
					new ContinuousRowsEnumerator(WorksheetTarget, TargetNear, TargetFar, IsReversedOrder, ShouldProcessRow) : 
					new ContinuousRowsEnumeratorFakeRowsAsNull(WorksheetTarget, TargetNear, TargetFar, IsReversedOrder, null);
				result.Add(fakeRows);
			}
			return result;
		}
		protected override int GetObjectModelIndex(Row current) {
			return current.Index;
		}
		protected override int GetSourceObjectModelIndex(int deletedSourceObjectIndex) {
			Row row = WorksheetSource.Rows.InnerList[deletedSourceObjectIndex];
			return row.Index;
		}
		protected override Row GetTargetObjectByModelIndex(int targetObjectIndex) {
			return WorksheetTarget.Rows.GetRow(targetObjectIndex);
		}
		protected override bool ValidateSourceObject(Row sourceObjectCurrent, bool sourceValid) {
			bool sourceIsNotEmpty = IsSourceRowContainsCellsInRange(sourceObjectCurrent, RangeSource);
			bool sourceHasFormattingForFakeCells = IsSourceRowHasRowFormattingForFakeCells(sourceObjectCurrent, RangeSource, sourceValid);
			return YieldAnyExistingSourceRow
						|| sourceIsNotEmpty
						|| sourceHasFormattingForFakeCells
						|| this.someSourceColumnsExistsInSourceRange;
		}
		protected override bool IsSourceRowHasNoCells(Row sourceObjectCurrent) {
			bool sourceObjectIsEmpty = !IsSourceRowContainsCellsInRange(sourceObjectCurrent, RangeSource);
			return sourceObjectIsEmpty;
		}
		bool IsSourceRowContainsCellsInRange(Row sourceRowCurrent, CellRange sourceRange) {
			if (sourceRowCurrent == null)
				return false;
			return sourceRowCurrent.Cells.Count != 0
				&& (sourceRowCurrent.Cells.GetExistingCellsEnumerator(sourceRange.TopLeft.Column, sourceRange.BottomRight.Column, false) as IEnumerator<ICell>).MoveNext();
		}
		bool IsSourceRowHasRowFormattingForFakeCells(Row sourceRowCurrent, CellRange sourceRange, bool sourceValid) {
			if (sourceRowCurrent == null && !sourceValid)
				throw new InvalidOperationException(); 
			if (sourceRowCurrent == null && sourceValid)
				return false;
			bool rowHasFormatting = sourceRowCurrent.FormatIndex > sourceRowCurrent.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			bool andSourceRangeIsNotRowIntervalRange = !sourceRange.IsRowRangeInterval();
			bool andSourceRangeIsNotWholeWorksheetRange = !(
				sourceRange.IsColumnRangeInterval() && CellRange.CheckIsRowRangeInterval(sourceRange.TopLeft, sourceRange.BottomRight, sourceRange.Worksheet as Worksheet));
			return (rowHasFormatting && andSourceRangeIsNotRowIntervalRange && andSourceRangeIsNotWholeWorksheetRange);
		}
		protected override void BeforeYieldResult(Row sourceObjectCurrent, int sourceObjectIndexCurrent, Row targetObjectCurrent, int targetObjectIndexCurrent) {
			Guard.ArgumentNotNull(targetObjectCurrent, "get targetRow is null");
			Guard.ArgumentNotNull(GetTargetObjectByModelIndex(targetObjectIndexCurrent), "targetRow not existing in sheet.rows (1)");
			Guard.ReferenceEquals(targetObjectCurrent, GetTargetObjectByModelIndex(targetObjectIndexCurrent));
		}
		protected override int IndexOfDeletedSourceObject() {
			return Int32.MinValue;
		}
		protected override void DeleteSourceObject(int deletedSourceObjectIndex) {
			throw new InvalidOperationException();
		}
		protected override bool ShouldCreateFakeSourceObject() { return false; }
		protected override Row CreateFakeSourceObject(int index) {
			throw new InvalidOperationException();
		}
		bool ShouldProcessRow(Row row) {
			if (row == null || filteredRanges == null)
				return true;
			return !WorksheetTarget.IsRowFiltered(row.Index, filteredRanges) || WorksheetTarget.IsRowVisible(row.Index);
		}
	}
	public class CellsInRowForCopyEnumerable : SourceTargetObjectPairForCopyEnumerable<ICell> {
		readonly bool onSameRow;
		readonly Row sourceRow;
		readonly Row targetRow;
		readonly int sourceRowIndex;
		readonly int targetRowIndex;
		CellsEnumerator<ICell> existingSourceCellsEnumerable;
		CellKey currentSourceCellKey;
		CellKey currentTargetCellKey;
		int defaultCellFormatIndexSource;
		public CellsInRowForCopyEnumerable(CellRange source, CellRange target, Row _sourceRow, Row _targetRow, int _sourceRowIndex, int _targetRowIndex, bool reversedOrder)
			: base(source, target, reversedOrder) {
			this.sourceRow = _sourceRow;
			this.targetRow = _targetRow;
			this.sourceRowIndex = _sourceRowIndex;
			this.targetRowIndex = _targetRowIndex;
			this.onSameRow = (Object.ReferenceEquals(source.Worksheet, target.Worksheet)
				&& (this.sourceRowIndex == this.targetRowIndex));
			this.defaultCellFormatIndexSource = sourceRow.DocumentModel.StyleSheet.DefaultCellFormatIndex;
		}
		protected override bool ShouldShiftSourceEnumeratorCore { get { return onSameRow; } }
		public CellKey CurrentSourceCellkey { [System.Diagnostics.DebuggerStepThrough] get { return currentSourceCellKey; } }
		public CellKey CurrentTargetCellKey { [System.Diagnostics.DebuggerStepThrough] get { return currentTargetCellKey; } }
		protected override int SourceNear { [System.Diagnostics.DebuggerStepThrough] get { return RangeSource.TopLeft.Column; } }
		protected override int SourceFar { [System.Diagnostics.DebuggerStepThrough] get { return RangeSource.BottomRight.Column; } }
		protected override int TargetNear { [System.Diagnostics.DebuggerStepThrough] get { return RangeTarget.TopLeft.Column; } }
		protected override int TargetFar { [System.Diagnostics.DebuggerStepThrough] get { return RangeTarget.BottomRight.Column; } }
		protected internal bool IsSheetProtected { get; set; }
		protected override void BeforeYieldResult(ICell sourceCellCurrent, int sourceColumnIndexCurrent, ICell targetCellCurrent, int targetColumnIndexCurrent) {
			currentSourceCellKey = (sourceCellCurrent != null) ? sourceCellCurrent.Key : new CellKey(WorksheetSource.SheetId, sourceColumnIndexCurrent, sourceRowIndex);
			currentTargetCellKey = (targetCellCurrent != null) ? targetCellCurrent.Key : new CellKey(WorksheetTarget.SheetId, targetColumnIndexCurrent, targetRowIndex);
		}
		protected override bool ValidateSourceObject(ICell sourceObjectCurrent, bool sourceValid) {
			return true;
		}
		protected override bool IsSourceRowHasNoCells(ICell sourceObjectCurrent) {
			return false; 
		}
		protected override int GetObjectModelIndex(ICell current) {
			return current.Key.ColumnIndex;
		}
		protected override int GetSourceObjectModelIndex(int indexInCollection) {
			return sourceRow.Cells.InnerList[indexInCollection].Key.ColumnIndex;
		}
		protected override ICell GetTargetObjectByModelIndex(int targetColumnIndex) {
			return this.targetRow.Cells.GetCell(targetColumnIndex);
		}
		bool ShouldCreateFakeCellsInRowOnExistingColumnEnumerator(CellRange range1, CellRange range2) {
			Model.CellIntervalRange intervalRange = range1 as Model.CellIntervalRange;
			bool isWholeWorksheetInterval = intervalRange != null && intervalRange.IsWholeWorksheetRange();
			if (range1.IsColumnRangeInterval() || isWholeWorksheetInterval)
				return false;
			if (OnSameWorksheet && range1.EqualsPosition(range2))
				return false;
			Worksheet worksheet1 = range1.Worksheet as Worksheet;
			IEnumerator<IColumnRange> existingColumnsFromRange1 = worksheet1.Columns.GetExistingColumnRangesEnumerator(range1.TopLeft.Column, range1.BottomRight.Column, false);
			bool isValid = existingColumnsFromRange1.MoveNext();
			int defaultFormatIndex = range1.Worksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			while (isValid) {
				IColumnRange columnFromRange1 = existingColumnsFromRange1.Current;
				bool hasCustomFormatting = columnFromRange1.FormatIndex != defaultFormatIndex;
				if (hasCustomFormatting) {
					if (!OnSameWorksheet)
						break;
					if (range1.LeftColumnIndex == range2.LeftColumnIndex)
						return false;
					int targetRangeOffset = range2.LeftColumnIndex - range1.LeftColumnIndex;
					int targetColumnRangeLeftIndex = columnFromRange1.StartIndex + targetRangeOffset;
					int targetColumnRangeRightIndex = columnFromRange1.EndIndex + targetRangeOffset;
					IColumnRange targetColumnAtTargetStartPosition = worksheet1.Columns.TryGetColumnRange(targetColumnRangeLeftIndex);
					if (targetColumnAtTargetStartPosition == null)
						return true; 
					if (targetColumnAtTargetStartPosition.FormatIndex == columnFromRange1.FormatIndex) {
						isValid = existingColumnsFromRange1.MoveNext();
						continue;
					}
					else if (targetColumnAtTargetStartPosition.FormatIndex != defaultFormatIndex) {
						return true;
					}
				}
				isValid = existingColumnsFromRange1.MoveNext();
			}
			return isValid;
		}
		bool IsRowHasFormattingForFakeCells(CellRange range, Row row, CellRange anotherRange) {
			if (row == null)
				return false;
			Model.CellIntervalRange intervalRange = range as Model.CellIntervalRange;
			bool isWholeWorksheetInterval = intervalRange != null && intervalRange.IsWholeWorksheetRange();
			if (range.IsRowRangeInterval() || isWholeWorksheetInterval)
				return false;
			if (OnSameWorksheet && range.TopRowIndex == anotherRange.TopRowIndex)
				return false; 
			return row.FormatIndex > row.DocumentModel.StyleSheet.DefaultCellFormatIndex;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		protected override IList<IOrderedEnumerator<ICell>> CreateEnumeratorsSource() {
			List<IOrderedEnumerator<ICell>> result = new List<IOrderedEnumerator<ICell>>();
			Row row = sourceRow;
			if (row == null)
				return result;
			int rowIndex = this.sourceRowIndex;
			bool fakeCellsAsNull = false;
			ICellCollection cells = row.Cells;
			this.existingSourceCellsEnumerable = cells.GetExistingCellsEnumerator(SourceNear, SourceFar, IsReversedOrder) as CellsEnumerator<ICell>;
			result.Add(existingSourceCellsEnumerable);
			if (ShouldCreateFakeCellsInRowOnExistingColumnEnumerator(RangeSource, RangeTarget))
				result.Add(new FakeCellsInRowOnExistingColumnsWithFormattingEnumerator(WorksheetSource, SourceNear, SourceFar, IsReversedOrder, rowIndex, fakeCellsAsNull));
			if (IsRowHasFormattingForFakeCells(RangeSource, row, RangeTarget))
				result.Add(new ContinuousCellsEnumerator(cells.InnerList, SourceNear, SourceFar, IsReversedOrder, WorksheetSource, rowIndex));
			return result;
		}
		protected override bool UseSourceRowContinuousEnumerator { get { return false; } } 
		protected override IList<IOrderedEnumerator<ICell>> CreateEnumeratorsTarget() {
			List<IOrderedEnumerator<ICell>> result = new List<IOrderedEnumerator<ICell>>();
			Row row = targetRow;
			if (row == null)
				return result;
			int rowIndex = this.targetRowIndex;
			bool fakeCellsAsNull = true;
			ICellCollection cells = row.Cells;
			result.Add(cells.GetExistingCellsEnumerator(TargetNear, TargetFar, IsReversedOrder));
			if (ShouldCreateFakeCellsInRowOnExistingColumnEnumerator(RangeTarget, RangeSource))
				result.Add(new FakeCellsInRowOnExistingColumnsWithFormattingEnumerator(WorksheetTarget, TargetNear, TargetFar, IsReversedOrder, rowIndex, fakeCellsAsNull));
			if (IsRowHasFormattingForFakeCells(RangeTarget, row, RangeSource))
				result.Add(new ContinuousCellsEnumeratorFakeCellsAsNull(cells.InnerList, TargetNear, TargetFar, IsReversedOrder, WorksheetTarget, rowIndex));
			return result;
		}
		protected override int IndexOfDeletedSourceObject() {
			if (!CutMode || SourceObjectCurrent == null || SourceObjectCurrent is FakeCell)
				return Int32.MinValue;
			if (CutMode && RangeTarget.ContainsCell(SourceObjectCurrent)) 
				return Int32.MinValue;
			if (CutMode && IsSheetProtected && !SourceObjectCurrent.FormatInfo.ActualProtection.Locked)
				return Int32.MinValue;
			return existingSourceCellsEnumerable.DebugInnerIndex;
		}
		protected override void DeleteSourceObject(int deletedSourceObjectIndex) {
			CellCollection sourceRowCells = sourceRow.Cells as CellCollection;
			if(sourceRowCells != null)
				sourceRowCells.RemoveAt(deletedSourceObjectIndex);
		}
		protected override bool ShouldCreateFakeSourceObject() {
			return sourceRow != null && sourceRow.FormatIndex > defaultCellFormatIndexSource;
		}
		protected override ICell CreateFakeSourceObject(int sourceColumnIndexCurrent) {
			return new FakeCell(new CellPosition(sourceColumnIndexCurrent, sourceRowIndex), WorksheetSource);
		}
	}
}
