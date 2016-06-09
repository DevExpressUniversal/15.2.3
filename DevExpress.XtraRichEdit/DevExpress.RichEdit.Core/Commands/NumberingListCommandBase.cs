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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Commands.Helper;
using DevExpress.XtraRichEdit;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands.Helper {
	#region ParagraphLayoutPosition
	public class ParagraphLayoutPosition {
		#region Fields
		Page page;
		PageArea pageArea;
		Column column;
		Row row;
		int boxIndex;
		readonly IRichEditControl control;
		#endregion
		public ParagraphLayoutPosition(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected internal Page Page { get { return page; } }
		protected internal PageArea PageArea { get { return pageArea; } }
		protected internal Column Column { get { return column; } }
		protected internal Row Row { get { return row; } }
		protected internal int BoxIndex { get { return boxIndex; } }
		#endregion
		protected internal void GetParagraphLayoutPosition(Paragraph paragraph) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			RunIndex runIndex = paragraph.FirstRunIndex;
			this.page = LookupPage(control.InnerControl.ActiveView.DocumentLayout.Pages, runIndex);
			this.pageArea = LookupPageArea(page, paragraph, runIndex);
			this.column = LookupColumn(pageArea.Columns, runIndex);
			this.row = LookupRow(column.Rows, runIndex);
			this.boxIndex = LookupBoxIndex(row.Boxes, runIndex);
		}
		protected internal virtual Page LookupPage(PageCollection pages, RunIndex runIndex) {
			return LookupBoxCore(pages, new PageRunComparer(runIndex));
		}
		protected internal virtual PageArea LookupPageArea(Page page, Paragraph paragraph, RunIndex runIndex) {
			if (page.Header != null && Object.ReferenceEquals(page.Header.PieceTable, paragraph.PieceTable))
				return page.Header;
			if (page.Footer != null && Object.ReferenceEquals(page.Footer.PieceTable, paragraph.PieceTable))
				return page.Footer;
			return LookupBoxCore(Page.Areas, new PageAreaRunComparer(runIndex));
		}
		protected internal virtual Column LookupColumn(ColumnCollection columns, RunIndex runIndex) {
			return LookupBoxCore(columns, new ColumnRunComparer(runIndex));
		}
		protected internal virtual Row LookupRow(RowCollection rows, RunIndex runIndex) {
			return LookupBoxCore(rows, new RowRunComparer(runIndex));
		}
		protected internal virtual int LookupBoxIndex(BoxCollection boxes, RunIndex runIndex) {
			return LookupBoxIndexCore(boxes, new BoxRunComparer(runIndex));
		}
		protected internal virtual T LookupBoxCore<T>(BoxCollectionBase<T> boxes, IComparable<T> comparer) where T : Box {
			int boxIndex = LookupBoxIndexCore(boxes, comparer);
			return boxes[boxIndex];
		}
		protected internal virtual int LookupBoxIndexCore<T>(BoxCollectionBase<T> boxes, IComparable<T> comparer) where T : Box {
			int boxIndex = Algorithms.BinarySearch(boxes, comparer);
			if (boxIndex < 0)
				boxIndex = ~boxIndex;
			Debug.Assert(boxIndex < boxes.Count);
			return boxIndex;
		}
	}
	#endregion
	#region HeaderFooterParagraphLayoutPosition
	public class HeaderFooterParagraphLayoutPosition : ParagraphLayoutPosition {
		readonly Page page;
		readonly PageArea area;
		public HeaderFooterParagraphLayoutPosition(IRichEditControl control, Page page, PageArea area)
			: base(control) {
			Guard.ArgumentNotNull(page, "page");
			Guard.ArgumentNotNull(area, "area");
			this.page = page;
			this.area = area;
		}
		protected internal override Page LookupPage(PageCollection pages, RunIndex runIndex) {
			return page;
		}
		protected internal override PageArea LookupPageArea(Page page, Paragraph paragraph, RunIndex runIndex) {
			return area;
		}
	}
	#endregion
	#region PageRunComparer
	public class PageRunComparer : IComparable<Page> {
		readonly RunIndex index;
		public PageRunComparer(RunIndex index) {
			this.index = index;
		}
		#region IComparable<Page> Members
		public int CompareTo(Page page) {
			if (IsRunBeforePage(page))
				return 1;
			if (IsRunAfterPage(page))
				return -1;
			return 0;
		}
		bool IsRunBeforePage(Page page) {
			FormatterPosition startPos = page.Areas.First.Columns.First.Rows.First.Boxes.First.StartPos;
			return index < startPos.RunIndex || (index == startPos.RunIndex && startPos.Offset > 0);
		}
		bool IsRunAfterPage(Page page) {
			FormatterPosition endPos = page.Areas.Last.Columns.Last.Rows.Last.Boxes.Last.StartPos;
			return index > endPos.RunIndex;
		}
		#endregion
	}
	#endregion
	#region PageAreaRunComparer
	public class PageAreaRunComparer : IComparable<PageArea> {
		readonly RunIndex index;
		public PageAreaRunComparer(RunIndex index) {
			this.index = index;
		}
		#region IComparable<PageArea> Members
		public int CompareTo(PageArea pageArea) {
			if (IsRunBeforePageArea(pageArea))
				return 1;
			if (IsRunAfterPageArea(pageArea))
				return -1;
			return 0;
		}
		bool IsRunBeforePageArea(PageArea pageArea) {
			FormatterPosition startPos = pageArea.Columns.First.Rows.First.Boxes.First.StartPos;
			return index < startPos.RunIndex || (index == startPos.RunIndex && startPos.Offset > 0);
		}
		bool IsRunAfterPageArea(PageArea pageArea) {
			FormatterPosition endPos = pageArea.Columns.Last.Rows.Last.Boxes.Last.StartPos;
			return index > endPos.RunIndex;
		}
		#endregion
	}
	#endregion
	#region ColumnRunComparer
	public class ColumnRunComparer : IComparable<Column> {
		readonly RunIndex index;
		public ColumnRunComparer(RunIndex index) {
			this.index = index;
		}
		#region IComparable<Column> Members
		public int CompareTo(Column column) {
			if (IsRunBeforeColumn(column))
				return 1;
			if (IsRunAfterColumn(column))
				return -1;
			return 0;
		}
		bool IsRunBeforeColumn(Column column) {
			FormatterPosition startPos = column.Rows.First.Boxes.First.StartPos;
			return index < startPos.RunIndex || (index == startPos.RunIndex && startPos.Offset > 0);
		}
		bool IsRunAfterColumn(Column column) {
			FormatterPosition endPos = column.Rows.Last.Boxes.Last.StartPos;
			return index > endPos.RunIndex;
		}
		#endregion
	}
	#endregion
	#region RowRunComparer
	public class RowRunComparer : IComparable<Row> {
		readonly RunIndex index;
		public RowRunComparer(RunIndex index) {
			this.index = index;
		}
		#region IComparable<Row> Members
		public int CompareTo(Row row) {
			if (IsRunBeforeRow(row))
				return 1;
			if (IsRunAfterRow(row))
				return -1;
			return 0;
		}
		bool IsRunBeforeRow(Row row) {
			FormatterPosition startPos = row.Boxes.First.StartPos;
			return index < startPos.RunIndex || (index == startPos.RunIndex && startPos.Offset > 0);
		}
		bool IsRunAfterRow(Row row) {
			FormatterPosition endPos = row.Boxes.Last.StartPos;
			return index > endPos.RunIndex;
		}
		#endregion
	}
	#endregion
	#region BoxRunComparer
	public class BoxRunComparer : IComparable<Box> {
		readonly RunIndex index;
		public BoxRunComparer(RunIndex index) {
			this.index = index;
		}
		#region IComparable<Box> Members
		public int CompareTo(Box box) {
			if (IsRunBeforeBox(box))
				return 1;
			if (IsRunAfterBox(box))
				return -1;
			return 0;
		}
		bool IsRunBeforeBox(Box box) {
			return index < box.StartPos.RunIndex || (index == box.StartPos.RunIndex && box.StartPos.Offset > 0);
		}
		bool IsRunAfterBox(Box box) {
			return index > box.StartPos.RunIndex;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands {
	#region NumberingListCommandBase
	public abstract class NumberingListCommandBase : InsertObjectCommandBase {
		protected internal class ParagraphInterval : IComparable<ParagraphInterval> {
			readonly ParagraphIndex start;
			readonly ParagraphIndex end;
			public ParagraphInterval(ParagraphIndex start, ParagraphIndex end) {
				this.start = start;
				this.end = end;
			}
			public ParagraphIndex Start { get { return start; } }
			public ParagraphIndex End { get { return end; } }
			#region IComparable<ParagraphInterval> Members
			public int CompareTo(ParagraphInterval other) {
				return (start - other.Start);
			}
			#endregion
		}
		#region Fields
		NumberingListIndex listIndex = NumberingListIndex.ListIndexNotSetted;
		Dictionary<ParagraphIndex, int> paragraphsLevelIndex;
		Dictionary<ParagraphIndex, ParagraphLayoutPosition> paragraphLayoutPosition;
		bool continueList;
		#endregion
		protected NumberingListCommandBase(IRichEditControl control)
			: base(control) {
			this.paragraphsLevelIndex = new Dictionary<ParagraphIndex, int>();
			this.paragraphLayoutPosition = new Dictionary<ParagraphIndex, ParagraphLayoutPosition>();
		}
		#region Properties
		protected internal Dictionary<ParagraphIndex, int> ParagraphsLevelIndex { get { return paragraphsLevelIndex; } set { paragraphsLevelIndex = value; } }
		protected internal Dictionary<ParagraphIndex, ParagraphLayoutPosition> ParagraphLayoutPositionIndex { get { return paragraphLayoutPosition; } set { paragraphLayoutPosition = value; } }
		protected internal NumberingListIndex ListIndex { get { return listIndex; } set { listIndex = value; } }
		protected internal bool EqualIndent { get { return EqualLeftIndent(); } }
		protected internal bool ContinueList { get { return continueList; } set { continueList = value; } }
		protected internal AbstractNumberingListCollection NumberingListsTemplate { get { return Control.InnerDocumentServer.DocumentModelTemplate.AbstractNumberingLists; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.PageArea; } }
		#endregion
		protected internal override void ModifyModel() {
			List<SelectionItem> selectionItems = DocumentModel.Selection.Items;
			int count = selectionItems.Count;
			List<ParagraphInterval> intervals = new List<ParagraphInterval>();
			for (int i = 0; i < count; i++) {
				SelectionItem selectionItem = selectionItems[i];
				ParagraphIndex startParagraphIndex = selectionItem.GetStartParagraphIndex();
				ParagraphIndex endParagraphIndex = selectionItem.GetEndParagraphIndex();
				intervals.Add(new ParagraphInterval(startParagraphIndex, endParagraphIndex));
			}
			intervals.Sort();
			ModifyParagraphs(intervals);
		}
		 protected internal virtual void ModifyParagraphs(List<ParagraphInterval> paragraphIntervals) {
			int count = paragraphIntervals.Count;
			for (int i = 0; i < count; i++) {
				ParagraphInterval paragraphInterval = paragraphIntervals[i];
				ModifyParagraphsCore(paragraphInterval.Start, paragraphInterval.End);
			}			
		}
		protected internal abstract void ModifyParagraphsCore(ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex);
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable ;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal NumberingType GetLevelType(Paragraph paragraph) {
			NumberingList numberingList = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()];
			return NumberingListHelper.GetLevelType(numberingList, paragraph.GetListLevelIndex());
		}
		protected internal virtual Box GetBox(int boxIndex, BoxCollection boxes) {
			for (int i = boxIndex; i < boxes.Count; i++) {
				if (boxes[i].IsNotWhiteSpaceBox)
					return boxes[i];
			}
			return boxes[boxIndex];
		}
		protected internal virtual bool EqualLeftIndent() {
			int count = ParagraphLayoutPositionIndex.Count;
			if (count == 1)
				return true;
			int minLeftIndent = Int32.MaxValue;
			int maxLeftIndent = Int32.MinValue;
			foreach (ParagraphLayoutPosition position in ParagraphLayoutPositionIndex.Values) {
				Box box = GetBox(position.BoxIndex, position.Row.Boxes);
				int boxLeft = box.Bounds.Left - GetRowParentIndent(position.Row, position);
				minLeftIndent = Math.Min(minLeftIndent, boxLeft);
				maxLeftIndent = Math.Max(maxLeftIndent, boxLeft);
			}
			NumberingList list = DocumentModel.NumberingLists[ListIndex];
			int leftIndent = list.Levels[1].LeftIndent - list.Levels[0].LeftIndent;
			return maxLeftIndent - minLeftIndent < leftIndent;
		}
		protected internal virtual int GetRowParentIndent(Row row, ParagraphLayoutPosition paragraphLayoutPosition) {
			TableCellRow tableCellRow = row as TableCellRow;
			if (tableCellRow == null)
				return paragraphLayoutPosition.Column.Bounds.Left;
			else
				return tableCellRow.CellViewInfo.TextLeft + paragraphLayoutPosition.Column.Bounds.Left;
		}
		protected internal virtual int GetRowIndent(Row row, ParagraphLayoutPosition paragraphLayoutPosition) {			
			return row.Bounds.Left - GetRowParentIndent(row, paragraphLayoutPosition);
		}
		protected internal ParagraphLayoutPosition CreateAndUpdateParagraphLayoutPosition(Paragraph paragraph) {
			ParagraphLayoutPosition paragraphPosition = CreateParagraphLayoutPositionCore(paragraph);
			paragraphPosition.GetParagraphLayoutPosition(paragraph);
			return paragraphPosition;
		}
		protected internal ParagraphLayoutPosition CreateParagraphLayoutPositionCore(Paragraph paragraph) {
			if (paragraph.PieceTable.IsMain)
				return new ParagraphLayoutPosition(Control);
			else {
				Debug.Assert(CaretPosition.LayoutPosition.DetailsLevel >= UpdateCaretPositionBeforeChangeSelectionDetailsLevel);
				return new HeaderFooterParagraphLayoutPosition(Control, CaretPosition.LayoutPosition.Page, CaretPosition.LayoutPosition.PageArea);
			}
		}
	}
	#endregion
}
