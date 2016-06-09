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
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList.Export;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.ASPxTreeList {
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxTreeList.Export.Design.ASPxTreeListExporterDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull), 
	PersistChildren(false), ParseChildren(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ASPxTreeListExporter), "Bitmaps256.ASPxTreeListExporter.bmp")
	]
	public class ASPxTreeListExporter : Control, IPrintable {
		static readonly object renderBrick = new object();
		ASPxTreeListPrintSettings settings;
		TreeListExportStyles exportStyles;
		[
		Category("Events")]
		public event ASPxTreeListRenderBrickEventHandler RenderBrick
		{
			add { Events.AddHandler(renderBrick, value); }
			remove { Events.RemoveHandler(renderBrick, value); }
		}
		public ASPxTreeListExporter() {
			this.settings = CreatePrintSettings(); 
			this.exportStyles = new TreeListExportStyles(null);
		}
		[
		Category("Data"), DefaultValue(""), Themeable(false), 
		TypeConverter("DevExpress.Web.ASPxTreeList.Export.Design.ASPxTreeListIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string TreeListID {
			get { return (string)ViewStateUtils.GetStringProperty(ViewState, "TreeListID", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				ViewStateUtils.SetStringProperty(ViewState, "TreeListID", string.Empty, value);
			}
		}
		[
		DefaultValue(""), Themeable(false)]
		public string FileName {
			get { return (string)ViewStateUtils.GetStringProperty(ViewState, "FileName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				ViewStateUtils.SetStringProperty(ViewState, "FileName", string.Empty, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ASPxTreeList TreeList {
			get {
				if(Page == null) return null;
				if(string.IsNullOrEmpty(TreeListID)) return FindTreeListRecursive(Page);
				return FindControlHelper.LookupControl(Page, TreeListID) as ASPxTreeList;
			}
		}
		ASPxTreeList FindTreeListRecursive(Control parent) {
			foreach(Control control in parent.Controls) {
				ASPxTreeList treeList = control as ASPxTreeList;
				if(treeList == null)
					treeList = FindTreeListRecursive(control);
				if(treeList != null) return treeList;
			}
			return null;
		}
		[
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxTreeListPrintSettings Settings { get { return settings; } }
		[
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeListExportStyles Styles { get { return exportStyles; } }
		protected virtual ASPxTreeListLink CreateLink() {
			if(TreeList == null)
				throw new ArgumentNullException("TreeList");
			ASPxTreeListLink link = new ASPxTreeListLink(this);
			link.PrintingSystemBase = new PrintingSystemBase();
			link.PaperKind = Settings.PageSettings.PaperKind;
			link.PaperName = Settings.PageSettings.PaperName;
			link.CreateDocument();
			return link;
		}
		protected virtual ASPxTreeListPrintSettings CreatePrintSettings() {
			return new ASPxTreeListPrintSettings();
		}
		protected string ExtractFileName() {
			if(!string.IsNullOrEmpty(FileName)) return FileName;
			if(TreeList != null) return TreeList.ID;
			return "TreeList";
		}
		protected internal void RaiseRenderBrick(ASPxTreeListExportRenderBrickEventArgs e) {
			ASPxTreeListRenderBrickEventHandler handler = (ASPxTreeListRenderBrickEventHandler)Events[renderBrick];
			if(handler != null)handler(this, e);
		}
		protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, ExportToStream getStream, ExportOptionsBase options) {
			using(MemoryStream stream = new MemoryStream()) {
				getStream(stream, options);
				HttpUtils.WriteFileToResponse(Page, stream, fileName, saveAsFile, fileFormat);
			}
		}
		protected delegate void ExportToStream(Stream stream, ExportOptionsBase options);
		#region Export to PDF
		public void WritePdf(Stream stream) {
			WritePdfCore(stream, null);
		}
		public void WritePdf(Stream stream, PdfExportOptions exportOptions) {
			WritePdfCore(stream, exportOptions);
		}
		public void WritePdfToResponse() {
			WritePdfToResponse(ExtractFileName());
		}
		public void WritePdfToResponse(string fileName) {
			WritePdfToResponse(fileName, true);
		}
		public void WritePdfToResponse(bool saveAsFile) {
			WritePdfToResponse(ExtractFileName(), saveAsFile);
		}
		public void WritePdfToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "pdf", new ExportToStream(WritePdfCore), null);
		}
		public void WritePdfToResponse(PdfExportOptions exportOptions) {
			WritePdfToResponse(ExtractFileName(), exportOptions);
		}
		public void WritePdfToResponse(string fileName, PdfExportOptions exportOptions) {
			WritePdfToResponse(fileName, true, exportOptions);
		}
		public void WritePdfToResponse(bool saveAsFile, PdfExportOptions exportOptions) {
			WritePdfToResponse(ExtractFileName(), saveAsFile, exportOptions);
		}
		protected void WritePdfCore(Stream stream, ExportOptionsBase options) {
			PdfExportOptions pdfOptions = options as PdfExportOptions;
			if(options == null)
				CreateLink().PrintingSystemBase.ExportToPdf(stream);
			else
				CreateLink().PrintingSystemBase.ExportToPdf(stream, pdfOptions);
		}
		protected void WritePdfToResponse(string fileName, bool saveAsFile, PdfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "pdf", new ExportToStream(WritePdfCore), exportOptions);
		}
		#endregion
		#region Export to Xls
		public void WriteXls(Stream stream) {
			WriteXlsCore(stream, null);
		}
		public void WriteXls(Stream stream, XlsExportOptions options) {
			WriteXlsCore(stream, options);
		}
		public void WriteXlsToResponse() {
			WriteXlsToResponse(ExtractFileName());
		}
		public void WriteXlsToResponse(string fileName) {
			WriteXlsToResponse(fileName, true);
		}
		public void WriteXlsToResponse(bool saveAsFile) {
			WriteXlsToResponse(ExtractFileName(), saveAsFile);
		}
		public void WriteXlsToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "xls", new ExportToStream(WriteXlsCore), null);
		}
		public void WriteXlsToResponse(XlsExportOptions exportOptions) {
			WriteXlsToResponse(ExtractFileName(), exportOptions);
		}
		public void WriteXlsToResponse(string fileName, XlsExportOptions exportOptions) {
			WriteXlsToResponse(fileName, true, exportOptions);
		}
		public void WriteXlsToResponse(bool saveAsFile, XlsExportOptions exportOptions) {
			WriteXlsToResponse(ExtractFileName(), saveAsFile, exportOptions);
		}
		protected void WriteXlsCore(Stream stream, ExportOptionsBase options) {
			XlsExportOptions xlsOptions = options as XlsExportOptions;
			if(options == null)
				CreateLink().PrintingSystemBase.ExportToXls(stream);
			else
				CreateLink().PrintingSystemBase.ExportToXls(stream, xlsOptions);
		}
		protected void WriteXlsToResponse(string fileName, bool saveAsFile, XlsExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "xls", new ExportToStream(WriteXlsCore), exportOptions);
		}
		#endregion
		#region Export to Xlsx
		public void WriteXlsx(Stream stream) {
			WriteXlsxCore(stream, null);
		}
		public void WriteXlsx(Stream stream, XlsxExportOptions options) {
			WriteXlsxCore(stream, options);
		}
		public void WriteXlsxToResponse() {
			WriteXlsxToResponse(ExtractFileName());
		}
		public void WriteXlsxToResponse(string fileName) {
			WriteXlsxToResponse(fileName, true);
		}
		public void WriteXlsxToResponse(bool saveAsFile) {
			WriteXlsxToResponse(ExtractFileName(), saveAsFile);
		}
		public void WriteXlsxToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "xlsx", new ExportToStream(WriteXlsxCore), null);
		}
		public void WriteXlsxToResponse(XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(ExtractFileName(), exportOptions);
		}
		public void WriteXlsxToResponse(string fileName, XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(fileName, true, exportOptions);
		}
		public void WriteXlsxToResponse(bool saveAsFile, XlsxExportOptions exportOptions) {
			WriteXlsxToResponse(ExtractFileName(), saveAsFile, exportOptions);
		}
		protected void WriteXlsxCore(Stream stream, ExportOptionsBase options) {
			XlsxExportOptions xlsxOptions = options as XlsxExportOptions;
			if(options == null)
				CreateLink().PrintingSystemBase.ExportToXlsx(stream);
			else
				CreateLink().PrintingSystemBase.ExportToXlsx(stream, xlsxOptions);
		}
		protected void WriteXlsxToResponse(string fileName, bool saveAsFile, XlsxExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "xlsx", new ExportToStream(WriteXlsxCore), exportOptions);
		}
		#endregion
		#region Export to Rtf
		public void WriteRtf(Stream stream) {
			WriteRtfCore(stream, null);
		}
		public void WriteRtf(Stream stream, RtfExportOptions options) {
			WriteRtfCore(stream, options);
		}
		public void WriteRtfToResponse() {
			WriteRtfToResponse(ExtractFileName());
		}
		public void WriteRtfToResponse(string fileName) {
			WriteRtfToResponse(fileName, true);
		}
		public void WriteRtfToResponse(bool saveAsFile) {
			WriteRtfToResponse(ExtractFileName(), saveAsFile);
		}
		public void WriteRtfToResponse(string fileName, bool saveAsFile) {
			WriteToResponse(fileName, saveAsFile, "rtf", new ExportToStream(WriteRtfCore), null);
		}
		public void WriteRtfToResponse(RtfExportOptions exportOptions) {
			WriteRtfToResponse(ExtractFileName(), exportOptions);
		}
		public void WriteRtfToResponse(string fileName, RtfExportOptions exportOptions) {
			WriteRtfToResponse(fileName, true, exportOptions);
		}
		public void WriteRtfToResponse(bool saveAsFile, RtfExportOptions exportOptions) {
			WriteRtfToResponse(ExtractFileName(), saveAsFile, exportOptions);
		}
		protected void WriteRtfCore(Stream stream, ExportOptionsBase options) {
			RtfExportOptions rtfOptions = options as RtfExportOptions;
			if(options == null)
				CreateLink().PrintingSystemBase.ExportToRtf(stream);
			else
				CreateLink().PrintingSystemBase.ExportToRtf(stream, rtfOptions);
		}
		protected void WriteRtfToResponse(string fileName, bool saveAsFile, RtfExportOptions exportOptions) {
			WriteToResponse(fileName, saveAsFile, "rtf", new ExportToStream(WriteRtfCore), exportOptions);
		}
		#endregion
		#region Hidden properties and methods
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible { get { return base.Visible; } set { base.Visible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableViewState { get { return base.EnableViewState; } set { base.EnableViewState = value; } }
		#endregion 
		#region IPrintable Members
		bool IPrintable.CreatesIntersectedBricks { get { return false; } }
		void IPrintable.AcceptChanges() { }
		bool IPrintable.HasPropertyEditor() { return false; }
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() { }
		void IPrintable.ShowHelp() { }
		bool IPrintable.SupportsHelp() { return false; }
		#endregion
		#region IBasePrintable Members
		ASPxTreeListLink iPrintableLink;
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			this.iPrintableLink = new ASPxTreeListLink(this);
			this.iPrintableLink.PrintingSystemBase = (PrintingSystemBase)ps;
			this.iPrintableLink.InitializePrintableLink();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			this.iPrintableLink.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			this.iPrintableLink.FinalizePrintableLink();
			this.iPrintableLink.Dispose();
			this.iPrintableLink = null;
		}
		#endregion
		public override void RenderControl(HtmlTextWriter writer) {
			base.RenderControl(writer);
		}
	}
	public class ASPxTreeListExportRenderBrickEventArgs : EventArgs {
		string nodeKey; 
		BrickStyle brickStyle;
		TreeListRowKind rowKind;		
		TreeListDataColumn column;
		string text;
		object value;
		string formatString;
		string url;
		internal ASPxTreeListExportRenderBrickEventArgs(string nodeKey, string text, object value, string url, TreeListDataColumn column, BrickStyle brickStyle, TreeListRowKind rowKind, byte[] imageValue) {
			this.nodeKey = nodeKey;
			this.text = text;
			this.value = value;
			this.formatString = String.Empty;
			if(column != null && rowKind != TreeListRowKind.Header)
				this.formatString = column.PropertiesEdit.DisplayFormatString;
			this.url = url;
			this.column = column;
			this.brickStyle = brickStyle;
			this.rowKind = rowKind;
			ImageValue = imageValue;
		}
		public string NodeKey { get { return nodeKey; } }
		public TreeListRowKind RowKind { get { return rowKind; } }
		public string Text { get { return text; } set { text = value; } }
		public object TextValue { get { return value; } set { this.value = value; } }
		public string TextValueFormatString { get { return formatString; } set { formatString = value; } }
		public string Url { get { return url; } set { url = value; } }
		public TreeListDataColumn Column { get { return column; } }
		public BrickStyle BrickStyle { get { return brickStyle; } }
		public byte[] ImageValue { get; set; }
	}
	public delegate void ASPxTreeListRenderBrickEventHandler(object sender, ASPxTreeListExportRenderBrickEventArgs e);
}
