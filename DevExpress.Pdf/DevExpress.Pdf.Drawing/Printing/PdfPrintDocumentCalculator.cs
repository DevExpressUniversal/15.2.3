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
using DevExpress.XtraPrinting;
namespace DevExpress.Pdf.Drawing {
	public class PdfPrintDocumentCalculator : PdfPrintCalculator {
		int printerResolution;
		float printingDpi;
		Rectangle? rectangle;
		bool isDrawOnGraphics;
		public PdfPrintDocumentCalculator(PdfPrinterSettings printerSettings, PdfPage page, int pageRotationAngle)
			: base(printerSettings, page, pageRotationAngle) {
			rectangle = Rectangle.Empty;
			this.printerResolution = printerSettings.PrintingDpi;
		}
		public Rectangle Rectangle {
			get {
				if (rectangle == null)
					CalculateAll();
				return rectangle ?? Rectangle.Empty;
			}
		}
		public float PrintScale { get { return printingDpi * Scale / (100 * UnitsPerDot); } }
		public bool IsDrawOnGraphics { get { return isDrawOnGraphics; } }
		PointF PrintOffset {
			get {
				PointF offset = PageOffset;
				float k = UnitsPerDot / Scale;
				float x = k * offset.X;
				float y = k * offset.Y;
				x = x < 0 ? x : 0;
				y = y < 0 ? y : 0;
				return new PointF(x, y);
			}
		}
		public IEnumerable<Rectangle> GetRectangles(int divisionCount) {
			int divisionX = (int)Math.Ceiling(Math.Sqrt(divisionCount));
			int divisionY = (int)Math.Ceiling((double)divisionCount / divisionX);
			Rectangle rectangle = Rectangle;
			List<Rectangle> rects = new List<Rectangle>(divisionX * divisionY);
			Size size = new Size(rectangle.Width / divisionX, rectangle.Height / divisionY);
			Point divOffset = rectangle.Location;
			for (int y = 0; y < divisionY; y++, divOffset.Y += size.Height) {
				if (y == divisionY - 1)
					size.Height = rectangle.Bottom - divOffset.Y;
				for (int x = 0; x < divisionX - 1; x++, divOffset.X += size.Width) {
					rects.Add(new Rectangle(divOffset, size));
				}
				rects.Add(new Rectangle(divOffset, new Size(rectangle.Right - divOffset.X, size.Height)));
				divOffset.X = rectangle.Location.X;
			}
			return rects;
		}
		public PointF GetOffset(Rectangle rect) {
			Rectangle rectangle = Rectangle;
			float scale = PrintScale;
			PointF printOffset = PrintOffset;
			PageSettings pageSettings = PageSettings;
			return new PointF(printOffset.X - pageSettings.HardMarginX * UnitsPerDot - (rect.Location.X - rectangle.X) / scale, printOffset.Y - pageSettings.HardMarginY * UnitsPerDot - (rect.Location.Y - rectangle.Y) / scale);
		}
		public void CalculatePrintingDpi(float printerGraphicsDpi) {
			isDrawOnGraphics = printerGraphicsDpi <= printerResolution || printerResolution < 0;
			printingDpi = isDrawOnGraphics ? printerGraphicsDpi : printerResolution;
			Update();
		}
		public PointF GetBitmapPosition(Rectangle rect) {
			float dotsPerHundredthInch = printerResolution / GraphicsDpi.HundredthsOfAnInch;
			PointF printOffset = PrintOffset;
			float xPosition = Convert.ToSingle(printOffset.X + rect.Location.X / dotsPerHundredthInch);
			float yPosition = Convert.ToSingle(printOffset.Y + rect.Location.Y / dotsPerHundredthInch);
			return new PointF(xPosition, yPosition);
		}
		public override void Update() {
			base.Update();
			rectangle = null;
		}
		protected override void CalculateAll() {
			base.CalculateAll();
			SizeF pageSize = PageSize;
			float unitDpi = printingDpi / GraphicsDpi.HundredthsOfAnInch;
			double scaleToDpi = Scale * unitDpi;
			pageSize = new SizeF((float)(pageSize.Width * scaleToDpi), (float)(pageSize.Height * scaleToDpi));
			Size paperSize = PaperSize;
			PointF offset = PageOffset;
			paperSize = new Size(Convert.ToInt32(paperSize.Width * unitDpi), Convert.ToInt32(paperSize.Height * unitDpi));
			int x = Convert.ToInt32(offset.X * unitDpi);
			int y = Convert.ToInt32(offset.Y * unitDpi);
			x = x > 0 ? x : 0;
			y = y > 0 ? y : 0;
			int graphicsWidth = Convert.ToInt32(Math.Min(paperSize.Width, pageSize.Width));
			int graphicsHeight = Convert.ToInt32(Math.Min(paperSize.Height, pageSize.Height));
			rectangle = new Rectangle(x, y, graphicsWidth, graphicsHeight);
		}
	}
}
