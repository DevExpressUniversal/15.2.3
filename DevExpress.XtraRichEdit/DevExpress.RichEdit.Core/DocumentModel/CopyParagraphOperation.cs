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
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model {
	#region CopyParagraphOperation
	public class CopyParagraphOperation : SelectionBasedOperation {
		#region Fields
		readonly DocumentModelCopyManager copyManager;
		int transactionItemCountBeforeExecute;
		bool isMergingTableCell;
		#endregion
		public CopyParagraphOperation(DocumentModelCopyManager copyManager)
			: base(GetPieceTable(copyManager)) {
			this.copyManager = copyManager;
		}
		static PieceTable GetPieceTable(DocumentModelCopyManager copyManager) {
			Guard.ArgumentNotNull(copyManager, "copyManager");
			return copyManager.SourcePieceTable;
		}
		#region Properties
		public DocumentModelCopyManager CopyManager { get { return copyManager; } }
		public DocumentModel TargetModel { get { return copyManager.TargetModel; } }
		public DocumentModel SourceModel { get { return copyManager.SourceModel; } }
		public PieceTable TargetPieceTable { get { return copyManager.TargetPieceTable; } }
		public PieceTable SourcePieceTable { get { return copyManager.SourcePieceTable; } }
		public DocumentModelPosition TargetPosition { get { return copyManager.TargetPosition; } }
		public TableCopyHelper TableCopyManager { get { return CopyManager.TableCopyHelper; } set { CopyManager.TableCopyHelper = value; } }
		protected internal bool IsMergingTableCell { get { return isMergingTableCell; } set { isMergingTableCell = value; } }
		#endregion
		public override bool ExecuteCore(RunInfo info, bool documentLastParagraphSelected) {
			using (HistoryTransaction transaction = new HistoryTransaction(TargetModel.History)) {
				return base.ExecuteCore(info, false);
			}
		}
		protected internal override bool ShouldProcessContentInSameParent(RunInfo info) {
			DocumentModelPosition start = info.Start;
			DocumentModelPosition end = info.End;
			ParagraphCollection paragraphs = SourcePieceTable.Paragraphs;
			return start.ParagraphIndex == end.ParagraphIndex &&
				(start.RunIndex != paragraphs[start.ParagraphIndex].FirstRunIndex || end.RunIndex != paragraphs[start.ParagraphIndex].LastRunIndex);
		}
		protected internal override bool ShouldProcessRunParent(RunInfo info) {
			TextRunBase endRun = SourcePieceTable.Runs[info.End.RunIndex];
			return (endRun.GetType() == typeof(ParagraphRun));
		}
		protected internal override void ProcessRunParent(RunInfo info, bool documentLastParagraphSelected) {
			ProcessParagraph(info.Start.RunIndex, info.End.ParagraphIndex);
		}
		protected internal virtual void ProcessParagraph(RunIndex runIndex, ParagraphIndex paragraphIndex) {
			Paragraph sourceParagraph = SourcePieceTable.Paragraphs[paragraphIndex];
			Paragraph targetParagraph = sourceParagraph.Copy(CopyManager);
			TextRunBase targetRun = TargetPieceTable.Runs[targetParagraph.LastRunIndex];
			TextRunBase sourceRun = SourcePieceTable.Runs[sourceParagraph.LastRunIndex];
			if (TargetModel.DocumentCapabilities.CharacterFormattingAllowed)
				targetRun.CharacterProperties.CopyFrom(sourceRun.CharacterProperties.Info);
			if (TargetModel.DocumentCapabilities.CharacterStyleAllowed)
				targetRun.CharacterStyleIndex = sourceRun.CharacterStyle.Copy(CopyManager.TargetModel);
			ProcessRuns(runIndex, sourceParagraph.LastRunIndex - 1);
			if (TargetPieceTable.DocumentModel.DocumentCapabilities.ParagraphsAllowed)
				copyManager.OnTargetParagraphInserted(sourceParagraph, targetParagraph, sourceRun, targetRun);
			else
				InsertSpaceInsteadParagraph();
		}
		protected internal virtual void InsertSpaceInsteadParagraph() {
			TargetPieceTable.InsertPlainText(TargetPosition.LogPosition, " ");
			TargetModel.ResetMerging();
			TargetPosition.LogPosition++;
			TargetPosition.RunStartLogPosition++;
			TargetPosition.RunIndex++;
		}
		protected internal override void ProcessContentInsideParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			Paragraph paragraph = SourcePieceTable.Paragraphs[info.Start.ParagraphIndex];
			RunIndex startRunIndex = info.Start.RunIndex;
			ProcessRuns(startRunIndex, info.End.RunIndex);
			if ((paragraph.IsInNonStyleList() || paragraph.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList)
				&& startRunIndex == paragraph.FirstRunIndex 
				&& CopyManager.ParagraphNumerationCopyOptions == ParagraphNumerationCopyOptions.CopyAlways)
				paragraph.CopyNumberingListProperties( CopyManager.TargetPieceTable.Paragraphs[CopyManager.TargetPosition.ParagraphIndex]);
		}
		protected internal virtual void ProcessRuns(RunIndex firstRunIndex, RunIndex lastRunIndex) {
			TargetModel.ResetMerging();
			for (RunIndex i = firstRunIndex; i <= lastRunIndex; i++) {
				TextRunBase sourceRun = SourcePieceTable.Runs[i];
				TextRunBase targetRun = sourceRun.Copy(CopyManager);
				TargetModel.ResetMerging();
				copyManager.OnTargetRunInserted(sourceRun, targetRun);
			}
		}
		protected internal override int ProcessHead(RunInfo info, bool documentLastParagraphSelected) {
			Paragraph paragraph = SourcePieceTable.Paragraphs[info.Start.ParagraphIndex];
			int paragraphCount = info.End.ParagraphIndex - paragraph.Index + 1;
			if (paragraph.FirstRunIndex == info.Start.RunIndex)
				return paragraphCount;
			else {
				ProcessParagraph(info.Start.RunIndex, paragraph.Index);
				return paragraphCount - 1;
			}
		}
		protected internal override bool ProcessMiddle(RunInfo info, int paragraphCount, bool documentLastParagraphSelected) {
			if (NeedInsertParagraphBeforeTable(info) || IsMergingTableCell)
				InsertParagraphBeforeTable();
			ParagraphIndex endParagraphIndex = info.End.ParagraphIndex;
			ParagraphIndex startIndex = endParagraphIndex - paragraphCount + 1;
			RunIndex lastRunIndex = SourcePieceTable.Paragraphs[endParagraphIndex].LastRunIndex;
			if (info.End.RunIndex == lastRunIndex) {
				ProcessMiddleCore(startIndex, endParagraphIndex);
				if (IsMergingTableCell)
					DeleteLastParagraphInTableCell();
				return false;
			}
			else {
				ProcessMiddleCore(startIndex, endParagraphIndex - 1);
				return true;
			}
		}
		protected internal virtual bool NeedInsertParagraphBeforeTable(RunInfo info) {
			TableCell sourceStartCell = SourcePieceTable.Paragraphs[info.Start.ParagraphIndex].GetCell();
			DocumentLogPosition targetParagraphLogPosition = TargetPieceTable.Paragraphs[TargetPosition.ParagraphIndex].LogPosition;
			return sourceStartCell != null && targetParagraphLogPosition != TargetPosition.LogPosition;
		}
		protected internal virtual void InsertParagraphBeforeTable() {
			TargetPieceTable.InsertParagraph(TargetPosition.LogPosition);
			CopyManager.TableCopyHelper.TargetStartParagraphIndex++;
			CopyManager.ParagraphWasInsertedBeforeTable = true;
			TargetModel.ResetMerging();
			TargetPosition.ParagraphIndex++;
			TargetPosition.LogPosition++;
			TargetPosition.RunStartLogPosition++;
			TargetPosition.RunIndex++;
		}
		protected internal virtual void DeleteLastParagraphInTableCell() {
			DeleteParagraphOperation operation = new DeleteParagraphOperation(TargetPieceTable);
			operation.AllowedDeleteLastParagraphInTableCell = true;
			operation.Execute(TargetPosition.LogPosition, 1, false);
		}
		protected internal virtual void ProcessMiddleCore(ParagraphIndex startIndex, ParagraphIndex endParagraphIndex) {
			ParagraphCollection paragraphs = SourcePieceTable.Paragraphs;
			for (ParagraphIndex i = startIndex; i <= endParagraphIndex; i++)
				ProcessParagraph(paragraphs[i].FirstRunIndex, i);
		}
		protected internal override int ProcessTail(RunInfo info, bool documentLastParagraphSelected) {
			Paragraph paragraph = SourcePieceTable.Paragraphs[info.End.ParagraphIndex];
			PieceTable targetPieceTable = CopyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = CopyManager.TargetPosition;
			Paragraph targetParagraph = targetPieceTable.Paragraphs[targetPosition.ParagraphIndex];
			DocumentCapabilitiesOptions options = targetParagraph.DocumentModel.DocumentCapabilities;
			if(options.ParagraphStyleAllowed)
				targetParagraph.ParagraphStyleIndex = paragraph.ParagraphStyle.Copy(CopyManager.TargetModel);
			targetParagraph.ParagraphStyle.ParagraphProperties.Alignment = ParagraphAlignment.Left;
			RunIndex firstRunIndex = paragraph.FirstRunIndex;
			ProcessRuns(firstRunIndex, info.End.RunIndex);
			if ((paragraph.IsInNonStyleList() || paragraph.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList)
				&& CopyManager.ParagraphNumerationCopyOptions == ParagraphNumerationCopyOptions.CopyAlways)
				paragraph.CopyNumberingListProperties(targetParagraph);
			return 0;
		}
		protected internal override void BeforeExecute() {
			DocumentModel.SuppressPerformLayout++;
			CompositeHistoryItem transaction = SourceModel.History.Transaction;
			transactionItemCountBeforeExecute = transaction.Items.Count;
		}
		protected internal override void AfterExecute() {
			CompositeHistoryItem transaction = SourceModel.History.Transaction;
			for (int i = transaction.Items.Count - 1; i >= transactionItemCountBeforeExecute; i--) {
				HistoryItem item = transaction.Items[i];
				if (Object.ReferenceEquals(item.DocumentModelPart, SourcePieceTable)) {
					item.Undo();
					transaction.Items.RemoveAt(i);
				}
			}
			DocumentModel.SuppressPerformLayout--;
		}
	}
	#endregion
}
