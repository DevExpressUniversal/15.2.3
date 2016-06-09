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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using DevExpress.XtraSpellChecker.Hunspell;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraSpellChecker.Native;
#if !SL && !WPF
using System.Drawing.Design;
#endif
#if SL
using System.IO.IsolatedStorage;
#endif
namespace DevExpress.XtraSpellChecker {
	#region FlagTypes
	public enum FlagTypes {
		OneChar,
		TwoChar,
		Number,
		UTF8
	}
	#endregion
	#region HunspellImportingInfo
	public class HunspellImportingInfo {
		Encoding encoding;
		FlagTypes flagType;
		readonly List<FlagsCollection> flagsSetCollection;
		public HunspellImportingInfo() {
			this.flagsSetCollection = new List<FlagsCollection>();
			Clear();
		}
		public Encoding Encoding { get { return encoding; } set { encoding = value; } }
		public FlagTypes FlagType { get { return flagType; } set { flagType = value; } }
		public List<FlagsCollection> FlagsSetCollection { get { return flagsSetCollection; } }
		public virtual void Clear() {
			this.flagsSetCollection.Clear();
			this.encoding = EmptyEncoding.Instance;
			this.flagType = FlagTypes.OneChar;
		}
	}
	#endregion
	#region HunspellDictionary
	public class HunspellDictionary : DictionaryBase, ICloseWordsCalculatorFactory {
		#region Fields
		internal readonly static string DefaultKeys = "qwertyuiop|asdfghjkl|zxcvbnm";
		readonly HunspellManager manager;
		HunspellImportingInfo importingInfo;
		string grammarPath;
		readonly SuffixCollection suffixes;
		readonly PreffixCollection preffixes;
		Dictionary<string, WordEntry> wordEntries;
		readonly List<CompoundRule> compoundRules;
		readonly List<CompoundPattern> compoundPatterns;
		readonly List<ICompoundWordValidationRule> compoundValidationRules;
		readonly Dictionary<HunspellFlags, HunspellFlag> flags;
		readonly ConversionTable conversionTable;
		readonly ConversionTable replacementTable;
		int compoundMin;
		bool checkCompoundTriple;
		bool simplifiedTriple;
		int compoundWordMax;
		char[] alphabetChars;
		string keys;
		#endregion
		public HunspellDictionary(string dictionaryPath, string grammarPath, CultureInfo culture)
			: base(dictionaryPath, culture) {
			this.grammarPath = grammarPath;
			this.manager = new HunspellManager(this);
			this.suffixes = new SuffixCollection();
			this.preffixes = new PreffixCollection();
			this.wordEntries = new Dictionary<string, WordEntry>();
			this.compoundRules = new List<CompoundRule>();
			this.compoundPatterns = new List<CompoundPattern>();
			this.compoundValidationRules = new List<ICompoundWordValidationRule>();
			this.flags = new Dictionary<HunspellFlags, HunspellFlag>();
			this.conversionTable = new ConversionTable();
			this.replacementTable = new ConversionTable();
			Clear();
			this.importingInfo = new HunspellImportingInfo();
		}
		public HunspellDictionary()
			: this(String.Empty, String.Empty, CultureInfo.InvariantCulture) {
		}
		#region Properties
#if !SL && !WPF
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("HunspellDictionaryGrammarPath"),
#endif
		Editor("DevExpress.XtraSpellChecker.Design.FileNameEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))]
#endif
		public string GrammarPath { get { return grammarPath; } set { grammarPath = value; } }
		protected internal HunspellManager Manager { get { return manager; } }
		protected internal int CompoundMin { get { return compoundMin; } set { compoundMin = value; } }
		protected internal bool CheckCompoundTriple { get { return checkCompoundTriple; } set { checkCompoundTriple = value; } }
		protected internal bool SimplifiedTriple { get { return simplifiedTriple; } set { simplifiedTriple = value; } }
		protected internal int CompoundWordMax { get { return compoundWordMax; } set { compoundWordMax = value; } }
		protected internal PreffixCollection Prefixes { get { return preffixes; } }
		protected internal SuffixCollection Suffixes { get { return suffixes; } }
		protected internal Dictionary<string, WordEntry> WordEntries { get { return wordEntries; } }
		protected internal List<CompoundRule> CompoundRules { get { return compoundRules; } }
		protected internal List<CompoundPattern> CompoundPatterns { get { return compoundPatterns; } }
		protected internal List<ICompoundWordValidationRule> CompoundValidationRules { get { return compoundValidationRules; } }
		protected internal Dictionary<HunspellFlags, HunspellFlag> Flags { get { return flags; } }
		protected internal ConversionTable ConversionTable { get { return conversionTable; } }
		protected internal ConversionTable ReplacementTable { get { return replacementTable; } }
		protected internal string Keys { get { return keys; } set { keys = value; } }
		public override int WordCount { get { return WordEntries.Count; } }
		#endregion
		public override void Clear() {
			this.suffixes.Clear();
			this.preffixes.Clear();
			this.wordEntries = new Dictionary<string, WordEntry>();
			this.compoundRules.Clear();
			this.compoundPatterns.Clear();
			this.flags.Clear();
			this.compoundMin = 3;
			this.checkCompoundTriple = false;
			this.simplifiedTriple = false;
			this.compoundWordMax = Int32.MaxValue;
			this.conversionTable.Clear();
			this.replacementTable.Clear();
			this.keys = DefaultKeys;
		}
		public override bool FindWord(string word) {
			return Manager.CheckWord(word);
		}
		public override bool Contains(string word) {
			return Manager.CheckWordCore(Manager.PrepareWordToCheck(word)) != null;
		}
		protected override void BeginLoad() {
			base.BeginLoad();
			this.importingInfo.Clear();
		}
		protected internal virtual void LoadGrammarFromStream(Stream stream) {
			AffixImporter affixImporter = new AffixImporter(this, this.importingInfo);
			affixImporter.Load(stream);
		}
		protected internal virtual void LoadGrammarFromFile(string path) {
			using (Stream stream = GetStreamFromPath(path)) {
				LoadGrammarFromStream(stream);
			}
		}
		protected internal virtual void LoadWordEntriesFromStream(Stream stream) {
			DictionaryImporter dictionaryImporter = new DictionaryImporter(this, this.importingInfo);
			dictionaryImporter.Load(stream);
		}
		protected internal virtual void LoadWordEntriesFromFile(string path) {
			using (Stream stream = GetStreamFromPath(path)) {
				LoadWordEntriesFromStream(stream);
			}
		}
		protected internal virtual bool IsCompoundDefined() {
			return Flags.ContainsKey(HunspellFlags.Compound) || Flags.ContainsKey(HunspellFlags.CompoundStart);
		}
		protected internal virtual bool CanBeSeparateWord(WordEntry wordEntry, AffixEntry affix) {
			return !CheckFlag(HunspellFlags.OnlyCompound, wordEntry, affix);
		}
		protected internal virtual bool OnlyCompound(IFlagedObject obj) {
			return CheckFlag(obj, HunspellFlags.OnlyCompound);
		}
		protected internal virtual bool IsAffixPermitted(AffixEntry affix) {
			return CheckFlag(affix, HunspellFlags.CompoundPermit);
		}
		protected internal virtual bool IsAffixForbidden(AffixEntry affix) {
			return CheckFlag(affix, HunspellFlags.CompoundForbid);
		}
		protected internal virtual bool NeedAffix(IFlagedObject obj) {
			return CheckFlag(obj, HunspellFlags.NeedAffix);
		}
		protected internal virtual bool CheckFlag(IFlagedObject obj, HunspellFlags flag) {
			return Flags.ContainsKey(flag) && obj.ContainsFlag(Flags[flag]);
		}
		protected internal virtual bool CheckCompoundingFlags(WordEntryType options, WordEntry wordEntry, AffixEntry affix) {
			switch (options) {
				case WordEntryType.Beginning:
					return CheckCompoundingStart(wordEntry, affix);
				case WordEntryType.Middle:
					return CheckCompoundingMiddle(wordEntry, affix);
				case WordEntryType.Ending:
					return CheckCompoundingEnd(wordEntry, affix);
				default:
					return false;
			}
		}
		bool CheckCompoundingStart(WordEntry wordEntry, AffixEntry affix) {
			return CheckFlag(HunspellFlags.Compound, wordEntry, affix) || CheckFlag(HunspellFlags.CompoundStart, wordEntry, affix);
		}
		bool CheckCompoundingMiddle(WordEntry wordEntry, AffixEntry affix) {
			return CheckFlag(HunspellFlags.Compound, wordEntry, affix) || CheckFlag(HunspellFlags.CompoundMiddle, wordEntry, affix);
		}
		bool CheckCompoundingEnd(WordEntry wordEntry, AffixEntry affix) {
			return CheckFlag(HunspellFlags.Compound, wordEntry, affix) || CheckFlag(HunspellFlags.CompoundEnd, wordEntry, affix);
		}
		protected internal virtual bool CheckFlag(HunspellFlags flag, WordEntry wordEntry, AffixEntry affix) {
			return CheckFlag(wordEntry, flag) || (affix != null && CheckFlag(affix, flag));
		}
		protected internal void SetCapacity(int capacity) {
			Debug.Assert(wordEntries.Count == 0);
			this.wordEntries = new Dictionary<string, WordEntry>(capacity);
		}
		protected internal void SetAlphabetChars(char[] alphabet) {
			this.alphabetChars = alphabet;
		}
		protected internal virtual WordEntry FindWordEntry(string root) {
			WordEntry result;
			if (wordEntries.TryGetValue(root, out result))
				return result;
			return null;
		}
		CloseWordsCalculatorBase ICloseWordsCalculatorFactory.Create() {
			return new HunspellCloseWordsCalculator(this);
		}
		protected override bool GetCaseSensitive() {
			return true;
		}
		protected override char[] GetAlphabetChars() {
			return this.alphabetChars;
		}
		protected override bool LoadCore() {
			if (!CanOpenFile(DictionaryPath) || !CanOpenFile(GrammarPath))
				return false;
			try {
				LoadGrammarFromFile(GrammarPath);
				LoadWordEntriesFromFile(DictionaryPath);
			}
			catch {
				Clear();
				throw;
			}
			return true;
		}
		public virtual void LoadFromStream(Stream dictionaryStream, Stream grammarStream) {
			BeginLoad();
			if (dictionaryStream == null)
				throw new ArgumentNullException("dictionaryStream");
			if (grammarStream == null)
				throw new ArgumentNullException("grammarStream");
			try {
				LoadGrammarFromStream(grammarStream);
				LoadWordEntriesFromStream(dictionaryStream);
			}
			catch {
				Clear();
				throw;
			}
			EndLoad();
		}
	}
	#endregion
	#region HunspellFlags
	[Flags]
	public enum HunspellFlags {
		None = 0x00000000,
		Compound = 0x00000001,
		CompoundStart = 0x00000002,
		CompoundMiddle = 0x00000004,
		CompoundEnd = 0x00000008,
		CompoundPermit = 0x00000010,
		CompoundForbid = 0x00000020,
		OnlyCompound = 0x00000040,
		NeedAffix = 0x00000080,
		KeepCase = 0x00000100,
		ForbiddenWord = 0x00000200
	}
	#endregion
	#region AffixCollection<T>
	public class AffixCollection<T> where T : AffixEntry {
		static readonly List<T> EmptyList = new List<T>();
		#region Fields
		readonly Dictionary<string, List<T>> affixTable;
		readonly Dictionary<HunspellFlag, List<T>> idToAffixMapTable;
		#endregion
		public AffixCollection() {
			this.affixTable = new Dictionary<string, List<T>>();
			this.idToAffixMapTable = new Dictionary<HunspellFlag, List<T>>();
		}
		#region Properties
		#endregion
		public virtual void Add(T affix) {
			string key = affix.StringToAppend;
			AddCore(key, affix);
			AddIdToAffixMapItem(affix);
		}
		void AddIdToAffixMapItem(T affix) {
			List<T> affixes;
			if (!this.idToAffixMapTable.TryGetValue(affix.Identifier, out affixes)) {
				affixes = new List<T>();
				this.idToAffixMapTable.Add(affix.Identifier, affixes);
			}
			affixes.Add(affix);
		}
		protected virtual void AddCore(string key, T affix) {
			List<T> affixes;
			if (!affixTable.TryGetValue(key, out affixes)) {
				affixes = new List<T>();
				affixTable.Add(key, affixes);
			}
			affixes.Add(affix);
		}
		public virtual List<T> GetAffixes(string affix) {
			if (affixTable.ContainsKey(affix))
				return affixTable[affix];
			return EmptyList;
		}
		public virtual List<T> GetAffixes(HunspellFlag id) {
			if (this.idToAffixMapTable.ContainsKey(id))
				return this.idToAffixMapTable[id];
			return EmptyList;
		}
		public virtual void Clear() {
			this.affixTable.Clear();
		}
	}
	#endregion
	#region SuffixCollection
	public class SuffixCollection : AffixCollection<Suffix> {
		readonly FlagsCollection flags = new FlagsCollection();
		public override void Add(Suffix affix) {
			base.Add(affix);
			FlagsCollection sfxFlags = affix.Flags;
			if (sfxFlags != null && sfxFlags.Count > 0)
				flags.AddSortedRange(sfxFlags);
		}
		public virtual bool CanCombineWithOtherSuffix(Suffix suffix) {
			return flags.ContainsFlag(suffix.Identifier);
		}
	}
	#endregion
	#region PreffixCollection
	public class PreffixCollection : AffixCollection<Prefix> {
	}
	#endregion
	#region IntegerFlag
	public struct HunspellFlag : IComparable<HunspellFlag> {
		public static HunspellFlag Empty = new HunspellFlag(-1);
		readonly int value;
		public HunspellFlag(int value) {
			this.value = value;
		}
		public override int GetHashCode() {
			return value;
		}
		public override string ToString() {
			return value.ToString();
		}
		public override bool Equals(object obj) {
			return obj is HunspellFlag && ((HunspellFlag)obj).value == this.value;
		}
		public static bool operator ==(HunspellFlag flag1, HunspellFlag flag2) {
			return flag1.value == flag2.value;
		}
		public static bool operator !=(HunspellFlag flag1, HunspellFlag flag2) {
			return flag1.value != flag2.value;
		}
		#region IComparable<IntegerFlag> Members
		public int CompareTo(HunspellFlag other) {
			return value - other.value;
		}
		#endregion
	}
	#endregion
	#region FlagsCollection
	public class FlagsCollection : List<HunspellFlag> {
		public HunspellFlag First { get { return Count > 0 ? this[Count - 1] : HunspellFlag.Empty; } }
		public virtual void AddRange(List<HunspellFlag> flags) {
			int count = flags.Count;
			for (int i = 0; i < count; i++)
				Add(flags[i]);
		}
		public virtual void AddSortedRange(List<HunspellFlag> flags) {
			int count = flags.Count;
			for (int i = 0; i < count; i++) {
				int index = BinarySearch(flags[i]);
				if (index < 0) {
					index = ~index;
					Insert(index, flags[i]);
				}
			}
		}
		protected internal virtual bool ContainsFlag(HunspellFlag flag) {
			if (Count == 0)
				return false;
			int left = 0;
			int right = Count - 1;
			while (left <= right) {
				int mid = (left + right) >> 1;
				int comparerResult = flag.CompareTo(this[mid]);
				if (comparerResult == 0)
					return true;
				if (comparerResult < 0)
					right = mid - 1;
				else
					left = mid + 1;
			}
			return false;
		}
	}
	#endregion
	#region IFlagedObject
	public interface IFlagedObject {
		FlagsCollection Flags { get; set; }
		bool ContainsFlag(HunspellFlag flag);
	}
	#endregion
	#region AffixEntry
	public abstract class AffixEntry : IFlagedObject {
		#region IAffixCondition interface
		protected internal interface IAffixCondition {
			bool IsMatch(char ch);
		}
		#endregion
		#region AffixCondition class
		protected internal class AffixCondition : IAffixCondition {
			readonly List<char> chars;
			bool shouldBeMet;
			public AffixCondition(char[] chars, bool shouldBeMet) {
				this.chars = new List<char>(chars);
				this.chars.Sort();
				this.shouldBeMet = shouldBeMet;
			}
			public List<char> Chars { get { return chars; } }
			public bool ShouldBeMet { get { return shouldBeMet; } }
			public bool IsMatch(char ch) {
				return (shouldBeMet) ? Contains(ch) : !Contains(ch);
			}
			protected bool Contains(char ch) {
				return chars.BinarySearch(ch) >= 0;
			}
		}
		#endregion
		#region EmptyAffixCondition class
		protected internal class EmptyAffixCondition : IAffixCondition {
			public bool IsMatch(char ch) {
				return true;
			}
		}
		#endregion
		#region Fields
		const int AvailableCharsIndex = 0;
		const int UnavailableCharsIndex = 1;
		FlagsCollection flags;
		bool canCombine;
		HunspellFlag identifier;
		string stringToStrip = String.Empty;
		string stringToAppend = String.Empty;
		List<IAffixCondition> conditions;
		AffixEntry nextEntry;
		#endregion
		protected AffixEntry(HunspellFlag identifier, bool canCombine) {
			this.identifier = identifier;
			this.canCombine = canCombine;
			this.flags = new FlagsCollection();
			this.conditions = new List<IAffixCondition>();
		}
		#region Properties
		public HunspellFlag Identifier { get { return identifier; } }
		public bool CanCombine { get { return canCombine; } }
		public FlagsCollection Flags { get { return flags; } set { flags = value; } }
		public string StringToStrip { get { return stringToStrip; } set { stringToStrip = value; } }
		public string StringToAppend { get { return stringToAppend; } set { stringToAppend = value; } }
		public AffixEntry NextEntry { get { return nextEntry; } set { nextEntry = value; } }
		protected internal List<IAffixCondition> Conditions { get { return conditions; } }
		#endregion
		protected internal void SetCondition(char[] chars, bool shouldBeMet) {
			conditions.Add(new AffixCondition(chars, shouldBeMet));
		}
		protected internal void SetCondition() {
			conditions.Add(new EmptyAffixCondition());
		}
		public bool ContainsFlag(HunspellFlag flag) {
			return Flags.ContainsFlag(flag);
		}
		public virtual bool TryGetWordRoot(string word, out string root) {
			root = String.Empty;
			if (word.Length < StringToAppend.Length)
				return false;
			string wordRoot = ExtractWordRoot(word);
			if (!IsWordRootFit(wordRoot))
				return false;
			root = wordRoot;
			return true;
		}
		protected internal bool IsWordRootFit(string wordRoot) {
			if (wordRoot.Length < Conditions.Count)
				return false;
			return IsWordRootFitCore(wordRoot);
		}
		public string Combine(string wordRoot) {
			if (!IsWordRootFit(wordRoot))
				return null;
			return CombineCore(wordRoot);
		}
		protected abstract bool IsWordRootFitCore(string wordRoot);
		protected abstract string ExtractWordRoot(string word);
		protected abstract string CombineCore(string wordRoot);
	}
	#endregion
	#region Preffix
	public class Prefix : AffixEntry {
		public Prefix(HunspellFlag key, bool canCombine)
			: base(key, canCombine) {
		}
		protected override bool IsWordRootFitCore(string wordRoot) {
			int count = Conditions.Count;
			for (int index = 0; index < count; index++) {
				IAffixCondition condition = Conditions[index];
				if (!condition.IsMatch(wordRoot[index]))
					return false;
			}
			return true;
		}
		protected override string ExtractWordRoot(string word) {
			return StringToStrip + word.Substring(StringToAppend.Length);
		}
		protected override string CombineCore(string wordRoot) {
			return StringToAppend + wordRoot.Substring(StringToStrip.Length);
		}
	}
	#endregion
	#region Suffix
	public class Suffix : AffixEntry {
		public Suffix(HunspellFlag key, bool canCombine)
			: base(key, canCombine) {
		}
		protected override bool IsWordRootFitCore(string wordRoot) {
			int rootLenght = wordRoot.Length;
			int count = Conditions.Count;
			for (int index = 0; index < count; index++) {
				IAffixCondition condition = Conditions[index];
				if (!condition.IsMatch(wordRoot[rootLenght - count + index]))
					return false;
			}
			return true;
		}
		protected override string ExtractWordRoot(string word) {
			return word.Substring(0, word.Length - StringToAppend.Length) + StringToStrip;
		}
		protected override string CombineCore(string wordRoot) {
			return wordRoot.Substring(0, wordRoot.Length - StringToStrip.Length) + StringToAppend;
		}
	}
	#endregion
	#region FlagedObject
	public class FlagedObject : IFlagedObject {
		string _value = String.Empty;
		FlagsCollection flags;
		public FlagedObject(FlagTypes type) {
			this.flags = new FlagsCollection();
		}
		public string Value { get { return _value; } set { _value = value; } }
		public FlagsCollection Flags { get { return flags; } set { flags = value; } }
		public bool IsValid { get { return Value != null; } }
		public bool ContainsFlag(HunspellFlag flag) {
			return Flags.ContainsFlag(flag);
		}
	}
	#endregion
	#region WordEntry
	public class WordEntry : FlagedObject {
		WordEntry nextEntry;
		public WordEntry(FlagTypes type)
			: base(type) {
		}
		public WordEntry NextEntry { get { return nextEntry; } set { nextEntry = value; } }
		public override string ToString() {
			return this.Value;
		}
		public override int GetHashCode() {
			return Value.GetHashCode();
		}
	}
	#endregion
	#region ConversionTable
	public class ConversionTable {
		readonly List<KeyValuePair<string, string>> innerList;
		public ConversionTable() {
			this.innerList = new List<KeyValuePair<string, string>>();
		}
		public bool IsEmpty { get { return innerList.Count == 0; } }
		protected internal KeyValuePair<string, string> this[int index] { get { return innerList[index]; } }
		public virtual void Add(string pattern, string conversion) {
			int indexToInsert = MatchCore(pattern, 0);
			if (indexToInsert < 0)
				indexToInsert = ~indexToInsert;
			KeyValuePair<string, string> newItem = new KeyValuePair<string, string>(pattern, conversion);
			innerList.Insert(indexToInsert, newItem);
		}
		protected internal virtual int Match(string word) {
			int startIndex = 0;
			int result = -1;
			for (int length = 1; length <= word.Length; length++) {
				int index = MatchCore(word.Substring(0, length), startIndex);
				if (index < 0) {
					if (result >= 0)
						break;
					index = ~index;
					if (index >= innerList.Count)
						break;
					bool canIncreaseLength = length < innerList[index].Key.Length;
					if (!canIncreaseLength)
						break;
					startIndex = index;
				}
				else {
					result = index;
					startIndex = index + 1;
				}
			}
			return result;
		}
		protected virtual int MatchCore(string word, int satrtIndex) {
			int low = satrtIndex;
			int hi = innerList.Count - 1;
			while (low <= hi) {
				int m = low + ((hi - low) >> 1);
				int c = String.Compare(word, innerList[m].Key, StringComparison.Ordinal);
				if (c == 0)
					return m;
				if (c < 0)
					hi = m - 1;
				else
					low = m + 1;
			}
			return ~low;
		}
		public virtual string Convert(string word) {
			StringBuilder result = new StringBuilder();
			int count = word.Length;
			int charIndex = 0;
			while (charIndex < count) {
				int index = Match(word.Substring(charIndex));
				if (index >= 0) {
					string key = innerList[index].Key;
					result.Append(innerList[index].Value);
					charIndex += key.Length;
				}
				else {
					result.Append(word[charIndex]);
					charIndex++;
				}
			}
			return result.ToString();
		}
		public virtual void Clear() {
			this.innerList.Clear();
		}
	}
	#endregion
}
