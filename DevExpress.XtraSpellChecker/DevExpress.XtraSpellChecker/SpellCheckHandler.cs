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
using System.Text;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Controls;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Localization;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
namespace DevExpress.XtraSpellChecker {
	#region Routers
	public abstract class SpellCheckClientRouterBase : IDisposable {
		readonly SearchStrategy searchStrategy;
		HandlerCache cache = new HandlerCache();
		protected SpellCheckClientRouterBase(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
			SubscribeToEvents();
		}
		public virtual SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public virtual SpellCheckerBase SpellChecker { get { return SearchStrategy.SpellChecker; } }
		protected HandlerCache Cache { get { return cache; } }
		protected virtual void SubscribeToEvents() {
			SearchStrategy.StrategyStateChanged += OnStrategyStateChanging;
		}
		protected virtual void UnsubscribeFromEvents() {
			SearchStrategy.StrategyStateChanged -= OnStrategyStateChanging;
		}
		void OnStrategyStateChanging(object sender, StrategyStateChangedEventArgs e) {
			OnStrategyStateChanging(e);
		}
		protected virtual void OnStrategyStateChanging(StrategyStateChangedEventArgs e) {
			HandleStateVisually(e.State);
		}
		protected virtual void HandleStateVisually(StrategyState state) {
			SpellCheckHandlerBase handler = Cache[state];
			if (handler == null)
				handler = CreateHandlerForState(state);
			if (handler != null)
				handler.HandleState();
		}
		protected virtual SpellCheckHandlerBase CreateHandlerForState(StrategyState state) {
			SpellCheckHandlerBase handler = null;
			switch (state) {
				case StrategyState.WaitForClientHandlingError:
					handler = CreateHandlerForFoundErrorState();
					break;
				case StrategyState.Stop:
					handler = CreateHandlerForStopState();
					break;
				case StrategyState.UserStop:
					handler = CreateHandlerForUserStopState();
					break;
			}
			if (handler != null)
				Cache.Add(state, handler);
			return handler;
		}
		protected abstract SpellCheckHandlerBase CreateHandlerForUserStopState();
		protected abstract SpellCheckHandlerBase CreateHandlerForStopState();
		protected abstract SpellCheckHandlerBase CreateHandlerForFoundErrorState();
		public void Dispose() {
			UnsubscribeFromEvents();
			if (cache != null) {
				Cache.ForEach(delegate(SpellCheckHandlerBase item) {
					item.Dispose();
				});
				Cache.Clear();
				this.cache = null;
			}
		}
	}
	public class SpellCheckMSOutlookLikeClientRouter : SpellCheckClientRouterBase {
		public SpellCheckMSOutlookLikeClientRouter(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override SpellCheckHandlerBase CreateHandlerForUserStopState() {
			return new UserStopStateHandler(SearchStrategy);
		}
		protected override SpellCheckHandlerBase CreateHandlerForStopState() {
			return new StopStateHandler(SearchStrategy);
		}
		protected override SpellCheckHandlerBase CreateHandlerForFoundErrorState() {
			return new FoundErrorStateHandler(SearchStrategy);
		}
	}
	public class SpellCheckMSWordLikeClientRouter : SpellCheckMSOutlookLikeClientRouter {
		public SpellCheckMSWordLikeClientRouter(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected override SpellCheckHandlerBase CreateHandlerForFoundErrorState() {
			return new MSWordFoundErrorStateHandler(SearchStrategy);
		}
	}
	#endregion
	#region Handlers
	public abstract class SpellCheckHandlerBase : IDisposable {
		readonly SearchStrategy searchStrategy;
		protected SpellCheckHandlerBase(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
		}
		public virtual SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public virtual SpellChecker SpellChecker { get { return SearchStrategy.SpellChecker as SpellChecker; } }
		public virtual SpellingFormsManager FormsManager { get { return SpellChecker.FormsManager; } }
		public abstract void HandleState();
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		#endregion
	}
	public class FoundErrorStateHandler : SpellCheckHandlerBase {
		bool isFirstTimeAdjusted = false;
		public FoundErrorStateHandler(SearchStrategy searchStrategy)
			: base(searchStrategy) {
		}
		public SpellingFormHelper SpellCheckFormHelper {
			get { return FormsManager.SpellingFormHelper; }
		}
		protected virtual ISpellCheckTextControlController TextController {
			get { return SearchStrategy.TextController as ISpellCheckTextControlController; }
		}
		protected virtual void AdjustControlFirstTime() {
			TextController.ShowSelection();  
			isFirstTimeAdjusted = true;
		}
		protected virtual void AdjustControlRegularly() {
			TextController.Select(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition);
			TextController.ScrollToCaretPos();
		}
		protected internal virtual void SubscribeToSpellCheckFormEvent() {
			SpellingFormBase spellCheckForm = FormsManager.SpellCheckForm;
			System.Diagnostics.Debug.Assert(spellCheckForm != null);
			if (spellCheckForm != null) {
				spellCheckForm.SpellCheckFormResultChanged += new SpellingFormResultChangedEventHandler(SpellCheckForm_SpellCheckFormResultChanged);
				System.Diagnostics.Debug.WriteLine("->SubscribeToSpellCheckFormEvent");
			}
		}
		protected internal virtual void UnsubscribeFromSpellCheckFormEvent() {
			SpellingFormBase spellCheckForm = FormsManager.SpellCheckForm;
			System.Diagnostics.Debug.Assert(spellCheckForm != null);
			if (spellCheckForm != null) {
				spellCheckForm.SpellCheckFormResultChanged -= new SpellingFormResultChangedEventHandler(SpellCheckForm_SpellCheckFormResultChanged);
				System.Diagnostics.Debug.WriteLine("->UnsubscribeFromSpellCheckFormEvent");
			}
		}
		protected virtual void OnClientHandlingError(SpellCheckErrorBase error) {
			if (!isFirstTimeAdjusted) {
				AdjustControlFirstTime();
			}
			SpellCheckFormHelper.OnNewErrorFound(error);
			AdjustControlRegularly();
			SubscribeToSpellCheckFormEvent();
			FormsManager.ShowSpellCheckForm();
		}
		void SpellCheckForm_SpellCheckFormResultChanged(object sender, SpellingFormResultChangedEventArgs e) {
			OnSpellCheckFormResultChanged(e);
		}
		protected virtual void OnSpellCheckFormResultChanged(SpellingFormResultChangedEventArgs e) {
			UnsubscribeFromSpellCheckFormEvent();
			SearchStrategy.DoSpellCheckOperation(e.Operation, e.Suggestion);
			SearchStrategy.SetState(StrategyState.Checking);
		}
		public override void HandleState() {
			OnClientHandlingError(SearchStrategy.ActiveError);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
	}
	public class MSWordFoundErrorStateHandler : FoundErrorStateHandler {
		bool isSelectedWordModified = false;
		bool isOtherWordModified = false;
		Position otherWordPosition = Position.Undefined;
		Position currentWordStartPosition = Position.Undefined;
		Position currentWordFinishPosition = Position.Undefined;
		List<Action> defferedActions = new List<Action>();
		public MSWordFoundErrorStateHandler(SearchStrategy searchStrategy)
			: base(searchStrategy) {
		}
		protected ISpellCheckMSWordTextControlController TextControlController { get { return SearchStrategy.TextController as ISpellCheckMSWordTextControlController; } }
		protected virtual SpellingWordStyleForm SpellCheckForm { get { return FormsManager.SpellCheckForm as SpellingWordStyleForm; } }
		protected virtual new SpellingWordStyleFormHelper SpellCheckFormHelper { get { return base.SpellCheckFormHelper as SpellingWordStyleFormHelper; } }
		protected internal virtual Position CurrentWordStartPosition { get { return currentWordStartPosition; } set { currentWordStartPosition = value; } }
		protected internal virtual Position CurrentWordFinishPosition { get { return currentWordFinishPosition; } set { currentWordFinishPosition = value; } }
		protected internal virtual bool IsSelectedWordModified {
			get { return isSelectedWordModified; }
			set {
				isSelectedWordModified = value;
			}
		}
		protected internal virtual bool IsOtherWordModified {
			get { return isOtherWordModified; }
			set {
				isOtherWordModified = value;
				OnIsOtherWordModified();
			}
		}
		protected internal override void SubscribeToSpellCheckFormEvent() {
			base.SubscribeToSpellCheckFormEvent();
			SubscribeToTextControlControllerEvent();
		}
		void SubscribeToTextControlControllerEvent() {
			TextControlController.MemoSelectionStartChanged += new SelectionStartChangedEventHandler(TextControlController_MemoSelectionStartChanged);
			TextControlController.MemoTextChanged += new TextChangedEventHandler(TextControlController_MemoTextChanged);
			TextControlController.MemoKeyDown += new KeyEventHandler(TextControlController_MemoKeyDown);
		}
		protected internal override void UnsubscribeFromSpellCheckFormEvent() {
			UnsubscribeToTextControlControllerEvent();
			base.UnsubscribeFromSpellCheckFormEvent();
		}
		void UnsubscribeToTextControlControllerEvent() {
			TextControlController.MemoSelectionStartChanged -= new SelectionStartChangedEventHandler(TextControlController_MemoSelectionStartChanged);
			TextControlController.MemoTextChanged -= new TextChangedEventHandler(TextControlController_MemoTextChanged);
			TextControlController.MemoKeyDown -= new KeyEventHandler(TextControlController_MemoKeyDown);
		}
		void TextControlController_MemoTextChanged(object sender, TextChangedEventArgs e) {
			OnMemoTextChanged(e);
		}
		void TextControlController_MemoSelectionStartChanged(object sender, SelectionStartChangedEventArgs e) {
			OnMemoSelectionStartChanged(e);
		}
		void TextControlController_MemoKeyDown(object sender, KeyEventArgs e) {
			OnMemoKeyDown(e);
		}
		protected override void AdjustControlRegularly() {
			IsOtherWordModified = false;
			IsSelectedWordModified = false;
			otherWordPosition = Position.Undefined;
			Position wordStart = SearchStrategy.WordStartPosition;
			Position wordEnd = SearchStrategy.CurrentPosition;
			CustomSpellCheckMemoEdit formEditControl = (FormsManager.SpellCheckForm as SpellingWordStyleForm).mmNotInDictionary;
			Position sentenceStart = SearchStrategy.TextController.GetSentenceStartPosition(wordStart);
			Position sentenceEnd = SearchStrategy.TextController.GetSentenceEndPosition(wordEnd);
			formEditControl.Text = String.Empty;
			formEditControl.Text = TextController.GetWord(sentenceStart, sentenceEnd);
			if (FormsManager.SpellCheckForm.Visible)
				SetSelection(formEditControl, wordStart, wordEnd, sentenceStart);
			else {
				EventHandler handler = null;
				handler = (s, e) => {
					FormsManager.SpellCheckForm.VisibleChanged -= handler;
					SetSelection(formEditControl, wordStart, wordEnd, sentenceStart);
				};
				FormsManager.SpellCheckForm.VisibleChanged += handler;
			}
		}
		void SetSelection(CustomSpellCheckMemoEdit formEditControl, Position wordStart, Position wordEnd, Position sentenceStart) {
			formEditControl.SelectionStart = TextController.GetWord(sentenceStart, wordStart).Length;
			formEditControl.SelectionLength = TextController.GetWord(wordStart, wordEnd).Length;
			TextController.Select(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition);
			TextController.ScrollToCaretPos();
			CurrentWordStartPosition = TextControlController.FormEditorSelectionStart;
			CurrentWordFinishPosition = TextControlController.FormEditorSelectionFinish;
		}
		protected Position OtherWordPosition {
			get { return otherWordPosition; }
			set { SetOtherWordPosition(value); }
		}
		protected virtual void SetOtherWordPosition(Position value) {
			if (OtherWordPosition == value) return;
			if (value == Position.Undefined)
				otherWordPosition = value;
			if (OtherWordPosition == Position.Undefined || Position.IsLess(value, OtherWordPosition) || Position.IsLess(value, SearchStrategy.WordStartPosition))
				otherWordPosition = value;
		}
		#region KeyInputProcessing
		protected virtual bool IsPasteKeys(KeyEventArgs e) {
			return (e.Control && e.KeyCode == Keys.V) || (e.Shift && e.KeyCode == Keys.Insert);
		}
		protected virtual bool IsUndoRedoKeys(KeyEventArgs e) {
			return (e.Control && e.KeyCode == Keys.Z) || (e.Control && e.KeyCode == Keys.Y);
		}
		protected internal virtual void OnMemoKeyDown(KeyEventArgs e) {
			this.defferedActions.Clear();
			if (IsOtherWordModified)
				return;
			if (IsPasteKeys(e) || IsUndoRedoKeys(e))
				ProcessComplexChange();
			else if (e.KeyCode == Keys.Return) {
				e.Handled = true;
				FormsManager.SpellCheckForm.AcceptButton.PerformClick();
			}
			else if (e.KeyData == Keys.Delete)
				ProcessDeleteKey();
			else if (e.KeyData == Keys.Back)
				ProcessBackSpace();
			else
				ProcessSimpleChar();
		}
		protected virtual void ProcessDeleteKey() {
			Position selectionStart = TextControlController.FormEditorSelectionStart.Clone();
			Position selectionFinish = TextControlController.FormEditorSelectionFinish.Clone();
			if (Position.Equals(selectionStart, selectionFinish)) {
				Position textLength = TextControlController.GetTextLength(TextControlController.Text);
				selectionFinish++;
				if (Position.IsGreaterOrEqual(selectionFinish, textLength))
					return;
			}
			ProcessKeyPressCore(selectionStart, selectionFinish);
		}
		protected virtual void ProcessBackSpace() {
			Position selectionStart = TextControlController.FormEditorSelectionStart.Clone();
			Position selectionFinish = TextControlController.FormEditorSelectionFinish.Clone();
			if (Position.Equals(selectionStart, selectionFinish)) {
				if (selectionStart.IsZero)
					return;
				selectionStart--;
			}
			ProcessKeyPressCore(selectionStart, selectionFinish);
		}
		void ProcessKeyPressCore(Position selectionStart, Position selectionFinish) {
			IsSelectedWordModified = IsSelectionInCurrentWord(selectionStart, selectionFinish);
			IsOtherWordModified = !IsSelectedWordModified;
			CorrectWordStartPosition(selectionStart, selectionFinish);
			CorrentWordFinishPosition(selectionStart, selectionFinish);
		}
		bool IsPositionInRange(Position start, Position end, Position position) {
			return Position.IsGreaterOrEqual(position, start) && Position.IsLessOrEqual(position, end);
		}
		bool IsSelectionInCurrentWord(Position selectionStart, Position selectionFinish) {
			return IsPositionInRange(CurrentWordStartPosition, CurrentWordFinishPosition, selectionStart) &&
				IsPositionInRange(CurrentWordStartPosition, CurrentWordFinishPosition, selectionFinish);
		}
		void CorrentWordFinishPosition(Position selectionStart, Position selectionFinish) {
			if (Position.IsLess(selectionFinish, CurrentWordFinishPosition)) {
				Position selectionLength = selectionFinish - selectionStart;
				CurrentWordFinishPosition = CurrentWordFinishPosition - selectionLength;
			}
			else if (Position.IsLess(selectionStart, CurrentWordFinishPosition))
				CurrentWordFinishPosition = selectionStart.Clone();
		}
		void CorrectWordStartPosition(Position selectionStart, Position selectionFinish) {
			if (Position.IsLess(selectionStart, CurrentWordStartPosition))
				CurrentWordStartPosition = selectionStart.Clone();
			else if (Position.IsLess(selectionFinish, CurrentWordStartPosition)) {
				Position selectionLength = selectionFinish - selectionStart;
				CurrentWordStartPosition = CurrentWordStartPosition - selectionLength;
			}
		}
		protected virtual void ProcessSimpleChar() {
			Position selectionStart = TextControlController.FormEditorSelectionStart.Clone();
			Position selectionFinish = TextControlController.FormEditorSelectionFinish.Clone();
			Action action = new Action(delegate() {
				ProcessKeyPressCore(selectionStart, selectionFinish);
				if (Position.IsLess(selectionStart, CurrentWordStartPosition)) {
					CurrentWordStartPosition++;
					CurrentWordFinishPosition++;
				}
				else if (Position.IsLessOrEqual(selectionStart, CurrentWordFinishPosition))
					CurrentWordFinishPosition++;
			});
			this.defferedActions.Add(action);
		}
		void ProcessComplexChange() {
			Action action = new Action(delegate() {
				IsOtherWordModified = true;
				IsSelectedWordModified = false;
			});
			this.defferedActions.Add(action);
		}
		#endregion
		protected virtual void OnMemoSelectionStartChanged(SelectionStartChangedEventArgs e) {
			if (e.Reason == SelectionChangeReason.Keyboard && e.Key != Keys.Return)
				OnMemoSelectionStartChangedByKeyboard(e.SelectionStart);
			else
				OnMemoSelectionStartChangedByMouse(e.SelectionStart);
		}
		protected virtual void OnMemoSelectionStartChangedByKeyboard(Position p) {
			SpellCheckFormHelper.OnSelectionStartChangedByKeyboard();
		}
		protected virtual void OnMemoSelectionStartChangedByMouse(Position p) { }
		protected internal virtual void OnMemoTextChanged(TextChangedEventArgs e) {
			foreach (Action action in this.defferedActions)
				action();
			this.defferedActions.Clear();
			SpellCheckFormHelper.OnMemoTextChanged();
		}
		protected virtual void OnIsOtherWordModified() {
			if (IsOtherWordModified) {
				OtherWordPosition = TextControlController.CaretPosition;
				SearchStrategy.UndoManager.Reset();
				if (SearchStrategy is PartSimpleTextSilentSearchStrategy)
					(SearchStrategy as PartSimpleTextSilentSearchStrategy).StoredStartPosition = Position.Null; 
			}
		}
		protected virtual void OnIsSelectedWordModified() {
		}
		protected override void AdjustControlFirstTime() {
			CustomSpellCheckMemoEdit editControl = (FormsManager.SpellCheckForm as SpellingWordStyleForm).mmNotInDictionary;
			editControl.Text = SearchStrategy.Text;
			ISpellCheckTextControlController originalController = SearchStrategy.TextController as ISpellCheckTextControlController;
			SearchStrategy.TextController = new SimpleTextEditMSWordTextController(editControl, originalController);
			SearchStrategy.CultureProvider = originalController as ISupportMultiCulture;
			base.AdjustControlFirstTime();
		}
		protected override void OnSpellCheckFormResultChanged(SpellingFormResultChangedEventArgs e) {
			if ((e.Operation == SpellCheckOperation.Change || e.Operation == SpellCheckOperation.ChangeAll) && (IsOtherWordModified || IsSelectedWordModified)) {
				UnsubscribeFromSpellCheckFormEvent();
				DoChangeOperation(e);
				SearchStrategy.SetState(StrategyState.Checking);
			}
			else
				base.OnSpellCheckFormResultChanged(e);
		}
		protected virtual void DoChangeOperation(SpellingFormResultChangedEventArgs e) {
			if (IsOtherWordModified) 
				DoChangeOperationWhenOtherWordModified(e);
			else
				if (IsSelectedWordModified)
					DoChangeOperationWhenSelectedWordModified(e);
		}
		protected virtual void DoChangeOperationWhenOtherWordModified(SpellingFormResultChangedEventArgs e) {
			SearchStrategy.ActiveError.StartPosition = TextControlController.GetSentenceStartPosition(TextControlController.SelectionStart);
			SearchStrategy.ActiveError.FinishPosition = TextControlController.GetSentenceEndPosition(TextControlController.SelectionFinish);
			SearchStrategy.WordStartPosition = SearchStrategy.ActiveError.StartPosition;
			SearchStrategy.CurrentPosition = SearchStrategy.ActiveError.FinishPosition;
			SearchStrategy.DoSpellCheckOperation(e.Operation, TextControlController.EditControlText);
			SearchStrategy.CurrentPosition = Position.Null;
			SearchStrategy.DoSpellCheckOperation(SpellCheckOperation.Recheck, e.Suggestion);
		}
		protected virtual void DoChangeOperationWhenSelectedWordModified(SpellingFormResultChangedEventArgs e) {
			string word = TextControlController.GetFormEditorText(CurrentWordStartPosition, CurrentWordFinishPosition);
			if (String.IsNullOrEmpty(word) || SearchStrategy.DictionaryHelper.FindWord(word, SearchStrategy.ActualCulture))
				SearchStrategy.DoSpellCheckOperation(e.Operation, word);
			else
				if (CanUseSuggestion())
					SearchStrategy.DoSpellCheckOperation(e.Operation, word);
				else {
					SearchStrategy.DoSpellCheckOperation(SpellCheckOperation.Recheck, word);
				}
		}
		protected virtual void SynchronizeTextControllerText() {
			TextControlController.UpdateText();
		}
		protected virtual bool CanUseSuggestion() {
			return DevExpress.XtraEditors.XtraMessageBox.Show(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MsgCanUseCurrentWord),
			Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
		}
	}
	public class StopStateHandler : SpellCheckHandlerBase {
		public StopStateHandler(SearchStrategy searchStrategy) : base(searchStrategy) { }
		protected virtual bool ShouldHideSpellCheckForm() {
			if (SearchStrategy.ParentStrategy == null)
				return true;
			else
				return SearchStrategy.ParentStrategy != null && SearchStrategy.ParentStrategy.IsLastObject(SpellChecker.CheckedControl);
		}
		public override void HandleState() {
			if (ShouldHideSpellCheckForm()) {
				FormsManager.HideSpellCheckForm(SpellChecker.SpellingFormType); 
				ShowFinishMessagebox();
			}
		}
		protected virtual void ShowFinishMessagebox() {
			FormShowingEventArgs e = new FormShowingEventArgs();
			SpellChecker.OnCheckCompleteFormShowing(e);
			if (!e.Handled)
				XtraMessageBox.Show(SpellChecker.FormsManager.DialogOwner, SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MsgBoxFinishCheck), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}
	}
	public class UserStopStateHandler : SpellCheckHandlerBase {
		public UserStopStateHandler(SearchStrategy searchStrategy) : base(searchStrategy) { }
		public override void HandleState() {
			FormsManager.HideSpellCheckForm(SpellChecker.SpellingFormType);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpellChecker.Forms {
	public abstract class SpellingFormHelper : IDisposable {
		SpellingFormBase spellCheckForm;
		protected SpellingFormHelper(SpellingFormBase spellCheckForm) {
			this.spellCheckForm = spellCheckForm;
			SubscribeToEvents();
		}
		public virtual SpellingFormBase SpellCheckForm { get { return this.spellCheckForm; } }
		public virtual SearchStrategy SearchStrategy { get { return SpellCheckForm.SpellChecker.SearchStrategy; } }
		public virtual void PopulateFormSuggestions(SuggestionCollection suggestions) {
			SpellCheckForm.Suggestions.Clear();
			if (suggestions == null)
				return;
			int count = suggestions.Count;
			for (int i = 0; i < count; i++)
				SpellCheckForm.Suggestions.Add(suggestions[i].Suggestion);
		}
		public abstract void OnNewErrorFound(SpellCheckErrorBase error);
		protected virtual void SubscribeToEvents() { }
		protected virtual void UnSubscribeFromEvents() { }
		protected virtual bool CustomDictionaryExits() {
			return SearchStrategy.DictionaryHelper.GetCustomDictionary(SearchStrategy.ActualCulture) != null;
		}
		protected virtual bool IsRepeatedWordError(SpellCheckErrorBase error) {
			return error.RulesController.IsRepeatedWordError(error);
		}
		protected virtual bool IsNotInDictionaryWordError(SpellCheckErrorBase error) {
			return error.RulesController.IsNotInDictionaryWordError(error);
		}
		public void Dispose() {
			UnSubscribeFromEvents();
			spellCheckForm = null;
		}
	}
	public class SpellingOutlookStyleFormHelper : SpellingFormHelper {
		public SpellingOutlookStyleFormHelper(SpellingOutlookStyleForm spellCheckForm) : base(spellCheckForm) { }
		public virtual new SpellingOutlookStyleForm SpellCheckForm { get { return base.SpellCheckForm as SpellingOutlookStyleForm; } }
		public override void PopulateFormSuggestions(SuggestionCollection suggestions) {
			base.PopulateFormSuggestions(suggestions);
			SpellCheckForm.lbcSuggestions.Items.BeginUpdate();
			SpellCheckForm.lbcSuggestions.Items.Clear();
			try {
				if (SpellCheckForm.Suggestions.Count > 0) {
					for (int i = 0; i < SpellCheckForm.Suggestions.Count; i++)
						SpellCheckForm.lbcSuggestions.Items.Add(SpellCheckForm.Suggestions[i]);
					SpellCheckForm.lbcSuggestions.Enabled = true;
				}
				else {
					SpellCheckForm.lbcSuggestions.Items.Add(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.ListBoxNoSuggestions));
					SpellCheckForm.lbcSuggestions.Enabled = false;
				}
			}
			finally {
				SpellCheckForm.lbcSuggestions.Items.EndUpdate();
			}
		}
		public override void OnNewErrorFound(SpellCheckErrorBase error) {
			SpellCheckForm.Suggestion = string.Empty;
			SpellCheckForm.btnOptions.Enabled = true;
			bool isUndoAvailable = SearchStrategy.UndoManager.IsUndoAvailable();
			SpellCheckForm.btnClose.Visible = isUndoAvailable;
			SpellCheckForm.btnCancel.Visible = !isUndoAvailable;
			SpellCheckForm.CancelButton = isUndoAvailable ? SpellCheckForm.btnClose : SpellCheckForm.btnCancel;
			SpellCheckForm.btnUndoLast.Enabled = isUndoAvailable;
			if (IsRepeatedWordError(error))
				OnNewRepeatedWordError(error);
			else
				if (IsNotInDictionaryWordError(error))
					OnNewNotInDictionaryWordError(error);
		}
		protected virtual void OnNewRepeatedWordError(SpellCheckErrorBase error) {
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.lblRepeatedWord.Visible = true;
			SpellCheckForm.lblNotInDictionary.Visible = false;
			SpellCheckForm.btnDelete.Visible = true;
			SpellCheckForm.btnChange.Visible = false;
			SpellCheckForm.btnChangeAll.Enabled = false;
			SpellCheckForm.btnIgnoreAll.Enabled = false;
			SpellCheckForm.lblNotInDictionary.Visible = false;
			SpellCheckForm.btnAdd.Enabled = false;
			SpellCheckForm.txtNotInDictionary.Text = error.WrongWord;
			UnSubscribeFromEvents();
			try {
				SpellCheckForm.txtChangeTo.Text = SpellCheckForm.Suggestions.Count == 0 ? String.Empty : SpellCheckForm.Suggestions[0];
				SpellCheckForm.Suggestion = SpellCheckForm.txtChangeTo.Text;
			}
			finally {
				SubscribeToEvents();
			}
		}
		protected virtual void OnNewNotInDictionaryWordError(SpellCheckErrorBase error) {
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.lblRepeatedWord.Visible = false;
			SpellCheckForm.lblNotInDictionary.Visible = true;
			SpellCheckForm.btnDelete.Visible = false;
			SpellCheckForm.btnChange.Visible = true;
			SpellCheckForm.btnAdd.Enabled = CustomDictionaryExits();
			SpellCheckForm.btnChangeAll.Visible = true;
			SpellCheckForm.btnIgnoreAll.Enabled = true;
			SpellCheckForm.btnChange.Enabled = SpellCheckForm.btnChangeAll.Enabled = SpellCheckForm.Suggestions.Count > 0;
			SpellCheckForm.txtNotInDictionary.Text = error.WrongWord;
			SpellCheckForm.btnUndoLast.Enabled = SearchStrategy.UndoManager.IsUndoAvailable();
			UnSubscribeFromEvents();
			try {
				SpellCheckForm.txtChangeTo.Text = SpellCheckForm.Suggestions.Count == 0 ? error.WrongWord : SpellCheckForm.Suggestions[0];
				SpellCheckForm.Suggestion = SpellCheckForm.txtChangeTo.Text;
			}
			finally {
				SubscribeToEvents();
			}
		}
		protected override void SubscribeToEvents() {
			SpellCheckForm.txtChangeTo.EditValueChanging += OnTextEditValueChanging;
		}
		protected override void UnSubscribeFromEvents() {
			SpellCheckForm.txtChangeTo.EditValueChanging -= OnTextEditValueChanging;
		}
		protected virtual void OnTextEditValueChanging(object sender, ChangingEventArgs e) {
			bool needChangeButton = e.NewValue != null && !String.IsNullOrEmpty(e.NewValue.ToString());
			SpellCheckForm.btnChange.Visible = SpellCheckForm.btnChangeAll.Visible = needChangeButton;
			SpellCheckForm.btnChange.Enabled = SpellCheckForm.btnChangeAll.Enabled = needChangeButton;
			SpellCheckForm.btnDelete.Visible = !needChangeButton;
		}
	}
	public class SpellingWordStyleFormHelper : SpellingFormHelper {
		public SpellingWordStyleFormHelper(SpellingWordStyleForm spellCheckForm) : base(spellCheckForm) { }
		public new SpellingWordStyleForm SpellCheckForm {
			get { return base.SpellCheckForm as SpellingWordStyleForm; }
		}
		public override void PopulateFormSuggestions(SuggestionCollection suggestions) {
			base.PopulateFormSuggestions(suggestions);
			SpellCheckForm.lbcSuggestions.Items.BeginUpdate();
			try {
				SpellCheckForm.lbcSuggestions.Items.Clear();
				if (SpellCheckForm.Suggestions.Count > 0) {
					for (int i = 0; i < SpellCheckForm.Suggestions.Count; i++)
						SpellCheckForm.lbcSuggestions.Items.Add(SpellCheckForm.Suggestions[i]);
					SpellCheckForm.lbcSuggestions.Enabled = true;
				}
				else {
					SpellCheckForm.lbcSuggestions.Items.Add(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.ListBoxNoSuggestions));
					SpellCheckForm.lbcSuggestions.Enabled = false;
				}
			}
			finally {
				SpellCheckForm.lbcSuggestions.Items.EndUpdate();
			}
		}
		protected override void SubscribeToEvents() {
			SpellCheckForm.Load += new EventHandler(OnSpellCheckFormLoad);
			SpellCheckForm.Activated += new EventHandler(OnSpellCheckFormActivated);
			SpellCheckForm.Deactivate += new EventHandler(OnSpellCheckFormDeactivate);
		}
		void OnSpellCheckFormDeactivate(object sender, EventArgs e) {
		}
		void OnSpellCheckFormActivated(object sender, EventArgs e) {
		}
		void OnSpellCheckFormLoad(object sender, EventArgs e) {
		}
		protected override void UnSubscribeFromEvents() {
			SpellCheckForm.Load -= new EventHandler(OnSpellCheckFormLoad);
			SpellCheckForm.Activated -= new EventHandler(OnSpellCheckFormActivated);
			SpellCheckForm.Deactivate -= new EventHandler(OnSpellCheckFormDeactivate);
		}
		public void OnSelectionStartChangedByKeyboard() {
			SpellCheckForm.lbcSuggestions.Enabled = false;
			SpellCheckForm.btnIgnoreAll.Enabled = false;
			SpellCheckForm.btnIgnore.Enabled = false;
			SpellCheckForm.btnAdd.Enabled = false;
			SpellCheckForm.btnOptions.Enabled = false;
			SpellCheckForm.btnUndoLast.Enabled = false;
			SpellCheckForm.btnChange.Visible = true;
			SpellCheckForm.btnChange.Enabled = true;
			SpellCheckForm.btnChangeAll.Visible = true;
			SpellCheckForm.btnChangeAll.Enabled = !String.IsNullOrEmpty(SpellCheckForm.mmNotInDictionary.Text);
			SpellCheckForm.btnDelete.Visible = false;
		}
		public override void OnNewErrorFound(SpellCheckErrorBase error) {
			SpellCheckForm.Suggestion = string.Empty;
			SpellCheckForm.btnOptions.Enabled = true;
			SpellCheckForm.btnIgnoreAll.Enabled = true;
			SpellCheckForm.btnIgnore.Enabled = true;
			bool isUndoAvailable = SearchStrategy.UndoManager.IsUndoAvailable();
			SpellCheckForm.btnClose.Visible = isUndoAvailable;
			SpellCheckForm.btnCancel.Visible = !isUndoAvailable;
			SpellCheckForm.CancelButton = isUndoAvailable ? SpellCheckForm.btnClose : SpellCheckForm.btnCancel;
			SpellCheckForm.btnUndoLast.Enabled = isUndoAvailable;
			PopulateFormSuggestions(error.Suggestions);
			if (IsRepeatedWordError(error))
				OnNewRepeatedWordError(error);
			else
				if (IsNotInDictionaryWordError(error))
					OnNewNotInDictionaryWordError(error);
		}
		protected virtual void OnNewRepeatedWordError(SpellCheckErrorBase error) {
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.lblRepeatedWord.Visible = true;
			SpellCheckForm.lblNotInDictionary.Visible = false;
			SpellCheckForm.btnDelete.Visible = true;
			SpellCheckForm.btnChange.Visible = false;
			SpellCheckForm.btnChangeAll.Enabled = false;
			SpellCheckForm.lblNotInDictionary.Visible = false;
			SpellCheckForm.btnAdd.Enabled = false;
			SpellCheckForm.btnUndoLast.Enabled = SearchStrategy.UndoManager.IsUndoAvailable();
			SpellCheckForm.Suggestion = SpellCheckForm.Suggestions.Count > 0 ? SpellCheckForm.Suggestions[0] : string.Empty;
		}
		protected virtual void OnNewNotInDictionaryWordError(SpellCheckErrorBase error) {
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.lblRepeatedWord.Visible = false;
			SpellCheckForm.btnDelete.Visible = false;
			SpellCheckForm.btnChange.Visible = true;
			SpellCheckForm.lblNotInDictionary.Visible = true;
			SpellCheckForm.btnAdd.Enabled = CustomDictionaryExits();
			SpellCheckForm.btnChange.Enabled = SpellCheckForm.btnChangeAll.Enabled = SpellCheckForm.Suggestions.Count > 0;
			SpellCheckForm.btnUndoLast.Enabled = SearchStrategy.UndoManager.IsUndoAvailable();
			SpellCheckForm.Suggestion = SpellCheckForm.Suggestions.Count > 0 ? SpellCheckForm.Suggestions[0] : string.Empty;
		}
		public virtual void OnMemoTextChanged() {
			OnSelectionStartChangedByKeyboard();
		}
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public class HandlerCache {
		Hashtable cache = null;
		public HandlerCache() {
			cache = new Hashtable();
		}
		public void Clear() {
			cache.Clear();
		}
		public void ForEach(Action<SpellCheckHandlerBase> action) {
			foreach (DictionaryEntry entry in cache)
				action((SpellCheckHandlerBase)entry.Value);
		}
		public virtual SpellCheckHandlerBase this[StrategyState state] {
			get { return cache[state] as SpellCheckHandlerBase; }
		}
		public virtual bool ContainsHandler(StrategyState state) {
			return this[state] != null;
		}
		public virtual void Add(StrategyState state, SpellCheckHandlerBase handler) {
			if (!ContainsHandler(state))
				cache.Add(state, handler);
		}
		public virtual void Remove(StrategyState state) {
			if (ContainsHandler(state))
				cache.Remove(state);
		}
	}
}
