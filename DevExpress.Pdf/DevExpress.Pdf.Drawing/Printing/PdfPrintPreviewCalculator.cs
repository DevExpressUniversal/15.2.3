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
using System.Drawing.Printing;
namespace DevExpress.Pdf.Drawing {
	public class PdfPrintPreviewCalculator : PdfPrintCalculator {
		const int border = 1;
		int largestEdgeLength;
		Point? pagePosition;
		Rectangle paperRectangle;
		Size previewSize;
		Point[] unprintableArea;
		Rectangle printablePageArea;
		Rectangle printablePaperArea;
		public int LargestEdgeLength {
			get {
				if (largestEdgeLength < 1)
					CalculateAll();
				return largestEdgeLength;
			}
		}
		public Point PagePosition {
			get {
				if (!pagePosition.HasValue)
					CalculateAll();
				return pagePosition ?? Point.Empty;
			}
		}
		public Rectangle PaperRectangle {
			get {
				if (paperRectangle == Rectangle.Empty)
					CalculateAll();
				return paperRectangle;
			}
		}
		bool? isDrawPageRectangle;
		public bool IsDrawPageRectangle {
			get {
				if (!isDrawPageRectangle.HasValue)
					CalculateAll();
				return isDrawPageRectangle.Value;
			}
		}
		public Size PreviewSize {
			get { return previewSize; }
			set {
				if (!previewSize.Equals(value)) {
					previewSize = value;
					Update();
				}
			}
		}
		public Rectangle PrintablePageArea { get { return IsDrawPageRectangle ? printablePageArea : Rectangle.Empty; } }
		public Point[] UnprintableArea {
			get {
				if (unprintableArea == null)
					CalculateAll();
				return unprintableArea;
			}
		}
		public PdfPrintPreviewCalculator(PdfPrinterSettings printerSettings, PdfPage page, Size previewSize, int pageRotationAngle)
			: base(printerSettings, page, pageRotationAngle) {
			this.previewSize = previewSize;
			Update();
		}
		 public override void Update() {
			base.Update();
			largestEdgeLength = 0;
			unprintableArea = null;
			pagePosition = null;
			paperRectangle = Rectangle.Empty;
		}
		protected override void CalculateAll() {
			base.CalculateAll();
			CalculatePreviewPositions();
			CalculateUnprintableArea();
			SizeF pageSize = PageSize;
			Size paperSize = PaperSize;
			float scale = Scale;
			isDrawPageRectangle = Convert.ToInt32(pageSize.Width * scale) > paperSize.Width || Convert.ToInt32(pageSize.Height * scale) > paperSize.Height;
		}
		void CalculateUnprintableArea() {
			Point outerTopLeft = new Point(0, 0);
			Point outerTopRight = new Point(previewSize.Width, 0);
			Point outerBottomRight = new Point(previewSize.Width, previewSize.Height);
			Point outerBottomLeft = new Point(0, previewSize.Height);
			Point paperBorderTopLeft = new Point(printablePaperArea.Left, printablePaperArea.Top);
			Point paperBorderTopRight = new Point(printablePaperArea.Left + printablePaperArea.Width, printablePaperArea.Top);
			Point paperBorderBottomLeft = new Point(printablePaperArea.Left, printablePaperArea.Top + printablePaperArea.Height);
			Point paperBorderBottomRight = new Point(printablePaperArea.Left + printablePaperArea.Width, printablePaperArea.Top + printablePaperArea.Height);
			unprintableArea = new Point[] { outerTopLeft, outerTopRight, outerBottomRight, outerBottomLeft, outerTopLeft, paperBorderTopLeft, paperBorderTopRight, paperBorderBottomRight, paperBorderBottomLeft, paperBorderTopLeft, outerTopLeft };
		}
		int ToInt(float value){
			return Convert.ToInt32(Math.Round(value,MidpointRounding.ToEven)); 
		}
		void CalculatePreviewPositions() {
			Size paperSize = PaperSize;
			int paperWidth = paperSize.Width;
			int paperHeight = paperSize.Height;
			float previewWidth = previewSize.Width - 2 - border * 2;
			float previewHeight = previewSize.Height - 2 - border * 2;
			float scaleFactor = Math.Min(previewWidth / paperWidth, previewHeight / paperHeight);
			paperWidth = ToInt(paperWidth * scaleFactor);
			paperHeight = ToInt(paperHeight * scaleFactor);
			this.paperRectangle = new Rectangle(ToInt((previewWidth - paperWidth) / 2f) + 1 + border, ToInt((previewHeight - paperHeight) / 2f) + 1 + border, paperWidth, paperHeight);
			CalculatePrintablePaperArea(scaleFactor);
			Size currentPageSize = GetCurrentPageSize(scaleFactor);
			Point pagePosition = new Point(ToInt(PageOffset.X * scaleFactor + paperRectangle.X), ToInt(PageOffset.Y * scaleFactor + paperRectangle.Y));
			this.pagePosition = pagePosition;
			printablePageArea = new Rectangle(Math.Max(paperRectangle.X, pagePosition.X) - 1, Math.Max(paperRectangle.Y, pagePosition.Y) - 1, Math.Min(currentPageSize.Width, paperWidth) + 1, Math.Min(currentPageSize.Height, paperHeight) + 1);
			largestEdgeLength = ToInt(currentPageSize.Height > currentPageSize.Width ? currentPageSize.Height : currentPageSize.Width);
		}
		void CalculatePrintablePaperArea(float scaleFactor) {
			PageSettings pageSettings = PageSettings;
			SizeF printableAreaSize = PrintableAreaSize;
			int printableAreaWidth = ToInt(printableAreaSize.Width * scaleFactor);
			int printableAreaHeight = ToInt(printableAreaSize.Height * scaleFactor);
			Size printablePaperAreaSize = pageSettings.Landscape ? new Size(printableAreaHeight, printableAreaWidth) : new Size(printableAreaWidth, printableAreaHeight);
			float marginX = 0;
			float marginY = 0;
			try {
				marginX = pageSettings.HardMarginX;
				marginY = pageSettings.HardMarginY;
			}
			catch { }
			printablePaperArea = new Rectangle(new Point(ToInt(paperRectangle.Left + marginX * scaleFactor), ToInt(paperRectangle.Top + marginY * scaleFactor)), printablePaperAreaSize);
		}
		Size GetCurrentPageSize(float scaleFactor) {
			SizeF pageSize = GetCurrentPageSize();
			return new Size(Convert.ToInt32(pageSize.Width * scaleFactor), Convert.ToInt32(pageSize.Height * scaleFactor));
		}
	}
}
