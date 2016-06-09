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
	public class SourceHyperlinkProcessorForRangeCopyOperation {
		ITargetRangeCalculatorOwner owner;
		IRangeCopyHyperlinkPart rangeCopy;
		public SourceHyperlinkProcessorForRangeCopyOperation(ITargetRangeCalculatorOwner rangeCopyOperation, IRangeCopyHyperlinkPart rangeCopy) {
			this.owner = rangeCopyOperation;
			this.rangeCopy = rangeCopy;
		}
		protected ITargetRangeCalculatorOwner Owner { get { return owner; } }
		public IRangeCopyHyperlinkPart RangeCopy { get { return rangeCopy; } }
		public virtual void Execute(RangeCopyInfo rangeInfo, ModelHyperlink sourceHyperlinkItem) {
			ModelHyperlink sourceHyperlink = sourceHyperlinkItem;
			CellRange sourceHyperlinkRange = sourceHyperlink.Range;
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			if (rangeInfo.SourceAndTargetEquals)
				return;
			CellRange intersectionTargetSourceRange = rangeInfo.SourceRange.Intersection(rangeInfo.TargetBigRange);
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
				&& intersectionTargetSourceRange != null
				&& intersectionTargetSourceRange.Includes(sourceHyperlinkRange)) {
				CellRange newRange = Owner.GetTargetObjectRange(sourceHyperlinkRange, rangeInfo.TargetBigRange) as CellRange;
				if (newRange.Intersects(sourceHyperlinkRange)) {
					return; 
				}
				RangeCopy.ModifySourceHyperlinkRange(sourceHyperlink, newRange);
				RangeCopy.RetrieveSourceHyperlinkRange(sourceHyperlink);
				return;
			}
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
				&& sourceHyperlinkRange.Includes(targetBigRange)) {
				RangeCopy.RemoveSourceHyperlink(sourceHyperlinkItem);
				return;
			}
			Direction objectDirection = rangeInfo.CalculateObjectRangeDirectionInSourceRange(sourceHyperlinkRange);
			if (objectDirection == Direction.Both) {
				RangeCopy.CreateTargetHyperlink(targetBigRange, sourceHyperlinkItem);
			}
			else
				foreach (CellRange currentTarget in rangeInfo.TargetRanges) {
					CellRange targetHyperlinkRange = Owner.GetTargetObjectRange(sourceHyperlinkRange, currentTarget) as CellRange;
					RangeCopy.CreateTargetHyperlink(targetHyperlinkRange, sourceHyperlink);
				}
		}
	}
}
