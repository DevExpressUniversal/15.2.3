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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native.Data;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Commands {
	#region TableCommandsHelper
	public static class TableCommandsHelper {
		public static bool IsWholeSelectionInOneBookmark(SnapPieceTable pieceTable, SelectedCellsIntervalInRow interval, TableCell startCell) {
			return IsWholeSelectionInOneBookmarkCore(pieceTable, startCell, interval.Row.Cells, interval.NormalizedStartCellIndex, interval.NormalizedEndCellIndex);
		}
		public static bool IsWholeSelectionInOneBookmark(SnapPieceTable pieceTable, TableRow row) {
			return IsWholeSelectionInOneBookmarkCore(pieceTable, row.Cells.First, row.Cells, 0, row.Cells.Count - 1);
		}
		static bool IsWholeSelectionInOneBookmarkCore(SnapPieceTable pieceTable, TableCell startCell, TableCellCollection cells, int startCellIndex, int endCellIndex) {
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			SnapBookmark startBookmark = controller.FindInnermostTemplateBookmarkByTableCell(startCell);
			for (int i = startCellIndex; i <= endCellIndex; i++) {
				TableCell cell = cells[i];
				if (!Object.ReferenceEquals(startBookmark, controller.FindInnermostTemplateBookmarkByTableCell(cell)))
					return false;
			}
			return true;
		}
		public static bool CanDeleteTableCells(DocumentModel documentModel) {
			return CanPerformTableCellOperationWithColumn(documentModel) ||
				CanPerformTableCellsOperationWithRow(documentModel) ||
				CanShiftToTheVertically(documentModel);
		}
		public static bool CanShiftToTheVertically(DocumentModel documentModel) {
			SnapPieceTable pieceTable = (SnapPieceTable)documentModel.ActivePieceTable;
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			return (Object.ReferenceEquals(controller.FindInnermostTemplateBookmarkByTableCell(documentModel.Selection.SelectedCells.FirstSelectedCell), null));
		}
		public static bool CanPerformTableCellOperationWithColumn(DocumentModel documentModel) {
			documentModel.BeginUpdate();
			try {
				return CanDeleteTableColumnsCore(documentModel);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		static bool CanDeleteTableColumnsCore(DocumentModel documentModel) {
			SelectedCellsCollection selectedCellsCollection = (SelectedCellsCollection)documentModel.Selection.SelectedCells;
			SnapPieceTable pieceTable = (SnapPieceTable)documentModel.ActivePieceTable;
			for (int i = 0; i < selectedCellsCollection.RowsCount; i++)
				if (!CheckSelectedCellsIntervalInRow(pieceTable, selectedCellsCollection[i]))
					return false;
			return true;
		}
		static bool CheckSelectedCellsIntervalInRow(SnapPieceTable pieceTable, SelectedCellsIntervalInRow interval) {
			if (!IsWholeSelectionInOneBookmark(pieceTable, interval, interval.NormalizedStartCell))
				return false;
			if (!CanMoveSeparatorTextRunToSameRow(pieceTable, interval))
				return false;
			return true;
		}
		static bool CanMoveSeparatorTextRunToSameRow(SnapPieceTable pieceTable, SelectedCellsIntervalInRow interval) {
			TableCellCollection cells = interval.Row.Cells;
			int endCellIndex = interval.NormalizedEndCellIndex;
			for (int i = interval.NormalizedStartCellIndex; i <= endCellIndex; i++) {
				TableCell cell = cells[i];
				RunInfo cellInfo = pieceTable.GetRunInfoByTableCell(cell);
#if DEBUGTEST
				for (RunIndex index = cellInfo.Start.RunIndex + 1; index <= cellInfo.End.RunIndex; index++) {
					if (pieceTable.Runs[index] is SeparatorTextRun) {
						TableCell nestedCell = pieceTable.Runs[index].Paragraph.GetCell();
						Debug.Assert(nestedCell != null);
						if (!Object.ReferenceEquals(cell, nestedCell))
							Debug.Assert(pieceTable.GetRunInfoByTableCell(nestedCell).Start.RunIndex == index);
					}
				}
#endif
				if (!(pieceTable.Runs[cellInfo.Start.RunIndex] is SeparatorTextRun))
					continue;
				if (endCellIndex == cells.Count - 1)
					return false;
				SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
				SnapBookmark cellBookmark = controller.FindInnermostTemplateBookmarkByPosition(cellInfo.NormalizedStart.RunIndex);
				RunInfo endCellInfo = pieceTable.GetRunInfoByTableCell(cells[endCellIndex + 1]);
				if (cellBookmark == null || !cellBookmark.Contains(endCellInfo.Start.LogPosition, endCellInfo.End.LogPosition))
					return false;
			}
			return true;
		}
		public static bool CanPerformTableCellsOperationWithRow(DocumentModel documentModel) {
			List<TableRow> rows = documentModel.Selection.GetSelectedTableRows();
			SnapPieceTable pieceTable = (SnapPieceTable)documentModel.ActivePieceTable;
			documentModel.BeginUpdate();
			try {
				return CanPerformTableCellsOperationWithRowCore(rows, pieceTable);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		static bool CanPerformTableCellsOperationWithRowCore(List<TableRow> rows, SnapPieceTable pieceTable) {
			SnapBookmark currentBookmark = null;
			int previousRowIndex = Int32.MinValue;
			DocumentLogPosition normalizedUnitStart = DocumentLogPosition.Zero;
			DocumentLogPosition normalizedUnitEnd = DocumentLogPosition.Zero;
			SnapObjectModelController objectModelController = new SnapObjectModelController(pieceTable);
			int count = rows.Count;
			for (int i = 0; i < count; i++) {
				TableRow row = rows[i];
				if (!IsWholeSelectionInOneBookmark(pieceTable, row))
					return false;
				TableCell firstCell = row.Cells.First;
				SnapBookmarkController bookmarkController = new SnapBookmarkController(pieceTable);
				SnapBookmark bookmark = bookmarkController.FindInnermostTemplateBookmarkByTableCell(firstCell);
				if (Object.ReferenceEquals(bookmark, null))
					continue;
				if (!Object.ReferenceEquals(bookmark, currentBookmark) || row.IndexInTable != previousRowIndex + 1) {
					currentBookmark = bookmark;
					normalizedUnitStart = objectModelController.FindCellNormalizedStartLogPosition(firstCell);
				}
				previousRowIndex = row.IndexInTable;
				normalizedUnitEnd = objectModelController.FindCellNormalizedEndLogPosition(row.Cells.Last) + 1;
				if (bookmark.NormalizedStart == normalizedUnitStart && bookmark.NormalizedEnd == normalizedUnitEnd)
					return false;
			}
			return true;
		}
		public static ColumnIndexesInfo CalculateSelectedColumnIndexes(SelectedCellsCollection selectedCells) {
			ColumnIndexesInfo result = new ColumnIndexesInfo();
			TableCell firstCell = selectedCells.NormalizedFirst.NormalizedStartCell;
			TableCell lastCell = selectedCells.NormalizedFirst.NormalizedEndCell;
			result.StartColumnIndex = GetStartColumnIndex(firstCell);
			result.EndColumnIndex = GetEndColumnIndex(lastCell);
			int bottomRowIndex = selectedCells.GetBottomRowIndex();
			for (int i = selectedCells.GetTopRowIndex(); i <= bottomRowIndex; i++) {
				firstCell = selectedCells[i].NormalizedStartCell;
				lastCell = selectedCells[i].NormalizedEndCell;
				result.StartColumnIndex = Algorithms.Max(result.StartColumnIndex, GetStartColumnIndex(firstCell));
				result.EndColumnIndex = Algorithms.Min(result.EndColumnIndex, GetEndColumnIndex(lastCell));
			}
			return result;
		}
		public static int GetStartColumnIndex(TableCell firstCell) {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(firstCell, false);
		}
		public static int GetEndColumnIndex(TableCell lastCell) {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(lastCell, false) + lastCell.ColumnSpan - 1;
		}
		public static void InsertSeparators(PieceTable pieceTable, List<TableCell> cells) {
			int count = cells.Count;
			SnapObjectModelController controller = new SnapObjectModelController((SnapPieceTable)pieceTable);
			for (int i = count - 1; i >= 0; i--) {
				DocumentLogPosition pos = controller.FindCellStartLogPosition(cells[i]);
				pieceTable.InsertSeparator(pos);
				pieceTable.LastInsertedSeparatorRunInfo.Run.Hidden = true;
			}
		}
		public static List<SnapListFieldInfo> InsertHeader(SnapPieceTable pieceTable, SnapDocumentModel targetModel, DocumentLogPosition insertPosition) {
			return InsertHeader(pieceTable, targetModel, insertPosition, null);
		}
		public static List<SnapListFieldInfo> InsertHeader(SnapPieceTable pieceTable, SnapDocumentModel targetModel, DocumentLogPosition insertPosition, SNDataInfo[] dataInfo) {
			List<SnapListFieldInfo> snListFields = new List<SnapListFieldInfo>();
			pieceTable.Fields.ForEach(field => {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				CalculatedFieldBase parsedInfo = calculator.ParseField(pieceTable, field);
				SNListField snListField = parsedInfo as SNListField;
				if(snListField != null) {
					snListFields.Add(new SnapListFieldInfo(pieceTable, field, snListField));
				}
			});
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(targetModel.ActivePieceTable, insertPosition);
			Field parentField = targetModel.ActivePieceTable.FindFieldByRunIndex(modelPos.RunIndex);
			int parentLevel = (parentField != null) ? parentField.GetLevel() : 0;
			foreach(SnapListFieldInfo fieldInfo in snListFields) {
				TemplateModifierExecutor templateModifierExecutor = new TemplateModifierExecutor(fieldInfo, SNListField.ListHeaderTemplateSwitch);
				InsertHeaderCommandInternal modifier = new InsertHeaderCommandInternal(targetModel, fieldInfo.Field.GetLevel() + parentLevel, dataInfo);
				templateModifierExecutor.SuppressFieldsUpdateAfterUpdateInstruction = true;
				templateModifierExecutor.ModifyTemplate(modifier);
			}
			return snListFields;
		}
		public static bool CheckForTablesIncluded(DocumentModel documentModel) {
			return documentModel.Selection.IsSelectionInTable();
		}
		public static SnapDocumentModel CreateModel(DocumentModel sourceDocumentModel, SnapPieceTable pieceTable, DocumentLogInterval templateInterval, Action<Field, MergefieldField, PieceTable, DocumentLogPosition> replaceFieldAction, Action<TableCollection> processTableBeforeFinishAction, Action<PieceTable> finishAction) {
			SnapDocumentModel result = (SnapDocumentModel)sourceDocumentModel.CreateNew();
			result.IntermediateModel = true;
			result.BeginSetContent();
			try {
				result.InheritDataServices(sourceDocumentModel);
				PieceTable resultPieceTable = result.MainPieceTable;
				if (templateInterval != null) {
					CopyHelper.CopyCore(pieceTable, resultPieceTable, templateInterval, DocumentLogPosition.Zero);
					if (replaceFieldAction != null)
						ReplaceFields(resultPieceTable, replaceFieldAction);
					else
						ClearContent(resultPieceTable);
					if (processTableBeforeFinishAction != null)
						processTableBeforeFinishAction(resultPieceTable.Tables);
				}
				resultPieceTable.FixLastParagraph();
				if (finishAction != null)
					finishAction(resultPieceTable);
			}
			finally {
				result.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			return result;
		}
		static void ReplaceFields(PieceTable resultPieceTable, Action<Field, MergefieldField, PieceTable, DocumentLogPosition> replaceFieldAction) {
			SnapFieldCalculatorService parser = new SnapFieldCalculatorService();
			FieldCollection fields = resultPieceTable.Fields;
			int fieldIndex = 0;
			while (fields.Count > 0) {
				Field current = fields[fieldIndex];
				MergefieldField mergeField = parser.ParseField(resultPieceTable, current) as MergefieldField;
				if (current.Parent != null) {
					fieldIndex++;
					continue;
				}
				DocumentLogPosition fieldStart = resultPieceTable.GetRunLogPosition(current.FirstRunIndex);
				DocumentLogPosition fieldEnd = resultPieceTable.GetRunLogPosition(current.LastRunIndex);
				resultPieceTable.DeleteContent(fieldStart, fieldEnd - fieldStart + 1, false);
				if (replaceFieldAction != null)
					replaceFieldAction(current, mergeField, resultPieceTable, fieldStart);
				fieldIndex = 0;
			}
		}
		static void ClearContent(PieceTable resultPieceTable) {
			TableCollection tables = resultPieceTable.Tables;
			for(int i = 0; i < tables.Count; i++) {
				Table table = tables[i];
				if(table.NestedLevel > 0) {
					resultPieceTable.DeleteTableWithContent(table);
					i--;
				}
			}
			tables.ForEach(table => {
				table.Rows.ForEach(row => {
					row.Cells.ForEach(cell => { 
						DocumentLogPosition start = resultPieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
						int length = resultPieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition - start;
						resultPieceTable.DeleteContent(start, length, false); 
					});
				});
			});
		}
		public static void MergeTablesHorizontally(TableCollection tables) {
			int count = tables.Count - 1;
			for (int tableIndex = count; tableIndex >= 0; tableIndex--) {
				Table table = tables[tableIndex];
				PieceTable pieceTable = table.PieceTable;
				for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++) {
					TableRow row = table.Rows[rowIndex];
					pieceTable.MergeTableCellsHorizontally(row.FirstCell, row.Cells.Count);
				}
			}
		}
		public static void MergeTablesTotally(TableCollection tables) {
			foreach(Table table in tables) { 
				PieceTable pieceTable = table.PieceTable;
				table.ForEachRow(row => pieceTable.MergeTableCellsHorizontally(row.FirstCell, row.Cells.Count));
				pieceTable.MergeTableCellsVertically(table.FirstRow.FirstCell, table.Rows.Count);
			}
		}
	}
	#endregion
}
