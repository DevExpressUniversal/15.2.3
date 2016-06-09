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

#region Workbook Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using DevExpress.Office;
	using DevExpress.Office.API.Internal;
	using DevExpress.Office.Drawing;
	using DevExpress.Office.Services.Implementation;
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet;
	using DevExpress.XtraSpreadsheet.Drawing;
	using DevExpress.XtraSpreadsheet.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Model;
	using DevExpress.XtraSpreadsheet.Utils;
	using Cell = DevExpress.Spreadsheet.Cell;
	using Model = DevExpress.XtraSpreadsheet.Model;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	#region NativeWorkbook
	partial class NativeWorkbook : IInnerSpreadsheetDocumentServerOwner, IDisposable {
		#region Fields
		ModelWorkbook workbook;
		NativeWorksheetCollection worksheets;
		NativeDefinedNameCollection definedNames;
		NativeStyleCollection styleCollection;
		NativeTableStyleCollection tableStyleCollection;
		NativeExternalWorkbookCollection nativeExternalWorkbooks;
		NativeDocumentSettings documentSettings;
		InnerSpreadsheetDocumentServer server;
		NativeStyleCache styleCache;
		UnitConverter unitConverter;
		NativeHistory history;
		NativeWorkbookFunctions nativeFunctions;
		NativeFormulaEngine nativeFormulas;
		NativePivotCacheCollection pivotCaches;
		NativeParametersCollection parameters;
		bool isValid;
		bool isDisposing;
		bool isDisposed;
		bool isExternalServer;
		#endregion
#if DXPORTABLE
		public
#else
		protected internal
#endif
			NativeWorkbook() {
			this.workbook = new ModelWorkbook();
			this.workbook.AddInitialSheets();
			this.server = CreateServer(workbook);
			BeginInitializeServer();
			server.SetInitialDocumentModelLayoutUnit(DocumentLayoutUnit.Pixel);
			EndInitializeServer();
			InitializeWorkbookCore();
		}
		protected internal NativeWorkbook(InnerSpreadsheetDocumentServer server) {
			Guard.ArgumentNotNull(server, "server");
			if (server.IsDisposed)
				throw new ArgumentException("workbook: disposed server");
			this.server = server;
			this.workbook = server.DocumentModel;
			this.isExternalServer = true;
			InitializeWorkbookCore();
		}
		protected internal NativeWorkbook(ModelWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			if (workbook.IsDisposed)
				throw new ArgumentException("workbook: disposed workbook");
			this.workbook = workbook;
			this.server = CreateServer(workbook);
			BeginInitializeServer();
			EndInitializeServer();
			InitializeWorkbookCore();
		}
#region Properties
		protected internal ModelWorkbook DocumentModel { get { return workbook; } }
		internal NativeWorksheetCollection NativeWorksheets { get { return worksheets; } }
		protected internal NativeStyleCache NativeStyleCache { get { return styleCache; } }
		protected internal NativeStyleCollection NativeCellStyles { get { return this.styleCollection; } }
		protected internal NativeTableStyleCollection NativeTableStyles { get { return this.tableStyleCollection; } }
		protected internal NativeExternalWorkbookCollection NativeExternalWorkbooks { get { return this.nativeExternalWorkbooks; } }
		protected internal NativeDocumentSettings NativeDocumentSettings { get { return documentSettings; } }
		protected internal ModelWorkbook ModelWorkbook { get { return workbook; } }
		protected internal UnitConverter UnitConverter { get { return unitConverter; } }
		protected internal InnerSpreadsheetDocumentServer Server { get { return server; } }
		protected internal NativeDefinedNameCollection NativeDefinedNames {
			get {
				if (definedNames == null) {
					definedNames = new NativeDefinedNameCollection(ModelWorkbook.DefinedNames, ModelWorkbook.DataContext, -1, worksheets);
				}
				return definedNames;
			}
		}
		protected internal NativeWorkbookFunctions NativeFunctions {
			get {
				if (nativeFunctions == null)
					nativeFunctions = new NativeWorkbookFunctions(this, ModelWorkbook.BuiltInOverridesFunctionProvider);
				return nativeFunctions;
			}
		}
		protected internal NativePivotCacheCollection NativePivotCaches {
			get {
				if (pivotCaches == null)
					pivotCaches = new NativePivotCacheCollection(this);
				return pivotCaches;
			}
		}
		protected internal NativeFormulaEngine NativeFormulas { get { return nativeFormulas; } }
		protected internal NativeHistory NativeHistory { get { return history; } }
		protected internal bool IsValid { get { return isValid; } set { isValid = value; } }
		protected internal bool IsDisposing { get { return isDisposing; } }
		public object Tag {
			get {
				CheckValid();
				return ModelWorkbook.Tag;
			}
			set {
				CheckValid();
				ModelWorkbook.Tag = value;
			}
		}
		protected internal NativeParametersCollection NativeParameters { get { return parameters; } }
#endregion
		protected internal void CheckValid() {
			if (!isValid)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseInvalidWorkbook);
		}
		protected internal virtual void InitializeWorkbookCore() {
			this.isValid = true;
			SubscribeServerEvents();
			CreateApiObjects();
			Initialize();
			SubscribeInternalAPIEvents();
			OnUnitsChanged();
		}
		protected internal virtual void CreateApiObjects() {
			this.worksheets = new NativeWorksheetCollection(this);
			this.styleCache = new NativeStyleCache();
			this.styleCollection = new NativeStyleCollection(this.ModelWorkbook);
			this.tableStyleCollection = new NativeTableStyleCollection(this);
			this.nativeExternalWorkbooks = new NativeExternalWorkbookCollection(this);
			this.documentSettings = new NativeDocumentSettings(ModelWorkbook.Properties);
			this.history = new NativeHistory(this.ModelWorkbook);
			this.nativeFormulas = new NativeFormulaEngine(this);
			this.parameters = new NativeParametersCollection(this);
			this.definedNames = null;
			this.pivotCaches = null;
		}
		protected internal virtual void Initialize() {
			PopulateWorksheets();
			this.styleCache.Clear();
			styleCollection.PopulateStyles();
			tableStyleCollection.PopulateStyles();
			this.nativeExternalWorkbooks.Initialize();
			if (this.definedNames != null)
				this.definedNames.Populate();
			if(this.parameters != null) {
				parameters.PopulateParameters();
			}
		}
		protected internal virtual void Invalidate() {
			UnsubscribeInternalAPIEvents();
			if (this.worksheets != null) {
				this.worksheets.Invalidate();
				this.worksheets = null;
			}
			if (this.definedNames != null) {
				this.definedNames.IsValid = false;
				this.definedNames = null;
			}
			if (this.styleCache != null) {
				this.styleCache.Clear();
				this.styleCache = null;
			}
			this.documentSettings = null;
			this.history = null;
			this.nativeFormulas = null;
			this.isValid = false;
			if (this.nativeExternalWorkbooks != null) {
				this.nativeExternalWorkbooks.Invalidate();
				this.nativeExternalWorkbooks = null;
			}
			if (this.pivotCaches != null) {
				this.pivotCaches.IsValid = false;
				this.pivotCaches = null;
			}
		}
		protected internal virtual void SubscribeInternalAPIEvents() {
			workbook.InnerDocumentCleared += OnDocumentCleared;
			workbook.AfterEndSetContent += OnAfterEndSetContent;
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			cellStyles.StyleAdded += OnCellStyleAdded;
			cellStyles.StyleRemoved += OnCellStyleRemoved;
			cellStyles.CollectionCleared += OnCellStyleCleared;
			Model.TableStyleCollection tableStyles = workbook.StyleSheet.TableStyles;
			tableStyles.StyleAdded += OnTableStyleAdded;
			tableStyles.StyleRemoved += OnTableStyleRemoved;
			tableStyles.CollectionCleared += OnTableStyleCleared;
			DevExpress.XtraSpreadsheet.API.Internal.InternalAPI internalApi = ModelWorkbook.InternalAPI;
			internalApi.ExternalLinkAdded += OnExternalLinkAdded;
			internalApi.ExternalLinkRemoved += OnExternalLinkRemoved;
			internalApi.CollectionClear += OnExternalLinksCleared;
			internalApi.AfterSheetInserted += OnAfterSheetInserted;
			internalApi.BeforeSheetRemoving += OnBeforeSheetRemoving;
			internalApi.AfterSheetCollectionCleared += OnAfterSheetCollectionCleared;
			internalApi.AfterWoksheetMoved += OnAfterWoksheetMoved;
			internalApi.CustomFunctionEvaluation += NativeFunctions.OnCustomFunctionEvaluation;
			MailMergeParametersCollection mailMergeParameters = workbook.MailMergeParameters;
			mailMergeParameters.ParameterAdded += OnParameterAdded;
			mailMergeParameters.ParameterInserted += OnParameterInserted;
			mailMergeParameters.ParameterRemoved += OnParameterRemoved;
			mailMergeParameters.CollectionCleared += OnParametersCleared;
		}
		protected internal virtual void UnsubscribeInternalAPIEvents() {
			if (workbook == null)
				return;
			workbook.InnerDocumentCleared -= OnDocumentCleared;
			workbook.AfterEndSetContent -= OnAfterEndSetContent;
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			cellStyles.StyleAdded -= OnCellStyleAdded;
			cellStyles.StyleRemoved -= OnCellStyleRemoved;
			cellStyles.CollectionCleared -= OnCellStyleCleared;
			Model.TableStyleCollection tableStyles = workbook.StyleSheet.TableStyles;
			tableStyles.StyleAdded -= OnTableStyleAdded;
			tableStyles.StyleRemoved -= OnTableStyleRemoved;
			tableStyles.CollectionCleared -= OnTableStyleCleared;
			DevExpress.XtraSpreadsheet.API.Internal.InternalAPI internalApi = ModelWorkbook.InternalAPI;
			internalApi.ExternalLinkAdded -= OnExternalLinkAdded;
			internalApi.ExternalLinkRemoved -= OnExternalLinkRemoved;
			internalApi.CollectionClear -= OnExternalLinksCleared;
			internalApi.AfterSheetInserted -= OnAfterSheetInserted;
			internalApi.BeforeSheetRemoving -= OnBeforeSheetRemoving;
			internalApi.AfterSheetCollectionCleared -= OnAfterSheetCollectionCleared;
			internalApi.AfterWoksheetMoved -= OnAfterWoksheetMoved;
			internalApi.CustomFunctionEvaluation -= NativeFunctions.OnCustomFunctionEvaluation;
			MailMergeParametersCollection mailMergeParameters = workbook.MailMergeParameters;
			mailMergeParameters.ParameterAdded -= OnParameterAdded;
			mailMergeParameters.ParameterInserted -= OnParameterInserted;
			mailMergeParameters.ParameterRemoved -= OnParameterRemoved;
			mailMergeParameters.CollectionCleared -= OnParametersCleared;
		}
		protected internal virtual void SubscribeServerEvents() {
			server.UnitChanged += OnServerUnitChanged;
		}
		protected internal virtual void UnsubscribeServerEvents() {
			server.UnitChanged -= OnServerUnitChanged;
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			Initialize();
		}
		void OnAfterWoksheetMoved(object sender, WorksheetMovedEventArgs e) {
			this.NativeWorksheets.InternalAPI_AfterWoksheetMoved(sender, e);
		}
		protected internal virtual void OnAfterEndSetContent(object sender, EventArgs e) {
			Initialize();
		}
		void OnServerUnitChanged(object sender, EventArgs e) {
			OnUnitsChanged();
		}
		protected internal virtual void OnAfterSheetCollectionCleared(object sender, EventArgs args) {
			this.NativeWorksheets.InternalAPI_AfterSheetCollectionCleared(sender, args);
		}
		protected internal virtual void OnBeforeSheetRemoving(object sender, WorksheetCollectionChangedEventArgs args) {
			this.NativeWorksheets.InternalAPI_BeforeSheetRemoving(sender, args);
		}
		protected internal virtual void OnAfterSheetInserted(object sender, WorksheetCollectionChangedEventArgs args) {
			this.NativeWorksheets.InternalAPI_AfterSheetInserted(sender, args);
		}
		protected internal virtual void OnCellStyleAdded(object sender, StyleCollectionChangedEventArgs args) {
			if (styleCollection != null)
				styleCollection.OnCellStyles_StyleAdded(sender, args);
		}
		protected internal virtual void OnCellStyleRemoved(object sender, StyleCollectionChangedEventArgs args) {
			if (styleCollection != null)
				styleCollection.OnCellStyles_StyleRemoved(sender, args);
		}
		protected internal virtual void OnCellStyleCleared(object sender, EventArgs e) {
			if (styleCollection != null)
				styleCollection.OnCellStyles_Cleared(sender, e);
		}
		protected internal virtual void OnTableStyleAdded(object sender, TableStyleCollectionChangedEventArgs args) {
			if (tableStyleCollection != null)
				tableStyleCollection.OnTableStyles_StyleAdded(sender, args);
		}
		protected internal virtual void OnTableStyleRemoved(object sender, TableStyleCollectionChangedEventArgs args) {
			if (tableStyleCollection != null)
				tableStyleCollection.OnTableStyles_StyleRemoved(sender, args);
		}
		protected internal virtual void OnTableStyleCleared(object sender, EventArgs e) {
			if (tableStyleCollection != null)
				tableStyleCollection.OnTableStyles_Cleared(sender, e);
		}
		protected internal virtual void OnExternalLinkAdded(object sender, ExternalLinksCollectionChangedEventArgs args) {
			if (ExternalWorkbooks != null)
				NativeExternalWorkbooks.OnExternalLinks_LinkAdded(sender, args);
		}
		protected internal virtual void OnExternalLinkRemoved(object sender, ExternalLinksCollectionChangedEventArgs args) {
			if (ExternalWorkbooks != null)
				NativeExternalWorkbooks.OnExternalLinks_LinkRemoved(sender, args);
		}
		protected internal virtual void OnExternalLinksCleared(object sender, EventArgs e) {
			if (ExternalWorkbooks != null)
				NativeExternalWorkbooks.OnExternalLinks_Cleared(sender, e);
		}
		protected internal virtual void OnParameterAdded(object sender, ParametersCollectionChangedEventArgs e) {
			NativeParameter parameter = new NativeParameter(e.Parameter);
			this.parameters.AddCore(parameter);
		}
		protected internal virtual void OnParameterInserted(object sender, ParameterInsertedEventArgs e) {
			NativeParameter parameter = new NativeParameter(e.Parameter);
			this.parameters.InsetCore(e.Index, parameter);
		}
		protected internal virtual void OnParameterRemoved(object sender, ParametersCollectionChangedEventArgs e) {
			for (int i = 0; i < parameters.Count; i++) {
				NativeParameter parameter = parameters[i] as NativeParameter;
				if (parameter == null || parameter.ModelParameter != e.Parameter || !parameter.IsValid)
					continue;
				parameters.RemoveCore(parameter);
				break;
			}
		}
		protected internal virtual void OnParametersCleared(object sender, EventArgs e) {
			parameters.Clear();
		}
