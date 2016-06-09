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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
namespace DevExpress.XtraRichEdit.Commands {
	#region TabKeyCommand
	public class TabKeyCommand : RichEditCaretBasedCommand {
		public TabKeyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabKeyCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.TabKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabKeyCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_TabKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabKeyCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_TabKeyDescription; } }
		protected internal DocumentLogPosition StartLogPosition { get { return DocumentModel.Selection.Interval.NormalizedStart.LogPosition; } }
		protected internal DocumentLogPosition EndLogPosition { get { return DocumentModel.Selection.Interval.NormalizedEnd.LogPosition; } }
		#endregion
		protected internal override void ExecuteCore() {
			Selection selection = DocumentModel.Selection;
			if (selection.IsWholeSelectionInOneTable()) {
				SelectedCellsCollection cells = (SelectedCellsCollection)selection.SelectedCells;
				ProcessSingleCell(cells.TopLeftCell);
				selection.ClearMultiSelection();
				selection.SetStartCell(selection.NormalizedStart);
				return;
			}
			if (IsSelectionStartFromBeginRow() && SelectedWholeRow() && !IsSelectedOneParagraphWithTab() && DocumentModel.DocumentCapabilities.ParagraphFormattingAllowed)
				ChangeIndent();
			else {
				InsertTabCommand insertCommand = CreateInsertTabCommand();
				insertCommand.ForceExecute(CreateDefaultCommandUIState());
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
		protected internal virtual bool IsSelectionStartFromBeginRow() {
			SelectionLayout selectionLayout = ActiveView.SelectionLayout;
			return selectionLayout.IsSelectionStartFromBeginRow();
		}
		protected internal virtual void ChangeIndent() {
			InputPosition oldInputPosition = null;
			if(DocumentModel.Selection.Length == 0)
				oldInputPosition = CaretPosition.GetInputPosition().Clone();
			IncrementIndentByTheTabCommand incrementCommand = CreateIncrementIndentByTheTabCommand();
			incrementCommand.ForceExecute(CreateDefaultCommandUIState());
			if(oldInputPosition != null) {
				UpdateCaretPosition(DocumentLayoutDetailsLevel.Character);
				InputPosition newInputPosition = CaretPosition.GetInputPosition();
				newInputPosition.CopyFormattingFrom(oldInputPosition);
			}
		}
		protected internal virtual InsertTabCommand CreateInsertTabCommand() {
			return new InsertTabCommand(Control);
		}
		protected internal virtual IncrementIndentByTheTabCommand CreateIncrementIndentByTheTabCommand() {
			return new IncrementIndentByTheTabCommand(Control);
		}
		protected internal virtual bool SelectedWholeRow() {
			SelectionLayout selectionLayout = ActiveView.SelectionLayout;
			Paragraph startParagraph = ActivePieceTable.Paragraphs[DocumentModel.Selection.Interval.Start.ParagraphIndex];
			if (DocumentModel.Selection.Length == 0)
				return (startParagraph.Length > 1 || startParagraph.IsInList());
			bool equalsStartAndEndRow = selectionLayout.StartLayoutPosition.Row.Equals(selectionLayout.EndLayoutPosition.Row);
			CharacterBox character = selectionLayout.EndLayoutPosition.Character;
			bool selectedUpToEndRow = character != null ?  character.EndPos.AreEqual(selectionLayout.EndLayoutPosition.Row.Boxes.Last.EndPos) : false;
			if (equalsStartAndEndRow && !selectedUpToEndRow)
				return false;
			return true;
		}
		bool IsSelectedOneParagraphWithTab() {
			ParagraphIndex startParagraphIndex = DocumentModel.Selection.Interval.Start.ParagraphIndex;
			ParagraphIndex endParagraphIndex = DocumentModel.Selection.Interval.End.ParagraphIndex;
			Paragraph startParagraph = ActivePieceTable.Paragraphs[startParagraphIndex];
			return (startParagraphIndex == endParagraphIndex && startParagraph.Tabs.Info.Count > 0 && !startParagraph.IsInList());
		}
		protected internal virtual bool IsLastCellInDirection(TableCell cell) {
			return cell.IsLastCellInRow;
		}
		TableCell GetCurrentCell(TableCell cell) {
			TableCell result = cell;
			if (IsLastCellInDirection(cell) && cell.VerticalMerging == MergingState.Restart) {
				TableCellVerticalAnchorCollection anchors = CaretPosition.LayoutPosition.TableCell.TableViewInfo.Anchors;
				List<TableCellVerticalAnchor> items = anchors.Items;
				int index = CaretPosition.TableCellTopAnchorIndex;
				TableRow targetRow = cell.Row;
				if (index >= 0)
					targetRow = cell.Table.Rows[index];
				else {
					index = ~index;
					if (index >= items.Count) {
						targetRow = cell.Table.LastRow;
					}
				}
				int startColumnIndex = cell.GetStartColumnIndexConsiderRowGrid();
				int cellIndexInTargetRow = cell.Table.GetAbsoluteCellIndexInRow(targetRow, startColumnIndex, false);
				result = targetRow.Cells[cellIndexInTargetRow];
			}
			return result;
		}
		TableCell GetNextUnmergedCellFromCoveredCell(TableCell cell) {
			TableCell result = cell;
			TableCellVerticalAnchorCollection anchors = CaretPosition.LayoutPosition.TableCell.TableViewInfo.Anchors;
			List<TableCellVerticalAnchor> items = anchors.Items;
			int index = CaretPosition.TableCellTopAnchorIndex;
			TableRow targetRow = cell.Row;
			if (index >= cell.Table.Rows.Count)
				targetRow = cell.Table.Rows.Last;
			else if (index >= 0)
				targetRow = cell.Table.Rows[index];
			else {
				index = ~index;
				if (index >= items.Count)
					targetRow = cell.Table.LastRow;
			}
			int startColumnIndex = cell.GetStartColumnIndexConsiderRowGrid();
			int cellIndexInTargetRow = cell.Table.GetAbsoluteCellIndexInRow(targetRow, startColumnIndex, false);
			result = targetRow.Cells[cellIndexInTargetRow];
			return result;
		}
		protected internal virtual void ModifyCaretPositionCellTopAnchor() {
			CaretPosition.TableCellTopAnchorIndex = CaretPosition.TableCellTopAnchorIndex + 1;
		}
		protected internal virtual TableCell GetNextCell(TableCell currentCell) {
			return currentCell.Next;
		}
		protected internal virtual bool ShouldAppledTableRowAtBottom(TableCell currentCell) {
			return currentCell.IsLastCellInTable;
		}
		protected internal virtual void ProcessSingleCell(TableCell cell) {
			TableCell currentCell = GetCurrentCell(cell);
			if (ShouldAppledTableRowAtBottom(currentCell))
				AppendTableRowAtBottom(currentCell.Table);
			else {
				bool nextCellInNewRow = false;
				TableCell nextCell = GetNextCell(currentCell);
				if (nextCell == null)
					return;
				if (currentCell.RowIndex != nextCell.RowIndex)
					nextCellInNewRow = true;
				if (currentCell.VerticalMerging == MergingState.Continue || currentCell.VerticalMerging == MergingState.Restart 
					&& nextCell.VerticalMerging == MergingState.None) {
					nextCell = GetNextUnmergedCellFromCoveredCell(currentCell);
					nextCell = GetNextCell(nextCell);
					if (nextCell == null) 
						return;
				}
				if (nextCell.VerticalMerging == MergingState.Continue) {
					nextCell = nextCell.Table.GetFirstCellInVerticalMergingGroup(nextCell);
					if (nextCellInNewRow && nextCell.IsFirstCellInRow) {
						ModifyCaretPositionCellTopAnchor();
						nextCellInNewRow = false;
					}
				}
				if (nextCellInNewRow && currentCell.IsFirstCellInRow) {
					ModifyCaretPositionCellTopAnchor();
					nextCellInNewRow = false;
				}
				SelectEntireCell(nextCell, nextCellInNewRow);
			}
		}
		protected internal virtual void SelectEntireCell(TableCell cell, bool shouldUpdateCaretY) {
			SelectTableCellCommand command = new SelectTableCellCommand(Control);
			command.Cell = cell;
			command.ShouldUpdateCaretTableAnchorVerticalPositionY = shouldUpdateCaretY;
			command.ForceExecute(command.CreateDefaultCommandUIState());
		}
		protected internal virtual void AppendTableRowAtBottom(Table table) {
			InsertTableRowBelowCoreCommand command = new InsertTableRowBelowCoreCommand(Control);
			command.Row = table.Rows.Last;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			if (state.Enabled && state.Visible) {
				command.ForceExecute(state);
			}
		}
	}
	#endregion
	#region ShiftTabKeyCommand
	public class ShiftTabKeyCommand : TabKeyCommand {
		public ShiftTabKeyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShiftTabKeyCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ShiftTabKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShiftTabKeyCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_TabKey; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShiftTabKeyCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_TabKeyDescription; } }
		#endregion
		protected internal override void ChangeIndent() {
			DecrementIndentByTheTabCommand decrementCommand = CreateDecrementIndentByTheTabCommand();
			decrementCommand.ForceExecute(CreateDefaultCommandUIState());
		}
		protected internal virtual DecrementIndentByTheTabCommand CreateDecrementIndentByTheTabCommand() {
			return new DecrementIndentByTheTabCommand(Control);
		}
		protected internal override void ProcessSingleCell(TableCell cell) {
			if (cell.IsFirstCellInTable && CaretPosition.TableCellTopAnchorIndex == 0)
				return;
			base.ProcessSingleCell(cell);
		}
		protected internal override void ModifyCaretPositionCellTopAnchor() {
			CaretPosition.TableCellTopAnchorIndex = CaretPosition.TableCellTopAnchorIndex - 1;
		}
		protected internal override TableCell GetNextCell(TableCell currentCell) {
			return currentCell.Previous;
		}
		protected internal override bool ShouldAppledTableRowAtBottom(TableCell currentCell) {
			return false;
		}
		protected internal override bool IsLastCellInDirection(TableCell cell) {
			return cell.IsFirstCellInRow;
		}
	}
	#endregion
}
