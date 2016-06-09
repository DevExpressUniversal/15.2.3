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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using System.Diagnostics;
namespace DevExpress.XtraRichEdit.Layout.TableLayout {
	#region TableCellViewInfo
	public class TableCellViewInfo {
		readonly TableViewInfo tableViewInfo;
		LayoutUnit left;
		LayoutUnit width;
		LayoutUnit textWidth;
		LayoutUnit textLeft;
		TableCell cell;		
		int topAnchorIndex;
		int bottomAnchorIndex;
		LayoutUnit initialContentTop = LayoutUnit.MinValue;
		readonly TableViewInfoCollection innerTables;
		readonly int rowSpan;
		readonly int startRowIndex;
		bool hasContent;
		public TableCellViewInfo(TableViewInfo tableViewInfo, TableCell cell, LayoutUnit left, LayoutUnit width, LayoutUnit textLeft, LayoutUnit textWidth, int topAnchorIndex, int bottomAnchorIndex, int startRowIndex, int rowSpan) {
			Debug.Assert(textWidth >= 0);
			this.tableViewInfo = tableViewInfo;
			this.cell = cell;
			this.width = width;
			this.left = left;
			this.topAnchorIndex = topAnchorIndex;
			this.bottomAnchorIndex = bottomAnchorIndex;
			this.textWidth = textWidth;
			this.textLeft = textLeft;			
			this.innerTables = new TableViewInfoCollection();
			this.rowSpan = rowSpan;
			this.startRowIndex = startRowIndex;
		}
		public int TopAnchorIndex { get { return topAnchorIndex; } }
		public int BottomAnchorIndex { get { return bottomAnchorIndex; } }		
		public bool SingleColumn {
			get {								
				return (!IsStartOnPreviousTableViewInfo() && !IsEndOnNextTableViewInfo());
			}
		}
		public LayoutUnit Left { get { return left; } }
		public LayoutUnit Width { get { return width; } }
		public LayoutUnit TextWidth { get { return textWidth; } }
		public LayoutUnit TextLeft { get { return textLeft; } }
		public TableCell Cell { get { return cell; } }
		public BorderBase LeftBorder { get { return Cell.GetActualLeftCellBorder(); } }
		public BorderBase RightBorder { get { return Cell.GetActualRightCellBorder(); } }
		public BorderBase TopBorder { get { return Cell.GetActualTopCellBorder(); } }
		public BorderBase BottomBorder { get { return GetActualBottomCellBorder(); } }
		public TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public TableViewInfoCollection InnerTables { get { return innerTables; } }
		public bool EmptyCell { get { return hasContent; } set { hasContent = value; } }
		internal LayoutUnit InitialContentTop { get { return initialContentTop; } set { initialContentTop = value; } }
		public int RowSpan { get { return rowSpan; } }
		public int StartRowIndex { get { return startRowIndex; } }
		public void SetBottomAnchorIndexToLastAnchor() {
			this.bottomAnchorIndex = tableViewInfo.Anchors.Count - 1;
		}
		public void SetTopAnchorIndexToLastAnchor() {
			this.topAnchorIndex = 0;
		}
		public void ShiftBottom(int delta) {
			this.bottomAnchorIndex += delta;
		}
		public void ExportTo(IDocumentLayoutExporter exporter) {			
			exporter.ExportTableCell(this);
		}
		protected internal bool IsStartOnPreviousTableViewInfo() {
			if (TableViewInfo.PrevTableViewInfo == null)
				return false;
			if (topAnchorIndex > 0)
				return false;			
			if (StartRowIndex < tableViewInfo.TopRowIndex)
				return true;
			if (StartRowIndex == tableViewInfo.TopRowIndex && tableViewInfo.TopRowIndex == tableViewInfo.PrevTableViewInfo.BottomRowIndex)
				return true;
			return false;
		}
		protected internal bool IsEndOnNextTableViewInfo() {
			if (tableViewInfo.NextTableViewInfo == null)
				return false;
			if (bottomAnchorIndex != tableViewInfo.Anchors.Count - 1)
				return false;
			int endRowIndex = StartRowIndex + RowSpan - 1;
			if (endRowIndex > tableViewInfo.BottomRowIndex)
				return true;
			if (endRowIndex == tableViewInfo.BottomRowIndex && tableViewInfo.BottomRowIndex == tableViewInfo.NextTableViewInfo.TopRowIndex)
				return true;
			return false;
		}
		BorderBase GetActualBottomCellBorder() {
			TableCell cell = GetBottomCell();
			return cell.GetActualBottomCellBorder();
		}
		TableCell GetBottomCell() {
			if (cell.VerticalMerging == MergingState.None)
				return cell;
			int bottomRowIndex = TableViewInfo.TopRowIndex + BottomAnchorIndex - 1;
			int columnIndex = cell.Row.GridBefore;
			int cellIndex = 0;
			while (cell.Row.Cells[cellIndex] != cell) {
				columnIndex += cell.Row.Cells[cellIndex].ColumnSpan;
				cellIndex++;
			}
			cellIndex = 0;
			columnIndex -= cell.Table.Rows[bottomRowIndex].GridBefore;
			while (columnIndex > 0) {
				columnIndex -= cell.Table.Rows[bottomRowIndex].Cells[cellIndex].ColumnSpan;
				cellIndex++;
			}
			Debug.Assert(columnIndex == 0);
			return cell.Table.Rows[bottomRowIndex].Cells[cellIndex];
		}
		internal static RowCollection GetRows(RowCollection rows, TableCell cell) {
			PieceTable pieceTable = cell.PieceTable;
			int firstRowIndex = Algorithms.BinarySearch(rows, new BoxAndLogPositionComparable<Row>(pieceTable, pieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition));
			if (firstRowIndex < 0) {
				firstRowIndex = ~firstRowIndex;
				Debug.Assert(firstRowIndex >= rows.Count || rows[firstRowIndex].Paragraph.Index >= cell.StartParagraphIndex);
			}
			int lastRowIndex = Algorithms.BinarySearch(rows, new BoxAndLogPositionComparable<Row>(pieceTable, pieceTable.Paragraphs[cell.EndParagraphIndex].EndLogPosition));
			if (lastRowIndex < 0) {
				lastRowIndex = (~lastRowIndex) - 1;
				Debug.Assert(lastRowIndex < 0 || rows[lastRowIndex].Paragraph.Index <= cell.EndParagraphIndex);
			}
			RowCollection result = new RowCollection();
			for (int i = firstRowIndex; i <= lastRowIndex; i++) {
				result.Add(rows[i]);
			}
			return result;
		}
		public RowCollection GetRows(Column column) {
			return GetRows(column.Rows, Cell);
		}
		public int GetFirstRowIndex(Column column) {
			PieceTable pieceTable = Cell.PieceTable;
			int firstRowIndex = Algorithms.BinarySearch(column.Rows, new BoxAndLogPositionComparable<Row>(pieceTable, pieceTable.Paragraphs[Cell.StartParagraphIndex].LogPosition));
			if (firstRowIndex < 0) {
				firstRowIndex = ~firstRowIndex;
				if (firstRowIndex >= column.Rows.Count)
					return -1;
				TableCell paragraphCell = column.Rows[firstRowIndex].Paragraph.GetCell();
				while (paragraphCell != null && paragraphCell != cell) {
					paragraphCell = paragraphCell.Table.ParentCell;
				}
				if (paragraphCell != cell)
					return -1;
			}
			return firstRowIndex;
		}
		public Row GetFirstRow(Column column) {
			int index = GetFirstRowIndex(column);
			if (index < 0)
				return null;
			return column.Rows[index];
		}
		public int GetLastRowIndex(Column column) {
			PieceTable pieceTable = Cell.PieceTable;
			int lastRowIndex = Algorithms.BinarySearch(column.Rows, new BoxAndLogPositionComparable<Row>(pieceTable, pieceTable.Paragraphs[Cell.EndParagraphIndex].EndLogPosition));
			if (lastRowIndex < 0) {
				lastRowIndex = (~lastRowIndex) - 1;
				if (lastRowIndex >= 0) {
					Debug.Assert(column.Rows[lastRowIndex].Paragraph.Index <= Cell.EndParagraphIndex);
				}
				else
					return -1;
			}
			return lastRowIndex;
		}
		public Row GetLastRow(Column column) {
			int index = GetLastRowIndex(column);
			if (index < 0)
				return null;
			return column.Rows[index];
		}
		protected internal virtual Rectangle GetBackgroundBounds() {
			Rectangle result = TableViewInfo.GetCellBounds(this);
			int top = TableViewInfo.Anchors[TopAnchorIndex].VerticalPosition;
			result.Height = result.Bottom - top;
			result.Y = top;
			return result;
		}
		protected internal virtual Rectangle GetBounds() {
			Rectangle result = TableViewInfo.GetCellBounds(this);
			return result;
		}
		public TableRowViewInfoBase GetTableRow() {
			int modelRowIndex = Cell.Table.Rows.IndexOf(Cell.Row);
			int layoutRowIndex = modelRowIndex - TableViewInfo.TopRowIndex;
			layoutRowIndex = Math.Max(0, Math.Min(layoutRowIndex, TableViewInfo.Rows.Count - 1));
			Debug.Assert(layoutRowIndex>=0);
			return TableViewInfo.Rows[layoutRowIndex];
		}
		public void AddInnerTable(TableViewInfo tableViewInfo) {
			Debug.Assert(tableViewInfo.Table.ParentCell == Cell);
			InnerTables.Add(tableViewInfo);
		}
		internal void RemoveInnerTable(TableViewInfo tableViewInfo) {
			InnerTables.Remove(tableViewInfo);
		}
	}
	#endregion
	#region TableCellViewInfoCollection
	public class TableCellViewInfoCollection : List<TableCellViewInfo> {
		public TableCellViewInfo Last { get { return Count > 0 ? this[Count - 1] : null; } }
		public TableCellViewInfo First { get { return Count > 0 ? this[0] : null; } }
	}
	#endregion
}
