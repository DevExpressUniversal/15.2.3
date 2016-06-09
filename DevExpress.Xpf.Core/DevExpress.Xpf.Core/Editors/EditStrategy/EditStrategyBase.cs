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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Globalization;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using System.Windows.Input;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Editors {
	public class ValidationState {
		BaseEdit Editor { get; set; }
		public BaseValidationError EditorValidationError { get; private set; }
		public bool IsEmpty { get; private set; }
		public bool IsValid { get; private set; }
		EditStrategyBase Strategy { get; set; }
		public ValidationState(EditStrategyBase strategy, BaseEdit editor) {
			Strategy = strategy;
			Editor = editor;
			IsEmpty = true;
			IsValid = true;
		}
		public void Reset() {
			IsEmpty = true;
			IsValid = true;
			EditorValidationError = null;
		}
		public void TryReset(UpdateEditorSource updateSource) {
			if (Strategy.ShouldResetValidateState(updateSource))
				Reset();
		}
		public void Initialize(bool isValid, BaseValidationError error) {
			IsEmpty = false;
			IsValid = isValid;
			EditorValidationError = error;
		}
		public FrameworkElement GetValidationToolTip(bool isToolTip) {
			return Strategy.CreateValidationToolTip(EditorValidationError, isToolTip);
		}
	}
	public class TrimmedTextToolTipContent {
		public TrimmedTextToolTipContent(string toolTip) {
			Content = toolTip;
		}
		public string Content { get; private set; }
	}
	public abstract partial class EditStrategyBase : IEditStrategy {
		readonly Locker editValueLocker = new Locker();
		readonly Locker editBoxTextLocker = new Locker();
		readonly Locker coerceValueLocker = new Locker();
		readonly Locker isInSupportInitialize = new Locker();
		readonly Locker raiseEventsLocker = new Locker();
		readonly Locker syncWithEditorLocker = new Locker();
		PostponedAction ApplyStyleSettingsAction { get; set; }
		protected ValidationState ValidationState { get; set; }
		protected virtual object GetEditValueInternal() {
			return ValueContainer.EditValue;
		}
		public object EditValue {
			get { return GetEditValueInternal(); }
		}
		protected BaseEditingSettingsService TextEditingSettings { get { return PropertyProvider.GetService<BaseEditingSettingsService>(); } }
		protected virtual bool IsNullTextSupported { get { return false; } }
		protected bool ShouldShowEmptyText { get { return ShouldShowEmptyTextInternal(ValueContainer.EditValue); } }
		protected bool ShouldShowNullText { get { return ShouldShowNullTextInternal(ValueContainer.EditValue); } }
		protected virtual bool ApplyDisplayTextConversion { get { return !Editor.FocusManagement.IsFocusWithin || !AllowKeyHandling || !AllowEditing; } }
		protected virtual bool ShouldSyncWithEditor { get { return !editBoxTextLocker.IsLocked && !syncWithEditorLocker.IsLocked && AllowEditing; } }
		protected bool AllowEditing { get { return TextEditingSettings.AllowEditing; } }
		protected bool AllowKeyHandling { get { return TextEditingSettings.AllowKeyHandling; } }
		ValueChangingService ValueChangingService { get { return PropertyProvider.GetService<ValueChangingService>(); } }
		public virtual bool IsValueChanged {
			get { return ValueChangingService.IsValueChanged || ValueContainer.HasValueCandidate; }
			set {
				if (value && ApplyDisplayTextConversion)
					ValueChangingService.SetIsValueChanged(true);
				if (!value) {
					ResetIsValueChanged();
				}
			}
		}
		protected internal virtual bool AllowTextInput { get { return AllowEditing; } }
		protected internal virtual bool ShouldApplyNullTextToDisplayText { get { return true; } }
		protected bool ShouldLockUpdate { get { return IsLockedByValueChanging || IsInSupportInitialize; } }
		protected bool ShouldLockRaiseEvents { get { return raiseEventsLocker.IsLocked; } }
		bool ShouldUpdateErrorProvider { get { return !ValueContainer.IsLockedByValueChanging && !ValueContainer.IsPostponedValueChanging; } }
		public bool IsInSupportInitialize { get { return isInSupportInitialize.IsLocked; } }
		public bool IsInPostponedUpdate { get { return ValueContainer.IsPostponedValueChanging; } }
		protected internal virtual bool IsLockedByValueChanging { get { return editValueLocker.IsLocked || coerceValueLocker.IsLocked; } }
		protected ValueContainerService ValueContainer { get { return PropertyProvider.GetService<ValueContainerService>(); } }
		protected BaseEdit Editor { get; private set; }
		protected BaseEditSettings Settings { get { return Editor.Settings; } }
		protected ActualPropertyProvider PropertyProvider { get { return Editor.PropertyProvider; } }
		protected BaseEditStyleSettings StyleSettings { get { return PropertyProvider.StyleSettings; } }
		public EditorSpecificValidator Validator { get { return PropertyProvider.GetService<EditorSpecificValidator>(); } }
		protected internal PropertyCoercionHelper PropertyUpdater { get; private set; }
		protected PostponedAction RemoveNullTextFromUndoStack { get; private set; }
		protected ImmediateActionsManager ImmediateActionsManager { get {return Editor.ImmediateActionsManager; } }
		protected EditStrategyBase(BaseEdit editor) {
			Editor = editor;
			ValidationState = new ValidationState(this, editor);
			PropertyUpdater = new PropertyCoercionHelper(Editor);
			RemoveNullTextFromUndoStack = new PostponedAction(() => PropertyProvider.IsNullTextVisible);
			InitializeServices();
			RegisterUpdateCallbacks();
			ApplyStyleSettingsAction = new PostponedAction(() => IsInSupportInitialize);
		}
		public virtual void StyleSettingsChanged(BaseEditStyleSettings settings) {
			Editor.Settings.SetCurrentValue(BaseEditSettings.StyleSettingsProperty, StyleSettings);			
			ApplyStyleSettings(StyleSettings);
		}
		public void ApplyStyleSettings(BaseEditStyleSettings settings) {
			ApplyStyleSettingsAction.PerformPostpone(() => {
				ApplyStyleSettingsInternal(settings);
				AfterApplyStyleSettings();
			});
		}
		protected virtual void ApplyStyleSettingsInternal(BaseEditStyleSettings settings) {
			if (settings == null)
				return;
			settings.ApplyToEdit(Editor);
		}
		protected virtual void AfterApplyStyleSettings() {
		}
		protected internal virtual bool ShouldShowNullTextInternal(object editValue) {
			return IsNullTextSupported && PropertyProvider.ShowNullText && IsNullValue(editValue) && ApplyDisplayTextConversion && !PropertyProvider.HasDisplayTextProviderText;
		}
		protected internal virtual bool ShouldShowEmptyTextInternal(object editValue) {
			return IsNullTextSupported && PropertyProvider.ShowNullText && IsNullValue(editValue) && !ApplyDisplayTextConversion && !ValueContainer.HasValueCandidate && ShouldSyncWithEditor;
		}
		protected internal virtual void ProcessEditModeChanged(EditMode oldValue, EditMode newValue) {
			UpdateEditorOnEditingChange(newValue != EditMode.InplaceInactive);
		}
		protected internal virtual void UpdateEditorOnEditingChange(bool syncWithValue) {
			DoValidate(UpdateEditorSource.LostFocus);
			if (syncWithValue)
				SyncWithValue();
		}
		protected virtual void RegisterUpdateCallbacks() {
			PropertyUpdater.Register(BaseEdit.EditValueProperty, baseValue => baseValue, baseValue => baseValue);
		}
		public bool DoValidate(UpdateEditorSource updateSource) {
			PrepareForCheckAllowLostKeyboardFocus();
			bool result = DoValidateInternal(EditValue, updateSource);
			UpdateDisplayText();
			return result;
		}
		public virtual FrameworkElement CreateValidationToolTip(BaseValidationError error, bool isToolTip) {
			return isToolTip ?
				CreateToolTipCore(error, Editor.ErrorToolTipContentTemplate) :
				CreateErrorControlCore(error, Editor.ErrorToolTipContentTemplate);
		}
		FrameworkElement CreateToolTipCore(object toolTip, DataTemplate contentTemplate) {
			return toolTip != null ? new ToolTip { Content = toolTip, ContentTemplate = contentTemplate } : null;
		}
		FrameworkElement CreateErrorControlCore(object toolTip, DataTemplate contentTemplate) {
			return toolTip != null ? new ContentControl { Content = toolTip, ContentTemplate = contentTemplate } : null;
		}
		public virtual object CoerceBaseValidationError(BaseValidationError error) {
			if (ValidationState.EditorValidationError != null)
				return ValidationState.EditorValidationError;
			if (Editor.Extractor != null && Editor.Extractor.ValidationError != null)
				return Editor.Extractor.ValidationError;
			return error;
		}
		public void SetEditValueForce(object editValue) {
			ResetValidationError();
			BaseEditHelper.SetCurrentValue(Editor, BaseEdit.EditValueProperty, editValue);
		}
		public virtual bool ResetValidationError() {
			if (!ValueContainer.HasValueCandidate || IsInPostponedUpdate)
				return false;
			ValueContainer.Reset();
			ValidationState.Reset();
			ResetValidationErrorInternal();
			return true;
		}
		protected virtual void ResetValidationErrorInternal() {
		}
		public void SupportInitializeBeginInit() {
			isInSupportInitialize.Lock();
			LockRaiseValueChangingEvents();
		}
		public void SupportInitializeEndInit() {
			isInSupportInitialize.Unlock();
			OnInitialized();
			UnlockRaiseValueChangingEvents();
		}
		public virtual void OnInitialized() {
			LogBase.Add(Editor, null, "EditStrategy.OnInitialized");
			ApplyStyleSettingsAction.Perform();
			PerformUpdateValueOnInitialized();
		}
		public virtual void PerformUpdateValueOnInitialized() {
			SyncWithValue();
		}
		public virtual object ConvertToBaseValue(object value) {
			return value;
		}
		public virtual void Initialize() {
		}
		public virtual void Release() {
		}
		public void ResetErrorProvider() {
			UpdateErrorRepresentation();
		}
		public virtual void DisplayTextChanged(string displayText) {
			UpdateToolTip();
		}
		public virtual void UpdateDisplayText() {
			if (ShouldRestoreCursorPosition())
				UpdateDisplayTextAndRestoreCursorPosition();
			else 
				UpdateDisplayTextInternal();
		}
		protected virtual void UpdateDisplayTextAndRestoreCursorPosition() {
			UpdateDisplayTextInternal();
		}
		protected virtual bool ShouldRestoreCursorPosition() {
			return false;
		}
		protected virtual void UpdateDisplayTextInternal() {
			PropertyProvider.SetDisplayText(CoerceDisplayText(null));
			UpdateEditCoreText(PropertyProvider.DisplayText);
			Editor.IsNullTextVisible = ShouldShowNullText;
			if (PropertyProvider.SuppressFeatures)
				return;
			Editor.DisplayText = PropertyProvider.DisplayText;
		}
		public virtual string CoerceDisplayText(string displayText) {
			return displayText;
		}
		public virtual void SetNullValue(object parameter) {
			ValueContainer.UndoTempValue();
			ValueContainer.SetEditValue(Editor.NullValue, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
			LogBase.Add(Editor, parameter);
		}
		protected virtual string GetDisplayText() {
			return Editor.GetDisplayText(GetValueForDisplayText(), ApplyDisplayTextConversion);
		}
		protected virtual object GetEditValueForEditText() {
			return PropertyProvider.InputTextToEditValueConverter.ConvertBack(GetEditValueForEditText());
		}
		protected virtual object GetEditValueForEditTextInternal() {
			return null;
		}
		protected internal virtual bool IsNullValue(object value) {
			return IsNativeNullValue(value) || (IsStringEmpty(value) && PropertyProvider.ShowNullTextForEmptyValue) || object.Equals(value, PropertyProvider.NullValue);
		}
		protected virtual bool IsNativeNullValue(object value) {
			return BaseEditSettings.IsNativeNullValue(value);
		}
		bool IsStringEmpty(object value) {
			return value is string && string.IsNullOrEmpty(value as string);
		}
		protected virtual object GetValueForDisplayText() {
			object editValue = ValueContainer.EditValue;
			return PropertyProvider.InputTextToEditValueConverter.ConvertBack(editValue);
		}
		protected virtual void UpdateEditCoreText(string displayText) {
			if (Editor.EditCore == null)
				return;
			syncWithEditorLocker.DoLockedAction(() => UpdateEditCoreTextInternal(displayText));
		}
		protected virtual void UpdateEditCoreTextInternal(string displayText) {
			if (Editor.EditCore is TextBlock)
				Editor.EditCore.SetValue(TextBlock.TextProperty, String.IsNullOrEmpty(displayText) ? " " : displayText);
			else if (Editor.EditCore is TextBox)
				(Editor.EditCore as TextBox).Text = displayText; 
		}
		void UpdateErrorProvider(bool isValid, BaseValidationError error) {
			ValidationState.Reset();
			PropertyProvider.SetHiddenValidationError(null);
			if (!isValid) {
				if (error.IsHidden)
					PropertyProvider.SetHiddenValidationError(error);
				else
					ValidationState.Initialize(false, error);
			}
			UpdateErrorRepresentation();
		}
		void UpdateErrorRepresentation() {
			Editor.CoerceValue(BaseEdit.ValidationErrorPropertyKey.DependencyProperty);
			CoerceToolTip();
		}
		public virtual void OnGotFocus() {
			IsValueChanged = false;
			MouseEventLockHelper.SubscribeMouseEvents(Editor);
			RemoveNullTextFromUndoStack.PerformPostpone(ClearUndoStack);
		}
		public virtual void AfterOnGotFocus() {
			RemoveNullTextFromUndoStack.Perform();
		}
		protected virtual void ClearUndoStack() {
		}
		public virtual void OnLostFocus() {
			if (IsValueChanged) {
				Editor.IsValueChanged = IsValueChanged;
				IsValueChanged = false;
			}
		}
		public virtual void OnLoaded() {
			DoValidate(UpdateEditorSource.ValueChanging);
			ApplyStyleSettings(StyleSettings);
		}
		protected internal bool DoValidateInternal(object value, UpdateEditorSource updateSource) {
			if (PropertyProvider.SuppressFeatures) {
				UpdateEditValue(value, updateSource, true);
				return true;
			}
			ValidationState.TryReset(updateSource);
			if (!UpdateSourceValidation(updateSource))
				return false;
			if (!ValidationState.IsEmpty)
				return ValidationState.IsValid;
			object convertedValue = null;
			BaseValidationError error = ProcessValueConversion(value, out convertedValue, updateSource);
			if (error != null) {
				UpdateErrorProvider(false, error);
				return false;
			}
			ValidationEventArgs args = new ValidationEventArgs(BaseEdit.ValidateEvent, this, convertedValue, CultureInfo.CurrentCulture, updateSource);
			if (!Validator.DoValidate(value, convertedValue, updateSource)) {
				args.IsValid = false;
				args.ErrorContent = Validator.GetValidationError();
				UpdateErrorProvider(args.IsValid, CreateValidationError(args));
				return false;
			}
			RaiseValidateEvent(args, updateSource);
			UpdateEditValue(convertedValue, updateSource, args.IsValid);
			UpdateErrorProvider(args.IsValid, CreateValidationError(args));
			return args.IsValid;
		}
		BaseValidationError ProcessValueConversion(object baseValue, out object convertedValue, UpdateEditorSource updateSource) {
			convertedValue = ValueContainer.ProcessConversion(baseValue, updateSource);
			if (PropertyProvider.InputTextToEditValueConverter.ValidationError != null)
				return PropertyProvider.InputTextToEditValueConverter.ValidationError;
			return PropertyProvider.ValueTypeConverter.ValidationError;
		}
		void UpdateEditValue(object editValue, UpdateEditorSource updateSource, bool isValid) {
			if (ShouldFlushEditValue(updateSource, isValid))
				ValueContainer.FlushEditValueCandidate(editValue, updateSource);
		}
		bool ShouldFlushEditValue(UpdateEditorSource updateSource, bool isValid) {
			return isValid && (updateSource == UpdateEditorSource.DoValidate || updateSource == UpdateEditorSource.LostFocus || updateSource == UpdateEditorSource.EnterKeyPressed);
		}
		public virtual bool ShouldResetValidateState(UpdateEditorSource updateSource) {
			return updateSource != UpdateEditorSource.DontValidate;
		}
		BaseValidationError CreateValidationError(ValidationEventArgs args) {
			return new BaseValidationError(args.ErrorContent, args.Exception, args.ErrorType) { IsHidden = (args.ErrorContent as string).Return(x => x == "hidden error", () => false) };
		}
		protected virtual string CreateStrategyErrorContent() {
			return string.Empty;
		}
		void RaiseValidateEvent(ValidationEventArgs args, UpdateEditorSource updateSource) {
			if (updateSource == UpdateEditorSource.DontValidate || !Editor.CausesValidation)
				return;
			try {
				Editor.RaiseEvent(args);
			}
			catch (Exception e) {
				args.IsValid = false;
				args.Exception = e;
				args.ErrorType = ErrorType.Critical;
#if DEBUGTEST
				args.ErrorContent = e.ToString();
#endif
			}
		}
		public void SyncWithValue() {
			if (ShouldLockUpdate)
				return;
			try {
				editValueLocker.Lock();
				PropertyUpdater.Update();
				SyncWithValueInternal();
			}
			finally {
				editValueLocker.Unlock();
			}
		}
		public void SyncWithValue(DependencyProperty dp, object oldValue, object newValue) {
			if (ShouldLockUpdate)
				return;
			try {
				editValueLocker.Lock();
				ResetValidationError();
				PropertyUpdater.Update(dp, oldValue, newValue);
				SyncWithValueInternal();
			}
			finally {
				editValueLocker.Unlock();
			}
		}
		protected virtual void SyncWithValueInternal() {
			UpdateDisplayText();
		}
		public void SyncEditCoreProperties() {
			SyncEditCorePropertiesInternal();
		}
		protected virtual void SyncEditCorePropertiesInternal() {
			UpdateDisplayText();
		}
		protected void LockRaiseValueChangingEvents() {
			raiseEventsLocker.Lock();
		}
		protected void UnlockRaiseValueChangingEvents() {
			raiseEventsLocker.Unlock();
		}
		public virtual void RaiseValueChangedEvents(object oldValue, object newValue) {
			if (ShouldLockRaiseEvents || object.Equals(oldValue, newValue))
				return;
			Editor.RaiseEvent(new EditValueChangedEventArgs(oldValue, newValue));
		}
		public virtual bool RaiseValueChangingEvents(object oldValue, object newValue) {
			if (ShouldLockRaiseEvents)
				return true;
			EditValueChangingEventArgs valueChangingEventArgs = new EditValueChangingEventArgs(oldValue, newValue);
			Editor.RaiseEvent(valueChangingEventArgs);
			if (!valueChangingEventArgs.Handled)
				return true;
			return !valueChangingEventArgs.IsCancel;
		}
		public void SyncWithEditor() {
			if (ShouldLockUpdate || !ShouldSyncWithEditor)
				return;
			try {
				editBoxTextLocker.Lock();
				SyncWithEditorInternal();
			}
			finally {
				editBoxTextLocker.Unlock();
			}
		}
		protected virtual void SyncWithEditorInternal() {
			string editText = (string)GetEditableObject();
			object editValue = string.IsNullOrEmpty(editText) && ReplaceTextWithNull(editText) ? Editor.NullValue : editText;
			editValue = ValueContainer.ConvertEditTextToEditValueCandidate(editValue);
			ValueContainer.SetEditValue(editValue, UpdateEditorSource.TextInput);
		}
		protected internal virtual bool ReplaceTextWithNull(object editValue) {
			return IsNullTextSupported && Editor.ShowNullText && IsNullValue(editValue) && !ApplyDisplayTextConversion && !ValueContainer.HasValueCandidate;
		}
		protected virtual object GetEditableObject() {
			return null;
		}
		public virtual object CoerceEditValue(object value) {
			return CoerceValue(BaseEdit.EditValueProperty, value);
		}
		public virtual void EditValueChanged(object oldValue, object newValue) {
			UpdateEditValue(
				oldValue,
				newValue,
				(value1, value2) => SyncWithValue(BaseEdit.EditValueProperty, value1, value2), false);
		}
		public virtual object CoerceValue(DependencyProperty dp, object value) {
			if (IsInSupportInitialize)
				PropertyUpdater.SetSyncValue(dp, value, SyncValueUpdateMode.Default);
			if (ShouldLockUpdate)
				PropertyUpdater.SetSyncValue(dp, value, SyncValueUpdateMode.Update);
			return value;
		}
		public virtual object CoerceMaskType(MaskType maskType) {
			return maskType;
		}
		public virtual object CoerceText(string text) {
			return CoerceValue(TextEdit.TextProperty, text);
		}
		protected virtual object GetOnInitializedSyncValue() {
			return ValueContainer.EditValue;
		}
		protected virtual object GetNullableValue() {
			return Editor.NullValue;
		}
		protected virtual object GetDefaultValue() {
			return null;
		}
		public virtual void ThemeChanged() {
			ApplyStyleSettings(StyleSettings);
		}
		protected internal virtual bool ShouldProcessNullInput(KeyEventArgs e) {
			return Editor.AllowNullInput && !Editor.IsReadOnly && AllowKeyHandling && ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)) && IsNullInputActivatingKey(e.Key);
		}
		protected virtual bool IsNullInputActivatingKey(Key key) {
#if !SL
			if (FrenchKeyboardDetector.IsFrenchKeyboard || GermanKeyboardDetector.IsGermanKeyboard)
				return key == Key.Delete;
#endif
			return key == Key.Delete || key == Key.D0;
		}
		protected virtual void PerformNullInput() {
			ValueContainer.SetEditValue(Editor.NullValue, UpdateEditorSource.TextInput);
			UpdateDisplayText();
		}
		public void ProcessPreviewKeyDown(KeyEventArgs e) {
			if (!AllowKeyHandling)
				return;
			if (ShouldProcessNullInput(e)) {
				PerformNullInput();
				e.Handled = true;
				return;
			}
			ProcessPreviewKeyDownInternal(e);
		}
		protected virtual void ProcessPreviewKeyDownInternal(KeyEventArgs e) {
		}
		bool UpdateSourceValidation(UpdateEditorSource updateSource) {
			if (updateSource == UpdateEditorSource.DontValidate)
				return false;
			if (updateSource == UpdateEditorSource.ValueChanging || updateSource == UpdateEditorSource.DoValidate)
				return true;
			if (updateSource == UpdateEditorSource.TextInput && !Editor.FocusManagement.IsFocusWithin)
				return true;
			if (Editor.ValidateOnTextInput)
				return true;
			if (Editor.ValidateOnEnterKeyPressed)
				return updateSource == UpdateEditorSource.EnterKeyPressed || updateSource == UpdateEditorSource.LostFocus;
			return updateSource == UpdateEditorSource.LostFocus;
		}
		protected internal virtual void PrepareForCheckAllowLostKeyboardFocus() {
		}
		public virtual void UpdateDataContext(DependencyObject target) {
			BaseEdit.SetOwnerEdit(target, Editor);
		}
		protected internal virtual void OnNullTextChanged(string nullText) {
			SyncWithValue();
		}
		protected internal virtual void OnNullValueChanged(object nullValue) {
			SyncWithValue();
		}
		public virtual FrameworkElement CreateTrimmedTextToolTip(string toolTip, bool isToolTip) {
			return isToolTip ?
				CreateToolTipCore(new TrimmedTextToolTipContent(toolTip), Editor.TrimmedTextToolTipContentTemplate) :
				CreateErrorControlCore(new TrimmedTextToolTipContent(toolTip), Editor.TrimmedTextToolTipContentTemplate);
		}
#if !SL
		void ForceClose(ToolTip toolTip) {
			toolTip.SetCurrentValue(ToolTip.VisibilityProperty, Visibility.Collapsed);
			toolTip.IsOpen = false;
			RoutedEventHandler toolTipClosedDelegate = null;
			toolTipClosedDelegate = delegate(object sender, RoutedEventArgs e) {
				toolTip.Closed -= toolTipClosedDelegate;
				toolTip.SetCurrentValue(ToolTip.VisibilityProperty, Visibility.Visible);
			};
			toolTip.Closed += toolTipClosedDelegate;
		}
		public void CoerceToolTip() {
			Editor.CoerceValue(BaseEdit.ToolTipProperty);
		}
		internal void ResetToolTipContent() {
			tooltipContent = null;
		}
		void UpdateToolTip() {
			if (Editor.EditCore is TextBlock && TextBlockService.GetIsTextTrimmed((TextBlock)Editor.EditCore))
				CoerceToolTip();
		}
		#region internal toolTip hack
		FrameworkElement tooltipContent;
		public virtual object CoerceValidationToolTip(object tooltip) {
			FrameworkElement result = CreateHackedContainerForInternalToolTip(tooltip);
			bool hasValidationToolTip = true;
			if (Editor.ShowErrorToolTip) {
				if (ValidationState.EditorValidationError != null)
					result = ValidationState.GetValidationToolTip(false);
				else if (Editor.Extractor != null && Editor.Extractor.ValidationError != null)
					result = Editor.Extractor.GetValidationToolTip(false);
				else if (Editor.ValidationError != null)
					result = CreateValidationToolTip(Editor.ValidationError, false);
				else
					hasValidationToolTip = false;
			}
			if (!hasValidationToolTip && IsTextTrimmed())
				result = CreateTrimmedTextToolTip(PropertyProvider.DisplayText, tooltip is ToolTip);
			ClosePreviousToolTip(tooltip, result);
			tooltipContent = result;
			return result;
		}
		void ClosePreviousToolTip(object baseToolTip, object coercedToolTip) {
			ToolTip tt = LayoutHelper.FindLayoutOrVisualParentObject<ToolTip>(tooltipContent);
			if (tt != null)
				ForceClose(tt);
		}
		FrameworkElement CreateHackedContainerForInternalToolTip(object tooltip) {
			if (tooltip is ToolTip || tooltip == null)
				return (FrameworkElement)tooltip;
			ContentControl result = new ContentControl();
			FrameworkElement frameworkToolTip = tooltip as FrameworkElement;
			if (frameworkToolTip != null) {
				ContentControl control = frameworkToolTip.Tag as ContentControl;
				if (control != null)
					control.Content = null;
			}
			result.Content = tooltip;
			if (frameworkToolTip != null)
				frameworkToolTip.Tag = result;
			return result;
		}
		bool IsTextTrimmed() {
			return Editor.EditCore is TextBlock ? TextBlockService.GetIsTextTrimmed((TextBlock)Editor.EditCore) : false;
		}
		#endregion
#endif
#if DEBUGTEST
#endif
		protected internal virtual object ConvertEditValueForFormatDisplayText(object convertedValue) {
			return ValueContainer.ConvertEditValueForFormatDisplayText(convertedValue);
		}
		protected virtual string FormatDisplayTextFast(object editValue) {
			return editValue != null ? editValue.ToString() : string.Empty;
		}
		internal string TryFormatDisplayText(string formatString, object editValue) {
			try {
				if (editValue is decimal && (formatString == "{0:d}" || formatString == "{0:D}"))
					return editValue.ToString();
				return string.Format(CultureInfo.CurrentCulture, formatString, editValue);
			}
			catch {
				return FormatDisplayTextFast(editValue);
			}
		}
		public virtual void ResetIsValueChanged() {
			ValueChangingService.SetIsValueChanged(false);
		}
		public void OnIsNullTextVisibleChanged(bool isVisible) {
			UpdateNullTextForeground(isVisible);
			UpdateAllowDrop(isVisible);
		}
		public virtual void UpdateAllowDrop(bool isVisible) {
		}
		public virtual void UpdateNullTextForeground(bool isVisible) {
			UpdateNullTextForegroundCore(isVisible);
		}
		partial void UpdateNullTextForegroundCore(bool isVisible);
		public virtual bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			return ValueContainer.ProvideEditValue(value, out provideValue, updateSource);
		}
		protected internal virtual void AddILogicalOwnerChild(object child) {
		}
		protected internal virtual void RemoveILogicalOwnerChild(object child) {
		}
		public virtual void OnMouseWheel(MouseWheelEventArgs e) {
		}
		public virtual void PreviewMouseDown(MouseButtonEventArgs e) {
		}
		public virtual void PreviewMouseUp(MouseButtonEventArgs e) {
		}
		public virtual void EditValuePostDelayChanged(int value) {
			ValueContainer.UpdatePostMode();
		}
		public virtual void EditValuePostModeChanged(PostMode value) {
			ValueContainer.UpdatePostMode();
		}
		public virtual void IsEditorActiveChanged(bool value) {
		}
		public virtual void EditValueTypeChanged(Type type) {
			UpdateEditValue(Editor.EditValue, (oldValue, newValue) => SyncWithValue(), true);
		}
		public virtual void EditValueConverterChanged(IValueConverter converter) {
			UpdateEditValue(Editor.EditValue, (oldValue, newValue) => SyncWithValue(), true);
		}
		public virtual void InputTextToEditValueConverterChanged(IValueConverter converter) {
			PropertyProvider.SetInputTextToEditValueConverter(CreateInputTextConverter());
		}
		void UpdateEditValue(object newValue, Action<object, object> syncWithValueCallback, bool updateConverters = false) {
			UpdateEditValue(null, newValue, syncWithValueCallback, updateConverters);
		}
		protected virtual void UpdateEditValue(object oldValue, object newValue, Action<object, object> syncWithValueCallback, bool updateConverters) {
			if (ValueContainer.HasTempValue && !IsInPostponedUpdate)
				ValueContainer.UndoTempValue();
			if (updateConverters)
				UpdateValueConverters();
			if (ShouldUpdateErrorProvider)
				ResetValidationError();
			if (ShouldLockUpdate) {
				DoValidateInternal(newValue, UpdateEditorSource.ValueChanging);
				UpdateDisplayText();
				return;
			}
			syncWithValueCallback(oldValue, newValue);
			if (ShouldUpdateErrorProvider)
				DoValidateInternal(newValue, UpdateEditorSource.ValueChanging);
		}
		protected virtual void UpdateValueConverters() {
			PropertyProvider.SetInputTextToEditValueConverter(CreateInputTextConverter());
			PropertyProvider.SetValueTypeConverter(CreateValueTypeConverter());
		}
		protected virtual ValueTypeConverter CreateValueTypeConverter() {
			return new ValueTypeConverter() { TargetType = Editor.EditValueType, };
		}
		protected virtual ValueTypeConverter CreateInputTextConverter() {
			return new ValueTypeConverter() { TargetType = Editor.EditValueType,  };
		}
		public virtual void FocusEditCore() {
			Utils.KeyboardHelper.Focus(Editor.EditCore);
		}
		protected virtual void InitializeServices() {
			PropertyProvider.RegisterService(CreateValueContainerService());
			PropertyProvider.RegisterService(CreateValidationService());
			PropertyProvider.RegisterService(CreateEditorValidatorService());
			PropertyProvider.RegisterService(CreateTextInputSettingsService());
			PropertyProvider.RegisterService(CreateValueChangingService());
		}
		protected virtual ValueContainerService CreateValueContainerService() {
			return new ValueContainerService(Editor);
		}
		protected virtual ValueChangingService CreateValueChangingService() {
			return new ValueChangingService(Editor);
		}
		protected virtual BaseEditingSettingsService CreateTextInputSettingsService() {
			return new BaseEditingSettingsService(Editor);
		}
		protected virtual EditorSpecificValidator CreateEditorValidatorService() {
			return new EditorSpecificValidator(Editor);
		}
		protected virtual Services.ValidationService CreateValidationService() {
			return new Services.ValidationService(Editor);
		}
		protected internal virtual string FormatDisplayText(object editValue, bool applyFormatting) {
			if (ShouldApplyNullTextToDisplayText) {
				if (ShouldShowNullTextInternal(editValue))
					return Editor.NullText;
				if (ShouldShowEmptyTextInternal(editValue))
					return string.Empty;
			}
			return FormatDisplayTextInternal(editValue, applyFormatting);
		}
		protected virtual string FormatDisplayTextInternal(object editValue, bool applyFormatting) {
			object convertedValue = ConvertEditValueForFormatDisplayText(editValue);
			if (!applyFormatting || string.IsNullOrEmpty(PropertyProvider.DisplayFormatString))
				return FormatDisplayTextFast(convertedValue);
			return TryFormatDisplayText(FormatStringConverter.GetDisplayFormat(PropertyProvider.DisplayFormatString), convertedValue);
		}
		protected internal virtual bool NeedsEnterKey(ModifierKeys modifiers) {
			return false;
		}
		protected internal virtual void OnUnloaded() {
		}
		protected internal virtual bool DoTextSearch(string text, int startIndex, ref object result) {
			return false;
		}
	}
	public class InvalidInputValue : ICustomItem {
		public object EditValue { get; set; }
		public object DisplayValue { get; set; }
	}
}