#region Worksheets
		protected internal void PopulateWorksheets() {
			this.worksheets.Invalidate();
			ModelWorkbook.Sheets.ForEach(RegisterWorksheet);
		}
		protected internal void RegisterWorksheet(DevExpress.XtraSpreadsheet.Model.InternalSheetBase worksheet) {
			ModelWorksheet modelWorksheet = worksheet as ModelWorksheet; 
			if (modelWorksheet == null)
				return;
			NativeWorksheet nativeWorksheet;
#if DATA_SHEET
			Model.VirtualWorksheet virtualWorksheet = modelWorksheet as Model.VirtualWorksheet;
			if (virtualWorksheet != null)
				nativeWorksheet = new NativeVirtualWorksheet(this, virtualWorksheet);
			else
#endif
				nativeWorksheet = new NativeWorksheet(this, modelWorksheet);
			this.worksheets.AddCore(nativeWorksheet);
		}
		protected internal DevExpress.Spreadsheet.Worksheet AddWorksheet(string name) {
			ModelWorksheet sheet = CreateWorksheet(name);
			ModelWorkbook.Sheets.Add(sheet);
			int index = ModelWorkbook.Sheets.Count - 1;
			ModelWorkbook.ActiveSheet = sheet;
			return NativeWorksheets[index];
		}
