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
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Editors.Internal {
	public class TokenEditorKeyboardHelper {
		public TokenEditorKeyboardHelper(TokenEditor tokenEditor) {
			TokenEditor = tokenEditor;
		}
		TokenEditor TokenEditor { get; set; }
		TextEdit ActivatedToken { get { return TokenEditor.ActiveEditor; } }
		TokenEditorPresenter FocusedToken { get { return TokenEditor.FocusedToken; } }
		bool HasActivatedToken { get { return ActivatedToken != null; } }
		TokenEditorPanel tokenPanel;
		TokenEditorPanel TokenPanel { get { return tokenPanel ?? CreateTokenPanel(); } }
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (key == Key.Enter) return true;
			if (IsDeleteTokenKey(key)) return true;
			if (TokenEditor.IsInProcessNewValue) return false;
			if (CanNavigateToLine(key, modifiers)) return true;
			if (!HasActivatedToken && IsActivatingKey(key, modifiers) && TokenEditor.CanActivateToken() && !IsLeftRightKey(key)) return true;
			if (TokenEditor.CanActivateToken() && HasActivatedToken) return NeedsNavigationKeyInEditableState(key, modifiers);
			else {
				if (IsShiftPressed(modifiers)) return true;
				if (IsLeftNavigationKey(key)) return IsFirstLeftTokenNotFocused();
				if (IsRightNavigationKey(key)) return IsLastRightTokenNotFocused();
				return false;
			}
		}
		public bool IsCtrlPressed() {
			return ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers);
		}
		public bool ProcessPreviewKeyDown(KeyEventArgs e) {
			if (IsDeleteTokenKey(e.Key)) {
				ProcessDeleteTokenKey(e);
				return e.Handled;
			}
			if (CanNavigateToLine(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e)) && NavigateUpDown(e)) e.Handled = true;
			else if (!HasActivatedToken) {
				if (NavigateLeftRight(e)) e.Handled = true;
				if (!e.Handled && TokenEditor.CanActivateToken() && (IsActivatingKey(e.Key, ModifierKeysHelper.GetKeyboardModifiers(e)) && !IsUpOrDownKey(e.Key) && !IsLeftRightKey(e.Key)))
					FocusedToken.ProcessActivatingKey(e);
			}
			else if (!TokenEditor.IsInProcessNewValue) {
				bool isDefaultFocused = TokenEditor.IsDefaultTokenFocused();
				int index = TokenEditor.EditValueInternal.Index;
				e.Handled = NavigateLeftRight(e);
				if (e.Handled && isDefaultFocused && !TokenEditor.IsInProcessNewValue) {
					TokenEditor.RemoveValueByIndex(index);
				}
			}
			return e.Handled;
		}
		bool IsRightToLeft() {
			return TokenEditor.FlowDirection == System.Windows.FlowDirection.RightToLeft;
		}
		TokenEditorPanel CreateTokenPanel() {
			tokenPanel = LayoutHelper.FindElementByType<TokenEditorPanel>(TokenEditor);
			return tokenPanel;
		}
		bool IsDeleteTokenKey(Key key) {
			return (!HasActivatedToken && key == Key.Delete) || (IsTokenTextEmpty() && (key == Key.Back || key == Key.Delete));
		}
		bool IsTokenTextEmpty() {
			return ActivatedToken != null && string.IsNullOrEmpty(ActivatedToken.Text);
		}
		bool IsRightNavigationKey(Key key) {
			return key == Key.Right || key == Key.End;
		}
		bool IsLeftNavigationKey(Key key) {
			return key == Key.Left || key == Key.Home;
		}
		bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			if (FocusedToken != null)
				return !TokenEditor.IsReadOnly && FocusedToken.IsActivatingKey(key, modifiers);
			return false;
		}
		bool IsUpOrDownKey(Key key) {
			return key == Key.Up || key == Key.Down;
		}
		bool IsLeftRightKey(Key key) {
			return key == Key.Left || key == Key.Right;
		}
		bool CanNavigateToLine(Key key, ModifierKeys modifiers) {
			if (!TokenEditor.EnableTokenWrapping) return false;
			bool upDownKey = IsUpOrDownKey(key);
			if (HasActivatedToken) return IsCtrlPressed() && upDownKey;
			return upDownKey;
		}
		bool IsFirstLineVisible() {
			return TokenPanel.IsFirstLineVisible();
		}
		bool IsEndLineVisible() {
			return TokenPanel.IsEndLineVisible();
		}
		bool IsFocusedEditorInEndLine() {
			return TokenPanel.IsTokenInEndLine(FocusedToken);
		}
		bool IsFocusedEditorInFirstLine() {
			return TokenPanel.IsTokenInFirstLine(FocusedToken);
		}
		bool IsLastRightTokenNotFocused() {
			return FocusedToken == null || (!IsRightToLeft() ? !IsLastTokenFocused() : !IsFirstTokenFocused());
		}
		bool IsFirstLeftTokenNotFocused() {
			return FocusedToken == null || (!IsRightToLeft() ? !IsFirstTokenFocused() : !IsLastTokenFocused());
		}
		bool IsLastTokenFocused() {
			return FocusedToken != null && TokenPanel.IsLastToken(FocusedToken);
		}
		bool IsFirstTokenFocused() {
			return FocusedToken != null && TokenPanel.IsFirstToken(FocusedToken);
		}
		bool NeedsNavigationKeyInEditableState(Key key, ModifierKeys modifiers) {
			if (key == Key.Left || key == Key.Home) return CanNavigateLeft();
			if (key == Key.Right || key == Key.End) return CanNavigateRight();
			return false;
		}
		bool CanNavigateRight() {
			return IsLastRightTokenNotFocused() && CanNavigateRightInActiveMode();
		}
		bool CanNavigateLeft() {
			return IsFirstLeftTokenNotFocused() && CanNavigateLeftInActiveMode();
		}
		bool CanNavigateLeftInActiveMode() {
			return ActivatedToken.CaretIndex == 0 || ActivatedToken.Text.Equals(ActivatedToken.SelectedText);
		}
		bool CanNavigateRightInActiveMode() {
			return ActivatedToken.CaretIndex == ActivatedToken.Text.Length || ActivatedToken.Text.Equals(ActivatedToken.SelectedText);
		}
		internal void ProcessDeleteTokenKey(System.Windows.Input.KeyEventArgs e) {
			e.Handled = true;
			if (TokenEditor.HasSelection)
				TokenEditor.RemoveSelectedTokens();
			else if (TokenEditor.IsDefaultTokenFocused()) {
				int index = -1;
				if ((e.Key == Key.Back && TokenEditor.ShowNewTokenFromEnd && !IsRightToLeft()) || (e.Key == Key.Delete && TokenEditor.ShowNewTokenFromEnd && IsRightToLeft()))
					index = TokenPanel.MaxVisibleIndex - 1;
				else if ((e.Key == Key.Back && !TokenEditor.ShowNewTokenFromEnd && IsRightToLeft()) || (e.Key == Key.Delete && !TokenEditor.ShowNewTokenFromEnd && !IsRightToLeft()))
					index = TokenPanel.MinVisibleIndex + 1;
				if (index > -1) {
					TokenEditor.RemoveToken(index);
				}
			}
		}
		bool NavigateUpDown(KeyEventArgs e) {
			var modifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			switch (e.Key) {
				case Key.Up:
					return NavigateUp(modifiers);
				case Key.Down:
					return NavigateDown(modifiers);
			}
			return false;
		}
		bool NavigateDown(ModifierKeys modifiers) {
			if (!IsFocusedEditorInEndLine()) {
				TokenEditor.OnNavigateDown(IsShiftPressed(modifiers));
			}
			return true;
		}
		bool NavigateUp(ModifierKeys modifiers) {
			if (!IsFocusedEditorInFirstLine()) {
				TokenEditor.OnNavigateUp(IsShiftPressed(modifiers));
			}
			return true;
		}
		bool NavigateLeftRight(System.Windows.Input.KeyEventArgs e) {
			var modifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			switch (e.Key) {
				case Key.Left:
					return !IsRightToLeft() ? NavigateLeft(modifiers) : NavigateRight(modifiers);
				case Key.Right:
					return !IsRightToLeft() ? NavigateRight(modifiers) : NavigateLeft(modifiers);
				case Key.Home:
					return !IsRightToLeft() ? NavigateHome() : NavigateEnd(); ;
				case Key.End:
					return !IsRightToLeft() ? NavigateEnd() : NavigateHome();
			}
			return false;
		}
		bool NavigateEnd() {
			if (TokenPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
				return NavigateEndHorizontal();
			else
				return NavigateEndVertical();
		}
		private bool NavigateEndVertical() {
			TokenEditor.NavigateToVerticalEnd();
			return true;
		}
		private bool NavigateEndHorizontal() {
			if (IsLastTokenFocused() && !TokenEditor.ShowNewTokenFromEnd) return false;
			if (HasActivatedToken)
				return CanNavigateRightInActiveMode() ? TokenEditor.OnNavigateHorizontalEnd(true) : false;
			else
				return TokenEditor.OnNavigateHorizontalEnd(false);
		}
		bool NavigateHome() {
			if (TokenPanel.Orientation == System.Windows.Controls.Orientation.Horizontal)
				return NavigateHomeHorizontal();
			else
				return NavigateHomeVertical();
		}
		bool NavigateHomeVertical() {
			TokenEditor.NavigateToVerticalStart();
			return true;
		}
		bool NavigateHomeHorizontal() {
			if (IsFirstTokenFocused() && TokenEditor.ShowNewTokenFromEnd) return false;
			if (HasActivatedToken)
				return CanNavigateLeftInActiveMode() ? TokenEditor.OnNavigateHorizontalStart(true) : false;
			else
				return TokenEditor.OnNavigateHorizontalStart(false);
		}
		bool NavigateRight(ModifierKeys modifiers) {
			bool shiftPressed = IsShiftPressed(modifiers);
			int index = FocusedToken != null ? TokenPanel.GetVisibleIndex(FocusedToken) + 1 : TokenEditor.GetTokenByHorizontalOffset(); ;
			return NavigateRightCore(index, shiftPressed) || shiftPressed;
		}
		private bool IsShiftPressed(ModifierKeys modifiers) {
			return ModifierKeysHelper.IsShiftPressed(modifiers);
		}
		bool NavigateRightCore(int index, bool isShiftPressed = false) {
			if (IsLastTokenFocused() && !TokenEditor.ShowNewTokenFromEnd) return false;
			if (HasActivatedToken)
				return (index < TokenPanel.Items.Count + 1 && CanNavigateRightInActiveMode()) ? TokenEditor.OnNavigateRight(index, true) : false;
			else
				return isShiftPressed ? TokenEditor.SelectTokenOnNavigateRight(index) : TokenEditor.OnNavigateRight(index, false);
		}
		bool NavigateLeft(ModifierKeys modifiers) {
			bool shiftPressed = IsShiftPressed(modifiers);
			int index = FocusedToken != null ? TokenPanel.GetVisibleIndex(FocusedToken) - 1 : TokenEditor.GetTokenByHorizontalOffset();
			return NavigateLeftCore(index, shiftPressed) || shiftPressed;
		}
		bool NavigateLeftCore(int index, bool isShiftPressed = false) {
			if (IsFirstTokenFocused() && TokenEditor.ShowNewTokenFromEnd) return false;
			if (HasActivatedToken)
				return CanNavigateLeftInActiveMode() ? TokenEditor.OnNavigateLeft(index, true) : false;
			else
				return isShiftPressed ? TokenEditor.SelectTokenOnNavigateLeft(index) : TokenEditor.OnNavigateLeft(index, false);
		}
		bool ShouldScroll(int index) {
			return TokenPanel.ShouldScroll(index);
		}
	}
}
