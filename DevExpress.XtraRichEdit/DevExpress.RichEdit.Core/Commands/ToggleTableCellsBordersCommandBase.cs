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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	public class TableCellActualBorders : ITableCellBorders {
		TableCell cell;
		BorderBase top;
		BorderBase bottom;
		BorderBase left;
		BorderBase right;
		public TableCellActualBorders(TableCell cell) {
			this.cell = cell;
		}
		public BorderBase TopBorder {
			get {
				if(top == null)
					top = cell.GetActualTopCellBorder();
				return top;
			}
		}
		public BorderBase BottomBorder {
			get {
				if(bottom == null)
					bottom = cell.GetActualBottomCellBorder();
				return bottom;
			}
		}
		public BorderBase LeftBorder {
			get {
				if(left == null)
					left = cell.GetActualLeftCellBorder();
				return left;
			}
		}
		public BorderBase RightBorder {
			get {
				if(right == null)
					right = cell.GetActualRightCellBorder();
				return right;
			}
		}
	}
	#region ToggleTableCellsBordersCommandBase (abstract class)
	public abstract class ToggleTableCellsBordersCommandBase : ChangeTableCellsCommandBase {
		#region Fields
		List<BorderBase> borders;
		List<BorderBase> actualBorders;
		#endregion
		protected ToggleTableCellsBordersCommandBase(IRichEditControl control)
			: base(control) {
		}
		public BorderInfo NewBorder { get; set; }
		protected internal override void CollectCells(List<TableCell> target) {
			this.borders = new List<BorderBase>();
			this.actualBorders = new List<BorderBase>();
			if (AreTableBordersAffected()) {
				TableBorders talbeBorders = DocumentModel.Selection.SelectedCells.FirstSelectedCell.Table.TableProperties.Borders;
				CollectTableBorders(talbeBorders, borders);
			}
			base.CollectCells(target);
		}
		protected bool AreTableBordersAffected() {
			Selection selection = DocumentModel.Selection;
			SelectedCellsCollection cells = (SelectedCellsCollection)selection.SelectedCells;
			return cells.IsSelectedEntireTable() && selection.IsWholeSelectionInOneTable();
		}
		protected abstract void CollectTableBorders(TableBorders tableBorders, List<BorderBase> borders);
		protected internal override void CollectCellsCore(SelectedCellsCollection cells, List<TableCell> target) {
			if (!cells.IsNotEmpty)
				return;
			List<SelectionItem> items = DocumentModel.Selection.Items;
			if (items[0].Generation == items[items.Count - 1].Generation) {
				CollectTopAndBottomNeighbourBordersCore(cells, target, cells.NormalizedFirst, cells.NormalizedLast);
				base.CollectCellsCore(cells, target);
			}
			else {
				CollectCellsCoreWithMultipleItems(cells, target);
			}
		}
		void CollectCellsCoreWithMultipleItems(SelectedCellsCollection selectedCells, List<TableCell> target) {
			CollectTopAndBottomNeighbourBorders(selectedCells, target);
			base.CollectCellsCore(selectedCells, target);
		}
		void CollectTopAndBottomNeighbourBorders(SelectedCellsCollection selectedCells, List<TableCell> target) {
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int generationStartCellIndex = 0;
			int count = items.Count;
#if DEBUGTEST
			Debug.Assert(count == selectedCells.RowsCount);
#endif
			for (int i = 0; i < count; i++) {
				if (i == count - 1 || items[i].Generation != items[i + 1].Generation) {
					CollectTopAndBottomNeighbourBordersCore(selectedCells, target, selectedCells[generationStartCellIndex], selectedCells[i]);
					generationStartCellIndex = i + 1;
				}
			}
		}
		void CollectTopAndBottomNeighbourBordersCore(SelectedCellsCollection cells, List<TableCell> target, SelectedCellsIntervalInRow first, SelectedCellsIntervalInRow last) {
			if (!(cells.First.Table.CellSpacing.Value > 0)) {
				CollectTopNeighbourBorders(first, borders);
				CollectBottomNeighbourBorders(last, borders);
			}
		}
		protected internal override void CollectCells(SelectedCellsIntervalInRow cellsInRow, SelectedTableCellPositionInfo cellPositionInfo, List<TableCell> target) {
			bool hasCellSpacing = cellsInRow.Table.CellSpacing.Value > 0;
			if (!hasCellSpacing)
				CollectLeftNeighbourBorders(cellsInRow, borders);
			base.CollectCells(cellsInRow, cellPositionInfo, target);
			if (!hasCellSpacing)
				CollectRightNeighbourBorders(cellsInRow, borders);
		}
		protected internal override void ModifyCells(List<TableCell> cells, ICommandUIState state) {
			if (state.Checked)
				borders.ForEach(ResetBorder);
			else
				borders.ForEach(ModifyBorder);
		}
		protected internal override void ModifyCell(TableCell tableCell) {
		}
		protected internal override void CollectCell(TableCell tableCell, SelectedTableCellPositionInfo cellPositionInfo, List<TableCell> target) {
			CollectBorders(tableCell.Properties.Borders, cellPositionInfo, borders);
			CollectBorders(new TableCellActualBorders(tableCell), cellPositionInfo, actualBorders);
		}
		protected internal virtual void CollectTopNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
			if (cellsInRow.Row.IsFirstRowInTable)
				return;
			List<TableCell> cells = new List<TableCell>();
			CollectNeighbourRowCells(cellsInRow.Row.Previous, cellsInRow, cells);
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				borders.Add(cells[i].Properties.Borders.BottomBorder);
		}
		protected internal virtual void CollectBottomNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
			TableRow row;
			if (IsSelectedCellsSquare) {
				List<TableCell> spanCells = cellsInRow.StartCell.GetVerticalSpanCells();
				row = spanCells[spanCells.Count - 1].Row;
			}
			else
				row = cellsInRow.Row;
			if (row.IsLastRowInTable)
				return;
			List<TableCell> cells = new List<TableCell>();
			CollectNeighbourRowCells(row.Next, cellsInRow, cells);
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				borders.Add(cells[i].Properties.Borders.TopBorder);
		}
		protected internal virtual void CollectNeighbourRowCells(TableRow row, SelectedCellsIntervalInRow cellsInRow, List<TableCell> target) {
			int startColumnIndex = cellsInRow.NormalizedStartCell.GetStartColumnIndexConsiderRowGrid();
			int endColumnIndex = cellsInRow.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid();
			List<TableCell> cells = TableCellVerticalBorderCalculator.GetCellsByIntervalColumnIndex(row, startColumnIndex, endColumnIndex);
			int count = cells.Count;
			if (count <= 0)
				return;
			TableCell lastCell = cells[count - 1];
			if (lastCell.GetEndColumnIndexConsiderRowGrid() > endColumnIndex) {
				cells.RemoveAt(count - 1);
				count--;
			}
			if (cells.Count == 0)
				return;
			TableCell firstCell = cells[0];
			if (firstCell.GetStartColumnIndexConsiderRowGrid() < startColumnIndex) {
				cells.RemoveAt(0);
				count--;
			}
			target.AddRange(cells);
		}
		protected internal virtual void CollectLeftNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
			TableCell cell = cellsInRow.NormalizedStartCell;
			if (IsSelectedCellsSquare) {
				CollectLeftNeighbourBordersInMergedCells(cell, borders);
				return;
			}
			if (cell.IsFirstCellInRow)
				return;
			TableCell previousCell = cell.Previous;
			borders.Add(previousCell.Properties.Borders.RightBorder);
		}
		protected internal virtual void CollectLeftNeighbourBordersInMergedCells(TableCell cell, List<BorderBase> borders) {
			List<TableCell> spanCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(cell, cell.GetStartColumnIndexConsiderRowGrid(), false);
			int spanCellsCount = spanCells.Count;
			for (int i = 0; i < spanCellsCount; i++) {
				TableCell currentCell = spanCells[i];
				if (cell.IsFirstCellInRow)
					continue;
				TableCell previousCell = currentCell.Previous;
				RightBorder rightBorder = previousCell.Properties.Borders.RightBorder;
				if (!borders.Contains(rightBorder))
					borders.Add(rightBorder);
			}
		}
		protected internal virtual void CollectRightNeighbourBorders(SelectedCellsIntervalInRow cellsInRow, List<BorderBase> borders) {
			TableCell cell = cellsInRow.NormalizedEndCell;
			if (IsSelectedCellsSquare) {
				CollectRightNeighbourBordersInMergedCells(cell, borders);
				return;
			}
			if (cell.IsLastCellInRow)
				return;
			TableCell nextCell = cell.Next;
			borders.Add(nextCell.Properties.Borders.LeftBorder);
		}
		protected internal virtual void CollectRightNeighbourBordersInMergedCells(TableCell cell, List<BorderBase> borders) {
			List<TableCell> spanCells = TableCellVerticalBorderCalculator.GetVerticalSpanCells(cell, cell.GetStartColumnIndexConsiderRowGrid(), false);
			int spanCellsCount = spanCells.Count;
			for (int i = 0; i < spanCellsCount; i++) {
				TableCell currentCell = spanCells[i];
				if (currentCell.IsLastCellInRow)
					continue;
				TableCell nextCell = currentCell.Next;
				LeftBorder leftBorder = nextCell.Properties.Borders.LeftBorder;
				if (!borders.Contains(leftBorder))
					borders.Add(leftBorder);
			}
		}
		protected internal virtual void ModifyBorder(BorderBase border) {
			BorderInfo borderInfo = GetActualNewBorder();
			border.BeginUpdate();
			try {
				border.Style = borderInfo.Style;
				border.Width = borderInfo.Width;
				border.Color = borderInfo.Color;
			}
			finally {
				border.EndUpdate();
			}
		}
		protected internal virtual void ResetBorder(BorderBase border) {
			border.ReplaceInfo(BorderInfo.Empty.Clone(), border.GetBatchUpdateChangeActions());
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled) {
				List<TableCell> cells = new List<TableCell>();
				CollectCells(cells); 
				state.Checked = CalculateIsChecked(actualBorders);
			}
		}
		protected internal virtual bool CalculateIsChecked(List<BorderBase> borders) {
			int count = borders.Count;
			if (count <= 0)
				return false;
			BorderInfo initialInfo = GetActualNewBorder();
			for (int i = 0; i < count; i++) {
				BorderInfo info = borders[i].Info;
				if (!Object.Equals(info, initialInfo))
					return false;
			}
			return true;
		}
		BorderInfo GetActualNewBorder(){
			if (NewBorder != null) {
				return NewBorder;
			}
			else 
				return DocumentModel.TableBorderInfoRepository.CurrentItem;
		}
		protected internal abstract void CollectBorders(ITableCellBorders cellBorders, SelectedTableCellPositionInfo cellPositionInfo, List<BorderBase> borders);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeTableBordersPlaceholderCommand
	public class ChangeTableBordersPlaceholderCommand : ChangeTableCellsCommandBase, IPlaceholderCommand {
		public ChangeTableBordersPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableBorders; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableBordersDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeTableBordersPlaceholder; } }
		public override string ImageName { get { return "BordersOutside"; } }
		public override void ForceExecute(ICommandUIState state) {
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultCommandUIState();
		}
		protected internal override void ModifyCell(TableCell tableCell) {
		}
	}
	#endregion
}
