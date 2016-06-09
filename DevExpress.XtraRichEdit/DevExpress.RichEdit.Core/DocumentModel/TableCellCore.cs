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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	public delegate void TableCellProcessorDelegate(TableCell cell);
	public delegate void TableRowProcessorDelegate(TableRow cell);
	public enum ConditionalRowType {
		Unknown = 0,
		FirstRow,
		LastRow,
		EvenRowBand,
		OddRowBand,
		Normal,
	}
	public enum ConditionalColumnType {
		Unknown = 0,
		FirstColumn,
		LastColumn,
		EvenColumnBand,
		OddColumnBand,
		Normal,
	}
	public class TableLayoutInfo {
		TableGrid tableGrid;
		int maxTableWidth;
		bool allowTablesToExtendIntoMargins;
		bool simpleView;
		int percentBaseWidth;
		public TableLayoutInfo(TableGrid tableGrid, int maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, int percentBaseWidth) {
			Guard.ArgumentNotNull(tableGrid, "tableGrid");
			this.tableGrid = tableGrid;
			this.maxTableWidth = maxTableWidth;
			this.allowTablesToExtendIntoMargins = allowTablesToExtendIntoMargins;
			this.simpleView = simpleView;
			this.percentBaseWidth = percentBaseWidth;
		}
		public TableGrid TableGrid { get { return tableGrid; } }
		public bool CanUseTableGrid(int maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, int percentBaseWidth) {
			return this.maxTableWidth == maxTableWidth && this.allowTablesToExtendIntoMargins == allowTablesToExtendIntoMargins && this.simpleView == simpleView && this.percentBaseWidth == percentBaseWidth;
		}
	}
	#region Table
	public class Table {
		#region Fields
		int index = -1;
		int styleIndex = 0;
		TableCell parentCell;
		int nestedLevel;
		TableLayoutInfo cachedTableLayoutInfo;
		readonly TableProperties properties;
		readonly TableRowCollection rows;
		readonly PieceTable pieceTable;
		#endregion
		public Table(PieceTable pieceTable)
			: this(pieceTable, null, 1, 1) {
		}
		public Table(PieceTable pieceTable, TableCell parentCell, int rowCount, int cellCount) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.parentCell = parentCell;
			this.pieceTable = pieceTable;
			this.nestedLevel = parentCell != null ? parentCell.Table.NestedLevel + 1 : 0;
			this.properties = new TableProperties(PieceTable);
			this.rows = new TableRowCollection(this, rowCount, cellCount);
			SubscribeTablePropertiesEvents();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public TableProperties TableProperties { get { return properties; } }
		public TableCell ParentCell { get { return parentCell; } }
		public TableLayoutInfo CachedTableLayoutInfo { get { return cachedTableLayoutInfo; } set { cachedTableLayoutInfo = value; } }
		public TableRowCollection Rows {
			[DebuggerStepThrough]
			get { return rows; }
		}
		public TableRow FirstRow { get { return Rows.First; } }
		public TableRow LastRow { get { return Rows.Last; } }
		public TableStyle TableStyle { get { return DocumentModel.TableStyles[StyleIndex]; } }
		internal int NestedLevel { get { return nestedLevel; } }
		internal int Index {
			get { return index; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Index", value);
				index = value;
			}
		}
		public int StyleIndex {
			get { return styleIndex; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TableStyleIndex", value);
				if (styleIndex == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					ChangeTableStyleIndexHistoryItem item = new ChangeTableStyleIndexHistoryItem(PieceTable, this.Index, styleIndex, value);
					DocumentModel.History.Add(item);
					item.Execute();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		internal bool SuppressIntegrityCheck { get { return false; } set { } }
		#endregion
		#region Table properties
		public MarginUnitBase LeftMargin { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseLeftMargin).CellMargins.Left; } }
		public MarginUnitBase RightMargin { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseRightMargin).CellMargins.Right; } }
		public MarginUnitBase TopMargin { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTopMargin).CellMargins.Top; } }
		public MarginUnitBase BottomMargin { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseBottomMargin).CellMargins.Bottom; } }
		public CellSpacing CellSpacing { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseCellSpacing).CellSpacing; } }
		public TableIndent TableIndent { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableIndent).TableIndent; } }
		public PreferredWidth PreferredWidth { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UsePreferredWidth).PreferredWidth; } }
		public TableLayoutType TableLayout { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableLayout).TableLayout; } set { TableProperties.TableLayout = value; } }
		public TableRowAlignment TableAlignment { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableAlignment).TableAlignment; } set { TableProperties.TableAlignment = value; } }
		public TableLookTypes TableLook { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableLook).TableLook; } set { TableProperties.TableLook = value; } }
		public int TableStyleColBandSize { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableStyleColBandSize).TableStyleColBandSize; } set { TableProperties.TableStyleColBandSize = value; } }
		public int TableStyleRowBandSize { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTableStyleRowBandSize).TableStyleRowBandSize; } set { TableProperties.TableStyleRowBandSize = value; } }
		public bool IsTableOverlap { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseIsTableOverlap).IsTableOverlap; } set { TableProperties.IsTableOverlap = value; } }
		public Color BackgroundColor { get { return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseBackgroundColor).BackgroundColor; } set { TableProperties.BackgroundColor = value; } }
		public ParagraphIndex StartParagraphIndex { get { return FirstRow.FirstCell.StartParagraphIndex; } }
		public ParagraphIndex EndParagraphIndex { get { return LastRow.LastCell.EndParagraphIndex; } }
		#endregion
		public void ResetCachedLayoutInfo() {
			this.cachedTableLayoutInfo = null;
		}
		protected internal void SetParentCell(TableCell newParentCell) {
			this.parentCell = newParentCell;
			RecalNestedLevel();
		}
		public void RecalNestedLevel() {
			if (parentCell != null)
				nestedLevel = parentCell.Table.NestedLevel + 1;
			else
				nestedLevel = 0;
		}
		public TableProperties GetTablePropertiesSource(TablePropertiesOptions.Mask mask) {
			return GetTablePropertiesSource(mask, ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public TableProperties GetTablePropertiesSourceForCell(TablePropertiesOptions.Mask mask, TableCell tableCell) {
			bool insideBorder;
			return GetTablePropertiesSourceForCell(mask, tableCell, true, out insideBorder);
		}
		public TableProperties GetTablePropertiesSourceForCell(TablePropertiesOptions.Mask mask, TableCell tableCell, bool isBorderCell, out bool insideBorder) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return GetTablePropertiesSource(mask, rowIndex, cellIndex, isBorderCell, out insideBorder);
		}
		public TableProperties GetTablePropertiesSource(TablePropertiesOptions.Mask mask, int rowIndex, int columnIndex) {
			bool insideBorder;
			return GetTablePropertiesSource(mask, rowIndex, columnIndex, true, out insideBorder);
		}
		public TableProperties GetTablePropertiesSource(TablePropertiesOptions.Mask mask, int rowIndex, int columnIndex, bool isBorderCell, out bool insideBorder) {
			return GetTablePropertiesSource(mask, GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, columnIndex), isBorderCell, out insideBorder);
		}
		public TableProperties GetTablePropertiesSource(TablePropertiesOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType) {
			bool insideBorder;
			return GetTablePropertiesSource(mask, rowType, columnType, true, out insideBorder);
		}
		public virtual TableProperties GetTablePropertiesSource(TablePropertiesOptions.Mask mask, ConditionalRowType rowType, ConditionalColumnType columnType, bool isBorderCell, out bool insideBorder) {
			if(TableProperties.GetUse(OuterOrInside(mask, isBorderCell))) {
				insideBorder = !isBorderCell;
				return TableProperties;
			}
			TableProperties source = TableStyle.GetTableProperties(mask, rowType, columnType, isBorderCell, out insideBorder);
			if (source != null)
				return source;
			insideBorder = !isBorderCell;
			return DocumentModel.DefaultTableProperties;
		}
		public static TablePropertiesOptions.Mask OuterOrInside(TablePropertiesOptions.Mask mask, bool isBorderCell) {
			if(isBorderCell)
				return mask;
			if((mask & (TablePropertiesOptions.Mask.UseLeftBorder | TablePropertiesOptions.Mask.UseRightBorder)) != 0)
				mask |= TablePropertiesOptions.Mask.UseInsideVerticalBorder;
			if((mask & (TablePropertiesOptions.Mask.UseTopBorder | TablePropertiesOptions.Mask.UseBottomBorder)) != 0)
				mask |= TablePropertiesOptions.Mask.UseInsideHorizontalBorder;
			mask &= ~(TablePropertiesOptions.Mask.UseLeftBorder | TablePropertiesOptions.Mask.UseRightBorder | TablePropertiesOptions.Mask.UseTopBorder | TablePropertiesOptions.Mask.UseBottomBorder);
			return mask;
		}
		internal ConditionalColumnType GetColumnTypeByIndex(int rowIndex, int columnIndex) {
			TableCell cell = rows[rowIndex].Cells[columnIndex];
			ConditionalColumnType result = cell.ConditionalType;
			if (result == ConditionalColumnType.Unknown) {
				result = GetColumnTypeByIndexCore(rowIndex, columnIndex);
				cell.ConditionalType = result;
			}
			return result;
		}
		private bool HasColumnStyle(TableStyle style, ConditionalColumnType type) {
			if (style == null)
				return false;
			bool hasColumnStyle = style.HasColumnStyle(type);
			if (!hasColumnStyle)
				hasColumnStyle = HasColumnStyle(style.Parent, type);
			return hasColumnStyle;
		}
		ConditionalColumnType GetColumnTypeByIndexCore(int rowIndex, int columnIndex) {
			bool hasFirstColumnStyle = false;
			if ((TableLook & TableLookTypes.ApplyFirstColumn) != 0) {
				if (HasColumnStyle(TableStyle, ConditionalColumnType.FirstColumn)) {
					hasFirstColumnStyle = true;
					if (columnIndex == 0)
						return ConditionalColumnType.FirstColumn;
				}
			}
			if ((TableLook & TableLookTypes.ApplyLastColumn) != 0) {
				if (columnIndex == Rows[rowIndex].Cells.Count - 1 && HasColumnStyle(TableStyle, ConditionalColumnType.LastColumn))
					return ConditionalColumnType.LastColumn;
			}
			if ((TableLook & TableLookTypes.DoNotApplyColumnBanding) == 0) {
				if (hasFirstColumnStyle)
					columnIndex--;
				if ((columnIndex / TableStyleColBandSize) % 2 == 0)
					return ConditionalColumnType.OddColumnBand;
				else
					return ConditionalColumnType.EvenColumnBand;
			}
			else
				return ConditionalColumnType.Normal;
		}
		internal ConditionalRowType GetRowTypeByIndex(int rowIndex) {
			TableRow row = rows[rowIndex];
			ConditionalRowType result = row.ConditionalType;
			if (result == ConditionalRowType.Unknown) {
				result = GetRowTypeByIndexCore(rowIndex);
				row.ConditionalType = result;
			}
			return result;
		}
		private bool HasRowStyle(TableStyle style, ConditionalRowType type) {
			if (style == null)
				return false;
			bool hasRowStyle = style.HasRowStyle(type);
			if (!hasRowStyle)
				hasRowStyle = HasRowStyle(style.Parent, type);
			return hasRowStyle;
		}
		ConditionalRowType GetRowTypeByIndexCore(int rowIndex) {
			bool hasFirstRowStyle = false;
			if ((TableLook & TableLookTypes.ApplyFirstRow) != 0) {
				if (HasRowStyle(TableStyle, ConditionalRowType.FirstRow)) {
					hasFirstRowStyle = true;
					if (rowIndex == 0)
						return ConditionalRowType.FirstRow;
				}
			}
			if ((TableLook & TableLookTypes.ApplyLastRow) != 0) {
				if (rowIndex == Rows.Count - 1 && HasRowStyle(TableStyle, ConditionalRowType.LastRow))
					return ConditionalRowType.LastRow;
			}
			if ((TableLook & TableLookTypes.DoNotApplyRowBanding) == 0) {
				if (hasFirstRowStyle) {
					rowIndex--;
				}
				if ((rowIndex / TableStyleRowBandSize) % 2 == 0)
					return ConditionalRowType.OddRowBand;
				else
					return ConditionalRowType.EvenRowBand;
			}
			else
				return ConditionalRowType.Normal;
		}
		public LeftBorder GetActualLeftBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseLeftBorder).Borders.LeftBorder;
		}
		public RightBorder GetActualRightBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseRightBorder).Borders.RightBorder;
		}
		public TopBorder GetActualTopBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseTopBorder).Borders.TopBorder;
		}
		public BottomBorder GetActualBottomBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseBottomBorder).Borders.BottomBorder;
		}
		public InsideHorizontalBorder GetActualInsideHorizontalBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseInsideHorizontalBorder).Borders.InsideHorizontalBorder;
		}
		public InsideVerticalBorder GetActualInsideVerticalBorder() {
			return GetTablePropertiesSource(TablePropertiesOptions.Mask.UseInsideVerticalBorder).Borders.InsideVerticalBorder;
		}
		protected internal virtual MergedTableProperties GetParentMergedTableProperties() {
			TablePropertiesMerger merger = new TablePropertiesMerger(TableStyle.GetMergedTableProperties());
			merger.Merge(DocumentModel.DefaultTableProperties);
			return merger.MergedProperties;
		}
		protected internal virtual MergedTableProperties GetMergedWithStyleTableProperties() {
			TablePropertiesMerger merger = new TablePropertiesMerger(TableProperties);
			merger.Merge(TableStyle.TableProperties);
			return merger.MergedProperties;
		}
		internal MergedCharacterProperties GetMergedCharacterProperties(TableCell cell) {
			int rowIndex = cell.RowIndex;
			int cellIndex = cell.IndexInRow;
			return TableStyle.GetMergedCharacterProperties(GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		internal MergedCharacterProperties GetMergedCharacterProperties(TableCell cell, TableStyle tableStyle) {
			return tableStyle.GetMergedCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public void SetTableStyleIndexCore(int newStyleIndex) {
			this.styleIndex = newStyleIndex;
			if (FirstRow == null || FirstRow.FirstCell == null || LastRow == null || LastRow.LastCell == null
				|| FirstRow.FirstCell.StartParagraphIndex < ParagraphIndex.Zero)
				return;
			for (ParagraphIndex i = FirstRow.FirstCell.StartParagraphIndex; i <= LastRow.LastCell.EndParagraphIndex; i++)
				pieceTable.Paragraphs[i].ResetCachedIndices(ResetFormattingCacheType.All);
			new TableConditionalFormattingController(this).ResetCachedProperties(0);
			if (StartParagraphIndex >= ParagraphIndex.Zero) {
				RunIndex start = PieceTable.Paragraphs[FirstRow.FirstCell.StartParagraphIndex].FirstRunIndex;
				RunIndex end = PieceTable.Paragraphs[LastRow.LastCell.EndParagraphIndex].LastRunIndex;
				PieceTable.ApplyChangesCore(TableChangeActionCalculator.CalculateChangeActions(TableChangeType.TableStyle), start, end);
			}
		}
		internal TableCell GetCell(TableRow row, int columnIndex) {
			if (row == null)
				return null;
			int index = GetAbsoluteCellIndexInRow(row, columnIndex, false);
			if (row.Cells.Count <= index)
				return GetCell(row.Previous, columnIndex);
			return row.Cells[index];
		}
		internal TableCell GetCell(int rowIndex, int columnIndex) {
			if (Rows.Count <= rowIndex)
				Exceptions.ThrowInternalException();
			TableRow row = Rows[rowIndex];
			return GetCell(row, columnIndex);
		}
		internal int GetAbsoluteCellIndexInRow(TableRow row, int columnIndex, bool layoutIndex) {
			if (row.Cells.Count == 0)
				Exceptions.ThrowInternalException();
			TableCellCollection cells = row.Cells;
			int cellsCount = cells.Count;
			int cellIndex = 0;
			columnIndex -= layoutIndex ? row.LayoutProperties.GridBefore : row.GridBefore;
			while (columnIndex > 0 && cellIndex < cellsCount) {
				TableCell currentCell = cells[cellIndex];
				columnIndex -= layoutIndex ? currentCell.LayoutProperties.ColumnSpan : currentCell.ColumnSpan;
				if (columnIndex >= 0)
					cellIndex++;
			}
			return cellIndex;
		}
		internal int GetCellColumnIndexConsiderRowGrid(TableCell cell) {
			return cell.GetStartColumnIndexConsiderRowGrid();
		}
		internal int GetStartCellColumnIndexConsiderRowGrid(TableRow row) {
			return row.GridBefore;
		}
		internal int GetLastCellColumnIndexConsiderRowGrid(TableRow row, bool layoutIndex) {
			TableCellCollection cells = row.Cells;
			int result = 0;
			int cellsCount = cells.Count;
			result += layoutIndex ? row.LayoutProperties.GridBefore : row.GridBefore;
			for (int index = 0; index < cellsCount; index++) {
				TableCell cell = cells[index];
				result += layoutIndex ? cell.LayoutProperties.ColumnSpan : cell.ColumnSpan;
			}
			return result;
		}
		public int GetTotalCellsInRowConsiderGrid(TableRow row) {
			int cells = row.GridBefore;
			for (int index = 0; index < row.Cells.Count; index++) {
				cells += row.Cells[index].ColumnSpan;
			}
			cells += row.GridAfter;
			return cells;
		}
		internal int FindTotalColumnsCountInTable() {
			int result = 0;
			for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++) {
				TableRow row = Rows[rowIndex];
				int cells = GetTotalCellsInRowConsiderGrid(row);
				result = Math.Max(result, cells);
			}
			return result;
		}
		protected internal virtual void CopyProperties(Table sourceTable) {
			this.TableProperties.CopyFrom(sourceTable.TableProperties);
			this.StyleIndex = sourceTable.TableStyle.Copy(DocumentModel);
			this.TableLayout = sourceTable.TableLayout;
		}
		public void NormalizeRows() {
			bool oldValue = DocumentModel.ForceNotifyStructureChanged;
			DocumentModel.ForceNotifyStructureChanged = true;
			int count = rows.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (AllCellsHasVerticalMerge(rows[i])) {
					if (rows[i].Height.Type != HeightUnitType.Auto && i > 0)
						rows[i - 1].Height.Value += rows[i].Height.Value;
					if (PieceTable.ShouldForceUpdateIntervals())
						PieceTable.UpdateIntervals();
					PieceTable.DeleteTableRowWithContent(rows[i]);
				}
			}
			DocumentModel.ForceNotifyStructureChanged = oldValue;
		}
		private bool AllCellsHasVerticalMerge(TableRow tableRow) {
			TableCellCollection cells = tableRow.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				if (cells[i].VerticalMerging != MergingState.Continue)
					return false;
			}
			return true;
		}
		public void NormalizeCellColumnSpans() {
			NormalizeCellColumnSpans(true);
		}
		public void NormalizeCellColumnSpans(bool canNormalizeWidthBeforeAndWidthAfter) {
			if (Rows.Count == 0)
				return;
			NormalizeRowsGridBefore(Rows, canNormalizeWidthBeforeAndWidthAfter);
			List<TableGridInterval> result = GetGridIntervals();
			int rowCount = rows.Count;
			for (int i = rowCount - 1; i >= 0; i--)
				NormalizeTableRow(rows[i], result, i);
		}
		void NormalizeRowsGridBefore(TableRowCollection rows, bool canNormalizeWidthBeforeAndWidthAfter) {
			int count = rows.Count;
			int minGridBefore = rows[0].GridBefore;
			int minGridAfter = rows[0].GridAfter;
			for (int i = 1; i < count; i++) {
				TableRow row = rows[i];
				minGridBefore = Math.Min(minGridBefore, row.GridBefore);
				minGridAfter = Math.Min(minGridAfter, row.GridAfter);
			}
			if (minGridBefore == 0 && minGridAfter == 0)
				return;
			for (int i = 0; i < count; i++)
				NormalizeRowGridBefore(rows[i], canNormalizeWidthBeforeAndWidthAfter, minGridBefore, minGridAfter);
		}
		void NormalizeRowGridBefore(TableRow row, bool canNormalizeWidthBeforeAndWidthAfter, int minGridBefore, int minGridAfter) {
			if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode) {
				row.UnsubscribeRowPropertiesEvents();
				try {
					NormalizeRowGridBeforeCore(row, canNormalizeWidthBeforeAndWidthAfter, minGridBefore, minGridAfter);
				}
				finally {
					row.SubscribeRowPropertiesEvents();
				}
			}
			else
				NormalizeRowGridBeforeCore(row, canNormalizeWidthBeforeAndWidthAfter, minGridBefore, minGridAfter);
		}
		void NormalizeRowGridBeforeCore(TableRow row, bool canNormalizeWidthBeforeAndWidthAfter, int minGridBefore, int minGridAfter) {
			if (minGridBefore != 0)
				row.GridBefore -= minGridBefore;
			if (row.GridBefore == 0 && (row.WidthBefore.Type != WidthUnitType.Nil || row.WidthBefore.Value != 0) && canNormalizeWidthBeforeAndWidthAfter) {
				row.Properties.WidthBefore.Type = WidthUnitType.Nil;
				row.Properties.WidthBefore.Value = 0;
			}
			if (minGridAfter != 0)
				row.GridAfter -= minGridAfter;
			if (row.GridAfter == 0 && (row.WidthAfter.Type != WidthUnitType.Nil || row.WidthAfter.Value != 0) && canNormalizeWidthBeforeAndWidthAfter) {
				row.Properties.WidthAfter.Type = WidthUnitType.Nil;
				row.Properties.WidthAfter.Value = 0;
			}
		}
		void NormalizeTableRow(TableRow row, List<TableGridInterval> gridIntervals, int rowIndex) {
			if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode) {
				row.UnsubscribeRowPropertiesEvents();
				try {
					NormalizeTableRowCore(row, gridIntervals, rowIndex);
				}
				finally {
					row.SubscribeRowPropertiesEvents();
				}
			}
			else
				NormalizeTableRowCore(row, gridIntervals, rowIndex);
		}
		void NormalizeTableRowCore(TableRow row, List<TableGridInterval> gridIntervals, int rowIndex) {
			TableCellCollection cells = row.Cells;
			int cellCount = cells.Count;
			TableGridIntervalIterator intervalIterator = new TableGridIntervalIterator(gridIntervals);
			int span = CalculateNewSpan(row.GridBefore, intervalIterator);
			if (row.GridBefore != span)
				row.GridBefore = span;
			bool shouldDeleteRow = true;
			for (int i = 0; i < cellCount; i++) {
				TableCell cell = cells[i];
				span = CalculateNewSpan(cell.ColumnSpan, intervalIterator);
				if (cell.ColumnSpan != span)
					cell.ColumnSpan = span;
				shouldDeleteRow &= cell.VerticalMerging == MergingState.Continue;
			}
			span = CalculateNewSpan(row.GridAfter, intervalIterator);
			if (row.GridAfter != span)
				row.GridAfter = span;
		}
		int CalculateNewSpan(int oldSpan, TableGridIntervalIterator intervalIterator) {
			int result = 0;
			int totalSum = 0;
			while (totalSum < oldSpan) {
				totalSum += intervalIterator.CurrentInterval.ColumnSpan;
				result++;
				intervalIterator.MoveNextInterval();
			}
			Debug.Assert(totalSum == oldSpan);
			return result;
		}
		protected internal virtual void NormalizeTableCellVerticalMerging() {
			int rowsCout = Rows.Count;
			TableCellCollection cellsInFirstRow = FirstRow.Cells;
			int cellsInFirstRowCount = cellsInFirstRow.Count;
			for (int i = 0; i < cellsInFirstRowCount; i++) {
				TableCell currentCell = cellsInFirstRow[i];
				if (currentCell.VerticalMerging == MergingState.Continue) {
					if (rowsCout == 1) {
						currentCell.Properties.VerticalMerging = MergingState.None;
						continue;
					}
					int columnIndex = currentCell.GetStartColumnIndexConsiderRowGrid();
					TableCell nextRowCell = GetCell(FirstRow.Next, columnIndex);
					if (nextRowCell.VerticalMerging == MergingState.Continue)
						currentCell.Properties.VerticalMerging = MergingState.Restart;
					else
						currentCell.Properties.VerticalMerging = MergingState.None;
				}
				else if (currentCell.VerticalMerging == MergingState.Restart) {
					if (rowsCout == 1)
						currentCell.Properties.VerticalMerging = MergingState.None;
				}
			}
			if (rowsCout == 1)
				return;
			TableCellCollection cellsInLastRow = LastRow.Cells;
			int cellsInLastRowCount = cellsInLastRow.Count;
			for (int i = 0; i < cellsInLastRowCount; i++) {
				TableCell currentCell = cellsInLastRow[i];
				if (currentCell.VerticalMerging == MergingState.Restart)
					currentCell.Properties.VerticalMerging = MergingState.None;
			}
		}
		protected internal virtual void SubscribeTablePropertiesEvents() {
			TableProperties.ObtainAffectedRange += new ObtainAffectedRangeEventHandler(OnTablePropertiesObtainAffectedRange);
		}
		protected internal virtual void OnPropertiesObtainAffectedRangeCore(ObtainAffectedRangeEventArgs e) {
			OnPropertiesObtainAffectedRangeCore(e, 0);
		}
		protected internal virtual void OnPropertiesObtainAffectedRangeCore(ObtainAffectedRangeEventArgs e, int firstRowIndex) {
			new TableConditionalFormattingController(this).ResetCachedProperties(firstRowIndex);
			ParagraphCollection paragraphs = this.PieceTable.Paragraphs;
			TableRow firstRow = Rows.First;
			TableRow lastRow = Rows.Last;
			if (firstRow == null || lastRow == null)
				return;
			TableCell firstCell = firstRow.FirstCell;
			TableCell lastCell = lastRow.LastCell;
			if (firstCell == null || lastCell == null)
				return;
			ParagraphIndex startParagraphIndex = firstCell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = lastCell.EndParagraphIndex;
			ParagraphIndex paragraphCount = new ParagraphIndex(paragraphs.Count);
			if (startParagraphIndex < ParagraphIndex.Zero || startParagraphIndex >= paragraphCount ||
				endParagraphIndex < ParagraphIndex.Zero || endParagraphIndex >= paragraphCount)
				return;
			e.Start = paragraphs[firstCell.StartParagraphIndex].FirstRunIndex;
			e.End = paragraphs[lastCell.EndParagraphIndex].LastRunIndex;
		}
		protected internal virtual void OnTablePropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			OnPropertiesObtainAffectedRangeCore(e);
		}
		protected internal virtual List<TableGridInterval> GetGridIntervals() {
			SimpleTableWidthsCalculator widthsCalculator = new SimpleTableWidthsCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			TableGridCalculator calculator = new TableGridCalculator(DocumentModel, widthsCalculator, Int32.MaxValue);
			return calculator.CalculateGridIntervals(this);
		}
		protected internal void NormalizeTableGrid() {
			int maxEndColumnIndex = -1;
			int rowsCount = Rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = Rows[i];
				int currentEndColumnIndex = GetEndColumnIndex(currentRow.LastCell) + currentRow.GridAfter;
				maxEndColumnIndex = Algorithms.Max(maxEndColumnIndex, currentEndColumnIndex);
			}
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = Rows[i];
				int currentEndColumnIndex = GetEndColumnIndex(currentRow.LastCell) + currentRow.GridAfter;
				int gridAfterDelta = maxEndColumnIndex - currentEndColumnIndex;
				if (gridAfterDelta != 0)
					currentRow.GridAfter += gridAfterDelta;
			}
		}
		protected internal int GetEndColumnIndex(TableCell lastCell) {
			return lastCell.GetEndColumnIndexConsiderRowGrid();
		}
		protected internal virtual TableCell GetFirstCellInVerticalMergingGroup(TableCell tableCell) {
			if (tableCell.VerticalMerging == MergingState.None || tableCell.VerticalMerging == MergingState.Restart)
				return tableCell;
			TableRowCollection rows = tableCell.Table.Rows;
			Debug.Assert(rows.Count > 1, "cell.VerticalMerging == MergingState.Continue & cell.Table.Rows.Count is 1", "");
			int rowIndex = rows.IndexOf(tableCell.Row);
			int cellColumnIndex = GetCellColumnIndexConsiderRowGrid(tableCell);
			for (rowIndex--; rowIndex >= 0; rowIndex--) {
				TableRow row = rows[rowIndex];
				TableCellCollection cells = row.Cells;
				int columnIndex = row.GridBefore;
				int cellCount = cells.Count;
				for (int i = 0; i < cellCount; i++) {
					TableCell cell = cells[i];
					if (cellColumnIndex <= columnIndex) {
						if (cell.VerticalMerging != MergingState.Continue || rowIndex == 0)
							return cell;
						else
							break;
					}
					columnIndex += cell.ColumnSpan;
				}
			}
			return null;
		}
		protected internal virtual void RemoveInvalidVerticalSpans(bool fixLastRow) {
			TableRowCollection rows = this.Rows;
			int rowCount = rows.Count;
			int count = fixLastRow ? rowCount : rowCount - 1; 
			if (rowCount == 1) {
				RemoveInvalidVarticalSpansFromTableWithOneRow(rows);
				return;
			}
			for (int rowIndex = 0; rowIndex < count; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					MergingState verticalMerging = cell.VerticalMerging;
					if (verticalMerging == MergingState.None)
						continue;
					int columnIndex = GetCellColumnIndexConsiderRowGrid(cell);
					if (verticalMerging == MergingState.Restart) {
						if (rowIndex == rowCount - 1)
							cell.VerticalMerging = MergingState.None;
						TableCell lowerCell = GetCell(rowIndex + 1, columnIndex);
						if (lowerCell.VerticalMerging != MergingState.Continue)
							cell.VerticalMerging = MergingState.None;
					}
					else if (cell.VerticalMerging == MergingState.Continue) {
						if (rowIndex == 0) {
							cell.VerticalMerging = MergingState.None;
							continue;
						}
						TableCell upperCell = GetCell(rowIndex - 1, columnIndex);
						if (upperCell == null || upperCell.VerticalMerging == MergingState.None) { 
							if (rowIndex == rowCount - 1)
								cell.VerticalMerging = MergingState.None;
							else {
								TableCell lowerCell = GetCell(rowIndex + 1, columnIndex);
								if (lowerCell.VerticalMerging == MergingState.Continue)
									cell.VerticalMerging = MergingState.Restart;
								else
									cell.VerticalMerging = MergingState.None;
							}
						}
					}
				}
			}
		}
		void RemoveInvalidVarticalSpansFromTableWithOneRow(TableRowCollection rows) {
			TableCellCollection cells = rows[0].Cells;
			int cellCount = cells.Count;
			for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
				TableCell cell = cells[cellIndex];
				MergingState verticalMerging = cell.VerticalMerging;
				if (verticalMerging == MergingState.Continue) {
					cell.VerticalMerging = MergingState.None;
				}
			}
		}
		protected internal virtual void RemoveVerticalSpanFromLastRowCells() {
			if (Rows.Count == 0)
				return;
			int cellsCount = LastRow.Cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCell cell = LastRow.Cells[i];
				if (cell.VerticalMerging == MergingState.Restart)
					cell.VerticalMerging = MergingState.None;
			}
		}
		protected internal virtual void Normalize() {
			Normalize(false);
		}
		protected internal virtual void Normalize(bool fixLastRow) {
			RemoveInvalidVerticalSpans(fixLastRow);
			RemoveVerticalSpanFromLastRowCells();
		}
		public void ForEachRow(TableRowProcessorDelegate rowProcessor) {
			TableRowCollection rows = Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++)
				rowProcessor(rows[i]);
		}
		public void ForEachCell(TableCellProcessorDelegate cellProcessor) {
			TableRowCollection rows = Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableCellCollection cells = rows[i].Cells;
				int cellCount = cells.Count; ;
				for (int j = 0; j < cellCount; j++)
					cellProcessor(cells[j]);
			}
		}
		public void InitializeColumnWidths(DevExpress.XtraRichEdit.Model.TableAutoFitBehaviorType autoFitBehavior, int fixedColumnWidths, int outerColumnWidth, bool matchHorizontalTableIndentsToTextEdge) {
			if (autoFitBehavior == TableAutoFitBehaviorType.AutoFitToContents) {
				ForEachCell(delegate(TableCell cell) {
					cell.PreferredWidth.Type = WidthUnitType.Auto;
					cell.PreferredWidth.Value = 0;
				});
				return;
			}
			if (autoFitBehavior == TableAutoFitBehaviorType.AutoFitToWindow) {
				DistributeHundredPercentToAllColumns();
				return;
			}
			if (autoFitBehavior == TableAutoFitBehaviorType.FixedColumnWidth) {
				TableProperties.PreferredWidth.Value = 0;
				TableProperties.PreferredWidth.Type = WidthUnitType.Auto;
				if (IsCellWidthDefined(fixedColumnWidths)) {
					SetAllColumnsWidthsToFixedColumnWidths(fixedColumnWidths);
				}
				else {
					int tableWidth = GetTableWidth(this, outerColumnWidth, matchHorizontalTableIndentsToTextEdge);
					if (matchHorizontalTableIndentsToTextEdge)
						tableWidth = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(tableWidth);
					int totalColumnsInTable = FindTotalColumnsCountInTable();
					int[] widths = DistributeWidthsToAllColumns(tableWidth, totalColumnsInTable);
					for (int i = 0; i < Rows.Count; i++) {
						Debug.Assert(totalColumnsInTable == rows[i].Cells.Count);
						for (int j = 0; j < Rows[i].Cells.Count; j++) {
							int cellWidth = widths[j];
							if(matchHorizontalTableIndentsToTextEdge)
								cellWidth = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(cellWidth);
							ApplyWidthModelUnits(Rows[i].Cells[j], cellWidth);
						}
					}
				}				
			}
		}
		int[] DistributeWidthsToAllColumns(int width, int count) {
			int[] result = new int[count];
			if (count == 0)
				return result;
			int rest = width;
			for (int i = 0; i < count; i++) {
				int cellWidth = rest / (count - i);
				result[i] = Math.Max(cellWidth, 1);				
				rest -= cellWidth;
			}
			return result;
		}
		bool IsCellWidthDefined(int fixedColumnWidths) {
			return fixedColumnWidths != Int32.MinValue && fixedColumnWidths != Int32.MaxValue && fixedColumnWidths != 0;
		}
		protected internal virtual void SetAllColumnsWidthsToFixedColumnWidths(int fixedColumnWidths) {
			TableCellProcessorDelegate applyWidthPercentBased = delegate(TableCell cell) {
				ApplyWidthModelUnits(cell, fixedColumnWidths);
			};
			ForEachCell(applyWidthPercentBased);
		}
		protected internal virtual void DistributeHundredPercentToAllColumns() {
			TableProperties.PreferredWidth.Value = 5000;
			TableProperties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
			int columnsCount = FindTotalColumnsCountInTable();
			Guard.ArgumentPositive(columnsCount, "Invalid Table");
			int columnWidthInFiftiethsOfPercent = Math.Max(5000 / columnsCount, 1);
			int lastColumnCorrection = 5000 % columnsCount;
			TableCellProcessorDelegate applyWidthPercentBased = delegate(TableCell cell) {
				ApplyWidthPercentBased(cell, columnWidthInFiftiethsOfPercent, lastColumnCorrection);
			};
			ForEachCell(applyWidthPercentBased);
		}
		void ApplyWidthPercentBased(TableCell cell, int columnWidthInFiftiethsOfPercent, int lastColumnCorrection) {
			cell.Properties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
			cell.Properties.PreferredWidth.Value = columnWidthInFiftiethsOfPercent;
			if (cell.IsLastCellInRow)
				cell.Properties.PreferredWidth.Value += lastColumnCorrection;
		}
		void ApplyWidthModelUnits(TableCell cell, int fixedColumnWidths) {
			cell.Properties.PreferredWidth.Value = fixedColumnWidths;
			cell.Properties.PreferredWidth.Type = WidthUnitType.ModelUnits;
		}
		int GetTableWidth(Table table, int columnWidth, bool matchHorizontalTableIndentsToTextEdge) {
			MarginUnitBase leftMargin = table.Rows.First.Cells.First.GetActualLeftMargin();
			MarginUnitBase rightMargin = table.Rows.First.Cells.Last.GetActualRightMargin();
			int leftMarginValue = GetLayoutMarginValue(leftMargin);
			int rightMarginValue = GetLayoutMarginValue(rightMargin);
			int tableWidth = columnWidth;
			if (!matchHorizontalTableIndentsToTextEdge)
				tableWidth += leftMarginValue + rightMarginValue;
			return tableWidth;
		}
		protected internal int GetLayoutMarginValue(MarginUnitBase margin) {
			return margin.Type == WidthUnitType.ModelUnits ? margin.Value : 0;
		}
		public bool ContainsTable(Table table) {
			Guard.ArgumentNotNull(table, "table");
			if (table.NestedLevel <= NestedLevel)
				return false;
			while (table.NestedLevel > NestedLevel)
				table = table.ParentCell.Table;
			return Object.ReferenceEquals(this, table);
		}
		public ConditionalTableStyleFormattingTypes GetTableCellConditionalTypes(TableCell tableCell) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return TableStyle.GetConditionalPropertiesMask(GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		public TableCellProperties GetTableCellProperties(TableCellPropertiesOptions.Mask mask, TableCell tableCell) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return TableStyle.GetTableCellProperties(mask, GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		public CharacterProperties GetCharacterProperties(CharacterFormattingOptions.Mask mask, TableCell tableCell) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return TableStyle.GetCharacterProperties(mask, GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		public ParagraphProperties GetParagraphProperties(ParagraphFormattingOptions.Mask mask, TableCell tableCell) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return TableStyle.GetParagraphProperties(mask, GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		public MergedParagraphProperties GetMergedParagraphProperties(TableCell tableCell) {
			int rowIndex = tableCell.RowIndex;
			int cellIndex = tableCell.IndexInRow;
			return TableStyle.GetMergedParagraphProperties(GetRowTypeByIndex(rowIndex), GetColumnTypeByIndex(rowIndex, cellIndex));
		}
		public MergedParagraphProperties GetMergedParagraphProperties(TableStyle tableStyle, TableCell tableCell) {			
			return tableStyle.GetMergedParagraphProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public TableRowProperties GetTableRowProperties(TableRowPropertiesOptions.Mask mask, TableRow tableRow) {
			int rowIndex = tableRow.IndexInTable;
			return TableStyle.GetTableRowProperties(mask, GetRowTypeByIndex(rowIndex));
		}
		public SortedList<int> GetExistingValidColumnsPositions() {
			SortedList<int> result = new SortedList<int>();
			int rowCount = rows.Count;
			int maxColumnIndex = 0;
			for(int rowIndex = 0;rowIndex<rowCount;rowIndex++) {				
				TableRow row = rows[rowIndex];
				int columnIndex = 0;
				Debug.Assert(row.GridBefore == 0 && row.GridAfter == 0);
				if(!result.Contains(columnIndex))
					result.Add(columnIndex);
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					columnIndex += cells[cellIndex].ColumnSpan;
					maxColumnIndex = Math.Max(columnIndex, maxColumnIndex);
					if (cellIndex < cellCount - 1 && !result.Contains(columnIndex))
						result.Add(columnIndex);
				}				
			}
			if (!result.Contains(maxColumnIndex))
				result.Add(maxColumnIndex);
			return result;
		}
	}
	#endregion
	#region TableItemCollectionCore<TItem, TOwner>
	public class TableItemCollectionCore<TItem, TOwner>
		where TOwner : class
		where TItem : class {
		readonly List<TItem> items;
		readonly TOwner owner;
		public TableItemCollectionCore(TOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.items = new List<TItem>();
		}
		protected List<TItem> Items { get { return items; } }
		public TItem this[int index] {
			[DebuggerStepThrough]
			get { return items[index]; }
		}
		public int Count { get { return items.Count; } }
		public TOwner Owner { get { return owner; } }
		protected internal TItem First { get { return Count > 0 ? Items[0] : null; } }
		protected internal TItem Last { get { return Count > 0 ? Items[Count - 1] : null; } }
		public void ForEach(Action<TItem> action) {
			items.ForEach(action);
		}
	}
	#endregion
	#region TableRowCollection
	public class TableRowCollection : TableItemCollectionCore<TableRow, Table> {
		const int binarySearchTreshold = 32;
		public TableRowCollection(Table owner, int rowCount, int cellCount)
			: base(owner) {
			for (int i = 0; i < rowCount; i++) {
				AddRowCore(i, cellCount);
			}
		}
		Table Table { get { return Owner; } }
		DocumentModel DocumentModel { get { return Table.DocumentModel; } }
		internal void AddRowCore(int index, int cellCount) {
			TableRow row = new TableRow(Table, cellCount);
			AddRowCore(index, row);
		}
		internal void AddRowCore(int index, TableRow row) {
			Items.Insert(index, row);
		}
		internal void DeleteRowCore(int index) {
			Items.RemoveAt(index);
		}
		internal int IndexOf(TableRow row) {
			if (Items.Count >= binarySearchTreshold)
				return IndexOfBinarySearch(row);
			else
				return Items.IndexOf(row);
		}
		int IndexOfBinarySearch(TableRow row) {
			TableCell firstCell = row.Cells.First;
			if (firstCell == null)
				return Items.IndexOf(row);
			ParagraphIndex startParagraphIndex = firstCell.StartParagraphIndex;
			int index = Algorithms.BinarySearch(Items, new TableRowAndParagraphIndexComparable(startParagraphIndex));
			if (index >= 0)
				return index;
			else
				return -1;
		}
		public void AddInternal(TableRow row) {
			Items.Add(row);
		}
		internal void RemoveInternal(TableRow row) {
			int index = Items.IndexOf(row);
			if (index >= 0) {
				Items.RemoveAt(index);
			}
		}
	}
	#endregion
	public class TableRowAndParagraphIndexComparable : IComparable<TableRow> {
		ParagraphIndex paragraphIndex;
		public TableRowAndParagraphIndexComparable(ParagraphIndex paragraphIndex) {
			this.paragraphIndex = paragraphIndex;
		}
		public int CompareTo(TableRow other) {
			return other.FirstCell.StartParagraphIndex - paragraphIndex;
		}
	}
	#region TableCellCollection
	public class TableCellCollection : TableItemCollectionCore<TableCell, TableRow> {
		public TableCellCollection(TableRow row, int cellCount)
			: base(row) {
			for (int i = 0; i < cellCount; i++) {
				AddCellCore(i);
			}
		}
		TableRow Row { get { return Owner; } }
		void AddCellCore(int index) {
			AddCellCore(index, new TableCell(Row));
		}
		internal void AddCellCore(int index, TableCell cell) {
			Items.Insert(index, cell);
		}
		internal int IndexOf(TableCell cell) {
			return Items.IndexOf(cell);
		}
		public void AddInternal(TableCell cell) {
			Items.Add(cell);
		}
		internal void DeleteInternal(TableCell cell) {
			Items.Remove(cell);
		}
		internal void DeleteInternal(int cellIndex) {
			Items.RemoveAt(cellIndex);
		}
	}
	#endregion
	#region TableRowLayoutProperties
	public class TableRowLayoutProperties {
		int gridBefore;
		int gridAfter;
		public int GridBefore { get { return gridBefore; } set { gridBefore = value; } }
		public int GridAfter { get { return gridAfter; } set { gridAfter = value; } }
		protected internal virtual void CopyFrom(TableRowLayoutProperties sourceProperties) {
			GridAfter = sourceProperties.GridAfter;
			GridBefore = sourceProperties.GridBefore;
		}
	}
	#endregion
	#region TableRow
	public class TableRow {
		#region Fields
		Table table;
		ConditionalRowType conditionalType;
		readonly TableRowProperties properties;
		readonly TableRowLayoutProperties layoutProperties;
		readonly TableCellCollection cells;
		readonly TableProperties tablePropertiesException;
		#endregion
		internal TableRow(Table table) 
			: this(table, 0) {
		}
		public TableRow(Table table, int cellCount) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
			this.cells = new TableCellCollection(this, cellCount);
			this.properties = new TableRowProperties(PieceTable);
			this.layoutProperties = new TableRowLayoutProperties();
			this.tablePropertiesException = new TableProperties(PieceTable);
			SubscribeRowPropertiesEvents();
		}
		#region Properties
		public Table Table { get { return table; } }
		public TableRowProperties Properties { get { return properties; } }
		public DocumentModel DocumentModel { get { return Table.DocumentModel; } }
		public PieceTable PieceTable { get { return Table.PieceTable; } }
		public TableRowLayoutProperties LayoutProperties { get { return layoutProperties; } }
		public TableProperties TablePropertiesException { get { return tablePropertiesException; } }
		public ConditionalRowType ConditionalType { get { return conditionalType; } set { conditionalType = value; } }
		public TableCellCollection Cells {
			[DebuggerStepThrough]
			get { return cells; }
		}
		public TableCell FirstCell { get { return Cells.First; } }
		public TableCell LastCell { get { return Cells.Last; } }
		public int IndexInTable { get { return table.Rows.IndexOf(this); } }
		#endregion
		#region Row properties
		public HeightUnit Height { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseHeight).Height; } }
		public WidthUnit WidthBefore { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseWidthBefore).WidthBefore; } }
		public WidthUnit WidthAfter { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseWidthAfter).WidthAfter; ; } }
		public WidthUnit CellSpacing { get { return GetCellSpacing(); } }
		public bool Header { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseHeader).Header; } set { Properties.Header = value; } }
		public bool HideCellMark { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseHideCellMark).HideCellMark; } set { Properties.HideCellMark = value; } }
		public bool CantSplit { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseCantSplit).CantSplit; } set { Properties.CantSplit = value; } }
		public TableRowAlignment TableRowAlignment { get { return GetTableRowAlignment(); } set { Properties.TableRowAlignment = value; } }
		public int GridAfter { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseGridAfter).GridAfter; } set { Properties.GridAfter = value; } }
		public int GridBefore { get { return GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask.UseGridBefore).GridBefore; } set { Properties.GridBefore = value; } }
		public bool IsFirstRowInTable { get { return Table.FirstRow == this; } }
		public bool IsLastRowInTable { get { return Table.LastRow == this; } }
		public TableRow Next {
			get {
				if (IsLastRowInTable)
					return null;
				else
					return Table.Rows[Table.Rows.IndexOf(this) + 1];
			}
		}
		public TableRow Previous {
			get {
				if (IsFirstRowInTable)
					return null;
				else
					return Table.Rows[Table.Rows.IndexOf(this) - 1];
			}
		}
		#endregion
		TableRowAlignment GetTableRowAlignment() {
			if (Properties.GetUse(TableRowPropertiesOptions.Mask.UseTableRowAlignment))
				return Properties.TableRowAlignment;
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseTableAlignment))
				return TablePropertiesException.TableAlignment;
			return Table.TableAlignment;
		}
		WidthUnit GetCellSpacing() {
			if (Properties.GetUse(TableRowPropertiesOptions.Mask.UseCellSpacing))
				return Properties.CellSpacing;
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseCellSpacing))
				return TablePropertiesException.CellSpacing;
			WidthUnit tableCellsSpacing = Table.CellSpacing;
			if (tableCellsSpacing != DocumentModel.DefaultTableProperties.CellSpacing)
				return tableCellsSpacing;
			TableRowProperties result = Table.GetTableRowProperties(TableRowPropertiesOptions.Mask.UseCellSpacing, this);
			if (result != null)
				return result.CellSpacing;
			return DocumentModel.DefaultTableRowProperties.CellSpacing;
		}
		TableRowProperties GetTableRowPropertiesSource(TableRowPropertiesOptions.Mask mask) {
			if (Properties.GetUse(mask))
				return Properties;
			TableRowProperties result = Table.GetTableRowProperties(mask, this);
			if (result != null)
				return result;
			return DocumentModel.DefaultTableRowProperties;
		}
		public void ResetConditionalType() {
			ConditionalType = ConditionalRowType.Unknown;
		}
		protected internal virtual MergedTableRowProperties GetParentMergedTableRowProperties() {
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(Table.TableStyle.GetMergedTableRowProperties(table.GetRowTypeByIndex(IndexInTable)));
			merger.Merge(DocumentModel.DefaultTableRowProperties);
			return merger.MergedProperties;
		}
		protected internal virtual void SubscribeRowPropertiesEvents() {
			Properties.ObtainAffectedRange += OnRowPropertiesObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeRowPropertiesEvents() {
			Properties.ObtainAffectedRange -= OnRowPropertiesObtainAffectedRange;
		}
		protected internal virtual void OnRowPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			Table.OnPropertiesObtainAffectedRangeCore(e, IndexInTable);
		}
		protected internal virtual MarginUnitBase GetLeftCellMarginConsiderExceptions() {
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseLeftMargin))
				return TablePropertiesException.CellMargins.Left;
			return Table.LeftMargin;
		}
		protected internal virtual MarginUnitBase GetRightCellMarginConsiderExceptions() {
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseRightMargin))
				return TablePropertiesException.CellMargins.Right;
			return Table.RightMargin;
		}
		protected internal virtual MarginUnitBase GetTopCellMarginConsiderExceptions() {
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseTopMargin))
				return TablePropertiesException.CellMargins.Top;
			return Table.TopMargin;
		}
		protected internal virtual MarginUnitBase GetBottomCellMarginConsiderExceptions() {
			if (TablePropertiesException.GetUse(TablePropertiesOptions.Mask.UseBottomMargin))
				return TablePropertiesException.CellMargins.Bottom;
			return Table.BottomMargin;
		}
		protected internal virtual void CopyFrom(TableRow sourceRow) {
			Properties.CopyFrom(sourceRow.Properties);
			LayoutProperties.CopyFrom(sourceRow.LayoutProperties);
			TablePropertiesException.CopyFrom(sourceRow.TablePropertiesException);
		}
		protected internal virtual void SetTable(Table table) {
			this.table = table;
		}
	}
	#endregion
	public class TableCellLayoutProperties {
		int columnSpan;
		WidthsContentInfo containerWidthsInfo;
		WidthsContentInfo contentWidthsInfo;
		public int ColumnSpan { get { return columnSpan; } set { columnSpan = value; } }
		public WidthsContentInfo ContainerWidthsInfo { get { return containerWidthsInfo; } set { containerWidthsInfo = value; } }
		public WidthsContentInfo ContentWidthsInfo { get { return contentWidthsInfo; } set { contentWidthsInfo = value; } }
	}
	#region TableBorderAccessor (abstract)
	public abstract class TableBorderAccessor {
		public abstract TablePropertiesOptions.Mask UseMask { get; }
		public abstract TableCellPropertiesOptions.Mask CellUseMask { get; }
		public abstract TablePropertiesOptions.Mask UseInsideBorderMask { get; }
		public abstract BorderBase GetBorder(TableCellBorders borders);
		public abstract BorderBase GetBorder(TableBorders borders);
		public abstract BorderBase GetInsideBorder(TableCellBorders borders);
		public abstract BorderBase GetInsideBorder(TableBorders borders);
		public BorderBase GetTableBorder(TableBorders borders, bool isInside) {
			return isInside ? GetInsideBorder(borders) : GetBorder(borders);
		}
	}
	#endregion
	#region TableLeftBorderAccessor
	public class TableLeftBorderAccessor : TableBorderAccessor {
		public override TablePropertiesOptions.Mask UseMask { get { return TablePropertiesOptions.Mask.UseLeftBorder; } }
		public override TableCellPropertiesOptions.Mask CellUseMask { get { return TableCellPropertiesOptions.Mask.UseLeftBorder; } }
		public override TablePropertiesOptions.Mask UseInsideBorderMask { get { return TablePropertiesOptions.Mask.UseInsideVerticalBorder; } }
		public override BorderBase GetBorder(TableCellBorders borders) {
			return borders.LeftBorder;
		}
		public override BorderBase GetBorder(TableBorders borders) {
			return borders.LeftBorder;
		}
		public override BorderBase GetInsideBorder(TableCellBorders borders) {
			return borders.InsideVerticalBorder;
		}
		public override BorderBase GetInsideBorder(TableBorders borders) {
			return borders.InsideVerticalBorder;
		}
	}
	#endregion
	#region TableRightBorderAccessor
	public class TableRightBorderAccessor : TableBorderAccessor {
		public override TablePropertiesOptions.Mask UseMask { get { return TablePropertiesOptions.Mask.UseRightBorder; } }
		public override TableCellPropertiesOptions.Mask CellUseMask { get { return TableCellPropertiesOptions.Mask.UseRightBorder; } }
		public override TablePropertiesOptions.Mask UseInsideBorderMask { get { return TablePropertiesOptions.Mask.UseInsideVerticalBorder; } }
		public override BorderBase GetBorder(TableCellBorders borders) {
			return borders.RightBorder;
		}
		public override BorderBase GetBorder(TableBorders borders) {
			return borders.RightBorder;
		}
		public override BorderBase GetInsideBorder(TableCellBorders borders) {
			return borders.InsideVerticalBorder;
		}
		public override BorderBase GetInsideBorder(TableBorders borders) {
			return borders.InsideVerticalBorder;
		}
	}
	#endregion
	#region TableTopBorderAccessor
	public class TableTopBorderAccessor : TableBorderAccessor {
		public override TablePropertiesOptions.Mask UseMask { get { return TablePropertiesOptions.Mask.UseTopBorder; } }
		public override TableCellPropertiesOptions.Mask CellUseMask { get { return TableCellPropertiesOptions.Mask.UseTopBorder; } }
		public override TablePropertiesOptions.Mask UseInsideBorderMask { get { return TablePropertiesOptions.Mask.UseInsideHorizontalBorder; } }
		public override BorderBase GetBorder(TableCellBorders borders) {
			return borders.TopBorder;
		}
		public override BorderBase GetBorder(TableBorders borders) {
			return borders.TopBorder;
		}
		public override BorderBase GetInsideBorder(TableCellBorders borders) {
			return borders.InsideHorizontalBorder;
		}
		public override BorderBase GetInsideBorder(TableBorders borders) {
			return borders.InsideHorizontalBorder;
		}
	}
	#endregion
	#region TableBottomBorderAccessor
	public class TableBottomBorderAccessor : TableBorderAccessor {
		public override TablePropertiesOptions.Mask UseMask { get { return TablePropertiesOptions.Mask.UseBottomBorder; } }
		public override TableCellPropertiesOptions.Mask CellUseMask { get { return TableCellPropertiesOptions.Mask.UseBottomBorder; } }
		public override TablePropertiesOptions.Mask UseInsideBorderMask { get { return TablePropertiesOptions.Mask.UseInsideHorizontalBorder; } }
		public override BorderBase GetBorder(TableCellBorders borders) {
			return borders.BottomBorder;
		}
		public override BorderBase GetBorder(TableBorders borders) {
			return borders.BottomBorder;
		}
		public override BorderBase GetInsideBorder(TableCellBorders borders) {
			return borders.InsideHorizontalBorder;
		}
		public override BorderBase GetInsideBorder(TableBorders borders) {
			return borders.InsideHorizontalBorder;
		}
	}
	#endregion
	#region TableCell
	public class TableCell : ICellPropertiesOwner {
		#region Fields
		int styleIndex = 0;
		readonly TableRow row;
		readonly TableCellProperties properties;
		readonly TableCellLayoutProperties layoutProperties;
		ConditionalColumnType conditionalType;
		ParagraphIndex startParagraphIndex = new ParagraphIndex(-1);
		ParagraphIndex endParagraphIndex = new ParagraphIndex(-1);
		#endregion
		public TableCell(TableRow row) {
			Guard.ArgumentNotNull(row, "row");
			this.row = row;
			this.properties = new TableCellProperties(PieceTable, this);
			this.layoutProperties = new TableCellLayoutProperties();
			SubscribeCellPropertiesEvents();
		}
		#region Properties
		public TableRow Row { get { return row; } }
		public TableCellProperties Properties { get { return properties; } }
		public DocumentModel DocumentModel { get { return Row.DocumentModel; } }
		public PieceTable PieceTable { get { return Row.PieceTable; } }
		public Table Table { get { return Row.Table; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public ConditionalColumnType ConditionalType { get { return conditionalType; } set { conditionalType = value; } }
		internal int RowIndex { get { return Table.Rows.IndexOf(Row); } }
		internal int IndexInRow { get { return Row.Cells.IndexOf(this); } }
		public TableCellStyle TableCellStyle { get { return DocumentModel.TableCellStyles[StyleIndex]; } }
		public int StyleIndex {
			get { return styleIndex; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("TableStyleIndex", value);
				if (styleIndex == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					ChangeTableCellStyleIndexHistoryItem item = new ChangeTableCellStyleIndexHistoryItem(PieceTable, this.Table.Index, this.RowIndex, this.IndexInRow, styleIndex, value);
					DocumentModel.History.Add(item);
					item.Execute();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
				#endregion
		#region Cell properties
		public PreferredWidth PreferredWidth { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UsePreferredWidth).PreferredWidth; } }
		public bool HideCellMark { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseHideCellMark).HideCellMark; } set { Properties.HideCellMark = value; } }
		public bool NoWrap { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseNoWrap).NoWrap; } set { Properties.NoWrap = value; } }
		public bool FitText { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseFitText).FitText; } set { Properties.FitText = value; } }
		public TextDirection TextDirection { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseTextDirection).TextDirection; } set { Properties.TextDirection = value; } }
		public VerticalAlignment VerticalAlignment { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseVerticalAlignment).VerticalAlignment; } set { Properties.VerticalAlignment = value; } }
		public int ColumnSpan { get { return Properties.ColumnSpan; } set { Properties.ColumnSpan = value; } }
		public MergingState VerticalMerging { get { return Properties.VerticalMerging; } set { Properties.VerticalMerging = value; } }
		public ConditionalTableStyleFormattingTypes CellConditionalFormatting { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseCellConditionalFormatting).CellConditionalFormatting; } set { Properties.CellConditionalFormatting = value; } }
		public ConditionalTableStyleFormattingTypes CellConditionalFormattingMasks { get { return Table.GetTableCellConditionalTypes(this); } }
		public Color BackgroundColor { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseBackgroundColor).BackgroundColor; } set { Properties.BackgroundColor = value; } }
		public Color ForegroundColor { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseForegroundColor).ForegroundColor; } set { Properties.ForegroundColor = value; } }
		public ShadingPattern Shading { get { return GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask.UseShading).ShadingPattern; } set { Properties.ShadingPattern = value; } }
		public bool IsFirstCellInRow { get { return Row.Cells.First == this; } }
		public bool IsLastCellInRow { get { return Row.Cells.Last == this; } }
		public bool IsFirstRow { get { return Table.Rows.First == Row; } }
		public bool IsLastRow { get { return Table.Rows.Last == Row; } }
		public bool IsFirstCellInTable { get { return Table.FirstRow.FirstCell == this; } }
		public bool IsLastCellInTable { get { return Table.LastRow.LastCell == this; } }
		public bool AvoidDoubleBorders { get { return Table.TableProperties.AvoidDoubleBorders; } }
		public TableCell Next {
			get {
				if (IsLastCellInRow) {
					if (Row.IsLastRowInTable)
						return null;
					else
						return Row.Next.FirstCell;
				}
				else
					return Row.Cells[Row.Cells.IndexOf(this) + 1];
			}
		}
		public TableCell NextCellInRow {
			get {
				if (IsLastCellInRow)
					return null;
				else
					return Row.Cells[Row.Cells.IndexOf(this) + 1];
			}
		}
		public TableCell Previous {
			get {
				if (IsFirstCellInRow) {
					if (Row.IsFirstRowInTable)
						return null;
					else
						return Row.Previous.LastCell;
				}
				else
					return Row.Cells[Row.Cells.IndexOf(this) - 1];
			}
		}
		public TableCellLayoutProperties LayoutProperties { get { return layoutProperties; } }
		#endregion
		public void ResetConditionalType() {
			conditionalType = ConditionalColumnType.Unknown;
		}
		TableCellProperties GetTableCellPropertiesSource(TableCellPropertiesOptions.Mask mask) {
			if (Properties.GetUse(mask))
				return Properties;
			TableCellProperties resultCell = TableCellStyle.GetTableCellProperties(mask);
			if (resultCell != null)
				return resultCell;
			TableCellProperties resultTable = Table.GetTableCellProperties(mask, this);
			if (resultTable != null)
				return resultTable;
			return DocumentModel.DefaultTableCellProperties;
		}
		protected internal virtual MergedTableCellProperties GetParentMergedTableCellProperties() {
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(TableCellStyle.GetMergedTableCellProperties());
			merger.Merge(Table.TableStyle.GetMergedTableCellProperties(Table.GetRowTypeByIndex(RowIndex), Table.GetColumnTypeByIndex(RowIndex, IndexInRow)));
			merger.Merge(DocumentModel.DefaultTableCellProperties);
			return merger.MergedProperties;
		}
		public int GetMinCellWidth() {
			return GetActualLeftMargin().Value + GetActualRightMargin().Value + GetActualRightCellBorder().Width + GetActualLeftCellBorder().Width;
		}
		public MarginUnitBase GetActualLeftMargin() {
			return GetActualCellMarginBase(LeftMarginUnit.PropertyAccessor);
		}
		public MarginUnitBase GetActualRightMargin() {
			return GetActualCellMarginBase(RightMarginUnit.PropertyAccessor);
		}
		public MarginUnitBase GetActualTopMargin() {
			return GetActualCellMarginBase(TopMarginUnit.PropertyAccessor);
		}
		public MarginUnitBase GetActualBottomMargin() {
			return GetActualCellMarginBase(BottomMarginUnit.PropertyAccessor);
		}
		protected virtual MarginUnitBase GetActualCellMarginBase(MarginUnitBase.MarginPropertyAccessorBase accessor) {
			if (Properties.GetUse(accessor.CellPropertiesMask))
				return accessor.GetValue(Properties.CellMargins);
			TableCellStyle currentCellStyle = TableCellStyle;
			while (currentCellStyle != null) {
				if (currentCellStyle.TableCellProperties.GetUse(accessor.CellPropertiesMask))
					return accessor.GetValue(currentCellStyle.TableCellProperties.CellMargins);
				currentCellStyle = currentCellStyle.Parent;
			}
			TableStyle currentTableStyle = Table.TableStyle;
			while (currentTableStyle != null) {
				if (currentTableStyle.TableCellProperties.GetUse(accessor.CellPropertiesMask))
					return accessor.GetValue(currentTableStyle.TableCellProperties.CellMargins);
				currentTableStyle = currentTableStyle.Parent;
			}
			TableProperties tablePropertiesException = Row.TablePropertiesException;
			if (tablePropertiesException.GetUse(accessor.TablePropertiesMask))
				return accessor.GetValue(tablePropertiesException.CellMargins);
			if (Table.TableProperties.GetUse(accessor.TablePropertiesMask))
				return accessor.GetValue(Table.TableProperties.CellMargins);
			currentTableStyle = Table.TableStyle;
			while (currentTableStyle != null) {
				if (currentTableStyle.TableProperties.GetUse(accessor.TablePropertiesMask))
					return accessor.GetValue(currentTableStyle.TableProperties.CellMargins);
				currentTableStyle = currentTableStyle.Parent;
			}
			return accessor.GetValue(DocumentModel.DefaultTableCellProperties.CellMargins);
		}
		internal MergedCharacterProperties GetMergedCharacterProperties() {
			return TableCellStyle.GetMergedCharacterProperties(Table.GetRowTypeByIndex(RowIndex), Table.GetColumnTypeByIndex(RowIndex, IndexInRow));
		}
		internal MergedParagraphProperties GetMergedParagraphProperties() {
			return TableCellStyle.GetMergedParagraphProperties(Table.GetRowTypeByIndex(RowIndex), Table.GetColumnTypeByIndex(RowIndex, IndexInRow));
		}
		BorderBase GetActualCellBorder(TableBorderAccessor borderAccessor, bool isBorderCell) {
			if (Properties.GetUse(borderAccessor.CellUseMask))
				return borderAccessor.GetBorder(Properties.Borders);
			TableCellProperties tableCellStyleCellProperties = TableCellStyle.GetTableCellProperties(borderAccessor.CellUseMask);
			if (tableCellStyleCellProperties != null)
				return borderAccessor.GetBorder(tableCellStyleCellProperties.Borders);
			TableCellProperties tableStyleCellProperties = Table.GetTableCellProperties(borderAccessor.CellUseMask, this);
			if (tableStyleCellProperties != null && isBorderCell)
				return borderAccessor.GetBorder(tableStyleCellProperties.Borders);
			int cellSpacing = Table.CellSpacing.Value;
			bool insideBorder;
			if (isBorderCell && cellSpacing <= 0) {
				if (Row.TablePropertiesException.GetUse(borderAccessor.UseMask))
					return borderAccessor.GetBorder(Row.TablePropertiesException.Borders);
				TableProperties properties = Table.GetTablePropertiesSourceForCell(borderAccessor.UseMask, this, true, out insideBorder);
				if (properties != null)
					return borderAccessor.GetBorder(properties.Borders);
			}
			else {
				if (Row.TablePropertiesException.GetUse(borderAccessor.UseInsideBorderMask))
					return borderAccessor.GetInsideBorder(Row.TablePropertiesException.Borders);
				TableProperties properties = Table.GetTablePropertiesSourceForCell(borderAccessor.UseMask, this, false, out insideBorder);
				if (properties != null)
					return borderAccessor.GetTableBorder(properties.Borders, insideBorder);
			}
			Debug.Assert(false);
			return borderAccessor.GetBorder(DocumentModel.DefaultTableCellProperties.Borders);
		}
		public BorderBase GetActualLeftCellBorder() {
			return GetActualCellBorder(new TableLeftBorderAccessor(), IsFirstCellInRow);
		}
		public BorderBase GetActualRightCellBorder() {
			return GetActualCellBorder(new TableRightBorderAccessor(), IsLastCellInRow);
		}
		public BorderBase GetActualTopCellBorder() {
			if (AvoidDoubleBorders && ShouldReturnEmptyTopCellBorder())
				return DocumentModel.EmptyTopBorder;
			return GetActualCellBorder(new TableTopBorderAccessor(), IsFirstRow);
		}
		bool ShouldReturnEmptyTopCellBorder() {
			if (!IsFirstRow || Table.ParentCell == null)
				return false;
			return Row.FirstCell.StartParagraphIndex == Table.ParentCell.StartParagraphIndex;
		}
		public BorderBase GetActualBottomCellBorder() {
			if (AvoidDoubleBorders && ShouldReturnEmptyBottomCellBorder())
				return DocumentModel.EmptyBottomBorder;
			return GetActualCellBorder(new TableBottomBorderAccessor(), IsLastRow);
		}
		bool ShouldReturnEmptyBottomCellBorder() {
			if (!IsLastRow || Table.ParentCell == null)
				return false;
			ParagraphIndex parentCellEndParagraphIndex = Table.ParentCell.EndParagraphIndex;
			ParagraphIndex lastCellEndParagraphIndex = Row.LastCell.EndParagraphIndex;
			if (lastCellEndParagraphIndex + 1 != parentCellEndParagraphIndex)
				return false;
			Paragraph lastCellEndParagraph = PieceTable.Paragraphs[lastCellEndParagraphIndex];
			Paragraph parentCellEndParagraph = PieceTable.Paragraphs[parentCellEndParagraphIndex];
			return PieceTable.VisibleTextFilter.GetNextVisibleRunIndex(lastCellEndParagraph.LastRunIndex) >= parentCellEndParagraph.LastRunIndex;
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == this.Properties);
			return new TableCellPropertiesChangedHistoryItem(PieceTable, StartParagraphIndex, Table.NestedLevel);
		}
		#endregion
		protected internal virtual void SubscribeCellPropertiesEvents() {
			Properties.ObtainAffectedRange += OnCellPropertiesObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeCellPropertiesEvents() {
			Properties.ObtainAffectedRange -= OnCellPropertiesObtainAffectedRange;
		}
		protected internal virtual void OnCellPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			ParagraphCollection paragraphs = this.PieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			if (StartParagraphIndex < ParagraphIndex.Zero || StartParagraphIndex >= count ||
				EndParagraphIndex < ParagraphIndex.Zero || EndParagraphIndex >= count)
				return;
			Paragraph startParagraph = paragraphs[StartParagraphIndex];
			e.Start = startParagraph.FirstRunIndex;
			Paragraph endParagraph = EndParagraphIndex != StartParagraphIndex ? paragraphs[EndParagraphIndex] : startParagraph;
			e.End = endParagraph.LastRunIndex;
		}
		internal int GetStartColumnIndexConsiderRowGrid() {
			return TableCellVerticalBorderCalculator.GetStartColumnIndex(this, false);
		}
		internal int GetEndColumnIndexConsiderRowGrid() {
			return GetEndColumnIndexConsiderRowGrid(GetStartColumnIndexConsiderRowGrid());
		}
		internal int GetEndColumnIndexConsiderRowGrid(int startColumnIndex) {
			return startColumnIndex + ColumnSpan - 1;
		}
		internal List<TableCell> GetVerticalSpanCells() {
			return TableCellVerticalBorderCalculator.GetVerticalSpanCells(this, false);
		}
		internal void SetTableCellStyleIndexCore(int newStyleIndex) {
			this.styleIndex = newStyleIndex;
			if (StartParagraphIndex < ParagraphIndex.Zero)
				return;
			for (ParagraphIndex i = StartParagraphIndex; i <= EndParagraphIndex; i++)
				PieceTable.Paragraphs[i].ResetCachedIndices(ResetFormattingCacheType.All);
			new TableConditionalFormattingController(this.Table).ResetCachedProperties(0);
			if (StartParagraphIndex >= ParagraphIndex.Zero) {
				RunIndex start = PieceTable.Paragraphs[StartParagraphIndex].FirstRunIndex;
				RunIndex end = PieceTable.Paragraphs[EndParagraphIndex].LastRunIndex;
				PieceTable.ApplyChangesCore(TableChangeActionCalculator.CalculateChangeActions(TableChangeType.TableStyle), start, end);
			}
		}
		public void CopyProperties(TableCell sourceCell) {
			Properties.CopyFrom(sourceCell.Properties);
			StyleIndex = sourceCell.TableCellStyle.Copy(DocumentModel);
		}
		internal Color GetActualBackgroundColor() {
			if (Shading == ShadingPattern.Solid)
				return ForegroundColor;
			else
				return BackgroundColor;
		}
	}
	#endregion
}
