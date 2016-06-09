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
using System.Text;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Services;
using DevExpress.Office.Services.Implementation;
using System.Net;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class HtmlEditorDocument : PropertiesBase {
		HtmlEditorCssFileCollection cssFiles = null;
		bool htmlRequiresCorrection = false;
		bool htmlCorrectingHandled = false;
		internal HtmlEditorDocument(ASPxHtmlEditor editor)
			: base(editor) {
			this.cssFiles = new HtmlEditorCssFileCollection(Owner);
		}
		protected new ASPxHtmlEditor Owner { get { return (ASPxHtmlEditor)base.Owner; } }
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				CssFiles
			});
		}
		string HtmlInternal {
			get { return GetStringProperty("Html", ""); }
			set { SetStringProperty("Html", "", value); }
		}
		internal string Html {
			get {
				if(HtmlRequiresCorrection && !Owner.DesignMode) {
					HtmlInternal = GetProcessedHtml(HtmlInternal, false, false);
					HtmlRequiresCorrection = false;
				}
				return HtmlInternal;
			}
			set {
				if(!string.Equals(value, HtmlInternal, StringComparison.InvariantCulture)) {
					HtmlRequiresCorrection = true;
					HtmlInternal = value;
					Owner.PropertyChanged("Document");
				}
			}
		}
		internal HtmlEditorCssFileCollection CssFiles { get { return cssFiles; } }
		bool HtmlRequiresCorrection {
			get { return htmlRequiresCorrection; }
			set { htmlRequiresCorrection = value; }
		}
		bool HtmlCorrectionHandled {
			get { return htmlCorrectingHandled; }
			set { htmlCorrectingHandled = value; }
		}
		internal string GetProcessedHtml(string html, bool requiresDecoding, bool requiresProcessing) {
			if(requiresDecoding)
				html = HtmlConvertor.DecodeHtml(html);
			HtmlCorrectingEventArgs e = new HtmlCorrectingEventArgs(html);
			Owner.OnHtmlCorrecting(e);
			if(e.Handled)
				HtmlCorrectionHandled = true;
			html = e.Html;
			return !e.Handled || requiresProcessing ? HtmlProcessor.ProcessHtml(html, Owner.SettingsHtmlEditing) : html;
		}
		internal bool LoadHtml(string html) {
			html = GetProcessedHtml(html, true, false);
			bool changed = !html.Equals(Html);
			HtmlInternal = html;
			return changed;
		}
		internal bool IsHtmlCorrectionHandled() {
			return HtmlCorrectionHandled;
		}
		internal void RequireHtmlCorrection() {
			HtmlRequiresCorrection = true;
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			HtmlEditorDocument document = source as HtmlEditorDocument;
			if(document != null) {
				HtmlInternal = document.HtmlInternal;
				CssFiles.Assign(document.CssFiles);
			}
		}
		const string HtmlFormat = @"<html>
