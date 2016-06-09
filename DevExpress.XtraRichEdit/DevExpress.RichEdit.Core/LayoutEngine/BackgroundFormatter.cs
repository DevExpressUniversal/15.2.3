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
using System.Threading;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.SpellChecker;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region BackgroundFormatterAction
	public static class BackgroundFormatterAction {
		public const int Shutdown = 0;
		public const int PerformPrimaryLayout = 2;
		public const int PerformSecondaryLayout = 1;
		public const int None = 3;
	}
	#endregion
	#region Predicate
	public delegate bool Predicate();
	#endregion
	public class BackgroundFormatter : IDisposable {
		#region Fields
		const int Infinite = -1;
		Thread workerThread;
		bool isDisposed;
		DocumentModelPosition secondaryLayoutStart;
		DocumentModelPosition secondaryLayoutEnd;
		DocumentModelPosition currentPosition;
		AutoResetEvent continueLayout;
		ManualResetEvent secondaryLayoutComplete;
		ManualResetEvent[] commands;
		DocumentFormatter documentFormatter;
		ParagraphFinalFormatter secondaryFormatter;
		Predicate<DocumentModelPosition> performPrimaryLayoutUntil;
		bool primaryLayoutComplete;
		DocumentFormattingController controller;
		SpellCheckerControllerBase spellCheckerController;
		int resetSecondaryLayoutFromPage;
		CommentPadding commentPadding;
		#endregion
		public BackgroundFormatter(DocumentFormattingController controller, CommentPadding commentPadding) {
			Guard.ArgumentNotNull(controller, "controller");
			Guard.ArgumentNotNull(commentPadding, "commentSkinPadding");
			this.controller = controller;
			this.spellCheckerController = new EmptySpellCheckerController();
			this.performPrimaryLayoutUntil = IsPrimaryLayoutComplete;
			this.documentFormatter = new DocumentFormatter(controller);
			this.commentPadding = commentPadding;
			this.secondaryFormatter = CreateParagraphFinalFormatter(controller.DocumentLayout);
			this.currentPosition = new DocumentModelPosition(PieceTable);
			this.currentPosition.LogPosition = new DocumentLogPosition(-1);
			this.secondaryLayoutStart = new DocumentModelPosition(PieceTable);
			this.secondaryLayoutEnd = new DocumentModelPosition(PieceTable);
			this.resetSecondaryLayoutFromPage = -1;
			this.continueLayout = new AutoResetEvent(true);
			this.secondaryLayoutComplete = new ManualResetEvent(false);
			this.commands = new ManualResetEvent[4];
			commands[BackgroundFormatterAction.Shutdown] = new ManualResetEvent(false);
			commands[BackgroundFormatterAction.PerformPrimaryLayout] = new ManualResetEvent(false);
			commands[BackgroundFormatterAction.PerformSecondaryLayout] = new ManualResetEvent(false);
			commands[BackgroundFormatterAction.None] = new ManualResetEvent(false);
			SubscribeToFormattingControllerEvents();
			this.workerThread = new Thread(Worker);
			this.workerThread.IsBackground = true;
#if !SL && !DXPORTABLE
			this.workerThread.Priority = ThreadPriority.Lowest;
#endif
			ResetPrimaryLayout();
			ResetSecondaryLayout();
		}
		protected virtual ParagraphFinalFormatter CreateParagraphFinalFormatter(DocumentLayout documentLayout) {
			return new ParagraphFinalFormatter(documentLayout, commentPadding);
		}
		protected internal virtual void OnNewMeasurementAndDrawingStrategyChanged() {
			documentFormatter.OnNewMeasurementAndDrawingStrategyChanged();
		}
		public void Start() {
			this.workerThread.Start(new WeakReference(this));
		}
		#region PageFormattingComplete
		PageFormattingCompleteEventHandler onPageFormattingComplete;
		public event PageFormattingCompleteEventHandler PageFormattingComplete { add { onPageFormattingComplete += value; } remove { onPageFormattingComplete -= value; } }
		void RaisePageFormattingComplete(PageFormattingCompleteEventArgs e) {
			if (onPageFormattingComplete != null)
				onPageFormattingComplete(this, e);
		}
		#endregion
		void SubscribeToFormattingControllerEvents() {
			controller.PageFormattingComplete += OnPageFormattingComplete;
		}
		void UnsubscribeFromFormattingControllerEvents() {
			controller.PageFormattingComplete -= OnPageFormattingComplete;
		}
		void OnPageFormattingComplete(object sender, PageFormattingCompleteEventArgs e) {
			RaisePageFormattingComplete(e);
		}
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		public DocumentFormatter DocumentFormatter { get { return documentFormatter; } }
		public DocumentModelPosition SecondaryLayoutStart { get { return secondaryLayoutStart; } }
		public DocumentModelPosition SecondaryLayoutEnd { get { return secondaryLayoutEnd; } }
		public Thread WorkerThread { get { return workerThread; } }
		public CommentPadding CommentPadding { get { return commentPadding; } }
		public SpellCheckerControllerBase SpellCheckerController {
			get { return spellCheckerController; }
			set {
				Guard.ArgumentNotNull(value, "SpellCheckerController");
				this.spellCheckerController = value;
			}
		}
		protected DocumentLayout DocumentLayout { get { return controller.DocumentLayout; } }
		protected DocumentModel DocumentModel { get { return controller.DocumentModel; } }
		protected PieceTable PieceTable { get { return controller.PieceTable; } }
		#endregion
		[System.Diagnostics.Conditional("DEBUG")]
		void CheckExecutedAtWorkerThread() {
#if DEBUG
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == WorkerThread.ManagedThreadId);
#endif
		}
		[System.Diagnostics.Conditional("DEBUG")]
		void CheckExecutedAtUIThread() {
#if DEBUG
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId != WorkerThread.ManagedThreadId);
#endif
		}
