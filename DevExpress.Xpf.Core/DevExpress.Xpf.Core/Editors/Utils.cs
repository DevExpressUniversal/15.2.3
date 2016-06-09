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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Helpers {
	public static class CollectionsGenerator {
		public static Type FindGenericType(Type sourceType) {
			IEnumerable<Type> typeHierarchy = GetTypeHierarchy(sourceType);
			foreach (Type type in typeHierarchy) {
				Type[] interfaces = type.GetInterfaces();
				Type genericCollectionInterfaceType = GetCollectionLikeGenericTypeFromInterfaces(interfaces);
				if (genericCollectionInterfaceType != null)
					return genericCollectionInterfaceType;
			}
			if (!sourceType.IsGenericType)
				return null;
			Type[] genericTypes = sourceType.GetGenericArguments();
			return genericTypes.Length == 1 ? genericTypes[0] : null;
		}
		static Type GetCollectionLikeGenericTypeFromInterfaces(IEnumerable<Type> interfaces) {
			foreach (Type interf in interfaces) {
				if (!interf.IsGenericType)
					continue;
				Type[] arguments = interf.GetGenericArguments();
				if (arguments.Length > 1)
					continue;
				if (typeof(IEnumerable<>).MakeGenericType(arguments) == interf)
					return arguments[0];
			}
			return null;
		}
		static IEnumerable<Type> GetTypeHierarchy(Type type) {
			IList<Type> hierarchy = new List<Type>();
			Type currentType = type;
			while (currentType.BaseType != null) {
				hierarchy.Add(currentType);
				currentType = currentType.BaseType;
			}
			return hierarchy.Reverse();
		}
		public static IList Generate(Type type) {
			if (type.IsAssignableFrom(typeof(IEnumerable)))
				return (IList)Activator.CreateInstance(type);
			Type genericType = FindGenericType(type);
			if (genericType != null)
				return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new[] { genericType }));
			return new List<object>();
		}
	}
	public static class Net45Detector {
		static bool propertyExists;
		static Net45Detector() {
			propertyExists = typeof(VirtualizingPanel).GetProperty("CanHierarchicallyScrollAndVirtualize") != null;
		}
		public static bool IsNet45() {
			return propertyExists;
		}
	}
	public static class HorizontalAlignmentToTextAlignmentExtensions {
		public static TextAlignment ToTextAlignment(this HorizontalAlignment ha) {
			switch (ha) {
				case HorizontalAlignment.Center:
					return TextAlignment.Center;
				case HorizontalAlignment.Right:
					return TextAlignment.Right;
#if !SL
				case HorizontalAlignment.Stretch:
					return TextAlignment.Justify;
#endif
				default:
					return TextAlignment.Left;
			}
		}
	}
	public class MVVMFocusBehavior : DependencyObject {
		public static readonly DependencyProperty IsFocusedProperty;
		static MVVMFocusBehavior() {
			Type ownerType = typeof(MVVMFocusBehavior);
			IsFocusedProperty = DependencyPropertyManager.RegisterAttached("IsFocused", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, IsFocusedPropertyChanged));
		}
		static void IsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement element = d as UIElement;
			if (element == null)
				return;
			if ((bool)e.NewValue)
				element.Focus();
		}
		public static bool GetIsFocused(DependencyObject d) {
			return (bool)d.GetValue(IsFocusedProperty);
		}
		public static void SetIsFocused(DependencyObject d, bool focused) {
			d.SetValue(IsFocusedProperty, focused);
		}
	}
	public interface ITargetChangedHelper<T> {
		event TargetChangedEventHandler<T> TargetChanged;
		void RaiseTargetChanged(T value);
	}
	public class TargetChangedEventArgs<T> : EventArgs {
		public T Value { get; private set; }
		public TargetChangedEventArgs(T value) {
			Value = value;
		}
	}
	public delegate void TargetChangedEventHandler<T>(object sender, TargetChangedEventArgs<T> e);
	public class EventToEventHelper<T> : ITargetChangedHelper<T> {
		public event TargetChangedEventHandler<T> TargetChanged;
		public void RaiseTargetChanged(T value) {
			if (TargetChanged != null)
				TargetChanged(this, new TargetChangedEventArgs<T>(value));
		}
	}
	public class RoutedEventToEventHelper<T> : ITargetChangedHelper<T> {
		public FrameworkElement Element { get; private set; }
		public System.Windows.RoutedEvent Event { get; private set; }
		Func<System.Windows.RoutedEventArgs, T> GetValueHandler { get; set; }
		public event TargetChangedEventHandler<T> TargetChanged;
		public RoutedEventToEventHelper(FrameworkElement element, System.Windows.RoutedEvent routedEvent, Func<System.Windows.RoutedEventArgs, T> getValueHandler) {
			Element = element;
			Event = routedEvent;
			GetValueHandler = getValueHandler;
			Subscribe();
		}
		void Subscribe() {
			Element.AddHandler(Event, new RoutedEventHandler((d, e) => RaiseTargetChanged((T)GetValueHandler(e))), false);
		}
		public void RaiseTargetChanged(T value) {
			if (TargetChanged != null)
				TargetChanged(this, new TargetChangedEventArgs<T>(value));
		}
	}
	public class BindingToEventHelper<T> : ITargetChangedHelper<T> {
		class PropertyProvider : DependencyObject {
			public static readonly DependencyProperty TargetProperty;
			static PropertyProvider() {
				Type ownerType = typeof(PropertyProvider);
				TargetProperty = DependencyPropertyManager.Register("Target", typeof(object), ownerType,
					new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (d, e) => ((PropertyProvider)d).TargetChanged(e.NewValue)));
			}
			void TargetChanged(object newValue) {
				T value = TargetChangedHelper.GetValueHandler(newValue);
				TargetChangedHelper.RaiseTargetChanged(value);
			}
			BindingToEventHelper<T> TargetChangedHelper { get; set; }
			public object Target {
				get { return GetValue(TargetProperty); }
				set { SetValue(TargetProperty, value); }
			}
			public PropertyProvider(BindingToEventHelper<T> helper) {
				TargetChangedHelper = helper;
				BindingOperations.SetBinding(this, TargetProperty, new Binding { Path = new PropertyPath(helper.Property), Mode = BindingMode.OneWay, Source = helper.Element });
			}
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty property;
		public FrameworkElement Element { get; private set; }
		public DependencyProperty Property { get { return property; } }
		public Func<object, T> GetValueHandler { get; set; }
		PropertyProvider Provider { get; set; }
		public BindingToEventHelper(FrameworkElement element, DependencyProperty property, Func<object, T> getValueHandler) {
			Element = element;
			this.property = property;
			GetValueHandler = getValueHandler;
			Provider = new PropertyProvider(this);
		}
		public void RaiseTargetChanged(T value) {
			if (TargetChanged != null)
				TargetChanged(this, new TargetChangedEventArgs<T>(value));
		}
		public event TargetChangedEventHandler<T> TargetChanged;
	}
