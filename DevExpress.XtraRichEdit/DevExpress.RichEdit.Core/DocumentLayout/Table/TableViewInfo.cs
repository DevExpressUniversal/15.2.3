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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
namespace DevExpress.XtraRichEdit.Layout.TableLayout {
	public class VerticalBorderPositions {
		SortedList<LayoutUnit> initialPositions;
		SortedList<LayoutUnit> alignmentedPositions;
		public VerticalBorderPositions(SortedList<LayoutUnit> initialPositions, SortedList<LayoutUnit> alignmentedPositions) {
			Guard.ArgumentNotNull(initialPositions, "initialPositions");
			Guard.ArgumentNotNull(alignmentedPositions, "alignmentedPositions");
			this.initialPositions = initialPositions;
			this.alignmentedPositions = alignmentedPositions;
		}
		public SortedList<LayoutUnit> InitialPositions { get { return initialPositions; } }
		public SortedList<LayoutUnit> AlignmentedPosition { get { return alignmentedPositions; } }
	}
	#region TableViewInfo
	public class TableViewInfo {
		#region Fields
		readonly Table table;
		readonly TableRowViewInfoCollection rows;
		readonly TableCellViewInfoCollection cells;
		LayoutUnit minBottomVerticalOffset;
		LayoutUnit textAreaLeft;
		readonly Column topLevelColumn;
		readonly Column column;
		readonly TableCellVerticalAnchorCollection anchors;
		int topRowIndex;
		VerticalBorderPositions verticalBorderPositions;
		LayoutUnit leftOffset;
		ModelUnit modelRelativeIndent;
		TableViewInfo prevTableViewInfo;
		TableViewInfo nextTableViewInfo;
		TableCellViewInfo parentTableCellViewInfo;
		bool firstContentInParentCell;		
		#endregion
		public TableViewInfo(Table table, Column topLevelColumn, Column column, VerticalBorderPositions verticalBorderPositions, int topRowIndex, TableCellViewInfo parentTableCellViewInfo, bool firstContentInParentCell) {
			Guard.ArgumentNotNull(table, "table");
			this.cells = new TableCellViewInfoCollection();
			this.rows = new TableRowViewInfoCollection(this);
			this.anchors = new TableCellVerticalAnchorCollection();
			this.table = table;
			this.topLevelColumn = topLevelColumn;
			this.column = column;
			this.verticalBorderPositions = verticalBorderPositions;
			this.topRowIndex = topRowIndex;
			this.parentTableCellViewInfo = parentTableCellViewInfo;
			this.firstContentInParentCell = firstContentInParentCell;
		}
		#region Properties
		public FloatingObjectsLayout AssociatedFloatingObjectsLayout { get; set; }
		public bool SingleColumn { get { return prevTableViewInfo == null && nextTableViewInfo == null; } }
		public TableCellViewInfoCollection Cells { get { return cells; } }
		public TableRowViewInfoCollection Rows { get { return rows; } }
		public TableCellVerticalAnchorCollection Anchors { get { return anchors; } }
		public TableCellViewInfo ParentTableCellViewInfo { get { return parentTableCellViewInfo; } }
		public Table Table { get { return table; } }
		public LayoutUnit MinBottomVerticalOffset { get { return minBottomVerticalOffset; } set { minBottomVerticalOffset = value; } }
		public ITableCellVerticalAnchor TopAnchor { get { return Rows.First.TopAnchor; } }
		public ITableCellVerticalAnchor BottomAnchor { get { return Rows.Last.BottomAnchor; } }
		public Column TopLevelColumn { get { return topLevelColumn; } }
		public Column Column { get { return column; } }
		public LayoutUnit LeftOffset { get { return leftOffset; } set { leftOffset = value; } }
		public ModelUnit ModelRelativeIndent { get { return modelRelativeIndent; } set { modelRelativeIndent = value; } }
		public int TopRowIndex { get { return topRowIndex; } }
		public int RowCount { get { return Anchors.Count - 1; } }
		public int BottomRowIndex { get { return TopRowIndex + RowCount - 1; } }
		public TableViewInfo PrevTableViewInfo { get { return prevTableViewInfo; } set { prevTableViewInfo = value; }}
		public TableViewInfo NextTableViewInfo { get { return nextTableViewInfo; } set { nextTableViewInfo = value; } }
		public bool FirstContentInParentCell { get { return firstContentInParentCell; } }
		public VerticalBorderPositions VerticalBorderPositions { get { return verticalBorderPositions; } set { verticalBorderPositions = value; } }
		public LayoutUnit TextAreaOffset { get { return textAreaLeft; } set { textAreaLeft = value; } }
		#endregion        
		public BorderBase GetActualLeftBorder() {
			return Table.GetActualLeftBorder();
		}
		public BorderBase GetActualRightBorder() {
			return Table.GetActualRightBorder();
		}
		public BorderBase GetActualTopBorder() {
			return Table.GetActualTopBorder();
		}
		public BorderBase GetActualBottomBorder() {
			return Table.GetActualBottomBorder();
		}
		public void SetTopRowIndex(int topRowIndex) {
			this.topRowIndex = topRowIndex;
		}
		public LayoutUnit GetColumnLeft(int columnIndex) {
			return VerticalBorderPositions.AlignmentedPosition[columnIndex] + Column.Bounds.Left;
		}
		public LayoutUnit GetColumnRight(int columnIndex) {
			return VerticalBorderPositions.AlignmentedPosition[columnIndex + 1] + Column.Bounds.Left;
		}
		internal int GetVerticalBorderPosition(int borderIndex) {
			return VerticalBorderPositions.AlignmentedPosition[borderIndex] + Column.Bounds.Left;
		}
		public void ExportTo(IDocumentLayoutExporter exporter) {
			int rowCount = Rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableRowViewInfoBase row = Rows[i];
				if (exporter.IsTableRowVisible(row))
					ExportCellsVerticalBorders(row, exporter);
			}
			int count = Anchors.Count;
			for (int i = 0; i < count; i++) {
				TableCellVerticalAnchor anchor = Anchors[i];
				if (exporter.IsAnchorVisible(anchor))
					ExportAnchorBorders(anchor, exporter);
			}
			for (int i = 0; i < count; i++) {
				if (exporter.IsAnchorVisible(Anchors[i]))
					ExportAnchorCorner(exporter, Anchors[i]);
			}
		}
		void ExportAnchorCorner(IDocumentLayoutExporter exporter, TableCellVerticalAnchor anchor) {
			List<HorizontalCellBordersInfo> borders = anchor.CellBorders;
			if (borders == null)
				return;
			int cornerCount = anchor.Corners.Count;
			Debug.Assert(cornerCount == anchor.CellBorders.Count + 1);
			for (int i = 0; i < cornerCount; i++) {
				CornerViewInfoBase cornerViewInfo = anchor.Corners[i];
				int left = i < anchor.CellBorders.Count ? GetColumnLeft(anchor.CellBorders[i].StartColumnIndex) : GetColumnRight(anchor.CellBorders[anchor.CellBorders.Count - 1].EndColumnIndex);
				cornerViewInfo.Export(exporter, left, anchor.VerticalPosition);
			}
		}
		TableCellVerticalBorderViewInfo GetVerticalBorderByColumnIndex(TableRowViewInfoNoCellSpacing rowViewInfo, int cornerColumnIndex) {
			if (rowViewInfo == null)
				return null;
			TableRow row = rowViewInfo.Row;
			int columnIndex = row.LayoutProperties.GridBefore;
			int cellIndex = 0;
			while (cornerColumnIndex > columnIndex && cellIndex < row.Cells.Count) {
				columnIndex += row.Cells[cellIndex].LayoutProperties.ColumnSpan;
				cellIndex++;
			}
			if (cornerColumnIndex == columnIndex)
				return rowViewInfo.VerticalBorders[cellIndex];
			else
				return null;
		}
		private void ExportCellsVerticalBorders(TableRowViewInfoBase tableRowViewInfoBase, IDocumentLayoutExporter exporter) {
			tableRowViewInfoBase.ExportTo(exporter);
		}
		public void ExportBackground(IDocumentLayoutExporter exporter) {
			ExportBackgroundCore(exporter, row => exporter.ExportTableRow(row));
			ExportBackgroundCore(exporter, row => ExportRowCells(row, exporter));
		}
		void ExportBackgroundCore(IDocumentLayoutExporter exporter, Action<TableRowViewInfoBase> rowAction) {
			int rowCount = RowCount;
			for (int i = 0; i < rowCount; i++) {
				TableRowViewInfoBase row = Rows[i];
				if (exporter.IsTableRowVisible(row))
					rowAction(row);
			}
		}
		private void ExportRowCells(TableRowViewInfoBase row, IDocumentLayoutExporter exporter) {
			TableCellViewInfoCollection cells = row.Cells;
			int count = cells.Count;			
			for (int i = 0; i < count; i++) {
				if(Anchors[cells[i].TopAnchorIndex] == row.TopAnchor)
					cells[i].ExportTo(exporter);
			}
		}
		protected virtual void ExportAnchorBorders(TableCellVerticalAnchor anchor, IDocumentLayoutExporter exporter) {
			List<HorizontalCellBordersInfo> cellBorders = anchor.CellBorders;
			if (cellBorders == null)
				return;
			int count = cellBorders.Count;
			Debug.Assert(count + 1 == anchor.Corners.Count );
			for (int i = 0; i < count; i++) {
				HorizontalCellBordersInfo borderInfo = cellBorders[i];
				if (borderInfo.Border == null)
					continue;
				TableBorderViewInfoBase viewInfo = new TableCellHorizontalBorderViewInfo(anchor, borderInfo, Table.DocumentModel.ToDocumentLayoutUnitConverter, anchor.Corners[i], anchor.Corners[i+1]);
				viewInfo.ExportTo(this, exporter);
			}
		}
		public Rectangle GetRowBounds(TableRowViewInfoBase row) {			
			return row.GetBounds();			
		}
		public Rectangle GetCellBounds(TableCellViewInfo cellViewInfo) {
			int topAnchorIndex = cellViewInfo.TopAnchorIndex;
			int bottomAnchorIndex = cellViewInfo.BottomAnchorIndex;
			ITableCellVerticalAnchor topAnchor = Anchors[topAnchorIndex];
			ITableCellVerticalAnchor bottomAnchor = Anchors[bottomAnchorIndex];
			if(topAnchor == null || bottomAnchor == null)
				return new Rectangle(); 
			int top = topAnchor.VerticalPosition + topAnchor.BottomTextIndent;
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			int topBorder = borderCalculator.GetActualWidth(cellViewInfo.Cell.GetActualTopCellBorder());
			MarginUnitBase topMargin = cellViewInfo.Cell.GetActualTopMargin();
			int topMarginValue = topMargin.Type == WidthUnitType.ModelUnits ? topMargin.Value : 0;
			top -= cellViewInfo.Cell.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(topBorder + topMarginValue);
			return new Rectangle(cellViewInfo.Left, top, cellViewInfo.Width, bottomAnchor.VerticalPosition - top);
		}
		public void Complete(LayoutUnit topBorderHeight, LayoutUnit bottomBorderHeight) {
			RemoveEmptyCells();
			cells.Sort(new TableCellViewInfoByParagraphIndexComparer());
			int count = Anchors.Count;
			int rowCount = Rows.Count;
			DocumentModelUnitToLayoutUnitConverter converter = table.DocumentModel.ToDocumentLayoutUnitConverter;
			for (int i = 0; i < count; i++) {
				TableRowViewInfoNoCellSpacing topRow = i > 0 ? Rows[i - 1] as TableRowViewInfoNoCellSpacing : null;
				TableRowViewInfoNoCellSpacing bottomRow = i < rowCount ? Rows[i] as TableRowViewInfoNoCellSpacing : null;
				CreateAnchorCorner(converter, Anchors[i], topRow, bottomRow);
			}
		}
		public void RemoveEmptyCells() {
			int rowCount = Rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableRowViewInfoBase row = Rows[i];
				if (row.ContainsEmptyCell()) {
					RemoveRowsFromIndex(i);
					break;
				}
			}
			if (Rows.Count == 0) {
				if (ParentTableCellViewInfo != null) {
					if (this.FirstContentInParentCell)
						ParentTableCellViewInfo.EmptyCell = true;
					ParentTableCellViewInfo.InnerTables.Remove(this);
				}
			}
		}
		public void RemoveRowsFromIndex(int firstRowViewInfoIndex) {
			int rowCount = Rows.Count;
			for (int i = rowCount - 1; i >= firstRowViewInfoIndex; i--) {				
				Rows.RemoveAt(i);								
			}
			int lastTableAnchorIndex = firstRowViewInfoIndex;
			int totalCellCount = this.cells.Count;
			for (int i = totalCellCount - 1; i >= 0; i--) {
				if (this.cells[i].TopAnchorIndex >= lastTableAnchorIndex) {
					this.Column.TopLevelColumn.RemoveTableCellViewInfoContent(this.cells[i]);
					RemoveInnerTablesFromTopLevelColumn(cells[i]);
					this.cells.RemoveAt(i);					
				}
			}
			if (firstRowViewInfoIndex > 0) {
				Anchors.RemoveAnchors(lastTableAnchorIndex + 1, Anchors.Count - lastTableAnchorIndex - 1);
				TableRowViewInfoBase lastRow = Rows[firstRowViewInfoIndex - 1];
				TableCellViewInfoCollection cells = lastRow.Cells;
				int cellCount = cells.Count;
				for (int i = 0; i < cellCount; i++) {
					TableCellViewInfo cell = cells[i];
					if (cell.BottomAnchorIndex > lastTableAnchorIndex) {
						cell.SetBottomAnchorIndexToLastAnchor();
					}
				}
			}
			else {
				Anchors.RemoveAnchors(0, Anchors.Count);
			}
		}
		void RemoveInnerTablesFromTopLevelColumn(TableCellViewInfo tableCellViewInfo) {
			tableCellViewInfo.InnerTables.ForEach(RemoveTableViewInfoFromTopLevelColumn);			
		}
		void RemoveTableViewInfoFromTopLevelColumn(TableViewInfo tableViewInfo) {
			TableViewInfoCollection tables = this.Column.TopLevelColumn.InnerTables;
			if (tables != null)
				tables.Remove(tableViewInfo);
			int cellCount = tableViewInfo.Cells.Count;
			for (int i = 0; i < cellCount; i++) {
				this.Column.TopLevelColumn.RemoveTableCellViewInfoContent(tableViewInfo.Cells[i]);
				RemoveInnerTablesFromTopLevelColumn(tableViewInfo.Cells[i]);
			}
		}
		void CreateAnchorCorner(DocumentModelUnitToLayoutUnitConverter converter, TableCellVerticalAnchor anchor, TableRowViewInfoNoCellSpacing topRow, TableRowViewInfoNoCellSpacing bottomRow) {
			List<HorizontalCellBordersInfo> borders = anchor.CellBorders;
			if (borders == null)
				return;
			int borderCount = borders.Count;
			for (int i = -1; i < borderCount; i++) {
				HorizontalCellBordersInfo leftBorderInfo = i >= 0 ? borders[i] : null;
				HorizontalCellBordersInfo rightBorderInfo = i + 1 < borderCount ? borders[i + 1] : null;
				BorderInfo borderAtLeft = (leftBorderInfo != null) && (leftBorderInfo.Border != null) ? leftBorderInfo.Border.Info : null;
				BorderInfo borderAtRight = (rightBorderInfo != null) && (rightBorderInfo.Border != null) ? rightBorderInfo.Border.Info : null;
				int cornerColumnIndex = leftBorderInfo != null ? leftBorderInfo.EndColumnIndex + 1 : rightBorderInfo.StartColumnIndex;
				TableCellVerticalBorderViewInfo borderViewInfoAtTop = GetVerticalBorderByColumnIndex(topRow, cornerColumnIndex);
				TableCellVerticalBorderViewInfo borderViewInfoAtBottom = GetVerticalBorderByColumnIndex(bottomRow, cornerColumnIndex);
				BorderInfo borderAtTop = borderViewInfoAtTop != null ? borderViewInfoAtTop.Border : null;
				BorderInfo borderAtBottom = borderViewInfoAtBottom != null ? borderViewInfoAtBottom.Border : null;
				CornerViewInfoBase cornerViewInfo = CornerViewInfoBase.CreateCorner(converter, borderAtLeft, borderAtTop, borderAtRight, borderAtBottom, 0);
				anchor.Corners.Add(cornerViewInfo);
				if (borderViewInfoAtTop != null) {
					Debug.Assert(borderViewInfoAtTop.CornerAtBottom == null);
					borderViewInfoAtTop.CornerAtBottom = cornerViewInfo;
				}
				if (borderViewInfoAtBottom != null) {
					Debug.Assert(borderViewInfoAtBottom.CornerAtTop == null);
					borderViewInfoAtBottom.CornerAtTop = cornerViewInfo;
				}
			}
		}
		public void MoveVertically(LayoutUnit deltaY) {
			int count = Anchors.Count;
			for (int i = 0; i < count; i++)
				Anchors[i].MoveVertically(deltaY);
		}
		public void MoveVerticallyRecursive(LayoutUnit deltaY) {
			MoveVertically(deltaY);
			int count = Cells.Count;
			for (int i = 0; i < count; i++)
				Cells[i].InnerTables.MoveVertically(deltaY);
		}
		public virtual int GetAlignmentedPosition(int horizontalPositionIndex) {
			return VerticalBorderPositions.AlignmentedPosition[horizontalPositionIndex] + Column.Bounds.Left;
		}
		public virtual LayoutUnit GetActualBottomPosition() {
			ITableCellVerticalAnchor lastAnchor = Anchors.Last;
			LayoutUnit result = lastAnchor.VerticalPosition;
			if (NextTableViewInfo == null)
				return result + lastAnchor.BottomTextIndent;
			else
				return result;
		}
		public virtual int GetTableBottom() {
			ITableCellVerticalAnchor lastAnchor = Anchors.Last;
			return lastAnchor.VerticalPosition + lastAnchor.BottomTextIndent;			
		}
		public virtual int GetTableTop() {
			ITableCellVerticalAnchor firstAnchor = Anchors.First;
			return firstAnchor.VerticalPosition;
		}
	}
	#endregion
	#region TableCellViewInfoByParagraphIndexComparer
	public class TableCellViewInfoByParagraphIndexComparer : IComparer<TableCellViewInfo> {
		#region IComparer<TableCellViewInfo> Members
		public int Compare(TableCellViewInfo x, TableCellViewInfo y) {
			return x.Cell.StartParagraphIndex - y.Cell.StartParagraphIndex;
		}
		#endregion
	} 
	#endregion
	#region TableViewInfoCollection
	public class TableViewInfoCollection : List<TableViewInfo> {
		public void ExportTo(IDocumentLayoutExporter exporter) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].ExportTo(exporter);
		}
		public void MoveVertically(LayoutUnit deltaY) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].MoveVertically(deltaY);
		}
		public void ExportBackground(IDocumentLayoutExporter exporter) {
			int count = Count;
			for(int i = 0; i < count; i++) {
				this[i].ExportBackground(exporter);
			}
		}
	}
	#endregion
	#region TableViewInfoAndLogPositionComparable
	public class TableViewInfoAndLogPositionComparable : IComparable<TableViewInfo> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public TableViewInfoAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<T> Members
		public int CompareTo(TableViewInfo table) {
			DocumentLogPosition firstPos = GetFirstPosition(table);
			if (firstPos > LogPosition)
				return 1;
			else
				return -1;
		}
		public DocumentLogPosition GetFirstPosition(TableViewInfo table) {
			return PieceTable.Paragraphs[table.Cells[0].Cell.StartParagraphIndex].LogPosition;
		}
		public DocumentLogPosition GetLastPosition(TableViewInfo table) {
			return PieceTable.Paragraphs[table.Cells[table.Cells.Count - 1].Cell.EndParagraphIndex].EndLogPosition;
		}
		#endregion
	}
	#endregion
	#region TableCellViewInfoAndLogPositionComparable
	public class TableCellViewInfoAndLogPositionComparable : IComparable<TableCellViewInfo> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentLogPosition logPosition;
		#endregion
		public TableCellViewInfoAndLogPositionComparable(PieceTable pieceTable, DocumentLogPosition logPosition) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.logPosition = logPosition;
		}
		#region Properties
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		#region IComparable<T> Members
		public int CompareTo(TableCellViewInfo cell) {
			DocumentLogPosition firstPos = GetFirstPosition(cell);
			if (logPosition < firstPos)
				return 1;
			else if (logPosition > firstPos) {
				DocumentLogPosition lastPos = GetLastPosition(cell);
				if (logPosition <= lastPos)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		DocumentLogPosition GetFirstPosition(TableCellViewInfo cell) {
			return PieceTable.Paragraphs[cell.Cell.StartParagraphIndex].LogPosition;
		}
		private DocumentLogPosition GetLastPosition(TableCellViewInfo cell) {
			return PieceTable.Paragraphs[cell.Cell.EndParagraphIndex].EndLogPosition;
		}
		#endregion
	}
	#endregion
}
