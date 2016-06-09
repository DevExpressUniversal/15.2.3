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
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Parser;
using System.Collections.Specialized;
using DevExpress.XtraSpellChecker.Strategies;
#if !SL && !WPF
using System.Drawing;
#endif
using DevExpress.Utils.Controls;
using System.Collections.Generic;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker.Strategies {
	public enum StrategyState {
		None,
		Checking,
		CheckingAfterMainPart,
		CheckingMainPart,
		CheckingBeforeMainPart,
		FoundError,
		Start,
		Stop,
		UserStop,
		WaitForClientHandlingError,
		SuspendChecking,
		ResumeChecking
	}
	public abstract class SpellCheckerStateBase {
		SearchStrategy searchStrategy;
		protected SpellCheckerStateBase(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		public SpellCheckerBase SpellChecker { get { return SearchStrategy.SpellChecker; } }
		public SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public string CheckedWord { get { return SearchStrategy.CheckedWord; } set { SearchStrategy.CheckedWord = value; } }
		protected abstract void BeforeDoOperation();
		protected abstract void AfterDoOperation();
		protected abstract StrategyState DoOperationCore();
		public virtual StrategyState DoOperation() {
			BeforeDoOperation();
			try {
				return DoOperationCore();
			}
			finally {
				AfterDoOperation();
			}
		}
	}
	public class StartCheckSpellCheckerState : SpellCheckerStateBase {
		public StartCheckSpellCheckerState(SearchStrategy strategy)
			: base(strategy) {
		}
		protected override void BeforeDoOperation() { }
		protected override void AfterDoOperation() { }
		protected override StrategyState DoOperationCore() {
			if (CanCheckText()) {
				return StrategyState.Checking;
			}
			else {
				return StrategyState.Stop;
			}
		}
		protected virtual bool CanCheckText() {
			if (SpellChecker == null || String.IsNullOrEmpty(SearchStrategy.Text))
				return false;
			BeforeCheckEventArgs args = new BeforeCheckEventArgs(SearchStrategy.Text, SpellChecker.EditControl, false);
			SpellChecker.OnBeforeCheck(args);
			return !args.Cancel;
		}
	}
	public class CheckingSpellCheckerState : SpellCheckerStateBase {
		const int MaxWordLength = 64; 
		bool canUsePotentialNextPosition = false;
		Position potentialNextPosition = null;
		public CheckingSpellCheckerState(SearchStrategy strategy) : base(strategy) { }
		public Position CurrentPosition {
			get { return SearchStrategy.CurrentPosition; }
			set { SearchStrategy.CurrentPosition = value; }
		}
		public virtual Position StartPosition {
			get { return SearchStrategy.StartPosition; }
			set { SearchStrategy.StartPosition = value; }
		}
		public virtual Position FinishPosition {
			get { return SearchStrategy.FinishPosition; }
			set { SearchStrategy.FinishPosition = value; }
		}
		public Position WordStartPosition {
			get { return SearchStrategy.WordStartPosition; }
			set { SearchStrategy.WordStartPosition = value; }
		}
		protected Position PotentialNextPosition {
			get { return this.potentialNextPosition; }
		}
		protected bool CanUsePotentialNextPosition {
			get { return canUsePotentialNextPosition; }
			set { canUsePotentialNextPosition = value; }
		}
		bool EndsWithDot { get { return TextController[CurrentPosition] == '.'; } }
		protected virtual void CalcPotentialNextPosition() {
			potentialNextPosition = TextController.GetNextPosition(CurrentPosition);
			canUsePotentialNextPosition = true;
		}
		protected ISpellCheckTextController TextController { get { return SearchStrategy.TextController; } }
		public virtual SpellCheckErrorBase ActiveError {
			get { return SearchStrategy.ActiveError; }
			set { SearchStrategy.ActiveError = value; }
		}
		public Dictionary<string, string> ChangeAllList { get { return SpellChecker.ChangeAllList; } }
		public IIgnoreList IgnoreList { get { return SpellChecker.IgnoreList; } }
		protected virtual bool ShouldRaiseAfterCheckWord() {
			return !String.IsNullOrEmpty(CheckedWord) && ActiveError != null;
		}
		AfterCheckWordEventArgs GetAfterCheckWordEventArgs() {
			AfterCheckWordEventArgs args = null;
			if (ActiveError == null)
				args = new AfterCheckWordEventArgs(SpellChecker.EditControl, CheckedWord, CheckedWord, SpellCheckOperation.None, this.WordStartPosition);
			else
				args = new AfterCheckWordEventArgs(SpellChecker.EditControl, CheckedWord, ActiveError.Suggestion, ActiveError.Result, this.WordStartPosition);
			return args;
		}
		protected override void BeforeDoOperation() {
			IUriRecognitionSupport uriRecognizer = TextController as IUriRecognitionSupport;
			if (uriRecognizer != null)
				uriRecognizer.IgnoreUri = !SpellChecker.OptionsSpelling.IsIgnoreUri();
			OnAfterCheckWord();
		}
		protected virtual void OnAfterCheckWord() {
			if (ShouldRaiseAfterCheckWord())
				SpellChecker.OnAfterCheckWord(GetAfterCheckWordEventArgs());
		}
		protected override void AfterDoOperation() { }
		protected virtual bool CanCheckWord() {
			BeforeCheckWordEventArgs e = new BeforeCheckWordEventArgs(SpellChecker.EditControl, CheckedWord, false);
			SpellChecker.OnBeforeCheckWord(e);
			return !e.Cancel;
		}
		protected virtual bool CheckPositionBeforeDoNextStep() {
			CalcPotentialNextPosition();
			return (StartPosition == Position.Undefined || Position.IsGreaterOrEqual(CurrentPosition, StartPosition)) &&
				(FinishPosition == Position.Undefined || Position.IsLessOrEqual(PotentialNextPosition, FinishPosition)) &&
				!Position.Equals(CurrentPosition, PotentialNextPosition) &&
				TextController.CanDoNextStep(CurrentPosition);
		}
		protected virtual bool CanContinueSearch() {
			if (!CanCheckWord()) 
				return true;
			if (!SearchStrategy.ShouldCheckWord(WordStartPosition, CurrentPosition))
				return true;
			if (ShouldProcessWordWithoutChecking())
				return true;
			ActiveError = null;
			if (RulesController.IsMet(CheckedWord, EndsWithDot))
				if (RulesController.NeedCheckWord()) {
					ActiveError = RulesController.GetSpellCheckerError();
				}
			return ActiveError == null;
		}
		protected virtual bool ShouldProcessWordWithoutChecking() {
			if (CheckedWord.Length > MaxWordLength)
				return true;
			if (IgnoreList.Contains(WordStartPosition, this.CurrentPosition, CheckedWord)) {
				return true;
			}
			IIgnoreList ignoreList = SpellChecker.GetIgnoreListCore();
			if (!SearchStrategy.ThreadChecking && ignoreList != null && ignoreList.Contains(WordStartPosition, CurrentPosition, CheckedWord))
				return true;
			if (!SearchStrategy.ThreadChecking && ChangeAllList.ContainsKey(CheckedWord)) {
				SearchStrategy.DoSpellCheckOperation(SpellCheckOperation.SilentChange, ChangeAllList[CheckedWord]);
				return true;
			}
			return false;
		}
		protected SpellCheckerRulesController RulesController { get { return SearchStrategy.RulesController; } }
		protected virtual StrategyState GetStrategyStateWhenCannotDoNextStep() {
			return StrategyState.Stop;
		}
		protected virtual StrategyState GetStrategyIfNextStepIsDisabled() {
			return GetStrategyStateWhenCannotDoNextStep();
		}
		protected override StrategyState DoOperationCore() {
			while (true) {
				if (!CheckPositionBeforeDoNextStep()) {
					SearchStrategy.ActiveError = null;
					return GetStrategyIfNextStepIsDisabled(); 
				}
				Position newPosition = null;
				if (CanUsePotentialNextPosition) {
					newPosition = PotentialNextPosition;
					CanUsePotentialNextPosition = false;
				}
				else
					newPosition = TextController.GetNextPosition(CurrentPosition);
				if (Position.Equals(newPosition, CurrentPosition)) {
					return GetStrategyStateWhenEmptyWord();
				}
				CurrentPosition = newPosition;
				WordStartPosition = TextController.GetWordStartPosition(CurrentPosition);
				CheckedWord = TextController.GetWord(WordStartPosition, CurrentPosition);
				SearchStrategy.OnCheckedWordChanged();
				if (String.IsNullOrEmpty(CheckedWord)) {
					return GetStrategyStateWhenEmptyWord();
				}
				if (!CanContinueSearch())
					break;
				else
					SpellChecker.OnAfterCheckWord(GetAfterCheckWordEventArgs());
			}
			return StrategyState.FoundError;
		}
		protected virtual StrategyState GetStrategyStateWhenEmptyWord() {
			return StrategyState.Stop;
		}
	}
	public class ErrorFoundSpellCheckerState : SpellCheckerStateBase {
		public ErrorFoundSpellCheckerState(SearchStrategy strategy)
			: base(strategy) {
		}
		protected override void BeforeDoOperation() {
		}
		protected override void AfterDoOperation() {
		}
		protected override StrategyState DoOperationCore() {
			SpellCheckErrorBase error = SearchStrategy.ActiveError;
			if (error.HandleError()) {
				SearchStrategy.DoSpellCheckOperation(error.Result, error.Suggestion);
				return StrategyState.Checking;
			}
			else if (!SpellChecker.CanHandleErrorVisually()) {
				return StrategyState.Checking;
			}
			return StrategyState.WaitForClientHandlingError;
		}
	}
	public class StopCheckSpellCheckerState : SpellCheckerStateBase {
		public StopCheckSpellCheckerState(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override void BeforeDoOperation() { }
		protected override void AfterDoOperation() { }
		protected override StrategyState DoOperationCore() {
			SearchStrategy.OnAfterCheck(StopCheckingReason.Default);
			return StrategyState.None;
		}
	}
	public class UserStopSpellCheckerState : SpellCheckerStateBase {
		public UserStopSpellCheckerState(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override void BeforeDoOperation() { }
		protected override void AfterDoOperation() { }
		protected override StrategyState DoOperationCore() {
			SearchStrategy.OnAfterCheck(StopCheckingReason.User);
			return StrategyState.None;
		}
	}
	public class TextOperationsManager {
		bool isUndo = false;
		SearchStrategy searchStrategy = null;
		public TextOperationsManager(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		public SearchStrategy SearchStrategy {
			get { return this.searchStrategy; }
		}
		public UndoManagerBase UndoManager { get { return SearchStrategy.UndoManager; } }
		public ISpellCheckTextController TextController {
			get { return SearchStrategy.TextController; }
		}
		public Position CurrentPosition {
			get { return SearchStrategy.CurrentPosition; }
			set { SearchStrategy.CurrentPosition = value; }
		}
		public Position WordStartPosition {
			get { return SearchStrategy.WordStartPosition; }
		}
		public string CheckedWord { get { return SearchStrategy.CheckedWord; } }
		public IIgnoreList Exceptions { get { return SearchStrategy.Exceptions; } }
		public virtual IIgnoreList IgnoreList { get { return SearchStrategy.IgnoreList; } }
		public virtual Dictionary<string, string> ChangeAllList { get { return SearchStrategy.ChangeAllList; } }
		public virtual DictionaryHelper DictionaryHelper { get { return SearchStrategy.DictionaryHelper; } }
		#region operations
		protected virtual void WriteUndoInfo(int actionID, Position startPosition, Position finishPosition, string oldWord) {
			if (!IsUndo)
				UndoManager.Add(actionID, startPosition, finishPosition, oldWord);
		}
		protected virtual bool Replace(Position start, Position finish, string newWord, int undoActionID) {
			Position savedPos = CurrentPosition;
			bool isReplaced = ReplaceCore(start, finish, newWord, undoActionID);
			if (isReplaced) {
				if (String.IsNullOrEmpty(newWord)) {
					Position pos = TextController.GetNextPosition(start);
					CurrentPosition = TextController.GetWordStartPosition(pos);
					SearchStrategy.DeltaPosition = Position.Subtract(start, finish);
				}
				else {
					CurrentPosition = Position.Add(start, TextController.GetTextLength(newWord));
					SearchStrategy.DeltaPosition = Position.Subtract(CurrentPosition, savedPos);
				}
			}
			return isReplaced;
		}
		public virtual void Replace(string newWord) {
			Replace(WordStartPosition, CurrentPosition, newWord, UndoManagerBase.ReplaceActionID);
		}
		protected virtual bool ReplaceCore(Position start, Position finish, string newWord, int undoActionID) {
			string replacingText = start != null && finish != null ? TextController.GetWord(start, finish) : CheckedWord;
			if (TextController.ReplaceWord(start, finish, newWord)) {
				WriteUndoInfo(undoActionID, start, Position.Add(start, TextController.GetTextLength(newWord)), replacingText);
				return true;
			}
			return false;
		}
		public virtual void Change(string word) {
			Replace(word);
		}
		public virtual bool Replace(Position start, Position finish, string newWord) {
			return Replace(start, finish, newWord, UndoManagerBase.ReplaceActionID);
		}
		public virtual void Delete() {
			Delete(WordStartPosition, CurrentPosition);
		}
		public virtual void Recheck() { }
		public virtual void Delete(Position start, Position finish) {
			Position savedStart = start;
			Position savedFinish = finish;
			string oldText = TextController.Text;
			Position oldLength = TextController.GetTextLength(oldText);
			if (!TextController.DeleteWord(ref start, ref finish))
				return;
			string oldWord = string.Empty;
			string newText = TextController.Text;
			Position newLength = TextController.GetTextLength(newText);
			try {
				TextController.Text = oldText;
				oldWord = TextController.GetWord(start, finish);
				WriteUndoInfo(UndoManagerBase.DeleteActionID, start, finish, oldWord);
			}
			finally {
				TextController.Text = newText;
			}
			UpdateCurrentPositionAfterDelete(savedStart, savedFinish);
			SearchStrategy.DeltaPosition = Position.Subtract(newLength, oldLength);
		}
		protected virtual void UpdateCurrentPositionAfterDelete(Position start, Position finish) {
			Position pos = TextController.GetTextLength(TextController.Text);
			if (Position.IsGreater(start, pos))
				CurrentPosition = pos;
			else
				CurrentPosition = start;
		}
		public virtual void AddToDictionary(string word, Position start, Position finish) {
			AddToDictionaryCore(word);
			WriteUndoInfo(UndoManagerBase.AddWordActionID, start, finish, CheckedWord);
			SearchStrategy.SpellChecker.OnWordAdded(word);
		}
		internal void AddToDictionaryCore(string word) {
			DictionaryHelper.AddWord(word, SearchStrategy.ActualCulture);
		}
		public virtual void IgnoreOnce() {
			IgnoreOnce(WordStartPosition, CurrentPosition, CheckedWord);
		}
		public virtual void IgnoreOnce(Position start, Position finish, string word) {
			if (Exceptions != null)
				Exceptions.Add(start, finish, word);
			WriteUndoInfo(UndoManagerBase.IgnoreActionID, start, finish, word);
		}
		public virtual void IgnoreAll(string word) {
			IgnoreList.Add(word);
			WriteUndoInfo(UndoManagerBase.IgnoreAllActionID, Position.Undefined, Position.Undefined, CheckedWord);
			WriteUndoInfo(UndoManagerBase.IgnoreActionID, WordStartPosition, CurrentPosition, CheckedWord);
		}
		public virtual void ChangeAll(string word) {
			ChangeAllList.Add(CheckedWord, word);
			WriteUndoInfo(UndoManagerBase.ChangeAllActionID, Position.Undefined, Position.Undefined, CheckedWord);
			Replace(word);
		}
		public virtual void ChangeAll(Position start, Position finish, string word) {
			if (!Replace(start, finish, word))
				return;
			ChangeAllList.Add(CheckedWord, word);
			WriteUndoInfo(UndoManagerBase.ChangeAllActionID, Position.Undefined, Position.Undefined, CheckedWord);
		}
		public virtual void IgnoreSilent() {
			IgnoreSilent(CheckedWord);
		}
		public virtual void IgnoreSilent(string word) {
			WriteUndoInfo(UndoManagerBase.SilentIgnoreActionID, Position.Undefined, Position.Undefined, word);
		}
		public virtual void ChangeSilent(string suggestion) {
			ChangeSilent(WordStartPosition, CurrentPosition, suggestion);
		}
		public virtual void ChangeSilent(Position start, Position finish, string suggestion) {
			Replace(start, finish, suggestion, UndoManagerBase.SilentChangeActionID);
		}
		protected internal virtual bool IsUndo {
			get { return isUndo; }
			set { isUndo = value; }
		}
		public virtual void Undo() {
			IsUndo = true;
			try {
				UndoManager.DoUndo();
			}
			finally {
				IsUndo = false;
			}
		}
		public virtual void Cancel() {
			SearchStrategy.SetState(StrategyState.UserStop);
		}
		public virtual void DoCustomCommand() { }
		public virtual void ShowOptionsDialog() { }
		#endregion
		public virtual void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion, SpellCheckErrorBase error) {
			CultureInfo culture = error != null ? error.Culture : SearchStrategy.ActualCulture;
			SpellCheckerCommand command = GetCommand(operation, suggestion, CheckedWord, WordStartPosition, CurrentPosition, culture);
			command.DoCommand();
		}
		protected virtual void InitializeCommand(SpellCheckerCommand command, string suggestion, string word, Position start, Position finish, CultureInfo culture) {
			command.Suggestion = suggestion;
			command.Word = word;
			command.Start = start;
			command.Finish = finish;
			command.Culture = culture;
		}
		public virtual SpellCheckerCommand GetCommand(SpellCheckOperation operation, string suggestion, string word, Position start, Position finish, CultureInfo culture) {
			SpellCheckerCommand command = CreateCommand(operation);
			InitializeCommand(command, suggestion, word, start, finish, culture);
			return command;
		}
		protected virtual SpellCheckerCommand CreateCommand(SpellCheckOperation operation) {
			switch (operation) {
				case SpellCheckOperation.Cancel:
					return new SpellCheckerCancelCommand(this);
				case SpellCheckOperation.AddToDictionary:
					return new SpellCheckerAddToDictionaryCommand(this);
				case SpellCheckOperation.Change:
					return new SpellCheckerChangeCommand(this);
				case SpellCheckOperation.SilentChange:
					return new SpellCheckerSilentChangeCommand(this);
				case SpellCheckOperation.ChangeAll:
					return new SpellCheckerChangeAllCommand(this);
				case SpellCheckOperation.Delete:
					return new SpellCheckerDeleteCommand(this);
				case SpellCheckOperation.Ignore:
					return new SpellCheckerIgnoreCommand(this);
				case SpellCheckOperation.SilentIgnore:
					return new SpellCheckerSilentIgnoreCommand(this);
				case SpellCheckOperation.IgnoreAll:
					return new SpellCheckerIgnoreAllCommand(this);
				case SpellCheckOperation.Recheck:
					return new SpellCheckerRecheckCommand(this);
				case SpellCheckOperation.Undo:
					return new SpellCheckerUndoCommand(this);
				case SpellCheckOperation.Custom:
					return new SpellCheckerCustomCommand(this);
				case SpellCheckOperation.Options:
					return new SpellCheckerOptionsCommand(this);
				case SpellCheckOperation.None:
					return new SpellCheckerNoSuggestionsCommand(this);
				default:
					throw new InvalidEnumArgumentException(operation.ToString(), (int)operation, typeof(SpellCheckOperation));
			}
		}
		protected internal virtual void OnUndo(IUndoItem undoItem) {
		}
	}
	public class PartTextOperationsManager : TextOperationsManager {
		public PartTextOperationsManager(PartTextSearchStrategy searchStrategy) : base(searchStrategy) { }
		public virtual new PartTextSearchStrategy SearchStrategy {
			get { return base.SearchStrategy as PartTextSearchStrategy; }
		}
		public Position StoredStartPosition {
			get { return SearchStrategy.StoredStartPosition; }
			set { SearchStrategy.StoredStartPosition = value; }
		}
		public Position StoredFinishPosition {
			get { return SearchStrategy.StoredFinishPosition; }
			set { SearchStrategy.StoredFinishPosition = value; }
		}
		protected override bool Replace(Position start, Position finish, string newWord, int undoActionID) {
			if (base.Replace(start, finish, newWord, undoActionID)) {
				UpdateStoredPositions();
				return true;
			}
			return false;
		}
		public override void Delete(Position start, Position finish) {
			base.Delete(start, finish);
			UpdateStoredPositions();
		}
		protected virtual void UpdateStoredPositions() {
			if (!IsUndo)
				UpdateStoredPositions(SearchStrategy.DeltaPosition);
		}
		void UpdateStoredPositions(Position delta) {
			if (Position.IsLess(WordStartPosition, StoredStartPosition)) {
				StoredStartPosition = Position.Add(StoredStartPosition, delta);
				StoredFinishPosition = Position.Add(StoredFinishPosition, delta);
			}
			else
				if (Position.IsLess(WordStartPosition, StoredFinishPosition))
					StoredFinishPosition = Position.Add(StoredFinishPosition, delta);
			if (Position.IsGreater(StoredStartPosition, StoredFinishPosition)) 
				StoredFinishPosition = StoredStartPosition;
		}
		protected internal override void OnUndo(IUndoItem undoItem) {
			if (undoItem.StartPosition == Position.Undefined || undoItem.FinishPosition == Position.Undefined)
				return;
			Position oldTextLength = TextController.GetTextLength(undoItem.OldText);
			Position newTextLength = Position.Subtract(undoItem.FinishPosition, undoItem.StartPosition);
			Position delta = Position.Subtract(oldTextLength, newTextLength);
			UpdateStoredPositions(delta);
		}
	}
	public abstract class SearchStrategy : IDisposable {
		SpellCheckerBase spellChecker;
		SpellCheckerRulesController rulesController;
		SpellCheckErrorBase activeError = null;
		Stack<StrategyState> stateStack = new Stack<StrategyState>();
		CultureInfo localCulture;
		CultureInfo culture;
		StrategyStateChangedEventHandler strategyStateChanged;
		EventHandler spellCheckOperationCompleted;
		EventHandler afterCheck;
		string checkedWord = null;
		StateCache stateCache = new StateCache();
		ISpellCheckTextController textController = null;
		ISupportMultiCulture cultureProvider;
		Position currentPosition = null;
		Position wordStartPosition = null;
		Position startPosition;
		Position finishPosition;
		Position deltaPosition = null;
		ContainerSearchStrategyBase parentStrategy = null;
		UndoManagerBase undoManager = null;
		TextOperationsManager operationsManager = null;
		List<WrongWordRecord> wrongWords = new List<WrongWordRecord>();
		bool isMisspelled = false;
		bool needSuggestions = true;
		bool threadChecking = false;
		protected SearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController) {
			this.spellChecker = spellChecker;
			this.textController = textController;
			Initialize();
		}
		protected SearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, ContainerSearchStrategyBase parentStrategy)
			: this(spellChecker, textController) {
			this.parentStrategy = parentStrategy;
		}
		public virtual SpellCheckerBase SpellChecker { get { return this.spellChecker; } }
		public virtual string Text {
			get { return TextController.Text; }
			set {
				TextController.Text = value;
				OnTextChanged();
			}
		}
		public virtual bool StopHandler() {
			return false;
		}
		protected virtual void OnTextChanged() {
			WrongWords.Clear();
		}
		public string CheckedWord {
			get { return checkedWord; }
			set { checkedWord = value; }
		}
		public CultureInfo Culture { 
			get { return culture; } 
			set {
				if (culture == value)
					return;
				culture = value;
				LocalCulture = null;
			} 
		}
		protected internal virtual CultureInfo ActualCulture { get { return LocalCulture ?? Culture; } }
		CultureInfo LocalCulture { get { return localCulture; } set { localCulture = value; } }
		public virtual Position StartPosition { get { return this.startPosition; } set { this.startPosition = value; } }
		public virtual Position FinishPosition { get { return this.finishPosition; } set { this.finishPosition = value; } }
		public virtual Position CurrentPosition {
			get { return currentPosition; }
			set {
				if (Position.Equals(CurrentPosition, value))
					return;
				currentPosition = value;
				OnCurrentPositionChanged();
			}
		}
		public virtual Position WordStartPosition { get { return wordStartPosition; } set { wordStartPosition = value; } }
		public virtual Position DeltaPosition {
			get { return deltaPosition; }
			set {
				deltaPosition = value;
				OnDeltaPositionChanged();
			}
		}
		public virtual ISpellCheckTextController TextController { get { return textController; } set { textController = value; } }
		protected internal ISupportMultiCulture CultureProvider {
			get { return cultureProvider; }
			set {
				if (CultureProvider == value)
					return;
				cultureProvider = value;
				OnCultureProviderChanged();
			}
		}
		#region CheckedWordChanged
		EventHandler checkedWordChangedHandler;
		internal event EventHandler CheckedWordChanged { add { checkedWordChangedHandler += value; } remove { checkedWordChangedHandler -= value; } }
		void RaiseCheckedWordChanged() {
			EventHandler handler = checkedWordChangedHandler;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected virtual void Initialize() {
			this.currentPosition = Position.Null;
			this.startPosition = Position.Undefined;
			this.startPosition = Position.Undefined;
			this.stateStack.Push(StrategyState.None);
			this.culture = SpellChecker.Culture;
			this.rulesController = CreateRulesController();
			this.cultureProvider = TextController as ISupportMultiCulture;
			ResetLocalCulture();
		}
		protected virtual void OnCultureProviderChanged() {
			ResetLocalCulture();
		}
		void ResetLocalCulture() {
			LocalCulture = null;
		}
		protected virtual void OnDeltaPositionChanged() {
			UndoManager.UpdateCompositeItemPositionsByChangeAll();
		}
		void OnCurrentPositionChanged() {
		}
		protected internal Stack<StrategyState> StateStack { get { return stateStack; } }
		public virtual StrategyState State { get { return StateStack.Peek(); } }
		#region Events
		public event StrategyStateChangedEventHandler StrategyStateChanged { add { strategyStateChanged += value; } remove { strategyStateChanged -= value; } }
		public event EventHandler AfterCheck { add { afterCheck += value; } remove { afterCheck -= value; } }
		public event EventHandler SpellCheckOperationCompleted { add { spellCheckOperationCompleted += value; } remove { spellCheckOperationCompleted -= value; } }
		#region ClientHandingError
		EventHandler onClientHandlingError;
		public event EventHandler ClientHandlingError { add { onClientHandlingError += value; } remove { onClientHandlingError -= value; } }
		protected virtual void RaiseClientHandlingErrorEvent() {
			if (onClientHandlingError != null)
				onClientHandlingError(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected virtual void OnStrategyStateChanged() {
			RaiseStrategyStateChanged();
		}
		protected internal virtual void UpdateLocalCulture(Position start, Position end) {
			LocalCulture = GetLocalCulture(start, end);
			SpellChecker.LoadDictionaries(ActualCulture);
		}
		protected internal virtual bool ShouldCheckWord(Position start, Position end) {
			return (CultureProvider != null) ? CultureProvider.ShouldCheckWord(start, end) : true;
		}
		protected internal virtual CultureInfo GetLocalCulture(Position start, Position end) {
			return (CultureProvider != null) ? CultureProvider.GetCulture(start, end) : null;
		}
		void RaiseStrategyStateChanged() {
			if (strategyStateChanged != null)
				strategyStateChanged(this, new StrategyStateChangedEventArgs(State));
		}
		public bool ThreadChecking { get { return this.threadChecking; } set { this.threadChecking = value; } }
		public void Dispose() {
			if (rulesController != null) {
				this.rulesController.Dispose();
				this.rulesController = null;
			}
			if (this.textController != null) {
				IDisposable disposable = this.textController as IDisposable;
				if (disposable != null)
					disposable.Dispose();
				this.textController = null;
				this.cultureProvider = null;
			}
			this.parentStrategy = null;
		}
		protected internal virtual void OnAfterCheck(StopCheckingReason reason) {
			RaiseAfterCheck();
			SpellChecker.OnAfterCheck(new AfterCheckEventArgs(reason));
			Reset();
		}
		protected virtual void RaiseAfterCheck() {
			if (afterCheck != null)
				afterCheck(this, EventArgs.Empty);
		}
		protected virtual void Reset() {
			CurrentPosition = Position.Null;
			WordStartPosition = Position.Null;
			ActiveError = null;
			StateCache.Clear();
			if (ShouldClearLists()) {
				ChangeAllList.Clear();
			}
		}
		protected virtual bool ShouldClearLists() {
			return !ThreadChecking;
		}
		protected internal List<WrongWordRecord> WrongWords {
			get { return wrongWords; }
		}
		public virtual void Check() {
			if (!IsChecking()) {
				CurrentPosition = StartPosition;
				SetState(StrategyState.Start);
			}
		}
		protected internal virtual void ProcessState(StrategyState startState) {
			StrategyState currentState = startState;
			for (; ; ) {
				if (currentState != StrategyState.ResumeChecking) {
					if (currentState != State && CanSetState(currentState))
						SetStateCore(currentState);
					else
						return;
				}
				else
					StateStack.Pop();
				if (State == StrategyState.None || State == StrategyState.SuspendChecking)
					return;
				if (State == StrategyState.WaitForClientHandlingError) {
					OnClientHandlingError();
					return;
				}
				SpellCheckerStateBase spellCheckerState = CreateSpellCheckerState(State);
				currentState = spellCheckerState.DoOperation();
			}
		}
		public virtual void SuspendChecking() {
			StateStack.Push(StrategyState.SuspendChecking);
		}
		public virtual void ResumeChecking() {
			if (State != StrategyState.SuspendChecking)
				return;
			ProcessState(StrategyState.ResumeChecking);
		}
		protected virtual bool CanSetState(StrategyState value) {
			switch (State) {
				case StrategyState.None:
					return value == StrategyState.Start;
				case StrategyState.Stop:
				case StrategyState.UserStop:
					return value == StrategyState.None;
				case StrategyState.SuspendChecking:
				case StrategyState.WaitForClientHandlingError:
					return IsCheckingState(value) || value == StrategyState.Stop || value == StrategyState.UserStop;
				default:
					return true;
			}
		}
		bool IsCheckingState(StrategyState state) {
			return state == StrategyState.Checking || state == StrategyState.CheckingAfterMainPart || state == StrategyState.CheckingBeforeMainPart || state == StrategyState.CheckingMainPart;
		}
		protected virtual void OnClientHandlingError() {
			RaiseClientHandlingErrorEvent();
		}
		public void SetState(StrategyState newState) {
			ProcessState(newState);
		}
		protected virtual void SetStateCore(StrategyState newState) {
			StateStack.Pop();
			StateStack.Push(newState);
			OnStrategyStateChanged();
		}
		public bool NeedSuggestions {
			get { return needSuggestions; }
			set { needSuggestions = value; }
		}
		public virtual bool IsWordMisspelled() {
			isMisspelled = false;
			needSuggestions = false;
			try {
				if (!IsChecking()) {
					SetState(StrategyState.Start);
				}
			}
			finally {
				needSuggestions = true;
			}
			return isMisspelled;
		}
		private bool IsChecking() {
			return State != StrategyState.None;
		}
		public virtual UndoManagerBase UndoManager {
			get {
				if (undoManager == null)
					undoManager = CreateUndoManager();
				return undoManager;
			}
		}
		protected virtual UndoManagerBase CreateUndoManager() {
			return new UndoManagerBase(this);
		}
		public IIgnoreList Exceptions { get { return SpellChecker.GetIgnoreListCore(); } }
		public IIgnoreList IgnoreList { get { return SpellChecker.IgnoreList; } }
		public Dictionary<string, string> ChangeAllList { get { return SpellChecker.ChangeAllList; } }
		protected StateCache StateCache { get { return stateCache; } }
		protected virtual bool NeedSaveUndoInformation(SpellCheckOperation operation) {
			return operation != SpellCheckOperation.Undo &&
				operation != SpellCheckOperation.Cancel &&
				operation != SpellCheckOperation.Recheck &&
				!OperationsManager.IsUndo;
		}
		protected virtual void UpdateActiveError(SpellCheckOperation operation, string suggestion) {
			if (ActiveError != null) {
				ActiveError.Result = operation;
				ActiveError.Suggestion = suggestion;
			}
		}
		public virtual void DoSpellCheckOperation(SpellCheckOperation operation, string suggestion) {
			BeginSpellCheckOperation(operation);
			try {
				if (operation == SpellCheckOperation.Cancel && ParentStrategy != null)
					ParentStrategy.StopCheck();
				UpdateActiveError(operation, suggestion); 
				DoSpellCheckOperationCore(operation, suggestion);
			}
			finally {
				EndSpellCheckOperation(operation);
			}
			AfterSpellCheckOperationComplete(operation);
		}
		protected void AfterSpellCheckOperationComplete(SpellCheckOperation operation) {
			if (ShouldRaiseOperationCompletedEvent(operation))
				RaiseSpellCheckOperationCompleted(EventArgs.Empty);
		}
		protected void BeginSpellCheckOperation(SpellCheckOperation operation) {
			if (NeedSaveUndoInformation(operation))
				UndoManager.StartWriteComplexAction();
		}
		protected void EndSpellCheckOperation(SpellCheckOperation operation) {
			if (NeedSaveUndoInformation(operation))
				UndoManager.FinishWriteComplexAction();
		}
		internal void DoSpellCheckOperationCore(SpellCheckOperation operation, string suggestion) {
			OperationsManager.DoSpellCheckOperation(operation, suggestion, ActiveError);
		}
		protected virtual bool ShouldRaiseOperationCompletedEvent(SpellCheckOperation operation) {
			if (OperationsManager.IsUndo && operation != SpellCheckOperation.Undo)
				return false;
			return true;
		}
		public virtual void RaiseSpellCheckOperationCompleted(EventArgs e) {
			if (spellCheckOperationCompleted != null)
				spellCheckOperationCompleted(this, e);
		}
		public virtual ContainerSearchStrategyBase ParentStrategy {
			get { return this.parentStrategy; }
		}
		public SpellCheckerRulesController RulesController {
			get {
				return rulesController;
			}
		}
		protected virtual SpellCheckerRulesController CreateRulesController() {
			return new SpellCheckerRulesController(this, SpellChecker.OptionsSpelling);
		}
		public DictionaryHelper DictionaryHelper {
			get {
				return SpellChecker.DictionaryHelper;
			}
		}
		public TextOperationsManager OperationsManager {
			get {
				if (operationsManager == null)
					operationsManager = CreateOperationsManager();
				return operationsManager;
			}
		}
		protected virtual TextOperationsManager CreateOperationsManager() {
			return new TextOperationsManager(this);
		}
		protected virtual SpellCheckerStateBase CreateSpellCheckerState(StrategyState fState) {
			SpellCheckerStateBase spellCheckerState = StateCache.GetState(fState);
			if (spellCheckerState != null)
				return spellCheckerState;
			switch (fState) {
				case StrategyState.Checking:
					spellCheckerState = new CheckingSpellCheckerState(this);
					break;
				case StrategyState.FoundError:
					spellCheckerState = new ErrorFoundSpellCheckerState(this);
					break;
				case StrategyState.None:
					spellCheckerState = null;
					break;
				case StrategyState.Start:
					spellCheckerState = new StartCheckSpellCheckerState(this);
					break;
				case StrategyState.Stop:
					spellCheckerState = new StopCheckSpellCheckerState(this);
					break;
				case StrategyState.UserStop:
					spellCheckerState = new UserStopSpellCheckerState(this);
					break;
				default:
					throw new NotSupportedException("Unknown Strategy state : " + fState.ToString());
			}
			StateCache.Add(fState, spellCheckerState);
			return spellCheckerState;
		}
		public virtual SpellCheckErrorBase ActiveError {
			get { return activeError; }
			set {
				if (ActiveError == value)
					return;
				SpellCheckErrorBase oldError = ActiveError;
				activeError = value;
				if (activeError != null)
					isMisspelled = true;
				OnActiveErrorChanged(oldError);
			}
		}
		protected virtual void OnActiveErrorChanged(SpellCheckErrorBase oldError) {
		}
		protected internal void OnCheckedWordChanged() {
			UpdateLocalCulture(WordStartPosition, CurrentPosition);
			RaiseCheckedWordChanged();
		}
	}
	public class SimpleTextSearchStrategy : SearchStrategy {
		public SimpleTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController controller, Position startPosition, Position finishPosition)
			: base(spellChecker, controller) {
			StartPosition = startPosition;
			FinishPosition = finishPosition;
		}
		public SimpleTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController controller, ContainerSearchStrategyBase parentStrategy)
			: base(spellChecker, controller, parentStrategy) {
		}
		public SimpleTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController controller)
			: base(spellChecker, controller) {
		}
	}
	public class CheckingPartSpellCheckerStateBase : CheckingSpellCheckerState {
		public CheckingPartSpellCheckerStateBase(PartTextSearchStrategy strategy)
			: base(strategy) {
			InitializeStoredPosition();
		}
		public new PartTextSearchStrategy SearchStrategy { get { return base.SearchStrategy as PartTextSearchStrategy; } }
		public override Position StartPosition {
			get { return SearchStrategy.StoredStartPosition; }
			set { SearchStrategy.StoredStartPosition = value; }
		}
		public override Position FinishPosition {
			get { return SearchStrategy.StoredFinishPosition; }
			set { SearchStrategy.StoredFinishPosition = value; }
		}
		protected virtual void InitializeStoredPosition() { }
	}
	public class CheckingBeforeMainPartSpellCheckerState : CheckingPartSpellCheckerStateBase {
		public CheckingBeforeMainPartSpellCheckerState(PartTextSearchStrategy strategy)
			: base(strategy) {
			if (Position.IsGreater(CurrentPosition, SearchStrategy.StoredStartPosition) ||
				(Position.Equals(SearchStrategy.StoredStartPosition, SearchStrategy.StoredFinishPosition) && Position.Equals(CurrentPosition, SearchStrategy.StoredStartPosition)) || 
			(Position.IsLessOrEqual(CurrentPosition, SearchStrategy.StoredStartPosition) && !TextController.HasLetters(CurrentPosition, SearchStrategy.StoredFinishPosition))) 
				CurrentPosition = Position.Null;
		}
		protected override bool CheckPositionBeforeDoNextStep() {
			CalcPotentialNextPosition();
			return TextController.CanDoNextStep(CurrentPosition) && Position.IsLessOrEqual(PotentialNextPosition, FinishPosition);
		}
		public override Position StartPosition { get { return Position.Null; } }
		public override Position FinishPosition { get { return SearchStrategy.StoredStartPosition; } }
		protected override StrategyState GetStrategyStateWhenCannotDoNextStep() {
			return StrategyState.Stop;
		}
	}
	public class CheckingMainPartSpellCheckerState : CheckingPartSpellCheckerStateBase {
		bool shouldRaiseFinishCheckingMainPart = true;
		public CheckingMainPartSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		protected virtual bool ShouldRaiseFinishCheckingMainPart { get { return shouldRaiseFinishCheckingMainPart; } set { shouldRaiseFinishCheckingMainPart = value; } }
		protected override StrategyState GetStrategyStateWhenCannotDoNextStep() {
			if (ShouldRaiseFinishCheckingMainPart) {
				FinishCheckingMainPartEventArgs args = new FinishCheckingMainPartEventArgs(true);
				SpellChecker.OnFinishCheckingMainPart(args);
				ShouldRaiseFinishCheckingMainPart = false;
				if (!SpellChecker.IsOnFinishCheckingMainPartEventAssigned())
					return GetDefaultFinishCheckingMainPartResult(args);
				return args.NeedCheckRemainingPart ? StrategyState.CheckingAfterMainPart : StrategyState.Stop;
			}
			else
				return StrategyState.CheckingAfterMainPart;
		}
		protected virtual StrategyState GetDefaultFinishCheckingMainPartResult(FinishCheckingMainPartEventArgs e) {
			return StrategyState.CheckingAfterMainPart;
		}
	}
	public class CheckingAfterMainPartSpellCheckerState : CheckingPartSpellCheckerStateBase {
		public CheckingAfterMainPartSpellCheckerState(PartTextSearchStrategy strategy)
			: base(strategy) {
			FinishPosition = TextController.GetTextLength(TextController.Text);
		}
		protected override StrategyState GetStrategyStateWhenCannotDoNextStep() {
			return StrategyState.CheckingBeforeMainPart;
		}
		protected override bool CheckPositionBeforeDoNextStep() {
			return TextController.CanDoNextStep(CurrentPosition); 
		}
		protected override StrategyState GetStrategyStateWhenEmptyWord() {
			return StrategyState.CheckingBeforeMainPart;
		}
	}
	public class CheckingMainPartSilentSpellCheckerState : CheckingPartSpellCheckerStateBase {
		public CheckingMainPartSilentSpellCheckerState(PartTextSearchStrategy strategy) : base(strategy) { }
		protected override StrategyState GetStrategyStateWhenCannotDoNextStep() {
			return StrategyState.CheckingAfterMainPart;
		}
		protected override StrategyState GetStrategyStateWhenEmptyWord() {
			return StrategyState.CheckingAfterMainPart;
		}
	}
	public abstract class PartTextSearchStrategy : SearchStrategy {
		Position storedStartPosition;
		Position storedFinishPosition;
		CheckingPartSpellCheckerStateBase cachedCheckState = null;
		StrategyState stateAfterUndo = StrategyState.None;
		protected PartTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition, Position finishPosition)
			: base(spellChecker, textController) {
			StartPosition = startPosition;
			FinishPosition = finishPosition;
			InitializePositions();
		}
		protected virtual void InitializePositions() {
			if (!String.IsNullOrEmpty(Text)) {
				StartPosition = GetValidStartPosition(StartPosition);
				FinishPosition = GetValidFinishPosition(StartPosition, FinishPosition);
				StoredFinishPosition = FinishPosition;
				StoredStartPosition = StartPosition;
				CurrentPosition = StartPosition;
			}
		}
		public override Position CurrentPosition {
			get { return base.CurrentPosition; }
			set {
				base.CurrentPosition = value;
				if (OperationsManager.IsUndo)
					StateAfterUndo = GetStateByPosition(CurrentPosition);
			}
		}
		public virtual Position StoredStartPosition { get { return this.storedStartPosition; } set { this.storedStartPosition = value; } }
		public virtual Position StoredFinishPosition { get { return this.storedFinishPosition; } set { this.storedFinishPosition = value; } }
		protected virtual CheckingPartSpellCheckerStateBase CachedCheckingState {
			get { return cachedCheckState; }
			set { cachedCheckState = value; }
		}
		protected virtual StrategyState StateAfterUndo {
			get { return stateAfterUndo; }
			set { stateAfterUndo = value; }
		}
		protected virtual Position GetValidStartPosition(Position pos) {
			Position wordStart = TextController.GetWordStartPosition(pos);
			if (Position.Equals(wordStart, pos))
				return pos;
			Position prevPos = TextController.GetPrevPosition(pos);
			Position wordEnd = TextController.GetNextPosition(prevPos);
			if (Position.IsGreaterOrEqual(wordEnd, pos))
				return wordStart;
			Position nextPos = TextController.GetNextPosition(wordEnd);
			return TextController.GetWordStartPosition(nextPos);
		}
		protected virtual Position GetValidFinishPosition(Position start, Position pos) {
			Position result = start;
			while (true) {
				result = TextController.GetNextPosition(result);
				if (Position.IsGreater(result, pos)) {
					Position wordStartPosition = TextController.GetWordStartPosition(result);
					if (Position.IsGreaterOrEqual(wordStartPosition, pos)) {
						result = TextController.GetPrevPosition(result);
						break;
					}
					else
						break;
				}
				else
					if (Position.Equals(result, pos) || (Position.IsLess(result, pos) && !TextController.HasLetters(result, pos)))
						break;
			}
			return result;
		}
		protected override void OnTextChanged() {
			base.OnTextChanged();
			InitializePositions();
		}
		protected override TextOperationsManager CreateOperationsManager() {
			return new PartTextOperationsManager(this);
		}
		protected virtual CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new CheckingMainPartSpellCheckerState(this);
		}
		protected virtual CheckingPartSpellCheckerStateBase CreateCheckingBeforeMainPartSpellCheckerState() {
			return new CheckingBeforeMainPartSpellCheckerState(this);
		}
		protected virtual CheckingPartSpellCheckerStateBase CreateCheckingAfterMainPartSpellCheckerState() {
			return new CheckingAfterMainPartSpellCheckerState(this);
		}
		protected override SpellCheckerStateBase CreateSpellCheckerState(StrategyState fState) {
			if (StateAfterUndo != StrategyState.None) {
				fState = StateAfterUndo;
				StateAfterUndo = StrategyState.None;
			}
			CheckingPartSpellCheckerStateBase result = this.StateCache.GetState(fState) as CheckingPartSpellCheckerStateBase;
			if (result != null &&
				(fState == StrategyState.CheckingMainPart || fState == StrategyState.CheckingAfterMainPart)) { 
				CachedCheckingState = result;
				return CachedCheckingState;
			}
			switch (fState) {
				case StrategyState.Checking:
					return GetStateWhenChecking();
				case StrategyState.CheckingMainPart:
					CachedCheckingState = CreateCheckingMainPartSpellCheckerState();
					StateCache.Add(fState, CachedCheckingState);
					return CachedCheckingState;
				case StrategyState.CheckingAfterMainPart:
					CachedCheckingState = CreateCheckingAfterMainPartSpellCheckerState();
					StateCache.Add(fState, CachedCheckingState);
					return CachedCheckingState;
				case StrategyState.CheckingBeforeMainPart:
					CachedCheckingState = CreateCheckingBeforeMainPartSpellCheckerState();
					return CachedCheckingState;
				default:
					return base.CreateSpellCheckerState(fState);
			}
		}
		protected virtual SpellCheckerStateBase GetStateWhenChecking() {
			if (CachedCheckingState == null) {
				CachedCheckingState = CreateCheckingMainPartSpellCheckerState();
				StateCache.Add(StrategyState.CheckingMainPart, CachedCheckingState);
			}
			return CachedCheckingState;
		}
		protected virtual bool IsCheckingMainPartState(SpellCheckerStateBase fState) {
			return fState is CheckingMainPartSpellCheckerState;
		}
		protected virtual bool IsCheckingAfterMainPartState(SpellCheckerStateBase fState) {
			return fState is CheckingAfterMainPartSpellCheckerState;
		}
		protected virtual bool IsCheckingBeforeMainPartState(SpellCheckerStateBase fState) {
			return fState is CheckingBeforeMainPartSpellCheckerState;
		}
		protected virtual StrategyState GetStateByPosition(Position pos) {
			if (Position.IsLess(pos, StoredStartPosition)) 
				return StrategyState.CheckingBeforeMainPart;
			else
				if (Position.IsGreaterOrEqual(pos, StoredFinishPosition))
					return StrategyState.CheckingAfterMainPart;
			return StrategyState.CheckingMainPart;
		}
	}
	public class PartSimpleTextSearchStrategy : PartTextSearchStrategy {
		public PartSimpleTextSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition, Position finishPosition)
			: base(spellChecker, textController, startPosition, finishPosition) { }
	}
	public class PartSimpleTextSilentSearchStrategy : PartSimpleTextSearchStrategy {
		public PartSimpleTextSilentSearchStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController, Position startPosition)
			: base(spellChecker, textController, startPosition, textController.GetTextLength(textController.Text)) { }
		protected override CheckingPartSpellCheckerStateBase CreateCheckingMainPartSpellCheckerState() {
			return new CheckingMainPartSilentSpellCheckerState(this);
		}
		protected override Position GetValidFinishPosition(Position start, Position pos) {
			return TextController.GetTextLength(TextController.Text);
		}
	}
	public abstract class ContainerSearchStrategyBase : IDisposable {
		SpellCheckerBase spellChecker = null;
		object container = null;
		object currentControl;
		bool isStoped = false;
		ControlBrowserBase controlBrowser = null;
		protected ContainerSearchStrategyBase(SpellCheckerBase spellChecker, object container) {
			this.spellChecker = spellChecker;
			this.container = container;
			this.controlBrowser = CreateControlBrowser(container);
		}
		public void Dispose() {
			if (this.controlBrowser != null) {
				this.controlBrowser.Dispose();
				this.controlBrowser = null;
			}
			this.spellChecker = null;
			this.container = null;
			this.currentControl = null;
		}
		protected ControlBrowserBase ControlBrowser { get { return controlBrowser; } }
		protected internal virtual ControlBrowserBase CreateControlBrowser(object control) {
			return null;
		}
		#region ISpellCheckStrategyContainer Members
		public virtual object GetNextObject(object currentObject) {
			object nextObject = ControlBrowser.GetNextObject(currentObject);
			while (!CanCheckControl(nextObject) && nextObject != null)
				nextObject = GetNextObject(nextObject);
			return nextObject;
		}
		public virtual bool IsLastObject(object currentObject) {
			if (ControlBrowser.IsLastObject(currentObject))
				return true;
			ControlBrowserBase oldControlBrowser = this.controlBrowser;
			this.controlBrowser = ControlBrowser.Clone();
			try {
				return GetNextObject(currentObject) == null;
			}
			finally {
				this.controlBrowser = oldControlBrowser;
			}
		}
		protected abstract bool CanCheckControl(object currentObject);
		public virtual object GetFirstObject() {
			object nextControl = ControlBrowser.GetFirstObject();
			while (!CanCheckControl(nextControl) && nextControl != null)
				nextControl = GetNextObject(nextControl);
			return nextControl;
		}
		public virtual void FinishControlChecking() {
			if (!IsLastObject(CurrentControl)) {
				CurrentControl = GetNextObject(CurrentControl);
				if (CurrentControl != null && !IsStoped)
					CheckCurrentControl();
			}
		}
		public void StopCheck() {
			isStoped = true;
		}
		protected virtual bool IsStoped { get { return isStoped; } set { isStoped = value; } }
		protected virtual void CheckCurrentControl() {
			SpellChecker.Check(CurrentControl, this);
		}
		#endregion
		public virtual SpellCheckerBase SpellChecker { get { return this.spellChecker; } }
		public virtual object Container { get { return container; } }
		public virtual void Check() {
			CurrentControl = GetFirstObject();
			CheckCurrentControl();
		}
		protected virtual object CurrentControl {
			get { return currentControl; }
			set { currentControl = value; }
		}
	}
	public abstract class ControlBrowserBase : IDisposable {
		object container = null;
		ControlBrowserBase childControlBrowser = null;
		protected ControlBrowserBase(object container) {
			this.container = container;
		}
		public object Container { get { return container; } }
		protected ControlBrowserBase ChildControlBrowser { get { return childControlBrowser; } set { childControlBrowser = value; } }
		public void Dispose() {
			container = null;
			DisposeChildControlBrowser();
		}
		protected virtual bool IsObjectContainer(object container) {
			return false;
		}
		protected virtual ControlBrowserBase CreateChildControlBrowser(object control) {
			return null;
		}
		protected virtual void DisposeChildControlBrowser() {
			if (ChildControlBrowser != null) {
				ChildControlBrowser.DisposeChildControlBrowser();
				ChildControlBrowser.Dispose();
				childControlBrowser = null;
			}
		}
		public virtual object GetNextObject(object control) {
			object nextObject = null;
			if (control != null && ChildControlBrowser != null && ChildControlBrowser.Contains(control)) {
				nextObject = ChildControlBrowser.GetNextObject(control);
				if (nextObject == null) {
					control = ChildControlBrowser.Container;
					DisposeChildControlBrowser();
				}
				else
					return nextObject;
			}
			if (control == null)
				nextObject = GetFirstObject();
			else
				nextObject = GetNextObjectCore(control);
			if (nextObject != null && IsObjectContainer(nextObject)) {
				DisposeChildControlBrowser();
				ChildControlBrowser = CreateChildControlBrowser(nextObject);
				return ChildControlBrowser.GetFirstObject();
			}
			return nextObject;
		}
		public virtual object GetFirstObject() {
			if (ChildControlBrowser != null)
				return ChildControlBrowser.GetFirstObject();
			object firstObject = GetFirstObjectCore();
			if (IsObjectContainer(firstObject)) {
				ChildControlBrowser = CreateChildControlBrowser(firstObject);
				return ChildControlBrowser.GetFirstObject();
			}
			return firstObject;
		}
		public virtual bool IsLastObject(object control) {
			if (control == null)
				return GetFirstObject() != null;
			if (ChildControlBrowser != null && ChildControlBrowser.Contains(control)) {
				if (ChildControlBrowser.IsLastObject(control))
					return IsLastObjectCore(ChildControlBrowser.Container);
				else
					return false;
			}
			return IsLastObjectCore(control);
		}
		internal ControlBrowserBase Clone() {
			ControlBrowserBase clone = CreateInstance(this.container);
			if (ChildControlBrowser != null)
				clone.ChildControlBrowser = ChildControlBrowser.Clone();
			return clone;
		}
		#region Abstract
		protected abstract object GetNextObjectCore(object control);
		protected abstract object GetFirstObjectCore();
		protected abstract bool IsLastObjectCore(object control);
		protected abstract bool Contains(object control);
		protected abstract ControlBrowserBase CreateInstance(object container);
		#endregion
	}
	public class RichEditCheckStrategy : SearchStrategy {
		public RichEditCheckStrategy(SpellCheckerBase spellChecker, ISpellCheckTextController textController)
			: base(spellChecker, textController) {
		}
		public virtual ICheckSpellingResult CheckText() {
			NeedSuggestions = false;
			try {
				Check();
				if (ActiveError != null) {
					CheckSpellingResultType type = (ActiveError is RepeatedWordError) ? CheckSpellingResultType.Repeating : CheckSpellingResultType.Misspelling;
					int startPos = GetActualIntPosition(ActiveError.StartPosition);
					int endPos = GetActualIntPosition(ActiveError.FinishPosition);
					return new CheckSpellingResult(Text, startPos, endPos - startPos, type);
				}
				else
					return new CheckSpellingResult(Text, 0, 0, CheckSpellingResultType.Success);
			}
			finally {
				NeedSuggestions = true;
			}
		}
		int GetActualIntPosition(Position position) {
			IntPosition intPosition = position as IntPosition;
			if (intPosition == null)
				return 0;
			return intPosition.ActualIntPosition;
		}
		protected internal override void OnAfterCheck(StopCheckingReason reason) {
			RaiseAfterCheck();
		}
		protected override bool ShouldClearLists() {
			return false;
		}
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public static class BoolDefaultBooleanConverter {
		public static DefaultBoolean ConvertBoolToDefaultBoolean(bool value) {
			return value ? DefaultBoolean.True : DefaultBoolean.False;
		}
	}
}
