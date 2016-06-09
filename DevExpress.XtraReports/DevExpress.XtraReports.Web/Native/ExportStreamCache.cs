#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using System.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native {
	public static class ExportStreamCache {
		public static ExportStreamInfo CreateExportStreamInfo(ExportOptionsBase options, string documentType, string responseContentDisposition) {
			return CreateExportStreamInfo(new NullPrintingSystemProvider(), options, documentType, responseContentDisposition);
		}
		public static ExportStreamInfo CreateExportStreamInfo(IPrintingSystemProvider psProvider, ExportOptionsBase options, string documentFormat, string responseContentDisposition) {
			var stream = new MemoryStream();
			var info = new ExportStreamInfo {
				Stream = stream,
				ResponseContentDisposition = responseContentDisposition,
				ReportName = psProvider.ReportName
			};
			psProvider.ForceLoadDocument();
			if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "pdf")) {
				psProvider.Do(x => x.ExportToPdf(stream, (PdfExportOptions)options));
				info.SetType("application/pdf", "pdf");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "xlsx")) {
				psProvider.Do(x => x.ExportToXlsx(stream, (XlsxExportOptions)options));
				info.SetType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "xls")) {
				psProvider.Do(x => x.ExportToXls(stream, (XlsExportOptions)options));
				info.SetType("application/vnd.ms-excel", "xls");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "rtf")) {
				psProvider.Do(x => x.ExportToRtf(stream, (RtfExportOptions)options));
				info.SetType("application/rtf", "rtf");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "mht")) {
				psProvider.Do(x => x.ExportToMht(stream, ExportOptionsHelper.ChangeMhtExportOptionsTitle((MhtExportOptions)options, info.ReportName)));
				info.SetType("message/rfc822", "mht");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "html")) {
				psProvider.Do(x => x.ExportToHtml(stream, ExportOptionsHelper.ChangeHtmlExportOptions((HtmlExportOptions)options, info.ReportName)));
				info.SetType("text/html", "html");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "txt")) {
				psProvider.Do(x => x.ExportToText(stream, (TextExportOptions)options));
				info.SetType("text/plain", "txt");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "csv")) {
				psProvider.Do(x => x.ExportToCsv(stream, (CsvExportOptions)options));
				info.SetType("text/plain", "csv");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "png")) {
				((ImageExportOptions)options).Format = ImageFormat.Png;
				psProvider.Do(x => x.ExportToImage(stream, (ImageExportOptions)options));
				info.SetType("image/png", "png");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "tiff")) {
				((ImageExportOptions)options).Format = ImageFormat.Tiff;
				psProvider.Do(x => x.ExportToImage(stream, (ImageExportOptions)options));
				info.SetType("image/tiff", "tiff");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "gif")) {
				((ImageExportOptions)options).Format = ImageFormat.Gif;
				psProvider.Do(x => x.ExportToImage(stream, (ImageExportOptions)options));
				info.SetType("image/gif", "gif");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "jpeg") || StringEqualsInvariantCultureIgnoreCase(documentFormat, "jpg")) {
				((ImageExportOptions)options).Format = ImageFormat.Jpeg;
				psProvider.Do(x => x.ExportToImage(stream, (ImageExportOptions)options));
				info.SetType("image/jpeg", "jpg");
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "bmp")) {
				((ImageExportOptions)options).Format = ImageFormat.Bmp;
				psProvider.Do(x => x.ExportToImage(stream, (ImageExportOptions)options));
				info.SetType("image/bmp", "bmp");
			} else
				throw new ArgumentException("documentType");
			stream.Position = 0;
			return info;
		}
		public static ExportOptionsBase CreateExportOptions(ExportOptions options, string documentFormat, bool singleFilePageByPage) {
			ExportOptionsBase res = null;
			if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "pdf")) {
				res = new PdfExportOptions();
				res.Assign(options.Pdf);
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "xlsx")) {
				var exportOptions = new XlsxExportOptions();
				exportOptions.Assign(options.Xlsx);
				FixXlsxExportOptions(exportOptions);
				res = exportOptions;
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "xls")) {
				var exportOptions = new XlsExportOptions();
				exportOptions.Assign(options.Xls);
				FixXlsExportOptions(exportOptions);
				res = exportOptions;
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "rtf")) {
				var exportOptions = new RtfExportOptions();
				exportOptions.Assign(options.Rtf);
				FixRtfExportOptions(exportOptions, singleFilePageByPage);
				res = exportOptions;
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "mht")) {
				res = new MhtExportOptions();
				res.Assign(options.Mht);
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "html")) {
				var exportOptions = new HtmlExportOptions();
				exportOptions.Assign(options.Html);
				FixHtmlExportOptions(exportOptions, singleFilePageByPage);
				res = exportOptions;
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "txt")) {
				res = new TextExportOptions();
				res.Assign(options.Text);
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "csv")) {
				res = new CsvExportOptions();
				res.Assign(options.Csv);
			} else if(StringEqualsInvariantCultureIgnoreCase(documentFormat, "png")
				|| StringEqualsInvariantCultureIgnoreCase(documentFormat, "tiff")
				|| StringEqualsInvariantCultureIgnoreCase(documentFormat, "gif")
				|| StringEqualsInvariantCultureIgnoreCase(documentFormat, "jpeg")
				|| StringEqualsInvariantCultureIgnoreCase(documentFormat, "jpg")
				|| StringEqualsInvariantCultureIgnoreCase(documentFormat, "bmp")) {
				var exportOptions = new ImageExportOptions();
				exportOptions.Assign(options.Image);
				FixImageExportOptions(exportOptions, singleFilePageByPage);
				res = exportOptions;
			}
			return res;
		}
		public static void WriteResponse(HttpResponse response, ExportStreamInfo info) {
			response.ClearContent();
			response.Buffer = true;
			response.Cache.SetCacheability(HttpCacheability.Private);
			if(IsSecureConnection() && RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion <= 8)
				PatchPragmaHeader(response);
			AssignResponseHeaders(new HttpResponseWrapper(response), info);
			info.Stream.CopyTo(response.OutputStream);
			Methods.EndResponse(response);
		}
		static bool IsSecureConnection() {
			var httpContext = HttpContext.Current;
			return httpContext != null && httpContext.Request.IsSecureConnection;
		}
		static void PatchPragmaHeader(HttpResponse response) {
			const string pragma = "Pragma";
			if(HttpRuntime.UsingIntegratedPipeline) {
				if(response.Headers[pragma] != null)
					response.Headers.Remove(pragma);
			}
			response.AppendHeader(pragma, string.Empty);
		}
		public static void AssignResponseHeaders(IHttpResponseWrapper response, ExportStreamInfo info) {
			response.ContentType = info.ContentType;
			string fileName = info.ReportName + "." + info.ExportType;
			response.AddHeader("Content-Disposition", ContentDispositionUtil.GetHeaderValue(info.ResponseContentDisposition, fileName));
		}
		public static void WriteTo(HttpResponse response, XtraReport report, string documentType, string exportType, ExportOptionsBase options) {
			if(report == null)
				return;
			if(options == null)
				options = CreateExportOptions(report.ExportOptions, documentType, report.PrintingSystem.Document.IsModified);
			var psProvider = new PrintingSystemProvider(report.PrintingSystem);
			ExportStreamInfo info = CreateExportStreamInfo(psProvider, options, documentType, DispositionTypeNames.Inline);
			if(!string.IsNullOrEmpty(exportType))
				info.ExportType = exportType;
			WriteResponse(response, info);
		}
		static bool StringEqualsInvariantCultureIgnoreCase(string a, string b) {
			return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
		static void FixImageExportOptions(ImageExportOptions exportOptions, bool singleFilePageByPage) {
			if(singleFilePageByPage || exportOptions.ExportMode == ImageExportMode.DifferentFiles) {
				exportOptions.ExportMode = ImageExportMode.SingleFilePageByPage;
			}
		}
		static void FixHtmlExportOptions(HtmlExportOptions exportOptions, bool singleFilePageByPage) {
			if(singleFilePageByPage || exportOptions.ExportMode == HtmlExportMode.DifferentFiles) {
				exportOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
			}
		}
		static void FixRtfExportOptions(RtfExportOptions exportOptions, bool singleFilePageByPage) {
			if(singleFilePageByPage) {
				exportOptions.ExportMode = RtfExportMode.SingleFilePageByPage;
			}
		}
		static void FixXlsExportOptions(XlsExportOptions exportOptions) {
			if(exportOptions.ExportMode == XlsExportMode.DifferentFiles) {
				exportOptions.ExportMode = XlsExportMode.SingleFile;
			}
		}
		static void FixXlsxExportOptions(XlsxExportOptions exportOptions) {
			if(exportOptions.ExportMode == XlsxExportMode.DifferentFiles) {
				exportOptions.ExportMode = XlsxExportMode.SingleFilePageByPage;
			}
		}
	}
}
