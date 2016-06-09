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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
namespace DevExpress.Xpf.Editors {
	public class ActualPropertyProvider : FrameworkElement, IServiceContainer {
		internal const bool ShouldDisableExcessiveUpdatesInInplaceInactiveModeDefaultValue = false;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty PropertiesProperty;
		readonly WeakReference weakEditor;
		public ActualPropertyProvider(BaseEdit editor) {
			this.weakEditor = new WeakReference(editor);
			ValueTypeConverter = new ValueTypeConverter();
			InputTextToEditValueConverter = new ValueTypeConverter();
			Services = new Dictionary<Type, object>();
			SetDisplayFormatString(editor.DisplayFormatString);
		}
		static ActualPropertyProvider() {
			Type ownerType = typeof(ActualPropertyProvider);
			PropertiesProperty = DependencyPropertyManager.RegisterAttached("Properties", typeof(ActualPropertyProvider), ownerType, new PropertyMetadata(null, PropertiesPropertyChanged));
		}
		static void PropertiesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is BaseEdit)
				((BaseEdit)d).SetActualPropertyProvider((ActualPropertyProvider)e.NewValue);
		}
		bool isNullTextVisible = true;
		object editValue;
		bool showNullText = true;
		bool showNullTextForEmptyValue = true;
		object nullValue;
		EditMode editMode = EditMode.Standalone;
		bool? disableExcessiveUpdatesInInplaceInactiveMode;
		bool shouldDisableExcessiveUpdatesInInplaceInactiveMode = ShouldDisableExcessiveUpdatesInInplaceInactiveModeDefaultValue;
		string displayFormatString;
		BaseEditStyleSettings defaultStyleSettings;
		BaseEditStyleSettings styleSettings;
		bool hasDisplayTextProviderText;
		bool hasValidationErrorTemplate;
#if !SL
		public CharacterCasing CharacterCasing { get; private set; }
#endif
		public bool CreatedFromSettings { get; protected internal set; }
		public string DisplayText { get; private set; }
		public IValueConverter DisplayTextConverter { get; private set; }
		public ValueTypeConverter InputTextToEditValueConverter { get; private set; }
		public ValueTypeConverter ValueTypeConverter { get; private set; }
		public bool IsNullTextVisible { get { return isNullTextVisible; } }
		public object EditValue { get { return editValue; } }
		public bool ShowNullText { get { return showNullText; } }
		public object NullValue { get { return nullValue; } }
		public bool ShowNullTextForEmptyValue { get { return showNullTextForEmptyValue; } }
		public bool? DisableExcessiveUpdatesInInplaceInactiveMode { get { return disableExcessiveUpdatesInInplaceInactiveMode; } }
		public bool ShouldDisableExcessiveUpdatesInInplaceInactiveMode { get { return shouldDisableExcessiveUpdatesInInplaceInactiveMode; } }
		public EditMode EditMode { get { return editMode; } }
		public bool SuppressFeatures { get { return EditMode == EditMode.InplaceInactive && CalcSuppressFeatures(); } }
		public string DisplayFormatString { get { return displayFormatString; } }
		public BaseEdit Editor { get { return (BaseEdit)weakEditor.Target; } }
		public BaseEditStyleSettings StyleSettings { get { return styleSettings ?? defaultStyleSettings ?? (defaultStyleSettings = Editor.CreateStyleSettings()); } }
		public bool HasDisplayTextProviderText { get { return hasDisplayTextProviderText; } }
		public bool HasValidationErrorTemplate { get { return hasValidationErrorTemplate; } }
		public BaseValidationError HiddenValidationError { get; private set; }
		public bool HasHiddenValidationError { get { return HiddenValidationError != null; } }
		public void SetStyleSettings(BaseEditStyleSettings settings) {
			this.styleSettings = settings;
		}
		public void SetValueTypeConverter(ValueTypeConverter valueConverter) {
			ValueTypeConverter = valueConverter;
			SetEditValue(EditValue);
		}
		public void SetDisplayTextConverter(IValueConverter converter) {
			DisplayTextConverter = converter;
		}
		public void SetInputTextToEditValueConverter(ValueTypeConverter converter) {
			InputTextToEditValueConverter = converter;
		}
		public void SetIsNullTextVisible(bool isNullTextVisible) {
			this.isNullTextVisible = isNullTextVisible;
		}
		public void SetShouldDisableExcessiveUpdatesInInplaceInactiveMode(bool shouldBeOptimized) {
			this.shouldDisableExcessiveUpdatesInInplaceInactiveMode = shouldBeOptimized;
		}
		public void SetDisableExcessiveUpdatesInInplaceInactiveMode(bool? optimizePerformance) {
			this.disableExcessiveUpdatesInInplaceInactiveMode = optimizePerformance;
		}
		public void SetEditMode(EditMode editMode) {
			this.editMode = editMode;
		}
		public void SetDisplayFormatString(string displayFormatString) {
			this.displayFormatString = displayFormatString;
		}
		public void SetNullValue(object nullValue) {
			this.nullValue = nullValue;
		}
		public void SetEditValue(object editValue) {
			this.editValue = ValueTypeConverter.Convert(editValue);
		}
		public void SetShowNullText(bool showNullText) {
			this.showNullText = showNullText;
		}
		public void SetShowNullTextForEmptyValue(bool showNullTextForEmptyValue) {
			this.showNullTextForEmptyValue = showNullTextForEmptyValue;
		}
		public void SetHiddenValidationError(BaseValidationError error) {
			HiddenValidationError = error;
		}
		public virtual bool CalcSuppressFeatures() {
			return (!DisableExcessiveUpdatesInInplaceInactiveMode.HasValue && ShouldDisableExcessiveUpdatesInInplaceInactiveMode) ||
				(DisableExcessiveUpdatesInInplaceInactiveMode.HasValue && DisableExcessiveUpdatesInInplaceInactiveMode.Value);
		}
