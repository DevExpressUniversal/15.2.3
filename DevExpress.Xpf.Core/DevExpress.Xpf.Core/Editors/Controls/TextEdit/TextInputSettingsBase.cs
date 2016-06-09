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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Mvvm.Native;
using System;
using System.Windows.Input;
namespace DevExpress.Xpf.Editors {
	public abstract class TextInputSettingsBase {
		public bool AllowRejectUnknownValues { get; private set; }
		readonly Locker updateTextLocker = new Locker();
		protected BaseEditingSettingsService EditingSettings { get { return PropertyProvider.GetService<BaseEditingSettingsService>(); } }
		protected TextEditBase OwnerEdit { get; private set; }
		protected TextEditStrategy EditStrategy { get { return OwnerEdit.EditStrategy as TextEditStrategy; } }
		protected ValueContainerService ValueContainer { get { return PropertyProvider.GetService<ValueContainerService>(); } }
		protected ActualPropertyProvider PropertyProvider { get { return OwnerEdit.PropertyProvider; } }
		protected EditBoxWrapper EditBox { get { return OwnerEdit.EditBox; } }
		protected internal virtual int SelectionStart { get { return EditBox.SelectionStart; } }
		protected internal virtual int SelectionLength { get { return EditBox.SelectionLength; } }
		bool HasSelection { get { return SelectionLength > 0; } }
		protected internal virtual bool ShouldUseFormatting { get { return false; } }
		protected bool IsSelectAll { get { return EditBox != null && EditBox.SelectionLength == EditBox.Text.Length; } }
		protected TextInputSettingsBase(TextEditBase editor) {
			OwnerEdit = editor;
			AssignEditorInternal();
		}
		protected virtual void AssignEditorInternal() {
			AssignProperties();
		}
		protected internal virtual void AssignProperties() {
			UpdateIme();
		}
		protected internal virtual EditStrategyBase CreateEditStrategy() {
			return new TextEditStrategy(OwnerEdit);
		}
		protected internal virtual void ProcessPreviewKeyDown(KeyEventArgs e) {
			if (e.Handled || !EditingSettings.AllowKeyHandling)
				return;
			if (e.Key == Key.Enter && OwnerEdit.ValidateOnEnterKeyPressed)
				EditStrategy.ValidateOnEnterKeyPressed(e);
			bool? result = e.Key == Key.Escape ? EditStrategy.RestoreDisplayText() : new bool?();
			if (e.Key == Key.Up && CanProcessKeyUpDown(e))
				result = EditStrategy.PerformSpinUp();
			if (e.Key == Key.Down && CanProcessKeyUpDown(e))
				result = EditStrategy.PerformSpinDown();
			if (CapsLockHelper.IsCapsLockToggled)
				EditStrategy.CoerceToolTip();
			if (result.HasValue) {
				e.Handled = result.Value;
			}
		}
		protected internal virtual string FormatDisplayText(object editValue, bool applyFormatting) {
			object converted = ConvertEditValueForFormatDisplayText(editValue);
			return converted.Return(x => x.ToString(), () => string.Empty);
		}
		protected virtual void UpdateEditValue(UpdateEditorSource updateSource) {
			updateTextLocker.DoLockedActionIfNotLocked(() => UpdateEditValueInternal(updateSource));
		}
		protected virtual void UpdateEditValueInternal(UpdateEditorSource updateSource) {
		}
		protected internal virtual void PerformNullInput() {
		}
		protected internal virtual void PerformGotFocus() {
		}
		protected internal virtual void PerformLostFocus() {
		}
		protected internal virtual void FlushPendingEditActions(UpdateEditorSource updateSource) {
		}
		protected internal virtual bool SpinUp() {
			return false;
		}
		protected internal virtual bool SpinDown() {
			return false;
		}
		protected internal virtual void Delete() {
		}
		protected internal virtual bool CanDelete() {
			return EditingSettings.AllowEditing && HasSelection;
		}
		protected internal virtual void ProcessPreviewTextInput(TextCompositionEventArgs e) {
		}
		protected internal virtual void Select(int selectionStart, int selectionLength) {
			EditBox.Select(selectionStart, selectionLength);
		}
		protected internal virtual void SetInitialEditValue(object editValue) {
			EditStrategy.UpdateDisplayText();
		}
		protected internal virtual bool CanUndo() {
			return true;
		}
		protected internal virtual void Undo() {
		}
		protected internal virtual void Cut() {
		}
		protected internal virtual bool CanCut() {
			return EditingSettings.AllowEditing && HasSelection;
		}
		protected internal virtual bool CanPaste() {
			return EditingSettings.AllowEditing && DXClipboard.ContainsText();
		}
		protected internal virtual void Paste() {
		}
		protected internal virtual bool CanCopy() {
			return HasSelection;
		}
		protected internal virtual void Copy() {
			EditBox.Copy();
		}
		protected internal virtual bool CanSelectAll() {
			return EditingSettings.AllowKeyHandling;
		}
		protected internal virtual void InsertText(string value) {
		}
		protected internal virtual bool NeedsKey(Key key, ModifierKeys modifier) {
			return false;
		}
		protected bool NeedsNavigateKeyLeftRight(Key key, ModifierKeys modifiers, Func<bool> checkMethod) {
			if (OwnerEdit.EditMode != EditMode.InplaceActive)
				return true;
			if (OwnerEdit.InplaceEditing.HandleTextNavigation(key, modifiers))
				return checkMethod();
			if (IsSelectAll)
				return ModifierKeysHelper.ContainsModifiers(modifiers);
			return checkMethod();
		}
		protected bool NeedsNavigateUpDown(Key key, ModifierKeys modifiers, Func<bool> checkMethod) {
			if (OwnerEdit.EditMode != EditMode.InplaceActive)
				return true;
			if (OwnerEdit.InplaceEditing.HandleTextNavigation(key, modifiers))
				return true;
			return checkMethod();
		}
		protected internal bool CanProcessKeyUpDown(KeyEventArgs e) {
			bool contains = ModifierKeysHelper.ContainsModifiers(ModifierKeysHelper.GetKeyboardModifiers(e));
			return OwnerEdit.EditMode == EditMode.Standalone ? !contains : contains;
		}
		protected internal virtual bool DoValidate(object editValue, UpdateEditorSource source) {
			return true;
		}
		protected internal virtual bool IsValueValid(object value) {
			return true;
		}
		protected internal virtual void ProcessSyncWithEditor() {
			var editValue = GetEditValueForSyncWithEditor();
			ValueContainer.SetEditValue(editValue, UpdateEditorSource.TextInput);
		}
		protected virtual object GetEditValueForSyncWithEditor() {
			string editText = EditBox.Text;
			object editValue = string.IsNullOrEmpty(editText) && EditStrategy.ReplaceTextWithNull(editText) ? OwnerEdit.NullValue : editText;
			editValue = ValueContainer.ConvertEditTextToEditValueCandidate(editValue);
			return editValue;
		}
		protected internal virtual bool GetAcceptsReturn() {
			return false;
		}
		protected internal virtual void ProcessPreviewMouseDown(MouseEventArgs e) {
		}
		protected internal virtual void ProcessPreviewMouseUp(MouseEventArgs e) {
		}
		protected internal virtual void ProcessPreviewMouseWheel(MouseWheelEventArgs e) {
			if (e.Handled || !(OwnerEdit as TextEdit).Return(x => x.AllowSpinOnMouseWheel, () => false))
				return;
			if (e.Delta == 0)
				return;
			int steps = e.Delta / System.Windows.Forms.SystemInformation.MouseWheelScrollDelta;
			bool result = false;
			int stepsCount = Math.Max(1, Math.Abs(steps));
			for (int i = 0; i < stepsCount; i++) {
				if (e.Delta > 0)
					result = EditStrategy.PerformSpinUp();
				else
					result = EditStrategy.PerformSpinDown();
			}
			e.Handled = result;
		}
		protected internal virtual void Reset() {
		}
		protected internal virtual object ProvideEditValue(object editValue, UpdateEditorSource updateSource) {
			LookUpEditableItem item = editValue as LookUpEditableItem;
			if (item == null && !EditingSettings.IsInLookUpMode)
				return editValue;
			if (item != null) {
				if (!EditingSettings.IsInLookUpMode)
					return editValue;
				return item.EditValue;
			}
			return editValue;
		}
		protected virtual object GetDisplayValue(object editValue) {
			LookUpEditableItem item = editValue as LookUpEditableItem;
			if (item == null && !EditingSettings.IsInLookUpMode)
				return editValue;
			if (item != null) {
				if (!EditingSettings.IsInLookUpMode)
					return editValue;
				return item.DisplayValue;
			}
			return editValue;
		}
		protected internal virtual object ConvertEditValueForFormatDisplayText(object convertedValue) {
			return convertedValue;
		}
		protected internal virtual object ProcessConversion(object value, UpdateEditorSource updateSource) {
			return value;
		}
		protected internal virtual void UpdateIme() {
		}
	}
}
