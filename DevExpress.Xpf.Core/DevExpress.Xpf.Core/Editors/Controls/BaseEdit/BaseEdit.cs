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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Editors {
	public interface IDisplayTextProvider {
		bool? GetDisplayText(string originalDisplayText, object value, out string displayText);
	}
	public enum PostMode { Immediate, Delayed }
#if DEBUGTEST
	public
#else
	internal 
#endif
 static class PostModeHelper {
		public static bool SuppressDelayedPost { get; set; }
	}
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[DefaultBindingProperty("EditValue")]
#if !SILVERLIGHT
#endif
	public abstract partial class BaseEdit : Control, IBaseEdit, ISupportDXValidation, IExportSettings, ILogicalOwner
#if SL
		, ILogicalOwnerEx
#endif
 {
		#region static
		public static readonly DependencyProperty AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty;
		public static readonly DependencyProperty EditValueTypeProperty;
		public static readonly DependencyProperty EditValueConverterProperty;
		public static readonly DependencyProperty InputTextToEditValueConverterProperty;
		internal static readonly DependencyPropertyKey ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty;
		public static readonly DependencyProperty DisableExcessiveUpdatesInInplaceInactiveModeProperty;
		public static readonly DependencyProperty EditValuePostDelayProperty;
		public static readonly DependencyProperty EditValuePostModeProperty;
		public static readonly DependencyProperty AllowNullInputProperty;
		static readonly DependencyPropertyKey IsNullTextVisiblePropertyKey;
		public static readonly DependencyProperty IsNullTextVisibleProperty;
		public static readonly DependencyProperty ShowNullTextProperty;
		public static readonly DependencyProperty NullTextProperty;
		public static readonly DependencyProperty NullValueProperty;
		public static readonly DependencyProperty ShowNullTextForEmptyValueProperty;
		public static readonly RoutedEvent CustomDisplayTextEvent;
		public static readonly DependencyProperty DisplayTextProperty;
		protected static readonly DependencyPropertyKey DisplayTextPropertyKey;
		public static readonly DependencyProperty DisplayFormatStringProperty;
		public static readonly DependencyProperty DisplayTextConverterProperty;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		protected static readonly DependencyPropertyKey EditModePropertyKey;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty EditModeProperty;
		public static readonly DependencyProperty DisplayTemplateProperty;
		public static readonly DependencyProperty EditTemplateProperty;
		public static readonly RoutedEvent EditValueChangedEvent;
		public static readonly RoutedEvent EditValueChangingEvent;
		public static readonly DependencyProperty IsReadOnlyProperty;
		public static readonly RoutedEvent EditorActivatedEvent;
		protected static readonly DependencyPropertyKey OwnerEditPropertyKey;
		public static readonly DependencyProperty OwnerEditProperty;
		public static readonly DependencyProperty ShowErrorProperty;
		public static readonly DependencyProperty ShowErrorToolTipProperty;
		static readonly DependencyPropertyKey HasValidationErrorPropertyKey;
		public static readonly DependencyProperty HasValidationErrorProperty;
		protected internal static readonly DependencyPropertyKey ValidationErrorPropertyKey;
		public static readonly DependencyProperty ValidationErrorProperty;
		public static readonly DependencyProperty ErrorToolTipContentTemplateProperty;
		public static readonly DependencyProperty ValidateOnEnterKeyPressedProperty;
		public static readonly DependencyProperty ValidateOnTextInputProperty;
		public static readonly DependencyProperty CausesValidationProperty;
		public static readonly DependencyProperty InvalidValueBehaviorProperty;
		public static readonly DependencyProperty IsPrintingModeProperty;
		protected static readonly DependencyPropertyKey ActualEditorControlTemplatePropertyKey;
		public static readonly DependencyProperty ActualEditorControlTemplateProperty;
		public static readonly DependencyProperty TrimmedTextToolTipContentTemplateProperty;
		public static readonly DependencyProperty IsEditorActiveProperty;
		static readonly DependencyPropertyKey IsEditorActivePropertyKey;
		public static readonly RoutedEvent ValidateEvent;
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty StyleSettingsProperty;
		public static readonly DependencyProperty ValidationErrorTemplateProperty;
		public static readonly DependencyProperty AllowUpdateTextBlockWhenPrintingProperty;
#if DEBUGTEST
		internal static int InstanceCount;
#endif
		static BaseEdit() {
			Type ownerType = typeof(BaseEdit);
			AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty = DependencyPropertyManager.Register("AllowUpdateTwoWayBoundPropertiesOnSynchronization", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			EditValueTypeProperty = DependencyPropertyManager.Register("EditValueType", typeof(Type), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseEdit)d).EditValueTypeChanged((Type)e.NewValue)));
			EditValueConverterProperty = DependencyPropertyManager.Register("EditValueConverter", typeof(IValueConverter), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseEdit)d).EditValueConverterChanged((IValueConverter)e.NewValue)));
			InputTextToEditValueConverterProperty = DependencyPropertyManager.Register("InputTextToEditValueConverter", typeof(IValueConverter), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseEdit)d).InputTextToEditValueConverterChanged((IValueConverter)e.NewValue)));
			EditValuePostDelayProperty = DependencyPropertyManager.Register("EditValuePostDelay", typeof(int), ownerType, new FrameworkPropertyMetadata(1000, (d, e) => ((BaseEdit)d).EditValuePostDelayChanged((int)e.NewValue)));
			EditValuePostModeProperty = DependencyPropertyManager.Register("EditValuePostMode", typeof(PostMode), ownerType, new FrameworkPropertyMetadata(PostMode.Immediate, (d, e) => ((BaseEdit)d).EditValuePostModeChanged((PostMode)e.NewValue)));
			ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ShouldDisableExcessiveUpdatesInInplaceInactiveMode", typeof(bool), ownerType, new PropertyMetadata(ActualPropertyProvider.ShouldDisableExcessiveUpdatesInInplaceInactiveModeDefaultValue, (d, e) => ((BaseEdit)d).ShouldDisableExcessiveUpdatesInInplaceInactiveModeChanged((bool)e.NewValue)));
			ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty = ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey.DependencyProperty;
			DisableExcessiveUpdatesInInplaceInactiveModeProperty = DependencyPropertyManager.Register("DisableExcessiveUpdatesInInplaceInactiveMode", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseEdit)d).DisableExcessiveUpdatesInInplaceInactiveModeChanged((bool?)e.NewValue)));
			AllowNullInputProperty = DependencyPropertyManager.Register("AllowNullInput", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnAllowNullInputChanged));
			IsNullTextVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsNullTextVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, OnIsNullTextVisiblePropertyChanged));
			IsNullTextVisibleProperty = IsNullTextVisiblePropertyKey.DependencyProperty;
			ShowNullTextProperty = DependencyPropertyManager.Register("ShowNullText", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowNullTextPropertyChanged));
			ShowNullTextForEmptyValueProperty = DependencyPropertyManager.Register("ShowNullTextForEmptyValue", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BaseEdit)d).OnShowNullTextForEmptyValueChanged((bool)e.NewValue)));
			NullTextProperty = DependencyPropertyManager.Register("NullText", typeof(string), ownerType, new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure, OnNullTextPropertyChanged));
			NullValueProperty = DependencyPropertyManager.Register("NullValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnNullValuePropertyChanged));
			CustomDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomDisplayText", RoutingStrategy.Direct, typeof(CustomDisplayTextEventHandler), ownerType);
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(OnBorderTemplatePropertyChanged));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowBorderChanged));
			ShowErrorProperty = DependencyPropertyManager.Register("ShowError", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowErrorChanged));
			ShowErrorToolTipProperty = DependencyPropertyManager.Register("ShowErrorToolTip", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowErrorToolTipChanged));
			DisplayTemplateProperty = DependencyPropertyManager.Register("DisplayTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnDisplayTemplateChanged));
			EditTemplateProperty = DependencyPropertyManager.Register("EditTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnEditTemplateChanged));
			EditModeProperty = DependencyPropertyManager.Register("EditMode", typeof(EditMode), ownerType, new FrameworkPropertyMetadata(EditMode.Standalone, FrameworkPropertyMetadataOptions.None, OnEditModeChanged));
			EditValueChangedEvent = EventManager.RegisterRoutedEvent("EditValueChanged", RoutingStrategy.Direct, typeof(EditValueChangedEventHandler), ownerType);
			EditValueChangingEvent = EventManager.RegisterRoutedEvent("EditValueChanging", RoutingStrategy.Direct, typeof(EditValueChangingEventHandler), ownerType);
			ValidateEvent = EventManager.RegisterRoutedEvent("Validate", RoutingStrategy.Direct, typeof(ValidateEventHandler), ownerType);
			IsReadOnlyProperty = DependencyPropertyManager.Register("IsReadOnly", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsReadOnlyChanged));
			EditorActivatedEvent = EventManager.RegisterRoutedEvent("EditorActivatedEvent", RoutingStrategy.Bubble, typeof(RoutedEventArgs), ownerType);
			OwnerEditPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("OwnerEdit", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			OwnerEditProperty = OwnerEditPropertyKey.DependencyProperty;
			HasValidationErrorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("HasValidationError", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnHasValidationErrorChanged));
			HasValidationErrorProperty = HasValidationErrorPropertyKey.DependencyProperty;
			ValidationErrorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ValidationError", typeof(BaseValidationError), ownerType, new FrameworkPropertyMetadata(null, OnValidationErrorChanged, OnCoerceValidationError));
			ValidationErrorProperty = ValidationErrorPropertyKey.DependencyProperty;
			ErrorToolTipContentTemplateProperty = DependencyPropertyManager.Register("ErrorToolTipContentTemplate", typeof(DataTemplate), ownerType);
			ValidateOnEnterKeyPressedProperty = DependencyPropertyManager.Register("ValidateOnEnterKeyPressed", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ValidateOnTextInputProperty = DependencyPropertyManager.Register("ValidateOnTextInput", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			InvalidValueBehaviorProperty = DependencyPropertyManager.Register("InvalidValueBehavior", typeof(InvalidValueBehavior), ownerType, new FrameworkPropertyMetadata(InvalidValueBehavior.WaitForValidValue));
			CausesValidationProperty = DependencyPropertyManager.Register("CausesValidation", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			IsPrintingModeProperty = DependencyPropertyManager.Register("IsPrintingMode", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnIsPrintingModePropertyChanged));
			ActualEditorControlTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditorControlTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, OnActualEditorControlTemplateChanged));
			ActualEditorControlTemplateProperty = ActualEditorControlTemplatePropertyKey.DependencyProperty;
			IsEditorActivePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsEditorActive", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, OnIsEditorActiveChanged));
			IsEditorActiveProperty = IsEditorActivePropertyKey.DependencyProperty;
			TrimmedTextToolTipContentTemplateProperty = DependencyPropertyManager.Register("TrimmedTextToolTipContentTemplate", typeof(DataTemplate), ownerType);
			System.Windows.Controls.Validation.ErrorTemplateProperty.AddOwner(typeof(BaseEdit), new FrameworkPropertyMetadata(GetDefaultErrorTemplate()));
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEditValueChanged, OnCoerceEditValue, true, UpdateSourceTrigger.LostFocus));
			BorderThicknessProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(Border.BorderThicknessProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			ToolTipProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(OnToolTipPropertyChanged, OnCoerceToolTip));
			DisplayTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("DisplayText", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, OnDisplayTextChanged));
			DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
			DisplayFormatStringProperty = DependencyPropertyManager.Register("DisplayFormatString", typeof(string), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDisplayFormatStringChanged), new CoerceValueCallback(OnCoerceFormatString)));
			DisplayTextConverterProperty = DependencyPropertyManager.Register("DisplayTextConverter", typeof(IValueConverter), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnDisplayTextConverterChanged)));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), typeof(BaseEdit), new PropertyMetadata());
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			StyleSettingsProperty = DependencyPropertyManager.Register("StyleSettings", typeof(BaseEditStyleSettings), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((BaseEdit)d).StyleSettingsChangedInternal((BaseEditStyleSettings)e.NewValue)));
			ValidationErrorTemplateProperty = DependencyPropertyManager.Register("ValidationErrorTemplate", typeof(DataTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d,e)=>((BaseEdit)d).OnValidationErrorTemplateChanged((DataTemplate)e.NewValue)));
			AllowUpdateTextBlockWhenPrintingProperty = DependencyPropertyManager.Register("AllowUpdateTextBlockWhenPrinting", typeof(bool), ownerType, new PropertyMetadata(true));
		}
		internal static bool? NeedsBasicKey(Key key, Func<bool> needsEnterFunc) {
			if (key == Key.Escape || key == Key.F2)
				return false;
			if (key == Key.Enter)
				return needsEnterFunc();
			return null;
		}
		protected virtual ActualPropertyProvider CreateActualPropertyProvider() {
			return new ActualPropertyProvider(this);
		}
