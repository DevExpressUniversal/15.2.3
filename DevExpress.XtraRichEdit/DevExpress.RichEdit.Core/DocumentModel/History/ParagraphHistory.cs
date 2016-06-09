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
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model.History;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region ParagraphBaseHistoryItem (abstract class)
	public abstract class ParagraphBaseHistoryItem : RichEditHistoryItem {
		ParagraphIndex paragraphIndex = new ParagraphIndex(-1);
		SectionIndex sectionIndex = new SectionIndex(-1);
		int tableCellIndex;
		int tableRowIndex;
		int tableIndex;
		protected ParagraphBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } set { paragraphIndex = value; } }
		public SectionIndex SectionIndex { get { return sectionIndex; } set { sectionIndex = value; } }
		int TableCellIndex { get { return tableCellIndex; } }
		int TableIndex { get { return tableIndex; } }
		Table Table { get { return (TableIndex >= 0) ? PieceTable.Tables[TableIndex] : null; } }
		protected TableCell Cell { get { return (tableRowIndex >= 0 && tableCellIndex >= 0) ? Table.Rows[tableRowIndex].Cells[TableCellIndex] : null; } }
		public void SetTableCell(TableCell cell) {
			if (cell == null) {
				this.tableCellIndex = -1;
				this.tableRowIndex = -1;
				this.tableCellIndex = -1;
				return;
			}
			Table table = cell.Table;
			this.tableIndex = table.PieceTable.Tables.IndexOf(table);
			TableRow row = cell.Row;
			this.tableRowIndex = table.Rows.IndexOf(row);
			this.tableCellIndex = row.Cells.IndexOf(cell);
		}
	}
	#endregion
	public class ParagraphsDeletedHistoryItem : ParagraphBaseHistoryItem {
		int deletedParagraphsCount = -1;
		List<Paragraph> deletedParagraphs;
		List<int> notificationIds;
		public ParagraphsDeletedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int DeletedParagraphsCount { get { return deletedParagraphsCount; } set { deletedParagraphsCount = value; } }
		protected override void RedoCore() {
			deletedParagraphs = new List<Paragraph>();
			if (notificationIds == null) {
				notificationIds = new List<int>(DeletedParagraphsCount);
				for (int i = 0; i < DeletedParagraphsCount; i++)
					notificationIds.Add(DocumentModel.History.GetNotificationId());
			} 
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			ParagraphIndex count = ParagraphIndex + DeletedParagraphsCount;
			for (ParagraphIndex i = count - 1; i >= ParagraphIndex; i--) {
				Paragraph paragraph = paragraphs[i];
				deletedParagraphs.Add(paragraph);
				RunIndex firstRunIndex = paragraph.FirstRunIndex;
				RunIndex lastRunIndex = paragraph.LastRunIndex;
				DocumentLogPosition logPosition = paragraph.LogPosition;
				DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(PieceTable, PieceTable, SectionIndex, i, firstRunIndex, notificationIds[i - ParagraphIndex]);				
				paragraphs.RemoveAt(i);
				paragraph.AfterRemove(firstRunIndex, lastRunIndex, logPosition);
				NumberingListNotifier.NotifyParagraphRemoved(DocumentModel, paragraph.GetOwnNumberingListIndex());
			}
		}
		protected override void UndoCore() {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			ParagraphIndex count = ParagraphIndex + DeletedParagraphsCount;
			for (ParagraphIndex i = ParagraphIndex; i < count; i++) {
				Paragraph paragraph = deletedParagraphs[count - i - 1];
				paragraphs.Insert(i, paragraph);
				paragraph.AfterUndoRemove();
				paragraphs.CheckTree(paragraph);
				DocumentModelStructureChangedNotifier.NotifyParagraphInserted(PieceTable, PieceTable, SectionIndex, i, paragraphs[i].FirstRunIndex, Cell, false, i, notificationIds[i - ParagraphIndex]);
				NumberingListNotifier.NotifyParagraphAdded(DocumentModel, paragraph.GetOwnNumberingListIndex());
			}
#if DEBUGTEST || DEBUG
			RunIndex prevParagraphStart = RunIndex.Zero;
			foreach (Paragraph paragraph in PieceTable.Paragraphs) {
				if (paragraph.FirstRunIndex < prevParagraphStart)
					Exceptions.ThrowInternalException();
				prevParagraphStart = paragraph.FirstRunIndex;
			}
#endif
		}
	}
	public abstract class ParagraphListHistoryItemBase : ParagraphBaseHistoryItem {
		NumberingListIndex numberingListIndex;
		int listLevelIndex;
		protected ParagraphListHistoryItemBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public NumberingListIndex NumberingListIndex { get { return numberingListIndex; } set { numberingListIndex = value; } }
		public int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
	}
	public class AddParagraphToListHistoryItem : ParagraphListHistoryItemBase {
		NumberingListIndex oldOwnNumberingListIndex = NumberingListIndex.ListIndexNotSetted;
		public AddParagraphToListHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		NumberingListIndex OldOwnNumberingListIndex {
			get { return oldOwnNumberingListIndex; }
			set { oldOwnNumberingListIndex = value; }
		}
		protected override void RedoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			OldOwnNumberingListIndex = paragraph.GetOwnNumberingListIndex();
			paragraph.SetNumberingListIndex(NumberingListIndex);
			paragraph.SetListLevelIndex(ListLevelIndex);
			NumberingListNotifier.NotifyParagraphAdded(DocumentModel, NumberingListIndex);
			NumberingListNotifier.NotifyParagraphRemoved(DocumentModel, OldOwnNumberingListIndex);
		}
		protected override void UndoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			paragraph.ResetNumberingListIndex(OldOwnNumberingListIndex);
			paragraph.ResetListLevelIndex();
			NumberingListNotifier.NotifyParagraphRemoved(DocumentModel, NumberingListIndex);
			NumberingListNotifier.NotifyParagraphAdded(DocumentModel, OldOwnNumberingListIndex);
		}
	}
	public class RemoveParagraphFromListHistoryItem : ParagraphListHistoryItemBase {
		public RemoveParagraphFromListHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void RedoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			NumberingListIndex = paragraph.GetOwnNumberingListIndex();
			ListLevelIndex = paragraph.GetOwnListLevelIndex();
			NumberingListIndex newValue = NumberingListIndex.ListIndexNotSetted;
			if (!paragraph.IsInNonStyleList())
				newValue = NumberingListIndex.NoNumberingList;
			paragraph.ResetNumberingListIndex(newValue);
			paragraph.ResetListLevelIndex();
			NumberingListNotifier.NotifyParagraphRemoved(DocumentModel, NumberingListIndex);
		}
		protected override void UndoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			paragraph.SetNumberingListIndex(NumberingListIndex);
			paragraph.SetListLevelIndex(ListLevelIndex);
			NumberingListNotifier.NotifyParagraphAdded(DocumentModel, NumberingListIndex);
		}
	}
	#region ChangeParagraphListLevelHistoryItem 
	public class ChangeParagraphListLevelHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int oldIndex;
		readonly int newIndex;
		readonly ParagraphIndex paragraphIndex;
		#endregion
		public ChangeParagraphListLevelHistoryItem(PieceTable pieceTable, ParagraphIndex paragraphIndex, int oldLevelIndex, int newLevelIndex)
			: base(pieceTable) {
			this.oldIndex = oldLevelIndex;
			this.newIndex = newLevelIndex;
			this.paragraphIndex = paragraphIndex;
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Paragraphs[ParagraphIndex].SetListLevelIndex(OldIndex);
		}
		protected override void RedoCore() {
			PieceTable.Paragraphs[ParagraphIndex].SetListLevelIndex(NewIndex);
		}
	}
	#endregion
	#region ChangeParagraphStyleIndexHistoryItem
	public class ChangeParagraphStyleIndexHistoryItem : RichEditHistoryItem {
		#region Fields
		readonly int oldIndex;
		readonly int newIndex;
		readonly ParagraphIndex paragraphIndex;
		#endregion
		public ChangeParagraphStyleIndexHistoryItem(PieceTable pieceTable, ParagraphIndex paragraphIndex, int oldStyleIndex, int newStyleIndex)
			: base(pieceTable) {
			this.oldIndex = oldStyleIndex;
			this.newIndex = newStyleIndex;
			this.paragraphIndex = paragraphIndex;
		}
		#region Properties
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		#endregion
		protected override void UndoCore() {
			PieceTable.Paragraphs[ParagraphIndex].SetParagraphStyleIndexCore(OldIndex);
		}
		protected override void RedoCore() {
			PieceTable.Paragraphs[ParagraphIndex].SetParagraphStyleIndexCore(NewIndex);
		}
	}
	#endregion
	#region ParagraphInsertedBaseHistoryItem
	public class ParagraphInsertedBaseHistoryItem : ParagraphBaseHistoryItem {
		#region Fields
		RunIndex paragraphMarkRunIndex = new RunIndex(-1);
		DocumentLogPosition logPosition = new DocumentLogPosition(-1);
		int notificationId;
		#endregion
		public ParagraphInsertedBaseHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public RunIndex ParagraphMarkRunIndex { get { return paragraphMarkRunIndex; } set { paragraphMarkRunIndex = value; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } set { logPosition = value; } }
		#endregion
		static void ChangeRangesParagraph(RunIndex runIndex, TextRunCollection runs, Paragraph oldParagraph, Paragraph newParagraph) {
			for (RunIndex i = runIndex; i >= new RunIndex(0); i--) {
				TextRunBase range = runs[i];
				if (range.Paragraph != oldParagraph)
					break;
				range.Paragraph = newParagraph;
			}
		}
		Paragraph newParagraph;
		public override void Execute() {
			this.newParagraph = new Paragraph(PieceTable);
			base.Execute();
		}
		protected override void UndoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex + 1];
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Debug.Assert(sectionIndex >= new SectionIndex(0) || (sectionIndex == SectionIndex.DontCare && (PieceTable.IsNote || PieceTable.IsTextBox || PieceTable.IsComment)));
			Paragraph newParagraph = PieceTable.Paragraphs[ParagraphIndex];
			RunIndex runIndex = paragraph.FirstRunIndex;
