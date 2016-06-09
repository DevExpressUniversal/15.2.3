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
using System.Collections;
using System.IO;
using System.Windows;
using DevExpress.Xpf.PivotGrid.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
using System.Collections.Generic;
using DevExpress.Xpf.Printing.Native;
#if !SL
using System.Printing;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public partial class PivotGridControl : IPrintableControl {
		protected internal PivotPrintHelper PrintHelper {
			get {
				if(printHelper == null)
					printHelper = CreatePrintHelper();
				return printHelper;
			}
		}
		protected virtual PivotPrintHelper CreatePrintHelper() {
			return new PivotPrintHelper(this);
		}
#if !SL
		protected virtual PrintableControlLink CreateLink() {
			PrintableControlLink link = new PrintableControlLink(this);
			link.VerticalContentSplitting = VerticalContentSplitting.Smart;
			return link;
		}
		PrintableControlLink CreateLink(string documentName) {
			PrintableControlLink link = CreateLink();
			link.DocumentName = documentName;
			return link;
		}
		void SetDesiredPrintLayoutMode(bool paged) {
			DesiredPrintLayoutMode = paged ? PrintLayoutMode.MultiplePagesLayout : PrintLayoutMode.SinglePageLayout;
		}
		#region Preview methods
		public void ShowPrintPreview(FrameworkElement owner) {
			ShowPrintPreview(owner, null, null);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName) {
			ShowPrintPreview(owner, documentName, null);
		}
		public void ShowPrintPreview(FrameworkElement owner, string documentName, string title) {
			SetDesiredPrintLayoutMode(true);
			CreateLink(documentName).ShowPrintPreview(owner, title);
		}
		public void ShowPrintPreview(Window owner) {
			ShowPrintPreview(owner, null);
		}
		public void ShowPrintPreview(Window owner, string documentName) {
			ShowPrintPreview(owner, documentName, null);
		}
		public void ShowPrintPreview(Window owner, string documentName, string title) {
			SetDesiredPrintLayoutMode(true);
			CreateLink(documentName).ShowPrintPreview(owner, title);
		}
		public void ShowPrintPreviewDialog(Window owner) {
			ShowPrintPreviewDialog(owner, null, null);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName) {
			ShowPrintPreviewDialog(owner, documentName, null);
		}
		public void ShowPrintPreviewDialog(Window owner, string documentName, string title) {
			SetDesiredPrintLayoutMode(true);
			using(DevExpress.Xpf.Printing.LinkBase link = CreateLink(documentName)) {
				link.ShowPrintPreviewDialog(owner, title);
			}
		}
		#endregion
		#region Print methods
		public void Print() {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.Print();
			}
		}
		public void PrintDirect() {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.PrintDirect();
			}
		}
		public void PrintDirect(PrintQueue queue) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.PrintDirect(queue);
			}
		}
		#endregion
		#region Export methods
		#region CSV
		public void ExportToCsv(Stream stream) {
			ExportToCsv(stream, new CsvExportOptions());
		}
		public void ExportToCsv(Stream stream, DevExpress.XtraPrinting.CsvExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToCsv(stream, options);
			}
		}
		public void ExportToCsv(string filePath) {
			ExportToCsv(filePath, new CsvExportOptions());
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToCsv(filePath, options);
			}
		}
		#endregion
		#region HTML
		public void ExportToHtml(Stream stream) {
			ExportToHtml(stream, new HtmlExportOptions());
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToHtml(stream, options);
			}
		}
		public void ExportToHtml(string filePath) {
			ExportToHtml(filePath, new HtmlExportOptions());
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToHtml(filePath, options);
			}
		}
		#endregion
		#region Image
		public void ExportToImage(Stream stream) {
			ExportToImage(stream, new ImageExportOptions());
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToImage(stream, options);
			}
		}
		public void ExportToImage(string filePath) {
			ExportToImage(filePath, new ImageExportOptions());
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToImage(filePath, options);
			}
		}
		#endregion
		#region MHT
		public void ExportToMht(Stream stream) {
			ExportToMht(stream, new MhtExportOptions());
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToMht(stream, options);
			}
		}
		public void ExportToMht(string filePath) {
			ExportToMht(filePath, new MhtExportOptions());
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToMht(filePath, options);
			}
		}
		#endregion
		#region PDF
		public void ExportToPdf(Stream stream) {
			ExportToPdf(stream, new PdfExportOptions());
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToPdf(stream, options);
			}
		}
		public void ExportToPdf(string filePath) {
			ExportToPdf(filePath, new PdfExportOptions());
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToPdf(filePath, options);
			}
		}
		#endregion
		#region RTF
		public void ExportToRtf(Stream stream) {
			ExportToRtf(stream, new RtfExportOptions());
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToRtf(stream, options);
			}
		}
		public void ExportToRtf(string filePath) {
			ExportToRtf(filePath, new RtfExportOptions());
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToRtf(filePath, options);
			}
		}
		#endregion
		#region Text
		public void ExportToText(Stream stream) {
			ExportToText(stream, new TextExportOptions());
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToText(stream, options);
			}
		}
		public void ExportToText(string filePath) {
			ExportToText(filePath, new TextExportOptions());
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToText(filePath, options);
			}
		}
		#endregion
		#region XLS
		public void ExportToXls(Stream stream) {
			ExportToXls(stream, new XlsExportOptions());
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXls(stream, options);
			}
		}
		public void ExportToXls(string filePath) {
			ExportToXls(filePath, new XlsExportOptions());
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXls(filePath, options);
			}
		}
		#endregion
		#region XLSX
		public void ExportToXlsx(Stream stream) {
			ExportToXlsx(stream, new XlsxExportOptions());
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXlsx(stream, options);
			}
		}
		public void ExportToXlsx(string filePath) {
			ExportToXlsx(filePath, new XlsxExportOptions());
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			SetDesiredPrintLayoutMode(false);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXlsx(filePath, options);
			}
		}
		#endregion
		#region XPS
		public void ExportToXps(Stream stream) {
			ExportToXps(stream, new XpsExportOptions());
		}
		public void ExportToXps(Stream stream, XpsExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXps(stream, options);
			}
		}
		public void ExportToXps(string filePath) {
			ExportToXps(filePath, new XpsExportOptions());
		}
		public void ExportToXps(string filePath, XpsExportOptions options) {
			SetDesiredPrintLayoutMode(true);
			using(PrintableControlLink link = CreateLink()) {
				link.ExportToXps(filePath, options);
			}
		}
		#endregion
		#endregion
#endif
		#region IPrintableControl Members
		bool IPrintableControl.CanCreateRootNodeAsync {
			get { return false; }
		}
		event EventHandler<DevExpress.Data.Utils.ServiceModel.ScalarOperationCompletedEventArgs<IRootDataNode>> IPrintableControl.CreateRootNodeCompleted {
			add { }
			remove { }
		}
		void IPrintableControl.CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			throw new NotImplementedException();
		}
		IRootDataNode IPrintableControl.CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return PrintHelper.CreateRootNode(new PrintSizeArgs(usablePageSize, reportHeaderSize, reportFooterSize, pageHeaderSize, pageFooterSize));
		}
		IVisualTreeWalker IPrintableControl.GetCustomVisualTreeWalker() {
			return new PivotVisualTreeWalker();
		}
		void IPrintableControl.PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
		}
		#endregion
	}
}
