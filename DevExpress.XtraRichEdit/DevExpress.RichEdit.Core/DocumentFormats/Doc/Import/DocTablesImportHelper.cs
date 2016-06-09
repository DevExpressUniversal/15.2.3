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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocTablesImporter
	public class DocTablesImporter {
		#region Fields
		readonly PieceTable pieceTable;
		readonly Stack<TableCell> tableCells;
		readonly DocTableLayoutUpdater tableLayoutUpdater;		
		ParagraphIndex lastAddedParagraphIndex;
		HorizontalAnchorTypes horizontalAnchor;
		VerticalAnchorTypes verticalAnchor;
		HashSet<Table> referencedTables;
		#endregion
		public DocTablesImporter(DocImporter importer, PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.tableCells = new Stack<TableCell>();
			this.referencedTables = new HashSet<Table>();
			this.tableLayoutUpdater = new DocTableLayoutUpdater(importer, pieceTable.DocumentModel.UnitConverter);
		}
		#region Properties
		protected PieceTable PieceTable { get { return this.pieceTable; } }
		protected Stack<TableCell> TableCells { get { return this.tableCells; } }
		protected DocTableLayoutUpdater TableLayoutUpdater { get { return this.tableLayoutUpdater; } }
		TableCell CurrentCell {
			get {
				return (this.tableCells.Count > 0) ? this.tableCells.Peek() : null;
			}
		}
		int CurrentTableDepth {
			get {
				return (CurrentCell != null) ? CurrentCell.Table.NestedLevel + 1 : 0;
			}
		}
		protected ParagraphIndex LastAddedParagraphIndex { get { return this.lastAddedParagraphIndex; } set { this.lastAddedParagraphIndex = value; } }
		#endregion
		public void ParagraphAdded(Paragraph paragraph, DocPropertyContainer propertyContainer) {
			LastAddedParagraphIndex = paragraph.Index;
			if (propertyContainer.ParagraphInfo == null)
				return;
			if (propertyContainer.ParagraphInfo.TableDepth == CurrentTableDepth)
				return;
			if (propertyContainer.ParagraphInfo.TableDepth > CurrentTableDepth)
				CreateNestedTable(propertyContainer);
			else
				FinishTable(TableCells.Pop().Table);
		}
		public bool Finish() {
			if (TableCells.Count == 0)
				return false;
			while (TableCells.Count > 0)
				FinishTable(TableCells.Pop().Table);
			return true;
		}
		public void BeforeSectionAdded() {
			while (TableCells.Count > 0) {
				FinishTable(TableCells.Pop().Table);
			}
		}
		public void CellEndReached(DocPropertyContainer propertyContainer) {
			if (CurrentTableDepth == 0)
				return;
			TableCell cell = TableCells.Pop();
			ApplyCellProperties(propertyContainer, cell);
			PieceTable.TableCellsManager.InitializeTableCell(cell, cell.StartParagraphIndex, cell.EndParagraphIndex);
			cell.Row.Cells.AddInternal(cell);
			TableCell nextCell = new TableCell(cell.Row);
			TableCells.Push(nextCell);
			nextCell.StartParagraphIndex = LastAddedParagraphIndex + 1;
		}
		void ApplyCellProperties(DocPropertyContainer propertyContainer, TableCell cell) {
			if (propertyContainer.TableCellInfo != null)
				ApplyCellPropertiesCore(cell, propertyContainer.TableCellInfo.TableCellProperties);
			cell.EndParagraphIndex = LastAddedParagraphIndex;
			if (propertyContainer.TableInfo != null)
				ApplyTableFloatingPosition(propertyContainer.TableInfo.TableProperties.FloatingPosition);
		}
		void ApplyCellPropertiesCore(TableCell cell, TableCellProperties properties) {
			cell.Properties.BeginInit();
			cell.Properties.CopyFrom(properties);
			cell.Properties.EndInit();
		}
		void ApplyTableFloatingPosition(TableFloatingPosition pos) {
			this.horizontalAnchor = pos.HorizontalAnchor;
			this.verticalAnchor = pos.VerticalAnchor;
		}
		public void RowEndReached(DocPropertyContainer propertyContainer) {
			if (CurrentTableDepth == 0)
				return;
			TableCell cell = TableCells.Pop();
			TableRow row = cell.Row;
			cell = TryToUpdateCurrentTable(cell, propertyContainer);
			ApplyRowProperties(row, propertyContainer);
			TableLayoutUpdater.PerformRowActions(row, propertyContainer);
			row.Table.Rows.AddInternal(row);
			TableRow nextRow = new TableRow(row.Table);
			TableCell nextCell = new TableCell(nextRow);
			nextCell.StartParagraphIndex = LastAddedParagraphIndex + 1;
			TableCells.Push(nextCell);
		}
		TableCell TryToUpdateCurrentTable(TableCell cell, DocPropertyContainer propertyContainer) {
			TableRow row = cell.Row;
			if (ShouldFinishTable(cell.Table, propertyContainer)) {
				FinishTable(cell.Table);
				CreateNestedTable(propertyContainer);
				cell = TableCells.Pop();
				row.SetTable(cell.Table);
			}
			return cell;
		}
		void ApplyRowProperties(TableRow row, DocPropertyContainer propertyContainer) {
			Table table = row.Table;
			if (propertyContainer.TableRowInfo != null)
				ApplyRowPropertiesCore(row, propertyContainer.TableRowInfo.TableRowProperties);
			if (propertyContainer.TableInfo != null)
				ApplyTableInfo(table, propertyContainer.TableInfo);
		}
		void ApplyRowPropertiesCore(TableRow row, TableRowProperties properties) {
			row.Properties.BeginInit();
			row.Properties.CopyFrom(properties);
			row.Properties.EndInit();
		}
		void ApplyTableInfo(Table table, TableInfo info) {
			UpdateTableProperties(table, info.TableProperties);
			if (info.TableStyleIndex >= 0)
				table.StyleIndex = info.TableStyleIndex;
			else
				ResetTableProperties(table.TableProperties);
		}
		void UpdateTableProperties(Table table, TableProperties properties) {
			table.TableProperties.BeginUpdate();
			table.TableProperties.CopyFrom(properties);
			table.TableProperties.EndUpdate();
		}
		void ResetTableProperties(TableProperties properties) {
			properties.BeginUpdate();
			properties.CellMargins.Left.Value = 0;
			properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
			properties.CellMargins.Right.Value = 0;
			properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
			properties.CellMargins.Top.Value = 0;
			properties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
			properties.CellMargins.Bottom.Value = 0;
			properties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
			properties.EndUpdate();
		}
		void CreateNestedTable(DocPropertyContainer propertyContainer) {
			if (propertyContainer.ParagraphInfo == null)
				return;
			int count = propertyContainer.ParagraphInfo.TableDepth - CurrentTableDepth;
			for (int i = 0; i < count; i++)
				StartTable();
		}
		void StartTable() {
			Table table = new Table(PieceTable, CurrentCell, 0, 0);
			PieceTable.Tables.Add(table);
			TableCell cell = new TableCell(new TableRow(table));
			cell.StartParagraphIndex = LastAddedParagraphIndex;
			TableCells.Push(cell);
		}
		bool ShouldFinishTable(Table table, DocPropertyContainer propertyContainer) {
			if (table.Rows.Count == 0)
				return false;
			if (propertyContainer.TableInfo == null)
				return true;
			TableFloatingPosition currentPosition = table.TableProperties.FloatingPosition;
			TableFloatingPosition nextPosition = propertyContainer.TableInfo.TableProperties.FloatingPosition;
			return (table.StyleIndex != propertyContainer.TableInfo.TableStyleIndex && propertyContainer.TableInfo.TableStyleIndex != -1) || !currentPosition.CompareTo(nextPosition);
		}
		void FinishTable(Table table) {
			if (table.Rows.Count == 0 || table.Rows.First.Cells.Count == 0) {
				if (this.referencedTables.Contains(table))
					DocImporter.ThrowInvalidDocFile();
				PieceTable.Tables.Remove(table.Index);
			}
			else {
				TableLayoutUpdater.UpdateLayout(table);
				table.NormalizeTableGrid();
				if (table.ParentCell != null)
					referencedTables.Add(table.ParentCell.Table);
			}
		}
	}
	#endregion
	#region DocTableLayoutUpdater
	public class DocTableLayoutUpdater {
		#region Fields
		const int tolerance = 32; 
		readonly DocumentModelUnitConverter unitConverter;
		readonly VerticalMergingStateUpdater verticalMergingUpdater;
		readonly Dictionary<Table, List<int>> tableCellPositions;
		readonly Dictionary<TableRow, List<int>> rowCellWidths;
		readonly Dictionary<TableRow, List<TableCellWidthOperand>> preferredCellWidths;
		readonly DocImporter importer;
		#endregion
		public DocTableLayoutUpdater(DocImporter importer, DocumentModelUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
			this.verticalMergingUpdater = new VerticalMergingStateUpdater();
			this.tableCellPositions = new Dictionary<Table, List<int>>();
			this.rowCellWidths = new Dictionary<TableRow, List<int>>();
			this.preferredCellWidths = new Dictionary<TableRow, List<TableCellWidthOperand>>();
			this.importer = importer;
		}
		#region Properties
		protected DocumentModelUnitConverter UnitConverter { get { return this.unitConverter; } }
		protected VerticalMergingStateUpdater VerticalMergingUpdater { get { return this.verticalMergingUpdater; } }
		protected Dictionary<Table, List<int>> TableCellPositions { get { return this.tableCellPositions; } }
		protected Dictionary<TableRow, List<int>> RowCellWidths { get { return this.rowCellWidths; } }
		protected Dictionary<TableRow, List<TableCellWidthOperand>> PreferredCellWidths { get { return this.preferredCellWidths; } }
		#endregion
		public void PerformRowActions(TableRow row, DocPropertyContainer propertyContainer) {
			AddPreferredCellWidths(row, propertyContainer);
			AddRowCellPositions(row, propertyContainer);
			TableCellPropertiesUpdater.UpdateTableRow(row, propertyContainer);
			VerticalMergingUpdater.UpdateCellsVerticalMergeState(row, propertyContainer);
			ApplyTablePropertiesException(row, propertyContainer.TableRowInfo);
		}
		void AddPreferredCellWidths(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableRowInfo == null)
				return;
			List<TableCellWidthOperand> tableCellWidths = propertyContainer.TableRowInfo.PreferredCellWidths;
			if (tableCellWidths.Count > 0)
				PreferredCellWidths.Add(row, tableCellWidths);
		}
		void AddRowCellPositions(TableRow row, DocPropertyContainer propertyContainer) {
			List<int> rowCellWidths = new List<int>();
			List<int> tableCellPositions = GetTableCellPositions(row.Table);
			ProcessTableDefinition(row, propertyContainer, rowCellWidths);
			ApplyInsertActions(propertyContainer, rowCellWidths);
			ApplyColumnWidthsActions(propertyContainer, rowCellWidths);
			RowCellWidths.Add(row, rowCellWidths);
			AddTableCellPositions(row, rowCellWidths, tableCellPositions);
		}
		List<int> GetTableCellPositions(Table table) {
			List<int> result;
			if (!TableCellPositions.TryGetValue(table, out result)) {
				result = new List<int>();
				TableCellPositions.Add(table, result);
			}
			return result;
		}
		void ProcessTableDefinition(TableRow row, DocPropertyContainer propertyContainer, List<int> rowCellWidths) {
			TableDefinitionOperand operand = propertyContainer.TableInfo.TableDefinition;
			int count = operand.ColumnsCount;
			for (int i = 0; i < count; i++) {
				int currentWidth = operand.Positions[i + 1] - operand.Positions[i];
				if (currentWidth >= 0)
					rowCellWidths.Add(currentWidth);
			}
			List<TableCellDescriptor> cells = propertyContainer.TableInfo.TableDefinition.Cells;
			int firstMergedCell = 0;
			int span;
			for (int i = 0; i < cells.Count; i++) {
				if (cells[i].HorizontalMerging != MergingState.Continue) {
					span = i - firstMergedCell;
					if (span > 1)
						importer.AddCellsHorizontalMerging(row, firstMergedCell, span);
					firstMergedCell = i;
				}
			}
			span = cells.Count - firstMergedCell;
			if (span > 1)
				importer.AddCellsHorizontalMerging(row, firstMergedCell, span);
		}
		void ApplyInsertActions(DocPropertyContainer propertyContainer, List<int> rowCellWidths) {
			if (propertyContainer.TableRowInfo == null)
				return;
			int insertionsCount = propertyContainer.TableRowInfo.InsertActions.Count;
			for (int insertionIndex = 0; insertionIndex < insertionsCount; insertionIndex++) {
				InsertOperand currentInsertion = propertyContainer.TableRowInfo.InsertActions[insertionIndex];
				byte count = currentInsertion.Count;
				for (int columnIndex = 0; columnIndex < count; columnIndex++) {
					rowCellWidths.Insert(currentInsertion.StartIndex + columnIndex, currentInsertion.WidthInTwips);
				}
			}
		}
		void ApplyColumnWidthsActions(DocPropertyContainer propertyContainer, List<int> rowCellWidths) {
			if (propertyContainer.TableRowInfo == null)
				return;
			int count = propertyContainer.TableRowInfo.ColumnWidthActions.Count;
			for (int widthActionIndex = 0; widthActionIndex < count; widthActionIndex++) {
				ColumnWidthOperand currentWidthAction = propertyContainer.TableRowInfo.ColumnWidthActions[widthActionIndex];
				for (int columnIndex = currentWidthAction.StartIndex; columnIndex <= currentWidthAction.EndIndex; columnIndex++) {
					rowCellWidths[columnIndex] = currentWidthAction.WidthInTwips;
				}
			}
		}
		void AddTableCellPositions(TableRow row, List<int> rowCellWidths, List<int> tableCellPositions) {
			int currentPosition = row.Properties.WidthBefore.Value;
			int correction = TryToAddCellPosition(tableCellPositions, ref currentPosition);
			if (correction != 0)
				row.Properties.WidthBefore.Value += correction;
			int count = rowCellWidths.Count;
			for (int i = 0; i < count; i++) {
				currentPosition += rowCellWidths[i];
				correction = TryToAddCellPosition(tableCellPositions, ref currentPosition);
				rowCellWidths[i] += correction;
			}
			if (row.Properties.WidthAfter.Value > 0) {
				currentPosition += row.Properties.WidthAfter.Value;
				correction = TryToAddCellPosition(tableCellPositions, ref currentPosition);
				if (correction != 0)
					row.Properties.WidthAfter.Value += correction;
			}
		}
		int TryToAddCellPosition(List<int> destination, ref int position) {
			int positionIndex = destination.BinarySearch(position);
			if (positionIndex >= 0)
				return 0;
			positionIndex = ~positionIndex;
			destination.Insert(positionIndex, position);
			return 0;
		}
		public void UpdateLayout(Table table) {
			ApplySpans(table);
			VerticalMergingUpdater.ApplyVerticalMerging(table);
		}
		void ApplySpans(Table table) {
			List<int> cellPositions;
			if (!TableCellPositions.TryGetValue(table, out cellPositions))
				cellPositions = new List<int>();
			int rowCount = table.Rows.Count;
			int minWidthBefore = CalcMinWidthBefore(table);
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableRow currentRow = table.Rows[rowIndex];
				List<int> rowCellWidths;
				if (!RowCellWidths.TryGetValue(currentRow, out rowCellWidths))
					rowCellWidths = new List<int>();
				CorrectCellWidthsAndSpans(currentRow, cellPositions, rowCellWidths, minWidthBefore);
				List<TableCellWidthOperand> preferredCellWidths;
				if (PreferredCellWidths.TryGetValue(currentRow, out preferredCellWidths))
					ApplyPreferredWidths(currentRow, preferredCellWidths);
			}
		}
		int CalcMinWidthBefore(Table table) {
			int result = Int32.MaxValue;
			TableRowCollection rows = table.Rows;
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				result = Math.Min(result, rows[i].WidthBefore.Value);
			return result;
		}
		void CorrectCellWidthsAndSpans(TableRow row, List<int> cellPositions, List<int> cellWidths, int minWidthBefore) {
			TableRowProperties rowProperties = row.Properties;
			row.UnsubscribeRowPropertiesEvents();			
			try {
					int span = CalcColumnSpan(cellPositions, 0, rowProperties.WidthBefore.Value - minWidthBefore);
					if (span != row.GridBefore)
						row.GridBefore = span;
					int currentPosition = rowProperties.WidthBefore.Value;
					currentPosition = ProcessRowCells(row, cellPositions, cellWidths, currentPosition);
					span = CalcColumnSpan(cellPositions, currentPosition, cellPositions[cellPositions.Count - 1] - currentPosition);
					if (span != row.GridAfter)
						row.GridAfter = span;
			}
			finally {
				row.SubscribeRowPropertiesEvents();
			}
		}
		int ProcessRowCells(TableRow row, List<int> cellPositions, List<int> cellWidths, int currentPosition) {
			int count = row.Cells.Count;
			if (count == cellWidths.Count)
				return ApplyActualColumnSpans(row, cellPositions, cellWidths, currentPosition);
			else
				return ApplyDefaultColumnSpans(row, cellPositions, cellWidths, currentPosition);
		}
		int ApplyActualColumnSpans(TableRow row, List<int> cellPositions, List<int> cellWidths, int currentPosition) {
			int count = cellWidths.Count;
			for (int cellIndex = 0; cellIndex < count; cellIndex++) {
				int currentWidth = cellWidths[cellIndex];
				TableCell currentCell = row.Cells[cellIndex];
				currentCell.UnsubscribeCellPropertiesEvents();
				try {
					currentCell.ColumnSpan = Math.Max(1, CalcColumnSpan(cellPositions, currentPosition, currentWidth));
					currentCell.Properties.PreferredWidth.Value = UnitConverter.TwipsToModelUnits(currentWidth);
					currentCell.Properties.PreferredWidth.Type = WidthUnitType.ModelUnits;
				}
				finally {
					currentCell.SubscribeCellPropertiesEvents();
				}
				currentPosition += currentWidth;
			}			
			return currentPosition;
		}
		int ApplyDefaultColumnSpans(TableRow row, List<int> cellPositions, List<int> cellWidths, int currentPosition) {
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				TableCell currentCell = cells[i];
				currentCell.UnsubscribeCellPropertiesEvents();
				try {
					currentCell.ColumnSpan = 1;
					currentCell.Properties.PreferredWidth.Type = WidthUnitType.Auto;
					currentCell.Properties.PreferredWidth.Value = 0;
				}
				finally {
					currentCell.SubscribeCellPropertiesEvents();
				}
			}
			count = cellWidths.Count;
			for (int i = 0; i < count; i++)
				currentPosition += cellWidths[i];
			return currentPosition;
		}
		int CalcColumnSpan(List<int> cellPositions, int currentPosition, int currentWidth) {
			int startPositionIndex = cellPositions.BinarySearch(currentPosition);
			if (startPositionIndex < 0)
				startPositionIndex = ~startPositionIndex;
			int endPositionIndex = cellPositions.BinarySearch(currentPosition + currentWidth);
			if (endPositionIndex < 0)
				endPositionIndex = ~endPositionIndex;
			return endPositionIndex - startPositionIndex;
		}
		void ApplyPreferredWidths(TableRow row, List<TableCellWidthOperand> preferredCellWidths) {
			int count = preferredCellWidths.Count;
			for (int i = 0; i < count; i++)
				ApplyPreferredWidthsCore(row, preferredCellWidths[i]);
		}
		void ApplyPreferredWidthsCore(TableRow row, TableCellWidthOperand operand) {
			int start = Math.Max(0, (int)operand.StartIndex);
			int end = Math.Min(row.Cells.Count - 1, operand.EndIndex);
			for (int i = start; i <= end; i++) {
				PreferredWidth preferredWidth = row.Cells[i].Properties.PreferredWidth;
				preferredWidth.Type = operand.WidthUnit.Type;
				preferredWidth.Value = operand.WidthUnit.Value;
			}
		}
		void ApplyTablePropertiesException(TableRow row, TableRowInfo info) {
			if (info == null || info.TableAutoformatLookSpecifier == null)
				return;
			info.TableAutoformatLookSpecifier.ApplyProperties(row.TablePropertiesException);
		}
	}
	#endregion
	#region VerticalMergingStateUpdater
	public class VerticalMergingStateUpdater {
		#region Fields
		Dictionary<TableRow, List<VerticalMergeInfo>> verticalMergedCells;
		#endregion
		public VerticalMergingStateUpdater() {
			this.verticalMergedCells = new Dictionary<TableRow, List<VerticalMergeInfo>>();
		}
		#region Properties
		protected Dictionary<TableRow, List<VerticalMergeInfo>> VerticalMergedCells { get { return this.verticalMergedCells; } }
		#endregion
		public void UpdateCellsVerticalMergeState(TableRow row, DocPropertyContainer propertyContainer) {
			AddVerticalMergedCells(row, propertyContainer);
			ApplyCellVerticalAlignmentActions(row, propertyContainer);
		}
		public void ApplyVerticalMerging(Table table) {
			int rowCount = table.Rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableRow currentRow = table.Rows[rowIndex];
				List<VerticalMergeInfo> verticalMergeInfo;
				if (!VerticalMergedCells.TryGetValue(currentRow, out verticalMergeInfo))
					continue;
				int mergedCellsCount = verticalMergeInfo.Count;
				for (int infoIndex = 0; infoIndex < mergedCellsCount; infoIndex++) {
					VerticalMergeInfo currentInfo = verticalMergeInfo[infoIndex];
					if (currentInfo.CellIndex >= currentRow.Cells.Count)
						continue;
					currentRow.Cells[currentInfo.CellIndex].VerticalMerging = currentInfo.MergingState;
				}
			}
		}
		void AddVerticalMergedCells(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableRowInfo == null)
				return;
			List<VerticalMergeInfo> legacyMergeInfo = GetVerticalMergingInfo(row);
			List<VerticalMergeInfo> mergeInfo = propertyContainer.TableRowInfo.VerticalMerging;
			int count = mergeInfo.Count;
			for (int i = 0; i < count; i++) {
				legacyMergeInfo.Add(mergeInfo[i]);
			}
		}
		void ApplyCellVerticalAlignmentActions(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableRowInfo == null)
				return;
			int count = propertyContainer.TableRowInfo.CellRangeVerticalAlignmentActions.Count;
			for (int i = 0; i < count; i++) {
				CellRangeVerticalAlignmentOperand currentVerticalAlignmentAction = propertyContainer.TableRowInfo.CellRangeVerticalAlignmentActions[i];
				for (int cellIndex = currentVerticalAlignmentAction.StartIndex; cellIndex <= currentVerticalAlignmentAction.EndIndex; cellIndex++) {
					row.Cells[cellIndex].Properties.VerticalAlignment = currentVerticalAlignmentAction.VerticalAlignment;
				}
			}
		}
		List<VerticalMergeInfo> GetVerticalMergingInfo(TableRow row) {
			List<VerticalMergeInfo> result;
			if (!VerticalMergedCells.TryGetValue(row, out result)) {
				result = new List<VerticalMergeInfo>();
				VerticalMergedCells.Add(row, result);
			}
			return result;
		}
	}
	#endregion
	#region TableCellPropertiesUpdater
	public static class TableCellPropertiesUpdater {
		public static void UpdateTableRow(TableRow row, DocPropertyContainer propertyContainer) {
			ApplyCellMargins(row, propertyContainer);
			ApplyOldCellBackgroundColors(row, propertyContainer);
			ApplyCellBackgroundColors(row, propertyContainer);
			ApplyCellBorders(row, propertyContainer);
		}
		static void ApplyCellBorders(TableRow row, DocPropertyContainer propertyContainer) {
			TableRowInfo rowInfo = propertyContainer.TableRowInfo;
			TableInfo tableInfo = propertyContainer.TableInfo;
			if (rowInfo == null)
				return;
			int count = propertyContainer.TableRowInfo.OverrideCellBordersActions.Count;
			for (int i = 0; i < count; i++) {
				TableBordersOverrideOperand operand = propertyContainer.TableRowInfo.OverrideCellBordersActions[i];
				int start = Math.Max(operand.StartIndex, 0);
				int end = Math.Min(operand.EndIndex, row.Cells.Count - 1);
				for (int cellIndex = start; cellIndex <= end; cellIndex++)
					operand.ApplyProperties(row.Cells[cellIndex].Properties.Borders, propertyContainer.UnitConverter);
			}
			if (count > 0)
				return;
			ApplyBorderColors(row, rowInfo.TopBorders, DocTableCellBorders.Top, tableInfo, propertyContainer.UnitConverter);
			ApplyBorderColors(row, rowInfo.LeftBorders, DocTableCellBorders.Left, tableInfo, propertyContainer.UnitConverter);
			ApplyBorderColors(row, rowInfo.RightBorders, DocTableCellBorders.Right, tableInfo, propertyContainer.UnitConverter);
			ApplyBorderColors(row, rowInfo.BottomBorders, DocTableCellBorders.Bottom, tableInfo, propertyContainer.UnitConverter);
		}
		static void ApplyBorderColors(TableRow row, List<DocTableBorderColorReference> colors, DocTableCellBorders type, TableInfo tableInfo, DocumentModelUnitConverter unitConverter) {
			List<TableCellDescriptor> cells = (tableInfo != null && tableInfo.TableDefinition != null) ? tableInfo.TableDefinition.Cells 
				: new List<TableCellDescriptor>();
			int count = Math.Min(colors.Count, row.Cells.Count);
			for (int i = 0; i < count; i++) {
				BorderBase border = GetModelBorder(row.Cells[i].Properties.Borders, type);
				border.BeginUpdate();
				try {
#if !SL
					if (colors[i].Color.IsEmpty)
						continue;
#endif
					border.Color = colors[i].Color;
					if (cells.Count <= i)
						continue;
					BorderDescriptor97 descriptor = GetBorderDescriptor(cells[i], type);
					if (descriptor != null) {
						border.Style = DocBorderCalculator.MapToBorderLineStyle(descriptor.Style);
						border.Width = unitConverter.TwipsToModelUnits(descriptor.Width);
					}
				}
				finally {
					border.EndUpdate();
				}
			}
		}
		static BorderBase GetModelBorder(TableCellBorders borders, DocTableCellBorders type) {
			switch (type) {
				case DocTableCellBorders.Top: return borders.TopBorder;
				case DocTableCellBorders.Left: return borders.LeftBorder;
				case DocTableCellBorders.Bottom: return borders.BottomBorder;
				case DocTableCellBorders.Right: return borders.RightBorder;
				case DocTableCellBorders.TopLeftToBottomRight: return borders.TopLeftDiagonalBorder;
				case DocTableCellBorders.TopRightToBottomLeft: return borders.TopRightDiagonalBorder;
				default: return null;
			}
		}
		static BorderDescriptor97 GetBorderDescriptor(TableCellDescriptor cell, DocTableCellBorders type) {
			switch (type) {
				case DocTableCellBorders.Top: return cell.TopBorder;
				case DocTableCellBorders.Left: return cell.LeftBorder;
				case DocTableCellBorders.Bottom: return cell.BottomBorder;
				case DocTableCellBorders.Right: return cell.RightBorder;
				default: return null;
			}
		}
		static void ApplyCellMargins(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableRowInfo == null)
				return;
			int count = propertyContainer.TableRowInfo.CellMarginsActions.Count;
			for (int i = 0; i < count; i++) {
				CellSpacingOperand operand = propertyContainer.TableRowInfo.CellMarginsActions[i];
				int start = Math.Max(operand.StartIndex, 0);
				int end = Math.Min(operand.EndIndex, row.Cells.Count - 1);
				for (int cellIndex = start; cellIndex <= end; cellIndex++)
					operand.ApplyProperties(row.Cells[cellIndex].Properties.CellMargins, propertyContainer.UnitConverter);
			}
		}
		static void ApplyOldCellBackgroundColors(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableRowInfo == null)
				return;
			ApplyOldCellBackgroundColorsCore(row, propertyContainer.TableRowInfo.DefaultCellsShading, 0);
			ApplyOldCellBackgroundColorsCore(row, propertyContainer.TableRowInfo.CellShading1, 0);
			ApplyOldCellBackgroundColorsCore(row, propertyContainer.TableRowInfo.CellShading2, 22);
			ApplyOldCellBackgroundColorsCore(row, propertyContainer.TableRowInfo.CellShading3, 44);
		}
		static void ApplyOldCellBackgroundColorsCore(TableRow row, List<Color> list, int firstCellIndex) {
			TableCellCollection cells = row.Cells;
			for (int i = 0; i < list.Count; i++) {
				if(!DXColor.IsEmpty(list[i]))
					cells[i + firstCellIndex].BackgroundColor = list[i];
			}
		}
		static void ApplyCellBackgroundColors(TableRow row, DocPropertyContainer propertyContainer) {
			if (propertyContainer.TableCellInfo == null)
				return;
			int count = propertyContainer.TableCellInfo.CellColors.Count;
			for (int i = 0; i < count; i++)
				if (propertyContainer.TableCellInfo.CellColors[i] != DXColor.Empty)
					row.Cells[i].BackgroundColor = propertyContainer.TableCellInfo.CellColors[i];
		}
	}
#endregion
}
