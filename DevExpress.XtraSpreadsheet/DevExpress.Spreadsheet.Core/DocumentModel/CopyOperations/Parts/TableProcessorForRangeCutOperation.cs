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

using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class TableProcessorForRangeCutOperation : TableProcessorForRangeCopyOperation {
		public TableProcessorForRangeCutOperation(ITargetRangeCalculatorOwner rangeCopyOperation, IRangeCopyTablePart rangeCopy)
			: base(rangeCopyOperation, rangeCopy) {
		}
		#region Calculate
		public override void Calculate(RangeCopyInfo rangeInfo, Table sourceTable, List<string> existingTableNames) {
			if (!Owner.IsEqualWorksheets) {
				base.Calculate(rangeInfo, sourceTable, existingTableNames);
				return;
			}
			if (rangeInfo.SourceAndTargetEquals) {
				return;
			}
			CellRange sourceRange = rangeInfo.SourceRange;
			CellRange targetRange = rangeInfo.TargetBigRange;
			CellRange sourceObjectRange = sourceTable.Range;
			CellRange targetObjectRange = Owner.GetTargetObjectRange(sourceObjectRange, targetRange) as CellRange;
			CellRange sourceTargetRangeIntersection = sourceRange.Intersection(targetRange);
			if(sourceRange.Includes(sourceObjectRange)) {
				if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals
				 && sourceTargetRangeIntersection != null && sourceTargetRangeIntersection.Includes(sourceObjectRange)) {
					RangeCopy.RetrieveSourceTableBackInCollection(sourceTable);
				}
				CellRangeBase targetObjectRangeExcludeSourceTable = targetObjectRange.ExcludeRange(sourceObjectRange);
				bool canTableInsert = true;
				NotificationChecks flags = NotificationChecks.PivotTable | NotificationChecks.Table | NotificationChecks.ProtectedCells;
				bool checkOnlyTables = true; 
				targetObjectRangeExcludeSourceTable.ForEach(r => canTableInsert &= RangeCopy.CanInsert(r, flags, checkOnlyTables));
				if (canTableInsert)
					RangeCopy.ChangeExistingTableRange(sourceTable, targetObjectRange);
				else {
					RangeCopy.RemoveTableAndUpdateFormulaReferences(sourceTable);
				}
			}
			else if (sourceRange.Intersects(sourceObjectRange) && targetObjectRange.Intersects(sourceObjectRange)) {
				bool targetTopLeftInsideSourceObjectRange = sourceObjectRange.ContainsCell(targetRange.TopLeft.Column, targetRange.TopLeft.Row);
				if (!targetTopLeftInsideSourceObjectRange)
					return;
				int additionalRows = Math.Max(0, targetRange.BottomRowIndex - sourceObjectRange.BottomRowIndex);  
				if (additionalRows == 0)
					return;
				CellRange resizedRange = sourceTable.Range.GetResized(0, 0, 0, additionalRows);
				CellRange rangeBelowSourceTable = resizedRange.ExcludeRange(sourceTable.Range) as CellRange;
				bool ensureRangeBelowTableIsNotAnotherTablesMergedRangesAndArrayFormulas 
					= RangeCopy.CanInsert(rangeBelowSourceTable, NotificationChecks.All, false);
				if (ensureRangeBelowTableIsNotAnotherTablesMergedRangesAndArrayFormulas)
					RangeCopy.ResizeExistingTableRange(sourceTable, additionalRows);
			}
		}
		#endregion
	}
}
