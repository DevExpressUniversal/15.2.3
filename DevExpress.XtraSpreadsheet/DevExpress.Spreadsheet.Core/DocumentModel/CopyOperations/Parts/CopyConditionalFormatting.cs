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

using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public interface ITargetRangeCalculatorOwner {
		CellRangeBase GetTargetObjectRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange);
		CellPositionOffset GetTargetRangeOffset(CellRange currentTargetRange);
		CellRangeBase GetRangeToClearAfterCut();
		bool IsEqualWorksheets { get; }
	}
	public class ConditionalFormattingProcessorForRangeCopyOperation {
		ITargetRangeCalculatorOwner owner;
		IRangeCopyConditionalFormattingPart rangeCopy;
		public ConditionalFormattingProcessorForRangeCopyOperation(ITargetRangeCalculatorOwner owner, IRangeCopyConditionalFormattingPart rangeCopy) {
			this.owner = owner;
			this.rangeCopy = rangeCopy;
		}
		protected ITargetRangeCalculatorOwner Owner { get { return owner; } }
		protected IRangeCopyConditionalFormattingPart RangeCopy { get { return rangeCopy; } }
		public virtual void Execute(RangeCopyInfo rangeInfo, ConditionalFormatting sourceConditionalFormatting) {
			CellRangeBase sourceCfRange = sourceConditionalFormatting.CellRange;
			CellRangeBase newCfRange = null;
			CellRangeBase SORinsideSR = sourceCfRange.IntersectionWith(rangeInfo.SourceRange).CellRangeValue;
			CellRangeBase tor = GetUnionOfFunctionResult(rangeInfo, SORinsideSR, GetRangeForNewConditionalFormatting, false);
			bool shouldModifySourceConditionalFormatting = rangeInfo.SourceAndTargetAreIntersected;
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
				&& !rangeInfo.SourceAndTargetEquals) {
				bool sorIntersectsTor = sourceCfRange.Intersects(tor);
				shouldModifySourceConditionalFormatting = sorIntersectsTor;
			}
			if (shouldModifySourceConditionalFormatting) {
				newCfRange = GetUnionOfFunctionResult(rangeInfo, sourceCfRange, ModifyExistingConditionalFormattingRange, true);
				RangeCopy.UseSourceConditionalFormatting(sourceConditionalFormatting, newCfRange, rangeInfo.TargetBigRange);
			}
			else {
				newCfRange = tor;
				RangeCopy.CreateTargetConditionalFormatting(rangeInfo.TargetBigRange, newCfRange, sourceConditionalFormatting);
			}
		}
		CellRangeBase ModifyExistingConditionalFormattingRange(RangeCopyInfo targetRangeHas, CellRange sourceObjectRange) {
			CellRange SourceRange = targetRangeHas.SourceRange;
			CellRange targetBigRange = targetRangeHas.TargetBigRange;
			if (targetRangeHas.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals)
				return UnionSourceObjectRangeWithShiftedAndCroppedTargetRange(sourceObjectRange, targetRangeHas);
			ISourceObjectRangeInfo objectRangeHas = targetRangeHas.CalculateSourceObjectRangeInfo(sourceObjectRange);
			if (objectRangeHas.NoGaps) {
				if (objectRangeHas.EqualsWidthIfVerticalOrEqualsHeightIfHorizontalOrEqualsIfBoth) {
					CellRangeBase cfRangeWithEnlargedVerticalDirection = sourceObjectRange.Expand(targetBigRange);
					return cfRangeWithEnlargedVerticalDirection;
				}
				else if (objectRangeHas.ObjectRangeWiderIfVerticalOrLongerIfHorizontal) {
					return CellUnion.Combine(sourceObjectRange, targetBigRange); 
				}
				else if (targetRangeHas.Direction == Direction.Both) {
					if (targetBigRange.Includes(sourceObjectRange)) 
						return sourceObjectRange.Expand(targetBigRange);
					else
						return CellUnion.Combine(sourceObjectRange, targetBigRange); 
				}
				throw new InvalidOperationException();
			}
			bool noGapsInTargetRangeDirection = objectRangeHas.ObjectDirection == targetRangeHas.Direction && !objectRangeHas.NoGaps;
			if (noGapsInTargetRangeDirection) {
				if (objectRangeHas.SourceObjectRangeNotWiderSourceRangeInVerticalDirectionOrNotLongerIfHoriz) 
					return ContinueObjectRangeBySameDirection(sourceObjectRange, targetBigRange, objectRangeHas);
				else
					return UnionSourceObjectRangeWithShiftedAndCroppedTargetRange(sourceObjectRange, targetRangeHas);
			}
			else {
				List<CellRangeBase> targetObjectRanges = ConvertTargetRangesToListOfTargetObjectRanges(targetRangeHas, sourceObjectRange, objectRangeHas.ObjectDirection);
				return CombineSourceCfRangeWithSomeRangesFromList(targetRangeHas, sourceObjectRange, targetObjectRanges, objectRangeHas);
			}
		}
		CellRangeBase GetRangeForNewConditionalFormatting(RangeCopyInfo rangeInfo, CellRange sourceCfCellRange) {
			CellRange targetBigRange = rangeInfo.TargetBigRange; 
			Direction objectDirection = rangeInfo.CalculateObjectRangeDirectionInSourceRange(sourceCfCellRange);
			if (objectDirection == Direction.Both) {
				return targetBigRange; 
			}
			List<CellRangeBase> targetObjectRanges = ConvertTargetRangesToListOfTargetObjectRanges(rangeInfo,
				sourceCfCellRange, objectDirection);
			if (targetObjectRanges.Count == 1)
				return targetObjectRanges[0];
			return new CellUnion(targetObjectRanges); 
		}
		CellRangeBase UnionSourceObjectRangeWithShiftedAndCroppedTargetRange(CellRange sourceObjectRange, RangeCopyInfo targetRangeHas) {
			CellRange anyCurrentTargetRange = targetRangeHas.TargetRanges[0];
			CellRange sourceCFRangeCroppedAndShifted = owner.GetTargetObjectRange(sourceObjectRange, anyCurrentTargetRange) as CellRange;
			CellRangeBase sourceCFRangeCroppedAndShiftedAndEnlarged = GetTargetObjectRangeStretchedToBigTargetRange(targetRangeHas, sourceCFRangeCroppedAndShifted);
			if (!sourceObjectRange.Includes(sourceCFRangeCroppedAndShiftedAndEnlarged))
				return CellUnion.Combine(sourceObjectRange, sourceCFRangeCroppedAndShiftedAndEnlarged);
			return sourceObjectRange;
		}
		CellRangeBase ContinueObjectRangeBySameDirection(CellRange sourceCfCellRange, CellRange targetBigRange, ISourceObjectRangeInfo info) {
			Direction objectDirection = info.ObjectDirection;
			CellRangeBase continuedSourceCFRange = sourceCfCellRange;
			if (objectDirection == Direction.Vertical) { 
				int topRowIndex = Math.Min(sourceCfCellRange.TopRowIndex, targetBigRange.TopRowIndex);
				int bottomRowIndex = Math.Max(sourceCfCellRange.BottomRowIndex, targetBigRange.BottomRowIndex);
				int startRowAddition = topRowIndex - sourceCfCellRange.TopRowIndex;
				int endRowAddition = bottomRowIndex - sourceCfCellRange.BottomRowIndex;
				continuedSourceCFRange = sourceCfCellRange.GetResized(0, startRowAddition, 0, endRowAddition); 
			}
			else if (objectDirection == Direction.Horizontal) {
				int leftColumnIndex = Math.Min(sourceCfCellRange.LeftColumnIndex, targetBigRange.LeftColumnIndex);
				int rightColumnIndex = Math.Max(sourceCfCellRange.RightColumnIndex, targetBigRange.RightColumnIndex);
				int startColumnAddition = leftColumnIndex - sourceCfCellRange.LeftColumnIndex;
				int endColumnAddition = rightColumnIndex - sourceCfCellRange.RightColumnIndex;
				continuedSourceCFRange = sourceCfCellRange.GetResized(startColumnAddition, 0, endColumnAddition, 0); 
			}
			return continuedSourceCFRange;
		}
		List<CellRange> GetTargetRangesSplittedByDirection(Direction objectDirection, RangeCopyInfo rangeInfo) {
			List<CellRange> targetRangesSplittedByDirection = null;
			if (objectDirection == Direction.Horizontal || objectDirection == Direction.Vertical) 
				targetRangesSplittedByDirection = rangeInfo.SplitTargetRangeBy(objectDirection);
			else
				targetRangesSplittedByDirection = SourceTargetRangeForCopyCalculator.SplitByIntegerNumberOfSourceRangeByColumns(rangeInfo.TargetBigRange, rangeInfo.SourceRange.Width, rangeInfo.SourceRange.Height);
			return targetRangesSplittedByDirection;
		}
		List<CellRangeBase> ConvertTargetRangesToListOfTargetObjectRanges(RangeCopyInfo rangeInfo, CellRangeBase sourceCfRange, Direction objectDirection) {
			List<CellRange> targetRangesSplittedByDirection = GetTargetRangesSplittedByDirection(objectDirection, rangeInfo);
			List<CellRangeBase> targetObjectRanges = new List<CellRangeBase>();
			foreach (var targetRange in targetRangesSplittedByDirection) {
				CellRangeBase targetObjectRange = owner.GetTargetObjectRange(sourceCfRange, targetRange);
				CellRangeBase targetObjectRangeStetched = GetTargetObjectRangeStretchedToBigTargetRange(targetObjectRange, targetRange, objectDirection);
				targetObjectRanges.Add(targetObjectRangeStetched);
			}
			return targetObjectRanges;
		}
		CellUnion CombineSourceCfRangeWithSomeRangesFromList(RangeCopyInfo rangeInfo, CellRange sourceCfCellRange, List<CellRangeBase> targetObjectRanges, ISourceObjectRangeInfo sourceObjectRangeHas) {
			CombineSourceCfRangeWithSomeRangesFromListMiddleCase(rangeInfo, sourceCfCellRange, targetObjectRanges);
			return new CellUnion(targetObjectRanges);
		}
		void CombineSourceCfRangeWithSomeRangesFromListMiddleCase(RangeCopyInfo rangeInfo, CellRange sourceCfCellRange, List<CellRangeBase> targetObjectRanges) {
			int targetObjectRangeIndexUnderSourceObjectRange = targetObjectRanges.FindIndex(tor => sourceCfCellRange.Includes(tor));
			int targetObjectRangeIndexIntersectsSourceObjectRange = targetObjectRanges.FindIndex(tor => sourceCfCellRange.Intersects(tor));
			bool shouldAddSourceObjectRangeToList = true;
			int targetObjectRangeIndex = targetObjectRangeIndexUnderSourceObjectRange;
			bool replaced = ReplaceTargetObjectRangeUnderSourceObjectRangeWithSourceObjectRange(sourceCfCellRange, targetObjectRanges, targetObjectRangeIndexUnderSourceObjectRange);
			if (!replaced && ReplaceTargetObjectRangeUnderSourceObjectRangeWithExpandedSourceObjectRange(sourceCfCellRange, targetObjectRanges, targetObjectRangeIndexIntersectsSourceObjectRange)) {
				replaced = true;
				targetObjectRangeIndex = targetObjectRangeIndexIntersectsSourceObjectRange;
			}
			int conjugatedTargetObjectRangeIndex = GetConjugatedTargetObjectRangeWithSourceObjectRangeIndex(rangeInfo, sourceCfCellRange, targetObjectRanges, targetObjectRangeIndex);
			shouldAddSourceObjectRangeToList &= !replaced;
			shouldAddSourceObjectRangeToList &= !TryReplaceConjugated(conjugatedTargetObjectRangeIndex, targetObjectRanges, targetObjectRangeIndex);
			if (shouldAddSourceObjectRangeToList) {
				targetObjectRanges.Insert(0, sourceCfCellRange);
			}
		}
		bool TryReplaceConjugated(int conjugatedTargetObjectRangeIndex, List<CellRangeBase> targetObjectRanges, int targetObjectRangeIndexUnderSourceObjectRange) {
			if (conjugatedTargetObjectRangeIndex < 0)
				return false;
			CellRange underSource = targetObjectRanges[targetObjectRangeIndexUnderSourceObjectRange] as CellRange;
			CellRange conjugated = targetObjectRanges[conjugatedTargetObjectRangeIndex] as CellRange;
			CellRange newOne = underSource.Expand(conjugated);
			if (conjugatedTargetObjectRangeIndex > targetObjectRangeIndexUnderSourceObjectRange) { 
				targetObjectRanges[conjugatedTargetObjectRangeIndex] = newOne;
				targetObjectRanges.RemoveAt(targetObjectRangeIndexUnderSourceObjectRange);
			}
			else {
				targetObjectRanges[targetObjectRangeIndexUnderSourceObjectRange] = newOne;
				targetObjectRanges.RemoveAt(conjugatedTargetObjectRangeIndex);
			}
			return true;
		}
		bool ReplaceTargetObjectRangeUnderSourceObjectRangeWithSourceObjectRange(CellRange sourceCfCellRange, List<CellRangeBase> targetObjectRanges, int targetObjectRangeIndexUnderSourceObjectRange) {
			if (targetObjectRangeIndexUnderSourceObjectRange < 0)
				return false;
			CellRange underSource = targetObjectRanges[targetObjectRangeIndexUnderSourceObjectRange] as CellRange;
			if (sourceCfCellRange.Includes(underSource)) {
				targetObjectRanges[targetObjectRangeIndexUnderSourceObjectRange] = sourceCfCellRange;
				return true;
			}
			return false;
		}
		bool ReplaceTargetObjectRangeUnderSourceObjectRangeWithExpandedSourceObjectRange(CellRange sourceCfCellRange, List<CellRangeBase> targetObjectRanges, int targetObjectRangeIndexIntersectedSourceObjectRange) {
			if (targetObjectRangeIndexIntersectedSourceObjectRange < 0)
				return false;
			CellRange underSource = targetObjectRanges[targetObjectRangeIndexIntersectedSourceObjectRange] as CellRange;
			Direction targetObjectRangeDirection = SourceTargetRangeForCopyCalculator.CalculateRangeDirection(sourceCfCellRange, underSource);
			if (IsSuitableToReplaceWithExpanded(sourceCfCellRange, underSource, targetObjectRangeDirection)) { 
				targetObjectRanges[targetObjectRangeIndexIntersectedSourceObjectRange] = sourceCfCellRange.Expand(underSource);
				return true;
			}
			return false;
		}
		bool IsSuitableToReplaceWithExpanded(CellRange sourceCfCellRange, CellRange second, Direction direction) {
			CellRange expanded = sourceCfCellRange.Expand(second);
			if (direction == Direction.Vertical)
				return (expanded.LeftColumnIndex == sourceCfCellRange.LeftColumnIndex
					&& expanded.LeftColumnIndex == second.LeftColumnIndex
					&& expanded.RightColumnIndex == sourceCfCellRange.RightColumnIndex
					&& expanded.RightColumnIndex == second.RightColumnIndex);
			if (direction == Direction.Horizontal)
				return (expanded.TopRowIndex == sourceCfCellRange.TopRowIndex
					&& expanded.TopRowIndex == second.TopRowIndex
						&& expanded.BottomRowIndex == sourceCfCellRange.BottomRowIndex
						&& expanded.BottomRowIndex == second.BottomRowIndex);
			return false;
		}
		int GetConjugatedTargetObjectRangeWithSourceObjectRangeIndex(RangeCopyInfo rangeInfo, CellRange sourceCfCellRange, List<CellRangeBase> targetObjectRanges, int targetObjectRangeIndexUnderSourceObjectRange) {
			if (targetObjectRangeIndexUnderSourceObjectRange < 0) 
				return Int32.MinValue;
			CellRange targetObjectRangeUnderSourceObjectRange = targetObjectRanges[targetObjectRangeIndexUnderSourceObjectRange] as CellRange;
			List<int> conjugatedTargetObjectRangedWithSourceObjectRange = new List<int>();
			Direction targetObjectRangeDirection = Direction.None;
			if (rangeInfo.Direction == Direction.Vertical || rangeInfo.Direction == Direction.Horizontal) {
				targetObjectRangeDirection = rangeInfo.Direction;
				if (targetObjectRangeIndexUnderSourceObjectRange != targetObjectRanges.Count - 1)
					conjugatedTargetObjectRangedWithSourceObjectRange.Add(targetObjectRangeIndexUnderSourceObjectRange + 1);
				if (targetObjectRangeIndexUnderSourceObjectRange > 0)
					conjugatedTargetObjectRangedWithSourceObjectRange.Add(targetObjectRangeIndexUnderSourceObjectRange - 1);
			}
			else if (rangeInfo.Direction == Direction.Both) {
				CellRange expanded = RangeCopyInfo.GetExpandedByOneCellCellRange(sourceCfCellRange);
				System.Diagnostics.Debug.Assert(expanded != null);
				int foundIndexOfConjugated = targetObjectRanges.FindIndex(
					range => range.Intersects(expanded)
						&& !expanded.Includes(range)); 
				if (foundIndexOfConjugated >= 0 && foundIndexOfConjugated != targetObjectRangeIndexUnderSourceObjectRange) { 
					conjugatedTargetObjectRangedWithSourceObjectRange.Add(foundIndexOfConjugated);
					CellRange foundConjugatedCellRange = targetObjectRanges[foundIndexOfConjugated] as CellRange;
					targetObjectRangeDirection = SourceTargetRangeForCopyCalculator.CalculateRangeDirection(targetObjectRangeUnderSourceObjectRange, foundConjugatedCellRange);
				}
			}
			if (conjugatedTargetObjectRangedWithSourceObjectRange.Count == 0)
				return Int32.MinValue;
			for (int i = 0; i < conjugatedTargetObjectRangedWithSourceObjectRange.Count; i++) {
				int index = conjugatedTargetObjectRangedWithSourceObjectRange[i];
				CellRange conjugated = (targetObjectRanges[index]) as CellRange;
				bool isConj = RangesAreConjugated(sourceCfCellRange, conjugated) || RangesAreConjugated(conjugated, sourceCfCellRange)
					|| conjugated.Intersects(sourceCfCellRange); 
				if (isConj && IsSuitableToReplaceWithExpanded(sourceCfCellRange, conjugated, targetObjectRangeDirection))
					return index;
			}
			return Int32.MinValue;
		}
		bool RangesAreConjugated(CellRange first, CellRange second) {
			var growthDirection = SourceTargetRangeForCopyCalculator.CalcGrowthDirection(first, second, Direction.Horizontal);
			if (growthDirection == GrowthDirectionOfConjugatedRanges.None)
				growthDirection = SourceTargetRangeForCopyCalculator.CalcGrowthDirection(first, second, Direction.Vertical);
			return growthDirection != GrowthDirectionOfConjugatedRanges.None;
		}
		CellRangeBase GetUnionOfFunctionResult(RangeCopyInfo rangeInfo, CellRangeBase sourceCFRange, Func<RangeCopyInfo, CellRange, CellRangeBase> function, bool includeOtherSourceObjectItems) {
			CellRange sourceCfCellRange = sourceCFRange as CellRange;
			CellUnion sourceCFUnionRange = sourceCFRange as CellUnion;
			if (sourceCfCellRange != null)
				return function(rangeInfo, sourceCfCellRange);
			List<CellRangeBase> targetObjectRanges = new List<CellRangeBase>();
			foreach (CellRangeBase innerSourceObjectRangeBase in sourceCFUnionRange.InnerCellRanges) {
				CellRange innerCellRange = innerSourceObjectRangeBase as CellRange;
				if (innerCellRange == null)
					Exceptions.ThrowInternalException();
				if (rangeInfo.SourceRange.Intersects(innerCellRange)) {
					CellRangeBase functionResult = function(rangeInfo, innerCellRange);
					if (!rangeInfo.SourceRange.ContainsRange(innerCellRange)) 
						targetObjectRanges.Add(innerCellRange);
					CellUnion functionResultAsUnion = functionResult as CellUnion;
					if (functionResultAsUnion != null)
						targetObjectRanges.AddRange(functionResultAsUnion.InnerCellRanges);
					else
						targetObjectRanges.Add(functionResult);
				}
				else if (includeOtherSourceObjectItems)
					targetObjectRanges.Add(innerCellRange);
			}
			CellUnion unmerged = new CellUnion(targetObjectRanges);
			CellRangeBase merged = rangeInfo.TryConsolidateRangesInCellUnion(unmerged);
			return merged;
		}
		CellRangeBase GetTargetObjectRangeStretchedToBigTargetRange(RangeCopyInfo rangeCopyInfo, CellRangeBase targetObjectRange) {
			return GetTargetObjectRangeStretchedToBigTargetRange(targetObjectRange, rangeCopyInfo.TargetBigRange, rangeCopyInfo.Direction);
		}
		CellRangeBase GetTargetObjectRangeStretchedToBigTargetRange(CellRangeBase targetObjectRange, CellRange _targetRange, Direction _direction) {
			CellRange targetObjectCellRange = targetObjectRange as CellRange;
			if (targetObjectCellRange == null)
				return targetObjectRange;
			CellRange result = targetObjectCellRange;
			int widthDelta = _targetRange.Width - targetObjectRange.Width;
			int heightDelta = _targetRange.Height - targetObjectRange.Height;
			if (widthDelta == 0 && heightDelta == 0)
				return targetObjectRange;
			if (_direction == Direction.Vertical) {
				System.Diagnostics.Debug.Assert(heightDelta >= 0);
				result = targetObjectCellRange.GetResized(0, 0, 0, heightDelta); 
			}
			else if (_direction == Direction.Horizontal) {
				System.Diagnostics.Debug.Assert(widthDelta >= 0);
				result = targetObjectCellRange.GetResized(0, 0, widthDelta, 0); 
			}
			else if (_direction == CopyOperation.Direction.Both) {
				System.Diagnostics.Debug.Assert(heightDelta >= 0);
				System.Diagnostics.Debug.Assert(widthDelta >= 0);
				result = targetObjectCellRange.GetResized(0, 0, widthDelta, heightDelta); 
			}
			return result;
		}
	}
}
