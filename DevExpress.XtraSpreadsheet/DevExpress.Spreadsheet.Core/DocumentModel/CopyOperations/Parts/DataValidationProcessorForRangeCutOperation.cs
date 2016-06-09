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
	#region DataValidationRangeCalculatorForCutOperation
	public class DataValidationProcessorForCutOperation : DataValidationSplitterBase {
		#region Fields
		ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker walker;
		#endregion
		public DataValidationProcessorForCutOperation(DataValidationNotificationInfo info, ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker walker)
			: base(info) {
			this.walker = walker;
		}
		#region Properties
		protected override CellRange ActionRange { get { return walker.SourceRange; } }
		protected override ReplaceThingsPRNVisitor Walker { get { return walker; } }
		#endregion
		public override List<DataValidationNotificationInfo> Process() {
			CellRangeBase affectedRange = GetAffectedRange();
			if (affectedRange == null) {
				ProcessCellRange(Range);
				return ResultList;
			}
			CellRangeBase unaffectedRange = Range.ExcludeRange(affectedRange);
			if (unaffectedRange != null)
				ProcessCellRange(unaffectedRange);
			ProcessAllCells(affectedRange);
			return ResultList;
		}
		CellRangeBase GetAffectedRange() {
			CellRangeBase allAffectedRange = null;
			foreach (CellRange range in Range.GetAreasEnumerable()) {
				List<CellRange> relativeRanges = Info.GetRelativeReferenceRanges(range.TopLeft, (Worksheet)ActionRange.Worksheet);
				foreach (CellRange relativeRange in relativeRanges) {
					if (SmallerThanRelativeRange(ActionRange, relativeRange))
						continue;
					CellRange enlarged = EnlargeReferencedRange(relativeRange, range.Width, range.Height);
					CellRange intersectionWithEnlarged = ActionRange.Intersection(enlarged);
					if (intersectionWithEnlarged == null || SmallerThanRelativeRange(intersectionWithEnlarged, relativeRange))
						continue;
					int offsetX = relativeRange.TopLeft.Column - range.TopLeft.Column;
					int offsetY = relativeRange.TopLeft.Row - range.TopLeft.Row;
					CellRange affectedRange = GetAffectedRangeCore(intersectionWithEnlarged, relativeRange, offsetX, offsetY);
					affectedRange = affectedRange.Intersection(range);
					if (affectedRange != null)
						allAffectedRange = allAffectedRange == null ? affectedRange : allAffectedRange.MergeWithRange(affectedRange);
				}
			}
			return allAffectedRange;
		}
		bool SmallerThanRelativeRange(CellRange range, CellRange relativeRange) {
			return range.Width < relativeRange.Width || range.Height < relativeRange.Height;
		}
		CellRange GetAffectedRangeCore(CellRange intersection, CellRange relativeRange, int offsetX, int offsetY) {
			CellPosition topLeft = new CellPosition(intersection.TopLeft.Column - offsetX, intersection.TopLeft.Row - offsetY);
			CellPosition bottomRight = new CellPosition(topLeft.Column + (intersection.Width - relativeRange.Width), topLeft.Row + (intersection.Height - relativeRange.Height));
			return new CellRange(Sheet, topLeft, bottomRight);
		}
	}
	#endregion
}
