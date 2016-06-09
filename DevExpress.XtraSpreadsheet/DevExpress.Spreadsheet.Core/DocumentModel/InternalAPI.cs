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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Csv;
using DevExpress.XtraSpreadsheet.Export.Csv;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Export.Html;
using DevExpress.XtraSpreadsheet.Export.Xlsm;
using DevExpress.XtraSpreadsheet.Import.Xlsm;
using DevExpress.XtraSpreadsheet.Internal;
#if OPENDOCUMENT
using DevExpress.XtraSpreadsheet.Import.OpenDocument;
using DevExpress.XtraSpreadsheet.Export.OpenDocument;
#endif
namespace DevExpress.XtraSpreadsheet.API.Internal {
	#region InternalAPI
	public class InternalAPI {
		#region Fields
		readonly DocumentModel workbook;
		readonly Dictionary<DocumentUnit, UnitConverter> unitConverters;
		#endregion
		public InternalAPI(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workBook");
			this.workbook = workbook;
			this.unitConverters = CreateUnitConverters();
		}
		public DocumentModel DocumentModel { get { return workbook; } }
		public Dictionary<DocumentUnit, UnitConverter> UnitConverters { get { return unitConverters; } }
		protected void ApplyDefaultOptions(DocumentImporterOptions options) {
			DocumentImporterOptions defaultOptions = DocumentModel.DocumentImportOptions.GetOptions(options.Format);
			if(defaultOptions != null)
				options.CopyFrom(defaultOptions);
		}
		protected void ApplyDefaultOptions(DocumentExporterOptions options) {
			DocumentExporterOptions defaultOptions = DocumentModel.DocumentExportOptions.GetOptions(options.Format);
			if(defaultOptions != null)
				options.CopyFrom(defaultOptions);
		}
		void ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat format, DevExpress.Office.Import.IImporterOptions options) {
			DocumentModel.RaiseBeforeImport(format, options);
		}
		void ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat format, DevExpress.Office.Export.IExporterOptions options) {
			DocumentModel.DocumentCoreProperties.RegisterDocumentModification();
			DocumentModel.DocumentApplicationProperties.CheckAppVersion();
			DocumentModel.RaiseBeforeExport(format, options);
		}
		#region OpenXml
		public virtual void LoadDocumentOpenXmlContent(Stream stream, OpenXmlDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.OpenXml, options);
			try {
				OpenXmlImporter importer = new OpenXmlImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch(Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentOpenXmlContent(Stream stream, OpenXmlDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.OpenXml, options);
			OpenXmlExporter exporter = new OpenXmlExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentOpenXmlContent(byte[] content) {
			OpenXmlDocumentImporterOptions options = new OpenXmlDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.OpenXml, options);
			SetDocumentOpenXmlContent(content, options);
		}
		public void SetDocumentOpenXmlContent(byte[] content, OpenXmlDocumentImporterOptions options) {
			using(MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentOpenXmlContent(stream, options);
			}
		}
		#endregion
#if OPENDOCUMENT
		#region OpenDocument
		public virtual void LoadDocumentOpenDocumentContent(Stream stream, OpenDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.OpenDocument, options);
			try {
				OpenDocumentWorkbookImporter importer = new OpenDocumentWorkbookImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentOpenDocumentContent(Stream stream, OpenDocumentDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.OpenDocument, options);
			OpenDocumentExporter exporter = new OpenDocumentExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentOpenDocumentContent(byte[] content) {
			OpenDocumentImporterOptions options = new OpenDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.OpenDocument, options);
			SetDocumentOpenDocumentContent(content, options);
		}
		public void SetDocumentOpenDocumentContent(byte[] content, OpenDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentOpenDocumentContent(stream, options);
			}
		}
		#endregion
#endif
		#region Xls
		public virtual void LoadDocumentXlsContent(Stream stream, XlsDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xls, options);
#if DISCOVER
			XlsImporter importer = new XlsImporter(DocumentModel, options);
			importer.Import(stream);
