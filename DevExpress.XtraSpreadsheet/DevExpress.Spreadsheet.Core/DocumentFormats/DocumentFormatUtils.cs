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
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#else
#endif
namespace DevExpress.XtraSpreadsheet.Import {
	#region CellReferenceStyle (Csv, Text)
	public enum CellReferenceStyle {
		A1 = 0,
		R1C1,
		WorkbookDefined = -1
	}
	#endregion
	#region NewlineType
	public enum NewlineType {
		CrLf = 0,
		Lf,
		Cr,
		LfCr,
		VerticalTab,
		FormFeed,
		MsDos = CrLf,
		Unix = Lf,
		Auto = -1,	 
		AnyCrLf = -2   
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Internal {
	#region SpreadsheetDestinationAndXmlBasedImporter (abstract class)
	public abstract class SpreadsheetDestinationAndXmlBasedImporter : DestinationAndXmlBasedImporter {
		#region Fields
		readonly DocumentImporterOptions options;
		PackageFileCollection packageFiles;
		#endregion
		protected SpreadsheetDestinationAndXmlBasedImporter(IDocumentModel documentModel, DocumentImporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#region Properties
		public PackageFileCollection PackageFiles { get { return packageFiles; } }
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		protected internal DocumentImporterOptions InnerOptions { get { return options; } }
		#endregion
		#region Packages
		protected internal virtual void OpenPackage(Stream stream) {
			this.packageFiles = GetPackageFiles(stream);
		}
		protected internal virtual PackageFileCollection GetPackageFiles(Stream stream) {
			InternalZipFileCollection files = InternalZipArchive.Open(stream);
			int count = files.Count;
			PackageFileCollection result = new PackageFileCollection();
			for (int i = 0; i < count; i++) {
				InternalZipFile file = files[i];
				result.Add(new PackageFile(file.FileName.Replace('\\', '/'), file.FileDataStream, (int)file.UncompressedSize));
			}
			return result;
		}
		protected internal virtual XmlReader GetPackageFileXmlReader(string fileName) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			return CreateXmlReader(stream);
		}
		protected internal virtual XmlReader GetPackageFileXmlReader(string fileName, XmlReaderSettings settings) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			return XmlReader.Create(stream, settings);
		}
		protected internal virtual Stream GetPackageFileStream(string fileName) {
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.Stream;
		}
		protected internal virtual PackageFile GetPackageFile(string fileName) {
			if (PackageFiles == null)
				return null;
			fileName = fileName.Replace('\\', '/');
			if (fileName.StartsWith("/", StringComparison.Ordinal))
				fileName = fileName.Substring(1);
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++) {
				PackageFile file = PackageFiles[i];
				if (String.Compare(file.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return file;
			}
			return null;
		}
		protected internal virtual void DisposePackageFiles() {
			if (PackageFiles == null)
				return;
			int count = PackageFiles.Count;
			for (int i = 0; i < count; i++)
				PackageFiles[i].Stream.Dispose();
			PackageFiles.Clear();
			this.packageFiles = null;
		}
		#endregion
		#region DestinationAndXmlBasedImporter implementation
		public override void BeginSetMainDocumentContent() {
			DocumentModel.BeginSetContent();
		}
		public override void EndSetMainDocumentContent() {
			DocumentModel.PrepareFormulas();
			DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
		}
		public override void SetMainDocumentEmptyContent() {
			if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode)
				DocumentModel.SetMainDocumentEmptyContentCore();
			else {
				DocumentModel.BeginSetContent();
				try {
					DocumentModel.SetMainDocumentEmptyContentCore();
				}
				finally {
					DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
				}
			}
		}
		#endregion
	}
	#endregion
	#region SpreadsheetMLBaseImporter (abstract class)
	public abstract class SpreadsheetMLBaseImporter : SpreadsheetDestinationAndXmlBasedImporter {
		#region Fields
		readonly List<string> relationIdCollection;
		readonly List<string> externalReferenceIdCollection;
		string currentExternalFileRelationId;
		InternalSheetBase currentSheet;
		readonly List<ConditionalFormattingCreatorData> conditionalFormattingCreatorCollection;
		readonly Dictionary<DefinedNameBase, string> definedNameReferences;
		IDrawingObjectsContainer currentDrawingObjectsContainer;
		#endregion
		protected SpreadsheetMLBaseImporter(IDocumentModel documentModel, DocumentImporterOptions options)
			: base(documentModel, options) {
			this.relationIdCollection = new List<string>();
			this.externalReferenceIdCollection = new List<string>();
			this.conditionalFormattingCreatorCollection = new List<ConditionalFormattingCreatorData>();
			this.definedNameReferences = new Dictionary<DefinedNameBase, string>();
		}
		#region Properties
		public abstract string SpreadsheetNamespace { get; }
		public abstract string OfficeNamespace { get; }
		public abstract string ExtendedPropertiesNamespace { get; }
		public abstract string CorePropertiesNamespace { get; }
		public abstract string CustomPropertiesNamespace { get; }
		public List<string> RelationIdCollection { get { return relationIdCollection; } }
		public List<string> ExternalReferenceIdCollection { get { return externalReferenceIdCollection; } }
		protected internal string CurrentExternalFileRelationId { get { return currentExternalFileRelationId; } set { currentExternalFileRelationId = value; } }
		public Worksheet CurrentWorksheet { get { return currentSheet as Worksheet; } set {  currentSheet = value; } }
		public InternalSheetBase CurrentSheet { get { return currentSheet; } set { currentSheet = value; } }
		public IDrawingObjectsContainer CurrentDrawingObjectsContainer { get { return currentDrawingObjectsContainer; } set { currentDrawingObjectsContainer = value; } }
		public List<ConditionalFormattingCreatorData> ConditionalFormattingCreatorCollection { get { return conditionalFormattingCreatorCollection; } }
		public abstract SpreadsheetMLImportStyleSheet StyleSheet { get; }
		public Dictionary<DefinedNameBase, string> DefinedNameReferences { get { return definedNameReferences; } }
		#endregion
		#region Conversion and Parsing utilities
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		protected internal virtual string ReadRelationAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName, RelationsNamespace);
		}
		protected internal Color GetWpSTColorValue(XmlReader reader, string attributeName) {
			return GetWpSTColorValue(reader, attributeName, DXColor.Empty);
		}
		protected internal Color GetWpSTColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName);
			if (!String.IsNullOrEmpty(value))
				return ParseColor(value, defaultValue);
			else
				return defaultValue;
		}
		protected internal Color GetVmlSTColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName);
			if (!String.IsNullOrEmpty(value))
				return ParseVmlSTColor(value, defaultValue);
			else
				return defaultValue;
		}
		protected internal float GetWpSTFloatValue(XmlReader reader, string attributeName) {
			return GetWpSTFloatValue(reader, attributeName, float.MinValue);
		}
		protected internal float GetWpSTFloatValue(XmlReader reader, string attributeName, float defaultValue) {
			return GetWpSTFloatValue(reader, attributeName, NumberStyles.Float, defaultValue);
		}
		protected internal float GetWpSTFloatValue(XmlReader reader, string attributeName, float defaultValue, string ns) {
			return GetWpSTFloatValue(reader, attributeName, NumberStyles.Float, defaultValue, ns);
		}
		protected internal float GetWpSTFloatOrVulgarFractionValue(XmlReader reader, string attributeName, float defaultValue, float denominator) {
			string attr = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(attr))
				return defaultValue;
			return GetFloatOrVulgarFractionValue(attr, defaultValue, denominator);
		}
		protected internal float GetFloatOrVulgarFractionValue(string value, float defaultValue, float denominator) {
			value = value.ToLower();
			if (value.EndsWith("f")) {
				return GetWpSTFloatValue(value.Replace("f", ""), NumberStyles.Float, defaultValue) / denominator;
			}
			return GetWpSTFloatValue(value, NumberStyles.Float, defaultValue);
		}
		[CLSCompliant(false)]
		protected internal UInt16 GetWpSTUnsignedShortHexValue(XmlReader reader, string attributeName, UInt16 defaultValue) {
			string attr = ReadAttribute(reader, attributeName);
			UInt16 result;
			if (!UInt16.TryParse(attr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
				return defaultValue;
			return (UInt16)result;
		}
		protected internal CellRangeBase GetWpSTSqref(XmlReader reader, string attributeName, Worksheet sheet) {
			string attr = ReadAttribute(reader, attributeName);
			if (string.IsNullOrEmpty(attr))
				return null;
			return CellRangeBase.CreateRangeBase(sheet, attr, ' ');
		}
		protected internal string GetWpSTXString(XmlReader reader, string attributeName) {
			string value = reader.GetAttribute(attributeName);
			return value;
		}
		protected internal byte[] GetBase64BinaryOrNull(XmlReader reader, string attributeName) {
			string value = reader.GetAttribute(attributeName);
			if (String.IsNullOrEmpty(value))
				return null;
			return Convert.FromBase64String(value);
		}
		public double? GetDoubleNullableValue(XmlReader reader, string attr) {
			string value = ReadAttribute(reader, attr);
			if (String.IsNullOrEmpty(value))
				return null;
			double result;
			if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				return result;
			else
				return null;
		}
		protected internal virtual float ReadPercentageAttribute(XmlReader reader, string attributeName) {
			string stringValue = reader.GetAttribute(attributeName);
			if (String.IsNullOrEmpty(stringValue))
				return 0;
			stringValue = stringValue.Replace("%", "");
			return GetWpSTFloatValue(stringValue, NumberStyles.Float, 0) / 100;
		}
		protected internal virtual Color ParseColor(string value, Color defaultValue) {
			if (value == "auto")
				return defaultValue;
			if (value[0] == '#')
				value = value.Substring(1, value.Length - 1);
			uint hexColor;
			if ((value.Length == 8 || value.Length == 6) && UInt32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor)) {
				if (value.Length == 6)
					hexColor += 0xFF000000;
				return DXColor.FromArgb((int)hexColor);
			}
			Color nameValue = DXColor.FromName(value);
			if (nameValue != DXColor.Empty)
				return nameValue;
			else
				return defaultValue;
		}
		protected internal virtual Color ParseVmlSTColor(string value, Color defaultValue) {
			if (value == "auto")
				return defaultValue;
			int pos = value.IndexOf(" [");
			if (pos != -1)
				value = value.Remove(pos);
			if (value[0] == '#') {
				value = value.Substring(1, value.Length - 1);
				if (value.Length == 3) {
					value = string.Format("{0}{0}{1}{1}{2}{2}", value[0], value[1], value[2]);
				}
				uint hexColor;
				if ((value.Length == 8 || value.Length == 6) && UInt32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor)) {
					if (value.Length == 6)
						hexColor += 0xFF000000;
					return DXColor.FromArgb((int)hexColor);
				}
			}
			Color nameValue = DXColor.FromName(value);
			if (nameValue != DXColor.Empty)
				return nameValue;
			else
				return defaultValue;
		}
		public CellRange ReadCellRange(XmlReader reader, string attributeName, ICellTable sheet) {
			string reference = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(reference))
				return null;
			return CellRange.Create(sheet, reference);
		}
		public CellPosition ReadCellPosition(XmlReader reader, string attributeName) {
			string reference = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(reference))
				return CellPosition.InvalidValue;
			return CellPosition.TryCreate(reference);
		}
		#endregion
		#region Protection
		protected abstract TranslationTable<HashAlgorithmType> HashAlgorithmTypeTable { get; }
		protected internal ProtectionCredentials ReadProtection(XmlReader reader) {
			ProtectionByPasswordVerifier byPasswordVerifier = ReadProtectionHashPasswordISOHashing(reader, "password");
			CryptographicProtectionInfo byHash = ReadProtectionHashCryptographic(reader, "algorithmName", "hashValue", "saltValue", "spinCount");
			ProtectionCredentials protection = new ProtectionCredentials();
			if (byPasswordVerifier != null)
				protection.RegisterPasswordVerifier(byPasswordVerifier);
			else if (byHash != null)
				protection.RegisterCryptographicProtection(byHash);
			return protection;
		}
		public ProtectionCredentials ReadWorkbookProtection(XmlReader reader) {
			ProtectionByPasswordVerifier passwordVerifier = ReadProtectionHashPasswordISOHashing(reader, "workbookPassword");
			ProtectionByWorkbookRevisions revisionPassword = ReadProtectionRevisionPassword(reader);
			CryptographicProtectionInfo cryptographicHash = ReadProtectionHashCryptographic(reader, "workbookAlgorithmName", "workbookHashValue", "workbookSaltValue", "workbookSpinCount");
			ProtectionCredentials credentials = new ProtectionCredentials();
			if (passwordVerifier != null)
				credentials.RegisterPasswordVerifier(passwordVerifier);
			if (revisionPassword != null)
				credentials.RegisterWorkbookRevisionsProtection(revisionPassword);
			if (cryptographicHash != null)
				credentials.RegisterCryptographicProtection(cryptographicHash);
			return credentials;
		}
		protected internal ProtectionByWorkbookRevisions ReadProtectionRevisionPassword(XmlReader reader) {
			UInt16 value = GetWpSTUnsignedShortHexValue(reader, "revisionsPassword", UInt16.MinValue);
			if (value != UInt16.MinValue)
				return new ProtectionByWorkbookRevisions(value);
			return null;
		}
		protected internal ProtectionByPasswordVerifier ReadProtectionHashPasswordISOHashing(XmlReader reader, string passwordAttrName) {
			UInt16 value = GetWpSTUnsignedShortHexValue(reader, passwordAttrName, UInt16.MinValue);
			if (value != UInt16.MinValue)
				return new ProtectionByPasswordVerifier(value);
			return null;
		}
		protected internal CryptographicProtectionInfo ReadProtectionHashCryptographic(XmlReader reader, string algorithmNameAttrName, string hashValueAttrName, string saltValueAttrName, string spinCountAttrName) {
			string algorithmName = GetWpSTXString(reader, algorithmNameAttrName);
			byte[] hashValue = GetBase64BinaryOrNull(reader, hashValueAttrName);
			byte[] saltValue = GetBase64BinaryOrNull(reader, saltValueAttrName);
			Int32 spinCount = GetWpSTIntegerValue(reader, spinCountAttrName, Int32.MinValue);
			if (String.IsNullOrEmpty(algorithmName) || hashValue == null || saltValue == null
				|| spinCount <= 0)
				return null;
			HashAlgorithmType algorithmType = HashAlgorithmTypeTable.GetEnumValue(algorithmName, HashAlgorithmType.Sha512);
			CryptographicProtectionInfo enforcedProtection = new CryptographicProtectionInfo(hashValue, saltValue, spinCount, algorithmType);
			return enforcedProtection;
		}
		#endregion
		public virtual int RegisterColor(ColorModelInfo colorInfo) {
			return DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
		}
		public abstract void ThrowInvalidFile(string reason);
		protected void ApplyDefinedNameReferences() {
			foreach (DefinedNameBase definedName in DefinedNameReferences.Keys)
				definedName.SetReferenceCore(DefinedNameReferences[definedName]);
		}
	}
	#endregion
	#region ImportExportStyleSheet
	public class ImportExportStyleSheet {
		readonly Dictionary<int, int> fontInfoTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> numberFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> borderInfoTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> cellStyleFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> cellFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> cellStyleIndexTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> differentialFormatTable = new Dictionary<int, int>();
		protected internal Dictionary<int, int> FontInfoTable { get { return fontInfoTable; } }
		protected internal Dictionary<int, int> NumberFormatTable { get { return numberFormatTable; } }
		protected internal Dictionary<int, int> BorderInfoTable { get { return borderInfoTable; } }
		protected internal Dictionary<int, int> CellStyleFormatTable { get { return cellStyleFormatTable; } }
		protected internal Dictionary<int, int> CellFormatTable { get { return cellFormatTable; } }
		protected internal Dictionary<int, int> CellStyleIndexTable { get { return cellStyleIndexTable; } }
		protected internal Dictionary<int, int> DifferentialFormatTable { get { return differentialFormatTable; } }
	}
	#endregion
	#region ExportStyleSheet
	public class ExportStyleSheet : ImportExportStyleSheet {
		#region Fields
		readonly DocumentModel workbook;
		readonly Dictionary<int, int> tableStyleElementFormatTable = new Dictionary<int, int>();
		readonly ExportFillInfoHelper fillInfoHelper = new ExportFillInfoHelper();
		SharedStringIndexTable sharedStringsTable = new SharedStringIndexTable(0);
		ChunkedArray<int> sharedStringsIndicies = new ChunkedArray<int>();
		int sharedStringsTotalRefCount;
		#endregion
		#region Properties
		protected internal SharedStringIndexTable SharedStringsTable { get { return sharedStringsTable; } }
		protected internal ChunkedArray<int> SharedStringsIndicies { get { return sharedStringsIndicies; } }
		protected internal int SharedStringsTotalRefCount { get { return sharedStringsTotalRefCount; } }
		protected internal Dictionary<int, int> TableStyleElementFormatTable { get { return tableStyleElementFormatTable; } }
		protected internal DocumentModel Workbook { get { return workbook; } }
		protected internal ExportFillInfoHelper FillInfoHelper { get { return fillInfoHelper; } }
		protected internal virtual int MaxNumberFormatIndex { get { return 369; } } 
		#endregion
		public ExportStyleSheet(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			InitializeInfoTables(workbook.StyleSheet);
		}
		void InitializeInfoTables(StyleSheet styleSheet) {
			BuiltInCellStyle normalStyle = (BuiltInCellStyle)styleSheet.CellStyles.Normal;
			NumberFormatTable.Add(normalStyle.FormatInfo.NumberFormatIndex, 0);
			FontInfoTable.Add(normalStyle.FormatInfo.FontIndex, 0);
			FillInfoHelper.RegisterPatternFillIndex(0);
			FillInfoHelper.RegisterPatternFillIndex(1);
			FillInfoHelper.RegisterFill(normalStyle.FormatInfo);
			BorderInfoTable.Add(0, 0);
			RegisterItem(BorderInfoTable, normalStyle.FormatInfo.BorderIndex);
			CellFormatTable.Add(styleSheet.DefaultCellFormatIndex, 0);
			CellStyleIndexTable.Add(0, 0);
		}
		public void RegisterSharedStrings() {
			this.sharedStringsTable = new SharedStringIndexTable(workbook.SharedStringTable.Count); 
			this.sharedStringsIndicies = new ChunkedArray<int>(8192, workbook.SharedStringTable.Count); 
			sharedStringsTotalRefCount = 0;
			for (int i = 0; i < workbook.Sheets.Count; i++) {
				Worksheet sheet = workbook.Sheets[i];
#if DATA_SHEET
				if (!sheet.IsDataSheet)
#endif
					RegisterSharedStrings(sheet);
			}
		}
#if BTREE
		protected internal void RegisterSharedStrings(Worksheet sheet) {
			foreach (ICell cell in sheet.Cells) {
				if (cell.Value.IsSharedString) {
					int index = cell.Value.SharedStringIndexValue.ToInt();
					if (!SharedStringsTable.Contains(index)) {
						SharedStringsTable[index] = SharedStringsIndicies.Count;
						SharedStringsIndicies.Add(index);
					}
					sharedStringsTotalRefCount++;
				}
			}
		}
#else
		protected internal void RegisterSharedStrings(Worksheet sheet) {
			CellRange range = GetMaximumCellRange(sheet);
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell != null && cell.Value.IsSharedString) {
					int index = cell.Value.SharedStringIndexValue.ToInt();
					if (!SharedStringsTable.Contains(index)) {
						SharedStringsTable[index] = SharedStringsIndicies.Count;
						SharedStringsIndicies.Add(index);
					}
					sharedStringsTotalRefCount++;
				}
			}
		}
