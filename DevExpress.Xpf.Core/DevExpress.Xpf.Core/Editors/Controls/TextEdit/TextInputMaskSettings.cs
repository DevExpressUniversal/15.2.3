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

using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
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
namespace DevExpress.Xpf.Editors {
	public interface IMaskManagerProvider {
		WpfMaskManager Instance { get; }
		MaskManager CreateNew();
		void UpdateRequired();
		void LocalEditActionPerformed();
		void SetMaskManagerValue(object editValue);
	}
	public class TextInputMaskSettings : TextInputSettingsBase, IMaskManagerProvider {
		static object OnCoerceMaskType(DependencyObject d, object baseValue) {
			return baseValue;
		}
		protected virtual void MaskPropertyChanged() {
			UpdateMaskManager();
		}
		protected virtual void MaskTypePropertyChanged() {
		}
		protected internal WpfMaskManager MaskManager { get; set; }
		bool IsPlaceHoldersSupport { get { return OwnerEdit.MaskType == MaskType.RegEx || OwnerEdit.MaskType == MaskType.Regular || OwnerEdit.MaskType == MaskType.Simple; } }
		RangeEditorService RangeEditorService { get { return OwnerEdit.PropertyProvider.GetService<RangeEditorService>(); } }
		ValueChangingService ValueChangingService { get { return OwnerEdit.PropertyProvider.GetService<ValueChangingService>(); } }
		new TextEdit OwnerEdit {
			get { return (TextEdit)base.OwnerEdit; }
		}
		new TextEditStrategy EditStrategy {
			get { return (TextEditStrategy)OwnerEdit.EditStrategy; }
		}
		protected internal override int SelectionStart {
			get { return MaskManager.DisplaySelectionStart; }
		}
		protected internal override int SelectionLength {
			get { return MaskManager.DisplaySelectionLength; }
		}
		protected internal override bool ShouldUseFormatting {
			get { return GetUseDisplayTextFromMask(); }
		}
		protected bool IsUpdateRequired { get; private set; }
		protected bool IsInLocalEditAction { get; private set; }
		public TextInputMaskSettings(TextEditBase editor) : base(editor) {
		}
		protected internal virtual MaskManager CreateDefaultMaskManager() {
			CultureInfo managerCultureInfo = OwnerEdit.MaskCulture;
			if (managerCultureInfo == null)
				managerCultureInfo = CultureInfo.CurrentCulture;
			string editMask = OwnerEdit.Mask;
			if (editMask == null)
				editMask = String.Empty;
			switch (OwnerEdit.MaskType) {
				case MaskType.Numeric:
					return new NumericMaskManager(editMask, managerCultureInfo, OwnerEdit.AllowNullInput);
				case MaskType.RegEx:
					if (OwnerEdit.MaskIgnoreBlank && editMask.Length > 0)
						editMask = "(" + editMask + ")?";
					return new RegExpMaskManager(editMask, false, OwnerEdit.MaskAutoComplete != AutoCompleteType.None, OwnerEdit.MaskAutoComplete == AutoCompleteType.Optimistic, OwnerEdit.MaskShowPlaceHolders,
						OwnerEdit.MaskPlaceHolder, managerCultureInfo);
				case MaskType.DateTime:
					return new DateTimeMaskManager(editMask, false, managerCultureInfo, OwnerEdit.AllowNullInput);
				case MaskType.DateTimeAdvancingCaret:
					return new DateTimeMaskManager(editMask, true, managerCultureInfo, OwnerEdit.AllowNullInput);
				case MaskType.Regular:
					return new LegacyMaskManager(LegacyMaskInfo.GetRegularMaskInfo(editMask, managerCultureInfo), OwnerEdit.MaskPlaceHolder, OwnerEdit.MaskSaveLiteral, OwnerEdit.MaskIgnoreBlank);
				case MaskType.Simple:
					return new LegacyMaskManager(LegacyMaskInfo.GetSimpleMaskInfo(editMask, managerCultureInfo), OwnerEdit.MaskPlaceHolder, OwnerEdit.MaskSaveLiteral, OwnerEdit.MaskIgnoreBlank);
				default:
					return null;
			}
		}
		protected internal override void AssignProperties() {
			base.AssignProperties();
			UpdateMaskManager();
		}
		#region IMaskManagerProvider
		MaskManager IMaskManagerProvider.CreateNew() {
			return CreateDefaultMaskManager();
		}
		void IMaskManagerProvider.UpdateRequired() {
			UpdateRequired();
		}
		void IMaskManagerProvider.LocalEditActionPerformed() {
			LocalEditActionPerformed();
		}
		void UpdateRequired() {
			IsUpdateRequired = true;
			IsInLocalEditAction = false;
			ValueChangingService.SetIsValueChanged(true);
		}
		void LocalEditActionPerformed() {
			IsInLocalEditAction = true;
			IsUpdateRequired = false;
			ValueChangingService.SetIsValueChanged(true);
		}
		WpfMaskManager IMaskManagerProvider.Instance {
			get { return MaskManager ?? new WpfMaskManager(this); }
		}
		void IMaskManagerProvider.SetMaskManagerValue(object editValue) {
			SetMaskManagerValue(editValue);
		}
		#endregion
		protected internal override void InsertText(string text) {
			bool result = MaskManager.Insert(text);
			UpdateEditor(result && !IsInLocalEditAction, UpdateEditorSource.TextInput);
			if (!result && OwnerEdit.MaskBeepOnError)
				BeepOnErrorHelper.Process();
		}
		protected internal override void ProcessPreviewKeyDown(KeyEventArgs e) {
			base.ProcessPreviewKeyDown(e);
			if (!EditingSettings.AllowKeyHandling || e.Handled)
				return;
			var result = new bool?();
			if (e.Key == Key.Home) {
				MaskManager.CursorHome(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				UpdateEditor(IsUpdateRequired, UpdateEditorSource.TextInput);
				result = true;
			}
			if (e.Key == Key.End) {
				MaskManager.CursorEnd(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				UpdateEditor(IsUpdateRequired, UpdateEditorSource.TextInput);
				result = true;
			}
			if (e.Key == Key.Right) {
				MaskManager.CursorRight(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				UpdateEditor(IsUpdateRequired, UpdateEditorSource.TextInput);
				result = true;
			}
			if (e.Key == Key.Left) {
				MaskManager.CursorLeft(ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e)));
				UpdateEditor(IsUpdateRequired, UpdateEditorSource.TextInput);
				result = true;
			}
			if (e.Key == Key.Back) {
				PerformBackspace();
				result = true;
			}
			if (e.Key == Key.Delete && !EditStrategy.ShouldProcessNullInput(e)) {
				PerformDelete();
				result = true;
			}
			if (e.Key == Key.A && ModifierKeysHelper.IsOnlyCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && !ModifierKeysHelper.IsAltPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				EditStrategy.SelectAll();
				result = true;
			}
			if (e.Key == Key.Space && EditingSettings.AllowEditing) {
				InsertText(" ");
				EditStrategy.UpdateDisplayText();
				result = true;
			}
			if (e.Key == Key.Enter && OwnerEdit.AcceptsReturn) {
				result = true;
			}
			if (result.HasValue) {
				e.Handled = true;
			}
		}
		void PerformBackspace() {
			if (!EditingSettings.AllowEditing)
				return;
			bool backResult = MaskManager.Backspace();
			UpdateEditor(backResult, UpdateEditorSource.TextInput);
		}
		bool GetUseDisplayTextFromMask() {
			bool focused = OwnerEdit.FocusManagement.IsFocusWithin && EditingSettings.AllowKeyHandling;
			return focused ? GetUseDisplayTextFromMaskFocused() : GetUseDisplayTextFromMaskUnfocused();
		}
		protected virtual bool GetUseDisplayTextFromMaskFocused() {
			if (RangeEditorService.ShouldRoundToBounds)
				return true;
			if (IsInLocalEditAction || IsUpdateRequired)
				return true;
			return !(OwnerEdit.AllowNullInput && EditStrategy.ShouldShowEmptyTextInternal(MaskManager.GetCurrentEditValue()));
		}
		protected virtual bool GetUseDisplayTextFromMaskUnfocused() {
			return OwnerEdit.MaskUseAsDisplayFormat && !EditStrategy.IsNullValue(ValueContainer.EditValue);
		}
		protected internal override string FormatDisplayText(object editValue, bool applyFormatting) {
			return ShouldUseFormatting ? MaskManager.DisplayText : base.FormatDisplayText(editValue, applyFormatting);
		}
		protected override void UpdateEditValueInternal(UpdateEditorSource updateSource) {
			object value = MaskManager.GetCurrentEditValue();
			ValueContainer.SetEditValue(value, updateSource);
		}
		void UpdateEditor(bool update, UpdateEditorSource updateSource) {
			if (update)
				UpdateEditValue(updateSource);
			EditStrategy.UpdateDisplayText();
		}
		internal void PerformDelete() {
			if (!EditingSettings.AllowEditing)
				return;
			bool result = MaskManager.Delete();
			UpdateEditor(result, UpdateEditorSource.TextInput);
			if (!result && OwnerEdit.MaskBeepOnError)
				BeepOnErrorHelper.Process();
		}
		protected internal override void PerformNullInput() {
			if (!EditingSettings.AllowEditing)
				return;
			SetMaskManagerValue(OwnerEdit.NullValue);
			UpdateEditValue(UpdateEditorSource.ValueChanging);
			MaskManager.SelectAll();
			IsInLocalEditAction = false;
			IsUpdateRequired = false;
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void SetMaskManagerValue(object value) {
			object maskValue = EditStrategy.ConvertToBaseValue(value);
			try {
				if (RangeEditorService.ShouldRoundToBounds)
					maskValue = RangeEditorService.CorrentToBounds(maskValue);
				MaskManager.SetInitialEditValue(maskValue);
			}
			catch (Exception) {
				if (OwnerEdit.EditMode == EditMode.Standalone)
					throw;
			}
		}
		protected internal override void PerformGotFocus() {
			base.PerformGotFocus();
			SetMaskManagerValue(ValueContainer.EditValue);
			EditStrategy.UpdateDisplayText();
		}
		protected internal override void PerformLostFocus() {
			base.PerformLostFocus();
			FlushPendingEditActions(UpdateEditorSource.LostFocus);
		}
		protected internal override void FlushPendingEditActions(UpdateEditorSource updateEditor) {
			base.FlushPendingEditActions(updateEditor);
			MaskManager.FlushPendingEditActions();
			UpdateEditor(IsInLocalEditAction || IsUpdateRequired, updateEditor);
			IsInLocalEditAction = false;
			IsUpdateRequired = false;
		}
		protected internal void UpdateMaskManager() {
			MaskManager = new WpfMaskManager(this);
			MaskManager.Initialize();
			SetMaskManagerValue(ValueContainer.EditValue);
		}
		protected internal override bool SpinUp() {
			if (SpinUpInternal()) {
				UpdateEditor(!IsInLocalEditAction, UpdateEditorSource.TextInput);
				return true;
			}
			return false;
		}
		protected internal override bool SpinDown() {
			if (SpinDownInternal()) {
				UpdateEditor(!IsInLocalEditAction, UpdateEditorSource.TextInput);
				return true;
			}
			return false;
		}
		protected virtual bool SpinUpInternal() {
			return RangeEditorService.SpinUp(ValueContainer.EditValue, this);
		}
		protected virtual bool SpinDownInternal() {
			return RangeEditorService.SpinDown(ValueContainer.EditValue, this);
		}
		protected internal override void Delete() {
			if (!CanDelete())
				return;
			PerformDelete();
			EditStrategy.UpdateDisplayText();
		}
		protected internal override void ProcessPreviewTextInput(TextCompositionEventArgs e) {
			if (MaskTextEditStrategyTextInputHelper.ShouldIgnoreTextInput(e.Text))
				return;
			if (!EditingSettings.AllowEditing)
				return;
			base.ProcessPreviewTextInput(e);
			InsertText(e.Text);
			e.Handled = true;
		}
		readonly Locker selectionLocker = new Locker();
		protected internal override void Select(int selectionStart, int selectionLength) {
			this.selectionLocker.DoLockedActionIfNotLocked(
				() => {
					base.Select(selectionStart, selectionLength);
					if (IsSelectAll)
						MaskManager.SelectAll();
					else {
						MaskManager.CursorToDisplayPosition(selectionStart, false);
						MaskManager.CursorToDisplayPosition(selectionStart + selectionLength, true);
						UpdateEditor(false, UpdateEditorSource.TextInput);
					}
				}
			);
		}
		protected internal override void SetInitialEditValue(object editValue) {
			if (Equals(MaskManager.GetCurrentEditValue(), editValue)) {
				base.SetInitialEditValue(editValue);
				return;
			}
			SetMaskManagerValue(editValue);
			UpdateEditor(false, UpdateEditorSource.TextInput);
		}
		protected internal override bool CanUndo() {
			return MaskManager.CanUndo;
		}
		protected internal override void Undo() {
			if (MaskManager.Undo())
				UpdateEditor(true, UpdateEditorSource.TextInput);
			else if (OwnerEdit.MaskBeepOnError)
				BeepOnErrorHelper.Process();
		}
		protected internal override void Cut() {
			if (!CanCut())
				return;
			Copy();
			bool result = MaskManager.Delete();
			UpdateEditor(result && !IsInLocalEditAction, UpdateEditorSource.TextInput);
			if (!result && OwnerEdit.MaskBeepOnError)
				BeepOnErrorHelper.Process();
		}
		protected internal override void Paste() {
			if (!CanPaste())
				return;
			InsertText(DXClipboard.GetText());
		}
		protected internal override bool NeedsKey(Key key, ModifierKeys modifier) {
			if (key == Key.PageDown || key == Key.PageUp)
				return false;
			if (key == Key.Left || key == Key.Home)
				return NeedsNavigateKeyLeftRight(key, modifier, () => MaskManager.CheckCursorLeft());
			if (key == Key.Right || key == Key.End)
				return NeedsNavigateKeyLeftRight(key, modifier, () => MaskManager.CheckCursorRight());
			if (key == Key.Up || key == Key.Down)
				return NeedsNavigateUpDown(key, modifier, () => ModifierKeysHelper.ContainsModifiers(modifier));
			return true;
		}
		protected internal override bool IsValueValid(object value) {
			return !IsPlaceHoldersSupport || MaskManager.IsMatch;
		}
		protected internal override void ProcessSyncWithEditor() {
			base.ProcessSyncWithEditor();
			EditStrategy.UpdateDisplayText();
			Reset();
		}
		protected override object GetEditValueForSyncWithEditor() {
			object editValue = base.GetEditValueForSyncWithEditor();
			if (object.Equals(editValue, MaskManager.GetCurrentEditValue()))
				return editValue;
			MaskManager.SetInitialEditValue(editValue);
			return MaskManager.GetCurrentEditValue();
		}
		protected internal override void ProcessPreviewMouseDown(MouseEventArgs e) {
			base.ProcessPreviewMouseDown(e);
			int index = EditBox.GetCharacterIndexFromPoint(e.GetPosition(OwnerEdit.EditCore), true);
			if (index < 0)
				return;
			if (ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers())) {
				int length = EditBox.CaretIndex - index;
				int caretIndex = Math.Min(index, length > 0 ? EditBox.SelectionStart + EditBox.SelectionLength : EditBox.SelectionStart);
				EditBox.Select(caretIndex, length > 0 ? length + EditBox.SelectionLength : Math.Abs(length));
				e.Handled = true;
			}
			else {
				EditBox.CaretIndex = index;
			}
			if (IsInLocalEditAction)
				FlushPendingEditActions(UpdateEditorSource.TextInput);
			MaskManager.CursorToDisplayPosition(index, false);
		}
		protected internal override void ProcessPreviewMouseUp(MouseEventArgs e) {
			base.ProcessPreviewMouseUp(e);
			Select(EditBox.SelectionStart, EditBox.SelectionLength);
			EditStrategy.UpdateDisplayText();
		}
		protected internal override void ProcessPreviewMouseWheel(MouseWheelEventArgs e) {
			base.ProcessPreviewMouseWheel(e);
			EditStrategy.UpdateDisplayText();
		}
		protected internal override void Reset() {
			base.Reset();
			IsUpdateRequired = false;
			IsInLocalEditAction = false;
		}
		protected internal override void UpdateIme() {
			base.UpdateIme();
			OwnerEdit.Do(x => EditBox.IsImeEnabled(false));
		}
	}
}