#if DEBUGTEST && !SL
		Exception threadException;
#endif
		#region Worker Thread Services
		static void Worker(object parameter) {
			WeakReference thisWeakRef = (WeakReference)parameter;
			if (thisWeakRef == null)
				return;	   
#if DEBUGTEST && !SL
			try {
#endif
				for (; ; ) {
					if (!WorkerBody(thisWeakRef))
						break;
				}
#if DEBUGTEST && !SL
			}
			catch (Exception e) {
				BackgroundFormatter thisObject = (BackgroundFormatter)thisWeakRef.Target;
				if (thisObject == null)
					return;
				thisObject.threadException = e;
			}
#endif
		}
		static bool WorkerBody(WeakReference thisWeakRef) {
			if (!WaitForContinueLayoutAllowed(thisWeakRef))
				return false;
			for (; ; ) {
				WaitHandle[] commands = GetCommands(thisWeakRef);
				if (commands == null)
					return false;
#if !SL && !DXPORTABLE
				int commandIndex = WaitHandle.WaitAny(commands, 1000, true);
#else
				int commandIndex = WaitHandle.WaitAny(commands, 1000);
#endif
				if (commandIndex != WaitHandle.WaitTimeout)
					return HandleCommand(thisWeakRef, commandIndex);
			}
		}
		static WaitHandle GetContinueLayoutEvent(WeakReference thisWeakRef) {
			BackgroundFormatter thisObject = (BackgroundFormatter)thisWeakRef.Target;
			if (thisObject == null)
				return null;
			return thisObject.continueLayout;
		}
		static WaitHandle[] GetCommands(WeakReference thisWeakRef) {
			BackgroundFormatter thisObject = (BackgroundFormatter)thisWeakRef.Target;
			if (thisObject == null)
				return null;
			return thisObject.commands;
		}
		static bool WaitForContinueLayoutAllowed(WeakReference thisWeakRef) {
			WaitHandle continueLayout = GetContinueLayoutEvent(thisWeakRef);
			if (continueLayout == null)
				return false;
			for (; ; ) {
#if !SL && !DXPORTABLE
				if (continueLayout.WaitOne(1000, true))
#else
				if (continueLayout.WaitOne(1000))
#endif
					return true;
				if (thisWeakRef.Target == null)
					return false;
			}
		}
		static bool HandleCommand(WeakReference thisWeakRef, int commandIndex) {
			BackgroundFormatter thisObject = thisWeakRef.Target as BackgroundFormatter;
			if (thisObject == null)
				return false;
			return thisObject.HandleCommand(commandIndex);
		}
		bool HandleCommand(int commandIndex) {
			switch (commandIndex) {
				case BackgroundFormatterAction.Shutdown:
					continueLayout.Set();
					return false;
				case BackgroundFormatterAction.PerformPrimaryLayout:
					PerformPrimaryLayout();
					continueLayout.Set();
					break;
				case BackgroundFormatterAction.PerformSecondaryLayout:
					PerformSecondaryLayout();
					continueLayout.Set();
					break;
				case BackgroundFormatterAction.None:
					continueLayout.Set();
					break;
			}
			return true;
		}
		protected internal virtual void PerformSecondaryLayout() {
			CheckExecutedAtWorkerThread();
			if (IsPrimaryLayoutComplete(currentPosition))
				PerformSecondaryLayoutCore();
			else if (performPrimaryLayoutUntil(currentPosition) && IsTableFormattingComplete(currentPosition))
				PerformSecondaryLayoutCore();
			else
				PerformPrimaryLayout();
		}
		bool IsTableFormattingComplete(DocumentModelPosition nextPosition) {			
			return !this.DocumentFormatter.ParagraphFormatter.RowsController.TablesController.IsInsideTable;
		}
		bool IsPositionInsideTable(ParagraphIndex paragraphIndex) {
			if (paragraphIndex >= ParagraphIndex.Zero)
				return PieceTable.Paragraphs[paragraphIndex].GetCell() != null;
			else
				return false;
		}
		protected internal virtual void PerformSecondaryLayoutCore() {
			CheckExecutedAtWorkerThread();
			PageCollection pages = controller.PageController.Pages;
			int firstPageIndex = Algorithms.BinarySearch(pages, new BoxAndLogPositionComparable<Page>(DocumentFormatter.DocumentModel.MainPieceTable, secondaryLayoutStart.LogPosition));
			if (firstPageIndex < 0)
				firstPageIndex = 0;
			int lastPageIndex = Algorithms.BinarySearch(pages, new BoxAndLogPositionComparable<Page>(DocumentFormatter.DocumentModel.MainPieceTable, secondaryLayoutEnd.LogPosition));
			if (lastPageIndex < 0)
				lastPageIndex = pages.Count - 1;
			for (int i = firstPageIndex; i <= lastPageIndex; i++) {
				Page page = pages[i];
#if DEBUGTEST
				if (i < lastPageIndex)
						Debug.Assert(page.PrimaryFormattingComplete == true);
				CheckPageTables(page);
#endif
				if (!page.SecondaryFormattingComplete) {
					PerformPageSecondaryFormatting(page);
				}
			}
			SpellCheckerController.CheckPages(firstPageIndex);
			secondaryLayoutComplete.Set();
			commands[BackgroundFormatterAction.PerformSecondaryLayout].Reset();
		}
		protected internal virtual void PerformPageSecondaryFormatting(Page page) {
			lock (this) {
#if DEBUGTEST || DEBUG
				CheckPageTables(page);
#endif
				secondaryFormatter.FormatPage(page);
				page.SecondaryFormattingComplete = page.PrimaryFormattingComplete;
			}
		}
