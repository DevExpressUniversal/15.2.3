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
	public class PdfNotEmbeddedGlyphMapper : PdfDisposableObject, IPdfGlyphMapper {
		readonly PdfFontFile fontFile;
		readonly PdfFontCmapTableDirectoryEntry cmap;
		readonly PdfFontKernTableDirectoryEntry kern;
		readonly double factor;
		public PdfNotEmbeddedGlyphMapper(PdfFontFile fontFile) {
			this.fontFile = fontFile;
			this.cmap = fontFile == null ? null : fontFile.CMap;
			this.kern = fontFile == null ? null : fontFile.Kern;
			this.factor = 1000.0 / (fontFile.Head == null ? 2048 : fontFile.Head.UnitsPerEm);
		}
		public int MapCharacter(char ch) {
			return cmap == null ? ch : cmap.MapCode(ch);
		}
		public PdfGlyphMappingResult MapString(string s, bool useKerning) {
			int length = s.Length;
			IList<PdfStringGlyph> glyphs = new List<PdfStringGlyph>(length);
			int[] glyphIndices = cmap == null ? null : cmap.MapCodes(s);
			IDictionary<int, string> result = new Dictionary<int, string>(length);
			for (int i = 0; i < length; i++) {
				char ch = s[i];
				int val = cmap == null ? ch : cmap.MapCode(ch);
				double glyphOffset = 0;
				if (useKerning && kern != null && i > 0)
					glyphOffset = -kern.GetKerning(glyphIndices[i - 1], glyphIndices[i]) * factor;
				if (!result.ContainsKey(ch))
					result.Add(ch, ch.ToString());
				glyphs.Add(new PdfStringGlyph(ch, fontFile.GetCharacterWidth(val), glyphOffset));
			}
			return new PdfGlyphMappingResult(new PdfNotEmbeddedStringGlyphRun(glyphs), result);
		}
		protected override void Dispose(bool disposing) {
		}
	}
}
