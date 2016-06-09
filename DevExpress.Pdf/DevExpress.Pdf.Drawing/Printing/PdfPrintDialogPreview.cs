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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfPrintDialogPreview : PdfDisposableObject {
		readonly PdfDocumentState documentState;
		PdfPrintPreviewCalculator calculator;
		Image previewImage;
		int pageIndex;
		Color previewPrintAreaBorderColor = Color.FromArgb(255, 96, 152, 192);
		public Image PreviewImage {
			get {
				if (previewImage == null && calculator != null) {
					previewImage = new Bitmap(calculator.PreviewSize.Width, calculator.PreviewSize.Height);
					if (pageIndex >= 0)
						using (Graphics g = Graphics.FromImage(previewImage)) {
							g.FillRectangle(new SolidBrush(Color.White), calculator.PaperRectangle);
							try {
								g.DrawImage(PdfViewerCommandInterpreter.GetBitmap(documentState, pageIndex, calculator.LargestEdgeLength, PdfRenderMode.Print), calculator.PagePosition);
							}
							catch { }
							MarkUnprintableArea(g);
						}
				}
				return previewImage;
			}
		}
		public int PageIndex {
			get { return pageIndex; }
			set {
				if (pageIndex != value && value >= -1) {
					pageIndex = value;
					if (pageIndex >= 0)
						calculator.Page = documentState.Document.Pages[pageIndex];
					DisposePreviewImage();
				}
			}
		}
		public PdfPrinterSettings PrinterSettings {
			get { return calculator.PrinterSettings; }
			set {
				calculator.PrinterSettings = value;
				DisposePreviewImage();
			}
		}
		public Size PreviewSize {
			get { return calculator.PreviewSize; }
			set {
				if (calculator.PreviewSize != value) {
					calculator.PreviewSize = value;
					DisposePreviewImage();
				}
			}
		}
		public Color PreviewPrintAreaBorderColor {
			get { return previewPrintAreaBorderColor; }
			set { previewPrintAreaBorderColor = value; }
		}
		internal float Scale { get { return calculator.Scale; } }
		internal bool HasException { get { return calculator.HasException; } }
		public PdfPrintDialogPreview(PdfDocumentState documentState, PdfPrinterSettings settings, int pageIndex, Size previewSize) {
			this.documentState = documentState;
			this.pageIndex = pageIndex;
			calculator = new PdfPrintPreviewCalculator(settings, documentState.Document.Pages[pageIndex], previewSize, documentState.RotationAngle);
		}
		public void DisposePreviewImage() {
			if (previewImage != null) {
				previewImage.Dispose();
				previewImage = null;
				calculator.Update();
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				DisposePreviewImage();
		}
		void MarkUnprintableArea(Graphics g) {
			Point[] area = calculator.UnprintableArea;
			if (area != null && area.Length > 0) {
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(60, Color.Black)))
					g.FillPolygon(brush, area);
			}
			if (calculator.IsDrawPageRectangle) {
				using (Pen pen = new Pen(previewPrintAreaBorderColor))
					g.DrawRectangle(pen, calculator.PrintablePageArea);
				using (Pen pen = new Pen(Color.FromArgb(128, Color.White))) {
					g.DrawRectangle(pen, new Rectangle(calculator.PrintablePageArea.X - 1, calculator.PrintablePageArea.Y - 1, calculator.PrintablePageArea.Width + 2, calculator.PrintablePageArea.Height + 2));
					g.DrawRectangle(pen, new Rectangle(calculator.PrintablePageArea.X + 1, calculator.PrintablePageArea.Y + 1, calculator.PrintablePageArea.Width - 2, calculator.PrintablePageArea.Height - 2));
				}
			}
		}
	}
}
