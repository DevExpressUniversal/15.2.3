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
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using DevExpress.DemoData.Utils;
using System.Windows.Input;
namespace DevExpress.DemoData.Helpers {
	public class BoolInverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? v = value as bool?;
			return !(v != null && (bool)v);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? v = value as bool?;
			return !(v != null && (bool)v);
		}
	}
	public class BoolToVisibilityConverter : IValueConverter {
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? v = value as bool?;
			return (v != null && (bool)v) ^ Inverted ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Visibility? v = value as Visibility?;
			return (v == Visibility.Visible) ^ Inverted;
		}
	}
	public class ObjectToBooleanConverter : IValueConverter {
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value != null ^ Inverted;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class BoolToAnyConverter : DependencyObjectExt, IValueConverter {
		#region Dependency Properties
		public static readonly DependencyProperty TrueValueProperty;
		public static readonly DependencyProperty FalseValueProperty;
		static BoolToAnyConverter() {
			Type ownerType = typeof(BoolToAnyConverter);
			TrueValueProperty = DependencyProperty.Register("TrueValue", typeof(object), ownerType, new PropertyMetadata(null));
			FalseValueProperty = DependencyProperty.Register("FalseValue", typeof(object), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public object TrueValue { get { return GetValue(TrueValueProperty); } set { SetValue(TrueValueProperty, value); } }
		public object FalseValue { get { return GetValue(FalseValueProperty); } set { SetValue(FalseValueProperty, value); } }
		public IValueConverter InnerConverter { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(InnerConverter != null)
				value = InnerConverter.Convert(value, typeof(bool), parameter, culture);
			bool? b = value as bool?;
			if(b == null) return null;
			return (bool)b ? TrueValue : FalseValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class AnyToBooleanConverter : DependencyObjectExt, IValueConverter {
		#region Dependency Properties
		public static readonly DependencyProperty TrueValueProperty;
		public static readonly DependencyProperty FalseValueProperty;
		static AnyToBooleanConverter() {
			Type ownerType = typeof(AnyToBooleanConverter);
			TrueValueProperty = DependencyProperty.Register("TrueValue", typeof(object), ownerType, new PropertyMetadata(null));
			FalseValueProperty = DependencyProperty.Register("FalseValue", typeof(object), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public object TrueValue { get { return GetValue(TrueValueProperty); } set { SetValue(TrueValueProperty, value); } }
		public object FalseValue { get { return GetValue(FalseValueProperty); } set { SetValue(FalseValueProperty, value); } }
		public bool Invert { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return object.Equals(TrueValue, value) ^ Invert;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? b = value as bool?;
			return (b != null && (bool)b) ^ Invert ? TrueValue : FalseValue;
		}
	}
	public class AnyToAnyConverter : DependencyObjectExt, IValueConverter {
		#region Dependency Properties
		public static readonly DependencyProperty Key0Property;
		public static readonly DependencyProperty Key1Property;
		public static readonly DependencyProperty Value0Property;
		public static readonly DependencyProperty Value1Property;
		public static readonly DependencyProperty DefaultValueProperty;
		static AnyToAnyConverter() {
			Type ownerType = typeof(AnyToAnyConverter);
			Key0Property = DependencyProperty.Register("Key0", typeof(object), ownerType, new PropertyMetadata(null));
			Key1Property = DependencyProperty.Register("Key1", typeof(object), ownerType, new PropertyMetadata(null));
			Value0Property = DependencyProperty.Register("Value0", typeof(object), ownerType, new PropertyMetadata(null));
			Value1Property = DependencyProperty.Register("Value1", typeof(object), ownerType, new PropertyMetadata(null));
			DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(object), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public object Key0 { get { return GetValue(Key0Property); } set { SetValue(Key0Property, value); } }
		public object Key1 { get { return GetValue(Key1Property); } set { SetValue(Key1Property, value); } }
		public object Value0 { get { return GetValue(Value0Property); } set { SetValue(Value0Property, value); } }
		public object Value1 { get { return GetValue(Value1Property); } set { SetValue(Value1Property, value); } }
		public object DefaultValue { get { return GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, value); } }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(object.Equals(Key0, value)) return Value0;
			if(object.Equals(Key1, value)) return Value1;
			return DefaultValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if(object.Equals(Value0, value)) return Key0;
			if(object.Equals(Value1, value)) return Key1;
			return DependencyProperty.UnsetValue;
		}
	}
	public class EmptyConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public class StringToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string s = value as string;
			return string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class TextToResourceUriConverter : IValueConverter {
		public object AssemblyMarker { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) return null;
			string s = value.ToString().Replace("#", "_sharp_").Replace(" ", "_");
			return AssemblyHelper.GetResourceUri(AssemblyMarker.GetType().Assembly, Prefix + s + Suffix);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class ActionToCommandConverter : IValueConverter {
		public class DelegateCommand : ICommand {
			readonly Action action;
			public DelegateCommand(Action action) {
				this.action = action;
			}
			public void Execute(object parameter) {
				if(action != null)
					action();
			}
			public bool CanExecute(object parameter) {
				return action != null;
			}
			public event EventHandler CanExecuteChanged {
				add { }
				remove { }
			}
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return new DelegateCommand(value as Action);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