#if !SL
	public class SkipPropertyAssertionAttribute : Attribute {
		public SkipPropertyAssertionAttribute() {
		}
	}
	public class BooleanToDockConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if ((bool)value)
				return Dock.Top;
			return Dock.Bottom;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if ((bool)value)
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class Counter {
		int innerCounter;
		public void Increment() {
			innerCounter++;
		}
		public int Value { get { return innerCounter; } }
		public void Reset() {
			innerCounter = 0;
		}
		public Counter() {
			Reset();
		}
		public bool IsClear {
			get { return innerCounter == 0; }
		}
	}
	public class DisplayFormatStringExtension : MarkupExtension {
		public string DisplayFormat { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return DisplayFormat;
		}
	}
#if !SL
	public class GermanKeyboardDetector {
		public static bool IsGermanKeyboard {
			get {
				switch (System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.LCID) {
					case 1031:
					case 3079:
					case 5127:
					case 4103:
					case 2055:
						return true;
					default:
						return false;
				}
			}
		}
	}
	public class FrenchKeyboardDetector {
		public static bool IsFrenchKeyboard {
			get {
				switch (System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.LCID) {
					case 1036:
					case 2060:
					case 5132:
					case 6156:
						return true;
					default:
						return false;
				}
			}
		}
	}
