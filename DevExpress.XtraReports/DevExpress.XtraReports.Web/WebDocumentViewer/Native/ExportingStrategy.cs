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
using System.Globalization;
using System.Net.Mime;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	public class ExportingStrategy {
		struct ExportingNamingInfoSimple {
			public string ContentType { get; private set; }
			public string FileExtension { get; private set; }
			public ExportingNamingInfoSimple(string contentType, string fileExtension)
				: this() {
				ContentType = contentType;
				FileExtension = fileExtension;
			}
		}
		const string PrintPdfFormat = "printpdf";
		public virtual string GetContentDisposition(bool printable) {
			 return printable ? DispositionTypeNames.Inline : DispositionTypeNames.Attachment;
		}
		public virtual ExportingNamingInfo GenerateInfo(string format, ExportOptions options) {
			ExportingNamingInfoSimple info = GenerateInfoCore(format, options);
			string contentDisposition = GetContentDisposition(format == PrintPdfFormat);
			return new ExportingNamingInfo(info.ContentType, info.FileExtension, contentDisposition);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static ExportOptions GetFixedExportOptions(ExportOptions exportOptions, bool shouldClone = true) {
			ExportOptions currentExportOptions = exportOptions;
			if(shouldClone) {
				var clone = new ExportOptions();
				clone.Assign(exportOptions);
				currentExportOptions = clone;
			}
			FixHtmlExportOptions(currentExportOptions.Html);
			FixHtmlExportOptions(currentExportOptions.Mht);
			FixImageExportOptions(currentExportOptions.Image);
			FixXlsExportOptions(currentExportOptions.Xls);
			FixXlsxExportOptions(exportOptions.Xlsx);
			return currentExportOptions;
		}
		ExportingNamingInfoSimple GenerateInfoCore(string format, ExportOptions options) {
			options = GetFixedExportOptions(options, shouldClone: false);
			if(StringEquals(format, "pdf") || StringEquals(format, PrintPdfFormat)) {
				ExportToPdf(options.Pdf);
				return new ExportingNamingInfoSimple(MediaTypeNames.Application.Pdf, "pdf");
			} else if(StringEquals(format, "xlsx")) {
				ExportToXlsx(options.Xlsx);
				return new ExportingNamingInfoSimple("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
			} else if(StringEquals(format, "xls")) {
				ExportToXls(options.Xls);
				return new ExportingNamingInfoSimple("application/vnd.ms-excel", "xls");
			} else if(StringEquals(format, "rtf")) {
				ExportToRtf(options.Rtf);
				return new ExportingNamingInfoSimple(MediaTypeNames.Application.Rtf, "rtf");
			} else if(StringEquals(format, "mht")) {
				ExportToMht(options.Mht);
				return new ExportingNamingInfoSimple("message/rfc822", "mht");
			} else if(StringEquals(format, "html")) {
				ExportToHtml(options.Html);
				return new ExportingNamingInfoSimple(MediaTypeNames.Text.Html, "html");
			} else if(StringEquals(format, "txt")) {
				ExportToText(options.Text);
				return new ExportingNamingInfoSimple(MediaTypeNames.Text.Plain, "txt");
			} else if(StringEquals(format, "csv")) {
				ExportToCsv(options.Csv);
				return new ExportingNamingInfoSimple("text/csv", "csv");
			} else if(StringEquals(format, "image")) {
				ExportToImage(options.Image);
				var exportExtensionType = options.Image.Format.ToString().ToLower(CultureInfo.InvariantCulture);
				var contentType = "image/" + exportExtensionType;
				return new ExportingNamingInfoSimple(contentType, exportExtensionType);
			} else
				throw new ArgumentException("Unsupported format: " + format, "format");
		}
		protected virtual void ExportToImage(ImageExportOptions exportOptions) {
		}
		protected virtual void ExportToCsv(CsvExportOptions exportOptions) {
		}
		protected virtual void ExportToText(TextExportOptions exportOptions) {
		}
		protected virtual void ExportToHtml(HtmlExportOptions exportOptions) {
		}
		protected virtual void ExportToMht(MhtExportOptions exportOptions) {
		}
		protected virtual void ExportToRtf(RtfExportOptions exportOptions) {
		}
		protected virtual void ExportToXls(XlsExportOptions exportOptions) {
		}
		protected virtual void ExportToXlsx(XlsxExportOptions exportOptions) {
		}
		protected virtual void ExportToPdf(PdfExportOptions exportOptions) {
		}
		static bool StringEquals(string a, string b) {
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		static void FixImageExportOptions(ImageExportOptions exportOptions) {
			if(exportOptions.ExportMode == ImageExportMode.DifferentFiles) {
				exportOptions.ExportMode = ImageExportMode.SingleFilePageByPage;
			}
		}
		static void FixHtmlExportOptions(HtmlExportOptionsBase exportOptions) {
			exportOptions.EmbedImagesInHTML = true;
			if(exportOptions.ExportMode == HtmlExportMode.DifferentFiles) {
				exportOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
			}
		}
		static void FixXlsxExportOptions(XlsxExportOptions exportOptions) {
			if(exportOptions.ExportMode == XlsxExportMode.DifferentFiles) {
				exportOptions.ExportMode = XlsxExportMode.SingleFilePageByPage;
			}
		}
		static void FixXlsExportOptions(XlsExportOptions exportOptions) {
			if(exportOptions.ExportMode == XlsExportMode.DifferentFiles) {
				exportOptions.ExportMode = XlsExportMode.SingleFile;
			}
		}
	}
}
