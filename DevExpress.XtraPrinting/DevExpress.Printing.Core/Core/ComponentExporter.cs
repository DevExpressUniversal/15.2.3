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
#if !SL
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrintingLinks;
using System.Drawing;
using System.IO;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting {
	[Flags]
	public enum PrintingSystemActivity { Idle = 0, Preparing = 1, Exporting = 2, Printing = 4 }
#if !SL
	public class ComponentExporter : IDisposable {
		PrintingSystemBase printingSystem;
		PrintingSystemBase outerPrintingSystem;
		PageSettings pageSettings;
		IPrintable component;
		PrintableComponentLinkBase linkBase;
		public event EventHandler<ProgressEventArgs> Progress;
		public IPrintable Component {
			get {
				return component;
			}
			set {
				if(value == null)
					throw new NullReferenceException();
				this.component = value;
				if(linkBase != null) linkBase.Component = component;
			}
		}
		public PrintingSystemActivity Activity { get { return LinkBase.Activity; } }
		protected PrintableComponentLinkBase LinkBase {
			get {
				if(linkBase == null) {
					linkBase = CreateLink();
					linkBase.PrintingSystemBase = PrintingSystemBase;
					linkBase.Component = component;
					ApplyPageSettings(linkBase);
				}
				return linkBase;
			}
		}
		protected virtual PrintableComponentLinkBase CreateLink() {
			return new PrintableComponentLinkBase();
		}
		public PrintingSystemBase PrintingSystemBase {
			get {
				return outerPrintingSystem != null ? outerPrintingSystem : InnerPrintingSystem;
			}
		}
		PrintingSystemBase InnerPrintingSystem {
			get {
				if(printingSystem == null) {
					printingSystem = new PrintingSystemBase();
					printingSystem.AfterChange += new ChangeEventHandler(OnAfterChange);
				}
				return printingSystem;
			}
		}
		public ComponentExporter(IPrintable component) {
			this.component = component;
		}
		public ComponentExporter(IPrintable component, PrintingSystemBase printingSystem) {
			this.component = component;
			this.outerPrintingSystem = printingSystem;
			outerPrintingSystem.AfterChange += new ChangeEventHandler(OnAfterChange);
		}
		void OnAfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(e.EventName == DevExpress.XtraPrinting.SR.ProgressPositionChanged) {
				int pos = (int)e.ValueOf(DevExpress.XtraPrinting.SR.ProgressPosition);
				int maximum = (int)e.ValueOf(DevExpress.XtraPrinting.SR.ProgressMaximum);
				if(Progress != null)
					Progress(this, new ProgressEventArgs(pos, maximum));
			}
		}
		public void Dispose() {
			if(printingSystem != null) {
				printingSystem.Dispose();
				printingSystem.AfterChange -= new ChangeEventHandler(OnAfterChange);
			}
			if(outerPrintingSystem != null)
				outerPrintingSystem.AfterChange -= new ChangeEventHandler(OnAfterChange);
		}
		void ApplyPageSettings(PrintableComponentLinkBase printableLink) {
			if(!IsActualPageSettings(pageSettings) || printableLink == null)
				return;
			printableLink.PaperKind = pageSettings.PaperSize.Kind;
			if(pageSettings.PaperSize.Kind == PaperKind.Custom && pageSettings.PaperSize.Width != 0 && pageSettings.PaperSize.Height != 0)
				printableLink.CustomPaperSize = new Size(pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);
			printableLink.PaperName = pageSettings.PaperSize.PaperName;
			printableLink.Landscape = pageSettings.Landscape;
			printableLink.Margins = pageSettings.Margins;
		}
		static bool IsActualPageSettings(PageSettings pageSettings) {
			if(pageSettings != null) {
				try {
					PaperSize ignore = pageSettings.PaperSize;
				} catch {
					return false;
				}
				return true;
			}
			return false;
		}
		public virtual void Export(ExportTarget target, string filePath, ExportOptionsBase options) {
			switch(target) {
				case ExportTarget.Xls:
					LinkBase.ExportToXls(filePath, (XlsExportOptions)options);
					break;
				case ExportTarget.Xlsx:
					LinkBase.ExportToXlsx(filePath, (XlsxExportOptions)options);
					break;
				case ExportTarget.Html:
					LinkBase.ExportToHtml(filePath, (HtmlExportOptions)options);
					break;
				case ExportTarget.Mht:
					LinkBase.ExportToMht(filePath, (MhtExportOptions)options);
					break;
				case ExportTarget.Pdf:
					LinkBase.ExportToPdf(filePath, (PdfExportOptions)options);
					break;
				case ExportTarget.Text:
					LinkBase.ExportToText(filePath, (TextExportOptions)options);
					break;
				case ExportTarget.Rtf:
					LinkBase.ExportToRtf(filePath, (RtfExportOptions)options);
					break;
				case ExportTarget.Csv:
					LinkBase.ExportToCsv(filePath, (CsvExportOptions)options);
					break;
				case ExportTarget.Image:
					LinkBase.ExportToImage(filePath, (ImageExportOptions)options);
					break;
				default:
					throw new ArgumentOutOfRangeException("target");
			}
		}
		public virtual void Export(ExportTarget target, string filePath) {
			switch(target) {
				case ExportTarget.Xls:
					LinkBase.ExportToXls(filePath);
					break;
				case ExportTarget.Xlsx:
					LinkBase.ExportToXlsx(filePath);
					break;
				case ExportTarget.Html:
					LinkBase.ExportToHtml(filePath);
					break;
				case ExportTarget.Mht:
					LinkBase.ExportToMht(filePath);
					break;
				case ExportTarget.Pdf:
					LinkBase.ExportToPdf(filePath);
					break;
				case ExportTarget.Text:
					LinkBase.ExportToText(filePath);
					break;
				case ExportTarget.Rtf:
					LinkBase.ExportToRtf(filePath);
					break;
				case ExportTarget.Csv:
					LinkBase.ExportToCsv(filePath);
					break;
				case ExportTarget.Image:
					LinkBase.ExportToImage(filePath);
					break;
				default:
					throw new ArgumentOutOfRangeException("target");
			}
		}
		public virtual void Export(ExportTarget target, Stream stream) {
			switch(target) {
				case ExportTarget.Xls:
					LinkBase.ExportToXls(stream);
					break;
				case ExportTarget.Xlsx:
					LinkBase.ExportToXlsx(stream);
					break;
				case ExportTarget.Html:
					LinkBase.ExportToHtml(stream);
					break;
				case ExportTarget.Mht:
					LinkBase.ExportToMht(stream);
					break;
				case ExportTarget.Pdf:
					LinkBase.ExportToPdf(stream);
					break;
				case ExportTarget.Text:
					LinkBase.ExportToText(stream);
					break;
				case ExportTarget.Rtf:
					LinkBase.ExportToRtf(stream);
					break;
				case ExportTarget.Csv:
					LinkBase.ExportToCsv(stream);
					break;
				case ExportTarget.Image:
					LinkBase.ExportToImage(stream);
					break;
				default:
					throw new ArgumentOutOfRangeException("target");
			}
		}
		public virtual void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			switch(target) {
				case ExportTarget.Xls:
					LinkBase.ExportToXls(stream, (XlsExportOptions)options);
					break;
				case ExportTarget.Xlsx:
					LinkBase.ExportToXlsx(stream, (XlsxExportOptions)options);
					break;
				case ExportTarget.Html:
					LinkBase.ExportToHtml(stream, (HtmlExportOptions)options);
					break;
				case ExportTarget.Mht:
					LinkBase.ExportToMht(stream, (MhtExportOptions)options);
					break;
				case ExportTarget.Pdf:
					LinkBase.ExportToPdf(stream, (PdfExportOptions)options);
					break;
				case ExportTarget.Text:
					LinkBase.ExportToText(stream, (TextExportOptions)options);
					break;
				case ExportTarget.Rtf:
					LinkBase.ExportToRtf(stream, (RtfExportOptions)options);
					break;
				case ExportTarget.Csv:
					LinkBase.ExportToCsv(stream, (CsvExportOptions)options);
					break;
				case ExportTarget.Image:
					LinkBase.ExportToImage(stream, (ImageExportOptions)options);
					break;
				default:
					throw new ArgumentOutOfRangeException("target");
			}
		}
		public void SetPageSettings(PageSettings newPageSettings) {
			pageSettings = newPageSettings != null ?
				(PageSettings)newPageSettings.Clone() : null;
			ApplyPageSettings(linkBase);
		}
		public bool IsDocumentEmpty { get { return PrintingSystemBase.Document.IsEmpty; } }
		public void ClearDocument() {
			LinkBase.ClearDocument();
		}
		public void CreateDocument() {
			LinkBase.CreateDocument();
		}
	}
#endif
}