#if !SL
		static ControlTemplate GetDefaultErrorTemplate() {
			ControlTemplate ct = new ControlTemplate();
			ct.Seal();
			return ct;
		}
		static void OnToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static object OnCoerceToolTip(DependencyObject d, object value) {
			return ((BaseEdit)d).OnCoerceToolTip(value);
		}
#endif
		static void OnIsNullTextVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)d).OnIsNullTextVisibleChanged((bool)e.NewValue);
		}
		protected static void OnDisplayFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnDisplayFormatStringChanged();
		}
		protected static void OnDisplayTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnDisplayTextChanged((string)e.NewValue);
		}
		protected static object OnCoerceFormatString(DependencyObject obj, object baseValue) {
			return ((BaseEdit)obj).OnCoerceFormatString(baseValue);
		}
		protected static void OnDisplayTextConverterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnDisplayTextConverterChanged((IValueConverter)e.NewValue);
		}
		static void OnAllowNullInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)d).OnAllowNullInputChanged();
		}
		static void OnHasValidationErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is BaseEdit)
				(d as BaseEdit).OnHasValidationErrorChanged();
		}
		static object OnCoerceValidationError(DependencyObject d, object value) {
			if (d is BaseEdit)
				return ((BaseEdit)d).OnCoerceValidationError((BaseValidationError)value);
			return value;
		}
		internal static void SetValidationError(DependencyObject element, BaseValidationError value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ValidationErrorPropertyKey, value);
		}
		internal static void SetValidationErrorTemplate(DependencyObject element, DataTemplate template) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ValidationErrorTemplateProperty, template);
		}
		public static BaseValidationError GetValidationError(DependencyObject element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (BaseValidationError)element.GetValue(ValidationErrorProperty);
		}
		protected static void OnValidationErrorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			SetHasValidationError(obj, e.NewValue != null);
			BaseEdit edit = obj as BaseEdit;
			if (edit != null)
				edit.OnValidationErrorChanged((BaseValidationError)e.NewValue);
		}
		protected static void OnActualValidationErrorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (obj is BaseEdit)
				((BaseEdit)obj).OnActualValidationErrorsChanged((IList)e.NewValue);
		}
		static void SetHasValidationError(DependencyObject element, bool value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(HasValidationErrorPropertyKey, value);
		}
		public static bool GetHasValidationError(DependencyObject element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(HasValidationErrorProperty);
		}
		internal static void SetOwnerEdit(DependencyObject element, BaseEdit value) {
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(OwnerEditPropertyKey, value);
		}
		public static BaseEdit GetOwnerEdit(DependencyObject element) {
			if (element == null)
				throw new ArgumentNullException("element");
			return (BaseEdit)DependencyObjectHelper.GetValueWithInheritance(element, OwnerEditProperty);
		}
		protected static void OnIsReadOnlyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnIsReadOnlyChanged();
		}
		protected static void OnEditValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnEditValueChanged(e.OldValue, e.NewValue);
		}
		protected static object OnCoerceEditValue(DependencyObject obj, object value) {
			return ((BaseEdit)obj).CoerceEditValue(obj, value);
		}
		protected static void OnBorderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnBorderTemplatePropertyChanged();
		}
		protected static void OnShowBorderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnShowBorderChanged();
		}
		protected static void OnShowErrorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnShowErrorChanged();
		}
		protected static void OnShowErrorToolTipChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnShowErrorToolTipChanged();
		}
		protected static void OnNullTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnNullTextChanged((string)e.NewValue);
		}
		protected static void OnNullValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnNullValueChanged(e.NewValue);
		}
		protected static void OnValidationModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnValidationModeChanged();
		}
		protected static void OnEditModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnEditModeChanged((EditMode)e.OldValue, (EditMode)e.NewValue);
		}
		protected static void OnDisplayTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnDisplayTemplateChanged((ControlTemplate)e.NewValue);
		}
		protected static void OnNullTextTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnNullTextTemplateChanged((ControlTemplate)e.NewValue);
		}
		protected static void OnShowNullTextPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnShowNullTextChanged((bool)e.NewValue);
		}
		protected static void OnEditTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnEditTemplateChanged((ControlTemplate)e.NewValue);
		}
		protected static void OnActualEditorControlTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
		}
		protected static void HasValidationErrorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
		}
		static void OnIsPrintingModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue) {
				((BaseEdit)obj).EditMode = EditMode.InplaceInactive;
#if SL
				((BaseEdit)obj).UpdateActualEditorControlTemplate();
#endif
			}
		}
		static void OnIsEditorActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEdit)obj).OnIsEditorActiveChaged((bool)e.NewValue);
		}
		#endregion
		readonly Locker supportInitializeLocker = new Locker();
		BaseEditSettings settings = null;
		protected List<FrameworkElement> additionalInplaceModeElements = new List<FrameworkElement>();
		internal bool IsInSupportInitializing { get { return supportInitializeLocker.IsLocked; } }
		protected IDisplayTextProvider displayTextProvider;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditIsEditorActive")]
