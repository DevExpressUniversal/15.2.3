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
using System.IO;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Export.Mht {
	#region MhtExporter
	public class MhtExporter : IMhtExporter {
		readonly DocumentModel documentModel;
		readonly MhtDocumentExporterOptions options;
		string packageId = String.Empty;
		MhtPartCollection parts;
		public MhtExporter(DocumentModel documentModel, MhtDocumentExporterOptions options) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.options = options;
			this.options.DisposeConvertedImagesImmediately = false;
		}
		public string Export() {
			ChunkedStringBuilder result = new ChunkedStringBuilder();
			using (ChunkedStringBuilderWriter writer = new ChunkedStringBuilderWriter(result)) {
				Export(writer);
			}
			return result.ToString();
		}
		public void Export(TextWriter writer) {
			this.packageId = CalculatePackageId();
			this.parts = new MhtPartCollection();
			IUriProvider uriProvider = new MhtUriProvider(this);
			IUriProviderService service = documentModel.GetService<IUriProviderService>();
			if (service != null)
				service.RegisterProvider(uriProvider);
			try {
				ExportCore(writer);
			}
			finally {
				writer.Flush();
				if (service != null)
					service.UnregisterProvider(uriProvider);
			}
		}
		protected internal virtual string CalculatePackageId() {
			return Guid.NewGuid().ToString();
		}
		protected internal virtual void ExportCore(TextWriter writer) {
			writer.WriteLine("MIME-Version: 1.0");
			writer.WriteLine("Content-Type: multipart/related;");
			writer.WriteLine(@"  type=""text/html"";");
			writer.WriteLine(@"  boundary=""----=_NextPart_" + packageId + "\"");
			writer.WriteLine();
			writer.WriteLine("This is a multi-part message in MIME format.");
			parts.Add(CreateHtmlPart());
			ExportParts(writer);
		}
		protected internal virtual void ExportParts(TextWriter writer) {
			int count = parts.Count;
			for (int i = 0; i < count; i++)
				ExportPart(parts[i], writer);
			writer.WriteLine("------=_NextPart_" + packageId + "--");
		}
		protected internal virtual void ExportPart(MhtPart part, TextWriter writer) {
			writer.WriteLine("------=_NextPart_" + packageId);
			string contentType = part.ContentType;
			if (!String.IsNullOrEmpty(part.ContentEncoding))
				contentType += "; charset=\"" + part.ContentEncoding + "\"";
			writer.WriteLine("Content-Type: " + contentType);
			writer.WriteLine("Content-Transfer-Encoding: " + part.ContentTransferEncoding);
			if (!String.IsNullOrEmpty(part.SourceUri))
				writer.WriteLine("Content-Location: " + part.SourceUri);
			writer.WriteLine();
			part.WriteContent(writer);
			writer.WriteLine();
			writer.WriteLine();
		}
		protected internal MhtPart CreateHtmlPart() {
			IHtmlExporter exporter = documentModel.InternalAPI.ExporterFactory.CreateHtmlExporter(documentModel, options);
			string htmlContent = exporter.Export();
			MhtPart result = new MhtPart();
			result.Content = ToQuotedPrintable(htmlContent);
			result.ContentTransferEncoding = "quoted-printable";
			result.ContentType = @"text/html";
			result.ContentEncoding = options.Encoding.WebName;
			result.SourceUri = String.Empty;
			return result;
		}
		protected internal void AddImagePart(OfficeImage image, string uri) {
			OfficeImageFormat format = image.RawFormat;
			string contentType = OfficeImage.GetContentType(format);
			if (String.IsNullOrEmpty(contentType))
				format = OfficeImageFormat.Png;
			ImageMhtPart part = new ImageMhtPart(image);
			part.ContentTransferEncoding = "base64";
			part.ContentType = OfficeImage.GetContentType(format);
			part.SourceUri = uri;
			this.parts.Add(part);
		}
		protected internal void AddCssPart(string css, string uri) {
			MhtPart part = new MhtPart();
			part.Content = ToQuotedPrintable(css);
			part.ContentTransferEncoding = "quoted-printable";
			part.ContentType = @"text/css; charset=""" + options.Encoding.WebName + "\"";
			part.SourceUri = uri;
			this.parts.Add(part);
		}
		public string ToQuotedPrintable(string value) {
			QuotedPrintableEncoding quotedEncoding = new QuotedPrintableEncoding();
			value = quotedEncoding.ToQuotedPrintableString(value, options.Encoding);
			value = quotedEncoding.ToMultilineQuotedPrintableString(value, 80);
			return value;
		}
	}
	#endregion
	#region MhtPart
	public class MhtPart {
		string contentType = String.Empty;
		string contentEncoding = String.Empty;
		string contentTransferEncoding = String.Empty;
		string contentId = String.Empty;
		string content = String.Empty;
		string sourceUri = String.Empty;
		public string ContentType { get { return contentType; } set { contentType = value; } }
		public string ContentEncoding { get { return contentEncoding; } set { contentEncoding = value; } }
		public string ContentTransferEncoding { get { return contentTransferEncoding; } set { contentTransferEncoding = value; } }
		public string ContentId { get { return contentId; } set { contentId = value; } }
		public string Content { get { return content; } set { content = value; } }
		public string SourceUri { get { return sourceUri; } set { sourceUri = value; } }
		public virtual void WriteContent(TextWriter writer) {
			writer.WriteLine(Content);
		}
	}
	#endregion
	#region ImageMhtPart
	public class ImageMhtPart : MhtPart {
		readonly OfficeImage image;
		public ImageMhtPart(OfficeImage image) {
			Guard.ArgumentNotNull(image, "image");
			this.image = image;
		}
		public override void WriteContent(TextWriter writer) {
			OfficeImageFormat format = image.RawFormat;
			string contentType = OfficeImage.GetContentType(format);
			if (String.IsNullOrEmpty(contentType))
				format = OfficeImageFormat.Png;
			byte[] buffer = new byte[60];
			Stream stream = image.GetImageBytesStreamSafe(format);
			for (; ; ) {
				int bytesRead = stream.Read(buffer, 0, buffer.Length);
				writer.WriteLine(Convert.ToBase64String(buffer, 0, bytesRead));
				if (bytesRead < buffer.Length)
					break;
			}
		}
	}
	#endregion
	#region MhtPartCollection
	public class MhtPartCollection : List<MhtPart> {
	}
	#endregion
	#region MhtUriProvider
	public class MhtUriProvider : IUriProvider {
		readonly MhtExporter exporter;
		int imageCounter;
		int cssCounter;
		public MhtUriProvider(MhtExporter exporter) {
			Guard.ArgumentNotNull(exporter, "exporter");
			this.exporter = exporter;
		}
		#region IUriProvider Members
		public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
			imageCounter++;
			string result = String.Format("image{0}", imageCounter);
			exporter.AddImagePart(image, result);
			return result;
		}
		public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
			cssCounter++;
			string result = String.Format("css{0}", cssCounter);
			exporter.AddCssPart(styleText, result);
			return result;
		}
		#endregion
	}
	#endregion
}