#if DATA_SHEET
		protected internal DevExpress.Spreadsheet.VirtualWorksheet AddVirtualWorksheet(string name, IVirtualData virtualData) {
			Model.VirtualWorksheet sheet = CreateVirtualWorksheet(name, virtualData);
			ModelWorkbook.Sheets.Add(sheet);
			int index = ModelWorkbook.Sheets.Count - 1;
			ModelWorkbook.ActiveSheet = sheet;
			return (NativeVirtualWorksheet)NativeWorksheets[index];
		}
#endif
		ModelWorksheet CreateWorksheet(string name) {
			return String.IsNullOrEmpty(name) ? new ModelWorksheet(ModelWorkbook) : new ModelWorksheet(ModelWorkbook, name);
		}
#if DATA_SHEET
		Model.VirtualWorksheet CreateVirtualWorksheet(string name, IVirtualData virtualData) {
			return new Model.VirtualWorksheet(ModelWorkbook, name, virtualData);
		}
#endif
#endregion
#region Load/Save
		bool LoadDocumentCore(string fileName) {
			if (server != null)
				return server.LoadDocument(fileName);
			return false;
		}
		bool LoadDocumentCore(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if (server != null)
				return server.LoadDocument(stream, format);
			return false;
		}
		bool LoadDocumentCore(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if (server != null)
				return server.LoadDocument(fileName, format);
			return false;
		}
		void SaveDocumentCore(Stream stream, DevExpress.Spreadsheet.DocumentFormat format) {
			if (server != null)
				server.SaveDocument(stream, format);
		}
		void SaveDocumentCore(string fileName) {
			if (server != null)
				server.SaveDocument(fileName);
		}
		void SaveDocumentCore(string fileName, DevExpress.Spreadsheet.DocumentFormat format) {
			if (server != null)
				server.SaveDocument(fileName, format);
		}
