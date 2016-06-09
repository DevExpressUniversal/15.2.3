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
	public interface IPdfGlyphMapper {
		int MapCharacter(char ch);
		PdfGlyphMappingResult MapString(string s, bool useKerning);
	}
	public class PdfGlyphMapper : PdfDisposableObject, IPdfGlyphMapper {
		readonly PdfFontFile fontFile;
		public PdfGlyphMapper(PdfFontFile fontFile) {
			this.fontFile = fontFile;
		}
		public int MapCharacter(char ch) {
			PdfFontCmapTableDirectoryEntry cmap = fontFile.CMap;
			return cmap == null ? ch : cmap.MapCode(ch);
		}
		public virtual PdfGlyphMappingResult MapString(string s, bool useKerning) {
			PdfFontCmapTableDirectoryEntry cmap = fontFile.CMap;
			int[] glyphIndices = cmap == null ? null : fontFile.CMap.MapCodes(s);
			int length = s.Length;
			IDictionary<int, string> result = new Dictionary<int, string>(length);
			IList<PdfStringGlyph> glyphs = new List<PdfStringGlyph>(length);
			byte[] codes = new byte[length * 2];
			double factor = 1000.0 / (fontFile.Head == null ? 2048 : fontFile.Head.UnitsPerEm);
			PdfFontKernTableDirectoryEntry kern = fontFile.Kern;
			for (int i = 0; i < length; i++) {
				char ch = s[i];
				int val = cmap == null ? ch : glyphIndices[i];
				double glyphOffset = 0;
				if (useKerning && kern != null && i > 0)
					glyphOffset = -kern.GetKerning(glyphIndices[i - 1], glyphIndices[i]) * factor;
				if (!result.ContainsKey(val))
					result.Add(val, ch.ToString());
				glyphs.Add(new PdfStringGlyph(val, fontFile.GetCharacterWidth(val), glyphOffset));
			}
			return new PdfGlyphMappingResult(new PdfStringGlyphRun(glyphs), result);
		}
		protected override void Dispose(bool disposing) {
		}
	}
}
