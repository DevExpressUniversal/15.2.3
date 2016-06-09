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
using System.IO;
using System.Security;
using System.Runtime;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
using DevExpress.XtraPrinting;
namespace DevExpress.Pdf.Drawing {
	public class PdfDocumentPrinter : PdfDisposableObject {
		public static void Print(PdfDocumentState documentState, string documentFilePath, int currentPageNumber, PdfPrinterSettings printerSettings, bool showPrintStatusDialog) {
			using (PdfDocumentPrinter printer = new PdfDocumentPrinter(documentState, documentFilePath, currentPageNumber, printerSettings, showPrintStatusDialog)) {
				if (printer.pageIndexes.Count > 0) {
					PrintDocument printDocument = printer.printDocument;
					try {
						printDocument.Print();
					}
					catch (Win32Exception) {
						PrintEventArgs args = new PrintEventArgs();
						args.Cancel = true;
						printDocument.PrintController.OnEndPrint(printDocument, args);
					}
				}
			}
		}
		readonly PdfDocumentState documentState;
		readonly IList<PdfPage> pages;
		readonly IList<int> pageIndexes;
		readonly PdfPrintDocumentCalculator calculator;
		readonly int copyCount;
		PrintDocument printDocument;
		int currentIndex;
		int currentCopy = 1;
		PdfDocumentPrinter(PdfDocumentState documentState, string documentFilePath, int currentPageNumber, PdfPrinterSettings printerSettings, bool showPrintStatusDialog) {
			this.documentState = documentState;
			PdfDocument document = documentState.Document;
			this.pages = document.Pages;
			PrinterSettings settings = printerSettings.Settings;
			printDocument = new PrintDocument();
			printDocument.PrinterSettings = settings;
			string documentName = document.Title;
			if (String.IsNullOrEmpty(documentName))
				documentName = String.IsNullOrEmpty(documentFilePath) ? PdfCoreLocalizer.GetString(PdfCoreStringId.DefaultDocumentName) : Path.GetFileName(documentFilePath);
			printDocument.DocumentName = documentName;
			if (settings.MaximumCopies < settings.Copies) {
				copyCount = settings.Copies;
				settings.Copies = 1;
				printDocument.PrintPage += new PrintPageEventHandler(ManualPrintPage);
			}
			else
				printDocument.PrintPage += new PrintPageEventHandler(AutoPrintPage);
			printDocument.QueryPageSettings += new QueryPageSettingsEventHandler(OnQueryPageSettings);
			pageIndexes = GetValidPageIndexes(printerSettings.GetPageNumbers(currentPageNumber, pages.Count));
			if (pageIndexes.Count > 0)
				calculator = new PdfPrintDocumentCalculator(printerSettings, pages[pageIndexes[0]], documentState.RotationAngle);
			if (!showPrintStatusDialog)
				printDocument.PrintController = new StandardPrintController();
		}
		void PrintCurrentPage(Graphics graphics) {
			int pageIndex = pageIndexes[currentIndex];
			calculator.CalculatePrintingDpi(GraphicsDpi.GetGraphicsDpi(graphics));
			IEnumerable<Rectangle> rectangles = calculator.GetRectangles(GetDivisionCount(calculator.Rectangle));
			float printingDpi = calculator.PrinterSettings.PrintingDpi;
			foreach (Rectangle rect in rectangles)
				if (calculator.IsDrawOnGraphics)
					PdfViewerCommandInterpreter.Draw(documentState, pageIndex, calculator.PrintScale, calculator.GetOffset(rect), PdfRenderMode.Print, graphics, rect);
				else
					using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height)) {
						bitmap.SetResolution(printingDpi, printingDpi);
						using (Graphics imageGraphics = Graphics.FromImage(bitmap))
							PdfViewerCommandInterpreter.Draw(documentState, pageIndex, calculator.PrintScale, calculator.GetOffset(rect), PdfRenderMode.Print, imageGraphics, new Rectangle(0, 0, rect.Width, rect.Height));
						graphics.DrawImage(bitmap, calculator.GetBitmapPosition(rect));
					}
		}
		[SecuritySafeCritical]
		int GetDivisionCount(Rectangle imageRect) {
			int mb = 1024 * 1024;
			int colorByteSize = 4;
			double maxImageSize = 512;
			double imageMemoryUsage = Math.Ceiling(3D * colorByteSize * imageRect.Width * imageRect.Height / mb);
			int divisionCount = (int)Math.Ceiling(imageMemoryUsage / maxImageSize);
			for (; ; ) {
				if (divisionCount < 1)
					return 1;
				int memUsage = Convert.ToInt32(imageMemoryUsage / divisionCount);
				if (memUsage < 16)
					break;
				try {
					new MemoryFailPoint(memUsage).Dispose();
					break;
				}
				catch (InsufficientMemoryException) {
					divisionCount *= 2;
				}
			}
			return divisionCount;
		}
		void ManualPrintPage(object sender, PrintPageEventArgs e) {
			currentIndex = Math.Max(currentIndex, 0);
			PrintCurrentPage(e.Graphics);
			if (e.PageSettings.PrinterSettings.Collate)
				if (++currentIndex == pageIndexes.Count) {
					currentIndex = 0;
					e.HasMorePages = ++currentCopy <= copyCount;
				}
				else
					e.HasMorePages = true;
			else if (++currentCopy > copyCount) {
				currentCopy = 1;
				e.HasMorePages = ++currentIndex < pageIndexes.Count;
			}
			else
				e.HasMorePages = true;
		}
		void AutoPrintPage(object sender, PrintPageEventArgs e) {
			currentIndex = Math.Max(currentIndex, 0);
			PrintCurrentPage(e.Graphics);
			e.HasMorePages = ++currentIndex < pageIndexes.Count;
		}
		void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e) {
			int pageIndex = pageIndexes[currentIndex];
			PdfPage page = pages[pageIndex];
			page.EnsurePage();
			calculator.Page = page;
			calculator.PageSettings = e.PageSettings;
		}
		IList<int> GetValidPageIndexes(int[] pageNumbers) {
			List<int> existingPageIndexes = new List<int>();
			int pagesCount = pages.Count;
			foreach (int pageNumber in pageNumbers)
				if (pageNumber > 0 && pageNumber <= pagesCount)
					existingPageIndexes.Add(pageNumber - 1);
			return existingPageIndexes;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (printDocument != null) {
					printDocument.PrintPage -= copyCount == 0 ? new PrintPageEventHandler(AutoPrintPage) : new PrintPageEventHandler(ManualPrintPage);
					printDocument.QueryPageSettings -= new QueryPageSettingsEventHandler(OnQueryPageSettings);
					printDocument.Dispose();
					printDocument = null;
				}
			}
		}
	}
}
