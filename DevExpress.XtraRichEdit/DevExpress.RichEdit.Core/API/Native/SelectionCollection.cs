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
using System.Linq;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using XtraRichEditModel = DevExpress.XtraRichEdit.Model;
using ModelTableCell = DevExpress.XtraRichEdit.Model.TableCell;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.API.Native {
	public class SelectionCollection : IEnumerable<DocumentRange>, System.Collections.IEnumerable {
		XtraRichEditModel.Selection selection;
		NativeSubDocument document;
		protected internal SelectionCollection(XtraRichEditModel.Selection selection, NativeSubDocument document) {
			this.selection = selection;
			this.document = document;
		}
		public DocumentRange this[int index] {
			get { return CreateDocumentRange(selection.Items[index]); }
			private set { selection.Items[index] = CreateSelectionItem((NativeDocumentRange)value); }
		}
		internal NativeSubDocument Document { get { return document; } }
		XtraRichEditModel.SelectionItem CreateSelectionItem(NativeDocumentRange range) {
			return CreateSelectionItem(range.Start.LogPosition, range.End.LogPosition);
		}
		XtraRichEditModel.SelectionItem CreateSelectionItem(XtraRichEditModel.DocumentLogPosition start, XtraRichEditModel.DocumentLogPosition end) {
			return new XtraRichEditModel.SelectionItem(document.DocumentModel.ActivePieceTable) { Start = start, End = end };
		}
		NativeDocumentRange CreateDocumentRange(XtraRichEditModel.SelectionItem item) {
			return CreateDocumentRange(item.NormalizedStart, item.NormalizedEnd);
		}
		NativeDocumentRange CreateDocumentRange(XtraRichEditModel.DocumentLogPosition start, XtraRichEditModel.DocumentLogPosition end) {
			XtraRichEditModel.DocumentLogPosition normalStart = start < end ? start : end;
			int length = Math.Abs(end - start);
			return (NativeDocumentRange)document.CreateRange(normalStart, length);
		}
		enum SelectionRangeType {
			Single,
			Start,
			Middle,
			End
		}
		public void Add(DocumentRange range) {
			Add(new List<DocumentRange>() { range });
		}
		public void Add(List<DocumentRange> items) {
			if(items == null || items.Count == 0)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_EmptyCollectionException);
			List<List<DocumentRange>> groupedRanges = GetGroupedSortedRanges(items);
			ValidateSelections(groupedRanges);
			document.DocumentModel.BeginUpdate();
			try {
				for(int i = 0; i < groupedRanges.Count; i++) {
					var selections = groupedRanges[i];
					if(selections.Count == 1)
						Add(selections[0], SelectionRangeType.Single);
					else {
						for(int j = 0; j < selections.Count; j++) {
							var item = selections[j];
							SelectionRangeType selElementType = j == 0 ? SelectionRangeType.Start : (j == selections.Count - 1 ? SelectionRangeType.End : SelectionRangeType.Middle);
							Add(item, selElementType);
						}
					}
				}
				XtraRichEditModel.RunIndex startRunIndex = ((NativeDocumentRange)groupedRanges[0][0]).NormalizedStart.Position.RunIndex;
				List<DocumentRange> lastRange = groupedRanges[groupedRanges.Count - 1];
				XtraRichEditModel.RunIndex endRunIndex = ((NativeDocumentRange)lastRange[lastRange.Count - 1]).NormalizedEnd.Position.RunIndex;
				document.DocumentModel.ApplyChangesCore(document.DocumentModel.ActivePieceTable, XtraRichEditModel.DocumentModelChangeActions.ResetSelectionLayout | XtraRichEditModel.DocumentModelChangeActions.RaiseSelectionChanged | XtraRichEditModel.DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
			}
			finally {
				document.DocumentModel.EndUpdate();
			}
		}
		XtraRichEditModel.RunIndex GetStartRunIndex() {
			return new XtraRichEditModel.RunIndex();
		}
		void Add(DocumentRange range, SelectionRangeType selElementType) {
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ValidateSelection(nativeRange, selElementType);
			if(selection.Items.Count == 1 && selection.Items[0].Length == 0)
				selection.Items.Clear();
			selection.AddSelection(CreateSelectionItem(nativeRange));
		}
		void ValidateSelections(List<List<DocumentRange>> groupedItems) {
			ValidateSelections(groupedItems, false);
		}
		void ValidateSelections(List<List<DocumentRange>> groupedItems, bool isRemoved) {
			for(int i = 0; i < groupedItems.Count; i++) {
				List<DocumentRange> selections = groupedItems[i];
				for(int j = 0; j < selections.Count; j++) {
					DocumentRange item = selections[j];
					for(int k = j + 1; k < selections.Count; k++) {
						var item2 = selections[k];
						if(AreSelectionItemsIntersected((NativeDocumentRange)item, (NativeDocumentRange)item2))
							RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SpecifiedSelectionsIntersectException);
					}
					for(int k = i + 1; k < groupedItems.Count; k++) {
						List<DocumentRange> selections2 = groupedItems[k];
						for(int l = 0; l < selections2.Count; l++) {
							var item2 = selections2[l];
							if(AreSelectionItemsIntersected((NativeDocumentRange)item, (NativeDocumentRange)item2))
								RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SpecifiedSelectionsIntersectException);
						}
					}
					SelectionRangeType selElementType = SelectionRangeType.Single;
					if(selections.Count != 1)
						selElementType = j == 0 ? SelectionRangeType.Start : (j == selections.Count - 1 ? SelectionRangeType.End : SelectionRangeType.Middle);
					ValidateSelection((NativeDocumentRange)item, selElementType, isRemoved);
				}
			}
		}
		List<DocumentRange> SortRanges(List<DocumentRange> items) {
			List<DocumentRange> result = new List<DocumentRange>(items);
			result.Sort((a, b) => ((NativeDocumentRange)a).NormalizedStart.CompareToCore(((NativeDocumentRange)b).NormalizedStart));
			return result;
		}
		List<DocumentRange> MergeNeighbourhoodRanges(List<DocumentRange> sortedRanges) {
			List<DocumentRange> result = new List<DocumentRange>();
			DocumentRange prevRange = null;
			for(int i = 0; i < sortedRanges.Count; i++) {
				if(prevRange != null) {
					DocumentRange range = sortedRanges[i];
					if(((NativeDocumentRange)prevRange).NormalizedEnd == ((NativeDocumentRange)range).NormalizedStart) {
						prevRange = CreateDocumentRange(((NativeDocumentRange)prevRange).NormalizedStart.LogPosition, ((NativeDocumentRange)range).NormalizedEnd.LogPosition);
						continue;
					}
					else
						result.Add(prevRange);
				}
				prevRange = sortedRanges[i];
			}
			if(prevRange != null)
				result.Add(prevRange);
			return result;
		}
		List<DocumentRange> SplitRanges(List<DocumentRange> sortedRanges) {
			List<DocumentRange> result = new List<DocumentRange>();
			List<DocumentRange> partiallySelectedCells = new List<DocumentRange>();
			List<DocumentRange> splittedRangesByRow = SplitRangesByRowWithChildTables(sortedRanges);
			do {
				partiallySelectedCells.Clear();
				for(int i = 0; i < splittedRangesByRow.Count; i++) {
					NativeDocumentRange range = (NativeDocumentRange)splittedRangesByRow[i];
					ModelTableCell startCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedStart.LogPosition);
					ModelTableCell endCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
					ModelTableCell normalStartCell = null;
					ModelTableCell normalEndCell = null;
					NormalizeCells(startCell, endCell, out normalStartCell, out normalEndCell);
					if(normalStartCell == null || normalEndCell == null || normalStartCell == normalEndCell) {
						result.Add(range);
						continue;
					}
					XtraRichEditModel.DocumentLogPosition endLogPos = range.NormalizedEnd.LogPosition;
					ModelTableCell lastSelectedCell = GetLastSelectedCell(normalStartCell, normalEndCell, range.NormalizedEnd.LogPosition);
					if(normalEndCell != lastSelectedCell) {
						result.Add(range);
						continue;
					}
					XtraRichEditModel.DocumentLogPosition endCellStartLogPos = document.DocumentModel.ActivePieceTable.Paragraphs[normalEndCell.StartParagraphIndex].LogPosition;
					NativeDocumentRange firstRangePart = CreateDocumentRange(range.NormalizedStart.LogPosition, endCellStartLogPos);
					NativeDocumentRange secondRangePart = CreateDocumentRange(endCellStartLogPos, endLogPos);
					result.Add(firstRangePart);
					partiallySelectedCells.Add(secondRangePart);
				}
				splittedRangesByRow = SplitRangesByRowWithChildTables(partiallySelectedCells);
			} while(partiallySelectedCells.Count != 0);
			return SortRanges(result);
		}
		List<DocumentRange> SplitRangesByRowWithChildTables(List<DocumentRange> sortedRanges) {
			List<DocumentRange> splittedRangesByRow = sortedRanges;
			int prevRangesCount = 0;
			do {
				prevRangesCount = splittedRangesByRow.Count;
				splittedRangesByRow = SplitRangesByRow_SameTable(splittedRangesByRow);
			} while(prevRangesCount != splittedRangesByRow.Count);
			return splittedRangesByRow;
		}
		List<DocumentRange> SplitRangesByRow_SameTable(List<DocumentRange> sortedRanges) {
			List<DocumentRange> result = new List<DocumentRange>();
			for(int i = 0; i < sortedRanges.Count; i++) {
				NativeDocumentRange range = (NativeDocumentRange)sortedRanges[i];
				ModelTableCell startCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedStart.LogPosition);
				ModelTableCell endCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
				if(startCell == endCell) {
					result.Add(range);
					continue;
				}
				ModelTableCell normalStartCell = null;
				ModelTableCell normalEndCell = null;
				NormalizeCells(startCell, endCell, out normalStartCell, out normalEndCell);
				ModelTableCell lastSelectedCell = GetLastSelectedCell(normalStartCell, normalEndCell, range.NormalizedEnd.LogPosition);
				if(normalStartCell == lastSelectedCell) {
					result.Add(range);
					continue;
				}
				if(normalStartCell != null && lastSelectedCell != null) {
					if(normalStartCell.Table == lastSelectedCell.Table) {
						int startCellRowIndex = normalStartCell.RowIndex;
						int endCellRowIndex = lastSelectedCell.RowIndex;
						if(startCellRowIndex != endCellRowIndex) {
							SplitRows(range, normalStartCell.Table, startCellRowIndex + 1, endCellRowIndex, result);
							continue;
						}
					}
					else if(normalStartCell.Table.ParentCell == lastSelectedCell.Table.ParentCell) {
						int startCellRowIndex = normalStartCell.RowIndex;
						XtraRichEditModel.TableRowCollection firstTableRows = normalStartCell.Table.Rows;
						SplitRows(range, normalStartCell.Table, startCellRowIndex + 1, firstTableRows.Count - 1, result);
						ModelTableCell lastRangeFirstCell = null;
						do {
							NativeDocumentRange lastRange = (NativeDocumentRange)result[result.Count - 1];
							result.RemoveAt(result.Count - 1);
							lastRangeFirstCell = GetCell(document.DocumentModel.ActivePieceTable, lastRange.NormalizedStart.LogPosition);
							if(lastRangeFirstCell == null || lastRangeFirstCell.Table.NestedLevel < normalStartCell.Table.NestedLevel) { 
								XtraRichEditModel.DocumentLogPosition endLogPos = document.DocumentModel.ActivePieceTable.Paragraphs[lastSelectedCell.Table.FirstRow.FirstCell.StartParagraphIndex].LogPosition;
								DocumentRange newRange = CreateDocumentRange(lastRange.NormalizedStart.LogPosition, endLogPos);
								result.Add(newRange);
								lastRange = CreateDocumentRange(endLogPos, lastRange.NormalizedEnd.LogPosition);
								lastRangeFirstCell = lastSelectedCell;
							}
							else
								lastRangeFirstCell = GetParentCellWithNestedLevel(lastRangeFirstCell, normalStartCell.Table.NestedLevel);
							int endCellRowIndex = lastRangeFirstCell.Table == lastSelectedCell.Table ? lastSelectedCell.RowIndex : lastRangeFirstCell.Table.Rows.Count - 1;
							SplitRows(lastRange, lastRangeFirstCell.Table, 1, endCellRowIndex, result);
						} while(lastRangeFirstCell.Table != lastSelectedCell.Table);
						continue;
					}
					else if(normalStartCell == lastSelectedCell.Table.ParentCell) {
						SplitRows(range, lastSelectedCell.Table, 0, lastSelectedCell.RowIndex, result);
						continue;
					}
					else if(normalStartCell.Table.ParentCell == lastSelectedCell) {
						int startCellRowIndex = normalStartCell.RowIndex;
						XtraRichEditModel.TableRowCollection tableRows = normalStartCell.Table.Rows;
						SplitRows(range, normalStartCell.Table, startCellRowIndex + 1, tableRows.Count - 1, result);
						continue;
					}
				}
				else if(normalStartCell != null && lastSelectedCell == null) {
					int startCellRowIndex = normalStartCell.RowIndex;
					XtraRichEditModel.TableRowCollection tableRows = normalStartCell.Table.Rows;
					SplitRows(range, normalStartCell.Table, startCellRowIndex + 1, tableRows.Count - 1, result);
					continue;
				}
				else if(normalStartCell == null && lastSelectedCell != null) {
					SplitRows(range, lastSelectedCell.Table, 0, lastSelectedCell.RowIndex, result);
					continue;
				}
				result.Add(range);
			}
			return result;
		}
		ModelTableCell GetLastSelectedCell(ModelTableCell startCell, ModelTableCell endCell, XtraRichEditModel.DocumentLogPosition endLogPos) {
			if(endCell == null)
				return GetCell(document.DocumentModel.ActivePieceTable, endLogPos - 1);
			ModelTableCell lastSelectedCell = null;
			if(!IsLogPosSelectAnotherCell(endCell, endLogPos))
				return endCell;
			lastSelectedCell = GetCell(document.DocumentModel.ActivePieceTable, endLogPos - 1);
			if(lastSelectedCell != null && startCell != null) {
				ModelTableCell tempCell = null;
				NormalizeCells(startCell, lastSelectedCell, out tempCell, out lastSelectedCell);
			}
			return lastSelectedCell;
		}
		void SplitRows(NativeDocumentRange range, XtraRichEditModel.Table table, int startRowIndex, int endRowIndex, List<DocumentRange> ranges) {
			XtraRichEditModel.TableRowCollection rows = table.Rows;
			XtraRichEditModel.DocumentLogPosition startLogPos = range.NormalizedStart.LogPosition;
			for(int i = startRowIndex; i <= endRowIndex; i++) {
				XtraRichEditModel.TableRow row = rows[i];
				XtraRichEditModel.DocumentLogPosition endLogPos = document.DocumentModel.ActivePieceTable.Paragraphs[row.FirstCell.StartParagraphIndex].LogPosition;
				DocumentRange newRange = CreateDocumentRange(startLogPos, endLogPos);
				ranges.Add(newRange);
				startLogPos = endLogPos;
			}
			if(startLogPos != range.NormalizedEnd.LogPosition) {
				XtraRichEditModel.DocumentLogPosition afterTableLogPos = document.DocumentModel.ActivePieceTable.Paragraphs[rows.Last.LastCell.EndParagraphIndex].EndLogPosition + 1;
				if(afterTableLogPos < range.NormalizedEnd.LogPosition) {
					DocumentRange newRange = CreateDocumentRange(startLogPos, afterTableLogPos);
					ranges.Add(newRange);
					startLogPos = afterTableLogPos;
				}
				DocumentRange lastRange = CreateDocumentRange(startLogPos, range.NormalizedEnd.LogPosition);
				ranges.Add(lastRange);
			}
		}
		List<List<DocumentRange>> GetGroupedSortedRanges(List<DocumentRange> ranges) {
			List<DocumentRange> sortedRanges = SortRanges(ranges);
			CorrectEndLogPosByEndOfDocument(sortedRanges);
			List<DocumentRange> sortedMergedRanges = MergeNeighbourhoodRanges(sortedRanges);
			RemoveRangesWithZeroWidth(sortedMergedRanges);
			List<DocumentRange> sortedCorrectedRanges = SplitRanges(sortedMergedRanges);
			List<List<DocumentRange>> groupedSortedRanges = new List<List<DocumentRange>>();
			int selectedCellsCount = 0;
			int selectedColumnsCount = 0;
			int i = 0;
			while(i < sortedCorrectedRanges.Count) {
				NativeDocumentRange range = (NativeDocumentRange)sortedCorrectedRanges[i];
				ModelTableCell startPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedStart.LogPosition);
				ModelTableCell endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
				if(endPosCell != null) {
					if(document.DocumentModel.ActivePieceTable.Paragraphs[endPosCell.StartParagraphIndex].LogPosition == range.NormalizedEnd.LogPosition)
						endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition - 1);
				}
				else if(!IsLogPos_EndOfDoc(range.NormalizedEnd.LogPosition)) {
					var docModelEndPos = PositionConverter.ToDocumentModelPosition(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
					XtraRichEditModel.Paragraph endPosParagraph = document.DocumentModel.ActivePieceTable.Paragraphs[docModelEndPos.ParagraphIndex];
					if(endPosParagraph.LogPosition == range.NormalizedEnd.LogPosition)
						endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition - 1);
				}
				ModelTableCell normalizedStartCell = null;
				ModelTableCell normalizedEndCell = null;
				NormalizeCells(startPosCell, endPosCell, out normalizedStartCell, out normalizedEndCell);
				List<DocumentRange> currentGroup = new List<DocumentRange>() { range };
				groupedSortedRanges.Add(currentGroup);
				if(!SameRowCell(normalizedStartCell, normalizedEndCell)) {
					i++;
					continue;
				}
				CalcColumnsAndCellsCount(normalizedStartCell.Row, normalizedStartCell.IndexInRow, normalizedEndCell.IndexInRow, out selectedColumnsCount, out selectedCellsCount);
				i = FillGroup(sortedCorrectedRanges, i, normalizedStartCell, selectedCellsCount, selectedColumnsCount, currentGroup);
			}
			return groupedSortedRanges;
		}
		int FillGroup(List<DocumentRange> sortedRanges, int prevRangeIndex, ModelTableCell prevStartCell, int selectedCellsCount, int selectedColumnsCount, List<DocumentRange> currentGroup) {
			int prevCellIndexInRow = prevStartCell.IndexInRow;
			int selectedColumnsCountInCurrentRow = 0;
			int selectedCellsCountInCurrentRow = 0;
			for(int i = prevRangeIndex + 1; i < sortedRanges.Count; i++) {
				NativeDocumentRange range = (NativeDocumentRange)sortedRanges[i];
				ModelTableCell startPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedStart.LogPosition);
				ModelTableCell endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
				if(endPosCell != null) {
					if(document.DocumentModel.ActivePieceTable.Paragraphs[endPosCell.StartParagraphIndex].LogPosition == range.NormalizedEnd.LogPosition)
						endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition - 1);
				}
				else if(!IsLogPos_EndOfDoc(range.NormalizedEnd.LogPosition)) {
					var docModelEndPos = PositionConverter.ToDocumentModelPosition(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition);
					XtraRichEditModel.Paragraph endPosParagraph = document.DocumentModel.ActivePieceTable.Paragraphs[docModelEndPos.ParagraphIndex];
					if(endPosParagraph.LogPosition == range.NormalizedEnd.LogPosition)
						endPosCell = GetCell(document.DocumentModel.ActivePieceTable, range.NormalizedEnd.LogPosition - 1);
				}
				ModelTableCell normalizedStartCell = null;
				ModelTableCell normalizedEndCell = null;
				NormalizeCells(startPosCell, endPosCell, out normalizedStartCell, out normalizedEndCell);
				if(!SameRowCell(normalizedStartCell, normalizedEndCell))
					return i;
				ModelTableCell normalizedPrevStartCell = null;
				ModelTableCell normalizedCurrStartCell = null;
				NormalizeCells(prevStartCell, normalizedStartCell, out normalizedPrevStartCell, out normalizedCurrStartCell);
				if(normalizedPrevStartCell.Table != normalizedCurrStartCell.Table)
					return i;
				if(normalizedStartCell.RowIndex - prevStartCell.RowIndex != 1)
					return i;
				int normStartCellIndexInRow = normalizedStartCell.IndexInRow;
				CalcColumnsAndCellsCount(normalizedStartCell.Row, normStartCellIndexInRow, normalizedEndCell.IndexInRow, out selectedColumnsCountInCurrentRow, out selectedCellsCountInCurrentRow);
				if(selectedCellsCount != selectedCellsCountInCurrentRow && selectedColumnsCount != selectedColumnsCountInCurrentRow)
					return i;
				if(prevCellIndexInRow != normStartCellIndexInRow)
					return i;
				currentGroup.Add(range);
				prevStartCell = normalizedStartCell;
			}
			return sortedRanges.Count;
		}
		void CorrectEndLogPosByEndOfDocument(List<DocumentRange> ranges) {
			if(ranges == null || ranges.Count == 0)
				return;
			XtraRichEditModel.DocumentLogPosition docEndLogPos = selection.PieceTable.Paragraphs.Last.EndLogPosition;
			if(document.DocumentModel.MainPieceTable == selection.PieceTable)
				docEndLogPos = document.Paragraphs.Last().Range.End.LogPosition;
			for(int i = 0; i < ranges.Count; i++) {
				var range = ranges[i];
				if(docEndLogPos < range.Start.LogPosition || docEndLogPos < range.End.LogPosition) {
					XtraRichEditModel.DocumentLogPosition startLogPos = docEndLogPos < range.Start.LogPosition ? docEndLogPos : range.Start.LogPosition;
					XtraRichEditModel.DocumentLogPosition endLogPos = docEndLogPos < range.End.LogPosition ? docEndLogPos : range.End.LogPosition;
					ranges[i] = CreateDocumentRange(startLogPos, endLogPos);
				}
			}
		}
		void RemoveRangesWithZeroWidth(List<DocumentRange> ranges) {
			if(ranges == null || ranges.Count == 0)
				return;
			for(int i = ranges.Count - 1; i >= 0; i--) {
				if(ranges[i].Length == 0)
					ranges.RemoveAt(i);
			}
		}
		void CalcColumnsAndCellsCount(XtraRichEditModel.TableRow row, int startIndexInRow, int endIndexInRow, out int columnsCount, out int cellsCount) {
			columnsCount = 0;
			for(int j = startIndexInRow; j <= endIndexInRow; j++)
				columnsCount += row.Cells[j].ColumnSpan;
			cellsCount = endIndexInRow - startIndexInRow;
		}
		bool SameRowCell(ModelTableCell cell1, ModelTableCell cell2) {
			return cell1 != null && cell2 != null && cell1.Row == cell2.Row;
		}
		void NormalizeCells(ModelTableCell cell1, ModelTableCell cell2, out ModelTableCell normalizedCell1, out ModelTableCell normalizedCell2) {
			if(cell1 == null && cell2 == null) {
				normalizedCell1 = null;
				normalizedCell2 = null;
				return;
			}
			if(cell1 != null && cell2 == null) {
				normalizedCell1 = GetParentCellWithNestedLevel(cell1, 0);
				normalizedCell2 = null;
				return;
			}
			if(cell1 == null && cell2 != null) {
				normalizedCell1 = null;
				normalizedCell2 = GetParentCellWithNestedLevel(cell2, 0);
				return;
			}
			if(cell1.Table == cell2.Table) {
				normalizedCell1 = cell1;
				normalizedCell2 = cell2;
				return;
			}
			var baseParentTable = GetContainerTable(cell1, cell2);
			if(baseParentTable != null) {
				if(IsParent(cell1, cell2)) {
					normalizedCell1 = cell1;
					normalizedCell2 = GetParentCellWithNestedLevel(cell2, cell1.Table.NestedLevel + 1);
				}
				else if(IsParent(cell2, cell1)) {
					normalizedCell2 = cell2;
					normalizedCell1 = GetParentCellWithNestedLevel(cell1, cell2.Table.NestedLevel + 1);
				}
				else {
					normalizedCell1 = GetParentCellWithNestedLevel(cell1, baseParentTable.NestedLevel);
					normalizedCell2 = GetParentCellWithNestedLevel(cell2, baseParentTable.NestedLevel);
					if(normalizedCell1 == normalizedCell2) {
						normalizedCell1 = GetParentCellWithNestedLevel(cell1, baseParentTable.NestedLevel + 1);
						normalizedCell2 = GetParentCellWithNestedLevel(cell2, baseParentTable.NestedLevel + 1);
					}
				}
				return;
			}
			normalizedCell1 = GetParentCellWithNestedLevel(cell1, 0);
			normalizedCell2 = GetParentCellWithNestedLevel(cell2, 0);
		}
		bool IsParent(ModelTableCell parent, ModelTableCell child) {
			if(parent == child || parent.Table == child.Table || child.Table.NestedLevel <= parent.Table.NestedLevel)
				return false;
			while(child.Table.NestedLevel > parent.Table.NestedLevel) {
				if(child.Table.ParentCell == parent)
					return true;
				child = child.Table.ParentCell;
			}
			return false;
		}
		ModelTableCell GetCell(XtraRichEditModel.PieceTable pieceTable, XtraRichEditModel.DocumentLogPosition logPos) {
			if(IsLogPos_EndOfDoc(logPos))
				return null;
			var docModelPos = PositionConverter.ToDocumentModelPosition(pieceTable, logPos);
			return pieceTable.Paragraphs[docModelPos.ParagraphIndex].GetCell();
		}
		bool IsLogPos_EndOfDoc(XtraRichEditModel.DocumentLogPosition logPos) {
			return document.Paragraphs.Last().Range.End.LogPosition == logPos;
		}
		void ValidateSelection(NativeDocumentRange range, bool isRemoved) {
			ValidateSelection(range, SelectionRangeType.Single, isRemoved);
		}
		void ValidateSelection(NativeDocumentRange range, SelectionRangeType selElementType) {
			ValidateSelection(range, selElementType, false);
		}
		void ValidateSelection(NativeDocumentRange range, SelectionRangeType selElementType, bool isRemoved) {
			if(range.Start == range.End && selection.Items.Count != 0)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SelectionShouldContainAtLeastOneCharacterException);
			if(!isRemoved) {
				foreach(var sel in selection.Items) {
					if(sel.Length == 0)
						continue;
					NativeDocumentRange selRange = new NativeDocumentRange(document, new XtraRichEditModel.DocumentModelPosition(sel.PieceTable) { LogPosition = sel.NormalizedStart }, new XtraRichEditModel.DocumentModelPosition(sel.PieceTable) { LogPosition = sel.NormalizedEnd });
					if(AreSelectionItemsIntersected(selRange, range))
						RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_CurrentSelectionAndSpecifiedSelectionIntersectException);
				}
			}
			XtraRichEditModel.DocumentLogPosition startLogPos = range.NormalizedStart.LogPosition;
			XtraRichEditModel.DocumentLogPosition endLogPos = range.NormalizedEnd.LogPosition;
			ModelTableCell startPosCell = GetCell(document.DocumentModel.ActivePieceTable, startLogPos);
			ModelTableCell endPosCell = GetCell(document.DocumentModel.ActivePieceTable, endLogPos);
			if(startPosCell == endPosCell)
				return;
			ModelTableCell normalizedStartCell = null;
			ModelTableCell normalizedEndCell = null;
			NormalizeCells(startPosCell, endPosCell, out normalizedStartCell, out normalizedEndCell);
			ModelTableCell lastSelectedCell = GetLastSelectedCell(normalizedStartCell, normalizedEndCell, endLogPos);
			if(ShouldSkipValidation(normalizedStartCell, normalizedEndCell, lastSelectedCell))
				return;
			if(normalizedEndCell != lastSelectedCell)
				normalizedEndCell = null;
			if(normalizedStartCell != null && lastSelectedCell != null) {
				if(normalizedStartCell.Table == lastSelectedCell.Table) {
					Validate_CellsWithinSameTable(normalizedStartCell, normalizedEndCell, startLogPos, endLogPos, selElementType);
					return;
				}
				else if(normalizedStartCell.Table.ParentCell == lastSelectedCell.Table.ParentCell) {
					Validate_CellsWithinDifferentTables(normalizedStartCell, normalizedEndCell, startLogPos, endLogPos);
					return;
				}
				else if(normalizedStartCell == lastSelectedCell.Table.ParentCell) {
					Validate_TableCell_TextBeforeTable(normalizedEndCell, endLogPos);
					return;
				}
				else if(normalizedStartCell.Table.ParentCell == lastSelectedCell) {
					Validate_TableCell_TextAfterTable(normalizedStartCell, startLogPos);
					return;
				}
			}
			else if(normalizedStartCell != null && lastSelectedCell == null)
				Validate_TableCell_TextAfterTable(normalizedStartCell, startLogPos);
			else if(normalizedStartCell == null && lastSelectedCell != null)
				Validate_TableCell_TextBeforeTable(normalizedEndCell, endLogPos);
		}
		bool ShouldSkipValidation(ModelTableCell start, ModelTableCell end, ModelTableCell lastSelected) {
			if(start == null && end == null && lastSelected == null)
				return true;
			if(start == lastSelected && end != null && end.Table.ParentCell == start)
				return true;
			return false;
		}
		bool IsLogPosSelectAnotherCell(ModelTableCell cell, XtraRichEditModel.DocumentLogPosition logPos) {
			ModelTableCell prevPosCell = GetCell(document.DocumentModel.ActivePieceTable, logPos - 1);
			if(cell == null)
				return prevPosCell != null;
			if(document.DocumentModel.ActivePieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition == logPos)
				return true;
			ModelTableCell tempCell = null;
			NormalizeCells(cell, prevPosCell, out tempCell, out prevPosCell);
			if(cell != prevPosCell)
				return true;
			return false;
		}
		XtraRichEditModel.Table GetContainerTable(ModelTableCell cell1, ModelTableCell cell2) {
			if(cell1 == null || cell2 == null)
				return null;
			if(cell1.Table.NestedLevel != cell2.Table.NestedLevel) {
				if(cell1.Table.NestedLevel < cell2.Table.NestedLevel)
					cell2 = GetParentCellWithNestedLevel(cell2, cell1.Table.NestedLevel);
				else
					cell1 = GetParentCellWithNestedLevel(cell1, cell2.Table.NestedLevel);
			}
			while(cell1.Table != cell2.Table) {
				if(cell1.Table.ParentCell == null)
					break;
				cell1 = cell1.Table.ParentCell;
				cell2 = cell2.Table.ParentCell;
			}
			if(cell1.Table == cell2.Table)
				return cell1.Table;
			return null;
		}
		ModelTableCell GetParentCellWithNestedLevel(ModelTableCell cell, int nestedLvl) {
			if(cell == null || nestedLvl < 0)
				return null;
			if(cell.Table.NestedLevel < nestedLvl)
				return null;
			while(cell.Table.NestedLevel > nestedLvl)
				cell = cell.Table.ParentCell;
			return cell;
		}
		void Validate_TableCell_TextAfterTable(ModelTableCell selectedCell, XtraRichEditModel.DocumentLogPosition selStartLogPos) {
			ModelTableCell firstNormalCellInRow = GetFirstNormalizedCellInRow(selectedCell.Row);
			bool isCellPartiallySel = IsCellPartiallySelected(selectedCell, selStartLogPos);
			if(firstNormalCellInRow != selectedCell) {
				if(selectedCell.VerticalMerging == XtraRichEditModel.MergingState.Continue)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_FirstCellContinuesVerticalMergeException);
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SelectionExtendsOutsideTableException);
			}
			if(isCellPartiallySel)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_PartiallySelectedCellsException);
		}
		void Validate_TableCell_TextBeforeTable(ModelTableCell selectedCell, XtraRichEditModel.DocumentLogPosition selEndLogPos) {
			Validate_TableCell_TextBeforeTable(selectedCell, selEndLogPos, null);
		}
		void Validate_TableCell_TextBeforeTable(ModelTableCell selectedCell, XtraRichEditModel.DocumentLogPosition selEndLogPos, ModelTableCell cellForNormalize) {
			ModelTableCell lastTableCell = null;
			if(selectedCell == null) {
				lastTableCell = GetCell(document.DocumentModel.ActivePieceTable, selEndLogPos - 1);
				if(cellForNormalize != null) {
					ModelTableCell normCell = null;
					NormalizeCells(cellForNormalize, lastTableCell, out normCell, out lastTableCell);
				}
			}
			bool isSelectedCellPartiallySelected = selectedCell == null ? false : IsCellPartiallySelected(selectedCell, selEndLogPos);
			if(isSelectedCellPartiallySelected)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_PartiallySelectedCellsException);
			ModelTableCell lastNormCellInRow = selectedCell != null ? GetLastNormalizedCellInRow(selectedCell.Previous.Row) : GetLastNormalizedCellInRow(lastTableCell.Row);
			if(selectedCell != null) {
				if(selectedCell.Previous.VerticalMerging == XtraRichEditModel.MergingState.Continue)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_LastCellContinuesVerticalMergeException);
				if(lastNormCellInRow != selectedCell.Previous)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SelectionExtendsOutsideTableException);
			}
			else {
				if(lastTableCell.VerticalMerging == XtraRichEditModel.MergingState.Continue)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_LastCellContinuesVerticalMergeException);
				if(lastNormCellInRow != lastTableCell)
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SelectionExtendsOutsideTableException);
			}
		}
		void Validate_CellsWithinSameTable(ModelTableCell firstSelectedCell, ModelTableCell lastSelectedCell, XtraRichEditModel.DocumentLogPosition selStartLogPos, XtraRichEditModel.DocumentLogPosition selEndLogPos, SelectionRangeType selElementType) {
			bool isStartCellPartiallySelected = IsCellPartiallySelected(firstSelectedCell, selStartLogPos);
			ModelTableCell lastTableCell = null;
			if(lastSelectedCell == null)
				lastTableCell = GetCell(document.DocumentModel.ActivePieceTable, selEndLogPos - 1);
			bool isEndCellPartiallySelected = lastSelectedCell == null ? false : IsCellPartiallySelected(lastSelectedCell, selEndLogPos);
			int startCellRowIndex = firstSelectedCell.RowIndex;
			int endCellRowIndex = lastSelectedCell == null ? lastTableCell.RowIndex : lastSelectedCell.RowIndex;
			if(lastSelectedCell != null && lastSelectedCell.IsFirstCellInRow && !isEndCellPartiallySelected)
				endCellRowIndex--;
			if(startCellRowIndex != endCellRowIndex)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_SelecitonShouldIncludeNotMoreThanOneRowException);
			if(firstSelectedCell.VerticalMerging == XtraRichEditModel.MergingState.Continue && (selElementType == SelectionRangeType.Start || selElementType == SelectionRangeType.Single))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_FirstCellContinuesVerticalMergeException);
			if(isStartCellPartiallySelected || isEndCellPartiallySelected)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_PartiallySelectedCellsException);
			if(lastSelectedCell != null) {
				if(lastSelectedCell.Previous.VerticalMerging == XtraRichEditModel.MergingState.Continue && (selElementType == SelectionRangeType.End || selElementType == SelectionRangeType.Single))
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_LastCellContinuesVerticalMergeException);
			}
			else if(lastSelectedCell == null) {
				if(lastTableCell.VerticalMerging == XtraRichEditModel.MergingState.Continue && (selElementType == SelectionRangeType.End || selElementType == SelectionRangeType.Single))
					RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_LastCellContinuesVerticalMergeException);
			}
		}
		void Validate_CellsWithinDifferentTables(ModelTableCell firstSelectedCell, ModelTableCell lastSelectedCell, XtraRichEditModel.DocumentLogPosition selStartLogPos, XtraRichEditModel.DocumentLogPosition selEndLogPos) {
			Validate_TableCell_TextAfterTable(firstSelectedCell, selStartLogPos);
			ModelTableCell cellForNormalize = lastSelectedCell == null ? firstSelectedCell : null;
			Validate_TableCell_TextBeforeTable(lastSelectedCell, selEndLogPos, cellForNormalize);
		}
		bool IsCellPartiallySelected(ModelTableCell cell, XtraRichEditModel.DocumentLogPosition logPos) {
			if(cell == null)
				return false;
			return cell.PieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition != logPos;
		}
		ModelTableCell GetFirstNormalizedCellInRow(XtraRichEditModel.TableRow row) { 
			if(row == null)
				return null;
			ModelTableCell result = row.FirstCell;
			while(result != null) {
				if(result.VerticalMerging != XtraRichEditModel.MergingState.Continue)
					return result;
				result = result.NextCellInRow;
			}
			return null;
		}
		ModelTableCell GetLastNormalizedCellInRow(XtraRichEditModel.TableRow row) { 
			if(row == null)
				return null;
			ModelTableCell result = row.LastCell;
			while(result != null) {
				if(result.VerticalMerging != XtraRichEditModel.MergingState.Continue)
					return result;
				result = GetPreviousCellInRow(result);
			}
			return null;
		}
		ModelTableCell GetPreviousCellInRow(ModelTableCell cell) {
			if(cell == null)
				return null;
			ModelTableCell previousCell = cell.Previous;
			if(previousCell != null && previousCell.Row == cell.Row)
				return previousCell;
			return null;
		}
		bool AreSelectionItemsIntersected(NativeDocumentRange item1, NativeDocumentRange item2) {
			if(item1.NormalizedStart == item1.NormalizedEnd) {
				if(item2.NormalizedStart < item1.NormalizedStart && item1.NormalizedStart < item2.NormalizedEnd)
					return true;
				return false;
			}
			if(item1.NormalizedStart <= item2.NormalizedStart && item2.NormalizedStart < item1.NormalizedEnd)
				return true;
			if(item1.NormalizedStart < item2.NormalizedEnd && item2.NormalizedEnd <= item1.NormalizedEnd)
				return true;
			if(item2.NormalizedStart <= item1.NormalizedStart && item1.NormalizedEnd <= item2.NormalizedEnd)
				return true;
			return false;
		}
		public void Clear() {
			if(selection.Items.Count == 0)
				Exceptions.ThrowInternalException();
			document.DocumentModel.BeginUpdate();
			try {
				XtraRichEditModel.RunIndex startRunIndex = selection.Items[0].Interval.NormalizedStart.RunIndex;
				XtraRichEditModel.RunIndex endRunIndex = selection.Items[selection.Items.Count - 1].Interval.NormalizedEnd.RunIndex;
				selection.Items.RemoveRange(1, Count - 1);
				selection.Items[0].End = selection.Items[0].Start;
				document.DocumentModel.ApplyChangesCore(document.DocumentModel.ActivePieceTable, XtraRichEditModel.DocumentModelChangeActions.ResetSelectionLayout | XtraRichEditModel.DocumentModelChangeActions.RaiseSelectionChanged | XtraRichEditModel.DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
			}
			finally {
				document.DocumentModel.EndUpdate();
			}
		}
		public int Count { get { return selection.Items.Count; } }
		public void RemoveAt(int index) {
			if(index < 0 || index >= selection.Items.Count)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_OutOfRangeException);
			if(selection.Items.Count == 1 && selection.Items[index].Length == 0)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_CannotRemoveCaretException);
			if(selection.Items.Count == 1) {
				document.DocumentModel.BeginUpdate();
				try {
					XtraRichEditModel.RunIndex startRunIndex = selection.Items[0].Interval.NormalizedStart.RunIndex;
					XtraRichEditModel.RunIndex endRunIndex = selection.Items[0].Interval.NormalizedEnd.RunIndex;
					selection.Items[0].End = selection.Items[0].Start;
					document.DocumentModel.ApplyChangesCore(document.DocumentModel.ActivePieceTable, XtraRichEditModel.DocumentModelChangeActions.ResetSelectionLayout | XtraRichEditModel.DocumentModelChangeActions.RaiseSelectionChanged | XtraRichEditModel.DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
				}
				finally {
					document.DocumentModel.EndUpdate();
				}
				return;
			}
			List<DocumentRange> clone = new List<DocumentRange>();
			foreach(var item in selection.Items)
				clone.Add(CreateDocumentRange(item));
			clone.RemoveAt(index);
			var groupedItems = GetGroupedSortedRanges(clone);
			ValidateSelections(groupedItems, true);
			clone.Clear();
			groupedItems.Clear();
			document.DocumentModel.BeginUpdate();
			try {
				XtraRichEditModel.RunIndex startRunIndex = selection.Items[index].Interval.NormalizedStart.RunIndex;
				XtraRichEditModel.RunIndex endRunIndex = selection.Items[index].Interval.NormalizedEnd.RunIndex;
				selection.Items.RemoveAt(index);
				document.DocumentModel.ApplyChangesCore(document.DocumentModel.ActivePieceTable, XtraRichEditModel.DocumentModelChangeActions.ResetSelectionLayout | XtraRichEditModel.DocumentModelChangeActions.RaiseSelectionChanged | XtraRichEditModel.DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
			}
			finally {
				document.DocumentModel.EndUpdate();
			}
		}
		public void Unselect(DocumentRange range) {
			if(selection.Items.Count == 0)
				Exceptions.ThrowInternalException();
			if(selection.Items.Count == 1 && selection.Items[0].Length == 0)
				return;
			if(range.Length == 0)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.SelectionCollection_RangeCannotBeEmptyException);
			List<XtraRichEditModel.SelectionItem> cloneCollection = CloneCollectionByValue(selection.Items);
			Unselect(cloneCollection, (NativeDocumentRange)range);
			List<DocumentRange> ranges = new List<DocumentRange>();
			foreach(var item in cloneCollection)
				ranges.Add(CreateDocumentRange(item));
			var groupedItems = GetGroupedSortedRanges(ranges);
			ValidateSelections(groupedItems, true);
			ranges.Clear();
			groupedItems.Clear();
			document.DocumentModel.BeginUpdate();
			try {
				XtraRichEditModel.RunIndex startRunIndex = new XtraRichEditModel.RunIndex();
				XtraRichEditModel.RunIndex endRunIndex = new XtraRichEditModel.RunIndex();
				if(cloneCollection.Count == 0) {
					startRunIndex = selection.Items[0].Interval.NormalizedStart.RunIndex;
					endRunIndex = selection.Items[selection.Items.Count - 1].Interval.NormalizedEnd.RunIndex;
					selection.Items.RemoveRange(1, selection.Items.Count - 1);
					selection.Items[0].End = selection.Items[0].Start;
				}
				else {
					startRunIndex = selection.Items[0].Interval.NormalizedStart.RunIndex;
					endRunIndex = selection.Items[selection.Items.Count - 1].Interval.NormalizedEnd.RunIndex;
					selection.Items.Clear();
					foreach(var item in cloneCollection)
						selection.AddSelection(item);
				}
				document.DocumentModel.ApplyChangesCore(document.DocumentModel.ActivePieceTable, XtraRichEditModel.DocumentModelChangeActions.ResetSelectionLayout | XtraRichEditModel.DocumentModelChangeActions.RaiseSelectionChanged | XtraRichEditModel.DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
			}
			finally {
				document.DocumentModel.EndUpdate();
			}
		}
		private List<XtraRichEditModel.SelectionItem> CloneCollectionByValue(List<XtraRichEditModel.SelectionItem> collection) {
			List<XtraRichEditModel.SelectionItem> clone = new List<XtraRichEditModel.SelectionItem>();
			foreach(var item in collection) {
				XtraRichEditModel.SelectionItem cloneItem = new XtraRichEditModel.SelectionItem(item.PieceTable) { Start = item.Start, End = item.End };
				clone.Add(cloneItem);
			}
			return clone;
		}
		private void Unselect(List<XtraRichEditModel.SelectionItem> cloneCollection, NativeDocumentRange range) {
			for(int i = cloneCollection.Count - 1; i >= 0; i--) {
				var sel = cloneCollection[i];
				if(sel.Length == 0) 
					continue;
				if(sel.NormalizedStart >= range.NormalizedStart.LogPosition && sel.NormalizedEnd <= range.NormalizedEnd.LogPosition) {
					cloneCollection.Remove(sel);
					continue;
				}
				else if(sel.NormalizedStart < range.NormalizedStart.LogPosition && sel.NormalizedEnd > range.NormalizedEnd.LogPosition) {
					if(sel.Start < sel.End) {
						var oldSelEnd = sel.End;
						sel.End = range.NormalizedStart.LogPosition;
						var newSelItem = CreateSelectionItem(range.NormalizedEnd.LogPosition, oldSelEnd);
						cloneCollection.Insert(i + 1, newSelItem);
					}
					else if(sel.Start > sel.End) {
						var oldSelEnd = sel.End;
						sel.End = range.NormalizedEnd.LogPosition;
						var newSelItem = CreateSelectionItem(range.NormalizedStart.LogPosition, oldSelEnd);
						cloneCollection.Insert(i + 1, newSelItem);
					}
				}
				else if(sel.NormalizedStart < range.NormalizedEnd.LogPosition && sel.NormalizedStart >= range.NormalizedStart.LogPosition) {
					if(sel.Start < sel.End)
						sel.Start = range.NormalizedEnd.LogPosition;
					else if(sel.Start > sel.End)
						sel.End = range.NormalizedEnd.LogPosition;
				}
				else if(sel.NormalizedEnd > range.NormalizedStart.LogPosition && sel.NormalizedEnd <= range.NormalizedEnd.LogPosition) {
					if(sel.Start < sel.End)
						sel.End = range.NormalizedStart.LogPosition;
					else if(sel.Start > sel.End)
						sel.Start = range.NormalizedStart.LogPosition;
				}
			}
		}
		#region IEnumerable<DocumentRange> Members
		public IEnumerator<DocumentRange> GetEnumerator() {
			return new SelectionCollectionEnumerator(selection, document);
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
	public class SelectionCollectionEnumerator : IEnumerator<DocumentRange> {
		private XtraRichEditModel.Selection selection;
		private int curIndex;
		private DocumentRange curRange;
		private NativeSubDocument doc;
		public SelectionCollectionEnumerator(XtraRichEditModel.Selection selection, NativeSubDocument document) {
			this.selection = selection;
			this.curIndex = -1;
			curRange = null;
			doc = document;
		}
		#region IEnumerator<DocumentRange> Members
		public DocumentRange Current {
			get { return curRange; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() { }
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			curIndex++;
			if(curIndex >= selection.Items.Count)
				return false;
			else
				curRange = doc.CreateRange(selection.Items[curIndex].NormalizedStart, selection.Items[curIndex].Length);
			return true;
		}
		public void Reset() {
			curIndex = -1;
		}
		#endregion
	}
}
