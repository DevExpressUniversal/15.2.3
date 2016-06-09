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
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class TableProcessorForRangeCopyOperation {
		ITargetRangeCalculatorOwner owner;
		IRangeCopyTablePart rangeCopy;
		public TableProcessorForRangeCopyOperation(ITargetRangeCalculatorOwner rangeCopyOperation, IRangeCopyTablePart rangeCopy) {
			this.owner = rangeCopyOperation;
			this.rangeCopy = rangeCopy;
		}
		protected ITargetRangeCalculatorOwner Owner { get { return owner; } }
		protected IRangeCopyTablePart RangeCopy { get { return rangeCopy; } }
		public virtual void Calculate(RangeCopyInfo rangeInfo, Table sourceTable, List<string> existingTableNames) {
			CellRange SR = rangeInfo.SourceRange;
			CellRange TR = rangeInfo.TargetBigRange;
			CellRange sourceObjectRange = sourceTable.Range;
			if (!SR.Includes(sourceObjectRange))
				return; 
			CellRange targetTableRange = Owner.GetTargetObjectRange(sourceObjectRange, TR) as CellRange;
			if (rangeInfo.SourceAndTargetEquals
				|| rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.SourceRangeIsTheFirst)
				return;
			if (rangeInfo.IntersectionType == SourceRangeIntersectsTargetRangeType.WithOffet_Case2OrEquals) {
				if (TR.Includes(sourceObjectRange) && SR.Includes(sourceObjectRange)) {
				}
				else if (targetTableRange.Intersects(sourceObjectRange)) {
					CellRange intersectionTargetSourceRange = SR.Intersection(TR);
					return;
				}
			}
			NotificationChecks flags = NotificationChecks.PivotTable | NotificationChecks.Table | NotificationChecks.ProtectedCells;
			bool canTableInsert = RangeCopy.CanInsert(targetTableRange, flags, false);
			if (!canTableInsert) 
				return;
			rangeCopy.CreateTargetTable(sourceTable, targetTableRange, existingTableNames, TR);
		}
	}
}
