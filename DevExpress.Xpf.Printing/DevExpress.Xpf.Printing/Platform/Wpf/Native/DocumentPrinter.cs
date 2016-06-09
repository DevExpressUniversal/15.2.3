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
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Serialization;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using PageOrientation = System.Printing.PageOrientation;
namespace DevExpress.Xpf.Printing.Native {
	class DocumentPrinter {
		XpsDocumentWriter writer;
		PrintTicket documentPrintTicket;
		ReadonlyPageData[] pageData;
		int startPageIndex;
		public bool Active { get { return writer != null; } }
		public event AsyncCompletedEventHandler PrintCompleted;
		public void CancelPrint() {
			if(writer != null)
				writer.CancelAsync();
		}
		public void PrintDirect(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, bool asyncMode) {
			PrintDirect(paginator, pageData, jobDescription, GetDefaultPrintQueue(), asyncMode);
		}
		[SecuritySafeCritical]
		public void PrintDirect(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, PrintQueue queue, bool asyncMode) {
			Guard.ArgumentNotNull(paginator, "paginator");
			Guard.ArgumentNotNull(pageData, "pageData");
			Guard.ArgumentNotNull(queue, "queue");
			if(paginator.PageCount == 0)
				throw new InvalidOperationException("A document must contain at least one page to be printed.");
			if(pageData.Length == 0)
				throw new ArgumentException("pageData can not be empty");
			if(Active)
				throw new InvalidOperationException("Can't start printing while another printing job is in progress");
			this.documentPrintTicket = queue.UserPrintTicket;
			this.pageData = pageData;
			queue.CurrentJobSettings.Description = GetJobDescriptionOrDefault(jobDescription);
			if(PrintHelper.UsePrintTickets)
				ApplyPageDataToTicket(queue.UserPrintTicket, pageData[0]);
			XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);
			Print(paginator, writer, asyncMode);
		}
		public bool PrintDialog(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, bool asyncMode) {
			Guard.ArgumentNotNull(paginator, "paginator");
			Guard.ArgumentNotNull(pageData, "pageData");
			if(paginator.PageCount == 0)
				throw new InvalidOperationException("A document must contain at least one page to be printed.");
			if(Active)
				throw new InvalidOperationException("Can't start printing while another printing job is in progress");
			if(pageData.Length == 0)
				throw new ArgumentException("pageData can not be empty");
			XpsDocumentWriter writer = null;
			PageRange pageRange = new PageRange();
			PageRangeSelection pageRangeSelection = PageRangeSelection.AllPages;
			this.pageData = pageData;
			writer = ShowPrintDialog(pageData[0], jobDescription, ref pageRangeSelection, ref pageRange, (uint)paginator.PageCount);
			if(writer == null) {
				OnAsyncPrintJobCompleted(error: null, cancelled: true, userState: false);
				return false;
			}
			var effectivePaginator = pageRangeSelection == PageRangeSelection.AllPages ? paginator : new PageRangeCustomPaginator(paginator, GetPageIndexes(pageRange));
			startPageIndex = pageRangeSelection == PageRangeSelection.AllPages ? 0 : pageRange.PageFrom - 1;
			Print(effectivePaginator, writer, asyncMode);
			return true;
		}
		static int[] GetPageIndexes(PageRange pageRange) {
			List<int> pageIndexes = new List<int>();
			for(int pageIndex = pageRange.PageFrom; pageIndex <= pageRange.PageTo; pageIndex++) {
				pageIndexes.Add(pageIndex - 1);
			}
			return pageIndexes.ToArray();
		}
		[SecuritySafeCritical]
		XpsDocumentWriter ShowPrintDialog(ReadonlyPageData pageData, string jobDescription, ref PageRangeSelection pageRangeSelection, ref PageRange pageRange, uint maxPage) {
			PrintDialog dialog = new PrintDialog();
			dialog.UserPageRangeEnabled = true;
			dialog.MaxPage = maxPage;
			if(PrintHelper.UsePrintTickets)
				ApplyPageDataToTicket(dialog.PrintTicket, pageData);
			if(dialog.ShowDialog() == false)
				return null;
			dialog.PrintQueue.CurrentJobSettings.Description = GetJobDescriptionOrDefault(jobDescription);
			documentPrintTicket = dialog.PrintTicket;
			pageRangeSelection = dialog.PageRangeSelection;
			pageRange = dialog.PageRange;
			return PrintQueue.CreateXpsDocumentWriter(dialog.PrintQueue);
		}
		void Print(DocumentPaginator paginator, XpsDocumentWriter writer, bool asyncMode) {
			this.writer = writer;
			writer.WritingCompleted += writer_WritingCompleted;
			writer.WritingCancelled += writer_WritingCancelled;
			writer.WritingProgressChanged += writer_WritingProgressChanged;
			if(PrintHelper.UsePrintTickets)
				writer.WritingPrintTicketRequired += writer_WritingPrintTicketRequired;
			if(asyncMode) {
				try {
					writer.WriteAsync(paginator);
				} catch(PrintSystemException e) {
					OnAsyncPrintJobCompleted(e, cancelled: false, userState: null);
				}
			} else {
				try {
					writer.Write(paginator);
				} finally {
					OnPrintJobCompleted();
				}
			}
		}
		[SecuritySafeCritical]
		void writer_WritingPrintTicketRequired(object sender, WritingPrintTicketRequiredEventArgs e) {
			if(e.CurrentPrintTicketLevel == PrintTicketLevel.FixedDocumentSequencePrintTicket) {
				if(documentPrintTicket != null) {
					e.CurrentPrintTicket = documentPrintTicket;
				}
				return;
			}
			if(e.CurrentPrintTicketLevel == System.Windows.Xps.Serialization.PrintTicketLevel.FixedPagePrintTicket) {
				ReadonlyPageData currentPageData = pageData.Length > 1 ? pageData[e.Sequence - 1 + startPageIndex] : pageData[0];
				PrintTicket pagePrintTicket = new PrintTicket();
				ApplyPageDataToTicket(pagePrintTicket, currentPageData);
				e.CurrentPrintTicket = pagePrintTicket;
			}
		}
		[SecuritySafeCritical]
		static void ApplyPageDataToTicket(PrintTicket ticket, ReadonlyPageData pageData) {
			Size pageSize = GetPageSize(pageData);
			ticket.PageMediaSize = new PageMediaSize(DrawingConverter.FromPaperKind(pageData.PaperKind), pageSize.Width, pageSize.Height);
			ticket.PageOrientation = pageData.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
		}
		internal static Size GetPageSize(ReadonlyPageData pageData) {
			Size size = new Size(
				GraphicsUnitConverter.Convert(pageData.Bounds.Width, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.DeviceIndependentPixel),
				GraphicsUnitConverter.Convert(pageData.Bounds.Height, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.DeviceIndependentPixel));
			if(pageData.Landscape) {
				double temp = size.Width;
				size.Width = size.Height;
				size.Height = temp;
			}
			return size;
		}
		void writer_WritingCancelled(object sender, WritingCancelledEventArgs e) {
			OnPrintJobCompleted();
		}
		void writer_WritingCompleted(object sender, WritingCompletedEventArgs e) {
			OnAsyncPrintJobCompleted(e.Error, e.Cancelled, e.UserState);
		}
		void writer_WritingProgressChanged(object sender, WritingProgressChangedEventArgs e) {
			TimeSpan timeout = new TimeSpan(100000);
			Dispatcher.CurrentDispatcher.Invoke(new ThreadStart(delegate { }), timeout, DispatcherPriority.ApplicationIdle);
		}
		void OnPrintJobCompleted() {
			if(writer == null)
				return;
			writer.WritingCompleted -= writer_WritingCompleted;
			writer.WritingCancelled -= writer_WritingCancelled;
			writer.WritingProgressChanged -= writer_WritingProgressChanged;
			writer.WritingPrintTicketRequired -= writer_WritingPrintTicketRequired;
			writer = null;
		}
		void OnAsyncPrintJobCompleted(Exception error, bool cancelled, object userState) {
			OnPrintJobCompleted();
			if(PrintCompleted != null)
				PrintCompleted(this, new AsyncCompletedEventArgs(error, cancelled, userState));
		}
		internal static string GetJobDescriptionOrDefault(string jobDescription) {
			return jobDescription ?? PrintingLocalizer.GetString(PrintingStringId.DefaultPrintJobDescription);
		}
		static PrintQueue GetDefaultPrintQueue() {
			using(PrintQueueCollection printQueues = new LocalPrintServer().GetPrintQueues()) {
				if(printQueues.Count() == 0)
					throw new NotSupportedException(PrintingLocalizer.GetString(PrintingStringId.Exception_NoPrinterFound));
			}
			return LocalPrintServer.GetDefaultPrintQueue();
		}
	}
}
