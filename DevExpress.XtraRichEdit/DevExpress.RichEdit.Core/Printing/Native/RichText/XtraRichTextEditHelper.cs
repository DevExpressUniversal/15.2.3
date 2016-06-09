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
using System.Drawing;
using System.IO;
using System.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Native.RichText {
	public static class XtraRichTextEditHelper {
		public const int DefaultPlainTextFontSize = 12;
		public const string DefaultPlainTextFontName = "Times New Roman";
		#region inner classes
		public class RtfImpoterFactory {
			static readonly RtfImpoterFactory instance = new RtfImpoterFactory();
			public static RtfImpoterFactory Instance { get { return instance; } }
			protected RtfImpoterFactory() {
			}
			public virtual RtfImporter CreateRtfImporter(DocumentModel documentModel) {
				return new RtfImporter(documentModel, new RtfDocumentImporterOptions());
			}
		}
		public class MailMergeRtfImpoterFactory : RtfImpoterFactory {
			static readonly MailMergeRtfImpoterFactory instance = new MailMergeRtfImpoterFactory();
			public static new MailMergeRtfImpoterFactory Instance { get { return instance; } }
			MailMergeRtfImpoterFactory() {
			}
			public override RtfImporter CreateRtfImporter(DocumentModel documentModel) {
				return new XRMailMergeRtfImporter((MailMergeDocumentManager)documentModel);
			}
		}
		#endregion
		public static DocumentModel CreateDocumentModel() {
			DocumentModel documentModel = new SimpleDocumentModel(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
			documentModel.LayoutUnit = DevExpress.Office.DocumentLayoutUnit.Document;
			documentModel.LayoutOptions.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge = true;
			return documentModel;
		}
		public static string GetRtfFromDocManager(DocumentModel documentModel) {
			return XtraRichTextEditRtfExportProvider.CreateRtf(documentModel);
		}
		public static string GetTextFromDocManager(DocumentModel documentModel) {
			return documentModel.InternalAPI.GetDocumentPlainTextContent();
		}
		public static string[] GetTextLinesFromDocManager(DocumentModel documentModel) {
			string content = GetTextFromDocManager(documentModel);
			return TextManipulatorHelper.GetPlainTextLines(content);
		}
		public static void ImportHtmlTextStreamToDocManager(Stream source, string sourceUri, DocumentModel documentModel) {
			ImportHtmlTextStreamToDocManager(source, sourceUri, documentModel, false); 
		}
		public static void ImportHtmlTextStreamToDocManager(Stream source, string sourceUri, DocumentModel documentModel, bool ignoreMetaCharset) {
			DevExpress.XtraRichEdit.Import.HtmlDocumentImporterOptions options = new DevExpress.XtraRichEdit.Import.HtmlDocumentImporterOptions();
			options.AsyncImageLoading = false;
			if(!string.IsNullOrEmpty(sourceUri))
				options.SourceUri = sourceUri;
			options.IgnoreMetaCharset = ignoreMetaCharset;
			DevExpress.XtraRichEdit.Import.Html.HtmlImporter importer = new DevExpress.XtraRichEdit.Import.Html.HtmlImporter(documentModel, options);
			importer.Import(source);
		}
		public static string ConvertHtmlTextStreamToRtf(Stream source) {
			using(DocumentModel documentModel = CreateDocumentModel()) {
				ImportHtmlTextStreamToDocManager(source, null, documentModel);
				DevExpress.XtraRichEdit.Export.Rtf.RtfExporter rtfExporter = new DevExpress.XtraRichEdit.Export.Rtf.RtfExporter(documentModel, new DevExpress.XtraRichEdit.Export.RtfDocumentExporterOptions());
				return rtfExporter.Export();
			}
		}
		public static void ImportOpenXmlContent(Stream stream, DocumentModel documentModel) {
			using(OpenXmlImporter importer = new OpenXmlImporter(documentModel, new OpenXmlDocumentImporterOptions())) {
				importer.Import(stream);
			}
		}
		public static void ImportRtfTextToDocManager(string rtfText, DocumentModel documentModel) {
			ImportRtfTextToDocManager(rtfText, documentModel, RtfImpoterFactory.Instance);
		}
		public static void ImportRtfTextToMailMergeDocManager(string rtfText, MailMergeDocumentManager documentModel) {
			ImportRtfTextToDocManager(rtfText, documentModel, MailMergeRtfImpoterFactory.Instance);
		}
		static void ImportRtfTextToDocManager(string rtfText, DocumentModel documentModel, RtfImpoterFactory factory) {
			if (String.IsNullOrEmpty(rtfText))
				rtfText = RtfTags.WrapTextInRtf(rtfText);
			using (MemoryStream stream = new MemoryStream()) {
				StreamWriter writer = new StreamWriter(stream, EmptyEncoding.Instance);
				writer.Write(GetUnicodeCompatibleString(rtfText));
				writer.Flush();
				stream.Position = 0;
				ImportRtfTextStreamToDocManager(stream, documentModel, factory);
			}
		}
		public static void ImportRtfTextStreamToDocManager(Stream stream, DocumentModel documentModel) {
			ImportRtfTextStreamToDocManager(stream, documentModel, RtfImpoterFactory.Instance);
		}
		static void ImportRtfTextStreamToDocManager(Stream stream, DocumentModel documentModel, RtfImpoterFactory factory) {
			XtraRichTextEditHelper.ClearDocumentModel(documentModel);
			RtfImporter importer = factory.CreateRtfImporter(documentModel);
			try {
				importer.Import(stream);
				importer.Dispose();
			}
			catch {
			}
			finally {
				importer.Dispose();
			}
		}
		public static void ImportPlainTextStreamToDocManager(Stream stream, DocumentModel documentModel) {
			ClearDocumentModel(documentModel);
			StreamReader reader = new StreamReader(stream);
			ImportPlainTextToDocManager(reader.ReadToEnd(), documentModel);
		}
		public static void ImportPlainTextToDocManager(string text, DocumentModel documentModel) {
			TextManipulatorHelper.SetText(documentModel.MainPieceTable, text, DefaultPlainTextFontName, 2 * DefaultPlainTextFontSize);
		}
		public static void ImportPlainTextLinesToDocManager(string[] lines, DocumentModel documentModel, Font font) {
			TextManipulatorHelper.SetTextLines(documentModel.MainPieceTable, lines, DefaultPlainTextFontName, 2 * DefaultPlainTextFontSize);
		}
		public static void SetContentFont(DocumentModel documentModel, Font font) {
			PieceTable pieceTable = documentModel.MainPieceTable;
			if (pieceTable.Paragraphs.Count <= 0)
				return;
			documentModel.BeginUpdate();
			try {
				int length = pieceTable.DocumentEndLogPosition - pieceTable.DocumentStartLogPosition;
				if (length != 0)
					pieceTable.SetFont(pieceTable.DocumentStartLogPosition, length, font);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public static void SetContentForeColor(DocumentModel documentModel, Color foreColor) {
			TextManipulatorHelper.SetTextForeColor(documentModel.MainPieceTable, foreColor);
		}
		public static void ClearDocumentModel(DocumentModel documentModel) {
			TextManipulatorHelper.SetText(documentModel.MainPieceTable, string.Empty, DefaultPlainTextFontName, 2 * DefaultPlainTextFontSize);
		}
		public static void WriteRtfFromDocManagerToStream(Stream stream, DocumentModel documentModel) {
			XtraRichTextEditRtfExportProvider.CreateRtf(stream, documentModel);
		}
		public static void WritePlainTextFromDocManagerToStream(Stream stream, DocumentModel documentModel) {
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(GetTextFromDocManager(documentModel));
			writer.Flush();
		}
		static string GetUnicodeCompatibleString(string text) {
			StringBuilder unicodeTextBuilder = new StringBuilder();
			unicodeTextBuilder.Length = 0;
			int textLength = text.Length;
			for (int i = 0; i < textLength; i++) {
				char ch = text[i];
				int code = (short)ch;
				if (code >= 0 && code <= 255) {
					unicodeTextBuilder.Append(ch);
				}
				else
					unicodeTextBuilder.AppendFormat(@"\u{0} ", code);
			}
			return unicodeTextBuilder.ToString();
		}
	}
}
