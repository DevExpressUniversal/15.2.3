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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Printing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
#if !DXPORTABLE
using DevExpress.Pdf;
#endif
#if !SL
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.Data.Helpers;
using System.Security.Permissions;
using PlatformIndependentBrush = System.Drawing.Brush;
using DevExpress.Office.Printing;
#else
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using PlatformIndependentBrush = System.Windows.Media.Brush;
#endif
namespace DevExpress.XtraRichEdit.Printing {
	#region RichEditPrinter
	public class RichEditPrinter : RichEditPrinterBase, IPrintable {
		public RichEditPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		public RichEditPrinter(InnerRichEditDocumentServer server)
			: base(server) {
		}
		#region IPrintable Members
		public bool CreatesIntersectedBricks { get { return true; } }
		public bool HasPropertyEditor() {
			return false;
		}
		public UserControl PropertyEditorControl { get { return null; } }
		public void AcceptChanges() {
		}
		public void RejectChanges() {
		}
		public void ShowHelp() {
		}
		public bool SupportsHelp() {
			return false;
		}
		#endregion
		protected internal virtual bool UseGdiPlus { get { return false; } }
		#region IBasePrintable Members
		public void CreateArea(string areaName, IBrickGraphics graph) {
			if (areaName == SR.Detail && Server != null) {
				PrintingSystemBase ps = ((BrickGraphics)graph).PrintingSystem;
				DrawContent(documentLayout, (DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocument)Server.Document, ps, false);
				if (commentDocumentLayout.DocumentModel.MainPieceTable.DocumentEndLogPosition > DocumentLogPosition.Zero){
					DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocument doc = new API.Native.Implementation.NativeDocument(commentDocumentLayout.DocumentModel.MainPieceTable, Server);
					DrawContent(commentDocumentLayout, doc, ps, true);
				}
			}
		}
		void DrawContent(DocumentLayout documentLayout, DevExpress.XtraRichEdit.API.Native.Implementation.NativeDocument document, PrintingSystemBase ps, bool startFromFirstPage) {
			using (PrintingDocumentExporter exporter = CreateDocumentExporter(documentLayout.DocumentModel)) {
				exporter.StartFromNotFirstPage = startFromFirstPage;
				exporter.Export(documentLayout, ps);
				for (int i = 0; i < documentLayout.Pages.Count; i++) {
					DevExpress.XtraRichEdit.API.Layout.PagePainter pagePainter = new API.Layout.PagePainter();
					DevExpress.XtraRichEdit.API.Layout.LayoutPage layoutPage = new API.Layout.LayoutPage(documentLayout.Pages[i], document);
					pagePainter.Initialize(exporter, layoutPage);
					BeforePagePaintEventArgs args = new BeforePagePaintEventArgs(pagePainter, layoutPage, exporter, API.Layout.CanvasOwnerType.Printer);
					Server.RaiseBeforePagePaint(args);
					if (args.Painter != null)
						args.Painter.Draw();
				}
			}
		}
		protected virtual PrintingDocumentExporter CreateDocumentExporter(DocumentModel documentModel) {
			return new PrintingDocumentExporter(documentModel, TextColors.Defaults);
		}
		DocumentLayout documentLayout;
		DocumentLayout commentDocumentLayout;
		public void Initialize(IPrintingSystem ps, ILink link) {
			PrintingSystemBase printingSystem = (PrintingSystemBase)ps;
			if (!UseGdiPlus) {
				DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
				if (unitConverter is DocumentLayoutUnitPixelsConverter || unitConverter is DocumentLayoutUnitTwipsConverter)
					unitConverter = new DocumentLayoutUnitDocumentConverter();
				SwitchToGdiMode(printingSystem, unitConverter);
			}
			HideUnsupportedCommands(printingSystem);
			this.documentLayout = CalculatePrintDocumentLayout();
			int position = printingSystem.PageBounds.Width - printingSystem.PageMargins.Left - printingSystem.PageMargins.Right;
			this.commentDocumentLayout = CalculatePrintCommentDocumentLayout(documentLayout, position);
			printingSystem.Graph.PageUnit = GraphicsUnit.Document;
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
		}
		#endregion
		protected internal virtual void SwitchToGdiMode(PrintingSystemBase ps, DocumentLayoutUnitConverter unitConverter) {
			IServiceContainer serviceContainer = ps;
#if !SL
			if (SecurityHelper.IsUnmanagedCodeGrantedAndCanUseGetHdc) {
				serviceContainer.RemoveService(typeof(GraphicsModifier));
				serviceContainer.AddService(typeof(GraphicsModifier), new GdiGraphicsModifier(unitConverter));
				serviceContainer.RemoveService(typeof(Measurer));
				serviceContainer.AddService(typeof(Measurer), new GdiMeasurer(unitConverter));
			}
#endif
		}
		#region Unsupported commands list
		static PrintingSystemCommand[] unsupportedCommands = new PrintingSystemCommand[] {
			PrintingSystemCommand.DocumentMap,
			PrintingSystemCommand.ExportCsv,
			PrintingSystemCommand.ExportHtm,
			PrintingSystemCommand.ExportMht,
			PrintingSystemCommand.ExportRtf,
			PrintingSystemCommand.ExportTxt,
			PrintingSystemCommand.ExportXls,
			PrintingSystemCommand.Open,
			PrintingSystemCommand.Save,
			PrintingSystemCommand.PageMargins,
			PrintingSystemCommand.PageOrientation,
			PrintingSystemCommand.PageSetup,
			PrintingSystemCommand.PaperSize,
			PrintingSystemCommand.Scale,
			PrintingSystemCommand.SendCsv,
			PrintingSystemCommand.SendMht,
			PrintingSystemCommand.SendRtf,
			PrintingSystemCommand.SendTxt,
			PrintingSystemCommand.SendXls,
			PrintingSystemCommand.Watermark
		};
		#endregion
		protected internal virtual void HideUnsupportedCommands(PrintingSystemBase ps) {
			ps.SetCommandVisibility(unsupportedCommands, CommandVisibility.None);
		}
		protected internal override DocumentPrinter CreateDocumentPrinter(DocumentModel documentModel) {
			return new BrickDocumentPrinter(documentModel, UseGdiPlus);
		}
	}
	#endregion
#if !DXPORTABLE
	public class PdfRichEditPrinter : RichEditPrinter {
		PdfCreationOptions creationOptions;
		public PdfRichEditPrinter(DocumentModel documentModel, PdfCreationOptions creationOptions)
			: base(documentModel) {
				this.creationOptions = creationOptions;
		}
		protected internal override DocumentPrinter CreateDocumentPrinter(DocumentModel documentModel) {
			return new PdfBrickDocumentPrinter(documentModel, UseGdiPlus, this.creationOptions);
		}
	}
#endif
	#region RichEditControlPrinter
	public class RichEditControlPrinter : RichEditPrinter {
		readonly InnerRichEditControl control;
		static DocumentModel GetDocumentModel(InnerRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			return control.DocumentModel;
		}
		public RichEditControlPrinter(InnerRichEditControl control)
			: base(control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected internal InnerRichEditControl InnerControl { get { return control; } }
		protected internal override bool UseGdiPlus { get { return InnerControl.Owner.UseGdiPlus; } }
		#endregion
		protected internal override void BeginDocumentRendering() {
			InnerControl.BeginDocumentRendering();
		}
		protected internal override void EndDocumentRendering() {
			InnerControl.EndDocumentRendering();
		}
	}
	#endregion
}
