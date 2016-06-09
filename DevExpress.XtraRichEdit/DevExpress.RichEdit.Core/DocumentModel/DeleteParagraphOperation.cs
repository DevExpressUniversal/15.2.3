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
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region DeleteParagraphOperation
	public class DeleteParagraphOperation : SelectionBasedOperation {
		#region Fields
		bool allowedDeleteLastParagraphInTableCell = false;
		bool isProcessContentCrossParentExecute;
		bool isDeletedSomeSections;
		#endregion
		public DeleteParagraphOperation(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		internal bool AllowedDeleteLastParagraphInTableCell { get { return allowedDeleteLastParagraphInTableCell; } set { allowedDeleteLastParagraphInTableCell = value; } }
		internal bool IsProcessContentCrossParentExecute { get { return isProcessContentCrossParentExecute; } }
		internal bool IsDeletedSomeSections { get { return isDeletedSomeSections; } set { isDeletedSomeSections = value; } }
		internal bool BackspacePressed { get; set; }
		#endregion
		protected internal override bool ShouldProcessContentInSameParent(RunInfo info) {
			DocumentModelPosition start = info.Start;
			DocumentModelPosition end = info.End;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			return start.ParagraphIndex == end.ParagraphIndex &&
				(start.RunIndex != paragraphs[start.ParagraphIndex].FirstRunIndex || end.RunIndex != paragraphs[start.ParagraphIndex].LastRunIndex);
		}
		protected internal override bool ShouldProcessRunParent(RunInfo info) {
			DocumentModelPosition start = info.Start;
			DocumentModelPosition end = info.End;
			Paragraph paragraph = PieceTable.Paragraphs[start.ParagraphIndex];
			return (start.RunIndex == paragraph.LastRunIndex && end.RunIndex == paragraph.LastRunIndex);
		}
		protected internal override void ProcessRunParent(RunInfo info, bool documentLastParagraphSelected) {
			if (IsDeletedLastParagraphInCell(info)) {
				return;
			}
			if (PieceTable.Runs.Last == PieceTable.Runs[info.Start.RunIndex])
				return;
			Paragraph paragraph = PieceTable.Paragraphs[info.Start.ParagraphIndex];
			ParagraphIndex paragraphIndex = paragraph.Index;
			if (ParagraphContentIsNumberingRunOnly(paragraph))
				ProcessParagraphs(paragraphIndex, 1);
			else {
				Paragraph paragraphInTable = PieceTable.Paragraphs[paragraphIndex + 1];
				TableCell cell = paragraph.GetCell();
				TableCell nextCell = paragraphInTable.GetCell();
				if (nextCell != null && (cell == null || cell.Table != nextCell.Table)) {
					int fieldsCounter = 0;
					TextRunBase run;
					for (RunIndex i = paragraph.FirstRunIndex; i <= paragraph.LastRunIndex; i++) {
						run = PieceTable.Runs[i];
						if (run is FieldCodeStartRun)
							fieldsCounter++;
						if (run is FieldResultEndRun)
							fieldsCounter--;
					}
					if (fieldsCounter != 0)
						return;
				}
				ProcessNumberingRun(paragraph, DocumentModel.EditingOptions.MergeUseFirstParagraphStyle);
				DeleteRuns(paragraph.LastRunIndex, 1);
				bool isNeedJoinTables = IsNeedJoinTables(paragraphIndex, paragraphIndex);
				DocumentModel.UnsafeEditor.MergeParagraphs(PieceTable, paragraph, paragraphInTable, true, cell);
				if (isNeedJoinTables)
					JoinTables(paragraphIndex);
			}
		}
		protected internal virtual bool IsNeedJoinTables(ParagraphIndex topParagraphIndex, ParagraphIndex bottomParagraphIndex) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			if (topParagraphIndex == ParagraphIndex.Zero || bottomParagraphIndex == new ParagraphIndex(paragraphs.Count - 1))
				return false;
			TableCell previousCell = paragraphs[topParagraphIndex - 1].GetCell();
			if (previousCell == null)
				return false;
			Table previousTable = previousCell.Table;
			int previousTableNestedLevel = previousTable.NestedLevel;
			TableCell nextCell = PieceTable.TableCellsManager.GetCellByNestingLevel(bottomParagraphIndex + 1, previousTableNestedLevel);
			if (nextCell == null || nextCell.StartParagraphIndex <= topParagraphIndex)
				return false;
			Table nextTable = nextCell.Table;
			if (previousTable != nextTable && previousTableNestedLevel == nextTable.NestedLevel) {
				return previousTable.ParentCell == nextTable.ParentCell;
			}
			return false;
		}
		protected internal virtual void JoinTables(ParagraphIndex paragraphIndex) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			Paragraph previousParagraph = paragraphs[paragraphIndex - 1];
			Table topTable = previousParagraph.GetCell().Table;
			Table bottomTable = PieceTable.TableCellsManager.GetCellByNestingLevel(paragraphIndex, topTable.NestedLevel).Table;
			topTable.NormalizeCellColumnSpans();
			bottomTable.NormalizeCellColumnSpans();
			PieceTable.JoinTables(topTable, bottomTable);
		}
		protected internal virtual bool IsDeletedLastParagraphInCell(RunInfo info) {
			ParagraphIndex endParagraphIndex = info.NormalizedEnd.ParagraphIndex;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			TableCell cell = paragraphs[endParagraphIndex].GetCell();
			if (cell == null)
				return false;
			return paragraphs[cell.EndParagraphIndex].LastRunIndex == info.NormalizedEnd.RunIndex;
		}
		protected internal virtual bool IsMergeParagraphWithTable(RunInfo info) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			ParagraphIndex startParagraphIndex = info.NormalizedStart.ParagraphIndex;
			if (startParagraphIndex + 1 == new ParagraphIndex(paragraphs.Count))
				return false;
			Paragraph startParagraph = paragraphs[startParagraphIndex];
			TableCell startCell = startParagraph.GetCell();
			Paragraph nextParagraph = paragraphs[startParagraphIndex + 1];
			bool selectOnlyParagraph = startParagraph.LastRunIndex == info.NormalizedEnd.RunIndex;
			TableCell nextCell = nextParagraph.GetCell();
			if (startCell == null && nextCell != null && selectOnlyParagraph)
				return true;
			Table startTable = startCell != null ? startCell.Table : null;
			Table nextTable = nextCell != null ? nextCell.Table : null;
			if (startTable != null && nextTable != null && startTable != nextTable && startTable.NestedLevel != nextTable.NestedLevel && selectOnlyParagraph)
				return true;
			return false;
		}
		private bool ParagraphContentIsNumberingRunOnly(Paragraph paragraph) {
			return (paragraph.IsInList() && paragraph.Length <= 1);
		}
		private void ProcessNumberingRun(Paragraph paragraph, bool useFirstParagraphStyle) {
			Paragraph nextParagraph = PieceTable.Paragraphs[paragraph.Index + 1];
			if (nextParagraph.IsInList() && paragraph.IsInList() && useFirstParagraphStyle) {
				return;
			}
			if (paragraph.IsInList() && !useFirstParagraphStyle) {
				PieceTable.RemoveNumberingFromParagraph(paragraph);
			}
			if (nextParagraph.IsInList()) {
				if (!useFirstParagraphStyle)
					PieceTable.AddNumberingListToParagraph(paragraph, nextParagraph.GetNumberingListIndex(), nextParagraph.GetListLevelIndex());
				PieceTable.RemoveNumberingFromParagraph(nextParagraph);
			}
		}
		protected internal override void ProcessContentInsideParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			bool shouldMergeParagraphs = DeleteRunsInsideParagraphCore(info, allowMergeWithNextParagraph);
			if (shouldMergeParagraphs)
				MergeParagraphWithNext(PieceTable.Paragraphs[info.Start.ParagraphIndex]);
		}
		protected internal virtual void MergeParagraphWithNext(Paragraph paragraph) {
			ParagraphIndex paragraphIndex = paragraph.Index;
			ParagraphIndex nextParagraph = paragraphIndex + 1;
			ProcessNumberingRun(paragraph, DocumentModel.EditingOptions.MergeUseFirstParagraphStyle);
			TableCell cell = paragraph.GetCell();
			bool isNeedJoinTables = IsNeedJoinTables(paragraphIndex, paragraphIndex);
			DocumentModel.UnsafeEditor.MergeParagraphs(PieceTable, paragraph, PieceTable.Paragraphs[nextParagraph], DocumentModel.EditingOptions.MergeUseFirstParagraphStyle, cell);
			if (isNeedJoinTables)
				JoinTables(paragraphIndex);
		}
		protected internal virtual bool DeleteRunsInsideParagraphCore(RunInfo info, bool allowMergeWithNextParagraph) {
			Paragraph paragraph = PieceTable.Paragraphs[info.Start.ParagraphIndex];
			RunIndex startRunIndex = info.Start.RunIndex;
			RunIndex endRunIndex = info.End.RunIndex;
			bool forbidMergeParagraph = false;
			bool mergeParagraphs = (endRunIndex == paragraph.LastRunIndex) && allowMergeWithNextParagraph;
			if (mergeParagraphs) {
				forbidMergeParagraph = (IsMergeParagraphWithTable(info) && !IsProcessContentCrossParentExecute) || IsDeletedLastParagraphInCell(info);
				mergeParagraphs = mergeParagraphs && !forbidMergeParagraph;
			}
			int runCount = endRunIndex - startRunIndex + 1;
			if (endRunIndex == paragraph.LastRunIndex && !allowMergeWithNextParagraph || endRunIndex == paragraph.LastRunIndex && forbidMergeParagraph)
				runCount--;
			DeleteRuns(startRunIndex, runCount);
			return mergeParagraphs;
		}
		protected internal override void ProcessContentCrossParent(RunInfo info, bool documentLastParagraphSelected) {
			if (IsTheOnlyParagraph(info.Start.ParagraphIndex))
				return;
			isProcessContentCrossParentExecute = true;
			PieceTable.DeleteSelectedTables(info, documentLastParagraphSelected);
			if (DeleteTablesHelper.IsDeleteParagraphsInTable(info, documentLastParagraphSelected, IsDeletedSomeSections)) {
				ProcessTable(info, documentLastParagraphSelected);
				return;
			}
			ProcessContentCrossParentCore(info, documentLastParagraphSelected);
		}
		protected internal virtual void ProcessContentCrossParentCore(RunInfo info, bool documentLastParagraphSelected) {
			DeleteTablesOptions options = new DeleteTablesOptions(documentLastParagraphSelected, IsDeletedSomeSections, BackspacePressed);
			DeleteTablesHelper.DeleteSelectedTableRows(info, options);
			int paragraphCount = ProcessTail(info, documentLastParagraphSelected);
			bool shouldDeleteHead = ProcessMiddle(info, paragraphCount, documentLastParagraphSelected);
			if (shouldDeleteHead)
				ProcessHead(info, documentLastParagraphSelected);
		}
		protected internal virtual bool IsTheOnlyParagraph(ParagraphIndex paragraphIndex) {
			return paragraphIndex >= new ParagraphIndex(PieceTable.Paragraphs.Count - 1);
		}
		protected internal override int ProcessHead(RunInfo info, bool documentLastParagraphSelected) {
			Paragraph startParagraph = PieceTable.Paragraphs[info.Start.ParagraphIndex];
			RunInfo startInfo = new RunInfo(PieceTable);
			startInfo.Start.CopyFrom(info.Start);
			DocumentModelPosition.SetParagraphEnd(startInfo.End, startParagraph.Index);
			ProcessContentSameParent(startInfo, DocumentModel.EditingOptions.MergeParagraphsContent, documentLastParagraphSelected);
			return 0;
		}
		protected internal override bool ProcessMiddle(RunInfo info, int paragraphCount, bool documentLastParagraphSelected) {
			Paragraph startParagraph = PieceTable.Paragraphs[info.Start.ParagraphIndex];
			if (info.Start.RunIndex == startParagraph.FirstRunIndex) {
				ProcessParagraphs(startParagraph.Index, paragraphCount + 1);
				return false;
			}
			else {
				ProcessParagraphs(startParagraph.Index + 1, paragraphCount);
				return true;
			}
		}
		protected internal bool ProcessMiddleTable(RunInfo info, int paragraphCount) {
			Paragraph startParagraph = PieceTable.Paragraphs[info.Start.ParagraphIndex];
			if (info.Start.RunIndex == startParagraph.FirstRunIndex) {
				ProcessParagraphsInTable(startParagraph.Index, paragraphCount + 1);
				return false;
			}
			else {
				ProcessParagraphsInTable(startParagraph.Index + 1, paragraphCount);
				return true;
			}
		}
		protected internal override int ProcessTail(RunInfo info, bool documentLastParagraphSelected) {
			Paragraph endParagraph = PieceTable.Paragraphs[info.End.ParagraphIndex];
			RunIndex startRunIndex = endParagraph.FirstRunIndex;
			int paragraphCount = endParagraph.Index - info.Start.ParagraphIndex;
			if (info.End.RunIndex != endParagraph.LastRunIndex) {
				int runCount = info.End.RunIndex - startRunIndex + 1;
				DeleteRuns(startRunIndex, runCount);
				return paragraphCount - 1;
			}
			else
				return paragraphCount;
		}
		protected internal virtual void ProcessTable(RunInfo info, bool documentLastParagraphSelected) {
			DeleteTablesOptions options = new DeleteTablesOptions(documentLastParagraphSelected, IsDeletedSomeSections, BackspacePressed);
			DeleteTablesHelper.DeleteSelectedTableRows(info, options);
			ProcessContentCrossParentTable(info, documentLastParagraphSelected);
		}
		protected internal virtual void ProcessContentCrossParentTable(RunInfo info, bool documentLastParagraphSelected) {
			int paragraphCount = ProcessTail(info, documentLastParagraphSelected);
			bool shouldDeleteHead = ProcessMiddleTable(info, paragraphCount);
			if (shouldDeleteHead)
				ProcessHead(info, documentLastParagraphSelected);
		}
		protected internal virtual void ProcessAllParagraphRuns(ParagraphIndex paragraphIndex) {
			DocumentModel.UnsafeEditor.DeleteAllRunsInParagraph(PieceTable, paragraphIndex);
		}
		protected internal virtual void ProcessParagraphs(ParagraphIndex startParagraphIndex, int count) {
			ProcessParagraphsCore(startParagraphIndex, count);
			bool isNeedJoinTables = IsNeedJoinTables(startParagraphIndex, startParagraphIndex + count - 1);
			DocumentModel.UnsafeEditor.DeleteParagraphs(PieceTable, startParagraphIndex, count, null);
			if (isNeedJoinTables)
				JoinTables(startParagraphIndex);
		}
		protected internal virtual void ProcessParagraphsInTable(ParagraphIndex startParagraphIndex, int count) {
			ParagraphIndex endParagraphIndex = startParagraphIndex + count - 1;
			bool isNeedJoinTables = IsNeedJoinTables(startParagraphIndex, endParagraphIndex);
			for (ParagraphIndex i = endParagraphIndex; i >= startParagraphIndex; i--) {
				TableCell currentCell = PieceTable.Paragraphs[i].GetCell();
				if (currentCell == null || currentCell.EndParagraphIndex != i || AllowedDeleteLastParagraphInTableCell) {
					ProcessParagraphsCore(i, 1);
					DocumentModel.UnsafeEditor.DeleteParagraphs(PieceTable, i, 1, currentCell);
				}
				else {
					Paragraph paragraph = PieceTable.Paragraphs[i];
					int runsCount = paragraph.LastRunIndex - paragraph.FirstRunIndex;
					if (runsCount > 0)
						DeleteRuns(paragraph.FirstRunIndex, runsCount);
				}
			}
			if (isNeedJoinTables)
				JoinTables(startParagraphIndex);
		}
		protected internal virtual void ProcessParagraphsCore(ParagraphIndex startParagraphIndex, int count) {
			ParagraphIndex endParagraphIndex = startParagraphIndex + count;
			for (ParagraphIndex i = startParagraphIndex; i < endParagraphIndex; i++) {
				ProcessAllParagraphRuns(i);
			}
		}
		protected internal override void BeforeExecute() {
		}
		protected internal override void AfterExecute() {
		}
		protected internal virtual void DeleteRuns(RunIndex startIndex, int runCount) {
			DocumentModel.UnsafeEditor.DeleteRuns(PieceTable, startIndex, runCount);
		}
	}
	#endregion
}
