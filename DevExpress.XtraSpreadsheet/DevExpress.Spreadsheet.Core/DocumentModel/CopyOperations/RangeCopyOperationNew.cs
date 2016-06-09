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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class RangeCopyOperation : CopyEverythingBase {
		TableColumnNamesCorrectedOperationBase tableColumnNamesCorrectedOperation;
		public RangeCopyOperation(SourceTargetRangesForCopy ranges, ModelPasteSpecialFlags pasteSpecialOptions)
			: base(ranges, pasteSpecialOptions) {
			this.tableColumnNamesCorrectedOperation = new TableColumnNamesChangeOperation();
		}
		public bool ShouldSkipFilteredRows { get; set; }
		public TableColumnNamesCorrectedOperationBase TableColumnNamesCorrectedOperation { get { return tableColumnNamesCorrectedOperation; } set { tableColumnNamesCorrectedOperation = value; } }
		public static IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode, InsertCellsFormatMode formatMode) {
			CellUnion unionRange = (range as CellUnion);
			if (unionRange != null) {
				IModelErrorInfo innerResult;
				foreach (CellRangeBase innerRange in unionRange.InnerCellRanges) {
					innerResult = CanRangeInsert(innerRange, mode, formatMode);
					if (innerResult != null)
						return innerResult;
				}
				return null;
			}
			CellRange targetRange = range as CellRange;
			Worksheet worksheet = targetRange.Worksheet as Worksheet;
			CellRange sourceRange = worksheet.GetSourceFormatRange(targetRange, mode, formatMode);
			if (formatMode == InsertCellsFormatMode.ClearFormat || sourceRange == null) {
				return null;
			}
			var ranges = new SourceTargetRangesForCopy(sourceRange, targetRange);
			RangeCopyOperation op = new RangeCopyOperation(ranges, ModelPasteSpecialFlags.FormatAndStyle);
			op.SuppressCheckDefinedNames = true;
			return op.Checks();
		}
		protected internal override void ExecuteCore() {
			if (this.RangesInfo.First.PasteSingleCellToMergedRange) {
				if (PasteSpecialOptions.ShouldCopyColumnWidths)
					PasteSpecialOptions.InnerFlags = ModelPasteSpecialFlags.Values | ModelPasteSpecialFlags.ColumnWidths;
				else
					PasteSpecialOptions.InnerFlags = ModelPasteSpecialFlags.Values;
			}
			foreach (RangeCopyInfo rangeInfo in RangesInfo) {
				CopyContent(rangeInfo);
				if (rangeInfo.PasteSingleCellToMergedRange)
					CopyRange(rangeInfo, rangeInfo.TargetRanges[0]);
				else {
					foreach (CellRange target in rangeInfo.TargetRanges) {
						CopyRange(rangeInfo, target);
					}
				}
			}
			LocateArrayFormulaToCells();
		}
		void CopyRange(RangeCopyInfo info, CellRange _currentTargetRange) {
			CellRange source = info.SourceRange;
			CellsForCopyEnumerable sourceAndTargetCellsEnumerable = new 
				CellsForCopyEnumerable(source, _currentTargetRange, CutMode, ShouldSkipFilteredRows);
			IEnumerator<ICell> sourceAndTargetCellsEnumerator = sourceAndTargetCellsEnumerable.GetEnumerator();
			while (sourceAndTargetCellsEnumerator.MoveNext()) {
				ICell sourceCell = sourceAndTargetCellsEnumerable.SourceCell;
				ICell targetCell = sourceAndTargetCellsEnumerable.TargetCell;
				Guard.ArgumentNotNull(targetCell, "CopyRange: targetCell is null");
				CopyCellContent(sourceCell, targetCell, _currentTargetRange);
			}
		}
		protected override void BeforeBeginTargetDocumentModelUpdate() {
		}
		protected override void AfterBeginTargetDocumentModelUpdate() {
		}
		protected override void BeforeEndTargetDocumentModelUpdate() {
			if (tableColumnNamesCorrectedOperation == null)
				return;
			bool changeValuesExpected = PasteSpecialOptions.ShouldCopyValues || PasteSpecialOptions.ShouldCopyFormulas;
			if (!changeValuesExpected)
				return;
			TablesColumnRenamedFormulaWalker walker = new TablesColumnRenamedFormulaWalker(DataContext);
			tableColumnNamesCorrectedOperation.ShouldCorrectColumnFormulas = false;
			tableColumnNamesCorrectedOperation.ShouldUpdateColumnCells = true;
			foreach (RangeCopyInfo rangeInfo in RangesInfo) {
				CellRange targetRange = rangeInfo.TargetBigRange;
				List<Table> targetTables = TargetWorksheet.Tables.GetItems(targetRange, true);
				Dictionary<string, string> tableColumnsNamesRenamed = new Dictionary<string, string>();
				int targetTablesCount = targetTables.Count;
				for (int i = 0; i < targetTablesCount; i++) {
					Table targetTable = targetTables[i];
					if (tableColumnNamesCorrectedOperation.Init(targetTables[i], targetRange, tableColumnsNamesRenamed)) {
						tableColumnNamesCorrectedOperation.Execute();
						walker.RegisterTableColumnsNamesRenamed(targetTable.Name, tableColumnsNamesRenamed);
					}
				}
			}
			if (walker.TableCount != 0)
				walker.Walk(TargetWorksheet.Workbook);
		}
		protected override void AfterEndTargetDocumentModelUpdate() {
		}
		public override CellPositionOffset GetTargetRangeOffset(CellRange currentTargetRange) {
			CellPositionOffset offset = SourceRange.GetCellPositionOffset(currentTargetRange.TopLeft);
			return offset;
		}
		#region SharedFormula
		protected override IDictionary<int, SharedFormula> GetInnerSharedFormulas(CellRange from, Predicate<CellRange> sharedFormulaRangeMatch) {
			Worksheet sheet = from.Worksheet as Worksheet;
			if (sheet.SharedFormulas.Count <= from.CellCount)
				return GetSharedFormulasFromCollection(sheet.SharedFormulas, sharedFormulaRangeMatch);
			else
				return GetSharedFormulasFromCells(from);
		}
		IDictionary<int, SharedFormula> GetSharedFormulasFromCollection(SharedFormulaCollection sharedFormulaCollection, Predicate<CellRange> match) {
			Dictionary<int, SharedFormula> result = new Dictionary<int, SharedFormula>();
			if (sharedFormulaCollection.Count == 0)
				return result;
			foreach (var pair in sharedFormulaCollection.InnerCollection) {
				SharedFormula sf = pair.Value;
				if (match(sf.Range))
					result.Add(pair.Key, pair.Value);
			}
			return result;
		}
		Dictionary<int, SharedFormula> GetSharedFormulasFromCells(CellRange sourceRange) {
			Dictionary<int, SharedFormula> result = new Dictionary<int, SharedFormula>();
			foreach (CellBase cellInfo in sourceRange.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (!cell.HasFormula)
					continue;
				SharedFormulaRef sourceSharedFormula = cell.GetFormula() as SharedFormulaRef;
				if (sourceSharedFormula == null)
					continue;
				if (!result.ContainsKey(sourceSharedFormula.HostFormulaIndex))
					result.Add(sourceSharedFormula.HostFormulaIndex, sourceSharedFormula.HostSharedFormula);
			}
			return result;
		}
		protected virtual SharedFormulaRangeCopyCalculator CreateCopySharedFormulaCalculator(ITargetRangeCalculatorOwner copyOperation) {
			return new SharedFormulaRangeCopyCalculator(copyOperation, this);
		}
		protected override void ProcessSourceSharedFormula(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange) {
			SharedFormulaRangeCopyCalculator calculator = CreateCopySharedFormulaCalculator(this as ITargetRangeCalculatorOwner);
			calculator.Execute(rangeInfo, sourceSharedFormula, existingSharedFormulaIndex, sourceSharedFormulaRange);
		}
		protected override bool CopyCellSharedFormulaCore(ICell source, FormulaBase formula, ICell target) {
			if (SourceRange.ContainsCell(target.Key)) {
				if (this.RangesInfo.First.IntersectionType != SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
					|| Object.ReferenceEquals(source, target))
					return true; 
			}
			SharedFormula sourceSharedFormula = (source.GetFormula() as SharedFormulaRef).HostSharedFormula;
			int targetSharedFormulaIndex = Int32.MinValue;
			if (!SourceSharedFormulaToTargetSharedFormulaIndexTranslationTable.TryGetValue(sourceSharedFormula, out targetSharedFormulaIndex))
				return false; 
			SharedFormula targetSharedFormula = target.Worksheet.SharedFormulas[targetSharedFormulaIndex];
			try {
				TargetDataContext.PushSetValueShouldAffectSharedFormula(false); 
				target.Worksheet.CreateSharedFormulaRefTransacted(target, targetSharedFormulaIndex, targetSharedFormula);
			}
			finally {
				TargetDataContext.PopSetValueShouldAffectSharedFormula();
			}
			return true;
		}
		#endregion
		#region ArrayFormula
		protected override void CopyCellArrayFormulaCore(CellRange sourceRange, FormulaBase sourceFormula, ICell target, CellRange currentTargetRange) {
		}
		protected override IEnumerable<CellRange> GetInnerSourceArrayFormulaRanges() {
			CellRange source = SourceRange;
			List<CellRange> formulasToCopy = new List<CellRange>();
			for (int i = 0; i < SourceWorksheet.ArrayFormulaRanges.Count; i++) {
				CellRange arrayRange = SourceWorksheet.ArrayFormulaRanges[i];
				if (source.Intersects(arrayRange))
					formulasToCopy.Add(arrayRange); 
			}
			return formulasToCopy;
		}
		protected virtual ArrayFormulaProcessorForRangeCopyOperation CreateArrayFormulaProcessor() {
			return new ArrayFormulaProcessorForRangeCopyOperation(this, this);
		}
		protected internal override void ProcessSourceArrayFormula(RangeCopyInfo rangeInfo, ArrayFormula sourceArrayFormula, CellRange sourceArrayFormulaRange) {
			ArrayFormulaProcessorForRangeCopyOperation op = CreateArrayFormulaProcessor();
			op.Execute(rangeInfo, sourceArrayFormula);
		}
		#endregion
		#region CopyRows
		protected internal override void CopyRows(RangeCopyInfo rangeCopyInfo) {
			bool shouldCopyRows =
				PasteSpecialOptions.ShouldCopyFormatAndStyle
				|| PasteSpecialOptions.ShouldCopyAllExceptBorders
				|| PasteSpecialOptions.ShouldCopyStyle
				|| PasteSpecialOptions.ShouldCopyOtherFormats
				|| PasteSpecialOptions.ShouldCopyBorders
				|| PasteSpecialOptions.ShouldCopyNumberFormat;
			if (!shouldCopyRows)
				return;
			bool shouldCopyPartial = !PasteSpecialOptions.ShouldCopyFormatAndStyle
				|| PasteSpecialOptions.ShouldCopyStyle
				|| PasteSpecialOptions.ShouldCopyOtherFormats
				|| PasteSpecialOptions.ShouldCopyBorders
				|| PasteSpecialOptions.ShouldCopyNumberFormat;
			if (!rangeCopyInfo.Info.SourceIsIntervalRange || !rangeCopyInfo.Info.TargetBigRangeIsIntervalRange)
				return;
			if ((rangeCopyInfo.SourceIsColumnIntervalRange || rangeCopyInfo.TargetBigIsColumnIntervalRange)
				&& !rangeCopyInfo.Info.SourceIsWholeWorksheetRange)
				return;
			foreach (CellRange currentTargetRange in rangeCopyInfo.TargetRanges) {
				bool reversed = false;
				RowsForCopyEnumerable rowsForCopy = new RowsForCopyEnumerable(
					rangeCopyInfo.GetSourceAsIntervalRange(),
					currentTargetRange, reversed);
				rowsForCopy.YieldAnyExistingSourceRow = true;
				IEnumerator<Row> enumerator = rowsForCopy.GetEnumerator();
				int defaultCellFormatIndex = TargetWorksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
				int defaultSourceCellFormatIndex = SourceWorksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
				while (enumerator.MoveNext()) {
					Row sourceRow = rowsForCopy.SourceObjectCurrent;
					Row targetRow = rowsForCopy.TargetObjectCurrent;
					if (sourceRow != null) {
						if (PasteSpecialOptions.ShouldCopyFormatAndStyle)
							CopyRowProperties(sourceRow, targetRow, defaultSourceCellFormatIndex);
						else if (shouldCopyPartial)
							CopyRowPartial(sourceRow, targetRow);
					}
					else {
						ClearTargetRow(targetRow, defaultCellFormatIndex);
					}
				}
			}
		}
		void CopyRowPartial(Row sourceRow, Row targetRow) {
			sourceRow.BeginUpdate();
			try {
				RowBatchUpdateHelper sourceRowBatchUpdateHelper = sourceRow.BatchUpdateHelper;
				CellFormat sourceFormat = (CellFormat)sourceRowBatchUpdateHelper.CellFormat;
				RowHeightInfo sourceHeightInfo = sourceRowBatchUpdateHelper.HeightInfo;
				RowInfo sourceRowInfo = sourceRowBatchUpdateHelper.Info;
				CopyRowCore(targetRow, sourceFormat, sourceHeightInfo, sourceRowInfo, PasteSpecialOptions);
			}
			finally {
				sourceRow.CancelUpdate();
			}
		}
		void CopyRowCore(Row targetRow, CellFormat sourceFormat, RowHeightInfo sourceHeightInfo, RowInfo sourceRowInfo, ModelPasteSpecialOptions PasteSpecialOptions) {
			targetRow.BeginUpdate();
			try {
				CellFormat targetFormat = targetRow.FormatInfo;
				CopyFormattingDeferred(targetFormat, sourceFormat);
				RowBatchUpdateHelper targetBatchUpdateHelper = targetRow.BatchUpdateHelper;
				bool isRowRange = SourceRange.IsRowRangeInterval() || SourceRange.IsWholeWorksheetRange();
				if (PasteSpecialOptions.ShouldCopyRowInfoAndHeightInfo && isRowRange) {
					targetBatchUpdateHelper.HeightInfo = sourceHeightInfo;
					targetBatchUpdateHelper.Info = sourceRowInfo;
				}
			}
			finally {
				targetRow.EndUpdate();
				ValidateTargetRowFormatIndex(targetRow);
			}
		}
		void ClearTargetRow(Row targetRow, int defaultCellFormatIndex) {
			CellFormat defaultCellFormat = WorkbookTarget.Cache.CellFormatCache[defaultCellFormatIndex] as CellFormat;
			RowHeightInfo defaultHeightInfo = new RowHeightInfo();
			RowInfo defaultRowInfo = new RowInfo();
			CopyRowCore(targetRow, defaultCellFormat, defaultHeightInfo, defaultRowInfo, PasteSpecialOptions);
		}
		#endregion
		#region Columns
		protected override void CopyColumns(RangeCopyInfo rangeCopyInfo) {
			CellIntervalRange sourceCellIntervalRange = rangeCopyInfo.GetSourceAsIntervalRange();
			CellIntervalRange targetCellIntervalRange = rangeCopyInfo.GetTargetBigRangeAsIntervalRange();
			bool rangeIsRowInterval = (sourceCellIntervalRange != null && sourceCellIntervalRange.IsRowInterval)
				|| (targetCellIntervalRange != null && targetCellIntervalRange.IsRowInterval);
			bool isRowIntervalAndShouldCopyColumnWidths = rangeIsRowInterval && PasteSpecialOptions.ShouldCopyColumnWidths;
			bool isColumnInterval = (sourceCellIntervalRange != null && sourceCellIntervalRange.IsColumnInterval)
					&& (targetCellIntervalRange != null && targetCellIntervalRange.IsColumnInterval);
			if (PasteSpecialOptions.ShouldCopyColumnWidths || isRowIntervalAndShouldCopyColumnWidths || isColumnInterval) {
				CopyColumnsOneByOne(rangeCopyInfo);
				CellRange targetBigRange = rangeCopyInfo.TargetBigRange;
				TargetWorksheet.Columns.MergeSameColumns(targetBigRange.TopLeft.Column, targetBigRange.BottomRight.Column);
			}
		}
		void CopyColumnsOneByOne(RangeCopyInfo rangeCopyInfo) {
			int sourceDefaultCellFormatIndex = WorkbookSource.StyleSheet.DefaultCellFormatIndex;
			int startSourceColumnIndex = SourceRange.TopLeft.Column;
			for (int offset = 0; offset < SourceRange.Width; offset++) {
				Column sourceColumn = SourceWorksheet.Columns.GetColumnRangeForReading(startSourceColumnIndex + offset);
				bool sourceColumnHasSomething = sourceColumn.FormatIndex != sourceDefaultCellFormatIndex
						|| sourceColumn.WidthIndex != 0 || sourceColumn.InfoIndex != 0;
				foreach (CellRange currentRange in rangeCopyInfo.TargetRanges) {
					int startTargetColumnIndex = currentRange.TopLeft.Column;
					int targetColumnIndex = startTargetColumnIndex + offset;
					Column targetColumn = TargetWorksheet.Columns.TryGetColumn(targetColumnIndex);
					if (targetColumn == null && !sourceColumnHasSomething)
						continue;
					targetColumn = TargetWorksheet.Columns.GetIsolatedColumn(targetColumnIndex); 
					CopyColumnProperties(sourceColumn, targetColumn);
				}
			}
		}
		protected override IEnumerable<Column> GetSourceColumns() {
			throw new InvalidOperationException();
		}
		protected override Column CreateTargetColumn(Column sourceColumn) {
			throw new InvalidOperationException();
		}
		#endregion
		protected override IEnumerable<CellRange> GetMergedCellRanges() {
			if (PasteSpecialOptions.InnerFlags == ModelPasteSpecialFlags.ColumnWidths)
				return new List<CellRange>();
			return GetMergedCellRangesWithoutIntersectionWithRangeBorders(SourceRange);
		}
		protected virtual MergedRangeProcessorForRangeCopyOperation CreateMergedRangeProcessor() {
			return new MergedRangeProcessorForRangeCopyOperation(this, this);
		}
		protected override void ProcessSourceMergedRange(RangeCopyInfo rangeInfo, CellRange sourceMergedRange) {
			MergedRangeProcessorForRangeCopyOperation op = CreateMergedRangeProcessor();
			op.Execute(rangeInfo, sourceMergedRange);
		}
		protected override IEnumerable<Comment> GetCommentsForCopy() {
			List<Comment> result = new List<Comment>();
			foreach (Comment item in SourceWorksheet.Comments.InnerList) {
				if (SourceRange.ContainsCell(item.Reference.Column, item.Reference.Row)) {
					result.Add(item);
				}
			}
			return result;
		}
		protected override void ProcessSourceComment(RangeCopyInfo rangeInfo, Comment sourceComment, CellRange sourceCommentRange) {
			SourceCommentProcessorForRangeCopyOperation copier = new SourceCommentProcessorForRangeCopyOperation(this, this);
			copier.Execute(rangeInfo, sourceComment, sourceCommentRange);
		}
		protected override List<ModelHyperlink> GetSourceHyperlinks() {
			var result = SourceWorksheet.Hyperlinks.InnerList.Where<ModelHyperlink>
				( hlink => SourceRange.Intersects(hlink.Range)).ToList();
			return result;
		}
		protected virtual SourceHyperlinkProcessorForRangeCopyOperation CreateSourceHyperlinkRangeProcessor() {
			return new SourceHyperlinkProcessorForRangeCopyOperation(this, this);
			}
		protected override void ProcessSourceHyperlink(RangeCopyInfo rangeInfo, ModelHyperlink sourceHyperlinkItem) {
			SourceHyperlinkProcessorForRangeCopyOperation op = CreateSourceHyperlinkRangeProcessor();
			op.Execute(rangeInfo, sourceHyperlinkItem);
		}
		CellRangeBase GetSourceObjectRangeCroppedBySourceRange(CellRangeBase sourceObjectRange, CellRange sourceRange) {
			VariantValue sourceObjectRangeIntersectedWithSourceRangeVariantValue = sourceObjectRange.IntersectionWith(sourceRange);
			CellRangeBase sourceObjectRangeCropped = sourceObjectRangeIntersectedWithSourceRangeVariantValue.CellRangeValue;
			return sourceObjectRangeCropped;
		}
		public override CellRangeBase GetTargetObjectRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange) {
			CellRangeBase sourceObjectRangeCropped = sourceObjectRange;
			sourceObjectRangeCropped = GetSourceObjectRangeCroppedBySourceRange(sourceObjectRange, SourceRange);
			CellPositionOffset offsetSourceTargetRanges = GetTargetRangeOffset(currentTargetRange);
			CellRangeBase targetObjectRangeShifted = sourceObjectRangeCropped.GetShiftedAny(offsetSourceTargetRanges, TargetWorksheet);
			VariantValue targetObjectRangeShiftedIntersectionWith = currentTargetRange.IntersectionWith(targetObjectRangeShifted);
			CellRangeBase targetObjectRangeShiftedCropped = targetObjectRangeShiftedIntersectionWith.CellRangeValue;
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(currentTargetRange.Worksheet, targetObjectRangeShiftedCropped.Worksheet));
			return targetObjectRangeShiftedCropped;
		}
		protected override bool IsSourceObjectRangeInterserctsWithCurrentTargetRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange) {
			if (!Object.ReferenceEquals(currentTargetRange.Worksheet, sourceObjectRange.Worksheet))
				return false;
			return currentTargetRange.Intersects(sourceObjectRange);
		}
		#region Tables
		protected internal override IEnumerable<Table> GetSourceTables(bool includeTablesIntersectedSourceRange) {
			List<Table> result = SourceWorksheet.Tables.GetItems(SourceRange, includeTablesIntersectedSourceRange);
			return result;
		}
		protected internal override List<Table> GetSourceTablesFromIntersectionSourceTargetRange(CellRange intersection) {
			List<Table> result = SourceWorksheet.Tables.GetItems(intersection, false);
			return result;
		}
		protected override void ProcessSourceTable(RangeCopyInfo rangeCopyInfo, Table sourceTable, List<string> existingTableNames) {
			TableProcessorForRangeCopyOperation calculator = CreateTableRangeCalculator();
			calculator.Calculate(rangeCopyInfo, sourceTable, existingTableNames);
		}
		#endregion
		protected internal override IEnumerable<PivotTable> GetSourcePivotTables(CellRange _sourceRange, bool includeTablesIntersectedSourceRange) {
			Predicate<CellRange> sourceRangeContainsObjectRange = objectRange => _sourceRange.ContainsRange(objectRange);
			Func<PivotTable, bool> pivotInsideSourceRange = (pivot) => pivot.WholeRange.ForAll(sourceRangeContainsObjectRange);
			List<PivotTable> result = SourceWorksheet.PivotTables.GetItems(_sourceRange, includeTablesIntersectedSourceRange);
			return result.Where(pivotInsideSourceRange);
		}
		protected override void ProcessSourcePivotTable(RangeCopyInfo rangeCopyInfo, PivotTable sourcePivotTable, IList<string> existingTableNames) {
			PivotTableProcessorForRangeCopyOperation calculator = CreatePivotTableRangeCalculator();
			calculator.Calculate(rangeCopyInfo, sourcePivotTable, existingTableNames);
		}
		public override IPivotCacheSource CopyPivotCacheSource(IPivotCacheSource sourcePivotCacheSource) {
			if (IsEqualWorkbook)
				return sourcePivotCacheSource; 
			return base.CopyPivotCacheSource(sourcePivotCacheSource);
		}
		#region Drawings
		protected override IEnumerable<IDrawingObject> GetDrawingsForCopy() {
			List<IDrawingObject> sourceDrawings = SourceWorksheet.DrawingObjectsByZOrderCollections.GetDrawingObjects(SourceRange);
			List<IDrawingObject> result = new List<IDrawingObject>();
			foreach (IDrawingObject item in sourceDrawings)
				if (ShouldCopyDrawing(item, SourceRange))
					result.Add(item);
			return result;
		}
		protected internal bool ShouldCopyDrawing(IDrawingObject drawing, CellRange sourceRange) {
			if(drawing.AnchorType == AnchorType.Absolute)
				return false;
			List<CellRange> minRanges = GetMinRangesForCopyPicture(drawing.Worksheet, drawing.From, drawing.To);
			foreach(CellRange range in minRanges) {
				if(sourceRange.Includes(range))
					return true;
			}
			return false;
		}
		internal static List<CellRange> GetMinRangesForCopyPicture(Worksheet sourceWorksheet, AnchorPoint from, AnchorPoint to) {
			int bottomRow = Math.Max(from.Row, to.RowOffset == 0 ? to.Row - 2 : to.Row - 1);
			int rightColumn = Math.Max(from.Col, to.ColOffset == 0 ? to.Col - 2 : to.Col - 1);
			int leftColumn = Math.Min(rightColumn, from.Col + 1);
			int topRow = Math.Min(bottomRow, from.Row + 1);
			if(((to.Row - from.Row == 2 && to.RowOffset == 0) || ((to.Row - from.Row == 1 && to.RowOffset != 0))) && ((to.Col - from.Col == 2 && to.ColOffset == 0) || (to.Col - from.Col == 1 && to.ColOffset != 0))) {
				return new List<CellRange> {
					new CellRange(sourceWorksheet, new CellPosition(from.Col, from.Row), new CellPosition(from.Col, from.Row)),
					new CellRange(sourceWorksheet, new CellPosition(from.Col, from.Row + 1), new CellPosition(from.Col, from.Row + 1)),
					new CellRange(sourceWorksheet, new CellPosition(from.Col + 1, from.Row), new CellPosition(from.Col + 1, from.Row)),
					new CellRange(sourceWorksheet, new CellPosition(from.Col + 1, from.Row + 1), new CellPosition(from.Col + 1, from.Row + 1)),
				};
			}
			if((to.Row - from.Row == 2 && to.RowOffset == 0) || (to.Row - from.Row == 1 && to.RowOffset != 0)) {
				leftColumn = to.Col - from.Col <= 1 ? from.Col : from.Col + 1;
				rightColumn = to.Col - from.Col <= 1 ? from.Col : to.ColOffset == 0 ? to.Col - 2 : to.Col - 1;
				return new List<CellRange> {
					new CellRange(sourceWorksheet, new CellPosition(leftColumn, from.Row), new CellPosition(rightColumn, from.Row)),
					new CellRange(sourceWorksheet, new CellPosition(leftColumn, from.Row + 1), new CellPosition(rightColumn, from.Row + 1))
				};
			}
			if((to.Col - from.Col == 2 && to.ColOffset == 0) || (to.Col - from.Col == 1 && to.ColOffset != 0)) {
				topRow = to.Row - from.Row <= 1 ? from.Row : from.Row + 1;
				bottomRow = to.Row - from.Row <= 1 ? from.Row : to.RowOffset == 0 ? to.Row - 2 : to.Row - 1;
				return new List<CellRange> {
					new CellRange(sourceWorksheet, new CellPosition(from.Col, topRow), new CellPosition(from.Col, bottomRow)),
					new CellRange(sourceWorksheet, new CellPosition(from.Col + 1, topRow), new CellPosition(from.Col + 1, bottomRow))
				};
			}
			CellPosition topLeft = new CellPosition(leftColumn, topRow);
			CellPosition bottomRight = new CellPosition(rightColumn, bottomRow);
			CellRange minRangeToCopyPicture = new CellRange(sourceWorksheet, topLeft, bottomRight);
			return new List<CellRange> {minRangeToCopyPicture};
		}
		protected virtual PictureProcessorForRangeCopyOperation CreateSourcePictureProcessor() {
			return new PictureProcessorForRangeCopyOperation(this, this);
		}
		protected virtual ChartProcessorForRangeCopyOperation CreateSourceChartProcessor() {
			return new ChartProcessorForRangeCopyOperation(this, this);
		}
		protected override void ProcessSourcePicture(RangeCopyInfo rangeInfo, Picture sourcePicture) {
			PictureProcessorForRangeCopyOperation op = CreateSourcePictureProcessor();
			op.Execute(rangeInfo, sourcePicture);
		}
		protected override void ProcessChart(RangeCopyInfo rangeInfo, Chart sourceChart) {
			ChartProcessorForRangeCopyOperation op = CreateSourceChartProcessor();
			op.Execute(rangeInfo, sourceChart);
		}
		#endregion
		#region ConditionalFormatting
		protected override IEnumerable<ConditionalFormatting> GetConditionalFormattingsForCopy(Worksheet worksheet) {
			return worksheet.ConditionalFormattings.Select(SourceRange, false);
		}
		protected virtual ConditionalFormattingProcessorForRangeCopyOperation CreateConditionalFormattingRangeCalculator() {
			return new ConditionalFormattingProcessorForRangeCopyOperation(this as ITargetRangeCalculatorOwner, this);
		}
		protected override void ProcessSourceConditionalFormatting(RangeCopyInfo rangeInfo, ConditionalFormatting sourceConditionalFormatting) {
			ConditionalFormattingProcessorForRangeCopyOperation calc = CreateConditionalFormattingRangeCalculator();
			calc.Execute(rangeInfo, sourceConditionalFormatting);
		}
		#endregion
		protected override IEnumerable<ModelProtectedRange> GetSourceProtectedRanges() {
			return SourceWorksheet.ProtectedRanges.FindAll( item => SourceRange.Includes(item.CellRange));
		}
		protected override void ProcessSourceProtectedRange(RangeCopyInfo rangeInfo, ModelProtectedRange sourceProtectedRange) {
			CellRangeBase sourceObjectRange = sourceProtectedRange.CellRange;
			CellRange sourceCellRange = sourceObjectRange as CellRange;
			CellUnion sourceCellUnion = sourceObjectRange as CellUnion;
			bool originalWasCellUnion = sourceCellUnion != null;
			CellRangeBase newCfRange = null;
			if (!originalWasCellUnion) 
				newCfRange = CellUnion.Combine(sourceCellRange, rangeInfo.TargetBigRange);
			sourceProtectedRange.CellRange = newCfRange;
		}
		protected override void ProcessSourceSparklineGroup(RangeCopyInfo rangeInfo, SparklineGroup sourceSparklineGroup) {
		}
		protected override IEnumerable<SparklineGroup> GetSourceSparklineGroups() {
			return new List<SparklineGroup>(); 
		}
		protected internal override void ApplyChanges() {
		}
		public override CellRangeBase GetRangeToClearAfterCut() {
			CellRangeBase result = SourceRange.Clone();
			foreach (RangeCopyInfo item in this.RangesInfo) {
				result = result.ExcludeRange(item.TargetBigRange);
			}
			return result;
		}
	}
}
