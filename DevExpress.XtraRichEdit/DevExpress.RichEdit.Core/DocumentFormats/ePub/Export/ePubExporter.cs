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
using System.Text;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Office.Export.Html;
namespace DevExpress.XtraRichEdit.Export.EPub {
	#region EPubExporter
	public class EPubExporter : DocumentModelExporter, IUriProvider {
		#region Fields
		readonly EPubDocumentExporterOptions options;
		readonly EPubHtmlExporter htmlExporter;
		readonly Dictionary<string, string> htmlParts;
		readonly Dictionary<string, OfficeImage> imageParts;
		readonly Dictionary<string, string> tableOfContents;
		Stream outputStream;
		XmlWriter documentContentWriter;
		InternalZipArchive package;
		DateTime now;
		int contentIndex;
		int imageIndex;
		string styles;
		#endregion
		public EPubExporter(DocumentModel documentModel, EPubDocumentExporterOptions options)
			: base(documentModel) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
			this.htmlExporter = CreateHtmlExporter();
			this.htmlParts = new Dictionary<string, string>();
			this.imageParts = new Dictionary<string, OfficeImage>();
			this.tableOfContents = new Dictionary<string, string>();
		}
		#region Properties
		protected EPubDocumentExporterOptions Options { get { return options; } }
		protected internal XmlWriter DocumentContentWriter { get { return documentContentWriter; } set { documentContentWriter = value; } }
		#endregion
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			IUriProviderService service = DocumentModel.GetService<IUriProviderService>();
			if (service != null)
				service.RegisterProvider(this);
			try {
				Export();
			}
			finally {
				if (service != null)
					service.UnregisterProvider(this);
			}
		}
		public override void Export() {
			if (outputStream == null)
				throw new InvalidOperationException();
			using (InternalZipArchive documentPackage = new InternalZipArchive(outputStream)) {
				this.package = documentPackage;
				InitializeExport();
				string containerContent =
					@"<?xml version=""1.0"" encoding=""UTF-8""?>" + "\r\n" +
					@"<container xmlns=""urn:oasis:names:tc:opendocument:xmlns:container"" version=""1.0"">" + "\r\n" +
					@"  <rootfiles>" + "\r\n" +
					@"    <rootfile full-path=""OEBPS/document.opf"" media-type=""application/oebps-package+xml""/>" + "\r\n" +
					@"  </rootfiles>" + "\r\n" +
					@"</container>";
				AddPackageContent(@"OEBPS\document.opf", ExportDocumentContent());
				if (tableOfContents.Count > 0)
					AddPackageContent(@"OEBPS\document.ncx", ExportTableOfContents());
				AddPackageContent(@"OEBPS\style.css", styles);
				AddPackageContent(@"META-INF\container.xml", containerContent);
				AddPackageContent(@"mimetype", "application/epub+zip");
			}
		}
		protected internal virtual void InitializeExport() {
			this.now = DateTime.Now;
			this.contentIndex = 0;
			this.imageIndex = 0;
			this.styles = String.Empty;
			this.htmlParts.Clear();
			this.imageParts.Clear();
		}
		protected internal virtual EPubHtmlExporter CreateHtmlExporter() {
			HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
			options.CssPropertiesExportType = CssPropertiesExportType.Link;
			options.Encoding = Encoding.UTF8;
			return new EPubHtmlExporter(DocumentModel, options);
		}
		protected internal virtual Stream CreateXmlContent(Action<XmlWriter> action) {
			using (MemoryStream stream = new MemoryStream()) {
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8)) {
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					settings.Encoding = Encoding.UTF8;
					settings.CheckCharacters = true;
					settings.OmitXmlDeclaration = false;
					using (XmlWriter writer = XmlWriter.Create(streamWriter, settings)) {
						action(writer);
						writer.Flush();
						return new MemoryStream(stream.GetBuffer(), 0, (int)stream.Length);
					}
				}
			}
		}
		protected internal virtual void AddPackageContent(string fileName, Stream content) {
			package.Add(fileName, now, content);
		}
		protected internal virtual void AddPackageContent(string fileName, string content) {
			package.Add(fileName, now, Encoding.UTF8.GetBytes(content));
		}
		#region ExportTableOfContents
		protected internal virtual Stream ExportTableOfContents() {
			return CreateXmlContent(GenerateTableOfContentsXmlContent);
		}
		protected internal virtual void GenerateTableOfContentsXmlContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			GenerateTableOfContentsXmlContent();
		}
		protected internal virtual void GenerateTableOfContentsXmlContent() {
			DocumentContentWriter.WriteStartElement("ncx", "http://www.daisy.org/z3986/2005/ncx/");
			try {
				DocumentContentWriter.WriteAttributeString("version", "2005-1");
				GenerateTocHeader();
				GenerateTocTitle();
				GenerateTocNavigationMap();
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GenerateTocHeader() {
			DocumentContentWriter.WriteStartElement("meta");
			try {
				DocumentContentWriter.WriteAttributeString("name", "dc:title");
				DocumentContentWriter.WriteAttributeString("content", "document");
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
			DocumentContentWriter.WriteStartElement("meta");
			try {
				DocumentContentWriter.WriteAttributeString("name", "dtb:uid");
				DocumentContentWriter.WriteAttributeString("content", Guid.NewGuid().ToString());
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GenerateTocTitle() {
			DocumentContentWriter.WriteStartElement("docTitle");
			try {
				DocumentContentWriter.WriteStartElement("text");
				try {
					DocumentContentWriter.WriteString("Table of contents");
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GenerateTocNavigationMap() {
			DocumentContentWriter.WriteStartElement("navMap");
			try {
				int order = 1;
				foreach (string id in tableOfContents.Keys) {
					GenerateNavigationPoint(id, tableOfContents[id], order);
					order++;
				}
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GenerateNavigationPoint(string id, string text, int index) {
			DocumentContentWriter.WriteStartElement("navPoint");
			try {
				DocumentContentWriter.WriteAttributeString("id", id);
				DocumentContentWriter.WriteAttributeString("playOrder", index.ToString());
				DocumentContentWriter.WriteStartElement("navLabel");
				try {
					DocumentContentWriter.WriteStartElement("text");
					try {
						DocumentContentWriter.WriteString(text);
					}
					finally {
						DocumentContentWriter.WriteEndElement();
					}
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
				DocumentContentWriter.WriteStartElement("content");
				try {
					DocumentContentWriter.WriteAttributeString("src", id + ".html");
				}
				finally {
					DocumentContentWriter.WriteEndElement();
				}
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		#endregion
		#region ExportDocumentContent
		protected internal virtual Stream ExportDocumentContent() {
			return CreateXmlContent(GenerateDocumentXmlContent);
		}
		protected internal virtual void GenerateDocumentXmlContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			GenerateDocumentContent();
		}
		protected internal virtual void GenerateDocumentContent() {
			DocumentContentWriter.WriteStartElement("package", "http://www.idpf.org/2007/opf");
			try {
				DocumentContentWriter.WriteAttributeString("version", "2.0");
				base.Export();
				GeneratePackageContent();
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GeneratePackageContent() {
			GeneratePackageMetadata();
			GeneratePackageManifest();
			GeneratePackageSpine();
		}
		protected internal virtual void GeneratePackageMetadata() {
			DocumentContentWriter.WriteStartElement("metadata");
			try {
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GeneratePackageManifest() {
			DocumentContentWriter.WriteStartElement("manifest");
			try {
				GeneratePackageManifestItem("css0001", "style.css", "text/css");
				if (tableOfContents.Count > 0)
					GeneratePackageManifestItem("ncx", "document.ncx", "text/xml");
				foreach (string id in imageParts.Keys) {
					OfficeImageFormat format = imageParts[id].RawFormat;
					string fileName = id + "." + OfficeImage.GetExtension(format);
					package.Add(@"OEBPS\" + fileName, now, imageParts[id].GetImageBytesSafe(format));
					GeneratePackageManifestItem(id, fileName, OfficeImage.GetContentType(format));
				}
				foreach (string id in htmlParts.Keys)
					GeneratePackageManifestItem(id, htmlParts[id], "application/xhtml+xml");
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GeneratePackageManifestItem(string id, string fileName, string mediaType) {
			DocumentContentWriter.WriteStartElement("item");
			try {
				DocumentContentWriter.WriteAttributeString("id", id);
				DocumentContentWriter.WriteAttributeString("href", fileName);
				DocumentContentWriter.WriteAttributeString("media-type", mediaType);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GeneratePackageSpine() {
			DocumentContentWriter.WriteStartElement("spine");
			try {
				DocumentContentWriter.WriteAttributeString("toc", "ncx");
				foreach (string id in htmlParts.Keys)
					GeneratePackageSpineItem(id);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GeneratePackageSpineItem(string id) {
			DocumentContentWriter.WriteStartElement("itemref");
			try {
				DocumentContentWriter.WriteAttributeString("idref", id);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal override void ExportSection(Section section) {
			string sectionContent = htmlExporter.ExportSection(section);
			string id = String.Format(@"content_{0}", contentIndex);
			string fileName = id + ".html";
			htmlParts.Add(id, fileName);
			AddPackageContent(@"OEBPS\" + fileName, sectionContent);
			contentIndex++;
			Bookmark bookmark = LookupBookmarkAtSectionStart(section);
			if (bookmark != null)
				tableOfContents.Add(id, bookmark.Name);
		}
		protected internal virtual Bookmark LookupBookmarkAtSectionStart(Section section) {
			DocumentLogPosition pos = PieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition;
			BookmarkCollection bookmarks = PieceTable.Bookmarks;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = bookmarks[i];
				if (bookmark.NormalizedStart == pos && bookmark.Length == 0)
					return bookmark;
			}
			return null;
		}
		#endregion
		#region IUriProvider Members
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			this.styles = styleText;
			return "style.css";
		}
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			string id = String.Format("image{0}", imageIndex);
			imageParts.Add(id, image);
			imageIndex++;
			return id + "." + OfficeImage.GetExtension(image.RawFormat);
		}
		#endregion
	}
	#endregion
	#region EPubHtmlExporter
	public class EPubHtmlExporter : HtmlExporter {
		public EPubHtmlExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options)
			: base(documentModel, options) {
			options.DisposeConvertedImagesImmediately = false;
		}
		protected internal override HtmlContentExporter CreateContentExporter(DocumentModel documentModel, HtmlDocumentExporterOptions options) {
			return new EPubSectionHtmlContentExporter(documentModel, ScriptContainer, new ServiceBasedImageRepository(documentModel, FilesPath, RelativeUri), options);
		}
		public string ExportSection(Section section) {
			EPubSectionHtmlContentExporter contentExporter = (EPubSectionHtmlContentExporter)ContentExporter;
			contentExporter.CurrentSection = section;
			return Export();
		}
		protected override void WriteHtmlDocumentPreamble(DXHtmlTextWriter writer) {
			writer.Write(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
		}
	}
	#endregion
	#region EPubSectionHtmlContentExporter
	public class EPubSectionHtmlContentExporter : HtmlContentExporter {
		Section currentSection;
		public EPubSectionHtmlContentExporter(DocumentModel documentModel, IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlNumberingListExportFormat numberingListExportFormat, bool isExportInlineStyle)
			: base(documentModel, scriptContainer, imageRepository, numberingListExportFormat, isExportInlineStyle) {
		}
		public EPubSectionHtmlContentExporter(DocumentModel documentModel, IScriptContainer scriptContainer, IOfficeImageRepository imageRepository, HtmlDocumentExporterOptions options)
			: base(documentModel, scriptContainer, imageRepository, options) {
		}
		public Section CurrentSection { get { return currentSection; } set { currentSection = value; } }
		public override void Export() {
			PieceTableNumberingListCounters.BeginCalculateCounters();
			try {
				this.ExportSection(CurrentSection);
			}
			finally {
				PieceTableNumberingListCounters.EndCalculateCounters();
			}
		}
	}
	#endregion
}
