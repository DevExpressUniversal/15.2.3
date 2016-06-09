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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region VerticalBorderBreaksCalculator
	public class VerticalBorderBreaksCalculator {
		readonly DocumentLayoutCalculatorBase layoutCalculator;
		readonly RowVerticalBorderBreaks breaks;
		readonly PreliminaryPage page;
		readonly int gridRowIndex;
		readonly int modelRowIndex;
		readonly int initialSingleBoxCount;
		public VerticalBorderBreaksCalculator(DocumentLayoutCalculatorBase layoutCalculator, PreliminaryPage page, RowVerticalBorderBreaks breaks, int gridRowIndex, int modelRowIndex, int initialSingleBoxCount) {
			Guard.ArgumentNotNull(layoutCalculator, "layoutCalculator");
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentNotNull(breaks, "breaks");
			this.layoutCalculator = layoutCalculator;
			this.page = page;
			this.breaks = breaks;
			this.gridRowIndex = gridRowIndex;
			this.modelRowIndex = modelRowIndex;
			this.initialSingleBoxCount = initialSingleBoxCount;
		}
		DocumentLayout Layout { get { return layoutCalculator.Layout; } }
		DocumentLayoutCalculatorBase LayoutCalculator { get { return layoutCalculator; } }
		public BorderBreaksInfo CalculateBreaks(List<SingleCellTextBox> boxes) {
			BorderBreaksInfo result = new BorderBreaksInfo();
			int count = boxes.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (breaks.ContainsKey(modelRowIndex))
					breaks.RemoveContinuousBreaks(modelRowIndex, boxes[i].GridColumnIndex);
				ComplexCellTextBox complexBox = ProcessBox(boxes[i], result.Breaks);
				if (complexBox != null) {
					page.Boxes.RemoveAt(i + initialSingleBoxCount);
					page.ComplexBoxes.Add(complexBox);
					if (!page.RowComplexBoxes.ContainsKey(complexBox.ClipFirstRowIndex))
						page.RowComplexBoxes.Add(complexBox.ClipFirstRowIndex, new ComplexCellTextBoxList());
					page.RowComplexBoxes[complexBox.ClipFirstRowIndex].Add(complexBox);
				}
			}
			if (result.Breaks.Count > 0) {
				breaks.Add(modelRowIndex, result.Breaks);
				if (page.Breaks == null)
					page.Breaks = breaks;
			}
			PreliminaryPageRowInfo rowInfo = new PreliminaryPageRowInfo();
			rowInfo.GridRowIndex = gridRowIndex;
			rowInfo.LastCellIndex = page.Boxes.Count - 1;
			if(rowInfo.GridRowIndex >= 0)
				page.SingleBoxRowInfos.Add(rowInfo);
			return result;
		}
		ComplexCellTextBox ProcessBox(SingleCellTextBox box, List<int> borderBreaks) {
			if(box.GridColumnIndex < 0 || box.GridRowIndex < 0)
				return null;
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			IActualCellAlignmentInfo actualAlign = cell.ActualAlignment;
			if(actualAlign.WrapText || actualAlign.ShrinkToFit)
				return null;
			int textWidth = GetTextWidth(box, cell);
			if (textWidth <= 0)
				return null;
			Rectangle actualTextBounds = box.CalculateActualTextBounds(page, Layout, textWidth, cell.ActualHorizontalAlignment);
			Rectangle cellBounds = box.GetBounds(page);
			if (actualTextBounds.X >= cellBounds.X && actualTextBounds.Right <= cellBounds.Right)
				return null;
			ComplexCellTextBox complexBox = CalculateComplexBox(box, cell, actualTextBounds, borderBreaks, cellBounds);
			complexBox.IsLongTextBox = true;
			page.LongTextBoxes.Add(complexBox);
			return complexBox;
		}
		ComplexCellTextBox CalculateComplexBox(SingleCellTextBox box, ICell cell, Rectangle actualTextBounds, List<int> borderBreaks, Rectangle cellBounds) {
			int lastClipColumnIndex = 0;
			int firstClipColumnIndex = 0;
			if (actualTextBounds.Right > cellBounds.Right) {
				Rectangle boxBounds = box.GetBounds(cell, actualTextBounds, Layout);
				if (box.ClipLastColumnIndex >= page.GridColumns.Count - 1)
					lastClipColumnIndex = Math.Max(box.ClipLastColumnIndex, EnsureColumnGridExists(boxBounds));
				if (page.HasRightPage && lastClipColumnIndex > box.ClipLastColumnIndex) {
					box.ClipLastColumnIndex = lastClipColumnIndex;
				}
				else {
					if (lastClipColumnIndex != 0)
						box.ClipLastColumnIndex = lastClipColumnIndex;
					lastClipColumnIndex = ProcessWideBoxRightSide(box, actualTextBounds.Right - cellBounds.Right, borderBreaks);
				}
			}
			else
				lastClipColumnIndex = box.GridColumnIndex;
			if (actualTextBounds.X < cellBounds.X)
				firstClipColumnIndex = ProcessWideBoxLeftSide(box, cellBounds.X - actualTextBounds.X + Layout.FourPixelsPadding, borderBreaks);
			else
				firstClipColumnIndex = box.GridColumnIndex;
			return CreateComplexBox(box, cell, actualTextBounds, lastClipColumnIndex, firstClipColumnIndex);
		}
		ComplexCellTextBox CreateComplexBox(SingleCellTextBox box, ICell cell, Rectangle actualTextBounds, int lastClipColumnIndex, int firstClipColumnIndex) {
			ComplexCellTextBox complexBox = new ComplexCellTextBox(cell);
			complexBox.ClipFirstRowIndex = gridRowIndex;
			complexBox.ClipLastRowIndex = gridRowIndex;
			complexBox.ClipFirstColumnIndex = firstClipColumnIndex;
			complexBox.ClipLastColumnIndex = lastClipColumnIndex;
			complexBox.Bounds = box.GetBounds(cell, actualTextBounds, Layout);
			return complexBox;
		}
		int GetTextWidth(SingleCellTextBox box, ICell cell) {
			Rectangle availableTextBounds = box.GetTextBounds(page, Layout);
			int textWidth = this.LayoutCalculator.CalculateCellSingleLineTextWidth(cell, page, availableTextBounds);
			return textWidth;
		}
		int EnsureColumnGridExists(Rectangle bounds) {
			if(bounds.Right <= page.GridColumns.Last.Far)
				return page.GridColumns.Count - 1;
			PageGridCalculator calculator = layoutCalculator.CreatePageGridCalculator(page.ClientBounds);
			return calculator.EnsureColumnGridExists(page.GridColumns, bounds);
		}
		int ProcessWideBoxLeftSide(SingleCellTextBox box, int width, List<int> borderBreaks) {
			int insertAt = borderBreaks.Count;
			for(int i = box.GridColumnIndex - 1; i > box.ClipFirstColumnIndex; i--) {
				borderBreaks.Insert(insertAt, i + 1);
				width -= page.GridColumns[i].Extent;
				if (width < 0)
					return i;
			}
			return box.ClipFirstColumnIndex;
		}
		int ProcessWideBoxRightSide(SingleCellTextBox box, int width, List<int> borderBreaks) {
			int insertAt = 0;
			for (int i = box.GridColumnIndex + 1; i <= box.ClipLastColumnIndex; i++, insertAt++) {
				if (width <= 0)
					return i - 1;
				width -= page.GridColumns[i].Extent;
				borderBreaks.Insert(insertAt, i);
			}
			if (width > 0) {
				if (box.ClipLastColumnIndex == page.GridColumns.Count - 1)
					borderBreaks.Add(box.ClipLastColumnIndex + 1);
			}
			return box.ClipLastColumnIndex;
		}
	}
	#endregion
}
