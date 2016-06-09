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
using System.IO;
using System.IO.Compression;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraPrinting.Export.Pdf.Compression {
	public class Deflater {
		BitBuffer bitBuffer = new BitBuffer();
		Adler32 adler32 = new Adler32();
		static TableItem[] lengthTable = new TableItem[29];
		static TableItem[] offsetTable = new TableItem[30];
		static CodeTableItem[] lengthBaseCodeTable = new CodeTableItem[4];
		const int lz77WindowSizeExponent = 13;
		static Deflater() {
			FillLengthTable();
			FillOffsetTable();
			FillLengthBaseCodeTable();
		}
		public static MemoryStream DeflateStream(MemoryStream source) {
			MemoryStream dest = new MemoryStream();
			source.Seek(0, SeekOrigin.Begin);
			new Deflater().Deflate(source, dest);
			return dest;
		}
		static void FillLengthTable() {
			lengthTable[0] = new TableItem(3, 3, 257, 0);
			lengthTable[1] = new TableItem(4, 4, 258, 0);
			lengthTable[2] = new TableItem(5, 5, 259, 0);
			lengthTable[3] = new TableItem(6, 6, 260, 0);
			lengthTable[4] = new TableItem(7, 7, 261, 0);
			lengthTable[5] = new TableItem(8, 8, 262, 0);
			lengthTable[6] = new TableItem(9, 9, 263, 0);
			lengthTable[7] = new TableItem(10, 10, 264, 0);
			lengthTable[8] = new TableItem(11, 12, 265, 1);
			lengthTable[9] = new TableItem(13, 14, 266, 1);
			lengthTable[10] = new TableItem(15, 16, 267, 1);
			lengthTable[11] = new TableItem(17, 18, 268, 1);
			lengthTable[12] = new TableItem(19, 22, 269, 2);
			lengthTable[13] = new TableItem(23, 26, 270, 2);
			lengthTable[14] = new TableItem(27, 30, 271, 2);
			lengthTable[15] = new TableItem(31, 34, 272, 2);
			lengthTable[16] = new TableItem(35, 42, 273, 3);
			lengthTable[17] = new TableItem(43, 50, 274, 3);
			lengthTable[18] = new TableItem(51, 58, 275, 3);
			lengthTable[19] = new TableItem(59, 66, 276, 3);
			lengthTable[20] = new TableItem(67, 82, 277, 4);
			lengthTable[21] = new TableItem(83, 98, 278, 4);
			lengthTable[22] = new TableItem(99, 114, 279, 4);
			lengthTable[23] = new TableItem(115, 130, 280, 4);
			lengthTable[24] = new TableItem(131, 162, 281, 5);
			lengthTable[25] = new TableItem(163, 194, 282, 5);
			lengthTable[26] = new TableItem(195, 226, 283, 5);
			lengthTable[27] = new TableItem(227, 257, 284, 5);
			lengthTable[28] = new TableItem(258, 258, 285, 0);
		}
		static void FillOffsetTable() {
			offsetTable[0] = new TableItem(1, 1, 0, 0);
			offsetTable[1] = new TableItem(2, 2, 1, 0);
			offsetTable[2] = new TableItem(3, 3, 2, 0);
			offsetTable[3] = new TableItem(4, 4, 3, 0);
			offsetTable[4] = new TableItem(5, 6, 4, 1);
			offsetTable[5] = new TableItem(7, 8, 5, 1);
			offsetTable[6] = new TableItem(9, 12, 6, 2);
			offsetTable[7] = new TableItem(13, 16, 7, 2);
			offsetTable[8] = new TableItem(17, 24, 8, 3);
			offsetTable[9] = new TableItem(25, 32, 9, 3);
			offsetTable[10] = new TableItem(33, 48, 10, 4);
			offsetTable[11] = new TableItem(49, 64, 11, 4);
			offsetTable[12] = new TableItem(65, 96, 12, 5);
			offsetTable[13] = new TableItem(97, 128, 13, 5);
			offsetTable[14] = new TableItem(129, 192, 14, 6);
			offsetTable[15] = new TableItem(193, 256, 15, 6);
			offsetTable[16] = new TableItem(257, 384, 16, 7);
			offsetTable[17] = new TableItem(385, 512, 17, 7);
			offsetTable[18] = new TableItem(513, 768, 18, 8);
			offsetTable[19] = new TableItem(769, 1024, 19, 8);
			offsetTable[20] = new TableItem(1025, 1536, 20, 9);
			offsetTable[21] = new TableItem(1537, 2048, 21, 9);
			offsetTable[22] = new TableItem(2049, 3072, 22, 10);
			offsetTable[23] = new TableItem(3073, 4096, 23, 10);
			offsetTable[24] = new TableItem(4097, 6144, 24, 11);
			offsetTable[25] = new TableItem(6145, 8192, 25, 11);
			offsetTable[26] = new TableItem(8193, 12288, 26, 12);
			offsetTable[27] = new TableItem(12289, 16384, 27, 12);
			offsetTable[28] = new TableItem(16385, 24576, 28, 13);
			offsetTable[29] = new TableItem(24577, 32768, 29, 13);
		}
		static void FillLengthBaseCodeTable() {
			lengthBaseCodeTable[0] = new CodeTableItem(0, 143, 8, 0x30);
			lengthBaseCodeTable[1] = new CodeTableItem(144, 255, 9, 0x190);
			lengthBaseCodeTable[2] = new CodeTableItem(256, 279, 7, 0x0);
			lengthBaseCodeTable[3] = new CodeTableItem(280, 287, 8, 0xC0);
		}
		void EncodeLengthBase(int lengthBase) {
			CodeTableItem item = null;
			for(int i = 0; i < lengthBaseCodeTable.Length; i++) {
				if(lengthBaseCodeTable[i].Contains(lengthBase)) {
					item = lengthBaseCodeTable[i];
					break;
				}
			}
			if(item == null)
				throw new CompressException("Invalid base of the length");
			item.Encode(lengthBase, bitBuffer);
		}
		void EncodeOffsetBase(int offsetBase) {
			int reverseValue = Utils.BitReverse(offsetBase, 5);
			bitBuffer.WriteBits(reverseValue, 5);
		}
		void EncodeLiteral(byte literal) {
			EncodeLengthBase((int)literal);
		}
		void EncodeLength(int length) {
			TableItem item = null;
			for(int i = 0; i < lengthTable.Length; i++) {
				if(lengthTable[i].Contains(length)) {
					item = lengthTable[i];
					break;
				}
			}
			if(item == null)
				throw new CompressException("Invalid length");
			EncodeLengthBase(item.Base);
			item.EncodeExtraBits(length, bitBuffer);
		}
		void EncodeOffset(int offset) {
			TableItem item = null;
			for(int i = 0; i < offsetTable.Length; i++) {
				if(offsetTable[i].Contains(offset)) {
					item = offsetTable[i];
					break;
				}
			}
			if(item == null)
				throw new CompressException("Invalid offset");
			EncodeOffsetBase(item.Base);
			item.EncodeExtraBits(offset, bitBuffer);
		}
		void WriteZLibHeader() {
			int header = (8 + ((lz77WindowSizeExponent - 8) << 4)) << 8; 
			header |= 2 << 6; 
			header += 31 - (header % 31); 
			bitBuffer.WriteShortMSB(header);
		}
		void WriteBlockHeader() {
			bitBuffer.WriteBits(1, 1); 
			bitBuffer.WriteBits(1, 2); 
		}
		bool ProcessLZ77ResultValue(LZ77ResultValue resultValue) {
			if(resultValue == null) return false;
			if(resultValue.IsLiteral) 
				EncodeLiteral(resultValue.Literal);
			else {
				EncodeLength(resultValue.Length);
				EncodeOffset(resultValue.Offset);
			}
			return true;
		}
		public void Deflate(MemoryStream source, MemoryStream dest) {
			if(source.Length == 0) return;
			WriteZLibHeader();
			bitBuffer.ToStream(dest);
			DeflateStream compressor = new DeflateStream(dest, CompressionMode.Compress, true);
			source.WriteTo(compressor);
			compressor.Dispose();
			int adler = (int)adler32.Calculate(source.GetBuffer(), 0, (int)source.Length);
			dest.WriteByte((byte)(adler >> 24));
			dest.WriteByte((byte)(adler >> 16));
			dest.WriteByte((byte)((adler & 0xFFFF) >> 8));
			dest.WriteByte((byte)(adler & 0xFFFF));
		}
	}
	public class TableItem {
		int start;
		int end;
		int base_;
		int extraBitsCount;
		public TableItem(int start, int end, int base_, int extraBitsCount) {
			this.start = start;
			this.end = end;
			this.base_ = base_;
			this.extraBitsCount = extraBitsCount;
		}
		public bool Contains(int value) {
			return (value >= start) && (value <= end);
		}
		public void EncodeExtraBits(int value, BitBuffer bitBuffer) {
			bitBuffer.WriteBits(value - start, extraBitsCount);
		}
		public int Start { get { return start; } }
		public int End { get { return end; } }
		public int Base { get { return base_; } }
		public int ExtraBitsCount { get { return extraBitsCount; } }
	}
	public class CodeTableItem {
		int start;
		int end;
		int codeLength;
		int codeStart;
		public CodeTableItem(int start, int end, int codeLength, int codeStart) {
			this.start = start;
			this.end = end;
			this.codeLength = codeLength;
			this.codeStart = codeStart;
		}
		public bool Contains(int value) {
			return (value >= start) && (value <= end);
		}
		public void Encode(int value, BitBuffer bitBuffer) {
			int reverseValue = (int)Utils.BitReverse(codeStart + (value - start), codeLength);
			bitBuffer.WriteBits(reverseValue, codeLength);
		}
		public int Start { get { return start; } }
		public int End { get { return end; } }
		public int CodeLength { get { return codeLength; } }
		public int CodeStart { get { return codeStart; } }
	}
	public class Utils {
		static readonly byte[] bit4Reverse = new byte[] {0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15};
		public static short BitReverse(int value, int length) {
			value <<= 16 - length;
			return (short)(bit4Reverse[value & 0xF] << 12
				| bit4Reverse[(value >> 4) & 0xF] << 8
				| bit4Reverse[(value >> 8) & 0xF] << 4
				| bit4Reverse[value >> 12]);
		}
	}
	public class CompressException : Exception {
		public CompressException() {
		}
		public CompressException(string message) : base(message) {
		}
		public CompressException(string message, Exception innerEx) : base(message, innerEx) {
		}
	}
}
