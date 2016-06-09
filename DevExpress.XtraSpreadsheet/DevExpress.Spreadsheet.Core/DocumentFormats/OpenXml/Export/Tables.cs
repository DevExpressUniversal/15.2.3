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
using DevExpress.Office;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static readonly Dictionary<TableType, string> TableTypeTable = CreateTableTypeTable();
		static Dictionary<TableType, string> CreateTableTypeTable() {
			Dictionary<TableType, string> result = new Dictionary<TableType, string>();
			result.Add(TableType.Worksheet, "worksheet");
			result.Add(TableType.Xml, "xml");
			result.Add(TableType.QueryTable, "queryTable");
			return result;
		}
		#endregion
		#region Tables
		protected internal virtual void AddTablesPackageContent() {
			if (!HasTables())
				return;
			this.tableCounter = 1;
			foreach (Worksheet sheet in Workbook.Sheets) {
				sheet.Tables.ForEach(AddTablePackageContent);
			}
		}
		protected internal virtual bool HasTables() {
			foreach (Worksheet sheet in Workbook.Sheets) {
				if (sheet.Tables.Count != 0)
					return true;
			}
			return false;
		}
		protected internal virtual void AddTablePackageContent(Table table) {
			this.activeTable = table;
			string path = TablePathsTable[table.Name];
			AddPackageContent(path, ExportTableContent());
			PopulateQueryTablesTable(table);
		}
		protected internal virtual CompressedStream ExportTableContent() {
			return CreateXmlContent(GenerateTableXmlContent);
		}
		protected internal virtual void GenerateTableXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateTableContent();
		}
		protected internal virtual void GenerateTableContent() {
			WriteShStartElement("table");
			try {
				GenerateTableAttributes(ActiveTable);
				GenerateTableAutoFilterContent(ActiveTable.AutoFilter);
				GenerateSortStateContent(ActiveTable.AutoFilter.SortState);
				GenerateTableColumnsContent(ActiveTable);
				GenerateTableStyleInfoContent(ActiveTable);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateTableAttributes(Table table) {
			WriteIntValue("id", this.tableCounter++);
			if (String.IsNullOrEmpty(table.OriginalName))
				WriteStringValue("name", table.Name);
			else
				WriteStringValue("name", table.OriginalName);
			WriteStringValue("displayName", table.Name);
			WriteStringValue("ref", table.Range.ToString());
			if (!String.IsNullOrEmpty(table.Comment))
				WriteStringValue("comment", table.Comment);
			TableInfo defaultItem = Workbook.Cache.TableInfoCache.DefaultItem;
			if (table.ConnectionId != defaultItem.ConnectionId)
				WriteIntValue("connectionId", table.ConnectionId);
			if (table.HasHeadersRow != defaultItem.HasHeadersRow)
				WriteIntValue("headerRowCount", table.HasHeadersRow ? 1 : 0);
			if (table.InsertRowProperty != defaultItem.InsertRowProperty)
				WriteBoolValue("insertRow", table.InsertRowProperty);
			if (table.InsertRowShift != defaultItem.InsertRowShift)
				WriteBoolValue("insertRowShift", table.InsertRowShift);
			if (table.Published != defaultItem.Published)
				WriteBoolValue("published", table.Published);
			 if (table.TableType != defaultItem.TableType)
				WriteStringValue("tableType", TableTypeTable[table.TableType]);
			if (table.HasTotalsRow != defaultItem.HasTotalsRow)
				WriteIntValue("totalsRowCount", table.HasTotalsRow ? 1 : 0);
			int headerRowIndex = Table.HeaderRowIndex;
			int dataIndex = Table.DataIndex;
			int totalsRowIndex = Table.TotalsRowIndex;
			int[] differentialFormatIndexes = table.DifferentialFormatIndexes;
			WriteDifferentialFormatId("headerRowDxfId", differentialFormatIndexes[headerRowIndex]);
			WriteDifferentialFormatId("dataDxfId", differentialFormatIndexes[dataIndex]);
			WriteDifferentialFormatId("totalsRowDxfId", differentialFormatIndexes[totalsRowIndex]);
			int[] borderFormatIndexes = table.BorderFormatIndexes;
			WriteDifferentialFormatId("headerRowBorderDxfId", borderFormatIndexes[headerRowIndex]);
			WriteDifferentialFormatId("tableBorderDxfId", borderFormatIndexes[dataIndex]);
			WriteDifferentialFormatId("totalsRowBorderDxfId", borderFormatIndexes[totalsRowIndex]);
			int[] cellFormatIndexes = table.CellFormatIndexes;
			if (table.GetApplyCellStyle(headerRowIndex))
				WriteCellStyleName("headerRowCellStyle", cellFormatIndexes[headerRowIndex]);
			if (table.GetApplyCellStyle(dataIndex))
				WriteCellStyleName("dataCellStyle", cellFormatIndexes[dataIndex]);
			if (table.GetApplyCellStyle(totalsRowIndex))
				WriteCellStyleName("totalsRowCellStyle", cellFormatIndexes[totalsRowIndex]);
		}
		void WriteCellStyleName(string attributeName, int cellFormatIndex) {
			CellFormat cellFormat = (CellFormat)Workbook.Cache.CellFormatCache[cellFormatIndex];
			WriteStringValue(attributeName, cellFormat.Style.Name);
		}
		protected internal virtual void GenerateTableColumnsContent(Table table) {
			WriteShStartElement("tableColumns");
			try {
				int columnCount = table.Columns.Count;
				WriteIntValue("count", columnCount);
				TableType tableType = table.TableType;
				for (int i = 0; i < columnCount; i++) {
					GenerateTableColumnContent(table.Columns[i], i + 1, tableType);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateTableColumnContent(TableColumn columnInfo, int index, TableType tableType) {
			WriteShStartElement("tableColumn");
			try {
				int id = tableType == TableType.Worksheet ? index : columnInfo.Id;
				WriteIntValue("id", id);
				WriteStringValue("name", EncodeXmlChars(columnInfo.Name));
				if (columnInfo.QueryTableFieldId != TableColumn.DefaultQueryTableFieldId)
					WriteIntValue("queryTableFieldId", columnInfo.QueryTableFieldId);
				if (columnInfo.TotalsRowFunction != TotalsRowFunctionType.None)
					WriteStringValue("totalsRowFunction", columnInfo.TotalsRowFunctionText);
				if (!String.IsNullOrEmpty(columnInfo.TotalsRowLabel))
					WriteStringValue("totalsRowLabel", EncodeXmlChars(columnInfo.TotalsRowLabel));
				if (!String.IsNullOrEmpty(columnInfo.UniqueName) && (tableType == TableType.QueryTable || tableType == TableType.Xml))
					WriteStringValue("uniqueName", EncodeXmlChars(columnInfo.UniqueName));
				int headerRowIndex = TableColumn.HeaderRowIndex;
				int dataIndex = TableColumn.DataIndex;
				int totalsRowIndex = TableColumn.TotalsRowIndex;
				WriteDifferentialFormatId("headerRowDxfId", columnInfo.DifferentialFormatIndexes[headerRowIndex]);
				WriteDifferentialFormatId("dataDxfId", columnInfo.DifferentialFormatIndexes[dataIndex]);
				WriteDifferentialFormatId("totalsRowDxfId", columnInfo.DifferentialFormatIndexes[totalsRowIndex]);
				if (columnInfo.GetApplyCellStyle(headerRowIndex))
					WriteCellStyleName("headerRowCellStyle", columnInfo.CellFormatIndexes[headerRowIndex]);
				if (columnInfo.GetApplyCellStyle(dataIndex))
					WriteCellStyleName("dataCellStyle", columnInfo.CellFormatIndexes[dataIndex]);
				if (columnInfo.GetApplyCellStyle(totalsRowIndex))
					WriteCellStyleName("totalsRowCellStyle", columnInfo.CellFormatIndexes[totalsRowIndex]);
				if(tableType == TableType.Worksheet)
					GenerateCalculatedColumnFormulaContent(columnInfo);
				GenerateTotalsRowFormulaContent(columnInfo);
				GenerateXMLColumnProperties(columnInfo.XmlProperties);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCalculatedColumnFormulaContent(TableColumn columnInfo) {
			if (!columnInfo.HasColumnFormula)
				return;
			string calculatedColumnFormula = columnInfo.ColumnFormulaText;
			if (String.IsNullOrEmpty(calculatedColumnFormula))
				return;
			WriteShStartElement("calculatedColumnFormula");
			try {
				if (columnInfo.ColumnFormulaIsArray)
					WriteShBoolValue("array", columnInfo.ColumnFormulaIsArray);
				WriteShString(calculatedColumnFormula);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateTotalsRowFormulaContent(TableColumn columnInfo) {
			if(!columnInfo.HasTotalsRowFormula)
				return;
			string totalsRowFormula = columnInfo.TotalsRowFormulaText;
			if (String.IsNullOrEmpty(totalsRowFormula))
				return;
			WriteShStartElement("totalsRowFormula");
			try {
				if (columnInfo.TotalsRowFormulaIsArray)
					WriteBoolValue("array", columnInfo.TotalsRowFormulaIsArray);
				WriteShString(totalsRowFormula);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateXMLColumnProperties(XmlColumnProperties xmlProperties) {
			if (!ShouldExportXMLColumnProperties(xmlProperties))
				return;
			WriteShStartElement("xmlColumnPr");
			try {
				if (xmlProperties.Denormalized != false)
					WriteBoolValue("denormalized", xmlProperties.Denormalized);
				if (xmlProperties.MapId != XmlColumnProperties.DefaultMapId)
					WriteIntValue("mapId", xmlProperties.MapId);
				if (!String.IsNullOrEmpty(xmlProperties.XmlDataType))
					WriteStringValue("xmlDataType", xmlProperties.XmlDataType);
				if (!String.IsNullOrEmpty(xmlProperties.Xpath))
					WriteStringValue("xpath", xmlProperties.Xpath);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportXMLColumnProperties(XmlColumnProperties xmlProperties) {
			if (xmlProperties == null)
				return false;
			return xmlProperties.Denormalized != false || xmlProperties.MapId != XmlColumnProperties.DefaultMapId ||
				!String.IsNullOrEmpty(xmlProperties.XmlDataType) || !String.IsNullOrEmpty(xmlProperties.Xpath);
		}
		protected internal virtual void GenerateTableStyleInfoContent(Table table) {
			WriteShStartElement("tableStyleInfo");
			try {
				if (!TableStyleName.CheckDefaultStyleName(table.TableInfo.StyleName))
					WriteStringValue("name", table.TableInfo.StyleName);
					WriteBoolValue("showColumnStripes", table.ShowColumnStripes);
					WriteBoolValue("showFirstColumn", table.ShowFirstColumn);
					WriteBoolValue("showLastColumn", table.ShowLastColumn);
					WriteBoolValue("showRowStripes", table.ShowRowStripes);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateTablePartsContent() {
			int tablesCount = ActiveSheet.Tables.Count;
			if (tablesCount == 0)
				return;
			WriteShStartElement("tableParts");
			try {
				WriteShIntValue("count", tablesCount);
				ActiveSheet.Tables.ForEach(GenerateTablePartContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateTablePartContent(Table table) {
			WriteShStartElement("tablePart");
			try {
				string id = PopulateTablesTable(table);
				WriteStringAttr(RelsPrefix, "id", null, id);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual string PopulateTablesTable(Table table) {
			this.tableCounter++;
			string fileName = String.Format("table{0}.xml", this.tableCounter);
			string tableRelationTarget = "../tables/" + fileName;
			OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
			string id = GenerateIdByCollection(relations);
			OpenXmlRelation relation = new OpenXmlRelation(id, tableRelationTarget, RelsTableNamespace);
			relations.Add(relation);
			string tablePath = @"xl\tables\" + fileName;
			TablePathsTable.Add(table.Name, tablePath);
			Builder.OverriddenContentTypes.Add("/xl/tables/" + fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.table+xml");
			if (NeedTableRelations(table)) {
				TableRelationPathTable.Add(table.Name, String.Format(@"xl\tables\_rels\table{0}.xml.rels", tableCounter));
				currentRelations = new OpenXmlRelationCollection();
				TableRelationsTable.Add(table.Name, currentRelations);
			}
			return id;
		}
		protected internal virtual bool NeedTableRelations(Table table) {
			return table.TableType == TableType.QueryTable && table.QueryTableContentId >= 0 && table.QueryTableContentId < Workbook.QueryTablesContent.Count;
		}
		protected internal virtual void AddTableRelationsPackageContent() {
			foreach (KeyValuePair<string, string> valuePair in TableRelationPathTable) {
				currentTableRelations = TableRelationsTable[valuePair.Key];
				AddPackageContent(valuePair.Value, ExportTableRelations());
			}
		}
		protected internal virtual CompressedStream ExportTableRelations() {
			return CreateXmlContent(GenerateTableRelationsContent);
		}
		protected internal virtual void GenerateTableRelationsContent(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, currentTableRelations);
		}
		#endregion
	}
}
