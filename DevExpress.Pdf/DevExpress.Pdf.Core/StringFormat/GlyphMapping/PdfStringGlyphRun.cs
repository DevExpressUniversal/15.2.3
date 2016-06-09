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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfStringGlyphRun {
		readonly IList<PdfStringGlyph> glyphs;
		double width;
		public double Width { get { return width; } }
		public IList<PdfStringGlyph> Glyphs { get { return glyphs; } }
		public virtual byte[] TextData {
			get {
				int count = glyphs.Count;
				if (count < 1)
					return new byte[0];
				int byteCount = ByteCountPerGlyph;
				byte[] result = new byte[count * byteCount];
				for (int i = 0, pos = 0; i < count; i++) {
					int gi = glyphs[i].GlyphIndex;
					for (int b = byteCount - 1; b >= 0; b--)
						result[pos++] = (byte)((gi >> 8 * b) & 0xFF);
				} 
				return result;
			}
		}
		public double[] GlyphOffsets { 
			get {
				int count = glyphs.Count;
				if (count < 1)
					return new double[0];
				int byteCount = ByteCountPerGlyph;
				double[] glyphOffsets = new double[(count + 1) * byteCount];
				for (int i = 0, k = 0; i < count; i++, k += byteCount)
					glyphOffsets[k] = glyphs[i].GlyphOffset;
				return glyphOffsets;	
			} 
		}
		public bool IsEmpty { get { return glyphs == null || glyphs.Count == 0; } }
		protected virtual int ByteCountPerGlyph { get { return 2; } }
		public PdfStringGlyphRun(IList<PdfStringGlyph> glyphs) {
			this.glyphs = glyphs;
			foreach (PdfStringGlyph glyph in glyphs)
				width += glyph.Width - glyph.GlyphOffset;
		}
		public PdfStringGlyphRun() {
			this.glyphs = new List<PdfStringGlyph>();
			width = 0;
		}
		public void Append(PdfStringGlyph glyph) {
			glyphs.Add(glyph);
			width += glyph.Width - glyph.GlyphOffset;
		}
		public void Append(PdfStringGlyphRun glyphRun) {
			IList<PdfStringGlyph> newGlyphs = glyphRun.glyphs;
			int count = newGlyphs.Count;
			for (int i = 0; i < count; i++)
				Append(newGlyphs[i]);
		}
		public void RemoveLast() {
			if (glyphs.Count > 0) {
				int lastPos = glyphs.Count - 1;
				PdfStringGlyph glyph = glyphs[lastPos];
				width -= glyph.Width - glyph.GlyphOffset;
				glyphs.RemoveAt(lastPos);
			}
		}
		public IList<PdfStringGlyphRun> GetWords(int spaceGlyphIndex, bool measureTrailingSpaces) {
			bool wordStarted = false;
			PdfStringGlyphRun wordRun = new PdfStringGlyphRun();
			IList<PdfStringGlyphRun> words = new List<PdfStringGlyphRun>();
			foreach (PdfStringGlyph glyph in glyphs) {
				if (glyph.GlyphIndex == spaceGlyphIndex) {
					if (wordStarted) {
						words.Add(wordRun);
						wordRun = new PdfStringGlyphRun();
						wordStarted = false;
					}
				}
				else
					wordStarted = true;
				wordRun.Append(glyph);
			}
			if (wordStarted || ((!wordStarted && wordRun.glyphs.Count != 0) && measureTrailingSpaces))
				words.Add(wordRun);
			return words;
		}
	}
}
