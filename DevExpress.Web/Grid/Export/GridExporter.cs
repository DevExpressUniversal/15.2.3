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
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Export;
using DevExpress.Web.Export;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Web {
	public enum GridViewExportedRowType { All, Selected }
	public abstract class ASPxGridExporterBase : Control, IPrintable {
		public const int DefaultMaxColumnWidth = 300;
		protected internal const int MinColumnWidth = 20;
		GridViewExporterHeaderFooter pageHeader, pageFooter;
		ASPxGridBase gridBase;
		public abstract string DefaultFileName { get; }
		public virtual bool Landscape {
			get { return (bool)GetObjectFromViewState("Landscape", false); }
			set { SetObjectToViewState("Landscape", false, value); }
		}
		public virtual string FileName {
			get { return (string)GetObjectFromViewState("FileName", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				SetObjectToViewState("FileName", string.Empty, value);
			}
		}
		public virtual int MaxColumnWidth {
			get { return (int)GetObjectFromViewState("MaxColumnWidth", DefaultMaxColumnWidth); }
			set {
				if(value <= MinColumnWidth)
					value = MinColumnWidth;
				SetObjectToViewState("MaxColumnWidth", DefaultMaxColumnWidth, value);
			}
		}
		public virtual bool PrintSelectCheckBox {
			get { return (bool)GetObjectFromViewState("PrintSelectCheckBox", false); }
			set { SetObjectToViewState("PrintSelectCheckBox", false, value); }
		}
		protected string GridBaseID {
			get { return (string)GetObjectFromViewState("GridBaseID", string.Empty); }
			set {
				if(value == null)
					value = string.Empty;
				SetObjectToViewState("GridBaseID", string.Empty, value);
			}
		}
		public virtual GridViewExporterHeaderFooter PageHeader {
			get {
				if(pageHeader == null)
					pageHeader = new GridViewExporterHeaderFooter();
				return pageHeader;
			}
		}
		public virtual GridViewExporterHeaderFooter PageFooter {
			get {
				if(pageFooter == null)
					pageFooter = new GridViewExporterHeaderFooter();
				return pageFooter;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public virtual string ReportHeader {
			get { return (string)GetObjectFromViewState("ReportHeader", string.Empty); }
			set { SetObjectToViewState("ReportHeader", string.Empty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public virtual string ReportFooter {
			get { return (string)GetObjectFromViewState("ReportFooter", string.Empty); }
			set { SetObjectToViewState("ReportFooter", string.Empty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public virtual PaperKind PaperKind {
			get { return (PaperKind)GetObjectFromViewState("PaperKind", PaperKind.Letter); }
			set { SetObjectToViewState("PaperKind", PaperKind.Letter, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public virtual string PaperName {
			get { return (string)GetObjectFromViewState("PaperName", ""); }
			set { SetObjectToViewState("PaperName", "", value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public virtual bool ExportSelectedRowsOnly {
			get { return (bool)GetObjectFromViewState("ExportSelectedRowsOnly", false); }
			set { SetObjectToViewState("ExportSelectedRowsOnly", false, value); }
		}
		public virtual int BottomMargin {
			get { return (int)GetObjectFromViewState("BottomMargin", -1); }
			set {
				if(value < -1)
					value = -1;
				SetObjectToViewState("BottomMargin", -1, value);
			}
		}
		public virtual int TopMargin {
			get { return (int)GetObjectFromViewState("TopMargin", -1); }
			set {
				if(value < -1)
					value = -1;
				SetObjectToViewState("TopMargin", -1, value);
			}
		}
		public virtual int LeftMargin {
			get { return (int)GetObjectFromViewState("LeftMargin", -1); }
			set {
				if(value < -1)
					value = -1;
				SetObjectToViewState("LeftMargin", -1, value);
			}
		}
		public virtual int RightMargin {
			get { return (int)GetObjectFromViewState("RightMargin", -1); }
			set {
				if(value < -1)
					value = -1;
				SetObjectToViewState("RightMargin", -1, value);
			}
		}
		[Obsolete("Use the ExportSelectedOnly property instead."), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public virtual GridViewExportedRowType ExportedRowType {
			get { return !ExportSelectedRowsOnly ? GridViewExportedRowType.All : GridViewExportedRowType.Selected; }
			set { ExportSelectedRowsOnly = value == GridViewExportedRowType.All ? false : true; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
		public override bool EnableTheming { get { return false; } set { } }
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string SkinID { get { return base.SkinID; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible { get { return false; } set { } }
		protected internal ASPxGridBase GridBase {
			get {
				if(gridBase == null)
					gridBase = GetGrid();
				return gridBase;
			}
			set { gridBase = value; }
		}
		protected GridLinkBase PrintableLink { get; set; }
		protected abstract GridLinkBase GetPrintableLink();
		protected void WriteExcelDataCore(Stream stream, ExportOptionsBase options, ExportTarget target) {
			var exporterOptions = DataAwareExportOptionsFactory.Create(target, options as IDataAwareExportOptions);
			PrepareExporterOptions(exporterOptions);
			exporterOptions.InitDefaults();
			using(var view = CreateExcelDataPrinter())
				new GridViewExcelExporter<GridXlsExportColumn, GridXlsExportRowBase>(view, exporterOptions).Export(stream);
		}
		protected virtual void PrepareExporterOptions(IDataAwareExportOptions options) {
			options.AllowCellMerge = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowCellMerge, false);
			options.AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowHyperLinks, true);
		}
		protected virtual GridExcelDataPrinter CreateExcelDataPrinter() { return new GridExcelDataPrinter(this); }
		protected virtual ASPxGridBase GetGrid() {
			if(Page == null)
				return null;
			if(string.IsNullOrEmpty(GridBaseID))
				return FindAnyGrid(Page);
			return FindControlHelper.LookupControl(this, GridBaseID) as ASPxGridBase;
		}
		protected virtual void InitializeLink(LinkBase link) {
			link.Landscape = Landscape;
			PageHeaderFooter headerFooter = new PageHeaderFooter(
				new PageHeaderArea(
					PageHeader.Texts,
					ExportStyleHelperBase.CreateFontByFontInfo(BrickStyle.DefaultFont, PageHeader.Font),
					PageHeader.VerticalAlignment
				),
				new PageFooterArea(
					PageFooter.Texts,
					ExportStyleHelperBase.CreateFontByFontInfo(BrickStyle.DefaultFont, PageFooter.Font),
					PageFooter.VerticalAlignment
				)
			);
			headerFooter.IncreaseMarginsByContent = true;
			link.PageHeaderFooter = headerFooter;
			link.RtfReportHeader = ReportHeader;
			link.RtfReportFooter = ReportFooter;
			link.PaperKind = PaperKind;
			link.PaperName = PaperName;
			if(BottomMargin > -1)
				link.Margins.Bottom = BottomMargin;
			if(TopMargin > -1)
				link.Margins.Top = TopMargin;
			if(LeftMargin > -1)
				link.Margins.Left = LeftMargin;
			if(RightMargin > -1)
				link.Margins.Right = RightMargin;
		}
		protected ASPxGridBase FindAnyGrid(Control control) {
			if(control is ASPxGridBase)
				return (ASPxGridBase)control;
			foreach(Control child in control.Controls) {
				var grid = FindAnyGrid(child);
				if(grid != null)
					return grid;
			}
			return null;
		}
		protected void WriteToResponse(string fileName, bool saveAsFile, ExportOptionsBase options, ExportTarget target) {
			using(MemoryStream stream = new MemoryStream()) {
				WriteDocumentToStream(stream, options, target);
				HttpUtils.WriteFileToResponse(Page, stream, fileName, saveAsFile, GetFileFormat(target));
			}
		}
		protected void WriteDocumentToStream(Stream stream, ExportOptionsBase options, ExportTarget target) {
			if(GridBase == null) return;
			var baseAction = GetExportAction(target);
			if(AllowExcelDataExport(options, target))
				WriteExcelDataCore(stream, options, target);
			else
				baseAction(stream, options);
		}
		protected virtual bool AllowExcelDataExport(ExportOptionsBase options, ExportTarget target) {
			if(target != ExportTarget.Csv && target != ExportTarget.Xls && target != ExportTarget.Xlsx)
				return false;
			var dataAwareOptions = options as IDataAwareExportOptions;
			var exportType = dataAwareOptions != null ? dataAwareOptions.ExportType : ExportSettings.DefaultExportType;
			return exportType == ExportType.DataAware;
		}
		protected virtual Action<Stream, ExportOptionsBase> GetExportAction(ExportTarget target) {
			switch(target) {
				case ExportTarget.Pdf:
					return WritePdfCore;
				case ExportTarget.Rtf:
					return WriteRtfCore;
				case ExportTarget.Csv:
					return WriteCsvCore;
				case ExportTarget.Xls:
					return WriteXlsCore;
				case ExportTarget.Xlsx:
					return WriteXlsxCore;
				case ExportTarget.Html:
				case ExportTarget.Image:
				case ExportTarget.Mht:
				case ExportTarget.Text:
					break;
			}
			return null;
		}
		protected string GetFileName() {
			if(!string.IsNullOrEmpty(FileName))
				return FileName;
			if(GridBase != null)
				return GridBase.ID;
			return DefaultFileName;
		}
		PrintingSystemBase GetPrintingSystem() {
			using(var link = CreateLink())
				return link.CreatePS();
		}
		#region Pdf Export
		protected virtual void WritePdfCore(Stream stream, ExportOptionsBase exportOptions) {
			var options = exportOptions as PdfExportOptions;
			using(var ps = GetPrintingSystem()) {
				if(options != null)
					ps.ExportToPdf(stream, options);
				else
					ps.ExportToPdf(stream);
			}
		}
		public void WritePdf(Stream stream) {
			WritePdf(stream, null);
		}
		public void WritePdfToResponse() {
			WritePdfToResponse(GetFileName());
		}
		public void WritePdfToResponse(string fileName) {
			WritePdfToResponse(fileName, true);
		}
		public void WritePdfToResponse(bool saveAsFile) {
			WritePdfToResponse(GetFileName(), saveAsFile);
		}
		public void WritePdfToResponse(string fileName, bool saveAsFile) {
			WritePdfToResponse(fileName, saveAsFile, null);
		}
		public void WritePdf(Stream stream, PdfExportOptions exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Pdf);
		}
		public void WritePdfToResponse(PdfExportOptions exportOptions) {
			WritePdfToResponse(GetFileName(), exportOptions);
		}
		public void WritePdfToResponse(string fileName, PdfExportOptions exportOptions) {
			WritePdfToResponse(fileName, true, exportOptions);
		}
		public void WritePdfToResponse(bool saveAsFile, PdfExportOptions exportOptions) {
			WritePdfToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WritePdfToResponse(string fileName, bool saveAsFile, PdfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Pdf);
		}
		#endregion
		#region Xls Export
		protected virtual void WriteXlsCore(Stream stream, ExportOptionsBase exportOptions) {
			var options = exportOptions as XlsExportOptionsEx;
			using(var ps = GetPrintingSystem()) {
				if(options != null)
					ps.ExportToXls(stream, options);
				else
					ps.ExportToXls(stream);
			}
		}
		public void WriteXls(Stream stream) {
			WriteXls(stream, null);
		}
		public void WriteXlsToResponse() {
			WriteXlsToResponse(GetFileName());
		}
		public void WriteXlsToResponse(string fileName) {
			WriteXlsToResponse(fileName, true);
		}
		public void WriteXlsToResponse(bool saveAsFile) {
			WriteXlsToResponse(GetFileName(), saveAsFile);
		}
		public void WriteXlsToResponse(string fileName, bool saveAsFile) {
			WriteXlsToResponse(fileName, saveAsFile, null);
		}
		public void WriteXls(Stream stream, XlsExportOptionsEx exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Xls);
		}
		public void WriteXlsToResponse(XlsExportOptionsEx exportOptions) {
			WriteXlsToResponse(GetFileName(), exportOptions);
		}
		public void WriteXlsToResponse(string fileName, XlsExportOptionsEx exportOptions) {
			WriteXlsToResponse(fileName, true, exportOptions);
		}
		public void WriteXlsToResponse(bool saveAsFile, XlsExportOptionsEx exportOptions) {
			WriteXlsToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteXlsToResponse(string fileName, bool saveAsFile, XlsExportOptionsEx exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Xls);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXls(Stream stream, XlsExportOptions exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Xls);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsToResponse(XlsExportOptions exportOptions) {
			WriteXlsToResponse(GetFileName(), exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsToResponse(string fileName, XlsExportOptions exportOptions) {
			WriteXlsToResponse(fileName, true, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsToResponse(bool saveAsFile, XlsExportOptions exportOptions) {
			WriteXlsToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsToResponse(string fileName, bool saveAsFile, XlsExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Xls);
		}
		#endregion
		#region Xlsx Export
		protected virtual void WriteXlsxCore(Stream stream, ExportOptionsBase exportOptions) {
			var options = exportOptions as XlsxExportOptionsEx;
			using(var ps = GetPrintingSystem()) {
				if(options != null)
					ps.ExportToXlsx(stream, options);
				else
					ps.ExportToXlsx(stream);
			}
		}
		public void WriteXlsx(Stream stream) {
			WriteXlsx(stream, null);
		}
		public void WriteXlsxToResponse() {
			WriteXlsxToResponse(GetFileName());
		}
		public void WriteXlsxToResponse(string fileName) {
			WriteXlsxToResponse(fileName, true);
		}
		public void WriteXlsxToResponse(bool saveAsFile) {
			WriteXlsxToResponse(GetFileName(), saveAsFile);
		}
		public void WriteXlsxToResponse(string fileName, bool saveAsFile) {
			WriteXlsxToResponse(fileName, saveAsFile, null);
		}
		public void WriteXlsx(Stream stream, XlsxExportOptionsEx exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Xlsx);
		}
		public void WriteXlsxToResponse(XlsxExportOptionsEx exportOptions) {
			WriteXlsxToResponse(GetFileName(), exportOptions);
		}
		public void WriteXlsxToResponse(string fileName, XlsxExportOptionsEx exportOptions) {
			WriteXlsxToResponse(fileName, true, exportOptions);
		}
		public void WriteXlsxToResponse(bool saveAsFile, XlsxExportOptionsEx exportOptions) {
			WriteXlsxToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteXlsxToResponse(string fileName, bool saveAsFile, XlsxExportOptionsEx exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Xlsx);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsx(Stream stream, XlsxExportOptions exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Xlsx);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsxToResponse(XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(GetFileName(), exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsxToResponse(string fileName, XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(fileName, true, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsxToResponse(bool saveAsFile, XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteXlsxToResponse(string fileName, bool saveAsFile, XlsxExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Xlsx);
		}
		#endregion
		#region Rtf Export
		protected virtual void WriteRtfCore(Stream stream, ExportOptionsBase exportOptions) {
			var options = exportOptions as RtfExportOptions;
			using(var ps = GetPrintingSystem()) {
				if(options != null)
					ps.ExportToRtf(stream, options);
				else
					ps.ExportToRtf(stream);
			}
		}
		public void WriteRtf(Stream stream) {
			WriteRtf(stream, null);
		}
		public void WriteRtfToResponse() {
			WriteRtfToResponse(GetFileName());
		}
		public void WriteRtfToResponse(string fileName) {
			WriteRtfToResponse(fileName, true);
		}
		public void WriteRtfToResponse(bool saveAsFile) {
			WriteRtfToResponse(GetFileName(), saveAsFile);
		}
		public void WriteRtfToResponse(string fileName, bool saveAsFile) {
			WriteRtfToResponse(fileName, saveAsFile, null);
		}
		public void WriteRtf(Stream stream, RtfExportOptions exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Rtf);
		}
		public void WriteRtfToResponse(RtfExportOptions exportOptions) {
			WriteRtfToResponse(GetFileName(), exportOptions);
		}
		public void WriteRtfToResponse(string fileName, RtfExportOptions exportOptions) {
			WriteRtfToResponse(fileName, true, exportOptions);
		}
		public void WriteRtfToResponse(bool saveAsFile, RtfExportOptions exportOptions) {
			WriteRtfToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteRtfToResponse(string fileName, bool saveAsFile, RtfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Rtf);
		}
		#endregion
		#region Csv Export
		protected virtual void WriteCsvCore(Stream stream, ExportOptionsBase exportOptions) {
			var options = exportOptions as CsvExportOptionsEx;
			using(var ps = GetPrintingSystem()) {
				if(options != null)
					ps.ExportToCsv(stream, options);
				else
					ps.ExportToCsv(stream);
			}
		}
		public void WriteCsv(Stream stream) {
			WriteCsv(stream, null);
		}
		public void WriteCsvToResponse() {
			WriteCsvToResponse(GetFileName());
		}
		public void WriteCsvToResponse(string fileName) {
			WriteCsvToResponse(fileName, true);
		}
		public void WriteCsvToResponse(bool saveAsFile) {
			WriteCsvToResponse(GetFileName(), saveAsFile);
		}
		public void WriteCsvToResponse(string fileName, bool saveAsFile) {
			WriteCsvToResponse(fileName, saveAsFile, null);
		}
		public void WriteCsv(Stream stream, CsvExportOptionsEx exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Csv);
		}
		public void WriteCsvToResponse(CsvExportOptionsEx exportOptions) {
			WriteCsvToResponse(GetFileName(), exportOptions);
		}
		public void WriteCsvToResponse(string fileName, CsvExportOptionsEx exportOptions) {
			WriteCsvToResponse(fileName, true, exportOptions);
		}
		public void WriteCsvToResponse(bool saveAsFile, CsvExportOptionsEx exportOptions) {
			WriteCsvToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		public void WriteCsvToResponse(string fileName, bool saveAsFile, CsvExportOptionsEx exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Csv);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteCsv(Stream stream, CsvExportOptions exportOptions) {
			WriteDocumentToStream(stream, exportOptions, ExportTarget.Csv);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteCsvToResponse(CsvExportOptions exportOptions) {
			WriteCsvToResponse(GetFileName(), exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteCsvToResponse(string fileName, CsvExportOptions exportOptions) {
			WriteCsvToResponse(fileName, true, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteCsvToResponse(bool saveAsFile, CsvExportOptions exportOptions) {
			WriteCsvToResponse(GetFileName(), saveAsFile, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public void WriteCsvToResponse(string fileName, bool saveAsFile, CsvExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, exportOptions, ExportTarget.Csv);
		}
		#endregion
		protected virtual GridLinkBase CreateLink() {
			var link = GetPrintableLink();
			InitializeLink(link); 
			return link;
		}
		protected string GetFileFormat(ExportTarget target) {
			switch(target) {
				case ExportTarget.Pdf:
					return "pdf";
				case ExportTarget.Rtf:
					return "rtf";
				case ExportTarget.Csv:
					return "csv";
				case ExportTarget.Xls:
					return "xls";
				case ExportTarget.Xlsx:
					return "xlsx";
				case ExportTarget.Html:
				case ExportTarget.Image:
				case ExportTarget.Mht:
				case ExportTarget.Text:
					break;
			}
			return string.Empty;
		}
		protected object GetObjectFromViewState(string name, object defaultValue) {
			object value = ViewState[name];
			return value != null ? value : defaultValue;
		}
		protected void SetObjectToViewState(string name, object defaultValue, object value) {
			if(value == defaultValue) {
				ViewState[name] = null;
			} else {
				ViewState[name] = value;
			}
		}
		bool IPrintable.CreatesIntersectedBricks { get { return false; } }
		void IPrintable.AcceptChanges() { }
		bool IPrintable.HasPropertyEditor() { return false; }
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() { }
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		#region IBasePrintable Members
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			var linkBase = link as LinkBase;
			if(linkBase != null && linkBase.Owner == null)
				InitializeLink(linkBase);
			PrintableLink = GetPrintableLink();
			PrintableLink.PrintingSystemBase = (PrintingSystemBase)ps;
			PrintableLink.InitializePrintableLink();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			OnCreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			PrintableLink.FinalizePrintableLink();
			PrintableLink.Dispose();
			PrintableLink = null;
		}
		#endregion
		protected void OnCreateArea(string areaName, IBrickGraphics graph) {
			((GridLinkBase)PrintableLink).CreateArea(areaName, graph);
		}
	}
}
namespace DevExpress.Web.Export {
	public abstract class GridLinkBase : LinkBase {
		Stack<GridPrinterBase> printers;
		public GridLinkBase(ASPxGridExporterBase exporterBase) {
			ExporterBase = exporterBase;
			GridBase = ExporterBase.GridBase;
		}
		protected ASPxGridExporterBase ExporterBase { get; private set; }
		protected ASPxGridBase GridBase { get; private set; }
		protected Stack<GridPrinterBase> Printers {
			get {
				if(printers == null)
					printers = new Stack<GridPrinterBase>();
				return printers;
			}
		}
		protected GridPrinterBase ActivePrinter {
			get {
				if(Printers.Count < 1)
					CreateFirstPrinter(GridBase);
				return (GridPrinterBase)Printers.Peek();
			}
		}
		public abstract void CreateArea(string areaName, IBrickGraphics graph);
		protected abstract void CreateFirstPrinter(ASPxGridBase grid);
		protected override void BeforeCreate() {
			base.BeforeCreate();
			ps.Graph.PageUnit = System.Drawing.GraphicsUnit.Pixel;
		}
		protected void RemovePrinter() {
			ActivePrinter.Dispose();
			Printers.Pop();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				while(Printers.Count > 0)
					RemovePrinter();
			}
			base.Dispose(disposing);
		}
		protected internal void InitializePrintableLink() {
			BeforeCreate();
		}
		protected internal void FinalizePrintableLink() {
			AfterCreate();
		}
		public PrintingSystemBase CreatePS() {
			var ps = new PrintingSystemBase();
			ps.PageSettings.AssignDefaultPageSettings();
			PrintingSystemBase = ps;
			CreateDocument();
			return ps;
		}
	}
}
