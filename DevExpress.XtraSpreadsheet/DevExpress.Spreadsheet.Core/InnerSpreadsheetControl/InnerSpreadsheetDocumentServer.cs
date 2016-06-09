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
using System.ComponentModel;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Internal {
	public interface IInnerSpreadsheetDocumentServerOwner {
		MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(DocumentModel documentModel);
		DocumentOptions CreateOptions(InnerSpreadsheetDocumentServer documentServer);
		void RaiseDeferredEvents(DocumentModelChangeActions changeActions);
	}
	public partial class InnerSpreadsheetDocumentServer : ISpreadsheetComponent, IDisposable {
		#region Fields
		readonly IInnerSpreadsheetDocumentServerOwner owner;
		bool isDisposed;
		bool invalidFormatExceptionRised;
		DocumentModelAccessor model;
		DocumentModel workbook;
		DocumentModel existingWorkbook;
		DocumentOptions options;
		MeasurementAndDrawingStrategy measurementAndDrawingStrategy;
		DocumentUnit unit;
		NativeWorkbook nativeDocument;
		PredefinedFontSizeCollection predefinedFontSizeCollection;
		DocumentDeferredChanges documentDeferredChanges;
		DocumentDeferredChanges documentDeferredChangesOnIdle;
		#endregion
		public InnerSpreadsheetDocumentServer(IInnerSpreadsheetDocumentServerOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public InnerSpreadsheetDocumentServer(IInnerSpreadsheetDocumentServerOwner owner, DocumentModel workbook) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(workbook, "workbook");
			this.owner = owner;
			this.existingWorkbook = workbook;
		}
		#region Properties
		public IInnerSpreadsheetDocumentServerOwner Owner { get { return owner; } }
		public bool IsDisposed { get { return isDisposed; } }
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return workbook.BatchUpdateHelper; } }
		[Browsable(false)]
		public bool IsUpdateLocked { get { return workbook.IsUpdateLocked; } }
		public DocumentModel DocumentModel { get { return workbook; } }
		public DocumentModelAccessor Model { get { return model; } }
		public DocumentOptions Options { get { return options; } }
		public MeasurementAndDrawingStrategy MeasurementAndDrawingStrategy { get { return measurementAndDrawingStrategy; } }
		#region Modified
		public bool Modified {
			get { return DocumentModel.Modified; }
			set { DocumentModel.Modified = value; }
		}
		#endregion
		#region LayoutUnit
		public DocumentLayoutUnit LayoutUnit {
			get { return DocumentModel.LayoutUnit; }
			set {
				if (value == LayoutUnit)
					return;
				SetLayoutUnit(value);
			}
		}
		#endregion
		#region Unit
		public DocumentUnit Unit {
			get { return unit; }
			set {
				if (unit == value)
					return;
				SetUnit(value);
			}
		}
		#endregion
		#region UIUnit
		public DocumentUnit UIUnit {
			get {
				if (Unit == DocumentUnit.Document)
					return DocumentUnit.Inch;
				else
					return Unit;
			}
		}
		#endregion
		public IClipboardManager Clipboard {
			get {
				return new DevExpress.XtraSpreadsheet.Commands.Internal.CopySelectionManager(DocumentModel, false);
			}
		}
		public virtual bool IsReadOnly { get { return false; } set { } }
		protected internal virtual bool ActualReadOnly { get { return false; } }
		public virtual bool IsEnabled { get { return true; } }
		internal bool IsEditable { get { return !IsReadOnly && IsEnabled; } }
		protected internal DocumentDeferredChanges DocumentDeferredChanges { get { return documentDeferredChanges; } }
		public PredefinedFontSizeCollection PredefinedFontSizeCollection { get { return predefinedFontSizeCollection; } }
		public IWorkbook Document { get { return NativeDocument; } }
		internal NativeWorkbook NativeDocument {
			get {
				if (nativeDocument == null)
					nativeDocument = CreateNativeDocument();
				return nativeDocument;
			}
		}
		internal virtual NativeWorkbook CreateNativeDocument() {
			return new NativeWorkbook(this);
		}
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				if (options != null) {
					UnsubscribeOptionsEvents();
					options = null;
				}
				if (workbook != null) {
					UnsubscribeDocumentModelEvents();
					if (workbook != existingWorkbook)
						workbook.Dispose();
					workbook = null;
				}
				if (measurementAndDrawingStrategy != null) {
					measurementAndDrawingStrategy.Dispose();
					measurementAndDrawingStrategy = null;
				}
				this.predefinedFontSizeCollection = null;
			}
			this.isDisposed = true;
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			DocumentModel.BeginUpdate();
		}
		public void EndUpdate() {
			DocumentModel.EndUpdate();
		}
		public void CancelUpdate() {
			DocumentModel.CancelUpdate();
		}
		#endregion
		public virtual void BeginInitialize(bool keepExistingContent) {
			if (keepExistingContent && this.workbook != null) {
				this.workbook.VisualControlAssigned = true;
				return;
			}
			this.workbook = GetDocumentModel();
			this.workbook.VisualControlAssigned = true;
			this.model = new DocumentModelAccessor(workbook);
			this.predefinedFontSizeCollection = new PredefinedFontSizeCollection();
			SubscribeDocumentModelEvents();
			this.options = CreateOptions();
		}
		public virtual void EndInitialize() {
			if (existingWorkbook == null) {
				workbook.AddInitialSheets();
				workbook.PrepareStyleSheet();
			}
			UnsubscribeOptionsEvents();
			SubscribeOptionsEvents();
			CreateNewMeasurementAndDrawingStrategy();
			AddServices();
		}
		void AddServices() {
			AddService(typeof(Spreadsheet.Functions.ICustomFunctionDescriptionsRegisterService), new NativeCustomFunctionDescriptionsRegisterService(workbook));
		}
		protected internal virtual DocumentModel GetDocumentModel() {
			if (existingWorkbook != null)
				return existingWorkbook;
			return CreateDocumentModelCore();
		}
		protected internal virtual DocumentModel CreateDocumentModelCore() {
			return new DocumentModel();
		}
		protected internal virtual void SubscribeDocumentModelEvents() {
			DocumentModel.BeginDocumentUpdate += OnBeginDocumentUpdate;
			DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
			DocumentModel.InnerSelectionChanged += OnInnerSelectionChanged;
			DocumentModel.SelectionChanged += OnSelectionChanged;
			DocumentModel.InnerContentChanged += OnInnerContentChanged;
			DocumentModel.ContentChanged += OnContentChanged;
			DocumentModel.SchemaChanged += OnSchemaChanged;
			DocumentModel.ModifiedChanged += OnModifiedChanged;
			DocumentModel.BeforeExport += OnBeforeExport;
			DocumentModel.BeforeImport += OnBeforeImport;
			DocumentModel.DocumentCleared += OnDocumentCleared;
			DocumentModel.InvalidFormatException += OnInvalidFormatException;
			DocumentModel.ActiveSheetChanging += OnActiveSheetChanging;
			DocumentModel.ActiveSheetChanged += OnActiveSheetChanged;
			DocumentModel.InnerActiveSheetChanged += OnInnerActiveSheetChanged;
			DocumentModel.SheetRenaming += OnSheetRenaming;
			DocumentModel.SheetRenamed += OnSheetRenamed;
			DocumentModel.SheetInserted += OnSheetInserted;
			DocumentModel.SheetRemoved += OnSheetRemoved;
			DocumentModel.RowsRemoved += OnRowsRemoved;
			DocumentModel.RowsInserted += OnRowsInserted;
			DocumentModel.ColumnsRemoved += OnColumnsRemoved;
			DocumentModel.ColumnsInserted += OnColumnsInserted;
			DocumentModel.PanesUnfrozen += OnPanesUnfrozen;
			DocumentModel.PanesFrozen += OnPanesFrozen;
			DocumentModel.InternalAPI.CellValueChanged += OnCellValueChanged;
			DocumentModel.ScrollPositionChanged += OnScrollPositionChanged;
			DocumentModel.CopyingRange += OnRangeCopying;
			DocumentModel.RangeCopied += OnCopiedRange;
			DocumentModel.CopyingShapes += OnShapesCopying;
			DocumentModel.CopiedRangePasting += OnCopiedRangePasting;
			DocumentModel.CopiedRangePasted += OnCopiedRangePasted;
			DocumentModel.ClipboardDataPasting += OnClipboardDataPasting;
			DocumentModel.ClipboardDataObtained += OnClipboardDataObtained;
			DocumentModel.ClipboardDataPasted += OnClipboardDataPasted;
		}		
		protected internal virtual void UnsubscribeDocumentModelEvents() {
			DocumentModel.BeginDocumentUpdate -= OnBeginDocumentUpdate;
			DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
			DocumentModel.InnerSelectionChanged -= OnInnerSelectionChanged;
			DocumentModel.SelectionChanged -= OnSelectionChanged;
			DocumentModel.InnerContentChanged -= OnInnerContentChanged;
			DocumentModel.ContentChanged -= OnContentChanged;
			DocumentModel.SchemaChanged -= OnSchemaChanged;
			DocumentModel.ModifiedChanged -= OnModifiedChanged;
			DocumentModel.BeforeExport -= OnBeforeExport;
			DocumentModel.BeforeImport -= OnBeforeImport;
			DocumentModel.DocumentCleared -= OnDocumentCleared;
			DocumentModel.InvalidFormatException -= OnInvalidFormatException;
			DocumentModel.ActiveSheetChanging -= OnActiveSheetChanging;
			DocumentModel.ActiveSheetChanged -= OnActiveSheetChanged;
			DocumentModel.InnerActiveSheetChanged -= OnInnerActiveSheetChanged;
			DocumentModel.SheetRenaming -= OnSheetRenaming;
			DocumentModel.SheetRenamed -= OnSheetRenamed;
			DocumentModel.SheetInserted -= OnSheetInserted;
			DocumentModel.SheetRemoved -= OnSheetRemoved;
			DocumentModel.RowsRemoved -= OnRowsRemoved;
			DocumentModel.RowsInserted -= OnRowsInserted;
			DocumentModel.ColumnsRemoved -= OnColumnsRemoved;
			DocumentModel.ColumnsInserted -= OnColumnsInserted;
			DocumentModel.PanesUnfrozen -= OnPanesUnfrozen;
			DocumentModel.PanesFrozen -= OnPanesFrozen;
			DocumentModel.InternalAPI.CellValueChanged -= OnCellValueChanged;
			DocumentModel.ScrollPositionChanged -= OnScrollPositionChanged;
			DocumentModel.CopyingRange -= OnRangeCopying;
			DocumentModel.RangeCopied -= OnCopiedRange;
			DocumentModel.CopyingShapes -= OnShapesCopying;
			DocumentModel.CopiedRangePasting -= OnCopiedRangePasting;
			DocumentModel.CopiedRangePasted -= OnCopiedRangePasted;
			DocumentModel.ClipboardDataPasting -= OnClipboardDataPasting;
			DocumentModel.ClipboardDataObtained -= OnClipboardDataObtained;
			DocumentModel.ClipboardDataPasted -= OnClipboardDataPasted;
		}
		#region Measurement And Drawing Strategy
		protected internal virtual MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy() {
			return Owner.CreateMeasurementAndDrawingStrategy(DocumentModel);
		}
		protected internal virtual void CreateNewMeasurementAndDrawingStrategy() {
			this.measurementAndDrawingStrategy = CreateMeasurementAndDrawingStrategy();
			this.measurementAndDrawingStrategy.Initialize();
			DocumentModel.SetFontCacheManager(measurementAndDrawingStrategy.CreateFontCacheManager());
		}
		public virtual void RecreateMeasurementAndDrawingStrategy() {
			BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					RecreateMeasurementAndDrawingStrategyCore();
					DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetSpellingCheck;
					DocumentModel.ApplyChanges(changeActions);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				EndUpdate();
			}
		}
		void RecreateMeasurementAndDrawingStrategyCore() {
			if (measurementAndDrawingStrategy != null)
				measurementAndDrawingStrategy.Dispose();
			CreateNewMeasurementAndDrawingStrategy();
		}
		#endregion
		#region Options
		protected internal virtual DocumentOptions CreateOptions() {
			return Owner.CreateOptions(this);
		}
		protected internal virtual void SubscribeOptionsEvents() {
			Options.Changed += OnOptionsChanged;
		}
		protected internal virtual void UnsubscribeOptionsEvents() {
			Options.Changed -= OnOptionsChanged;
		}
		protected internal virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
		}
		#endregion
		#region Save/Load content
		public void CreateNewDocument() {
			invalidFormatExceptionRised = false;
			DocumentModel.InternalAPI.CreateNewDocument();
		}
		public bool LoadDocument(byte[] buffer, DocumentFormat documentFormat) {
			using (MemoryStream stream = new MemoryStream(buffer)) {
				return LoadDocument(stream, documentFormat);
			}
		}
		public bool LoadDocument(Stream stream, DocumentFormat documentFormat) {
			return LoadDocumentCore(stream, documentFormat, String.Empty);
		}
		public bool LoadDocument(string fileName) {
			return LoadDocument(fileName, DocumentFormat.Undefined);
		}
		public bool LoadDocument(string fileName, DocumentFormat documentFormat) {
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				if (documentFormat == DocumentFormat.Undefined)
					documentFormat = DocumentModel.AutodetectDocumentFormat(fileName);
				WorkbookSaveOptions saveOptions = DocumentModel.DocumentSaveOptions;
				string previousFileName = saveOptions.CurrentFileName;
				DocumentFormat previousDocumentFormat = saveOptions.CurrentFormat;
				try {
					SetupSaveOptions(saveOptions, fileName, documentFormat);
					return LoadDocumentCore(fileStream, documentFormat, fileName);
				}
				catch {
					RestoreSaveOptions(saveOptions, previousFileName, previousDocumentFormat);
					throw;
				}
			}
		}
		bool LoadDocumentCore(Stream stream, DocumentFormat documentFormat, string fileName) {
			invalidFormatExceptionRised = false;
			DocumentModel.LoadDocument(stream, documentFormat, fileName);
			return !invalidFormatExceptionRised;
		}
		public static void SetupSaveOptions(WorkbookSaveOptions saveOptions, string fileName, DocumentFormat documentFormat) {
			saveOptions.CurrentFileName = fileName;
			saveOptions.CurrentFormat = IsTemplateFormat(documentFormat) ? DocumentFormat.Undefined : documentFormat;
		}
		public static void RestoreSaveOptions(WorkbookSaveOptions saveOptions, string previousFileName, DocumentFormat previousDocumentFormat) {
			saveOptions.CurrentFileName = previousFileName;
			saveOptions.CurrentFormat = previousDocumentFormat;
		}
		static bool IsTemplateFormat(DocumentFormat documentFormat) {
			return documentFormat == DocumentFormat.Xlt || documentFormat == DocumentFormat.Xltx || documentFormat == DocumentFormat.Xltm;
		}
		public void SaveDocument(Stream stream, DocumentFormat documentFormat) {
			SaveDocumentCore(stream, documentFormat, String.Empty);
		}
		public void SaveDocument(string fileName) {
			SaveDocument(fileName, DocumentModel.AutodetectDocumentFormat(fileName));
		}
		public byte[] SaveDocument(DocumentFormat documentFormat) {
			using (MemoryStream stream = new MemoryStream()) {
				SaveDocumentCore(stream, documentFormat, String.Empty);
				return stream.ToArray();
			}
		}
		public void SaveDocument(string fileName, DocumentFormat documentFormat) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				WorkbookSaveOptions options = DocumentModel.DocumentSaveOptions;
				string prevFileName = options.CurrentFileName;
				DocumentFormat prevDocumentFormat = options.CurrentFormat;
				options.CurrentFileName = fileName;
				options.CurrentFormat = documentFormat;
				try {
					SaveDocumentCore(stream, documentFormat, fileName);
				}
				catch {
					options.CurrentFileName = prevFileName;
					options.CurrentFormat = prevDocumentFormat;
					throw;
				}
			}
		}
		void SaveDocumentCore(Stream stream, DocumentFormat documentFormat, string targetUri) {
			DocumentModel.SaveDocument(stream, documentFormat, targetUri);
			RaiseDocumentPropertiesChanged(true, false);
		}
		public void ExportToHtml(Stream stream, int sheetIndex) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, sheetIndex, null);
		}
		public void ExportToHtml(Stream stream, DevExpress.Spreadsheet.Worksheet sheet) {
			if (sheet != null)
				((NativeWorksheet)sheet).NativeWorkbook.ExportToHtml(stream, sheet);
		}
		public void ExportToHtml(Stream stream, DevExpress.Spreadsheet.Range range) {
			if (range != null)
				((NativeWorksheet)range.Worksheet).NativeWorkbook.ExportToHtml(stream, range);
		}
		public void ExportToHtml(Stream stream, HtmlDocumentExporterOptions options) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, options);
		}
		public void ExportToHtml(string fileName, int sheetIndex) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, sheetIndex, null);
		}
		public void ExportToHtml(string fileName, DevExpress.Spreadsheet.Worksheet sheet) {
			if (sheet != null)
				((NativeWorksheet)sheet).NativeWorkbook.ExportToHtml(fileName, sheet);
		}
		public void ExportToHtml(string fileName, DevExpress.Spreadsheet.Range range) {
			if (range != null)
				((NativeWorksheet)range.Worksheet).NativeWorkbook.ExportToHtml(fileName, range);
		}
		public void ExportToHtml(string fileName, HtmlDocumentExporterOptions options) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, options);
		}
		#endregion
		protected internal virtual bool CanCloseExistingDocument() {
			if (!Modified)
				return true;
			return RaiseDocumentClosing();
		}
		public virtual void OnUpdateUI() {
		}
		protected internal virtual void OnModifiedChanged(object sender, EventArgs e) {
			OnModifiedChangedCore();
			OnUpdateUI();
		}
		protected internal virtual void OnModifiedChangedCore() {
			RaiseModifiedChanged();
		}
		protected internal virtual void OnSchemaChanged(object sender, EventArgs e) {
			OnSchemaChangedCore();
		}
		protected internal virtual void OnSchemaChangedCore() {
			RaiseSchemaChanged();
		}
		protected internal virtual void OnBeforeExport(object sender, SpreadsheetBeforeExportEventArgs e) {
			RaiseBeforeExport(e);
		}
		protected internal virtual void OnBeforeImport(object sender, SpreadsheetBeforeImportEventArgs e) {
			RaiseBeforeImport(e);
		}
		protected internal virtual void OnDocumentCleared(object sender, EventArgs e) {
			RaiseInitializeDocument(e);
		}
		protected internal virtual void OnInvalidFormatException(object sender, SpreadsheetInvalidFormatExceptionEventArgs e) {
			RaiseInvalidFormatException(e.Exception);
			this.invalidFormatExceptionRised = true;
			if (Options.Import.ThrowExceptionOnInvalidDocument)
				throw e.Exception;
		}
		protected internal virtual void OnActiveSheetChanging(object sender, ActiveSheetChangingEventArgs e) {
			RaiseActiveSheetChanging(e);
		}
		protected internal virtual void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			RaiseActiveSheetChanged(e);
		}
		protected internal virtual void OnInnerActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
		}
		void OnSheetRenaming(object sender, SheetRenamingEventArgs e) {
			RaiseSheetRenaming(e);
		}
		void OnSheetRenamed(object sender, SheetRenamedEventArgs e) {
			RaiseSheetRenamed(e);
		}
		void OnSheetInserted(object sender, SheetInsertedEventArgs e) {
			RaiseSheetInserted(e);
		}
		void OnSheetRemoved(object sender, SheetRemovedEventArgs e) {
			RaiseSheetRemoved(e);
		}
		void OnRowsRemoved(object sender, RowsChangedEventArgs e) {
			RaiseRowsRemoved(e);
		}
		void OnRowsInserted(object sender, RowsChangedEventArgs e) {
			RaiseRowsInserted(e);
		}
		void OnColumnsRemoved(object sender, ColumnsChangedEventArgs e) {
			RaiseColumnsRemoved(e);
		}
		void OnColumnsInserted(object sender, ColumnsChangedEventArgs e) {
			RaiseColumnsInserted(e);
		}
		void OnPanesFrozen(object sender, PanesFrozenEventArgs e) {
			RaisePanesFrozen(e);
		}
		void OnCellValueChanged(object sender, SpreadsheetCellEventArgs e) {
			RaiseCellValueChanged(e);
		}
		void OnPanesUnfrozen(object sender, PanesUnfrozenEventArgs e) {
			RaisePanesUnfrozen(e);
		}
		void OnScrollPositionChanged(object sender, ScrollPositionChangedEventArgs e) {
			RaiseScrollPositionChanged(e);
		}
		void OnClipboardDataPasted(object sender, EventArgs e) {
			RaiseClipboardDataPasted(e);
		}
		void OnClipboardDataObtained(object sender, ClipboardDataObtainedEventArgs e) {
			RaiseClipboardDataObtained(e);
		}
		void OnClipboardDataPasting(object sender, EventArgs e) {
			RaiseClipboardDataPasting(e);
		}
		void OnCopiedRangePasted(object sender, CopiedRangePastedEventArgs e) {
			RaiseCopiedRangePasted(e);
		}
		void OnCopiedRangePasting(object sender, CopiedRangePastingEventArgs e) {
			RaiseCopiedRangePasting(e);
		}
		void OnRangeCopying(object sender, RangeCopyingEventArgs e) {
			RaiseRangeCopying(e);
		}
		void OnCopiedRange(object sender, RangeCopiedEventArgs e) {
			RaiseRangeCopied(e);
		}
		void OnShapesCopying(object sender, ShapesCopyingEventArgs e) {
			RaiseShapesCopying(e);
		}
		protected internal virtual void RaiseCellValueChanged(SpreadsheetCellEventArgs args) {
		}
		protected internal virtual void RaiseDocumentPropertiesChanged(bool builtInPropertiesChanged, bool customPropertiesChanged) {
		}
		protected internal virtual void OnEmptyDocumentCreated() {
			if (!ShouldRaiseEmptyDocumentCreated())
				return;
			bool historyEnabled = DocumentModel.IsNormalHistory && DocumentModel.History.Count <= 0;
			if (historyEnabled)
				DocumentModel.SwitchToEmptyHistory(false);
			try {
				RaiseEmptyDocumentCreated();
			}
			finally {
				if (historyEnabled)
					DocumentModel.SwitchToNormalHistory(false);
			}
		}
		protected internal virtual void OnDocumentLoaded() {
			bool historyEnabled = DocumentModel.IsNormalHistory;
			if (historyEnabled)
				DocumentModel.SwitchToEmptyHistory(false);
			try {
				RaiseDocumentLoaded();
			}
			finally {
				if (historyEnabled)
					DocumentModel.SwitchToNormalHistory(false);
			}
		}
		protected internal virtual void OnInnerSelectionChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnSelectionChanged(object sender, EventArgs e) {
			OnSelectionChangedCore();
			OnUpdateUI();
		}
		protected internal virtual void OnSelectionChangedCore() {
			RaiseSelectionChanged();
		}
		protected internal virtual void OnInnerContentChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnContentChanged(object sender, EventArgs e) {
			OnContentChangedCore(false);
			OnUpdateUI();
		}
		protected internal virtual void OnContentChangedCore(bool suppressBindingNotifications) {
			RaiseContentChanged(suppressBindingNotifications);
		}
		public virtual void OnApplicationIdle() {
			if (documentDeferredChangesOnIdle != null) {
				DocumentModel.BeginUpdate();
				try {
					DocumentModel.ApplyChanges(documentDeferredChangesOnIdle.ChangeActions);
				}
				finally {
					DocumentModel.EndUpdate();
				}
				documentDeferredChangesOnIdle = null;
			}
		}
		protected internal virtual void SetUnit(DocumentUnit value) {
			RaiseUnitChanging();
			this.unit = value;
			if (this.nativeDocument != null)
				this.nativeDocument.OnUnitsChanged();
			RaiseUnitChanged();
		}
		#region SetLayoutUnit
		protected internal virtual void SetLayoutUnit(DocumentLayoutUnit unit) {
			BeginUpdate();
			try {
				SetLayoutUnitCore(unit);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetLayoutUnitCore(DocumentLayoutUnit unit) {
			SetDocumentModelLayoutUnit(unit);
		}
		protected internal virtual void SetDocumentModelLayoutUnit(DocumentLayoutUnit unit) {
			DocumentModel.BeginUpdate();
			try {
				SetDocumentModelLayoutUnitCore(unit);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SetDocumentModelLayoutUnitCore(DocumentLayoutUnit unit) {
			DocumentModel.LayoutUnit = unit;
			RecreateMeasurementAndDrawingStrategy();
			MeasurementAndDrawingStrategy.OnLayoutUnitChanged();
		}
		public void SetInitialDocumentModelLayoutUnit(DocumentLayoutUnit unit) {
			System.Diagnostics.Debug.Assert(measurementAndDrawingStrategy == null);
			DocumentModel.LayoutUnit = unit;
		}
		#endregion
		protected internal virtual void OnBeginDocumentUpdate(object sender, EventArgs e) {
			this.documentDeferredChanges = new DocumentDeferredChanges();
			OnBeginDocumentUpdateCore();
		}
		protected internal virtual void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			OnEndDocumentUpdateCore(sender, e);
			this.documentDeferredChanges = null;
		}
		protected internal virtual void OnBeginDocumentUpdateCore() {
		}
		protected internal virtual DocumentModelChangeActions OnEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = ProcessEndDocumentUpdateCore(sender, e);
			ApplyChangesCore(changeActions);
			return changeActions;
		}
		protected internal virtual DocumentModelChangeActions ProcessEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = e.DeferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.PerformActionsOnIdle) != 0) {
				if (documentDeferredChangesOnIdle == null)
					this.documentDeferredChangesOnIdle = new DocumentDeferredChanges();
				documentDeferredChangesOnIdle.ChangeActions |= changeActions;
				documentDeferredChangesOnIdle.ChangeActions &= ~DocumentModelChangeActions.PerformActionsOnIdle;
#if !SL
				changeActions = DocumentModelChangeActions.None;
#endif
			}
			return changeActions;
		}
		protected internal virtual void ApplyChangesCore(DocumentModelChangeActions changeActions) {
			if (DocumentModel.IsUpdateLocked)
				DocumentDeferredChanges.ChangeActions |= changeActions;
			else
				RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			if (changeActions == DocumentModelChangeActions.None)
				return;
			Owner.RaiseDeferredEvents(changeActions);
		}
		public void RaiseDeferredEventsCore(DocumentModelChangeActions changeActions) {
			lock (this) {
				if (IsDisposed)
					return;
				PerformRaiseDeferredEventsCore(changeActions);
			}
		}
		protected internal virtual void PerformRaiseDeferredEventsCore(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0)
				OnEmptyDocumentCreated();
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0)
				OnDocumentLoaded();
			bool raiseUpdateUI =
				(changeActions & DocumentModelChangeActions.RaiseUpdateUI) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseModifiedChanged) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0;
			if ((changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0) {
				bool suppressBindingNotifications = (changeActions & DocumentModelChangeActions.SuppressBindingsNotifications) != 0;
				OnContentChangedCore(suppressBindingNotifications);
			}
			if ((changeActions & DocumentModelChangeActions.RaiseModifiedChanged) != 0)
				OnModifiedChangedCore();
			if ((changeActions & DocumentModelChangeActions.RaiseSchemaChanged) != 0)
				OnSchemaChangedCore();
			if ((changeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0)
				OnSelectionChangedCore();
			if (raiseUpdateUI)
				OnUpdateUI();
		}
	}
	#region SpreadsheetDocumentServerOptions
	public class SpreadsheetDocumentServerOptions : DocumentOptions {
		public SpreadsheetDocumentServerOptions(InnerSpreadsheetDocumentServer documentServer)
			: base(documentServer) {
		}
	}
	#endregion
	#region DocumentDeferredChanges
	public class DocumentDeferredChanges {
		DocumentModelChangeActions changeActions;
		public DocumentModelChangeActions ChangeActions { get { return changeActions; } set { changeActions = value; } }
	}
	#endregion
	#region SpreadsheetControlDeferredChanges
	public class SpreadsheetControlDeferredChanges {
		bool redraw;
		bool resize;
		bool raiseUpdateUI;
		public bool Redraw { get { return redraw; } set { redraw = value; } }
		public bool Resize { get { return resize; } set { resize = value; } }
		public bool RaiseUpdateUI { get { return raiseUpdateUI; } set { raiseUpdateUI = value; } }
		RefreshAction redrawAction;
		public RefreshAction RedrawAction { get { return redrawAction; } set { redrawAction = value; } }
	}
	#endregion
	#region RefreshAction
	[Flags]
	public enum RefreshAction {
		AllDocument = 1,
		Zoom = 2,
		Transforms = 4,
	}
	#endregion
}
