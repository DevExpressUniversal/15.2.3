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
using System.IO;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.Printing.Native.PrintEditor {
	[Flags]
	public enum PrinterType {
		Printer = 0,
		Fax = 1,
		Network = 2,
		Default = 4,
		Offline = 8
	}
	public class PrintEditorController {
		static bool IsValidPath(string path) {
			try {
				using (File.Create(path)) {}
				File.Delete(path);
			}
			catch {
				return false;
			}
			return true;
		}
		public static bool ValidateFilePath(string printFileName, out string messageText) {
			messageText = null;
			if (string.IsNullOrEmpty(printFileName)) {
				messageText = PreviewStringId.Msg_InvalidatePath.GetString();
				return false;
			}
			if (File.Exists(printFileName)) {
				messageText = PreviewStringId.Msg_FileAlreadyExists.GetString();
				return true;
			}
			if (!IsValidPath(printFileName)) {
				messageText = PreviewStringId.Msg_InvalidatePath.GetString();
				return false;
			}
			return true;
		}
		string oldPrinterName;
		IPrintForm form;
		PrintDocument document;
		PrinterSettings PrinterSettings { get { return document.PrinterSettings; } }
		PageSettings PageSettings { get { return document.DefaultPageSettings; } }
		public PrintEditorController(IPrintForm form) {
			this.form = form;
		}
		public void LoadForm(PrinterItemContainer printerItemContainer) {
			document = form.Document;
			oldPrinterName = PrinterSettings.PrinterName;
			foreach(var item in printerItemContainer.Items) {
				form.AddPrinterItem(item);
			}
			if(!PrinterSettings.IsValid && !string.IsNullOrEmpty(printerItemContainer.DefaultPrinterName))
				PrinterSettings.PrinterName = printerItemContainer.DefaultPrinterName;
			form.SetSelectedPrinter(PrinterSettings.PrinterName);
			if(document is IPrintDocumentExtension) {
				form.PageRangeText = (document as IPrintDocumentExtension).PageRange;
			} else if(PrinterSettings.MaximumPage == 1) {
				form.PageRangeText = string.Empty;
				form.AllowSomePages = false;
			} else
				form.PageRangeText = new PageScope(PrinterSettings.FromPage, PrinterSettings.ToPage).PageRange;
			form.SetPrintRange(PrinterSettings.PrintRange);
			form.PrintToFile = PrinterSettings.PrintToFile;
			form.PrintFileName = PrinterSettings.PrintToFile ? PrinterSettings.PrintFileName : string.Empty;
			if(PrinterSettings.IsValid) {
				form.Copies = PrinterSettings.Copies;
				form.Collate = PrinterSettings.Collate;
			}
		}
		public void ShowPrinterPreferences(IntPtr hwnd) {
			short savedCopies = PrinterSettings.Copies;
			bool savedCollate = PrinterSettings.Collate;
			PrinterSettings.Copies = form.Copies;
			PrinterSettings.Collate = form.Collate;
			try {
				new PrinterPreferences().ShowPrinterProperties(PrinterSettings, hwnd);
				form.Copies = PrinterSettings.Copies;
				form.Collate = PrinterSettings.Collate;
			} finally {
				PrinterSettings.Copies = savedCopies;
				PrinterSettings.Collate = savedCollate;
			}
		}
		public void AssignPrinterSettings() {
			PrinterSettings.Copies = form.Copies;
			PrinterSettings.Collate = form.Collate;
			PrinterSettings.PrintToFile = form.PrintToFile;
			if(form.PrintToFile)
				PrinterSettings.PrintFileName = form.PrintFileName;
			if(!string.IsNullOrEmpty(form.PaperSource))
				foreach(PaperSource paperSource in PrinterSettings.PaperSources)
					if(paperSource.SourceName == form.PaperSource) {
						PageSettings.PaperSource = paperSource;
						break;
					}
			if(form.PrintRange == PrintRange.SomePages) {
				if(document is IPrintDocumentExtension)
					(document as IPrintDocumentExtension).PageRange = form.PageRangeText;
				PageScope pageScope = new PageScope(form.PageRangeText, PrinterSettings.MaximumPage);
				PrinterSettings.FromPage = pageScope.FromPage;
				PrinterSettings.ToPage = pageScope.ToPage;
			}
			PrinterSettings.PrintRange = form.PrintRange;
		}
		public void CancelPrinterSettings() {
			PrinterSettings.PrinterName = oldPrinterName;
		}
	}
}