#if !SL
		public void SetCharacterCasing(CharacterCasing characterCasing) {
			CharacterCasing = characterCasing;
		}
#endif
		public void SetDisplayText(string displayText) {
			DisplayText = displayText;
		}
		public virtual EditorPlacement GetNullValueButtonPlacement() {
			return EditorPlacement.None;
		}
		public virtual EditorPlacement GetAddNewButtonPlacement() {
			return EditorPlacement.None;
		}
		public virtual PopupFooterButtons GetPopupFooterButtons() {
			return PopupFooterButtons.None;
		}
		public virtual EditorPlacement GetFindButtonPlacement() {
			return EditorPlacement.None;
		}
		protected virtual ControlTemplate GetPopupTopAreaTemplate() {
			return null;
		}
		protected virtual ControlTemplate GetPopupBottomAreaTemplate() {
			return null;
		}
		public static ActualPropertyProvider GetProperties(DependencyObject obj) {
			return (ActualPropertyProvider)obj.GetValue(PropertiesProperty);
		}
		public static void SetProperties(DependencyObject obj, ActualPropertyProvider value) {
			obj.SetValue(PropertiesProperty, value);
		}
		internal static ActualPropertyProvider GetProperties<T>(IBaseEdit obj) {
			return GetProperties((DependencyObject)obj);
		}
		public void SetHasDisplayTextProviderText(bool hasDisplayTextProviderText) {
			this.hasDisplayTextProviderText = hasDisplayTextProviderText;
		}
		public void SetHasValidationErrorTemplate(bool hasValidationErrorTemplate) {
			this.hasValidationErrorTemplate = hasValidationErrorTemplate;
		}
		#region IServiceContainer
		Dictionary<Type, object> Services { get; set; }
		public T GetService<T>() where T : class {
			IServiceProvider This = this;
			return (T)This.GetService(typeof(T));
		}
		public void RegisterService<T>(T service) {
			IServiceContainer sc = this;
			sc.AddService(typeof(T), service);
		}
		ValueContainerService valueContainerService;
		TextInputServiceBase textInputService;
		ItemsProviderService itemsProviderService;
		object IServiceProvider.GetService(Type serviceType) {
			if (serviceType == typeof(ValueContainerService))
				return valueContainerService;
			if (serviceType == typeof(TextInputServiceBase))
				return textInputService;
			if (serviceType == typeof(ItemsProviderService))
				return itemsProviderService;
			return Services[serviceType];
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			if (serviceType == typeof(ValueContainerService))
				valueContainerService = (ValueContainerService)serviceInstance;
			if (serviceType == typeof(TextInputServiceBase))
				textInputService = (TextInputServiceBase)serviceInstance;
			if (serviceType == typeof(ItemsProviderService))
				itemsProviderService = (ItemsProviderService)serviceInstance;
			Services[serviceType] = serviceInstance;
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			IServiceContainer sc = this;
			sc.AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			IServiceContainer sc = this;
			sc.AddService(serviceType, callback(sc, serviceType));
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			IServiceContainer sc = this;
			sc.AddService(serviceType, callback);
		}
		void IServiceContainer.RemoveService(Type serviceType) {
			Services.Remove(serviceType);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			Services.Remove(serviceType);
		}
		#endregion
	}
}
