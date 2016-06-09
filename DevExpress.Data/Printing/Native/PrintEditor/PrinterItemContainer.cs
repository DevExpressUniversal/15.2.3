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
using System.Printing;
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Printing {
	public class PrinterItemContainer : IDisposable {
		List<PrinterItem> items;
		LocalPrintServer printServer = new LocalPrintServer();
		string defaultPrinterName = null;
		public string DefaultPrinterName {
			get {
				if(defaultPrinterName == null) {
					try {
						defaultPrinterName = printServer.DefaultPrintQueue.FullName;
					} catch {
						defaultPrinterName = string.Empty;
					}
				}
				return defaultPrinterName;
			}
		}
		public IList<PrinterItem> Items { 
			get {
				if(items == null) {
					items = new List<PrinterItem>();
					LoadItems();
				}
				return items;
			}
		}
		void LoadItems() {
			if(printServer == null)
				return;
			List<string> offlinePrinters = GetFullNamesOfSpecifiedTypes((new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.WorkOffline, EnumeratedPrintQueueTypes.Connections }));
			List<string> faxes = GetFullNamesOfSpecifiedTypes(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Fax, EnumeratedPrintQueueTypes.Connections });
			using(PrintQueueCollection locals = printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Local }))
				foreach(var printer in locals)
					items.Add(new PrinterItem(printer, faxes.Contains(printer.FullName), false, Equals(printer.FullName, DefaultPrinterName), offlinePrinters.Contains(printer.FullName)));
			using(PrintQueueCollection connections = printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Connections }))
				foreach(var printer in connections)
					items.Add(new PrinterItem(printer, faxes.Contains(printer.FullName), true, Equals(printer.FullName, DefaultPrinterName), offlinePrinters.Contains(printer.FullName)));
			Comparison<PrinterItem> comparision = new Comparison<PrinterItem>((item1, item2) => Comparer<string>.Default.Compare(item1.DisplayName, item2.DisplayName));
			items.Sort(comparision);
		}
		List<string> GetFullNamesOfSpecifiedTypes(EnumeratedPrintQueueTypes[] enumerationFlag) {
			using(PrintQueueCollection printQueues = printServer.GetPrintQueues(enumerationFlag))
				return printQueues.Select<PrintQueue, string>(item => item.FullName).ToList();
		}
		public virtual void Dispose() {
			if(printServer != null) {
				printServer.Dispose();
				printServer = null;
			}
		}
	}
}