#if DEBUGTEST || DEBUG
		internal static void CheckPageTables(Page page) {
			foreach (PageArea area in page.Areas) {
				CheckAreaTables(area);
			}
		}
		internal static void CheckAreaTables(PageArea area) {
			foreach (Column column in area.Columns) {
				CheckColumnTables(column);
			}
		}
		internal static void CheckColumnTables(Column column) {
			DevExpress.XtraRichEdit.Layout.TableLayout.TableViewInfoCollection columnTables = column.InnerTables;
			if (columnTables == null)
				return;
			foreach (DevExpress.XtraRichEdit.Layout.TableLayout.TableViewInfo tableViewInfo in columnTables)
				CheckColumnTables(tableViewInfo);
		}
		internal static void CheckColumnTables(DevExpress.XtraRichEdit.Layout.TableLayout.TableViewInfo tableViewInfo) {
			TableCellVerticalAnchorCollection anchors = tableViewInfo.Anchors;
			int maxAnchorIndex = anchors.Count;
			foreach(DevExpress.XtraRichEdit.Layout.TableLayout.TableCellViewInfo cell in tableViewInfo.Cells) {
				Debug.Assert(cell.TopAnchorIndex >= 0);
				Debug.Assert(cell.TopAnchorIndex <= maxAnchorIndex);
				Debug.Assert(cell.BottomAnchorIndex > cell.TopAnchorIndex);
				Debug.Assert(cell.BottomAnchorIndex <= maxAnchorIndex);
				Debug.Assert(anchors[cell.TopAnchorIndex] != null);
				Debug.Assert(anchors[cell.BottomAnchorIndex] != null);
			}
		}
