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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Utils;
using LayoutUnit = System.Int32;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfTableExporter
	public class RtfTableExporter {
		readonly RtfContentExporter exporter;
		bool exportAsNestedTable;
		public RtfTableExporter(RtfContentExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		public RtfContentExporter RtfExporter { get { return exporter; } }
		public bool ExportAsNestedTable { get { return exportAsNestedTable; } set { exportAsNestedTable = value; } }
		public ParagraphIndex Export(Table table) {
			RtfTableExporterStateBase tableExporterState = CreateExporterState(table);
			tableExporterState.Export();
			return tableExporterState.TableHelper.GetLastParagraphIndex();
		}
		protected virtual RtfTableExporterStateBase CreateExporterState(Table table) {
			if (GetNestingLevel(table) > 1) {
				Table rootTable = GetRootTable(table);
				if (ExportAsNestedTable)
					return new RtfNestedTableExporterState(RtfExporter, rootTable, 2);
				else
					return new RtfTableStartingWithNestedTableExporterState(RtfExporter, rootTable);
			}
			else {
				if (ExportAsNestedTable)
					return new RtfNestedTableExporterState(RtfExporter, table, 2);
				else
					return new RtfTableExporterState(RtfExporter, table);
			}
		}
		Table GetRootTable(Table table) {
			while (table.ParentCell != null)
				table = table.ParentCell.Table;
			return table;
		}
		int GetNestingLevel(Table table) {
			int result = 1;
			TableCell parentCell = table.ParentCell;
			while (parentCell != null) {
				Debug.Assert(parentCell.Row != null);
				Debug.Assert(parentCell.Row.Table != null);
				Table parentTable = parentCell.Row.Table;
				parentCell = parentTable.ParentCell;
				result++;
			}
			return result;
		}
	}
	#endregion
	#region RtfTableWidthsCalculator
	public class RtfTableWidthsCalculator : SimpleTableWidthsCalculator {
		public RtfTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter)
			: base(converter) {
		}
		public RtfTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter, int percentBaseWidth)
			: base(converter, percentBaseWidth) {
		}
	}
	#endregion
	#region RtfTableExportHelper
	public class RtfTableExportHelper {
		#region Fields
		const int RootTableNestingLevel = 1;
		readonly Table table;
		int width = -1;
		TableGrid grid;
		#endregion
		public RtfTableExportHelper(Table table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		#region Properties
		public Table Table { get { return table; } }
		public int Width {
			get {
				if (width == -1)
					width = GetTableWidth();
				return width;
			}
		}
		public TableGrid Grid {
			get {
				if (grid == null)
					grid = GetTableGrid();
				return grid;
			}
		}
		#endregion
		public int GetCellWidth(int leftSideIndex, int columnSpan) {
			if (leftSideIndex < 0 || columnSpan <= 0)
				Exceptions.ThrowInternalException();
			int result = 0;
			for (int i = 0; i < columnSpan; i++)
				result += Math.Max(Grid[i + leftSideIndex].Width, 1);
			return result;
		}
		public ParagraphIndex GetLastParagraphIndex() {
			TableCell lastCell = Table.Rows.Last.Cells.Last;
			return lastCell.EndParagraphIndex;
		}
		protected virtual TableGrid GetTableGrid() {
			DocumentModel documentModel = Table.DocumentModel;
			RtfTableWidthsCalculator widthsCalculator = new RtfTableWidthsCalculator(documentModel.ToDocumentLayoutUnitConverter);
			TableGridCalculator calculator = new TableGridCalculator(documentModel, widthsCalculator, Int32.MaxValue);
			return calculator.CalculateTableGrid(Table, documentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(RtfTableWidthsCalculator.DefaultPercentBaseWidthInTwips));
		}
		int GetTableWidth() {
			int result = 0;
			TableGridColumnCollection columns = Grid.Columns;
			int colCount = columns.Count;
			for (int i = 0; i < colCount; i++)
				result += columns[i].Width;
			return result;
		}
	}
	#endregion
	#region RtfTableExporterStateBase (abstract class)
	public abstract class RtfTableExporterStateBase {
		#region Fields
		readonly RtfContentExporter rtfExporter;
		readonly RtfTableExportHelper tableHelper;
		int nestingLevel;
		int tableStyleIndex;
		RtfTableRowPropertiesExporter tableRowPropertiesExporter;
		RtfTableCellPropertiesExporter tableCellPropertiesExporter;
		RtfTablePropertiesExporter tablePropertiesExporter;
		#endregion
		protected RtfTableExporterStateBase(RtfContentExporter rtfExporter, Table table, int nestingLevel) {
			Guard.ArgumentNotNull(rtfExporter, "rtfExporter");
			Guard.ArgumentNotNull(table, "tableHelper");
			this.rtfExporter = rtfExporter;
			this.nestingLevel = nestingLevel;
			this.tableHelper = CreateTableHelper(table);
			tableRowPropertiesExporter = new RtfTableRowPropertiesExporter(DocumentModel, RtfContentExporter.RtfExportHelper, RtfBuilder);
			tableCellPropertiesExporter = new RtfTableCellPropertiesExporter(DocumentModel, RtfContentExporter.RtfExportHelper, RtfBuilder);
			tablePropertiesExporter = new RtfTablePropertiesExporter(DocumentModel, RtfContentExporter.RtfExportHelper, RtfBuilder);
			tableStyleIndex = GetTableStyleIndex();
		}
		#region Properties
		protected RtfContentExporter RtfContentExporter { get { return rtfExporter; } }
		protected RtfBuilder RtfBuilder { get { return RtfContentExporter.RtfBuilder; } }
		protected internal RtfTableExportHelper TableHelper { get { return tableHelper; } }
		DocumentModel DocumentModel { get { return RtfContentExporter.DocumentModel; } }
		PieceTable PieceTable { get { return RtfContentExporter.PieceTable; } }
		internal DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		public int NestingLevel { get { return nestingLevel; } }
		#endregion
		protected virtual void ExportBase() {
			int rowsCount = TableHelper.Table.Rows.Count;
			for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
				ExportRow(TableHelper.Table.Rows[rowIndex], rowIndex);
		}
		int GetTableStyleIndex() {
			if (rtfExporter.RtfExportHelper.SupportStyle && tableHelper.Table.StyleIndex != TableStyleCollection.DefaultTableStyleIndex) {
				string styleName = tableHelper.Table.TableStyle.StyleName;
				Dictionary<string, int> styleCollection = rtfExporter.RtfExportHelper.TableStylesCollectionIndex;
				if (styleCollection.ContainsKey(styleName))
					return styleCollection[styleName];
			}
			return -1;
		}
		protected void ExportRowCells(TableRow row, int rowIndex) {
			int cellsCount = row.Cells.Count;
			for (int i = 0; i < cellsCount; i++)
				ExportCellParagraphs(row.Cells[i], rowIndex);
		}
		bool IsCellEmpty(TableCell cell) {
			if (cell.StartParagraphIndex != cell.EndParagraphIndex)
				return false;
			Paragraph par = PieceTable.Paragraphs[cell.StartParagraphIndex];
			return par.Length == 1;
		}
		protected virtual void ExportInTableParagraph(ParagraphIndex parIndex, int tableNestingLevel, bool isEndParagraph, ConditionalTableStyleFormattingTypes condTypes) {
			Paragraph paragraph = PieceTable.Paragraphs[parIndex];
			RtfContentExporter.ExportParagraphCore(paragraph, tableNestingLevel, condTypes, tableStyleIndex);
			FinishParagraph(isEndParagraph);
		}
		protected void FinishParagraph(bool isEndParagraph) {
			if (isEndParagraph)
				WriteParagraphEndMark();
			else
				RtfBuilder.WriteCommand(RtfExportSR.EndOfParagraph);
		}
		protected internal void ExportCellParagraphs(TableCell cell, int parentRowIndex) {
			ParagraphIndex startParagraphIndex = cell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = cell.EndParagraphIndex;
			for (ParagraphIndex parIndex = startParagraphIndex; parIndex <= endParagraphIndex; parIndex++) {
				ParagraphIndex nextParIndex;
				if (ExportNestedTable(cell, parIndex, out nextParIndex)) {
					parIndex = nextParIndex;
					if (NestingLevel == 1) {
						TableRow row = cell.Row;
						ExportRowProperties(row, parentRowIndex);
					}
				}
				else {
					bool isEndParagraph = parIndex == endParagraphIndex;
					ExportInTableParagraph(parIndex, NestingLevel, isEndParagraph, cell.CellConditionalFormattingMasks);
				}
			}
		}
		bool ExportNestedTable(TableCell cell, ParagraphIndex parIndex, out ParagraphIndex lastParIndex) {
			Paragraph paragraph = PieceTable.Paragraphs[parIndex];
			TableCell parCell = paragraph.GetCell();
			Debug.Assert(paragraph.DocumentModel.DocumentCapabilities.TablesAllowed);
			Debug.Assert(parCell != null);
			if (Object.ReferenceEquals(parCell, cell)) {
				lastParIndex = parIndex;
				return false;
			}
			Debug.Assert(parCell.Row != null);
			Table nestedTable = parCell.Table;
			Debug.Assert(nestedTable != null);
			while (!Object.ReferenceEquals(cell, nestedTable.ParentCell)) {
				Debug.Assert(nestedTable.ParentCell != null);
				nestedTable = nestedTable.ParentCell.Table;
			}
			ExportNestedTable(nestedTable, out lastParIndex);
			return true;
		}
		protected virtual void ExportNestedTable(Table table, out ParagraphIndex lastParIndex) {
			RtfNestedTableExporterState nestedTableExporter = new RtfNestedTableExporterState(RtfContentExporter, table, NestingLevel + 1);
			nestedTableExporter.Export();
			lastParIndex = nestedTableExporter.TableHelper.GetLastParagraphIndex();
		}
		protected virtual RtfTableExportHelper CreateTableHelper(Table table) {
			return new RtfTableExportHelper(table);
		}
		public abstract void Export();
		protected abstract void ExportRow(TableRow row, int rowIndex);
		protected abstract void WriteParagraphEndMark();
		#region ExportRowProperties
		internal void ExportRowProperties(TableRow row, int rowIndex) {
			int rowLeft = 0;
			ExportOwnRowProperties(row, rowIndex, out rowLeft);
			int cellsCount = row.Cells.Count;
			int cellLeftSideIndex = row.GridBefore;
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			int cellRight = unitConverter.ToLayoutUnits(rowLeft);
			for (int i = 0; i < cellsCount; i++) {
				TableCell cell = row.Cells[i];
				int columnSpan = cell.ColumnSpan;
				int cellWidth = TableHelper.GetCellWidth(cellLeftSideIndex, columnSpan);
				cellLeftSideIndex += columnSpan;
				cellRight += cellWidth;
				ExportCellProperties(cell, unitConverter.ToModelUnits(cellRight));
			}
		}
		int CalculateRowLeft(TableRow row, WidthUnit indent) {
			int widthBefore = row.GridBefore > 0 ? TableHelper.GetCellWidth(0, row.GridBefore) : 0;
			int offset = GetActualWidth(indent) + widthBefore;
			return offset - (rowLeftOffset ?? CalculateRowLeftOffset(row));
		}
		int? rowLeftOffset;
		protected virtual int CalculateRowLeftOffset(TableRow row) {
			int borderWidth = row.Cells.First.GetActualLeftCellBorder().Width;
			WidthUnit leftMargin = row.Cells.First.GetActualLeftMargin();
			rowLeftOffset = Math.Max(borderWidth / 2, GetActualWidth(leftMargin));
			return (int)rowLeftOffset;
		}
		int GetActualWidth(WidthUnit unit) {
			if (unit.Type == WidthUnitType.ModelUnits)
				return UnitConverter.ModelUnitsToTwips(unit.Value);
			return 0;
		}
		internal void ExportOwnRowProperties(TableRow row, int rowIndex, out int left) {
			StartNewRow(rowIndex);
			tableRowPropertiesExporter.WriteRowAlignment(row.TableRowAlignment);
			Table table = row.Table;
			tablePropertiesExporter.WriteTableBorders(table.GetActualTopBorder().Info, table.GetActualLeftBorder().Info, table.GetActualBottomBorder().Info, table.GetActualRightBorder().Info, table.GetActualInsideHorizontalBorder().Info, table.GetActualInsideVerticalBorder().Info);
			tablePropertiesExporter.WriteTableFloatingPosition(table.TableProperties.FloatingPosition.Info);
			left = CalculateRowLeft(row, table.TableIndent);
			tablePropertiesExporter.WriteRowLeft(left);
			tableRowPropertiesExporter.WriteRowHeight(row.Height.Info);
			tableRowPropertiesExporter.WriteRowHeader(row.Header);
			tableRowPropertiesExporter.WriteRowCantSplit(row.CantSplit);
			tablePropertiesExporter.WriteTableWidth(table.PreferredWidth.Info);
			tableRowPropertiesExporter.WriteWidthBefore(row.WidthBefore.Info);
			tableRowPropertiesExporter.WriteWidthAfter(row.WidthAfter.Info);
			tablePropertiesExporter.WriteTableLayout(table.TableLayout);
			tableRowPropertiesExporter.WriteRowCellSpacing(row.CellSpacing.Info);
			tablePropertiesExporter.WriteTableCellMargins(table.LeftMargin.Info, table.RightMargin.Info, table.BottomMargin.Info, table.TopMargin.Info);
			tablePropertiesExporter.WriteTableLook(table.TableLook);
			tablePropertiesExporter.WriteTableIndent(table.TableIndent.Info);
			tablePropertiesExporter.WriteBandSizes(table.TableProperties.GeneralSettings.Info, table.TableStyle.HasRowBandingStyleProperties, table.TableStyle.HasColumnBandingStyleProperties);
		}
		#region StartNewRow
		void StartNewRow(int rowIndex) {
			RtfBuilder.WriteCommand(RtfExportSR.ResetTableProperties);
			RtfBuilder.WriteCommand(RtfExportSR.TableRowIndex, rowIndex);
			if ((tableHelper.Table.TableLook & TableLookTypes.ApplyFirstRow) > 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowBandIndex, rowIndex-1);
			else
				RtfBuilder.WriteCommand(RtfExportSR.TableRowBandIndex, rowIndex);
			if (rowIndex == tableHelper.Table.Rows.Count - 1)
				tableRowPropertiesExporter.WriteLastRowMark();
			WriteTableStyleIndex();
			tableRowPropertiesExporter.WriteHalfSpaceBetweenCells(CalcHalfSpaceBetweenCells());
		}
		#region WriteTableStyleIndex
		void WriteTableStyleIndex() {
			if (tableStyleIndex != -1)
				RtfBuilder.WriteCommand(RtfExportSR.TableStyleIndex, tableStyleIndex);
		}
		#endregion
		int CalcHalfSpaceBetweenCells() {
			WidthUnit leftMargin = tableHelper.Table.TableProperties.CellMargins.Left;
			int leftMarginVal = leftMargin.Type == WidthUnitType.ModelUnits ? UnitConverter.ModelUnitsToTwips(leftMargin.Value) : 0;
			WidthUnit rightMargin = tableHelper.Table.TableProperties.CellMargins.Right;
			int rightMarginVal = rightMargin.Type == WidthUnitType.ModelUnits ? UnitConverter.ModelUnitsToTwips(rightMargin.Value) : 0;
			return (leftMarginVal + rightMarginVal) / 2;
		}
		#endregion
		#endregion
		#region ExportCellProperties
		internal void ExportCellProperties(TableCell cell, int cellRight) {
			tableCellPropertiesExporter.WriteCellMerging(cell.VerticalMerging);
			tableCellPropertiesExporter.WriteCellVerticalAlignment(cell.VerticalAlignment);
			tableCellPropertiesExporter.WriteCellBackgroundColor(cell);
			tableCellPropertiesExporter.WriteCellForegroundColor(cell);
			tableCellPropertiesExporter.WriteCellShading(cell);
			tableCellPropertiesExporter.WriteCellBasicBorders(cell.GetActualTopCellBorder().Info, cell.GetActualLeftCellBorder().Info, cell.GetActualRightCellBorder().Info, cell.GetActualBottomCellBorder().Info);
			tableCellPropertiesExporter.WriteCellTextDirection(cell.TextDirection);
			tableCellPropertiesExporter.WriteCellFitText(cell.FitText);
			tableCellPropertiesExporter.WriteCellNoWrap(cell.NoWrap);
			tableCellPropertiesExporter.WriteCellHideCellMark(cell.HideCellMark);
			tableCellPropertiesExporter.WriteCellPreferredWidth(cell.PreferredWidth.Info);
			tableCellPropertiesExporter.WriteCellMargings(cell.GetActualTopMargin().Info, cell.GetActualLeftMargin().Info, cell.GetActualRightMargin().Info, cell.GetActualBottomMargin().Info);
			tableCellPropertiesExporter.WriteCellRight(cellRight);
		}
		#endregion
	}
	#endregion
	#region RtfTableExporterState
	public class RtfTableExporterState : RtfTableExporterStateBase {
		public RtfTableExporterState(RtfContentExporter rtfExporter, Table table)
			: base(rtfExporter, table, 1) {
		}
		public override void Export() {
			ExportRowProperties(TableHelper.Table.Rows.First, 0);
			ExportBase();
		}
		protected override void WriteParagraphEndMark() {
			RtfBuilder.WriteCommand(RtfExportSR.TableEndCell);
		}
		protected override void ExportRow(TableRow row, int rowIndex) {
			ExportRowCells(row, rowIndex);
			ExportRowProperties(row, rowIndex);
			RtfBuilder.WriteCommand(RtfExportSR.TableEndRow);
		}
	}
	#endregion
	#region RtfTableStartingWithNestedTableExporterState
	public class RtfTableStartingWithNestedTableExporterState : RtfTableExporterState {
		public RtfTableStartingWithNestedTableExporterState(RtfContentExporter rtfExporter, Table table)
			: base(rtfExporter, table) {
		}
		public override void Export() {
			ExportBase();
		}
	}
	#endregion
	#region RtfNestedTableExporterState
	public class RtfNestedTableExporterState : RtfTableExporterStateBase {
		public RtfNestedTableExporterState(RtfContentExporter rtfExporter, Table table, int nestingLevel)
			: base(rtfExporter, table, nestingLevel) {
		}
		public override void Export() {
			ExportBase();
		}
		protected override void WriteParagraphEndMark() {
			RtfBuilder.WriteCommand(RtfExportSR.NestedTableEndCell);
			WriteNoNestedTableGroup();
		}
		protected override void ExportRow(TableRow row, int rowIndex) {
			ExportRowCells(row, rowIndex);
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.NestedTableProperties);
			ExportRowProperties(row, rowIndex);
			RtfBuilder.WriteCommand(RtfExportSR.NestedTableEndRow);
			RtfBuilder.CloseGroup();
		}
		void WriteNoNestedTableGroup() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.NoNestedTable);
			RtfBuilder.WriteCommand(RtfExportSR.EndOfParagraph);
			RtfBuilder.CloseGroup();
		}
	}
	#endregion
}
