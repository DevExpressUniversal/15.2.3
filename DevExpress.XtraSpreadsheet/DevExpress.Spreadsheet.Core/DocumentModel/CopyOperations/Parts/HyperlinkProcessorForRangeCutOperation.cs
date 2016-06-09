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
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class SourceHyperlinkProcessorForRangeCutOperation : SourceHyperlinkProcessorForRangeCopyOperation {
		public SourceHyperlinkProcessorForRangeCutOperation(ITargetRangeCalculatorOwner rangeCopyOperation, IRangeCopyHyperlinkPart rangeCopy)
			: base(rangeCopyOperation, rangeCopy) {
		}
		public override void Execute(RangeCopyInfo rangeInfo, ModelHyperlink sourceHyperlink) {
			if (!Owner.IsEqualWorksheets) {
				base.Execute(rangeInfo, sourceHyperlink);
				return;
			}
			CellRange sourceHyperlinkRange = sourceHyperlink.Range;
			CellRange targetRange = rangeInfo.TargetBigRange;
			CellRange targetHyperlinkRange = Owner.GetTargetObjectRange(sourceHyperlinkRange, targetRange) as CellRange;
			if (rangeInfo.SourceAndTargetEquals)
				return;
			CellRange sourceRange = rangeInfo.SourceRange;
			CellRange intersectionTargetSourceRange = sourceRange.Intersection(targetRange);
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
				&& sourceRange.Includes(sourceHyperlinkRange)) {
				RangeCopy.ModifySourceHyperlinkRange(sourceHyperlink, targetHyperlinkRange);
				if (intersectionTargetSourceRange != null && intersectionTargetSourceRange.Includes(sourceHyperlinkRange)) {
					RangeCopy.RetrieveSourceHyperlinkRange(sourceHyperlink);
				}
				return;
			}
			if (rangeInfo.ObjectRangeIntersectsRangeBorder(sourceRange, sourceHyperlinkRange)) {
				CellRangeBase cuttedPart = sourceHyperlinkRange.ExcludeRange(sourceRange);
				if (cuttedPart.RangeType == CellRangeType.UnionRange && rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.None)
					CuttedPartIsUnionCase(sourceHyperlink, sourceHyperlinkRange, targetRange, cuttedPart);
				else
					CuttedPartIsCellRangeCase(rangeInfo, sourceHyperlink, targetHyperlinkRange, cuttedPart);
				return;
			}
			RangeCopy.CreateTargetHyperlink(targetHyperlinkRange, sourceHyperlink);
		}
		void CuttedPartIsUnionCase(ModelHyperlink sourceHyperlink, CellRange sourceHyperlinkRange, CellRange targetRange, CellRangeBase cuttedPart) {
			CellUnion cuttedPartAsUnion = cuttedPart as CellUnion;
			int indexOfInnerRangeInsideTargetRange = Int32.MinValue;
			for (int i = 0; i < cuttedPartAsUnion.InnerCellRanges.Count; i++) {
				CellRangeBase innerRange = cuttedPartAsUnion.InnerCellRanges[i];
				CellRange casted = innerRange as CellRange;
				if (casted == null)
					continue;
				if (targetRange.Includes(casted)) {
					indexOfInnerRangeInsideTargetRange = i;
					break;
				}
			}
			if (indexOfInnerRangeInsideTargetRange != Int32.MinValue) {
				CellRangeBase innerRange = cuttedPartAsUnion.InnerCellRanges[indexOfInnerRangeInsideTargetRange];
				CellRange newSourceRange = sourceHyperlinkRange.ExcludeRange(innerRange) as CellRange;
				if (newSourceRange != null)
					RangeCopy.ModifySourceHyperlinkRange(sourceHyperlink, newSourceRange);
			}
		}
		void CuttedPartIsCellRangeCase(RangeCopyInfo rangeInfo, ModelHyperlink sourceHyperlink, CellRange targetHyperlinkRange, CellRangeBase cuttedPart) {
			CellRange sourceHyperlinkRange = sourceHyperlink.Range;
			CellRange targetRange = rangeInfo.TargetBigRange;
			CellRange sourceRange = rangeInfo.SourceRange;
			CellRange cuttedPartNormal = cuttedPart as CellRange;
			bool cuttedPartInsideTargetRange = cuttedPart == null || targetRange.Includes(cuttedPart);
			if (cuttedPartInsideTargetRange) {
				CellRange newTargetHyperlinkRange = targetHyperlinkRange;
				if (rangeInfo.IntersectionType != SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
					&& rangeInfo.CanAddPartToRange(targetHyperlinkRange, cuttedPartNormal, true)) {
					newTargetHyperlinkRange = targetHyperlinkRange.Expand(cuttedPartNormal);
				}
				RangeCopy.ModifySourceHyperlinkRange(sourceHyperlink, newTargetHyperlinkRange);
			}
			else {
				CellRange sourceObjectRangeInsideSourceRange = sourceHyperlinkRange.Intersection(sourceRange);
				CellRange sourceObjectRangeTargetObjectRangeExpanded = targetHyperlinkRange.Expand(sourceObjectRangeInsideSourceRange);
				bool cuttedPartInsideSoRToRExpanded = cuttedPart != null && targetRange.Intersects(cuttedPart);
				if (cuttedPartInsideSoRToRExpanded) {
					return;
				}
				if (rangeInfo.CanAddPartToRange(sourceObjectRangeTargetObjectRangeExpanded, cuttedPartNormal, false)) {
					CellRange expanded = sourceHyperlinkRange.Expand(targetHyperlinkRange);
					RangeCopy.ModifySourceHyperlinkRange(sourceHyperlink, expanded);
				}
			}
		}
	}
}