#endif
		protected internal virtual CellRange GetMaximumCellRange(IWorksheet sheet) {
			CellPosition topLeft = new CellPosition(0, 0);
			CellPosition bottomRight = new CellPosition(IndicesChecker.MaxColumnCount - 1, IndicesChecker.MaxRowCount - 1);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		public virtual void RegisterStyles() {
			RegisterCellStylesAndFormats();
			RegisterNumberFormats();
			RegisterFonts();
			RegisterFills();
			RegisterBorders();
		}
		protected void RegisterItem<T>(Dictionary<T, int> collection, T key) {
			if (!collection.ContainsKey(key))
				collection.Add(key, collection.Count);
		}
		#region RegisterCellStylesAndFormats
		protected internal virtual void RegisterCellStylesAndFormats() {
			RegisterDefaultCellStyle();
			RegisterFormats();
			RegisterReferencedCellStyle();
			RegisterNotHiddenCustomCellStyle();
			RegisterHiddenOrCustomBuiltInStyles();
			RegisterCellStyleFormats();
			workbook.StyleSheet.TableStyles.ForEach(RegisterTableStyleElementFormatIndex);
		}
		protected internal virtual void RegisterDefaultCellStyle() {
		}
		#region RegisterFormats
		protected internal virtual void RegisterFormats() {
			for (int i = 0; i < workbook.Sheets.Count; i++) { 
				Worksheet sheet = workbook.Sheets[i];
#if !BTREE
#if DATA_SHEET
					if (!sheet.IsDataSheet)
#endif
						sheet.Rows.ForEach(RegisterRowCellFormatIndex);
#endif
			}
			for (int i = 0; i < workbook.Sheets.Count; i++) {
				Worksheet sheet = workbook.Sheets[i];
				sheet.Columns.ForEach(RegisterColumnCellFormatIndex);
#if DATA_SHEET
				if (!sheet.IsDataSheet) {
					RegisterCellFormatIndex(sheet);
					RegisterCellFormatIndex(sheet.Tables);
					RegisterDifferentialFormatIndex(sheet);
					RegisterPivotNumberFormatIndexes(sheet.PivotTables);
				}
#else
				RegisterCellFormatIndex(sheet);
				RegisterCellFormatIndex(sheet.Tables);
				RegisterDifferentialFormatIndex(sheet);
#endif
			}
		}
		public void RegisterPivotNumberFormatIndexes(PivotTableCollection pivotTables) {
			for (int i = 0; i < pivotTables.Count; i++) {
				PivotTable table = pivotTables[i];
				table.Fields.ForEach(RegisterPivotFieldNumberFormatIndex);
				table.DataFields.ForEach(RegisterPivotDataFieldNumberFormatIndex);
				if (table.Cache != null && table.Cache.CacheFields != null)
					table.Cache.CacheFields.ForEach(RegisterPivotCacheFieldNumberFormatIndex);
			}
		}
		void RegisterPivotFieldNumberFormatIndex(PivotField field) {
			if (field.NumberFormatIndex > 0)
				RegisterNumberFormatCore(field.NumberFormatIndex);
		}
		void RegisterPivotDataFieldNumberFormatIndex(PivotDataField dataField) {
			if (dataField.NumberFormatIndex > 0)
				RegisterNumberFormatCore(dataField.NumberFormatIndex);
		}
		void RegisterPivotCacheFieldNumberFormatIndex(IPivotCacheField cacheField) {
			if (cacheField.NumberFormatIndex > 0) 
				RegisterNumberFormatCore(cacheField.NumberFormatIndex);
		}
		void RegisterRowCellFormatIndex(Row row) {
			RegisterCellFormatIndex(row.FormatIndex);
		}
		void RegisterColumnCellFormatIndex(IColumnRange column) {
			RegisterCellFormatIndex(column.FormatIndex);
		}
		void RegisterCellFormatIndex(int index) {
			if (index >= workbook.StyleSheet.DefaultCellFormatIndex)
				RegisterItem(CellFormatTable, index);
		}
		#region RegisterCellFormatIndex
#if BTREE
		void RegisterCellFormatIndex(Worksheet sheet) {
			foreach (ICell cell in sheet.Cells) {
				RegisterCellFormatIndex(cell.FormatIndex);
			}
		}
#else
		void RegisterCellFormatIndex(Worksheet sheet) {
			CellRange range = GetMaximumCellRange(sheet);
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell != null)
					RegisterCellFormatIndex(cell.FormatIndex);
			}
		}
