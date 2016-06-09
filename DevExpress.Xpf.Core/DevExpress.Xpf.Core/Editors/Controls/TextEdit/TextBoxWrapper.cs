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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Mvvm.Native;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core.Internal;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors {
	public enum HighlightedTextCriteria {
		StartsWith,
		Contains,
	}
	public abstract class EditBoxWrapper {
		string highlightedText = string.Empty;
		bool allowDrop = false;
		HighlightedTextCriteria highlightedTextCriteria;
		bool showHighlightedText;
		protected TextEditBase Editor { get; set; }
		public EditBoxWrapper(TextEditBase editor) {
			Editor = editor;
		}
		public virtual bool AllowDrop {
			get { return allowDrop; }
			set { allowDrop = value; }
		}
		public bool ShowHighlightedText {
			get { return showHighlightedText; }
			set {
				showHighlightedText = value;
				SetShowHighlightedTextInternal(value);
			}
		}
		public HighlightedTextCriteria HighlightedTextCriteria {
			get { return highlightedTextCriteria; }
			set {
				highlightedTextCriteria = value;
				SetHighlightedTextCriteriaInternal(value);
			}
		}
		public string HighlightedText {
			get { return highlightedText; }
			set {
				highlightedText = value;
				SetHighlightedTextInternal(value);
			}
		}
		protected virtual void SetShowHighlightedTextInternal(bool value) {
		}
		protected virtual void SetHighlightedTextInternal(string value) {
		}
		protected virtual void SetHighlightedTextCriteriaInternal(HighlightedTextCriteria value) {
		}
		public abstract Brush Foreground { get; }
		public abstract int LineCount { get; }
		public abstract string SelectedText { get; set; }
		public abstract int SelectionLength { get; set; }
		public abstract int SelectionStart { get; set; }
		public abstract int CaretIndex { get; set; }
		public abstract string Text { get; }
		public abstract object EditValue { get; set; }
		public abstract bool CanUndo { get; }
		public abstract int MaxLength { get; set; }
		public virtual void ClearUndoStack() {
		}
		public virtual void UpdateHighlighting() {}
		public virtual bool ProccessKeyDown(KeyEventArgs e) { return false; }
		public virtual void SyncWithValue(UpdateEditorSource updateSource) { }
		public virtual void BeforeAcceptPopupValue() { }
#if !SL
		public abstract CharacterCasing CharacterCasing { get; set; }
		public void AddPreviewExecutedHandler(ExecutedRoutedEventHandler handler) {
			CommandManager.AddPreviewExecutedHandler(Editor, handler);
		}
		public void RemovePreviewExecutedHandler(ExecutedRoutedEventHandler handler) {
			CommandManager.RemovePreviewExecutedHandler(Editor, handler);
		}
		public void AddCommandBinding(CommandBinding binding) {
			if (GetEditBox() == null)
				return;
			GetEditBox().CommandBindings.Add(binding);
		}
		public void RemoveCommandBinding(CommandBinding binding) {
			if (GetEditBox() == null)
				return;
			GetEditBox().CommandBindings.Remove(binding);
		}
#endif
		public void ExecuteCommand(RoutedCommand command, object parameter) {
			command.Execute(parameter, Editor);
		}
		public abstract int GetCharacterIndexFromLineIndex(int lineIndex);
		public abstract int GetCharacterIndexFromPoint(Point point, bool snapToText);
		public abstract int GetFirstVisibleLineIndex();
		public abstract int GetLastVisibleLineIndex();
		public abstract int GetLineIndexFromCharacterIndex(int charIndex);
		public abstract int GetLineLength(int lineIndex);
		public abstract string GetLineText(int lineIndex);
		public abstract void Select(int start, int length);
		public abstract void ScrollToHome();
		public abstract void SelectAll();
		public abstract void UnselectAll();
		public abstract void Copy();
		public abstract void Cut();
		public abstract void Undo();
		public abstract void Paste();
		public abstract bool IsReadOnly { get; set; }
		public abstract bool IsUndoEnabled { get; set; }
		public void ClearValue(DependencyProperty dependencyProperty) {
			if (GetEditBox() == null)
				return;
			GetEditBox().ClearValue(dependencyProperty);
		}
		protected abstract FrameworkElement GetEditBox();
#if SL
		public abstract void ProcessKeyUp(KeyEventArgs e);
#endif
		public abstract bool NeedsKey(Key key, ModifierKeys modifiers);
		public virtual bool NeedsNavigationKey(Key key, ModifierKeys modifiers) {
			return false;
		}
		public virtual void UpdateIsTextTrimming() {
		}
		public virtual void IsImeEnabled(bool value) {
		}
		public virtual bool NeedsEnterKey() {
			return false;
		}
		public virtual void AfterAcceptPopupValue() {}
	}
	public class TextBoxWrapper : EditBoxWrapper {
#if !SL
		static readonly ReflectionHelper reflectionHelper = new ReflectionHelper();
#endif
		new TextEdit Editor { get { return base.Editor as TextEdit; } }
		TextBox EditBox { get { return Editor.EditCore as TextBox; } }
		TextBlock EditBlock { get { return Editor.EditCore as TextBlock; } }
		public override bool CanUndo {
			get {
				if (EditBox != null)
					return EditBox.CanUndo;
				return false;
			}
		}
#if !SL
		public override bool AllowDrop {
			get {
				if (EditBox != null)
					return EditBox.AllowDrop;
				return base.AllowDrop;
			}
			set {
				if (EditBox != null) {
					EditBox.AllowDrop = value;
					return;
				}
				base.AllowDrop = value;
			}
		}
		public override CharacterCasing CharacterCasing {
			get {
				if (EditBox != null)
					return EditBox.CharacterCasing;
				return CharacterCasing.Normal;
			}
			set {
				if (EditBox != null)
					EditBox.CharacterCasing = value;
			}
		}
#endif
		public override int LineCount {
			get {
				if (EditBox != null)
					return EditBox.LineCount;
				if (EditBlock != null && !string.IsNullOrEmpty(EditBlock.Text))
					return EditBlock.Text.Split(Environment.NewLine.ToCharArray()).Length;
				return 0;
			}
		}
		public override bool IsReadOnly {
			get {
				if (EditBox != null)
					return EditBox.IsReadOnly;
				return false;
			}
			set {
				if (EditBox != null)
					EditBox.IsReadOnly = value;
			}
		}
		public override string Text {
			get {
				if (EditBox != null)
					return EditBox.Text;
				if (EditBlock != null)
					return EditBlock.Text;
				return string.Empty;
			}
		}
		public override object EditValue {
			get {
				if (EditBox != null)
					return EditBox.Text;
				if (EditBlock != null)
					return EditBlock.Text;
				return string.Empty;
			}
			set {
				if (EditBox == null && EditBlock == null)
					return;
				string text = (string)value;
				if (EditBlock != null) {
					SetEditBlockText(text, HighlightedText, HighlightedTextCriteria);
					return;
				}
				if (EditBox != null && EditBox.Text != text && IsEditBoxRendered) {
					EditBox.Text = text;
					return;
				}
			}
		}
		public override Brush Foreground {
			get {
				if (EditBox != null)
					return EditBox.Foreground;
				if (EditBlock != null)
					return EditBlock.Foreground;
				return null;
			}
		}
		public override string SelectedText {
			get { return EditBox != null ? EditBox.SelectedText : string.Empty; }
			set {
				if (EditBox != null)
					EditBox.SelectedText = value != null ? value : string.Empty;
			}
		}
		public override int SelectionLength {
			get {
				if (EditBox != null)
					return EditBox.SelectionLength;
				return 0;
			}
			set {
				if (EditBox != null && EditBox.SelectionLength != value)
					EditBox.SelectionLength = value;
			}
		}
		public override int SelectionStart {
			get {
				if (EditBox != null)
					return EditBox.SelectionStart;
				return 0;
			}
			set {
				if (EditBox != null && EditBox.SelectionStart != value)
					EditBox.SelectionStart = value;
			}
		}
		public override int MaxLength {
			get {
				if (EditBox != null)
					return EditBox.MaxLength;
				return -1;
			}
			set {
				if (EditBox != null)
					EditBox.MaxLength = value;
			}
		}
		public override int CaretIndex {
			get {
				if (EditBox != null)
					return EditBox.CaretIndex;
				return 0;
			}
			set {
				if (EditBox != null)
					EditBox.CaretIndex = value;
			}
		}
		public override bool IsUndoEnabled {
			get {
				if (EditBox != null)
					return EditBox.IsUndoEnabled;
				return false;
			}
			set {
				if (EditBox != null)
					EditBox.IsUndoEnabled = value;
			}
		}
		bool IsEditBlockRendered {
			get {
				if (EditBlock == null)
					return false;
				return EditBlock.DataContext != null;
			}
		}
		bool IsEditBoxRendered {
			get {
				if (EditBox == null)
					return false;
				return !(Editor.EditMode == EditMode.InplaceInactive && Editor.IsEditorActive);
			}
		}
		public TextBoxWrapper(TextEdit editor)
			: base(editor) {
		}
		public override int GetCharacterIndexFromLineIndex(int lineIndex) {
			if (EditBox != null)
				return EditBox.GetCharacterIndexFromLineIndex(lineIndex);
			return -1;
		}
		TextPointer GetTextPositionFromPointInternal(Point point, bool snapToText) {
			return reflectionHelper.GetInstanceMethodHandler<Func<TextBox, Point, bool, TextPointer>>(EditBox, "GetTextPositionFromPointInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, EditBox.GetType())(EditBox, point, snapToText);
		}
		public override int GetCharacterIndexFromPoint(Point point, bool snapToText) {
			if (EditBox != null) {
				var pointer = GetTextPositionFromPointInternal(point, snapToText);
				return pointer.Return(x => x.GetLineStartPosition(0).GetOffsetToPosition(x), () => -1);
			}
			return -1;
		}
		public override int GetFirstVisibleLineIndex() {
			if (EditBox != null)
				return EditBox.GetFirstVisibleLineIndex();
			return -1;
		}
		public override int GetLastVisibleLineIndex() {
			if (EditBox != null)
				return EditBox.GetLastVisibleLineIndex();
			return -1;
		}
		public override int GetLineIndexFromCharacterIndex(int charIndex) {
			if (EditBox != null)
				return EditBox.GetLineIndexFromCharacterIndex(charIndex);
			return -1;
		}
		public override int GetLineLength(int lineIndex) {
			if (EditBox != null)
				return EditBox.GetLineLength(lineIndex);
			return -1;
		}
		public override string GetLineText(int lineIndex) {
			if (EditBox != null)
				return EditBox.GetLineText(lineIndex);
			return null;
		}
		public override void Select(int start, int length) {
			if (EditBox != null && (EditBox.SelectionStart != start || EditBox.SelectionLength != length)) 
				EditBox.Select(start, length);
		}
		public override void ScrollToHome() {
#if !SL
			if (EditBox != null)
				EditBox.ScrollToHome();
#endif
		}
		public override void SelectAll() {
			if (EditBox != null)
				EditBox.SelectAll();
		}
		public override void UnselectAll() {
			if (EditBox != null)
				EditBox.SelectedText = string.Empty;
		}
		public override void Copy() {
			if (EditBox != null)
				EditBox.Copy();
		}
		public override void Cut() {
			if (EditBox != null)
				EditBox.Cut();
		}
		public override void Undo() {
			if (EditBox != null)
				EditBox.Undo();
		}
		public override void Paste() {
			if (EditBox != null)
				EditBox.Paste();
		}
		protected override void SetHighlightedTextInternal(string value) {
			base.SetHighlightedTextInternal(value);
			UpdateHighlighting(HighlightedTextCriteria, value);
		}
		protected override void SetHighlightedTextCriteriaInternal(HighlightedTextCriteria value) {
			base.SetHighlightedTextCriteriaInternal(value);
			UpdateHighlighting(value, HighlightedText);
		}
		void UpdateHighlighting(HighlightedTextCriteria criteria, string highlightedText) {
			if (EditBlock == null)
				return;
			SetEditBlockText(string.IsNullOrEmpty(editBlockTextForHighlighting) ? EditBlock.Text : editBlockTextForHighlighting, highlightedText, criteria);
		}
		public override void UpdateHighlighting() {
			UpdateHighlighting(HighlightedTextCriteria, HighlightedText);
		}
		protected override void SetShowHighlightedTextInternal(bool value) {
			base.SetShowHighlightedTextInternal(value);
		}
#if SL
		public override void ProcessKeyUp(KeyEventArgs e) {
			if(EditBox != null)
				EditBox.ProcessKeyUp(e);
		}
#endif
		protected override FrameworkElement GetEditBox() {
			return EditBox;
		}
		public override bool NeedsKey(Key key, ModifierKeys modifiers) {
			return TextBoxHelper.NeedKey(Editor.EditCore as TextBox, key);
		}
		public override void ClearUndoStack() {
			base.ClearUndoStack();
			if (EditBox != null) {
				try {
					bool undo = EditBox.IsUndoEnabled;
					EditBox.IsUndoEnabled = false;
					EditBox.IsUndoEnabled = undo;
				}
				catch {
				}
			}
		}
		string editBlockTextForHighlighting;
		void SetEditBlockText(string value, string highlightedText, HighlightedTextCriteria criteria) {
			if (SetupHighlighting()) {
				editBlockTextForHighlighting = value;
				SetTextInfo(value, highlightedText, criteria);
				return;
			}
			editBlockTextForHighlighting = null;
			if (TextBlockService.GetTextInfo(EditBlock) != null) {
				SetTextInfo(value, highlightedText, criteria);
				return;
			}
			EditBlock.Text = string.IsNullOrEmpty(value) ? " " : value;
		}
		bool SetupHighlighting() {
			if (string.IsNullOrEmpty(HighlightedText))
				return false;
			return IsEditBlockRendered;
		}
		void SetTextInfo(string value, string highlightedText, HighlightedTextCriteria criteria) {
			TextBlockInfo info = new TextBlockInfo() {
				Text = value,
				HighlightedText = highlightedText,
				HighlightedTextCriteria = criteria
			};
			if (info == TextBlockService.GetTextInfo(EditBlock))
				return;
			TextBlockService.SetTextInfo(EditBlock, info);
		}
		public override void IsImeEnabled(bool value) {
			base.IsImeEnabled(value);
#if !SL
			if (EditBox != null) {
				InputMethod.SetIsInputMethodEnabled(EditBox, value);
			}
#endif
		}
	}
	public class PasswordBoxWrapper : EditBoxWrapper {
		new PasswordBoxEdit Editor { get { return base.Editor as PasswordBoxEdit; } }
		PasswordBox EditBox { get { return Editor.PasswordBox; } }
		public override int LineCount { get { return 1; } }
		public override bool CanUndo { get { return false; } }
		public override bool IsUndoEnabled {
			get { return false; }
			set { }
		}
#if !SL
		public override CharacterCasing CharacterCasing {
			get { return CharacterCasing.Normal; }
			set { }
		}
#endif
		public override string Text {
			get {
				if (EditBox != null)
					return EditBox.Password;
				return string.Empty;
			}
		}
		public override object EditValue {
			get {
				if (EditBox != null)
					return EditBox.Password;
				return string.Empty;
			}
			set {
				if (EditBox == null || EditBox.Password == (string)value)
					return;
				EditBox.Password = (string)value;
			}
		}
		public override int MaxLength {
			get {
				if (EditBox != null)
					return EditBox.MaxLength;
				return -1;
			}
			set {
				if (EditBox != null)
					EditBox.MaxLength = value;
			}
		}
		public override Brush Foreground {
			get {
				if (EditBox == null)
					return null;
				return EditBox.Foreground;
			}
		}
		public override bool IsReadOnly { get; set; }
		public override string SelectedText {
			get { return string.Empty; }
			set { }
		}
		public override int SelectionLength {
			get { return -1; }
			set { }
		}
		public override int SelectionStart {
			get { return -1; }
			set { }
		}
		public override int CaretIndex {
			get { return -1; }
			set { }
		}
		public PasswordBoxWrapper(PasswordBoxEdit editor)
			: base(editor) {
		}
		public override int GetCharacterIndexFromLineIndex(int lineIndex) {
			return -1;
		}
		public override int GetCharacterIndexFromPoint(Point point, bool snapToText) {
			return -1;
		}
		public override int GetFirstVisibleLineIndex() {
			return -1;
		}
		public override int GetLastVisibleLineIndex() {
			return -1;
		}
		public override int GetLineIndexFromCharacterIndex(int charIndex) {
			return -1;
		}
		public override int GetLineLength(int lineIndex) {
			return -1;
		}
		public override string GetLineText(int lineIndex) {
			if (EditBox != null)
				return EditBox.Password;
			return null;
		}
		public override void Select(int start, int length) {
			if (start == 0 && length == Text.Return(x => x.Length, () => 0))
				SelectAll();
		}
		public override void ScrollToHome() {
		}
		public override void SelectAll() {
			if (EditBox != null)
				EditBox.SelectAll();
		}
		public override void UnselectAll() {
		}
		public override void Copy() {
		}
		public override void Cut() {
		}
		public override void Undo() {
		}
		public override void Paste() {
#if !SL
			if (EditBox != null)
				EditBox.Paste();
#endif
		}
#if SL
		public override void ProcessKeyUp(KeyEventArgs e) {
		}
#endif
		protected override FrameworkElement GetEditBox() {
			return EditBox;
		}
		public override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (key == Key.Up || key == Key.Down)
				return false;
			if (key == Key.Left || key == Key.Right)
				return Editor.IsValueChanged;
			return true;
		}
	}
}
