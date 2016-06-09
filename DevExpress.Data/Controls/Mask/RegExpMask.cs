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
using System.IO;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
namespace DevExpress.Data.Mask {
	class PokeableReader {
		List<TextReader> readers = new List<TextReader>();
		public PokeableReader() { }
		public PokeableReader(TextReader firstReader)
			: this() {
			PokeReader(firstReader);
		}
		public void PokeReader(TextReader reader) {
			readers.Insert(0, reader);
		}
		public void Poke(string nextInput) {
			PokeReader(new StringReader(nextInput));
		}
		public int Read() {
			while(readers.Count > 0) {
				int result = readers[0].Read();
				if(result >= 0) {
					return result;
				}
				readers.RemoveAt(0);
			}
			return -1;
		}
		public int Peek() {
			while(readers.Count > 0) {
				int result = readers[0].Peek();
				if(result >= 0) {
					return result;
				}
				readers.RemoveAt(0);
			}
			return -1;
		}
	}
	class Yylex: yyInput {
		readonly PokeableReader reader;
		readonly CultureInfo parseCulture;
		int remembered_token;
		object remembered_value;
		bool inBracketExpression = false;
		bool inBracketExpressionStart = false;
		bool inDupCount = false;
		static string ReadUnicodeCategoryName(PokeableReader reader) {
			int readedChar = reader.Read();
			if(readedChar != '{')
				throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskCurveBracketAfterPpExpected);
			string result = string.Empty;
			for(; ; ) {
				readedChar = reader.Read();
				if(readedChar == -1)
					throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskClosingBracketAfterPpExpected);
				if(readedChar == '}')
					return result;
				result += (char)readedChar;
			}
		}
		static char ReadCharCharCode(PokeableReader reader, int digitsWanted) {
			string readed = string.Empty;
			for(int i = 0; i < digitsWanted; ++i) {
				int readedChar = reader.Read();
				if(readedChar == -1)
					break;
				readed += (char)readedChar;
			}
			int result = int.Parse(readed, NumberStyles.HexNumber);
			return (char)result;
		}
		public bool advance() {
			remembered_value = reader.Read();
			if((int)remembered_value == -1)
				return false;
			remembered_value = (char)(int)remembered_value;
			bool inBracketExpressionStartFlag = false;
			remembered_token = Token.CHAR;
			switch((char)remembered_value) {
				case '\\':
					remembered_value = reader.Read();
					if((int)remembered_value == -1)
						throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashBeforeEndOfMask);
					remembered_value = (char)(int)remembered_value;
					switch((char)remembered_value) {
						case 'd':
							remembered_token = Token.CHARClassDecimalDigit;
							break;
						case 'D':
							remembered_token = Token.CHARClassNonDecimalDigit;
							break;
						case 'w':
							remembered_token = Token.CHARClassWord;
							break;
						case 'W':
							remembered_token = Token.CHARClassNonWord;
							break;
						case 's':
							remembered_token = Token.CHARClassWhiteSpace;
							break;
						case 'S':
							remembered_token = Token.CHARClassNonWhiteSpace;
							break;
						case 'x':
							remembered_value = ReadCharCharCode(reader, 2);
							break;
						case 'u':
							remembered_value = ReadCharCharCode(reader, 4);
							break;
						case 'p':
							remembered_token = Token.CHARClassUnicodeCategory;
							remembered_value = ReadUnicodeCategoryName(reader);
							break;
						case 'P':
							remembered_token = Token.CHARClassUnicodeCategoryNot;
							remembered_value = ReadUnicodeCategoryName(reader);
							break;
						case 'R':
							remembered_value = reader.Read();
							if((int)remembered_value == -1)
								throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashRBeforeEndOfMask);
							remembered_value = (char)(int)remembered_value;
							switch((char)remembered_value) {
								case '.':
									reader.Poke(NamedMasks.GetNumberDecimalSeparator(parseCulture));
									return advance();
								case ':':
									reader.Poke(NamedMasks.GetTimeSeparator(parseCulture));
									return advance();
								case '/':
									reader.Poke(NamedMasks.GetDateSeparator(parseCulture));
									return advance();
								case '{':
									string maskName = string.Empty;
									for(; ; ) {
										int currentToken = reader.Read();
										if(currentToken == -1)
											throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskClosingBracketAfterRExpected);
										if((char)currentToken == '}')
											break;
										maskName += (char)currentToken;
									}
									reader.Poke(NamedMasks.GetNamedMask(maskName, parseCulture));
									return advance();
								default:
									throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidCharAfterBackslashR);
							}
					}
					break;
				case '{':
					if(!inBracketExpression) {
						remembered_token = (char)remembered_value;
						inDupCount = true;
					}
					break;
				case ',':
					if(inDupCount) {
						remembered_token = (char)remembered_value;
					}
					break;
				case '}':
					if(inDupCount) {
						inDupCount = false;
						remembered_token = (char)remembered_value;
					}
					break;
				case '.':
				case '(':
				case ')':
				case '|':
				case '*':
				case '+':
				case '?':
					if(!inBracketExpression)
						remembered_token = (char)remembered_value;
					break;
				case '[':
					if(!inBracketExpression) {
						remembered_token = (char)remembered_value;
						inBracketExpression = true;
						inBracketExpressionStartFlag = true;
					}
					break;
				case ']':
					if(inBracketExpression) {
						remembered_token = (char)remembered_value;
						inBracketExpression = false;
					}
					break;
				case '-':
					if(inBracketExpression && !inBracketExpressionStart && reader.Peek() != ']') {
						remembered_token = (char)remembered_value;
					}
					break;
				case '^':
					if(inBracketExpressionStart) {
						remembered_token = (char)remembered_value;
						inBracketExpressionStartFlag = true;
					}
					break;
				default:
					if((char)remembered_value >= '0' && (char)remembered_value <= '9')
						remembered_token = Token.DIGIT;
					break;
			}
			inBracketExpressionStart = inBracketExpressionStartFlag;
			return true;
		}
		public int token() {
			return remembered_token;
		}
		public Object value() {
			return remembered_value;
		}
		public Yylex(TextReader reader, CultureInfo parseCulture) {
			this.reader = new PokeableReader(reader);
			this.parseCulture = parseCulture;
		}
	}
	public class RegExpMaskLogic {
		Dfa regExp;
		bool IsAutoComplete = true;
		public bool IsValidStart(string text) {
			return regExp.IsValidStart(text);
		}
		public RegExpMaskLogic(Dfa regExp, bool isAutoComplete) {
			this.regExp = regExp;
			this.IsAutoComplete = isAutoComplete;
		}
		public RegExpMaskLogic(string regExp, CultureInfo culture, bool isAutoComplete) : this(Dfa.Parse(regExp, culture), isAutoComplete) { }
		string AutoCompleteResultProcessing(string origin) {
			object[] originPlaceHoldersInfo = regExp.GetPlaceHoldersInfo(origin);
			string result = origin;
			foreach(object ch in originPlaceHoldersInfo) {
				if(ch == null)
					break;
				string candidate = result + (char)ch;
				object[] resultPlaceHoldersInfo = regExp.GetPlaceHoldersInfo(result);
				object[] candidatePlaceHoldersInfo = regExp.GetPlaceHoldersInfo(candidate);
				if(candidatePlaceHoldersInfo.Length != resultPlaceHoldersInfo.Length - 1)
					break;
				bool valid = true;
				for(int i = 0; i < candidatePlaceHoldersInfo.Length; ++i) {
					object a = candidatePlaceHoldersInfo[i];
					object b = resultPlaceHoldersInfo[i + 1];
					if((a == null && b == null) || (a != null && ((char)a).Equals(b)))
						continue;
					valid = false;
					break;
				}
				if(!valid)
					break;
				result = candidate;
			}
			return result;
		}
		string AutoCompleteTailPreprocessing(string head, string tail) {
			string original = head + tail;
			string result = tail;
			while(result.Length > 0) {
				string candidate = StringWithoutLastChar(result);
				if(AutoCompleteResultProcessing(head + candidate) == original) {
					result = candidate;
				} else {
					break;
				}
			}
			if(result == tail)
				return tail;
			string work = SmartAutoComplete(head + result);
			if(work.Length < head.Length)
				return tail;
			if(head.Length + tail.Length <= work.Length)
				return tail;
			result = work.Substring(head.Length);
			return result;
		}
		MaskLogicResult CreateResult(string result, string cursorBase) {
		#region Mask_Tests
#if DEBUGTEST
			Assert.IsTrue(IsValidStart(result));
#endif
		#endregion
			if(IsAutoComplete) {
				result = SmartAutoComplete(result);
				result = AutoCompleteResultProcessing(result);
				cursorBase = SmartAutoComplete(cursorBase);
			}
			return new MaskLogicResult(result, cursorBase.Length);
		}
		string SmartAutoComplete(string before) {
			if(!IsValidStart(before))
				return null;
			string work = StringWithoutLastChar(ExactsTruncate(before));
			if(work != null) {
				AutoCompleteInfo info = regExp.GetAutoCompleteInfo(work);
				if(info.DfaAutoCompleteType == DfaAutoCompleteType.FinalOrExactBeforeNone)
					return work;
			}
			work = ExactsAppend(before);
			return work;
		}
		#region Mask_Tests
#if DEBUGTEST
		void TestAutoCompleteAssert(string input, string autocompleteResult) {
			if(autocompleteResult == null)
				Assert.IsNull(SmartAutoComplete(input));
			else
				Assert.AreEqual(autocompleteResult, SmartAutoComplete(input));
		}
		void TestAutoCompleteAssert(string input) {
			TestAutoCompleteAssert(input, input);
		}
		[Test]
		public void TestSmartAutoComplete() {
			regExp = Dfa.Parse(string.Empty);
			TestAutoCompleteAssert(string.Empty);
			regExp = Dfa.Parse(".*");
			TestAutoCompleteAssert(string.Empty);
			regExp = Dfa.Parse(@"(\(\d\))?\d{2,3}-\d\d-\d");
			TestAutoCompleteAssert(string.Empty);
			TestAutoCompleteAssert("q", null);
			TestAutoCompleteAssert("1");
			TestAutoCompleteAssert("12");
			TestAutoCompleteAssert("12-");
			TestAutoCompleteAssert("12-3");
			TestAutoCompleteAssert("12-34", "12-34-");
			TestAutoCompleteAssert("12-34-");
			TestAutoCompleteAssert("12-34-5");
			TestAutoCompleteAssert("123", "123-");
			TestAutoCompleteAssert("123-");
			TestAutoCompleteAssert("123-4");
			TestAutoCompleteAssert("123-45", "123-45-");
			TestAutoCompleteAssert("123-45-");
			TestAutoCompleteAssert("123-45-6");
			TestAutoCompleteAssert("(", "(");
			TestAutoCompleteAssert("(q", null);
			TestAutoCompleteAssert("(8", "(8)");
			TestAutoCompleteAssert("(8)");
			TestAutoCompleteAssert("(8)123", "(8)123-");
			TestAutoCompleteAssert("(8)123-4");
			TestAutoCompleteAssert("(8)123-45", "(8)123-45-");
			TestAutoCompleteAssert("(8)123-45-6");
			regExp = Dfa.Parse(@"qwe?rty");
			TestAutoCompleteAssert(string.Empty, "qw");
			TestAutoCompleteAssert("qwe", "qwerty");
			TestAutoCompleteAssert("qwr", "qwrty");
			regExp = Dfa.Parse(@"(-.-)*");
			TestAutoCompleteAssert(string.Empty, string.Empty);
			TestAutoCompleteAssert("-", string.Empty);
			TestAutoCompleteAssert("-0", "-0-");
			TestAutoCompleteAssert("-0-", "-0-");
			TestAutoCompleteAssert("-0--", "-0-");
			TestAutoCompleteAssert("-0--1", "-0--1-");
		}
#endif
		#endregion
		string SmartAppend(string before, char input) {
			if(!IsValidStart(before))
				return null;
			string work = SmartAutoComplete(before);
			AutoCompleteInfo info = regExp.GetAutoCompleteInfo(work);
			switch(info.DfaAutoCompleteType) {
				case DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone:
					work += info.AutoCompleteChar;
					if(input != info.AutoCompleteChar)
						work = ExactsAppend(work) + input;
					break;
				case DfaAutoCompleteType.FinalOrExactBeforeFinal:
					work += info.AutoCompleteChar;
					break;
				case DfaAutoCompleteType.FinalOrExactBeforeNone:
					work = ExactsAppend(work + info.AutoCompleteChar) + input;
					break;
				case DfaAutoCompleteType.None:
					work += input;
					break;
				case DfaAutoCompleteType.Final:
					return null;
				default:
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.InternalErrorNonCoveredCase, info.DfaAutoCompleteType.ToString()));
			}
			work = SmartAutoComplete(work);
			return work;
		}
		#region Mask_Tests