#endif
		void RegisterCellFormatIndex(TableCollection tables) {
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = tables[i];
				for (int j = 0; j < Table.CellFormatElementCount; j++)
					RegisterCellFormatIndex(table.CellFormatIndexes[j]);
				RegisterCellFormatIndex(table.Columns);
			}
		}
		void RegisterCellFormatIndex(TableColumnInfoCollection columns) {
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				TableColumn column = columns[i];
				for (int j = 0; j < TableColumn.ElementCount; j++)
					RegisterCellFormatIndex(column.CellFormatIndexes[j]);
			}
		}
		#endregion
		#region RegisterDifferentialFormatIndex
		protected internal virtual void RegisterDifferentialFormatIndex(Worksheet sheet) {
			RegisterDifferentialFormatIndex(sheet.Tables);
			RegisterDifferentialFormatIndex(sheet.AutoFilter);
			RegisterDifferentialFormatIndex(sheet.ConditionalFormattings);
			RegisterDifferentialFormatIndex(sheet.PivotTables);
		}
		protected internal void RegisterDifferentialFormatIndex(TableCollection tables) {
			for (int i = 0; i < tables.Count; i++) {
				Table table = tables[i];
				RegisterDifferentialFormatIndex(table);
				RegisterDifferentialFormatIndex(table.Columns);
				RegisterDifferentialFormatIndex(table.AutoFilter);
			}
		}
		protected internal void RegisterDifferentialFormatIndex(Table table) {
			for (int i = 0; i < Table.DifferentialFormatElementCount; i++) {
				RegisterDifferentialFormatIndex(DifferentialFormatTable, table.DifferentialFormatIndexes[i]);
				RegisterDifferentialFormatIndex(DifferentialFormatTable, table.BorderFormatIndexes[i]);
			}
		}
		protected internal void RegisterDifferentialFormatIndex(TableColumnInfoCollection columns) {
			for (int i = 0; i < columns.Count; i++)
				RegisterDifferentialFormatIndex(columns[i]);
		}
		protected internal void RegisterDifferentialFormatIndex(TableColumn column) {
			for (int i = 0; i < TableColumn.ElementCount; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, column.DifferentialFormatIndexes[i]);
		}
		protected internal void RegisterDifferentialFormatIndex(ConditionalFormattingCollection conditionalFormattings) {
			for (int i = 0; i < conditionalFormattings.Count; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, conditionalFormattings[i].DifferentialFormatIndex);
		}
		protected internal void RegisterDifferentialFormatIndex(PivotTableCollection tables) {
			foreach (PivotTable table in tables)
				foreach (PivotFormat format in table.Formats)
					RegisterDifferentialFormatIndex(DifferentialFormatTable, format.Index);
		}
		protected internal void RegisterDifferentialFormatIndex(AutoFilterBase autoFilter) {
			AutoFilterColumnCollection filterColumns = autoFilter.FilterColumns;
			int count = filterColumns.Count;
			for (int i = 0; i < count; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, filterColumns[i].FormatIndex);
		}
		void RegisterDifferentialFormatIndex(Dictionary<int, int> collection, int index) {
			if (CellFormatCache.DefaultDifferentialFormatIndex != index)
				RegisterItem(collection, index);
		}
		#endregion
		#endregion
		#region RegisterReferencedCellStyle
		protected internal void RegisterReferencedCellStyle() {
			foreach (int index in CellFormatTable.Keys) {
				CellFormat cellFormat = (CellFormat)workbook.Cache.CellFormatCache[index];
				RegisterItem(CellStyleIndexTable, cellFormat.Style.StyleIndex);
			}
		}
		#endregion
		#region RegisterNotHiddenCustomCellStyle
		protected internal void RegisterNotHiddenCustomCellStyle() {
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				CustomCellStyle cellStyle = cellStyles[i] as CustomCellStyle;
				if (cellStyle != null && !cellStyle.IsHidden)
					RegisterItem(CellStyleIndexTable, cellStyle.StyleIndex);
			}
		}
		#endregion
		#region RegisterHiddenOrCustomBuiltInStyles
		protected internal void RegisterHiddenOrCustomBuiltInStyles() {
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				CellStyleBase cellStyle = cellStyles[i];
				if (IsCustomOrHiddenBuiltInCellStyle(cellStyle))
					RegisterItem(CellStyleIndexTable, cellStyle.StyleIndex);
			}
		}
		bool IsCustomOrHiddenBuiltInCellStyle(CellStyleBase cellStyle) {
			BuiltInCellStyle builtInCellStyle = cellStyle as BuiltInCellStyle;
			if (builtInCellStyle != null)
				return builtInCellStyle.CustomBuiltIn || builtInCellStyle.IsHidden;
			OutlineCellStyle outlineCellStyle = cellStyle as OutlineCellStyle;
			if (outlineCellStyle != null)
				return outlineCellStyle.CustomBuiltIn || outlineCellStyle.IsHidden;
			return false;
		}
		#endregion
		#region RegisterCellStyleFormats
		protected internal virtual void RegisterCellStyleFormats() {
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			foreach (int index in CellStyleIndexTable.Keys)
				RegisterItem(CellStyleFormatTable, cellStyles[index].FormatIndex);
		}
		#endregion
		#region RegisterDifferentialFormatForCustomTableStyles
		protected internal void RegisterTableStyleElementFormatIndex(TableStyle style) {
			if (!style.IsPredefined)
				for (int i = 0; i < TableStyle.ElementsCount; i++) {
					RegisterDifferentialFormatIndex(DifferentialFormatTable, style.GetElementFormat(i).DifferentialFormatIndex);
					RegisterTableStyleElementFormatIndex(TableStyleElementFormatTable, style.FormatIndexes[i]);
				}
		}
		void RegisterTableStyleElementFormatIndex(Dictionary<int, int> collection, int index) {
			if (TableStyleElementFormatCache.DefaultItemIndex != index)
				RegisterItem(collection, index);
		}
		#endregion
		#endregion
		#region RegisterNumberFormats
		protected internal virtual void RegisterNumberFormats() {
			RegisterNumberFormat(CellFormatTable);
			RegisterNumberFormat(CellStyleFormatTable);
		}
		void RegisterNumberFormat(Dictionary<int, int> collection) {
			foreach (int index in collection.Keys)
				RegisterNumberFormatCore(workbook.Cache.CellFormatCache[index].NumberFormatIndex);
		}
		void RegisterNumberFormatCore(int index) {
			RegisterItem(NumberFormatTable, index);
		}
		#endregion
		#region RegisterFonts
		protected internal virtual void RegisterFonts() {
			CellFormatCache cache = workbook.Cache.CellFormatCache;
			int normalStyleFormatIndex = workbook.StyleSheet.CellStyles.Normal.FormatIndex;
			int defaultCellFormatIndex = workbook.StyleSheet.DefaultCellFormatIndex;
			RegisterFont(CellFormatTable, cache[defaultCellFormatIndex].FontIndex == cache[normalStyleFormatIndex].FontIndex);
			RegisterFont(CellStyleFormatTable, normalStyleFormatIndex == CellFormatCache.DefaultCellStyleFormatIndex);
		}
		void RegisterFont(Dictionary<int, int> table, bool shouldRegisteredDefaultCacheIndex) {
			foreach (int index in table.Keys) {
				int fontIndex = workbook.Cache.CellFormatCache[index].FontIndex;
				if (fontIndex != RunFontInfoCache.DefaultItemIndex || shouldRegisteredDefaultCacheIndex)
					RegisterItem(FontInfoTable, fontIndex);
			}
		}
		#endregion
		#region RegisterFills
		protected internal void RegisterFills() {
			RegisterFill(CellFormatTable);
			RegisterFill(CellStyleFormatTable);
		}
		void RegisterFill(Dictionary<int, int> collection) {
			foreach (int index in collection.Keys)
				FillInfoHelper.RegisterFill(workbook.Cache.CellFormatCache[index]);
		}
		#endregion
		#region RegisterBorders
		protected internal void RegisterBorders() {
			RegisterBorder(CellFormatTable);
			RegisterBorder(CellStyleFormatTable);
		}
		void RegisterBorder(Dictionary<int, int> collection) {
			foreach (int index in collection.Keys)
				RegisterItem(BorderInfoTable, workbook.Cache.CellFormatCache[index].BorderIndex);
		}
		#endregion
		public List<int> GetCustomNumberFormats() {
			bool shouldWriteLogOnWarning = true;
			List<int> result = new List<int>(NumberFormatTable.Count);
			foreach (int index in NumberFormatTable.Keys) {
				if (index > MaxNumberFormatIndex) {
					if (shouldWriteLogOnWarning) {
						Workbook.LogMessageFormat(LogCategory.Warning, XtraSpreadsheetStringId.Msg_NumberFormatRecordsSkipped, MaxNumberFormatIndex + 1);
						shouldWriteLogOnWarning = false;
					}
					continue;
				}
				bool isVolatile = (index >= 5 && index <= 8) || (index >= 23 && index <= 26) || (index >= 41 && index <= 44) || (index >= 63 && index <= 66) || (index >= 164);
				if (isVolatile)
					result.Add(index);
			}
			return result;
		}
		public int GetNumberFormatId(int index) {
			if (index <= MaxNumberFormatIndex)
				return index;
			return NumberFormatCollection.DefaultItemIndex;
		}
		protected internal int GetDifferentialFormatId(int differentialFormatIndex) {
			if (differentialFormatIndex == CellFormatCache.DefaultDifferentialFormatIndex)
				return Int32.MinValue;
			if (!DifferentialFormatTable.ContainsKey(differentialFormatIndex))
				Exceptions.ThrowInternalException();
			return DifferentialFormatTable[differentialFormatIndex];
		}
		protected internal IList<ImportExportTableStyleInfo> GetCustomTableStyleInfoes(TableStyleCollection tableStyles) {
			IList<ImportExportTableStyleInfo> result = new List<ImportExportTableStyleInfo>();
			foreach (TableStyle style in tableStyles) {
				if (!style.IsPredefined)
					result.Add(ImportExportTableStyleInfo.CreateFrom(style, this));
			}
			return result;
		}
	}
	#endregion
	#region SpreadsheetMLImportStyleSheet
	public class SpreadsheetMLImportStyleSheet : ImportExportStyleSheet {
		int fontCount;
		int borderCount;
		int fillCount;
		int cellFormatCount;
		int cellStyleFormatCount;
		readonly DocumentModelImporter importer;
		readonly Dictionary<int, int> fillInfoTable;
		readonly Dictionary<int, GradientStopInfoCollection> gradientStopInfoTable;
		bool shouldAddNormalStyle;
		public SpreadsheetMLImportStyleSheet(DocumentModelImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
			this.shouldAddNormalStyle = true;
			this.fillInfoTable = new Dictionary<int, int>();
			this.gradientStopInfoTable = new Dictionary<int, GradientStopInfoCollection>();
			CellStyleIndexTable.Add(0, 0);
		}
		public DocumentModelImporter Importer { get { return importer; } }
		public DocumentModel DocumentModel { get { return (DocumentModel)importer.DocumentModel; } }
		public virtual bool ShouldAddNormalStyle { get { return shouldAddNormalStyle; } set { shouldAddNormalStyle = value; } }
		protected internal CellStyleCollection CellStyles { get { return DocumentModel.StyleSheet.CellStyles; } }
		protected internal Dictionary<int, int> FillInfoTable { get { return fillInfoTable; } }
		protected internal Dictionary<int, GradientStopInfoCollection> GradientStopInfoTable { get { return gradientStopInfoTable; } }
		public int GetNumberFormatIndex(int documentIndex) {
			int modelIndex;
			if (!NumberFormatTable.TryGetValue(documentIndex, out modelIndex)) {
				if (documentIndex >= 0 && documentIndex < DocumentModel.Cache.NumberFormatCache.DefaultItemCount)
					modelIndex = documentIndex;
				else {
					modelIndex = 0;
				}
			}
			return modelIndex;
		}
		public void RegisterNumberFormat(int id, string formatCode) {
			NumberFormat info = NumberFormatParser.Parse(formatCode);
			int index = DocumentModel.Cache.NumberFormatCache.GetItemIndex(info);
			NumberFormatTable.Add(id, index);
		}
		public virtual void RegisterFont(RunFontInfo info) {
			int index = DocumentModel.Cache.FontInfoCache.GetItemIndex(info);
			FontInfoTable.Add(fontCount, index);
			fontCount++;
		}
		public void RegisterBorder(BorderInfo info) {
			int index = DocumentModel.Cache.BorderInfoCache.GetItemIndex(info);
			BorderInfoTable.Add(borderCount, index);
			borderCount++;
		}
		public void RegisterPatternFill(FillInfo info) {
			int index = DocumentModel.Cache.FillInfoCache.GetItemIndex(info);
			FillInfoTable.Add(fillCount, index);
			fillCount++;
		}
		public void RegisterGradientFill(GradientFillInfo info, GradientStopInfoCollection stops) {
			int index = DocumentModel.Cache.GradientFillInfoCache.GetItemIndex(info);
			FillInfoTable.Add(fillCount, index);
			GradientStopInfoTable.Add(fillCount, stops);
			fillCount++;
		}
		public virtual int RegisterCellStyleFormat(ImportCellFormatInfo info) {
			CellStyleFormat cellStyleFormat = new CellStyleFormat(DocumentModel);
			InitializeFormat(cellStyleFormat, info);
			CellFormatCache cache = DocumentModel.Cache.CellFormatCache;
			int index = cache.GetItemIndex(cellStyleFormat);
			CellStyleFormatTable.Add(cellStyleFormatCount, index);
			cellStyleFormatCount++;
			return index;
		}
		public virtual int RegisterCellFormat(ImportCellFormatInfo info) {
			CellFormat cellFormat = new CellFormat(DocumentModel);
			if (info.StyleId == ImportCellFormatInfo.DefaultStyleId)
				PrepareCellFormatWithoutStyleId(cellFormat, info);
			else
				PrepareCellFormatWithStyleId(cellFormat, info);
			int index = DocumentModel.Cache.CellFormatCache.GetItemIndex(cellFormat);
			CellFormatTable.Add(cellFormatCount, index);
			if (cellFormatCount == 0)
				DocumentModel.StyleSheet.DefaultCellFormatIndex = index;
			cellFormatCount++;
			return index;
		}
		void PrepareCellFormatWithoutStyleId(CellFormat cellFormat, ImportCellFormatInfo info) {
			InitializeFormatWithoutFlags(cellFormat, info);
			info.FormatFlagsInfo = GetCellFormatFlagsInfoForMissingStyleId(info);
			AssignCellFormatFlagsIndex(cellFormat, info);
		}
		void PrepareCellFormatWithStyleId(CellFormat cellFormat, ImportCellFormatInfo info) {
			if (!CellStyleFormatTable.ContainsKey(info.StyleId))
				Importer.ThrowInvalidFile();
			InitializeFormat(cellFormat, info);
			cellFormat.AssignStyleIndex(info.StyleId);
		}
		CellFormatFlagsInfo GetCellFormatFlagsInfoForMissingStyleId(ImportCellFormatInfo info) {
			CellFormatFlagsInfo result = info.FormatFlagsInfo.Clone();
			result.ApplyNumberFormat = info.NumberFormatId != 0;
			result.ApplyFont = info.FontId != 0;
			result.ApplyFill = info.FillId != 0;
			result.ApplyBorder = info.BorderId != 0;
			return result;
		}
		public virtual int RegisterCellStyle(ImportCellStyleInfo info) {
			CellStyleBase cellStyle = CreateCellStyle(info);
			bool isDefaultCellStyle = info.BuiltInId == 0 && !info.IsHidden;
			if (isDefaultCellStyle) {
				ReplaceNormalStyle(info);
				return 0;
			}
			CellStyles.Add(cellStyle);
			int importCellStyleFormatIndex = info.StyleFormatId;
			if (!CellStyleIndexTable.ContainsKey(importCellStyleFormatIndex))
				CellStyleIndexTable.Add(importCellStyleFormatIndex, cellStyle.StyleIndex);
			return cellStyle.StyleIndex;
		}
		protected void ReplaceNormalStyle(ImportCellStyleInfo info) {
			int formatIndex = CellStyleFormatTable[info.StyleFormatId];
			BuiltInCellStyle normalStyle = (BuiltInCellStyle)CellStyles[0];
			normalStyle.AssignCellStyleFormatIndex(formatIndex);
			normalStyle.SetCustomBuiltInCore(info.CustomBuiltIn);
			normalStyle.AssignStyleIndex(0);
			ShouldAddNormalStyle = false;
		}
		public int RegisterCellAlignment(CellAlignmentInfo info) {
			return DocumentModel.Cache.CellAlignmentInfoCache.GetItemIndex(info);
		}
		protected internal void InitializeFormat(FormatBase format, ImportCellFormatInfo info) {
			InitializeFormatWithoutFlags(format, info);
			AssignCellFormatFlagsIndex(format, info);
		}
		void AssignCellFormatFlagsIndex(FormatBase format, ImportCellFormatInfo info) {
			CellFormatFlagsInfo flagsInfo = info.FormatFlagsInfo.Clone();
			flagsInfo.FillType = format.Fill.FillType;
			format.AssignCellFormatFlagsIndex(flagsInfo.PackedValues);
		}
		void SetFillIndex(FormatBase format, int fillId) {
			int fillIndex;
			if (FillInfoTable.TryGetValue(fillId, out fillIndex)) {
				if (GradientStopInfoTable.ContainsKey(fillId)) {
					format.AssignCellFormatFlagsIndex(format.CellFormatFlagsIndex + CellFormatFlagsInfo.MaskFillType);
					format.AssignGradientFillInfoIndex(fillIndex);
					format.GradientStopInfoCollection.CopyFrom(GradientStopInfoTable[fillId]);
				}
				else
					format.AssignFillIndex(fillIndex);
				return;
			}
			Importer.ThrowInvalidFile();
		}
		void SetInfoIndex(Dictionary<int, int> table, int id, Action<int> assignIndex) {
			int infoIndex;
			if (!table.TryGetValue(id, out infoIndex))
				Importer.ThrowInvalidFile();
			assignIndex(infoIndex);
		}
		void SetNumberFormatIndex(FormatBase format, int numberFormatId) {
			int index = GetNumberFormatIndex(numberFormatId);
			format.AssignNumberFormatIndex(index);
		}
		void InitializeFormatWithoutFlags(FormatBase format, ImportCellFormatInfo info) {
			SetInfoIndex(FontInfoTable, info.FontId, format.AssignFontIndex);
			SetFillIndex(format, info.FillId);
			SetInfoIndex(BorderInfoTable, info.BorderId, format.AssignBorderIndex);
			SetNumberFormatIndex(format, info.NumberFormatId);
			format.AssignAlignmentIndex(info.AlignmentIndex);
		}
		protected CellStyleBase CreateCellStyle(ImportCellStyleInfo info) {
			int styleFormatId = info.StyleFormatId;
			if (!CellStyleFormatTable.ContainsKey(styleFormatId))
				Importer.ThrowInvalidFile();
			int formatIndex = CellStyleFormatTable[styleFormatId];
			CellStyleFormat styleFormat = (CellStyleFormat)DocumentModel.Cache.CellFormatCache[formatIndex];
			CellStyleBase result;
			int outlineLevel = info.OutlineLevel;
			int builtInId = info.BuiltInId;
			if (BuiltInCellStyleCalculator.IsValidateOutlineBuildInId(builtInId)) {
				if (outlineLevel <= 0)
					Importer.ThrowInvalidFile();
				result = new OutlineCellStyle(DocumentModel, builtInId == 1, outlineLevel, styleFormat);
				((OutlineCellStyle)result).CustomBuiltIn = info.CustomBuiltIn;
			}
			else if (builtInId != Int32.MinValue) {
				if (!BuiltInCellStyleCalculator.IsValidateBuiltInId(builtInId))
					Importer.ThrowInvalidFile();
				result = new BuiltInCellStyle(DocumentModel, builtInId, styleFormat);
				((BuiltInCellStyle)result).SetCustomBuiltInCore(info.CustomBuiltIn);
			}
			else {
				string name = info.Name;
				if (String.IsNullOrEmpty(name))
					name = CalculateCustomName();
				result = new CustomCellStyle(DocumentModel, name, styleFormat);
			}
			bool isHidden = info.IsHidden;
			if (isHidden)
				result.SetHiddenCore(isHidden);
			return result;
		}
		protected internal void Update() {
			if (shouldAddNormalStyle)
				UpdateDefaultCellStyleFormat(CellStyles[0].FormatInfo);
			UpdateCellFormats();
		}
		void UpdateDefaultCellStyleFormat(CellStyleFormat cellStyleFormat) {
			cellStyleFormat.AssignNumberFormatIndex(NumberFormatCollection.DefaultItemIndex);
			cellStyleFormat.AssignFontIndex(FontInfoTable[0]);
			if (cellStyleFormat.Fill.FillType == ModelFillType.Pattern)
				cellStyleFormat.AssignFillIndex(FillInfoCache.DefaultItemIndex);
			else
				cellStyleFormat.AssignGradientFillInfoIndex(GradientFillInfoCache.DefaultItemIndex);
			cellStyleFormat.AssignBorderIndex(BorderInfoCache.DefaultItemIndex);
			cellStyleFormat.AssignAlignmentIndex(CellAlignmentInfoCache.DefaultItemIndex);
			cellStyleFormat.AssignCellFormatFlagsIndex(DocumentModel.Cache.CellFormatCache.DefaultCellStyleFormatItem.CellFormatFlagsIndex);
		}
		void UpdateCellFormats() {
			CellFormatCache formatCache = DocumentModel.Cache.CellFormatCache;
			int count = formatCache.Count;
			int defaultCellFormatIndex = DocumentModel.StyleSheet.DefaultCellFormatIndex;
			for (int i = defaultCellFormatIndex; i < count; i++)
				UpdateCellFormat(i);
		}
		int GetStyleIndex(int styleIndex) {
			if (CellStyleIndexTable.ContainsKey(styleIndex))
				return CellStyleIndexTable[styleIndex];
			return GetFakeCellStyle(styleIndex).StyleIndex;
		}
		CellStyleBase GetFakeCellStyle(int styleIndex) {
			int cellStyleFormatIndex = CellStyleFormatTable[styleIndex];
			CellStyleFormat styleFormat = (CellStyleFormat)DocumentModel.Cache.CellFormatCache[cellStyleFormatIndex];
			StyleSheet styleSheet = DocumentModel.StyleSheet;
			CustomCellStyle result = new CustomCellStyle(DocumentModel, styleSheet.GenerateCustomStyleName(), styleFormat);
			styleSheet.CellStyles.Add(result);
			return result;
		}
		void UpdateCellFormat(int cellFormatIndex) {
			CellFormat cellFormat = DocumentModel.Cache.CellFormatCache[cellFormatIndex] as CellFormat;
			if (cellFormat == null)
				return;
			if (shouldAddNormalStyle) {
				CellStyleFormat normalFormat = CellStyles[0].FormatInfo;
				if (cellFormatIndex == DocumentModel.StyleSheet.DefaultCellFormatIndex)
					UpdateDefaultCellFormat(cellFormat, normalFormat);
				UpdateNormalIndexes(cellFormat, normalFormat);
			}
			cellFormat.AssignStyleIndex(GetStyleIndex(cellFormat.StyleIndex));
			CellStyleFormat styleFormat = cellFormat.Style.FormatInfo;
			CellFormatFlagsInfo cloneFlagsInfo = cellFormat.CellFormatFlagsInfo.Clone();
			if (!cloneFlagsInfo.ApplyNumberFormat)
				cloneFlagsInfo.ApplyNumberFormat = styleFormat.NumberFormatIndex != cellFormat.NumberFormatIndex;
			if (!cloneFlagsInfo.ApplyFont)
				cloneFlagsInfo.ApplyFont = styleFormat.FontIndex != cellFormat.FontIndex;
			if (!cloneFlagsInfo.ApplyFill && cloneFlagsInfo.FillType == ModelFillType.Pattern)
				cloneFlagsInfo.ApplyFill = styleFormat.FillIndex != cellFormat.FillIndex;
			if (!cloneFlagsInfo.ApplyFill && cloneFlagsInfo.FillType == ModelFillType.Gradient)
				cloneFlagsInfo.ApplyFill = styleFormat.GradientFillInfoIndex != cellFormat.GradientFillInfoIndex;
			if (!cloneFlagsInfo.ApplyBorder)
				cloneFlagsInfo.ApplyBorder = styleFormat.BorderIndex != cellFormat.BorderIndex;
			if (!cloneFlagsInfo.ApplyAlignment)
				cloneFlagsInfo.ApplyAlignment = styleFormat.AlignmentIndex != cellFormat.AlignmentIndex;
			if (!cloneFlagsInfo.ApplyProtection)
				cloneFlagsInfo.ApplyProtection = styleFormat.Protection.Hidden != cellFormat.Protection.Hidden || styleFormat.Protection.Locked != cellFormat.Protection.Locked;
			cellFormat.AssignCellFormatFlagsIndex(cloneFlagsInfo.PackedValues);
		}
		void UpdateDefaultCellFormat(CellFormat cellFormat, CellStyleFormat normalFormat) {
			cellFormat.AssignNumberFormatIndex(normalFormat.NumberFormatIndex);
			cellFormat.AssignFontIndex(normalFormat.FontIndex);
			if (cellFormat.Fill.FillType == ModelFillType.Pattern)
				cellFormat.AssignFillIndex(normalFormat.FillIndex);
			else
				cellFormat.AssignGradientFillInfoIndex(normalFormat.GradientFillInfoIndex);
			cellFormat.AssignBorderIndex(normalFormat.BorderIndex);
			cellFormat.AssignAlignmentIndex(normalFormat.AlignmentIndex);
			cellFormat.AssignCellFormatFlagsIndex(DocumentModel.Cache.CellFormatCache.DefaultItem.CellFormatFlagsIndex);
		}
		void UpdateNormalIndexes(CellFormat cellFormat, CellStyleFormat normalFormat) {
			if (cellFormat.FillIndex == FillInfoTable[0] && cellFormat.Fill.FillType == ModelFillType.Pattern)
				cellFormat.AssignFillIndex(normalFormat.FillIndex);
			if (cellFormat.GradientFillInfoIndex == FillInfoTable[0] && cellFormat.Fill.FillType == ModelFillType.Gradient)
				cellFormat.AssignGradientFillInfoIndex(normalFormat.GradientFillInfoIndex);
			if (cellFormat.BorderIndex == BorderInfoTable[0])
				cellFormat.AssignBorderIndex(normalFormat.BorderIndex);
		}
		string CalculateCustomName() {
			string prefixCustomName = "Style ";
			int index = 1;
			string result = prefixCustomName + index.ToString();
			for (int i = 0; i < CellStyles.Count; i++) {
				CustomCellStyle cellStyle = CellStyles[i] as CustomCellStyle;
				if (cellStyle != null) {
					result = prefixCustomName + index.ToString();
					if (StringExtensions.CompareInvariantCultureIgnoreCase(cellStyle.Name, result) == 0)
						index++;
					else
						return result;
				}
			}
			return result;
		}
		public virtual int GetCellFormatIndex(int formatIndex) {
			if (formatIndex > 0) {
				int index;
				if (CellFormatTable.TryGetValue(formatIndex, out index))
					return index;
			}
			return 0;
		}
		public int RegisterIndexedColor(int colorIndex) {
			ColorModelInfo info = new ColorModelInfo();
			info.ColorIndex = colorIndex;
			return DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(info);
		}
		public int GetNextDifferentialFormatIndex() {
			return DifferentialFormatTable.Count;
		}
		public void RegisterDifferentialFormat(DifferentialFormat info) {
			int index = DocumentModel.Cache.CellFormatCache.AddItem(info);
			DifferentialFormatTable.Add(DifferentialFormatTable.Count, index);
		}
		internal int GetDifferentialFormatIndex(int id) {
			if (id < 0)
				return CellFormatCache.DefaultDifferentialFormatIndex;
			return DifferentialFormatTable[id];
		}
		internal int GetCellFormatIndex(int elementIndex, CellStyleBase cellStyle) {
			DocumentModel documentModel = cellStyle.DocumentModel;
			CellFormat cellFormat = new CellFormat(documentModel);
			cellFormat.AssignStyleIndex(cellStyle.StyleIndex);
			return documentModel.Cache.CellFormatCache.AddItem(cellFormat);
		}
		protected internal void SetTableStyleName(TableInfo info, string name) {
			if (!String.IsNullOrEmpty(name)) {
				info.StyleName = name;
				bool isPredefinedStyle = TableStyleName.CheckPredefinedName(name);
				TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
				bool hasStyle = tableStyles.ContainsStyleName(name);
				info.ApplyTableStyle = isPredefinedStyle || hasStyle;
				if (isPredefinedStyle && !hasStyle) {
					PredefinedTableStyleId id = (PredefinedTableStyleId)TableStyleName.PredefinedTableNamesTable[name];
					tableStyles.AddCore(TableStyle.CreateTablePredefinedStyle(DocumentModel, id));
				}
			}
			else
				info.StyleName = TableStyleName.DefaultStyleName.Name;
		}
	}
	#endregion
	#region ImportCellFormatInfo
	public class ImportCellFormatInfo {
		internal const int DefaultStyleId = -1;
		CellFormatFlagsInfo formatFlagsInfo;
		public ImportCellFormatInfo(CellFormatFlagsInfo initialValue) {
			this.formatFlagsInfo = initialValue;
		}
		public int FontId { get; set; }
		public int FillId { get; set; }
		public int BorderId { get; set; }
		public int NumberFormatId { get; set; }
		public int StyleId { get; set; }
		public bool QuotePrefix { get { return formatFlagsInfo.QuotePrefix; } set { formatFlagsInfo.QuotePrefix = value; } }
		public bool PivotButton { get { return formatFlagsInfo.PivotButton; } set { formatFlagsInfo.PivotButton = value; } }
		public bool ApplyFont { get { return formatFlagsInfo.ApplyFont; } set { formatFlagsInfo.ApplyFont = value; } }
		public bool ApplyFill { get { return formatFlagsInfo.ApplyFill; } set { formatFlagsInfo.ApplyFill = value; } }
		public bool ApplyBorder { get { return formatFlagsInfo.ApplyBorder; } set { formatFlagsInfo.ApplyBorder = value; } }
		public bool ApplyNumberFormat { get { return formatFlagsInfo.ApplyNumberFormat; } set { formatFlagsInfo.ApplyNumberFormat = value; } }
		public bool ApplyAlignment { get { return formatFlagsInfo.ApplyAlignment; } set { formatFlagsInfo.ApplyAlignment = value; } }
		public bool ApplyProtection { get { return formatFlagsInfo.ApplyProtection; } set { formatFlagsInfo.ApplyProtection = value; } }
		public bool HasExtension { get { return formatFlagsInfo.HasExtension; } set { formatFlagsInfo.HasExtension = value; } }
		public bool IsLocked { get { return formatFlagsInfo.Locked; } set { formatFlagsInfo.Locked = value; } }
		public bool IsHidden { get { return formatFlagsInfo.Hidden; } set { formatFlagsInfo.Hidden = value; } }
		public ModelFillType FillType { get { return formatFlagsInfo.FillType; } set { formatFlagsInfo.FillType = value; } }
		public int AlignmentIndex { get; set; }
		public CellFormatFlagsInfo FormatFlagsInfo { get { return formatFlagsInfo; } set { formatFlagsInfo = value; } }
	}
	#endregion
	#region ImportExportTableStyleInfo
	public class ImportExportTableStyleInfo {
		#region Static Members
		public static ImportExportTableStyleInfo CreateFrom(TableStyle style, ExportStyleSheet exportStyleSheet) {
			ImportExportTableStyleInfo result = new ImportExportTableStyleInfo();
			result.name = style.Name.Name;
			for (byte elementIndex = 0; elementIndex < TableStyle.ElementsCount; elementIndex++) {
				int formatIndex = style.FormatIndexes[elementIndex];
				if (formatIndex != TableStyleElementFormatCache.DefaultItemIndex) {
					TableStyleElementFormat format = exportStyleSheet.Workbook.Cache.TableStyleElementFormatCache[formatIndex];
					int dxfId = exportStyleSheet.GetDifferentialFormatId(format.DifferentialFormatIndex);
					ImportExportTableStyleElementInfo importInfo = new ImportExportTableStyleElementInfo(dxfId, format.StripeSize);
					result.RegisteredTableStyleElementTable.Add(elementIndex, importInfo);
				}
			}
			bool isHidden = style.IsHidden;
			if (style.IsTableStyle) {
				result.isTable = !isHidden;
				result.isPivot = false;
			}
			if (style.IsPivotStyle) {
				result.isTable = false;
				result.isPivot = !isHidden;
			}
			if (style.IsGeneralStyle) {
				result.isTable = !isHidden;
				result.isPivot = !isHidden;
			}
			return result;
		}
		#endregion
		#region Fields
		readonly Dictionary<int, ImportExportTableStyleElementInfo> registeredTableStyleElementTable = new Dictionary<int, ImportExportTableStyleElementInfo>();
		string name;
		bool isTable = true;
		bool isPivot = true;
		#endregion
		#region Properties
		public Dictionary<int, ImportExportTableStyleElementInfo> RegisteredTableStyleElementTable { get { return registeredTableStyleElementTable; } }
		public string Name { get { return name; } set { name = value; } }
		public bool IsTable { get { return isTable; } set { isTable = value; } }
		public bool IsPivot { get { return isPivot; } set { isPivot = value; } }
		public bool IsHidden { get { return !IsTable && !IsPivot; } }
		public TableStyleElementIndexTableType TableType {
			get {
				if (IsHidden || (IsTable && IsPivot))
					return TableStyleElementIndexTableType.General;
				if (isPivot)
					return TableStyleElementIndexTableType.Pivot;
				return TableStyleElementIndexTableType.Table;
			}
		}
		#endregion
		public void RegisterStyleElementFormat(int elementIndex, int? dxfId, int? stripeSize) {
			ImportExportTableStyleElementInfo info = new ImportExportTableStyleElementInfo(dxfId, stripeSize);
			if (!RegisteredTableStyleElementTable.ContainsKey(elementIndex))
				RegisteredTableStyleElementTable.Add(elementIndex, info);
		}
		public void RegisterStyle(DocumentModel documentModel, Dictionary<int, int> dxfIdToFormatIndexTranslationTable) {
			if (documentModel.StyleSheet.TableStyles.ContainsStyleName(name))
				return;
			TableStyle style = TableStyle.CreateCustomStyle(documentModel, name, TableType);
			foreach (KeyValuePair<int, ImportExportTableStyleElementInfo> pair in registeredTableStyleElementTable) {
				ImportExportTableStyleElementInfo info = pair.Value;
				TableStyleElementFormat elementFormat = info.TryGetTableStyleElementFormat(documentModel, dxfIdToFormatIndexTranslationTable);
				if (elementFormat != null) {
					int formatIndex = documentModel.Cache.TableStyleElementFormatCache.GetItemIndex(elementFormat);
					style.AssignFormatIndex(pair.Key, formatIndex);
				}
			}
			style.IsHidden = IsHidden;
			documentModel.StyleSheet.TableStyles.Add(style);
		}
	}
	#endregion
	#region ImportExportTableStyleElementInfo
	public class ImportExportTableStyleElementInfo {
		#region Fields
		int? dxfId;
		int? stripeSize;
		#endregion
		public ImportExportTableStyleElementInfo(int? dxfId, int? stripeSize) {
			this.dxfId = dxfId;
			this.stripeSize = stripeSize;
		}
		#region Properties
		public int? DxfId { get { return dxfId; } }
		public int? StripeSize { get { return stripeSize; } }
		#endregion
		public TableStyleElementFormat TryGetTableStyleElementFormat(DocumentModel documentModel, Dictionary<int, int> dxfIdToFormatIndexTranslationTable) {
			bool hasDxfId = dxfId.HasValue;
			if (hasDxfId && !dxfIdToFormatIndexTranslationTable.ContainsKey(dxfId.Value))
				return null;
			TableStyleElementFormat result = new TableStyleElementFormat(documentModel);
			if (stripeSize.HasValue) {
				int stripeSizeValue = this.stripeSize.Value;
				if (stripeSizeValue >= StripeSizeInfo.DefaultValue)
					result.AssignStripeSizeInfoIndex(stripeSizeValue);
			}
			if (hasDxfId) {
				int differentialFormatIndex = dxfIdToFormatIndexTranslationTable[dxfId.Value];
				result.AssignDifferentialFormatIndex(differentialFormatIndex);
			}
			return result;
		}
	}
	#endregion
	#region CellStyleByNameComparer
	public class CellStyleByNameComparer : IComparer<CellStyleBase> {
		public int Compare(CellStyleBase x, CellStyleBase y) {
			return String.Compare(x.Name, y.Name);
		}
	}
	#endregion
	public static class LogServiceHelper {
		public static void LogMessage(this DocumentModel workbook, LogCategory category, string message) {
			ILogService logService = workbook.GetService<ILogService>();
			if (logService != null)
				logService.LogMessage(category, message);
		}
		public static void LogMessage(this DocumentModel workbook, LogCategory category, XtraSpreadsheetStringId stringId) {
			ILogService logService = workbook.GetService<ILogService>();
			if (logService != null)
				logService.LogMessage(category, XtraSpreadsheetLocalizer.GetString(stringId));
		}
		public static void LogMessageFormat(this DocumentModel workbook, LogCategory category, XtraSpreadsheetStringId stringId, params object[] args) {
			ILogService logService = workbook.GetService<ILogService>();
			if (logService != null)
				logService.LogMessage(category, string.Format(XtraSpreadsheetLocalizer.GetString(stringId), args));
		}
		public static void LogMessage(this WorkbookDataContext context, LogCategory category, string message) {
			LogMessage(context.Workbook, category, message);
		}
	}
	#region IGuidProvider
	public interface IGuidProvider {
		Guid GetBinary();
		string GetUppercaseString();
		string GetUppercaseString(string format);
	}
	#endregion
	#region GuidProvider
	public class GuidProvider : IGuidProvider {
		public Guid GetBinary() {
			return Guid.NewGuid();
		}
		public string GetUppercaseString() {
			return Guid.NewGuid().ToString().ToUpper();
		}
		public string GetUppercaseString(string format) {
			return Guid.NewGuid().ToString(format).ToUpper();
		}
	}
	#endregion
	#region SharedStringIndexTable
	public class SharedStringIndexTable {
		int[] table;
		public SharedStringIndexTable(int capacity) {
			Guard.ArgumentNonNegative(capacity, "capacity");
			this.table = new int[capacity];
		}
		public int this[int index] {
			get { return this.table[index] - 1; }
			set { this.table[index] = value + 1; }
		}
		public bool Contains(int index) {
			return this.table[index] > 0;
		}
	}
	#endregion
	public class OptionsRangeParser {
		public CellRange CalculateOptionsRange(Worksheet worksheet, string formula) {
			if (String.IsNullOrEmpty(formula))
				return null;
			WorkbookDataContext dataContext = worksheet.DataContext;
			ParsedExpression expression = new ParsedExpression();
			if (formula.StartsWith("=", StringComparison.Ordinal))
				formula = formula.Remove(0, 1);
			dataContext.PushCulture(CultureInfo.InvariantCulture);
			dataContext.PushCurrentWorksheet(worksheet);
			dataContext.PushRelativeToCurrentCell(true);
			dataContext.PushCurrentCell(0, 0);
			try {
				expression = dataContext.ParseExpression(formula, OperandDataType.Reference, true);
				if (expression != null) {
					VariantValue expressionValue = expression.Evaluate(dataContext);
					if (expressionValue.IsCellRange) {
						CellRange cellRange = expressionValue.CellRangeValue as CellRange;
						if (cellRange != null) {
							if (object.ReferenceEquals(cellRange.Worksheet, worksheet))
								return cellRange;
							Exceptions.ThrowArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet), formula);
						}
						Exceptions.ThrowArgumentException("Union ranges are not supported", formula);
					}
				}
				Exceptions.ThrowArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpression), formula);
			}
			finally {
				dataContext.PopRelativeToCurrentCell();
				dataContext.PopCurrentCell();
				dataContext.PopCurrentWorksheet();
				dataContext.PopCulture();
			}
			return null;
		}
	}
}
