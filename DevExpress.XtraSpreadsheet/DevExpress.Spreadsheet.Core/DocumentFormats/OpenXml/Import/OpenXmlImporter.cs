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
using System.IO;
using System.Xml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
#if !SL
using DevExpress.Office.Crypto.Agile;
using DevExpress.Office.Drawing;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class OpenXmlImporter : SpreadsheetMLBaseImporter, IDisposable {
		public static Dictionary<SheetVisibleState, string> visibilityTypeTable = CreateVisibilityTypeTable();
		static Dictionary<SheetVisibleState, string> CreateVisibilityTypeTable() {
			Dictionary<SheetVisibleState, string> result = new Dictionary<SheetVisibleState, string>();
			result.Add(SheetVisibleState.Visible, "visible");
			result.Add(SheetVisibleState.VeryHidden, "veryHidden");
			result.Add(SheetVisibleState.Hidden, "hidden");
			return result;
		}
		static TranslationTable<HashAlgorithmType> hashAlgorithmTypeTable = CreateHashAlgorithmTypeTable();
		static TranslationTable<HashAlgorithmType> CreateHashAlgorithmTypeTable() {
			TranslationTable<HashAlgorithmType> result = new TranslationTable<HashAlgorithmType>();
			result.Add(HashAlgorithmType.Sha512, "SHA-512");
			result.Add(HashAlgorithmType.Sha1, "SHA-1");
			result.Add(HashAlgorithmType.Sha256, "SHA-256");
			result.Add(HashAlgorithmType.Sha384, "SHA-384");
			result.Add(HashAlgorithmType.Md5, "MD5");
			result.Add(HashAlgorithmType.Md4, "MD4");
			result.Add(HashAlgorithmType.Md2, "MD2");
			result.Add(HashAlgorithmType.Ripemd, "RIPEMD-128");
			result.Add(HashAlgorithmType.Ripemd160, "RIPEMD-160");
			result.Add(HashAlgorithmType.Whirlpool, "WHIRLPOOL");
			return result;
		}
		public static TranslationTable<HashAlgorithmType> HashAlgorithmTypeTranslationTable { get { return hashAlgorithmTypeTable; } }
		#region Fields
		public const string InvalidFileMessage = "Invalid OpenXml file";
		Stack<OpenXmlRelationCollection> documentRelationsStack;
		string documentRootFolder;
		PackageFileStreams packageFileStreams;
		Dictionary<string, OfficeNativeImage> packageImages;
		List<string> sheetsRelationIds = new List<string>();
		Dictionary<string, Worksheet> tableRelations = new Dictionary<string, Worksheet>();
		Dictionary<string, Worksheet> pivotTableRelations = new Dictionary<string, Worksheet>();
		Dictionary<string, int> pivotCacheDefinitionRelations = new Dictionary<string, int>();
		Dictionary<string, int> pivotCacheRecordsRelations = new Dictionary<string, int>();
		Dictionary<int, OpenXmlRelationCollection> pivotCacheIdToExternalLinkPathsTranslationTable = new Dictionary<int, OpenXmlRelationCollection>();
		Dictionary<string, Chart> chartRelations = new Dictionary<string, Chart>();
		Dictionary<string, InternalSheetBase> drawingsRelations = new Dictionary<string, InternalSheetBase>();
		Dictionary<string, Table> queryTableRelations = new Dictionary<string, Table>();
		Dictionary<string, PivotSelection> pivotSelectionRelations = new Dictionary<string, PivotSelection>();
		Dictionary<string, Worksheet> commentRelations = new Dictionary<string, Worksheet>();
		Dictionary<int, int> commentAuthorIds = new Dictionary<int, int>();
		Dictionary<string, Worksheet> vmlDrawingRelations = new Dictionary<string, Worksheet>();
		Dictionary<string, OpenXmlRelationCollection> relationsCache = new Dictionary<string, OpenXmlRelationCollection>();
		Dictionary<int, int> sharedFormulaIds = new Dictionary<int, int>();
		Dictionary<int, Worksheet> sheetIdsTable = new Dictionary<int, Worksheet>();
		SpreadsheetMLImportStyleSheet styleSheet;
		#endregion
		public OpenXmlImporter(IDocumentModel documentModel, OpenXmlDocumentImporterOptions options)
			: base(documentModel, options) {
			this.styleSheet = new SpreadsheetMLImportStyleSheet(this);
		}
		#region Properties
		public OpenXmlDocumentImporterOptions Options { get { return (OpenXmlDocumentImporterOptions)InnerOptions; } }
		public Stack<OpenXmlRelationCollection> DocumentRelationsStack { get { return documentRelationsStack; } }
		public override OpenXmlRelationCollection DocumentRelations { get { return documentRelationsStack.Peek(); } }
		public override string DocumentRootFolder { get { return documentRootFolder; } set { documentRootFolder = value; } }
		public override string SpreadsheetNamespace { get { return OpenXmlExporter.SpreadsheetNamespaceConst; } }
		public override string OfficeNamespace { get { return OpenXmlExporter.OfficeNamespaceConst; } }
		public override string RelationsNamespace { get { return OpenXmlExporter.RelsNamespace; } }
		public override string ExtendedPropertiesNamespace { get { return OpenXmlExporter.ExtendedPropertiesNamespace; } }
		public override string CorePropertiesNamespace { get { return OpenXmlExporter.CorePropertiesNamespace; } }
		public override string CustomPropertiesNamespace { get { return OpenXmlExporter.CustomPropertiesNamespace; } }
		public string OfficeDocumentNamespace { get { return OpenXmlExporter.OfficeDocumentNamespace; } }
		public string RelsStylesNamespace { get { return OpenXmlExporter.RelsStylesNamespace; } }
		public string RelsCalculationChainNamespace { get { return OpenXmlExporter.RelsCalculationChainNamespace; } }
		public string PackageRelsNamespace { get { return OpenXmlExporter.PackageRelsNamespace; } }
		public string RelsSharedStrings { get { return OpenXmlExporter.RelsSharedStringsNamespace; } }
		public string RelsTablesNamespace { get { return OpenXmlExporter.RelsTablesNamespace; } }
		public string RelsPivotCacheDefinitionNamepace { get { return OpenXmlExporter.RelsPivotCacheDefinitionNamepace; } }
		public string RelsPivotCacheRecordsNamepace { get { return OpenXmlExporter.RelsPivotCacheRecordsNamepace; } }
		public string RelsPivotTablesNamepace { get { return OpenXmlExporter.RelsPivotTablesNamepace; } }
		public string RelsConnectionsNamespace { get { return OpenXmlExporter.RelsConnectionsNamespace; } }
		public string RelsXmlMapsNamespace { get { return OpenXmlExporter.RelsXmlMapsNamespace; } }
		public string RelsQueryTablesNamespace { get { return OpenXmlExporter.RelsQueryTablesNamespace; } }
		public string RelsCommentsNamepace { get { return OpenXmlExporter.RelsCommentsNamepace; } }
		public string SpreadsheetDrawingNamespace { get { return OpenXmlExporter.SpreadsheetDrawingNamespace; } }
		public string DrawingMLNamespace { get { return OpenXmlExporter.DrawingMLNamespace; } }
		public string VmlDrawingNamespace { get { return OpenXmlExporter.VmlDrawingNamespace; } }
		public string VmlDrawingOfficeNamespace { get { return OpenXmlExporter.VmlDrawingOfficeNamespace; } }
		public string VmlDrawingExcelNamespace { get { return OpenXmlExporter.VmlDrawingExcelNamespace; } }
		public string RelsVbaProjectNamespace { get { return OpenXmlExporter.RelsVbaProjectNamespace; } }
		public string RelsVolatileDependenciesNamespace { get { return OpenXmlExporter.RelsVolatileDependenciesNamespace; } }
		public string RelsExternalLinkPathNamespace { get { return OpenXmlExporter.RelsExternalLinkPathNamespace; } }
		public string ExternalTargetMode { get { return OpenXmlExporter.ExternalTargetMode; } }
		public List<string> SheetsRelationIds { get { return sheetsRelationIds; } }
		public Dictionary<int, Worksheet> SheetIdsTable { get { return sheetIdsTable; } }
		public Dictionary<string, Worksheet> TableRelations { get { return tableRelations; } }
		public Dictionary<string, Worksheet> PivotTableRelations { get { return pivotTableRelations; } }
		public Dictionary<string, int> PivotCacheDefinitionRelations { get { return pivotCacheDefinitionRelations; } }
		public Dictionary<string, int> PivotCacheRecordsRelations { get { return pivotCacheRecordsRelations; } }
		public Dictionary<int, OpenXmlRelationCollection> PivotCacheIdToExternalLinkPathsTranslationTable { get { return pivotCacheIdToExternalLinkPathsTranslationTable; } }
		public Dictionary<string, Chart> ChartRelations { get { return chartRelations; } }
		public Dictionary<string, InternalSheetBase> DrawingsRelations { get { return drawingsRelations; } }
		public Dictionary<string, Worksheet> CommentRelations { get { return commentRelations; } }
		public Dictionary<string, Worksheet> VmlDrawingRelations { get { return vmlDrawingRelations; } }
		public Dictionary<string, Table> QueryTableRelations { get { return queryTableRelations; } }
		public Dictionary<string, PivotSelection> PivotSelectionRelations { get { return pivotSelectionRelations; } }
		protected internal PackageFileStreams PackageFileStreams { get { return packageFileStreams; } }
		protected internal Dictionary<string, OfficeNativeImage> PackageImages { get { return packageImages; } }
		protected override bool CreateEmptyDocumentOnLoadError { get { return Options.CreateEmptyDocumentOnLoadError; } }
		public override SpreadsheetMLImportStyleSheet StyleSheet { get { return styleSheet; } }
		protected internal Dictionary<int, int> CommentAuthorIds { get { return commentAuthorIds; } }
		protected internal Dictionary<int, int> SharedFormulaIds { get { return sharedFormulaIds; } }
		protected override TranslationTable<HashAlgorithmType> HashAlgorithmTypeTable { get { return hashAlgorithmTypeTable; } }
		public int NonRegisteredCellsInCellsChainCount { get; set; }
		public int NonRegisteredPivotCacheDefinitionRelationsCount { get; set; }
		public int ActualPivotCacheDefinitionId { get; set; }
		#endregion
		public virtual void Import(Stream stream) {
			try {
				ImportWorkbook(stream);
			}
			catch {
				if (CreateEmptyDocumentOnLoadError)
					SetMainDocumentEmptyContent();
				throw;
			}
			finally {
				CellDestination.ClearInstance();
				FormulaDestination.ClearInstance();
				CellValueDestination<ICell>.ClearInstance();
				CellValueDestination<DevExpress.XtraSpreadsheet.Model.External.ExternalCell>.ClearInstance();
				CalculationChainElement.ClearInstance();
			}
		}
		protected internal virtual void ImportWorkbook(Stream stream) {
			OpenPackage(stream);
			ILogService logService = DocumentModel.GetService<ILogService>();
			if (logService != null)
				logService.Clear();
			OpenXmlRelationCollection rootRelations = ImportRelations("_rels/.rels");
			string fileName = LookupRelationTargetByType(rootRelations, OfficeDocumentNamespace, String.Empty, "workbook.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader == null)
				ThrowInvalidFile("Can't get reader for workbook.xml");
			if (!ReadToRootElement(reader, "workbook", SpreadsheetNamespace))
				ThrowInvalidFile("Can't find root element \"workbook\"");
			this.documentRootFolder = Path.GetDirectoryName(fileName);
			CreateDocumentRelationsStack();
			this.documentRelationsStack.Push(ImportRelations(documentRootFolder + "/_rels/" + Path.GetFileName(fileName) + ".rels"));
			CreatePackageFileStreams();
			CreatePackageImages();
			ImportMainDocument(reader, stream);
		}
#if !SL
		#region OpenPackage
		protected internal override void OpenPackage(Stream stream) {
			long position = CalculateInitialStreamPosition(stream);
			base.OpenPackage(stream);
			if (PackageFiles.Count <= 0 && SeekToInitialStreamPosition(stream, position)) {
				Stream dataStream = OpenEncryptedStream(stream);
				if (dataStream != null)
					base.OpenPackage(dataStream);
			}
		}
		Stream OpenEncryptedStream(Stream stream) {
			try {
				PackageFileReader reader = new PackageFileReader(stream);
				Stream encryptedInfoStream = reader.GetCachedPackageFileStream(@"\EncryptionInfo");
				if (encryptedInfoStream == null)
					return null;
				EncryptionSession session = EncryptionSession.LoadFromStream(encryptedInfoStream, true);
				bool unlocked = session.UnlockWithPassword(Options.EncryptionPassword);
				if (!unlocked)
					return null;
				Stream encryptedDataStream = reader.GetCachedPackageFileStream(@"\EncryptedPackage");
				if (encryptedDataStream == null)
					return null;
				return session.GetEncryptedStream(encryptedDataStream);
			}
			catch {
				return null;
			}
		}
		long CalculateInitialStreamPosition(Stream stream) {
			if (stream == null)
				return -1;
			try {
				return stream.Position;
			}
			catch {
				return -1;
			}
		}
		bool SeekToInitialStreamPosition(Stream stream, long position) {
			try {
				if (stream.CanSeek && position >= 0) {
					stream.Seek(position, SeekOrigin.Begin);
					return true;
				}
				else
					return false;
			}
			catch {
				return false;
			}
		}
		#endregion
#endif
		protected internal virtual void CreateDocumentRelationsStack() {
			this.documentRelationsStack = new Stack<OpenXmlRelationCollection>();
		}
		protected void CreatePackageFileStreams() {
			this.packageFileStreams = new PackageFileStreams();
		}
		protected void CreatePackageImages() {
			this.packageImages = new Dictionary<string, OfficeNativeImage>();
		}
		protected internal virtual OpenXmlRelationCollection ImportRelations(string fileName) {
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				return ImportRelationsCore(reader);
			else
				return new OpenXmlRelationCollection();
		}
		protected internal virtual OpenXmlRelationCollection ImportRelationsCore(XmlReader reader) {
			OpenXmlRelationCollection result = new OpenXmlRelationCollection();
			if (!ReadToRootElement(reader, "Relationships", PackageRelsNamespace))
				return result;
			ImportContent(reader, new RelationshipsDestination(this, result));
			return result;
		}
		protected internal virtual OpenXmlRelationCollection GetRelations(string fileName) {
			OpenXmlRelationCollection value;
			if (this.relationsCache.TryGetValue(fileName, out value) == false) {
				value = ImportRelations(fileName);
				this.relationsCache.Add(fileName, value);
			}
			return value;
		}
		protected internal virtual string LookupRelationTargetByType(OpenXmlRelationCollection relations, string type, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.LookupRelationTargetByType(relations, type, rootFolder, defaultFileName);
		}
		protected internal virtual string LookupRelationTargetById(OpenXmlRelationCollection relations, string id, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.LookupRelationTargetById(relations, id, rootFolder, defaultFileName);
		}
		public override OfficeImage LookupImageByRelationId(IDocumentModel documentModel, string relationId, string rootFolder) {
			OfficeNativeImage rootImage;
			if (PackageImages.TryGetValue(relationId, out rootImage))
				return new OfficeReferenceImage(documentModel, rootImage);
			Stream stream = LookupPackageFileStreamByRelationId(relationId, rootFolder, false);
			if (stream == null)
				return null;
			else {
				try {
					OfficeReferenceImage image = documentModel.CreateImage(stream);
					if (image != null)
						PackageImages.Add(relationId, image.NativeRootImage);
					return image;
				}
				catch {
					return null;
				}
			}
		}
		protected internal virtual string CalculateRelationTargetCore(OpenXmlRelation relation, string rootFolder, string defaultFileName) {
			return OpenXmlImportRelationHelper.CalculateRelationTarget(relation, rootFolder, defaultFileName);
		}
		protected override void BeforeImportMainDocument() {
			base.BeforeImportMainDocument();
			ImportStyles(); 
			ImportSharedStringTable();
		}
		protected override void AfterImportMainDocument() {
			base.AfterImportMainDocument();
			OverrideCalculationMode();
			PrepareConditionalFormattingsCollection();
			ImportExternalReference();
			ImportDocumentCoreProperties();
			ImportDocumentApplicationProperties();
			ImportDocumentCustomProperties();
			ImportWorksheets();
			ImportVmlDrawings();
			ImportComments();
			ImportTables();
			ImportDrawings();
			ImportCharts();
			ImportConnections();
			ImportXmlMaps();
			ImportQueryTables();
			ImportTheme();
			ImportPivotCaches();
			ImportPivotCacheRecords();
			ImportPivotTables();
			ImportVbaProject();
			ImportVolatileDependencies();
			ApplyDefinedNameReferences();
			ApplyConditionalFormattingsCollection();
			ImportCalculationChain();
			ImportCustomXmlPart();
		}
		void OverrideCalculationMode() {
			if (Options.OverrideCalculationMode == CalculationModeOverride.None)
				return;
			DocumentModel.Properties.CalculationOptions.CalculationMode = (ModelCalculationMode)Options.OverrideCalculationMode;
		}
		#region Worksheets
		protected internal virtual void ImportWorksheets() {
			NonRegisteredCellsInCellsChainCount = 0;
			for (int i = 0; i < SheetsRelationIds.Count; i++) {
				OpenXmlRelation relation = DocumentRelations.LookupRelationById(SheetsRelationIds[i]);
				if (relation != null) {
					string fileName = CalculateRelationTargetCore(relation, DocumentRootFolder, String.Empty);
					XmlReader reader = GetPackageFileXmlReader(fileName);
					if (reader != null) {
						string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
						this.documentRelationsStack.Push(GetRelations(pathToRels));
						ImportWorksheetsCore(reader, i);
						DocumentModel.Sheets[i].IsNotWorksheet = (relation.Type != OpenXmlExporter.RelsWorksheetNamespace);
						this.documentRelationsStack.Pop();
					}
				}
			}
		}
		protected internal virtual void ImportWorksheetsCore(XmlReader reader, int sheetIndex) {
			if (!ReadToRootElement(reader, "worksheet", SpreadsheetNamespace))
				return;
			ImportContent(reader, new WorksheetDestination(this, DocumentModel.Sheets[sheetIndex]));
		}
		protected internal void ImportWorksheetRelations() {
			if (documentRelationsStack == null)
				return;
			foreach (OpenXmlRelation relation in DocumentRelations) {
				AddWorksheetRelations(relation, CommentRelations, RelsCommentsNamepace);
				AddWorksheetRelations(relation, PivotTableRelations, RelsPivotTablesNamepace);
			}
		}
		void AddWorksheetRelations(OpenXmlRelation relation, Dictionary<string, Worksheet> relations, string relationType) {
			if (relation.Type == relationType) {
				string relationPath = CalculateRelationTargetCore(relation, DocumentRootFolder, string.Empty);
				if (!string.IsNullOrEmpty(relationPath))
					relations.Add(relationPath, CurrentWorksheet);
			}
		}
		#endregion
		#region Theme
		protected internal virtual void ImportTheme() {
			foreach (OpenXmlRelation relation in DocumentRelations) {
				if (relation.Type == OpenXmlExporter.RelsThemeNamespace) {
					string fileName = CalculateRelationTargetCore(relation, DocumentRootFolder, String.Empty);
					string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
					OpenXmlRelationCollection currentRelations = GetRelations(pathToRels);
					if (currentRelations.Count > 0)
						documentRelationsStack.Push(currentRelations);
					XmlReader reader = GetPackageFileXmlReader(fileName);
					if (reader != null && ReadToRootElement(reader, "theme", DrawingMLNamespace)) {
						ImportThemeCore(reader);
					}
				}
			}
		}
		protected override void PrepareOfficeTheme() {
			OfficeThemeBase<DevExpress.Spreadsheet.DocumentFormat> theme = new OfficeThemeBase<DevExpress.Spreadsheet.DocumentFormat>();
			DocumentModel.OfficeTheme = theme;
			ActualDocumentModel = theme;
		}
		#endregion
		#region Styles
		protected internal virtual void ImportStyles() {
			string fileName = LookupRelationTargetByType(DocumentRelations, RelsStylesNamespace, DocumentRootFolder, "styles.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportStylesCore(reader);
		}
		protected internal virtual void ImportStylesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "styleSheet", SpreadsheetNamespace))
				return;
			StyleSheet styleSheet = DocumentModel.StyleSheet;
			styleSheet.CellStyles.BeginUpdate();
			styleSheet.TableStyles.BeginUpdate();
			try {
				ImportContent(reader, new StyleSheetDestination(this));
			}
			finally {
				styleSheet.TableStyles.EndUpdate();
				styleSheet.CellStyles.EndUpdate();
			}
		}
		#endregion
		#region VolatileDependencies
		protected internal virtual void ImportVolatileDependencies() {
			string fileName = LookupRelationTargetByType(DocumentRelations, RelsVolatileDependenciesNamespace, DocumentRootFolder, "volatileDependencies.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportVolatileDependenciesCore(reader);
		}
		protected internal virtual void ImportVolatileDependenciesCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "volTypes", SpreadsheetNamespace))
				return;
			ImportContent(reader, new VolatileDependenciesDestination(this));
		}
		#endregion
		#region SharedStringTable
		protected internal virtual void ImportSharedStringTable() {
			string fileName = LookupRelationTargetByType(DocumentRelations,
				RelsSharedStrings,
				DocumentRootFolder,
				"sharedStrings.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportSharedStringTableCore(reader);
		}
		protected internal virtual void ImportSharedStringTableCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "sst", SpreadsheetNamespace))
				return;
			ImportContent(reader, new SharedStringTableDestination(this, DocumentModel.SharedStringTable));
		}
		#endregion
		#region Comments
		protected internal virtual void ImportComments() {
			foreach (KeyValuePair<string, Worksheet> commentRelation in CommentRelations) {
				XmlReader reader = GetPackageFileXmlReader(commentRelation.Key);
				if (reader != null)
					ImportCommentsCore(reader, commentRelation.Value);
			}
		}
		protected internal virtual void ImportCommentsCore(XmlReader reader, Worksheet sheet) {
			if (!ReadToRootElement(reader, "comments", SpreadsheetNamespace))
				return;
			Destination destination = new CommentsDestination(this, sheet);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region VmlDrawings
		protected internal virtual void ImportVmlDrawings() {
			foreach (KeyValuePair<string, Worksheet> vmlDrawingRelation in VmlDrawingRelations) {
				string fileName = vmlDrawingRelation.Key;
				Stream stream = GetPackageFileStream(fileName);
				if (stream == null)
					return;
				StreamReader streamReader = new StreamReader(stream);
				string vmlContent = streamReader.ReadToEnd();
				ImportVmlDrawingsCore(vmlDrawingRelation.Value, vmlContent, fileName);
			}
		}
		protected internal virtual void ImportVmlDrawingsCore(Worksheet sheet, string vmlContent, string fileName) {
			XmlReader reader = CreateXmlReaderByString(vmlContent);
			if (reader == null)
				return;
			string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
			OpenXmlRelationCollection vmlDrawingRelations = GetRelations(pathToRels);
			this.documentRelationsStack.Push(vmlDrawingRelations);
			try {
				ImportVmlDrawingContent(reader, sheet, fileName);
			}
			catch { }
			finally {
#if DXPORTABLE
				reader.Dispose();
#else
				reader.Close();
#endif
				if (sheet.VmlDrawing == null) {
					vmlContent = ValidateXml(vmlContent);
					reader = CreateXmlReaderByString(vmlContent);
					ImportVmlDrawingContent(reader, sheet, fileName);
					if (sheet.VmlDrawing == null)
						sheet.VmlDrawing = new VmlDrawing();
				}
				this.documentRelationsStack.Pop();
			}
		}
		protected internal virtual void ImportVmlDrawingContent(XmlReader reader, Worksheet sheet, string fileName) {
			if (!ReadToRootElement(reader, "xml"))
				return;
			Worksheet previousWorksheet = this.CurrentWorksheet;
			this.CurrentWorksheet = sheet;
			try {
				Destination destination = new VmlDrawingDestination(this, sheet);
				ImportContent(reader, destination);
			}
			finally {
				this.CurrentWorksheet = previousWorksheet;
			}
		}
		string ValidateXml(string xmlContent) {
			return XmlValidator.ValidateXml(xmlContent);
		}
		#endregion
		#region ExternalReference
		protected internal virtual void ImportExternalReference() {
			int count = ExternalReferenceIdCollection.Count;
			for (int i = 0; i < count; i++) {
				string fileName = LookupRelationTargetById(DocumentRelations, ExternalReferenceIdCollection[i], DocumentRootFolder, String.Empty);
				XmlReader reader = GetPackageFileXmlReader(fileName);
				if (reader != null) {
					ImportExternalReferenceCore(reader, i);
					ReadExternalFilePath(fileName);
				}
			}
		}
		protected internal virtual void ReadExternalFilePath(string fileName) {
			OpenXmlRelationCollection relations = ImportRelations(Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels");
			OpenXmlRelation relation = relations.LookupRelationById(CurrentExternalFileRelationId);
			if (relation != null)
				DocumentModel.ExternalLinks[DocumentModel.ExternalLinks.Count - 1].Workbook.FilePath = relation.Target;
		}
		protected internal virtual void ImportExternalReferenceCore(XmlReader reader, int index) {
			if (!ReadToRootElement(reader, "externalLink", SpreadsheetNamespace))
				return;
			ImportContent(reader, new ExternalLinkDestination(this, index));
		}
		#endregion
		#region Tables
		protected internal virtual void ImportTables() {
			foreach (KeyValuePair<string, Worksheet> tableRelation in TableRelations) {
				string fileName = tableRelation.Key;
				XmlReader reader = GetPackageFileXmlReader(fileName);
				if (reader != null) {
					string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
					OpenXmlRelationCollection tableRelations = GetRelations(pathToRels);
					this.documentRelationsStack.Push(tableRelations);
					ImportTablesCore(reader, tableRelation.Value);
					this.documentRelationsStack.Pop();
				}
			}
		}
		protected internal virtual void ImportTablesCore(XmlReader reader, Worksheet sheet) {
			if (!ReadToRootElement(reader, "table", SpreadsheetNamespace))
				return;
			Worksheet previousSheet = this.CurrentWorksheet;
			this.CurrentWorksheet = sheet;
			try {
				TableDestination destination = new TableDestination(this, sheet);
				destination.ProcessElementOpen(reader);
				ImportContent(reader, destination);
			}
			finally {
				this.CurrentWorksheet = previousSheet;
			}
		}
		#endregion
		#region PivotCacheRecords
		void ImportPivotCacheRecords() {
			foreach (KeyValuePair<string, int> relation in pivotCacheRecordsRelations) {
				XmlReader reader = GetPackageFileXmlReader(relation.Key);
				if (reader != null && ReadToRootElement(reader, "pivotCacheRecords", SpreadsheetNamespace))
					ImportContent(reader, new PivotCacheRecordsDestination(this, DocumentModel.PivotCaches[relation.Value]));
			}
		}
		#endregion
		#region PivotCaches
		void ImportPivotCaches() {
			foreach (KeyValuePair<string, int> relation in pivotCacheDefinitionRelations) {
				XmlReader reader = GetPackageFileXmlReader(relation.Key);
				if (reader != null && ReadToRootElement(reader, "pivotCacheDefinition", SpreadsheetNamespace)) {
					int cacheId = relation.Value;
					ActualPivotCacheDefinitionId = cacheId;
					PivotCacheDefinitionDestination destination = new PivotCacheDefinitionDestination(this, new ImportPivotCacheInfo());
					destination.ProcessElementOpen(reader);
					ImportContent(reader, destination);
				}
			}
		}
		protected internal void ImportPivotCacheRelations(string relationId) {
			string cacheRelationFileName = LookupRelationTargetById(DocumentRelations, relationId, DocumentRootFolder, string.Empty);
			if (String.IsNullOrEmpty(cacheRelationFileName))
				ThrowInvalidFile();
			RegisterPivotCacheDefinitionRelations(cacheRelationFileName, NonRegisteredPivotCacheDefinitionRelationsCount);
			NonRegisteredPivotCacheDefinitionRelationsCount++;
		}
		void RegisterPivotCacheDefinitionRelations(string cacheRelationFileName, int cacheId) {
			PivotCacheDefinitionRelations.Add(cacheRelationFileName, cacheId);
			string cacheFolder = Path.GetDirectoryName(cacheRelationFileName);
			string cacheRelationsPath = cacheFolder + "/_rels/" + Path.GetFileName(cacheRelationFileName) + ".rels";
			OpenXmlRelationCollection cacheRelations = GetRelations(cacheRelationsPath);
			int count = cacheRelations.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlRelation relation = cacheRelations[i];
				if (relation.Type == RelsPivotCacheRecordsNamepace)
					RegisterPivotCacheRecordsRelation(relation, cacheFolder, cacheId);
				else if (relation.TargetMode == ExternalTargetMode)
					RegisterPivotCacheExternalLinkPathRelations(relation, cacheId);
			}
		}
		void RegisterPivotCacheRecordsRelation(OpenXmlRelation relation, string cacheFolder, int cacheId) {
			string rootFolder = cacheFolder.Replace("\\", "/");
			string pivotCacheRecordsFileName = CalculateRelationTargetCore(relation, rootFolder, string.Empty);
			if (!String.IsNullOrEmpty(pivotCacheRecordsFileName))
				pivotCacheRecordsRelations.Add(pivotCacheRecordsFileName, cacheId);
		}
		void RegisterPivotCacheExternalLinkPathRelations(OpenXmlRelation relation, int cacheId) {
			OpenXmlRelationCollection relations;
			if (pivotCacheIdToExternalLinkPathsTranslationTable.ContainsKey(cacheId)) {
				relations = pivotCacheIdToExternalLinkPathsTranslationTable[cacheId];
			}
			else {
				relations = new OpenXmlRelationCollection();
				pivotCacheIdToExternalLinkPathsTranslationTable.Add(cacheId, relations);
			}
			relations.Add(relation);
		}
		internal string TryGetPivotCacheExternalLinkPath(string relationId, int cacheId) {
			if (!pivotCacheIdToExternalLinkPathsTranslationTable.ContainsKey(cacheId))
				return null;
			OpenXmlRelationCollection cacheRelations = pivotCacheIdToExternalLinkPathsTranslationTable[cacheId];
			return cacheRelations.LookupRelationById(relationId).Target;
		}
		#endregion
		#region PivotTables
		void ImportPivotTables() {
			foreach (KeyValuePair<string, Worksheet> relation in PivotTableRelations) {
				string fileName = relation.Key;
				XmlReader reader = GetPackageFileXmlReader(fileName);
				if (reader != null && ReadToRootElement(reader, "pivotTableDefinition", SpreadsheetNamespace)) {
					PivotCache cache = TryGetModelPivotCache(fileName);
					if (cache != null) {
						PivotTable table = ImportPivotTableCore(reader, relation.Value, cache);
						if (pivotSelectionRelations.ContainsKey(fileName))
							pivotSelectionRelations[fileName].PivotTable = table;
					}
				}
			}
		}
		PivotTable ImportPivotTableCore(XmlReader reader, Worksheet sheet, PivotCache cache) {
			Worksheet previousSheet = this.CurrentWorksheet;
			this.CurrentWorksheet = sheet;
			PivotTableDefinitionDestination destination = new PivotTableDefinitionDestination(this, sheet, cache);
			try {
				destination.ProcessElementOpen(reader);
				ImportContent(reader, destination);
			}
			finally {
				this.CurrentWorksheet = previousSheet;
			}
			return destination.PivotTable;
		}
		PivotCache TryGetModelPivotCache(string pivotTableRelationFileName) {
			string pathToRels = Path.GetDirectoryName(pivotTableRelationFileName) + "/_rels/" + Path.GetFileName(pivotTableRelationFileName) + ".rels";
			OpenXmlRelationCollection pivotTableRelations = GetRelations(pathToRels);
			if (pivotTableRelations.Count == 0)
				return null;
			string cacheFileName = LookupRelationTargetByType(pivotTableRelations, RelsPivotCacheDefinitionNamepace, DocumentRootFolder, String.Empty);
			if (String.IsNullOrEmpty(cacheFileName) || !PivotCacheDefinitionRelations.ContainsKey(cacheFileName))
				return null;
			int cacheId = PivotCacheDefinitionRelations[cacheFileName];
			PivotCacheCollection caches = DocumentModel.PivotCaches;
			return caches.Count > cacheId ? caches[cacheId] : null;
		}
		#endregion
		#region Charts
		XmlReader internalReader;
		protected internal XmlReader InternalReader { get { return internalReader; } }
		protected internal virtual void ImportCharts() {
			foreach (KeyValuePair<string, Chart> chartRelation in ChartRelations) {
				string fileName = chartRelation.Key;
				XmlReader reader = GetPackageFileXmlReaderChain(fileName);
				if (reader != null) {
					string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
					this.documentRelationsStack.Push(GetRelations(pathToRels));
					try {
						ImportChartsCore(reader, chartRelation.Value);
					}
					finally {
						this.documentRelationsStack.Pop();
					}
				}
			}
		}
		protected internal XmlReader GetPackageFileXmlReaderChain(string fileName) {
			Stream stream = GetPackageFileStream(fileName);
			if (stream == null)
				return null;
			XmlReaderSettings settings = CreateXmlReaderSettings();
			settings.IgnoreWhitespace = false;
			this.internalReader = XmlReader.Create(stream, settings);
			settings = CreateXmlReaderSettings();
			settings.IgnoreWhitespace = true;
			return XmlReader.Create(internalReader, settings);
		}
		protected internal virtual void ImportChartsCore(XmlReader reader, Chart chart) {
			if (!ReadToRootElement(reader, "chartSpace", OpenXmlExporter.DrawingMLChartNamespace))
				return;
			Destination destination = new ChartSpaceDestination(this, chart);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region Drawings
		protected internal virtual void ImportDrawings() {
			foreach (KeyValuePair<string, InternalSheetBase> drawingsRelation in DrawingsRelations) {
				string fileName = drawingsRelation.Key;
				XmlReader reader = GetPackageFileXmlReader(drawingsRelation.Key);
				if (reader != null) {
					string pathToRels = Path.GetDirectoryName(fileName) + "/_rels/" + Path.GetFileName(fileName) + ".rels";
					this.documentRelationsStack.Push(GetRelations(pathToRels));
					try {
						ImportDrawingsCore(reader, drawingsRelation.Value);
					}
					finally {
						this.documentRelationsStack.Pop();
					}
				}
			}
		}
		protected internal virtual void ImportDrawingsCore(XmlReader reader, InternalSheetBase sheet) {
			if (!ReadToRootElement(reader, "wsDr", SpreadsheetDrawingNamespace))
				return;
			CurrentSheet = sheet;
			Destination destination = new DrawingDestination(this);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
			PackageImages.Clear();
			PackageFileStreams.Clear();
		}
		#endregion
		#region Connections
		protected internal virtual void ImportConnections() {
			string fileName = LookupRelationTargetByType(DocumentRelations, RelsConnectionsNamespace, DocumentRootFolder, "connections.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportConnectionsCore(reader);
		}
		protected internal virtual void ImportConnectionsCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "connections", SpreadsheetNamespace))
				return;
			ConnectionsDestination destination = new ConnectionsDestination(this);
			destination.ProcessElementOpen(reader);
		}
		#endregion
		#region QueryTable
		protected internal virtual void ImportQueryTables() {
			foreach (KeyValuePair<string, Table> queryTableRelation in QueryTableRelations) {
				XmlReader reader = GetPackageFileXmlReader(queryTableRelation.Key);
				if (reader != null)
					ImportQueryTableCore(reader, queryTableRelation.Value);
			}
		}
		protected internal virtual void ImportQueryTableCore(XmlReader reader, Table table) {
			if (!ReadToRootElement(reader, "queryTable", SpreadsheetNamespace))
				return;
			QueryTableDestination destination = new QueryTableDestination(this, table);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region XmlMaps
		protected internal virtual void ImportXmlMaps() {
			string fileName = LookupRelationTargetByType(DocumentRelations, RelsXmlMapsNamespace, DocumentRootFolder, "xmlMaps.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportXmlMapsCore(reader);
		}
		protected internal virtual void ImportXmlMapsCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "MapInfo", SpreadsheetNamespace))
				return;
			XmlMapsDestination destination = new XmlMapsDestination(this);
			destination.ProcessElementOpen(reader);
		}
		#endregion
		#region VbaProject
		protected internal virtual void ImportVbaProject() {
		}
		#endregion
		#region ImportDocumentCoreProperties
		protected internal virtual void ImportDocumentCoreProperties() {
			XmlReader reader = GetPackageFileXmlReader("docProps/core.xml");
			if (reader == null)
				return;
			if (!ReadToRootElement(reader, "coreProperties", CorePropertiesNamespace))
				return;
			Destination destination = new DocumentCorePropertiesDestination(this);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region ImportDocumentApplicationProperties
		protected internal virtual void ImportDocumentApplicationProperties() {
			XmlReader reader = GetPackageFileXmlReader("docProps/app.xml");
			if (reader == null)
				return;
			if (!ReadToRootElement(reader, "Properties", ExtendedPropertiesNamespace))
				return;
			Destination destination = new DocumentApplicationPropertiesDestination(this);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region ImportDocumentCustomProperties
		protected internal virtual void ImportDocumentCustomProperties() {
			XmlReader reader = GetPackageFileXmlReader("docProps/custom.xml");
			if (reader == null)
				return;
			if (!ReadToRootElement(reader, "Properties", CustomPropertiesNamespace))
				return;
			Destination destination = new DocumentCustomPropertiesDestination(this);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
		}
		#endregion
		#region CalculationChain
		protected internal virtual void ImportCalculationChain() {
			if (!DocumentModel.CalculationChain.Enabled)
				return;
			string fileName = LookupRelationTargetByType(DocumentRelations, RelsCalculationChainNamespace, DocumentRootFolder, "calcChain.xml");
			XmlReader reader = GetPackageFileXmlReader(fileName);
			if (reader != null)
				ImportCalculationChainCore(reader);
		}
		protected internal virtual void ImportCalculationChainCore(XmlReader reader) {
			if (!ReadToRootElement(reader, "calcChain", SpreadsheetNamespace))
				return;
			ImportContent(reader, new CalculationChainDestination(this));
		}
		#endregion
		#region CustomXmlPart
		protected internal virtual void ImportCustomXmlPart() {
			foreach (OpenXmlRelation relation in DocumentRelations) {
				if (relation.Type == OpenXmlExporter.RelsCustomXmlNamepace) {
					string path = relation.Target;
					if (path.StartsWith(".."))
						path = path.Substring(2);
					Stream stream = GetPackageFileStream(path);
					if (stream == null)
						continue;
					StreamReader streamReader = new StreamReader(stream);
					string customXmlContent = streamReader.ReadToEnd();
					ImportCustomXmlPartCore(customXmlContent);
				}
			}
		}
		protected internal virtual void ImportCustomXmlPartCore(string customXmlContent) {
			try {
				XmlReader reader = CreateXmlReaderByString(customXmlContent);
				ImportCustomXmlContent(reader);
			}
			catch { }
		}
		protected internal virtual void ImportCustomXmlContent(XmlReader reader) {
			Destination destination = new CustomXmlDestination(this);
			ImportContent(reader, destination);
		}
		#endregion
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this);
		}
		public override void ThrowInvalidFile() {
			throw new Exception(InvalidFileMessage);
		}
		public override void ThrowInvalidFile(string reason) {
			throw new Exception(InvalidFileMessage + ": " + reason);
		}
		#region Conversion and Parsing utilities
		public override bool ConvertToBool(string value) {
			if (value == "1" || value == "on" || value == "true" || value == "t")
				return true;
			if (value == "0" || value == "off" || value == "false" || value == "f")
				return false;
			ThrowInvalidFile(string.Format("Can't convert {0} to bool", value));
			return false;
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposePackageFiles();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal virtual Stream LookupPackageFileStreamByRelationId(string relationId, string rootFolder, bool cacheStream) {
			Stream stream;
			if (PackageFileStreams.TryGetValue(relationId, out stream)) {
				stream.Seek(0, SeekOrigin.Begin);
				return stream;
			}
			stream = CreatePackageFileStreamByRelationId(relationId, rootFolder);
			if (stream == null)
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			PackageFileStreams.Add(relationId, stream);
			return stream;
		}
		protected internal virtual Stream CreatePackageFileStreamByRelationId(string relationId, string rootFolder) {
			string fileName = LookupRelationTargetById(DocumentRelations, relationId, rootFolder, String.Empty);
			PackageFile file = GetPackageFile(fileName);
			if (file == null)
				return null;
			return file.SeekableStream;
		}
		protected internal virtual XmlReader CreateXmlReaderByString(string content) {
			StringReader stringReader = new StringReader(content);
			return XmlReader.Create(stringReader, CreateXmlReaderSettings());
		}
		protected void PrepareConditionalFormattingsCollection() {
			if (ConditionalFormattingCreatorCollection != null)
				ConditionalFormattingCreatorCollection.Clear();
		}
		protected void ApplyConditionalFormattingsCollection() {
			ConditionalFormattingCreatorData.ApplyCollection(ConditionalFormattingCreatorCollection);
		}
	}
	public static class SpreadsheetMLBaseImporterExtensions {
		public static DateTime ReadDateTime(this SpreadsheetMLBaseImporter importer, XmlReader reader, string attributeName, DateTime defaultValue) {
			string value = importer.ReadAttribute(reader, attributeName);
			if (!string.IsNullOrEmpty(value)) {
				DateTime dateTime;
				if (DateTime.TryParse(value, importer.DocumentModel.Culture, System.Globalization.DateTimeStyles.None, out dateTime))
					return dateTime;
			}
			return defaultValue;
		}
		public static DateTime? ReadDateTime(this SpreadsheetMLBaseImporter importer, XmlReader reader, string attributeName) {
			string value = importer.ReadAttribute(reader, attributeName);
			if (!string.IsNullOrEmpty(value)) {
				DateTime dateTime;
				if (DateTime.TryParse(value, importer.DocumentModel.Culture, System.Globalization.DateTimeStyles.None, out dateTime))
					return dateTime;
			}
			return null;
		}
		public static int ReadDifferentialFormatId(this SpreadsheetMLBaseImporter importer, XmlReader reader, string attributeName) {
			int id = importer.GetIntegerValue(reader, attributeName, Int32.MinValue);
			return importer.StyleSheet.GetDifferentialFormatIndex(id);
		}
	}
	public enum OpenXmlFormulaType {
		Normal,
		Array,
		DataTable,
		Shared
	}
}
