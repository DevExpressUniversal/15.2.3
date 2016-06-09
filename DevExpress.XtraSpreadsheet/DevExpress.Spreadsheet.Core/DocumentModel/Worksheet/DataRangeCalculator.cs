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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataRangeCalculator
	public class DataRangeCalculator {
		readonly Worksheet sheet;
		public DataRangeCalculator(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
		public CellRange CalculateDataRange() {
			CellRange range = GetDataRange();
			range = AppendMergedCells(range);
			return (CellRange)range.GetWithModifiedPositionType(PositionType.Absolute);
		}
		CellRange GetDataRange() {
			int leftColumnIndex = sheet.MaxColumnCount + 1;
			int topRowIndex = sheet.MaxRowCount;
			int rightColumnIndex = 0;
			int bottomRowIndex = 0;
			foreach (Row row in sheet.Rows.GetExistingRows(0, sheet.MaxRowCount, false)) {
				if (row.CellsCount == 0)
					continue; 
				ICellCollection cells = row.Cells;
				ICell leftCell = cells.TryGetFirstCellWith(cell => { return cell.HasContent; });
				if (leftCell == null)
					continue; 
				CellPosition leftPosition = leftCell.Position;
				leftColumnIndex = Math.Min(leftColumnIndex, leftPosition.Column);
				topRowIndex = Math.Min(topRowIndex, leftPosition.Row);
				int leftColumnIndexStopSearch = 0;
				ICell rightCell = cells.TryGetLastCellWith(cell => { return cell.HasContent; },
					leftColumnIndexStopSearch, sheet.MaxColumnCount - 1);
				CellPosition rightPosition = rightCell != null ? rightCell.Position : leftCell.Position;
				rightColumnIndex = Math.Max(rightColumnIndex, rightPosition.Column);
				bottomRowIndex = Math.Max(bottomRowIndex, rightPosition.Row);
			}
			if (leftColumnIndex > sheet.MaxColumnCount)
				return new CellRange(sheet, new CellPosition(0, 0), new CellPosition(0, 0));
			return new CellRange(
				sheet,
				new CellPosition(leftColumnIndex, topRowIndex),
				new CellPosition(rightColumnIndex, bottomRowIndex));
		}
		CellRange AppendMergedCells(CellRange range) {
			CellRange temp = null;
			bool rangeEnlarged = true;
			while (rangeEnlarged) {
				temp = range;
				rangeEnlarged = false;
				List<CellRange> mergedCellRanges = sheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
				foreach (CellRange mergedCellRange in mergedCellRanges) {
					range = range.Expand(mergedCellRange);
					rangeEnlarged = !range.EqualsPosition(temp);
					if (rangeEnlarged)
						break;
				}
			}
			return range;
		}
	}
	#endregion
}
