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

using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Office;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public abstract class CopyEverythingBase : SpreadsheetModelCommand,
		ICopyEverythingArguments, ITargetRangeCalculatorOwner, IRangeCopyMergedRangePart, IRangeCopyArrayFormulaPart,
		IRangeCopySharedFormulaPart, IRangeCopyConditionalFormattingPart, IRangeCopyTablePart, IRangeCopyHyperlinkPart,
	IRangeCopyCommentPart, IRangeCopyPicturePart, IRangeCopyChartPart, IRangeCopyPivotTablePart {
		ModelPasteSpecialOptions pasteSpecialOptions;
		bool disabledHistory;
		Dictionary<SharedFormula, int> sourceSharedFormulaToTargetSharedFormulaIndexTranslationTable;
		Dictionary<string, int> styleTranslationTable;
		List<ArrayFormula> arrayFormulasForTargetWorksheetList;
		HashSet<string> targetStyleNames;
		bool copyCellStyles;
		bool suppressChecks;
		bool suppressCheckDefinedNames;
		bool shouldConvertSourceStringTextValueToFormattedValue;
		string sourceWorkbookFilePath;
		SheetDefToOuterAreasReplaceMode referencesWithSheetDefinitionToOuterAreasReplaceMode;
		SourceSheetDefinitionToTargetCache sourceSheetDefinitionToTargetCache; 
		PrepareTargetParsedExpressionVisitor walker = null;
		SourceTargetRangesForCopy rangesInfo;
		IErrorHandler errorHandler = null;
		bool removeHyperlinksIntersectedClearedRange = true;
		bool suppressCellValueChanged;
		readonly int defaultTargetCellFormatIndex;
		protected CopyEverythingBase(SourceTargetRangesForCopy rangeInfos, ModelPasteSpecialFlags pasteSpecialOptions)
			: base(rangeInfos.TargetWorksheet) {
			this.rangesInfo = rangeInfos;
			this.pasteSpecialOptions = new ModelPasteSpecialOptions(pasteSpecialOptions);
			this.sourceSharedFormulaToTargetSharedFormulaIndexTranslationTable = new Dictionary<SharedFormula, int>();
			this.styleTranslationTable = new Dictionary<string, int>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.arrayFormulasForTargetWorksheetList = new List<ArrayFormula>();
			this.copyCellStyles = true;
			this.suppressChecks = false;
			this.suppressCheckDefinedNames = false;
			this.shouldConvertSourceStringTextValueToFormattedValue = false;
			this.sourceWorkbookFilePath = String.Empty;
			this.sourceSheetDefinitionToTargetCache = new SourceSheetDefinitionToTargetCache();
			this.errorHandler = new NonSettedErrorHandler();
			this.walker = CreateFormulaWalker(Worksheet, rangesInfo.SourceRange, this);
			this.defaultTargetCellFormatIndex = rangesInfo.TargetWorksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
		}
		#region Properties
		public DocumentModel WorkbookSource {[System.Diagnostics.DebuggerStepThrough] get { return rangesInfo.SourceWorksheet.Workbook; } }
		public DocumentModel WorkbookTarget {[System.Diagnostics.DebuggerStepThrough] get { return Worksheet.Workbook; } }
		public CellRange SourceRange {[System.Diagnostics.DebuggerStepThrough] get { return rangesInfo.SourceRange; } }
		protected Worksheet SourceWorksheet {[System.Diagnostics.DebuggerStepThrough] get { return rangesInfo.SourceWorksheet; } }
		protected Worksheet TargetWorksheet {[System.Diagnostics.DebuggerStepThrough] get { return Worksheet; } }
		public WorkbookDataContext SourceDataContext {[System.Diagnostics.DebuggerStepThrough] get { return WorkbookSource.DataContext; } }
		public WorkbookDataContext TargetDataContext {[System.Diagnostics.DebuggerStepThrough] get { return WorkbookTarget.DataContext; } }
		DevExpress.XtraSpreadsheet.Model.External.ExternalLinkCollection ICopyEverythingArguments.SourceExternalLinks { get { return WorkbookSource.ExternalLinks; } }
		DevExpress.XtraSpreadsheet.Model.External.ExternalLinkCollection ICopyEverythingArguments.TargetExternalLinks { get { return WorkbookTarget.ExternalLinks; } }
		protected Dictionary<SharedFormula, int> SourceSharedFormulaToTargetSharedFormulaIndexTranslationTable {[System.Diagnostics.DebuggerStepThrough]get { return this.sourceSharedFormulaToTargetSharedFormulaIndexTranslationTable; } }
		public bool IsEqualWorkbook {[System.Diagnostics.DebuggerStepThrough]  get { return Object.ReferenceEquals(WorkbookTarget, WorkbookSource); } }
		public bool IsEqualWorksheets {[System.Diagnostics.DebuggerStepThrough] get { return Object.ReferenceEquals(TargetWorksheet, SourceWorksheet); } }
		public bool DisabledHistory {[System.Diagnostics.DebuggerStepThrough]get { return disabledHistory; } set { disabledHistory = value; } }
		public bool CopyCellStyles {[System.Diagnostics.DebuggerStepThrough]get { return copyCellStyles; } set { copyCellStyles = value; } }
		public ModelPasteSpecialOptions PasteSpecialOptions {[System.Diagnostics.DebuggerStepThrough]get { return this.pasteSpecialOptions; } }
		public virtual bool CutMode {[System.Diagnostics.DebuggerStepThrough]get { return false; } }
		public bool SuppressChecks {[System.Diagnostics.DebuggerStepThrough]get { return suppressChecks; } set { suppressChecks = value; } }
		public bool SuppressCheckDefinedNames {[System.Diagnostics.DebuggerStepThrough]get { return suppressCheckDefinedNames; } set { suppressCheckDefinedNames = value; } }
		public bool ShouldConvertSourceStringTextValueToFormattedValue {[System.Diagnostics.DebuggerStepThrough]get { return shouldConvertSourceStringTextValueToFormattedValue; } set { shouldConvertSourceStringTextValueToFormattedValue = value; } }
		public SheetDefToOuterAreasReplaceMode SheetDefinitionToOuterAreasReplaceMode {[System.Diagnostics.DebuggerStepThrough]get { return referencesWithSheetDefinitionToOuterAreasReplaceMode; } set { referencesWithSheetDefinitionToOuterAreasReplaceMode = value; } }
		public string SourceWorkbookFilePath {[System.Diagnostics.DebuggerStepThrough]get { return sourceWorkbookFilePath; } set { sourceWorkbookFilePath = value; } }
		public SourceSheetDefinitionToTargetCache SourceSheetDefinitionToTargetCache {[System.Diagnostics.DebuggerStepThrough]get { return sourceSheetDefinitionToTargetCache; } set { sourceSheetDefinitionToTargetCache = value; } }
		public virtual bool IsCopyWorksheetOperation {[System.Diagnostics.DebuggerStepThrough]get { return false; } }
		public IErrorHandler ErrorHandler {[System.Diagnostics.DebuggerStepThrough]get { return errorHandler; } set { errorHandler = value; } }
		public SourceTargetRangesForCopy RangesInfo {[System.Diagnostics.DebuggerStepThrough]get { return rangesInfo; } }
		protected List<ArrayFormula> ArrayFormulasForTargetWorksheetList { get { return arrayFormulasForTargetWorksheetList; } }
		public bool RemoveHyperlinksIntersectedClearedRange { get { return removeHyperlinksIntersectedClearedRange; } set { removeHyperlinksIntersectedClearedRange = value; } }
		public bool SuppressCellValueChanged {[System.Diagnostics.DebuggerStepThrough]get { return suppressCellValueChanged; } set { suppressCellValueChanged = value; } }
		protected int DefaultTargetCellFormatIndex { get { return defaultTargetCellFormatIndex; } }
		#endregion
		public virtual bool IsSheetWillBeExternalSheet(string name) {
			return !String.Equals(SourceWorksheet.Name, name, StringComparison.OrdinalIgnoreCase);
		}
		protected internal override bool Validate() {
			bool baseValidate = base.Validate();
			if (!baseValidate)
				return baseValidate;
			IModelErrorInfo error = Checks();
			if (error != null) {
				return RegisterErrorAndShouldContinue(error);
			}
			return true;
		}
		public bool SourceTargetRangesAreIntersects() {
			return this.rangesInfo.First.SourceAndTargetAreIntersected;
		}
		protected internal override void BeginExecute() {
			BeforeBeginTargetDocumentModelUpdate();
			base.BeginExecute();
			if (!PasteSpecialOptions.ShouldCopyFormulas && PasteSpecialOptions.ShouldCopyValues && WorkbookSource.Properties.CalculationOptions.CalculationMode != ModelCalculationMode.Manual)
				CalculateSourceRange();
			Removes();
			AfterBeginTargetDocumentModelUpdate();
			targetStyleNames = GetTargetWorkbookAllStyleNames();
			if (!IsEqualWorkbook && CopyCellStyles)
				WorkbookTarget.StyleSheet.CellStyles.BeginUpdate();
		}
		protected internal override void EndExecute() {
			if (!IsEqualWorkbook && CopyCellStyles)
				WorkbookTarget.StyleSheet.CellStyles.EndUpdate();
			styleTranslationTable.Clear();
			sourceSharedFormulaToTargetSharedFormulaIndexTranslationTable.Clear();  
			BeforeEndTargetDocumentModelUpdate();
			base.EndExecute();
			AfterEndTargetDocumentModelUpdate();
		}
		protected abstract void BeforeBeginTargetDocumentModelUpdate();
		protected abstract void AfterBeginTargetDocumentModelUpdate();
		protected abstract void BeforeEndTargetDocumentModelUpdate();
		protected abstract void AfterEndTargetDocumentModelUpdate();
		#region Checks
		public virtual IModelErrorInfo Checks() {
			bool expectedCopyValuesOrFormulas = PasteSpecialOptions.ShouldCopyFormulas || PasteSpecialOptions.ShouldCopyValues;
			foreach (RangeCopyInfo item in RangesInfo) {
				if (item.ErrorType != ModelErrorType.None)
					return new ModelErrorInfo(item.ErrorType);
			}
			if (SuppressChecks)
				return null;
			DefinedNameBase conflicktedDefinedName = CheckDefinedNameConflictsAndReturnIt();
			if (conflicktedDefinedName != null)
				return new ModelErrorInfo(ModelErrorType.InternalError);
			IModelErrorInfo error = null;
			foreach (RangeCopyInfo rangeInfo in RangesInfo) {
				CellRange targetBigRange = rangeInfo.TargetBigRange;
				if (expectedCopyValuesOrFormulas)
					error = CheckTargetRangeContainsPartOfArrayFormula(rangeInfo, targetBigRange);
				if (error != null)
					return error;
				if (PasteSpecialOptions.ShouldCopyOtherFormats)
					error = CheckTargetRangeNotIntersectsMergedRanges(rangeInfo, targetBigRange);
				if (error != null)
					return error;
				error = CheckTargetRangeNotIntersectsPivotTables(rangeInfo, targetBigRange);
				if (error != null)
					return error;
			}
			return null;
		}
		protected virtual bool RegisterErrorAndShouldContinue(IModelErrorInfo error) {
			ErrorHandlingResult result = ErrorHandler.HandleError(error);
			if (result == ErrorHandlingResult.Ignore)
				return true;
			return false;
		}
		DefinedNameBase CheckDefinedNameConflictsAndReturnIt() {
			bool copyformulas = PasteSpecialOptions.ShouldCopyFormulas;
			if (suppressCheckDefinedNames || IsEqualWorksheets || !copyformulas)
				return null;
			IEnumerable<string> usedDefinedName = GetUsedDefinedNames();
			DefinedNameToCopyAnalizer analizer = new DefinedNameToCopyAnalizer(SourceWorksheet, TargetWorksheet);
			analizer.Process(usedDefinedName);
			return analizer.ConflictedDefinedName;
		}
		protected virtual IModelErrorInfo CheckTargetRangeContainsPartOfArrayFormula(RangeCopyInfo rangeInfo, CellRange targetRange) {
			ArrayFormulaRangesCollection targetArrays = TargetWorksheet.ArrayFormulaRanges;
			bool collectionNotEmpty = targetArrays.Count > 0;
			Predicate<CellRange> targetRangeIncludesOrIntersectsArrayRange =
				array => {
					bool intersectsTargetRange = rangeInfo.ObjectRangeIntersectsRangeBorder(targetRange, array);
					bool sourceRangeIntersectsTargetRange = rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.None
						&& !rangeInfo.SourceAndTargetEquals;
					bool targetRangeIncludesArrayFormulaWhenIntersection = sourceRangeIntersectsTargetRange;
					if (targetRangeIncludesArrayFormulaWhenIntersection)
						return true;
					return intersectsTargetRange;
				};
			if (collectionNotEmpty && targetArrays.Exists(targetRangeIncludesOrIntersectsArrayRange))
				return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			return null;
		}
		protected IModelErrorInfo CheckTargetRangeNotIntersectsPivotTables(RangeCopyInfo rangeInfo, CellRange targetRange) {
			List<PivotTable> intersectedPivotTables = TargetWorksheet.PivotTables.GetItems(targetRange, true);
			if (!(PasteSpecialOptions.ShouldCopyFormulas || PasteSpecialOptions.ShouldCopyValues))
				return null;
			bool copyValues = !PasteSpecialOptions.ShouldCopyFormulas && PasteSpecialOptions.ShouldCopyValues;
			bool collectionEmpty = intersectedPivotTables.Count == 0;
			bool operationAllowed = collectionEmpty || intersectedPivotTables.Exists((pivot) => {
				CellRange pivotRange = pivot.Range;
				bool srIntersectsSoR = RangeIntersectsPivot(rangeInfo, SourceRange, pivot.WholeRange); 
				if (rangeInfo.Info.SourceAndTargetEquals)
					return copyValues && !srIntersectsSoR;
				CellRange pivotCoveredRange = pivot.WholeRange.GetCoveredRange();
				if (CutMode && rangeInfo.RangeHasEqualsPosition(pivotCoveredRange, targetRange)) {
					return true;
				}
				if (pivotRange.Includes(targetRange)) { 
					if (CutMode)
						return false; 
					return !copyValues;
				}
				if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
					if (CutMode && SourceRange.ContainsRange(pivotRange) && !rangeInfo.ObjectRangeIntersectsRangeBorder(SourceRange, pivotRange))
						return true;
					if (targetRange.Includes(pivot.WholeRange) && srIntersectsSoR)
						return false;
					return false;
				}
				if (!targetRange.Includes(pivotRange))
					return false;
				return true;
			});
			return operationAllowed ? null : new ModelErrorInfo(ModelErrorType.SelectedCellsAffectsPivotTableAndCanNotBeChanged);
		}
		protected bool RangeIntersectsPivot(RangeCopyInfo rangeInfo, CellRange range, CellRangeBase wholeRange) {
			CellRange singleRange = wholeRange as CellRange;
			Predicate<CellRange> x = partOfPivotRange => partOfPivotRange.Intersects(range);
			bool result = wholeRange.Exists(x) && !(singleRange != null
				&& (rangeInfo.RangeHasEqualsPosition(range, singleRange)
						|| range.ContainsRange(singleRange))
				);
			bool rangeContainsAllPivotRanges = false;
			if (singleRange == null) {
				CellRange coveredRange = wholeRange.GetCoveredRange();
				rangeContainsAllPivotRanges = range.ContainsRange(coveredRange);
				if (rangeContainsAllPivotRanges)
					return false; 
			}
			return result;
		}
		protected IModelErrorInfo CheckTargetRangeNotIntersectsMergedRanges(RangeCopyInfo rangeInfo, CellRange range) {
			if (range.RangeType == CellRangeType.IntervalRange)
				return null;
			List<CellRange> intersectedMergedCellRanges = TargetWorksheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
			IModelErrorInfo result = CheckMergedRangesIntersectedTargetRange(rangeInfo, range, intersectedMergedCellRanges);
			return result;
		}
		protected virtual IModelErrorInfo CheckMergedRangesIntersectedTargetRange(RangeCopyInfo rangeInfo, CellRange targetRange, List<CellRange> intersectedMergedCellRanges) {
			bool collectionEmpty = intersectedMergedCellRanges.Count == 0;
			bool operationAllowed = collectionEmpty || intersectedMergedCellRanges.Exists((mergedRange) => {
				if (SourceRange.CellCount == 1 && mergedRange.TopLeft.EqualsPosition(targetRange.TopLeft)
			   && !rangeInfo.Info.SourceAndTargetEquals) {
					rangeInfo.PasteSingleCellToMergedRange = true;
					return true;
				}
				if (!targetRange.Includes(mergedRange))
					return false;
				if (rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.None) {
					foreach (Model.CellRange currentTargetRange in rangeInfo.TargetRanges) {
						if (rangeInfo.ObjectRangeIntersectsRangeBorder(currentTargetRange, mergedRange))
							return false;
					}
				}
				if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
					CellRange sourceRange = rangeInfo.SourceRange;
					if (rangeInfo.ObjectRangeIntersectsRangeBorder(sourceRange, mergedRange))
						return false;
				}
				return true;
			});
			return operationAllowed ? null : new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
		}
		#endregion
		#region Removes
		public void Removes() {
			if (IsCopyWorksheetOperation)
				return;
			bool replaceFormulasToValues = PasteSpecialOptions.ShouldCopyValues && !PasteSpecialOptions.ShouldCopyFormulas;
			foreach (RangeCopyInfo rangeInfo in RangesInfo) {
				RemoveFromTargetBigRange(rangeInfo, replaceFormulasToValues);
			}
		}
		void RemoveFromTargetBigRange(RangeCopyInfo rangeInfo, bool replaceFormulasWithCalculatedValues) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			if (rangeInfo.SourceAndTargetEquals)
				UnmergeWholeMergedRangesInTargetRange(targetBigRange); 
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.None)
				RemoveArrayFormulasAndValuesFrom(targetBigRange);
			if (rangeInfo.SourceAndTargetEquals && replaceFormulasWithCalculatedValues) {
				TargetWorksheet.RemoveArrayFormulasFromCollectionInsideRange((ISheetPosition)rangeInfo.SourceRange);
				ConvertSharedFormulaToNormalFormulaAndRecalculate(rangeInfo.SourceRange);
			}
			RemoveHyperlinks(rangeInfo);
			UnmergeWholeMergedRangesInTargetRange(targetBigRange);
			RemovePivotTablesFromTargetRange(rangeInfo);
			RemoveComments(rangeInfo);
			RemoveProtectedRanges(rangeInfo);
			RemoveSharedFormulas(rangeInfo);
			if (PasteSpecialOptions.ShouldCopyOtherFormats) {
				RemoveTablesFromTargetRange(rangeInfo);
				Predicate<ConditionalFormatting> notIntersectSourceRange = (cf) => {
					return !cf.CellRange.Intersects(SourceRange);
				};
				TargetWorksheet.ClearConditionalFormattingsFromRange(targetBigRange, notIntersectSourceRange);
				CellRangeBase targetRangeExceptSourceRange = targetBigRange;
				if (rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.None) {
					if (rangeInfo.SourceAndTargetEquals)
						targetRangeExceptSourceRange = null;
					else
						targetRangeExceptSourceRange = targetBigRange.ExcludeRange(SourceRange);
				}
				if (targetRangeExceptSourceRange != null) {
					TargetWorksheet.IgnoredErrors.Clear(targetRangeExceptSourceRange);
					RemoveCellTagsFromTargetWorksheet(targetRangeExceptSourceRange);
				}
			}
		}
		void RemoveCellTagsFromTargetWorksheet(CellRangeBase targetRangeExceptSourceRange) {
			if (TargetWorksheet.CellTags.IsEmpty)
				return;
			RemoveRangeNotificationContext context = new RemoveRangeNotificationContext() {
				Mode = RemoveCellMode.Default
			};
			targetRangeExceptSourceRange.ForEach(innerRange => {
				context.Range = innerRange;
				TargetWorksheet.CellTags.OnRangeRemoving(context);
			});
		}
		void ConvertSharedFormulaToNormalFormulaAndRecalculate(CellRange cellRange) {
			IRangeCopySharedFormulaPart op = this as IRangeCopySharedFormulaPart;
			IDictionary<int, SharedFormula> sourceFormulas = GetInnerSharedFormulas(cellRange, (sfRange) => sfRange.Intersects(cellRange));
			CellRange rangeForRecalc = null;
			foreach (var item in sourceFormulas) {
				CellRange sharedFormulaRange = item.Value.Range;
				CellRange sharedFormulaRangeInsideSR = sharedFormulaRange.Intersection(cellRange);
				rangeForRecalc = rangeForRecalc != null ? rangeForRecalc.Expand(sharedFormulaRangeInsideSR) : sharedFormulaRangeInsideSR;
				op.RemoveCellsFromAffectedRange(item.Value, sharedFormulaRangeInsideSR);
				if (cellRange.ContainsRange(item.Value.Range))
					SourceWorksheet.SharedFormulas.MarkUpForDeletion(item.Key);
			}
			SourceWorksheet.SharedFormulas.RemoveMarkedItems();
			if (rangeForRecalc != null)
				WorkbookSource.CalculationChain.CalculateRangeIfHasMarkedCells(rangeForRecalc);
		}
		void RemoveSharedFormulas(RangeCopyInfo rangeInfo) {
			bool shouldRemoveSharedFormulas = (PasteSpecialOptions.ShouldCopyFormulas || PasteSpecialOptions.ShouldCopyValues)
				&& rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.None
				&& !rangeInfo.SourceAndTargetEquals;
			if (!shouldRemoveSharedFormulas)
				return;
			CellRange targetRange = rangeInfo.TargetBigRange;
			Predicate<CellRange> insideTargetRange = sfRange => targetRange.ContainsRange(sfRange);
			foreach (var pair in GetInnerSharedFormulas(targetRange, insideTargetRange)) {
				SharedFormula sf = pair.Value;
				(this as IRangeCopySharedFormulaPart).RemoveCellsFromAffectedRange(sf, sf.Range);
				TargetWorksheet.SharedFormulas.MarkUpForDeletion(pair.Key);
			}
			TargetWorksheet.SharedFormulas.RemoveMarkedItems();
		}
		public void RemoveArrayFormulasAndValuesFrom(CellRange range) {
			bool expectedCopyValuesOrFormulas = PasteSpecialOptions.ShouldCopyFormulas || PasteSpecialOptions.ShouldCopyValues;
			if (!expectedCopyValuesOrFormulas)
				return;
			ArrayFormulaRangesCollection arrayFormulas = TargetWorksheet.ArrayFormulaRanges;
			int arrayIndex = arrayFormulas.Count;
			while (true) {
				arrayIndex = arrayFormulas.FindArrayFormulaIndexInsideRange(range, arrayIndex - 1);
				if (arrayIndex < 0)
					return;
				CellRange currentArrayRange = arrayFormulas[arrayIndex];
				TargetWorksheet.RemoveArrayFormulasAndValuesFrom(currentArrayRange);
			}
		}
		protected virtual void RemoveHyperlinks(RangeCopyInfo rangeInfo) {
			if (!(PasteSpecialOptions.ShouldCopyOtherFormats) || rangeInfo.SourceAndTargetEquals)
				return;
			CellRange targetRange = rangeInfo.TargetBigRange;
			if (rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
				Predicate<CellRange> match = (hlinkRange) => {
					return targetRange.Includes(hlinkRange);
				};
				if (RemoveHyperlinksIntersectedClearedRange) {
					CellPosition targetTL = targetRange.TopLeft;
					match = (hlinkRange) => {
						bool intersectsTargetRangeSpecial = targetRange.Intersects(hlinkRange)
							&& !hlinkRange.Intersects(SourceRange)
							&& hlinkRange.ContainsCell(targetTL.Column, targetTL.Row); 
						return targetRange.Includes(hlinkRange) || intersectsTargetRangeSpecial;
					};
				}
				RemoveHyperlinksFromRange(targetRange, match);
			}
			else {
				CellRangeBase sourceTargetRangeIntersection = SourceRange.Intersection(targetRange);
				Predicate<CellRange> removeCondition = (hlinkRange) => {
					bool insideTargetRange = targetRange.Includes(hlinkRange);
					bool insideSourceTargetRangeIntersection = sourceTargetRangeIntersection == null ? false : sourceTargetRangeIntersection.Includes(hlinkRange);
					bool result = insideTargetRange  && !insideSourceTargetRangeIntersection;
					return result;
				};
				RemoveHyperlinksFromRange(targetRange, removeCondition);
			}
		}
		protected void RemoveHyperlinksAnyIntersectedRange(CellRangeBase range) {
			Predicate<CellRange> match = (hlinkRange) => {
				return range.Includes(hlinkRange) || hlinkRange.Includes(range);
			};
			RemoveHyperlinksFromRange(range, match);
		}
		protected void RemoveHyperlinksFromRangeNotIntersectedRangeBorders(CellRangeBase range) {
			Predicate<CellRange> match = (hlinkRange) => {
				return range.Includes(hlinkRange);
			};
			RemoveHyperlinksFromRange(range, match);
		}
		protected void RemoveHyperlinksFromRange(CellRangeBase range, Predicate<CellRange> match) {
			Worksheet worksheet = range.Worksheet as Worksheet;
			List<int> indexesSortedDescending = worksheet.Hyperlinks.GetIndexesInsideRangeSortedDescending(match);
			indexesSortedDescending.ForEach((index) => worksheet.RemoveHyperlinkAt(index));
		}
		public void UnmergeWholeMergedRangesInTargetRange(CellRange _currentTargetRange) {
			if (!PasteSpecialOptions.ShouldCopyOtherFormats)
				return;
			IList<CellRange> mergedRangesInsideCurrentTargetRange = GetMergedCellRangesWithoutIntersectionWithRangeBorders(_currentTargetRange);
			foreach (CellRange item in mergedRangesInsideCurrentTargetRange) {
				if (!RangesInfo.First.PasteSingleCellToMergedRange)
					TargetWorksheet.UnMergeCells(item, this.ErrorHandler);
			}
		}
		public List<CellRange> GetMergedCellRangesWithoutIntersectionWithRangeBorders(CellRange range) {
			List<CellRange> foundMergedRanges = (range.Worksheet as Worksheet).MergedCells.GetMergedCellRangesIntersectsRange(range);
			List<CellRange> withoutIntersectionWithRangeBorders = new List<CellRange>();
			for (int i = 0; i < foundMergedRanges.Count; i++) {
				CellRange foundMergedRange = foundMergedRanges[i];
				if (range.ContainsRange(foundMergedRange)) 
					withoutIntersectionWithRangeBorders.Add(foundMergedRange);
			}
			return withoutIntersectionWithRangeBorders;
		}
		void RemoveComments(RangeCopyInfo rangeInfo) {
			bool allowed = PasteSpecialOptions.ShouldCopyOtherFormats || PasteSpecialOptions.ShouldCopyComments;
			if (!allowed)
				return;
			CellRange targetRange = rangeInfo.TargetBigRange;
			Predicate<Model.Comment> condition = (current) => {
				CellPosition commentPosition = current.Reference;
				CellKey commentKey = commentPosition.ToKey(TargetWorksheet.SheetId);
				bool insideSource = SourceRange.ContainsCell(commentKey);
				bool fromTargetRange = targetRange.ContainsCell(commentKey);
				return fromTargetRange && !insideSource;
			};
			TargetWorksheet.ClearComments(condition);
		}
		#endregion
		void RemovePivotTablesFromTargetRange(RangeCopyInfo rangeInfo) {
			bool replaceFormulaWithCalculatedValues = PasteSpecialOptions.ShouldCopyValues && !pasteSpecialOptions.ShouldCopyFormulas;
			bool shouldRemove = PasteSpecialOptions.ShouldCopyValues || PasteSpecialOptions.ShouldCopyFormulas;
			if (!shouldRemove)
				return;
			if (rangeInfo.SourceAndTargetEquals && !replaceFormulaWithCalculatedValues)
				return;
			const bool intersectes = false;
			List<PivotTable> itemsToRemove = TargetWorksheet.PivotTables.GetItems(rangeInfo.TargetBigRange, intersectes);
			int count = itemsToRemove.Count;
			CellRange sourceRange = rangeInfo.SourceRange;
			for (int i = 0; i < count; i++) {
				PivotTable table = itemsToRemove[i];
				CellRange tableRange = table.Range;
				bool case1 = sourceRange.ContainsRange(tableRange) && !replaceFormulaWithCalculatedValues;
				bool tableShouldLive = case1;
				if (!tableShouldLive) {
					PivotTableRemoveCommand c = new PivotTableRemoveCommand(table);
					c.ShouldClearHistory = false;
					c.Execute();
				}
			}
		}
		void RemoveTablesFromTargetRange(RangeCopyInfo rangeInfo) {
			bool allowed = PasteSpecialOptions.ShouldCopyOtherFormats || pasteSpecialOptions.ShouldCopyTables;
			if (!allowed || rangeInfo.SourceAndTargetEquals)
				return;
			CellRange currentTargetRange = rangeInfo.TargetBigRange;
			IList<Table> tablesToDelete = TargetWorksheet.Tables.GetItems(currentTargetRange, false);
			int count = tablesToDelete.Count;
			CellRange sourceRange = rangeInfo.SourceRange;
			for (int i = 0; i < count; i++) {
				Table table = tablesToDelete[i];
				CellRange tableRange = table.Range;
				bool case1 = sourceRange.ContainsRange(tableRange);
				bool case2 = sourceRange.TopLeft.EqualsPosition(tableRange.TopLeft)
					&& sourceRange.TopLeft.EqualsPosition(currentTargetRange.TopLeft);
				bool case3 = rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
					&& currentTargetRange.ContainsRange(tableRange)
					&& currentTargetRange.TopLeft.EqualsPosition(tableRange.TopLeft);
				bool tableShouldLive = case1 || case2 || case3;
				if (!tableShouldLive) 
					TargetWorksheet.RemoveTable(table);
			}
		}
		public virtual void CopyContent(RangeCopyInfo rangeCopyInfo) {
			CopyDefinedNames(rangeCopyInfo.TargetBigRange);
			CopySharedFormulaList(rangeCopyInfo);
			CopyArrayFormulaList(rangeCopyInfo);
			CopyRows(rangeCopyInfo);
			CopyColumns(rangeCopyInfo);
			CopyMergedCells(rangeCopyInfo);
			CopyHyperlinks(rangeCopyInfo);
			CopyTables(rangeCopyInfo);
			CopyCharts();
			CopyDrawingObjects(rangeCopyInfo);
			CopyComments(rangeCopyInfo);
			CopyProtectedRanges(rangeCopyInfo);
			CopyDataValidations(rangeCopyInfo);
			CopyConditionalFormattings(rangeCopyInfo);
			CopyVmlDrawing();
			CopyAutoFilter(rangeCopyInfo);
			CopySparklines(rangeCopyInfo);
			CopyIgnoredErrors(rangeCopyInfo);
			CopyPivotTables(rangeCopyInfo);
		}
		public abstract CellPositionOffset GetTargetRangeOffset(CellRange currentTargetRange);
		public abstract CellRangeBase GetTargetObjectRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange);
		public virtual CellRangeBase GetRangeToClearAfterCut() {
			throw new InvalidOperationException();
		}
		protected ParsedExpression ConvertParsedExpressionWithShiftReferencesOrNull(ParsedExpression sourceParsedExpression, CellRange currentTargetRange) {
			return ConvertParsedExpressionWithShiftReferencesOrNull(sourceParsedExpression, GetTargetRangeOffset(currentTargetRange));
		}
		protected ParsedExpression ConvertParsedExpressionWithShiftReferencesOrNull(ParsedExpression sourceParsedExpression, CellPositionOffset offset) {
			ParsedExpression sourceExpressionClone = sourceParsedExpression.Clone();
			ParsedExpression modifiedExpression = ModifyFormulaExpressionWithReferenceThingsOffsetRPNVisitor(sourceExpressionClone, offset);
			return modifiedExpression;
		}
		public virtual PrepareTargetParsedExpressionVisitor CreateFormulaWalker(Worksheet targetWorksheet, CellRange sourceRange, ICopyEverythingArguments args) {
			bool diffWorkbooks = !IsEqualWorkbook;
			PrepareTargetParsedExpressionVisitor walker = null;
			if (diffWorkbooks)
				walker = new PrepareTargetParsedExpressionVisitorDiffWorkbooks(targetWorksheet, args);
			else
				walker = new PrepareTargetParsedExpressionVisitor(args.TargetDataContext, targetWorksheet, args);
			return walker;
		}
		protected ParsedExpression ModifyFormulaExpressionWithReferenceThingsOffsetRPNVisitor(ParsedExpression copiedExpression, CellPositionOffset offset) {
			walker.Offset = offset;
			ParsedExpression result = ModifyFormulaExpressionCore(copiedExpression, walker);
			return result; 
		}
		protected ParsedExpression ModifyFormulaExpressionCore(ParsedExpression copiedExpression, ReplaceThingsPRNVisitor walker) {
			try {
				TargetWorksheet.DataContext.PushCurrentWorkbook(WorkbookTarget);
				TargetWorksheet.DataContext.PushCurrentWorksheet(TargetWorksheet);
				walker.PushCurrentSourceWorksheet(SourceWorksheet);
				return walker.Process(copiedExpression);
			}
			finally {
				walker.PopCurrentSourceWorksheet();
				TargetWorksheet.DataContext.PopCurrentWorksheet();
				TargetWorksheet.DataContext.PopCurrentWorkbook();
			}
		}
		bool CopyCellNormalFormulaCore(ICell source, FormulaBase formula, ICell target, CellRange _currentTargetRange) {
			ParsedExpression sourceExpression = formula.Expression;
			if (!CutMode && sourceExpression.Count == 1) { 
				IParsedThing singleThing = sourceExpression[0];
				if (singleThing is ParsedThingError || singleThing is ParsedThingRefErr) {
					VariantValue sourcevalue = source.Value;
					if (!sourcevalue.IsError)
						return false; 
				}
			}
			ParsedExpression modifiedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceExpression, _currentTargetRange);
			if (modifiedExpression == null) 
				return false;  
			Formula targetCellFormula = new Formula(target, modifiedExpression);
			target.SetFormula(targetCellFormula);
			return true;
		}
		#region DefinedNames
		protected virtual void CopyDefinedNames(CellRange targetBigRange) {
			if (!PasteSpecialOptions.ShouldCopyFormulas)
				return;
			if (IsEqualWorksheets)
				return;
			foreach (DefinedNameBase item in GetDefinedNamesToCopyWithChecking()) {
				DefinedName sourceDefinedName = item as DefinedName;
				CopyDefinedNameCore(sourceDefinedName, targetBigRange);
			}
			if (!IsEqualWorkbook && referencesWithSheetDefinitionToOuterAreasReplaceMode != SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
				ProcessParsedThingNameXInRange();
			}
		}
		protected void CopyDefinedNameCore(DefinedName sourceDefinedName, CellRange _currentTargetRange) {
			WorkbookDataContext sourceDataContext = SourceWorksheet.DataContext;
			sourceDataContext.PushDefinedNameProcessing(sourceDefinedName);
			try {
				TryAddDefinedNameToTarget(sourceDefinedName, GetTargetDefinedNameExpression, _currentTargetRange);
			}
			finally {
				sourceDataContext.PopDefinedNameProcessing();
			}
		}
		void TryAddDefinedNameToTarget(DefinedName sourceDefinedName, Func<string, DefinedNameBase, CellRange, ParsedExpression> targetDefinedNameExpressionConverter, CellRange _currentTargetRange) {
			DefinedNameBase targetExisting = null;
			string name = sourceDefinedName.Name;
			if (sourceDefinedName.ScopedSheetId >= 0) {
				if (TargetWorksheet.DefinedNames.TryGetItemByName(name, out targetExisting)) {
					return;
				}
			}
			else {
				if (WorkbookTarget.DefinedNames.TryGetItemByName(name, out targetExisting)) {
					return;
				}
			}
			ParsedExpression targetDefinedNameExpression = targetDefinedNameExpressionConverter(sourceDefinedName.Name, sourceDefinedName as DefinedNameBase, _currentTargetRange);
			if (sourceDefinedName.ScopedSheetId >= 0)
				AddLocalDefinedName(sourceDefinedName, targetDefinedNameExpression);
			else
				AddGlobalDefinedName(sourceDefinedName, targetDefinedNameExpression);
		}
		DefinedNameBase AddLocalDefinedName(DefinedName sourceDefinedName, ParsedExpression targetDefinedNameExpression) {
			DefinedName newTargetDefinedName = TargetWorksheet.CreateDefinedName(sourceDefinedName.Name, targetDefinedNameExpression);
			newTargetDefinedName.Comment = sourceDefinedName.Comment;
			newTargetDefinedName.IsHidden = sourceDefinedName.IsHidden;
			return newTargetDefinedName;
		}
		DefinedName AddGlobalDefinedName(DefinedName sourceDefinedName, ParsedExpression targetDefinedNameExpression) {
			DefinedName newTargetDefinedName = TargetWorksheet.Workbook.CreateDefinedName(sourceDefinedName.Name, targetDefinedNameExpression);
			newTargetDefinedName.Comment = sourceDefinedName.Comment;
			newTargetDefinedName.IsHidden = sourceDefinedName.IsHidden;
			return newTargetDefinedName;
		}
		protected virtual IEnumerable<DefinedNameBase> GetDefinedNamesToCopyWithChecking() {
			IEnumerable<string> usedDefinedName = GetUsedDefinedNames();
			DefinedNameToCopyAnalizer analizer = new DefinedNameToCopyAnalizer(SourceWorksheet, TargetWorksheet);
			analizer.Process(usedDefinedName);
			if (analizer.ConflictedDefinedName != null) {
				throw new InvalidOperationException();
			}
			return analizer.SourceDefinedNamesToCopy.Values;
		}
		protected virtual IEnumerable<string> GetUsedDefinedNames() {
			return GetUsedDefinedNamesFromRange(SourceRange);
		}
		protected HashSet<string> GetUsedDefinedNamesFromRange(CellRange range) {
			CopyFormulaGatherAllDefinedNamesRPNVisitor visitor = new CopyFormulaGatherAllDefinedNamesRPNVisitor(this as ICopyEverythingArguments);
			visitor.SuppressParsedThingXFinding = SheetDefinitionToOuterAreasReplaceMode == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue;
			try {
				SourceDataContext.PushCurrentWorksheet(SourceWorksheet);
				foreach (CellBase rangeInfo in range.GetExistingCellsEnumerable()) {
					ICell cell = rangeInfo as ICell;
					if (!cell.HasFormula)
						continue;
					ParsedExpression sourceParsedExpression = cell.GetFormula().Expression;
					visitor.Process(sourceParsedExpression);
				}
				foreach (DataValidation validation in GetSourceDataValidations()) {
					visitor.Process(validation.Expression1);
					visitor.Process(validation.Expression2);
				}
			}
			finally {
				SourceWorksheet.DataContext.PopCurrentWorksheet();
			}
			return visitor.Result;
		}
		protected ParsedExpression GetTargetDefinedNameExpression(string Name, DefinedNameBase sourceDefinedName, CellRange _currentTargetRange) {
			try {
				DataContext.PushDefinedNameProcessing(sourceDefinedName);
				ParsedExpression sourceExpression = sourceDefinedName.Expression;
				ParsedExpression withModifiedReferences = ConvertParsedExpressionWithShiftReferencesOrNull(sourceExpression, _currentTargetRange);
				if (withModifiedReferences == null)
					return CreateParsedThingRefErrExpression();
				if (!CutMode && IsEqualWorkbook) {
					RenameSheetDefinitionFormulaWalker renameSheetDefinitionFormulaWalker = new RenameSheetDefinitionFormulaWalker(SourceWorksheet.Name, TargetWorksheet.Name, TargetWorksheet.DataContext);
					ParsedExpression withChangedSheetName = ModifyFormulaExpressionCore(withModifiedReferences, renameSheetDefinitionFormulaWalker);
					return withChangedSheetName;
				}
				return withModifiedReferences;
			}
			finally {
				DataContext.PopDefinedNameProcessing();
			}
		}
		ParsedExpression CreateParsedThingRefErrExpression() {
			ParsedExpression result = new ParsedExpression();
			result.Add(new ParsedThingRefErr());
			return result;
		}
		protected void ProcessParsedThingNameXInRange() {
			WorkbookDataContext sourceDataContext = SourceWorksheet.DataContext;
			CopyFormulaGatherAllDefinedNamesRPNVisitor visitor = new CopyFormulaGatherAllDefinedNamesRPNVisitor(this as ICopyEverythingArguments);
			visitor.SuppressParsedThingXFinding = SheetDefinitionToOuterAreasReplaceMode == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue;
			sourceDataContext.PushCurrentWorksheet(SourceWorksheet);
			try {
				foreach (CellBase rangeInfo in SourceRange.GetExistingCellsEnumerable()) {
					ICell cell = rangeInfo as ICell;
					if (!cell.HasFormula)
						continue;
					visitor.Process(cell.Formula.Expression);
				}
				PrepareTargetParsedExpressionVisitorDiffWorkbooks diffworkbookVisitor = new PrepareTargetParsedExpressionVisitorDiffWorkbooks(TargetWorksheet, this);
				foreach (ParsedThingNameX item in visitor.ThingsWithExternalSheetDefinitions)
					ProcessParsedThingNameXWithExternalSheetDefinition(item, diffworkbookVisitor);
			}
			finally {
				sourceDataContext.PopCurrentWorksheet();
			}
		}
		void ProcessParsedThingNameXWithExternalSheetDefinition(ParsedThingNameX thing, PrepareTargetParsedExpressionVisitorDiffWorkbooks visitor) {
			SheetDefinition sourceSheetDefinition = SourceDataContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			string sheetNameStart = sourceSheetDefinition.SheetNameStart;
			bool isSheetDefinitionExternal = sourceSheetDefinition.IsExternalReference;
			bool isWorkbookGlobalNotExternalDefinedName = !isSheetDefinitionExternal
				&& (String.IsNullOrEmpty(sheetNameStart) || SourceDataContext.GetDefinedName(thing.DefinedName, sourceSheetDefinition).ScopedSheetId < 0);
			bool definedNameIsLocalInSourceSheet = !isSheetDefinitionExternal && !isWorkbookGlobalNotExternalDefinedName && String.Equals(sheetNameStart, this.SourceWorksheet.Name, StringComparison.OrdinalIgnoreCase);
			bool definedNameLocalInOtherSheetsToCopy = !isSheetDefinitionExternal && !isWorkbookGlobalNotExternalDefinedName && !IsSheetWillBeExternalSheet(sheetNameStart);
			bool definedNameIsLocal =
				!(definedNameIsLocalInSourceSheet || definedNameLocalInOtherSheetsToCopy || isWorkbookGlobalNotExternalDefinedName);
			bool createExternalDefinedName = true;
			if (IsCopyWorksheetOperation && definedNameIsLocal
				&& !isSheetDefinitionExternal)  
				createExternalDefinedName = false;
			if (createExternalDefinedName) {
				int targetSheetDefinitionIndex = visitor.GetTargetSheetDefinition(sourceSheetDefinition, !createExternalDefinedName);
				AddExternalDefinedName(thing.DefinedName, sourceSheetDefinition, targetSheetDefinitionIndex);
			}
		}
		void AddExternalDefinedName(string name, SheetDefinition sourceSheetDefinition, int targetSheetDefinitionIndex) {
			string sheetNameStart = sourceSheetDefinition.SheetNameStart;
			IModelWorkbook sourceWorkbookWhereDefinedNameIs = null;
			IWorksheet sourceSheetWhereDefinedNameIs = null;
			DefinedNameBase sourceDefinedName = null;
			if (sourceSheetDefinition.IsExternalReference) {
				sourceWorkbookWhereDefinedNameIs = WorkbookSource.GetExternalWorkbookByIndex(sourceSheetDefinition.ExternalReferenceIndex);
			}
			else {
				sourceWorkbookWhereDefinedNameIs = WorkbookSource;
			}
			sourceSheetWhereDefinedNameIs = sourceWorkbookWhereDefinedNameIs.GetSheetByName(sheetNameStart);
			sourceDefinedName = GetSourceDefinedName(SourceDataContext, name, sourceWorkbookWhereDefinedNameIs, sourceSheetWhereDefinedNameIs);
			try {
				bool isGlobalDf = String.IsNullOrEmpty(sourceSheetDefinition.SheetNameStart) || sourceDefinedName.ScopedSheetId < 0;
				SourceDataContext.PushCurrentWorkbook(sourceWorkbookWhereDefinedNameIs);
				SourceDataContext.PushCurrentWorksheet(sourceSheetWhereDefinedNameIs);
				SheetDefinition targetSheetDefinition = WorkbookTarget.SheetDefinitions[targetSheetDefinitionIndex];
				DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook targetExternalWorkbook = WorkbookTarget.GetExternalWorkbookByIndex(targetSheetDefinition.ExternalReferenceIndex);
				DevExpress.XtraSpreadsheet.Model.External.ExternalWorksheet targetExternalSheet = targetExternalWorkbook.Sheets[sourceSheetDefinition.SheetNameStart];
				ParsedExpression targetReference = new ParsedExpression();
				this.DataContext.PushCurrentWorkbook(targetExternalWorkbook);
				this.DataContext.PushCurrentWorksheet(targetExternalSheet);
				if (sourceDefinedName != null) {
					ParsedExpression clonned = sourceDefinedName.Expression.Clone();
					CloneShDefFormulaWalker walker = new CloneShDefFormulaWalker(this.SourceDataContext, this.DataContext);
					targetReference = walker.Process(clonned);
					if (targetReference == null)
						targetReference = CreateParsedThingRefErrExpression();
				}
				if (isGlobalDf)
					AddGlobalExternalDefinedName(name, targetReference, targetExternalWorkbook);
				else
					AddExternalSheetDefinedName(name, targetReference, sourceSheetWhereDefinedNameIs, targetExternalWorkbook, targetExternalSheet);
			}
			finally {
				this.DataContext.PopCurrentWorksheet();
				this.DataContext.PopCurrentWorkbook();
				SourceDataContext.PopCurrentWorkbook();
				SourceDataContext.PopCurrentWorksheet();
			}
		}
		DefinedNameBase AddExternalSheetDefinedName(string sourceDefinedNameName, ParsedExpression expression, IWorksheet referencedSheet, DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook targetExternalWorkbook, DevExpress.XtraSpreadsheet.Model.External.ExternalWorksheet targetExternalSheet) {
			int externalSheetId = targetExternalSheet.SheetId;
			DefinedNameBase targetExternalDefinedName = null;
			if (targetExternalSheet.DefinedNames.TryGetItemByName(sourceDefinedNameName, out targetExternalDefinedName)) {
				if (!targetExternalDefinedName.Expression.Equals(expression))
					System.Diagnostics.Debug.WriteLine(String.Format("target existing external local definedName {0} has a different reference", sourceDefinedNameName));
				return targetExternalDefinedName;
			}
			targetExternalDefinedName = new Model.External.ExternalDefinedName(sourceDefinedNameName, targetExternalWorkbook, expression, externalSheetId);
			targetExternalSheet.DefinedNames.Add(targetExternalDefinedName);
			return targetExternalDefinedName;
		}
		DefinedNameBase AddGlobalExternalDefinedName(string sourceDefinedNameName, ParsedExpression expression, DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook targetExternalWorkbook) {
			DefinedNameBase targetExternalDefinedName = null;
			if (targetExternalWorkbook.DefinedNames.TryGetItemByName(sourceDefinedNameName, out targetExternalDefinedName)) {
				if (!targetExternalDefinedName.Expression.Equals(expression))
					System.Diagnostics.Debug.WriteLine(String.Format("target existing external global definedName {0} has a different reference", sourceDefinedNameName));
				return targetExternalDefinedName;
			}
			targetExternalDefinedName = new Model.External.ExternalDefinedName(sourceDefinedNameName, targetExternalWorkbook, expression, -1);
			targetExternalWorkbook.DefinedNames.Add(targetExternalDefinedName);
			return targetExternalDefinedName;
		}
		DefinedNameBase GetSourceDefinedName(WorkbookDataContext sourceContext, string definedNameName, IModelWorkbook workbook, IWorksheet sheet) {
			try {
				sourceContext.PushCurrentWorksheet(sheet);
				sourceContext.PushCurrentWorkbook(workbook);
				return sourceContext.GetDefinedName(definedNameName);
			}
			finally {
				sourceContext.PopCurrentWorksheet();
				sourceContext.PopCurrentWorkbook();
			}
		}
		#endregion
		#region SharedFormulas
		protected abstract IDictionary<int, SharedFormula> GetInnerSharedFormulas(CellRange range, Predicate<CellRange> sfRangeMatch);
		protected virtual void CopySharedFormulaList(RangeCopyInfo rangeInfo) {
			if (!PasteSpecialOptions.ShouldCopyFormulas)
				return;
			try {
				WorkbookSource.DataContext.PushSharedFormulaProcessing(true);
				WorkbookTarget.DataContext.PushSharedFormulaProcessing(true);
				foreach (KeyValuePair<int, SharedFormula> sourceSharedFormulaPair
					in GetInnerSharedFormulas(SourceRange, (sfRange) => sfRange.Intersects(SourceRange))) {
					SharedFormula sourceSharedFormula = sourceSharedFormulaPair.Value;
					int existingSharedFormulaIndex = sourceSharedFormulaPair.Key;
					CellRange sourceSharedFormulaRange = sourceSharedFormula.Range;
					if (sourceSharedFormulaRange == null)
						continue;
					CellPosition firstCellPosition = new CellPosition(sourceSharedFormulaRange.TopLeft.Column, sourceSharedFormulaRange.TopLeft.Row);
					WorkbookSource.DataContext.PushCurrentCell(firstCellPosition);
					try {
						ProcessSourceSharedFormula(rangeInfo, sourceSharedFormula, existingSharedFormulaIndex, sourceSharedFormulaRange);
					}
					finally {
						WorkbookSource.DataContext.PopCurrentCell();
					}
				}
			}
			finally {
				WorkbookSource.DataContext.PopSharedFormulaProcessing();
				WorkbookTarget.DataContext.PopSharedFormulaProcessing();
			}
		}
		void IRangeCopySharedFormulaPart.UseExistingSharedFormula(SharedFormula sourceSharedFormula, int existingSharedFormulaIndex) {
			sourceSharedFormulaToTargetSharedFormulaIndexTranslationTable.Add(sourceSharedFormula, existingSharedFormulaIndex);
		}
		void IRangeCopySharedFormulaPart.ModifyExistingSharedFormulaRange(SharedFormula sourceSharedFormula, CellRange newRange) {
			sourceSharedFormula.SetRange(newRange);
		}
		void IRangeCopySharedFormulaPart.CreateTargetSharedFormula(SharedFormula sourceSharedFormula, CellRange targetSharedFormulaRange, CellRange currentTargetRange) {
			SharedFormula targetNewSharedFormula = CreateTargetSharedFormulaOrNull(sourceSharedFormula, targetSharedFormulaRange, currentTargetRange);
			if (targetNewSharedFormula != null) {
				int desiredSharedFormulaIndex = TargetWorksheet.SharedFormulas.Add(targetNewSharedFormula);
				SourceSharedFormulaToTargetSharedFormulaIndexTranslationTable.Add(sourceSharedFormula, desiredSharedFormulaIndex);
			}
		}
		void IRangeCopySharedFormulaPart.RemoveSourceSharedFormula(CellRangeBase sourceSharedFormulaRange, int existingSharedFormulaIndex) {
			SourceWorksheet.SharedFormulas.Remove(existingSharedFormulaIndex);
		}
		List<CellRange> IRangeCopySharedFormulaPart.SharedFormulaGetReferencedRangesInSourceWorksheet(SharedFormula sf, CellRange sfRange) {
			List<CellRange> result = new List<CellRange>();
			CellPosition topLeftCell = sfRange.TopLeft;
			Worksheet worksheet = SourceWorksheet;
			WorkbookDataContext context = worksheet.Workbook.DataContext;
			context.PushCurrentWorksheet(worksheet);
			context.PushCurrentCell(topLeftCell);
			context.PushSharedFormulaProcessing(true);
			try {
				GetFormulaRangesRPNVisitor visitor = new GetFormulaRangesRPNVisitor(context, null);
				CellRangeBase resultRange = visitor.Perform(sf.Expression, worksheet);
				if (resultRange != null)
					resultRange.AddRanges(result);
				return result;
			}
			finally {
				context.PopSharedFormulaProcessing();
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		void IRangeCopySharedFormulaPart.RemoveCellsFromAffectedRange(SharedFormula sourceSharedFormula, CellRangeBase affectedRange) {
			try {
				SourceDataContext.PushSetValueShouldAffectSharedFormula(false);
				sourceSharedFormula.RemoveCellsFromAffectedRange(affectedRange);
			}
			finally {
				SourceDataContext.PopSetValueShouldAffectSharedFormula();
			}
		}
		protected abstract void ProcessSourceSharedFormula(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange);
		protected internal virtual SharedFormula CreateTargetSharedFormulaOrNull(SharedFormula sourceSharedFormula, CellRange targetSharedFormulaRange, CellRange _currentTargetRange) {
			ParsedExpression sourceExpression = sourceSharedFormula.Expression; 
			ParsedExpression targetFormula = ConvertParsedExpressionWithShiftReferencesOrNull(sourceExpression, _currentTargetRange);
			if (targetFormula == null)
				return null;
			SharedFormula result = new SharedFormula(targetFormula, targetSharedFormulaRange);
			return result;
		}
		protected abstract bool CopyCellSharedFormulaCore(ICell source, FormulaBase formula, ICell target);
		#endregion
		protected abstract bool IsSourceObjectRangeInterserctsWithCurrentTargetRange(CellRangeBase sourceObjectRange, CellRange _currentTargetRange);
		#region ArrayFormula
		protected abstract void CopyCellArrayFormulaCore(CellRange sourceCellRange, FormulaBase formula, ICell target, CellRange currentTargetRange);
		protected abstract IEnumerable<CellRange> GetInnerSourceArrayFormulaRanges();
		void IRangeCopyArrayFormulaPart.RemoveSourceArrayFormula(CellRange sourceArrayFormulaRange) {
			int index = SourceWorksheet.ArrayFormulaRanges.IndexOf(sourceArrayFormulaRange);
			SourceWorksheet.RemoveArrayFormulaAt(index); 
		}
		void IRangeCopyArrayFormulaPart.RemoveSourceArrayFormulaCut(CellRange sourceArrayFormulaRange) {
			TargetWorksheet.RemoveArrayFormulasAndValuesFrom(sourceArrayFormulaRange);
		}
		bool IRangeCopyArrayFormulaPart.ShouldCopyArrayFormulas { get { return PasteSpecialOptions.ShouldCopyFormulas; } }
		void IRangeCopyArrayFormulaPart.RetrieveSourceArrayFormula(ArrayFormula array) {
			ArrayFormulasForTargetWorksheetList.Add(array);
		}
		protected virtual void CopyArrayFormulaList(RangeCopyInfo rangeInfo) {
			if (!PasteSpecialOptions.ShouldCopyFormulas)
				return;
			foreach (CellRange sourceArrayFormulaRange in GetInnerSourceArrayFormulaRanges()) {
				ICell topLeftCell = sourceArrayFormulaRange.GetFirstCellUnsafe() as ICell;
				ArrayFormula sourceArrayFormula = topLeftCell.GetFormula() as ArrayFormula;
				ProcessSourceArrayFormula(rangeInfo, sourceArrayFormula, sourceArrayFormulaRange);
			}
		}
		protected internal abstract void ProcessSourceArrayFormula(RangeCopyInfo rangeInfo, ArrayFormula sourceArrayFormula, CellRange sourceArrayFormulaRange);
		void IRangeCopyArrayFormulaPart.CreateTargetArrayFormula(ArrayFormula sourceArrayFormula, CellRange targetBigRange) {
			CellRange sourceArrayFormulaRange = sourceArrayFormula.Range;
			CellPositionOffset finalArrayFormulaOffset = GetArrayFormulaExpressionReferencesOffset(sourceArrayFormulaRange, targetBigRange); 
			ParsedExpression targetArrayFormulaExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceArrayFormula.Expression, finalArrayFormulaOffset);
			if (targetArrayFormulaExpression == null)
				Exceptions.ThrowInternalException();
			CellRange targetArrayFormulaRange = GetTargetObjectRange(sourceArrayFormulaRange, targetBigRange) as CellRange;
			ArrayFormula targetArrayFormula = new ArrayFormula(targetArrayFormulaRange, targetArrayFormulaExpression);
			this.ArrayFormulasForTargetWorksheetList.Add(targetArrayFormula);
		}
		protected internal virtual CellPositionOffset GetArrayFormulaExpressionReferencesOffset(CellRange topLeftCellArrayFromulaRange, CellRange currentTargetRange) {
			CellPositionOffset currentTargetSourceRangeOffset = GetTargetRangeOffset(currentTargetRange); 
			CellPositionOffset offsetFromTopLeftArrayFormulaCellToTopLeftSourceRangeCell =
					topLeftCellArrayFromulaRange.GetCellPositionOffset(SourceRange.TopLeft); 
			int correctedColumnOffset = offsetFromTopLeftArrayFormulaCellToTopLeftSourceRangeCell.ColumnOffset;
			int correctedRowOffset = offsetFromTopLeftArrayFormulaCellToTopLeftSourceRangeCell.RowOffset;
			if (SourceRange.LeftColumnIndex < topLeftCellArrayFromulaRange.LeftColumnIndex)
				correctedColumnOffset = Math.Max(0, correctedColumnOffset);
			if (SourceRange.TopRowIndex < topLeftCellArrayFromulaRange.TopRowIndex)
				correctedRowOffset = Math.Max(0, correctedRowOffset);
			int columnOffset = currentTargetSourceRangeOffset.ColumnOffset + correctedColumnOffset;
			int rowOffset = currentTargetSourceRangeOffset.RowOffset + correctedRowOffset;
			CellPositionOffset result = new CellPositionOffset(columnOffset, rowOffset);
			return result;
		}
		protected void LocateArrayFormulaToCells() {
			foreach (ArrayFormula item in arrayFormulasForTargetWorksheetList) {
				CellRange targetArrayFormulaRange = item.Range;
				InsertWorksheetArrayFormulaCommand.ExecuteWithoutChecks(item, targetArrayFormulaRange);
			}
		}
		#endregion
		protected abstract internal void CopyRows(RangeCopyInfo rangeCopyInfo);
		protected void CopyRowProperties(Row sourceRow, Row targetNewRow, int defaultSourceFormatIndex) {
			targetNewRow.CopyFrom(sourceRow, defaultSourceFormatIndex);
			ValidateTargetRowFormatIndex(targetNewRow);
		}
		protected void ValidateTargetRowFormatIndex(Row targetNewRow) {
			if (!IsEqualWorkbook && targetNewRow.FormatIndex < DefaultTargetCellFormatIndex) {
				targetNewRow.ChangeIndexCore(Row.CellFormatIndexAccessor, DefaultTargetCellFormatIndex, targetNewRow.GetBatchUpdateChangeActions());
			}
		}
		protected virtual void CopyColumns(RangeCopyInfo rangeCopyInfo) {
			foreach (Column sourceColumn in GetSourceColumns()) {
				Column targetColumn = CreateTargetColumn(sourceColumn);
				if (targetColumn == null) {
					System.Diagnostics.Debug.Assert(targetColumn != null);
					continue;
				}
				CopyColumnProperties(sourceColumn, targetColumn);
			}
		}
		protected abstract IEnumerable<Column> GetSourceColumns();
		protected abstract Column CreateTargetColumn(Column sourceColumn);
		protected void CopyColumnProperties(Column sourceColumn, Column targetColumn) {
			if (PasteSpecialOptions.ShouldCopyColumnWidths) {
				targetColumn.BeginUpdate();
				try {
					ColumnBatchUpdateHelper buh = targetColumn.BatchUpdateHelper;
					buh.Info = sourceColumn.Info;
					buh.WidthInfo = sourceColumn.WidthInfo;
				}
				finally {
					targetColumn.EndUpdate();
				}
			}
			else {
				targetColumn.CopyFrom(sourceColumn);
				ValidateColumnCellFormatIndex(targetColumn);
			}
		}
		protected void ValidateColumnCellFormatIndex(Column targetColumn) {
			if (!IsEqualWorkbook && targetColumn.FormatIndex < DefaultTargetCellFormatIndex) {
				targetColumn.ChangeIndexCore(Column.CellFormatIndexAccessor, DefaultTargetCellFormatIndex, targetColumn.GetBatchUpdateChangeActions());
			}
		}
		#region MergedCell
		protected abstract IEnumerable<CellRange> GetMergedCellRanges();
		protected void CopyMergedCells(RangeCopyInfo rangeInfo) {
			bool shouldCopyMergedCells = PasteSpecialOptions.ShouldCopyOtherFormats;
			if (!shouldCopyMergedCells)
				return;
			IEnumerable<CellRange> mergedRanges = GetMergedCellRanges();
			foreach (CellRange sourceMergedRange in mergedRanges) {
				ProcessSourceMergedRange(rangeInfo, sourceMergedRange);
			}
		}
		protected abstract void ProcessSourceMergedRange(RangeCopyInfo rangeInfo, CellRange sourceMergedRange);
		void IRangeCopyMergedRangePart.CreateTargetMergedRange(CellRange targetRangeToMerge) {
			TargetWorksheet.MergedCells.Add(targetRangeToMerge);
		}
		#endregion
		#region Table
		protected abstract internal IEnumerable<Table> GetSourceTables(bool includeTablesIntersectedSourceRange);
		protected abstract internal List<Table> GetSourceTablesFromIntersectionSourceTargetRange(CellRange intersection);
		bool IRangeCopyTablePart.CanInsert(CellRange targetObjectRange, NotificationChecks flags, bool checkOnlyTables) {
			if (checkOnlyTables)
				return !TargetWorksheet.Tables.ContainsItemsInRange(targetObjectRange, true);
			bool result = TargetDataContext.Workbook.CanRangeInsert(targetObjectRange,
					   InsertCellMode.ShiftCellsDown, InsertCellsFormatMode.ClearFormat, flags) == null
				&& !TargetWorksheet.Tables.ContainsItemsInRange(targetObjectRange, true);
			return result;
		}
		protected internal void CopyTables(RangeCopyInfo rangeCopyInfo) {
			bool shouldCopyTables = (PasteSpecialOptions.ShouldCopyOtherFormats && PasteSpecialOptions.ShouldCopyValues) || PasteSpecialOptions.ShouldCopyTables;
			if (!shouldCopyTables) 
				return;
			WorkbookTarget.BeginUpdate();
			WorkbookTarget.StyleSheet.TableStyles.BeginUpdate();
			try {
				bool includeTablesIntersectedSourceRange = true;
				IEnumerable<Table> sourceTables = GetSourceTables(includeTablesIntersectedSourceRange); 
				List<String> existingTableNames = WorkbookTarget.GetTableNames();
				if (!rangeCopyInfo.SourceAndTargetEquals &&
					rangeCopyInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
					CellRange intersectionTargetSourceRange = rangeCopyInfo.SourceRange.Intersection(rangeCopyInfo.TargetBigRange);
					if (intersectionTargetSourceRange != null) {
						List<Table> tablesFromSourceTargetIntersection = GetSourceTablesFromIntersectionSourceTargetRange(intersectionTargetSourceRange);
						tablesFromSourceTargetIntersection.ForEach(table => SourceWorksheet.RemoveTable(table));
					}
				}
				foreach (Table sourceTable in sourceTables) {
					ProcessSourceTable(rangeCopyInfo, sourceTable, existingTableNames);
				}
			}
			finally {
				WorkbookTarget.StyleSheet.TableStyles.EndUpdate();
				WorkbookTarget.EndUpdate(); 
			}
		}
		protected virtual TableProcessorForRangeCopyOperation CreateTableRangeCalculator() {
			return new TableProcessorForRangeCopyOperation(this, this);
		}
		protected virtual PivotTableProcessorForRangeCopyOperation CreatePivotTableRangeCalculator() {
			return new PivotTableProcessorForRangeCopyOperation(this, this);
		}
		protected abstract void ProcessSourceTable(RangeCopyInfo rangeCopyInfo, Table sourceTable, List<string> existingTableNames);
		void IRangeCopyPivotTablePart.CreateTargetPivotTable(PivotTable sourcePivotTable, CellRange newRange, IList<string> existingPivotTableNames, CellRange currentTargetRange) {
			string targetPivotTableName = GetTargetPivotTableName(sourcePivotTable.Name, existingPivotTableNames);
			PivotTable pivot = CreateTargetPivotTableWithAdd(sourcePivotTable, newRange, targetPivotTableName, currentTargetRange);
			existingPivotTableNames.Add(targetPivotTableName);
		}
		void IRangeCopyPivotTablePart.ChangeExistingPivotTableRange(PivotTable sourceTable, CellRange resizedRange) {
			sourceTable.Location.SetRange(resizedRange, sourceTable);
		}
		bool IRangeCopyPivotTablePart.CanInsert(CellRange targetObjectRange, NotificationChecks flags, bool checkOnlyTables) {
			return (this as IRangeCopyTablePart).CanInsert(targetObjectRange, flags, checkOnlyTables);
		}
		void IRangeCopyTablePart.CreateTargetTable(Table sourceTable, CellRange newRange, List<string> existingTableNames, CellRange currentTargetRange) {
			string targetTableName = GetTargetTableName(sourceTable.Name, existingTableNames);
			Table targetTable = CreateTargetTableNotAdd(sourceTable, newRange, targetTableName, currentTargetRange);
			TargetWorksheet.Tables.Add(targetTable);
			existingTableNames.Add(targetTableName);
		}
		void IRangeCopyTablePart.RemoveTableAndUpdateFormulaReferences(Table sourceTable) {
			SourceWorksheet.RemoveTable(sourceTable);
		}
		void IRangeCopyTablePart.ResizeExistingTableRange(Table sourceTable, int additionalRows) {
			CellRange resizedRange = sourceTable.Range.GetResized(0, 0, 0, additionalRows);
			sourceTable.Range = resizedRange;
		}
		void IRangeCopyTablePart.ChangeExistingTableRange(Table sourceTable, CellRange resizedRange) {
			sourceTable.Range = resizedRange;
		}
		void IRangeCopyTablePart.RetrieveSourceTableBackInCollection(Table sourceTable) {
			SourceWorksheet.Tables.Add(sourceTable);
		}
		PivotTable CreateTargetPivotTableWithAdd(PivotTable sourcePivotTable, CellRange targetPivotTableRange, string targetPivotTableName, CellRange currentTargetRange) {
			PivotCache sourceCache = sourcePivotTable.Cache;
			PivotCache targetCache = CopyPivotCache(sourceCache, currentTargetRange);
			PivotTable targetPivotTable = new PivotTable(WorkbookTarget, targetPivotTableName, new PivotTableLocation(targetPivotTableRange), targetCache);
			CopyTableStyle(sourcePivotTable.StyleInfo.StyleName);
			CellPositionOffset offset = GetTargetRangeOffset(currentTargetRange);
			targetPivotTable.CopyFromNoHistory(sourcePivotTable, offset);
			TargetWorksheet.PivotTables.Add(targetPivotTable);
			return targetPivotTable;
		}
		PivotCache CopyPivotCache(PivotCache sourceCache, CellRange currentTargetRange) {
			if (IsEqualWorkbook)
				return sourceCache;
			IPivotCacheSource sourcePivotCacheSource = sourceCache.Source;
			IPivotCacheSource targetPivotCacheSource = CopyPivotCacheSource(sourcePivotCacheSource);
			PivotCache targetCache = WorkbookTarget.PivotCaches.TryGetPivotCache(targetPivotCacheSource);
			if (targetCache != null)
				return targetCache;
			targetCache = new PivotCache(WorkbookTarget, targetPivotCacheSource); 
			targetCache.CopyFrom(sourceCache, TargetWorksheet, GetTargetRangeOffset(currentTargetRange));
			WorkbookTarget.PivotCaches.Add(targetCache);
			return targetCache;
		}
		public virtual IPivotCacheSource CopyPivotCacheSource(IPivotCacheSource sourcePivotCacheSource) {
			PivotCacheSourceWorksheet sourceCacheSourceWorksheet = sourcePivotCacheSource as PivotCacheSourceWorksheet;
			if (sourceCacheSourceWorksheet == null)
				Exceptions.ThrowInvalidOperationException("that pivot cache source type is not supported yet");
			CellRange sourceCacheSourceRange = sourceCacheSourceWorksheet.GetRange(WorkbookSource.DataContext);
			ParsedExpression sourceExpression = sourceCacheSourceWorksheet.Expression.Clone();
			CellRange currentTargetRange = this.RangesInfo.First.TargetBigRange;
			string sourceCacheSourceWorksheetName = sourceCacheSourceRange.Worksheet.Name;
			ParsedExpression targetExpression = null;
			bool replaceExpressionToError = IsSheetWillBeExternalSheet(sourceCacheSourceWorksheetName);
			if (!replaceExpressionToError && !SourceRange.Includes(sourceCacheSourceRange) && !IsCopyWorksheetOperation)
				replaceExpressionToError = true;
			if (replaceExpressionToError) {
				targetExpression = new ParsedExpression() { Capacity = 1 };
				targetExpression.Add(new ParsedThingRefErr());
			}
			else {
				var offset = GetTargetRangeOffset(currentTargetRange);
				targetExpression = ModifyFormulaExpressionWithReferenceThingsOffsetRPNVisitor(sourceExpression, offset);
			}
			PivotCacheSourceWorksheet targetCacheSource = PivotCacheSourceWorksheet.CreateInstanceCore(targetExpression);
			targetCacheSource.SetSourceCore(targetExpression);
			return targetCacheSource;
		}
		Table CreateTargetTableNotAdd(Table sourceTable, CellRange targetTableRange, string targetTableName, CellRange currentTargetRange) {
			Table targetTable = new Table(TargetWorksheet, targetTableName, targetTableRange, sourceTable.HasHeadersRow);
			System.Diagnostics.Debug.Assert(targetTable.Columns.Count == 0);
			foreach (TableColumn sourceColumn in sourceTable.Columns) {
				TableColumn targetColumn = new TableColumn(targetTable, sourceColumn.Name);
				targetTable.Columns.Add(targetColumn);
			}
			CopyTable(sourceTable, targetTable, currentTargetRange);
			return targetTable;
		}
		void CopyTable(Table sourceTable, Table targetTable, CellRange currentTargetRange) {
			bool differentSourceHasTotalsRowBetweenDefaultValue = targetTable.HasTotalsRow != sourceTable.HasTotalsRow;
			(targetTable as DevExpress.Office.MultiIndexObject<Table, DocumentModelChangeActions>).CopyFrom(sourceTable);
			string styleName = targetTable.TableInfo.StyleName;
			CopyTableStyle(styleName);
			if (differentSourceHasTotalsRowBetweenDefaultValue)
				targetTable.SetHasTotalsRow(sourceTable.HasTotalsRow);
			TableColumnInfoCollection targetColumns = targetTable.Columns;
			TableColumnInfoCollection sourceColumns = sourceTable.Columns;
			int sourceColumnsCount = sourceColumns.Count;
			System.Diagnostics.Debug.Assert(targetColumns.Count == sourceColumnsCount, "Assumed collections are same sized", "");
			for (int sourceColumnIndex = 0; sourceColumnIndex < sourceColumnsCount; sourceColumnIndex++) {
				TableColumn sourceColumn = sourceColumns[sourceColumnIndex];
				TableColumn targetColumn = targetColumns[sourceColumnIndex];
				CopyTableColumn(sourceColumn, targetColumn);
			}
			targetTable.ChangeHeaders(sourceTable.HasHeadersRow, sourceTable.HasAutoFilter, sourceTable.ShowAutoFilterButton, ErrorHandler);
			if (targetTable.HasTotalsRow && targetTable.HasAutoFilter)
				targetTable.AutoFilter.OnTableRangeSetting(targetTable.Range, true);
			CopySortState(sourceTable.AutoFilter.SortState, targetTable.AutoFilter.SortState, currentTargetRange);
		}
		void CopyTableStyle(string styleName) {
			if (String.IsNullOrEmpty(styleName))
				return;
			if (!TableStyleName.CheckDefaultStyleName(styleName) && !Object.ReferenceEquals(WorkbookTarget, WorkbookSource)) {
				TableStyleCollection targetTableStyles = WorkbookTarget.StyleSheet.TableStyles;
				bool isStyleExists = targetTableStyles.ContainsStyleName(styleName);
				if (!isStyleExists) {
					TableStyle targetTableStyle = TableStyle.CreateTableStyle(WorkbookTarget, styleName);
					targetTableStyles.AddCore(targetTableStyle);
				}
			}
		}
		protected void CopySortState(SortState source, SortState target, CellRange currentTargetRange) {
			target.CopyFromCore(source, SetSortRange, AddSortCondition, currentTargetRange);
		}
		void SetSortRange(SortState sortState, CellRange sourceSourceRange, CellRange currentTargetRange) {
			if (sourceSourceRange != null)
				sortState.SortRange = GetTargetObjectRange(sourceSourceRange, currentTargetRange) as CellRange;
		}
		void AddSortCondition(SortConditionCollection collection, SortCondition item, CellRange currentTargetRange) {
			CellRange targetRange = GetTargetObjectRange(item.SortReference, currentTargetRange) as CellRange;
			SortCondition targetItem = new SortCondition(targetRange.Worksheet as Worksheet, targetRange);
			targetItem.CopyFrom(item);
			collection.Add(targetItem);
		}
		public void CopyTableColumn(TableColumn sourceColumn, TableColumn targetColumn) {
			targetColumn.DocumentModel.BeginUpdate();
			try {
				(targetColumn as DevExpress.Office.MultiIndexObject<TableColumn, DocumentModelChangeActions>).CopyFrom(sourceColumn);
				var historyItem = new ChangeTableColumnNameHistoryItem(targetColumn, targetColumn.Name, sourceColumn.Name);
				WorkbookTarget.History.Add(historyItem);
				historyItem.Execute();
				targetColumn.UniqueName = sourceColumn.UniqueName;
				targetColumn.QueryTableFieldId = sourceColumn.QueryTableFieldId;
				targetColumn.XmlProperties = sourceColumn.XmlProperties;
				string targetTableName = targetColumn.Table.Name;
				bool shouldCopyFormulas = PasteSpecialOptions.ShouldCopyFormulas;
				TotalsRowFunctionType sourceTotalsRowFunction = sourceColumn.TotalsRowFunction;
				if (shouldCopyFormulas) {
					if (sourceTotalsRowFunction == TotalsRowFunctionType.None) {
						targetColumn.TotalsRowLabel = sourceColumn.TotalsRowLabel;
						targetColumn.SetTotalsRowFunctionTransacted(sourceTotalsRowFunction);
					}
					else if (sourceTotalsRowFunction == TotalsRowFunctionType.Custom) {
						Formula targetTotalsRowFormula = CopyColumnFormula(sourceColumn.TotalsRowFormulaProvider, targetTableName);
						targetColumn.SetTotalsRowFormula(targetTotalsRowFormula);
						targetColumn.SetTotalsRowFunctionTransacted(sourceTotalsRowFunction);
					}
					else
						targetColumn.TotalsRowFunction = sourceTotalsRowFunction;
					if (sourceColumn.HasColumnFormula) {
						Formula targetColumnFormula = CopyColumnFormula(sourceColumn.ColumnFormulaProvider, targetTableName);
						targetColumn.ColumnFormulaProvider.SetFormula(targetColumnFormula);
					}
				}
			}
			finally {
				targetColumn.DocumentModel.EndUpdate();
			}
		}
		Formula CopyColumnFormula(TableFormulaProvider formulaProvider, string targetTableName) {
			Formula formula = formulaProvider.Formula;
			if (formula == null)
				return null;
			CopyTableColumnFormulaDiffWorkseetRPNVisitor visitor = new CopyTableColumnFormulaDiffWorkseetRPNVisitor(SourceWorksheet, TargetWorksheet);
			visitor.NewTableName = targetTableName;
			ParsedExpression resultExpression = visitor.Process(formula.Expression);
			Formula result;
			ArrayFormula sourceArrayFormula = formula as ArrayFormula;
			if (sourceArrayFormula != null) {
				CellRange targetArrayFormulaRange = (CellRange)sourceArrayFormula.Range.Clone(TargetWorksheet);
				result = new ArrayFormula(targetArrayFormulaRange, resultExpression);
			}
			else
				result = new Formula(null, resultExpression);
			return result;
		}
		protected virtual string GetTargetTableName(string sourceTableName, List<string> existingTableNamesInWorkbook) {
			return WorkbookTarget.GetService<DevExpress.XtraSpreadsheet.Services.ITableNameCreationService>().GetNewNameForTableCopy(existingTableNamesInWorkbook, sourceTableName);
		}
		protected virtual string GetTargetPivotTableName(string sourceName, IList<string> existingPivotTableNames) {
			if (existingPivotTableNames.Count == 0)
				return sourceName;
			Services.IPivotTableNameCreationService service = WorkbookTarget.GetService<Services.IPivotTableNameCreationService>();
			return service.GetNewName(existingPivotTableNames);
		}
		#endregion
		void CopyCharts() {
		}
		protected abstract IEnumerable<Comment> GetCommentsForCopy();
		protected void CopyComments(RangeCopyInfo rangeInfo) {
			if (!PasteSpecialOptions.ShouldCopyComments)
				return;
			if (rangeInfo.SourceAndTargetEquals)
				return;
			IEnumerable<Comment> commentsForCopy = GetCommentsForCopy();
			bool intersectionExists = !rangeInfo.SourceAndTargetEquals && rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals;
			if (intersectionExists) {
				CellRange intersectionTargetSourceRange = rangeInfo.SourceRange.Intersection(rangeInfo.TargetBigRange);
				if (intersectionTargetSourceRange != null) {
					SourceWorksheet.ClearComments(intersectionTargetSourceRange);
				}
			}
			foreach (Comment sourceComment in commentsForCopy) {
				CellRange sourceCommentRange = new CellRange(SourceWorksheet, sourceComment.Reference, sourceComment.Reference);
				ProcessSourceComment(rangeInfo, sourceComment, sourceCommentRange);
			}
		}
		void IRangeCopyCommentPart.RetrieveSourceComment(Comment comment) {
			SourceWorksheet.Comments.Add(comment);
		}
		void IRangeCopyCommentPart.MoveComment(Comment comment, CellPosition targetCommentPosition) {
			comment.Reference = targetCommentPosition;
		}
		void IRangeCopyCommentPart.CreateTargetComment(Comment sourceComment, CellPosition targetCommentPosition) {
			VmlShape targetShape = new VmlShape(TargetWorksheet);
			targetShape.CopyFrom(sourceComment.Shape);
			int targetShapeId = TargetWorksheet.VmlDrawing.Shapes.RegisterShape(targetShape);
			Comment targetComment = new Comment(TargetWorksheet, targetShapeId);
			targetComment.CopyFrom(sourceComment);
			targetComment.SetAuthorIdCore(GetCommentAuthorId(sourceComment));
			targetComment.Reference = targetCommentPosition;
			TargetWorksheet.AddComment(targetComment); 
		}
		protected virtual void ProcessSourceComment(RangeCopyInfo rangeInfo, Comment sourceComment, CellRange sourceCommentRange) {
			foreach (Model.CellRange currentTargetRange in rangeInfo.TargetRanges) {
				CellPosition targetCommentPosition = GetTargetObjectRange(sourceCommentRange, currentTargetRange).TopLeft;
				if (IsEqualWorksheets && sourceCommentRange.TopLeft.EqualsPosition(targetCommentPosition))
					continue;
				(this as IRangeCopyCommentPart).CreateTargetComment(sourceComment, targetCommentPosition);
			}
		}
		int GetCommentAuthorId(Comment sourceComment) {
			if (IsEqualWorksheets)
				return sourceComment.AuthorId;
			string author = SourceWorksheet.Workbook.CommentAuthors[sourceComment.AuthorId];
			CommentAuthorCollection targetAuthorCollection = TargetWorksheet.Workbook.CommentAuthors;
			int result = targetAuthorCollection.AddIfNotPresent(author);
			return result;
		}
		protected void RemoveProtectedRanges(RangeCopyInfo rangeInfo) {
			bool allowed = PasteSpecialOptions.ShouldCopyOtherFormats && !rangeInfo.SourceAndTargetEquals;
			if (!allowed)
				return;
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			RemoveProtectedRangesFromRangeNotIntersectedRangeBorders(targetBigRange);
		}
		protected void RemoveProtectedRangesFromRangeNotIntersectedRangeBorders(CellRangeBase range) {
			RemoveProtectedRangesFromRange(range, (protectedRange) => range.Includes(protectedRange.CellRange));
		}
		void RemoveProtectedRangesFromRange(CellRangeBase range, Predicate<ModelProtectedRange> match) {
			Worksheet worksheet = range.Worksheet as Worksheet;
			List<ModelProtectedRange> toDelete = worksheet.ProtectedRanges.FindAll(match);
			toDelete.ForEach(item => worksheet.ProtectedRanges.RemoveProtectedRangeTODO(item));
		}
		protected virtual void CopyProtectedRanges(RangeCopyInfo rangeInfo) {
			bool shouldCopyMergedCells = PasteSpecialOptions.ShouldCopyOtherFormats;
			if (!shouldCopyMergedCells)
				return;
			IEnumerable<ModelProtectedRange> protectedRanges = GetSourceProtectedRanges();
			if (!rangeInfo.SourceAndTargetEquals && rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
				CellRange intersectionTargetSourceRange = rangeInfo.SourceRange.Intersection(rangeInfo.TargetBigRange);
				if (intersectionTargetSourceRange != null) {
				}
			}
			foreach (ModelProtectedRange sourceProtectedRange in protectedRanges) {
				ProcessSourceProtectedRange(rangeInfo, sourceProtectedRange);
			}
		}
		protected abstract IEnumerable<ModelProtectedRange> GetSourceProtectedRanges();
		protected virtual void ProcessSourceProtectedRange(RangeCopyInfo rangeInfo, ModelProtectedRange sourceProtectedRange) {
		}
		protected void CreateTargetProtectedRange(ModelProtectedRange sourceProtectedRange, CellRangeBase targetObjectRange) {
			ModelProtectedRange targetNewProtectedRange = new ModelProtectedRange(sourceProtectedRange.Name, targetObjectRange);
			targetNewProtectedRange.CopyFrom(sourceProtectedRange);
			TargetWorksheet.ProtectedRanges.Add(targetNewProtectedRange);
		}
		#region DataValidations
		protected virtual List<DataValidation> GetSourceDataValidations() {
			return SourceWorksheet.DataValidations.LookupDataValidations(SourceRange);
		}
		protected virtual void CopyDataValidations(RangeCopyInfo rangeInfo) {
			if (!PasteSpecialOptions.ShouldCopyOtherFormats ||
				(IsEqualWorksheets && rangeInfo.SourceAndTargetEquals))
				return;
			List<DataValidation> sourceValidations = GetSourceDataValidations();
			if (!CutMode)
				ClearDataValidationRange(rangeInfo.TargetBigRange, sourceValidations);
			foreach (DataValidation sourceValidation in sourceValidations)
				CopyDataValidationsCore(sourceValidation, rangeInfo);
		}
		void ClearDataValidationRange(CellRange targetBigRange, List<DataValidation> sourceValidations) {
			DataValidationCollection targetCollection = TargetWorksheet.DataValidations;
			if (!IsEqualWorksheets || sourceValidations.Count == 0)
				targetCollection.ClearRange(targetBigRange);
			else
				targetCollection.ClearRange(targetBigRange, sourceValidations);
		}
		protected virtual void CopyDataValidationsCore(DataValidation sourceValidaiton, RangeCopyInfo rangeInfo) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			VariantValue sourceValidationRange = sourceValidaiton.CellRange.IntersectionWith(SourceRange);
			System.Diagnostics.Debug.Assert(sourceValidationRange.IsCellRange);
			CellRangeBase targetRange = GetTargetObjectRange(sourceValidationRange.CellRangeValue, targetBigRange);
			if (IsEqualWorksheets)
				sourceValidaiton.ProcessCopiedRange(targetRange, targetBigRange.ExcludeRange(targetRange));
			else
				ProcessTargetDataValidation(sourceValidaiton.CloneTo(targetRange), CellPositionOffset.None);
		}
		protected void ProcessTargetDataValidation(DataValidation targetValidation, CellPositionOffset offset) {
			WorkbookSource.DataContext.PushSharedFormulaProcessing(true);
			WorkbookTarget.DataContext.PushSharedFormulaProcessing(true);
			try {
				if (IsEqualWorkbook) {
					if (CutMode && IsEqualWorksheets)
						ProcessDataValidationExpressions(targetValidation, offset);
					else
						RenameDataValidationSheetDefinition(targetValidation);
				}
				else {
					walker.ShouldReplacePtg3dWithError = true;
					ProcessDataValidationExpressions(targetValidation, CellPositionOffset.None);
					walker.ShouldReplacePtg3dWithError = false;
				}
			}
			finally {
				WorkbookSource.DataContext.PopSharedFormulaProcessing();
				WorkbookTarget.DataContext.PopSharedFormulaProcessing();
			}
			TargetWorksheet.DataValidations.AddOrMergeWithExisting(targetValidation);
		}
		void RenameDataValidationSheetDefinition(DataValidation validation) {
			RenameSheetDefinitionFormulaWalker walker = new RenameSheetDefinitionFormulaWalker(SourceWorksheet.Name, TargetWorksheet.Name, DataContext);
			DataContext.PushCurrentWorkbook(WorkbookTarget);
			DataContext.PushCurrentWorksheet(TargetWorksheet);
			walker.PushCurrentSourceWorksheet(SourceWorksheet);
			try {
				validation.Expression1 = walker.Process(validation.Expression1);
				validation.Expression2 = walker.Process(validation.Expression2);
			}
			finally {
				walker.PopCurrentSourceWorksheet();
				DataContext.PopCurrentWorksheet();
				DataContext.PopCurrentWorkbook();
			}
		}
		void ProcessDataValidationExpressions(DataValidation validation, CellPositionOffset offset) {
			ParsedExpression expression1 = ConvertParsedExpressionWithShiftReferencesOrNull(validation.Expression1, offset);
			ParsedExpression expression2 = ConvertParsedExpressionWithShiftReferencesOrNull(validation.Expression2, offset);
			if (expression1 != null && expression1.Count != 0)
				validation.Expression1 = expression1;
			if (expression2 != null && expression1.Count != 0)
				validation.Expression2 = expression2;
		}
		#endregion
		protected abstract IEnumerable<ConditionalFormatting> GetConditionalFormattingsForCopy(Worksheet worksheet);
		protected virtual void CopyConditionalFormattings(RangeCopyInfo info) {
			if (!PasteSpecialOptions.ShouldCopyOtherFormats)
				return;
			foreach (ConditionalFormatting sourceConditionalFormatting in GetConditionalFormattingsForCopy(SourceWorksheet)) {
				CellPosition topLeftCellInCFRange = GetTopLeftCellFromSourceObjectRange(sourceConditionalFormatting.CellRange);
				if (!topLeftCellInCFRange.IsValid)
					continue;
				SourceDataContext.PushSharedFormulaProcessing(true);
				SourceDataContext.PushCurrentCell(topLeftCellInCFRange);
				try {
					ProcessSourceConditionalFormatting(info, sourceConditionalFormatting);
					if (!CutMode && !SourceRange.Intersects(info.TargetBigRange) 
						&& sourceConditionalFormatting.CellRange.Intersects(info.TargetBigRange))
						SubtractSourceConditionalFormatttingRange(info, sourceConditionalFormatting);
				}
				finally {
					SourceDataContext.PopSharedFormulaProcessing();
					SourceDataContext.PopCurrentCell();
				}
			}
		}
		void SubtractSourceConditionalFormatttingRange(RangeCopyInfo info, ConditionalFormatting sourceConditionalFormatting) {
			CellRangeBase newRange = sourceConditionalFormatting.CellRange.ExcludeRange(info.TargetBigRange);
			System.Diagnostics.Debug.Assert(newRange != null);
			CellRangeBase merged = newRange;
			if (newRange.RangeType == CellRangeType.UnionRange)
				merged = info.TryConsolidateRangesInCellUnion(newRange);
			sourceConditionalFormatting.SetCellRange(merged);
		}
		protected virtual void ProcessSourceConditionalFormatting(RangeCopyInfo rangeInfo, ConditionalFormatting sourceConditionalFormatting) {
			CellRangeBase sourceCfRange = sourceConditionalFormatting.CellRange;
			CellRange allworksheetRange = rangeInfo.TargetBigRange;
			CellRangeBase targetCfRange = GetTargetObjectRange(sourceCfRange, allworksheetRange);
			(this as IRangeCopyConditionalFormattingPart).CreateTargetConditionalFormatting(allworksheetRange, targetCfRange, sourceConditionalFormatting);
		}
		public virtual void UseSourceConditionalFormatting(ConditionalFormatting sourceConditionalFormatting, CellRangeBase newRange, CellRange targetRange) {
			sourceConditionalFormatting.SetCellRange(newRange);
		}
		void IRangeCopyConditionalFormattingPart.CreateTargetConditionalFormatting(CellRange currentTargetRange, CellRangeBase targetCFrange, ConditionalFormatting sourceConditionalFormatting) {
			ConditionalFormatting targetConditionalFormatting = sourceConditionalFormatting.CreateInstance(TargetWorksheet);
			targetConditionalFormatting.CopyFrom(sourceConditionalFormatting);
			targetConditionalFormatting.SetCellRange(targetCFrange);
			ProcessFormulaConditionalFormatting(sourceConditionalFormatting, targetConditionalFormatting, currentTargetRange);
			TargetWorksheet.InsertConditionalFormattingWithHistoryAndNotification(targetConditionalFormatting); 
		}
		CellPosition GetTopLeftCellFromSourceObjectRange(CellRangeBase cellRange) {
			CellRangeBase sourceCFRange = cellRange;
			VariantValue intersectionWith = SourceRange.IntersectionWith(sourceCFRange);
			if (intersectionWith.IsError)
				return CellPosition.InvalidValue;
			CellPosition topLeftCellInCFRange = intersectionWith.CellRangeValue.TopLeft;
			return topLeftCellInCFRange;
		}
		protected void ProcessFormulaConditionalFormatting(ConditionalFormatting sourceConditionalFormatting, ConditionalFormatting targetConditionFormatting, CellRange currentTargetRange) {
			ExpressionFormulaConditionalFormatting sourceExprCf = sourceConditionalFormatting as ExpressionFormulaConditionalFormatting;
			ExpressionFormulaConditionalFormatting targetExprCf = targetConditionFormatting as ExpressionFormulaConditionalFormatting;
			if (sourceExprCf != null && targetExprCf != null) {
				ProcessFormulaContitionalFormattingExpressionComparer(targetConditionFormatting.CellRange, sourceExprCf, targetExprCf, currentTargetRange);
				return;
			}
			RangeFormulaConditionalFormatting sourceRangeComp = sourceConditionalFormatting as RangeFormulaConditionalFormatting;
			RangeFormulaConditionalFormatting targetRangeComp = targetConditionFormatting as RangeFormulaConditionalFormatting;
			if (sourceRangeComp != null && targetRangeComp != null) {
				ProcessFormulaConditionalFormattingRangeComparer(targetConditionFormatting.CellRange, sourceRangeComp, targetRangeComp, currentTargetRange);
				return;
			}
			TextFormulaConditionalFormatting sourceTextComparer = sourceConditionalFormatting as TextFormulaConditionalFormatting;
			TextFormulaConditionalFormatting targetTextComparer = targetConditionFormatting as TextFormulaConditionalFormatting;
			if (sourceTextComparer != null && targetTextComparer != null) {
				ProcessFormulaConditionFormattingTextComparer(targetConditionFormatting.CellRange, sourceTextComparer, targetTextComparer, currentTargetRange);
				return;
			}
		}
		void ProcessFormulaContitionalFormattingExpressionComparer(CellRangeBase targetConditionalFormattingRange, ExpressionFormulaConditionalFormatting sourceExprComp, ExpressionFormulaConditionalFormatting targetExprComp, CellRange currentTargetRange) {
			ParsedExpression sourceComparerParsedExpression = sourceExprComp.ValueExpression;
			ParsedExpression targetComparerParsedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceComparerParsedExpression, currentTargetRange);
			targetExprComp.ValueExpression = targetComparerParsedExpression;
		}
		void ProcessFormulaConditionalFormattingRangeComparer(CellRangeBase targetCFRange, RangeFormulaConditionalFormatting sourceRangeComparer, RangeFormulaConditionalFormatting targetRangeComparer, CellRange currentTargetRange) {
			ParsedExpression sourceComparerParsedExpression = sourceRangeComparer.ValueExpression;
			ParsedExpression targetComparerParsedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceComparerParsedExpression, currentTargetRange);
			targetRangeComparer.ValueExpression = targetComparerParsedExpression;
			sourceComparerParsedExpression = sourceRangeComparer.Value2Expression;
			targetComparerParsedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceComparerParsedExpression, currentTargetRange);
			targetRangeComparer.Value2Expression = targetComparerParsedExpression;
		}
		void ProcessFormulaConditionFormattingTextComparer(CellRangeBase targetCfRange, TextFormulaConditionalFormatting sourceTextComparer, TextFormulaConditionalFormatting targetTextComparer, CellRange currentTargetRange) {
			ParsedExpression sourceComparerParsedExpression = sourceTextComparer.ValueExpression;
			ParsedExpression targetComparerParsedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(sourceComparerParsedExpression, currentTargetRange);
			targetTextComparer.ValueExpression = targetComparerParsedExpression;
		}
		protected virtual void CopyVmlDrawing() {
		}
		protected virtual void CopyAutoFilter(RangeCopyInfo rangeInfo) {
		}
		protected void CopySparklines(RangeCopyInfo rangeCopyInfo) {
			bool shouldCopy = pasteSpecialOptions.ShouldCopyOtherFormats;
			if (!shouldCopy) 
				return;
			IEnumerable<SparklineGroup> sourceGroups = GetSourceSparklineGroups();
			foreach (SparklineGroup sourceGroup in sourceGroups)
				ProcessSourceSparklineGroup(rangeCopyInfo, sourceGroup);
		}
		protected abstract void ProcessSourceSparklineGroup(RangeCopyInfo rangeInfo, SparklineGroup sourceSparklineGroup);
		protected abstract IEnumerable<SparklineGroup> GetSourceSparklineGroups();
		void CopyIgnoredErrors(RangeCopyInfo rangeCopyInfo) {
		}
		protected void CopyPivotTables(RangeCopyInfo rangeCopyInfo) {
			bool shouldCopyPivots = (PasteSpecialOptions.ShouldCopyOtherFormats && PasteSpecialOptions.ShouldCopyValues) || PasteSpecialOptions.ShouldCopyTables;  
			if (!shouldCopyPivots) 
				return;
			WorkbookTarget.BeginUpdate();
			try {
				bool includeTablesIntersectedSourceRange = false; 
				IEnumerable<PivotTable> sourcePivotTables = GetSourcePivotTables(SourceRange, includeTablesIntersectedSourceRange); 
				IList<string> existingNames = TargetWorksheet.PivotTables.GetExistingNames();
				if (!rangeCopyInfo.SourceAndTargetEquals &&
					rangeCopyInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
					CellRange intersectionTargetSourceRange = rangeCopyInfo.SourceRange.Intersection(rangeCopyInfo.TargetBigRange);
					if (intersectionTargetSourceRange != null) {
					}
				}
				foreach (PivotTable sourcePivot in sourcePivotTables) {
					ProcessSourcePivotTable(rangeCopyInfo, sourcePivot, existingNames);
				}
			}
			finally {
				WorkbookTarget.EndUpdate(); 
			}
		}
		protected abstract internal IEnumerable<PivotTable> GetSourcePivotTables(CellRange sourceRange, bool includeTablesIntersectedSourceRange);
		protected abstract void ProcessSourcePivotTable(RangeCopyInfo rangeCopyInfo, PivotTable sourcePivotTable, IList<string> existingNames);
		#region Hyperlinks
		protected void CopyHyperlinks(RangeCopyInfo rangeInfo) {
			bool shouldCopyHyperlinks = pasteSpecialOptions.ShouldCopyOtherFormats;
			if (!shouldCopyHyperlinks)
				return;
			List<ModelHyperlink> sourceHyperlinks = GetSourceHyperlinks();
			if (!rangeInfo.SourceAndTargetEquals && rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
				CellRange intersectionTargetSourceRange = rangeInfo.SourceRange.Intersection(rangeInfo.TargetBigRange);
				if (intersectionTargetSourceRange != null)
					RemoveHyperlinksFromRangeNotIntersectedRangeBorders(intersectionTargetSourceRange);
			}
			foreach (ModelHyperlink sourceHyperlinkItem in sourceHyperlinks) {
				ProcessSourceHyperlink(rangeInfo, sourceHyperlinkItem);
			}
		}
		void IRangeCopyHyperlinkPart.RemoveSourceHyperlink(ModelHyperlink sourceHyperlink) {
			int indexOfSourceHyperlink = SourceWorksheet.Hyperlinks.IndexOf(sourceHyperlink);
			if (indexOfSourceHyperlink >= 0) {
				SourceWorksheet.RemoveHyperlinkAt(indexOfSourceHyperlink);
			}
		}
		void IRangeCopyHyperlinkPart.ModifySourceHyperlinkRange(ModelHyperlink sourceHyperlink, CellRange newRange) {
			sourceHyperlink.SetRange(newRange);
		}
		void IRangeCopyHyperlinkPart.RetrieveSourceHyperlinkRange(ModelHyperlink sourceHyperlink) {
			SourceWorksheet.Hyperlinks.Add(sourceHyperlink);
		}
		void IRangeCopyHyperlinkPart.CreateTargetHyperlink(CellRange targetHyperlinkRange, ModelHyperlink sourceHyperlink) {
			ModelHyperlink hyperlink = new ModelHyperlink(TargetWorksheet,
										   targetHyperlinkRange,
										   sourceHyperlink.TargetUri,
										   sourceHyperlink.IsExternal,
										   sourceHyperlink.DisplayText,
										   sourceHyperlink.TooltipText);
			TargetWorksheet.Hyperlinks.Add(hyperlink);
		}
		protected virtual void ProcessSourceHyperlink(RangeCopyInfo rangeInfo, ModelHyperlink sourceHyperlinkItem) {
			CellRange targetHyperlinkRange = GetTargetObjectRange(sourceHyperlinkItem.Range, rangeInfo.TargetBigRange) as CellRange;
			(this as IRangeCopyHyperlinkPart).CreateTargetHyperlink(targetHyperlinkRange, sourceHyperlinkItem);
		}
		protected abstract List<ModelHyperlink> GetSourceHyperlinks();
		#endregion
		#region CellContent
		protected internal void CopyCellContent(ICell cellSource, ICell cellTarget, CellRange currentTargetRange) {
			System.Diagnostics.Debug.Assert(!cellTarget.IsUpdateLocked);
			bool oldRestriction = WorkbookTarget.SuppressCellValueAssignment;
			WorkbookTarget.SuppressCellValueAssignment = false;
			DataContext.PushCurrentCell(cellTarget); 
			try {
				if (!CopyCellFormula(cellSource, cellTarget, currentTargetRange)) {
					VariantValue sourceValue = GetSourceValue(cellSource, !IsEqualWorkbook);
					bool shouldCellValueToTargetCell = false;
					if (PasteSpecialOptions.ShouldCopyFormulas || PasteSpecialOptions.ShouldCopyValues) {
						bool targetCellNotEmpty = cellTarget.HasFormula || cellTarget.Value != Model.VariantValue.Empty;
						bool targetCellEmpty = !targetCellNotEmpty;
						shouldCellValueToTargetCell = !(sourceValue.IsEmpty && targetCellEmpty);
					}
					if (shouldCellValueToTargetCell)
						SetValueToTargetCell(cellTarget, sourceValue);
				}
			}
			finally {
				DataContext.PopCurrentCell();
				WorkbookTarget.SuppressCellValueAssignment = oldRestriction;
			}
			CopyCellFormatAndStyle(cellSource, cellTarget);
		}
		static VariantValue GetSourceValue(ICell cellSource, bool isCopyBetweenDiffModels) {
			VariantValue sourceValue = VariantValue.Empty;
			if (cellSource != null && !(cellSource is FakeCell)) {
				sourceValue = cellSource.GetValue();
				if (sourceValue.IsSharedString && isCopyBetweenDiffModels)
					sourceValue = sourceValue.GetTextValue(cellSource.Context.StringTable);
			}
			return sourceValue;
		}
		bool CopyCellFormula(ICell source, ICell target, CellRange currentTargetRange) {
			bool formulaCopied = false;
			if (PasteSpecialOptions.ShouldCopyFormulas) {
				if (source != null && source.HasFormula && !(source is FakeCell))
					formulaCopied = CopyCellFormulaCore(source, target, currentTargetRange);
			}
			return formulaCopied;
		}
		bool CopyCellFormulaCore(ICell source, ICell target, CellRange currentTargetRange) {
			FormulaBase sourceFormula = source.GetFormula();
			bool result = false;
			bool isArrayFormula = sourceFormula is ArrayFormula || sourceFormula is ArrayFormulaPart;
			if (sourceFormula is Formula && !isArrayFormula)
				result = CopyCellNormalFormulaCore(source, sourceFormula, target, currentTargetRange);
			else if (isArrayFormula){
				CopyCellArrayFormulaCore(source.GetRange(), sourceFormula, target, currentTargetRange);
				result = true;
			}
			else if (sourceFormula is SharedFormulaRef)
				result = CopyCellSharedFormulaCore(source, sourceFormula, target);
			return result;
		}
		void SetValueToTargetCell(ICell target, VariantValue newValue) {
			System.Diagnostics.Debug.Assert(!(target is FakeCell), "setting value to fake cell", "");
			CellContentSnapshot snapshot = null;
			if (!(target.HasNoValue && newValue == VariantValue.Empty) && !suppressCellValueChanged)
				snapshot = new CellContentSnapshot(target);
			SetValueToTargetCellCore(target, newValue);
			if (snapshot != null)
				this.WorkbookTarget.InternalAPI.RaiseCellValueChanged(snapshot);
		}
		void SetValueToTargetCellCore(ICell target, VariantValue newValue) {
			FormattedVariantValue targetFormattedNewValue = FormattedVariantValue.Empty;
			NumberFormat targetActualNumberFormat = null;
			target.BeginUpdate();
			this.TargetDataContext.PushSetValueShouldAffectSharedFormula(false);
			try {
				if (shouldConvertSourceStringTextValueToFormattedValue && !newValue.IsEmpty) {
					targetActualNumberFormat = target.ActualFormat;
					if (!targetActualNumberFormat.IsText) {
						targetFormattedNewValue = ConvertSourceStringTextValueToTargetFormattedValue(target, newValue);
						newValue = targetFormattedNewValue.Value;
					}
				}
				if (DisabledHistory)
					target.SetValueCore(newValue);
				else
					(target as Cell).TransactedSetValueNoChecks(newValue);
				if (targetActualNumberFormat != null)
					SetDetectedNumberFormatToTargetCellIfTargetNumberFormatIsGeneric(target, targetFormattedNewValue, targetActualNumberFormat);
			}
			finally {
				this.TargetDataContext.PopSetValueShouldAffectSharedFormula(); 
				target.EndUpdate();
			}
		}
		void SetDetectedNumberFormatToTargetCellIfTargetNumberFormatIsGeneric(ICell target, FormattedVariantValue formattedNewValue, NumberFormat targetActualNumberFormat) {
			NumberFormat sourceNumberFormat = WorkbookTarget.StyleSheet.NumberFormats[formattedNewValue.NumberFormatId];
			if (targetActualNumberFormat.IsGeneric && !sourceNumberFormat.IsGeneric && !sourceNumberFormat.IsText)
				target.FormatString = sourceNumberFormat.FormatCode;
		}
		FormattedVariantValue ConvertSourceStringTextValueToTargetFormattedValue(ICell target, VariantValue newValue) {
			WorkbookDataContext sourceDataContext = WorkbookSource.DataContext;
			string sourceStringTextValue = newValue.GetTextValue(sourceDataContext.StringTable);
			if (sourceStringTextValue == null) {
				if (newValue.IsError)
					return new FormattedVariantValue(newValue, 0);
				else
					sourceStringTextValue = String.Empty;
			}
			WorkbookDataContext targetDataContext = WorkbookTarget.DataContext;
			FormattedVariantValue formattedValue = CellValueFormatter.GetValue(VariantValue.Empty, sourceStringTextValue, targetDataContext, true);
			return formattedValue;
		}
		void CopyCellFormatAndStyle(ICell source, ICell target) {
			bool shouldResetFormatting = PasteSpecialOptions.ShouldCopyFormatAndStyle;
			bool savedLocked = target.ActualProtection.Locked;
			if (source == null) {
				if (shouldResetFormatting) {
					target.ChangeFormatIndex(DocumentModel.StyleSheet.DefaultCellFormatIndex, target.GetBatchUpdateChangeActions());
					if (TargetWorksheet.IsProtected)
						target.Protection.Locked = savedLocked;
					return;
				}
			}
			FakeCell fakeSourceCell = source as FakeCell;
			bool shouldCopyPartialFormatting = PasteSpecialOptions.ShouldCopyOtherFormats
							|| PasteSpecialOptions.ShouldCopyStyle
							|| PasteSpecialOptions.ShouldCopyNumberFormat
							|| PasteSpecialOptions.ShouldCopyBorders;
			if (fakeSourceCell != null) {
				bool sameModels = Object.ReferenceEquals(WorkbookSource, WorkbookTarget);
				FakeCellFormatToCellFormatConverter converter = new FakeCellFormatToCellFormatConverter(PasteSpecialOptions, sameModels);
				converter.CopyActualFormattingFromFakeCellToCell(fakeSourceCell, target);
				if (TargetWorksheet.IsProtected)
					target.Protection.Locked = savedLocked;
			}
			else
				if (PasteSpecialOptions.ShouldCopyFormatAndStyle || shouldCopyPartialFormatting)
					CellCopyFormatDeffered(target, source);
		}
		public void CellCopyFormatDeffered(ICell target, ICell source) {
			Cell targetCellCasted = target as Cell;
			CellFormat sourceFormat = source.FormatInfo;
			CellFormat targetFormat = targetCellCasted.GetInfoForModification() as CellFormat;
			sourceFormat.BeginUpdate();
			targetFormat.BeginUpdate();
			try {
				CopyFormattingDeferred(targetFormat, sourceFormat);
			}
			finally {
				targetFormat.EndUpdate();
				sourceFormat.CancelUpdate();
			}
			targetCellCasted.ReplaceInfo(targetFormat, target.GetBatchUpdateChangeActions());
			ValidateCellFormatIndex(targetCellCasted);
		}
		void ValidateCellFormatIndex(Cell targetCellCasted) {
			if (!IsEqualWorkbook && targetCellCasted.FormatIndex < DefaultTargetCellFormatIndex) {
				targetCellCasted.ChangeFormatIndex(DefaultTargetCellFormatIndex, targetCellCasted.GetBatchUpdateChangeActions());
			}
		}
		public void CopyFormattingDeferred(CellFormat targetFormat, CellFormat obj) { 
			System.Diagnostics.Debug.Assert(targetFormat.IsUpdateLocked);
			FormatBase targetFormatOwner = targetFormat.GetOwner();
			FormatBase sourceFormatOwner = obj.GetOwner();
			if (PasteSpecialOptions.ShouldCopyBorders) {
				FormatBase.BorderIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				targetFormat.ApplyBorder = obj.ApplyBorder;
			}
			if (PasteSpecialOptions.ShouldCopyNumberFormat) {
				FormatBase.NumberFormatIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				targetFormat.ApplyNumberFormat = obj.ApplyNumberFormat;
			}
			if (PasteSpecialOptions.ShouldCopyStyle) {
				CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
			}
			if (PasteSpecialOptions.ShouldCopyOtherFormats) {
				FormatBase.FontIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				FormatBase.AlignmentIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				FormatBase.FillIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				FormatBase.GradientFillIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				bool savedLocked = targetFormatOwner.Protection.Locked;
				FormatBase.CellFormatFlagsIndexAccessor.CopyDeferredInfo(targetFormatOwner, sourceFormatOwner);
				if (TargetWorksheet.IsProtected)
					targetFormatOwner.Protection.Locked = savedLocked;
			}
			System.Diagnostics.Debug.Assert(8 == obj.IndexAccessorsCount); 
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			StyleInfoIndexAccessor styleInfoIndexAccessor = CellFormat.StyleInfoIndexAccessor;
			StyleInfo source = styleInfoIndexAccessor.GetInfo(from);
			CellStyleBase sourceStyle = from.DocumentModel.StyleSheet.CellStyles[source.StyleIndex];
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				StyleInfo result = new StyleInfo();
				result.StyleIndex = GetTargetStyleIndex(sourceStyle);
				styleInfoIndexAccessor.SetDeferredInfo(owner.BatchUpdateHelper, result);
				return;
			}
			styleInfoIndexAccessor.SetDeferredInfo(owner.BatchUpdateHelper, source);
		}
		int GetTargetStyleIndex(CellStyleBase sourceStyle) {
			System.Diagnostics.Debug.Assert(!IsEqualWorkbook);
			int result;
			if (styleTranslationTable.TryGetValue(sourceStyle.Name, out result))
				return result;
			if (!CopyCellStyles) {
				result = WorkbookTarget.StyleSheet.CellStyles.GetCellStyleIndexByName(sourceStyle.Name);
				styleTranslationTable.Add(sourceStyle.Name, result);
				return result;
			}
			CustomCellStyle customStyle = sourceStyle as CustomCellStyle;
			BuiltInCellStyle builtInStyle = sourceStyle as BuiltInCellStyle;
			if (customStyle != null)
				ConvertSourceBuiltInStyleToTargetCustomStyle(customStyle, targetStyleNames, styleTranslationTable, false);
			else if (builtInStyle != null && builtInStyle.CustomBuiltIn)
				ConvertSourceBuiltInStyleToTargetCustomStyle(builtInStyle, targetStyleNames, styleTranslationTable, true);
			else {
				result = WorkbookTarget.StyleSheet.CellStyles.GetCellStyleIndexByName(sourceStyle.Name);
				if (result < 0) {
					CellStyleBase newStyle = sourceStyle.Clone(WorkbookTarget, sourceStyle.Name);
					WorkbookTarget.StyleSheet.CellStyles.Add(newStyle);
					result = newStyle.StyleIndex;
				}
				styleTranslationTable.Add(sourceStyle.Name, result);
				return result;
			}
			if (styleTranslationTable.TryGetValue(sourceStyle.Name, out result))
				return result;
			throw new Exception("");
		}
		#endregion
		#region CopyStyles
		void ConvertSourceBuiltInStyleToTargetCustomStyle(CellStyleBase sourceStyle, HashSet<string> targetStyleNames, Dictionary<string, int> styleTranslationTable, bool sourceStyleWasBuiltIn) {
			string sourceCellStyleName = sourceStyle.Name;
			string newTargetCustomStyleName = String.Empty;
			bool targetContainsSameNameStyle = targetStyleNames.Contains(sourceCellStyleName);
			bool customStyleNotExistInTarget = !sourceStyleWasBuiltIn && !targetContainsSameNameStyle;
			if (customStyleNotExistInTarget) {
				targetStyleNames.Add(sourceCellStyleName);
				newTargetCustomStyleName = sourceCellStyleName;
			}
			if (targetContainsSameNameStyle) {
				CellStyleBase targetExistingCustomStyle = WorkbookTarget.StyleSheet.CellStyles.GetCellStyleByName(sourceCellStyleName);
				bool shouldUseExisitingTargetCustomStyle = sourceStyle.EqualsByFormatting(targetExistingCustomStyle);
				if (shouldUseExisitingTargetCustomStyle) {
					styleTranslationTable.Add(sourceCellStyleName, targetExistingCustomStyle.StyleIndex);
					return;
				}
			}
			if (!customStyleNotExistInTarget) {
				newTargetCustomStyleName = GetTargetStyleName(sourceStyle.Name, targetStyleNames);
				targetStyleNames.Add(newTargetCustomStyleName);
			}
			CellStyleBase targetStyle = new CustomCellStyle(WorkbookTarget, newTargetCustomStyleName);
			targetStyle.CopyFrom(sourceStyle);
			WorkbookTarget.StyleSheet.CellStyles.Add(targetStyle);
			styleTranslationTable.Add(sourceStyle.Name, targetStyle.StyleIndex);
		}
		string GetTargetStyleName(string sourceCellStyleName, HashSet<string> targetStyleNames) {
			DevExpress.XtraSpreadsheet.Services.ICellStyleNameCreationService service = WorkbookTarget.GetService<DevExpress.XtraSpreadsheet.Services.ICellStyleNameCreationService>();
			if (service == null)
				throw new InvalidOperationException("ICellStyleNameCreationService is null");
			return service.GetDuplicateStyleName(sourceCellStyleName, targetStyleNames.ToList());
		}
		HashSet<string> GetTargetWorkbookAllStyleNames() {
			HashSet<string> result = new HashSet<string>();
			foreach (CellStyleBase item in TargetWorksheet.Workbook.StyleSheet.CellStyles.GetStyles())
				result.Add(item.Name);
			return result;
		}
		#endregion
		#region Pictures
		protected virtual internal void CopyDrawingObjects(RangeCopyInfo rangeInfo) {
			bool shouldCopyDrawings = PasteSpecialOptions.ShouldCopyOtherFormats;
			if (!shouldCopyDrawings)
				return;
			foreach (IDrawingObject sourceDrawing in GetDrawingsForCopy()) {
				Picture sourcePicture = sourceDrawing as Picture;
				if (sourcePicture != null)
					ProcessSourcePicture(rangeInfo, sourcePicture);
				else {
					Chart sourceChart = sourceDrawing as Chart;
					if(sourceChart != null)
						ProcessChart(rangeInfo, sourceChart);
				}
			}
			}
		protected virtual void ProcessSourcePicture(RangeCopyInfo rangeInfo, Picture sourcePicture) {
			foreach (Model.CellRange currentTargetRange in rangeInfo.TargetRanges) {
				CellPositionOffset offset = GetTargetRangeOffset(currentTargetRange);
				(this as IRangeCopyPicturePart).CreateTargetPicture(sourcePicture, offset);
			}		  
		}
		protected virtual void ProcessChart(RangeCopyInfo rangeInfo, Chart sourceChart) {
			foreach (Model.CellRange currentTargetRange in rangeInfo.TargetRanges) {
				CellPositionOffset offset = GetTargetRangeOffset(currentTargetRange);
				(this as IRangeCopyChartPart).CreateTargetChart(sourceChart, offset);
		}
			}
		void IRangeCopyChartPart.CreateTargetChart(Chart sourceChart, CellPositionOffset offset) {
			Chart targetChart = new Chart(TargetWorksheet);
			targetChart.CopyFrom(sourceChart, offset);
			MoveAnchorAndOffsets(targetChart, offset);
			SeriesFormulaDataWalker walker = new SeriesFormulaDataWalker(ProcessDataReference, ProcessChartText);
			walker.Walk(targetChart);
			targetChart.UpdateDataReferences();
			TargetWorksheet.InsertChart(targetChart);
		}
		void ProcessChartText(IChartText text) {
			ChartTextRef chartTextRef = text as ChartTextRef;
			if (chartTextRef != null)
				ProcessChartFormulaData(chartTextRef.FormulaData);
		}
		void ProcessDataReference(IDataReference dataReference) {
			ChartDataReference chartDataReference = dataReference as ChartDataReference;
			if (chartDataReference != null)
				ProcessChartFormulaData(chartDataReference.FormulaData);
		}
		void ProcessChartFormulaData(FormulaData formulaData) {
			ParsedExpression sourceExpression = formulaData.Expression.Clone();
			WorkbookDataContext sourceDataContext = SourceWorksheet.DataContext;
			WorkbookDataContext targetDataContext = WorkbookTarget.DataContext;
			ParsedExpression newExpression = sourceExpression;
			if (!IsEqualWorkbook) {
				RenameSheetDefinitionFormulaWalker renameSheetDefinitionFormulaWalker = new RenameSheetDefinitionFormulaWalker(SourceWorksheet.Name, TargetWorksheet.Name, sourceDataContext, targetDataContext);
				newExpression = ModifyFormulaExpressionCore(sourceExpression, renameSheetDefinitionFormulaWalker);
			}
			else { 
				CellRange targetBigRange = this.RangesInfo.First.TargetBigRange;
				newExpression = ConvertParsedExpressionWithShiftReferencesOrNull(newExpression, targetBigRange);
			}
			formulaData.Expression = newExpression;
		}
		protected abstract IEnumerable<IDrawingObject> GetDrawingsForCopy();
		void IRangeCopyPicturePart.CreateTargetPicture(Picture sourcePicture, CellPositionOffset offset) {
			OfficeImage targetImage = GetCopyOfSourceOfficeImage(sourcePicture.Image);
			Picture targetPicture = null;
			if (sourcePicture.DrawingObject.GetResizingAnchorType() == AnchorType.OneCell)
				targetPicture = CreateTargetPictureOneCellAnchor(sourcePicture, targetImage, TargetWorksheet, offset);
			else if (sourcePicture.DrawingObject.GetResizingAnchorType() == AnchorType.TwoCell)
				targetPicture = CreateTargetPictureTwoCellAnchor(sourcePicture, targetImage, offset);
			else 
				targetPicture = CreateTargetPictureAbsoluteAnchor(sourcePicture, targetImage, TargetWorksheet, offset);
			targetPicture.Copy(sourcePicture);
		}
		void IRangeCopyPicturePart.RemoveSourcePicture(IDrawingObject sourcePicture) {
			RemoveSourceDrawing(sourcePicture);
		}
		void IRangeCopyChartPart.RemoveSourceChart(Chart sourceChart) {
			RemoveSourceDrawing(sourceChart);
		}
		void RemoveSourceDrawing(IDrawingObject obj) {
			SourceWorksheet.RemoveDrawing(obj); 
		}
		OfficeImage GetCopyOfSourceOfficeImage(OfficeImage image) {
			OfficeNativeImage nativeImage = image as OfficeNativeImage;
			if (nativeImage != null)
				return new OfficeReferenceImage(DocumentModel, nativeImage);
			else
				return image.Clone(DocumentModel);
		}
		CellKey GetShiftedCellKey(int sheetId, CellKey source, CellPositionOffset offset) {
			int targetWorksheetId = sheetId;
			int shiftedColumnIndex = source.ColumnIndex + offset.ColumnOffset;
			int shiftedRowIndex = source.RowIndex + offset.RowOffset;
			return new CellKey(targetWorksheetId, shiftedColumnIndex, shiftedRowIndex);
		}
		Picture CreateTargetPictureOneCellAnchor(Picture sourcePicture, OfficeImage targetImage, Worksheet targetWorksheet, CellPositionOffset offset) {
			CellKey sourcePictureFromCellKey = sourcePicture.From.CellKey;
			CellKey targetFrom = GetShiftedCellKey(targetWorksheet.SheetId, sourcePictureFromCellKey, offset);
			Picture result = targetWorksheet.InsertPicture(targetImage, targetFrom);
			result.From = new AnchorPoint(targetFrom, sourcePicture.From.ColOffset, sourcePicture.From.RowOffset);
			return result;
		}
		Picture CreateTargetPictureTwoCellAnchor(Picture sourcePicture, OfficeImage targetImage, CellPositionOffset offset) {
			CellKey sourceFrom = sourcePicture.From.CellKey;
			CellKey sourceTo = sourcePicture.To.CellKey;
			CellKey targetFrom = GetShiftedCellKey(TargetWorksheet.SheetId, sourceFrom, offset);
			CellKey targetTo = GetShiftedCellKey(TargetWorksheet.SheetId, targetFrom, new CellPositionOffset(sourceTo.ColumnIndex - sourceFrom.ColumnIndex, sourceTo.RowIndex - sourceFrom.RowIndex));
			Picture result = TargetWorksheet.InsertPicture(targetImage, targetFrom, targetTo, sourcePicture.Locks.NoChangeAspect);
			result.From = new AnchorPoint(targetFrom, sourcePicture.From.ColOffset, sourcePicture.From.RowOffset);
			result.To = new AnchorPoint(targetTo, sourcePicture.To.ColOffset, sourcePicture.To.RowOffset);
			return result;
		}
		void IRangeCopyPicturePart.MovePicture(Picture picture, CellPositionOffset offset) {
			MoveDrawingObject(picture, offset);
		}
		void IRangeCopyChartPart.MoveChart(Chart chart, CellPositionOffset offset) {
			MoveDrawingObject(chart, offset);
		}
		void MoveDrawingObject(IDrawingObject drawingObject, CellPositionOffset offset) {
			switch(drawingObject.AnchorData.GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					MoveAnchorAndOffsets(drawingObject, offset);
					break;
				case AnchorType.OneCell:
					MoveTopLeftAnchor(drawingObject, offset);
					break;
			}
		}
		void MoveTopLeftAnchor(IDrawingObject targetDrawingObject, CellPositionOffset offset) {
			CellKey sourceFrom = targetDrawingObject.From.CellKey;
			CellKey targetFrom = GetShiftedCellKey(TargetWorksheet.SheetId, sourceFrom, offset);
			float originalHeight = targetDrawingObject.Height;
			float originalWidth = targetDrawingObject.Width;
			targetDrawingObject.From = new AnchorPoint(targetFrom, targetDrawingObject.From.ColOffset, targetDrawingObject.From.RowOffset);
			targetDrawingObject.DrawingObject.AnchorData.SetIndependentHeight(originalHeight);
			targetDrawingObject.DrawingObject.AnchorData.SetIndependentWidth(originalWidth);
		}
		void MoveAnchorAndOffsets(IDrawingObject targetDrawingObject, CellPositionOffset offset) {
			CellKey sourceFrom = targetDrawingObject.From.CellKey;
			CellKey sourceTo = targetDrawingObject.To.CellKey;
			CellKey targetFrom = GetShiftedCellKey(TargetWorksheet.SheetId, sourceFrom, offset);
			CellKey targetTo = GetShiftedCellKey(TargetWorksheet.SheetId, targetFrom, new CellPositionOffset(sourceTo.ColumnIndex - sourceFrom.ColumnIndex, sourceTo.RowIndex - sourceFrom.RowIndex));
			targetDrawingObject.BeginUpdate();
			try {
				targetDrawingObject.From = new AnchorPoint(targetFrom, targetDrawingObject.From.ColOffset, targetDrawingObject.From.RowOffset);
				targetDrawingObject.To = new AnchorPoint(targetTo, targetDrawingObject.To.ColOffset, targetDrawingObject.To.RowOffset);
			}
			finally {
				targetDrawingObject.EndUpdate();
			}
		}
		protected Picture CreateTargetPictureAbsoluteAnchor(Picture sourcePicture, OfficeImage targetImage, Worksheet targetWorksheet, CellPositionOffset offset) {
			Picture result = targetWorksheet.InsertPicture(targetImage, sourcePicture.CoordinateX,
														   sourcePicture.CoordinateY, sourcePicture.Width,
														   sourcePicture.Height, sourcePicture.Locks.NoChangeAspect);
			if (!(offset.ColumnOffset == 0 && offset.RowOffset == 0)) {
				CellKey targetFrom = GetShiftedCellKey(targetWorksheet.SheetId, sourcePicture.From.CellKey, offset);
				result.From = new AnchorPoint(targetFrom, 0, 0);
			}
			return result;
		}
		void CalculateSourceRange() {
			bool old = WorkbookSource.SuppressResetingCopiedRange;
			try {
				WorkbookSource.BeginUpdate();
				WorkbookSource.SuppressResetingCopiedRange = true;
				WorkbookSource.CalculationChain.CalculateRangeIfHasMarkedCells(SourceRange);
				WorkbookSource.ApplyChanges(DocumentModelChangeActions.UpdateTransactedVersionInCopiedRange);
			}
			finally {
				WorkbookSource.SuppressResetingCopiedRange = old;
				WorkbookSource.EndUpdate();
			}
		}
		#endregion
	}
}
