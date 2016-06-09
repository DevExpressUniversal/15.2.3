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
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Forms {
	#region TablePropertiesFormControllerParameters
	public class TablePropertiesFormControllerParameters : FormControllerParameters {
		readonly SelectedCellsCollection selectedCells;
		internal TablePropertiesFormControllerParameters(IRichEditControl control, SelectedCellsCollection selectedCells)
			: base(control) {
			Guard.ArgumentNotNull(selectedCells, "selectedCells");
			this.selectedCells = selectedCells;
		}
		internal SelectedCellsCollection SelectedCells { get { return selectedCells; } }
	}
	#endregion
	#region TablePropertiesFormController
	public class TablePropertiesFormController : FormController {
		#region Fields
		readonly SelectedCellsCollection sourceSelectedCells;
		bool useDefaultTableWidth;
		int tableWidth;
		WidthUnitType tableWidthUnitType;
		TableRowAlignment? tableAlignment;
		int tableIndent;
		bool? useDefaultRowHeight;
		int rowHeight;
		HeightUnitType rowHeightType;
		bool? rowCantSplit;
		bool? rowHeader;
		bool? useDefaultColumnWidth;
		int columnWidth;
		WidthUnitType columnWidthUnitType;
		bool? useDefaultCellWidth;
		int cellWidth;
		WidthUnitType cellWidthUnitType;
		WidthUnitInfo oldCellWidth;
		VerticalAlignment? cellVerticalAlignment;
		int pageFirstColumnWidth;
		List<TableCell> cellsInSelectedColumns;
		List<TableCell> selectedCells;
		#endregion
		public TablePropertiesFormController(TablePropertiesFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceSelectedCells = controllerParameters.SelectedCells;
			this.selectedCells = GetSelectedCells1();
			this.cellsInSelectedColumns = GetCellsInSelectedColumns();
			InitializeController();
			this.pageFirstColumnWidth = CalculateColumnWidth();
		}
		#region Properties 
		public SelectedCellsCollection SourceSelectedCells { get { return sourceSelectedCells; } }
		public bool UseDefaultTableWidth { get { return useDefaultTableWidth; } set { useDefaultTableWidth = value; } }
		public int TableWidth { get { return tableWidth; } set { tableWidth = value; } }
		public WidthUnitType TableWidthUnitType { get { return tableWidthUnitType; } set { tableWidthUnitType = value; } }
		public TableRowAlignment? TableAlignment { get { return tableAlignment; } set { tableAlignment = value; } }
		public int TableIndent { get { return tableIndent; } set { tableIndent = value; } }
		public bool? UseDefaultRowHeight { get { return useDefaultRowHeight; } set { useDefaultRowHeight = value; } }
		public int RowHeight { get { return rowHeight; } set { rowHeight = value; } }
		public HeightUnitType RowHeightType { get { return rowHeightType; } set { rowHeightType = value; } }
		public bool? RowCantSplit { get { return rowCantSplit; } set { rowCantSplit = value; } }
		public bool? RowHeader { get { return rowHeader; } set { rowHeader = value; } }
		public bool? UseDefaultColumnWidth { get { return useDefaultColumnWidth; } set { useDefaultColumnWidth = value; } }
		public int ColumnWidth { get { return columnWidth; } set { columnWidth = value; } }
		public WidthUnitType ColumnWidthUnitType { get { return columnWidthUnitType; } set { columnWidthUnitType = value; } }
		public bool? UseDefaultCellWidth { get { return useDefaultCellWidth; } set { useDefaultCellWidth = value; } }
		public int CellWidth { get { return cellWidth; } set { cellWidth = value; } }
		public WidthUnitType CellWidthUnitType { get { return cellWidthUnitType; } set { cellWidthUnitType = value; } }
		public VerticalAlignment? CellVerticalAlignment { get { return cellVerticalAlignment; } set { cellVerticalAlignment = value; } }
		protected internal Table Table { get { return sourceSelectedCells.FirstSelectedCell.Table; } }
		protected internal TableRowCollection Rows { get { return Table.Rows; } }
		protected internal int PageFirstColumnWidth { get { return pageFirstColumnWidth; } }
		protected internal virtual List<TableCell> SelectedCells { get { return selectedCells; } }
		#endregion
		protected internal virtual void InitializeController() {
			InitializeTableTab();
			InitializeRowTab();
			InitializeColumnTab();
			InitializeCellTab();
		}
		void InitializeTableTab() {
			WidthUnit tableWidth = Table.PreferredWidth;
			UseDefaultTableWidth = tableWidth.Type == WidthUnitType.Auto || tableWidth.Type == WidthUnitType.Nil;
			TableWidth = ModelUnitToPercent(tableWidth);
			TableWidthUnitType = tableWidth.Type == WidthUnitType.FiftiethsOfPercent ? WidthUnitType.FiftiethsOfPercent : WidthUnitType.ModelUnits;
			TableAlignment = GetTableAlignment();
			TableIndent = GetTableIndent();
		}
		protected internal virtual int ModelUnitToPercent(WidthUnit width) {
			if (width.Type == WidthUnitType.FiftiethsOfPercent)
				return width.Value / 50;
			return width.Value;
		}
		protected internal virtual bool IsSelectedFirstRowInTable() {
			List<TableRow> selectedRows = sourceSelectedCells.GetSelectedTableRows();
			int selectedRowsCount = selectedRows.Count;
			for (int i = 0; i < selectedRowsCount; i++) {
				if (selectedRows[i].IsFirstRowInTable)
					return true;
			}
			return false;
		}
		TableRowAlignment? GetTableAlignment() {
			int rowsCount = Rows.Count;
			TableRowAlignment firstRowAlignment = Rows.First.TableRowAlignment;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = Rows[i];
				if (firstRowAlignment != currentRow.TableRowAlignment)
					return null;
			}
			return firstRowAlignment;
		}
		int GetTableIndent() {
			WidthUnit tableIndent = Table.TableIndent;
			return tableIndent.Type == WidthUnitType.ModelUnits ? tableIndent.Value : 0;
		}
		void InitializeRowTab() {
			List<TableRow> selectedRows = sourceSelectedCells.GetSelectedTableRows();
			bool identicalCantSplit = true;
			bool identicalHeader = true;
			bool identicalRowHeight = true;
			TableRow firstSelectedRow = selectedRows[0];
			bool firstRowCantSplit = firstSelectedRow.CantSplit;
			bool firstRowHeader = firstSelectedRow.Header;
			HeightUnit firstRowHeight = firstSelectedRow.Height;
			int selectedRowsCount = selectedRows.Count;
			for (int i = 1; i < selectedRowsCount; i++) {
				TableRow currentRow = selectedRows[i];
				identicalCantSplit &= firstRowCantSplit == currentRow.CantSplit;
				identicalHeader &= firstRowHeader == currentRow.Header;
				identicalRowHeight &= EqualsHeightUnits(firstRowHeight, currentRow.Height);
			}
			UseDefaultRowHeight = identicalRowHeight ? firstRowHeight.Value == 0 : (bool?)null;
			RowHeight = identicalRowHeight ? firstRowHeight.Value : 0;
			RowHeightType = firstRowHeight.Type == HeightUnitType.Exact ? HeightUnitType.Exact : HeightUnitType.Minimum;
			RowCantSplit = identicalCantSplit ? firstRowCantSplit : (bool?)null;
			RowHeader = identicalHeader ? firstRowHeader : (bool?)null;
		}
		bool EqualsHeightUnits(HeightUnit value1, HeightUnit value2) {
			return value1.Value == value2.Value && value1.Type == value2.Type;
		}
		void InitializeColumnTab() {
			bool identicalCellWidth = true;
			WidthUnit firstCellWidth = cellsInSelectedColumns[0].PreferredWidth;
			int cellsInSelectedColumnsCount = cellsInSelectedColumns.Count;
			for (int i = 1; i < cellsInSelectedColumnsCount; i++) {
				TableCell currentCell = cellsInSelectedColumns[i];
				identicalCellWidth &= EqualsWidthUnit(firstCellWidth, currentCell.PreferredWidth);
			}
			UseDefaultColumnWidth = identicalCellWidth ? firstCellWidth.Type == WidthUnitType.Auto : (bool?)null;
			ColumnWidth = ModelUnitToPercent(firstCellWidth);
			ColumnWidthUnitType = firstCellWidth.Type;
		}
		void InitializeCellTab() {
			bool identicalCellWidth = true;
			bool identicalVerticalAlignment = true;
			TableCell firstSelectedCell = SelectedCells[0];
			WidthUnit firstCellWidth = firstSelectedCell.PreferredWidth;
			VerticalAlignment firstCellVerticalAlignment = firstSelectedCell.VerticalAlignment;
			int selectedCellsCount = SelectedCells.Count;
			for (int i = 1; i < selectedCellsCount; i++) {
				TableCell currentCell = SelectedCells[i];
				identicalCellWidth &= EqualsWidthUnit(firstCellWidth, currentCell.PreferredWidth);
				identicalVerticalAlignment &= firstCellVerticalAlignment == currentCell.VerticalAlignment;
			}
			UseDefaultCellWidth = identicalCellWidth ? firstCellWidth.Type == WidthUnitType.Auto : (bool?)null;
			CellWidth = ModelUnitToPercent(firstCellWidth);
			CellWidthUnitType = firstCellWidth.Type;
			this.oldCellWidth = new WidthUnitInfo(CellWidthUnitType, CellWidth);
			CellVerticalAlignment = identicalVerticalAlignment ? firstCellVerticalAlignment : (VerticalAlignment?)null;
		}
		bool EqualsWidthUnit(WidthUnit value1, WidthUnit value2) {
			return value1.Value == value2.Value && value1.Type == value2.Type;
		}
		protected internal virtual List<TableCell> GetSelectedCells1() {
			List<TableCell> result = new List<TableCell>();
			int rowsCount = sourceSelectedCells.RowsCount;
			for (int i = 0; i < rowsCount; i++) {
				SelectedCellsIntervalInRow currentInterval = sourceSelectedCells[i];
				int endCellIndex = currentInterval.NormalizedEndCellIndex;
				TableCellCollection cells = currentInterval.Row.Cells;
				for (int j = currentInterval.NormalizedStartCellIndex; j <= endCellIndex; j++) {
					result.Add(cells[j]);
				}
			}
			return result;
		}
		int CalculateColumnWidth() {
			TableCell firstSelectedCell = sourceSelectedCells.FirstSelectedCell;
			DocumentModel documentModel = firstSelectedCell.DocumentModel;
			ParagraphIndex firstCellStartParagraphIndex = firstSelectedCell.StartParagraphIndex;
			DocumentLogPosition firstCellLogPosition = documentModel.ActivePieceTable.Paragraphs[firstCellStartParagraphIndex].LogPosition;
			SectionIndex sectionIndex = documentModel.FindSectionIndex(firstCellLogPosition);
			Section section = documentModel.Sections[sectionIndex];
			DocumentModelDocumentsToLayoutDocumentsConverter converter = new DocumentModelDocumentsToLayoutDocumentsConverter();
			PageBoundsCalculator pageBoundsCalculator = new PageBoundsCalculator(converter);
			Rectangle pageClientBounds = pageBoundsCalculator.CalculatePageClientBounds(section);
			ColumnsBoundsCalculator columnsBoundsCalculator = new ColumnsBoundsCalculator(converter);
			List<Rectangle> columnWidthsCollection = columnsBoundsCalculator.Calculate(section, pageClientBounds);
			return columnWidthsCollection[0].Width;
		}
		List<TableCell> GetCellsInSelectedColumns() {
			SelectedCellsIntervalInRow firstSelectedRow = sourceSelectedCells.First;
			int startColumnIndex = firstSelectedRow.NormalizedStartCell.GetStartColumnIndexConsiderRowGrid();
			int endColumnIndex = firstSelectedRow.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid();
			List<TableCell> result = new List<TableCell>();
			int topSelectedRowIndex = sourceSelectedCells.GetTopRowIndex();
			int rowsCount = Rows.Count;
			for (int i = topSelectedRowIndex; i < rowsCount; i++) {
				TableRow currentRow = Rows[i];
				List<TableCell> cells = TableCellVerticalBorderCalculator.GetCellsByIntervalColumnIndex(currentRow, startColumnIndex, endColumnIndex);
				result.AddRange(cells);
			}
			return result;
		}
		public override void ApplyChanges() {
			DocumentModel documentModel = Table.DocumentModel;
			documentModel.BeginUpdate();
			try {
				ApplyChangesCore();
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void ApplyChangesCore() {
			ApplyTableProperties();
			ApplyRowProperties();
			ApplyColumnProperties();
			ApplyCellProperties();
		}
		protected internal virtual void ApplyTableProperties() {
			ApplyTableWidth();
			ApplyTableAlignment();
			ApplyTableIndent();
		}
		void ApplyTableWidth() {
			WidthUnitInfo newTableWidth = GetActualWidth(UseDefaultTableWidth, TableWidth, TableWidthUnitType);
			if (!EqualsWidthUnit(newTableWidth, Table.PreferredWidth))
				Table.TableProperties.PreferredWidth.CopyFrom(newTableWidth);
		}
		protected internal virtual WidthUnitInfo GetActualWidth(bool? useDefaultValue, int width, WidthUnitType widthUnitType) {
			if (!useDefaultValue.HasValue)
				return null;
			if (useDefaultValue.Value)
				return new WidthUnitInfo(WidthUnitType.Auto, 0);
			return new WidthUnitInfo(widthUnitType, PercentToModelUnit(width, widthUnitType));
		}
		protected internal virtual int PercentToModelUnit(int width, WidthUnitType type) {
			if (type == WidthUnitType.FiftiethsOfPercent)
				return width * 50;
			return width;
		}
		void ApplyTableAlignment() {
			if (!TableAlignment.HasValue)
				return;
			if (Table.TableAlignment != TableAlignment)
				Table.TableProperties.TableAlignment = TableAlignment.Value;
			int rowsCount = Rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				TableRow currentRow = Rows[i];
				if (currentRow.TableRowAlignment != TableAlignment)
					currentRow.Properties.TableRowAlignment = TableAlignment.Value;
			}
		}
		bool EqualsWidthUnit(WidthUnitInfo value1, WidthUnit value2) {
			if (value1 == null || value2 == null)
				return false;
			if (value2.Type == WidthUnitType.Nil && value1.Value == value2.Value)
				return true;
			return value1.Type == value2.Type && value1.Value == value2.Value;
		}
		void ApplyTableIndent() {
			WidthUnitInfo newTableIndent = null;
			if (!TableAlignment.HasValue || TableAlignment.Value != TableRowAlignment.Left)
				newTableIndent = new WidthUnitInfo(WidthUnitType.ModelUnits, 0);
			else
				newTableIndent = new WidthUnitInfo(WidthUnitType.ModelUnits, TableIndent);
			if (!EqualsWidthUnit(newTableIndent, Table.TableIndent))
				Table.TableProperties.TableIndent.CopyFrom(newTableIndent);
		}
		protected internal virtual void ApplyRowProperties() {
			HeightUnitInfo newRowHeight = GetActualHeight(UseDefaultRowHeight, RowHeight, RowHeightType);
			List<TableRow> selectedRows = sourceSelectedCells.GetSelectedTableRows();
			int selectedRowsCount = selectedRows.Count;
			for (int i = 0; i < selectedRowsCount; i++) {
				TableRow currentRow = selectedRows[i];
				TableRowProperties currentRowProperties = currentRow.Properties;
				if (newRowHeight != null && !EqualsHeightUnits(newRowHeight, currentRow.Height))
					currentRow.Properties.Height.CopyFrom(newRowHeight);
				if (RowCantSplit.HasValue && currentRow.CantSplit != RowCantSplit)
					currentRowProperties.CantSplit = RowCantSplit.Value;
				if (RowHeader.HasValue && currentRow.Header != RowHeader)
					currentRowProperties.Header = RowHeader.Value;
			}
		}
		protected internal virtual HeightUnitInfo GetActualHeight(bool? useDefaultValue, int height, HeightUnitType heightUnitType) {
			if (!useDefaultValue.HasValue)
				return null;
			if (useDefaultValue.Value)
				return new HeightUnitInfo(0, HeightUnitType.Auto);
			return new HeightUnitInfo(height, heightUnitType);
		}
		protected internal virtual bool EqualsHeightUnits(HeightUnitInfo value1, HeightUnit value2) {
			return value1.Value == value2.Value && value1.Type == value2.Type;
		}
		protected internal virtual void ApplyColumnProperties() {
			WidthUnitInfo newCellWidth = GetActualWidth(UseDefaultColumnWidth, ColumnWidth, ColumnWidthUnitType);
			int cellsInSelectedColumnsCount = cellsInSelectedColumns.Count;
			for (int i = 0; i < cellsInSelectedColumnsCount; i++) {
				TableCell currentCell = cellsInSelectedColumns[i];
				if (newCellWidth != null && !EqualsWidthUnit(newCellWidth, currentCell.PreferredWidth))
					currentCell.Properties.PreferredWidth.CopyFrom(newCellWidth);
			}
		}
		protected internal virtual void ApplyCellProperties() {
			WidthUnitInfo newCellWidth = GetActualWidth(UseDefaultCellWidth, CellWidth, CellWidthUnitType);
			int selectedCellsCount = SelectedCells.Count;
			for (int i = 0; i < selectedCellsCount; i++) {
				TableCell currentCell = SelectedCells[i];
				if (newCellWidth != null && !EqualsWidthUnit(newCellWidth, currentCell.PreferredWidth) && !newCellWidth.Equals(this.oldCellWidth))
					currentCell.Properties.PreferredWidth.CopyFrom(newCellWidth);
				if (CellVerticalAlignment.HasValue && currentCell.VerticalAlignment != CellVerticalAlignment)
					currentCell.Properties.VerticalAlignment = CellVerticalAlignment.Value;
			}
		}
		protected internal virtual int GetTableWidthMaxValueConsiderWidthUnitType() {
			if (TableWidthUnitType == WidthUnitType.FiftiethsOfPercent)
				return TablePropertiesFormDefaults.MaxTableWidthInPercentByDefault;
			return TablePropertiesFormDefaults.MaxTableWidthInModelUnitsByDefault;
		}
		protected internal virtual int GetColumnWidthMaxValueConsiderWidthUnitType() {
			if (ColumnWidthUnitType == WidthUnitType.FiftiethsOfPercent)
				return TablePropertiesFormDefaults.MaxColumnWidthInPercentByDefault;
			return TablePropertiesFormDefaults.MaxColumnWidthInModelUnitsByDefault;
		}
		protected internal virtual int GetCellWidthMaxValueConsiderWidthUnitType() {
			if (CellWidthUnitType == WidthUnitType.FiftiethsOfPercent)
				return TablePropertiesFormDefaults.MaxCellWidthInPercentByDefault;
			return TablePropertiesFormDefaults.MaxCellWidthInModelUnitsByDefault;
		}
	}
	#endregion
	#region TablePropertiesFormDefaults
	public static class TablePropertiesFormDefaults {
		public const int MinTableIndentByDefault = -15 * 1440;
		public const int MaxTableIndentByDefault = 15 * 1440;
		public const int MinTableWidthByDefault = 0;
		public const int MaxTableWidthInModelUnitsByDefault = 22 * 1440;
		public const int MaxTableWidthInPercentByDefault = 600;
		public const int MinRowHeightByDefault = 0;
		public const int MaxRowHeightByDefault = 22 * 1440;
		public const int MinColumnWidthByDefault = 0;
		public const int MaxColumnWidthInModelUnitsByDefault = 22 * 1440;
		public const int MaxColumnWidthInPercentByDefault = 100;
		public const int MinCellWidthByDefault = 0;
		public const int MaxCellWidthInModelUnitsByDefault = 22 * 1440;
		public const int MaxCellWidthInPercentByDefault = 100;
	}
	#endregion
}
