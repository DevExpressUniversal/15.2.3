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
using System.IO;
using System.Text;
namespace DevExpress.XtraRichEdit.Model {
	#region Hyphenizer
	public class Hyphenizer {
		#region Fields
		Dictionary<string, int[]> patternTable = new Dictionary<string, int[]>();
		Dictionary<string, List<int>> exceptionTable = new Dictionary<string, List<int>>();
		int maxPatternLength;
		int leftHyphenMin = 2;
		int rightHyphenMin = 2;
		string extendedWord;
		int extendedWordLength;
		int[] patternCharsValues;
		#endregion
		public Hyphenizer(string patternFileName)
			: this(patternFileName, null) {
		}
		public Hyphenizer(Stream patternStream)
			: this(patternStream, null) {
		}
		public Hyphenizer(Stream patternStream, Stream exceptionsStream) {
			LoadPatterns(patternStream);
			LoadExceptions(exceptionsStream);
		}
		public Hyphenizer(string patternsFileName, string exceptionsFileName) {
			LoadPatterns(patternsFileName);
			LoadExceptions(exceptionsFileName);
		}
		#region Properties
		public int LeftHyphenMin { get { return leftHyphenMin; } set { leftHyphenMin = value; } }
		public int RightHyphenMin { get { return rightHyphenMin; } set { rightHyphenMin = value; } }
		#endregion
		void LoadPatterns(string fileName) {
			if (fileName == null || fileName.Length == 0)
				return;
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				LoadPatterns(stream);
			}
		}
		void LoadExceptions(string fileName) {
			if (fileName == null || fileName.Length == 0)
				return;
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				LoadExceptions(stream);
			}
		}
		void LoadPatterns(Stream stream) {
			if (stream == null)
				return;
			string line;
			using (StreamReader sr = new StreamReader(stream, Encoding.Unicode)) {
				while ((line = sr.ReadLine()) != null)
					AddPattern(line);
			}
		}
		void LoadExceptions(Stream stream) {
			if (stream == null)
				return;
			string line;
			using (StreamReader sr = new StreamReader(stream, Encoding.Unicode)) {
				while ((line = sr.ReadLine()) != null)
					AddException(line);
			}
		}
		void AddException(string exception) {
			int length = exception.Length;
			if (length == 0)
				return;
			List<int> positions = new List<int>();
			for (int i = 0; i < length; i++) {
				if (exception[i] == '-')
					positions.Add(i - positions.Count);
			}
			string realWord = (string)exception.Replace("-", "");
			exceptionTable.Add(realWord, positions);
		}
		static int GetPatternStringLength(string pattern) {
			int count = pattern.Length;
			int result = 0;
			for (int i = 0; i < count; i++) {
				if (!Char.IsDigit(pattern, i))
					result++;
			}
			return result;
		}
		void ApplySubPatterns(string patternString, int[] patternArray) {
			while (patternString.Length > 0) {
				patternString = patternString.Remove(patternString.Length - 1, 1);
				int[] subPatternArray;
				if (patternTable.TryGetValue(patternString, out subPatternArray) && subPatternArray != null)
					ApplyPattern(patternArray, subPatternArray, 0);
			}
		}
		void AddPattern(string pattern) {
			if (pattern.Length == 0)
				return;
			int patternLength = GetPatternStringLength(pattern);
			int[] patternArray = new int[patternLength + 1];
			maxPatternLength = Math.Max(patternLength, maxPatternLength);
			string patternString = CreatePatternString(pattern, patternArray);
			ApplySubPatterns(patternString, patternArray);
			patternTable.Add(patternString, patternArray);
		}
		static string CreatePatternString(string pattern, int[] patternArray) {
			int count = pattern.Length;
			StringBuilder result = new StringBuilder();
			int patternArrayIndex = 0;
			for (int i = 0; i < count; i++) {
				int numericValue = (int)Char.GetNumericValue(pattern[i]);
				if (numericValue >= 0)
					patternArray[patternArrayIndex] = numericValue;
				else {
					patternArrayIndex++;
					result.Append(pattern[i]);
				}
			}
			return result.ToString();
		}
		static void ApplyPattern(int[] values, int[] pattern, int position) {
			int length = pattern.Length;
			for (int i = 0; i < length; i++)
				values[i + position] = Math.Max(values[i + position], pattern[i]);
		}
		static bool IsOdd(int number) {
			return (number & 1) == 1;
		}
		void ApplyPatternToSubwords(int maxSubwordLength, int subwordsStartIndex) {
			maxSubwordLength = Math.Min(maxSubwordLength, extendedWordLength - subwordsStartIndex);
			for (int subStrLength = maxSubwordLength; subStrLength > 0; subStrLength--) {
				string subword = extendedWord.Substring(subwordsStartIndex, subStrLength);
				int[] pattern;
				if (patternTable.TryGetValue(subword, out pattern) && pattern != null) {
					ApplyPattern(patternCharsValues, pattern, subwordsStartIndex);
					return;
				}
			}
		}
		List<int> GetHyphenPositions(string word, int offset) {
			List<int> result = new List<int>();
			int wordLength = word.Length;
			int maxHyphenPosition = wordLength - RightHyphenMin - 1;
			for (int i = 0; i < wordLength; i++) {
				bool canAddHyphen = IsOdd(patternCharsValues[i + 1]) && i >= LeftHyphenMin && i <= maxHyphenPosition;
				if (canAddHyphen)
					result.Add(i + offset);
			}
			return result;
		}
		public virtual List<int> Hyphenize(string originalWord) {
			originalWord = originalWord.ToLower();
			List<int> result = new List<int>();
			int length = originalWord.Length;
			int startOfWord = 0;
			for (int i = 0; i < length; i++) {
				if (Char.IsLetter(originalWord, i))
					continue;
				int prevWordLength = i - startOfWord;
				if (prevWordLength > 0)
					result.AddRange(HyphenizePart(originalWord, startOfWord, prevWordLength));
				startOfWord = i + 1;
			}
			if (startOfWord == 0)
				return HyphenizeCore(originalWord, 0);
			if (startOfWord < length)
				result.AddRange(HyphenizePart(originalWord, startOfWord, length - startOfWord));
			return result;
		}
		List<int> HyphenizePart(string originalWord, int offset, int length) {
			originalWord = originalWord.Substring(offset, length);
			return HyphenizeCore(originalWord, offset);
		}
		List<int> HyphenizeCore(string originalWord, int offset) {
			List<int> exception;
			if (exceptionTable.TryGetValue(originalWord, out exception))
				return exception;
			extendedWord = "." + originalWord + ".";
			extendedWordLength = extendedWord.Length;
			patternCharsValues = new int[extendedWordLength + 1];
			int maxSubwordLength = Math.Min(extendedWordLength, maxPatternLength);
			for (int i = 0; i < extendedWordLength; i++)
				ApplyPatternToSubwords(maxSubwordLength, i);
			return GetHyphenPositions(originalWord, offset);
		}
	}
	#endregion
	#region EmptyHyphenizer
	public class EmptyHyphenizer : Hyphenizer {
		public EmptyHyphenizer()
			: base(String.Empty, String.Empty) {
		}
		public override List<int> Hyphenize(string originalWord) {
			return new List<int>();
		}
	}
	#endregion
}
