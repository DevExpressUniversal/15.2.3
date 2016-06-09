#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.Spreadsheet.Functions;
using System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel.Design;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.Spreadsheet {
	public partial class Workbook : IWorkbook, IDisposable {
		NativeWorkbook workbook;
		bool isDisposed;
		#region Properties
		NativeWorkbook InnerWorkbook {
			get {
				CheckValid();
				if (workbook == null) {
					workbook = new NativeWorkbook();
					workbook.History.IsEnabled = false;
				}
				return workbook;
			}
		}
		IPrintable PrintableImplementation { get { return InnerWorkbook; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookCurrentAuthor")]
#endif
		public string CurrentAuthor { get { return InnerWorkbook.CurrentAuthor; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookDefinedNames")]
#endif
		public DefinedNameCollection DefinedNames { get { return InnerWorkbook.DefinedNames; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookDocumentSettings")]
#endif
		public DocumentSettings DocumentSettings { get { return InnerWorkbook.DocumentSettings; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookOptions")]
#endif
		public DocumentOptions Options { get { return InnerWorkbook.Options; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookStyles")]
#endif
		public StyleCollection Styles { get { return InnerWorkbook.Styles; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookTableStyles")]
#endif
		public TableStyleCollection TableStyles { get { return InnerWorkbook.TableStyles; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookExternalWorkbooks")]
#endif
		public ExternalWorkbookCollection ExternalWorkbooks { get { return InnerWorkbook.ExternalWorkbooks; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookHistory")]
#endif
		public SpreadsheetHistory History { get { return InnerWorkbook.History; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookGlobalCustomFunctions")]
#endif
		public Functions.CustomFunctionCollection GlobalCustomFunctions { get { return InnerWorkbook.GlobalCustomFunctions; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookCustomFunctions")]
#endif
		public Functions.CustomFunctionCollection CustomFunctions { get { return InnerWorkbook.CustomFunctions; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookFunctions")]
#endif
		public WorkbookFunctions Functions { get { return InnerWorkbook.Functions; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookUnit")]
#endif
		public DocumentUnit Unit { get { return InnerWorkbook.Unit; } set { InnerWorkbook.Unit = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookWorksheets")]
#endif
		public WorksheetCollection Worksheets { get { return InnerWorkbook.Worksheets; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookIsUpdateLocked")]
#endif
		public bool IsUpdateLocked { get { return InnerWorkbook.IsUpdateLocked; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookIsDisposed")]
#endif
		public bool IsDisposed { get { return isDisposed; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookModel")]
#endif
		public DocumentModelAccessor Model { get { return InnerWorkbook.Model; } }
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return InnerWorkbook.BatchUpdateHelper; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookHasMacros")]
#endif
		public bool HasMacros { get { return InnerWorkbook.HasMacros; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookRange")]
#endif
		public IRangeProvider Range { get { return InnerWorkbook.Range; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookModified")]
#endif
		public bool Modified { get { return InnerWorkbook.Modified; } set { InnerWorkbook.Modified = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookClipboard")]
#endif
		public IClipboardManager Clipboard { get { return InnerWorkbook.Clipboard; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookTag")]
#endif
		public object Tag { get { return InnerWorkbook.Tag; } set { InnerWorkbook.Tag = value; } }
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookDocumentProperties")]
#endif
		public DocumentProperties DocumentProperties { get { return InnerWorkbook.DocumentProperties; } }
		public PivotCacheCollection PivotCaches {get { return InnerWorkbook.PivotCaches; } }
		#endregion
		void CheckValid() {
			if (isDisposed)
				throw new ObjectDisposedException("InnerWorkbook");
		}
		public void BeginUpdate() {
			InnerWorkbook.BeginUpdate();
		}
		public void EndUpdate() {
			InnerWorkbook.EndUpdate();
		}
		public void CancelUpdate() {
			InnerWorkbook.EndUpdate();
		}
#if !SL && !DXPORTABLE
		public void ExportToPdf(string fileName) {
			InnerWorkbook.ExportToPdf(fileName);
		}
		public void ExportToPdf(Stream stream) {
			InnerWorkbook.ExportToPdf(stream);
		}
		public void ExportToPdf(string fileName, PdfExportOptions options) {
			InnerWorkbook.ExportToPdf(fileName, options);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			InnerWorkbook.ExportToPdf(stream, options);
		}
#endif
		public void CreateNewDocument() {
			InnerWorkbook.CreateNewDocument();
		}
		public bool LoadDocument(Stream stream, DocumentFormat format) {
			return InnerWorkbook.LoadDocument(stream, format);
		}
		public bool LoadDocument(byte[] buffer, DocumentFormat format) {
			return InnerWorkbook.LoadDocument(buffer, format);
		}
		public bool LoadDocument(string fileName) {
			return InnerWorkbook.LoadDocument(fileName);
		}
		public bool LoadDocument(string fileName, DocumentFormat format) {
			return InnerWorkbook.LoadDocument(fileName, format);
		}
		public void SaveDocument(Stream stream, DocumentFormat format) {
			InnerWorkbook.SaveDocument(stream, format);
		}
		public byte[] SaveDocument(DocumentFormat format) {
			return InnerWorkbook.SaveDocument(format);
		}
		public void SaveDocument(string fileName, DocumentFormat format) {
			InnerWorkbook.SaveDocument(fileName, format);
		}
		public void SaveDocument(string fileName) {
			InnerWorkbook.SaveDocument(fileName);
		}
		public void Calculate() {
			InnerWorkbook.Calculate();
		}
		public void Calculate(Worksheet sheet) {
			InnerWorkbook.Calculate(sheet);
		}
		public void Calculate(Range range) {
			InnerWorkbook.Calculate(range);
		}
		public void CalculateFull() {
			InnerWorkbook.CalculateFull();
		}
		public void CalculateFullRebuild() {
			InnerWorkbook.CalculateFullRebuild();
		}
		public ParameterValue Evaluate(string formula) {
			return InnerWorkbook.Evaluate(formula);
		}
		public ParameterValue Evaluate(string formula, FormulaEvaluationContext context) {
			return InnerWorkbook.Evaluate(formula, context);
		}
		#region IServiceContainer
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			InnerWorkbook.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			InnerWorkbook.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			InnerWorkbook.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			InnerWorkbook.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			InnerWorkbook.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			InnerWorkbook.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return InnerWorkbook.GetService(serviceType);
		}
		#endregion
		public T GetService<T>() where T : class {
			return InnerWorkbook.GetService<T>();
		}
		public T ReplaceService<T>(T newService) where T : class {
			return InnerWorkbook.ReplaceService<T>(newService);
		}
		#region IPrintable
		bool IPrintable.CreatesIntersectedBricks { get { return PrintableImplementation.CreatesIntersectedBricks; } }
		UserControl IPrintable.PropertyEditorControl { get { return PrintableImplementation.PropertyEditorControl; } }
		void IPrintable.AcceptChanges() {
			PrintableImplementation.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			PrintableImplementation.RejectChanges();
		}
		bool IPrintable.HasPropertyEditor() {
			return PrintableImplementation.HasPropertyEditor();
		}
		void IPrintable.ShowHelp() {
			PrintableImplementation.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return PrintableImplementation.SupportsHelp();
		}
		#endregion
		#region IBasePrintable
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			PrintableImplementation.Initialize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			PrintableImplementation.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			PrintableImplementation.Finalize(ps, link);
		}
		#endregion
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				if (workbook != null) {
					workbook.Dispose();
					workbook = null;
				}
			}
			this.isDisposed = true;
		}
		#region Events
		#region ActiveSheetChanging
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookActiveSheetChanging")]
#endif
		public event ActiveSheetChangingEventHandler ActiveSheetChanging {
			add { InnerWorkbook.ActiveSheetChanging += value; }
			remove { InnerWorkbook.ActiveSheetChanging -= value; }
		}
		#endregion
		#region ActiveSheetChanged
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookActiveSheetChanged")]
#endif
		public event ActiveSheetChangedEventHandler ActiveSheetChanged {
			add { InnerWorkbook.ActiveSheetChanged += value; }
			remove { InnerWorkbook.ActiveSheetChanged -= value; }
		}
		#endregion
		#region SheetRenaming
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSheetRenaming")]
#endif
		public event SheetRenamingEventHandler SheetRenaming {
			add { InnerWorkbook.SheetRenaming += value; }
			remove { InnerWorkbook.SheetRenaming -= value; }
		}
		#endregion
		#region SheetRenamed
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSheetRenamed")]
#endif
		public event SheetRenamedEventHandler SheetRenamed {
			add { InnerWorkbook.SheetRenamed += value; }
			remove { InnerWorkbook.SheetRenamed -= value; }
		}
		#endregion
		#region SheetInserted
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSheetInserted")]
#endif
		public event SheetInsertedEventHandler SheetInserted {
			add { InnerWorkbook.SheetInserted += value; }
			remove { InnerWorkbook.SheetInserted -= value; }
		}
		#endregion
		#region SheetRemoved
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSheetRemoved")]
#endif
		public event SheetRemovedEventHandler SheetRemoved {
			add { InnerWorkbook.SheetRemoved += value; }
			remove { InnerWorkbook.SheetRemoved -= value; }
		}
		#endregion
		#region RowsInserted
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookRowsInserted")]
#endif
		public event RowsInsertedEventHandler RowsInserted {
			add { InnerWorkbook.RowsInserted += value; }
			remove { InnerWorkbook.RowsInserted -= value; }
		}
		#endregion
		#region RowsRemoved
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookRowsRemoved")]
#endif
		public event RowsRemovedEventHandler RowsRemoved {
			add { InnerWorkbook.RowsRemoved += value; }
			remove { InnerWorkbook.RowsRemoved -= value; }
		}
		#endregion
		#region ColumnsInserted
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookColumnsInserted")]
#endif
		public event ColumnsInsertedEventHandler ColumnsInserted {
			add { InnerWorkbook.ColumnsInserted += value; }
			remove { InnerWorkbook.ColumnsInserted -= value; }
		}
		#endregion
		#region ColumnsRemoved
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookColumnsRemoved")]
#endif
		public event ColumnsRemovedEventHandler ColumnsRemoved {
			add { InnerWorkbook.ColumnsRemoved += value; }
			remove { InnerWorkbook.ColumnsRemoved -= value; }
		}
		#endregion
		#region SelectionChanged
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSelectionChanged")]
#endif
		public event EventHandler SelectionChanged {
			add { InnerWorkbook.SelectionChanged += value; }
			remove { InnerWorkbook.SelectionChanged -= value; }
		}
		#endregion
		#region ModifiedChanged
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookModifiedChanged")]
#endif
		public event EventHandler ModifiedChanged {
			add { InnerWorkbook.ModifiedChanged += value; }
			remove { InnerWorkbook.ModifiedChanged -= value; }
		}
		#endregion
		#region DocumentLoaded
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookDocumentLoaded")]
#endif
		public event EventHandler DocumentLoaded {
			add { InnerWorkbook.DocumentLoaded += value; }
			remove { InnerWorkbook.DocumentLoaded -= value; }
		}
		#endregion
		#region EmptyDocumentCreated
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookEmptyDocumentCreated")]
#endif
		public event EventHandler EmptyDocumentCreated {
			add { InnerWorkbook.EmptyDocumentCreated += value; }
			remove { InnerWorkbook.EmptyDocumentCreated -= value; }
		}
		#endregion
		#region BeforeImport
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookBeforeImport")]
#endif
		public event BeforeImportEventHandler BeforeImport {
			add { InnerWorkbook.BeforeImport += value; }
			remove { InnerWorkbook.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookBeforeExport")]
#endif
		public event BeforeExportEventHandler BeforeExport {
			add { InnerWorkbook.BeforeExport += value; }
			remove { InnerWorkbook.BeforeExport -= value; }
		}
		#endregion
		#region InitializeDocument
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookInitializeDocument")]
#endif
		public event EventHandler InitializeDocument {
			add { InnerWorkbook.InitializeDocument += value; }
			remove { InnerWorkbook.InitializeDocument -= value; }
		}
		#endregion
		#region UnitChanging
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookUnitChanging")]
#endif
		public event EventHandler UnitChanging {
			add { InnerWorkbook.UnitChanging += value; }
			remove { InnerWorkbook.UnitChanging -= value; }
		}
		#endregion
		#region UnitChanged
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookUnitChanged")]
#endif
		public event EventHandler UnitChanged {
			add { InnerWorkbook.UnitChanged += value; }
			remove { InnerWorkbook.UnitChanged -= value; }
		}
		#endregion
		#region InvalidFormatException
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookInvalidFormatException")]
#endif
		public event InvalidFormatExceptionEventHandler InvalidFormatException {
			add { InnerWorkbook.InvalidFormatException += value; }
			remove { InnerWorkbook.InvalidFormatException -= value; }
		}
		#endregion
		#region PanesFrozen
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookPanesFrozen")]
#endif
		public event PanesFrozenEventHandler PanesFrozen {
			add { InnerWorkbook.PanesFrozen += value; }
			remove { InnerWorkbook.PanesFrozen -= value; }
		}
		#endregion
		#region PanesUnfrozen
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookPanesUnfrozen")]
#endif
		public event PanesUnfrozenEventHandler PanesUnfrozen {
			add { InnerWorkbook.PanesUnfrozen += value; }
			remove { InnerWorkbook.PanesUnfrozen -= value; }
		}
		#endregion
		#region BeforePrintSheet
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookBeforePrintSheet")]
#endif
		public event BeforePrintSheetEventHandler BeforePrintSheet {
			add { if (InnerWorkbook != null) InnerWorkbook.BeforePrintSheet += value; }
			remove { if (InnerWorkbook != null) InnerWorkbook.BeforePrintSheet -= value; }
		}
		#endregion
		#region ScrollPositionChanged
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookScrollPositionChanged")]
#endif
		public event ScrollPositionChangedEventHandler ScrollPositionChanged {
			add { InnerWorkbook.ScrollPositionChanged += value; }
			remove { InnerWorkbook.ScrollPositionChanged -= value; }
		}
		#endregion
		#region RangeCopying
		public event RangeCopyingEventHandler RangeCopying {
			add { InnerWorkbook.RangeCopying += value; }
			remove { InnerWorkbook.RangeCopying -= value; }
		}
		#endregion
		#region ShapesCopying
		public event ShapesCopyingEventHandler ShapesCopying {
			add { InnerWorkbook.ShapesCopying += value; }
			remove { InnerWorkbook.ShapesCopying -= value; }
		}
		#endregion
		#region RangeCopied
		public event RangeCopiedEventHandler RangeCopied {
			add { InnerWorkbook.RangeCopied += value; }
			remove { InnerWorkbook.RangeCopied -= value; }
		}
		#endregion
		#region CopiedRangePasting
		public event CopiedRangePastingEventHandler CopiedRangePasting {
			add { InnerWorkbook.CopiedRangePasting += value; }
			remove { InnerWorkbook.CopiedRangePasting -= value; }
		}
		#endregion
		#region CopiedRangePasted
		public event CopiedRangePastedEventHandler CopiedRangePasted {
			add { InnerWorkbook.CopiedRangePasted += value; }
			remove { InnerWorkbook.CopiedRangePasted -= value; }
		}
		#endregion
		#region ClipboardDataPasting
		public event ClipboardDataPastingEventHandler ClipboardDataPasting {
			add { InnerWorkbook.ClipboardDataPasting += value; }
			remove { InnerWorkbook.ClipboardDataPasting -= value; }
		}
		#endregion
		#region ClipboardDataObtained
		public event ClipboardDataObtainedEventHandler ClipboardDataObtained {
			add { InnerWorkbook.ClipboardDataObtained += value; }
			remove { InnerWorkbook.ClipboardDataObtained -= value; }
		}
		#endregion
		#region ClipboardDataPasted
		public event EventHandler ClipboardDataPasted {
			add { InnerWorkbook.ClipboardDataPasted += value; }
			remove { InnerWorkbook.ClipboardDataPasted -= value; }
		} 
		#endregion
		#endregion
		#region ExternalWorkbook Members
		System.Collections.Generic.IEnumerable<ExternalDefinedName> ExternalWorkbook.DefinedNames {
			get { return ((ExternalWorkbook)InnerWorkbook).DefinedNames; }
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookPath")]
#endif
		public string Path {
			get { return InnerWorkbook.Path; }
		}
		System.Collections.Generic.IEnumerable<ExternalWorksheet> ExternalWorkbook.Worksheets {
			get { return ((ExternalWorkbook)InnerWorkbook).Worksheets; }
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookContentChanged")]
#endif
		public event EventHandler ContentChanged {
			add { InnerWorkbook.ContentChanged += value; }
			remove { InnerWorkbook.ContentChanged -= value; }
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookSchemaChanged")]
#endif
		public event EventHandler SchemaChanged {
			add { InnerWorkbook.SchemaChanged += value; }
			remove { InnerWorkbook.SchemaChanged -= value; }
		}
		#endregion
		public System.Collections.Generic.IList<IWorkbook> GenerateMailMergeDocuments() {
			return InnerWorkbook.GenerateMailMergeDocuments();
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookMailMergeDataMember")]
#endif
		public string MailMergeDataMember {
			get {
				return InnerWorkbook.MailMergeDataMember;
			}
			set {
				InnerWorkbook.MailMergeDataMember = value;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookMailMergeDataSource")]
#endif
		public object MailMergeDataSource {
			get {
				return InnerWorkbook.MailMergeDataSource;
			}
			set {
				InnerWorkbook.MailMergeDataSource = value;
			}
		}
		public ParametersCollection MailMergeParameters { get { return InnerWorkbook.MailMergeParameters; } }
		public System.Collections.Generic.IEnumerable<Cell> Search(string text, SearchOptions options) {
			return InnerWorkbook.Search(text, options);
		}
		public System.Collections.Generic.IEnumerable<Cell> Search(string text) {
			return InnerWorkbook.Search(text);
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookFormulaEngine")]
#endif
		public Formulas.FormulaEngine FormulaEngine {
			get { return InnerWorkbook.FormulaEngine; }
		}
		public void ExportToHtml(string fileName, Worksheet sheet) {
			InnerWorkbook.ExportToHtml(fileName, sheet);
		}
		public void ExportToHtml(Stream stream, Worksheet sheet) {
			InnerWorkbook.ExportToHtml(stream, sheet);
		}
		public void ExportToHtml(string fileName, int sheetIndex) {
			InnerWorkbook.ExportToHtml(fileName, sheetIndex);
		}
		public void ExportToHtml(Stream stream, int sheetIndex) {
			InnerWorkbook.ExportToHtml(stream, sheetIndex);
		}
		public void ExportToHtml(string fileName, Range range) {
			InnerWorkbook.ExportToHtml(fileName, range);
		}
		public void ExportToHtml(Stream stream, Range range) {
			InnerWorkbook.ExportToHtml(stream, range);
		}
		public void ExportToHtml(string fileName, HtmlDocumentExporterOptions options) {
			InnerWorkbook.ExportToHtml(fileName, options);
		}
		public void ExportToHtml(Stream stream, HtmlDocumentExporterOptions options) {
			InnerWorkbook.ExportToHtml(stream, options);
		}
#if !SL
	[DevExpressDocsLocalizedDescription("WorkbookIsProtected")]
#endif
		public bool IsProtected { get { return InnerWorkbook.IsProtected; } }
		public void Protect(string password, bool lockStructure, bool lockWindows) {
			InnerWorkbook.Protect(password, lockStructure, lockWindows);
		}
		public bool Unprotect(string password) {
			return InnerWorkbook.Unprotect(password);
		}
	}
}
