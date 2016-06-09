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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.ASPxPivotGrid {
	using DevExpress.Web.ASPxPivotGrid.Export;
	using DevExpress.XtraExport;
	using DevExpress.Export;
	using DevExpress.PivotGrid.Export;
	[
	DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxPivotGrid.Export.Design.ASPxPivotGridExporterDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	PersistChildren(false), ParseChildren(true),
	System.Drawing.ToolboxBitmap(typeof(ASPxPivotGrid), "ASPxPivotGridExporter.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class ASPxPivotGridExporter : Control, IPivotGridPrinterOwner, IPivotGridOptionsPrintOwner {
		static readonly object customExportHeader = new object();
		static readonly object customExportFieldValue = new object();
		static readonly object customExportCell = new object();
		static readonly object exportStarted = new object();
		static readonly object exportFinished = new object();
		PivotGridWebPrinter printer;
		DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid pivotGrid;
		WebPivotGridOptionsPrint optionsPrint;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid PivotGrid {
			get {
				return pivotGrid;
			}
		}
		protected PivotGridWebData ASPxGridData {
			get {
				return PivotGrid != null ? PivotGrid.Data : null;
			}
		}
		protected void BestFitViewInfoData() {
			PivotGridWebData Data = ASPxGridData;
			if(Data == null)
				return;
			Dictionary<PivotGridFieldBase, int> sizes = new Dictionary<PivotGridFieldBase, int>();
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
			fields.AddRange(Data.GetFieldsByArea(PivotArea.DataArea, true));
			fields.AddRange(Data.GetFieldsByArea(PivotArea.ColumnArea, true));
			fields.AddRange(Data.GetFieldsByArea(PivotArea.RowArea, true));
			Printer.BestFitter.BeginBestFit();
			for(int i = 0; i < fields.Count; i++) {
				sizes.Add(fields[i], fields[i].Width);
				PivotGridField field = (PivotGridField)fields[i];
				if(field.ExportBestFit)
					Printer.BestFitter.BestFit(Data.GetFieldItem(field));
			}
			if(Data.VisualItems.RowTreeField != null && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
				Printer.BestFitter.BestFit(Data.GetFieldItem(Data.VisualItems.RowTreeField));
			Printer.BestFitter.EndBestFit();
			ResizingFieldsCache cache = new ResizingFieldsCache();
			foreach(KeyValuePair<PivotGridFieldBase, int> pair in sizes)
				if(pair.Value != pair.Key.Width)
					cache.Add(pair.Key, true, false);
			cache.RaiseFieldSizeChangedEvents(Data);
		}
		protected IPivotGridEventsImplementor DataOwner {
			get {
				return PivotGrid;
			}
		}
		protected internal WebPrintAppearance AppearancePrint {
			get {
				return PivotGrid != null ? PivotGrid.StylesPrint : null;
			}
		}
		protected PivotGridWebPrinter Printer {
			get {
				if(printer == null) {
					printer = CreatePrinter();
					printer.Owner = this;
				}
				return printer;
			}
		}
		protected virtual PivotGridWebPrinter CreatePrinter() {
			return new PivotGridWebPrinter(this, ASPxGridData, AppearancePrint, this);
		}
		bool ShouldSerializeOptionsPrint() {
			return OptionsPrint.ShouldSerialize();
		}
		void ResetOptionsPrint() {
			OptionsPrint.Reset();
		}
		[ Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), PersistenceMode(PersistenceMode.InnerProperty)]
		public WebPivotGridOptionsPrint OptionsPrint {
			get {
				return optionsPrint;
			}
		}
		public ASPxPivotGridExporter()
			: base() {
			optionsPrint = new WebPivotGridOptionsPrint(OnOptionsPrintChanged);
		}
		public override void Dispose() {
			if(this.printer != null)
				this.printer.Dispose();
			base.Dispose();
		}
		public override void RenderControl(HtmlTextWriter writer) {
			base.RenderControl(writer);
		}
		#region Events
		[ Category("Export")]
		public event EventHandler<WebCustomExportHeaderEventArgs> CustomExportHeader {
			add {
				Events.AddHandler(customExportHeader, value);
			}
			remove {
				Events.RemoveHandler(customExportHeader, value);
			}
		}
		[ Category("Export")]
		public event EventHandler<WebCustomExportFieldValueEventArgs> CustomExportFieldValue {
			add {
				Events.AddHandler(customExportFieldValue, value);
			}
			remove {
				Events.RemoveHandler(customExportFieldValue, value);
			}
		}
		[ Category("Export")]
		public event EventHandler<WebCustomExportCellEventArgs> CustomExportCell {
			add {
				Events.AddHandler(customExportCell, value);
			}
			remove {
				Events.RemoveHandler(customExportCell, value);
			}
		}
		[ Category("Export")]
		public event EventHandler ExportStarted {
			add {
				this.Events.AddHandler(exportStarted, value);
			}
			remove {
				this.Events.RemoveHandler(exportStarted, value);
			}
		}
		[ Category("Export")]
		public event EventHandler ExportFinished {
			add {
				this.Events.AddHandler(exportFinished, value);
			}
			remove {
				this.Events.RemoveHandler(exportFinished, value);
			}
		}
		#endregion
		#region Raise methods
		protected virtual bool RaiseCustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase fieldItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			EventHandler<WebCustomExportHeaderEventArgs> handler = (EventHandler<WebCustomExportHeaderEventArgs>)this.Events[customExportHeader];
			if(handler != null) {
				WebCustomExportHeaderEventArgs e = new WebCustomExportHeaderEventArgs(brick, fieldItem, (WebPrintAppearanceObject)appearance, PivotGrid.Data.GetField(fieldItem), ref rect);
				handler(this, e);
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			} else
				return false;
		}
		protected virtual bool RaiseCustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem item, IPivotPrintAppearance appearance, ref Rectangle rect) {
			EventHandler<WebCustomExportFieldValueEventArgs> handler = (EventHandler<WebCustomExportFieldValueEventArgs>)this.Events[customExportFieldValue];
			if(handler != null) {
				WebCustomExportFieldValueEventArgs e = new WebCustomExportFieldValueEventArgs(brick, item, (WebPrintAppearanceObject)appearance, ref rect);
				handler(this, e);
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			} else
				return false;
		}
		protected virtual bool RaiseCustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			EventHandler<WebCustomExportCellEventArgs> handler = (EventHandler<WebCustomExportCellEventArgs>)this.Events[customExportCell];
			if(handler != null) {
				WebCustomExportCellEventArgs e = new WebCustomExportCellEventArgs(brick, cellItem, (WebPrintAppearanceObject)appearance, PivotGrid.Data,
				 PivotGrid.Data.GetField(cellItem.ColumnField), PivotGrid.Data.GetField(cellItem.RowField), PivotGrid.Data.GetField(cellItem.DataField), graphicsUnit, printer, ref rect);
				handler(this, e);
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			} else
				return false;
		}
		protected virtual void RaiseExportStarted() {
			EventHandler handler = (EventHandler)this.Events[exportStarted];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseExportFinished() {
			EventHandler handler = (EventHandler)this.Events[exportFinished];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region ViewState
		IXtraPropertyCollection firstSnapshot;
		protected IXtraPropertyCollection FirstSnapshot {
			get {
				return firstSnapshot;
			}
		}
		protected void MakeFirstSnapshot() {
			firstSnapshot = GetSnapshot();
		}
		protected IXtraPropertyCollection GetSnapshot() {
			return new SnapshotSerializeHelper().SerializeObject(this, OptionsLayoutBase.FullLayout);
		}
		protected string SerializeSnapshot(IXtraPropertyCollection snapshot) {
			return new Base64XtraSerializer().Serialize(snapshot);
		}
		protected void DeserializeSnapshot(string base64Snapshot) {
			new Base64XtraSerializer().Deserialize(this, base64Snapshot);
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			MakeFirstSnapshot();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			MakeFirstSnapshot();
		}
		#endregion
		[
		DefaultValue(""), Themeable(false), Category("Data"),
		TypeConverter("DevExpress.Web.ASPxPivotGrid.Design.PivotGridIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)
		]
		public string ASPxPivotGridID {
			get {
				object value = ViewState["ASPxPivotGridID"];
				return value != null ? value.ToString() : string.Empty;
			}
			set {
				if(value == null)
					value = string.Empty;
				ViewState["ASPxPivotGridID"] = value;
				Reset();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible {
			get {
				return base.Visible;
			}
			set {
				base.Visible = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EnableViewState {
			get {
				return base.EnableViewState;
			}
			set {
				base.EnableViewState = value;
			}
		}
		void Reset() {
			pivotGrid = null;
			printer = null;
		}
		protected virtual void OnOptionsPrintChanged(object sender, EventArgs e) {
			if(printer != null)
				printer.Release();
		}
		#region Export
		void ExportToResponse(MemoryStream stream, string fileName, string fileFormat, string contentType, bool saveAsFile) {
			if(string.IsNullOrEmpty(fileName))
				fileName = "ASPxPivotGrid";
			DevExpress.Web.Internal.HttpUtils.WriteFileToResponse(Page, stream, fileName, saveAsFile, fileFormat, contentType);
		}
		void Export(ExportTarget target, string filePath) {
			Printer.PerformPrintingExportAction(delegate() {
				Printer.ComponentPrinter.Export(target, filePath);
			});
		}
		void Export(ExportTarget target, Stream stream) {
			Printer.PerformPrintingExportAction(delegate() {
				Printer.ComponentPrinter.Export(target, stream);
			});
		}
		void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			if(ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions,target)) {
				BindToPivotGridAndData();
				using(PivotGridViewImplementer gridView = new PivotGridViewImplementer(target, options, OptionsPrint, ASPxGridData, PivotGrid.Caption)) {
					gridView.Export(filePath);
				}
			}
			else
				Printer.PerformPrintingExportAction(delegate() {
					Printer.ComponentPrinter.Export(target, filePath, options);
				});
		}
		void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			if(ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions,target)) {
				BindToPivotGridAndData();
				using(PivotGridViewImplementer gridView = new PivotGridViewImplementer(target, options, OptionsPrint, ASPxGridData, PivotGrid.Caption)) {
					gridView.Export(stream);
				}
			}
			else
				Printer.PerformPrintingExportAction(delegate() {
					Printer.ComponentPrinter.Export(target, stream, options);
				});
		}
		public void ExportToXls(string filePath) {
			ExportToXls(filePath, TextExportMode.Value);
		}
		public void ExportToXls(string filePath, TextExportMode textExportMode) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, filePath, new XlsExportOptions(textExportMode));
			RaiseExportFinished();
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToXls(Stream stream) {
			ExportToXls(stream, TextExportMode.Value);
		}
		public void ExportToXls(Stream stream, TextExportMode textExportMode) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, stream, new XlsExportOptions(textExportMode));
			RaiseExportFinished();
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, stream, options);
			RaiseExportFinished();
		}
		public void ExportXlsToResponse(string fileName) {
			ExportXlsToResponse(fileName, new XlsExportOptions(TextExportMode.Value), true);
		}
		public void ExportXlsToResponse(string fileName, bool saveAsFile) {
			ExportXlsToResponse(fileName, new XlsExportOptions(TextExportMode.Value), saveAsFile);
		}
		public void ExportXlsToResponse(string fileName, TextExportMode textExportMode) {
			ExportXlsToResponse(fileName, new XlsExportOptions(textExportMode), true);
		}
		public void ExportXlsToResponse(string fileName, TextExportMode textExportMode, bool saveAsFile) {
			ExportXlsToResponse(fileName, new XlsExportOptions(textExportMode), saveAsFile);
		}
		public void ExportXlsToResponse(string fileName, XlsExportOptions options) {
			ExportXlsToResponse(fileName, options, true);
		}
		public void ExportXlsToResponse(string fileName, XlsExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToXls(stream, options);
				ExportToResponse(stream, fileName, "xls", "application/ms-excel", saveAsFile);
			}
		}
		public void ExportToXlsx(string filePath) {
			ExportToXlsx(filePath, TextExportMode.Value);
		}
		public void ExportToXlsx(string filePath, TextExportMode textExportMode) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, filePath, new XlsxExportOptions(textExportMode));
			RaiseExportFinished();
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToXlsx(Stream stream) {
			ExportToXlsx(stream, TextExportMode.Value);
		}
		public void ExportToXlsx(Stream stream, TextExportMode textExportMode) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, stream, new XlsxExportOptions(textExportMode));
			RaiseExportFinished();
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, stream, options);
			RaiseExportFinished();
		}
		public void ExportXlsxToResponse(string fileName) {
			ExportXlsxToResponse(fileName, new XlsxExportOptions(TextExportMode.Value), true);
		}
		public void ExportXlsxToResponse(string fileName, bool saveAsFile) {
			ExportXlsxToResponse(fileName, new XlsxExportOptions(TextExportMode.Value), saveAsFile);
		}
		public void ExportXlsxToResponse(string fileName, TextExportMode textExportMode) {
			ExportXlsxToResponse(fileName, new XlsxExportOptions(textExportMode), true);
		}
		public void ExportXlsxToResponse(string fileName, TextExportMode textExportMode, bool saveAsFile) {
			ExportXlsxToResponse(fileName, new XlsxExportOptions(textExportMode), saveAsFile);
		}
		public void ExportXlsxToResponse(string fileName, XlsxExportOptions options) {
			ExportXlsxToResponse(fileName, options, true);
		}
		public void ExportXlsxToResponse(string fileName, XlsxExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToXlsx(stream, options);
				ExportToResponse(stream, fileName, "xlsx", "application/ms-excel", saveAsFile);
			}
		}
		public void ExportToRtf(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Rtf, filePath);
			RaiseExportFinished();
		}
		public void ExportToRtf(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Rtf, stream);
			RaiseExportFinished();
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Rtf, stream, options);
			RaiseExportFinished();
		}
		public void ExportRtfToResponse(string fileName) {
			ExportRtfToResponse(fileName, true);
		}
		public void ExportRtfToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToRtf(stream);
				ExportToResponse(stream, fileName, "rtf", "text/enriched", saveAsFile);
			}
		}
		public void ExportToHtml(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath);
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, new HtmlExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream, options);
			RaiseExportFinished();
		}
		public void ExportHtmlToResponse(string fileName) {
			ExportHtmlToResponse(fileName, true);
		}
		public void ExportHtmlToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToHtml(stream);
				ExportToResponse(stream, fileName, "htm", "text/html", saveAsFile);
			}
		}
		public void ExportHtmlToResponse(string fileName, string htmlCharSet, string title, bool compressed) {
			ExportHtmlToResponse(fileName, new HtmlExportOptions(htmlCharSet, title, compressed), true);
		}
		public void ExportHtmlToResponse(string fileName, string htmlCharSet, string title, bool compressed, bool saveAsFile) {
			ExportHtmlToResponse(fileName, new HtmlExportOptions(htmlCharSet, title, compressed), saveAsFile);
		}
		public void ExportHtmlToResponse(string fileName, HtmlExportOptions options) {
			ExportHtmlToResponse(fileName, options, true);
		}
		public void ExportHtmlToResponse(string fileName, HtmlExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToHtml(stream, options);
				ExportToResponse(stream, fileName, "htm", "text/html", saveAsFile);
			}
		}
		public void ExportToMht(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath);
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, new MhtExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, stream, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, stream);
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, stream, options);
			RaiseExportFinished();
		}
		public void ExportMhtToResponse(string fileName, string htmlCharSet, string title, bool compressed) {
			ExportMhtToResponse(fileName, new MhtExportOptions(htmlCharSet, title, compressed), true);
		}
		public void ExportMhtToResponse(string fileName, string htmlCharSet, string title, bool compressed, bool saveAsFile) {
			ExportMhtToResponse(fileName, new MhtExportOptions(htmlCharSet, title, compressed), saveAsFile);
		}
		public void ExportMhtToResponse(string fileName, bool saveAsFile) {
			ExportMhtToResponse(fileName, new MhtExportOptions(), saveAsFile);
		}
		public void ExportMhtToResponse(string fileName, MhtExportOptions options) {
			ExportMhtToResponse(fileName, options, true);
		}
		public void ExportMhtToResponse(string fileName, MhtExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToMht(stream, options);
				ExportToResponse(stream, fileName, "mht", "multipart/related", saveAsFile);
			}
		}
		public void ExportToPdf(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, filePath);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, stream);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, stream, options);
			RaiseExportFinished();
		}
		public void ExportPdfToResponse(string fileName) {
			ExportPdfToResponse(fileName, true);
		}
		public void ExportPdfToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToPdf(stream);
				ExportToResponse(stream, fileName, "pdf", "application/pdf", saveAsFile);
			}
		}
		public void ExportPdfToResponse(string fileName, PdfExportOptions options) {
			ExportPdfToResponse(fileName, options, true);
		}
		public void ExportPdfToResponse(string fileName, PdfExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToPdf(stream, options);
				ExportToResponse(stream, fileName, "pdf", "application/pdf", saveAsFile);
			}
		}
		public void ExportToText(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator, Encoding encoding) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator, Encoding encoding) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportTextToResponse(string fileName) {
			ExportTextToResponse(fileName, true);
		}
		public void ExportTextToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToText(stream);
				ExportToResponse(stream, fileName, "txt", "text/plain", saveAsFile);
			}
		}
		public void ExportTextToResponse(string fileName, string separator) {
			ExportTextToResponse(fileName, separator, true);
		}
		public void ExportTextToResponse(string fileName, string separator, bool saveAsFile) {
			ExportTextToResponse(fileName, new TextExportOptions(separator), saveAsFile);
		}
		public void ExportTextToResponse(string fileName, TextExportOptions options) {
			ExportTextToResponse(fileName, options, true);
		}
		public void ExportTextToResponse(string fileName, TextExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToText(stream, options);
				ExportToResponse(stream, fileName, "txt", "text/plain", saveAsFile);
			}
		}
		public void ExportToCsv(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, filePath);
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, stream);
			RaiseExportFinished();
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, stream, options);
			RaiseExportFinished();
		}
		public void ExportCsvToResponse(string fileName) {
			ExportCsvToResponse(fileName, true);
		}
		public void ExportCsvToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToCsv(stream);
				ExportToResponse(stream, fileName, "csv", "text/csv", saveAsFile);
			}
		}
		public void ExportCsvToResponse(string fileName, CsvExportOptions options) {
			ExportCsvToResponse(fileName, options, true);
		}
		public void ExportCsvToResponse(string fileName, CsvExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToCsv(stream, options);
				ExportToResponse(stream, fileName, "csv", "text/csv", saveAsFile);
			}
		}
		public void ExportToImage(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Image, filePath);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Image, stream);
			RaiseExportFinished();
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Image, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Image, stream, options);
			RaiseExportFinished();
		}
		public void ExportImageToResponse(string fileName) {
			ExportImageToResponse(fileName, true);
		}
		public void ExportImageToResponse(string fileName, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToImage(stream);
				ImageExportOptions options = Printer.ComponentPrinter.PrintingSystemBase.ExportOptions.Image;
				string fileFormat = ImageFormatHelper.ToExtension(options.Format);
				ExportToResponse(stream, fileName, fileFormat, HttpUtils.GetContentType(fileFormat), saveAsFile);
			}
		}
		public void ExportImageToResponse(string fileName, ImageExportOptions options) {
			ExportImageToResponse(fileName, options, true);
		}
		public void ExportImageToResponse(string fileName, ImageExportOptions options, bool saveAsFile) {
			using(MemoryStream stream = new MemoryStream()) {
				ExportToImage(stream, options);
				string fileFormat = ImageFormatHelper.ToExtension(options.Format);
				ExportToResponse(stream, fileName, fileFormat, HttpUtils.GetContentType(fileFormat), saveAsFile);
			}
		}
		#endregion
		#region IPrintable Members
		bool IPrintable.CreatesIntersectedBricks {
			get {
				return false;
			}
		}
		void IPrintable.AcceptChanges() {
			Printer.AcceptChanges();
		}
		bool IPrintable.HasPropertyEditor() {
			return false;
		}
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl {
			get {
				return null;
			}
		}
		void IPrintable.RejectChanges() {
			Printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
		}
		bool IPrintable.SupportsHelp() {
			return false;
		}
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			Printer.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(Printer != null)
				Printer.Release();
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			Printer.SetCommandsVisibility(ps);
			BindToPivotGridAndData();
			Printer.Initialize(ps, link);
		}
		#endregion
		void BindToPivotGridAndData() {
			BindToPivotGrid();
			if(PivotGrid != null)
				PivotGrid.EnsureRefreshData();
			BestFitViewInfoData();
		}
		DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid FindControl(Control root, string id, Control excludeControl) {
			if(root == null)
				return null;
			foreach(Control control in root.Controls) {
				if(control == excludeControl)
					continue;
				if(control.ID == id)
					return control as DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid;
			}
			foreach(Control control in root.Controls) {
				if(control == excludeControl)
					continue;
				Control result = FindControl(control, id, control.Parent);
				if(result != null)
					return result as DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid;
			}
			if(root.Parent != excludeControl)
				return FindControl(root.Parent, id, root);
			return null;
		}
		protected void BindToPivotGrid() {
			Reset();
			pivotGrid = FindControl(this, ASPxPivotGridID, this);
			if(pivotGrid == null)
				pivotGrid = GetPivotGrid();
			if(PivotGrid == null)
				throw new Exception("The control specified by the ASPxPivotGridID property couldn't be found.");
		}
		ASPxPivotGrid GetPivotGrid() {
			if(Page == null)
				return null;
			if(string.IsNullOrEmpty(ASPxPivotGridID))
				return FindAnyPivotGrid(Page);
			return FindControlHelper.LookupControl(this, ASPxPivotGridID) as ASPxPivotGrid;
		}
		ASPxPivotGrid FindAnyPivotGrid(Control control) {
			if(control is ASPxPivotGrid)
				return control as ASPxPivotGrid;
			foreach(Control child in control.Controls) {
				ASPxPivotGrid grid = FindAnyPivotGrid(child);
				if(grid != null)
					return grid;
			}
			return null;
		}
		#region IPivotGridPrinterOwner Members
		bool IPivotGridPrinterOwner.CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return RaiseCustomExportCell(ref brick, cellItem, appearance, graphicsUnit, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem item, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaiseCustomExportFieldValue(ref brick, item, appearance, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase fieldItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaiseCustomExportHeader(ref brick, fieldItem, appearance, ref rect);
		}
		#endregion
		#region IPivotGridOptionsPrintOwner Members
		PivotGridOptionsPrint IPivotGridOptionsPrintOwner.OptionsPrint {
			get {
				return OptionsPrint;
			}
		}
		#endregion
	}
	public class WebPivotGridOptionsPrint : PivotGridOptionsPrint {
		public WebPivotGridOptionsPrint(EventHandler optionsChanged)
			: base(optionsChanged) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool UsePrintAppearance {
			get {
				return base.UsePrintAppearance;
			}
			set {
				base.UsePrintAppearance = value;
			}
		}
		bool ShouldSerializePageSettings() {
			return !PageSettings.IsEmpty;
		}
		void ResetPageSettings() {
			PageSettings.Reset();
		}
		[ XtraSerializableProperty(XtraSerializationVisibility.Content),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public override PivotGridPageSettings PageSettings {
			get {
				return base.PageSettings;
			}
		}
	}
}
namespace DevExpress.Web.ASPxPivotGrid.Export {
	public class ImageFormatHelper {
		static Dictionary<ImageFormat, string> formatExtensions = new Dictionary<ImageFormat, string>();
		static ImageFormatHelper() {
			ImageFormat[] formats = { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf };
			string[] extensions = { "bmp", "emf", "gif", "ico", "jpg", "png", "tif", "wmf" };
			for(int i = 0; i < formats.Length; i++)
				formatExtensions.Add(formats[i], extensions[i]);
		}
		public static string ToExtension(ImageFormat format) {
			string result;
			if(formatExtensions.TryGetValue(format, out result))
				return result;
			return "png";
		}
	}
}
