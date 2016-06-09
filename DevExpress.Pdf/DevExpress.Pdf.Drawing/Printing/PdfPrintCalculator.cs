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
	public abstract class PdfPrintCalculator {
		protected const float UnitsPerDot = 0.720f;
		PdfPrinterSettings printerSettings;
		PdfPage page;
		PageSettings pageSettings;
		float scale = -1;
		Size paperSize = Size.Empty;
		SizeF pageSize = Size.Empty;
		PointF? pageOffset;
		int pageRotationAngle;
		bool hasException = false;
		public PageSettings PageSettings {
			get { return pageSettings; }
			set {
				if (!pageSettings.Equals(value)) {
					pageSettings = value;
					CalculatePageLandscape();
				}
				Update();
			}
		}
		public PdfPrinterSettings PrinterSettings {
			get { return printerSettings; }
			set {
				if (!printerSettings.Equals(value)) {
					printerSettings = value;
					pageSettings = printerSettings.Settings.DefaultPageSettings;
					Update();
				}
			}
		}
		public PdfPage Page {
			get { return page; }
			set {
				if (page != value) {
					page = value;
					Update();
					CalculatePageLandscape();
				}
			}
		}
		public Size PaperSize {
			get {
				if (paperSize == Size.Empty)
					CalculateAll();
				return paperSize;
			}
		}
		public SizeF PageSize {
			get {
				if (pageSize == SizeF.Empty)
					CalculateAll();
				return pageSize;
			}
		}
		public float Scale {
			get {
				if (scale < 0)
					CalculateAll();
				return scale;
			}
		}
		public PointF PageOffset {
			get {
				if (pageOffset == null)
					CalculateAll();
				return pageOffset.Value;
			}
		}
		public int RotateAngel { get { return pageSettings.Landscape ? printerSettings.Settings.LandscapeAngle : 0; } }
		internal bool HasException { get { return hasException; } }
		protected SizeF PrintableAreaSize {
			get {
				try {
					RectangleF printableArea = pageSettings.PrintableArea;
					hasException = false;
					return printableArea.Size;
				}
				catch {
					hasException = true;
					return new SizeF(850, 1100);
				}
			}
		}
		PaperSize PrintPaperSize {
			get {
				try {
					PaperSize printableArea = pageSettings.PaperSize;
					hasException = false;
					return printableArea;
				}
				catch {
					hasException = true;
					return new PaperSize(String.Empty, 850, 1100);
				}
			}
		}
		protected PdfPrintCalculator(PdfPrinterSettings printerSettings, PdfPage page, int pageRotationAngle) {
			if (printerSettings == null || page == null)
				throw new ArgumentException();
			this.printerSettings = printerSettings;
			this.pageSettings = printerSettings.Settings.DefaultPageSettings;
			this.page = page;
			this.pageRotationAngle = pageRotationAngle;
			CalculatePageLandscape();
		}
		protected SizeF GetCurrentPageSize() {
			SizeF pageSize = PageSize;
			float scale = Scale;
			return new SizeF(pageSize.Width * scale, pageSize.Height * scale);
		}
		public virtual void Update() {
			scale = -1;
			paperSize = Size.Empty;
			pageOffset = null;
		}
		protected virtual void CalculateAll() {
			CalculatePageLandscape();
			PaperSize printPaperSize = PrintPaperSize;
			int paperWidth = Convert.ToInt32(printPaperSize.Width);
			int paperHeight = Convert.ToInt32(printPaperSize.Height);
			this.paperSize = pageSettings.Landscape ? new Size(paperHeight, paperWidth) : new Size(paperWidth, paperHeight);
			PdfPoint size = page.GetSize(pageRotationAngle);
			pageSize = new SizeF((float)size.X / UnitsPerDot, (float)size.Y / UnitsPerDot);
			scale = CalculateScale();
			CalculatePageOffset();
		}
		void CalculatePageOffset() {
			Size paperSize = PaperSize;
			SizeF pageSize = GetCurrentPageSize();
			if (printerSettings.PageOrientation == PdfPrintPageOrientation.Auto || printerSettings.ScaleMode == PdfPrintScaleMode.Fit)
				pageOffset = new PointF((paperSize.Width - pageSize.Width) / 2, (paperSize.Height - pageSize.Height) / 2);
			else 
				pageOffset = new PointF(0, 0);
		}
		void CalculatePageLandscape() {
			if (printerSettings != null && printerSettings.PageOrientation != PdfPrintPageOrientation.Auto)
				pageSettings.Landscape = printerSettings.PageOrientation == PdfPrintPageOrientation.Landscape;
			else {
				SizeF printableAreaSize = PrintableAreaSize;
				float settingsAspectRatio = printableAreaSize.Height / printableAreaSize.Width;
				PdfPoint pageSize = page.GetSize(pageRotationAngle);
				double pageAspectRatio = pageSize.Y / pageSize.X;
				pageSettings.Landscape = ((settingsAspectRatio < 1 && pageAspectRatio > 1) || (settingsAspectRatio > 1 && pageAspectRatio < 1));
			}
		}
		float CalculateScale() {
			PdfPrintScaleMode scaleMode = printerSettings.ScaleMode;
			float scale = printerSettings.Scale / 100f;
			switch (scaleMode) {
				case PdfPrintScaleMode.Fit:
					SizeF printableAreaSize = PrintableAreaSize;
					if (pageSettings.Landscape)
						printableAreaSize = new SizeF(printableAreaSize.Height, printableAreaSize.Width); 
					PdfPoint size = page.GetSize(pageRotationAngle);
					PointF pageSize = new PointF((float)size.X / UnitsPerDot, (float)size.Y / UnitsPerDot);
					scale = Math.Min(printableAreaSize.Width / pageSize.X, printableAreaSize.Height / pageSize.Y);
					break;
				case PdfPrintScaleMode.ActualSize:
					scale = 1.0f;
					break;
			}
			return scale;
		}
	}
}
