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
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Export;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Internal;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Data.Helpers;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Utils.Controls;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
#if !SL && !DXPORTABLE
using System.Security.Permissions;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region IModelWorkbook
	public interface IModelWorkbook {
		int CreateNextSheetId();
		int SheetCount { get; }
		int ContentVersion { get; }
		WorkbookDataContext DataContext { get; }
		CultureInfo Culture { get; set; }
		CalculationChain CalculationChain { get; }
		SharedStringTable SharedStringTable { get; }
		StyleSheet StyleSheet { get; }
		DefinedNameCollectionBase DefinedNames { get; }
		SheetDefinitionCollection SheetDefinitions { get; }
		int IncrementContentVersion();
		IWorksheet GetSheetByName(string name);
		IWorksheet GetSheetById(int sheetId);
		IWorksheet GetSheetByIndex(int index);
		List<IWorksheet> GetSheets(string startSheetName, string endSheetName);
		DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook GetExternalWorkbookByIndex(int index);
		DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook GetExternalWorkbookByName(string name);
		List<IWorksheet> GetSheets();
		Table GetTableByName(string name);
		void CheckDefinedName(string name, int scopedSheetId);
	}
	#endregion
	public interface IDocumentModelSkinColorProvider {
		Color SkinGridlineColor { get; }
		Color SkinForeColor { get; }
		Color SkinBackColor { get; }
	}
	public interface IDocumentModelPartWithApplyChanges : IDocumentModelPart {
		DocumentModel Workbook { get; }
		void ApplyChanges(DocumentModelChangeActions actions);
	}
	#region DocumentModel
	public partial class DocumentModel : DocumentModelBase<DocumentFormat>, IModelWorkbook, IDocumentModelPartWithApplyChanges, IDocumentModelSkinColorProvider, IDocumentPropertiesContainer {
		#region Fields
		static readonly BorderLineRepository defaultBorderLineRepository = new BorderLineRepository();
		WorksheetCollection sheets;
		DefinedNameCollection definedNames;
		ExternalLinkCollection externalLinks;
		CommentAuthorCollection commentAuthors;
		VbaProject vbaProjectContent;
		PivotCacheCollection pivotCaches;
		InternalAPI internalAPI;
		CalculationChain calculationChain;
		StyleSheet styleSheet;
		MailOptions mailOptions;
		WorkbookProperties properties;
		ModelDocumentCoreProperties documentCoreProperties;
		ModelDocumentApplicationProperties documentApplicationProperties;
		ModelDocumentCustomProperties documentCustomProperties;
		SharedStringTable sharedStringTable;
		WorkbookCapabilitiesOptions documentCapabilities;
		WorkbookSaveOptions documentSaveOptions;
		WorkbookImportOptions documentImportOptions;
		WorkbookExportOptions documentExportOptions;
		SpreadsheetBehaviorOptions behaviorOptions;
		SpreadsheetPivotTableFieldListOptions pivotTableFieldListOptions;
		SpreadsheetViewOptions viewOptions;
		SpreadsheetPrintOptions printOptions;
		WorkbookEventOptions eventOptions;
		SpreadsheetProtectionOptions protectionOptions;
		CultureInfo culture;
		WorkbookDataContext dataContext;
		DocumentCache cache;
		UIBorderInfoRepository uiBorderInfoRepository;
		DocumentModelDeferredChanges deferredChanges;
		SheetDefinitionCollection sheetDefinitions;
		FontCache pixelFontCache;
		IWorksheetNameCreationService oldWorksheetNameCreationService;
		ICustomFunctionProvider customFunctionProvider;
		ICustomFunctionProvider builtInOverridesFunctionProvider;
		readonly Dictionary<short, CellBatchUpdateHelper> cellTransactions = new Dictionary<short, CellBatchUpdateHelper>();
		int beginUpdateFromUICounter;
		int transactionVersion;
		int contentVersion;
		int sheetCounter;
		int cycleReferenceCellsCounter = 0;
		short cellTransactionCount = 0;
		string connectionsContent;
		string xmlMapsContent;
		string currentAuthor;
		List<string> queryTablesContent;
		bool checkForCycleReferences = true;
		bool suppressCellValueAssignment;
		bool suppressCellCreation = false;
		bool isHistoryEnabled;
		bool innerModified;
		bool recalculateAfterLoad;
		bool suppressRecalculateOnLoad;
		bool suppressAutoStartCalculation;
		RealTimeDataManager realTimeDataManager;
		bool hasCircularReferences;
		bool iterative;
		bool visualControlAssigned;
		DataComponentInfoList dataComponentInfos;
		object mailMergeDataSource;
		MailMergeParametersCollection mailMergeParameters;
		SpreadsheetDataControllerAdapter mailMergeDataAdapter;
		DefinedNamesRefencesCacheManager definedNamesRefencesCacheManager;
		CustomFunctionsDescriptions customFunctionsDescriptions;
		CopiedRangeProvider copiedRangeProvider;
		IClipboard clipboard;
		bool suppressResetingCopiedRange = false;
		#endregion
		public DocumentModel()
			: base() {
			Initialize();
			this.culture = CultureInfo.CurrentCulture;
			this.dataContext = new WorkbookDataContext(this);
			this.currentAuthor = GetUserName();
			IsSetContentMode = false;
			this.copiedRangeProvider = new CopiedRangeProvider();
			Clipboard = new SpreadsheetClipboard(this as IServiceProvider);
		}
		#region Properties
		public static BorderLineRepository DefaultBorderLineRepository { get { return defaultBorderLineRepository; } }
		public override IDocumentModelPart MainPart { get { return this; } }
		public InternalAPI InternalAPI { get { return internalAPI; } protected set { internalAPI = value; } }
		public bool HasCircularReferences { get { return hasCircularReferences || cycleReferenceCellsCounter > 0; } set { hasCircularReferences = value; } }
		public bool Iterative { get { return iterative; } set { iterative = value; } }
		public bool Modified {
			get {
				if (innerModified)
					return true;
				return History.Modified;
			}
			set {
				History.Modified = value;
				if (!value)
					ChangeInnerModified(false);
			}
		}
		public Worksheet ActiveSheet {
			get {
				int activeSheetIndex = ActiveSheetIndex;
				if (activeSheetIndex >= Sheets.Count)
					return null;
				return Sheets[activeSheetIndex];
			}
			set {
				if (!Object.ReferenceEquals(value.Workbook, this))
					return;
				int index = Sheets.IndexOf(value);
				if (index < 0)
					return;
				ActiveSheetIndex = index;
			}
		}
		public int ActiveSheetIndex {
			get {
				List<WorkbookWindowProperties> windowProperties = Properties.WorkbookWindowPropertiesList;
				if (windowProperties.Count == 0)
					return 0;
				return Math.Max(0, Math.Min(Sheets.Count - 1, windowProperties[0].SelectedTabIndex));
			}
			set {
				SetActiveSheetIndex(value, true);
			}
		}
		public virtual WorksheetCollection Sheets {[System.Diagnostics.DebuggerStepThrough] get { return this.sheets; } }
		public virtual StyleSheet StyleSheet {[System.Diagnostics.DebuggerStepThrough] get { return styleSheet; } }
		public MailOptions MailOptions {[System.Diagnostics.DebuggerStepThrough] get { return this.mailOptions; } set { this.mailOptions = value; } }
		public virtual WorkbookProperties Properties {[System.Diagnostics.DebuggerStepThrough] get { return properties; } set { properties = value; } }
		public virtual ModelDocumentCoreProperties DocumentCoreProperties {[System.Diagnostics.DebuggerStepThrough] get { return documentCoreProperties; } }
		public virtual ModelDocumentApplicationProperties DocumentApplicationProperties {[System.Diagnostics.DebuggerStepThrough] get { return documentApplicationProperties; } }
		public virtual ModelDocumentCustomProperties DocumentCustomProperties {[System.Diagnostics.DebuggerStepThrough] get { return documentCustomProperties; } }
		public virtual SharedStringTable SharedStringTable {[System.Diagnostics.DebuggerStepThrough] get { return sharedStringTable; } }
		public virtual WorkbookSaveOptions DocumentSaveOptions {[System.Diagnostics.DebuggerStepThrough] get { return documentSaveOptions; } }
		public virtual WorkbookCapabilitiesOptions DocumentCapabilities {[System.Diagnostics.DebuggerStepThrough] get { return documentCapabilities; } }
		public virtual WorkbookImportOptions DocumentImportOptions {[System.Diagnostics.DebuggerStepThrough] get { return documentImportOptions; } }
		public virtual WorkbookExportOptions DocumentExportOptions {[System.Diagnostics.DebuggerStepThrough] get { return documentExportOptions; } }
		public virtual SpreadsheetBehaviorOptions BehaviorOptions {[System.Diagnostics.DebuggerStepThrough] get { return behaviorOptions; } }
		public virtual SpreadsheetPivotTableFieldListOptions PivotTableFieldListOptions {[System.Diagnostics.DebuggerStepThrough] get { return pivotTableFieldListOptions; } }
		public virtual SpreadsheetViewOptions ViewOptions {[System.Diagnostics.DebuggerStepThrough] get { return viewOptions; } }
		public virtual SpreadsheetPrintOptions PrintOptions {[System.Diagnostics.DebuggerStepThrough] get { return printOptions; } }
		public virtual WorkbookEventOptions EventOptions {[System.Diagnostics.DebuggerStepThrough] get { return eventOptions; } }
		public virtual SpreadsheetProtectionOptions ProtectionOptions {[System.Diagnostics.DebuggerStepThrough] get { return protectionOptions; } }
		public virtual CultureInfo InnerCulture {[System.Diagnostics.DebuggerStepThrough] get { return culture; } set { culture = value; } }
		public virtual CultureInfo Culture {
			get { return InnerCulture; }
			set {
				if (InnerCulture == value)
					return;
				InnerCulture = value;
				CalculationChain.ForceCalculate();
			}
		}
		public virtual CalculationChain CalculationChain {[System.Diagnostics.DebuggerStepThrough]  get { return calculationChain; } }
		public virtual SheetDefinitionCollection SheetDefinitions {[System.Diagnostics.DebuggerStepThrough] get { return sheetDefinitions; } }
		public DefinedNameCollection DefinedNames {[System.Diagnostics.DebuggerStepThrough] get { return definedNames; } }
		public ExternalLinkCollection ExternalLinks {[System.Diagnostics.DebuggerStepThrough] get { return externalLinks; } }
		public PivotCacheCollection PivotCaches {[System.Diagnostics.DebuggerStepThrough] get { return pivotCaches; } }
		public virtual int ContentVersion {[System.Diagnostics.DebuggerStepThrough]  get { return contentVersion; } set { contentVersion = value; } }
		public virtual int TransactionVersion {[System.Diagnostics.DebuggerStepThrough]  get { return transactionVersion; } }
		public virtual DocumentCache Cache {[System.Diagnostics.DebuggerStepThrough]  get { return cache; } }
		public bool CheckForCycleReferences { get { return checkForCycleReferences && !Properties.CalculationOptions.IterationsEnabled; } set { checkForCycleReferences = value; } }
		public virtual WorkbookDataContext DataContext {[System.Diagnostics.DebuggerStepThrough]  get { return dataContext; } protected set { dataContext = value; } }
		public bool SuppressCellValueAssignment {[System.Diagnostics.DebuggerStepThrough] get { return suppressCellValueAssignment; } set { suppressCellValueAssignment = value; } }
		public bool SuppressCellCreation {[System.Diagnostics.DebuggerStepThrough] get { return suppressCellCreation; } set { suppressCellCreation = value; } }
		public bool SuppressResetingCopiedRange {[System.Diagnostics.DebuggerStepThrough]  get { return suppressResetingCopiedRange; } set { suppressResetingCopiedRange = value; } }
		public string ConnectionsContent {[System.Diagnostics.DebuggerStepThrough] get { return connectionsContent; } set { connectionsContent = value; } }
		public string XmlMapsContent {[System.Diagnostics.DebuggerStepThrough] get { return xmlMapsContent; } set { xmlMapsContent = value; } }
		public List<string> QueryTablesContent {[System.Diagnostics.DebuggerStepThrough] get { return queryTablesContent; } }
		public virtual CommentAuthorCollection CommentAuthors {[System.Diagnostics.DebuggerStepThrough]  get { return commentAuthors; } }
		public VbaProject VbaProjectContent {[System.Diagnostics.DebuggerStepThrough] get { return vbaProjectContent; } }
		public bool HasMacros { get { return !vbaProjectContent.IsEmpty && !vbaProjectContent.HasNoMacros; } }
		public float MaxDigitWidth {
			get {
				return GetDefaultFontInfo().MaxDigitWidth;
			}
		}
		public float MaxDigitWidthInPixels {
			get {
#if !SL && !DXPORTABLE
				return GetDefaultRunFontInfo().GetFontInfoNoCache(PixelFontCache).MaxDigitWidth;
#else
				float result = GetDefaultFontInfo().MaxDigitWidth;
				return (float)Math.Round((double)LayoutUnitConverter.LayoutUnitsToPixelsF(result, DocumentModel.DpiX));
#endif
			}
		}
		public virtual UIBorderInfoRepository UiBorderInfoRepository {[System.Diagnostics.DebuggerStepThrough]  get { return uiBorderInfoRepository; } }
		protected internal DocumentModelDeferredChanges DeferredChanges {[System.Diagnostics.DebuggerStepThrough] get { return deferredChanges; } }
		protected internal virtual FontCache PixelFontCache {
			get {
#if !SL && !DXPORTABLE
				if (this.pixelFontCache == null) {
					if (PrecalculatedMetricsFontCacheManager.ShouldUse())
						this.pixelFontCache = new PrecalculatedMetricsFontCache(new DevExpress.Office.Layout.DocumentLayoutUnitPixelsConverter(DocumentModel.DpiX));
					else
						this.pixelFontCache = new PureGdiFontCache(new DevExpress.Office.Layout.DocumentLayoutUnitPixelsConverter(DocumentModel.DpiX));
				}
				return this.pixelFontCache;
#else
				return FontCache;
#endif
			}
		}
		public string CurrentAuthor {
			get {
				return !String.IsNullOrEmpty(currentAuthor) ? currentAuthor : "Default Author";
			}
			set {
				CommentAuthors.AddIfNotPresent(value);
				currentAuthor = value;
			}
		}
		Color skinGridlineColor;
		public virtual Color SkinGridlineColor {
			get {
				if (this.BehaviorOptions.UseSkinColors)
					return skinGridlineColor;
				return DXColor.FromArgb(217, 217, 217);
			}
			set {
				if (SkinGridlineColor == value)
					return;
				skinGridlineColor = value;
				UpdateGridlineColor();
			}
		}
		Color skinForeColor;
		public virtual Color SkinForeColor {
			get {
				if (this.BehaviorOptions.UseSkinColors)
					return skinForeColor;
				return DXSystemColors.WindowText;
			}
			set {
				if (SkinForeColor == value)
					return;
				skinForeColor = value;
			}
		}
		Color skinBackColor;
		public virtual Color SkinBackColor {
			get {
				if (this.BehaviorOptions.UseSkinColors)
					return skinBackColor;
				return DXSystemColors.Window;
			}
			set {
				if (SkinBackColor == value)
					return;
				skinBackColor = value;
			}
		}
		internal ICustomFunctionProvider CustomFunctionProvider { get { return customFunctionProvider; } }
		internal ICustomFunctionProvider BuiltInOverridesFunctionProvider { get { return builtInOverridesFunctionProvider; } }
		internal bool IsBeginUpdateFromUI { get { return beginUpdateFromUICounter > 0; } }
		internal DataComponentInfoList DataComponentInfos {
			get { return dataComponentInfos ?? (dataComponentInfos = new DataComponentInfoList(this)); }
		}
		public MailMergeParametersCollection MailMergeParameters { get { return mailMergeParameters ?? (mailMergeParameters = new MailMergeParametersCollection()); } }
		public object MailMergeDataSource {
			get { return mailMergeDataSource; }
			set {
				if (value != mailMergeDataSource) {
					mailMergeDataSource = value;
					mailMergeDataAdapter = null;
					RaiseMailMergeDataSourceChanged();
				}
			}
		}
		public string MailMergeDataMember { get; set; }
		public MailMergeProcessor MailMergeProcessor { get; set; }
		public bool ShowMailMergeRanges { get; set; }
		public bool ShowReferenceSelection { get; set; }
		public bool ReferenceEditMode { get; set; }
		public bool RecalculateAfterLoad { get { return recalculateAfterLoad; } set { recalculateAfterLoad = value; } }
		public bool SuppressRecalculateOnLoad { get { return suppressRecalculateOnLoad; } set { suppressRecalculateOnLoad = value; } }
		public bool SuppressAutoStartCalculation { get { return suppressAutoStartCalculation; } set { suppressAutoStartCalculation = value; } }
		public RealTimeDataManager RealTimeDataManager { get { return realTimeDataManager; } protected set { realTimeDataManager = value; } }
		public bool VisualControlAssigned { get { return visualControlAssigned; } set { visualControlAssigned = value; } }
		protected internal bool IsSetContentMode { get; private set; }
		public DefinedNamesRefencesCacheManager DefinedNamesRefencesCacheManager { get { return definedNamesRefencesCacheManager; } }
		public object Tag { get; set; }
		public CustomFunctionsDescriptions CustomFunctionsDescriptions { get { return customFunctionsDescriptions ?? (customFunctionsDescriptions = new CustomFunctionsDescriptions()); } }
		public bool IsCopyCutMode { get { return copiedRangeProvider.Range != null; } }
		public CopiedRangeProvider CopiedRangeProvider { get { return copiedRangeProvider; } }
		public IClipboard Clipboard { get { return clipboard; }
			set {
				if (clipboard != null)
					clipboard.ChangedOutside -= OnClipboardChangedOutside;
				clipboard = value;
				if (clipboard != null)
					clipboard.ChangedOutside += OnClipboardChangedOutside;
			}
		}
		#endregion
		#region Events
		#region BeginDocumentUpdate
		EventHandler onBeginDocumentUpdate;
		public event EventHandler BeginDocumentUpdate { add { onBeginDocumentUpdate += value; } remove { onBeginDocumentUpdate -= value; } }
		protected internal virtual void RaiseBeginDocumentUpdate() {
			if (onBeginDocumentUpdate != null)
				onBeginDocumentUpdate(this, EventArgs.Empty);
		}
		#endregion
		#region EndDocumentUpdate
		DocumentUpdateCompleteEventHandler onEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler EndDocumentUpdate { add { onEndDocumentUpdate += value; } remove { onEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseEndDocumentUpdate() {
			if (onEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region BeforeEndDocumentUpdate
		DocumentUpdateCompleteEventHandler onBeforeEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler BeforeEndDocumentUpdate { add { onBeforeEndDocumentUpdate += value; } remove { onBeforeEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseBeforeEndDocumentUpdate() {
			if (onBeforeEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onBeforeEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region AfterEndDocumentUpdate
		DocumentUpdateCompleteEventHandler onAfterEndDocumentUpdate;
		public event DocumentUpdateCompleteEventHandler AfterEndDocumentUpdate { add { onAfterEndDocumentUpdate += value; } remove { onAfterEndDocumentUpdate -= value; } }
		protected internal virtual void RaiseAfterEndDocumentUpdate() {
			if (onAfterEndDocumentUpdate != null) {
				DocumentUpdateCompleteEventArgs args = new DocumentUpdateCompleteEventArgs(DeferredChanges);
				onAfterEndDocumentUpdate(this, args);
			}
		}
		#endregion
		#region InnerContentChanged
		EventHandler onInnerContentChanged;
		internal event EventHandler InnerContentChanged { add { onInnerContentChanged += value; } remove { onInnerContentChanged -= value; } }
		protected internal virtual void RaiseInnerContentChanged() {
			if (onInnerContentChanged != null)
				onInnerContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentVersionChanged
		EventHandler onContentVersionChanged;
		internal event EventHandler ContentVersionChanged { add { onContentVersionChanged += value; } remove { onContentVersionChanged -= value; } }
		protected internal virtual void RaiseContentVersionChanged() {
			if (onContentVersionChanged != null)
				onContentVersionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		internal event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if (onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SchemaChanged
		EventHandler onSchemaChanged;
		internal event EventHandler SchemaChanged { add { onSchemaChanged += value; } remove { onSchemaChanged -= value; } }
		public void RaiseSchemaChanged() {
			if (IsUpdateLockedOrOverlapped)
				ApplyChanges(DocumentModelChangeActions.RaiseSchemaChanged);
			else
				RaiseSchemaChangedCore();
		}
		protected internal virtual void RaiseSchemaChangedCore() {
			if (onSchemaChanged != null)
				onSchemaChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InvalidFormatException
		InvalidFormatExceptionEventHandler onInvalidFormatException;
		public event InvalidFormatExceptionEventHandler InvalidFormatException { add { onInvalidFormatException += value; } remove { onInvalidFormatException -= value; } }
		protected internal virtual bool RaiseInvalidFormatException(Exception e) {
			if (onInvalidFormatException != null) {
				onInvalidFormatException(this, new SpreadsheetInvalidFormatExceptionEventArgs(e));
				return true;
			}
			else
				return false;
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		internal event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InnerDocumentCleared
		EventHandler onInnerDocumentCleared;
		public event EventHandler InnerDocumentCleared { add { onInnerDocumentCleared += value; } remove { onInnerDocumentCleared -= value; } }
		protected internal virtual void RaiseInnerDocumentCleared() {
			if (onInnerDocumentCleared != null)
				onInnerDocumentCleared(this, EventArgs.Empty);
		}
		#endregion
		#region DocumentCleared
		EventHandler onDocumentCleared;
		public event EventHandler DocumentCleared { add { onDocumentCleared += value; } remove { onDocumentCleared -= value; } }
		protected internal virtual void RaiseDocumentCleared() {
			if (onDocumentCleared != null)
				onDocumentCleared(this, EventArgs.Empty);
		}
		#endregion
		#region AfterEndSetContent
		EventHandler onAfterEndSetContent;
		public event EventHandler AfterEndSetContent { add { onAfterEndSetContent += value; } remove { onAfterEndSetContent -= value; } }
		protected internal virtual void RaiseAfterEndSetContent() {
			if (onAfterEndSetContent != null)
				onAfterEndSetContent(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeImport
		BeforeImportEventHandler onBeforeImport;
		public event BeforeImportEventHandler BeforeImport { add { onBeforeImport += value; } remove { onBeforeImport -= value; } }
		public virtual void RaiseBeforeImport(DocumentFormat format, IImporterOptions options) {
			if (onBeforeImport != null) {
				SpreadsheetBeforeImportEventArgs args = new SpreadsheetBeforeImportEventArgs(format, options);
				onBeforeImport(this, args);
			}
		}
		#endregion
		#region BeforeExport
		BeforeExportEventHandler onBeforeExport;
		public event BeforeExportEventHandler BeforeExport { add { onBeforeExport += value; } remove { onBeforeExport -= value; } }
		public virtual void RaiseBeforeExport(DocumentFormat format, IExporterOptions options) {
			if (onBeforeExport != null) {
				SpreadsheetBeforeExportEventArgs args = new SpreadsheetBeforeExportEventArgs(format, options);
				onBeforeExport(this, args);
			}
		}
		#endregion
		#region InnerSelectionChanged
		EventHandler onInnerSelectionChanged;
		internal event EventHandler InnerSelectionChanged { add { onInnerSelectionChanged += value; } remove { onInnerSelectionChanged -= value; } }
		protected internal virtual void RaiseInnerSelectionChanged() {
			if (onInnerSelectionChanged != null)
				onInnerSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region InnerParameterSelectionChanged
		EventHandler onInnerParameterSelectionChanged;
		internal event EventHandler InnerParameterSelectionChanged { add { onInnerParameterSelectionChanged += value; } remove { onInnerParameterSelectionChanged -= value; } }
		protected internal virtual void RaiseInnerParameterSelectionChanged() {
			if (onInnerParameterSelectionChanged != null)
				onInnerParameterSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
		EventHandler onSelectionChanged;
		internal event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null && CanRaiseEventOnChangingFromAPI())
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ParameterSelectionChanged
		EventHandler onParameterSelectionChanged;
		internal event EventHandler ParameterSelectionChanged { add { onParameterSelectionChanged += value; } remove { onParameterSelectionChanged -= value; } }
		protected internal virtual void RaiseParameterSelectionChanged() {
			if (onParameterSelectionChanged != null)
				onParameterSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ActiveSheetChanging
		ActiveSheetChangingEventHandler onActiveSheetChanging;
		internal event ActiveSheetChangingEventHandler ActiveSheetChanging { add { onActiveSheetChanging += value; } remove { onActiveSheetChanging -= value; } }
		protected internal virtual bool RaiseActiveSheetChanging(string oldActiveSheetName, string newActiveSheetName) {
			if (onActiveSheetChanging != null && CanRaiseEventOnChangingFromAPI()) {
				ActiveSheetChangingEventArgs args = new ActiveSheetChangingEventArgs(oldActiveSheetName, newActiveSheetName);
				onActiveSheetChanging(this, args);
				return args.Cancel;
			}
			else
				return false;
		}
		#endregion
		#region ActiveSheetChanged
		ActiveSheetChangedEventHandler onActiveSheetChanged;
		public event ActiveSheetChangedEventHandler ActiveSheetChanged { add { onActiveSheetChanged += value; } remove { onActiveSheetChanged -= value; } }
		protected internal virtual void RaiseActiveSheetChanged(string oldActiveSheetName, string newActiveSheetName) {
			if (onActiveSheetChanged != null && CanRaiseEventOnChangingFromAPI())
				onActiveSheetChanged(this, new ActiveSheetChangedEventArgs(oldActiveSheetName, newActiveSheetName));
		}
		#endregion
		#region InnerActiveSheetChanged
		ActiveSheetChangedEventHandler onInnerActiveSheetChanged;
		public event ActiveSheetChangedEventHandler InnerActiveSheetChanged { add { onInnerActiveSheetChanged += value; } remove { onInnerActiveSheetChanged -= value; } }
		protected internal virtual void RaiseInnerActiveSheetChanged(string oldActiveSheetName, string newActiveSheetName) {
			if (onInnerActiveSheetChanged != null)
				onInnerActiveSheetChanged(this, new ActiveSheetChangedEventArgs(oldActiveSheetName, newActiveSheetName));
		}
		#endregion
		#region InnerActiveSheetTryChange
		EventHandler onInnerActiveSheetTryChange;
		public event EventHandler InnerActiveSheetTryChange { add { onInnerActiveSheetTryChange += value; } remove { onInnerActiveSheetTryChange -= value; } }
		protected internal virtual void RaiseInnerActiveSheetTryChange() {
			if (onInnerActiveSheetTryChange != null)
				onInnerActiveSheetTryChange(this, EventArgs.Empty);
		}
		#endregion
		#region SheetRenaming
		SheetRenamingEventHandler onSheetRenaming;
		internal event SheetRenamingEventHandler SheetRenaming { add { onSheetRenaming += value; } remove { onSheetRenaming -= value; } }
		protected internal virtual bool RaiseSheetRenaming(SheetRenamingEventArgs args) {
			if (onActiveSheetChanging != null && CanRaiseEventOnChangingFromAPI()) {
				onSheetRenaming(this, args);
				return args.Cancel;
			}
			else
				return false;
		}
		#endregion
		#region InnerSheetRenamed
		SheetRenamedEventHandler onInnerSheetRenamed;
		public event SheetRenamedEventHandler InnerSheetRenamed { add { onInnerSheetRenamed += value; } remove { onInnerSheetRenamed -= value; } }
		protected internal virtual void RaiseInnerSheetRenamed(string oldName, string newName) {
			if (onInnerSheetRenamed != null)
				onInnerSheetRenamed(this, new SheetRenamedEventArgs(oldName, newName));
		}
		#endregion
		#region SheetRenamed
		SheetRenamedEventHandler onSheetRenamed;
		public event SheetRenamedEventHandler SheetRenamed { add { onSheetRenamed += value; } remove { onSheetRenamed -= value; } }
		protected internal virtual void RaiseSheetRenamed(string oldName, string newName) {
			if (onSheetRenamed != null && CanRaiseEventOnChangingFromAPI())
				onSheetRenamed(this, new SheetRenamedEventArgs(oldName, newName));
			ChangeInnerModified(true);
		}
		#endregion
		#region SheetInserted
		SheetInsertedEventHandler onSheetInserted;
		public event SheetInsertedEventHandler SheetInserted { add { onSheetInserted += value; } remove { onSheetInserted -= value; } }
		protected internal virtual void RaiseSheetInserted(string sheetName) {
			if (onSheetInserted != null && CanRaiseEventOnChangingFromAPI())
				onSheetInserted(this, new SheetInsertedEventArgs(sheetName));
			ChangeInnerModified(true);
		}
		#endregion
		#region SheetRemoved
		SheetRemovedEventHandler onSheetRemoved;
		public event SheetRemovedEventHandler SheetRemoved { add { onSheetRemoved += value; } remove { onSheetRemoved -= value; } }
		protected internal virtual void RaiseSheetRemoved(string sheetName) {
			if (onSheetRemoved != null && CanRaiseEventOnChangingFromAPI())
				onSheetRemoved(this, new SheetRemovedEventArgs(sheetName));
			ChangeInnerModified(true);
		}
		#endregion
		#region RowsRemoved
		RowsRemovedEventHandler onRowsRemoved;
		public event RowsRemovedEventHandler RowsRemoved { add { onRowsRemoved += value; } remove { onRowsRemoved -= value; } }
		protected internal virtual void RaiseRowsRemoved(RowsChangedEventArgs args) {
			if (onRowsRemoved != null && CanRaiseEventOnChangingFromAPI())
				onRowsRemoved(this, args);
		}
		#endregion
		#region RowsInserted
		RowsInsertedEventHandler onRowsInserted;
		public event RowsInsertedEventHandler RowsInserted { add { onRowsInserted += value; } remove { onRowsInserted -= value; } }
		protected internal virtual void RaiseRowsInserted(RowsChangedEventArgs args) {
			if (onRowsInserted != null && CanRaiseEventOnChangingFromAPI())
				onRowsInserted(this, args);
		}
		#endregion
		#region ColumnsRemoved
		ColumnsRemovedEventHandler onColumnsRemoved;
		public event ColumnsRemovedEventHandler ColumnsRemoved { add { onColumnsRemoved += value; } remove { onColumnsRemoved -= value; } }
		protected internal virtual void RaiseColumnsRemoved(ColumnsChangedEventArgs args) {
			if (onColumnsRemoved != null && CanRaiseEventOnChangingFromAPI())
				onColumnsRemoved(this, args);
		}
		#endregion
		#region ColumnsInserted
		ColumnsInsertedEventHandler onColumnsInserted;
		public event ColumnsInsertedEventHandler ColumnsInserted { add { onColumnsInserted += value; } remove { onColumnsInserted -= value; } }
		protected internal virtual void RaiseColumnsInserted(ColumnsChangedEventArgs args) {
			if (onColumnsInserted != null && CanRaiseEventOnChangingFromAPI())
				onColumnsInserted(this, args);
		}
		#endregion
		#region SheetVisibleStateChanged
		SheetVisibleStateChangedEventHandler onSheetVisibleStateChanged;
		public event SheetVisibleStateChangedEventHandler SheetVisibleStateChanged { add { onSheetVisibleStateChanged += value; } remove { onSheetVisibleStateChanged -= value; } }
		protected internal virtual void RaiseSheetVisibleStateChanged(string sheetName, SheetVisibleState oldValue, SheetVisibleState newValue) {
			if (onSheetVisibleStateChanged != null)
				onSheetVisibleStateChanged(this, new SheetVisibleStateChangedEventArgs(sheetName, oldValue, newValue));
		}
		#endregion
		#region ContentSetted
		DocumentContentSettedEventHandler onContentSetted;
		public event DocumentContentSettedEventHandler ContentSetted { add { onContentSetted += value; } remove { onContentSetted -= value; } }
		protected internal virtual void RaiseContentSetted(DocumentModelChangeType changeType) {
			if (onContentSetted != null) {
				DocumentContentSettedEventArgs args = new DocumentContentSettedEventArgs(changeType);
				onContentSetted(this, args);
			}
		}
		#endregion
		#region MailMergeDataSourceChanged
		EventHandler onMailMergeDataSourceChanged;
		public event EventHandler MailMergeDataSourceChanged { add { onMailMergeDataSourceChanged += value; } remove { onMailMergeDataSourceChanged -= value; } }
		protected internal virtual void RaiseMailMergeDataSourceChanged() {
			if (onMailMergeDataSourceChanged != null) {
				EventArgs args = new EventArgs();
				onMailMergeDataSourceChanged(this, args);
			}
		}
		#endregion
		#region MailMergeQueriesChanged
		EventHandler onMailMergeQueriesChanged;
		public event EventHandler MailMergeQueriesChanged { add { onMailMergeQueriesChanged += value; } remove { onMailMergeQueriesChanged -= value; } }
		protected internal virtual void RaiseMailMergeQueriesChanged() {
			if (onMailMergeQueriesChanged != null) {
				EventArgs args = new EventArgs();
				onMailMergeQueriesChanged(this, args);
			}
		}
		#endregion
		#region MailMergeRelationsChanged
		EventHandler onMailMergeRelationsChanged;
		public event EventHandler MailMergeRelationsChanged { add { onMailMergeRelationsChanged += value; } remove { onMailMergeRelationsChanged -= value; } }
		protected internal virtual void RaiseMailMergeRelationsChanged() {
			if (onMailMergeRelationsChanged != null) {
				EventArgs args = new EventArgs();
				onMailMergeRelationsChanged(this, args);
			}
		}
		#endregion
		#region PanesFrozen
		PanesFrozenEventHandler onPanesFrozen;
		internal event PanesFrozenEventHandler PanesFrozen { add { onPanesFrozen += value; } remove { onPanesFrozen -= value; } }
		protected internal virtual void RaisePanesFrozen(string sheetName, int rowOffset, int columnOffset, CellPosition topLeftCell) {
			if (onPanesFrozen != null && CanRaiseEventOnChangingFromAPI())
				onPanesFrozen(this, new PanesFrozenEventArgs(sheetName, rowOffset, columnOffset, topLeftCell));
		}
		#endregion
		#region PanesUnfrozen
		PanesUnfrozenEventHandler onPanesUnfrozen;
		internal event PanesUnfrozenEventHandler PanesUnfrozen { add { onPanesUnfrozen += value; } remove { onPanesUnfrozen -= value; } }
		protected internal virtual void RaisePanesUnfrozen(string sheetName) {
			if (onPanesUnfrozen != null && CanRaiseEventOnChangingFromAPI())
				onPanesUnfrozen(this, new PanesUnfrozenEventArgs(sheetName));
		}
		#endregion
		#region SheetTabColorChanged
		SheetTabColorChangedEventHandler onSheetTabColorChanged;
		public event SheetTabColorChangedEventHandler SheetTabColorChanged { add { onSheetTabColorChanged += value; } remove { onSheetTabColorChanged -= value; } }
		protected internal virtual void RaiseSheetTabColorChanged(string sheetName, Color oldValue, Color newValue) {
			if (onSheetTabColorChanged != null)
				onSheetTabColorChanged(this, new SheetTabColorChangedEventArgs(sheetName, oldValue, newValue));
		}
		#endregion
		#region ScrollPositionChanged
		ScrollPositionChangedEventHandler onScrollPositionChanged;
		public event ScrollPositionChangedEventHandler ScrollPositionChanged { add { onScrollPositionChanged += value; } remove { onScrollPositionChanged -= value; } }
		protected internal virtual void RaiseScrollPositionChanged(string sheetName, int columnIndex, int rowIndex) {
			if (onScrollPositionChanged != null && CanRaiseEventOnChangingFromAPI())
				onScrollPositionChanged(this, new ScrollPositionChangedEventArgs(sheetName, columnIndex, rowIndex));
		}
		#endregion
		#region InnerCellRemoving
		CellRemovingEventHandler onInnerCellRemoving;
		public event CellRemovingEventHandler InnerCellRemoving { add { onInnerCellRemoving += value; } remove { onInnerCellRemoving -= value; } }
		protected internal virtual void RaiseInnerCellRemoving(string sheetName, CellKey key) {
			if (onInnerCellRemoving != null)
				onInnerCellRemoving(this, new CellRemovingEventArgs(sheetName, key));
		}
		#endregion
		RangeCopyingEventHandler onRangeCopying;
		internal event RangeCopyingEventHandler CopyingRange {
			add { onRangeCopying += value; }
			remove { onRangeCopying -= value; }
		}
		protected internal virtual bool RaiseRangeCopying(RangeCopyingEventArgs args) {
			if (onRangeCopying != null) {
				onRangeCopying(this, args);
				return args.Cancel;
			}
			return true;
		}
		RangeCopiedEventHandler onRangeCopied;
		internal event RangeCopiedEventHandler RangeCopied {
			add { onRangeCopied += value; }
			remove { onRangeCopied -= value; }
		}
		protected internal virtual void RaiseRangeCopied(CellRange range, bool isCut) {
			if (onRangeCopied != null) {
				RangeCopiedEventArgs e = new RangeCopiedEventArgs(range, isCut, this.DataContext);
				onRangeCopied(this, e);
			}
		}
		ClipboardDataPastingEventHandler onClipboardDataPasting;
		internal event ClipboardDataPastingEventHandler ClipboardDataPasting {
			add { onClipboardDataPasting += value; }
			remove { onClipboardDataPasting -= value; }
		}
		protected internal virtual void RaiseClipboardDataPasting() {
			if (onClipboardDataPasting != null)
				onClipboardDataPasting(this, new EventArgs());
		}
		ShapesCopyingEventHandler onShapesCopying;
		internal event ShapesCopyingEventHandler CopyingShapes {
			add { onShapesCopying += value; }
			remove { onShapesCopying -= value; }
		}
		protected internal virtual bool RaiseShapesCopying() {
			if (onShapesCopying != null) {
				var args = new Spreadsheet.ShapesCopyingEventArgs();
				onShapesCopying(this, args);
				return args.Cancel;
			}
			return false;
		}
		CopiedRangePastingEventHandler onCopiedRangePasting;
		internal event CopiedRangePastingEventHandler CopiedRangePasting {
			add { onCopiedRangePasting += value; }
			remove { onCopiedRangePasting -= value; }
		}
		protected internal virtual bool RaiseCopiedRangePasting(CopiedRangePastingEventArgs args) {
			if (onCopiedRangePasting != null) {
				onCopiedRangePasting(this, args);
				return args.Cancel;
			}
			return false;
		}
		CopiedRangePastedEventHandler onCopiedRangePasted;
		internal event CopiedRangePastedEventHandler CopiedRangePasted {
			add { onCopiedRangePasted += value; }
			remove { onCopiedRangePasted -= value; }
		}
		protected internal virtual void RaiseCopiedRangePasted(Model.CellRange range, Model.CellRangeBase targetRange) {
			if (onCopiedRangePasted != null) {
				CopiedRangePastedEventArgs args = new CopiedRangePastedEventArgs(range, targetRange);
				onCopiedRangePasted(this, args);
			}
		}
		EventHandler onClipboardDataPasted;
		internal event EventHandler ClipboardDataPasted {
			add { onClipboardDataPasted += value; }
			remove { onClipboardDataPasted -= value; }
		}
		protected internal virtual void RaiseClipboardDataPasted(Model.CellRangeBase range) {
			if (onClipboardDataPasted != null) {
				ClipboardDataPastedEventArgs args = new ClipboardDataPastedEventArgs(range);
				onClipboardDataPasted(this, args);
			}
		}
		ClearingSourceRangeOnPasteAfterCutEventHandler onClearingSourceRangeOnPasteAfterCut;
		internal event ClearingSourceRangeOnPasteAfterCutEventHandler ClearingSourceRangeOnPasteAfterCut {
			add { onClearingSourceRangeOnPasteAfterCut += value; }
			remove { onClearingSourceRangeOnPasteAfterCut -= value; }
		}
		protected internal virtual void RaiseClearingSourceRangeOnPasteAfterCut(ClearingSourceRangeOnPasteAfterCutEventArgs args) {
			if (onClearingSourceRangeOnPasteAfterCut != null)
				onClearingSourceRangeOnPasteAfterCut(this, args);
		}
		ClipboardDataObtainedEventHandler onClipboardDataObtained;
		internal event ClipboardDataObtainedEventHandler ClipboardDataObtained {
			add { onClipboardDataObtained += value; }
			remove { onClipboardDataObtained -= value; }
		}
		protected internal virtual void RaiseClipboardDataObtained(ClipboardDataObtainedEventArgs args) {
			if (onClipboardDataObtained != null)
				onClipboardDataObtained(this, args);
		}
		#endregion
		protected override DocumentHistory CreateDocumentHistory() {
			return new SpreadsheetDocumentHistory(this);
		}
		protected override DocumentHistory CreateEmptyHistory() {
			return new SpreadsheetEmptyHistory(this);
		}
#if DEBUGTEST
		internal void SetIsBeginUpdateFromUI(bool value) { 
			if (value)
				beginUpdateFromUICounter = 10;
			else
				beginUpdateFromUICounter = 0;
		}
#endif
		bool CanRaiseEventOnChangingFromAPI() {
			if (IsBeginUpdateFromUI)
				return true;
			return EventOptions.RaiseOnModificationsViaAPI;
		}
		public static implicit operator DocumentModel(DocumentModelAccessor instance) {
			if (instance == null)
				return null;
			else
				return instance.DocumentModel;
		}
		protected internal virtual InternalAPI CreateInternalApi() {
			return new InternalAPI(this);
		}
		protected internal virtual void ChangeInnerModified(bool modified) {
			bool oldModified = Modified;
			innerModified = modified;
			if (oldModified != Modified)
				OnHistoryModifiedChangedCore();
		}
#if !SL && !DXPORTABLE
		#region GetUserName
		public static string GetUserName() {
			if (!SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)))
				return String.Empty;
			return DevExpress.Office.PInvoke.Win32.GetUserName(DevExpress.Office.PInvoke.Win32.ExtendedNameFormat.Display);
		}
		#endregion
#else
		public static string GetUserName() {
			return String.Empty;
		}
#endif
		#region IBatchUpdateHandler Members
		public override void OnBeginUpdate() {
			History.BeginTransaction();
		}
		public override void OnCancelUpdate() {
			History.EndTransaction();
		}
		public override void OnEndUpdate() {
			History.EndTransaction();
		}
		public override void OnFirstBeginUpdate() {
			RaiseBeginDocumentUpdate();
			CreateDeferredChanges();
			History.BeginTransaction();
		}
		public override void OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		public override void OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		public virtual void BeginSetContent() {
			BeginSetContent(true);
		}
		protected internal void BeginSetContent(bool clearContent) { 
			if (IsUpdateLocked && DeferredChanges.IsSetContentMode)
				Exceptions.ThrowInternalException();
			this.LeaveCopiedDataInClipboard();
			IsSetContentMode = true;
			BeginUpdate();
			ReplaceIWorksheetNameCreationService();
			DeferredChanges.IsSetContentMode = true;
			SuppressCellValueAssignment = true;
			UnsubscribeDocumentObjectsEvents();
			UnsubscribeSelectionEvents();
			if (clearContent)
				ClearDocumentCore();
			CheckForCycleReferences = false;
			isHistoryEnabled = IsNormalHistory;
			if (isHistoryEnabled)
				SwitchToEmptyHistory(true);
			Properties.CalculationOptions.SetIndexInitial(0);
			DocumentApplicationProperties.Reset();
			DocumentCoreProperties.RegisterDocumentCreation(true);
			DocumentCustomProperties.Reset();
			DataContext.SetImportExportSettings();
			RaiseInnerDocumentCleared();
			RaiseDocumentCleared();
		}
		public virtual void EndSetContent(DocumentModelChangeType changeType) {
			try {
				CheckForCycleReferences = true;
				SuppressCellValueAssignment = false;
				SuppressResetingCopiedRange = false;
				bool calculationChainEnabled = CalculationChain.Enabled;
				CalculationChain.SetUncalculatedState();
				bool shouldRebuildCellsChain = changeType != DocumentModelChangeType.LoadNewDocument || calculationChain.CellsChain.Header == null;
				CalculationChain.Regenerate(shouldRebuildCellsChain); 
				bool needRecalculation = Properties.CalculationOptions.CalculationMode != ModelCalculationMode.Manual && (recalculateAfterLoad || Properties.CalculationOptions.FullCalculationOnLoad);
				if (SuppressRecalculateOnLoad)
					needRecalculation = false;
				if (!calculationChainEnabled || Properties.CalculationOptions.FullCalculationOnLoad) {
					int initialCellContentVersion = needRecalculation && calculationChainEnabled ? 0x3FFF : 0;
					WorkBookCellsWalker walker = new ResetCellContentVersionCellsWalker(initialCellContentVersion);
					walker.Walk(this);
				}
				this.contentVersion = needRecalculation ? 1 : 0;
				PrepareStyleSheet();
				ValidateSheets();
				RestoreIWorksheetNameCreationService();
				DataContext.SetWorkbookDefinedSettings();
				SubscribeDocumentObjectsEvents();
				SubscribeSelectionEvents();
				RaiseAfterEndSetContent();
				if (isHistoryEnabled)
					SwitchToNormalHistory(true);
				ClearCopiedRange();
				ApplyChanges(changeType);
				DeferredChanges.IsSetContentMode = false;
				if (calculationChainEnabled && needRecalculation)
					calculationChain.CalculateWorkbook();
				RealTimeDataManager.OnEndSetContent();
				ResetTransactionVersion();
				EndUpdate();
				RaiseContentSetted(changeType);
				Properties.CalculationOptions.FullCalculationOnLoad = false;
			}
			finally {
				IsSetContentMode = false;
			}
		}
		internal void PrepareStyleSheet() {
			BuiltInCellStyleCalculator.AppendBuiltInCellStyles(StyleSheet);
			PredefinedTableStyleCalculator.AppendPredefinedTableStyles(StyleSheet.TableStyles);
		}
		internal void SetMainDocumentEmptyContentCore() {
			System.Diagnostics.Debug.Assert(IsUpdateLocked && DeferredChanges.IsSetContentMode);
			ClearDocumentCore();
			Worksheet newSheet = new Worksheet(this);
			newSheet.IsSelected = true;
			Sheets.Add(newSheet);
			System.Diagnostics.Debug.Assert(Sheets.Count == 1);
		}
		#region DeferredChanges
		protected internal virtual void CreateDeferredChanges() {
			this.deferredChanges = new DocumentModelDeferredChanges(this);
		}
		protected internal virtual void DeleteDeferredChanges() {
			this.deferredChanges = null;
		}
		protected internal void InnerApplyChanges(DocumentModelChangeActions changeActions) {
			BeginUpdate();
			try {
				ApplyChanges(changeActions);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void ApplyChanges(DocumentModelChangeType changeType) {
			ApplyChanges(DocumentModelChangeActionsCalculator.CalculateChangeActions(changeType));
		}
		protected internal virtual void ApplyChanges(DocumentModelChangeActions actions) {
			if (actions == DocumentModelChangeActions.None)
				return;
			System.Diagnostics.Debug.Assert(IsUpdateLocked || BatchUpdateHelper.OverlappedTransaction);
			this.DeferredChanges.ApplyChanges(actions);
		}
		protected internal virtual void OnLastEndUpdateCore() {
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetCachedContentVersions) != 0)
				ResetCachedContentVersions();
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetCachedTransactionVersions) != 0)
				ResetCachedTransactionVersions();
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetInvalidDataCircles) != 0 && ActiveSheet.ShowInvalidDataCircles)
				ActiveSheet.InvalidDataCircles.Clear();
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.RaisePivotTableChanged) != 0)
				OnPivotTableChangedCore();
			if (History.HasChangesInCurrentTransaction())
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.RaiseContentChanged;
			RaiseBeforeEndDocumentUpdate(); 
			History.EndTransaction(); 
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.ClearHistory) != 0)
				History.Clear();
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseContentVersionChanged) != 0)
				RaiseContentVersionChanged();
			if (DeferredChanges.ChangeActions != DocumentModelChangeActions.None)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.Redraw;
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.UpdateTransactedVersionInCopiedRange) != 0)
				copiedRangeProvider.UpdateTransactedVersion(TransactionVersion);
			else 
			if(IsCopyCutMode && TransactionVersion != copiedRangeProvider.TransactedVersion) {
				ClearCopiedRange();
			}
			RaiseEndDocumentUpdate();
			RaiseAfterEndDocumentUpdate();
			DeleteDeferredChanges();
		}
		#endregion
		#region Initialize
		protected void Initialize() {
			AddServices();
			this.uiBorderInfoRepository = new UIBorderInfoRepository();
			UpdateFontCache();
			CreateOptions();
			CreateDocumentObjects();
			documentCoreProperties.RegisterDocumentCreation(false);
			SubscribeDocumentObjectsEvents();
			SubscribeOptionsEvents();
			SwitchToNormalHistory(true);
			ResetTransactionVersion();
			recalculateAfterLoad = false;
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IDocumentImportManagerService), new DocumentImportManagerService());
			AddService(typeof(IDocumentExportManagerService), new DocumentExportManagerService());
			AddService(typeof(IUriStreamService), new UriStreamService());
			AddService(typeof(IUriProviderService), new UriProviderService());
			AddService(typeof(IWorksheetNameCreationService), new WorksheetNameCreationService());
			AddService(typeof(IThreadPoolService), new OfficeThreadPoolService());
			AddService(typeof(ILogService), new LogService());
			AddService(typeof(IColumnWidthCalculationService), new DevExpress.XtraSpreadsheet.Layout.Engine.PreciseColumnWidthCalculationService());
			AddService(typeof(ITableNameCreationService), new TableNameCreationService());
			AddService(typeof(IPivotTableNameCreationService), new PivotTableNameCreationService());
			AddService(typeof(IPivotCacheFieldNameCreationService), new PivotCacheFieldNameCreationService());
			AddService(typeof(IPivotDataFieldNameCreationService), new PivotDataFieldNameCreationService());
			AddService(typeof(ITableStyleNameCreationService), new TableStyleNameCreationService());
			AddService(typeof(IPivotStyleNameCreationService), new PivotStyleNameCreationService());
			AddService(typeof(ICellStyleNameCreationService), new CellStyleNameCreationService());
			AddService(typeof(IDataColumnNameCreationService), new DataColumnNameCreationService());
#if !SL && !DXPORTABLE
			AddService(typeof(IShapeRenderService), new DrawingShapeController());
			AddService(typeof(IRTDServiceFactory), new RTDServiceFactory());
#endif
			AddService(typeof(IClipboardProvider), new ClipboardProvider());
			AddService(typeof(ICopiedRangeDataForClipboard), new CopiedRangeDataForClipboard(this));
		}
		protected virtual void CreateOptions() {
			this.documentSaveOptions = new WorkbookSaveOptions();
			this.documentCapabilities = new WorkbookCapabilitiesOptions();
			this.documentImportOptions = new WorkbookImportOptions();
			this.documentExportOptions = new WorkbookExportOptions();
			this.behaviorOptions = new SpreadsheetBehaviorOptions();
			this.pivotTableFieldListOptions = new SpreadsheetPivotTableFieldListOptions();
			this.viewOptions = new SpreadsheetViewOptions();
			this.printOptions = new SpreadsheetPrintOptions();
			this.eventOptions = new WorkbookEventOptions();
			this.protectionOptions = new SpreadsheetProtectionOptions();
		}
		void SubscribeOptionsEvents() {
			if (this.viewOptions != null)
				this.viewOptions.Changed += OnViewOptionsChanged;
		}
		void UnsubscribeOptionsEvents() {
			if (this.viewOptions != null)
				this.viewOptions.Changed -= OnViewOptionsChanged;
		}
		void OnViewOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "Charts.Antialiasing")
				InvalidateCharts(true);
			if (e.Name == "Charts.TextAntialiasing")
				InvalidateCharts(false);
		}
		protected virtual void CreateDocumentObjects() {
			this.documentCoreProperties = new ModelDocumentCoreProperties();
			this.documentApplicationProperties = new ModelDocumentApplicationProperties();
			this.documentCustomProperties = new ModelDocumentCustomProperties();
			this.calculationChain = new CalculationChain(this);
			this.customFunctionProvider = new CustomFunctionProvider();
			this.builtInOverridesFunctionProvider = new CustomFunctionProvider();
			this.sheets = new WorksheetCollection(this);
			this.sharedStringTable = new SharedStringTable();
			this.definedNames = CreateDefinedNameCollection();
			this.properties = new WorkbookProperties(this);
			this.mailOptions = new MailOptions();
			this.internalAPI = CreateInternalApi();
			this.externalLinks = new ExternalLinkCollection(this);
			this.cache = new DocumentCache(this);
			this.styleSheet = new StyleSheet(this);
			this.styleSheet.Initialize();
			this.queryTablesContent = new List<string>();
			this.commentAuthors = new CommentAuthorCollection();
			this.sheetDefinitions = new SheetDefinitionCollection();
			this.vbaProjectContent = new VbaProject();
			this.definedNamesRefencesCacheManager = new DefinedNamesRefencesCacheManager(this);
			this.realTimeDataManager = new RealTimeDataManager(this);
			this.pivotCaches = new PivotCacheCollection(this);
		}
		void SubscribeDocumentObjectsEvents() {
			if (Sheets != null)
				SubscribeSheetsEvents();
		}
		void UnsubscribeDocumentObjectsEvents() {
			if (Sheets != null)
				UnsubscribeSheetsEvents();
		}
		protected internal virtual DefinedNameCollection CreateDefinedNameCollection() {
			return new DefinedNameCollection(this);
		}
		protected internal virtual void SubscribeSheetsEvents() {
			Sheets.AfterSheetInserted += OnAfterSheetInserted;
			Sheets.BeforeSheetRemoving += OnBeforeSheetRemoving;
			Sheets.AfterSheetRemoved += OnAfterSheetRemoved;
			Sheets.BeforeSheetCollectionCleared += OnBeforeSheetCollectionCleared;
			Sheets.AfterSheetCollectionCleared += OnAfterSheetCollectionCleared;
			Sheets.AfterWoksheetMoved += OnAfterWoksheetMoved;
		}
		protected internal virtual void UnsubscribeSheetsEvents() {
			Sheets.AfterSheetInserted -= OnAfterSheetInserted;
			Sheets.BeforeSheetRemoving -= OnBeforeSheetRemoving;
			Sheets.AfterSheetRemoved -= OnAfterSheetRemoved;
			Sheets.BeforeSheetCollectionCleared -= OnBeforeSheetCollectionCleared;
			Sheets.AfterSheetCollectionCleared -= OnAfterSheetCollectionCleared;
			Sheets.AfterWoksheetMoved -= OnAfterWoksheetMoved;
		}
		#endregion
		#region Clear
		public override void ClearCore() {
			UnsubscribeDocumentObjectsEvents();
			UnsubscribeOptionsEvents();
			base.ClearCore();
			if (realTimeDataManager != null) {
				realTimeDataManager.Dispose();
				realTimeDataManager = null;
			}
			if (cache != null) {
				cache.Dispose();
				cache = null;
			}
			if (dataContext != null) {
				dataContext.Dispose();
				dataContext = null;
			}
			if (calculationChain != null) {
				calculationChain.Dispose();
				calculationChain = null;
			}
			if (sheets != null) {
				foreach (Worksheet sheet in sheets.InnerList)
					sheet.ClearDrawingObjectsCollection();
				sheets.InnerList.Clear();
			}
			sheets = null;
			styleSheet = null;
			mailOptions = null;
			properties = null;
			sharedStringTable = null;
			definedNames = null;
			pivotCaches = null;
			documentSaveOptions = null;
			documentCapabilities = null;
			documentImportOptions = null;
			documentExportOptions = null;
			pivotTableFieldListOptions = null;
			behaviorOptions = null;
			viewOptions = null;
			printOptions = null;
			eventOptions = null;
			protectionOptions = null;
			externalLinks = null;
			connectionsContent = null;
			xmlMapsContent = null;
			queryTablesContent = null;
			commentAuthors = null;
			vbaProjectContent = null;
			customFunctionProvider.ClearFunctions();
			builtInOverridesFunctionProvider.ClearFunctions();
			recalculateAfterLoad = false;
			Tag = null;
			if (customFunctionsDescriptions != null) {
				foreach (KeyValuePair<string, CustomFunctionDescription> pair in customFunctionsDescriptions) {
					pair.Value.ParametersDescription.Clear();
					pair.Value.ParametersName.Clear();
					pair.Value.ReturnTypes.Clear();
				}
				customFunctionsDescriptions.Clear();
				customFunctionsDescriptions = null;
			}
		}
		protected internal virtual void ClearDocumentCore() {
			IThreadPoolService threadPoolService = GetService<IThreadPoolService>();
			if (threadPoolService != null)
				threadPoolService.Reset();
			if (ImageCache != null)
				ImageCache.Clear();
			if (this.cache != null)
				this.cache.Dispose();
			this.cache = new DocumentCache(this);
			StyleSheet.CellStyles.Clear();
			Properties.WorkbookWindowPropertiesList.Clear();
			sheetCounter = 0;
			cycleReferenceCellsCounter = 0;
			foreach (Worksheet sheet in sheets.InnerList)
				sheet.ClearDrawingObjectsCollection();
			sheets.InnerList.Clear();
			calculationChain.Reset();
			sheetDefinitions.Clear();
			definedNames.ClearNoSchemaChanged();
			pivotCaches.Clear();
			sharedStringTable.Clear();
			externalLinks.Clear();
			queryTablesContent.Clear();
			commentAuthors.Clear();
			vbaProjectContent.Clear();
			styleSheet.Initialize();
			properties.Reset();
			MailMergeDataMember = null;
			MailMergeDataSource = null;
			DataComponentInfos.Clear();
			MailMergeParameters.Clear();
			this.mailOptions = new MailOptions();
			recalculateAfterLoad = false;
			if (this.realTimeDataManager != null)
				this.realTimeDataManager.Dispose();
			this.realTimeDataManager = new RealTimeDataManager(this);
			GetService<IColumnWidthCalculationService>().ResetCachedValues();
		}
		#endregion
		protected override void OnHistoryModifiedChanged(object sender, EventArgs e) {
			if (!innerModified)
				OnHistoryModifiedChangedCore();
		}
		protected internal virtual void OnHistoryModifiedChangedCore() {
			if (IsUpdateLockedOrOverlapped)
				ApplyChanges(DocumentModelChangeActions.RaiseModifiedChanged);
			else
				RaiseModifiedChanged();
		}
		protected override void OnHistoryOperationCompleted(object sender, EventArgs e) {
			IncrementTransactionVersion();
			RaiseInnerContentChanged();
			if (IsUpdateLockedOrOverlapped)
				ApplyChanges(DocumentModelChangeActions.RaiseContentChanged);
			else
				RaiseContentChanged();
		}
		protected internal int ConvertCharactersToModelUnits(int columnWidth) {
			int characterWidth = GetMaxDigitCharWidth();
			return characterWidth * columnWidth;
		}
		protected internal int ConvertModelUnitsToCharacters(float columnWidth) {
			int characterWidth = GetMaxDigitCharWidth();
			return (int)Math.Round(columnWidth / characterWidth);
		}
		int GetMaxDigitCharWidth() {
			RunFontInfo font = GetDefaultRunFontInfo();
			int fontInfoId = FontCache.CalcNormalFontIndex(font.Name, (int)Math.Ceiling(font.Size) * 2, font.Bold, font.Italic, font.StrikeThrough, font.Underline != XlUnderlineType.None);
			FontInfo fontInfo = FontCache[fontInfoId];
			char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			int characterWidth = 0;
			for (int i = 0; i < 10; i++) {
				characterWidth = Math.Max(characterWidth, FontCache.Measurer.MeasureCharacterWidth(digits[i], fontInfo));
			}
			return characterWidth;
		}
		#region Import/Export
		protected override ImportHelper<DocumentFormat, bool> CreateDocumentImportHelper() {
			return new DocumentImportHelper(this);
		}
		public override ExportHelper<DocumentFormat, bool> CreateDocumentExportHelper(DocumentFormat documentFormat) {
			return new DocumentExportHelper(this);
		}
		protected override IImportManagerService<DocumentFormat, bool> GetImportManagerService() {
			return GetService<IDocumentImportManagerService>();
		}
		protected override IExportManagerService<DocumentFormat, bool> GetExportManagerService() {
			return GetService<IDocumentExportManagerService>();
		}
		#endregion
		#region ExternalLinks
		public DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook GetExternalWorkbookByIndex(int index) {
			if (index <= 0)
				return null;
			ExternalLink link = externalLinks[index - 1];
			if (link == null)
				return null;
			return link.Workbook;
		}
		public DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook GetExternalWorkbookByName(string name) {
			ExternalLink link = externalLinks[name];
			if (link == null)
				return null;
			return link.Workbook;
		}
		#endregion
		#region IDocumentModelPart Members
		IDocumentModel IDocumentModelPart.DocumentModel { get { return this; } }
		DocumentModel IDocumentModelPartWithApplyChanges.Workbook { get { return this; } }
		void IDocumentModelPartWithApplyChanges.ApplyChanges(DocumentModelChangeActions actions) {
			this.ApplyChanges(actions);
		}
		#endregion
		#region IModelWorkbook Members
		DefinedNameCollectionBase IModelWorkbook.DefinedNames { get { return definedNames; } }
		public IWorksheet GetSheetByName(string name) {
			return Sheets[name];
		}
		public List<IWorksheet> GetSheets(string startSheetName, string endSheetName) {
			List<Worksheet> list = sheets.GetRange(startSheetName, endSheetName);
			return GetSheetsCore(list);
		}
		public IWorksheet GetSheetById(int sheetId) {
			return sheets.GetById(sheetId);
		}
		public IWorksheet GetSheetByIndex(int index) {
			return sheets[index];
		}
		public int SheetCount {
			get { return sheets.Count; }
		}
		public List<IWorksheet> GetSheets() {
			return GetSheetsCore(sheets.GetRange());
		}
		List<IWorksheet> GetSheetsCore(List<Worksheet> target) {
			if (target == null)
				return null;
			List<IWorksheet> resultList = new List<IWorksheet>();
			foreach (Worksheet sheet in target)
				resultList.Add(sheet);
			return resultList;
		}
		#endregion
		#region Formulas
		public int IncrementContentVersion() {
			IncrementContentVersionCore();
			IncrementTransactionVersion();
			OnContentVersionChanged();
			return contentVersion;
		}
		public void IncrementContentVersionCore() {
			contentVersion++;
			int delta = contentVersion - (int)Cell.contentVersionMask;
			if (delta > 0) {
				contentVersion = delta;
				ResetCachedContentVersions();
			}
		}
		void OnContentVersionChanged() {
			if (IsUpdateLockedOrOverlapped)
				ApplyChanges(DocumentModelChangeActions.RaiseContentVersionChanged);
			else
				RaiseContentVersionChanged();
		}
		void ResetCachedContentVersions() {
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.ResetCachedContentVersions;
			else
				Sheets.ResetCachedContentVersions();
		}
		public void ResetContentVersion() {
			this.contentVersion = 0;
			OnContentVersionChanged();
		}
		public virtual int IncrementTransactionVersion() {
			transactionVersion++;
			int delta = transactionVersion - ((int)Cell.transactionVersionMask >> Cell.transactionMaskOffset);
			if (delta > 0) {
				transactionVersion = delta;
				ResetCachedTransactionVersions();
			}
			return transactionVersion;
		}
		void ResetCachedTransactionVersions() {
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.ResetCachedTransactionVersions;
			else
				Sheets.ResetCachedTransactionVersions();
		}
		public void ResetTransactionVersion() {
			this.transactionVersion = 1; 
		}
		public void IncrementCycleReferenceCellsCounter() {
			cycleReferenceCellsCounter++;
		}
		public void DecrementCycleReferenceCellsCounter() {
			cycleReferenceCellsCounter--;
		}
		public void CheckCircularReferences() {
			if (HasCircularReferences)
				throw new DevExpress.Spreadsheet.CircularReferenceException();
		}
		public void SetUseR1C1(bool value) {
			if (properties.UseR1C1ReferenceStyle == value)
				return;
			properties.UseR1C1ReferenceStyle = value;
			IncrementContentVersion();
		}
		#region PrepareFormulas
		internal void PrepareFormulas() {
			sheets.ForEach(PrepareSheetFormulas);
		}
		void PrepareSheetFormulas(Worksheet sheet) {
			sheet.PrepareFormulas();
		}
		#endregion
		#endregion
		#region Table
		public Table GetTableByName(string name) {
			foreach (Worksheet sheet in Sheets) {
				Table table = null;
				if (sheet.Tables.TryGetItemByName(name, out table))
					return table;
			}
			return null;
		}
		public List<string> GetTableNames() {
			int tablesCount = 0;
			foreach (Worksheet sheet in Sheets) {
				tablesCount += sheet.Tables.Count;
			}
			List<string> result = new List<string>(tablesCount);
			foreach (Worksheet sheet in Sheets) {
				result.AddRange(sheet.Tables.NamesHash.Keys);
			}
			return result;
		}
		protected internal IModelErrorInfo CheckTableName(string name) {
			if (!WorkbookDataContext.IsIdent(name))
				return new ModelErrorInfo(ModelErrorType.TableNameIsNotValid);
			if (this.definedNames.Contains(name))
				return new ModelErrorInfo(ModelErrorType.TableAlreadyExists);
			foreach (Worksheet sheet in Sheets)
				if (sheet.Tables.Contains(name) || sheet.DefinedNames.Contains(name))
					return new ModelErrorInfo(ModelErrorType.TableAlreadyExists);
			return null;
		}
		#endregion
		#region PivotTable
		public void OnPivotTableChanged() {
			if (IsUpdateLockedOrOverlapped)
				ApplyChanges(DocumentModelChangeActions.RaisePivotTableChanged);
			else
				OnPivotTableChangedCore();
		}
		void OnPivotTableChangedCore() {
		}
		#endregion
		#region InsertRange
		public IModelErrorInfo CanRangeInsert(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode, NotificationChecks checks) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)cellRange;
				if (union.HasNonConsistantRanges())
					return new ModelErrorInfo(ModelErrorType.DiffRangeTypesCanNotBeChanged);
				if (union.HasIntersectedRanges())
					return new ModelErrorInfo(ModelErrorType.IntersectedRangesCanNotBeChanged);
			}
			Worksheet sheet = cellRange.Worksheet as Worksheet;
			IModelErrorInfo error = sheet.CanRangeInsert(cellRange, mode, formatMode, checks);
			if (error != null)
				return error;
			return null;
		}
		protected internal void OnSingleRangeInserting(InsertRangeNotificationContext context) {
			Sheets.OnRangeInserting(context);
			DefinedNames.OnRangeInserting(context);
			PivotCaches.OnRangeInserting(context);
		}
		protected internal void OnAfterRangeInserting(int sheetId, CellRangeBase range, InsertCellMode mode) {
			RangeInsertedHistoryItem item = new RangeInsertedHistoryItem(this, sheetId, range, mode);
			History.Add(item);
			item.Execute();
		}
		#endregion
		#region RemoveRange
		public IModelErrorInfo CanRangeRemove(CellRangeBase cellRange, RemoveCellMode mode, NotificationChecks checks) {
			if (mode != RemoveCellMode.Default && cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)cellRange;
				if (union.HasNonConsistantRanges())
					return new ModelErrorInfo(ModelErrorType.DiffRangeTypesCanNotBeChanged);
				if (union.HasIntersectedRanges())
					return new ModelErrorInfo(ModelErrorType.IntersectedRangesCanNotBeChanged);
			}
			Worksheet sheet = cellRange.Worksheet as Worksheet;
			IModelErrorInfo error = sheet.CanRangeRemove(cellRange, mode, checks);
			if (error != null)
				return error;
			return null;
		}
		protected internal void OnSingleRangeRemoving(RemoveRangeNotificationContext context) {
			Sheets.OnRangeRemoving(context);
			DefinedNames.OnRangeRemoving(context);
			PivotCaches.OnRangeRemoving(context);
		}
		protected internal void OnAfterRangeRemoving(int sheetId, CellRangeBase range, RemoveCellMode mode) {
			RangeRemovedHistoryItem item = new RangeRemovedHistoryItem(this, sheetId, range, mode);
			History.Add(item);
			item.Execute();
		}
		#endregion
		public void CheckDefinedName(string name, int scopedSheetId) {
			string errorMessage = CheckDefinedNameCore(name, scopedSheetId, String.Empty);
			if (!String.IsNullOrEmpty(errorMessage))
				Exceptions.ThrowInvalidOperationException(errorMessage);
		}
		protected internal string CheckDefinedNameCore(string name, int scopedSheetId, string originalName) {
			if (!WorkbookDataContext.IsIdent(name))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidDefinedName);
			if (scopedSheetId < 0) {
				if (IsDuplicateName(name, originalName, definedNames))
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
			}
			foreach (Worksheet sheet in Sheets) {
				if (sheet.Tables.Contains(name))
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
				if (sheet.SheetId == scopedSheetId) {
					if (IsDuplicateName(name, originalName, sheet.DefinedNames))
						return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
				}
			}
			return String.Empty;
		}
		bool IsDuplicateName(string name, string originalName, DefinedNameCollection where) {
			if (String.IsNullOrEmpty(originalName))
				return where.Contains(name);
			DefinedNameBase definedName;
			if (!where.TryGetItemByName(name, out definedName))
				return false;
			DefinedNameBase originalDefinedName;
			where.TryGetItemByName(originalName, out originalDefinedName);
			return originalDefinedName != null && !Object.ReferenceEquals(originalDefinedName, definedName);
		}
		short CalculateNextCellTransactionId() {
			this.cellTransactionCount = Math.Max((short)1, ++cellTransactionCount);
			return cellTransactionCount;
		}
		public short RegisterCellTransaction(ICell cell) {
			BeginUpdate();
			short transactionId = CalculateNextCellTransactionId();
			CellBatchUpdateHelper helper = new CellBatchUpdateHelper(cell, transactionId);
			cellTransactions.Add(transactionId, helper);
			helper.Transaction = new HistoryTransaction(History);
			return transactionId;
		}
		public void UnregisterCellTransaction(short transactionId) {
			CellBatchUpdateHelper helper = cellTransactions[transactionId];
			helper.Transaction.Dispose();
			cellTransactions.Remove(transactionId);
			EndUpdate();
		}
		public CellBatchUpdateHelper GetCellTransaction(short transactionId) {
			return cellTransactions[transactionId];
		}
		#region Check Integrity
		public void CheckIntegrity() {
			CheckIntegrity(CheckIntegrityFlags.None);
		}
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (CalculationChain == null)
				IntegrityChecks.Fail("Workbook: calculation chain should not be null");
			if (CalculationChain.Workbook != this)
				IntegrityChecks.Fail("Workbook: CalculationChain.Workbook != this");
			SharedStringTable.CheckIntegrity();
			CalculationChain.CheckIntegrity();
			if ((flags & CheckIntegrityFlags.AllowZeroSheetCount) == 0) {
				if (SheetCount <= 0)
					IntegrityChecks.Fail("Workbook: there are no sheets in workbook");
			}
			if (Sheets == null)
				IntegrityChecks.Fail("Workbook: Sheets should not be null");
			if (Sheets.Count != SheetCount)
				IntegrityChecks.Fail("Workbook: Sheets.Count != SheetCount");
			Worksheet dataContextWoorksheet = DataContext.CurrentWorksheet as Worksheet;
			if (dataContextWoorksheet != null && !Sheets.Contains(dataContextWoorksheet))
				IntegrityChecks.Fail("DataContext: Sheets does not contains CurrentWorksheet");
			int count = SheetCount;
			for (int i = 0; i < count; i++)
				Sheets[i].CheckIntegrity(flags);
		}
		#endregion
		#region Formatting
		protected internal RunFontInfo GetDefaultRunFontInfoZeroItemInCache() {
			return Cache.FontInfoCache.DefaultItem;
		}
		protected internal RunFontInfo GetDefaultRunFontInfo() {
			return StyleSheet.CellStyles.Normal.FormatInfo.FontInfo;
		}
		protected internal FontInfo GetDefaultFontInfo() {
			return StyleSheet.CellStyles.Normal.FormatInfo.FontInfo.GetFontInfo(FontCache);
		}
		protected internal virtual float CalculateDefaultRowHeightInPoints() {
			FontInfo fontInfo = GetDefaultFontInfo();
			return LayoutUnitConverter.LayoutUnitsToPointsF(fontInfo.LineSpacing);
		}
		protected internal virtual float CalculateDefaultRowHeightInLayoutUnits() {
			FontInfo fontInfo = GetDefaultFontInfo();
			return fontInfo.LineSpacing;
		}
		protected override void ClearFontCache() {
			base.ClearFontCache();
			if (cache != null) {
				RunFontInfoCache fontInfoCache = cache.FontInfoCache;
				int count = fontInfoCache.Count;
				for (int i = 0; i < count; i++)
					fontInfoCache[i].ResetFontInfoIndex();
			}
			if (this.pixelFontCache != null) {
				this.pixelFontCache.Dispose();
				this.pixelFontCache = null;
			}
		}
		void UpdateGridlineColor() {
			foreach (Worksheet sheet in Sheets)
				sheet.UpdateGridlineColor();
		}
		#endregion
		#region Worksheet operations
		protected internal virtual void AddInitialSheets() {
			AddInitialSheets(true);
		}
		protected internal virtual void AddInitialSheets(bool subscribeSelectionEvents) {
			if (Sheets.Count > 0)
				return;
			System.Diagnostics.Debug.Assert(this.History.Count <= 0);
			bool historyEnabled = IsNormalHistory;
			if (historyEnabled)
				SwitchToEmptyHistory(true);
			try {
				try {
					if (subscribeSelectionEvents)
						UnsubscribeSelectionEvents();
					Worksheet sheet = CreateWorksheet();
					this.Sheets.Add(sheet);
					SelectSheet(ActiveSheetIndex);
				}
				finally {
					if (subscribeSelectionEvents)
						SubscribeSelectionEvents();
				}
			}
			finally {
				this.innerModified = false;
				if (historyEnabled)
					SwitchToNormalHistory(true);
			}
		}
		public Worksheet CreateWorksheet() {
			return CreateWorksheetCore(string.Empty);
		}
		public Worksheet CreateWorksheet(string name) {
			return CreateWorksheetCore(name);
		}
		protected internal virtual Worksheet CreateWorksheetCore(string name) {
			if (!String.IsNullOrEmpty(name))
				return new Worksheet(this, name);
			return new Worksheet(this);
		}
		internal void SetActiveSheetIndex(int value, bool raiseChanging) {
			if (IsUpdateLocked && DeferredChanges.IsSetContentMode)
				SetActiveSheetIndexCore(value, raiseChanging);
			else {
				UnsubscribeSelectionEvents();
				try {
					SetActiveSheetIndexCore(value, raiseChanging);
				}
				finally {
					SubscribeSelectionEvents();
				}
			}
		}
		void SetActiveSheetIndexCore(int value, bool raiseChanging) {
			if (value == ActiveSheetIndex) {
				RaiseInnerActiveSheetTryChange();
				return;
			}
			string oldSheetName = ActiveSheet.Name;
			string newSheetName = Sheets[value].Name;
			if (raiseChanging && RaiseActiveSheetChanging(oldSheetName, newSheetName))
				return;
			BeginUpdate();
			try {
				SetActiveSheetIndexWithoutEvents(value);
				SelectSheet(value);
				RaiseInnerActiveSheetChanged(oldSheetName, newSheetName);
			}
			finally {
				EndUpdate();
			}
			RaiseActiveSheetChanged(oldSheetName, newSheetName);
		}
		protected internal void SetActiveSheetIndexWithoutEvents(int index) {
			List<WorkbookWindowProperties> windowProperties = Properties.WorkbookWindowPropertiesList;
			if (windowProperties.Count == 0)
				windowProperties.Add(new WorkbookWindowProperties(this));
			windowProperties[0].SelectedTabIndex = index;
		}
		protected internal void SelectSheet(int index) {
			int count = Sheets.Count;
			for (int i = 0; i < count; i++) {
				Sheets[i].IsSelected = false;
			}
			Sheets[index].IsSelected = true;
		}
		void CorrectActiveSheet(int index) {
			int activeSheetIndex = ActiveSheetIndex;
			if (activeSheetIndex < index)
				return;
			int correctedIndex;
			if (activeSheetIndex > index)
				correctedIndex = Math.Max(0, activeSheetIndex - 1);
			else
				correctedIndex = Math.Min(Sheets.Count - 2, activeSheetIndex);
			SetActiveSheetIndexWithoutEvents(correctedIndex);
		}
		#region Get worksheet collection
		public List<Worksheet> GetSelectedSheets() {
			List<Worksheet> result = new List<Worksheet>();
			foreach (Worksheet sheet in Sheets) {
				if (sheet.IsSelected)
					result.Add(sheet);
			}
			return result;
		}
		public List<Worksheet> GetVisibleSheets() {
			List<Worksheet> result = new List<Worksheet>();
			foreach (Worksheet sheet in Sheets) {
				if (sheet.VisibleState == SheetVisibleState.Visible)
					result.Add(sheet);
			}
			return result;
		}
		public List<string> GetVisibleSheetNames() {
			List<string> result = new List<string>();
			foreach (Worksheet sheet in Sheets) {
				if (sheet.VisibleState == SheetVisibleState.Visible)
					result.Add(sheet.Name);
			}
			return result;
		}
		public List<Worksheet> GetHiddenSheets() {
			List<Worksheet> result = new List<Worksheet>();
			foreach (Worksheet sheet in Sheets) {
				if (sheet.VisibleState == SheetVisibleState.Hidden)
					result.Add(sheet);
			}
			return result;
		}
		public List<string> GetHiddenSheetNames() {
			List<string> result = new List<string>();
			foreach (Worksheet sheet in Sheets) {
				if (sheet.VisibleState == SheetVisibleState.Hidden)
					result.Add(sheet.Name);
			}
			return result;
		}
		#endregion
		void ReplaceIWorksheetNameCreationService() {
			WorksheetNameValidationType validationType = DocumentImportOptions.WorksheetNameValidationType;
			if (oldWorksheetNameCreationService != null || validationType == WorksheetNameValidationType.Check)
				return;
			oldWorksheetNameCreationService = GetService<IWorksheetNameCreationService>();
			System.Diagnostics.Debug.Assert(validationType == WorksheetNameValidationType.AutoCorrect);
			ReplaceService<IWorksheetNameCreationService>(new WorksheetNotValidatedNameCreationService());
		}
		void RestoreIWorksheetNameCreationService() {
			if (oldWorksheetNameCreationService == null)
				return;
			ReplaceService<IWorksheetNameCreationService>(oldWorksheetNameCreationService);
			oldWorksheetNameCreationService = null;
			ValidateSheetNames();
		}
		void ValidateSheetNames() {
			IWorksheetNameCreationService service = GetService<IWorksheetNameCreationService>();
			string[] existingSheetNames = null;
			foreach (Worksheet sheet in Sheets) {
				if (existingSheetNames == null)
					existingSheetNames = this.Sheets.GetSheetNames();
				string validatedSheetName = service.GetNormalizedName(sheet.Name, existingSheetNames, true);
				if (string.Compare(sheet.Name, validatedSheetName) != 0) {
					sheet.Name = validatedSheetName;
					existingSheetNames = null;
				}
			}
		}
		void ValidateSheets() {
			SetSheetSelected();
		}
		void SetSheetSelected() {
			List<Worksheet> selectedSheets = GetSelectedSheets();
			if (selectedSheets.Count == 0 && Sheets.Count > 0)
				Sheets[0].IsSelected = true;
		}
		void OnAfterWoksheetMoved(object sender, WorksheetMovedEventArgs e) {
			InternalAPI.RaiseAfterWoksheetMoved(e.Worksheet, e.OldIndex, e.NewIndex);
		}
		public int CreateNextSheetId() {
			return ++sheetCounter;
		}
		void OnAfterSheetInserted(object sender, WorksheetCollectionChangedEventArgs e) {
			InternalAPI.OnAfterSheetInserted(e.Worksheet, e.Index);
			RaiseSheetInserted(e.Worksheet.Name);
			RaiseSchemaChanged();
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.ResetAllLayout;
		}
		void OnBeforeSheetRemoving(object sender, WorksheetCollectionChangedEventArgs e) {
			Worksheet sheet = e.Worksheet;
			int sheetIndex = e.Index;
			CorrectActiveSheet(sheetIndex);
			SheetRemovedFormulaWalker walker = new SheetRemovedFormulaWalker(sheet.SheetId, DataContext);
			walker.Walk(this);
			sheet.Tables.Clear();
			sheet.ClearDrawingObjectsCollection();
			InternalAPI.OnBeforeSheetRemoving(sheet, sheetIndex);
		}
		void OnAfterSheetRemoved(object sender, WorksheetCollectionChangedEventArgs e) {
			SelectSheet(ActiveSheetIndex);
			InternalAPI.OnAfterSheetRemoved(e.Worksheet, e.Index);
			RaiseSheetRemoved(e.Worksheet.Name);
			RaiseSchemaChanged();
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.ResetAllLayout;
		}
		void OnBeforeSheetCollectionCleared(object sender, EventArgs e) {
		}
		void OnAfterSheetCollectionCleared(object sender, EventArgs e) {
			InternalAPI.OnAfterSheetCollectionCleared();
		}
		protected internal virtual void OnActiveSheetSelectionChanged(object sender, EventArgs e) {
			RaiseInnerSelectionChanged();
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= CalculateSelectionChangeActions();
			else
				RaiseSelectionChanged();
		}
		protected internal virtual DocumentModelChangeActions CalculateSelectionChangeActions() {
			DocumentModelChangeActions result = DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetHeaderContent | DocumentModelChangeActions.Redraw;
			if (CanRaiseEventOnChangingFromAPI())
				result |= DocumentModelChangeActions.RaiseSelectionChanged;
			return result;
		}
		protected internal virtual void OnActiveSheetParameterSelectionChanged(object sender, EventArgs e) {
			RaiseInnerParameterSelectionChanged();
			if (IsUpdateLocked)
				DeferredChanges.ChangeActions |= DocumentModelChangeActions.RaiseParameterSelectionChanged |
					DocumentModelChangeActions.ResetSelectionLayout |
					DocumentModelChangeActions.ResetHeaderContent;
			else
				RaiseParameterSelectionChanged();
		}
		#endregion
		protected internal virtual void SubscribeSelectionEvents() {
			if (ActiveSheet != null)
				SubscribeSelectionEventsCore();
		}
		protected internal virtual void SubscribeSelectionEventsCore() {
			ActiveSheet.Selection.SelectionChanged += OnActiveSheetSelectionChanged;
			ActiveSheet.ReferenceEditSelection.SelectionChanged += OnActiveSheetParameterSelectionChanged;
		}
		protected internal virtual void UnsubscribeSelectionEvents() {
			if (ActiveSheet != null)
				UnsubscribeSelectionEventsCore();
		}
		protected internal virtual void UnsubscribeSelectionEventsCore() {
			ActiveSheet.Selection.SelectionChanged -= OnActiveSheetSelectionChanged;
			ActiveSheet.ReferenceEditSelection.SelectionChanged -= OnActiveSheetParameterSelectionChanged;
		}
		public string GetMailMergeDisplayName(string name, bool needRelationDisplayName) {
			if (mailMergeDataSource != null) {
				if (mailMergeDataAdapter == null)
					mailMergeDataAdapter = new SpreadsheetDataControllerAdapter(new SpreadsheetDataController());
				mailMergeDataAdapter.DataSource = mailMergeDataSource;
				mailMergeDataAdapter.DataMember = MailMergeDataMember;
				return mailMergeDataAdapter.GetDisplayName(name, needRelationDisplayName);
			}
			return name;
		}
		public float GetBoxRotationAngleInDegrees(DrawingBox box) {
			return GetPictureRotationAngleInDegrees(box.DrawingIndex);
		}
		public float GetPictureRotationAngleInDegrees(int pictureIndex) {
			return ActiveSheet.DrawingObjects[pictureIndex].GetRotationAngleInDegrees();
		}
		public void BeginUpdateFromUI() {
			beginUpdateFromUICounter++;
			BeginUpdate();
		}
		public void EndUpdateFromUI() {
			beginUpdateFromUICounter--;
			EndUpdate();
		}
		public void CancelUpdateFromUI() {
			beginUpdateFromUICounter--;
			CancelUpdate();
		}
