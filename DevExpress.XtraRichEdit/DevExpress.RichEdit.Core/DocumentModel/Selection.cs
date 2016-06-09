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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Model {
	#region Old selection
	#endregion
	#region SelectionRangeCollection
	public class SelectionRangeCollection : List<SelectionRange> {
		public SelectionRangeCollection() {
		}
		internal SelectionRangeCollection(DocumentLogPosition from, int length) {
			Add(new SelectionRange(from, length));
		}
		#region First
		public SelectionRange First {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return null;
				else
					return this[0];
			}
		}
		#endregion
		#region Last
		public SelectionRange Last {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return null;
				else
					return this[Count - 1];
			}
		}
		#endregion
	}
	#endregion
	#region SelectionRange
	public class SelectionRange : IComparable<SelectionRange> {
		DocumentLogPosition from;
		int length;
		public SelectionRange(DocumentLogPosition from, int length) {
			this.from = from;
			this.length = length;
		}
		public DocumentLogPosition From { get { return from; } set { from = value; } }
		public int Length { get { return length; } set { length = value; } }
		public DocumentLogPosition Start { get { return From; } }
		public DocumentLogPosition End { get { return From + Length; } }
		#region IComparable<SelectionRange> Members
		public int CompareTo(SelectionRange other) {
			return (int)(From - other.From);
		}
		#endregion
	}
	#endregion
	#region SelectionRangeComparable
	public class SelectionRangeComparable : IComparer<SelectionRange> {
		#region IComparer<SelectionRange> Members
		public int Compare(SelectionRange x, SelectionRange y) {
			int xValue = ((IConvertToInt<DocumentLogPosition>)x.From).ToInt();
			int yValue = ((IConvertToInt<DocumentLogPosition>)y.From).ToInt();
			return Comparer<int>.Default.Compare(xValue, yValue);
		}
		#endregion
	}
	#endregion
	public class SelectionPersistentInfo {
		DocumentLogPosition start;
		public DocumentLogPosition Start { get { return start; } set { start = value; } }
	}
	public class Selection : IBatchUpdateable, IBatchUpdateHandler, IDocumentModelStructureChangedListener {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		PieceTable pieceTable;
		List<SelectionItem> items;
		ISelectedTableStructureBase tableSelectionStucture;
		int selectionGeneration;
		bool activeSelectionChanged;
		#endregion
		public Selection(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.items = new List<SelectionItem>();
			SelectionItem selectionItem = InitialSelection(pieceTable);
			selectionItem.Changed += OnSelectionChanged;
			selectionItem.IsSelectionInTable = false;
			selectionItem.Generation = 0;
			this.items.Add(selectionItem);
			tableSelectionStucture = new SelectedCellsCollection();
		}
		#region Properties
		public virtual DocumentLogPosition Start {
			get { return ActiveSelection.Start; }
			set {
				ActiveSelection.Start = value;
				TryAndMergeSelectionStart(value);
			}
		}
		public virtual DocumentLogPosition End {
			get { return ActiveSelection.End; }
			set {
				ActiveSelection.End = value;
				TryAndMergeSelectionEnd(value);
			}
		}
		public virtual void SetInterval(DocumentLogPosition start, DocumentLogPosition end) {
			ActiveSelection.Start = start;
			ActiveSelection.End = end;
			TryMergeByActiveSelection();
		}
		protected internal RunInfo Interval { get { return ActiveSelection.Interval; } }
		public DocumentLogPosition NormalizedStart { get { return Interval.NormalizedStart.LogPosition; } }
		public DocumentLogPosition NormalizedEnd { get { return Interval.NormalizedEnd.LogPosition; } }
		public DocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		internal SelectionItem ActiveSelection { get { return items[items.Count - 1]; } }
		internal List<SelectionItem> Items { get { return items; } }
		internal SelectionItem First { get { return items[0]; } }
		public ISelectedTableStructureBase SelectedCells { get { return tableSelectionStucture; } set { tableSelectionStucture = value; } }
		public int Length { get { return ActiveSelection.Length; } }
		protected internal DocumentLogPosition VirtualEnd { get { return ActiveSelection.VirtualEnd; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public int SelectionGeneration { get { return selectionGeneration; } set { selectionGeneration = value; } }
		public bool UsePreviousBoxBounds {
			get { return ActiveSelection.UsePreviousBoxBounds; }
			set {
				if (ActiveSelection.UsePreviousBoxBounds == value)
					return;
				ActiveSelection.UsePreviousBoxBounds = value;
			}
		}
		protected internal DocumentLogPosition NormalizedVirtualEnd {
			get {
				if (Length != 0)
					return NormalizedEnd - 1;
				return NormalizedEnd;
			}
		}
		protected internal virtual bool IsMultiSelection { get { return Items.Count > 1; } }
		public bool IsSelectionChanged {
			get {
				int count = Items.Count;
				for (int i = 0; i < count; i++)
					if (Items[i].IsChanged)
						return true;
				return false;
			}
			set {
				int count = Items.Count;
				for (int i = 0; i < count; i++)
					Items[i].IsChanged = value;
			}
		}
		#endregion
		public virtual SelectionPersistentInfo GetSelectionPersistentInfo() {
			SelectionPersistentInfo result = new SelectionPersistentInfo();
			result.Start = Start;
			return result;
		}
		public virtual void RestoreSelection(SelectionPersistentInfo info) {
			BeginUpdate();
			try {
				ClearMultiSelection();
				Start = info.Start;
				End = info.Start;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual SelectionItem InitialSelection(PieceTable pieceTable) {
			return new SelectionItem(pieceTable);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			ActiveSelection.BeginUpdate();
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			ActiveSelection.EndUpdate();
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			ActiveSelection.CancelUpdate();
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		internal void TryMergeByActiveSelection() {
			TryAndMergeSelectionStart(ActiveSelection.Start);
			TryAndMergeSelectionEnd(ActiveSelection.End);
		}
		void TryAndMergeSelectionEnd(DocumentLogPosition value) {
			for (int i = items.Count - 2; i >= 0; i--) {
				SelectionItem currentItem = Items[i];
				if (value > currentItem.NormalizedStart && value < currentItem.NormalizedEnd) {
					items.Remove(currentItem);
				}
			}
		}
		void TryAndMergeSelectionStart(DocumentLogPosition value) {
			for (int i = items.Count - 2; i >= 0; i--) {
				SelectionItem currentItem = Items[i];
				if (ActiveSelection.NormalizedStart <= currentItem.NormalizedStart && ActiveSelection.NormalizedEnd >= currentItem.NormalizedEnd) {
					items.Remove(currentItem);
				}
			}
		}
		internal void AddSelection(SelectionItem newSelection) {
			newSelection.Changed += OnSelectionChanged;
			items.Add(newSelection);
			this.activeSelectionChanged = true;
		}
		protected internal virtual void BeginMultiSelection(PieceTable activePiecetable) {
			SelectionItem newSelection = new SelectionItem(activePiecetable);
			AddSelection(newSelection);
			selectionGeneration++;
			newSelection.Generation = selectionGeneration;
		}
		internal void ClearMultiSelection() {
			ClearMultiSelection(0);
		}
		internal void ClearMultiSelection(int clearFrom) {
			int itemsCount = items.Count;
			int removeCount = itemsCount - clearFrom - 1;
			if (removeCount > 0) {
				for (int i = clearFrom; i < removeCount; i++)
					items[i].Changed -= OnSelectionChanged;
				items.RemoveRange(clearFrom, removeCount);
			}
			clearFrom = items.Count - 1;
			items[clearFrom].IsSelectionInTable = false;
			items[clearFrom].IsCovered = false;
			if (removeCount > 0)
				this.activeSelectionChanged = true;
			this.selectionGeneration = 0;
		}
		internal void ClearOutdatedItems() {
			int start = items.Count - 1;
			for (int i = start; i >= 0; i--) {
				SelectionItem item = items[i];
				if (item.Length == 0 && items.Count > 1) {
					this.activeSelectionChanged = true;
					items[i].Changed -= OnSelectionChanged;
					items.RemoveAt(i);
				}
			}
		}
		protected internal virtual void ClearSelectionInTable() {
			for (int i = items.Count - 2; i >= 0; i--) {
				SelectionItem item = items[i];
				if (item.IsSelectionInTable && item.Generation == SelectionGeneration) {
					item.Changed -= OnSelectionChanged;
					items.RemoveAt(i);
				}
			}
		}
		internal virtual bool IsWholeSelectionInOneTable() {
			return (SelectedCells is SelectedCellsCollection && SelectedCells.IsNotEmpty);
		}
		internal virtual bool IsFloatingObjectSelected() {
			return Length == 1 && PieceTable.Runs[Interval.NormalizedStart.RunIndex] is FloatingObjectAnchorRun;
		}
		internal virtual bool IsInlinePictureSelected() {
			return Length == 1 && PieceTable.Runs[Interval.NormalizedStart.RunIndex] is InlinePictureRun;
		}
		protected internal bool IsSelectionInTable() {
			for (DocumentLogPosition i = NormalizedStart; i <= NormalizedEnd; i++)
				if (PieceTable.FindParagraph(i).IsInCell())
					return true;
			return false;
		}
		protected internal virtual void OnSelectionChanged(object sender, EventArgs e) {
			if (onChanged != null)
				onChanged(this, e);
		}
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.activeSelectionChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (this.activeSelectionChanged)
				ActiveSelection.OnChangedCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			if (this.activeSelectionChanged)
				ActiveSelection.OnChangedCore();
		}
		#endregion
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnBeginMultipleRunSplit();
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnEndMultipleRunSplit();
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnFieldInserted(fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnFieldRemoved(fieldIndex);
		}
		#endregion
		#region OnParagraphInserted
		protected internal virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			}
		}
		#endregion
		#region OnParagraphRemoved
		protected internal virtual void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
		}
		#endregion
		#region OnParagraphMerged
		protected internal virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			}
		}
		#endregion
		#region OnRunInserted
		protected internal virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
			}
		}
		#endregion
		#region OnRunRemoved
		protected internal virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
			}
		}
		#endregion
		int multipleRunSplitCount;
		protected internal virtual void OnBeginMultipleRunSplit() {
			multipleRunSplitCount++;
		}
		protected internal virtual void OnEndMultipleRunSplit() {
			multipleRunSplitCount--;
			if (multipleRunSplitCount == 0) {
				int selectionsCount = Items.Count;
				for (int index = 0; index < selectionsCount; index++) {
					Items[index].UpdateStartPosition();
					Items[index].UpdateEndPosition();
				}
			}
		}
		#region OnRunSplit
		protected internal virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (multipleRunSplitCount > 0)
				return;
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunSplit(paragraphIndex, runIndex, splitOffset);
			}
		}
		#endregion
		#region OnRunJoined
		protected internal virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			}
		}
		#endregion
		#region OnRunMerged
		protected internal virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
			}
		}
		#endregion
		#region OnRunUnmerged
		protected internal virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
			}
		}
		#endregion
		#region OnFieldInserted
		protected internal virtual void OnFieldInserted(int fieldIndex) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnFieldInserted(fieldIndex);
			}
		}
		#endregion
		#region OnFieldRemoved
		protected internal virtual void OnFieldRemoved(int fieldIndex) {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].OnFieldRemoved(fieldIndex);
			}
		}
		#endregion
		protected internal virtual void UpdateStartPosition() {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].UpdateStartPosition();
			}
		}
		protected internal virtual void UpdateEndPosition() {
			int selectionsCount = Items.Count;
			for (int index = 0; index < selectionsCount; index++) {
				Items[index].UpdateEndPosition();
			}
		}
		protected internal virtual SelectionRangeCollection GetSelectionCollection() {
			SelectionRangeCollection result = new SelectionRangeCollection();
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				SelectionItem item = Items[i];
				result.Add(new SelectionRange(item.NormalizedStart, item.Length));
			}
			return result;
		}
		internal SelectionRangeCollection GetSortedSelectionCollection() {
			SelectionRangeCollection result = GetSelectionCollection();
			result.Sort(new SelectionRangeComparable());
			return result;
		}
		protected internal virtual bool IsSelectFieldPictureResult() {
			if (Length != 1)
				return false;
			if (Items.Count != 1)
				return false;
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = Items[0].Interval.NormalizedStart.RunIndex;
			RunIndex prevIndex = runIndex - 1;
			RunIndex nextIndex = runIndex + 1;
			if (prevIndex < RunIndex.Zero || nextIndex >= new RunIndex(runs.Count))
				return false;
			return runs[runIndex] is InlinePictureRun && runs[prevIndex] is FieldCodeEndRun && runs[nextIndex] is FieldResultEndRun;
		}		
		protected internal virtual void SetStartCell(DocumentLogPosition logPosition) {
			TableStructureBySelectionCalculator selectionCalculator = new TableStructureBySelectionCalculator(PieceTable);
			tableSelectionStucture = selectionCalculator.SetStartCell(logPosition);
		}
		protected internal virtual void ManualySetTableSelectionStructureAndChangeSelection(TableCell startCell, TableCell endCell) {
			ManualySetTableSelectionStructureAndChangeSelection(startCell, endCell, false);
		}
		protected internal virtual void ManualySetTableSelectionStructureAndChangeSelection(TableCell startCell, TableCell endCell, bool isColumnSelected) {
			TableStructureBySelectionCalculator selectionCalculator = new TableStructureBySelectionCalculator(PieceTable);
			SelectedCellsCollection result = selectionCalculator.Calculate(startCell, endCell, isColumnSelected);
			result.SetOriginalStartLogPosition(SelectedCells.OriginalStartLogPosition);
			UpdateSelectionBy(result);
			SelectedCells = result;
		}
		ParagraphIndex GetEndParagraphIndex(DocumentLogPosition logPosition) {
			return PieceTable.FindParagraphIndex(logPosition);
		}
		protected internal virtual void UpdateTableSelectionStart(DocumentLogPosition logPosition) {
			TableCell startCell = SelectedCells.FirstSelectedCell;
			if (startCell == null)
				return;
			TableCell endCell = PieceTable.FindParagraph(logPosition).GetCell();
			if (FirstCellIsParentCellForSecondCellsTable(endCell, startCell) || endCell == null) {
				int nestedLevel = endCell != null ? endCell.Table.NestedLevel + 1 : 0;
				TableCell cell = PieceTable.TableCellsManager.GetCellByNestingLevel(startCell.StartParagraphIndex, nestedLevel);
				Start = GetStartPositionInTableRow(cell.Row);
				return;
			}
			TableCell mostParentForFirst = PieceTable.TableCellsManager.GetCellByNestingLevel(startCell.StartParagraphIndex, 0);
			TableCell mostParentForSecond = PieceTable.TableCellsManager.GetCellByNestingLevel(endCell.StartParagraphIndex, 0);
			if (!Object.ReferenceEquals(mostParentForFirst.Table, mostParentForSecond.Table)) {
				Start = GetStartPositionInTableRow(mostParentForFirst.Row);
			}
		}
		DocumentLogPosition GetStartPositionInTableRow(TableRow row) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			return (Start < End) ?
				paragraphs[row.FirstCell.StartParagraphIndex].LogPosition
				: paragraphs[row.LastCell.EndParagraphIndex].EndLogPosition + 1;
		}
		protected internal virtual void UpdateTableSelectionEnd(DocumentLogPosition logPosition) {
			UpdateTableSelectionEnd(logPosition, false);
		}
		protected internal virtual void UpdateTableSelectionEnd(DocumentLogPosition logPosition, bool considerCellStart) {
			DocumentLogPosition selectionStart = CalculateSelectionStart(logPosition);
			TableCell startCell = SelectedCells.FirstSelectedCell;
			TableCell endCell = DetermineEndCellByLogPosition(selectionStart, logPosition, considerCellStart);
			if (startCell != null && Object.ReferenceEquals(startCell, endCell) && !IsCellSelected(startCell, selectionStart, End)) {
				SelectedCells = new SelectedCellsCollection(SelectedCells.FirstSelectedCell, SelectedCells.OriginalStartLogPosition);
				Start = selectionStart;
				ClearSelectionInTable();
				return;
			}
			if (endCell == null || FirstCellIsParentCellForSecondCellsTable(endCell, startCell)) {
				if (SelectedCells is SelectedCellsCollection)
					SelectedCells = new StartSelectedCellInTable(SelectedCells);
				return;
			}
			bool paragraphBeforeTable = false;
			TextRunCollection runs = DocumentModel.ActivePieceTable.Runs;
			RunIndex runIndex = Interval.End.RunIndex;
			if (runIndex > new RunIndex(0) && (runs[runIndex - 1].Paragraph.GetCell() == null && runs[runIndex].Paragraph.GetCell() != null) && IsSelectionChanged) 
				paragraphBeforeTable = true;
			if (!SelectedCells.IsNotEmpty || FirstCellIsParentCellForSecondCellsTable(startCell, endCell)) {
				if (SelectedCells is SelectedCellsCollection)
					SelectedCells = new StartSelectedCellInTable(SelectedCells);
				if (!paragraphBeforeTable) {
					int startCellNestedLevel = GetActualNestedLevelConsiderStartCell(startCell);
					End = GetEndPositionInTableRow(logPosition, endCell, startCellNestedLevel);
				}
				return;
			}
			if (startCell != null && endCell != null && startCell.Table == endCell.Table) {
				ManualySetTableSelectionStructureAndChangeSelection(startCell, endCell);
				return;
			}
			if (FirstAndSecondCellHaveCommonTableButSecondCellNotParentForFirstCell(startCell, endCell)) {
				NormalizeTableCellsToMinNestedLevel(ref startCell, ref endCell);
				if (startCell.Table == endCell.Table) {
					ManualySetTableSelectionStructureAndChangeSelection(startCell, endCell);
					return;
				}
				TableCell parentCellForFirst = startCell.Table.ParentCell;
				TableCell parentCellForSecond = endCell.Table.ParentCell;
				while (parentCellForFirst != null && parentCellForSecond != null) {
					if (Object.ReferenceEquals(parentCellForFirst.Table, parentCellForSecond.Table)) {
						ManualySetTableSelectionStructureAndChangeSelection(parentCellForFirst, parentCellForSecond);
						return;
					}
					parentCellForFirst = parentCellForFirst.Table.ParentCell;
					parentCellForSecond = parentCellForSecond.Table.ParentCell;
				}
				return;
			}
			else if (!Object.ReferenceEquals(startCell.Table, endCell.Table)) {
				if (SelectedCells is SelectedCellsCollection)
					SelectedCells = new StartSelectedCellInTable(SelectedCells);
				if (!paragraphBeforeTable)
					End = GetEndPositionInTableRow(logPosition, endCell, 0);
				return;
			}
			else {
				Start = SelectedCells.OriginalStartLogPosition;
			}
		}
		DocumentLogPosition CalculateSelectionStart(DocumentLogPosition logPosition) {
			DocumentLogPosition actualStart = Items[0].Start;
			TableCell firstSelectedCell = SelectedCells.FirstSelectedCell;
			if (firstSelectedCell != null && SelectedCells.SelectedOnlyOneCell) {
				DocumentLogPosition cellStartPos = PieceTable.Paragraphs[firstSelectedCell.StartParagraphIndex].LogPosition;
				DocumentLogPosition cellEndPos = PieceTable.Paragraphs[firstSelectedCell.EndParagraphIndex].EndLogPosition + 1;
				DocumentLogPosition originalStart = SelectedCells.OriginalStartLogPosition;
				if (originalStart >= cellStartPos && originalStart < cellEndPos)
					return originalStart;
				if (actualStart > logPosition && actualStart == cellEndPos)
					actualStart--;
			}
			return actualStart;
		}
		TableCell DetermineStartSelectedCell(DocumentLogPosition selectionStart) {
			ParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(selectionStart);
			TableCell result = PieceTable.Paragraphs[paragraphIndex].GetCell();
			return (result != null) ? result : SelectedCells.FirstSelectedCell;
		}
		bool IsCellSelectedFully(TableCell tableCell, DocumentLogPosition selectionStart, DocumentLogPosition selectionEnd) {
			DocumentLogPosition normalizeStart = Algorithms.Min(selectionStart, selectionEnd);
			DocumentLogPosition normalizeEnd = Algorithms.Max(selectionStart, selectionEnd);
			DocumentLogPosition cellStartPos = PieceTable.Paragraphs[tableCell.StartParagraphIndex].LogPosition;
			DocumentLogPosition cellEndPos = PieceTable.Paragraphs[tableCell.EndParagraphIndex].EndLogPosition + 1;
			return normalizeStart <= cellStartPos && normalizeEnd >= cellEndPos;
		}
		DocumentLogPosition GetCellEndPosition(TableCell startCell) {
			if (Start <= End) {
				DocumentLogPosition cellEndPos = PieceTable.Paragraphs[startCell.EndParagraphIndex].EndLogPosition;
				return Algorithms.Min(cellEndPos, End);
			}
			else {
				DocumentLogPosition cellStartPos = PieceTable.Paragraphs[startCell.StartParagraphIndex].LogPosition;
				return Algorithms.Max(cellStartPos, End);
			}
		}
		TableCell DetermineEndCellByLogPosition(DocumentLogPosition selectionStart, DocumentLogPosition selectionEnd, bool considerCellStart) {
			TableCell firstSelectedCell = SelectedCells.FirstSelectedCell;
			if (selectionStart == selectionEnd)
				return firstSelectedCell;
			TableCell nextSelectedCell = DetermineEndCellCore(selectionStart, selectionEnd);
			if (nextSelectedCell == null)
				return null;
			if (Object.ReferenceEquals(firstSelectedCell, nextSelectedCell))
				return firstSelectedCell;
			TableCell prevSelectedCell = GetLastSelectedCell();
			if (prevSelectedCell == null || IsRowsSelected(prevSelectedCell, nextSelectedCell))
				return nextSelectedCell;
			nextSelectedCell = EnsureCellIsSelected(selectionStart, selectionEnd, nextSelectedCell, prevSelectedCell, considerCellStart);
			if (IsColumnsSelected(prevSelectedCell, nextSelectedCell))
				return nextSelectedCell;
			int prevSelectedColumnIndex = prevSelectedCell.GetStartColumnIndexConsiderRowGrid();
			int nextSelectedColumnIndex = nextSelectedCell.GetStartColumnIndexConsiderRowGrid();
			int columnIndex;
			if (selectionStart < selectionEnd)
				columnIndex = Math.Max(nextSelectedColumnIndex, prevSelectedColumnIndex);
			else
				columnIndex = Math.Min(nextSelectedColumnIndex, prevSelectedColumnIndex);
			return nextSelectedCell.Table.GetCell(nextSelectedCell.Row, columnIndex);
		}
		TableCell DetermineEndCellCore(DocumentLogPosition selectionStart, DocumentLogPosition selectionEnd) {
			ParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(selectionEnd);
			Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex];
			TableCell cell = paragraph.GetCell();
			if (cell != null)
				return cell;
			if (selectionStart > selectionEnd || selectionEnd > paragraph.LogPosition)
				return null;
			paragraphIndex--;
			if (paragraphIndex < ParagraphIndex.Zero)
				return null;
			return PieceTable.Paragraphs[paragraphIndex].GetCell();
		}
		TableCell EnsureCellIsSelected(DocumentLogPosition selectionStart, DocumentLogPosition selectionEnd, TableCell nextSelectedCell, TableCell prevSelectedCell, bool considerCellStart) {
			DocumentLogPosition normalizeEnd = Algorithms.Max(selectionStart, selectionEnd);
			DocumentLogPosition cellStartPos = PieceTable.Paragraphs[nextSelectedCell.StartParagraphIndex].LogPosition;
			bool selectedCellChanged = !Object.ReferenceEquals(nextSelectedCell, prevSelectedCell);
			bool cellShouldBeUnselected = !selectedCellChanged && !IsCellSelectedFully(nextSelectedCell, selectionStart, selectionEnd);
			bool cellIsNotSelected = (considerCellStart) ? normalizeEnd < cellStartPos : normalizeEnd <= cellStartPos;
			bool isLeftToRightDirection = selectionStart <= selectionEnd;
			if (cellShouldBeUnselected || cellIsNotSelected) {
				TableCell previousCell = GetPreviousTableCell(nextSelectedCell, isLeftToRightDirection);
				if (previousCell != null)
					return previousCell;
			}
			return nextSelectedCell;
		}
		bool IsCellSelected(TableCell cell, DocumentLogPosition selectionStart, DocumentLogPosition selectionEnd) {
			DocumentLogPosition normalizeStart = Algorithms.Min(selectionStart, selectionEnd);
			DocumentLogPosition normalizeEnd = Algorithms.Max(selectionStart, selectionEnd);
			DocumentLogPosition cellEndPos = PieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition + 1;
			return normalizeStart < cellEndPos && normalizeEnd >= cellEndPos;
		}
		TableCell GetPreviousTableCell(TableCell cell, bool isLeftToRightDirection) {
			ParagraphIndex paragraphIndex;
			if (isLeftToRightDirection) {
				paragraphIndex = cell.StartParagraphIndex - 1;
				if (paragraphIndex < ParagraphIndex.Zero)
					return null;
			}
			else {
				paragraphIndex = cell.EndParagraphIndex + 1;
				if (paragraphIndex > new ParagraphIndex(PieceTable.Paragraphs.Count - 1))
					return null;
			}
			return PieceTable.Paragraphs[paragraphIndex].GetCell();
		}
		bool IsColumnsSelected(TableCell prevSelectedCell, TableCell nextSelectedCell) {
			return nextSelectedCell.Row == prevSelectedCell.Row;
		}
		TableCell GetLastSelectedCell() {
			SelectedCellsCollection selectedCells = SelectedCells as SelectedCellsCollection;
			return (selectedCells != null && selectedCells.IsNotEmpty) ? selectedCells.Last.EndCell : SelectedCells.FirstSelectedCell;
		}
		bool IsRowsSelected(TableCell prevSelectedCell, TableCell nextSelectedCell) {
			int prevSelectedColumnIndex = prevSelectedCell.GetStartColumnIndexConsiderRowGrid();
			int nextSelectedColumnIndex = nextSelectedCell.GetStartColumnIndexConsiderRowGrid();
			return !Object.ReferenceEquals(prevSelectedCell, nextSelectedCell) && prevSelectedColumnIndex == nextSelectedColumnIndex;
		}
		int GetActualNestedLevelConsiderStartCell(TableCell startCell) {
			if (SelectedCells.IsNotEmpty)
				return startCell.Table.NestedLevel + 1;
			return 0;
		}
		internal bool FirstAndSecondCellHaveCommonTableButSecondCellNotParentForFirstCell(TableCell first, TableCell second) {
			NormalizeTableCellsToMinNestedLevel(ref first, ref second);
			if (Object.ReferenceEquals(first, second))
				return false;
			if (Object.ReferenceEquals(first.Table, second.Table))
				return true;
			TableCell parentCellForFirst = first.Table.ParentCell;
			TableCell parentCellForSecond = second.Table.ParentCell;
			while (parentCellForFirst != null && parentCellForSecond != null) {
				if (Object.ReferenceEquals(parentCellForFirst.Table, parentCellForSecond.Table))
					return true;
				parentCellForFirst = parentCellForFirst.Table.ParentCell;
				parentCellForSecond = parentCellForSecond.Table.ParentCell;
			}
			return false;
		}
		void NormalizeTableCellsToMinNestedLevel(ref TableCell startCell, ref TableCell endCell) {
			int startCellNestedLevel = startCell.Table.NestedLevel;
			int endCellNestedLevel = endCell.Table.NestedLevel;
			if (startCellNestedLevel == endCellNestedLevel)
				return;
			TableCellsManager tableCellManager = PieceTable.TableCellsManager;
			if (startCellNestedLevel < endCellNestedLevel)
				endCell = tableCellManager.GetCellByNestingLevel(endCell.StartParagraphIndex, startCellNestedLevel);
			else
				startCell = tableCellManager.GetCellByNestingLevel(startCell.StartParagraphIndex, endCellNestedLevel);
		}
		internal bool FirstCellIsParentCellForSecondCellsTable(TableCell firstCell, TableCell secondCell) {
			if (firstCell == null || secondCell == null || secondCell.Table.ParentCell == null)
				return false;
			Table parentTable = secondCell.Table;
			while (parentTable.ParentCell != null) {
				if (Object.ReferenceEquals(parentTable.ParentCell, firstCell))
					return true;
				parentTable = parentTable.ParentCell.Table;
			}
			return false;
		}
		DocumentLogPosition GetEndPositionInTableRow(DocumentLogPosition initialEnd, TableCell cell, int nestedLevel) {
			TableRow row = cell.Row;
			while (row.Table.NestedLevel > nestedLevel)
				row = row.Table.ParentCell.Row;
			TableCell firstCell = GetFirstNonCoveredByVerticalMergingCell(row);
			TableCell lastCell = GetLastNonCoveredByVerticalMergingCell(row);
			ParagraphIndex startParagraphIndexInRow = firstCell.StartParagraphIndex;
			ParagraphIndex endParagraphIndexInRow = lastCell.EndParagraphIndex;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			DocumentLogPosition startLogPos = paragraphs[startParagraphIndexInRow].LogPosition;
			DocumentLogPosition endLogPos = paragraphs[endParagraphIndexInRow].EndLogPosition + 1;
			return (initialEnd < Start) ?
				Algorithms.Min<DocumentLogPosition>(startLogPos, endLogPos) :
				Algorithms.Max<DocumentLogPosition>(startLogPos, endLogPos);
		}
		TableCell GetLastNonCoveredByVerticalMergingCell(TableRow row) {
			int rowCellsCount = row.Cells.Count;
			TableCell result = row.Cells.Last;
			for (int cellIndex = rowCellsCount - 1; cellIndex >= 0 && result.VerticalMerging == MergingState.Continue; cellIndex--)
				result = row.Cells[cellIndex];
			return result;
		}
		TableCell GetFirstNonCoveredByVerticalMergingCell(TableRow row) {
			int rowCellsCount = row.Cells.Count;
			TableCell result = row.Cells.First;
			for (int cellIndex = 0; cellIndex < rowCellsCount && result.VerticalMerging == MergingState.Continue; cellIndex++)
				result = row.Cells[cellIndex];
			return result;
		}
		protected internal virtual void UpdateSelectionBy(SelectedCellsCollection selectedCells) {
			ClearSelectionInTable();
			bool isLeftToRightDirection = IsLeftToRightDirection(selectedCells);
			int selectedRowsCount = selectedCells.RowsCount;
			for (int i = 0; i < selectedRowsCount; i++) {
				if (i >= Items.Count)
					AddSelection(new SelectionItem(PieceTable));
				UpdateInTableSelectionItem(ActiveSelection, selectedCells[i], isLeftToRightDirection);
			}
			RemoveIntersectedSelectionItems(selectedRowsCount);
		}
		bool IsLeftToRightDirection(SelectedCellsCollection selectedCells) {
			if (selectedCells.SelectedOnlyOneCell)
				return Start <= End;
			int firstRowIndex = selectedCells.First.Row.IndexInTable;
			int lastRowIndex = selectedCells.Last.Row.IndexInTable;
			if (firstRowIndex != lastRowIndex)
				return firstRowIndex < lastRowIndex;
			else
				return selectedCells.First.StartCellIndex <= selectedCells.First.EndCellIndex;
		}
		#region UpdateInTableSelectionItem
		void UpdateInTableSelectionItem(SelectionItem item, SelectedCellsIntervalInRow currentCellsInterval, bool isLeftToRightDirection) {
			item.IsSelectionInTable = true;
			item.Generation = DocumentModel.Selection.SelectionGeneration;
			if (isLeftToRightDirection) {
				ParagraphIndex start = currentCellsInterval.StartCell.StartParagraphIndex;
				ParagraphIndex end = currentCellsInterval.EndCell.EndParagraphIndex;
				item.Start = PieceTable.Paragraphs[start].LogPosition;
				item.End = PieceTable.Paragraphs[end].EndLogPosition + 1;
			}
			else {
				ParagraphIndex start = currentCellsInterval.StartCell.EndParagraphIndex;
				ParagraphIndex end = currentCellsInterval.EndCell.StartParagraphIndex;
				item.Start = PieceTable.Paragraphs[start].EndLogPosition + 1;
				item.End = PieceTable.Paragraphs[end].LogPosition;
			}
		}
		#endregion
		#region RemoveIntersectedSelectionItems
		protected internal virtual void RemoveIntersectedSelectionItems(int lastInsertedItemsCount) {
			if (Items.Count == 0)
				return;
			List<int> generationsToRemove = new List<int>();
			int lastItemsGeneration = SelectionGeneration;
			List<SelectionItem> lastAddedItems = new List<SelectionItem>();
			List<SelectionItem> previouslyAdded = items.GetRange(0, items.Count - lastInsertedItemsCount);
			lastAddedItems = items.GetRange(Items.Count - lastInsertedItemsCount, lastInsertedItemsCount);
			int lastAddedItemsCount = lastAddedItems.Count;
			for (int lastAddedItemIndex = 0; lastAddedItemIndex < lastAddedItemsCount; lastAddedItemIndex++) {
				SelectionItem item = lastAddedItems[lastAddedItemIndex];
				int previouslyAddedCount = previouslyAdded.Count;
				for (int i = 0; i < previouslyAddedCount; i++) {
					SelectionItem currentItem = previouslyAdded[i];
					int lastGensIndex = generationsToRemove.Count - 1;
					if (generationsToRemove.Count > 0 && generationsToRemove[lastGensIndex] == currentItem.Generation)
						continue;
#if (DEBUGTEST||DEBUG)
					System.Diagnostics.Debug.Assert(currentItem.Generation != lastItemsGeneration);
#endif
					DocumentLogPosition itemNormalizedEnd = item.NormalizedEnd;
					DocumentLogPosition currentItemNormalizedStart = currentItem.NormalizedStart;
					DocumentLogPosition itemNormalizedStart = item.NormalizedStart;
					DocumentLogPosition currentItemNormalizedEnd = currentItem.NormalizedEnd;
					bool isItemNormEndInCurrentInterval = itemNormalizedEnd > currentItemNormalizedStart && itemNormalizedEnd < currentItemNormalizedEnd;
					bool isItemNormStartInCurrentInterval = itemNormalizedStart > currentItemNormalizedStart && itemNormalizedStart < currentItemNormalizedEnd;
					bool isCurrentIntervalBelongsItem = itemNormalizedStart <= currentItemNormalizedStart && itemNormalizedEnd >= currentItemNormalizedEnd;
					if (isItemNormEndInCurrentInterval || isItemNormStartInCurrentInterval || isCurrentIntervalBelongsItem) {
						if (!generationsToRemove.Contains(currentItem.Generation))
							generationsToRemove.Add(currentItem.Generation);
					}
				}
			}
			if (generationsToRemove.Count == 0)
				return;
			for (int i = items.Count - lastInsertedItemsCount - 1; i >= 0; i--) {
				SelectionItem item = Items[i];
				if (generationsToRemove.Contains(item.Generation)) {
					item.Changed -= OnSelectionChanged;
					items.RemoveAt(i);
				}
			}
		}
		#endregion
		public List<TableRow> GetSelectedTableRows() {
			if (!IsWholeSelectionInOneTable())
				return new List<TableRow>();
			SelectedCellsCollection cells = (SelectedCellsCollection)SelectedCells;
			return cells.GetSelectedTableRows();
		}
		public ParagraphList GetSelectedParagraphs() {
			ParagraphList result = new ParagraphList();
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			List<SelectionItem> items = Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				ParagraphIndex start = item.GetStartParagraphIndex();
				ParagraphIndex end = item.GetEndParagraphIndex();
				for (ParagraphIndex paragraphIndex = start; paragraphIndex <= end; paragraphIndex++)
					result.Add(paragraphs[paragraphIndex]);
			}
			return result;
		}
	}
	#region SelectionItem
	public class SelectionItem : ChangableDocumentInterval, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		readonly BatchUpdateHelper batchUpdateHelper;
		bool changed;
		bool usePreviousBoxBounds;
		bool isSelectionInTable;
		int generation;
		bool isCovered;
		int leftOffset;
		int rightOffset;
		#endregion
		public SelectionItem(PieceTable pieceTable)
			: base(pieceTable) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region Properties
		protected internal bool IsSelectionInTable { get { return isSelectionInTable; } set { isSelectionInTable = value; } }
		public int Generation { get { return generation; } set { generation = value; } }
		protected internal bool IsCovered { get { return isCovered; } set { isCovered = value; } }
		protected internal int LeftOffset { get { return leftOffset; } set { leftOffset = value; } }
		protected internal int RightOffset { get { return rightOffset; } set { rightOffset = value; } }
		public bool IsChanged { get { return changed; } set { changed = value; } }
		public bool UsePreviousBoxBounds {
			get { return usePreviousBoxBounds; }
			set {
				if (usePreviousBoxBounds == value)
					return;
				usePreviousBoxBounds = value;
			}
		}
		protected internal DocumentLogPosition VirtualEnd {
			get {
				if (UsePreviousBoxBounds)
					return Algorithms.Max(DocumentLogPosition.Zero, End - 1);
				else
					return End;
			}
		}
		public override DocumentLogPosition Start {
			get {
				return base.Start;
			}
			set {
				base.Start = value;
				ResetOffsets();
			}
		}
		public override DocumentLogPosition End {
			get {
				return base.End;
			}
			set {
				base.End = value;
				ResetOffsets();
			}
		}
		void ResetOffsets() {
			leftOffset = 0;
			rightOffset = 0;
		}
		#endregion
		#region Events
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.changed = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (changed)
				OnChangedCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			if (changed)
				OnChangedCore();
		}
		#endregion
		protected internal override void OnChanged(bool startChanged, bool endChanged) {
			if (startChanged || endChanged) {
				this.usePreviousBoxBounds = false;
				OnChangedCore();
			}
		}
		protected internal override void OnChangedCore() {
			if (IsUpdateLocked)
				changed = true;
			else
				RaiseChanged();
		}
		public ParagraphIndex GetStartParagraphIndex() {
			return Interval.NormalizedStart.ParagraphIndex;
		}
		public ParagraphIndex GetEndParagraphIndex() {
			if (NormalizedEnd > new DocumentLogPosition(0) && NormalizedEnd != NormalizedStart)
				return PieceTable.FindParagraphIndex(NormalizedEnd - 1, false);
			else
				return PieceTable.FindParagraphIndex(NormalizedEnd, false);
		}
		public DocumentModelPosition CalculateStartPosition(bool allowSelectionExpanding) {
			DocumentModelPosition result;
			if (Start < End)
				result = Interval.Start;
			else if (Start > End)
				result = Interval.End;
			else {
				DocumentModelPosition start = Interval.Start;
				if (allowSelectionExpanding) {
					WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(PieceTable);
					if (!iterator.IsInsideWord(start) || iterator.IsNewElement(start))
						result = start;
					else
						result = iterator.MoveBack(start);
				}
				else
					result = start;
			}
			return result;
		}
		public DocumentModelPosition CalculateEndPosition(bool allowSelectionExpanding) {
			DocumentModelPosition result;
			if (Start < End)
				result = Interval.End;
			else if (Start > End)
				result = Interval.Start;
			else {
				if (allowSelectionExpanding) {
					WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(PieceTable);
					if (!iterator.IsInsideWord(Interval.End) || iterator.IsNewElement(Interval.End))
						result = Interval.End;
					else
						result = iterator.MoveForward(Interval.End);
				}
				else
					result = Interval.Start;
			}
			return result;
		}
	}
	#endregion
	#region EmptySelection
	public class EmptySelection : Selection {
		public EmptySelection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override SelectionItem InitialSelection(PieceTable pieceTable) {
			return new EmptySelectionItem(pieceTable);
		}
	}
	#endregion
	#region EmptySelectionItem
	public class EmptySelectionItem : SelectionItem {
		public EmptySelectionItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override DocumentLogPosition Start { get { return DocumentLogPosition.Zero; }  }
		public override DocumentLogPosition End { get { return DocumentLogPosition.Zero; }  }
		protected internal override void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
		}
		protected internal override void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected internal override void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
		}
		protected internal override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
		}
		protected internal override void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
		}
		protected internal override void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
		}
		protected internal override void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
		}
		protected internal override void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
		}
		protected internal override void RaiseChanged() {
		}
		protected internal override void UpdateEndPosition() {
		}
		protected internal override void UpdateStartPosition() {
		}
	}
	#endregion
}
