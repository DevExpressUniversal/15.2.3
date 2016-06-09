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
	public class PdfTrueTypeGlyfTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "glyf";
		static int Pad4(int val) {
			int diff = val % 4;
			return diff == 0 ? val : (val + 4 - diff);
		}
		readonly IDictionary<int, PdfGlyphDescription> glyphs = new Dictionary<int, PdfGlyphDescription>();
		IDictionary<int, PdfGlyphDescription> subsetGlyphs;
		int[] glyphOffsets;
		bool shouldWrite;
		public IDictionary<int, PdfGlyphDescription> Glyphs { get { return glyphs; } }
		public PdfTrueTypeGlyfTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
		}
		public void ReadGlyphs(PdfFontFile fontFile) {
			glyphs.Clear();
			PdfTrueTypeLocaTableDirectoryEntry locaEntry = fontFile.Loca;
			if (locaEntry != null) {
				PdfBinaryStream tableStream = TableStream;
				tableStream.Position = 0;
				int streamLength = (int)tableStream.Length;
				glyphOffsets = locaEntry.GlyphOffsets;
				int count = glyphOffsets.Length - 1;
				List<int> offsets = new List<int>(glyphOffsets);
				offsets.Sort();
				Dictionary<int, PdfGlyphDescription> glyphDescriptions = new Dictionary<int, PdfGlyphDescription>();
				int nextOffset = offsets[0];
				if (offsets[0] != glyphOffsets[0])
					shouldWrite = true;
				for (int glyphIndex = 0, nextOffsetIndex = 1; glyphIndex < count; glyphIndex++) {
					int offset = nextOffset;
					int currentOffset = glyphOffsets[nextOffsetIndex];
					nextOffset = Math.Min(offsets[nextOffsetIndex++], streamLength);
					if (nextOffset != currentOffset)
						shouldWrite = true;
					int size = nextOffset - offset;
					if (size >= PdfGlyphDescription.HeaderSize) {
						tableStream.Position = offset;
						PdfGlyphDescription glyph = new PdfGlyphDescription(tableStream, size);
						if (glyph.IsEmpty) 
							shouldWrite = true;
						else
							glyphDescriptions[offset] = glyph;
					} 
					else if (size != 0) {
						nextOffset = offset;
						shouldWrite = true;
					}
				}
				int glyphOffset = glyphOffsets[0];
				for (int index = 0, i = 1; i <= count; index++, i++) {
					int nextGlyphOffset = glyphOffsets[i];
					PdfGlyphDescription glyph;
					if (nextGlyphOffset - glyphOffset != 0 && glyphDescriptions.TryGetValue(glyphOffsets[index], out glyph))
						glyphs.Add(index, glyph);
					glyphOffset = nextGlyphOffset;
				}
				subsetGlyphs = glyphs;
				glyphOffsets = CalculateOffsets(count);
				if (shouldWrite) {
					locaEntry.GlyphOffsets = glyphOffsets;
					PdfFontMaxpTableDirectoryEntry maxp = fontFile.Maxp;
					if (maxp != null)
						maxp.NumGlyphs = (short)count;
				}   
			}
		}
		public void CreateSubset(PdfFontFile fontFile, PdfCharacterCache cache) {
			subsetGlyphs = new SortedDictionary<int, PdfGlyphDescription>();
			foreach (int glyphIndex in cache.Mapping.Keys) {
				PdfGlyphDescription glyph;
				if (glyphs.TryGetValue(glyphIndex, out glyph)) {
					foreach (int index in glyph.AdditionalGlyphIndices)
						if (!subsetGlyphs.ContainsKey(index) && glyphs.ContainsKey(index))
							subsetGlyphs[index] = glyphs[index];
					subsetGlyphs[glyphIndex] = glyph;
				}
			}
			glyphOffsets = CalculateOffsets(glyphOffsets.Length - 1);
			PdfTrueTypeLocaTableDirectoryEntry loca = fontFile.Loca;
			if (loca != null)
				loca.GlyphOffsets = glyphOffsets;
			shouldWrite = true;
		}
		int[] CalculateOffsets(int glyphCount) {
			int[] offsets = new int[glyphCount + 1];
			int offset = 0;
			int offsetIndex = 0;
			foreach (KeyValuePair<int, PdfGlyphDescription> pair in subsetGlyphs) {
				int glyphIndex = pair.Key;
				for (; offsetIndex <= glyphIndex; offsetIndex++)
					offsets[offsetIndex] = offset;
				offset += Pad4(pair.Value.Size);
			}
			for (; offsetIndex <= glyphCount; offsetIndex++)
				offsets[offsetIndex] = offset;
			return offsets;
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream stream = CreateNewStream();
				foreach (KeyValuePair<int, PdfGlyphDescription> glyph in subsetGlyphs) {
					stream.Position = glyphOffsets[glyph.Key];
					glyph.Value.Write(stream);
					int pos = (int)stream.Position;
					int end = Pad4(pos);
					for (int i = pos; i < end; i++)
						stream.WriteByte(0);
				}
			}
		}
	}
}