#if UseOldIndicies
			paragraph.FirstRunIndex = newParagraph.FirstRunIndex;
#endif
			paragraph.SetRelativeFirstRunIndex(newParagraph.FirstRunIndex);
			paragraph.Length += newParagraph.Length;
#if UseOldIndicies
			paragraph.LogPosition = newParagraph.LogPosition;
#endif
			paragraph.SetRelativeLogPosition(newParagraph.LogPosition);
			ChangeRangesParagraph(newParagraph.LastRunIndex, PieceTable.Runs, newParagraph, paragraph);
			DocumentModelStructureChangedNotifier.NotifyParagraphRemoved(PieceTable, PieceTable, sectionIndex, ParagraphIndex, runIndex, notificationId);
			PieceTable.Paragraphs.RemoveAt(ParagraphIndex);
		}
		protected override void RedoCore() {
			Paragraph paragraph = PieceTable.Paragraphs[ParagraphIndex];
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Debug.Assert(sectionIndex >= new SectionIndex(0) || (sectionIndex == SectionIndex.DontCare && (PieceTable.IsNote || PieceTable.IsTextBox || PieceTable.IsComment)));
			newParagraph.SetParagraphStyleIndexCore(paragraph.ParagraphStyleIndex);
			RunIndex paragraphLastRunIndex = paragraphMarkRunIndex;
#if UseOldIndicies
			newParagraph.LastRunIndex = paragraphLastRunIndex;
#endif
			RunIndex paragraphFirstRunIndex = paragraph.FirstRunIndex;
#if UseOldIndicies
			newParagraph.FirstRunIndex = paragraphFirstRunIndex;
#endif
			DocumentLogPosition paragraphLogPosition = paragraph.LogPosition;
#if UseOldIndicies
			newParagraph.LogPosition = paragraphLogPosition;
#endif
			newParagraph.Length = LogPosition - paragraphLogPosition + 1; 
#if UseOldIndicies
			paragraph.FirstRunIndex = paragraphMarkRunIndex + 1;
#endif
			paragraph.SetRelativeFirstRunIndex(paragraphMarkRunIndex + 1);
			paragraph.Length -= newParagraph.Length;
#if UseOldIndicies
			paragraph.LogPosition += newParagraph.Length;
#endif
			paragraph.ShiftLogPosition(newParagraph.Length);
			PieceTable.Paragraphs.Insert(ParagraphIndex, newParagraph);
			newParagraph.SetRelativeFirstRunIndex(paragraphFirstRunIndex);
			newParagraph.SetRelativeLastRunIndex(paragraphLastRunIndex);
			newParagraph.SetRelativeLogPosition(paragraphLogPosition);
			PieceTable.Paragraphs.CheckTree(newParagraph);
			ChangeRangesParagraph(paragraphLastRunIndex, PieceTable.Runs, paragraph, newParagraph);
			if(notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(PieceTable, PieceTable, sectionIndex, ParagraphIndex, paragraphMarkRunIndex + 1, Cell, false, ParagraphIndex, notificationId);
			newParagraph.InheritStyleAndFormattingFromCore(paragraph);
		}
	}
	#endregion
	#region MarkRunInsertedHistoryItemBase (abstract class)
	public abstract class MarkRunInsertedHistoryItemBase : TextRunInsertedBaseHistoryItem {
		int notificationId;
		protected MarkRunInsertedHistoryItemBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();			
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			Paragraph paragraph = runs[RunIndex].Paragraph;
			TextRunBase newRun = CreateRun(paragraph);
			newRun.StartIndex = StartIndex;
			runs.Insert(RunIndex, newRun);
			ApplyFormattingToNewRun(runs, newRun);
			DocumentModel.ResetMerging();
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
			AfterInsertRun(DocumentModel, newRun, RunIndex);
			runs[RunIndex].AfterRunInserted();
		}
		public abstract TextRunBase CreateRun(Paragraph paragraph);
		protected virtual void AfterInsertRun(DocumentModel documentModel, TextRunBase run, RunIndex runIndex) {
		}
	}
	#endregion
	#region ParagraphRunInsertedHistoryItem
	public class ParagraphRunInsertedHistoryItem : MarkRunInsertedHistoryItemBase {
		public ParagraphRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override TextRunBase CreateRun(Paragraph paragraph) {
			return new ParagraphRun(paragraph);
		}
	}
	#endregion
	#region SectionRunInsertedHistoryItem
	public class SectionRunInsertedHistoryItem : ParagraphRunInsertedHistoryItem {
		public SectionRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override TextRunBase CreateRun(Paragraph paragraph) {
			return new SectionRun(paragraph);
		}
	}
	#endregion
	#region FieldResultEndRunInsertedHistoryItem
	public class FieldResultEndRunInsertedHistoryItem : MarkRunInsertedHistoryItemBase {
		public FieldResultEndRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override TextRunBase CreateRun(Paragraph paragraph) {
			return new FieldResultEndRun(paragraph);
		}
	}
	#endregion
	#region FieldCodeStartRunInsertedHistoryItem
	public class FieldCodeStartRunInsertedHistoryItem : MarkRunInsertedHistoryItemBase {
		public FieldCodeStartRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override TextRunBase CreateRun(Paragraph paragraph) {
			return new FieldCodeStartRun(paragraph);
		}
	}
	#endregion
	#region FieldCodeEndRunInsertedHistoryItem
	public class FieldCodeEndRunInsertedHistoryItem : MarkRunInsertedHistoryItemBase {
		public FieldCodeEndRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override TextRunBase CreateRun(Paragraph paragraph) {
			return new FieldCodeEndRun(paragraph);
		}
	}
	#endregion
	#region MergeParagraphsHistoryItem
	public class MergeParagraphsHistoryItem : ParagraphBaseHistoryItem {
		#region Fields
		Paragraph startParagraph;
		Paragraph endParagraph;
		int index;
		ParagraphIndex endParagraphIndex;
		int paragraphStyleIndex;
		bool useFirstParagraphStyle;
		int notificationId;
		#endregion
		public MergeParagraphsHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public Paragraph StartParagraph { get { return startParagraph; } set { startParagraph = value; } }
		public Paragraph EndParagraph { get { return endParagraph; } set { endParagraph = value; } }
		public bool UseFirstParagraphStyle { get { return useFirstParagraphStyle; } set { useFirstParagraphStyle = value; } }
		#endregion
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			this.index = EndParagraph.LastRunIndex + 1 - EndParagraph.FirstRunIndex;
			for (RunIndex i = EndParagraph.FirstRunIndex; i <= EndParagraph.LastRunIndex; i++) {
				runs[i].Paragraph = StartParagraph;
				StartParagraph.Length += runs[i].Length;
			}
			if (!UseFirstParagraphStyle) {
				paragraphStyleIndex = StartParagraph.ParagraphStyleIndex;
				StartParagraph.SetParagraphStyleIndexCore(EndParagraph.ParagraphStyleIndex);
				for (RunIndex i = StartParagraph.FirstRunIndex; i <= StartParagraph.LastRunIndex; i++) {
					runs[i].ResetMergedCharacterFormattingIndex();
				}
			}
#if UseOldIndicies
			StartParagraph.LastRunIndex = EndParagraph.LastRunIndex;
#endif
			StartParagraph.SetRelativeLastRunIndex(EndParagraph.GetLastRunIndex());
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(endParagraph.Index);
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyParagraphMerged(PieceTable, PieceTable, sectionIndex, EndParagraph.Index, EndParagraph.FirstRunIndex, notificationId);
			endParagraphIndex = EndParagraph.Index;
			RunIndex firstRunIndex = EndParagraph.FirstRunIndex;
			RunIndex lastRunIndex = EndParagraph.LastRunIndex;
			DocumentLogPosition logPosition = EndParagraph.LogPosition;
			PieceTable.Paragraphs.RemoveAt(endParagraphIndex);
			EndParagraph.AfterRemove(firstRunIndex, lastRunIndex, logPosition);
		}
		protected override void UndoCore() {
			TextRunCollection runs = PieceTable.Runs;
			PieceTable.Paragraphs.Insert(endParagraphIndex, EndParagraph);
			EndParagraph.AfterUndoRemove();
			PieceTable.Paragraphs.CheckTree(EndParagraph);
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(EndParagraph.Index - 1);
			DocumentModelStructureChangedNotifier.NotifyParagraphInserted(PieceTable, PieceTable, sectionIndex, EndParagraph.Index, EndParagraph.FirstRunIndex, Cell, true, EndParagraph.Index, notificationId);
			if (!UseFirstParagraphStyle) {
				EndParagraph.SetParagraphStyleIndexCore(StartParagraph.ParagraphStyleIndex);
				StartParagraph.SetParagraphStyleIndexCore(paragraphStyleIndex);
				for (RunIndex i = StartParagraph.FirstRunIndex; i <= StartParagraph.LastRunIndex; i++) {
					runs[i].ResetMergedCharacterFormattingIndex();
				}
			}
			for (RunIndex i = StartParagraph.LastRunIndex; i > StartParagraph.LastRunIndex - index; i--) {
				runs[i].Paragraph = EndParagraph;
				StartParagraph.Length -= runs[i].Length;
			}
#if UseOldIndicies
			StartParagraph.LastRunIndex = StartParagraph.LastRunIndex - index;
#endif
			StartParagraph.SetRelativeLastRunIndex(StartParagraph.GetLastRunIndex() - index);
		}
	}
	#endregion
	public class RemoveLastParagraphHistoryItem : ParagraphBaseHistoryItem {
		Paragraph paragraph;
		TextRunBase run;
		int originalParagraphCount;
		public RemoveLastParagraphHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int OriginalParagraphCount { get { return originalParagraphCount; } set { originalParagraphCount = value; } }
		protected override void RedoCore() {
			ParagraphIndex paragraphIndex = new ParagraphIndex(OriginalParagraphCount - 1);
			this.paragraph = PieceTable.Paragraphs[paragraphIndex];
			PieceTable.Paragraphs.RemoveAt(paragraphIndex);
			RunIndex runIndex = new RunIndex(PieceTable.Runs.Count - 1);
			this.run = PieceTable.Runs[runIndex];
			PieceTable.Runs.RemoveAt(runIndex);
		}
		protected override void UndoCore() {
			PieceTable.Runs.Add(this.run);
			PieceTable.Paragraphs.Add(this.paragraph);
		}
	}
	public class FixLastParagraphOfLastSectionHistoryItem : ParagraphBaseHistoryItem {
		Section section;
		SectionRun lastRun;
		int originalParagraphCount;
		public FixLastParagraphOfLastSectionHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int OriginalParagraphCount { get { return originalParagraphCount; } set { originalParagraphCount = value; } }
		protected override void RedoCore() {
			SectionCollection sections = DocumentModel.Sections;
			this.section = sections.Last;
			if (this.section.FirstParagraphIndex == new ParagraphIndex(OriginalParagraphCount - 1)) {
				this.section.UnsubscribeHeadersFootersEvents();
				SectionIndex sectionIndex = new SectionIndex(sections.Count - 1);
				sections.RemoveAt(sectionIndex);
			}
			else
				this.section.LastParagraphIndex--;
			TextRunCollection runs = PieceTable.Runs;
			this.lastRun = runs.Last as SectionRun;
			if (this.lastRun != null)
				DocumentModel.UnsafeEditor.ReplaceSectionRunWithParagraphRun(PieceTable, lastRun, new RunIndex(runs.Count - 1));
		}
		protected override void UndoCore() {
			if (this.section.FirstParagraphIndex == new ParagraphIndex(OriginalParagraphCount - 1)) {
				this.section.SubscribeHeadersFootersEvents();
				DocumentModel.Sections.Add(section);
			}
			else
				this.section.LastParagraphIndex++;
			if (this.lastRun != null)
				DocumentModel.UnsafeEditor.ReplaceParagraphRunWithSectionRun(PieceTable, lastRun, new RunIndex(PieceTable.Runs.Count - 1));
		}
	}
}
namespace DevExpress.XtraRichEdit.Model {
	public partial class PieceTable {
		public virtual void FixLastParagraph() {
			if (ShouldFixLastParagraph())
				FixLastParagraphCore();
			if (!DocumentModel.DocumentCapabilities.ParagraphsAllowed)
				UnsafeRemoveLastSpaceSymbolRun();
		}
		protected internal virtual bool ShouldFixLastParagraph() {
			if (Paragraphs.Count <= 1)
				return false;
			Paragraph lastParagraph = Paragraphs.Last;
			if (!lastParagraph.IsEmpty)
				return false;
			if (lastParagraph.Index > ParagraphIndex.Zero) {
				if (Paragraphs[lastParagraph.Index - 1].GetCell() != null)
					return false;
			}
			return true;
		}
		protected internal virtual void FixLastParagraphCore() {
			int count = Paragraphs.Count;
			RemoveLastParagraph(count);
			FixLastParagraphOfLastSection(count);
		}
		protected internal virtual void RemoveLastParagraph(int originalParagraphCount) {
#if DEBUGTEST
			Paragraph lastParagraph = Paragraphs.Last;
			Debug.Assert(new RunIndex(Runs.Count - 1) == lastParagraph.FirstRunIndex);
			Debug.Assert(new RunIndex(Runs.Count - 1) == lastParagraph.LastRunIndex);
#endif
			RemoveLastParagraphHistoryItem item = new RemoveLastParagraphHistoryItem(this);
			item.OriginalParagraphCount = originalParagraphCount;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal void FixLastParagraphOfLastSection(int originalParagraphCount) {
			contentType.FixLastParagraphOfLastSection(originalParagraphCount);
		}
		protected internal virtual void UnsafeRemoveLastSpaceSymbolRun() {
			Debug.Assert(!DocumentModel.DocumentCapabilities.ParagraphsAllowed);
			if (Runs.Count <= 1)
				return;
			RunIndex runIndex = new RunIndex(Runs.Count - 2);
			TextRun run = runs[runIndex] as TextRun;
			if (run != null)
				if (run.Length == 1)
					documentModel.UnsafeEditor.DeleteRuns(this, runIndex, 1);
				else
					DeleteContent(DocumentEndLogPosition - 1, 1, false);
			}
	}
}
