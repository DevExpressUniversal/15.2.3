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
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class SourceTargetRangeForCopyCalculator {
		readonly CellRange sourceRange;
		bool targetRangeAndSourceHaveSameSize;
		bool targetRangeContainsNotIntegerNumberOfSourceRanges;
		bool canSplitByIntegerNumberOfSourceRanges;
		bool targetRangeSmallerThanSource;
		bool targetRangeInVerticalSingleCellRange;
		bool targetRangeIsHorizontalSingleCellRange;
		bool targetRangeSmallerThanSourceByOneSide;
		public SourceTargetRangeForCopyCalculator(CellRange sourceRange) {
			Guard.ArgumentNotNull(sourceRange, "sourceRange");
			this.sourceRange = sourceRange;
		}
		int SourceRangeHeight { [System.Diagnostics.DebuggerStepThrough] get { return sourceRange.Height; } }
		int SourceRangeWidth { [System.Diagnostics.DebuggerStepThrough] get { return sourceRange.Width; } }
		void RegisterTargetRange(RangeCopyInfo result, CellRange enlargedTarget) {
			if (enlargedTarget == null) {
				result.ErrorType = ModelErrorType.CopyAreaCannotBeFitIntoThePasteArea;
				return;
			}
			result.TargetRanges.Add(enlargedTarget);
			result.SelectedRanges.Add(enlargedTarget);
		}
		public List<RangeCopyInfo> Process(CellRangeBase target) {
			List<RangeCopyInfo> result = new List<RangeCopyInfo>();
			CellUnion union = (target as CellUnion);
			if (union == null) {
				RangeCopyInfo newItem = ProcessNormalRange(sourceRange, target as CellRange);
				Calculate(newItem);
				result.Add(newItem);
			}
			else {
				foreach (CellRangeBase item in union.InnerCellRanges) {
					CellRange currentCellRange = item as CellRange;
					RangeCopyInfo newInfo = ProcessNormalRange(sourceRange, currentCellRange);
					result.Add(newInfo);
					Calculate(newInfo);
					if (newInfo.ErrorType != ModelErrorType.None)
						return result;
				}
			}
			return result;
		}
		void Calculate(RangeCopyInfo rangeInfo) {
			if (rangeInfo.ErrorType != ModelErrorType.None)
				return;
			CellRange sourceRange = rangeInfo.SourceRange;
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			List<CellRange> currentTargetRanges = rangeInfo.TargetRanges;
			if (sourceRange.Intersects(targetBigRange)) {
				if (IsRangeIntersectsSeveralRanges(sourceRange, currentTargetRanges)) {
					rangeInfo.ErrorType = ModelErrorType.CopyAreaCannotOverlapUnlessSameSizeAndShape;
				}
				else {
					ProcessSourceIntersectsTargetBigRangeCase(rangeInfo);
				}
				return;
			}
			GrowthDirectionOfConjugatedRanges growthDirection = GrowthDirectionOfConjugatedRanges.None;
			int growthSize = Int32.MinValue;
			Direction direction = Direction.None;
			if (currentTargetRanges.Count == 1) {
				growthDirection = CalcGrowthDirection(sourceRange, targetBigRange, Direction.Horizontal);
				if (growthDirection == GrowthDirectionOfConjugatedRanges.None)
					growthDirection = CalcGrowthDirection(sourceRange, targetBigRange, Direction.Vertical);
			}
			else {
				CellRange first = currentTargetRanges[0];
				CellRange last = currentTargetRanges[currentTargetRanges.Count - 1];
				direction = CalculateRangeDirection(first, last);
				growthDirection = CalcGrowthDirection(sourceRange, targetBigRange, direction);
			}
			if (growthDirection == GrowthDirectionOfConjugatedRanges.HorizontalContinueLeft || growthDirection == GrowthDirectionOfConjugatedRanges.HorizontalContinueRight)
				growthSize = targetBigRange.Width;
			if (growthDirection == GrowthDirectionOfConjugatedRanges.VerticalContinueBottom || growthDirection == GrowthDirectionOfConjugatedRanges.VerticalContinueTop)
				growthSize = targetBigRange.Height;
			rangeInfo.GrowthDirection = growthDirection;
			rangeInfo.Direction = direction;
			rangeInfo.GrowthSize = growthSize;
		}
		void ProcessSourceIntersectsTargetBigRangeCase(RangeCopyInfo rangeInfo) {
			CellRange sourceRange = rangeInfo.SourceRange;
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			List<CellRange> currentTargetRanges = rangeInfo.TargetRanges;
			CellRange first = currentTargetRanges[0];
			if (currentTargetRanges.Count == 1) {
				rangeInfo.IntersectionType = SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals;
				return;
			}
			CellRange last = currentTargetRanges[currentTargetRanges.Count - 1];
			Direction direction = CalculateRangeDirection(first, last);
			rangeInfo.Direction = direction;
			rangeInfo.IntersectionType = CalcIntersectionType(direction, sourceRange, first, last);
		}
		SourceRangeIntersectsTargetRangeType CalcIntersectionType(Direction direction, CellRange sourceRange, CellRange first, CellRange last) {
			if (sourceRange.Equals(first))
				return SourceRangeIntersectsTargetRangeType.SourceRangeIsTheFirst;
			else if (sourceRange.Equals(last))
				return SourceRangeIntersectsTargetRangeType.SourceRangeIsTheLast;
			return SourceRangeIntersectsTargetRangeType.SourceRangeInTheMiddle;
		}
		bool IsRangeIntersectsSeveralRanges(CellRange sourceRange, List<CellRange> currentTargetRanges) {
			int intersections = 0;
			const int severalCount = 2;
			foreach (CellRange item in currentTargetRanges) {
				if (item.Intersects(sourceRange))
					intersections++;
				if (intersections == severalCount)
					break;
			}
			return intersections == severalCount;
		}
		public static Direction CalculateRangeDirection(CellRange first, CellRange last) {
			Direction direction = Direction.None;
			if (first.TopLeft.Column == last.TopLeft.Column && first.BottomRight.Column == last.BottomRight.Column)
				direction = Direction.Vertical;
			else if (first.TopLeft.Row == last.TopLeft.Row && first.BottomRight.Row == last.BottomRight.Row)
				direction = Direction.Horizontal;
			else
				direction = Direction.Both;
			return direction;
		}
		public static GrowthDirectionOfConjugatedRanges CalcGrowthDirection(CellRange sourceRange, CellRange targetBigRange, Direction direction) {
			GrowthDirectionOfConjugatedRanges growthDirection = GrowthDirectionOfConjugatedRanges.None;
			if (direction == Direction.Vertical) {
				if (sourceRange.BottomRight.Row + 1 == targetBigRange.TopLeft.Row) {
					growthDirection = GrowthDirectionOfConjugatedRanges.VerticalContinueBottom;
				}
				else if (sourceRange.TopLeft.Row - 1 == targetBigRange.BottomRight.Row) {
					growthDirection = GrowthDirectionOfConjugatedRanges.VerticalContinueTop;
				}
			}
			else if (direction == Direction.Horizontal) {
				if (sourceRange.BottomRight.Column + 1 == targetBigRange.TopLeft.Column) {
					growthDirection = GrowthDirectionOfConjugatedRanges.HorizontalContinueRight;
				}
				else if (sourceRange.TopLeft.Column - 1 == targetBigRange.BottomRight.Column) {
					growthDirection = GrowthDirectionOfConjugatedRanges.HorizontalContinueLeft;
				}
			}
			return growthDirection;
		}
		void Analize(CellRange target) {
			targetRangeAndSourceHaveSameSize = SourceRangeWidth == target.Width && SourceRangeHeight == target.Height;
			targetRangeContainsNotIntegerNumberOfSourceRanges =
								(target.Width % SourceRangeWidth > 0 || target.Height % SourceRangeHeight > 0)
								 && target.Width >= SourceRangeWidth
								 && target.Height >= SourceRangeHeight;
			canSplitByIntegerNumberOfSourceRanges = target.Width % SourceRangeWidth == 0
				&& target.Height % SourceRangeHeight == 0;
			targetRangeSmallerThanSource = target.Width <= sourceRange.Width && target.Height <= sourceRange.Height;
			targetRangeInVerticalSingleCellRange = target.Width == 1 && target.Height != 1;
			targetRangeIsHorizontalSingleCellRange = target.Height == 1 && target.Width != 1;
			targetRangeSmallerThanSourceByOneSide = ((target.Width < sourceRange.Width || target.Height < sourceRange.Height)
				 && !(targetRangeInVerticalSingleCellRange || targetRangeIsHorizontalSingleCellRange));
		}
		public RangeCopyInfo ProcessNormalRange(CellRange sourceRange, CellRange target) {
			RangeCopyInfo result = null;
			if (target == null) {
				result = new RangeCopyInfo(sourceRange, target);
				result.ErrorType = ModelErrorType.CopyAreaCannotBeFitIntoThePasteArea;
				return result;
			}
			Analize(target);
			if (targetRangeAndSourceHaveSameSize) {
				result = new RangeCopyInfo(sourceRange, target);
				RegisterTargetRange(result, target);
			}
			else if (targetRangeSmallerThanSource || targetRangeSmallerThanSourceByOneSide) {
				CellRange enlargedTarget = target.GetSubRange(0, 0, sourceRange.Width - 1, sourceRange.Height - 1);
				result = new RangeCopyInfo(sourceRange, enlargedTarget);
				RegisterTargetRange(result, enlargedTarget);
			}
			else if (canSplitByIntegerNumberOfSourceRanges) {
				if (!this.sourceRange.IsColumnRangeInterval() && target.IsColumnRangeInterval()
				 || !this.sourceRange.IsRowRangeInterval() && target.IsRowRangeInterval()) {
					return RegisterFirstSubRangeInTargetRangeEqualsToSource(target);
				}
				else {
					result = new RangeCopyInfo(sourceRange, target);
					SplitByIntegerNumberOfSourceRange(result, target);
				}
			}
			else if (targetRangeContainsNotIntegerNumberOfSourceRanges) {
				return RegisterFirstSubRangeInTargetRangeEqualsToSource(target);
			}
			else if (targetRangeInVerticalSingleCellRange) {
				CellRange enlargedTarget = target.GetSubRange(0, 0, sourceRange.Width - 1, target.Height - 1);
				return ProcessNormalRange(sourceRange, enlargedTarget);
			}
			else if (targetRangeIsHorizontalSingleCellRange) {
				CellRange enlargedTarget = target.GetSubRange(0, 0, target.Width - 1, sourceRange.Height - 1);
				return ProcessNormalRange(sourceRange, enlargedTarget);
			}
			return result;
		}
		RangeCopyInfo RegisterFirstSubRangeInTargetRangeEqualsToSource(CellRange target) {
			CellRange subRangeTargetAsSourceRange = target.GetSubRange(0, 0, SourceRangeWidth - 1, SourceRangeHeight - 1);
			RangeCopyInfo result = new RangeCopyInfo(sourceRange, subRangeTargetAsSourceRange);
			RegisterTargetRange(result, subRangeTargetAsSourceRange);
			return result;
		}
		public static List<CellRange> SplitByIntegerNumberOfSourceRangeByColumns(CellRange target, int byWidth, int byHeight) {
			List<CellRange> result = new List<CellRange>();
			int numberOfSourceRangeWidhts = target.Width / byWidth;
			int numberOfSourceRangeHeights = target.Height / byHeight;
			int startColumn = 0;
			int startRow = 0;
			for (int i = 1; i < numberOfSourceRangeWidhts + 1; i++) {
				for (int j = 1; j < numberOfSourceRangeHeights + 1; j++) {
					startColumn = (i - 1) * byWidth;
					startRow = (j - 1) * byHeight;
					int endColumn = i * byWidth - 1;
					int endRow = j * byHeight - 1;
					CellRange subRange = target.GetSubRange(startColumn, startRow, endColumn, endRow);
					if (subRange == null) {
						throw new InvalidOperationException();
					}
					result.Add(subRange);
				}
			}
			return result;
		}
		void SplitByIntegerNumberOfSourceRange(RangeCopyInfo result, CellRange target) {
			int numberOfSourceRangeWidhts = target.Width / SourceRangeWidth;
			int numberOfSourceRangeHeights = target.Height / SourceRangeHeight;
			int startColumn = 0;
			int startRow = 0;
			for (int j = 1; j < numberOfSourceRangeHeights + 1; j++) {
				for (int i = 1; i < numberOfSourceRangeWidhts + 1; i++) {
					startColumn = (i - 1) * SourceRangeWidth;
					startRow = (j - 1) * SourceRangeHeight;
					int endColumn = i * SourceRangeWidth - 1;
					int endRow = j * SourceRangeHeight - 1;
					CellRange subRange = target.GetSubRange(startColumn, startRow, endColumn, endRow);
					if (subRange == null) {
						result.ErrorType = ModelErrorType.CopyAreaCannotBeFitIntoThePasteArea;
						return;
					}
					result.TargetRanges.Add(subRange);
				}
			}
			result.SelectedRanges.Add(target);
		}
	}
	public interface ISourceObjectRangeInfo {
		bool EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth { get; }
		bool ObjectRangeWiderIfVerticalOrLongerIfHorizontal { get; }
		bool SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz { get; }
		bool ObjectRangeWiderIfVerticalAndLongerIfHorizBothSides { get; }
		bool ObjectRangeWiderOrLongerAtOneSide { get; }
		bool NoGaps { get; }
		Direction ObjectDirection { get; }
	}
	public enum GrowthDirectionOfConjugatedRanges {
		None,
		VerticalContinueBottom,
		VerticalContinueTop,
		HorizontalContinueRight,
		HorizontalContinueLeft
	}
	public enum Direction {
		None,
		Vertical,
		Horizontal,
		Both,
	}
	public enum SourceRangeIntersectsTargetRangeType {
		None,
		SourceRangeIsTheFirst,
		SourceRangeIsTheLast,
		SourceRangeInTheMiddle,
		WithOffet_Case2OrEquals,
	}
	public interface IRangeInfo {
		bool SourceIsIntervalRange { get; }
		bool TargetBigRangeIsIntervalRange { get; }
		bool SourceIsWholeWorksheetRange { get; }
		bool SourceAndTargetEquals { get; }
	}
	public class SourceTargetRangesForCopyInfo {
		readonly List<CellRange> targetRanges;
		readonly List<CellRange> selectedRanges;
		public SourceTargetRangesForCopyInfo() {
			targetRanges = new List<CellRange>();
			selectedRanges = new List<CellRange>();
		}
		public List<CellRange> TargetRanges { get { return targetRanges; } }
		public List<CellRange> SelectedRanges { get { return selectedRanges; } }
		public ModelErrorType ErrorType { get; set; }
	}
}