#endif
	public static class ModifierKeysHelper {
#if DEBUGTEST
		[ThreadStatic]
		public static ModifierKeys? ForcedModifiers;
#endif
		public static ModifierKeys GetKeyboardModifiers() {
#if DEBUGTEST
			return ForcedModifiers.HasValue ? ForcedModifiers.Value : Keyboard.Modifiers;
#else
			return Keyboard.Modifiers;
#endif
		}
		public static ModifierKeys GetKeyboardModifiers(KeyEventArgs e) {
#if !SL
			return e.KeyboardDevice.Modifiers;
#else
			return Keyboard.Modifiers;
#endif
		}
		public static bool IsAltPressed(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return (ForcedModifiers.Value & ModifierKeys.Alt) != ModifierKeys.None;
#endif
			return (modifiers & ModifierKeys.Alt) != ModifierKeys.None;
		}
		public static bool IsShiftPressed(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return (ForcedModifiers.Value & ModifierKeys.Shift) != ModifierKeys.None;
#endif
			return (modifiers & ModifierKeys.Shift) != ModifierKeys.None;
		}
		public static bool IsCtrlPressed(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return (ForcedModifiers.Value & ModifierKeys.Control) != ModifierKeys.None;
#endif
			return (modifiers & ModifierKeys.Control) != ModifierKeys.None;
		}
		public static bool IsOnlyCtrlPressed(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return ForcedModifiers.Value == ModifierKeys.Control;
#endif
			return modifiers == ModifierKeys.Control;
		}
		public static bool IsWinPressed(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return (ForcedModifiers.Value & ModifierKeys.Windows) != ModifierKeys.None;
#endif
			return (modifiers & ModifierKeys.Windows) != ModifierKeys.None;
		}
		public static bool NoModifiers(ModifierKeys modifiers) {
#if DEBUGTEST
			if (ForcedModifiers.HasValue)
				return ForcedModifiers.Value == ModifierKeys.None;
#endif
			return modifiers == ModifierKeys.None;
		}
		public static bool ContainsModifiers(ModifierKeys modifiers) {
			return IsAltPressed(modifiers) || IsShiftPressed(modifiers) || IsCtrlPressed(modifiers);
		}
	}
	public static class CapsLockHelper {
		public static bool IsCapsLockToggled { get { return Keyboard.PrimaryDevice.IsKeyToggled(Key.CapsLock); } }
	}
