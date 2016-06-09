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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using System.Linq;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors {
	public enum NewTokenPosition { None, Near, Far };
}
namespace DevExpress.Xpf.Editors.Internal {
	[DXToolboxBrowsable(false)]
	public class TokenEditor : Control, IScrollBarThumbDragDeltaListener {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ActiveEditorProperty;
		public static readonly DependencyProperty IsTextEditableProperty;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly DependencyProperty FocusPopupOnOpenProperty;
		static readonly DependencyPropertyKey ActiveEditorPropertyKey;
		public static readonly DependencyProperty EnableTokenWrappingProperty;
		public static readonly DependencyProperty TokenBorderTemplateProperty;
		public static readonly DependencyProperty ShowTokenButtonsProperty;
		public static readonly DependencyProperty TokenButtonsProperty;
		public static readonly DependencyProperty NewTokenPositionProperty;
		public static readonly DependencyProperty TokenTextTrimmingProperty;
		public static readonly DependencyProperty TokenMaxWidthProperty;
		public static readonly DependencyProperty EditModeProperty;
		public static readonly DependencyProperty AllowEditTokensProperty;
		public static readonly DependencyProperty CharacterCasingProperty;
		public static readonly DependencyProperty NullTextForegroundProperty;
		static TokenEditor() {
			Type ownerType = typeof(TokenEditor);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			EditValueProperty = DependencyProperty.Register("EditValue", typeof(object), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((TokenEditor)d).OnEditValueChanged(e.OldValue, e.NewValue)));
			ActiveEditorPropertyKey = DependencyProperty.RegisterReadOnly("ActiveEditor", typeof(TextEdit), ownerType, new FrameworkPropertyMetadata(null));
			ActiveEditorProperty = ActiveEditorPropertyKey.DependencyProperty;
			IsTextEditableProperty = DependencyProperty.Register("IsTextEditable", typeof(bool), ownerType);
			IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TokenEditor)d).OnIsReadOnlyChanged()));
			FocusPopupOnOpenProperty = DependencyProperty.Register("FocusPopupOnOpen", typeof(bool), ownerType);
			EnableTokenWrappingProperty = DependencyProperty.Register("EnableTokenWrapping", typeof(bool), ownerType);
			TokenBorderTemplateProperty = DependencyProperty.Register("TokenBorderTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((TokenEditor)d).OnTokenBorderTemplateChanged(), (d, e) => ((TokenEditor)d).OnCoerceTokenBorderTemplate(e as ControlTemplate)));
			TokenButtonsProperty = DependencyProperty.Register("TokenButtons", typeof(ButtonInfoCollection), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((TokenEditor)d).OnTokenButtonsChanged((ButtonInfoCollection)e.OldValue, (ButtonInfoCollection)e.NewValue)));
			ShowTokenButtonsProperty = DependencyProperty.Register("ShowTokenButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			NewTokenPositionProperty = DependencyProperty.Register("NewTokenPosition", typeof(NewTokenPosition), ownerType,
				new FrameworkPropertyMetadata(NewTokenPosition.Near, (d, e) => ((TokenEditor)d).OnNewTokenPositionChanged()));
			TokenTextTrimmingProperty = DependencyProperty.Register("TokenTextTrimming", typeof(TextTrimming), ownerType);
			TokenMaxWidthProperty = DependencyProperty.Register("TokenMaxWidth", typeof(double), ownerType);
			EditModeProperty = DependencyProperty.Register("EditMode", typeof(EditMode), ownerType);
			FontSizeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((TokenEditor)d).OnFontSizeChanged()));
			AllowEditTokensProperty = DependencyProperty.Register("AllowEditTokens", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			CharacterCasingProperty = DependencyProperty.Register("CharacterCasing", typeof(CharacterCasing), ownerType, new FrameworkPropertyMetadata(CharacterCasing.Normal));
			NullTextForegroundProperty = DependencyProperty.Register("NullTextForeground", typeof(Brush), ownerType);
		}
		public TokenEditor() {
			this.Loaded += TokenEditorLoaded;
			this.LayoutUpdated += TokenEditorLayoutUpdated;
			this.LostFocus += TokenEditorLostFocus;
			ImmediateActionsManager = new ImmediateActionsManager(this);
		}
		public LookUpEditBase OwnerEdit { get { return (LookUpEditBase)GetValue(BaseEdit.OwnerEditProperty); } }
		public Brush NullTextForeground {
			get { return (Brush)GetValue(NullTextForegroundProperty); }
			set { SetValue(NullTextForegroundProperty, value); }
		}
		public CharacterCasing CharacterCasing {
			get { return (CharacterCasing)GetValue(CharacterCasingProperty); }
			set { SetValue(CharacterCasingProperty, value); }
		}
		public bool AllowEditTokens {
			get { return (bool)GetValue(AllowEditTokensProperty); }
			set { SetValue(AllowEditTokensProperty, value); }
		}
		public EditMode EditMode {
			get { return (EditMode)GetValue(EditModeProperty); }
			set { SetValue(EditModeProperty, value); }
		}
		public double TokenMaxWidth {
			get { return (double)GetValue(TokenMaxWidthProperty); }
			set { SetValue(TokenMaxWidthProperty, value); }
		}
		public TextTrimming TokenTextTrimming {
			get { return (TextTrimming)GetValue(TokenTextTrimmingProperty); }
			set { SetValue(TokenTextTrimmingProperty, value); }
		}
		public bool EnableTokenWrapping {
			get { return (bool)GetValue(EnableTokenWrappingProperty); }
			set { SetValue(EnableTokenWrappingProperty, value); }
		}
		public NewTokenPosition NewTokenPosition {
			get { return (NewTokenPosition)GetValue(NewTokenPositionProperty); }
			set { SetValue(NewTokenPositionProperty, value); }
		}
		public bool ShowNewTokenFromEnd { get { return NewTokenPosition == NewTokenPosition.Far; } }
		public ControlTemplate TokenBorderTemplate {
			get { return (ControlTemplate)GetValue(TokenBorderTemplateProperty); }
			set { SetValue(TokenBorderTemplateProperty, value); }
		}
		public ButtonInfoCollection TokenButtons {
			get { return (ButtonInfoCollection)GetValue(TokenButtonsProperty); }
			set { SetValue(TokenButtonsProperty, value); }
		}
		public bool ShowTokenButtons {
			get { return (bool)GetValue(ShowTokenButtonsProperty); }
			set { SetValue(ShowTokenButtonsProperty, value); }
		}
		public TextEdit ActiveEditor {
			get { return (TextEdit)GetValue(ActiveEditorProperty); }
			private set { SetValue(ActiveEditorPropertyKey, value); }
		}
		public object EditValue {
			get { return (object)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public bool IsTextEditable {
			get { return (bool)GetValue(IsTextEditableProperty); }
			set { SetValue(IsTextEditableProperty, value); }
		}
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public bool FocusPopupOnOpen {
			get { return (bool)GetValue(FocusPopupOnOpenProperty); }
			set { SetValue(FocusPopupOnOpenProperty, value); }
		}
		public bool IsEditorKeyboardFocused { get { return OwnerEdit.Return(x => x.GetIsEditorKeyboardFocused(), () => false); } }
		public bool IsPopupCloseInProgress { get { return OwnerEdit.Return(x => x.GetIsPopupCloseInProgress(), () => false); } }
		public bool ShowDefaultToken { get { return CanEditing && NewTokenPosition != Editors.NewTokenPosition.None; } }
		public bool HasSelection { get { return Selection.HasSelectedTokens; } }
		public string SelectedText {
			get { return HasActiveEditor ? ActiveEditor.SelectedText : string.Empty; }
			set { if (HasActiveEditor) ActiveEditor.SelectedText = value; }
		}
		public int SelectionLength {
			get { return HasActiveEditor ? ActiveEditor.SelectionLength : 0; }
			set { if (HasActiveEditor) ActiveEditor.SelectionLength = value; }
		}
		public int SelectionStart {
			get { return HasActiveEditor ? ActiveEditor.SelectionStart : -1; }
			set { if (HasActiveEditor) ActiveEditor.SelectionStart = value; }
		}
		public int CaretIndex {
			get { return HasActiveEditor ? ActiveEditor.CaretIndex : -1; }
			set { if (HasActiveEditor)ActiveEditor.CaretIndex = value; }
		}
		public bool CanUndo {
			get { return HasActiveEditor ? ((TextEditStrategy)ActiveEditor.EditStrategy).CanUndo() : false; }
		}
		public int MaxLength {
			get { return HasActiveEditor ? ActiveEditor.MaxLength : 0; }
			set { if (HasActiveEditor) ActiveEditor.MaxLength = value; }
		}
		public string Text { get { return HasActiveEditor ? ActiveEditor.EditValue.Return(x => x.ToString(), () => string.Empty) : string.Empty; } }
		public bool IsUndoEnabled {
			get { return HasActiveEditor && IsActiveEditorHasEditBox() ? ActiveEditor.EditBox.IsUndoEnabled : false; }
			set { if (HasActiveEditor && IsActiveEditorHasEditBox()) ActiveEditor.EditBox.IsUndoEnabled = value; }
		}
		Locker changeFocusedTokenLocker = new Locker();
		Locker lockDefaultTokenActivation = new Locker();
		Locker processEditValueLocker = new Locker();
		Locker editValueLocker = new Locker();
		bool ForceUpdate { get; set; }
		bool CanEditing { get { return IsTextEditable && !IsReadOnly; } }
		int EditableValueIndex { get { return EditValueInternal.Return(x => x.Index, () => -1); } }
		TokenEditorKeyboardHelper keyboardHelper;
		TokenEditorKeyboardHelper KeyboardHelper { get { return keyboardHelper ?? CreateKeyboardHelper(); } }
		TokenEditorSelection selection;
		TokenEditorSelection Selection { get { return selection ?? CreateSelection(); } }
		TokenEditorPresenter DefaultToken { get { return tokenEditorPanel.Return(x => x.DefaultTokenPresenter, () => null); } }
		bool HasFocusedToken { get { return FocusedToken != null; } }
		internal TokenEditorCustomItem EditValueInternal { get { return EditValue as TokenEditorCustomItem; } }
		internal bool HasActiveEditor { get { return ActiveEditor != null; } }
		internal ImmediateActionsManager ImmediateActionsManager { get; private set; }
		TokenEditorPresenter focusedToken;
		public TokenEditorPresenter FocusedToken {
			get { return focusedToken; }
			set {
				if (focusedToken != value) {
					focusedToken = value;
					OnFocusedTokenChanged(value);
				}
			}
		}
		InplaceEditorOwnerBase cellEditorOwner;
		internal InplaceEditorOwnerBase CellEditorOwner { get { return cellEditorOwner ?? (cellEditorOwner = CreateCellEditorOwner()); } }
		internal bool IsInProcessNewValue { get; set; }
		EventHandler<EventArgs> textChanged;
		public event EventHandler<EventArgs> TextChanged {
			add { textChanged += value; }
			remove { textChanged -= value; }
		}
		EventHandler<EventArgs> valueChanged;
		public event EventHandler<EventArgs> ValueChanged {
			add { valueChanged += value; }
			remove { valueChanged -= value; }
		}
		EventHandler<EventArgs> tokenClosed;
		public event EventHandler<EventArgs> TokenClosed {
			add { tokenClosed += value; }
			remove { tokenClosed -= value; }
		}
		public void RemoveValueByIndex(int index) {
			var listEditValue = EditValueInternal.EditValue as IList;
			if (index < listEditValue.Count && index > -1) {
				listEditValue.RemoveAt(index);
				EditValueInternal.Index = -1;
			}
		}
		public void RemoveFocusedToken(TokenEditorPresenter token = null) {
			if (!IsReadOnly)
				RemoveToken(token == null ? FocusedToken : token);
		}
		public void RemoveToken(int index) {
			ForceUpdateTokens(() => {
				if (!IsReadOnly && tokenEditorPanel.Items.Count > 0) {
					RemoveTokenByIndex(tokenEditorPanel.ConvertToEditableIndex(index));
					RaiseValueChanged();
				}
			});
		}
		public void RemoveSelectedTokens() {
			if (!IsReadOnly && Selection.HasSelectedTokens) {
				RemoveTokens(Selection.SelectedTokensIndexes);
				Selection.ResetSelection();
				AfterTokenRemoved(FocusedToken);
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == ForegroundProperty) {
				HasForeground = true;
			}
		}
		internal bool HasForeground { get; set; }
		TokenEditorPanel tokenEditorPanel;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateTokenEditorPanel();
		}
		public int GetCharacterIndexFromLineIndex(int lineIndex) {
			return HasActiveEditor ? ActiveEditor.GetCharacterIndexFromLineIndex(lineIndex) : -1;
		}
		public int GetCharacterIndexFromPoint(Point point, bool snapToText) {
			return HasActiveEditor ? ActiveEditor.GetCharacterIndexFromPoint(point, snapToText) : -1;
		}
		public int GetFirstVisibleLineIndex() {
			return HasActiveEditor ? ActiveEditor.GetFirstVisibleLineIndex() : -1;
		}
		public int GetLastVisibleLineIndex() {
			return HasActiveEditor ? ActiveEditor.GetLastVisibleLineIndex() : -1;
		}
		public int GetLineIndexFromCharacterIndex(int charIndex) {
			return HasActiveEditor ? ActiveEditor.GetLineIndexFromCharacterIndex(charIndex) : -1;
		}
		public int GetLineLength(int lineIndex) {
			return HasActiveEditor ? ActiveEditor.GetLineLength(lineIndex) : 0;
		}
		public string GetLineText(int lineIndex) {
			return HasActiveEditor ? ActiveEditor.GetLineText(lineIndex) : string.Empty;
		}
		public void Select(int start, int length) {
			if (HasActiveEditor)
				ActiveEditor.Select(start, length);
		}
		public void ScrollToHome() {
			if (HasActiveEditor && IsActiveEditorHasEditBox()) ActiveEditor.EditBox.ScrollToHome();
		}
		private bool IsActiveEditorHasEditBox() {
			return ActiveEditor.EditBox != null;
		}
		public void SelectAll() {
			if (HasActiveEditor) ActiveEditor.SelectAll();
		}
		public void Copy() {
			if (HasActiveEditor) ActiveEditor.Copy();
		}
		public void Cut() {
			if (HasActiveEditor) ActiveEditor.Cut();
		}
		public void Undo() {
			if (HasActiveEditor) ActiveEditor.Undo();
		}
		public void Paste() {
			if (HasActiveEditor) ActiveEditor.Paste();
		}
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			return KeyboardHelper.NeedsKey(key, modifiers);
		}
		public void SetEditValue(object value) {
			EditValue = value;
			if (editValueLocker.IsLocked) return;
			if (HasActiveEditor)
				UpdateActiveEditorValue(value);
			else {
				if (ShouldAssignNullText())
					ImmediateActionsManager.EnqueueAction(() => UpdateDefaultToken());
			}
			InvalidateLayout();
		}
		public void BeforeAcceptPopupValue() {
		}
		public void AfterAcceptPopupValue() {
			if (IsDefaultTokenFocused()) {
				ImmediateActionsManager.EnqueueAction(ActivateDefaultToken);
			}
		}
		public bool NeedsActivationKeyInInactiveMode(Key key, ModifierKeys modifiers) {
			return DefaultToken != null && DefaultToken.IsActivatingKey(key, modifiers);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Enter) {
				e.Handled = true;
				if (postponeSyncWithValue != null) {
					postponeSyncWithValue.Invoke();
					postponeSyncWithValue = null;
				}
			}
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
			ProcessMouseWheel(e.Delta);
		}
		private void ProcessMouseWheel(double delta) {
			if (delta > 0) {
				if (tokenEditorPanel.Orientation == Orientation.Horizontal)
					tokenEditorPanel.LineLeft();
				else
					tokenEditorPanel.LineUp();
			}
			else {
				if (tokenEditorPanel.Orientation == Orientation.Horizontal)
					tokenEditorPanel.LineRight();
				else
					tokenEditorPanel.LineDown();
			}
		}
		protected virtual object OnCoerceTokenBorderTemplate(ControlTemplate value) {
			return value ?? TokenBorderTemplate;
		}
		protected virtual void OnTokenButtonsChanged(ButtonInfoCollection oldValue, ButtonInfoCollection newValue) { }
		protected virtual void OnTokenBorderTemplateChanged() { }
		protected virtual void OnNewTokenPositionChanged() {
			if (tokenEditorPanel != null) {
				tokenEditorPanel.UpdateMeasureStrategy();
				InvalidateLayout();
			}
		}
		protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			if (tokenEditorPanel == null || !tokenEditorPanel.IsLoaded) {
				Action action = new Action(() => {
					changeFocusedTokenLocker.DoLockedAction(() => ProcessMouseLeftButtonDown(e));
				});
				ReraiseAction reraiseAction = new ReraiseAction(action, CanProcessMouseDown, ImmediateActionsManager);
				ImmediateActionsManager.EnqueueAction(() => reraiseAction.Perform());
			}
			else
				changeFocusedTokenLocker.DoLockedAction(() => ProcessMouseLeftButtonDown(e));
		}
		private bool CanProcessMouseDown() {
			return tokenEditorPanel != null && tokenEditorPanel.Items.Count != 0 && tokenEditorPanel.HasMeasuredTokens;
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			if (KeyboardHelper.IsCtrlPressed()) return;
			if (IsEditorKeyboardFocused && e.NewFocus == this && HasActiveEditor) {
				FocusedToken.Editor.FocusEditCore();
				return;
			}
			if (!IsFocusFromCellEditor(e.OldFocus) && e.NewFocus == this) {
				if (lockDefaultTokenActivation) return;
				if (DefaultToken != null) {
					if (!DefaultToken.IsEditorActivated && IsDefaultTokenVisible()) {
						ActivateDefaultToken();
					}
					else if (LayoutHelper.FindParentObject<TokenEditorPresenter>((DependencyObject)Keyboard.FocusedElement) != DefaultToken)
						DefaultToken.Editor.FocusEditCore();
				}
				else {
					Action action = new Action(() => {
						if (IsDefaultTokenVisible())
							ActivateDefaultToken();
					});
					ReraiseAction reraiseAction = new ReraiseAction(action, () => DefaultToken != null, ImmediateActionsManager);
					ImmediateActionsManager.EnqueueAction(() => reraiseAction.Perform());
				}
			}
		}
		private bool IsDefaultTokenVisible() {
			if (DefaultToken == null) return false;
			var bounds = DefaultToken.TransformToVisual(this).TransformBounds(new Rect(0, 0, DefaultToken.ActualWidth, DefaultToken.ActualHeight));
			return (new Rect(0, 0, ActualWidth, ActualHeight)).IntersectsWith(bounds);
		}
		private void CommitActiveEditor() {
			if (tokenEditorPanel == null) return;
			if (IsDefaultTokenFocused())
				processEditValueLocker.DoLockedAction(() => tokenEditorPanel.ClearDefaultTokenValue());
			if (HasFocusedToken)
				FocusedToken.CommitEditor();
		}
		internal int GetTokenByHorizontalOffset() {
			return tokenEditorPanel.OffsetToIndex(tokenEditorPanel.HorizontalOffset);
		}
		internal bool CanActivateToken() {
			return CanEditing && (IsDefaultTokenFocused() || AllowEditTokens);
		}
		internal void BeforeCancelEdit() {
			if (IsDefaultTokenFocused() && EditValueInternal != null) {
				RemoveTokenByIndex(EditValueInternal.Index);
				RaiseValueChanged();
			}
		}
		internal void OnStartEditing(TextEdit editor) {
			ActiveEditor = editor;
			Selection.SetSelectionFromFocusedToken();
			if (HasFocusedToken) {
				var index = tokenEditorPanel.GetEditableIndexOfContainer(FocusedToken);
				UpdateEditValueOnStartEditing(index);
			}
		}
		internal string GetNullText() {
			return EditValueInternal.Return(x => string.IsNullOrEmpty(x.NullText) ? null : x.NullText, () => null);
		}
		internal void SetFocusedTokenByIndexWithLock(int index, bool shouldActivate = false) {
			bool activate = shouldActivate || IsDefaultTokenIndex(index);
			changeFocusedTokenLocker.DoLockedAction(() => {
				if (activate)
					SetFocusedAndActivateTokenByIndex(index);
				else {
					var container = GetTokenByVisibleIndex(index);
					if (container != null) FocusedToken = container;
				}
			});
		}
		internal void SetFocusedTokenByEditableIndex(int index, bool shouldActivate = false) {
			SetFocusedTokenByIndexWithLock(tokenEditorPanel.ConvertToVisibleIndex(index), shouldActivate);
		}
		internal void OnNavigateDown(bool isShiftPressed) {
			OnVerticalNavigation(false, isShiftPressed);
		}
		internal void OnNavigateUp(bool isShiftPressed) {
			OnVerticalNavigation(true, isShiftPressed);
		}
		internal void NavigateToVerticalStart() {
			tokenEditorPanel.ScrollToVerticalStart();
			UpdateLayout();
			SetFocusedTokenByIndexWithLock(tokenEditorPanel.MinVisibleIndex);
		}
		internal void NavigateToVerticalEnd() {
			tokenEditorPanel.ScrollToVerticalEnd();
			UpdateLayout();
			SetFocusedTokenByIndexWithLock(tokenEditorPanel.MaxVisibleIndex);
		}
		private void OnVerticalNavigation(bool up, bool isShiftPressed) {
			if (FocusedToken != null) {
				int tokenIndex = tokenEditorPanel.GetEditableIndexOfContainer(FocusedToken);
				int focusedIndex = tokenEditorPanel.ConvertToVisibleIndex(tokenIndex);
				var line = tokenEditorPanel.GetLineRelativeToken(focusedIndex, up);
				if (line != null) {
					int newFocusedIndex = FindNewFocusedTokenOnVerticalNavigation(line);
					if (newFocusedIndex != -1) {
						if (isShiftPressed)
							SelectTokensOnVerticalNavigation(up, focusedIndex, newFocusedIndex);
						else
							SetFocusedTokenByIndexWithLock(newFocusedIndex);
						tokenEditorPanel.BringIntoViewByIndex(newFocusedIndex);
					}
				}
			}
			else if (up)
				tokenEditorPanel.LineUp();
			else
				tokenEditorPanel.LineDown();
		}
		private int FindNewFocusedTokenOnVerticalNavigation(TokenEditorLineInfo line) {
			var bounds = LayoutHelper.GetRelativeElementRect(FocusedToken, this);
			List<int> lineIndexes = new List<int>();
			line.Tokens.ForEach(x => lineIndexes.Add(x.VisibleIndex));
			lineIndexes.Reverse();
			int newFocusedIndex = -1;
			Rect maxIntersection = new Rect();
			foreach (int index in lineIndexes) {
				var token = tokenEditorPanel.GetContainer(tokenEditorPanel.ConvertToEditableIndex(index)) as TokenEditorPresenter;
				if (token != null) {
					var tokenBounds = LayoutHelper.GetRelativeElementRect(token, this);
					var intersectBounds = new Rect(bounds.X, tokenBounds.Y, bounds.Width, tokenBounds.Height);
					tokenBounds.Intersect(intersectBounds);
					if (IsGreaterBounds(tokenBounds, maxIntersection)) {
						maxIntersection = tokenBounds;
						newFocusedIndex = index;
					}
				}
			}
			if (newFocusedIndex == -1 && lineIndexes.Count > 0)
				newFocusedIndex = lineIndexes[0];
			return newFocusedIndex;
		}
		bool IsGreaterBounds(Rect targetBounds, Rect maxBounds) {
			return targetBounds.Width > maxBounds.Width;
		}
		private void SelectTokensOnVerticalNavigation(bool up, int focusedIndex, int newFocusedIndex) {
			List<int> newSelection = new List<int>();
			int editableIndex = tokenEditorPanel.ConvertToEditableIndex(newFocusedIndex);
			if (up) {
				for (int i = newFocusedIndex + 1; i < focusedIndex; i++)
					newSelection.Add(tokenEditorPanel.ConvertToEditableIndex(i));
				SelectTokensOnVerticalNavigationCore(newSelection, focusedIndex, editableIndex < Selection.StartSelectionIndex);
			}
			else {
				for (int i = focusedIndex + 1; i < newFocusedIndex; i++)
					newSelection.Add(tokenEditorPanel.ConvertToEditableIndex(i));
				SelectTokensOnVerticalNavigationCore(newSelection, focusedIndex, editableIndex > Selection.StartSelectionIndex);
			}
			Selection.SelectTokenByIndex(tokenEditorPanel.ConvertToEditableIndex(newFocusedIndex));
			Selection.LockSelection(() => SetFocusedTokenByIndexWithLock(newFocusedIndex, false));
		}
		private void SelectTokensOnVerticalNavigationCore(List<int> newSelection, int oldFocused, bool canAdd) {
			if (!newSelection.Contains(Selection.StartSelectionIndex) && canAdd)
				newSelection.ForEach(x => Selection.SelectTokenByIndex(x));
			else {
				var toRemove = tokenEditorPanel.GetIndexesInLine(oldFocused);
				toRemove.ForEach(x => Selection.RemoveTokenFromSelection(tokenEditorPanel.ConvertToEditableIndex(x)));
			}
		}
		internal bool IsValueChanged() {
			return ActiveEditor != null && ActiveEditor.EditValue != null;
		}
		internal void ProcessKeyDownFromCellEditor(KeyEventArgs e) {
			if (e.Key == Key.Tab) {
				CellEditorOwner.MoveFocus(e);
				e.Handled = true;
			}
		}
		internal bool ProcessKeyDown(KeyEventArgs e) {
			return KeyboardHelper.ProcessPreviewKeyDown(e);
		}
		internal int EditableIndexOfToken(TokenEditorPresenter token) {
			return tokenEditorPanel.GetEditableIndexOfContainer(token);
		}
		internal Dictionary<int, UIElement> GetVisibleTokens() {
			return tokenEditorPanel != null ? tokenEditorPanel.GetVisibleTokens() : null;
		}
		internal void ClearAndRemoveDefaultTokenValue(bool shouldRaiseEvent = true) {
			if (EditableValueIndex < 0) return;
			processEditValueLocker.DoLockedAction(() => tokenEditorPanel.ClearDefaultTokenValue());
			RemoveValueByIndex(EditableValueIndex);
			if (shouldRaiseEvent)
				RaiseValueChanged();
		}
		internal bool OnNavigateHorizontalStart(bool activateToken) {
			return ScrollToIndex(tokenEditorPanel.MinVisibleIndex, activateToken);
		}
		internal bool OnNavigateHorizontalEnd(bool activateToken) {
			if (ShouldScroll(tokenEditorPanel.MaxVisibleIndex)) {
				tokenEditorPanel.ScrollToHorizontalEnd();
				ImmediateActionsManager.EnqueueAction(() => SetFocusedTokenByIndexWithLock(tokenEditorPanel.MaxVisibleIndex, activateToken));
			}
			else
				SetFocusedTokenByIndexWithLock(tokenEditorPanel.MaxVisibleIndex, activateToken);
			return true;
		}
		internal bool OnNavigateLeft(int index, bool activateToken) {
			return ScrollToIndex(index, activateToken);
		}
		private bool ScrollToIndex(int index, bool activateToken) {
			if (ShouldScroll(index))
				return ScrollLeftAndMoveFocusedToken(index, activateToken);
			else {
				SetFocusedTokenByIndexWithLock(index, activateToken);
				return true;
			}
		}
		internal bool OnNavigateRight(int index, bool activateToken) {
			if (ShouldScroll(index))
				return ScrollRightAndMoveFocusedToken(index, activateToken);
			else {
				SetFocusedTokenByIndexWithLock(index, activateToken);
				return true;
			}
		}
		internal bool SelectTokenOnNavigateLeft(int index) {
			if (ShouldScroll(index))
				return ScrollLeftAndSelectToken(index);
			else {
				SelectTokenByKeyboard(index, true);
				return true;
			}
		}
		internal bool SelectTokenOnNavigateRight(int index) {
			if (ShouldScroll(index))
				return ScrollRightAndSelectToken(index);
			else {
				SelectTokenByKeyboard(index, false);
				return true;
			}
		}
		internal void ProcessActiveEditorEditValueChanged(object oldValue, object newValue) {
			if (processEditValueLocker.IsLocked) return;
			if (EditValueInternal != null && EditValueInternal.Index > -1) {
				IsInProcessNewValue = true;
				var listEditValue = EditValueInternal.EditValue as IList<CustomItem>;
				CustomItem item = new CustomItem() { EditValue = newValue };
				listEditValue[EditValueInternal.Index] = item;
				RaiseTextChanged();
				ImmediateActionsManager.EnqueueAction(() => tokenEditorPanel.OnActiveTokenEditValueChanged(FocusedToken));
			}
		}
		internal bool IsDefaultTokenFocused() {
			return DefaultToken == FocusedToken;
		}
		internal void OnTokenHided() {
			IsInProcessNewValue = false;
			OnTokenHidedCore();
		}
		internal void MakeVisibleToken(TokenEditorPresenter token) {
			tokenEditorPanel.BringIntoView(token);
		}
		internal void ContinueEditToken(TokenEditorPresenter token) {
			if (token == null) return;
			var editor = token.Editor.GetEditor() as TextEdit;
			if (editor != null)
				OnStartEditing(editor);
		}
		void ForceUpdateTokens(Action action) {
			ForceUpdate = true;
			action.Invoke();
			ForceUpdate = false;
		}
		void OnIsReadOnlyChanged() {
			ResetTokens();
		}
		void ResetTokens() {
			if (tokenEditorPanel != null) {
				tokenEditorPanel.Clear();
				UpdateTokens();
				InvalidateLayout();
			}
		}
		void OnFontSizeChanged() {
		}
		void TokenEditorLostFocus(object sender, RoutedEventArgs e) {
			var focus = Keyboard.FocusedElement;
			if (!IsEditorKeyboardFocused && focus != null && OwnerEdit != null && OwnerEdit != focus && (!OwnerEdit.IsChildElement(focus as DependencyObject) ||
			   (OwnerEdit.EditMode != Editors.EditMode.Standalone && !LayoutHelper.IsChildElement(this, focus as DependencyObject)))) {
				CommitActiveEditor();
				ResetFocusedToken();
			}
		}
		void UpdateEditValueOnStartEditing(int index) {
			if (EditValueInternal != null) {
				if (IsDefaultTokenFocused()) {
					var editValue = EditValueInternal.EditValue as IList<CustomItem>;
					if (editValue != null) editValue.Add(new CustomItem());
				}
				EditValueInternal.Index = index;
			}
			else
				SetEditValueInternal(new TokenEditorCustomItem() { Index = index, EditValue = new List<CustomItem>() { new CustomItem() } });
		}
		bool ScrollLeftByIndex(int index) {
			tokenEditorPanel.ScrollLeft(index);
			return true;
		}
		bool ScrollRightByIndex(int index) {
			tokenEditorPanel.ScrollRight(index);
			return true;
		}
		bool ScrollLeftAndDoAction(int index, Action action) {
			bool isScroll = ScrollLeftByIndex(index);
			ImmediateActionsManager.EnqueueAction(() => action.Invoke());
			return isScroll;
		}
		TokenEditorSelection CreateSelection() {
			selection = new TokenEditorSelection(this);
			return selection;
		}
		TokenEditorKeyboardHelper CreateKeyboardHelper() {
			keyboardHelper = new TokenEditorKeyboardHelper(this);
			return keyboardHelper;
		}
		bool ShouldScroll(int index) {
			return tokenEditorPanel.ShouldScroll(index);
		}
		bool ScrollLeftAndMoveFocusedToken(int index, bool shouldActivate = false) {
			return ScrollLeftAndDoAction(index, () => SetFocusedTokenByIndexWithLock(index, shouldActivate));
		}
		bool ScrollLeftAndSelectToken(int newSelectedIndex) {
			return ScrollLeftAndDoAction(newSelectedIndex, () => SelectTokenByKeyboard(newSelectedIndex, true));
		}
		void SelectTokenByKeyboard(int newSelectedIndex, bool isLeft) {
			int editableIndex = tokenEditorPanel.ConvertToEditableIndex(newSelectedIndex);
			int increment = isLeft ? 1 : -1;
			bool shouldAdd = isLeft ? editableIndex < Selection.StartSelectionIndex : editableIndex > Selection.StartSelectionIndex;
			if (shouldAdd)
				Selection.SelectTokenByIndexWithUpdate(editableIndex);
			else
				Selection.RemoveTokenFromSelection(editableIndex + increment);
			Selection.LockSelection(() => SetFocusedTokenByIndexWithLock(newSelectedIndex, false));
		}
		bool ScrollRightAndDoAction(int index, Action action) {
			bool isScroll = ScrollRightByIndex(index);
			ImmediateActionsManager.EnqueueAction(() => action.Invoke());
			return isScroll;
		}
		bool ScrollRightAndMoveFocusedToken(int index, bool shouldActivate = false) {
			return ScrollRightAndDoAction(index, () => SetFocusedTokenByIndexWithLock(index, shouldActivate));
		}
		bool ScrollRightAndSelectToken(int newSelectedIndex) {
			return ScrollRightAndDoAction(newSelectedIndex, () => SelectTokenByKeyboard(newSelectedIndex, false));
		}
		TokenEditorPresenter GetTokenByVisibleIndex(int visibleIndex) {
			return tokenEditorPanel.GetTokenByVisibleIndex(visibleIndex);
		}
		void SetFocusedAndActivateTokenByIndex(int index, bool activate = true) {
			if (IsDefaultTokenIndex(index)) SetFocusedAndActivateToken(DefaultToken);
			else {
				var container = GetTokenByVisibleIndex(index);
				if (container != null) {
					if (activate)
						SetFocusedAndActivateToken(container);
					else
						FocusedToken = container;
				}
			}
		}
		bool IsDefaultToken(TokenEditorPresenter container) {
			return container == DefaultToken;
		}
		bool ContainsBounds(Rect bounds) {
			return tokenEditorPanel.GetBounds().Contains(bounds);
		}
		void UpdateFocusedTokenPresenter() {
			FocusedToken = ActiveEditor != null ? LayoutHelper.FindLayoutOrVisualParentObject<TokenEditorPresenter>(ActiveEditor) : null;
		}
		void UpdateTokens() {
			UpdatePresenterItems(GetInnerEditValue());
		}
		void AfterTokenRemoved(TokenEditorPresenter token) {
			editValueLocker.DoLockedAction(RaiseValueChanged);
			UpdateTokens();
			if (!IsDefaultTokenFocused()) {
				if (HasActiveEditor)
					FocusedToken.CommitEditor();
				ResetFocusedToken();
				int index = tokenEditorPanel.GetVisibleIndex(token);
				int itemsCount = tokenEditorPanel.Items.Count;
				if (itemsCount == 0)
					index = tokenEditorPanel.GetVisibleIndex(DefaultToken);
				SetFocusedAndActivateTokenByIndex(index, false);
			}
			if (FocusedToken != null)
				FocusedToken.FocusEditCore();
		}
		void RemoveToken(TokenEditorPresenter token) {
			RemoveTokenByIndex(EditableIndexOfToken(token));
			AfterTokenRemoved(token);
		}
		void RemoveTokenByIndex(int index) {
			var listEditValue = EditValueInternal.EditValue as IList<CustomItem>;
			if (listEditValue != null) {
				if (index > -1 && index < listEditValue.Count) {
					listEditValue.RemoveAt(index);
					UpdateEditValueInternalAfterRemove(listEditValue);
				}
			}
		}
		private void UpdateEditValueInternalAfterRemove(IList<CustomItem> listEditValue) {
			if (EditValueInternal != null) {
				EditValueInternal.Index = -1;
				if (listEditValue.Count == 0)
					EditValueInternal.EditValue = null;
				else
					EditValueInternal.EditValue = new List<CustomItem>(listEditValue);
			}
		}
		void RemoveTokens(List<int> indexes) {
			var listEditValue = EditValueInternal.EditValue as IList<CustomItem>;
			if (listEditValue != null) {
				var originValue = new List<CustomItem>(listEditValue);
				foreach (int index in indexes)
					listEditValue.Remove(originValue[index]);
				UpdateEditValueInternalAfterRemove(listEditValue);
			}
		}
		void OnFocusedTokenChanged(TokenEditorPresenter newFocused) {
			Selection.SetSelectionFromFocusedToken();
			UpdateTokenEditorsFocused(newFocused);
		}
		void ResetFocusedToken() {
			ActiveEditor = null;
			FocusedToken = null;
		}
		void TokenEditorLayoutUpdated(object sender, EventArgs e) {
			ImmediateActionsManager.ExecuteActions();
		}
		void SetFocusedAndActivateToken(TokenEditorPresenter container) {
			if (container == DefaultToken && !ShowDefaultToken) return;
			FocusedToken = container;
			ActivateToken(FocusedToken);
		}
		void ProcessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			if (tokenEditorPanel != null) {
				var hitTest = VisualTreeHelper.HitTest(tokenEditorPanel, e.GetPosition(tokenEditorPanel));
				var visualHit = hitTest.Return(x => x.VisualHit, () => null);
				bool isButton = LayoutHelper.FindLayoutOrVisualParentObject<Button>(visualHit) != null;
				var token = LayoutHelper.FindLayoutOrVisualParentObject<TokenEditorPresenter>(visualHit);
				if (token != null) {
					if (KeyboardHelper.IsCtrlPressed() && !IsDefaultToken(token) && !isButton) {
						if (HasActiveEditor) {
							FocusedToken.CommitEditor();
							FocusedToken.IsEditorActivated = false;
						}
						Selection.SelectToken(token);
						Selection.LockSelection(() => FocusedToken = token);
					}
					else {
						if (FocusedToken != token && IsDefaultTokenFocused())
							ClearAndRemoveDefaultTokenValue(!isButton);
						if (!isButton && !token.IsEditorActivated)
							ChangeFocusedTokenOnMouseDown(token, e);
					}
				}
			}
		}
		void ChangeFocusedTokenOnMouseDown(TokenEditorPresenter token, MouseButtonEventArgs e) {
			try {
				tokenEditorPanel.LockBringIntoView = true;
				FocusedToken = token;
				ActivateToken(FocusedToken, false);
				lockDefaultTokenActivation.DoLockedAction(() => CellEditorOwner.ProcessMouseLeftButtonDown(e));
			}
			finally {
				tokenEditorPanel.LockBringIntoView = false;
				ImmediateActionsManager.EnqueueAction(() => tokenEditorPanel.BringIntoView(FocusedToken));
			}
		}
		void UpdateTokenEditorsFocused(TokenEditorPresenter newFocused) {
			var editors = GetInplaceEditorContainers();
			foreach (var edit in editors)
				((TokenEditorPresenter)edit).IsTokenFocused = (edit == newFocused);
		}
		List<UIElement> GetInplaceEditorContainers() {
			return tokenEditorPanel.GetInplaceEditorContainers();
		}
		void OnEditValueChanged(object oldValue, object newValue) {
			if (tokenEditorPanel != null) {
				ImmediateActionsManager.EnqueueAction(() => tokenEditorPanel.EnsureOffset());
			}
		}
		Action postponeSyncWithValue;
		public void SyncWithValue(UpdateEditorSource updateSource) {
			if (updateSource != UpdateEditorSource.TextInput)
				SyncWithValueCore();
			else 
				postponeSyncWithValue = new Action(SyncWithValueCore);
		}
		private void SyncWithValueCore() {
			if (HasActiveEditor)
				CommitActiveEditor();
			UpdateTokens();
		}
		void UpdateActiveEditorValue(object newValue) {
			UpdateActiveEditorValueCore(newValue);
		}
		void UpdateActiveEditorValueCore(object newValue) {
			var listEditValue = GetInnerEditValue();
			if (listEditValue != null && (EditableValueIndex > -1 || IsDefaultTokenFocused())) {
				UpdateActiveEditorContent(listEditValue);
			}
		}
		IList<CustomItem> GetInnerEditValue() {
			return EditValueInternal.Return(x => x.EditValue as IList<CustomItem>, () => null);
		}
		void UpdateActiveEditorContent(IList<CustomItem> listEditValue) {
			processEditValueLocker.DoLockedAction(() => {
				var item = GetValueForEditableItem(listEditValue);
				if (!IsInProcessNewValue) SetIsInProcessValue(FocusedToken.Item.EditValue, item.EditValue);
				FocusedToken.Item = item;
			});
		}
		void SetIsInProcessValue(object oldValue, object newValue) {
			IsInProcessNewValue = oldValue != newValue;
		}
		CustomItem GetValueForEditableItem(IList<CustomItem> listEditValue) {
			CustomItem value = EditableValueIndex > -1 && EditableValueIndex < listEditValue.Count ? listEditValue[EditableValueIndex] : null;
			return value != null ? value : new CustomItem();
		}
		void UpdatePresenterItems(IList<CustomItem> items) {
			if (tokenEditorPanel != null) {
				var oldValue = tokenEditorPanel.DisplayItems;
				var newValue = items != null ? new List<CustomItem>(items) : new List<CustomItem>();
				tokenEditorPanel.DisplayItems = newValue;
			}
		}
		void CreateTokenEditorPanel() {
			var viewer = LayoutHelper.FindElementByName(this, "PART_ScrollViewer") as ScrollViewer;
			if (viewer != null) {
				tokenEditorPanel = viewer.Content as TokenEditorPanel;
				tokenEditorPanel.Owner = this;
				tokenEditorPanel.ScrollOwner = viewer;
				viewer.ScrollChanged += OnScrollChanged;
				UpdateTokens();
			}
		}
		void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
			Selection.UpdateSelectedTokens();
		}
		void TokenEditorLoaded(object sender, RoutedEventArgs e) {
			UpdateTokens();
		}
		InplaceEditorOwnerBase CreateCellEditorOwner() {
			return new CellEditorOwner(this);
		}
		void SetProperty(DependencyProperty dependencyProperty, object newValue) {
			SetCurrentValue(dependencyProperty, newValue);
		}
		void SetEditValueInternal(object newValue) {
			SetCurrentValue(EditValueProperty, newValue);
		}
		void RaiseTextChanged() {
			if (textChanged != null) {
				textChanged(this, EventArgs.Empty);
			}
		}
		void RaiseTokenClosed() {
			if (tokenClosed != null) {
				tokenClosed(this, EventArgs.Empty);
			}
		}
		void RaiseValueChanged() {
			if (valueChanged != null) {
				valueChanged(this, EventArgs.Empty);
			}
		}
		void OnTokenHidedCore() {
			ActiveEditor = null;
			bool shouldActivateDefaultToken = false;
			if (!changeFocusedTokenLocker.IsLocked) {
				if (HasFocusedToken)
					FocusedToken.HideEditor();
				if ((IsDefaultTokenFocused() || FocusedToken == null)) {
					if (EditValueInternal != null && EditValueInternal.Index != -1) {
						var index = EditValueInternal.Index;
						var listValue = EditValueInternal.EditValue as IList<CustomItem>;
						var value = listValue[index] as CustomItem;
						if (value == null || (value.EditValue == null && string.IsNullOrEmpty(value.DisplayText))) {
							List<CustomItem> newValue = new List<CustomItem>(listValue);
							newValue.RemoveAt(index);
							EditValueInternal.EditValue = newValue;
						}
					}
					UpdateDefaultToken();
					shouldActivateDefaultToken = !IsPopupCloseInProgress;
				}
			}
			ResetEditableIndex();
			UpdateTokens();
			RaiseTokenClosed();
			if (shouldActivateDefaultToken) {
				ImmediateActionsManager.EnqueueAction(ActivateDefaultToken);
			}
		}
		void ResetEditableIndex() {
			if (EditValueInternal != null)
				EditValueInternal.Index = -1;
		}
		void UpdateDefaultToken() {
			tokenEditorPanel.ClearDefaultTokenValue();
		}
		void InvalidateLayout() {
			InvalidateMeasure();
		}
		bool ShouldAssignNullText() {
			if (DefaultToken == null || DefaultToken.ItemData == null) return false;
			var data = DefaultToken.ItemData;
			return GetNullText() != data.Settings.NullText;
		}
		bool ShouldClearInputText(object value) {
			return HasActiveEditor && value == null && EditValue == null;
		}
		bool IsFocusWithinParent(DependencyObject child) {
			var parent = LayoutHelper.FindParentObject<LookUpEditBase>(this);
			return parent != null && child != null && LayoutHelper.IsChildElement(parent, child);
		}
		bool IsFocusFromCellEditor(IInputElement element) {
			return element != this && LayoutHelper.FindLayoutOrVisualParentObject<TokenEditor>((DependencyObject)element, true) == this;
		}
		void ActivateDefaultToken() {
			if (!FocusHelper.IsKeyboardFocusWithin(this) || DefaultToken == null || DefaultToken.IsEditorActivated) return;
			ActivateDefaultTokenCore();
		}
		void ActivateDefaultTokenCore() {
			if (DefaultToken == null || !ShowDefaultToken) return;
			if (IsDefaultTokenFocused()) {
				ActivateToken(DefaultToken);
			}
			else
				SetFocusedAndActivateToken(DefaultToken);
		}
		bool IsSingleSelection() {
			return LookUpEditHelper.GetIsSingleSelection(OwnerEdit);
		}
		void ActivateToken(TokenEditorPresenter token, bool showEditor = true) {
			if (CanActivateToken()) {
				tokenEditorPanel.BringIntoView(token);
				token.IsEditorActivated = true;
				if (showEditor) {
					token.Editor.Do(x => x.ShowEditor(true));
					token.FocusEditCore();
				}
			}
		}
		bool IsDefaultTokenIndex(int visibleIndex) {
			return tokenEditorPanel.IsDefaultTokenIndex(visibleIndex);
		}
		#region IScrollBarThumbDragDeltaListener Members
		ScrollBar IScrollBarThumbDragDeltaListener.ScrollBar { get; set; }
		Orientation IScrollBarThumbDragDeltaListener.Orientation { get { return tokenEditorPanel != null ? tokenEditorPanel.Orientation : Orientation.Horizontal; } }
		void IScrollBarThumbDragDeltaListener.OnScrollBarThumbDragDelta(DragDeltaEventArgs e) {
			if (tokenEditorPanel != null)
				tokenEditorPanel.OnThumbDragDelta(e);
		}
		void IScrollBarThumbDragDeltaListener.OnScrollBarThumbMouseMove(MouseEventArgs e) {
			if (tokenEditorPanel != null)
				tokenEditorPanel.OnThumbMouseMove(e);
		}
		#endregion
	}
	public class TokenItemData : EditableDataObject {
		static TokenItemData() {
			Type ownerType = typeof(TokenItemData);
		}
		public TokenItemData(TokenEditor owner) {
			Owner = owner;
			Settings = CreateSettings();
		}
		EditorColumn column;
		public EditorColumn Column { get { return column ?? (column = new EditorColumn(this)); } }
		public ButtonEditSettings Settings { get; private set; }
		public string DisplayText { get; set; }
		public void UpdateEditSettings() {
			AssignSettings(Settings);
			Column.RaiseContentChanged();
		}
		TokenEditor Owner { get; set; }
		private ButtonEditSettings CreateSettings() {
			var settings = new ButtonEditSettings();
			AssignSettings(settings);
			return settings;
		}
		private void AssignSettings(ButtonEditSettings settings) {
			settings.AllowNullInput = true;
		}
	}
	public class EditorColumn : IInplaceEditorColumn {
		public EditorColumn(TokenItemData owner) {
			OwnerItem = owner;
		}
		TokenItemData OwnerItem { get; set; }
		#region IInplaceEditorColumn Members
		BaseEditSettings IInplaceEditorColumn.EditSettings {
			get { return OwnerItem.Settings; }
		}
		DataTemplateSelector IInplaceEditorColumn.EditorTemplateSelector {
			get { return null; }
		}
		ControlTemplate IInplaceEditorColumn.EditTemplate {
			get { return null; }
		}
		ControlTemplate IInplaceEditorColumn.DisplayTemplate {
			get { return null; }
		}
		ColumnContentChangedEventHandler contentChanged;
		event ColumnContentChangedEventHandler IInplaceEditorColumn.ContentChanged {
			add { contentChanged += value; }
			remove { contentChanged -= value; }
		}
		#endregion
		#region IDefaultEditorViewInfo Members
		HorizontalAlignment IDefaultEditorViewInfo.DefaultHorizontalAlignment {
			get { return HorizontalAlignment.Stretch; }
		}
		bool IDefaultEditorViewInfo.HasTextDecorations { get { return false; } }
		#endregion
		internal void RaiseContentChanged() {
			if (contentChanged != null)
				contentChanged(this, new ColumnContentChangedEventArgs(null));
		}
	}
	public class ReraiseAction {
		public ReraiseAction(Action action, Func<bool> canExecute, ImmediateActionsManager manager) {
			Action = action;
			CanExecute = canExecute;
			Manager = manager;
		}
		Action Action { get; set; }
		Func<bool> CanExecute { get; set; }
		ImmediateActionsManager Manager { get; set; }
		public void Perform() {
			if (CanExecute()) {
				Action();
				Action = null;
			}
			else
				Manager.EnqueueAction(GetPerformAction());
		}
		Action GetPerformAction() {
			return new Action(() => this.Perform());
		}
	}
}