#if !SL && !DXPORTABLE
		static readonly DevExpress.Utils.Text.IWordBreakProvider wordBreakProvider = new SpreadsheetWordBreakProvider();
		public static DevExpress.Utils.Text.IWordBreakProvider WordBreakProvider { get { return wordBreakProvider; } }
#endif
		protected override void OnLayoutUnitChanged() {
			IncrementTransactionVersion();
			base.OnLayoutUnitChanged();
		}
		public bool CanEditActiveCellContent(bool checkAccessRights) {
			DevExpress.XtraSpreadsheet.Model.Worksheet sheet = this.ActiveSheet;
			if (!sheet.Properties.Protection.SheetLocked)
				return true;
			CellPosition activeCellPosition = sheet.Selection.ActiveCell;
			DevExpress.XtraSpreadsheet.Model.ICell activeCell = sheet.TryGetCell(activeCellPosition.Column, activeCellPosition.Row);
			if (activeCell == null)
				activeCell = new FakeCell(activeCellPosition, sheet);
			if (!activeCell.ActualProtection.Locked)
				return true;
			if (!checkAccessRights)
				return false;
			return CheckActiveCellAccessGranted();
		}
		bool CheckActiveCellAccessGranted() {
			IList<ModelProtectedRange> ranges = ObtainActiveCellProtectedRanges(ActiveSheet, ActiveSheet.Selection.ActiveCell);
			foreach (ModelProtectedRange range in ranges) {
				if (range.IsAccessGranted)
					return true;
			}
			return false;
		}
		public bool CheckRangeAccess(CellRange range) {
			IList<ModelProtectedRange> ranges = ObtainProtectedRanges(range);
			if (ranges == null || ranges.Count <= 0)
				return !range.ContainsLockedCells();
			foreach (ModelProtectedRange protectedRange in ranges) {
				if (protectedRange.IsAccessGranted)
					return true;
			}
			return !range.ContainsLockedCells();
		}
		public IList<ModelProtectedRange> ObtainProtectedRanges(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			return sheet.ProtectedRanges.LookupProtectedRangesContainingEntireRange(range);
		}
		public ModelProtectedRange ObtainActiveCellProtectedRange() {
			DevExpress.XtraSpreadsheet.Model.Worksheet sheet = this.ActiveSheet;
			return sheet.ProtectedRanges.LookupProtectedRange(sheet.Selection.ActiveCell);
		}
		public IList<ModelProtectedRange> ObtainActiveCellProtectedRanges(Worksheet sheet, CellPosition activeCell) {
			return sheet.ProtectedRanges.LookupProtectedRanges(activeCell);
		}
		public bool IsProtected { get { return properties.Protection.IsLocked; } }
		public void Protect(string password, bool lockStructure, bool lockWindows) {
			if (IsProtected)
				throw new InvalidOperationException("Workbook is already protected");
			WorkbookProtectionOptions protection = properties.Protection;
			properties.Protection.LockStructure = lockStructure;
			properties.Protection.LockWindows = lockWindows;
			protection.Credentials = new ProtectionCredentials(password, true, this.ProtectionOptions.UseStrongPasswordVerifier, this.ProtectionOptions.SpinCount);
			ClearHistorySafe();
			InnerApplyChanges(DocumentModelChangeActions.RaiseUpdateUI);
		}
		public ModelErrorInfo UnProtect(string password) {
			if (!IsProtected)
				throw new InvalidOperationException("Workbook is not protected");
			WorkbookProtectionOptions protectionOptions = Properties.Protection;
			if (!protectionOptions.CheckPassword(password)) {
				return new ModelErrorInfo(ModelErrorType.IncorrectPassword);
			}
			protectionOptions.LockRevisions = false;
			protectionOptions.LockStructure = false;
			protectionOptions.LockWindows = false;
			protectionOptions.Credentials = new ProtectionCredentials(); 
			ClearHistorySafe();
			InnerApplyChanges(DocumentModelChangeActions.RaiseUpdateUI);
			return null;
		}
		void ClearHistorySafe() {
			if (!IsUpdateLocked)
				History.Clear();
		}
		protected internal void MoveSheet(Worksheet sheet, int index) {
			Sheets.Move(index, sheet);
			ValidateActiveSheet();
		}
		protected internal void ValidateActiveSheet() {
			if (ActiveSheet.VisibleState == SheetVisibleState.Visible)
				return;
			Worksheet activeSheet = CalculateActualActiveSheet();
			System.Diagnostics.Debug.Assert(activeSheet != null);
			if (activeSheet != null)
				ActiveSheet = activeSheet;
		}
		protected internal Worksheet CalculateActualActiveSheet() {
			int sheetCount = Sheets.Count;
			for (int i = ActiveSheetIndex + 1; i < sheetCount; i++) {
				Worksheet currentSheet = Sheets[i];
				if (currentSheet.VisibleState == SheetVisibleState.Visible)
					return currentSheet;
			}
			for (int i = ActiveSheetIndex - 1; i >= 0; i--) {
				Worksheet currentSheet = Sheets[i];
				if (currentSheet.VisibleState == SheetVisibleState.Visible)
					return currentSheet;
			}
			return null;
		}
		public void RaiseCellValueChangedProgrammatically(CellContentSnapshot snapshot) {
			if (EventOptions.RaiseOnModificationsViaAPI)
				InternalAPI.RaiseCellValueChanged(snapshot);
		}
		public void OnBeginPainting() {
		}
		public void OnEndPainting() {
			realTimeDataManager.OnEndPaint();
		}
		#region IDocumentPropertiesContainer Members
		void IDocumentPropertiesContainer.SetTitle(string value) {
			DocumentCoreProperties.Title = value;
		}
		void IDocumentPropertiesContainer.SetSubject(string value) {
			DocumentCoreProperties.Subject = value;
		}
		void IDocumentPropertiesContainer.SetAuthor(string value) {
			DocumentCoreProperties.Creator = value;
		}
		void IDocumentPropertiesContainer.SetKeywords(string value) {
			DocumentCoreProperties.Keywords = value;
		}
		void IDocumentPropertiesContainer.SetDescription(string value) {
			DocumentCoreProperties.Description = value;
		}
		void IDocumentPropertiesContainer.SetLastModifiedBy(string value) {
			DocumentCoreProperties.LastModifiedBy = value;
		}
		void IDocumentPropertiesContainer.SetCategory(string value) {
			DocumentCoreProperties.Category = value;
		}
		void IDocumentPropertiesContainer.SetCreated(DateTime value) {
			DocumentCoreProperties.Created = value;
		}
		void IDocumentPropertiesContainer.SetModified(DateTime value) {
			DocumentCoreProperties.Modified = value;
		}
		void IDocumentPropertiesContainer.SetLastPrinted(DateTime value) {
			DocumentCoreProperties.LastPrinted = value;
		}
		void IDocumentPropertiesContainer.SetApplication(string value) {
			DocumentApplicationProperties.Application = value;
		}
		void IDocumentPropertiesContainer.SetManager(string value) {
			DocumentApplicationProperties.Manager = value;
		}
		void IDocumentPropertiesContainer.SetCompany(string value) {
			DocumentApplicationProperties.Company = value;
		}
		void IDocumentPropertiesContainer.SetVersion(string value) {
			DocumentApplicationProperties.Version = value;
		}
		void IDocumentPropertiesContainer.SetSecurity(int value) {
			DocumentApplicationProperties.Security = (ModelDocumentSecurity)value;
		}
		void IDocumentPropertiesContainer.SetText(string propName, string propValue) {
			DocumentCustomProperties[propName] = propValue;
		}
		void IDocumentPropertiesContainer.SetNumeric(string propName, double propValue) {
			DocumentCustomProperties[propName] = propValue;
		}
		void IDocumentPropertiesContainer.SetBoolean(string propName, bool propValue) {
			DocumentCustomProperties[propName] = propValue;
		}
		void IDocumentPropertiesContainer.SetDateTime(string propName, DateTime propValue) {
			DocumentCustomProperties[propName] = propValue;
		}
		#endregion
		public void CopyCutRangeToClipboard(CellRange range, bool isCutMode) {
			var args = new RangeCopyingEventArgs(range, isCutMode);
			if (RaiseRangeCopying(args))
				return;
			CellRange modifiedRange = args.ModelRange;
			copiedRangeProvider.SetRange(modifiedRange, args.IsCut, this.transactionVersion);
			ICopiedRangeDataForClipboard provider = GetService<ICopiedRangeDataForClipboard>();
			if (provider == null)
				throw new InvalidOperationException("DocumentModel has no IDataForClipboardPrivoder");
			Clipboard.SetData(provider, CopiedRangeProvider);
			RaiseRangeCopied(modifiedRange, args.IsCut);
		}
		void OnClipboardChangedOutside(object sender, EventArgs e) {
			ClearCopiedRangeWithoutClearingClipboard();
			AfterCopiedRangeChangedApplyRedraw();
		}
		public void LeaveCopiedDataInClipboard() {
			Clipboard.LeaveCopiedDataInClipboard();
			ClearCopiedRangeWithoutClearingClipboard();
		}
		public void ClearCopiedRangeWithoutClearingClipboard() {
			CopiedRangeProvider.SetRange(null, false, -1);
		}
		public void ClearCopiedRange() {
			if (SuppressResetingCopiedRange)
				return;
			if (!IsCopyCutMode)
				return;
			Clipboard.Clear();
			ClearCopiedRangeWithoutClearingClipboard();
			AfterCopiedRangeChangedApplyRedraw();
		}
		void AfterCopiedRangeChangedApplyRedraw() {
			try {
				BeginUpdate();
				ApplyChanges(DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.Redraw);
			}
			finally {
				EndUpdate();
			}
		}
	}
	#endregion
	#region SpreadsheetWordBreakProvider
#if !SL && !DXPORTABLE
	public class SpreadsheetWordBreakProvider : DevExpress.Utils.Text.IWordBreakProvider {
		bool DevExpress.Utils.Text.IWordBreakProvider.IsWordBreakChar(char ch) {
			return (Char.IsWhiteSpace(ch) || ch == '-');
		}
	}
#endif
	#endregion
	#region CheckIntegrityFlags
	[Flags]
	public enum CheckIntegrityFlags {
		None = 0,
		AllowZeroSheetCount = 0x1,
		SkipTimeConsumingChecks = 0x2,
	}
	#endregion
	#region WorkbookCollection
	public class WorkbookCollection : List<DocumentModel> {
	}
	#endregion
	#region DocumentModelAccessor
	public class DocumentModelAccessor {
		readonly DocumentModel documentModel;
		internal DocumentModelAccessor(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		internal DocumentModel DocumentModel { get { return documentModel; } }
	}
	#endregion
}
