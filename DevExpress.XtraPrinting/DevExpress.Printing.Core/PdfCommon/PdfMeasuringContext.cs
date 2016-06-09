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
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Pdf.Common {
	public static class MeasuringHelper {
		static StringFormat stringFormat;
		static MeasuringHelper() {
			stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;
		}
		public static float MeasureCharWidth(char ch, Font font) {
			return Measurement.Measurer.MeasureString(new string(new char[] { ch }), font, int.MaxValue, stringFormat, GraphicsUnit.Point).Width;
		}
	}
	public class PdfMeasuringContext {
		float currentCharSpacing;
		float currentWordSpacing;
		PdfFont currentFont;
		Font actualFont;
		int currentHorizontalScaling = 100;
		protected Font ActualFont {
			get { return actualFont; }
		}
		protected PdfFont CurrentFont {
			get { return currentFont; }
		}
		protected float CurrentCharSpacing {
			get { return currentCharSpacing; }
			set { currentCharSpacing = value; }
		}
		protected float CurrentWordSpacing {
			get { return currentWordSpacing; }
			set { currentWordSpacing = value; }
		}
		internal PdfMeasuringContext() {
		}
		internal void SetFont(TTFFile ttfFile, Font font) {
			SetFont((ttfFile == null ? null : new PdfFont(font, ttfFile, true)), font);
		}
		protected void SetFont(PdfFont currentFont, Font font) {
			this.currentFont = currentFont;
			this.actualFont = font;
		}
		public virtual void SetHorizontalScaling(int scale) {
			if(scale < 0)
				return;
			this.currentHorizontalScaling = scale;
		}
		public virtual void SetCharSpacing(float charSpace) {
			this.currentCharSpacing = charSpace;
		}
		public virtual void SetWordSpacing(float wordSpace) {
			this.currentWordSpacing = wordSpace;
		}
		public float GetTextWidth(TextRun run) {
			return run.HasGlyphs
				? GetTextWidth(run.Glyphs)
				: GetTextWidth(run.Text);
		}
		public float GetTextWidth(string text) {
			if(currentFont == null) return 0;
			float result = 0;
			for(int i = 0; i < text.Length; i++) {
				float charWidth = GetCharWidth(text[i]);
				if(currentHorizontalScaling != 100)
					charWidth *= (float)currentHorizontalScaling / 100;
				if(charWidth > 0)
					charWidth += currentCharSpacing;
				else
					charWidth = 0;
				if(text[i] == ' ' && i < text.Length - 1)
					charWidth += currentWordSpacing;
				result += charWidth;
			}
			return result;
		}
		internal float GetTextWidth(ushort[] glyphs) {
			if(currentFont == null) return 0;
			ushort space = currentFont.GetGlyphByChar(' ');
			float result = 0;
			for(int i = 0; i < glyphs.Length; i++) {
				float charWidth = GetCharWidth(glyphs[i]);
				if(currentHorizontalScaling != 100)
					charWidth *= (float)currentHorizontalScaling / 100;
				if(charWidth > 0)
					charWidth += currentCharSpacing;
				else
					charWidth = 0;
				if(glyphs[i] == space && i < glyphs.Length - 1)
					charWidth += currentWordSpacing;
				result += charWidth;
			}
			return result;
		}
		protected virtual float GetCharWidth(char ch) {
			return GetCharWidth(currentFont.GetGlyphByChar(ch));
		}
		float GetCharWidth(ushort glyph) {
			return currentFont.GetCharWidth(glyph) / 1000f * actualFont.Size;
		}
	}
}
