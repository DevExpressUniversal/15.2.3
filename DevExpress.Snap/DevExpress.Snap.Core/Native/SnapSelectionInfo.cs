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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.History;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native.Data.Implementations;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native {
	public class SnapSelectionInfo {
		readonly Stack<EnteredBookmarkInfo> enteredBookmarksTrunk;
		readonly List<EnteredBookmarkInfo> enteredBookmarksLeaves;
		readonly SnapDocumentModel documentModel;
		DocumentLogInterval prevSelection;
		PieceTable prevSelectionPieceTable;
		public SnapSelectionInfo(SnapDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");			
			this.documentModel = documentModel;
			this.enteredBookmarksTrunk = new Stack<EnteredBookmarkInfo>();
			this.enteredBookmarksLeaves = new List<EnteredBookmarkInfo>();
		}
		public SnapDocumentModel DocumentModel { get { return documentModel; } }
		public Stack<EnteredBookmarkInfo> EnteredBookmarks { get { return enteredBookmarksTrunk; } }
		public List<EnteredBookmarkInfo> EnteredBookmarksLeaves { get { return enteredBookmarksLeaves; } }
		protected Selection Selection { get { return DocumentModel.Selection; } }
		protected Office.History.DocumentHistory History { get { return DocumentModel.History; } }
		protected internal void ResetCurrentBookmark() {
			ResetCurrentBookmark(false);
		}
		protected internal void ResetCurrentBookmark(bool force) {
			DocumentLogInterval selectionInterval = GetWholeSelectionInterval();
			SnapBookmark currentBookmark = GetBoundedBookmark(selectionInterval.Start, selectionInterval.Start + selectionInterval.Length, (SnapPieceTable)Selection.PieceTable);
			List<SnapBookmark> list = GetSelectedBookmarks(selectionInterval.Start, selectionInterval.Start + selectionInterval.Length, (SnapPieceTable)Selection.PieceTable);
			if (Object.ReferenceEquals(currentBookmark, null) && (!Object.ReferenceEquals(list, null)))
				if (list.Count > 0)
					currentBookmark = list[0];
			if (Object.ReferenceEquals(currentBookmark, null))
				return;
			RemoveDeletedBookmarks();
			ProcessLeavedBookmarks(currentBookmark.Parent, false, force);
			selectionInterval = GetWholeSelectionInterval();
			currentBookmark = GetBoundedBookmark(selectionInterval.Start, selectionInterval.Start + selectionInterval.Length, (SnapPieceTable)Selection.PieceTable);
			ProcessEnteredBookmarks(currentBookmark);
		}
		protected internal bool CheckCurrentSnapBookmark() {
			return CheckCurrentSnapBookmark(false, false);
		}
		public bool CheckCurrentSnapBookmark(bool suppressUpdate, bool internalUpdate) {
			DocumentLogIntervalEx selectionInterval = GetWholeSelectionInterval();
			return CheckCurrentSnapBookmark(suppressUpdate, internalUpdate, selectionInterval.Start, selectionInterval.Start + selectionInterval.Length, (SnapPieceTable)selectionInterval.PieceTable);
		}
		DocumentLogIntervalEx GetWholeSelectionInterval() {
			if (!DocumentModel.ActivePieceTable.IsTextBox)
				return GetWholeSelectionIntervalCore();
			TextBoxContentType contentType = (TextBoxContentType)DocumentModel.ActivePieceTable.ContentType;
			RunIndex runIndex = contentType.AnchorRun.GetRunIndex();
			PieceTable pieceTable = contentType.AnchorRun.PieceTable;
			DocumentModelPosition anchorPosition = DocumentModelPosition.FromRunStart(pieceTable, runIndex);
			return new DocumentLogIntervalEx(pieceTable, anchorPosition.LogPosition, 0);
		}
		DocumentLogIntervalEx GetWholeSelectionIntervalCore() {
			List<SelectionItem> selectionItems = Selection.Items;
			int count = selectionItems.Count;
			Debug.Assert(count > 0);
			DocumentLogPosition startSelection = selectionItems[0].NormalizedStart;
			DocumentLogPosition endSelection = selectionItems[0].NormalizedEnd;
			for (int i = 1; i < count; i++) {
				startSelection = Algorithms.Min(startSelection, selectionItems[i].NormalizedStart);
				endSelection = Algorithms.Max(endSelection, selectionItems[i].NormalizedEnd);
			}
			return new DocumentLogIntervalEx(Selection.PieceTable, startSelection, endSelection - startSelection);
		}
		public bool CheckCurrentSnapBookmark(bool suppressUpdate, bool internalUpdate, DocumentLogPosition startPosition, DocumentLogPosition endPosition, SnapPieceTable selectionPieceTable) {
			bool currentBookmarkChanged = CheckCurrentSnapBookmarkCore(suppressUpdate, internalUpdate, startPosition, endPosition, selectionPieceTable);
			if (currentBookmarkChanged)
				DocumentModel.RaiseCurrentSnapBookmarkChanged();
			return currentBookmarkChanged;
		}
		bool CheckCurrentSnapBookmarkCore(bool suppressUpdate, bool internalUpdate, DocumentLogPosition startPosition, DocumentLogPosition endPosition, SnapPieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
#if DEBUGTEST || DEBUG
			int historyItemIndexBefore = DocumentModel.History.CurrentIndex;
#endif
			try {
				SnapBookmark currentBookmark = GetBoundedBookmark(startPosition, endPosition, pieceTable);
				bool deleted = RemoveDeletedBookmarks();
				List<SnapBookmark> enteredBookmarks = GetSelectedBookmarks(startPosition, endPosition, pieceTable);
				if(!IsCurrentBookmarkChanged(currentBookmark) && !AreSelectedBookmarksChanged(enteredBookmarks))
					return deleted;
				LeaveBookmarksUntilCurrent(suppressUpdate, currentBookmark);
				if (History.Transaction != null && !suppressUpdate && !internalUpdate)
					Exceptions.ThrowInternalException();				
				if (!suppressUpdate) {
					DocumentLogIntervalEx selectionInterval = GetWholeSelectionInterval();
					startPosition = selectionInterval.Start;
					endPosition = selectionInterval.Start + selectionInterval.Length;
					pieceTable = (SnapPieceTable)selectionInterval.PieceTable;
				}
				currentBookmark = GetBoundedBookmark(startPosition, endPosition, pieceTable);				
				ProcessEnteredBookmarks(currentBookmark);
				List<SnapBookmark> bookmarks = GetSelectedBookmarks(startPosition, endPosition, pieceTable);
				if(!object.ReferenceEquals(bookmarks, null))
					ProcessEnteredBookmarks(bookmarks);
				return true;
			}
			finally {
				prevSelection = new DocumentLogInterval(startPosition, endPosition - startPosition);
				prevSelectionPieceTable = pieceTable;
#if DEBUGTEST || DEBUG
				if (!internalUpdate) {
					int newHistoryItemIndex = History.CurrentIndex - historyItemIndexBefore;
					if (newHistoryItemIndex < 0 || newHistoryItemIndex > 1)
						Exceptions.ThrowInternalException();
				}
#endif
			}
		}
		bool RemoveDeletedBookmarks() {
			bool changed = false;
			Stack<EnteredBookmarkInfo> trunk = EnteredBookmarks;
#if DEBUGTEST || DEBUG
			if(EnteredBookmarksLeaves.Count != 0)
				if(trunk.Count > 0 && trunk.Peek().Bookmark.Deleted)
					Exceptions.ThrowInternalException();
#endif
			while (trunk.Count > 0 && trunk.Peek().Bookmark.Deleted) {
				EnteredBookmarkInfo bookmarkInfo = trunk.Pop();
				bookmarkInfo.Dispose();
				changed = true;
			}
			return changed;
		}
		bool IsCurrentBookmarkChanged(SnapBookmark newCurrentBookmark) {
			if (EnteredBookmarks.Count == 0)
				return newCurrentBookmark != null;
			if (new SnapObjectModelController((SnapPieceTable)Selection.PieceTable).IsWholeContentSelected())
				return false;
			return !Object.ReferenceEquals(enteredBookmarksTrunk.Peek().Bookmark, newCurrentBookmark);
		}
		bool AreSelectedBookmarksChanged(List<SnapBookmark> newSelectedBookmarks) {
			if(EnteredBookmarksLeaves.Count == 0 && !IsCollectionNullOrEmpty(newSelectedBookmarks))
				return true;
			if(EnteredBookmarksLeaves.Count != 0 && IsCollectionNullOrEmpty(newSelectedBookmarks))
				return true;
			if(EnteredBookmarksLeaves.Count == 0 && IsCollectionNullOrEmpty(newSelectedBookmarks))
				return false;
			if(EnteredBookmarksLeaves.Count != newSelectedBookmarks.Count)
				return true;
			for(int i = 0; i < EnteredBookmarksLeaves.Count; i++) {
				EnteredBookmarkInfo bookmarkInfo = EnteredBookmarksLeaves[i];
				if(!newSelectedBookmarks.Contains(bookmarkInfo.Bookmark))
					return true;
			}
			return false;
		}
		bool IsCollectionNullOrEmpty(List<SnapBookmark> bookmarks) {
			return object.ReferenceEquals(bookmarks, null) || bookmarks.Count == 0;
		}
		List<SnapBookmark> GetSelectedBookmarks(DocumentLogPosition startPosition, DocumentLogPosition endPosition, SnapPieceTable pieceTable) {
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			SnapBookmark startBookmark = controller.FindInnermostTemplateBookmarkByPosition(startPosition);
			SnapBookmark endBookmark = startPosition != endPosition ? controller.FindInnermostTemplateBookmarkByPosition(endPosition - 1) : startBookmark;
			if (startBookmark == endBookmark || startBookmark == null || endBookmark == null) {
				return null;
			}
			if(((ISingleObjectFieldContext)startBookmark.FieldContext).Parent == ((ISingleObjectFieldContext)endBookmark.FieldContext).Parent) {
				List<SnapBookmark> result = new List<SnapBookmark>();
				HashSet<SnapTemplateIntervalType> flags = new HashSet<SnapTemplateIntervalType>();
				SnapBookmark bookmark = startBookmark;
				for(; ; ) {
					SnapTemplateIntervalType bmType = bookmark.TemplateInterval.TemplateInfo.TemplateType;
					if(!flags.Contains(bmType)) {
						result.Add(bookmark);
						flags.Add(bmType);
					}
					if(bookmark == endBookmark)
						break;
					bookmark = FindNextBookmark(controller, startBookmark.Parent, bookmark);
				}
				return result;
			}
			return null;
		}
		static SnapBookmark FindNextBookmark(SnapBookmarkController controller, SnapBookmark parent, SnapBookmark bookmark) {
			DocumentLogPosition pos = bookmark.End + 1;
			do {
				bookmark = controller.FindInnermostTemplateBookmarkByPosition(pos);
				pos++;
			}
			while(bookmark == parent);
			while(bookmark.Parent != parent)
				bookmark = bookmark.Parent;
			return bookmark;
		}
		internal SnapBookmark GetBoundedBookmark(DocumentLogPosition startPosition, DocumentLogPosition endPosition, SnapPieceTable pieceTable) {
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			SnapBookmark startBookmark = controller.FindInnermostTemplateBookmarkByPosition(startPosition);
			SnapBookmark endBookmark = startPosition != endPosition ? controller.FindInnermostTemplateBookmarkByPosition(endPosition - 1) : startBookmark;
			return FindCommonParentBookmark(startBookmark, endBookmark);
		}
		SnapBookmark FindCommonParentBookmark(SnapBookmark startBookmark, SnapBookmark endBookmark) {
			if (startBookmark == endBookmark)
				return startBookmark;
			List<SnapBookmark> bookmarks = new List<SnapBookmark>();
			SnapBookmark currentBookmark = startBookmark;
			while (currentBookmark != null) {
				bookmarks.Add(currentBookmark);
				currentBookmark = currentBookmark.Parent;
			}
			currentBookmark = endBookmark;
			while (currentBookmark != null) {
				if (bookmarks.Contains(currentBookmark))
					return currentBookmark;
				currentBookmark = currentBookmark.Parent;
			}
			return null;
		}
		void LeaveBookmarksUntilCurrent(bool suppressUpdate, SnapBookmark currentBookmark) {
			DocumentModel.BeginUpdate();
			try {
				if (prevSelectionPieceTable != null)
					((SnapDocumentHistory)History).ChangeSavedSelectionState(new SelectionState(prevSelectionPieceTable, prevSelection));
				ProcessLeavedBookmarks(currentBookmark, suppressUpdate, false);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}	   
		Field GetSNListField(SnapPieceTable pieceTable, RunInfo runInfo) {
			Field field = pieceTable.FindNonTemplateFieldByRunIndex(runInfo.Start.RunIndex);
			while (field != null) {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField parsedInfo = calculator.ParseField(pieceTable, field) as SNListField;
				if (parsedInfo != null)
					return field;
				field = field.Parent;
			}
			return null;
		}
		protected internal bool IsWholeFieldSelected(SnapPieceTable pieceTable, Field field, RunIndex startRunIndex, RunIndex endRunIndex) {
			IVisibleTextFilter filter = pieceTable.VisibleTextFilter;
			if (startRunIndex < field.FirstRunIndex) {
				return false;
			}
			while (startRunIndex > field.FirstRunIndex) {
				startRunIndex--;
				if (filter.IsRunVisible(startRunIndex)) {
					return false;
				}
			}
			if (endRunIndex > field.LastRunIndex) {
				return false;
			}
			while (endRunIndex < field.LastRunIndex) {
				endRunIndex++;
				if (filter.IsRunVisible(endRunIndex)) {
					return false;
				}
			}
			return true;
		}
		void ProcessLeavedBookmarks(SnapBookmark currentBookmark, bool suppressUpdate, bool force) {
			bool saveSelection = true;
			bool leavesLeaved = false;
			Stack<EnteredBookmarkInfo> trunk = EnteredBookmarks;
			List<EnteredBookmarkInfo> leaves = EnteredBookmarksLeaves;
			foreach(EnteredBookmarkInfo bookmarkInfo in leaves) { 
				if(ShouldLeaveBookmark(bookmarkInfo.Bookmark, currentBookmark)) {
					if(OnBookmarkLeave(bookmarkInfo, saveSelection, suppressUpdate, force))
						saveSelection = false;
					leavesLeaved = true;
				}
			}
			if(leavesLeaved)
				leaves.Clear();
			if(leavesLeaved || leaves.Count == 0) {
				while(trunk.Count > 0 && ShouldLeaveBookmark(trunk.Peek().Bookmark, currentBookmark)) {
					EnteredBookmarkInfo bookmarkInfo = trunk.Peek();
					if(OnBookmarkLeave(bookmarkInfo, saveSelection, suppressUpdate, force))
						saveSelection = false;
					trunk.Pop();
				}
			}
		}
		bool ShouldLeaveBookmark(SnapBookmark bookmark, SnapBookmark currentBookmark) {
			if (currentBookmark == null || !Object.ReferenceEquals(bookmark.PieceTable, currentBookmark.PieceTable))
				return true;
			return !bookmark.Contains(currentBookmark);
		}
		bool OnBookmarkLeave(EnteredBookmarkInfo bookmarkInfo, bool saveSelection, bool suppressUpdate, bool force) {
			if (!force && (!bookmarkInfo.Modified || suppressUpdate))
				return false;
			SnapBookmark bookmark = bookmarkInfo.Bookmark;
			SnapPieceTable pieceTable = bookmark.PieceTable;
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.NormalizedStart);
			Field field = pieceTable.FindNonTemplateFieldByRunIndex(position.RunIndex);
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			while (field != null && calculator.ParseField(pieceTable, field) as SNListField == null)
				field = field.Parent;
			if(field == null)
				return false;
			DocumentModel.BeginUpdate();
			try {
				if (saveSelection) {
					SaveEnteredBookmarksHistoryItem historyItem = new SaveEnteredBookmarksHistoryItem(pieceTable);
					historyItem.EnteredBookmarks = new List<EnteredBookmarkInfo>();
					foreach (EnteredBookmarkInfo info in EnteredBookmarks)
						historyItem.EnteredBookmarks.Add(info.Clone());
					History.Add(historyItem);
				}
				SnapCaretPosition caretPosition = null;
				if (prevSelectionPieceTable != null && prevSelection != null)
					caretPosition = GetSnapCaretPositionFromSelection(prevSelectionPieceTable, prevSelection.Start);
				pieceTable.UpdateTemplateCore(bookmark.NormalizedStart, true);
				pieceTable.FieldUpdater.UpdateFieldAndNestedFields(field);
				if (caretPosition != null)
					RestoreCaretPosition(caretPosition);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return true;
		}
		internal void ProcessEnteredBookmarks(SnapBookmark currentBookmark) {
			if (currentBookmark == null)
				return;
			if(EnteredBookmarksLeaves.Count == 1 && (EnteredBookmarks.Count == 0 || object.ReferenceEquals(EnteredBookmarksLeaves[0].Bookmark.Parent, EnteredBookmarks.Peek().Bookmark))) {
				EnteredBookmarks.Push(EnteredBookmarksLeaves[0]);
				EnteredBookmarksLeaves.Clear();
			}
#if DEBUGTEST || DEBUG
			if(EnteredBookmarksLeaves.Count != 0)
				Exceptions.ThrowInternalException();
#endif
			Stack<SnapBookmark> stack = new Stack<SnapBookmark>();
			Stack<EnteredBookmarkInfo> trunk = EnteredBookmarks;
			SnapBookmark innerBookmark = trunk.Count > 0 ? trunk.Peek().Bookmark : null;
			while (!Object.ReferenceEquals(currentBookmark, innerBookmark)) {
				stack.Push(currentBookmark);
				currentBookmark = currentBookmark.Parent;
#if DEBUGTEST || DEBUG
				if (stack.Count > 100)
					Exceptions.ThrowInternalException();
#endif
			}
			while (stack.Count > 0) {
				SnapBookmark bookmark = stack.Pop();
				trunk.Push(new EnteredBookmarkInfo(bookmark));				
			}
		}
		internal void ProcessEnteredBookmarks(IEnumerable<SnapBookmark> selectedBookmarks) {
#if DEBUGTEST || DEBUG
			int cnt = EnteredBookmarksLeaves.Count;
#endif
			foreach(SnapBookmark bookmark in selectedBookmarks) {
#if DEBUGTEST || DEBUG
				if(++cnt > 100)
					Exceptions.ThrowInternalException();
#endif
				EnteredBookmarksLeaves.Add(new EnteredBookmarkInfo(bookmark));
			}
		}
		public SnapCaretPosition GetSnapCaretPositionFromSelection(PieceTable pieceTable, DocumentLogPosition position) {
			SnapPieceTable snapPieceTable = pieceTable as SnapPieceTable;
			if (snapPieceTable == null)
				return null;
			SnapBookmark bookmark = new SnapBookmarkController(snapPieceTable).FindInnermostTemplateBookmarkByPosition(position);
			if (bookmark == null)
				return null;
			int offset = position - bookmark.NormalizedStart;
			TableCell cell = new SnapObjectModelController(snapPieceTable).FindCellByLogPosition(position);
			int columnIndex = cell != null ? cell.GetStartColumnIndexConsiderRowGrid() : 0;
			List<int> relativeIndexes = new List<int>();
			while (bookmark != null) {
				int index = snapPieceTable.GetIndexRelativeToParent(bookmark);
				relativeIndexes.Add(index);
				bookmark = bookmark.Parent;
			}
			return new SnapCaretPosition(snapPieceTable, relativeIndexes.ToArray(), offset, columnIndex);
		}
		public void RestoreCaretPosition(SnapCaretPosition caretPosition) {
			SnapPieceTable pieceTable = caretPosition.PieceTable;
			if (pieceTable != DocumentModel.ActivePieceTable)
				return;
			int[] relativeIndexes = caretPosition.RelativeBookmarkIndexes;
			int offset = caretPosition.Offset;
			SnapBookmark bookmark = null;
			for (int i = relativeIndexes.Length - 1; i >= 0; i--) {
				SnapBookmark currentBookmark = pieceTable.GetBookmarkByRelativeIndex(bookmark, relativeIndexes[i]);
				if (currentBookmark == null) {
					offset = 0;
					break;
				}
				bookmark = currentBookmark;
			}
			DocumentLogPosition position = (bookmark != null ? bookmark.NormalizedStart : DocumentLogPosition.Zero);
			if (bookmark != null && offset < bookmark.Length)
				position += offset;
			TableCell cell = new SnapObjectModelController(pieceTable).FindCellByLogPosition(position);
			if (cell != null && bookmark != null) {
				if (cell.IndexInRow != caretPosition.ColumnIndex) {
					int rowIndex = cell.Row.IndexInTable;
					TableCell newCell = cell.Table.GetCell(rowIndex, caretPosition.ColumnIndex);
					if (newCell == null)
						newCell = cell.Table.Rows[rowIndex].FirstCell;
					cell = newCell;
				}
				DocumentLogPosition cellPosition = pieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
				if (cell.StartParagraphIndex == cell.EndParagraphIndex || pieceTable.Paragraphs[cell.EndParagraphIndex - 1].GetCell() == cell)
					if ((bookmark.NormalizedStart <= cellPosition && cellPosition < bookmark.NormalizedEnd))
						position = cellPosition;
			}
			if (position != DocumentLogPosition.Zero && bookmark != null && position > bookmark.NormalizedEnd)
				position = Algorithms.Max(bookmark.NormalizedEnd - 1, bookmark.NormalizedStart);
			Selection selection = pieceTable.DocumentModel.Selection;
			selection.Start = position;
			selection.End = position;
			selection.SetStartCell(position);
		}
		protected internal void PerformModifyModelBySelection(Action modifier) {
			DocumentModel.BeginUpdate();
			try {
				PerformModifyModelBySelectionCore(modifier);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void PerformModifyModelBySelectionCore(Action modifier) {
			SnapCaretPosition caretPosition = GetSnapCaretPositionFromSelection(Selection.PieceTable, Selection.NormalizedStart);
			try {
				modifier();
			}
			finally {
				UpdateDocument(caretPosition);
			}
		}
		protected internal void UpdateDocument(SnapCaretPosition caretPosition) {
			if (!Object.ReferenceEquals(caretPosition, null))
				RestoreCaretPosition(caretPosition);
			ResetCurrentBookmark(true);
			if (!Object.ReferenceEquals(caretPosition, null))
				RestoreCaretPosition(caretPosition);
		}
	}
	public class DocumentLogIntervalEx : DocumentLogInterval {
		readonly PieceTable pieceTable;
		public DocumentLogIntervalEx(PieceTable pieceTable, DocumentLogPosition start, int length)
			: base(start, length) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
	}
}
