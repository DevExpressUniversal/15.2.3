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
using System.Text;
using DevExpress.XtraPrinting.Preview;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Control;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.XLS;
using System.IO;
using DevExpress.XtraPrinting.Exports;
using System.ComponentModel;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraEditors.Preview;
using System.Linq;
using DevExpress.Printing.Native;
namespace DevExpress.XtraPrinting {
	using DevExpress.XtraPrinting.Native;
	static class PrintingSystemBaseExtentions {
		public static IPrintingSystemExtender Extend(this PrintingSystemBase ps) {
			if(ps == null || ps.IsDisposed || ps.IsDisposing)
				return FakedPrintingSystemExtender.Instance;
			if(!(ps.Extender is IPrintingSystemExtender))
				ps.Extender = new PrintingSystemExtenderWin(ps);
			return ps.Extender as IPrintingSystemExtender;
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	interface IPrintingSystemExtender : IPrintingSystemExtenderBase {
		bool ShowPageSetup(IWin32Window owner, UserLookAndFeel lookAndFeel);
		DialogResult PrintDlg();
		DialogResult PrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel);
		PrintDocument RunPrintDlg();
		PrintDocument RunPrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel);
		UserLookAndFeel LookAndFeel { get; }
		CommandSet CommandSet { get; }
		PrintControl ActiveViewer {
			get;
			set;
		}
		System.Windows.Forms.Form FindForm();
		void RemoveViewer(PrintControl value);
		bool CanExecCommand(PrintingSystemCommand command, IPrintControl printControl);
	}
	class FakedPrintingSystemExtender : PrintingSystemExtenderBase, IPrintingSystemExtender {
		public static readonly IPrintingSystemExtender Instance = new FakedPrintingSystemExtender();
		public bool ShowPageSetup(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			return false;
		}
		public DialogResult PrintDlg() {
			return DialogResult.None;
		}
		public DialogResult PrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			return DialogResult.None;
		}
		public PrintDocument RunPrintDlg() {
			return null;
		}
		public PrintDocument RunPrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			return null;
		}
		public UserLookAndFeel LookAndFeel {
			get {
				return UserLookAndFeel.Default;
			}
		}
		public CommandSet CommandSet {
			get { return null; }
		}
		public PrintControl ActiveViewer {
			get { return null; }
			set { }
		}
		public System.Windows.Forms.Form FindForm() {
			return null;
		}
		public void RemoveViewer(PrintControl value) {
		}
		public bool CanExecCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return false;
		}
	}
	public abstract class PrintingSystemExtenderWinBase : PrintingSystemExtenderPrint {
		bool warningWasShown = false;
		bool ShowMarginsWarning {
			get {
				return printingSystem.ShowMarginsWarning && !warningWasShown;
			}
		}
		public PrintingSystemExtenderWinBase(PrintingSystemBase printingSystem)
			: base(printingSystem) {
		}
		public bool ShowPageSetup(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PageSettingsHelper.PrinterExists)
				return ShowPageSetupCore(owner, lookAndFeel);
			NotificationService.ShowMessage<PrintingSystemBase>(lookAndFeel ?? LookAndFeel, owner ?? FindForm(), PreviewStringId.Msg_NeedPrinter.GetString(), PreviewStringId.Msg_Caption.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		bool ShowPageSetupCore(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			try {
				ClearPrinterSettingsCache(PrinterSettings);
				PageSettingsHelper.ChangePageSettings(PageSettings, PageSizeInfo.GetPaperSize(XPageSettings.Data, PrinterSettings.PaperSizes), XPageSettings.Data);
				PageSettingsHelper.SetPrinterName(PrinterSettings, XPageSettings.PrinterName);
				PageSetupEditorForm psEditor = new PageSetupEditorForm();
				using(DevExpress.XtraBars.BarManager barManager = CreateBarManager(psEditor)) {
					psEditor.LookAndFeel.ParentLookAndFeel = lookAndFeel ?? LookAndFeel;
					psEditor.PageSettings = PageSettings;
					if(DialogRunner.ShowDialog(psEditor, owner ?? FindForm()) == DialogResult.OK) {
						AssignPageSettings(PageSettings, PrinterSettings);
						return true;
					}
					psEditor.Dispose();
					return false;
				}
			} catch {
				return false;
			}
		}
		static DevExpress.XtraBars.BarManager CreateBarManager(DevExpress.XtraEditors.XtraForm form) {
			DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
			barManager.Controller = new DevExpress.XtraBars.BarAndDockingController();
			barManager.Controller.LookAndFeel.ParentLookAndFeel = form.LookAndFeel;
			barManager.Form = form;
			return barManager;
		}
		void AssignPageSettings(PageSettings pageSettings, PrinterSettings printerSettings) {
			PaperSize paperSize = PageSizeInfo.GetAppropriatePaperSize(printerSettings.PaperSizes, pageSettings.PaperSize);
			if(paperSize != null) {
				XPageSettings.Assign(pageSettings.Margins, XPageSettings.MinMargins, paperSize.Kind, PageData.ToSize(paperSize), pageSettings.Landscape, paperSize.PaperName);
				XPageSettings.PaperName = paperSize.PaperName;
			} else
				XPageSettings.Assign(pageSettings, XPageSettings.MinMargins);
		}
		static void ClearPrinterSettingsCache(PrinterSettings sets) {
			sets.PrinterName = sets.PrinterName;
		}
		public override void Print(string printerName) {
			if(printingSystem.Document.IsEmpty)
				return;
			if(!PageSettingsHelper.PrinterExists)
				PrintDlg();
			else
				base.Print(printerName);
		}
		public DialogResult PrintDlg() {
			return PrintDlg(null, null);
		}
		public PrintDocument RunPrintDlg() {
			return RunPrintDlg(null, null);
		}
		public DialogResult PrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			PrintDocument pd = RunPrintDlg(owner, lookAndFeel);
			if(pd == null)
				return DialogResult.None;
			try {
				PrintDocument(pd);
				printingSystem.OnEndPrint(EventArgs.Empty);
				PrinterSettings.PrintToFile = false;
				return DialogResult.OK;
			} finally {
				pd.Dispose();
			}
		}
		public PrintDocument RunPrintDlg(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PSNativeMethods.AspIsRunning || printingSystem.Document.IsEmpty)
				return null;
			PSPrintDocument printDocument = CreatePrintDocument(printingSystem);
			printingSystem.OnStartPrint(new PrintDocumentEventArgs(printDocument));
			printDocument.PageRange = GetPageRange(printDocument.PrinterSettings);
			printDocument.DefaultPageSettings.PaperSize = PageSizeInfo.GetPaperSize(printingSystem.PageSettings.Data, PrinterSettings.PaperSizes);
			printDocument.DefaultPageSettings.Landscape = printingSystem.PageSettings.Landscape;
			DialogResult dialogResult = DialogResult.None;
			try {
				dialogResult = PrintDialogRunner.Instance.Run(printDocument, lookAndFeel ?? LookAndFeel, owner ?? FindForm(), GetPrintDialogAllowFlags());   
			} catch(Exception ex) {
				NotificationService.ShowException<PrintingSystemBase>(lookAndFeel ?? LookAndFeel, owner ?? FindForm(), ex);
			}
			if(dialogResult != DialogResult.OK) {
				printDocument.Dispose();
				return null;
			}
			PredefinedPageRange = printDocument.PageRange;
			switch(printDocument.PrinterSettings.PrintRange) {
				case PrintRange.AllPages:
					printDocument.PageRange = new PageScope(1, printingSystem.Pages.Count).PageRange;
					break;
				case PrintRange.CurrentPage:
					if(SelectedPageIndex >= 0)
						printDocument.PageRange = (SelectedPageIndex + 1).ToString();
					break;
				case PrintRange.Selection:
					PrinterSettings oldPrinterSettings = printDocument.PrinterSettings;
					PrintingSystemBase fakedPS = CreateSelectionPrinting();
					if(fakedPS == null)
						return printDocument;
					printDocument = CreatePrintDocument(fakedPS);
					printDocument.PrinterSettings = oldPrinterSettings;
					break;
			}
			return printDocument;
		}
		PrintDialogAllowFlags GetPrintDialogAllowFlags() {
			PrintDialogAllowFlags flags = PrintDialogAllowFlags.AllowPrintToFile | PrintDialogAllowFlags.AllowSomePages | PrintDialogAllowFlags.AllowAllPages;
			if(SelectedPageIndex >= 0)
				flags |= PrintDialogAllowFlags.AllowCurrentPage;
			if(HasSelection)
				flags |= PrintDialogAllowFlags.AllowSelection;
			return flags;
		}
		protected override PSPrintDocument CreatePrintDocument(PrintingSystemBase ps) {
			warningWasShown = false;
			return base.CreatePrintDocument(ps);
		}
		protected override bool PredicateMargins() {
			if(!ShowMarginsWarning)
				return true;
			if(PSNativeMethods.AspIsRunning)
				throw new InvalidOperationException("One or more margins are set outside the printable area of the page.");
			warningWasShown = true;
			DialogResult result = NotificationService.ShowMessage<PrintingSystemBase>(
				LookAndFeel, FindForm(),
				PreviewStringId.Msg_PageMarginsWarning.GetString(),
				PreviewStringId.Msg_Caption.GetString(),
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);
			return result == DialogResult.Yes;
		}
		#region abstraction
		public abstract UserLookAndFeel LookAndFeel { get; }
		protected abstract int SelectedPageIndex { get; }
		protected abstract bool HasSelection { get; }
		public abstract System.Windows.Forms.Form FindForm();
		protected abstract PrintingSystemBase CreateSelectionPrinting();
		#endregion
	}
	class PrintingSystemExtenderWin : PrintingSystemExtenderWinBase, IPrintingSystemExtender {
		CommandSet commandSet;
		List<PrintControl> viewers = new List<PrintControl>();
		List<ICommandHandler> commandHandlers = new List<ICommandHandler>();
		public PrintingSystemExtenderWin(PrintingSystemBase printingSystem)
			: base(printingSystem) {
			commandSet = new CommandSet();
		}
		protected override PrintingSystemBase CreateSelectionPrinting() {
			return ActiveViewer != null ? ActiveViewer.SelectionService.GetFakedPSWithSelection() : null;
		}
		public override UserLookAndFeel LookAndFeel {
			get {
				return GetLookAndFeel(ActiveViewer);
			}
		}
		static UserLookAndFeel GetLookAndFeel(PrintControl printControl) {
			return printControl != null && !printControl.IsDisposed ? printControl.LookAndFeel : UserLookAndFeel.Default;
		}
		public CommandSet CommandSet {
			get { return commandSet; }
		}
		public override ProgressReflector ActiveProgressReflector {
			get {
				return ActiveViewer != null && !ActiveViewer.IsDisposed ?
					ActiveViewer.ProgressReflector : null;
			}
		}
		public PrintControl ActiveViewer {
			get {
				return viewers.Count > 0 ? viewers[0] : null;
			}
			set {
				if(ActiveViewer != value) {
					if(ActiveViewer != null)
						StopPageBuilding();
					RemoveViewer(value);
					viewers.Insert(0, value);
				}
			}
		}
		protected override int SelectedPageIndex {
			get { return ActiveViewer != null ? ActiveViewer.SelectedPage.Index : -1; }
		}
		protected override bool HasSelection {
			get { return ActiveViewer != null ? ActiveViewer.SelectionService.HasSelection : false; }
		}
		public override System.Windows.Forms.Form FindForm() {
			return ActiveViewer != null ? ActiveViewer.FindForm() : null;
		}
		public void RemoveViewer(PrintControl value) {
			if(viewers.Contains(value)) {
				if(viewers.Count == 1)
					StopPageBuilding();
				viewers.Remove(value);
				object ignore = printingSystem.ProgressReflector;
			}
		}
		void StopPageBuilding() {
			if(printingSystem.Document.IsCreating)
				printingSystem.Document.StopPageBuilding();
		}
		public override void AddCommandHandler(ICommandHandler handler) {
			if(!commandHandlers.Contains(handler))
				commandHandlers.Add(handler);
		}
		public override void RemoveCommandHandler(ICommandHandler handler) {
			commandHandlers.Remove(handler);
		}
		void ClearCommandHadlers() {
			while(commandHandlers.Count > 0) {
				ICommandHandler commandHandler = commandHandlers[0];
				commandHandlers.Remove(commandHandler);
				if(commandHandler is IDisposable)
					((IDisposable)commandHandler).Dispose();
			}
		}
		public override void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility, Priority priority) {
			commandSet.SetCommandVisibility(commands, visibility, priority, printingSystem);
			foreach(IPrintControl item in viewers)
				item.CommandSet.SetCommandVisibility(commands, visibility, priority, printingSystem);
		}
		public override void EnableCommand(PrintingSystemCommand command, bool enabled) {
			commandSet[command].Enabled = enabled;
			foreach(IPrintControl item in viewers)
				item.CommandSet.EnableCommand(enabled, command);
		}
		public bool CanExecCommand(PrintingSystemCommand command, IPrintControl printControl) {
			if(IsNonVisibleCommand(command))
				return false;
			try {
				foreach(ICommandHandler commandHandler in GetCommandHandlers()) {
					if(commandHandler.CanHandleCommand(command, printControl))
						return true;
				}
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
			}
			return false;
		}
		IEnumerable<ICommandHandler> GetCommandHandlers() {
			byte[] dxkey = GetType().Assembly.GetName().GetPublicKeyToken();
			for(int i = commandHandlers.Count - 1; i >= 0; i--) {
				byte[] key = commandHandlers[i].GetType().Assembly.GetName().GetPublicKeyToken();
				if(!key.SequenceEqual<byte>(dxkey))
					yield return commandHandlers[i];
			}
			for(int i = commandHandlers.Count - 1; i >= 0; i--) {
				byte[] key = commandHandlers[i].GetType().Assembly.GetName().GetPublicKeyToken();
				if(key.SequenceEqual<byte>(dxkey))
					yield return commandHandlers[i];
			}
		}
		public override void ExecCommand(PrintingSystemCommand command, object[] args) {
			foreach(PrintControl printControl in new List<PrintControl>(viewers))
				ExecCommand(command, args, printControl);
		}
		public override void ExecCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl) {
			if(IsNonVisibleCommand(command))
				return;
			bool handled = false;
			try {
				foreach(ICommandHandler commandHandler in GetCommandHandlers()) {
					if(commandHandler.CanHandleCommand(command, printControl)) {
						commandHandler.HandleCommand(command, args, printControl, ref handled);
						if(handled)
							return;
					}
				}
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
			}
			((PrintControl)printControl).ExecCommandCore(command, args);
		}
		bool IsNonVisibleCommand(PrintingSystemCommand command) {
			return GetCommandVisibility(command) == CommandVisibility.None;
		}
		public override CommandVisibility GetCommandVisibility(PrintingSystemCommand command) {
			return commandSet.GetCommandVisibility(command);
		}
		public override void Clear() {
			ClearCommandHadlers();
			viewers.Clear();
		}
		public override void OnBeginCreateDocument() {
			SetCommandVisibility(
				new PrintingSystemCommand[] {
					PrintingSystemCommand.Customize, 
					PrintingSystemCommand.EditPageHF, 
					PrintingSystemCommand.ExportCsv, 
					PrintingSystemCommand.ExportHtm,
					PrintingSystemCommand.ExportMht,
					PrintingSystemCommand.ExportRtf,
					PrintingSystemCommand.ExportTxt,
					PrintingSystemCommand.ExportXls,
					PrintingSystemCommand.ExportXlsx,
					PrintingSystemCommand.SendCsv,
					PrintingSystemCommand.SendMht,
					PrintingSystemCommand.SendRtf,
					PrintingSystemCommand.SendTxt,
					PrintingSystemCommand.SendXls, 
					PrintingSystemCommand.SendXlsx },
				CommandVisibility.None, Priority.Low);
			((CommandSet)CommandSet).EnableAllCommands(false);
			PredefinedPageRange = string.Empty;
		}
	}
}
