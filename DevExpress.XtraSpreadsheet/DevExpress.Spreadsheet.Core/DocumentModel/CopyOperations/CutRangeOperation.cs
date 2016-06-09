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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class CutRangeOperation : RangeCopyOperation {
		public CutRangeOperation(CellRange sourceRange, CellRange target)
			: this(new SourceTargetRangesForCopy(sourceRange, target)) {
		}
		public CutRangeOperation(SourceTargetRangesForCopy ranges)
			: base(ranges, Model.ModelPasteSpecialFlags.All) { 
		}
		public override bool CutMode { [System.Diagnostics.DebuggerStepThrough]get { return true; } }
		public override PrepareTargetParsedExpressionVisitor CreateFormulaWalker(Worksheet targetWorksheet, CellRange sourceRange, ICopyEverythingArguments args) {
			return new PrepareTargetParsedExpressionVisitorCutMode(args.TargetDataContext, targetWorksheet, args);
		}
		public override IModelErrorInfo Checks() {
			if (SuppressChecks)
				return null;
			bool targetRangeIsCellUnion = RangesInfo.TargetRangeIsCellUnion;
			if (targetRangeIsCellUnion)
				return new ModelErrorInfo(ModelErrorType.ErrorCommandCannotPerformedWithMultipleSelections);
			foreach (RangeCopyInfo item in RangesInfo) {
				if (item.ErrorType != ModelErrorType.None)
					return new ModelErrorInfo(item.ErrorType);
			}
			IModelErrorInfo error = null;
			CellRange sourceRangeToClear = SourceRange;
			RangeCopyInfo rangeInfo = RangesInfo.First;
			if (rangeInfo.SourceAndTargetEquals) {
				error = CheckTargetRangeNotIntersectsMergedRanges(rangeInfo, rangeInfo.TargetBigRange);
				return error;
			}
			error = CheckSourceRangeContainsPartOfArrayFormula(rangeInfo, sourceRangeToClear);
			if (error != null)
				return error;
			error = CheckSourceRangeNotIntersectsMergedRanges(rangeInfo, sourceRangeToClear);
			if (error != null)
				return error;
			error = CheckSourceRangeNotInsideOrIntersectsPivotTables(rangeInfo, sourceRangeToClear);
			if (error != null)
				return error;
			return base.Checks();
		}
		protected internal override void ExecuteCore() {
			RangeCopyInfo rangeInfo = RangesInfo.First;
			CellRange targetRange = rangeInfo.TargetBigRange;
			OnBeforeRangeMoved(rangeInfo);
			base.ExecuteCore();
			OnAfterRangeMoved(SourceRange, targetRange);
		}
		protected override ConditionalFormattingProcessorForRangeCopyOperation CreateConditionalFormattingRangeCalculator() {
			return new ConditionalFormattingProcessorForRangeCutOperation(this as ITargetRangeCalculatorOwner, this);
		}
		public override void UseSourceConditionalFormatting(ConditionalFormatting sourceConditionalFormatting, CellRangeBase newRange, CellRange targetRange) {
			ProcessFormulaConditionalFormatting(sourceConditionalFormatting, sourceConditionalFormatting, targetRange);
			base.UseSourceConditionalFormatting(sourceConditionalFormatting, newRange, targetRange);
		}
		protected override SharedFormulaRangeCopyCalculator CreateCopySharedFormulaCalculator(ITargetRangeCalculatorOwner copyOperation) {
			return new SharedFormulaRangeCutCalculator(copyOperation, this);
		}
		IModelErrorInfo CheckSourceRangeContainsPartOfArrayFormula(RangeCopyInfo rangeInfo, CellRange sourceRangeToClear) {
			ArrayFormulaRangesCollection targetArrays = TargetWorksheet.ArrayFormulaRanges;
			bool collectionNotEmpty = targetArrays.Count > 0;
			Predicate<CellRange> sourceRangeIncludesOrIntersectsArrayRange =
				array => {
					bool intersectsSourceRange = rangeInfo.ObjectRangeIntersectsRangeBorder(sourceRangeToClear, array); 
					return intersectsSourceRange;
				};
			if (collectionNotEmpty && targetArrays.Exists(sourceRangeIncludesOrIntersectsArrayRange))
				return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			return null;
		}
		protected override IModelErrorInfo CheckTargetRangeContainsPartOfArrayFormula(RangeCopyInfo rangeInfo, CellRange targetRange) {
			ArrayFormulaRangesCollection targetArrays = TargetWorksheet.ArrayFormulaRanges;
			bool collectionNotEmpty = targetArrays.Count > 0;
			Predicate<CellRange> targetRangeIncludesOrIntersectsArrayRange =
				array => {
					bool intersectsTargetRange = rangeInfo.ObjectRangeIntersectsRangeBorder(targetRange, array);
					bool sourceTargetOffsetCase = rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.None;
					bool arrayRangeInsideSourceRangeWhenSRIntersectsTR = sourceTargetOffsetCase
						&& intersectsTargetRange && SourceRange.ContainsRange(array);
					if (arrayRangeInsideSourceRangeWhenSRIntersectsTR)
						return false;
					return intersectsTargetRange;
				};
			if (collectionNotEmpty && targetArrays.Exists(targetRangeIncludesOrIntersectsArrayRange))
				return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			return null;
		}
		protected override ArrayFormulaProcessorForRangeCopyOperation CreateArrayFormulaProcessor() {
			return new ArrayFormulaProcessorForRangeCutOperation(this, this);
		}
		protected override IModelErrorInfo CheckMergedRangesIntersectedTargetRange(RangeCopyInfo rangeInfo, CellRange range, List<CellRange> intersectedMergedCellRanges) {
			bool sourceAndTargetRangesAreIntersectedWithOffsetOrEquals = rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals;
			bool collectionNotEmpty = intersectedMergedCellRanges.Count > 0;
			bool shouldWarnUserAboutUnmergeMergedRangeInSourceRange = collectionNotEmpty && intersectedMergedCellRanges.Exists((mergedRange) => {
				if (sourceAndTargetRangesAreIntersectedWithOffsetOrEquals && SourceRange.Includes(mergedRange))
					return true;
				return false;
			});
			if (shouldWarnUserAboutUnmergeMergedRangeInSourceRange)
				return new ClarificationErrorInfo(ModelErrorType.UnmergeMergedCellsClarification);
			return base.CheckMergedRangesIntersectedTargetRange(rangeInfo, range, intersectedMergedCellRanges);
		}
		IModelErrorInfo CheckSourceRangeNotIntersectsMergedRanges(RangeCopyInfo rangeCopyInfo, CellRange sourceRangeToClear) {
			List<CellRange> intersectedMergedCellRanges = SourceWorksheet.MergedCells.GetMergedCellRangesIntersectsRange(sourceRangeToClear);
			bool collectionNotEmpty = intersectedMergedCellRanges.Count > 0;
			Predicate<CellRange> intersectsSourceRangeBorders =
				mergedRange => {
					return rangeCopyInfo.ObjectRangeIntersectsRangeBorder(sourceRangeToClear, mergedRange);
				};
			if (collectionNotEmpty && intersectedMergedCellRanges.Exists(intersectsSourceRangeBorders))
				return new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
			return null;
		}
		protected override MergedRangeProcessorForRangeCopyOperation CreateMergedRangeProcessor() {
			return new MergedRangeProcessorForRangeCutOperation(this, this);
		}
		protected override TableProcessorForRangeCopyOperation CreateTableRangeCalculator() {
			return new TableProcessorForRangeCutOperation(this, this);
		}
		IModelErrorInfo CheckSourceRangeNotInsideOrIntersectsPivotTables(RangeCopyInfo rangeInfo, CellRange sourceRangeToClear) {
			Worksheet sheet = sourceRangeToClear.Worksheet as Worksheet;
			bool orIntersects = true;
			List<PivotTable> findedPivots = sheet.PivotTables.GetItems(sourceRangeToClear, orIntersects);
			bool collectionNotEmpty = findedPivots.Count > 0;
			Predicate<PivotTable> condition =
				pivot => {
					return RangeIntersectsPivot(rangeInfo, sourceRangeToClear, pivot.WholeRange);
				};
			if (collectionNotEmpty && findedPivots.Exists(condition))
				return new ModelErrorInfo(ModelErrorType.PivotTableCanNotBeChanged);
			return null;
		}
		protected override PivotTableProcessorForRangeCopyOperation CreatePivotTableRangeCalculator() {
			return new PivotTableProcessorForRangeCutOperation(this, this);
		}
		protected override void RemoveHyperlinks(RangeCopyInfo rangeInfo) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			if (!rangeInfo.SourceAndTargetEquals) { 
				Predicate<CellRange> fromTargetRangeButNotIntersectsBordersAndNotIntersectsSourceRange = (hlinkRange) => {
					return targetBigRange.Includes(hlinkRange) && !hlinkRange.Intersects(SourceRange);
				};
				RemoveHyperlinksFromRange(targetBigRange, fromTargetRangeButNotIntersectsBordersAndNotIntersectsSourceRange);
			}
		}
		protected override SourceHyperlinkProcessorForRangeCopyOperation CreateSourceHyperlinkRangeProcessor() {
			return new SourceHyperlinkProcessorForRangeCutOperation(this, this);
		}
		public void OnBeforeRangeMoved(RangeCopyInfo rangeInfo) {
			if (rangeInfo.SourceAndTargetEquals)
				return;
			CellRangeBase targetRangeWithoutSource = rangeInfo.TargetBigRange;
			if (rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.None) {
				targetRangeWithoutSource = rangeInfo.TargetBigRange.ExcludeRange(rangeInfo.SourceRange);
				if (targetRangeWithoutSource == null) {
					Debug.Assert(false,"strange case", "");
					return;
				}
			}
			RemoveRangeCommand command = CreateRemoveRangeCommand(targetRangeWithoutSource);
			command.Execute();
		}
		protected virtual RemoveRangeCommand CreateRemoveRangeCommand(CellRangeBase range) {
			RemoveRangeCommand command = new RemoveRangeCommand(TargetWorksheet, range, RemoveCellMode.NoShiftOrRangeToPasteCutRange, false, false, ErrorHandler);
			command.SuppressDataValidationSplit = !IsEqualWorksheets;
			return command;
		}
		public void OnAfterRangeMoved(CellRange sourceRange, CellRange targetRange) {
			CellPositionOffset offset = GetTargetRangeOffset(targetRange);
			ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker walker =
				new ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker(offset, TargetWorksheet.Workbook.DataContext, sourceRange, targetRange);
			List<Table> targetTables = TargetWorksheet.Tables.GetItems(targetRange, true);
			int targetTablesCount = targetTables.Count;
			for (int i = 0; i < targetTablesCount; i++) {
				Dictionary<string, string> tableColumnsNamesRenamed = new Dictionary<string, string>();
				Table targetTable = targetTables[i];
				TableColumnNamesChangeOperation operation = new TableColumnNamesChangeOperation();
				operation.ShouldCorrectColumnFormulas = false;
				operation.ShouldUpdateColumnCells = true;
				if (operation.Init(targetTable, targetRange, tableColumnsNamesRenamed)) {
					operation.Execute();
					walker.RegisterTableColumnsNamesRenamed(targetTable.Name, tableColumnsNamesRenamed);
				}
			}
			walker.Walk(TargetWorksheet.Workbook);
			MoveCellTagsAfterCut(sourceRange, targetRange);
		}
		protected override PictureProcessorForRangeCopyOperation CreateSourcePictureProcessor() {
			return new PictureProcessorForRangeCutOperation(this, this);
			}
		protected override ChartProcessorForRangeCopyOperation CreateSourceChartProcessor() {
			return new ChartProcessorForRangeCutOperation(this, this);
		}
		void MoveCellTagsAfterCut(CellRange sourceRange, CellRange targetRange) {
			CellPositionOffset offset = GetTargetRangeOffset(targetRange);
			if (this.IsEqualWorksheets) {
				this.Worksheet.CellTags.ShiftTagsInRange(sourceRange, offset);
				return;
			}
			foreach (var keyValuePair in this.SourceWorksheet.CellTags.GetExistingCellTagsFrom(sourceRange)) {
				CellPosition sourceTagPosition = keyValuePair.Key;
				CellPosition targetTagPosition = sourceTagPosition.GetShiftedAny(offset, TargetWorksheet);
				TargetWorksheet.CellTags.SetValue(targetTagPosition, keyValuePair.Value);
			}
		}
		protected override void CopyAutoFilter(RangeCopyInfo rangeInfo) {
			if (IsEqualWorksheets)
				CopyAutoFilterSameWorksheets(rangeInfo);
			else
				CopyAutoFilterDifferentWorksheets(rangeInfo);
		}
		void CopyAutoFilterSameWorksheets(RangeCopyInfo rangeInfo) {
			CellRange targetRange = rangeInfo.TargetBigRange;
			SheetAutoFilter autoFilter = SourceWorksheet.AutoFilter;
			if (!autoFilter.Enabled || SourceRange.Equals(targetRange))
				return;
			CellRange filterRange = autoFilter.Range;
			if (SourceRange.ContainsRange(filterRange)) {
				SortState sourceSortState = autoFilter.SortState;
				if (!filterRange.Equals(SourceRange)) {
					CellRange newRange = GetTargetObjectRange(filterRange, targetRange) as CellRange;
					autoFilter.Disable();
					autoFilter.Range = newRange;
				}
				CopySortState(sourceSortState, autoFilter.SortState, targetRange);
			}
			else if (targetRange.ContainsRange(filterRange))
				autoFilter.Disable();
		}
		void CopyAutoFilterDifferentWorksheets(RangeCopyInfo rangeinfo) {
			CellRange currentTargetRange = rangeinfo.TargetBigRange;
			SheetAutoFilter sourceAutoFilter = SourceWorksheet.AutoFilter;
			SheetAutoFilter targetAutoFilter = TargetWorksheet.AutoFilter;
			if (!sourceAutoFilter.Enabled && !targetAutoFilter.Enabled)
				return;
			if (sourceAutoFilter.Enabled && SourceRange.ContainsRange(sourceAutoFilter.Range))
				sourceAutoFilter.Disable();
			else if (targetAutoFilter.Enabled && currentTargetRange.ContainsRange(targetAutoFilter.Range))
				targetAutoFilter.Disable();
		}
		protected override void CopyDataValidationsCore(DataValidation sourceValidaiton, RangeCopyInfo rangeInfo) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			VariantValue sourceValidationRange = sourceValidaiton.CellRange.IntersectionWith(SourceRange);
			Debug.Assert(sourceValidationRange.IsCellRange);
			CellRangeBase targetRange = GetTargetObjectRange(sourceValidationRange.CellRangeValue, targetBigRange);
			DataValidation targetValidation = sourceValidaiton.CloneTo(targetRange);
			CellPositionOffset offset = new CellPositionOffset(sourceValidationRange.CellRangeValue.TopLeft, targetValidation.CellRange.TopLeft);
			sourceValidaiton.ProcessCopiedRange(null, targetBigRange);
			ProcessTargetDataValidation(targetValidation, offset);
		}
	}
}