#endif
		protected internal virtual void PerformPrimaryLayout() {
			CheckExecutedAtWorkerThread();
			PerformPrimaryLayoutCore();
		}
		protected internal virtual void PerformPrimaryLayoutCore() {
			FormattingProcessResult result = documentFormatter.FormatNextRow();			
			ParagraphIndex prevParagraphIndex = result.FormattingProcess == FormattingProcess.ContinueFromParagraph ? result.ParagraphIndex - 1 : documentFormatter.ParagraphIndex;
			if (prevParagraphIndex >= ParagraphIndex.Zero) {
				Paragraph paragraph = PieceTable.Paragraphs[prevParagraphIndex];
				this.currentPosition = new DocumentModelPosition(PieceTable);
				this.currentPosition.ParagraphIndex = prevParagraphIndex;
				this.currentPosition.RunIndex = documentFormatter.ParagraphFormatter.Iterator.RunIndex;
				int offset = 0;
				RunIndex count = result.FormattingProcess == FormattingProcess.ContinueFromParagraph ? paragraph.LastRunIndex + 1 : documentFormatter.ParagraphFormatter.Iterator.RunIndex;
				for (RunIndex i = paragraph.FirstRunIndex; i < count; i++) {
					offset += PieceTable.Runs[i].Length;
				}
				this.currentPosition.RunStartLogPosition = paragraph.LogPosition + offset;
				if (result.FormattingProcess != FormattingProcess.ContinueFromParagraph) {
					offset += documentFormatter.ParagraphFormatter.Iterator.Offset;
					this.currentPosition.LogPosition = paragraph.LogPosition + offset;
				}
				else
					this.currentPosition.LogPosition = paragraph.EndLogPosition;
			}
			else {
				this.currentPosition = new DocumentModelPosition(PieceTable);
				this.currentPosition.LogPosition = new DocumentLogPosition(-1);
			}
			if (result.FormattingProcess == FormattingProcess.Finish) {
				commands[BackgroundFormatterAction.PerformPrimaryLayout].Reset();
				primaryLayoutComplete = true;
#if DEBUGTEST
				for (int i = 0; i < controller.PageController.Pages.Count; i++) {
					Page page = controller.PageController.Pages[i];
					if (!page.PrimaryFormattingComplete)
						Exceptions.ThrowInternalException();
				}
#endif
				UpdateSecondaryPositions(this.SecondaryLayoutStart, controller.PageController.Pages.Last.GetLastPosition(documentFormatter.DocumentModel.MainPieceTable));
			}
		}
		protected internal virtual bool IsPrimaryLayoutComplete(DocumentModelPosition currentFormatterPosition) {
			CheckExecutedAtWorkerThread();
			return primaryLayoutComplete;
		}
