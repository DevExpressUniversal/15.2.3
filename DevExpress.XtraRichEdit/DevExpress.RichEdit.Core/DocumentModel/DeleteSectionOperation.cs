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
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Model {
	#region DeleteSectionOperation
	public class DeleteSectionOperation : SelectionBasedOperation {
		public DeleteSectionOperation(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal bool AffectsMainPieceTable { get { return Object.ReferenceEquals(PieceTable, DocumentModel.MainPieceTable); } }
		bool isDeletedSomeSections;
		public bool BackspacePressed { get; set; }
		protected internal override bool ShouldProcessContentInSameParent(RunInfo info) {
			if (!AffectsMainPieceTable)
				return true; 
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			SectionCollection sections = DocumentModel.Sections;
			SectionIndex startSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			SectionIndex endSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			return startSectionIndex == endSectionIndex &&
				(info.Start.RunIndex != paragraphs[sections[startSectionIndex].FirstParagraphIndex].FirstRunIndex || info.End.RunIndex != paragraphs[sections[startSectionIndex].LastParagraphIndex].LastRunIndex);
		}
		protected internal override bool ShouldProcessRunParent(RunInfo info) {
			if (!AffectsMainPieceTable)
				return false; 
			SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			Section section = DocumentModel.Sections[sectionIndex];
			TextRunBase endRun = PieceTable.Runs[info.End.RunIndex];
			RunIndex lastSectionRun = PieceTable.Paragraphs[section.LastParagraphIndex].LastRunIndex;
			return (info.End.RunIndex == lastSectionRun && endRun.GetType() == typeof(SectionRun));
		}
		protected internal override void ProcessRunParent(RunInfo info, bool documentLastParagraphSelected) {
			if (AffectsMainPieceTable) {
				SectionIndex sectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
				DocumentModel.UnsafeEditor.DeleteSections(sectionIndex, 1);
			}
			ProcessContentInsideParent(info, true, documentLastParagraphSelected);
		}
		protected internal override void ProcessContentInsideParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			DeleteParagraphOperation deleteParagraphOperation = new DeleteParagraphOperation(PieceTable);
			deleteParagraphOperation.IsDeletedSomeSections = isDeletedSomeSections;
			deleteParagraphOperation.BackspacePressed = BackspacePressed;
			deleteParagraphOperation.ExecuteCore(info, documentLastParagraphSelected);
		}
		protected internal override void ProcessContentCrossParent(RunInfo info, bool documentLastParagraphSelected) {
			int sectionCount = ProcessTail(info, documentLastParagraphSelected);
			bool shouldDeleteHead = ProcessMiddle(info, sectionCount, documentLastParagraphSelected);
			if (shouldDeleteHead)
				ProcessHead(info, documentLastParagraphSelected);
		}
		protected internal virtual void ProcessSections(SectionIndex startSectionIndex, int count) {
			SectionIndex endSectionIndex = startSectionIndex + count;
			DeleteAllParagraphsInsideSection(startSectionIndex, endSectionIndex);
			DocumentModel.UnsafeEditor.DeleteSections(startSectionIndex, count);
		}
		protected internal virtual void DeleteAllParagraphsInsideSection(SectionIndex startSectionIndex, SectionIndex endSectionIndex) {
			for (SectionIndex i = startSectionIndex; i < endSectionIndex; i++) {
				Section sections = DocumentModel.Sections[i];
				int paragraphCount = sections.LastParagraphIndex - sections.FirstParagraphIndex + 1;
				DeleteParagraphOperation deleteParagraph = new DeleteParagraphOperation(PieceTable);
				ParagraphCollection paragraphs = PieceTable.Paragraphs;
				DocumentLogPosition startLogPosition = paragraphs[sections.FirstParagraphIndex].LogPosition;
				DocumentLogPosition endLogPosition = paragraphs[sections.FirstParagraphIndex + paragraphCount - 1].EndLogPosition + 1;
				deleteParagraph.Execute(startLogPosition, endLogPosition - startLogPosition, endLogPosition == PieceTable.DocumentEndLogPosition);
			}
		}
		protected internal override int ProcessHead(RunInfo info, bool documentLastParagraphSelected) {
			SectionIndex startSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			Paragraph paragraph = PieceTable.Paragraphs[DocumentModel.Sections[startSectionIndex].LastParagraphIndex];
			int length = paragraph.LogPosition + paragraph.Length - info.Start.LogPosition;
			RunInfo sectionContentInfo = PieceTable.ObtainAffectedRunInfo(info.Start.LogPosition, length);
			DocumentModel.UnsafeEditor.DeleteSections(startSectionIndex, 1);
			ProcessContentInsideParent(sectionContentInfo, true, documentLastParagraphSelected);
			return 0;
		}
		protected internal override bool ProcessMiddle(RunInfo info, int count, bool documentLastParagraphSelected) {
			SectionIndex startSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			Section startSection = DocumentModel.Sections[startSectionIndex];
			if (info.Start.RunIndex == PieceTable.Paragraphs[startSection.FirstParagraphIndex].FirstRunIndex) {
				ProcessSections(startSectionIndex, count + 1);
				return false;
			}
			else {
				ProcessSections(startSectionIndex + 1, count);
				return true;
			}
		}
		protected internal override int ProcessTail(RunInfo info, bool documentLastParagraphSelected) {
			SectionIndex endSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			SectionIndex startSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			Section endSection = DocumentModel.Sections[endSectionIndex];
			int paragraphCount = endSectionIndex - startSectionIndex;
			if (info.End.RunIndex != PieceTable.Paragraphs[endSection.LastParagraphIndex].LastRunIndex) {
				DeleteTail(info, endSection, documentLastParagraphSelected);
				return paragraphCount - 1;
			}
			else
				return paragraphCount;
		}
		private void DeleteTail(RunInfo info, Section endSection, bool documentLastParagraphSelected) {
			DocumentLogPosition startLogPosition = PieceTable.Paragraphs[endSection.FirstParagraphIndex].LogPosition;
			RunInfo sectionContentInfo = PieceTable.ObtainAffectedRunInfo(startLogPosition, info.End.LogPosition - startLogPosition + 1);
			SectionIndex startSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.NormalizedStart.ParagraphIndex);
			SectionIndex endSectionIndex = PieceTable.LookupSectionIndexByParagraphIndex(info.NormalizedEnd.ParagraphIndex);
			isDeletedSomeSections = startSectionIndex != endSectionIndex;
			ProcessContentInsideParent(sectionContentInfo, true, documentLastParagraphSelected);
		}
		protected internal override void BeforeExecute() {
		}
		protected internal override void AfterExecute() {
		}
	}
	#endregion
	#region DeleteTablesHelper (helper class)
	public class DeleteTablesOptions {
		public DeleteTablesOptions(bool documentLastParagraphSelected, bool isDeletedSomeSections, bool backspacePressed) {
			DocumentLastParagraphSelected = documentLastParagraphSelected;
			IsDeletedSomeSections = isDeletedSomeSections;
			BackspacePressed = backspacePressed;
		}
		public bool DocumentLastParagraphSelected { get; set; }
		public bool IsDeletedSomeSections { get; set; }
		public bool BackspacePressed { get; set; }
	}
	public static class DeleteTablesHelper {
		public static bool IsDeleteParagraphsInTable(RunInfo info, bool documentLastParagraphSelected, bool isDeletedSomeSections) {
			if (isDeletedSomeSections)
				return false;
			PieceTable pieceTable = info.End.PieceTable;
			TableCellsManager.TableCellNode root = pieceTable.TableCellsManager.GetCellSubTree(info.Start.ParagraphIndex, info.End.ParagraphIndex, 0);
			if (root != null && root.ChildNodes.Count > 0)
				return true;
			ParagraphIndex start = info.Start.ParagraphIndex;
			ParagraphIndex end = info.End.ParagraphIndex;
			TableCell startCell = pieceTable.Paragraphs[start].GetCell();
			TableCell endCell = pieceTable.Paragraphs[end].GetCell();
			if (startCell != null || endCell != null)
				return true;
			else
				return false;
		}
		static bool IsSelectedEntireTableRows(RunInfo info, DeleteTablesOptions options) {
			PieceTable PieceTable = info.End.PieceTable;
			ParagraphIndex startParagraphIndex = info.NormalizedStart.ParagraphIndex;
			ParagraphIndex endParagraphIndex = info.NormalizedEnd.ParagraphIndex;
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			TableCell startCell = paragraphs[startParagraphIndex].GetCell();
			TableCell endCell = paragraphs[endParagraphIndex].GetCell();
			if (startCell == null && endCell == null)
				return false;
			if (startCell != null && endCell != null && startCell.Table == endCell.Table && !options.DocumentLastParagraphSelected && !options.IsDeletedSomeSections)
				return false;
			RunIndex infoEndRunIndex = info.NormalizedEnd.RunIndex;
			RunIndex infoStartRunIndex = info.NormalizedStart.RunIndex;
			bool selectWhollyFirstCell = startCell != null ? startCell.IsFirstCellInRow && paragraphs[startCell.StartParagraphIndex].FirstRunIndex == infoStartRunIndex : false;
			bool selectWhollyLastCell = endCell != null ? endCell.IsLastCellInRow && paragraphs[endCell.EndParagraphIndex].LastRunIndex == infoEndRunIndex : false;
			bool differentTable = startCell != null && endCell != null && startCell.Table != endCell.Table;
			if ((startCell == null || differentTable) && selectWhollyLastCell)
				return true;
			if ((endCell == null || differentTable) && selectWhollyFirstCell)
				return true;
			if (startCell != null && endCell != null && selectWhollyFirstCell && selectWhollyLastCell)
				return true;
			return false;
		}
		static void GetSelectedRowsCore(TableCellsManager.TableCellNode current, RunInfo info, List<TableRow> result) {
			if (current == null)
				return;
			for (int i = current.ChildNodes.Count - 1; i >= 0; i--) {
				TableCellsManager.TableCellNode currentNode = current.ChildNodes[i];
				if (currentNode.ChildNodes != null)
					GetSelectedRowsCore(currentNode, info, result);
				TableRow row = currentNode.Cell.Row;
				if (!result.Contains(row) && IsSelectedEntireTableRow(row, info))
					result.Add(row);
			}
		}
		static bool IsSelectedEntireTableRow(TableRow row, RunInfo info) {
			ParagraphIndex rowStartParagraphIndex = row.FirstCell.StartParagraphIndex;
			ParagraphIndex rowEndParagraphIndex = row.LastCell.EndParagraphIndex;
			ParagraphCollection paragraphs = row.PieceTable.Paragraphs;
			RunIndex rowStartRunIndex = paragraphs[rowStartParagraphIndex].FirstRunIndex;
			RunIndex rowEndRunIndex = paragraphs[rowEndParagraphIndex].LastRunIndex;
			if (rowStartRunIndex >= info.NormalizedStart.RunIndex && rowEndRunIndex <= info.NormalizedEnd.RunIndex)
				return true;
			return false;
		}
		static List<TableRow> GetSelectedRows(RunInfo info) {
			List<TableRow> result = new List<TableRow>();
			TableCellsManager.TableCellNode root = info.End.PieceTable.TableCellsManager.GetCellSubTree(info.NormalizedStart.ParagraphIndex, info.NormalizedEnd.ParagraphIndex, -1);
			GetSelectedRowsCore(root, info, result);
			return result;
		}
		public static void DeleteSelectedTableRows(RunInfo info, DeleteTablesOptions options) {
			if (!IsSelectedEntireTableRows(info, options) && !options.BackspacePressed)
				return;
			List<TableRow> selectedRows = GetSelectedRows(info);
			if (selectedRows.Count > 0) {
				Table table = selectedRows[0].Table;
				DeleteSelectedTableRows(selectedRows);
				table.NormalizeCellColumnSpans();
			}
		}
		static void DeleteSelectedTableRows(List<TableRow> selectedRows) {
			PieceTable pieceTable = selectedRows[0].PieceTable;
			for (int i = selectedRows.Count - 1; i >= 0; i--) {
				TableRow currentRow = selectedRows[i];
				pieceTable.DeleteEmptyTableRowCore(currentRow.Table.Index, currentRow.IndexInTable);
			}
		}
	}
	#endregion
}
