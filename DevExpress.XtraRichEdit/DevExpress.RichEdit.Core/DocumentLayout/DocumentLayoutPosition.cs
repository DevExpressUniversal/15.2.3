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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
	#region DocumentLayoutPosition
	public class DocumentLayoutPosition : ICloneable<DocumentLayoutPosition>, ISupportsCopyFrom<DocumentLayoutPosition> {
		#region Fields
		readonly DocumentLayout documentLayout;
		readonly PieceTable pieceTable;
		DocumentLogPosition logPosition;
		DocumentLayoutDetailsLevel detailsLevel = DocumentLayoutDetailsLevel.None;
		Page page;
		Page floatingObjectBoxPage;
		PageArea pageArea;
		Column column;
		Row row;
		Box box;
		CharacterBox character;
		TableRowViewInfoBase tableRow;
		TableCellViewInfo tableCell;
		int leftOffset;
		int rightOffset;
		bool suppressSuspendFormatting;
		#endregion
		public DocumentLayoutPosition(DocumentLayout documentLayout, PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Debug.Assert(Object.ReferenceEquals(documentLayout.DocumentModel, pieceTable.DocumentModel));
			this.documentLayout = documentLayout;
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
			this.suppressSuspendFormatting = false;
		}
		#region Properties
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public DocumentModel DocumentModel { get { return DocumentLayout.DocumentModel; } }
		public virtual PieceTable PieceTable { get { return pieceTable; } }
		public DocumentLayoutDetailsLevel DetailsLevel { get { return detailsLevel; } }
		public Page Page { get { return page; } set { page = value; } }
		public Page FloatingObjectBoxPage { get { return floatingObjectBoxPage; } set { floatingObjectBoxPage = value; } }
		public PageArea PageArea { get { return pageArea; } set { pageArea = value; } }
		public Column Column { get { return column; } set { column = value; } }
		public Row Row { get { return row; } set { row = value; } }
		public Box Box { get { return box; } set { box = value; } }
		public CharacterBox Character { get { return character; } set { character = value; } }
		public TableRowViewInfoBase TableRow { get { return tableRow; } set { tableRow = value; } }
		public TableCellViewInfo TableCell { get { return tableCell; } set { tableCell = value; } }
		protected internal virtual DocumentLogPosition LogPosition { get { return logPosition; } }
		protected internal int LeftOffset { get { return leftOffset; } set { leftOffset = value; } }
		protected internal int RightOffset { get { return rightOffset; } set { rightOffset = value; } }
		protected virtual bool UseLastAvailablePosition { get { return false; } set { } }
		internal bool SuppressSuspendFormatting { get { return suppressSuspendFormatting; } set { suppressSuspendFormatting = value; } }
		#endregion
		public void SetLogPosition(DocumentLogPosition value) {
			this.logPosition = value;
		}
		public void IncreaseDetailsLevel(DocumentLayoutDetailsLevel detailsLevel) {
			if (this.detailsLevel < detailsLevel)
				this.detailsLevel = detailsLevel;
		}
		protected internal void DecreaseDetailLevel(DocumentLayoutDetailsLevel detailsLevel) {
			if (this.detailsLevel > detailsLevel)
				this.detailsLevel = detailsLevel;
		}
		public virtual bool IsValid(DocumentLayoutDetailsLevel detailsLevel) {
			return detailsLevel <= DetailsLevel;
		}
		protected internal virtual void Invalidate() {
			detailsLevel = DocumentLayoutDetailsLevel.None;
		}
		protected internal virtual void EnsureFormattingComplete() {
		}
		protected internal virtual void EnsurePageSecondaryFormattingComplete(Page page) {
		}
		protected internal virtual bool Update(PageCollection pages, DocumentLayoutDetailsLevel detailsLevel) {
			return Update(pages, detailsLevel, false);
		}
		protected internal virtual bool Update(PageCollection pages, DocumentLayoutDetailsLevel detailsLevel, bool useLastAvailablePosition) {
			this.UseLastAvailablePosition = useLastAvailablePosition;
			if (!IsValid(detailsLevel)) {
				this.detailsLevel = UpdateCore(pages, detailsLevel);
				return IsValid(detailsLevel);
			}
			else
				return true;
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdateCore(PageCollection pages, DocumentLayoutDetailsLevel detailsLevel) {
			if (detailsLevel < DocumentLayoutDetailsLevel.Page)
				return DocumentLayoutDetailsLevel.None;
			EnsureFormattingComplete();
			return UpdatePageRecursive(pages, 0, detailsLevel);
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdatePageRecursive(PageCollection pages, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Page)) {
				Page page = LookupPage(pages, startIndex);
				if (page == null)
					return DocumentLayoutDetailsLevel.None;
				else
					this.page = page;
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.Page)
				return DocumentLayoutDetailsLevel.Page;
			return UpdatePageAreaRecursive(this.page, 0, detailsLevel);
		}
		protected internal virtual Page LookupPage(PageCollection pages, int startIndex) {
			int pageIndex = GetPageIndex(pages, startIndex, pages.Count - 1);
			return pageIndex >= 0 ? pages[pageIndex] : null;
		}
		protected internal delegate bool IsIntersectedWithPrevBoxDelegate<T>(T box) where T : Box;
		protected internal virtual DocumentLayoutDetailsLevel UpdatePageAreaRecursive(Page page, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.PageArea)) {
				this.pageArea = LookupPageArea(page, startIndex);
				if (this.pageArea == null)
					return DocumentLayoutDetailsLevel.Page;
				CheckPageAreaPieceTable(pageArea);
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.PageArea)
				return DocumentLayoutDetailsLevel.PageArea;
			return UpdateColumnRecursive(pageArea.Columns, 0, detailsLevel);
		}
		protected internal virtual void CheckPageAreaPieceTable(PageArea pageArea) {
			Debug.Assert(Object.ReferenceEquals(pageArea.PieceTable, this.PieceTable));
		}
		protected internal virtual PageArea LookupPageArea(Page page, int startIndex) {
			if (page.Header != null && Object.ReferenceEquals(page.Header.PieceTable, this.PieceTable))
				return page.Header;
			if (page.Footer != null && Object.ReferenceEquals(page.Footer.PieceTable, this.PieceTable))
				return page.Footer;
			PageAreaCollection areas = page.Areas;
			int pageAreaIndex = GetPageAreaIndex(areas, startIndex, areas.Count - 1);
			return pageAreaIndex >= 0 ? areas[pageAreaIndex] : null;
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdateColumnRecursive(ColumnCollection columns, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Column)) {
				int columnIndex = GetColumnIndex(columns, startIndex, columns.Count - 1);
				if (columnIndex < 0)
					return DocumentLayoutDetailsLevel.PageArea;
				this.column = columns[columnIndex];
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.Column)
				return DocumentLayoutDetailsLevel.Column;
			return UpdateCellRecursive(this.column, detailsLevel);
		}
		public virtual DocumentLayoutDetailsLevel UpdateCellRecursive(Column column, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.TableCell)) {
				TableViewInfoCollection columnTables = column.InnerTables;
				if (columnTables != null) {
					TableCellViewInfo cell = BinarySearchTableCell(columnTables);
					this.tableCell = cell;
					if (cell != null)
						Debug.Assert(Object.ReferenceEquals(cell.TableViewInfo.Table.PieceTable, this.PieceTable));
				}
				else
					this.tableCell = null;
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.TableCell)
				return DocumentLayoutDetailsLevel.TableCell;
			RowCollection rows = TableCell != null ? TableCell.GetRows(column) : column.Rows;
			return UpdateRowRecursive(rows, 0, detailsLevel);
		}
		TableCellViewInfo BinarySearchTableCell(TableViewInfoCollection tableViewInfoCollection) {			
			TableViewInfo tableViewInfo = GetTableViewInfo(tableViewInfoCollection);
			if(tableViewInfo == null)
				return null;
			TableCellViewInfoCollection cells = tableViewInfo.Cells;
			TableCellViewInfoAndLogPositionComparable comparable = new TableCellViewInfoAndLogPositionComparable(PieceTable, LogPosition);
			int startIndex = 0;
			int endIndex = cells.Count - 1;
			int cellIndex = Algorithms.BinarySearch(cells, comparable, startIndex, endIndex);
			if (cellIndex < 0) {
				Paragraph paragraph = PieceTable.FindParagraph(LogPosition);
				if (paragraph == null)
					Exceptions.ThrowInternalException();
				TableCell cell = paragraph.GetCell();
				if (cell == null)
					Exceptions.ThrowInternalException();
				TableCell firstCell = cell.Table.GetFirstCellInVerticalMergingGroup(cell);
				comparable = new TableCellViewInfoAndLogPositionComparable(PieceTable, PieceTable.Paragraphs[firstCell.StartParagraphIndex].LogPosition);
				cellIndex = Algorithms.BinarySearch(cells, comparable, startIndex, endIndex);
				if (cellIndex < 0)
					return null;
			}
			return cells[cellIndex];
		}
		TableViewInfo GetTableViewInfo(TableViewInfoCollection tableViewInfoCollection) {
			TableViewInfoAndLogPositionComparable tableViewInfoAndLogPositionComparable = new TableViewInfoAndLogPositionComparable(PieceTable, LogPosition);
			int count = tableViewInfoCollection.Count;
			for (int tableIndex = count - 1; tableIndex >= 0; tableIndex--) {
				TableViewInfo tableViewInfo = tableViewInfoCollection[tableIndex];
				if(tableViewInfo.Cells.Count == 0)
					continue;
				if (tableViewInfoAndLogPositionComparable.GetFirstPosition(tableViewInfo) <= LogPosition && tableViewInfoAndLogPositionComparable.GetLastPosition(tableViewInfo) >= LogPosition) {
					return tableViewInfo;
				}
			}
			return null;
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdateRowRecursive(RowCollection rows, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Row)) {
				EnsurePageSecondaryFormattingComplete(Page);
				int rowIndex = GetRowIndex(rows, startIndex, rows.Count - 1);
				if (rowIndex < 0 || rowIndex >= rows.Count)
					return DocumentLayoutDetailsLevel.Column;
				this.row = rows[rowIndex];
				Debug.Assert(Object.ReferenceEquals(row.Paragraph.PieceTable, this.PieceTable));
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.Row)
				return DocumentLayoutDetailsLevel.Row;
			return UpdateBoxRecursive(row, 0, detailsLevel);
		}
		bool IsTableCellVerticalMerged() {
			return TableCell.Cell.VerticalMerging == MergingState.Restart;
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdateBoxRecursive(Row row, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Box)) {
				BoxCollection boxes = row.Boxes;
				int boxIndex = GetBoxIndex(boxes, startIndex, boxes.Count - 1);
				if (boxIndex >= boxes.Count) {
					if (UseLastAvailablePosition || (TableCell != null && IsTableCellVerticalMerged()))
						boxIndex = boxes.Count - 1;
					else
						return DocumentLayoutDetailsLevel.Row;
				}
				this.box = boxes[boxIndex];
			}
			if (detailsLevel <= DocumentLayoutDetailsLevel.Box)
				return DocumentLayoutDetailsLevel.Box;
			return UpdateCharacterBoxRecursive(row, 0, detailsLevel);
		}
		protected internal virtual DocumentLayoutDetailsLevel UpdateCharacterBoxRecursive(Row row, int startIndex, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Character)) {
				DetailRow detailRow = DocumentLayout.CreateDetailRowForBox(row, this.Box, SuppressSuspendFormatting);
				CharacterBoxCollection characters = detailRow.Characters;
				int charIndex = GetCharIndex(characters, startIndex, characters.Count - 1);
				if (charIndex >= detailRow.Characters.Count) {
					if (UseLastAvailablePosition || (TableCell != null && IsTableCellVerticalMerged()))
						charIndex = detailRow.Characters.Count - 1;
					else
						return DocumentLayoutDetailsLevel.Box;
				}
				this.character = detailRow.Characters[charIndex];
			}
			return DocumentLayoutDetailsLevel.Character;
		}
		protected internal virtual DocumentLayoutPosition CreateEmptyClone() {
			return new DocumentLayoutPosition(DocumentLayout, PieceTable, LogPosition);
		}
		protected internal virtual int GetPageIndex(PageCollection pages, int startIndex, int endIndex) {
			return FindBoxIndex<Page>(pages, new ExactPageAndLogPositionComparable(PieceTable, LogPosition), IsIntersectedWithPrevBox, startIndex, endIndex);
		}
		protected internal virtual int GetPageIndex(PageCollection pages) {
			return GetPageIndex(pages, 0, pages.Count - 1);
		}
		protected internal virtual int GetPageAreaIndex(PageAreaCollection areas, int startIndex, int endIndex) {
			return FindBoxIndex(areas, new ExactPageAreaAndLogPositionComparable(PieceTable, LogPosition), IsIntersectedWithPrevBox, startIndex, endIndex);
		}
		protected internal virtual int GetPageAreaIndex(PageAreaCollection areas) {
			return GetPageAreaIndex(areas, 0, areas.Count - 1);
		}
		protected internal virtual int GetColumnIndex(ColumnCollection columns, int startIndex, int endIndex) {
			return FindBoxIndex(columns, new ExactColumnAndLogPositionComparable(PieceTable, LogPosition), IsIntersectedWithPrevBox, startIndex, endIndex);
		}
		protected internal virtual int GetColumnIndex(ColumnCollection columns) {
			return GetColumnIndex(columns, 0, columns.Count - 1);
		}
		protected internal virtual int FindBoxIndex<T>(IList<T> boxes, IComparable<T> exactComparable, IsIntersectedWithPrevBoxDelegate<T> isIntersect, int startIndex, int endIndex) where T : Box {
			return FindBoxIndex<T>(boxes, new BoxStartAndLogPositionComparable<T>(PieceTable, LogPosition), exactComparable, isIntersect, startIndex, endIndex);
		}
		protected internal virtual int FindBoxIndex<T>(IList<T> boxes, IComparable<T> comparable, IComparable<T> exactComparable, IsIntersectedWithPrevBoxDelegate<T> isIntersect, int startIndex, int endIndex) where T : Box {
			int boxIndex = Algorithms.BinarySearch(boxes, comparable, startIndex, endIndex);
			if (boxIndex >= 0)
				return boxIndex;
			int lastBoxIndex = ~boxIndex - 1;
			if (lastBoxIndex < 0) {
				return boxes.Count > 0 ? 0 : -1;
			}
			int startBoxIndex = lastBoxIndex;
			while (startBoxIndex > 0 && isIntersect(boxes[startBoxIndex])) {
				startBoxIndex--;
			}
			boxIndex = Algorithms.BinarySearch(boxes, exactComparable, startBoxIndex, lastBoxIndex);
			if (boxIndex >= 0)
				return boxIndex;
			boxIndex = ~boxIndex;
			if (boxIndex < boxes.Count)
				return boxIndex;
			else {
				if (UseLastAvailablePosition)
					return boxes.Count - 1;
				else
					return -1;
			}
		}
		protected bool IsIntersectedWithPrevBox(Page page) {
			return IsIntersectedWithPrevBox(page.Areas.First);
		}
		protected bool IsIntersectedWithPrevBox(PageArea area) {
			return IsIntersectedWithPrevBox(area.Columns.First);
		}
		protected bool IsIntersectedWithPrevBox(Column column) {
			return column.IsIntersectedWithPrevColumn();
		}
		protected internal virtual int GetRowIndex(RowCollection rows, int startIndex, int endIndex) {
			int rowIndex = Algorithms.BinarySearch(rows, new BoxAndLogPositionComparable<Row>(PieceTable, LogPosition), startIndex, endIndex);
			if (rowIndex < 0)
				rowIndex = ~rowIndex;
			if (rowIndex >= rows.Count && (UseLastAvailablePosition || (TableCell != null && IsTableCellVerticalMerged())))
				rowIndex = rows.Count - 1;
			return rowIndex;
		}
		protected internal virtual int GetRowIndex(RowCollection rows) {
			return GetRowIndex(rows, 0, rows.Count - 1);
		}
		protected internal virtual int GetBoxIndex(BoxCollection boxes, int startIndex, int endIndex) {
			int boxIndex = Algorithms.BinarySearch(boxes, new BoxAndLogPositionComparable<Box>(PieceTable, LogPosition), startIndex, endIndex);
			if (boxIndex < 0)
				return ~boxIndex;
			return boxIndex;
		}
		protected internal virtual int GetBoxIndex(BoxCollection boxes) {
			return GetBoxIndex(boxes, 0, boxes.Count - 1);
		}
		protected internal virtual int GetCharIndex(CharacterBoxCollection characters, int startIndex, int endIndex) {
			int charIndex = Algorithms.BinarySearch(characters, new BoxAndLogPositionComparable<CharacterBox>(PieceTable, LogPosition), startIndex, endIndex);
			if (charIndex < 0)
				return ~charIndex;
			return charIndex;
		}
		protected internal virtual int GetCharIndex(CharacterBoxCollection characters) {
			return GetCharIndex(characters, 0, characters.Count - 1);
		}
		protected virtual void CopyFrom(DocumentLayoutPosition value, DocumentLayoutDetailsLevel detailsLevel) {
			this.detailsLevel = detailsLevel;
			if (IsValid(DocumentLayoutDetailsLevel.Page))
				this.Page = value.Page;
			if (IsValid(DocumentLayoutDetailsLevel.PageArea))
				this.PageArea = value.PageArea;
			if (IsValid(DocumentLayoutDetailsLevel.Column))
				this.Column = value.Column;
			if (IsValid(DocumentLayoutDetailsLevel.TableCell))
				this.TableCell = value.TableCell;
			if (IsValid(DocumentLayoutDetailsLevel.Row))
				this.Row = value.Row;
			if (IsValid(DocumentLayoutDetailsLevel.Box))
				this.Box = value.Box;
			if (IsValid(DocumentLayoutDetailsLevel.Character))
				this.Character = value.Character;
		}
		#region ICloneable<DocumentLayoutPosition> Members
		public DocumentLayoutPosition Clone() {
			DocumentLayoutPosition clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<DocumentLayoutPosition> Members
		public virtual void CopyFrom(DocumentLayoutPosition value) {
			this.detailsLevel = value.DetailsLevel;
			this.logPosition = value.LogPosition;
			this.Page = value.Page;
			this.PageArea = value.PageArea;
			this.Column = value.Column;
			this.Row = value.Row;
			this.Box = value.Box;
			this.Character = value.Character;
			this.TableRow = value.TableRow;
			this.TableCell = value.TableCell;
			this.LeftOffset = value.LeftOffset;
			this.RightOffset = value.RightOffset;
		}
		#endregion
	}
	#endregion
	#region HeaderFooterDocumentLayoutPosition
	public class HeaderFooterDocumentLayoutPosition : DocumentLayoutPosition {
		int preferredPageIndex;
		public HeaderFooterDocumentLayoutPosition(DocumentLayout documentLayout, PieceTable pieceTable, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(documentLayout, pieceTable, logPosition) {
			Guard.ArgumentNonNegative(preferredPageIndex, "preferredPageIndex");
			this.preferredPageIndex = preferredPageIndex;
		}
		protected internal int PreferredPageIndex {
			get { return preferredPageIndex; }
			set {
				Guard.ArgumentNonNegative(value, "preferredPageIndex");
				preferredPageIndex = value;
			}
		}
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new HeaderFooterDocumentLayoutPosition(DocumentLayout, PieceTable, LogPosition, PreferredPageIndex);
		}
		protected internal override Page LookupPage(PageCollection pages, int startIndex) {
			return LookupPreferredPage(pages);
		}
		protected internal override PageArea LookupPageArea(Page page, int startIndex) {
			if (page.Header != null && Object.ReferenceEquals(page.Header.PieceTable, this.PieceTable))
				return page.Header;
			if (page.Footer != null && Object.ReferenceEquals(page.Footer.PieceTable, this.PieceTable))
				return page.Footer;
			return null;
		}
		protected internal virtual Page LookupPreferredPage(PageCollection pages) {
			if (LogPosition > PieceTable.DocumentEndLogPosition)
				return null; 
			if (PreferredPageIndex >= pages.Count)
				return LookupPreferredPageBackward(pages);
			Page page = pages[PreferredPageIndex];
			if (IsPageMatch(page))
				return page;
			return LookupPreferredPageBackward(pages);
		}
		protected internal virtual bool IsPageMatch(Page page) {
			if (page.Header != null && Object.ReferenceEquals(page.Header.PieceTable, PieceTable))
				return true;
			if (page.Footer != null && Object.ReferenceEquals(page.Footer.PieceTable, PieceTable))
				return true;
			return false;
		}
		protected internal virtual Page LookupPreferredPageBackward(PageCollection pages) {
			for (int i = pages.Count - 1; i >= 0; i--) {
				if (IsPageMatch(pages[i]))
					return pages[i];
			}
			return null;
		}
	}
	#endregion
	#region TextBoxDocumentLayoutPosition
	public class TextBoxDocumentLayoutPosition : DocumentLayoutPosition {
		PieceTable anchorPieceTable;
		int preferredPageIndex;
		bool useLastAvailablePosition;
		public TextBoxDocumentLayoutPosition(DocumentLayout documentLayout, TextBoxContentType textBoxContentType, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(documentLayout, textBoxContentType.PieceTable, logPosition) {
			this.preferredPageIndex = preferredPageIndex;
		}
		public override PieceTable PieceTable { get { return anchorPieceTable != null ? anchorPieceTable : base.PieceTable; } }
		protected internal PieceTable AnchorPieceTable { get { return anchorPieceTable; } }
		public int PreferredPageIndex { get { return preferredPageIndex; } set { preferredPageIndex = value; } }
		protected override bool UseLastAvailablePosition { get { return useLastAvailablePosition; } set { useLastAvailablePosition = value; } }
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new TextBoxDocumentLayoutPosition(DocumentLayout, (TextBoxContentType)PieceTable.ContentType, LogPosition, preferredPageIndex);
		}
		protected internal override int GetPageIndex(PageCollection pages, int startIndex, int endIndex) {
			if (LogPosition > PieceTable.DocumentEndLogPosition)
				return -1;
			TextBoxContentType textBoxPieceTable = (TextBoxContentType)PieceTable.ContentType;
			PieceTable pieceTable = textBoxPieceTable.AnchorRun.PieceTable;
			if (pieceTable.IsHeaderFooter)
				return preferredPageIndex;
			this.anchorPieceTable = pieceTable;
			try {
				DocumentLogPosition anchorLogPosition = anchorPieceTable.GetRunLogPosition(textBoxPieceTable.AnchorRun);
				return FindBoxIndex<Page>(pages, new BoxStartAndLogPositionComparable<Page>(anchorPieceTable, anchorLogPosition), new ExactPageAndLogPositionComparable(anchorPieceTable, anchorLogPosition), IsIntersectedWithPrevBox, startIndex, endIndex);
			}
			finally {
				this.anchorPieceTable = null;
			}
		}
		protected internal override void CheckPageAreaPieceTable(PageArea pageArea) {
		}
		protected internal override PageArea LookupPageArea(Page page, int startIndex) {
			TextBoxContentType textBoxPieceTable = (TextBoxContentType)PieceTable.ContentType;
			FloatingObjectAnchorRun anchorRun = textBoxPieceTable.AnchorRun;
			PageArea result = LookupPageArea(page.InnerFloatingObjects, anchorRun);
			if (result != null)
				return result;
			result = LookupPageArea(page.InnerForegroundFloatingObjects, anchorRun);
			if (result != null)
				return result;
			result = LookupPageArea(page.InnerBackgroundFloatingObjects, anchorRun);
			if (result != null)
				return result;
			return null;
		}
		protected internal virtual PageArea LookupPageArea(IList<FloatingObjectBox> floatingObjects, FloatingObjectAnchorRun run) {
			if (floatingObjects == null)
				return null;
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++) {
				if (floatingObjects[i].GetFloatingObjectRun() == run)
					return floatingObjects[i].DocumentLayout.Pages.First.Areas.First;
			}
			return null;
		}
	}
	#endregion
	#region CommentDocumentLayoutPosition
	public class CommentDocumentLayoutPosition : DocumentLayoutPosition {
		PieceTable anchorPieceTable;
		int preferredPageIndex;
		bool useLastAvailablePosition;
		public CommentDocumentLayoutPosition(DocumentLayout documentLayout, CommentContentType commentContentType, DocumentLogPosition logPosition, int preferredPageIndex)
			: base(documentLayout, commentContentType.PieceTable, logPosition) {
			this.preferredPageIndex = preferredPageIndex;
		}
		public override PieceTable PieceTable { get { return anchorPieceTable != null ? anchorPieceTable : base.PieceTable; } }
		protected internal PieceTable AnchorPieceTable { get { return anchorPieceTable; } }
		public int PreferredPageIndex { get { return preferredPageIndex; } set { preferredPageIndex = value; } }
		protected override bool UseLastAvailablePosition { get { return useLastAvailablePosition; } set { useLastAvailablePosition = value; } }
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new CommentDocumentLayoutPosition(DocumentLayout, (CommentContentType)PieceTable.ContentType, LogPosition, preferredPageIndex);
		}
		protected internal override int GetPageIndex(PageCollection pages, int startIndex, int endIndex) {
			if (LogPosition > PieceTable.DocumentEndLogPosition)
				return -1;
			CommentContentType commentPieceTable = (CommentContentType)PieceTable.ContentType;
			PieceTable pieceTable = commentPieceTable.ReferenceComment.Content.PieceTable.DocumentModel.MainPieceTable;
			if (pieceTable.IsHeaderFooter)
				return preferredPageIndex;
			this.anchorPieceTable = pieceTable;
			try {
				DocumentLogPosition anchorLogPosition = commentPieceTable.ReferenceComment.Start;
				return FindBoxIndex<Page>(pages, new BoxStartAndLogPositionComparable<Page>(anchorPieceTable, anchorLogPosition), new ExactPageAndLogPositionComparable(anchorPieceTable, anchorLogPosition), IsIntersectedWithPrevBox, startIndex, endIndex);
			}
			finally {
				this.anchorPieceTable = null;
			}
		}
		protected internal override void CheckPageAreaPieceTable(PageArea pageArea) {
		}
		protected internal override PageArea LookupPageArea(Page page, int startIndex) {
			CommentViewInfoHelper helper = new CommentViewInfoHelper();
			CommentViewInfo commentViewInfo = helper.FindCommentViewInfo(page, this.PieceTable);
			if (commentViewInfo != null)
				return commentViewInfo.CommentDocumentLayout.Pages.First.Areas.First;
			return null;
		}
	}
#endregion
}
