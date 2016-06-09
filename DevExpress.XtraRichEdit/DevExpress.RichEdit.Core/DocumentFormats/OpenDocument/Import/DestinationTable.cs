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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region TablePropertiesInfo
	public class TablePropertiesInfo {
		TableProperties tableProperties;
		ParagraphBreaksInfo breaks;
		TableRowAlignment rowAlignment;
		string masterPageName;
		public TablePropertiesInfo(PieceTable pieceTable) {
			this.tableProperties = new TableProperties(pieceTable);
			this.breaks = new ParagraphBreaksInfo();
			this.rowAlignment = TableRowAlignment.Left;
		}
		public TableProperties TableProperties { get { return tableProperties; } }
		public ParagraphBreaksInfo Breaks { get { return breaks; } set { breaks = value; } }
		public TableRowAlignment RowAlignment { get { return rowAlignment; } set { rowAlignment = value; } }
		public string MasterPageName { get { return masterPageName; } set { masterPageName = value; } }
		public void Reset() {
			TableProperties.Reset();
			Breaks.Reset();
		}
	}
	#endregion
	#region TableColumnWidthInfo
	public class TableColumnWidthInfo {
		int width;
		bool useOptimalColumnWidth;
		public TableColumnWidthInfo(int width, bool useOptimalColumnWidth) {
			this.width = width;
			this.useOptimalColumnWidth = useOptimalColumnWidth;
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public bool UseOptimalColumnWidth {
			get { return useOptimalColumnWidth; }
			set { useOptimalColumnWidth = value; }
		}
	}
	#endregion
	#region TableColumnWidthInfo
	public class TableRowInfo {
		TableRowProperties properties;
		public TableRowInfo(PieceTable pieceTable) {
			this.properties = new TableRowProperties(pieceTable);
		}
		public TableRowProperties Properties { get { return properties; } set { properties = value; } }
	}
	#endregion
	#region TableHeaderRowsDestination
	public class TableHeaderRowsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-row", OnTableRow);
			return result;
		}
		static Destination OnTableRow(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableRowDestination(importer);
		}
		readonly Table table;
		int row;
		public TableHeaderRowsDestination(OpenDocumentTextImporter importer, Table table)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		public int RowIndex { get { return row; } set { row = value; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			RowIndex = Table.Rows.Count;
		}
		public override void ProcessElementClose(XmlReader reader) {
			TableRowCollection rows = Table.Rows;
			for (int i = RowIndex; i < rows.Count; i++)
				rows[i].Header = true;
		}
	}
	#endregion
	#region TableDestination
	public class TableDestination : ElementDestination {
		#region ElementHandlerTable
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-row", OnTableRow);
			result.Add("table-column", OnTableColumn);
			result.Add("table-columns", OnTableColumns);
			result.Add("table-header-rows", OnHeaderRows);
			return result;
		}
		#region Handlers
		static TableDestination GetThis(OpenDocumentTextImporter importer) {
			return (TableDestination)importer.PeekDestination();
		}
		static Destination OnTableRow(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableRowDestination(importer);
		}
		static Destination OnTableColumn(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableColumnDestination(importer);
		}
		static Destination OnTableColumns(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableColumnsDestination(importer);
		}
		static Destination OnHeaderRows(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableHeaderRowsDestination(importer, GetThis(importer).Table);
		}
		#endregion
		#endregion
		readonly TableCell parentCell;
		public TableDestination(OpenDocumentTextImporter importer, TableCell parentCell)
			: base(importer) {
			this.parentCell = parentCell;
		}
		public TableDestination(OpenDocumentTextImporter importer)
			: this(importer, null) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal Table Table { get { return Importer.InputPosition.TablesImportHelper.Table; } }
		public TableCell ParentCell { get { return parentCell; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.InputPosition.TablesImportHelper.CreateTable(ParentCell); 
			ApplyTableProperties(reader);
			Importer.IncTablesCount();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.InputPosition.CurrentTableCellReference.ProcessTableClose();
			Importer.InputPosition.TablesImportHelper.FinalizeTableCreation();
			Importer.DecTablesCount();
		}
		void ApplyTableProperties(XmlReader reader) {
			string styleName = ImportHelper.GetTableStringAttribute(reader, "style-name");
			TablePropertiesInfo properties;
			if (Importer.TableAutoStyles.TryGetValue(styleName, out properties)) {
				ApplyTableAutoStyle(properties);
				Importer.InputPosition.TablesImportHelper.TableInfo.MasterPageName = properties.MasterPageName;
				Importer.InputPosition.TablesImportHelper.TableInfo.RowAlignment = properties.RowAlignment;
			}
		}
		void ApplyTableAutoStyle(TablePropertiesInfo tableInfo) {
			Table.TableProperties.Merge(tableInfo.TableProperties);
		}
	}
	#endregion
	#region TableRowDestination
	public class TableRowDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-cell", OnCell);
			result.Add("covered-table-cell", OnCoveredCell);
			return result;
		}
		static TableRowDestination GetThis(OpenDocumentTextImporter importer) {
			return (TableRowDestination)importer.PeekDestination();
		}
		static Destination OnCell(OpenDocumentTextImporter importer, XmlReader reader) {
			TableCellDestination result= new TableCellDestination(importer, GetThis(importer).Row);
			importer.InputPosition.CurrentTableCellReference.ProcessNewCell(result);
			return result;
		}
		static Destination OnCoveredCell(OpenDocumentTextImporter importer, XmlReader reader) {
			TableCellCoveredDestination result = new TableCellCoveredDestination(importer, GetThis(importer).Row);
			return result;
		}
		TableRow row;
		public TableRowDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal TableRow Row { get { return row; } set { row = value; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Row = Importer.InputPosition.TablesImportHelper.CreateNewRowOrGetLastEmpty();
			ApplyStyle(reader);
			ApplyRowAlignment(Row);
		}
		void ApplyStyle(XmlReader reader) {
			string styleName = ImportHelper.GetTableStringAttribute(reader, "style-name");
			if (Importer.TableRowsAutoStyles.ContainsKey(styleName)) {
				TableRowProperties properties = Importer.TableRowsAutoStyles[styleName].Properties;
				Row.Properties.Height.CopyFrom(properties.Height);
				Row.Properties.CantSplit = properties.CantSplit;
			}
		}
		protected internal virtual void ApplyRowAlignment(TableRow row) {
			TableRowAlignment align = Importer.InputPosition.TablesImportHelper.TableInfo.RowAlignment;
			if (align != TableRowAlignment.Left)
				row.Properties.TableRowAlignment = align;
		}
		public override void ProcessElementClose(XmlReader reader) {
			TableRow lastRow = Importer.InputPosition.TablesImportHelper.Table.LastRow;
			if (lastRow == null)
				return;
			Importer.InputPosition.TablesImportHelper.RemoveEmptyRow();
			Importer.InputPosition.TablesImportHelper.MoveNextRow();
		}
	}
	#endregion
	#region TableColumnDestination
	public class TableColumnDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		public TableColumnDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string styleName = ImportHelper.GetTableStringAttribute(reader, "style-name");
			if (Importer.TableColumnAutoStyles.ContainsKey(styleName)) {
				int width = Importer.TableColumnAutoStyles[styleName].Width;
				Importer.InputPosition.TablesImportHelper.TableInfo.UseOptimalColumnWidth |= Importer.TableColumnAutoStyles[styleName].UseOptimalColumnWidth;
				int numberColumnsRepeated = ImportHelper.GetTableIntegerAttribute(reader, "number-columns-repeated", 1);
				for (int i = 0; i < numberColumnsRepeated; i++) {					
					Importer.InputPosition.TablesImportHelper.TableInfo.AddColumnWidth(width);
				}
			}
		}
	}
	#endregion
	#region TableColumnsDestination
	public class TableColumnsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-column", OnTableColumn);
			return result;
		}
		public TableColumnsDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		static Destination OnTableColumn(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableColumnDestination(importer);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	public interface ITableCellDestination {
		ParagraphIndex EndParagraphIndex { get; set; }
		bool IsEmptyCell { get; set; }
	}
	#region TableCellDestination
	public class TableCellDestination : TextDestination, ICellPropertiesOwner, ITableCellDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("h", OnHeading);
			result.Add("list", OnList);
			result.Add("table-of-content", OnTableOfContent);
			result.Add("table", OnInnerTable);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		static TableCellDestination GetThis(OpenDocumentTextImporter importer) {
			return (TableCellDestination)importer.PeekDestination();
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			importer.InputPosition.RegisterParagraphForTableCellDestination();
			return new ParagraphDestination(importer);
		}
		static Destination OnHeading(OpenDocumentTextImporter importer, XmlReader reader) {
			importer.InputPosition.RegisterParagraphForTableCellDestination();
			return new HeadingDestination(importer);
		}
		static Destination OnInnerTable(OpenDocumentTextImporter importer, XmlReader reader) {
			TableCellDestination destination = GetThis(importer);
			destination.isEmptyCell = false;
			return new TableDestination(importer, destination.Cell);
		}
		#region Fields
		readonly TableCell cell;
		TableCellProperties properties;
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		bool isEmptyCell = true;
		int rowSpanedCounter;
		int columnSpannedCounter ;
		#endregion
		public TableCellDestination(OpenDocumentTextImporter importer, TableRow row)
			: base(importer) {
			Guard.ArgumentNotNull(row, "Row");
				this.startParagraphIndex = importer.InputPosition.ParagraphIndex;
			this.endParagraphIndex = this.startParagraphIndex;
			this.cell = new TableCell(row);
			this.cell.Row.Cells.AddInternal(this.cell);
			this.properties = new TableCellProperties(importer.PieceTable, this);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal TableCell Cell { get { return cell; } }
		public TableCellProperties Properties { get { return properties; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public bool IsEmptyCell { get { return isEmptyCell; } set { isEmptyCell = value; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string styleName = MergeCellProperties(reader);
			ImportMerging(reader, styleName);
		}
		protected internal virtual string MergeCellProperties(XmlReader reader) {
			string styleName = ImportHelper.GetTableStringAttribute(reader, "style-name");
			if (Importer.TableCellsAutoStyles.ContainsKey(styleName)) {
				TableCellProperties cellProperties = Importer.TableCellsAutoStyles[styleName];
				Properties.Merge(cellProperties);
			}
			return styleName;
		}
		protected internal virtual void ImportMerging(XmlReader reader, string styleName) {
			this.rowSpanedCounter = ImportHelper.GetTableIntegerAttribute(reader, "number-rows-spanned", 1);
			if (this.rowSpanedCounter > 1)
				Cell.VerticalMerging = MergingState.Restart;
			this.columnSpannedCounter = ImportHelper.GetTableIntegerAttribute(reader, "number-columns-spanned", 1);
			if (this.columnSpannedCounter > 1)
				Cell.ColumnSpan = this.columnSpannedCounter;
			OpenDocumentTablesImportHelper helper = Importer.InputPosition.TablesImportHelper;
			if (helper.Table.Rows.Count == 1)
				helper.AddCellToSpanCollection(this.rowSpanedCounter, this.columnSpannedCounter);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Cell.Properties.BeginInit();
			Cell.Properties.Merge(Properties);
			Cell.Properties.EndInit();
			InitializeCell();
			Importer.InputPosition.CurrentTableCellReference.ProcessCellClose();
		}
		protected internal virtual void InitializeCell() {
			bool needInsertParagraph = IsInnerTableClosedWithoutParagraphAfterIt(Importer.InputPosition.ParagraphIndex);
			if (isEmptyCell || needInsertParagraph) {
				Importer.InputPosition.RegisterParagraphForTableCellDestination();
				Importer.InsertParagraph();
			}
			Importer.InputPosition.TablesImportHelper.InitializeTableCell(Cell, StartParagraphIndex, EndParagraphIndex);
			Importer.InputPosition.TablesImportHelper.UpdateFirstRowSpanCollection(this.rowSpanedCounter, this.columnSpannedCounter);
		}
		bool IsInnerTableClosedWithoutParagraphAfterIt(ParagraphIndex paragraphIndex) {
			TableCell cell = Importer.PieceTable.Paragraphs[EndParagraphIndex].GetCell();
			return  cell != null;
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == this.properties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region TableCellCoveredDestination
	public class TableCellCoveredDestination : LeafElementDestination, ITableCellDestination {
		#region Fields
		ParagraphIndex endParagraphIndex;
		#endregion
		public TableCellCoveredDestination(OpenDocumentTextImporter importer, TableRow row)
			: base(importer) {
			Guard.ArgumentNotNull(row, "row");
		}
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public bool IsEmptyCell { get { return false; } set {  } }
	}
	#endregion
	#region OdtImportedTableInfo
	public class OdtImportedTableInfo : ImportedTableInfo {
		#region Fields
		TableRowAlignment rowAlignment;
		readonly List<int> columnWidths;
		string masterPageName;
		#endregion
		public OdtImportedTableInfo(Table table)
			: base(table) {
			this.rowAlignment = TableRowAlignment.Left;
			this.columnWidths = new List<int>();
		}
		#region Properties
		public TableRowAlignment RowAlignment { get { return rowAlignment; } set { rowAlignment = value; } }
		List<int> ColumnWidths { get { return columnWidths; } }
		public string MasterPageName { get { return masterPageName; } set { masterPageName = value; } }
		public bool HasMasterPage { get { return !String.IsNullOrEmpty(masterPageName); } }
		public bool UseOptimalColumnWidth { get; set; }
		#endregion
		public void AddColumnWidth(int width) {
			ColumnWidths.Add(width);
		}
		public int GetCellWidth(int columnIndex, int columnSpannedCounter) {
			if (columnWidths.Count == 0 || columnWidths.Count <= columnIndex)
				return 0;
			if (columnSpannedCounter == 1)
				return columnWidths[columnIndex];
			int totalWidth = 0;
			int endSpannedColumnId = columnIndex + columnSpannedCounter;
			for (int i = columnIndex; i < endSpannedColumnId; i++) {
				totalWidth += columnWidths[i];
			}
			return totalWidth;
		}
	}
	#endregion
	#region OpenDocumentTablesImportHelper
	public class OpenDocumentTablesImportHelper : TablesImportHelper {
		readonly OpenDocumentInputPosition inputPosition;
		public OpenDocumentTablesImportHelper(PieceTable pieceTable, OpenDocumentInputPosition inputPosition)
			: base(pieceTable) {
			Guard.ArgumentNotNull(inputPosition, "inputPosition");
			this.inputPosition = inputPosition;
		}
		public OpenDocumentInputPosition InputPosition { get { return inputPosition; } }
		public new OdtImportedTableInfo TableInfo {
			get { return (base.TableInfo == null) ? null : (OdtImportedTableInfo)base.TableInfo; } 
		}
		public new OdtImportedTableInfo TopLevelTableInfo {
			get { return (OdtImportedTableInfo)base.TopLevelTableInfo; }
		}
		protected override ImportedTableInfo CreateTableInfo(Table newTable) {
			return new OdtImportedTableInfo(newTable);
		}
		protected internal override TableCell CreateCoveredCellWithEmptyParagraph(TableRow lastRow) {
			InputPosition.RegisterParagraphForTableCellDestination();
			ParagraphIndex start = InputPosition.ParagraphIndex;
			ParagraphIndex end = InputPosition.ParagraphIndex;
			PieceTable.InsertParagraphCore(InputPosition);
			TableCell cell = new TableCell(lastRow);
			cell.Row.Cells.AddInternal(cell);
			InitializeTableCell(cell, start, end);
			return cell;
		}
		public override void InitializeTableCell(TableCell cell, ParagraphIndex startParagraphIndex, ParagraphIndex endParagraphIndex) {
			base.InitializeTableCell(cell, startParagraphIndex, endParagraphIndex);
		}
		public override void PostProcessTableCells() {
			SetCellsWidths();
		}
		void SetCellsWidths() {
			TableRowCollection rows = Table.Rows;
			TableLayoutType layoutType = TableInfo.UseOptimalColumnWidth ? TableLayoutType.Autofit : TableLayoutType.Fixed;
			if (Table.TableLayout != layoutType)
				Table.TableLayout = layoutType;
			int count = rows.Count;
			for (int rowID = 0; rowID < count; rowID++) {
				TableRow row = rows[rowID];
				TableCellCollection cells = row.Cells;
				int cellsCount = cells.Count;
				int columnIndex = row.GridBefore;
				for (int cellId = 0; cellId < cellsCount; cellId++) {
					TableCell cell = cells[cellId];
					WidthUnitInfo width = new WidthUnitInfo();
					width.Value = TableInfo.GetCellWidth(columnIndex, cell.ColumnSpan);
					width.Type = WidthUnitType.ModelUnits;
					cell.Properties.PreferredWidth.CopyFrom(width);
					columnIndex += cell.ColumnSpan;
				}
				columnIndex += row.GridAfter;
			}
		}
	}
	#endregion
	#region TableDisabledDestination
	public class TableDisabledDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-row", OnTableRow);
			return result;
		}
		public TableDisabledDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnTableRow(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableRowDisabledDestination(importer);
		}
	}
	#endregion
	#region TableRowDisabledDestination
	public class TableRowDisabledDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-cell", OnCell);
			return result;
		}
		static Destination OnCell(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableCellDisabledDestination(importer);
		}
		public TableRowDisabledDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableCellDisabledDestination
	public class TableCellDisabledDestination : TextDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("h", OnHeading);
			result.Add("list", OnList);
			result.Add("table-of-content", OnTableOfContent);
			result.Add("table", OnInnerTable);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		static Destination OnHeading(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		static Destination OnInnerTable(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableDisabledDestination(importer);
		}
		public TableCellDisabledDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	public class OpenDocumentTableCellReference {
		Stack<ITableCellDestination> cellDestinationStack;
		public OpenDocumentTableCellReference() {
			this.cellDestinationStack = new Stack<ITableCellDestination>();
		}
		public Stack<ITableCellDestination> CellDestinationStack {
			get { return cellDestinationStack; }
			set { cellDestinationStack = value; }
		}
		public ITableCellDestination CurrentCell { 
			get { return CellDestinationStack.Count > 0 ? CellDestinationStack.Peek(): null; } 
		}
		public void ProcessCellClose() {
			ParagraphIndex lastEnd = CellDestinationStack.Peek().EndParagraphIndex;
			CellDestinationStack.Pop();
			ITableCellDestination prevCell = CellDestinationStack.Count > 0 ? CellDestinationStack.Peek() : null;
			if (prevCell != null) {
				prevCell.EndParagraphIndex = lastEnd;
			}
		}
		public void ProcessNewCell(ITableCellDestination dest) {
			CellDestinationStack.Push(dest);
		}
		public void ProcessTableClose() {
		}
	}
}