#endregion
#region Main Thread Services
		public void BeginDocumentRendering(Predicate<DocumentModelPosition> performPrimaryLayoutUntil) {
			CheckExecutedAtUIThread();
			if (performPrimaryLayoutUntil == null)
				performPrimaryLayoutUntil = IsPrimaryLayoutComplete;
			bool alreadySuspended = !SuspendWorkerThread();
			if (alreadySuspended) {
				if (this.performPrimaryLayoutUntil != performPrimaryLayoutUntil) {
				}
				return;
			}
			this.performPrimaryLayoutUntil = performPrimaryLayoutUntil;			
			ResumeWorkerThread();
			SyncUtils.BlockedWaitOne(secondaryLayoutComplete);
			SuspendWorkerThread();
			if (resetSecondaryLayoutFromPage >= 0) {
				RefreshSecondaryLayout(resetSecondaryLayoutFromPage);
				resetSecondaryLayoutFromPage = -1;
			}
		}
		void RefreshSecondaryLayout(int firstPageIndex) {
			PageCollection pages = controller.PageController.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				Page page = pages[i];
				if (!page.PrimaryFormattingComplete)
					break;
				if(!page.SecondaryFormattingComplete)
					PerformPageSecondaryFormatting(page);
			}
		}
		public void EndDocumentRendering() {
			CheckExecutedAtUIThread();
			this.performPrimaryLayoutUntil = IsPrimaryLayoutComplete;			
			ResumeWorkerThread();
		}
		int documentBeginUpdateCount;
		public void BeginDocumentUpdate() {
			CheckExecutedAtUIThread();
			documentBeginUpdateCount++;
			SuspendWorkerThread();
		}
		public void EndDocumentUpdate() {
			CheckExecutedAtUIThread();
			Debug.Assert(documentBeginUpdateCount > 0);
			documentBeginUpdateCount--;
			ResumeWorkerThread();
		}
		public void WaitForPrimaryLayoutReachesLogPosition(DocumentLogPosition logPosition) {
			CheckExecutedAtUIThread();
			BeginDocumentUpdate();
			try {				
				while ((currentPosition.LogPosition <= logPosition || !IsTableFormattingComplete(currentPosition)) && !primaryLayoutComplete)
					PerformPrimaryLayoutCore();
			}
			finally {
				EndDocumentUpdate();
			}
		}
		public void WaitForPrimaryLayoutReachesPreferredPage(int preferredPageIndex) {
			CheckExecutedAtUIThread();
			BeginDocumentUpdate();
			try {
				while ((DocumentLayout.Pages.Count - 1 <= preferredPageIndex || !IsTableFormattingComplete(currentPosition)) && !primaryLayoutComplete)
					PerformPrimaryLayoutCore();
			}
			finally {
				EndDocumentUpdate();
			}
		}
		public void WaitForPagePrimaryLayoutComplete(Page page) {
			CheckExecutedAtUIThread();
			BeginDocumentUpdate();
			try {
				while (!page.PrimaryFormattingComplete && !primaryLayoutComplete)
					PerformPrimaryLayoutCore();
			}
			finally {
				EndDocumentUpdate();
			}			
		}
		public void WaitForSecondaryLayoutReachesPosition(DocumentModelPosition pos) {
			CheckExecutedAtUIThread();
			DocumentModelPosition originalStartPos;
			DocumentModelPosition originalEndPos;
			BeginDocumentUpdate();
			try {
				originalStartPos = SecondaryLayoutStart;
				originalEndPos = SecondaryLayoutEnd;
				DocumentModelPosition startPos;
				if (pos < SecondaryLayoutStart)
					startPos = pos;
				else
					startPos = SecondaryLayoutStart;
				DocumentModelPosition endPos;
				if (pos > SecondaryLayoutEnd)
					endPos = pos;
				else
					endPos = SecondaryLayoutEnd;
				UpdateSecondaryPositions(startPos, endPos);
				NotifyDocumentChanged(startPos, endPos, DocumentLayoutResetType.SecondaryLayout);
			}
			finally {
				EndDocumentUpdate();
			}
			Predicate<DocumentModelPosition> predicate = delegate(DocumentModelPosition currentFormatterPosition)
			{
				return currentFormatterPosition > pos;
			};
			BeginDocumentRendering(predicate);
			UpdateSecondaryPositions(originalStartPos, originalEndPos);
			NotifyDocumentChanged(originalStartPos, originalEndPos, DocumentLayoutResetType.SecondaryLayout);
			EndDocumentRendering();
		}
		int threadSuspendCount;
#if DEBUGTEST
		[System.Security.SecuritySafeCritical]
