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
	public class PdfFontCmapSegmentMappingFormatEntry : PdfFontCmapShortFormatEntry {
		struct Row : IComparable<Row> {
			readonly short endCode;
			readonly short startCode;
			readonly short idDelta;
			readonly short idRangeOffset;
			public short EndCode { get { return endCode; } }
			public short StartCode { get { return startCode; } }
			public short IdDelta { get { return idDelta; } }
			public short IdRangeOffset { get { return idRangeOffset; } }
			public Row(short endCode, short startCode, short idDelta, short idRangeOffset) {
				this.endCode = endCode;
				this.startCode = startCode;
				this.idDelta = idDelta;
				this.idRangeOffset = idRangeOffset;
			}
			int IComparable<Row>.CompareTo(Row other) {
				int result = (ushort)endCode - (ushort)other.endCode;
				if (result == 0) {
					result = (ushort)startCode - (ushort)other.startCode;
					if (result == 0)
						result = (ushort)idDelta - (ushort)other.idDelta;
				}
				return result;
			}
		}
		const short finalCode = -1;
		const short finalDelta = 1;
		readonly int segCount;
		readonly short[] endCode;
		readonly short[] startCode;
		readonly short[] idDelta;
		readonly short[] idRangeOffset;
		readonly short[] glyphIdArray;
		readonly int[] segmentOffsets;
		int SegmentsLength { get { return 10 + segCount * 8; } }
		internal List<PdfFontCmapGlyphRange> GlyphRanges {
			get {
				List<PdfFontCmapGlyphRange> glyphRanges = new List<PdfFontCmapGlyphRange>();
				for (int i = 0; i < segCount; i++) {
					short end = endCode[i];
					if (end == -1)
						break;
					glyphRanges.Add(new PdfFontCmapGlyphRange(startCode[i], end));
				}
				return glyphRanges;
			}
		}
		public int SegCount { get { return segCount; } }
		public short[] EndCode { get { return endCode; } }
		public short[] StartCode { get { return startCode; } }
		public short[] IdDelta { get { return idDelta; } }
		public short[] IdRangeOffset { get { return idRangeOffset; } }
		public short[] GlyphIdArray { get { return glyphIdArray; } }
		public override int Length { get { return HeaderLength + SegmentsLength + glyphIdArray.Length * 2; } }
		protected override PdfFontCmapFormatID Format { get { return PdfFontCmapFormatID.SegmentMapping; } }
		public PdfFontCmapSegmentMappingFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfBinaryStream stream) : base(platformId, encodingId, stream) {
			segCount = stream.ReadShort() / 2;
			stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			endCode = ReadSegmentsArray(stream);
			stream.ReadShort();
			startCode = ReadSegmentsArray(stream);
			idDelta = ReadSegmentsArray(stream);
			idRangeOffset = ReadSegmentsArray(stream);
			int glyphIdArrayLength = (BodyLength - SegmentsLength) / 2;
			if (glyphIdArrayLength < 0)
				glyphIdArrayLength = 0;
			glyphIdArray = new short[glyphIdArrayLength];
			for (int i = 0; i < glyphIdArrayLength; i++)
				glyphIdArray[i] = stream.ReadShort();
			segmentOffsets = new int[segCount];
			for (int i = 0, seg = 1; i < segCount; i++, seg++)
				segmentOffsets[i] = (idRangeOffset[i] - (segCount - seg) - 2) / 2;
		}
		public PdfFontCmapSegmentMappingFormatEntry(PdfFontEncodingID encodingID) : base(PdfFontPlatformID.Microsoft, encodingID, 0) {
			segCount = 2;
			endCode = new short[] { 0, finalCode };
			startCode = new short[] { 0, finalCode };
			idDelta = new short[] { 0, finalDelta };
			idRangeOffset = new short[] { 4, 0 };
			glyphIdArray = new short[] { 0 };
		}
		public PdfFontCmapSegmentMappingFormatEntry(PdfFontEncodingID encodingID, PdfFontCmapTrimmedMappingFormatEntry formatEntry) : base(PdfFontPlatformID.Microsoft, encodingID, formatEntry.Language) {
			segCount = 2;
			short firstCode = formatEntry.FirstCode;
			if (encodingID == PdfFontEncodingID.Undefined)
				firstCode = (short)(firstCode + 0xf000);
			endCode = new short[] { (short)(firstCode + formatEntry.EntryCount - 1), finalCode };
			startCode = new short[] { firstCode, finalCode };
			idDelta = new short[] { 0, finalDelta };
			idRangeOffset = new short[] { 4, 0 };
			glyphIdArray = formatEntry.GlyphIdArray;
		}
		public PdfFontCmapSegmentMappingFormatEntry(PdfFontEncodingID encodingID, PdfFontCmapByteEncodingFormatEntry formatEntry) : base(PdfFontPlatformID.Microsoft, encodingID, formatEntry.Language) {
			byte[] glyphArray = formatEntry.GlyphIdArray;
			SortedDictionary<short, short> glyphIndices = new SortedDictionary<short, short>();
			if (encodingID == PdfFontEncodingID.Undefined) 
				for (short i = 0; i < 256; i++) {
					short glyphIndex = glyphArray[i];
					if (glyphIndex != 0)
						glyphIndices.Add((short)(0xf000 + i), glyphIndex);
				}
			else if (formatEntry.PlatformId == PdfFontPlatformID.Macintosh)
				for (short i = 0; i < 256; i++) {
					short glyphIndex = glyphArray[i];
					if (glyphIndex != 0) {
						string glyphName;
						ushort actualGlyphCode;
						if (PdfSimpleFontEncoding.MacRomanEncoding.TryGetValue((byte)i, out glyphName) && PdfUnicodeConverter.GlyphCodes.TryGetValue(glyphName, out actualGlyphCode))
							glyphIndices.Add((short)actualGlyphCode, glyphIndex);
					}
				}
			else 
				for (short i = 0; i < 256; i++) {
					short glyphIndex = glyphArray[i];
					if (glyphIndex != 0)
						glyphIndices.Add(i, glyphIndex);
				}
			int length = glyphIndices.Count;
			segCount = length + 1;
			startCode = new short[segCount];
			endCode = new short[segCount];
			idDelta = new short[segCount];
			idRangeOffset = new short[segCount];
			int index = 0;
			foreach (KeyValuePair<short, short> pair in glyphIndices) {
				short glyphCode = pair.Key;
				startCode[index] = glyphCode;
				endCode[index] = glyphCode;
				idDelta[index++] = (short)(pair.Value - glyphCode);
			}
			startCode[length] = finalCode;
			endCode[length] = finalCode;
			idDelta[length] = finalDelta;
			glyphIdArray = new short[0];
		}
		public PdfFontCmapSegmentMappingFormatEntry(PdfFontEncodingID encodingID, PdfFontCmapSegmentMappingFormatEntry formatEntry) : base(PdfFontPlatformID.Microsoft, encodingID, formatEntry.Language) {
			segCount = formatEntry.segCount;
			endCode = formatEntry.endCode;
			startCode = formatEntry.startCode;
			idDelta = formatEntry.idDelta;
			idRangeOffset = formatEntry.idRangeOffset;
			glyphIdArray = formatEntry.GlyphIdArray;
			if (encodingID == PdfFontEncodingID.Undefined) {
				int count = segCount - 1;
				for (int i = 0; i < count; i++) {
					endCode[i] = (short)(0xf000 + endCode[i]);
					startCode[i] = (short)(0xf000 + startCode[i]);
				}
				count = idDelta.Length;
				for (int i = 0; i < count; i++) {
					short delta = idDelta[i];
					if (delta != 0)
						idDelta[i] = (short)(delta - 0xf000);
				}
			}
		}
		public PdfFontCmapSegmentMappingFormatEntry(ICollection<short> encoding) : base(PdfFontPlatformID.Microsoft, PdfFontEncodingID.UGL, 0) {
			int length = encoding.Count;
			segCount = length + 1;
			startCode = new short[segCount];
			endCode = new short[segCount];
			idDelta = new short[segCount];
			idRangeOffset = new short[segCount];
			SortedDictionary<short, short> counts = new SortedDictionary<short, short>();
			short delta = 1;
			foreach (short count in encoding)
				counts[count] = (short)(delta++ - count);
			int index = 0;
			foreach (KeyValuePair<short, short> pair in counts) {
				short count = pair.Key;
				startCode[index] = count;
				endCode[index] = count;
				idDelta[index++] = pair.Value;
			}
			startCode[length] = finalCode;
			endCode[length] = finalCode;
			idDelta[length] = finalDelta;
			glyphIdArray = new short[0];
		}
		public override int MapCode(char character) {
			int glyphCount = glyphIdArray.Length;
			int idRangeCount = idRangeOffset.Length;
			for (int i = 0; i < segCount; i++) {
				short start = startCode[i];
				if (endCode[i] >= character && start <= character) {
					short offset = idRangeOffset[i];
					if (offset != 0) {
						int position = idRangeOffset[i] / 2 + character - start + i - idRangeCount;
						if (position < glyphCount) {
							ushort glyphIndex = (ushort)glyphIdArray[position];
							return glyphIndex == MissingGlyphIndex ? glyphIndex : (glyphIndex + idDelta[i]) % 65536;
						}
					}
					else
						return character == MissingGlyphIndex ? character : (character + idDelta[i]) % 65536;
				}
			}
			return MissingGlyphIndex;
		}
		public bool Validate() {
			if (segCount <= 0)
				return false;
			int maxIndex = segCount - 1;
			ushort previous = (ushort)endCode[0];
			for (int previousIndex = 0, index = 1; index < maxIndex; previousIndex++, index++) {
				ushort value = (ushort)endCode[index];
				if (value < previous) {
					List<Row> list = new List<Row>(maxIndex);
					for (int i = 0; i < maxIndex; i++)
						list.Add(new Row(endCode[i], startCode[i], idDelta[i], idRangeOffset[i]));
					list.Sort();
					for (int i = 0; i < maxIndex; i++) {
						Row row = list[i];
						endCode[i] = row.EndCode;
						startCode[i] = row.StartCode;
						idDelta[i] = row.IdDelta;
						idRangeOffset[i] = row.IdRangeOffset;
					}
					return true;
				}
				previous = value;
			}
			return false;
		}
		internal IDictionary<string, ushort> GetGlyphMapping(IList<string> glyphNames) {
			if (glyphNames == null)
				return null;
			int glyphNamesCount = glyphNames.Count;
			int glyphIdCount = glyphIdArray.Length;
			Dictionary<string, ushort> glyphCodes = new Dictionary<string, ushort>();
			for (int i = 0, offset = segCount; i < segCount; i++, offset--) {
				short start = startCode[i];
				short end = endCode[i];
				if (start != -1) {
					short rangeOffset = idRangeOffset[i];
					if (rangeOffset > 0) {
						int startIndex = rangeOffset / 2 - start - offset;
						int idIndex = start + startIndex;
						for (short code = start; code <= end && idIndex < glyphIdCount; code++, idIndex++) {
							short glyphId = glyphIdArray[idIndex];
							if (glyphId < 0 || glyphId >= glyphNamesCount) 
								return null;
							glyphCodes[glyphNames[glyphId]] = (ushort)code;
						}
					}
					else {
						int glyphId = start + idDelta[i];
						for (short code = start; code <= end; code++, glyphId++) {
							if (glyphId >= glyphNamesCount)
								return null;
							glyphCodes[glyphNames[glyphId]] = (ushort)code;
						}
					}
				}
			}
			return glyphCodes;
		}
		short[] ReadSegmentsArray(PdfBinaryStream cmapStream) {
			short[] result = new short[segCount];
			for (int i = 0; i < segCount; i++) 
				result[i] = cmapStream.ReadShort();
			return result;
		}
		public override void Write(PdfBinaryStream tableStream) {
			base.Write(tableStream);
			short doubleSegCount = (short)(segCount * 2);
			tableStream.WriteShort(doubleSegCount);
			short searchRange = Convert.ToInt16(2 * Math.Pow(2, Math.Floor(Math.Log(segCount, 2))));
			tableStream.WriteShort(searchRange);
			tableStream.WriteShort(Convert.ToInt16(Math.Log(searchRange / 2, 2)));
			tableStream.WriteShort((short)(doubleSegCount - searchRange));
			tableStream.WriteShortArray(endCode);
			tableStream.WriteShort(0);
			tableStream.WriteShortArray(startCode);
			tableStream.WriteShortArray(idDelta);
			tableStream.WriteShortArray(idRangeOffset);
			tableStream.WriteShortArray(glyphIdArray);
		}
	}
}