#endregion
		InnerSpreadsheetDocumentServer CreateServer(ModelWorkbook modelWorkbook) {
			if (modelWorkbook == null)
				return new InnerSpreadsheetDocumentServer(this);
			else
				return new InnerSpreadsheetDocumentServer(this, modelWorkbook);
		}
		protected internal virtual void BeginInitializeServer() {
			server.BeginInitialize(false);
		}
		protected internal virtual void EndInitializeServer() {
			server.EndInitialize();
		}
#region Unit conversion
		protected internal virtual void OnUnitsChanged() {
			this.unitConverter = ModelWorkbook.InternalAPI.UnitConverters[Unit];
		}
		protected internal virtual int UnitsToLayoutUnits(float value) {
			float modelValue = UnitConverter.ToUnits(value);
			return (int)Math.Round(ModelWorkbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelValue));
		}
		protected internal virtual float LayoutUnitsToUnits(int value) {
			float modelValue = ModelWorkbook.ToDocumentLayoutUnitConverter.ToModelUnits(value);
			return UnitConverter.FromUnits(modelValue);
		}
		protected internal virtual int UnitsToModelUnits(float value) {
			return (int)Math.Round(UnitConverter.ToUnits(value));
		}
		protected internal virtual float UnitsToModelUnitsF(float value) {
			return UnitConverter.ToUnits(value);
		}
		protected internal virtual float ModelUnitsToUnits(int value) {
			return UnitConverter.FromUnits(value);
		}
		protected internal virtual float ModelUnitsToUnitsF(float value) {
			return UnitConverter.FromUnits(value);
		}
		protected internal virtual float? ModelUnitsToUnits(int? value) {
			if (value.HasValue)
				return ModelUnitsToUnits(value.Value);
			else
				return null;
		}
		protected internal virtual float? ModelUnitsToUnitsF(float? value) {
			if (value.HasValue)
				return ModelUnitsToUnitsF(value.Value);
			else
				return null;
		}
