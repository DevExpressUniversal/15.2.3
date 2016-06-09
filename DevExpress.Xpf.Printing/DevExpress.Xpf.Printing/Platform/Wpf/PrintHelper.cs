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

using System.ComponentModel;
using System.IO;
using System.Printing;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports;
namespace DevExpress.Xpf.Printing {
	public static class PrintHelper {
		#region Preview methods
		#region Floating Container
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintableControl source) {
			return ShowPrintPreview(owner, source, null, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintableControl source, string documentName) {
			return ShowPrintPreview(owner, source, documentName, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintableControl source, string documentName, string title) {
			return ShowPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, LinkBase link) {
			return ShowPrintPreview(owner, link, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, LinkBase link, string title) {
			return new LinkPreviewHelper(link).ShowDocumentPreview(owner, title);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IReport report) {
			return ShowPrintPreview(owner, report, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IReport report, string title) {
			return new ReportPreviewHelper(report).ShowDocumentPreview(owner, title);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintable source) {
			return ShowPrintPreview(owner, source, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintable source, string documentName) {
			return ShowPrintPreview(owner, source, documentName, null);
		}
		public static FloatingContainer ShowPrintPreview(FrameworkElement owner, IPrintable source, string documentName, string title) {
			return ShowPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintableControl source) {
			return ShowRibbonPrintPreview(owner, source, null, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintableControl source, string documentName) {
			return ShowRibbonPrintPreview(owner, source, documentName, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintableControl source, string documentName, string title) {
			return ShowRibbonPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, LinkBase link) {
			return ShowRibbonPrintPreview(owner, link, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, LinkBase link, string title) {
			return new LinkPreviewHelper(link).ShowRibbonDocumentPreview(owner, title);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IReport report) {
			return ShowRibbonPrintPreview(owner, report, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IReport report, string title) {
			return new ReportPreviewHelper(report).ShowRibbonDocumentPreview(owner, title);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintable source) {
			return ShowRibbonPrintPreview(owner, source, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintable source, string documentName) {
			return ShowRibbonPrintPreview(owner, source, documentName, null);
		}
		public static FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, IPrintable source, string documentName, string title) {
			return ShowRibbonPrintPreview(owner, CreateLink(source, documentName), title);
		}
		#endregion
		#region Window
		public static Window ShowPrintPreview(Window owner, IPrintableControl source) {
			return ShowPrintPreview(owner, source, null);
		}
		public static Window ShowPrintPreview(Window owner, IPrintableControl source, string documentName) {
			return ShowPrintPreview(owner, source, documentName, null);
		}
		public static Window ShowPrintPreview(Window owner, IPrintableControl source, string documentName, string title) {
			return ShowPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static Window ShowPrintPreview(Window owner, LinkBase link) {
			return ShowPrintPreview(owner, link, null);
		}
		public static Window ShowPrintPreview(Window owner, LinkBase link, string title) {
			return new LinkPreviewHelper(link).ShowDocumentPreview(owner, title);
		}
		public static Window ShowPrintPreview(Window owner, IReport report) {
			return ShowPrintPreview(owner, report, null);
		}
		public static Window ShowPrintPreview(Window owner, IReport report, string title) {
			return new ReportPreviewHelper(report).ShowDocumentPreview(owner, title);
		}
		public static Window ShowPrintPreview(Window owner, IPrintable source) {
			return ShowPrintPreview(owner, source, null);
		}
		public static Window ShowPrintPreview(Window owner, IPrintable source, string documentName) {
			return ShowPrintPreview(owner, source, documentName, null);
		}
		public static Window ShowPrintPreview(Window owner, IPrintable source, string documentName, string title) {
			return ShowPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintableControl source) {
			return ShowRibbonPrintPreview(owner, source, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintableControl source, string documentName) {
			return ShowRibbonPrintPreview(owner, source, documentName, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintableControl source, string documentName, string title) {
			return ShowRibbonPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static Window ShowRibbonPrintPreview(Window owner, LinkBase link) {
			return ShowRibbonPrintPreview(owner, link, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, LinkBase link, string title) {
			return new LinkPreviewHelper(link).ShowRibbonDocumentPreview(owner, title);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IReport report) {
			return ShowRibbonPrintPreview(owner, report, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IReport report, string title) {
			return new ReportPreviewHelper(report).ShowRibbonDocumentPreview(owner, title);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintable source) {
			return ShowRibbonPrintPreview(owner, source, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintable source, string documentName) {
			return ShowRibbonPrintPreview(owner, source, documentName, null);
		}
		public static Window ShowRibbonPrintPreview(Window owner, IPrintable source, string documentName, string title) {
			return ShowRibbonPrintPreview(owner, CreateLink(source, documentName), title);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl source) {
			ShowPrintPreviewDialog(owner, source, null, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl source, string documentName) {
			ShowPrintPreviewDialog(owner, source, documentName, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintableControl source, string documentName, string title) {
			using(LinkBase link = CreateLink(source, documentName)) {
				ShowPrintPreviewDialog(owner, link, title);
			}
		}
		public static void ShowPrintPreviewDialog(Window owner, LinkBase link) {
			ShowPrintPreviewDialog(owner, link, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, LinkBase link, string title) {
			new LinkPreviewHelper(link).ShowDocumentPreviewDialog(owner, title);
		}
		public static void ShowPrintPreviewDialog(Window owner, IReport report) {
			ShowPrintPreviewDialog(owner, report, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, IReport report, string title) {
			new ReportPreviewHelper(report).ShowDocumentPreviewDialog(owner, title);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintable source) {
			ShowPrintPreview(owner, source, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintable source, string documentName) {
			ShowPrintPreview(owner, source, documentName, null);
		}
		public static void ShowPrintPreviewDialog(Window owner, IPrintable source, string documentName, string title) {
			using(LinkBase link = CreateLink(source, documentName)) {
				ShowPrintPreviewDialog(owner, link, title);
			}
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintableControl source) {
			ShowRibbonPrintPreviewDialog(owner, source, null, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintableControl source, string documentName) {
			ShowRibbonPrintPreviewDialog(owner, source, documentName, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintableControl source, string documentName, string title) {
			using(LinkBase link = CreateLink(source, documentName)) {
				ShowRibbonPrintPreviewDialog(owner, link, title);
			}
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, LinkBase link) {
			ShowRibbonPrintPreviewDialog(owner, link, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, LinkBase link, string title) {
			new LinkPreviewHelper(link).ShowRibbonDocumentPreviewDialog(owner, title);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IReport report) {
			ShowRibbonPrintPreviewDialog(owner, report, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IReport report, string title) {
			new ReportPreviewHelper(report).ShowRibbonDocumentPreviewDialog(owner, title);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintable source) {
			ShowRibbonPrintPreview(owner, source, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintable source, string documentName) {
			ShowRibbonPrintPreview(owner, source, documentName, null);
		}
		public static void ShowRibbonPrintPreviewDialog(Window owner, IPrintable source, string documentName, string title) {
			using(LinkBase link = CreateLink(source, documentName)) {
				ShowRibbonPrintPreviewDialog(owner, link, title);
			}
		}
		#endregion
		static PrintableControlLink CreateLink(IPrintableControl source, string documentName) {
			return new PrintableControlLink(source) { DocumentName = documentName };
		}
		static LegacyPrintableComponentLink CreateLink(IPrintable source, string documentName) {
			return new LegacyPrintableComponentLink(source, documentName);
		}
		#endregion
		#region Print methods
		public static void Print(IPrintableControl source) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.Print();
			}
		}
		public static void Print(IReport source) {
			var model = new XtraReportPreviewModel(source);
			model.PrintDialog();
		}
		public static void Print(IPrintable source) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.Print();
			}
		}
		public static void PrintDirect(IPrintableControl source) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.PrintDirect();
			}
		}
		public static void PrintDirect(IReport source) {
			var model = new XtraReportPreviewModel(source);
			model.PrintDirect();
		}
		public static void PrintDirect(IPrintable source) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.PrintDirect();
			}
		}
		public static void PrintDirect(IPrintableControl source, PrintQueue queue) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.PrintDirect(queue);
			}
		}
		public static void PrintDirect(IReport source, PrintQueue queue) {
			var model = new XtraReportPreviewModel(source);
			model.PrintDirect(queue);
		}
		public static void PrintDirect(IPrintable source, PrintQueue queue) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.PrintDirect(queue);
			}
		}
#if !SL
		internal static ReadonlyPageData[] CollectPageData(Page[] pages) {
			ReadonlyPageData[] pageSettings = new ReadonlyPageData[pages.Length];
			for(int i = 0; i < pages.Length; i++) {
				pageSettings[i] = pages[i].PageData;
			}
			return pageSettings;
		}
		static bool usePrintTickets = true;
		public static bool UsePrintTickets {
			get { return usePrintTickets; }
			set { usePrintTickets = value; }
		}
#endif
		#region Asynchronous Printing
		public static event AsyncCompletedEventHandler PrintCompleted;
		public static void PrintAsync(IPrintableControl source) {
			PrintableControlLink link = new PrintableControlLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintAsync();
		}
		public static void PrintAsync(IPrintable source) {
			LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintAsync();
		}
		public static void PrintDirectAsync(IPrintableControl source) {
			PrintableControlLink link = new PrintableControlLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintDirectAsync();
		}
		public static void PrintDirectAsync(IPrintable source) {
			LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintDirectAsync();
		}
		public static void PrintDirectAsync(IPrintableControl source, PrintQueue queue) {
			PrintableControlLink link = new PrintableControlLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintDirectAsync(queue);
		}
		public static void PrintDirectAsync(IPrintable source, PrintQueue queue) {
			LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source);
			link.PrintCompleted += link_PrintCompleted;
			link.PrintDirectAsync(queue);
		}
		static void link_PrintCompleted(object sender, AsyncCompletedEventArgs e) {
			LinkBase link = (LinkBase)sender;
			link.PrintCompleted -= link_PrintCompleted;
			link.Dispose();
			if(PrintCompleted != null)
				PrintCompleted(null, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
		}
		#endregion
		#endregion
		#region Export methods
		#region CSV
		public static void ExportToCsv(IPrintableControl source, Stream stream) {
			ExportToCsv(source, stream, new CsvExportOptions());
		}
		public static void ExportToCsv(IPrintableControl source, Stream stream, CsvExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToCsv(stream, options);
			}
		}
		public static void ExportToCsv(IPrintableControl source, string filePath) {
			ExportToCsv(source, filePath, new CsvExportOptions());
		}
		public static void ExportToCsv(IPrintableControl source, string filePath, CsvExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToCsv(filePath, options);
			}
		}
		public static void ExportToCsv(IPrintable source, Stream stream) {
			ExportToCsv(source, stream, new CsvExportOptions());
		}
		public static void ExportToCsv(IPrintable source, Stream stream, CsvExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToCsv(stream, options);
			}
		}
		public static void ExportToCsv(IPrintable source, string filePath) {
			ExportToCsv(source, filePath, new CsvExportOptions());
		}
		public static void ExportToCsv(IPrintable source, string filePath, CsvExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToCsv(filePath, options);
			}
		}
		#endregion
		#region HTML
		public static void ExportToHtml(IPrintableControl source, Stream stream) {
			ExportToHtml(source, stream, new HtmlExportOptions());
		}
		public static void ExportToHtml(IPrintableControl source, Stream stream, HtmlExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToHtml(stream, options);
			}
		}
		public static void ExportToHtml(IPrintableControl source, string filePath) {
			ExportToHtml(source, filePath, new HtmlExportOptions());
		}
		public static void ExportToHtml(IPrintableControl source, string filePath, HtmlExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToHtml(filePath, options);
			}
		}
		public static void ExportToHtml(IPrintable source, Stream stream) {
			ExportToHtml(source, stream, new HtmlExportOptions());
		}
		public static void ExportToHtml(IPrintable source, Stream stream, HtmlExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToHtml(stream, options);
			}
		}
		public static void ExportToHtml(IPrintable source, string filePath) {
			ExportToHtml(source, filePath, new HtmlExportOptions());
		}
		public static void ExportToHtml(IPrintable source, string filePath, HtmlExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToHtml(filePath, options);
			}
		}
		#endregion
		#region Image
		public static void ExportToImage(IPrintableControl source, Stream stream) {
			ExportToImage(source, stream, new ImageExportOptions());
		}
		public static void ExportToImage(IPrintableControl source, Stream stream, ImageExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToImage(stream, options);
			}
		}
		public static void ExportToImage(IPrintableControl source, string filePath) {
			ExportToImage(source, filePath, new ImageExportOptions());
		}
		public static void ExportToImage(IPrintableControl source, string filePath, ImageExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToImage(filePath, options);
			}
		}
		public static void ExportToImage(IPrintable source, Stream stream) {
			ExportToImage(source, stream, new ImageExportOptions());
		}
		public static void ExportToImage(IPrintable source, Stream stream, ImageExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToImage(stream, options);
			}
		}
		public static void ExportToImage(IPrintable source, string filePath) {
			ExportToImage(source, filePath, new ImageExportOptions());
		}
		public static void ExportToImage(IPrintable source, string filePath, ImageExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToImage(filePath, options);
			}
		}
		#endregion
		#region MHT
		public static void ExportToMht(IPrintableControl source, Stream stream) {
			ExportToMht(source, stream, new MhtExportOptions());
		}
		public static void ExportToMht(IPrintableControl source, Stream stream, MhtExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToMht(stream, options);
			}
		}
		public static void ExportToMht(IPrintableControl source, string filePath) {
			ExportToMht(source, filePath, new MhtExportOptions());
		}
		public static void ExportToMht(IPrintableControl source, string filePath, MhtExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToMht(filePath, options);
			}
		}
		public static void ExportToMht(IPrintable source, Stream stream) {
			ExportToMht(source, stream, new MhtExportOptions());
		}
		public static void ExportToMht(IPrintable source, Stream stream, MhtExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToMht(stream, options);
			}
		}
		public static void ExportToMht(IPrintable source, string filePath) {
			ExportToMht(source, filePath, new MhtExportOptions());
		}
		public static void ExportToMht(IPrintable source, string filePath, MhtExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToMht(filePath, options);
			}
		}
		#endregion
		#region PDF
		public static void ExportToPdf(IPrintableControl source, Stream stream) {
			ExportToPdf(source, stream, new PdfExportOptions());
		}
		public static void ExportToPdf(IPrintableControl source, Stream stream, PdfExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToPdf(stream, options);
			}
		}
		public static void ExportToPdf(IPrintableControl source, string filePath) {
			ExportToPdf(source, filePath, new PdfExportOptions());
		}
		public static void ExportToPdf(IPrintableControl source, string filePath, PdfExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToPdf(filePath, options);
			}
		}
		public static void ExportToPdf(IPrintable source, Stream stream) {
			ExportToPdf(source, stream, new PdfExportOptions());
		}
		public static void ExportToPdf(IPrintable source, Stream stream, PdfExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToPdf(stream, options);
			}
		}
		public static void ExportToPdf(IPrintable source, string filePath) {
			ExportToPdf(source, filePath, new PdfExportOptions());
		}
		public static void ExportToPdf(IPrintable source, string filePath, PdfExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToPdf(filePath, options);
			}
		}
		#endregion
		#region RTF
		public static void ExportToRtf(IPrintableControl source, Stream stream) {
			ExportToRtf(source, stream, new RtfExportOptions());
		}
		public static void ExportToRtf(IPrintableControl source, Stream stream, RtfExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToRtf(stream, options);
			}
		}
		public static void ExportToRtf(IPrintableControl source, string filePath) {
			ExportToRtf(source, filePath, new RtfExportOptions());
		}
		public static void ExportToRtf(IPrintableControl source, string filePath, RtfExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToRtf(filePath, options);
			}
		}
		public static void ExportToRtf(IPrintable source, Stream stream) {
			ExportToRtf(source, stream, new RtfExportOptions());
		}
		public static void ExportToRtf(IPrintable source, Stream stream, RtfExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToRtf(stream, options);
			}
		}
		public static void ExportToRtf(IPrintable source, string filePath) {
			ExportToRtf(source, filePath, new RtfExportOptions());
		}
		public static void ExportToRtf(IPrintable source, string filePath, RtfExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToRtf(filePath, options);
			}
		}
		#endregion
		#region Text
		public static void ExportToText(IPrintableControl source, Stream stream) {
			ExportToText(source, stream, new TextExportOptions());
		}
		public static void ExportToText(IPrintableControl source, Stream stream, TextExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToText(stream, options);
			}
		}
		public static void ExportToText(IPrintableControl source, string filePath) {
			ExportToText(source, filePath, new TextExportOptions());
		}
		public static void ExportToText(IPrintableControl source, string filePath, TextExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToText(filePath, options);
			}
		}
		public static void ExportToText(IPrintable source, Stream stream) {
			ExportToText(source, stream, new TextExportOptions());
		}
		public static void ExportToText(IPrintable source, Stream stream, TextExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToText(stream, options);
			}
		}
		public static void ExportToText(IPrintable source, string filePath) {
			ExportToText(source, filePath, new TextExportOptions());
		}
		public static void ExportToText(IPrintable source, string filePath, TextExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToText(filePath, options);
			}
		}
		#endregion
		#region XLS
		public static void ExportToXls(IPrintableControl source, Stream stream) {
			ExportToXls(source, stream, new XlsExportOptions());
		}
		public static void ExportToXls(IPrintableControl source, Stream stream, XlsExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXls(stream, options);
			}
		}
		public static void ExportToXls(IPrintableControl source, string filePath) {
			ExportToXls(source, filePath, new XlsExportOptions());
		}
		public static void ExportToXls(IPrintableControl source, string filePath, XlsExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXls(filePath, options);
			}
		}
		public static void ExportToXls(IPrintable source, Stream stream) {
			ExportToXls(source, stream, new XlsExportOptions());
		}
		public static void ExportToXls(IPrintable source, Stream stream, XlsExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXls(stream, options);
			}
		}
		public static void ExportToXls(IPrintable source, string filePath) {
			ExportToXls(source, filePath, new XlsExportOptions());
		}
		public static void ExportToXls(IPrintable source, string filePath, XlsExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXls(filePath, options);
			}
		}
		#endregion
		#region XLSX
		public static void ExportToXlsx(IPrintableControl source, Stream stream) {
			ExportToXlsx(source, stream, new XlsxExportOptions());
		}
		public static void ExportToXlsx(IPrintableControl source, Stream stream, XlsxExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXlsx(stream, options);
			}
		}
		public static void ExportToXlsx(IPrintableControl source, string filePath) {
			ExportToXlsx(source, filePath, new XlsxExportOptions());
		}
		public static void ExportToXlsx(IPrintableControl source, string filePath, XlsxExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXlsx(filePath, options);
			}
		}
		public static void ExportToXlsx(IPrintable source, Stream stream) {
			ExportToXlsx(source, stream, new XlsxExportOptions());
		}
		public static void ExportToXlsx(IPrintable source, Stream stream, XlsxExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXlsx(stream, options);
			}
		}
		public static void ExportToXlsx(IPrintable source, string filePath) {
			ExportToXlsx(source, filePath, new XlsxExportOptions());
		}
		public static void ExportToXlsx(IPrintable source, string filePath, XlsxExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXlsx(filePath, options);
			}
		}
		#endregion
		#region XPS
		public static void ExportToXps(IPrintableControl source, Stream stream) {
			ExportToXps(source, stream, new XpsExportOptions());
		}
		public static void ExportToXps(IPrintableControl source, Stream stream, XpsExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXps(stream, options);
			}
		}
		public static void ExportToXps(IPrintableControl source, string filePath) {
			ExportToXps(source, filePath, new XpsExportOptions());
		}
		public static void ExportToXps(IPrintableControl source, string filePath, XpsExportOptions options) {
			using(PrintableControlLink link = new PrintableControlLink(source)) {
				link.ExportToXps(filePath, options);
			}
		}
		public static void ExportToXps(IPrintable source, Stream stream) {
			ExportToXps(source, stream, new XpsExportOptions());
		}
		public static void ExportToXps(IPrintable source, Stream stream, XpsExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXps(stream, options);
			}
		}
		public static void ExportToXps(IPrintable source, string filePath) {
			ExportToXps(source, filePath, new XpsExportOptions());
		}
		public static void ExportToXps(IPrintable source, string filePath, XpsExportOptions options) {
			using(LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(source)) {
				link.ExportToXps(filePath, options);
			}
		}
		#endregion
		#endregion
	}
}
