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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using DevExpress.Office.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region MovePivotCommand
	public class MovePivotCommand : PivotTableErrorHandledCommand {
		#region Fields
		readonly CellRangeBase sourceRange;
		readonly CellRange targetRange;
		MovePivotOperation movePivotOperation;
		CellRangeBase targetNonPivotRanges;
		CellRangeBase sourceNonPivotRanges;
		#endregion
		public MovePivotCommand(PivotTable pivotTable, CellRange targetRange, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			Guard.ArgumentNotNull(targetRange, "targetRange");
			Guard.ArgumentNotNull(targetRange.Worksheet, "targetWorksheet");
			this.sourceRange = PivotTable.WholeRange;
			this.targetRange = CorrectTargetRange(targetRange);
			Initialize();
		}
		#region Properties
		bool HasPageFields { get { return PivotTable.PageFields.Count > 0; } }
		CellPositionOffset Offset { get { return movePivotOperation.GetTargetRangeOffset(targetRange); } }
		bool NoNonPivotRanges { get { return sourceNonPivotRanges == null || targetNonPivotRanges == null; } }
		SourceTargetRangesForCopy RangesInfo { get { return movePivotOperation.RangesInfo; } }
		CellRange TargetBigRange { get { return RangesInfo.First.TargetBigRange; } }
		#endregion
		#region Initialize
		void Initialize() {
			InitializeMove(sourceRange.GetCoveredRange(), targetRange);
			if (HasPageFields) {
				CellRange pageCoveredRange = PivotTable.Location.PageFieldsRange.GetCoveredRange();
				sourceNonPivotRanges = RangesInfo.SourceRange.ExcludeRange(sourceRange).ExcludeRange(pageCoveredRange);
				targetNonPivotRanges = sourceNonPivotRanges.GetShiftedAny(Offset, targetRange.Worksheet);
			}
		}
		void InitializeMove(CellRange sourceRange, CellRange targetRange) {
			SourceTargetRangesForCopy ranges = new SourceTargetRangesForCopy(sourceRange, targetRange);
			movePivotOperation = new MovePivotOperation(ranges);
			movePivotOperation.ErrorHandler = ErrorHandler;
		}
		CellRange CorrectTargetRange(CellRange targetRange) {
			if (!HasPageFields)
				return targetRange;
			int heightOfPageFieldsAbovePivot = sourceRange.GetAreaByIndex(0).Height + 1;
			int newRow = targetRange.TopLeft.Row - heightOfPageFieldsAbovePivot;
			if (newRow < 0)
				newRow = 0;
			CellPosition newTopLeft = new CellPosition(targetRange.TopLeft.Column, newRow);
			return new CellRange(targetRange.Worksheet, newTopLeft, newTopLeft);
		}
		#endregion
		#region Validate
		protected internal override bool Validate() {
			IModelErrorInfo error = movePivotOperation.Checks();
			if (error != null)
				return HandleError(GetErrorInfo(error));
			if (TargetRangeIntersectsTable())
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotTableReportCanNotOverlapTable));
			if (TargetRangeIntersectsAnotherPivot())
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotTableCanNotOverlapPivotTable));
			if (RangesInfo.First.SourceAndTargetEquals)
				return false;
			if (RangeAfterMoveIntersectsData())
				return HandleError(new ClarificationErrorInfo(ModelErrorType.PivotTableWillOverrideSheetCells));
			return true;
		}
		bool RangeAfterMoveIntersectsData() {
			CellRangeBase rangeAfterMove;
			if (targetNonPivotRanges == null)
				rangeAfterMove = TargetBigRange;
			else
				rangeAfterMove = TargetBigRange.ExcludeRange(targetNonPivotRanges);
			if (rangeAfterMove.Intersects(sourceRange))
				rangeAfterMove = rangeAfterMove.ExcludeRange(sourceRange);
			return rangeAfterMove.HasData();
		}
		bool TargetRangeIntersectsTable() {
			return RangesInfo.TargetWorksheet.Tables.ContainsItemsInRange(TargetBigRange, true);
		}
		bool TargetRangeIntersectsAnotherPivot() {
			List<PivotTable> pivots = RangesInfo.TargetWorksheet.PivotTables.GetItems(TargetBigRange, true);
			return pivots.Count > 1 || (pivots.Count == 1 && !Object.ReferenceEquals(pivots[0], PivotTable));
		}
		IModelErrorInfo GetErrorInfo(IModelErrorInfo error) {
			ModelErrorType errorType = error.ErrorType;
			ModelErrorType newErrorType = ModelErrorType.None;
			if (errorType == ModelErrorType.SelectedCellsAffectsPivotTableAndCanNotBeChanged)
				newErrorType = ModelErrorType.PivotTableCanNotOverlapPivotTable;
			if (errorType == ModelErrorType.MergedCellCanNotBeChanged)
				newErrorType = ModelErrorType.CantDoThatToAMergedCell;
			if (errorType == ModelErrorType.CopyAreaCannotBeFitIntoThePasteArea)
				newErrorType = ModelErrorType.PivotTableWillNotFitOnTheSheetSelectNewLocation;
			return newErrorType != ModelErrorType.None ? new ModelErrorInfo(newErrorType) : error;
		}
		#endregion
		protected internal override void ExecuteCore() {
			movePivotOperation.ExecuteWithoutValidation();
			ClearRange(movePivotOperation.GetRangeToClearAfterCut());
			MoveBackNonPivotRanges();
		}
		void ClearRange(CellRangeBase rangeToClear) {
			Worksheet targetSheet = rangeToClear.Worksheet as Worksheet;
			targetSheet.ClearAll(rangeToClear, ErrorHandler);
			targetSheet.ClearCellsNoShift(rangeToClear);
		}
		#region MoveBack
		void MoveBackNonPivotRanges() {
			if (NoNonPivotRanges)
				return;
			if (movePivotOperation.IsEqualWorksheets) {
				CellRangeBase intersectionWithMovedPivot = GetIntersectionWithMovedPivot();
				if (intersectionWithMovedPivot != null) {
					CellRangeBase rangeToClear = intersectionWithMovedPivot.GetShiftedAny(Offset, targetRange.Worksheet);
					ClearRange(rangeToClear);
					ExcludeFromNonPivotRanges(intersectionWithMovedPivot, rangeToClear);
				}
			}
			int currentAreaIndex = 0;
			while (!NoNonPivotRanges) {
				if (currentAreaIndex >= targetNonPivotRanges.AreasCount)
					currentAreaIndex = 0;
				CellRange from = targetNonPivotRanges.GetAreaByIndex(currentAreaIndex);
				CellRange to = sourceNonPivotRanges.GetAreaByIndex(currentAreaIndex);
				if (TargetAndSourceIntersect(to, from))
					currentAreaIndex++;
				else {
					InitializeMove(from, to);
					movePivotOperation.ExecuteWithoutValidation();
					ClearRange(movePivotOperation.GetRangeToClearAfterCut());
					ExcludeFromNonPivotRanges(to, from);
				}
			}
		}
		void ExcludeFromNonPivotRanges(CellRangeBase source, CellRangeBase target) {
			sourceNonPivotRanges = sourceNonPivotRanges.ExcludeRange(source);
			targetNonPivotRanges = targetNonPivotRanges.ExcludeRange(target);
		}
		bool TargetAndSourceIntersect(CellRange source, CellRange exception) {
			foreach (CellRange innerTargetRange in targetNonPivotRanges.GetAreasEnumerable())
				if (!Object.ReferenceEquals(innerTargetRange, exception) && source.Intersects(innerTargetRange))
					return true;
			return false;
		}
		CellRangeBase GetIntersectionWithMovedPivot() {
			VariantValue result = TargetBigRange.ExcludeRange(targetNonPivotRanges).IntersectionWith(sourceNonPivotRanges);
			if (result.IsCellRange)
				return result.CellRangeValue;
			return null;
		}
		#endregion
	}
	#endregion
	#region MovePivotOperation
	public class MovePivotOperation : CutRangeOperation {
		public MovePivotOperation(SourceTargetRangesForCopy ranges) : base(ranges) { 
		}
		public void ExecuteWithoutValidation() {
			try {
				BeginExecute();
				ExecuteCore();
			}
			finally {
				EndExecute();
			}
		}
		protected override RemoveRangeCommand CreateRemoveRangeCommand(CellRangeBase range) {
			RemoveRangeCommand command = base.CreateRemoveRangeCommand(range);
			command.SuppressPivotTableChecks = true;
			return command;
		}
		public override void CopyContent(RangeCopyInfo rangeCopyInfo) {
			CopyDefinedNames(rangeCopyInfo.TargetBigRange);
			CopySharedFormulaList(rangeCopyInfo);
			CopyMergedCells(rangeCopyInfo);
			CopyDrawingObjects(rangeCopyInfo);
			CopyComments(rangeCopyInfo);
			CopyProtectedRanges(rangeCopyInfo);
			CopyConditionalFormattings(rangeCopyInfo);
			CopyVmlDrawing(); 
			CopyPivotTables(rangeCopyInfo);
		}
		protected override IModelErrorInfo CheckMergedRangesIntersectedTargetRange(RangeCopyInfo rangeInfo, CellRange range, List<CellRange> intersectedMergedCellRanges) {
			if (RangeContainsMergedCellRanges(range, intersectedMergedCellRanges))
				return null;
			return base.CheckMergedRangesIntersectedTargetRange(rangeInfo, range, intersectedMergedCellRanges);
		}
		bool RangeContainsMergedCellRanges(CellRange range, List<CellRange> intersectedMergedCellRanges) {
			foreach (CellRange mergedRange in intersectedMergedCellRanges)
				if (!range.ContainsRange(mergedRange))
					return false;
			return true;
		}
	}
	#endregion
}
