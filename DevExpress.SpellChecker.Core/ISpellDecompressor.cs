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
using System.IO;
using System.Globalization;
using DevExpress.XtraSpellChecker;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DevExpress.XtraSpellChecker.Native {
	public interface IAffixCondition {
		bool IsMatch(char ch);
	}
	#region AffixCondition
	public class AffixCondition : IAffixCondition {
		readonly char[] chars;
		readonly bool shouldBeMet;
		public AffixCondition(char[] chars, bool shouldBeMet) {
			this.chars = chars;
			this.shouldBeMet = shouldBeMet;
		}
		public AffixCondition(string str, bool shouldBeMet)
			: this(str.ToCharArray(), shouldBeMet) {
		}
		public AffixCondition(char ch, bool shouldBeMet)
			: this(new char[] { ch }, shouldBeMet) {
		}
		protected internal char[] Chars { get { return chars; } }
		protected internal bool ShouldBeMet { get { return shouldBeMet; } }
		public virtual bool IsMatch(char ch) {
			int count = Chars.Length;
			for (int i = 0; i < count; i++) {
				if (this.chars[i] == ch)
					return ShouldBeMet;
			}
			return !ShouldBeMet;
		}
	}
	#endregion
	#region EmptyAffixCondition
	public class EmptyAffixCondition : IAffixCondition {
		public bool IsMatch(char ch) {
			return true;
		}
	}
	#endregion
	public enum AffixElementKind { Prefix, Suffix, None }
	public abstract class AffixElement {
		string deletedString = string.Empty;
		List<AffixElement> additions = new List<AffixElement>();
		string key = string.Empty;
		string element = string.Empty;
		bool canCombine;
		string[] flags;
		List<IAffixCondition> conditions = new List<IAffixCondition>();
		protected AffixElement(string key) {
			this.key = key;
		}
		public List<AffixElement> Additions { get { return additions; } }
		public List<IAffixCondition> Conditions { get { return conditions; } }
		public string DeletedString { get { return deletedString; } set { deletedString = value; } }
		public string Element { get { return element; } set { element = value; } }
		public bool CanCombine { get { return canCombine; } set { canCombine = value; } }
		public string[] Flags { get { return flags; } set { flags = value; } }
		public string Key { get { return key; } }
		protected virtual void AddAffixElement(AffixElement element) {
			element.CanCombine = CanCombine;
			Additions.Add(element);
		}
		public abstract AffixElement AddAffixElement();
		protected virtual void RemoveAffixElement(AffixElement element) {
			Additions.Remove(element);
		}
		protected virtual string ProcessDeletedString(string str, CultureInfo culture) {
			return str;
		}
		protected virtual string Combine(AffixElement element, string word, CultureInfo culture) {
			if(!element.IsWordValid(word, culture))
				return string.Empty;
			string result = word;
			if(!String.IsNullOrEmpty(element.DeletedString))
				result = element.ProcessDeletedString(word, culture);
			if(!String.IsNullOrEmpty(result))
				return element.DoCombineCore(result, culture);
			return string.Empty;
		}
		public virtual void Combine(string word, CultureInfo culture, StringCollection list) {
			string result = Combine(this, word, culture);
			if(!String.IsNullOrEmpty(result))
				list.Add(result);
			for(int i = 0; i < Additions.Count; i++) {
				result = Combine(Additions[i], word, culture);
				if(!String.IsNullOrEmpty(result))
					list.Add(result);
			}
		}
		public bool IsFilled {
			get { return !String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(element); }
		}
		protected virtual bool IsWordValid(string word, CultureInfo culture) {
			int conditionDepth = Conditions.Count;
			if (conditionDepth > word.Length)
				return false;
			for (int i = 0; i < conditionDepth; i++) {
				IAffixCondition condition = Conditions[i];
				if (!condition.IsMatch(GetWordCharToConditionCheck(word, i, culture)))
					return false;
			}
			return true;
		}
		protected abstract char GetWordCharToConditionCheck(string word, int conditionIndex, CultureInfo culture);
		protected abstract string DoCombineCore(string word, CultureInfo culture);
	}
	public class AffixPrefix : AffixElement {
		public AffixPrefix(string key)
			: base(key) {
		}
		public override AffixElement AddAffixElement() {
			AffixPrefix prefix = new AffixPrefix(Key);
			base.AddAffixElement(prefix);
			return prefix;
		}
		bool StartsWith(string origin, string value, bool ignoreCase, CultureInfo culture) {
			if(value == null)
				throw new ArgumentNullException("value");
			if(origin == value)
				return true;
			CultureInfo info = (culture == null) ? CultureInfo.CurrentCulture : culture;
			return info.CompareInfo.IsPrefix(origin, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}
		protected override string ProcessDeletedString(string str, CultureInfo culture) {
			if(StartsWith(str, DeletedString, true, culture))
				return str.Remove(0, DeletedString.Length);
			return str;
		}
		protected override char GetWordCharToConditionCheck(string word, int conditionIndex, CultureInfo culture) {
			return Char.ToUpper(GetWordCharToConditionCheckCore(word, conditionIndex), culture);
		}
		protected char GetWordCharToConditionCheckCore(string word, int conditionIndex) {
			return word[Conditions.Count - 1 - conditionIndex];
		}
		protected override string DoCombineCore(string word, CultureInfo culture) {
			return (char.IsUpper(word[0]) ? Element.ToUpper(culture) : Element.ToLower(culture)) + word;
		}
	}
	public class AffixSuffix : AffixElement {
		public AffixSuffix(string key)
			: base(key) {
		}
		public virtual void AddSuffix(AffixSuffix suffix) {
			base.AddAffixElement(suffix);
		}
		public virtual AffixSuffix AddSuffix() {
			AffixSuffix suffix = new AffixSuffix(Key);
			base.AddAffixElement(suffix);
			return suffix;
		}
		public override AffixElement AddAffixElement() {
			return AddSuffix();
		}
		public virtual void RemoveSuffix(AffixSuffix suffix) {
			base.RemoveAffixElement(suffix);
		}
		bool EndsWith(string origin, string value, bool ignoreCase, CultureInfo culture) {
			if(value == null)
				throw new ArgumentNullException("value");
			if(origin == value)
				return true;
			CultureInfo info = (culture == null) ? CultureInfo.CurrentCulture : culture;
			return info.CompareInfo.IsSuffix(origin, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}
		protected override string ProcessDeletedString(string str, CultureInfo culture) {
			if(EndsWith(str, DeletedString, true, culture))
				return str.Remove(str.Length - DeletedString.Length);
			return str;
		}
		protected override char GetWordCharToConditionCheck(string word, int conditionIndex, CultureInfo culture) {
			return Char.ToUpper(GetWordCharToConditionCheckCore(word, conditionIndex), culture);
		}
		protected char GetWordCharToConditionCheckCore(string word, int conditionIndex) {
			return word[word.Length - 1 - conditionIndex];
		}
		public override void Combine(string word, CultureInfo culture, StringCollection list) {
			string result = Combine(this, word, culture);
			if(!String.IsNullOrEmpty(result))
				list.Add(result);
			for(int i = 0; i < Additions.Count; i++) {
				result = Combine(Additions[i], word, culture);
				if(!String.IsNullOrEmpty(result))
					list.Add(result);
			}
		}
		protected override string DoCombineCore(string word, CultureInfo culture) {
			if(word.Length > 0)
				return word + (char.IsUpper(word[word.Length - 1]) ? Element.ToUpper(culture) : Element.ToLower(culture));
			return word;
		}
	}
	public class AffixOOSuffix : AffixSuffix {
		public AffixOOSuffix(string key)
			: base(key) {
		}
		public override AffixSuffix AddSuffix() {
			AffixOOSuffix suffix = new AffixOOSuffix(Key);
			base.AddAffixElement(suffix);
			return suffix;
		}
		protected override char GetWordCharToConditionCheck(string word, int conditionIndex, CultureInfo culture) {
			return GetWordCharToConditionCheckCore(word, conditionIndex);
		}
	}
	public class AffixOOPrefix : AffixPrefix {
		public AffixOOPrefix(string key)
			: base(key) {
		}
		public override AffixElement AddAffixElement() {
			AffixElement prefix = new AffixOOPrefix(Key);
			base.AddAffixElement(prefix);
			return prefix;
		}
		protected override char GetWordCharToConditionCheck(string word, int conditionIndex, CultureInfo culture) {
			return GetWordCharToConditionCheckCore(word, conditionIndex);
		}
	}
	#region ISpellDecompressor
	#region AffixISpellDecompressor
	public class AffixISpellDecompressor {
		readonly Dictionary<string, AffixElement> suffixes = new Dictionary<string, AffixElement>();
		readonly Dictionary<string, AffixElement> prefixes = new Dictionary<string, AffixElement>();
		List<string> activePrefixes = new List<string>();
		List<string> activeSuffixes = new List<string>();
		AffixFileReader fileReader;
		readonly HashSet<char> alphabetChars = new HashSet<char>();
		protected virtual AffixFileReader CreateAffixFileReader() {
			return new AffixFileReader(this);
		}
		public AffixFileReader FileReader {
			get {
				if (fileReader == null)
					fileReader = CreateAffixFileReader();
				return fileReader;
			}
		}
		public const char AffixSeparator = '/';
		public void AddSuffix(AffixSuffix suffix) {
			if(!Suffixes.ContainsKey(suffix.Key))
				Suffixes.Add(suffix.Key, suffix);
		}
		public void AddPrefix(AffixPrefix prefix) {
			if(!Prefixes.ContainsKey(prefix.Key))
				Prefixes.Add(prefix.Key, prefix);
		}
		public void Add(AffixElement element) {
			AffixSuffix suffix = element as AffixSuffix;
			if(suffix != null)
				AddSuffix(suffix);
			else
				AddPrefix((AffixPrefix)element);
		}
		public void AddAlphabetChars(IEnumerable<char> chars) {
			this.alphabetChars.UnionWith(chars);
		}
		protected virtual int IndexOfSeparator(string word) {
			return word.IndexOf(AffixSeparator);
		}
		protected List<string> ActivePrefixes {
			get { return activePrefixes; }
			set { activePrefixes = value; }
		}
		protected List<string> ActiveSuffixes {
			get { return activeSuffixes; }
			set { activeSuffixes = value; }
		}
		protected virtual void Reset() {
			ActiveSuffixes.Clear();
			ActivePrefixes.Clear();
		}
		public void ReadAffixFile(string grammarPath, Encoding dictionaryEncoding) {
			FileReader.Read(grammarPath, dictionaryEncoding);
		}
		public void ReadAffixStream(SpellCheckerISpellDictionary dictionary, Stream grammarStream, Encoding dictionaryEncoding) {
			FileReader.Read(dictionary, grammarStream, dictionaryEncoding);
		}
		protected virtual bool IsGrammarFilled() {
			return Suffixes.Count > 0 || Prefixes.Count > 0;
		}
		protected virtual void DecompressCore(string word, SpellCheckerDictionaryBase dictionary) {
			if(Suffixes.Count > 0 || Prefixes.Count > 0) {
				int index = IndexOfSeparator(word);
				if(index == -1) {
					dictionary.AddWordCore(word);
				} else
					DecompressWord(word, index, dictionary);
			}
		}
		protected internal virtual void DecompressWord(string word, int separatorIndex, SpellCheckerDictionaryBase dictionary) {
			Reset();
			SeparateCommands(word, separatorIndex + 1, word.Length);
			word = word.Substring(0, separatorIndex);
			StringCollection wordsList = new StringCollection();
			wordsList.Add(word);
			foreach (string p in ActivePrefixes) {
				AffixElement prefix = Prefixes[p];
				int startIndex = wordsList.Count;
				prefix.Combine(word, dictionary.Culture, wordsList);
				if (prefix.CanCombine) {
					int count = wordsList.Count;
					for (int i = startIndex; i < count; i++) {
						string newWord = wordsList[i];
						foreach (string s in ActiveSuffixes) {
							AffixElement suffix = Suffixes[s];
							if (suffix.CanCombine)
								suffix.Combine(newWord, dictionary.Culture, wordsList);
						}
					}
				}
			}
			foreach(string s in ActiveSuffixes) {
				AffixElement suffix = Suffixes[s];
				suffix.Combine(word, dictionary.Culture, wordsList);
			}
			MoveWordsToDictionary(wordsList, dictionary);
		}
		void MoveWordsToDictionary(StringCollection wordsList, SpellCheckerDictionaryBase dictionary) {
			for(int i = 0; i < wordsList.Count; i++)
				dictionary.AddWordCore(wordsList[i]);
		}
		public virtual bool DecompressDictionary(SpellCheckerISpellDictionary dictionary) {
			Stream dictionaryStream = dictionary.GetStreamFromPath(dictionary.DictionaryPath);
			Stream grammarStream = dictionary.GetStreamFromPath(dictionary.GrammarPath);
			if (dictionaryStream == null || grammarStream == null)
				return false;
			try {
				return DecompressDictionary(dictionary, dictionaryStream, grammarStream);
			}
			finally {
				dictionaryStream.Close();
				grammarStream.Close();
			}
		}
		public virtual bool DecompressDictionary(SpellCheckerISpellDictionary dictionary, Stream dictionaryStream, Stream grammarStream) {
			ReadAffixStream(dictionary, grammarStream, dictionary.Encoding);
			if (Suffixes.Count == 0 && Prefixes.Count == 0)
				return false;
			if (this.alphabetChars.Count > 0) {
				List<char> chars = new List<char>();
				foreach (char ch in this.alphabetChars) {
					if (dictionary.CaseSensitive || Char.IsLower(ch))
						chars.Add(ch);
				}
				dictionary.SetAlphabetChars(chars.ToArray());
			}
			using (StreamReader strReader = new StreamReader(dictionaryStream, dictionary.Encoding)) {
				try {
					int wordsCount;
					string line = strReader.ReadLine();
					if (int.TryParse(line, out wordsCount))
						line = strReader.ReadLine();
					while (line != null) {
						DecompressCore(line, dictionary);
						line = strReader.ReadLine();
					}
				}
				catch (OutOfMemoryException e) {
					throw new NotLoadedDictionaryException(e.Message, e);
				}
				catch {
					dictionary.Clear();
					return false;
				}
			}
			return true;
		}
		protected virtual bool AddActiveElement(string element) {
			if(Prefixes.ContainsKey(element)) {
				ActivePrefixes.Add(element);
				return true;
			}
			else
				if(Suffixes.ContainsKey(element)) {
					ActiveSuffixes.Add(element);
					return true;
				}
			return false;			
		}
		private bool AddActiveElementByCharKey(char element) {
			for(int i = 0; i < 2; i++) {
				if(i == 1)
					element = char.IsUpper(element) ? char.ToLower(element) : char.ToUpper(element);
				string strElement = element.ToString();
				if(Prefixes.ContainsKey(strElement)) {
					ActivePrefixes.Add(strElement);
					return true;
				}
				else
					if(Suffixes.ContainsKey(strElement)) {
						ActiveSuffixes.Add(strElement);
						return true;
					}
			}
			return false;			
		}
		protected virtual int GetKeyLength() {
			if(Suffixes.Count > 0)
				return GetKeyLength(Suffixes.Keys);
			if(Prefixes.Count > 0)
				return GetKeyLength(Prefixes.Keys);
			return 1;
		}
		private int GetKeyLength(IEnumerable keys) {
			IEnumerator enumerator = keys.GetEnumerator();
			enumerator.Reset();
			enumerator.MoveNext();
			string key = enumerator.Current.ToString();
			return key.Length;
		}
		protected virtual void SeparateCommands(string str, int startIndex, int finishIndex) {
			int singleCommandLength = GetKeyLength();
			if(singleCommandLength == 1)
				for(int i = startIndex; i < finishIndex; i++) {
					if(!char.IsLetter(str[i]))
						continue;
					AddActiveElementByCharKey(str[i]);
				}
			else {
				int i = startIndex;
				while(i < finishIndex) {
					try {
						AddActiveElement(str.Substring(i, singleCommandLength));
					}
					catch { }
					i += singleCommandLength;
				}
			}
		}
		protected internal Dictionary<string, AffixElement> Suffixes { get { return suffixes; } }
		protected internal Dictionary<string, AffixElement> Prefixes { get { return prefixes; } }
		protected internal HashSet<char> AlphabetChars { get { return alphabetChars; } }
	}
	#endregion
	#region AffixFileReader
	public class AffixFileReader {
		protected const string Suffixes = "suffixes";
		protected const string Prefixes = "prefixes";
		protected const string Wordchars = "wordchars";
		protected const string Flag = "flag";
		protected const char Comments = '#';
		protected const char Period = '.';
		protected const char Asterisk = '*';
		protected const char BracketLeft = '[';
		protected const char BracketRight = ']';
		protected const char Arrow = '>';
		protected const char Not = '^';
		protected const char Minus = '-';
		protected const char Coma = ',';
		protected const char Colon = ':';
		AffixISpellDecompressor decompressor = null;
		AffixElement currentElement = null;
		AffixElementKind currentElementKind = AffixElementKind.None;
		public AffixFileReader(AffixISpellDecompressor decompressor)
			: base() {
			this.decompressor = decompressor;
		}
		public virtual AffixISpellDecompressor Decompressor { get { return decompressor; } }
		protected AffixElement CurrentElement { get { return currentElement; } set { currentElement = value; } }
		protected AffixElementKind CurrentElementKind { get { return currentElementKind; } set { currentElementKind = value; } }
		public virtual void Read(string fileName, Encoding dictionaryEncoding) {
			if(!File.Exists(fileName))
				return;
			ReadCore(File.OpenRead(fileName), dictionaryEncoding);
		}
		public virtual void Read(SpellCheckerISpellDictionary dictionary, Stream grammarStream, Encoding dictionaryEncoding) {
			ReadCore(grammarStream, dictionaryEncoding);
		}
		void ReadCore(Stream grammarStream, Encoding dictionaryEncoding) {
			using(StreamReader strReader = new StreamReader(grammarStream, dictionaryEncoding)) {
				string line = string.Empty;
				while((line = strReader.ReadLine()) != null)
					ProcessLine(line);
			}
		}
		protected int CompareStrings(string str1, string str2, bool ignoreCase, CultureInfo culture) {
#if SL
			return string.Compare(str1, str2, CultureInfo.InvariantCulture, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
#else
			return string.Compare(str1, str2, ignoreCase, culture);
#endif
		}
		protected virtual bool SetElementKind(string line) {
			if(CompareStrings(line, Suffixes, false, CultureInfo.InvariantCulture) == 0) {
				currentElementKind = AffixElementKind.Suffix;
				return true;
			}
			if(CompareStrings(line, Prefixes, false, CultureInfo.InvariantCulture) == 0) {
				currentElementKind = AffixElementKind.Prefix;
				return true;
			}
			return false;
		}
		protected virtual string ExtractKey(string line) {
			int flagIndex = line.IndexOf(Flag);
			int asteriskIndex = line.IndexOf(Asterisk, flagIndex + Flag.Length); 
			int startIndex = asteriskIndex > 0 ? asteriskIndex + 1 : flagIndex + Flag.Length + 1;
			int colonIndex = line.IndexOf(Colon); 
			return line.Substring(startIndex, colonIndex - startIndex);
		}
		protected virtual AffixElement CreateNewAffixElementCore(string key) {
			switch(CurrentElementKind) {
				case AffixElementKind.Prefix: {
						return new AffixPrefix(key);
					}
				case AffixElementKind.Suffix: {
						return new AffixSuffix(key);
					}
				case AffixElementKind.None: {
						throw new NotSupportedException("None is not supported for Affix");
					}
				default:
					throw new NotSupportedException("Unknown Affix element");
			}
		}
		protected virtual void CreateNewAffixElement(string line) {
			if(CurrentElementKind == AffixElementKind.None)
				throw new NotSupportedException("None is not supported for Affix");
			else {
				string key = ExtractKey(line);
				if(!String.IsNullOrEmpty(key)) {
					currentElement = CreateNewAffixElementCore(key);
					CurrentElement.CanCombine = line.IndexOf(Asterisk) != -1;
					Decompressor.Add(CurrentElement);
				}
			}
		}
		protected virtual void FillAffixElement(AffixElement element, string line) {
			int arrowIndex = line.IndexOf(Arrow);
			if(arrowIndex != -1) {
				string s = line.Substring(0, arrowIndex);
				FillGoodWrongChars(element, s);
			}
			int indexOfComment = line.IndexOf(Comments);
			line = line.Substring(arrowIndex + 1, indexOfComment != -1 ? indexOfComment - arrowIndex - 1 : line.Length - arrowIndex - 1);
			line = line.Trim();
			FillElement(element, line);
		}
		protected virtual void FillGoodWrongCharsFromComplexLine(AffixElement element, string line) {
			int charsSetLength = 0;
			for (int index = line.Length - 1; index >= 0; index--) {
				char ch = line[index];
				switch (ch) {
					case '.':
						continue;
					case ' ':
					case ']':
						if (charsSetLength > 0) {
							string charsSet = line.Substring(index + 1, charsSetLength);
							element.Conditions.Add(new AffixCondition(charsSet, true));
							charsSetLength = 0;
						}
						break;
					case '[':
						if (charsSetLength > 0) {
							string charsSet = line.Substring(index + 1, charsSetLength);
							if (charsSet.StartsWith("^"))
								element.Conditions.Add(new AffixCondition(charsSet.TrimStart('^'), false));
							else
								element.Conditions.Add(new AffixCondition(charsSet, true));
							charsSetLength = 0;
						}
						break;
					default:
						charsSetLength++;
						if (index == 0)
							element.Conditions.Add(new AffixCondition(line.Substring(0, charsSetLength), true));
						break;
				}
			}
		}
		protected virtual void FillGoodWrongChars(AffixElement element, string line) {
			line = line.Trim();
			if (!String.IsNullOrEmpty(line) && line[0] != Period)
				FillGoodWrongCharsFromComplexLine(element, line);
		}
		protected virtual void FillElement(AffixElement element, string line) {
			char firstChar = line[0];
			if(firstChar == Minus && CurrentElement is AffixSuffix) {
				int indexOfComa = line.IndexOf(Coma);
				if(indexOfComa != -1)
					element.DeletedString = line.Substring(1, indexOfComa - 1);
				element.Element = line.Substring(indexOfComa + 1, line.Length - indexOfComa - 1);
			} else
				element.Element = line.Substring(0, line.Length);
		}
		protected internal virtual void ProcessLine(string line) {
			if (String.IsNullOrEmpty(line))
				return;
			line = line.Trim();
			if (String.IsNullOrEmpty(line))
				return;
			if (line.StartsWith(Comments.ToString()))
				return;
			if (line.StartsWith(Wordchars)) {
				SetAlphametChars(line.Remove(0, Wordchars.Length).Trim());
				return;
			}
			if (SetElementKind(line))
				return;
			if (line.StartsWith(Flag)) {
				CreateNewAffixElement(line);
				return;
			}
			if (CurrentElement != null && !CurrentElement.IsFilled) {
				FillAffixElement(CurrentElement, line);
				return;
			}
			if (CurrentElement != null && CurrentElement.IsFilled && CurrentElement is AffixSuffix)
				FillAffixElement(((AffixSuffix)CurrentElement).AddSuffix(), line);
		}
		void SetAlphametChars(string line) {
			string[] ranges = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			if (ranges == null || ranges.Length == 0)
				return;
			HashSet<char> chars = new HashSet<char>();
			int count = Math.Min(2, ranges.Length);
			for (int i = 0; i < count; i++) {
				string range = ranges[i];
				if (range.StartsWith("#"))
					break;
				if (range.Length > 1) {
					if (range[0] == '[' && range[range.Length - 1] == ']') {
						string rangeString = range.Trim('[', ']');
						if (rangeString.Length == 0)
							continue;
						bool shouldExcludeChars = rangeString[0] == '^';
						rangeString = rangeString.TrimStart('^');
						List<char> rangeChars = new List<char>();
						int startIndex = 0;
						while (startIndex < rangeString.Length) {
							int sequenceSeparatorIndex = rangeString.IndexOf('-', startIndex);
							if (sequenceSeparatorIndex < 0) {
								rangeChars.AddRange(rangeString.Substring(startIndex));
								break;
							}
							if (sequenceSeparatorIndex > 0 && sequenceSeparatorIndex < rangeString.Length - 1) {
								if (startIndex < sequenceSeparatorIndex - 1)
									rangeChars.AddRange(rangeString.Substring(startIndex, ((sequenceSeparatorIndex - 1) - startIndex)));
								char firstChar = rangeString[sequenceSeparatorIndex - 1];
								char lastChar = rangeString[sequenceSeparatorIndex + 1];
								for (char ch = firstChar; ch <= lastChar; ch++)
									rangeChars.Add(ch);
								startIndex = sequenceSeparatorIndex + 2;
							}
							else {
								if (sequenceSeparatorIndex > startIndex)
									rangeChars.AddRange(rangeString.Substring(startIndex, sequenceSeparatorIndex - startIndex));
								startIndex = sequenceSeparatorIndex + 1;
							}
						}
						if (shouldExcludeChars) {
							HashSet<char> allChars = new HashSet<char>();
							for (char ch = 'a'; ch <= 'z'; ch++)
								allChars.Add(ch);
							for (char ch = 'A'; ch <= 'Z'; ch++)
								allChars.Add(ch);
							allChars.ExceptWith(rangeChars);
							chars.UnionWith(allChars);
						}
						else
							chars.UnionWith(rangeChars);
					}
				}
				else
					chars.Add(range[0]);
			}
			Decompressor.AddAlphabetChars(chars);
		}
	}
	#endregion
	#endregion
	#region OpenOfficeDecompressor
	#region AffixOpenOfficeDecompressor
	public class AffixOpenOfficeDecompressor : AffixISpellDecompressor {
		public new AffixOpenOfficeFileReader FileReader { get { return (AffixOpenOfficeFileReader)base.FileReader; } }
		protected override AffixFileReader CreateAffixFileReader() {
			return new AffixOpenOfficeFileReader(this);
		}
		protected override void DecompressCore(string line, SpellCheckerDictionaryBase dictionary) {
			if (String.IsNullOrEmpty(line) || IsCommentLine(line))
				return;
			base.DecompressCore(GetWordDefinition(line), dictionary);
		}
		bool IsCommentLine(string line) {
			return line[0] == '#' || line[0] == '\t';
		}
		string GetWordDefinition(string line) {
			return FileReader.GetTokens(line)[0];
		}
		protected override void SeparateCommands(string str, int startIndex, int finishIndex) {
			string flagsSrt = str.Substring(startIndex, finishIndex - startIndex);
			string[] flags = GetFlags(flagsSrt);
			for (int i = 0; i < flags.Length; i++)
				AddActiveElement(flags[i]);
		}
		string[] GetFlags(string flags) {
			if (FileReader.IsFlagAliasesDefined) {
				int index;
				if (Int32.TryParse(flags, out index))
					return FileReader.GetFlagSet(index);
			}
			return FileReader.SplitFlags(flags);
		}
		protected override int IndexOfSeparator(string word) {
			int charsCount = word.Length;
			for (int index = 0; index < charsCount; index++) {
				char character = word[index];
				if (character == '\\')
					index++;
				else if (character == '\'' || character == '"') {
					char subsetStartChar = character;
					for (int i = index + 1; i < charsCount; i++) {
						if (word[i] == subsetStartChar) {
							index = i;
							break;
						}
					}
				}
				else if (character == AffixSeparator)
					return index;
			}
			return -1;
		}
	}
	#endregion
	#region AffixOpenOfficeFileReader
	public class AffixOpenOfficeFileReader : AffixFileReader {
		protected const string FlagSetAlias = "AF";
		protected const string Suffix = "SFX";
		protected const string Prefix = "PFX";
		protected const string CanCombineSymbol = "Y";
		protected const string CanNotCombineSymbol = "N";
		protected const string Zero = "0";
		protected const string Try = "TRY";
		protected const string Set = "SET";
		static readonly char[] Spaces = {' ', '\t'};
		int count = 0;
		int currentIndex = 0;
		FlagTypes flagType = FlagTypes.OneChar;
		List<string[]> flags;
		int currentLineIndex;
		public AffixOpenOfficeFileReader(AffixOpenOfficeDecompressor decompressor)
			: base(decompressor) {
		}
		public FlagTypes FlagType { get { return flagType; } }
		public bool IsFlagAliasesDefined { get { return flags != null && flags.Count > 0; } }
		protected int CurrentLineIndex { get { return currentLineIndex; } }
		protected override AffixElement CreateNewAffixElementCore(string key) {
			switch (CurrentElementKind) {
				case AffixElementKind.Prefix:
					return new AffixOOPrefix(key);
				case AffixElementKind.Suffix:
					return new AffixOOSuffix(key);
				case AffixElementKind.None:
					throw new NotSupportedException("None is not supported for Affix");
				default:
					throw new NotSupportedException("Unknown Affix element");
			}
		}
		public string[] GetFlagSet(int index) {
			if (flags == null)
				return null;
			index--;
			return (index >= 0 && index < flags.Count) ? flags[index] : null;
		}
		public override void Read(SpellCheckerISpellDictionary dictionary, Stream grammarStream, Encoding dictionaryEncoding) {
			using (StreamReader strReader = new StreamReader(grammarStream, dictionaryEncoding)) {
				while (!strReader.EndOfStream) {
					this.currentLineIndex++;
					string line = strReader.ReadLine().Trim();
					ProcessLine((SpellCheckerOpenOfficeDictionary)dictionary, line);
				}
			}
		}
		protected internal virtual void ProcessLine(SpellCheckerOpenOfficeDictionary dictionary, string line) {
			if(line.StartsWith(Try))
				dictionary.FillAlphabetCore(line.ToCharArray());
			else
				ProcessLine(line);
		}
		protected virtual void SetEncodingFromLine(SpellCheckerISpellDictionary dictionary, string line) {
			line = line.Trim();
			if(line.StartsWith(Set)) {
				string encodingName = line.Substring(4);
				try {
					dictionary.Encoding = Encoding.GetEncoding(encodingName);
				} catch { }
			}
#if !SL
			dictionary.Encoding = System.Text.Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
#else
			dictionary.Encoding = System.Text.Encoding.UTF8;
#endif
		}
		protected internal override void ProcessLine(string line) {
			if (String.IsNullOrEmpty(line) || ContainsKey(Comments.ToString(), line))
				return;
			if (ContainsKey(Flag, line)) {
				ReadFlagType(line);
				return;
			}
			if (ContainsKey(FlagSetAlias, line)) {
				ReadFlagsSet(line);
				return;
			}
			if (CurrentElement != null && this.currentIndex < this.count) {
				AffixElement element = this.currentIndex == 0 ? CurrentElement : CurrentElement.AddAffixElement();
				FillAffixElement(element, line);
				this.currentIndex++;
				return;
			}
			if (SetElementKind(line)) {
				CreateNewAffixElement(line);
				return;
			}
		}
		bool ContainsKey(string key, string line) {
			return line.StartsWith(key, StringComparison.InvariantCultureIgnoreCase);
		}
		void ReadFlagsSet(string line) {
			string[] tokens = GetTokens(line);
			if (tokens.Length < 2)
				return;
			string value = tokens[1];
			if (flags != null)
				this.flags.Add(SplitFlags(value));
			else {
				int capacity;
				if (int.TryParse(value, out capacity))
					this.flags = new List<string[]>(capacity);
				else {
					this.flags = new List<string[]>();
					this.flags.Add(SplitFlags(value));
				}
			}
		}
		protected internal string[] SplitFlags(string value) {
			List<string> result = new List<string>();
			if (FlagType == FlagTypes.Number) {
				string[] flags = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				int count = flags.Length;
				for (int i = 0; i < count; i++) {
					result.Add(flags[i].Trim());
				}
			}
			else {
				int flagLength = FlagType == FlagTypes.TwoChar ? 2 : 1;
				AddFlags(result, value, flagLength);
			}
			return result.ToArray();
		}
		void AddFlags(List<string> flags, string value, int flagLength) {
			int count = value.Length;
			for (int i = count % flagLength; i < count; i += flagLength) {
				flags.Add(value.Substring(i, flagLength));
			}
		}
		void ReadFlagType(string line) {
			string[] tokens = GetTokens(line);
			if (tokens.Length < 2)
				return;
			switch (tokens[1]) {
				case "long":
					flagType = FlagTypes.TwoChar;
					break;
				case "num":
					flagType = FlagTypes.Number;
					break;
				case "UTF-8":
					flagType = FlagTypes.UTF8;
					break;
			}
		}
		protected internal string[] GetTokens(string line) {
			return line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
		}
		protected override void CreateNewAffixElement(string line) {
			this.currentIndex = 0;
			this.count = 0;
			string[] tokens = GetTokens(line);
			string key = tokens[1];
			bool canCombine = tokens[2] == CanCombineSymbol;
			if (!Int32.TryParse(tokens[3], out count)) {
				int length = tokens[3].Length;
				do {
					length--;
					if (Int32.TryParse(tokens[3].Substring(0, length), out count))
						break;
				} while (length > 0);
				if (length == 0)
					throw new Exception("Affixes count definition is incorrect. Line: " + this.currentLineIndex);
			}
			if (CurrentElementKind == AffixElementKind.None)
				throw new NotSupportedException("None is not supported for Affix");
			CurrentElement = CreateNewAffixElementCore(key);
			CurrentElement.CanCombine = canCombine;
			Decompressor.Add(CurrentElement);
		}
		protected override string ExtractKey(string line) {
			string[] lines = line.Split(Spaces, StringSplitOptions.RemoveEmptyEntries);
			if(lines.Length > 1)
				return lines[1];
			return string.Empty;
		}
		protected override void FillAffixElement(AffixElement element, string line) {
			string[] tokens = GetTokens(line);
			int count = tokens.Length;
			for (int tokenIndex = 0; tokenIndex < count; tokenIndex++) {
				string token = tokens[tokenIndex];
				switch (tokenIndex) {
					case 0:
						break;
					case 1:
						if (element.Key != token)
							throw new Exception("Incorrect affix key. Line: " + this.currentLineIndex);
						break;
					case 2:
						ParseDeletedString(element, token);
						break;
					case 3:
						ParseAffix(element, token);
						break;
					case 4:
						FillGoodWrongChars(element, token);
						break;
				}
			}
		}
		void ParseDeletedString(AffixElement element, string token) {
			element.DeletedString = token != "0" ? token : String.Empty;
		}
		void ParseAffix(AffixElement element, string token) {
			string[] tokens = token.Split('/');
			element.Element = tokens[0] != "0" ? tokens[0] : String.Empty;
			if (tokens.Length < 2)
				return;
			SetFlags(element, tokens[1]);
		}
		void SetFlags(AffixElement element, string value) {
			if (this.flags != null && this.flags.Count > 0) {
				int index;
				if (Int32.TryParse(value, out index) && index > 0 && index <= this.flags.Count) {
					element.Flags = this.flags[index - 1];
					return;
				}
			}
			string[] flags = SplitFlags(value);
			element.Flags = flags;
		}
		protected virtual void FillDeletedChars(AffixElement element, string line) {
			int index = line.IndexOfAny(Spaces);
			element.DeletedString = line.Substring(0, index);
		}
		protected override bool SetElementKind(string line) {
			if(line.StartsWith(Suffix)) {
				CurrentElementKind = AffixElementKind.Suffix;
				return true;
			}
			if(line.StartsWith(Prefix)) {
				CurrentElementKind = AffixElementKind.Prefix;
				return true;
			}
			return false;
		}
		public static Encoding GetEncodingFromStream(Stream grammarStream) {
			if (grammarStream == null)
				return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
			byte[] data = new byte[grammarStream.Length];
			long position = grammarStream.Position;
			grammarStream.Read(data, 0, Convert.ToInt32(grammarStream.Length));
			grammarStream.Position = position;
			using (MemoryStream stream = new MemoryStream(data)) {
				return GetEncodingFromStreamCore(stream);
			}
		}
		protected static Encoding GetEncodingFromStreamCore(Stream stream) {
			using (StreamReader reader = new StreamReader(stream)) {
				while (!reader.EndOfStream) {
					string line = reader.ReadLine().Trim();
					if (line.StartsWith(Set))
						return GetEncodingFromLine(line);
				}
				return null;
			}
		}
		public static Encoding GetEncodingFromLine(string line) {
			line = line.Trim();
			if(line.StartsWith(Set)) {
				string encodingName = line.Substring(4);
				try {
					Regex encodingRegex = new Regex("^(SET ISO)([0-9]*)-([0-9]*)$");
					Match match = encodingRegex.Match(line);
					if(match.Success)
						try {
							string encoding = string.Format("ISO-{0}-{1}", match.Groups[2], match.Groups[3]);
							return Encoding.GetEncoding(encoding);
						} catch(System.ArgumentException) {
						}
					return Encoding.GetEncoding(encodingName);
				} catch { }
			}
#if !SL
			return System.Text.Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
#else
			return System.Text.Encoding.UTF8;
#endif
		}
		public static char[] GetAlphabetChars(Stream grammarStream, Encoding encoding) {
			using (StreamReader reader = new StreamReader(grammarStream, encoding)) {
				while (!reader.EndOfStream) {
					string line = reader.ReadLine();
					if (line.StartsWith(Try)) {
						return line.Substring(4).Trim().ToCharArray(); 
					}
				}
				return null;
			}
		}
		protected override void FillGoodWrongCharsFromComplexLine(AffixElement element, string line) {
			for (int index = line.Length - 1; index >= 0; index--) {
				char ch = line[index];
				if (ch == ']') {
					int startIndex = line.LastIndexOf('[', index);
					if (startIndex < 0)
						break;
					int endIndex = index;
					index = startIndex;
					startIndex++;
					if (startIndex == endIndex)
						continue;
					string charsSet = line.Substring(startIndex, endIndex - startIndex);
					if (charsSet.StartsWith("^"))
						element.Conditions.Add(new AffixCondition(charsSet.TrimStart('^'), false));
					else
						element.Conditions.Add(new AffixCondition(charsSet, true));
				}
				else if (ch == '.')
					element.Conditions.Add(new EmptyAffixCondition());
				else
					element.Conditions.Add(new AffixCondition(ch, true));
			}
		}
	}
	#endregion
	#endregion
}
