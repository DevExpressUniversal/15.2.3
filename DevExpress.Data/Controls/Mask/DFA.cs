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
using System.Collections.Specialized;
using System.Globalization;
#region Mask_Tests
#if DEBUGTEST && !SILVERLIGHT
using NUnit.Framework;
#endif
#endregion
using DevExpress.Utils;
namespace DevExpress.Data.Mask {
	public class State {
		readonly List<Transition> transitions = new List<Transition>();
		public ICollection Transitions { get { return transitions; } }
		public void AddTransition(Transition transition) {
			#region Mask_Tests
#if DEBUGTEST
			Assert.IsNull(reachableStatesDictionary);
#endif
			#endregion
			transitions.Add(transition);
		}
		IDictionary<State, bool> reachableStatesDictionary = null;
		IDictionary<State, bool> ReachableStatesDictionary {
			get {
				if(reachableStatesDictionary == null) {
					reachableStatesDictionary = new Dictionary<State, bool>();
					CollectReachableStates(this, reachableStatesDictionary);
				}
				return reachableStatesDictionary;
			}
		}
		static void CollectReachableStates(State nextState, IDictionary<State, bool> states) {
			if(!states.ContainsKey(nextState)) {
				states.Add(nextState, true);
				foreach(Transition transition in nextState.Transitions)
					CollectReachableStates(transition.Target, states);
			}
		}
		public ICollection<State> GetReachableStates() {
			return ReachableStatesDictionary.Keys;
		}
		public bool CanReach(State state) {
			return ReachableStatesDictionary.ContainsKey(state);
		}
		#region Mask_Tests
#if DEBUGTEST
		internal Transition this[int transitionIndex] {
			get {
				return (Transition)transitions[transitionIndex];
			}
		}
#endif
		#endregion
	}
	public abstract class Transition {
		readonly State target;
		public State Target { get { return target; } }
		public abstract bool IsMatch(char input);
		protected Transition(State target) {
			this.target = target;
		}
		protected Transition() : this(new State()) { }
		public abstract Transition Copy(State target);
		public virtual bool IsEmpty { get { return false; } }
		public virtual bool IsExact { get { return false; } }
		public abstract char GetSampleChar();
		#region Mask_Tests
#if DEBUGTEST
		internal void CommonTest(string matchChars, string nonMatchChars) {
			CommonTest(matchChars, nonMatchChars, null);
		}
		internal void CommonTest(string matchChars, string nonMatchChars, object exactChar) {
			Transition transition = this;
			Assert.IsNotNull(transition.Target);
			if(matchChars == null) {
				Assert.AreEqual(true, transition.IsEmpty);
				Assert.IsTrue(exactChar == null);
				matchChars = string.Empty;
				for(char ch = (char)0; ch <= (char)2048; ++ch)
					nonMatchChars += ch;
			} else {
				Assert.AreEqual(false, transition.IsEmpty);
				if(exactChar != null) {
					matchChars += (char)exactChar;
					for(char ch = (char)0; ch <= (char)2048; ++ch)
						if(ch != (char)exactChar)
							nonMatchChars += ch;
				}
			}
			foreach(char input in matchChars)
				Assert.AreEqual(true, transition.IsMatch(input));
			foreach(char input in nonMatchChars)
				Assert.AreEqual(false, transition.IsMatch(input));
			Assert.AreEqual(exactChar != null, transition.IsExact);
			if(transition.IsExact)
				Assert.AreEqual((char)exactChar, transition.GetSampleChar());
			State copyTarget = new State();
			Transition copy = transition.Copy(copyTarget);
			Assert.AreSame(copyTarget, copy.Target);
			Assert.AreSame(transition.GetType(), copy.GetType());
			Assert.AreEqual(transition.ToString(), copy.ToString());
		}
#endif
		#endregion
	}
	public sealed class OneSymbolTransition : Transition {
		readonly char input;
		public override bool IsMatch(char input) {
			return input == this.input;
		}
		OneSymbolTransition(State target, char input)
			: base(target) {
			this.input = input;
		}
		public OneSymbolTransition(char input)
			: base() {
			this.input = input;
		}
		public override Transition Copy(State target) {
			return new OneSymbolTransition(target, input);
		}
		public override string ToString() { return "'" + input + "'"; }
		public override bool IsExact { get { return true; } }
		public override char GetSampleChar() {
			return input;
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class OneSymbolTransitionTests {
		[Test]
		public void CommonTests() {
			new OneSymbolTransition('a').CommonTest("a", "Autofill", 'a');
			new OneSymbolTransition('B').CommonTest("B", "autofill", 'B');
		}
	}
#endif
	#endregion
	public sealed class AnySymbolTransition: Transition {
		public override bool IsMatch(char input) {
			return true;
		}
		AnySymbolTransition(State target)
			: base(target) {
		}
		public AnySymbolTransition()
			: base() {
		}
		public override Transition Copy(State target) {
			return new AnySymbolTransition(target);
		}
		public override string ToString() { return "any"; }
		public override char GetSampleChar() {
			return '.';
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class AnySymbolTransitionTests {
		[Test]
		public void CommonTests() {
			new AnySymbolTransition().CommonTest("`1234567890-=~!@#$%^&*()_+	qwertyuiop[]QWERTYUIOP{}|asdfghjkl;' ", "");
		}
	}
#endif
	#endregion
	public sealed class EmptyTransition: Transition {
		public override bool IsEmpty { get { return true; } }
		public override bool IsMatch(char input) {
			return false;
		}
		public EmptyTransition(State target)
			: base(target) {
		}
		public EmptyTransition()
			: base() {
		}
		public override Transition Copy(State target) {
			return new EmptyTransition(target);
		}
		public override string ToString() { return "empty"; }
		public override char GetSampleChar() {
			throw new InvalidOperationException(MaskExceptionsTexts.InternalErrorGetSampleCharForEmpty);
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class EmptyTransitionTests {
		[Test]
		public void CommonTests() {
			new EmptyTransition().CommonTest(null, "autofill");
		}
	}
#endif
	#endregion
	public struct BracketTransitionRange {
		public readonly char From;
		public readonly char To;
		public BracketTransitionRange(char from, char to) {
			this.From = from;
			this.To = to;
		}
	}
	public sealed class BracketTransition : Transition {
		readonly bool notMatch;
		readonly BracketTransitionRange[] ranges;
		public override bool IsMatch(char input) {
			foreach(BracketTransitionRange range in ranges) {
				if((input >= range.From && input <= range.To) || input == range.From || input == range.To) {	
					return !notMatch;
				}
			}
			return notMatch;
		}
		BracketTransition(State target, bool notMatch, BracketTransitionRange[] ranges)
			: base(target) {
			this.notMatch = notMatch;
			this.ranges = ranges;
		}
		public BracketTransition(bool notMatch, BracketTransitionRange[] ranges)
			: base() {
			this.notMatch = notMatch;
			this.ranges = ranges;
		}
		public override Transition Copy(State target) {
			return new BracketTransition(target, notMatch, (BracketTransitionRange[])ranges.Clone());
		}
		public override string ToString() {
			String rv = string.Empty;
			foreach(BracketTransitionRange range in ranges) {
				rv += range.From + "-" + range.To;
			}
			return string.Format("[{0}{1}]", notMatch ? "^" : string.Empty, rv);
		}
		public override bool IsExact {
			get {
				if(notMatch)
					return false;
				bool charLocked = false;
				char lockedChar = '\0';
				foreach(BracketTransitionRange range in ranges) {
					if(!charLocked) {
						charLocked = true;
						lockedChar = range.From;
					}
					if(lockedChar != range.From)
						return false;
					if(lockedChar != range.To)
						return false;
				}
				if(!charLocked)
					return false;
				return true;
			}
		}
		public override char GetSampleChar() {
			foreach(BracketTransitionRange range in ranges) {
				char testChar = range.From;
				if(IsMatch(testChar))
					return testChar;
				testChar = unchecked((char)(range.From - 1));
				if(IsMatch(testChar))
					return testChar;
				testChar = unchecked((char)(range.To + 1));
				if(IsMatch(testChar))
					return testChar;
			}
			return '?';
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class BracketTransitionTests {
		[Test, Ignore("TODO")]
		public void CommonTests() {
			new BracketTransition(false, new BracketTransitionRange[0]).CommonTest("", "no matches realy");
		}
	}
#endif
	#endregion
	public sealed class DecimalDigitTransition: Transition {
		readonly bool notMatch;
		public override bool IsMatch(char input) {
			bool match = input.GetUnicodeCategory() == UnicodeCategory.DecimalDigitNumber;
			return notMatch ? !match : match;
		}
		DecimalDigitTransition(State target, bool notMatch)
			: base(target) {
			this.notMatch = notMatch;
		}
		public DecimalDigitTransition(bool notMatch)
			: base() {
			this.notMatch = notMatch;
		}
		public override Transition Copy(State target) {
			return new DecimalDigitTransition(target, notMatch);
		}
		public override char GetSampleChar() {
			return notMatch ? '.' : '0';
		}
		public override string ToString() { return notMatch ? "\\D" : "\\d"; }
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class DecimalDigitTransitionTests {
		[Test]
		public void CommonTests() {
			new DecimalDigitTransition(false).CommonTest("0123456789", "`-=~!@#$%^&*()_+\t qwertyuiop[]QWERTYUIOP{}'\"\\|");
			new DecimalDigitTransition(true).CommonTest("`-=~!@#$%^&*()_+\t qwertyuiop[]QWERTYUIOP{}'\"\\|", "0123456789");
		}
	}
#endif
	#endregion
	public sealed class WordTransition: Transition {
		readonly bool notMatch;
		public override bool IsMatch(char input) {
			bool match;
			switch(input.GetUnicodeCategory()) {
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.ConnectorPunctuation:
					match = true;
					break;
				default:
					match = false;
					break;
			}
			return notMatch ? !match : match;
		}
		WordTransition(State target, bool notMatch)
			: base(target) {
			this.notMatch = notMatch;
		}
		public WordTransition(bool notMatch)
			: base() {
			this.notMatch = notMatch;
		}
		public override Transition Copy(State target) {
			return new WordTransition(target, notMatch);
		}
		public override char GetSampleChar() {
			return notMatch ? '.' : 'a';
		}
		public override string ToString() { return notMatch ? "\\W" : "\\w"; }
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class WordTransitionTests {
		[Test]
		public void CommonTests() {
			new WordTransition(false).CommonTest("0123456789_QWERTYqwertyЙЦУКЕНйцукен", "\"'[]\\{}|`~-=+ \t");
			new WordTransition(true).CommonTest("\"'[]\\{}|`~-=+ \t", "0123456789_QWERTYqwertyЙЦУКЕНйцукен");
		}
	}
#endif
	#endregion
	public sealed class WhiteSpaceTransition: Transition {
		readonly bool notMatch;
		public override bool IsMatch(char input) {
			bool match;
			switch(input.GetUnicodeCategory()) {
				case UnicodeCategory.LineSeparator:
				case UnicodeCategory.ParagraphSeparator:
				case UnicodeCategory.SpaceSeparator:
					match = true;
					break;
				default:
					switch(input) {
						case '\f':
						case '\n':
						case '\r':
						case '\t':
						case '\v':
						case '\x85':
							match = true;
							break;
						default:
							match = false;
							break;
					}
					break;
			}
			return notMatch ? !match : match;
		}
		WhiteSpaceTransition(State target, bool notMatch)
			: base(target) {
			this.notMatch = notMatch;
		}
		public WhiteSpaceTransition(bool notMatch)
			: base() {
			this.notMatch = notMatch;
		}
		public override Transition Copy(State target) {
			return new WhiteSpaceTransition(target, notMatch);
		}
		public override char GetSampleChar() {
			return notMatch ? '.' : ' ';
		}
		public override string ToString() { return notMatch ? "\\S" : "\\s"; }
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class WhiteSpaceTransitionTests {
		[Test]
		public void CommonTests() {
			new WhiteSpaceTransition(false).CommonTest(" \t\n\r", "0123456789_QWERTYqwertyЙЦУКЕНйцукен\"'[]\\{}|`~-=+");
			new WhiteSpaceTransition(true).CommonTest("0123456789_QWERTYqwertyЙЦУКЕНйцукен\"'[]\\{}|`~-=+", " \t\n\r");
		}
	}
#endif
	#endregion
	public sealed class UnicodeCategoryTransition: Transition {
		readonly bool notMatch;
		readonly string fCategory;
		readonly UnicodeCategory[] fCategoriesCodes;
		readonly static IDictionary<string, UnicodeCategory[]> fUnicodeCategoryNames;
		public override bool IsMatch(char input) {
			UnicodeCategory category = input.GetUnicodeCategory();
			bool match = false;
			foreach(UnicodeCategory categoryTested in fCategoriesCodes) {
				if(categoryTested == category) {
					match = true;
					break;
				}
			}
			return notMatch ? !match : match;
		}
		UnicodeCategoryTransition(State target, UnicodeCategoryTransition source)
			: base(target) {
			this.notMatch = source.notMatch;
			this.fCategory = source.fCategory;
			this.fCategoriesCodes = source.fCategoriesCodes;
		}
		public UnicodeCategoryTransition(string category, bool notMatch)
			: base() {
			this.notMatch = notMatch;
			this.fCategory = category;
			this.fCategoriesCodes = GetUnicodeCategoryListFromCharacterClassName(category);
			if(this.fCategoriesCodes == null)
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, MaskExceptionsTexts.IncorrectMaskInvalidUnicodeCategory, category));
		}
		public override Transition Copy(State target) {
			return new UnicodeCategoryTransition(target, this);
		}
		const string sampleChars = "\x41\x61\x1c5\x2b0\x1bb\x300\x903\x488\x30\x2160\xb2\x20\x2028\x2029\x9\x70f\xd800\xe000\x5f\x2d\x28\x29\xab\xbb\x21\x2b\x24\x5e\xa6\x220";
		public override char GetSampleChar() {
			foreach(char ch in sampleChars)
				if(IsMatch(ch))
					return ch;
			return '\0';
		}
		public override string ToString() { return "\\" + (notMatch ? 'P' : 'p') + '{' + fCategory + '}'; }
		static UnicodeCategoryTransition() {
			fUnicodeCategoryNames = new Dictionary<string, UnicodeCategory[]>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			foreach(UnicodeCategory category in EnumExtensions.GetValues(typeof(UnicodeCategory))) {
				fUnicodeCategoryNames.Add(category.ToString(), new UnicodeCategory[] { category });
			}
			fUnicodeCategoryNames.Add("Lu", new UnicodeCategory[] { UnicodeCategory.UppercaseLetter });
			fUnicodeCategoryNames.Add("Ll", new UnicodeCategory[] { UnicodeCategory.LowercaseLetter });
			fUnicodeCategoryNames.Add("Lt", new UnicodeCategory[] { UnicodeCategory.TitlecaseLetter });
			fUnicodeCategoryNames.Add("Lm", new UnicodeCategory[] { UnicodeCategory.ModifierLetter });
			fUnicodeCategoryNames.Add("Lo", new UnicodeCategory[] { UnicodeCategory.OtherLetter });
			fUnicodeCategoryNames.Add("L", new UnicodeCategory[] { UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherLetter });
			fUnicodeCategoryNames.Add("Letter", fUnicodeCategoryNames["L"]);
			fUnicodeCategoryNames.Add("Mn", new UnicodeCategory[] { UnicodeCategory.NonSpacingMark });
			fUnicodeCategoryNames.Add("Mc", new UnicodeCategory[] { UnicodeCategory.SpacingCombiningMark });
			fUnicodeCategoryNames.Add("Me", new UnicodeCategory[] { UnicodeCategory.EnclosingMark });
			fUnicodeCategoryNames.Add("M", new UnicodeCategory[] { UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark, UnicodeCategory.EnclosingMark });
			fUnicodeCategoryNames.Add("Mark", fUnicodeCategoryNames["M"]);
			fUnicodeCategoryNames.Add("Nd", new UnicodeCategory[] { UnicodeCategory.DecimalDigitNumber });
			fUnicodeCategoryNames.Add("Nl", new UnicodeCategory[] { UnicodeCategory.LetterNumber });
			fUnicodeCategoryNames.Add("No", new UnicodeCategory[] { UnicodeCategory.OtherNumber });
			fUnicodeCategoryNames.Add("N", new UnicodeCategory[] { UnicodeCategory.DecimalDigitNumber, UnicodeCategory.LetterNumber, UnicodeCategory.OtherNumber });
			fUnicodeCategoryNames.Add("Number", fUnicodeCategoryNames["N"]);
			fUnicodeCategoryNames.Add("Sm", new UnicodeCategory[] { UnicodeCategory.MathSymbol });
			fUnicodeCategoryNames.Add("Sc", new UnicodeCategory[] { UnicodeCategory.CurrencySymbol });
			fUnicodeCategoryNames.Add("Sk", new UnicodeCategory[] { UnicodeCategory.ModifierLetter });
			fUnicodeCategoryNames.Add("So", new UnicodeCategory[] { UnicodeCategory.OtherSymbol });
			fUnicodeCategoryNames.Add("S", new UnicodeCategory[] { UnicodeCategory.MathSymbol, UnicodeCategory.CurrencySymbol, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherSymbol });
			fUnicodeCategoryNames.Add("Symbol", fUnicodeCategoryNames["S"]);
			fUnicodeCategoryNames.Add("Pc", new UnicodeCategory[] { UnicodeCategory.ConnectorPunctuation });
			fUnicodeCategoryNames.Add("Pd", new UnicodeCategory[] { UnicodeCategory.DashPunctuation });
			fUnicodeCategoryNames.Add("Ps", new UnicodeCategory[] { UnicodeCategory.OpenPunctuation });
			fUnicodeCategoryNames.Add("Pe", new UnicodeCategory[] { UnicodeCategory.ClosePunctuation });
			fUnicodeCategoryNames.Add("Pi", new UnicodeCategory[] { UnicodeCategory.InitialQuotePunctuation });
			fUnicodeCategoryNames.Add("Pf", new UnicodeCategory[] { UnicodeCategory.FinalQuotePunctuation });
			fUnicodeCategoryNames.Add("Po", new UnicodeCategory[] { UnicodeCategory.OtherPunctuation });
			fUnicodeCategoryNames.Add("P", new UnicodeCategory[] { UnicodeCategory.ConnectorPunctuation, UnicodeCategory.DashPunctuation, UnicodeCategory.OpenPunctuation, UnicodeCategory.ClosePunctuation, UnicodeCategory.InitialQuotePunctuation, UnicodeCategory.FinalQuotePunctuation, UnicodeCategory.OtherPunctuation });
			fUnicodeCategoryNames.Add("Punctuation", fUnicodeCategoryNames["P"]);
			fUnicodeCategoryNames.Add("Zs", new UnicodeCategory[] { UnicodeCategory.SpaceSeparator });
			fUnicodeCategoryNames.Add("Zl", new UnicodeCategory[] { UnicodeCategory.LineSeparator });
			fUnicodeCategoryNames.Add("Zp", new UnicodeCategory[] { UnicodeCategory.ParagraphSeparator });
			fUnicodeCategoryNames.Add("Z", new UnicodeCategory[] { UnicodeCategory.SpaceSeparator, UnicodeCategory.LineSeparator, UnicodeCategory.ParagraphSeparator });
			fUnicodeCategoryNames.Add("Separator", fUnicodeCategoryNames["Z"]);
			fUnicodeCategoryNames.Add("Cc", new UnicodeCategory[] { UnicodeCategory.Control });
			fUnicodeCategoryNames.Add("Cf", new UnicodeCategory[] { UnicodeCategory.Format });
			fUnicodeCategoryNames.Add("Cs", new UnicodeCategory[] { UnicodeCategory.Surrogate });
			fUnicodeCategoryNames.Add("Co", new UnicodeCategory[] { UnicodeCategory.PrivateUse });
			fUnicodeCategoryNames.Add("Cn", new UnicodeCategory[] { UnicodeCategory.OtherNotAssigned });
			fUnicodeCategoryNames.Add("C", new UnicodeCategory[] { UnicodeCategory.Control, UnicodeCategory.Format, UnicodeCategory.Surrogate, UnicodeCategory.PrivateUse, UnicodeCategory.OtherNotAssigned });
			fUnicodeCategoryNames.Add("Other", fUnicodeCategoryNames["C"]);
		}
		public static UnicodeCategory[] GetUnicodeCategoryListFromCharacterClassName(string className) {
			className = className.Replace(" ", string.Empty);
			className = className.Replace("_", string.Empty);
			UnicodeCategory[] rv;
			if(fUnicodeCategoryNames.TryGetValue(className, out rv))
				return rv;
			else
				return null;
		}
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class UnicodeCategoryTransitionTests {
		[Test]
		public void CommonTests() {
			new UnicodeCategoryTransition("L", false).CommonTest("qwerQWERйцуЙЦУ", " \t1248(*&");
			new UnicodeCategoryTransition("Letter", false).CommonTest("qwerQWERйцуЙЦУ", " \t1248(*&");
			new UnicodeCategoryTransition("Lu", false).CommonTest("QWEЙЦУ", "qweйцу578 *(");
			new UnicodeCategoryTransition("Ll", false).CommonTest("qweйцу", "QWEЙЦУ578 *(");
			new UnicodeCategoryTransition("Nd", false).CommonTest("01234567890", "qwertyQWERTY(*&");
			new UnicodeCategoryTransition("Letter", true).CommonTest(" \t1248(*&", "qwerQWERйцуЙЦУ");
		}
	}
#endif
	#endregion
	sealed class StringKeyTable {
		readonly Dictionary<object, StringKey> inner = new Dictionary<object, StringKey>();
		public void Add(StringKey key) {
			inner.Add(key, key);
		}
		public StringKey this[SubstringWithHash index] {
			get {
				StringKey rv;
				inner.TryGetValue(index, out rv);
				return rv;
			}
		}
	}
	sealed class SubstringWithHash {
		public readonly string Str;
		public readonly int Length;
		public readonly int Hash;
		public SubstringWithHash(string str, int length, int hash) {
			this.Str = str;
			this.Length = length;
			this.Hash = hash;
		}
		public override int GetHashCode() {
			return Hash;
		}
		public override bool Equals(object obj) {
			return StringKey.IsEqual(Str, Length, (StringKey)obj);
		}
	}
	sealed class StringKey {
		readonly int Hash;
		public readonly int Length;
		public readonly StringKey Next;
		public readonly char Symbol;
		public readonly DfaWave Wave;
		public StringKey(StringKey next, char symbol, DfaWave wave) {
			Next = next;
			Symbol = symbol;
			if(next != null) {
				Hash = GetNextHash(next.Hash, symbol);
				Length = next.Length + 1;
			} else {
				Hash = HashSeed;
				Length = 0;
			}
			Wave = wave;
		}
		public override int GetHashCode() {
			return Hash;
		}
		public const int HashSeed = 0;
		public static int GetNextHash(int prevHash, char nextChar) {
			unchecked {
				return prevHash * 911 + (int)nextChar;
			}
		}
		public override bool Equals(object obj) {
			SubstringWithHash swh = obj as SubstringWithHash;
			if(swh != null)
				return IsEqual(swh.Str, swh.Length, this);
			else
				return IsEqual(this, (StringKey)obj);
		}
		static bool IsEqual(StringKey item, StringKey key) {
			if(item.Length != key.Length)
				return false;
			StringKey next = key;
			StringKey next2 = item;
			while(next.Next != null) {
				if(next2.Symbol != next.Symbol)
					return false;
				next = next.Next;
				next2 = next2.Next;
			}
			return true;
		}
		internal static bool IsEqual(string str, int length, StringKey key) {
			int count = length;
			if(key.Length != count)
				return false;
			StringKey next = key;
			while(next.Next != null) {
				count--;
				if(str[count] != next.Symbol)
					return false;
				next = next.Next;
			}
			return true;
		}
	}
	public class Dfa {
		State initialState;
		State finalState;
		readonly Dictionary<DfaWave, DfaWave> statesCache = new Dictionary<DfaWave, DfaWave>();
		readonly StringKeyTable stringsToStatesCache = new StringKeyTable();
		public static Dfa Empty { get { return new Dfa(); } }
		public static Dfa EmptyTransitionDfa { get { return new Dfa(new EmptyTransition()); } }
		Dfa() {
			this.initialState = new State();
			this.finalState = this.initialState;
			#region Mask_Tests
#if DEBUGTEST
			Check();
#endif
			#endregion
		}
		public Dfa(Transition initialTransition) {
			this.initialState = new State();
			this.initialState.AddTransition(initialTransition);
			this.finalState = initialTransition.Target;
			#region Mask_Tests
#if DEBUGTEST
			Check();
#endif
			#endregion
		}
		#region Mask_Tests
#if DEBUGTEST
		public void TestCtor() { TestCtorStatic(); }
		static void TestCtorStatic() {
			Transition tr = new EmptyTransition();
			Dfa testedDfa = new Dfa(tr);
			testedDfa.Check();
			Assert.AreSame(testedDfa.finalState, tr.Target);
			Assert.AreEqual(2, testedDfa.GetAllStates().Count);
		}
#endif
		#endregion
		bool CanReturnFromFinalState() {
			return object.ReferenceEquals(initialState, finalState)
				|| finalState.Transitions.Count > 0;
		}
		bool CanReturnToInitialState() {
			if(object.ReferenceEquals(initialState, finalState))
				return true;
			foreach(State state in GetAllStates())
				foreach(Transition transition in state.Transitions)
					if(object.ReferenceEquals(initialState, transition.Target))
						return true;
			return false;
		}
		static Dfa HardAnd(Dfa head, Dfa tail) {
			#region Mask_Tests
#if DEBUGTEST
			head.Check();
			tail.Check();
#endif
			#endregion
			Dfa newDfa = new Dfa(head);
			Dictionary<State, State> map = new Dictionary<State, State>();
			foreach(State state in tail.GetAllStates()) {
				State mappedState = object.ReferenceEquals(tail.initialState, state)
					? newDfa.finalState
					: new State();
				map.Add(state, mappedState);
			}
			newDfa.finalState = map[tail.finalState];
			foreach(State state in map.Keys) {
				foreach(Transition transition in state.Transitions) {
					State srcState = map[state];
					State targetState = map[transition.Target];
					srcState.AddTransition(transition.Copy(targetState));
				}
			}
			#region Mask_Tests
#if DEBUGTEST
			newDfa.Check();
#endif
			#endregion
			return newDfa;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestHardAnd() {
			Dfa left = new Dfa(new OneSymbolTransition('q'));
			Dfa right = new Dfa(new OneSymbolTransition('w'));
			Dfa result = Dfa.HardAnd(left, right);
			result.HardAndResultTest();
		}
		void HardAndResultTest() {
			Check();
			Assert.AreEqual(1, initialState.Transitions.Count);
			Assert.AreEqual('q', ((OneSymbolTransition)initialState[0]).GetSampleChar());
			Assert.AreEqual(1, ((OneSymbolTransition)initialState[0]).Target.Transitions.Count);
			Assert.AreEqual('w', ((OneSymbolTransition)((OneSymbolTransition)initialState[0]).Target[0]).GetSampleChar());
			Assert.AreSame(finalState, ((OneSymbolTransition)((OneSymbolTransition)initialState[0]).Target[0]).Target);
			Assert.AreEqual(0, finalState.Transitions.Count);
		}
#endif
		#endregion
		public static Dfa operator &(Dfa left, Dfa right) {
			if(right == null)
				return left;
			if(left == null)
				return right;
			if(left.CanReturnFromFinalState() && right.CanReturnToInitialState())
				left = HardAnd(left, EmptyTransitionDfa);
			Dfa workDfa = HardAnd(left, right);
			return workDfa;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("TODO")]
		public void TestAnd() {
		}
#endif
		#endregion
		static Dfa HardOr(Dfa one, Dfa merged) {
			#region Mask_Tests
#if DEBUGTEST
			one.Check();
			merged.Check();
#endif
			#endregion
			Dfa newDfa = new Dfa(one);
			Dictionary<State, State> map = new Dictionary<State, State>();
			foreach(State state in merged.GetAllStates()) {
				State mappedState;
				if(object.ReferenceEquals(merged.initialState, state))
					mappedState = newDfa.initialState;
				else if(object.ReferenceEquals(merged.finalState, state))
					mappedState = newDfa.finalState;
				else
					mappedState = new State();
				map.Add(state, mappedState);
			}
			foreach(State state in map.Keys) {
				foreach(Transition transition in state.Transitions) {
					State srcState = map[state];
					State targetState = map[transition.Target];
					srcState.AddTransition(transition.Copy(targetState));
				}
			}
			#region Mask_Tests
#if DEBUGTEST
			newDfa.Check();
#endif
			#endregion
			return newDfa;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestHardOr() {
			Dfa left = new Dfa(new OneSymbolTransition('q'));
			Dfa right = new Dfa(new OneSymbolTransition('w'));
			Dfa result = Dfa.HardOr(left, right);
			result.HardOrResultTest();
		}
		void HardOrResultTest() {
			Check();
			Assert.AreEqual(2, initialState.Transitions.Count);
			Assert.AreSame(finalState, ((Transition)initialState[0]).Target);
			Assert.AreSame(finalState, ((Transition)initialState[1]).Target);
			Assert.AreEqual(0, finalState.Transitions.Count);
		}
#endif
		#endregion
		public static Dfa operator |(Dfa left, Dfa right) {
			if(left.CanReturnToInitialState())
				left = HardAnd(EmptyTransitionDfa, left);
			if(right.CanReturnToInitialState())
				right = HardAnd(EmptyTransitionDfa, right);
			if(left.CanReturnFromFinalState())
				left = HardAnd(left, EmptyTransitionDfa);
			if(right.CanReturnFromFinalState())
				right = HardAnd(right, EmptyTransitionDfa);
			Dfa workDfa = HardOr(left, right);
			return workDfa;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("TODO")]
		public void TestOr() {
		}
#endif
		#endregion
		static Dfa Power0Unlimited(Dfa operand) {
			if(operand.CanReturnFromFinalState())
				operand = HardAnd(operand, EmptyTransitionDfa);
			if(operand.CanReturnToInitialState())
				operand = HardAnd(EmptyTransitionDfa, operand);
			return HardOr(Empty, operand);
		}
		static Dfa Power1Unlimited(Dfa operand) {
			Dfa workDfa = new Dfa(operand);
			workDfa.finalState.AddTransition(new EmptyTransition(workDfa.initialState));
			return workDfa;
		}
		static Dfa PowerExact(Dfa operand, int power) {
			if(power == 0)
				return null;
			if(power < 0)
				throw new ArgumentException("Incorrect power", "power");
			if(power == 1)
				return operand;
			int nestedPower = power / 2;
			Dfa workDfa = PowerExact(operand, nestedPower);
			workDfa = workDfa & workDfa;
			if(nestedPower + nestedPower != power)
				workDfa = workDfa & operand;
			return workDfa;
		}
		static Dfa PowerOptional(Dfa operand, int count) {
			if(count == 0)
				return null;
			Dfa repetitive = new Dfa(operand);
			if(repetitive.CanReturnFromFinalState())
				repetitive = HardAnd(repetitive, EmptyTransitionDfa);
			if(repetitive.CanReturnToInitialState())
				repetitive = HardAnd(EmptyTransitionDfa, repetitive);
			Dfa result = new Dfa(operand) | EmptyTransitionDfa;
			for(int i = 1; i < count; ++i) {
				Dictionary<State, State> map = new Dictionary<State, State>();
				foreach(State state in repetitive.GetAllStates()) {
					State mappedState;
					if(ReferenceEquals(state, repetitive.finalState))
						mappedState = result.initialState;
					else
						mappedState = new State();
					map.Add(state, mappedState);
				}
				result.initialState = map[repetitive.initialState];
				foreach(State state in map.Keys) {
					foreach(Transition transition in state.Transitions) {
						State srcState = map[state];
						State targetState = map[transition.Target];
						srcState.AddTransition(transition.Copy(targetState));
					}
				}
				result.initialState.AddTransition(new EmptyTransition(result.finalState));
			}
			return result;
		}
		public static Dfa Power(Dfa operand, int minMatches, int maxMatches) {
			if(ReferenceEquals(operand.initialState, operand.finalState))
				return operand;
			Dfa workDfa;
			if(maxMatches == -1) {
				if(minMatches == 0)
					workDfa = Power0Unlimited(operand);
				else
					workDfa = PowerExact(operand, minMatches - 1) & Power1Unlimited(operand);
			} else
				workDfa = PowerExact(operand, minMatches) & PowerOptional(operand, maxMatches - minMatches);
			if(workDfa == null)
				workDfa = Dfa.Empty;
#if DEBUGTEST
			workDfa.Check();
#endif
			return workDfa;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestPower0U() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 0, -1).Power0UTest();
		}
		void Power0UTest() {
			Check();
			Assert.AreEqual(1, GetAllStates().Count);
			Assert.AreEqual(1, initialState.Transitions.Count);
			Assert.AreSame(initialState, finalState);
			Assert.IsTrue(initialState[0] is AnySymbolTransition);
			Assert.AreSame(initialState, ((Transition)initialState[0]).Target);
		}
		[Test]
		public void TestPower1U() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 1, -1).Power1UTest();
		}
		void Power1UTest() {
			Check();
			Assert.AreEqual(2, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is EmptyTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(initialState, cs);
		}
		[Test]
		public void TestPower3U() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 3, -1).Power3UTest();
		}
		void Power3UTest() {
			Check();
			Assert.AreEqual(4, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			State returnPoint = cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is EmptyTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(returnPoint, cs);
		}
		[Test]
		public void TestPower03() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 0, 3).Power03Test();
		}
		void Power03Test() {
			Check();
			Assert.AreEqual(4, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(0, cs.Transitions.Count);
		}
		[Test]
		public void TestPower13() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 1, 3).Power13Test();
		}
		void Power13Test() {
			Check();
			Assert.AreEqual(4, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(0, cs.Transitions.Count);
		}
		[Test]
		public void TestPower23() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 2, 3).Power23Test();
		}
		void Power23Test() {
			Check();
			Assert.AreEqual(4, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(2, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			Assert.IsTrue(cs[1] is EmptyTransition);
			Assert.AreSame(finalState, ((Transition)cs[1]).Target);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(0, cs.Transitions.Count);
		}
		[Test]
		public void TestPower33() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 3, 3).Power33Test();
		}
		void Power33Test() {
			Check();
			Assert.AreEqual(4, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(0, cs.Transitions.Count);
		}
		[Test]
		public void TestPower11() {
			Dfa.Power(new Dfa(new AnySymbolTransition()), 1, 1).Power11Test();
		}
		void Power11Test() {
			Check();
			Assert.AreEqual(2, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is AnySymbolTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreSame(finalState, cs);
			Assert.AreEqual(0, cs.Transitions.Count);
		}
		[Test]
		public void TestBug_DB36094() {
			Dfa.Power(Dfa.Power(new Dfa(new OneSymbolTransition('-')) | new Dfa(new WordTransition(false)), 0, -1) & new Dfa(new WordTransition(false)), 0, -1).Bug_DB36094_Test();
		}
		private void Bug_DB36094_Test() {
			Check();
			Assert.AreSame(initialState, finalState);
			Assert.AreEqual(2, GetAllStates().Count);
			State cs = initialState;
			Assert.AreEqual(1, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is EmptyTransition);
			cs = ((Transition)cs[0]).Target;
			Assert.AreEqual(3, cs.Transitions.Count);
			Assert.IsTrue(cs[0] is OneSymbolTransition);
			Assert.AreSame(cs, ((Transition)cs[0]).Target);
			Assert.IsTrue(cs[1] is WordTransition);
			Assert.AreSame(cs, ((Transition)cs[1]).Target);
			Assert.IsTrue(cs[2] is WordTransition);
			Assert.AreSame(finalState, ((Transition)cs[2]).Target);
		}
#endif
		#endregion
		public ICollection<State> GetAllStates() {
			return initialState.GetReachableStates();
		}
		Dfa(Dfa source) {
			#region Mask_Tests
#if DEBUGTEST
			source.Check();
#endif
			#endregion
			Dictionary<State, State> mapping = new Dictionary<State, State>();
			foreach(State state in source.GetAllStates())
				mapping.Add(state, new State());
			this.initialState = mapping[source.initialState];
			this.finalState = mapping[source.finalState];
			foreach(State state in mapping.Keys) {
				foreach(Transition transition in state.Transitions) {
					Transition newTransition = transition.Copy(mapping[transition.Target]);
					(mapping[state]).AddTransition(newTransition);
				}
			}
			#region Mask_Tests
#if DEBUGTEST
#endif
			#endregion
		}
		#region Mask_Tests
#if DEBUGTEST
		[Ignore("TODO")]
		[Test]
		public void TestCtorCopy() { }
#endif
		#endregion
		public bool IsMatch(string input) {
			return GetWave(input).Contains(finalState);
		}
		public bool IsFinal(string input) {
			return GetWave(input).GetAutoCompleteInfo().DfaAutoCompleteType == DfaAutoCompleteType.Final;
		}
		public bool IsValidStart(string start) {
			return GetWave(start).Count > 0;
		}
		public static Dfa Parse(string pattern, CultureInfo cultureInfo) {
			return Parse(pattern, false, cultureInfo);
		}
		#region Mask_Tests
#if DEBUGTEST
		public static Dfa Parse(string pattern) {
			return Parse(pattern, CultureInfo.InvariantCulture);
		}
#endif
		#endregion
		public static Dfa Parse(string pattern, bool reverseAutomate, CultureInfo cultureInfo) {
			return RegExpParser.Parse(pattern, reverseAutomate, cultureInfo);
		}
		public static bool IsMatch(string input, string pattern, CultureInfo cultureInfo) {
			return Parse(pattern, cultureInfo).IsMatch(input);
		}
		#region Mask_Tests
#if DEBUGTEST
		public static bool IsMatch(string input, string pattern) {
			return IsMatch(input, pattern, CultureInfo.InvariantCulture); ;
		}
#endif
		#endregion
		string ToString(DfaWave wave) {
			ICollection<State> statesList = GetAllStates();
			Dictionary<State, string> states = new Dictionary<State, string>();
			int i = 0;
			foreach(State state in statesList) {
				string stateName = string.Empty;
				if(object.ReferenceEquals(initialState, state))
					stateName += 'i';
				if(object.ReferenceEquals(finalState, state))
					stateName += 'f';
				if(stateName.Length == 0) {
					++i;
					stateName = i.ToString();
				}
				states.Add(state, stateName);
			}
			string rv = string.Empty;
			if(wave != null) {
				rv += "Wave:";
				foreach(State waveElement in wave) {
					rv += ' ';
					rv += states[waveElement];
				}
				rv += Environment.NewLine;
			}
			rv += "Dfa:";
			foreach(State state in statesList) {
				foreach(Transition transition in state.Transitions) {
					rv += Environment.NewLine + states[state] + " " + transition.ToString() + " -> " + states[transition.Target];
				}
			}
			#region Mask_Tests
#if DEBUGTEST
#if !DXPORTABLE
			using(System.IO.StreamWriter sw = new System.IO.StreamWriter("Dfa.txt")) {
				sw.Write(rv);
			}
#endif
#endif
#endregion
			return rv;
		}
		public override string ToString() {
			return ToString(null);
		}
		DfaWave GetInitialStates() {
			DfaWave currentStates = new DfaWave(finalState);
			currentStates.AddStateWithEmptyTransitionsTargets(initialState);
			return currentStates;
		}
		public object[] GetPlaceHoldersInfo(string displayText) {
			return GetWave(displayText).GetPlaceHoldersInfo();
		}
		public string GetOptimisticHint(string displayText) {
			return GetWave(displayText).GetOptimisticHint();
		}
		public string GetPlaceHolders(string displayText, char anySymbolHolder) {
			string result = string.Empty;
			object[] placeHoldersInfo = GetPlaceHoldersInfo(displayText);
			foreach(object placeHolder in placeHoldersInfo)
				result += placeHolder == null ? anySymbolHolder : (char)placeHolder;
			return result;
		}
		#region Mask_Tests
#if DEBUGTEST
		void AssertPlaceholders(string expectedPlaceholders, string input) {
			Assert.AreEqual(expectedPlaceholders, GetPlaceHolders(input, '_'));
			Assert.AreEqual(expectedPlaceholders.Replace('_', '*'), GetPlaceHolders(input, '*'));
		}
		[Test]
		public void TestGetPlaceHolders() {
			TestGetPlaceHoldersStatic();
		}
		public static void TestGetPlaceHoldersStatic() {
			Dfa regExp;
			regExp = Dfa.Parse(@"\(qwe\)");
			regExp.AssertPlaceholders("(qwe)", "");
			regExp = Dfa.Parse(@"\(q.e\)");
			regExp.AssertPlaceholders("(q_e)", "");
			regExp = Dfa.Parse(@"\(qw?e\)");
			regExp.AssertPlaceholders("(qe)", "");
			regExp.AssertPlaceholders("e)", "(q");
			regExp.AssertPlaceholders("e)", "(qw");
			regExp.AssertPlaceholders(")", "(qe");
			regExp.AssertPlaceholders("", "(qe)");
			regExp.AssertPlaceholders("", "(qwe)");
			regExp = Dfa.Parse(@"(10|11|12|[1-9]):[0-5]\d:[0-5]\d(am|pm)");
			regExp.AssertPlaceholders("_:__:___m", "");
			regExp.AssertPlaceholders(":__:___m", "3");
			regExp.AssertPlaceholders(":__:___m", "1");
			regExp.AssertPlaceholders("__:___m", "1:");
			regExp.AssertPlaceholders(":__:___m", "10");
			regExp.AssertPlaceholders("m", "1:01:01a");
			regExp.AssertPlaceholders("", "1:01:01pm");
			regExp = Dfa.Parse(@"\d?\d\d(-\d\d)+");
			regExp.AssertPlaceholders("__-__", "");
			regExp.AssertPlaceholders("_-__", "0");
			regExp.AssertPlaceholders("-__", "01");
			regExp.AssertPlaceholders("-__", "012");
			regExp.AssertPlaceholders("__", "01-");
			regExp.AssertPlaceholders("__", "012-");
			regExp.AssertPlaceholders("", "01-33");
			regExp.AssertPlaceholders("", "012-33");
			regExp.AssertPlaceholders("__", "01-33-");
			regExp.AssertPlaceholders("__", "012-33-");
			regExp.AssertPlaceholders("_", "01-33-4");
			regExp.AssertPlaceholders("_", "012-33-4");
			regExp = Dfa.Parse(@"((10|11|12|[1-9]):[0-5]\d:[0-5]\d(am|pm))|((2[0-3]|[01]?[0-9]):[0-5][0-9]:[0-5][0-9])");
			regExp.AssertPlaceholders("_:__:__", "");
			regExp.AssertPlaceholders(":__:__", "0");
			regExp.AssertPlaceholders(":__:__", "1");
			regExp.AssertPlaceholders(":__:__", "3");
			regExp.AssertPlaceholders(":__:__", "13");
			regExp.AssertPlaceholders("", "12:01:01");
			regExp.AssertPlaceholders("m", "12:01:01a");
			regExp = Dfa.Parse(@"((10|11|12|[1-9]):[0-5]\d:[0-5]\d(am|pm))|((2[0-3]|[01][0-9]):[0-5][0-9]:[0-5][0-9])");
			regExp.AssertPlaceholders("________", "");
			regExp.AssertPlaceholders("_:__:__", "0");
			regExp.AssertPlaceholders("_______", "1");
			regExp.AssertPlaceholders(":__:___m", "3");
			regExp.AssertPlaceholders(":__:__", "13");
			regExp.AssertPlaceholders("", "12:01:01");
			regExp.AssertPlaceholders("m", "12:01:01a");
		}
		[Test]
		public void TestGetPlaceHoldersBug1() {
			Dfa regExp = Dfa.Parse(@"((-.-)+(-\d-)+)?");
			regExp.AssertPlaceholders("", "");
			regExp.AssertPlaceholders("-_-", "-0-");
		}
		[Test]
		public void Bug_B18642_UnexpectedPlacehordersForIpMaskTest() {
			Dfa regExp = Dfa.Parse(@"-\d?\d-\d?\d");
			regExp.AssertPlaceholders("-_-_", "");
			regExp.AssertPlaceholders("-_", "-1");
			regExp.AssertPlaceholders("-_", "-10");
			regExp.AssertPlaceholders("_", "-10-");
			regExp.AssertPlaceholders("", "-10-1");
			regExp.AssertPlaceholders("", "-10-11");
			regExp = Dfa.Parse(@"(2([0-4]\d|5[0-4])|([01]?\d?\d))\.(2([0-4]\d|5[0-4])|([01]?\d?\d))\.(2([0-4]\d|5[0-4])|([01]?\d?\d))\.(2([0-4]\d|5[0-4])|([01]?\d?\d))");
			regExp.AssertPlaceholders("_._._._", "");
			regExp.AssertPlaceholders("._._._", "2");
			regExp.AssertPlaceholders("._", "192.168.0");
		}
#endif
		#endregion
		StringKey CacheWave(StringKey next, char symbol, DfaWave candidateWave) {
			if(candidateWave.Count == 0)
				return new StringKey(next, symbol, candidateWave);
			DfaWave states;
			if(!statesCache.TryGetValue(candidateWave, out states)) {
				states = candidateWave;
				statesCache.Add(states, states);
			}
			StringKey key = new StringKey(next, symbol, states);
			stringsToStatesCache.Add(key);
			return key;
		}
		StringKey lastState;
		string lastText;
		DfaWave GetWave(string text) {
			if(lastText == text)
				return lastState.Wave;
			if(lastText != null && text.Length > 0 && lastText.StartsWith(text, StringComparison.Ordinal)) {
				StringKey curr = lastState;
				for(; curr.Length > text.Length; curr = curr.Next)
					;
				return curr.Wave;
			}
			int[] hashesByLength = new int[text.Length + 1];
			{
				int currentHash = StringKey.HashSeed;
				hashesByLength[0] = currentHash;
				for(int len = 1; len <= text.Length; ++len) {
					currentHash = StringKey.GetNextHash(currentHash, text[len - 1]);
					hashesByLength[len] = currentHash;
				}
			}
			StringKey currentState = stringsToStatesCache[new SubstringWithHash(text, text.Length, hashesByLength[text.Length])];
			if(currentState == null) {
				if(lastText != null && text.StartsWith(lastText)) {
					currentState = lastState;
				} else {
					currentState = stringsToStatesCache[new SubstringWithHash(text, 0, hashesByLength[0])];
				}
				if(currentState == null) {
					currentState = CacheWave(null, '\0', GetInitialStates());
				} else {
					int notFoundLen = text.Length;
					while(currentState.Length + 1 < notFoundLen) {
						int candidate = (currentState.Length + notFoundLen) / 2;
						StringKey found = stringsToStatesCache[new SubstringWithHash(text, candidate, hashesByLength[candidate])];
						if(found == null) {
							notFoundLen = candidate;
						} else {
							currentState = found;
						}
					}
				}
			}
			while(currentState.Length != text.Length) {
				char nextChar = text[currentState.Length];
				currentState = CacheWave(currentState, nextChar, currentState.Wave.GetNextWave(nextChar));
			}
			lastState = currentState;
			lastText = text;
			return currentState.Wave;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestGetWave() {
			Dfa regExp;
			DfaWave wave, wave1, wave2;
			regExp = Dfa.Parse(@".*");
			wave = regExp.GetWave("");
			Assert.AreSame(wave, regExp.GetWave(""));
			Assert.AreSame(wave, regExp.GetWave("q"));
			Assert.AreSame(wave, regExp.GetWave("w"));
			Assert.AreSame(wave, regExp.GetWave("qw"));
			regExp = Dfa.Parse(@"(a|ab)*");
			wave1 = regExp.GetWave("");
			wave2 = regExp.GetWave("a");
			Assert.IsTrue(!object.ReferenceEquals(wave1, wave2));
			Assert.AreSame(wave1, regExp.GetWave(""));
			Assert.AreSame(wave2, regExp.GetWave("a"));
			Assert.AreSame(wave1, regExp.GetWave("ab"));
			Assert.AreSame(wave2, regExp.GetWave("aba"));
			Assert.AreSame(wave1, regExp.GetWave("abab"));
			Assert.AreSame(wave2, regExp.GetWave("ababa"));
		}
#endif
		#endregion
		public AutoCompleteInfo GetAutoCompleteInfo(string text) {
			return GetWave(text).GetAutoCompleteInfo();
		}
		#region Mask_Tests
#if DEBUGTEST
		void Check() {
			Assert.IsNotNull(initialState);
			Assert.IsNotNull(finalState);
			bool initialStateContained = false;
			bool finalStateContained = false;
			foreach(State state in GetAllStates()) {
				if(object.ReferenceEquals(initialState, state))
					initialStateContained = true;
				if(object.ReferenceEquals(finalState, state))
					finalStateContained = true;
				bool notADeadWay = false;
				foreach(State reachableState in state.GetReachableStates()) {
					if(object.ReferenceEquals(finalState, reachableState)) {
						notADeadWay = true;
						break;
					}
				}
				Assert.IsTrue(notADeadWay);
			}
			Assert.IsTrue(initialStateContained);
			Assert.IsTrue(finalStateContained);
		}
		[Test]
		public void Bug_DB58057_invalidPowerOptionalResult() {
			Dfa dfa = Parse(@"(\d*\.?\d*){0,3}");
			dfa.Check();
		}
#endif
		#endregion
	}
		#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class DfaTests: Dfa {
		public DfaTests() : base(new EmptyTransition()) { }
	}
#endif
		#endregion
	public enum DfaAutoCompleteType { None, ExactChar, Final, FinalOrExactBeforeNone, FinalOrExactBeforeFinal, FinalOrExactBeforeFinalOrNone }
	public class AutoCompleteInfo {
		readonly DfaAutoCompleteType autoCompleteType;
		public DfaAutoCompleteType DfaAutoCompleteType { get { return autoCompleteType; } }
		readonly char autoCompleteChar;
		public char AutoCompleteChar { get { return autoCompleteChar; } }
		public AutoCompleteInfo(DfaAutoCompleteType autoCompleteType, char autoCompleteChar) {
			this.autoCompleteType = autoCompleteType;
			this.autoCompleteChar = autoCompleteChar;
		}
	}
	public class DfaWave : IEnumerable {
		readonly Dictionary<State, bool> states = new Dictionary<State,bool>();
		readonly State finalState;
		object[] placeHoldersInfo = null;
		string sample = null;
		bool hashCodeCalculated = false;
		int hashCode;
		AutoCompleteInfo autoCompleteInfo = null;
		public DfaWave(State finalState) {
			this.finalState = finalState;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestCtor() {
			State testedState = new State();
			DfaWave statesSet = new DfaWave(testedState);
			Assert.AreEqual(0, statesSet.states.Count);
			Assert.AreSame(testedState, statesSet.finalState);
			Assert.IsNull(statesSet.placeHoldersInfo);
			Assert.AreEqual(false, statesSet.hashCodeCalculated);
			Assert.IsNull(statesSet.autoCompleteInfo);
		}
#endif
		#endregion
		public int Count { get { return states.Count; } }
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestCount() {
			states.Clear();
			Assert.AreEqual(0, states.Count);
			Assert.AreEqual(0, Count);
			states.Add(new State(), false);
			Assert.AreEqual(1, Count);
			states.Add(new State(), false);
			Assert.AreEqual(2, Count);
		}
#endif
		#endregion
		public IEnumerator GetEnumerator() {
			return states.Keys.GetEnumerator();
		}
		public bool Contains(State state) {
			return states.ContainsKey(state);
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestContains() {
			states.Clear();
			State s1 = new State();
			State s2 = new State();
			states.Add(s1, false);
			Assert.AreEqual(true, Contains(s1));
			Assert.AreEqual(false, Contains(s2));
		}
#endif
		#endregion
		void Add(State state) {
			states.Add(state, false);
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestAdd() {
			states.Clear();
			State s1 = new State();
			State s2 = new State();
			Add(s1);
			Assert.AreEqual(1, states.Count);
			Assert.AreEqual(true, states.ContainsKey(s1));
			Assert.AreEqual(false, states.ContainsKey(s2));
		}
#endif
		#endregion
		public void AddStateWithEmptyTransitionsTargets(State state) {
			if(!Contains(state)) {
				Add(state);
				foreach(Transition transition in state.Transitions) {
					if(transition.IsEmpty)
						AddStateWithEmptyTransitionsTargets(transition.Target);
				}
			}
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void TestAddStateWithEmptyTransitionsTargets() {
			states.Clear();
			State s1 = new State();
			AddStateWithEmptyTransitionsTargets(s1);
			Assert.AreEqual(1, Count);
			Assert.AreEqual(true, Contains(s1));
			AddStateWithEmptyTransitionsTargets(s1);
			Assert.AreEqual(1, Count);
			Assert.AreEqual(true, Contains(s1));
			Transition t1 = new AnySymbolTransition();
			State s2 = t1.Target;
			s1.AddTransition(t1);
			states.Clear();
			AddStateWithEmptyTransitionsTargets(s1);
			Assert.AreEqual(1, Count);
			Assert.AreEqual(true, Contains(s1));
			states.Clear();
			AddStateWithEmptyTransitionsTargets(s2);
			Assert.AreEqual(1, Count);
			Assert.AreEqual(true, Contains(s2));
			Transition t2 = new AnySymbolTransition();
			State s3 = t2.Target;
			s2.AddTransition(t2);
			s1.AddTransition(new EmptyTransition(s2));
			s2.AddTransition(new EmptyTransition(s3));
			s3.AddTransition(new EmptyTransition(s2));
			states.Clear();
			AddStateWithEmptyTransitionsTargets(s1);
			Assert.AreEqual(3, Count);
			Assert.AreEqual(true, Contains(s1));
			Assert.AreEqual(true, Contains(s2));
			Assert.AreEqual(true, Contains(s3));
			states.Clear();
			AddStateWithEmptyTransitionsTargets(s2);
			Assert.AreEqual(2, Count);
			Assert.AreEqual(false, Contains(s1));
			Assert.AreEqual(true, Contains(s2));
			Assert.AreEqual(true, Contains(s3));
			states.Clear();
			AddStateWithEmptyTransitionsTargets(s3);
			Assert.AreEqual(2, Count);
			Assert.AreEqual(false, Contains(s1));
			Assert.AreEqual(true, Contains(s2));
			Assert.AreEqual(true, Contains(s3));
		}
#endif
		#endregion
		public DfaWave GetNextWave(char input) {
			DfaWave nextStates = new DfaWave(finalState);
			foreach(State state in this) {
				foreach(Transition transition in state.Transitions) {
					if(transition.IsMatch(input)) {
						nextStates.AddStateWithEmptyTransitionsTargets(transition.Target);
					}
				}
			}
			return nextStates;
		}
		#region GetPlaceHolders internals
		class PlaceHoldersPredictAssociation {
			public readonly object[] PlaceHolders;
			public readonly State State;
			public readonly PlaceHoldersPredictAssociation PassedStatesSource;
			public readonly string OptimisticHint;
			public PlaceHoldersPredictAssociation(State state) {
				this.PlaceHolders = new object[0];
				this.State = state;
				this.PassedStatesSource = null;
				this.OptimisticHint = string.Empty;
			}
			public PlaceHoldersPredictAssociation(PlaceHoldersPredictAssociation prevHolder, Transition transition) {
				if(transition.IsEmpty) {
					this.PlaceHolders = prevHolder.PlaceHolders;
					this.OptimisticHint = prevHolder.OptimisticHint;
				} else {
					this.PlaceHolders = new object[prevHolder.PlaceHolders.Length + 1];
					prevHolder.PlaceHolders.CopyTo(this.PlaceHolders, 0);
					if(transition.IsExact) {
						this.PlaceHolders[this.PlaceHolders.Length - 1] = transition.GetSampleChar();
					}
					this.OptimisticHint = prevHolder.OptimisticHint + transition.GetSampleChar();
				}
				this.State = transition.Target;
				this.PassedStatesSource = prevHolder;
			}
			bool IsPassed(State state) {
				if(this.State == state)
					return true;
				else if(this.PassedStatesSource != null)
					return this.PassedStatesSource.IsPassed(state);
				else
					return false;
			}
			static bool CanReachPast(State nextState, State targetState, State pastState, IDictionary<State, bool> states) {
				if(nextState == targetState)
					return true;
				if(nextState == pastState)
					return false;
				if(states.ContainsKey(nextState))
					return false;
				states.Add(nextState, true);
				foreach(Transition transition in nextState.Transitions)
					if(CanReachPast(transition.Target, targetState, pastState, states))
						return true;
				return false;
			}
			public bool CanSkip(State suspectState, State finalState) {
				if(IsPassed(suspectState))
					return true;
				if(suspectState == finalState)
					return false;
				foreach(Transition transition in State.Transitions) {
					if(transition.Target == suspectState)
						continue;
					if(!transition.IsEmpty)
						continue;
					if(!suspectState.CanReach(transition.Target))
						continue;
					if(!CanReachPast(suspectState, finalState, transition.Target, new Dictionary<State, bool>()))
						return true;
				}
				return false;
			}
		}
		ICollection<PlaceHoldersPredictAssociation> PlaceHoldersPredictGetNextHolders(ICollection<PlaceHoldersPredictAssociation> completeHolders, ICollection<PlaceHoldersPredictAssociation> currentHolders) {
			List<PlaceHoldersPredictAssociation> nextHolders = new List<PlaceHoldersPredictAssociation>();
			foreach(PlaceHoldersPredictAssociation candidate in currentHolders) {
				foreach(Transition transition in candidate.State.Transitions) {
					if(candidate.CanSkip(transition.Target, finalState))
						continue;
					PlaceHoldersPredictAssociation nextAssociation = new PlaceHoldersPredictAssociation(candidate, transition);
					if(object.ReferenceEquals(finalState, nextAssociation.State))
						completeHolders.Add(nextAssociation);
					else
						nextHolders.Add(nextAssociation);
				}
			}
			return nextHolders;
		}
		static object[] MergeMasks(object[] firstMask, object[] secondMask) {
			System.Diagnostics.Debug.Assert(firstMask.Length <= secondMask.Length);
			object[] host = firstMask;
			object[] newData = secondMask;
			int hostPosition = 0;
			int newDataPosition = 0;
			while(hostPosition < host.Length) {
				if(host[hostPosition] != null) {
					bool goodCharFound = false;
					for(int posInNewData = newDataPosition; newData.Length - posInNewData >= host.Length - hostPosition; ++posInNewData) {
						if(newData[posInNewData] != null && (char)host[hostPosition] == (char)newData[posInNewData]) {
							goodCharFound = true;
							break;
						}
					}
					if(!goodCharFound)
						host[hostPosition] = null;
				}
				++hostPosition;
				++newDataPosition;
			}
			return host;
		}
		void CalculatePlaceHoldersInfo() {
			if(placeHoldersInfo != null)
				return;
			List<PlaceHoldersPredictAssociation> completeHolders = new List<PlaceHoldersPredictAssociation>();
			ICollection<PlaceHoldersPredictAssociation> workHolders = new List<PlaceHoldersPredictAssociation>();
			foreach(State state in this) {
				if(object.ReferenceEquals(finalState, state)) {
					placeHoldersInfo = new object[0];
					sample = string.Empty;
					return;
				}
				workHolders.Add(new PlaceHoldersPredictAssociation(state));
			}
			while(workHolders.Count != 0) {
				workHolders = PlaceHoldersPredictGetNextHolders(completeHolders, workHolders);
			}
			if(completeHolders.Count == 0) {
				#region Mask_Tests
#if DEBUGTEST
				Assert.Fail("Final state should be reachable (by design)!");
#endif
				#endregion
				placeHoldersInfo = new object[0];
				sample = string.Empty;
				return;
			}
			completeHolders.Sort(delegate(PlaceHoldersPredictAssociation a, PlaceHoldersPredictAssociation b) { return a.PlaceHolders.Length - b.PlaceHolders.Length; });
			foreach(PlaceHoldersPredictAssociation association in completeHolders) {
				if(placeHoldersInfo == null) {
					placeHoldersInfo = association.PlaceHolders;
				} else {
					placeHoldersInfo = MergeMasks(placeHoldersInfo, association.PlaceHolders);
				}
				if(sample == null || sample.Length > association.OptimisticHint.Length || (sample.Length == association.OptimisticHint.Length && string.Compare(sample, association.OptimisticHint) > 0)) {
					sample = association.OptimisticHint;
				}
			}
		}
		#endregion
		public object[] GetPlaceHoldersInfo() {
			CalculatePlaceHoldersInfo();
			return placeHoldersInfo;
		}
		public string GetOptimisticHint() {
			CalculatePlaceHoldersInfo();
			return sample;
		}
		#region Mask_Tests
#if DEBUGTEST
#endif
		#endregion
		public override bool Equals(object obj) {
			if(obj == null)
				return false;
			if(object.ReferenceEquals(obj, this))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			if(((DfaWave)obj).Count != this.Count)
				return false;
			if(obj.GetHashCode() != this.GetHashCode())
				return false;
			foreach(State state in (DfaWave)obj)
				if(!this.Contains(state))
					return false;
			return true;
		}
		public override int GetHashCode() {
			if(hashCodeCalculated)
				return hashCode;
			hashCodeCalculated = true;
			hashCode = -1;
			foreach(State state in this)
				hashCode ^= state.GetHashCode();
			return hashCode;
		}
		struct GetAutoCompleteInfoTransitionsProcessingResult {
			public bool NonExactsFound;
			public bool ExactCharFound;
			public char ExactChar;
		}
		GetAutoCompleteInfoTransitionsProcessingResult GetAutoCompleteInfoTransitionsProcessing() {
			GetAutoCompleteInfoTransitionsProcessingResult result = new GetAutoCompleteInfoTransitionsProcessingResult();
			result.ExactCharFound = false;
			result.ExactChar = '\0';
			result.NonExactsFound = false;
			foreach(State state in this) {
				foreach(Transition transition in state.Transitions) {
					if(transition.IsEmpty) {
						continue;
					} else if(transition.IsExact) {
						if(result.ExactCharFound) {
							if(result.ExactChar != transition.GetSampleChar())
								result.NonExactsFound = true;
						} else {
							result.ExactCharFound = true;
							result.ExactChar = transition.GetSampleChar();
						}
					} else {
						result.NonExactsFound = true;
					}
					if(result.NonExactsFound)
						break;
				}
				if(result.NonExactsFound)
					break;
			}
			return result;
		}
		public AutoCompleteInfo GetAutoCompleteInfo() {
			if(autoCompleteInfo != null)
				return autoCompleteInfo;
			GetAutoCompleteInfoTransitionsProcessingResult transitionsProcessingResult = GetAutoCompleteInfoTransitionsProcessing();
			DfaAutoCompleteType returnType;
			if(transitionsProcessingResult.NonExactsFound) {
				returnType = DfaAutoCompleteType.None;
			} else if(transitionsProcessingResult.ExactCharFound) {
				if(!this.Contains(finalState)) {
					returnType = DfaAutoCompleteType.ExactChar;
				} else {
					GetAutoCompleteInfoTransitionsProcessingResult currentResult = transitionsProcessingResult;
					DfaWave currentWave = this;
					do {
						currentWave = currentWave.GetNextWave(currentResult.ExactChar);
						currentResult = currentWave.GetAutoCompleteInfoTransitionsProcessing();
					} while(!currentResult.NonExactsFound && currentResult.ExactCharFound && !currentWave.Contains(finalState));
					if(currentWave.Contains(finalState)) {
						if(currentResult.NonExactsFound)
							returnType = DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone;
						else
							returnType = DfaAutoCompleteType.FinalOrExactBeforeFinal;
					} else {
						returnType = DfaAutoCompleteType.FinalOrExactBeforeNone;
					}
				}
			} else {
				returnType = DfaAutoCompleteType.Final;
			}
			autoCompleteInfo = new AutoCompleteInfo(returnType, transitionsProcessingResult.ExactChar);
			return autoCompleteInfo;
		}
		#region Mask_Tests
#if DEBUGTEST
		[Test]
		public void GetAutoCompleteInfoBug1() {
			AutoCompleteInfo info = Dfa.Parse(@"(\.\d*)?").GetAutoCompleteInfo("");
			Assert.AreEqual(DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone, info.DfaAutoCompleteType);
		}
#endif
		#endregion
	}
	#region Mask_Tests
#if DEBUGTEST
	[TestFixture]
	public class DfaWaveTests: DfaWave {
		public DfaWaveTests() : base(new State()) { }
	}
#endif
	#endregion
}
