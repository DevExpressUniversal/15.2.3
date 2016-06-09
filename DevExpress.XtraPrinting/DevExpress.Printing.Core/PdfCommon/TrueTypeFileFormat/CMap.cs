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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Pdf.Common {
	class TTFCMap : TTFTable {
		#region inner classes
		struct Subheader {
			int glyphOffset;
			ushort firstCode;
			ushort entryCount;
			short idDelta;
			public Subheader(ushort firstCode, ushort entryCount, short idDelta, int glyphOffset ) {
				this.firstCode = firstCode;
				this.entryCount = entryCount;
				this.idDelta = idDelta;
				this.glyphOffset = glyphOffset;
			}
			public ushort FirstCode { get { return firstCode; } }
			public ushort EntryCount { get { return entryCount; } }
			public short IdDelta { get { return idDelta; } }
			public int GlyphOffset { get { return glyphOffset; } set { glyphOffset = value; } }
		}
		#endregion
		const string unicodeMissingError = "The unicode CMap doesn't exist in the font file";
		const string tableFormatError = "Invalid CMap format";
		ushort versionNumber;
		ushort unicodeVersion;
		int segCount;
		ushort[] ends;
		ushort[] starts;
		ushort[] deltas;
		ushort[] rangeOffsets;
		ushort[] map = new ushort[65536];
		ushort eID;
		ushort format;
		int startPosition;
		System.Text.Encoding fontEncoding;
		protected internal override string Tag { get { return "cmap"; } }
		public int Count { get { return map.Length; } }
		public ushort this[ushort code] {
			get {
				ushort actualCode = 0;
				if(format == 4) {
					if(eID == 0)
						actualCode = map[code] == 0 ? (ushort)(code + 0xF000) : code;
					else
						actualCode = code;
				}
				if(format == 2) {
					if(fontEncoding != null) {
						byte[] bytes = Encoding.Convert(Encoding.Unicode, fontEncoding, Encoding.Unicode.GetBytes(new char[] { (char)code }));
						actualCode = bytes.Length > 1 ? (ushort)(bytes[0] << 8 | bytes[1]) : bytes[0];
					}
				}
				return map[actualCode];
			}
		}
		public override int Length {
			get {
				int unicodeLength = TTFStream.SizeOf_UShort * 7;
				unicodeLength += TTFStream.SizeOf_UShort * segCount;
				unicodeLength += TTFStream.SizeOf_UShort;
				unicodeLength += TTFStream.SizeOf_UShort * segCount * 3;
				for(int i = 0; i < segCount; i++)
					if(rangeOffsets[i] != 0)
						unicodeLength += (ends[i] - starts[i] + 1) * 2;
				return
					TTFStream.SizeOf_UShort * 4 +
					TTFStream.SizeOf_ULong +
					unicodeLength;
			}
		}
		public TTFCMap(TTFFile ttfFile)
			: base(ttfFile) {
		}
		void ReadUnicodeTable(TTFStream ttfStream) {
			startPosition = ttfStream.Position;
			format = ttfStream.ReadUShort();
			switch(format) {
				case 2: ReadUnicodeTable2(ttfStream); break;
				case 4: ReadUnicodeTable4(ttfStream); break;
				default: new TTFFileException(tableFormatError); break;
			}
		}
		void ReadUnicodeTable2(TTFStream ttfStream) {
			ushort length = ttfStream.ReadUShort();
			unicodeVersion = ttfStream.ReadUShort();
			ushort[] subHeaderKeys = new ushort[256];
			List<ushort> subHeaderIndexes = new List<ushort>();
			for(int i = 0; i < subHeaderKeys.Length; i++) {
				subHeaderKeys[i] = (ushort)(ttfStream.ReadUShort() / 8);
				if(!subHeaderIndexes.Contains(subHeaderKeys[i]))
					subHeaderIndexes.Add(subHeaderKeys[i]);
			}
			Subheader[] subHeaders = new Subheader[subHeaderIndexes.Count];
			int endOfSubheadersPosition = ttfStream.Position + subHeaderIndexes.Count * 8;
			for(int i = 0; i < subHeaderIndexes.Count; i++) {
				ushort firstCode = ttfStream.ReadUShort();
				ushort entryCount = ttfStream.ReadUShort();
				short idDelta = ttfStream.ReadShort();
				int pos = ttfStream.Position;
				ushort rangeOffset = ttfStream.ReadUShort();
				subHeaders[i] = new Subheader(firstCode, entryCount, idDelta, (rangeOffset - (endOfSubheadersPosition - pos)) / 2);
			}
			int len = length - (ttfStream.Position - startPosition);
			ushort[] glyphIndexArray = new ushort[len / 2];
			for(int i = 0; i < glyphIndexArray.Length; i++) {
				glyphIndexArray[i] = ttfStream.ReadUShort();
			}
			fontEncoding = GetEncoding();
			CreateFormat2Map(subHeaderKeys, subHeaders, glyphIndexArray);
		}
		Encoding GetEncoding() {
			try {
				return DXEncoding.GetEncoding((int)Owner.FontCodePage);
			} catch {
				return null;
			}
		}
		void CreateFormat2Map(ushort[] subHeaderKeys, Subheader[] subHeaders, ushort[] glyphIndexArray) {
			for(int i = 0; i < 0x100; i++) {
				if((subHeaderKeys[i] > 0) || (i == 0)) {
					int subHeaderKeysIndex = subHeaderKeys[i];
					for(int j = 0; j < subHeaders[subHeaderKeysIndex].EntryCount; j++) {
						ushort glifIndex = glyphIndexArray[subHeaders[subHeaderKeysIndex].GlyphOffset + j];
						if(glifIndex != 0) {
							glifIndex += (ushort)subHeaders[subHeaderKeysIndex].IdDelta;
							map[(i << 8) | (subHeaders[subHeaderKeysIndex].FirstCode + j)] = glifIndex;
						}
					}
				}
			}
		}
		void ReadUnicodeTable4(TTFStream ttfStream) {
			ushort length = ttfStream.ReadUShort();
			unicodeVersion = ttfStream.ReadUShort();
			segCount = Convert.ToInt32(ttfStream.ReadUShort() / 2);
			ends = new ushort[segCount];
			starts = new ushort[segCount];
			deltas = new ushort[segCount];
			rangeOffsets = new ushort[segCount];
			ttfStream.Move(TTFStream.SizeOf_UShort * 3);
			for(int i = 0; i < segCount; i++)
				ends[i] = ttfStream.ReadUShort();
			ttfStream.Move(TTFStream.SizeOf_UShort);
			for(int i = 0; i < segCount; i++)
				starts[i] = ttfStream.ReadUShort();
			for(int i = 0; i < segCount; i++)
				deltas[i] = ttfStream.ReadUShort();
			for(int i = 0; i < segCount; i++)
				rangeOffsets[i] = ttfStream.ReadUShort();
			CreateFormat4Map(ttfStream);
		}
		void CreateFormat4Map(TTFStream ttfStream) {
			int glyphIdArrayPosition = ttfStream.Position;
			for(int i = 0; i < ends.Length; i++) {
				for(int code = starts[i]; code <= ends[i]; code++) {
					ushort glyphIndex = 0;
					if(rangeOffsets[i] != 0 && code != 65535) {
						int glyphIdOffset = (rangeOffsets[i] / 2 + code - starts[i] + i - rangeOffsets.Length) * 2;
						ttfStream.Seek(glyphIdArrayPosition);
						ttfStream.Move(glyphIdOffset);
						glyphIndex = ttfStream.ReadUShort();
						if(glyphIndex != 0)
							glyphIndex = (ushort)((glyphIndex + deltas[i]) & 0xffff);
					} else
						glyphIndex = (ushort)((code + deltas[i]) & 0xffff);
					map[code] = glyphIndex;
				}
			}
		}
		void CreateUnicodeTable2() {
			int startGlyphIdOffset = 0;
			for(int i = 0; i < segCount; i++) {
				bool isContiguousRange = true;
				for(int code = starts[i]; code < ends[i]; code++) {
					if(map[code + 1] != map[code] + 1) {
						isContiguousRange = false;
						break;
					}
				}
				if(isContiguousRange) {
					rangeOffsets[i] = 0;
					int delta = map[starts[i]] - starts[i];
					if(delta < 0)
						delta += 0x10000;
					deltas[i] = (ushort)delta;
				} else {
					rangeOffsets[i] = (ushort)(startGlyphIdOffset + (rangeOffsets.Length - i) * 2);
					startGlyphIdOffset += (ends[i] - starts[i] + 1) * 2;
					deltas[i] = 0;
				}
			}
		}
		void CreateUnicodeTable(TTFInitializeParam param) {
			segCount = 0;
			for(int i = 0; i < param.Chars.Count - 1; i++) {
				if(param.Chars[i + 1] != param.Chars[i] + 1)
					segCount++;
			}
			segCount += 2;
			ends = new ushort[segCount];
			starts = new ushort[segCount];
			deltas = new ushort[segCount];
			rangeOffsets = new ushort[segCount];
			int j = 0;
			if(param.Chars.Count > 0) {
				starts[0] = param.Chars[0];
				for(int i = 0; i < param.Chars.Count - 1; i++) {
					if(param.Chars[i + 1] != param.Chars[i] + 1) {
						ends[j] = param.Chars[i];
						starts[j + 1] = param.Chars[i + 1];
						j++;
					}
				}
				ends[j] = param.Chars[param.Chars.Count - 1];
				starts[j + 1] = ends[j + 1] = 65535;
			}
			CreateUnicodeTable2();
		}
		void WriteGlyphIdArray(TTFStream ttfStream) {
			for(int i = 0; i < rangeOffsets.Length; i++) {
				if(rangeOffsets[i] != 0) {
					for(int code = starts[i]; code <= ends[i]; code++)
						ttfStream.WriteUShort(map[code]);
				}
			}
		}
		void WriteUnicodeTable(TTFStream ttfStream) {
			int tableStartPosition = ttfStream.Position;
			ttfStream.WriteUShort(4); 
			ttfStream.WriteUShort(0); 
			ttfStream.WriteUShort(unicodeVersion);
			ttfStream.WriteUShort((ushort)(segCount * 2));
			double power = Math.Floor(Math.Log(segCount, 2));
			double searchRange = Math.Pow(2, power) * 2;
			ttfStream.WriteUShort((ushort)searchRange); 
			ttfStream.WriteUShort((ushort)power); 
			ttfStream.WriteUShort((ushort)(segCount * 2 - searchRange)); 
			for(int i = 0; i < ends.Length; i++)
				ttfStream.WriteUShort(ends[i]);
			ttfStream.WriteUShort(0); 
			for(int i = 0; i < starts.Length; i++)
				ttfStream.WriteUShort(starts[i]);
			for(int i = 0; i < deltas.Length; i++)
				ttfStream.WriteUShort(deltas[i]);
			for(int i = 0; i < rangeOffsets.Length; i++)
				ttfStream.WriteUShort(rangeOffsets[i]);
			WriteGlyphIdArray(ttfStream);
			int length = ttfStream.Position - tableStartPosition;
			ttfStream.Seek(tableStartPosition);
			ttfStream.Move(TTFStream.SizeOf_UShort);
			ttfStream.WriteUShort((ushort)length);
			ttfStream.Seek(tableStartPosition);
			ttfStream.Move(length);
		}
		protected override void ReadTable(TTFStream ttfStream) {
			int startPosition = ttfStream.Position;
			versionNumber = ttfStream.ReadUShort();
			int tableNumber = Convert.ToInt32(ttfStream.ReadUShort());
			int unicodeTableOffset = -1;
			for(int i = 0; i < tableNumber; i++) {
				ushort pID = ttfStream.ReadUShort();
				this.eID = ttfStream.ReadUShort();
				if((pID == 3 && eID == 1) || (pID == 3 && eID == 0) || (pID == 3 && eID == 3)) {
					unicodeTableOffset = Convert.ToInt32(ttfStream.ReadULong());
					break;
				}
				ttfStream.Move(TTFStream.SizeOf_ULong);
			}
			if(unicodeTableOffset == -1)
				throw new TTFFileException(unicodeMissingError);
			ttfStream.Seek(startPosition);
			ttfStream.Move(unicodeTableOffset);
			ReadUnicodeTable(ttfStream);
		}
		protected override void WriteTable(TTFStream ttfStream) {
			int startPosition = ttfStream.Position;
			ttfStream.WriteUShort(versionNumber);
			ttfStream.WriteUShort(1); 
			ttfStream.WriteUShort(3); 
			ttfStream.WriteUShort(1); 
			ttfStream.WriteULong((uint)(ttfStream.Position - startPosition + TTFStream.SizeOf_ULong));
			WriteUnicodeTable(ttfStream);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFCMap p = pattern as TTFCMap;
			versionNumber = p.versionNumber;
			unicodeVersion = p.unicodeVersion;
			for(int i = 0; i < param.Chars.Count; i++) {
				ushort code = (ushort)param.Chars[i];
				this.map[code] = p[code];
			}
			CreateUnicodeTable(param);
		}
	}
}
