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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#else
using System.IO.Compression;
#endif
namespace DevExpress.XtraRichEdit.Export.OpenXml {
	#region OpenXmlExporter
	public class OpenXmlExporter : WordProcessingMLBaseExporter, IOpenXmlExporter {
		#region Constants
		public const string WordProcessingNamespaceConst = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
		public const string WordProcessingDrawingNamespaceConst = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
		public const string WordProcessingDrawing14NamespaceConst = "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
		public const string DrawingMLPrefix = "a";
		public const string DrawingMLNamespace = "http://schemas.openxmlformats.org/drawingml/2006/main";
		public const string DrawingMLPicturePrefix = "pic";
		public const string DrawingMLPictureNamespace = "http://schemas.openxmlformats.org/drawingml/2006/picture";
		public const string RelsPrefix = "r";
		public const string RelsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
		public const string OfficeDocumentType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
		public const string OfficeStylesType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
		public const string OfficeWebSettingsType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/webSettings";
		public const string OfficeNumberingType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/numbering";
		public const string OfficeDocumentSettings = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings";
		public const string OfficeHyperlinkType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
		public const string OfficeFootNoteType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes";
		public const string OfficeEndNoteType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes";
		public const string OfficeCommentType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
		public const string OfficeCommentsExtendedType = "http://schemas.microsoft.com/office/2011/relationships/commentsExtended";
		public const string OfficeThemesType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme";
		public const string PackageRelsNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";
		public const string RelsImage = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
		public const string RelsHeader = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header";
		public const string RelsFooter = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer";
		public const string RelsFootNote = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes";
		public const string RelsEndNote = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes";
		public const string RelsComment = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/comments";
		public const string RelsCommentsExtended = "http://schemas.microsoft.com/office/2011/relationships/commentsExtended";
		public const string MCPrefix = "mc";
		public const string MCNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		public const string WpsPrefix = "wps";
		public const string WpsNamespace = "http://schemas.microsoft.com/office/word/2010/wordprocessingShape";
		public const string Wp14Prefix = "wp14";
		public const string Wp14Namespace = "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing";
		const int maxBookmarkNameLength = 40;
		const int maxStyleNameLength = 253;
		const int maxFontNameLength = 31;
		#endregion
		#region Fields
		Stream outputStream;
		InternalZipArchive package;
		Stack<Dictionary<string, string>> imageRelationsTableStack;
		Stack<Dictionary<OfficeImage, string>> exportedImageTableStack;
		Dictionary<string, string> hyperlinkRelationsTable;
		Dictionary<string, string> headerRelationsTable;
		Dictionary<string, string> footerRelationsTable;
		Dictionary<string, string> footNoteRelationsTable;
		Dictionary<string, string> endNoteRelationsTable;
		Dictionary<string, string> commentRelationsTable;
		Dictionary<string, string> usedContentTypes;
		Dictionary<string, string> overriddenContentTypes;
		Dictionary<string, string> commentsExtendedRelationsTable;
		List<FootNote> footNotes;
		List<EndNote> endNotes;
		List<Comment> comments;
		HashSet<string> bookmarkNames;
		HashSet<string> styleNames;
		string documentRelationId;
		int commentParaId;
		int commentParagraphIndex;
		int commentParagraphCount;
		int headerCounter;
		int footerCounter;
		List<string> ignorableNamespaces;
		#endregion
		public OpenXmlExporter(DocumentModel documentModel, OpenXmlDocumentExporterOptions options)
			: base(documentModel, options) {
		}
		#region Properties
		public new OpenXmlDocumentExporterOptions Options { get { return (OpenXmlDocumentExporterOptions)base.Options; } }
		protected internal virtual Dictionary<string, string> UsedContentTypes { get { return usedContentTypes; } }
		protected internal virtual Dictionary<string, string> OverriddenContentTypes { get { return overriddenContentTypes; } }
		protected internal Stack<Dictionary<string, string>> ImageRelationsTableStack { get { return imageRelationsTableStack; } }
		protected internal Stack<Dictionary<OfficeImage, string>> ExportedImageTableStack { get { return exportedImageTableStack; } }
		protected internal Dictionary<string, string> ImageRelationsTable { get { return imageRelationsTableStack.Peek(); } }
		protected internal override Dictionary<OfficeImage, string> ExportedImageTable { get { return exportedImageTableStack.Peek(); } }
		protected internal Dictionary<string, string> HeaderRelationsTable { get { return headerRelationsTable; } }
		protected internal Dictionary<string, string> FooterRelationsTable { get { return footerRelationsTable; } }
		protected internal Dictionary<string, string> FootNoteRelationsTable { get { return footNoteRelationsTable; } }
		protected internal Dictionary<string, string> EndNoteRelationsTable { get { return endNoteRelationsTable; } }
		protected internal Dictionary<string, string> CommentRelationsTable { get { return commentRelationsTable; } }
		protected internal Dictionary<string, string> CommentsExtendedRelationsTable { get { return commentsExtendedRelationsTable; } }
		protected internal Stream OutputStream { get { return outputStream; } }
		protected internal string DocumentRelationId { get { return documentRelationId; } }
		protected internal override InternalZipArchive Package { get { return package; } }
		protected internal override bool ShouldExportHiddenText { get { return true; } } 
		protected override string WordProcessingNamespace { get { return WordProcessingNamespaceConst; } }
		protected override string WordProcessingPrefix { get { return "w"; } }
		protected internal static string WordProcessingDrawingNamespace { get { return WordProcessingDrawingNamespaceConst; } }
		protected internal static string WordProcessingDrawing14Namespace { get { return WordProcessingDrawing14NamespaceConst; } }
		protected internal static string WordProcessingDrawingPrefix { get { return "wp"; } }
		protected internal static string WordProcessingDrawingPrefix14 { get { return "wp14"; } }
		protected internal int PictureId { get { return DrawingElementId; } }
		protected override Dictionary<FloatingObjectHorizontalPositionType, WordProcessingMLValue> HorizontalPositionTypeAttributeTable { get { return DevExpress.XtraRichEdit.Import.OpenXml.InlineObjectDestination.OpenXmlHorizontalPositionTypeAttributeTable; } }
		protected override Dictionary<FloatingObjectVerticalPositionType, WordProcessingMLValue> VerticalPositionTypeAttributeTable { get { return DevExpress.XtraRichEdit.Import.OpenXml.InlineObjectDestination.OpenXmlVerticalPositionTypeAttributeTable; } }
		#endregion
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			Export();
		}
		public override void Export() {
			if (outputStream == null)
				throw new InvalidOperationException();
			using (InternalZipArchive documentPackage = new InternalZipArchive(outputStream)) {
				this.package = documentPackage;
				InitializeExport();
				AddCompressedPackages();
			}
		}
		protected internal virtual void AddCompressedPackages() {
			AddCompressedPackageContent(@"word\document.xml", ExportDocumentContent());
			AddCompressedPackageContent(@"word\styles.xml", ExportStyles());
			AddCompressedPackageContent(@"word\numbering.xml", ExportNumbering());
			AddCompressedPackageContent(@"word\settings.xml", ExportSettings());
			ExportFootNotesAndEndNotes();
			ExportComments();
			ExportWebSettings();
			AddCompressedPackageContent(@"_rels\.rels", ExportPackageRelations());
			AddCompressedPackageContent(@"[Content_Types].xml", ExportContentTypes());
			AddCompressedPackageContent(@"word\_rels\document.xml.rels", ExportDocumentRelations());		 
		}
		protected internal void SetPackage(InternalZipArchive package) {
			this.package = package;
		}
		protected internal virtual void ExportFootNotesAndEndNotes() {
			PopulateExportedFootNotes();
			PopulateExportedEndNotes();
			if (footNotes.Count > 0 && DocumentModel.DocumentCapabilities.FootNotesAllowed) {
				AddCompressedPackageContent(@"word\footnotes.xml", ExportFootNotes());
				RegisterContentTypeOverride(@"/word/footnotes.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml");
				FootNoteRelationsTable.Add("RelFnt1", "footnotes.xml");
			}
			if (endNotes.Count > 0 && DocumentModel.DocumentCapabilities.EndNotesAllowed) {
				AddCompressedPackageContent(@"word\endnotes.xml", ExportEndNotes());
				RegisterContentTypeOverride(@"/word/endnotes.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml");
				EndNoteRelationsTable.Add("RelEnt1", "endnotes.xml");
			}
		}
		protected internal virtual void ExportTheme() {
			AddCompressedPackageContent(@"word\theme\theme1.xml", ExportThemeContent());
			RegisterContentTypeOverride(@"/word/theme/theme1.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.theme1+xml");
		}
		protected internal virtual void ExportComments() {
			ImageRelationsTableStack.Push(new Dictionary<string, string>());
			ExportedImageTableStack.Push(new Dictionary<OfficeImage, string>());
			try {
				PopulateExportedComments();
				if (comments.Count > 0) {
					AddCompressedPackageContent(@"word\comments.xml", ExportStreamComments());
					RegisterContentTypeOverride(@"/word/comments.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.comments+xml");
					if (ImageRelationsTable.Count > 0) {
						string relsFileName = String.Format(@"word\_rels\comments.xml.rels");
						AddCompressedPackageContent(relsFileName, ExportCommentRelations());
					}
					CommentRelationsTable.Add("RelCmt1", "comments.xml");
					AddCompressedPackageContent(@"word\commentsExtended.xml", ExportCommentsExtended());
					RegisterContentTypeOverride(@"/word/commentsExtended.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.commentsExtended+xml");
					CommentsExtendedRelationsTable.Add("RelCmtEx1", "commentsExtended.xml");
				}
			}
			finally {
				ExportedImageTableStack.Pop();
				ImageRelationsTableStack.Pop();
			}
		}
		protected internal virtual void ExportWebSettings() {
			WebSettings webSettings = DocumentModel.WebSettings;
			if (!webSettings.IsBodyMarginsSet())
				return;
			AddCompressedPackageContent(@"word\webSettings.xml", CreateWebSettingsContent());
			RegisterContentTypeOverride(@"/word/webSettings.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.webSettings+xml");
		}
		protected internal virtual CompressedStream CreateWebSettingsContent() {
			return CreateCompressedXmlContent(GenerateWebSettingsContent);
		}
		protected internal virtual void GenerateWebSettingsContent(XmlWriter xmlWriter) {
			DocumentContentWriter = xmlWriter;
			WriteWpStartElement("webSettings");
			WriteWpStartElement("divs");
			try {
				WebSettings webSettings = DocumentModel.WebSettings;
				WriteWpStartElement("div");
				try {
					WriteWpIntAttr("id", webSettings.GetHashCode());
					WriteWpBoolValue("bodyDiv", true);
					WriteWpIntValue("marLeft", UnitConverter.ModelUnitsToTwips(webSettings.LeftMargin));
					WriteWpIntValue("marRight", UnitConverter.ModelUnitsToTwips(webSettings.RightMargin));
					WriteWpIntValue("marTop", UnitConverter.ModelUnitsToTwips(webSettings.TopMargin));
					WriteWpIntValue("marBottom", UnitConverter.ModelUnitsToTwips(webSettings.BottomMargin));
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
				WriteWpEndElement();
			}
		}
		protected internal virtual void InitializeExport() {
			this.ImageCounter = 0;
			this.headerCounter = 0;
			this.footerCounter = 0;
			this.imageRelationsTableStack = new Stack<Dictionary<string,string>>();
			this.imageRelationsTableStack.Push(new Dictionary<string, string>());
			this.exportedImageTableStack = new Stack<Dictionary<OfficeImage, string>>();
			this.exportedImageTableStack.Push(new Dictionary<OfficeImage, string>());
			this.headerRelationsTable = new Dictionary<string, string>();
			this.footerRelationsTable = new Dictionary<string, string>();
			this.footNoteRelationsTable = new Dictionary<string, string>();
			this.endNoteRelationsTable = new Dictionary<string, string>();
			this.commentRelationsTable = new Dictionary<string, string>();
			this.commentsExtendedRelationsTable = new Dictionary<string, string>();
			this.hyperlinkRelationsTable = new Dictionary<string, string>();
			this.usedContentTypes = new Dictionary<string, string>();
			this.overriddenContentTypes = new Dictionary<string, string>();
			this.footNotes = new List<FootNote>();
			this.endNotes = new List<EndNote>();
			this.comments = new List<Comment>();
			this.bookmarkNames = new HashSet<string>();
			this.styleNames = new HashSet<string>();
			this.ignorableNamespaces = new List<string>();
			this.documentRelationId = CalcDocumentRelationId();
			usedContentTypes.Add("rels", "application/vnd.openxmlformats-package.relationships+xml");
			RegisterContentTypeOverride("/word/document.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml");
		}
		protected internal virtual string CalcDocumentRelationId(){
			return String.Format("R{0:X}", new Random((int)DateTime.Now.Ticks).Next());
		}
		protected internal virtual CompressedStream ExportDocumentContent() {
			return CreateCompressedXmlContent(GenerateDocumentXmlContent);
		}
		protected internal virtual CompressedStream ExportPackageRelations() {
			return CreateCompressedXmlContent(GeneratePackageRelationsContent);
		}
		protected internal virtual CompressedStream ExportDocumentRelations() {
			return CreateCompressedXmlContent(GenerateDocumentRelationsContent);
		}
		protected internal virtual CompressedStream ExportHeaderFooterRelations() {
			return CreateCompressedXmlContent(GenerateHeaderFooterRelationsContent);
		}
		protected internal virtual CompressedStream ExportCommentRelations() {
			return CreateCompressedXmlContent(GenerateCommentRelationsContent);
		}
		protected internal virtual CompressedStream ExportContentTypes() {
			return CreateCompressedXmlContent(GenerateContentTypesContent);
		}
		protected internal virtual CompressedStream ExportStyles() {
			return CreateCompressedXmlContent(GenerateStylesContent);
		}
		protected internal virtual CompressedStream ExportNumbering() {
			return CreateCompressedXmlContent(GenerateNumberingContent);
		}
		protected internal virtual CompressedStream ExportSettings() {
			return CreateCompressedXmlContent(GenerateSettingsContent);
		}
		protected internal virtual void GenerateDocumentRelationsContent(XmlWriter writer) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			GenerateDocumentRelations(writer);
			writer.WriteStartElement("Relationship");
			writer.WriteAttributeString("Id", "RelStyle1");
			writer.WriteAttributeString("Type", OfficeStylesType);
			writer.WriteAttributeString("Target", "styles.xml");
			writer.WriteEndElement();
			if (DocumentModel.WebSettings.IsBodyMarginsSet()) {
				writer.WriteStartElement("Relationship");
				writer.WriteAttributeString("Id", "RelWebSettings1");
				writer.WriteAttributeString("Type", OfficeWebSettingsType);
				writer.WriteAttributeString("Target", "webSettings.xml");
				writer.WriteEndElement();
			}
			writer.WriteStartElement("Relationship");
			writer.WriteAttributeString("Id", "RelNum1");
			writer.WriteAttributeString("Type", OfficeNumberingType);
			writer.WriteAttributeString("Target", "numbering.xml");
			writer.WriteEndElement();
			writer.WriteStartElement("Relationship");
			writer.WriteAttributeString("Id", "RelSettings1");
			writer.WriteAttributeString("Type", OfficeDocumentSettings);
			writer.WriteAttributeString("Target", "settings.xml");
			writer.WriteEndElement();
			foreach (string relationId in hyperlinkRelationsTable.Keys) {
				writer.WriteStartElement("Relationship");
				writer.WriteAttributeString("Id", relationId);
				writer.WriteAttributeString("Type", OfficeHyperlinkType);
				writer.WriteAttributeString("Target", hyperlinkRelationsTable[relationId]);
				writer.WriteAttributeString("TargetMode", "External");
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		protected internal virtual void GenerateDocumentRelations(XmlWriter writer) {
			GenerateFileRelationsCore(writer, ImageRelationsTable, RelsImage);
			GenerateFileRelationsCore(writer, headerRelationsTable, RelsHeader);
			GenerateFileRelationsCore(writer, footerRelationsTable, RelsFooter);
			GenerateFileRelationsCore(writer, footNoteRelationsTable, RelsFootNote);
			GenerateFileRelationsCore(writer, endNoteRelationsTable, RelsEndNote);
			GenerateFileRelationsCore(writer, commentsExtendedRelationsTable, RelsCommentsExtended);
			GenerateFileRelationsCore(writer, commentRelationsTable, RelsComment);
		}
		protected internal virtual void GenerateHeaderFooterRelationsContent(XmlWriter writer) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			GenerateFileRelationsCore(writer, ImageRelationsTable, RelsImage);
			writer.WriteEndElement();
		}
		protected internal virtual void GenerateCommentRelationsContent(XmlWriter writer) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			GenerateFileRelationsCore(writer, ImageRelationsTable, RelsImage);
			writer.WriteEndElement();
		}
		protected internal void GenerateFileRelationsCore(XmlWriter writer, Dictionary<string, string> relationTable, string relationType) {
			foreach (string relationId in relationTable.Keys) {
				string path = relationTable[relationId];
				writer.WriteStartElement("Relationship");
				writer.WriteAttributeString("Id", relationId);
				writer.WriteAttributeString("Type", relationType);
				writer.WriteAttributeString("Target", path);
				writer.WriteEndElement();
			}
		}
		protected internal virtual void GenerateDocumentXmlContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			GenerateDocumentContent();
		}
		protected internal virtual void GeneratePackageRelationsContent(XmlWriter writer) {
			writer.WriteStartElement("Relationships", PackageRelsNamespace);
			writer.WriteStartElement("Relationship");
			writer.WriteAttributeString("Id", DocumentRelationId);
			writer.WriteAttributeString("Type", OfficeDocumentType);
			writer.WriteAttributeString("Target", "/word/document.xml");
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
		protected internal virtual void GenerateContentTypesContent(XmlWriter writer) {
			RegisterContentTypeOverride("/word/styles.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml");
			RegisterContentTypeOverride("/word/settings.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml");
			RegisterContentTypeOverride("/word/numbering.xml", "application/vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml");
			writer.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
			foreach (string extension in usedContentTypes.Keys) {
				string contentType = usedContentTypes[extension];
				writer.WriteStartElement("Default");
				writer.WriteAttributeString("Extension", extension);
				writer.WriteAttributeString("ContentType", contentType);
				writer.WriteEndElement();
			}
			foreach (string partName in overriddenContentTypes.Keys) {
				writer.WriteStartElement("Override");
				writer.WriteAttributeString("PartName", partName);
				writer.WriteAttributeString("ContentType", overriddenContentTypes[partName]);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		protected internal virtual void RegisterContentTypeOverride(string partName, string contentType) {
			overriddenContentTypes.Add(partName, contentType);
		}
		protected internal virtual void GenerateDocumentContent() {
			WriteWpStartElement("document");
			try {
				RegisterNamespaces();
				ExportDocumentBackground();
				ExportDocumentVersion();
				WriteWpStartElement("body");
				try {
					base.Export();
					ExportSectionProperties(CurrentSection); 
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		void ClearIgnorableNamespaces() {
			this.ignorableNamespaces.Clear();
		}
		protected internal virtual void RegisterNamespaces() {
			ClearIgnorableNamespaces();
			RegisterDefaultNamespaces();
			RegisterIgnorableNamespaces();
		}
		void RegisterDefaultNamespaces() {
			RegisterNamespace(WordProcessingPrefix, WordProcessingNamespace, false);
			RegisterNamespace(WpsPrefix, WpsNamespace, false);
			RegisterNamespace(MCPrefix, MCNamespace, false);
			RegisterNamespace(W10MLPrefix, W10MLNamespace, false);
			RegisterNamespace(VMLPrefix, VMLNamespace, false);
			RegisterNamespace(WordProcessingDrawingPrefix14, WordProcessingDrawing14Namespace, true);
		}
		void RegisterNamespace(string prefix, string ns, bool ignorable) {
			DocumentContentWriter.WriteAttributeString("xmlns", prefix, null, ns);
			if (ignorable)
				this.ignorableNamespaces.Add(prefix);
		}
		protected internal virtual void RegisterWW14W15Namespace() {
			RegisterNamespace(W14Prefix, W14NamespaceConst, true);
			RegisterNamespace(W15Prefix, W15NamespaceConst, true);
		}
		protected internal virtual void RegisterMCIgnorable() {
			RegisterIgnorableNamespaces(W14Prefix, W15Prefix);
		}
		void RegisterIgnorableNamespaces(params string[] prefixes) {
			DocumentContentWriter.WriteAttributeString("mc", "Ignorable", null, String.Join<string>(" ", prefixes));
		}
		void RegisterIgnorableNamespaces() {
			if (this.ignorableNamespaces.Count > 0)
				RegisterIgnorableNamespaces(this.ignorableNamespaces.ToArray());
			this.ignorableNamespaces.Clear();
		}
		protected internal virtual void ExportDocumentBackground() {
			Color pageBackColor = DocumentModel.DocumentProperties.PageBackColor;
			if ((pageBackColor == DXColor.Empty))
				return;
			WriteWpStartElement("background");
			try {
					WriteWpStringAttr("color", ConvertColorToString(pageBackColor));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportDocumentVersion() {
		}
		#region Paragraph
		protected internal override void ExportParagraphListReference(Paragraph paragraph) {
			WriteWpStartElement("numPr");
			try {
				WriteWpIntValue("ilvl", paragraph.GetListLevelIndex());
				WriteWpIntValue("numId", GetNumberingListIndexForExport(paragraph.GetOwnNumberingListIndex()));
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region Run
		#endregion
		#region Picture
		protected override void ExportImageReference(InlinePictureRun run) {
			if (run.PieceTable.IsComment)
				ExportImageAsDrawing(run);
			else
				ExportImageReferenceCore(run, "pict");
		}
		protected void ExportImageReferenceCore(InlinePictureRun run, string startElement) {
			WriteWpStartElement(startElement);
			try {
				DocumentContentWriter.WriteStartElement(VMLPrefix, "shape", VMLNamespace);
				try {
					float finalWidth = UnitConverter.ModelUnitsToPointsF(run.ActualSizeF.Width);
					float finalHeight = UnitConverter.ModelUnitsToPointsF(run.ActualSizeF.Height);
					string imageStyle = String.Format(CultureInfo.InvariantCulture, "width:{0}pt;height:{1}pt", finalWidth, finalHeight);
					DocumentContentWriter.WriteAttributeString("style", imageStyle);
					ExportImageData(run);
				} finally {
					DocumentContentWriter.WriteEndElement();
				}
			} finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual string ExportImageData(OfficeImage image) {
			string imageRelationId;
			OfficeImage rootImage = image.RootImage;
			if (ExportedImageTable.TryGetValue(rootImage, out imageRelationId))
				return imageRelationId;
			string imageId = GenerateImageId();
			string imagePath = ExportImageData(rootImage, imageId);
			imageRelationId = GenerateImageRelationId(imageId);
			ImageRelationsTable.Add(imageRelationId, imagePath);
			ExportedImageTable.Add(rootImage, imageRelationId);
			return imageRelationId;
		}
		protected internal virtual void ExportImageData(InlinePictureRun run) {
			string imageRelationId = ExportImageData(run.Image);
			ExportImageReference(imageRelationId);
		}
		protected internal virtual string ExportImageData(OfficeImage image, string imageId) {
			string extension = GetImageExtension(image);
			string contentType;
			if (!usedContentTypes.TryGetValue(extension, out contentType)) {
				contentType = OfficeImage.GetContentType(image.RawFormat);
				if (String.IsNullOrEmpty(contentType)) {
					contentType = OfficeImage.GetContentType(OfficeImageFormat.Png);
				}
				usedContentTypes.Add(extension, contentType);
			}
			AddPackageImage(GetImageFileName(imageId, extension), image);
			return GetImagePath(imageId, extension);
		}
		string GetImageFileName(string imageId, string extension) {
			if (DocumentModel.DocumentExportOptions.OpenXml.AlternateImageFolder)
				return @"word\media\" + imageId + "." + extension;
			return @"media\" + imageId + "." + extension;
		}
		string GetImagePath(string imageId, string extension) {
			if (DocumentModel.DocumentExportOptions.OpenXml.AlternateImageFolder)
				return "/word/media/" + imageId + "." + extension;
			return "/media/" + imageId + "." + extension;
		}
		protected internal virtual void ExportImageReference(string imageRelationId) {
			DocumentContentWriter.WriteStartElement(VMLPrefix, "imagedata", VMLNamespace);
			try {
				DocumentContentWriter.WriteAttributeString(RelsPrefix, "id", RelsNamespace, imageRelationId);
				DocumentContentWriter.WriteAttributeString(OfficePrefix, "title", OfficeNamespace, String.Empty);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		#endregion
		#region Styles
		protected internal virtual void GenerateStylesContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			ExportStylesCore();
		}
		protected internal override void ExportDocumentDefaults() {
			WriteWpStartElement("docDefaults");
			try {
				ExportDocumentCharacterDefaults();
				ExportDocumentParagraphDefaults();
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportDocumentCharacterDefaults() {
			WriteWpStartElement("rPrDefault");
			try {
				WriteWpStartElement("rPr");
				try {
					ExportRunPropertiesCore(DocumentModel.DefaultCharacterProperties);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportDocumentParagraphDefaults() {
			WriteWpStartElement("pPrDefault");
			try {
				WriteWpStartElement("pPr");
				try {
					ExportParagraphPropertiesCore(DocumentModel.DefaultParagraphProperties, true, null, null, null);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportParagraphStyleListReference(NumberingListIndex numberingListIndex, int listLevelIndex) {
			if (!HasNumberingProperties(numberingListIndex, listLevelIndex))
				return;
			WriteWpStartElement("numPr");
			try {
				if (listLevelIndex > 0)
					WriteWpIntValue("ilvl", listLevelIndex);
				if (numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList)
					WriteWpIntValue("numId", GetNumberingListIndexForExport(numberingListIndex));
			}
			finally {
				WriteWpEndElement();
			}
		}
		bool HasNumberingProperties(NumberingListIndex numberingListIndex, int listLevelIndex) {
			return numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList || listLevelIndex > 0;
		}
		#endregion
		#region Numbering
		protected internal virtual void GenerateNumberingContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportNumberingCore();
		}
		protected internal override void ExportAbstractNumberingList(AbstractNumberingList list, int id) {
			WriteWpStartElement("abstractNum");
			try {
				WriteWpIntAttr("abstractNumId", id);
				WriteWpStringValue("nsid", ConvertToHexString(list.Id).PadLeft(8, '0'));
				WriteWpStringValue("multiLevelType", ConvertNumberingListType(NumberingListHelper.GetListType(list)));
				int styleLinkIndex = list.StyleLinkIndex;
				int numberingStyleReferenceIndex = list.NumberingStyleReferenceIndex;
				bool shouldExportLevels = styleLinkIndex >= 0 || numberingStyleReferenceIndex < 0;
				if (styleLinkIndex >= 0) {
					WriteWpStringValue("tmpl", ConvertToHexString(list.TemplateCode).PadLeft(8, '0'));
					WriteWpStringValue("styleLink", GetNumberingStyleId(styleLinkIndex));
				}
				else if(numberingStyleReferenceIndex >= 0) {
					WriteWpStringValue("tmpl", ConvertToHexString(list.TemplateCode).PadLeft(8, '0'));
					string styleId = GetNumberingStyleId(numberingStyleReferenceIndex);
					WriteWpStringValue("numStyleLink", styleId);
				}
				if(shouldExportLevels)
					ExportLevels(list.Levels);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportNumberingList(NumberingList list, int id) {
			WriteWpStartElement("num");
			try {
				WriteWpIntAttr("numId", id + 1);
				WriteWpIntValue("abstractNumId", ((IConvertToInt<AbstractNumberingListIndex>)list.AbstractNumberingListIndex).ToInt());
				ExportOverrideLevels(list.Levels);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportNumberFormatValue(ListLevelProperties properties) {
			WriteWpStringValue("numFmt", ConvertNumberFormat(properties.Format));
		}
		#endregion
		#region Settings
		protected internal virtual void GenerateSettingsContent(XmlWriter writer) {
			this.DocumentContentWriter = writer;
			ExportSettingsCore();
		}
		protected internal virtual void ExportSettingsCore() {
			WordProcessingMLValue val = new WordProcessingMLValue("settings", "docPr");
			WriteWpStartElement(GetWordProcessingMLValue(val));
			try {
				WriteSettingsCore();
			} finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportDocumentProtectionSettingsCore() {
			DocumentProtectionProperties properties = DocumentModel.ProtectionProperties;
			if (properties.PasswordHash == null || properties.PasswordHash.Length <= 0)
				return;
			WriteWpStringAttr("cryptProviderType", "rsaFull");
			WriteWpStringAttr("cryptAlgorithmClass", "hash");
			WriteWpStringAttr("cryptAlgorithmType", "typeAny");
			WriteWpIntAttr("cryptAlgorithmSid", (int)properties.HashAlgorithmType);
			WriteWpStringAttr("cryptSpinCount", Math.Max(1, properties.HashIterationCount).ToString());
			if (properties.PasswordHash != null)
				WriteWpStringAttr("hash", Convert.ToBase64String(properties.PasswordHash));
			if (properties.PasswordPrefix != null)
				WriteWpStringAttr("salt", Convert.ToBase64String(properties.PasswordPrefix));
		}
		#endregion
		#region Bookmarks
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			WriteWpStartElement("bookmarkStart");
			try {
				WriteWpStringAttr("id", GenerateBookmarkId(bookmark));
				WriteWpStringAttr("name", PrepareBookmarkName(bookmark.Name));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected override string PrepareFontName(string name) {
			if (!DocumentModel.DocumentExportOptions.OpenXml.LimitFontNameTo31Chars || name.Length <= maxFontNameLength)
				return name;
			return name.Substring(0, maxFontNameLength);
		}
		string PrepareBookmarkName(string name) {
			if (!DocumentModel.DocumentExportOptions.OpenXml.LimitBookmarkNameTo40Chars || name.Length <= maxBookmarkNameLength)
				return name;
			return CutNameToPermittedLength(name, this.bookmarkNames, maxBookmarkNameLength);
		}
		string PrepareStyleName(string name) {
			if (!DocumentModel.DocumentExportOptions.OpenXml.LimitStyleNameTo253Chars || name.Length <= maxStyleNameLength)
				return name;
			return CutNameToPermittedLength(name, this.styleNames, maxStyleNameLength);
		}
		string CutNameToPermittedLength(string name, HashSet<string> namesHashSet, int maxLength) {
			name = name.Substring(0, maxLength);
			int index = 2;
			while (namesHashSet.Contains(name)) {
				name = name.Substring(0, maxLength - index.ToString().Length) + index;
				index++;
			}
			namesHashSet.Add(name);
			return name;
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			WriteWpStartElement("bookmarkEnd");
			try {
				WriteWpStringAttr("id", GenerateBookmarkId(bookmark));
			}
			finally {
				WriteWpEndElement();
			}
		}
		string GenerateBookmarkId(Bookmark bookmark) {
			return PieceTable.Bookmarks.IndexOf(bookmark).ToString(); 
		}
		#endregion
		#region Comments
		protected internal override void ExportCommentStart(Comment comment) {
			if (comment.Start == comment.End) 
				return;
			WriteWpStartElement("commentRangeStart");
			try {
				WriteWpStringAttr("id", Convert.ToString(comment.Index));
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal override void ExportCommentEnd(Comment comment) {
			if (comment.Start != comment.End) {
				WriteWpStartElement("commentRangeEnd");
				try {
					WriteWpStringAttr("id", Convert.ToString(comment.Index));
				}
				finally {
					WriteWpEndElement();
				}
			}
			ExportCommentReference(comment);
		}
		protected internal virtual void ExportCommentReference(Comment comment) {
			WriteWpStartElement("r");
			WriteWpStartElement("commentReference");
			try {
				WriteWpStringAttr("id", Convert.ToString(comment.Index));
			}
			finally {
				WriteWpEndElement();
			}
			WriteWpEndElement();
		}
		protected internal virtual void PopulateExportedComments() {
			CommentCollection comments = DocumentModel.MainPieceTable.Comments;
			int count = comments.Count;
			for (int i = 0; i < count; i++)
				if (comments[i].Content.IsReferenced)
					this.comments.Add(comments[i]);
		}
		protected internal virtual CompressedStream ExportThemeContent() {
			return CreateCompressedXmlContent(GenerateThemeContent);
		}
		protected internal virtual CompressedStream ExportStreamComments() {
			return CreateCompressedXmlContent(GenerateCommentsContent);
		}
		protected internal virtual CompressedStream ExportCommentsExtended(){
			return CreateCompressedXmlContent(GenerateCommentsExtendedContent);
		}
		protected internal virtual void GenerateThemeContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportThemeCore();
		}
		protected internal virtual void GenerateCommentsContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportCommentsCore();
		}
		protected internal virtual void GenerateCommentsExtendedContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportCommentsExtendedCore();
		}
		protected internal virtual void ExportThemeCore() {
			WriteADrawingStartElement("theme");
			try {
				ExportThemeElements();
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		protected internal virtual void ExportThemeElements() {
			WriteADrawingStartElement("themeElements");
			try {
				ExportColorSheme();
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		protected internal virtual void ExportColorSheme() {
			WriteADrawingStartElement("clrScheme");
			try {
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		protected internal virtual void ExportSRGBColor(string attribute, Color color) {
			WriteADrawingStartElement("srgbClr");
			try {
				WriteStringValue("val", ConvertColorToString(color));
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		protected internal virtual void ExportSysColor(string attribute, Color color, Color lastColor) {
			WriteADrawingStartElement(attribute);
			try {
				ExportSysColorCore(color, lastColor);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		protected internal virtual void ExportSysColorCore(Color color, Color lastColor) {
			WriteADrawingStartElement("sysClr");
			try {
				ExportSystemColorAttributes(color, lastColor); 
			}
			finally {
				WriteADrawingEndElement();
			}
		}
			void ExportSystemColorAttributes(Color color, Color lastColor) {
					WriteStringValue("val", color.Name);
					WriteStringValue("lastClr", ConvertColorToString(lastColor));
			}																				 
		protected internal virtual void ExportCommentsExtendedCore() {
			WriteW15StartElement("commentsEx");
			try {
				RegisterCommentsNamespaces();
				int count = comments.Count;
				for (int i = 0; i < count; i++)
					ExportCommentsExtendedCore(DocumentContentWriter, comments[i]);
			}
			finally {
				WriteW15EndElement();
			}
		}
		void RegisterCommentsNamespaces() {
			ClearIgnorableNamespaces();
			RegisterDefaultNamespaces();
			RegisterWW14W15Namespace();
			RegisterIgnorableNamespaces();
		}
		protected internal virtual void ExportCommentsExtendedCore(XmlWriter writer, Comment comment) {
			string tagName = "commentEx"; 
			XmlWriter oldWriter = this.DocumentContentWriter;
			try {
				this.DocumentContentWriter = writer;
				WriteW15StartElement(tagName);
				try {
					WriteW15StringAttr("paraId", ConvertToHexString(comment.Index + 1).PadLeft(8, '0'));
					if (comment.ParentComment != null)
						WriteW15StringAttr("paraIdParent", ConvertToHexString(comment.ParentComment.Index + 1).PadLeft(8, '0'));
					WriteW15StringAttr("done", "0");
				}
				finally {
					WriteW15EndElement();
				}
			}
			finally {
				this.DocumentContentWriter = oldWriter;
			}
		}
		protected internal virtual void ExportCommentsCore() {
			WriteWpStartElement("comments");
			try {
				RegisterCommentsNamespaces();
				int count = comments.Count;
				for (int i = 0; i < count; i++)
					ExportCommentCore(DocumentContentWriter, comments[i]);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportCommentCore(XmlWriter writer, Comment comment) {
			string tagName = "comment"; 
			XmlWriter oldWriter = this.DocumentContentWriter;
			try {
				this.DocumentContentWriter = writer;
				WriteWpStartElement(tagName);
				try {
					WriteWpStringAttr("id", Convert.ToString(comment.Index));
					WriteWpStringAttr("author", comment.Author);
						WriteWpStringAttr("date", DateTimeUtils.ToDateTimeISO8601(comment.Date));
					WriteWpStringAttr("initials", comment.Name);
					commentParaId = comment.Index + 1;
					commentParagraphIndex = 0;
					commentParagraphCount = comment.Content.PieceTable.Paragraphs.Count;
					PerformExportPieceTable(comment.Content.PieceTable, ExportPieceTable);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				this.DocumentContentWriter = oldWriter;
			}
		}
		protected internal override void ExportParagraphProperties(Paragraph paragraph) {
			if ((commentParaId > 0) && (commentParagraphIndex == (commentParagraphCount - 1))) {
				WriteStringAttr(W14Prefix, "paraId", W14NamespaceConst, ConvertToHexString(commentParaId).PadLeft(8, '0'));
				commentParaId = 0;
			}
			base.ExportParagraphProperties(paragraph);
			commentParagraphIndex++;
		}
		#endregion
		#region Headers/Footers
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportHeader(sectionHeader);
			ExportHeaderReference("first", relationId);
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportHeader(sectionHeader);
			ExportHeaderReference("default", relationId);
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportHeader(sectionHeader);
			ExportHeaderReference("even", relationId);
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportFooter(sectionFooter);
			ExportFooterReference("first", relationId);
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportFooter(sectionFooter);
			ExportFooterReference("default", relationId);
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			if (linkedToPrevious)
				return;
			string relationId = ExportFooter(sectionFooter);
			ExportFooterReference("even", relationId);
		}
		protected internal virtual string ExportHeader(SectionHeader header) {
			ImageRelationsTableStack.Push(new Dictionary<string, string>());
			ExportedImageTableStack.Push(new Dictionary<OfficeImage, string>());
			try {
				headerCounter++;
				string fileName = String.Format(@"header{0}.xml", headerCounter);
				CompressedStream contentStream = CreateCompressedXmlContent(delegate(XmlWriter writer) { ExportHeaderCore(writer, header); });
				AddCompressedPackageContent(@"word\" + fileName, contentStream);
				RegisterContentTypeOverride("/word/" + fileName, "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml");
				if (ImageRelationsTable.Count > 0) {
					string relsFileName = String.Format(@"word\_rels\header{0}.xml.rels", headerCounter);
					AddCompressedPackageContent(relsFileName, ExportHeaderFooterRelations());
				}
				string relationId = GenerateHeaderRelationId();
				headerRelationsTable.Add(relationId, fileName);
				return relationId;
			}
			finally {
				ExportedImageTableStack.Pop();
				ImageRelationsTableStack.Pop();
			}
		}
		protected internal virtual string ExportFooter(SectionFooter footer) {
			ImageRelationsTableStack.Push(new Dictionary<string, string>());
			ExportedImageTableStack.Push(new Dictionary<OfficeImage, string>());
			try {
				footerCounter++;
				string fileName = String.Format(@"footer{0}.xml", footerCounter);
				CompressedStream contentStream = CreateCompressedXmlContent(delegate(XmlWriter writer) { ExportFooterCore(writer, footer); });
				AddCompressedPackageContent(@"word\" + fileName, contentStream);
				RegisterContentTypeOverride("/word/" + fileName, "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml");
				if (ImageRelationsTable.Count > 0) {
					string relsFileName = String.Format(@"word\_rels\footer{0}.xml.rels", footerCounter);
					AddCompressedPackageContent(relsFileName, ExportHeaderFooterRelations());
				}
				string relationId = GenerateFooterRelationId();
				footerRelationsTable.Add(relationId, fileName);
				return relationId;
			}
			finally {
				ExportedImageTableStack.Pop();
				ImageRelationsTableStack.Pop();
			}
		}
		protected internal virtual string GenerateHeaderRelationId() {
			return String.Format("RelHdr{0}", headerCounter);
		}
		protected internal virtual string GenerateFooterRelationId() {
			return String.Format("RelFtr{0}", footerCounter);
		}
		protected internal virtual void ExportHeaderCore(XmlWriter writer, SectionHeaderFooterBase header) {
			XmlWriter oldWriter = this.DocumentContentWriter;
			try {
				this.DocumentContentWriter = writer;
				WriteWpStartElement("hdr");
				try {
					RegisterNamespaces();
					PerformExportPieceTable(header.PieceTable, ExportPieceTable);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				this.DocumentContentWriter = oldWriter;
			}
		}
		protected internal virtual void ExportFooterCore(XmlWriter writer, SectionFooter footer) {
			XmlWriter oldWriter = this.DocumentContentWriter;
			try {
				this.DocumentContentWriter = writer;
				WriteWpStartElement("ftr");
				try {
					RegisterNamespaces();
					PerformExportPieceTable(footer.PieceTable, ExportPieceTable);
				}
				finally {
					WriteWpEndElement();
				}
			}
			finally {
				this.DocumentContentWriter = oldWriter;
			}
		}
		protected internal virtual void ExportHeaderReference(string type, string headerRelationId) {
			WriteWpStartElement("headerReference");
			try {
				WriteWpStringAttr("type", type);
				DocumentContentWriter.WriteAttributeString(RelsPrefix, "id", RelsNamespace, headerRelationId);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void ExportFooterReference(string type, string footerRelationId) {
			WriteWpStartElement("footerReference");
			try {
				WriteWpStringAttr("type", type);
				DocumentContentWriter.WriteAttributeString(RelsPrefix, "id", RelsNamespace, footerRelationId);
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region FootNotes
		protected internal override void ExportFootNoteReference(FootNoteRun run) {
			WriteWpStartElement("footnoteReference");
			try {
				WriteWpIntAttr("id", run.NoteIndex);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void PopulateExportedFootNotes() {
			FootNoteCollection notes = DocumentModel.FootNotes;
			int count = notes.Count;
			for (int i = 0; i < count; i++)
				if (notes[i].IsReferenced)
					this.footNotes.Add(notes[i]);
		}
		protected internal virtual CompressedStream ExportFootNotes() {
			return CreateCompressedXmlContent(GenerateFootNotesContent);
		}
		protected internal virtual void GenerateFootNotesContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportFootNotesCore();
		}
		protected internal virtual void ExportFootNotesCore() {
			WriteWpStartElement("footnotes");
			try {
				RegisterNamespaces();
				int count = footNotes.Count;
				for (int i = 0; i < count; i++)
					ExportFootNoteCore(DocumentContentWriter, footNotes[i], DocumentModel.FootNotes.IndexOf(footNotes[i]));
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region EndNotes
		protected internal override void ExportEndNoteReference(EndNoteRun run) {
			WriteWpStartElement("endnoteReference");
			try {
				WriteWpIntAttr("id", run.NoteIndex);
			}
			finally {
				WriteWpEndElement();
			}
		}
		protected internal virtual void PopulateExportedEndNotes() {
			EndNoteCollection notes = DocumentModel.EndNotes;
			int count = notes.Count;
			for (int i = 0; i < count; i++)
				if (notes[i].IsReferenced)
					this.endNotes.Add(notes[i]);
		}
		protected internal virtual CompressedStream ExportEndNotes() {
			return CreateCompressedXmlContent(GenerateEndNotesContent);
		}
		protected internal virtual void GenerateEndNotesContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			ExportEndNotesCore();
		}
		protected internal virtual void ExportEndNotesCore() {
			WriteWpStartElement("endnotes");
			try {
				RegisterNamespaces();
				int count = endNotes.Count;
				for (int i = 0; i < count; i++)
					ExportEndNoteCore(DocumentContentWriter, endNotes[i], DocumentModel.EndNotes.IndexOf(endNotes[i]));
			}
			finally {
				WriteWpEndElement();
			}
		}
		#endregion
		#region Floating Objects
		protected internal virtual void WriteWp14DrawingStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(WordProcessingDrawingPrefix14, tag, null);
		}
		protected internal virtual void WriteWpDrawingStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(WordProcessingDrawingPrefix, tag, WordProcessingDrawingNamespace);
		}
		protected internal virtual void WriteADrawingStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(DrawingMLPrefix, tag, DrawingMLNamespace);
		}
		protected internal virtual void WritePicDrawingStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(DrawingMLPicturePrefix, tag, DrawingMLPictureNamespace);
		}
		protected internal virtual void WriteMcStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(MCPrefix, tag, MCNamespace);
		}
		protected internal virtual void WriteWpsStartElement(string tag) {
			DocumentContentWriter.WriteStartElement(WpsPrefix, tag, WpsNamespace);
		}
		protected internal virtual void WriteWpDrawingEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteWp14DrawingEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteADrawingEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WritePicDrawingEndElement() {
			DocumentContentWriter.WriteEndElement();
		}	   
		protected internal virtual void WriteMcEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal virtual void WriteWpsEndElement() {
			DocumentContentWriter.WriteEndElement();
		}
		protected internal override void ExportFloatingObjectAnchorRun(FloatingObjectAnchorRun run) {
			PictureFloatingObjectContent pictureContent = run.Content as PictureFloatingObjectContent;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (pictureContent == null && textBoxContent == null)
				return;
			if (pictureContent != null) {
				WriteWpStartElement("r");
				try {
					ExportRunProperties(run);
					FloatingObjectDrawingObject drawObject = new FloatingObjectDrawingObject(run);
					FloatingObjectDrawingObjectContent content = new FloatingObjectDrawingObjectContent(pictureContent);
					WriteFloatingObjectDrawing(drawObject, content);
				}
				finally {
					WriteWpEndElement();
				}
			}
			else {
				WriteWpStartElement("r");
				try {
					WriteFloatingObjectAlternateContent(run, textBoxContent);
				}
				finally {
					WriteWpEndElement();
				}
			}
		}
		void WriteFloatingObjectAlternateContent(FloatingObjectAnchorRun run, TextBoxFloatingObjectContent textBoxContent) {
			WriteMcStartElement("AlternateContent");
			try {
				int textBoxId = WriteFloatingObjectTextBoxContent2010(run, textBoxContent);
				WriteFloatingObjectTextBoxContent2007(run, textBoxContent, textBoxId);
			}
			finally {
				WriteMcEndElement();
			}
		}
		void WriteFloatingObjectAnchor(IOpenXMLDrawingObject drawingObject, IOpenXMLDrawingObjectContent pictureContent) {
			if (drawingObject.IsFloatingObject)
				WriteWpDrawingStartElement("anchor");
			else
				WriteWpDrawingStartElement("inline");
			try {
				int id = DrawingElementId;
				string name = GenerateFloatingObjectName(drawingObject.Name, "Picture", DrawingElementId);
				IncrementDrawingElementId();
				ExportFloatingObjectProperties(drawingObject, name, id);
				WriteFloatingObjectPictureContent(pictureContent, drawingObject, id);
				if (drawingObject.IsFloatingObject) 
					ExportRelativeSize(drawingObject);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		protected virtual void ExportImageAsDrawing(InlinePictureRun run) {
			if (run.Image != null) {
				InlineDrawingObject drawObject = new InlineDrawingObject(run);
				InlineDrawingObjectContent content = new InlineDrawingObjectContent(run.Image);
				WriteFloatingObjectDrawing(drawObject, content);
			}
		}
		void WriteFloatingObjectDrawing(IOpenXMLDrawingObject drawingObject, IOpenXMLDrawingObjectContent pictureContent) {
			WriteWpStartElement("drawing");
			try {
				WriteFloatingObjectAnchor(drawingObject, pictureContent);
			}
			finally {
				WriteWpEndElement();
			}
		}
		int WriteFloatingObjectTextBoxContent2010(FloatingObjectAnchorRun run, TextBoxFloatingObjectContent textBoxContent) {
			WriteMcStartElement("Choice");
			try {
				WriteStringValue("Requires", WpsPrefix);
				ExportRunProperties(run);
				FloatingObjectDrawingObject drawingObject = new FloatingObjectDrawingObject(run);
				return WriteFloatingObjectDrawing(drawingObject, textBoxContent);
			}
			finally {
				WriteMcEndElement();
			}
		}
		int WriteFloatingObjectDrawing(IOpenXMLDrawingObject drawingObject, TextBoxFloatingObjectContent textBoxContent) {
			WriteWpStartElement("drawing");
			try {
				return WriteFloatingObjectAnchor(drawingObject, textBoxContent);
			}
			finally {
				WriteWpEndElement();
			}
		}
		int WriteFloatingObjectAnchor(IOpenXMLDrawingObject drawingObject, TextBoxFloatingObjectContent textBoxContent) {
			WriteWpDrawingStartElement("anchor");
			try {
				int id = DrawingElementId;
				string name = GenerateFloatingObjectName(drawingObject.Name, "Text Box", DrawingElementId);
				IncrementDrawingElementId();
				ExportFloatingObjectProperties(drawingObject, name, id);
				WriteFloatingObjectTextBoxContent(textBoxContent, drawingObject);
				ExportRelativeSize(drawingObject);
				return id;
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectTextBoxContent2007(FloatingObjectAnchorRun run, TextBoxFloatingObjectContent textBoxContent, int textBoxId) {
			WriteMcStartElement("Fallback");
			try {
				string name = GenerateFloatingObjectName(run.Name, "Text Box", textBoxId);
				WriteFloatingObjectPict(run.FloatingObjectProperties, textBoxContent, null, run.Shape, name);
			}
			finally {
				WriteMcEndElement();
			}
		}
		void WriteFloatingObjectTextBoxContent(TextBoxFloatingObjectContent content, IOpenXMLDrawingObject drawingObject) {
			WriteADrawingStartElement("graphic");
			try {
				WriteFloatingObjectGraphicData(content, drawingObject);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectGraphicData(TextBoxFloatingObjectContent content, IOpenXMLDrawingObject drawingObject) {
			WriteADrawingStartElement("graphicData");
			try {
				WriteStringValue("uri", WpsNamespace);
				WriteFloatingObjectWsp(content, drawingObject);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectWsp(TextBoxFloatingObjectContent content, IOpenXMLDrawingObject drawingObject) {
			WriteWpsStartElement("wsp");
			try {
				WriteFloatingObjectCNvSpPr();
				WriteFloatingObjectWpsSpPr(drawingObject);
				WriteFloatingObjectTxbx(content); 
				WriteFloatingObjectBodyPr(content.TextBoxProperties);
			}
			finally {
				WriteWpsEndElement();
			}
		}
		void WriteFloatingObjectBodyPr(TextBoxProperties properties) {
			WriteWpsStartElement("bodyPr");
			try {
				if (properties.UseWrapText)
					WriteStringValue("wrap", properties.WrapText ? "square" : "none");
				if (properties.UseLeftMargin)
					WriteIntValue("lIns", UnitConverter.ModelUnitsToEmu(properties.LeftMargin));
				if (properties.UseTopMargin)
					WriteIntValue("tIns", UnitConverter.ModelUnitsToEmu(properties.TopMargin));
				if (properties.UseRightMargin)
					WriteIntValue("rIns", UnitConverter.ModelUnitsToEmu(properties.RightMargin));
				if (properties.UseBottomMargin)
					WriteIntValue("bIns", UnitConverter.ModelUnitsToEmu(properties.BottomMargin));
				if (properties.UseVerticalAlignment)
					WriteStringValue("anchor", ConvertTextBoxVerticalAlignment(properties.VerticalAlignment));
				if (properties.UseUpright)
					WriteStringValue("upright", ConvertBoolToString(properties.Upright));
				if (properties.UseResizeShapeToFitText) {
					if (properties.ResizeShapeToFitText)
						WriteADrawingStartElement("spAutoFit");
					else
						WriteADrawingStartElement("noAutofit");
					WriteADrawingEndElement();
				}
			}
			finally {
				WriteWpsEndElement();
			}
		}
		protected string ConvertTextBoxVerticalAlignment(VerticalAlignment value) {
			WordProcessingMLValue result;
			if (textBoxVerticalAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(verticalAlignmentTable[VerticalAlignment.Top]);
		}
		void WriteFloatingObjectTxbx(TextBoxFloatingObjectContent content) {
			WriteWpsStartElement("txbx");
			try {
				WriteFloatingObjectTxbxContent(content);
			}
			finally {
				WriteWpsEndElement();
			}
		}
		void WriteFloatingObjectWpsSpPr(IOpenXMLDrawingObject drawingObject) {
			WriteWpsStartElement("spPr");
			try {
				WriteFloatingObjectSpPr(drawingObject);
			} finally {
				WriteWpsEndElement();
			}
		}
		void WriteFloatingObjectLn(Shape shape) {
			WriteADrawingStartElement("ln");
			try {
				if(shape.UseOutlineWidth)
					WriteIntValue("w", ModelUnitsToEMU(shape.OutlineWidth));
				WriteFloatingObjectSolidFill(shape.OutlineColor);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectSolidFill(Color color) {
			if (DXColor.IsTransparentOrEmpty(color)) {
				WriteADrawingStartElement("noFill");
				WriteADrawingEndElement();
			}
			else {
				WriteADrawingStartElement("solidFill");
				try {
					WriteFloatingObjectSrgbClr(color);
				}
				finally {
					WriteADrawingEndElement();
				}
			}
		}
		void WriteFloatingObjectSrgbClr(Color color) {
			WriteADrawingStartElement("srgbClr");
			try {
				WriteStringValue("val", ConvertColorToString(color));
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectCNvSpPr() {
			WriteWpsStartElement("cNvSpPr");
			WriteWpsEndElement();
		}
		void WriteFloatingObjectGraphicData(IOpenXMLDrawingObjectContent content, IOpenXMLDrawingObject run, int id) {
			WriteADrawingStartElement("graphicData");
			try {
				WriteStringValue("uri", DrawingMLPictureNamespace);
				WriteFloatingObjectPic(content, run, id);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectCNvPicPr() {
			WritePicDrawingStartElement("cNvPicPr");
			WritePicDrawingEndElement();
		}
		void WriteFloatingObjectCNvPr(int id) {
			WritePicDrawingStartElement("cNvPr");
			try {
				WriteIntValue("id", id);
				WriteStringValue("name", "Picture " + id );
			}
			finally {
				WritePicDrawingEndElement();
			}  
		}
		void WriteFloatingObjectNvPicPr(int id) {
			WritePicDrawingStartElement("nvPicPr");
			try {
				WriteFloatingObjectCNvPr(id);
				WriteFloatingObjectCNvPicPr();			   
			}
			finally {
				WritePicDrawingEndElement();
			}  
		}
		private void WriteFloatingObjectExt(IOpenXMLDrawingObject run) {
			WriteADrawingStartElement("ext");
			try {
				WriteIntValue("cx", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(0, run.ActualSize.Width)));
				WriteIntValue("cy", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(0, run.ActualSize.Height)));
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectOff() {
			WriteADrawingStartElement("off");
			try {
				WriteIntValue("x", 0);
				WriteIntValue("y", 0);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectBlip(IOpenXMLDrawingObjectContent content) {
			string imageRelationId = ExportImageData(content.Image);
			WriteADrawingStartElement("blip");
			try {
				WriteStringAttr(RelsPrefix, "embed", RelsNamespace, imageRelationId);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectStretch() {
			WriteADrawingStartElement("stretch");
			try {
				WriteADrawingStartElement("fillRect");
				WriteADrawingEndElement();
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectBlipFill(IOpenXMLDrawingObjectContent content) {
			WritePicDrawingStartElement("blipFill");
			try {
				WriteFloatingObjectBlip(content);
				WriteFloatingObjectStretch();
			}
			finally {
				WritePicDrawingEndElement();
			}
		}
		void WriteFloatingObjectXfrm(IOpenXMLDrawingObject run) {
			WriteADrawingStartElement("xfrm");
			try {
				if (!Object.ReferenceEquals(run.Shape, null) && run.UseRotation)
					WriteIntValue("rot", UnitConverter.ModelUnitsToAdjAngle(run.Rotation));
				WriteFloatingObjectOff();
				WriteFloatingObjectExt(run);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectPrstGeom() {
			WriteADrawingStartElement("prstGeom");
			try {
				WriteStringValue("prst", "rect");  
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectPicSpPr(IOpenXMLDrawingObject drawingObject) {
			WritePicDrawingStartElement("spPr");
			try {
				WriteFloatingObjectSpPr(drawingObject);
			}
			finally {
				WritePicDrawingEndElement();
			}
		}
		void WriteFloatingObjectSpPr(IOpenXMLDrawingObject drawingObject) {
			WriteFloatingObjectXfrm(drawingObject); 
			WriteFloatingObjectPrstGeom();
			if (!Object.ReferenceEquals(drawingObject.Shape, null)) {
				if (drawingObject.Shape.UseFillColor)
					WriteFloatingObjectSolidFill(drawingObject.Shape.FillColor);
				if (drawingObject.Shape.UseOutlineColor)
					WriteFloatingObjectLn(drawingObject.Shape);
			}					 
		}
		void WriteFloatingObjectPic(IOpenXMLDrawingObjectContent content, IOpenXMLDrawingObject drawingObject, int id) {
			WritePicDrawingStartElement("pic");
			try {
				WriteFloatingObjectNvPicPr(id);
				WriteFloatingObjectBlipFill(content);
				WriteFloatingObjectPicSpPr(drawingObject);
			}
			finally {
				WritePicDrawingEndElement();
			}		  
		}
		void WriteInlinePictureRunContent(InlinePictureRun run, int id) {
			WriteADrawingStartElement("graphic");
			try {
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectPictureContent(IOpenXMLDrawingObjectContent content, IOpenXMLDrawingObject drawingObject, int id) {
			WriteADrawingStartElement("graphic");
			try {
				WriteFloatingObjectGraphicData(content, drawingObject, id);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void ExportFloatingObjectProperties(IOpenXMLDrawingObject drawingObject, string name, int id) {
			if (drawingObject.IsFloatingObject) {
				WriteIntValue("simplePos", 0);
				WriteBoolValue("allowOverlap", drawingObject.AllowOverlap);
				WriteBoolValue("behindDoc",  drawingObject.IsBehindDoc);
				WriteBoolValue("layoutInCell", drawingObject.LayoutInTableCell);
				WriteBoolValue("locked", drawingObject.Locked);
				WriteIntValue("relativeHeight", drawingObject.ZOrder);
				if (drawingObject.UseBottomDistance && drawingObject.BottomDistance > 0)
					WriteIntValue("distB", ModelUnitsToEMU(drawingObject.BottomDistance));
				if (drawingObject.UseLeftDistance && drawingObject.LeftDistance > 0)
					WriteIntValue("distL", ModelUnitsToEMU(drawingObject.LeftDistance));
				if (drawingObject.UseRightDistance && drawingObject.RightDistance > 0)
					WriteIntValue("distR", ModelUnitsToEMU(drawingObject.RightDistance));
				if (drawingObject.UseTopDistance && drawingObject.TopDistance > 0)
					WriteIntValue("distT", ModelUnitsToEMU(drawingObject.TopDistance));
				if (drawingObject.UseHidden)
					WriteBoolValue("hidden", drawingObject.Hidden);
				WriteFloatingObjectSimplePosition();
				if (drawingObject.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None && drawingObject.UsePercentOffset && drawingObject.PercentOffsetX != 0) {
					WriteFloatingObjectPercentPositionH2010(drawingObject);
				}
				else {
					WriteFloatingObjectPositionH(drawingObject);
				}
				if (drawingObject.VerticalPositionAlignment == FloatingObjectVerticalPositionAlignment.None && drawingObject.UsePercentOffset && drawingObject.PercentOffsetY != 0) {
					WriteFloatingObjectPercentPositionV2010(drawingObject);
				}
				else {
					WriteFloatingObjectPositionV(drawingObject);
				}
			}
			WriteFloatingObjectExtent(drawingObject);
			WriteFloatingObjectEffectExtent(drawingObject);
			if (drawingObject.IsFloatingObject) 
				WriteFloatingObjectWrap(drawingObject);
			WriteFloatingObjectDocPr(name, id);
			WriteFloatingObjectCNvGraphicFramePr(drawingObject);
		}
		void ExportInlinePictureRun(InlinePictureRun run, string name, int id) {
			WriteInlinePictureRunExtent(run);
			WriteInlinePictureEffectExtent(run);
			WriteInlinePictureDocPr(name, id);
			WriteInlinePictureCNvGraphicFramePr(run);
		}
		private void ExportRelativeSize(IOpenXMLDrawingObject run) {
			if ((bool)run.UseRelativeWidth)
				WriteFloatingObjectPercentWidth(run.RelativeWidth);
			if ((bool)run.UseRelativeHeight)
				WriteFloatingObjectPercentHeight(run.RelativeHeight);
		}
		void WriteInlinePictureCNvGraphicFramePr(InlinePictureRun run) {
			WriteWpDrawingStartElement("cNvGraphicFramePr");
			try {
				WriteInlinePictureGraphicFrameLocks(run);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteInlinePictureGraphicFrameLocks(InlinePictureRun run) {
			WriteADrawingStartElement("graphicFrameLocks");
			try {
				WriteBoolValue("noChangeAspect", run.LockAspectRatio);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteFloatingObjectCNvGraphicFramePr(IOpenXMLDrawingObject run) {
			if (!run.UseLockAspectRatio)
				return;
			WriteWpDrawingStartElement("cNvGraphicFramePr");
			try {
				WriteFloatingObjectGraphicFrameLocks(run);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectGraphicFrameLocks(IOpenXMLDrawingObject run) {
			WriteADrawingStartElement("graphicFrameLocks");
			try {
				WriteBoolValue("noChangeAspect", run.LockAspectRatio);
			}
			finally {
				WriteADrawingEndElement();
			}
		}
		void WriteInlinePictureDocPr(string name, int id) {
			WriteElementDocPrCore(name, id);
		}
		void WriteFloatingObjectDocPr(string name, int id) {
			WriteElementDocPrCore(name, id);
		}
		void WriteElementDocPrCore(string name, int id) {
			WriteWpDrawingStartElement("docPr");
			try {
				WriteIntValue("id", id);
				if (!String.IsNullOrEmpty(name))
					WriteStringValue("name", name);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectSimplePosition() {
			WriteWpDrawingStartElement("simplePos");
			try {
				WriteIntValue("x", 0);
				WriteIntValue("y", 0);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectWrap(IOpenXMLDrawingObject run) {
			WordProcessingMLValue textWrapType;
			bool useTextWrapSide = false;
			if (floatingObjectTextWrapTypeTable.TryGetValue(run.TextWrapType, out textWrapType)) {
				if (run.TextWrapType == FloatingObjectTextWrapType.Square || run.TextWrapType == FloatingObjectTextWrapType.Through || run.TextWrapType == FloatingObjectTextWrapType.Tight)
					useTextWrapSide = true;
				WriteWpDrawingElement(run, textWrapType.OpenXmlValue, useTextWrapSide);
			}
		}
		void WriteWpWrapPolygonElement(IOpenXMLDrawingObject run) {
			if (run.TextWrapType != FloatingObjectTextWrapType.Through && run.TextWrapType != FloatingObjectTextWrapType.Tight)
				return;
			WriteWpDrawingStartElement("wrapPolygon");
			try {
				WriteWpDrawingStart();
				WriteWpLineToDrawingElement(0, 21600);
				WriteWpLineToDrawingElement(21600, 21600);
				WriteWpLineToDrawingElement(21600, 0);
				WriteWpLineToDrawingElement(0, 0);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteWpLineToDrawingElement(int x, int y) {
			WriteWpDrawingStartElement("lineTo");
			try {
				WriteIntValue("x", x);
				WriteIntValue("y", y);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteWpDrawingStart() {
			WriteWpDrawingStartElement("start");
			try {
				WriteIntValue("x", 0);
				WriteIntValue("y", 0);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteWpDrawingElement(IOpenXMLDrawingObject run, string elementName, bool useTextWrapSide) {
			WriteWpDrawingStartElement(elementName);
			try {
				if (useTextWrapSide)
					WriteStringValue("wrapText", ConvertFloatingObjectTextWrapSide(run.TextWrapSide));
				WriteWpWrapPolygonElement(run);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectPercentWidth(FloatingObjectRelativeWidth relativeWidth) {
			WriteWp14DrawingStartElement("sizeRelH");
			try {
				WriteStringValue("relativeFrom", ConvertFloatingObjectRelativeFromHorizontalType(relativeWidth.From));
				WriteWp14DrawingStartElement("pctWidth");
				DocumentContentWriter.WriteString(relativeWidth.Width.ToString());
				WriteWp14DrawingEndElement();
			}
			finally {
				WriteWp14DrawingEndElement();
			}
		}
		string ConvertFloatingObjectRelativeFromHorizontalType(FloatingObjectRelativeFromHorizontal value) {
			WordProcessingMLValue result;
			if (floatingObjectRelativeFromHorizontalTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(floatingObjectRelativeFromHorizontalTable[FloatingObjectRelativeFromHorizontal.Page]);
		}
		void WriteFloatingObjectPercentHeight(FloatingObjectRelativeHeight relativeHeight) {
			WriteWp14DrawingStartElement("sizeRelV");
			try {				
				WriteStringValue("relativeFrom", ConvertFloatingObjectRelativeFromVerticalType(relativeHeight.From));
				WriteWp14DrawingStartElement("pctHeight");
				DocumentContentWriter.WriteString(relativeHeight.Height.ToString());
				WriteWp14DrawingEndElement();
			}
			finally {
				WriteWp14DrawingEndElement();
			}
		}
		string ConvertFloatingObjectRelativeFromVerticalType(FloatingObjectRelativeFromVertical value) {
			WordProcessingMLValue result;
			if (floatingObjectRelativeFromVerticalTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(floatingObjectRelativeFromVerticalTable[FloatingObjectRelativeFromVertical.Page]);
		}
		void WriteFloatingObjectPercentPositionV2010(IOpenXMLDrawingObject drawingObject) {
			WriteMcStartElement("AlternateContent");
			WriteMcStartElement("Choice");
			WriteStringValue("Requires", Wp14Prefix);
			WriteFloatingObjectPositionVCore(drawingObject, WriteFloatingObjectPercentVerticalOffset);
			WriteMcEndElement();
			WriteMcStartElement("Fallback");
			WriteFloatingObjectPositionV(drawingObject);
			WriteMcEndElement();
			WriteMcEndElement();
		}
		void WriteFloatingObjectPositionV(IOpenXMLDrawingObject drawingObject) {
			WriteFloatingObjectPositionVCore(drawingObject, WriteFloatingObjectVerticalOffset);
		}
		void WriteFloatingObjectPositionVCore(IOpenXMLDrawingObject drawingObject, Action<IOpenXMLDrawingObject> writeVerticalOffsetAction) {
			WriteWpDrawingStartElement("positionV");
			try {
				WriteStringValue("relativeFrom", ConvertFloatingObjectVerticalPositionType(drawingObject.VerticalPositionType));
				if (drawingObject.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None)
					WriteFloatingObjectVerticalPositionAlignment(drawingObject);
				else
					writeVerticalOffsetAction(drawingObject);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectVerticalOffset(IOpenXMLDrawingObject drawingObject) {
			WriteWpDrawingStartElement("posOffset");
			DocumentContentWriter.WriteString(DocumentModel.UnitConverter.ModelUnitsToEmu(drawingObject.Offset.Y).ToString());
			WriteWpDrawingEndElement();
		}
		void WriteFloatingObjectPercentVerticalOffset(IOpenXMLDrawingObject drawingObject) {
			WriteWp14DrawingStartElement("pctPosVOffset");
			try {
				DocumentContentWriter.WriteString(drawingObject.PercentOffset.Y.ToString());
			}
			finally {
				WriteWp14DrawingEndElement();
			}
		}
		void WriteFloatingObjectVerticalPositionAlignment(IOpenXMLDrawingObject drawingObject) {
			WriteWpDrawingStartElement("align");
			try {
				DocumentContentWriter.WriteString(ConvertFloatingObjectVerticalPositionAlignment(drawingObject.VerticalPositionAlignment));
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		protected string ConvertFloatingObjectVerticalPositionAlignment(FloatingObjectVerticalPositionAlignment value) {
			WordProcessingMLValue result;
			if (floatingObjectVerticalPositionAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return String.Empty;
		}
		protected string ConvertFloatingObjectHorizontalPositionAlignment(FloatingObjectHorizontalPositionAlignment value) {
			WordProcessingMLValue result;
			if (floatingObjectHorizontalPositionAlignmentTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return String.Empty;
		}
		protected string ConvertFloatingObjectVerticalPositionType(FloatingObjectVerticalPositionType value) {
			WordProcessingMLValue result;
			if (floatingObjectVerticalPositionTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(floatingObjectVerticalPositionTypeTable[FloatingObjectVerticalPositionType.Page]);
		}
		protected string ConvertFloatingObjectHorizontalPositionType(FloatingObjectHorizontalPositionType value) {
			WordProcessingMLValue result;
			if (floatingObjectHorizontalPositionTypeTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(floatingObjectHorizontalPositionTypeTable[FloatingObjectHorizontalPositionType.Page]);
		}
		protected string ConvertFloatingObjectTextWrapSide(FloatingObjectTextWrapSide value) {
			WordProcessingMLValue result;
			if (floatingObjectTextWrapSideTable.TryGetValue(value, out result))
				return GetWordProcessingMLValue(result);
			else
				return GetWordProcessingMLValue(floatingObjectTextWrapSideTable[FloatingObjectTextWrapSide.Both]);
		}
		void WriteFloatingObjectPercentPositionH2010(IOpenXMLDrawingObject drawingObject) {
			WriteMcStartElement("AlternateContent");
			WriteMcStartElement("Choice");
			WriteStringValue("Requires", Wp14Prefix);
			WriteFloatingObjectPositionHCore(drawingObject, WriteFloatingObjectPercentHorizontalOffset);
			WriteMcEndElement();
			WriteMcStartElement("Fallback");
			WriteFloatingObjectPositionH(drawingObject);
			WriteMcEndElement();
			WriteMcEndElement();
		}
		void WriteFloatingObjectPositionH(IOpenXMLDrawingObject drawingObject) {
			WriteFloatingObjectPositionHCore(drawingObject, WriteFloatingObjectHorizontalOffset);
		}
		void WriteFloatingObjectPositionHCore(IOpenXMLDrawingObject drawingObject, Action<IOpenXMLDrawingObject> writeHorizontalOffsetAction) {
			WriteWpDrawingStartElement("positionH");
			try {
				WriteStringValue("relativeFrom", ConvertFloatingObjectHorizontalPositionType(drawingObject.HorizontalPositionType));
				if (drawingObject.HorizontalPositionAlignment != FloatingObjectHorizontalPositionAlignment.None)
					WriteFloatingObjectHorizontalPositionAlignment(drawingObject);
				else
					writeHorizontalOffsetAction(drawingObject);
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectHorizontalPositionAlignment(IOpenXMLDrawingObject drawingObject) {
			WriteWpDrawingStartElement("align");
			try {
				DocumentContentWriter.WriteString(ConvertFloatingObjectHorizontalPositionAlignment(drawingObject.HorizontalPositionAlignment));
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectHorizontalOffset(IOpenXMLDrawingObject drawingObject) {
			WriteWpDrawingStartElement("posOffset");
			try {
				DocumentContentWriter.WriteString(DocumentModel.UnitConverter.ModelUnitsToEmu(drawingObject.Offset.X).ToString());
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteFloatingObjectPercentHorizontalOffset(IOpenXMLDrawingObject drawingObject) {
			WriteWp14DrawingStartElement("pctPosHOffset");
			try {
				DocumentContentWriter.WriteString(drawingObject.PercentOffset.X.ToString());
			}
			finally {
				WriteWp14DrawingEndElement();
			}
		}
		void WriteFloatingObjectExtent(IOpenXMLDrawingObject drawingObject) {
			WriteWpDrawingStartElement("extent");
			try {
				WriteIntValue("cx", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(0, drawingObject.ActualSize.Width)));
				WriteIntValue("cy", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(drawingObject.ActualSize.Height, 0)));
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteInlinePictureRunExtent(InlinePictureRun run) {
			WriteElementExtentCore(run.ActualSize);
		}
		void WriteElementExtentCore(Size actualSize) {
			WriteWpDrawingStartElement("extent");
			try {
				WriteIntValue("cx", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(0, actualSize.Width)));
				WriteIntValue("cy", DocumentModel.UnitConverter.ModelUnitsToEmu(Math.Max(0, actualSize.Height)));
			}
			finally {
				WriteWpDrawingEndElement();
			}
		}
		void WriteInlinePictureEffectExtent(InlinePictureRun run) { 
		 WriteWpDrawingStartElement("effectExtent");
		 try {
			 WriteIntValue("l", 0);
			 WriteIntValue("t", 0);
			 WriteIntValue("r", 0);
			 WriteIntValue("b", 0);
		 }
		 finally {
			 WriteWpDrawingEndElement();
		 }
		}
		void WriteFloatingObjectEffectExtent(IOpenXMLDrawingObject run) {
			if (!run.UseRotation)
				return;
			WriteWpDrawingStartElement("effectExtent");
			try {
				EffectExtent effectExtent = new EffectExtent(DocumentModel.UnitConverter.ModelUnitsToEmu(run.ActualSize.Width),
															 DocumentModel.UnitConverter.ModelUnitsToEmu(run.ActualSize.Height),
															 run.Rotation, DocumentModel.UnitConverter);
				effectExtent.Calculate();
				WriteIntValue("l", effectExtent.HorizontalIndent);
				WriteIntValue("t", effectExtent.VerticalIndent);
				WriteIntValue("r", effectExtent.HorizontalIndent);
				WriteIntValue("b", effectExtent.VerticalIndent);
			} finally {
				WriteWpDrawingEndElement();
			}
		}
		#endregion
		int ModelUnitsToEMU(int modelUnits) {
			int twips = UnitConverter.ModelUnitsToTwips(modelUnits);
			return twips * 91440 / 144;
		}
		protected internal override string ConvertBoolToString(bool value) {
			return value ? "1" : "0";
		}
		protected internal virtual string GetAbstractNumberingId(int styleIndex) {
			return "C" + styleIndex.ToString();
		}
		protected internal override string GetWordProcessingMLValue(WordProcessingMLValue value) {
			return value.OpenXmlValue;
		}
		protected internal override void ExportImageReference(FloatingObjectAnchorRun run) {
		}
		protected internal override void ExportStyleName(IStyle style) {
			if (!DocumentModel.DocumentExportOptions.OpenXml.AllowAlternateStyleNames && !DocumentModel.DocumentExportOptions.OpenXml.LimitStyleNameTo253Chars)
				base.ExportStyleName(style);
			else {
				string styleName = PrepareStyleName(style.StyleName);
				int index = styleName.IndexOf(',');
				if (index < 0)
					WriteWpStringValue("name", styleName);
				else {
					string[] names = styleName.Trim(',').Split(',');
					if (!string.IsNullOrEmpty(names[0]))
						WriteWpStringValue("name", names[0]);
					if (names.Length > 1)
						WriteWpStringValue("aliases", string.Join(",", names, 1, names.Length - 1));
				}
			}
		}
	}
	#endregion
	#region EffectExtent
	public class EffectExtent {
		int horizontalIndent;
		int verticalIndent;
		int width;
		int height;
		int rotation;
		DocumentModelUnitConverter unitConverter;
		public EffectExtent(int width, int height, int rotation, DocumentModelUnitConverter unitConverter) {
			this.width = width;
			this.height = height;
			this.rotation = rotation;
			this.unitConverter = unitConverter;
		}
		public int HorizontalIndent { get { return horizontalIndent; } }
		public int VerticalIndent { get { return verticalIndent; } }
		public void Calculate() {
			float angle = unitConverter.ModelUnitsToDegree(rotation) * (float)Math.PI / 180f;
			int xCoordinateA = width / 2;
			int yCoordinateA = height / 2;
			int newXCoordinateA = (int)(xCoordinateA * Math.Cos(angle) - yCoordinateA * Math.Sin(angle));
			int newYCoordinateA = (int)(xCoordinateA * Math.Sin(angle) + yCoordinateA * Math.Cos(angle));
			int xCoordinateB = xCoordinateA;
			int yCoordinateB = -yCoordinateA;
			int newXCoordinateB = (int)(xCoordinateB * Math.Cos(angle) - yCoordinateB * Math.Sin(angle));
			int newYCoordinateB = (int)(xCoordinateB * Math.Sin(angle) + yCoordinateB * Math.Cos(angle));
			if (Math.Abs(newXCoordinateA) > Math.Abs(newXCoordinateB)) {
				horizontalIndent = Math.Abs(Math.Abs(newXCoordinateA) - Math.Abs(xCoordinateA));
				verticalIndent = Math.Abs(Math.Abs(newYCoordinateB) - Math.Abs(yCoordinateB));
			} else {
				horizontalIndent = Math.Abs(Math.Abs(newXCoordinateB) - Math.Abs(xCoordinateB));
				verticalIndent = Math.Abs(Math.Abs(newYCoordinateA) - Math.Abs(yCoordinateA));
			}
			if ((unitConverter.ModelUnitsToDegree(rotation) - 90) % 180 == 0)
				horizontalIndent = 0;
		}
	}
	#endregion
}
