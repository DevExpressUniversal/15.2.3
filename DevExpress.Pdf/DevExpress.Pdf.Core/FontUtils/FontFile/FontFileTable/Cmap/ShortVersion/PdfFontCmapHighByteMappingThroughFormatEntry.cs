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
	public class PdfFontCmapHighByteMappingThroughFormatEntry : PdfFontCmapShortFormatEntry {
		const int subHeaderKeysLength = 512;
		const int subHeaderLength = 8;
		readonly short[] subHeaderKeys;
		readonly PdfFontCmapHighByteMappingThroughSubHeader[] subHeaders;
		readonly short[] glyphIndexArray;
		public short[] SubHeaderKeys { get { return subHeaderKeys; } }
		public PdfFontCmapHighByteMappingThroughSubHeader[] SubHeaders { get { return subHeaders; } }
		public short [] GlyphIndexArray { get { return glyphIndexArray; } }
		public override int Length { get { return HeaderLength + subHeaderKeysLength + subHeaderLength * subHeaders.Length + glyphIndexArray.Length * 2; } }
		protected override PdfFontCmapFormatID Format { get { return PdfFontCmapFormatID.HighByteMappingThrough; } }
		public PdfFontCmapHighByteMappingThroughFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfBinaryStream stream) : base(platformId, encodingId, stream) {
			subHeaderKeys = stream.ReadShortArray(256);
			HashSet<int> subheaderIndices = new HashSet<int>();
			foreach (int key in subHeaderKeys)
				if (!subheaderIndices.Contains(key))
					subheaderIndices.Add(key);
			int subHeaderCount = subheaderIndices.Count;
			subHeaders = new PdfFontCmapHighByteMappingThroughSubHeader[subHeaderCount];
			int endOfSubheadersPosition = (int)stream.Position + subHeaderCount * 8;
			int offset = subHeaderCount * 8 - 6;
			PdfFontCmapHighByteMappingThroughSubHeader firstSubheader = ReadSubHeader(stream, endOfSubheadersPosition);
			int glyphIndexArrayCount = firstSubheader.CalcGlyphIndexArraySize(offset);
			subHeaders[0] = firstSubheader;
			offset -= 8;
			for (int i = 1; i < subHeaderCount; i++, offset -= 8) {
				PdfFontCmapHighByteMappingThroughSubHeader subHeader = ReadSubHeader(stream, endOfSubheadersPosition);
				subHeaders[i] = subHeader;
				glyphIndexArrayCount = Math.Max(glyphIndexArrayCount, subHeader.CalcGlyphIndexArraySize(offset));
			}
			glyphIndexArray = stream.ReadShortArray(glyphIndexArrayCount);
		}
		PdfFontCmapHighByteMappingThroughSubHeader ReadSubHeader(PdfBinaryStream stream, int endOfSubheadersPosition) {
			short firstCode = stream.ReadShort();
			short entryCount = stream.ReadShort();
			short idDelta = stream.ReadShort();
			int pos = (int)stream.Position;
			short idRangeOffset = stream.ReadShort();
			return new PdfFontCmapHighByteMappingThroughSubHeader(firstCode, entryCount, idDelta, idRangeOffset, (idRangeOffset - (endOfSubheadersPosition - pos)) / 2);
		}
		public override void Write(PdfBinaryStream tableStream) {
			base.Write(tableStream);
			tableStream.WriteShortArray(subHeaderKeys);
			foreach (PdfFontCmapHighByteMappingThroughSubHeader subHeader in subHeaders) {
				tableStream.WriteShort(subHeader.FirstCode);
				tableStream.WriteShort(subHeader.EntryCount);
				tableStream.WriteShort(subHeader.IdDelta);
				tableStream.WriteShort(subHeader.IdRangeOffset);
			}
			tableStream.WriteShortArray(glyphIndexArray);
		}
		public override int MapCode(char character) {
			byte high = (byte)(character >> 8);
			byte low = (byte)(character & 0xff);
			int subheaderIndex = subHeaderKeys[high] / 8;
			if (subheaderIndex == 0) {
				ushort glyph = (ushort)glyphIndexArray[low];
				return subHeaders != null && subHeaders.Length > 0 && glyph != MissingGlyphIndex ? (glyph + subHeaders[0].IdDelta) % 65536 : glyph;
			}
			if (subheaderIndex > subHeaders.Length)
				return MissingGlyphIndex;
			PdfFontCmapHighByteMappingThroughSubHeader subHeader = subHeaders[subheaderIndex];
			short firstCode = subHeader.FirstCode;
			if (firstCode > low || low >= firstCode + subHeader.EntryCount)
				return MissingGlyphIndex;
			int index = subHeader.GlyphOffset + low;
			if (index > glyphIndexArray.Length)
				return MissingGlyphIndex;
			int p = (ushort)glyphIndexArray[index];
			if (p != MissingGlyphIndex)
				p += subHeader.IdDelta;
			return p % 65536;
		}
	}
}
