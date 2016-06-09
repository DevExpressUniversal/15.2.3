#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public class PdfFontData {
		public static PdfFontData CreateFontData(PdfFont font) {
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			PdfSimpleFont simpleFont = font as PdfSimpleFont;
			if (simpleFont != null) {
				double missingWidth;
				if (fontDescriptor == null)
					missingWidth = 0;
				else {
					missingWidth = fontDescriptor.MissingWidth;
					if (missingWidth == 0)
						missingWidth = fontDescriptor.AvgWidth;
				}
				return new PdfFontData(font, simpleFont.FirstChar, simpleFont.Widths, missingWidth);
			}
			PdfType0Font type0Font = font as PdfType0Font;
			if (type0Font == null)
				throw new InvalidOperationException();
			IDictionary<int, double> widthsDictionary = type0Font.Widths;
			int maxKey = 0;
			foreach (int key in widthsDictionary.Keys)
				maxKey = Math.Max(key, maxKey);
			double[] widths = new double[maxKey + 1];
			foreach (KeyValuePair<int, double> pair in widthsDictionary)
				widths[pair.Key] = pair.Value;
			double defaultWidth = type0Font.DefaultWidth;
			if (defaultWidth == 0 && fontDescriptor != null)
				defaultWidth = fontDescriptor.AvgWidth;
			return new PdfFontData(font, 0, widths, defaultWidth);
		}
		readonly PdfFont font;
		readonly int firstChar;
		readonly double[] widths;
		readonly double missingWidth;
		readonly double bboxHeight;
		readonly double height;
		readonly double ascent;
		readonly double descent;
		readonly PdfCharacterMapping toUnicode;
		public PdfFont Font { get { return font; } }
		public int FirstChar { get { return firstChar; } }
		public double[] Widths { get { return widths; } }
		public double MissingWidth { get { return missingWidth; } }
		public double BBoxHeight { get { return bboxHeight; } }
		public double Height { get { return height; } }
		public double Ascent { get { return ascent; } }
		public double Descent { get { return descent; } }
		public PdfCharacterMapping ToUnicode { get { return toUnicode; } }
		PdfFontData(PdfFont font, int firstChar, double[] widths, double missingWidth) {
			this.font = font;
			this.firstChar = firstChar;
			this.widths = widths;
			this.missingWidth = Math.Max(missingWidth, 1);
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor != null) {
				ascent = fontDescriptor.ActualAscent;
				descent = fontDescriptor.ActualDescent;
				height = ascent - descent;
				PdfRectangle fontBBox = fontDescriptor.FontBBox;
				if (fontBBox != null)
					bboxHeight = fontBBox.Height;
			}
			height = Math.Max(height, 1);
			toUnicode = font.ToUnicode;
		}
	}
}
