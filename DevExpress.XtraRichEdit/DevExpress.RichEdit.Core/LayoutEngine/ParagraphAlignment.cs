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
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region ParagraphAlignmentCalculatorBase (abstract class)
	public abstract class ParagraphAlignmentCalculatorBase {
		#region Fields
		static readonly ParagraphAlignmentCalculatorBase leftAlignmentCalculator = new LeftAlignmentCalculator();
		static readonly ParagraphAlignmentCalculatorBase rightAlignmentCalculator = new RightAlignmentCalculator();
		static readonly ParagraphAlignmentCalculatorBase centerAlignmentCalculator = new CenterAlignmentCalculator();
		static readonly ParagraphAlignmentCalculatorBase justifyAlignmentCalculator = new JustifyAlignmentCalculator();
		#endregion
		#region Properties
		public static ParagraphAlignmentCalculatorBase LeftAlignmentCalculator { get { return leftAlignmentCalculator; } }
		public static ParagraphAlignmentCalculatorBase RightAlignmentCalculator { get { return rightAlignmentCalculator; } }
		public static ParagraphAlignmentCalculatorBase CenterAlignmentCalculator { get { return centerAlignmentCalculator; } }
		public static ParagraphAlignmentCalculatorBase JustifyAlignmentCalculator { get { return justifyAlignmentCalculator; } }
		#endregion
		public void AlignRow(Row row) {
			if (row.InnerBoxRanges == null)
				AlignRow(row.Bounds, row);
			else
				AlignMultipartRow(row.Bounds, row);
		}
		public void AlignRow(Rectangle rowBounds, Row row) {
			BoxCollection boxes = row.Boxes;
			AlignRowCore(rowBounds, row.TextOffset, boxes, 0, boxes.Count - 1, row.ShouldProcessLayoutDependentText);
			Box lastBox = boxes.Last;
			if (lastBox is SectionMarkBox || lastBox is PageBreakBox || lastBox is ColumnBreakBox)
				AlignBreakBox(rowBounds, boxes);
			AlignNumberingListBox(row.NumberingListBox, row.Boxes.First); 
		}
		public void AlignMultipartRow(Rectangle rowBounds, Row row) {
			BoxCollection boxes = row.Boxes;
			RowBoxRangeCollection rowBoxRanges = row.InnerBoxRanges;
			int count = rowBoxRanges.Count;
			bool shouldProcessLayoutDependentText = row.ShouldProcessLayoutDependentText;
			for (int i = 0; i < count; i++) {
				RowBoxRange range = rowBoxRanges[i];
				AlignRowCore(range.Bounds, row.TextOffset, boxes, range.FirstBoxIndex, range.LastBoxIndex, shouldProcessLayoutDependentText);
			}
			Box lastBox = boxes.Last;
			if (lastBox is SectionMarkBox || lastBox is PageBreakBox || lastBox is ColumnBreakBox)
				AlignBreakBox(rowBounds, boxes);
			AlignNumberingListBox(row.NumberingListBox, row.Boxes.First); 
		}
		public void AlignNumberingListBox(NumberingListBox numberingListBox, Box firstRowBox) {
			if (numberingListBox == null)
				return;
			Rectangle bounds = numberingListBox.Bounds;
			bounds.X = firstRowBox.Bounds.Left - bounds.Width;
			numberingListBox.Bounds = bounds;			
		}
		protected internal abstract void AlignRowCore(Rectangle rowBounds, int textOffset, BoxCollection boxes, int from, int to, bool containsLayoutDependentBox);
		internal static ParagraphAlignmentCalculatorBase GetAlignmentCalculator(ParagraphAlignment alignment) {
			switch (alignment) {
				case ParagraphAlignment.Center:
					return CenterAlignmentCalculator;
				case ParagraphAlignment.Right:
					return RightAlignmentCalculator;
				case ParagraphAlignment.Left:
					return LeftAlignmentCalculator;
				case ParagraphAlignment.Justify:
					return JustifyAlignmentCalculator;
				default:
					Exceptions.ThrowArgumentException("alignment", alignment);
					return null;
			}
		}
		protected internal void AlignBreakBox(Rectangle rowBounds, BoxCollection boxes) {
			Box sectionBox = boxes.Last;
			Rectangle bounds = sectionBox.Bounds;
			int contentRightBounds = GetRowContentRightBounds(rowBounds, boxes);
			bounds.Width = Math.Max(1, Math.Abs(rowBounds.Right - contentRightBounds));
			bounds.X = Math.Min(rowBounds.Right, contentRightBounds);
			sectionBox.Bounds = bounds;
		}
		int GetRowContentRightBounds(Rectangle rowBounds, BoxCollection boxes) {
			int count = boxes.Count;
			if (count > 1)
				return boxes[count - 2].Bounds.Right; 
			else
				return GetSectionBoxLeft(rowBounds);
		}
		protected internal virtual int GetSectionBoxLeft(Rectangle rowBounds) {
			return rowBounds.Left;
		}
		protected static int AlignLeft(BoxCollection boxes, int left, int from, int to) {
			for (int i = from; i <= to; i++) {
				Box box = boxes[i];
				ISpaceBox spaceBox = box as ISpaceBox;
				Rectangle r = box.Bounds;
				if (spaceBox != null)
					r.Width = spaceBox.MinWidth;
				r.X = left;
				left += r.Width;
				box.Bounds = r;
			}
			return left;
		}
		protected static int FindLastVisibleBoxIndex(BoxCollection boxes, int from, int to) {
			for (int i = to; i >= from; i--)
				if (boxes[i].IsVisible)
					return i;
			return from - 1;
		}
		protected static bool IsBoxOutsideOfRow(Rectangle rowBounds, int from, BoxCollection boxes, int boxIndex) {
			return boxIndex > from && boxes[boxIndex].Bounds.Right > rowBounds.Right;
		}
	}
	#endregion
	#region LeftAlignmentCalculator
	public class LeftAlignmentCalculator : ParagraphAlignmentCalculatorBase {
		protected internal override void AlignRowCore(Rectangle rowBounds, int textOffset, BoxCollection boxes, int from, int to, bool containsLayoutDependentBox) {
			AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
		}
	}
	#endregion
	#region RightAlignmentCalculator
	public class RightAlignmentCalculator : ParagraphAlignmentCalculatorBase {
		protected internal override void AlignRowCore(Rectangle rowBounds, int textOffset, BoxCollection boxes, int from, int to, bool containsLayoutDependentBox) {
			if (to < from)
				return;
			int lastNotSpaceBoxIndex = FindLastVisibleBoxIndex(boxes, from, to);
			if (IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex)) {
				AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
				if (!containsLayoutDependentBox || IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex))
					return;
			}
			AlignLeft(boxes, rowBounds.Right, lastNotSpaceBoxIndex + 1, to);
			int right = rowBounds.Right;
			for (int i = lastNotSpaceBoxIndex; i >= from; i--) {
				Box box = boxes[i];
				Rectangle r = box.Bounds;
				ISpaceBox spaceBox = box as ISpaceBox;
				if (spaceBox != null)
					r.Width = spaceBox.MinWidth;
				right -= r.Width;
				r.X = right;
				box.Bounds = r;
			}
		}
		protected internal override int GetSectionBoxLeft(Rectangle rowBounds) {
			return rowBounds.Right;
		}
	}
	#endregion
	#region CenterAlignmentCalculator
	public class CenterAlignmentCalculator : ParagraphAlignmentCalculatorBase {
		protected internal override void AlignRowCore(Rectangle rowBounds, int textOffset, BoxCollection boxes, int from, int to, bool containsLayoutDependentBox) {
			if (to < from)
				return;
			int lastNotSpaceBoxIndex = FindLastVisibleBoxIndex(boxes, from, to);
			if (IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex)) {
				AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
				if (!containsLayoutDependentBox || IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex))
					return;
			}
			int totalWidth = 0;
			for (int i = from; i <= lastNotSpaceBoxIndex; i++) {
				Box box = boxes[i];
				ISpaceBox spaceBox = box as ISpaceBox;
				if (spaceBox != null)
					totalWidth += spaceBox.MinWidth;
				else
					totalWidth += boxes[i].Bounds.Width;
			}
			int left = (rowBounds.Left + rowBounds.Right - totalWidth + textOffset) / 2;
			for (int i = from; i <= lastNotSpaceBoxIndex; i++) {
				Box box = boxes[i];
				Rectangle r = box.Bounds;
				ISpaceBox spaceBox = box as ISpaceBox;
				if (spaceBox != null)
					r.Width = spaceBox.MinWidth;
				r.X = left;
				left += r.Width;
				box.Bounds = r;
			}
			AlignLeft(boxes, left, lastNotSpaceBoxIndex + 1, to);
		}
		protected internal override int GetSectionBoxLeft(Rectangle rowBounds) {
			return rowBounds.Width / 2 + rowBounds.Left;
		}
	}
	#endregion
	#region JustifyAlignmentCalculator
	public class JustifyAlignmentCalculator : ParagraphAlignmentCalculatorBase {
		static void JustifyRow(Rectangle rowBounds, int textOffset, BoxCollection boxes, int minFrom, int from, int to, int totalSpaceWidth, int totalWidth) {
			int left = AlignLeft(boxes, rowBounds.Left + textOffset, minFrom, from - 1);
			int freeSpace = rowBounds.Right - left - totalWidth;
			int remainder = 0;
			for (int i = from; i <= to; i++) {
				Box box = boxes[i];
				Rectangle r = box.Bounds;
				ISpaceBox spaceBox = box as ISpaceBox;
				r.X = left;
				if (spaceBox != null) {
					int t = freeSpace * spaceBox.MinWidth + remainder;
					int delta = t / totalSpaceWidth;
					remainder = t % totalSpaceWidth;
					left += delta;
					left += spaceBox.MinWidth;
					r.Width = spaceBox.MinWidth + delta;
				}
				else
					left += r.Width;
				box.Bounds = r;
			}
		}
		static int FindFirstBoxIndex(BoxCollection boxes, int from, int to) {
			int lastNonSpace = to;
			for (int i = to; i >= from; i--) {
				if (boxes[i] is TabSpaceBox)
					return lastNonSpace;
				if (!(boxes[i] is ISpaceBox))
					lastNonSpace = i;
			}
			return from - 1;
		}
		protected internal override void AlignRowCore(Rectangle rowBounds, int textOffset, BoxCollection boxes, int from, int to, bool containsLayoutDependentBox) {
			if (to < from)
				return;
			if (boxes[to] is ParagraphMarkBox) {
				AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
				return;
			}
			if (to - from > 0 && boxes[to - 1] is ParagraphMarkBox) {
				AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
				return;
			}
			int lastNotSpaceBoxIndex = FindLastVisibleBoxIndex(boxes, from, to);
			if (IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex)) {
				AlignLeft(boxes, rowBounds.Left + textOffset, from, to);
				if (!containsLayoutDependentBox || IsBoxOutsideOfRow(rowBounds, from, boxes, lastNotSpaceBoxIndex))
					return;
			}
			AlignLeft(boxes, rowBounds.Right, lastNotSpaceBoxIndex + 1, to);
			int firstBoxIndex = FindFirstBoxIndex(boxes, from, lastNotSpaceBoxIndex);
			int totalWidth = 0;
			int totalSpaceWidth = 0;
			for (int i = lastNotSpaceBoxIndex; i > firstBoxIndex; i--) {
				Box box = boxes[i];
				int boxWidth = box.Bounds.Width;
				ISpaceBox spaceBox = box as ISpaceBox;
				if (spaceBox != null) {
					totalSpaceWidth += spaceBox.MinWidth;
					totalWidth += spaceBox.MinWidth;
				}
				else {
					totalWidth += boxWidth;
				}
			}
			if (totalSpaceWidth == 0) {
				ParagraphAlignmentCalculatorBase.LeftAlignmentCalculator.AlignRowCore(rowBounds, textOffset, boxes, from, to, containsLayoutDependentBox);
				return;
			}
			JustifyRow(rowBounds, textOffset, boxes, from, firstBoxIndex + 1, lastNotSpaceBoxIndex, totalSpaceWidth, totalWidth);
		}
	}
	#endregion
}
