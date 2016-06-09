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
using System.Windows;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Services;
#if SL
using PlatformIWin32Window = DevExpress.Xpf.Core.Native.IWin32Window;
#else
using PlatformIWin32Window = System.Windows.Forms.IWin32Window;
using DevExpress.Xpf.Printing;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.IO;
#endif
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl {
		void ISpreadsheetControl.ShowPrintPreview() {
			ISpreadsheetPrintingService service = GetService<ISpreadsheetPrintingService>();
			if (service != null)
				service.ShowPrintPreview(this);
			else {
#if SL
				this.BrowserPrintPreview();
#endif
			}
		}
		void ISpreadsheetControl.ShowRibbonPrintPreview() {
			ISpreadsheetPrintingService service = GetService<ISpreadsheetPrintingService>();
			if (service != null)
				service.ShowRibbonPrintPreview(this);
			else {
#if SL
				this.BrowserPrintPreview();
#endif
			}
		}
		bool ISpreadsheetControl.IsPrintingAvailable { get { return true; } }
		void ISpreadsheetControl.ShowPrintDialog() {
			PrintCore();
		}
		public void Print() {
			PrintCore();
		}
		void PrintCore() {
			ExecutePrinterAction((link) => { link.Print(); });
		}
		void ExecutePrinterAction(Action<LegacyPrintableComponentLink> action) {
			using (LegacyPrintableComponentLink link = new LegacyPrintableComponentLink(InnerControl)) {
				link.CreateDocument();
				if (link.PrintingSystem.Document.PageCount > 0)
					action(link);
			}
		}
		#region Export to pdf
		public void ExportToPdf(string fileName) {
			ExecutePrinterAction((link) => { link.ExportToPdf(fileName); });
		}
		public void ExportToPdf(string fileName, PdfExportOptions options) {
			ExecutePrinterAction((link) => { link.ExportToPdf(fileName, options); });
		}
		public void ExportToPdf(Stream stream) {
			ExecutePrinterAction((link) => { link.ExportToPdf(stream); });
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			ExecutePrinterAction((link) => { link.ExportToPdf(stream, options); });
		}
		#endregion
	}
}
