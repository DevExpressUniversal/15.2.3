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
using System.Linq;
using System.Printing;
using System.Windows.Documents;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.Native {
	class PrintService : IPrintService {
		public void PrintDirect(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, bool asyncMode) {
			DocumentPrinter documentPrinter = new DocumentPrinter();
			documentPrinter.PrintDirect(paginator, pageData, jobDescription, asyncMode);
		}
		public void PrintDirect(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, string printerName, bool asyncMode) {
			PrintDirect(paginator, pageData, jobDescription, GetPrintQueue(printerName), asyncMode);
		}
		public void PrintDirect(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, PrintQueue printQueue, bool asyncMode) {
			DocumentPrinter documentPrinter = new DocumentPrinter();
			documentPrinter.PrintDirect(paginator, pageData, jobDescription, printQueue, asyncMode);
		}
		public bool? PrintDialog(DocumentPaginator paginator, ReadonlyPageData[] pageData, string jobDescription, bool asyncMode) {
			DocumentPrinter documentPrinter = new DocumentPrinter();
			return documentPrinter.PrintDialog(paginator, pageData, jobDescription, asyncMode);
		}
		PrintQueue GetPrintQueue(string printerName) {
			Guard.ArgumentIsNotNullOrEmpty(printerName, "printerName");
			using(PrintServer printServer = new PrintServer())
			using(PrintQueueCollection queues = printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Connections | EnumeratedPrintQueueTypes.Local })) {
				PrintQueue queue = queues.FirstOrDefault(x => string.Compare(printerName, x.FullName, true) == 0);
				if(queue == null)
					throw new ArgumentException(string.Format("Print queue '{0}' not found.", printerName), "printerName");
				return queue;
			}
		}
	}
}