<head>
{0}
</head>
<body>
{1}
</body>
</html>";
		string GetHtml() {
			return GetHtml(GetHeadContent(), Owner.Html);
		}
		string GetHtml(string headContent, string bodyContent) {
			return string.Format(HtmlFormat, headContent ?? "", bodyContent);
		}
		string GetIncludeCssFilesHtml() {
			StringBuilder builder = new StringBuilder();
			foreach(string cssFile in Owner.GetClientCssFiles())
				builder.AppendLine(RenderUtils.GetLinkHtml(cssFile));
			return builder.ToString();
		}
		string GetHeadContent() {
			string docStyleCssText = Owner.GetDocumentStyleCssText();
			return string.IsNullOrEmpty(docStyleCssText) ? GetIncludeCssFilesHtml()
				: string.Format("<style type='text/css'>body {{ {0} }}</style>{1}", docStyleCssText, GetIncludeCssFilesHtml() ?? "");
		}
		void ExportRichEditCore(DocumentFormat format, Stream stream) {
			using(DocumentModel model = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				PrepareDocumentModel(model);
				if(format == DocumentFormat.PlainText)
					model.DocumentExportOptions.PlainText.Encoding = Encoding.UTF8;
				model.SaveDocument(stream, format, null);
			}
		}
		void PrepareDocumentModel(DocumentModel model) {
			HtmlDocumentImporterOptions options = new HtmlDocumentImporterOptions();
			HttpRequest request = HttpUtils.GetRequest(Owner);
			model.GetService<DevExpress.Office.Services.IUriStreamService>().RegisterProvider(new StreamProvider(request));
			if(request != null)
				options.SourceUri = request.Url.AbsoluteUri;
			model.InternalAPI.SetDocumentHtmlContent(GetHtml(), options);
		}
		static DocumentFormat GetDocumentFormat(HtmlEditorExportFormat format) {
			return HtmlEditorImportHelper.GetDocumentFormat(format.ToString());
		}
		void ExportPrintingCore(Action<PrintingSystemBase> export) {
			using(DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer server = DevExpress.XtraRichEdit.Internal.InnerRichEditDocumentServer.CreateServer(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				PrepareDocumentModel(server.Model);
				using(PrintingSystemBase ps = new PrintingSystemBase()) {
					using(PrintableComponentLinkBase pcl = new PrintableComponentLinkBase(ps)) {
						pcl.Component = server;
						pcl.CreateDocument();
						export(pcl.PrintingSystemBase);
					}
				}
			}
		}
		public void Export(HtmlEditorExportFormat format) {
			Export(format, string.Empty);
		}
		public void Export(HtmlEditorExportFormat format, bool saveAsFile) {
			Export(format, string.Empty, saveAsFile);
		}
		public void Export(HtmlEditorExportFormat format, string fileName) {
			Export(format, fileName, true);
		}
		void Export(HtmlEditorExportFormat format, string fileName, bool saveAsFile) {
			string fileFormat = format.ToString().ToLower();
			using(MemoryStream stream = new MemoryStream()) {
				Export(stream, format);
				HttpUtils.WriteFileToResponse(Owner.Page, stream, GetFileName(fileName), saveAsFile, fileFormat);
			}
		}
		public void Export(Stream stream, HtmlEditorExportFormat format) {
			switch(format) {
				case HtmlEditorExportFormat.Rtf:
				case HtmlEditorExportFormat.Mht:
				case HtmlEditorExportFormat.Odt:
				case HtmlEditorExportFormat.Docx:
				case HtmlEditorExportFormat.Txt:
					ExportRichEditCore(GetDocumentFormat(format), stream);
					break;
				case HtmlEditorExportFormat.Pdf:
					ExportPrintingCore(delegate(PrintingSystemBase ps) {
						ps.ExportToPdf(stream);
					});
					break;
				default:
					throw new NotImplementedException();
			}
		}
		string GetFileName(string fileName) {
			if(!string.IsNullOrEmpty(fileName))
				return fileName;
			if(!string.IsNullOrEmpty(Owner.ID))
				return Owner.ID;
			return Owner.GetType().Name;
		}
		public void Import(string filePath) {
			Import(filePath, false);
		}
		public void Import(string filePath, bool useInlineStyles) {
			Import(filePath, useInlineStyles, null);
		}
		public void Import(string filePath, string contentFolder) {
			Import(filePath, false, contentFolder);
		}
		public void Import(string filePath, bool useInlineStyles, string contentFolder) {
			Import(HtmlEditorImportHelper.ParseImportFormat(filePath), filePath, useInlineStyles, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, string filePath) {
			Import(format, filePath, false);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles) {
			Import(format, filePath, useInlineStyles, null);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, string contentFolder) {
			Import(format, filePath, false, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, string filePath, bool useInlineStyles, string contentFolder) {
			using(FileStream input = File.OpenRead(UrlUtils.ResolvePhysicalPath(filePath))) {
				Import(format, input, useInlineStyles, contentFolder);
			}
		}
		public void Import(HtmlEditorImportFormat format, Stream input) {
			Import(format, input, false);
		}
		public void Import(HtmlEditorImportFormat format, Stream input, bool useInlineStyles) {
			Import(format, input, useInlineStyles, null);
		}
		public void Import(HtmlEditorImportFormat format, Stream input, string contentFolder) {
			Import(format, input, false, contentFolder);
		}
		public void Import(HtmlEditorImportFormat format, Stream input, bool useInlineStyles, string contentFolder) {
			HtmlEditorImportHelper.Import(
				Owner,
				format,
				input,
				contentFolder,
				Owner.SettingsDialogs.InsertImageDialog.SettingsImageUpload.UploadFolder,
				useInlineStyles,
				delegate(string html, IEnumerable<string> cssFiles) {
					Owner.Html = html;
					Owner.Document.CssFiles.Clear();
					foreach(string cssFile in cssFiles)
						Owner.Document.CssFiles.Add(cssFile);
				}
			);
		}
	}
	class StreamProvider : DevExpress.Office.Services.IUriStreamProvider {
		protected HttpRequest Request { get; private set; }
		public StreamProvider(HttpRequest request) {
			Request = request;
		}
		#region IUriStreamProvider Members
		public Stream GetStream(string uri) {
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
			req.CookieContainer = new CookieContainer();
			var result = new MemoryStream();
			using(var source = req.GetResponse().GetResponseStream()) {
				CommonUtils.CopyStream(source, result);
				result.Seek(0, SeekOrigin.Begin);
			}
			return result;
		}
		#endregion
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public static class HtmlEditorImportHelper {
		class UriProvider : IUriProvider {
			const string RelativeUri = @"\";
			FileBasedUriProvider innerProvider = new FileBasedUriProvider();
			string targetUri, physicalPath;
			IUrlResolutionService service;
			List<string> cssFiles = new List<string>();
			public UriProvider(string targetUri, IUrlResolutionService service) {
				this.physicalPath = UrlUtils.ResolvePhysicalPath(targetUri).TrimEnd('\\') + RelativeUri;
				this.targetUri = targetUri.StartsWith("~")
					? targetUri
					: UrlUtils.GetAppRelativePath(PhysicalPath);
				this.service = service;
			}
			FileBasedUriProvider InnerProvider { get { return innerProvider; } }
			string TargetUri { get { return targetUri; } }
			string PhysicalPath { get { return physicalPath; } }
			IUrlResolutionService Service { get { return service; } }
			public List<string> CssFiles { get { return cssFiles; } }
			string GetRelativeUri(string uri) {
				return Path.Combine(TargetUri, uri.Substring(1));
			}
			string GetClientUri(string uri) {
				return Service.ResolveClientUrl(GetRelativeUri(uri));
			}
			public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
				string uri = InnerProvider.CreateCssUri(PhysicalPath, styleText, RelativeUri);
				CssFiles.Add(GetRelativeUri(uri));
				if(!string.IsNullOrEmpty(uri))
					return GetClientUri(uri);
				throw new Exception(string.Format(StringResources.HtmlEditorExceptionText_ImportFileContentFolderAccessDenied, PhysicalPath));
			}
			public string CreateImageUri(string rootUri, DevExpress.Office.Utils.OfficeImage image, string relativeUri) {
				string uri = InnerProvider.CreateImageUri(PhysicalPath, image, RelativeUri);
				if(!string.IsNullOrEmpty(uri))
					return GetClientUri(uri);
				throw new Exception(string.Format(StringResources.HtmlEditorExceptionText_ImportFileContentFolderAccessDenied, PhysicalPath));
			}
		}
		public delegate void OnImport(string html, IEnumerable<string> cssFiles);
		public static void Import(IUrlResolutionService service, HtmlEditorImportFormat documentFormat, Stream input, string contentFolder, string defaultContentFolder, bool useInlineStyles, OnImport onImport) {
			using (DocumentModel model = new DocumentModel(DevExpress.XtraRichEdit.Internal.RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				model.LoadDocument(input, GetDocumentFormat(documentFormat), null);
				HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
				options.CssPropertiesExportType = useInlineStyles
					? XtraRichEdit.Export.Html.CssPropertiesExportType.Inline
					: XtraRichEdit.Export.Html.CssPropertiesExportType.Link;
				options.EmbedImages = false;
				UriProvider provider = new UriProvider(GetTargetUri(contentFolder, defaultContentFolder), service);
				model.GetService<IUriProviderService>().RegisterProvider(provider);
				using(MemoryStream output = new MemoryStream()) {
					model.InternalAPI.SaveDocumentHtmlContent(output, options);
					output.Position = 0;
					using(StreamReader reader = new StreamReader(output))
						onImport(reader.ReadToEnd(), provider.CssFiles);
				}
			}
		}
		static DocumentFormat GetDocumentFormat(HtmlEditorImportFormat format) {
			return GetDocumentFormat(format.ToString());
		}
		public static DocumentFormat GetDocumentFormat(string format) {
			switch(format) {
				case "Rtf":
					return DocumentFormat.Rtf;
				case "Mht":
					return DocumentFormat.Mht;
				case "Odt":
					return DocumentFormat.OpenDocument;
				case "Docx":
					return DocumentFormat.OpenXml;
				case "Txt":
					return DocumentFormat.PlainText;
				default:
					throw new NotImplementedException();
			}
		}
		static string GetTargetUri(string contentFolder, string defaultContentFolder) {
			if(!string.IsNullOrEmpty(contentFolder))
				return contentFolder;
			if(!string.IsNullOrEmpty(defaultContentFolder))
				return defaultContentFolder;
			return "~/";
		}
		public static HtmlEditorImportFormat ParseImportFormat(string filePath) {
			string extension = new FileInfo(UrlUtils.ResolvePhysicalPath(filePath)).Extension.TrimStart('.');
			try {
				string formatStr = extension.Substring(0, 1).ToUpper() + extension.Substring(1).ToLower();
				return (HtmlEditorImportFormat)Enum.Parse(typeof(HtmlEditorImportFormat), formatStr);
			}
			catch(Exception e) {
				throw new ArgumentException("The format of the specified file is not supported", e);
			}
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorDocumentFontInfo : PropertiesBase {
		public HtmlEditorDocumentFontInfo(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentFontInfoBold"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Bold
		{
			get { return GetBoolProperty("Bold", false); }
			set { SetBoolProperty("Bold", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentFontInfoItalic"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool Italic
		{
			get { return GetBoolProperty("Italic", false); }
			set { SetBoolProperty("Italic", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentFontInfoName"),
#endif
		Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design", typeof(UITypeEditor)),
		DefaultValue(""), TypeConverter(typeof(FontConverter.FontNameConverter)), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), RefreshProperties(RefreshProperties.Repaint)]
		public string Name
		{
			get { return GetStringProperty("Name", string.Empty); }
			set
			{
				SetStringProperty("Name", string.Empty, value);
				if (string.IsNullOrEmpty(value))
				{
					if (Names.Length > 0)
						Names = new string[0];
				}
				else if (Names.Length == 0 || Names[0] != value)
					Names = new string[] { value };
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentFontInfoNames"),
#endif
		Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design", typeof(UITypeEditor)),
		TypeConverter(typeof(FontNamesConverter)), NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
		public string[] Names
		{
			get { return (string[])GetObjectProperty("Names", new string[0]); }
			set
			{
				SetObjectProperty("Names", new string[0], value);
				if (value == null || value.Length == 0)
				{
					if (!string.IsNullOrEmpty(Name))
						Name = string.Empty;
				}
				else if (value[0] != Name)
					Name = value[0];
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentFontInfoSize"),
#endif
		DefaultValue(typeof(FontUnit), ""), NotifyParentProperty(true)]
		public FontUnit Size
		{
			get { return (FontUnit)GetObjectProperty("Size", FontUnit.Empty); }
			set { SetObjectProperty("Size", FontUnit.Empty, value); }
		}
		public override string ToString() {
			string result = !string.IsNullOrEmpty(Name) ? string.Format("{0}{1}", Name, !Size.IsEmpty ? ", " : "") : "";
			result += Size.ToString();
			return result;
		}
	}
	public class HtmlEditorDocumentStyles : PropertiesBase {
		HtmlEditorDocumentFontInfo fontInfo;
		public HtmlEditorDocumentStyles(ASPxHtmlEditor editor)
			: base(editor) {
			fontInfo = new HtmlEditorDocumentFontInfo(editor);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentStylesBackColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
		public Color BackColor
		{
			get { return GetColorProperty("BackColor", Color.Empty); }
			set { SetColorProperty("BackColor", Color.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentStylesForeColor"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter))]
		public Color ForeColor
		{
			get { return GetColorProperty("ForeColor", Color.Empty); }
			set { SetColorProperty("ForeColor", Color.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentStylesHorizontalAlign"),
#endif
		NotifyParentProperty(true), DefaultValue(HorizontalAlign.NotSet)]
		public HorizontalAlign HorizontalAlign
		{
			get { return (HorizontalAlign)GetEnumProperty("HorizontalAlign", HorizontalAlign.NotSet); }
			set { SetEnumProperty("HorizontalAlign", HorizontalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentStylesFont"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDocumentFontInfo Font { get { return fontInfo; } }
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Font });
		}
		public AppearanceStyleBase MergeStyles(AppearanceStyleBase source) {
			source.BackColor = BackColor.IsEmpty ? source.BackColor : BackColor;
			source.HorizontalAlign = HorizontalAlign.NotSet == HorizontalAlign ? source.HorizontalAlign : HorizontalAlign;
			source.ForeColor = ForeColor.IsEmpty ? source.ForeColor : ForeColor;
			source.Font.Names = Font.Names.Length == 0 ? source.Font.Names : Font.Names;
			source.Font.Size = Font.Size.IsEmpty ? source.Font.Size : Font.Size;
			if(Font.Bold)
				source.Font.Bold = Font.Bold;
			if(Font.Italic)
				source.Font.Italic = Font.Italic;
			return source;
		}
		public override string ToString() {
			return string.Empty;
		}
	}
}