#if DEBUGTEST
		void TestSmartAppendAssert(string before, char input, string autocompleteResult) {
			Assert.AreEqual(autocompleteResult, SmartAppend(before, input));
		}
		void TestSmartAppendAssert(string before, char input) {
			TestSmartAppendAssert(before, input, before + input);
		}
		[Test]
		public void TestSmartAppend() {
			regExp = Dfa.Parse(string.Empty);
			TestSmartAppendAssert("", 'q', null);
			TestSmartAppendAssert("q", 'q', null);
			regExp = Dfa.Parse(".");
			TestSmartAppendAssert("", 'q');
			TestSmartAppendAssert("q", 'w', null);
			regExp = Dfa.Parse("qwe?rty");
			TestSmartAppendAssert("qw", 'z', null);
			TestSmartAppendAssert("qw", 'e', "qwerty");
			TestSmartAppendAssert("qw", 'r', "qwrty");
			regExp = Dfa.Parse("(--)?");
			TestSmartAppendAssert("", '-', "--");
			TestSmartAppendAssert("-", '-', null);
			TestSmartAppendAssert("--", '-', null);
			TestSmartAppendAssert("", '+', "--");
			TestSmartAppendAssert("-", '+', null);
			TestSmartAppendAssert("--", '+', null);
			regExp = Dfa.Parse("(--)*");
			TestSmartAppendAssert("", '-', "--");
			TestSmartAppendAssert("-", '-', "----");
			TestSmartAppendAssert("--", '-', "----");
			TestSmartAppendAssert("", '+', "--");
			TestSmartAppendAssert("-", '+', "----");
			TestSmartAppendAssert("--", '+', "----");
			regExp = Dfa.Parse("(-.-)*");
			TestSmartAppendAssert("", '-', "---");
			TestSmartAppendAssert("-", '-', "---");
			TestSmartAppendAssert("", 'q', "-q-");
			TestSmartAppendAssert("-q-", 'w', "-q--w-");
			regExp = Dfa.Parse(@"(-\d-)*");
			TestSmartAppendAssert("", '-', null);
			TestSmartAppendAssert("-", '-', null);
			TestSmartAppendAssert("", '0', "-0-");
			TestSmartAppendAssert("-0-", '1', "-0--1-");
			TestSmartAppendAssert("-0-", '-', null);
		}
		[Test]
		public void TestSmartAppendBug1() {
			regExp = Dfa.Parse(@"(\.\d*)?");
			IsAutoComplete = true;
			TestSmartAppendAssert("", '.');
			TestSmartAppendAssert("", '0', ".0");
		}
