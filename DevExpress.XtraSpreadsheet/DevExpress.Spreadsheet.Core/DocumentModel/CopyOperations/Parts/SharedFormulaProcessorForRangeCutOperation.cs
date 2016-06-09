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
	public class SharedFormulaRangeCutCalculator : SharedFormulaRangeCopyCalculator {
		public SharedFormulaRangeCutCalculator(ITargetRangeCalculatorOwner owner, IRangeCopySharedFormulaPart rangeCopy)
			: base(owner, rangeCopy) {
		}
		public override void Execute(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange) {
			if (rangeInfo.SourceAndTargetEquals) {
				RangeCopy.UseExistingSharedFormula(sourceSharedFormula, existingSharedFormulaIndex);
				return;
			}
			CellRange targetObjectRange = Owner.GetTargetObjectRange(sourceSharedFormulaRange, rangeInfo.TargetBigRange) as CellRange;
			CellRange sourceRange = rangeInfo.SourceRange;
			List<CellRange> ranges = RangeCopy.SharedFormulaGetReferencedRangesInSourceWorksheet(sourceSharedFormula, sourceSharedFormulaRange);
			bool rangesNotEmtpy = ranges.Count > 0;
			bool containsRelativeReferences = rangesNotEmtpy && ranges.Exists((r) => (r.TopLeft.IsRelative || r.BottomRight.IsRelative));
			bool referenceIntersectsSourceRange = rangesNotEmtpy && ranges.Exists((r) => sourceRange.Intersects(r));
			bool referenceInsideSR = Owner.IsEqualWorksheets && referenceIntersectsSourceRange && sourceRange.ContainsRange(sourceSharedFormulaRange);
			if (referenceIntersectsSourceRange && !containsRelativeReferences) {
				RangeCopy.RemoveCellsFromAffectedRange(sourceSharedFormula, sourceSharedFormulaRange);
				RangeCopy.RemoveSourceSharedFormula(sourceSharedFormulaRange, existingSharedFormulaIndex);
			}
			else if (!containsRelativeReferences && Owner.IsEqualWorksheets) {
				CellRange newRange = AbsoluteReferencesCase(rangeInfo, sourceSharedFormulaRange, targetObjectRange);
				RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, newRange);
				RangeCopy.UseExistingSharedFormula(sourceSharedFormula, existingSharedFormulaIndex);
			}
			else if (referenceInsideSR) {
				RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, targetObjectRange);
				RangeCopy.UseExistingSharedFormula(sourceSharedFormula, existingSharedFormulaIndex);
			}
			else
				RelativeReferenceCase(sourceSharedFormula, existingSharedFormulaIndex, sourceSharedFormulaRange, rangeInfo);
		}
		CellRange AbsoluteReferencesCase(RangeCopyInfo rangeInfo, CellRange objectRange, CellRange targetObjectRange) {
			CellRangeBase cuttedPart = objectRange.ExcludeRange(rangeInfo.SourceRange);
			CellRange targetRange = rangeInfo.TargetBigRange;
			Predicate<CellRange> insideTargetRange = cuttedPartItem => targetRange.ContainsRange(cuttedPartItem);
			bool ignoreCuttedPart = cuttedPart == null || cuttedPart.ForAll(insideTargetRange);
			CellRange newRange = targetObjectRange;
			if (!ignoreCuttedPart) {
				cuttedPart.ForEach(range => newRange = range.Expand(newRange));
			}
			return newRange;
		}
		void RelativeReferenceCase(SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange, RangeCopyInfo rangeInfo) {
			CellRange sourceRange = rangeInfo.SourceRange;
			VariantValue intersectionResult = sourceSharedFormulaRange.IntersectionWith(sourceRange);
			if (!intersectionResult.IsCellRange) {
				return;
			}
			bool removeSharedformula = !Owner.IsEqualWorksheets;
			CellRangeBase range = removeSharedformula ? sourceSharedFormulaRange : intersectionResult.CellRangeValue;
			RangeCopy.RemoveCellsFromAffectedRange(sourceSharedFormula, range);
			CellRangeBase excluded = sourceSharedFormulaRange.ExcludeRange(range);
			CellUnion excludedUnion = excluded as CellUnion;
			if (sourceRange.ContainsRange(sourceSharedFormulaRange) || removeSharedformula) {
				RangeCopy.RemoveSourceSharedFormula(sourceSharedFormulaRange, existingSharedFormulaIndex);
			}
			else if (excludedUnion != null && excludedUnion.InnerCellRanges.Count > 1) {
				List<CellRangeBase> unionInnerRanges = excludedUnion.InnerCellRanges;
				CellRangeBase newSFRangeUnion = excludedUnion.ExcludeRange(rangeInfo.TargetBigRange);
				CellRange newSFRange = newSFRangeUnion.GetCoveredRange();
				RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, newSFRange);
			}
			else if (excluded != null && excluded.RangeType == CellRangeType.SingleRange) {
				CellRange excludedasCellRange = excluded.GetCoveredRange();
				CellRange intersectionWithTarget = excludedasCellRange.Intersection(rangeInfo.TargetBigRange);
				CellRangeBase excludedTarget = excludedasCellRange.ExcludeRange(rangeInfo.TargetBigRange);
				if (intersectionWithTarget != null)
					RangeCopy.RemoveCellsFromAffectedRange(sourceSharedFormula, intersectionWithTarget);
				if (excludedTarget == null)
					RangeCopy.RemoveSourceSharedFormula(sourceSharedFormulaRange, existingSharedFormulaIndex);
				else
					RangeCopy.ModifyExistingSharedFormulaRange(sourceSharedFormula, excludedTarget.GetCoveredRange());
			}
		}
	}
}