#endregion
#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				this.isValid = false;
				this.isDisposing = false;
				this.isDisposed = true;
			}
		}
		void DisposeCore() {
			Invalidate();
			if (server != null) {
				UnsubscribeServerEvents();
				if (!isExternalServer) {
					server.Dispose();
					server = null;
				}
			}
		}
#endregion
#region IInnerSpreadsheetDocumentServerOwner Members
		MeasurementAndDrawingStrategy IInnerSpreadsheetDocumentServerOwner.CreateMeasurementAndDrawingStrategy(ModelWorkbook documentModel) {
#if !SL && !DXPORTABLE
			if (PrecalculatedMetricsFontCacheManager.ShouldUse())
				return new ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(documentModel);
			else
				return new ServerGdiMeasurementAndDrawingStrategy(documentModel);
#else
			return new ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(documentModel);
#endif
		}
		DocumentOptions IInnerSpreadsheetDocumentServerOwner.CreateOptions(InnerSpreadsheetDocumentServer documentServer) {
			return new SpreadsheetDocumentServerOptions(documentServer);
		}
		void IInnerSpreadsheetDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			IThreadSyncService service = server.GetService<IThreadSyncService>();
			if (service != null)
				service.EnqueueInvokeInUIThread(new Action(delegate () { server.RaiseDeferredEventsCore(changeActions); }));
			else
				server.RaiseDeferredEventsCore(changeActions);
		}
