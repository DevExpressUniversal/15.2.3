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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Common {
	public class PdfCharCache : IEnumerable {
		List<char> list = new List<char>();
		List<ushort> glyphIndices = new List<ushort>();
		List<char[]> glyphToUnicode = new List<char[]>();
		ushort[] glyphIndicesArray;
		char[][] glyphToUnicodeArray;
		char[] defaultValue = new char[] { };
		public int Count { get { return list.Count; } }
		public char this[int index] { get { return (char)list[index]; } }
		internal ushort[] GlyphIndices {
			get {
				if(glyphIndicesArray == null)
					glyphIndicesArray = glyphIndices.ToArray();
				return glyphIndicesArray;
			}
		}
		char[][] GlyphToUnicode {
			get {
				if(glyphToUnicodeArray == null)
					glyphToUnicodeArray = glyphToUnicode.ToArray();
				return glyphToUnicodeArray;
			}
		}
		protected virtual bool ShouldExpandCompositeGlyphs { get { return true; } }
		IEnumerator IEnumerable.GetEnumerator() {
			return this.list.GetEnumerator();
		}
		void AddUnique(char ch) {
			if(!this.list.Contains(ch))
				this.list.Add(ch);
		}
		void AddUniqueGlyph(ushort glyph) {
			AddUniqueGlyph(glyph, defaultValue);
		}
		internal bool AddUniqueGlyph(ushort glyph, char[] c) {
			if(!this.glyphIndices.Contains(glyph)) {
				AddGlyph(glyph, c);
				return true;
			}
			return false;
		}
		void AddGlyph(ushort glyph, char[] c) {
			this.glyphIndices.Add(glyph);
			this.glyphToUnicode.Add(c);
			glyphIndicesArray = null;
			glyphToUnicodeArray = null;
		}
		void RegisterString(string string_) {
			foreach(char char_ in string_)
				AddUnique(char_);
		}
		void RegisterGlyphs(TextRun textRun) {
			for(int i = 0; i < textRun.Glyphs.Length; i++)
				AddUniqueGlyph(textRun.Glyphs[i], textRun.CharMap[textRun.Glyphs[i]]);
		}
		public void RegisterTextRun(TextRun textRun) {
			if(textRun.HasGlyphs)
				RegisterGlyphs(textRun);
			else
				RegisterString(textRun.Text);
		}
		void RegisterGlyphs(ushort[] glyphs) {
			foreach(ushort glyph in glyphs)
				AddUniqueGlyph(glyph);
		}
		internal void CalculateGlyphIndeces(TTFFile ttfFile) {
			if(ShouldExpandCompositeGlyphs)
				RegisterGlyphs(ttfFile.CreateGlyphIndices(this));
			Array.Sort(GlyphIndices, GlyphToUnicode);
		}
		internal ICollection<KeyValuePair<ushort, char[]>> GetCharMap() {
			List<KeyValuePair<ushort, char[]>> map = new List<KeyValuePair<ushort, char[]>>();
			for(int i = 0; i < GlyphIndices.Length; i++) {
				if(GlyphIndices[i] == 0 || GlyphToUnicode[i] == defaultValue)
					continue;
				map.Add(new KeyValuePair<ushort, char[]>(GlyphIndices[i], GlyphToUnicode[i]));
			}
			return map;
		}
	}
}
