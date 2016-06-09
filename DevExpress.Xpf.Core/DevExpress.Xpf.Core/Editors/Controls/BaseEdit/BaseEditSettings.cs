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
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Input;
using System.Globalization;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
#if !SL
using System.Windows.Data;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using DXFrameworkContentElement = DevExpress.Xpf.Editors.Settings.BaseEditSettingsBaseClass;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public abstract partial class BaseEditSettings : DXFrameworkContentElement {
		internal event EventHandler EditSettingsChanged;
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (!IsBaseTypeProperty(e.Property.GetName())) {
#if !SL
				DependencyPropertyDescriptor decriptor = DependencyPropertyDescriptor.FromProperty(e.Property, GetType());
				if ((decriptor != null) && (!decriptor.IsAttached))
#endif
					RaiseChangedEventIfNotLoading();
			}
		}
		protected void RaiseChangedEventIfNotLoading() {
			if (AssignToEditCoreLocker.IsLocked)
				return;
			endInitPostponedAction.PerformIfNotLoading(RaiseChangedEvent);
		}
		void RaiseChangedEvent() {
			createEditorLocker.DoIfNotLocked(ResetEditor);
			if (EditSettingsChanged != null)
				EditSettingsChanged(this, EventArgs.Empty);
		}
		void ResetEditor() {
			editor = null;
		}
		public override void BeginInit() {
			base.BeginInit();
			beginEndInitLocker.Lock();
		}
		public override void EndInit() {
			beginEndInitLocker.Unlock();
			endInitPostponedAction.PerformActionOnEndInitIfNeeded(RaiseChangedEvent);
			base.EndInit();
		}
		static readonly DependencyPropertyKey ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty;
		public static readonly DependencyProperty DisableExcessiveUpdatesInInplaceInactiveModeProperty;
		public static readonly DependencyProperty AllowNullInputProperty;
		public static readonly DependencyProperty NullTextProperty;
		public static readonly DependencyProperty NullValueProperty;
		public static readonly DependencyProperty DisplayTextConverterProperty;
		public static readonly DependencyProperty DisplayFormatProperty;
		public static readonly DependencyProperty HorizontalContentAlignmentProperty;
		public static readonly DependencyProperty VerticalContentAlignmentProperty;
		public static readonly DependencyProperty StyleSettingsProperty;
		public static readonly DependencyProperty ValidationErrorTemplateProperty;
#if !SL
		public static readonly DependencyProperty FlowDirectionProperty;
		public static readonly DependencyProperty MaxWidthProperty;
#else
		public static new readonly DependencyProperty FlowDirectionProperty;
		public new static readonly DependencyProperty MaxWidthProperty;
