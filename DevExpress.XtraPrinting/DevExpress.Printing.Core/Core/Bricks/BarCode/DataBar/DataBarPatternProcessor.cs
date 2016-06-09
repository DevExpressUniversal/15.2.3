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
using DevExpress.XtraPrinting.BarCode;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.IO;
namespace DevExpress.XtraPrinting.BarCode {
	public struct StructSymbol {
		public int MinValue;
		public int MaxValue;
		public int OddModules;
		public int EvenModules;
		public int OddWidestModule;
		public int EvenWidestModules;
		public int OddNumberCombinations;
		public int EvenNumberCombinations;
		public StructSymbol(int minValue, int maxValue, int oddModules, int evenModules, int oddWidestModule, int evenWidestModules, int oddNumberCombinations, int evenNumberCombinations) {
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.OddModules = oddModules;
			this.EvenModules = evenModules;
			this.OddWidestModule = oddWidestModule;
			this.EvenWidestModules = evenWidestModules;
			this.OddNumberCombinations = oddNumberCombinations;
			this.EvenNumberCombinations = evenNumberCombinations;
		}
	}
	public abstract class DataBarPatternProcessor {
		public static DataBarPatternProcessor CreateInstance(DataBarType type) {
			if(type == DataBarType.Expanded || type == DataBarType.ExpandedStacked)
				return new DataBarExpandedPatternProcessor(type);
			if(type == DataBarType.Limited)
				return new DataBarLimitedPatternProcessor();
			if(type == DataBarType.Omnidirectional || type == DataBarType.StackedOmnidirectional || type == DataBarType.Truncated || type == DataBarType.Stacked)
				return new DataBarBasePatternProcessor(type);
			return null;
		}
		protected const int countIndentForSeparation = 4;
		protected const int fixedHeightSeparation = 1;
		protected static bool[] indentForSeparation = { false, false, false, false };
		protected static int[] generalPatternLimiter = { 1, 1 };
		ArrayList pattern = new ArrayList();
		const string defaultFNC1Subst = "#";
		protected const char fnc1Char = (char)232;
		DataBarType type = DataBarType.Omnidirectional;
		protected int segmentPairsInRow = 10;
		protected string fnc1Subst = defaultFNC1Subst;
		protected DataBarPatternProcessor() {
		}
		public int SegmentsInRow {
			get { return segmentPairsInRow * 2; }
			set {
				if(value % 2 == 1) value--;
				if(value < 2) value = 2;
				if(value > 20) value = 20;
				segmentPairsInRow = value / 2;
			}
		}
		public DataBarType Type {
			get { return type; }
			set { type = value; }
		}
		public string FNC1Substitute {
			get { return fnc1Subst; }
			set { fnc1Subst = value; }
		}
		public abstract ArrayList GetPattern(string text);
		public virtual bool IsValidTextFormat(string text) {
			if(!System.Text.RegularExpressions.Regex.Match(text, @"^\d+$").Success || text.Length > 14)
				return false;
			return true;
		}
		public void Assign(DataBarPatternProcessor source) {
			DataBarPatternProcessor sourceProcessor = source as DataBarPatternProcessor;
			this.segmentPairsInRow = sourceProcessor.segmentPairsInRow;
			this.FNC1Substitute = sourceProcessor.fnc1Subst;
		}
		protected int[] GetRSSwidths(int val, int n, int elements, int maxWidth, Boolean noNarrow) {
			int[] widths = new int[elements];
			int bar;
			int narrowMask = 0;
			for(bar = 0; bar < elements - 1; bar++) {
				narrowMask |= (1 << bar);
				int elmWidth = 1;
				int subVal;
				while(true) {
					subVal = Combins(n - elmWidth - 1, elements - bar - 2);
					if(noNarrow && (narrowMask == 0) &&
						(n - elmWidth - (elements - bar - 1) >= elements - bar - 1)) {
						subVal -= Combins(n - elmWidth - (elements - bar), elements - bar - 2);
					}
					if(elements - bar - 1 > 1) {
						int lessVal = 0;
						for(int mxwElement = n - elmWidth - (elements - bar - 2);
							 mxwElement > maxWidth;
							 mxwElement--) {
							lessVal += Combins(n - elmWidth - mxwElement - 1, elements - bar - 3);
						}
						subVal -= lessVal * (elements - 1 - bar);
					} else if(n - elmWidth > maxWidth) {
						subVal--;
					}
					val -= subVal;
					if(val < 0) break;
					elmWidth++;
					narrowMask &= ~(1 << bar);
				}
				val += subVal;
				n -= elmWidth;
				widths[bar] = elmWidth;
			}
			widths[bar] = n;
			return widths;
		}
		int Combins(int n, int r) {
			int i, j;
			int maxDenom, minDenom;
			int val;
			if(n - r > r) {
				minDenom = r;
				maxDenom = n - r;
			} else {
				minDenom = n - r;
				maxDenom = r;
			}
			val = 1;
			j = 1;
			for(i = n; i > maxDenom; i--) {
				val *= i;
				if(j <= minDenom) {
					val /= j;
					j++;
				}
			}
			for(; j <= minDenom; j++) {
				val /= j;
			}
			return val;
		}
		protected int[] ComposeOddEven(int[] oddPart, int[] evenPart, int count) {
			int[] result = new int[2 * count];
			for(int i = 0; i < count; i++) {
				result[2 * i] = oddPart[i];
				result[2 * i + 1] = evenPart[i];
			}
			return result;
		}
		protected List<PatternElement> ConvertBooleanArrayToPattern(List<List<bool>> array) {
			int count = array.Count;
			List<PatternElement> pattern = new List<PatternElement>();
			for(int i = 0; i < count; i++) {
				List<int> patternPart = new List<int>();
				int countModules = 1;
				for(int j = 1; j < array[i].Count; j++) {
					if(array[i][j] == array[i][j - 1]) {
						countModules++;
					} else {
						patternPart.Add(countModules);
						countModules = 1;
					}
				}
				PatternElement partStructPattern = new PatternElement(fixedHeightSeparation, patternPart, false);
				pattern.Add(partStructPattern);
			}
			return pattern;
		}
		protected List<List<bool>> ConvertPatternToBooleanArray(List<PatternElement> pattern) {
			int count = pattern.Count;
			List<List<bool>> result = new List<List<bool>>();
			for(int i = 0; i < count; i++) {
				PatternElement currentPattern = (PatternElement)pattern[i];
				List<bool> lineResult = new List<bool>();
				Boolean currentColorBar = currentPattern.startBarBlack;
				for(int j = 0; j < currentPattern.pattern.Count; j++) {
					for(int k = 0; k < (int)currentPattern.pattern[j]; k++) {
						lineResult.Add(currentColorBar);
					}
					currentColorBar = !currentColorBar;
				}
				result.Add(lineResult);
			}
			return result;
		}
		protected int[] SubPattern(int data, StructSymbol symbol, int elementsPair, bool oddFlag) {
			int number = data - symbol.MinValue;
			int odd = number / symbol.EvenNumberCombinations;
			int even = number % symbol.EvenNumberCombinations;
			int[] oddPattern = GetRSSwidths(odd, symbol.OddModules, elementsPair, symbol.OddWidestModule, oddFlag);
			int[] evenPattern = GetRSSwidths(even, symbol.EvenModules, elementsPair, symbol.EvenWidestModules, !oddFlag);
			return ComposeOddEven(oddPattern, evenPattern, elementsPair);
		}
		protected int[] SubPatternInverted(int data, StructSymbol symbol, int elementsPair, bool oddFlag) {
			int number = data - symbol.MinValue;
			int odd = number % symbol.OddNumberCombinations;
			int even = number / symbol.OddNumberCombinations;
			int[] oddPattern = GetRSSwidths(odd, symbol.OddModules, elementsPair, symbol.OddWidestModule, oddFlag);
			int[] evenPattern = GetRSSwidths(even, symbol.EvenModules, elementsPair, symbol.EvenWidestModules, !oddFlag);
			return ComposeOddEven(oddPattern, evenPattern, elementsPair);
		}
	}
	public class DataBarBasePatternProcessor : DataBarPatternProcessor {
		const int elementsPair = 4;
		const long combSignSymbol = 4537077;
		const int combSignSymbolIn = 1597;
		const int checkSumModifier = 79;
		#region predefined static values
		static int[,] widthsChecksumPattern = new int[9, 5] {
			{3,8,2,1,1}, {3,5,5,1,1}, {3,3,7,1,1},
			{3,1,9,1,1}, {2,7,4,1,1}, {2,5,6,1,1}, 
			{2,3,8,1,1}, {1,5,7,1,1}, {1,3,9,1,1}};
		static int[,] weightChecksumElem = new int[4, 8] {
			{1, 3, 9, 27, 2, 6, 18 , 54},
			{4, 12, 36, 29, 8, 24, 72, 58},
			{16, 48, 65, 37, 32, 17, 51, 74},
			{64, 34, 23, 69, 49, 68, 46, 59}};
		static StructSymbol[] symbolOut = new StructSymbol[5] {
			new StructSymbol(0,	160,  12, 4,  8,  1, 161, 1),
			new StructSymbol(161,  960,  10, 6,  6,  3, 80,  10),
			new StructSymbol(961,  2014, 8,  8,  4,  5, 31,  34),
			new StructSymbol(2015, 2714, 6,  10, 3,  6, 10,  70),
			new StructSymbol(2715, 2840, 4,  12, 1,  8, 1,   126)};
		static StructSymbol[] symbolIn = new StructSymbol[4] {
			new StructSymbol(0,	335,  5,  10,  2,  7, 4,  84),
			new StructSymbol(336,  1035, 7,  8,   4,  5, 20, 35),
			new StructSymbol(1036, 1515, 9,  6,   6,  3, 48, 10),
			new StructSymbol(1516, 1596, 11, 4,   8,  1, 81, 1)};
		#endregion
		public DataBarBasePatternProcessor(DataBarType type) {
			this.Type = type;
		}
		int[] TextToData(string text) {
			long symbol;
			if(!long.TryParse(text.Substring(0, text.Length - 1), out symbol)) symbol = 0;
			long symbolLeftPart = symbol / combSignSymbol;
			long symbolRightPart = symbol % combSignSymbol;
			long datan = symbolLeftPart / combSignSymbolIn;
			int[] result = new int[4] {
				(int)symbolLeftPart / combSignSymbolIn, 
				(int)symbolLeftPart % combSignSymbolIn, 
				(int)symbolRightPart / combSignSymbolIn,
				(int)symbolRightPart % combSignSymbolIn};
			return result;
		}
		int[] GetDataType(int[] data) {
			int[] result = new int[4];
			for(int i = 0; i < 4; i++) {
				for(int j = 0; j < 5; j++) {
					if(i % 2 == 0) {
						if(data[i] >= symbolOut[j].MinValue && data[i] <= symbolOut[j].MaxValue) {
							result[i] = j;
							break;
						}
					} else {
						if(data[i] >= symbolIn[j].MinValue && data[i] <= symbolIn[j].MaxValue) {
							result[i] = j;
							break;
						}
					}
				}
			}
			return result;
		}
		List<int> MakeFinalPattern(int[][] signPattern, int[] checkSumLeftPartPattern, int[] checkSumRightPartPattern) {
			List<int> result = new List<int>(46);
			result.AddRange(generalPatternLimiter);
			result.AddRange(signPattern[0]);
			result.AddRange(checkSumLeftPartPattern);
			Array.Reverse(signPattern[1]);
			result.AddRange(signPattern[1]);
			result.AddRange(signPattern[3]);
			Array.Reverse(checkSumRightPartPattern);
			result.AddRange(checkSumRightPartPattern);
			Array.Reverse(signPattern[2]);
			result.AddRange(signPattern[2]);
			result.AddRange(generalPatternLimiter);
			return result;
		}
		public override ArrayList GetPattern(string text) {
			int checkSum = 0;
			int checkSumLeftPart, checkSumRightPart;
			int[] checkSumLeftPartPattern = new int[5];
			int[] checkSumRightPartPattern = new int[5];
			int[][] signPattern = { new int[elementsPair * 2], new int[elementsPair * 2], new int[elementsPair * 2], new int[elementsPair * 2] };
			int[] data = TextToData(text);
			int[] dataType = GetDataType(data);
			for(int i = 0; i < 4; i++) {
				if(i % 2 == 0) {
					StructSymbol symbol = symbolOut[dataType[i]];
					signPattern[i] = SubPattern(data[i], symbol, elementsPair, false);
				} else {
					StructSymbol symbol = symbolIn[dataType[i]];
					signPattern[i] = SubPatternInverted(data[i], symbol, elementsPair, true);
				}
			}
			for(int i = 0; i < elementsPair; i++) {
				for(int j = 0; j < 2 * elementsPair; j++) {
					checkSum += signPattern[i][j] * weightChecksumElem[i, j];
				}
			}
			checkSum = checkSum % checkSumModifier;
			if(checkSum >= 8) { checkSum += 1; }
			if(checkSum >= 72) { checkSum += 1; }
			checkSumLeftPart = checkSum / 9;
			checkSumRightPart = checkSum % 9;
			for(int i = 0; i < 5; i++) {
				checkSumLeftPartPattern[i] = widthsChecksumPattern[checkSumLeftPart, i];
				checkSumRightPartPattern[i] = widthsChecksumPattern[checkSumRightPart, i];
			}
			List<int> pattern = MakeFinalPattern(signPattern, checkSumLeftPartPattern, checkSumRightPartPattern);
			List<PatternElement> result = ConvertPatternToNewFormat(pattern);
			return new ArrayList(result);
		}
		List<PatternElement> ConvertPatternToNewFormat(List<int> pattern) {
			List<PatternElement> newFormatResult = new List<PatternElement>();
			if(Type == DataBarType.Omnidirectional || Type == DataBarType.Truncated) {
				int fixedHeight = Type == DataBarType.Omnidirectional ? 33 : 13;
				PatternElement result = new PatternElement(fixedHeight, pattern, false);
				newFormatResult.Add(result);
			} else {
				if(Type == DataBarType.Stacked) {
					newFormatResult = SplitPattern(pattern, 5, 7);
					newFormatResult = AddStackedPattern(newFormatResult);
				} else {
					if(Type == DataBarType.StackedOmnidirectional) {
						newFormatResult = SplitPattern(pattern, 33, 33);
						newFormatResult = AddStackedOmnidirectionPattern(newFormatResult);
					}
				}
			}
			return newFormatResult;
		}
		List<PatternElement> SplitPattern(List<int> pattern, int firstHight, int secondHight) {
			int half = pattern.Count / 2;
			List<int> pattern1 = new List<int>(pattern.GetRange(0, half));
			List<int> pattern2 = new List<int>(pattern.GetRange(half, half));
			pattern1.AddRange(generalPatternLimiter);
			pattern2.InsertRange(0, generalPatternLimiter);
			List<PatternElement> result = new List<PatternElement>(2);
			PatternElement part1 = new PatternElement(firstHight, pattern1, false);
			PatternElement part2 = new PatternElement(secondHight, pattern2, true);
			result.Add(part1);
			result.Add(part2);
			return result;
		}
		List<PatternElement> AddStackedPattern(List<PatternElement> pattern) {
			Boolean value = false;
			List<List<bool>> booleanPattern = ConvertPatternToBooleanArray(pattern);
			int countModulesInPattern = booleanPattern[0].Count;
			List<List<bool>> resultBooleanPattern = new List<List<bool>>();
			for(int i = 0; i < booleanPattern.Count - 1; i++) {
				List<bool> partBooleanPattern = new List<bool>();
				partBooleanPattern.AddRange(indentForSeparation);
				for(int j = indentForSeparation.Length; j < countModulesInPattern - indentForSeparation.Length; j++) {
					if(booleanPattern[i][j] != booleanPattern[i + 1][j]) {
						value = !partBooleanPattern[j - 1];
					} else {
						value = !booleanPattern[i][j];
					}
					partBooleanPattern.Add(value);
				}
				partBooleanPattern.AddRange(indentForSeparation);
				resultBooleanPattern.Add(partBooleanPattern);
			}
			List<PatternElement> separationPattern = ConvertBooleanArrayToPattern(resultBooleanPattern);
			pattern.InsertRange(1, separationPattern);
			return pattern;
		}
		List<PatternElement> AddStackedOmnidirectionPattern(List<PatternElement> pattern) {
			List<List<bool>> booleanPattern = ConvertPatternToBooleanArray(pattern);
			int countModulesInPattern = booleanPattern[0].Count;
			List<List<bool>> resultBooleanPattern = new List<List<bool>>();
			for(int i = 0; i < 3; i++) {
				resultBooleanPattern.Add(new List<bool>(indentForSeparation));
			}
			for(int i = 0; i < booleanPattern.Count - 1; i++) {
				bool midValue = false;
				bool topValue = true;
				bool botValue = true;
				for(int j = countIndentForSeparation; j < countModulesInPattern - countIndentForSeparation; j++) {
					if(j > 17 && j < 31) {
						if(booleanPattern[0][j] == true) {
							resultBooleanPattern[0].Add(false);
						} else {
							resultBooleanPattern[0].Add(topValue);
							topValue = !topValue;
						}
					} else {
						resultBooleanPattern[0].Add(!booleanPattern[0][j]);
					}
					resultBooleanPattern[1].Add(midValue);
					midValue = !midValue;
					if(j > 18 && j < 32) {
						if(booleanPattern[1][j] == true) {
							resultBooleanPattern[2].Add(false);
						} else {
							resultBooleanPattern[2].Add(botValue);
							botValue = !botValue;
						}
					} else {
						resultBooleanPattern[2].Add(!booleanPattern[1][j]);
					}
				}
			}
			List<bool> testBoolPattern = resultBooleanPattern[2].GetRange(19, 13);
			if(SpecailKindPattern(testBoolPattern)) {
				resultBooleanPattern[2][28] = false;
				resultBooleanPattern[2][29] = true;
			}
			for(int i = 0; i < 3; i++) {
				resultBooleanPattern[i].AddRange(indentForSeparation);
			}
			List<PatternElement> separationPattern = ConvertBooleanArrayToPattern(resultBooleanPattern);
			pattern.InsertRange(1, separationPattern);
			return pattern;
		}
		bool SpecailKindPattern(List<bool> testBoolPattern) {
			for(int i = 0; i < 9; i++) {
				if(testBoolPattern[i] != false) return false;
			}
			if(testBoolPattern[9] != true) return false;
			for(int i = 10; i < 13; i++) {
				if(testBoolPattern[i] != false) return false;
			}
			return true;
		}
	}
	public class DataBarLimitedPatternProcessor : DataBarPatternProcessor {
		const int elementsPairLimited = 7;
		const long combSignSymbolLimited = 2013571;
		const int checkSumModifierLimited = 89;
		#region predefined static values
		static int[] rightPatternLimiter = { 1, 1, 5 };
		static int[] sequenceNumber = new int[89] {
			0,   1,   2,   3,   4,   5,   6,   7,   8,   9,
			10,  11,  12,  13,  14,  15,  16,  17,  18,  19,
			20,  21,  22,  23,  24,  25,  26,  27,  28,  29,
			30,  31,  32,  33,  34,  35,  36,  37,  38,  39, 
			40,  41,  42,  43,  45,  52,  57,  63,  64,  65,
			66,  73,  74,  75,  76,  77,  78,  79,  82,  126,
			127, 128, 129, 130, 132, 141, 142, 143, 144, 145,
			146, 210, 211, 212, 213, 214, 215, 216, 217, 220,
			316, 317, 318, 319, 320, 322, 323, 326, 337};
		static StructSymbol[] symbolLimited = new StructSymbol[7] {
			new StructSymbol(0,	   183063,  17,  9, 6, 3, 6538,  28),
			new StructSymbol(183064,  820063,  13, 13, 5, 4, 875,   728),
			new StructSymbol(820064,  1000775, 9,  17, 3, 6, 28,	6454),
			new StructSymbol(1000776, 1491020, 15, 11, 5, 4, 2415,  203),
			new StructSymbol(1491021, 1979844, 11, 15, 4, 5, 203,   2408),
			new StructSymbol(1979845, 1996938, 19, 7,  8, 1, 17094, 1),
			new StructSymbol(1996939, 2013570, 7,  19, 1, 8, 1,	 16632)};
		static StructSymbol symbolCheckSum = new StructSymbol(0, 0, 8, 8, 3, 3, 21, 21);
		static int[,] weightChecksumElemLimited = new int[2, 14] {
			{1, 3, 9, 27, 81, 65, 17, 51, 64, 14, 42, 37, 22, 66},
			{20, 60, 2, 6, 18, 54, 73, 41, 34, 13, 39, 28, 84, 74}};
		#endregion
		public DataBarLimitedPatternProcessor() {
		}
		public override bool IsValidTextFormat(string text) {
			return base.IsValidTextFormat(text) && (text.Length == 14 ? text[0] == '0' || text[0] == '1' : true) ? true : false;   
		}
		long[] TextToData(string text) {
			long symbol;
			if(!long.TryParse(text.Substring(0, text.Length - 1), out symbol)) symbol = 0;
			long[] data = new long[2] {
				symbol / combSignSymbolLimited,
				symbol % combSignSymbolLimited};
			return data;
		}
		int[] GetDataType(long[] data) {
			int[] dataType = new int[2];
			for(int i = 0; i < 2; i++) {
				for(int j = 0; j < 7; j++) {
					if(data[i] >= symbolLimited[j].MinValue && data[i] <= symbolLimited[j].MaxValue) {
						dataType[i] = j;
						break;
					}
				}
			}
			return dataType;
		}
		public override ArrayList GetPattern(string text) {
			List<int> pattern = new List<int>(47);
			int checkSum = 0;
			int[] checkSumPattern = new int[14];
			int[] endPartCheckSum = new int[2] { 1, 1 };
			int[][] signPattern = { new int[elementsPairLimited * 2], new int[elementsPairLimited * 2] };
			long[] data = TextToData(text);
			int[] dataType = GetDataType(data);
			for(int i = 0; i < 2; i++) {
				StructSymbol symbol = symbolLimited[dataType[i]];
				signPattern[i] = SubPattern((int)data[i], symbol, elementsPairLimited, false);
			}
			for(int i = 0; i < 2; i++) {
				for(int j = 0; j < 2 * elementsPairLimited; j++) {
					checkSum += signPattern[i][j] * weightChecksumElemLimited[i, j];
				}
			}
			checkSum = checkSum % checkSumModifierLimited;
			checkSumPattern[12] = 1;
			checkSumPattern[13] = 1;
			int[] partCheckSumPattern = SubPattern(sequenceNumber[checkSum], symbolCheckSum, 6, false);
			partCheckSumPattern.CopyTo(checkSumPattern, 0);
			pattern.AddRange(generalPatternLimiter);
			int[] arrayWithoutLimiter = new int[42];
			for(int i = 0; i < elementsPairLimited * 2; i++) {
				arrayWithoutLimiter[i] = signPattern[0][i];
				arrayWithoutLimiter[28 + i] = signPattern[1][i];
				arrayWithoutLimiter[14 + i] = checkSumPattern[i];
			}
			pattern.AddRange(arrayWithoutLimiter);
			pattern.AddRange(rightPatternLimiter);
			PatternElement resultPattern = new PatternElement(10, pattern, false);
			List<PatternElement> newFormatResult = new List<PatternElement>();
			newFormatResult.Add(resultPattern);
			return new ArrayList(newFormatResult);
		}
	}
	public class DataBarExpandedPatternProcessor : DataBarPatternProcessor {
		public enum CodingType {
			ISOIEC = 0,
			ALPHA_OR_ISO = 1,
			ANY_ENC = 2,
			NUMERIC = 3,
			ALPHA = 4,
		}
		public class BlockData {
			public CodingType Type;
			public int Length;
			public BlockData() {
			}
			public BlockData(BlockData blockData) {
				this.Type = blockData.Type;
				this.Length = blockData.Length;
			}
			public BlockData(CodingType type, int length) {
				this.Type = type;
				this.Length = length;
			}
		}
		const int elementsPairExpanded = 4;
		const int checkSumModifierExpanded = 211;
		const int barsInSegmentPair = 21;
		#region predefined static values
		static int[] numbersAI = new int[23] { 00, 01, 02, 03, 04, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 23, 31, 32, 33, 34, 35, 36, 41 };
		static int[] specialPatternLimiter = { 2, 1 };
		static bool[] boolFinderPatternType1 = { true, true, true, true, true, true, true, true, true, true, true, true, true, false, false };
		static bool[] boolFinderPatternType2 = { false, false, true, true, true, true, true, true, true, true, true, true, true, true, true };
		static bool[] boolPatternLimiter = { false, false };
		static int[,] weightChecksumElemExpanded = new int[23, 8] {
			{1, 3, 9, 27, 81, 32, 96, 77},
			{20, 60, 180, 118, 143, 7, 21, 63},
			{189, 145, 13, 39, 117, 140, 209, 205},
			{193, 157, 49, 147, 19, 57, 171, 91},
			{62, 186, 136, 197, 169, 85, 44, 132},
			{185, 133, 188, 142, 4, 12, 36, 108},
			{113, 128, 173, 97, 80, 29, 87, 50},
			{150, 28, 84, 41, 123, 158, 52, 156},
			{46, 138, 203, 187, 139, 206, 196, 166},
			{76, 17, 51, 153, 37, 111, 122, 155},
			{43, 129, 176, 106, 107, 110, 119, 146}, 
			{16, 48, 144, 10, 30, 90, 59, 177},	 
			{109, 116, 137, 200, 178, 112, 125, 164},
			{70, 210, 208, 202, 184, 130, 179, 115},
			{134, 191, 151, 31, 93, 68, 204, 190},
			{148, 22, 66, 198, 172, 94, 71, 2},
			{6, 18, 54, 162, 64, 192, 154, 40},
			{120, 149, 25, 75, 14, 42, 126, 167},
			{79, 26, 78, 23, 69, 207, 199, 175},
			{103, 98, 83, 38, 114, 131, 182, 124},
			{161, 61, 183, 127, 170, 88, 53, 159},
			{55, 165, 73, 8, 24, 72, 5, 15},
			{45, 135, 194, 160, 58, 174, 100, 89}};
		static int[,] weightRowsExpanded = new int[19, 21] {
			{1, 2, 3,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
			{1, 6, 7, 4,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
			{1, 6, 7, 4, 5,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
			{1, 10, 11, 4, 5, 14,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 
			{1, 10, 11, 4, 5, 14, 15,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 18, 19, 4, 5, 14, 15, 8,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 18, 19, 4, 5, 14, 15, 8, 9,  0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 18, 19, 4, 5, 14, 15, 12, 13, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
			{1, 18, 19, 4, 5, 14, 15, 12, 13, 22, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
			{1, 18, 19, 4, 5, 14, 15, 16, 17, 22, 23, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
			{1, 18, 19, 4, 5, 14, 15, 16, 17, 22, 23, 20, 21, 0, 0, 0, 0, 0, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 0, 0, 0, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 18, 19, 16, 0, 0, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 18, 19, 16, 17, 0, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 18, 19, 20, 21, 22, 0, 0, 0,},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 18, 19, 20, 21, 22, 23, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 15, 12, 13, 18, 19, 16, 17, 22, 23, 20, 0},
			{1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 15, 12, 13, 18, 19, 16, 17, 22, 23, 20, 21}};
		static int[,] widthsChecksumPatternExpanded = new int[12, 5] {
			{1, 8, 4, 1, 1},	{1, 1, 4, 8, 1},	
			{3, 6, 4, 1, 1}, 	{1, 1, 4, 6, 3},	
			{3, 4, 6, 1, 1},	{1, 1, 6, 4, 3},
			{3, 2, 8, 1, 1},	{1, 1, 8, 2, 3},	
			{2, 6, 5, 1, 1},	{1, 1, 5, 6, 2},	
			{2, 2, 9, 1, 1},	{1, 1, 9, 2, 2}};
		static int[,] checksumPatternExpandedSequence = new int[19, 11] {
			{1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 4, 3, 0, 0, 0, 0, 0, 0, 0, 0},
			{1, 6, 3, 8, 0, 0, 0, 0, 0, 0, 0},
			{1, 6, 3, 8, 0, 0, 0, 0, 0, 0, 0},
			{1, 10, 3, 8, 5, 0, 0, 0, 0, 0, 0},
			{1, 10, 3, 8, 5, 0, 0, 0, 0, 0, 0},
			{1, 10, 3, 8, 7, 12, 0, 0, 0, 0, 0},
			{1, 10, 3, 8, 7, 12, 0, 0, 0, 0, 0},
			{1, 10, 3, 8, 9, 12, 11, 0, 0, 0, 0},
			{1, 10, 3, 8, 9, 12, 11, 0, 0, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 10, 9, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 10, 9, 0, 0},
			{1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 0},
			{1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 0},
			{1, 2, 3, 4, 5, 8, 7, 10, 9, 12, 11},
			{1, 2, 3, 4, 5, 8, 7, 10, 9, 12, 11}};
		static StructSymbol[] symbolExpanded = new StructSymbol[5] {
			new StructSymbol(0,	347,  12, 5,  7,  2, 87, 4),
			new StructSymbol(348,  1387, 10, 7,  5,  4, 52, 20),
			new StructSymbol(1388, 2947, 8,  9,  4,  5, 30, 52),
			new StructSymbol(2948, 3987, 6,  11, 3,  6, 10, 104),
			new StructSymbol(3988, 4191, 4,  13, 1,  8, 1,  204)};
		#endregion
		public DataBarExpandedPatternProcessor(DataBarType type) {
			this.Type = type;
		}
		public override bool IsValidTextFormat(string text) {
			return ConvertTextToBinaryData(text).Length > 252 || text == "" ? false : true;
		}
		int GetEncodingMethod(List<GS1Helper.ElementResult> textElement) {
			int count = textElement.Count;
			if(count > 0 && (String.IsNullOrEmpty(textElement[0].AI) || textElement[0].AI != "01" || textElement[0].Value.Length != 14) || count == 0)
				return 2;
			if(textElement[0].Value[0] != '9')
				return 1;
			if(count == 1 || String.IsNullOrEmpty(textElement[1].AI) || textElement[1].AI.Length != 4)
				return 2;
			if(textElement[1].AI.Substring(0, 3) == "310" && textElement[1].Value.Length == 6) {
				int weight;
				if(int.TryParse(textElement[1].Value, out weight) && weight <= 99999) {
					if(count == 2)
						return (textElement[1].AI == "3103" && weight <= 32767) ? 3 : 7;
					if(count == 3 && textElement[2].Value.Length == 6 && !String.IsNullOrEmpty(textElement[2].AI)) {
						switch(textElement[2].AI) {
							case "11": return 7;
							case "13": return 9;
							case "15": return 11;
							case "17": return 13;
						}
					}
				}
			} else if(textElement[1].AI.Substring(0, 3) == "320" && textElement[1].Value.Length == 6) {
				int weight;
				if(int.TryParse(textElement[1].Value, out weight) && weight <= 99999) {
					if(count == 2)
						return textElement[1].AI == "3202" && weight <= 9999 || textElement[1].AI == "3203" && weight <= 22767 ? 4 : 8;
					if(count == 3 && textElement[2].Value.Length == 6 && !String.IsNullOrEmpty(textElement[2].AI)) {
						switch(textElement[2].AI) {
							case "11": return 8;
							case "13": return 10;
							case "15": return 12;
							case "17": return 14;
						}
					}
				}
			} else if(textElement[1].AI.Substring(0, 2) == "39") {
				int lastSignAI = Convert.ToInt32((textElement[1].AI)[3] - '0');
				if(textElement[1].AI.Substring(0, 3) == "392" && lastSignAI >= 0 && lastSignAI <= 3 && textElement[1].Value.Length > 0) return 5;
				if(textElement[1].AI.Substring(0, 3) == "393" && lastSignAI >= 0 && lastSignAI <= 3 && textElement[1].Value.Length > 3) return 6;
			}
			return 2;
		}
		char[] MakeBinaryString(int encodingMethod, List<GS1Helper.ElementResult> textElement) {
			StringBuilder resultBinaryString = new StringBuilder();
			char[] binaryArray = new char[260];
			resultBinaryString.Append('0');
			switch(encodingMethod) {
				case 1: resultBinaryString.Append("1XY"); break;
				case 2: resultBinaryString.Append("00XY"); break;
				case 3: resultBinaryString.Append("0100"); break;
				case 4: resultBinaryString.Append("0101"); break;
				case 5: resultBinaryString.Append("01100XY"); break;
				case 6: resultBinaryString.Append("01101XY"); break;
				case 7: resultBinaryString.Append("0111000"); break;
				case 8: resultBinaryString.Append("0111001"); break;
				case 9: resultBinaryString.Append("0111010"); break;
				case 10: resultBinaryString.Append("0111011"); break;
				case 11: resultBinaryString.Append("0111100"); break;
				case 12: resultBinaryString.Append("0111101"); break;
				case 13: resultBinaryString.Append("0111110"); break;
				case 14: resultBinaryString.Append("0111111"); break;
			}
			AddBinaryCompressedData(encodingMethod, textElement, resultBinaryString);
			string universalData = MakeUniversalData(encodingMethod, textElement);
			if(universalData == "" && encodingMethod != 1 && encodingMethod != 5 && encodingMethod != 6) { 
				return resultBinaryString.ToString().ToCharArray();
			} else {
				if(universalData != "") {
					if(universalData[universalData.Length - 1] == fnc1Char) { universalData = universalData.Remove(universalData.Length - 1); }
					if(universalData == "") universalData = "0";
					CodingType[] universalDataTypes = MakeUniversalDataTypesArray(universalData);
					List<BlockData> resultBlock = SeparateToTypedBlocks(universalDataTypes);
					ConvertTypedBlocks(resultBlock);
					bool lastNumericOddSize = false;
					if((resultBlock[resultBlock.Count - 1].Type == CodingType.NUMERIC) && (resultBlock[resultBlock.Count - 1].Length % 2 == 1))
						lastNumericOddSize = true;
					if(lastNumericOddSize) resultBlock[resultBlock.Count - 1].Length -= 1;
					MakeBinaryDataFromUniversalData(universalData, resultBlock, resultBinaryString);
					PadBinaryString(resultBinaryString, universalData, resultBlock, lastNumericOddSize);
				}
				SetVaiableLengthField(resultBinaryString);
				binaryArray = resultBinaryString.ToString().ToCharArray();
			}
			return binaryArray;
		}
		CodingType[] MakeUniversalDataTypesArray(string universalData) {
			CodingType[] result = new CodingType[universalData.Length];
			for(int i = 0; i < universalData.Length; i++) {
				char currentChar = (char)universalData[i];
				if(currentChar >= ' ' && currentChar <= 'z') { result[i] = CodingType.ISOIEC; }
				if((currentChar == '*' || currentChar == ',' || currentChar == '-' || currentChar == '.' || currentChar == '/') || (currentChar >= 'A' && currentChar <= 'Z')) { result[i] = CodingType.ALPHA_OR_ISO; }
				if(currentChar >= '0' && currentChar <= '9') result[i] = CodingType.ANY_ENC;
				if(currentChar == fnc1Char) { result[i] = result[i - 1]; }
			}
			return result;
		}
		void PadBinaryString(StringBuilder resultBinaryString, string universalData, List<BlockData> resultBlock, bool lastNumericOddSIze) {
			int remainder = (12 - resultBinaryString.Length % 12) % 12;
			if(resultBinaryString.Length < 36) remainder = 36 - resultBinaryString.Length;
			if(lastNumericOddSIze) {
				int d1, d2;
				if((remainder >= 4) && (remainder <= 6)) {
					d1 = Convert.ToInt16(universalData[universalData.Length - 1] - '0');
					d1++;
					resultBinaryString.Append(ConvertIntToBinary(d1, 0x08, 4));
				} else {
					d1 = Convert.ToInt16(universalData[universalData.Length - 1] - '0');
					d2 = 10;
					int value = (11 * d1) + d2 + 8;
					resultBinaryString.Append(ConvertIntToBinary(value, 0x40, 7));
				}
				remainder = (12 - resultBinaryString.Length % 12) % 12;
				if(resultBinaryString.Length < 36) remainder = 36 - resultBinaryString.Length;
			}
			if(resultBlock[resultBlock.Count - 1].Type == CodingType.NUMERIC) {
				resultBinaryString.Append("00000010000100001000010000100", 0, remainder);
			} else {
				resultBinaryString.Append("001000010000100001000010000100", 0, remainder);
			}
		}
		void MakeBinaryDataFromUniversalData(string universalData, List<BlockData> resultBlock, StringBuilder resultBinaryString) {
			int currentIndex = 0;
			for(int i = 0; i < resultBlock.Count; i++) {
				for(int j = 0; j < resultBlock[i].Length; j++) {
					switch(resultBlock[i].Type) {
						case CodingType.NUMERIC:
							int d1, d2;
							if(i != 0 && j == 0) {
								if(universalData[currentIndex - 1] != fnc1Char)
									resultBinaryString.Append("000");
							}
							if(universalData[currentIndex] != fnc1Char)
								d1 = Convert.ToInt16(universalData[currentIndex] - '0');
							else {
								d1 = 10;
								int test = resultBinaryString.Length;
							}
							if(universalData[currentIndex + 1] != fnc1Char)
								d2 = Convert.ToInt16(universalData[currentIndex + 1] - '0');
							else d2 = 10;
							int value = 11 * d1 + d2 + 8;
							string valueBinary = ConvertIntToBinary(value, 0x40, 7);
							resultBinaryString.Append(valueBinary);
							j++;
							currentIndex++;
							break;
						case CodingType.ALPHA:
							if(i == 0 && j == 0) { resultBinaryString.Append("0000"); }
							if(i != 0 && j == 0) {
								if(resultBlock[i - 1].Type == CodingType.NUMERIC || universalData[currentIndex - 1] == fnc1Char)
									resultBinaryString.Append("0000");
								if(resultBlock[i - 1].Type == CodingType.ISOIEC)
									resultBinaryString.Append("00100");
							}
							if(universalData[currentIndex] >= '0' && universalData[currentIndex] <= '9') {
								resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 43, 0x10, 5));
							} else {
								if(universalData[currentIndex] >= 'A' && universalData[currentIndex] <= 'Z') {
									resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 33, 0x20, 6));
								} else {
									switch(universalData[currentIndex]) {
										case fnc1Char: resultBinaryString.Append("01111"); break;
										case '*': resultBinaryString.Append("111010"); break;
										case ',': resultBinaryString.Append("111011"); break;
										case '-': resultBinaryString.Append("111100"); break;
										case '.': resultBinaryString.Append("111101"); break;
										case '/': resultBinaryString.Append("111110"); break;
									}
								}
							}
							break;
						case CodingType.ISOIEC:
							if(j == 0) {
								if(i == 0) {
									if(universalData[currentIndex] >= '0' && universalData[currentIndex] <= '9') {
										resultBinaryString.Append("0000");
										resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 43, 0x10, 5));
										resultBinaryString.Append("00100");
										break;
									} else
										resultBinaryString.Append("000000100");
								} else {
									if(resultBlock[i - 1].Type == CodingType.NUMERIC || universalData[currentIndex - 1] == fnc1Char) {
										if(resultBlock[i - 1].Type == CodingType.NUMERIC && universalData[currentIndex] >= '0' && universalData[currentIndex] <= '9') {
											resultBinaryString.Append("0000");
											resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 43, 0x10, 5));
											resultBinaryString.Append("00100");
											break;
										} else
											resultBinaryString.Append("000000100");
									} else if(resultBlock[i - 1].Type == CodingType.ALPHA)
										resultBinaryString.Append("00100");
								}
							}
							if(universalData[currentIndex] >= '0' && universalData[currentIndex] <= '9') {
								resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 43, 0x10, 5));
							} else {
								if(universalData[currentIndex] >= 'A' && universalData[currentIndex] <= 'Z') {
									resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 1, 0x40, 7));
								} else {
									if(universalData[currentIndex] >= 'a' && universalData[currentIndex] <= 'z') {
										resultBinaryString.Append(ConvertIntToBinary((int)universalData[currentIndex] - 7, 0x40, 7));
									} else {
										switch(universalData[currentIndex]) {
											case fnc1Char: resultBinaryString.Append("01111"); break;
											case '!': resultBinaryString.Append("11101000"); break;
											case '"': resultBinaryString.Append("11101001"); break;
											case '%': resultBinaryString.Append("11101010"); break;
											case '&': resultBinaryString.Append("11101011"); break;
											case (char)39: resultBinaryString.Append("11101100"); break;
											case '(': resultBinaryString.Append("11101101"); break;
											case ')': resultBinaryString.Append("11101110"); break;
											case '*': resultBinaryString.Append("11101111"); break;
											case '+': resultBinaryString.Append("11110000"); break;
											case ',': resultBinaryString.Append("11110001"); break;
											case '-': resultBinaryString.Append("11110010"); break;
											case '.': resultBinaryString.Append("11110011"); break;
											case '/': resultBinaryString.Append("11110100"); break;
											case ':': resultBinaryString.Append("11110101"); break;
											case ';': resultBinaryString.Append("11110110"); break;
											case '<': resultBinaryString.Append("11110111"); break;
											case '=': resultBinaryString.Append("11111000"); break;
											case '>': resultBinaryString.Append("11111001"); break;
											case '?': resultBinaryString.Append("11111010"); break;
											case '_': resultBinaryString.Append("11111011"); break;
											case ' ': resultBinaryString.Append("11111100"); break;
										}
									}
								}
							}
							break;
					}
					currentIndex++;
				}
			}
		}
		string MakeUniversalData(int encodingMethod, List<GS1Helper.ElementResult> textElement) {
			string result = "";
			if((encodingMethod == 1 && textElement.Count > 1) || (encodingMethod == 2) || (encodingMethod == 5 && textElement[1].Value.Length > 0) || (encodingMethod == 6 && textElement[1].Value.Length > 3)) {
				if(encodingMethod == 1) {
					for(int i = 1; i < textElement.Count; i++) {
						result += textElement[i].AI + textElement[i].Value;
						if(!FixedLengthAI(textElement[i].AI) && !IsMaxLength(textElement[i])) { result += fnc1Char; }
					}
				} else if(encodingMethod == 2) {
					for(int i = 0; i < textElement.Count; i++) {
						result += textElement[i].AI + textElement[i].Value;
						if(!FixedLengthAI(textElement[i].AI) && !IsMaxLength(textElement[i])) {
							result += fnc1Char;
						}
					}
				} else if(encodingMethod == 5) {
					result += textElement[1].Value;
					for(int i = 2; i < textElement.Count; i++) {
						result += textElement[i].AI + textElement[i].Value;
						if(!FixedLengthAI(textElement[i].AI) && !IsMaxLength(textElement[i])) { result += fnc1Char; }
					}
				} else if(encodingMethod == 6) {
					result += textElement[1].Value.Substring(3) + fnc1Char;
					for(int i = 2; i < textElement.Count; i++) {
						result += textElement[i].AI + textElement[i].Value;
						if(!FixedLengthAI(textElement[i].AI) && !IsMaxLength(textElement[i])) { result += fnc1Char; }
					}
				}
			}
			return result;
		}
		void ConvertTypedBlocks(List<BlockData> resultBlock) {
			for(int i = 0; i < resultBlock.Count; i++) {
				BlockData currentBlock = new BlockData(resultBlock[i]);
				if(i != resultBlock.Count - 1) {
					BlockData nextBlock = new BlockData(resultBlock[i + 1]);
					if(currentBlock.Type == CodingType.ISOIEC) {
						if(CanBeNumeric(resultBlock, i + 1) && nextBlock.Type != CodingType.ISOIEC) {
							if(nextBlock.Type == CodingType.ANY_ENC && nextBlock.Length >= 4)
								resultBlock[i + 1].Type = CodingType.NUMERIC;
							else
								if(FiveAlphaSymbols(resultBlock, i + 1))
									resultBlock[i + 1].Type = CodingType.ALPHA;
								else
									resultBlock[i + 1].Type = CodingType.ISOIEC;
						} else
							resultBlock[i + 1].Type = CodingType.ISOIEC;
					}
					if(currentBlock.Type == CodingType.ALPHA || currentBlock.Type == CodingType.ALPHA_OR_ISO) {
						if(nextBlock.Type == CodingType.ANY_ENC && nextBlock.Length >= 6) {
							resultBlock[i + 1].Type = CodingType.NUMERIC;
						} else {
							if(nextBlock.Type == CodingType.ANY_ENC && i == resultBlock.Count - 2 && nextBlock.Length >= 4) {
								resultBlock[i + 1].Type = CodingType.NUMERIC;
							} else {
								if(nextBlock.Type == CodingType.ISOIEC) {
									resultBlock[i + 1].Type = CodingType.ISOIEC;
								} else
									resultBlock[i + 1].Type = CodingType.ALPHA;
							}
						}
					}
				}
				if(currentBlock.Type == CodingType.ALPHA_OR_ISO) { resultBlock[i].Type = CodingType.ALPHA; }
				if(currentBlock.Type == CodingType.ANY_ENC) { resultBlock[i].Type = CodingType.NUMERIC; }
			}
			for(int i = 0; i < resultBlock.Count - 1; i++) {
				if(resultBlock[i].Type == resultBlock[i + 1].Type) {
					resultBlock[i].Length += resultBlock[i + 1].Length;
					resultBlock.RemoveAt(i + 1);
					i--;
				}
			}
			for(int i = 0; i < resultBlock.Count - 1; i++) {
				if((resultBlock[i].Type == CodingType.NUMERIC) && (resultBlock[i].Length % 2 == 1)) {
					resultBlock[i].Length -= 1;
					resultBlock[i + 1].Length += 1;
				}
			}
			for(int i = 0; i < resultBlock.Count; i++) {
				if(resultBlock[i].Length == 0) {
					resultBlock.RemoveAt(i);
					i--;
				}
			}
		}
		static List<BlockData> SeparateToTypedBlocks(CodingType[] universalDataTypes) {
			List<BlockData> result = new List<BlockData>();
			int blockLength = 1;
			CodingType blockType = universalDataTypes[0];
			for(int i = 1; i < universalDataTypes.Length; i++) {
				if(blockType == universalDataTypes[i]) {
					blockLength += 1;
				} else {
					result.Add(new BlockData(blockType, blockLength));
					blockLength = 1;
					blockType = universalDataTypes[i];
				}
			}
			result.Add(new BlockData(blockType, blockLength));
			return result;
		}
		void AddBinaryCompressedData(int encodingMethod, List<GS1Helper.ElementResult> textElement, StringBuilder resultBinaryString) {
			if(encodingMethod == 1) {
				int firstNumber = Convert.ToInt16(textElement[0].Value.Substring(0, 1));
				resultBinaryString.Append(ConvertIntToBinary(firstNumber, 0x08, 4));
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
			} else if(encodingMethod == 3) {
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
				int weight = Convert.ToInt32(textElement[1].Value.Substring(0, 6));
				resultBinaryString.Append(ConvertIntToBinary(weight, 0x4000, 15));
			} else if(encodingMethod == 4) {
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
				int weight = Convert.ToInt32(textElement[1].Value.Substring(0, 6));
				if(textElement[1].AI == "3203") { weight += 10000; }
				resultBinaryString.Append(ConvertIntToBinary(weight, 0x4000, 15));
			} else if(encodingMethod >= 7 && encodingMethod <= 14) {
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
				int weight = Convert.ToInt32(textElement[1].AI.Substring(3, 1) + textElement[1].Value.Substring(1, 5));
				resultBinaryString.Append(ConvertIntToBinary(weight, 0x80000, 20));
				int date = 0;
				if(textElement.Count == 3) {
					date += Convert.ToInt32(textElement[2].Value.Substring(0, 2)) * 384;
					date += (Convert.ToInt32(textElement[2].Value.Substring(2, 2)) - 1) * 32;
					date += Convert.ToInt32(textElement[2].Value.Substring(4, 2));
				} else {
					date = 38400;
				}
				resultBinaryString.Append(ConvertIntToBinary(date, 0x8000, 16));
			} else if(encodingMethod == 5) {
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
				switch(textElement[1].AI[3]) {
					case '0': resultBinaryString.Append("00"); break;
					case '1': resultBinaryString.Append("01"); break;
					case '2': resultBinaryString.Append("10"); break;
					case '3': resultBinaryString.Append("11"); break;
				}
			} else if(encodingMethod == 6) {
				for(int i = 0; i < 4; i++) {
					int number = Convert.ToInt16(textElement[0].Value.Substring(i * 3 + 1, 3));
					resultBinaryString.Append(ConvertIntToBinary(number, 0x200, 10));
				}
				switch(textElement[1].AI[3]) {
					case '0': resultBinaryString.Append("00"); break;
					case '1': resultBinaryString.Append("01"); break;
					case '2': resultBinaryString.Append("10"); break;
					case '3': resultBinaryString.Append("11"); break;
				}
				int codeCurrency = Convert.ToInt16(textElement[1].Value.Substring(0, 3));
				resultBinaryString.Append(ConvertIntToBinary(codeCurrency, 0x200, 10));
			}
		}
		static void SetVaiableLengthField(StringBuilder resultBinaryString) {
			int d1 = ((resultBinaryString.Length / 12) + 1) % 2;
			int d2 = resultBinaryString.Length <= 156 ? 0 : 1;
			resultBinaryString.Replace('X', d1 == 0 ? '0' : '1', 0, 8);
			resultBinaryString.Replace('Y', d2 == 0 ? '0' : '1', 0, 8);
		}
		bool CanBeNumeric(List<BlockData> blocks, int startIndex) {
			int totalLength = blocks[startIndex].Length;
			for(int i = startIndex + 1; i < blocks.Count && totalLength < 10; i++) {
				if(blocks[i].Type == CodingType.ISOIEC)
					return false;
				totalLength += blocks[i].Length;
			}
			return true;
		}
		bool FiveAlphaSymbols(List<BlockData> blocks, int startIndex) {
			int totalLength = 0;
			for(int i = startIndex; i < blocks.Count; i++) {
				totalLength += blocks[i].Length;
				if(totalLength >= 5)
					return true;
			}
			return false;
		}
		string ConvertIntToBinary(int value, int mask, int n) {
			string result = "";
			for(int i = 0; i < n; i++) {
				if((value & mask) == 0x00) {
					result += '0';
				} else {
					result += '1';
				}
				mask = mask >> 1;
			}
			return result;
		}
		bool FixedLengthAI(string ai) {
			if(String.IsNullOrEmpty(ai)) return false;
			if(Array.IndexOf(numbersAI, Convert.ToInt16(ai.Substring(0, 2))) < 0) return false;
			return true;
		}
		bool IsMaxLength(GS1Helper.ElementResult textElement) {
			int aiElementMaxLength = -1;
			if(!String.IsNullOrEmpty(textElement.AI))
				aiElementMaxLength = GS1Helper.knownAI[GS1Helper.knownAI.FindIndex(x => x.id == textElement.AI)].length;
			if(aiElementMaxLength != textElement.Value.Length) return false;
			else
				return true;
		}
		char[] ConvertTextToBinaryData(string text) {
			List<GS1Helper.ElementResult> textElement = new List<GS1Helper.ElementResult>();
			foreach(GS1Helper.ElementResult aiElement in GS1Helper.GetAIElements(text, fnc1Char, fnc1Subst))
				textElement.Add(aiElement);
			int encodingMethod = GetEncodingMethod(textElement);
			char[] result = MakeBinaryString(encodingMethod, textElement);
			return result;
		}
		int[] ConvertBinaryDataToInt(char[] binaryData) {
			int count = binaryData.Length / 12;
			int[] result = new int[count];
			for(int i = 0; i < count; i++) {
				int number = 0;
				for(int j = 0; j < 12; j++) {
					number <<= 1;
					number |= binaryData[i * 12 + j] - '0';
				}
				result[i] = number;
			}
			return result;
		}
		public override ArrayList GetPattern(string text) {
			List<int> pattern = new List<int>();
			char[] binaryData = ConvertTextToBinaryData(text);
			int[] data = ConvertBinaryDataToInt(binaryData);
			int[] dataType = new int[data.Length];
			for(int i = 0; i < data.Length; i++) {
				for(int j = 0; j < 5; j++) {
					if(data[i] >= symbolExpanded[j].MinValue && data[i] <= symbolExpanded[j].MaxValue) {
						dataType[i] = j;
						break;
					}
				}
			}
			int[][] signPattern = new int[data.Length][];
			for(int i = 0; i < data.Length; i++) {
				StructSymbol symbol = symbolExpanded[dataType[i]];
				signPattern[i] = SubPattern(data[i], symbol, elementsPairExpanded, true);
			}
			int checkSum = 0;
			for(int i = 0; i < data.Length; i++) {
				for(int j = 0; j < 2 * elementsPairExpanded; j++) {
					checkSum += signPattern[i][j] * weightChecksumElemExpanded[weightRowsExpanded[data.Length - 3, i] - 1, j];
				}
			}
			checkSum = checkSum % checkSumModifierExpanded;
			int checkSignValue = checkSumModifierExpanded * (data.Length - 3) + checkSum;
			int[] checkSignPattern = new int[elementsPairExpanded * 2];
			for(int i = 0; i < 5; i++) {
				if(checkSignValue >= symbolExpanded[i].MinValue && checkSignValue <= symbolExpanded[i].MaxValue) {
					StructSymbol symbol = symbolExpanded[i];
					checkSignPattern = SubPattern(checkSignValue, symbol, elementsPairExpanded, true);
					break;
				}
			}
			pattern.AddRange(generalPatternLimiter);
			pattern.AddRange(checkSignPattern);
			for(int i = 0; i < ((data.Length + 1) / 2); i++) {
				int test = checksumPatternExpandedSequence[data.Length - 3, i] - 1;
				for(int j = 0; j < 5; j++) {
					pattern.Add(widthsChecksumPatternExpanded[checksumPatternExpandedSequence[data.Length - 3, i] - 1, j]);
				}
				for(int j = 0; j < 8; j++) {
					pattern.Add(signPattern[2 * i][7 - j]);
				}
				if((data.Length + 1) % 2 != 0 || ((data.Length + 1) % 2 == 0 && i != ((data.Length + 1) / 2) - 1)) {
					for(int j = 0; j < 8; j++) {
						pattern.Add(signPattern[2 * i + 1][j]);
					}
				}
			}
			if(data.Length % 2 == 0) {
				for(int j = 0; j < 5; j++) {
					pattern.Add(widthsChecksumPatternExpanded[checksumPatternExpandedSequence[data.Length - 3, (data.Length / 2)] - 1, j]);
				}
			}
			pattern.AddRange(generalPatternLimiter);
			int count = pattern.Count;
			List<PatternElement> newFormatResult = new List<PatternElement>();
			if(Type == DataBarType.ExpandedStacked) {
				newFormatResult = SplitExpandedPattern(pattern, segmentPairsInRow);
				newFormatResult = AddStackedExpandedPattern(newFormatResult, segmentPairsInRow);
			} else {
				PatternElement patternElement = new PatternElement(34, pattern, false);
				newFormatResult.Add(patternElement);
			}
			ArrayList result = new ArrayList(newFormatResult);
			return new ArrayList(result);
		}
		protected List<PatternElement> SplitExpandedPattern(List<int> pattern, int segmentPairsInRow) {
			int lengthData = pattern.Count - 4;
			int countsegmentPairs = (int)Math.Ceiling((double)lengthData / barsInSegmentPair);
			bool fullLastSegment = (lengthData % barsInSegmentPair == 0 ? true : false);
			bool fullLastRow = ((countsegmentPairs % segmentPairsInRow == 0) && (fullLastSegment) ? true : false);
			bool reversEvenRows = (segmentPairsInRow % 2 == 0 ? true : false);
			int countPatternInRow = barsInSegmentPair * segmentPairsInRow;
			int countRows = (int)Math.Ceiling((double)countsegmentPairs / segmentPairsInRow);
			List<PatternElement> result = new List<PatternElement>();
			if(segmentPairsInRow < countsegmentPairs) {
				for(int i = 0; i < countRows - 1; i++) {
					int startIndex = 2 + i * barsInSegmentPair * segmentPairsInRow;
					List<int> partPattern = pattern.GetRange(startIndex, countPatternInRow);
					if(reversEvenRows && i % 2 == 1)
						partPattern.Reverse();
					PatternElement part = new PatternElement(34, partPattern, Convert.ToBoolean(i % 2));
					part.pattern.InsertRange(0, generalPatternLimiter);
					part.pattern.AddRange(generalPatternLimiter);
					result.Add(part);
				}
				List<int> lastPartPattern = new List<int>();
				bool startBarBlack = !result[countRows - 2].startBarBlack;
				if(!fullLastRow) {
					int startIndex = (countRows - 1) * barsInSegmentPair * segmentPairsInRow + 2;
					lastPartPattern = pattern.GetRange(startIndex, pattern.Count - startIndex - 2);
					if(reversEvenRows && (countRows - 1) % 2 == 1) {
						lastPartPattern.InsertRange(0, specialPatternLimiter);
						startBarBlack = !startBarBlack;
					} else {
						lastPartPattern.InsertRange(0, generalPatternLimiter);
					}
				} else {
					int startIndex = (countRows - 1) * barsInSegmentPair * segmentPairsInRow + 2;
					lastPartPattern = pattern.GetRange(startIndex, countPatternInRow);
					if(reversEvenRows && (countRows - 1) % 2 == 1) lastPartPattern.Reverse();
					lastPartPattern.InsertRange(0, generalPatternLimiter);
				}
				lastPartPattern.AddRange(generalPatternLimiter);
				PatternElement lastPart = new PatternElement(34, lastPartPattern, startBarBlack);
				result.Add(lastPart);
			} else {
				PatternElement part = new PatternElement(34, pattern, false);
				result.Add(part);
			}
			return result;
		}
		protected List<PatternElement> AddStackedExpandedPattern(List<PatternElement> pattern, int segmentPairsInRow) {
			List<List<bool>> booleanPattern = ConvertPatternToBooleanArray(pattern);
			int countRows = booleanPattern.Count;
			if(countRows > 1) {
				bool incompleteLastRow = (booleanPattern[countRows - 1].Count != booleanPattern[countRows - 2].Count ? true : false);
				int countModulesInFullPattern = booleanPattern[0].Count;
				List<List<bool>> resultBooleanPattern = new List<List<bool>>();
				int countFullRows = (incompleteLastRow ? countRows - 1 : countRows);
				for(int i = 0; i < 3 * (countFullRows - 1); i++) {
					resultBooleanPattern.Add(new List<bool>(indentForSeparation));
				}
				List<bool> exceptionModules = CreateExceptionArray(segmentPairsInRow);
				for(int i = 0; i < countFullRows - 1; i++) {
					bool midValue = false;
					bool topValue = true;
					bool botValue = false;
					bool flagOnePart = false;
					for(int j = countIndentForSeparation; j < countModulesInFullPattern - countIndentForSeparation; j++) {
						if(exceptionModules[j]) {
							if(booleanPattern[i][j] == true) {
								resultBooleanPattern[i * 3].Add(false);
								if(flagOnePart) {
									flagOnePart = false;
									topValue = true;
								}
							} else {
								resultBooleanPattern[i * 3].Add(topValue);
								topValue = !topValue;
								flagOnePart = true;
							}
						} else {
							resultBooleanPattern[i * 3].Add(!booleanPattern[i][j]);
						}
						resultBooleanPattern[i * 3 + 1].Add(midValue);
						midValue = !midValue;
						if(exceptionModules[j]) {
							if(booleanPattern[i + 1][j] == true) {
								resultBooleanPattern[i * 3 + 2].Add(false);
							} else {
								resultBooleanPattern[i * 3 + 2].Add(botValue);
								botValue = !botValue;
							}
						} else {
							resultBooleanPattern[i * 3 + 2].Add(!booleanPattern[i + 1][j]);
						}
					}
					for(int k = 0; k < 3; k++) {
						resultBooleanPattern[i + k].AddRange(indentForSeparation);
					}
					List<PatternElement> separationPattern = ConvertBooleanArrayToPattern(resultBooleanPattern);
					pattern.Insert(i * 4 + 1, separationPattern[i * 3]);
					pattern.Insert(i * 4 + 2, separationPattern[i * 3 + 1]);
					pattern.Insert(i * 4 + 3, separationPattern[i * 3 + 2]);
				}
				if(incompleteLastRow) {
					bool midValue = false;
					bool topValue = true;
					bool botValue = false;
					bool flagOnePart = false;
					List<List<bool>> lastPartBooleanPattern = new List<List<bool>>();
					for(int i = 0; i < 3; i++) {
						lastPartBooleanPattern.Add(new List<bool>(indentForSeparation));
					}
					for(int j = countIndentForSeparation; j < countModulesInFullPattern - countIndentForSeparation; j++) {
						if(exceptionModules[j]) {
							if(booleanPattern[countRows - 2][j] == true) {
								lastPartBooleanPattern[0].Add(false);
								if(flagOnePart) {
									flagOnePart = false;
									topValue = true;
								}
							} else {
								lastPartBooleanPattern[0].Add(topValue);
								topValue = !topValue;
								flagOnePart = true;
							}
						} else {
							lastPartBooleanPattern[0].Add(!booleanPattern[countRows - 2][j]);
						}
						lastPartBooleanPattern[1].Add(midValue);
						midValue = !midValue;
						if(j < booleanPattern[countRows - 1].Count - 1) {
							if(exceptionModules[j]) {
								if(booleanPattern[countRows - 1][j] == true) {
									lastPartBooleanPattern[2].Add(false);
								} else {
									lastPartBooleanPattern[2].Add(botValue);
									botValue = !botValue;
								}
							} else {
								lastPartBooleanPattern[2].Add(!booleanPattern[countRows - 1][j]);
							}
						} else {
							lastPartBooleanPattern[2].Add(false);
						}
					}
					for(int k = 0; k < 3; k++) {
						lastPartBooleanPattern[k].AddRange(indentForSeparation);
					}
					List<PatternElement> lastSeparationPattern = ConvertBooleanArrayToPattern(lastPartBooleanPattern);
					pattern.InsertRange(pattern.Count - 1, lastSeparationPattern);
				}
			}
			return pattern;
		}
		List<bool> CreateExceptionArray(int segmentPairsInRow) {
			bool[] sign = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
			List<bool> result = new List<bool>();
			result.AddRange(boolPatternLimiter);
			for(int i = 0; i < segmentPairsInRow; i++) {
				result.AddRange(sign);
				if(i % 2 == 0) result.AddRange(boolFinderPatternType1);
				else result.AddRange(boolFinderPatternType2);
				result.AddRange(sign);
			}
			result.AddRange(boolPatternLimiter);
			return result;
		}
#if DEBUGTEST
		internal char[] Test_ConvertTextToBinaryData(string text){
			return ConvertTextToBinaryData(text);
		}
#endif
	}
}