#endif
		protected internal virtual bool SuspendWorkerThread() {
			CheckExecutedAtUIThread();
			threadSuspendCount++;
			if (threadSuspendCount > 1)
				return false;
			commands[BackgroundFormatterAction.None].Set();
#if DEBUGTEST && !SL && !DXPORTABLE
			for (; ; ) {
				if (SyncUtils.BlockedWaitOne(continueLayout, 10000))
					break;
				if (threadException != null) {
				NUnit.Framework.Assert.Fail(threadException.ToString());
					break;
				}
			}
#else
			SyncUtils.BlockedWaitOne(continueLayout);
#endif
			commands[BackgroundFormatterAction.None].Reset();
			return true;
		}
		protected internal virtual void ResumeWorkerThread() {
			CheckExecutedAtUIThread();
			Debug.Assert(threadSuspendCount > 0);
			threadSuspendCount--;
			if (threadSuspendCount == 0)
				continueLayout.Set();
		}
		internal void Restart(DocumentModelPosition from, DocumentModelPosition to, DocumentModelPosition secondaryFrom) {
			UpdateSecondaryPositions(secondaryFrom, to);
			NotifyDocumentChanged(from, to, DocumentLayoutResetType.PrimaryLayoutFormPosition);
			this.currentPosition = from.Clone();
			DocumentFormatter.Restart(from);
		}
		internal void UpdateSecondaryPositions(DocumentModelPosition from, DocumentModelPosition to) {
			this.secondaryLayoutStart = from;
			this.secondaryLayoutEnd = to;
		}
		public void NotifyDocumentChanged(DocumentModelPosition from, DocumentModelPosition to, DocumentLayoutResetType documentLayoutResetType) {
			switch (documentLayoutResetType) {
				case DocumentLayoutResetType.SecondaryLayout:
					ResetSecondaryLayout();
					break;
				case DocumentLayoutResetType.AllPrimaryLayout:
					ResetSecondaryLayout();
					ResetPrimaryLayout();
					break;
				case DocumentLayoutResetType.PrimaryLayoutFormPosition:
					ResetSecondaryLayout();
					if (ShouldResetPrimaryLayout(from, to))
						ResetPrimaryLayout(from, to);
					break;
				case DocumentLayoutResetType.None:
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal virtual bool ShouldResetPrimaryLayout(DocumentModelPosition from, DocumentModelPosition to) {
			return from <= currentPosition || to <= currentPosition;
		}
		protected internal virtual bool ShouldResetSecondaryLayout(DocumentModelPosition from, DocumentModelPosition to) {
			if (from > secondaryLayoutEnd)
				return false;
			if (to < secondaryLayoutStart)
				return false;
			return true;
		}
		protected internal virtual void ResetSecondaryLayout() {
			secondaryLayoutComplete.Reset();
			commands[BackgroundFormatterAction.PerformSecondaryLayout].Set();
		}
		protected internal virtual void ResetPrimaryLayout() {
			documentFormatter.ParagraphIndex = new ParagraphIndex(-1);
			documentFormatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
			this.currentPosition = new DocumentModelPosition(PieceTable);
			this.currentPosition.LogPosition = new DocumentLogPosition(-1);
			this.primaryLayoutComplete = false;
			commands[BackgroundFormatterAction.PerformPrimaryLayout].Set();
			secondaryFormatter.BookmarkCalculator.Reset();
			secondaryFormatter.RangePermissionCalculator.Reset();
			secondaryFormatter.CommentCalculator.Reset();
			secondaryFormatter.CustomMarkCalculator.Reset();
			this.spellCheckerController.Reset();
		}
		protected internal virtual void ResetPrimaryLayout(DocumentModelPosition from, DocumentModelPosition to) {
			documentFormatter.ParagraphIndex = from.ParagraphIndex - 1;
			documentFormatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
			this.currentPosition = DocumentModelPosition.FromParagraphStart(PieceTable, from.ParagraphIndex);
			this.primaryLayoutComplete = false;
			commands[BackgroundFormatterAction.PerformPrimaryLayout].Set();
			secondaryFormatter.BookmarkCalculator.ResetFrom(from);
			secondaryFormatter.RangePermissionCalculator.ResetFrom(from);
			secondaryFormatter.CommentCalculator.ResetFrom(from);
			secondaryFormatter.CustomMarkCalculator.ResetFrom(from);
			this.spellCheckerController.Reset();
		}
#endregion
#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (IsDisposed)
				return;
			if (disposing) {
				SuspendWorkerThread();
				commands[BackgroundFormatterAction.Shutdown].Set();
				ResumeWorkerThread();
				if (workerThread.ThreadState != System.Threading.ThreadState.Unstarted) {
#if (DEBUGTEST)
					workerThread.Join(5000);
#else
					workerThread.Join();
#endif
					workerThread = null;
				}
				UnsubscribeFromFormattingControllerEvents();
				if (spellCheckerController != null) {
					spellCheckerController.Dispose();
					spellCheckerController = null;
				}
				if (this.documentFormatter != null) {
					documentFormatter.Dispose();
					documentFormatter = null;
				}
				this.commands = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
#endregion
		internal void ResetSecondaryFormattingForPage(Page page, int pageIndex) {
			if (resetSecondaryLayoutFromPage >= 0)
				this.resetSecondaryLayoutFromPage = Math.Min(pageIndex, resetSecondaryLayoutFromPage);
			else
				resetSecondaryLayoutFromPage = pageIndex;
		}
		public void ApplyNewCommentPadding(CommentPadding commentPadding) {
			this.commentPadding = commentPadding;
			this.secondaryFormatter.ApplyNewCommentPadding(commentPadding);
		}
	}
}