#endregion
#region InvalidFormatException
		public event InvalidFormatExceptionEventHandler InvalidFormatException { add { server.InvalidFormatException += value; } remove { server.InvalidFormatException -= value; } }
#endregion
		public IEnumerable<Cell> Search(string text) {
			ModelSearchOptions modelOptions = CreateDefaultSearchOptions();
			modelOptions.DocumentModel = workbook;
			return SearchCore(text, modelOptions, ConvertModelCellToApiCell);
		}
		public IEnumerable<Cell> Search(string text, SearchOptions options) {
			ModelSearchOptions modelOptions = ConvertSearchOptions(options);
			modelOptions.DocumentModel = workbook;
			return SearchCore(text, modelOptions, ConvertModelCellToApiCell);
		}
		internal ModelSearchOptions ConvertSearchOptions(SearchOptions options) {
			ModelSearchOptions result = new ModelSearchOptions();
			result.LookIn = (ModelSearchLookIn)options.SearchIn;
			result.Direction = (ModelSearchDirection)options.SearchBy;
			result.MatchEntireCellContents = options.MatchEntireCellContents;
			result.MatchCase = options.MatchCase;
			return result;
		}
		internal ModelSearchOptions CreateDefaultSearchOptions() {
			ModelSearchOptions options = new ModelSearchOptions();
			options.LookIn = ModelSearchLookIn.ValuesAndFormulas;
			options.Direction = ModelSearchDirection.ByRows;
			options.MatchEntireCellContents = false;
			options.MatchCase = false;
			return options;
		}
		internal IEnumerable<Cell> SearchCore(string text, ModelSearchOptions options, ConvertMethod<Model.ICell, Cell> convert) {
			SpreadsheetSearchEngine engine = new SpreadsheetSearchEngine();
			IEnumerator<Model.ICell> modelCells = engine.Search(text, options);
			IEnumerator<Cell> cells = new EnumeratorConverter<Model.ICell, Cell>(modelCells, convert);
			return new Enumerable<Cell>(cells);
		}
		Cell ConvertModelCellToApiCell(Model.ICell cell) {
			return worksheets[cell.Sheet.Name][cell.RowIndex, cell.ColumnIndex];
		}
	}
#endregion
}
#endregion
