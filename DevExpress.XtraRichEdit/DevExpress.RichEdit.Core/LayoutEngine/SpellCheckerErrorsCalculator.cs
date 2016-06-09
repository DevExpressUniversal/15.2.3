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
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraSpellChecker;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	public class BoxLayoutInfo {
		readonly ExpandedDocumentLayoutPosition start;
		readonly ExpandedDocumentLayoutPosition end;
		public BoxLayoutInfo(DocumentLayout documentLayout, PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end) {
			this.start = new ExpandedDocumentLayoutPosition(documentLayout, pieceTable, start);
			this.end = new ExpandedDocumentLayoutPosition(documentLayout, pieceTable, end);
		}
		public ExpandedDocumentLayoutPosition Start { get { return start; } }
		public ExpandedDocumentLayoutPosition End { get { return end; } }
		public DocumentLogPosition StartLogPosition { get { return Start.LogPosition; } }
		public DocumentLogPosition EndLogPosition { get { return End.LogPosition; } }
	}
	#region SpellCheckerErrorBoxCalculator
	public class SpellCheckerErrorBoxCalculator {
		#region Fields
		const int UnderlineThicknessDefault = 8;
		readonly DocumentLayout documentLayout;
		readonly BoxMeasurer boxMeasurer;
		readonly PieceTable pieceTable;
		Row lastProcessedRow;
		ExpandedDocumentLayoutPosition lastCalcLayoutPos;
		#endregion
		public SpellCheckerErrorBoxCalculator(DocumentLayout documentLayout, BoxMeasurer boxMeasurer, PieceTable pieceTable) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(boxMeasurer, "boxMeasurer");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.documentLayout = documentLayout;
			this.boxMeasurer = boxMeasurer;
			this.pieceTable = pieceTable;
		}
		#region Properties
		protected internal DocumentLayout DocumentLayout { get { return documentLayout; } }
		protected internal BoxMeasurer BoxMeasurer { get { return boxMeasurer; } }
		protected DocumentModel DocumentModel { get { return DocumentLayout.DocumentModel; } }
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		public virtual void Calculate(int pageIndex, DocumentModelPosition startPos, DocumentModelPosition endPos, SpellingError errorType) {
			Debug.Assert(endPos.LogPosition >= startPos.LogPosition);
			BoxLayoutInfo boxInfo = new BoxLayoutInfo(DocumentLayout, PieceTable, startPos.LogPosition, endPos.LogPosition);
			if (lastCalcLayoutPos != null && pageIndex == lastCalcLayoutPos.PageIndex) {
				if (!Update(boxInfo, lastCalcLayoutPos, DocumentLayoutDetailsLevel.Box)) {
					lastCalcLayoutPos = null;
					return;
				}
			}
			else {
				lastCalcLayoutPos = null;
				if (!Update(boxInfo, pageIndex, DocumentLayoutDetailsLevel.Box))
					return;
			}
			FormatterPosition start = PositionConverter.ToFormatterPosition(startPos);
			FormatterPosition end = PositionConverter.ToFormatterPosition(endPos);
			CalculateCore(boxInfo, start, end, errorType);
			lastCalcLayoutPos = boxInfo.End;
		}
		protected internal bool Update(BoxLayoutInfo boxInfo, int index, DocumentLayoutDetailsLevel detailsLevel) {
			PageCollection pages = DocumentLayout.Pages;
			return boxInfo.Start.Update(pages, index, detailsLevel) && boxInfo.End.Update(pages, boxInfo.Start, detailsLevel);
		}
		protected internal bool Update(BoxLayoutInfo boxInfo, ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			PageCollection pages = DocumentLayout.Pages;
			return boxInfo.Start.Update(pages, from, detailsLevel) && boxInfo.End.Update(pages, boxInfo.Start, detailsLevel);
		}
		protected virtual void CalculateCore(BoxLayoutInfo boxInfo, FormatterPosition start, FormatterPosition end, SpellingError errorType) {
			DocumentLayoutIterator iterator = new DocumentLayoutIterator(DocumentLayout, boxInfo);
			if (!Object.ReferenceEquals(iterator.Start.Row, iterator.End.Row))
				CalculateErrorBoxInSomeRows(iterator, start, end, errorType);
			else {
				ErrorBox box = new ErrorBox();
				box.ErrorType = errorType;
				box.StartPos = start;
				box.EndPos = end;
				Box first = iterator.Start.Box;
				Box last = iterator.End.Box;
				CalculateErrorBox(iterator.Start.Row, box, first, last);
			}
		}
		protected virtual void CalculateErrorBoxInSomeRows(DocumentLayoutIterator iterator, FormatterPosition start, FormatterPosition end, SpellingError errorType) {
			do
				CalculateErrorBoxInCurrentRow(iterator, start, end, errorType);
			while (iterator.MoveRowToNext());
		}
		public virtual void ClearErrorBoxes(DocumentLogPosition start, DocumentLogPosition end) {
			BoxLayoutInfo boxInfo = new BoxLayoutInfo(DocumentLayout, PieceTable, start, end);
			if (!boxInfo.Start.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row))
				return;
			if (!boxInfo.End.Update(DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row))
				return;
			DocumentLayoutIterator iterator = new DocumentLayoutIterator(DocumentLayout, boxInfo, true);
			do
				iterator.CurrentRow.ClearErrors();
			while (iterator.MoveRowToNext());
		}
		protected virtual void CalculateErrorBoxInCurrentRow(DocumentLayoutIterator iterator, FormatterPosition start, FormatterPosition end, SpellingError errorType) {
			Row row = iterator.CurrentRow;
			ErrorBox result = new ErrorBox();
			result.ErrorType = errorType;
			Box first = FirstBox(iterator.Start.Box, iterator.CurrentRow.Boxes.First);
			Box last = LastBox(iterator.End.Box, iterator.CurrentRow.Boxes.Last);
			result.StartPos = MaxPosition(first.StartPos, start);
			result.EndPos = MinPosition(last.EndPos, end);
			CalculateErrorBox(row, result, first, last);
		}
		protected internal virtual Box FirstBox(Box box1, Box box2) {
			if (box1.StartPos >= box2.StartPos)
				return box1;
			return box2;
		}
		protected internal virtual Box LastBox(Box box1, Box box2) {
			if (box1.EndPos <= box2.EndPos)
				return box1;
			return box2;
		}
		protected internal virtual FormatterPosition MaxPosition(FormatterPosition position1, FormatterPosition position2) {
			if (position1 >= position2)
				return position1;
			return position2;
		}
		protected internal virtual FormatterPosition MinPosition(FormatterPosition position1, FormatterPosition position2) {
			if (position1 <= position2)
				return position1;
			return position2;
		}
		protected internal virtual void CalculateErrorBox(Row row, ErrorBox errorBox, Box first, Box last) {
			if (!Object.ReferenceEquals(this.lastProcessedRow, row)) {
				row.ClearErrors();
				this.lastProcessedRow = row;
			}
			row.Errors.Add(errorBox);
			CalculateErrorBoxBounds(row, errorBox, first, last);
		}
		protected internal virtual void CalculateErrorBoxBounds(Row row, ErrorBox errorBox, Box first, Box last) {
			FontInfo fontInfo = GetFontInfo(row.Boxes[errorBox.StartAnchorIndex]);
			errorBox.UnderlinePosition = fontInfo.UnderlinePosition;
			errorBox.UnderlineThickness = fontInfo.UnderlineThickness;
			int underlineLeft = GetUnderlineLeft(first, errorBox);
			int underlineRight = GetUnderlineRight(last, errorBox);
			int baseLinePosition = row.Bounds.Top + row.BaseLineOffset + errorBox.UnderlineBounds.Y;
			int underlineTop = baseLinePosition + errorBox.UnderlinePosition / 2;
			int underlineBottom = baseLinePosition + errorBox.UnderlinePosition + errorBox.UnderlineThickness;
			int bottomDistance = row.Bounds.Bottom - underlineBottom;
			if (bottomDistance < 0) {
				underlineTop += bottomDistance;
				underlineBottom += bottomDistance;
			}
			errorBox.UnderlineBounds = Rectangle.FromLTRB(row.Bounds.Left, underlineTop, row.Bounds.Right, underlineBottom);
			errorBox.ClipBounds = Rectangle.FromLTRB(underlineLeft, underlineTop, underlineRight, underlineBottom);
		}
		protected internal virtual FontInfo GetFontInfo(Box box) {
			return box.GetFontInfo(PieceTable);
		}
		protected internal virtual int GetUnderlineLeft(Box first, ErrorBox errorBox) {
			if (errorBox.StartPos.RunIndex != first.StartPos.RunIndex)  
				return first.Bounds.Left;
			int startOffset = errorBox.StartPos.Offset - first.StartPos.Offset;
			return first.Bounds.Left + GetFirstBoxOffset(first, startOffset);
		}
		protected internal virtual int GetUnderlineRight(Box last, ErrorBox errorBox) {
			if (errorBox.EndPos.RunIndex != last.EndPos.RunIndex) 
				return last.Bounds.Left;
			int endOffset = last.EndPos.Offset - errorBox.EndPos.Offset;
			return last.Bounds.Right - GetLastBoxOffset(last, endOffset);
		}
		protected internal virtual int GetFirstBoxOffset(Box box, int offset) {
			if (offset == 0)
				return 0;
			string boxText = GetBoxText(box);
			if (offset >= boxText.Length)
				return box.Bounds.Width;
			Rectangle[] charBounds = GetCharactersBounds(box, boxText);
			Debug.Assert(charBounds.Length == boxText.Length);
			if (charBounds.Length < offset)
				return 0;
			int count = offset;
			int result = 0;
			for (int i = 0; i < count; i++)
				result += charBounds[i].Width;
			return result;
		}
		protected internal virtual int GetLastBoxOffset(Box box, int offset) {
			if (offset == 0)
				return 0;
			string boxText = GetBoxText(box);
			if (offset >= boxText.Length)
				return box.Bounds.Width;
			Rectangle[] charBounds = GetCharactersBounds(box, boxText);
			Debug.Assert(charBounds.Length == boxText.Length);
			if (charBounds.Length < boxText.Length)
				return 0;
			int charCount = boxText.Length;
			int endIndex = charCount - offset;
			int result = 0;
			for (int i = charCount - 1; i >= endIndex; i--)
				result += charBounds[i].Width;
			return result;
		}
		Rectangle[] GetCharactersBounds(Box box, string boxText) {
			return BoxMeasurer.MeasureCharactersBounds(boxText, GetFontInfo(box), box.Bounds);
		}
		string GetBoxText(Box box) {
			return PieceTable.GetTextFromSingleRun(box.StartPos, box.EndPos);
		}
	}
	#endregion
	#region DocumentLayoutIterator
	public class DocumentLayoutIterator {
		#region Fields
		readonly DocumentLayout documentLayout;
		readonly BoxLayoutInfo boxInfo;
		TableCellViewInfo tableCell;
		int currentBoxIndex = -1;
		int currentRowIndex = -1;
		int currentColumnIndex = -1;
		int currentPageAreaIndex = -1;
		int currentPageIndex = -1;
		HeaderPageArea header;
		FooterPageArea footer;
		bool stopInHeaderFooter;
		#endregion
		public DocumentLayoutIterator(DocumentLayout documentLayout, BoxLayoutInfo boxInfo, bool stopInHeaderFooter) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(boxInfo, "boxInfo");
			this.documentLayout = documentLayout;
			this.boxInfo = boxInfo;
			this.stopInHeaderFooter = stopInHeaderFooter;
			Initialize();
		}
		public DocumentLayoutIterator(DocumentLayout documentLayout, BoxLayoutInfo boxInfo)
			: this(documentLayout, boxInfo, false) {
		}
		#region Properties
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public BoxLayoutInfo BoxInfo { get { return boxInfo; } }
		internal DocumentLayoutPosition Start { get { return BoxInfo.Start; } }
		internal DocumentLayoutPosition End { get { return BoxInfo.End; } }
		public Page CurrentPage { get { return currentPageIndex >= 0 ? DocumentLayout.Pages[currentPageIndex] : End.Page; } }
		public PageArea CurrentPageArea {
			get {
				if (StopInHeaderFooter && (Header != null || Footer != null))
					return (PageArea)Header ?? Footer;
				return currentPageAreaIndex >= 0 ? CurrentPage.Areas[currentPageAreaIndex] : End.PageArea;
			}
		}
		public Column CurrentColumn { get { return currentColumnIndex >= 0 ? CurrentPageArea.Columns[currentColumnIndex] : End.Column; } }
		public HeaderPageArea Header { get { return header; } }
		public FooterPageArea Footer { get { return footer; } }
		internal TableCellViewInfo TableCell {
			get {
				return tableCell;
			}
		}
		internal RowCollection Rows {
			get {
				if (TableCell != null)
					return TableCell.GetRows(CurrentColumn);
				return CurrentColumn.Rows;
			}
		}
		public Row CurrentRow { get { return currentRowIndex >= 0 ? Rows[currentRowIndex] : End.Row; } }
		public Box CurrentBox { get { return currentBoxIndex >= 0 ? CurrentRow.Boxes[currentBoxIndex] : End.Box; } }
		public bool StopInHeaderFooter { get { return stopInHeaderFooter; } }
		#endregion
		protected internal virtual void Initialize() {
			this.tableCell = BoxInfo.Start.TableCell;
			this.currentPageIndex = BoxInfo.Start.PageIndex;
			this.currentPageAreaIndex = BoxInfo.Start.PageAreaIndex;
			this.currentColumnIndex = BoxInfo.Start.ColumnIndex;
			this.currentRowIndex = BoxInfo.Start.RowIndex;
			this.currentBoxIndex = BoxInfo.Start.BoxIndex;
			SetHeaderFooter();
		}
		protected internal virtual void SetHeaderFooter() {
			if (!StopInHeaderFooter)
				return;
			if (CurrentPage.Header != null)
				this.header = CurrentPage.Header;
			else
				this.footer = CurrentPage.Footer;
		}
		public virtual bool MoveNext(DocumentLayoutDetailsLevel detailsLevel) {
			if (detailsLevel == DocumentLayoutDetailsLevel.Box && CurrentBox != null)
				return MoveBoxToNext();
			if (detailsLevel == DocumentLayoutDetailsLevel.Row && CurrentRow != null)
				return MoveRowToNext();
			if (detailsLevel == DocumentLayoutDetailsLevel.Column && CurrentColumn != null)
				return MoveColumnToNext();
			if (detailsLevel == DocumentLayoutDetailsLevel.PageArea && CurrentPageArea != null)
				return MovePageAreaToNext();
			if (detailsLevel == DocumentLayoutDetailsLevel.Page && CurrentPage != null)
				return MovePageToNext();
			return false;
		}
		protected internal virtual bool MoveBoxToNext() {
			if (Object.ReferenceEquals(CurrentBox, End.Box))
				return false;
			this.currentBoxIndex++;
			if (this.currentBoxIndex < CurrentRow.Boxes.Count)
				return true;
			this.currentBoxIndex = 0;
			return MoveRowToNext();
		}
		protected internal virtual bool MoveRowToNext() {
			if (Object.ReferenceEquals(CurrentRow, End.Row))
				return false;
			this.currentRowIndex++;
			if (this.currentRowIndex < Rows.Count)
				return true;
			if (TableCell != null)
				return MoveTableCellRowToNext();
			else {
				this.currentRowIndex = 0;
				return MoveColumnToNext();
			}
		}
		bool MoveTableCellRowToNext() {
			Row lastCellRow = TableCell.GetLastRow(CurrentColumn);
			if (IsRowLastInCurrentColumn(lastCellRow)) {
				this.currentRowIndex = 0;
				this.tableCell = null;
				if (MoveColumnToNext()) {
					if (CurrentRow is TableCellRow)
						this.tableCell = ((TableCellRow)CurrentRow).CellViewInfo;
					return true;
				}
				else
					return false;
			}
			int lastCellRowIndex = CurrentColumn.Rows.IndexOf(lastCellRow);
			if (lastCellRowIndex < 0)
				Exceptions.ThrowInternalException();
			int nextRowIndex = lastCellRowIndex + 1;
			if (CurrentColumn.Rows[nextRowIndex] is TableCellRow) {
				this.currentRowIndex = 0;
				this.tableCell = ((TableCellRow)CurrentColumn.Rows[nextRowIndex]).CellViewInfo;
			}
			else {
				this.currentRowIndex = nextRowIndex;
				this.tableCell = null;
			}
			return true;
		}
		bool IsRowLastInCurrentColumn(Row row) {
			return Object.ReferenceEquals(CurrentColumn.Rows.Last, row);
		}
		protected internal virtual bool MoveColumnToNext() {
			if (Object.ReferenceEquals(CurrentColumn, End.Column))
				return false;
			this.currentColumnIndex++;
			if (this.currentColumnIndex < CurrentPageArea.Columns.Count)
				return true;
			this.currentColumnIndex = 0;
			return MovePageAreaToNext();
		}
		protected internal virtual bool MovePageAreaToNext() {
			if (Object.ReferenceEquals(CurrentPageArea, End.PageArea))
				return false;
			if (Header != null) {
				this.header = null;
				this.footer = CurrentPage.Footer;
			}
			else if (Footer != null)
				this.footer = null;
			else {
				this.currentPageAreaIndex++;
				if (this.currentPageAreaIndex < CurrentPage.Areas.Count)
					return true;
				this.currentPageAreaIndex = 0;
				return MovePageToNext();
			}
			return true;
		}
		protected internal virtual bool MovePageToNext() {
			if (Object.ReferenceEquals(CurrentPage, End.Page))
				return false;
			this.currentPageIndex++;
			if (!CurrentPage.PrimaryFormattingComplete)
				return false;
			SetHeaderFooter();
			return true;
		}
	}
	#endregion
	#region ExpandedDocumentLayoutPosition
	public class ExpandedDocumentLayoutPosition : DocumentLayoutPosition {
		int pageIndex = -1;
		int pageAreaIndex = -1;
		int columnIndex = -1;
		int rowIndex = -1;
		int boxIndex = -1;
		int characterIndex = -1;
		public ExpandedDocumentLayoutPosition(DocumentLayout documentLayout, PieceTable pieceTable, DocumentLogPosition logPosition)
			: base(documentLayout, pieceTable, logPosition) {
		}
		public int PageIndex { get { return pageIndex; } set { pageIndex = value; } }
		public int PageAreaIndex { get { return pageAreaIndex; } set { pageAreaIndex = value; } }
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		public int BoxIndex { get { return boxIndex; } set { boxIndex = value; } }
		public int CharacterIndex { get { return characterIndex; } set { characterIndex = value; } }
		protected internal override DocumentLayoutPosition CreateEmptyClone() {
			return new ExpandedDocumentLayoutPosition(DocumentLayout, PieceTable, LogPosition);
		}
		protected internal override int GetPageIndex(PageCollection pages, int startIndex, int endIndex) {
			this.pageIndex = base.GetPageIndex(pages, startIndex, endIndex);
			return this.pageIndex;
		}
		protected internal override int GetPageAreaIndex(PageAreaCollection areas, int startIndex, int endIndex) {
			this.pageAreaIndex = base.GetPageAreaIndex(areas, startIndex, endIndex);
			return this.pageAreaIndex;
		}
		protected internal override int GetColumnIndex(ColumnCollection columns, int startIndex, int endIndex) {
			this.columnIndex = base.GetColumnIndex(columns, startIndex, endIndex);
			return this.columnIndex;
		}
		protected internal override int GetRowIndex(RowCollection rows, int startIndex, int endIndex) {
			this.rowIndex = base.GetRowIndex(rows, startIndex, endIndex);
			return this.rowIndex;
		}
		protected internal override int GetBoxIndex(BoxCollection boxes, int startIndex, int endIndex) {
			this.boxIndex = base.GetBoxIndex(boxes, startIndex, endIndex);
			return this.boxIndex;
		}
		protected internal override int GetCharIndex(CharacterBoxCollection characters, int startIndex, int endIndex) {
			this.characterIndex = base.GetCharIndex(characters, startIndex, endIndex);
			return this.characterIndex;
		}
		protected internal virtual bool Update(PageCollection pages, int index, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(detailsLevel)) {
				IncreaseDetailsLevel(UpdateCore(pages, index, detailsLevel));
				return IsValid(detailsLevel);
			}
			else
				return true;
		}
		protected internal DocumentLayoutDetailsLevel UpdateCore(PageCollection pages, int index, DocumentLayoutDetailsLevel detailsLevel) {
			if (detailsLevel < DocumentLayoutDetailsLevel.Page)
				return DocumentLayoutDetailsLevel.None;
			Page page = pages[index];
			DocumentLayoutDetailsLevel resulLevel = UpdatePageAreaRecursive(page, 0, detailsLevel);
			if (resulLevel >= detailsLevel) {
				this.Page = page;
				this.pageIndex = index;
				return resulLevel;
			}
			return UpdatePageRecursive(pages, index, detailsLevel);
		}
		protected internal bool Update(PageCollection pages, ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(detailsLevel)) {
				IncreaseDetailsLevel(UpdateCore(pages, from, detailsLevel));
				return IsValid(detailsLevel);
			}
			else
				return true;
		}
		protected internal DocumentLayoutDetailsLevel UpdateCore(PageCollection pages, ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (from == null || !from.IsValid(detailsLevel))
				return DocumentLayoutDetailsLevel.None;
			DocumentLayoutDetailsLevel resulLevel = UpdateBoxRecursive(from, detailsLevel);
			if (resulLevel >= detailsLevel)
				return resulLevel;
			return UpdatePageRecursive(pages, from.PageIndex, detailsLevel);
		}
		DocumentLayoutDetailsLevel UpdateBoxRecursive(ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Box) && detailsLevel >= DocumentLayoutDetailsLevel.Box) {
				DocumentLayoutDetailsLevel resultLevel = UpdateBoxRecursive(from.Row, from.BoxIndex, detailsLevel);
				if (resultLevel == detailsLevel) {
					CopyFrom(from, DocumentLayoutDetailsLevel.Row);
					return resultLevel;
				}
			}
			return UpdateRowRecursive(from, detailsLevel);
		}
		DocumentLayoutDetailsLevel UpdateRowRecursive(ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Column) && detailsLevel >= DocumentLayoutDetailsLevel.Row) {
				RowCollection rows = from.TableCell != null ? from.TableCell.GetRows(from.Column) : from.Column.Rows;
				DocumentLayoutDetailsLevel resultLevel = UpdateRowRecursive(rows, from.RowIndex, detailsLevel);
				if (resultLevel == detailsLevel) {
					if (from.TableCell == null) {
						if (Row is TableCellRow) {
							TableCellRow tableRow = Row as TableCellRow;
							TableCell = tableRow.CellViewInfo;
							int rowIndex = TableCell.GetRows(from.Column).IndexOf(tableRow);
							if (rowIndex < 0)
								return DocumentLayoutDetailsLevel.None;
							this.rowIndex = rowIndex;
						}
						CopyFrom(from, DocumentLayoutDetailsLevel.Column);
					}
					else
						CopyFrom(from, DocumentLayoutDetailsLevel.TableCell);
					return resultLevel;
				}
			}
			return UpdateColumnRecursive(from, detailsLevel);
		}
		DocumentLayoutDetailsLevel UpdateColumnRecursive(ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.PageArea) && detailsLevel >= DocumentLayoutDetailsLevel.Column) {
				DocumentLayoutDetailsLevel resultLevel = UpdateColumnRecursive(from.PageArea.Columns, from.ColumnIndex, detailsLevel);
				if (resultLevel == detailsLevel) {
					CopyFrom(from, DocumentLayoutDetailsLevel.PageArea);
					return resultLevel;
				}
			}
			return UpdatePageAreaRecursive(from, detailsLevel);
		}
		DocumentLayoutDetailsLevel UpdatePageAreaRecursive(ExpandedDocumentLayoutPosition from, DocumentLayoutDetailsLevel detailsLevel) {
			if (!IsValid(DocumentLayoutDetailsLevel.Page) && detailsLevel >= DocumentLayoutDetailsLevel.PageArea) {
				DocumentLayoutDetailsLevel resultLevel = UpdatePageAreaRecursive(from.Page, from.PageAreaIndex, detailsLevel);
				if (resultLevel == detailsLevel) {
					CopyFrom(from, DocumentLayoutDetailsLevel.Page);
					return resultLevel;
				}
			}
			return DocumentLayoutDetailsLevel.None;
		}
		protected internal override void Invalidate() {
			base.Invalidate();
			this.pageIndex = -1;
			this.pageAreaIndex = -1;
			this.columnIndex = -1;
			this.rowIndex = -1;
			this.boxIndex = -1;
			this.characterIndex = -1;
		}
		protected override void CopyFrom(DocumentLayoutPosition value, DocumentLayoutDetailsLevel detailsLevel) {
			base.CopyFrom(value, detailsLevel);
			ExpandedDocumentLayoutPosition expandedPosition = value as ExpandedDocumentLayoutPosition;
			if (expandedPosition == null)
				return;
			if (IsValid(DocumentLayoutDetailsLevel.Page))
				this.pageIndex = expandedPosition.pageIndex;
			if (IsValid(DocumentLayoutDetailsLevel.PageArea))
				this.pageAreaIndex = expandedPosition.pageAreaIndex;
			if (IsValid(DocumentLayoutDetailsLevel.Column))
				this.columnIndex = expandedPosition.columnIndex;
			if (IsValid(DocumentLayoutDetailsLevel.Row))
				this.rowIndex = expandedPosition.rowIndex;
			if (IsValid(DocumentLayoutDetailsLevel.Box))
				this.boxIndex = expandedPosition.boxIndex;
			if (IsValid(DocumentLayoutDetailsLevel.Character))
				this.characterIndex = expandedPosition.characterIndex;
		}
	}
	#endregion
}
