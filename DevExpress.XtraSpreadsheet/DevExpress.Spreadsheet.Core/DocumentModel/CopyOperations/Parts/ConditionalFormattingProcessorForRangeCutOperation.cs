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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using IntersectionType = DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceRangeIntersectsTargetRangeType;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class ConditionalFormattingProcessorForRangeCutOperation : ConditionalFormattingProcessorForRangeCopyOperation {
		public ConditionalFormattingProcessorForRangeCutOperation(ITargetRangeCalculatorOwner owner, IRangeCopyConditionalFormattingPart rangeCopy)
			: base(owner, rangeCopy) {
		}
		public override void Execute(RangeCopyInfo rangeInfo, ConditionalFormatting  sourceCF) {
			if (!Owner.IsEqualWorksheets) {
				base.Execute(rangeInfo, sourceCF);
				return;
			}
			CellRangeBase sourceObjectRange = sourceCF.CellRange;
			CellRangeBase objectRange = RemoveCellUnionInnerCellRangesOverlaping(sourceObjectRange);
			CellUnion sourceObjectAsUnion = objectRange as CellUnion;
			CellRange sourceObjectAsNormal = objectRange as CellRange;
			if (rangeInfo.SourceAndTargetEquals) {
				CellRangeBase newRangeMerged2 = rangeInfo.TryConsolidateRangesInCellUnion(objectRange);
				RangeCopy.UseSourceConditionalFormatting(sourceCF, newRangeMerged2, rangeInfo.TargetBigRange);
				return;
			}
			CellRange sourceRange = rangeInfo.SourceRange;
			if (!(sourceRange.Includes(objectRange) || sourceRange.Intersects(objectRange)))
				return;
			CellRangeBase cuttedPart = objectRange.ExcludeRange(sourceRange);
			CellRangeBase newRange = null;
			CellRange targetRange = rangeInfo.TargetBigRange;
			bool ignoreCuttedPart = cuttedPart == null || targetRange.Includes(cuttedPart);
			if (ignoreCuttedPart) {
				newRange = Owner.GetTargetObjectRange(objectRange, targetRange);
			}
			else {
				CellRangeBase targetObjectRange = Owner.GetTargetObjectRange(objectRange, targetRange);
				CellRangeBase cuttedPartWithoutTargetRange = cuttedPart.ExcludeRange(targetRange);
				cuttedPartWithoutTargetRange = rangeInfo.TryConsolidateRangesInCellUnion(cuttedPartWithoutTargetRange);
				bool cuttedPartAsLast = TestIfSecondRangeShouldHaveSecondPositionInCellUnion(rangeInfo, targetObjectRange, cuttedPartWithoutTargetRange);
				if (cuttedPartWithoutTargetRange != null) {
					if (cuttedPartAsLast) {
						VariantValue concatination = targetObjectRange.ConcatinateWith(cuttedPartWithoutTargetRange);
						newRange = concatination.CellRangeValue;
					}
					else {
						VariantValue newVariable = cuttedPartWithoutTargetRange.ConcatinateWith(targetObjectRange);
						newRange = newVariable.CellRangeValue;
					}
				}
			}
			CellRangeBase newRangeMerged = rangeInfo.TryConsolidateRangesInCellUnion(newRange);
			CellRangeBase newRangeWithoutOverlaping = RemoveCellUnionInnerCellRangesOverlaping(newRangeMerged); 
			RangeCopy.UseSourceConditionalFormatting(sourceCF, newRangeWithoutOverlaping, rangeInfo.TargetBigRange);
		}
		CellRangeBase RemoveCellUnionInnerCellRangesOverlaping(CellRangeBase rangeBase) {
			CellUnion range = rangeBase as CellUnion;
			if (range == null)
				return rangeBase;
			List<CellRangeBase> innerRanges = range.InnerCellRanges;
			int count = innerRanges.Count;
			if (count == 0)
				return rangeBase;
			List<CellRangeBase> resultList = new List<CellRangeBase>();
			resultList.Add(innerRanges[0]);
			CellUnion result = new CellUnion(resultList);
			for (int innerRangeIndex = 1; innerRangeIndex < count; innerRangeIndex++) {
				CellRangeBase item = innerRanges[innerRangeIndex];
				CellRangeBase excluded = item.ExcludeRange(result);
				CellUnion excludedAsUnion = excluded as CellUnion;
				if (excludedAsUnion != null)
					resultList.AddRange(excludedAsUnion.InnerCellRanges);
				else if (excluded != null)
					resultList.Add(excluded);
			}
			if (result.InnerCellRanges.Count == 1)
				return innerRanges[0];
			return result;
		}
		bool TestIfSecondRangeShouldHaveSecondPositionInCellUnion(RangeCopyInfo rangeinfo, CellRangeBase first, CellRangeBase second) {
			if (rangeinfo.IntersectionType != SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals)
				return false;
			CellRange extendedRange = GetExtendedRangeFromTwoRanges(first, second);
			bool reversedHoriz = !CalculateIsReversedHorizontalEnumerator(rangeinfo.SourceRange, rangeinfo.TargetBigRange);
			bool reversedVert = !CalculateIsReversedVertinalEnumerator(rangeinfo.SourceRange, rangeinfo.TargetBigRange);
			int firstSeqPosition = GetTopLeftCellPositionInRange(first.TopLeft, extendedRange, reversedHoriz, reversedVert);
			int secondSeqPosition = GetTopLeftCellPositionInRange(second.TopLeft, extendedRange, reversedHoriz, reversedVert);
			return secondSeqPosition > firstSeqPosition;
		}
		CellRange GetExtendedRangeFromTwoRanges(CellRangeBase first, CellRangeBase second) {
			int rangeWidth = Math.Abs(first.TopLeft.Column - second.TopLeft.Column) + 1;
			int rangeHeight = Math.Abs(first.TopLeft.Row - second.TopLeft.Row) + 1;
			int rangeLeftColumn = Math.Min(first.TopLeft.Column, second.TopLeft.Column);
			int rangeTopRow = Math.Min(first.TopLeft.Row, second.TopLeft.Row);
			CellPosition rangeTopLeft = new CellPosition(rangeLeftColumn, rangeTopRow);
			CellPosition rangeBottomRight = new CellPosition(rangeLeftColumn + rangeWidth - 1, rangeTopRow + rangeHeight - 1);
			CellRange range = new CellRange(first.Worksheet, rangeTopLeft, rangeBottomRight);
			return range;
		}
		int GetTopLeftCellPositionInRange(CellPosition topLeft, CellRange range, bool reversedHoriz, bool reversedVert) {
			int relativeRows = !reversedVert ? topLeft.Row - range.TopRowIndex : range.BottomRowIndex - topLeft.Row;
			int relativeColumn = (!reversedHoriz) ? topLeft.Column - range.LeftColumnIndex : range.RightColumnIndex - topLeft.Column;
			return relativeRows * range.Width + relativeColumn;
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
}
