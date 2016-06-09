#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.BarCodes.Internal;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraPrintingLinks;
using DevExpress.Utils;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.BarCodes {
	partial class BarCode : IPrintable {
		BarCodePrinter instance;
		protected internal virtual BarCodePrinter CreateBarCodePrinter() {
			BarCodePrinter printer = new BarCodePrinter(this);
			return printer;
		}
		protected internal IPrintable PrintableImplementation {
			get {
				return LoadIPrintableImplementation();
			}
		}
		protected internal virtual IPrintable LoadIPrintableImplementation() {
			if (instance == null)
				instance = CreateBarCodePrinter();
			return instance;
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
		#region Printing support methods and properties
		ComponentPrinterBase printer;
		ComponentPrinterBase Printer {
			get {
				if (printer == null) {
					PrintingSystemBase printingSystem = new PrintingSystemBase();
					printer = new ComponentPrinterDynamic(this, printingSystem);
				}
				return printer;
			}
		}
		#endregion
		public void ShowPrintPreview() {
			ExecutePrinterAction(delegate() {
				Printer.ShowPreview(null);
			});
		}
		public void Print() {
			ExecutePrinterAction(delegate() {
				Printer.Print();
			});
		}
		public void PrintDialog() {
			ExecutePrinterAction(delegate() {
				Printer.PrintDialog();
			});
		}
		void ExecutePrinterAction(Action0 action) {
			Printer.ClearDocument();
			action();
		}
#if !SL
		public void ExportToPdf(Stream stream) {
			if (!IsPrintingAvailable)
				return;
			using (PrintingSystemBase printingSystem = new PrintingSystemBase()) {
				using (PrintableComponentLinkBase link = new PrintableComponentLinkBase(printingSystem)) {
					link.Component = this;
					link.CreateDocument();
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
#endif
		public bool IsPrintingAvailable { get { return PrintableImplementation != null; } }
	}
}
