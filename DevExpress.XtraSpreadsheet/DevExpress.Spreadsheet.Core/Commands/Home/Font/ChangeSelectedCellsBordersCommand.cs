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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChangeSelectedCellsOuterBordersCommand (abstract class)
	public abstract class ChangeSelectedCellsOutsideBordersCommand : SpreadsheetChangeSelectedCellsFormattingCommandBase {
		BorderModifier borderModifier;
		protected ChangeSelectedCellsOutsideBordersCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			this.borderModifier = CreateBorderModifier();
			SetupBorderModifier(borderModifier);
			base.ModifyDocumentModelCore(state);
		}
		protected internal virtual void SetupBorderModifier(BorderModifier borderModifier) {
			ColorModelInfo color = new ColorModelInfo();
			color.Rgb = DocumentModel.UiBorderInfoRepository.CurrentItem.Color;
			borderModifier.LineColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(color);
		}
		protected virtual BorderModifier CreateBorderModifier() {
			return new BorderModifier(ActiveSheet);
		}
		protected internal override void ModifyEntireSheet(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			borderModifier.ModifyEntireSheet(range, processedRanges);
		}
		protected internal override void ModifyColumns(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			CreateRowsCellsInterceptWithColumnCells(Selection.Sheet, range.TopLeft.Column, range.BottomRight.Column);
			borderModifier.ModifyColumns(range, processedRanges);
		}
		protected internal override void ModifyRows(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			CreateColumnCellsInterceptWithRowsCells(Selection.Sheet, range.TopLeft.Row, range.BottomRight.Row);
			borderModifier.ModifyRows(range, processedRanges);
		}
		void CreateColumnCellsInterceptWithRowsCells(Worksheet sheet, int topRowIndex, int bottomRowIndex) {
			IEnumerable<Column> existingColumns = sheet.Columns.GetExistingColumns();
			foreach (Column column in existingColumns) {
				for (int currentRowIndex = topRowIndex; currentRowIndex <= bottomRowIndex; currentRowIndex++) {
					int endColumnIndex = column.EndIndex;
					for (int currentColumnIndex = column.StartIndex; currentColumnIndex <= endColumnIndex; currentColumnIndex++)
						sheet.GetCellOrCreate(currentColumnIndex, currentRowIndex);
				}
			}
		}
		protected internal override void ModifyAllCells(CellRange range, IList<CellRange> processedRanges, ICommandUIState state) {
			borderModifier.ModifyAllCells(range, processedRanges);
		}
	}
	#endregion
	#region BorderModifier
	public class BorderModifier {
		readonly Worksheet sheet;
		int lineColorIndex;
		XlBorderLineStyle leftLineStyle;
		XlBorderLineStyle rightLineStyle;
		XlBorderLineStyle topLineStyle;
		XlBorderLineStyle bottomLineStyle;
		XlBorderLineStyle diagonalUpLineStyle; 
		XlBorderLineStyle diagonalDownLineStyle; 
		bool applyLeftBorder;
		bool applyRightBorder;
		bool applyTopBorder;
		bool applyBottomBorder;
		bool applyDiagonalUpBorder; 
		bool applyDiagonalDownBorder; 
		public BorderModifier(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
		public int LineColorIndex { get { return lineColorIndex; } set { lineColorIndex = value; } }
		public XlBorderLineStyle LeftLineStyle { get { return leftLineStyle; } set { leftLineStyle = value; } }
		public XlBorderLineStyle RightLineStyle { get { return rightLineStyle; } set { rightLineStyle = value; } }
		public XlBorderLineStyle TopLineStyle { get { return topLineStyle; } set { topLineStyle = value; } }
		public XlBorderLineStyle BottomLineStyle { get { return bottomLineStyle; } set { bottomLineStyle = value; } }
		public XlBorderLineStyle DiagonalUpLineStyle { get { return diagonalUpLineStyle; } set { diagonalUpLineStyle = value; } } 
		public XlBorderLineStyle DiagonalDownLineStyle { get { return diagonalDownLineStyle; } set { diagonalDownLineStyle = value; } } 
		public bool ApplyLeftBorder { get { return applyLeftBorder; } set { applyLeftBorder = value; } }
		public bool ApplyRightBorder { get { return applyRightBorder; } set { applyRightBorder = value; } }
		public bool ApplyTopBorder { get { return applyTopBorder; } set { applyTopBorder = value; } }
		public bool ApplyBottomBorder { get { return applyBottomBorder; } set { applyBottomBorder = value; } }
		public bool ApplyDiagonalUpBorder { get { return applyDiagonalUpBorder; } set { applyDiagonalUpBorder = value; } } 
		public bool ApplyDiagonalDownBorder { get { return applyDiagonalDownBorder; } set { applyDiagonalDownBorder = value; } } 
		public virtual void ModifyEntireSheet(CellRange range, IList<CellRange> processedRanges) {
			if (ApplyLeftBorder) {
				if (ModifyLeftExistingCells(range, processedRanges))
					ModifyLeftColumn(range);
			}
			if (ApplyTopBorder) {
				if (ModifyTopExistingCells(range, processedRanges))
					ModifyTopRow(range);
			}
		}
		public virtual void ModifyColumns(CellRange range, IList<CellRange> processedRanges) {
			if (ApplyTopBorder) {
				if (ModifyTopExistingCells(range, processedRanges))
					ModifyTopAllCells(range, processedRanges);
			}
			if (ApplyLeftBorder) {
				ModifyLeftColumn(range);
				ModifyLeftExistingCells(range, processedRanges);
			}
			if (ApplyRightBorder) {
				ModifyRightColumn(range);
				ModifyRightExistingCells(range, processedRanges);
			}
		}
		public virtual void ModifyRows(CellRange range, IList<CellRange> processedRanges) {
			if (ApplyLeftBorder) {
				ModifyLeftAllCells(range, processedRanges);
			}
			if (ApplyTopBorder) {
				ModifyTopRow(range);
				ModifyTopExistingCells(range, processedRanges);
			}
			if (ApplyBottomBorder) {
				ModifyBottomRow(range);
				ModifyBottomExistingCells(range, processedRanges);
			}
		}
		public virtual void ModifyAllCells(CellRange range, IList<CellRange> processedRanges) {
			if (ApplyLeftBorder)
				ModifyLeftAllCells(range, processedRanges);
			if (ApplyRightBorder)
				ModifyRightAllCells(range, processedRanges);
			if (ApplyTopBorder)
				ModifyTopAllCells(range, processedRanges);
			if (ApplyBottomBorder)
				ModifyBottomAllCells(range, processedRanges);
		}
		#region Modify(*)Column
		protected void ModifyColumnDiagonalBorders(int columnIndex) {
			if (ApplyDiagonalUpBorder)
				ModifyColumn(columnIndex, BorderSideAccessor.DiagonalUp, DiagonalUpLineStyle);
			if (ApplyDiagonalDownBorder)
				ModifyColumn(columnIndex, BorderSideAccessor.DiagonalDown, DiagonalDownLineStyle);
		}
		protected void ModifyLeftColumn(CellRange range) {
			int columnIndex = range.TopLeft.Column;
			ModifyColumn(columnIndex, BorderSideAccessor.Left, LeftLineStyle);
			ModifyColumnDiagonalBorders(columnIndex);
			if (columnIndex > 0) {
				IColumnRange columnRange = Sheet.Columns.TryGetColumnRange(columnIndex - 1);
				if (columnRange != null) {
					if (BorderSideAccessor.Right.GetLineStyle(columnRange.ActualBorder) != XlBorderLineStyle.None)
						ModifyColumn(columnIndex - 1, BorderSideAccessor.Right, XlBorderLineStyle.None);
				}
			}
		}
		protected void ModifyRightColumn(CellRange range) {
			int columnIndex = range.BottomRight.Column;
			ModifyColumn(columnIndex, BorderSideAccessor.Right, RightLineStyle);
			ModifyColumnDiagonalBorders(columnIndex);
			if (columnIndex + 1 < Sheet.MaxColumnCount) {
				IColumnRange columnRange = Sheet.Columns.TryGetColumnRange(columnIndex + 1);
				if (columnRange != null) {
					if (BorderSideAccessor.Left.GetLineStyle(columnRange.ActualBorder) != XlBorderLineStyle.None)
						ModifyColumn(columnIndex + 1, BorderSideAccessor.Left, XlBorderLineStyle.None);
				}
			}
		}
		protected void ModifyColumnsTop(CellRange range) {
			int lastIndex = range.BottomRight.Column;
			for (int i = range.TopLeft.Column; i <= lastIndex; i++) {
				ModifyColumn(i, BorderSideAccessor.Top, TopLineStyle);
				ModifyColumnDiagonalBorders(i);
			}
		}
		protected void ModifyColumn(int index, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			ApplyBorder(Sheet.Columns.GetIsolatedColumn(index).Border, accessor, lineStyle);
		}
		#endregion
		#region Modify(*)Row
		protected void ModifyRowDiagonalBorders(int rowIndex) {
			if (ApplyDiagonalUpBorder)
				ModifyRow(rowIndex, BorderSideAccessor.DiagonalUp, DiagonalUpLineStyle);
			if (ApplyDiagonalDownBorder)
				ModifyRow(rowIndex, BorderSideAccessor.DiagonalDown, DiagonalDownLineStyle);
		}
		protected void ModifyTopRow(CellRange range) {
			int rowIndex = range.TopLeft.Row;
			ModifyRow(rowIndex, BorderSideAccessor.Top, TopLineStyle);
			ModifyRowDiagonalBorders(rowIndex);
			if (rowIndex > 0) {
				Row row = Sheet.Rows.TryGetRow(rowIndex - 1);
				if (row != null) {
					if (BorderSideAccessor.Bottom.GetLineStyle(row.ActualBorder) != XlBorderLineStyle.None)
						ModifyRow(rowIndex - 1, BorderSideAccessor.Bottom, XlBorderLineStyle.None);
				}
			}
		}
		protected void ModifyBottomRow(CellRange range) {
			int rowIndex = range.BottomRight.Row;
			ModifyRow(rowIndex, BorderSideAccessor.Bottom, BottomLineStyle);
			ModifyRowDiagonalBorders(rowIndex);
			if (rowIndex + 1 < sheet.MaxRowCount) {
				Row row = Sheet.Rows.TryGetRow(rowIndex + 1);
				if (row != null) {
					if (BorderSideAccessor.Top.GetLineStyle(row.ActualBorder) != XlBorderLineStyle.None)
						ModifyRow(rowIndex + 1, BorderSideAccessor.Top, TopLineStyle);
				}
			}
		}
		protected void ModifyRowsLeft(CellRange range) {
			int lastIndex = range.BottomRight.Row;
			for (int i = range.TopLeft.Row; i <= lastIndex; i++) {
				ModifyRow(i, BorderSideAccessor.Left, LeftLineStyle);
				ModifyRowDiagonalBorders(i);
			}
		}
		protected void ModifyRow(int index, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			Row row = Sheet.Rows[index];
			ApplyBorder(row.Border, accessor, lineStyle);
		}
		#endregion
		#region Modify(*)Cells
		delegate bool ModifyRangeCellsMethod(CellRange range, IList<CellRange> processedRanges, BorderSideAccessor accessor, XlBorderLineStyle lineStyle);
		bool ModifyLeftCells(CellRange range, IList<CellRange> processedRanges, ModifyRangeCellsMethod modifyExistingCells, ModifyRangeCellsMethod modifyAllCells) {
			int columnIndex = range.TopLeft.Column;
			bool allCellsProcessed;
			allCellsProcessed = modifyAllCells(new CellRange(range.Worksheet, range.TopLeft, new CellPosition(columnIndex, range.BottomRight.Row)), processedRanges, BorderSideAccessor.Left, LeftLineStyle);
			if (columnIndex > 0) {
				ModifyRangeCellsMethod modify = modifyExistingCells;
				if (LeftLineStyle == XlBorderLineStyle.None) {
					IColumnRange columnRange = Sheet.Columns.TryGetColumnRange(columnIndex - 1);
					if (columnRange != null) {
						if (BorderSideAccessor.Right.GetLineStyle(columnRange.ActualBorder) != XlBorderLineStyle.None)
							modify = modifyAllCells;
					}
				}
				bool cellsProcessed;
				cellsProcessed = modify(new CellRange(range.Worksheet, new CellPosition(columnIndex - 1, range.TopLeft.Row), new CellPosition(columnIndex - 1, range.BottomRight.Row)), processedRanges, BorderSideAccessor.Right, XlBorderLineStyle.None);
				return allCellsProcessed || cellsProcessed;
			}
			else
				return allCellsProcessed;
		}
		bool ModifyTopCells(CellRange range, IList<CellRange> processedRanges, ModifyRangeCellsMethod modifyExistingCells, ModifyRangeCellsMethod modifyAllCells) {
			int rowIndex = range.TopLeft.Row;
			bool allCellsProcessed;
			allCellsProcessed = modifyAllCells(new CellRange(range.Worksheet, range.TopLeft, new CellPosition(range.BottomRight.Column, rowIndex)), processedRanges, BorderSideAccessor.Top, TopLineStyle);
			if (rowIndex > 0) {
				ModifyRangeCellsMethod modify = modifyExistingCells;
				if (TopLineStyle == XlBorderLineStyle.None) {
					Row row = Sheet.Rows.TryGetRow(rowIndex - 1);
					if (row != null) {
						if (BorderSideAccessor.Bottom.GetLineStyle(row.Border) != XlBorderLineStyle.None)
							modify = modifyAllCells;
					}
				}
				bool cellsProcessed;
				cellsProcessed = modify(new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, rowIndex - 1), new CellPosition(range.BottomRight.Column, rowIndex - 1)), processedRanges, BorderSideAccessor.Bottom, XlBorderLineStyle.None);
				return allCellsProcessed || cellsProcessed;
			}
			else
				return allCellsProcessed;
		}
		void ModifyRightCells(CellRange range, IList<CellRange> processedRanges, ModifyRangeCellsMethod modifyExistingCells, ModifyRangeCellsMethod modifyAllCells) {
			int columnIndex = range.BottomRight.Column;
			modifyAllCells(new CellRange(range.Worksheet, new CellPosition(columnIndex, range.TopLeft.Row), new CellPosition(columnIndex, range.BottomRight.Row)), processedRanges, BorderSideAccessor.Right, RightLineStyle);
			if (columnIndex + 1 < Sheet.MaxColumnCount) {
				ModifyRangeCellsMethod modify = modifyExistingCells;
				if (RightLineStyle == XlBorderLineStyle.None) {
					IColumnRange columnRange = Sheet.Columns.TryGetColumnRange(columnIndex + 1);
					if (columnRange != null) {
						if (BorderSideAccessor.Left.GetLineStyle(columnRange.ActualBorder) != XlBorderLineStyle.None)
							modify = modifyAllCells;
					}
				}
				modify(new CellRange(range.Worksheet, new CellPosition(columnIndex + 1, range.TopLeft.Row), new CellPosition(columnIndex + 1, range.BottomRight.Row)), processedRanges, BorderSideAccessor.Left, XlBorderLineStyle.None);
			}
		}
		bool ModifyBottomCells(CellRange range, IList<CellRange> processedRanges, ModifyRangeCellsMethod modifyExistingCells, ModifyRangeCellsMethod modifyAllCells) {
			int rowIndex = range.BottomRight.Row;
			bool allCellsProcessed;
			allCellsProcessed = modifyAllCells(new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, rowIndex), new CellPosition(range.BottomRight.Column, rowIndex)), processedRanges, BorderSideAccessor.Bottom, BottomLineStyle);
			if (rowIndex + 1 < sheet.MaxRowCount) {
				ModifyRangeCellsMethod modify = modifyExistingCells;
				if (BottomLineStyle == XlBorderLineStyle.None) {
					Row row = Sheet.Rows.TryGetRow(rowIndex + 1);
					if (row != null) {
						if (BorderSideAccessor.Top.GetLineStyle(row.Border) != XlBorderLineStyle.None)
							modify = modifyAllCells;
					}
				}
				bool cellsProcessed;
				cellsProcessed = modify(new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, rowIndex + 1), new CellPosition(range.BottomRight.Column, rowIndex + 1)), processedRanges, BorderSideAccessor.Top, XlBorderLineStyle.None);
				return allCellsProcessed || cellsProcessed;
			}
			else
				return allCellsProcessed;
		}
		#endregion
		#region Modify(*)AllCells
		protected void ModifyLeftAllCells(CellRange range, IList<CellRange> processedRanges) {
			ModifyLeftCells(range, processedRanges, ModifyExistingCells, ModifyAllCells);
		}
		protected void ModifyRightAllCells(CellRange range, IList<CellRange> processedRanges) {
			ModifyRightCells(range, processedRanges, ModifyExistingCells, ModifyAllCells);
		}
		protected void ModifyTopAllCells(CellRange range, IList<CellRange> processedRanges) {
			ModifyTopCells(range, processedRanges, ModifyExistingCells, ModifyAllCells);
		}
		protected void ModifyBottomAllCells(CellRange range, IList<CellRange> processedRanges) {
			ModifyBottomCells(range, processedRanges, ModifyExistingCells, ModifyAllCells);
		}
		protected bool ModifyAllCells(CellRange range, IList<CellRange> processedRanges, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			return ModifyCells(range.GetAllPositionsEnumerator(), processedRanges, accessor, lineStyle);
		}
		bool ModifyCells(IEnumerator<CellPosition> cellPositions, IList<CellRange> processedRanges, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			int count = 0;
			for (; ; ) {
				if (!cellPositions.MoveNext())
					break;
				CellPosition position = cellPositions.Current;
				if (CanModifyCell(position.Column, position.Row, processedRanges)) {
					ICell cell = Sheet[position.Column, position.Row];
					ModifyCell(cell, accessor, lineStyle);
					count++;
				}
			}
			return count > 0;
		}
		#endregion
		#region Modify(*)ExistingCells
		protected bool ModifyLeftExistingCells(CellRange range, IList<CellRange> processedRanges) {
			return ModifyLeftCells(range, processedRanges, ModifyExistingCells, ModifyExistingCells);
		}
		protected void ModifyRightExistingCells(CellRange range, IList<CellRange> processedRanges) {
			ModifyRightCells(range, processedRanges, ModifyExistingCells, ModifyExistingCells);
		}
		protected bool ModifyTopExistingCells(CellRange range, IList<CellRange> processedRanges) {
			return ModifyTopCells(range, processedRanges, ModifyExistingCells, ModifyExistingCells);
		}
		protected bool ModifyBottomExistingCells(CellRange range, IList<CellRange> processedRanges) {
			return ModifyBottomCells(range, processedRanges, ModifyExistingCells, ModifyExistingCells);
		}
		bool ModifyExistingCells(CellRange range, IList<CellRange> processedRanges, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			return ModifyCells(range.GetExistingCellsEnumerator(false), processedRanges, accessor, lineStyle);
		}
		bool ModifyCells(IEnumerator<ICellBase> cells, IList<CellRange> processedRanges, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			int count = 0;
			for (; ; ) {
				if (!cells.MoveNext())
					break;
				ICell cell = (ICell)cells.Current;
				if (cell == null)
					continue;
				if (CanModifyCell(cell.ColumnIndex, cell.RowIndex, processedRanges)) {
					ModifyCell(cell, accessor, lineStyle);
					count++;
				}
			}
			return count > 0;
		}
		protected virtual bool CanModifyCell(int column, int row, IList<CellRange> processedRanges) {
			int count = processedRanges.Count;
			if (count <= 0)
				return true;
			for (int i = 0; i < count; i++)
				if (processedRanges[i].ContainsCell(column, row))
					return false;
			return true;
		}
		#endregion
		void ModifyCell(ICell cell, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			ApplyBorder(cell.Border, accessor, lineStyle);
		}
		protected void ApplyBorder(IBorderInfo borders, BorderSideAccessor accessor, XlBorderLineStyle lineStyle) {
			if (lineStyle == XlBorderLineStyle.None)
				accessor.SetLineColorIndex(borders, 0);
			else
				accessor.SetLineColorIndex(borders, lineColorIndex);
			accessor.SetLineStyle(borders, lineStyle);
		}
	}
	#endregion
	#region AllBordersModifier
	public class AllBordersModifier : BorderModifier {
		public AllBordersModifier(Worksheet sheet)
			: base(sheet) {
		}
		public override void ModifyEntireSheet(CellRange range, IList<CellRange> processedRanges) {
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet == null)
				return;
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			ModifyExistingCells(sheet);
			ModifyExistingRows(sheet, topRow, bottomRow);
			ModifyExistingColumnRange(sheet, leftColumn, rightColumn);
			ModifyColumnRangesEnsureExist(sheet, leftColumn, rightColumn);
		}
		void ModifyExistingCells(Worksheet sheet) {
			foreach (ICellBase info in sheet.GetExistingCells()) {
				ICell cell = info as ICell;
				if (cell != null)
					ModifyBorder(cell.Border);
			}
		}
		void ModifyExistingRows(Worksheet sheet, int topRow, int bottomRow) {
			IEnumerable<Row> rows = sheet.Rows.GetExistingRows(topRow, bottomRow, false);
			foreach (Row row in rows)
				ModifyBorder(row.Border);
		}
		void ModifyExistingColumnRange(Worksheet sheet, int leftColumn, int rightColumn) {
			IEnumerator<Column> columns = sheet.Columns.GetExistingColumnsEnumerator(leftColumn, rightColumn, false);
			while (columns.MoveNext())
				ModifyBorder(columns.Current.Border);
		}
		void ModifyColumnRangesEnsureExist(Worksheet sheet, int leftColumn, int rightColumn) {
			IList<Column> columns = sheet.Columns.GetColumnRangesEnsureExist(leftColumn, rightColumn);
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				ModifyBorder(columns[i].Border);
		}
		void ModifyBorder(IBorderInfo border) {
			if (ApplyLeftBorder)
				ApplyBorder(border, BorderSideAccessor.Left, LeftLineStyle);
			if (ApplyRightBorder)
				ApplyBorder(border, BorderSideAccessor.Right, RightLineStyle);
			if (ApplyTopBorder)
				ApplyBorder(border, BorderSideAccessor.Top, TopLineStyle);
			if (ApplyBottomBorder)
				ApplyBorder(border, BorderSideAccessor.Bottom, BottomLineStyle);
			if (ApplyDiagonalUpBorder)
				ApplyBorder(border, BorderSideAccessor.DiagonalUp, DiagonalUpLineStyle);
			if (ApplyDiagonalDownBorder)
				ApplyBorder(border, BorderSideAccessor.DiagonalDown, DiagonalDownLineStyle);
		}
		public override void ModifyColumns(CellRange range, IList<CellRange> processedRanges) {
			processedRanges = new List<CellRange>();
			if (ApplyTopBorder) 
				ModifyColumnsTop(range);
			if (ApplyBottomBorder) 
				ModifyColumnsBottom(range);
			if (ApplyLeftBorder) {
				int leftColumn = range.TopLeft.Column;
				int rightColumn = range.BottomRight.Column;
				int topRow = range.TopLeft.Row;
				int bottomRow = range.BottomRight.Row;
				for (int i = leftColumn; i <= rightColumn; i++) {
					CellRange columnRange = new CellRange(range.Worksheet, new CellPosition(i, topRow), new CellPosition(i, bottomRow));
					ModifyLeftColumn(columnRange);
					ModifyLeftExistingCells(columnRange, processedRanges);
				}
			}
			if (ApplyRightBorder) {
				ModifyRightColumn(range);
				ModifyRightExistingCells(range, processedRanges);
			}
		}
		public override void ModifyRows(CellRange range, IList<CellRange> processedRanges) {
			processedRanges = new List<CellRange>();
			if (ApplyLeftBorder) {
				ModifyRowsLeft(range);
				ModifyLeftAllCells(range, processedRanges);
			}
			if (ApplyRightBorder) {
				ModifyRowsRight(range);
				ModifyRightAllCells(range, processedRanges);
			}
			if (ApplyTopBorder) {
				int leftColumn = range.TopLeft.Column;
				int rightColumn = range.BottomRight.Column;
				int topRow = range.TopLeft.Row;
				int bottomRow = range.BottomRight.Row;
				for (int i = topRow; i <= bottomRow; i++) {
					CellRange rowRange = new CellRange(range.Worksheet, new CellPosition(leftColumn, i), new CellPosition(rightColumn, i));
					ModifyTopRow(rowRange);
					ModifyTopExistingCells(rowRange, processedRanges);
				}
			}
			if (ApplyBottomBorder) {
				ModifyBottomRow(range);
				ModifyBottomExistingCells(range, processedRanges);
			}
		}
		public override void ModifyAllCells(CellRange range, IList<CellRange> processedRanges) {
			if (ApplyLeftBorder) {
				int leftColumn = range.TopLeft.Column;
				int rightColumn = range.BottomRight.Column;
				int topRow = range.TopLeft.Row;
				int bottomRow = range.BottomRight.Row;
				for (int i = leftColumn; i <= rightColumn; i++) {
					CellRange columnRange = new CellRange(range.Worksheet, new CellPosition(i, topRow), new CellPosition(i, bottomRow));
					ModifyLeftAllCells(columnRange, processedRanges);
				}
			}
			if (ApplyRightBorder)
				ModifyRightAllCells(range, processedRanges);
			if (ApplyTopBorder) {
				int leftColumn = range.TopLeft.Column;
				int rightColumn = range.BottomRight.Column;
				int topRow = range.TopLeft.Row;
				int bottomRow = range.BottomRight.Row;
				for (int i = topRow; i <= bottomRow; i++) {
					CellRange rowRange = new CellRange(range.Worksheet, new CellPosition(leftColumn, i), new CellPosition(rightColumn, i));
					ModifyTopAllCells(rowRange, processedRanges);
				}
			}
			if (ApplyBottomBorder)
				ModifyBottomAllCells(range, processedRanges);
			if (ApplyDiagonalUpBorder)
				ModifyAllCells(range, processedRanges, BorderSideAccessor.DiagonalUp, DiagonalUpLineStyle);
			if (ApplyDiagonalDownBorder)
				ModifyAllCells(range, processedRanges, BorderSideAccessor.DiagonalDown, DiagonalDownLineStyle);
		}
		protected override bool CanModifyCell(int column, int row, IList<CellRange> processedRanges) {
			return true;
		}
		void ModifyColumnsBottom(CellRange range) {
			int lastIndex = range.BottomRight.Column;
			for (int i = range.TopLeft.Column; i <= lastIndex; i++)
				ModifyColumn(i, BorderSideAccessor.Bottom, BottomLineStyle);
		}	  
		void ModifyRowsRight(CellRange range) {
			int lastIndex = range.BottomRight.Row;
			for (int i = range.TopLeft.Row; i <= lastIndex; i++) 
				ModifyRow(i, BorderSideAccessor.Right, RightLineStyle);
		}
	}
	#endregion
}