#endif
		#endregion
		static string StringWithoutLastChar(string str) {
			return str.Length == 0 ? null : str.Substring(0, str.Length - 1);
		}
		string ExactsTruncate(string before) {
			string work = before;
			for(; work.Length > 0; ) {
				string tmp = StringWithoutLastChar(work);
				if(regExp.GetAutoCompleteInfo(tmp).DfaAutoCompleteType == DfaAutoCompleteType.ExactChar)
					work = tmp;
				else
					break;
			}
			return work;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestExactsTruncate() {
			regExp = Dfa.Parse(string.Empty);
			Assert.AreEqual("", ExactsTruncate(""));
			regExp = Dfa.Parse(".*");
			Assert.AreEqual("", ExactsTruncate(""));
			Assert.AreEqual("q", ExactsTruncate("q"));
			Assert.AreEqual("qw", ExactsTruncate("qw"));
			regExp = Dfa.Parse("qwe?rty");
			Assert.AreEqual("", ExactsTruncate("q"));
			Assert.AreEqual("", ExactsTruncate("qw"));
			Assert.AreEqual("qwe", ExactsTruncate("qwe"));
			Assert.AreEqual("qwr", ExactsTruncate("qwr"));
			Assert.AreEqual("qwe", ExactsTruncate("qwerty"));
			Assert.AreEqual("qwr", ExactsTruncate("qwrty"));
			regExp = Dfa.Parse("(--)?");
			Assert.AreEqual("", ExactsTruncate(""));
			Assert.AreEqual("-", ExactsTruncate("-"));
			Assert.AreEqual("-", ExactsTruncate("--"));
			regExp = Dfa.Parse("(--)*");
			Assert.AreEqual("", ExactsTruncate(""));
			Assert.AreEqual("-", ExactsTruncate("-"));
			Assert.AreEqual("-", ExactsTruncate("--"));
			Assert.AreEqual("---", ExactsTruncate("---"));
			Assert.AreEqual("---", ExactsTruncate("----"));
			regExp = Dfa.Parse("(-.-)*");
			Assert.AreEqual("", ExactsTruncate(""));
			Assert.AreEqual("-", ExactsTruncate("-"));
			Assert.AreEqual("-q", ExactsTruncate("-q"));
			Assert.AreEqual("-q", ExactsTruncate("-q-"));
			Assert.AreEqual("-q--", ExactsTruncate("-q--"));
			Assert.AreEqual("-q--w", ExactsTruncate("-q--w"));
			Assert.AreEqual("-q--w", ExactsTruncate("-q--w-"));
		}
#endif
		#endregion
		string ExactsAppend(string before) {
			string work = before;
			for(; ; ) {
				AutoCompleteInfo info = regExp.GetAutoCompleteInfo(work);
				if(info.DfaAutoCompleteType != DfaAutoCompleteType.ExactChar)
					break;
				work += info.AutoCompleteChar;
			}
			#region Mask_Tests
#if DEBUGTEST
			Assert.IsTrue(IsValidStart(work));
#endif
			#endregion
			return work;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestExactsAppend() {
			regExp = Dfa.Parse(string.Empty);
			Assert.AreEqual("", ExactsAppend(""));
			regExp = Dfa.Parse(".*");
			Assert.AreEqual("", ExactsAppend(""));
			Assert.AreEqual("q", ExactsAppend("q"));
			Assert.AreEqual("qw", ExactsAppend("qw"));
			regExp = Dfa.Parse("qwe?rty");
			Assert.AreEqual("qw", ExactsAppend("q"));
			Assert.AreEqual("qw", ExactsAppend("qw"));
			Assert.AreEqual("qwerty", ExactsAppend("qwe"));
			Assert.AreEqual("qwrty", ExactsAppend("qwr"));
			Assert.AreEqual("qwerty", ExactsAppend("qwerty"));
			Assert.AreEqual("qwrty", ExactsAppend("qwrty"));
			regExp = Dfa.Parse("(--)?");
			Assert.AreEqual("", ExactsAppend(""));
			Assert.AreEqual("--", ExactsAppend("-"));
			Assert.AreEqual("--", ExactsAppend("--"));
			regExp = Dfa.Parse("(--)*");
			Assert.AreEqual("", ExactsAppend(""));
			Assert.AreEqual("--", ExactsAppend("-"));
			Assert.AreEqual("--", ExactsAppend("--"));
			Assert.AreEqual("----", ExactsAppend("---"));
			Assert.AreEqual("----", ExactsAppend("----"));
			regExp = Dfa.Parse("(-.-)*");
			Assert.AreEqual("", ExactsAppend(""));
			Assert.AreEqual("-", ExactsAppend("-"));
			Assert.AreEqual("-q-", ExactsAppend("-q"));
			Assert.AreEqual("-q-", ExactsAppend("-q-"));
			Assert.AreEqual("-q--", ExactsAppend("-q--"));
			Assert.AreEqual("-q--w-", ExactsAppend("-q--w"));
			Assert.AreEqual("-q--w-", ExactsAppend("-q--w-"));
		}
#endif
		#endregion
		string RemoveExacts(string stringBefore, string stringWithExacts) {
			string full = stringBefore + stringWithExacts;
			string work = stringBefore;
			string result = string.Empty;
			if(!IsValidStart(full))
				return null;
			while(work.Length < full.Length) {
				char nextChar = full[work.Length];
				AutoCompleteInfo info = regExp.GetAutoCompleteInfo(work);
				switch(info.DfaAutoCompleteType) {
					case DfaAutoCompleteType.None:
					case DfaAutoCompleteType.FinalOrExactBeforeFinal:
					case DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone:
						result += nextChar;
						break;
					case DfaAutoCompleteType.ExactChar:
					case DfaAutoCompleteType.FinalOrExactBeforeNone:
						break;
					default:
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.InternalErrorNonCoveredCase, info.DfaAutoCompleteType.ToString()));
				}
				work += nextChar;
			}
			#region Mask_Tests
#if DEBUGTEST
			if(stringBefore.Length > 0) {
				Assert.AreEqual(RemoveExacts(string.Empty, full), RemoveExacts(string.Empty, stringBefore) + result);
			}
#endif
			#endregion
			return result;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestRemoveExacts() {
			regExp = Dfa.Parse(string.Empty);
			Assert.AreEqual(null, RemoveExacts("", "q"));
			Assert.AreEqual("", RemoveExacts("", ""));
			Assert.AreEqual(null, RemoveExacts("q", "q"));
			Assert.AreEqual(null, RemoveExacts("q", ""));
			regExp = Dfa.Parse(".");
			Assert.AreEqual("q", RemoveExacts("", "q"));
			Assert.AreEqual("", RemoveExacts("", ""));
			Assert.AreEqual(null, RemoveExacts("q", "q"));
			Assert.AreEqual("", RemoveExacts("q", ""));
			regExp = Dfa.Parse("qwe?rty");
			Assert.AreEqual("e", RemoveExacts("", "qwerty"));
			Assert.AreEqual("r", RemoveExacts("", "qwrty"));
			Assert.AreEqual(null, RemoveExacts("", "qwz"));
			Assert.AreEqual("", RemoveExacts("", "qw"));
			Assert.AreEqual("", RemoveExacts("", "q"));
			Assert.AreEqual("", RemoveExacts("", ""));
			Assert.AreEqual(null, RemoveExacts("q", "qwerty"));
			Assert.AreEqual("e", RemoveExacts("q", "werty"));
			Assert.AreEqual("e", RemoveExacts("qw", "erty"));
			Assert.AreEqual("", RemoveExacts("qwe", "rty"));
			Assert.AreEqual("", RemoveExacts("qwer", "ty"));
			Assert.AreEqual("r", RemoveExacts("q", "wrty"));
			Assert.AreEqual("r", RemoveExacts("qw", "rty"));
			Assert.AreEqual("", RemoveExacts("qwr", "ty"));
			Assert.AreEqual("", RemoveExacts("qwr", "t"));
			Assert.AreEqual("", RemoveExacts("qwr", ""));
			regExp = Dfa.Parse("(--)?");
			Assert.AreEqual(null, RemoveExacts("", "---"));
			Assert.AreEqual("-", RemoveExacts("", "--"));
			Assert.AreEqual("-", RemoveExacts("", "-"));
			Assert.AreEqual("", RemoveExacts("", ""));
			Assert.AreEqual("", RemoveExacts("-", "-"));
			Assert.AreEqual("", RemoveExacts("-", ""));
			Assert.AreEqual("", RemoveExacts("--", ""));
			regExp = Dfa.Parse("(--)*");
			Assert.AreEqual("---", RemoveExacts("", "-----"));
			Assert.AreEqual("--", RemoveExacts("", "----"));
			Assert.AreEqual("--", RemoveExacts("", "---"));
			Assert.AreEqual("-", RemoveExacts("", "--"));
			Assert.AreEqual("-", RemoveExacts("", "-"));
			Assert.AreEqual("", RemoveExacts("", ""));
			Assert.AreEqual("--", RemoveExacts("-", "----"));
			Assert.AreEqual("-", RemoveExacts("-", "---"));
			Assert.AreEqual("-", RemoveExacts("-", "--"));
			Assert.AreEqual("", RemoveExacts("-", "-"));
			Assert.AreEqual("", RemoveExacts("-", ""));
			Assert.AreEqual("--", RemoveExacts("--", "---"));
			Assert.AreEqual("-", RemoveExacts("--", "--"));
			Assert.AreEqual("-", RemoveExacts("--", "-"));
			Assert.AreEqual("", RemoveExacts("--", ""));
			Assert.AreEqual("-", RemoveExacts("---", "--"));
			Assert.AreEqual("", RemoveExacts("---", "-"));
			Assert.AreEqual("", RemoveExacts("---", ""));
			Assert.AreEqual("-", RemoveExacts("----", "-"));
			Assert.AreEqual("", RemoveExacts("----", ""));
			Assert.AreEqual("", RemoveExacts("-----", ""));
			regExp = Dfa.Parse("(-.-)*");
			Assert.AreEqual("-", RemoveExacts("", "---"));
			Assert.AreEqual("q", RemoveExacts("", "-q-"));
			Assert.AreEqual("qw", RemoveExacts("", "-q--w-"));
			Assert.AreEqual("qw", RemoveExacts("", "-q--w--"));
			Assert.AreEqual("qwe", RemoveExacts("", "-q--w--e"));
			Assert.AreEqual("qwe", RemoveExacts("", "-q--w--e-"));
			Assert.AreEqual("-", RemoveExacts("-", "--"));
			Assert.AreEqual("q", RemoveExacts("-", "q-"));
			Assert.AreEqual("qw", RemoveExacts("-", "q--w-"));
			Assert.AreEqual("qw", RemoveExacts("-", "q--w--"));
			Assert.AreEqual("qwe", RemoveExacts("-", "q--w--e"));
			Assert.AreEqual("qwe", RemoveExacts("-", "q--w--e-"));
			Assert.AreEqual("", RemoveExacts("--", "-"));
			Assert.AreEqual("", RemoveExacts("-q", "-"));
			Assert.AreEqual("w", RemoveExacts("-q", "--w-"));
			Assert.AreEqual("w", RemoveExacts("-q", "--w--"));
			Assert.AreEqual("we", RemoveExacts("-q", "--w--e"));
			Assert.AreEqual("we", RemoveExacts("-q", "--w--e-"));
			Assert.AreEqual("", RemoveExacts("---", ""));
			Assert.AreEqual("", RemoveExacts("-q-", ""));
			Assert.AreEqual("w", RemoveExacts("-q-", "-w-"));
			Assert.AreEqual("w", RemoveExacts("-q-", "-w--"));
			Assert.AreEqual("we", RemoveExacts("-q-", "-w--e"));
			Assert.AreEqual("we", RemoveExacts("-q-", "-w--e-"));
			Assert.AreEqual("w", RemoveExacts("-q--", "w-"));
			Assert.AreEqual("w", RemoveExacts("-q--", "w--"));
			Assert.AreEqual("we", RemoveExacts("-q--", "w--e"));
			Assert.AreEqual("we", RemoveExacts("-q--", "w--e-"));
			Assert.AreEqual("", RemoveExacts("-q--w", "-"));
			Assert.AreEqual("", RemoveExacts("-q--w", "--"));
			Assert.AreEqual("e", RemoveExacts("-q--w", "--e"));
			Assert.AreEqual("e", RemoveExacts("-q--w", "--e-"));
		}
#endif
		#endregion
		string RestoreExacts(string stringBefore, string stringWithoutExacts) {
			if(!IsValidStart(stringBefore))
				return null;
			string work = SmartAutoComplete(stringBefore);
			if(work == null)
				return null;
			foreach(char input in stringWithoutExacts) {
				work = SmartAppend(work, input);
				if(work == null)
					return null;
			}
			if(work.Length < stringBefore.Length)
				return null;
			return work;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestRestoreExacts() {
			regExp = Dfa.Parse(string.Empty);
			Assert.AreEqual(null, RestoreExacts("", "q"));
			Assert.AreEqual("", RestoreExacts("", ""));
			Assert.AreEqual(null, RestoreExacts("q", "q"));
			Assert.AreEqual(null, RestoreExacts("q", ""));
			regExp = Dfa.Parse(".");
			Assert.AreEqual("q", RestoreExacts("", "q"));
			Assert.AreEqual("", RestoreExacts("", ""));
			Assert.AreEqual(null, RestoreExacts("q", "q"));
			Assert.AreEqual("q", RestoreExacts("q", ""));
			regExp = Dfa.Parse("qwe?rty");
			Assert.AreEqual("qw", RestoreExacts("", ""));
			Assert.AreEqual(null, RestoreExacts("", "z"));
			Assert.AreEqual("qwerty", RestoreExacts("", "e"));
			Assert.AreEqual("qwrty", RestoreExacts("", "r"));
			Assert.AreEqual(null, RestoreExacts("", "er"));
			Assert.AreEqual(null, RestoreExacts("qw", "z"));
			Assert.AreEqual("qwerty", RestoreExacts("qw", "e"));
			Assert.AreEqual("qwrty", RestoreExacts("qw", "r"));
			Assert.AreEqual(null, RestoreExacts("qw", "er"));
			regExp = Dfa.Parse("(--)?");
			Assert.AreEqual("", RestoreExacts("", ""));
			Assert.AreEqual("--", RestoreExacts("", "-"));
			Assert.AreEqual("--", RestoreExacts("", "+"));
			Assert.AreEqual(null, RestoreExacts("", "--"));
			Assert.AreEqual("--", RestoreExacts("-", ""));
			Assert.AreEqual("--", RestoreExacts("--", ""));
			regExp = Dfa.Parse("(--)*");
			Assert.AreEqual("", RestoreExacts("", ""));
			Assert.AreEqual("--", RestoreExacts("", "-"));
			Assert.AreEqual("--", RestoreExacts("", "+"));
			Assert.AreEqual("----", RestoreExacts("", "-+"));
			Assert.AreEqual("------", RestoreExacts("", "---"));
			Assert.AreEqual("----", RestoreExacts("--", "-"));
			Assert.AreEqual("----", RestoreExacts("-", "-"));
			regExp = Dfa.Parse("(-.-)*");
			Assert.AreEqual("", RestoreExacts("", ""));
			Assert.AreEqual("---", RestoreExacts("", "-"));
			Assert.AreEqual("-q-", RestoreExacts("", "q"));
			Assert.AreEqual("-q--w-", RestoreExacts("", "qw"));
			Assert.AreEqual(null, RestoreExacts("-", ""));
			Assert.AreEqual("---", RestoreExacts("-", "-"));
			Assert.AreEqual("-q-", RestoreExacts("-", "q"));
			Assert.AreEqual("-q--w-", RestoreExacts("-", "qw"));
			Assert.AreEqual("-0-", RestoreExacts("-0", ""));
			Assert.AreEqual("-0----", RestoreExacts("-0", "-"));
			Assert.AreEqual("-0--q-", RestoreExacts("-0", "q"));
			Assert.AreEqual("-0--q--w-", RestoreExacts("-0", "qw"));
		}
#endif
		#endregion
		public string GetMaskedText(string text, char anySymbolHolder) {
			string mask = regExp.GetPlaceHolders(text, anySymbolHolder);
			if(mask.Length == 0)
				return text;
			return text + ' ' + mask;
		}
		public bool IsMatch(string text) {
			return regExp.IsMatch(text);
		}
		public bool IsFinal(string text) {
			return regExp.IsFinal(text);
		}
		string GetTail(string oldHead, string oldTail, string head) {
			string tailNonexacts = RemoveExacts(oldHead, oldTail);
			string work;
			if(IsAutoComplete) {
				work = RestoreExacts(head, tailNonexacts);
				if(work != null)
					return work;
			}
			work = head + oldTail;
			if(IsValidStart(work))
				return work;
			if(!IsAutoComplete) {
				work = RestoreExacts(head, tailNonexacts);
				if(work != null)
					return work;
			}
			if(oldTail.Length > 0) {
				work = head + oldTail.Substring(1);
				if(IsValidStart(work))
					return work;
			}
			if(tailNonexacts.Length > 0) {
				work = RestoreExacts(head, tailNonexacts.Substring(1));
				if(work != null)
					return work;
			}
			return null;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("//TODO")]
		[Test]
		public void TestGetTail() { }
#endif
		#endregion
		MaskLogicResult InsertCaseTailPlain(string appended, string tail) {
			if(appended == null)
				return null;
			string work = appended + tail;
			if(IsValidStart(work))
				return CreateResult(work, appended);
			else
				return null;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("//TODO")]
		[Test]
		public void TestInsertCaseTailPlain() { }
#endif
		#endregion
		MaskLogicResult InsertCaseTailNonExacts(string appended, string tailNonexacts) {
			if(appended == null)
				return null;
			string work = RestoreExacts(appended, tailNonexacts);
			if(work != null)
				return CreateResult(work, appended);
			else
				return null;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("//TODO")]
		[Test]
		public void TestInsertCaseTailNonExacts() { }
#endif
		#endregion
		bool InsertCaseByOneCharBody(out string nextHead, out string nextFull, bool isSmart, string head, string replaced, char input, string tail) {
			nextFull = null;
			if(isSmart) {
				nextHead = SmartAppend(head, input);
				if(nextHead == null)
					return false;
			} else {
				nextHead = head + input;
			}
			nextFull = GetTail(head + replaced, tail, nextHead);
			return nextFull != null;
		}
		MaskLogicResult InsertCaseByOneChar(bool isSmart, string head, string replaced, string inserted, string tail) {
			return InsertCaseByOneChar(isSmart, head, replaced, inserted, tail, false);
		}
		MaskLogicResult InsertCaseByOneChar(bool isSmart, string head, string replaced, string inserted, string tail, bool isLastChance) {
			string currentHead = head;
			string currentTail = tail;
			string currentReplaced = replaced;
			foreach(char inputChar in inserted) {
				string nextHead, nextFull;
				if(!InsertCaseByOneCharBody(out nextHead, out nextFull, isSmart, currentHead, currentReplaced, inputChar, currentTail)) {
					char inputCharChanged = Char.ToUpper(inputChar);
					if(inputCharChanged == inputChar)
						inputCharChanged = Char.ToLower(inputChar);
					if(inputCharChanged == inputChar) {
						if(isLastChance)
							continue;
						return null;
					}
					if(!InsertCaseByOneCharBody(out nextHead, out nextFull, isSmart, currentHead, currentReplaced, inputCharChanged, currentTail)) {
						if(isLastChance)
							continue;
						return null;
					}
				}
				currentHead = nextHead;
				currentReplaced = string.Empty;
				currentTail = nextFull.Substring(Math.Min(nextHead.Length, nextFull.Length));
			}
			if(!IsValidStart(currentHead + currentTail))
				return null;
			return CreateResult(currentHead + currentTail, currentHead);
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("//TODO")]
		[Test]
		public void TestInsertCaseTailByOneChar() { }
#endif
		#endregion
		public MaskLogicResult GetReplaceResult(string head, string replaced, string tail, string inserted) {
			if(!IsValidStart(head + replaced + tail))
				return null;
			if(IsAutoComplete) {
				tail = AutoCompleteTailPreprocessing(head + replaced, tail);
			}
			string fullPrevious = head + replaced + tail;
			if(inserted == replaced)
				return CreateResult(fullPrevious, head + inserted);
			string insertedAsPlain = head + inserted;
			if(!IsValidStart(insertedAsPlain))
				insertedAsPlain = null;
			string tailNonExacts = RemoveExacts(head + replaced, tail);
			string insertedAsInput = RestoreExacts(head, inserted);
			MaskLogicResult result;
			if(IsAutoComplete && inserted.Length > 1) {
				result = InsertCaseTailNonExacts(insertedAsPlain, tailNonExacts);
				if(result != null)
					return result;
			}
			if(IsAutoComplete) {
				result = InsertCaseTailNonExacts(insertedAsInput, tailNonExacts);
				if(result != null)
					return result;
			}
			if(!IsAutoComplete || inserted.Length > 1) {
				result = InsertCaseTailPlain(insertedAsPlain, tail);
				if(result != null)
					return result;
			}
			if(IsAutoComplete || inserted.Length > 1) {
				result = InsertCaseTailPlain(insertedAsInput, tail);
				if(result != null)
					return result;
			}
			if(!IsAutoComplete) {
				result = InsertCaseTailNonExacts(insertedAsPlain, tailNonExacts);
				if(result != null)
					return result;
			}
			if(!IsAutoComplete && inserted.Length > 1) {
				result = InsertCaseTailNonExacts(insertedAsInput, tailNonExacts);
				if(result != null)
					return result;
			}
			if(inserted.Length > 0 && (!IsAutoComplete || inserted.Length > 1)) {
				result = InsertCaseByOneChar(false, head, replaced, inserted, tail);
				if(result != null)
					return result;
			}
			if(inserted.Length > 0 && (IsAutoComplete || inserted.Length > 1)) {
				result = InsertCaseByOneChar(true, head, replaced, inserted, tail);
				if(result != null)
					return result;
			}
			if(IsAutoComplete && replaced.Length == 0 && inserted.Length == 1) {
				string completedPrevious = SmartAutoComplete(fullPrevious);
				if(completedPrevious != null && completedPrevious != fullPrevious) {
					return CreateResult(completedPrevious, head);
				}
			}
			if(inserted.Length > 1) {
				result = InsertCaseByOneChar(true, head, replaced, inserted, tail, true);
				if(result != null && fullPrevious != result.EditText)
					return result;
			}
			return null;
		}
		#region Mask_Tests
#if DEBUGTEST
		void TestAppendAssertCore(string text, string input, string expected) {
			TestInsertAssertCore(text, input, string.Empty, expected, expected != null ? expected.Length : -1);
		}
		void TestAppendAssert(string text, string input, string autocompleteResult, string nonAutocompleteResult) {
			IsAutoComplete = true;
			TestAppendAssertCore(text, input, autocompleteResult);
			IsAutoComplete = false;
			TestAppendAssertCore(text, input, nonAutocompleteResult);
		}
		void TestAppendAssert(string text, string input, string autocompleteResult) {
			TestAppendAssert(text, input, autocompleteResult, text + input);
		}
		void TestAppendAssert(string text, string input) {
			TestAppendAssert(text, input, text + input);
		}
		[Test]
		public void TestAppend() {
			regExp = Dfa.Parse(string.Empty);
			TestAppendAssert("", "", "");
			TestAppendAssert("q", "", null, null);
			TestAppendAssert("", "w", null, null);
			TestAppendAssert("q", "w", null, null);
			TestAppendAssert("", "we", null, null);
			TestAppendAssert("q", "we", null, null);
			regExp = Dfa.Parse(".*");
			TestAppendAssert("", "", "");
			TestAppendAssert("q", "");
			TestAppendAssert("", "w");
			TestAppendAssert("q", "w");
			TestAppendAssert("", "we");
			TestAppendAssert("q", "we");
			regExp = Dfa.Parse("qwe?rty");
			TestAppendAssert("qw", "z", null, null);
			TestAppendAssert("qw", "e", "qwerty");
			TestAppendAssert("qw", "er", "qwerty");
			TestAppendAssert("qw", "r", "qwrty");
			TestAppendAssert("qw", "rt", "qwrty");
			TestAppendAssert("qw", "erty", "qwerty");
			regExp = Dfa.Parse("(--)?");
			TestAppendAssert("", "-", "--");
			TestAppendAssert("-", "-", "--");
			TestAppendAssert("--", "-", null, null);
			TestAppendAssert("", "+", "--", null);
			TestAppendAssert("-", "+", "--", null);
			TestAppendAssert("--", "+", null, null);
			TestAppendAssert("", "--");
			TestAppendAssert("", "++", "--", "--");
			regExp = Dfa.Parse("(--)*");
			TestAppendAssert("", "-", "--");
			TestAppendAssert("-", "-", "----");
			TestAppendAssert("--", "-", "----");
			TestAppendAssert("", "+", "--", null);
			TestAppendAssert("-", "+", "----", null);
			TestAppendAssert("--", "+", "----", null);
			TestAppendAssert("", "--");
			TestAppendAssert("-", "--", "----");
			TestAppendAssert("--", "--");
			TestAppendAssert("---", "--", "------");
			TestAppendAssert("", "+-", "----", "----");
			TestAppendAssert("-", "+-", "------", "------");
			TestAppendAssert("--", "+-", "------", "------");
			TestAppendAssert("---", "+-", "--------", "--------");
			regExp = Dfa.Parse("(-.-)*");
			TestAppendAssert("", "-", "---");
			TestAppendAssert("-", "-", "---");
			TestAppendAssert("", "q", "-q-", null);
			TestAppendAssert("-q-", "w", "-q--w-", null);
			TestAppendAssert("", "-q-");
			TestAppendAssert("", "qwe", "-q--w--e-", "-q--w--e-");
			regExp = Dfa.Parse(@"(-\d-)*");
			TestAppendAssert("", "-", null, "-");
			TestAppendAssert("", "-0", "-0-");
			TestAppendAssert("-", "-", "", null);
			TestAppendAssert("", "0", "-0-", null);
		}
		void TestInsertAssertCore(string head, string inserted, string tail, string expected, int expectedPosition) {
			TestReplaceAssertCore(head, string.Empty, inserted, tail, expected, expectedPosition);
		}
		void TestInsertAssert(string head, string inserted, string tail, string autocompleteResult, int autoCompleteCursorPos, string nonAutocompleteResult, int nonAutoCompleteCursorPos) {
			IsAutoComplete = true;
			TestInsertAssertCore(head, inserted, tail, autocompleteResult, autoCompleteCursorPos);
			IsAutoComplete = false;
			TestInsertAssertCore(head, inserted, tail, nonAutocompleteResult, nonAutoCompleteCursorPos);
		}
		void TestInsertAssert(string head, string inserted, string tail, string autocompleteResult, int autocompleteCursorPos) {
			TestInsertAssert(head, inserted, tail, autocompleteResult, autocompleteCursorPos, head + inserted + tail, (head + inserted).Length);
		}
		void TestInsertAssert(string head, string inserted, string tail) {
			TestInsertAssert(head, inserted, tail, head + inserted + tail, (head + inserted).Length);
		}
		[Test]
		public void TestInsert() {
			regExp = Dfa.Parse(string.Empty);
			TestInsertAssert("", "", "");
			TestInsertAssert("", "", "e", null, -1, null, -1);
			TestInsertAssert("", "", "ty", null, -1, null, -1);
			TestInsertAssert("qw", "", "ty", null, -1, null, -1);
			TestInsertAssert("qw", "er", "ty", null, -1, null, -1);
			regExp = Dfa.Parse(".*");
			TestInsertAssert("", "", "");
			TestInsertAssert("", "", "e");
			TestInsertAssert("", "", "ty");
			TestInsertAssert("qw", "", "ty");
			TestInsertAssert("qw", "er", "ty");
			regExp = Dfa.Parse("qwe?rty");
			TestInsertAssert("", "", "", "qwrty", 2);
			TestInsertAssert("qw", "e", "rty", "qwerty", 6);
			TestInsertAssert("qw", "z", "rty", null, -1, null, -1);
			TestInsertAssert("qw", "r", "rty", "qwrty", 5, "qwrty", 3);
			TestInsertAssert("qw", "e", "rty", "qwerty", 6, "qwerty", 3);
			regExp = Dfa.Parse("(--)?");
			TestInsertAssert("", "", "");
			TestInsertAssert("-", "", "", "--", 2);
			TestInsertAssert("", "-", "", "--", 2);
			TestInsertAssert("", "", "-", "--", 0);
			TestInsertAssert("", "-", "-", "--", 2);
			TestInsertAssert("-", "", "-", "--", 2);
			TestInsertAssert("-", "-", "", "--", 2);
			TestInsertAssert("-", "-", "-", null, -1, "--", 2);
			TestInsertAssert("--", "", "");
			TestInsertAssert("", "--", "");
			TestInsertAssert("", "", "--", "--", 0);
			TestInsertAssert("", "+", "", "--", 2, null, -1);
			TestInsertAssert("", "+", "-", "--", 2, null, -1);
			TestInsertAssert("-", "+", "", "--", 2, null, -1);
			TestInsertAssert("-", "+", "-", null, -1, null, -1);
			TestInsertAssert("", "++", "", "--", 2, "--", 2);
			regExp = Dfa.Parse("(--)*");
			TestInsertAssert("", "", "");
			TestInsertAssert("-", "", "", "--", 2);
			TestInsertAssert("", "-", "", "--", 2);
			TestInsertAssert("", "", "-", "--", 0);
			TestInsertAssert("", "-", "-", "----", 2);
			TestInsertAssert("-", "", "-", "--", 2);
			TestInsertAssert("-", "-", "", "----", 4);
			TestInsertAssert("-", "-", "-", "----", 4);
			TestInsertAssert("--", "", "");
			TestInsertAssert("", "--", "");
			TestInsertAssert("", "", "--", "--", 0);
			TestInsertAssert("", "+", "", "--", 2, null, -1);
			TestInsertAssert("", "+", "-", "----", 2, null, -1);
			TestInsertAssert("-", "+", "", "----", 4, null, -1);
			TestInsertAssert("-", "+", "-", "----", 4, null, -1);
			TestInsertAssert("", "++", "", "----", 4, "----", 4);
			regExp = Dfa.Parse("(-.-)*");
			TestInsertAssert("", "", "");
			TestInsertAssert("-", "", "", "", 0);
			TestInsertAssert("", "-", "", "---", 3);
			TestInsertAssert("", "", "-", "", 0);
			TestInsertAssert("", "-", "-", "---", 3);
			TestInsertAssert("-", "", "-", "---", 0);
			TestInsertAssert("-", "-", "", "---", 3);
			TestInsertAssert("-", "-", "-", "------", 3);
			TestInsertAssert("--", "", "", "---", 3);
			TestInsertAssert("", "--", "", "---", 3);
			TestInsertAssert("", "", "--", "---", 0);
			TestInsertAssert("", "+", "", "-+-", 3, null, -1);
			TestInsertAssert("", "+", "-", "-+-", 3, null, -1);
			TestInsertAssert("-", "+", "", "-+-", 3);
			TestInsertAssert("-", "+", "-", "-+----", 3);
			TestInsertAssert("", "++", "", "-+--+-", 6, "-+--+-", 6);
			TestInsertAssert("-q-", "", "");
			TestInsertAssert("", "-q-", "");
			TestInsertAssert("", "", "-q-", "-q-", 0);
			TestInsertAssert("-q-", "-w-", "");
			TestInsertAssert("", "-q-", "-w-", "-q--w-", 3);
			TestInsertAssert("-q-", "", "-w-", "-q--w-", 3);
			TestInsertAssert("-q-", "-w-", "-e-", "-q--w--e-", 6);
			TestInsertAssert("-q-", "e", "-w-", "-q--e--w-", 6, null, -1);
			TestInsertAssert("-q-", "er", "-w-", "-q--e--r--w-", 9, "-q--e--r--w-", 9);
			TestInsertAssert("-q-", "-", "-w-", "-q-----w-", 6, "-q--w-", 4);
			TestInsertAssert("-", "w", "q-", "-w--q-", 3, "-w--q-", 2);
			TestInsertAssert("-", "we", "q-", "-w--e--q-", 6, "-w--e--q-", 6);
			regExp = Dfa.Parse(@"(-\d-)*");
			TestInsertAssert("", "-", "-7-", null, -1, "-7-", 1);
			TestInsertAssert("", "+", "-7-", null, -1, null, -1);
			TestInsertAssert("", "-0", "-7-", "-0--7-", 3, "-0--7-", 2);
			TestInsertAssert("", "0-", "-7-", "-0--7-", 3, "-0--7-", 3);
			TestInsertAssert("", "123", "-7-", "-1--2--3--7-", 9, "-1--2--3--7-", 9);
			TestInsertAssert("", "1-2-3", "-7-", "-1--2--3--7-", 9, "-1--2--3--7-", 9);
		}
		void TestReplaceAssertCore(string head, string replaced, string inserted, string tail, string expected, int expectedPosition) {
			MaskLogicResult result = GetReplaceResult(head, replaced, tail, inserted);
			if(result != null)
				Assert.IsNotNull(result.EditText);
			string helper = (IsAutoComplete ? "A" : "P") + " '" + head + "' - '" + replaced + "' + '" + inserted + "' + '" + tail + "' expected " + (expected == null ? "null" : "'" + expected.Substring(0, expectedPosition) + "|" + expected.Substring(expectedPosition) + "'");
			Assert.AreEqual(expected, result == null ? null : result.EditText, helper);
			if(expected != null) {
				Assert.AreEqual(expectedPosition, result.CursorPosition, "Unexpected cursor pos " + helper);
				Assert.IsTrue(IsValidCursorPosition(expected, expectedPosition), "Invalid cursor pos " + helper);
			}
		}
		void TestReplaceAssert(string head, string replaced, string inserted, string tail, string autocompleteResult, int autoCompleteCursorPos, string nonAutocompleteResult, int nonAutoCompleteCursorPos) {
			IsAutoComplete = true;
			TestReplaceAssertCore(head, replaced, inserted, tail, autocompleteResult, autoCompleteCursorPos);
			IsAutoComplete = false;
			TestReplaceAssertCore(head, replaced, inserted, tail, nonAutocompleteResult, nonAutoCompleteCursorPos);
		}
		void TestReplaceAssert(string head, string replaced, string inserted, string tail, string autocompleteResult, int autocompleteCursorPos) {
			TestReplaceAssert(head, replaced, inserted, tail, autocompleteResult, autocompleteCursorPos, head + inserted + tail, (head + inserted).Length);
		}
		void TestReplaceAssert(string head, string replaced, string inserted, string tail) {
			TestReplaceAssert(head, replaced, inserted, tail, head + inserted + tail, (head + inserted).Length);
		}
		[Ignore("//TODO")]
		[Test]
		public void TestDelete() { }
		[Ignore("//TODO")]
		[Test]
		public void TestReplace() { }
		[Test]
		public void TestReplaceResultDeleteBug1() {
			regExp = Dfa.Parse("Апрель|Август|Декабрь");
			TestReplaceAssert("", "А", "", "вгуст", null, -1, null, -1);
		}
		[Test]
		public void TestReplaceResultAutoCompleteDecomplete() {
			regExp = Dfa.Parse(@"(\(\d+\))?\d{3}-\d\d-\d\d");
			TestReplaceAssert("", "", "(", "");
			TestReplaceAssert("(", "", "0", "", "(0)", 2);
			TestReplaceAssert("(", "0", "", ")", "(", 1, null, -1);
			TestReplaceAssert("(0", "0", "", ")");
			TestReplaceAssert("", "(", "", "1234567)", "123-45-67", 0, null, -1);
		}
		[Test]
		public void TestInsertBug1() {
			regExp = Dfa.Parse(@"(\.\d*)?");
			IsAutoComplete = true;
			TestInsertAssert("", ".", "");
		}
#endif
		#endregion
		public MaskLogicResult GetBackspaceResult(string head, string tail) {
			if(!IsAutoComplete)
				return head.Length == 0 ? null : GetReplaceResult(StringWithoutLastChar(head), head.Substring(head.Length - 1), tail, string.Empty);
			else {
				if(head.Length > 0) {
					string dummyBackspacedHead = head.Substring(0, head.Length - 1);
					string dummyBackspacedString = dummyBackspacedHead + tail;
					if(IsMatch(dummyBackspacedString))
						return CreateResult(dummyBackspacedString, dummyBackspacedHead);
				}
				string headExacts = RemoveExacts(string.Empty, head);
				headExacts = StringWithoutLastChar(headExacts);
				if(headExacts == null)
					return null;
				string newHead = RestoreExacts(string.Empty, headExacts);
				return GetReplaceResult(newHead, head.Substring(newHead.Length), tail, string.Empty);
			}
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestGetBackspaceResultBug1() {
			regExp = Dfa.Parse(@"(\.\d*)?");
			IsAutoComplete = true;
			Assert.AreEqual(".0", GetBackspaceResult(".01", "").EditText);
			Assert.AreEqual(".", GetBackspaceResult(".0", "").EditText);
			Assert.AreEqual("", GetBackspaceResult(".", "").EditText);
		}
		[Test]
		public void TestGetBackspaceResultRequestedBehavior1() {
			regExp = Dfa.Parse(@"\d?\d:\d\d");
			IsAutoComplete = true;
			Assert.AreEqual("1:34", GetBackspaceResult("12", ":34").EditText);
		}
#endif
		#endregion
		public MaskLogicResult GetDeleteResult(string head, string tail) {
			if(!IsAutoComplete)
				return tail.Length == 0 ? null : GetReplaceResult(head, tail.Substring(0, 1), tail.Substring(1), string.Empty);
			else {
				if(tail.Length > 0) {
					string dummyDeletedString = head + tail.Substring(1);
					if(IsMatch(dummyDeletedString))
						return CreateResult(dummyDeletedString, head);
				}
				string tailExacts = RemoveExacts(head, tail);
				if(tailExacts.Length == 0)
					return null;
				string backspaced = RestoreExacts(head, tailExacts.Substring(0, 1));
				if(!(head + tail).StartsWith(backspaced))
					return null;
				int deletedLength = backspaced.Length - head.Length;
				return GetReplaceResult(head, tail.Substring(0, deletedLength), tail.Substring(deletedLength), string.Empty);
			}
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestGetDeleteResultRequestedBehavior1() {
			regExp = Dfa.Parse(@"\d?\d:\d\d");
			IsAutoComplete = true;
			Assert.AreEqual("2:34", GetDeleteResult("", "12:34").EditText);
		}
		[Test]
		public void TestGetDeleteResultBug19814() {
			regExp = Dfa.Parse(@"today|tomorrow|yesterday");
			IsAutoComplete = true;
			Assert.IsNull(GetDeleteResult("", "t"));
		}
		[Test]
		public void TestGetDeleteResultQ97889() {
			regExp = Dfa.Parse(@"(\(\d{3}\) )?\d{3}-\d{4}");
			IsAutoComplete = true;
			Assert.AreEqual("123-457", GetDeleteResult("123-45", "67").EditText);
		}
#endif
		#endregion
		public bool IsValidCursorPosition(string text, int testedPositionInEditText) {
			if(testedPositionInEditText < 0 || testedPositionInEditText > text.Length)
				return false;
			if(!IsAutoComplete)
				return true;
			string work = text.Substring(0, testedPositionInEditText);
			return work == SmartAutoComplete(work);
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void IsValidCursorPositionBug1() {
			regExp = Dfa.Parse(@"((-.-)+(-\d-)+)?");
			IsAutoComplete = true;
			Assert.IsTrue(IsValidCursorPosition("-9--9--i--9-", 0));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 1));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 2));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 3));
			Assert.IsTrue(IsValidCursorPosition("-9--9--i--9-", 4));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 5));
			Assert.IsTrue(IsValidCursorPosition("-9--9--i--9-", 6));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 7));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 8));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 9));
			Assert.IsTrue(IsValidCursorPosition("-9--9--i--9-", 10));
			Assert.IsTrue(!IsValidCursorPosition("-9--9--i--9-", 11));
			Assert.IsTrue(IsValidCursorPosition("-9--9--i--9-", 12));
		}
