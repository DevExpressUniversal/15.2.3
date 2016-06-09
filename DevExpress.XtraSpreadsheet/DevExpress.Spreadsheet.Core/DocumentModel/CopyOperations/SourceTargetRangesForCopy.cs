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
using System.Collections;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class SourceTargetRangesForCopy : IEnumerable<RangeCopyInfo> {
		readonly Worksheet targetWorksheet;
		readonly Worksheet sourceWorksheet;
		List<RangeCopyInfo> innerList;
		public SourceTargetRangesForCopy(CellRange source, CellRangeBase target) {
			source = (CellRange)source.GetWithModifiedPositionType(PositionType.Absolute);
			target = target.GetWithModifiedPositionType(PositionType.Absolute);
			this.sourceWorksheet = source.Worksheet as Worksheet;
			this.targetWorksheet = target.Worksheet as Worksheet;
			innerList = CalculateList(source, target);
		}
		public SourceTargetRangesForCopy(ICell source, ICell target)
			: this(source.GetRange(), target.GetRange()) {
		}
		public SourceTargetRangesForCopy(Worksheet source, Worksheet target)
			: this(CellIntervalRange.CreateAllWorksheetRange(source), CellIntervalRange.CreateAllWorksheetRange(target)) {
		}
		List<RangeCopyInfo> CalculateList(CellRange sourceRange, CellRangeBase target) {
			CellRange sourceRangeAbs = (CellRange)sourceRange.GetWithModifiedPositionType(PositionType.Absolute);
			CellRangeBase targetAbs = target.GetWithModifiedPositionType(PositionType.Absolute);
			SourceTargetRangeForCopyCalculator calc = new SourceTargetRangeForCopyCalculator(sourceRangeAbs);
			List<RangeCopyInfo> result = calc.Process(targetAbs);
			return result;
		}
		public Worksheet TargetWorksheet { get { return targetWorksheet; } }
		public Worksheet SourceWorksheet { get { return sourceWorksheet; } }
		public CellRange SourceRange { get { return First.SourceRange; } }
		public RangeCopyInfo First { get { return innerList[0]; } }
		public bool TargetRangeIsCellUnion { get { return this.innerList.Count != 1; } }
		public IEnumerator<RangeCopyInfo> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (innerList as IEnumerable).GetEnumerator();
		}
		public CellRangeBase GetUnionOfTargetRanges() {
			if (!TargetRangeIsCellUnion)
				return First.TargetBigRange;
			List<CellRangeBase> result = new List<CellRangeBase>();
			foreach (RangeCopyInfo item in innerList) {
				result.Add(item.TargetBigRange);
			}
			return new CellUnion(result);
		}
	}
}
