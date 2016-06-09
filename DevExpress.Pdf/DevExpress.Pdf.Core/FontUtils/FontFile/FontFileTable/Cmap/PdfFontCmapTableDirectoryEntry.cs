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
	public class PdfFontCmapTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "cmap";
		readonly short version;
		readonly List<PdfFontCmapFormatEntry> cMapTables = new List<PdfFontCmapFormatEntry>();
		readonly Dictionary<int, int> mappedGlyphsCache = new Dictionary<int, int>();
		bool shouldWrite;
		public List<PdfFontCmapFormatEntry> CMapTables { get { return cMapTables; } }
		public bool ShouldWrite { get { return shouldWrite; } }
		public PdfFontCmapTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			PdfBinaryStream tableStream = TableStream;
			version = tableStream.ReadShort();
			short numberOfEncodingTables = tableStream.ReadShort();
			cMapTables = new List<PdfFontCmapFormatEntry>(numberOfEncodingTables);
			for (int i = 0; i < numberOfEncodingTables; i++) {
				PdfFontPlatformID platformId = (PdfFontPlatformID)tableStream.ReadShort();
				PdfFontEncodingID encodingId = (PdfFontEncodingID)tableStream.ReadShort();
				int offset = tableStream.ReadInt();
				long position = tableStream.Position;
				tableStream.Position = offset;
				cMapTables.Add(PdfFontCmapFormatEntry.CreateEntry(platformId, encodingId, (PdfFontCmapFormatID)tableStream.ReadShort(), tableStream));
				tableStream.Position = position;
			}
		}
		public PdfFontCmapTableDirectoryEntry(IFont font) : base(EntryTag) {
			cMapTables.Add(new PdfFontCmapSegmentMappingFormatEntry(font.FontFileEncoding));
			shouldWrite = true;
		}
		public PdfFontCmapTableDirectoryEntry(PdfFontCmapSegmentMappingFormatEntry segmentMappingFormatEntry) : base(EntryTag) {
			cMapTables.Add(segmentMappingFormatEntry);
			shouldWrite = true;
		}
		public PdfFontCmapSegmentMappingFormatEntry Validate(bool skipEncodingValidation, bool isSymbolic) {
			PdfFontEncodingID encodingID = isSymbolic ? PdfFontEncodingID.Undefined : PdfFontEncodingID.UGL;
			PdfFontCmapSegmentMappingFormatEntry segmentMappingFormatEntry;
			PdfFontCmapTrimmedMappingFormatEntry actualTrimmedMappingFormatEntry = null;
			PdfFontCmapByteEncodingFormatEntry actualByteEncodingFormatEntry = null;
			PdfFontCmapSegmentMappingFormatEntry uncompatibleSegmentMappingFormatEntry = null;
			foreach (PdfFontCmapFormatEntry formatEntry in cMapTables) {
				segmentMappingFormatEntry = formatEntry as PdfFontCmapSegmentMappingFormatEntry;
				if (segmentMappingFormatEntry != null && (skipEncodingValidation || segmentMappingFormatEntry.EncodingId == encodingID))
					if (segmentMappingFormatEntry.PlatformId == PdfFontPlatformID.Microsoft) {
						shouldWrite = segmentMappingFormatEntry.Validate();
						return segmentMappingFormatEntry;
					}
					else
						uncompatibleSegmentMappingFormatEntry = segmentMappingFormatEntry;
				PdfFontCmapTrimmedMappingFormatEntry trimmedMappingFormatEntry = formatEntry as PdfFontCmapTrimmedMappingFormatEntry;
				if (trimmedMappingFormatEntry != null)
					actualTrimmedMappingFormatEntry = trimmedMappingFormatEntry;
				PdfFontCmapByteEncodingFormatEntry byteEncodingFormatEntry = formatEntry as PdfFontCmapByteEncodingFormatEntry;
				if (byteEncodingFormatEntry != null)
					actualByteEncodingFormatEntry = byteEncodingFormatEntry;
			}
			if (actualTrimmedMappingFormatEntry != null)
				segmentMappingFormatEntry = new PdfFontCmapSegmentMappingFormatEntry(encodingID, actualTrimmedMappingFormatEntry);
			else if (actualByteEncodingFormatEntry != null)
				segmentMappingFormatEntry = new PdfFontCmapSegmentMappingFormatEntry(encodingID, actualByteEncodingFormatEntry);
			else if (uncompatibleSegmentMappingFormatEntry != null)
				segmentMappingFormatEntry = new PdfFontCmapSegmentMappingFormatEntry(encodingID, uncompatibleSegmentMappingFormatEntry);
			else
				return null;
			cMapTables.Clear();
			cMapTables.Add(segmentMappingFormatEntry);
			shouldWrite = true;
			return segmentMappingFormatEntry;
		}
		public int[] MapCodes(string str) {
			if (string.IsNullOrEmpty(str))
				return new int[0];
			int count = str.Length;
			int[] codes = new int[count];
			for (int i = 0; i < count; i++)
				codes[i] = MapCode(str[i]);
			return codes;
		}
		public int MapCode(char character) {
			int glyph = 0;
			if (mappedGlyphsCache.TryGetValue(character, out glyph))
				return glyph;
			foreach (PdfFontCmapFormatEntry cmap in cMapTables) {
				glyph = cmap.MapCode(character);
				if (glyph != PdfFontCmapFormatEntry.MissingGlyphIndex)
					break;
			}
			mappedGlyphsCache.Add(character, glyph);
			return glyph;
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream tableStream = CreateNewStream();
				tableStream.WriteShort(version);
				short cmapTableCount = (short)cMapTables.Count;
				tableStream.WriteShort(cmapTableCount);
				int offset = 4 + cmapTableCount * 8;
				foreach (PdfFontCmapFormatEntry cmapRecord in cMapTables) {
					tableStream.WriteShort((short)cmapRecord.PlatformId);
					tableStream.WriteShort((short)cmapRecord.EncodingId);
					tableStream.WriteInt(offset);
					offset += cmapRecord.Length;
				}
				foreach (PdfFontCmapFormatEntry cmapRecord in cMapTables) 
					cmapRecord.Write(tableStream);
			}
		}
	}
}
