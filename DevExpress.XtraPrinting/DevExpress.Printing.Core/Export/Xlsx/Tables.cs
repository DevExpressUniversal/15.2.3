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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		const string relsTableType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table";
		const string tableContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.table+xml";
		int tableId;
		readonly HashSet<string> uniqueTableNames = new HashSet<string>();
		#region Statics
		static readonly Dictionary<XlTotalRowFunction, string> totalRowFunctionTable = CreateTotalRowFunctionTable();
		static Dictionary<XlTotalRowFunction, string> CreateTotalRowFunctionTable() {
			Dictionary<XlTotalRowFunction, string> result = new Dictionary<XlTotalRowFunction, string>();
			result.Add(XlTotalRowFunction.None, "none");
			result.Add(XlTotalRowFunction.Average, "average");
			result.Add(XlTotalRowFunction.Count, "count");
			result.Add(XlTotalRowFunction.CountNums, "countNums");
			result.Add(XlTotalRowFunction.Max, "max");
			result.Add(XlTotalRowFunction.Min, "min");
			result.Add(XlTotalRowFunction.StdDev, "stdDev");
			result.Add(XlTotalRowFunction.Sum, "sum");
			result.Add(XlTotalRowFunction.Var, "var");
			return result;
		}
		#endregion
		void GenerateTableParts() {
			IList<XlTable> tables = currentSheet.Tables;
			if(tables.Count == 0)
				return;
			ValidateTables();
			WriteShStartElement("tableParts");
			try {
				WriteIntValue("count", tables.Count);
				foreach(XlTable table in tables)
					WriteTablePart(table);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ValidateTables() {
			IList<XlTable> tables = currentSheet.Tables;
			for(int i = 0; i < tables.Count; i++) {
				XlTable table = tables[i];
				tableId++;
				table.Id = tableId;
			}
		}
		void WriteTablePart(XlTable table) {
			string rId = this.sheetRelations.GenerateId();
			string target = string.Format(@"../tables/table{0}.xml", table.Id);
			OpenXmlRelation relation = new OpenXmlRelation(rId, target, relsTableType);
			this.sheetRelations.Add(relation);
			WriteShStartElement("tablePart");
			try {
				WriteStringAttr(XlsxPackageBuilder.RelsPrefix, "id", null, rId);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateTablesContent() {
			foreach(XlTable table in currentSheet.Tables)
				GenerateTableContent(table);
		}
		void GenerateTableContent(XlTable table) {
			BeginWriteXmlContent();
			GenerateTableContentCore(table);
			AddPackageContent(String.Format(@"xl\tables\table{0}.xml", table.Id), EndWriteXmlContent());
			Builder.OverriddenContentTypes.Add(String.Format(@"/xl/tables/table{0}.xml", table.Id), tableContentType);
		}
		void GenerateTableContentCore(XlTable table) {
			WriteShStartElement("table");
			try {
				GenerateTableAttributes(table);
				GenerateTableAutoFilterContent(table);
				GenerateTableColumnsContent(table);
				GenerateTableStyleInfoContent(table);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateTableAttributes(XlTable table) {
			WriteIntValue("id", table.Id);
			WriteStringValue("name", table.Name);
			WriteStringValue("displayName", table.Name);
			WriteStringValue("ref", table.Range.ToString());
			if(!String.IsNullOrEmpty(table.Comment))
				WriteStringValue("comment", table.Comment);
			if(!table.HasHeaderRow)
				WriteIntValue("headerRowCount", 0);
			if(table.InsertRowShowing)
				WriteBoolValue("insertRow", table.InsertRowShowing);
			if(table.InsertRowShift)
				WriteBoolValue("insertRowShift", table.InsertRowShift);
			if(table.Published)
				WriteBoolValue("published", table.Published);
			if(table.HasTotalRow)
				WriteIntValue("totalsRowCount", 1);
			WriteDxfId("headerRowDxfId", table.HeaderRowFormatting);
			WriteDxfId("dataDxfId", table.DataFormatting);
			WriteDxfId("totalsRowDxfId", table.TotalRowFormatting);
			WriteDxfId("headerRowBorderDxfId", table.HeaderRowBorderFormatting);
			WriteDxfId("tableBorderDxfId", table.TableBorderFormatting);
			WriteDxfId("totalsRowBorderDxfId", table.TotalRowBorderFormatting);
		}
		void WriteDxfId(string attributeName, XlDifferentialFormatting formatting) {
			int differentialFormatId = RegisterDifferentialFormatting(formatting);
			if(differentialFormatId >= 0)
				WriteIntValue(attributeName, differentialFormatId);
		}
		void GenerateTableAutoFilterContent(XlTable table) {
			if(!table.HasHeaderRow || !table.HasAutoFilter)
				return;
			XlCellPosition autoFilterBottomRight;
			if(table.HasTotalRow)
				autoFilterBottomRight = new XlCellPosition(table.Range.BottomRight.Column, table.Range.BottomRight.Row - 1);
			else
				autoFilterBottomRight = table.Range.BottomRight;
			XlCellRange autoFilterRange = new XlCellRange(table.Range.TopLeft, autoFilterBottomRight);
			WriteShStartElement("autoFilter");
			try {
				WriteStringValue("ref", autoFilterRange.ToString());
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateTableColumnsContent(XlTable table) {
			WriteShStartElement("tableColumns");
			try {
				int columnCount = table.Columns.Count;
				WriteIntValue("count", columnCount);
				for(int i = 0; i < columnCount; i++)
					GenerateTableColumnContent(table.Columns[i], i + 1);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateTableColumnContent(IXlTableColumn column, int id) {
			WriteShStartElement("tableColumn");
			try {
				WriteIntValue("id", id);
				WriteStringValue("name", EncodeXmlChars(column.Name));
				if(column.TotalRowFunction != XlTotalRowFunction.None)
					WriteStringValue("totalsRowFunction", totalRowFunctionTable[column.TotalRowFunction]);
				if(!String.IsNullOrEmpty(column.TotalRowLabel))
					WriteStringValue("totalsRowLabel", EncodeXmlChars(column.TotalRowLabel));
				WriteDxfId("headerRowDxfId", column.HeaderRowFormatting);
				WriteDxfId("dataDxfId", column.DataFormatting);
				WriteDxfId("totalsRowDxfId", column.TotalRowFormatting);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateTableStyleInfoContent(XlTable table) {
			WriteShStartElement("tableStyleInfo");
			try {
				if(!string.IsNullOrEmpty(table.Style.Name))
					WriteStringValue("name", table.Style.Name);
				WriteBoolValue("showColumnStripes", table.Style.ShowColumnStripes);
				WriteBoolValue("showFirstColumn", table.Style.ShowFirstColumn);
				WriteBoolValue("showLastColumn", table.Style.ShowLastColumn);
				WriteBoolValue("showRowStripes", table.Style.ShowRowStripes);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