#endif
	public static class ThemeHelper {
		public static string GetWindowThemeName(DependencyObject obj) {
			var walker = ThemeManager.GetTreeWalker(obj);
			string baseName = GetTreewalkerThemeName(walker, false);
			if (baseName == null)
				return null;
			return baseName + walker.If(x => x.IsTouch).Return(x => ThemeManager.TouchDelimiterAndDefinition, () => string.Empty);
		}
		public static string GetEditorThemeName(DependencyObject obj) {
			return GetEditorThemeNameCore(obj, false);
		}
		public static bool IsBlackTheme(DependencyObject obj) {
			string baseThemeName = ThemeHelper.GetEditorThemeNameCore(obj, true);
			return baseThemeName == Theme.Office2010BlackName || baseThemeName == Theme.MetropolisDarkName || baseThemeName == Theme.TouchlineDarkName;
		}
		public static bool IsTouchTheme(DependencyObject obj) {
			return ThemeHelper.GetThemeName(obj) == Theme.TouchlineDarkName;
		}
		static string GetEditorThemeNameCore(DependencyObject obj, bool getBase) {
#if !SL
			FrameworkElement fe = obj as FrameworkElement;
			if (fe != null && fe.OverridesDefaultStyle)
				return null;
			FrameworkContentElement fce = obj as FrameworkContentElement;
			if (fce != null && fce.OverridesDefaultStyle)
				return null;
			ThemeTreeWalker theme = ThemeManager.GetTreeWalker(obj);
			return GetTreewalkerThemeName(theme, getBase);
#else
			string result = ThemeManager.ActualApplicationThemeName;
			if (!getBase)
				return result;
			else
			{
				string baseThemeName = Theme.GetBaseThemeName(result);
				return baseThemeName != null ? baseThemeName : result;
			}
#endif
		}
#if !SL
		public static string GetTreewalkerThemeName(ThemeTreeWalker theme, bool getBase) {
			string result = theme != null && !theme.IsDefault && theme.IsRegistered ? theme.ThemeName : null;
			if (!getBase)
				return result;
			string baseThemeName = Theme.GetBaseThemeName(result);
			return baseThemeName ?? result;
		}
#endif
		public static string GetThemeName(DependencyObject obj) {
			return GetRealThemeNameEx(obj);
		}
		public static string GetRealThemeNameEx(DependencyObject obj) {
#if !SL
			FrameworkElement fe = obj as FrameworkElement;
			if (fe != null && fe.OverridesDefaultStyle)
				return null;
			FrameworkContentElement fce = obj as FrameworkContentElement;
			if (fce != null && fce.OverridesDefaultStyle)
				return null;
#endif
			string themeName = GetRealThemeName(obj);
			return themeName == Theme.DeepBlueName ? null : themeName;
		}
		public static string GetRealThemeName(DependencyObject obj) {
			DependencyObject node = obj;
			DependencyObject newNode = null;
			while (node != null) {
#if SILVERLIGHT
				string themeName = ThemeManager.GetThemeName(node);
#else
				string themeName = ThemeManager.GetThemeName(node);
#endif
				if (!string.IsNullOrEmpty(themeName))
					return themeName;
				newNode = LayoutHelper.GetParent(node);
				if (newNode == null && node is FrameworkElement)
					newNode = ((FrameworkElement)node).Parent;
				node = newNode;
			}
			return null;
		}
		public static bool ShouldUpdateForeground(DependencyObject d) {
			return GetEditorThemeName(d) == Theme.Office2010BlackName;
		}
#if !SL
		public static InplaceResourceProvider GetResourceProvider(DependencyObject d) {
			var treewalker = ThemeManager.GetTreeWalker(d);
			return treewalker != null ? treewalker.InplaceResourceProvider : new InplaceResourceProvider(Theme.DefaultThemeName);
		}
#endif
	}
	public class HorizontalContentAlignmentToTextAlignmentConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			switch ((HorizontalAlignment)value) {
				case HorizontalAlignment.Center:
					return TextAlignment.Center;
				case HorizontalAlignment.Left:
					return TextAlignment.Left;
				case HorizontalAlignment.Right:
					return TextAlignment.Right;
				default:
					return TextAlignment.Justify;
			}
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class TextBlockInfo {
		public string Text { get; set; }
		public string HighlightedText { get; set; }
		public HighlightedTextCriteria HighlightedTextCriteria { get; set; }
		public override int GetHashCode() {
			return GetHashCode(Text) ^ GetHashCode(HighlightedText) ^ HighlightedTextCriteria.GetHashCode();
		}
		public override bool Equals(object obj) {
			TextBlockInfo textInfo = obj as TextBlockInfo;
			if (object.Equals(textInfo, null))
				return false;
			return Equals(Text, textInfo.Text) &&
				Equals(HighlightedText, textInfo.HighlightedText) &&
				Equals(HighlightedTextCriteria, textInfo.HighlightedTextCriteria);
		}
		int GetHashCode(string value) {
			return value == null ? 0 : value.GetHashCode();
		}
		public static bool operator ==(TextBlockInfo info1, TextBlockInfo info2) {
			return Equals(info1, info2);
		}
		public static bool operator !=(TextBlockInfo info1, TextBlockInfo info2) {
			return !Equals(info1, info2);
		}
	}
	public class TextHighlightingBehavior : Behavior<FrameworkElement> {
		public static readonly DependencyProperty TextBlockTextProperty;
		static TextHighlightingBehavior() {
			TextBlockTextProperty = DependencyProperty.Register("TextBlockText", typeof(string), typeof(TextHighlightingBehavior),
				new FrameworkPropertyMetadata(string.Empty, (d, e) => ((TextHighlightingBehavior)d).OnTextBlockTextChanged()));
		}
		public string TextBlockText {
			get { return (string)GetValue(TextBlockTextProperty); }
			set { SetValue(TextBlockTextProperty, value); }
		}
		TextBlock TextBlock { get { return AssociatedObject as TextBlock; } }
		RoutedEventHandler HighlightTextChangedHandler;
		protected override void OnAttached() {
			base.OnAttached();
			if (TextBlock != null) {
				if (TextBlock.IsInitialized)
					SetTextBinding();
				else
					TextBlock.Initialized += TextBlockInitialized;
				HighlightTextChangedHandler = new RoutedEventHandler(HighlightTextChanged);
				TextBlockService.AddHighlightTextChangedHandler(TextBlock, HighlightTextChangedHandler);
			}
		}
		void TextBlockInitialized(object sender, EventArgs e) {
			((FrameworkElement)sender).Initialized -= TextBlockInitialized;
			if (TextBlock == null) return;
			SetTextBinding();
		}
		void SetTextBinding() {
			var expression = TextBlock.GetBindingExpression(TextBlock.TextProperty);
			Binding bind = null;
			bind = expression != null ? expression.ParentBinding : new Binding("Text") { Source = TextBlock, Mode = BindingMode.OneWay };
			BindingOperations.SetBinding(this, TextBlockTextProperty, bind);
		}
		Locker updateTextLocker = new Locker();
		void HighlightTextChanged(object sender, RoutedEventArgs e) {
			if (!updateTextLocker.IsLocked) {
				updateTextLocker.DoLockedAction(() => {
					UpdateTextInfo();
				});
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if (TextBlock != null) {
				TextBlockService.RemoveHighlightTextChangedHandler(TextBlock, HighlightTextChangedHandler);
			}
		}
		void OnTextBlockTextChanged() {
			if (!updateTextLocker.IsLocked) {
				updateTextLocker.DoLockedAction(() => {
					UpdateTextInfo();
				});
			}
		}
		private void UpdateTextInfo() {
			if (TextBlock == null || (!string.IsNullOrEmpty(TextBlockText) && string.IsNullOrWhiteSpace(TextBlockText))) 
				return;
			var highlightText = (string)TextBlock.GetValue(TextBlockService.HighlightTextProperty);
			TextBlockInfo info = new TextBlockInfo() {
				Text = TextBlockText,
				HighlightedText = highlightText,
				HighlightedTextCriteria = HighlightedTextCriteria.StartsWith
			};
			TextBlock.SetValue(TextBlockService.TextInfoProperty, info);
		}
	}
	public class TextBlockService : DependencyObject {
		public const string SearchStringDelimiter = "\n";
		public static readonly DependencyProperty TextInfoProperty;
		public static readonly DependencyProperty AllowIsTextTrimmedProperty;
		protected static readonly DependencyPropertyKey IsTextTrimmedPropertyKey;
		public static readonly DependencyProperty IsTextTrimmedProperty;
		public static readonly DependencyProperty EnableTextHighlightingProperty;
		public static readonly DependencyProperty HighlightTextProperty;
		public static readonly RoutedEvent IsTextTrimmedChangedEvent;
		public static readonly RoutedEvent HighlightTextChangedEvent;
		internal const string DefaultTextValue = " ";
		static TextBlockService() {
			Type ownerType = typeof(TextBlockService);
			TextInfoProperty = DependencyPropertyManager.RegisterAttached("TextInfo", typeof(TextBlockInfo), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, TextInfoChanged));
			HighlightTextProperty = DependencyPropertyManager.RegisterAttached("HighlightText", typeof(string), ownerType,
	new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, HighlightTextChanged));
			EnableTextHighlightingProperty = DependencyPropertyManager.RegisterAttached("EnableTextHighlighting", typeof(bool), ownerType,
			   new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, EnableTextHighlightingChanged));
			AllowIsTextTrimmedProperty = DependencyPropertyManager.RegisterAttached("AllowIsTextTrimmed", typeof(bool), typeof(TextBlockService),
				new FrameworkPropertyMetadata(false));
			IsTextTrimmedPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBlockService),
				new FrameworkPropertyMetadata(false, OnIsTextTrimmedChanged));
			IsTextTrimmedProperty = IsTextTrimmedPropertyKey.DependencyProperty;
			EventManager.RegisterClassHandler(typeof(TextBlock), TextBlock.MouseEnterEvent, new MouseEventHandler(OnTextBlockMouseEnter), true);
			EventManager.RegisterClassHandler(typeof(TextBlock), TextBlock.MouseLeaveEvent, new MouseEventHandler(OnTextBlockMouseLeave), true);
			IsTextTrimmedChangedEvent = EventManager.RegisterRoutedEvent("IsTextTrimmedChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TextBlockService));
			HighlightTextChangedEvent = EventManager.RegisterRoutedEvent("HighlightTextChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TextBlockService));
		}
		private static void HighlightTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var element = d as TextBlock;
			if (element != null && (bool)element.GetValue(TextBlockService.EnableTextHighlightingProperty) == true)
				element.RaiseEvent(new RoutedEventArgs(HighlightTextChangedEvent));
		}
		static void EnableTextHighlightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var textBlock = d as TextBlock;
			if (textBlock == null)
				return;
			var behaviors = Interaction.GetBehaviors(textBlock);
			if ((bool)e.NewValue)
				behaviors.Add(new TextHighlightingBehavior());
			else {
				var behavior = behaviors.FirstOrDefault(x => x is TextHighlightingBehavior);
				if (behavior != null)
					behaviors.Remove(behavior);
			}
		}
		static void TextInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var textBlock = d as TextBlock;
			if (textBlock == null)
				return;
			TextBlockInfo info = e.NewValue as TextBlockInfo;
			if (info == null)
				ClearHighlighting(textBlock, null);
			else
				UpdateTextBlock(textBlock, info.Text, info.HighlightedText, info.HighlightedTextCriteria);
		}
		public static bool GetEnableTextHighlighting(DependencyObject d) {
			return (bool)d.GetValue(EnableTextHighlightingProperty);
		}
		public static void SetEnableTextHighlighting(DependencyObject d, bool value) {
			d.SetValue(EnableTextHighlightingProperty, value);
		}
		public static string GetHighlightText(DependencyObject d) {
			return (string)d.GetValue(HighlightTextProperty);
		}
		public static void SetHighlightText(DependencyObject d, string value) {
			d.SetValue(HighlightTextProperty, value);
		}
		public static TextBlockInfo GetTextInfo(DependencyObject d) {
			return (TextBlockInfo)d.GetValue(TextInfoProperty);
		}
		public static void SetTextInfo(DependencyObject d, TextBlockInfo value) {
			d.SetValue(TextInfoProperty, value);
		}
		public static void UpdateTextBlock(TextBlock textBlock, string text, string highlightedText, HighlightedTextCriteria criteria) {
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(highlightedText)) {
				ClearHighlighting(textBlock, text);
				return;
			}
			var searchStrings = highlightedText.Split(new[] { SearchStringDelimiter }, StringSplitOptions.None);
			UpdateInlines(textBlock, text, searchStrings, textOperand => GetMultipleInlines(textBlock, text, textOperand, criteria));
		}
		static void UpdateInlines(TextBlock textBlock, string text, IEnumerable<string> searchStrings, Func<IEnumerable<string>, IEnumerable<Inline>> getInlines) {
			IEnumerable<Inline> inlines = getInlines(searchStrings);
			if (inlines != null) {
				textBlock.Inlines.Clear();
				textBlock.Inlines.AddRange(inlines);
				return;
			}
			ClearHighlighting(textBlock, text);
		}
		static void ClearHighlighting(TextBlock textBlock, string text) {
			textBlock.Text = string.IsNullOrEmpty(text) ? DefaultTextValue : text;
		}
		static IEnumerable<Inline> GetMultipleInlines(TextBlock textBlock, string text, IEnumerable<string> searchString, HighlightedTextCriteria criteria) {
			IEnumerable<StringDescriptor> merged = new List<StringDescriptor>();
			foreach (string highlightedText in searchString) {
				IEnumerable<StringDescriptor> splitted = criteria == HighlightedTextCriteria.Contains ? SplitContains(text, highlightedText) : SplitStartWith(text, highlightedText);
				merged = Merge(merged, splitted);
			}
			int cursorPosition = 0;
			var sortedAndMerged = merged.OrderBy(descr => descr.SelectionStart);
			foreach (StringDescriptor descr in sortedAndMerged) {
				if (cursorPosition < descr.SelectionStart)
					yield return CreateRun(text.Substring(cursorPosition, descr.SelectionStart - cursorPosition));
				yield return CreateHighlightedInline(textBlock, text.Substring(descr.SelectionStart, descr.SelectionEnd - descr.SelectionStart));
				cursorPosition = descr.SelectionEnd;
			}
			string end = text.Substring(cursorPosition, text.Length - cursorPosition);
			if (!string.IsNullOrEmpty(end))
				yield return CreateRun(end);
		}
		static IEnumerable<StringDescriptor> Merge(IEnumerable<StringDescriptor> first, IEnumerable<StringDescriptor> second) {
			var temp = new Stack<StringDescriptor>();
			var result = new Stack<StringDescriptor>();
			foreach (StringDescriptor watched in first.Append(second)) {
				StringDescriptor currentWatched = watched;
				if (temp.Count == 0)
					temp.Push(currentWatched);
				while (temp.Count > 0) {
					StringDescriptor current = temp.Pop();
					if (current.Intersect(watched)) {
						currentWatched = current.Merge(currentWatched);
						while (result.Count > 0)
							temp.Push(result.Pop());
					}
					else
						result.Push(current);
				}
				result.Push(currentWatched);
			}
			return result;
		}
		public class StringDescriptor {
			public int SelectionStart { get; set; }
			public int SelectionEnd { get; set; }
			public bool Intersect(StringDescriptor stringDescriptor) {
				return Math.Max(SelectionStart, stringDescriptor.SelectionStart) <= Math.Min(SelectionEnd, stringDescriptor.SelectionEnd);
			}
			public StringDescriptor Merge(StringDescriptor stringDescriptor) {
				if (!Intersect(stringDescriptor))
					throw new ArgumentException("intersect");
				return new StringDescriptor { SelectionStart = Math.Min(SelectionStart, stringDescriptor.SelectionStart), SelectionEnd = Math.Max(SelectionEnd, stringDescriptor.SelectionEnd) };
			}
			bool Equals(StringDescriptor other) {
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;
				return other.SelectionStart == SelectionStart && other.SelectionEnd == SelectionEnd;
			}
			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != typeof(StringDescriptor))
					return false;
				return Equals((StringDescriptor)obj);
			}
			public override int GetHashCode() {
				return (SelectionStart * 397) ^ SelectionEnd;
			}
		}
		static Run CreateRun(string text) {
			if (string.IsNullOrEmpty(text))
				return null;
			return new Run { Text = text };
		}
		public static IEnumerable<StringDescriptor> SplitStartWith(string text, string highlightedText) {
			return SplitStartWith(text, highlightedText, 0);
		}
		public static IEnumerable<StringDescriptor> SplitStartWith(string text, string highlightedText, int startIndex) {
			List<StringDescriptor> result = new List<StringDescriptor>();
			if (string.IsNullOrEmpty(highlightedText))
				return result;
			string part = GetStartWithPart(text, highlightedText, startIndex);
			if (!string.IsNullOrEmpty(part))
				result.Add(new StringDescriptor() { SelectionStart = startIndex, SelectionEnd = startIndex + part.Length });
			return result;
		}
		public static string GetStartWithPart(string originalString, string searchString) {
			return GetStartWithPart(originalString, searchString, 0);
		}
		public static string GetStartWithPart(string originalString, string searchString, int startIndex) {
			string candidate = originalString.Substring(startIndex, Math.Min(searchString.Length, originalString.Length - startIndex));
			if (IsSame(searchString, candidate)) {
				return candidate;
			}
			for (int i = startIndex + 1; i <= originalString.Length; i++) {
				string str = originalString.Substring(startIndex, i - startIndex);
				if (StartWith(searchString, str)) {
					return str;
				}
			}
			return string.Empty;
		}
		public static bool IsSame(string editText, string candidate) {
			return string.Compare(candidate, editText, true) == 0;
		}
		public static bool StartWith(string searchString, string originalString) {
			return originalString.StartsWith(searchString, true, CultureInfo.CurrentCulture);
		}
		public static IEnumerable<StringDescriptor> SplitContains(string text, string highlightedText) {
			List<StringDescriptor> result = new List<StringDescriptor>();
			if (string.IsNullOrEmpty(highlightedText))
				return result;
			CompareInfo info = CultureInfo.CurrentUICulture.CompareInfo;
			int startIndex = info.IndexOf(text, highlightedText, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
			if (startIndex < 0)
				return result;
			while (startIndex > -1) {
				StringDescriptor endDescriptor = SplitStartWith(text, highlightedText, startIndex).LastOrDefault();
				if (endDescriptor != null) {
					result.Add(endDescriptor);
					startIndex = info.IndexOf(text, highlightedText, endDescriptor.SelectionEnd, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
				}
				else {
					break;
				}
			}
			return result;
		}
		static Inline CreateHighlightedInline(TextBlock textBlock, string highlightedText) {
			BaseEditStyleSettings settings = null;
			var edit = BaseEdit.GetOwnerEdit(textBlock);
			FrameworkElement editor = edit;
			if (edit != null)
				settings = edit.PropertyProvider.StyleSettings;
			if (settings == null) {
				var ibe = LayoutHelper.FindParentObject<InplaceBaseEdit>(textBlock) as InplaceBaseEdit;
				if (ibe != null) {
					editor = ibe;
					settings = ibe.Settings.StyleSettings as BaseEditStyleSettings ?? new TextEditStyleSettings();
				}
			}
			return CreateEditorHighlightedInline(editor, settings, highlightedText);
		}
		static Inline CreateDefaultHighlightedInline(string highlightedText) {
			var highlightedRun = new Run { Text = highlightedText };
			var bold = new Bold();
			bold.Inlines.Add(highlightedRun);
			return bold;
		}
		static Inline CreateEditorHighlightedInline(FrameworkElement editor, BaseEditStyleSettings settings, string highlightedText) {
			if (settings == null)
				return CreateDefaultHighlightedInline(highlightedText);
			return settings.CreateInlineForHighlighting(editor, highlightedText);
		}
		static void OnIsTextTrimmedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UIElement uiElement = d as UIElement;
			if (uiElement != null)
				uiElement.RaiseEvent(new RoutedEventArgs(IsTextTrimmedChangedEvent));
		}
		public static void AddIsTextTrimmedChangedHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null)
				uiElement.AddHandler(IsTextTrimmedChangedEvent, handler);
		}
		public static void RemoveIsTextTrimmedChangedHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null)
				uiElement.RemoveHandler(IsTextTrimmedChangedEvent, handler);
		}
		public static void AddHighlightTextChangedHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null)
				uiElement.AddHandler(HighlightTextChangedEvent, handler);
		}
		public static void RemoveHighlightTextChangedHandler(DependencyObject dObj, RoutedEventHandler handler) {
			UIElement uiElement = dObj as UIElement;
			if (uiElement != null)
				uiElement.RemoveHandler(HighlightTextChangedEvent, handler);
		}
		public static void SetAllowIsTextTrimmed(DependencyObject element, bool value) {
			element.SetValue(AllowIsTextTrimmedProperty, value);
		}
		public static bool GetAllowIsTextTrimmed(DependencyObject element) {
			return (bool)element.GetValue(AllowIsTextTrimmedProperty);
		}
		[AttachedPropertyBrowsableForType(typeof(TextBlock))]
		public static bool GetIsTextTrimmed(TextBlock target) {
			return (bool)target.GetValue(IsTextTrimmedProperty);
		}
		internal static void SetIsTextTrimmed(TextBlock target, bool value) {
			target.SetValue(IsTextTrimmedPropertyKey, value);
		}
		public static void SubscribeIsTextTrimmedChanged(TextBlock textBlock) {
			if (FrameworkElementHelper.GetIsLoaded(textBlock))
				return;
			FrameworkElementHelper.SetIsLoaded(textBlock, true);
			textBlock.MouseEnter += OnTextBlockMouseEnter;
			textBlock.MouseLeave += OnTextBlockMouseLeave;
		}
		static void OnTextBlockMouseEnter(object sender, MouseEventArgs e) {
			TextBlock textBlock = sender as TextBlock;
			UpdateIsTextTrimmed(textBlock, true);
		}
		static void OnTextBlockMouseLeave(object sender, MouseEventArgs e) {
			TextBlock textBlock = sender as TextBlock;
			UpdateIsTextTrimmed(textBlock, false);
		}
		public static void UpdateIsTextTrimmed(TextBlock textBlock, bool enabled) {
			if (textBlock == null || !GetAllowIsTextTrimmed(textBlock))
				return;
			if (enabled)
				SetIsTextTrimmed(textBlock, CalcIsTextTrimmed(textBlock));
			else
				textBlock.ClearValue(IsTextTrimmedPropertyKey);
		}
		public static string GetFirstLineFromText(string text) {
			if (string.IsNullOrEmpty(text))
				return text;
			return text.Split(Environment.NewLine[0]).FirstOrDefault();
		}
		public static bool CalcIsTextTrimmed(TextBlock textBlock) {
			if (!textBlock.IsArrangeValid)
				return false;
			bool? isTrimmed_NoTrimmingOWrapping = CalcIsTextTrimmed_NoTrimmingOWrapping(textBlock, textBlock.TextTrimming, textBlock.TextWrapping);
			if (isTrimmed_NoTrimmingOWrapping != null)
				return isTrimmed_NoTrimmingOWrapping.Value;
			Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
			FormattedText formattedText = new FormattedText(textBlock.Text, System.Globalization.CultureInfo.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize,
				textBlock.Foreground, new NumberSubstitution() { Substitution = NumberSubstitution.GetSubstitution(textBlock) }, TextOptions.GetTextFormattingMode(textBlock));
			return CalcIsTextTrimmed(formattedText, textBlock.ActualWidth, textBlock.ActualHeight, true);
		}
		public static bool CalcIsTextTrimmed(RenderTextBlockContext textBlockContext) {
			if (textBlockContext.Child != null) {
				var realContext = textBlockContext.Child;
				var textBlock = realContext.Control as TextBlock;
				if (textBlock != null)
					return CalcIsTextTrimmed(textBlock);
			}
			if (textBlockContext.FormattedTextContainer != null)
				return textBlockContext.FormattedTextContainer.HasCollapsedLines;
			return false;
		}
		static bool? CalcIsTextTrimmed_NoTrimmingOWrapping(FrameworkElement element, TextTrimming trimming, TextWrapping wrapping) {
			if ((trimming == TextTrimming.None) || (wrapping != TextWrapping.NoWrap)) {
				Geometry g = LayoutInformation.GetLayoutClip(element);
				if (g != null)
					return g.Bounds.Width > 0.0 || g.Bounds.Height > 0d;
			}
			return null;
		}
		static bool CalcIsTextTrimmed(FormattedText formattedText, double actualWidth, double actualHeight, bool useRounding) {
			double textWidth = formattedText.Width;
			double textHeight = formattedText.Height;
			if (useRounding)
				textWidth = Math.Floor(textWidth);
			textHeight = Math.Floor(textHeight);
			return textWidth > actualWidth || textHeight > actualHeight;
		}
	}
}
