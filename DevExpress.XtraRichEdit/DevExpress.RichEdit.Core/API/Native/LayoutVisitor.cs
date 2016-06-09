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
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using PieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
namespace DevExpress.XtraRichEdit.API.Layout {
	#region LayoutVisitor
	public abstract class LayoutVisitor {
		internal virtual bool VisitCommentsManually { get { return false; } }
		public virtual void Visit(LayoutElement element) {
			element.Accept(this);
		}
		protected internal virtual void VisitPage(LayoutPage page) {
			if (page.Header != null)
				VisitHeader(page.Header);
			if (page.Footer != null)
				VisitFooter(page.Footer);
			for (int i = 0; i < page.PageAreas.Count; i++)
				VisitPageArea(page.PageAreas[i]);
			if (!VisitCommentsManually)
				VisitComments(page);
		}
		protected internal virtual void VisitPageArea(LayoutPageArea pageArea) {
			VisitArea(pageArea);
		}
		protected internal virtual void VisitColumn(LayoutColumn column) {
			VisitLayoutElementsCollection<LayoutRow>(column.Rows);
			VisitLayoutElementsCollection<LayoutTable>(column.Tables);
			VisitLayoutElementsCollection<LineNumberBox>(column.LineNumbers);
		}
		internal virtual void VisitComments(LayoutPage page) {
			VisitLayoutElementsCollection<LayoutComment>(page.Comments);
		}
		protected internal virtual void VisitComment(LayoutComment comment) {
			VisitLayoutElementsCollection<LayoutColumn>(comment.Columns);
		}
		protected internal virtual void VisitHeader(LayoutHeader header) {
			VisitArea(header);
		}
		protected internal virtual void VisitFooter(LayoutFooter footer) {
			VisitArea(footer);
		}
		protected internal virtual void VisitTextBox(LayoutTextBox textBox) {
			VisitLayoutElementsCollection<LayoutRow>(textBox.Rows);
			VisitLayoutElementsCollection<LayoutTable>(textBox.Tables);
		}
		protected internal virtual void VisitFloatingPicture(LayoutFloatingPicture floatingPicture) {
		}
		protected internal virtual void VisitTable(LayoutTable table) {
			VisitLayoutElementsCollection<LayoutTableRow>(table.TableRows);
		}
		protected internal virtual void VisitTableRow(LayoutTableRow tableRow) {
			VisitLayoutElementsCollection<LayoutTableCell>(tableRow.TableCells);
		}
		protected internal virtual void VisitTableCell(LayoutTableCell tableCell) {
			VisitLayoutElementsCollection<LayoutRow>(tableCell.Rows);
			VisitLayoutElementsCollection<LayoutTable>(tableCell.NestedTables);
		}
		internal virtual void VisitRowBackground(RowExtendedBoxes rowExtendedBoxes) {
			VisitLayoutElementsCollection<FieldHighlightAreaBox>(rowExtendedBoxes.FieldHighlightAreas);
			VisitLayoutElementsCollection<RangePermissionHighlightAreaBox>(rowExtendedBoxes.RangePermissionHighlightAreas);
			VisitLayoutElementsCollection<CommentHighlightAreaBox>(rowExtendedBoxes.CommentHighlightAreas);
			VisitLayoutElementsCollection<HighlightAreaBox>(rowExtendedBoxes.HighlightAreas);
		}
		protected internal virtual void VisitRow(LayoutRow row) {
			RowExtendedBoxes rowExtendedBoxes = row.ExtendedBoxes;
			VisitInnerParagraphFrames(row);
			VisitRowBackground(rowExtendedBoxes);
			if (row.NumberingListBox != null)
				row.NumberingListBox.Accept(this);
			VisitLayoutElementsCollection<Box>(row.Boxes);
			VisitUnderlineBoxes(rowExtendedBoxes.Underlines);
			VisitStrikeoutBoxes(rowExtendedBoxes.Strikeouts);
			VisitErrorBoxes();
			VisitBookmarkBoxes(rowExtendedBoxes.BookmarkBoxes);
			VisitLayoutElementsCollection<RangePermissionBox>(rowExtendedBoxes.RangePermissionBoxes);
			VisitLayoutElementsCollection<CommentBox>(rowExtendedBoxes.CommentBoxes);
			VisitImeBoxes();
			VisitHiddenTextUnderlineBoxes(rowExtendedBoxes.HiddenTextUnderlineBoxes);
			VisitSpecialAreaTextBoxes(rowExtendedBoxes.SpecialAreaBoxes);
			VisitLayoutElementsCollection<CustomMarkBox>(rowExtendedBoxes.CustomMarkBoxes);
		}
		void VisitInnerParagraphFrames(LayoutRow row) {
			List<DevExpress.XtraRichEdit.Layout.ParagraphFrameBox> frames = null;
			LayoutColumn column = row.GetParentByType<LayoutColumn>();
			if (column != null)
				frames = column.ModelColumn.InnerParagraphFrames;
			else {
				LayoutTextBox textBox = row.GetParentByType<LayoutTextBox>();
				if (textBox != null)
					frames = textBox.ModelFloatingObject.DocumentLayout.Pages[0].Areas[0].Columns[0].InnerParagraphFrames;
			}
			if (frames != null) {
				for (int i = 0; i < frames.Count; i++)
					if (frames[i].FirstRow == row.ModelRow)
						VisitParagraphFrame(frames[i]);
			}
		}
		internal virtual void VisitUnderlineBoxes(UnderlineBoxCollection underlineBoxes) {
			VisitLayoutElementsCollection<UnderlineBox>(underlineBoxes);
		}
		internal virtual void VisitStrikeoutBoxes(StrikeoutBoxCollection strikeoutBoxes) {
			VisitLayoutElementsCollection<StrikeoutBox>(strikeoutBoxes);
		}
		internal virtual void VisitBookmarkBoxes(BookmarkBoxCollection bookmarkBoxes) {
			VisitLayoutElementsCollection<BookmarkBox>(bookmarkBoxes);
		}
		protected internal virtual void VisitPlainTextBox(PlainTextBox plainTextBox) {
		}
		protected internal virtual void VisitSpecialTextBox(PlainTextBox specialTextBox) {
		}
		protected internal virtual void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
		}
		protected internal virtual void VisitFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
		}
		protected internal virtual void VisitSpaceBox(PlainTextBox spaceBox) {
		}
		protected internal virtual void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
		}
		protected internal virtual void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
		}
		protected internal virtual void VisitLineBreakBox(PlainTextBox lineBreakBox) {
		}
		protected internal virtual void VisitPageBreakBox(PlainTextBox pageBreakBox) {
		}
		protected internal virtual void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
		}
		protected internal virtual void VisitHyphenBox(PlainTextBox hyphen) {
		}
		protected internal virtual void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
		}
		protected internal virtual void VisitPageNumberBox(PlainTextBox pageNumberBox) {
		}
		protected internal virtual void VisitNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
		}
		protected internal virtual void VisitNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
			numberingListWithSeparatorBox.Separator.Accept(this);
		}
		protected internal virtual void VisitUnderlineBox(UnderlineBox underlineBox) {
		}
		protected internal virtual void VisitStrikeoutBox(StrikeoutBox strikeoutBox) {
		}
		protected internal virtual void VisitHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
		}
		protected internal virtual void VisitBookmarkStartBox(BookmarkBox bookmarkStartBox) {
		}
		protected internal virtual void VisitBookmarkEndBox(BookmarkBox bookmarkEndBox) {
		}
		protected internal virtual void VisitHiddenTextUnderlineBox(HiddenTextUnderlineBox hiddenTextUnderlineBox) {
		}
		protected internal virtual void VisitLineNumberBox(LineNumberBox lineNumberBox) {
		}
		internal virtual void VisitCustomRunBox(PlainTextBox customRunBox) {
		}
		internal virtual void VisitDataContainerRunBox(PlainTextBox dataContainerRunBox) {
		}
		protected internal virtual void VisitFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
		}
		protected internal virtual void VisitRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
		}
		protected internal virtual void VisitRangePermissionStartBox(RangePermissionBox rangePermissionStartBox) {
		}
		protected internal virtual void VisitRangePermissionEndBox(RangePermissionBox rangePermissionEndBox) {
		}
		protected internal virtual void VisitCommentHighlightAreaBox(CommentHighlightAreaBox commentHighlightAreaBox) {
		}
		protected internal virtual void VisitCommentStartBox(CommentBox commentStartBox) {
		}
		protected internal virtual void VisitCommentEndBox(CommentBox commentEndBox) {
		}
		internal void VisitLayoutElementsCollection<T>(LayoutElementCollection<T> collection) where T : LayoutElement {
			int count = collection.Count;
			for (int i = 0; i < count; i++)
				collection[i].Accept(this);
		}
		void VisitArea(LayoutPageAreaBase area) {
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = ((IPieceTableContainer)area).PieceTable;
			LayoutPage page = area.GetParentByType<LayoutPage>();
			VisitFloatingObjects(page.BackgroundFloatingObjects, pieceTable);
			VisitLayoutElementsCollection<LayoutColumn>(area.Columns);
			VisitParagraphFrames(page.ParagraphFrames, pieceTable);
			VisitFloatingObjects(page.FloatingObjects, pieceTable);
			VisitFloatingObjects(page.ForegroundFloatingObjects, pieceTable);
		}
		void VisitFloatingObjects(LayoutFloatingObjectCollection collection, DevExpress.XtraRichEdit.Model.PieceTable pieceTable) {
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				LayoutFloatingObject floatingObject = collection[i];
				if (Object.ReferenceEquals(floatingObject.ModelFloatingObject.PieceTable, pieceTable))
					floatingObject.Accept(this);
			}
		}
		void VisitParagraphFrames(LayoutFloatingParagraphFrameCollection collection, DevExpress.XtraRichEdit.Model.PieceTable pieceTable) {
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				LayoutFloatingParagraphFrame paragraphFrame = collection[i];
				if (Object.ReferenceEquals(paragraphFrame.ModelParagraphFrame.PieceTable, pieceTable))
					paragraphFrame.Accept(this);
			}
		}
		internal virtual void VisitFloatingParagraphFrame(LayoutFloatingParagraphFrame paragraphFrame) {
			VisitFloatingParagraphFrame(paragraphFrame.ModelParagraphFrame);
		}
		internal virtual void VisitFloatingParagraphFrame(DevExpress.XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) { }
		internal virtual void VisitParagraphFrame(DevExpress.XtraRichEdit.Layout.ParagraphFrameBox paragraphFrame) { }
		internal virtual void VisitErrorBoxes() { }
		internal virtual void VisitSpecialAreaBox(SpecialAreaBox specialAreaBox) { }
		internal virtual void VisitCustomMarkBox(CustomMarkBox customMarkBox) { }
		internal virtual void VisitImeBoxes() { }
		internal virtual void VisitHiddenTextUnderlineBoxes(HiddenTextUnderlineBoxCollection collection) {
			VisitLayoutElementsCollection<HiddenTextUnderlineBox>(collection);
		}
		internal virtual void VisitSpecialAreaTextBoxes(LayoutElementCollection<SpecialAreaBox> collection) {
			VisitLayoutElementsCollection<SpecialAreaBox>(collection);
		}
	}
	#endregion
	#region TextCollectorVisitor
	class TextCollectorVisitor : LayoutVisitor {
		public TextCollectorVisitor()
			: base() {
			Text = String.Empty;
		}
		public string Text { get; private set; }
		protected internal override void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
			Text += columnBreakBox.Text;
			base.VisitColumnBreakBox(columnBreakBox);
		}
		protected internal override void VisitHyphenBox(PlainTextBox hyphen) {
			Text += hyphen.Text;
			base.VisitHyphenBox(hyphen);
		}
		protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
			Text += lineBreakBox.Text;
			base.VisitLineBreakBox(lineBreakBox);
		}
		protected internal override void VisitLineNumberBox(LineNumberBox lineNumberBox) {
			Text += lineNumberBox.Text;
			base.VisitLineNumberBox(lineNumberBox);
		}
		protected internal override void VisitNumberingListMarkBox(NumberingListMarkBox numberingListMarkBox) {
			Text += numberingListMarkBox.Text;
			base.VisitNumberingListMarkBox(numberingListMarkBox);
		}
		protected internal override void VisitNumberingListWithSeparatorBox(NumberingListWithSeparatorBox numberingListWithSeparatorBox) {
			Text += numberingListWithSeparatorBox.Text;
			base.VisitNumberingListWithSeparatorBox(numberingListWithSeparatorBox);
		}
		protected internal override void VisitPageBreakBox(PlainTextBox pageBreakBox) {
			Text += pageBreakBox.Text;
			base.VisitPageBreakBox(pageBreakBox);
		}
		protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
			Text += paragraphMarkBox.Text;
			base.VisitParagraphMarkBox(paragraphMarkBox);
		}
		protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
			Text += plainTextBox.Text;
			base.VisitPlainTextBox(plainTextBox);
		}
		protected internal override void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
			Text += sectionBreakBox.Text;
			base.VisitSectionBreakBox(sectionBreakBox);
		}
		protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
			Text += spaceBox.Text;
			base.VisitSpaceBox(spaceBox);
		}
		protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
			Text += specialTextBox.Text;
			base.VisitSpecialTextBox(specialTextBox);
		}
		protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
			Text += tabSpaceBox.Text;
			base.VisitTabSpaceBox(tabSpaceBox);
		}
		protected internal override void VisitPageNumberBox(PlainTextBox pageNumberBox) {
			Text += pageNumberBox.Text;
			base.VisitPageNumberBox(pageNumberBox);
		}
	}
	#endregion
	#region LayoutElementSearcher
	class LayoutElementSearcher : LayoutVisitor {
		int position;
		PieceTable pieceTable;
		RangedLayoutElement findedElement;
		Func<RangedLayoutElement, bool> comparison;
		public LayoutElementSearcher(int position, PieceTable pieceTable, Func<RangedLayoutElement, bool> comparison) {
			this.position = position;
			this.pieceTable = pieceTable;
			this.comparison = comparison;
		}
		public int Position { get { return position; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		Func<RangedLayoutElement, bool> Comparison { get { return comparison; } }
		public RangedLayoutElement FindedElement { get { return findedElement; } }
		protected internal override void VisitColumn(LayoutColumn column) {
			if (Comparison(column)) {
				FindElement(column);
				return;
			}
			base.VisitColumn(column);
		}
		protected internal override void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
			if (Comparison(columnBreakBox)) {
				FindElement(columnBreakBox);
				return;
			}
			base.VisitColumnBreakBox(columnBreakBox);
		}
		protected internal override void VisitComment(LayoutComment comment) {
			if (!IsInSearchingDocument((IPieceTableContainer)comment))
				return;
			if (Comparison(comment)) {
				FindElement(comment);
				return;
			}
			base.VisitComment(comment);
		}
		protected internal override void VisitCommentHighlightAreaBox(CommentHighlightAreaBox commentHighlightAreaBox) {
			if (Comparison(commentHighlightAreaBox)) {
				FindElement(commentHighlightAreaBox);
				return;
			}
			base.VisitCommentHighlightAreaBox(commentHighlightAreaBox);
		}
		protected internal override void VisitFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox) {
			if (Comparison(fieldHighlightAreaBox)) {
				FindElement(fieldHighlightAreaBox);
				return;
			}
			base.VisitFieldHighlightAreaBox(fieldHighlightAreaBox);
		}
		protected internal override void VisitFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
			if (Comparison(floatingObjectAnchorBox)) {
				FindElement(floatingObjectAnchorBox);
				return;
			}
			base.VisitFloatingObjectAnchorBox(floatingObjectAnchorBox);
		}
		protected internal override void VisitFooter(LayoutFooter footer) {
			if (Comparison(footer) && IsInSearchingDocument((IPieceTableContainer)footer)) {
				FindElement(footer);
				return;
			}
			base.VisitFooter(footer);
		}
		protected internal override void VisitHeader(LayoutHeader header) {
			if (Comparison(header) && IsInSearchingDocument((IPieceTableContainer)header)) {
				FindElement(header);
				return;
			}
			base.VisitHeader(header);
		}
		protected internal override void VisitHiddenTextUnderlineBox(HiddenTextUnderlineBox hiddenTextUnderlineBox) {
			if (Comparison(hiddenTextUnderlineBox)) {
				FindElement(hiddenTextUnderlineBox);
				return;
			}
			base.VisitHiddenTextUnderlineBox(hiddenTextUnderlineBox);
		}
		protected internal override void VisitHighlightAreaBox(HighlightAreaBox highlightAreaBox) {
			if (Comparison(highlightAreaBox)) {
				FindElement(highlightAreaBox);
				return;
			}
			base.VisitHighlightAreaBox(highlightAreaBox);
		}
		protected internal override void VisitHyphenBox(PlainTextBox hyphen) {
			if (Comparison(hyphen)) {
				FindElement(hyphen);
				return;
			}
			base.VisitHyphenBox(hyphen);
		}
		protected internal override void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
			if (Comparison(inlinePictureBox)) {
				FindElement(inlinePictureBox);
				return;
			}
			base.VisitInlinePictureBox(inlinePictureBox);
		}
		protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
			if (Comparison(lineBreakBox)) {
				FindElement(lineBreakBox);
				return;
			}
			base.VisitLineBreakBox(lineBreakBox);
		}
		protected internal override void VisitPageArea(LayoutPageArea pageArea) {
			if (Comparison(pageArea) && IsInSearchingDocument((IPieceTableContainer)pageArea)) {
				FindElement(pageArea);
				return;
			}
			base.VisitPageArea(pageArea);
		}
		protected internal override void VisitPageBreakBox(PlainTextBox pageBreakBox) {
			if (Comparison(pageBreakBox)) {
				FindElement(pageBreakBox);
				return;
			}
			base.VisitPageBreakBox(pageBreakBox);
		}
		protected internal override void VisitPageNumberBox(PlainTextBox pageNumberBox) {
			if (Comparison(pageNumberBox)) {
				FindElement(pageNumberBox);
				return;
			}
			base.VisitPageNumberBox(pageNumberBox);
		}
		protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
			if (Comparison(paragraphMarkBox)) {
				FindElement(paragraphMarkBox);
				return;
			}
			base.VisitParagraphMarkBox(paragraphMarkBox);
		}
		protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
			if (Comparison(plainTextBox)) {
				FindElement(plainTextBox);
				return;
			}
			base.VisitPlainTextBox(plainTextBox);
		}
		protected internal override void VisitRangePermissionHighlightAreaBox(RangePermissionHighlightAreaBox rangePermissionHighlightAreaBox) {
			if (Comparison(rangePermissionHighlightAreaBox)) {
				FindElement(rangePermissionHighlightAreaBox);
				return;
			}
			base.VisitRangePermissionHighlightAreaBox(rangePermissionHighlightAreaBox);
		}
		protected internal override void VisitRow(LayoutRow row) {
			if (Comparison(row)) {
				FindElement(row);
				return;
			}
			base.VisitRow(row);
		}
		protected internal override void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
			if (Comparison(sectionBreakBox)) {
				FindElement(sectionBreakBox);
				return;
			}
			base.VisitSectionBreakBox(sectionBreakBox);
		}
		protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
			if (Comparison(spaceBox)) {
				FindElement(spaceBox);
				return;
			}
			base.VisitSpaceBox(spaceBox);
		}
		protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
			if (Comparison(specialTextBox)) {
				FindElement(specialTextBox);
				return;
			}
			base.VisitSpecialTextBox(specialTextBox);
		}
		protected internal override void VisitStrikeoutBox(StrikeoutBox strikeoutBox) {
			if (Comparison(strikeoutBox)) {
				FindElement(strikeoutBox);
				return;
			}
			base.VisitStrikeoutBox(strikeoutBox);
		}
		protected internal override void VisitTable(LayoutTable table) {
			if (Comparison(table)) {
				FindElement(table);
				return;
			}
			base.VisitTable(table);
		}
		protected internal override void VisitTableCell(LayoutTableCell tableCell) {
			if (Comparison(tableCell)) {
				FindElement(tableCell);
				if (tableCell.NestedTables.Count == 0)
					return;
			}
			base.VisitTableCell(tableCell);
		}
		protected internal override void VisitTableRow(LayoutTableRow tableRow) {
			if (Comparison(tableRow)) {
				FindElement(tableRow);
				return;
			}
			base.VisitTableRow(tableRow);
		}
		protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
			if (Comparison(tabSpaceBox)) {
				FindElement(tabSpaceBox);
				return;
			}
			base.VisitTabSpaceBox(tabSpaceBox);
		}
		protected internal override void VisitTextBox(LayoutTextBox textBox) {
			if (!IsInSearchingDocument((IPieceTableContainer)textBox))
				return;
			if (Comparison(textBox)) {
				FindElement(textBox);
				return;
			}
			base.VisitTextBox(textBox);
		}
		protected internal override void VisitUnderlineBox(UnderlineBox underlineBox) {
			if (Comparison(underlineBox)) {
				FindElement(underlineBox);
				return;
			}
			base.VisitUnderlineBox(underlineBox);
		}
		void FindElement(RangedLayoutElement element) {
			if (element.Range.Contains(Position)) {
				IPieceTableContainer pieceTableContainer = element as IPieceTableContainer;
				if (pieceTableContainer == null)
					pieceTableContainer = element.GetParentByType<IPieceTableContainer>();
				if (IsInSearchingDocument(pieceTableContainer))
					this.findedElement = element;
			}
		}
		bool IsInSearchingDocument(IPieceTableContainer container) {
			return Object.ReferenceEquals(PieceTable, container.PieceTable);
		}
	}
	#endregion
	public enum LayoutLevel {
		Page,
		PageArea,
		Column,
		Table,
		TableRow,
		TableCell,
		Row,
		Box
	}
	public enum InitialState { Start, End }
	#region LayoutIterator
	public class LayoutIterator {
		enum LogicalDirection { Backward, Forward }
		enum MoveResult { True, False, MoveToNextPage }
		#region Fields
		readonly DocumentLayout documentLayout;
		FixedRange range;
		NativeSubDocument document;
		bool isLayoutValid = true;
		int currentIndex;
		LayoutElementCollection<LayoutElement> linkCollection;
		LayoutTreeLinkCollector collector;
		int currentPageIndex;
		int? startPageIndex, endPageIndex;
		#endregion
		public LayoutIterator(DocumentLayout documentLayout) {
			this.documentLayout = documentLayout;
			this.document = (NativeSubDocument)DocumentLayout.Provider.Document;
			DocumentLayout.DocumentLayoutInvalidated += OnDocumentLayoutInvalidated;
			Reset();
		}
		public LayoutIterator(DocumentLayout documentLayout, DocumentRange range)
			: this(documentLayout, ((NativeDocumentRange)range).Start.Document, new FixedRange(range.Start.ToInt(), range.Length)) {
		}
		public LayoutIterator(DocumentLayout documentLayout, SubDocument document, FixedRange range) {
			if (range.Length == 0)
				throw new ArgumentException("Cannot iterate over a range with zero length.", "range");
			this.documentLayout = documentLayout;
			DocumentLayout.DocumentLayoutInvalidated += OnDocumentLayoutInvalidated;
			this.range = range;
			this.document = (NativeSubDocument)document;
			Reset();
		}
		#region Properties
		public bool IsLayoutValid { get { return isLayoutValid; } }
		public LayoutElement Current {
			get {
				CheckIsValid();
				return (CurrentIndex >= 0 && CurrentIndex < LinkCollection.Count) ? LinkCollection[CurrentIndex] : null;
			}
		}
		public bool IsStart {
			get {
				CheckIsValid();
				return Current != null && CanMovePrevious() == MoveResult.False;
			}
		}
		public bool IsEnd {
			get {
				CheckIsValid();
				return CanMoveNext() == MoveResult.False && Current != null;
			}
		}
		DocumentLayout DocumentLayout { get { return documentLayout; } }
		int CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }
		LayoutElementCollection<LayoutElement> LinkCollection { get { return linkCollection; } }
		LayoutTreeLinkCollector Collector { get { return collector; } }
		int CurrentPageIndex {
			get { return currentPageIndex; }
			set {
				if (currentPageIndex == value)
					return;
				currentPageIndex = value;
				OnCurrentPageIndexChanged();
			}
		}
		FixedRange Range { get { return range; } }
		NativeSubDocument Document { get { return document; } }
		#endregion
		void OnCurrentPageIndexChanged() {
			if (LinkCollection != null)
				LinkCollection.Clear();
		}
		void OnDocumentLayoutInvalidated(object sender, DocumentLayoutInvalidatedEventArgs e) {
			DocumentLayout.DocumentLayoutInvalidated -= OnDocumentLayoutInvalidated;
			if (LinkCollection != null)
				LinkCollection.Clear();
			this.collector = null;
			this.isLayoutValid = false;
		}
		public bool MoveNext() {
			CheckIsValid();
			MoveResult canMove = CanMoveNext();
			if (canMove == MoveResult.False)
				return false;
			if (canMove == MoveResult.MoveToNextPage) {
				CurrentPageIndex++;
				InitializeLinkCollection();
				CurrentIndex = 0;
			}
			else
				CurrentIndex++;
			return true;
		}
		public bool MovePrevious() {
			CheckIsValid();
			MoveResult canMove = CanMovePrevious();
			if (canMove == MoveResult.False)
				return false;
			if (canMove == MoveResult.MoveToNextPage) {
				CurrentPageIndex--;
				InitializeLinkCollection();
				CurrentIndex = LinkCollection.Count - 1;
			}
			else
				CurrentIndex--;
			return true;
		}
		public bool MoveNext(LayoutLevel layoutLevel) {
			return Move(layoutLevel, true, LogicalDirection.Forward);
		}
		public bool MovePrevious(LayoutLevel layoutLevel) {
			return Move(layoutLevel, true, LogicalDirection.Backward);
		}
		public bool MoveNext(LayoutLevel layoutLevel, bool allowTreeTraversal) {
			return Move(layoutLevel, allowTreeTraversal, LogicalDirection.Forward);
		}
		public bool MovePrevious(LayoutLevel layoutLevel, bool allowTreeTraversal) {
			return Move(layoutLevel, allowTreeTraversal, LogicalDirection.Backward);
		}
		bool Move(LogicalDirection direction) {
			return direction == LogicalDirection.Forward ? MoveNext() : MovePrevious();
		}
		bool Move(LayoutLevel layoutLevel, bool allowTreeTraversal, LogicalDirection direction) {
			int oldIndex = CurrentIndex;
			int oldPageIndex = CurrentPageIndex;
			LayoutLevel? oldLevel = null;
			LayoutElement parent = null;
			if (Current != null) {
				parent = Current.Parent;
				oldLevel = GetLevel(Current);
			}
			while (Move(direction)) {
				LayoutLevel level = GetLevel(Current);
				if (layoutLevel == level) {
					if (!allowTreeTraversal && parent != null && oldLevel.HasValue && parent != Current.Parent && oldLevel.Value == level)
						break;
					return true;
				}
			}
			CurrentPageIndex = oldPageIndex;
			InitializeLinkCollection();
			CurrentIndex = oldIndex;
			return false;
		}
		public void Reset() {
			Reset(InitialState.Start);
		}
		public void Reset(InitialState initialState) {
			CheckIsValid();
			this.linkCollection = new LayoutElementCollection<LayoutElement>();
			this.collector = new LayoutTreeLinkCollector(Document.PieceTable, Range, LinkCollection);
			InitializePageIndices(initialState);
			InitializeLinkCollection();
			CurrentIndex = initialState == InitialState.Start ? -1 : LinkCollection.Count;
		}
		LayoutLevel GetLevel(LayoutElement element) {
			switch (element.Type) {
				case LayoutType.Page:
					return LayoutLevel.Page;
				case LayoutType.PageArea:
					return LayoutLevel.PageArea;
				case LayoutType.Column:
					return LayoutLevel.Column;
				case LayoutType.Table:
					return LayoutLevel.Table;
				case LayoutType.TableRow:
					return LayoutLevel.TableRow;
				case LayoutType.TableCell:
					return LayoutLevel.TableCell;
				case LayoutType.Row:
					return LayoutLevel.Row;
				default:
					return LayoutLevel.Box;
			}
		}
		void InitializePageIndices(InitialState initialState) {
			if (Range == null || !Document.PieceTable.IsMain) {
				this.startPageIndex = null;
				this.endPageIndex = null;
				CurrentPageIndex = initialState == InitialState.Start ? 0 : DocumentLayout.GetPageCount() - 1;
			}
			else {
				LayoutPageArea pageArea = DocumentLayout.GetElement<LayoutPageArea>(Document.CreatePosition(Range.Start));
				this.startPageIndex = DocumentLayout.GetPageIndex(pageArea);
				pageArea = DocumentLayout.GetElement<LayoutPageArea>(Document.CreatePosition(Range.Start + Range.Length - 1));
				this.endPageIndex = DocumentLayout.GetPageIndex(pageArea);
				CurrentPageIndex = initialState == InitialState.Start ? this.startPageIndex.Value : this.endPageIndex.Value;
			}
		}
		bool IsValidPageIndex(int pageIndex) {
			if (pageIndex < 0)
				return false;
			if (!this.startPageIndex.HasValue || !this.endPageIndex.HasValue)
				return pageIndex <= DocumentLayout.GetPageCount() - 1;
			else
				return pageIndex >= this.startPageIndex.Value && pageIndex <= this.endPageIndex.Value;
		}
		void InitializeLinkCollection() {
			if (LinkCollection.Count > 0)
				return;
			Collector.Visit(DocumentLayout.GetPage(CurrentPageIndex));
		}
		void CheckIsValid() {
			if (!IsLayoutValid)
				throw new InvalidOperationException("The document layout has been invalidated. The iterator cannot operate with outdated layout. Create a new iterator instance.");
		}
		MoveResult CanMoveNext() {
			if (CurrentIndex < LinkCollection.Count - 1)
				return MoveResult.True;
			int nextPageIndex = CurrentPageIndex + 1;
			return (IsValidPageIndex(nextPageIndex) && IsPageContainsSomeElementsFromRange(nextPageIndex)) ? MoveResult.MoveToNextPage : MoveResult.False;
		}
		MoveResult CanMovePrevious() {
			if (CurrentIndex > 0)
				return MoveResult.True;
			int prevPageIndex = CurrentPageIndex - 1;
			return (IsValidPageIndex(prevPageIndex) && IsPageContainsSomeElementsFromRange(prevPageIndex)) ? MoveResult.MoveToNextPage : MoveResult.False;
		}
		bool IsPageContainsSomeElementsFromRange(int pageIndex) {
			if (Range == null || Document.PieceTable.IsMain)
				return true;
			return DocumentLayout.GetElementFromPage<RangedLayoutElement>(Range.Start, Document.PieceTable, DocumentLayout.GetPage(pageIndex)) != null;
		}
	}
	#endregion
	#region LayoutTreeLinkCollector
	class LayoutTreeLinkCollector : LayoutVisitor {
		LayoutElementCollection<LayoutElement> collection;
		PieceTable pieceTable;
		FixedRange range;
		public LayoutTreeLinkCollector(PieceTable pieceTable, FixedRange range, LayoutElementCollection<LayoutElement> collection) {
			this.pieceTable = pieceTable;
			this.collection = collection;
			this.range = range;
		}
		public LayoutElementCollection<LayoutElement> Collection { get { return collection; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public FixedRange Range { get { return range; } }
		protected internal override void VisitColumn(LayoutColumn column) {
			IPieceTableContainer pieceTableContainer = column.GetParentByType<IPieceTableContainer>();
			if (pieceTableContainer.PieceTable != PieceTable)
				return;
			AddElement(column);
			base.VisitColumn(column);
		}
		protected internal override void VisitColumnBreakBox(PlainTextBox columnBreakBox) {
			AddElement(columnBreakBox);
			base.VisitColumnBreakBox(columnBreakBox);
		}
		protected internal override void VisitComment(LayoutComment comment) {
			AddElement(comment);
			base.VisitComment(comment);
		}
		protected internal override void VisitFloatingObjectAnchorBox(FloatingObjectAnchorBox floatingObjectAnchorBox) {
			AddElement(floatingObjectAnchorBox);
			base.VisitFloatingObjectAnchorBox(floatingObjectAnchorBox);
		}
		protected internal override void VisitFooter(LayoutFooter footer) {
			AddElement(footer);
			base.VisitFooter(footer);
		}
		protected internal override void VisitHeader(LayoutHeader header) {
			AddElement(header);
			base.VisitHeader(header);
		}
		protected internal override void VisitHyphenBox(PlainTextBox hyphen) {
			AddElement(hyphen);
			base.VisitHyphenBox(hyphen);
		}
		protected internal override void VisitInlinePictureBox(InlinePictureBox inlinePictureBox) {
			AddElement(inlinePictureBox);
			base.VisitInlinePictureBox(inlinePictureBox);
		}
		protected internal override void VisitLineBreakBox(PlainTextBox lineBreakBox) {
			AddElement(lineBreakBox);
			base.VisitLineBreakBox(lineBreakBox);
		}
		protected internal override void VisitPage(LayoutPage page) {
			if (page.Document.PieceTable == PieceTable)
				AddElement(page);
			base.VisitPage(page);
		}
		protected internal override void VisitPageArea(LayoutPageArea pageArea) {
			AddElement(pageArea);
			base.VisitPageArea(pageArea);
		}
		protected internal override void VisitPageBreakBox(PlainTextBox pageBreakBox) {
			AddElement(pageBreakBox);
			base.VisitPageBreakBox(pageBreakBox);
		}
		protected internal override void VisitPageNumberBox(PlainTextBox pageNumberBox) {
			AddElement(pageNumberBox);
			base.VisitPageNumberBox(pageNumberBox);
		}
		protected internal override void VisitParagraphMarkBox(PlainTextBox paragraphMarkBox) {
			AddElement(paragraphMarkBox);
			base.VisitParagraphMarkBox(paragraphMarkBox);
		}
		protected internal override void VisitPlainTextBox(PlainTextBox plainTextBox) {
			AddElement(plainTextBox);
			base.VisitPlainTextBox(plainTextBox);
		}
		protected internal override void VisitRow(LayoutRow row) {
			AddElement(row);
			base.VisitRow(row);
		}
		protected internal override void VisitSectionBreakBox(PlainTextBox sectionBreakBox) {
			AddElement(sectionBreakBox);
			base.VisitSectionBreakBox(sectionBreakBox);
		}
		protected internal override void VisitSpaceBox(PlainTextBox spaceBox) {
			AddElement(spaceBox);
			base.VisitSpaceBox(spaceBox);
		}
		protected internal override void VisitSpecialTextBox(PlainTextBox specialTextBox) {
			AddElement(specialTextBox);
			base.VisitSpecialTextBox(specialTextBox);
		}
		protected internal override void VisitTable(LayoutTable table) {
			AddElement(table);
			base.VisitTable(table);
		}
		protected internal override void VisitTableCell(LayoutTableCell tableCell) {
			AddElement(tableCell);
			base.VisitTableCell(tableCell);
		}
		protected internal override void VisitTabSpaceBox(PlainTextBox tabSpaceBox) {
			AddElement(tabSpaceBox);
			base.VisitTabSpaceBox(tabSpaceBox);
		}
		protected internal override void VisitTableRow(LayoutTableRow tableRow) {
			AddElement(tableRow);
			base.VisitTableRow(tableRow);
		}
		protected internal override void VisitTextBox(LayoutTextBox textBox) {
			if (((IPieceTableContainer)textBox).PieceTable != PieceTable)
				return;
			AddElement(textBox);
			base.VisitTextBox(textBox);
		}
		void AddElement(LayoutElement element) {
			if (Range == null) {
				Collection.Add(element);
				return;
			}
			if (!CanAddElement(element))
				return;
			FixedRange elementRange = null;
			RangedLayoutElement rangedElement = element as RangedLayoutElement;
			if (rangedElement != null)
				elementRange = rangedElement.Range;
			else {
				LayoutPage page = element as LayoutPage;
				if (page == null)
					return;
				int start = page.PageAreas.First.Range.Start;
				int end = page.PageAreas.Last.Range.Start + page.PageAreas.Last.Range.Length;
				elementRange = new FixedRange(start, end - start);
			}
			if (elementRange != null && Range.Intersect(elementRange))
				Collection.Add(element);
		}
		bool CanAddElement(LayoutElement element) {
			PieceTable pieceTable = null;
			IPieceTableContainer pieceTableContainer = element as IPieceTableContainer;
			if (pieceTableContainer == null)
				pieceTableContainer = element.GetParentByType<IPieceTableContainer>();
			if (pieceTableContainer != null)
				pieceTable = pieceTableContainer.PieceTable;
			else {
				LayoutPage page = element as LayoutPage;
				if (page != null)
					pieceTable = page.Document.PieceTable;
			}
			return pieceTable == PieceTable;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Layout.Internal {
	using XtraRichEdit.Layout;
	using DevExpress.XtraRichEdit.Layout.TableLayout;
	using DevExpress.Compatibility.System.Drawing;
	using System.Drawing;
	public class LayoutVisitorInternal : LayoutVisitor {
		internal override void VisitFloatingParagraphFrame(LayoutFloatingParagraphFrame paragraphFrame) {
			VisitFloatingParagraphFrameInternal(paragraphFrame);
		}
		protected virtual void VisitFloatingParagraphFrameInternal(LayoutFloatingParagraphFrame paragraphFrame) {
			VisitLayoutElementsCollection<LayoutRow>(paragraphFrame.Rows);
			VisitLayoutElementsCollection<LayoutTable>(paragraphFrame.Tables);
		}
	}
}
