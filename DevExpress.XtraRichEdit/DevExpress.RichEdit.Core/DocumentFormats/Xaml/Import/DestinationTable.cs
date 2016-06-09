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
using System.Globalization;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Xaml {
	#region TableDestination
	public class TableDestination : BlockElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableRowGroup", OnTableRowGroup);
			return result;
		}
		static Destination OnTableRowGroup(XamlImporter importer, XmlReader reader) {
			return new TableRowGroupDestination(importer);
		}
		readonly TableCell parentCell;
		public TableDestination(XamlImporter importer, TableCell parentCell)
			: base(importer) {
			this.parentCell = parentCell;
		}
		public TableDestination(XamlImporter importer)
			:this(importer, null){
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableCell ParentCell { get { return parentCell; } }
		public bool TablesDisabled { get { return !Importer.TablesImportHelper.TablesAllowed; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (TablesDisabled)
				return;
			Importer.TablesImportHelper.CreateTable(ParentCell); 
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (TablesDisabled)
				return;
			Importer.TablesImportHelper.FinalizeTableCreation();
		}
	}
	#endregion
	#region TableRowGroupDestination
	public class TableRowGroupDestination : TextElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableRow", OnTableRow);
			return result;
		}
		static Destination OnTableRow(XamlImporter importer, XmlReader reader) {
			return new TableRowDestination(importer);
		}
		public TableRowGroupDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	} 
	#endregion
	#region TableRowDestination
	public class TableRowDestination : TextElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableCell", OnTableCell);
			return result;
		}
		static Destination OnTableCell(XamlImporter importer, XmlReader reader) {
			return new TableCellDestination(importer);
		}
		TableRow row;
		public TableRowDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal TableRow Row { get { return row; } set { row = value; } }
		public bool TablesDisabled {
			get {
				return !Importer.TablesImportHelper.TablesAllowed || !Importer.TablesImportHelper.IsInTable;
			}
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			Row = Importer.TablesImportHelper.CreateNewRowOrGetLastEmpty();
		}
		public override void  ProcessElementClose(XmlReader reader){
 			base.ProcessElementClose(reader);
			if (TablesDisabled)
				return;
			TableRow lastRow = Importer.TablesImportHelper.Table.LastRow;
			if (lastRow == null)
				return;
			Importer.TablesImportHelper.RemoveEmptyRow();
			Importer.TablesImportHelper.MoveNextRow();
		}
	} 
	#endregion
	#region TableCellDestination
	public class TableCellDestination : BlockElementDestination, ICellPropertiesOwner {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("Paragraph", OnParagraph);
			result.Add("Section", OnSection);
			result.Add("BlockUIContainer", OnBlockUIContainer);
			result.Add("Table", OnInnerTable);
			result.Add("List", OnList);
			return result;
		}
		static TableCellDestination GetThis(XamlImporter importer) {
			return (TableCellDestination)importer.PeekDestination();
		}
		static Destination OnParagraph(XamlImporter importer, XmlReader reader) {
			TableCellDestination destination = GetThis(importer);
			destination.EndParagraphIndex = importer.Position.ParagraphIndex;
			destination.isEmptyCell = false;
			return new ParagraphDestination(importer);
		}
		public static Destination OnInnerTable(XamlImporter importer, XmlReader reader) {
			TableCellDestination destination = GetThis(importer);
			destination.isEmptyCell = false;
			return new TableDestination(importer, destination.Cell);
		}
		TableCell cell;
		ParagraphIndex startParagraphIndex;
		ParagraphIndex endParagraphIndex;
		bool isEmptyCell = true;
		TableCellProperties properties;
		int rosSpan;
		int colSpan;
		bool tdTagWithoutOpenedTable;
		public TableCellDestination(XamlImporter importer)
			: base(importer) {
			this.properties = new TableCellProperties(Importer.PieceTable, this);
			tdTagWithoutOpenedTable = false;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal TableCell Cell { get { return cell; } }
		public TableCellProperties Properties { get { return properties; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public ParagraphIndex EndParagraphIndex { get { return endParagraphIndex; } set { endParagraphIndex = value; } }
		public int RosSpan { get { return rosSpan; } set { rosSpan = value; } }
		public int ColSpan { get { return colSpan; } set { colSpan = value; } }
		public bool TdTagWithoutOpenedTable { get { return tdTagWithoutOpenedTable; } set { tdTagWithoutOpenedTable = value; } }
		public bool TablesDisabled {
			get {
				return !Importer.TablesImportHelper.TablesAllowed || !Importer.TablesImportHelper.IsInTable;
			}
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (TablesDisabled) {
				TdTagWithoutOpenedTable = true;
				return;
			}
			XamlTablesImportHelper helper = Importer.TablesImportHelper;
			if (helper.Table.LastRow == null) {
				helper.CreateNewRow();
			}
			this.cell = new TableCell(helper.Table.LastRow);
			this.cell.Row.Cells.AddInternal(this.cell);
			ImportMerging(reader);
			this.startParagraphIndex = helper.FindStartParagraphIndexForCell(Importer.Position.ParagraphIndex);
			this.endParagraphIndex = this.startParagraphIndex;
			cell.StartParagraphIndex = this.startParagraphIndex;
		}
		protected internal virtual void ImportMerging(XmlReader reader) {
			string rowSpanString = Importer.ReadAttribute(reader, "RowSpan");
			string columnSpanString = Importer.ReadAttribute(reader, "ColumnSpan");
			RosSpan = Importer.GetIntegerValue(rowSpanString, NumberStyles.Integer, 1);
			if (RosSpan <= 0)
				RosSpan = 1;
			if (RosSpan > 1)
				Cell.VerticalMerging = MergingState.Restart;
			ColSpan = Importer.GetIntegerValue(columnSpanString, NumberStyles.Integer, 1);
			if (ColSpan <= 0)
				ColSpan = 1;
			if (ColSpan > 1)
				Cell.ColumnSpan = ColSpan;
			XamlTablesImportHelper helper = Importer.TablesImportHelper;
			if (helper.Table.Rows.Count == 1)
				helper.AddCellToSpanCollection(RosSpan, ColSpan);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Cell.Properties.BeginInit();
			Cell.Properties.Merge(Properties);
			Cell.Properties.EndInit();
			InitializeCell();
		}
		protected internal virtual void InitializeCell() {
			if (isEmptyCell)
				Importer.PieceTable.InsertParagraphCore(Importer.Position);
			Importer.TablesImportHelper.InitializeTableCell(Cell, StartParagraphIndex, EndParagraphIndex);
			Importer.TablesImportHelper.UpdateFirstRowSpanCollection(RosSpan, ColSpan);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == this.properties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region XamlImportedTableInfo
	public class XamlImportedTableInfo : ImportedTableInfo {
		public XamlImportedTableInfo(Table table)
			: base(table) {
		}
	}
	#endregion
	#region OpenDocumentTablesImportHelper
	public class XamlTablesImportHelper : TablesImportHelper {
		readonly XamlImporter importer;
		public XamlTablesImportHelper(PieceTable pieceTable, XamlImporter importer)
			: base(pieceTable) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		public XamlImporter Importer { get { return importer; } }
		public new XamlImportedTableInfo TableInfo {
			get { return (base.TableInfo == null) ? null : (XamlImportedTableInfo)base.TableInfo; }
		}
		protected override ImportedTableInfo CreateTableInfo(Table newTable) {
			return new XamlImportedTableInfo(newTable);
		}
		protected internal override TableCell CreateCoveredCellWithEmptyParagraph(TableRow lastRow) {
			ParagraphIndex start = Importer.Position.ParagraphIndex;
			ParagraphIndex end = Importer.Position.ParagraphIndex;
			PieceTable.InsertParagraphCore(Importer.Position);
			TableCell cell = new TableCell(lastRow);
			cell.Row.Cells.AddInternal(cell);
			InitializeTableCell(cell, start, end);
			return cell;
		}
	}
	#endregion
	#region TableDisabledDestination
	public class TableDisabledDestination : BlockElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableRowGroup", OnTableRowGroup);
			return result;
		}
		static Destination OnTableRowGroup(XamlImporter importer, XmlReader reader) {
			return new TableRowGroupDisabledDestination(importer);
		}
		public TableDisabledDestination(XamlImporter importer, TableCell parentCell)
			: base(importer) {
		}
		public TableDisabledDestination(XamlImporter importer)
			: this(importer, null) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableRowGroupDisabledDestination
	public class TableRowGroupDisabledDestination : TextElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableRow", OnTableRow);
			return result;
		}
		static Destination OnTableRow(XamlImporter importer, XmlReader reader) {
			return new TableRowDisabledDestination(importer);
		}
		public TableRowGroupDisabledDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableRowDisabledDestination
	public class TableRowDisabledDestination : TextElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("TableCell", OnTableCell);
			return result;
		}
		static Destination OnTableCell(XamlImporter importer, XmlReader reader) {
			return new TableCellDisabledDestination(importer);
		}
		public TableRowDisabledDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region TableCellDisabledDestination
	public class TableCellDisabledDestination : BlockElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = CreateBlockElementHandlerTable();
			return result;
		}
		public TableCellDisabledDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
}
