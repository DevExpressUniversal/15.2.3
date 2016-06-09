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
using System.Drawing;
using System.Threading;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using ModelUnit = System.Int32;
using LayoutUnit = System.Int32;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
	#region TablesControllerState (abstract class)
	public abstract class TablesControllerStateBase {
		readonly TablesController tablesController;
		protected TablesControllerStateBase(TablesController tablesController) {
			Guard.ArgumentNotNull(tablesController, "tablesController");
			this.tablesController = tablesController;
		}
		#region Properties
		public TablesController TablesController {
			[System.Diagnostics.DebuggerStepThrough]
			get { return tablesController; }
		}
		#endregion
		public abstract void EnsureCurrentCell(TableCell cell);
		public abstract void UpdateCurrentCellBottom(LayoutUnit bottom);
		public abstract void EndParagraph(Row lastRow);
		public abstract void BeforeMoveRowToNextColumn();
		public abstract void AfterMoveRowToNextColumn();
		public abstract void UpdateCurrentCellHeight(Row row);
		public abstract CanFitCurrentRowToColumnResult CanFitRowToColumn(LayoutUnit lastTextRowBottom, Column column);
		public abstract void OnCurrentRowFinished();
		public abstract ParagraphIndex RollbackToStartOfRowTableOnFirstCellRowColumnOverfull(bool firstTableRowViewInfoInColumn, bool innerMostTable);
	}
	#endregion
	#region TablesControllerEmptyState
	public class TablesControllerNoTableState : TablesControllerStateBase {
		public TablesControllerNoTableState(TablesController tablesController)
			: base(tablesController) {
		}
		public override void EnsureCurrentCell(TableCell cell) {
			if (cell != null) {
				TablesController.RowsController.ColumnController.PageAreaController.PageController.BeginTableFormatting();
				TablesController.StartNewTable(cell);
			}
		}
		public override void EndParagraph(Row lastRow) {
		}
		public override void BeforeMoveRowToNextColumn() {
		}
		public override void AfterMoveRowToNextColumn() {
		}
		public override void UpdateCurrentCellHeight(Row row) {
		}
		public override void UpdateCurrentCellBottom(LayoutUnit bottom) {
		}
		public override CanFitCurrentRowToColumnResult CanFitRowToColumn(int lastTextRowBottom, Column column) {
			if (TablesController.RowsController.CurrentColumn.Rows.Count <= 0)
				return CanFitCurrentRowToColumnResult.RowFitted;
			int columnBottom = column.Bounds.Bottom;
			return lastTextRowBottom <= columnBottom ? CanFitCurrentRowToColumnResult.RowFitted : CanFitCurrentRowToColumnResult.PlainRowNotFitted;
		}
		public override void OnCurrentRowFinished() {
		}
		public override ParagraphIndex RollbackToStartOfRowTableOnFirstCellRowColumnOverfull(bool firstTableRowViewInfoInColumn, bool innerMostTable) {
			return ParagraphIndex.Zero;
		}
	}
	#endregion
	public class TableCellVerticalAnchorCollection {
		List<TableCellVerticalAnchor> anchors;
		public TableCellVerticalAnchorCollection() {
			this.anchors = new List<TableCellVerticalAnchor>();
		}
		public int Count { get { return anchors.Count; } }
		public TableCellVerticalAnchor this[int index] {
			get { return index < Count ? anchors[index] : null; }
			set {
				EnsureCapacity(index);
				anchors[index] = value;
			}
		}
		public ITableCellVerticalAnchor First { get { return Count > 0 ? this[0] : null; } }
		public ITableCellVerticalAnchor Last { get { return Count > 0 ? this[Count - 1] : null; } }
		protected internal List<TableCellVerticalAnchor> Items { get { return anchors; } }
		void EnsureCapacity(int index) {
			for (int i = Count; i <= index; i++)
				anchors.Add(null);
		}
		public void RemoveAnchors(int startAnchorIndex, int anchorCount) {
			for (int i = startAnchorIndex + anchorCount - 1; i >= startAnchorIndex; i--) {
				if (i < anchors.Count)
					anchors.RemoveAt(i);
			}
		}
		public void ShiftForward(int from, int delta) {
			Guard.ArgumentNonNegative(delta, "delta");
			int initialAnchorCount = anchors.Count;
			for (int i = 0; i < delta; i++)
				anchors.Add(null);
			Debug.Assert(from < initialAnchorCount);
			for (int i = initialAnchorCount - 1; i >= from; i--) {
				anchors[i + delta] = anchors[i];
			}
			for (int i = 0; i < delta; i++)
				anchors[i + from] = null;
		}
		public int GetAnchorIndex(int logicalVerticalPoint) {
			int index = Algorithms.BinarySearch(anchors, new TableCellVerticalAnchorYComparable(logicalVerticalPoint));
			if (index < 0) {
				index = ~index - 1;
				if (index >= anchors.Count)
					return anchors.Count - 1;
			}
			return index;
		}
	}
	public class LayoutGridRectangle {
		Rectangle bounds;
		int columnIndex;
		int columnSpan;
		int rowSpan;
		int rowIndex;
		public LayoutGridRectangle(Rectangle bounds, int rowIndex, int columnIndex, int columnSpan) {
			Guard.ArgumentNonNegative(columnIndex, "columnIndex");
			Guard.ArgumentPositive(columnSpan, "columnSpan");
			this.bounds = bounds;
			this.columnIndex = columnIndex;
			this.columnSpan = columnSpan;
			this.rowIndex = rowIndex;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public int ColumnSpan { get { return columnSpan; } set { columnSpan = value; } }
		public int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		public int RowSpan { get { return rowSpan; } set { rowSpan = value; } }
	}
	#region TableCellLeftComparable
	class TableCellLeftComparable : IComparable<TableCellViewInfo> {
		int pos;
		public TableCellLeftComparable(int pos) {
			this.pos = pos;
		}
		#region IComparable<RulerTickmark> Members
		public int CompareTo(TableCellViewInfo cell) {
			return cell.Left - pos;
		}
		#endregion
	}
	#endregion
	public class TableViewInfoManager {
		#region TableViewInfoAndStartRowSeparatorIndexComparer
		class TableViewInfoAndStartRowSeparatorIndexComparer : IComparable<TableViewInfo> {
			int rowSeparatorIndex;
			public TableViewInfoAndStartRowSeparatorIndexComparer(int rowSeparatorIndex) {
				this.rowSeparatorIndex = rowSeparatorIndex;
			}
			#region IComparable<TableViewInfo> Members
			public int CompareTo(TableViewInfo other) {
				int diff = other.TopRowIndex - rowSeparatorIndex;
				if (diff != 0)
					return diff;
				if (other.PrevTableViewInfo == null)
					return 0;
				else
					return other.PrevTableViewInfo.BottomRowIndex == rowSeparatorIndex ? 1 : 0;
			}
			#endregion
		}
		#endregion
		#region TableViewInfoAndEndRowSeparatorIndexComparer
		class TableViewInfoAndEndRowSeparatorIndexComparer : IComparable<TableViewInfo> {
			int rowSeparatorIndex;
			public TableViewInfoAndEndRowSeparatorIndexComparer(int rowSeparatorIndex) {
				this.rowSeparatorIndex = rowSeparatorIndex;
			}
			#region IComparable<TableViewInfo> Members
			public int CompareTo(TableViewInfo other) {
				int bottomSeparatorIndex = other.BottomRowIndex + 1;
				if (bottomSeparatorIndex < rowSeparatorIndex)
					return -1;
				int topSeparatorIndex = other.TopRowIndex + 1;
				if (topSeparatorIndex > rowSeparatorIndex)
					return 1;
				if (bottomSeparatorIndex == rowSeparatorIndex) {
					if (other.NextTableViewInfo != null && other.NextTableViewInfo.TopRowIndex == other.BottomRowIndex)
						return -1;
					else
						return 0;
				}
				return 0;
			}
			#endregion
		}
		#endregion
		readonly RowsController rowsController;
		readonly PageController pageController;
		readonly TableViewInfoManager parentTableViewInfoManager;
		readonly List<TableViewInfo> tableViewInfos;
		readonly List<int> currentCellBottoms;
		TableCellColumnController columnController;
		TableGrid tableGrid;
		int currentTableViewInfoIndex;
		TableCellViewInfo currentTableCellViewInfo;
		Table table;
		public TableViewInfoManager(TableViewInfoManager parentTableViewInfoManager, PageController pageController, RowsController rowsController) {
			this.tableViewInfos = new List<TableViewInfo>();
			this.currentCellBottoms = new List<LayoutUnit>();
			this.rowsController = rowsController;
			this.pageController = pageController;
			this.parentTableViewInfoManager = parentTableViewInfoManager;
		}
		public TableCellColumnController ColumnController {
			get { return columnController; }
			set {
				if (columnController != null)
					Exceptions.ThrowInternalException();
				columnController = value;
			}
		}
		TableViewInfo CurrentTableViewInfo { get { return tableViewInfos[currentTableViewInfoIndex]; } }
		public int CurrentCellBottom { get { return currentCellBottoms[currentTableViewInfoIndex]; } set { currentCellBottoms[currentTableViewInfoIndex] = value; } }
		TableCellViewInfo CurrentTableCellViewInfo {
			get { return currentTableCellViewInfo; }
			set {
				currentTableCellViewInfo = value;
				ColumnController.SetCurrentTableCellViewInfo(value);				
			}
		}
		RowsController RowsController { get { return rowsController; } }
		public TableViewInfo StartNewTable(Table table, Dictionary<TableCell, LayoutGridRectangle> cellBounds, LayoutUnit leftOffset, LayoutUnit rightCellMargin, bool firstContentInParentCell, TextArea tableTextArea, out LayoutUnit maxRight) {
			int columnWidth = tableTextArea.Width;
			int maxTableWidth = columnWidth - leftOffset + rightCellMargin;
			int percentBaseWidth = columnWidth;
			maxRight = percentBaseWidth;
			if (table.NestedLevel == 0 && !RowsController.MatchHorizontalTableIndentsToTextEdge) {				
				percentBaseWidth += rightCellMargin - leftOffset;
				if (table.TableAlignment == TableRowAlignment.Center)
					maxRight = percentBaseWidth;
			}
			TableWidthsCalculator widthsCalculator = new TableWidthsCalculator(table.PieceTable, RowsController.ColumnController.Measurer, maxTableWidth);
			TableGridCalculator calculator = RowsController.CreateTableGridCalculator(table.DocumentModel, widthsCalculator, maxTableWidth);
			tableGrid = calculator.CalculateTableGrid(table, percentBaseWidth);
			TableViewInfo tableViewInfo = CreateTableViewInfo(table, 0, firstContentInParentCell, null);
			this.currentTableViewInfoIndex = AddTableViewInfo(tableViewInfo);
			this.table = table;
			return CurrentTableViewInfo;
		}
		TableViewInfo CreateTableViewInfo(Table table, int topRowIndex, bool firstContentInParentCell, VerticalBorderPositions verticalBorderPositions) {
			TableCellColumnController currentController = RowsController.ColumnController as TableCellColumnController;
			Column currentTopLevelColumn = currentController != null ? currentController.CurrentTopLevelColumn : RowsController.CurrentColumn;
			TableViewInfo result = new TableViewInfo(table, currentTopLevelColumn, RowsController.CurrentColumn, verticalBorderPositions, topRowIndex, GetParentTableCellViewInfo(), firstContentInParentCell);
			if (parentTableViewInfoManager == null)
				result.AssociatedFloatingObjectsLayout = pageController.FloatingObjectsLayout;
			currentTopLevelColumn.Tables.Add(result);
			if (currentController != null)
				currentController.AddInnerTable(result);
			return result;
		}
		protected virtual TableCellViewInfo GetParentTableCellViewInfo() {
			if (parentTableViewInfoManager == null)
				return null;
			return parentTableViewInfoManager.CurrentTableCellViewInfo;
		}
		TableViewInfo GetStartTableViewInfoByRowSeparatorIndex(int rowSeparatorIndex) {
			int tableViewInfoIndex = GetStartTableViewInfoIndexByRowSeparatorIndex(rowSeparatorIndex);
			return tableViewInfos[tableViewInfoIndex];
		}
		int GetStartTableViewInfoIndexByRowSeparatorIndex(int rowSeparatorIndex) {
			int tableViewInfoIndex = Algorithms.BinarySearch<TableViewInfo>(tableViewInfos, new TableViewInfoAndStartRowSeparatorIndexComparer(rowSeparatorIndex));
			if (tableViewInfoIndex < 0)
				tableViewInfoIndex = (~tableViewInfoIndex) - 1;
#if DEBUGTEST || DEBUG
			TableViewInfo tableViewInfo = tableViewInfos[tableViewInfoIndex];
			int topSeparatorIndex = tableViewInfo.TopRowIndex;
			int bottomSeparatorIndex = tableViewInfo.BottomRowIndex;
			if (tableViewInfoIndex == tableViewInfos.Count - 1)
				Debug.Assert(rowSeparatorIndex >= topSeparatorIndex);
			else
				Debug.Assert(rowSeparatorIndex >= topSeparatorIndex && rowSeparatorIndex <= bottomSeparatorIndex);
			if (rowSeparatorIndex == topSeparatorIndex)
				Debug.Assert(tableViewInfo.PrevTableViewInfo == null || tableViewInfo.PrevTableViewInfo.BottomRowIndex < tableViewInfo.TopRowIndex);
#endif
			return tableViewInfoIndex;
		}
		int GetEndTableViewInfoIndexByRowSeparatorIndex(int rowSeparatorIndex) {
			Debug.Assert(rowSeparatorIndex > 0);
			int tableViewInfoIndex = Algorithms.BinarySearch<TableViewInfo>(tableViewInfos, new TableViewInfoAndEndRowSeparatorIndexComparer(rowSeparatorIndex));
			if (tableViewInfoIndex < 0) {
				Debug.Assert(~tableViewInfoIndex == tableViewInfos.Count);
				tableViewInfoIndex = tableViewInfos.Count - 1;
			}
#if DEBUGTEST || DEBUG
			TableViewInfo tableViewInfo = tableViewInfos[tableViewInfoIndex];
			int topSeparatorIndex = tableViewInfo.TopRowIndex + 1;
			int bottomSeparatorIndex = tableViewInfo.BottomRowIndex + 1;
			if (tableViewInfoIndex == tableViewInfos.Count - 1)
				Debug.Assert(rowSeparatorIndex >= topSeparatorIndex);
			else
				Debug.Assert(rowSeparatorIndex >= topSeparatorIndex && rowSeparatorIndex <= bottomSeparatorIndex);
			if (rowSeparatorIndex == bottomSeparatorIndex)
				Debug.Assert(tableViewInfo.NextTableViewInfo == null || tableViewInfo.NextTableViewInfo.TopRowIndex > tableViewInfo.BottomRowIndex);
#endif
			return tableViewInfoIndex;
		}
		int AddTableViewInfo(TableViewInfo tableViewInfo) {
			int count = tableViewInfos.Count;
			if (count > 0) {
				TableViewInfo lastTableViewInfo = tableViewInfos[count - 1];
				lastTableViewInfo.NextTableViewInfo = tableViewInfo;
				tableViewInfo.PrevTableViewInfo = lastTableViewInfo;
			}
			this.tableViewInfos.Add(tableViewInfo);
			this.currentCellBottoms.Add(0);
			return count;
		}
		public TableGrid GetTableGrid() {
			return tableGrid;
		}
		public List<TableViewInfo> GetTableViewInfos() {
			return tableViewInfos;
		}
		public void SetRowSeparator(int rowSeparatorIndex, TableCellVerticalAnchor anchor) {
			TableViewInfo tableViewInfo = GetStartTableViewInfoByRowSeparatorIndex(rowSeparatorIndex);
			SetRowSeparatorCore(tableViewInfo, rowSeparatorIndex, anchor);
		}
		public void SetRowSeparatorForCurrentTableViewInfo(int rowSeparatorIndex, TableCellVerticalAnchor anchor) {
			int actualRowSeparatorIndex = Math.Min(CurrentTableViewInfo.TopRowIndex + CurrentTableViewInfo.Anchors.Count, rowSeparatorIndex);
			if (actualRowSeparatorIndex - CurrentTableViewInfo.TopRowIndex > CurrentTableViewInfo.Rows.Count)
				return;
			SetRowSeparatorCore(CurrentTableViewInfo, actualRowSeparatorIndex, anchor);
		}
		void SetRowSeparatorCore(TableViewInfo tableViewInfo, int rowSeparatorIndex, TableCellVerticalAnchor anchor) {
			int anchorIndex = rowSeparatorIndex - tableViewInfo.TopRowIndex;
			Debug.Assert(anchorIndex <= tableViewInfo.Rows.Count);
			tableViewInfo.Anchors[anchorIndex] = anchor;
		}
		public void EnsureTableRowViewInfo(TableViewInfo tableViewInfo, TableRow row, int rowIndex) {
			if (rowIndex < tableViewInfo.TopRowIndex + tableViewInfo.Rows.Count)
				return;
			Debug.Assert(rowIndex == tableViewInfo.TopRowIndex + tableViewInfo.Rows.Count);
			tableViewInfo.Rows.Add(CreateTableRowViewInfo(tableViewInfo, row, rowIndex));
		}
		TableRowViewInfoBase CreateTableRowViewInfo(TableViewInfo tableViewInfo, TableRow row, int rowIndex) {
			ModelUnit cellSpacing = TablesControllerTableState.GetActualCellSpacing(row);
			if (cellSpacing != 0)
				return new TableRowViewInfoWithCellSpacing(tableViewInfo, rowIndex, cellSpacing);
			else
				return new TableRowViewInfoNoCellSpacing(tableViewInfo, rowIndex);
		}
		public ITableCellVerticalAnchor GetRowStartAnchor(int rowIndex) {
			TableViewInfo tableViewInfo = GetStartTableViewInfoByRowSeparatorIndex(rowIndex);
			return GetRowStartAnchor(tableViewInfo, rowIndex);
		}
		int GetRowStartAnchorIndex(TableViewInfo tableViewInfo, int rowIndex) {
			return rowIndex - tableViewInfo.TopRowIndex;
		}
		ITableCellVerticalAnchor GetRowStartAnchor(TableViewInfo tableViewInfo, int rowIndex) {
			int anchorIndex = GetRowStartAnchorIndex(tableViewInfo, rowIndex);
			return tableViewInfo.Anchors[anchorIndex];
		}
		public Column GetRowStartColumn(int rowIndex) {
			TableViewInfo tableViewInfo = GetStartTableViewInfoByRowSeparatorIndex(rowIndex);
			return tableViewInfo.Column;
		}
		public TableViewInfo GetRowStartTableViewInfo(TableRow tableRow) {
			int rowIndex = tableRow.Table.Rows.IndexOf(tableRow);
			return GetStartTableViewInfoByRowSeparatorIndex(rowIndex);
		}
		TableCellViewInfo AddFirstCellViewInfo(TableViewInfo tableViewInfo, TableCell cell, Rectangle bounds, LayoutGridRectangle gridBounds, Rectangle textBounds) {
			int topAnchorIndex = GetRowStartAnchorIndex(tableViewInfo, gridBounds.RowIndex);
			int bottomAnchorIndex = topAnchorIndex + gridBounds.RowSpan;
			if (bottomAnchorIndex >= tableViewInfo.Anchors.Count && tableViewInfo.NextTableViewInfo != null)
				bottomAnchorIndex = tableViewInfo.Anchors.Count - 1;
			TableCellViewInfo cellViewInfo = new TableCellViewInfo(tableViewInfo, cell, bounds.Left, bounds.Width, textBounds.Left, textBounds.Width, topAnchorIndex, bottomAnchorIndex, gridBounds.RowIndex, gridBounds.RowSpan);
			tableViewInfo.Cells.Add(cellViewInfo);
			AddTableCellViewInfo(tableViewInfo, cellViewInfo, topAnchorIndex, bottomAnchorIndex - 1);
			return cellViewInfo;
		}
		void AddMiddleCellViewInfo(TableViewInfo tableViewInfo, TableCell cell, Rectangle bounds, LayoutGridRectangle gridBounds, Rectangle textBounds) {
			int topAnchorIndex = 0;
			int bottomAnchorIndex = tableViewInfo.Anchors.Count - 1;
			TableCellViewInfo cellViewInfo = new TableCellViewInfo(tableViewInfo, cell, bounds.Left, bounds.Width, textBounds.Left, textBounds.Width, topAnchorIndex, bottomAnchorIndex, gridBounds.RowIndex, gridBounds.RowSpan);
			tableViewInfo.Cells.Add(cellViewInfo);
			AddTableCellViewInfo(tableViewInfo, cellViewInfo, topAnchorIndex, bottomAnchorIndex - 1);
		}
		void AddLastCellViewInfo(TableViewInfo tableViewInfo, TableCell cell, Rectangle bounds, LayoutGridRectangle gridBounds, Rectangle textBounds) {
			int topAnchorIndex = 0;
			int bottomAnchorIndex = GetRowStartAnchorIndex(tableViewInfo, gridBounds.RowIndex + gridBounds.RowSpan);
			TableCellViewInfo cellViewInfo = new TableCellViewInfo(tableViewInfo, cell, bounds.Left, bounds.Width, textBounds.Left, textBounds.Width, topAnchorIndex, bottomAnchorIndex, gridBounds.RowIndex, gridBounds.RowSpan);
			tableViewInfo.Cells.Add(cellViewInfo);
			AddTableCellViewInfo(tableViewInfo, cellViewInfo, topAnchorIndex, bottomAnchorIndex - 1);
		}
		protected virtual void SetCurrentTableCellViewInfo(TableCellViewInfo tableCellViewInfo) {
			Guard.ArgumentNotNull(tableCellViewInfo, "tableCellViewInfo");
			currentTableCellViewInfo = tableCellViewInfo;
			currentTableViewInfoIndex = tableViewInfos.IndexOf(currentTableCellViewInfo.TableViewInfo);
			if (parentTableViewInfoManager != null)
				parentTableViewInfoManager.SetCurrentTableCellViewInfo(tableCellViewInfo.TableViewInfo.ParentTableCellViewInfo);
			else
				EnsureFloatingObjectsLayoutValid(tableCellViewInfo.TableViewInfo);
			Debug.Assert(currentTableViewInfoIndex >= 0);
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected internal virtual void ValidateTopLevelColumn() {
#if DEBUG
			TableViewInfoCollection tables = columnController.CurrentTopLevelColumn.InnerTables;
			Debug.Assert(tables != null && tables.IndexOf(CurrentTableViewInfo) >= 0);
			Debug.Assert(CurrentTableCellViewInfo.TableViewInfo == CurrentTableViewInfo);
#endif
			if (parentTableViewInfoManager != null)
				parentTableViewInfoManager.ValidateTopLevelColumn();
		}
		protected virtual void SetCurrentParentColumn(TableViewInfo tableViewInfo) {
			columnController.SetCurrentParentColumn(tableViewInfo.Column);
			if (parentTableViewInfoManager != null)
				parentTableViewInfoManager.SetCurrentParentColumn(tableViewInfo.ParentTableCellViewInfo.TableViewInfo);
			else
				EnsureFloatingObjectsLayoutValid(tableViewInfo);
		}
		protected virtual void EnsureFloatingObjectsLayoutValid(TableViewInfo tableViewInfo) {
			Debug.Assert(parentTableViewInfoManager == null);
			if(tableViewInfo.AssociatedFloatingObjectsLayout != null)
				pageController.SetFloatingObjectsLayout(tableViewInfo.AssociatedFloatingObjectsLayout);
		}
		public void BeforeStartNextCell(TableCell cell, LayoutGridRectangle gridBounds) {
			int startTableViewInfoIndex = GetStartTableViewInfoIndexByRowSeparatorIndex(gridBounds.RowIndex);
			if (startTableViewInfoIndex != currentTableViewInfoIndex) {
				currentTableViewInfoIndex = startTableViewInfoIndex;
				if (parentTableViewInfoManager != null) {
					Debug.Assert(CurrentTableViewInfo.ParentTableCellViewInfo != null);
					parentTableViewInfoManager.SetCurrentTableCellViewInfo(CurrentTableViewInfo.ParentTableCellViewInfo);
				}
				SetCurrentParentColumn(CurrentTableViewInfo);
			}			
		}
		public void StartNextCell(TableCell cell, Rectangle bounds, LayoutGridRectangle gridBounds, Rectangle textBounds) {
			int startTableViewInfoIndex = GetStartTableViewInfoIndexByRowSeparatorIndex(gridBounds.RowIndex);
			int lastTableViewInfoIndex = GetEndTableViewInfoIndexByRowSeparatorIndex(gridBounds.RowIndex + gridBounds.RowSpan);
			CurrentTableCellViewInfo = AddFirstCellViewInfo(tableViewInfos[startTableViewInfoIndex], cell, bounds, gridBounds, textBounds);
			for (int tableViewInfoIndex = startTableViewInfoIndex + 1; tableViewInfoIndex < lastTableViewInfoIndex; tableViewInfoIndex++) {
				AddMiddleCellViewInfo(tableViewInfos[tableViewInfoIndex], cell, bounds, gridBounds, textBounds);
			}
			if (lastTableViewInfoIndex > startTableViewInfoIndex)
				AddLastCellViewInfo(tableViewInfos[lastTableViewInfoIndex], cell, bounds, gridBounds, textBounds);			
			ValidateTopLevelColumn();
			if (cell.Row.CellSpacing.Type == WidthUnitType.ModelUnits && cell.Row.CellSpacing.Value > 0) {
			}
		}
		TableViewInfo GetLastTableViewInfo() {
			return tableViewInfos[tableViewInfos.Count - 1];
		}
		void AddTableCellViewInfo(TableViewInfo tableViewInfo, TableCellViewInfo cellViewInfo, int startRowViewInfoIndex, int endRowViewInfoIndex) {
			for (int rowViewInfoIndex = startRowViewInfoIndex; rowViewInfoIndex <= endRowViewInfoIndex; rowViewInfoIndex++) {
				int rowIndex = tableViewInfo.TopRowIndex + rowViewInfoIndex;
				EnsureTableRowViewInfo(tableViewInfo, tableViewInfo.Table.Rows[rowIndex], rowIndex);
				int cellIndex = Algorithms.BinarySearch(tableViewInfo.Rows[rowViewInfoIndex].Cells, new TableCellLeftComparable(cellViewInfo.Left));
				Debug.Assert(cellIndex < 0);
				cellIndex = ~cellIndex;
				tableViewInfo.Rows[rowViewInfoIndex].Cells.Insert(cellIndex, cellViewInfo);
			}
		}
		public int GetLastTableBottom() {
			TableViewInfo lastTableViewInfo = GetLastTableViewInfo();
			return lastTableViewInfo.GetTableBottom();
		}
		public TableCellVerticalAnchor GetBottomCellAnchor(TableCell CurrentCell) {
			int bottomAnchorIndex = CurrentTableCellViewInfo.BottomAnchorIndex;
			Debug.Assert(CurrentCell == CurrentTableCellViewInfo.Cell);
			return CurrentTableViewInfo.Anchors[bottomAnchorIndex];
		}
		public int GetBottomCellRowSeparatorIndex(TableCell CurrentCell, int rowSpan) {
			return CurrentCell.Table.Rows.IndexOf(CurrentCell.Row) + rowSpan;
		}
		public void BeforeMoveRowToNextColumn(LayoutUnit currentCellBottom) {
		}
		protected TableCellViewInfo FindSuitableTableCellViewInfo(TableCellViewInfoCollection cells, TableCell cell) {
			for (int i = cells.Count - 1; i >= 0; i--) {
				TableCellViewInfo cellViewInfo = cells[i];
				if (cellViewInfo.Cell == cell)
					return cellViewInfo;
			}
			return null;
		}
		public void AfterMoveRowToNextColumn() {
			LayoutUnit currentCellBottom = CurrentCellBottom;
			TableCellColumnController columnController = ColumnController;
			if (columnController.ViewInfo != null) {
				TableViewInfo prevTableViewInfo = CurrentTableViewInfo;
				currentTableViewInfoIndex = tableViewInfos.IndexOf(columnController.ViewInfo);
				TableCellViewInfo suitableTableCellViewInfo = FindSuitableTableCellViewInfo(CurrentTableViewInfo.Cells, CurrentTableCellViewInfo.Cell);
				if(parentTableViewInfoManager == null)
					EnsureFloatingObjectsLayoutValid(tableViewInfos[currentTableViewInfoIndex]);
				if (suitableTableCellViewInfo != null) {
					CurrentTableCellViewInfo = suitableTableCellViewInfo;
					int count = prevTableViewInfo.Anchors.Count;
					prevTableViewInfo.Anchors[count - 1].SetVerticalPositionInternal(Math.Max(prevTableViewInfo.BottomAnchor.VerticalPosition, currentCellBottom));
				}
				else {
					int bottomAnchorIndex = CurrentTableCellViewInfo.BottomAnchorIndex;
					MoveRowAndAnchors_(prevTableViewInfo, CurrentTableViewInfo, bottomAnchorIndex);
					TableCellViewInfoCollection cells = prevTableViewInfo.Rows.Last.Cells;
					int currentCellViewInfoIndex = Algorithms.BinarySearch(cells, new TableCellLeftComparable(CurrentTableCellViewInfo.Left));
					CurrentTableCellViewInfo = CurrentTableViewInfo.Rows.First.Cells[currentCellViewInfoIndex];
				}
			}
			else {
				int rowViewInfoIndex = CurrentTableCellViewInfo.BottomAnchorIndex - 1;
				TableCellViewInfoCollection cells = CurrentTableViewInfo.Rows[CurrentTableCellViewInfo.BottomAnchorIndex - 1].Cells;
				int currentCellViewInfoIndex = Algorithms.BinarySearch(cells, new TableCellLeftComparable(CurrentTableCellViewInfo.Left));
				Debug.Assert(currentCellViewInfoIndex >= 0);
				TableViewInfo newTableViewInfo = new TableViewInfo(table, columnController.CurrentTopLevelColumn, columnController.ParentColumn, CurrentTableViewInfo.VerticalBorderPositions, CurrentTableViewInfo.TopRowIndex + CurrentTableCellViewInfo.BottomAnchorIndex - 1, GetParentTableCellViewInfo(), true);
				if (parentTableViewInfoManager == null)
					newTableViewInfo.AssociatedFloatingObjectsLayout = pageController.FloatingObjectsLayout;
				columnController.Parent.AddInnerTable(newTableViewInfo);
				newTableViewInfo.LeftOffset = CurrentTableViewInfo.LeftOffset;
				newTableViewInfo.TextAreaOffset = CurrentTableViewInfo.LeftOffset;
				newTableViewInfo.ModelRelativeIndent = CurrentTableViewInfo.ModelRelativeIndent;
				int newTableViewInfoIndex = AddTableViewInfo(newTableViewInfo);
				columnController.CurrentTopLevelColumn.Tables.Add(newTableViewInfo);
				columnController.AddTableViewInfo(newTableViewInfo);
				MoveRowAndAnchor(CurrentTableViewInfo, newTableViewInfo, rowViewInfoIndex + 1);
				int splitAnchorIndex = CurrentTableCellViewInfo.BottomAnchorIndex;
				TableCellVerticalAnchor splitAnchor = CurrentTableViewInfo.Anchors[splitAnchorIndex - 1];
				List<HorizontalCellBordersInfo> borders = GetSplitAnchorHorizontalCellBorders(splitAnchor);
				CurrentTableViewInfo.Anchors[splitAnchorIndex] = new TableCellVerticalAnchor(currentCellBottom, 0, borders);
				newTableViewInfo.Anchors[0] = new TableCellVerticalAnchor(newTableViewInfo.Column.Bounds.Top, splitAnchor != null ? splitAnchor.BottomTextIndent : 0, borders);
				SplitCellsByAncor(CurrentTableViewInfo, newTableViewInfo, splitAnchorIndex);
				Debug.Assert(CurrentTableCellViewInfo.BottomAnchorIndex - splitAnchorIndex + 1 == 1);
				Debug.Assert(newTableViewInfo.Rows.First.Cells[currentCellViewInfoIndex].Cell == CurrentTableCellViewInfo.Cell);
				CurrentTableCellViewInfo = newTableViewInfo.Rows.First.Cells[currentCellViewInfoIndex];
				currentTableViewInfoIndex = newTableViewInfoIndex;
			}
			Row currentRow = RowsController.CurrentRow;
			Rectangle rowBounds = currentRow.Bounds;
			rowBounds.Y = CurrentTableViewInfo.TopAnchor.VerticalPosition + CurrentTableViewInfo.TopAnchor.BottomTextIndent + currentRow.SpacingBefore;
			RowsController.CurrentRow.Bounds = rowBounds;
			RowsController.HorizontalPositionController.SetCurrentHorizontalPosition(rowBounds.X);
			ValidateTopLevelColumn();
		}
		public int GetCurrentCellTop() {
			return CurrentTableViewInfo.Anchors[CurrentTableCellViewInfo.TopAnchorIndex].VerticalPosition;
		}
		public int GetCurrentCellTopAnchorIndex() {
			return CurrentTableCellViewInfo.TopAnchorIndex;
		}
		public int GetCurrentCellBottomAnchorIndex() {
			return CurrentTableCellViewInfo.BottomAnchorIndex;
		}
		void MoveRowAndAnchors_(TableViewInfo source, TableViewInfo target, int startAnchorIndex) {
			Debug.Assert(startAnchorIndex > 0);
			int initialTargetTopRowIndex = target.TopRowIndex;
			int initialSourceBottomRowIndex = source.BottomRowIndex;
			int sourceRowCount = source.Rows.Count;
			int movedRowCount = sourceRowCount - startAnchorIndex;
			if (target.TopRowIndex == source.BottomRowIndex)
				movedRowCount--;
			target.Rows.ShiftForward(movedRowCount + 1);
			for (int i = 0; i < movedRowCount; i++) {
				int rowViewInfoIndex = startAnchorIndex + i;
				int rowIndex = source.TopRowIndex + rowViewInfoIndex;
				TableRow row = source.Rows[rowViewInfoIndex].Row;
				target.Rows[i + 1] = CreateTableRowViewInfo(target, row, rowIndex);
			}
			target.Rows[0] = CreateTableRowViewInfo(target, source.Rows[startAnchorIndex - 1].Row, source.TopRowIndex + startAnchorIndex - 1);
			source.Rows.RemoveRows(startAnchorIndex, sourceRowCount - startAnchorIndex);
			int sourceAnchorsCount = source.Anchors.Count;
			int lastMovedAnchorIndex = initialSourceBottomRowIndex == initialTargetTopRowIndex ? sourceAnchorsCount - 2 : sourceAnchorsCount - 1;
			int movedAnchorCount = lastMovedAnchorIndex - startAnchorIndex + 1;
			bool keepFirstAnchorInTarget = initialSourceBottomRowIndex == initialTargetTopRowIndex;
			target.Anchors.ShiftForward(keepFirstAnchorInTarget ? 1 : 0, movedAnchorCount);
			if (!keepFirstAnchorInTarget)
				Exceptions.ThrowInternalException();
			for (int anchorIndex = startAnchorIndex; anchorIndex <= lastMovedAnchorIndex; anchorIndex++) {
				if (source.Anchors[anchorIndex] != null) {
					TableCellVerticalAnchor sourceAnchor = source.Anchors[anchorIndex];
					target.Anchors[anchorIndex - startAnchorIndex + 1] = sourceAnchor.CloneWithNewVerticalPosition(0); ;
				}
			}
			source.Anchors.RemoveAnchors(startAnchorIndex, movedAnchorCount);
			target.SetTopRowIndex(source.BottomRowIndex);
			TableRowViewInfoBase topTargetRowViewInfo = target.Rows[movedRowCount + 1];
			Debug.Assert(topTargetRowViewInfo != null);
			TableCellViewInfoCollection targetCells = topTargetRowViewInfo.Cells;
			int targetCellCount = targetCells.Count;
			for (int i = 0; i < targetCellCount; i++) {
				AddTableCellViewInfo(target, targetCells[i], 0, movedRowCount);
				targetCells[i].ShiftBottom(movedAnchorCount);
				targetCells[i].SetTopAnchorIndexToLastAnchor();
			}
			TableRowViewInfoBase sourceRowViewInfo = source.Rows[startAnchorIndex - 1];
			TableCellViewInfoCollection sourceCells = sourceRowViewInfo.Cells;
			int sourceCellCount = sourceCells.Count;
			for (int i = 0; i < sourceCellCount; i++) {
				TableCellViewInfo cellViewInfo = sourceCells[i];
				CellAction action = ShouldSplitCell(source, target, cellViewInfo, startAnchorIndex, sourceAnchorsCount, initialSourceBottomRowIndex, initialTargetTopRowIndex);
				switch (action) {
					case CellAction.Split:
						SplitCellByAnchor(source, target, startAnchorIndex, cellViewInfo);
						break;
					case CellAction.SetBottomIndex:
						cellViewInfo.SetBottomAnchorIndexToLastAnchor();
						break;
				}
			}
		}
		enum CellAction {
			None,
			Split,
			SetBottomIndex
		}
		CellAction ShouldSplitCell(TableViewInfo source, TableViewInfo target, TableCellViewInfo cellViewInfo, int splitAnchorIndex, int sourceAnchorsCount, int initialSourceBottomRowIndex, int initialTargetTopRowIndex) {
			int bottomAnchorIndex = cellViewInfo.BottomAnchorIndex;
			if (bottomAnchorIndex < splitAnchorIndex)
				return CellAction.None;
			if (bottomAnchorIndex < sourceAnchorsCount - 1)
				return CellAction.Split;
			Debug.Assert(bottomAnchorIndex == sourceAnchorsCount - 1);
			if (initialTargetTopRowIndex == initialSourceBottomRowIndex)
				return CellAction.SetBottomIndex;
			Exceptions.ThrowInternalException();
			return CellAction.None;
		}
		void MoveRowAndAnchor(TableViewInfo from, TableViewInfo to, int startAnchorIndex) {
			int rowCount = from.Rows.Count;
			for (int rowIndex = startAnchorIndex - 1; rowIndex < rowCount; rowIndex++) {
				TableRow row = from.Rows[rowIndex].Row;
				EnsureTableRowViewInfo(to, row, from.TopRowIndex + rowIndex);
			}
			int anchorCount = from.Anchors.Count;
			for (int anchorIndex = startAnchorIndex; anchorIndex < anchorCount; anchorIndex++) {
				if (from.Anchors[anchorIndex] != null) {
					TableCellVerticalAnchor sourceAnchor = from.Anchors[anchorIndex];
					to.Anchors[anchorIndex - startAnchorIndex + 1] = sourceAnchor.CloneWithNewVerticalPosition(0);
				}
			}
			from.Rows.RemoveRows(startAnchorIndex, rowCount - startAnchorIndex);
			from.Anchors.RemoveAnchors(startAnchorIndex, anchorCount - startAnchorIndex);
		}
		void SplitCellsByAncor(TableViewInfo currentTableViewInfo, TableViewInfo nextTableViewInfo, int anchorIndex) {
			Debug.Assert(anchorIndex > 0);
			int rowViewInfoIndex = anchorIndex - 1;
			TableRowViewInfoBase rowViewInfo = currentTableViewInfo.Rows[rowViewInfoIndex];
			int cellCount = rowViewInfo.Cells.Count;
			for (int i = 0; i < cellCount; i++)
				SplitCellByAnchor(currentTableViewInfo, nextTableViewInfo, anchorIndex, rowViewInfo.Cells[i]);
		}
		void SplitCellByAnchor(TableViewInfo currentTableViewInfo, TableViewInfo nextTableViewInfo, int anchorIndex, TableCellViewInfo tableCellViewInfo) {
			int newBottomAnchorIndex = tableCellViewInfo.BottomAnchorIndex - anchorIndex + 1;
			int relativeLeft = tableCellViewInfo.Left - currentTableViewInfo.Column.Bounds.Left;
			TableCellViewInfo nextCellViewInfo = new TableCellViewInfo(nextTableViewInfo, tableCellViewInfo.Cell, relativeLeft + nextTableViewInfo.Column.Bounds.Left, tableCellViewInfo.Width, tableCellViewInfo.TextLeft, tableCellViewInfo.TextWidth, 0, newBottomAnchorIndex, tableCellViewInfo.StartRowIndex, tableCellViewInfo.RowSpan);
			nextTableViewInfo.Cells.Add(nextCellViewInfo);
			tableCellViewInfo.SetBottomAnchorIndexToLastAnchor();
			AddTableCellViewInfo(nextTableViewInfo, nextCellViewInfo, 0, newBottomAnchorIndex - 1);
		}
		public virtual void LeaveCurrentTable(bool beforeRestart) {
			if (!beforeRestart) {
				int tableViewInfosCount = this.tableViewInfos.Count;
				Debug.Assert(tableViewInfosCount > 0);
				TableViewInfo lastTableViewInfo = tableViewInfos[tableViewInfosCount - 1];
				if (parentTableViewInfoManager != null) {
					parentTableViewInfoManager.SetCurrentTableCellViewInfo(lastTableViewInfo.ParentTableCellViewInfo);
					parentTableViewInfoManager.SetCurrentParentColumn(lastTableViewInfo.ParentTableCellViewInfo.TableViewInfo);
					parentTableViewInfoManager.ValidateTopLevelColumn();
				}
			}
		}
#if DEBUGTEST || DEBUG
		public virtual void ValidateTableViewInfos() {
			int count = tableViewInfos.Count;
			for (int i = 0; i < count; i++)
				ValidateTableViewInfo(tableViewInfos[i]);
		}
		protected virtual void ValidateTableViewInfo(TableViewInfo tableViewInfo) {
			TableCellViewInfoCollection cells = tableViewInfo.Cells;
			int anchorCount = tableViewInfo.Anchors.Count;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				TableCellViewInfo cell = cells[i];
				if (cell.TopAnchorIndex < 0 | cell.TopAnchorIndex >= anchorCount)
					Exceptions.ThrowInternalException();
				if (cell.BottomAnchorIndex < 0 || cell.BottomAnchorIndex >= anchorCount)
					Exceptions.ThrowInternalException();
			}
		}
#endif
		public void RemoveAllInvalidRowsOnColumnOverfull(int firstInvalidRowIndex) {
			int lastValidTableViewInfoIndex = GetStartTableViewInfoIndexByRowSeparatorIndex(firstInvalidRowIndex);
			TableViewInfo lastValidTableViewInfo = tableViewInfos[lastValidTableViewInfoIndex];
			int firstInvalidRowViewInfoIndex = firstInvalidRowIndex - lastValidTableViewInfo.TopRowIndex;
			if (firstInvalidRowViewInfoIndex == 0)
				lastValidTableViewInfoIndex--;
			for (int i = tableViewInfos.Count - 1; i > lastValidTableViewInfoIndex; i--) {
				TableViewInfo tableViewInfo = tableViewInfos[i];
				tableViewInfo.Column.TopLevelColumn.RemoveTableViewInfoWithContent(tableViewInfo);
				tableViewInfos.RemoveAt(i);
				columnController.RemoveTableViewInfo(tableViewInfo);
				if (tableViewInfo.ParentTableCellViewInfo != null) {
					tableViewInfo.ParentTableCellViewInfo.InnerTables.Remove(tableViewInfo);
				}
				bool shouldRemoveColumn = ShouldRemoveColumn(i, tableViewInfo);
				if (shouldRemoveColumn)
					columnController.RemoveGeneratedColumn(tableViewInfo.Column);
			}
			if (firstInvalidRowViewInfoIndex > 0) {
				lastValidTableViewInfo.RemoveRowsFromIndex(firstInvalidRowViewInfoIndex);
				lastValidTableViewInfo.NextTableViewInfo = null;
			}
		}
		public virtual void FixColumnOverflow() {
			if (parentTableViewInfoManager != null)
				return;
			int count = tableViewInfos.Count;
			for (int i = 0; i < count; i++) {
				TableViewInfo tableViewInfo = tableViewInfos[i];
				TableCellVerticalAnchorCollection verticalAnchors = tableViewInfo.Anchors;
				int anchorCount = tableViewInfo.Anchors.Count;
				if (anchorCount == 0)
					continue;
				TableCellVerticalAnchor lastAnchor = verticalAnchors[anchorCount - 1];
				if (lastAnchor.VerticalPosition > tableViewInfo.Column.Bounds.Bottom)
					lastAnchor.SetVerticalPositionInternal(tableViewInfo.Column.Bounds.Bottom);
			}
		}
		bool ShouldRemoveColumn(int tableViewInfoIndex, TableViewInfo tableViewInfo) {
			if (tableViewInfoIndex > 0)
				return true;
			return tableViewInfo.FirstContentInParentCell;
		}
		public bool IsCurrentCellViewInfoFirst() {
			return !CurrentTableCellViewInfo.IsStartOnPreviousTableViewInfo();
		}
		public bool IsFirstTableRowViewInfoInColumn() {
			if (CurrentTableCellViewInfo.TopAnchorIndex > 0)
				return false;
			if (!CurrentTableViewInfo.FirstContentInParentCell)
				return false;
			if (parentTableViewInfoManager == null)
				return true;
			else
				return parentTableViewInfoManager.IsFirstTableRowViewInfoInColumn();
		}
		public void SetCurrentCellHasContent() {
			CurrentTableCellViewInfo.EmptyCell = true;
		}
		public int GetCurrentTableViewInfoIndex() {
			return currentTableViewInfoIndex;
		}
		public int GetCurrentTopLevelTableViewInfoIndex() {
			if (parentTableViewInfoManager != null)
				return parentTableViewInfoManager.GetCurrentTopLevelTableViewInfoIndex();
			else
				return GetCurrentTableViewInfoIndex();
		}
		public bool IsFirstContentInParentCell() {
			return CurrentTableViewInfo.FirstContentInParentCell;
		}
		public TableViewInfoManager GetParentTableViewInfoManager() {
			return parentTableViewInfoManager;
		}
		internal TableViewInfoManager GetTopLevelTableViewInfoManager() {
			return parentTableViewInfoManager != null ? parentTableViewInfoManager.GetTopLevelTableViewInfoManager() : this;
		}
		protected internal virtual List<HorizontalCellBordersInfo> GetSplitAnchorHorizontalCellBorders(TableCellVerticalAnchor splitAnchor) {
			return splitAnchor != null ? splitAnchor.CellBorders : null;
		}
	}
	public enum RestartFrom {
		NoRestart,
		CellStart,
		TableStart
	}
	#region TablesControllerTableState
	public class TablesControllerTableState : TablesControllerStateBase {
		#region Fields
		readonly TableViewInfoManager tableViewInfoManager;
		readonly Dictionary<TableCell, LayoutGridRectangle> cellsBounds;
		readonly TableBorderCalculator borderCalculator;
		readonly DocumentModelUnitToLayoutUnitConverter unitConverter;
		readonly TableCellVerticalBorderCalculator verticalBordersCalculator;
		readonly TableCellHorizontalBorderCalculator horizontalBordersCalculator;
		TableRow currentRow;
		TableCell currentCell;
		int currentCellRowCount;
		RestartFrom restartFrom;
		Rectangle rowBoundsBeforeTableStart;
		LayoutUnit maxTableWidth;
		LayoutUnit tableRight;
		#endregion
		public TablesControllerTableState(TablesController tablesController, TableCell startCell, bool firstContentInParentCell)
			: base(tablesController) {
			Guard.ArgumentNotNull(startCell, "startCell");
			this.currentCell = startCell;
			this.borderCalculator = new TableBorderCalculator();
			this.unitConverter = startCell.DocumentModel.ToDocumentLayoutUnitConverter;
			TablesControllerTableState tableControllerParentState = tablesController.State as TablesControllerTableState;
			this.tableViewInfoManager = tablesController.CreateTableViewInfoManager(tableControllerParentState != null ? tableControllerParentState.TableViewInfoManager : null, TablesController.PageController, TablesController.RowsController);
			this.cellsBounds = new Dictionary<TableCell, LayoutGridRectangle>();
			Table table = CurrentCell.Table;
			this.verticalBordersCalculator = new TableCellVerticalBorderCalculator(table);
			this.horizontalBordersCalculator = new TableCellHorizontalBorderCalculator(table);
			PrepareGridCellBounds();
			StartNewTable(table, firstContentInParentCell);
		}
		#region Properties
		TableCell CurrentCell { get { return currentCell; } }
		TableRow CurrentRow { get { return currentRow; } }
		Table CurrentTable { get { return CurrentCell != null ? CurrentCell.Table : null; } }
		RowsController RowsController { get { return TablesController.RowsController; } }
		DocumentModelUnitToLayoutUnitConverter UnitConverter { get { return unitConverter; } }
		TableCellVerticalBorderCalculator VerticalBordersCalculator { get { return verticalBordersCalculator; } }
		public TableViewInfoManager TableViewInfoManager { get { return tableViewInfoManager; } }
		public bool CurrentCellViewInfoEmpty { get { return currentCellRowCount == 0; } }
		#endregion
		public override CanFitCurrentRowToColumnResult CanFitRowToColumn(int lastTextRowBottom, Column column) {
			int specialBottomAnchorIndex;
			bool infinityHeight = TablesController.IsInfinityHeight(TableViewInfoManager.GetCurrentTopLevelTableViewInfoIndex(), out specialBottomAnchorIndex);
			if (infinityHeight) {
				if (CurrentTable.NestedLevel > 0)
					return CanFitCurrentRowToColumnResult.RowFitted;
				if (TableViewInfoManager.GetCurrentCellTopAnchorIndex() >= specialBottomAnchorIndex) {
					TableViewInfoManager.SetCurrentCellHasContent();
					return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
				}
				else
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
			if (currentCellRowCount == 0) {
				TableBreakType breakType = TableBreakType.NoBreak;
				int bottomBounds = column.Bounds.Bottom;
				if (!IsFirstTableRowViewInfoInColumn()) {
					TableCell cell = CurrentCell;
					TableViewInfoManager currentTableViewInfoManager = tableViewInfoManager;
					while (cell != null) {
						Table table = cell.Table;
						breakType = this.TablesController.GetTableBreak(table, tableViewInfoManager.GetCurrentTableViewInfoIndex(), out bottomBounds);
						if (cell.VerticalMerging == MergingState.Restart) {
							List<TableCell> cells = cell.GetVerticalSpanCells();
							if (cells.Count > 1) {
								if (breakType == TableBreakType.NoBreak || tableViewInfoManager.GetCurrentCellTop() != bottomBounds)
									return CanFitCurrentRowToColumnResult.RowFitted;
							}
						}
						if (breakType != TableBreakType.NoBreak)
							break;
						if (currentTableViewInfoManager.GetCurrentCellTopAnchorIndex() > 0)
							break;
						if (!currentTableViewInfoManager.IsCurrentCellViewInfoFirst())
							break;
						if (!currentTableViewInfoManager.IsFirstContentInParentCell())
							break;
						currentTableViewInfoManager = tableViewInfoManager.GetParentTableViewInfoManager();
						cell = cell.Table.ParentCell;
					}
				}
				else
					breakType = TableBreakType.NoBreak;
				if (breakType == TableBreakType.NextPage) {
					if (lastTextRowBottom > bottomBounds) {
						tableViewInfoManager.SetCurrentCellHasContent();
						return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
					}
					else
						return CanFitCurrentRowToColumnResult.RowFitted;
				}
				if (lastTextRowBottom > column.Bounds.Bottom)
					return CanFitCurrentRowToColumnResult.FirstCellRowNotFitted;
				else
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
			else {
				int bottomBounds = column.Bounds.Bottom;
				TableBreakType breakType = this.TablesController.GetTableBreak(CurrentTable, tableViewInfoManager.GetCurrentTableViewInfoIndex(), out bottomBounds);
				if (breakType == TableBreakType.NoBreak)
					bottomBounds = column.Bounds.Bottom;
				if (lastTextRowBottom > bottomBounds)
					return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
				else
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
		}
		public bool IsFirstTableRowViewInfoInColumn() {
			return tableViewInfoManager.IsFirstTableRowViewInfoInColumn();
		}
		void StartNewTable(Table table, bool firstContentInParentCell) {
			ModelUnit modelRelativeIndent = CalculateTableActualLeftRelativeOffset(table);
			ModelUnit actualTableIndent = table.TableIndent.Type == WidthUnitType.ModelUnits && table.TableAlignment == TableRowAlignment.Left ? table.TableIndent.Value : 0;
			ModelUnit modelLeftOffset = actualTableIndent + modelRelativeIndent;
			LayoutUnit layoutLeftOffset = UnitConverter.ToLayoutUnits(modelLeftOffset);
			ModelUnit rightCellMargin = GetActualCellRightMargin(table.Rows.First.Cells.First);
			LayoutUnit layoutRightTableBorder = 0;
			if (RowsController.MatchHorizontalTableIndentsToTextEdge) {
				LayoutUnit rightTableBorder = VerticalBordersCalculator.GetRightBorderWidth(borderCalculator, table.FirstRow.LastCell);
				layoutRightTableBorder = UnitConverter.ToLayoutUnits(rightTableBorder);
			}
			LayoutUnit layoutRightCellMaring = UnitConverter.ToLayoutUnits(rightCellMargin);
			int tableTop = RowsController.CurrentRow.Bounds.Top;
			TableViewInfo tableViewInfo;
			if (table.TableLayout == TableLayoutType.Autofit && (table.PreferredWidth.Type == WidthUnitType.Auto || table.PreferredWidth.Type == WidthUnitType.Nil)) {
				LayoutUnit minTableWidth = 1;
				tableTop = RowsController.MoveCurrentRowDownToFitTable(minTableWidth, tableTop);
				TextArea tableTextArea = RowsController.GetTextAreaForTable();
				tableViewInfo = TableViewInfoManager.StartNewTable(table, cellsBounds, layoutLeftOffset, layoutRightCellMaring, firstContentInParentCell, tableTextArea, out maxTableWidth);
				tableViewInfo.TextAreaOffset = tableTextArea.Start - RowsController.CurrentColumn.Bounds.Left;
				layoutLeftOffset += tableViewInfo.TextAreaOffset;
			}
			else {
				Rectangle columnBounds = RowsController.CurrentColumn.Bounds;
				TextArea tableTextArea = new TextArea(columnBounds.Left, columnBounds.Right - layoutRightTableBorder);
				tableViewInfo = TableViewInfoManager.StartNewTable(table, cellsBounds, layoutLeftOffset, layoutRightCellMaring, firstContentInParentCell, tableTextArea, out maxTableWidth);
				tableTop = RowsController.MoveCurrentRowDownToFitTable(columnBounds.Width, tableTop);
				tableTextArea = RowsController.GetTextAreaForTable();
				tableViewInfo.TextAreaOffset = tableTextArea.Start - RowsController.CurrentColumn.Bounds.Left;
				layoutLeftOffset += tableViewInfo.TextAreaOffset;
			}
			tableViewInfo.VerticalBorderPositions = PrepareCellsBounds(layoutLeftOffset);
			tableViewInfo.LeftOffset = layoutLeftOffset;
			tableViewInfo.ModelRelativeIndent = modelRelativeIndent;
			EnsureCurrentTableRow(CurrentCell.Row);
			List<HorizontalCellBordersInfo> cellBordersInfo;
			LayoutUnit bottomTextIndent = CalculateBottomTextIndent(0, out cellBordersInfo);
			this.rowBoundsBeforeTableStart = RowsController.CurrentRow.Bounds;
			TableViewInfoManager.SetRowSeparator(0, new TableCellVerticalAnchor(tableTop, bottomTextIndent, cellBordersInfo));
			Rectangle bounds = GetCellBounds(CurrentCell);
			Rectangle cellBounds = bounds;
			cellBounds.Offset(RowsController.CurrentColumn.Bounds.X, 0);
			Rectangle textBounds = GetTextBounds(CurrentCell, bounds);
			TableCellColumnController tableCellController = new TableCellColumnController(RowsController.ColumnController, RowsController.CurrentColumn, textBounds.Left, textBounds.Top, textBounds.Width, tableViewInfo, CurrentCell);
			RowsController.SetColumnController(tableCellController);
			TableViewInfoManager.ColumnController = tableCellController;
			TableViewInfoManager.StartNextCell(CurrentCell, cellBounds, cellsBounds[CurrentCell], textBounds);
			Rectangle newRowBounds = RowsController.CurrentRow.Bounds;
			newRowBounds.Y = textBounds.Y;
			RowsController.CurrentRow.Bounds = newRowBounds;
			TableViewInfoManager.CurrentCellBottom = tableViewInfo.TopAnchor.VerticalPosition + tableViewInfo.TopAnchor.BottomTextIndent;
			RowsController.CurrentColumn = tableCellController.GetStartColumn();
			RowsController.OnCellStart();
			currentCellRowCount = 0;
		}
		void EnsureCurrentTableRow(TableRow row) {
			if (CurrentRow != row) {
				currentRow = row;
			}
		}
		ModelUnit CalculateTableActualLeftRelativeOffset(Table table) {
			TableCell firstCell = table.Rows.First.Cells.First;
			ModelUnit leftBorderWidth = VerticalBordersCalculator.GetLeftBorderWidth(borderCalculator, firstCell, false);
			if (table.NestedLevel > 0 || RowsController.MatchHorizontalTableIndentsToTextEdge) {
				return leftBorderWidth / 2;
			}
			else {
				MarginUnitBase leftMargin = firstCell.GetActualLeftMargin();
				ModelUnit leftMarginValue = leftMargin.Type == WidthUnitType.ModelUnits ? leftMargin.Value : 0;
				return -Math.Max(leftMarginValue, leftBorderWidth / 2);
			}
		}
		Rectangle GetTextBounds(TableCell cell, Rectangle cellBounds) {
			ModelUnit topBorderHeight = borderCalculator.GetActualWidth(cell.GetActualTopCellBorder());
			ModelUnit topMargin = GetActualCellTopMargin(cell);
			ModelUnit leftBorderWidth = VerticalBordersCalculator.GetLeftBorderWidth(borderCalculator, cell, true); 
			ModelUnit rightBorderWidth = VerticalBordersCalculator.GetRightBorderWidth(borderCalculator, cell);
			ModelUnit leftMargin = GetActualCellLeftMargin(cell);
			ModelUnit rightMargin = GetActualCellRightMargin(cell);
			ModelUnit leftOffsetInModelUnit = Math.Max(leftMargin, leftBorderWidth / 2);
			ModelUnit rightOffsetInModelUnit = Math.Max(rightMargin, rightBorderWidth / 2);
			LayoutUnit leftOffset = UnitConverter.ToLayoutUnits(leftOffsetInModelUnit);
			LayoutUnit marginsWidth = UnitConverter.ToLayoutUnits(leftOffsetInModelUnit + rightOffsetInModelUnit);
			LayoutUnit topOffset = UnitConverter.ToLayoutUnits(topBorderHeight + topMargin);
			return new Rectangle(cellBounds.X + leftOffset, cellBounds.Y + topOffset, Math.Max(0, cellBounds.Width - marginsWidth), cellBounds.Height);
		}
		internal static ModelUnit GetActualCellSpacing(TableRow row) {
			WidthUnit spacing = row.CellSpacing;
			if (spacing.Type == WidthUnitType.ModelUnits)
				return spacing.Value * 2;
			return 0;
		}
		ModelUnit GetActualCellTopMargin(TableCell cell) {
			if (cell == null)
				return 0;
			WidthUnit margin = cell.GetActualTopMargin();
			if (margin.Type == WidthUnitType.ModelUnits)
				return margin.Value;
			return 0;
		}
		ModelUnit GetActualCellBottomMargin(TableCell cell) {
			WidthUnit margin = cell.GetActualBottomMargin();
			if (margin.Type == WidthUnitType.ModelUnits)
				return margin.Value;
			return 0;
		}
		ModelUnit GetActualCellLeftMargin(TableCell cell) {
			WidthUnit margin = cell.GetActualLeftMargin();
			if (margin.Type == WidthUnitType.ModelUnits)
				return margin.Value;
			return 0;
		}
		ModelUnit GetActualCellRightMargin(TableCell cell) {
			WidthUnit margin = cell.GetActualRightMargin();
			if (margin.Type == WidthUnitType.ModelUnits)
				return margin.Value;
			return 0;
		}
		abstract class BottomTextIndentCalculatorBase {
			public static BottomTextIndentCalculatorBase Create(TablesControllerTableState state, Table table, int rowSeparatorIndex) {
				int lastSeparatorIndex = table.Rows.Count;
				int rowIndex = rowSeparatorIndex < lastSeparatorIndex ? rowSeparatorIndex : lastSeparatorIndex - 1;
				TableRow row = table.Rows[rowIndex];
				int cellSpacing = TablesControllerTableState.GetActualCellSpacing(row);
				bool hasCellSpacig = cellSpacing > 0;
				if (hasCellSpacig) {
					return new SpacingBottomTextIndentCalculator(state, table, rowSeparatorIndex);
				}
				else {
					if (rowSeparatorIndex < lastSeparatorIndex)
						return new NoSpacingBottomTextIndentCalculator(state, table, rowSeparatorIndex);
					else
						return new NoSpacingLastAnchorBottomTextIndentCalculator(state, table, rowSeparatorIndex);
				}
			}
			int rowIndex;
			int rowSeparatorIndex;
			TablesControllerTableState state;
			Table table;
			protected BottomTextIndentCalculatorBase(TablesControllerTableState state, Table table, int rowSeparatorIndex) {
				this.table = table;
				this.state = state;
				this.rowSeparatorIndex = rowSeparatorIndex;
				int rowCount = table.Rows.Count;
				this.rowIndex = Math.Min(rowSeparatorIndex, rowCount - 1);
			}
			protected TablesControllerTableState State { get { return state; } }
			protected TableBorderCalculator BorderCalculator { get { return State.borderCalculator; } }
			protected Table Table { get { return table; } }
			protected TableRow Row { get { return Table.Rows[RowIndex]; } }
			protected int RowIndex { get { return rowIndex; } }
			protected int RowSeparatorIndex { get { return rowSeparatorIndex; } }
			public LayoutUnit CalculateBottomTextIndent(out List<HorizontalCellBordersInfo> horizontalCellBordersInfo) {
				ModelUnit result = CalculateBottomTextIndentCore(out horizontalCellBordersInfo);
				return State.UnitConverter.ToLayoutUnits(result);
			}
			protected abstract ModelUnit CalculateBottomTextIndentCore(out List<HorizontalCellBordersInfo> horizontalCellBordersInfo);
		}
		class NoSpacingBottomTextIndentCalculator : BottomTextIndentCalculatorBase {
			public NoSpacingBottomTextIndentCalculator(TablesControllerTableState state, Table table, int rowSeparatorIndex)
				: base(state, table, rowSeparatorIndex) {
			}
			protected override ModelUnit CalculateBottomTextIndentCore(out List<HorizontalCellBordersInfo> horizontalCellBordersInfo) {
				ModelUnit maxIndent = 0;
				List<HorizontalCellBordersInfo> bordersInfo = GetBordersInfo();
				horizontalCellBordersInfo = bordersInfo;
				int count = bordersInfo.Count;
				for (int i = 0; i < count; i++) {
					HorizontalCellBordersInfo borderInfo = bordersInfo[i];
					if (borderInfo.Border == null)
						continue;
					ModelUnit bottomTextIndent = GetBottomTextIndentForCell(borderInfo);
					maxIndent = Math.Max(bottomTextIndent, maxIndent);
				}
				return maxIndent;
			}
			protected virtual List<HorizontalCellBordersInfo> GetBordersInfo() {
				return State.horizontalBordersCalculator.GetTopBorders(Row);
			}
			protected virtual ModelUnit GetBottomTextIndentForCell(HorizontalCellBordersInfo borderInfo) {
				TableCell cell = borderInfo.BelowCell;
				BorderBase border = borderInfo.Border;
				ModelUnit borderHeight = BorderCalculator.GetActualWidth(border);
				ModelUnit cellTopMargin = State.GetActualCellTopMargin(cell);
				return borderHeight + cellTopMargin;
			}
		}
		class NoSpacingLastAnchorBottomTextIndentCalculator : NoSpacingBottomTextIndentCalculator {
			public NoSpacingLastAnchorBottomTextIndentCalculator(TablesControllerTableState state, Table table, int rowSeparatorIndex)
				: base(state, table, rowSeparatorIndex) {
			}
			protected override ModelUnit GetBottomTextIndentForCell(HorizontalCellBordersInfo borderInfo) {
				TableCell cell = borderInfo.AboveCell;
				return BorderCalculator.GetActualWidth(cell.GetActualBottomCellBorder());
			}
			protected override List<HorizontalCellBordersInfo> GetBordersInfo() {
				return State.horizontalBordersCalculator.GetBottomBorders(Row);
			}
		}
		class SpacingBottomTextIndentCalculator : BottomTextIndentCalculatorBase {
			public SpacingBottomTextIndentCalculator(TablesControllerTableState state, Table table, int rowSeparatorIndex)
				: base(state, table, rowSeparatorIndex) {
			}
			protected override ModelUnit CalculateBottomTextIndentCore(out List<HorizontalCellBordersInfo> horizontalCellBordersInfo) {
				ModelUnit maxBottomBorderCell = GetMaxBottomBorderCell(RowSeparatorIndex - 1);
				ModelUnit maxTopBorderCell = GetMaxTopBorderCell(RowSeparatorIndex);
				ModelUnit cellSpacing = TablesControllerTableState.GetActualCellSpacing(Row);
				horizontalCellBordersInfo = null;
				return maxBottomBorderCell + maxTopBorderCell + cellSpacing;
			}
			protected virtual ModelUnit GetMaxBottomBorderCell(int rowIndex) {
				if (rowIndex < 0) {
					BorderLineStyle borderStyle = Table.GetActualTopBorder().Style;
					if (borderStyle == BorderLineStyle.None || borderStyle == BorderLineStyle.Nil || borderStyle == BorderLineStyle.Disabled)
						return 0;
					else
						return BorderCalculator.GetActualWidth(Table.GetActualTopBorder());
				}
				Debug.Assert(rowIndex < Table.Rows.Count);
				TableRow row = Table.Rows[rowIndex];
				TableCellCollection cells = row.Cells;
				ModelUnit maxBorder = 0;
				int cellCount = cells.Count;
				for (int i = 0; i < cellCount; i++) {
					TableCell cell = cells[i];
					ModelUnit bottomTextIndent = BorderCalculator.GetActualWidth(cell.GetActualBottomCellBorder());
					maxBorder = Math.Max(bottomTextIndent, maxBorder);
				}
				return maxBorder;
			}
			protected virtual ModelUnit GetMaxTopBorderCell(int rowIndex) {
				int rowCount = Table.Rows.Count;
				if (rowIndex >= rowCount) {
					BorderLineStyle borderStyle = Table.GetActualBottomBorder().Style;
					if (borderStyle == BorderLineStyle.None || borderStyle == BorderLineStyle.Nil || borderStyle == BorderLineStyle.Disabled)
						return 0;
					else
						return BorderCalculator.GetActualWidth(Table.GetActualBottomBorder());
				}
				Debug.Assert(rowIndex >= 0);
				TableRow row = Table.Rows[rowIndex];
				TableCellCollection cells = row.Cells;
				ModelUnit maxBorder = 0;
				int cellCount = cells.Count;
				for (int i = 0; i < cellCount; i++) {
					TableCell cell = cells[i];
					ModelUnit borderWidth = BorderCalculator.GetActualWidth(cell.GetActualTopCellBorder());
					ModelUnit topMargin = State.GetActualCellTopMargin(cell);
					maxBorder = Math.Max(borderWidth + topMargin, maxBorder);
				}
				return maxBorder;
			}
		}
		LayoutUnit CalculateBottomTextIndent(int rowSeparatorIndex, out List<HorizontalCellBordersInfo> cellBordersInfo) {
			BottomTextIndentCalculatorBase indentCalculator = BottomTextIndentCalculatorBase.Create(this, CurrentTable, rowSeparatorIndex);
			return indentCalculator.CalculateBottomTextIndent(out cellBordersInfo);
		}
		void ChangeCurrentCellInTheSameTable(TableCell newCell) {
			if (restartFrom == RestartFrom.NoRestart) {
				tableViewInfoManager.ValidateTopLevelColumn();
				FinishCurrentCell();
			}
			currentCell = newCell;
			EnsureCurrentTableRow(CurrentCell.Row);
			int rowIndex = newCell.Table.Rows.IndexOf(newCell.Row);
			ITableCellVerticalAnchor topAnchor = TableViewInfoManager.GetRowStartAnchor(rowIndex);
			if (topAnchor == null) { 
				AddTopAnchor();
				topAnchor = TableViewInfoManager.GetRowStartAnchor(rowIndex);
			}
			Rectangle bounds = GetCellBounds(CurrentCell);
			tableViewInfoManager.BeforeStartNextCell(CurrentCell, cellsBounds[CurrentCell]);
			TableCellColumnController columnController = ((TableCellColumnController)(RowsController.ColumnController));
			Rectangle cellBounds = bounds;
			cellBounds.Offset(columnController.ParentColumn.Bounds.X, 0);
			Rectangle textBounds = GetTextBounds(CurrentCell, bounds);
			TableViewInfoManager.StartNextCell(CurrentCell, cellBounds, cellsBounds[CurrentCell], textBounds);
			columnController.StartNewCell(TableViewInfoManager.GetRowStartColumn(rowIndex), textBounds.Left, textBounds.Top, textBounds.Width, CurrentCell);
			RowsController.CurrentColumn = columnController.GetStartColumn();
			RowsController.OnCellStart();
			Rectangle newRowBounds = RowsController.CurrentRow.Bounds;
			newRowBounds.Y = textBounds.Y;
			RowsController.CurrentRow.Bounds = newRowBounds;
			TableViewInfoManager.CurrentCellBottom = topAnchor.VerticalPosition + topAnchor.BottomTextIndent;
			tableViewInfoManager.ValidateTopLevelColumn();
		}
		void PrepareGridCellBounds() {
			TableRowCollection rows = CurrentTable.Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				int columnIndex = row.GridBefore;
				for (int j = 0; j < cellCount; j++) {
					TableCell cell = cells[j];
					cellsBounds.Add(cell, new LayoutGridRectangle(new Rectangle(), i, columnIndex, cell.ColumnSpan));
					columnIndex += cell.ColumnSpan;
				}
			}
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				for (int j = 0; j < cellCount; j++) {
					TableCell cell = cells[j];
					switch (cell.VerticalMerging) {
						case MergingState.None: cellsBounds[cell].RowSpan = 1; break;
						case MergingState.Continue: cellsBounds[cell].RowSpan = 0; break;
						case MergingState.Restart: cellsBounds[cell].RowSpan = GetRowSpan(cellsBounds[cell]); break;
					}
				}
			}
		}
		VerticalBorderPositions PrepareCellsBounds(LayoutUnit leftOffset) {
			SortedList<LayoutUnit> initialPositions = CalculateInitialPositions(leftOffset);
			SortedList<LayoutUnit> shiftedPositions = CalculateShiftedPositions();
			return new VerticalBorderPositions(initialPositions, shiftedPositions);
		}
		SortedList<LayoutUnit> CalculateShiftedPositions() {
			SortedList<LayoutUnit> shiftedPositions = new SortedList<int>();
			TableRowCollection rows = CurrentTable.Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				int offset = GetRowOffset(row);
				ShiftRow(shiftedPositions, row, offset);
			}
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				row.LayoutProperties.GridBefore = shiftedPositions.BinarySearch(cellsBounds[row.Cells.First].Bounds.Left);
				row.LayoutProperties.GridAfter = shiftedPositions.Count - shiftedPositions.BinarySearch(cellsBounds[row.Cells.Last].Bounds.Right) - 1;
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				int columnIndex = row.LayoutProperties.GridBefore;
				for (int j = 0; j < cellCount; j++) {
					TableCell cell = cells[j];
					LayoutGridRectangle gridBounds = cellsBounds[cell];
					Rectangle bounds = gridBounds.Bounds;
					int columnSpan = shiftedPositions.BinarySearch(bounds.Right) - shiftedPositions.BinarySearch(bounds.Left);
					cell.LayoutProperties.ColumnSpan = columnSpan;
					gridBounds.ColumnIndex = columnIndex;
					gridBounds.ColumnSpan = columnSpan;
					columnIndex += columnSpan;
				}
			}
			return shiftedPositions;
		}
		SortedList<LayoutUnit> CalculateInitialPositions(LayoutUnit leftOffset) {
			TableGrid grid = TableViewInfoManager.GetTableGrid();
			TableRowCollection rows = CurrentTable.Rows;
			SortedList<LayoutUnit> initialPositions = new SortedList<LayoutUnit>();
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				LayoutUnit left = GetWidth(grid, 0, row.GridBefore) + leftOffset;
				int columnIndex = row.GridBefore;
				initialPositions.Add(left);
				for (int j = 0; j < cellCount; j++) {
					TableCell cell = cells[j];
					LayoutUnit width = GetWidth(grid, columnIndex, cell.ColumnSpan);
					cellsBounds[cell].Bounds = new Rectangle(left, 0, width, 0);
					left += width;
					columnIndex += cell.ColumnSpan;
					initialPositions.Add(left);
				}
				if (i == 0 || left > tableRight)
					tableRight = left;
			}
			return initialPositions;
		}
		protected virtual void ShiftRow(SortedList<LayoutUnit> positions, TableRow row, int offset) {
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				TableCell cell = cells[i];
				Rectangle bounds = cellsBounds[cell].Bounds;
				bounds.Offset(offset, 0);
				positions.Add(bounds.Left);
				cellsBounds[cell].Bounds = bounds;
			}
			positions.Add(cellsBounds[cells.Last].Bounds.Right);
		}
		private int GetRowSpan(LayoutGridRectangle gridPosition) {
			TableRowCollection rows = CurrentTable.Rows;
			int rowCount = rows.Count;
			int result = 1;
			for (int i = gridPosition.RowIndex + 1; i < rowCount; i++) {
				TableRow row = rows[i];
				TableCellCollection cells = row.Cells;
				int cellCount = cells.Count;
				bool cellFound = false;
				for (int j = 0; j < cellCount; j++) {
					TableCell cell = cells[j];
					if (cell.VerticalMerging == MergingState.Continue) {
						LayoutGridRectangle nextGridPosition = cellsBounds[cell];
						if (nextGridPosition.ColumnIndex == gridPosition.ColumnIndex && nextGridPosition.ColumnSpan == gridPosition.ColumnSpan) {
							cellFound = true;
							break;
						}
					}
				}
				if (!cellFound)
					break;
				result++;
			}
			return result;
		}
		LayoutUnit GetWidth(TableGrid grid, int firstColumn, int count) {
			int result = 0;
			for (int i = 0; i < count; i++)
				result += Math.Max(grid.Columns[firstColumn + i].Width, 1);
			return result;
		}
		public override void EnsureCurrentCell(TableCell cell) {
			if (restartFrom == RestartFrom.CellStart) {
				TableCell sameLevelCell = cell;
				while (sameLevelCell.Table.NestedLevel > CurrentTable.NestedLevel)
					sameLevelCell = sameLevelCell.Table.ParentCell;
				ChangeCurrentCellInTheSameTable(sameLevelCell);
				restartFrom = RestartFrom.NoRestart;
				if (cell.Table.NestedLevel > CurrentTable.NestedLevel)
					TablesController.StartNewTable(cell);
				return;
			}
			if (cell == CurrentCell)
				return;
			if (cell == null || CurrentCell.Table.ParentCell == cell) {
				TablesController.LeaveCurrentTable(cell);
				return;
			}
			if (cell.Table == CurrentTable) {
				ChangeCurrentCellInTheSameTable(cell);
				return;
			}
			if (cell.Table.NestedLevel == CurrentTable.NestedLevel) {
				TablesController.LeaveCurrentTable(cell);
				return;
			}
			if (cell.Table.NestedLevel > CurrentTable.NestedLevel) {
				TableCell sameLevelCell = cell;
				while (sameLevelCell.Table.NestedLevel > CurrentTable.NestedLevel)
					sameLevelCell = sameLevelCell.Table.ParentCell;
				if (sameLevelCell != CurrentCell) {
					if (sameLevelCell.Table == CurrentCell.Table)
						ChangeCurrentCellInTheSameTable(sameLevelCell);
					else {
						TablesController.LeaveCurrentTable(cell);
						return;
					}
				}
			}
			if (cell.Table.NestedLevel < CurrentTable.NestedLevel) {
				TablesController.LeaveCurrentTable(cell);
				return;
			}
			TablesController.StartNewTable(cell);
		}
		public override void OnCurrentRowFinished() {
			currentCellRowCount++;
		}
		public void LeaveCurrentTable(bool beforeRestart, bool roolbackParent) {
			if (!beforeRestart) {
				FinishCurrentCell();
				FinishCurrentTable();
			}
			TableViewInfoManager.LeaveCurrentTable(beforeRestart);
			TableCellColumnController columnController = ((TableCellColumnController)(RowsController.ColumnController));
			RowsController.SetColumnController(columnController.Parent);
			currentCell = null;
			if (beforeRestart) {
				if (!roolbackParent)
					RowsController.CurrentColumn = columnController.FirstParentColumn;
				else
					RowsController.CurrentColumn = columnController.ParentColumn;
			}
			else
				RowsController.CurrentColumn = columnController.LastParentColumn;
			if (!beforeRestart) {
				Rectangle newRowBounds = RowsController.CurrentRow.Bounds;
				newRowBounds.Y = TableViewInfoManager.GetLastTableBottom();
				newRowBounds.Width = RowsController.CurrentColumn.Bounds.Width;
				RowsController.CurrentRow.Bounds = newRowBounds;
#if DEBUGTEST || DEBUG
				tableViewInfoManager.ValidateTableViewInfos();
#endif
			}
			else {
				RowsController.CurrentRow.Bounds = this.rowBoundsBeforeTableStart;
			}
		}
		void FinishCurrentTable() {
			List<TableViewInfo> tableViewInfos = TableViewInfoManager.GetTableViewInfos();
			int count = tableViewInfos.Count;
			List<TableCellViewInfo> pendingCells = new List<TableCellViewInfo>();
			for (int i = 0; i < count; i++) {
				TableViewInfo tableViewInfo = tableViewInfos[i];
				tableViewInfoManager.FixColumnOverflow();
				tableViewInfo.Complete(0, 0);
				ProcessPendingCells(tableViewInfo, pendingCells);
			}
			Debug.Assert(tableViewInfos.Count > 0);
			if (tableViewInfos[0].Cells.Count == 0) {
				Debug.Assert(tableViewInfos.Count > 1);
				tableViewInfos[1].PrevTableViewInfo = null;
				tableViewInfos[0].TopLevelColumn.RemoveTableViewInfoWithContent(tableViewInfos[0]);
			}
			if (CurrentTable.NestedLevel == 0) {
				TablesController.RemoveAllTableBreaks();
			}
		}
		void ProcessPendingCells(TableViewInfo tableViewInfo, List<TableCellViewInfo> pendingCells) {
			int count = pendingCells.Count;
			for (int i = count - 1; i >= 0; i--) {
				Debug.Assert(false);
			}
		}
		void FinishCurrentCell() {
			LayoutUnit bottomMargin = UnitConverter.ToLayoutUnits(GetActualCellBottomMargin(CurrentCell));
			int currentCellBottom = TableViewInfoManager.CurrentCellBottom;
			currentCellBottom += bottomMargin;
			bool fixedRowHeight = false;
			if (cellsBounds[CurrentCell].RowSpan == 1 && CurrentCell.Row.Height.Type != HeightUnitType.Auto) {
				LayoutUnit rowTop = TableViewInfoManager.GetCurrentCellTop();
				LayoutUnit currentRowHeight = currentCellBottom - TableViewInfoManager.GetCurrentCellTop();
				LayoutUnit rowHeight = CurrentCell.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(currentCell.Row.Height.Value);
				if (currentCell.Row.Height.Type == HeightUnitType.Minimum && currentRowHeight < rowHeight)
					currentCellBottom += (rowHeight - currentRowHeight);
				else if (currentCell.Row.Height.Type == HeightUnitType.Exact) {
					currentCellBottom = rowTop + rowHeight;
					fixedRowHeight = true;
				}
			}
			TableCellVerticalAnchor bottomAnchor = TableViewInfoManager.GetBottomCellAnchor(CurrentCell);
			int rowSeparatorIndex = tableViewInfoManager.GetBottomCellRowSeparatorIndex(CurrentCell, cellsBounds[CurrentCell].RowSpan);
			TableCellColumnController columnController = ((TableCellColumnController)(RowsController.ColumnController));
			if (bottomAnchor == null) {
				List<HorizontalCellBordersInfo> cellBordersInfo;
				LayoutUnit bottomTextIndent = CalculateBottomTextIndent(rowSeparatorIndex, out cellBordersInfo);
				bottomAnchor = new TableCellVerticalAnchor(currentCellBottom, bottomTextIndent, cellBordersInfo);
				bottomAnchor.TopTextIndent = bottomMargin;
				TableViewInfoManager.SetRowSeparator(rowSeparatorIndex, bottomAnchor);
			}
			else {
				if (bottomMargin > bottomAnchor.TopTextIndent) {
					int newBottom = bottomAnchor.VerticalPosition - bottomAnchor.TopTextIndent + bottomMargin;
					if (!fixedRowHeight)
						currentCellBottom = Math.Max(currentCellBottom, newBottom);
				}
				TableCellVerticalAnchor anchor = new TableCellVerticalAnchor(currentCellBottom, bottomAnchor.BottomTextIndent, bottomAnchor.CellBorders);
				TableCellVerticalAnchor maxAnchor = columnController.GetMaxAnchor(bottomAnchor, anchor);
				bottomAnchor = maxAnchor;
				bottomAnchor.TopTextIndent = Math.Max(bottomMargin, bottomAnchor.TopTextIndent);
			}
			TableViewInfoManager.SetRowSeparatorForCurrentTableViewInfo(rowSeparatorIndex, bottomAnchor);
			this.currentCellRowCount = 0;
			TableViewInfoManager.CurrentCellBottom = currentCellBottom;
		}
		protected internal virtual void AddTopAnchor() {
			int rowSeparatorIndex = CurrentCell.RowIndex;
			List<HorizontalCellBordersInfo> cellBordersInfo;
			LayoutUnit bottomTextIndent = CalculateBottomTextIndent(rowSeparatorIndex, out cellBordersInfo);
			TableCellVerticalAnchor topAnchor = new TableCellVerticalAnchor(TableViewInfoManager.CurrentCellBottom, bottomTextIndent, cellBordersInfo);
			topAnchor.TopTextIndent = UnitConverter.ToLayoutUnits(GetActualCellTopMargin(CurrentCell));
			TableViewInfoManager.SetRowSeparator(rowSeparatorIndex, topAnchor);
			TableViewInfoManager.SetRowSeparatorForCurrentTableViewInfo(rowSeparatorIndex, topAnchor);
		}
		private Rectangle GetCellBounds(TableCell newCell) {
			LayoutGridRectangle gridPosition = cellsBounds[newCell];
			Rectangle result = gridPosition.Bounds;
			MarginUnitBase topMargin = newCell.GetActualTopMargin();
			int cellTopMargin = topMargin.Type == WidthUnitType.ModelUnits ? topMargin.Value : 0;
			cellTopMargin = newCell.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(cellTopMargin);
			int topBorderHeight = borderCalculator.GetActualWidth(newCell.GetActualTopCellBorder());
			topBorderHeight = newCell.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(topBorderHeight);
			ITableCellVerticalAnchor topAnchor = TableViewInfoManager.GetRowStartAnchor(gridPosition.RowIndex);
			result.Y = topAnchor.VerticalPosition + topAnchor.BottomTextIndent - cellTopMargin - topBorderHeight;
			WidthUnit cellSpacing = newCell.Row.CellSpacing;
			int cellSpacingValue = cellSpacing.Value * 2;
			if (cellSpacing.Type == WidthUnitType.ModelUnits && cellSpacingValue > 0) {
				DocumentModelUnitToLayoutUnitConverter converter = newCell.DocumentModel.ToDocumentLayoutUnitConverter;
				bool isFirstCellInRow = gridPosition.ColumnIndex == 0;
				int gridColumnCount = newCell.Row.Cells.Last.GetEndColumnIndexConsiderRowGrid() + 1;
				bool isLastCellInRow = (gridPosition.ColumnIndex + gridPosition.ColumnSpan) == gridColumnCount;
				int leftShift = isFirstCellInRow ? cellSpacingValue : cellSpacingValue / 2;
				int rightShift = isLastCellInRow ? cellSpacingValue : (cellSpacingValue - cellSpacingValue / 2);
				ModelUnit leftBorderWidth = VerticalBordersCalculator.GetLeftBorderWidth(borderCalculator, newCell, true);
				ModelUnit rightBorderWidth = VerticalBordersCalculator.GetRightBorderWidth(borderCalculator, newCell);
				leftShift += leftBorderWidth / 2;
				rightShift += rightBorderWidth / 2;
				leftShift = converter.ToLayoutUnits(leftShift);
				rightShift = converter.ToLayoutUnits(rightShift);
				if (!isFirstCellInRow) {
					result.X += leftShift;
					result.Width -= leftShift + rightShift;
				}
				else {
					result.Width -= rightShift;
				}
				if (result.Width < 0)
					result.Width = 1;
			}
			return result;
		}
		protected virtual int GetRowOffset(TableRow row) {
			TableRowAlignment alignment = row.TableRowAlignment;
			if (alignment != TableRowAlignment.Center && alignment != TableRowAlignment.Right)
				return 0;
			switch (alignment) {
				case TableRowAlignment.Center:
					return (maxTableWidth - tableRight) / 2;
				case TableRowAlignment.Right:
					int maxRight = maxTableWidth;
					Table table = row.Table;
					if (table.NestedLevel == 0) {
						ModelUnit leftCellMargin = GetActualCellLeftMargin(table.Rows.First.Cells.First);
						LayoutUnit layoutLeftCellMaring = UnitConverter.ToLayoutUnits(leftCellMargin);
						maxRight += layoutLeftCellMaring;
					}
					return maxRight - tableRight;
				default:
					return 0;
			}
		}
		public override void EndParagraph(Row lastRow) {
			UpdateCurrentCellHeight(lastRow);
		}
		public override void UpdateCurrentCellBottom(LayoutUnit bottom) {
			if (CurrentCell == null)
				return;
			TableViewInfoManager.CurrentCellBottom = Math.Max(bottom, TableViewInfoManager.CurrentCellBottom);
		}
		public override void UpdateCurrentCellHeight(Row row) {
			if (CurrentCell == null)
				return;
			TableViewInfoManager.CurrentCellBottom = Math.Max(row.Bounds.Bottom, TableViewInfoManager.CurrentCellBottom);
		}
		public override void BeforeMoveRowToNextColumn() {
		}
		public override void AfterMoveRowToNextColumn() {
			TableViewInfoManager.AfterMoveRowToNextColumn();
			TableViewInfoManager.CurrentCellBottom = 0;
			this.currentCellRowCount = 0;
			tableViewInfoManager.ValidateTopLevelColumn();
		}
		public override ParagraphIndex RollbackToStartOfRowTableOnFirstCellRowColumnOverfull(bool firstTableRowViewInfoInColumn, bool innerMostTable) {
			int newColumnBottom;
			int bottomAnchorIndex;
			int firstInvalidRowIndex;
			if (firstTableRowViewInfoInColumn) {
				newColumnBottom = Int32.MaxValue;
				bottomAnchorIndex = tableViewInfoManager.GetCurrentCellBottomAnchorIndex();
				if (CurrentCell.Table.NestedLevel == 0) {
					TablesController.AddInfinityTableBreak(tableViewInfoManager.GetCurrentTableViewInfoIndex(), bottomAnchorIndex);
					firstInvalidRowIndex = CalcFirstInvalidRowIndex();
				}
				else {
					firstInvalidRowIndex = 0;
				}
			}
			else {
				newColumnBottom = tableViewInfoManager.GetCurrentCellTop();
				bottomAnchorIndex = tableViewInfoManager.GetCurrentCellTopAnchorIndex();
				if (innerMostTable)
					TablesController.AddTableBreak(CurrentTable, tableViewInfoManager.GetCurrentTableViewInfoIndex(), newColumnBottom);
				firstInvalidRowIndex = CalcFirstInvalidRowIndex();
			}
			TableViewInfoManager.RemoveAllInvalidRowsOnColumnOverfull(firstInvalidRowIndex);
			if (firstInvalidRowIndex == 0) {
				restartFrom = RestartFrom.TableStart;
				ParagraphIndex paragraphIndex = CurrentTable.Rows.First.Cells.First.StartParagraphIndex;
				int nestedLevel = CurrentCell.Table.NestedLevel;
				LeaveCurrentTable(true, CurrentTable.NestedLevel > 0);
				this.TablesController.ReturnToPrevState();
				if (nestedLevel > 0)
					return new ParagraphIndex(-1);
				else
					return paragraphIndex;
			}
			else {
				TableCellCollection cells = CurrentTable.Rows[firstInvalidRowIndex].Cells;
				int cellCount = cells.Count;
				for (int i = 0; i < cellCount; i++) {
					if (cells[i].VerticalMerging != MergingState.Continue) {
						restartFrom = RestartFrom.CellStart;
						return cells[i].StartParagraphIndex;
					}
				}
				Exceptions.ThrowInternalException();
				return ParagraphIndex.Zero;
			}
		}
		private int CalcFirstInvalidRowIndex() {
			int result = CurrentTable.Rows.IndexOf(CurrentCell.Row);
			for (; result > 0; ) {
				int newResult = CalcFirstInvalidRowIndexCore(result);
				if (newResult == result)
					break;
				else
					result = newResult;
			}
			return result;
		}
		int CalcFirstInvalidRowIndexCore(int startRowIndex) {
			int result = startRowIndex;
			TableCellCollection cells = CurrentTable.Rows[startRowIndex].Cells;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				TableCell cell = cells[i];
				if (cell == CurrentCell)
					continue;
				if (cell.VerticalMerging != MergingState.Continue)
					continue;
				TableCell topCell = CurrentTable.GetFirstCellInVerticalMergingGroup(cell);
				result = Math.Min(result, CurrentTable.Rows.IndexOf(topCell.Row));
			}
			return result;
		}
		public TableCell GetCurrentCell() {
			return CurrentCell;
		}
	}
	#endregion
	public enum TableBreakType {
		NoBreak,
		NextPage,
		InfinityHeight
	}
	public class BreakComparer : IComparable<KeyValuePair<int, int>> {
		int key;
		public BreakComparer(int key) {
			this.key = key;
		}
		#region IComparable<KeyValuePair<int,int>> Members
		public int CompareTo(KeyValuePair<int, int> other) {
			return other.Key - key;
		}
		#endregion
	}
	public class BreakPairComparer : IComparer<KeyValuePair<int, int>> {
		#region IComparer<KeyValuePair<int,int>> Members
		public int Compare(KeyValuePair<int, int> x, KeyValuePair<int, int> y) {
			return x.Key - y.Key;
		}
		#endregion
	}
	#region TablesController
	public class TablesController {
		readonly RowsController rowsController;
		readonly PageController pageController;
		readonly Stack<TablesControllerStateBase> states;
		readonly Dictionary<Table, SortedList<KeyValuePair<int, int>>> tableBreaks;
		Dictionary<int, int> infinityHeights;
		public TablesController(PageController pageController, RowsController rowsController) {
			Guard.ArgumentNotNull(rowsController, "rowsController");
			Guard.ArgumentNotNull(pageController, "pageController");
			this.rowsController = rowsController;
			this.pageController = pageController;
			this.states = new Stack<TablesControllerStateBase>();
			this.states.Push(new TablesControllerNoTableState(this));
			this.tableBreaks = new Dictionary<Table, SortedList<KeyValuePair<int, int>>>();
			this.infinityHeights = new Dictionary<int, int>();
		}
		public RowsController RowsController { get { return rowsController; } }
		public PageController PageController { get { return pageController; } }
		public TablesControllerStateBase State { get { return states.Peek(); } }
		public bool IsInsideTable { get { return states.Count > 1; } }
		public TableBreakType GetTableBreak(Table table, int tableViewInfoIndex, out int bottomBounds) {
			SortedList<KeyValuePair<int, int>> breaks;
			bottomBounds = 0;
			if (!tableBreaks.TryGetValue(table, out breaks))
				return TableBreakType.NoBreak;
			int index = breaks.BinarySearch(new BreakComparer(tableViewInfoIndex));
			if (index < 0)
				return TableBreakType.NoBreak;
			else {
				bottomBounds = breaks[index].Value;
				return TableBreakType.NextPage;
			}
		}
		public bool IsInfinityHeight(int tableViewInfoIndex, out int bottomAnchorIndex) {
			if (!infinityHeights.TryGetValue(tableViewInfoIndex, out bottomAnchorIndex)) {
				return false;
			}
			return true;
		}
		public void AddTableBreak(Table table, int tableViewInfoIndex, int bottomBounds) {
			SortedList<KeyValuePair<int, int>> breaks;
			if (!tableBreaks.TryGetValue(table, out breaks)) {
				breaks = new SortedList<KeyValuePair<int, int>>(new BreakPairComparer());
				tableBreaks.Add(table, breaks);
			}
			else {
				int keyIndex = breaks.Count - 1;
				while (keyIndex >= 0 && breaks[keyIndex].Key > tableViewInfoIndex) {
					breaks.RemoveAt(keyIndex);
					keyIndex--;
				}
			}
			breaks.Add(new KeyValuePair<int, int>(tableViewInfoIndex, bottomBounds));
		}
		public void AddInfinityTableBreak(int tableViewInfoIndex, int bottomAnchorIndex) {
			this.infinityHeights.Add(tableViewInfoIndex, bottomAnchorIndex);
		}
		public void RemoveAllTableBreaks() {
			this.tableBreaks.Clear();
			this.infinityHeights.Clear();
		}
		public void BeginParagraph(Paragraph paragraph) {
			TableCell paragraphCell = paragraph.GetCell();
			State.EnsureCurrentCell(paragraphCell);
		}
		public void StartNewTable(TableCell newCell) {
			int currentNestedLevel = states.Count - 1;
			if (newCell.Table.NestedLevel > currentNestedLevel) {
				StartNewTable(newCell.Table.ParentCell);
			}
			if (newCell.Table.NestedLevel > 0)
				StartInnerTable(newCell);
			else
				StartTopLevelTable(newCell);
		}
		public void StartTopLevelTable(TableCell newCell) {
			rowsController.NewTableStarted();
			states.Push(new TablesControllerTableState(this, newCell, rowsController.CurrentColumn.Rows.Count == 0));
		}
		public void EndParagraph(Row lastRow) {
			foreach (TablesControllerStateBase state in states)
				state.EndParagraph(lastRow);
		}
		public virtual CanFitCurrentRowToColumnResult CanFitRowToColumn(LayoutUnit lastTextRowBottom, Column column) {
			return State.CanFitRowToColumn(lastTextRowBottom, column);
		}
		public void BeforeMoveRowToNextColumn() {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			for (int i = count - 1; i >= 0; i--)
				statesArray[i].BeforeMoveRowToNextColumn();
		}
		public void AfterMoveRowToNextColumn() {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			for (int i = count - 1; i >= 0; i--)
				statesArray[i].AfterMoveRowToNextColumn();
		}
		public void AfterMoveRowToNextPage() {
		}
		public void BeforeMoveRowToNextPage() {
		}
		public void StartInnerTable(TableCell cell) {
			rowsController.NewTableStarted();
			states.Push(new TablesControllerTableState(this, cell, ((TablesControllerTableState)State).CurrentCellViewInfoEmpty));
		}
		public void UpdateCurrentCellBottom(LayoutUnit bottom) {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			for (int i = count - 1; i >= 0; i--)
				statesArray[i].UpdateCurrentCellBottom(bottom);
		}
		public void UpdateCurrentCellHeight() {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			for (int i = count - 1; i >= 0; i--)
				statesArray[i].UpdateCurrentCellHeight(RowsController.CurrentRow);
		}
		public void ReturnToPrevState() {
			states.Pop();
		}
		public void LeaveCurrentTable(TableCell nextCell) {
			TablesControllerTableState tableState = (TablesControllerTableState)State;
			tableState.LeaveCurrentTable(false, false);
			ReturnToPrevState();
			if (states.Count == 1)
				RowsController.ColumnController.PageAreaController.PageController.EndTableFormatting();
			State.EnsureCurrentCell(nextCell);
		}
		public void ClearInvalidatedContent() {
			this.states.Clear();
			this.states.Push(new TablesControllerNoTableState(this));
		}
		public void Reset() {
			ClearInvalidatedContent();
		}
		public void OnCurrentRowFinished() {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			for (int i = count - 1; i >= 0; i--)
				statesArray[i].OnCurrentRowFinished();
		}
		public ParagraphIndex RollbackToStartOfRowTableOnFirstCellRowColumnOverfull() {
			int count = states.Count;
			TablesControllerStateBase[] statesArray = states.ToArray();
			bool firstTableRowViewInfoInColumn = false;
			if (count > 1)
				firstTableRowViewInfoInColumn = ((TablesControllerTableState)State).IsFirstTableRowViewInfoInColumn();
			for (int i = 0; i < count; i++) {
				ParagraphIndex paragraphIndex = statesArray[i].RollbackToStartOfRowTableOnFirstCellRowColumnOverfull(firstTableRowViewInfoInColumn, i == 0);
				if (paragraphIndex >= ParagraphIndex.Zero)
					return paragraphIndex;
			}
			Debug.Assert(false);
			return ParagraphIndex.Zero;
		}
		protected internal virtual TableViewInfoManager CreateTableViewInfoManager(TableViewInfoManager parentTableViewInfoManager, PageController pageController, RowsController rowsController) {
			return new TableViewInfoManager(parentTableViewInfoManager, pageController, rowsController);
		}
		public TableCell GetCurrentCell() {
			if (states.Count <= 1)
				return null;
			else
				return ((TablesControllerTableState)states.Peek()).GetCurrentCell();
		}
	}
	#endregion
	public class TableCellVerticalBorderCalculator {
		Table table;
		public TableCellVerticalBorderCalculator(Table table) {
			this.table = table;
		}
		BorderInfo GetLeftBorder(TableBorderCalculator borderCalculator, TableCell cell) {
			if (cell.Row.CellSpacing.Type == WidthUnitType.ModelUnits && cell.Row.CellSpacing.Value > 0)
				return cell.GetActualLeftCellBorder().Info;
			int cellIndex = cell.Row.Cells.IndexOf(cell);
			if (cellIndex > 0) {
				BorderBase prevCellBorder = cell.Row.Cells[cellIndex - 1].GetActualRightCellBorder();
				return borderCalculator.GetVerticalBorderSource(table, prevCellBorder.Info, cell.GetActualLeftCellBorder().Info);
			}
			else {
				BorderInfo border = cell.GetActualLeftCellBorder().Info;
				return border;
			}
		}
		BorderInfo GetRightBorder(TableBorderCalculator borderCalculator, TableCell cell) {
			if (cell.Row.CellSpacing.Type == WidthUnitType.ModelUnits && cell.Row.CellSpacing.Value > 0)
				return cell.GetActualRightCellBorder().Info;
			int cellIndex = cell.Row.Cells.IndexOf(cell);
			if (cellIndex + 1 < cell.Row.Cells.Count) {
				BorderInfo nextCellBorder = cell.Row.Cells[cellIndex + 1].GetActualLeftCellBorder().Info;
				return borderCalculator.GetVerticalBorderSource(table, nextCellBorder, cell.GetActualRightCellBorder().Info);
			}
			else {
				BorderInfo border = cell.GetActualRightCellBorder().Info;
				return border;
			}
		}
		public ModelUnit GetLeftBorderWidth(TableBorderCalculator borderCalculator, TableCell cell, bool layoutIndex) {
			int startColumnIndex = GetStartColumnIndex(cell, layoutIndex);
			List<TableCell> verticalSpanCells = GetVerticalSpanCells(cell, startColumnIndex, layoutIndex);
			int count = verticalSpanCells.Count;
			ModelUnit result = 0;
			for (int i = 0; i < count; i++)
				result = Math.Max(borderCalculator.GetActualWidth(GetLeftBorder(borderCalculator, verticalSpanCells[i])), result);
			return result;
		}
		public ModelUnit GetRightBorderWidth(TableBorderCalculator borderCalculator, TableCell cell) {
			int startColumnIndex = GetStartColumnIndex(cell, true);
			List<TableCell> verticalSpanCells = GetVerticalSpanCells(cell, startColumnIndex, true);
			int count = verticalSpanCells.Count;
			ModelUnit result = 0;
			for (int i = 0; i < count; i++)
				result = Math.Max(borderCalculator.GetActualWidth(GetRightBorder(borderCalculator, verticalSpanCells[i])), result);
			return result;
		}
		internal static int GetStartColumnIndex(TableCell cell, bool layoutIndex) {
			TableRow row = cell.Row;
			int columnIndex = layoutIndex ? row.LayoutProperties.GridBefore : row.GridBefore;
			int cellIndex = 0;
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			while (cellIndex < count && cells[cellIndex] != cell) {
				TableCell currentCell = cells[cellIndex];
				columnIndex += layoutIndex ? currentCell.LayoutProperties.ColumnSpan : currentCell.ColumnSpan;
				cellIndex++;
			}
			return columnIndex;
		}
		internal static List<TableCell> GetVerticalSpanCells(TableCell cell, int startColumnIndex, bool layoutIndex) {
			List<TableCell> result = new List<TableCell>();
			result.Add(cell);
			if (cell.VerticalMerging != MergingState.Restart)
				return result;
			Table table = cell.Table;
			int rowIndex = table.Rows.IndexOf(cell.Row) + 1;
			for (; rowIndex < table.Rows.Count; rowIndex++) {
				TableRow row = table.Rows[rowIndex];
				TableCell rowCell = GetCellByStartColumnIndex(row, startColumnIndex, layoutIndex);
				if (rowCell != null && rowCell.VerticalMerging == MergingState.Continue)
					result.Add(rowCell);
				else
					break;
			}
			return result;
		}
		internal static List<TableCell> GetVerticalSpanCells(TableCell cell, bool layoutIndex) {
			int startColumnIndex = GetStartColumnIndex(cell, layoutIndex);
			return GetVerticalSpanCells(cell, startColumnIndex, layoutIndex);
		}
		internal static TableCell GetCellByStartColumnIndex(TableRow row, int startColumnIndex, bool layoutIndex) {
			int columnIndex = layoutIndex ? row.LayoutProperties.GridBefore : row.GridBefore;
			int cellIndex = 0;
			while (columnIndex < startColumnIndex && cellIndex < row.Cells.Count) {
				TableCell cell = row.Cells[cellIndex];
				columnIndex += layoutIndex ? cell.LayoutProperties.ColumnSpan : cell.ColumnSpan;
				cellIndex++;
			}
			if (cellIndex < row.Cells.Count)
				return row.Cells[cellIndex];
			else
				return null;
		}
		internal static TableCell GetCellByColumnIndex(TableRow row, int startColumnIndex) {
			int columnIndex = row.GridBefore;
			TableCellCollection cells = row.Cells;
			for (int i = 0; i < cells.Count; i++) {
				TableCell currentCell = cells[i];
				if (startColumnIndex >= columnIndex && startColumnIndex < columnIndex + currentCell.ColumnSpan)
					return currentCell;
				columnIndex += currentCell.ColumnSpan;
			}
			return null;
		}
		internal static TableCell GetCellByEndColumnIndex(TableRow row, int endColumnIndex) {
			TableCell cellByColumnIndex = GetCellByColumnIndex(row, endColumnIndex);
			if (cellByColumnIndex == null)
				return null;
			if (GetStartColumnIndex(cellByColumnIndex, false) + cellByColumnIndex.ColumnSpan - 1 <= endColumnIndex)
				return cellByColumnIndex;
			int cellIndex = row.Cells.IndexOf(cellByColumnIndex);
			if (cellIndex != 0)
				return row.Cells[cellIndex - 1];
			return null;
		}
		internal static List<TableCell> GetCellsByIntervalColumnIndex(TableRow row, int startColumnIndex, int endColumnIndex) {
			List<TableCell> result = new List<TableCell>();
			while (startColumnIndex <= endColumnIndex) {
				TableCell cell = GetCellByColumnIndex(row, startColumnIndex);
				if (cell == null)
					return result;
				result.Add(cell);
				int cellStartColumnIndex = GetStartColumnIndex(cell, false);
				startColumnIndex += startColumnIndex - cellStartColumnIndex + cell.ColumnSpan;
			}
			return result;
		}
	}
	public class HorizontalCellBordersInfo {
		int columnSpan;
		int startColumnIndex;
		BorderBase border;
		TableCell aboveCell;
		TableCell belowCell;
		public HorizontalCellBordersInfo(TableCell aboveCell, TableCell belowCell, BorderBase border, int startColumnIndex, int endColumnIndex) {
			if (endColumnIndex < startColumnIndex)
				Exceptions.ThrowArgumentException("endColumnIndex", endColumnIndex);
			this.border = border;
			this.startColumnIndex = startColumnIndex;
			this.columnSpan = endColumnIndex - startColumnIndex + 1;
			this.aboveCell = aboveCell;
			this.belowCell = belowCell;
		}
		public int StartColumnIndex { get { return startColumnIndex; } }
		public int ColumnSpan { get { return columnSpan; } }
		public int EndColumnIndex { get { return StartColumnIndex + ColumnSpan - 1; } }
		public BorderBase Border { get { return border; } }
		public TableCell AboveCell { get { return aboveCell; } }
		public TableCell BelowCell { get { return belowCell; } }
	}
	public abstract class TableCellIteratorBase {
		public abstract int CurrentStartColumnIndex { get; }
		public abstract int CurrentEndColumnIndex { get; }
		public abstract TableCell CurrentCell { get; }
		public abstract bool EndOfRow { get; }
		public abstract void SetStartColumnIndex(int newStartColumnIndex);
		public abstract bool MoveNextCell();
	}
	public class TableCellIterator : TableCellIteratorBase {
		TableRow row;
		int currentStartColumnIndex;
		int currentEndColumnIndex;
		int currentCellIndex;
		public TableCellIterator(TableRow row) {
			this.row = row;
			this.currentStartColumnIndex = -1;
			this.currentEndColumnIndex = -1;
			this.currentCellIndex = -1;
		}
		public override int CurrentStartColumnIndex { get { return currentStartColumnIndex; } }
		public override int CurrentEndColumnIndex { get { return currentEndColumnIndex; } }
		public override TableCell CurrentCell { get { return (currentCellIndex >= 0 && currentCellIndex < CellCount) ? row.Cells[currentCellIndex] : null; } }
		public override bool EndOfRow { get { return currentCellIndex >= CellCount; } }
		protected TableRow Row { get { return row; } }
		protected int CellCount { get { return Row.Cells.Count; } }
		public override void SetStartColumnIndex(int newStartColumnIndex) {
			if (newStartColumnIndex < CurrentStartColumnIndex)
				Exceptions.ThrowInternalException();
			while (newStartColumnIndex > CurrentEndColumnIndex)
				if (!MoveNextCell())
					return;
			currentStartColumnIndex = newStartColumnIndex;
		}
		public override bool MoveNextCell() {
			if (EndOfRow)
				return false;
			currentCellIndex++;
			currentStartColumnIndex = currentCellIndex == 0 ? Row.LayoutProperties.GridBefore : CurrentEndColumnIndex + 1;
			if (currentCellIndex < Row.Cells.Count) {
				currentEndColumnIndex = CurrentStartColumnIndex + CurrentCell.LayoutProperties.ColumnSpan - 1;
			}
			else {
				currentEndColumnIndex = currentStartColumnIndex;
			}
			return true;
		}
	}
	public class TableCellEmptyIterator : TableCellIteratorBase {
		public override int CurrentStartColumnIndex {
			get { throw new NotImplementedException(); }
		}
		public override int CurrentEndColumnIndex {
			get { throw new NotImplementedException(); }
		}
		public override TableCell CurrentCell {
			get { return null; }
		}
		public override bool EndOfRow {
			get { return true; }
		}
		public override bool MoveNextCell() {
			return false;
		}
		public override void SetStartColumnIndex(int newStartColumnIndex) {
			throw new NotImplementedException();
		}
	}
	public class TableCellBorderIterator {
		TableCell currentCellAbove;
		TableCell currentCellBelow;
		TableCellIteratorBase aboveRowIterator;
		TableCellIteratorBase belowRowIterator;
		int currentStartColumnIndex;
		int currentEndColumnIndex;
		public TableCellBorderIterator(TableRow aboveRow, TableRow belowRow) {
			if (aboveRow == null && belowRow == null)
				Exceptions.ThrowInternalException();
			if (aboveRow != null)
				this.aboveRowIterator = new TableCellIterator(aboveRow);
			else
				this.aboveRowIterator = new TableCellEmptyIterator();
			if (belowRow != null)
				this.belowRowIterator = new TableCellIterator(belowRow);
			else
				this.belowRowIterator = new TableCellEmptyIterator();
			this.currentCellAbove = null;
			this.currentCellBelow = null;
			this.aboveRowIterator.MoveNextCell();
			this.belowRowIterator.MoveNextCell();
		}
		public HorizontalCellBordersInfo CurrentAboveInfo {
			get {
				if (currentCellAbove == null)
					return null;
				BorderBase border = currentCellAbove.GetActualBottomCellBorder();
				return new HorizontalCellBordersInfo(currentCellAbove, currentCellBelow, border, currentStartColumnIndex, currentEndColumnIndex);
			}
		}
		public HorizontalCellBordersInfo CurrentMergedInfo {
			get {
				Debug.Assert(currentCellAbove != null && currentCellBelow != null);
				return new HorizontalCellBordersInfo(currentCellAbove, currentCellBelow, null, currentStartColumnIndex, currentEndColumnIndex);
			}
		}
		public HorizontalCellBordersInfo CurrentBelowInfo {
			get {
				if (currentCellBelow == null)
					return null;
				BorderBase border = currentCellBelow.GetActualTopCellBorder();
				return new HorizontalCellBordersInfo(currentCellAbove, currentCellBelow, border, currentStartColumnIndex, currentEndColumnIndex);
			}
		}
		public bool MoveNext() {
			if (aboveRowIterator.EndOfRow && belowRowIterator.EndOfRow)
				return false;
			if (aboveRowIterator.EndOfRow) {
				currentStartColumnIndex = belowRowIterator.CurrentStartColumnIndex;
				currentEndColumnIndex = belowRowIterator.CurrentEndColumnIndex;
				currentCellAbove = null;
				currentCellBelow = belowRowIterator.CurrentCell;
				belowRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				return true;
			}
			if (belowRowIterator.EndOfRow) {
				currentStartColumnIndex = aboveRowIterator.CurrentStartColumnIndex;
				currentEndColumnIndex = aboveRowIterator.CurrentEndColumnIndex;
				currentCellAbove = aboveRowIterator.CurrentCell;
				currentCellBelow = null;
				aboveRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				return true;
			}
			int aboveStartColumnIndex = aboveRowIterator.CurrentStartColumnIndex;
			int belowStartColumnIndex = belowRowIterator.CurrentStartColumnIndex;
			if (aboveStartColumnIndex < belowStartColumnIndex) {
				currentStartColumnIndex = aboveStartColumnIndex;
				currentEndColumnIndex = Math.Min(belowStartColumnIndex - 1, aboveRowIterator.CurrentEndColumnIndex);
				currentCellAbove = aboveRowIterator.CurrentCell;
				aboveRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				return true;
			}
			else if (belowStartColumnIndex < aboveStartColumnIndex) {
				currentStartColumnIndex = belowStartColumnIndex;
				currentEndColumnIndex = Math.Min(aboveStartColumnIndex - 1, belowRowIterator.CurrentEndColumnIndex);
				currentCellBelow = belowRowIterator.CurrentCell;
				belowRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				return true;
			}
			else {
				currentStartColumnIndex = aboveStartColumnIndex;
				currentEndColumnIndex = Math.Min(aboveRowIterator.CurrentEndColumnIndex, belowRowIterator.CurrentEndColumnIndex);
				currentCellAbove = aboveRowIterator.CurrentCell;
				currentCellBelow = belowRowIterator.CurrentCell;
				aboveRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				belowRowIterator.SetStartColumnIndex(currentEndColumnIndex + 1);
				return true;
			}
		}
		public bool IsVerticallyMerged() {
			if (currentCellAbove == null || currentCellBelow == null)
				return false;
			return (currentCellAbove.VerticalMerging == MergingState.Restart || currentCellAbove.VerticalMerging == MergingState.Continue) &&
				(currentCellBelow.VerticalMerging == MergingState.Continue);
		}
	}
	public class TableCellHorizontalBorderCalculator {
		Table table;
		public TableCellHorizontalBorderCalculator(Table table) {
			this.table = table;
		}
		public List<HorizontalCellBordersInfo> GetBottomBorders(TableRow row) {
			int rowIndex = table.Rows.IndexOf(row);
			int anchorIndex = rowIndex + 1;
			return GetAnchorBorders(anchorIndex);
		}
		public List<HorizontalCellBordersInfo> GetTopBorders(TableRow row) {
			int rowIndex = table.Rows.IndexOf(row);
			int anchorIndex = rowIndex;
			return GetAnchorBorders(anchorIndex);
		}
		List<HorizontalCellBordersInfo> GetAnchorBorders(int anchorIndex) {
			int rowAboveIndex = anchorIndex - 1;
			int rowBelowIndex = anchorIndex;
			TableRow rowAbove = rowAboveIndex >= 0 ? table.Rows[rowAboveIndex] : null;
			TableRow rowBelow = rowBelowIndex < table.Rows.Count ? table.Rows[rowBelowIndex] : null;
			TableCellBorderIterator iterator = new TableCellBorderIterator(rowAbove, rowBelow);
			List<HorizontalCellBordersInfo> result = new List<HorizontalCellBordersInfo>();
			while (iterator.MoveNext()) {
				HorizontalCellBordersInfo border;
				if (!iterator.IsVerticallyMerged()) {
					HorizontalCellBordersInfo aboveBorder = iterator.CurrentAboveInfo;
					HorizontalCellBordersInfo belowBorder = iterator.CurrentBelowInfo;
					border = ResolveBorder(table, aboveBorder, belowBorder);
				}
				else {
					border = iterator.CurrentMergedInfo;
				}
				result.Add(border);
			}
			return result;
		}
		HorizontalCellBordersInfo ResolveBorder(Table table, HorizontalCellBordersInfo border1, HorizontalCellBordersInfo border2) {
			Debug.Assert(border1 != null || border2 != null);
			if (border1 == null)
				return border2;
			if (border2 == null)
				return border1;
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			BorderInfo result = borderCalculator.GetVerticalBorderSource(table, border1.Border.Info, border2.Border.Info);
			if (result == border1.Border.Info)
				return border1;
			else
				return border2;
		}
	}
	public class TableBorderViewInfoCollection : List<TableBorderViewInfoBase> {
	}
	[Flags]
	public enum BorderTypes {
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		Horizontal = Top | Bottom,
		Vertical = Left | Right
	}
	[Flags]
	public enum AdjacentBorderTypes {
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right
	}
	public abstract class TableBorderViewInfoBase : ITableBorderViewInfoBase {
		public abstract BorderInfo Border { get; }
		public abstract BorderTypes BorderType { get; }
		public abstract Rectangle GetBounds(TableViewInfo tableViewInfo);
		public abstract CornerViewInfoBase StartCorner { get; }
		public abstract CornerViewInfoBase EndCorner { get; }
		public bool HasStartCorner { get { return StartCorner != null && !(StartCorner is NoneLineCornerViewInfo); } }
		public bool HasEndCorner { get { return EndCorner != null && !(EndCorner is NoneLineCornerViewInfo); } }
		public abstract DocumentModelUnitToLayoutUnitConverter Converter { get; }
		public virtual void ExportTo(TableViewInfo tableViewInfo, IDocumentLayoutExporter exporter) {
			Rectangle bounds = GetBounds(tableViewInfo);
			exporter.ExportTableBorder(this, bounds);
#if !SL
#endif
		}
#if !SL
#endif
	}
	public abstract class TableOuterBorderViewInfoBase : TableBorderViewInfoBase {
		readonly TableViewInfo tableViewInfo;
		protected TableOuterBorderViewInfoBase(TableViewInfo tableViewInfo) {
			Guard.ArgumentNotNull(tableViewInfo, "tableViewInfo");
			this.tableViewInfo = tableViewInfo;
		}
		public TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public override CornerViewInfoBase StartCorner { get { return null; } }
		public override CornerViewInfoBase EndCorner { get { return null; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return tableViewInfo.Table.DocumentModel.ToDocumentLayoutUnitConverter; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			TableRowViewInfoBase row = GetAnchorRow();
			TableCellViewInfoCollection cells = row.Cells;
			LayoutUnit leftCellBorderWidth = Converter.ToLayoutUnits(borderCalculator.GetActualWidth(cells.First.LeftBorder));
			LayoutUnit rightCellBorderWidth = Converter.ToLayoutUnits(borderCalculator.GetActualWidth(cells.Last.RightBorder));
			TableCellViewInfo lastCell = cells.Last;
			LayoutUnit left = cells.First.Left - leftCellBorderWidth / 2;
			LayoutUnit right = lastCell.Left + lastCell.Width + rightCellBorderWidth / 2;
			LayoutUnit top = row.TopAnchor.VerticalPosition;
			LayoutUnit bottom = row.BottomAnchor.VerticalPosition + row.BottomAnchor.BottomTextIndent;
			return new Rectangle(left, top, right - left, bottom - top);
		}
		protected abstract TableRowViewInfoBase GetAnchorRow();
	}
	public abstract class VerticalTableOuterBorderViewInfoBase : TableOuterBorderViewInfoBase {
		readonly int rowIndex;
		readonly int cellSpacing;
		protected VerticalTableOuterBorderViewInfoBase(TableViewInfo tableViewInfo, int rowIndex, int cellSpacing)
			: base(tableViewInfo) {
			this.rowIndex = rowIndex;
			this.cellSpacing = cellSpacing;
		}
		protected override TableRowViewInfoBase GetAnchorRow() {
			return TableViewInfo.Rows[rowIndex];
		}
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			Rectangle bounds = base.GetBounds(tableViewInfo);
			bounds.X -= cellSpacing;
			bounds.Width += cellSpacing * 2;
			return bounds;
		}
	}
	public class LeftTableOuterBorderViewInfo : VerticalTableOuterBorderViewInfoBase {
		public LeftTableOuterBorderViewInfo(TableViewInfo tableViewInfo, int rowIndex, int cellSpacing)
			: base(tableViewInfo, rowIndex, cellSpacing) {
		}
		public override BorderInfo Border { get { return TableViewInfo.GetActualLeftBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Left; } }
	}
	public class RightTableOuterBorderViewInfo : VerticalTableOuterBorderViewInfoBase {
		public RightTableOuterBorderViewInfo(TableViewInfo tableViewInfo, int rowIndex, int cellSpacing)
			: base(tableViewInfo, rowIndex, cellSpacing) {
		}
		public override BorderInfo Border { get { return TableViewInfo.GetActualRightBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Right; } }
	}
	public abstract class HorizontalTableOuterBorderViewInfoBase : TableOuterBorderViewInfoBase {
		protected HorizontalTableOuterBorderViewInfoBase(TableViewInfo tableViewInfo)
			: base(tableViewInfo) {
		}
	}
	public class TopTableOuterBorderViewInfo : HorizontalTableOuterBorderViewInfoBase {
		public TopTableOuterBorderViewInfo(TableViewInfo tableViewInfo)
			: base(tableViewInfo) {
		}
		public override BorderInfo Border { get { return TableViewInfo.GetActualTopBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Top; } }
		protected override TableRowViewInfoBase GetAnchorRow() {
			return TableViewInfo.Rows.First;
		}
	}
	public class BottomTableOuterBorderVewInfo : HorizontalTableOuterBorderViewInfoBase {
		public BottomTableOuterBorderVewInfo(TableViewInfo tableViewInfo)
			: base(tableViewInfo) {
		}
		public override BorderInfo Border { get { return TableViewInfo.GetActualBottomBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Bottom; } }
		protected override TableRowViewInfoBase GetAnchorRow() {
			return TableViewInfo.Rows.Last;
		}
	}
	public class ParagraphHorizontalBorderViewInfo : TableBorderViewInfoBase {
		BorderInfo borderInfo;
		DocumentModelUnitToLayoutUnitConverter converter;
		CornerViewInfoBase cornerAtLeft;
		CornerViewInfoBase cornerAtRight;
		public ParagraphHorizontalBorderViewInfo(BorderInfo borderInfo, DocumentModelUnitToLayoutUnitConverter converter, CornerViewInfoBase cornerAtLeft, CornerViewInfoBase cornerAtRight) {
			this.borderInfo = borderInfo;
			this.converter = converter;
			this.cornerAtLeft = cornerAtLeft;
			this.cornerAtRight = cornerAtRight;
		}
		public override BorderInfo Border { get { return borderInfo; } }
		public override BorderTypes BorderType { get { return BorderTypes.Horizontal; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return converter; } }
		public override CornerViewInfoBase StartCorner { get { return cornerAtLeft; } }
		public override CornerViewInfoBase EndCorner { get { return cornerAtRight; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			int y = 0;
			int left = 0;
			int right = 0;
			return new Rectangle(left, y, right - left, 0);
		}
	}
	public class TableCellHorizontalBorderViewInfo : TableBorderViewInfoBase {
		TableCellVerticalAnchor anchor;
		HorizontalCellBordersInfo borderInfo;
		DocumentModelUnitToLayoutUnitConverter converter;
		CornerViewInfoBase cornerAtLeft;
		CornerViewInfoBase cornerAtRight;
		public TableCellHorizontalBorderViewInfo(TableCellVerticalAnchor anchor, HorizontalCellBordersInfo borderInfo, DocumentModelUnitToLayoutUnitConverter converter, CornerViewInfoBase cornerAtLeft, CornerViewInfoBase cornerAtRight) {
			this.anchor = anchor;
			this.borderInfo = borderInfo;
			this.converter = converter;
			this.cornerAtLeft = cornerAtLeft;
			this.cornerAtRight = cornerAtRight;
		}
		public override BorderInfo Border { get { return borderInfo.Border.Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Horizontal; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return converter; } }
		public override CornerViewInfoBase StartCorner { get { return cornerAtLeft; } }
		public override CornerViewInfoBase EndCorner { get { return cornerAtRight; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			int y = anchor.VerticalPosition;
			int left = tableViewInfo.GetColumnLeft(borderInfo.StartColumnIndex);
			int right = tableViewInfo.GetColumnRight(borderInfo.EndColumnIndex);
			return new Rectangle(left, y, right - left, 0);
		}
	}
	public class TableCellVerticalBorderViewInfo : TableBorderViewInfoBase {
		BorderInfo border;
		DocumentModelUnitToLayoutUnitConverter converter;
		TableRowViewInfoBase row;
		int layoutBorderIndex;
		int modelBorderIndex;
		CornerViewInfoBase cornerAtTop;
		CornerViewInfoBase cornerAtBottom;
		public TableCellVerticalBorderViewInfo(TableRowViewInfoBase row, BorderInfo border, int layoutBorderIndex, int modelBorderIndex, DocumentModelUnitToLayoutUnitConverter converter) {
			this.converter = converter;
			this.border = border;
			this.row = row;
			this.layoutBorderIndex = layoutBorderIndex;
			this.modelBorderIndex = modelBorderIndex;
		}
		public override BorderInfo Border { get { return border; } }
		public override BorderTypes BorderType { get { return BorderTypes.Vertical; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return converter; } }
		public CornerViewInfoBase CornerAtTop { get { return cornerAtTop; } set { cornerAtTop = value; } }
		public CornerViewInfoBase CornerAtBottom { get { return cornerAtBottom; } set { cornerAtBottom = value; } }
		public override CornerViewInfoBase StartCorner { get { return CornerAtTop; } }
		public override CornerViewInfoBase EndCorner { get { return CornerAtBottom; } }
		public int LayoutBorderIndex { get { return layoutBorderIndex; } }
		public int ModelBorderIndex { get { return modelBorderIndex; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			int top = row.TopAnchor.VerticalPosition;
			int bottom = row.BottomAnchor.VerticalPosition;
			int left = tableViewInfo.GetVerticalBorderPosition(layoutBorderIndex);
			return new Rectangle(left, top, 0, bottom - top);
		}
		public override void ExportTo(TableViewInfo tableViewInfo, IDocumentLayoutExporter exporter) {
			base.ExportTo(tableViewInfo, exporter);
		}
	}
	public abstract class TableCellBorderViewInfoBase : TableBorderViewInfoBase {
		readonly TableCellViewInfo sourceCell;
		protected TableCellBorderViewInfoBase(TableCellViewInfo sourceCell) {
			Guard.ArgumentNotNull(sourceCell, "sourceCell");
			this.sourceCell = sourceCell;
		}
		public TableCellViewInfo SourceCell { get { return sourceCell; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return sourceCell.Cell.DocumentModel.ToDocumentLayoutUnitConverter; } }
		public Rectangle GetBoundsCore(TableViewInfo tableViewInfo) {
			return tableViewInfo.GetCellBounds(sourceCell);
		}
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			return GetBoundsCore(tableViewInfo);
		}
	}
	public abstract class TableCellHorizontalBorderViewInfoBase : TableCellBorderViewInfoBase {
		CornerViewInfoBase leftCorner;
		CornerViewInfoBase rightCorner;
		protected TableCellHorizontalBorderViewInfoBase(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public CornerViewInfoBase LeftCorner { get { return leftCorner; } set { leftCorner = value; } }
		public CornerViewInfoBase RightCorner { get { return rightCorner; } set { rightCorner = value; } }
		public override CornerViewInfoBase StartCorner { get { return LeftCorner; } }
		public override CornerViewInfoBase EndCorner { get { return RightCorner; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			Rectangle rect = base.GetBounds(tableViewInfo);
			int left = rect.Left;
			int right = rect.Right;
			return new Rectangle(left, rect.Top, right - left, rect.Height);
		}
	}
	public abstract class TableCellVerticalBorderViewInfoBase : TableCellBorderViewInfoBase {
		CornerViewInfoBase topCorner;
		CornerViewInfoBase bottomCorner;
		protected TableCellVerticalBorderViewInfoBase(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public CornerViewInfoBase TopCorner { get { return topCorner; } set { topCorner = value; } }
		public CornerViewInfoBase BottomCorner { get { return bottomCorner; } set { bottomCorner = value; } }
		public override CornerViewInfoBase StartCorner { get { return TopCorner; } }
		public override CornerViewInfoBase EndCorner { get { return BottomCorner; } }
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			Rectangle rect = base.GetBounds(tableViewInfo);
			int top = rect.Top;
			int bottom = rect.Bottom;
			return new Rectangle(rect.Left, top, rect.Width, bottom - top);
		}
	}
	public class LeftTableCellBorderViewInfo : TableCellVerticalBorderViewInfoBase {
		public LeftTableCellBorderViewInfo(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public override BorderInfo Border { get { return SourceCell.LeftBorder.Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Left; } }
	}
	public class RightTableCellBorderViewInfo : TableCellVerticalBorderViewInfoBase {
		public RightTableCellBorderViewInfo(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public override BorderInfo Border { get { return SourceCell.RightBorder.Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Right; } }
	}
	public class TopTableCellBorderViewInfo : TableCellHorizontalBorderViewInfoBase {
		public TopTableCellBorderViewInfo(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public override BorderInfo Border { get { return SourceCell.TopBorder.Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Top; } }
	}
	public class BottomTableCellBorderViewInfo : TableCellHorizontalBorderViewInfoBase {
		public BottomTableCellBorderViewInfo(TableCellViewInfo sourceCell)
			: base(sourceCell) {
		}
		public override BorderInfo Border { get { return SourceCell.BottomBorder.Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Bottom; } }
	}
	#region TableBorderWithSpacingViewInfoBase (abstract class)
	public abstract class TableBorderWithSpacingViewInfoBase : TableBorderViewInfoBase {
		#region Fields
		TableRowViewInfoWithCellSpacing rowViewInfo;
		CornerViewInfoBase leftCorner;
		CornerViewInfoBase rightCorner;
		#endregion
		protected TableBorderWithSpacingViewInfoBase(TableRowViewInfoWithCellSpacing rowViewInfo) {
			Guard.ArgumentNotNull(rowViewInfo, "rowViewInfo");
			this.rowViewInfo = rowViewInfo;
		}
		#region Properties
		public TableRowViewInfoWithCellSpacing RowViewInfo { get { return rowViewInfo; } set { rowViewInfo = value; } }
		protected internal TableViewInfo TableViewInfo { get { return rowViewInfo.TableViewInfo; } }
		public override DocumentModelUnitToLayoutUnitConverter Converter { get { return rowViewInfo.Row.DocumentModel.ToDocumentLayoutUnitConverter; } }
		public CornerViewInfoBase LeftCorner { get { return leftCorner; } set { leftCorner = value; } }
		public CornerViewInfoBase RightCorner { get { return rightCorner; } set { rightCorner = value; } }
		public override CornerViewInfoBase StartCorner { get { return LeftCorner; } }
		public override CornerViewInfoBase EndCorner { get { return RightCorner; } }
		#endregion
	}
	#endregion
	#region LeftRightTableBorderWithSpacingViewInfoBase (abstract class)
	public abstract class LeftRightTableBorderWithSpacingViewInfoBase : TableBorderWithSpacingViewInfoBase {
		protected LeftRightTableBorderWithSpacingViewInfoBase(TableRowViewInfoWithCellSpacing rowViewInfo)
			: base(rowViewInfo) {
		}
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			int top = RowViewInfo.TopAnchor.VerticalPosition;
			TableRowViewInfoWithCellSpacing previousRowViewInfo = RowViewInfo.Previous as TableRowViewInfoWithCellSpacing;
			if (previousRowViewInfo != null)
				top += Converter.ToLayoutUnits(previousRowViewInfo.CellSpacing) / 2;
			int cellSpacing = Converter.ToLayoutUnits(RowViewInfo.CellSpacing);
			TableCellViewInfo firstCellViewInfo = RowViewInfo.Cells[0];
			int left = firstCellViewInfo.Left - cellSpacing;
			TableCellViewInfo lastCellViewInfo = RowViewInfo.Cells.Last;
			int right = lastCellViewInfo.Left + lastCellViewInfo.Width + cellSpacing;
			ITableCellVerticalAnchor bottomAnchor = RowViewInfo.BottomAnchor;
			int bottom = bottomAnchor.VerticalPosition;
			if (RowViewInfo.Next != null)
				bottom += cellSpacing / 2;
			else
				bottom += cellSpacing;
			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
	#endregion
	#region LeftTableBorderWithSpacingViewInfo
	public class LeftTableBorderWithSpacingViewInfo : LeftRightTableBorderWithSpacingViewInfoBase {
		public LeftTableBorderWithSpacingViewInfo(TableRowViewInfoWithCellSpacing rowViewInfo)
			: base(rowViewInfo) {
		}
		#region Properties
		public override BorderInfo Border { get { return TableViewInfo.GetActualLeftBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Left; } }
		#endregion
	}
	#endregion
	#region RightTableBorderWithSpacingViewInfo
	public class RightTableBorderWithSpacingViewInfo : LeftRightTableBorderWithSpacingViewInfoBase {
		public RightTableBorderWithSpacingViewInfo(TableRowViewInfoWithCellSpacing rowViewInfo)
			: base(rowViewInfo) {
		}
		#region Properties
		public override BorderInfo Border { get { return TableViewInfo.GetActualRightBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Right; } }
		#endregion
	}
	#endregion
	#region TopTableBorderWithSpacingViewInfo
	public class TopTableBorderWithSpacingViewInfo : TableBorderWithSpacingViewInfoBase {
		public TopTableBorderWithSpacingViewInfo(TableRowViewInfoWithCellSpacing rowViewInfo)
			: base(rowViewInfo) {
		}
		#region Properties
		public override BorderInfo Border { get { return TableViewInfo.GetActualTopBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Top; } }
		#endregion
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			int top = tableViewInfo.TopAnchor.VerticalPosition;
			TableCellViewInfo firstCellViewInfo = RowViewInfo.Cells[0];
			int cellSpacing = Converter.ToLayoutUnits(RowViewInfo.CellSpacing);
			int left = firstCellViewInfo.Left - cellSpacing;
			TableCellViewInfo lastCellViewInfo = RowViewInfo.Cells.Last;
			int right = lastCellViewInfo.Left + lastCellViewInfo.Width + cellSpacing;
			return new Rectangle(left, top, right - left, 0);
		}
	}
	#endregion
	#region BottomTableBorderWithSpacingViewInfo
	public class BottomTableBorderWithSpacingViewInfo : TableBorderWithSpacingViewInfoBase {
		public BottomTableBorderWithSpacingViewInfo(TableRowViewInfoWithCellSpacing rowViewInfo)
			: base(rowViewInfo) {
		}
		#region Properties
		public override BorderInfo Border { get { return TableViewInfo.GetActualBottomBorder().Info; } }
		public override BorderTypes BorderType { get { return BorderTypes.Bottom; } }
		#endregion
		public override Rectangle GetBounds(TableViewInfo tableViewInfo) {
			ITableCellVerticalAnchor bottomAnchor = tableViewInfo.BottomAnchor;
			int cellSpacing = Converter.ToLayoutUnits(RowViewInfo.CellSpacing);
			int bottom = bottomAnchor.VerticalPosition + cellSpacing;
			TableCellViewInfo firstCellViewInfo = RowViewInfo.Cells[0];
			int left = firstCellViewInfo.Left - cellSpacing;
			TableCellViewInfo lastCellViewInfo = RowViewInfo.Cells.Last;
			int right = lastCellViewInfo.Left + lastCellViewInfo.Width + cellSpacing;
			return new Rectangle(left, bottom, right - left, 0);
		}
	}
	#endregion
	public class TableBorderCalculator {
		internal class TableBorderInfo {
			readonly int[] compoundArray;
			readonly float[] drawingCompoundArray;
			readonly int widthDivider;
			readonly int widthMultiplier;
			readonly int lineCount;
			public TableBorderInfo(int[] compoundArray, int widthDivider) {
				this.compoundArray = compoundArray;
				this.widthDivider = widthDivider;
				int lastIndex = compoundArray.Length - 1;
				this.lineCount = compoundArray.Length / 2;
				this.widthMultiplier = this.compoundArray[lastIndex];
				drawingCompoundArray = new float[compoundArray.Length];
				for (int i = 0; i <= lastIndex; i++)
					drawingCompoundArray[i] = (float)compoundArray[i] / (float)widthMultiplier;
			}
			public int LineCount { get { return lineCount; } }
			public int[] CompoundArray { get { return compoundArray; } }
			public float[] DrawingCompoundArray { get { return drawingCompoundArray; } }
			public int GetActualWidth(int borderWidth) {
				return borderWidth * widthMultiplier / widthDivider;
			}
		}
		static Dictionary<BorderLineStyle, TableBorderInfo> lineStyleInfos;
		static TableBorderCalculator() {
			lineStyleInfos = new Dictionary<BorderLineStyle, TableBorderInfo>();
			AddLineStyleInfo(BorderLineStyle.Single, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.Thick, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.Double, new int[] { 0, 1, 2, 3 }, 1); 
			AddLineStyleInfo(BorderLineStyle.Dotted, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.Dashed, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.DotDash, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.DotDotDash, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.Triple, new int[] { 0, 1, 2, 3, 4, 5 }, 1);
			AddLineStyleInfo(BorderLineStyle.ThinThickSmallGap, new int[] { 0, 1, 2, 10 }, 8); 
			AddLineStyleInfo(BorderLineStyle.ThickThinSmallGap, new int[] { 0, 8, 9, 10 }, 8);
			AddLineStyleInfo(BorderLineStyle.ThinThickThinSmallGap, new int[] { 0, 1, 2, 10, 11, 12 }, 8);
			AddLineStyleInfo(BorderLineStyle.ThinThickMediumGap, new int[] { 0, 1, 2, 4 }, 2);
			AddLineStyleInfo(BorderLineStyle.ThickThinMediumGap, new int[] { 0, 2, 3, 4 }, 2);
			AddLineStyleInfo(BorderLineStyle.ThinThickThinMediumGap, new int[] { 0, 1, 2, 4, 5, 6 }, 2);
			AddLineStyleInfo(BorderLineStyle.ThinThickLargeGap, new int[] { 0, 1, 9, 11 }, 8);
			AddLineStyleInfo(BorderLineStyle.ThickThinLargeGap, new int[] { 0, 2, 10, 11 }, 8);
			AddLineStyleInfo(BorderLineStyle.ThinThickThinLargeGap, new int[] { 0, 1, 9, 11, 19, 20 }, 8);
			AddLineStyleInfo(BorderLineStyle.Wave, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.DoubleWave, new int[] { 0, 1, 1, 2 }, 1);
			AddLineStyleInfo(BorderLineStyle.DashSmallGap, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.DashDotStroked, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.ThreeDEmboss, new int[] { 0, 1, 1, 5, 6 }, 4);
			AddLineStyleInfo(BorderLineStyle.ThreeDEngrave, new int[] { 0, 1, 1, 5, 6 }, 4);
			AddLineStyleInfo(BorderLineStyle.Outset, new int[] { 0, 1 }, 1);
			AddLineStyleInfo(BorderLineStyle.Inset, new int[] { 0, 1 }, 1);
		}
		internal static Dictionary<BorderLineStyle, TableBorderInfo> LineStyleInfos { get { return lineStyleInfos; } }
		static void AddLineStyleInfo(BorderLineStyle lineStyle, int[] compoundArray, int widthDivider) {
			lineStyleInfos.Add(lineStyle, new TableBorderInfo(compoundArray, widthDivider));
		}
		public int GetActualWidth(BorderBase border) {
			return GetActualWidth(border.Info);
		}
		public int GetActualWidth(BorderInfo border) {
			return GetActualWidth(border.Style, border.Width);
		}
		protected BorderLineStyle GetActualBorderLineStyle(BorderLineStyle borderLineStyle) {
			if (borderLineStyle == BorderLineStyle.None || borderLineStyle == BorderLineStyle.Nil || borderLineStyle == BorderLineStyle.Disabled || lineStyleInfos.ContainsKey(borderLineStyle))
				return borderLineStyle;
			return BorderLineStyle.Single;
		}
		internal int GetActualWidth(BorderLineStyle style, int width) {
			TableBorderInfo info;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(style), out info))
				return 0;
			else
				return info.GetActualWidth(width);
		}
		public int GetLineCount(BorderInfo border) {
			TableBorderInfo info;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(border.Style), out info))
				return 0;
			else
				return info.LineCount;
		}
		public float[] GetDrawingCompoundArray(BorderLineStyle borderLineStyle) {
			TableBorderInfo info;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(borderLineStyle), out info))
				return null;
			else
				return info.DrawingCompoundArray;
		}
		public float[] GetDrawingCompoundArray(BorderInfo border) {
			return GetDrawingCompoundArray(border.Style);
		}
		public float[] GetDrawingCompoundArray(BorderBase border) {
			return GetDrawingCompoundArray(border.Style);
		}
		public BorderInfo GetVerticalBorderSource(Table table, BorderInfo firstCellBorder, BorderInfo secondCellBorder) {
			if (firstCellBorder == null)
				return secondCellBorder;
			if (secondCellBorder == null)
				return firstCellBorder;
			int leftCellBorderWeight = CalculateWeight(firstCellBorder);
			int rightCellBorderWeight = CalculateWeight(secondCellBorder);
			if (leftCellBorderWeight > rightCellBorderWeight)
				return firstCellBorder;
			else if (rightCellBorderWeight > leftCellBorderWeight)
				return secondCellBorder;
			int leftCellStyleWeight = (int)firstCellBorder.Style;
			int rightCellStyleWeight = (int)secondCellBorder.Style;
			if (leftCellStyleWeight > rightCellStyleWeight)
				return firstCellBorder;
			else if (rightCellStyleWeight > leftCellStyleWeight)
				return secondCellBorder;
			int leftCellBrightness = firstCellBorder.Color.R + firstCellBorder.Color.B + 2 * firstCellBorder.Color.G;
			int rightCellBrightness = secondCellBorder.Color.R + secondCellBorder.Color.B + 2 * secondCellBorder.Color.G;
			if (leftCellBrightness == rightCellBrightness) {
				leftCellBrightness = firstCellBorder.Color.B + 2 * firstCellBorder.Color.G;
				rightCellBrightness = secondCellBorder.Color.B + 2 * secondCellBorder.Color.G;
				if (leftCellBrightness == rightCellBrightness) {
					leftCellBrightness = firstCellBorder.Color.G;
					rightCellBrightness = secondCellBorder.Color.G;
				}
			}
			if (leftCellBrightness < rightCellBrightness)
				return firstCellBorder;
			else if (rightCellBrightness < leftCellBrightness)
				return secondCellBorder;
			return firstCellBorder;
		}
		public bool IsVisuallyAdjacentBorder(BorderInfo border1, BorderInfo border2, bool sameDirection) {
			if (border1 == null || border2 == null)
				return false;
			if (Object.ReferenceEquals(border1, border2))
				return true;
			TableBorderInfo borderInfo1;
			TableBorderInfo borderInfo2;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(border1.Style), out borderInfo1))
				return false;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(border2.Style), out borderInfo2))
				return false;
			if (borderInfo1.GetActualWidth(border1.Width) != borderInfo2.GetActualWidth(border2.Width))
				return false;
			if (borderInfo1.LineCount != borderInfo2.LineCount)
				return false;
			if (sameDirection)
				return border1.Style == border2.Style;
			else
				return true;
		}
		int CalculateWeight(BorderInfo border) {
			TableBorderInfo info;
			BorderLineStyle borderStyle = border.Style;
			if (!lineStyleInfos.TryGetValue(GetActualBorderLineStyle(borderStyle), out info))
				return borderStyle == BorderLineStyle.Disabled ? Int32.MaxValue : 0;
			else
				return info.LineCount * (int)borderStyle;
		}
	}
	#region ITableCellVerticalAnchor
	public interface ITableCellVerticalAnchor {
		LayoutUnit VerticalPosition { get; }
		LayoutUnit BottomTextIndent { get; }
		TableCellVerticalAnchor CloneWithNewVerticalPosition(LayoutUnit newVerticalPostion);
	}
	#endregion
	#region TableCellVerticalAnchor
	public class TableCellVerticalAnchor : ITableCellVerticalAnchor {
		LayoutUnit verticalPosition;
		LayoutUnit topTextIndent;
		readonly LayoutUnit bottomTextIndent;
		readonly List<HorizontalCellBordersInfo> cellBorders;
		readonly List<CornerViewInfoBase> corners;
		public TableCellVerticalAnchor(LayoutUnit verticalPosition, LayoutUnit bottomTextIndent, List<HorizontalCellBordersInfo> cellBorders) {
			this.verticalPosition = verticalPosition;
			this.bottomTextIndent = bottomTextIndent;
			this.cellBorders = cellBorders;
			this.corners = new List<CornerViewInfoBase>();
		}
		#region ITableCellVertiaclAnchor Members
		public LayoutUnit VerticalPosition { get { return verticalPosition; } }
		public LayoutUnit BottomTextIndent { get { return bottomTextIndent; } }
		public List<HorizontalCellBordersInfo> CellBorders { get { return cellBorders; } }
		public List<CornerViewInfoBase> Corners { get { return corners; } }
		public LayoutUnit TopTextIndent { get { return topTextIndent; } set { topTextIndent = value; } }
		#endregion
		public TableCellVerticalAnchor CloneWithNewVerticalPosition(LayoutUnit newVerticalPostion) {
			TableCellVerticalAnchor result = new TableCellVerticalAnchor(newVerticalPostion, BottomTextIndent, CellBorders);
			result.TopTextIndent = TopTextIndent;
			Debug.Assert(Corners.Count == 0);
			return result;
		}
		public virtual void MoveVertically(LayoutUnit deltaY) {
			verticalPosition += deltaY;
		}
		internal void SetVerticalPositionInternal(LayoutUnit value) {
			this.verticalPosition = value;
		}
	}
	#endregion
	#region TableCellVerticalAnchorYComparable
	public class TableCellVerticalAnchorYComparable : IComparable<TableCellVerticalAnchor> {
		readonly int pos;
		public TableCellVerticalAnchorYComparable(int pos) {
			this.pos = pos;
		}
		#region IComparable<TableCellVerticalAnchor> Members
		public int CompareTo(TableCellVerticalAnchor anchor) {
			if (pos < anchor.VerticalPosition)
				return 1;
			else if (pos > anchor.VerticalPosition)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
