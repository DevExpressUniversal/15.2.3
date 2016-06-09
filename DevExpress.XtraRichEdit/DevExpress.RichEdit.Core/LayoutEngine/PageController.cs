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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region ClearInvalidatedContentResult
	public enum ClearInvalidatedContentResult {
		Restart,
		RestartFromTheStartOfSection,
		NoRestart,
		ClearOnlyTableCellRows,
	}
	#endregion
	#region PageController (abstract class)
	public abstract class PageController {
		#region Fields
		readonly DocumentLayout documentLayout;
		SectionIndex currentSectionIndex;
		Section currentSection;
		Rectangle pageBounds;
		Rectangle pageClientBounds;
		Rectangle currentPageClientBounds;
		SectionStartType nextPageOrdinalType;
		PageBoundsCalculator pageBoundsCalculator;
		FloatingObjectsLayout floatingObjectsLayout;
		ParagraphFramesLayout paragraphFramesLayout;
		bool firstPageOfSection;
		bool tableStarted;
		int pagesBeforeTable;
		bool nextSection;
		RunIndex pageLastRunIndex;
		#endregion
		protected PageController(DocumentLayout documentLayout, FloatingObjectsLayout floatingObjectsLayout, ParagraphFramesLayout paragraphFramesLayout) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			this.documentLayout = documentLayout;
			this.nextPageOrdinalType = SectionStartType.Continuous;
			this.pageBoundsCalculator = CreatePageBoundsCalculator();
			if (floatingObjectsLayout == null)
				this.floatingObjectsLayout = new FloatingObjectsLayout();
			else
				this.floatingObjectsLayout = floatingObjectsLayout;
			if (paragraphFramesLayout == null)
				this.paragraphFramesLayout = new ParagraphFramesLayout();
			else
				this.paragraphFramesLayout = paragraphFramesLayout;
		}
		#region Properties
		public Rectangle PageBounds { get { return pageBounds; } }
		public Rectangle PageClientBounds { get { return currentPageClientBounds; } }
		public virtual SectionIndex CurrentSectionIndex { get { return currentSectionIndex; } set { currentSectionIndex = value; } }
		public virtual Section CurrentSection { get { return currentSection; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public virtual PageCollection Pages { get { return DocumentLayout.Pages; } }
		public int PageCount { get { return Pages.Count; } }
		public SectionStartType NextPageOrdinalType { get { return nextPageOrdinalType; } }
		public virtual PageBoundsCalculator PageBoundsCalculator { get { return pageBoundsCalculator; } }
		public virtual PieceTable PieceTable { get { return DocumentModel.MainPieceTable; } }
		public DocumentModel DocumentModel { get { return documentLayout.DocumentModel; } }
		protected virtual bool FirstPageOfSection { get { return firstPageOfSection; } }
		public FloatingObjectsLayout FloatingObjectsLayout { get { return floatingObjectsLayout; } }
		public ParagraphFramesLayout ParagraphFramesLayout { get { return paragraphFramesLayout; } }
		public bool NextSection { get { return nextSection; } set { nextSection = value; } }
		public RunIndex PageLastRunIndex { get { return pageLastRunIndex; } }
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
		protected internal virtual void RaisePageFormattingComplete(Page page) {
			if (onPageFormattingComplete != null) {
				PageFormattingCompleteEventArgs args = new PageFormattingCompleteEventArgs(page, false);
				onPageFormattingComplete(this, args);
			}
		}
		#endregion
		protected internal virtual void PageContentFormattingComplete(Page page) {
			RaisePageFormattingComplete(page);
		}
		#region FloatingObjectsLayoutChanged
		public event EventHandler FloatingObjectsLayoutChanged;
		protected internal virtual void RaiseFloatingObjectsLayoutChanged() {
			if (FloatingObjectsLayoutChanged != null)
				FloatingObjectsLayoutChanged(this, EventArgs.Empty);
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
		#endregion
		public virtual void Reset(bool keepFloatingObjects) {
			Pages.Clear();
			tableStarted = false;
			CurrentSectionIndex = new SectionIndex(-1);
			this.pageBoundsCalculator = CreatePageBoundsCalculator();
			BeginNextSectionFormatting();
			if (!keepFloatingObjects)
				ClearFloatingObjectsLayout();
			RaisePageCountChanged();
		}
		public void SetFloatingObjectsLayout(FloatingObjectsLayout layout) {
			this.floatingObjectsLayout = layout;
			RaiseFloatingObjectsLayoutChanged();
		}
		public void ResetPageLastRunIndex() {
			pageLastRunIndex = RunIndex.MaxValue;
		}
		public void SetPageLastRunIndex(RunIndex runIndex) {
			this.pageLastRunIndex = runIndex;
			FloatingObjectsLayout.ClearFloatingObjects(runIndex + 1);
		}
		public virtual ClearInvalidatedContentResult ClearInvalidatedContentFromTheStartOfRowAtCurrentPage(FormatterPosition pos, ParagraphIndex paragraphIndex, bool keepFloatingObjects, TablesController tablesController, bool emptyCurrentRow, out bool leaveTable) {
			leaveTable = false;
			if (tableStarted) {
				TableCell currentCell = tablesController.GetCurrentCell();
				if ((currentCell != null && paragraphIndex >= currentCell.Table.StartParagraphIndex) || currentCell.Table.NestedLevel > 0)
					return ClearInvalidatedContentResult.ClearOnlyTableCellRows;
				tablesController.RemoveAllTableBreaks();
				leaveTable = true;
			}
			return ClearInvalidatedContent(pos, paragraphIndex, keepFloatingObjects, emptyCurrentRow);
		}
		public virtual ClearInvalidatedContentResult ClearInvalidatedContent(FormatterPosition pos, ParagraphIndex paragraphIndex, bool keepFloatingObjects, bool emptyCurrentRow) {
			if (!keepFloatingObjects)
				FloatingObjectsLayout.ClearFloatingObjects(pos.RunIndex);
			tableStarted = false;
			int pageIndex = Pages.BinarySearchBoxIndex(pos);
			if (pageIndex < 0)
				pageIndex = ~pageIndex;
			if (pageIndex > 0) {
				Column lastColumn = Pages[pageIndex - 1].GetLastColumn();
				lastColumn.RemoveEmptyTableViewInfos();
			}
			if (pageIndex >= Pages.Count && emptyCurrentRow) {
				if (!Pages.Last.IsEmpty)
					return ClearInvalidatedContentResult.NoRestart;
				CorrectLastPageOrdinal(paragraphIndex, pageIndex);
				return ClearInvalidatedContentResult.NoRestart;
			}
			Debug.Assert(pageIndex >= 0);
			if (pageIndex + 1 < Pages.Count) {
				int pageCount = Pages.Count - pageIndex - 1;
				if (pageCount > 0) {
					Pages.RemoveRange(pageIndex + 1, pageCount);
					RaisePageCountChanged();
				}
			}
			PieceTable pieceTable = PieceTable;
			if (pageIndex < Pages.Count)
				Pages[pageIndex].ClearInvalidatedContent(pos.RunIndex, pieceTable);
			SectionIndex newSectionIndex = pieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
			if (newSectionIndex > CurrentSectionIndex)
				return ClearInvalidatedContentResult.NoRestart;
			CurrentSectionIndex = newSectionIndex;
			Section section = DocumentModel.Sections[CurrentSectionIndex];
			if (section.FirstParagraphIndex == paragraphIndex && pos.Offset == 0 && pos.RunIndex == pieceTable.Paragraphs[paragraphIndex].FirstRunIndex) {
				CurrentSectionIndex--;
				Pages[pageIndex].ClearInvalidatedContent(pos.RunIndex, null); 
				return ClearInvalidatedContentResult.RestartFromTheStartOfSection;
			}
			else
				return ClearInvalidatedContentResult.Restart;
		}
		protected internal virtual void CorrectLastPageOrdinal(ParagraphIndex paragraphIndex, int pageIndex) {
			pageIndex = Pages.Count - 2; 
			if (pageIndex < 0)
				return;
			PieceTable pieceTable = PieceTable;
			Page lastNonEmptyPage = Pages[pageIndex];
			DocumentLogPosition emptyPageStartLogPosition = lastNonEmptyPage.GetLastPosition(pieceTable).LogPosition + 1;
			SectionIndex sectionIndex = pieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
			Section section = DocumentModel.Sections[sectionIndex];
			if (pieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition == emptyPageStartLogPosition) {
				this.nextPageOrdinalType = CalculateNextPageOrdinalType(section);
				CalculatePageOrdinalResult pageOrdinal = CalculatePageOrdinal(FirstPageOfSection);
				Pages.Last.PageOrdinal = pageOrdinal.PageOrdinal;
				Pages.Last.NumSkippedPages = pageOrdinal.SkippedPageCount;
				this.nextPageOrdinalType = SectionStartType.Continuous;
			}
		}
		protected internal virtual void BeginNextSectionFormatting() {
			CurrentSectionIndex++;
			Section section = DocumentLayout.DocumentModel.Sections[CurrentSectionIndex];
			ApplySectionStart(section);
			RestartFormattingFromTheMiddleOfSectionCore(section);
			this.firstPageOfSection = true;
		}
		protected internal virtual void RestartFormattingFromTheStartOfSection() {
			CurrentSectionIndex++;
			Section section = DocumentLayout.DocumentModel.Sections[CurrentSectionIndex];
			ApplySectionStart(section);
			RestartFormattingFromTheMiddleOfSectionCore(section);
			this.firstPageOfSection = true;
		}
		protected internal virtual Rectangle CalculatePageBounds(Section section) {
			return PageBoundsCalculator.CalculatePageBounds(section);
		}
		protected internal virtual void RestartFormattingFromTheMiddleOfSectionCore(Section section) {
			this.currentSection = section;
			this.pageBounds = CalculatePageBounds(CurrentSection);
			this.pageClientBounds = PageBoundsCalculator.CalculatePageClientBounds(CurrentSection);
			this.currentPageClientBounds = pageClientBounds;
			if (Pages.Count > 0) {
				Page lastPage = Pages.Last;
				if (!lastPage.IsEmpty) {
					PopulateFloatingObjectsLayout(lastPage);
					lastPage.ClearFloatingObjects();
				}
			}
		}
		void PopulateFloatingObjectsLayout(Page lastPage) {
			PopulateFloatingObjects(lastPage.InnerFloatingObjects);
			PopulateFloatingObjects(lastPage.InnerForegroundFloatingObjects);
			PopulateFloatingObjects(lastPage.InnerBackgroundFloatingObjects);
		}
		void PopulateFloatingObjects(List<FloatingObjectBox> floatingObjects) {
			if (floatingObjects == null)
				return;
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox floatingObject = floatingObjects[i];
				FloatingObjectAnchorRun run = (FloatingObjectAnchorRun)floatingObject.PieceTable.Runs[floatingObject.StartPos.RunIndex];
				FloatingObjectsLayout.Add(run, floatingObject);
			}
		}
		protected internal virtual void RestartFormattingFromTheMiddleOfSection(Section section) {
			RestartFormattingFromTheMiddleOfSectionCore(section);
			Page lastPage = Pages.Last;
			if (lastPage != null) {
				ApplyExistingHeaderAreaBounds(lastPage);
				ApplyExistingFooterAreaBounds(lastPage);
				this.currentPageClientBounds = lastPage.ClientBounds;
			}
			this.nextPageOrdinalType = SectionStartType.Continuous;
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage() {
		}
		public virtual CompleteFormattingResult CompleteCurrentPageFormatting() {
			if (PageCount > 0 && !AppendFloatingObjectsToPage(Pages.Last))
				return CompleteFormattingResult.OrphanedFloatingObjects;
			return CompleteFormattingResult.Success;
		}
		public virtual Page GetNextPage(bool keepFloatingObjects) {
			bool originalFirstPageOfSection = firstPageOfSection;
			if (PageCount > 0) {
				if (!tableStarted)
					PageContentFormattingComplete(Pages.Last);
			}
			if (!keepFloatingObjects)
				ClearFloatingObjectsLayout();
			Page newPage = GetNextPageCore();
			newPage.PageIndex = PageCount;
			CalculatePageOrdinalResult pageOrdinal = CalculatePageOrdinal(originalFirstPageOfSection);
			newPage.PageOrdinal = pageOrdinal.PageOrdinal;
			newPage.NumSkippedPages = pageOrdinal.SkippedPageCount;
			newPage.NumPages = DocumentModel.ExtendedDocumentProperties.Pages;
			nextPageOrdinalType = SectionStartType.Continuous;
			Pages.Add(newPage);
			RaisePageFormattingStarted(newPage);
			bool isFirstPageOfSection = NextSection ? true : originalFirstPageOfSection;
			FormatHeader(newPage, isFirstPageOfSection);
			FormatFooter(newPage, isFirstPageOfSection);
			this.currentPageClientBounds = newPage.ClientBounds;
			this.firstPageOfSection = false;
			this.nextSection = false;
			RaisePageCountChanged();
			ResetPageLastRunIndex();
			return newPage;
		}
		protected internal virtual CalculatePageOrdinalResult CalculatePageOrdinal(bool firstPageOfSection) {
			int pageOrdinal;
			int numSkippedPages;
			if (PageCount <= 0) {
				pageOrdinal = 0;
				numSkippedPages = 0;
			}
			else {
				pageOrdinal = Pages.Last.PageOrdinal;
				numSkippedPages = Pages.Last.NumSkippedPages;
			}
			return CalculatePageOrdinalCore(pageOrdinal, numSkippedPages, firstPageOfSection);
		}
		protected internal virtual CalculatePageOrdinalResult CalculatePageOrdinalCore(int previousPageOrdinal, int numSkippedPages, bool firstPageOfSection) {
			int pageOrdinal;
			if (firstPageOfSection && CurrentSection.PageNumbering.StartingPageNumber > 0)
				pageOrdinal = CurrentSection.PageNumbering.StartingPageNumber;
			else
				pageOrdinal = previousPageOrdinal + 1;
			bool isPageOrdinalOdd = ((pageOrdinal) % 2) != 0; 
			switch (nextPageOrdinalType) {
				case SectionStartType.OddPage:
					if (isPageOrdinalOdd)
						return new CalculatePageOrdinalResult(pageOrdinal, numSkippedPages);
					else
						return new CalculatePageOrdinalResult(pageOrdinal + 1, numSkippedPages + 1);
				case SectionStartType.EvenPage:
					if (!isPageOrdinalOdd)
						return new CalculatePageOrdinalResult(pageOrdinal, numSkippedPages);
					else
						return new CalculatePageOrdinalResult(pageOrdinal + 1, numSkippedPages + 1);
				default:
				case SectionStartType.Continuous:
					return new CalculatePageOrdinalResult(pageOrdinal, numSkippedPages);
			}
		}
		protected internal virtual Page GetNextPageCore() {
			Page result = new Page();
			result.Bounds = pageBounds;
			result.ClientBounds = pageClientBounds;
			if (CurrentSection.FootNote.NumberingRestartType == LineNumberingRestart.NewPage)
				DocumentLayout.Counters.GetCounter(FootNote.FootNoteCounterId).LastValue = CurrentSection.FootNote.StartingNumber;
			if (CurrentSection.EndNote.NumberingRestartType == LineNumberingRestart.NewPage)
				DocumentLayout.Counters.GetCounter(EndNote.EndNoteCounterId).LastValue = CurrentSection.EndNote.StartingNumber;
			return result;
		}
		protected internal virtual bool IsCurrentPageEven() {
			if (PageCount == 0)
				return false; 
			return Pages.Last.IsEven;
		}
		protected internal virtual void ApplySectionStart(Section section) {
			this.nextPageOrdinalType = CalculateNextPageOrdinalType(section);
			if (section.FootNote.NumberingRestartType == LineNumberingRestart.NewSection)
				DocumentLayout.Counters.GetCounter(FootNote.FootNoteCounterId).LastValue = section.FootNote.StartingNumber;
			if (section.EndNote.NumberingRestartType == LineNumberingRestart.NewSection)
				DocumentLayout.Counters.GetCounter(EndNote.EndNoteCounterId).LastValue = section.EndNote.StartingNumber;
		}
		protected internal virtual SectionStartType CalculateNextPageOrdinalType(Section section) {
			switch (section.GeneralSettings.StartType) {
				case SectionStartType.OddPage:
					return SectionStartType.OddPage;
				case SectionStartType.EvenPage:
					return SectionStartType.EvenPage;
				case SectionStartType.NextPage:
				case SectionStartType.Continuous:
				case SectionStartType.Column:
				default:
					return SectionStartType.Continuous;
			}
		}
		protected internal virtual void FinalizePagePrimaryFormatting(Page page, bool documentEnded) {
			if (documentEnded) {
#if DEBUGTEST || DEBUG
				bool added =
#endif
 AppendFloatingObjectsToPage(page);
#if DEBUGTEST || DEBUG
				Debug.Assert(added);
#endif
			}
			this.firstPageOfSection = false;
		}
		internal virtual bool AppendFloatingObjectsToPage(Page page) {
			bool pageContainsFloatingObjects = FloatingObjectsLayout.Items.Count > 0 ||
				FloatingObjectsLayout.ForegroundItems.Count > 0 ||
				FloatingObjectsLayout.BackgroundItems.Count > 0;
			if (!pageContainsFloatingObjects && !(ParagraphFramesLayout.Items.Count > 0)) {
				ClearFloatingObjectsLayout();
				ClearParagraphFramesLayout();
				return true;
			}
			RunIndex maxRunIndex = page.IsEmpty ? RunIndex.MaxValue : page.GetLastPosition(PieceTable).RunIndex;
			if (ContainsOrphanedItems(FloatingObjectsLayout.Items, maxRunIndex))
				return false;
			if (ContainsOrphanedItems(FloatingObjectsLayout.ForegroundItems, maxRunIndex))
				return false;
			if (ContainsOrphanedItems(FloatingObjectsLayout.BackgroundItems, maxRunIndex))
				return false;
			if (ContainsOrphanedItems(ParagraphFramesLayout.Items, maxRunIndex))
				return false;
			AppendFloatingObjects(FloatingObjectsLayout.Items, page.FloatingObjects, maxRunIndex);
			AppendFloatingObjects(FloatingObjectsLayout.ForegroundItems, page.ForegroundFloatingObjects, maxRunIndex);
			AppendFloatingObjects(FloatingObjectsLayout.BackgroundItems, page.BackgroundFloatingObjects, maxRunIndex);
			ClearFloatingObjectsLayout();
			AppendParagraphFrames(ParagraphFramesLayout.Items, page.ParagraphFrames, maxRunIndex);
			ParagraphFramesLayout.Clear();
			return true;
		}
		private bool ContainsOrphanedItems(List<FloatingObjectBox> items, RunIndex maxRunIndex) {
			if (items.Count == 0)
				return false;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox floatingObject = items[i];
				if (floatingObject.PieceTable == PieceTable && floatingObject.StartPos.RunIndex > maxRunIndex && floatingObject.WasRestart)
					return true;
			}
			return false;
		}
		private bool ContainsOrphanedItems(List<ParagraphFrameBox> items, RunIndex maxRunIndex) {
			if (items.Count == 0)
				return false;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ParagraphFrameBox paragraphFrame = items[i];
				if (paragraphFrame.PieceTable == PieceTable && paragraphFrame.StartPos.RunIndex > maxRunIndex && paragraphFrame.WasRestart)
					return true;
			}
			return false;
		}
		static FloatingObjectBoxZOrderComparer floatingObjectBoxZOrderComparer = new FloatingObjectBoxZOrderComparer();
		protected internal virtual void AppendFloatingObjects(List<FloatingObjectBox> from, List<FloatingObjectBox> to, RunIndex maxRunIndex) {
			to.Clear();
			if (from.Count == 0)
				return;
			int count = from.Count;
			for (int i = 0; i < count; i++) {
				FloatingObjectBox floatingObject = from[i];
				if (floatingObject.PieceTable != PieceTable || floatingObject.StartPos.RunIndex <= maxRunIndex)
					to.Add(floatingObject);
			}
			to.Sort(floatingObjectBoxZOrderComparer);
		}
		static ParagraphFrameBoxIndexComparer paragraphFrameBoxIndexComparer = new ParagraphFrameBoxIndexComparer();
		protected internal virtual void AppendParagraphFrames(List<ParagraphFrameBox> from, List<ParagraphFrameBox> to, RunIndex maxRunIndex) {
			to.Clear();
			if (from.Count == 0)
				return;
			int count = from.Count;
			for (int i = 0; i < count; i++) {
				ParagraphFrameBox box = from[i];
				if (box.PieceTable != PieceTable || box.StartPos.RunIndex <= maxRunIndex)
					to.Add(box);
			}
			to.Sort(paragraphFrameBoxIndexComparer);
		}
		protected internal virtual void ClearFloatingObjectsLayout() {
			FloatingObjectsLayout.Clear();
			floatingObjectsLayout = new FloatingObjectsLayout();
			RaiseFloatingObjectsLayoutChanged();
		}
		protected internal virtual void ClearParagraphFramesLayout() {
			ParagraphFramesLayout.Clear();
		}
		protected internal virtual void FormatHeader(Page page, bool firstPageOfSection) {
		}
		protected internal virtual void FormatFooter(Page page, bool firstPageOfSection) {
		}
		protected internal virtual void ApplyExistingHeaderAreaBounds(Page page) {
		}
		protected internal virtual void ApplyExistingFooterAreaBounds(Page page) {
		}
		protected internal abstract PageBoundsCalculator CreatePageBoundsCalculator();
		public void RemoveLastPage() {
			Pages.RemoveAt(Pages.Count - 1);
			ClearFloatingObjectsLayout();
			ClearParagraphFramesLayout();
		}
		public void BeginTableFormatting() {
			if (!tableStarted) {
				this.pagesBeforeTable = this.Pages.Count;
			}
			tableStarted = true;
		}
		public void EndTableFormatting() {
			tableStarted = false;
			int newPageCount = this.Pages.Count - pagesBeforeTable;
			if (newPageCount <= 0)
				return;
			Page[] newPages = new Page[newPageCount];
			Pages.CopyTo(pagesBeforeTable, newPages, 0, newPageCount);
			Pages.RemoveRange(pagesBeforeTable, newPageCount);
			for (int i = 0; i < newPageCount; i++) {
				PageContentFormattingComplete(Pages.Last);
				Pages.Add(newPages[i]);
			}
		}
		protected internal abstract BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result);
	}
	#endregion
	public struct CalculatePageOrdinalResult {
		readonly int pageOrdinal;
		readonly int skippedPageCount;
		public CalculatePageOrdinalResult(int pageOrdinal, int skippedPageCount) {
			this.pageOrdinal = pageOrdinal;
			this.skippedPageCount = skippedPageCount;
		}
		public int PageOrdinal { get { return pageOrdinal; } }
		public int SkippedPageCount { get { return skippedPageCount; } }
		public override bool Equals(object obj) {
			if (!(obj is CalculatePageOrdinalResult))
				return false;
			CalculatePageOrdinalResult other = (CalculatePageOrdinalResult)obj;
			return other.PageOrdinal == PageOrdinal && other.SkippedPageCount == SkippedPageCount;
		}
		public override int GetHashCode() {
			return PageOrdinal ^ (SkippedPageCount << 16);
		}
		public override string ToString() {
			return String.Format("PageOrdinal={0}, SkippedPageCount={1}", PageOrdinal, SkippedPageCount);
		}
	}
	#region NonPrintViewPageControllerBase (abstract class)
	public abstract class NonPrintViewPageControllerBase : PageController {
		protected NonPrintViewPageControllerBase(DocumentLayout documentLayout)
			: base(documentLayout, null, null) {
		}
		protected virtual int GetColumnBottom(Column column) {
			int result = 0;
			Row lastRow = column.Rows.Last;
			TableCellRow tableCellRow = lastRow as TableCellRow;
			if (tableCellRow != null) {
				TableViewInfo tableViewInfo = tableCellRow.CellViewInfo.TableViewInfo;
				while (tableViewInfo.ParentTableCellViewInfo != null)
					tableViewInfo = tableViewInfo.ParentTableCellViewInfo.TableViewInfo;
				result = tableViewInfo.GetTableBottom();
			}
			else
				result = lastRow.Bounds.Bottom;
			Rectangle columnBounds = column.Bounds;
			columnBounds.Height = int.MaxValue - columnBounds.Y;
			IList<FloatingObjectBox> floatingObjects = FloatingObjectsLayout.GetAllObjectsInRectangle(columnBounds);
			int count = floatingObjects.Count;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, floatingObjects[i].ExtendedBounds.Bottom);
			return result;
		}
	}
	#endregion
	#region PageBoundsCalculator
	public class PageBoundsCalculator {
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		public PageBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public DocumentModelUnitToLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		protected internal virtual Rectangle CalculatePageBounds(Section section) {
			SectionPage pageSettings = section.Page;
			int width = unitConverter.ToLayoutUnits(pageSettings.Width);
			int height = unitConverter.ToLayoutUnits(pageSettings.Height);
			return new Rectangle(0, 0, width, height);
		}
		protected internal virtual Rectangle CalculatePageClientBounds(Section section) {
			SectionPage page = section.Page;
			SectionMargins margins = section.Margins;
			return CalculatePageClientBoundsCore(page.Width, page.Height, margins.Left, Math.Abs(margins.Top), margins.Right, Math.Abs(margins.Bottom));
		}
		protected internal virtual Rectangle CalculatePageClientBoundsCore(int pageWidth, int pageHeight, int marginLeft, int marginTop, int marginRight, int marginBottom) {
			int width = UnitConverter.ToLayoutUnits(pageWidth);
			int height = UnitConverter.ToLayoutUnits(pageHeight);
			int left = UnitConverter.ToLayoutUnits(marginLeft);
			int top = UnitConverter.ToLayoutUnits(marginTop);
			int right = UnitConverter.ToLayoutUnits(marginRight);
			int bottom = UnitConverter.ToLayoutUnits(marginBottom);
			return Rectangle.FromLTRB(left, top, width - right, height - bottom);
		}
	}
	#endregion
}
