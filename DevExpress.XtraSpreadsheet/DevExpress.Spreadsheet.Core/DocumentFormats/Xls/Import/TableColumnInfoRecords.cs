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
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsTableColumnInfo // Feature11FieldDataItem structure
	public class XlsTableColumnInfo {
		#region Static Members
		public static XlsTableColumnInfo FromTableColumnInfo(int index, TableColumn info, TableFeatureType tableInfo, ExportXlsStyleSheet styleSheet) {
			XlsTableColumnInfo result = new XlsTableColumnInfo(tableInfo);
			result.AssignThisProperties(index, info, styleSheet);
			return result;
		}
		static Dictionary<XlsXmlDataType, string> xmlDataTypeToStringTable = CreateXmlDataTypeToStringTable();
		static Dictionary<string, XlsXmlDataType> stringToXmlDataTypeTable = CreateStringToXmlDataTypeTable(xmlDataTypeToStringTable);
		static Dictionary<XlsXmlDataType, string> CreateXmlDataTypeToStringTable() {
			Dictionary<XlsXmlDataType, string> result = new Dictionary<XlsXmlDataType, string>();
			result.Add(XlsXmlDataType.DataType_AnyType, "anyType");
			result.Add(XlsXmlDataType.DataType_AnyUri, "anyURI");
			result.Add(XlsXmlDataType.DataType_Base64Binary, "base64Binary");
			result.Add(XlsXmlDataType.DataType_Boolean, "boolean");
			result.Add(XlsXmlDataType.DataType_Byte, "byte");
			result.Add(XlsXmlDataType.DataType_Date, "date");
			result.Add(XlsXmlDataType.DataType_DateTime, "dateTime");
			result.Add(XlsXmlDataType.DataType_Day, "gDay");
			result.Add(XlsXmlDataType.DataType_Decimal, "decimal");
			result.Add(XlsXmlDataType.DataType_Double, "double");
			result.Add(XlsXmlDataType.DataType_Duration, "duration");
			result.Add(XlsXmlDataType.DataType_Entities, "ENTITIES");
			result.Add(XlsXmlDataType.DataType_Entity, "ENTITY");
			result.Add(XlsXmlDataType.DataType_Float, "float");
			result.Add(XlsXmlDataType.DataType_HexBinary, "hexBinary");
			result.Add(XlsXmlDataType.DataType_Id, "ID");
			result.Add(XlsXmlDataType.DataType_IdRef, "IDREF");
			result.Add(XlsXmlDataType.DataType_IdRefs, "IDREFS");
			result.Add(XlsXmlDataType.DataType_Int, "int");
			result.Add(XlsXmlDataType.DataType_Integer, "integer");
			result.Add(XlsXmlDataType.DataType_Language, "language");
			result.Add(XlsXmlDataType.DataType_Long, "long");
			result.Add(XlsXmlDataType.DataType_Month, "gMonth");
			result.Add(XlsXmlDataType.DataType_MonthDay, "gMonthDay");
			result.Add(XlsXmlDataType.DataType_Name, "Name");
			result.Add(XlsXmlDataType.DataType_NcName, "NCName");
			result.Add(XlsXmlDataType.DataType_NegativeInteger, "negativeInteger");
			result.Add(XlsXmlDataType.DataType_NmToken, "NMTOKEN");
			result.Add(XlsXmlDataType.DataType_NmTokens, "NMTOKENS");
			result.Add(XlsXmlDataType.DataType_NonNegativeInteger, "nonNegativeInteger");
			result.Add(XlsXmlDataType.DataType_NonPositiveInteger, "nonPositiveInteger");
			result.Add(XlsXmlDataType.DataType_NormalizedString, "normalizedString");
			result.Add(XlsXmlDataType.DataType_Notation, "NOTATION");
			result.Add(XlsXmlDataType.DataType_PositiveInteger, "positiveInteger");
			result.Add(XlsXmlDataType.DataType_QName, "QName");
			result.Add(XlsXmlDataType.DataType_Short, "short");
			result.Add(XlsXmlDataType.DataType_String, "string");
			result.Add(XlsXmlDataType.DataType_Time, "time");
			result.Add(XlsXmlDataType.DataType_Token, "token");
			result.Add(XlsXmlDataType.DataType_UnsignedByte, "unsignedByte");
			result.Add(XlsXmlDataType.DataType_UnsignedInt, "unsignedInt");
			result.Add(XlsXmlDataType.DataType_UnsignedLong, "unsignedLong");
			result.Add(XlsXmlDataType.DataType_UnsignedShort, "unsignedShort");
			result.Add(XlsXmlDataType.DataType_Year, "gYear");
			result.Add(XlsXmlDataType.DataType_YearMonth, "gYearMonth");
			return result;
		}
		static Dictionary<string, XlsXmlDataType> CreateStringToXmlDataTypeTable(Dictionary<XlsXmlDataType, string> table) {
			Dictionary<string, XlsXmlDataType> result = new Dictionary<string, XlsXmlDataType>();
			foreach (KeyValuePair<XlsXmlDataType, string> pair in table)
				result.Add(pair.Value, pair.Key);
			return result;
		}
		static string ConvertXmlDataTypeToString(XlsXmlDataType value) {
			if (xmlDataTypeToStringTable.ContainsKey(value))
				return xmlDataTypeToStringTable[value];
			return xmlDataTypeToStringTable[XlsXmlDataType.DataType_AnyType];
		}
		static XlsXmlDataType ConvertStringToXmlDataType(string value) {
			if (stringToXmlDataTypeTable.ContainsKey(value))
				return stringToXmlDataTypeTable[value];
			return XlsXmlDataType.DataType_AnyType;
		}
		#endregion
		#region Fields
		const short fixedPartSize = 36;
		internal const int TableStyleId = -1;
		readonly TableFeatureType tableInfo;
		readonly XlsXMapInfo xMapInfo = new XlsXMapInfo();
		XlsWebBasedDataProviderDataType webBasedDataProviderDataType = XlsWebBasedDataProviderDataType.NotUsed;
		XlsXmlDataType xmlDataType = XlsXmlDataType.NotUsed;
		TotalsRowFunctionType totalsRowFunctionType = TotalsRowFunctionType.None;
		DxfN12ListInfo totalRowFormatInfo;
		DxfN12ListInfo insertRowFormatInfo;
		DxfN12ListInfo headerRowFormatInfo;
		XLUnicodeString headerRowCellStyleName;
		XLUnicodeString fieldName = new XLUnicodeString();
		XLUnicodeString captionName = new XLUnicodeString();
		XLUnicodeString totalString = new XLUnicodeString();
		XlsAutoFilterInfo autoFilterInfo;
		XlsColumnFormulaInfo columnFormulaInfo;
		XlsTotalColumnFormulaInfo totalColumnFormulaInfo;
		XlsWebBasedDataProviderInfo webBasedDataProviderInfo;
		int totalRowCellStyleId = TableStyleId;
		int insertRowCellStyleId = TableStyleId;
		bool saveStyleName;
		int variablePartSize = 0;
		#endregion
		public XlsTableColumnInfo(TableFeatureType tableInfo) {
			Guard.ArgumentNotNull(tableInfo, "TableFeatureType");
			this.tableInfo = tableInfo;
		}
		#region Properties
		public TableFeatureType TableInfo { get { return tableInfo; } }
		public XlsAutoFilterInfo AutoFilterInfo { get { return autoFilterInfo; } set { autoFilterInfo = value; } }
		public XlsXMapInfo XMapInfo { get { return xMapInfo; } }
		public XlsWebBasedDataProviderDataType WebBasedDataProviderDataType { get { return webBasedDataProviderDataType; } set { webBasedDataProviderDataType = value; } }
		public XlsXmlDataType XmlDataType { get { return xmlDataType; } set { xmlDataType = value; } }
		public TotalsRowFunctionType TotalsRowFunctionType { get { return totalsRowFunctionType; } set { totalsRowFunctionType = value; } }
		public XLUnicodeString FieldName { get { return fieldName; } set { fieldName = value; } }
		public XLUnicodeString CaptionName { get { return captionName; } set { captionName = value; } }
		public XLUnicodeString TotalString { get { return totalString; } set { totalString = value; } }
		public XlsColumnFormulaInfo ColumnFormulaInfo { get { return columnFormulaInfo; } set { columnFormulaInfo = value; } }
		public XlsTotalColumnFormulaInfo TotalColumnFormulaInfo { get { return totalColumnFormulaInfo; } set { totalColumnFormulaInfo = value; } }
		public XlsWebBasedDataProviderInfo WebBasedDataProviderInfo { get { return webBasedDataProviderInfo; } set { webBasedDataProviderInfo = value; } }
		public DxfN12ListInfo TotalRowFormatInfo { get { return totalRowFormatInfo; } set { totalRowFormatInfo = value; } }
		public DxfN12ListInfo InsertRowFormatInfo { get { return insertRowFormatInfo; } set { insertRowFormatInfo = value; } }
		public DxfN12ListInfo HeaderRowFormatInfo { get { return headerRowFormatInfo; } set { headerRowFormatInfo = value; } }
		public int TotalRowCellStyleId { get { return totalRowCellStyleId; } set { totalRowCellStyleId = value; } }
		public int InsertRowCellStyleId { get { return insertRowCellStyleId; } set { insertRowCellStyleId = value; } }
		public XLUnicodeString HeaderRowCellStyleName {
			get { return headerRowCellStyleName; }
			set {
				saveStyleName = value != null;
				headerRowCellStyleName = value;
			}
		}
		public bool SaveStyleName {
			get { return saveStyleName; }
			set {
				if (value) {
					if (headerRowCellStyleName == null)
						headerRowCellStyleName = new XLUnicodeString();
				}
				else
					headerRowCellStyleName = null;
				saveStyleName = value;
			}
		}
		public int IdField { get; set; }
		public int TotalRowFormatSize { get; set; }
		public int InsertRowFormatSize { get; set; }
		public int HeaderRowFormatSize { get; set; }
		public int QsifId { get; set; }
		public bool HasAutoFilter { get; set; }
		public bool AutoFilterHidden { get; set; }
		public bool LoadXMapi { get; set; }
		public bool LoadFormula { get; set; }
		public bool LoadTotalFormula { get; set; }
		public bool LoadTotalArray { get; set; }
		public bool LoadTotalString { get; set; }
		public bool AutoCreateCalcColumn { get; set; }
		#endregion
		public void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			IdField = (int)reader.ReadUInt32();
			WebBasedDataProviderDataType = (XlsWebBasedDataProviderDataType)reader.ReadUInt32();
			XmlDataType = (XlsXmlDataType)reader.ReadUInt32();
			TotalsRowFunctionType = TotalRowFunctionCodeToType(reader.ReadInt32());
			TotalRowFormatSize = (int)reader.ReadUInt32();
			TotalRowCellStyleId = (int)reader.ReadUInt32();
			ReadFlags(reader);
			InsertRowFormatSize = (int)reader.ReadUInt32();
			InsertRowCellStyleId = (int)reader.ReadUInt32();
			FieldName = XLUnicodeString.FromStream(reader);
			if ((TableInfo.TableType == TableType.Worksheet || TableInfo.TableType == TableType.Xml) && (FieldName.Value == "0"))
				FieldName.Value = string.Empty;
			if (!TableInfo.FlagsInfo.SingleCell)
				CaptionName = XLUnicodeString.FromStream(reader);
			if (TotalRowFormatSize > 0)
				TotalRowFormatInfo = DxfN12ListInfo.FromStream(reader, TotalRowFormatSize);
			if (InsertRowFormatSize > 0)
				InsertRowFormatInfo = DxfN12ListInfo.FromStream(reader, InsertRowFormatSize);
			ReadAutoFilterInfo(reader);
			if (LoadXMapi)
				XMapInfo.Read(reader);
			ReadColumnFormula(reader, contentBuilder.RPNContext);
			ReadTotalColumnFormula(reader, contentBuilder.RPNContext);
			if (LoadTotalString)
				TotalString = XLUnicodeString.FromStream(reader);
			ReadWebBasedDataProviderInfo(reader);
			ReadQsifId(reader);
			ReadCachedDiskHeader(reader);
		}
		void ReadFlags(BinaryReader reader) {
			uint bitwiseField = reader.ReadUInt32();
			HasAutoFilter = Convert.ToBoolean(bitwiseField & 0x00000001);
			AutoFilterHidden = Convert.ToBoolean(bitwiseField & 0x00000002);
			LoadXMapi = Convert.ToBoolean(bitwiseField & 0x00000004);
			LoadFormula = Convert.ToBoolean(bitwiseField & 0x00000008);
			LoadTotalFormula = Convert.ToBoolean(bitwiseField & 0x00000080);
			LoadTotalArray = Convert.ToBoolean(bitwiseField & 0x00000100);
			SaveStyleName = Convert.ToBoolean(bitwiseField & 0x00000200);
			LoadTotalString = Convert.ToBoolean(bitwiseField & 0x0000400);
			AutoCreateCalcColumn = Convert.ToBoolean(bitwiseField & 0x00000800);
		}
		void ReadAutoFilterInfo(BinaryReader reader) {
			if (!HasAutoFilter)
				return;
			int size = reader.ReadInt32();
			reader.ReadInt16();
			if (size > 0)
				AutoFilterInfo = XlsAutoFilterInfo.FromStream(reader);
		}
		void ReadColumnFormula(BinaryReader reader, XlsRPNContext context) {
			if (!LoadFormula)
				return;
			ColumnFormulaInfo = new XlsColumnFormulaInfo();
			ColumnFormulaInfo.Read(reader, context);
		}
		void ReadTotalColumnFormula(BinaryReader reader, XlsRPNContext context) {
			if (!LoadTotalFormula)
				return;
			TotalColumnFormulaInfo = new XlsTotalColumnFormulaInfo(LoadTotalArray);
			TotalColumnFormulaInfo.Read(reader, context);
		}
		void ReadWebBasedDataProviderInfo(BinaryReader reader) {
			if (TableInfo.TableType != TableType.SharePoint)
				return;
			WebBasedDataProviderInfo = new XlsWebBasedDataProviderInfo(webBasedDataProviderDataType);
			WebBasedDataProviderInfo.Read(reader);
		}
		void ReadQsifId(BinaryReader reader) {
			if (TableInfo.TableType != TableType.QueryTable)
				return;
			QsifId = (int)reader.ReadUInt32();
			variablePartSize += 4;
		}
		void ReadCachedDiskHeader(BinaryReader reader) {
			if (TableInfo.HasHeadersRow || TableInfo.FlagsInfo.SingleCell)
				return;
			HeaderRowFormatSize = reader.ReadInt32();
			if (HeaderRowFormatSize > 0)
				HeaderRowFormatInfo = DxfN12ListInfo.FromStream(reader, HeaderRowFormatSize);
			if (SaveStyleName)
				HeaderRowCellStyleName = XLUnicodeString.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((uint)IdField);
			writer.Write((uint)WebBasedDataProviderDataType);
			writer.Write((uint)XmlDataType);
			writer.Write(TotalRowFunctionTypeToCode(TotalsRowFunctionType));
			WriteRowFormatInfoSize(writer, TotalRowFormatInfo);
			writer.Write((uint)TotalRowCellStyleId);
			WriteFlags(writer);
			WriteRowFormatInfoSize(writer, InsertRowFormatInfo);
			writer.Write((uint)InsertRowCellStyleId);
			if (string.IsNullOrEmpty(FieldName.Value) && (TableInfo.TableType == TableType.Worksheet || TableInfo.TableType == TableType.Xml)) {
				XLUnicodeString fakeFieldName = new XLUnicodeString();
				fakeFieldName.Value = "0";
				fakeFieldName.Write(writer);
			}
			else
				FieldName.Write(writer);
			if (!TableInfo.FlagsInfo.SingleCell)
				CaptionName.Write(writer);
			WriteRowFormatInfo(writer, TotalRowFormatInfo);
			WriteRowFormatInfo(writer, InsertRowFormatInfo);
			WriteAutoFilterInfo(writer);
			if (LoadXMapi)
				XMapInfo.Write(writer);
			if (LoadFormula)
				WriteFormulaInfo(writer, ColumnFormulaInfo);
			if (LoadTotalFormula)
				WriteFormulaInfo(writer, TotalColumnFormulaInfo);
			if (LoadTotalString)
				TotalString.Write(writer);
			WriteWebBasedDataProviderInfo(writer);
			WriteQsifId(writer);
			WriteCachedDiskHeader(writer);
		}
		void WriteRowFormatInfoSize(BinaryWriter writer, DxfN12ListInfo info) {
			if (info != null)
				writer.Write((uint)info.GetSize());
			else
				writer.Write((uint)0);
		}
		void WriteRowFormatInfo(BinaryWriter writer, DxfN12ListInfo info) {
			if (info != null && info.GetSize() > 0)
				info.Write(writer);
		}
		void WriteFlags(BinaryWriter writer) {
			uint bitwiseField = 0;
			if (HasAutoFilter)
				bitwiseField |= 0x00000001;
			if (AutoFilterHidden)
				bitwiseField |= 0x00000002;
			if (LoadXMapi)
				bitwiseField |= 0x00000004;
			if (LoadFormula)
				bitwiseField |= 0x00000008;
			if (LoadTotalFormula)
				bitwiseField |= 0x00000080;
			if (LoadTotalArray)
				bitwiseField |= 0x00000100;
			if (SaveStyleName)
				bitwiseField |= 0x00000200;
			if (LoadTotalString)
				bitwiseField |= 0x00000400;
			if (AutoCreateCalcColumn)
				bitwiseField |= 0x00000800;
			writer.Write(bitwiseField);
		}
		void WriteAutoFilterInfo(BinaryWriter writer) {
			if (!HasAutoFilter)
				return;
			if (AutoFilterInfo != null) {
				writer.Write((uint)AutoFilterInfo.GetSize());
				writer.Write((ushort)0xffff);
				AutoFilterInfo.Write(writer);
			}
			else {
				writer.Write((uint)0);
				writer.Write((ushort)0xffff);
			}
		}
		void WriteFormulaInfo(BinaryWriter writer, XlsColumnFormulaInfo info) {
			if (info != null)
				info.Write(writer);
		}
		void WriteWebBasedDataProviderInfo(BinaryWriter writer) {
			if (TableInfo.TableType == TableType.SharePoint && WebBasedDataProviderInfo != null)
				WebBasedDataProviderInfo.Write(writer);
		}
		void WriteQsifId(BinaryWriter writer) {
			if (TableInfo.TableType != TableType.QueryTable)
				return;
			writer.Write((uint)QsifId);
			variablePartSize += 4;
		}
		void WriteCachedDiskHeader(BinaryWriter writer) {
			if (TableInfo.HasHeadersRow || TableInfo.FlagsInfo.SingleCell)
				return;
			WriteRowFormatInfoSize(writer, HeaderRowFormatInfo);
			WriteRowFormatInfo(writer, HeaderRowFormatInfo);
			if (SaveStyleName)
				HeaderRowCellStyleName.Write(writer);
		}
		int GetCachedDiskHeaderSize() {
			int result = 4;
			if (HeaderRowFormatInfo != null)
				result += HeaderRowFormatInfo.GetSize();
			if (SaveStyleName)
				result += HeaderRowCellStyleName.Length;
			return result;
		}
		#region CreateTableColumnInfo
		protected internal TableColumn CreateTableColumnInfo(XlsContentBuilder contentBuilder) {
			TableColumn result;
			Table table = contentBuilder.ActiveTable;
			string columnCaption = CaptionName.Value;
			if (!String.IsNullOrEmpty(columnCaption)) {
				string uniqueCaption = GetUniqueColumnCaption(table, columnCaption);
				result = new TableColumn(table, uniqueCaption);
				if (columnCaption != uniqueCaption) {
					string msg = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_TableColumnNameChanged), columnCaption, table.Name, uniqueCaption);
					contentBuilder.LogMessage(LogCategory.Info, msg);
				}
			}
			else
				result = new TableColumn(table);
			AssignTableColumnInfoProperties(result, contentBuilder);
			return result;
		}
		protected internal void RestoreColumnFormula(TableColumn info) {
			if (!AutoCreateCalcColumn)
				return;
			CellRange range = info.GetColumnRangeWithoutHeadersAndTotalsRows();
			foreach (ICellBase cellInfo in range.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (cell != null && cell.HasFormula) {
					info.BeginUpdate();
					try {
						FormulaBase cellFormula = cell.GetFormula();
						bool isArrayFormula = cellFormula is ArrayFormula || cellFormula is ArrayFormulaPart;
						info.SetColumnFormulaCore(cellFormula.Expression, isArrayFormula);
					}
					finally {
						info.EndUpdate();
					}
					return;
				}
			}
		}
		string GetUniqueColumnCaption(Table table, string caption) {
			if (IsUniqueColumnCaption(table, caption))
				return caption;
			int modifier = 2;
			string result = caption + modifier.ToString();
			while (!IsUniqueColumnCaption(table, result)) {
				modifier++;
				result = caption + modifier.ToString();
			}
			return result;
		}
		bool IsUniqueColumnCaption(Table table, string caption) {
			foreach (TableColumn column in table.Columns) {
				if (column.Name == caption)
					return false;
			}
			return true;
		}
		void AssignTableColumnInfoProperties(TableColumn info, XlsContentBuilder contentBuilder) {
			Table table = contentBuilder.ActiveTable;
			info.BeginUpdate();
			try {
				info.TotalsRowFunction = TotalsRowFunctionType;
				if (!String.IsNullOrEmpty(TotalString.Value))
					info.SetTotalsRowLabelCore(TotalString.Value);
				if (!String.IsNullOrEmpty(FieldName.Value))
					info.UniqueName = FieldName.Value;
				info.Id = IdField; 
				if (TableInfo.TableType == TableType.QueryTable)
					info.QueryTableFieldId = QsifId; 
				if (LoadTotalFormula)
					info.SetTotalsRowFormulaCore(TotalColumnFormulaInfo.GetFormulaExpression(contentBuilder), LoadTotalArray);
				if (HasAutoFilter) {
					int columnId = table.Columns.Count;
					AutoFilterColumnCollection filterColumns = table.AutoFilter.FilterColumns;
					AutoFilterColumn filterColumn = filterColumns[columnId];
					if (autoFilterInfo == null)
						filterColumn.HiddenAutoFilterButton = AutoFilterHidden;
					else
						autoFilterInfo.ApplyContent(filterColumn, columnId, AutoFilterHidden, contentBuilder);
				}
				if (table.TableType == TableType.Xml && XMapInfo.XMapEntry != null) {
					XmlColumnProperties xmlProperties = new XmlColumnProperties();
					xmlProperties.MapId = XMapInfo.XMapEntry.Details.MapId;
					xmlProperties.Xpath = XMapInfo.XMapEntry.Details.XPathExpession.Value;
					xmlProperties.XmlDataType = ConvertXmlDataTypeToString(XmlDataType);
					info.XmlProperties = xmlProperties;
				}
			}
			finally {
				info.EndUpdate();
			}
			if (TotalRowFormatInfo != null)
				AssignDifferentialFormat(info, contentBuilder, TotalRowFormatInfo, TableColumn.TotalsRowIndex);
			if (TotalRowCellStyleId != TableStyleId)
				AssignCellStyle(info, contentBuilder, TotalRowCellStyleId, TableColumn.TotalsRowIndex);
			if (InsertRowFormatInfo != null)
				AssignDifferentialFormat(info, contentBuilder, InsertRowFormatInfo, TableColumn.DataIndex);
			if (InsertRowCellStyleId != TableStyleId)
				AssignCellStyle(info, contentBuilder, InsertRowCellStyleId, TableColumn.DataIndex);
			AssignCachedDiskHeader(info, contentBuilder);
		}
		void AssignDifferentialFormat(TableColumn columnInfo, XlsContentBuilder contentBuilder, DxfN12ListInfo formatInfo, int elementIndex) {
			Guard.ArgumentNotNull(formatInfo, "formatInfo");
			DifferentialFormat format = formatInfo.GetDifferentialFormat(contentBuilder);
			int formatIndex = contentBuilder.DocumentModel.Cache.CellFormatCache.AddItem(format);
			columnInfo.AssignDifferentialFormatIndex(elementIndex, formatIndex);
		}
		void AssignCellStyle(TableColumn info, XlsContentBuilder contentBuilder, int cellStyleId, int elementIndex) {
			XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
			int index = styleSheet.XFCellStyleIndexTable[cellStyleId];
			CellStyleBase cellStyle = styleSheet.CellStyles[index];
			AssignCellStyleCore(info, cellStyle, elementIndex);
		}
		void AssignCellStyle(TableColumn info, XlsContentBuilder contentBuilder, string name, int elementIndex) {
			CellStyleBase cellStyle = contentBuilder.StyleSheet.CellStyles.GetCellStyleByName(name);
			AssignCellStyleCore(info, cellStyle, elementIndex);
		}
		void AssignCellStyleCore(TableColumn columnInfo, CellStyleBase cellStyle, int elementIndex) {
			int formatIndex = GetCellFormatIndex(cellStyle);
			columnInfo.AssignCellFormatIndex(elementIndex, formatIndex);
			AssignApplyFlagsInfo(columnInfo, elementIndex);
		}
		int GetCellFormatIndex(CellStyleBase cellStyle) {
			DocumentModel documentModel = cellStyle.DocumentModel;
			CellFormat format = new CellFormat(documentModel);
			format.Style = cellStyle;
			return documentModel.Cache.CellFormatCache.AddItem(format);
		}
		void AssignApplyFlagsInfo(TableColumn columnInfo, int elementIndex) {
			TableCellStyleApplyFlagsInfo options = columnInfo.ApplyFlagsInfo.Clone();
			options.SetApplyCellStyle(elementIndex, true);
			columnInfo.AssignApplyFlagsIndex(options.PackedValues);
		}
		void AssignCachedDiskHeader(TableColumn info, XlsContentBuilder contentBuilder) {
			if (TableInfo.HasHeadersRow || TableInfo.FlagsInfo.SingleCell)
				return;
			if (HeaderRowFormatInfo != null)
				AssignDifferentialFormat(info, contentBuilder, HeaderRowFormatInfo, TableColumn.HeaderRowIndex);
			if (SaveStyleName)
				AssignCellStyle(info, contentBuilder, HeaderRowCellStyleName.Value, TableColumn.HeaderRowIndex);
		}
		#endregion
		#region AssignThisProperties
		protected internal bool HasLoadTotalString(TableColumn info) {
			return !info.Table.HasTotalsRow && info.TotalsRowFunction == TotalsRowFunctionType.None && !String.IsNullOrEmpty(info.TotalsRowLabel);
		}
		protected internal bool HasLoadTotalFormula(TableColumn info) {
			return  info.TotalsRowFunction == TotalsRowFunctionType.Custom;
		}
		void AssignThisProperties(int index, TableColumn info, ExportXlsStyleSheet styleSheet) {
			IdField = index + 1;
			if (!String.IsNullOrEmpty(info.Name))
				CaptionName.Value = info.Name;
			TotalsRowFunctionType = info.TotalsRowFunction;
			if (HasLoadTotalString(info)) {
				LoadTotalString = true;
				TotalString.Value = info.TotalsRowLabel;
			}
			if (!String.IsNullOrEmpty(info.UniqueName))
				FieldName.Value = info.UniqueName;
				IdField = info.Id;
			if (info.QueryTableFieldId != TableColumn.DefaultQueryTableFieldId)
				QsifId = info.QueryTableFieldId;
			AutoCreateCalcColumn = info.HasColumnFormula;
			if (HasLoadTotalFormula(info)) {
				LoadTotalFormula = true;
				LoadTotalArray = info.TotalsRowFormulaIsArray;
				TotalColumnFormulaInfo = XlsTotalColumnFormulaInfo.FromTotalColumnFormula(info.TotalsRowFormula, styleSheet.RPNContext);
			}
			if (info.Table.AutoFilter.Enabled) {
				HasAutoFilter = true;
				AutoFilterColumn filterColumn = info.Table.AutoFilter.FilterColumns[index];
				if (filterColumn.IsNonDefault) {
					AutoFilterHidden = filterColumn.HiddenAutoFilterButton;
					AssignAutoFilter(filterColumn);
				}
			}
			if (this.TableInfo.TableType == TableType.Xml && info.XmlProperties != null) {
				LoadXMapi = true;
				XmlDataType = ConvertStringToXmlDataType(info.XmlProperties.XmlDataType);
				XMapInfo.XMapEntry = new XMapEntry();
				XMapInfo.XMapEntry.Details.MapId = info.XmlProperties.MapId;
				XMapInfo.XMapEntry.Details.XPathExpession.Value = info.XmlProperties.Xpath;
			}
			if (info.TotalsRow.ApplyDifferentialFormat) {
				TotalRowFormatInfo = GetFormatInfo(info, TableColumn.TotalsRowIndex);
				TotalRowFormatSize = TotalRowFormatInfo.GetSize();
			}
			if (info.Data.ApplyDifferentialFormat) {
				InsertRowFormatInfo = GetFormatInfo(info, TableColumn.DataIndex);
				InsertRowFormatSize = InsertRowFormatInfo.GetSize();
			}
			if (info.TotalsRow.ApplyCellStyle)
				TotalRowCellStyleId = GetCellStyleId(info, TableColumn.TotalsRowIndex, styleSheet);
			if (info.Data.ApplyCellStyle)
				InsertRowCellStyleId = GetCellStyleId(info, TableColumn.DataIndex, styleSheet);
			AssignHeaderRowFormat(info);
		}
		void AssignAutoFilter(AutoFilterColumn filterColumn) {
			if (filterColumn.ShouldWriteAutoFilter12) {
				AutoFilterInfo = new XlsAutoFilterInfo();
				AutoFilterInfo.FirstCriteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
			}
			else
				AutoFilterInfo = XlsAutoFilterInfo.CreateForExport(filterColumn);
		}
		DxfN12ListInfo GetFormatInfo(TableColumn info, int elementIndex) {
			DifferentialFormat format = info.GetDifferentialFormat(elementIndex);
			return DxfN12ListInfo.FromDifferentialFormat(format);
		}
		int GetCellStyleId(TableColumn info, int elementIndex, ExportXlsStyleSheet styleSheet) {
			CellFormat cellFormat = info.GetCellFormat(elementIndex);
			return styleSheet.CellStyleIndexTable[cellFormat.StyleIndex];
		}
		void AssignHeaderRowFormat(TableColumn info) {
			if (TableInfo.HasHeadersRow || TableInfo.FlagsInfo.SingleCell)
				return;
			if (info.HeaderRow.ApplyDifferentialFormat) {
				HeaderRowFormatInfo = GetFormatInfo(info, TableColumn.HeaderRowIndex);
				HeaderRowFormatSize = HeaderRowFormatInfo.GetSize();
			}
			if (info.HeaderRow.ApplyCellStyle) {
				HeaderRowCellStyleName = new XLUnicodeString();
				HeaderRowCellStyleName.Value = info.HeaderRow.CellStyle.Name;
			}
		}
		#endregion
		public short GetSize() {
			int result = fixedPartSize;
			if (string.IsNullOrEmpty(FieldName.Value) && (TableInfo.TableType == TableType.Worksheet || TableInfo.TableType == TableType.Xml))
				result += 4;
			else
				result += FieldName.Length;
			if (!TableInfo.FlagsInfo.SingleCell)
				result += CaptionName.Length;
			if (LoadTotalString)
				result += TotalString.Length;
			if (TotalRowFormatInfo != null)
				result += TotalRowFormatInfo.GetSize();
			if (InsertRowFormatInfo != null)
				result += InsertRowFormatInfo.GetSize();
			if (HasAutoFilter) {
				result += 6;
				if (AutoFilterInfo != null)
					result += AutoFilterInfo.GetSize();
			}
			if (LoadXMapi)
				result += XMapInfo.Size;
			if (LoadFormula)
				result += ColumnFormulaInfo.GetSize();
			if (LoadTotalFormula && TotalColumnFormulaInfo != null)
				result += TotalColumnFormulaInfo.GetSize();
			if (TableInfo.TableType == TableType.SharePoint && WebBasedDataProviderInfo != null)
				result += WebBasedDataProviderInfo.GetSize();
			if (TableInfo.TableType == TableType.QueryTable)
				result += 4;
			if (!TableInfo.HasHeadersRow && !TableInfo.FlagsInfo.SingleCell)
				result += GetCachedDiskHeaderSize();
			return (short)result;
		}
		int TotalRowFunctionTypeToCode(TotalsRowFunctionType functionType) {
			switch (functionType) {
				case TotalsRowFunctionType.Average: return 1;
				case TotalsRowFunctionType.Count: return 2;
				case TotalsRowFunctionType.CountNums: return 3;
				case TotalsRowFunctionType.Max: return 4;
				case TotalsRowFunctionType.Min: return 5;
				case TotalsRowFunctionType.Sum: return 6;
				case TotalsRowFunctionType.StdDev: return 7;
				case TotalsRowFunctionType.Var: return 8;
				case TotalsRowFunctionType.Custom: return 9;
			}
			return 0;
		}
		TotalsRowFunctionType TotalRowFunctionCodeToType(int functionCode) {
			switch (functionCode) {
				case 1: return TotalsRowFunctionType.Average;
				case 2: return TotalsRowFunctionType.Count;
				case 3: return TotalsRowFunctionType.CountNums;
				case 4: return TotalsRowFunctionType.Max;
				case 5: return TotalsRowFunctionType.Min;
				case 6: return TotalsRowFunctionType.Sum;
				case 7: return TotalsRowFunctionType.StdDev;
				case 8: return TotalsRowFunctionType.Var;
				case 9: return TotalsRowFunctionType.Custom;
			}
			return TotalsRowFunctionType.None;
		}
	}
	#endregion
	#region XlsXmlDataType
	public enum XlsXmlDataType {
		NotUsed = 0,
		Schema = 0x00001000,
		Attribute = 0x00001001,
		AttributeGroup = 0x00001002,
		Notation = 0x00001003,
		IdentityConstraint = 0x00001100,
		Key = 0x00001101,
		KeyRef = 0x00001102,
		Unique = 0x00001103,
		AnyType = 0x00002000,
		DataType = 0x00002100,
		DataType_AnyType = 0x00002101,
		DataType_AnyUri = 0x00002102,
		DataType_Base64Binary = 0x00002103,
		DataType_Boolean = 0x00002104,
		DataType_Byte = 0x00002105,
		DataType_Date = 0x00002106,
		DataType_DateTime = 0x00002107,
		DataType_Day = 0x00002108,
		DataType_Decimal = 0x00002109,
		DataType_Double = 0x0000210A,
		DataType_Duration = 0x0000210B,
		DataType_Entities = 0x0000210C,
		DataType_Entity = 0x0000210D,
		DataType_Float = 0x0000210E,
		DataType_HexBinary = 0x0000210F,
		DataType_Id = 0x00002110,
		DataType_IdRef = 0x00002111,
		DataType_IdRefs = 0x00002112,
		DataType_Int = 0x00002113,
		DataType_Integer = 0x00002114,
		DataType_Language = 0x00002115,
		DataType_Long = 0x00002116,
		DataType_Month = 0x00002117,
		DataType_MonthDay = 0x00002118,
		DataType_Name = 0x00002119,
		DataType_NcName = 0x0000211A,
		DataType_NegativeInteger = 0x0000211B,
		DataType_NmToken = 0x0000211C,
		DataType_NmTokens = 0x0000211D,
		DataType_NonNegativeInteger = 0x0000211E,
		DataType_NonPositiveInteger = 0x0000211F,
		DataType_NormalizedString = 0x00002120,
		DataType_Notation = 0x00002121,
		DataType_PositiveInteger = 0x00002122,
		DataType_QName = 0x00002123,
		DataType_Short = 0x00002124,
		DataType_String = 0x00002125,
		DataType_Time = 0x00002126,
		DataType_Token = 0x00002127,
		DataType_UnsignedByte = 0x00002128,
		DataType_UnsignedInt = 0x00002129,
		DataType_UnsignedLong = 0x0000212A,
		DataType_UnsignedShort = 0x0000212B,
		DataType_Year = 0x0000212C,
		DataType_YearMonth = 0x0000212D,
		DataType_AnySimpleType = 0x000021FF,
		SimpleType = 0x00002200,
		ComplexType = 0x00002400,
		Particle = 0x00004000,
		Any = 0x00004001,
		AnyAttribute = 0x00004002,
		Element = 0x00004003,
		Group = 0x00004100,
		All = 0x00004101,
		Choice = 0x00004102,
		Sequence = 0x00004103,
		EmptyParticle = 0x00004104,
		Null = 0x00000800,
		Null_Type = 0x00002800,
		Null_Any = 0x00004801,
		Null_AnyAttribute = 0x00004802,
		Null_Element = 0x00004803
	}
	#endregion
}
