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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region TextRunBaseHistoryItem
	public abstract class TextRunBaseHistoryItem : ParagraphBaseHistoryItem {
		RunIndex runIndex = new RunIndex(-1);
		protected TextRunBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public RunIndex RunIndex { get { return runIndex; } set { runIndex = value; } }
	}
	#endregion
	#region TextRunInsertedBaseHistoryItem
	public abstract class TextRunInsertedBaseHistoryItem : TextRunBaseHistoryItem {
		int startIndex = -1;
		bool forceVisible;
		protected TextRunInsertedBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int StartIndex { get { return startIndex; } set { startIndex = value; } }
		public bool ForceVisible { get { return forceVisible; } set { forceVisible = value; } }
		protected internal virtual void ApplyFormattingToNewRun(TextRunCollection runs, TextRunBase run) {
			RunIndex formattingRunIndex = CalculateFormattingRunIndex(runs);
			if (formattingRunIndex < new RunIndex(runs.Count)) {
				TextRunBase runWithFormatting = runs[formattingRunIndex];
				run.InheritStyleAndFormattingFromCore(runWithFormatting, forceVisible);
			}
		}
		protected internal RunIndex CalculateFormattingRunIndex(TextRunCollection runs) {
			RunIndex prevRunIndex = RunIndex - 1;
			RunIndex nextRunIndex = RunIndex + 1;
			for (; ; ) {
				if (prevRunIndex < new RunIndex(0) || runs[prevRunIndex] is ParagraphRun) {
					if (runs[nextRunIndex] is SeparatorTextRun)
						nextRunIndex++;
					else
						return nextRunIndex;
				}
				else {
					if (runs[prevRunIndex] is SeparatorTextRun)
						prevRunIndex--;
					else
						return prevRunIndex;
				}
			}
		}
	}
	#endregion
	public class BookmarkInfo {
		readonly BookmarkBase bookmark;
		DocumentModelPosition startPosition;
		DocumentModelPosition endPosition;
		public BookmarkInfo(BookmarkBase bookmark) {
			Guard.ArgumentNotNull(bookmark, "bookmark");
			this.bookmark = bookmark;
		}
		public BookmarkBase Bookmark { get { return bookmark; } }
		public DocumentModelPosition StartPosition { get { return startPosition; } set { startPosition = value; } }
		public DocumentModelPosition EndPosition { get { return endPosition; } set { endPosition = value; } }
	}
	#region TextRunsDeletedHistoryItem
	public class TextRunsDeletedHistoryItem : TextRunBaseHistoryItem {
		#region Fields
		int deletedRunCount = -1;
		List<TextRunBase> deletedRuns;
		int deltaLength;
		List<BookmarkInfo> affectedBookmarks;
		List<BookmarkInfo> affectedRangePermissions;
		List<BookmarkInfo> affectedComments;
		List<int> notificationIds;
		#endregion
		public TextRunsDeletedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int DeletedRunCount { get { return deletedRunCount; } set { deletedRunCount = value; } }
		#endregion       
		protected override void RedoCore() {
			DocumentModel.ResetMerging();
			TextRunCollection runs = PieceTable.Runs;
			RunIndex count = RunIndex + DeletedRunCount;
			bool saveUndoRedoInfo = !DocumentModel.ModelForExport;
			if (saveUndoRedoInfo) {
				affectedBookmarks = CalculateAffectedBookmarkList(PieceTable.Bookmarks);
				affectedRangePermissions = CalculateAffectedBookmarkList(PieceTable.RangePermissions);
				affectedComments = CalculateAffectedBookmarkList(PieceTable.Comments);
				deletedRuns = new List<TextRunBase>(DeletedRunCount);
				deltaLength = 0;
				if (notificationIds == null) {
					notificationIds = new List<int>(DeletedRunCount);
					for (int i = 0; i < DeletedRunCount; i++)
						notificationIds.Add(DocumentModel.History.GetNotificationId());
				}
			}
			for (RunIndex i = count - 1; i >= RunIndex; i--) {
				int length = runs[i].Length;
				if (saveUndoRedoInfo) {
					deletedRuns.Add(runs[i]);
					deltaLength += length;
				}
				runs[i].BeforeRunRemoved();
				runs.RemoveAt(i);
				DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, i, length, notificationIds != null ? notificationIds[i - RunIndex] : NotificationIdGenerator.EmptyId);
			}
		}
		protected override void UndoCore() {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex count = RunIndex + DeletedRunCount;
			for (RunIndex i = RunIndex; i < count; i++) {
				TextRunBase run = deletedRuns[count - i - 1];
				runs.Insert(i, run);
				DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, i, run.Length, notificationIds[i - RunIndex]);
				runs[i].AfterRunInserted();
			}
			RepairBookmarks(affectedBookmarks);
			RepairBookmarks(affectedRangePermissions);
			RepairBookmarks(affectedComments);
		}
		void RepairBookmarks(List<BookmarkInfo> affectedBookmarks) {
			int count = affectedBookmarks.Count;
			for (int i = 0; i < count; i++) {
				BookmarkInfo info = affectedBookmarks[i];
				if (info.StartPosition != null)
					info.Bookmark.Interval.Start.CopyFrom(info.StartPosition);
				if (info.EndPosition != null)
					info.Bookmark.Interval.End.CopyFrom(info.EndPosition);
			}
		}
		List<BookmarkInfo> CalculateAffectedBookmarkList<T>(BookmarkBaseCollection<T> bookmarks) where T : BookmarkBase, IDocumentModelStructureChangedListener {
			List<BookmarkInfo> result = new List<BookmarkInfo>();
			RunIndex startDeletedIndex = RunIndex;
			RunIndex endDeletedIndex = startDeletedIndex + deletedRunCount - 1;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				BookmarkBase bookmark = bookmarks[i];
				BookmarkInfo info = null;
				if (IsBookmarkStartPositionAffected(bookmark, startDeletedIndex, endDeletedIndex)) {
					info = new BookmarkInfo(bookmark);
					info.StartPosition = bookmark.Interval.Start.Clone();
				}
				if (IsBookmarkEndPositionAffected(bookmark, startDeletedIndex, endDeletedIndex)) {
					if (info == null)
						info = new BookmarkInfo(bookmark);
					info.EndPosition = bookmark.Interval.End.Clone();
				}
				if (info != null)
					result.Add(info);
			}
			return result;
		}
		bool IsBookmarkStartPositionAffected(BookmarkBase bookmark, RunIndex startRunIndex, RunIndex endRunIndex) {
			RunIndex startBookmarkIndex = bookmark.Interval.Start.RunIndex;
			if (startRunIndex <= startBookmarkIndex && endRunIndex >= startBookmarkIndex)
				return true;
			return false;
		}
		bool IsBookmarkEndPositionAffected(BookmarkBase bookmark, RunIndex startRunIndex, RunIndex endRunIndex) {
			RunIndex endBookmarkIndex = bookmark.Interval.End.RunIndex;
			if (startRunIndex <= endBookmarkIndex && endRunIndex >= endBookmarkIndex)
				return true;
			return false;
		}
	}
	#endregion
	#region TextRunInsertedHistoryItem
	public class TextRunInsertedHistoryItem : TextRunInsertedBaseHistoryItem {
		int newLength = -1;
		int notificationId;
		public TextRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int NewLength { get { return newLength; } set { newLength = value; } }
		void AfterInsertRun(DocumentModel documentModel, TextRunBase run, RunIndex runIndex) {
			LastInsertedRunInfo lastInsertedRunInfo = PieceTable.LastInsertedRunInfo;
			lastInsertedRunInfo.Run = (TextRun)run;
			lastInsertedRunInfo.HistoryItem = this;
			lastInsertedRunInfo.RunIndex = runIndex;
		}
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, NewLength, notificationId);
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged, RunIndex.DontCare, RunIndex.DontCare);
		}
		protected override void RedoCore() {
			Debug.Assert(NewLength > 0);
			TextRunCollection runs = PieceTable.Runs;
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			TextRun run = CreateTextRun(paragraph);
			runs.Insert(RunIndex, run);
			ApplyFormattingToNewRun(runs, run);
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, NewLength, notificationId);
			AfterInsertRun(DocumentModel, run, RunIndex);
			runs[RunIndex].AfterRunInserted();
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged, RunIndex.DontCare, RunIndex.DontCare);
		}
		protected virtual TextRun CreateTextRun(Paragraph paragraph) {
			return new TextRun(paragraph, StartIndex, NewLength);
		}
	}
	#endregion
	#region LayoutDependentTextRunInsertedHistoryItem
	public class LayoutDependentTextRunInsertedHistoryItem : TextRunInsertedHistoryItem {
		FieldResultFormatting fieldResultFormatting;
		public LayoutDependentTextRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public FieldResultFormatting FieldResultFormatting { get { return fieldResultFormatting; } set { fieldResultFormatting = value; } }
		protected override TextRun CreateTextRun(Paragraph paragraph) {
			LayoutDependentTextRun result = CreateTextRunCore(paragraph);
			result.FieldResultFormatting = fieldResultFormatting;
			return result;
		}
		protected virtual LayoutDependentTextRun CreateTextRunCore(Paragraph paragraph) {
			return new LayoutDependentTextRun(paragraph, StartIndex, NewLength);
		}
	}
	#endregion
	#region FootNoteRunInsertedHistoryItemBase<T> (abstract class)
	public abstract class FootNoteRunInsertedHistoryItemBase<T> : LayoutDependentTextRunInsertedHistoryItem where T : FootNoteBase<T> {
		int noteIndex;
		protected FootNoteRunInsertedHistoryItemBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int NoteIndex { get { return noteIndex; } set { noteIndex = value; } }
		protected override void UndoCore() {
			FootNoteBase<T> note = GetNote(NoteIndex);
			Debug.Assert(note != null);
			note.ReferenceRun = null;
			base.UndoCore();
		}
		protected override TextRun CreateTextRun(Paragraph paragraph) {
			FootNoteRunBase<T> result = (FootNoteRunBase<T>)base.CreateTextRun(paragraph);
			result.NoteIndex = NoteIndex;
			if (!PieceTable.IsNote) {
				FootNoteBase<T> note = GetNote(NoteIndex);
				if (note != null)
					note.ReferenceRun = result;
			}
			return result;
		}
		protected internal abstract FootNoteBase<T> GetNote(int noteIndex);
	}
	#endregion
	#region FootNoteRunInsertedHistoryItem
	public class FootNoteRunInsertedHistoryItem : FootNoteRunInsertedHistoryItemBase<FootNote> {
		public FootNoteRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override LayoutDependentTextRun CreateTextRunCore(Paragraph paragraph) {
			return new FootNoteRun(paragraph, StartIndex, NewLength);
		}
		protected internal override FootNoteBase<FootNote> GetNote(int noteIndex) {
			if (noteIndex < 0)
				return null;
			return DocumentModel.FootNotes[noteIndex];
		}
	}
	#endregion
	#region EndNoteRunInsertedHistoryItem
	public class EndNoteRunInsertedHistoryItem : FootNoteRunInsertedHistoryItemBase<EndNote> {
		public EndNoteRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override LayoutDependentTextRun CreateTextRunCore(Paragraph paragraph) {
			return new EndNoteRun(paragraph, StartIndex, NewLength);
		}
		protected internal override FootNoteBase<EndNote> GetNote(int noteIndex) {
			if (noteIndex < 0)
				return null;
			return DocumentModel.EndNotes[noteIndex];
		}
	}
	#endregion
	#region TextRunSplitHistoryItem
	public class TextRunSplitHistoryItem : TextRunBaseHistoryItem {
		int splitOffset = -1;
		public TextRunSplitHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int SplitOffset { get { return splitOffset; } set { splitOffset = value; } }
		protected override void UndoCore() {
			TextRunCollection runs = PieceTable.Runs;
			TextRunBase run = runs[RunIndex];
			TextRunBase tailRun = runs[RunIndex + 1];
			run.Length += tailRun.Length;
			runs.RemoveAt(RunIndex + 1);
			DocumentModelStructureChangedNotifier.NotifyRunJoined(PieceTable, PieceTable, ParagraphIndex, RunIndex, SplitOffset, tailRun.Length);
			PieceTable.ApplyChanges(DocumentModelChangeType.JoinRun, RunIndex, RunIndex);
			DocumentModel.ResetMerging();
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			TextRun run = (TextRun)runs[RunIndex];
			TextRun tailRun = run.CreateRun(run.Paragraph, run.StartIndex + SplitOffset, run.Length - SplitOffset);
			runs.Insert(RunIndex + 1, tailRun);
			run.Length = SplitOffset;
			tailRun.InheritStyleAndFormattingFromCore(run, false);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(PieceTable, PieceTable, ParagraphIndex, RunIndex, SplitOffset);
			PieceTable.ApplyChanges(DocumentModelChangeType.SplitRun, RunIndex, RunIndex + 1);
			DocumentModel.ResetMerging();
		}
	}
	#endregion
	#region TextRunsJoinedHistoryItem
	public class TextRunsJoinedHistoryItem : TextRunBaseHistoryItem {
		int splitOffset = -1;
		public TextRunsJoinedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void UndoCore() {
			TextRunCollection runs = PieceTable.Runs;
			TextRun run = (TextRun)runs[RunIndex];
			TextRun tailRun = new TextRun(run.Paragraph, run.StartIndex + splitOffset, run.Length - splitOffset);
			runs.Insert(RunIndex + 1, tailRun);
			run.Length = splitOffset;
			tailRun.InheritStyleAndFormattingFromCore(run, false);
			DocumentModelStructureChangedNotifier.NotifyRunSplit(PieceTable, PieceTable, ParagraphIndex, RunIndex, splitOffset);
			PieceTable.ApplyChanges(DocumentModelChangeType.SplitRun, RunIndex, RunIndex + 1);
			DocumentModel.ResetMerging();
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			TextRun run = (TextRun)runs[RunIndex];
			TextRun nextRun = (TextRun)runs[RunIndex + 1];
			this.splitOffset = run.Length;
#if DEBUGTEST
			Debug.Assert(run.CanJoinWith(nextRun));
#endif
			runs.RemoveAt(RunIndex + 1);
			run.Length += nextRun.Length;
			DocumentModelStructureChangedNotifier.NotifyRunJoined(PieceTable, PieceTable, ParagraphIndex, RunIndex, splitOffset, nextRun.Length);
			PieceTable.ApplyChanges(DocumentModelChangeType.JoinRun, RunIndex, RunIndex);
			DocumentModel.ResetMerging();
		}
	}
	#endregion
	#region TextRunAppendTextHistoryItem
	public class TextRunAppendTextHistoryItem : TextRunBaseHistoryItem {
		#region Fields
		int textLength;
		DocumentLogPosition logPosition;
		TextRunInsertedHistoryItem historyItem;
		#endregion
		public TextRunAppendTextHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int TextLength { get { return textLength; } set { textLength = value; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } set { logPosition = value; } }
		#endregion
		protected override void UndoCore() {
			LastInsertedRunInfo lastInsertedRunInfo = PieceTable.LastInsertedRunInfo;
			lastInsertedRunInfo.RunIndex = RunIndex;
			lastInsertedRunInfo.Run = (TextRun)PieceTable.Runs[RunIndex];
			lastInsertedRunInfo.HistoryItem = historyItem;
			lastInsertedRunInfo.LogPosition = logPosition;
			lastInsertedRunInfo.Run.Length -= TextLength;
			lastInsertedRunInfo.HistoryItem.NewLength -= TextLength;
			DocumentModel.History.RaiseOperationCompleted(); 
			DocumentModelStructureChangedNotifier.NotifyRunUnmerged(PieceTable, PieceTable, ParagraphIndex, RunIndex, -TextLength);
		}
		protected override void RedoCore() {
			LastInsertedRunInfo lastInsertedRunInfo = PieceTable.LastInsertedRunInfo;
			Debug.Assert(lastInsertedRunInfo != null);
			Debug.Assert(lastInsertedRunInfo.Run != null);
			Debug.Assert(lastInsertedRunInfo.HistoryItem != null);
			Debug.Assert(lastInsertedRunInfo.RunIndex == RunIndex);
			this.historyItem = lastInsertedRunInfo.HistoryItem;
			lastInsertedRunInfo.LogPosition = LogPosition + TextLength;
			lastInsertedRunInfo.Run.Length += TextLength;
			lastInsertedRunInfo.HistoryItem.NewLength += TextLength;
			DocumentModel.History.RaiseOperationCompleted(); 
			DocumentModelStructureChangedNotifier.NotifyRunMerged(PieceTable, PieceTable, ParagraphIndex, RunIndex, TextLength);
		}
	}
	#endregion
	#region ChangeCharacterStyleIndexHistoryItem
	public class ChangeCharacterStyleIndexHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int oldIndex;
		readonly int newIndex;
		readonly RunIndex runIndex;
		#endregion
		public ChangeCharacterStyleIndexHistoryItem(PieceTable pieceTable, RunIndex runIndex, int oldStyleIndex, int newStyleIndex)
			: base(pieceTable) {
			this.oldIndex = oldStyleIndex;
			this.newIndex = newStyleIndex;
			this.runIndex = runIndex;
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public RunIndex RunIndex { get { return runIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].SetCharacterStyleIndexCore(OldIndex);
		}
		protected override void RedoCore() {
			PieceTable.Runs[RunIndex].SetCharacterStyleIndexCore(NewIndex);
		}
	}
	#endregion
	#region TextRunChangeCharacterStyleHistoryItem
	public class TextRunChangeCharacterStyleHistoryItem : TextRunBaseHistoryItem {
		#region Fields
		int oldStyleIndex = -1;
		int newStyleIndex = -1;
		#endregion
		public TextRunChangeCharacterStyleHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public int OldStyleIndex { get { return oldStyleIndex; } set { oldStyleIndex = value; } }
		public int NewStyleIndex { get { return newStyleIndex; } set { newStyleIndex = value; } }
		#endregion
		protected override void UndoCore() {
			TextRunBase run = PieceTable.Runs[RunIndex];
			run.CharacterStyleIndex = OldStyleIndex;
		}
		protected override void RedoCore() {
			TextRunBase run = PieceTable.Runs[RunIndex];
			run.CharacterStyleIndex = NewStyleIndex;
		}
	}
	#endregion
	#region TextRunChangeCaseHistoryItem (abstract class)
	public abstract class TextRunChangeCaseHistoryItem : RichEditHistoryItem {
		readonly RunIndex runIndex;
		string originalText;
		const DocumentModelChangeActions changeActions = DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseModifiedChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout;
		protected TextRunChangeCaseHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable) {
			this.runIndex = runIndex;
		}
		public RunIndex RunIndex { get { return runIndex; } }
		public override void Execute() {
			TextRun run = (TextRun)PieceTable.Runs[RunIndex];
			this.originalText = run.GetPlainText(PieceTable.TextBuffer);
			base.Execute();
		}
		protected override void UndoCore() {
			TextRunBase run = PieceTable.Runs[RunIndex];
			Debug.Assert(run.Length == originalText.Length);
			ChunkedStringBuilder textBuffer = PieceTable.TextBuffer;
			int count = originalText.Length;
			for (int i = 0; i < count; i++)
				textBuffer[i + run.StartIndex] = originalText[i];
			PieceTable.ApplyChangesCore(changeActions, RunIndex, RunIndex);
		}
		protected override void RedoCore() {
			ChunkedStringBuilder textBuffer = PieceTable.TextBuffer;
			TextRunBase run = PieceTable.Runs[RunIndex];
			bool allCaps = run.AllCaps;
			for (int i = run.StartIndex + run.Length - 1; i >= run.StartIndex; i--)
				textBuffer[i] = ChangeCase(textBuffer[i], allCaps);
			ApplyChangeActions();
		}
		protected void ApplyChangeActions() {
			PieceTable.ApplyChangesCore(changeActions, RunIndex, RunIndex);
		}
		protected internal abstract char ChangeCase(char value, bool allCaps);
	}
	#endregion
	#region TextRunMakeUpperCaseHistoryItem
	public class TextRunMakeUpperCaseHistoryItem : TextRunChangeCaseHistoryItem {
		public TextRunMakeUpperCaseHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected internal override char ChangeCase(char value, bool allCaps) {
			return Char.ToUpper(value);
		}
	}
	#endregion
	#region TextRunMakeLowerCaseHistoryItem
	public class TextRunMakeLowerCaseHistoryItem : TextRunChangeCaseHistoryItem {
		public TextRunMakeLowerCaseHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected internal override char ChangeCase(char value, bool allCaps) {
			return Char.ToLower(value);
		}
	}
	#endregion
	#region TextRunToggleCaseHistoryItem
	public class TextRunToggleCaseHistoryItem : TextRunChangeCaseHistoryItem {
		public TextRunToggleCaseHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected internal override char ChangeCase(char value, bool allCaps) {
			if (allCaps) {
				if (Char.IsUpper(value))
					return Char.ToLower(value);
			}
			else {
				if (Char.IsLower(value))
					return Char.ToUpper(value);
				if (Char.IsUpper(value))
					return Char.ToLower(value);
			}
			return value;
		}
	}
	#endregion
	#region TextRunCapitalizeEachWordCaseHistoryItem
	public class TextRunCapitalizeEachWordCaseHistoryItem : TextRunChangeCaseHistoryItem {
		public TextRunCapitalizeEachWordCaseHistoryItem(PieceTable pieceTable, RunIndex runIndex)
			: base(pieceTable, runIndex) {
		}
		protected override void RedoCore() {
			ChunkedStringBuilder textBuffer = PieceTable.TextBuffer;
			TextRunBase run = PieceTable.Runs[RunIndex];
			bool allCaps = run.AllCaps;
			for (int i = run.StartIndex + run.Length - 1; i > run.StartIndex; i--)
				textBuffer[i] = ChangeCase(textBuffer[i], textBuffer[i - 1]);
			textBuffer[run.StartIndex] = ChangeCase(textBuffer[run.StartIndex], allCaps);
			ApplyChangeActions();
		}
		char ChangeCase(char current, char next) {
			return next == Characters.Space ? Char.ToUpper(current) : Char.ToLower(current);
		}
		protected internal override char ChangeCase(char value, bool allCaps) {
			if (RunIndex == RunIndex.Zero)
				return Char.ToUpper(value);
			TextRun prevRun = PieceTable.Runs[RunIndex - 1] as TextRun;
			if (prevRun == null)
				return Char.ToUpper(value);
			string prevText = prevRun.GetPlainText(PieceTable.TextBuffer);
			if (prevText[prevText.Length - 1] == Characters.Space)
				return Char.ToUpper(value);
			return Char.ToLower(value);
		}
	}
	#endregion
	#region PositionsDocumentModelIterator
	public class PositionsDocumentModelIterator {
		PieceTable pieceTable;
		public PositionsDocumentModelIterator(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		protected void UpdateModelPositionByLogPosition(DocumentModelPosition currentPosition) {
			Paragraph paragraph = PieceTable.Paragraphs[currentPosition.ParagraphIndex];
			DocumentLogPosition targetPosition = currentPosition.LogPosition;
			if (targetPosition >= currentPosition.RunStartLogPosition && targetPosition <= currentPosition.RunEndLogPosition)
				return;
			DocumentLogPosition logPosition = currentPosition.RunEndLogPosition + 1;
			currentPosition.RunIndex++;
			while (logPosition <= paragraph.EndLogPosition) {
				DocumentLogPosition nextRunStart = logPosition + PieceTable.Runs[currentPosition.RunIndex].Length;
				if (nextRunStart > targetPosition)
					break;
				logPosition = nextRunStart;
				currentPosition.RunIndex++;
				currentPosition.RunStartLogPosition = nextRunStart;
			}
			if (logPosition > paragraph.EndLogPosition) {
				currentPosition.ParagraphIndex++;
				paragraph = PieceTable.Paragraphs[currentPosition.ParagraphIndex];
			}
			while (targetPosition > paragraph.EndLogPosition) {
				logPosition += paragraph.Length;
				currentPosition.RunIndex += paragraph.LastRunIndex - paragraph.FirstRunIndex + 1;
				currentPosition.ParagraphIndex++;
				paragraph = PieceTable.Paragraphs[currentPosition.ParagraphIndex];				
			}			
			DocumentLogPosition runStart = logPosition;
			while (targetPosition >= runStart + PieceTable.Runs[currentPosition.RunIndex].Length) {
				runStart = runStart + PieceTable.Runs[currentPosition.RunIndex].Length;
				currentPosition.RunIndex++;
			}
			currentPosition.RunStartLogPosition = runStart;
		}
		protected internal void MoveForwardCore(DocumentModelPosition pos) {			
			UpdateModelPositionByLogPosition(pos);
		}
	}
	#endregion
	public class RunSplitLocation {
		readonly RunIndex runIndex;
		readonly ParagraphIndex paragraphIndex;
		readonly int splitOffset;
		public RunSplitLocation(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			this.paragraphIndex = paragraphIndex;
			this.runIndex = runIndex;
			this.splitOffset = splitOffset;
		}
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public RunIndex RunIndex { get { return runIndex; } }
		public int SplitOffset { get { return splitOffset; } }
	}
	public class MultipleTextRunSplitHistoryItem : RichEditHistoryItem {
		List<DocumentLogPosition> positions;
		List<RunSplitLocation> runSplitLocations;
		public MultipleTextRunSplitHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public List<DocumentLogPosition> Positions { get { return positions; } set { positions = value; } }
		protected override void UndoCore() {
			int count = runSplitLocations.Count;
			for (int i = count - 1; i >= 0; i--) {
				RunSplitLocation runSplit = runSplitLocations[i];
				RunIndex runIndex = runSplit.RunIndex;
				ParagraphIndex paragraphIndex = runSplit.ParagraphIndex;
				int splitOffset = runSplit.SplitOffset;
				TextRunCollection runs = PieceTable.Runs;
				TextRunBase run = runs[runIndex];
				TextRunBase tailRun = runs[runIndex + 1];
				run.Length += tailRun.Length;
				runs.RemoveAt(runIndex + 1);
				DocumentModelStructureChangedNotifier.NotifyRunJoined(PieceTable, PieceTable, paragraphIndex, runIndex, splitOffset, tailRun.Length);
				PieceTable.ApplyChanges(DocumentModelChangeType.JoinRun, runIndex, runIndex);
				DocumentModel.ResetMerging();
			}
		}
		protected override void RedoCore() {
			runSplitLocations = new List<RunSplitLocation>();
			List<RunIndex> targetPositions = new List<RunIndex>();
			List<TextRunBase> insertedRuns = new List<TextRunBase>();
			PositionsDocumentModelIterator iterator = new PositionsDocumentModelIterator(PieceTable);
			DocumentModelPosition position = new DocumentModelPosition(PieceTable);
			int count = positions.Count;
			DocumentModelStructureChangedNotifier.NotifyBeginMultipleRunSplit(PieceTable, PieceTable);
			int initialLength = PieceTable.DocumentEndLogPosition - DocumentLogPosition.Zero + 1;
			for (int i = 0; i < count; i++) {
				position.LogPosition = positions[i];
				iterator.MoveForwardCore(position); 
				if (position.RunOffset == 0)
					continue;
				SplitCore(targetPositions, insertedRuns, position.ParagraphIndex, position.RunIndex, position.RunOffset);
				int totalLength = 0;
				for (int k = 0; k < PieceTable.Runs.Count; k++)
					totalLength += PieceTable.Runs[new RunIndex(k)].Length;
				for (int k = 0; k < insertedRuns.Count; k++)
					totalLength += insertedRuns[k].Length;
				if (totalLength != initialLength) {
				}
				runSplitLocations.Add(new RunSplitLocation(position.ParagraphIndex, position.RunIndex + insertedRuns.Count - 1, position.RunOffset));				
				position.RunStartLogPosition = position.LogPosition;
			}
			InsertDeferredRuns(targetPositions, insertedRuns);
			PieceTable.ApplyChanges(DocumentModelChangeType.SplitRun, RunIndex.MinValue, RunIndex.MaxValue);
			PieceTable.ApplyChangesCore(CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.CharacterStyle), RunIndex.MinValue, RunIndex.MinValue);
			DocumentModelStructureChangedNotifier.NotifyEndMultipleRunSplit(PieceTable, PieceTable);
		}
		protected virtual void InsertDeferredRuns(List<RunIndex> targetPositions, List<TextRunBase> insertedRuns) {
			TextRunCollection runs = PieceTable.Runs;
			int count = runs.Count;
			int insertedRunCount = insertedRuns.Count;
			if (insertedRunCount == 0)
				return;
			int newRunCount = insertedRuns.Count + count;			
			for (int i = 0; i < insertedRunCount; i++)
				runs.Add(null);
			int delta = insertedRunCount;			
			for (RunIndex j = new RunIndex(newRunCount - 1); j >= RunIndex.Zero; j--) {
				if (j - delta + 1 == targetPositions[delta - 1]) {
					runs[j] = insertedRuns[delta - 1];
					delta--;
					if (delta == 0)
						break;
				}
				else
					runs[j] = runs[j - delta];
			}
#if DEBUGTEST || DEBUG
			for (RunIndex k = new RunIndex(newRunCount - 1); k >= RunIndex.Zero; k--)
				Debug.Assert(runs[k] != null);
#endif
		}
		protected virtual void SplitCore(List<RunIndex> targetPositions, List<TextRunBase> insertedRuns, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			TextRunCollection runs = PieceTable.Runs;
			TextRun run = (TextRun)runs[runIndex];			
			TextRun tailRun = run.CreateRun(run.Paragraph, run.StartIndex + splitOffset, run.Length - splitOffset);
			targetPositions.Add(runIndex);
			insertedRuns.Add(runs[runIndex]);
			runs[runIndex] = tailRun;
			run.Length = splitOffset;
			tailRun.CharacterProperties.BeginInit();
			try {
				tailRun.CharacterProperties.SuppressDirectNotifications(); 
				tailRun.CharacterProperties.SuppressIndexRecalculationOnEndInit(); 
				tailRun.InheritStyleAndFormattingFromCore(run, false, true);
			}
			finally {
				tailRun.CharacterProperties.EndInit();
			}
			DocumentModelStructureChangedNotifier.NotifyRunSplit(PieceTable, PieceTable, paragraphIndex, runIndex, splitOffset);
			DocumentModel.ResetMerging();
		}
	}
}