#endif
		#endregion
		public string OptimisticallyExpand(string baseText) {
			string result = baseText + regExp.GetOptimisticHint(baseText);
			if(IsMatch(result))
				return result;
			return baseText;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("//TODO")]
		[Test]
		public void TestOptimisticallyExpand() { }
#endif
		#endregion
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class RegExpMaskLogicTests: RegExpMaskLogic {
		public RegExpMaskLogicTests() : base(string.Empty, CultureInfo.InvariantCulture, false) { }
	}
#endif
	#endregion
	public class RegExpMaskManagerCore: MaskManagerCommon<MaskManagerPlainTextState> {
		RegExpMaskLogic logic;
		bool showPlaceHolders;
		char anySymbolPlaceHolder;
		bool isOptimistic;
		public RegExpMaskManagerCore(string regExp, bool reverseDfa, bool isAutoComplete, bool isOptimistic, bool showPlaceHolders, char anySymbolPlaceHolder, CultureInfo managerCultureInfo)
			: base(MaskManagerPlainTextState.Empty) {
			this.logic = new RegExpMaskLogic(Dfa.Parse(regExp, reverseDfa, managerCultureInfo), isAutoComplete);
			this.isOptimistic = isOptimistic;
			this.showPlaceHolders = showPlaceHolders;
			this.anySymbolPlaceHolder = anySymbolPlaceHolder;
		}
		public override bool IsEditValueAssignedAsFormattedText {
			get { return true; }
		}
		protected override string GetEditText(MaskManagerPlainTextState state) {
			return state.EditText;
		}
		protected override string GetDisplayText(MaskManagerPlainTextState state) {
			string editText = GetEditText(state);
			return showPlaceHolders ? logic.GetMaskedText(editText, anySymbolPlaceHolder) : editText;
		}
		protected override int GetCursorPosition(MaskManagerPlainTextState state) {
			return state.CursorPosition;
		}
		protected override int GetSelectionAnchor(MaskManagerPlainTextState state) {
			return state.SelectionAnchor;
		}
		public override bool Insert(string insertion) {
			StateChangeType changeType = (insertion.Length == 0 && IsSelection) ? StateChangeType.Delete : StateChangeType.Insert;
			string head = GetCurrentEditText().Substring(0, DisplaySelectionStart);
			string replaced = GetCurrentEditText().Substring(DisplaySelectionStart, DisplaySelectionLength);
			string tail = GetCurrentEditText().Substring(DisplaySelectionEnd);
			MaskLogicResult result = logic.GetReplaceResult(head, replaced, tail, insertion);
			if(result == null)
				return false;
			MaskManagerPlainTextState resultState;
			if(isOptimistic && changeType == StateChangeType.Insert && tail.Length == 0) {
				string expandedResultText = logic.OptimisticallyExpand(result.EditText);
				resultState = CreateStateForApply(expandedResultText, result.CursorPosition, expandedResultText.Length);
			} else {
				resultState = CreateStateForApply(result.EditText, result.CursorPosition, result.CursorPosition);
			}
			return Apply(resultState, changeType);
		}
		protected override MaskManagerPlainTextState CreateStateForApply(string editText, int cursorPosition, int selectionAnchor) {
			return new MaskManagerPlainTextState(editText, cursorPosition, selectionAnchor);
		}
		protected override MaskManagerPlainTextState GetEmptyState() {
			return MaskManagerPlainTextState.Empty;
		}
		protected bool Apply(MaskLogicResult result, StateChangeType changeType) {
			if(result == null)
				return false;
			return Apply(CreateStateForApply(result.EditText, result.CursorPosition, result.CursorPosition), changeType);
		}
		public override void SelectAll() {
			CursorEnd(false);
			CursorHome(true);
		}
		public override bool Delete() {
			if(IsSelection)
				return Insert(string.Empty);
			MaskLogicResult result = logic.GetDeleteResult(GetCurrentEditText().Substring(0, DisplayCursorPosition), GetCurrentEditText().Substring(DisplayCursorPosition));
			return Apply(result, StateChangeType.Delete);
		}
		public override bool Backspace() {
			if(IsSelection)
				return Insert(string.Empty);
			MaskLogicResult result = logic.GetBackspaceResult(GetCurrentEditText().Substring(0, DisplayCursorPosition), GetCurrentEditText().Substring(DisplayCursorPosition));
			return Apply(result, StateChangeType.Delete);
		}
		public override bool IsMatch { get { return logic.IsMatch(GetCurrentEditText()); } }
		public override bool IsFinal {
			get {
				return DisplayCursorPosition == DisplayText.Length && logic.IsFinal(GetCurrentEditText());
			}
		}
		protected override bool IsValidCursorPosition(int testedPosition) {
			return logic.IsValidCursorPosition(GetCurrentEditText(), testedPosition);
		}
		public override void SetInitialEditText(string initialEditText) {
			MaskLogicResult firstResult = logic.GetReplaceResult(initialEditText ?? string.Empty, string.Empty, string.Empty, string.Empty);
			if(firstResult != null) {
				SetInitialState(CreateStateForApply(firstResult.EditText, firstResult.CursorPosition, firstResult.CursorPosition));
			} else {
				base.SetInitialEditText(initialEditText);
			}
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		void SetState(string regExp, bool autoComplete, string editText, int cursor, int selectionAnchor) {
			this.logic = new RegExpMaskLogic(regExp, CultureInfo.InvariantCulture, autoComplete);
			SetInitialState(CreateStateForApply(editText, cursor, selectionAnchor));
		}
		void SetState(string regExp, bool autoComplete, string editText, int cursor) {
			SetState(regExp, autoComplete, editText, cursor, cursor);
		}
		void SetState(string regExp, bool autoComplete, string editText) {
			SetState(regExp, autoComplete, editText, editText.Length);
		}
		void AssertState(string editText, string maskedText, int cursor, int selectionAnchor) {
			Assert.AreEqual(editText, this.GetCurrentEditText());
			Assert.AreEqual(maskedText, this.logic.GetMaskedText(this.GetCurrentEditText(), '_'));
			Assert.AreEqual(cursor, this.DisplayCursorPosition);
			Assert.AreEqual(selectionAnchor, this.DisplaySelectionAnchor);
		}
		void AssertState(string editText, string maskedText, int cursor) {
			AssertState(editText, maskedText, cursor, cursor);
		}
		void AssertState(string editText, string maskedText) {
			AssertState(editText, maskedText, editText.Length);
		}
		void AssertState(string editText) {
			AssertState(editText, editText);
		}
		[Test]
		public void FunctionalTestEmpty() {
			SetState(string.Empty, false, "");
			AssertState("");
			Assert.IsTrue(IsMatch);
			Assert.IsTrue(!CursorLeft(false));
			AssertState("");
			Assert.IsTrue(!CursorRight(false));
			AssertState("");
			Assert.IsTrue(!CursorToDisplayPosition(0, false));
			AssertState("");
			Assert.IsTrue(!CursorToDisplayPosition(1, false));
			AssertState("");
			Assert.IsTrue(!CursorToDisplayPosition(-1, false));
			Assert.IsTrue(!Delete());
			Assert.IsTrue(!Backspace());
			Assert.IsTrue(!Insert("q"));
			Assert.IsTrue(!Insert("werty"));
			AssertState("");
		}
		[Test]
		public void FunctionalTestMoreThenOneDigits() {
			SetState(@"\d+", true, "");
			AssertState("", " _");
			Assert.IsFalse(IsMatch);
			Assert.IsFalse(CursorLeft(false));
			Assert.IsFalse(CursorRight(false));
			Assert.IsFalse(CursorToDisplayPosition(0, false));
			Assert.IsFalse(CursorToDisplayPosition(1, false));
			Assert.IsFalse(CursorToDisplayPosition(-1, false));
			Assert.IsFalse(Delete());
			Assert.IsFalse(Backspace());
			Assert.IsFalse(Insert("q"));
			Assert.IsFalse(Insert("werty"));
			AssertState("", " _");
			Assert.IsTrue(Insert("0"));
			AssertState("0");
			Assert.IsTrue(IsMatch);
			Assert.IsTrue(Insert("999"));
			AssertState("0999");
			Assert.IsFalse(Insert("-"));
			AssertState("0999");
			Assert.IsTrue(Insert("0-0"));
			AssertState("099900");
			Assert.IsTrue(Backspace());
			AssertState("09990");
			Assert.IsTrue(Backspace());
			AssertState("0999");
			Assert.IsFalse(Delete());
			AssertState("0999");
			Assert.IsTrue(Backspace());
			AssertState("099");
			Assert.IsTrue(CursorLeft(false));
			AssertState("099", "099", 2);
			Assert.IsTrue(Insert("5"));
			AssertState("0959", "0959", 3);
			Assert.IsTrue(CursorRight(true));
			AssertState("0959", "0959", 4, 3);
			Assert.IsTrue(CursorLeft(false));
			AssertState("0959", "0959", 3);
			Assert.IsTrue(Insert("8"));
			AssertState("09589", "09589", 4);
			Assert.IsTrue(Delete());
			AssertState("0958");
			Assert.IsTrue(CursorToDisplayPosition(3, false));
			AssertState("0958", "0958", 3);
			Assert.IsTrue(CursorLeft(true));
			AssertState("0958", "0958", 2, 3);
			Assert.IsTrue(CursorToDisplayPosition(0, true));
			AssertState("0958", "0958", 0, 3);
			Assert.IsTrue(CursorToDisplayPosition(4, false));
			AssertState("0958");
			Assert.IsTrue(Backspace());
			AssertState("095");
			Assert.IsTrue(Backspace());
			AssertState("09");
			Assert.IsTrue(Backspace());
			AssertState("0");
			Assert.IsTrue(IsMatch);
			Assert.IsTrue(Backspace());
			AssertState("", " _");
			Assert.IsFalse(Backspace());
			AssertState("", " _");
			Assert.IsFalse(IsMatch);
			Assert.IsTrue(Insert("2"));
			AssertState("2");
			Assert.IsTrue(CursorLeft(true));
			AssertState("2", "2", 0, 1);
			Assert.IsTrue(!CursorLeft(true));
			AssertState("2", "2", 0, 1);
			Assert.IsTrue(CursorLeft(false));
			AssertState("2", "2", 0);
			Assert.IsTrue(!CursorLeft(false));
			AssertState("2", "2", 0);
			Assert.IsTrue(IsMatch);
			Assert.IsTrue(Delete());
			AssertState("", " _");
			Assert.IsFalse(IsMatch);
		}
		[Test]
		public void FunctionalTestPhone() {
			SetState(@"(\(\d+\))?\d{2,3}-\d\d-\d\d", true, "");
			AssertState("", " __-__-__");
			Assert.IsTrue(Insert("1"));
			AssertState("1", "1 _-__-__");
			Assert.IsTrue(Insert("2"));
			AssertState("12-", "12- __-__", 2);
			Assert.IsTrue(Insert("3"));
			AssertState("123-", "123- __-__");
			Assert.IsTrue(Insert("4"));
			AssertState("123-4", "123-4 _-__");
			Assert.IsTrue(Backspace());
			AssertState("123-", "123- __-__");
			Assert.IsTrue(Backspace());
			AssertState("12-", "12- __-__", 2);
			Assert.IsTrue(Insert("-"));
			AssertState("12-", "12- __-__");
			Assert.IsTrue(Backspace());
			AssertState("12-", "12- __-__", 2);
			Assert.IsTrue(CursorLeft(false));
			AssertState("12-", "12- __-__", 1);
			Assert.IsTrue(CursorLeft(false));
			AssertState("12-", "12- __-__", 0);
			Assert.IsTrue(!Insert("q"));
			AssertState("12-", "12- __-__", 0);
			Assert.IsTrue(Insert("("));
			AssertState("(12)", "(12) __-__-__", 1);
			Assert.IsTrue(!Insert(")"));
			AssertState("(12)", "(12) __-__-__", 1);
			Assert.IsTrue(Insert("8"));
			AssertState("(812)", "(812) __-__-__", 2);
			Assert.IsTrue(Insert("3"));
			AssertState("(8312)", "(8312) __-__-__", 3);
			Assert.IsTrue(Insert("7"));
			AssertState("(83712)", "(83712) __-__-__", 4);
			Assert.IsTrue(Insert("7"));
			AssertState("(837712)", "(837712) __-__-__", 5);
			Assert.IsTrue(CursorToDisplayPosition(3, false));
			AssertState("(837712)", "(837712) __-__-__", 3);
			Assert.IsTrue(Insert(")"));
			AssertState("(83)771-2", "(83)771-2 _-__", 4);
			Assert.IsTrue(Insert("9"));
			AssertState("(83)977-12-", "(83)977-12- __", 5);
			Assert.IsTrue(CursorToDisplayPosition(7, false));
			AssertState("(83)977-12-", "(83)977-12- __", 8);
			Assert.IsTrue(Insert("3"));
			AssertState("(83)977-31-2", "(83)977-31-2 _", 9);
			Assert.IsTrue(!IsMatch);
			Assert.IsTrue(Insert("4"));
			AssertState("(83)977-34-12", "(83)977-34-12", 11);
			Assert.IsTrue(IsMatch);
			Assert.IsTrue(CursorToDisplayPosition(4, false));
			AssertState("(83)977-34-12", "(83)977-34-12", 4);
			Assert.IsTrue(Insert("7"));
			AssertState("(83)777-34-12", "(83)777-34-12", 5);
		}
		[Test]
		public void DfaBugTest1() {
			SetState(@"((-.-)+(-\d-)+)?", true, "");
			AssertState("", "");
			Assert.IsTrue(Insert("q"));
			AssertState("-q--", "-q-- _-");
			Assert.IsTrue(Backspace());
			AssertState("", "");
			Assert.IsTrue(Insert("0"));
			AssertState("-0--", "-0-- _-");
		}
		[Test]
		public void DeleteBug1() {
			SetState(@"\d{2,3}-\d\d-\d\d", true, "");
			AssertState("", " __-__-__");
			Assert.IsTrue(Insert("123456"));
			AssertState("123-45-6", "123-45-6 _");
			Assert.IsTrue(CursorToDisplayPosition(0, false));
			AssertState("123-45-6", "123-45-6 _", 0);
			Assert.IsTrue(Delete());
			AssertState("234-56-", "234-56- __", 0);
			Assert.IsTrue(Delete());
			AssertState("345-6", "345-6 _-__", 0);
		}
		[Test]
		public void DeleteBug2() {
			SetState(@"(-.-)+(-\d-)+", true, "");
			AssertState("", " -_--_-");
			Assert.IsTrue(Insert("12i0"));
			AssertState("-1--2--i--0-", "-1--2--i--0-");
			Assert.IsTrue(CursorLeft(false));
			AssertState("-1--2--i--0-", "-1--2--i--0-", 10);
			Assert.IsTrue(CursorLeft(false));
			AssertState("-1--2--i--0-", "-1--2--i--0-", 6);
			Assert.IsTrue(Backspace());
			AssertState("-1--i--0-", "-1--i--0-", 4);
		}
		[Test]
		public void DeleteBug3() {
			SetState(@"((10|11|12|[1-9]):[0-5]\d:[0-5]\d(am|pm))|((2[0-3]|[01][0-9]):[0-5][0-9]:[0-5][0-9])", true, "");
			AssertState("", " ________");
			Assert.IsTrue(Insert("0"));
			AssertState("0", "0 _:__:__");
			Assert.IsTrue(Insert("2"));
			AssertState("02:", "02: __:__");
			Assert.IsTrue(Backspace());
			AssertState("0", "0 _:__:__");
		}
		[Test]
		public void FullAutoBug1() {
			SetState(@"a", true, "");
			AssertState("", " a");
			Insert("a");
			AssertState("a");
		}
		[Test]
		public void RequestedBehavior1() {
			SetState(@"\d?\d:\d\d", true, @"");
			AssertState("", " _:__");
			Assert.IsTrue(Insert("1"));
			AssertState("1:", "1: __", 1);
			Assert.IsTrue(Insert("2"));
			AssertState("12:", "12: __");
			Assert.IsTrue(Insert("3"));
			AssertState("12:3", "12:3 _");
			Assert.IsTrue(Insert("4"));
			AssertState("12:34", "12:34");
			Assert.IsTrue(CursorHome(false));
			AssertState("12:34", "12:34", 0, 0);
			Assert.IsTrue(Delete());
			AssertState("2:34", "2:34", 0, 0);
		}
		[Test]
		public void ChangeRequest19711() {
			SetState("today|tomorrow|yesturday", true, @"");
			AssertState("", " t____");
			Assert.IsTrue(!Delete());
			AssertState("", " t____");
			Assert.IsTrue(Insert("t"));
			AssertState("to", "to ___");
			Assert.IsTrue(CursorHome(true));
			AssertState("to", "to ___", 0, 2);
			Assert.IsTrue(Delete());
			AssertState("", " t____", 0);
			SetState("June|July|January|October", true, @"");
			AssertState("", " ____");
			Assert.IsTrue(Insert("j"));
			AssertState("Ju", "Ju __", 1);
			Assert.IsTrue(Backspace());
			AssertState("", " ____");
			Assert.IsTrue(Insert("J"));
			AssertState("Ju", "Ju __", 1);
			Assert.IsTrue(Insert("u"));
			AssertState("Ju", "Ju __");
			Assert.IsTrue(Insert("l"));
			AssertState("July", "July");
			Assert.IsTrue(Backspace());
			Assert.IsTrue(Backspace());
			AssertState("Ju", "Ju __", 1);
			Assert.IsTrue(Insert("a"));
			AssertState("January", "January");
		}
		[Test]
		public void ChangeRequest_A473_1() {
			SetState(@"(\(\d{3}\)\d{3}-\d{4})?", true, "");
			AssertState("");
			Assert.IsTrue(Insert("(123) 456-7890"));
			AssertState("(123)456-7890");
		}
		[Test]
		public void ChangeRequest_A473_2() {
			SetState(@"\(\d{3}\)\d{3}-\d{4}", true, "");
			AssertState("", " (___)___-____");
			Assert.IsTrue(Insert("(123)456-7890"));
			AssertState("(123)456-7890");
		}
		[Test]
		public void ChangeRequest_A473_3() {
			SetState(@"\(\d{3}\)\d{3}-\d{4}", true, "(");
			AssertState("(", "( ___)___-____");
			Assert.IsTrue(Insert("(123)456-7890"));
			AssertState("(123)456-7890");
		}
		[Test]
		public void Bug_AB6537() {
			SetState(@"(\w*[\-\.\+]*)+@((\w*[\-]*)+\.)+\w{2,4}", true, "qq@qqqqq.qqqq");
			AssertState("qq@qqqqq.qqqq", "qq@qqqqq.qqqq", 13);
			Assert.IsTrue(this.IsMatch);
			Assert.IsTrue(Insert("q"));
			AssertState("qq@qqqqq.qqqqq.", "qq@qqqqq.qqqqq. __", 14);
			Assert.IsFalse(this.IsMatch);
		}
		[Test]
		public void Bug_AB6537_shorter() {
			SetState(@"((\d*)+\.)+\d{2,4}", true, "1.2345");
			AssertState("1.2345", "1.2345", 6);
			Assert.IsTrue(this.IsMatch);
			Assert.IsTrue(Insert("6"));
			AssertState("1.23456.", "1.23456. __", 7);
			Assert.IsFalse(this.IsMatch);
		}
		[Test]
		public void Bug_B30880() {
			SetState(@"([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])/([1-9]|[1-2]\d?|([3][0-2]))", true, "172.16.226.13/28", 11, 0);
			AssertState("172.16.226.13/28", "172.16.226.13/28", 11, 0);
			Assert.IsFalse(Insert("172.16.226.13"));
			AssertState("172.16.226.13/28", "172.16.226.13/28", 11, 0);
		}
		[Test]
		public void Bug_B141249_UnintendedInsertOnInitialValueAssign() {
			RegExpMaskManager manager = new RegExpMaskManager("A.*", false, true, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("A");
			Assert.AreEqual("A", manager.DisplayText);
		}
		[Test]
		public void Bug_B141249_UnintendedInsertOnInitialValueAssign_2() {
			RegExpMaskManager manager = new RegExpMaskManager("A.+", false, true, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("A");
			Assert.AreEqual("A _", manager.DisplayText);
		}
		[Test]
		public void Bug_B141249_UnintendedInsertOnInitialValueAssign_3() {
			RegExpMaskManager manager = new RegExpMaskManager("A.+", false, true, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("B");
			Assert.AreEqual("AB", manager.DisplayText);
		}
		[Test]
		public void Bug_B141249_UnintendedInsertOnInitialValueAssign_4() {
			RegExpMaskManager manager = new RegExpMaskManager("A.+", false, true, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("");
			Assert.AreEqual("A _", manager.DisplayText);
		}
		[Test]
		public void Bug_B141249_UnintendedInsertOnInitialValueAssign_5() {
			RegExpMaskManager manager = new RegExpMaskManager("A\\d+", false, true, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("B");
			Assert.AreEqual("A _", manager.DisplayText);
		}
		[Test]
		public void Bug_B203289_HugeWrongInsert() {
			RegExpMaskManager manager = new RegExpMaskManager(".{0,10}", false, false, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("");
			Assert.AreEqual("", manager.DisplayText);
			Assert.IsTrue(manager.Insert(new string('Q', 1024 * 128)));
			Assert.AreEqual("QQQQQQQQQQ", manager.DisplayText);
		}
		[Test]
		public void Bug_B205857_UndoEnabledAfterInitialValueSet() {
			RegExpMaskManager manager = new RegExpMaskManager(".*", false, false, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("Foo");
			Assert.IsFalse(manager.CanUndo);
		}
		[Test]
		public void Bug_Q363042_ArgOutOfRangeOnCrazyMaskTest() {
			RegExpMaskManager manager = new RegExpMaskManager(@"(https?://[a-z]+)?", false, false, false, true, '_', CultureInfo.InvariantCulture);
			manager.SetInitialEditText("");
			Assert.AreEqual("", manager.DisplayText);
			Assert.AreEqual(0, manager.DisplayCursorPosition);
			Assert.IsTrue(manager.Insert("h"));
			Assert.AreEqual("h ttp://_", manager.DisplayText);
			Assert.AreEqual(1, manager.DisplayCursorPosition);
			Assert.IsTrue(manager.CursorHome(false));
			Assert.AreEqual("h ttp://_", manager.DisplayText);
			Assert.AreEqual(0, manager.DisplayCursorPosition);
			Assert.IsTrue(manager.Insert("h"));
			Assert.AreEqual("h ttp://_", manager.DisplayText);
			Assert.AreEqual(1, manager.DisplayCursorPosition);
		}
#endif
		#endregion
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class RegExpMaskManagerTests: RegExpMaskManagerCore {
		public RegExpMaskManagerTests() : base(string.Empty, false, true, false, true, '_', CultureInfo.InvariantCulture) { }
	}
#endif
		#endregion
	public class RegExpMaskManager: MaskManagerSelectAllEnhancer<RegExpMaskManagerCore> {
		public RegExpMaskManager(string regExp, bool reverseDfa, bool isAutoComplete, bool isOptimistic, bool showPlaceHolders, char anySymbolPlaceHolder, CultureInfo managerCultureInfo)
			: base(new RegExpMaskManagerCore(regExp, false, isAutoComplete, isOptimistic, showPlaceHolders, anySymbolPlaceHolder, managerCultureInfo)) {
			System.Diagnostics.Debug.Assert(reverseDfa == false);
		}
	}
}
