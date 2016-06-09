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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraReports.UI;
using System.IO;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.Office.Services;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraReports.Native {
	class DocumentModelHelper {
		#region inner classes
		class DummyUriProvider : IUriProvider {
			public string CreateCssUri(string rootUri, string styleText, string relativeUri) {
				return "does_not_exist";
			}
			public string CreateImageUri(string rootUri, OfficeImage image, string relativeUri) {
				return "does_not_exist";
			}
		}
		#endregion
		DocumentModel documentModel;
		string path;
		public DocumentModelHelper()
			: this(XtraRichTextEditHelper.CreateDocumentModel()) {
		}
		public DocumentModelHelper(DocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public void Load(string path) {
			Load(path, ToStreamType(Path.GetExtension(path)));
		}
		public void Load(string path, XRRichTextStreamType streamType) {
			ValidateXRRichTextStreamType(streamType);
			using(Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				Load(stream, path, streamType);
			}
		}
		public void Load(Stream data, XRRichTextStreamType streamType) {
			Load(data, null, streamType);
		}
		protected virtual void Load(Stream data, string sourceUri, XRRichTextStreamType streamType) {
			ValidateXRRichTextStreamType(streamType);
			data.Seek(0, SeekOrigin.Begin);
			switch(streamType) {
				case XRRichTextStreamType.RtfText:
					XtraRichTextEditHelper.ImportRtfTextStreamToDocManager(data, documentModel);
					break;
				case XRRichTextStreamType.PlainText:
					LoadPlainText(data);
					break;
				case XRRichTextStreamType.HtmlText:
					XtraRichTextEditHelper.ImportHtmlTextStreamToDocManager(data, sourceUri, documentModel);
					break;
				case XRRichTextStreamType.XmlText:
					XtraRichTextEditHelper.ImportOpenXmlContent(data, documentModel);
					break;
			}
		}
		protected virtual void LoadPlainText(Stream data) {
			XtraRichTextEditHelper.ImportPlainTextStreamToDocManager(data, documentModel);
		}
		static XRRichTextStreamType ToStreamType(string extension) {
			switch(extension.ToLower()) {
				case ".txt":
					return XRRichTextStreamType.PlainText;
				case ".htm":
				case ".html":
					return XRRichTextStreamType.HtmlText;
				case ".docx":
					return XRRichTextStreamType.XmlText;
			}
			return XRRichTextStreamType.RtfText;
		}
		static void ValidateXRRichTextStreamType(XRRichTextStreamType streamType) {
			if(!Enum.IsDefined(typeof(XRRichTextStreamType), streamType)) {
				throw new InvalidEnumArgumentException("streamType", (int)streamType, typeof(XRRichTextStreamType));
			}
		}
		public void Save(string path) {
			Save(path, XRRichTextStreamType.RtfText);
		}
		public void Save(string path, XRRichTextStreamType streamType) {
			ValidateXRRichTextStreamType(streamType);
			using(Stream stream = File.Create(path)) {
				this.path = path;
				Save(stream, streamType);
				this.path = string.Empty;
			}
		}
		public void Save(Stream data, XRRichTextStreamType streamType) {
			ValidateXRRichTextStreamType(streamType);
			switch(streamType) {
				case XRRichTextStreamType.RtfText:
					XtraRichTextEditHelper.WriteRtfFromDocManagerToStream(data, documentModel);
					break;
				case XRRichTextStreamType.PlainText:
					XtraRichTextEditHelper.WritePlainTextFromDocManagerToStream(data, documentModel);
					break;
				case XRRichTextStreamType.HtmlText:
					SaveHtml(data);
					break;
			}
		}
		void SaveHtml(Stream data) {
			HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
			IUriProviderService uriProviderService = documentModel.GetService(typeof(IUriProviderService)) as IUriProviderService;
			if(string.IsNullOrEmpty(path) && uriProviderService != null) {
				IUriProvider uriProvider = new DummyUriProvider();
				uriProviderService.RegisterProvider(uriProvider);
				try {
					SaveHtmlCore(data, options);
					return;
				} finally {
					uriProviderService.UnregisterProvider(uriProvider);
				}
			}
			if(!string.IsNullOrEmpty(path))
				options.TargetUri = path;
			SaveHtmlCore(data, options);
		}
		void SaveHtmlCore(Stream data, HtmlDocumentExporterOptions options) {
			HtmlExporter exporter = new HtmlExporter(documentModel, options);
			string result = exporter.Export();
			byte[] bytes = options.Encoding.GetBytes(result);
			data.Write(bytes, 0, bytes.Length);
		}
		public string GetRtf() {
			return XtraRichTextEditHelper.GetRtfFromDocManager(documentModel);
		}
	}
}
