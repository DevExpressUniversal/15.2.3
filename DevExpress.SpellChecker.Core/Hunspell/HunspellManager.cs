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
using DevExpress.XtraSpellChecker.Native;
using System.Collections;
using DevExpress.XtraSpellChecker.Algorithms;
namespace DevExpress.XtraSpellChecker.Hunspell {
	#region CompoundingOptions
	public enum WordEntryType {
		WholeWord,
		Beginning,
		Ending,
		Middle
	}
	#endregion
	#region WordInfo
	public class WordInfo {
		readonly string originalWord;
		readonly WordEntry root;
		readonly Suffix suffix;
		readonly Prefix preffix;
		public WordInfo(string originalWord, WordEntry root, Prefix preffix, Suffix suffix) {
			this.originalWord = originalWord;
			this.root = root;
			this.suffix = suffix;
			this.preffix = preffix;
		}
		public WordInfo(string word, WordEntry root, Prefix preffix)
			: this(word, root, preffix, null) {
		}
		public WordInfo(string word, WordEntry root, Suffix suffix)
			: this(word, root, null, suffix) {
		}
		public WordInfo(string word, WordEntry root)
			: this(word, root, null, null) {
		}
		public string OriginalWord { get { return originalWord; } }
		public WordEntry Root { get { return root; } }
		public Suffix Suffix { get { return suffix; } }
		public Prefix Preffix { get { return preffix; } }
	}
	#endregion
	#region HunspellManager
	public class HunspellManager {
		readonly HunspellDictionary dictionary;
		public HunspellManager(HunspellDictionary dictionary) {
			this.dictionary = dictionary;
		}
		public HunspellDictionary Dictionary { get { return dictionary; } }
		public virtual bool CheckWord(string word) {
			word = PrepareWordToCheck(word);
			WordInfo wordInfo = CheckWordCore(word);
			if (wordInfo != null)
				return true;
			if (IsAllCaps(word)) {
				if (CheckWordWithChangedCase(word.ToLower(Dictionary.Culture)))
					return true;
				return CheckWordWithChangedCase(MakeInitialCharCaps(word));
			}
			if (IsInitCaps(word))
				return CheckWordWithChangedCase(word.ToLower(Dictionary.Culture));
			return false;
		}
		bool CheckWordWithChangedCase(string word) {
			WordInfo wordInfo = CheckWordCore(word);
			return wordInfo != null && !Dictionary.CheckFlag(wordInfo.Root, HunspellFlags.KeepCase);
		}
		protected internal virtual string PrepareWordToCheck(string word) {
			string result = word.Trim();
			if (!Dictionary.ConversionTable.IsEmpty)
				return Dictionary.ConversionTable.Convert(result);
			return result;
		}
		protected internal bool IsAllCaps(string word) {
			return StringUtils.IsAllCaps(word, Dictionary.Culture);
		}
		protected internal bool IsInitCaps(string word) {
			return StringUtils.IsInitCaps(word, Dictionary.Culture);
		}
		protected internal string MakeInitialCharCaps(string word) {
			return StringUtils.MakeInitialCharCaps(word, Dictionary.Culture);
		}
		protected internal virtual WordInfo CheckWordCore(string word) {
			WordInfo wordInfo = FindWordInfo(word, WordEntryType.WholeWord, false);
			if (wordInfo != null)
				return !IsWordForbidden(wordInfo.Root) ? wordInfo : null;
			if (Dictionary.IsCompoundDefined() || Dictionary.CompoundRules.Count > 0) {
				List<WordInfo> wordParts = DecompoundWord(word);
				if (wordParts != null)
					return wordParts[0];
			}
			return null;
		}
		bool IsWordForbidden(WordEntry wordEntry) {
			return Dictionary.CheckFlag(wordEntry, HunspellFlags.ForbiddenWord);
		}
		protected internal virtual List<WordInfo> DecompoundWord(string word) {
			if (String.IsNullOrEmpty(word) || word.Length < (Dictionary.CompoundMin * 2))
				return null;
			if (Dictionary.IsCompoundDefined()) {
				List<WordInfo> wordParts = DecompoundWord(word, true);
				if (wordParts != null && wordParts.Count > 0)
					return wordParts;
			}
			if (Dictionary.CompoundRules.Count > 0) {
				List<WordInfo> wordParts = DecompoundWord(word, false);
				if (wordParts != null && AreWordPartsFitRule(wordParts))
					return wordParts;
			}
			return null;
		}
		List<WordInfo> DecompoundWord(string word, bool checkCompoundFlags) {
			List<WordInfo> result = DecompoundWordCore(word, false, checkCompoundFlags);
			if (result != null)
				return result;
			if (Dictionary.CompoundPatterns.Count > 0)
				return DecompoundWordUsingCompoundPatterns(word, checkCompoundFlags);
			return null;
		}
		List<WordInfo> DecompoundRemainWordPartCore(string remainPart, bool checkCompoundFlags) {
			if (String.IsNullOrEmpty(remainPart) || remainPart.Length < Dictionary.CompoundMin)
				return null;
			WordInfo wordInfo = FindWordInfo(remainPart, WordEntryType.Ending, checkCompoundFlags);
			if (wordInfo == null)
				return DecompoundWordCore(remainPart, true, checkCompoundFlags);
			if (IsWordForbidden(wordInfo.Root))
				return null;
			List<WordInfo> result = new List<WordInfo>();
			result.Add(wordInfo);
			return result;
		}
		List<WordInfo> DecompoundWordCore(string word, bool isContinue, bool checkCompoundFlags) {
			int minLength = GetMinSubWordLength();
			int maxLength = GetMaxSubWordLength(word, isContinue, minLength);
			for (int length = minLength; length <= maxLength; length++) {
				WordEntryType entryType = isContinue ? WordEntryType.Middle : WordEntryType.Beginning;
				WordInfo wordInfo = FindWordInfo(word.Substring(0, length), entryType, checkCompoundFlags);
				if (wordInfo != null && !IsWordForbidden(wordInfo.Root) && (checkCompoundFlags || IsWordInfoFitInWithRule(wordInfo))) {
					List<WordInfo> remainParts = DecompoundRemainWordPartCore(word.Substring(length), checkCompoundFlags);
					if (remainParts != null && remainParts.Count > 0) {
						if (!CanCombine(wordInfo, remainParts[0]))
							continue;
					}
					else {
						int lastIndex = length - 1;
						bool hasDuplicatedChars = lastIndex >= 1 && word[lastIndex] == word[lastIndex - 1];
						if (!hasDuplicatedChars || !Dictionary.SimplifiedTriple)
							continue;
						remainParts = DecompoundRemainWordPartCore(word.Substring(lastIndex), checkCompoundFlags);
						if (remainParts == null || remainParts.Count == 0)
							continue;
					}
					List<WordInfo> result = new List<WordInfo>();
					result.Add(wordInfo);
					result.AddRange(remainParts);
					if (result.Count <= Dictionary.CompoundWordMax)
						return result;
				}
			}
			return null;
		}
		List<WordInfo> DecompoundWordUsingCompoundPatterns(string word, bool checkCompoundFlags) {
			int minLength = GetMinSubWordLength();
			int maxLength = GetMaxSubWordLength(word, false, minLength);
			int count = Dictionary.CompoundPatterns.Count;
			for (int length = minLength; length <= maxLength; length++) {
				for (int i = 0; i < count; i++) {
					CompoundPattern pattern = Dictionary.CompoundPatterns[i];
					if (!pattern.CanApply(word, length))
						continue;
					string firstPart = word.Substring(0, length) + pattern.Ending.Value;
					string remainPart = pattern.Begining.Value + word.Substring(length + pattern.Replacement.Length);
					WordInfo wordInfo = FindWordInfo(firstPart, WordEntryType.Beginning, checkCompoundFlags);
					if (wordInfo == null)
						continue;
					List<WordInfo> remainParts = DecompoundRemainWordPartCore(remainPart, checkCompoundFlags);
					if (remainParts == null || remainParts.Count == 0 || !pattern.ValidateFlags(wordInfo, remainParts[0]))
						continue;
					List<WordInfo> result = new List<WordInfo>();
					result.Add(wordInfo);
					result.AddRange(remainParts);
					if (result.Count <= Dictionary.CompoundWordMax)
						return result;
				}
			}
			return null;
		}
		int GetMaxSubWordLength(string word, bool isContinue, int minLength) {
			return isContinue ? word.Length : word.Length - minLength;
		}
		int GetMinSubWordLength() {
			return Math.Max(1, Dictionary.CompoundMin);
		}
		protected virtual bool CanCombine(WordInfo wordInfo1, WordInfo wordInfo2) {
			List<ICompoundWordValidationRule> validationRules = Dictionary.CompoundValidationRules;
			int count = validationRules.Count;
			for (int i = 0; i < count; i++) {
				if (!validationRules[i].CanCombine(wordInfo1, wordInfo2))
					return false;
			}
			return true;
		}
		protected virtual bool AreWordPartsFitRule(List<WordInfo> wordParts) {
			List<CompoundRule> compoundRules = Dictionary.CompoundRules;
			int count = compoundRules.Count;
			for (int i = 0; i < count; i++) {
				if (compoundRules[i].Validate(wordParts) == CompoundRuleCheckResult.Success)
					return true;
			}
			return false;
		}
		protected virtual bool IsWordInfoFitInWithRule(WordInfo wordInfo) {
			List<CompoundRule> compoundRules = Dictionary.CompoundRules;
			int count = compoundRules.Count;
			for (int i = 0; i < count; i++) {
				if (compoundRules[i].IsWordInfoFitInWithRule(wordInfo))
					return true;
			}
			return false;
		}
		protected virtual WordInfo FindWordInfo(string word, WordEntryType entryType, bool checkCompoundFlags) {
			WordInfo wordInfo = null;
			if (TryFindWordEntryByRoot(word, entryType, checkCompoundFlags, out wordInfo))
				return wordInfo;
			if (TryLookForWordWithPreffix(word, entryType, checkCompoundFlags, out wordInfo))
				return wordInfo;
			if (TryLookForWordWithSuffix(word, null, entryType, checkCompoundFlags, out wordInfo))
				return wordInfo;
			return wordInfo;
		}
		bool TryFindWordEntryByRoot(string root, WordEntryType entryType, bool checkCompoundFlags, out WordInfo wordInfo) {
			wordInfo = FindWordInfoByRoot(root, entryType, checkCompoundFlags);
			return wordInfo != null;
		}
		bool TryLookForWordWithPreffix(string word, WordEntryType entryType, bool checkCompoundFlags, out WordInfo wordInfo) {
			wordInfo = LookForWordWithPreffix(word, entryType, checkCompoundFlags);
			return wordInfo != null;
		}
		bool TryLookForWordWithSuffix(string word, Prefix preffix, WordEntryType entryType, bool checkCompoundFlags, out WordInfo wordInfo) {
			wordInfo = LookForWordWithSuffix(word, preffix, null, entryType, checkCompoundFlags);
			return wordInfo != null;
		}
		protected internal virtual WordEntry FindWordEntry(string root, Prefix prefix) {
			WordEntry wordEntry = Dictionary.FindWordEntry(root);
			while (wordEntry != null) {
				if (wordEntry.ContainsFlag(prefix.Identifier))
					return wordEntry;
				wordEntry = wordEntry.NextEntry;
			}
			return null;
		}
		protected internal virtual bool ValidateEntries(WordEntry wordEntry, AffixEntry affix, WordEntryType entryType, bool checkCompoundFlags) {
			if (entryType == WordEntryType.WholeWord)
				return Dictionary.CanBeSeparateWord(wordEntry, affix);
			else {
				if (checkCompoundFlags)
					return Dictionary.CheckCompoundingFlags(entryType, wordEntry, affix);
				else
					return true;
			}
		}
		protected internal virtual WordInfo FindWordInfoByRoot(string root, WordEntryType entryType, bool checkCompoundFlags) {
			WordEntry wordEntry = Dictionary.FindWordEntry(root);
			while (wordEntry != null) {
				if ((ValidateEntries(wordEntry, null, entryType, checkCompoundFlags)) && !Dictionary.NeedAffix(wordEntry))
					return new WordInfo(root, wordEntry);
				wordEntry = wordEntry.NextEntry;
			}
			return null;
		}
		protected internal virtual WordInfo LookForWordWithPreffix(string word, WordEntryType entryType, bool checkCompoundFlags) {
			int maxLength = word.Length;
			for (int length = 0; length <= maxLength; length++) {
				string affixKey = word.Substring(0, length);
				WordInfo wordInfo = LookForWordWithPreffix(word, affixKey, entryType, checkCompoundFlags);
				if (wordInfo != null)
					return wordInfo;
			}
			return null;
		}
		private WordInfo LookForWordWithPreffix(string word, string affixKey, WordEntryType entryType, bool checkCompoundFlags) {
			List<Prefix> prefixes = Dictionary.Prefixes.GetAffixes(affixKey);
			int count = prefixes.Count;
			for (int prefixIndex = 0; prefixIndex < count; prefixIndex++) {
				Prefix current = prefixes[prefixIndex];
				string root;
				if (!(CanUsePrefix(current, entryType) && current.TryGetWordRoot(word, out root)))
					continue;
				if (!Dictionary.NeedAffix(current)) {
					for (WordEntry entry = Dictionary.FindWordEntry(root); entry != null; entry = entry.NextEntry) {
						if (!entry.ContainsFlag(current.Identifier))
							continue;
						if (ValidateEntries(entry, current, entryType, checkCompoundFlags))
							return new WordInfo(word, entry, current);
					}
				}
				if (current.CanCombine) {
					WordInfo wordInfo = LookForWordWithSuffix(root, current, null, entryType, checkCompoundFlags);
					if (wordInfo != null)
						return wordInfo;
				}
			}
			return null;
		}
		protected internal virtual WordInfo LookForWordWithSuffix(string word, Prefix prefix, Suffix suffix, WordEntryType entryType, bool checkCompoundFlags) {
			int maxLength = word.Length;
			for (int length = 0; length <= maxLength; length++) {
				string affixKey = word.Substring(word.Length - length, length);
				WordInfo wordInfo = LookForWordWithSuffix(word, affixKey, prefix, suffix, entryType, checkCompoundFlags);
				if (wordInfo != null)
					return wordInfo;
			}
			return null;
		}
		protected internal virtual WordInfo LookForWordWithSuffix(string word, string affixKey, Prefix prefix, Suffix suffix, WordEntryType entryType, bool checkCompoundFlags) {
			List<Suffix> suffixes = Dictionary.Suffixes.GetAffixes(affixKey);
			int count = suffixes.Count;
			for (int suffixIndex = 0; suffixIndex < count; suffixIndex++) {
				Suffix current = suffixes[suffixIndex];
				string root;
				if (!(CanCombineAffixes(prefix, current, suffix) && CanUseSuffix(current, entryType) && current.TryGetWordRoot(word, out root)))
					continue;
				for (WordEntry entry = Dictionary.FindWordEntry(root); entry != null; entry = entry.NextEntry) {
					if (!CanUseSuffix(entry, prefix, current) || !CanUsePrefix(entry, prefix, current))
						continue;
					if (ValidateEntries(entry, current, entryType, checkCompoundFlags))
						return new WordInfo(word, entry, prefix, current);
				}
				if (suffix == null && Dictionary.Suffixes.CanCombineWithOtherSuffix(current)) {
					WordInfo wordInfo = LookForWordWithSuffix(root, prefix, current, entryType, checkCompoundFlags);
					if (wordInfo != null)
						return wordInfo;
				}
			}
			return null;
		}
		bool CanUsePrefix(WordEntry wordEntry, Prefix prefix, Suffix current) {
			return prefix == null || (current.ContainsFlag(prefix.Identifier) || wordEntry.ContainsFlag(prefix.Identifier));
		}
		bool CanUseSuffix(WordEntry wordEntry, Prefix prefix, Suffix current) {
			return wordEntry.ContainsFlag(current.Identifier) || (prefix != null && prefix.ContainsFlag(current.Identifier));
		}
		protected internal virtual bool CanCombineAffixes(Prefix prefix, Suffix suffix1, Suffix suffix2) {
			Debug.Assert(suffix1 != null);
			if (prefix != null && !(suffix1.CanCombine && prefix.CanCombine))
				return false;
			if (suffix2 != null)
				return suffix1.ContainsFlag(suffix2.Identifier);
			return !Dictionary.NeedAffix(suffix1) || (prefix != null && !Dictionary.NeedAffix(prefix));
		}
		protected internal virtual bool CanUseSuffix(Suffix suffix, WordEntryType entryType) {
			return entryType == WordEntryType.WholeWord || CanUseSuffixInCompound(suffix, entryType);
		}
		protected internal virtual bool CanUsePrefix(Prefix prefix, WordEntryType entryType) {
			return entryType == WordEntryType.WholeWord || CanUsePeffixInCompound(prefix, entryType);
		}
		protected internal virtual bool CanUseSuffixInCompound(Suffix suffix, WordEntryType entryType) {
			return (entryType == WordEntryType.Ending || Dictionary.IsAffixPermitted(suffix)) && !Dictionary.IsAffixForbidden(suffix);
		}
		protected internal virtual bool CanUsePeffixInCompound(Prefix preffix, WordEntryType entryType) {
			return (entryType == WordEntryType.Beginning || Dictionary.IsAffixPermitted(preffix)) && !Dictionary.IsAffixForbidden(preffix);
		}
		public string[] ExpandWordRoot(WordEntry wordEntry) {
			List<string> result = new List<string>();
			bool isRootAdded = false;
			for (WordEntry current = wordEntry; current != null; current = current.NextEntry) {
				if (IsWordForbidden(current))
					break;
				if (Dictionary.OnlyCompound(current))
					continue;
				if (!isRootAdded && !Dictionary.NeedAffix(current)) {
					result.Add(current.Value);
					isRootAdded = true;
				}
				List<Prefix> prefixes = GetAffixes(current, Dictionary.Prefixes);
				List<Suffix> suffixes = GetAffixes(current, Dictionary.Suffixes);
				ProcessAffixes(current, prefixes, suffixes, result);
				ProcessSuffixes(current, suffixes, result);
			}
			return result.ToArray();
		}
		bool CanUseAffix(string word, AffixEntry affix) {
			return affix.IsWordRootFit(word) && !Dictionary.OnlyCompound(affix);
		}
		bool CanUseAffix(string word, IFlagedObject wordEntry, AffixEntry affix) {
			return !wordEntry.ContainsFlag(affix.Identifier) && CanUseAffix(word, affix);
		}
		bool CanUseWordForm(string word) {
			WordEntry wordEntry = Dictionary.FindWordEntry(word);
			return wordEntry == null || !IsWordForbidden(wordEntry);
		}
		void ProcessSuffixes(WordEntry wordEntry, List<Suffix> suffixes, List<string> result) {
			int suffixesCount = suffixes.Count;
			for (int suffixIndex = 0; suffixIndex < suffixesCount; suffixIndex++) {
				Suffix suffix = suffixes[suffixIndex];
				if (!CanUseAffix(wordEntry.Value, suffix))
					continue;
				string wordWithSuffix = suffix.Combine(wordEntry.Value);
				if (String.IsNullOrEmpty(wordWithSuffix))
					continue;
				if (!String.IsNullOrEmpty(wordWithSuffix) && CanUseWordForm(wordWithSuffix) && !Dictionary.NeedAffix(suffix))
					result.Add(wordWithSuffix);
				if (suffix.CanCombine)
					ProcessSuffix(wordWithSuffix, wordEntry, suffix, result);
			}
		}
		void ProcessSuffix(string word, IFlagedObject wordEntry, Suffix suffix, List<string> result) {
			List<Prefix> prefixes = GetAffixes(suffix, Dictionary.Prefixes);
			List<Suffix> suffixes = GetAffixes(suffix, Dictionary.Suffixes);
			int prefixesCount = prefixes.Count;
			for (int prefixIndex = 0; prefixIndex < prefixesCount; prefixIndex++) {
				Prefix prefix = prefixes[prefixIndex];
				if (!prefix.CanCombine || !CanUseAffix(word, wordEntry, prefix))
					continue;
				string wordWithPrefix = prefix.Combine(word);
				if (String.IsNullOrEmpty(wordWithPrefix))
					continue;
				if (CanUseWordForm(wordWithPrefix) && (!Dictionary.NeedAffix(suffix) || !Dictionary.NeedAffix(prefix)))
					result.Add(wordWithPrefix);
				ProcessSuffix(wordWithPrefix, suffix, suffixes, result);
			}
			ProcessSuffix(word, suffix, suffixes, result);
		}
		void ProcessSuffix(string word, Suffix suffix, List<Suffix> suffixes, List<string> result) {
			int count = suffixes.Count;
			for (int suffixIndex = 0; suffixIndex < count; suffixIndex++) {
				Suffix relSuffix = suffixes[suffixIndex];
				string wordWithSuffix = relSuffix.Combine(word);
				if (String.IsNullOrEmpty(wordWithSuffix))
					continue;
				if (CanUseWordForm(wordWithSuffix) && suffix.CanCombine && CanUseAffix(word, suffix))
					result.Add(wordWithSuffix);
			}
		}
		void ProcessAffixes(WordEntry wordEntry, List<Prefix> prefixes, List<Suffix> suffixes, List<string> result) {
			int prefixesCount = prefixes.Count;
			for (int prefixIndex = 0; prefixIndex < prefixesCount; prefixIndex++) {
				Prefix prefix = prefixes[prefixIndex];
				if (!CanUseAffix(wordEntry.Value, prefix))
					continue;
				string wordWithPrefix = prefix.Combine(wordEntry.Value);
				if (String.IsNullOrEmpty(wordWithPrefix))
					continue;
				if (!Dictionary.NeedAffix(prefix))
					result.Add(wordWithPrefix);
				if (!prefix.CanCombine)
					continue;
				int suffixesCount = suffixes.Count;
				for (int suffixIndex = 0; suffixIndex < suffixesCount; suffixIndex++) {
					Suffix suffix = suffixes[suffixIndex];
					if (!(suffix.CanCombine && CanUseAffix(wordWithPrefix, suffix)))
						continue;
					string wordWithSuffix = suffix.Combine(wordWithPrefix);
					if (String.IsNullOrEmpty(wordWithSuffix))
						continue;
					if (CanUseWordForm(wordWithSuffix) && (!Dictionary.NeedAffix(suffix) || !Dictionary.NeedAffix(prefix)))
						result.Add(wordWithSuffix);
					List<Suffix> relSuffixes = GetAffixes(suffix, Dictionary.Suffixes);
					ProcessSuffix(wordWithSuffix, suffix, relSuffixes, result);
				}
				ProcessPrefix(wordWithPrefix, wordEntry, prefix, result);
			}
		}
		void ProcessPrefix(string word, IFlagedObject wordEntry, Prefix prefix, List<string> result) {
			List<Suffix> suffixes = GetAffixes(prefix, Dictionary.Suffixes);
			int suffixesCount = suffixes.Count;
			for (int suffixIndex = 0; suffixIndex < suffixesCount; suffixIndex++) {
				Suffix suffix = suffixes[suffixIndex];
				if (!suffix.CanCombine || !CanUseAffix(word, wordEntry, suffix))
					continue;
				string wordWithSuffix = suffix.Combine(word);
				if (String.IsNullOrEmpty(wordWithSuffix))
					continue;
				if (CanUseWordForm(wordWithSuffix) && (!Dictionary.NeedAffix(suffix) || !Dictionary.NeedAffix(prefix)))
					result.Add(wordWithSuffix);
				List<Suffix> relSuffixes = GetAffixes(suffix, Dictionary.Suffixes);
				ProcessSuffix(wordWithSuffix, suffix, relSuffixes, result);
			}
		}
		List<T> GetAffixes<T>(IFlagedObject obj, AffixCollection<T> affixes) where T : AffixEntry  {
			List<T> result = new List<T>();
			int flagCount = obj.Flags.Count;
			for (int i = 0; i < flagCount; i++) {
				List<T> affixSet = affixes.GetAffixes(obj.Flags[i]);
				if (affixSet.Count > 0)
					result.AddRange(affixSet);
			}
			return result;
		}
	}
	#endregion
	#region CompoundPattern
	public class CompoundPattern : ICompoundWordValidationRule {
		FlagedObject ending;
		FlagedObject begining;
		string replacement;
		public CompoundPattern(FlagTypes type) {
			this.ending = new FlagedObject(type);
			this.begining = new FlagedObject(type);
			this.replacement = String.Empty;
		}
		public FlagedObject Ending { get { return ending; } set { ending = value; } }
		public FlagedObject Begining { get { return begining; } set { begining = value; } }
		public string Replacement { get { return replacement; } set { replacement = value; } }
		public virtual bool CanCombine(WordInfo wordInfo1, WordInfo wordInfo2) {
			string word1 = wordInfo1.OriginalWord;
			string word2 = wordInfo2.OriginalWord;
			if (String.IsNullOrEmpty(word1) || String.IsNullOrEmpty(word2))
				return false;
			string ending = Ending.Value;
			string begining = Begining.Value;
			if ((word1.Length < ending.Length) || (word2.Length < begining.Length))
				return true;
			if (!ValidateFlags(wordInfo1, wordInfo2))
				return true;
			return !(word1.EndsWith(ending) && word2.StartsWith(begining));
		}
		protected internal virtual bool ValidateFlags(WordInfo wordInfo1, WordInfo wordInfo2) {
			if (Object.ReferenceEquals(wordInfo2, null))
				return false;
			if (Ending.Flags.Count > 0 && !wordInfo1.Root.ContainsFlag(Ending.Flags.First))
				return false;
			if (Begining.Flags.Count > 0 && !wordInfo2.Root.ContainsFlag(Begining.Flags.First))
				return false;
			return true;
		}
		protected internal bool CanApply(string word, int index) {
			if (!String.IsNullOrEmpty(Replacement)) {
				if (index > 0 && index + Replacement.Length < word.Length)
					return String.Equals(Replacement, word.Substring(index, Replacement.Length));
			}
			return false;
		}
	}
	#endregion
	#region ICompoundWordValidationRule
	public interface ICompoundWordValidationRule {
		bool CanCombine(WordInfo wordInfo1, WordInfo wordInfo2);
	}
	#endregion
	#region CheckCompoundTripleValidationRule
	public class CheckCompoundTripleValidationRule : ICompoundWordValidationRule {
		public bool CanCombine(WordInfo wordInfo1, WordInfo wordInfo2) {
			string firstWord = wordInfo1.OriginalWord;
			string secondWord = wordInfo2.OriginalWord;
			if (firstWord.Length < 2 || secondWord.Length < 2)
				return true;
			if (firstWord[firstWord.Length - 1] != secondWord[0])
				return true;
			return firstWord[firstWord.Length - 2] != secondWord[0] && firstWord[firstWord.Length - 1] != secondWord[1];
		}
	}
	#endregion
	#region CheckCompoundDuplicationValidationRule
	public class CheckCompoundDuplicationValidationRule : ICompoundWordValidationRule {
		public bool CanCombine(WordInfo wordInfo1, WordInfo wordInfo2) {
			string firstWord = wordInfo1.OriginalWord;
			string secondWord = wordInfo2.OriginalWord;
			return !String.Equals(firstWord, secondWord);
		}
	}
	#endregion
}
