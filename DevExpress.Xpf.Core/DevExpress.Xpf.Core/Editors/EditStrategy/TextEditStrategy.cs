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

using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Services;
#if !SL
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Data.Mask;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Native;
#else
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
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
using TextCompositionEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs;
#endif
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class TextEditStrategy : EditStrategyBase {
		public virtual bool IsInProcessNewValueDialog { get { return false; } }
		protected bool AllowSpinOnMouseWheel {
			get { return AllowKeyHandling && Editor.IsKeyboardFocusWithin && GetAllowSpinOnMouseWheelInternal(); }
		}
		protected bool IsMultilineText {
			get { return Editor.AcceptsReturn; }
		}
#if !SL
		protected CharacterCasing CharacterCasing {
			get { return Editor.GetActualCharactedCasing(); }
		}
#endif
		new TextEditSettings Settings { get { return base.Settings as TextEditSettings; } }
		protected bool IsSelectAll { get { return EditBox != null && EditBox.SelectionLength == EditBox.Text.Length; } }
		protected override bool IsNullTextSupported { get { return true; } }
		protected internal virtual bool NeedsEnter { get { return TextInputService.AcceptsReturn; } }
		protected internal virtual bool AllowSpin { get { return Editor.IsEnabled && !Editor.IsReadOnly && AllowKeyHandling; } }
		protected new TextEditBase Editor { get { return base.Editor as TextEditBase; } }
		protected EditBoxWrapper EditBox { get { return Editor.EditBox; } }
		protected TextInputServiceBase TextInputService { get { return PropertyProvider.GetService<TextInputServiceBase>(); } }
		protected IItemsProvider2 ItemsProvider { get { return PropertyProvider.GetService<ItemsProviderService>().ItemsProvider; } }
		public TextEditStrategy(TextEditBase editor) : base(editor) {
		}
		public override void FocusEditCore() {
			base.FocusEditCore();
			PerformSelectAllOnGotFocus();
		}
		public override void AfterOnGotFocus() {
			base.AfterOnGotFocus();
			PerformSelectAllOnGotFocus();
		}
		protected virtual void PerformSelectAllOnGotFocus() {
			if (!Editor.CanSelectAllOnGotFocus)
				return;
			Editor.PerformKeyboardSelectAll();
		}
		protected internal virtual void ValidateOnEnterKeyPressed(KeyEventArgs e) {
			DoValidate(UpdateEditorSource.EnterKeyPressed);
			ValueContainer.FlushEditValue();
			UpdateDisplayText();
		}
		protected override void ProcessPreviewKeyDownInternal(KeyEventArgs e) {
			base.ProcessPreviewKeyDownInternal(e);
			TextInputService.PreviewKeyDown(e);
		}
		protected internal virtual bool? RestoreDisplayText() {
			if (EditBox == null)
				return new bool?();
			CursorPositionSnapshot snapshot = new CursorPositionSnapshot(EditBox.SelectionStart, EditBox.SelectionLength, EditBox.Text, false);
			bool result = ResetValidationError();
			DoValidate(UpdateEditorSource.TextInput);
			snapshot.ApplyToEdit(Editor);
			return result;
		}
		protected virtual void UpdateEditorForce(object value) {
			if (EditBox == null)
				return;
			EditBox.EditValue = GetDisplayText();
		}
		public bool PerformSpinDown() {
			if (!CanSpinDown())
				return false;
			if (RaiseSpin(false))
				return true;
			return SpinDown();
		}
		protected virtual bool SpinDown() {
			return TextInputService.SpinDown();
		}
		protected internal virtual bool CanSpinDown() {
			return AllowSpin;
		}
		public bool PerformSpinUp() {
			if (!CanSpinUp())
				return false;
			if (RaiseSpin(true))
				return true;
			return SpinUp();
		}
		protected virtual bool SpinUp() {
			return TextInputService.SpinUp();
		}
		protected internal virtual bool CanSpinUp() {
			return AllowSpin;
		}
		bool RaiseSpin(bool isUp) {
			SpinEventArgs e = new SpinEventArgs(isUp);
			Editor.RaiseEvent(e);
			return e.Handled;
		}
		public virtual void OnPreviewTextInput(TextCompositionEventArgs e) {
			TextInputService.PreviewTextInput(e);
		}
		public override void PreviewMouseDown(MouseButtonEventArgs e) {
			base.PreviewMouseDown(e);
			TextInputService.PreviewMouseDown(e);
		}
		public override void PreviewMouseUp(MouseButtonEventArgs e) {
			base.PreviewMouseUp(e);
			TextInputService.PreviewMouseUp(e);
		}
		public override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			TextInputService.PreviewMouseWheel(e);
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(TextEditBase.TextProperty, baseValue => baseValue, (baseValue) => baseValue != null ? Convert.ToString(baseValue, CultureInfo.InvariantCulture) : string.Empty);
		}
		public virtual void Select(int start, int length) {
			TextInputService.Select(start, length);
		}
		public virtual void SelectAll() {
			TextInputService.SelectAll();
		}
		public virtual void UnselectAll() {
			if (EditBox != null)
				EditBox.UnselectAll();
		}
		public virtual void Clear() {
			SetEditValueForce(null);
		}
		protected override string FormatDisplayTextInternal(object editValue, bool applyFormatting) {
			return ProcessTextWithCharacterCasing(base.FormatDisplayTextInternal(editValue, applyFormatting));
		}
		public override string CoerceDisplayText(string displayText) {
			string text = IsInSupportInitialize ? string.Empty : GetDisplayText();
			text = base.CoerceDisplayText(text);
			return text;
		}
		string ProcessTextWithCharacterCasing(string text) {
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			string result = text;
			switch (CharacterCasing) {
				case CharacterCasing.Lower:
					result = text.ToLower();
					break;
				case CharacterCasing.Upper:
					result = text.ToUpper();
					break;
			}
			return result;
		}
		public override void OnLoaded() {
			base.OnLoaded();
			UpdateDisplayText();
		}
		public virtual bool NeedsKey(Key key, ModifierKeys modifiers) {
			return TextInputService.NeedsKey(key, modifiers);
		}
		public override void Initialize() {
			TextInputService.Initialize();
		}
		public override void UpdateAllowDrop(bool isVisible) {
			base.UpdateAllowDrop(isVisible);
			EditBox.AllowDrop = !isVisible;
		}
		public virtual void Undo() {
			TextInputService.Undo();
		}
		public virtual bool CanUndo() {
			return TextInputService.CanUndo();
		}
		public virtual void Paste() {
			TextInputService.Paste();
		}
		protected internal virtual void Paste(string text) {
		}
		public virtual bool CanPaste() {
			return TextInputService.CanPaste();
		}
		public virtual void Copy() {
			TextInputService.Copy();
		}
		public virtual bool CanCopy() {
			return TextInputService.CanCopy();
		}
		public virtual void Cut() {
			TextInputService.Cut();
		}
		public virtual bool CanSelectAll() {
			return TextInputService.CanSelectAll();
		}
		public virtual bool CanCut() {
			return TextInputService.CanCut();
		}
		public virtual void Delete() {
			TextInputService.Delete();
		}
		public virtual bool CanDelete() {
			return TextInputService.CanDelete();
		}
		internal virtual void FlushPendingEditActions(UpdateEditorSource updateSource) {
			TextInputService.FlushPendingEditActions(updateSource);
		}
		protected override void ResetValidationErrorInternal() {
			base.ResetValidationErrorInternal();
			TextInputService.SetInitialEditValue(ValueContainer.EditValue);
		}
		protected internal virtual void OnTextChanged(string oldText, string text) {
			if (ShouldLockUpdate)
				return;
			SyncWithValue(TextEditBase.TextProperty, oldText, text);
		}
		protected override void SyncWithValueInternal() {
			TextInputService.SetInitialEditValue(ValueContainer.EditValue);
		}
		protected override void ClearUndoStack() {
			base.ClearUndoStack();
			EditBox.ClearUndoStack();
		}
		protected virtual bool GetAllowSpinOnMouseWheelInternal() {
			if (Editor is TextEdit)
				return ((TextEdit)Editor).AllowSpinOnMouseWheel;
			return true;
		}
		protected override void UpdateEditCoreTextInternal(string displayText) {
			if (SkipEditCoreDisplayTextUpdate(displayText))
				return;
			EditBox.EditValue = displayText;
			if (TextInputService.ShouldUseFormatting) {				
				EditBox.Select(TextInputService.SelectionStart, TextInputService.SelectionLength);			   
			}
		}
		protected virtual bool SkipEditCoreDisplayTextUpdate(string displayText) {
			if (IsInSupportInitialize || !Editor.IsPrintingMode && Editor.EditMode == EditMode.InplaceInactive || Editor.AllowUpdateTextBlockWhenPrinting)
				return false;
			if (EditBox.Text == displayText)
				return true;
			int newLineCount = !string.IsNullOrEmpty(displayText) ? displayText.Split(Environment.NewLine.ToCharArray()).Length : 0;
			return EditBox.LineCount == newLineCount && Editor.GetActualTextWrapping() == TextWrapping.NoWrap;
		}
		public virtual void HighlightedTextChanged(string text) {
			Settings.HighlightedText = text;
			EditBox.HighlightedText = text;
		}
		public virtual void HighlightedTextCriteriaChanged(HighlightedTextCriteria criteria) {
			Settings.HighlightedTextCriteria = criteria;
			EditBox.HighlightedTextCriteria = criteria;
		}
		public virtual void CaretIndexChanged(int value) {
			TextInputService.Select(value, 0);
		}
		public virtual void SetSelectionStart(int value) {
			TextInputService.Select(value, EditBox.SelectionLength);
		}
		public virtual void SetSelectionLength(int value) {
			TextInputService.Select(EditBox.SelectionStart, value);
		}
		public virtual void SetSelectedText(string value) {
			TextInputService.InsertText(value);
		}
		protected override void SyncEditCorePropertiesInternal() {
			base.SyncEditCorePropertiesInternal();
			TextInputService.UpdateIme();
		}
