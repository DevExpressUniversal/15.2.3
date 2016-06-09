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
using System.Drawing;
using System.Drawing.Printing;
namespace DevExpress.XtraPrinting {
	class PrintDocumentContainer : IDocumentSource {
		System.Drawing.Printing.PrintDocument printDocument;
		PrintingSystemBase printingSystem;
		List<Tuple<PaperSize, bool>> listSettings = new List<Tuple<PaperSize, bool>>();
		#region IDocumentSource Members
		[System.ComponentModel.Browsable(false)]
		public PrintingSystemBase PrintingSystemBase {
			get {
				return printingSystem;
			}
		}
		#endregion
		#region ILink Members
		void ILink.CreateDocument(bool buildPagesInBackground) {
			printingSystem.SetCommandVisibility(unsupportedCommands, CommandVisibility.None);
			PrintController oldPrintController = printDocument.PrintController;
			bool oldOriginAtMargins = printDocument.OriginAtMargins;
			this.printDocument.PrintPage += printDocument_PrintPage;
			try {
				printDocument.OriginAtMargins = false;
				printDocument.PrintController = new PreviewPrintController();
				listSettings.Clear();
				printDocument.Print();
				printDocument.OriginAtMargins = oldOriginAtMargins;
				LinkBase link = new LinkBase(printingSystem) {
					MinMargins = new Margins(0, 0, 0, 0),
					Landscape = printDocument.DefaultPageSettings.Landscape,
					PaperKind = printDocument.DefaultPageSettings.PaperSize.Kind,
					PaperName = printDocument.DefaultPageSettings.PaperSize.PaperName,
					Margins = printDocument.DefaultPageSettings.Margins,
				};
				link.CreateDetailArea += link_CreateDetailArea;
				link.CreateDocument();
			} finally {
				printDocument.PrintController = oldPrintController;
				printDocument.OriginAtMargins = oldOriginAtMargins;
				this.printDocument.PrintPage -= printDocument_PrintPage;
			}
		}
		void ILink.CreateDocument() {
			((ILink)this).CreateDocument(false);
		}
		IPrintingSystem ILink.PrintingSystem {
			get { return PrintingSystemBase; }
		}
		#endregion
		#region Unsupported commands list
		static PrintingSystemCommand[] unsupportedCommands = new PrintingSystemCommand[] {
			PrintingSystemCommand.DocumentMap,
			PrintingSystemCommand.Thumbnails,
			PrintingSystemCommand.ExportCsv,
			PrintingSystemCommand.ExportHtm,
			PrintingSystemCommand.ExportMht,
			PrintingSystemCommand.ExportRtf,
			PrintingSystemCommand.ExportTxt,
			PrintingSystemCommand.ExportXls,
			PrintingSystemCommand.ExportXlsx,
			PrintingSystemCommand.PageMargins,
			PrintingSystemCommand.PageOrientation,
			PrintingSystemCommand.PageSetup,
			PrintingSystemCommand.PaperSize,
			PrintingSystemCommand.SendCsv,
			PrintingSystemCommand.SendMht,
			PrintingSystemCommand.SendRtf,
			PrintingSystemCommand.SendTxt,
			PrintingSystemCommand.SendXls,
			PrintingSystemCommand.SendXlsx,
			PrintingSystemCommand.Find,
			PrintingSystemCommand.EditPageHF,
			PrintingSystemCommand.Parameters,
			PrintingSystemCommand.Customize,
		};
		#endregion
		public PrintDocumentContainer(System.Drawing.Printing.PrintDocument printDocument) {
			this.printDocument = printDocument;
			printingSystem = new PrintingSystemBase();
		}
		void printDocument_PrintPage(object sender, PrintPageEventArgs e) {
			listSettings.Add(new Tuple<PaperSize, bool>(e.PageSettings.PaperSize, e.PageSettings.Landscape));
		}
		void link_CreateDetailArea(object sender, CreateAreaEventArgs e) {
			PreviewPageInfo[] pages = ((PreviewPrintController)printDocument.PrintController).GetPreviewPageInfo();
			for(int index = 0; index < pages.Length; index++) {
				CreateNewDetail();
				Margins margins = printDocument.OriginAtMargins ? printDocument.DefaultPageSettings.Margins : new Margins(0, 0, 0, 0);
				printingSystem.InsertPageBreak(0, margins,
					listSettings[index].Item1.Kind,
					new Size(listSettings[index].Item1.Width, listSettings[index].Item1.Height),
				   listSettings[index].Item2);
				ImageBrick imageBrick = new ImageBrick() {
					Image = pages[index].Image, Sides = BorderSide.None, PrintingSystem = printingSystem,
					InitialRect = new Rectangle(Point.Empty, GraphicsUnitConverter.Convert(pages[index].PhysicalSize, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document))
				};
				SizeF usefulPageSize = new SizeF(pages[index].PhysicalSize.Width - margins.Left - margins.Right, pages[index].PhysicalSize.Height - margins.Top - margins.Bottom);
				PanelBrick panel = new PanelBrick() {
					Sides = BorderSide.None,
					InitialRect = new RectangleF(Point.Empty, GraphicsUnitConverter.Convert(usefulPageSize, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document))
				};
				panel.InnerBrickList.Add(imageBrick);
				RectangleF rect = new RectangleF(Point.Empty, GraphicsUnitConverter.Convert(usefulPageSize, GraphicsDpi.HundredthsOfAnInch, printingSystem.Graph.Dpi));
				printingSystem.Graph.DrawBrick(panel, rect);
			}
		}
		void CreateNewDetail() {
			printingSystem.Graph.Modifier = BrickModifier.None;
			printingSystem.Graph.Modifier = BrickModifier.Detail;
		}
	}
}
