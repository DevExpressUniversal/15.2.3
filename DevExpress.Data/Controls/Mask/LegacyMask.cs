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
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
using System.Collections.Generic;
namespace DevExpress.Data.Mask {
	public abstract class LegacyMaskPrimitive {
		public abstract bool IsLiteral { get; }
		public abstract string CapturingExpression { get; }
		public abstract int MinMatches { get; }
		public abstract int MaxMatches { get; }
		public abstract string GetDisplayText(string elementValue, char blank);
		public abstract string GetEditText(string elementValue, char blank, bool saveLiteral);
		char CaseConversion;
		public bool IsAcceptableStrong(char input) {
			return Regex.IsMatch(input.ToString(), CapturingExpression);
		}
		public bool IsAcceptable(char input) {
			switch(CaseConversion) {
				case 'U':
					return IsAcceptableStrong(CultureInfo.InvariantCulture.TextInfo.ToUpper(input));
				case 'L':
					return IsAcceptableStrong(CultureInfo.InvariantCulture.TextInfo.ToLower(input));
				default:
					return IsAcceptableStrong(input) || IsAcceptableStrong(CultureInfo.InvariantCulture.TextInfo.ToUpper(input)) || IsAcceptableStrong(CultureInfo.InvariantCulture.TextInfo.ToLower(input));
			}
		}
		public char GetAcceptableChar(char input) {
			char patched;
			switch(CaseConversion) {
				case 'U':
					patched = CultureInfo.InvariantCulture.TextInfo.ToUpper(input);
					break;
				case 'L':
					patched = CultureInfo.InvariantCulture.TextInfo.ToLower(input);
					break;
				default:
					patched = input;
					if(IsAcceptableStrong(patched))
						break;
					patched = CultureInfo.InvariantCulture.TextInfo.ToUpper(input);
					if(IsAcceptableStrong(patched))
						break;
					patched = CultureInfo.InvariantCulture.TextInfo.ToLower(input);
					break;
			}
			if(!IsAcceptableStrong(patched))
				throw new InvalidOperationException();
			return patched;
		}
		protected LegacyMaskPrimitive(char caseConversion) {
			this.CaseConversion = caseConversion;
		}
	}
	public class LegacyMaskLiteral : LegacyMaskPrimitive {
		char literal;
		public override bool IsLiteral { get { return true; } }
		public override string CapturingExpression {
			get {
				return string.Format(CultureInfo.InvariantCulture, literal == '^' || literal == '\\' ? "[\\{0}]" : "[{0}]", literal);
			}
		}
		public override int MinMatches { get { return 1; } }
		public override int MaxMatches { get { return 1; } }
		public override string GetDisplayText(string elementValue, char blank) {
			return literal.ToString();
		}
		public override string GetEditText(string elementValue, char blank, bool saveLiteral) {
			return saveLiteral ? literal.ToString() : string.Empty;
		}
		public LegacyMaskLiteral(char literal)
			: base('A') {
			this.literal = literal;
		}
	}
	public class LegacyMaskChar : LegacyMaskPrimitive {
		string capturing;
		int minMatches, maxMatches;
		public override bool IsLiteral { get { return false; } }
		public override string CapturingExpression { get { return capturing; } }
		public override int MinMatches { get { return minMatches; } }
		public override int MaxMatches { get { return maxMatches; } }
		public override string GetDisplayText(string elementValue, char blank) {
			if(elementValue.Length < MaxMatches) {
				int holders = elementValue.Length >= MinMatches ? 1 : MinMatches - elementValue.Length;
				return elementValue + new string(blank, holders);
			} else {
				return elementValue;
			}
		}
		public override string GetEditText(string elementValue, char blank, bool saveLiteral) {
			if(elementValue.Length < MinMatches) {
				return elementValue + new string(blank, MinMatches - elementValue.Length);
			} else {
				return elementValue;
			}
		}
		public LegacyMaskChar(string capturing, char caseConversion, int minMatches, int maxMatches)
			: base(caseConversion) {
			this.capturing = capturing;
			this.minMatches = minMatches;
			this.maxMatches = maxMatches;
			Regex.IsMatch("q", CapturingExpression);
		}
		public void PatchMatches(int min, int max) {
			this.minMatches = min;
			this.maxMatches = max;
		}
	}
	public class LegacyMaskInfo : IEnumerable {
		readonly List<LegacyMaskPrimitive> container = new List<LegacyMaskPrimitive>();
		IEnumerator IEnumerable.GetEnumerator() {
			return container.GetEnumerator();
		}
		static void PatchZeroLengthMaskInfo(LegacyMaskInfo info, char caseConversion) {
			if(info.Count == 0) {
				info.container.Add(new LegacyMaskChar(".", caseConversion, 0, int.MaxValue));
			}
		}
		public static LegacyMaskInfo GetSimpleMaskInfo(string mask, CultureInfo maskCulture) {
			LegacyMaskInfo info = new LegacyMaskInfo();
			string work = mask;
			char caseConversion = 'A';
			while(work.Length > 0) {
				char nextChar = work[0];
				string nextWork = work.Substring(1);
				switch(nextChar) {
					case '>':
						caseConversion = 'U';
						break;
					case '<':
						if(work.StartsWith("<>")) {
							nextWork = work.Substring(2);
							caseConversion = 'A';
						} else
							caseConversion = 'L';
						break;
					case '/':
						foreach(char ch in DateTimeFormatHelper.GetDateSeparator(maskCulture))
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case ':':
						foreach(char ch in DateTimeFormatHelper.GetTimeSeparator(maskCulture))
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case '$':
						foreach(char ch in maskCulture.NumberFormat.CurrencySymbol)
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case 'l':
					case 'L':
						info.container.Add(new LegacyMaskChar("\\p{L}", caseConversion, char.IsUpper(nextChar) ? 1 : 0, 1));
						break;
					case 'a':
					case 'A':
						info.container.Add(new LegacyMaskChar("(\\p{L}|\\d)", caseConversion, char.IsUpper(nextChar) ? 1 : 0, 1));
						break;
					case 'c':
					case 'C':
						info.container.Add(new LegacyMaskChar(".", caseConversion, char.IsUpper(nextChar) ? 1 : 0, 1));
						break;
					case '0':
					case '9':
						info.container.Add(new LegacyMaskChar("\\d", caseConversion, nextChar == '0' ? 1 : 0, 1));
						break;
					case '#':
						info.container.Add(new LegacyMaskChar("(\\d|[+]|[-])", caseConversion, 0, 1));
						break;
					case '\\':
						if(nextWork.Length == 0)
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashBeforeEndOfMask);
						nextChar = nextWork[0];
						nextWork = nextWork.Substring(1);
						info.container.Add(new LegacyMaskLiteral(nextChar));
						break;
					default:
						info.container.Add(new LegacyMaskLiteral(nextChar));
						break;
				}
				work = nextWork;
			}
			PatchZeroLengthMaskInfo(info, caseConversion);
			return info;
		}
		public static LegacyMaskInfo GetRegularMaskInfo(string mask, CultureInfo maskCulture) {
			LegacyMaskInfo info = new LegacyMaskInfo();
			string work = mask;
			char caseConversion = 'A';
			while(work.Length > 0) {
				char nextChar = work[0];
				string nextWork = work.Substring(1);
				switch(nextChar) {
					case '>':
						caseConversion = 'U';
						break;
					case '<':
						if(work.StartsWith("<>")) {
							nextWork = work.Substring(2);
							caseConversion = 'A';
						} else
							caseConversion = 'L';
						break;
					case '/':
						foreach(char ch in DateTimeFormatHelper.GetDateSeparator(maskCulture))
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case ':':
						foreach(char ch in DateTimeFormatHelper.GetTimeSeparator(maskCulture))
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case '$':
						foreach(char ch in maskCulture.NumberFormat.CurrencySymbol)
							info.container.Add(new LegacyMaskLiteral(ch));
						break;
					case '.':
						info.container.Add(new LegacyMaskChar(".", caseConversion, 1, 1));
						break;
					case '[':
						int inBracketExpressionLength = nextWork.Replace("\\]", "\\}").IndexOf(']');
						if(inBracketExpressionLength < 0)
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskClosingSquareBracketExpected);
						string pattern = work.Substring(0, inBracketExpressionLength + 2);
						nextWork = work.Substring(inBracketExpressionLength + 2);
						info.container.Add(new LegacyMaskChar(pattern, caseConversion, 1, 1));
						break;
					case '\\':
						if(nextWork.Length == 0)
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskBackslashBeforeEndOfMask);
						nextChar = nextWork[0];
						nextWork = nextWork.Substring(1);
						switch(nextChar) {
							case 'D':
							case 'd':
							case 'W':
							case 'w':
								info.container.Add(new LegacyMaskChar("\\" + nextChar, caseConversion, 1, 1));
								break;
							default:
								info.container.Add(new LegacyMaskLiteral(nextChar));
								break;
						}
						break;
					case '*':
						info.PatchQuantifier(0, int.MaxValue);
						break;
					case '+':
						info.PatchQuantifier(1, int.MaxValue);
						break;
					case '?':
						info.PatchQuantifier(0, 1);
						break;
					case '{':
						int quantifierLength = nextWork.IndexOf('}');
						if(quantifierLength == 0)
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskClosingCurveBracketExpected);
						string[] quantifier = nextWork.Substring(0, quantifierLength).Split(',');
						nextWork = work.Substring(quantifierLength + 2);
						if(quantifier.Length == 1) {
							int n = Int32.Parse(quantifier[0], CultureInfo.InvariantCulture);
							info.PatchQuantifier(n, n);
						} else if(quantifier.Length == 2) {
							int n = Int32.Parse(quantifier[0], CultureInfo.InvariantCulture);
							int m = quantifier[1].Length == 0 ? int.MaxValue : Int32.Parse(quantifier[1], CultureInfo.InvariantCulture);
							info.PatchQuantifier(n, m);
						} else {
							throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidQuantifierFormat);
						}
						break;
					default:
						info.container.Add(new LegacyMaskLiteral(nextChar));
						break;
				}
				work = nextWork;
			}
			PatchZeroLengthMaskInfo(info, caseConversion);
			return info;
		}
		public int Count { get { return container.Count; } }
		public LegacyMaskPrimitive this[int primitiveIndex] { get { return container[primitiveIndex]; } }
		public string GetDisplayText(string[] elements, char blank) {
			string result = string.Empty;
			for(int i = 0; i < this.Count; ++i) {
				result += this[i].GetDisplayText(elements[i], blank);
			}
			return result;
		}
		public string GetEditText(string[] elements, char blank, bool saveLiteral) {
			string result = string.Empty;
			for(int i = 0; i < this.Count; ++i) {
				result += this[i].GetEditText(elements[i], blank, saveLiteral);
			}
			return result;
		}
		public int GetPosition(string[] elements, int element, int insideElement) {
			int result = 0;
			for(int i = 0; i < element; ++i) {
				result += this[i].GetDisplayText(elements[i], '@').Length;
			}
			result += insideElement;
			return result;
		}
		public int GetNextEditableElement(int current) {
			for(int i = current + 1; i < this.Count; ++i)
				if(!this[i].IsLiteral)
					return i;
			return -1;
		}
		public int GetFirstEditableIndex() {
			return GetNextEditableElement(-1);
		}
		public int GetPrevEditableElement(int current) {
			for(int i = current - 1; i >= 0; --i)
				if(!this[i].IsLiteral)
					return i;
			return -1;
		}
		public int GetLastEditableIndex() {
			return GetPrevEditableElement(this.Count);
		}
		public bool GetIsEditable() {
			return GetFirstEditableIndex() >= 0;
		}
		public void PatchQuantifier(int min, int max) {
			if(Count == 0)
				throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidQuantifierFormat);
			LegacyMaskChar patched = this[Count - 1] as LegacyMaskChar;
			if(patched == null)
				throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidQuantifierFormat);
			if(patched.MinMatches != 1 || patched.MaxMatches != 1)
				throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidQuantifierFormat);
			if(min < 0 || min > max)
				throw new ArgumentException(MaskExceptionsTexts.IncorrectMaskInvalidQuantifierFormat);
			patched.PatchMatches(min, max);
		}
		public string[] GetElementsEmpty() {
			string[] res = new string[this.Count];
			for(int i = 0; i < this.Count; ++i)
				res[i] = string.Empty;
			return res;
		}
		string BuildExtruderRegExp(char blank, bool saveLiteral) {
			string extruder = string.Empty;
			string blankExtruder = string.Format(CultureInfo.InvariantCulture, blank == '^' || blank == '\\' ? "[\\{0}]" : "[{0}]", blank);
			for(int i = 0; i < Count; ++i) {
				LegacyMaskPrimitive current = this[i];
				if(current.IsLiteral) {
					if(saveLiteral) {
						extruder += "(" + current.CapturingExpression + ")";
					} else {
					}
				} else {
					string extruderMask = "(?<element{4}>(({0})";
					if(!Regex.IsMatch(blank.ToString(), current.CapturingExpression))
						extruderMask += "|({1})";
					extruderMask += ")";
					if(current.MinMatches != 1 || current.MaxMatches != 1)
						extruderMask += "{{{2},{3}}}";
					extruderMask += ")";
					extruder += string.Format(CultureInfo.InvariantCulture, extruderMask, current.CapturingExpression, blankExtruder, current.MinMatches, current.MaxMatches, i);
				}
			}
			return "^" + extruder + "$";
		}
		public string[] GetElementsFromEditText(string editText, char blank, bool saveLiteral) {
			string extruder = BuildExtruderRegExp(blank, saveLiteral);
			Match match = Regex.Match(editText, extruder);
			if(!match.Success || match.Value != editText)
				return null;
			string[] result = new string[Count];
			for(int i = 0; i < Count; ++i) {
				LegacyMaskPrimitive current = this[i];
				string currentElement;
				if(current.IsLiteral) {
					currentElement = string.Empty;
				} else {
					currentElement = match.Groups[string.Format(CultureInfo.InvariantCulture, "element{0}", i)].Value;
					currentElement = currentElement.TrimEnd(blank);
					if(!current.IsAcceptableStrong(blank))
						currentElement = currentElement.Replace(blank.ToString(), string.Empty);
				}
				result[i] = currentElement;
			}
			return result;
		}
	}
	public sealed class LegacyMaskManagerState : MaskManagerState {
		public string[] Elements;
		public int CursorPositionElement;
		public int CursorPositionInsideElement;
		public int SelectionAnchorElement;
		public int SelectionAnchorInsideElement;
		public LegacyMaskInfo Info;
		public override bool IsSame(MaskManagerState comparedState) {
			if(comparedState == null)
				return false;
			if(this.GetType() != typeof(LegacyMaskManagerState))
				throw new NotImplementedException(MaskExceptionsTexts.InternalErrorNonSpecific);
			if(comparedState.GetType() != typeof(LegacyMaskManagerState))
				throw new InvalidOperationException(MaskExceptionsTexts.InternalErrorNonSpecific);
			LegacyMaskManagerState comparedLegacyState = (LegacyMaskManagerState)comparedState;
			if(this.CursorPositionElement != comparedLegacyState.CursorPositionElement)
				return false;
			if(this.CursorPositionInsideElement != comparedLegacyState.CursorPositionInsideElement)
				return false;
			if(this.SelectionAnchorElement != comparedLegacyState.SelectionAnchorElement)
				return false;
			if(this.SelectionAnchorInsideElement != comparedLegacyState.SelectionAnchorInsideElement)
				return false;
			if(!object.ReferenceEquals(this.Info, comparedLegacyState.Info))
				throw new InvalidOperationException(MaskExceptionsTexts.InternalErrorNonSpecific);
			if(this.Elements.Length != comparedLegacyState.Elements.Length)
				throw new InvalidOperationException(MaskExceptionsTexts.InternalErrorNonSpecific);
			for(int i = 0; i < this.Elements.Length; ++i)
				if(this.Elements[i] != comparedLegacyState.Elements[i])
					return false;
			return true;
		}
		public string GetDisplayText(char blank) { return Info.GetDisplayText(Elements, blank); }
		public string GetEditText(char blank, bool saveLiteral) { return Info.GetEditText(Elements, blank, saveLiteral); }
		public int DisplayCursorPosition { get { return Info.GetPosition(Elements, CursorPositionElement, CursorPositionInsideElement); } }
		public int DisplaySelectionAnchor { get { return Info.GetPosition(Elements, SelectionAnchorElement, SelectionAnchorInsideElement); } }
		public LegacyMaskManagerState(LegacyMaskInfo info, string[] elements, int cursorPositionElement, int cursorPositionInsideElement, int selectionAnchorElement, int selectionAnchorInsideElement) {
			this.Info = info;
			this.Elements = elements;
			this.CursorPositionElement = cursorPositionElement;
			this.CursorPositionInsideElement = cursorPositionInsideElement;
			this.SelectionAnchorElement = selectionAnchorElement;
			this.SelectionAnchorInsideElement = selectionAnchorInsideElement;
		}
		public LegacyMaskManagerState(LegacyMaskInfo info) : this(info, info.GetElementsEmpty(), 0, 0, 0, 0) { }
		LegacyMaskManagerState(LegacyMaskManagerState source) : this(source.Info, (string[])source.Elements.Clone(), source.CursorPositionElement, source.CursorPositionInsideElement, source.SelectionAnchorElement, source.SelectionAnchorInsideElement) { }
		public LegacyMaskManagerState Clone() {
			return new LegacyMaskManagerState(this);
		}
		public bool CursorTo(int newPosition, bool forceSelection) {
			if(newPosition < 0 || newPosition > Info.GetDisplayText(Elements, '@').Length)
				return false;
			int elem = 0;
			while(Info.GetPosition(Elements, elem + 1, 0) < newPosition) {
				++elem;
			}
			CursorPositionElement = elem;
			CursorPositionInsideElement = newPosition - Info.GetPosition(Elements, elem, 0);
			if(!forceSelection) {
				SelectionAnchorElement = CursorPositionElement;
				SelectionAnchorInsideElement = CursorPositionInsideElement;
			}
			return true;
		}
		void SetPositions(int element, int insideElement) {
			CursorPositionElement = element;
			CursorPositionInsideElement = insideElement;
			SelectionAnchorElement = element;
			SelectionAnchorInsideElement = insideElement;
		}
		bool SetCaretTo(int element, int insideElement) {
			if(insideElement >= Info[element].GetDisplayText(Elements[element], '@').Length) {
				if(element >= Info.Count - 1) {
					SetPositions(element, insideElement);
					return true;
				}
				element++;
				insideElement = 0;
			}
			SetPositions(element, insideElement);
			SelectionAnchorInsideElement++;
			return true;
		}
		public bool CursorHome(bool forceSelection) {
			if(forceSelection || !Info.GetIsEditable())
				return CursorTo(0, forceSelection);
			return SetCaretTo(Info.GetFirstEditableIndex(), 0);
		}
		public bool CursorEnd(bool forceSelection) {
			if(forceSelection || !Info.GetIsEditable())
				return CursorTo(Info.GetDisplayText(Elements, '@').Length, forceSelection);
			int lastEditable = Info.GetLastEditableIndex();
			return SetCaretTo(lastEditable, Info[lastEditable].GetDisplayText(Elements[lastEditable], '@').Length - 1);
		}
		public void SelectAll() {
			int ln = Info.GetDisplayText(Elements, '@').Length;
			CursorTo(ln, false);
			CursorTo(0, true);
		}
		public bool CursorLeft() {
			if(!Info[CursorPositionElement].IsLiteral && CursorPositionInsideElement > 0) {
				return SetCaretTo(CursorPositionElement, CursorPositionInsideElement - 1);
			}
			int nextElem = Info.GetPrevEditableElement(CursorPositionElement);
			if(nextElem >= 0) {
				return SetCaretTo(nextElem, Info[nextElem].GetDisplayText(Elements[nextElem], '@').Length - 1);
			}
			nextElem = Info.GetFirstEditableIndex();
			if(nextElem >= 0) {
				return SetCaretTo(nextElem, 0);
			}
			return false;
		}
		public bool CursorRight() {
			if(!Info[CursorPositionElement].IsLiteral && CursorPositionInsideElement < Info[CursorPositionElement].GetDisplayText(Elements[CursorPositionElement], '@').Length - 1) {
				return SetCaretTo(CursorPositionElement, CursorPositionInsideElement + 1);
			}
			int nextElem = Info.GetNextEditableElement(CursorPositionElement);
			if(nextElem >= 0) {
				return SetCaretTo(nextElem, 0);
			}
			nextElem = Info.GetLastEditableIndex();
			if(nextElem >= 0) {
				return SetCaretTo(nextElem, Info[nextElem].GetDisplayText(Elements[nextElem], '@').Length - 1);
			}
			return false;
		}
		public bool IsFinal(char blank) {
			if(!IsMatch(blank))
				return false;
			if(CursorPositionElement == Info.GetLastEditableIndex() && CursorPositionInsideElement == Info[CursorPositionElement].MaxMatches)
				return true;
			else
				return false;
		}
		public bool IsMatch(char blank) {
			for(int i = 0; i < Info.Count; ++i) {
				if(Info[i].IsLiteral)
					continue;
				if(Elements[i].Length >= Info[i].MinMatches)
					continue;
				if(Info[i].IsAcceptableStrong(blank))
					continue;
				return false;
			}
			return true;
		}
		bool Insert(char inp) {
			Erase();
			if(Info[CursorPositionElement].MaxMatches <= CursorPositionInsideElement) {
				if(CursorPositionElement + 1 >= Info.Count)
					return false;
				CursorPositionElement++;
				CursorPositionInsideElement = 0;
			}
			if(!Info[CursorPositionElement].IsAcceptable(inp)) {
				for(int i = 1; ; ++i) {
					if(CursorPositionElement + i >= Info.Count)
						return false;
					if(Info[CursorPositionElement + i].IsAcceptable(inp)) {
						CursorPositionElement += i;
						CursorPositionInsideElement = 0;
						break;
					}
				}
			}
			if(Info[CursorPositionElement].IsLiteral) {
				CursorPositionInsideElement++;
				return SetCaretTo(CursorPositionElement, CursorPositionInsideElement);
			}
			inp = Info[CursorPositionElement].GetAcceptableChar(inp);
			if(Elements[CursorPositionElement].Length <= CursorPositionInsideElement) {
				Elements[CursorPositionElement] += inp;
				return SetCaretTo(CursorPositionElement, Elements[CursorPositionElement].Length);
			}
			string tail = Elements[CursorPositionElement].Substring(CursorPositionInsideElement);
			if(Elements[CursorPositionElement].Length >= Info[CursorPositionElement].MaxMatches)
				tail = tail.Substring(1);
			Elements[CursorPositionElement] = Elements[CursorPositionElement].Substring(0, CursorPositionInsideElement) + inp + tail;
			return SetCaretTo(CursorPositionElement, CursorPositionInsideElement + 1);
		}
		public bool Insert(string insertion) {
			if(insertion.Length == 0)
				return Erase();
			bool inserted = false;
			foreach(char ch in insertion) {
				if(Insert(ch))
					inserted = true;
			}
			if(!inserted)
				return false;
			if(Info[CursorPositionElement].IsLiteral) {
				CursorRight();
			}
			return true;
		}
		bool Erase() {
			int startIndex, endIndex, startInside, endInside;
			if(CursorPositionElement < SelectionAnchorElement || (CursorPositionElement == SelectionAnchorElement && CursorPositionInsideElement < SelectionAnchorInsideElement)) {
				startIndex = CursorPositionElement;
				startInside = CursorPositionInsideElement;
				endIndex = SelectionAnchorElement;
				endInside = SelectionAnchorInsideElement;
			} else {
				startIndex = SelectionAnchorElement;
				startInside = SelectionAnchorInsideElement;
				endIndex = CursorPositionElement;
				endInside = CursorPositionInsideElement;
			}
			if(startIndex == endIndex) {
				string elem = Elements[startIndex];
				if(startInside > elem.Length)
					startInside = elem.Length;
				if(endInside > elem.Length)
					endInside = elem.Length;
				elem = elem.Substring(0, startInside) + elem.Substring(endInside);
				Elements[startIndex] = elem;
			} else {
				if(startInside < Elements[startIndex].Length) {
					Elements[startIndex] = Elements[startIndex].Substring(0, startInside);
				}
				if(endInside < Elements[endIndex].Length) {
					Elements[endIndex] = Elements[endIndex].Substring(endInside);
				} else {
					Elements[endIndex] = string.Empty;
				}
				for(int i = startIndex + 1; i < endIndex; ++i)
					Elements[i] = string.Empty;
			}
			SetPositions(startIndex, startInside);
			return true;
		}
		public bool Delete() {
			if(DisplayCursorPosition == DisplaySelectionAnchor) {
				CursorRight();
			}
			bool advanceCaret = (DisplaySelectionAnchor - DisplayCursorPosition == 1);
			Erase();
			if(advanceCaret && Elements[CursorPositionElement].Length <= CursorPositionInsideElement)
				CursorRight();
			SetCaretTo(CursorPositionElement, CursorPositionInsideElement);
			return true;
		}
		public bool Backspace() {
			if(DisplayCursorPosition == DisplaySelectionAnchor)
				CursorLeft();
			Erase();
			CursorLeft();
			return true;
		}
		public bool IsEmpty() {
			for(int i = 0; i < Info.Count; ++i) {
				string element = Elements[i];
				if(element != null && element.Length != 0 && !Info[i].IsLiteral)
					return false;
			}
			return true;
		}
	}
	public class LegacyMaskManagerCore: MaskManagerStated<LegacyMaskManagerState> {
		readonly LegacyMaskInfo info;
		readonly bool saveLiteral;
		readonly char blank;
		readonly bool ignoreMaskBlank;
		public override bool Insert(string insertion) {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.Insert(insertion))
				return false;
			return Apply(work, StateChangeType.Insert);
		}
		public override bool Delete() {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.Delete())
				return false;
			return Apply(work, StateChangeType.Delete);
		}
		public override bool Backspace() {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.Backspace())
				return false;
			return Apply(work, StateChangeType.Delete);
		}
		public override bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.CursorTo(newPosition, forceSelection))
				return false;
			return Apply(work, StateChangeType.Terminator);
		}
		public override bool CursorHome(bool forceSelection) {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.CursorHome(forceSelection))
				return false;
			return Apply(work, StateChangeType.Terminator);
		}
		public override bool CursorEnd(bool forceSelection) {
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.CursorEnd(forceSelection))
				return false;
			return Apply(work, StateChangeType.Terminator);
		}
		public override void SelectAll() {
			LegacyMaskManagerState work = CurrentState.Clone();
			work.SelectAll();
			Apply(work, StateChangeType.Terminator);
		}
		public override void SetInitialEditText(string initialEditText) {
			string[] elements = info.GetElementsFromEditText(initialEditText, blank, saveLiteral);
			if(elements != null) {
				SetInitialState(new LegacyMaskManagerState(info, elements, 0, 0, 0, 0));
			} else {
				LegacyMaskManagerState tmpState = new LegacyMaskManagerState(info);
				tmpState.Insert(initialEditText);
				SetInitialState(new LegacyMaskManagerState(info, tmpState.Elements, 0, 0, 0, 0));
			}
			CursorHome(false);
		}
		public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck) {
			if(forceSelection) {
				if(isNeededKeyCheck)
					return true;
				else
					return CursorToDisplayPosition(DisplayCursorPosition - 1, forceSelection);
			}
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.CursorLeft())
				return false;
			return Apply(work, StateChangeType.Terminator, isNeededKeyCheck);
		}
		public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck) {
			if(forceSelection) {
				if(isNeededKeyCheck)
					return true;
				else
					return CursorToDisplayPosition(DisplayCursorPosition + 1, forceSelection);
			}
			LegacyMaskManagerState work = CurrentState.Clone();
			if(!work.CursorRight())
				return false;
			return Apply(work, StateChangeType.Terminator, isNeededKeyCheck);
		}
		public override bool IsMatch {
			get {
				if(ignoreMaskBlank && CurrentState.IsEmpty())
					return true;
				return CurrentState.IsMatch(this.blank);
			}
		}
		public override bool IsFinal { get { return CurrentState.IsFinal(this.blank); } }
		protected override string GetEditText(LegacyMaskManagerState state) {
			if(ignoreMaskBlank && state.IsEmpty())
				return string.Empty;
			return state.GetEditText(blank, saveLiteral);
		}
		protected override string GetDisplayText(LegacyMaskManagerState state) {
			return state.GetDisplayText(blank);
		}
		protected override object GetEditValue(LegacyMaskManagerState state) {
			return GetEditText(state);
		}
		public override void SetInitialEditValue(object initialEditValue) {
			SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", initialEditValue));
		}
		public override bool IsEditValueAssignedAsFormattedText {
			get { return saveLiteral; }
		}
		protected override int GetCursorPosition(LegacyMaskManagerState state) {
			return state.DisplayCursorPosition;
		}
		protected override int GetSelectionAnchor(LegacyMaskManagerState state) {
			return state.DisplaySelectionAnchor;
		}
		public LegacyMaskManagerCore(LegacyMaskInfo info, char blank, bool saveLiteral, bool ignoreMaskBlank)
			: base(new LegacyMaskManagerState(info)) {
			this.info = info;
			this.saveLiteral = saveLiteral;
			this.blank = blank;
			this.ignoreMaskBlank = ignoreMaskBlank;
		}
	}
	public class LegacyMaskManager: MaskManagerSelectAllEnhancer<LegacyMaskManagerCore> {
		public LegacyMaskManager(LegacyMaskInfo info, char blank, bool saveLiteral, bool ignoreMaskBlank)
			: base(new LegacyMaskManagerCore(info, blank, saveLiteral, ignoreMaskBlank)) {
		}
	}
}
