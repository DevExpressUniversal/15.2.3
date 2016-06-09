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
using System.Windows.Forms;
using System.Drawing.Printing;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl : IPrintable {
		protected internal IPrintable PrintableImplementation { get { return InnerControl; } }
		protected internal virtual void CheckPrintableImplmenentation() {
		}
		#region IPrintable Members
		bool IPrintable.CreatesIntersectedBricks {
			get {
				CheckPrintableImplmenentation();
				return PrintableImplementation.CreatesIntersectedBricks;
			}
		}
		UserControl IPrintable.PropertyEditorControl {
			get {
				CheckPrintableImplmenentation();
				return PrintableImplementation.PropertyEditorControl;
			}
		}
		void IPrintable.AcceptChanges() {
			CheckPrintableImplmenentation();
			PrintableImplementation.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			CheckPrintableImplmenentation();
			PrintableImplementation.RejectChanges();
		}
		bool IPrintable.HasPropertyEditor() {
			CheckPrintableImplmenentation();
			return PrintableImplementation.HasPropertyEditor();
		}
		void IPrintable.ShowHelp() {
			CheckPrintableImplmenentation();
			PrintableImplementation.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			CheckPrintableImplmenentation();
			return PrintableImplementation.SupportsHelp();
		}
		#endregion
		#region IBasePrintable Members
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			CheckPrintableImplmenentation();
			PrintableImplementation.Initialize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graphics) {
			CheckPrintableImplmenentation();
			PrintableImplementation.CreateArea(areaName, graphics);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if (PrintableImplementation == null)
				return;
			PrintableImplementation.Finalize(ps, link);
		}
		#endregion
		#region Printing support methods and properties
		IDisposable printer;
		ComponentPrinterBase Printer {
			get {
				if (printer == null) {
					PrintingSystemBase printingSystem = new PrintingSystemBase();
					printer = new ComponentPrinter(this, printingSystem);
				}
				return (ComponentPrinterBase)printer;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return PrintableImplementation != null && ComponentPrinterBase.IsPrintingAvailable(false); } }
		bool ISpreadsheetControl.IsPrintPreviewAvailable { get { return IsPrintingAvailable; } }
		public void ShowPrintPreview() {
			ExecutePrinterAction(delegate() {
				Printer.ShowPreview(this.LookAndFeel);
			});
		}
		public void ShowRibbonPrintPreview() {
			ExecutePrinterAction(delegate() {
				Printer.ShowRibbonPreview(this.LookAndFeel);
			});
		}
		public void ShowPrintDialog() {
			ExecutePrinterActionAndHandleError(delegate() {
				Printer.PrintDialog();
			});
		}
		public void Print() {
			ExecutePrinterActionAndHandleError(delegate() {
				Printer.Print();
			});
		}
		#endregion
		public void ExportToPdf(string fileName) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, fileName);
			});
		}
		public void ExportToPdf(string fileName, PdfExportOptions pdfExportOptions) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, fileName, pdfExportOptions);
			});
		}
		public void ExportToPdf(Stream stream, PdfExportOptions pdfExportOptions) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, stream, pdfExportOptions);
			});
		}
		public void ExportToPdf(Stream stream) {
			ExecutePrinterAction(delegate() {
				Printer.Export(ExportTarget.Pdf, stream);
			});
		}
		void ExecutePrinterAction(Action0 action) {
			Printer.ClearDocument();
			action();
		}
		void ExecutePrinterActionAndHandleError(Action0 action) {
			Printer.ClearDocument();
			try {
				action();
			}
			catch (Win32Exception ex1) {
				ShowPrintingError(ex1.NativeErrorCode);
			}
			catch (InvalidPrinterException ex2) {
				ShowPrintingError(ex2.Message);
			}
		}
		void ShowPrintingError(string message) {
			ISpreadsheetControl control = this as ISpreadsheetControl;
			if (!string.IsNullOrEmpty(message) && control != null)
				control.ShowWarningMessage(message);
		}
		void ShowPrintingError(int nativeErrorCode) {
			string message = GetMessageByErrorCode(nativeErrorCode);
			ShowPrintingError(message);
		}
		string GetMessageByErrorCode(int nativeErrorCode) {
			const int ERROR_CANCELLED = 1223, 
					  ERROR_PRINT_CANCELLED = 63, 
					  RPC_S_SERVER_UNAVAILABLE = 1722, 
					  ERROR_INVALID_PRINTER_NAME = 1801, 
					  ERROR_SPL_NO_STARTDOC = 3003; 
			if (nativeErrorCode == ERROR_CANCELLED || nativeErrorCode == ERROR_SPL_NO_STARTDOC || nativeErrorCode == ERROR_PRINT_CANCELLED)
				return string.Empty;
			if (nativeErrorCode == ERROR_INVALID_PRINTER_NAME)
				return PreviewStringId.Msg_WrongPrinter.GetString();
			if (nativeErrorCode == RPC_S_SERVER_UNAVAILABLE)
				return PreviewStringId.Msg_UnavailableNetPrinter.GetString();
			return PreviewStringId.Msg_WrongPrinting.GetString();
		}
	}
}
