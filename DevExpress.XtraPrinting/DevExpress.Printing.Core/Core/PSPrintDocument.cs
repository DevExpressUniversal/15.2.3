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

using DevExpress.Printing.Native;
using DevExpress.XtraPrinting.BrickExporters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
namespace DevExpress.XtraPrinting.Native {
	public class PSPrintDocument : PrintDocument, IPrintDocumentExtension {
		class PaperData {
			PaperSize paperSize;
			Margins minMargins;
			public PaperSize PaperSize { get { return paperSize; } }
			public Margins MinMargins { get { return minMargins; } set { minMargins = value; } }
			public PaperData(PaperSize paperSize) {
				this.paperSize = paperSize;
			}
		}
		Queue<int> pageIndices = new Queue<int>();
		Document doc;
		Color backColor;
		PrintingSystemBase ps;
		Dictionary<ReadonlyPageData, PaperData> paperData = new Dictionary<ReadonlyPageData, PaperData>();
		Func<bool> predicateMargins;
		string pageRange = string.Empty;
		int PageIndex { get { return pageIndices.Count > 0 ? pageIndices.Peek() : -1; } }
		Page CurrentPage { get { return doc.Pages[PageIndex]; } }
		public string PageRange {
			get { return pageRange; }
			set { pageRange = value; }
		}
		public PSPrintDocument(PrintingSystemBase ps, Color backColor, PrintController printController, PrinterSettings printerSettings, Func<bool> predicateMargins) {
			this.ps = ps;
			this.doc = ps.Document;
			this.backColor = backColor;
			this.DocumentName = ps.Document.Name;
			PrinterSettings = printerSettings;
			DefaultPageSettings = (PageSettings)PrinterSettings.DefaultPageSettings.Clone();
			ResetUserSetPageSettings();
			if(!string.IsNullOrEmpty(ps.PageSettings.PrinterName))
				PageSettingsHelper.SetPrinterName(PrinterSettings, ps.PageSettings.PrinterName);
			if(printController != null)
				PrintController = printController;
			this.predicateMargins = predicateMargins;
		}
		protected override void OnBeginPrint(PrintEventArgs e) {
			base.OnBeginPrint(e);
			paperData.Clear();
			if(PageSettingsHelper.PrinterExists) {
				foreach(Page page in ps.Document.Pages) {
					ReadonlyPageData pageData = page.PageData;
					if(!paperData.ContainsKey(pageData))
						paperData.Add(pageData, new PaperData(PageSizeInfo.GetPaperSize(pageData, PrinterSettings.PaperSizes)));
				}
			}
			switch(PrinterSettings.PrintRange) {
				case PrintRange.AllPages:
				case PrintRange.CurrentPage:
				case PrintRange.SomePages:
					pageIndices = new Queue<int>(PageRangeParser.GetIndices(PageRange, ps.Pages.Count));
					break;
				default:
					pageIndices = new Queue<int>(PageRangeParser.GetIndices(string.Empty, ps.Pages.Count));
					break;
			}
		}
		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e) {
			ReadonlyPageData pageData = CurrentPage.PageData;
			PaperSize paperSize = paperData.ContainsKey(pageData) ? paperData[pageData].PaperSize :
				new PaperSize(pageData.PaperName, pageData.Size.Width, pageData.Size.Height);
			PageSettingsHelper.ChangePageSettings(e.PageSettings, paperSize, pageData);
			this.ps.OnPrintProgress(new PrintProgressEventArgs(e, PageIndex));
			base.OnQueryPageSettings(e);
		}
		protected override void OnPrintPage(PrintPageEventArgs e) {
			base.OnPrintPage(e);
			Graphics graph = e.Graphics;
			if(graph == null || PageIndex < 0)
				e.HasMorePages = false;
			else {
				Margins minMargins = EnsureMinMargins(graph);
				if(minMargins == null) {
					e.Cancel = true;
					return;
				}
				MarginsF minMarginsF = new MarginsF(minMargins);
				using(GdiGraphics gdiGraphics = new GdiGraphics(graph, ps)) {
					PageExporter exporter = ps.ExportersFactory.GetExporter(CurrentPage) as PageExporter;
					try {
						exporter.IsPrinting = true;
						exporter.DrawPage(gdiGraphics, new PointF(-minMarginsF.Left, -minMarginsF.Top));
					} finally {
						exporter.IsPrinting = false;
					}
				}
				pageIndices.Dequeue();
				e.HasMorePages = PageIndex >= 0;
			}
		}
		Margins EnsureMinMargins(Graphics graph) {
			Page currentPage = CurrentPage;
			Margins minMargins = DeviceCaps.GetMinMargins(graph);
			ReadonlyPageData pageData = currentPage.PageData;
			if(!paperData.ContainsKey(pageData) || paperData[pageData].MinMargins != null)
				return minMargins;
			paperData[pageData].MinMargins = minMargins;
			if(DeviceCaps.CompareMargins(currentPage.Margins, minMargins) > 0)
				return minMargins;
			return predicateMargins != null && predicateMargins() ? minMargins : null;
		}
		void ResetUserSetPageSettings() {
			typeof(PrintDocument).InvokeMember("userSetPageSettings",
			   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, null,
			   this, new object[] { false });
		}
	}
}
