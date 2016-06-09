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
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Localization;
#if !DXPORTABLE
using DevExpress.XtraRichEdit.Printing;
using DevExpress.XtraPrintingLinks;
using DevExpress.Pdf;
#endif
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Utils;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	internal interface IInternalRichEditDocumentServerOwner {
		InternalRichEditDocumentServer InternalServer { get; }
	}
	partial class InternalRichEditDocumentServer : IPrintable {
		protected internal IPrintable PrintableImplementation { get { return InnerServer; } }
		protected internal virtual void CheckPrintableImplmenentation() {
#if !SL && !DXPORTABLE
			if (PrintableImplementation == null) {
				string message = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_PrintingUnavailable),
					AssemblyInfo.SRAssemblyPrinting + ", Version=" + AssemblyInfo.Version,
					String.Empty);
				throw new NotSupportedException(message);
			}
#else
			if (PrintableImplementation == null)
				throw new NotSupportedException();
#endif
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
#if !SL && !DXPORTABLE
		public void ExportToPdf(Stream stream) {
			ExportToPdf(stream, new PdfCreationOptions() { Compatibility = PdfCompatibility.Pdf }, new PdfSaveOptions());
		}
		public void ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			PdfDocument document = new PdfDocument(stream, creationOptions, saveOptions);
			Rectangle bounds = new Rectangle(0, 0, Int32.MaxValue / 4, Int32.MaxValue / 4);
			PdfRichEditPrinter printer = new PdfRichEditPrinter(DocumentModel, creationOptions);
			DevExpress.XtraRichEdit.Layout.DocumentLayout layout = printer.CalculatePrintDocumentLayout();
			PdfGraphicsDocumentLayoutExporter exporter = new PdfGraphicsDocumentLayoutExporter(document, layout, new PdfPainter(layout), new PdfAdapter(), bounds, TextColors.Defaults);
			exporter.ExportToPdf(stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions pdfExportOptions) {
			if(!IsPrintingAvailable)
				return;
			using(ExportToPdfPrintingSystem printingSystem = new ExportToPdfPrintingSystem()) {
				using(PrintableComponentLinkBase link = new PrintableComponentLinkBase(printingSystem)) {
					link.Component = this;
					link.CreateDocument();
					link.PrintingSystemBase.ExportToPdf(stream, pdfExportOptions);
				}
			}
		}
#endif
		public bool IsPrintingAvailable { get { return PrintableImplementation != null; } }
#endregion
	}
}
namespace DevExpress.XtraRichEdit.Internal {
#if !DXPORTABLE
	public partial class InnerRichEditControl {
		protected internal override RichEditPrinter CreateRichEditPrinter() {
			return new RichEditControlPrinter(this);
		}
		protected internal void ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			PdfDocument document = new PdfDocument(stream, creationOptions, saveOptions);
			Rectangle bounds = new Rectangle(0, 0, Int32.MaxValue / 4, Int32.MaxValue / 4);
			PdfRichEditPrinter printer = new PdfRichEditPrinter(DocumentModel, creationOptions);
			DevExpress.XtraRichEdit.Layout.DocumentLayout layout = printer.CalculatePrintDocumentLayout();
			PdfGraphicsDocumentLayoutExporter exporter = new PdfGraphicsDocumentLayoutExporter(document, layout, new PdfPainter(layout), new PdfAdapter(), bounds, TextColors.Defaults);
			exporter.ExportToPdf(stream);
		}
	}
#endif
	public partial class InnerRichEditDocumentServer : IPrintable {
#if !DXPORTABLE
		protected internal virtual RichEditPrinter CreateRichEditPrinter() {
			return new RichEditPrinter(this);
		}
#endif
		protected internal IPrintable PrintableImplementation {
			get {
#if !DXPORTABLE
				IPrintable result = GetService<IPrintable>();
				if (result == null)
					result = LoadIPrintableImplementation();
				return result;
#else
				return null;
#endif
			}
		}
#if !DXPORTABLE
		protected internal virtual IPrintable LoadIPrintableImplementation() {
			RichEditPrinter instance = CreateRichEditPrinter();
			AddService(typeof(IPrintable), instance);
			return instance;
		}
#endif
		protected internal virtual void CheckPrintableImplmenentation() {
#if !SL && !DXPORTABLE
			if (PrintableImplementation == null) {
				string message = String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_PrintingUnavailable),					
					AssemblyInfo.SRAssemblyPrinting + ", Version=" + AssemblyInfo.Version,
					String.Empty);
				throw new NotSupportedException(message);
			}
#else
			if (PrintableImplementation == null)
				throw new NotSupportedException();
#endif
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
	}
}
