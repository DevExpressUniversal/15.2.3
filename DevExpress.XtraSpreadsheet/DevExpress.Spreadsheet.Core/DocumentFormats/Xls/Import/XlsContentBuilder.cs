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
using System.IO;
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsContentType
	public enum XlsContentType {
		None,
		WorkbookGlobals,
		Sheet,
		Chart,
		ChartSheet,
		MacroSheet,
		VisualBasicModule,
		Workspace,
		SheetCustomView,
		ChartCustomView,
		ChartSheetCustomView,
		MacroSheetCustomView,
		VisualBasicModuleCustomView,
		PivotCache
	}
	#endregion
	#region IXlsCommandFactory and proxy classes
	public interface IXlsCommandFactory {
		IXlsCommand CreateCommand(XlsReader reader);
	}
	class XlsBIFF8CommandFactoryProxy : IXlsCommandFactory {
		public IXlsCommand CreateCommand(XlsReader reader) {
			return XlsCommandFactory.CreateCommand(reader);
		}
	}
	#endregion
	#region XlsContentBuilder
	public class XlsContentBuilder : IBinaryContentBuilder, IDisposable, IXlsContentBuilder {
		#region Fields
		PackageFileReader packageFileReader;
		BinaryReader baseReader;
		XlsReader workbookReader;
		IXlsCommandFactory commandFactory;
		XlsDocumentImporterOptions options;
		int fontId;
		XlsImportStyleSheet styleSheet;
		Stack<XlsContentType> contentTypes = new Stack<XlsContentType>();
		bool firstSheetViewSelection = true;
		List<CellRangeInfo> selectionRanges = new List<CellRangeInfo>();
		Stack<IXlsDataCollector> dataCollectors = new Stack<IXlsDataCollector>();
		readonly XlsRPNContext rpnContext;
		int externSheetIndex;
		int currentSheetIndex = -1;
		int boundSheetCount;
		Dictionary<int, int> boundSheetTable = new Dictionary<int, int>();
		List<XlsContentDefinedNameExt> definedNameItems = new List<XlsContentDefinedNameExt>();
		readonly MergedCellsRangeOverlapChecker mergedCellsRangeChecker = new MergedCellsRangeOverlapChecker();
		readonly Stack<OfficeArtShapeGroupCoordinateSystem> coordinateSystems = new Stack<OfficeArtShapeGroupCoordinateSystem>();
		readonly Dictionary<string, UriBasedOfficeImage> uniqueUriBasedImages = new Dictionary<string, UriBasedOfficeImage>();
		ILogService logService;
		readonly Dictionary<int, OfficeArtShapeContainer> commentObjects = new Dictionary<int, OfficeArtShapeContainer>();
		readonly Dictionary<CellPosition, int> sharedFormulas = new Dictionary<CellPosition, int>();
		readonly List<XlsCondFormat> conditionalFormats = new List<XlsCondFormat>();
		XlsCondFormat activeConditionalFormat = null;
		readonly HashSet<string> uniqueTableNames = new HashSet<string>();
		readonly List<XlsNoteInfo> notes = new List<XlsNoteInfo>();
		Chart currentChart = null;
		readonly Stack<IXlsChartBuilder> chartBuilders = new Stack<IXlsChartBuilder>();
		IXlsChartBuilder savedChartBuilder = XlsChartEmptyBuilder.Instance;
		readonly List<XlsChartSeriesBuilder> seriesFormats = new List<XlsChartSeriesBuilder>();
		int currentViewIndex = 0;
		int currentSeriesDataIndex = 0;
		IXlsChartBuilder chartRootBuilder = XlsChartEmptyBuilder.Instance;
		readonly List<XlsChartAxisGroupBuilder> axisGroups = new List<XlsChartAxisGroupBuilder>();
		readonly List<XlsRealTimeData> rtdTopics = new List<XlsRealTimeData>();
		readonly HashSet<string> uniqueProtectedRangeNames = new HashSet<string>();
		string rc4Password;
		byte[] rc4Salt;
		readonly List<IXlsPivotCacheBuilder> pivotCacheBuilders = new List<IXlsPivotCacheBuilder>();
		IXlsPivotCacheBuilder currentPivotCacheBuilder = XlsPivotCacheEmptyBuilder.Instance;
		Dictionary<PivotTable, int> bindPivotTableId = new Dictionary<PivotTable, int>();
		#endregion
		internal XlsContentBuilder(XlsImportStyleSheet styleSheet, XlsDocumentImporterOptions options) {
			Guard.ArgumentNotNull(styleSheet, "styleSheet");
			Guard.ArgumentNotNull(options, "options");
			this.styleSheet = styleSheet;
			this.options = options;
			this.logService = DocumentModel.GetService<ILogService>();
			rpnContext = new XlsRPNContext(DocumentModel.DataContext);
			HasVBAProject = false;
			this.rc4Password = null;
			this.rc4Salt = null;
			this.commandFactory = new XlsBIFF8CommandFactoryProxy();
			OleObjectRange = null;
		}
		public XlsContentBuilder(Stream stream, XlsImportStyleSheet styleSheet, XlsDocumentImporterOptions options)
			: this(styleSheet, options) {
			OpenPackage(stream);
		}
		#region Properties
		protected internal XlsReader WorkbookReader { get { return this.workbookReader; } }
		protected internal IXlsCommandFactory CommandFactory { get { return this.commandFactory; } }
		public PackageFileReader FileReader { get { return this.packageFileReader; } }
		public DocumentModel DocumentModel { get { return this.StyleSheet.DocumentModel; } }
		public XlsDocumentImporterOptions Options { get { return this.options; } }
		public int FontId { get { return fontId; } set { fontId = value; } }
		public XlsImportStyleSheet StyleSheet { get { return styleSheet; } }
		public IXlsDataCollector DataCollector {
			get {
				if (dataCollectors.Count == 0)
					return null;
				return dataCollectors.Peek();
			}
		}
		public IXlsFormulaBuilder FormulaBuilder { get; set; }
		public XlsContentType ContentType {
			get {
				if (contentTypes.Count == 0)
					return XlsContentType.None;
				return contentTypes.Peek();
			}
		}
		protected internal int XFCount { get; set; }
		protected internal int XFCRC { get; set; }
		protected internal bool UseXFExt { get; set; }
		protected internal Worksheet CurrentSheet { get; set; }
		protected internal OfficeArtDrawingGroupContainer DrawingGroup { get; set; }
		protected internal XlsRPNContext RPNContext { get { return rpnContext; } }
		protected internal int ExternSheetIndex {
			get { return externSheetIndex; }
			set {
				ExternalWorkbook workbook = GetCurrentExternalWorkbook();
				if (workbook == null)
					ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ExternSheetIndex");
				else
					ValueChecker.CheckValue(value, 0, workbook.SheetCount - 1, "ExternSheetIndex");
				externSheetIndex = value;
			}
		}
		protected internal Dictionary<int, int> BoundSheetTable { get { return boundSheetTable; } }
		protected internal List<XlsContentDefinedNameExt> DefinedNameItems { get { return definedNameItems; } }
		protected internal Stack<OfficeArtShapeGroupCoordinateSystem> CoordinateSystems { get { return coordinateSystems; } }
		protected internal Dictionary<string, UriBasedOfficeImage> UniqueUriBasedImages { get { return uniqueUriBasedImages; } }
		protected internal Table ActiveTable { get; set; }
		protected internal Dictionary<int, OfficeArtShapeContainer> CommentObjects { get { return commentObjects; } }
		protected internal XlsCondFormat ActiveConditionalFormat { get { return activeConditionalFormat; } }
		protected internal Dictionary<CellPosition, int> SharedFormulas { get { return sharedFormulas; } }
		protected internal CellPosition SelectionActiveCell { get; set; }
		protected internal List<CellRangeInfo> SelectionRanges { get { return selectionRanges; } }
		protected internal OfficeArtDrawingObjectsContainer DrawingObjects { get; set; }
		protected internal List<XlsNoteInfo> Notes { get { return notes; } }
		protected internal bool HasVBAProject { get; set; }
		protected internal Chart CurrentChart {
			get { return currentChart; }
			set {
				currentChart = value;
				CurrentAxisGroup = null;
				currentViewIndex = 0;
				seriesFormats.Clear();
				axisGroups.Clear();
			}
		}
		protected internal bool ChartHasSecondaryAxisGroup { get; set; }
		protected internal AxisGroup CurrentAxisGroup { get; set; }
		protected internal List<XlsChartSeriesBuilder> SeriesFormats { get { return seriesFormats; } }
		protected internal int CurrentViewIndex { get { return currentViewIndex; } set { currentViewIndex = value; } }
		protected internal int CurrentSeriesDataIndex {
			get { return currentSeriesDataIndex; }
			set {
				ValueChecker.CheckValue(value, 0, 3, "CurrentSeriesDataIndex");
				currentSeriesDataIndex = value;
			}
		}
		protected internal IXlsChartBuilder ChartRootBuilder { get { return chartRootBuilder; } }
		protected internal bool HasChartBuilder { get { return chartBuilders.Count > 0; } }
		protected internal List<XlsChartAxisGroupBuilder> AxisGroups { get { return axisGroups; } }
		protected internal string PreviousRTDTopic { get; set; }
		protected internal List<XlsRealTimeData> RTDTopics { get { return rtdTopics; } }
		protected internal HashSet<string> UniqueProtectedRangeNames { get { return uniqueProtectedRangeNames; } }
		protected internal XlsBuildPivotView CurrentBuilderPivotView { get; set; }
		protected internal int NumberOfPivotFilterItems { get; set; }
		protected internal int NumberOfPivotCacheMappingItems { get; set; }
		protected internal bool ParamQueryNext { get; set; }
		protected internal int CurrentPivotCacheStreamId { get; set; }
		protected internal List<IXlsPivotCacheBuilder> PivotCacheBuilders { get { return pivotCacheBuilders; } }
		protected internal IXlsPivotCacheBuilder CurrentPivotCacheBuilder { get { return currentPivotCacheBuilder; } set { currentPivotCacheBuilder = value; } }
		protected internal Dictionary<PivotTable, int> BindPivotTableId { get { return bindPivotTableId; } }
		protected internal CellRangeInfo OleObjectRange { get; set; }
		#endregion
		public void LogMessage(LogCategory category, string message) {
			if (this.logService != null) {
				this.logService.LogMessage(category, message);
			}
		}
		public void PushDataCollector(IXlsDataCollector collector) {
			Guard.ArgumentNotNull(collector, "collector");
			dataCollectors.Push(collector);
		}
		public void PopDataCollector() {
			if (dataCollectors.Count > 0)
				dataCollectors.Pop();
		}
		public void ClearDataCollectors() {
			dataCollectors.Clear();
		}
		public void SetupSheetProtection() {
			if (CurrentSheet == null) return;
			WorksheetProtectionOptions protection = CurrentSheet.Properties.Protection;
			if (!protection.SheetLocked) {
				protection.BeginUpdate();
				try {
					protection.AutoFiltersLocked = false;
					protection.DeleteColumnsLocked = false;
					protection.DeleteRowsLocked = false;
					protection.FormatCellsLocked = false;
					protection.FormatColumnsLocked = false;
					protection.FormatRowsLocked = false;
					protection.InsertColumnsLocked = false;
					protection.InsertHyperlinksLocked = false;
					protection.InsertRowsLocked = false;
					protection.PivotTablesLocked = false;
					protection.SortLocked = false;
				}
				finally {
					protection.EndUpdate();
				}
			}
		}
		#region Current content
		public void StartContent(XlsContentType contentType) {
			if (contentType == XlsContentType.WorkbookGlobals) {
				XFCount = 0;
				XFCRC = 0;
				UseXFExt = false;
			}
			if (contentType == XlsContentType.Sheet) {
				this.mergedCellsRangeChecker.Clear();
			}
			contentTypes.Push(contentType);
		}
		public void StartContent(XlsSubstreamType substreamType) {
			StartContent(GetContentTypeBySubstreamType(substreamType));
		}
		public void EndContent() {
			contentTypes.Pop();
		}
		XlsContentType GetContentTypeBySubstreamType(XlsSubstreamType dataType) {
			switch (dataType) {
				case XlsSubstreamType.WorkbookGlobals:
					return XlsContentType.WorkbookGlobals;
				case XlsSubstreamType.Sheet:
					return XlsContentType.Sheet;
				case XlsSubstreamType.MacroSheet:
					return XlsContentType.MacroSheet;
				case XlsSubstreamType.VisualBasicModule:
					return XlsContentType.VisualBasicModule;
				case XlsSubstreamType.Workspace:
					return XlsContentType.Workspace;
				case XlsSubstreamType.Chart:
					if (ContentType == XlsContentType.Sheet)
						return XlsContentType.Chart;
					return XlsContentType.ChartSheet;
			}
			return XlsContentType.None;
		}
		#endregion
		void OpenPackage(Stream stream) {
			this.packageFileReader = new PackageFileReader(stream);
			this.baseReader = this.packageFileReader.GetCachedPackageFileReader(XlsStreams.WorkbookStreamName);
			if (this.baseReader == null) {
					XlsImporter.ThrowInvalidXlsFile("Workbook stream not found!");
			}
			this.workbookReader = new XlsReader(this.baseReader);
		}
		public void ReadWorkbookSubstreams() {
			while (WorkbookReader.Position != WorkbookReader.StreamLength) {
				XlsCommandBeginOfSubstream beginOfSubstream = commandFactory.CreateCommand(WorkbookReader) as XlsCommandBeginOfSubstream;
				if (beginOfSubstream == null)
					continue;
				beginOfSubstream.Read(WorkbookReader, this);
				beginOfSubstream.Execute(this);
				ReadSubstream(beginOfSubstream.SubstreamType);
			}
		}
		public void ReadSubstream() {
			XlsCommandBeginOfSubstream beginOfSubstream = commandFactory.CreateCommand(WorkbookReader) as XlsCommandBeginOfSubstream;
			beginOfSubstream.Read(WorkbookReader, this);
			beginOfSubstream.Execute(this);
			ReadSubstream(beginOfSubstream.SubstreamType);
		}
		public void ReadSubstream(XlsSubstreamType dataType) {
			switch (dataType) {
				case XlsSubstreamType.Sheet:
					ReadSheet();
					break;
				default:
					ReadSubstreamCommands();
					break;
			}
		}
		void ReadSheet() {
			sharedFormulas.Clear();
			this.currentSheetIndex++;
			this.firstSheetViewSelection = true;
			SelectionActiveCell = new CellPosition();
			SelectionRanges.Clear();
			UniqueProtectedRangeNames.Clear();
			CurrentSheet = GetCurrentSheet();
			ReadSubstreamCommands();
			CurrentSheet = null;
		}
		Worksheet GetCurrentSheet() {
			if (this.currentSheetIndex < 0 || this.currentSheetIndex >= DocumentModel.SheetCount)
				return null;
			return DocumentModel.Sheets[this.currentSheetIndex];
		}
		void ReadSubstreamCommands() {
			int level = this.contentTypes.Count;
			while (WorkbookReader.Position != WorkbookReader.StreamLength) {
				IXlsCommand command = commandFactory.CreateCommand(WorkbookReader);
				command.Read(WorkbookReader, this);
				command.Execute(this);
				if (command is XlsCommandEndOfSubstream) {
					if (ContentType == XlsContentType.None) return;
					if (this.contentTypes.Count < level) return;
				}
			}
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.workbookReader != null) {
					((IDisposable)this.workbookReader).Dispose();
					this.workbookReader = null;
				}
				if (this.packageFileReader != null) {
					this.packageFileReader.Dispose();
					this.packageFileReader = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void ThrowInvalidFile(string reason) {
			XlsImporter importer = StyleSheet.Importer as XlsImporter;
			if (importer != null)
				importer.ThrowInvalidFile(reason);
			else
				StyleSheet.Importer.ThrowInvalidFile();
		}
		public void SetupRC4Decryptor(string password, byte[] salt) {
			this.rc4Password = password;
			this.rc4Salt = salt;
			((IDisposable)this.workbookReader).Dispose();
			this.workbookReader = new XlsRC4EncryptedReader(this.baseReader, password, salt);
		}
		#region Selection
		void AddSelection(CellPosition activeCell, CellRangeBase selectionRange) {
			Worksheet sheet = CurrentSheet;
			if (this.firstSheetViewSelection) {
				sheet.Selection.SetSelectionCore(activeCell, selectionRange, true);
				this.firstSheetViewSelection = false;
			}
			else {
				sheet.Selection.AddSelectionCore(selectionRange, true);
			}
		}
		void AddSelection(CellPosition activeCell) {
			Worksheet sheet = CurrentSheet;
			if (this.firstSheetViewSelection) {
				sheet.Selection.SetSelection(activeCell);
				this.firstSheetViewSelection = false;
			}
		}
		protected internal void CreateSelection() {
			if (CurrentSheet == null) return;
			int count = SelectionRanges.Count;
			if(count > 0) {
				bool isValidActiveCell = false;
				for(int i = 0; i < count && !isValidActiveCell; i++) {
					CellRangeInfo range = SelectionRanges[i];
					isValidActiveCell = range.ContainsCell(SelectionActiveCell);
				}
				if(!isValidActiveCell)
					SelectionActiveCell = SelectionRanges[0].First;
				for(int i = 0; i < count; i++) {
					CellRangeInfo range = SelectionRanges[i];
					AddSelection(SelectionActiveCell, XlsCellRangeFactory.CreateCellRange(CurrentSheet, range.First, range.Last));
				}
			}
			else
				AddSelection(SelectionActiveCell);
		}
		#endregion
		#region SupBooks/External
		public void RegisterSupBook(XlsSupBookInfo info) {
			if (info.LinkType == XlsSupportingLinkType.ExternalWorkbook) {
				ExternalLink link = new ExternalLink(DocumentModel);
				ExternalWorkbook workbook = link.Workbook;
				workbook.FilePath = XlsVirtualPath.GetFilePath(info.VirtualPath, DocumentModel.DocumentSaveOptions);
				foreach (string name in info.SheetNames) {
					ExternalWorksheet sheet = new ExternalWorksheet(workbook, name);
					workbook.Sheets.Add(sheet);
				}
				DocumentModel.ExternalLinks.InnerList.Add(link);
				RPNContext.SupBooksTable.Add(RPNContext.SupBooks.Count, DocumentModel.ExternalLinks.Count);
			}
			else if (info.LinkType == XlsSupportingLinkType.DataSource) {
				string ddeServer = XlsVirtualPath.GetDdeServer(info.VirtualPath);
				string ddeTopic = XlsVirtualPath.GetDdeTopic(info.VirtualPath);
				if (string.IsNullOrEmpty(ddeServer) || string.IsNullOrEmpty(ddeTopic))
					ThrowInvalidFile("Invalid DDE server|topic");
				DdeExternalWorkbook ddeWorkbook = new DdeExternalWorkbook(DocumentModel);
				ddeWorkbook.DdeServiceName = ddeServer;
				ddeWorkbook.DdeServerTopic = ddeTopic;
				ExternalLink link = new ExternalLink(ddeWorkbook);
				DocumentModel.ExternalLinks.InnerList.Add(link);
				RPNContext.SupBooksTable.Add(RPNContext.SupBooks.Count, DocumentModel.ExternalLinks.Count);
			}
			else if (info.LinkType == XlsSupportingLinkType.AddIn) {
			}
			RPNContext.SupBooks.Add(info);
		}
		public XlsSupBookInfo GetLastSupBook() {
			int count = RPNContext.SupBooks.Count;
			if (count == 0)
				return null;
			return RPNContext.SupBooks[count - 1];
		}
		protected internal ExternalWorkbook GetCurrentExternalWorkbook() {
			XlsSupBookInfo supBook = GetLastSupBook();
			if (supBook == null)
				return null;
			if (supBook.LinkType != XlsSupportingLinkType.ExternalWorkbook)
				return null;
			ExternalLinkCollection links = DocumentModel.ExternalLinks;
			if (links.Count == 0)
				return null;
			return links[links.Count - 1].Workbook;
		}
		protected internal ExternalWorksheet GetCurrentExternalWorkSheet() {
			ExternalWorkbook workbook = GetCurrentExternalWorkbook();
			if (workbook == null)
				return null;
			if (externSheetIndex >= workbook.SheetCount)
				return null;
			return workbook.Sheets[externSheetIndex];
		}
		protected internal DdeExternalWorkbook GetCurrentDdeExternalWorkbook() {
			XlsSupBookInfo supBook = GetLastSupBook();
			if (supBook == null)
				return null;
			if (supBook.LinkType != XlsSupportingLinkType.DataSource)
				return null;
			ExternalLinkCollection links = DocumentModel.ExternalLinks;
			if (links.Count == 0)
				return null;
			return links[links.Count - 1].Workbook as DdeExternalWorkbook;
		}
		protected internal void RegisterSheetInfo(SheetType sheetType, string name, SheetVisibleState visibleState) {
			if (sheetType == SheetType.RegularWorksheet) {
				Worksheet sheet = new Worksheet(DocumentModel, name);
				sheet.VisibleState = visibleState;
				DocumentModel.Sheets.Add(sheet);
				BoundSheetTable.Add(this.boundSheetCount, DocumentModel.SheetCount - 1);
			}
			this.boundSheetCount++;
			RPNContext.SheetNames.Add(name);
		}
		public string RegisterExternalDefinedName(string name, int sheetIndex, ParsedExpression formula) {
			ExternalWorkbook workbook = GetCurrentExternalWorkbook();
			if (workbook != null) {
				string scopeName = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_ExternalWorkbook) + " \"" + workbook.FilePath + "\"";
				DefinedNameCollectionBase definedNames;
				if (sheetIndex == 0)
					definedNames = workbook.DefinedNames;
				else {
					ExternalWorksheet sheet = workbook.Sheets[sheetIndex - 1];
					definedNames = sheet.DefinedNames;
					scopeName += ", \"" + sheet.Name + "\"";
				}
				string originalName = name;
				name = XlsDefinedNameHelper.ReplaceInvalidChars(name);
				while (!WorkbookDataContext.IsIdent(name))
					name = "_" + name;
				if (!definedNames.Contains(name)) {
					if (originalName != name) {
						string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefinedNameHasBeenChanged), originalName, scopeName, name);
						LogMessage(LogCategory.Info, message);
					}
					string description = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_DefinedName) + " \"{0}\" ({1})", name, scopeName);
					RPNContext.PushCurrentSubject(description);
					try {
						ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(formula, RPNContext);
						ExternalDefinedName item = new ExternalDefinedName(name, workbook, expression, sheetIndex - 1);
						definedNames.AddWithoutHistoryAndNotifications(item);
					}
					finally {
						RPNContext.PopCurrentSubject();
					}
				}
			}
			return name;
		}
		#endregion
		public void RegisterDefinedNames() {
			foreach (XlsContentDefinedNameExt item in definedNameItems)
				item.Apply(this);
		}
		public bool IsNotOverlapped(CellRangeInfo range) {
			return this.mergedCellsRangeChecker.IsNotOverlapped(range);
		}
		public void SetupWorkbookWindowProperties() {
			foreach (WorkbookWindowProperties item in DocumentModel.Properties.WorkbookWindowPropertiesList) {
				SetupWorkbookWindowProperties(item);
			}
		}
		void SetupWorkbookWindowProperties(WorkbookWindowProperties properties) {
			int sheetsCount = DocumentModel.SheetCount;
			properties.BeginUpdate();
			try {
				properties.SelectedTabIndex = BoundSheetTable.ContainsKey(properties.SelectedTabIndex) ? BoundSheetTable[properties.SelectedTabIndex] : 0;
				properties.FirstDisplayedTabIndex = BoundSheetTable.ContainsKey(properties.FirstDisplayedTabIndex) ? BoundSheetTable[properties.FirstDisplayedTabIndex] : 0;
				if (properties.FirstDisplayedTabIndex > properties.SelectedTabIndex)
					properties.FirstDisplayedTabIndex = properties.SelectedTabIndex;
				if ((properties.SelectedTabIndex + properties.SelectedTabsCount) > sheetsCount)
					properties.SelectedTabsCount = Math.Min(1, sheetsCount - properties.SelectedTabIndex);
			}
			finally {
				properties.EndUpdate();
			}
		}
		public void CleanupIncompliantSharedFormulas() {
			if (CurrentSheet == null)
				return;
			SharedFormulaCollection sharedFormulas = CurrentSheet.SharedFormulas;
			int count = sharedFormulas.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (!sharedFormulas[i].IsFormulaCompliant)
					sharedFormulas.RemoveCore(i);
			}
		}
		protected internal void ClearConditionalFormats() {
			conditionalFormats.Clear();
			activeConditionalFormat = null;
		}
		protected internal void AddConditionalFormat(XlsCondFormat item) {
			conditionalFormats.Add(item);
			activeConditionalFormat = item;
		}
		protected internal void SetActiveConditionalFormat(int id) {
			foreach (XlsCondFormat item in conditionalFormats) {
				if (item.Id == id) {
					activeConditionalFormat = item;
					return;
				}
			}
			activeConditionalFormat = null;
		}
		protected internal void RegisterConditionalFormats() {
			if (CurrentSheet == null) return;
			foreach (XlsCondFormat item in conditionalFormats)
				item.Register(this);
		}
		protected internal void CheckTableNames() {
			if (CurrentSheet == null) return;
			foreach (Table table in CurrentSheet.Tables) {
				string uniqueName = GetUniqueTableName(table.Name);
				if(uniqueName != table.Name)
					table.SetDisplayNameCore(uniqueName);
			}
		}
		string GetUniqueTableName(string name) {
			string result = name;
			int modifier = 1;
			while(uniqueTableNames.Contains(result)) {
				result = string.Format("{0}_{1}", name, modifier);
				modifier++;
			}
			uniqueTableNames.Add(result);
			return result;
		}
		public void ReadCustomXmlMappings() {
			BinaryReader reader = this.packageFileReader.GetCachedPackageFileReader(XlsStreams.XmlStreamName);
			if(reader != null) {
				byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
				using(MemoryStream ms = new MemoryStream(data, false)) {
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.IgnoreComments = true;
					settings.IgnoreWhitespace = true;
					using(XmlReader xmlReader = XmlReader.Create(ms, settings)) {
						xmlReader.MoveToContent();
						string content = xmlReader.ReadOuterXml();
						if(content.StartsWith("<MapInfo "))
							DocumentModel.XmlMapsContent = XlsXmlMapsHelper.InsertSpreadsheetMLNameSpace(content);
					}
				}
			}
		}
		protected internal void CreateDrawingObjects() {
			if(DrawingObjects != null) {
				XlsMsoDrawingHelper.HandleShapeGroup(this, DrawingObjects.ShapeGroup);
				DrawingObjects = null;
			}
		}
		protected internal void CreateNotes() {
			foreach(XlsNoteInfo note in Notes)
				note.Register(this);
			Notes.Clear();
			CommentObjects.Clear();
		}
		public void ReadVbaProjectContent() {
			if(!HasVBAProject) return;
			IEnumerable<string> vbaFiles = this.packageFileReader.EnumerateFiles(XlsStreams.VbaProjectRootStorageName, false);
			foreach(string entry in vbaFiles) {
				BinaryReader reader = this.packageFileReader.GetCachedPackageFileReader(entry);
				if(reader != null) {
					string name = entry.Substring(XlsStreams.VbaProjectRootStorageName.Length + 1);
					byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
					DocumentModel.VbaProjectContent.Items.Add(new VbaProjectItem(name, data));
				}
				else {
					DocumentModel.VbaProjectContent.Clear();
					return;
				}
			}
		}
		public void ReadOlePropertiesContent(string streamName) {
			BinaryReader reader = this.packageFileReader.GetCachedPackageFileReader(streamName);
			if (reader != null) {
				OlePropertyStreamContent content = OlePropertyStreamContent.FromStream(reader);
				content.Execute(DocumentModel);
			}
		}
		#region PivotCache
		public void GeneratePivotItemsAndWholeRange() {
			for (int i = 0; i < DocumentModel.Sheets.Count; ++i) {
				Worksheet sheet = DocumentModel.Sheets[i];
				for (int j = 0; j < sheet.PivotTables.Count; ++j) {
					PivotTable pivotTable = sheet.PivotTables[j];
					pivotTable.Location.UpdateRangesWithoutHistory(pivotTable);
					PivotGenerateItemsCommand command = new PivotGenerateItemsCommand(pivotTable);
					command.GenerateItemsAfterImport();
				}
			}
		}
		public void CreatePivotCaches() {
			List<int> excludedCache = new List<int>();
			int cacheIndex = 0;
			foreach (XlsPivotCacheBuilderBase builder in pivotCacheBuilders) {
				currentPivotCacheBuilder = builder;
				currentPivotCacheBuilder.Execute(this);
				if (builder.CalculatedFields.Count != 0)
					excludedCache.Add(cacheIndex);
				cacheIndex++;
			}
			foreach (PivotTable pivotTable in BindPivotTableId.Keys){
				int index = BindPivotTableId[pivotTable];
				if (!excludedCache.Contains(index))
					pivotTable.SetCacheCore(DocumentModel.PivotCaches[index]);
			}
			foreach (Worksheet sheet in DocumentModel.Sheets)
				for (int index = sheet.PivotTables.Count - 1; index >= 0; index--)
					if (sheet.PivotTables[index].Cache == null)
						sheet.PivotTables.RemoveAtCore(index);
			for (int index = excludedCache.Count - 1; index >= 0; index--)
				DocumentModel.PivotCaches.RemoveAt(excludedCache[index]);
		}
		public void ReadPivotCacheContent(string streamName) {
			BinaryReader reader = this.packageFileReader.GetCachedPackageFileReader(streamName);
			if (reader == null)
				ThrowInvalidFile(string.Format("Pivot cache stream \"{0}\" not found!", streamName));
			using (XlsReader pivotCacheReader = string.IsNullOrEmpty(rc4Password) ? new XlsReader(reader) : new XlsRC4EncryptedReader(reader, rc4Password, rc4Salt)) {
				StartContent(XlsContentType.PivotCache);
				while (pivotCacheReader.Position < pivotCacheReader.StreamLength && ContentType == XlsContentType.PivotCache) {
					IXlsCommand command = commandFactory.CreateCommand(pivotCacheReader);
					command.Read(pivotCacheReader, this);
					command.Execute(this);
				}
				if (ContentType == XlsContentType.PivotCache)
					EndContent();
			}
		}
		#endregion
		#region CustomXMLData
		public void ReadCustomXmlData() {
			IEnumerable<string> customXmlStreams = this.packageFileReader.EnumerateFiles(XlsStreams.CustomXMlStreamName, false);
			foreach(string streamName in customXmlStreams) {
				if(!streamName.EndsWith(@"\Item"))
					continue;
				BinaryReader reader = this.packageFileReader.GetCachedPackageFileReader(streamName);
				if(reader == null)
					continue;
				byte[] data = reader.ReadBytes((int) reader.BaseStream.Length);
				using(MemoryStream ms = new MemoryStream(data, false)) {
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.IgnoreComments = true;
					settings.IgnoreWhitespace = true;
					using(XmlReader xmlReader = XmlReader.Create(ms, settings)) {
						using(OpenXmlImporter importer = new OpenXmlImporter(this.DocumentModel, new OpenXmlDocumentImporterOptions())) {
							importer.ImportCustomXmlContent(xmlReader);
						}
					}					
				}
			}
		}
		#endregion
		#region ChartBuilders
		protected internal IXlsChartBuilder CurrentChartBuilder {
			get {
				if (chartBuilders.Count == 0)
					return XlsChartEmptyBuilder.Instance;
				return chartBuilders.Peek();
			}
		}
		protected internal void PutChartBuilder(IXlsChartBuilder chartBuilder) {
			this.savedChartBuilder = chartBuilder != null ? chartBuilder : XlsChartEmptyBuilder.Instance;
		}
		protected internal void PushChartBuilder() {
			if (chartBuilders.Count == 0)
				chartRootBuilder = savedChartBuilder;
			chartBuilders.Push(savedChartBuilder);
			savedChartBuilder = XlsChartEmptyBuilder.Instance;
		}
		protected internal void PopChartBuilder() {
			chartBuilders.Pop();
			if (chartBuilders.Count == 0)
				chartRootBuilder = XlsChartEmptyBuilder.Instance;
		}
		protected internal void SetupChartDataCache() {
			if (CurrentChart == null)
				return;
			CurrentChart.BeginUpdate();
			try {
				foreach (XlsChartSeriesBuilder series in SeriesFormats)
					series.SetupDataCache(this);
			}
			finally {
				CurrentChart.EndUpdate();
			}
			CurrentChart.UpdateDataReferences();
		}
		protected internal List<XlsChartCachedValue> GetCurrentDataCache(int seriesIndex) {
			if (seriesIndex >= 0 && seriesIndex < SeriesFormats.Count) {
				XlsChartSeriesBuilder series = SeriesFormats[seriesIndex];
				if (currentSeriesDataIndex == 2)
					return series.ArgumentCache;
				if (currentSeriesDataIndex == 1)
					return series.ValueCache;
				if (currentSeriesDataIndex == 3)
					return series.BubbleSizeCache;
			}
			return null;
		}
		protected internal void SetupSeriesFormats() {
			for (int i = 0; i < SeriesFormats.Count; i++) {
				XlsChartSeriesBuilder seriesFormat = SeriesFormats[i];
				seriesFormat.SeriesIndex = i;
				if(seriesFormat.ViewIndex != -1)
					seriesFormat.GetOrderFromDataFormat();
			}
		}
		#endregion
		#region RealTimeData
		public void SetupRealTimeData() {
			if (RTDTopics.Count <= 0)
				return;
			foreach (XlsRealTimeData topicDefinition in RTDTopics) {
				RealTimeDataApplication application = DocumentModel.RealTimeDataManager.PrepareApplication(topicDefinition.ApplicationId, topicDefinition.ServerName, false);
				RealTimeTopic topic = application.AddTopic(topicDefinition.Parameters.ToArray());
				topic.ReferencedCells.AddRange(GetRTDCells(topicDefinition));
				topic.CachedValue = topicDefinition.Value.GetVariantValue();
			}
		}
		List<ICell> GetRTDCells(XlsRealTimeData topicDefinition) {
			List<ICell> result = new List<ICell>();
			foreach (XlsRTDCell item in topicDefinition.Cells) {
				int sheetIndex = BoundSheetTable[item.SheetIndex];
				Worksheet sheet = DocumentModel.Sheets[sheetIndex];
				result.Add(sheet[item.ColumnIndex, item.RowIndex]);
			}
			return result;
		}
		#endregion
	}
	#endregion
}
