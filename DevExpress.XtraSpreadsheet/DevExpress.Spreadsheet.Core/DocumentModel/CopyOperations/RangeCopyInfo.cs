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
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class RangeCopyInfo : IRangeInfo {
		readonly List<CellRange> targetRanges;
		readonly List<CellRange> selectedRanges;
		CellRange sourceRange;
		CellRange targetBigRange;
		ModelErrorType errorType;
		GrowthDirectionOfConjugatedRanges growthDirection;
		Direction direction;
		int growthSize;
		SourceRangeIntersectsTargetRangeType sourceRangeIntersectsTargetRangeType;
		public RangeCopyInfo(CellRange sourceRange, CellRange targetBigRange) {
			this.sourceRange = sourceRange;
			this.targetBigRange = targetBigRange;
			targetRanges = new List<CellRange>();
			selectedRanges = new List<CellRange>();
			this.growthDirection = GrowthDirectionOfConjugatedRanges.None;
			this.direction = Direction.None;
			this.growthSize = Int32.MinValue;
			this.sourceRangeIntersectsTargetRangeType = SourceRangeIntersectsTargetRangeType.None;
		}
		public List<CellRange> TargetRanges { get { return targetRanges; } }
		public List<CellRange> SelectedRanges { get { return selectedRanges; } }
		public ModelErrorType ErrorType { get { return errorType; } set { errorType = value; } }
		public CellRange SourceRange { get { return sourceRange; } }
		public CellRange TargetBigRange { get { return targetBigRange; } }
		public IRangeInfo Info { get { return this; } }
		bool IRangeInfo.SourceIsIntervalRange { get { return GetSourceAsIntervalRange() != null; } }
		bool IRangeInfo.SourceIsWholeWorksheetRange { get { return Info.SourceIsIntervalRange && GetSourceAsIntervalRange().IsWholeWorksheetRange(); } }
		bool IRangeInfo.TargetBigRangeIsIntervalRange { get { return GetTargetBigRangeAsIntervalRange() != null; } }
		public GrowthDirectionOfConjugatedRanges GrowthDirection { get { return growthDirection; } set { growthDirection = value; } }
		public Direction Direction { get { return direction; } set { direction = value; } }
		public SourceRangeIntersectsTargetRangeType IntersectionType { get { return sourceRangeIntersectsTargetRangeType; } set { sourceRangeIntersectsTargetRangeType = value; } }
		public bool SourceAndTargetAreIntersected { get { return sourceRangeIntersectsTargetRangeType != SourceRangeIntersectsTargetRangeType.None; } }
		public bool SourceAndTargetEquals { get { return Object.ReferenceEquals(sourceRange.Worksheet, targetBigRange.Worksheet) && sourceRange.EqualsPosition(targetBigRange); } }
		public int GrowthSize { get { return growthSize; } set { growthSize = value; } }
		public bool SourceIsColumnIntervalRange {
			get {
				CellIntervalRange interval = GetSourceAsIntervalRange();
				return (interval != null) ? interval.IsColumnInterval : false;
			}
		}
		public bool TargetBigIsColumnIntervalRange {
			get {
				CellIntervalRange interval = GetTargetBigRangeAsIntervalRange();
				return (interval != null) ? interval.IsColumnInterval : false;
			}
		}
		public bool PasteSingleCellToMergedRange { get; set; }
		public CellIntervalRange GetTargetBigRangeAsIntervalRange() {
			return targetBigRange as CellIntervalRange;
		}
		public CellIntervalRange GetSourceAsIntervalRange() {
			return sourceRange as CellIntervalRange;
		}
		public bool CheckObjectHasNoGapsInRangeInDirection(CellRange objectRange, CellRange insideThisRange, Direction direction) {
			if (direction == Direction.Horizontal)
				return objectRange.LeftColumnIndex <= insideThisRange.LeftColumnIndex
				&& objectRange.RightColumnIndex >= insideThisRange.RightColumnIndex;
			else
				return objectRange.TopRowIndex <= insideThisRange.TopRowIndex && objectRange.BottomRowIndex >= insideThisRange.BottomRowIndex;
		}
		public Direction CalculateObjectRangeDirectionInSourceRange(CellRange objectRange) {
			bool hNoGaps = CheckObjectHasNoGapsInRangeInDirection(objectRange, SourceRange, CopyOperation.Direction.Horizontal);
			bool vNoGaps = CheckObjectHasNoGapsInRangeInDirection(objectRange, SourceRange, CopyOperation.Direction.Vertical);
			if (hNoGaps && vNoGaps)
				return Direction.Both;
			if (hNoGaps)
				return Direction.Horizontal;
			else if (vNoGaps)
				return Direction.Vertical;
			return CopyOperation.Direction.None;
		}
		public List<CellRange> SplitTargetRangeBy(Direction sourceObjectDirection) {
			if (sourceObjectDirection == CopyOperation.Direction.Vertical)
				return SplitTargetRangeByVerticalDirection(TargetBigRange, SourceRange.Width);
			if (sourceObjectDirection == CopyOperation.Direction.Horizontal)
				return SplitTargetRangeByHorizontalDirection(TargetBigRange, sourceRange.Height);
			throw new InvalidOperationException("SplitTargetRangeBy non vertical or horizontal direction");
		}
		List<CellRange> SplitTargetRangeByVerticalDirection(CellRange range, int sourceRangeWidth) {
			List<CellRange> result = new List<CellRange>();
			int rangesCount = range.Width / sourceRangeWidth;
			for (int i = 0; i < rangesCount; i++) {
				int leftColumnRelative = 0 + i * sourceRangeWidth;
				int rightColumnRelative = sourceRangeWidth * (i + 1) - 1;
				CellPosition topLeft = new CellPosition(range.LeftColumnIndex + leftColumnRelative, range.TopRowIndex);
				CellPosition bottomRight = new CellPosition(range.LeftColumnIndex + rightColumnRelative, range.BottomRowIndex);
				CellRange resultRange = new CellRange(range.Worksheet, topLeft, bottomRight);
				result.Add(resultRange);
			}
			return result;
		}
		List<CellRange> SplitTargetRangeByHorizontalDirection(CellRange range, int sourceRangeHeight) {
			List<CellRange> result = new List<CellRange>();
			int count = range.Height / sourceRangeHeight;
			for (int i = 0; i < count; i++) {
				int topRowRelative = 0 + i * sourceRangeHeight;
				int bottomRowRelative = sourceRangeHeight * (i + 1) - 1;
				CellPosition topLeft = new CellPosition(range.LeftColumnIndex, topRowRelative + range.TopRowIndex);
				CellPosition bottomRight = new CellPosition(range.RightColumnIndex, bottomRowRelative + range.BottomRowIndex);
				CellRange newRange = new CellRange(range.Worksheet, topLeft, bottomRight);
				result.Add(newRange);
			}
			return result;
		}
		struct SourceObjectRangeWithSourceRangeInfo : ISourceObjectRangeInfo {
			public bool EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth { get; set; }
			public bool ObjectRangeWiderIfVerticalOrLongerIfHorizontal { get; set; }
			public bool SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz { get; set; }
			public bool ObjectRangeWiderIfVerticalAndLongerIfHorizBothSides { get; set; }
			public bool ObjectRangeWiderOrLongerAtOneSide { get; set; }
			public bool NoGaps { get; set; }
			public Direction ObjectDirection { get; set; }
		}
		public ISourceObjectRangeInfo CalculateSourceObjectRangeInfo(CellRange sourceObjectRange) {
			SourceObjectRangeWithSourceRangeInfo result = new SourceObjectRangeWithSourceRangeInfo();
			Direction objectDirection = CalculateObjectRangeDirectionInSourceRange(sourceObjectRange);
			bool equalsWidthsSourceObjectAndSourceRange = SourceRange.LeftColumnIndex == sourceObjectRange.LeftColumnIndex && SourceRange.RightColumnIndex == sourceObjectRange.RightColumnIndex;
			bool objectRangeWiderAtLeft = sourceObjectRange.LeftColumnIndex < SourceRange.LeftColumnIndex && sourceObjectRange.RightColumnIndex == SourceRange.RightColumnIndex;  
			bool objectRangeWiderAtRight = sourceObjectRange.RightColumnIndex > SourceRange.RightColumnIndex && sourceObjectRange.LeftColumnIndex == SourceRange.LeftColumnIndex;
			bool objectRangeWiderBothSides = sourceObjectRange.RightColumnIndex > SourceRange.RightColumnIndex && sourceObjectRange.LeftColumnIndex < SourceRange.LeftColumnIndex;
			bool objectRangeNotWiderInsideSourceRange = sourceObjectRange.LeftColumnIndex >= SourceRange.LeftColumnIndex && sourceObjectRange.RightColumnIndex <= SourceRange.RightColumnIndex;
			bool equalsHeightsSourceObjectAndSourceRange = SourceRange.TopRowIndex == sourceObjectRange.TopRowIndex && SourceRange.BottomRowIndex == sourceObjectRange.BottomRowIndex;
			bool objectRangeLongerAtTop = sourceObjectRange.TopRowIndex < SourceRange.TopRowIndex && sourceObjectRange.BottomRowIndex == SourceRange.BottomRowIndex; 
			bool objectRangeLongerAtBottom = sourceObjectRange.BottomRowIndex > SourceRange.BottomRowIndex && sourceObjectRange.TopRowIndex == SourceRange.TopRowIndex;
			bool objectRangeLongerBothSides = sourceObjectRange.BottomRowIndex > SourceRange.BottomRowIndex && sourceObjectRange.TopRowIndex < SourceRange.TopRowIndex;
			bool objectRangeNotLongerInsideSourceRange = sourceObjectRange.TopRowIndex >= SourceRange.TopRowIndex && sourceObjectRange.BottomRowIndex <= SourceRange.BottomRowIndex;
			result.ObjectRangeWiderIfVerticalAndLongerIfHorizBothSides = false;
			if (Direction == Direction.Vertical)
				result.ObjectRangeWiderIfVerticalAndLongerIfHorizBothSides = objectRangeWiderBothSides;
			else if (Direction == Direction.Horizontal)
				result.ObjectRangeWiderIfVerticalAndLongerIfHorizBothSides = objectRangeLongerBothSides;
			result.EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth = false;
			if (Direction == Direction.Vertical)
				result.EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth = equalsWidthsSourceObjectAndSourceRange || sourceObjectRange.Equals(SourceRange);
			else if (Direction == Direction.Horizontal)
				result.EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth = equalsHeightsSourceObjectAndSourceRange || sourceObjectRange.Equals(SourceRange);
			else if (Direction == Direction.Both)
				result.EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth = sourceObjectRange.EqualsPosition(SourceRange)
				   && Object.ReferenceEquals(sourceObjectRange.Worksheet, SourceRange.Worksheet); 
			result.ObjectRangeWiderIfVerticalOrLongerIfHorizontal = false;
			result.ObjectRangeWiderOrLongerAtOneSide = false;
			if (Direction == Direction.Vertical) {
				result.ObjectRangeWiderIfVerticalOrLongerIfHorizontal = (objectRangeWiderAtLeft || objectRangeWiderAtRight || objectRangeWiderBothSides);
				result.ObjectRangeWiderOrLongerAtOneSide = (objectRangeWiderAtLeft || objectRangeWiderAtRight);
			}
			else if (Direction == Direction.Horizontal) {
				result.ObjectRangeWiderIfVerticalOrLongerIfHorizontal = (objectRangeLongerAtTop || objectRangeLongerAtBottom || objectRangeLongerBothSides);
				result.ObjectRangeWiderOrLongerAtOneSide = objectRangeLongerAtTop || objectRangeLongerAtBottom;
			}
			result.SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz = false;
			if (Direction == Direction.Vertical)
				result.SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz = objectRangeNotWiderInsideSourceRange;
			else if (Direction == Direction.Horizontal)
				result.SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz = objectRangeNotLongerInsideSourceRange;
			result.NoGaps = objectDirection == Direction.Both;
			result.ObjectDirection = objectDirection;
			return result;
		}
		public static CellRange GetExpandedByOneCellCellRange(CellRange sourceCfCellRange) {
			return GetExpandedByOneCellCellRange(sourceCfCellRange, true, true, true, true);
		}
		public static CellRange GetExpandedByOneCellCellRange(CellRange sourceCfCellRange, bool expandTop, bool expandBottom, bool expandLeft, bool expandRight) {
			const int minusOne = -1;
			const int plusOne = 1;
			int diffLeftColumn = (sourceCfCellRange.LeftColumnIndex > 0 && expandLeft) ? minusOne : 0; 
			int diffTopRow = (sourceCfCellRange.TopRowIndex > 0 && expandTop) ? minusOne : 0;
			int diffRightColumn = (sourceCfCellRange.RightColumnIndex < DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxColumnCount - 1 && expandRight) ? plusOne : 0;
			int diffBottomRow = (sourceCfCellRange.BottomRowIndex < DevExpress.XtraSpreadsheet.Utils.IndicesChecker.MaxRowCount - 1 && expandBottom) ? plusOne : 0;
			CellRange expanded = sourceCfCellRange.GetResized(diffLeftColumn, diffTopRow, diffRightColumn, diffBottomRow);
			return expanded;
		}
		public bool CanAddPartToRange(CellRange range, CellRange part, bool intersected) {
			if (part == null)
				return false;
			bool conjugated = intersected ? RangesIsConjugatedOrIntersected(range, part) : RangesIsConjugatedSharedFormulaCase(range, part);
			Func<CellRange, CellRange, bool> rangesHasEqualsWidthOrHeight = (one, two) => {
				return (one.Width == two.Width && one.Height != two.Height)
				  || (one.Width != two.Width && one.Height == two.Height);
			};
			return conjugated && rangesHasEqualsWidthOrHeight(range, part);
		}
		public bool RangesIsConjugatedOrIntersected(CellRange one, CellRange two) {
			CellRange expandedOne = GetExpandedByOneCellCellRange(one);
			return expandedOne.Intersects(two);
		}
		public bool RangesIsConjugated(RangeCopyInfo rangeInfo) {
			if (rangeInfo.SourceAndTargetAreIntersected)
				return false;
			CellRange expandedSourceRange = GetExpandedByOneCellCellRange(rangeInfo.SourceRange);
			return expandedSourceRange.Intersects(rangeInfo.TargetBigRange);
		}
		public bool RangesIsConjugatedSharedFormulaCase(CellRange one, CellRange two) {
			if (one.Intersects(two))
				return false;
			CellRange expandedOne = RangeCopyInfo.GetExpandedByOneCellCellRange(one);
			return expandedOne.Intersects(two);
		}
		public CellRangeBase TryConsolidateRangesInCellUnion(CellRangeBase range) {
			CellRangeBase merged = range.TryMergeWithRange(range.GetFirstInnerCellRange().Clone());
			return merged;
		}
		public bool ObjectRangeIntersectsRangeBorder(CellRange firstRange, CellRange objectRange) {
			bool intersectsBorders = objectRange.Intersects(firstRange) && !RangeHasEqualsPosition(objectRange, firstRange);
			return intersectsBorders
				&& !firstRange.ContainsRange(objectRange); 
		}
		public bool RangeHasEqualsPosition(CellRange first, CellRange second) {
			return first.EqualsPosition(second) && Object.ReferenceEquals(first.Worksheet, second.Worksheet);
		}
	}
}
