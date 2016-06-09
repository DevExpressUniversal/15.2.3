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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class TokenEditorWrapper : EditBoxWrapper {
		public TokenEditorWrapper(LookUpEditBase editor) : base(editor) { }
		TokenEditor EditBox { get { return Editor.EditCore as TokenEditor; } }
		new LookUpEditBase Editor { get { return base.Editor as LookUpEditBase; } }
		bool HasEditBox { get { return EditBox != null; } }
		public override Brush Foreground {
			get { return HasEditBox ? EditBox.Foreground : null; }
		}
		public override int LineCount {
			get { return 1; }
		}
		public override string SelectedText {
			get { return HasEditBox ? EditBox.SelectedText : string.Empty; }
			set { if (HasEditBox) EditBox.SelectedText = value; }
		}
		public override int SelectionLength {
			get { return HasEditBox ? EditBox.SelectionLength : 0; }
			set { if (HasEditBox) EditBox.SelectionLength = value; }
		}
		public override int SelectionStart {
			get { return HasEditBox ? EditBox.SelectionStart : 0; }
			set { if (HasEditBox) EditBox.SelectionStart = value; }
		}
		public override int CaretIndex {
			get { return HasEditBox ? EditBox.CaretIndex : -1; }
			set { if (HasEditBox) EditBox.CaretIndex = value; }
		}
		public override bool CanUndo {
			get { return HasEditBox && EditBox.CanUndo; }
		}
		public override int MaxLength {
			get { return HasEditBox ? EditBox.MaxLength : 0; }
			set { if (HasEditBox) EditBox.MaxLength = value; }
		}
		public override CharacterCasing CharacterCasing {
			get { return HasEditBox ? EditBox.CharacterCasing : System.Windows.Controls.CharacterCasing.Normal; }
			set { if (HasEditBox) EditBox.CharacterCasing = value; }
		}
		public override string Text {
			get { return HasEditBox ? EditBox.Text : string.Empty; }
		}
		public override object EditValue {
			get {
				if (EditBox != null) return EditBox.EditValue;
				return null;
			}
			set {
				if (EditBox != null) EditBox.SetEditValue(value);
			}
		}
		public override void SyncWithValue(UpdateEditorSource updateSource) {
			base.SyncWithValue(updateSource);
			if (HasEditBox)
				EditBox.SyncWithValue(updateSource);
		}
		public override int GetCharacterIndexFromLineIndex(int lineIndex) {
			return HasEditBox ? EditBox.GetCharacterIndexFromLineIndex(lineIndex) : -1;
		}
		public override int GetCharacterIndexFromPoint(Point point, bool snapToText) {
			return HasEditBox ? EditBox.GetCharacterIndexFromPoint(point, snapToText) : -1;
		}
		public override int GetFirstVisibleLineIndex() {
			return HasEditBox ? EditBox.GetFirstVisibleLineIndex() : -1;
		}
		public override int GetLastVisibleLineIndex() {
			return HasEditBox ? EditBox.GetLastVisibleLineIndex() : -1;
		}
		public override int GetLineIndexFromCharacterIndex(int charIndex) {
			return HasEditBox ? EditBox.GetLineIndexFromCharacterIndex(charIndex) : -1;
		}
		public override int GetLineLength(int lineIndex) {
			return HasEditBox ? EditBox.GetLineLength(lineIndex) : 0;
		}
		public override string GetLineText(int lineIndex) {
			return HasEditBox ? EditBox.GetLineText(lineIndex) : string.Empty;
		}
		public override void Select(int start, int length) {
			if (HasEditBox)
				EditBox.Select(start, length);
		}
		public override void ScrollToHome() {
			if (HasEditBox)
				EditBox.ScrollToHome();
		}
		public override void SelectAll() {
			if (HasEditBox)
				EditBox.SelectAll();
		}
		public override void UnselectAll() {
			if (HasEditBox)
				EditBox.SelectedText = string.Empty;
		}
		public override void Copy() {
			if (HasEditBox)
				EditBox.Copy();
		}
		public override void Cut() {
			if (HasEditBox)
				EditBox.Cut();
		}
		public override void Undo() {
			if (HasEditBox)
				EditBox.Undo();
		}
		public override void Paste() {
			if (HasEditBox)
				EditBox.Paste();
		}
		public override bool IsReadOnly {
			get { return HasEditBox && EditBox.IsReadOnly; }
			set { if (HasEditBox) EditBox.IsReadOnly = value; }
		}
		public override bool IsUndoEnabled {
			get { return HasEditBox && EditBox.IsUndoEnabled; }
			set { if (HasEditBox) EditBox.IsUndoEnabled = value; }
		}
		protected override FrameworkElement GetEditBox() {
			return EditBox;
		}
		public override bool NeedsKey(Key key, ModifierKeys modifiers) {
			return NeedsNavigationKey(key, modifiers) || (NeedsKeyInInplaceActiveMode(key, modifiers) ||NeedsEnterInInplaceActiveMode(key) || NeedsActivationKeyInInactiveMode(key, modifiers));
		}
		private bool NeedsActivationKeyInInactiveMode(Key key, ModifierKeys modifiers) {
			return EditBox.ActiveEditor == null && EditBox.NeedsActivationKeyInInactiveMode(key, modifiers);
		}
		private bool NeedsEnterInInplaceActiveMode(Key key) {
			return NeedsEnterKey() && key == Key.Enter;
		}
		private bool NeedsKeyInInplaceActiveMode(Key key, ModifierKeys modifiers) {
			return ((IsInActiveMode()) && EditBox.ActiveEditor != null && EditBox.ActiveEditor.NeedsKey(key, modifiers));
		}
		private bool IsInActiveMode() {
			return Editor.EditMode == EditMode.InplaceActive;
		}
		public override bool NeedsNavigationKey(Key key, ModifierKeys modifiers) {
			return HasEditBox && EditBox.NeedsKey(key, modifiers);
		}
		public override bool NeedsEnterKey() {
			return IsInActiveMode() && EditBox.IsValueChanged();
		}
		public override bool ProccessKeyDown(KeyEventArgs e) {
			if (HasEditBox)
				return EditBox.ProcessKeyDown(e);
			return false;
		}
		public override void AfterAcceptPopupValue() {
			if (HasEditBox)
				EditBox.AfterAcceptPopupValue();
		}
		public override void BeforeAcceptPopupValue() {
			if (HasEditBox)
				EditBox.BeforeAcceptPopupValue();
		}
	}
}
