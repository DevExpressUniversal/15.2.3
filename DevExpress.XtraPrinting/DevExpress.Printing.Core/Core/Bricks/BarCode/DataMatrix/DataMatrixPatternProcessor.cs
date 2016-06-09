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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BarCode;
using System.ComponentModel;
using System.Drawing;
using System.IO;
namespace DevExpress.XtraPrinting.BarCode.Native {
	public abstract class DataMatrixPatternProcessor: IPatternProcessor {
		public static DataMatrixPatternProcessor CreateInstance(DataMatrixCompactionMode mode) {
			switch(mode) {
				case DataMatrixCompactionMode.ASCII:
					return new DataMatrixASCIIPatternProcessor();
				case DataMatrixCompactionMode.Text:
					return new DataMatrixTextPatternProcessor();
				case DataMatrixCompactionMode.C40:
					return new DataMatrixC40PatternProcessor();
				case DataMatrixCompactionMode.X12:
					return new DataMatrixX12PatternProcessor();
				case DataMatrixCompactionMode.Edifact:
					return new DataMatrixEdifactPatternProcessor();
				case DataMatrixCompactionMode.Binary:
					return new DataMatrixBinaryPatternProcessor();
				default:
					return null;
			}
		}
		#region fields & properties
		DataMatrixSize matrixSize = DataMatrixSize.MatrixAuto;
		DataMatrixSize realMatrixSize = DataMatrixSize.Matrix10x10;
		List<List<bool>> pattern = new List<List<bool>>();
		byte[] encodeBuf = null;
		int encodeDataSize = 0;
		int protectedDataSize = 0;
		[DefaultValue(DataMatrixSize.MatrixAuto)]
		public DataMatrixSize MatrixSize {
			get { return matrixSize; }
			set {
				if(matrixSize == value) return;
				matrixSize = value;
				CalculatePattern();
			}
		}
		public DataMatrixSize RealMatrixSize {
			get { return realMatrixSize; }
		}
		#endregion
		protected DataMatrixPatternProcessor() {
			matrixSize = DataMatrixSize.MatrixAuto;
			realMatrixSize = DataMatrixSize.Matrix10x10;
			DataMatrixMatrixProperties maxMatrixProperties = DataMatrixMatrixProperties.GetProperties(DataMatrixSize.Matrix144x144);
			int maxEncodeBufSize = maxMatrixProperties.CodewordsTotal + maxMatrixProperties.BlocksTotal * maxMatrixProperties.RsBlock;
			encodeBuf = new byte[maxEncodeBufSize];
			encodeDataSize = protectedDataSize = 0;
			CalculatePattern();
		}
		protected byte Randomize255State(byte codewordValue, int codewordPosition) {
			int pseudoRandom = ((149 * (codewordPosition + 1)) % 255) + 1;
			int result = codewordValue + pseudoRandom;
			if(result >= 256) result -= 256;
			return (byte)result;
		}
		protected byte Randomize253State(byte codewordValue, int codewordPosition) {
			int pseudoRandom = ((149 * (codewordPosition + 1)) % 253) + 1;
			int result = codewordValue + pseudoRandom;
			if(result > 254) result -= 254;
			return (byte)result;
		}
		#region placement
		void ECC200PlacementBit(int[] places, int rowsTotal, int columsTotal, int row, int column, int point, byte bit) {
			if(row < 0) {
				row += rowsTotal;
				column += 4 - ((rowsTotal + 4) % 8);
			}
			if(column < 0) {
				column += columsTotal;
				row += 4 - ((columsTotal + 4) % 8);
			}
			places[row * columsTotal + column] = (point << 3) + bit;
		}
		void ECC200PlacementBlock(int[] places, int dataRows, int dataColumns, int row, int column,  int point) {
			ECC200PlacementBit(places, dataRows, dataColumns, row - 2, column - 2, point, 7);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 2, column - 1, point, 6);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 1, column - 2, point, 5);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 1, column - 1, point, 4);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 1, column - 0, point, 3);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 0, column - 2, point, 2);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 0, column - 1, point, 1);
			ECC200PlacementBit(places, dataRows, dataColumns, row - 0, column - 0, point, 0);
		}
		void ECC200PlacementCornerA(int[] places, int dataRows, int dataColumns, int point) {
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 0, point, 7);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 1, point, 6);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 2, point, 5);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 2, point, 4);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 1, point, 3);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 1, point, 2);
			ECC200PlacementBit(places, dataRows, dataColumns, 2, dataColumns - 1, point, 1);
			ECC200PlacementBit(places, dataRows, dataColumns, 3, dataColumns - 1, point, 0);
		}
		void ECC200PlacementCornerB(int[] places, int dataRows, int dataColumns, int point) {
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 3, 0, point, 7);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 2, 0, point, 6);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 0, point, 5);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 4, point, 4);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 3, point, 3);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 2, point, 2);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 1, point, 1);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 1, point, 0);
		}
		void ECC200PlacementCornerC(int[] places, int dataRows, int dataColumns, int point) {
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 3, 0, point, 7);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 2, 0, point, 6);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 0, point, 5);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 2, point, 4);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 1, point, 3);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 1, point, 2);
			ECC200PlacementBit(places, dataRows, dataColumns, 2, dataColumns - 1, point, 1);
			ECC200PlacementBit(places, dataRows, dataColumns, 3, dataColumns - 1, point, 0);
		}
		void ECC200PlacementCornerD(int[] places, int dataRows, int dataColumns, int point) {
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, 0, point, 7);
			ECC200PlacementBit(places, dataRows, dataColumns, dataRows - 1, dataColumns - 1, point, 6);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 3, point, 5);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 2, point, 4);
			ECC200PlacementBit(places, dataRows, dataColumns, 0, dataColumns - 1, point, 3);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 3, point, 2);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 2, point, 1);
			ECC200PlacementBit(places, dataRows, dataColumns, 1, dataColumns - 1, point, 0);
		}
		void ECC200Placement(int[] places, int dataRows, int dataColumns) {
			for(int r = 0; r < dataRows; r++)
				for(int c = 0; c < dataColumns; c++)
					places[r * dataColumns + c] = 0;
			int row = 4, column = 0, point = 1;
			do {
				if(row == dataRows && column == 0)
					ECC200PlacementCornerA(places, dataRows, dataColumns, point++);
				if(row == dataRows - 2 && column == 0 && (dataColumns % 4) != 0)
					ECC200PlacementCornerB(places, dataRows, dataColumns, point++);
				if(row == dataRows - 2 && column == 0 && (dataColumns % 8) == 4)
					ECC200PlacementCornerC(places, dataRows, dataColumns, point++);
				if(row == dataRows + 4 && column == 2 && (dataColumns % 8) == 0)
					ECC200PlacementCornerD(places, dataRows, dataColumns, point++);
				do {
					if(row < dataRows && column >= 0 && !(places[row * dataColumns + column] != 0))
						ECC200PlacementBlock(places, dataRows, dataColumns, row, column, point++);
					row -= 2;
					column += 2;
				}
				while(row >= 0 && column < dataColumns);
				row++;
				column += 3;
				do {
					if(row >= 0 && column < dataColumns && !(places[row * dataColumns + column] != 0))
						ECC200PlacementBlock(places, dataRows, dataColumns, row, column, point++);
					row += 2;
					column -= 2;
				}
				while(row < dataRows && column >= 0);
				row += 3;
				column++;
			}
			while(row < dataRows || column < dataColumns);
			if(places[dataRows * dataColumns - 1] == 0)
				places[dataRows * dataColumns - 1] = places[dataRows * dataColumns - dataColumns - 2] = 1;
		}
		#endregion
		void SelectMatrix() {
			DataMatrixSize optimalMatrixSize = DataMatrixMatrixProperties.FindOptimalMatrixSize(encodeDataSize);
			DataMatrixMatrixProperties optimalMatrixProperties = DataMatrixMatrixProperties.GetProperties(optimalMatrixSize);
			if(MatrixSize != DataMatrixSize.MatrixAuto && DataMatrixMatrixProperties.GetProperties(MatrixSize).CodewordsTotal >= optimalMatrixProperties.CodewordsTotal)
				realMatrixSize = MatrixSize;
			else
				realMatrixSize = optimalMatrixSize;
		}
		protected virtual void PadData(byte[] encodeBuf, ref int protectedDataSize) {
			DataMatrixMatrixProperties realMatrixProperties = DataMatrixMatrixProperties.GetProperties(RealMatrixSize);
			if(protectedDataSize < realMatrixProperties.CodewordsTotal)
				encodeBuf[protectedDataSize++] = DataMatrixConstants.AsciiPad;
			while(protectedDataSize < realMatrixProperties.CodewordsTotal) {
				encodeBuf[protectedDataSize] = Randomize253State(129, protectedDataSize);
				protectedDataSize++;
			}
		}
		void ProtectData() {
			protectedDataSize = encodeDataSize;
			PadData(encodeBuf, ref protectedDataSize);
			DataMatrixMatrixProperties realMatrixProperties = DataMatrixMatrixProperties.GetProperties(RealMatrixSize);
			ECC200ReedSolomon reedSolomon = new ECC200ReedSolomon(0x12d, realMatrixProperties.RsBlock, 1);
			byte[] dataBlock = new byte[realMatrixProperties.DataBlock];
			int symbolTotalWords = realMatrixProperties.CodewordsTotal + realMatrixProperties.BlocksTotal * realMatrixProperties.RsBlock;
			for(int block = 0; block < realMatrixProperties.BlocksTotal; block++) {
				int p = 0;
				for(int n = block; n < realMatrixProperties.CodewordsTotal; n += realMatrixProperties.BlocksTotal)
					dataBlock[p++] = encodeBuf[n];
				byte[] rsBlock = null;
				reedSolomon.Encode(dataBlock, p, out rsBlock);
				p = realMatrixProperties.RsBlock - 1;
				int dataBlockSize = realMatrixProperties.GetDataBlockSize(block);
				for(int n = block + realMatrixProperties.BlocksTotal * dataBlockSize; n < symbolTotalWords; n += realMatrixProperties.BlocksTotal)
					encodeBuf[n] = rsBlock[p--];
			}
			protectedDataSize = symbolTotalWords;
		}
		void FillPattern() {
			DataMatrixMatrixProperties realMatrixProperties = DataMatrixMatrixProperties.GetProperties(RealMatrixSize);
			int dataRows = realMatrixProperties.SymbolHeight - 2 * (realMatrixProperties.SymbolHeight / realMatrixProperties.RegionHeight);
			int dataColumns = realMatrixProperties.SymbolWidth - 2 * (realMatrixProperties.SymbolWidth / realMatrixProperties.RegionWidth);
			int[] places = new int[dataRows * dataColumns];
			ECC200Placement(places, dataRows, dataColumns);
			byte[] grid = new byte[realMatrixProperties.SymbolHeight * realMatrixProperties.SymbolWidth];
			for(int row = 0; row < realMatrixProperties.SymbolHeight; row += realMatrixProperties.RegionHeight) {
				for(int column = 0; column < realMatrixProperties.SymbolWidth; column++)
					grid[row * realMatrixProperties.SymbolWidth + column] = 1;
				for(int column = 0; column < realMatrixProperties.SymbolWidth; column += 2)
					grid[(row + realMatrixProperties.RegionHeight - 1) * realMatrixProperties.SymbolWidth + column] = 1;
			}
			for(int column = 0; column < realMatrixProperties.SymbolWidth; column += realMatrixProperties.RegionWidth) {
				for(int row = 0; row < realMatrixProperties.SymbolHeight; row++)
					grid[row * realMatrixProperties.SymbolWidth + column] = 1;
				for(int row = 0; row < realMatrixProperties.SymbolHeight; row += 2)
					grid[row * realMatrixProperties.SymbolWidth + column + realMatrixProperties.RegionWidth - 1] = 1;
			}
			for(int row = 0; row < dataRows; row++) {
				for(int column = 0; column < dataColumns; column++) {
					int value = places[(dataRows - row - 1) * dataColumns + column];
					if(value == 1 || value > 7 && (encodeBuf[(value >> 3) - 1] & (1 << (value & 7))) != 0)
						grid[(1 + row + 2 * (row / (realMatrixProperties.RegionHeight - 2))) * realMatrixProperties.SymbolWidth +
							1 + column + 2 * (column / (realMatrixProperties.RegionWidth - 2))] = 1;
				}
			}
			pattern.Clear();
			for(int row = 0; row < realMatrixProperties.SymbolHeight; row++) {
				List<bool> listRow = new List<bool>();
				for(int column = 0; column < realMatrixProperties.SymbolWidth; column++) {
					listRow.Add(grid[(realMatrixProperties.SymbolHeight - row - 1) * realMatrixProperties.SymbolWidth + column] != 0);
				}
				pattern.Add(listRow);
			}
		}
		void CalculatePattern() {
			SelectMatrix();
			ProtectData();
			FillPattern();
		}
		public abstract string GetValidCharSet();
		protected abstract bool EncodeData(object data, byte[] encodeBuf, out int encodeBufSize);
		#region IPatternProcessor Members
		ArrayList IPatternProcessor.Pattern {
			get { return new ArrayList(pattern); }
		}
		void IPatternProcessor.RefreshPattern(object data) {
			if(!EncodeData(data, encodeBuf, out encodeDataSize)) return;
			CalculatePattern();
		}
		void IPatternProcessor.Assign(IPatternProcessor source) {
			this.matrixSize = ((DataMatrixPatternProcessor)source).matrixSize;
		}
		#endregion
	}
	public class DataMatrixASCIIPatternProcessor : DataMatrixPatternProcessor {
		static string validCharset = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\x7F\x81\x8d\x8f\x90\x9d\xa0¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
		static bool IsDigit(byte b) {
			return b >= '0' && b <= '9';
		}
		static byte GetDigit(byte b) {
			return (byte)(b - '0');
		}
		public static string ValidCharSet { get { return validCharset; } }
		public static bool TextToAscii(byte[] asciiBuf, int asciiBufSize, ref int asciiLen, byte[] textBuf, int textSize, ref int textPtr) {
			while(asciiLen < asciiBufSize && textPtr < textSize) {
				if(textBuf[textPtr] == DataMatrixGS1Generator.fnc1Char) {
					asciiBuf[asciiLen++] = textBuf[textPtr++];
					continue;
				}
				if(textPtr + 1 < textSize && IsDigit(textBuf[textPtr]) && IsDigit(textBuf[textPtr + 1])) {
					asciiBuf[asciiLen++] = (byte)(GetDigit(textBuf[textPtr]) * 10 + GetDigit(textBuf[textPtr + 1]) + 130);
					textPtr += 2;
				} else if(textBuf[textPtr] > 127) {
					if(asciiLen + 1 >= asciiBufSize) break;
					asciiBuf[asciiLen++] = DataMatrixConstants.AsciiUpperShift;
					asciiBuf[asciiLen++] = (byte)(textBuf[textPtr++] - 127);
				} else
					asciiBuf[asciiLen++] = (byte)(textBuf[textPtr++] + 1);
			}
			return textPtr == textSize;
		}
		public DataMatrixASCIIPatternProcessor() {
		}
		public override string GetValidCharSet() { return validCharset; }
		protected override bool EncodeData(object data, byte[] encodeBuf, out int encodeBufSize) {
			System.Diagnostics.Debug.Assert(data is string, "data must be string type");
#if !WINRT && !WP
			byte[] byteText = Encoding.GetEncoding(28591).GetBytes((string)data);
#else
			byte[] byteText = Encoding.GetEncoding("iso-8859-1").GetBytes((string)data);
#endif
			DataMatrixMatrixProperties maxMatrixProperties = DataMatrixMatrixProperties.GetProperties(DataMatrixSize.Matrix144x144);
			int byteTextPtr = 0;
			encodeBufSize = 0;
			return TextToAscii(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize, byteText, byteText.Length, ref byteTextPtr);
		}
	}
	public class DataMatrixC40PatternProcessor : DataMatrixPatternProcessor {
		protected virtual string BasicCharset { get { return " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; } }
		protected virtual string Shift2Charset { get { return "!\"#$%&'()*+,-./:;<=>?@[\\]^_"; } }
		protected virtual string Shift3Charset { get { return "`abcdefghijklmnopqrstuvwxyz{|}~\x7F"; } }
		protected virtual byte Latch { get { return DataMatrixConstants.AsciiLatchToC40; } }
		protected virtual bool OnlyBasicCharset { get { return false; } }
		protected virtual byte UnknownChar { get { return (byte)'?'; } }
		protected virtual bool UpperShift { get { return true; } }
		int c40Len = 0;
		byte[] c40Buf = new byte[6];
		public DataMatrixC40PatternProcessor() {
		}
		void CharToC40(byte inChar) {
			if((inChar & 0x80) != 0 && UpperShift) {
				inChar &= 0x7F;
				c40Buf[c40Len++] = (byte)1;
				c40Buf[c40Len++] = (byte)30;
			}
			int index = BasicCharset.IndexOf((char)inChar);
			if(index >= 0) {
				c40Buf[c40Len++] = (byte)((index + 3) % 40);
				return;
			}
			if(!OnlyBasicCharset) {
				if(inChar < 32) {
					c40Buf[c40Len++] = (byte)0;
					c40Buf[c40Len++] = inChar;
					return;
				}
				index = Shift2Charset.IndexOf((char)inChar);
				if(index >= 0) {
					c40Buf[c40Len++] = (byte)1;
					c40Buf[c40Len++] = (byte)index;
					return;
				}
				index = Shift3Charset.IndexOf((char)inChar);
				if(index >= 0) {
					c40Buf[c40Len++] = (byte)2;
					c40Buf[c40Len++] = (byte)index;
					return;
				}
			}
			CharToC40(UnknownChar);
		}
		void C40ToBytes(byte[] byteBuf, int byteSize, ref int byteLen) {
			while(byteLen + 1 < byteSize && c40Len >= 3) {
				int c40Word = c40Buf[0] * 1600 + c40Buf[1] * 40 + c40Buf[2] + 1;
				byteBuf[byteLen++] = (byte)(c40Word >> 8);
				byteBuf[byteLen++] = (byte)(c40Word & 0xFF);
				Array.Copy(c40Buf, 3, c40Buf, 0, c40Len - 3);
				c40Len -= 3;
			}
		}
		public override string GetValidCharSet() { return DataMatrixASCIIPatternProcessor.ValidCharSet; }
		protected override bool EncodeData(object data, byte[] encodeBuf, out int encodeBufSize) {
			System.Diagnostics.Debug.Assert(data is string, "data must be string type");
#if !WINRT && !WP
			byte[] byteText = Encoding.GetEncoding(28591).GetBytes((string)data);
#else
			byte[] byteText = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding("iso-8859-1"), Encoding.Unicode.GetBytes((string)data));
#endif
			DataMatrixMatrixProperties maxMatrixProperties = DataMatrixMatrixProperties.GetProperties(DataMatrixSize.Matrix144x144);
			int byteTextPtr = 0;
			encodeBufSize = 0;
			c40Len = 0;
			if(byteText.Length == 0) return true;
			int textPtrAlignedToC40Word = 0;
			int encodePtrAlignedToC40Word = 0;
			if(encodeBufSize < maxMatrixProperties.CodewordsTotal) {
				encodeBuf[encodeBufSize++] = Latch;
			}
			while(encodeBufSize + 1 < maxMatrixProperties.CodewordsTotal && byteTextPtr < byteText.Length) {
				CharToC40(byteText[byteTextPtr++]);
				C40ToBytes(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize);
				if(c40Len == 0) {
					textPtrAlignedToC40Word = byteTextPtr;
					encodePtrAlignedToC40Word = encodeBufSize;
				}
			}
			encodeBufSize = encodePtrAlignedToC40Word;
			byteTextPtr = textPtrAlignedToC40Word;
			if(encodeBufSize != 0 && encodeBufSize < maxMatrixProperties.CodewordsTotal) {
				encodeBuf[encodeBufSize++] = DataMatrixConstants.C40X12TextUnlatch;
			}
			if(byteTextPtr < byteText.Length) {
				DataMatrixASCIIPatternProcessor.TextToAscii(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize, byteText, byteText.Length, ref byteTextPtr);
			}
			return byteTextPtr == byteText.Length;
		}
	}
	public class DataMatrixTextPatternProcessor : DataMatrixC40PatternProcessor {
		protected override string BasicCharset { get { return " 0123456789abcdefghijklmnopqrstuvwxyz"; } }
		protected override string Shift2Charset { get { return "!\"#$%&'()*+,-./:;<=>?@[\\]^_"; } }
		protected override string Shift3Charset { get { return "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\x7F"; } }
		protected override byte Latch { get { return DataMatrixConstants.AsciiLatchToText; } }
		protected override bool OnlyBasicCharset { get { return false; } }
		protected override byte UnknownChar { get { return (byte)'?'; } }
		protected override bool UpperShift { get { return true; } }
	}
	public class DataMatrixX12PatternProcessor : DataMatrixC40PatternProcessor {
		static string validCharset = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\r*>";
		protected override string BasicCharset { get { return " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\r*>"; } }
		protected override string Shift2Charset { get { return ""; } }
		protected override string Shift3Charset { get { return ""; } }
		protected override byte Latch { get { return DataMatrixConstants.AsciiLatchToX12; } }
		protected override bool OnlyBasicCharset { get { return true; } }
		protected override byte UnknownChar { get { return (byte)'*'; } }
		protected override bool UpperShift { get { return false; } }
		public override string GetValidCharSet() { return validCharset; }
	}
	public class DataMatrixEdifactPatternProcessor : DataMatrixPatternProcessor {
		static string validCharset = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^";
		byte[] byteText = null;
		int textPtrAlignedToEdifactWord = 0;
		int encodePtrAlignedToEdifactWord = 0;
		int edifactLen = 0;
		byte[] edifact = new byte[4];		
		public DataMatrixEdifactPatternProcessor() {
		}
		void ConvertToEdifactCharset(byte[] byteText, int byteSize) {
			for(int byteTextPtr = 0; byteTextPtr < byteSize; byteTextPtr++) {
				if(byteText[byteTextPtr] < 32 || byteText[byteTextPtr] > 94) byteText[byteTextPtr] = (byte)'?';
			}
		}
		void CharToEdifact(byte inChar) {
			if(inChar < 32 || inChar > 95) inChar = (byte)'?';
			edifact[edifactLen++] = (byte)(inChar & 0x3F);
		}
		int EdifactToBytes(byte[] byteBuf, int byteSize, ref int byteLen) {
			int edifactToWrite = 0;
			if(byteLen < byteSize && edifactToWrite < edifactLen) {
				byteBuf[byteLen] = (byte)((edifact[edifactToWrite++] & 0x3F) << 2);
				if(edifactToWrite < edifactLen)
					byteBuf[byteLen] |= (byte)((edifact[edifactToWrite] & 0x30) >> 4);
				byteLen++;
			}
			if(byteLen < byteSize && edifactToWrite < edifactLen) {
				byteBuf[byteLen] = (byte)((edifact[edifactToWrite++] & 0x0F) << 4);
				if(edifactToWrite < edifactLen)
					byteBuf[byteLen] |= (byte)((edifact[edifactToWrite] & 0x3C) >> 2);
				byteLen++;
			}
			if(byteLen < byteSize && edifactToWrite < edifactLen) {
				byteBuf[byteLen] = (byte)((edifact[edifactToWrite++] & 0x03) << 6);
				if(edifactToWrite < edifactLen)
					byteBuf[byteLen] |= (byte)(edifact[edifactToWrite++] & 0x3F);
				byteLen++;
			}
			return edifactToWrite;
		}
		protected override void PadData(byte[] encodeBuf, ref int protectedDataSize) {
			DataMatrixMatrixProperties realMatrixProperties = DataMatrixMatrixProperties.GetProperties(RealMatrixSize);
			if(encodePtrAlignedToEdifactWord > 0) {
				if(realMatrixProperties.CodewordsTotal - encodePtrAlignedToEdifactWord <= 2) {
					int byteTextPtr = textPtrAlignedToEdifactWord;
					protectedDataSize = encodePtrAlignedToEdifactWord;
					DataMatrixASCIIPatternProcessor.TextToAscii(encodeBuf, realMatrixProperties.CodewordsTotal, ref protectedDataSize, byteText, byteText.Length, ref byteTextPtr);
				} else {
					protectedDataSize = encodePtrAlignedToEdifactWord;
					CharToEdifact(DataMatrixConstants.EdifactUnlatch);
					EdifactToBytes(encodeBuf, realMatrixProperties.CodewordsTotal, ref protectedDataSize);
					edifactLen--;
				}
			}
			base.PadData(encodeBuf, ref protectedDataSize);
		}
		public override string GetValidCharSet() { return validCharset; }
		protected override bool EncodeData(object data, byte[] encodeBuf, out int encodeBufSize) {
			System.Diagnostics.Debug.Assert(data is string, "data must be string type");
#if !WINRT && !WP
			byteText = Encoding.Convert(Encoding.Default, Encoding.GetEncoding(28591), Encoding.Default.GetBytes((string)data));
#else
			byteText = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding("iso-8859-1"), Encoding.Unicode.GetBytes((string)data));
#endif
			DataMatrixMatrixProperties maxMatrixProperties = DataMatrixMatrixProperties.GetProperties(DataMatrixSize.Matrix144x144);
			ConvertToEdifactCharset(byteText, byteText.Length);
			encodePtrAlignedToEdifactWord = 0;
			textPtrAlignedToEdifactWord = 0;
			int byteTextPtr = 0;
			encodeBufSize = 0;		 
			edifactLen = 0;
			if(byteText.Length == 0) return true;
			if(encodeBufSize < maxMatrixProperties.CodewordsTotal) {
				encodeBuf[encodeBufSize++] = DataMatrixConstants.AsciiLatchToEdifact;
			}
			while(encodeBufSize+2 < maxMatrixProperties.CodewordsTotal && byteTextPtr < byteText.Length) {
				CharToEdifact(byteText[byteTextPtr++]);
				if(edifactLen >= 4) {
					EdifactToBytes(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize);
					encodePtrAlignedToEdifactWord = encodeBufSize;
					textPtrAlignedToEdifactWord = byteTextPtr;
					edifactLen = 0;
				}
			}
			byteTextPtr = textPtrAlignedToEdifactWord;
			encodeBufSize = encodePtrAlignedToEdifactWord;
			DataMatrixASCIIPatternProcessor.TextToAscii(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize, byteText, byteText.Length, ref byteTextPtr);
			if(encodePtrAlignedToEdifactWord > 0 && encodeBufSize - encodePtrAlignedToEdifactWord > 2) {
				encodeBufSize = encodePtrAlignedToEdifactWord;
				CharToEdifact(DataMatrixConstants.EdifactUnlatch);
				EdifactToBytes(encodeBuf, maxMatrixProperties.CodewordsTotal, ref encodeBufSize);
				edifactLen--;
			}
			return byteTextPtr == byteText.Length;
		}
	}
	public class DataMatrixBinaryPatternProcessor : DataMatrixPatternProcessor {
		public DataMatrixBinaryPatternProcessor() {
		}
		public override string GetValidCharSet() { return string.Empty; }
		protected override bool EncodeData(object data, byte[] encodeBuf, out int encodeBufSize) {
			DataMatrixMatrixProperties maxMatrixProperties = DataMatrixMatrixProperties.GetProperties(DataMatrixSize.Matrix144x144);
			System.Diagnostics.Debug.Assert(data is byte[], "data must be byte[] type");
			byte[] byteData = (byte[])data;
			encodeBufSize = 0;
			if(byteData.Length == 0) {
				return true;
			} else if(byteData.Length < 250 && 2 + byteData.Length <= maxMatrixProperties.CodewordsTotal) {
				encodeBuf[encodeBufSize] = DataMatrixConstants.AsciiLatchToB256;
				encodeBufSize++;
				encodeBuf[encodeBufSize] = Randomize255State((byte)byteData.Length, encodeBufSize);
				encodeBufSize++;
			} else if(byteData.Length < 1750 && 3 + byteData.Length <= maxMatrixProperties.CodewordsTotal) {
				encodeBuf[encodeBufSize] = DataMatrixConstants.AsciiLatchToB256;
				encodeBufSize++;
				encodeBuf[encodeBufSize] = Randomize255State((byte)(249 + (byteData.Length / 250)), encodeBufSize);
				encodeBufSize++;
				encodeBuf[encodeBufSize] = Randomize255State((byte)(byteData.Length % 250), encodeBufSize);
				encodeBufSize++;
			} else {
				return false;
			}
			int byteDataPtr = 0;
			while(byteDataPtr < byteData.Length) {
				encodeBuf[encodeBufSize] = Randomize255State((byte)byteData[byteDataPtr++], encodeBufSize);
				encodeBufSize++;
			}
			return true;
		}
	}
}
