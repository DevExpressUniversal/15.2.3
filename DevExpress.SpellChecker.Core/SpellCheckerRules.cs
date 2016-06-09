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
using System.Globalization;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Strategies;
#if SL
using DevExpress.Utils;
#endif
using System.Collections.Generic;
namespace DevExpress.XtraSpellChecker.Rules {
	#region Errors
	public abstract class SpellCheckErrorBase {
		#region Fields
		SuggestionCollection suggestions;
		SearchStrategy searchStrategy;
		SpellCheckOperation result;
		string suggestion;
		Position startPosition;
		Position finishPosition;
		string wrongWord = string.Empty;
		CultureInfo culture;
		#endregion
		protected SpellCheckErrorBase(SearchStrategy strategy) {
			this.searchStrategy = strategy;
			this.FinishPosition = searchStrategy.CurrentPosition;
			this.StartPosition = SearchStrategy.WordStartPosition;
			this.wrongWord = strategy.CheckedWord;
			this.culture = strategy.ActualCulture;
		}
		#region Properties
		public string WrongWord { get { return wrongWord; } }
		protected internal SpellCheckerBase SpellChecker { get { return SearchStrategy.SpellChecker; } }
		protected internal SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public virtual string Suggestion { get { return suggestion; } set { suggestion = value; } }
		public Position StartPosition { get { return startPosition; } set { startPosition = value; } }
		public Position FinishPosition { get { return finishPosition; } set { finishPosition = value; } }
		public SpellCheckOperation Result { get { return result; } set { result = value; } }
		public SpellCheckerRulesController RulesController { get { return SearchStrategy.RulesController; } }
		public virtual SuggestionCollection Suggestions { get { return suggestions; } protected set { suggestions = value; } }
		public CultureInfo Culture { get { return culture; } }
		#endregion
		public virtual bool HandleError() {
			if (String.IsNullOrEmpty(WrongWord))
				return false;
			return ProcessErrorUsingEvent();
		}
		public override bool Equals(object obj) {
			SpellCheckErrorBase error = obj as SpellCheckErrorBase;
			if(error == null)
				return false;
			if(Position.Equals(error.StartPosition, StartPosition) && Position.Equals(error.FinishPosition, FinishPosition))
				return true;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected abstract bool ProcessErrorUsingEvent();
	}
	public class NotInDictionaryWordError : SpellCheckErrorBase {
		public NotInDictionaryWordError(SearchStrategy strategy) : base(strategy) {
			Result = SpellCheckOperation.Change;
		}
		internal bool NeedSuggestions { get; set; }
		internal SuggestionCollection InnerSuggestions { get { return base.Suggestions; } set { base.Suggestions = value; } }
		public override SuggestionCollection Suggestions {
			get {
				if (InnerSuggestions == null && NeedSuggestions)
					InnerSuggestions = SpellChecker.GetSuggestions(WrongWord, Culture);
				return InnerSuggestions;
			}
		}
		internal string InnerSuggestion { get { return base.Suggestion; } set { base.Suggestion = value; } }
		public override string Suggestion {
			get {
				if (InnerSuggestion == null) {
					if (Suggestions != null && Suggestions.Count > 0)
						InnerSuggestion = Suggestions[0].Suggestion;
					else
						InnerSuggestion = WrongWord;
				}
				return InnerSuggestion;
			}
		}
		protected override bool ProcessErrorUsingEvent() {
			return NotInDictionaryWordFound();
		}
		protected virtual bool NotInDictionaryWordFound() {
			NeedSuggestions = false;
			if (SpellChecker.IsTextMode && !SpellChecker.ThreadChecking) {
				WrongWordRecord wordRecord = new WrongWordRecord(SpellChecker, Culture, WrongWord, StartPosition.Clone(), FinishPosition.Clone());
				SearchStrategy.WrongWords.Add(wordRecord);
			}
			else {
				if (SearchStrategy.NeedSuggestions)
					NeedSuggestions = true;
			}
			return OnNotInDictionaryWordFound();
		}
		protected virtual bool OnNotInDictionaryWordFound() {
			NotInDictionaryWordFoundEventArgs e = new NotInDictionaryWordFoundEventArgs(this, false);
			SpellChecker.OnNotInDictionaryWordFound(e);
			Result = e.Result;
			return e.Handled;
		}
	}
	public class RepeatedWordError : SpellCheckErrorBase {
		public RepeatedWordError(SearchStrategy strategy) : base(strategy) {
			Result = SpellCheckOperation.Delete;
		}
		protected override bool ProcessErrorUsingEvent() {
			return RepeatedWordFound();
		}
		protected virtual bool RepeatedWordFound() {
			bool handled = false;
			SpellCheckOperation result = SpellCheckOperation.Delete;
			RepeatedWordFoundEventArgs e = new RepeatedWordFoundEventArgs(WrongWord, result, string.Empty, this.StartPosition, new IntPosition(WrongWord.Length), handled);
			SpellChecker.OnRepeatedWordFound(e);
			Result = e.Result;
			Suggestion = e.Suggestion;
			return e.Handled;
		}
	}
	public class SpellCheckerErrorClassManager {
		public virtual SpellCheckErrorBase CreateNotInDictionaryWordError(SearchStrategy strategy) {
			return new NotInDictionaryWordError(strategy);
		}
		public virtual SpellCheckErrorBase CreateRepeatedWordError(SearchStrategy strategy) {
			return new RepeatedWordError(strategy);
		}
	}
	#endregion
	public abstract class SpellCheckerRule : IDisposable {
		SearchStrategy searchStrategy;
		protected SpellCheckerRule(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		public SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public ISpellCheckTextController TextController { get { return SearchStrategy.TextController; } }
		public CultureInfo Culture { get { return SearchStrategy.ActualCulture; } }
		public abstract bool IsMet(string word);
		public virtual bool NeedCheckWord() {
			return false;
		}
		protected virtual SpellCheckErrorBase CreateSpellCheckerError() {
			return null;
		}
		protected virtual void OnCheckedWordChanged(SearchStrategy strategy, Position start, Position end) {
		}
		public SpellCheckErrorBase GetSpellCheckerError() { 
			return NeedCheckWord() ? CreateSpellCheckerError() : null;
		}
		public virtual void Dispose() {
		}
	}
	public class MisspelledWordRule : SpellCheckerRule {
		public MisspelledWordRule(SearchStrategy strategy): base(strategy) {}
		public override bool NeedCheckWord() { return true; }
		protected override SpellCheckErrorBase CreateSpellCheckerError() { 
			return SearchStrategy.RulesController.ErrorClassManager.CreateNotInDictionaryWordError(SearchStrategy); 
		}
		public virtual bool FindWord(string word) {
			if (!HasLetters(word) || IsCorrect(word))
				return true;
			string correctedWord = word.Replace('\u2019', '\'');
			if (!Object.ReferenceEquals(word, correctedWord) && IsCorrect(correctedWord))
				return true;
			string[] strings = word.Split(new char[] { '-', '/' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < strings.Length; i++) {
				if (!IsCorrect(strings[i]))
					return false;
			}
			return true;
		}
		bool IsCorrect(string word) {
			return SearchStrategy.DictionaryHelper.FindWord(word, SearchStrategy.ActualCulture);
		}
		bool HasLetters(string word) {
			int length = word.Length;
			for (int charIndex = 0; charIndex < length; charIndex++) {
				if (Char.IsLetter(word[charIndex]))
					return true;
			}
			return false;
		}
		public override bool IsMet(string word) {
			return !FindWord(word);
		}
	}
	public class UpperCaseWordsRule : SpellCheckerRule { 
		public UpperCaseWordsRule(SearchStrategy strategy) : base(strategy) { }
		protected virtual bool IsUpperCaseWord(string word) { 
			string wordInUpperCase = word.ToUpper(Culture);
			return AreEqual(word, wordInUpperCase) && !AreEqual(word.ToLower(), wordInUpperCase);
		}
		bool AreEqual(string str1, string str2) {
#if SL
			return string.Compare(str1, str2, Culture, CompareOptions.None) == 0; 
#else
			return string.Compare(str1, str2, false, Culture) == 0; 
#endif
		}
		public override bool IsMet(string word) {
			return IsUpperCaseWord(word);
		}
	}
	public class MixedCaseWordsRule : UpperCaseWordsRule {
		public MixedCaseWordsRule(SearchStrategy strategy) : base(strategy) { }
		protected virtual bool IsMixedCaseWord(string word) { 
			if(word.Length <= 1) return false;
			int startIndex = char.IsUpper(word[0]) ? 1 : 0;
#if SL
			return string.Compare(word, startIndex, word.ToUpper(Culture), startIndex, word.Length, Culture, CompareOptions.None) != 0 &&
				string.Compare(word, startIndex, word.ToLower(Culture), startIndex, word.Length, Culture, CompareOptions.None) != 0; 
#else
			return string.Compare(word, startIndex, word.ToUpper(Culture), startIndex, word.Length, false, Culture) != 0 &&
				string.Compare(word, startIndex, word.ToLower(Culture), startIndex, word.Length, false, Culture) != 0; 
#endif
		}
		public override bool IsMet(string word) {
			return IsMixedCaseWord(word);
		}
	}
	public class WordsWithNumbersRule : SpellCheckerRule { 
		public WordsWithNumbersRule(SearchStrategy strategy) : base(strategy) { }
		protected virtual bool HasWordNumber(string word) { 
			bool hasNumber = false;
			bool hasLetter = false;
			for(int i = 0;i < word.Length;i++) {
				if(char.IsDigit(word, i))
					hasNumber = true;
				else
					if(char.IsLetter(word, i))
						hasLetter = true;
				if(hasNumber && hasLetter)
					return true;
			}
			return false;
		}
		public override bool IsMet(string word) {
			return HasWordNumber(word);
		}
	}
	public class NumberRule : SpellCheckerRule { 
		public NumberRule(SearchStrategy strategy) : base(strategy) { }
		protected virtual bool IsWordANumber(string word) { 
			bool result = true;
			for(int i = 0; i < word.Length; i++) {
				result = result && char.IsDigit(word, i);
				if(!result)
					break;
			}
			return result;
		}
		public override bool IsMet(string word) {
			return IsWordANumber(word);
		}
	}
	public class UrlsRule : SpellCheckerRule { 
		public UrlsRule(SearchStrategy strategy) : base(strategy) { }
		string[] webSitePrefixes = null;
		protected string[] WebSitePrefixes {
			get { 
				if(webSitePrefixes == null) {
					webSitePrefixes = new string[4];
					webSitePrefixes[0] = "http";
					webSitePrefixes[1] = "https";
					webSitePrefixes[2] = "www";
					webSitePrefixes[3] = "ftp";
				}
				return webSitePrefixes;
			}
		}
		protected virtual bool IsWordPartOfUrl(string word) {
			for(int i = 0; i < WebSitePrefixes.Length; i++) {
				if(word.Length >= WebSitePrefixes[i].Length) {
					string prefix = word.Substring(0, WebSitePrefixes[i].Length);
#if SL
					if(string.Compare(prefix, webSitePrefixes[i], Culture, CompareOptions.IgnoreCase) == 0)
						return true;
#else
					if (string.Compare(prefix, webSitePrefixes[i], true, Culture) == 0)
						return true;
#endif
				}
			}
			return false;
		}
		public override bool IsMet(string word) {
			return IsWordPartOfUrl(word);			
		}
	}
	public class UriRule : SpellCheckerRule {
		public UriRule(SearchStrategy strategy)
			: base(strategy) {
		}
		public override bool IsMet(string word) {
			return UriHelper.IsUri(word);
		}
	}
	public class EmailsRule : SpellCheckerRule {  
		public EmailsRule(SearchStrategy strategy) : base(strategy) {}
		protected virtual bool IsWordPartOfEMail(string word) {
			return word.IndexOf('@') > 1;
		}
		public override bool IsMet(string word) {
			return IsWordPartOfEMail(word);			
		}
	}
	public class RepeatedWordsRule : SpellCheckerRule { 
		public RepeatedWordsRule(SearchStrategy strategy) : base(strategy) {}
		protected virtual bool IsRepeatedWord(string word) {
			string previous = TextController.GetPreviousWord(SearchStrategy.CurrentPosition);
#if SL
			bool result = string.Compare(previous, word, Culture, CompareOptions.None) == 0;
#else
			bool result = string.Compare(previous, word, true, Culture) == 0;
#endif
			if(result) {
				Position pos = TextController.GetPrevPosition(SearchStrategy.CurrentPosition);
				string text = TextController.GetWord(pos, SearchStrategy.WordStartPosition).Trim(' ');
				return text.Length == 0;
			}
			return result;
		}
		public override bool IsMet(string word) {
			return IsRepeatedWord(word);  
		}
		public override bool NeedCheckWord() { return true; }
		protected override SpellCheckErrorBase CreateSpellCheckerError() { 
			return SearchStrategy.RulesController.ErrorClassManager.CreateRepeatedWordError(SearchStrategy); 
		}
	}
	public class TagsRule : SpellCheckerRule {
		Stack<Position> openTagMarksStack = new Stack<Position>();
		public TagsRule(SearchStrategy strategy)
			: base(strategy) {
			SubscribeToSearchStrategyEvents();
		}
		void SubscribeToSearchStrategyEvents() {
			SearchStrategy.CheckedWordChanged += OnCheckedWordChanged;
		}
		void UnsubscribeFromSearchStrategyEvents() {
			SearchStrategy.CheckedWordChanged -= OnCheckedWordChanged;
		}
		void OnCheckedWordChanged(object sender, EventArgs e) {
			Position wordStartPosition = SearchStrategy.WordStartPosition.Clone();
			if (wordStartPosition.IsZero)
				return;
			Position wordEndPosition = SearchStrategy.CurrentPosition.Clone();
			Position prevWordEndPosition = TextController.GetPrevPosition(wordEndPosition);
			if (Position.IsGreaterOrEqual(prevWordEndPosition, wordStartPosition))
				return;
			Position startPosition = prevWordEndPosition;
			while (Position.IsLess(startPosition, wordStartPosition)) {
				Position endPosition = startPosition.Clone();
				endPosition++;
				string character = TextController.GetWord(startPosition, endPosition);
				if (!String.IsNullOrEmpty(character)) {
					if (IsOpenTagBracket(character[0]))
						this.openTagMarksStack.Push(startPosition.Clone());
					else if (IsCloseTagBracket(character[0]) && this.openTagMarksStack.Count > 0)
						this.openTagMarksStack.Pop();
				}
				startPosition = endPosition;
			}
		}
		protected virtual bool IsOpenTagBracket(char tag) {
			return tag.CompareTo('<') == 0 || tag.CompareTo('[') == 0;
		}
		protected virtual bool IsCloseTagBracket(char tag) {
			return tag.CompareTo('>') == 0 || tag.CompareTo(']') == 0;
		}
		protected virtual char GetCloseTagBracket(char openTagBracket) {
			if(openTagBracket.CompareTo('<') == 0)
				return '>';
			if(openTagBracket.CompareTo('[') == 0)
				return ']';
			throw new Exception("Wrong Tag Bracket!");
		}
		protected virtual bool IsHtmlMarkUp(string word) {
			Position startWordPosition = Position.Subtract(SearchStrategy.WordStartPosition, new IntPosition(1));
			Position finishPosition = SearchStrategy.WordStartPosition;
			if(SearchStrategy.WordStartPosition == Position.Null || ((IntPosition)SearchStrategy.WordStartPosition).ActualIntPosition == 0) {
				startWordPosition = Position.Null;
				finishPosition = Position.Add(Position.Null, new IntPosition(1));
			}
			string firstSymbol = TextController.GetWord(startWordPosition, finishPosition);
#if SL
			if(string.Compare(firstSymbol, "&", Culture, CompareOptions.IgnoreCase) != 0)
				return false;
#else
			if (string.Compare(firstSymbol, "&", true, Culture) != 0)
				return false;
#endif
			Position finishWordPosition = Position.Add(SearchStrategy.CurrentPosition, new IntPosition(1));
			if(Position.IsGreater(finishWordPosition, (TextController as SimpleTextController).FinishPosition))
				finishWordPosition = (TextController as SimpleTextController).FinishPosition;
			string lastSymbol = TextController.GetWord(SearchStrategy.CurrentPosition, finishWordPosition);
#if SL
			if(string.Compare(lastSymbol, ";", Culture, CompareOptions.IgnoreCase) != 0)
				return false;
#else
			if (string.Compare(lastSymbol, ";", true, Culture) != 0)
				return false;
#endif
			return true;
		}
		public override bool IsMet(string word) {
			if (this.openTagMarksStack.Count > 0)
				return true;
			if(IsHtmlMarkUp(word))
				return true;
			return false;
		}
		public override void Dispose() {
			if (SearchStrategy != null)
				UnsubscribeFromSearchStrategyEvents();
			base.Dispose();
		}
	}
	public class SpellCheckerRuleCollection : ICollection<SpellCheckerRule>  {
		List<SpellCheckerRule> list = new List<SpellCheckerRule>();
		public SpellCheckerRule this[int index] { get { return list[index]; } }
		public void Remove(SpellCheckerRule rule) {
			list.Remove(rule);
		}
		public void AddRange(ICollection c) {
			foreach (SpellCheckerRule item in c)
				list.Add(item);
		}
		public void AddRange(ICollection<SpellCheckerRule> c){
			foreach(SpellCheckerRule item in c)
				list.Add(item);		
		}
		public void Add(SpellCheckerRule rule) {
			int index = IndexOf(rule);
			if(index == -1) 
				list.Add(rule);
		}
		public void Insert(int index, SpellCheckerRule rule) {
			if (IndexOf(rule) < 0)
				list.Insert(index, rule);
		}
		public int IndexOf(SpellCheckerRule rule) {
			return list.IndexOf(rule);
		}
		public void Clear() {
			list.ForEach(item => item.Dispose());
			list.Clear();
		}
		#region ICollection<SpellCheckerRule> Members
		public bool Contains(SpellCheckerRule item) {
			return list.Contains(item);
		}
		public void CopyTo(SpellCheckerRule[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return list.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		bool ICollection<SpellCheckerRule>.Remove(SpellCheckerRule item) {
			return list.Remove(item);
		}
		#endregion
		#region IEnumerable<SpellCheckerRule> Members
		public IEnumerator<SpellCheckerRule> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
	}
	public class SpellCheckerRulesController : IDisposable {
		SearchStrategy strategy = null;
		SpellCheckerRuleCollection actualRules = new SpellCheckerRuleCollection();
		SpellCheckerRule currentRule;
		OptionsSpellingBase optionsSpelling;
		SpellCheckerErrorClassManager errorClassManager = null;
		public SpellCheckerRulesController(SearchStrategy strategy, OptionsSpellingBase optionsSpelling) {
			this.strategy = strategy;
			this.optionsSpelling = optionsSpelling;
			SubscribeToOptionChangedEvent();
			RecreateActualRules();
		}
		public SearchStrategy Strategy { get { return this.strategy; } }
		public OptionsSpellingBase OptionsSpelling { get { return optionsSpelling; } }
		protected internal SpellCheckerRuleCollection ActualRules { get { return actualRules; } }
		protected SpellCheckerRule CurrentRule { get { return currentRule; } set { currentRule = value; } }
		protected virtual void SubscribeToOptionChangedEvent() {
			OptionsSpelling.OptionChanged += OnSpellCheckerOptionsChanged;
		}
		protected virtual void UnsubscribeFromOptionChangedEvent() {
			OptionsSpelling.OptionChanged -= OnSpellCheckerOptionsChanged;
		}
		public virtual bool NeedCheckWord() {
			return CurrentRule.NeedCheckWord();
		}
		public virtual SpellCheckErrorBase GetSpellCheckerError() {
			return CurrentRule.GetSpellCheckerError();
		}
		public virtual bool IsRepeatedWordError(SpellCheckErrorBase error) {
			return error is RepeatedWordError;
		}
		public virtual bool IsNotInDictionaryWordError(SpellCheckErrorBase error) {
			return error is NotInDictionaryWordError;
		}
		protected virtual SpellCheckerErrorClassManager CreateErrorClassManager() {
			return new SpellCheckerErrorClassManager();
		}
		public SpellCheckerErrorClassManager ErrorClassManager {
			get {
				if(errorClassManager == null)
					errorClassManager = CreateErrorClassManager();
				return errorClassManager;
			}
		}
		public void Dispose() {
			if (OptionsSpelling != null) {
				UnsubscribeFromOptionChangedEvent();
				this.optionsSpelling = null;
			}
			if (ActualRules != null) {
				ActualRules.Clear();
				this.actualRules = null;
			}
			this.strategy = null;
		}
		protected virtual void OnSpellCheckerOptionsChanged(object sender, EventArgs e) {
			RecreateActualRules();
		}
		protected internal virtual void RecreateActualRules() {
			ActualRules.Clear();
			ActualRules.AddRange(CreateRules(OptionsSpelling));
		}
		protected internal virtual SpellCheckerRuleCollection CreateRules(OptionsSpellingBase options) {
			SpellCheckerRuleCollection result = new SpellCheckerRuleCollection();
			if(options.IsIgnoreMarkupTags())
				result.Add(new TagsRule(Strategy));
			if (options.IsIgnoreUpperCaseWords())
				result.Add(new UpperCaseWordsRule(Strategy));
			if (options.IsIgnoreWordsWithNumbers())
				result.Add(new WordsWithNumbersRule(Strategy));
			if (options.IsIgnoreUri())
				result.Add(new UriRule(Strategy));
			if (options.IsIgnoreEmails())
				result.Add(new EmailsRule(Strategy));
			if (options.IsIgnoreMixedCaseWords())
				result.Add(new MixedCaseWordsRule(Strategy));
			result.Add(new NumberRule(Strategy));
			return result;
		}
		protected virtual bool IsWordMisspelled(string word, bool endsWithDot) {
			MisspelledWordRule rule = new MisspelledWordRule(Strategy);
			if (!rule.IsMet(word))
				return false;
			if (endsWithDot && !rule.IsMet(word + '.'))
				return false;
			this.currentRule = rule;
			return true;
		}
		protected virtual bool IsRepeatedWord(string word) {
			if(OptionsSpelling.IsIgnoreRepeatedWords())
				return false;
			RepeatedWordsRule rule = new RepeatedWordsRule(Strategy);
			if(rule.IsMet(word)) {
				this.currentRule = rule;
				return true;
			}
			return false;
		}
		public virtual bool IsMet(string word, bool endsWithDot) {
			Reset();
			bool isWordMisspelled = IsWordMisspelled(word, endsWithDot);
			bool isRepeatedWord = IsRepeatedWord(word);
			if(!isWordMisspelled && !isRepeatedWord)
				return false;
			for(int i = 0; i < ActualRules.Count; i++) {
				SpellCheckerRule rule = ActualRules[i] as SpellCheckerRule;
				if(rule.IsMet(word)) {
					this.currentRule = rule;
					return true;
				}
			}
			if(isWordMisspelled || isRepeatedWord) {
				return true;
			}
			this.currentRule = null;
			return false;
		}
		public virtual void Reset() {
			currentRule = null;
		}
	}
}
