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
using System.IO;
using DevExpress.XtraPrinting.Export;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export.XLS;
using DevExpress.XtraPrinting.Export.Web;
using System.Drawing.Printing;
using DevExpress.Printing.Native;
namespace DevExpress.XtraPrinting.Native {
	public interface IPrintingSystemExtenderBase : IPageSettingsExtenderBase {
		ProgressReflector ActiveProgressReflector { get; }
		void AddCommandHandler(ICommandHandler handler);
		void RemoveCommandHandler(ICommandHandler handler);
		void ExecCommand(PrintingSystemCommand command, object[] args);
		void ExecCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl);
		void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility, Priority priority);
		CommandVisibility GetCommandVisibility(PrintingSystemCommand command);		
		void EnableCommand(PrintingSystemCommand command, bool enabled);
		void Print(string printerName);
		void OnBeginCreateDocument();
		void Clear();
	}
	public abstract class PrintingSystemExtenderBase : IPrintingSystemExtenderBase {
		public virtual void Dispose() {
		}
		public virtual string PredefinedPageRange { get; set; }
		public virtual PageSettings PageSettings { get { return null; } }
		public virtual PrinterSettings PrinterSettings { get { return null; } }
		public virtual void AssignPrinterSettings(string printerName, string paperName, PrinterSettingsUsing settingsUsing) {
		}
		public virtual void Assign(Margins margins, PaperKind paperKind, string paperName, bool landscape) {
		}
		public virtual void AssignDefaultPrinterSettings(PrinterSettingsUsing settingsUsing) { 
		}
		public virtual ProgressReflector ActiveProgressReflector {
			get { return null; }
		}
		public virtual void AddCommandHandler(ICommandHandler handler) {
		}
		public virtual void RemoveCommandHandler(ICommandHandler handler) {
		}
		public virtual void ExecCommand(PrintingSystemCommand command, object[] args) {
		}
		public virtual void ExecCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl) {
		}
		public virtual void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility, Priority priority) {
		}
		public virtual CommandVisibility GetCommandVisibility(PrintingSystemCommand command) {
			return CommandVisibility.None;
		}
		public virtual void EnableCommand(PrintingSystemCommand command, bool enabled) {
		}
		public virtual void Print(string printerName) {
		}
		public virtual void OnBeginCreateDocument() {
		}
		public virtual void Clear() {
		}
	}
	public class PrintingSystemExtenderPrint : PrintingSystemExtenderBase {
		protected PrintingSystemBase printingSystem;
		string predefinedPageRange = string.Empty;
		PageSettings pageSettings;
		protected XtraPageSettingsBase XPageSettings { 
			get; 
			private set;
		}
		public override string PredefinedPageRange {
			get {
				if(string.IsNullOrEmpty(predefinedPageRange))
					return new PageScope(1, printingSystem.PageCount).PageRange;
				return predefinedPageRange;
			}
			set {
				predefinedPageRange = value;
			}
		}
		public override PageSettings PageSettings {
			get {
				if(pageSettings == null) {
					pageSettings = (PageSettings)PageSettingsHelper.DefaultPageSettings.Clone();
					if(pageSettings.PrinterSettings != null) {
						pageSettings.PrinterSettings = (PrinterSettings)pageSettings.PrinterSettings.Clone();
						PageSettings defaultPageSettings = (PageSettings)PageSettingsHelper.DefaultPageSettings.Clone();
						defaultPageSettings.PrinterSettings = pageSettings.PrinterSettings;
						PageSettingsHelper.SetDefaultPageSettings(pageSettings.PrinterSettings, defaultPageSettings);
					}
				}
				return pageSettings;
			}
		}
		public override PrinterSettings PrinterSettings {
			get { 
				return PageSettings.PrinterSettings; 
			}
		}
		public PrintingSystemExtenderPrint(PrintingSystemBase printingSystem) {
			this.printingSystem = printingSystem;
			XPageSettings = printingSystem.PageSettings;
		}
		public override void AssignPrinterSettings(string printerName, string paperName, PrinterSettingsUsing settingsUsing) {
			XPageSettings.PrinterName = printerName;
			XPageSettings.PaperName = paperName;
			AssignDefaultPrinterSettings(settingsUsing);
		}
		public override void AssignDefaultPrinterSettings(PrinterSettingsUsing settingsUsing) {
			if(!settingsUsing.AnySettingUsed)
				return;
			AssignDefaultPrinterSettingsCore(settingsUsing, PageSettingsHelper.DefaultPageSettings);
		}
		void AssignDefaultPrinterSettingsCore(PrinterSettingsUsing settingsUsing, PageSettings defaultPageSettings) {
			try {
				Margins newMargins = settingsUsing.UseMargins ? defaultPageSettings.Margins : XPageSettings.Margins;
				PaperKind newPaperKind = settingsUsing.UsePaperKind ? defaultPageSettings.PaperSize.Kind : XPageSettings.PaperKind;
				System.Drawing.Size newSize = settingsUsing.UsePaperKind ? PageData.ToSize(defaultPageSettings.PaperSize) : XPageSettings.Data.Size;
				bool newLandscape = settingsUsing.UseLandscape ? defaultPageSettings.Landscape : XPageSettings.Landscape;
				XPageSettings.Assign(new PageData(newMargins, new Margins(0, 0, 0, 0), newPaperKind, newSize, newLandscape));
			} catch {
			}
		}
		public override void Assign(Margins margins, PaperKind paperKind, string paperName, bool landscape) {
			try {
				PageData pageData = new PageData(margins, new Margins(0, 0, 0, 0), GetPaperSize(XPageSettings.PrinterName, paperKind, paperName), landscape);
				pageData.PaperName = paperName;
				XPageSettings.Assign(pageData);
			} catch {
			}
		}
		PaperSize GetPaperSize(string printerName, PaperKind paperKind, string paperName) {
			string defaultPrinterName = PrinterSettings.PrinterName;
			try {
				if(printerName != String.Empty)
					PrinterSettings.PrinterName = printerName;
				return GetPaperSize(paperKind, paperName);
			} finally {
				PrinterSettings.PrinterName = defaultPrinterName;
			}
		}
		PaperSize GetPaperSize(PaperKind paperKind, string paperName) {
			foreach(PaperSize paperSize in PrinterSettings.PaperSizes)
				if(IsActualParerSize(paperSize, paperKind, paperName))
					return paperSize;
			return PageSettingsHelper.CreateLetterPaperSize();
		}
		static bool IsActualParerSize(PaperSize paperSize, PaperKind paperKind, string paperName) {
			return paperSize.Kind == paperKind && (paperSize.Kind != PaperKind.Custom || paperSize.PaperName == paperName);
		}
		public override void Print(string printerName) {
			if(printingSystem.Document.IsEmpty)
				return;
			string pageSettingsPrinterName = String.Empty;
			string defaultPageSettingsPrinterName = String.Empty;
			PrinterSettings printerSettings = PrinterSettings;
			if(!string.IsNullOrEmpty(printerName)) {
				pageSettingsPrinterName = printerSettings.PrinterName;
				printerSettings.PrinterName = printerName;
			}
			PSPrintDocument pd = CreatePrintDocument(printingSystem);
			try {
				printingSystem.RemoveService(typeof(IBrickPublisher), true);
				if(!string.IsNullOrEmpty(printerName)) {
					defaultPageSettingsPrinterName = pd.DefaultPageSettings.PrinterSettings.PrinterName;
					pd.DefaultPageSettings.PrinterSettings.PrinterName = printerName;
				}
				printingSystem.OnStartPrint(new PrintDocumentEventArgs(pd));
				pd.PageRange = GetPageRange(pd.PrinterSettings);
				PrintDocument(pd);
				printingSystem.OnEndPrint(EventArgs.Empty);
			} finally {
				if(!string.IsNullOrEmpty(defaultPageSettingsPrinterName))
					pd.DefaultPageSettings.PrinterSettings.PrinterName = defaultPageSettingsPrinterName;
				if(!string.IsNullOrEmpty(pageSettingsPrinterName))
				  printerSettings.PrinterName = pageSettingsPrinterName;
				pd.Dispose();
			}
		}
		protected static string GetPageRange(PrinterSettings printerSettings) {
			return new PageScope(printerSettings.FromPage, printerSettings.ToPage).PageRange;
		}
		protected virtual PSPrintDocument CreatePrintDocument(PrintingSystemBase ps) {
			PrinterSettings printerSettings = PrinterSettings;
			printerSettings.MinimumPage = 1;
			printerSettings.MaximumPage = ps.Document.PageCount;
			PageScope pageScope = new PageScope(PredefinedPageRange, ps.Document.PageCount);
			printerSettings.FromPage = pageScope.FromPage;
			printerSettings.ToPage = pageScope.ToPage;
			return new PSPrintDocument(ps, ps.Graph.PageBackColor, GetActualPrintController(), printerSettings, PredicateMargins) { PageRange = PredefinedPageRange };
		}
		protected virtual bool PredicateMargins() {
			return true;
		}
		PrintController GetActualPrintController() {
			return (PSNativeMethods.AspIsRunning || !printingSystem.ShowPrintStatusDialog) ?
				new StandardPrintController() : null;
		}
		protected void PrintDocument(PrintDocument pd) {
			CursorStorage.SetCursor(Cursors.WaitCursor);
			try {
				printingSystem.PrintDocument(pd);
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
				try {
					CancelPrint(pd);
				} catch(Exception ex2) {
					Tracer.TraceError(NativeSR.TraceSource, ex2);
					throw new AggregateException(new Exception[] { ex, ex2 });
				}
				throw;
			} finally {
				CursorStorage.RestoreCursor();
			}
		}
		static void CancelPrint(PrintDocument pd) {
			if(pd != null && pd.PrintController != null) {
				pd.PrintController.OnEndPrint(pd, new PrintEventArgs() { Cancel = true });
			}
		}
	}
}
