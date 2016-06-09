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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region TableReferenceDestination
	public class TableReferenceDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public TableReferenceDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		internal virtual new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			string relationPath = Importer.LookupRelationTargetById(Importer.DocumentRelations, relationId, Importer.DocumentRootFolder, string.Empty);
			Importer.TableRelations.Add(relationPath, Importer.CurrentWorksheet);
		}
	}
	#endregion
	#region TableReferencesDestination
	public class TableReferencesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tablePart", OnTablePart);
			return result;
		}
		#endregion
		public TableReferencesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTablePart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableReferenceDestination(importer);
		}
		#endregion
	}
	#endregion
	#region TableDestination
	public class TableDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tableColumns", OnTableColumns);
			result.Add("tableStyleInfo", OnTableStyleInfo);
			result.Add("autoFilter", OnAutoFilter);
			result.Add("sortState", OnSortState);
			return result;
		}
		static TableDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TableDestination)importer.PeekDestination();
		}
		static Dictionary<TableType, string> tableTypeTable = OpenXmlExporter.TableTypeTable;
		#endregion
		#region Fields
		readonly Worksheet sheet;
		Table table;
		int[] differentialFormatIds;
		int[] borderFormatIds;
		string[] cellStyleNames;
		#endregion
		public TableDestination(SpreadsheetMLBaseImporter importer, Worksheet sheet)
			: base(importer) {
			this.sheet = sheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public Table Table { get { return table; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = Importer.ReadAttribute(reader, "name");
			string displayName = Importer.ReadAttribute(reader, "displayName");
			if (string.IsNullOrEmpty(displayName))
				Importer.ThrowInvalidFile("Table display name is null or empty");
			if (string.IsNullOrEmpty(name))
				name = displayName;
			CellRange range = Importer.ReadCellRange(reader, "ref", sheet);
			if (range == null)
				Importer.ThrowInvalidFile("Table ref is not specified");
			table = CreateTable(name, displayName, range);
			table.BeginUpdate();
			string comment = Importer.ReadAttribute(reader, "comment");
			if (!String.IsNullOrEmpty(comment))
				table.Comment = comment;
			TableInfo defaultItem = Importer.DocumentModel.Cache.TableInfoCache.DefaultItem;
			int defaultTotalsRowCount = defaultItem.HasTotalsRow ? 1 : 0;
			table.SetHasTotalsRow(Importer.GetIntegerValue(reader, "totalsRowCount", defaultTotalsRowCount) > 0);
			int defaultHeaderRowCount = defaultItem.HasHeadersRow ? 1 : 0;
			table.SetHasHeadersRow(Importer.GetIntegerValue(reader, "headerRowCount", defaultHeaderRowCount) > 0);
			table.InsertRowProperty = Importer.GetWpSTOnOffValue(reader, "insertRow", defaultItem.InsertRowProperty);
			table.Published = Importer.GetWpSTOnOffValue(reader, "published", defaultItem.Published);
			table.InsertRowShift = Importer.GetWpSTOnOffValue(reader, "insertRowShift", defaultItem.InsertRowShift);
			table.ConnectionId = Importer.GetIntegerValue(reader, "connectionId", defaultItem.ConnectionId);
			table.TableType = Importer.GetWpEnumValue<TableType>(reader, "tableType", tableTypeTable, defaultItem.TableType);
			ReadFormatIds(reader);
			ReadCellStyleName(reader);
			sheet.Tables.AddWithoutHistoryAndNotifications(table);
			if (table.TableType == TableType.QueryTable) {
				string relationPath = Importer.LookupRelationTargetByType(Importer.DocumentRelations, Importer.RelsQueryTablesNamespace, Importer.DocumentRootFolder, string.Empty);
				if (!string.IsNullOrEmpty(relationPath))
					Importer.QueryTableRelations.Add(relationPath, table);
			}
			table.AutoFilter.SetRangeCore(null);
		}
		void ReadFormatIds(XmlReader reader) {
			differentialFormatIds = new int[Table.DifferentialFormatElementCount];
			ReadDifferentialFormatId(reader, "headerRowDxfId", Table.HeaderRowIndex, differentialFormatIds);
			ReadDifferentialFormatId(reader, "dataDxfId", Table.DataIndex, differentialFormatIds);
			ReadDifferentialFormatId(reader, "totalsRowDxfId", Table.TotalsRowIndex, differentialFormatIds);
			borderFormatIds = new int[Table.BorderFormatElementCount];
			ReadDifferentialFormatId(reader, "headerRowBorderDxfId", Table.HeaderRowIndex, borderFormatIds);
			ReadDifferentialFormatId(reader, "tableBorderDxfId", Table.DataIndex, borderFormatIds);
			ReadDifferentialFormatId(reader, "totalsRowBorderDxfId", Table.TotalsRowIndex, borderFormatIds);
		}
		void ReadCellStyleName(XmlReader reader) {
			cellStyleNames = new string[Table.CellFormatElementCount];
			cellStyleNames[Table.HeaderRowIndex] = Importer.ReadAttribute(reader, "headerRowCellStyle");
			cellStyleNames[Table.DataIndex] = Importer.ReadAttribute(reader, "dataCellStyle");
			cellStyleNames[Table.TotalsRowIndex] = Importer.ReadAttribute(reader, "totalsRowCellStyle");
		}
		void ReadDifferentialFormatId(XmlReader reader, string attributeName, int elementIndex, int[] collection) {
			int id = Importer.GetIntegerValue(reader, attributeName, Int32.MinValue);
			collection[elementIndex] = Importer.StyleSheet.GetDifferentialFormatIndex(id);
		}
		public override void ProcessElementClose(XmlReader reader) {
			table.EndUpdate();
			AssignDifferentialFormatIndexes();
			AssignCellFormatIndexes();
		}
		void AssignDifferentialFormatIndexes() {
			table.AssignDifferentialFormatIndex(Table.HeaderRowIndex, differentialFormatIds[Table.HeaderRowIndex]);
			table.AssignDifferentialFormatIndex(Table.DataIndex, differentialFormatIds[Table.DataIndex]);
			table.AssignDifferentialFormatIndex(Table.TotalsRowIndex, differentialFormatIds[Table.TotalsRowIndex]);
			table.AssignBorderFormatIndex(Table.HeaderRowIndex, borderFormatIds[Table.HeaderRowIndex]);
			table.AssignBorderFormatIndex(Table.DataIndex, borderFormatIds[Table.DataIndex]);
			table.AssignBorderFormatIndex(Table.TotalsRowIndex, borderFormatIds[Table.TotalsRowIndex]);
		}
		void AssignCellFormatIndexes() {
			AssignCellStyle(Table.HeaderRowIndex);
			AssignCellStyle(Table.DataIndex);
			AssignCellStyle(Table.TotalsRowIndex);
		}
		void AssignCellStyle(int elementIndex) {
			int formatIndex = GetCellFormatIndex(elementIndex, cellStyleNames[elementIndex]);
			table.AssignCellFormatIndex(elementIndex, formatIndex);
		}
		int GetCellFormatIndex(int elementIndex, string cellStyleName) {
			if (String.IsNullOrEmpty(cellStyleName))
				return table.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			CellStyleBase cellStyle = table.DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(cellStyleName);
			if (cellStyle == null)
				return table.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			AssignApplyCellStyle(elementIndex);
			return Importer.StyleSheet.GetCellFormatIndex(elementIndex, cellStyle);
		}
		void AssignApplyCellStyle(int elementIndex) {
			TableCellStyleApplyFlagsInfo options = table.ApplyFlagsInfo.Clone();
			options.SetApplyCellStyle(elementIndex, true);
			table.AssignApplyFlagsIndex(options.PackedValues);
		}
		protected internal virtual Table CreateTable(string name, string displayName, CellRange range) {
			Table table = new Table(sheet, name, displayName, range);
			return table;
		}
		static Destination OnTableColumns(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableColumnsDestination(importer, GetThis(importer).Table);
		}
		static Destination OnTableStyleInfo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableStyleInfoDestination(importer, GetThis(importer).Table.TableInfo);
		}
		static Destination OnAutoFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new AutoFilterDestination(importer, table.AutoFilter, table.Worksheet);
		}
		static Destination OnSortState(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Table table = GetThis(importer).Table;
			return new SortStateDestination(importer, table.AutoFilter.SortState, table.Worksheet);
		}
	}
	#endregion
	#region TableColumnsDestination
	public class TableColumnsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tableColumn", OnTableColumn);
			return result;
		}
		static TableColumnsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TableColumnsDestination)importer.PeekDestination();
		}
		#endregion
		readonly Table table;
		public TableColumnsDestination(SpreadsheetMLBaseImporter importer, Table table)
			: base(importer) {
			this.table = table;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Table Table { get { return table; } }
		#endregion
		#region Handlers
		static Destination OnTableColumn(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableColumnDestination(importer, GetThis(importer).Table);
		}
		#endregion
	}
	#endregion
	#region TableColumnDestination
	public class TableColumnDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("calculatedColumnFormula", OnCalculatedColumnFormula);
			result.Add("totalsRowFormula", OnTotalsRowFormula);
			result.Add("xmlColumnPr", OnXmlColumnProperties);
			return result;
		}
		static TableColumnDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TableColumnDestination)importer.PeekDestination();
		}
		#endregion
		#region RowFunctions table
		static Dictionary<string, TotalsRowFunctionType> rowFunctions = CreateRowFunctionsCollection();
		static Dictionary<string, TotalsRowFunctionType> CreateRowFunctionsCollection() {
			Dictionary<string, TotalsRowFunctionType> result = new Dictionary<string, TotalsRowFunctionType>();
			result.Add("none", TotalsRowFunctionType.None);
			result.Add("sum", TotalsRowFunctionType.Sum);
			result.Add("min", TotalsRowFunctionType.Min);
			result.Add("max", TotalsRowFunctionType.Max);
			result.Add("average", TotalsRowFunctionType.Average);
			result.Add("count", TotalsRowFunctionType.Count);
			result.Add("countNums", TotalsRowFunctionType.CountNums);
			result.Add("stdDev", TotalsRowFunctionType.StdDev);
			result.Add("var", TotalsRowFunctionType.Var);
			result.Add("custom", TotalsRowFunctionType.Custom);
			return result;
		}
		#endregion
		TableColumn column;
		readonly Table table;
		public TableColumnDestination(SpreadsheetMLBaseImporter importer, Table table)
			: base(importer) {
			this.table = table;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public TableColumn Column { get { return column; } }
		public Table Table { get { return table; } }
		#endregion
		#region Handlers
		static Destination OnCalculatedColumnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableCalculatedColumnFormulaDestination(importer, GetThis(importer).Column);
		}
		static Destination OnTotalsRowFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableColumnTotalsRowFormulaDestination(importer, GetThis(importer).Column);
		}
		static Destination OnXmlColumnProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableColumnXmlProperties(importer, GetThis(importer).Column);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadAttribute(reader, "name");
			if (String.IsNullOrEmpty(name))
				Importer.ThrowInvalidFile("Table column name is null or empty");
			name = Importer.DecodeXmlChars(name);
			column = new TableColumn(Table, name);
			table.Columns.Add(column);
			string totalsRowFormula = Importer.ReadAttribute(reader, "totalsRowFunction");
			if (!String.IsNullOrEmpty(totalsRowFormula))
				SetTotalsRowFormula(reader, column, totalsRowFormula);
			string totalsRowLabel = Importer.ReadAttribute(reader, "totalsRowLabel");
			if (!String.IsNullOrEmpty(totalsRowLabel))
				column.SetTotalsRowLabelCore(totalsRowLabel);
			column.UniqueName = Importer.ReadAttribute(reader, "uniqueName");
			column.QueryTableFieldId = Importer.GetIntegerValue(reader, "queryTableFieldId", TableColumn.DefaultQueryTableFieldId);
			column.Id = Importer.GetIntegerValue(reader, "id", -1);
			AssignDifferentialFormatIndexes(reader, column);
			AssignCellFormatIndexes(reader, column);
		}
		void SetTotalsRowFormula(XmlReader reader, TableColumn column, string totalsRowFormula) {
			TotalsRowFunctionType totalsRowFunctionType;
			if (!rowFunctions.TryGetValue(totalsRowFormula, out totalsRowFunctionType))
				return;
			column.SetTotalsRowFunctionCore(totalsRowFunctionType);
		}
		void AssignDifferentialFormatIndexes(XmlReader reader, TableColumn column) {
			AssignDifferentialFormatIndex(reader, "headerRowDxfId", column, TableColumn.HeaderRowIndex);
			AssignDifferentialFormatIndex(reader, "dataDxfId", column, TableColumn.DataIndex);
			AssignDifferentialFormatIndex(reader, "totalsRowDxfId", column, TableColumn.TotalsRowIndex);
		}
		void AssignDifferentialFormatIndex(XmlReader reader, string attributeName, TableColumn column, int elementIndex) {
			int id = Importer.GetIntegerValue(reader, attributeName, Int32.MinValue);
			int formatIndex = Importer.StyleSheet.GetDifferentialFormatIndex(id);
			column.AssignDifferentialFormatIndex(elementIndex, formatIndex);
		}
		void AssignCellFormatIndexes(XmlReader reader, TableColumn column) {
			int formatIndex = GetCellFormatIndex(reader, "headerRowCellStyle", TableColumn.HeaderRowIndex);
			column.AssignCellFormatIndex(TableColumn.HeaderRowIndex, formatIndex);
			formatIndex = GetCellFormatIndex(reader, "dataCellStyle", TableColumn.DataIndex);
			column.AssignCellFormatIndex(TableColumn.DataIndex, formatIndex);
			formatIndex = GetCellFormatIndex(reader, "totalsRowCellStyle", TableColumn.TotalsRowIndex);
			column.AssignCellFormatIndex(TableColumn.TotalsRowIndex, formatIndex);
		}
		int GetCellFormatIndex(XmlReader reader, string attributeName, int elementIndex) {
			string cellStyleName = Importer.ReadAttribute(reader, attributeName);
			return GetCellStyleIndex(elementIndex, cellStyleName);
		}
		int GetCellStyleIndex(int elementIndex, string cellStyleName) {
			if (String.IsNullOrEmpty(cellStyleName))
				return column.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			CellStyleBase cellStyle = column.DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(cellStyleName);
			if (cellStyle == null)
				return column.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			AssignApplyCellStyle(elementIndex);
			return Importer.StyleSheet.GetCellFormatIndex(elementIndex, cellStyle);
		}
		void AssignApplyCellStyle(int elementIndex) {
			TableCellStyleApplyFlagsInfo options = column.ApplyFlagsInfo.Clone();
			options.SetApplyCellStyle(elementIndex, true);
			column.AssignApplyFlagsIndex(options.PackedValues);
		}
	}
	#endregion
	#region TableColumnFormulaDestinationBase (abstract class)
	public abstract class TableColumnFormulaDestinationBase : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly TableColumn column;
		string formulaText;
		bool isArrayFormula;
		#endregion
		protected TableColumnFormulaDestinationBase(SpreadsheetMLBaseImporter importer, TableColumn column)
			: base(importer) {
			Guard.ArgumentNotNull(column, "column");
			this.column = column;
		}
		#region Properties
		public TableColumn Column { get { return column; } }
		#endregion
		public override bool ProcessText(XmlReader reader) {
			formulaText = reader.Value;
			if (!String.IsNullOrEmpty(formulaText))
				formulaText = String.Concat("=", formulaText);
			return true;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			isArrayFormula = Importer.GetWpSTOnOffValue(reader, "array", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!string.IsNullOrEmpty(formulaText))
				SetFormulaText(formulaText, isArrayFormula);
			base.ProcessElementClose(reader);
		}
		public abstract void SetFormulaText(string formulaText, bool isArrayFormula);
	}
	#endregion
	#region TableColumnTotalsRowFormulaDestination
	public class TableColumnTotalsRowFormulaDestination : TableColumnFormulaDestinationBase {
		public TableColumnTotalsRowFormulaDestination(SpreadsheetMLBaseImporter importer, TableColumn column)
			: base(importer, column) {
		}
		public override void SetFormulaText(string formulaText, bool isArrayFormula) {
			Column.SetTotalsRowFormulaTemporarily(formulaText, isArrayFormula);
		}
	}
	#endregion
	#region TableCalculatedColumnFormulaDestination
	public class TableCalculatedColumnFormulaDestination : TableColumnFormulaDestinationBase {
		public TableCalculatedColumnFormulaDestination(SpreadsheetMLBaseImporter importer, TableColumn column)
			: base(importer, column) {
		}
		public override void SetFormulaText(string formulaText, bool isArrayFormula) {
			Column.SetColumnFormulaTemporarily(formulaText, isArrayFormula);
		}
	}
	#endregion
	#region TableStyleInfoDestination
	public class TableStyleInfoDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly TableInfo tableInfo;
		public TableStyleInfoDestination(SpreadsheetMLBaseImporter importer, TableInfo tableInfo)
			: base(importer) {
			Guard.ArgumentNotNull(tableInfo, "tableInfo");
			this.tableInfo = tableInfo;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.StyleSheet.SetTableStyleName(tableInfo, Importer.ReadAttribute(reader, "name"));
			tableInfo.ShowColumnStripes = Importer.GetWpSTOnOffValue(reader, "showColumnStripes", false);
			tableInfo.ShowRowStripes = Importer.GetWpSTOnOffValue(reader, "showRowStripes", false);
			tableInfo.ShowFirstColumn = Importer.GetWpSTOnOffValue(reader, "showFirstColumn", false);
			tableInfo.ShowLastColumn = Importer.GetWpSTOnOffValue(reader, "showLastColumn", false);
		}
	}
	#endregion
	#region TableColumnXmlProperties
	public class TableColumnXmlProperties : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly TableColumn column;
		public TableColumnXmlProperties(SpreadsheetMLBaseImporter importer, TableColumn column)
			: base(importer) {
			Guard.ArgumentNotNull(column, "column");
			this.column = column;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			XmlColumnProperties xmlProperties = new XmlColumnProperties();
			xmlProperties.Denormalized = Importer.GetWpSTOnOffValue(reader, "denormalized", false);
			xmlProperties.MapId = Importer.GetIntegerValue(reader, "mapId", XmlColumnProperties.DefaultMapId);
			xmlProperties.Xpath = Importer.ReadAttribute(reader, "xpath");
			xmlProperties.XmlDataType = Importer.ReadAttribute(reader, "xmlDataType");
			column.XmlProperties = xmlProperties;
		}
	}
	#endregion
}
