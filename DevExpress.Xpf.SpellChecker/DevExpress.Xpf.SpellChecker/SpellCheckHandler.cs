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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.SpellChecker.Forms;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
using System.Collections.Generic;
using DevExpress.XtraSpellChecker.Localization;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpellChecker.Parser;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
#if SL
using MessageBoxButton = DevExpress.Xpf.Core.DXMessageBoxButton;
#endif
namespace DevExpress.XtraSpellChecker {
	#region Routers
	public abstract class SpellCheckClientRouterBase : IDisposable {
		SearchStrategy searchStrategy;
		HandlerCache cache = new HandlerCache();
		protected SpellCheckClientRouterBase(SearchStrategy searchStrategy) {
			this.searchStrategy = searchStrategy;
			SubscribeToEvents();
		}
		public virtual SearchStrategy SearchStrategy { get { return searchStrategy; } }
		public virtual SpellCheckerBase SpellChecker { get { return SearchStrategy.SpellChecker; } }
		protected HandlerCache Cache { get { return cache; } }
		protected virtual void SubscribeToEvents() {
			SearchStrategy.StrategyStateChanged += OnStrategyStateChanged;
		}
		protected virtual void UnsubscribeFromEvents() {
			SearchStrategy.StrategyStateChanged -= OnStrategyStateChanged;
		}
		void OnStrategyStateChanged(object sender, StrategyStateChangedEventArgs e) {
			OnStrategyStateChanged(e.State);
		}
		protected virtual void OnStrategyStateChanged(StrategyState state) {
			HandleStateVisually(state);
		}
		protected virtual void HandleStateVisually(StrategyState state) {
			SpellCheckHandlerBase handler;
			if (!Cache.TryGetHandler(state, out handler))
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
				Cache.ForEach(item => item.Dispose());
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
		public virtual DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return SearchStrategy.SpellChecker as DevExpress.Xpf.SpellChecker.SpellChecker; } }
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
		protected virtual void OnClientHandlingError(SpellCheckErrorBase error) {
			FormsManager.ShowSpellCheckForm();
			if (!isFirstTimeAdjusted) {
				AdjustControlFirstTime();
			}
			SpellCheckFormHelper.OnNewErrorFound(error);
			AdjustControlRegularly();
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
#if SL
		bool ShouldSetFocusOnFormEditControl;
#endif
		List<Action> defferedActions = new List<Action>();
		public MSWordFoundErrorStateHandler(SearchStrategy searchStrategy)
			: base(searchStrategy) {
		}
		protected ISpellCheckMSWordTextControlController TextControlController { get { return SearchStrategy.TextController as ISpellCheckMSWordTextControlController; } }
		protected virtual SpellingWordStyleControl SpellCheckForm { get { return FormsManager.SpellCheckForm as SpellingWordStyleControl; } }
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
		void SubscribeToTextControlControllerEvent() {
			TextControlController.FormEditorSelectionStartChanged += new SelectionStartChangedEventHandler(TextControlController_FormEditorSelectionStartChanged);
			TextControlController.FormEditorTextChanged += new TextChangedEventHandler(TextControlController_FormEditorTextChanged);
			TextControlController.FormEditorKeyDown += new KeyEventHandler(TextControlController_FormEditorKeyDown);
		}
		void UnsubscribeToTextControlControllerEvent() {
			TextControlController.FormEditorSelectionStartChanged -= new SelectionStartChangedEventHandler(TextControlController_FormEditorSelectionStartChanged);
			TextControlController.FormEditorTextChanged -= new TextChangedEventHandler(TextControlController_FormEditorTextChanged);
			TextControlController.FormEditorKeyDown -= new KeyEventHandler(TextControlController_FormEditorKeyDown);
		}
		void TextControlController_FormEditorTextChanged(object sender, TextChangedEventArgs e) {
			OnFormEditorTextChanged(e);
		}
		void TextControlController_FormEditorSelectionStartChanged(object sender, SelectionStartChangedEventArgs e) {
			OnFormEditorSelectionStartChanged(e);
		}
		void TextControlController_FormEditorKeyDown(object sender, KeyEventArgs e) {
			OnFormEditorKeyDown(e);
		}
		protected override void AdjustControlRegularly() {
			IsOtherWordModified = false;
			IsSelectedWordModified = false;
			otherWordPosition = Position.Undefined;
			Position wordStart = SearchStrategy.WordStartPosition;
			Position wordEnd = SearchStrategy.CurrentPosition;
			TextEdit formEditControl = SpellCheckForm.EditControl;
			Position sentenceStart = SearchStrategy.TextController.GetSentenceStartPosition(wordStart);
			Position sentenceEnd = SearchStrategy.TextController.GetSentenceEndPosition(wordEnd);
			UnsubscribeToTextControlControllerEvent();
			formEditControl.Text = String.Empty;
			formEditControl.Text = TextController.GetWord(sentenceStart, sentenceEnd);
			SetSelection(formEditControl, wordStart, wordEnd, sentenceStart);
#if SL
			RoutedEventHandler handler = null;
			handler = (s, e) => {
				FormsManager.SpellCheckForm.LostFocus -= handler;
				if (ShouldSetFocusOnFormEditControl) {
					SetSelection(formEditControl, wordStart, wordEnd, sentenceStart);
					ShouldSetFocusOnFormEditControl = false;
				}
			};
			FormsManager.SpellCheckForm.LostFocus += handler;
#endif
			SubscribeToTextControlControllerEvent();
		}
		void SetSelection(TextEdit formEditControl, Position wordStart, Position wordEnd, Position sentenceStart) {
			formEditControl.Dispatcher.BeginInvoke(new Action(() => { formEditControl.Focus(); }));
			int selectionStart = TextController.GetWord(sentenceStart, wordStart).Length;
			int selectionLength = TextController.GetWord(wordStart, wordEnd).Length;
			TextControlController.FormEditorSelectionStart = new IntPosition(selectionStart);
			TextControlController.FormEditorSelectionFinish = new IntPosition(selectionStart + selectionLength);
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
			return (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) || (e.Key == Key.Insert && Keyboard.Modifiers == ModifierKeys.Shift);
		}
		protected virtual bool IsUndoRedoKeys(KeyEventArgs e) {
			return (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control) || (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control);
		}
		protected internal virtual void OnFormEditorKeyDown(KeyEventArgs e) {
			this.defferedActions.Clear();
			if (IsOtherWordModified)
				return;
			if (IsPasteKeys(e) || IsUndoRedoKeys(e))
				ProcessComplexChange();
			else if (e.Key == Key.Enter) {
				e.Handled = true;
				SpellCheckForm.IgnoreCommand.Execute(null);
			}
			else if (e.Key == Key.Delete)
				ProcessDeleteKey();
			else if (e.Key == Key.Back)
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
		protected virtual void OnFormEditorSelectionStartChanged(SelectionStartChangedEventArgs e) {
			if (e.Reason == SelectionChangeReason.Keyboard && e.Key != Key.Enter)
				OnFormEditorSelectionStartChangedByKeyboard(e.SelectionStart);
			else
				OnFormEditorSelectionStartChangedByMouse(e.SelectionStart);
		}
		protected virtual void OnFormEditorSelectionStartChangedByKeyboard(Position p) {
			SpellCheckFormHelper.OnSelectionStartChangedByKeyboard();
		}
		protected virtual void OnFormEditorSelectionStartChangedByMouse(Position p) { }
		protected internal virtual void OnFormEditorTextChanged(TextChangedEventArgs e) {
			foreach (Action action in this.defferedActions)
				action();
			this.defferedActions.Clear();
			if (IsSelectedWordModified)
				SpellCheckForm.Suggestion = TextControlController.GetFormEditorText(CurrentWordStartPosition, CurrentWordFinishPosition);
			SpellCheckFormHelper.OnFormEditorTextChanged();
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
			ISpellCheckTextControlController originalController = SearchStrategy.TextController as ISpellCheckTextControlController;
			SearchStrategy.TextController = new SimpleTextEditMSWordTextController(SpellCheckForm.EditControl, originalController);
#if SL
			ShouldSetFocusOnFormEditControl = true;
#endif
			base.AdjustControlFirstTime();
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
			if (!ShouldHideSpellCheckForm())
				return;
			FormsManager.HideSpellCheckForm(SpellChecker.SpellingFormType); 
			FormShowingEventArgs args = new FormShowingEventArgs();
			SpellChecker.OnCheckCompleteFormShowing(args);
			if (args.Handled)
				return;
			string title = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_Spelling_Caption);
			string text = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Msg_FinishCheck);
			FrameworkElement owner = SpellChecker.FormsManager.GetWidnowOwner();
#if SL
			DialogClosedDelegate onClosed = dialogResult => SearchStrategy.ResumeChecking();
			DialogHelper.ShowDialog(title, text, owner, onClosed, MessageBoxButton.OK);
			SearchStrategy.SuspendChecking();
#else
			DialogHelper.ShowDialog(title, text, owner, MessageBoxButton.OK);
#endif
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
		SpellingControlBase spellCheckForm;
		protected SpellingFormHelper(SpellingControlBase spellCheckForm) {
			this.spellCheckForm = spellCheckForm;
			SubscribeToEvents();
		}
		public virtual SpellingControlBase SpellCheckForm { get { return this.spellCheckForm; } }
		public virtual SearchStrategy SearchStrategy { get { return SpellCheckForm.SpellChecker.SearchStrategy; } }
		public virtual void PopulateFormSuggestions(SuggestionCollection suggestions) {
			SpellCheckForm.PopulateFormSuggestions(suggestions);
		}
		public abstract void OnNewErrorFound(SpellCheckErrorBase error);
		protected virtual void SubscribeToEvents() { }
		protected virtual void UnSubscribeFromEvents() { }
		protected virtual bool CustomDictionaryExits() {
			return SearchStrategy.DictionaryHelper.GetCustomDictionary() != null;
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
		public SpellingOutlookStyleFormHelper(SpellingOutlookStyleControl spellCheckForm) : base(spellCheckForm) { }
		public virtual new SpellingOutlookStyleControl SpellCheckForm { get { return base.SpellCheckForm as SpellingOutlookStyleControl; } }
		public override void PopulateFormSuggestions(SuggestionCollection suggestions) {
			base.PopulateFormSuggestions(suggestions);
		}
		public override void OnNewErrorFound(SpellCheckErrorBase error) {
			SpellCheckForm.Suggestion = string.Empty;
			SpellCheckForm.IsUndoAvailable = SearchStrategy.UndoManager.IsUndoAvailable();
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.WrongWord = error.WrongWord;
			SpellCheckForm.IsCustomDictionaryExits = CustomDictionaryExits();
			if (IsRepeatedWordError(error))
				OnNewRepeatedWordError(error);
			else if (IsNotInDictionaryWordError(error))
				OnNewNotInDictionaryWordError(error);
		}
		protected virtual void OnNewRepeatedWordError(SpellCheckErrorBase error) {
			SpellCheckForm.GoToState("RepeatedWordError");
			SpellCheckForm.Suggestion = SpellCheckForm.HasSuggestions ? SpellCheckForm.Suggestions[0] : String.Empty;
		}
		protected virtual void OnNewNotInDictionaryWordError(SpellCheckErrorBase error) {
			SpellCheckForm.GoToState("NotInDictionaryWordError_ChangeWord");
			SpellCheckForm.Suggestion = SpellCheckForm.HasSuggestions ? SpellCheckForm.Suggestions[0] : error.WrongWord;
		}
		protected virtual void OnTextEditValueChanging(object sender, EventArgs e) {
		}
	}
	public class SpellingWordStyleFormHelper : SpellingFormHelper {
		public SpellingWordStyleFormHelper(SpellingWordStyleControl spellCheckForm) : base(spellCheckForm) { }
		public new SpellingWordStyleControl SpellCheckForm {
			get { return base.SpellCheckForm as SpellingWordStyleControl; }
		}
		protected override void SubscribeToEvents() {
		}
		void OnSpellCheckFormDeactivate(object sender, EventArgs e) {
		}
		void OnSpellCheckFormActivated(object sender, EventArgs e) {
		}
		void OnSpellCheckFormLoad(object sender, EventArgs e) {
		}
		protected override void UnSubscribeFromEvents() {
		}
		public void OnSelectionStartChangedByKeyboard() {
			SpellCheckForm.CanDoChange = true;
			SpellCheckForm.GoToState("FormEditorSelectionStartChanged");
		}
		public override void OnNewErrorFound(SpellCheckErrorBase error) {
			SpellCheckForm.Suggestion = string.Empty;
			SpellCheckForm.IsUndoAvailable = SearchStrategy.UndoManager.IsUndoAvailable();
			PopulateFormSuggestions(error.Suggestions);
			SpellCheckForm.WrongWord = error.WrongWord;
			SpellCheckForm.IsCustomDictionaryExits = CustomDictionaryExits();
			if (IsRepeatedWordError(error))
				OnNewRepeatedWordError(error);
			else if (IsNotInDictionaryWordError(error))
				OnNewNotInDictionaryWordError(error);
		}
		protected virtual void OnNewRepeatedWordError(SpellCheckErrorBase error) {
			SpellCheckForm.GoToState("RepeatedWordError");
			SpellCheckForm.Suggestion = SpellCheckForm.HasSuggestions ? SpellCheckForm.Suggestions[0] : String.Empty;
		}
		protected virtual void OnNewNotInDictionaryWordError(SpellCheckErrorBase error) {
			SpellCheckForm.GoToState("NotInDictionaryWordError_ChangeWord");
			SpellCheckForm.Suggestion = SpellCheckForm.HasSuggestions ? SpellCheckForm.Suggestions[0] : error.WrongWord;
		}
		public virtual void OnFormEditorTextChanged() {
			OnSelectionStartChangedByKeyboard();
		}
	}
	public static class DialogHelper {
		public static bool? ShowDialog(string title, string text, FrameworkElement owner, DialogClosedDelegate onClosing, MessageBoxButton buttons) {
			TextBlock textBox = new TextBlock() { Text = text, Margin = new Thickness(5) };
			textBox.TextWrapping = TextWrapping.Wrap;
			textBox.VerticalAlignment = VerticalAlignment.Center;
			textBox.HorizontalAlignment = HorizontalAlignment.Center;
#if !SL
			DXDialogWindow window = new DXDialogWindow(title, buttons);
			if (owner != null) {
				ThemeTreeWalker walker = ThemeManager.GetTreeWalker(owner);
				if (walker != null && !String.IsNullOrEmpty(walker.ThemeName))
					ThemeManager.SetThemeName(window, walker.ThemeName);
			}
			window.Content = textBox;
			Window parent = LayoutHelper.FindParentObject<Window>(owner);
			window.Owner = parent;
			window.ResizeMode = ResizeMode.NoResize;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowIcon = false;
			window.MinWidth = 200;
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.ShowInTaskbar = false;
			window.ShowDialogWindow();
			if (onClosing != null)
				onClosing(window.DialogResult);
			return window.DialogResult;
#else
			DXDialogWindow window = new DXDialogWindow(title);
			window.Content = textBox;
			window.CloseOnEscape = true;
			window.MinWidth = 200;
			window.ShowDialogWindow(buttons).ContinueWith(task => {
				bool result = task.Result == MessageBoxResult.Yes || task.Result == MessageBoxResult.OK;
				if (onClosing != null)
					window.Dispatcher.BeginInvoke(new Action(() => onClosing(result)));
			});
			return null;
#endif
		}
		public static bool? ShowDialog(string title, string text, FrameworkElement owner, MessageBoxButton button) {
			return ShowDialog(title, text, owner, null, button);
		}
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public class HandlerCache {
		Dictionary<StrategyState, SpellCheckHandlerBase> cache = new Dictionary<StrategyState, SpellCheckHandlerBase>();
		public virtual SpellCheckHandlerBase this[StrategyState state] {
			get {
				SpellCheckHandlerBase result;
				if (cache.TryGetValue(state, out result))
					return result;
				else
					return null;
			}
		}
		public void ForEach(Action<SpellCheckHandlerBase> action) {
			foreach (SpellCheckHandlerBase value in cache.Values)
				value.Dispose();
		}
		public bool TryGetHandler(StrategyState state, out SpellCheckHandlerBase handler) {
			return cache.TryGetValue(state, out handler);
		}
		public virtual bool ContainsHandler(StrategyState state) {
			return cache.ContainsKey(state);
		}
		public virtual void Add(StrategyState state, SpellCheckHandlerBase handler) {
			if (!ContainsHandler(state))
				cache.Add(state, handler);
		}
		public virtual void Remove(StrategyState state) {
			cache.Remove(state);
		}
		public void Clear() {
			cache.Clear();
		}
	}
}