#endif
		public bool IsEditorActive {
			get { return (bool)GetValue(IsEditorActiveProperty); }
			protected set { this.SetValue(IsEditorActivePropertyKey, value); }
		}
		FrameworkElement editCore;
		public bool RaiseOnLoading { get; set; }
		internal EditStrategyBase EditStrategy { get; set; }
		internal ContentManagementStrategyBase ContentManagementStrategy { get; private set; }
		internal ContentControl ErrorPresenterInplace { get; set; }
		internal ContentControl ErrorPresenterStandalone { get; set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditEditCore")]
#endif
		public FrameworkElement EditCore {
			get { return editCore; }
			set {
				if (EditCore == value)
					return;
#if SL
				FrameworkElement oldValue = editCore;
#endif
				if (EditCore != null)
					UnsubscribeEditEventsCore();
				editCore = value;
				if (EditCore != null) {
					SubscribeEditEventsCore();
					SyncEditCoreProperties();
				}
				OnEditCoreAssigned();
#if SL
				SLOnEditCoreAssigned(oldValue);
#endif
			}
		}
		bool isValueChanged;
		internal bool IsValueChanged {
			get { return isValueChanged || EditStrategy.IsValueChanged; }
			set {
				isValueChanged = value;
				if (!value)
					EditStrategy.IsValueChanged = false;
			}
		}
		protected internal virtual BaseEditSettings CreateEditorSettings() {
			return EditorSettingsProvider.Default.CreateEditorSettings(GetType());
		}
		protected internal BaseEditSettings Settings {
			get {
				if (settings == null)
					settings = CreateEditorSettings();
				return settings;
			}
		}
		protected virtual bool IsInactiveMode { get { return EditMode == EditMode.InplaceInactive; } }
		protected virtual bool IsInplaceMode { get { return EditMode != EditMode.Standalone; } }
		protected internal void SetSettings(BaseEditSettings settings) {
			UnsubscribeFromSettings(this.settings);
			this.settings = settings;
			SubscribeToSettings(settings);
		}
		protected virtual void AfterSetSettings() {
		}
		void OnEditSettingsChanged() {
			Settings.AssignToEdit(this);
		}
		protected virtual bool ShouldReplaceEditStrategy(EditStrategyBase newStrategy) {
			if (EditStrategy == null)
				return true;
			return !(EditStrategy.GetType() == newStrategy.GetType());
		}
		protected void UpdateActualEditorControlTemplate() {
			ActualEditorControlTemplate = GetActualEditorControlTemplate();
		}
		protected virtual ControlTemplate GetActualEditorControlTemplate() {
#if !SL
			return EditTemplate;
#else
			return SLGetActualEditorControlTemplate();
#endif
		}
		public bool AllowUpdateTwoWayBoundPropertiesOnSynchronization {
			get { return (bool)GetValue(AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty); }
			set { SetValue(AllowUpdateTwoWayBoundPropertiesOnSynchronizationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditAllowNullInput"),
#endif
 Category("Behavior")]
		public bool AllowNullInput {
			get { return (bool)GetValue(AllowNullInputProperty); }
			set { SetValue(AllowNullInputProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditIsNullTextVisible")]
#endif
		public bool IsNullTextVisible {
			get { return (bool)GetValue(IsNullTextVisibleProperty); }
			internal set { this.SetValue(IsNullTextVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditShowNullText")]
#endif
		public bool ShowNullText {
			get { return (bool)GetValue(ShowNullTextProperty); }
			set { SetValue(ShowNullTextProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditShowNullTextForEmptyValue")]
#endif
		public bool ShowNullTextForEmptyValue {
			get { return (bool)GetValue(ShowNullTextForEmptyValueProperty); }
			set { SetValue(ShowNullTextForEmptyValueProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditNullText")]
#endif
		public string NullText {
			get { return (string)GetValue(NullTextProperty); }
			set { SetValue(NullTextProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditNullValue")]
#endif
		public object NullValue {
			get { return GetValue(NullValueProperty); }
			set { SetValue(NullValueProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditActualEditorControlTemplate")]
#endif
		public ControlTemplate ActualEditorControlTemplate {
			get { return (ControlTemplate)GetValue(ActualEditorControlTemplateProperty); }
			protected internal set { this.SetValue(ActualEditorControlTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditIsReadOnly"),
#endif
 Category("Behavior")]
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditDisplayTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate DisplayTemplate {
			get { return (ControlTemplate)GetValue(DisplayTemplateProperty); }
			set { SetValue(DisplayTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditEditTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate EditTemplate {
			get { return (ControlTemplate)GetValue(EditTemplateProperty); }
			set { SetValue(EditTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditErrorToolTipContentTemplate")]
#endif
		public DataTemplate ErrorToolTipContentTemplate {
			get { return (DataTemplate)GetValue(ErrorToolTipContentTemplateProperty); }
			set { SetValue(ErrorToolTipContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditTrimmedTextToolTipContentTemplate")]
#endif
		public DataTemplate TrimmedTextToolTipContentTemplate {
			get { return (DataTemplate)GetValue(TrimmedTextToolTipContentTemplateProperty); }
			set { SetValue(TrimmedTextToolTipContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditValidateOnEnterKeyPressed"),
#endif
 Category("Behavior")]
		public bool ValidateOnEnterKeyPressed {
			get { return (bool)GetValue(ValidateOnEnterKeyPressedProperty); }
			set { SetValue(ValidateOnEnterKeyPressedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditValidateOnTextInput"),
#endif
 Category("Behavior")]
		public bool ValidateOnTextInput {
			get { return (bool)GetValue(ValidateOnTextInputProperty); }
			set { SetValue(ValidateOnTextInputProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditCausesValidation"),
#endif
 Category("Behavior")]
		public bool CausesValidation {
			get { return (bool)GetValue(CausesValidationProperty); }
			set { SetValue(CausesValidationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditEditValue"),
#endif
 TypeConverter(typeof(ObjectConverter)), Category("Common Properties")]
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public event EditValueChangedEventHandler EditValueChanged {
			add { this.AddHandler(EditValueChangedEvent, value); }
			remove { this.RemoveHandler(EditValueChangedEvent, value); }
		}
		public event EditValueChangingEventHandler EditValueChanging {
			add { this.AddHandler(EditValueChangingEvent, value); }
			remove { this.RemoveHandler(EditValueChangingEvent, value); }
		}
		public event ValidateEventHandler Validate {
			add { this.AddHandler(ValidateEvent, value); }
			remove { this.RemoveHandler(ValidateEvent, value); }
		}
		public event RoutedEventHandler EditorActivated {
			add { this.AddHandler(EditorActivatedEvent, value); }
			remove { this.RemoveHandler(EditorActivatedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditEditMode"),
#endif
 Browsable(false)]
		public EditMode EditMode {
			get { return (EditMode)GetValue(EditModeProperty); }
			set { SetValue(EditModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditHasValidationError")]
#endif
		public bool HasValidationError {
			get { return (bool)GetValue(HasValidationErrorProperty); }
			internal set { this.SetValue(HasValidationErrorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditBorderTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditInvalidValueBehavior"),
#endif
 Category("Behavior")]
		public InvalidValueBehavior InvalidValueBehavior {
			get { return (InvalidValueBehavior)GetValue(InvalidValueBehaviorProperty); }
			set { SetValue(InvalidValueBehaviorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditShowBorder"),
#endif
 Category("Appearance")]
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditShowError"),
#endif
 Category("Behavior")]
		public bool ShowError {
			get { return (bool)GetValue(ShowErrorProperty); }
			set { SetValue(ShowErrorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditShowErrorToolTip"),
#endif
 Category("Behavior")]
		public bool ShowErrorToolTip {
			get { return (bool)GetValue(ShowErrorToolTipProperty); }
			set { SetValue(ShowErrorToolTipProperty, value); }
		}
		[Browsable(false)]
		public BaseEditStyleSettings StyleSettings {
			get { return (BaseEditStyleSettings)GetValue(StyleSettingsProperty); }
			set { SetValue(StyleSettingsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditIsPrintingMode")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingMode {
			get { return (bool)GetValue(IsPrintingModeProperty); }
			set { SetValue(IsPrintingModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditValidationError")]
#endif
		public BaseValidationError ValidationError {
			get { return GetValidationError(this); }
			internal set { SetValidationError(this, value); }
		}
		public int EditValuePostDelay {
			get { return (int)GetValue(EditValuePostDelayProperty); }
			set { SetValue(EditValuePostDelayProperty, value); }
		}
		public PostMode EditValuePostMode {
			get { return (PostMode)GetValue(EditValuePostModeProperty); }
			set { SetValue(EditValuePostModeProperty, value); }
		}
		public Type EditValueType {
			get { return (Type)GetValue(EditValueTypeProperty); }
			set { SetValue(EditValueTypeProperty, value); }
		}
		[Obsolete("Use BindingBase.Converter property instead")]
		public IValueConverter EditValueConverter {
			get { return (IValueConverter)GetValue(EditValueConverterProperty); }
			set { SetValue(EditValueConverterProperty, value); }
		}
		[Obsolete("Use BindingBase.Converter property instead")]
		public IValueConverter InputTextToEditValueConverter {
			get { return (IValueConverter)GetValue(InputTextToEditValueConverterProperty); }
			set { SetValue(InputTextToEditValueConverterProperty, value); }
		}
		public event CustomDisplayTextEventHandler CustomDisplayText {
			add { this.AddHandler(CustomDisplayTextEvent, value); }
			remove { this.RemoveHandler(CustomDisplayTextEvent, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditDisplayText")]
#endif
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			internal set { this.SetValue(DisplayTextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditDisplayTextConverter")]
#endif
		public IValueConverter DisplayTextConverter {
			get { return (IValueConverter)GetValue(DisplayTextConverterProperty); }
			set { SetValue(DisplayTextConverterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditDisplayFormatString"),
#endif
 Category("Behavior")]
		public string DisplayFormatString {
			get { return (string)GetValue(DisplayFormatStringProperty); }
			set { SetValue(DisplayFormatStringProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditActualBorderTemplate")]
#endif
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}
		public bool? DisableExcessiveUpdatesInInplaceInactiveMode {
			get { return (bool?)GetValue(DisableExcessiveUpdatesInInplaceInactiveModeProperty); }
			set { SetValue(DisableExcessiveUpdatesInInplaceInactiveModeProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldDisableExcessiveUpdatesInInplaceInactiveMode {
			get { return (bool)GetValue(ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty); }
			internal set { this.SetValue(ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditSetNullValueCommand")]
#endif
		public ICommand SetNullValueCommand { get; private set; }
		protected virtual bool IsEditingMode() {
			return EditMode == EditMode.Standalone || EditMode == EditMode.InplaceActive;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowUpdateTextBlockWhenPrinting {
			get { return (bool)GetValue(AllowUpdateTextBlockWhenPrintingProperty); }
			set { SetValue(AllowUpdateTextBlockWhenPrintingProperty, value); }
		}
		public DataTemplate ValidationErrorTemplate {
			get { return (DataTemplate)GetValue(ValidationErrorTemplateProperty); }
			set { SetValue(ValidationErrorTemplateProperty, value); }
		}
		internal ValidationErrorsExtractor Extractor { get; private set; }
		internal EditorFocusManagement FocusManagement { get; private set; }
		protected internal BorderRenderer BorderRenderer { get; private set; }
		ActualPropertyProvider CachedPropertyProvider { get; set; }
		EditSettingsChangedEventHandler<BaseEdit> EditSettingsChangedEventHandler { get; set; }
		PostponedAction SetupFocusAction { get; set; }
		internal IInplaceEditingProvider InplaceEditing { get; set; }
		internal ImmediateActionsManager ImmediateActionsManager { get; private set; }
		protected BaseEdit() {
			CanAcceptFocus = true;
#if DEBUGTEST
			InstanceCount++;
#endif
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			UpdateContentManagementStrategy();
			ActualPropertyProvider.SetProperties(this, CreateActualPropertyProvider());
			EditStrategy = CreateEditStrategy();
			Extractor = new ValidationErrorsExtractor(this);
			FocusManagement = new EditorFocusManagement(this);
			InplaceEditing = InplaceEditingProvider.Default;
			SetOwnerEdit(this, this);
			UpdateActualBorderTemplate();
			BorderRenderer = new BorderRenderer(this);
			PropertyProvider.SetEditValue(GetDefaultEditValue());
			SetNullValueCommand = DelegateCommandFactory.Create<object>(SetNullValueInternal, CanSetNullValueInternal, false);
			EditSettingsChangedEventHandler = new EditSettingsChangedEventHandler<BaseEdit>(this, (owner, o, e) => owner.OnEditSettingsChanged());
			SetupFocusAction = new PostponedAction(() => EditMode == Editors.EditMode.Standalone && !FrameworkElementHelper.GetIsLoaded(this));
#if !SL
			ToolTipClosing += EditorToolTipClosing;
			ToolTipService.SetInitialShowDelay(this, 500);
#endif
#if SL      
			ConstructorSLPart();
#endif
			ImmediateActionsManager = new ImmediateActionsManager(this);
			LayoutUpdated += OnLayoutUpdated;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			ImmediateActionsManager.ExecuteActions();
		}
		protected bool CanSetNullValueInternal(object arg) {
			return !IsReadOnly;
		}
		protected virtual object GetDefaultEditValue() {
			return EditValue;
		}
		void OnThemeChanged(object sender, EventArgs e) {
			EditStrategy.ThemeChanged();
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			OnLoadedInternal();
			ThemeManager.AddThemeChangedHandler(this, OnThemeChanged);
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			OnUnloadedInternal();
			ThemeManager.RemoveThemeChangedHandler(this, OnThemeChanged);
		}
		void OnUnloadedInternal() {
			EditStrategy.OnUnloaded();
		}
		protected virtual void OnLoadedInternal() {
			SetupFocusAction.Perform();
			RaiseOnLoading = true;
			EditStrategy.OnLoaded();
			RaiseOnLoading = false;
		}
		protected virtual EditStrategyBase CreateEditStrategy() {
			return new DummyEditStrategy(this);
		}
		internal void SetActualPropertyProvider(ActualPropertyProvider provider) {
			CachedPropertyProvider = provider;
		}
		protected virtual void OnAllowNullInputChanged() { }
		protected virtual void OnHasValidationErrorChanged() {
			if (MarginCorrector != null)
				MarginCorrector.HasValidationError = HasValidationError;
		}
		void StyleSettingsChangedInternal(BaseEditStyleSettings settings) {
			PropertyProvider.SetStyleSettings(settings);
			CheckStyleSettings(settings);
			StyleSettingsChanged(settings);
		}
		protected virtual void StyleSettingsChanged(BaseEditStyleSettings settings) {
			EditStrategy.StyleSettingsChanged(settings);
		}
		protected internal ActualPropertyProvider PropertyProvider { get { return CachedPropertyProvider; } }
		protected internal virtual Type StyleSettingsType { get { return typeof(EditStyleSettings); } }
		protected virtual void CheckStyleSettings(BaseEditStyleSettings settings) {
			if (settings == null)
				return;
			if (!StyleSettingsType.IsAssignableFrom(settings.GetType()))
				throw new ArgumentException(String.Format("The StyleSettings should be descendant of the {0} class for the {1}.", StyleSettingsType.Name, this.GetType().Name));
		}
		protected internal virtual BaseEditStyleSettings CreateStyleSettings() {
			return (BaseEditStyleSettings)Activator.CreateInstance(StyleSettingsType);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			EditStrategy.ProcessPreviewKeyDown(e);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateActualEditorControlTemplate();
			ContentManagementStrategy.OnEditorApplyTemplate();
			UpdateErrorPresenter();
#if SL
			SLOnApplyTemplate();
#endif
			UpdateIsEditorActivePropertyForce(EditMode);
		}
		protected virtual string GetStateName() {
			return "stub";
		}
		protected virtual void UpdateVisualState(bool useTransitions) {
			VisualStateManager.GoToState(this, GetStateName(), useTransitions);
		}
		protected virtual void OnEditCoreAssigned() {
			if (EditCore != null && IsFocused && EditMode != EditMode.InplaceInactive)
				SetupFocusAction.PerformPostpone(() => EditStrategy.FocusEditCore());
			IsEditorActive = IsEditingMode();
			EditStrategy.UpdateNullTextForeground(PropertyProvider.IsNullTextVisible);
			EditStrategy.UpdateAllowDrop(PropertyProvider.IsNullTextVisible);
		}
		internal T GetTemplateChildInternal<T>(string childName) where T : FrameworkElement {
			return GetTemplateChild(childName) as T;
		}
		protected virtual void UnsubscribeEditEventsCore() {
#if SL
			SLUnsubscribeEditEventsCore();
#endif
		}
		protected virtual void SubscribeEditEventsCore() {
#if SL
			SLSubscribeEditEventsCore();
#endif
		}
		protected virtual void SyncEditCoreProperties() {
			EditStrategy.SyncEditCoreProperties();
		}
		protected virtual void OnBorderTemplatePropertyChanged() {
			UpdateActualBorderTemplate();
		}
		protected void UpdateActualBorderTemplate() {
			ActualBorderTemplate = GetActualBorderTemplate();
		}
		protected virtual ControlTemplate GetActualBorderTemplate() {
			if (IsBorderVisible())
				return BorderTemplate;
			ControlTemplate result = null;
#if SL
			result = EmptyBorderTemplate;
			if (result == null) {
				FrameworkElement rootElement = GetTemplateChild("PART_Root") as FrameworkElement;
				if (rootElement != null)
					result = rootElement.Resources["EmptyBorderTemplate"] as ControlTemplate;
			}
#endif
			if (result == null)
				result = BorderTemplate;
			return result;
		}
		bool IsBorderVisible() {
			return ShowBorder && EditMode == EditMode.Standalone;
		}
		protected virtual void OnShowBorderChanged() {
			UpdateActualBorderTemplate();
			if (MarginCorrector != null)
				MarginCorrector.ShowBorder = ShowBorder;
		}
		protected virtual void OnShowErrorChanged() {
			UpdateErrorPresenter();
			if (MarginCorrector != null)
				MarginCorrector.ShowError = ShowError;
		}
		protected virtual void OnShowErrorToolTipChanged() {
		}
		protected virtual void OnNullTextChanged(string nullText) {
			Settings.NullText = nullText;
			EditStrategy.OnNullTextChanged(nullText);
		}
		protected virtual void OnNullValueChanged(object nullValue) {
			PropertyProvider.SetNullValue(nullValue);
			Settings.NullValue = nullValue;
			EditStrategy.OnNullValueChanged(nullValue);
		}
		protected virtual void OnValidationModeChanged() {
		}
		protected virtual object CoerceEditValue(DependencyObject d, object value) {
			return EditStrategy.CoerceEditValue(value);
		}
		protected void OnEditValueChanged(object oldValue, object newValue) {
			object newOldValue = PropertyProvider.EditValue;
			PropertyProvider.SetEditValue(newValue);
			object newNewValue = PropertyProvider.EditValue;
			isValueChanged = true;
			EditStrategy.IsValueChanged = true;
			EditStrategy.EditValueChanged(newOldValue, newNewValue);
		}
		protected virtual bool IsEditorKeyboardFocused {
			get { return IsKeyboardFocusWithin; }
		}
		protected virtual void OnEditModeChanged(EditMode oldValue, EditMode newValue) {
#if SL
			UpdateIsTabStop();
#endif
			PropertyProvider.SetEditMode(newValue);
			EditStrategy.ProcessEditModeChanged(oldValue, newValue);
			if (CanAcceptFocus && IsEditorKeyboardFocused)
				Focus();
			UpdateIsEditorActiveProperty(newValue);
			UpdateContentManagementStrategy();
			UpdateActualEditorControlTemplate();
			UpdateActualBorderTemplate();
			if (MarginCorrector != null)
				MarginCorrector.EditMode = EditMode;
#if SL
			InvalidateArrange();
#endif
		}
#if !SL
		void EditorToolTipClosing(object sender, ToolTipEventArgs e) {
			EditStrategy.ResetToolTipContent();
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			UpdateCommands(e.Property);
			if (e.Property == System.Windows.Controls.Validation.ErrorsProperty)
				Extractor.ValidationErrors = (INotifyCollectionChanged)e.NewValue;
		}
#endif
		protected virtual void UpdateCommands(DependencyProperty property) {
		}
		protected void UpdateCommand(ICommand command) {
			DelegateCommand<object> dCommand = command as DelegateCommand<object>;
			if (dCommand != null)
				dCommand.RaiseCanExecuteChanged();
		}
		void UpdateIsEditorActiveProperty(EditMode editMode) {
			if (Equals(GetEditTemplate(), GetDisplayTemplate()))
				IsEditorActive = editMode != EditMode.InplaceInactive;
		}
		void UpdateIsEditorActivePropertyForce(EditMode editMode) {
			IsEditorActive = editMode != EditMode.InplaceInactive;
		}
		protected virtual void OnDisplayFormatStringChanged() {
			PropertyProvider.SetDisplayFormatString(DisplayFormatString);
			Settings.DisplayFormat = DisplayFormatString;
			ForceChangeDisplayText();
		}
		protected internal virtual void ForceChangeDisplayText() {
			EditStrategy.UpdateDisplayText();
		}
		protected virtual object OnCoerceFormatString(object baseValue) {
			string baseString = (string)baseValue;
			if (baseValue == null || baseString.Trim() == String.Empty)
				return null;
			return baseString;
		}
		protected virtual void OnDisplayTextConverterChanged(IValueConverter converter) {
			PropertyProvider.SetDisplayTextConverter(converter);
			ForceChangeDisplayText();
		}
		protected virtual void OnDisplayTextChanged(string displayText) {
			EditStrategy.DisplayTextChanged(displayText);
		}
		protected virtual void OnIsNullTextVisibleChanged(bool isVisible) {
			PropertyProvider.SetIsNullTextVisible(isVisible);
			EditStrategy.OnIsNullTextVisibleChanged(isVisible);
		}
		protected virtual CustomDisplayTextEventArgs RaiseCustomDisplayText(object editValue, string displayText) {
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(CustomDisplayTextEvent);
			e.EditValue = editValue;
			e.DisplayText = displayText;
			this.RaiseEvent(e);
			return e;
		}
		protected internal virtual string GetCustomDisplayText(object editValue, string displayText) {
			if (displayTextProvider != null || PropertyProvider.SuppressFeatures)
				return null;
			CustomDisplayTextEventArgs e = RaiseCustomDisplayText(editValue, displayText);
			return e.Handled ? e.DisplayText : null;
		}
		protected internal virtual string GetDisplayText(object editValue, bool applyFormatting) {
			PropertyProvider.SetHasDisplayTextProviderText(false);
			string displayText = FormatDisplayText(editValue, applyFormatting);
			if (!applyFormatting)
				return displayText;
			string customDisplayText = GetCustomDisplayText(editValue, displayText);
			if (customDisplayText != null)
				return customDisplayText;
			customDisplayText = GetDisplayTextConverterText(editValue);
			displayText = String.IsNullOrEmpty(customDisplayText) ? displayText : customDisplayText;
			if (displayTextProvider != null) {
				string newDisplayText;
				bool? hasDisplayTextProviderText = displayTextProvider.GetDisplayText(displayText, editValue, out newDisplayText);
				PropertyProvider.SetHasDisplayTextProviderText(hasDisplayTextProviderText.GetValueOrDefault(false));
				if (hasDisplayTextProviderText.GetValueOrDefault(true))
					return newDisplayText;
			}
			return displayText;
		}
		protected virtual string FormatDisplayText(object editValue, bool applyFormatting) {
			return EditStrategy.FormatDisplayText(editValue, applyFormatting);
		}
		protected internal virtual string GetDisplayTextConverterText(object editValue) {
			if (PropertyProvider.DisplayTextConverter == null)
				return null;
			object converted = PropertyProvider.DisplayTextConverter.Convert(editValue, typeof(string), null, CultureInfo.CurrentCulture);
			if (converted is string)
				return converted as string;
			return converted != null ? converted.ToString() : string.Empty;
		}
		protected internal virtual bool HandlePreviewLostKeyboardFocus(DependencyObject oldFocus, DependencyObject newFocus) {
			if (newFocus == null && EditMode != EditMode.Standalone)
				return true;
			bool oldFocusInEditor = IsChildElement(oldFocus);
			bool newFocusInEditor = IsChildElement(newFocus);
			if (oldFocusInEditor && newFocusInEditor)
				return false;
			if (!newFocusInEditor) {
				EditStrategy.PrepareForCheckAllowLostKeyboardFocus();
				bool result = EditMode == EditMode.Standalone && CheckAllowLostKeyboardFocus();
				return InvalidValueBehavior != InvalidValueBehavior.AllowLeaveEditor && result;
			}
			return false;
		}
		protected virtual bool CheckAllowLostKeyboardFocus() {
			return !EditStrategy.DoValidate(UpdateEditorSource.LostFocus)
#if SL
				|| HasValidationError
#endif
;
		}
		protected virtual ControlTemplate GetEditTemplate() {
			return EditTemplate;
		}
		protected virtual ControlTemplate GetDisplayTemplate() {
			return DisplayTemplate;
		}
		void UpdateContentManagementStrategy() {
			if (EditMode != EditMode.Standalone)
				ContentManagementStrategy = (ContentManagementStrategyBase)new InplaceContentManagementStrategy(this);
			else
				ContentManagementStrategy = (ContentManagementStrategyBase)new StandaloneContentManagementStrategy(this);
		}
		internal void SetDisplayTextProvider(IDisplayTextProvider displayTextProvider) {
			this.displayTextProvider = displayTextProvider;
		}
		protected virtual void OnDisplayTemplateChanged(ControlTemplate newTemplate) {
			UpdateActualEditorControlTemplate();
		}
		protected virtual void OnNullTextTemplateChanged(ControlTemplate newTemplate) {
			UpdateActualEditorControlTemplate();
		}
		protected virtual void OnShowNullTextChanged(bool show) {
			PropertyProvider.SetShowNullText(show);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnShowNullTextForEmptyValueChanged(bool show) {
			PropertyProvider.SetShowNullTextForEmptyValue(show);
			EditStrategy.UpdateDisplayText();
		}
		protected virtual void OnEditTemplateChanged(ControlTemplate newTemplate) {
			UpdateActualEditorControlTemplate();
		}
		internal virtual void UpdateButtonPanelsInplaceMode() {
		}
		public virtual void SelectAll() { }
		protected virtual void OnIsReadOnlyChanged() { }
		protected virtual void OnIsEditorActiveChaged(bool value) {
			if (value)
				this.RaiseEvent(new RoutedEventArgs(EditorActivatedEvent));
			EditStrategy.IsEditorActiveChanged(value);
		}
		protected internal bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			GetIsActivatingKeyEventArgs e = new GetIsActivatingKeyEventArgs(key, modifiers, this, IsActivatingKeyCore(key, modifiers));
			Settings.RaiseGetIsActivatingKey(this, e);
			return e.IsActivatingKey;
		}
		bool IsActivatingKeyCore(Key key, ModifierKeys modifiers) {
			return !IsReadOnly && IsEnabled && Settings.IsActivatingKey(key, modifiers);
		}
		protected internal void ProcessActivatingKey(Key key, ModifierKeys modifiers) {
			ProcessActivatingKeyEventArgs e = new ProcessActivatingKeyEventArgs(key, modifiers, this);
			Settings.RaiseProcessActivatingKey(this, e);
			if (!e.IsProcessed)
				ProcessActivatingKeyCore(key, modifiers);
		}
		protected virtual void ProcessActivatingKeyCore(Key key, ModifierKeys modifiers) {
		}
		protected internal virtual bool NeedsKey(Key key, ModifierKeys modifiers) {
			bool? needsBasicKey = NeedsBasicKey(key, () => NeedsEnter(modifiers));
			if (needsBasicKey != null)
				return needsBasicKey.Value;
			if (key == Key.Tab)
				return NeedsTab();
			if (key == Key.Left || key == Key.Right)
				return NeedsLeftRight();
			if (key == Key.Up || key == Key.Down)
				return NeedsUpDown();
			return true;
		}
		protected virtual bool NeedsUpDown() {
			return true;
		}
		protected virtual bool NeedsLeftRight() {
			return true;
		}
		protected virtual bool NeedsTab() {
			return false;
		}
		protected virtual bool NeedsEnter(ModifierKeys modifiers) {
			return EditStrategy.NeedsEnterKey(modifiers);
		}
		protected virtual void UpdateErrorPresenter() {
			ContentManagementStrategy.UpdateErrorPresenter();
		}
		internal Size MeasureOverrideStandaloneMode(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		internal Size ArrangeOverrideStandaloneMode(Size arrangeSize) {
			return base.ArrangeOverride(arrangeSize);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return ContentManagementStrategy.ArrangeOverride(arrangeBounds);
		}
#if DEBUGTEST
		internal int MeasureCount { get; private set; }
#endif
		protected override Size MeasureOverride(Size constraint) {
#if DEBUGTEST
			MeasureCount++;
#endif
			return ContentManagementStrategy.MeasureOverride(constraint);
		}
#if !SL
		protected internal virtual bool BeginInplaceEditing() {
			return true;
		}
		internal void UpdateInplaceErrorPresenter() {
			if (HasValidationError) {
				if (ErrorPresenterInplace == null) {
					ErrorPresenterInplace = new ErrorControl();
					DataObjectBase.SetNeedsResetEvent(ErrorPresenterInplace, true);
					AddVisualChild(ErrorPresenterInplace);
					additionalInplaceModeElements.Insert(0, ErrorPresenterInplace);
					InvalidateMeasure();
				}
				ErrorPresenterInplace.Content = ValidationError;
			}
			else {
				if (ErrorPresenterInplace != null) {
					RemoveVisualChild(ErrorPresenterInplace);
					additionalInplaceModeElements.Remove(ErrorPresenterInplace);
					ErrorPresenterInplace = null;
					InvalidateMeasure();
				}
			}
			EditStrategy.ResetErrorProvider();
		}
		protected override Visual GetVisualChild(int index) {
			return ContentManagementStrategy.GetVisualChild(index);
		}
		internal Visual GetVisualChildStandaloneMode(int index) {
			return base.GetVisualChild(index);
		}
		protected override int VisualChildrenCount { get { return ContentManagementStrategy.VisualChildrenCount; } }
		internal int VisualChildrenCountStandaloneMode { get { return base.VisualChildrenCount; } }
		internal int VisualChildrenCountInplaceMode {
			get {
				return base.VisualChildrenCount + additionalInplaceModeElements.Count;
			}
		}
		internal Visual GetVisualChildInplaceMode(int index) {
			if (index < additionalInplaceModeElements.Count)
				return additionalInplaceModeElements[index];
			if (index == additionalInplaceModeElements.Count && base.VisualChildrenCount > 0)
				return base.GetVisualChild(0);
			throw new ArgumentOutOfRangeException("index");
		}
		internal Size MeasureOverrideInplaceMode(Size constraint) {
			return DockPanelLayoutHelper.MeasureDockPanelLayout(this, constraint);
		}
		internal Size ArrangeOverrideInplaceMode(Size arrangeSize) {
			return DockPanelLayoutHelper.ArrangeDockPanelLayout(this, arrangeSize, true);
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			BorderRenderer.Render(drawingContext);
		}
		protected virtual object OnCoerceToolTip(object tooltip) {
			return EditStrategy.CoerceValidationToolTip(tooltip);
		}
#endif
		bool showEditButtons = true;
		protected internal virtual bool GetShowEditorButtons() {
			return showEditButtons;
		}
		protected internal virtual void SetShowEditorButtons(bool show) {
			showEditButtons = show;
		}
		protected virtual void OnValidationErrorChanged(BaseValidationError error) {
			UpdateErrorPresenter();
			UpdateValidationService(error);
		}
		protected virtual object OnCoerceValidationError(BaseValidationError error) {
			return EditStrategy.CoerceBaseValidationError(error);
		}
		protected virtual void OnActualValidationErrorsChanged(IList errors) {
			if (errors != null)
				SetHasValidationError(this, errors.Count > 0);
			else
				SetHasValidationError(this, false);
			UpdateErrorPresenter();
			UpdateValidationService(errors.Count > 0 ? (BaseValidationError)errors[0] : null);
		}
		void OnValidationErrorTemplateChanged(DataTemplate newValue) {
			PropertyProvider.SetHasValidationErrorTemplate(newValue != null);
		}
		void UpdateValidationService(BaseValidationError error) {
			ValidationService service = ValidationService.GetValidationService(this);
			if (service == null)
				return;
			if (error != null)
				service.UpdateEditor(this);
			else
				service.RemoveEditor(this);
		}
		protected internal virtual bool IsChildElement(DependencyObject element, DependencyObject root = null) {
			return LayoutHelper.IsChildElementEx(root ?? this, element, true);
		}
		bool IBaseEdit.IsChildElement(IInputElement element, DependencyObject root) {
			return IsChildElement((DependencyObject)element, root);
		}
		bool IBaseEdit.ShowEditorButtons { get { return GetShowEditorButtons(); } set { SetShowEditorButtons(value); } }
#if !SL
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
#else
		protected override void OnGotFocus(System.Windows.RoutedEventArgs e) {
			base.OnGotFocus(e);
#endif
			if (!e.GetHandled() && e.OriginalSource == this)
				e.SetHandled(FocusEditCore());
		}
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			base.OnPreviewMouseWheel(e);
			EditStrategy.OnMouseWheel(e);
		}
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if (EditCore != null && FocusHelper.CanBeFocused(EditCore) && !IsKeyboardFocusWithin) {
				ProcessFocusEditCore(e);
			}
			else {
#if SL
				UpdateFocusProperties(); 
				if(EditMode == EditMode.InplaceInactive) { 
					if(e != null) e.Handled = true;
					return;
				}
#endif
				if (!IsFocused && !IsKeyboardFocusWithin)
					Focus();
			}
		}
		protected virtual void ProcessFocusEditCore(MouseButtonEventArgs e) {
			FocusEditCore();
		}
		internal bool CanAcceptFocus { get; set; }
		protected virtual bool FocusEditCore() {
			if (EditCore == null || IsInactiveMode)
				return false;
			if (!FocusHelper.IsFocused(EditCore) || !FocusHelper.IsKeyboardFocused(EditCore))
				return EditCore.Focus();
			return false;
		}
		public virtual bool DoValidate() {
			return EditStrategy.DoValidate(UpdateEditorSource.DoValidate);
		}
		public void ClearError() {
			EditStrategy.ResetValidationError();
			DoValidate();
		}
		protected internal virtual void UpdateDataContext(DependencyObject target) {
			EditStrategy.UpdateDataContext(target);
		}
		protected virtual void SubscribeToSettings(BaseEditSettings settings) {
			if (settings != null)
				settings.EditSettingsChanged += EditSettingsChangedEventHandler.Handler;
			PropertyProvider.CreatedFromSettings = true;
		}
		protected virtual void UnsubscribeFromSettings(BaseEditSettings settings) {
			if (settings != null)
				settings.EditSettingsChanged -= EditSettingsChangedEventHandler.Handler;
			PropertyProvider.CreatedFromSettings = true;
		}
		internal virtual void FlushPendingEditActions(UpdateEditorSource updateEditor) {
		}
		protected virtual void SetNullValueInternal(object parameter) {
			EditStrategy.SetNullValue(parameter);
		}
		protected virtual void EditValuePostDelayChanged(int value) {
			EditStrategy.EditValuePostDelayChanged(value);
		}
		protected virtual void EditValuePostModeChanged(PostMode value) {
			EditStrategy.EditValuePostModeChanged(value);
		}
		protected virtual void DisableExcessiveUpdatesInInplaceInactiveModeChanged(bool? newValue) {
			PropertyProvider.SetDisableExcessiveUpdatesInInplaceInactiveMode(newValue);
			if (!PropertyProvider.SuppressFeatures)
				EditStrategy.UpdateEditorOnEditingChange(true);
		}
		protected virtual void ShouldDisableExcessiveUpdatesInInplaceInactiveModeChanged(bool newValue) {
			PropertyProvider.SetShouldDisableExcessiveUpdatesInInplaceInactiveMode(newValue);
			if (!PropertyProvider.SuppressFeatures)
				EditStrategy.UpdateEditorOnEditingChange(true);
		}
		#region ISupportInitialize
		void IBaseEdit.BeginInit(bool callBase) {
			if (callBase && !IsInSupportInitializing) {
				LogBase.Add(this, null, "Control.BeginInit");
				base.BeginInit();
			}
			BeginInitInternal();
			supportInitializeLocker.Lock();
			EditStrategy.SupportInitializeBeginInit();
		}
		void IBaseEdit.EndInit(bool callBase, bool shouldSync) {
			supportInitializeLocker.Unlock();
			if (callBase && !IsInSupportInitializing) {
				LogBase.Add(this, null, "Control.EndInit");
				base.EndInit();
			}
			EndInitInternal(callBase);
			if (shouldSync)
				EditStrategy.SupportInitializeEndInit();
		}
		protected virtual void BeginInitInternal() {
		}
		protected virtual void EndInitInternal(bool callBase) {
			LogBase.Add(this, null, "EndInitInternal");
		}
		public override sealed void BeginInit() {
			((IBaseEdit)this).BeginInit(true);
		}
		public override sealed void EndInit() {
			((IBaseEdit)this).EndInit(true);
		}
		void IBaseEdit.ForceInitialize(bool callBase) {
			if (callBase)
				OnInitialized(EventArgs.Empty);
			else
				OnInitializedInternal();
		}
		#endregion
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			OnInitializedInternal();
		}
		void OnInitializedInternal() {
			if (IsInSupportInitializing)
				return;
			EditStrategy.OnInitialized();
		}
		protected virtual void EditValueTypeChanged(Type type) {
			EditStrategy.EditValueTypeChanged(type);
		}
		protected virtual void EditValueConverterChanged(IValueConverter converter) {
			throw new NotSupportedException("An exception occurred due to an obsolete property/method. For the actual component structure, please refer to the technical documentation.");
		}
		protected virtual void InputTextToEditValueConverterChanged(IValueConverter converter) {
			throw new NotSupportedException("An exception occurred due to an obsolete property/method. For the actual component structure, please refer to the technical documentation.");
		}
		#region IBaseEdit Members
		bool IBaseEdit.ShouldDisableExcessiveUpdatesInInplaceInactiveMode {
			get { return ShouldDisableExcessiveUpdatesInInplaceInactiveMode; }
			set { ShouldDisableExcessiveUpdatesInInplaceInactiveMode = value; }
		}
		BaseEditSettings IBaseEdit.Settings {
			get { return Settings; }
		}
		bool IBaseEdit.CanAcceptFocus {
			get { return CanAcceptFocus; }
			set { CanAcceptFocus = value; }
		}
		bool IBaseEdit.IsValueChanged {
			get { return IsValueChanged; }
			set { IsValueChanged = value; }
		}
		void IBaseEdit.SetSettings(BaseEditSettings settings) {
			SetSettings(settings);
			AfterSetSettings();
		}
		void IBaseEdit.SetInplaceEditingProvider(IInplaceEditingProvider provider) {
			this.InplaceEditing = provider;
		}
		bool IBaseEdit.NeedsKey(Key key, ModifierKeys modifiers) {
			return NeedsKey(key, modifiers);
		}
		bool IBaseEdit.IsActivatingKey(Key key, ModifierKeys modifiers) {
			return IsActivatingKey(key, modifiers);
		}
		void IBaseEdit.ProcessActivatingKey(Key key, ModifierKeys modifiers) {
			ProcessActivatingKey(key, modifiers);
		}
		bool IBaseEdit.GetShowEditorButtons() {
			return GetShowEditorButtons();
		}
		void IBaseEdit.SetShowEditorButtons(bool show) {
			SetShowEditorButtons(show);
		}
		void IBaseEdit.FlushPendingEditActions() {
			FlushPendingEditActions(UpdateEditorSource.DoValidate);
		}
		string IBaseEdit.GetDisplayText(object editValue, bool applyFormatting) {
			return GetDisplayText(editValue, applyFormatting);
		}
		BaseValidationError IBaseEdit.ValidationError {
			get { return ValidationError; }
			set { ValidationError = value; }
		}
		BindingExpressionBase IBaseEdit.SetBinding(DependencyProperty dp, BindingBase binding) {
#if !SL
			return SetBinding(dp, binding);
#else 
			return SetBinding(dp, (Binding)binding);
#endif
		}
		#endregion
		protected override IEnumerator LogicalChildren { get { return logicalChildren.GetEnumerator(); } }
		readonly List<object> logicalChildren = new List<object>();
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChildInternal(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChildInternal(child);
		}
#if SL
		IEnumerator ILogicalOwnerEx.LogicalChildren { get { return LogicalChildren; } }
#endif
		void AddLogicalChildInternal(object child) {
			AddLogicalChild(child);
			logicalChildren.Add(child);
			EditStrategy.AddILogicalOwnerChild(child);
		}
		void RemoveLogicalChildInternal(object child) {
			EditStrategy.RemoveILogicalOwnerChild(child);
			RemoveLogicalChild(child);
			logicalChildren.Remove(child);
		}
		#endregion
		#region IExportSettings Members
		Color IExportSettings.Background {
			get {
				if (Background != null)
					return (Color)Background.GetValue(SolidColorBrush.ColorProperty);
				return ExportSettingDefaultValue.Background;
			}
		}
		Color IExportSettings.Foreground {
			get {
				if (Foreground != null)
					return (Color)Foreground.GetValue(SolidColorBrush.ColorProperty);
				return ExportSettingDefaultValue.Foreground;
			}
		}
		Color IExportSettings.BorderColor {
			get {
				if (BorderBrush != null)
					return (Color)BorderBrush.GetValue(SolidColorBrush.ColorProperty);
				return ExportSettingDefaultValue.BorderColor;
			}
		}
		Thickness IExportSettings.BorderThickness {
			get { return BorderThickness; }
		}
		BorderDashStyle IExportSettings.BorderDashStyle {
			get { return ExportSettingDefaultValue.BorderDashStyle; }
		}
		string IExportSettings.Url {
			get { return ExportSettingDefaultValue.Url; }
		}
		IOnPageUpdater IExportSettings.OnPageUpdater {
			get {
				return null;
			}
		}
		object IExportSettings.MergeValue {
			get {
				return null;
			}
		}
		#endregion
		WeakReference marginCorrector;
		internal EditorMarginCorrector MarginCorrector {
			get {
				return (marginCorrector != null && marginCorrector.IsAlive) ? (EditorMarginCorrector)marginCorrector.Target : null;
			}
			set {
				marginCorrector = (value != null) ? new WeakReference(value) : null;
			}
		}
	}
	public interface IInplaceEditingProvider {
		bool HandleTextNavigation(Key key, ModifierKeys keys);
		bool HandleScrollNavigation(Key key, ModifierKeys keys);
	}
	public class InplaceEditingProvider : IInplaceEditingProvider {
		public static readonly InplaceEditingProvider Default = new InplaceEditingProvider();
		public bool HandleTextNavigation(Key key, ModifierKeys keys) {
			return false;
		}
		public bool HandleScrollNavigation(Key key, ModifierKeys keys) {
			return false;
		}
	}
}
namespace DevExpress.Xpf.Editors.Helpers {
	public static class ControlHelper {
		public static readonly DependencyProperty IsFocusedProperty;
		public static readonly DependencyProperty ShowFocusedStateProperty;
#if SL
		public static readonly DependencyProperty ClearDefaultStyleKeyProperty;
#endif
		static ControlHelper() {
			Type ownerType = typeof(ControlHelper);
			IsFocusedProperty = DependencyPropertyManager.RegisterAttached("IsFocused", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false));
			ShowFocusedStateProperty = DependencyPropertyManager.RegisterAttached("ShowFocusedState", typeof(bool), ownerType, new PropertyMetadata(PropertyChangedShowFocusedState));
#if SL
			ClearDefaultStyleKeyProperty = DependencyPropertyManager.RegisterAttached("ClearDefaultStyleKey", typeof(bool), ownerType, new PropertyMetadata(PropertyChangedClearDefaultStyleKey));
#endif
		}
		public static bool GetIsFocused(DependencyObject obj) {
			return (bool)obj.GetValue(IsFocusedProperty);
		}
		public static bool GetShowFocusedState(DependencyObject obj) {
			return (bool)obj.GetValue(ShowFocusedStateProperty);
		}
		public static void SetIsFocused(DependencyObject obj, bool value) {
			obj.SetValue(IsFocusedProperty, value);
		}
		public static void SetShowFocusedState(DependencyObject obj, bool value) {
			obj.SetValue(ShowFocusedStateProperty, value);
		}
		static void PropertyChangedShowFocusedState(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VisualStateManager.GoToState((System.Windows.Controls.Control)d, ((bool)e.NewValue) ? "InternalFocused" : "InternalUnfocused", true);
		}
#if SL
		public static bool GetClearDefaultStyleKey(DependencyObject obj) {
			return (bool)obj.GetValue(ClearDefaultStyleKeyProperty);
		}
		public static void SetClearDefaultStyleKey(DependencyObject obj, bool value) {
			obj.SetValue(ClearDefaultStyleKeyProperty, value);
		}
		static void PropertyChangedClearDefaultStyleKey(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue)
				d.ClearValue(DefaultStyleKeyExtensions.DefaultStyleKeyProperty);
		}
#endif
	}
}
