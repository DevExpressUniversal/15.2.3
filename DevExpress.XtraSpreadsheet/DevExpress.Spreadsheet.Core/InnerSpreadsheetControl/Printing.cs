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
using DevExpress.XtraPrinting;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
#if !DXPORTABLE
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraSpreadsheet.Printing;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	partial class InnerSpreadsheetDocumentServer : IPrintable {
		#region Fields
		int sheetIndex = -1;
		bool sheetMode;
		#endregion
#if !DXPORTABLE
		protected internal virtual SpreadsheetPrinter CreateSpreadsheetPrinter() {
			SpreadsheetPrinter printer = !SheetMode ? new SpreadsheetPrinter(this) : new SpreadsheetWorksheetPrinter(this, SheetIndex);
			return printer;
		}
#endif
		protected internal IPrintable PrintableImplementation {
			get {
				IPrintable result = GetService<IPrintable>();
				if (result == null)
					result = LoadIPrintableImplementation();
				return result;
			}
		}
		protected internal void ResetPrintableImplementation(bool sheetMode) {
			ResetPrintableImplementation(sheetMode, -1);
		}
		protected internal void ResetPrintableImplementation(bool sheetMode, int sheetIndex) {
			SheetMode = sheetMode;
			SheetIndex = sheetIndex;
			IPrintable service = GetService<IPrintable>();
			if (service == null)
				return;
#if !DXPORTABLE
			if (sheetMode) {
				SpreadsheetWorksheetPrinter printer = service as SpreadsheetWorksheetPrinter;
				if (printer == null || printer.SheetIndex != sheetIndex)
					RemoveService(typeof(IPrintable));
			}
			else {
				if (service.GetType() != typeof(SpreadsheetPrinter))
					RemoveService(typeof(IPrintable));
			}
#endif
		}
#if DXPORTABLE
		class EmptyPrintableImplementation : IPrintable {
			public bool CreatesIntersectedBricks { get { return false; } }
			public UserControl PropertyEditorControl { get { return null; } }
			public void AcceptChanges() {
			}
			public void CreateArea(string areaName, IBrickGraphics graph) {
			}
			public void Finalize(IPrintingSystem ps, ILink link) {
			}
			public bool HasPropertyEditor() {
				return false;
			}
			public void Initialize(IPrintingSystem ps, ILink link) {
			}
			public void RejectChanges() {
			}
			public void ShowHelp() {
			}
			public bool SupportsHelp() {
				return false;
			}
		}
#endif
		protected internal virtual IPrintable LoadIPrintableImplementation() {
#if DXPORTABLE
			return new EmptyPrintableImplementation();
#else
			SpreadsheetPrinter instance = CreateSpreadsheetPrinter();
			AddService(typeof(IPrintable), instance);
			return instance;
#endif
		}
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
#if !SL && !DXPORTABLE
		public void ExportToPdf(Stream stream) {
			ExportToPdfCore(stream, null);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			ExportToPdfCore(stream, options);
		}
		void ExportToPdfCore(Stream stream, PdfExportOptions options) {
			if (!IsPrintingAvailable)
				return;
			using (PrintingSystemBase printingSystem = new PrintingSystemBase()) {
				using (PrintableComponentLinkBase link = new PrintableComponentLinkBase(printingSystem)) {
					link.Component = this;
					link.CreateDocument();
					if (options != null)
						link.PrintingSystemBase.ExportToPdf(stream, options);
					else
						link.PrintingSystemBase.ExportToPdf(stream);
				}
			}
		}
		public void ExportToPdf(string fileName) {
			if (!IsPrintingAvailable)
				return;
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				ExportToPdf(stream);
			}
		}
		public void ExportToPdf(string fileName, PdfExportOptions options) {
			if (!IsPrintingAvailable)
				return;
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				ExportToPdf(stream, options);
			}
		}
		public void ExportToImage(Stream stream) {
			if (!IsPrintingAvailable)
				return;
			using (PrintingSystemBase printingSystem = new PrintingSystemBase()) {
				using (PrintableComponentLinkBase link = new PrintableComponentLinkBase(printingSystem)) {
					link.Component = this;
					link.CreateDocument();
					link.PrintingSystemBase.ExportOptions.Image.ExportMode = ImageExportMode.SingleFile;
					link.PrintingSystemBase.ExportOptions.Image.Format = System.Drawing.Imaging.ImageFormat.Png;
					link.PrintingSystemBase.ExportToImage(stream);
				}
			}
		}
		public void ExportToImage(string fileName) {
			if (!IsPrintingAvailable)
				return;
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				ExportToImage(stream);
			}
		}
#endif
		public bool IsPrintingAvailable { get { return PrintableImplementation != null; } }
		public bool SheetMode { get { return sheetMode; } set { sheetMode = value; } }
		public int SheetIndex { get { return sheetIndex; } set { sheetIndex = value; } }
	}
}
