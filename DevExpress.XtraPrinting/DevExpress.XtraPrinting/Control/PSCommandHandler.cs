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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using DevExpress.XtraPrinting.Localization;
using System.Collections;
using System.Drawing.Imaging;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Preview.Native;
namespace DevExpress.XtraPrinting.Control {
	class PrintCommandExecutor {
		PrintingSystemBase ps;
		UserLookAndFeel lookAndFeel;
		IWin32Window owner;
		public PrintCommandExecutor(PrintingSystemBase ps, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			this.lookAndFeel = lookAndFeel;
			this.owner = owner;
			this.ps = ps;
		}
		public void Print(string printerName) {
			try {
				ps.Extend().Print(printerName);
			} catch(Exception ex) {
				ShowExceptionIfNeeded(ex);
			}
		}
		public void PrintDlg() {
			try {
				ps.Extend().PrintDlg();
			} catch(Exception ex) {
				ShowExceptionIfNeeded(ex);
			}
		}
		void ShowExceptionIfNeeded(Exception ex) {
			Exception ex2 = GetPrinterException(ex);
			if(ex2 != null)
				NotificationService.ShowException<PrintingSystemBase>(lookAndFeel, owner, ex2);
		}
		static Exception GetPrinterException(Exception ex) {
			string message;
			if(ex is Win32Exception && TryGetMessage(((Win32Exception)ex).NativeErrorCode, out message))
				return new Exception(message, ex);
			return ex is InvalidPrinterException ? ex : null;
		}
		static bool TryGetMessage(int nativeErrorCode, out string message) {
			const int ERROR_CANCELLED = 1223, 
					  ERROR_PRINT_CANCELLED = 63, 
					  RPC_S_SERVER_UNAVAILABLE = 1722, 
					  ERROR_INVALID_PRINTER_NAME = 1801, 
					  ERROR_SPL_NO_STARTDOC = 3003; 
			if(nativeErrorCode == ERROR_CANCELLED || nativeErrorCode == ERROR_SPL_NO_STARTDOC || nativeErrorCode == ERROR_PRINT_CANCELLED) {
				message = string.Empty;
				return false;
			} 
			if(nativeErrorCode == ERROR_INVALID_PRINTER_NAME)
				message = PreviewStringId.Msg_WrongPrinter.GetString();
			else if(nativeErrorCode == RPC_S_SERVER_UNAVAILABLE)
				message = PreviewStringId.Msg_UnavailableNetPrinter.GetString();
			else
				message = PreviewStringId.Msg_WrongPrinting.GetString();
			return true;
		}
	}
}
namespace DevExpress.XtraPrinting.Control.Native {
	class PSCommandHandler : PSCommandHandlerBase, ICommandHandler, IDisposable {
		XtraPrinting.Control.PrintControl printControl;
		SelectionService selectionService;
		protected override PrintingSystemBase PrintingSystemBase {
			get { return printControl.PrintingSystem; }
		}
		public PSCommandHandler(XtraPrinting.Control.PrintControl printControl, SelectionService selectionService) {
			this.printControl = printControl;
			this.selectionService = selectionService;
			handlers.Add(PrintingSystemCommand.DocumentMap, new CommandEventHandler(HandleDocumentMap));
			handlers.Add(PrintingSystemCommand.Thumbnails, new CommandEventHandler(HandleThumbnails));
			handlers.Add(PrintingSystemCommand.Parameters, new CommandEventHandler(HandleParameters));
			handlers.Add(PrintingSystemCommand.Pointer, new CommandEventHandler(HandlePointer));
			handlers.Add(PrintingSystemCommand.HandTool, new CommandEventHandler(HandleHandTool));
			handlers.Add(PrintingSystemCommand.Print, new CommandEventHandler(HandlePrint));
			handlers.Add(PrintingSystemCommand.PrintDirect, new CommandEventHandler(HandlePrintDirect));
			handlers.Add(PrintingSystemCommand.PageSetup, new CommandEventHandler(HandlePageSetup));
			handlers.Add(PrintingSystemCommand.Magnifier, new CommandEventHandler(HandleMagnifier));
			handlers.Add(PrintingSystemCommand.ZoomIn, new CommandEventHandler(HandleZoomIn));
			handlers.Add(PrintingSystemCommand.ZoomOut, new CommandEventHandler(HandleZoomOut));
			handlers.Add(PrintingSystemCommand.Zoom, new CommandEventHandler(HandleZoom));
			handlers.Add(PrintingSystemCommand.ZoomTrackBar, new CommandEventHandler(HandleZoomTrackBar));
			handlers.Add(PrintingSystemCommand.ViewWholePage, new CommandEventHandler(HandleViewWholePage));
			handlers.Add(PrintingSystemCommand.ShowFirstPage, new CommandEventHandler(HandleShowFirstPage));
			handlers.Add(PrintingSystemCommand.ShowPrevPage, new CommandEventHandler(HandleShowPrevPage));
			handlers.Add(PrintingSystemCommand.ShowNextPage, new CommandEventHandler(HandleShowNextPage));
			handlers.Add(PrintingSystemCommand.ScrollPageUp, new CommandEventHandler(HandleScrollPageUp));
			handlers.Add(PrintingSystemCommand.ScrollPageDown, new CommandEventHandler(HandleScrollPageDown));
			handlers.Add(PrintingSystemCommand.ShowLastPage, new CommandEventHandler(HandleShowLastPage));
			handlers.Add(PrintingSystemCommand.FillBackground, new CommandEventHandler(HandleFillBackground));
			handlers.Add(PrintingSystemCommand.ZoomToPageWidth, new CommandEventHandler(HandlePageWidth));
			handlers.Add(PrintingSystemCommand.ZoomToTextWidth, new CommandEventHandler(HandleTextWidth));
			handlers.Add(PrintingSystemCommand.ZoomToWholePage, new CommandEventHandler(HandleWholePage));
			handlers.Add(PrintingSystemCommand.ZoomToTwoPages, new CommandEventHandler(HandleTwoPages));
			handlers.Add(PrintingSystemCommand.PageLayoutContinuous, new CommandEventHandler(HandleContinuous));
			handlers.Add(PrintingSystemCommand.PageLayoutFacing, new CommandEventHandler(HandleFacing));
			handlers.Add(PrintingSystemCommand.ClosePreview, new CommandEventHandler(HandleClosePreview));
			handlers.Add(PrintingSystemCommand.Watermark, new CommandEventHandler(HandleWatermark));
			handlers.Add(PrintingSystemCommand.MultiplePages, new CommandEventHandler(HandleMultiplePages));
			handlers.Add(PrintingSystemCommand.Scale, new CommandEventHandler(HandleScale));
			handlers.Add(PrintingSystemCommand.PageMargins, new CommandEventHandler(HandlePageMargins));
			handlers.Add(PrintingSystemCommand.PageOrientation, new CommandEventHandler(HandlePageOrientation));
			handlers.Add(PrintingSystemCommand.PaperSize, new CommandEventHandler(HandlePaperSize));
			handlers.Add(PrintingSystemCommand.Find, new CommandEventHandler(HandleFind));
			handlers.Add(PrintingSystemCommand.Open, new CommandEventHandler(HandleOpen));
			handlers.Add(PrintingSystemCommand.Save, new CommandEventHandler(HandleSave));
			handlers.Add(PrintingSystemCommand.Copy, new CommandEventHandler(HandleCopy));
			handlers.Add(PrintingSystemCommand.PrintSelection, new CommandEventHandler(HandlePrintSelection));
			handlers.Add(PrintingSystemCommand.GoToPage, new CommandEventHandler(HandleGoToPage));
			Application.Idle += new EventHandler(Application_Idle);
		}
		void Application_Idle(object sender, EventArgs e) {
			if(!printControl.Visible)
				printControl.HidePanels();
		}
		public void Dispose() {
			Application.Idle -= new EventHandler(Application_Idle);
		}
		protected override ExportFileHelperBase CreateExportFileHelper() {
			return new ExportFileHelper(PrintingSystemBase, new EmailSender(printControl));
		}
		protected override void DoExport(ExportOptionsBase options) {
			try {
				base.DoExport(options);
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
				NotificationService.ShowException<PrintingSystemBase>(printControl.LookAndFeel, printControl.FindForm(), ex);
			}
		}
		protected override void SendFileByEmail(ExportOptionsBase options) {
			try {
				base.SendFileByEmail(options);
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
				NotificationService.ShowException<PrintingSystemBase>(printControl.LookAndFeel, printControl.FindForm(), ex);
			}
		}
		protected override System.Collections.Generic.IDictionary<Type, object[]> DisabledExportModes {
			get { return printControl != null ? printControl.DisabledExportModes : null; }
		}
		void HandlePrint(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.BeginInvoke(new MethodInvoker(PrintDlgProc));
		}
		void PrintDlgProc() {
			PrintingSystemBase.Extend().PrinterSettings.PrintRange = PrintRange.AllPages;
			new PrintCommandExecutor(PrintingSystemBase, printControl.LookAndFeel, printControl.FindForm()).PrintDlg();
		}
		void HandlePrintDirect(object[] args, XtraPrinting.Control.PrintControl printControl) {
			PrintingSystemBase.Extend().PrinterSettings.PrintRange = PrintRange.AllPages;
			new PrintCommandExecutor(PrintingSystemBase, printControl.LookAndFeel, printControl.FindForm()).Print(string.Empty);
		}
		void HandlePrintSelection(object[] args, XtraPrinting.Control.PrintControl printControl) {
			PrintingSystemBase.Extend().PrinterSettings.PrintRange = PrintRange.Selection;
			new PrintCommandExecutor(PrintingSystemBase, printControl.LookAndFeel, printControl.FindForm()).PrintDlg();
		}
		void HandlePageSetup(object[] args, XtraPrinting.Control.PrintControl printControl) {
			PrintingSystemBase.Extend().ShowPageSetup(printControl.FindForm(), printControl.LookAndFeel);
		}
		void HandlePointer(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.AutoZoom = false;
			printControl.HandTool = false;
		}
		void HandleHandTool(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.AutoZoom = false;
			printControl.HandTool = Convert.ToBoolean(args[0]);
		}
		void HandleThumbnails(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPanelVisibility(PrintingSystemCommand.Thumbnails, Convert.ToBoolean(args[0]));
		}
		void HandleDocumentMap(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPanelVisibility(PrintingSystemCommand.DocumentMap, Convert.ToBoolean(args[0]));
		}
		void HandleParameters(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPanelVisibility(PrintingSystemCommand.Parameters, Convert.ToBoolean(args[0]));
		}
		void HandleMagnifier(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.HandTool = false;
			printControl.AutoZoom = Convert.ToBoolean(args[0]);
		}
		void HandleZoom(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args != null) {
				printControl.Zoom = Convert.ToSingle(args[0]);
				printControl.SetFocus();
			}
		}
		void HandleZoomTrackBar(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args != null) {
				printControl.Zoom = Convert.ToSingle(args[0]);
			}
		}
		void HandleZoomIn(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.ZoomIn();
		}
		void HandleZoomOut(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.ZoomOut();
		}
		void HandleViewWholePage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.ViewWholePage();
		}
		void HandleShowFirstPage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SelectFirstPage();
		}
		void HandleShowPrevPage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SelectPrevPage();
		}
		void HandleShowNextPage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SelectNextPage();
		}
		void HandleScrollPageDown(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.ScrollPageDown();
		}
		void HandleScrollPageUp(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.ScrollPageUp();
		}
		void HandleShowLastPage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SelectLastPage();
		}
		void HandleMultiplePages(object[] args, XtraPrinting.Control.PrintControl printControl) {
		}
		void HandleFillBackground(object[] args, XtraPrinting.Control.PrintControl printControl) {
			SetBackground();
		}
		void HandleScale(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args.Length == 1) {
				if(args[0] is int)
					PrintingSystemBase.Document.AutoFitToPagesWidth = (int)args[0];
				else if(args[0] is float)
					PrintingSystemBase.Document.ScaleFactor = (float)args[0];
			}
		}
		void HandlePageMargins(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args.Length == 1 && args[0] is MarginsF) {
				MarginsF margins = (MarginsF)args[0];
				PrintingSystemBase.PageSettings.SetMarginsInHundredthsOfInch(margins);
			}
		}
		void HandlePaperSize(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args.Length == 1 && args[0] is PaperSize) {
				PaperSize paperSize = (PaperSize)args[0];
				PrintingSystemBase.PageSettings.SetPaperSize(paperSize);
			}
		}
		void HandlePageOrientation(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(args.Length == 1 && args[0] is PageOrientation) {
				PageOrientation orientation = (PageOrientation)args[0];
				bool landScape = orientation == PageOrientation.Landscape;
				if(PrintingSystemBase.PageSettings.Landscape != landScape)
					PrintingSystemBase.PageSettings.Landscape = landScape;
			}
		}
		void HandlePageWidth(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPageView(PageViewModes.PageWidth);
		}
		void HandleTextWidth(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPageView(PageViewModes.TextWidth);
		}
		void HandleWholePage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPageView(1, 1);
		}
		void HandleTwoPages(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.SetPageView(2, 1);
		}
		void HandleContinuous(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.Continuous = true;
		}
		void HandleFacing(object[] args, XtraPrinting.Control.PrintControl printControl) {
			printControl.Continuous = false;
		}
		void HandleClosePreview(object[] args, XtraPrinting.Control.PrintControl printControl) {
			Form form = printControl.FindForm();
			if(form != null)
				form.Close();
		}
		void HandleWatermark(object[] args, XtraPrinting.Control.PrintControl printControl) {
			WatermarkEditorForm wmEditor = new WatermarkEditorForm();
			wmEditor.LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
			PageData data = PrintingSystemBase.PageSettings.Data;
			wmEditor.PageSettings.Assign(data.Margins, data.MinMargins, data.PaperKind, data.Size, data.Landscape);
			wmEditor.Assign(PrintingSystemBase.Watermark);
			if(wmEditor.ShowDialog() == DialogResult.OK) {
				PrintingSystemBase.Watermark.CopyFrom(wmEditor.Watermark);
				PrintingSystemBase.RaisePageBackgrChanged(EventArgs.Empty);
			}
			wmEditor.Dispose();
		}
		void HandleFind(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(this.printControl.FindPanelVisible && !Convert.ToBoolean(args[0]))
				this.printControl.CloseFindControl();
			else if(!this.printControl.FindPanelVisible && Convert.ToBoolean(args[0]))
				this.printControl.ShowFindControl();
			else
				this.printControl.FocusFindControl();
		}
		void HandleOpen(object[] args, XtraPrinting.Control.PrintControl printControl) {
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = PreviewStringId.OpenFileDialog_Filter.GetString(NativeFormatOptionsController.NativeFormatExtension);
				dialog.Title = PreviewStringId.OpenFileDialog_Title.GetString();
				if(DialogRunner.ShowDialog(dialog) == DialogResult.OK) {
					try {
						PrintingSystemBase.LoadDocument(dialog.FileName);
					} catch(Exception ex) {
						Tracer.TraceError(NativeSR.TraceSource, ex);
						NotificationService.ShowException<PrintingSystemBase>(printControl.LookAndFeel, printControl.FindForm(), new Exception(PreviewStringId.Msg_CannotLoadDocument.GetString(), ex));
					}
				}
			}
		}
		void HandleSave(object[] args, XtraPrinting.Control.PrintControl printControl) {
			DoExport(PrintingSystemBase.ExportOptions.NativeFormat);
		}
		void HandleCopy(object[] args, XtraPrinting.Control.PrintControl printControl) {
			if(selectionService != null)
				selectionService.CopyToClipboard();
		}
		void HandleGoToPage(object[] args, XtraPrinting.Control.PrintControl printControl) {
			using(GoToPageDialog dialog = new GoToPageDialog(printControl.PrintingSystem.Pages.Count)) {
				if(DialogRunner.ShowDialog(dialog, printControl.FindForm()) == DialogResult.OK && dialog.PageNumber > 0)
					printControl.ViewManager.SelectedPagePlaceIndex = dialog.PageNumber - 1;
			}
		}
		public void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
			PrintingSystemBase printingSystem = printControl.GetPrintingSystem();
			if(printingSystem == null && !printControl.IsVisible())
				return;
			if(printingSystem != null && printingSystem.Document.IsCreating) {
				handled = HandleCommandCore2(command, args, (XtraPrinting.Control.PrintControl)printControl);
				return;
			}
			handled = HandleCommandCore(command, args, (XtraPrinting.Control.PrintControl)printControl);
		}
		bool HandleCommandCore(PrintingSystemCommand command, object[] args, XtraPrinting.Control.PrintControl printControl) {
			try {
				printControl.Status = PreviewLocalizer.GetString(PreviewStringId.Msg_CreatingDocument);
				return HandleCommandCore2(command, args, printControl);
			} finally {
				printControl.Status = "";
			}
		}
		bool HandleCommandCore2(PrintingSystemCommand command, object[] args, XtraPrinting.Control.PrintControl printControl) {
			object handler;
			if(handlers.TryGetValue(command, out handler)) {
				if(handler is PrintingSystemCommandHandlerBase) {
					((PrintingSystemCommandHandlerBase)handler)(args);
				} else {
					((CommandEventHandler)handler)(args, printControl);
				}
				return true;
			}
			return false;
		}
		public bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return handlers.ContainsKey(command);
		}
		private void SetBackground() {
			ColorDialog dlg = new ColorDialog();
			dlg.Color = PrintingSystemBase.Graph.PageBackColor;
			if(DialogRunner.ShowDialog(dlg) == DialogResult.OK) {
				PrintingSystemBase.Graph.PageBackColor = dlg.Color;
			}
		}
	}
}
namespace DevExpress.XtraPrinting {
	public static class PrintControlExtentions {
		public static PrintingSystemBase GetPrintingSystem(this IPrintControl pc) {
			return pc is XtraPrinting.Control.PrintControl ? ((XtraPrinting.Control.PrintControl)pc).PrintingSystem : null;
		}
		public static bool IsVisible(this IPrintControl pc) {
			return pc is System.Windows.Forms.Control ? ((System.Windows.Forms.Control)pc).Visible : false;
		}
	}
}