#endif
		private static readonly object getIsActivatingKey = new object();
		private static readonly object processActivatingKey = new object();
		static BaseEditSettings() {
			Type ownerType = typeof(BaseEditSettings);
			ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ShouldDisableExcessiveUpdatesInInplaceInactiveMode", typeof(bool), ownerType, new PropertyMetadata(ActualPropertyProvider.ShouldDisableExcessiveUpdatesInInplaceInactiveModeDefaultValue, OnSettingsPropertyChanged));
			ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty = ShouldDisableExcessiveUpdatesInInplaceInactiveModePropertyKey.DependencyProperty;
			DisableExcessiveUpdatesInInplaceInactiveModeProperty = DependencyPropertyManager.Register("DisableExcessiveUpdatesInInplaceInactiveMode", typeof(bool?), ownerType, new PropertyMetadata(null, OnSettingsPropertyChanged));
			AllowNullInputProperty = DependencyPropertyManager.Register("AllowNullInput", typeof(bool), ownerType, new PropertyMetadata(true, OnSettingsPropertyChanged));
			NullTextProperty = DependencyPropertyManager.Register("NullText", typeof(string), ownerType, new PropertyMetadata("", new PropertyChangedCallback(OnSettingsPropertyChanged)));
			NullValueProperty = DependencyPropertyManager.Register("NullValue", typeof(object), ownerType, new PropertyMetadata(null, new PropertyChangedCallback(OnSettingsPropertyChanged)));
			DisplayTextConverterProperty = DependencyPropertyManager.Register("DisplayTextConverter", typeof(IValueConverter), ownerType, new FrameworkPropertyMetadata(null, OnSettingsPropertyChanged));
			DisplayFormatProperty = DependencyPropertyManager.Register("DisplayFormat", typeof(string), ownerType, new PropertyMetadata("", new PropertyChangedCallback(OnSettingsPropertyChanged), new CoerceValueCallback(OnDisplayFormatCoerce)));
			HorizontalContentAlignmentProperty = DependencyPropertyManager.Register("HorizontalContentAlignment", typeof(EditSettingsHorizontalAlignment), ownerType, new PropertyMetadata(EditSettingsHorizontalAlignment.Default, new PropertyChangedCallback(OnSettingsPropertyChanged)));
			VerticalContentAlignmentProperty = DependencyPropertyManager.Register("VerticalContentAlignment", typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnSettingsPropertyChanged)));
			MaxWidthProperty = DependencyPropertyManager.Register("MaxWidth", typeof(double), ownerType, new PropertyMetadata(double.PositiveInfinity));
			StyleSettingsProperty = BaseEdit.StyleSettingsProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSettingsPropertyChanged));
			FlowDirectionProperty = DependencyPropertyManager.Register("FlowDirection", typeof(FlowDirection), ownerType, new FrameworkPropertyMetadata(FlowDirection.LeftToRight));
			ValidationErrorTemplateProperty = DependencyPropertyManager.Register("ValidationErrorTemplate", typeof(DataTemplate), ownerType);
		}
		internal static bool IsBaseTypeProperty(string propertyName) {
			return typeof(BaseEditSettings).BaseType.GetProperty(propertyName) != null;
		}
		static object OnDisplayFormatCoerce(DependencyObject obj, object baseValue) {
			return ((BaseEditSettings)obj).OnDisplayFormatCoerce((string)baseValue);
		}
		protected static void OnSettingsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BaseEditSettings)obj).OnSettingsChanged(e);
		}
		readonly Locker beginEndInitLocker = new Locker();
		protected internal Locker AssignToEditCoreLocker { get; private set; }
		readonly EndInitPostponedAction endInitPostponedAction;
		IDefaultEditorViewInfo defaultViewInfo;
		IBaseEdit editor;
		bool? isInDesignTime;
		readonly Locker createEditorLocker = new Locker();
		bool IsInDesignTime {
			get {
				if (isInDesignTime == null)
					isInDesignTime = this.IsInDesignTool();
				return isInDesignTime.Value;
			}
		}
		protected IBaseEdit Editor {
			get {
				if (editor == null) {
					createEditorLocker.DoLockedAction(() => {
						editor = CreateEditor(defaultViewInfo ?? EmptyDefaultEditorViewInfo.Instance);
						editor.DisableExcessiveUpdatesInInplaceInactiveMode = true;
						editor.EditMode = EditMode.InplaceInactive;
						editor.ForceInitialize(true);
					});
				}
				return editor;
			}
		}
		protected internal virtual bool RequireDisplayTextSorting { get { return false; } }
		readonly Dictionary<object, Delegate> events;
		protected BaseEditSettings() {
			AssignToEditCoreLocker = new Locker();
			this.events = new Dictionary<object, Delegate>();
			endInitPostponedAction = new EndInitPostponedAction(() => beginEndInitLocker.IsLocked);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected virtual void OnLoaded() {
		}
		protected virtual void OnUnloaded() {
		}
		public IBaseEdit CreateEditor(EditorOptimizationMode optimizationMode = EditorOptimizationMode.Disabled) {
			return CreateEditor(EmptyDefaultEditorViewInfo.Instance, optimizationMode);
		}
		internal void ApplyToEdit(IBaseEdit edit, bool assignEditorSettings) {
			ApplyToEdit(edit, assignEditorSettings, EmptyDefaultEditorViewInfo.Instance);
		}
		public IBaseEdit CreateEditor(IDefaultEditorViewInfo defaultViewInfo, EditorOptimizationMode optimizationMode = EditorOptimizationMode.Disabled) {
			return CreateEditor(true, defaultViewInfo, optimizationMode);
		}
		public virtual IBaseEdit CreateEditor(bool assignEditorSettings, IDefaultEditorViewInfo defaultViewInfo, EditorOptimizationMode optimizationMode) {
			IBaseEdit res = EditorSettingsProvider.Default.CreateEditor(GetType(), optimizationMode);
			ApplyToEdit(res, assignEditorSettings, defaultViewInfo);
			res.ForceInitialize(false);
			return res;
		}
		public void ApplyToEdit(IBaseEdit edit, bool assignEditorSettings, IDefaultEditorViewInfo defaultViewInfo, bool force) {
			if (edit == null)
				return;
			AssignToEdit(edit, defaultViewInfo, force, assignEditorSettings);
		}
		public void ApplyToEdit(IBaseEdit edit, bool assignEditorSettings, IDefaultEditorViewInfo defaultViewInfo) {
			ApplyToEdit(edit, assignEditorSettings, defaultViewInfo, false);
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsAllowNullInput"),
#endif
 Category(EditSettingsCategories.Behavior), SkipPropertyAssertion]
		public bool AllowNullInput {
			get { return (bool)GetValue(AllowNullInputProperty); }
			set { SetValue(AllowNullInputProperty, value); }
		}
		protected internal Dictionary<object, Delegate> Events { get { return events; } }
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsDisplayFormat"),
#endif
 Category(EditSettingsCategories.Format), SkipPropertyAssertion]
		public string DisplayFormat {
			get { return (string)GetValue(DisplayFormatProperty); }
			set { SetValue(DisplayFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsDisplayTextConverter"),
#endif
 Category(EditSettingsCategories.Format)]
		public IValueConverter DisplayTextConverter {
			get { return (IValueConverter)GetValue(DisplayTextConverterProperty); }
			set { SetValue(DisplayTextConverterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsHorizontalContentAlignment"),
#endif
 Category(EditSettingsCategories.Layout), SkipPropertyAssertion]
		public EditSettingsHorizontalAlignment HorizontalContentAlignment {
			get { return (EditSettingsHorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
			set { SetValue(HorizontalContentAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsStyleSettings"),
#endif
 Category(EditSettingsCategories.Behavior), SkipPropertyAssertion]
		public BaseEditStyleSettings StyleSettings {
			get { return (BaseEditStyleSettings)GetValue(StyleSettingsProperty); }
			set { SetValue(StyleSettingsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsVerticalContentAlignment"),
#endif
 Category(EditSettingsCategories.Layout), SkipPropertyAssertion]
		public VerticalAlignment VerticalContentAlignment {
			get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
			set { SetValue(VerticalContentAlignmentProperty, value); }
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
		public DataTemplate ValidationErrorTemplate {
			get { return (DataTemplate)GetValue(ValidationErrorTemplateProperty); }
			set { SetValue(ValidationErrorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BaseEditSettingsMaxWidth"),
#endif
 Category(EditSettingsCategories.Layout), SkipPropertyAssertion]
#if !SL
		public double MaxWidth {
#else
		public new double MaxWidth {
#endif
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}
		public event GetIsActivatingKeyEventHandler GetIsActivatingKey {
			add { AddHandler(getIsActivatingKey, value); }
			remove { RemoveHandler(getIsActivatingKey, value); }
		}
		public event ProcessActivatingKeyEventHandler ProcessActivatingKey {
			add { AddHandler(processActivatingKey, value); }
			remove { RemoveHandler(processActivatingKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditSettingsNullText")]
#endif
		public string NullText {
			get { return (string)GetValue(NullTextProperty); }
			set { SetValue(NullTextProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BaseEditSettingsNullValue")]
#endif
		public object NullValue {
			get { return GetValue(NullValueProperty); }
			set { SetValue(NullValueProperty, value); }
		}
		public
#if SL
			new 
#endif
 FlowDirection FlowDirection {
			get { return (FlowDirection)GetValue(FlowDirectionProperty); }
			set { SetValue(FlowDirectionProperty, value); }
		}
		protected void AddHandler(object obj, Delegate handler) {
			if (Events.ContainsKey(obj)) {
				Events[obj] = MulticastDelegate.Combine(Events[obj], handler);
			}
			else
				Events[obj] = handler;
		}
		protected void RemoveHandler(object obj, Delegate handler) {
			if (Events.ContainsKey(obj))
				Events[obj] = MulticastDelegate.Remove(Events[obj], handler);
		}
		public virtual string GetDisplayTextFromEditor(object editValue) {
			return GetDisplayText(editValue, true);
		}
		public virtual string GetDisplayText(object editValue, bool applyFormatting) {
			return Editor.GetDisplayText(editValue, applyFormatting);
		}
		internal void AssignToEdit(IBaseEdit edit) {
			AssignToEdit(edit, defaultViewInfo ?? EmptyDefaultEditorViewInfo.Instance);
		}
		protected internal void AssignToEdit(IBaseEdit edit, IDefaultEditorViewInfo defaultViewInfo) {
			AssignToEdit(edit, defaultViewInfo, false);
		}
		readonly Locker forceAssignLocker = new Locker();
		protected internal void AssignToEdit(IBaseEdit edit, IDefaultEditorViewInfo defaultViewInfo, bool isForce, bool assignEditSettings = false) {
			if (edit == null)
				throw new ArgumentNullException("edit");
			edit.BeginInit(false);
			if (isForce)
				forceAssignLocker.Lock();
			try {
				AssignViewInfoProperties(edit, defaultViewInfo);
				AssignToEditCoreLocker.DoLockedAction(() => {
					if (assignEditSettings)
						edit.SetSettings(this);
					AssignToEditCore(edit);
				});
			}
			finally {
				if (isForce)
					forceAssignLocker.Unlock();
				edit.EndInit(false);
			}
		}
		protected virtual void OnSettingsChanged() {
		}
		protected virtual void AssignToEditCore(IBaseEdit edit) {
			edit.VerticalContentAlignment = VerticalContentAlignment;
#if !SL
			if (edit is IInplaceBaseEdit) {
				edit.NullText = NullText;
				edit.NullValue = NullValue;
				return;
			}
#endif
			BaseEdit baseEdit = edit as BaseEdit;
			if (baseEdit == null)
				return;
			SetValueFromSettings(AllowNullInputProperty, () => baseEdit.AllowNullInput = AllowNullInput);
			SetValueFromSettings(MaxWidthProperty, () => edit.MaxWidth = MaxWidth);
			SetValueFromSettings(NullTextProperty, () => edit.NullText = NullText);
			SetValueFromSettings(NullValueProperty, () => edit.NullValue = NullValue);
			SetValueFromSettings(DisplayFormatProperty, () => edit.DisplayFormatString = DisplayFormat);
			SetValueFromSettings(DisplayTextConverterProperty, () => edit.DisplayTextConverter = DisplayTextConverter);
			SetValueFromSettings(DisableExcessiveUpdatesInInplaceInactiveModeProperty, () => edit.DisableExcessiveUpdatesInInplaceInactiveMode = DisableExcessiveUpdatesInInplaceInactiveMode);
			SetValueFromSettings(ShouldDisableExcessiveUpdatesInInplaceInactiveModeProperty, () => edit.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = ShouldDisableExcessiveUpdatesInInplaceInactiveMode);
			SetValueFromSettings(StyleSettingsProperty, () => baseEdit.StyleSettings = StyleSettings);
			SetValueFromSettings(FlowDirectionProperty, () => baseEdit.FlowDirection = FlowDirection);
			SetValueFromSettings(ValidationErrorTemplateProperty,
				() => baseEdit.ValidationErrorTemplate = ValidationErrorTemplate,
				() => ClearEditorPropertyIfNeeded(baseEdit, ButtonEdit.ValidationErrorTemplateProperty, ValidationErrorTemplateProperty));
		}
		protected void SetValueFromSettings(DependencyProperty property, Action setAction, Func<bool> clearAction) {
			if (clearAction == null) {
				SetValueFromSettings(property, setAction);
				return;
			}
			if (!clearAction())
				SetValueFromSettings(property, setAction);
		}
		protected bool ClearEditorPropertyIfNeeded(BaseEdit edit, DependencyProperty editorProperty, DependencyProperty settingsProperty) {
			if (!this.IsPropertyAssigned(settingsProperty) && IsDefaultValue(settingsProperty)) {
				LogBase.Add(this, settingsProperty, "ClearEditorProperty - SettingsProperty");
				LogBase.Add(this, editorProperty, "ClearEditorProperty - EditorProperty");
				edit.ClearValue(editorProperty);
				return true;
			}
			return false;
		}
		protected void SetValueFromSettings(DependencyProperty property, Action setAction) {
			if (forceAssignLocker || (IsInDesignTime || !IsDefaultValue(property)) || this.IsPropertyAssigned(property)) {
				LogBase.Add(this, property, "SetValueFromSettings");
				setAction();
			}
		}
		bool IsDefaultValue(DependencyProperty property) {
			object defaultValue = property.GetMetadata(GetType()).DefaultValue;
			defaultValue = defaultValue == DependencyProperty.UnsetValue ? null : defaultValue;
			return GetValue(property) == defaultValue;
		}
		protected internal virtual void AssignViewInfoProperties(IBaseEdit edit, IDefaultEditorViewInfo defaultViewInfo) {
			this.defaultViewInfo = defaultViewInfo;
			edit.HorizontalContentAlignment = EditSettingsHorizontalAlignmentHelper.GetHorizontalAlignment(GetActualHorizontalContentAlignment(), defaultViewInfo.DefaultHorizontalAlignment);
		}
		protected virtual EditSettingsHorizontalAlignment GetActualHorizontalContentAlignment() {
			return HorizontalContentAlignment;
		}
		protected void OnSettingsChanged(DependencyPropertyChangedEventArgs e) {
			OnSettingsChanged();
		}
		protected internal virtual void RaiseGetIsActivatingKey(object sender, GetIsActivatingKeyEventArgs e) {
			Delegate handler;
			if (Events.TryGetValue(getIsActivatingKey, out handler))
				((GetIsActivatingKeyEventHandler)handler)(sender, e);
		}
		protected internal virtual void RaiseProcessActivatingKey(object sender, ProcessActivatingKeyEventArgs e) {
			Delegate handler;
			if (Events.TryGetValue(processActivatingKey, out handler))
				((ProcessActivatingKeyEventHandler)handler)(sender, e);
		}
		protected virtual string OnDisplayFormatCoerce(string baseValue) {
			return FormatStringConverter.GetDisplayFormat(baseValue);
		}
		protected internal virtual bool IsCompatibleWith(BaseEditSettings settings) {
			return object.Equals(this.GetType(), settings.GetType());
		}
		protected internal virtual bool IsActivatingKey(Key key, ModifierKeys modifiers) {
			return false;
		}
		protected internal virtual bool IsPasteGesture(Key key, ModifierKeys modifiers) {
#if !SL
			if (BrowserInteropHelper.IsBrowserHosted) 
				return false;
#endif
			return key == Key.V && modifiers == ModifierKeys.Control;
		}
		protected internal static bool IsNativeNullValue(object value) {
			return value == null || value == DBNull.Value;
		}
	}
}
