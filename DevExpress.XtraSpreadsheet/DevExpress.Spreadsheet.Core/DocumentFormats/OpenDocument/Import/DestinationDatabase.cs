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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region DatabaseRangesDestination
	public class DatabaseRangesDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("database-range", OnDatabaseRange);
			return result;
		}
		#endregion
		public DatabaseRangesDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		#region Handlers
		static Destination OnDatabaseRange(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new DatabaseRangeDestination(importer);
		}
		#endregion
	}
	#endregion
	#region DatabaseRangeDestination
	public class DatabaseRangeDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("sort", OnSortState);
			result.Add("filter", OnFilter);
			return result;
		}
		static DatabaseRangeDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (DatabaseRangeDestination)importer.PeekDestination();
		}
		#endregion
		Table table;
		public DatabaseRangeDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string name = reader.GetAttribute("table:name");
			if (string.IsNullOrEmpty(name))
				return;
			CellRange tableRange = Importer.GetCellRangeByCellRangeAddress(reader.GetAttribute("table:target-range-address"));
			bool containsHeader = Importer.GetWpSTOnOffValue(reader, "table:contains-header", true);
			Worksheet sheet = (Worksheet)tableRange.Worksheet;
			table = new Table(sheet, name, tableRange, containsHeader);
			bool suppress = Importer.DocumentModel.SuppressCellValueAssignment;
			Importer.DocumentModel.SuppressCellValueAssignment = false;
			GenerateColumns();
			Importer.DocumentModel.SuppressCellValueAssignment = suppress;
			table.Worksheet.Tables.Add(table);
		}
		void GenerateColumns() {
			TableColumnsInitialCreationCommand command = new TableColumnsInitialCreationCommand(table);
			command.Execute();
		}
		#region Handlers
		static Destination OnSortState(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new SortStateDestination(table, importer);
		}
		static Destination OnFilter(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterDestination(table, importer);
		}
		#endregion
	}
	#endregion
	#region FilterDestination
	public class FilterDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("filter-and", OnFilterAnd);
			result.Add("filter-condition", OnFilterCondition);
			result.Add("filter-or", OnFilterOr);
			return result;
		}
		static FilterDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (FilterDestination)importer.PeekDestination();
		}
		#endregion
		Table table;
		public FilterDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		#region Handlers
		static Destination OnFilterAnd(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterAndDestination(table, importer);
		}
		static Destination OnFilterCondition(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterConditionDestination(table, importer);
		}
		static Destination OnFilterOr(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterOrDestination(table, importer);
		}
		#endregion
	}
	#endregion
	#region FilterAndDestination
	public class FilterAndDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("filter-condition", OnFilterCondition);
			result.Add("filter-or", OnFilterOr);
			return result;
		}
		static FilterAndDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (FilterAndDestination)importer.PeekDestination();
		}
		#endregion
		Table table;
		public FilterAndDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		#region Handlers
		static Destination OnFilterCondition(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterConditionDestination(table, importer);
		}
		static Destination OnFilterOr(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterOrDestination(table, importer);
		}
		#endregion
	}
	#endregion
	#region FilterOrDestination
	public class FilterOrDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("filter-and", OnFilterAnd);
			result.Add("filter-condition", OnFilterCondition);
			return result;
		}
		static FilterOrDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (FilterOrDestination)importer.PeekDestination();
		}
		#endregion
		Table table;
		public FilterOrDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		#region Handlers
		static Destination OnFilterAnd(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterAndDestination(table, importer);
		}
		static Destination OnFilterCondition(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new FilterConditionDestination(table, importer);
		}
		#endregion
	}
	#endregion
	#region FilterConditionDestination
	public class FilterConditionDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		#endregion
		Table table;
		public FilterConditionDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		#region Handlers
		#endregion
	}
	#endregion
	#region SortStateDestination
	public class SortStateDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("sort-by", OnSortBy);
			return result;
		}
		static SortStateDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (SortStateDestination)importer.PeekDestination();
		}
		#endregion
		Table table;
		public SortStateDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			table.AutoFilter.SortState.SetSortRangeCore(table.GetDataRange());
			table.AutoFilter.SortState.CaseSensitive = Importer.GetBoolean(reader, "table:case-sensitive", false);
		}
		#region Handlers
		static Destination OnSortBy(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new SortByDestination(table, importer);
		}
		#endregion
	}
	#endregion
	#region SortByDestination
	public class SortByDestination : LeafElementDestination {
		Table table;
		public SortByDestination(Table table, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.table = table;
		}
		public Table Table { get { return table; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int sortedColumn = Importer.GetWpSTIntegerValue(reader, "table:field-number", 0);
			CellRange sortReference = table.AutoFilter.SortState.SortRange.GetSubColumnRange(sortedColumn, sortedColumn);
			SortCondition sortCondition = new SortCondition(table.Worksheet, sortReference);
			sortCondition.Descending = Importer.GetAttribute(reader, "table:order", "ascending").Equals("descending");
			table.AutoFilter.SortState.SortConditions.AddCore(sortCondition);
		}
	}
	#endregion
}
#endif