#else
			try {
				XlsImporter importer = new XlsImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch(Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
#endif
		}
		public virtual void SaveDocumentXlsContent(Stream stream, XlsDocumentExporterOptions options) {
			options.ClipboardMode = false;
			SaveDocumentXlsContent(stream, options, null);
		}
		public void SaveBiff8ContentForClipboard(Stream stream, XlsDocumentExporterOptions options, CellRangeInfo rangeToExport) {
			options.ClipboardMode = true;
			SaveDocumentXlsContent(stream, options, rangeToExport);
		}
		void SaveDocumentXlsContent(Stream stream, XlsDocumentExporterOptions options, CellRangeInfo rangeToExport) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Xls, options);
			XlsExporter exporter = new XlsExporter(DocumentModel, options);
			exporter.OleObjectRange = rangeToExport;
			exporter.Export(stream);
		}
		public void SetDocumentXlsContent(byte[] content) {
			XlsDocumentImporterOptions options = new XlsDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xls, options);
			SetDocumentXlsContent(content, options);
		}
		public void SetDocumentXlsContent(byte[] content, XlsDocumentImporterOptions options) {
			using(MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentXlsContent(stream, options);
			}
		}
		#endregion
		#region Xlt
		public virtual void LoadDocumentXltContent(Stream stream, XltDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xlt, options);
			try {
				XlsImporter importer = new XlsImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentXltContent(Stream stream, XltDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Xlt, options);
			XlsExporter exporter = new XlsExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentXltContent(byte[] content) {
			XltDocumentImporterOptions options = new XltDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xlt, options);
			SetDocumentXltContent(content, options);
		}
		public void SetDocumentXltContent(byte[] content, XltDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentXlsContent(stream, options);
			}
		}
		#endregion
		#region Csv
		public virtual void LoadDocumentCsvContent(Stream stream, TextDocumentImporterOptionsBase options) {
			ProcessBeforeImport(options.Format, options);
			try {
				CsvImporter importer = new CsvImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch(Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentCsvContent(Stream stream, TextDocumentExporterOptionsBase options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Csv, options);
			CsvExporter exporter = new CsvExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public string GetDocumentCsvContent(TextDocumentExporterOptionsBase options) {
			CsvExporter exporter = PrepareCsvExport(options);
			return exporter.ExportAsString();
		}
		CsvExporter PrepareCsvExport(TextDocumentExporterOptionsBase options) {
			if(options == null) {
				options = new CsvDocumentExporterOptions();
			}
			return new CsvExporter(DocumentModel, options);
		}
		#endregion
		#region Html
		public virtual void SaveDocumentHtmlContent(Stream stream, HtmlDocumentExporterOptions options) {
			DocumentModel.RaiseBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Html, options);
			HtmlExporter exporter = PrepareHtmlExport(options);
			StreamWriter writer = new StreamWriter(stream, options.Encoding);
			exporter.Export(writer);
		}
		public virtual void SaveDocumentHtmlContent(string fileName, HtmlDocumentExporterOptions options) {
			if (String.IsNullOrEmpty(options.TargetUri))
				options.TargetUri = fileName;
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				SaveDocumentHtmlContent(stream, options);
			}
		}
		public virtual void SaveDocumentHtmlContent(Stream stream, int sheetIndex, CellRange range) {
			SaveDocumentHtmlContent(stream, sheetIndex, range, String.Empty);
		}
		public virtual void SaveDocumentHtmlContent(Stream stream, int sheetIndex, CellRange range, string targetUri) {
			HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
			options.TargetUri = targetUri;
			options.SheetIndex = sheetIndex;
			if (range != null)
				options.Range = range.ToString(DocumentModel.DataContext);
			SaveDocumentHtmlContent(stream, options);
		}
		public virtual void SaveDocumentHtmlContent(string fileName, int sheetIndex, CellRange range) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				SaveDocumentHtmlContent(stream, sheetIndex, range, fileName);
			}
		}
		public string GetDocumentHtmlContent(IUriProvider provider) {
			return GetDocumentHtmlContent(provider, null);
		}
		public string GetDocumentHtmlContent(IUriProvider provider, HtmlDocumentExporterOptions options) {
			if (options == null) {
				options = new HtmlDocumentExporterOptions();
				ApplyDefaultOptions(options);
				options.CssPropertiesExportType = CssPropertiesExportType.Style;
				DocumentModel.RaiseBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Html, options);
			}
			if (provider == null)
				provider = new DataStringUriProvider();
			IUriProviderService service = DocumentModel.GetService<IUriProviderService>();
			if (service != null)
				service.RegisterProvider(provider);
			try {
				return GetDocumentHtmlContent(options);
			}
			finally {
				if (service != null)
					service.UnregisterProvider(provider);
			}
		}
		protected internal string GetDocumentHtmlContent(HtmlDocumentExporterOptions options) {
			HtmlExporter exporter = PrepareHtmlExport(options);
			return exporter.Export();
		}
		protected internal string GetDocumentCF_HTMLContent(HtmlDocumentExporterOptions options) {
			options.ActualEncoding = Encoding.UTF8;
			options.CssPropertiesExportType = CssPropertiesExportType.Inline;
			options.Encoding = Encoding.UTF8;
			options.ExportRootTag = ExportRootTag.Html;
			options.UriExportType = UriExportType.Absolute;
			options.AddClipboardHtmlFragmentTags = true;
			HtmlExporter exporter = PrepareHtmlExport(options);
			string htmlContent = exporter.Export();
			HtmlToClipboardHtmlConverter converter = new HtmlToClipboardHtmlConverter();
			string cf_html = converter.Convert(htmlContent);
			return cf_html;
		}
		protected internal HtmlExporter PrepareHtmlExport(HtmlDocumentExporterOptions options) {
			return new HtmlExporter(PrepareModelForExport(DevExpress.Spreadsheet.DocumentFormat.Html), options);
		}
		#endregion
		#region Xlsm
		public virtual void LoadDocumentXlsmContent(Stream stream, XlsmDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xlsm, options);
			try {
				XlsmImporter importer = new XlsmImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch(Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentXlsmContent(Stream stream, XlsmDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Xlsm, options);
			XlsmExporter exporter = new XlsmExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentXlsmContent(byte[] content) {
			XlsmDocumentImporterOptions options = new XlsmDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xlsm, options);
			SetDocumentXlsmContent(content, options);
		}
		public void SetDocumentXlsmContent(byte[] content, XlsmDocumentImporterOptions options) {
			using(MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentXlsmContent(stream, options);
			}
		}
		#endregion
		#region Xltx
		public virtual void LoadDocumentXltxContent(Stream stream, XltxDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xltx, options);
			try {
				OpenXmlImporter importer = new OpenXmlImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentXltxContent(Stream stream, XltxDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Xltx, options);
			XltxExporter exporter = new XltxExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentXltxContent(byte[] content) {
			XltxDocumentImporterOptions options = new XltxDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xltx, options);
			SetDocumentXltxContent(content, options);
		}
		public void SetDocumentXltxContent(byte[] content, XltxDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentXltxContent(stream, options);
			}
		}
		#endregion
		#region Xltm
		public virtual void LoadDocumentXltmContent(Stream stream, XltmDocumentImporterOptions options) {
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xltm, options);
			try {
				XlsmImporter importer = new XlsmImporter(DocumentModel, options);
				importer.Import(stream);
			}
			catch (Exception e) {
				DocumentModel.RaiseInvalidFormatException(e);
			}
		}
		public virtual void SaveDocumentXltmContent(Stream stream, XltmDocumentExporterOptions options) {
			ProcessBeforeExport(DevExpress.Spreadsheet.DocumentFormat.Xltm, options);
			XltmExporter exporter = new XltmExporter(DocumentModel, options);
			exporter.Export(stream);
		}
		public void SetDocumentXltmContent(byte[] content) {
			XltmDocumentImporterOptions options = new XltmDocumentImporterOptions();
			ApplyDefaultOptions(options);
			ProcessBeforeImport(DevExpress.Spreadsheet.DocumentFormat.Xltm, options);
			SetDocumentXltmContent(content, options);
		}
		public void SetDocumentXltmContent(byte[] content, XltmDocumentImporterOptions options) {
			using (MemoryStream stream = new MemoryStream(content)) {
				LoadDocumentXltmContent(stream, options);
			}
		}
		#endregion
		DocumentModel PrepareModelForExport(DevExpress.Spreadsheet.DocumentFormat format) {
			return this.DocumentModel;
		}
		#region Events
		#region AfterSheetRemoved
		WorksheetCollectionChangedEventHandler onAfterSheetRemoved;
		public event WorksheetCollectionChangedEventHandler AfterSheetRemoved { add { onAfterSheetRemoved += value; } remove { onAfterSheetRemoved -= value; } }
		protected internal virtual void RaiseAfterSheetRemoved(Worksheet sheet, int sheetIndex) {
			if (onAfterSheetRemoved != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onAfterSheetRemoved(this, args);
			}
		}
		public void OnAfterSheetRemoved(Worksheet sheet, int sheetIndex) {
			RaiseAfterSheetRemoved(sheet, sheetIndex);
		}
		#endregion
		#region BeforeSheetRemoving
		WorksheetCollectionChangedEventHandler onBeforeSheetRemoving;
		public event WorksheetCollectionChangedEventHandler BeforeSheetRemoving { add { onBeforeSheetRemoving += value; } remove { onBeforeSheetRemoving -= value; } }
		protected internal virtual void RaiseBeforeSheetRemoving(Worksheet sheet, int sheetIndex) {
			if(onBeforeSheetRemoving != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onBeforeSheetRemoving(this, args);
			}
		}
		public void OnBeforeSheetRemoving(Worksheet sheet, int sheetIndex) {
			RaiseBeforeSheetRemoving(sheet, sheetIndex);
		}
		#endregion
		#region AfterSheetInserted
		WorksheetCollectionChangedEventHandler onAfterSheetInserted;
		public event WorksheetCollectionChangedEventHandler AfterSheetInserted { add { onAfterSheetInserted += value; } remove { onAfterSheetInserted -= value; } }
		protected internal virtual void RaiseAfterSheetInserted(Worksheet sheet, int sheetIndex) {
			if(onAfterSheetInserted != null) {
				WorksheetCollectionChangedEventArgs args = new WorksheetCollectionChangedEventArgs(sheet, sheetIndex);
				onAfterSheetInserted(this, args);
			}
		}
		public void OnAfterSheetInserted(Worksheet sheet, int index) {
			RaiseAfterSheetInserted(sheet, index);
		}
		#endregion
		#region AfterSheetCollectionCleared
		EventHandler onAfterSheetCollectionCleared;
		public event EventHandler AfterSheetCollectionCleared { add { onAfterSheetCollectionCleared += value; } remove { onAfterSheetCollectionCleared -= value; } }
		protected internal virtual void RaiseAfterSheetCollectionCleared() {
			if(onAfterSheetCollectionCleared != null) {
				EventArgs args = new EventArgs();
				onAfterSheetCollectionCleared(this, args);
			}
		}
		public void OnAfterSheetCollectionCleared() {
			RaiseAfterSheetCollectionCleared();
		}
		#endregion
		#region AfterWoksheetMoved
		WorksheetMovedEventHandler onAfterWoksheetMoved;
		public event WorksheetMovedEventHandler AfterWoksheetMoved { add { onAfterWoksheetMoved += value; } remove { onAfterWoksheetMoved -= value; } }
		protected internal virtual void RaiseAfterWoksheetMoved(Worksheet sheet, int oldIndex, int newIndex) {
			if(onAfterWoksheetMoved != null) {
				WorksheetMovedEventArgs args = new WorksheetMovedEventArgs(sheet, oldIndex, newIndex);
				onAfterWoksheetMoved(this, args);
			}
		}
		#endregion
		#region HyperlinkInserted
		HyperlinkCollectionChangedEventHandler onAfterHyperlinkInserted;
		internal event HyperlinkCollectionChangedEventHandler AfterHyperlinkInserted { add { onAfterHyperlinkInserted += value; } remove { onAfterHyperlinkInserted -= value; } }
		protected internal virtual void RaiseAfterHyperlinkInserted(Worksheet worksheet, int hyperlinkIndex) {
			if(onAfterHyperlinkInserted != null) {
				HyperlinkCollectionChangedEventArgs args = new HyperlinkCollectionChangedEventArgs(worksheet, hyperlinkIndex);
				onAfterHyperlinkInserted(this, args);
			}
		}
		#endregion
		#region BeforeHyperlinkRemoved
		HyperlinkCollectionChangedEventHandler onBeforeHyperlinkRemoved;
		internal event HyperlinkCollectionChangedEventHandler BeforeHyperlinkRemoved { add { onBeforeHyperlinkRemoved += value; } remove { onBeforeHyperlinkRemoved -= value; } }
		protected internal virtual void RaiseBeforeHyperlinkRemoved(Worksheet worksheet, int hyperlinkIndex) {
			if(onBeforeHyperlinkRemoved != null) {
				HyperlinkCollectionChangedEventArgs args = new HyperlinkCollectionChangedEventArgs(worksheet, hyperlinkIndex);
				onBeforeHyperlinkRemoved(this, args);
			}
		}
		public void OnBeforeHyperlinkRemoved(Worksheet worksheet, int hyperlinkIndex) {
			RaiseBeforeHyperlinkRemoved(worksheet, hyperlinkIndex);
		}
		public void OnAfterHyperlinkInserted(Worksheet worksheet, int hyperlinkIndex) {
			RaiseAfterHyperlinkInserted(worksheet, hyperlinkIndex);
		}
		#endregion
		#region BeforeRowRemoved
		BeforeRowRemoveEventHandler onBeforeRowRemoved;
		internal event BeforeRowRemoveEventHandler BeforeRowRemoved { add { onBeforeRowRemoved += value; } remove { onBeforeRowRemoved -= value; } }
		protected internal virtual void RaiseBeforeRowRemoved(Worksheet sheet, int deletedRowIndex, int deletedRowsCount) {
			if(onBeforeRowRemoved != null) {
				BeforeRowRemoveEventArgs args = new BeforeRowRemoveEventArgs(sheet, deletedRowIndex, deletedRowsCount);
				onBeforeRowRemoved(this, args);
			}
		}
		public void OnBeforeRowRemoved(Worksheet worksheet, int rowIndex, int deletedRowsCount) {
			RaiseBeforeRowRemoved(worksheet, rowIndex, deletedRowsCount);
		}
		#endregion
		#region BeforeRowsCleared
		BeforeRowsClearedEventHandler onBeforeRowsCleared;
		internal event BeforeRowsClearedEventHandler BeforeRowsCleared { add { onBeforeRowsCleared += value; } remove { onBeforeRowsCleared -= value; } }
		protected internal virtual void RaiseBeforeRowsCleared(Worksheet sheet) {
			if(onBeforeRowsCleared != null) {
				BeforeRowsClearedEventArgs args = new BeforeRowsClearedEventArgs(sheet);
				onBeforeRowsCleared(this, args);
			}
		}
		public void OnBeforeRowsCleared(Worksheet sheet) {
			RaiseBeforeRowsCleared(sheet);
		}
		#endregion
		#region ColumnRemoved
		ColumnRemovedEventHandler onColumnRemoved;
		internal event ColumnRemovedEventHandler ColumnRemoved { add { onColumnRemoved += value; } remove { onColumnRemoved -= value; } }
		protected internal virtual void RaiseColumnRemoved(Worksheet sheet, int deletedRowIndex) {
			if(onColumnRemoved != null) {
				ColumnRemovedEventArgs args = new ColumnRemovedEventArgs(sheet, deletedRowIndex);
				onColumnRemoved(this, args);
			}
		}
		public void OnColumnRemoved(Worksheet worksheet, int rowIndex) {
			RaiseColumnRemoved(worksheet, rowIndex);
		}
		#endregion
		#region ShiftCellsLeft
		ShiftCellsLeftEventHandler onShiftCellsLeft;
		internal event ShiftCellsLeftEventHandler ShiftCellsLeft { add { onShiftCellsLeft += value; } remove { onShiftCellsLeft -= value; } }
		protected internal virtual void RaiseShiftCellsLeft(Worksheet sheet, int firstColumnIndex, int firstRowIndex, int lastRowIndex) {
			if(onShiftCellsLeft != null) {
				ShiftCellsLeftEventArgs args = new ShiftCellsLeftEventArgs(sheet, firstColumnIndex, firstRowIndex, lastRowIndex);
				onShiftCellsLeft(this, args);
			}
		}
		public void OnShiftCellsLeft(Worksheet worksheet, int firstColumnIndex, int firstRowIndex, int lastRowIndex) {
			RaiseShiftCellsLeft(worksheet, firstColumnIndex, firstRowIndex, lastRowIndex);
		}
		#endregion
		#region ShiftCellsUp
		ShiftCellsUpEventHandler onShiftCellsUp;
		internal event ShiftCellsUpEventHandler ShiftCellsUp { add { onShiftCellsUp += value; } remove { onShiftCellsUp -= value; } }
		protected internal virtual void RaiseShiftCellsUp(Worksheet sheet, int firstColumnIndex, int lastColumnIndex, int firstRowIndex) {
			if(onShiftCellsUp != null) {
				ShiftCellsUpEventArgs args = new ShiftCellsUpEventArgs(sheet, firstColumnIndex, lastColumnIndex, firstRowIndex);
				onShiftCellsUp(this, args);
			}
		}
		public void OnShiftCellsUp(Worksheet worksheet, int firstColumnIndex, int lastColumnIndex, int firstRowIndex) {
			RaiseShiftCellsUp(worksheet, firstColumnIndex, lastColumnIndex, firstRowIndex);
		}
		#endregion
		#region HyperlinkAdd
		HyperlinkAddEventHandler onHyperlinkAdd;
		internal event HyperlinkAddEventHandler HyperlinkAdd { add { onHyperlinkAdd += value; } remove { onHyperlinkAdd -= value; } }
		protected internal virtual void RaiseHyperlinkAdd(Worksheet sheet, ModelHyperlink hyperlink) {
			if(onHyperlinkAdd != null) {
				HyperlinkAddEventArgs args = new HyperlinkAddEventArgs(sheet, hyperlink);
				onHyperlinkAdd(this, args);
			}
		}
		public void OnHyperlinkAdd(Worksheet worksheet, ModelHyperlink hyperlink) {
			RaiseHyperlinkAdd(worksheet, hyperlink);
		}
		#endregion
		#region HyperlinkRemoveAt
		HyperlinkRemoveAtEventHandler onHyperlinkRemoveAt;
		internal event HyperlinkRemoveAtEventHandler HyperlinkRemoveAt { add { onHyperlinkRemoveAt += value; } remove { onHyperlinkRemoveAt -= value; } }
		protected internal virtual void RaiseHyperlinkRemoveAt(Worksheet sheet, int deletedIndex) {
			if(onHyperlinkRemoveAt != null) {
				HyperlinkRemoveAtEventArgs args = new HyperlinkRemoveAtEventArgs(sheet, deletedIndex);
				onHyperlinkRemoveAt(this, args);
			}
		}
		public void OnHyperlinkRemoveAt(Worksheet worksheet, int rowIndex) {
			RaiseHyperlinkRemoveAt(worksheet, rowIndex);
		}
		#endregion
		#region HyperlinkCollectionClear
		HyperlinkCollectionClearEventHandler onHyperlinkCollectionClear;
		internal event HyperlinkCollectionClearEventHandler HyperlinkCollectionClear { add { onHyperlinkCollectionClear += value; } remove { onHyperlinkCollectionClear -= value; } }
		protected internal virtual void RaiseHyperlinkCollectionClear(Worksheet sheet) {
			if(onHyperlinkCollectionClear != null) {
				HyperlinkCollectionClearEventArgs args = new HyperlinkCollectionClearEventArgs(sheet);
				onHyperlinkCollectionClear(this, args);
			}
		}
		public void OnHyperlinkCollectionClear(Worksheet worksheet) {
			RaiseHyperlinkCollectionClear(worksheet);
		}
		#endregion
		#region AfterMergedCellsInserted
		MergedCellsCollectionChangedEventHandler onAfterMergedCellsInserted;
		internal event MergedCellsCollectionChangedEventHandler AfterMergedCellsInserted { add { onAfterMergedCellsInserted += value; } remove { onAfterMergedCellsInserted -= value; } }
		protected internal virtual void RaiseAfterMergedCellsInserted(Worksheet sheet, CellRange mergedCellRange) {
			if(onAfterMergedCellsInserted != null) {
				MergedCellsCollectionChangedEventArgs args = new MergedCellsCollectionChangedEventArgs(sheet, mergedCellRange);
				onAfterMergedCellsInserted(this, args);
			}
		}
		public void OnAfterMergedCellsInserted(Worksheet worksheet, CellRange mergedCellRange) {
			RaiseAfterMergedCellsInserted(worksheet, mergedCellRange);
		}
		#endregion
		#region BeforeMergedCellsRemoved
		MergedCellsCollectionChangedEventHandler onBeforeMergedCellsRemoved;
		internal event MergedCellsCollectionChangedEventHandler BeforeMergedCellsRemoved { add { onBeforeMergedCellsRemoved += value; } remove { onBeforeMergedCellsRemoved -= value; } }
		protected internal virtual void RaiseBeforeMergedCellsRemoved(Worksheet sheet, CellRange mergedRange) {
			if(onBeforeMergedCellsRemoved != null) {
				MergedCellsCollectionChangedEventArgs args = new MergedCellsCollectionChangedEventArgs(sheet, mergedRange);
				onBeforeMergedCellsRemoved(this, args);
			}
		}
		public void OnBeforeMergedCellsRemoved(Worksheet worksheet, CellRange mergedRange) {
			RaiseBeforeMergedCellsRemoved(worksheet, mergedRange);
		}
		#endregion
		#region BeforeMergedCellsCleared
		MergedCellsCollectionClearedEventHandler onBeforeMergedCellsCleared;
		internal event MergedCellsCollectionClearedEventHandler BeforeMergedCellsCleared { add { onBeforeMergedCellsCleared += value; } remove { onBeforeMergedCellsCleared -= value; } }
		protected internal virtual void RaiseBeforeMergedCellsCleared(Worksheet sheet) {
			if(onBeforeMergedCellsCleared != null) {
				MergedCellsCollectionClearedEventArgs args = new MergedCellsCollectionClearedEventArgs(sheet);
				onBeforeMergedCellsCleared(this, args);
			}
		}
		public void OnBeforeMergedCellsCleared(Worksheet worksheet) {
			RaiseBeforeMergedCellsCleared(worksheet);
		}
		#endregion
		#region ArrayFormulaAdd
		ArrayFormulaAddEventHandler onArrayFormulaAdd;
		internal event ArrayFormulaAddEventHandler ArrayFormulaAdd { add { onArrayFormulaAdd += value; } remove { onArrayFormulaAdd -= value; } }
		protected internal virtual void RaiseArrayFormulaAdd(Worksheet sheet, ArrayFormula arrayFormula) {
			if(onArrayFormulaAdd != null) {
				ArrayFormulaAddEventArgs args = new ArrayFormulaAddEventArgs(sheet, arrayFormula);
				onArrayFormulaAdd(this, args);
			}
		}
		public void OnArrayFormulaAdd(Worksheet worksheet, ArrayFormula arrayFormula) {
			RaiseArrayFormulaAdd(worksheet, arrayFormula);
		}
		#endregion
		#region ArrayFormulaRemoveAt
		ArrayFormulaRemoveAtEventHandler onArrayFormulaRemoveAt;
		internal event ArrayFormulaRemoveAtEventHandler ArrayFormulaRemoveAt { add { onArrayFormulaRemoveAt += value; } remove { onArrayFormulaRemoveAt -= value; } }
		protected internal virtual void RaiseArrayFormulaRemoveAt(Worksheet sheet, int index) {
			if(onArrayFormulaRemoveAt != null) {
				ArrayFormulaRemoveEventArgs args = new ArrayFormulaRemoveEventArgs(sheet, index);
				onArrayFormulaRemoveAt(this, args);
			}
		}
		public void OnArrayFormulaRemoveAt(Worksheet worksheet, int index) {
			RaiseArrayFormulaRemoveAt(worksheet, index);
		}
		#endregion
		#region ArrayFormulaCollectionClear
		ArrayFormulaCollectionClearEventHandler onArrayFormulaCollectionClear;
		internal event ArrayFormulaCollectionClearEventHandler ArrayFormulaCollectionClear { add { onArrayFormulaCollectionClear += value; } remove { onArrayFormulaCollectionClear -= value; } }
		protected internal virtual void RaiseArrayFormulaCollectionClear(Worksheet sheet) {
			if(onArrayFormulaCollectionClear != null) {
				ArrayFormulaCollectionClearEventArgs args = new ArrayFormulaCollectionClearEventArgs(sheet);
				onArrayFormulaCollectionClear(this, args);
			}
		}
		public void OnArrayFormulaCollectionClear(Worksheet worksheet) {
			RaiseArrayFormulaCollectionClear(worksheet);
		}
		#endregion
		#region TableAdd
		TableAddEventHandler onTableAdd;
		internal event TableAddEventHandler TableAdd { add { onTableAdd += value; } remove { onTableAdd -= value; } }
		protected internal virtual void RaiseTableAdd(Worksheet sheet, Table table) {
			if (onTableAdd != null) {
				TableAddEventArgs args = new TableAddEventArgs(sheet, table);
				onTableAdd(this, args);
			}
		}
		public void OnTableAdd(Worksheet worksheet, Table table) {
			RaiseTableAdd(worksheet, table);
		}
		#endregion
		#region TableRemoveAt
		TableRemoveAtEventHandler onTableRemoveAt;
		internal event TableRemoveAtEventHandler TableRemoveAt { add { onTableRemoveAt += value; } remove { onTableRemoveAt -= value; } }
		protected internal virtual void RaiseTableRemoveAt(Worksheet sheet, int index) {
			if (onTableRemoveAt != null) {
				TableRemoveAtEventArgs args = new TableRemoveAtEventArgs(sheet, index);
				onTableRemoveAt(this, args);
			}
		}
		public void OnTableRemoveAt(Worksheet worksheet, int index) {
			RaiseTableRemoveAt(worksheet, index);
		}
		#endregion
		#region TableCollectionClear
		TableCollectionClearEventHandler onTableCollectionClear;
		internal event TableCollectionClearEventHandler TableCollectionClear { add { onTableCollectionClear += value; } remove { onTableCollectionClear -= value; } }
		protected internal virtual void RaiseTableCollectionClear(Worksheet sheet) {
			if (onTableCollectionClear != null) {
				TableCollectionClearEventArgs args = new TableCollectionClearEventArgs(sheet);
				onTableCollectionClear(this, args);
			}
		}
		public void OnTableCollectionClear(Worksheet worksheet) {
			RaiseTableCollectionClear(worksheet);
		}
		#endregion
		#region TableColumnAdd
		TableColumnAddEventHandler onTableColumnAdd;
		internal event TableColumnAddEventHandler TableColumnAdd { add { onTableColumnAdd += value; } remove { onTableColumnAdd -= value; } }
		protected internal virtual void RaiseTableColumnAdd(TableColumn column, int index) {
			if (onTableColumnAdd != null) {
				TableColumnAddEventArgs args = new TableColumnAddEventArgs(column, index);
				onTableColumnAdd(this, args);
			}
		}
		public void OnTableColumnAdd(TableColumn column, int index) {
			RaiseTableColumnAdd(column, index);
		}
		#endregion
		#region TableColumnRemoveAt
		TableColumnRemoveAtEventHandler onTableColumnRemoveAt;
		internal event TableColumnRemoveAtEventHandler TableColumnRemoveAt { add { onTableColumnRemoveAt += value; } remove { onTableColumnRemoveAt -= value; } }
		protected internal virtual void RaiseTableColumnRemoveAt(Table table, int index) {
			if (onTableColumnRemoveAt != null) {
				TableColumnRemoveAtEventArgs args = new TableColumnRemoveAtEventArgs(table, index);
				onTableColumnRemoveAt(this, args);
			}
		}
		public void OnTableColumnRemoveAt(Table table, int index) {
			RaiseTableColumnRemoveAt(table, index);
		}
		#endregion
		#region DefinedNameWorksheetAdd
		DefinedNameWorksheetAddEventHandler onDefinedNameWorksheetAdd;
		internal event DefinedNameWorksheetAddEventHandler DefinedNameWorksheetAdd { add { onDefinedNameWorksheetAdd += value; } remove { onDefinedNameWorksheetAdd -= value; } }
		protected internal virtual void RaiseDefinedNameWorksheetAdd(Worksheet worksheet, DefinedName definedName) {
			if(onDefinedNameWorksheetAdd != null) {
				DefinedNameWorksheetAddEventArgs args = new DefinedNameWorksheetAddEventArgs(worksheet, definedName);
				onDefinedNameWorksheetAdd(this, args);
			}
		}
		public void OnDefinedNameWorksheetAdd(Worksheet worksheet, DefinedName definedName) {
			RaiseDefinedNameWorksheetAdd(worksheet, definedName);
		}
		#endregion
		#region DefinedNameWorksheetRemove
		DefinedNameWorksheetRemoveEventHandler onDefinedNameWorksheetRemove;
		internal event DefinedNameWorksheetRemoveEventHandler DefinedNameWorksheetRemove { add { onDefinedNameWorksheetRemove += value; } remove { onDefinedNameWorksheetRemove -= value; } }
		protected internal virtual void RaiseDefinedNameWorksheetRemove(Worksheet sheet, DefinedName definedName) {
			if(onDefinedNameWorksheetRemove != null) {
				DefinedNameWorksheetRemoveEventArgs args = new DefinedNameWorksheetRemoveEventArgs(sheet, definedName);
				onDefinedNameWorksheetRemove(this, args);
			}
		}
		public void OnDefinedNameWorksheetRemove(Worksheet worksheet, DefinedName definedName) {
			RaiseDefinedNameWorksheetRemove(worksheet, definedName);
		}
		#endregion
		#region DefinedNameWorksheetCollectionClear
		DefinedNameWorksheetCollectionClearEventHandler onDefinedNameWorksheetCollectionClear;
		internal event DefinedNameWorksheetCollectionClearEventHandler DefinedNameWorksheetCollectionClear { add { onDefinedNameWorksheetCollectionClear += value; } remove { onDefinedNameWorksheetCollectionClear -= value; } }
		protected internal virtual void RaiseDefinedNameWorksheetCollectionClear(Worksheet sheet) {
			if(onDefinedNameWorksheetCollectionClear != null) {
				DefinedNameWorksheetCollectionClearEventArgs args = new DefinedNameWorksheetCollectionClearEventArgs(sheet);
				onDefinedNameWorksheetCollectionClear(this, args);
			}
		}
		public void OnDefinedNameWorksheetCollectionClear(Worksheet worksheet) {
			RaiseDefinedNameWorksheetCollectionClear(worksheet);
		}
		#endregion
		#region DefinedNameWorkbookAdd
		DefinedNameWorkbookAddEventHandler onDefinedNameWorkbookAdd;
		internal event DefinedNameWorkbookAddEventHandler DefinedNameWorkbookAdd { 
			add { onDefinedNameWorkbookAdd += value; } 
			remove { onDefinedNameWorkbookAdd -= value; } 
		}
		protected internal virtual void RaiseDefinedNameWorkbookAdd(DocumentModel workbook, DefinedName definedName) {
			if(onDefinedNameWorkbookAdd != null) {
				DefinedNameWorkbookAddEventArgs args = new DefinedNameWorkbookAddEventArgs(workbook, definedName);
				onDefinedNameWorkbookAdd(this, args);
			}
		}
		public void OnDefinedNameWorkbookAdd(DocumentModel workbook, DefinedName definedName) {
			RaiseDefinedNameWorkbookAdd(workbook, definedName);
		}
		#endregion
		#region DefinedNameWorkbookRemove
		DefinedNameWorkbookRemoveEventHandler onDefinedNameWorkbookRemove;
		internal event DefinedNameWorkbookRemoveEventHandler DefinedNameWorkbookRemove { add { onDefinedNameWorkbookRemove += value; } remove { onDefinedNameWorkbookRemove -= value; } }
		protected internal virtual void RaiseDefinedNameWorkbookRemove(DocumentModel book, DefinedName definedName) {
			if(onDefinedNameWorkbookRemove != null) {
				DefinedNameWorkbookRemoveEventArgs args = new DefinedNameWorkbookRemoveEventArgs(book, definedName);
				onDefinedNameWorkbookRemove(this, args);
			}
		}
		public void OnDefinedNameWorkbookRemove(DocumentModel workbook, DefinedName definedName) {
			RaiseDefinedNameWorkbookRemove(workbook, definedName);
		}
		#endregion
		#region DefinedNameWorkbookCollectionClear
		DefinedNameWorkbookCollectionClearEventHandler onDefinedNameWorkbookCollectionClear;
		internal event DefinedNameWorkbookCollectionClearEventHandler DefinedNameWorkbookCollectionClear { add { onDefinedNameWorkbookCollectionClear += value; } remove { onDefinedNameWorkbookCollectionClear -= value; } }
		protected internal virtual void RaiseDefinedNameWorkbookCollectionClear(DocumentModel book) {
			if(onDefinedNameWorkbookCollectionClear != null) {
				DefinedNameWorkbookCollectionClearEventArgs args = new DefinedNameWorkbookCollectionClearEventArgs(book);
				onDefinedNameWorkbookCollectionClear(this, args);
			}
		}
		public void OnDefinedNameWorkbookCollectionClear(DocumentModel workbook) {
			RaiseDefinedNameWorkbookCollectionClear(workbook);
		}
		#endregion
		#region CellValueChanged
		CellValueChangedEventHandler onCellValueChanged;
		public event CellValueChangedEventHandler CellValueChanged { add { onCellValueChanged += value; } remove { onCellValueChanged -= value; } }
		protected internal virtual void RaiseCellValueChanged(CellContentSnapshot cellSnapshot) {
			if (onCellValueChanged != null) {
				SpreadsheetCellEventArgs args = new SpreadsheetCellEventArgs(cellSnapshot);
				onCellValueChanged(this, args);
			}
		}
		#endregion
		#region AfterDefinedNameRenamed
		AfterDefinedNameRenamedEventHandler onAfterDefinedNameRenamed;
		internal event AfterDefinedNameRenamedEventHandler AfterDefinedNameRenamed {
			add { onAfterDefinedNameRenamed += value; }
			remove { onAfterDefinedNameRenamed -= value; }
		}
		protected internal virtual void RaiseAfterDefinedNameRenamed(DocumentModel workbook, DefinedName name, string oldName, string newName) {
			if(onAfterDefinedNameRenamed != null) {
				AfterDefinedNameRenamedEventArgs args = new AfterDefinedNameRenamedEventArgs(workbook, name, oldName, newName);
				onAfterDefinedNameRenamed(this, args);
			}
		}
		public void OnAfterDefinedNameRenamed(DocumentModel workbook, DefinedName name, string oldName, string newName) {
			RaiseAfterDefinedNameRenamed(workbook, name, oldName, newName);
		}
		#endregion
		#region CustomFunctionEvaluate
		CustomFunctionEvaluateEventHandler onCustomFunctionEvaluation;
		internal event CustomFunctionEvaluateEventHandler CustomFunctionEvaluation {
			add { onCustomFunctionEvaluation += value; }
			remove { onCustomFunctionEvaluation -= value; }
		}
		protected internal virtual VariantValue RaiseCustomFunctionEvaluation(string name, IList<VariantValue> arguments) {
			if (onCustomFunctionEvaluation != null) {
				CustomFunctionEvaluateEventArgs args = new CustomFunctionEvaluateEventArgs(name, arguments);
				onCustomFunctionEvaluation(this, args);
				return args.Result;
			}
			return VariantValue.Empty;
		}
		#endregion
		#region ExternalLinkAdded event
		ExternalLinksCollectionChangedEventHandler onExternalLinkAdded;
		internal event ExternalLinksCollectionChangedEventHandler ExternalLinkAdded { add { onExternalLinkAdded += value; } remove { onExternalLinkAdded -= value; } }
		protected internal virtual void RaiseExternalLinkAdded(DevExpress.XtraSpreadsheet.Model.External.ExternalLink newExternalLink) {
			if (onExternalLinkAdded != null) {
				ExternalLinksCollectionChangedEventArgs args = new ExternalLinksCollectionChangedEventArgs(newExternalLink);
				onExternalLinkAdded(this, args);
			}
		}
		#endregion
		#region ExternalLinkRemoved event
		ExternalLinksCollectionChangedEventHandler onExternalLinkRemoved;
		internal event ExternalLinksCollectionChangedEventHandler ExternalLinkRemoved { add { onExternalLinkRemoved += value; } remove { onExternalLinkRemoved -= value; } }
		protected internal virtual void RaiseExternalLinkRemoved(DevExpress.XtraSpreadsheet.Model.External.ExternalLink newExternalLink) {
			if (onExternalLinkRemoved != null) {
				ExternalLinksCollectionChangedEventArgs args = new ExternalLinksCollectionChangedEventArgs(newExternalLink);
				onExternalLinkRemoved(this, args);
			}
		}
		#endregion
		#region ExternalLinksCollectionClear event
		EventHandler onExternalLinksCollectionClear;
		internal event EventHandler CollectionClear { add { onExternalLinksCollectionClear += value; } remove { onExternalLinksCollectionClear -= value; } }
		protected internal virtual void RaiseExternalLinksCollectionClear() {
			if (onExternalLinksCollectionClear != null) {
				EventArgs args = new EventArgs();
				onExternalLinksCollectionClear(this, args);
			}
		}
		#endregion
		#endregion
		Dictionary<DocumentUnit, UnitConverter> CreateUnitConverters() {
			UnitConvertersCreator creator = new UnitConvertersCreator();
			return creator.CreateUnitConverters(workbook.UnitConverter);
		}
		public void CreateNewDocument() {
			DocumentModel.BeginSetContent();
			try {
				workbook.DocumentSaveOptions.ResetCurrentFileName();
				workbook.DocumentSaveOptions.ResetCurrentFormat();
				workbook.AddInitialSheets(false);
				workbook.DocumentCoreProperties.RegisterDocumentCreation(false);
			}
			finally {
				DocumentModel.EndSetContent(DocumentModelChangeType.CreateEmptyDocument);
			}
		}
	}
	#endregion
}
