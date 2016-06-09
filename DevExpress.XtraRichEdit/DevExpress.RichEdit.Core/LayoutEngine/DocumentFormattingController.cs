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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	public class ResetSecondaryFormattingForPageArgs : EventArgs {
		readonly Page page;
		readonly int pageIndex;
		public ResetSecondaryFormattingForPageArgs(Page page, int pageIndex) {
			this.page = page;
			this.pageIndex = pageIndex;
		}
		public Page Page { get { return page; } }
		public int PageIndex { get { return pageIndex; } }
	}
	public delegate void ResetSecondaryFormattingForPageHandler(object sender, ResetSecondaryFormattingForPageArgs e);
	#region DocumentFormattingController (abstract class)
	public abstract class DocumentFormattingController {
		#region Fields
		readonly DocumentLayout documentLayout;
		readonly FloatingObjectsLayout floatingObjectsLayout;
		readonly ParagraphFramesLayout paragraphFramesLayout;
		readonly PageController pageController;
		readonly PageAreaController pageAreaController;
		readonly ColumnController columnController;
		readonly RowsController rowsController;
		readonly PieceTable pieceTable;
		int pageCountSinceLastResetSecondaryFormatting;
		#endregion
		protected DocumentFormattingController(DocumentLayout documentLayout, PieceTable pieceTable, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.documentLayout = documentLayout;
			this.pieceTable = pieceTable;
			this.floatingObjectsLayout = floatingObjectsLayout;
			this.paragraphFramesLayout = paragraphFramesLayout;
			this.pageController = CreatePageController();
			PageController.FloatingObjectsLayoutChanged += OnFloatingObjectsLayoutChanged;
			this.pageAreaController = CreatePageAreaController();
			this.columnController = CreateColumnController();
			this.rowsController = CreateRowController();
			Reset(false);
			PageController.PageFormattingStarted += OnPageFormattingStarted;
			PageController.PageFormattingComplete += OnPageFormattingComplete;
			PageController.PageCountChanged += OnPageCountChanged;
			rowsController.BeginNextSectionFormatting += OnBeginNextSectionFormatting;
		}
		#region Properties
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public DocumentModel DocumentModel { get { return DocumentLayout.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public PageController PageController { get { return pageController; } }
		public PageAreaController PageAreaController { get { return pageAreaController; } }
		public ColumnController ColumnController { get { return columnController; } }
		public RowsController RowsController { get { return rowsController; } }
		public int PageCount { get { return PageController.PageCount; } }
		protected FloatingObjectsLayout FloatingObjectsLayout { get { return floatingObjectsLayout; } }
		protected ParagraphFramesLayout ParagraphFramesLayout { get { return paragraphFramesLayout; } }
		#endregion
		#region Events
		#region PageFormattingStarted
		PageFormattingCompleteEventHandler onPageFormattingStarted;
		public event PageFormattingCompleteEventHandler PageFormattingStarted { add { onPageFormattingStarted += value; } remove { onPageFormattingStarted -= value; } }
		protected internal virtual void RaisePageFormattingStarted(Page page) {
			if (onPageFormattingStarted != null) {
				PageFormattingCompleteEventArgs args = new PageFormattingCompleteEventArgs(page, false);
				onPageFormattingStarted(this, args);
			}
		}
		#endregion
		#region PageFormattingComplete
		PageFormattingCompleteEventHandler onPageFormattingComplete;
		public event PageFormattingCompleteEventHandler PageFormattingComplete { add { onPageFormattingComplete += value; } remove { onPageFormattingComplete -= value; } }
		protected internal virtual void RaisePageFormattingComplete(Page page, bool documentFormattingComplete) {
			if (onPageFormattingComplete != null) {
				PageFormattingCompleteEventArgs args = new PageFormattingCompleteEventArgs(page, documentFormattingComplete);
				onPageFormattingComplete(this, args);
			}
		}
		#endregion
		#region PageCountChanged
		EventHandler onPageCountChanged;
		public event EventHandler PageCountChanged { add { onPageCountChanged += value; } remove { onPageCountChanged -= value; } }
		protected internal virtual void RaisePageCountChanged() {
			if (onPageCountChanged != null)
				onPageCountChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ResetSecondaryFormattingForPage
		ResetSecondaryFormattingForPageHandler onResetSecondaryFormattingForPage;
		public event ResetSecondaryFormattingForPageHandler ResetSecondaryFormattingForPage { add { onResetSecondaryFormattingForPage += value; } remove { onResetSecondaryFormattingForPage -= value; } }
		protected internal virtual void RaiseResetSecondaryFormattingForPage(Page page, int pageIndex) {
			if (onResetSecondaryFormattingForPage != null)
				onResetSecondaryFormattingForPage(this, new ResetSecondaryFormattingForPageArgs(page, pageIndex));
		}
		#endregion
		#endregion
		protected internal abstract PageController CreatePageController();
		protected internal virtual PageAreaController CreatePageAreaController() {
			return new PageAreaController(PageController);
		}
		protected internal abstract ColumnController CreateColumnController();
		protected internal abstract RowsController CreateRowController();
		public virtual void Reset(bool keepFloatingObjects) {
			DocumentLayout.Counters.Reset();
			PageController.Reset(keepFloatingObjects);
			PageAreaController.Reset(PageController.CurrentSection);
			ColumnController.Reset(PageController.CurrentSection);
			RowsController.Reset(PageController.CurrentSection, keepFloatingObjects);
		}
		public virtual DocumentModelPosition ResetFrom(DocumentModelPosition from, bool keepFloatingObjects) {
			from = EnsurePositionVisible(from);
#if DEBUGTEST || DEBUG
			TableCell cell = PieceTable.Paragraphs[from.ParagraphIndex].GetCell();
			Debug.Assert(cell == null || cell == cell.Table.Rows.First.Cells.First);
#endif
			if (from.ParagraphIndex == ParagraphIndex.Zero) {
				Reset(keepFloatingObjects);
				return from;
			}
			ClearInvalidatedContentResult clearContentResult = ClearInvalidatedContent(from, true, keepFloatingObjects);
			if (clearContentResult == ClearInvalidatedContentResult.NoRestart) {
				DocumentLayout.Counters.ResetFrom(from.LogPosition);
				return from;
			}
			int pageCountBefore = DocumentLayout.Pages.Count;
			DocumentModelPosition result = CleanupEmptyBoxes(from);
			if (DocumentLayout.Pages.Count < pageCountBefore)
				PageController.ClearFloatingObjectsLayout();
			Page lastPage = DocumentLayout.Pages.Last;
			if (lastPage != null) {
				lastPage.PrimaryFormattingComplete = false;
				lastPage.SecondaryFormattingComplete = false;
				lastPage.CheckSpellingComplete = false;
				result = Algorithms.Min(result, lastPage.GetFirstPosition(PieceTable));
				DocumentLayout.Counters.ResetFrom(result.LogPosition);
			}
			else {
				Reset(keepFloatingObjects);
				from = DocumentModelPosition.FromParagraphStart(PieceTable, ParagraphIndex.Zero);
				return from;
			}
			if (clearContentResult == ClearInvalidatedContentResult.RestartFromTheStartOfSection)
				RestartFormattingFromTheStartOfSection();
			else
				RestartFormattingFromTheMiddleOfSection();
			return result;
		}
		public virtual void ResetFromTheStartOfRowAtCurrentPage(DocumentModelPosition from, bool keepFloatingObjects, bool forceRestart) {
			ClearInvalidatedContentResult clearContentResult = ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(from, false, keepFloatingObjects);
			if (clearContentResult == ClearInvalidatedContentResult.NoRestart && !forceRestart)
				return;
			DocumentModelPosition result = CleanupEmptyBoxesForCurrentPage(from);
			Page lastPage = DocumentLayout.Pages.Last;
			Debug.Assert(lastPage != null);
			lastPage.PrimaryFormattingComplete = false;
			lastPage.SecondaryFormattingComplete = false;
			lastPage.CheckSpellingComplete = false;
			result = Algorithms.Min(result, lastPage.GetFirstPosition(PieceTable));
			DocumentLayout.Counters.ResetFrom(result.LogPosition);
			RestartFormattingFromTheStartOfRowAtCurrentPage(clearContentResult, result);
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage(ClearInvalidatedContentResult clearContentResult, DocumentModelPosition from) {
			Page lastPage = DocumentLayout.Pages.Last;
			PageArea lastPageArea = lastPage.Areas.Last;
			Column lastColumn = lastPageArea != null ? lastPageArea.Columns.Last : null;
			if (clearContentResult == ClearInvalidatedContentResult.RestartFromTheStartOfSection)
				PageController.RestartFormattingFromTheStartOfSection();
			else
				PageController.RestartFormattingFromTheStartOfRowAtCurrentPage();
			Section section = DocumentLayout.DocumentModel.Sections[PageController.CurrentSectionIndex];
			PageAreaController.RestartFormattingFromTheStartOfRowAtCurrentPage();
			ColumnController.RestartFormattingFromTheStartOfRowAtCurrentPage();
				RowsController.RestartFormattingFromTheStartOfRowAtCurrentPage(section, lastColumn);
		}
		protected virtual DocumentModelPosition EnsurePositionVisible(DocumentModelPosition pos) {
			ParagraphIndex index = pos.ParagraphIndex;
			pos = EnsureRunVisible(pos);
			pos = EnsureParagraphVisible(pos);
			if (pos.ParagraphIndex != index)
				return DevExpress.XtraRichEdit.Internal.DocumentChangesHandler.EnsureTopLevelParagraph(pos);
			else
				return pos;
		}
		protected virtual DocumentModelPosition EnsureParagraphVisible(DocumentModelPosition pos) {
			ParagraphCollection paragraphs = pos.PieceTable.Paragraphs;
			ParagraphIndex paragraphIndex = pos.ParagraphIndex;
			Paragraph paragraph;
			if (paragraphIndex > ParagraphIndex.Zero) {
				while (paragraphIndex > ParagraphIndex.Zero) {
					paragraph = paragraphs[paragraphIndex];
					if (pos.PieceTable.Runs[paragraph.LastRunIndex] is SectionRun) {
						paragraphIndex--;
						continue;
					}
					if(paragraph.IsEmpty && paragraphIndex + 1 < new ParagraphIndex(paragraphs.Count)) {
						Paragraph nextParagraph = paragraphs[paragraphIndex + 1];
						if (nextParagraph.IsEmpty && pos.PieceTable.Runs[nextParagraph.FirstRunIndex] is SectionRun) {
							paragraphIndex--;
							continue;
						}
					}
					break;
				}
			}
			TableCell initialCell = paragraphs[paragraphIndex].GetCell();
			ParagraphIndex cellStartParagraphIndex = initialCell != null ? initialCell.StartParagraphIndex : ParagraphIndex.Zero;
			while (paragraphIndex > cellStartParagraphIndex && !PieceTable.VisibleTextFilter.IsRunVisible(paragraphs[paragraphIndex - 1].LastRunIndex)) {
				if (initialCell != null || !paragraphs[paragraphIndex - 1].IsInCell())
					paragraphIndex--;
				else
					break;
			}
			paragraph = paragraphs[paragraphIndex];
			pos.RunIndex = paragraph.FirstRunIndex;
			pos.ParagraphIndex = paragraphIndex;			
			pos.LogPosition = paragraph.LogPosition;
			pos.RunStartLogPosition = paragraph.LogPosition;
			return pos;
		}
		protected virtual DocumentModelPosition EnsureRunVisible(DocumentModelPosition pos) {
			if (PieceTable.VisibleTextFilter.IsRunVisible(pos.RunIndex)) {
				return pos;
			}
			RunIndex runIndex = pos.RunIndex;
			ParagraphIndex paragraphIndex = pos.ParagraphIndex;
			ParagraphCollection paragraphs = pos.PieceTable.Paragraphs;
			Paragraph paragraph = paragraphs[paragraphIndex];
			TableCell initialCell = paragraph.GetCell();		   
			ParagraphIndex cellStartParagraphIndex = initialCell != null ? initialCell.StartParagraphIndex : ParagraphIndex.Zero;
			while (!PieceTable.VisibleTextFilter.IsRunVisible(runIndex) || PositionSectionBreakAfterParagraphBreak(paragraph, runIndex)) {
				runIndex--;
				if (runIndex < RunIndex.Zero)
					break;
				if (runIndex >= paragraph.FirstRunIndex)
					continue;
				paragraphIndex--;
				paragraph = paragraphs[paragraphIndex];
				if (initialCell != null) {
					if (paragraphIndex < cellStartParagraphIndex) {
						paragraphIndex++;
						runIndex++;
						break;
					}					
				}
				else {
					bool isInCell = paragraph.IsInCell();
					if (isInCell) {
						paragraphIndex++;
						runIndex++;
						break;
					}					
				}
			}
			pos.RunIndex = runIndex;
			pos.ParagraphIndex = paragraphIndex;
			return pos;
		}
		bool PositionSectionBreakAfterParagraphBreak(Paragraph paragraph, RunIndex runIndex) {
			return runIndex > RunIndex.Zero && paragraph.Length == 1 && paragraph.PieceTable.Runs[runIndex] is SectionRun;
		}
		FormatterPosition CreateFormatterPosition(DocumentModelPosition from, bool roundToParagraphBoundary) {
			if (roundToParagraphBoundary) {
				Paragraph paragraph = PieceTable.Paragraphs[from.ParagraphIndex];
				return new FormatterPosition(paragraph.FirstRunIndex, 0, 0);
			}
			else
				return new FormatterPosition(from.RunIndex, from.RunOffset, 0);
		}
		protected internal virtual ClearInvalidatedContentResult ClearInvalidatedContent(DocumentModelPosition from, bool roundToParagraphBoundary, bool keepFloatingObjects) {
			FormatterPosition pos = CreateFormatterPosition(from, roundToParagraphBoundary);
			bool emptyCurrentRow = RowsController.CurrentRow.Boxes.Count == 0;
			ClearInvalidatedContentResult clearContentResult = PageController.ClearInvalidatedContent(pos, from.ParagraphIndex, keepFloatingObjects, emptyCurrentRow);
			if (clearContentResult == ClearInvalidatedContentResult.NoRestart)
				return clearContentResult;
			PageAreaController.ClearInvalidatedContent(pos);
			ColumnController.ClearInvalidatedContent(pos);
			RowsController.ClearInvalidatedContent(DocumentLayout.Pages.Last.Areas.Last.Columns.Last, pos, from.ParagraphIndex);
			return clearContentResult;
		}
		protected internal virtual ClearInvalidatedContentResult ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(DocumentModelPosition from, bool roundToParagraphBoundary, bool keepFloatingObjects) {
			FormatterPosition pos = CreateFormatterPosition(from, roundToParagraphBoundary);
			bool leaveTable;
			bool emptyCurrentRow = RowsController.CurrentRow.Boxes.Count == 0;
			ClearInvalidatedContentResult clearContentResult = PageController.ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(pos, from.ParagraphIndex, keepFloatingObjects, RowsController.TablesController, emptyCurrentRow, out leaveTable);
			if (clearContentResult == ClearInvalidatedContentResult.NoRestart)
				return clearContentResult;
			if (clearContentResult == ClearInvalidatedContentResult.ClearOnlyTableCellRows) {
				RowsController.ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(DocumentLayout.Pages.Last.Areas.Last.Columns.Last, pos, from.ParagraphIndex);
				return ClearInvalidatedContentResult.Restart;
			}
			else {
				if (leaveTable)
					RowsController.CurrentColumn = RowsController.CurrentColumn.TopLevelColumn;
				PageAreaController.ClearInvalidatedContent(pos);
				ColumnController.ClearInvalidatedContent(pos);
				RowsController.ClearInvalidatedContent(DocumentLayout.Pages.Last.Areas.Last.Columns.Last, pos, from.ParagraphIndex);
				return clearContentResult;
			}
		}
		protected internal virtual DocumentModelPosition CleanupEmptyBoxesForCurrentPage(DocumentModelPosition from) {
			Page lastPage = DocumentLayout.Pages.Last;
			lastPage.ClearFloatingObjects();
			PageArea lastPageArea = lastPage.Areas.Last;
			Column lastColumn = lastPageArea.Columns.Last;
			if (lastColumn.Rows.Count <= 0)
				return from;
			if (lastPageArea.Columns.Count > 0)
				return from;
			if (lastPageArea.Columns.Count <= 0)
					lastPage.Areas.RemoveAt(lastPage.Areas.Count - 1);
			return from;
		}
		protected internal virtual DocumentModelPosition CleanupEmptyBoxes(DocumentModelPosition from) {
			Page lastPage = DocumentLayout.Pages.Last;
			lastPage.ClearInvalidatedContent(from.RunIndex, from.PieceTable);
			PageArea lastPageArea = lastPage.Areas.Last;
			Column lastColumn = lastPageArea.Columns.Last;
			if (lastColumn.Rows.Count <= 0) {
				lastPageArea.Columns.RemoveAt(lastPageArea.Columns.Count - 1);
				ColumnController.CleanupEmptyBoxes(lastColumn);
			}
			if (lastPageArea.Columns.Count > 0)
				return from;
			if (lastPageArea.Columns.Count <= 0)
				lastPage.Areas.RemoveAt(lastPage.Areas.Count - 1);
			if (lastPage.Areas.Count > 0)
				return from;
			DocumentLayout.Pages.RemoveAt(DocumentLayout.Pages.Count - 1);
			Page page = DocumentLayout.Pages.Last;
			if (page == null)
				return new DocumentModelPosition(PieceTable);
			else
				return page.GetLastPosition(PieceTable);
		}
		protected internal virtual void ClearPages() {
			DocumentLayout.Pages.Clear();
		}
		protected internal virtual void ClearPagesFrom(int pageIndex) {
			PageCollection pages = DocumentLayout.Pages;
			pages.RemoveRange(pageIndex, pages.Count - pageIndex);
		}
		protected internal virtual void OnBeginNextSectionFormatting(object sender, EventArgs e) {
			BeginNextSectionFormatting();
		}
		protected internal virtual void BeginNextSectionFormatting() {
			PageController.BeginNextSectionFormatting();
			PageAreaController.BeginSectionFormatting(PageController.CurrentSection, ColumnController.TopLevelColumnsCount);
			ColumnController.BeginSectionFormatting(PageController.CurrentSection);
			RowsController.BeginSectionFormatting(PageController.CurrentSection);
		}
		protected internal virtual void RestartFormattingFromTheStartOfSection() {
			Page lastPage = DocumentLayout.Pages.Last;
			PageArea lastPageArea = lastPage.Areas.Last;
			Column lastColumn = lastPageArea.Columns.Last;
			PageController.RestartFormattingFromTheStartOfSection();
			PageAreaController.RestartFormattingFromTheStartOfSection(PageController.CurrentSection, lastPage.Areas.Count - 1);
			int index = lastPageArea.Columns.IndexOf(lastColumn);
			ColumnController.RestartFormattingFromTheStartOfSection(PageController.CurrentSection, index);
			if (index < columnController.ColumnsBounds.Count)
				lastColumn.Bounds = columnController.ColumnsBounds[index];
			RowsController.RestartFormattingFromTheStartOfSection(PageController.CurrentSection, lastColumn);
		}
		protected internal virtual void RestartFormattingFromTheMiddleOfSection() {
			Page lastPage = DocumentLayout.Pages.Last;
			PageArea lastPageArea = lastPage.Areas.Last;
			Column lastColumn = lastPageArea.Columns.Last;
			Section section = DocumentLayout.DocumentModel.Sections[PageController.CurrentSectionIndex];
			PageController.RestartFormattingFromTheMiddleOfSection(section);
			PageAreaController.RestartFormattingFromTheMiddleOfSection(PageController.CurrentSection, lastPage.Areas.Count - 1);
			int index = lastPageArea.Columns.IndexOf(lastColumn);
			ColumnController.RestartFormattingFromTheMiddleOfSection(PageController.CurrentSection, index);
			if(index < columnController.ColumnsBounds.Count)
				lastColumn.Bounds = columnController.ColumnsBounds[index];
			RowsController.RestartFormattingFromTheMiddleOfSection(PageController.CurrentSection, lastColumn);
		}
		protected internal virtual void OnPageFormattingStarted(object sender, PageFormattingCompleteEventArgs e) {
			RaisePageFormattingStarted(e.Page);
		}
		protected internal virtual void OnPageFormattingComplete(object sender, PageFormattingCompleteEventArgs e) {
			PageController.FinalizePagePrimaryFormatting(e.Page, false);
			RowsController.OnPageFormattingComplete(pageController.CurrentSection, e.Page);
			e.Page.PrimaryFormattingComplete = true;
			int oldPageCount = DocumentModel.ExtendedDocumentProperties.Pages;			
			int newPageCount = Math.Max(oldPageCount, PageController.PageCount + e.Page.NumSkippedPages);
			if (newPageCount != oldPageCount)
				SetPageCount(newPageCount, false);
			RaisePageFormattingComplete(e.Page, false);
		}
		void SetPageCount(int pageCount, bool primiryFormattingFinished) {
			PieceTable.ContentType.SetPageCount(pageCount);
			if (!primiryFormattingFinished && pageCountSinceLastResetSecondaryFormatting < 100) {
				pageCountSinceLastResetSecondaryFormatting++;
				return;
			}
			pageCountSinceLastResetSecondaryFormatting = 0;
			PageCollection pages = PageController.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				Page page = pages[i];
				if (page.NumPages != pageCount) {
					page.NumPages = pageCount;
					if (page.SecondaryFormattingComplete) {
						page.SecondaryFormattingComplete = false;
						page.CheckSpellingComplete = false;
						RaiseResetSecondaryFormattingForPage(page, i);
					}
				}
			}
		}
		protected internal virtual void OnPageCountChanged(object sender, EventArgs e) {
			RaisePageCountChanged();
		}
		protected internal virtual void NotifyDocumentFormattingComplete() {
			Page page = PageController.Pages.Last;
			PageController.FinalizePagePrimaryFormatting(page, true);
			RowsController.OnPageFormattingComplete(pageController.CurrentSection, page);
			page.PrimaryFormattingComplete = true;
			int newPageCount = PageController.PageCount + page.NumSkippedPages;
			if (newPageCount != DocumentModel.ExtendedDocumentProperties.Pages)
				SetPageCount(newPageCount, true);
			RaisePageFormattingComplete(page, true);
		}
		protected internal virtual void OnParagraphFormattingComplete(ParagraphIndex paragraphIndex) {
		}
		public void OnFloatingObjectsLayoutChanged(object sender, EventArgs e) {
			RowsController.OnFloatingObjectsLayoutChanged();
		}
	}
	#endregion
}
