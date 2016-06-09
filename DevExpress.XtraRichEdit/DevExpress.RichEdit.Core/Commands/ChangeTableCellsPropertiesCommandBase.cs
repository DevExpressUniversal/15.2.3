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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeTableCellsCommandBase (abstract class)
	public abstract class ChangeTableCellsCommandBase : RichEditSelectionCommand {
		#region Fields
		ICommandUIState executeState;
		List<TableCell> cells;
		bool isSelectedCellsSquare;
		#endregion
		protected ChangeTableCellsCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal bool IsSelectedCellsSquare { get { return isSelectedCellsSquare; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			return true;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return false;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (DocumentModel.Selection.IsWholeSelectionInOneTable())
				UpdateUIStateCore(state, DocumentModel.Selection.SelectedCells);
			else
				state.Enabled = false;
		}
		protected internal virtual void UpdateUIStateCore(ICommandUIState state, ISelectedTableStructureBase selectedCellsCollection) {
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, DocumentModel.Selection.IsWholeSelectionInOneTable());
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		public override void ForceExecute(ICommandUIState state) {
			this.executeState = state;
			base.ForceExecute(state);
		}
		protected internal override void PerformModifyModel() {
			PerformModifyModelCore(this.executeState);
		}
		protected internal virtual void PerformModifyModelCore(ICommandUIState state) {
			this.cells = new List<TableCell>();
			CollectCells(cells);
			ModifyCells(cells, state);
			CheckInsideTableBorders();
		}
		protected internal virtual void CollectCells(List<TableCell> target) {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection cells = GetSelectedCellsCollection();
			if (cells == null)
				return;
			isSelectedCellsSquare = cells.SelectedOnlyOneCell || cells.IsSquare();
			CollectCellsCore(cells, target);
		}
		protected internal virtual void CollectCellsCore(SelectedCellsCollection cells, List<TableCell> target) {
			SelectedTableCellPositionInfo cellPositionInfo = new SelectedTableCellPositionInfo();
			int top = cells.GetTopRowIndex();
			int bottom = cells.GetBottomRowIndex();
			bool invert = cells[0] != cells.NormalizedFirst;
			for (int i = top; i <= bottom; i++) {
				int index = invert ? bottom - i : i;
				cellPositionInfo.FirstSelectedRow = (i == top);
				cellPositionInfo.LastSelectedRow = (i == bottom);
				CollectCells(cells[index], cellPositionInfo, target);
			}
		}
		protected internal virtual void CollectCells(SelectedCellsIntervalInRow cellsInRow, SelectedTableCellPositionInfo cellPositionInfo, List<TableCell> target) {
			TableRow row = cellsInRow.Row;
			int startCellIndex = cellsInRow.NormalizedStartCellIndex;
			int endCellIndex = cellsInRow.NormalizedEndCellIndex;
			for (int i = startCellIndex; i <= endCellIndex; i++) {
				cellPositionInfo.FirstSelectedColumn = (i == startCellIndex);
				cellPositionInfo.LastSelectedColumn = (i == endCellIndex);
				TableCell currentCell = row.Cells[i];
				if (IsSelectedCellsSquare) {
					List<TableCell> spanCells = currentCell.GetVerticalSpanCells();
					int spanCellsCount = spanCells.Count;
					for (int j = 0; j < spanCellsCount; j++) {
						SelectedTableCellPositionInfo cellPositionInfoForMergedCells = new SelectedTableCellPositionInfo();
						cellPositionInfoForMergedCells.CopyFrom(cellPositionInfo);
						cellPositionInfoForMergedCells.FirstSelectedRow &= (j == 0);
						cellPositionInfoForMergedCells.LastSelectedRow |= (j == spanCellsCount - 1 && spanCellsCount > 1);
						CollectCell(spanCells[j], cellPositionInfoForMergedCells, target);
					}
				}
				else
					CollectCell(currentCell, cellPositionInfo, target);
			}
		}
		protected internal virtual void CollectCell(TableCell cell, SelectedTableCellPositionInfo cellPositionInfo, List<TableCell> target) {
			if (!target.Contains(cell))
				target.Add(cell);
		}
		protected internal virtual void ModifyCells(List<TableCell> cells, ICommandUIState state) {
			cells.ForEach(ModifyCell);
		}
		protected internal abstract void ModifyCell(TableCell tableCell);
		private static void SetInsideTableBorder(BorderBase insideBorder, BorderLineStyle style, int width, Color color) {
			insideBorder.BeginUpdate();
			try {
				insideBorder.Style = style;
				insideBorder.Width = width;
				insideBorder.Color = color;
			}
			finally {
				insideBorder.EndUpdate();
			}
		}
		private void CheckInsideTableBorder(Table table, bool isCheckingVerticalBorder, BorderBase insideBorder, BorderInfo styleBorderInfo) {
			for (int i = 0; i < table.Rows.Count; i++) {
				TableRow currentRow = table.Rows[i];
				for (int j = 0; j < currentRow.Cells.Count; j++) {
					TableCellBorders currentCellBorders = currentRow.Cells[j].Properties.Borders;
					BorderInfo currentCellBorder = isCheckingVerticalBorder ? currentCellBorders.LeftBorder.Info : currentCellBorders.TopBorder.Info;
					if (!currentRow.IsFirstRowInTable && currentCellBorder.Style != BorderLineStyle.None) {
						SetInsideTableBorder(insideBorder, styleBorderInfo.Style, styleBorderInfo.Width, styleBorderInfo.Color);
						return;
					}
				}
			}
			SetInsideTableBorder(insideBorder, BorderLineStyle.None, 0, DXColor.Empty);
		}
		private void CheckInsideTableBorders() {
			TableCell firstSelectedCell = DocumentModel.Selection.SelectedCells.FirstSelectedCell;
			if (firstSelectedCell == null)
				return;
			Table table = firstSelectedCell.Table;
			TableBorders borders = table.TableProperties.Borders;
			MergedTableProperties tableStyleProperties = table.TableStyle.GetMergedTableProperties();
			CheckInsideTableBorder(table, false, borders.InsideHorizontalBorder, tableStyleProperties.Info.Borders.InsideHorizontalBorder);
			CheckInsideTableBorder(table, true, borders.InsideVerticalBorder, tableStyleProperties.Info.Borders.InsideVerticalBorder);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	public class SelectedTableCellPositionInfo {
		bool firstSelectedRow;
		bool lastSelectedRow;
		bool firstSelectedColumn;
		bool lastSelectedColumn;
		TableCell cell;
		public bool FirstSelectedRow { get { return firstSelectedRow; } set { firstSelectedRow = value; } }
		public bool LastSelectedRow { get { return lastSelectedRow; } set { lastSelectedRow = value; } }
		public bool FirstSelectedColumn { get { return firstSelectedColumn; } set { firstSelectedColumn = value; } }
		public bool LastSelectedColumn { get { return lastSelectedColumn; } set { lastSelectedColumn = value; } }
		public TableCell Cell { get { return cell; } set { cell = value; } }
		protected internal virtual void CopyFrom(SelectedTableCellPositionInfo source) {
			firstSelectedRow = source.FirstSelectedRow;
			lastSelectedRow = source.LastSelectedRow;
			firstSelectedColumn = source.FirstSelectedColumn;
			lastSelectedColumn = source.LastSelectedColumn;
		}
	}
}
