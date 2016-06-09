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

using DevExpress.LookAndFeel;
using DevExpress.Printing.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Links;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrintingLinks;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting {
	public static class ExportOptionsTool {
		public static DialogResult EditExportOptions(ExportOptionsBase options, PrintingSystemBase ps, IDictionary<Type,object[]> disabledExportModes) {
			ExportOptionsControllerBase controller = ExportOptionsControllerBase.GetControllerByOptions(options);
			ExportOptionsBase clonedOptions = ExportOptionsHelper.CloneOptions(options);
			BaseLine[] lines = Array.ConvertAll<ILine, BaseLine>(controller.GetExportLines(clonedOptions, new LineFactory(), disabledExportModes != null ? GetAvailableModes(ps.Document.AvailableExportModes, disabledExportModes) : ps.Document.AvailableExportModes, ps.ExportOptions.HiddenOptions), delegate(ILine line) { return (BaseLine)line; });
			if(lines.Length > 0) {
				using(LinesForm linesForm = new LinesForm(lines, ps.Extend().LookAndFeel, controller.CaptionStringId.GetString())) {
					if(DialogRunner.ShowDialog(linesForm, ps.Extend().FindForm()) == DialogResult.OK) {
						if(clonedOptions is PageByPageExportOptionsBase && !IsSingleFileExport(clonedOptions) &&
							PageRangeParser.GetIndices(((PageByPageExportOptionsBase)clonedOptions).PageRange, ps.Pages.Count).Length <= 0)
							throw new ArgumentException(PreviewStringId.Msg_IncorrectPageRange.GetString());
						options.Assign(clonedOptions);
						return DialogResult.OK;
					}
				}
			}
			return DialogResult.Cancel;
		}
		public static DialogResult EditExportOptions(ExportOptionsBase options, PrintingSystemBase ps) {
			return EditExportOptions(options, ps, null);
		}
		static AvailableExportModes GetAvailableModes(AvailableExportModes documentAvailableModes, IDictionary<Type, object[]> disabledExportModes) {
			var res =  new AvailableExportModes(
			GetAvailableByUserModes<RtfExportMode>(documentAvailableModes.Rtf, DevExpress.Utils.ArrayHelper.ConvertAll<object, RtfExportMode>(disabledExportModes[typeof(RtfExportMode)], obj => (RtfExportMode)obj)),
			GetAvailableByUserModes<HtmlExportMode>(documentAvailableModes.Html, DevExpress.Utils.ArrayHelper.ConvertAll<object, HtmlExportMode>(disabledExportModes[typeof(HtmlExportMode)], obj => (HtmlExportMode)obj)),
			GetAvailableByUserModes<ImageExportMode>(documentAvailableModes.Image, DevExpress.Utils.ArrayHelper.ConvertAll<object, ImageExportMode>(disabledExportModes[typeof(ImageExportMode)], obj => (ImageExportMode)obj)),
			GetAvailableByUserModes<XlsExportMode>(documentAvailableModes.Xls, DevExpress.Utils.ArrayHelper.ConvertAll<object, XlsExportMode>(disabledExportModes[typeof(XlsExportMode)], obj => (XlsExportMode)obj)),
			GetAvailableByUserModes<XlsxExportMode>(documentAvailableModes.Xlsx, DevExpress.Utils.ArrayHelper.ConvertAll<object, XlsxExportMode>(disabledExportModes[typeof(XlsxExportMode)], obj => (XlsxExportMode)obj)));
			return res;
		}
		static IEnumerable<T> GetAvailableByUserModes<T>(IEnumerable<T> documentAvailableModes, T[] disabledExportModes) {
			var result = documentAvailableModes.Where(c => !disabledExportModes.Contains(c)).ToList();
			if(result.Count() == 0) result.Add(documentAvailableModes.First());
			return result.ToArray();
		}
		static bool IsSingleFileExport(ExportOptionsBase clonedOptions) {
			return ((clonedOptions is RtfExportOptions && ((RtfExportOptions)clonedOptions).ExportMode == RtfExportMode.SingleFile) ||
			(clonedOptions is HtmlExportOptionsBase && ((HtmlExportOptionsBase)clonedOptions).ExportMode == HtmlExportMode.SingleFile) ||
			(clonedOptions is XlsExportOptions && ((XlsExportOptions)clonedOptions).ExportMode == XlsExportMode.SingleFile) ||
			(clonedOptions is XlsxExportOptions && ((XlsxExportOptions)clonedOptions).ExportMode == XlsxExportMode.SingleFile) ||
			(clonedOptions is ImageExportOptions && ((ImageExportOptions)clonedOptions).ExportMode == ImageExportMode.SingleFile));
		}
	}
	public class PrintTool : PrintToolBase, IDisposable {
		public static void MakeCommandResponsive(PrintingSystemBase printingSystem) {
			printingSystem.Extend();
		}
		PreviewFormContainer formContainer;
		internal virtual PreviewFormContainer FormContainer {
			get {
				if(formContainer == null)
					formContainer = new PreviewFormContainer(PrintingSystem);
				return formContainer;
			}
		}
		public PrintPreviewFormEx PreviewForm {
			get { return FormContainer.PreviewForm; }
		}
		public PrintPreviewRibbonFormEx PreviewRibbonForm {
			get { return FormContainer.PreviewRibbonForm; }
		}
		public PrintTool(PrintingSystemBase printingSystem) : base(printingSystem){
		}
		protected override void ExtendPrintingSystem(PrintingSystemBase printingSystem) {
			printingSystem.Extend();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(formContainer != null) {
					formContainer.Dispose();
					formContainer = null;
				}
				PrintingSystem = null;
			}
		}
		protected bool CancelPending { get { return PrintingSystem.CancelPending; } }
		void ShowPreview(XtraForm form, UserLookAndFeel lookAndFeel) {
			BeforeShowPreview(form, lookAndFeel);
			if(CancelPending) return;
			((IPrintPreviewForm)form).Show(lookAndFeel);
		}
		void ShowPreview(XtraForm form, IWin32Window owner, UserLookAndFeel lookAndFeel) {
			BeforeShowPreview(form, lookAndFeel);
			if(CancelPending) return;
			((IPrintPreviewForm)form).Show(owner, lookAndFeel);
		}
		void ShowPreviewDialog(XtraForm form, IWin32Window owner, UserLookAndFeel lookAndFeel) {
			try {
				BeforeShowPreview(form, lookAndFeel);
				if(!CancelPending) {
					((IPrintPreviewForm)form).ShowDialog(owner, lookAndFeel);
				}
			} finally {
				form.Dispose();
			}
		}
		protected virtual void BeforeShowPreview(XtraForm form, UserLookAndFeel lookAndFee) {
		}
		public void ShowPreview() {
			ShowPreview(null);
		}
		public void ShowPreview(UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreview(PreviewForm, lookAndFeel);
		}
		public void ShowPreview(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreview(PreviewForm, owner, lookAndFeel);
		}
		public void ClosePreview() {
			PreviewForm.Close();
		}
		public void ShowPreviewDialog() {
			ShowPreviewDialog(null);
		}
		public void ShowPreviewDialog(UserLookAndFeel lookAndFeel) {
			ShowPreviewDialog(null, lookAndFeel);
		}
		public void ShowPreviewDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreviewDialog(PreviewForm, owner, lookAndFeel);
		}
		public void ShowRibbonPreview() {
			ShowRibbonPreview(null);
		}
		public void ShowRibbonPreview(UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreview(PreviewRibbonForm, lookAndFeel);
		}
		public void ShowRibbonPreview(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreview(PreviewRibbonForm, owner, lookAndFeel);
		}
		public void CloseRibbonPreview() {
			PreviewRibbonForm.Close();
		}
		public void ShowRibbonPreviewDialog() {
			ShowRibbonPreviewDialog(null);
		}
		public void ShowRibbonPreviewDialog(UserLookAndFeel lookAndFeel) {
			ShowRibbonPreviewDialog(null, lookAndFeel);
		}
		public void ShowRibbonPreviewDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null)
				ShowPreviewDialog(PreviewRibbonForm, owner, lookAndFeel);
		}
		public bool? PrintDialog() {
			if(PrintingSystem != null) {
				BeforePrint();
				return PrintingSystem.Extend().PrintDlg() == DialogResult.OK;
			}
			return false;
		}
		public bool? PrintDialog(IWin32Window owner) {
			return PrintDialog(owner, null);
		}
		public bool? PrintDialog(UserLookAndFeel lookAndFeel) {
			return PrintDialog(null, lookAndFeel);
		}
		public bool? PrintDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			if(PrintingSystem != null) {
				BeforePrint();
				return PrintingSystem.Extend().PrintDlg(owner, lookAndFeel) == DialogResult.OK;
			}
			return false;
		}
		public virtual bool? ShowPageSetup() {
			return PrintingSystem.Extend().ShowPageSetup(null, null);
		}
		public void SavePrinterSettings(string filePath) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.AssignFrom(PrinterSettings);
			printerSettings.SaveToXml(filePath);
		}
		public void LoadPrinterSettings(string filePath) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.RestoreFromXml(filePath);
			ApplyPrinterSettings(printerSettings);
		}
		void ApplyPrinterSettings(XtraPrinterSettings printerSettings) {
			printerSettings.AssignTo(PrinterSettings);
			PrintingSystem.Extender.PredefinedPageRange = new PageScope(printerSettings.FromPage, printerSettings.ToPage).PageRange;
		}
		public void SavePrinterSettingsToRegistry(string path) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.AssignFrom(PrinterSettings);
			printerSettings.SaveToRegistry(path);
		}
		public void LoadPrinterSettingsFromRegistry(string path) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.RestoreFromRegistry(path);
			ApplyPrinterSettings(printerSettings);
		}
		public void SavePrinterSettingsToStream(Stream stream) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.AssignFrom(PrinterSettings);
			printerSettings.SaveToStream(stream);
		}
		public void LoadPrinterSettingsFromStream(Stream stream) {
			XtraPrinterSettings printerSettings = new XtraPrinterSettings();
			printerSettings.RestoreFromStream(stream);
			ApplyPrinterSettings(printerSettings);
		}
	}
	public class ComponentPrinter : ComponentPrinterBase {
		static ComponentPrinter() {
			BrickResolver.EnsureStaticConstructor();
		}
		public ComponentPrinter(IPrintable component)
			: base(component) {
		}
		public ComponentPrinter(IPrintable component, PrintingSystemBase printingSystem)
			: base(component, printingSystem) {
		}
		protected override PrintableComponentLinkBase CreateLink() {
			return new PrintableComponentLink();
		}
		protected PrintableComponentLink Link { get { return (PrintableComponentLink)this.LinkBase; } }
		public override void Print() {
			Link.Print("");
		}
		public override void PrintDialog() {
			Link.PrintDlg();
		}
		public override Form ShowPreview(IWin32Window owner, object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			tool.ShowPreview(owner, (UserLookAndFeel)lookAndFeel);
			return tool.PreviewForm;
		}
		public override Form ShowPreview(object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			tool.ShowPreview((UserLookAndFeel)lookAndFeel);
			return tool.PreviewForm;
		}
		public override Form ShowRibbonPreview(IWin32Window owner, object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			tool.ShowRibbonPreview(owner, (UserLookAndFeel)lookAndFeel);
			return tool.PreviewRibbonForm;
		}
		public override Form ShowRibbonPreview(object lookAndFeel) {
			var tool = new LinkPrintTool(Link);
			tool.ShowRibbonPreview((UserLookAndFeel)lookAndFeel);
			return tool.PreviewRibbonForm;
		}
		public override PageSettings PageSettings {
			get {
				return PrintingSystemBase.Extend().PageSettings;
			}
		}
	}
}
