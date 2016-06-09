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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	class PdfStringPainter {
		readonly PdfCommandConstructor constructor;
		public PdfStringPainter(PdfCommandConstructor constructor) {
			this.constructor = constructor;
		}
		public void DrawLines(IList<PdfStringGlyphRun> lines, PdfEditableFontData fontData, double fontSize, PdfRectangle layoutRect, PdfStringFormat format, bool useKerning) {
			DrawLines(lines, fontData, fontSize, new PdfStringPaintingInsideRectStrategy(layoutRect, format, fontData.Metrics, fontSize, constructor), format, useKerning);
		}
		public void DrawLines(IList<PdfStringGlyphRun> lines, PdfEditableFontData fontData, double fontSize, PdfPoint location, PdfStringFormat format, bool useKerning) {
			DrawLines(lines, fontData, fontSize, new PdfStringPaintingAtPointStrategy(location, format, constructor), format, useKerning);
		}
		protected virtual double GetGlyphRunWidth(PdfStringGlyphRun glyphRun, double emFactor) {
			return glyphRun.Width * emFactor;
		}
		protected virtual void BeginTextDrawing() {
			constructor.SaveGraphicsState();
		}
		protected virtual void EndTextDrawing() {
			constructor.RestoreGraphicsState();
		}
		void DrawLines(IList<PdfStringGlyphRun> lines, PdfEditableFontData fontData, double fontSize, PdfStringPaintingStrategy strategy, PdfStringFormat format, bool useKerning) {
			int count = lines == null ? 0 : lines.Count;
			if (count > 0) {
				double emFactor = fontSize / 1000;
				double lineSpacing = fontData.Metrics.GetLineSpacing(fontSize);
				double leftMargin = format.LeadingMarginFactor * lineSpacing;
				BeginTextDrawing();
				double ascent = fontData.Metrics.GetAscent(fontSize);
				double descent = fontData.Metrics.GetDescent(fontSize);
				double y = strategy.GetFirstLineVerticalPosition(count) - ascent;
				bool shouldDrawLine = fontData.Underline || fontData.Strikeout;
				if (shouldDrawLine)
					constructor.SetLineWidth(fontSize / 14);
				strategy.Clip();
				for (int i = 0; i < count; i++) {
					double width = GetGlyphRunWidth(lines[i], emFactor);
					double x = leftMargin + strategy.GetHorizontalPosition(width);
					double actualY = y - lineSpacing * i;
					if (fontData.ShouldEmulateItalic)
						constructor.DrawObliqueString(lines[i].TextData, new PdfPoint(x, actualY), fontData.PdfFont, fontSize, useKerning ? lines[i].GlyphOffsets : null);
					else
						constructor.DrawString(lines[i].TextData, new PdfPoint(x, actualY), fontData.PdfFont, fontSize, useKerning ? lines[i].GlyphOffsets : null);
					if (shouldDrawLine) {
						if (fontData.Underline) {
							double underlineY = actualY - 0.5 * descent;
							constructor.DrawLine(new PdfPoint(x, underlineY), new PdfPoint(x + width, underlineY));
						}
						if (fontData.Strikeout) {
							double strikeoutY = actualY + ascent * 0.5 - descent;
							constructor.DrawLine(new PdfPoint(x, strikeoutY), new PdfPoint(x + width, strikeoutY));
						}
					}
				}
				EndTextDrawing();
			}
		}
	}
}
