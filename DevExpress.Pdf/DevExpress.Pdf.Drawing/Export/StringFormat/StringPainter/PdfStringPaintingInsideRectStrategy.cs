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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	class PdfStringPaintingInsideRectStrategy : PdfStringPaintingStrategy {
		readonly PdfRectangle rect;
		readonly PdfFontMetrics metrics;
		readonly double fontSize;
		readonly PdfStringAlignment lineAlignment;
		readonly bool clip;
		readonly double layoutWidth;
		readonly double layoutHeight;
		public PdfStringPaintingInsideRectStrategy(PdfRectangle rect, PdfStringFormat format, PdfFontMetrics metrics, double fontSize, PdfCommandConstructor constructor)
			: base(constructor, format.Alignment) {
			this.rect = rect;
			this.lineAlignment = format.LineAlignment;
			this.clip = !format.FormatFlags.HasFlag(PdfStringFormatFlags.NoClip);
			this.fontSize = fontSize;
			this.metrics = metrics;
			double lineSpacing = metrics.GetLineSpacing(fontSize);
			layoutWidth = rect.Width - lineSpacing * (format.TrailingMarginFactor + format.LeadingMarginFactor);
			layoutHeight = rect.Height - lineSpacing * format.LeadingMarginFactor;
		}
		public override void Clip() {
			if (clip)
				Constructor.IntersectClip(rect);
		}
		public override double GetHorizontalPosition(double stringWidth) {
			return rect.Left + GetAlignedPosition(layoutWidth, stringWidth, Alignment);
		}
		public override double GetFirstLineVerticalPosition(int lineCount) {
			double firstLineHeight = metrics.GetAscent(fontSize) + metrics.GetDescent(fontSize);
			return rect.Top - GetAlignedPosition(layoutHeight, firstLineHeight + (lineCount - 1) * metrics.GetLineSpacing(fontSize), lineAlignment);
		}
		double GetAlignedPosition(double layoutLength, double length, PdfStringAlignment alignment) {
			switch (alignment) {
				case PdfStringAlignment.Center:
					return (layoutLength - length) / 2;
				case PdfStringAlignment.Far:
					return layoutLength - length;
				default:
					return 0;
			}
		}
	}
}
