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
	public class PdfCCITTFaxDecoder {
		enum CodingMode { Pass, Horizontal, Vertical0, VerticalRight1, VerticalRight2, VerticalRight3, VerticalLeft1, VerticalLeft2, VerticalLeft3, EndOfData }
		static readonly Dictionary<string, int> whiteRunLengths = new Dictionary<string, int>() {
			{ "00110101", 0 }, { "000111", 1 }, { "0111", 2 }, { "1000", 3 }, { "1011", 4 }, { "1100", 5 }, { "1110", 6 }, { "1111", 7 }, { "10011", 8 }, { "10100", 9 }, { "00111", 10 },
			{ "01000", 11 }, { "001000", 12 }, { "000011", 13 }, { "110100", 14 }, { "110101", 15 }, { "101010", 16 }, { "101011", 17 }, { "0100111", 18 }, { "0001100", 19 },
			{ "0001000", 20 }, { "0010111", 21 }, { "0000011", 22 }, { "0000100", 23 }, { "0101000", 24 }, { "0101011", 25 }, { "0010011", 26 }, { "0100100", 27 }, { "0011000", 28 },
			{ "00000010", 29 }, { "00000011", 30 }, { "00011010", 31 }, { "00011011", 32 }, { "00010010", 33 }, { "00010011", 34 }, { "00010100", 35 }, { "00010101", 36 }, 
			{ "00010110", 37 }, { "00010111", 38 }, { "00101000", 39 }, { "00101001", 40 }, { "00101010", 41 }, { "00101011", 42 }, { "00101100", 43 }, { "00101101", 44 },
			{ "00000100", 45 }, { "00000101", 46 }, { "00001010", 47 }, { "00001011", 48 }, { "01010010", 49 }, { "01010011", 50 }, { "01010100", 51 }, { "01010101", 52 },
			{ "00100100", 53 }, { "00100101", 54 }, { "01011000", 55 }, { "01011001", 56 }, { "01011010", 57 }, { "01011011", 58 }, { "01001010", 59 }, { "01001011", 60 },
			{ "00110010", 61 }, { "00110011", 62 }, { "00110100", 63 }, { "11011", 64 }, { "10010", 128 }, { "010111", 192 }, { "0110111", 256 }, { "00110110", 320 }, { "00110111", 384 },
			{ "01100100", 448 }, { "01100101", 512 }, { "01101000", 576 }, { "01100111", 640 }, { "011001100", 704 }, { "011001101", 768 }, { "011010010", 832 }, { "011010011", 896 },
			{ "011010100", 960 }, { "011010101", 1024 }, { "011010110", 1088 }, { "011010111", 1152 }, { "011011000", 1216 }, { "011011001", 1280 }, { "011011010", 1344 },
			{ "011011011", 1408 }, { "010011000", 1472 }, { "010011001", 1536 }, { "010011010", 1600 }, { "011000", 1664 }, { "010011011", 1728 }, { "000000000001", -1 } };
		static readonly Dictionary<string, int> blackRunLengths = new Dictionary<string, int>() {
			{ "0000110111", 0 }, { "010", 1 }, { "11", 2 }, { "10", 3 }, { "011", 4 }, { "0011", 5 }, { "0010", 6 }, { "00011", 7 }, { "000101", 8 }, { "000100", 9 }, { "0000100", 10 },
			{ "0000101", 11 }, { "0000111", 12 }, { "00000100", 13 }, { "00000111", 14 }, { "000011000", 15 }, { "0000010111", 16 }, { "0000011000", 17 }, { "0000001000", 18 },
			{ "00001100111", 19 }, { "00001101000", 20 }, { "00001101100", 21 }, { "00000110111", 22 }, { "00000101000", 23 }, { "00000010111", 24 }, { "00000011000", 25 },
			{ "000011001010", 26 }, { "000011001011", 27 }, { "000011001100", 28 }, { "000011001101", 29 }, { "000001101000", 30 }, { "000001101001", 31 }, { "000001101010", 32 },
			{ "000001101011", 33 }, { "000011010010", 34 }, { "000011010011", 35 }, { "000011010100", 36 }, { "000011010101", 37 }, { "000011010110", 38 }, { "000011010111", 39 },
			{ "000001101100", 40 }, { "000001101101", 41 }, { "000011011010", 42 }, { "000011011011", 43 }, { "000001010100", 44 }, { "000001010101", 45 }, { "000001010110", 46 },
			{ "000001010111", 47 }, { "000001100100", 48 }, { "000001100101", 49 }, { "000001010010", 50 }, { "000001010011", 51 }, { "000000100100", 52 }, { "000000110111", 53 },
			{ "000000111000", 54 }, { "000000100111", 55 }, { "000000101000", 56 }, { "000001011000", 57 }, { "000001011001", 58 }, { "000000101011", 59 }, { "000000101100", 60 },
			{ "000001011010", 61 }, { "000001100110", 62 }, { "000001100111", 63 }, { "0000001111", 64 }, { "000011001000", 128 }, { "000011001001", 192 }, { "000001011011", 256 },
			{ "000000110011", 320 }, { "000000110100", 384 }, { "000000110101", 448 }, { "0000001101100", 512 }, { "0000001101101", 576 }, { "0000001001010", 640 }, 
			{ "0000001001011", 704 }, { "0000001001100", 768 }, { "0000001001101", 832 }, { "0000001110010", 896 }, { "0000001110011", 960 }, { "0000001110100", 1024 },
			{ "0000001110101", 1088 }, { "0000001110110", 1152 }, { "0000001110111", 1216 }, { "0000001010010", 1280 }, { "0000001010011", 1344 }, { "0000001010100", 1408 },
			{ "0000001010101", 1472 }, { "0000001011010", 1536 }, { "0000001011011", 1600 }, { "0000001100100", 1664 }, { "0000001100101", 1728 }, { "000000000001", -1 } };
		static readonly Dictionary<string, int> commonRunLengths = new Dictionary<string, int>() {
			{ "00000001000", 1792 }, { "00000001100", 1856 }, { "00000001101", 1920 }, { "000000010010", 1984 }, { "000000010011", 2048 }, { "000000010100", 2112 },
			{ "000000010101", 2176 }, { "000000010110", 2240 }, { "000000010111", 2304 }, { "000000011100", 2368 }, { "000000011101", 2432 }, { "000000011110", 2496 },
			{ "000000011111", 2560 } };
		static readonly PdfHuffmanTreeBranch whiteTree = new PdfHuffmanTreeBranch();
		static readonly PdfHuffmanTreeBranch blackTree = new PdfHuffmanTreeBranch();
		static PdfCCITTFaxDecoder() {
			whiteTree.Fill(whiteRunLengths);
			whiteTree.Fill(commonRunLengths);
			blackTree.Fill(blackRunLengths);
			blackTree.Fill(commonRunLengths);
		}
		public static byte[] Decode(PdfCCITTFaxDecodeFilter filter, byte[] data) {
			PdfCCITTFaxDecoder decoder = new PdfCCITTFaxDecoder(filter, data);
			if (decoder.length > 0)
				switch (filter.EncodingScheme) {
					case PdfCCITTFaxEncodingScheme.TwoDimensional:
						decoder.DecodeGroup4();
						break;
					case PdfCCITTFaxEncodingScheme.OneDimensional:
						decoder.DecodeGroup3();
						break;
					default:
						decoder.DecodeGroup3TwoDimensional();
						break;
				}   
			return decoder.result.ToArray();
		}
		static byte FillByte(byte b, int start, int end, bool black) {
			byte mask = (byte)((0xff >> start) & (0xff << (8 - end)));
			return black ? (byte)(b & (0xff ^ mask)) : (byte)(b | mask);
		}
		readonly byte[] data;
		readonly int length;
		readonly bool blackIs1;
		readonly bool alignEncodedBytes;
		readonly int twoDimensionalLineCount;
		readonly int columns;
		readonly int size;
		readonly List<byte> result;
		readonly int lineSize;
		byte[] referenceLine;
		byte[] decodingLine;
		int currentPosition;
		byte currentByte;
		int currentByteOffset = 7;
		bool isBlack;
		int a0;
		int a1;
		int b1;
		int b2;
		bool Completed { get { return size != 0 && result.Count >= size; } }
		PdfCCITTFaxDecoder(PdfCCITTFaxDecodeFilter filter, byte[] data) {
			this.data = data;
			length = data.Length;
			blackIs1 = filter.BlackIs1;
			alignEncodedBytes = filter.EncodedByteAlign;
			twoDimensionalLineCount = filter.TwoDimensionalLineCount;
			columns = filter.Columns;
			size = columns / 8;
			if (columns % 8 != 0)
				size++;
			size *= filter.Rows;
			result = size == 0 ? new List<byte>() : new List<byte>(size);
			lineSize = columns / 8;
			if (columns % 8 > 0)
				lineSize++;
			referenceLine = new byte[lineSize];
			decodingLine = new byte[lineSize];
			if (length > 0)
				currentByte = data[0];
			b1 = columns;
			b2 = columns;
		}
		void MoveNextByte() {
			if (++currentPosition < length)
				currentByte = data[currentPosition];
			currentByteOffset = 7;
		}
		bool ReadBit() {
			if (currentPosition >= length)
				PdfDocumentReader.ThrowIncorrectDataException();
			bool result = ((currentByte >> currentByteOffset) & 1) == 1;
			if (--currentByteOffset < 0)
				MoveNextByte();
			return result;
		}
		CodingMode ReadMode() {
			int wordLength = 1;
			try {
				while (!ReadBit()) 
					wordLength++;
			}
			catch (ArgumentException) {
				return CodingMode.EndOfData;
			}
			switch (wordLength) {
				case 1:
					return CodingMode.Vertical0;
				case 2:
					return ReadBit() ? CodingMode.VerticalRight1 : CodingMode.VerticalLeft1;
				case 3:
					return CodingMode.Horizontal;
				case 4:
					return CodingMode.Pass;
				case 5:
					return ReadBit() ? CodingMode.VerticalRight2 : CodingMode.VerticalLeft2;
				case 6:
					return ReadBit() ? CodingMode.VerticalRight3 : CodingMode.VerticalLeft3;
				default:
					return CodingMode.EndOfData;
			}
		}
		int FindRunningLengthPart(PdfHuffmanTreeBranch branch) {
			try {
				for (int i = 0;; i++) {
					bool nextBit = ReadBit();
					PdfHuffmanTreeNode nextNode = nextBit ? branch.One : branch.Zero;
					if (nextNode == null) {
						if (nextBit)
							PdfDocumentReader.ThrowIncorrectDataException();
						for (; i < 10; i++)
							if (ReadBit())
								PdfDocumentReader.ThrowIncorrectDataException();
						while (!ReadBit());
						return -1;
					}
					PdfHuffmanTreeLeaf leaf = nextNode as PdfHuffmanTreeLeaf;
					if (leaf != null)
						return leaf.RunLength;
					branch = nextNode as PdfHuffmanTreeBranch;
					if (branch == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
			catch {
				return -1;
			}
		}
		int FindRunningLength(PdfHuffmanTreeBranch branch) {
			int code = FindRunningLengthPart(branch);
			if (code < 64)
				return code;
			int result = code;
			while (code == 2560) {
				code = FindRunningLengthPart(branch);
				result += code;
			}
			if (code >= 64) {
				code = FindRunningLengthPart(branch);
				if (code >= 64)
					PdfDocumentReader.ThrowIncorrectDataException();
				result += code;
			}
			return result;
		}
		int FindB(int startPosition, bool isWhite) {
			if (startPosition == columns)
				return columns;
			int result = startPosition;
			int bytePosition = startPosition / 8;
			int byteOffset = startPosition % 8;
			byte b = referenceLine[bytePosition];
			b <<= byteOffset;
			for (int i = 0; i < columns; i++, result++) {
				if (((b & 0x80) == 0x80) == isWhite)
					return result;
				if (++byteOffset == 8) {
					if (++bytePosition == lineSize)
						return columns;
					b = referenceLine[bytePosition];
					byteOffset = 0;
				}
				else
					b <<= 1;
			}
			return columns;
		}
		void NextLine() {
			byte[] temp = referenceLine;
			referenceLine = decodingLine;
			decodingLine = temp;
			isBlack = false;
			a0 = 0;
			b1 = FindB(0, false);
			b2 = FindB(b1, true);
		}
		void ReadEOL() {
			int wordLength = 1;
			while (!ReadBit()) 
				wordLength++;
			if (wordLength != 12)
				PdfDocumentReader.ThrowIncorrectDataException();
			NextLine();
		}
		void FillDecodingLine(int a0, int a1, bool black) {
			if (a0 != 0 || a1 != 0) {
				if (a1 <= a0 || a1 > columns)
					PdfDocumentReader.ThrowIncorrectDataException();
				int startByteIndex = a0 / 8;
				int endByteIndex = a1 / 8;
				if (startByteIndex == endByteIndex)
					decodingLine[startByteIndex] = FillByte(decodingLine[startByteIndex], a0 % 8, a1 % 8, black);
				else {
					decodingLine[startByteIndex] = FillByte(decodingLine[startByteIndex], a0 % 8, 8, black);
					for (int i = startByteIndex + 1; i < endByteIndex; i++)
						decodingLine[i] = black ? (byte)0 : (byte)0xff;
					if (endByteIndex < lineSize)
						decodingLine[endByteIndex] = FillByte(decodingLine[endByteIndex], 0, a1 % 8, black);
				}
			}
		}
		void AccumulateResult() {
			if (blackIs1) {
				for (int i = 0; i < lineSize; i++)
					referenceLine[i] = (byte)(decodingLine[i] ^ 0xff);
				result.AddRange(referenceLine);
			}
			else
				result.AddRange(decodingLine);
			if (alignEncodedBytes && currentByteOffset < 7)
				MoveNextByte();
		}
		bool DecodeGroup3Line() {
			for (;;) {
				PdfHuffmanTreeBranch tree = isBlack ? blackTree : whiteTree;
				int runningLength = FindRunningLength(tree);
				if (runningLength < 0) {
					runningLength = FindRunningLength(tree);
					if (runningLength < 0) {
						for (int i = 0; i < 4; i++)
							if (FindRunningLength(tree) > 0)
								PdfDocumentReader.ThrowIncorrectDataException();
						return false;
					}
				}
				a1 = a0 + runningLength;
				FillDecodingLine(a0, a1, isBlack);
				a0 = a1;
				isBlack = !isBlack;
				if (a0 == columns) {
					AccumulateResult();
					a0 = 0;
					isBlack = false;
					return !Completed;
				}
			}
		}
		int DecodeGroup4() {
			int linesCount = 0;
			for (;;) {
				CodingMode mode = ReadMode();
				switch (mode) {
					case CodingMode.EndOfData:
						if (a0 != 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						return linesCount;
					case CodingMode.Pass:
						FillDecodingLine(a0, b2, isBlack);
						isBlack = !isBlack;
						a0 = b2;
						break;
					case CodingMode.Horizontal:
						PdfHuffmanTreeBranch startingTree;
						PdfHuffmanTreeBranch terminatingTree;
						if (isBlack) {
							startingTree = blackTree;
							terminatingTree = whiteTree;
						}
						else {
							startingTree = whiteTree;
							terminatingTree = blackTree;
						}
						int startingLength = FindRunningLength(startingTree);
						if (startingLength < 0) 
							PdfDocumentReader.ThrowIncorrectDataException();
						int terminatingLength = FindRunningLength(terminatingTree);
						if (terminatingLength < 0) 
							PdfDocumentReader.ThrowIncorrectDataException();
						a1 = a0 + startingLength;
						int a2 = a1 + terminatingLength;
						if (startingLength > 0)
							FillDecodingLine(a0, a1, isBlack);
						if (terminatingLength > 0)
							FillDecodingLine(a1, a2, !isBlack);
						isBlack = !isBlack;
						a0 = a2;
						break;
					default:
						switch (mode) {
							case CodingMode.Vertical0:
								a1 = b1;
								break;
							case CodingMode.VerticalRight1:
								a1 = b1 + 1;
								break;
							case CodingMode.VerticalRight2:
								a1 = b1 + 2;
								break;
							case CodingMode.VerticalRight3:
								a1 = b1 + 3;
								break;
							case CodingMode.VerticalLeft1:
								a1 = b1 - 1;
								break;
							case CodingMode.VerticalLeft2:
								a1 = b1 - 2;
								break;
							case CodingMode.VerticalLeft3:
								a1 = b1 - 3;
								break;
							}
						FillDecodingLine(a0, a1, isBlack);
						a0 = a1;
						break;
				}
				if (a0 == columns) {
					AccumulateResult();
					linesCount++;
					if (Completed)
						return linesCount;
					NextLine();
				}
				else {
					isBlack = !isBlack;
					b1 = FindB(FindB(a0, !isBlack), isBlack);
					b2 = FindB(b1, !isBlack);
				}
			}
		}
		void DecodeGroup3() {
			while (DecodeGroup3Line());
		}
		void DecodeGroup3TwoDimensional() {
			ReadEOL();
			if (!ReadBit())
				PdfDocumentReader.ThrowIncorrectDataException();
			if (DecodeGroup3Line()) {
				ReadEOL();
				int remainTwoDimensionalLineCount = twoDimensionalLineCount;
				while (!Completed)
					if (ReadBit()) {
						if (remainTwoDimensionalLineCount > 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						if (!DecodeGroup3Line())
							break;
						remainTwoDimensionalLineCount = twoDimensionalLineCount;
						ReadEOL();
					}
					else
						remainTwoDimensionalLineCount = Math.Max(0, remainTwoDimensionalLineCount - DecodeGroup4());
				if (remainTwoDimensionalLineCount > 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
	}
}
