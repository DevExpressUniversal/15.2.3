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

using System.Linq.Expressions;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Utils {
#endif
	public static class DependencyObjectHelper {
		public static object GetValueWithInheritance(DependencyObject o, DependencyProperty p) {
			return o.GetValue(p);
		}
		public static object GetCoerceValue(DependencyObject o, DependencyProperty p) {
			return o.GetValue(p);
		}
	}
	public static class DependencyPropertyManager {
		public static DependencyProperty Register(string name, Type propertyType, Type ownerType) {
			return DependencyProperty.Register(name, propertyType, ownerType);
		}
		public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata) {
			return DependencyProperty.Register(name, propertyType, ownerType, typeMetadata);
		}
		public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata, ValidateValueCallback validateValueCallback) {
			return DependencyProperty.Register(name, propertyType, ownerType, typeMetadata, validateValueCallback);
		}
		public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType) {
			return DependencyProperty.RegisterAttached(name, propertyType, ownerType);
		}
		public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata) {
			return DependencyProperty.RegisterAttached(name, propertyType, ownerType, defaultMetadata);
		}
		public static DependencyProperty RegisterAttached(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback) {
			return DependencyProperty.RegisterAttached(name, propertyType, ownerType, defaultMetadata, validateValueCallback);
		}
		public static DependencyPropertyKey RegisterAttachedReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata defaultMetadata) {
			return DependencyProperty.RegisterAttachedReadOnly(name, propertyType, ownerType, defaultMetadata);
		}
		public static DependencyPropertyKey RegisterReadOnly(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata) {
			return DependencyProperty.RegisterReadOnly(name, propertyType, ownerType, typeMetadata);
		}
	}
	public static class FocusHelper {
		public static bool CanBeFocused(UIElement element) {
			return element.Focusable && element.IsEnabled;
		}
		public static IInputElement GetFocusedElement() {
			return Keyboard.FocusedElement;
		}
		public static bool IsFocused(UIElement element) {
			return element.IsFocused;
		}
		public static bool IsKeyboardFocused(UIElement element) {
			return element.IsKeyboardFocused;
		}
		public static bool IsKeyboardFocusWithin(UIElement element) {
			return element.IsKeyboardFocusWithin;
		}
		public static void SetFocusable(UIElement element, bool value) {
			element.Focusable = value;
		}
	}
	public static class PropertyMetadataHelper {
		public static PropertyMetadata Create(object defaultValue) {
			return new PropertyMetadata(defaultValue);
		}
		public static PropertyMetadata Create(PropertyChangedCallback propertyChangedCallback) {
			return new PropertyMetadata(propertyChangedCallback);
		}
		public static PropertyMetadata Create(object defaultValue, PropertyChangedCallback propertyChangedCallback) {
			return new PropertyMetadata(defaultValue, propertyChangedCallback);
		}
		public static PropertyMetadata Create(object defaultValue, PropertyChangedCallback propertyChangedCallback,
				CoerceValueCallback coerceValueCallback) {
			return new PropertyMetadata(defaultValue, propertyChangedCallback, coerceValueCallback);
		}
	}
	public static class RoutedEventArgsExtensions {
		public static bool GetHandled(this RoutedEventArgs e) {
			return e.Handled;
		}
		public static void SetHandled(this RoutedEventArgs e, bool value) {
			e.Handled = value;
		}
	}
	public static class UIElementHelper {
		public static void Collapse(UIElement element) {
			element.Visibility = Visibility.Collapsed;
		}
		public static void Hide(UIElement element) {
			element.Visibility = Visibility.Hidden;
		}
		public static bool IsEnabled(UIElement element) {
			return element.IsEnabled;
		}
		public static bool IsVisible(UIElement element) {
			return element.IsVisible;
		}
		public static bool IsVisibleInTree(UIElement element) {
			return IsVisibleInTree(element, false);
		}
		public static bool IsVisibleInTree(UIElement element, bool visualTreeOnly) {
			return element.IsVisible;
		}
		public static void Show(UIElement element) {
			element.Visibility = Visibility.Visible;
		}
	}
	public static class ControlExtensions {
		class DefaultStyleKeyHelper : FrameworkElement {
			public static DependencyProperty GetProperty() {
				return DefaultStyleKeyProperty;
			}
		}
		public static void SetDefaultStyleKey(this FrameworkElement control, Type value) {
			DependencyProperty dp = DefaultStyleKeyHelper.GetProperty();
			lock (dp) {
				if ((System.Type)dp.GetMetadata(value).DefaultValue != value) {
					dp.OverrideMetadata(value, new FrameworkPropertyMetadata(value));
				}
			}
		}
	}
}
#if !DXWINDOW
namespace DevExpress.Xpf.Core {
	public static class FocusHelper2 {
		public static readonly DependencyProperty FocusableProperty =
			DependencyProperty.RegisterAttached("Focusable", typeof(bool), typeof(FocusHelper2), new PropertyMetadata(true, (d, e) => PropertyChangedFocusable(d, e)));
		public static bool GetFocusable(DependencyObject o) {
			return (bool)o.GetValue(FocusableProperty);
		}
		public static void SetFocusable(DependencyObject o, bool value) {
			o.SetValue(FocusableProperty, value);
		}
		static void PropertyChangedFocusable(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PropertyInfo pi = d.GetType().GetProperty("Focusable", BindingFlags.Instance | BindingFlags.Public);
			if (pi != null)
				pi.SetValue(d, e.NewValue, null);
		}
	}
}
#endif
