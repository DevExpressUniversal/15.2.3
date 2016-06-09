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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xlsx;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	#region SpreadsheetMLBaseExporter (abstract class)
	public abstract class SpreadsheetMLBaseExporter : DocumentModelExporter {
		#region Fields
		DocumentExporterOptions options;
		XmlWriter documentContentWriter;
		#endregion
		protected SpreadsheetMLBaseExporter(DocumentModel workbook, DocumentExporterOptions options)
			: base(workbook) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#region Properties
		protected abstract string SpreadsheetNamespace { get; }
		protected DocumentExporterOptions Options { get { return options; } }
		protected internal XmlWriter DocumentContentWriter { get { return documentContentWriter; } set { documentContentWriter = value; } }
		#endregion
		protected internal virtual XmlWriterSettings CreateXmlWriterSettings() {
			return XmlBasedExporterUtils.Instance.CreateDefaultXmlWriterSettings();
		}
		#region Write Helpers
		#region Bool value
		protected internal virtual void WriteShBoolValue(string tag, bool value) {
			WriteShStringValue(tag, ConvertBoolToString(value));
		}
		protected internal virtual void WriteBoolValue(string tag, bool value) {
			WriteStringAttr(null, tag, null, ConvertBoolToString(value));
		}
		protected internal virtual void WriteShBoolAttr(string attr, bool value) {
			WriteShStringAttr(attr, ConvertBoolToString(value));
		}
		protected internal virtual void WriteBoolValue(string attr, bool value, bool defaultValue) {
			if (value != defaultValue)
				WriteBoolValue(attr, value);
		}
		protected internal virtual void WriteOptionalBoolValue(string attr, bool value, bool shouldExport) {
			if (shouldExport)
				WriteBoolValue(attr, value);
		}
		#endregion
		#region Int value
		protected internal virtual void WriteShIntValue(string tag, int value) {
			WriteShStringValue(tag, value.ToString());
		}
		protected internal virtual void WriteIntValue(string tag, int value) {
			WriteStringAttr(null, tag, null, value.ToString());
		}
		protected internal virtual void WriteShIntAttr(string attr, int value) {
			WriteShStringAttr(attr, value.ToString());
		}
		protected internal virtual void WriteIntValue(string attr, int value, bool shouldExport) {
			if (shouldExport)
				WriteIntValue(attr, value);
		}
		protected internal virtual void WriteIntValue(string attr, int value, int defaultValue) {
			WriteIntValue(attr, value, value != defaultValue);
		}
		protected internal virtual void WriteIntEmuValue(string tag, int value) {
			WriteStringAttr(null, tag, null, Workbook.UnitConverter.ModelUnitsToEmu(value).ToString());
		}
		protected internal virtual void WriteIntEmuValue(string attr, int value, bool shouldExport) {
			if (shouldExport)
				WriteIntEmuValue(attr, value);
		}
		protected internal virtual void WriteIntEmuValue(string attr, int value, int defaultValue) {
			WriteIntEmuValue(attr, value, value != defaultValue);
		}
		#endregion
		#region Long value
		protected internal virtual void WriteLongValue(string tag, long value) {
			WriteStringAttr(null, tag, null, value.ToString());
		}
		protected internal virtual void WriteLongValue(string tag, long value, bool shouldExport) {
			if (shouldExport)
				WriteLongValue(tag, value);
		}
		protected internal virtual void WriteLongValue(string tag, long value, int defaultValue) {
			WriteLongValue(tag, value, value != defaultValue);
		}
		#endregion
		#region Double value
		protected internal virtual void WriteDoubleValue(string tag, double value) {
			WriteStringAttr(null, tag, null, value.ToString(CultureInfo.InvariantCulture));
		}
		protected internal virtual void WriteDoubleValue(string attr, double value, bool shouldExport) {
			if (shouldExport)
				WriteDoubleValue(attr, value);
		}
		protected internal virtual void WriteDoubleValue(string attr, double value, double defaultValue) {
			WriteDoubleValue(attr, value, value != defaultValue);
		}
		#endregion
		#region Float value
		protected internal virtual void WriteFloatValue(string tag, float value) {
			WriteStringAttr(null, tag, null, value.ToString(CultureInfo.InvariantCulture));
		}
		protected internal virtual void WriteFloatValue(string attr, float value, bool shouldExport) {
			if (shouldExport)
				WriteFloatValue(attr, value);
		}
		protected internal virtual void WriteFloatValue(string attr, float value, float defaultValue) {
			WriteFloatValue(attr, value, value != defaultValue);
		}
		#endregion
		#region String value
		protected internal virtual void WriteShStringValue(string tag, string value) {
			WriteStringValue(tag, value);
		}
		protected internal virtual void WriteStringValue(string tag, string value) {
			WriteStringAttr(null, tag, null, value);
		}
		protected internal virtual void WriteStringValue(string attr, string value, bool shouldExport) {
			if (shouldExport)
				WriteStringValue(attr, value);
		}
		protected internal virtual void WriteShStringAttr(string attr, string value) {
			documentContentWriter.WriteAttributeString(attr, SpreadsheetNamespace, value);
		}
		protected internal virtual void WriteStringAttr(string prefix, string attr, string ns, string value) {
			documentContentWriter.WriteAttributeString(prefix, attr, ns, value);
		}
		protected internal virtual void WriteShString(string tag, string text) {
			WriteShString(tag, text, false);
		}
		protected internal virtual void WriteShString(string tag, string text, bool writeXmlSpaceAttr) {
			WriteString(tag, text, SpreadsheetNamespace, writeXmlSpaceAttr);
		}
		protected internal virtual void WriteShString(string text) {
			documentContentWriter.WriteString(text);
		}
		protected internal virtual void WriteString(string tag, string ns, string text) {
			WriteString(tag, text, ns, false);
		}
		protected internal virtual void WriteString(string tag, string text, string ns, bool writeXmlSpaceAttr) {
			WriteString(String.Empty, tag, text, ns, writeXmlSpaceAttr);
		}
		protected internal virtual void WriteString(string prefix, string tag, string text, string ns, bool writeXmlSpaceAttr) {
			if (!String.IsNullOrEmpty(prefix))
				WriteStartElement(prefix, tag, ns);
			else
				WriteStartElement(tag, ns);
			try {
				if (!string.IsNullOrEmpty(text)) {
					if (writeXmlSpaceAttr && IsNeedWriteXmlSpaceAttr(text))
						WriteStringAttr("xml", "space", null, "preserve");
					documentContentWriter.WriteString(text);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		bool IsNeedWriteXmlSpaceAttr(string text) {
			if (String.IsNullOrEmpty(text))
				return false;
			return IsSpace(text[0]) || IsSpace(text[text.Length - 1]) || text.Contains("  ");
		}
		bool IsSpace(char ch) {
			return ch == ' ' || ch == Characters.EmSpace || ch == Characters.EnSpace || ch == Characters.QmSpace || ch == '\u000A';
		}
		#endregion
		#region Start\end elements
		protected internal virtual void WriteShStartElement(string tag) {
			WriteStartElement(tag, SpreadsheetNamespace);
		}
		protected internal virtual void WriteStartElement(string tag, string ns) {
			documentContentWriter.WriteStartElement(tag, ns);
		}
		protected internal virtual void WriteStartElement(string prefix, string tag, string ns) {
			documentContentWriter.WriteStartElement(prefix, tag, ns);
		}
		protected internal virtual void WriteShEndElement() {
			WriteEndElement();
		}
		protected internal virtual void WriteEndElement() {
			documentContentWriter.WriteEndElement();
		}
		#endregion
		#region Vml values
		protected internal virtual void WriteVmlNullableBoolAttr(string attr, bool? value) {
			if (value.HasValue)
				WriteStringValue(attr, GetVmlBooleanShortStringValue(value.Value));
		}
		protected internal virtual void WriteVmlBoolAttr(string attr, string ns, bool value) {
			WriteStringAttr(String.Empty, attr, ns, GetVmlBooleanShortStringValue(value));
		}
		protected internal virtual void WriteVmlBoolValue(string tag, string ns, bool value) {
			WriteStartElement(tag, ns);
			try {
				WriteShString(GetVmlBooleanShortStringValue(value));
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void WriteVmlTrueFalseBlankValue(string tag, string ns, bool value, bool defaultValue) {
			WriteStartElement(tag, ns);
			try {
				if (value != defaultValue)
					WriteShString(GetVmlBooleanStringValue(value));
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		protected internal virtual void WriteCellPosition(string attr, CellPosition cellPosition) {
			WriteStringValue(attr, CellReferenceParser.ToString(cellPosition));
		}
		protected internal virtual void WritePercentageAttr(string attr, float value) {
			int intValue = (int)(value * 100);
			WriteStringAttr(String.Empty, attr, null, intValue.ToString(CultureInfo.InvariantCulture) + "%");
		}
		protected internal virtual string GetVmlBooleanShortStringValue(bool value) {
			return value ? "t" : "f";
		}
		protected internal virtual string GetVmlBooleanStringValue(bool value) {
			return value ? "True" : "False";
		}
		protected string GetStSqrefValue(CellRangeBase cellRange) {
			CellRangeBase preparedRange = cellRange.GetWithoutIntervals().GetWithModifiedPositionType(PositionType.Relative);
			return (preparedRange.RangeType == CellRangeType.UnionRange) ? ((CellUnion)preparedRange).ToString(' ', false) : preparedRange.ToString();
		}
		protected string GetStRefValue(CellRange cellRange) {
			CellRangeBase preparedRange = cellRange.GetWithoutIntervals().GetWithModifiedPositionType(PositionType.Relative);
			return preparedRange.ToString();
		}
		protected internal virtual void WriteStSqref(CellRangeBase cellRange, string attr) {
			WriteStringAttr(String.Empty, attr, null, GetStSqrefValue(cellRange));
		}
		protected internal virtual void WriteStRef(CellRange cellRange, string attr) {
			WriteStringAttr(String.Empty, attr, null, GetStRefValue(cellRange));
		}
		[CLSCompliant(false)]
		protected internal virtual void WriteSTUnsignedShortHexValue(string attr, UInt16 value) {
			WriteStringAttr(null, attr, null, value.ToString("X4"));
		}
		protected internal virtual void WriteDateTime(string attr, DateTime dateTime) {
			string dateTimeString = dateTime.ToString(Workbook.Culture.DateTimeFormat.SortableDateTimePattern);
			WriteStringValue(attr, dateTimeString);
		}
		protected internal virtual void WriteDateTime(string attr, DateTime dateTime, DateTime defaultDateTime) {
			WriteDateTime(attr, dateTime, DateTime.Compare(dateTime, defaultDateTime) != 0);
		}
		protected internal virtual void WriteDateTime(string attr, DateTime dateTime, bool shouldExport) {
			if (shouldExport)
				WriteDateTime(attr, dateTime);
		}
		#region WriteEnumValue
		protected internal virtual void WriteEnumValue<T>(string attr, T value, Dictionary<T, string> table) {
			WriteStringValue(attr, table[value]);
		}
		protected internal virtual void WriteEnumValue<T>(string attr, string ns, T value, Dictionary<T, string> table) {
			WriteStringAttr(String.Empty, attr, ns, table[value]);
		}
		protected internal virtual void WriteEnumValue<T>(string attr, T value, Dictionary<T, string> table, bool shouldExport) {
			if (shouldExport)
				WriteEnumValue(attr, value, table);
		}
		protected internal virtual void WriteEnumValue<T>(string attr, T value, Dictionary<T, string> table, T defaultValue) {
			if (!value.Equals(defaultValue))
				WriteEnumValue(attr, value, table);
		}
		#endregion
		#region WriteElementWithValAttr
		protected internal virtual void WriteElementWithValAttr(string tag, bool val) {
			WriteShStartElement(tag);
			try {
				WriteBoolValue("val", val);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void WriteElementWithValAttr(string tag, int val) {
			WriteShStartElement(tag);
			try {
				WriteIntValue("val", val);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void WriteElementWithValAttr(string tag, string val) {
			WriteShStartElement(tag);
			try {
				WriteStringAttr(null, "val", null, val);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#endregion
		#region Converters
		protected internal abstract string ConvertBoolToString(bool value);
		#endregion
	}
	#endregion
	#region OpenXmlExporter
	public partial class OpenXmlExporter : SpreadsheetMLBaseExporter {
		#region Constants
		public const string OfficeNamespaceConst = "urn:schemas-microsoft-com:office:office";
		public const string WordProcessingDrawingNamespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
		public const string DrawingMLNamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";
		public const string DrawingMLPictureNamespace = "http://schemas.openxmlformats.org/drawingml/2006/picture";
		public const string OfficeDocumentNamespace = XlsxPackageBuilder.OfficeDocumentNamespace;
		public const string RelsVolatileDependenciesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/volatileDependencies";
		public const string ExtendedPropertiesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
		public const string CorePropertiesNamespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
		public const string CustomPropertiesNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
		public const string CorePropertiesPrefix = "cp";
		public const string DcPropertiesPrefix = "dc";
		public const string DcPropertiesNamespace = "http://purl.org/dc/elements/1.1/";
		public const string DcTermsPropertiesPrefix = "dcterms";
		public const string DcTermsPropertiesNamespace = "http://purl.org/dc/terms/";
		public const string XsiPrefix = "xsi";
		public const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";
		public const string OfficeHyperlinkType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
		public const string SpreadsheetDrawingNamespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
		public const string DrawingMLChartNamespace = "http://schemas.openxmlformats.org/drawingml/2006/chart";
		public const string VmlDrawingNamespace = "urn:schemas-microsoft-com:vml";
		public const string VmlDrawingOfficeNamespace = "urn:schemas-microsoft-com:office:office";
		public const string VmlDrawingExcelNamespace = "urn:schemas-microsoft-com:office:excel";
		public const string VmlDrawingContentType = "application/vnd.openxmlformats-officedocument.vmlDrawing";
		public const string ThemeContentType = "application/vnd.openxmlformats-officedocument.theme+xml";
		public const string CalculationChainContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.calcChain+xml";
		public const string ConnectionsContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.connections+xml";
		public const string VbaProjectContentType = "application/vnd.ms-office.vbaProject";
		public const string VolatileDependenciesContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.volatileDependencies+xml";
		public const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		public const string OfficeDrawingChart14Namespace = "http://schemas.microsoft.com/office/drawing/2007/8/2/chart";
		public const string OfficeThemeRelationsFileName = "xl\\theme\\_rels\\theme1.xml.rels";
		public static string RelsCalculationChainNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/calcChain"; } }
		public static string RelsSharedStringsNamespace { get { return XlsxPackageBuilder.RelsSharedStringsNamespace; } }
		public static string RelsTablesNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table"; } }
		public static string RelsExternalLinkNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLink"; } }
		public static string RelsExternalLinkPathNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLinkPath"; } }
		public static string RelsExternalLinkMissedPathNamespace { get { return "http://schemas.microsoft.com/office/2006/relationships/xlExternalLinkPath/xlPathMissing"; } }
		public static string RelsTableNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table"; } }
		public static string RelsPivotCacheDefinitionNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotCacheDefinition"; } }
		public static string RelsPivotCacheRecordsNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotCacheRecords"; } }
		public static string RelsPivotTablesNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotTable"; } }
		public static string RelsConnectionsNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/connections"; } }
		public static string RelsXmlMapsNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/xmlMaps"; } }
		public static string RelsQueryTablesNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/queryTable"; } }
		public static string RelsCommentsNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments"; } }
		public static string RelsVmlDrawingNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing"; } }
		public static string RelsDrawingNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"; } }
		public static string RelsImagesNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image"; } }
		public static string RelsThemeNamespace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme"; } }
		public static string RelsVbaProjectNamespace { get { return "http://schemas.microsoft.com/office/2006/relationships/vbaProject"; } }
		public static string RelsChartNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart"; } }
		public static string RelsCustomXmlNamepace { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/customXml"; } }
		public static string ExternalTargetMode { get { return "External"; } }
		public static string RelsPrefix { get { return XlsxPackageBuilder.RelsPrefix; } }
		public static string RelsNamespace { get { return XlsxPackageBuilder.RelsNamespace; } }
		public static string SpreadsheetNamespaceConst { get { return XlsxPackageBuilder.SpreadsheetNamespaceConst; } }
		public static string PackageRelsNamespace { get { return XlsxPackageBuilder.PackageRelsNamespace; } }
		public static string RelsWorksheetNamespace { get { return XlsxPackageBuilder.RelsWorksheetNamespace; } }
		public static string RelsStylesNamespace { get { return XlsxPackageBuilder.RelsStylesNamespace; } }
		public static string WorkbookContentType { get { return XlsxPackageBuilder.WorkbookContentType; } }
		public static string RelsContentType { get { return XlsxPackageBuilder.RelsContentType; } }
		public static string XmlContentType { get { return XlsxPackageBuilder.XmlContentType; } }
		public static string NamespaceXmlnsX14 { get { return x14NamespaceReference; } }
		public static string PivotTableNamespaceXmlns { get { return xmNamespaceReference; } }
		public const string DataValidationExtUri = "{CCE6A557-97BC-4b89-ADB6-D9C93CAAB3DF}";
		public const string SparklineGroupsExtUri = "{05C60535-1F16-4fd2-B633-F4F36F0B64E0}";
		public const string PivotDataFieldsExtUri = "{E15A36E0-9728-4e99-A89B-3F7291B0FE68}";
		public const string PivotFieldExtUri = "{2946ED86-A175-432a-8AC1-64E0C546D7DE}";
		public const string PivotTableExtUri = "{962EF5D1-5CA2-4c93-8EF4-DBF5C05439D2}";
		#endregion
		#region Fields
		int imageCounter;
		XlsxPackageBuilder builder;
		Stream outputStream;
		Dictionary<string, string> sheetNamesTable;
		Dictionary<string, string> worksheetRelationPathTable;
		Dictionary<string, string> tableRelationPathTable;
		Dictionary<string, string> drawingRelationPathTable;
		Dictionary<string, string> chartRelationPathTable;
		Dictionary<string, OpenXmlRelationCollection> sheetRelationsTable;
		Dictionary<string, OpenXmlRelationCollection> tableRelationsTable;
		Dictionary<string, OpenXmlRelationCollection> drawingRelationsTable;
		Dictionary<string, OpenXmlRelationCollection> chartRelationsTable;
		OpenXmlRelationCollection currentRelations;
		OpenXmlRelationCollection currentTableRelations;
		Dictionary<ExternalWorkbook, string> externalReferencePathsTable;
		Dictionary<string, string> tablePathsTable;
		Dictionary<int, string> pivotCachesPathsTable;
		Dictionary<int, string> queryTablePathsTable;
		Dictionary<PivotTable, string> pivotTablesPathsTable;
		Dictionary<PivotTable, string> pivotTablesRelationId;
		Dictionary<VmlDrawing, string> vmlDrawingPathsTable;
		Dictionary<Chart, string> chartPathsTable;
		Dictionary<InternalSheetBase, string> drawingPathsTable;
		Dictionary<OfficeImage, string> exportedImages;
		Dictionary<OfficeImage, string> exportedImageTable;
		Dictionary<string, string> exportedExternalImageTable;
		Dictionary<string, string> exportedPictureHyperlinkTable;
		InternalSheetBase activeSheet;
		float activeSheetDefaultColumnWidthInChars;
		ExternalLink activeExternalLink;
		Table activeTable;
		PivotTable activePivotTable;
		PivotCache activePivotCache;
		int sheetCounter;
		int chartsheetCounter;
		int externalReferenceCounter;
		int tableCounter;
		int pivotTableCounter;
		bool shouldExportCalculationChain = true;
		Dictionary<SharedFormula, int> exportedSharedFormulas;
		string currentExternalRelationPath;
		OpenXmlRelation currentExternalReferenceRelation;
		readonly ExportStyleSheet exportStyleSheet;
		#endregion
		public OpenXmlExporter(DocumentModel workbook, OpenXmlDocumentExporterOptions options)
			: base(workbook, options) {
			this.exportStyleSheet = new ExportStyleSheet(Workbook);
		}
		#region Translation tables
		internal static readonly Dictionary<ModelCalculationMode, string> calculationModeTable = CreateCalculationModeTable();
		static Dictionary<ModelCalculationMode, string> CreateCalculationModeTable() {
			Dictionary<ModelCalculationMode, string> result = new Dictionary<ModelCalculationMode, string>();
			result.Add(ModelCalculationMode.Automatic, "auto");
			result.Add(ModelCalculationMode.AutomaticExceptTables, "autoNoTable");
			result.Add(ModelCalculationMode.Manual, "manual");
			return result;
		}
		#endregion
		#region ExportStyleSheet
		protected internal ExportStyleSheet ExportStyleSheet { get { return this.exportStyleSheet; } }
		#endregion
		#region Properties
		public new OpenXmlDocumentExporterOptions Options { get { return (OpenXmlDocumentExporterOptions)base.Options; } }
		protected internal Stream OutputStream { get { return outputStream; } }
		protected internal InternalZipArchive Package { get { return Builder.Package; } }
		protected internal XlsxPackageBuilder Builder { get { return builder; } set { builder = value; } }
		public Dictionary<string, string> SheetNamesTable { get { return sheetNamesTable; } }
		protected internal Worksheet ActiveSheet { get { return activeSheet as Worksheet; } }
		protected internal InternalSheetBase ActiveSheetCore { get { return activeSheet; } }
		protected internal float ActiveSheetDefaultColumnWidthInChars { get { return activeSheetDefaultColumnWidthInChars; } }
		protected internal ExternalLink ActiveExternalLink { get { return activeExternalLink; } }
		protected internal ExternalWorkbook ActiveExternalWorkbook { get { return activeExternalLink.Workbook; } }
		protected internal Table ActiveTable { get { return activeTable; } }
		protected internal PivotTable ActivePivotTable { get { return activePivotTable; } }
		protected internal PivotCache ActivePivotCache { get { return activePivotCache; } }
		protected override string SpreadsheetNamespace { get { return XlsxPackageBuilder.SpreadsheetNamespaceConst; } }
		public Dictionary<SharedFormula, int> ExportedSharedFormulas { get { return exportedSharedFormulas; } }
		internal Dictionary<string, OpenXmlRelationCollection> SheetRelationsTable { get { return sheetRelationsTable; } }
		internal Dictionary<string, OpenXmlRelationCollection> TableRelationsTable { get { return tableRelationsTable; } }
		internal Dictionary<string, OpenXmlRelationCollection> DrawingRelationsTable { get { return drawingRelationsTable; } }
		internal Dictionary<string, OpenXmlRelationCollection> ChartRelationsTable { get { return chartRelationsTable; } }
		public Dictionary<string, string> WorksheetRelationPathTable { get { return worksheetRelationPathTable; } }
		public Dictionary<string, string> TableRelationPathTable { get { return tableRelationPathTable; } }
		public Dictionary<string, string> DrawingRelationPathTable { get { return drawingRelationPathTable; } }
		public Dictionary<string, string> ChartRelationPathTable { get { return chartRelationPathTable; } }
		public Dictionary<int, string> PivotCachesPathsTable { get { return pivotCachesPathsTable; } }
		public Dictionary<int, string> QueryTablePathsTable { get { return queryTablePathsTable; } }
		public Dictionary<PivotTable, string> PivotTablesPathsTable { get { return pivotTablesPathsTable; } }
		public Dictionary<PivotTable, string> PivotTablesRelationId { get { return pivotTablesRelationId; } }
		public Dictionary<VmlDrawing, string> VmlDrawingPathsTable { get { return vmlDrawingPathsTable; } }
		public Dictionary<InternalSheetBase, string> DrawingPathsTable { get { return drawingPathsTable; } }
		public Dictionary<Chart, string> ChartPathsTable { get { return chartPathsTable; } }
		public Dictionary<ExternalWorkbook, string> ExternalReferencePathsTable { get { return externalReferencePathsTable; } }
		public Dictionary<string, string> TablePathsTable { get { return tablePathsTable; } }
		public string CurrentExternalRelationPath { get { return currentExternalRelationPath; } set { currentExternalRelationPath = value; } }
		protected internal int ImageCounter { get { return imageCounter; } set { imageCounter = value; } }
		protected internal Dictionary<OfficeImage, string> ExportedImages { get { return exportedImages; } }
		protected internal Dictionary<OfficeImage, string> ExportedImageTable { get { return exportedImageTable; } }
		protected internal Dictionary<string, string> ExportedExternalImageTable { get { return exportedExternalImageTable; } }
		protected internal Dictionary<string, string> ExportedPictureHyperlinkTable { get { return exportedPictureHyperlinkTable; } }
		protected internal OpenXmlRelationCollection CurrentRelations { get { return currentRelations; } set { currentRelations = value; } }
		#endregion
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			Export();
		}
		public override void Export() {
			if (outputStream == null)
				throw new InvalidOperationException();
			ILogService logService = Workbook.GetService<ILogService>();
			if (logService != null)
				logService.Clear();
			Workbook.DataContext.SetImportExportSettings();
			CreateInternalZipArchive(outputStream);
			Workbook.DataContext.SetWorkbookDefinedSettings();
		}
		protected void CreateInternalZipArchive(Stream outputStream) {
			BeginExport(outputStream);
			try {
				InitializeExport();
				AddPackageContents();
			}
			finally {
				EndExport();
			}
		}
		protected void BeginExport(Stream outputStream) {
			this.builder = CreatePackageBuilder(outputStream);
			Builder.BeginExport();
		}
		protected void EndExport() {
			Builder.EndExport();
		}
		protected virtual XlsxPackageBuilder CreatePackageBuilder(Stream stream) {
			return new XlsxPackageBuilder(stream);
		}
		protected virtual void AddPackageContents() {
			ExportOfficeThemes();
			AddPackageContent(@"xl\styles.xml", ExportStyles());
			AddPackageContent(@"xl\workbook.xml", ExportWorkbook());
			AddDocumentApplicationPropertiesContent();
			AddDocumentCorePropertiesContent();
			AddDocumentCustomPropertiesContent();
			AddSharedStringPackageContent();
			AddSheetsPackageContent();
			AddCalculationChainPackageContent();
			AddExternalReferencesPackageContent();
			AddTablesPackageContent();
			AddQueryTablesPackageContent();
			AddConnectionsPackageContent();
			AddXmlMapsPackageContent();
			AddVmlDrawingPackageContent();
			AddDrawingPackageContent();
			AddChartsPackageContent();
			AddPivotTablesPackageContent();
			AddPivotCachesPackageContent();
			AddVbaProjectContent();
			AddVolatileDependenciesPackageContent();
			AddCustomXmlPackageContent();
			AddPackageContent(@"_rels\.rels", ExportPackageRelations());
			AddPackageContent(@"[Content_Types].xml", ExportContentTypes());
			AddPackageContent(@"xl\_rels\workbook.xml.rels", ExportWorkbookRelations());
			AddWorksheetRelationsPackageContent();
			AddTableRelationsPackageContent();
			AddDrawingRelationsPackageContent();
			AddChartRelationsPackageContent();
		}
		protected void ExportOfficeThemes() {
			currentRelations = new OpenXmlRelationCollection();
			ExportOfficeThemesCore();
		}
		protected internal virtual void ExportOfficeThemesCore() {
			AddPackageContent(@"xl\theme\theme1.xml", ExportThemes());
			AddRelationsPackageContent(OfficeThemeRelationsFileName, currentRelations);
		}
		protected internal virtual void InitializeExport() {
			this.sheetCounter = 0;
			this.chartsheetCounter = 0;
			this.externalReferenceCounter = 0;
			this.tableCounter = 0;
			this.shouldExportCalculationChain = true;
			this.sheetNamesTable = new Dictionary<string, string>();
			this.exportedSharedFormulas = new Dictionary<SharedFormula, int>(GetTotalSharedFormulasCount()); 
			this.externalReferencePathsTable = new Dictionary<ExternalWorkbook, string>();
			this.tablePathsTable = new Dictionary<string, string>();
			this.queryTablePathsTable = new Dictionary<int, string>();
			this.vmlDrawingPathsTable = new Dictionary<VmlDrawing, string>();
			this.chartPathsTable = new Dictionary<Chart, string>();
			this.drawingPathsTable = new Dictionary<InternalSheetBase, string>();
			this.sheetRelationsTable = new Dictionary<string, OpenXmlRelationCollection>();
			this.tableRelationsTable = new Dictionary<string, OpenXmlRelationCollection>();
			this.drawingRelationsTable = new Dictionary<string, OpenXmlRelationCollection>();
			this.chartRelationsTable = new Dictionary<string, OpenXmlRelationCollection>();
			this.worksheetRelationPathTable = new Dictionary<string, string>();
			this.tableRelationPathTable = new Dictionary<string, string>();
			this.drawingRelationPathTable = new Dictionary<string, string>();
			this.chartRelationPathTable = new Dictionary<string, string>();
			this.imageCounter = 0;
			CreateExportedImages();
			CreateExportedImageTable();
			this.exportedExternalImageTable = new Dictionary<string, string>();
			this.exportedPictureHyperlinkTable = new Dictionary<string, string>();
			PopulateUsedContentTypesTable();
			PopulateOverriddenContentTypesTable();
			this.exportStyleSheet.RegisterSharedStrings();
			this.exportStyleSheet.RegisterStyles();
			this.pivotCachesPathsTable = new Dictionary<int, string>();
			this.pivotTablesPathsTable = new Dictionary<PivotTable, string>();
			this.pivotTablesRelationId = new Dictionary<PivotTable, string>();
		}
#region Export relations
		protected void AddRelationsPackageContent(string fileName, OpenXmlRelationCollection collection) {
			if (collection.Count <= 0)
				return;
			currentRelations = collection;
			AddPackageContent(fileName, ExportRelations());
		}
		protected internal virtual CompressedStream ExportRelations() {
			return CreateXmlContent(GenerateRelationsContent);
		}
		void GenerateRelationsContent(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, currentRelations);
		}
#endregion
		protected void CreateExportedImages() {
			this.exportedImages = new Dictionary<OfficeImage, string>();
		}
		protected void CreateExportedImageTable() {
			this.exportedImageTable = new Dictionary<OfficeImage, string>();
		}
		protected internal virtual void PopulateUsedContentTypesTable() {
			Builder.UsedContentTypes.Clear();
			Builder.UsedContentTypes.Add("rels", RelsContentType);
			Builder.UsedContentTypes.Add("xml", XmlContentType);
			if (ShouldAddVmlContentType())
				Builder.UsedContentTypes.Add("vml", VmlDrawingContentType);
		}
		protected internal virtual bool ShouldAddVmlContentType() {
			WorksheetCollection sheets = Workbook.Sheets;
			int count = sheets.Count;
			for (int i = 0; i < count; i++) {
				if (ShouldExportVmlDrawing(sheets[i]))
					return true;
			}
			return false;
		}
		protected internal virtual void PopulateOverriddenContentTypesTable() {
			Dictionary<string, string> overriddenContentTypes = Builder.OverriddenContentTypes;
			overriddenContentTypes.Clear();
			overriddenContentTypes.Add("/xl/workbook.xml", GetMainDocumentContentType());
			overriddenContentTypes.Add("/xl/theme/theme1.xml", ThemeContentType);
			overriddenContentTypes.Add("/xl/styles.xml", XlsxPackageBuilder.StylesContentType);
			if (ShouldExportCalculationChain())
				overriddenContentTypes.Add("/xl/calcChain.xml", CalculationChainContentType);
			if (ShouldExportSharedStrings())
				overriddenContentTypes.Add("/xl/sharedStrings.xml", XlsxPackageBuilder.SharedStringsContentType);
			if (ShouldExportConnections())
				overriddenContentTypes.Add("/xl/connections.xml", ConnectionsContentType);
			if (ShouldExportVbaProject())
				overriddenContentTypes.Add("/xl/vbaProject.bin", VbaProjectContentType);
			if (ShouldExportVolatileDependencies())
				overriddenContentTypes.Add("/xl/volatileDependencies.xml", VolatileDependenciesContentType);
			if (ShouldExportDocumentApplicationProperties())
				overriddenContentTypes.Add("/docProps/app.xml", DocumentApplicationPropertiesContentType);
			if (ShouldExportDocumentCoreProperties())
				overriddenContentTypes.Add("/docProps/core.xml", DocumentCorePropertiesContentType);
			if (ShouldExportDocumentCustomProperties())
				overriddenContentTypes.Add("/docProps/custom.xml", DocumentCustomPropertiesContentType);
		}
		protected internal virtual void AddPackageContent(string fileName, Stream content) {
			if (content != null)
				Package.Add(fileName, Builder.Now, content);
		}
		protected internal virtual void AddPackageContent(string fileName, CompressedStream content) {
			if (content != null)
				Package.AddCompressed(fileName, Builder.Now, content);
		}
#region Calc Properties
		protected internal virtual void GenerateCalcProperties(WorkbookProperties options) {
			WriteShStartElement("calcPr");
			try {
				if (options.UseR1C1ReferenceStyle)
					WriteShStringValue("refMode", "R1C1");
				GenerateCalculationOptions(options.CalculationOptions);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCalculationOptions(CalculationOptions calcOptions) {
			if (calcOptions.IsDefault())
				return;
			if (calcOptions.CalculationMode != ModelCalculationMode.Automatic)
				WriteShStringValue("calcMode", calculationModeTable[calcOptions.CalculationMode]);
			if (!calcOptions.RecalculateBeforeSaving)
				WriteShBoolValue("calcOnSave", calcOptions.RecalculateBeforeSaving);
			if (calcOptions.FullCalculationOnLoad)
				WriteShBoolValue("fullCalcOnLoad", calcOptions.FullCalculationOnLoad);
			if (calcOptions.PrecisionAsDisplayed)
				WriteShBoolValue("fullPrecision", false);
			if (calcOptions.IterationsEnabled)
				WriteShBoolValue("iterate", calcOptions.IterationsEnabled);
			CalculationOptionsInfo defaultItem = Workbook.Cache.CalculationOptionsInfoCache.DefaultItem;
			if (calcOptions.MaximumIterations != defaultItem.MaximumIterations)
				WriteShStringValue("iterateCount", calcOptions.MaximumIterations.ToString(CultureInfo.InvariantCulture));
			if (calcOptions.IterativeCalculationDelta != defaultItem.IterativeCalculationDelta)
				WriteShStringValue("iterateDelta", calcOptions.IterativeCalculationDelta.ToString(CultureInfo.InvariantCulture));
		}
#endregion
		protected internal virtual CompressedStream ExportPackageRelations() {
			return CreateXmlContent(GeneratePackageRelationsContent);
		}
		protected internal virtual void GeneratePackageRelationsContent(XmlWriter writer) {
			OpenXmlRelationCollection relations = new OpenXmlRelationCollection();
			string id = GenerateIdByCollection(relations);
			relations.Add(new OpenXmlRelation(id, "xl/workbook.xml", OfficeDocumentNamespace));
			if (ShouldExportDocumentApplicationProperties()) {
				id = GenerateIdByCollection(relations);
				relations.Add(new OpenXmlRelation(id, "docProps/app.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"));
			}
			if (ShouldExportDocumentCoreProperties()) {
				id = GenerateIdByCollection(relations);
				relations.Add(new OpenXmlRelation(id, "docProps/core.xml", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"));
			}
			if (ShouldExportDocumentCustomProperties()) {
				id = GenerateIdByCollection(relations);
				relations.Add(new OpenXmlRelation(id, "docProps/custom.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties"));
			}
			Builder.GenerateRelationsContent(writer, relations);
		}
		protected internal virtual CompressedStream ExportWorkbookRelations() {
			return CreateXmlContent(GenerateWorkbookRelationsContent);
		}
		protected internal virtual void GenerateWorkbookRelationsContent(XmlWriter writer) {
			OpenXmlRelationCollection workbookRelations = Builder.WorkbookRelations;
			string id = GenerateIdByCollection(workbookRelations);
			workbookRelations.Add(new OpenXmlRelation(id, "styles.xml", RelsStylesNamespace));
			id = GenerateIdByCollection(workbookRelations);
			workbookRelations.Add(new OpenXmlRelation(id, "theme/theme1.xml", RelsThemeNamespace));
			if (ShouldExportCalculationChain()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "calcChain.xml", RelsCalculationChainNamespace));
			}
			if (ShouldExportSharedStrings()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "sharedStrings.xml", RelsSharedStringsNamespace));
			}
			if (ShouldExportConnections()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "connections.xml", RelsConnectionsNamespace));
			}
			if (ShouldExportXmlMaps()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "xmlMaps.xml", RelsXmlMapsNamespace));
			}
			if (ShouldExportVbaProject()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "vbaProject.bin", RelsVbaProjectNamespace));
			}
			if (ShouldExportVolatileDependencies()) {
				id = GenerateIdByCollection(workbookRelations);
				workbookRelations.Add(new OpenXmlRelation(id, "volatileDependencies.xml", RelsVolatileDependenciesNamespace));
			}
			if (ShouldExportCustomXmlPart()) {
				GenerateCustomXmlRelations();
			}
			Builder.GenerateRelationsContent(writer, workbookRelations);
		}
		protected internal virtual string GenerateIdByCollection(OpenXmlRelationCollection collection) {
			return collection.GenerateId();
		}
		protected internal void WriteDifferentialFormatId(string attributeName, int differentialFormatIndex) {
			int differentialFormatId = exportStyleSheet.GetDifferentialFormatId(differentialFormatIndex);
			if (differentialFormatId >= 0)
				WriteIntValue(attributeName, differentialFormatId);
		}
#region External
		protected internal virtual void AddExternalReferencesPackageContent() {
			List<ExternalLink> links = GetExternalLinks();
			int linksCount = links.Count;
			if (linksCount == 0)
				return;
			this.externalReferenceCounter = 0;
			for (int i = 0; i < linksCount; i++) {
				this.activeExternalLink = links[i];
				string path = ExternalReferencePathsTable[ActiveExternalWorkbook];
				AddPackageContent(path, ExportExternalLinkContent());
				if (!String.IsNullOrEmpty(CurrentExternalRelationPath))
					AddPackageContent(CurrentExternalRelationPath, ExportExternalFilePathRelation());
			}
		}
		protected internal virtual CompressedStream ExportExternalLinkContent() {
			return CreateXmlContent(GenerateExternalLinkXmlContent);
		}
		protected internal void GenerateExternalLinkXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateExternalLinkXmlContentCore();
		}
		protected internal virtual void GenerateExternalLinkXmlContentCore() {
			WriteShStartElement("externalLink");
			try {
				GenerateExternalWorkbookXmlContent();
				GenerateExternalDdeConnectionContent(ActiveExternalLink.Workbook as DdeExternalWorkbook);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateExternalWorkbookXmlContent() {
			if (ActiveExternalLink.Workbook is DdeExternalWorkbook)
				return;
			string filePath = ActiveExternalWorkbook.FilePath;
			WriteShStartElement("externalBook");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				string id = InitializeExternalFilePath(filePath);
				WriteStringAttr(RelsPrefix, "id", null, id);
				ExternalWorksheetCollection externalSheets = ActiveExternalWorkbook.Sheets;
				ExportExternalSheetNames(externalSheets);
				ExportExternalDefinedNames(ActiveExternalWorkbook);
				ExportExternalSheets(externalSheets);
			}
			finally {
				WriteShEndElement();
			}
		}
		OpenXmlRelation GetExternalReferenceRelation(string filePath) {
			this.externalReferenceCounter++;
			string id = String.Format("refId{0}", this.externalReferenceCounter);
			if (string.IsNullOrEmpty(filePath))
				return new OpenXmlRelation(id, string.Format("RecoveredExternalLink{0}", this.externalReferenceCounter), RelsExternalLinkMissedPathNamespace, ExternalTargetMode);
			else {
				string relsNamespace = !CheckFileExists(filePath) ? RelsExternalLinkMissedPathNamespace : RelsExternalLinkPathNamespace;
				if ((filePath.IndexOf(@"\\") == 0) || (filePath.IndexOf(@":\") == 1))
					filePath = "file:///" + filePath;
				return new OpenXmlRelation(id, filePath, relsNamespace, ExternalTargetMode);
			}
		}
		bool CheckFileExists(string path) {
			try {
				if ((Path.IsPathRooted(path) && (path.IndexOf(@"\\") == 0)) || (path.IndexOf(@"file:///\\") == 0))
					return true; 
				return File.Exists(path);
			}
			catch { }
			return false;
		}
		protected internal virtual string InitializeExternalFilePath(string filePath) {
			currentExternalReferenceRelation = GetExternalReferenceRelation(filePath);
			string fileName = String.Format("externalLink{0}.xml.rels", this.externalReferenceCounter);
			CurrentExternalRelationPath = @"xl\externalLinks\_rels\" + fileName;
			return currentExternalReferenceRelation.Id;
		}
		protected internal virtual void ExportExternalSheetNames(ExternalWorksheetCollection sheets) {
			int sheetsCount = sheets.Count;
			if (sheetsCount <= 0)
				return;
			WriteShStartElement("sheetNames");
			try {
				for (int i = 0; i < sheetsCount; i++) {
					ExportExternalSheetName(sheets[i]);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalSheetName(ExternalWorksheet externalSheet) {
			WriteElementWithValAttr("sheetName", EncodeXmlChars(externalSheet.Name));
		}
		protected internal virtual void ExportExternalDefinedNames(ExternalWorkbook workbook) {
			GenerateDefinedNamesCore(workbook, ExportExternalDefinedNameCore);
		}
		void ExportExternalDefinedNameCore(DefinedNameBase item, IWorksheet scopeSheet) {
			WriteShStartElement("definedName");
			try {
				WriteShStringValue("name", item.Name);
				string reference = item.GetReference(0, 0);
				if (!string.IsNullOrEmpty(reference)) {
					if (!reference.StartsWith("=", StringComparison.Ordinal))
						reference = String.Concat("=", reference);
					WriteShStringValue("refersTo", reference);
				}
				if (scopeSheet != null) {
					int sheetIndex = scopeSheet.Workbook.GetSheets().IndexOf(scopeSheet);
					WriteShStringValue("sheetId", sheetIndex.ToString());
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalSheets(ExternalWorksheetCollection sheets) {
			int sheetsCount = sheets.Count;
			if (sheetsCount <= 0)
				return;
			WriteShStartElement("sheetDataSet");
			try {
				for (int i = 0; i < sheetsCount; i++) {
					ExportExternalSheet(sheets[i], i, sheets[i].RefreshFailed);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalSheet(ExternalWorksheet externalSheet, int index, bool refreshFailed) {
			WriteShStartElement("sheetData");
			try {
				if (refreshFailed)
					WriteShBoolValue("refreshError", true);
				WriteShIntValue("sheetId", index);
				externalSheet.Rows.ForEach(ExportExternalRow);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalRow(ExternalRow row) {
			WriteShStartElement("row");
			try {
				WriteShIntValue("r", row.Index + 1);
				row.Cells.ForEach(ExportExternalCell);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportExternalCell(ExternalCell cell) {
			WriteShStartElement("cell");
			try {
				ExportCellReference(cell);
				System.Diagnostics.Debug.Assert(cell.Value.Type != VariantValueType.SharedString);
				ExportExternalCellDataType(cell.Value, false);
				ExportExternalCellValue(cell.Value, false);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual CompressedStream ExportExternalFilePathRelation() {
			return CreateXmlContent(GenerateExternalFilePathRelationXmlContent);
		}
		protected internal void GenerateExternalFilePathRelationXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateExternalFilePathRelationXmlContentCore(writer);
		}
		protected internal virtual void GenerateExternalFilePathRelationXmlContentCore(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, currentExternalReferenceRelation);
		}
#endregion
#region VbaProject
		protected internal virtual string GetMainDocumentContentType() {
			return WorkbookContentType;
		}
		protected internal virtual bool ShouldExportVbaProject() {
			return false; 
		}
		protected internal virtual void AddVbaProjectContent() {
			if (!ShouldExportVbaProject())
				return;
			AddPackageContent(@"xl\vbaProject.bin", ExportVbaProject());
		}
		protected internal virtual Stream ExportVbaProject() {
			return null; 
		}
#endregion
		protected internal virtual CompressedStream ExportContentTypes() {
			return CreateXmlContent(GenerateContentTypesContent);
		}
		protected internal virtual void GenerateContentTypesContent(XmlWriter writer) {
			Builder.GenerateContentTypesContent(writer);
		}
		protected internal virtual Stream CreateUncompressedXmlContent(Action<XmlWriter> action) {
			return CreateUncompressedXmlContent(action, CreateXmlWriterSettings());
		}
		protected internal virtual Stream CreateUncompressedXmlContent(Action<XmlWriter> action, XmlWriterSettings xmlSettings) {
			return XmlBasedExporterUtils.Instance.CreateUncompressedXmlContent(action, xmlSettings);
		}
		protected internal virtual CompressedStream CreateXmlContent(Action<XmlWriter> action) {
			return CreateXmlContent(action, CreateXmlWriterSettings());
		}
		protected internal virtual CompressedStream CreateXmlContent(Action<XmlWriter> action, XmlWriterSettings xmlSettings) {
			return XmlBasedExporterUtils.Instance.CreateCompressedXmlContent(action, xmlSettings);
		}
		protected internal override string ConvertBoolToString(bool value) {
			return value ? "1" : "0";
		}
		string EncodeXmlChars(string value) {
			return XmlBasedExporterUtils.Instance.EncodeXmlChars(value);
		}
		string EncodeXmlCharsXML1_0(string value) {
			return XmlBasedExporterUtils.Instance.EncodeXmlCharsXML1_0(value);
		}
		protected internal virtual void SetActiveSheet(Worksheet sheet) {
			SetActiveSheet(sheet as InternalSheetBase);
		}
		protected internal virtual void SetActiveSheet(InternalSheetBase sheet) {
			this.activeSheet = sheet;
		}
		protected internal virtual void SetActivePivotCache(PivotCache cache) {
			this.activePivotCache = cache;
		}
		protected internal virtual void SetActivePivotTable(PivotTable pivotTable) {
			this.activePivotTable = pivotTable;
		}
		int GetTotalSharedFormulasCount() {
			int result = 0;
			foreach (Worksheet sheet in Workbook.Sheets) {
				result += sheet.SharedFormulas.Count;
			}
			return result;
		}
		bool CheckExternalWorksheetSource(PivotCache cache) {
			return cache.Source.Type == PivotCacheType.External;
		}
	}
#endregion
#region XltxExporter
	public class XltxExporter : OpenXmlExporter {
		public XltxExporter(DocumentModel workbook, XltxDocumentExporterOptions options)
			: base(workbook, options) {
		}
		protected internal override string GetMainDocumentContentType() {
			return "application/vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml";
		}
	}
#endregion
}