#if SL
		protected internal override void UpdateToolTip() {
			base.UpdateToolTip();
			if (Editor.EditMode != EditMode.InplaceInactive)
				return;
			TextBlock tb = Editor.EditCore as TextBlock;
			if (tb == null)
				return;
			ToolTip tooltip = ToolTipService.GetToolTip(tb) as ToolTip;
			if (!Editor.ShowTooltipForTrimmedText) {
				if (tooltip != null)
					tooltip.Visibility = Visibility.Collapsed;
				return;
			}
			bool isTextTrimmed = LayoutHelper.IsInVisualTree(Editor) && LayoutHelper.IsInVisualTree(tb) && TextBlockService.CalcIsTextTrimmed(tb);
			if (!isTextTrimmed) {
				if (tooltip != null)
					tooltip.Visibility = Visibility.Collapsed;
				return;
			}
			if (tooltip == null)
				tooltip = (ToolTip)CreateTrimmedTextToolTip(tb.Text, true);
			tooltip.Visibility = Visibility.Visible;
			ToolTipService.SetToolTip(tb, tooltip);
		}
#endif
		protected override void InitializeServices() {
			base.InitializeServices();
			PropertyProvider.RegisterService(CreateTextInputService());
			PropertyProvider.RegisterService(CreateRangeEditService());
			PropertyProvider.RegisterService(CreateItemsProviderService());
			PropertyProvider.RegisterService(CreatePopupService());
		}
		protected virtual PopupService CreatePopupService() {
			return new PopupService(Editor);
		}
		protected virtual ItemsProviderService CreateItemsProviderService() {
			return new ItemsProviderService(Editor);
		}
		protected virtual TextInputServiceBase CreateTextInputService() {
			return new TextInputService(Editor);
		}
		protected virtual RangeEditorService CreateRangeEditService() {
			return new RangeEditorService(Editor);
		}
		protected override ValueContainerService CreateValueContainerService() {
			return new TextInputValueContainerService(Editor);
		}
		protected override void PerformNullInput() {
			base.PerformNullInput();
			TextInputService.PerformNullInput();
		}
		public override void OnGotFocus() {
			base.OnGotFocus();
			TextInputService.GotFocus();
		}
		public override void OnLostFocus() {
			base.OnLostFocus();
			TextInputService.LostFocus();
		}
		protected internal override void PrepareForCheckAllowLostKeyboardFocus() {
			base.PrepareForCheckAllowLostKeyboardFocus();
			TextInputService.FlushPendingEditActions(UpdateEditorSource.DontValidate);
		}
		public override object ConvertToBaseValue(object value) {
			return value == DBNull.Value ? null : value;
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new TextEditorValidator(Editor);
		}
		protected override BaseEditingSettingsService CreateTextInputSettingsService() {
			return new TextEditSettingsService(Editor);
		}
		protected internal override string FormatDisplayText(object editValue, bool applyFormatting) {
			if (TextInputService.ShouldUseFormatting)
				return TextInputService.FormatDisplayText(editValue, applyFormatting);
			return base.FormatDisplayText(editValue, applyFormatting);
		}
		protected override void SyncWithEditorInternal() {
			TextInputService.SyncWithEditor();
		}
		protected override object GetEditableObject() {
			return EditBox.Text;
		}
		protected override void UpdateDisplayTextAndRestoreCursorPosition() {
			CursorPositionSnapshot snapshot = new CursorPositionSnapshot(EditBox.SelectionStart, EditBox.SelectionLength, EditBox.Text, false);
			UpdateDisplayTextInternal();
			snapshot.ApplyToEdit(Editor);
		}
		protected override bool ShouldRestoreCursorPosition() {
			bool shoudNonUpdate = TextInputService.ShouldUseFormatting || ApplyDisplayTextConversion || IsMultilineText || PropertyProvider.SuppressFeatures;
			return !shoudNonUpdate;
		}
		public override void ResetIsValueChanged() {
			base.ResetIsValueChanged();
			TextInputService.Reset();
		}
		public virtual bool ProcessNewValue(string editText) {
			return false;
		}
	}
	class CursorPositionSnapshot {
		public bool IsAutoComplete { get; private set; }
		public int SelectionStart { get; private set; }
		public int SelectionLength { get; private set; }
		public string DisplayText { get; private set; }
		public bool IsCursorAtEnd { get { return string.IsNullOrEmpty(DisplayText) || SelectionStart >= DisplayText.Length; } }
		public bool IsCursorAtStart { get { return !string.IsNullOrEmpty(DisplayText) && SelectionLength == 0 && SelectionStart == 0; } }
		public bool IsSelectAll { get { return !string.IsNullOrEmpty(DisplayText) && SelectionStart == 0 && SelectionLength == DisplayText.Length; } }
		bool IsAutoCompleteSelection { get { return IsAutoComplete && (SelectionStart + SelectionLength == DisplayText.Length); } }
		public CursorPositionSnapshot(int selectionStart, int selectionLength, string displayText, bool isAutoComplete) {
			IsAutoComplete = isAutoComplete;
			SelectionStart = selectionStart;
			SelectionLength = selectionLength;
			DisplayText = displayText;
		}
		public void ApplyToEdit(TextEditBase editor) {
			string currentText = editor.EditBox.Text;
			if (string.IsNullOrEmpty(currentText))
				return;
			if (DisplayText == currentText)
				return;
			if (IsSelectAll && !IsAutoComplete)
				SelectAll(editor);
			else if (IsCursorAtStart) {
				SetCursorStart(editor);
			}
			else if ((IsCursorAtEnd && !IsAutoComplete) || (currentText.Length < SelectionStart))
				SetCursorEnd(editor);
			else {
				if (string.IsNullOrEmpty(currentText))
					SetCursorPosition(editor, 0, false);
				if ((DisplayText.Length < SelectionStart) || (currentText.Length < SelectionStart))
					SetCursorEnd(editor);
				if (DisplayText.Substring(0, Math.Max(0, SelectionStart)) == currentText.Substring(0, Math.Max(0, SelectionStart))) {
					if (IsSelectAll && currentText.StartsWith(DisplayText)) 
						SetCursorPosition(editor, DisplayText.Length, true);
					else
						SetCursorPosition(editor, SelectionStart, IsAutoCompleteSelection);
				}
				else
					SetCursorPosition(editor, SelectionStart, IsAutoComplete);
			}
		}
		void SelectAll(TextEditBase editor) {
			editor.EditBox.SelectAll();
		}
		void SetCursorPosition(TextEditBase editor, int selectionStart, bool autoComplete) {
			EditBoxWrapper editBox = editor.EditBox;
			int newStart = Math.Min(selectionStart, editBox.Text.Length);
			int newLength = autoComplete ? Math.Max(editBox.Text.Length - selectionStart, 0) : 0;
			editBox.Select(newStart, newLength);
		}
		void SetCursorEnd(TextEditBase editor) {
			EditBoxWrapper editBox = editor.EditBox;
			editBox.Select(editor.EditBox.Text.Length + 1, 0);
		}
		void SetCursorStart(TextEditBase editor) {
			EditBoxWrapper editBox = editor.EditBox;
			editBox.Select(0, 0);
		}
	}
}
