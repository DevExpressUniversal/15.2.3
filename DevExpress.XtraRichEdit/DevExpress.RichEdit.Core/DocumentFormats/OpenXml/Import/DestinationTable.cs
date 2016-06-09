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
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Export.OpenXml;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region TableDestination
	public class TableDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tr", OnRow);
			result.Add("tblPr", OnTableProperties);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			result.Add("tblGrid", OnTableGrid);
			return result;
		}
		protected static TableDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableDestination)importer.PeekDestination();
		}
		readonly Table table;
		readonly List<int> tableGrid;
		public TableDestination(WordProcessingMLBaseImporter importer, TableCell parentCell)
			: base(importer) {
			this.table = new Table(PieceTable, parentCell, 0, 0);
			this.tableGrid = new List<int>();
		}
		public TableDestination(WordProcessingMLBaseImporter importer)
			: this(importer, null) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal Table Table { get { return table; } }
		protected internal List<int> TableGrid { get { return tableGrid; } }
		static Destination OnRow(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableDestination destination = GetThis(importer);
			return new TableRowDestination(importer, destination.Table);
		}
		protected static Destination OnTableProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableDestination destination = GetThis(importer);
			return new TablePropertiesDestination(importer, destination.Table);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
		static Destination OnTableGrid(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableDestination destination = GetThis(importer);
			return new TableGridDestination(importer, destination.TableGrid);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.BeginTable();
			Importer.PieceTable.Tables.Add(table);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (Table.TableLayout == TableLayoutType.Fixed && tableGrid.Count > 0)
				EnsureTableCellsWidth(Table);
			Importer.EndTable();
		}
		void EnsureTableCellsWidth(Table table) {
			int rowsCount = table.Rows.Count;
			for (int i = 0; i < rowsCount; i++)
				EnsureRowCellsWidth(table.Rows[i]);
		}
		void EnsureRowCellsWidth(TableRow tableRow) {
			int columnIndex = 0;
			int cellsCount = tableRow.Cells.Count;
			for (int i = 0; i < cellsCount; i++) {
				TableCellProperties properties = tableRow.Cells[i].Properties;
				int remainedCellsCount = cellsCount - i - 1;
				int maxAvailableColumnSpan = TableGrid.Count - columnIndex - remainedCellsCount;
				int columnSpan = Math.Min(maxAvailableColumnSpan, properties.ColumnSpan);
				if (properties.PreferredWidth.Type == WidthUnitType.Nil || properties.PreferredWidth.Type == WidthUnitType.Auto) {
					properties.PreferredWidth.Value = CalculateCellWidth(columnIndex, columnSpan);
					properties.PreferredWidth.Type = WidthUnitType.ModelUnits;
				}
				columnIndex += columnSpan;
			}
		}
		int CalculateCellWidth(int columnIndex, int columnSpan) {
			int result = 0;
			int startIndex = columnIndex;
			int endIndex = startIndex + columnSpan - 1;
			for (int i = startIndex; i <= endIndex; i++)
				result += TableGrid[i];
			return result;
		}
	}
	#endregion
	#region TableGridDestination
	public class TableGridDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("gridCol", OnColumn);
			return result;
		}
		readonly List<int> tableGrid;
		public TableGridDestination(WordProcessingMLBaseImporter importer, List<int> tableGrid)
			: base(importer) {
			this.tableGrid = tableGrid;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static TableGridDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableGridDestination)importer.PeekDestination();
		}
		static Destination OnColumn(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableGridDestination destination = GetThis(importer);
			return new GridColumnDestination(importer, destination.tableGrid);
		}
	}
	#endregion
	#region GridColumnDestination
	public class GridColumnDestination : ElementDestination {
		readonly List<int> tableGrid;
		public GridColumnDestination(WordProcessingMLBaseImporter importer, List<int> tableGrid)
			: base(importer) {
			this.tableGrid = tableGrid;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "w");
			this.tableGrid.Add(value);
		}
	}
	#endregion
	#region TableRowDestination
	public class TableRowDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tc", OnCell);
			result.Add("trPr", OnTableRowProperties);
			result.Add("tblPrEx", OnTablePropertiesException);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		protected static TableRowDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableRowDestination)importer.PeekDestination();
		}
		readonly TableRow row;
		public TableRowDestination(WordProcessingMLBaseImporter importer, Table table)
			: base(importer) {
			this.row = new TableRow(table);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal TableRow Row { get { return row; } }
		static Destination OnCell(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellDestination(importer, GetThis(importer).Row);
		}
		protected static Destination OnTableRowProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowPropertiesDestination(importer, GetThis(importer).Row.Properties);
		}
		static Destination OnTablePropertiesException(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TablePropertiesDestinationCore(importer, GetThis(importer).Row.TablePropertiesException);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			row.Table.Rows.AddInternal(row);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
	}
	#endregion
	#region TableCellDestination
	public class TableCellDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("tcPr", OnCellProperies);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			result.Add("altChunk", OnAltChunk);
			return result;
		}
		static TableCellDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellDestination)importer.PeekDestination();
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).EndParagraphIndex = importer.Position.ParagraphIndex;
			return importer.CreateParagraphDestination();
		}
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableCellDestination destination = GetThis(importer);
			return new TableDestination(importer, destination.Cell);
		}
		static Destination OnCellProperies(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellPropertiesDestination(importer, GetThis(importer).Cell);
		}
		protected static Destination OnBookmarkStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkStartElementDestination(reader);
		}
		protected static Destination OnBookmarkEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkEndElementDestination(reader);
		}
		protected static Destination OnRangePermissionStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionStartElementDestination(importer);
		}
		protected static Destination OnRangePermissionEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionEndElementDestination(importer);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
		readonly TableCell cell;
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		public TableCellDestination(WordProcessingMLBaseImporter importer, TableRow row)
			: base(importer) {
			this.startParagraphIndex = importer.Position.ParagraphIndex;
			this.cell = new TableCell(row);
		}
		protected internal TableCell Cell { get { return cell; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Cell.Row.Cells.AddInternal(cell);
		}
		public override void ProcessElementClose(XmlReader reader) {
			PieceTable.TableCellsManager.InitializeTableCell(Cell, StartParagraphIndex, EndParagraphIndex);
		}
	}
	#endregion
	#region TablePropertiesDestination
	public class TablePropertiesDestination : TablePropertiesDestinationCore {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tblStyle", OnTableStyle);
			result.Add("tblW", OnTableWidth);
			result.Add("tblInd", OnTableIndent);
			result.Add("tblLook", OnTableLook);
			result.Add("tblBorders", OnTableBorders);
			result.Add("tblCellMar", OnTableCellMarginBorders);
			result.Add("tblCellSpacing", OnTableCellCellSpacing);
			result.Add("tblLayout", OnTableLayout);
			result.Add("tblOverlap", OnTableOverlap);
			result.Add("tblpPr", OnTableFloatingPosition);
			result.Add("tblStyleColBandSize", OnTableStyleColBandSize);
			result.Add("tblStyleRowBandSize", OnTableStyleRowBandSize);
			result.Add("shd", OnTableBackground);
			result.Add("jc", OnTableAlignment);
			result.Add("adb", OnTableAvoidDoubleBorders);
			return result;
		}
		Table table;
		public TablePropertiesDestination(WordProcessingMLBaseImporter importer, Table table)
			: base(importer, table.TableProperties) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static TablePropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TablePropertiesDestination)importer.PeekDestination();
		}
		static Destination OnTableStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableStyleDestination(importer, GetThis(importer).Table);
		}
	}
	#endregion
	#region TablePropertiesDestinationCore
	public class TablePropertiesDestinationCore : TablePropertiesBaseDestination<TableProperties> {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tblW", OnTableWidth);
			result.Add("tblInd", OnTableIndent);
			result.Add("tblLook", OnTableLook);
			result.Add("tblBorders", OnTableBorders);
			result.Add("tblCellMar", OnTableCellMarginBorders);
			result.Add("tblCellSpacing", OnTableCellCellSpacing);
			result.Add("tblLayout", OnTableLayout);
			result.Add("tblOverlap", OnTableOverlap);
			result.Add("tblpPr", OnTableFloatingPosition);
			result.Add("tblStyleColBandSize", OnTableStyleColBandSize);
			result.Add("tblStyleRowBandSize", OnTableStyleRowBandSize);
			result.Add("shd", OnTableBackground);
			result.Add("jc", OnTableAlignment);
			result.Add("adb", OnTableAvoidDoubleBorders);
			return result;
		}
		static TablePropertiesDestinationCore GetThis(WordProcessingMLBaseImporter importer) {
			return (TablePropertiesDestinationCore)importer.PeekDestination();
		}
		static TableProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TablePropertiesDestinationCore destination = GetThis(importer);
			return (TableProperties)destination.Properties;
		}
		public TablePropertiesDestinationCore(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnTableWidth(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitNonNegativeDestination(importer, GetProperties(importer).PreferredWidth);
		}
		protected static Destination OnTableIndent(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetProperties(importer).TableIndent);
		}
		protected static Destination OnTableLook(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableLookDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableBorders(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBordersDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellMarginBorders(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellMarginsDestination(importer, GetProperties(importer).CellMargins);
		}
		protected static Destination OnTableCellCellSpacing(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetProperties(importer).CellSpacing);
		}
		protected static Destination OnTableLayout(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableLayoutDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableAlignmentDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableOverlap(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableOverlapDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableFloatingPosition(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableFloatingPositionDestination(importer, GetProperties(importer).FloatingPosition);
		}
		protected static Destination OnTableStyleColBandSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableStyleColBandSizeDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableStyleRowBandSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableStyleRowBandSizeDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableBackground(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBackgroundDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableAvoidDoubleBorders(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableAvoidDoubleBordersDestination(importer, GetProperties(importer));
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginInit();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndInit();
		}
	}
	#endregion
	#region TableRowPropertiesDestination
	public class TableRowPropertiesDestination : TablePropertiesBaseDestination<TableRowProperties> {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("cantSplit", OnTableRowCantSplit);
			result.Add("gridAfter", OnTableRowGridAfter);
			result.Add("gridBefore", OnTableRowGridBefore);
			result.Add("hidden", OnTableRowHideCellMark);
			result.Add("jc", OnTableRowAlignment);
			result.Add("tblCellSpacing", OnTableRowCellSpacing);
			result.Add("tblHeader", OnTableRowHeader);
			result.Add("trHeight", OnTableRowHeight);
			result.Add("wAfter", OnTableRowWidthAfter);
			result.Add("wBefore", OnTableRowWidthBefore);
			result.Add("cnfStyle", OnTableRowConditionalFormatting);
			return result;
		}
		static TableRowPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableRowPropertiesDestination)importer.PeekDestination();
		}
		static TableRowProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TableRowPropertiesDestination destination = GetThis(importer);
			return (TableRowProperties)destination.Properties;
		}
		public TableRowPropertiesDestination(WordProcessingMLBaseImporter importer, TableRowProperties cellProperties)
			: base(importer, cellProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnTableRowCantSplit(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowCantSplitDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowGridAfter(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowGridAfterDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowGridBefore(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowGridBeforeDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowHideCellMark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowHideCellMarkDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowAlignmentDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowCellSpacing(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetProperties(importer).CellSpacing);
		}
		static Destination OnTableRowHeader(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowHeaderDestination(importer, GetProperties(importer));
		}
		static Destination OnTableRowHeight(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HeightUnitDestination(importer, GetProperties(importer).Height);
		}
		static Destination OnTableRowWidthAfter(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetProperties(importer).WidthAfter);
		}
		static Destination OnTableRowWidthBefore(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetProperties(importer).WidthBefore);
		}
		static Destination OnTableRowConditionalFormatting(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowConditionalFormattingDestination(importer, GetProperties(importer));
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginInit();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndInit();
		}
	}
	#endregion
	#region TableCellPropertiesDestination
	public class TableCellPropertiesDestination : TableCellPropertiesDestinationCore {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tcW", OnTableCellWidth);
			result.Add("tcBorders", OnTableCellBorders);
			result.Add("vMerge", OnTableCellMerge);
			result.Add("tblCStyle", OnTableCellStyle);
			result.Add("gridSpan", OnTableCellColumnSpan);
			result.Add("shd", OnTableCellShading);
			result.Add("tcMar", OnTableCellMargins);
			result.Add("tcFitText", OnTableCellFitText);
			result.Add("noWrap", OnTableCellNoWrap);
			result.Add("hideMark", OnTableCellHideMark);
			result.Add("textDirection", OnTableCellTextDirection);
			result.Add("vAlign", OnTableCellVerticalAlignment);
			result.Add("cnfStyle", OnTableCellConditionalFormatting);
			return result;
		}
		static TableCellPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnTableCellStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellStyleDestination(importer, GetThis(importer).Cell);
		}
		TableCell cell;
		public TableCellPropertiesDestination(WordProcessingMLBaseImporter importer, TableCell cell)
			: base(importer, cell.Properties) {
				this.cell = cell;
		}
		protected TableCell Cell { get { return cell; } }
		protected internal override ElementHandlerTable ElementHandlerTable {
			get {
				return handlerTable;
			}
		}
	}
	#endregion
	#region TableCellPropertiesDestinationCore
	public class TableCellPropertiesDestinationCore : TablePropertiesBaseDestination<TableCellProperties> {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tcW", OnTableCellWidth);
			result.Add("tcBorders", OnTableCellBorders);
			result.Add("vMerge", OnTableCellMerge);
			result.Add("gridSpan", OnTableCellColumnSpan);
			result.Add("shd", OnTableCellShading);
			result.Add("tcMar", OnTableCellMargins);
			result.Add("tcFitText", OnTableCellFitText);
			result.Add("noWrap", OnTableCellNoWrap);
			result.Add("hideMark", OnTableCellHideMark);
			result.Add("textDirection", OnTableCellTextDirection);
			result.Add("vAlign", OnTableCellVerticalAlignment);
			result.Add("cnfStyle", OnTableCellConditionalFormatting);
			return result;
		}
		static TableCellPropertiesDestinationCore GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellPropertiesDestinationCore)importer.PeekDestination();
		}
		static TableCellProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TableCellPropertiesDestinationCore destination = GetThis(importer);
			return (TableCellProperties)destination.Properties;
		}
		public TableCellPropertiesDestinationCore(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnTableCellWidth(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitNonNegativeDestination(importer, GetProperties(importer).PreferredWidth);
		}
		protected static Destination OnTableCellBorders(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellBordersDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellMerge(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellVerticalMergingStateDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellColumnSpan(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellColumnSpanDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellShading(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellShadingDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellFitText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellFitTextDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellNoWrap(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellNoWrapDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellMargins(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellMarginsDestination(importer, GetProperties(importer).CellMargins);
		}
		protected static Destination OnTableCellHideMark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellHideMarkDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellTextDirection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellTextDirectionDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellVerticalAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellVerticalAlignmentDestination(importer, GetProperties(importer));
		}
		protected static Destination OnTableCellConditionalFormatting(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellConditionalFormattingDestination(importer, GetProperties(importer));
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginInit();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndInit();
		}
	}
	#endregion
	public class WidthUnitNonNegativeDestination : WidthUnitDestination {
		public WidthUnitNonNegativeDestination(WordProcessingMLBaseImporter importer, WidthUnit widthUnit)
			: base(importer, widthUnit) {
		}
		protected internal override bool IsValid(int value) {
			return base.IsValid(value) && value >= 0;
		}
	}
	#region WidthUnitDestination
	public class WidthUnitDestination : LeafElementDestination {
		static Dictionary<WidthUnitType, string> widthUnitTypeTable = CreateWidthUnitTypeTable();
		static Dictionary<WidthUnitType, string> CreateWidthUnitTypeTable() {
			Dictionary<WidthUnitType, string> result = new Dictionary<WidthUnitType, string>();
			result.Add(WidthUnitType.Auto, "auto");
			result.Add(WidthUnitType.FiftiethsOfPercent, "pct");
			result.Add(WidthUnitType.Nil, "nil");
			result.Add(WidthUnitType.ModelUnits, "dxa");
			return result;
		}
		readonly WidthUnit widthUnit;
		public WidthUnitDestination(WordProcessingMLBaseImporter importer, WidthUnit widthUnit)
			: base(importer) {
			Guard.ArgumentNotNull(widthUnit, "widthUnit");
			this.widthUnit = widthUnit;
		}
		protected internal virtual bool IsValid(int value) {
			return value != Int32.MinValue;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			WidthUnitType unitType = Importer.GetWpEnumValue<WidthUnitType>(reader, "type", widthUnitTypeTable, WidthUnitType.Auto);
			int value = Importer.GetWpSTIntegerValue(reader, "w");
			if (IsValid(value)) {
				widthUnit.Type = unitType;
				widthUnit.Value = unitType == WidthUnitType.ModelUnits ? DocumentModel.UnitConverter.TwipsToModelUnits(value) : value;
			}
			else
				widthUnit.Type = WidthUnitType.Auto;
		}
	}
	#endregion
	#region HeightUnitDestination
	public class HeightUnitDestination : LeafElementDestination {
		static Dictionary<HeightUnitType, string> heightUnitTypeTable = CreateHeightUnitTypeTable();
		static Dictionary<HeightUnitType, string> CreateHeightUnitTypeTable() {
			Dictionary<HeightUnitType, string> result = new Dictionary<HeightUnitType, string>();
			result.Add(HeightUnitType.Auto, "auto");
			result.Add(HeightUnitType.Exact, "exact");
			result.Add(HeightUnitType.Minimum, "atLeast");
			return result;
		}
		readonly HeightUnit heightUnit;
		public HeightUnitDestination(WordProcessingMLBaseImporter importer, HeightUnit heightUnit)
			: base(importer) {
			Guard.ArgumentNotNull(heightUnit, "heightUnit");
			this.heightUnit = heightUnit;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			HeightUnitType unitType = Importer.GetWpEnumValue<HeightUnitType>(reader, "hRule", heightUnitTypeTable, HeightUnitType.Minimum);
			heightUnit.Type = unitType;
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue)
				heightUnit.Value = DocumentModel.UnitConverter.TwipsToModelUnits(value);
		}
	}
	#endregion
	#region TableStyleDestination
	public class TableStyleDestination : LeafElementDestination {
		Table table;
		public TableStyleDestination(WordProcessingMLBaseImporter importer, Table table)
			: base(importer) {
			this.table = table;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string styleName = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			int styleIndex = Importer.LookupTableStyleIndex(styleName);
			if (styleIndex >= 0)
				this.table.StyleIndex = styleIndex;
		}
	}
	#endregion
	#region TableCellStyleDestination
	public class TableCellStyleDestination : LeafElementDestination {
		TableCell cell;
		public TableCellStyleDestination(WordProcessingMLBaseImporter importer, TableCell cell)
			: base(importer) {
			this.cell = cell;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string styleName = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			int styleIndex = Importer.LookupTableCellStyleIndex(styleName);
			if (styleIndex >= 0)
				cell.SetTableCellStyleIndexCore(styleIndex);
		}
	}
	#endregion
	#region TableLookDestination
	public class TableLookDestination : TablePropertiesLeafElementDestination {
		public TableLookDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val", NumberStyles.HexNumber, Int32.MinValue);
			if (value == Int32.MinValue)
				return;
			TableProperties.TableLook = (TableLookTypes)value;
		}
	}
	#endregion
	#region TableBordersDestination
	public class TableBordersDestination : TablePropertiesElementBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("top", OnTopBorder);
			result.Add("left", OnLeftBorder);
			result.Add("bottom", OnBottomBorder);
			result.Add("right", OnRightBorder);
			result.Add("insideH", OnInsideHorizontalBorder);
			result.Add("insideV", OnInsideVerticalBorder);
			return result;
		}
		static TableBorders GetTableBorders(WordProcessingMLBaseImporter importer) {
			return GetProperties(importer).Borders;
		}
		static Destination OnTopBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).TopBorder);
		}
		static Destination OnLeftBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).LeftBorder);
		}
		static Destination OnBottomBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).BottomBorder);
		}
		static Destination OnRightBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).RightBorder);
		}
		static Destination OnInsideHorizontalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).InsideHorizontalBorder);
		}
		static Destination OnInsideVerticalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetTableBorders(importer).InsideVerticalBorder);
		}
		public TableBordersDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableLayoutDestination
	public class TableLayoutDestination : TablePropertiesLeafElementDestination {
		static Dictionary<TableLayoutType, string> tableLayoutTypeTable = CreateTableLayoutTypeTable();
		static Dictionary<TableLayoutType, string> CreateTableLayoutTypeTable() {
			Dictionary<TableLayoutType, string> result = new Dictionary<TableLayoutType, string>();
			result.Add(TableLayoutType.Autofit, "autofit");
			result.Add(TableLayoutType.Fixed, "fixed");
			return result;
		}
		public TableLayoutDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TableProperties.TableLayout = Importer.GetWpEnumValue<TableLayoutType>(reader, "type", tableLayoutTypeTable, TableLayoutType.Autofit);
		}
	}
	#endregion
	#region TableAlignmentDestination
	public class TableAlignmentDestination : TablePropertiesLeafElementDestination {
		static Dictionary<TableRowAlignment, string> tableRowAlignmentTable = CreateTableRowAlignmentTable();
		static Dictionary<TableRowAlignment, string> CreateTableRowAlignmentTable() {
			Dictionary<TableRowAlignment, string> result = new Dictionary<TableRowAlignment, string>();
			result.Add(TableRowAlignment.Both, "both");
			result.Add(TableRowAlignment.Center, "center");
			result.Add(TableRowAlignment.Distribute, "distribute");
			result.Add(TableRowAlignment.Left, "left");
			result.Add(TableRowAlignment.NumTab, "numTab");
			result.Add(TableRowAlignment.Right, "right");
			return result;
		}
		public TableAlignmentDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TableProperties.TableAlignment = Importer.GetWpEnumValue<TableRowAlignment>(reader, "val", tableRowAlignmentTable, TableRowAlignment.Left);
		}
	}
	#endregion
	#region TableOverlapDestination
	public class TableOverlapDestination : TablePropertiesLeafElementDestination {
		public TableOverlapDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string overlap = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (overlap == "never")
				TableProperties.IsTableOverlap = false;
			else
				TableProperties.IsTableOverlap = true;
		}
	}
	#endregion
	#region TableStyleColBandSizeDestination
	public class TableStyleColBandSizeDestination : TablePropertiesLeafElementDestination {
		public TableStyleColBandSizeDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue)
				TableProperties.TableStyleColBandSize = value;
		}
	}
	#endregion
	#region TableStyleRowBandSizeDestination
	public class TableStyleRowBandSizeDestination : TablePropertiesLeafElementDestination {
		public TableStyleRowBandSizeDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue)
				TableProperties.TableStyleRowBandSize = value;
		}
	}
	#endregion
	#region TableBackgroundDestination
	public class TableBackgroundDestination : TablePropertiesLeafElementDestination {
		public TableBackgroundDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ShadingPattern pattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			Color fill = Importer.GetWpSTColorValue(reader, "fill", DXColor.Empty);
			Color patternColor = Importer.GetWpSTColorValue(reader, "color", DXColor.Empty);
			Color actualColor = ShadingHelper.GetActualBackColor(fill, patternColor, pattern);
			if (actualColor != DXColor.Empty)
				TableProperties.BackgroundColor = actualColor;
#if THEMES_EDIT
			Shading shading = TableProperties.Shading;
			shading.ShadingPattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			shading.ColorInfo = helper.SaveColorModelInfo(Importer, reader, "color");
			shading.FillInfo = helper.SaveFillInfo(Importer, reader);
#endif
		}
	}
	#endregion
	#region TableAvoidDoubleBordersDestination
	public class TableAvoidDoubleBordersDestination : TablePropertiesLeafElementDestination {
		public TableAvoidDoubleBordersDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TableProperties.AvoidDoubleBorders = Importer.GetWpSTOnOffValue(reader, "val", false);
		}
	}
	#endregion
	#region TableFloatingPositionDestination
	public class TableFloatingPositionDestination : LeafElementDestination {
		static Dictionary<HorizontalAnchorTypes, string> horizontalAnchorTypeTable = CreateHorizontalAnchorTypeTable();
		static Dictionary<VerticalAnchorTypes, string> verticalAnchorTypeTable = CreateVerticalAnchorTypeTable();
		static Dictionary<HorizontalAlignMode, string> horizontalAlignModeTable = CreateHorizontalAlignModeTable();
		static Dictionary<VerticalAlignMode, string> verticalAlignModeTable = CreateVerticalAlignModeTable();
		static Dictionary<HorizontalAnchorTypes, string> CreateHorizontalAnchorTypeTable() {
			Dictionary<HorizontalAnchorTypes, string> result = new Dictionary<HorizontalAnchorTypes, string>();
			result.Add(HorizontalAnchorTypes.Column, "text");
			result.Add(HorizontalAnchorTypes.Margin, "margin");
			result.Add(HorizontalAnchorTypes.Page, "page");
			return result;
		}
		static Dictionary<VerticalAnchorTypes, string> CreateVerticalAnchorTypeTable() {
			Dictionary<VerticalAnchorTypes, string> result = new Dictionary<VerticalAnchorTypes, string>();
			result.Add(VerticalAnchorTypes.Paragraph, "text");
			result.Add(VerticalAnchorTypes.Margin, "margin");
			result.Add(VerticalAnchorTypes.Page, "page");
			return result;
		}
		static Dictionary<HorizontalAlignMode, string> CreateHorizontalAlignModeTable() {
			Dictionary<HorizontalAlignMode, string> result = new Dictionary<HorizontalAlignMode, string>();
			result.Add(HorizontalAlignMode.Center, "center");
			result.Add(HorizontalAlignMode.Inside, "inside");
			result.Add(HorizontalAlignMode.Left, "left");
			result.Add(HorizontalAlignMode.Outside, "outside");
			result.Add(HorizontalAlignMode.Right, "right");
			return result;
		}
		static Dictionary<VerticalAlignMode, string> CreateVerticalAlignModeTable() {
			Dictionary<VerticalAlignMode, string> result = new Dictionary<VerticalAlignMode, string>();
			result.Add(VerticalAlignMode.Bottom, "bottom");
			result.Add(VerticalAlignMode.Center, "center");
			result.Add(VerticalAlignMode.Inline, "inline");
			result.Add(VerticalAlignMode.Inside, "inside");
			result.Add(VerticalAlignMode.Outside, "outside");
			result.Add(VerticalAlignMode.Top, "top");
			return result;
		}
		readonly TableFloatingPosition floatingPosition;
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "bottomFromText");
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			if (value != Int32.MinValue)
				floatingPosition.BottomFromText = unitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "leftFromText");
			if (value != Int32.MinValue)
				floatingPosition.LeftFromText = unitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "rightFromText");
			if (value != Int32.MinValue)
				floatingPosition.RightFromText = unitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "topFromText");
			if (value != Int32.MinValue)
				floatingPosition.TopFromText = unitConverter.TwipsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "tblpX");
			if (value != Int32.MinValue) {
				floatingPosition.TableHorizontalPosition = unitConverter.TwipsToModelUnits(value);
				if (value != 0)
					floatingPosition.TextWrapping = TextWrapping.Around;
			}
			value = Importer.GetWpSTIntegerValue(reader, "tblpY");
			if (value != Int32.MinValue) {
				floatingPosition.TableVerticalPosition = unitConverter.TwipsToModelUnits(value);
				if (value != 0)
					floatingPosition.TextWrapping = TextWrapping.Around;
			}
			HorizontalAnchorTypes defaultHorizontalAnchor = HorizontalAnchorTypes.Column;
			floatingPosition.HorizontalAnchor = GetHorizontalAnchor(reader, defaultHorizontalAnchor);
			if (floatingPosition.HorizontalAnchor != defaultHorizontalAnchor)
				floatingPosition.TextWrapping = TextWrapping.Around;
			VerticalAnchorTypes defaultVerticalAnchor = VerticalAnchorTypes.Margin;
			floatingPosition.VerticalAnchor = GetVerticalAnchor(reader, defaultVerticalAnchor);
			if (floatingPosition.VerticalAnchor != defaultVerticalAnchor)
				floatingPosition.TextWrapping = TextWrapping.Around;
			floatingPosition.HorizontalAlign = Importer.GetWpEnumValue<HorizontalAlignMode>(reader, "tblpXSpec", horizontalAlignModeTable, HorizontalAlignMode.None);
			floatingPosition.VerticalAlign = Importer.GetWpEnumValue<VerticalAlignMode>(reader, "tblpYSpec", verticalAlignModeTable, VerticalAlignMode.None);
		}
		protected internal virtual HorizontalAnchorTypes GetHorizontalAnchor(XmlReader reader, HorizontalAnchorTypes defaultValue) {
			string value = Importer.ReadAttribute(reader, "horzAnchor");
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			if (value == "column")
				return HorizontalAnchorTypes.Page;
			return Importer.GetWpEnumValueCore<HorizontalAnchorTypes>(value, horizontalAnchorTypeTable, defaultValue);
		}
		protected internal virtual VerticalAnchorTypes GetVerticalAnchor(XmlReader reader, VerticalAnchorTypes defaultValue) {
			string value = Importer.ReadAttribute(reader, "vertAnchor");
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			if (value == "paragraph")
				return VerticalAnchorTypes.Paragraph;
			return Importer.GetWpEnumValueCore<VerticalAnchorTypes>(value, verticalAnchorTypeTable, defaultValue);
		}
		public TableFloatingPositionDestination(WordProcessingMLBaseImporter importer, TableFloatingPosition floatingPosition)
			: base(importer) {
			Guard.ArgumentNotNull(floatingPosition, "floatingPosition");
			this.floatingPosition = floatingPosition;
		}
	}
	#endregion
	#region TableBorderElementDestination
	public class TableBorderElementDestination : LeafElementDestination {
		static Dictionary<BorderLineStyle, string> borderStyleTable = CreateBorderStyleTable();
		static Dictionary<BorderLineStyle, string> CreateBorderStyleTable() {
			Dictionary<BorderLineStyle, string> result = new Dictionary<BorderLineStyle, string>();
			result.Add(BorderLineStyle.DashDotStroked, "dashDotStroked");
			result.Add(BorderLineStyle.Dashed, "dashed");
			result.Add(BorderLineStyle.DashSmallGap, "dashSmallGap");
			result.Add(BorderLineStyle.DotDash, "dotDash");
			result.Add(BorderLineStyle.DotDotDash, "dotDotDash");
			result.Add(BorderLineStyle.Dotted, "dotted");
			result.Add(BorderLineStyle.Double, "double");
			result.Add(BorderLineStyle.DoubleWave, "doubleWave");
			result.Add(BorderLineStyle.Inset, "inset");
			result.Add(BorderLineStyle.Disabled, "disabled");
			result.Add(BorderLineStyle.None, "none");
			result.Add(BorderLineStyle.Nil, "nil");
			result.Add(BorderLineStyle.Outset, "outset");
			result.Add(BorderLineStyle.Single, "single");
			result.Add(BorderLineStyle.Thick, "thick");
			result.Add(BorderLineStyle.ThickThinLargeGap, "thickThinLargeGap");
			result.Add(BorderLineStyle.ThickThinMediumGap, "thickThinMediumGap");
			result.Add(BorderLineStyle.ThickThinSmallGap, "thickThinSmallGap");
			result.Add(BorderLineStyle.ThinThickLargeGap, "thinThickLargeGap");
			result.Add(BorderLineStyle.ThinThickMediumGap, "thinThickMediumGap");
			result.Add(BorderLineStyle.ThinThickSmallGap, "thinThickSmallGap");
			result.Add(BorderLineStyle.ThinThickThinLargeGap, "thinThickThinLargeGap");
			result.Add(BorderLineStyle.ThinThickThinMediumGap, "thinThickThinMediumGap");
			result.Add(BorderLineStyle.ThinThickThinSmallGap, "thinThickThinSmallGap");
			result.Add(BorderLineStyle.ThreeDEmboss, "threeDEmboss");
			result.Add(BorderLineStyle.ThreeDEngrave, "threeDEngrave");
			result.Add(BorderLineStyle.Triple, "triple");
			result.Add(BorderLineStyle.Wave, "wave");
			result.Add(BorderLineStyle.Apples, "apples");
			result.Add(BorderLineStyle.ArchedScallops, "archedScallops");
			result.Add(BorderLineStyle.BabyPacifier, "babyPacifier");
			result.Add(BorderLineStyle.BabyRattle, "babyRattle");
			result.Add(BorderLineStyle.Balloons3Colors, "balloons3Colors");
			result.Add(BorderLineStyle.BalloonsHotAir, "balloonsHotAir");
			result.Add(BorderLineStyle.BasicBlackDashes, "basicBlackDashes");
			result.Add(BorderLineStyle.BasicBlackDots, "basicBlackDots");
			result.Add(BorderLineStyle.BasicBlackSquares, "basicBlackSquares");
			result.Add(BorderLineStyle.BasicThinLines, "basicThinLines");
			result.Add(BorderLineStyle.BasicWhiteDashes, "basicWhiteDashes");
			result.Add(BorderLineStyle.BasicWhiteDots, "basicWhiteDots");
			result.Add(BorderLineStyle.BasicWhiteSquares, "basicWhiteSquares");
			result.Add(BorderLineStyle.BasicWideInline, "basicWideInline");
			result.Add(BorderLineStyle.BasicWideMidline, "basicWideMidline");
			result.Add(BorderLineStyle.BasicWideOutline, "basicWideOutline");
			result.Add(BorderLineStyle.Bats, "bats");
			result.Add(BorderLineStyle.Birds, "birds");
			result.Add(BorderLineStyle.BirdsFlight, "birdsFlight");
			result.Add(BorderLineStyle.Cabins, "cabins");
			result.Add(BorderLineStyle.CakeSlice, "cakeSlice");
			result.Add(BorderLineStyle.CandyCorn, "candyCorn");
			result.Add(BorderLineStyle.CelticKnotwork, "celticKnotwork");
			result.Add(BorderLineStyle.CertificateBanner, "certificateBanner");
			result.Add(BorderLineStyle.ChainLink, "chainLink");
			result.Add(BorderLineStyle.ChampagneBottle, "champagneBottle");
			result.Add(BorderLineStyle.CheckedBarBlack, "checkedBarBlack");
			result.Add(BorderLineStyle.CheckedBarColor, "checkedBarColor");
			result.Add(BorderLineStyle.Checkered, "checkered");
			result.Add(BorderLineStyle.ChristmasTree, "christmasTree");
			result.Add(BorderLineStyle.CirclesLines, "circlesLines");
			result.Add(BorderLineStyle.CirclesRectangles, "circlesRectangles");
			result.Add(BorderLineStyle.ClassicalWave, "classicalWave");
			result.Add(BorderLineStyle.Clocks, "clocks");
			result.Add(BorderLineStyle.Compass, "compass");
			result.Add(BorderLineStyle.Confetti, "confetti");
			result.Add(BorderLineStyle.ConfettiGrays, "confettiGrays");
			result.Add(BorderLineStyle.ConfettiOutline, "confettiOutline");
			result.Add(BorderLineStyle.ConfettiStreamers, "confettiStreamers");
			result.Add(BorderLineStyle.ConfettiWhite, "confettiWhite");
			result.Add(BorderLineStyle.CornerTriangles, "cornerTriangles");
			result.Add(BorderLineStyle.CouponCutoutDashes, "couponCutoutDashes");
			result.Add(BorderLineStyle.CouponCutoutDots, "couponCutoutDots");
			result.Add(BorderLineStyle.CrazyMaze, "crazyMaze");
			result.Add(BorderLineStyle.CreaturesButterfly, "creaturesButterfly");
			result.Add(BorderLineStyle.CreaturesFish, "creaturesFish");
			result.Add(BorderLineStyle.CreaturesInsects, "creaturesInsects");
			result.Add(BorderLineStyle.CreaturesLadyBug, "creaturesLadyBug");
			result.Add(BorderLineStyle.CrossStitch, "crossStitch");
			result.Add(BorderLineStyle.Cup, "cup");
			result.Add(BorderLineStyle.DecoArch, "decoArch");
			result.Add(BorderLineStyle.DecoArchColor, "decoArchColor");
			result.Add(BorderLineStyle.DecoBlocks, "decoBlocks");
			result.Add(BorderLineStyle.DiamondsGray, "diamondsGray");
			result.Add(BorderLineStyle.DoubleD, "doubleD");
			result.Add(BorderLineStyle.DoubleDiamonds, "doubleDiamonds");
			result.Add(BorderLineStyle.Earth1, "earth1");
			result.Add(BorderLineStyle.Earth2, "earth2");
			result.Add(BorderLineStyle.EclipsingSquares1, "eclipsingSquares1");
			result.Add(BorderLineStyle.EclipsingSquares2, "eclipsingSquares2");
			result.Add(BorderLineStyle.EggsBlack, "eggsBlack");
			result.Add(BorderLineStyle.Fans, "fans");
			result.Add(BorderLineStyle.Film, "film");
			result.Add(BorderLineStyle.Firecrackers, "firecrackers");
			result.Add(BorderLineStyle.FlowersBlockPrint, "flowersBlockPrint");
			result.Add(BorderLineStyle.FlowersDaisies, "flowersDaisies");
			result.Add(BorderLineStyle.FlowersModern1, "flowersModern1");
			result.Add(BorderLineStyle.FlowersModern2, "flowersModern2");
			result.Add(BorderLineStyle.FlowersPansy, "flowersPansy");
			result.Add(BorderLineStyle.FlowersRedRose, "flowersRedRose");
			result.Add(BorderLineStyle.FlowersRoses, "flowersRoses");
			result.Add(BorderLineStyle.FlowersTeacup, "flowersTeacup");
			result.Add(BorderLineStyle.FlowersTiny, "flowersTiny");
			result.Add(BorderLineStyle.Gems, "gems");
			result.Add(BorderLineStyle.GingerbreadMan, "gingerbreadMan");
			result.Add(BorderLineStyle.Gradient, "gradient");
			result.Add(BorderLineStyle.Handmade1, "handmade1");
			result.Add(BorderLineStyle.Handmade2, "handmade2");
			result.Add(BorderLineStyle.HeartBalloon, "heartBalloon");
			result.Add(BorderLineStyle.HeartGray, "heartGray");
			result.Add(BorderLineStyle.Hearts, "hearts");
			result.Add(BorderLineStyle.HeebieJeebies, "heebieJeebies");
			result.Add(BorderLineStyle.Holly, "holly");
			result.Add(BorderLineStyle.HouseFunky, "houseFunky");
			result.Add(BorderLineStyle.Hypnotic, "hypnotic");
			result.Add(BorderLineStyle.IceCreamCones, "iceCreamCones");
			result.Add(BorderLineStyle.LightBulb, "lightBulb");
			result.Add(BorderLineStyle.Lightning1, "lightning1");
			result.Add(BorderLineStyle.Lightning2, "lightning2");
			result.Add(BorderLineStyle.MapleLeaf, "mapleLeaf");
			result.Add(BorderLineStyle.MapleMuffins, "mapleMuffins");
			result.Add(BorderLineStyle.MapPins, "mapPins");
			result.Add(BorderLineStyle.Marquee, "marquee");
			result.Add(BorderLineStyle.MarqueeToothed, "marqueeToothed");
			result.Add(BorderLineStyle.Moons, "moons");
			result.Add(BorderLineStyle.Mosaic, "mosaic");
			result.Add(BorderLineStyle.MusicNotes, "musicNotes");
			result.Add(BorderLineStyle.Northwest, "northwest");
			result.Add(BorderLineStyle.Ovals, "ovals");
			result.Add(BorderLineStyle.Packages, "packages");
			result.Add(BorderLineStyle.PalmsBlack, "palmsBlack");
			result.Add(BorderLineStyle.PalmsColor, "palmsColor");
			result.Add(BorderLineStyle.PaperClips, "paperClips");
			result.Add(BorderLineStyle.Papyrus, "papyrus");
			result.Add(BorderLineStyle.PartyFavor, "partyFavor");
			result.Add(BorderLineStyle.PartyGlass, "partyGlass");
			result.Add(BorderLineStyle.Pencils, "pencils");
			result.Add(BorderLineStyle.People, "people");
			result.Add(BorderLineStyle.PeopleHats, "peopleHats");
			result.Add(BorderLineStyle.PeopleWaving, "peopleWaving");
			result.Add(BorderLineStyle.Poinsettias, "poinsettias");
			result.Add(BorderLineStyle.PostageStamp, "postageStamp");
			result.Add(BorderLineStyle.Pumpkin1, "pumpkin1");
			result.Add(BorderLineStyle.PushPinNote1, "pushPinNote1");
			result.Add(BorderLineStyle.PushPinNote2, "pushPinNote2");
			result.Add(BorderLineStyle.Pyramids, "pyramids");
			result.Add(BorderLineStyle.PyramidsAbove, "pyramidsAbove");
			result.Add(BorderLineStyle.Quadrants, "quadrants");
			result.Add(BorderLineStyle.Rings, "rings");
			result.Add(BorderLineStyle.Safari, "safari");
			result.Add(BorderLineStyle.Sawtooth, "sawtooth");
			result.Add(BorderLineStyle.SawtoothGray, "sawtoothGray");
			result.Add(BorderLineStyle.ScaredCat, "scaredCat");
			result.Add(BorderLineStyle.Seattle, "seattle");
			result.Add(BorderLineStyle.ShadowedSquares, "shadowedSquares");
			result.Add(BorderLineStyle.SharksTeeth, "sharksTeeth");
			result.Add(BorderLineStyle.ShorebirdTracks, "shorebirdTracks");
			result.Add(BorderLineStyle.Skyrocket, "skyrocket");
			result.Add(BorderLineStyle.SnowflakeFancy, "snowflakeFancy");
			result.Add(BorderLineStyle.Snowflakes, "snowflakes");
			result.Add(BorderLineStyle.Sombrero, "sombrero");
			result.Add(BorderLineStyle.Southwest, "southwest");
			result.Add(BorderLineStyle.Stars, "stars");
			result.Add(BorderLineStyle.Stars3d, "stars3d");
			result.Add(BorderLineStyle.StarsBlack, "starsBlack");
			result.Add(BorderLineStyle.StarsShadowed, "starsShadowed");
			result.Add(BorderLineStyle.StarsTop, "starsTop");
			result.Add(BorderLineStyle.Sun, "sun");
			result.Add(BorderLineStyle.Swirligig, "swirligig");
			result.Add(BorderLineStyle.TornPaper, "tornPaper");
			result.Add(BorderLineStyle.TornPaperBlack, "tornPaperBlack");
			result.Add(BorderLineStyle.Trees, "trees");
			result.Add(BorderLineStyle.TriangleParty, "triangleParty");
			result.Add(BorderLineStyle.Triangles, "triangles");
			result.Add(BorderLineStyle.Tribal1, "tribal1");
			result.Add(BorderLineStyle.Tribal2, "tribal2");
			result.Add(BorderLineStyle.Tribal3, "tribal3");
			result.Add(BorderLineStyle.Tribal4, "tribal4");
			result.Add(BorderLineStyle.Tribal5, "tribal5");
			result.Add(BorderLineStyle.Tribal6, "tribal6");
			result.Add(BorderLineStyle.TwistedLines1, "twistedLines1");
			result.Add(BorderLineStyle.TwistedLines2, "twistedLines2");
			result.Add(BorderLineStyle.Vine, "vine");
			result.Add(BorderLineStyle.Waveline, "waveline");
			result.Add(BorderLineStyle.WeavingAngles, "weavingAngles");
			result.Add(BorderLineStyle.WeavingBraid, "weavingBraid");
			result.Add(BorderLineStyle.WeavingRibbon, "weavingRibbon");
			result.Add(BorderLineStyle.WeavingStrips, "weavingStrips");
			result.Add(BorderLineStyle.WhiteFlowers, "whiteFlowers");
			result.Add(BorderLineStyle.Woodwork, "woodwork");
			result.Add(BorderLineStyle.XIllusions, "xIllusions");
			result.Add(BorderLineStyle.ZanyTriangles, "zanyTriangles");
			result.Add(BorderLineStyle.ZigZag, "zigZag");
			result.Add(BorderLineStyle.ZigZagStitch, "zigZagStitch");
			return result;
		}
		readonly BorderBase border;
		public TableBorderElementDestination(WordProcessingMLBaseImporter importer, BorderBase border)
			: base(importer) {
			Guard.ArgumentNotNull(border, "border");
			this.border = border;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			BorderLineStyle borderLineStyle = Importer.GetWpEnumValue<BorderLineStyle>(reader, "val", borderStyleTable, BorderLineStyle.None);
			Color color = Importer.GetWpSTColorValue(reader, "color");
			bool frame = Importer.GetWpSTOnOffValue(reader, "frame", false);
			bool shadow = Importer.GetWpSTOnOffValue(reader, "shadow", false);
			bool isDefaultValue = borderLineStyle == BorderLineStyle.None && Object.Equals(color, DXColor.Empty) &&
				frame == false && shadow == false;
			if (!isDefaultValue) {
				border.Style = borderLineStyle;
				border.Color = color;
				border.Frame = frame;
				border.Shadow = shadow;
			}
			int value = Importer.GetWpSTIntegerValue(reader, "space");
			if (value != Int32.MinValue)
				border.Offset = UnitConverter.PointsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "sz");
			if (value != Int32.MinValue)
				border.Width = (int)UnitConverter.PointsToModelUnitsF(value * 0.125f);
#if THEMES_EDIT            
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			border.ColorModelInfo = helper.SaveColorModelInfo(Importer, reader, "color");
#endif
		}
	}
	#endregion
	#region TableRowCantSplitDestination
	public class TableRowCantSplitDestination : TableRowPropertiesLeafElementDestination {
		public TableRowCantSplitDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			RowProperties.CantSplit = Importer.GetWpSTOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region TableRowGridAfterDestination
	public class TableRowGridAfterDestination : TableRowPropertiesLeafElementDestination {
		public TableRowGridAfterDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue)
				RowProperties.GridAfter = Math.Max(0, value);
		}
	}
	#endregion
	#region TableRowGridBeforeDestination
	public class TableRowGridBeforeDestination : TableRowPropertiesLeafElementDestination {
		public TableRowGridBeforeDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue)
				RowProperties.GridBefore = Math.Max(0, value);
		}
	}
	#endregion
	#region TableRowHideCellMarkDestination
	public class TableRowHideCellMarkDestination : TableRowPropertiesLeafElementDestination {
		public TableRowHideCellMarkDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			RowProperties.HideCellMark = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region TableRowAlignmentDestination
	public class TableRowAlignmentDestination : TableRowPropertiesLeafElementDestination {
		static Dictionary<TableRowAlignment, string> tableRowAlignmentTable = CreateTableRowAlignmentTable();
		static Dictionary<TableRowAlignment, string> CreateTableRowAlignmentTable() {
			Dictionary<TableRowAlignment, string> result = new Dictionary<TableRowAlignment, string>();
			result.Add(TableRowAlignment.Both, "both");
			result.Add(TableRowAlignment.Center, "center");
			result.Add(TableRowAlignment.Distribute, "distribute");
			result.Add(TableRowAlignment.Left, "left");
			result.Add(TableRowAlignment.NumTab, "numTab");
			result.Add(TableRowAlignment.Right, "right");
			return result;
		}
		public TableRowAlignmentDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			RowProperties.TableRowAlignment = Importer.GetWpEnumValue<TableRowAlignment>(reader, "val", tableRowAlignmentTable, TableRowAlignment.Left);
		}
	}
	#endregion
	#region TableRowHeaderDestination
	public class TableRowHeaderDestination : TableRowPropertiesLeafElementDestination {
		public TableRowHeaderDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			RowProperties.Header = Importer.GetWpSTOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region TableRowConditionalFormattingDestination
	public class TableRowConditionalFormattingDestination : TableRowPropertiesLeafElementDestination {
		public TableRowConditionalFormattingDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
		}
	}
	#endregion
	#region TableCellMarginsDestination
	public class TableCellMarginsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("top", OnTopCellMargin);
			result.Add("left", OnLeftCellMargin);
			result.Add("bottom", OnBottomCellMargin);
			result.Add("right", OnRightCellMargin);
			return result;
		}
		static TableCellMarginsDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellMarginsDestination)importer.PeekDestination();
		}
		static CellMargins GetCellMargins(WordProcessingMLBaseImporter importer) {
			TableCellMarginsDestination destination = GetThis(importer);
			return destination.CellMargins;
		}
		static Destination OnTopCellMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetCellMargins(importer).Top);
		}
		static Destination OnLeftCellMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetCellMargins(importer).Left);
		}
		static Destination OnBottomCellMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetCellMargins(importer).Bottom);
		}
		static Destination OnRightCellMargin(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidthUnitDestination(importer, GetCellMargins(importer).Right);
		}
		readonly CellMargins cellMargins;
		public TableCellMarginsDestination(WordProcessingMLBaseImporter importer, CellMargins cellMargins)
			: base(importer) {
			Guard.ArgumentNotNull(cellMargins, "cellMargins");
			this.cellMargins = cellMargins;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal CellMargins CellMargins { get { return cellMargins; } }
	}
	#endregion
	#region TableCellShadingDestination
	public class TableShadingDestination : TablePropertiesLeafElementDestination {
		public TableShadingDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
	}
	#endregion
	#region TableCellBordersDestination
	public class TableCellBordersDestination : TableCellPropertiesElementBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("top", OnTopBorder);
			result.Add("left", OnLeftBorder);
			result.Add("bottom", OnBottomBorder);
			result.Add("right", OnRightBorder);
			result.Add("insideH", OnInsideHorizontalBorder);
			result.Add("insideV", OnInsideVerticalBorder);
			result.Add("tl2br", OnTopLeftDiagonalBorder);
			result.Add("tr2bl", OnTopRightDiagonalBorder);
			return result;
		}
		static TableCellBorders GetCellBorders(WordProcessingMLBaseImporter importer) {
			return GetProperties(importer).Borders;
		}
		static Destination OnTopBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).TopBorder);
		}
		static Destination OnLeftBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).LeftBorder);
		}
		static Destination OnBottomBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).BottomBorder);
		}
		static Destination OnRightBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).RightBorder);
		}
		static Destination OnInsideHorizontalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).InsideHorizontalBorder);
		}
		static Destination OnInsideVerticalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).InsideVerticalBorder);
		}
		static Destination OnTopLeftDiagonalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).TopLeftDiagonalBorder);
		}
		static Destination OnTopRightDiagonalBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableBorderElementDestination(importer, GetCellBorders(importer).TopRightDiagonalBorder);
		}
		public TableCellBordersDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableCellVerticalMergingStateDestination
	public class TableCellVerticalMergingStateDestination : TableCellPropertiesLeafElementDestination {
		static Dictionary<MergingState, string> mergingStateTable = CreateMergingStateTable();
		static Dictionary<MergingState, string> CreateMergingStateTable() {
			Dictionary<MergingState, string> result = new Dictionary<MergingState, string>();
			result.Add(MergingState.None, "none");
			result.Add(MergingState.Restart, "restart");
			result.Add(MergingState.Continue, "continue");
			return result;
		}
		public TableCellVerticalMergingStateDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			MergingState state = Importer.GetWpEnumValue<MergingState>(reader, "val", mergingStateTable, MergingState.Continue);
			CellProperties.VerticalMerging = state;
		}
	}
	#endregion
	#region TableCellShadingDestination
	public class TableCellShadingDestination : TableCellPropertiesLeafElementDestination {
		public TableCellShadingDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			if (!DXColor.IsTransparentOrEmpty(CellProperties.BackgroundColor))
				return;
			ShadingPattern pattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			Color fill = Importer.GetWpSTColorValue(reader, "fill", DXColor.Empty);
			Color patternColor = Importer.GetWpSTColorValue(reader, "color", DXColor.Empty);
			Color actualColor = ShadingHelper.GetActualBackColor(fill, patternColor, pattern);
			if (actualColor != DXColor.Empty)
				CellProperties.BackgroundColor = actualColor;
#if THEMES_EDIT
			Shading shading = CellProperties.Shading;
			shading.ShadingPattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			shading.ColorInfo = helper.SaveColorModelInfo(Importer, reader, "color");
			shading.FillInfo = helper.SaveFillInfo(Importer, reader);
#endif
		}
	}
	#endregion
	#region TableCellColumnSpanDestination
	public class TableCellColumnSpanDestination : TableCellPropertiesLeafElementDestination {
		public TableCellColumnSpanDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementClose(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val");
			if (value != Int32.MinValue && value != 0)
				CellProperties.ColumnSpan = value;
		}
	}
	#endregion
	#region TableCellFitTextDestination
	public class TableCellFitTextDestination : TableCellPropertiesLeafElementDestination {
		public TableCellFitTextDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellProperties.FitText = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region TableCellNoWrapDestination
	public class TableCellNoWrapDestination : TableCellPropertiesLeafElementDestination {
		public TableCellNoWrapDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellProperties.NoWrap = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region TableCellHideMarkDestination
	public class TableCellHideMarkDestination : TableCellPropertiesLeafElementDestination {
		public TableCellHideMarkDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellProperties.HideCellMark = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region TableCellTextDirectionDestination
	public class TableCellTextDirectionDestination : TableCellPropertiesLeafElementDestination {
		static Dictionary<TextDirection, string> textDirectionTable = CreateTextDirectionTable();
		static Dictionary<TextDirection, string> CreateTextDirectionTable() {
			Dictionary<TextDirection, string> result = new Dictionary<TextDirection, string>();
			result.Add(TextDirection.BottomToTopLeftToRight, "btLr");
			result.Add(TextDirection.LeftToRightTopToBottom, "lrTb");
			result.Add(TextDirection.LeftToRightTopToBottomRotated, "lrTbV");
			result.Add(TextDirection.TopToBottomLeftToRightRotated, "tbLrV");
			result.Add(TextDirection.TopToBottomRightToLeft, "tbRl");
			result.Add(TextDirection.TopToBottomRightToLeftRotated, "tbRlV");
			return result;
		}
		public TableCellTextDirectionDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellProperties.TextDirection = Importer.GetWpEnumValue<TextDirection>(reader, "val", textDirectionTable, TextDirection.LeftToRightTopToBottom);
		}
	}
	#endregion
	#region TableCellVerticalAlignmentDestination
	public class TableCellVerticalAlignmentDestination : TableCellPropertiesLeafElementDestination {
		static Dictionary<VerticalAlignment, string> verticalAlignmentTable = CreateVerticalAlignmentTable();
		static Dictionary<VerticalAlignment, string> CreateVerticalAlignmentTable() {
			Dictionary<VerticalAlignment, string> result = new Dictionary<VerticalAlignment, string>();
			result.Add(VerticalAlignment.Both, "both");
			result.Add(VerticalAlignment.Bottom, "bottom");
			result.Add(VerticalAlignment.Center, "center");
			result.Add(VerticalAlignment.Top, "top");
			return result;
		}
		public TableCellVerticalAlignmentDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellProperties.VerticalAlignment = Importer.GetWpEnumValue<VerticalAlignment>(reader, "val", verticalAlignmentTable, VerticalAlignment.Top);
		}
	}
	#endregion
	#region TableCellConditionalFormattingDestination
	public class TableCellConditionalFormattingDestination : TableCellPropertiesLeafElementDestination {
		public TableCellConditionalFormattingDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string strValue = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(strValue))
				return;
			int value = Convert.ToInt32(strValue, 2);
			CellProperties.CellConditionalFormatting = (ConditionalTableStyleFormattingTypes)value;
		}
	}
	#endregion
	#region PropertiesDestinationBase (abstract class)
	public abstract class TablePropertiesBaseDestination<T> : ElementDestination where T : IPropertiesContainer {
		readonly T properties;
		protected TablePropertiesBaseDestination(WordProcessingMLBaseImporter importer, T properties)
			: base(importer) {
			Guard.ArgumentNotNull(properties, "properties");
			this.properties = properties;
		}
		protected T Properties { get { return properties; } }
		public override void ProcessElementOpen(XmlReader reader) {
			properties.BeginPropertiesUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			properties.EndPropertiesUpdate();
		}
	}
	#endregion
	#region TablePropertiesElementBaseDestination (abstract class)
	public abstract class TablePropertiesElementBaseDestination : ElementDestination {
		static TablePropertiesElementBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TablePropertiesElementBaseDestination)importer.PeekDestination();
		}
		protected static TableProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TablePropertiesElementBaseDestination destination = GetThis(importer);
			return (TableProperties)destination.TableProperties;
		}
		readonly TableProperties tableProperties;
		protected TablePropertiesElementBaseDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer) {
			Guard.ArgumentNotNull(tableProperties, "tableProperties");
			this.tableProperties = tableProperties;
		}
		protected internal TableProperties TableProperties { get { return tableProperties; } }
	}
	#endregion
	#region TablePropertiesLeafElementDestination (abstract class)
	public abstract class TablePropertiesLeafElementDestination : TablePropertiesElementBaseDestination {
		protected TablePropertiesLeafElementDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
	#region TableRowPropertiesElementBaseDestination (abstract class)
	public abstract class TableRowPropertiesElementBaseDestination : ElementDestination {
		static TableRowPropertiesElementBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableRowPropertiesElementBaseDestination)importer.PeekDestination();
		}
		protected static TableRowProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TableRowPropertiesElementBaseDestination destination = GetThis(importer);
			return (TableRowProperties)destination.RowProperties;
		}
		readonly TableRowProperties rowProperties;
		protected TableRowPropertiesElementBaseDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer) {
			Guard.ArgumentNotNull(rowProperties, "rowProperties");
			this.rowProperties = rowProperties;
		}
		protected internal TableRowProperties RowProperties { get { return rowProperties; } }
	}
	#endregion
	#region TableRowPropertiesLeafElementDestination (abstract class)
	public abstract class TableRowPropertiesLeafElementDestination : TableRowPropertiesElementBaseDestination {
		protected TableRowPropertiesLeafElementDestination(WordProcessingMLBaseImporter importer, TableRowProperties rowProperties)
			: base(importer, rowProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
	#region TableCellPropertiesElementBaseDestination (abstract class)
	public abstract class TableCellPropertiesElementBaseDestination : ElementDestination {
		static TableCellPropertiesElementBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellPropertiesElementBaseDestination)importer.PeekDestination();
		}
		protected static TableCellProperties GetProperties(WordProcessingMLBaseImporter importer) {
			TableCellPropertiesElementBaseDestination destination = GetThis(importer);
			return (TableCellProperties)destination.CellProperties;
		}
		readonly TableCellProperties cellProperties;
		protected TableCellPropertiesElementBaseDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer) {
			Guard.ArgumentNotNull(cellProperties, "cellProperties");
			this.cellProperties = cellProperties;
		}
		protected internal TableCellProperties CellProperties { get { return cellProperties; } }
	}
	#endregion
	#region TableCellPropertiesLeafElementDestination (abstract class)
	public abstract class TableCellPropertiesLeafElementDestination : TableCellPropertiesElementBaseDestination {
		protected TableCellPropertiesLeafElementDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
	#region Disabled Tables by DocumentCapabilites
	#region TableDisabledDestination
	public class TableDisabledDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tr", OnRow);
			return result;
		}
		public TableDisabledDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRow(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowDisabledDestination(importer);
		}
	}
	#endregion
	#region TableRowDisabledDestination
	public class TableRowDisabledDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tc", OnCell);
			return result;
		}
		public TableRowDisabledDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCell(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellDisabledDestination(importer);
		}
	}
	#endregion
	#region TableCellDisabledDestination
	public class TableCellDisabledDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("altChunk", OnAltChunk);
			return result;
		}
		public TableCellDisabledDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		protected static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableDisabledDestination(importer);
		}
		protected static Destination OnBookmarkStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkStartElementDestination(reader);
		}
		protected static Destination OnBookmarkEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkEndElementDestination(reader);
		}
		protected static Destination OnRangePermissionStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionStartElementDestination(importer);
		}
		protected static Destination OnRangePermissionEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionEndElementDestination(importer);
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
	}
	#endregion
	#endregion
}
