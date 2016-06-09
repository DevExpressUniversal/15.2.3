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
using System.Drawing.Imaging;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing {
	public abstract partial class LinkBase : DependencyObject, IDisposable {
		bool IsDocumentEmpty { get { return PrintingSystem.Document.PageCount == 0; } }
		#region Preview
		public FloatingContainer ShowPrintPreview(FrameworkElement owner) {
			return ShowPrintPreview(owner, null);
		}
		public FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner) {
			return ShowRibbonPrintPreview(owner, null);
		}
		public FloatingContainer ShowPrintPreview(FrameworkElement owner, string title) {
			return PrintHelper.ShowPrintPreview(owner, this, title);
		}
		public FloatingContainer ShowRibbonPrintPreview(FrameworkElement owner, string title) {
			return PrintHelper.ShowRibbonPrintPreview(owner, this, title);
		}
		public Window ShowPrintPreview(Window owner) {
			return ShowPrintPreview(owner, null);
		}
		public Window ShowRibbonPrintPreview(Window owner) {
			return ShowRibbonPrintPreview(owner, null);
		}
		public Window ShowPrintPreview(Window owner, string title) {
			return PrintHelper.ShowPrintPreview(owner, this, title);
		}
		public Window ShowRibbonPrintPreview(Window owner, string title) {
			return PrintHelper.ShowRibbonPrintPreview(owner, this, title);
		}
		public void ShowPrintPreviewDialog(Window owner) {
			ShowPrintPreviewDialog(owner, null);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner) {
			ShowRibbonPrintPreviewDialog(owner, null);
		}
		public void ShowPrintPreviewDialog(Window owner, string title) {
			PrintHelper.ShowPrintPreviewDialog(owner, this, title);
		}
		public void ShowRibbonPrintPreviewDialog(Window owner, string title) {
			PrintHelper.ShowRibbonPrintPreviewDialog(owner, this, title);
		}
		#endregion
		#region Print
		#region Nested
		enum PrintMode { Dialog, Direct }
		class PrintingContext {
			readonly DocumentPrinter documentPrinter;
			readonly PrintMode printMode;
			readonly PrintQueue printQueue;
			public DocumentPrinter DocumentPrinter { get { return documentPrinter; } }
			public PrintMode PrintMode { get { return printMode; } }
			public PrintQueue PrintQueue { get { return printQueue; } }
			public PrintingContext(DocumentPrinter documentPrinter, PrintMode printMode, PrintQueue printQueue) {
				Guard.ArgumentNotNull(documentPrinter, "documentPrinter");
				this.documentPrinter = documentPrinter;
				this.printMode = printMode;
				this.printQueue = printQueue;
			}
			public void Print(DocumentPaginator paginator, ReadonlyPageData[] pageData, string documentName, bool asyncMode) {
				if(PrintMode == PrintMode.Dialog) {
					DocumentPrinter.PrintDialog(paginator, pageData, documentName, asyncMode);
				} else {
					if(PrintQueue == null)
						DocumentPrinter.PrintDirect(paginator, pageData, documentName, asyncMode);
					else
						DocumentPrinter.PrintDirect(paginator, pageData, documentName, PrintQueue, asyncMode);
				}
			}
		}
		#endregion
		PrintingContext ActualPrintingContext { get; set; }
		public void Print() {
			PrepareToPrint().PrintDialog(
				CreatePaginator(),
				PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()),
				PrintingSystem.Document.Name,
				false);
		}
		public void PrintDirect() {
			PrepareToPrint().PrintDirect(
				CreatePaginator(),
				PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()),
				PrintingSystem.Document.Name,
				false);
		}
		public void PrintDirect(PrintQueue queue) {
			PrepareToPrint().PrintDirect(
				CreatePaginator(),
				PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()),
				PrintingSystem.Document.Name,
				queue,
				false);
		}
		public event AsyncCompletedEventHandler PrintCompleted;
		public void PrintAsync() {
			PrintAsyncCore(PrintMode.Dialog, printQueue: null);
		}
		public void PrintDirectAsync() {
			PrintAsyncCore(PrintMode.Direct, printQueue: null);
		}
		public void PrintDirectAsync(PrintQueue queue) {
			PrintAsyncCore(PrintMode.Direct, printQueue: queue);
		}
		public void CancelPrintAsync() {
			if(ActualPrintingContext != null)
				ActualPrintingContext.DocumentPrinter.CancelPrint();
		}
		void PrintAsyncCore(PrintMode printMode, PrintQueue printQueue) {
			ActualPrintingContext = new PrintingContext(new DocumentPrinter(), printMode, printQueue);
			if(IsDocumentEmpty) {
				CreateDocumentFinished += LinkBase_CreateDocumentFinished;
				CreateDocument(true);
			} else {
				StartPrintAsync();
			}
		}
		void StartPrintAsync() {
			ActualPrintingContext.DocumentPrinter.PrintCompleted += printer_PrintCompleted;
			ActualPrintingContext.Print(CreatePaginator(), PrintHelper.CollectPageData(PrintingSystem.Pages.ToArray()), PrintingSystem.Document.Name, asyncMode: true);
		}
		void LinkBase_CreateDocumentFinished(object sender, EventArgs e) {
			CreateDocumentFinished -= LinkBase_CreateDocumentFinished;
			StartPrintAsync();
		}
		void printer_PrintCompleted(object sender, AsyncCompletedEventArgs e) {
			ActualPrintingContext.DocumentPrinter.PrintCompleted -= printer_PrintCompleted;
			ActualPrintingContext = null;
			if(PrintCompleted != null)
				PrintCompleted(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
		}
		DocumentPrinter PrepareToPrint() {
			CreateIfEmpty(false);
			return new DocumentPrinter();
		}
		internal DocumentPaginator CreatePaginator() {
			return new DelegatePaginator(VisualizePage, () => PrintingSystem.Document.Pages.Count);
		}
		#endregion
		#region Export
		public void ExportToCsv(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToCsv(stream);
		}
		public void ExportToCsv(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToCsv(filePath);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToCsv(stream, options);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToCsv(filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToHtml(stream);
		}
		public void ExportToHtml(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToHtml(filePath);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToHtml(stream, options);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToHtml(filePath, options);
		}
		public void ExportToImage(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(stream);
		}
		public void ExportToImage(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(filePath);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(stream, options);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(stream, format);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(filePath, options);
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToImage(filePath, format);
		}
		public void ExportToMht(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToMht(stream);
		}
		public void ExportToMht(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToMht(filePath);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToMht(stream, options);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToMht(filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToPdf(stream);
		}
		public void ExportToPdf(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToPdf(filePath);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToPdf(stream, options);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToPdf(filePath, options);
		}
		public void ExportToRtf(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToRtf(stream);
		}
		public void ExportToRtf(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToRtf(filePath);
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToRtf(stream, options);
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToRtf(filePath, options);
		}
		public void ExportToText(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToText(stream);
		}
		public void ExportToText(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToText(filePath);
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToText(stream, options);
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToText(filePath, options);
		}
		public void ExportToXls(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXls(stream);
		}
		public void ExportToXls(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXls(filePath);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXls(stream, options);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXls(filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXlsx(stream);
		}
		public void ExportToXlsx(string filePath) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXlsx(filePath);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXlsx(stream, options);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXlsx(filePath, options);
		}
		public void ExportToXps(Stream stream, XpsExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXps(stream, options);
		}
		public void ExportToXps(string filePath, XpsExportOptions options) {
			CreateIfEmpty(false);
			this.PrintingSystem.ExportToXps(filePath, options);
		}
		#endregion
	}
}
