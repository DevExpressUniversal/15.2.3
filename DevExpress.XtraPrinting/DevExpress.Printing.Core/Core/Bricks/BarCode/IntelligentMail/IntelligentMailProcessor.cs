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
using System.Globalization;
#if WINRT || WP
using Math = System.MathEx;
#endif
namespace DevExpress.XtraPrinting.BarCode.Native {
	class IntelligentMailProcessor: IPatternProcessor {
		#region Types
		struct Codeword {
			public int Divider { get; set; }
			public int Character { get; set; }
			public int NewCharacter { get; set; }
		}
		#endregion
		#region Fields & Properties
		Codeword[] codewords = new Codeword[CodewordCount];
		ArrayList pattern;
		#endregion
		#region Static and Constants
		const short BitsInShort = sizeof(short) * BitsInByte;
		const short BitsInByte = 8;
		const int Table2Of13Size = 78;
		const int Table5Of13Size = 1287;
		const int TablesSize = Table2Of13Size + Table5Of13Size;
		const short CharacterBitCount = 13;
		const short CodewordCount = 10;
		const short MaxCharacter = 0x1FFF;
		static int[] table2Of13;
		static int[] table5Of13;
		static Dictionary<int, long> correctionValuesByLength = new Dictionary<int, long> {
			{ 5, 1 },
			{ 9, 100001 },
			{ 11, 1000100001 }
		};
		static readonly int[] barTopCharIndexes = { 4, 0, 2, 6, 3, 5, 1, 9, 8, 7, 1, 2, 0, 6, 4, 8, 2, 9, 5, 3, 0, 1, 3, 7, 4, 6, 8, 9, 2, 0, 5, 1, 9, 4, 3, 8, 6, 7, 1, 2, 4, 3, 9, 5, 7, 8, 3, 0, 2, 1, 4, 0, 9, 1, 7, 0, 2, 4, 6, 3, 7, 1, 9, 5, 8 };
		static readonly int[] barBottomCharIndexes = { 7, 1, 9, 5, 8, 0, 2, 4, 6, 3, 5, 8, 9, 7, 3, 0, 6, 1, 7, 4, 6, 8, 9, 2, 5, 1, 7, 5, 4, 3, 8, 7, 6, 0, 2, 5, 4, 9, 3, 0, 1, 6, 8, 2, 0, 4, 5, 9, 6, 7, 5, 2, 6, 3, 8, 5, 1, 9, 8, 7, 4, 0, 2, 6, 3 };
		static readonly int[] barTopCharShifts = { 3, 0, 8, 11, 1, 12, 8, 11, 10, 6, 4, 12, 2, 7, 9, 6, 7, 9, 2, 8, 4, 0, 12, 7, 10, 9, 0, 7, 10, 5, 7, 9, 6, 8, 2, 12, 1, 4, 2, 0, 1, 5, 4, 6, 12, 1, 0, 9, 4, 7, 5, 10, 2, 6, 9, 11, 2, 12, 6, 7, 5, 11, 0, 3, 2 };
		static readonly int[] barBottomCharShifts = { 2, 10, 12, 5, 9, 1, 5, 4, 3, 9, 11, 5, 10, 1, 6, 3, 4, 1, 10, 0, 2, 11, 8, 6, 1, 12, 3, 8, 6, 4, 4, 11, 0, 6, 1, 9, 11, 5, 3, 7, 3, 10, 7, 11, 8, 2, 10, 3, 5, 8, 0, 3, 12, 11, 8, 4, 5, 1, 3, 0, 7, 12, 9, 8, 10 };
		static IntelligentMailProcessor() {
			table2Of13 = new int[Table2Of13Size];
			InitializeNOf13Table(table2Of13, 2);
			table5Of13 = new int[Table5Of13Size];
			InitializeNOf13Table(table5Of13, 5);
		}
		static int[] MakeBinaryData(string source, out string binaryDataString) {
			long routingCodeConversion = MakeRoutingCodeConversion(source);
			routingCodeConversion = routingCodeConversion * 10 + BarCodeGeneratorBase.Char2Int(source[0]);
			routingCodeConversion = routingCodeConversion * 5 + BarCodeGeneratorBase.Char2Int(source[1]);
			binaryDataString = routingCodeConversion.ToString() + GetTrackingCodeWithoutBarcodeId(source);
			int[] binaryData = StringBase10ToArrayIntBase32(binaryDataString);
			return binaryData;
		}
		static int[] StringBase10ToArrayIntBase32(string source) {
			const short BinaryDataByteCount = 13;
			int[] byteArray = new int[BinaryDataByteCount];
			long remainder;
			for(int iteratoryteArray = BinaryDataByteCount - 1; iteratoryteArray >= 0; iteratoryteArray--) {
				source = DivideString(source, 256, out remainder);
				byteArray[iteratoryteArray] = (int)remainder;
			}
			return byteArray;
		}
		static string DivideString(string divident, long divider, out long reminder) {
			string quotient = "";
			long tempDivident = 0;
			long tempReminder = 0;
			for(int i = 0; i < divident.Length; i++) {
				tempDivident *= 10;
				tempDivident += long.Parse(divident.Substring(i, 1));
				if(tempDivident >= divider) {
					quotient += Math.DivRem(tempDivident, divider, out tempReminder).ToString();
					tempDivident = tempReminder;
				} else {
					if(i == divident.Length - 1) { quotient += "0"; }
					if(i != divident.Length - 1 && quotient != "") { quotient += "0"; }
				}
			}
			reminder = tempDivident;
			return quotient;
		}
		static long MakeRoutingCodeConversion(string source) {
			long correctionValue;
			string zip = GetZip(source);
			correctionValuesByLength.TryGetValue(zip.Length, out correctionValue);
			long convertedZip;
			if(long.TryParse(zip, out convertedZip))
				convertedZip += correctionValue;
			return convertedZip;
		}
		static void InitializeNOf13Table(int[] table, int n) {
			const short CharacterBitOffset = BitsInShort - CharacterBitCount;
			int index = 0;
			int reverseIndex = table.Length - 1;
			for(short character = 0; character <= MaxCharacter; character++) {
				int bitOneCounter = 0;
				for(int characterBitIndex = 0; characterBitIndex < CharacterBitCount; characterBitIndex++)
					if((character & 1 << characterBitIndex) != 0)
						bitOneCounter++;
				if(bitOneCounter == n) {
					int reverseValue = MathReverse(character) >> CharacterBitOffset;
					if(reverseValue >= character) {
						if(character == reverseValue) {
							table[reverseIndex] = character;
							reverseIndex--;
						} else {
							table[index] = character;
							index++;
							table[index] = reverseValue;
							index++;
						}
					}
				}
			}
			System.Diagnostics.Debug.Assert(index == reverseIndex + 1);
		}
		static int CalculateFcs(int[] binaryDataFcs) {
			const ushort GeneratorPolinomial = 0xF35;
			const ushort FrameCheckSequenceDefault = 0x7FF;
			const ushort EleventhBitIndex = 0x400;
			ushort frameCheckSequence = FrameCheckSequenceDefault;
			ushort data = (ushort)(binaryDataFcs[0] << 5);
			for(short bit = 2; bit < BitsInByte; bit++) {
				if(((frameCheckSequence ^ data) & EleventhBitIndex) != 0)
					frameCheckSequence = (ushort)((frameCheckSequence << 1) ^ GeneratorPolinomial);
				else
					frameCheckSequence <<= 1;
				frameCheckSequence &= FrameCheckSequenceDefault;
				data <<= 1;
			}
			for(int byteIndex = 1; byteIndex < binaryDataFcs.Length; byteIndex++) {
				data = (ushort)(binaryDataFcs[byteIndex] << 3);
				for(short bit = 0; bit < BitsInByte; bit++) {
					if(((frameCheckSequence ^ data) & EleventhBitIndex) != 0)
						frameCheckSequence = (ushort)((frameCheckSequence << 1) ^ GeneratorPolinomial);
					else
						frameCheckSequence <<= 1;
					frameCheckSequence &= FrameCheckSequenceDefault;
					data <<= 1;
				}
			}
			return frameCheckSequence;
		}
		static int MathReverse(int value) {
			int result = 0;
			for(short k = 0; k < BitsInShort; k++) {
				result <<= 1;
				result |= (value & 1);
				value >>= 1;
			}
			return result;
		}
		static string GetBarcodeId(string source) {
			return source.Substring(0, 2);
		}
		static string GetTrackingCodeWithoutBarcodeId(string source) {
			return source.Substring(2, 18);
		}
		static string GetZip(string source) {
			return source.Substring(20);
		}
		#endregion
		#region Methods
		public ArrayList CalculatePattern(string source) {
			const short BarCount = 65;
			const int DividerA = 659;
			const int DividerJ = 636;
			const int CorrectionA = 659;
			const short FcsLength = 11;
			int[] tableAscender = new int[BarCount];
			int[] tableDescender = new int[BarCount];
			string binaryDataString = "";
			int[] binaryData = MakeBinaryData(source, out binaryDataString);
			int crcFcs = CalculateFcs(binaryData);
			for(short i = 1; i < CodewordCount - 1; i++) {
				codewords[i].Divider = TablesSize;
				codewords[i].Character = 0;
			}
			codewords[0].Divider = DividerA;
			codewords[CodewordCount - 1].Divider = DividerJ;
			ConvertBinaryDataToCodewords(binaryDataString);
			codewords[CodewordCount - 1].Character *= 2;
			if(crcFcs >> (FcsLength - 1) != 0)
				codewords[0].Character += CorrectionA;
			for(short i = 0; i < CodewordCount; i++) {
				if(codewords[i].Character >= TablesSize)
					return null;
				codewords[i].NewCharacter = codewords[i].Character >= Table5Of13Size
					? table2Of13[codewords[i].Character - Table5Of13Size]
					: table5Of13[codewords[i].Character];
			}
			for(short i = 0; i < 10; i++)
				if((crcFcs & 1 << i) != 0)
					codewords[i].NewCharacter = ~codewords[i].NewCharacter & MaxCharacter;
			for(short i = 0; i < BarCount; i++) {
				tableAscender[i] = codewords[barTopCharIndexes[i]].NewCharacter >> barTopCharShifts[i] & 1;
				tableDescender[i] = codewords[barBottomCharIndexes[i]].NewCharacter >> barBottomCharShifts[i] & 1;
			}
			ArrayList pattern = new ArrayList();
			for(int i = 0; i < BarCount; i++) {
				char symbol = new char();
				if(tableAscender[i] == 0)
					symbol = tableDescender[i] == 0 ? 'T' : 'D';
				else
					symbol = tableDescender[i] == 0 ? 'A' : 'F';
				pattern.Add(symbol);
			}
			return pattern;
		}
		void ConvertBinaryDataToCodewords(string binaryDataString) {
			long remainder;
			int i;
			for(i = CodewordCount - 1; i > 0; i--) {
				binaryDataString = DivideString(binaryDataString, codewords[i].Divider, out remainder);
				codewords[i].Character = (int)remainder;
			}
			codewords[i].Character = int.Parse(binaryDataString);
		}
		#endregion
		#region IPatternProcessor Members
		ArrayList IPatternProcessor.Pattern { get { return pattern; } }
		void IPatternProcessor.RefreshPattern(object data) {
		   pattern = CalculatePattern(data as string);
		}
		void IPatternProcessor.Assign(IPatternProcessor source) {
		}
		#endregion
	}
}
