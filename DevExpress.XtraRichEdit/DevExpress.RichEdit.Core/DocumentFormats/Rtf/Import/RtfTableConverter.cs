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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf.Tables {
	#region ProcessStatus
	public enum ProcessStatus {
		Continue,
		Restart,
		Skip
	}
	#endregion
	#region RtfTablePropertiesCalculator
	#endregion
	#region RtfTableConverter
	public class RtfTableConverter {
		readonly RtfTableReader tableReader;
		readonly Dictionary<RtfTableCell, TableCell> rtfCellMap;
		RtfTableCollection tablesQueue;
		public RtfTableConverter(RtfTableReader tableReader) {
			Guard.ArgumentNotNull(tableReader, "tableReader");
			this.tableReader = tableReader;
			this.rtfCellMap = new Dictionary<RtfTableCell, TableCell>();
		}
		public RtfTableReader TableReader { get { return tableReader; } }
		PieceTable PieceTable { get { return TableReader.Importer.PieceTable; } }
		public void ConvertTables(RtfTableCollection rtfTables, bool isCopySingleCellAsText) {
			this.rtfCellMap.Clear();
			this.tablesQueue = new RtfTableCollection(rtfTables);
			if (!(IsOnlySingleCellInTable() && isCopySingleCellAsText)) {
				while (tablesQueue.Count > 0) {
					RtfTable table = tablesQueue.First;
					tablesQueue.RemoveAt(0);
					ConvertTable(table);
				}
			}
		}
		protected internal virtual bool IsOnlySingleCellInTable() {
			if (tablesQueue.Count > 1)
				return false;
			RtfTable table = tablesQueue.First;
			if (table.Rows.Count > 1 || table.Rows.Count == 0)
				return false;
			RtfTableRow firstRow = table.Rows[0];
			if (firstRow.Cells.Count > 1)
				return false;
			RtfTableCell cell = firstRow.Cells[0];
			if (cell.StartParagraphIndex != ParagraphIndex.Zero || cell.EndParagraphIndex != new ParagraphIndex(PieceTable.Paragraphs.Count - 2))
				return false;
			return true;
		}
		void ConvertTable(RtfTable rtfTable) {
			if (!RtfTableIsValid(rtfTable)) {
				return;
			}
			TableCell parentCell = null;
			if (rtfTable.ParentCell != null) {
				Debug.Assert(rtfCellMap.ContainsKey(rtfTable.ParentCell));
				parentCell = rtfCellMap[rtfTable.ParentCell];
			}
			PrepareRtfTable(rtfTable);
			Table table = new Table(PieceTable, parentCell, 0, 0);
			ConvertTableCore(table, rtfTable);
			PieceTable.Tables.Add(table);
		}
		bool RtfTableIsValid(RtfTable rtfTable) {
			if (rtfTable.Rows.Count == 0)
				return false;
			RtfTableRow lastRow = rtfTable.Rows.Last;
			if (lastRow.Cells.Count > 0)
				return true;
			rtfTable.Rows.Remove(lastRow);
			return RtfTableIsValid(rtfTable);
		}
		protected internal virtual void PrepareRtfTable(RtfTable table) {
			if (ShouldUseFloatingPosition(table)) {
				RtfTableRow row = table.Rows.First;
				Debug.Assert(row != null);
				table.Properties.FloatingPosition.CopyFrom(row.Properties.FloatingPosition);
			}
			WidthUnit tablePreferredWidth = table.Properties.PreferredWidth;
			if (tablePreferredWidth.Type == WidthUnitType.Nil) {
				tablePreferredWidth.Type = WidthUnitType.Auto;
				tablePreferredWidth.Value = 0;
			}
			if (!table.Properties.UseRightMargin && !table.Properties.UseLeftMargin && table.Properties.UseHalfSpace) {
				int margin = table.Properties.HalfSpace;
				table.Properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
				table.Properties.CellMargins.Left.Value = margin;
				table.Properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
				table.Properties.CellMargins.Right.Value = margin;
			}
			table.Indent = CalculateTableLeftOffset(table);
			WidthUnit indent = table.Properties.TableIndent;
			if (indent.Type != WidthUnitType.ModelUnits) {
				indent.Value = CalculateTableIndent(table);
				indent.Type = WidthUnitType.ModelUnits;
			}
		}
		protected internal virtual bool ShouldUseFloatingPosition(RtfTable table) {
			bool result = true;
			TableFloatingPositionInfo previous = null;
			foreach (RtfTableRow row in table.Rows) {
				TableFloatingPositionInfo current = row.Properties.FloatingPosition;
				if (previous != null)
					result &= current.Equals(previous);
				previous = current;
			}
			return result;
		}
		protected internal virtual int CalculateTableLeftOffset(RtfTable table) {
			RtfTableRowCollection rows = table.Rows;
			int result = rows[0].Left;
			int rowsCount = rows.Count;
			for (int i = 1; i < rowsCount; i++)
				result = Math.Min(result, rows[i].Left);
			return result;
		}
		protected internal virtual int CalculateTableIndent(RtfTable table) {
			RtfTableCellProperties cellProperties = table.Rows.First.Cells.First.Properties;
			BorderBase leftBorder = cellProperties.Borders.LeftBorder;
			WidthUnit leftMargin = GetCellLeftMargin(table);
			TableBorderCalculator calculator = new TableBorderCalculator();
			int borderWidth = calculator.GetActualWidth(leftBorder);
			return Math.Max(borderWidth / 2, GetActualWidth(leftMargin.Info)) + table.Indent;
		}
		WidthUnit GetCellLeftMargin(RtfTable table) {
			RtfTableCellProperties cellProperties = table.Rows.First.Cells.First.Properties;
			if (cellProperties.CellMargins.Left.Info.Type == WidthUnitType.ModelUnits)
				return cellProperties.CellMargins.Left;
			return table.Properties.CellMargins.Left;
		}
		int GetActualWidth(WidthUnitInfo unitInfo) {
			if (unitInfo.Type == WidthUnitType.ModelUnits)
				return unitInfo.Value;
			return 0;
		}
		void ConvertTableCore(Table table, RtfTable rtfTable) {
			table.TableProperties.CopyFrom(rtfTable.Properties);
			table.SetTableStyleIndexCore(rtfTable.Properties.TableStyleIndex);
			table.TableProperties.FloatingPosition.CopyFrom(rtfTable.Properties.FloatingPosition);
			RtfTableGrid tableGrid = CalculateTableGrid(rtfTable);
			TableRowCollection rows = table.Rows;
			RtfTableRowCollection rtfRows = rtfTable.Rows;
			int count = rtfRows.Count;
			TableLayoutType tableLayoutType = table.TableLayout;			
			for (int i = 0; i < count; i++) {
				PrepareRtfRow(rtfRows[i], tableGrid, tableLayoutType);
				TableRow row = new TableRow(table);
				rows.AddInternal(row);
				ConvertRow(row, rtfRows[i]);
			}
		}
		protected internal virtual RtfTableGrid CalculateTableGrid(RtfTable table) {
			RtfTableColumnsCalculator calculator = new RtfTableColumnsCalculator();
			return calculator.Calculate(table, table.Indent);
		}
		protected internal virtual void PrepareRtfRow(RtfTableRow row, RtfTableGrid grid, TableLayoutType tableLayoutType) {
			int gridBefore = grid.BinarySearchLeft(row.Left);
			Debug.Assert(gridBefore >= 0);
			row.Properties.GridBefore = gridBefore;
			if (row.Properties.WidthBefore.Value == 0 && gridBefore > 0) {
				row.Properties.WidthBefore.Value = row.Offset;
				row.Properties.WidthBefore.Type = WidthUnitType.ModelUnits;
			}
			PrepareRtfRowCells(row, grid, gridBefore, tableLayoutType);
			int lastColumnIndex = grid.Count - 1;
			int gridAfter = lastColumnIndex - CalculateRowColumnSpan(row);
			row.Properties.GridAfter = gridAfter;
			if (row.Properties.WidthAfter.Value == 0 && gridAfter > 0) {
				int widthAfter = 1;
				if (row.Properties.WidthAfter.Type == WidthUnitType.ModelUnits) {
					widthAfter = CalculateWidthAfter(row);
				}
				else
					widthAfter = (int)(grid[lastColumnIndex] - grid[lastColumnIndex - gridAfter]);
				row.Properties.WidthAfter.Value = widthAfter;
				row.Properties.WidthAfter.Type = WidthUnitType.ModelUnits;
			}
		}
		private int CalculateWidthAfter(RtfTableRow row) {
			int rowWidth = CalculateTotalRowWidth(row);
			if (rowWidth < 0)
				return 1;
			RtfTableRowCollection rows = row.Table.Rows;
			int maxWidth = 0;
			for (int i = 0; i < rows.Count; i++) {
				RtfTableRow currentRow = rows[i];
				if (Object.ReferenceEquals(currentRow, row))
					continue;
				int currentRowWidth = CalculateTotalRowWidth(currentRow);
				if (currentRowWidth > 0)
					maxWidth = Math.Max(maxWidth, currentRowWidth);
			}
			return Math.Max(1, rowWidth - maxWidth);
		}
		private int CalculateTotalRowWidth(RtfTableRow row) {
			int totalWidth = 0;
			RtfTableCellCollection cells = row.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				PreferredWidth width = cells[i].Properties.PreferredWidth;
				if (width.Type == WidthUnitType.ModelUnits)
					totalWidth += width.Value;
				else
					return -1;
			}
			return totalWidth;
		}
		void PrepareRtfRowCells(RtfTableRow row, RtfTableGrid grid, int gridBefore, TableLayoutType tableLayoutType) {
			int prevBorderIndex = gridBefore;
			int left = grid[gridBefore];
			for (int i = 0; i < row.Cells.Count; i++) {
				RtfTableCell cell = row.Cells[i];
				int right = cell.Right > left ? cell.Right : left;
				int borderIndex = grid.BinarySearchRight(right) - CalculateEquidistantCellOrder(row.Cells, i, right);
				int columnSpan = borderIndex - prevBorderIndex;
				cell.Properties.ColumnSpan = columnSpan;
				WidthUnit preferredWidth = cell.Properties.PreferredWidth;
				if (preferredWidth.Type == WidthUnitType.Nil || (preferredWidth.Type == WidthUnitType.Auto && tableLayoutType != TableLayoutType.Autofit)) {
					preferredWidth.Value = (int)(grid[borderIndex] - grid[prevBorderIndex]);
					preferredWidth.Type = WidthUnitType.ModelUnits;
				}
				prevBorderIndex = Math.Max(borderIndex, prevBorderIndex);
				left = right;
			}
		}
		int CalculateEquidistantCellOrder(RtfTableCellCollection cells, int index, int left) {
			int count = cells.Count;
			int equidistantCellsCount = 0;
			for (int i = index + 1; i < count; i++) {
				if (cells[i].Right > left)
					break;
				equidistantCellsCount++;
			}
			return equidistantCellsCount;
		}
		int CalculateRowColumnSpan(RtfTableRow row) {
			int result = 0;
			int cellsCount = row.Cells.Count;
			for (int i = 0; i < cellsCount; i++)
				result += row.Cells[i].Properties.ColumnSpan;
			return result;
		}
		void ConvertRow(TableRow row, RtfTableRow rtfRow) {
			row.Properties.CopyFrom(rtfRow.Properties);
			RtfTableCellCollection rtfCells = rtfRow.Cells;
			for (int i = 0; i < rtfCells.Count; i++)
				ConvertCell(row, rtfCells[i]);
		}
		void ConvertCell(TableRow row, RtfTableCell rtfCell) {
			if (rtfCell.Properties.HorizontalMerging == MergingState.Restart)
				MergeCells(rtfCell);
			TableCell cell = new TableCell(row);
			row.Cells.AddInternal(cell);
			cell.Properties.BeginInit();
			cell.Properties.CopyFrom(rtfCell.Properties);
			cell.Properties.EndInit();
			ParagraphIndex startParagraphIndex = rtfCell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = rtfCell.EndParagraphIndex;
			PieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, endParagraphIndex);
			rtfCellMap.Add(rtfCell, cell);
		}
		void MergeCells(RtfTableCell firstCell) {
			int nextIndex = firstCell.Index + 1;
			RtfTableCellCollection cells = firstCell.Row.Cells;
			while (nextIndex < cells.Count && cells[nextIndex].Properties.HorizontalMerging == MergingState.Continue) {
				RtfTableCell nextRtfCell = cells[nextIndex];
				Debug.Assert(Object.ReferenceEquals(firstCell.Row, nextRtfCell.Row));
				Debug.Assert((nextRtfCell.Index - firstCell.Index) == 1);
				RtfParentCellMap parentCellMap = TableReader.ParentCellMap;
				if (parentCellMap.ContainsKey(nextRtfCell)) {
					RtfTableCollection tables = parentCellMap[nextRtfCell];
					foreach (RtfTable table in tables)
						this.tablesQueue.Remove(table);
					parentCellMap.Remove(nextRtfCell);
				}
				firstCell.Properties.ColumnSpan += nextRtfCell.Properties.ColumnSpan;
				RemoveCell(cells, nextRtfCell);
			}
		}
		void RemoveCell(RtfTableCellCollection cells, RtfTableCell cell) {
			cells.Remove(cell);
			int count = cells.Count;
			for (int i = cell.Index; i < count; i++)
				cells[i].Index--;
			DocumentLogPosition start = PieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
			DocumentLogPosition end = PieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition;
			int length = end - start + 1;
#if DEBUGTEST
			int delta = cell.EndParagraphIndex - cell.StartParagraphIndex + 1;
			int paragraphsCount = PieceTable.Paragraphs.Count;
#endif
			PieceTable.DeleteContent(start, length, false);
#if DEBUGTEST
			Debug.Assert((PieceTable.Paragraphs.Count + delta) == paragraphsCount);
#endif
			RecalcParagraphIndexes(cell);
		}
		void RecalcParagraphIndexes(RtfTableCell removedCell) {
			ParagraphIndex removedStartParagraphIndex = removedCell.StartParagraphIndex;
			int delta = removedCell.EndParagraphIndex - removedStartParagraphIndex + 1;
			RecalcParagraphIndexesInRow(removedCell.Row, removedCell.Index, delta);
			RtfTable table = removedCell.Row.Table;
			int rowIndex = table.Rows.IndexOf(removedCell.Row);
			RecalcParagraphIndexesInTable(table, rowIndex + 1, delta, removedStartParagraphIndex);
			RecalcParagraphIndexesInTables(delta, removedStartParagraphIndex);
		}
		void RecalcParagraphIndexesInRow(RtfTableRow row, int cellIndex, int delta) {
			int count = row.Cells.Count;
			for (int i = cellIndex; i < count; i++) {
				row.Cells[i].StartParagraphIndex -= delta;
				row.Cells[i].EndParagraphIndex -= delta;
				Debug.Assert(row.Cells[i].StartParagraphIndex >= ParagraphIndex.Zero);
				Debug.Assert(row.Cells[i].EndParagraphIndex >= ParagraphIndex.Zero);
			}
		}
		void RecalcParagraphIndexesInTable(RtfTable table, int rowIndex, int delta, ParagraphIndex paragraphIndex) {
			if (table.Rows.Last.Cells.Last.EndParagraphIndex <= paragraphIndex)
				return;
			int count = table.Rows.Count;
			for (int i = rowIndex; i < count; i++)
				RecalcParagraphIndexesInRow(table.Rows[i], 0, delta);
		}
		void RecalcParagraphIndexesInTables(int delta, ParagraphIndex paragraphIndex) {
			int count = tablesQueue.Count;
			for (int i = 0; i < count; i++)
				RecalcParagraphIndexesInTable(tablesQueue[i], 0, delta, paragraphIndex);
		}
	}
	#endregion
	#region TablePropertiesConverter
	public static class TablePropertyUnitsToModelUnitsConverter {
		public static void Convert(Table table, DocumentModelUnitConverter unitConverter) {
			TableProperties properties = table.TableProperties;
			ConvertTableBorders(properties.Borders, unitConverter);
			ConvertCellMargins(properties.CellMargins, unitConverter);
			if (properties.UseCellSpacing)
				ConvertWidthUnit(properties.CellSpacing, unitConverter);
			if (properties.UsePreferredWidth)
				ConvertWidthUnit(properties.PreferredWidth, unitConverter);
			if (properties.UseTableIndent)
				ConvertWidthUnit(properties.TableIndent, unitConverter);
			if (properties.UseFloatingPosition)
				ConvertFloatingPosition(properties.FloatingPosition, unitConverter);
		}
		static void ConvertFloatingPosition(TableFloatingPosition floatingPosition, DocumentModelUnitConverter unitConverter) {
			floatingPosition.BottomFromText = TwipsToModelUnits(floatingPosition.BottomFromText, unitConverter);
			floatingPosition.LeftFromText = TwipsToModelUnits(floatingPosition.LeftFromText, unitConverter);
			floatingPosition.RightFromText = TwipsToModelUnits(floatingPosition.RightFromText, unitConverter);
			floatingPosition.TopFromText = TwipsToModelUnits(floatingPosition.TopFromText, unitConverter);
			floatingPosition.TableHorizontalPosition = TwipsToModelUnits(floatingPosition.TableHorizontalPosition, unitConverter);
			floatingPosition.TableVerticalPosition = TwipsToModelUnits(floatingPosition.TableVerticalPosition, unitConverter);
		}
		public static void Convert(TableRow row, DocumentModelUnitConverter unitConverter) {
			TableRowProperties properties = row.Properties;
			if (properties.UseCellSpacing)
				ConvertWidthUnit(properties.CellSpacing, unitConverter);
			if (properties.UseWidthBefore)
				ConvertWidthUnit(properties.WidthBefore, unitConverter);
			if (properties.UseWidthAfter)
				ConvertWidthUnit(properties.WidthAfter, unitConverter);
			if (properties.UseHeight)
				ConvertHeightUnit(properties.Height, unitConverter);
		}
		public static void Convert(TableCell cell, DocumentModelUnitConverter unitConverter) {
			TableCellProperties properties = cell.Properties;
			ConvertCellBorders(properties.Borders, unitConverter);
			ConvertCellMargins(properties.CellMargins, unitConverter);
			if (properties.UsePreferredWidth)
				ConvertWidthUnit(properties.PreferredWidth, unitConverter);
		}
		static void ConvertCellBorders(TableCellBorders cellBorders, DocumentModelUnitConverter unitConverter) {
			if (cellBorders.UseBottomBorder)
				ConvertBorderWidth(cellBorders.BottomBorder, unitConverter);
			if (cellBorders.UseLeftBorder)
				ConvertBorderWidth(cellBorders.LeftBorder, unitConverter);
			if (cellBorders.UseRightBorder)
				ConvertBorderWidth(cellBorders.RightBorder, unitConverter);
			if (cellBorders.UseTopBorder)
				ConvertBorderWidth(cellBorders.TopBorder, unitConverter);
			if (cellBorders.UseInsideHorizontalBorder)
				ConvertBorderWidth(cellBorders.InsideHorizontalBorder, unitConverter);
			if (cellBorders.UseInsideVerticalBorder)
				ConvertBorderWidth(cellBorders.InsideVerticalBorder, unitConverter);
			if (cellBorders.UseTopLeftDiagonalBorder)
				ConvertBorderWidth(cellBorders.TopLeftDiagonalBorder, unitConverter);
			if (cellBorders.UseTopRightDiagonalBorder)
				ConvertBorderWidth(cellBorders.TopRightDiagonalBorder, unitConverter);
		}
		static void ConvertCellMargins(CellMargins cellMargins, DocumentModelUnitConverter unitConverter) {
			if (cellMargins.UseTopMargin)
				ConvertWidthUnit(cellMargins.Top, unitConverter);
			if (cellMargins.UseBottomMargin)
				ConvertWidthUnit(cellMargins.Bottom, unitConverter);
			if (cellMargins.UseLeftMargin)
				ConvertWidthUnit(cellMargins.Left, unitConverter);
			if (cellMargins.UseRightMargin)
				ConvertWidthUnit(cellMargins.Right, unitConverter);
		}
		static void ConvertTableBorders(TableBorders tableBorders, DocumentModelUnitConverter unitConverter) {
			if (tableBorders.UseBottomBorder)
				ConvertBorderWidth(tableBorders.BottomBorder, unitConverter);
			if (tableBorders.UseLeftBorder)
				ConvertBorderWidth(tableBorders.LeftBorder, unitConverter);
			if (tableBorders.UseRightBorder)
				ConvertBorderWidth(tableBorders.RightBorder, unitConverter);
			if (tableBorders.UseTopBorder)
				ConvertBorderWidth(tableBorders.TopBorder, unitConverter);
			if (tableBorders.UseInsideHorizontalBorder)
				ConvertBorderWidth(tableBorders.InsideHorizontalBorder, unitConverter);
			if (tableBorders.UseInsideVerticalBorder)
				ConvertBorderWidth(tableBorders.InsideVerticalBorder, unitConverter);
		}
		static void ConvertBorderWidth(BorderBase border, DocumentModelUnitConverter unitConverter) {
			border.Width = unitConverter.TwipsToModelUnits(border.Width);
		}
		static void ConvertWidthUnit(WidthUnit widthUnit, DocumentModelUnitConverter unitConverter) {
			widthUnit.Value = widthUnit.Type == WidthUnitType.ModelUnits ? TwipsToModelUnits(widthUnit.Value, unitConverter) : widthUnit.Value;
		}
		static void ConvertHeightUnit(HeightUnit heightUnit, DocumentModelUnitConverter unitConverter) {
			heightUnit.Value = TwipsToModelUnits(heightUnit.Value, unitConverter);
		}
		static int TwipsToModelUnits(int value, DocumentModelUnitConverter unitConverter) {
			return unitConverter.TwipsToModelUnits(value);
		}
	}
	#endregion
	#region Old properties
	#endregion
	#region RtfTableCellProperties
	public class RtfTableCellProperties : TableCellProperties {
		int right;
		MergingState horizontalMerging;
		public RtfTableCellProperties(PieceTable pieceTable, ICellPropertiesOwner owner)
			: base(pieceTable, owner) {
		}
		public int Right { get { return right; } set { right = value; } }
		public MergingState HorizontalMerging { get { return horizontalMerging; } set { horizontalMerging = value; } }
	}
	#endregion
	#region RtfCellSpacing
	public class RtfCellSpacing {
		readonly WidthUnit cellSpacing;
		public RtfCellSpacing(WidthUnit cellSpacing) {
			this.cellSpacing = cellSpacing;
		}
		public WidthUnit Left { get { return cellSpacing; } }
		public WidthUnit Top { get { return cellSpacing; } }
		public WidthUnit Right { get { return cellSpacing; } }
		public WidthUnit Bottom { get { return cellSpacing; } }
		internal WidthUnit CellSpacing { get { return cellSpacing; } }
	}
	#endregion
	#region RtfTableRowProperties
	public class RtfTableRowProperties : TableRowProperties {
		int left;
		readonly TableFloatingPositionInfo floatingPosition;
		readonly RtfCellSpacing cellSpacing;
		public RtfTableRowProperties(PieceTable pieceTable)
			: base(pieceTable) {
			this.floatingPosition = pieceTable.DocumentModel.Cache.TableFloatingPositionInfoCache.DefaultItem.Clone();
			this.floatingPosition.HorizontalAnchor = HorizontalAnchorTypes.Column;
			this.cellSpacing = new RtfCellSpacing(base.CellSpacing);
		}
		public int Left { get { return left; } set { left = value; } }
		public TableFloatingPositionInfo FloatingPosition { get { return floatingPosition; } }
		public new RtfCellSpacing CellSpacing { get { return cellSpacing; } }
	}
	#endregion
	#region RtfTableProperties
	public class RtfTableProperties : TableProperties {
		readonly RtfCellSpacing cellSpacing;
		int tableStyleIndex;
		int halfSpace;
		bool useHalfSpace;
		public RtfTableProperties(PieceTable pieceTable)
			: base(pieceTable) {
			this.cellSpacing = new RtfCellSpacing(base.CellSpacing);
			this.TableLayout = TableLayoutType.Fixed;
		}
		public new RtfCellSpacing CellSpacing { get { return cellSpacing; } }
		public int TableStyleIndex { get { return tableStyleIndex; } set { tableStyleIndex = value; } }
		public int HalfSpace {
			get { return halfSpace; }
			set {
				halfSpace = value;
				useHalfSpace = true;
			}
		}
		public bool UseHalfSpace { get { return useHalfSpace; } }
		public bool IsChanged() {
			return this.UseHalfSpace ||
				this.UseLeftMargin ||
				this.UseRightMargin ||
				this.UseTopMargin ||
				this.UseBottomMargin ||
				this.UseCellSpacing ||
				this.UseIsTableOverlap ||
				this.UsePreferredWidth ||
				this.UseTableIndent ||
				(this.TableLayout == TableLayoutType.Autofit) ||
				this.UseTableLook ||
				this.UseTableStyleColBandSize ||
				this.UseTableStyleRowBandSize ||
				this.Borders.UseBottomBorder ||
				this.Borders.UseInsideHorizontalBorder ||
				this.Borders.UseInsideVerticalBorder ||
				this.Borders.UseLeftBorder ||
				this.Borders.UseRightBorder ||
				this.Borders.UseTopBorder;
		}
		public override void CopyFrom(TableProperties newProperties) {
			base.CopyFrom(newProperties);
			RtfTableProperties sourceProperties = newProperties as RtfTableProperties;
			if (sourceProperties != null && sourceProperties.UseHalfSpace)
				HalfSpace = sourceProperties.HalfSpace;
		}
	}
	#endregion
	#region RtfTable
	public class RtfTable {
		#region Fields
		RtfTableProperties tableProperties;
		readonly RtfTableRowCollection rows;
		RtfTableCell parentCell;
		int right;
		int indent;
		#endregion
		public RtfTable(RtfImporter importer, RtfTableCell parentCell) {
			Guard.ArgumentNotNull(importer, "importer");
			this.tableProperties = new RtfTableProperties(importer.PieceTable);
			this.rows = new RtfTableRowCollection();
			this.parentCell = parentCell;
		}
		public RtfTable(RtfImporter importer)
			: this(importer, null) {
		}
		#region Properties
		public RtfTableProperties Properties { get { return tableProperties; } internal set { tableProperties = value; } }
		public RtfTableRowCollection Rows { get { return rows; } }
		public RtfTableCell ParentCell { get { return parentCell; } set { parentCell = value; } }
		public int NestingLevel { get { return GetNestedLevel(); } }
		internal int Right {
			get { return right; }
			set {
				Guard.ArgumentNonNegative(value, "Right");
				right = value;
			}
		}
		public int Indent { get { return indent; } set { indent = value; } }
		#endregion
		protected virtual internal int GetNestedLevel() {
			int nestingLevel = 1;
			RtfTableCell parentCell = ParentCell;
			while (parentCell != null) {
				Debug.Assert(parentCell.Row != null);
				Debug.Assert(parentCell.Row.Table != null);
				RtfTable parentTable = parentCell.Row.Table;
				parentCell = parentTable.ParentCell;
				nestingLevel++;
			}
			return nestingLevel;
		}
	}
	#endregion
	#region RtfTableRow
	public class RtfTableRow {
		#region Fields
		readonly RtfTableCellCollection cells;
		int offset;
		int left;
		int index = -1;
		int gridBefore;
		bool defineProperties;
		RtfTableRowProperties rowProperties;
		RtfTable table;
		#endregion
		public RtfTableRow(RtfTable table, RtfImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.rowProperties = new RtfTableRowProperties(importer.PieceTable);
			this.table = table;
			this.cells = new RtfTableCellCollection();
		}
		#region Properties
		public RtfTableRowProperties Properties { get { return rowProperties; } internal set { rowProperties = value; } }
		public RtfTableCellCollection Cells { get { return cells; } }
		public int Left { get { return left; } set { left = value; } }
		public RtfTable Table { get { return table; } set { table = value; } }
		internal int Offset { get { return offset; } set { offset = value; } }
		internal int Index { get { return index; } set { index = value; } }
		internal int GridBefore { get { return gridBefore; } set { gridBefore = value; } }
		internal bool DefineProperties { get { return defineProperties; } set { defineProperties = value; } }
		internal int Right {
			get {
				int cellsCount = Cells.Count;
				if (cellsCount > 0) {
					return Cells[cellsCount - 1].Right;
				}
				return 0;
			}
		}
		#endregion
		public void CopyFrom(RtfTableRow row) {
			Offset = row.Offset;
			Left = row.Left;
			GridBefore = row.GridBefore;
			Properties.CopyFrom(row.Properties);
			int cellsCount = Math.Min(row.Cells.Count, this.Cells.Count);
			for (int i = 0; i < cellsCount; i++)
				Cells[i].CopyFrom(row.Cells[i]);
		}
	}
	#endregion
	#region RtfTableRowCollection
	public class RtfTableRowCollection : List<RtfTableRow> {
		public RtfTableRow First { get { return Count > 0 ? this[0] : null; } }
		public RtfTableRow Last { get { return Count > 0 ? this[Count - 1] : null; } }
	}
	#endregion
	#region RtfTableCell
	public class RtfTableCell : ICellPropertiesOwner {
		#region Fields
		int right;
		RtfTableCellProperties cellProperties;
		RtfTableRow row;
		ParagraphIndex startParagraphIndex = new ParagraphIndex(-1);
		ParagraphIndex endParagraphIndex = new ParagraphIndex(-1);
		int width;
		int index = -1;
		#endregion
		public RtfTableCell(RtfTableRow row, RtfImporter importer, int right) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNonNegative(right, "right");
			this.right = right;
			this.row = row;
			this.cellProperties = new RtfTableCellProperties(importer.PieceTable, this);
		}
		public RtfTableCell(RtfTableRow row, RtfImporter importer)
			: this(row, importer, 0) {
		}
		#region Properties
		public RtfTableCellProperties Properties { get { return cellProperties; } internal set { cellProperties = value; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public int Right { get { return right; } set { right = value; } }
		public RtfTableRow Row { get { return row; } set { row = value; } }
		internal bool IsEmpty { get { return (StartParagraphIndex == new ParagraphIndex(-1) || EndParagraphIndex == new ParagraphIndex(-1)); } }
		internal int DefinedWidth { get { return width; } set { width = value; } }
		internal int Index { get { return index; } set { index = value; } }
		#endregion
		public void CopyFrom(RtfTableCell cell) {
			Right = cell.Right;
			DefinedWidth = cell.DefinedWidth;
			Properties.CopyFrom(cell.Properties);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == cellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region RtfTableCellCollection
	public class RtfTableCellCollection : List<RtfTableCell> {
		public RtfTableCellCollection()
			: base() {
		}
		public RtfTableCellCollection(IEnumerable<RtfTableCell> collection)
			: base(collection) {
		}
		public RtfTableCell First { get { return Count > 0 ? this[0] : null; } }
		public RtfTableCell Last { get { return Count > 0 ? this[Count - 1] : null; } }
	}
	#endregion
	#region RtfTableGrid
	public class RtfTableGrid : List<int> {
		public int First { get { return Count > 0 ? this[0] : -1; } }
		public int Last { get { return Count > 0 ? this[Count - 1] : -1; } }
		public int BinarySearchRight(int right) {
			int index = BinarySearch(right);
			if (index <= 0)
				return index;
			int nextIndex = index + 1;
			while (nextIndex < Count && this[nextIndex] == this[index]) {
				index = nextIndex;
				nextIndex++;
			}
			return index;
		}
		public int BinarySearchLeft(int right) {
			int index = BinarySearch(right);
			if (index <= 0)
				return index;
			int prevIndex = index - 1;
			while (prevIndex >= 0 && this[prevIndex] == this[index]) {
				index = prevIndex;
				prevIndex--;
			}
			return index;
		}
	}
	#endregion
	#region RtfTableColumnsCalculator
	public class RtfTableColumnsCalculator {
		public RtfTableGrid Calculate(RtfTable table, int tableIndent) {
			Guard.ArgumentNotNull(table, "table");
			int rowsCount = table.Rows.Count;
			if (rowsCount == 0)
				Exceptions.ThrowArgumentException("table.Rows.Count", rowsCount);
			RtfTableGrid result = new RtfTableGrid();
			result.Add(tableIndent);
			for (int i = 0; i < rowsCount; i++) {
				RtfTableGrid rowColumns = GetRowColumns(table.Rows[i]);
				result = Merge(rowColumns, result);
			}
			return result;
		}
		protected internal virtual RtfTableGrid Merge(RtfTableGrid source, RtfTableGrid destination) {
			RtfTableGrid result = new RtfTableGrid();
			int sourceIndex = 0;
			int destinationIndex = 0;
			while (sourceIndex < source.Count && destinationIndex < destination.Count) {
				if (source[sourceIndex] <= destination[destinationIndex]) {
					result.Add(source[sourceIndex]);
					if (source[sourceIndex] == destination[destinationIndex])
						destinationIndex++;
					sourceIndex++;
				}
				else {
					result.Add(destination[destinationIndex]);
					destinationIndex++;
				}
			}
			if (destinationIndex < destination.Count) {
				for (; destinationIndex < destination.Count; destinationIndex++)
					result.Add(destination[destinationIndex]);
			}
			else if (sourceIndex < source.Count) {
				for (; sourceIndex < source.Count; sourceIndex++)
					result.Add(source[sourceIndex]);
			}
			return result;
		}
		protected internal virtual RtfTableGrid GetRowColumns(RtfTableRow row) {
			RtfTableGrid result = new RtfTableGrid();
			int left = row.Left;
			result.Add(left);
			int cellsCount = row.Cells.Count;
			if (cellsCount == 0)
				Exceptions.ThrowArgumentException("row.Cells.Count", cellsCount);
			for (int index = 0; index < cellsCount; index++) {
				int right = row.Cells[index].Right;
				if (right <= left)
					result.Add(left);
				else {
					result.Add(right);
					left = right;
				}
			}
			return result;
		}
	}
	#endregion
}
