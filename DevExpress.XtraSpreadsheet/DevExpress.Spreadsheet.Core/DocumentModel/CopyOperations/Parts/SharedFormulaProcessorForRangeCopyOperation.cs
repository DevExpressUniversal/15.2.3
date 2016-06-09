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
using IntersectionType = DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceRangeIntersectsTargetRangeType;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class SharedFormulaRangeCopyCalculator {
		ITargetRangeCalculatorOwner owner;
		IRangeCopySharedFormulaPart rangeCopy;
		public SharedFormulaRangeCopyCalculator(ITargetRangeCalculatorOwner owner, IRangeCopySharedFormulaPart rangeCopy) {
			this.owner = owner;
			this.rangeCopy = rangeCopy;
		}
		protected ITargetRangeCalculatorOwner Owner { get { return owner; } }
		protected IRangeCopySharedFormulaPart RangeCopy { get { return rangeCopy; } }
		public virtual void Execute(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange) {
			bool useExistingSharedFormula = !ShouldCreateNewSharedFormula(rangeInfo, sourceSharedFormulaRange);
			if (useExistingSharedFormula) {
				CellRange targetObjectRangeExpanded = 
					GetTargetObjectRangeExpandedForTargetSharedFormula(rangeInfo, sourceSharedFormulaRange, excludeFirstLast : true); 
				CellRange torWithCroppedPart = targetObjectRangeExpanded.Expand(sourceSharedFormulaRange);
				CellRange modifiedRange = torWithCroppedPart;
				if (rangeInfo.IntersectionType == IntersectionType.WithOffet_Case2OrEquals && !rangeInfo.SourceAndTargetEquals) {
					modifiedRange = CropTargetObjectRangeExpandedToSourceObjectRangeWhenSourceTargetRangeIntersectsWithOffset(rangeInfo, torWithCroppedPart, sourceSharedFormulaRange);
				}
				RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, modifiedRange);
				RangeCopy.UseExistingSharedFormula(sourceSharedFormula, existingSharedFormulaIndex);
			}
			else {
				if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.SourceRangeInTheMiddle) { 
				}
				CellRange tore = GetTargetObjectRangeExpandedForTargetSharedFormula(rangeInfo, sourceSharedFormulaRange, excludeFirstLast : false);
				bool shouldExcludeFirstLastTargetObjectRange = tore.ExcludeRange(sourceSharedFormulaRange) is CellRange;
				CellRange targetSharedFormulaRange = tore;
				if(shouldExcludeFirstLastTargetObjectRange) 
					targetSharedFormulaRange = GetTargetObjectRangeExpandedForTargetSharedFormula(rangeInfo, sourceSharedFormulaRange, excludeFirstLast: true);
				CellRange modifiedSourceSharedFormulaRange = 
					CalculateModifiedSourceSharedFormulaRangeOrNull(rangeInfo, sourceSharedFormulaRange, targetSharedFormulaRange);
				if (modifiedSourceSharedFormulaRange != null)
					RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, modifiedSourceSharedFormulaRange);
				RangeCopy.CreateTargetSharedFormula(sourceSharedFormula, targetSharedFormulaRange, rangeInfo.TargetBigRange);
			}
		}
		CellRange CropTargetObjectRangeExpandedToSourceObjectRangeWhenSourceTargetRangeIntersectsWithOffset(RangeCopyInfo rangeInfo, CellRange torWithCroppedPart, CellRange sourceSharedFormulaRange) {
			Debug.Assert(rangeInfo.IntersectionType == IntersectionType.WithOffet_Case2OrEquals && !rangeInfo.SourceAndTargetEquals, "condition", "");
			CellRange TR = rangeInfo.TargetBigRange;
			CellRange SR = rangeInfo.SourceRange;
			CellRangeBase emptySpace = SR.ExcludeRange(TR);
			if (emptySpace == null) {
				return torWithCroppedPart;
			}
			CellRangeBase emptySpaceWithouSourceObject = emptySpace.ExcludeRange(sourceSharedFormulaRange);
			if(emptySpaceWithouSourceObject == null)
				return torWithCroppedPart;
			CellRangeBase shifted = Owner.GetTargetObjectRange(emptySpaceWithouSourceObject, TR);
			CellRangeBase finalObjectRangeUnion = torWithCroppedPart.ExcludeRange(shifted);
			if (finalObjectRangeUnion == null) {
				return torWithCroppedPart;
			}
			return finalObjectRangeUnion.GetCoveredRange();
		}
		CellRange CalculateModifiedSourceSharedFormulaRangeOrNull(RangeCopyInfo rangeInfo, CellRange sourceSharedFormulaRange, CellRange targetSharedFormulaRange) {
			Debug.Assert(sourceSharedFormulaRange != null);
			bool whenTargetContainsSourceRange =
				rangeInfo.IntersectionType == IntersectionType.SourceRangeIsTheFirst
				|| rangeInfo.IntersectionType == IntersectionType.SourceRangeIsTheLast
				|| rangeInfo.IntersectionType == IntersectionType.SourceRangeInTheMiddle;
			bool whenSoRNotExceedTRBorders = rangeInfo.TargetBigRange.Includes(sourceSharedFormulaRange); 
			CellRange sorInsideSR = rangeInfo.SourceRange.Intersection(sourceSharedFormulaRange);
			CellRange intersectionWithSor = targetSharedFormulaRange.Intersection(sourceSharedFormulaRange);
			if (rangeInfo.IntersectionType == IntersectionType.None
				|| (whenTargetContainsSourceRange && whenSoRNotExceedTRBorders)) {
				CellRange result = null;
				if (intersectionWithSor != null) {
					CellRange sorCroppedByToR = sourceSharedFormulaRange.ExcludeRange(targetSharedFormulaRange) as CellRange;
					bool croppedPartIsSingleRange = sorCroppedByToR != null;
					if (croppedPartIsSingleRange)
						result = sorCroppedByToR;
				}
				CellRangeBase cuttedParts = sourceSharedFormulaRange.ExcludeRange(sorInsideSR);
				if (cuttedParts == null)
					return null;
				if (rangeInfo.TargetBigRange.Includes(cuttedParts))
					result = sorInsideSR;
				return result;
			}
			if (rangeInfo.IntersectionType == IntersectionType.SourceRangeInTheMiddle) {
				CellRangeBase cuttedParts2 = sourceSharedFormulaRange.ExcludeRange(sorInsideSR);
				CellRangeBase cuttedPartOutsideTore = cuttedParts2.ExcludeRange(targetSharedFormulaRange);
				CellRangeBase sorInsideSRWithCuttedPartUnion = CellUnion.Combine(sorInsideSR, cuttedPartOutsideTore);
				return sorInsideSRWithCuttedPartUnion.GetCoveredRange();
			}
			return null;
		}
		bool ShouldCreateNewSharedFormula(RangeCopyInfo targetRangeHas, CellRange sourceObjectRange) {
			if (!Owner.IsEqualWorksheets)
				return true;
			IntersectionType intersectionType = targetRangeHas.IntersectionType;
			if (intersectionType == IntersectionType.WithOffet_Case2OrEquals)
				return false;
			CellRange targetObjectRangeExpanded = GetTargetObjectRangeExpandedForTargetSharedFormula(targetRangeHas, sourceObjectRange, excludeFirstLast: true);
			CellRange SoR = sourceObjectRange;
			CellRange TORE = targetObjectRangeExpanded;
			return ShouldCreateNewSharedFormulaExtracted(targetRangeHas, SoR, TORE);
		}
		bool ShouldCreateNewSharedFormulaExtracted(RangeCopyInfo targetRangeHas, CellRange sourceObjectRange, CellRange TORE) {
			CellRange SR = targetRangeHas.SourceRange;
			CellRange TR = targetRangeHas.TargetBigRange;
			SourceTargetRangeMutualArrangementTest test = new SourceTargetRangeMutualArrangementTest(SR, TR);
			ISourceObjectRangeInfo objectRangeHas = targetRangeHas.CalculateSourceObjectRangeInfo(sourceObjectRange);
			bool sorHasGapsInHorizontalDirection = objectRangeHas.ObjectDirection == Direction.Vertical || objectRangeHas.ObjectDirection == Direction.None;
			bool sorHasGapsInVerticalDirection = objectRangeHas.ObjectDirection == Direction.Horizontal || objectRangeHas.ObjectDirection == Direction.None;
			bool conjugatedWithToRE = targetRangeHas.RangesIsConjugatedSharedFormulaCase(SR, TORE) || targetRangeHas.RangesIsConjugatedOrIntersected(sourceObjectRange, TORE);
			bool conditionForLeft = sorHasGapsInHorizontalDirection
				|| (sourceObjectRange.TopRowIndex < TR.TopRowIndex && TR.TopRowIndex >= SR.TopRowIndex); 
			bool conditionForBottom = !conjugatedWithToRE;
			bool conditionForTop = sorHasGapsInVerticalDirection || (sourceObjectRange.LeftColumnIndex < SR.LeftColumnIndex && TR.LeftColumnIndex >= SR.LeftColumnIndex); 
			bool conditionForRight = !conjugatedWithToRE;
			if (test.LeftTest && test.TopTest) {
				if (!TR.ContainsRange(SR)) {
					if (!sorHasGapsInHorizontalDirection && !sorHasGapsInVerticalDirection)
						return false;
					return true; 
				}
			}
			bool result = true;
			result &= test.LeftTest ? conditionForLeft : result;
			result &= test.BottomTest ? conditionForBottom : result;
			result &= test.TopTest ? conditionForTop : result;
			result &= test.RightTest ? conditionForRight : result;
			return result;
		}
		CellRange GetTargetObjectRangeExpandedForTargetSharedFormula(RangeCopyInfo targetRangeHas, CellRange sourceObjectRange, bool excludeFirstLast) {
			int first = 0;
			int last = targetRangeHas.TargetRanges.Count - 1;
			if (excludeFirstLast) {
				if (targetRangeHas.IntersectionType == IntersectionType.SourceRangeIsTheFirst) {
					first++; 
				}
				else if (targetRangeHas.IntersectionType == IntersectionType.SourceRangeIsTheLast) {
					last--;
				}
			}
			CellRange firstCurrentTargetRange = targetRangeHas.TargetRanges[first];
			CellRange lastCurrentTargetRange = targetRangeHas.TargetRanges[last];
			CellRange firstTargetObjectRange = Owner.GetTargetObjectRange(sourceObjectRange, firstCurrentTargetRange) as CellRange;
			CellRange lastTargetObjectRange = Owner.GetTargetObjectRange(sourceObjectRange, lastCurrentTargetRange) as CellRange;
			CellRange targetSharedFormulaRange = firstTargetObjectRange.Expand(lastTargetObjectRange);
			return targetSharedFormulaRange;
		}
		#region SourceTargetRangeMutualArrangementTest
		struct SourceTargetRangeMutualArrangementTest {
			public bool LeftTest { get; set; }
			public bool BottomTest { get; set; }
			public bool TopTest { get; set; }
			public bool RightTest { get; set; }
			public SourceTargetRangeMutualArrangementTest(CellRange SR, CellRange TR)
				: this() {
				LeftTest = TR.LeftColumnIndex < SR.LeftColumnIndex &&
			   TR.RightColumnIndex >= SR.LeftColumnIndex - 1 && SR.LeftColumnIndex > 0;
				BottomTest = TR.TopRowIndex >= SR.TopRowIndex &&
					TR.BottomRowIndex >= SR.BottomRowIndex + 1
					&& TR.LeftColumnIndex >= SR.LeftColumnIndex;
				TopTest = TR.TopRowIndex < SR.TopRowIndex &&
					TR.BottomRowIndex >= SR.TopRowIndex - 1 &&
					TR.LeftColumnIndex <= SR.LeftColumnIndex; 
				RightTest = TR.LeftColumnIndex >= SR.LeftColumnIndex && TR.LeftColumnIndex <= SR.RightColumnIndex + 1 &&
					TR.RightColumnIndex >= SR.RightColumnIndex + 1
					&& TR.BottomRowIndex > SR.TopRowIndex &&
					TR.TopRowIndex >= SR.TopRowIndex;
			}
			public override string ToString() {
				return String.Concat("T L R B: ", TopTest.ToString(), " ", LeftTest.ToString(), " ", RightTest.ToString(), " ", BottomTest.ToString());
			}
		} 
		#endregion
		#region notes
		#endregion
	}
}
