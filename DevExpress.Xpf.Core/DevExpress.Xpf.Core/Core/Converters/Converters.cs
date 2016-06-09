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
using System.Windows.Markup;
using System.Windows;
using System.Collections;
using System.Windows.Media;
using System.Windows.Controls;
#if !SLDESIGN && !DXWINDOW
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Localization;
using DevExpress.Xpf.Utils;
#endif
#if SL && !SLDESIGN
using MarkupExtension = System.Object;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.ComponentModel;
#else
#endif
#if SLDESIGN
namespace DevExpress.Xpf.Core.Design.CoreUtils {
#else
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
#endif
	public class NegationConverterExtension : MarkupExtension, IValueConverter {
#if !SILVERLIGHT || SLDESIGN
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
#endif
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is bool))
				return DependencyProperty.UnsetValue;
			return !((bool)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is bool))
				return DependencyProperty.UnsetValue;
			return !((bool)value);
		}
	}
	public class BoolToVisibilityInverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (Visibility)value == Visibility.Collapsed;
		}
	}
	public class VisibilityToBoolInverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (Visibility)value == Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("VisibilityToBoolInverseConverter.ConvertBack");
		}
	}
	public class BoolToDoubleConverter : IValueConverter {
		public IValueConverter InnerConverter { get; set; }
		public double TrueValue { get; set; }
		public double? FalseValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(InnerConverter != null)
				value = InnerConverter.Convert(value, targetType, parameter, culture);
			return (bool)value ? TrueValue : (FalseValue ?? double.NaN);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BoolToObjectConverter :
#if !SL || SLDESIGN
 DependencyObject,
#endif
 IValueConverter {
#if !SL || SLDESIGN
		public static readonly DependencyProperty TrueValueProperty =
			DependencyProperty.Register("TrueValue", typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null, (d, e) => ((BoolToObjectConverter)d).UpdateCachedValues()));
		public static readonly DependencyProperty FalseValueProperty =
			DependencyProperty.Register("FalseValue", typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null, (d, e) => ((BoolToObjectConverter)d).UpdateCachedValues()));
#endif
		object trueValue;
		object falseValue;
		public object TrueValue {
			get { return trueValue; }
#if !SL || SLDESIGN
			set { SetValue(TrueValueProperty, value); }
#else
			set { trueValue = value; }
#endif
		}
		public object FalseValue {
			get { return falseValue; }
#if !SL || SLDESIGN
			set { SetValue(FalseValueProperty, value); }
#else
			set { falseValue = value; }
#endif
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			object obj = System.Convert.ToBoolean(value) ? TrueValue : FalseValue;
			SealHelper.SealIfSealable(obj);
			return ConverterHelper.Convert(obj, targetType);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
#if !SL || SLDESIGN
		void UpdateCachedValues() {
			trueValue = GetValue(TrueValueProperty);
			falseValue = GetValue(FalseValueProperty);
		}
#endif
	}
	public class BoolToStringConverter : IValueConverter {
		public BoolToStringConverter() {
		}
		public BoolToStringConverter(string trueString, string falseString) {
			this.TrueString = trueString;
			this.FalseString = falseString;
		}
		public string TrueString { get; set; }
		public string FalseString { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is bool)
				return (bool)value ? TrueString : FalseString;
			return value.ToString();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	internal static class ConverterHelper {
		static ThicknessConverter thicknessConverter = new ThicknessConverter();
		static CornerRadiusConverter cornerRadiusConverter = new CornerRadiusConverter();
		public static object Convert(object value, Type targetType) {
			if(!(value is string))
				return value;
			if(targetType == typeof(Thickness))
				return thicknessConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			if(targetType == typeof(CornerRadius))
				return cornerRadiusConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			return value;
		}
	}
	public class StringEmptyToSpaceConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			string converted = System.Convert.ToString(value);
			if (string.IsNullOrEmpty(converted))
				return " ";
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public static class SealHelper {
		public static void SealIfSealable(object obj) {
#if !SL
			Freezable freezable = obj as Freezable;
			if (freezable != null && freezable.CanFreeze) {
				freezable.Freeze();
				return;
			}
			FrameworkTemplate template = obj as FrameworkTemplate;
			if (template != null && !template.IsSealed) {
				template.Seal();
				return;
			}
			Style style = obj as Style;
			if (style != null && !style.IsSealed) {
				style.Seal();
				return;
			}
#endif
		}
	}
#if !SLDESIGN
	public class EnumToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.Equals(Enum.Parse(value.GetType(), (string)parameter, true));
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if ((bool)value)
				return Enum.Parse(targetType, (string)parameter, true);
			else
				return DependencyProperty.UnsetValue;
		}
	}
#endif
	public class EnumToObjectConverter : 
		IValueConverter {
		public object DefaultValue { get; set; }
		public IValueConverter AdditionalConverter { get; set; }
		public Dictionary<object, object> Values { get; set; }
		public EnumToObjectConverter() {
			Values = new Dictionary<object, object>();
		}
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			object result;
			if(Values.TryGetValue(System.Convert.ToString(value), out result)) {
				if(result is EnumObjectProvider) result = ((EnumObjectProvider)result).Value;
			} else {
				result = DefaultValue;
			}
			if(AdditionalConverter != null && result != null) result = AdditionalConverter.Convert(result, targetType, parameter, culture);
			return ConverterHelper.Convert(result, targetType);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class EnumObjectProvider
	{
		public object Value { get; set; }
	}
#if !SLDESIGN
	public class EnumToVisibilityConverter : IValueConverter {
		public bool Invert { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (value.Equals(Enum.Parse(value.GetType(), (string)parameter, true)) ^ Invert) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("EnumToVisibilityConverter.ConvertBack");
		}
	}
	public class ObjectToVisibilityConverter : IValueConverter {
		public bool Invert { get; set; }
		public bool HandleList { get; set; }
		public ObjectToVisibilityConverter() {
			HandleList = true;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!HandleList) {
				return (value != null) ^ Invert ? Visibility.Visible : Visibility.Collapsed;
			}
			IList list = value as IList;
			bool hasItems = list != null && list.Count > 0;
			return hasItems ^ Invert ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public class StringToVisibitityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null || String.IsNullOrEmpty(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ObjectToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value != null ^ Invert;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		public bool Invert { get; set; }
	}
	public class IntToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value is int)
				return Invert ? (int)value == 0 : (int)value != 0;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
		public bool Invert { get; set; }
	}
	public class BoolToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (Visibility)value == Visibility.Visible;
		}
	}
	public class StringToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return String.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("StringToVisibilityConverter.ConvertBack");
		}
	}
	public class BoolInverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return !(bool)value;
		}
	}
	public class VisibilityInverseConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (((Visibility)value) == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("VisibilityInverseConverter.ConvertBack");
		}
	}
	public class TypeToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value != null && (value.GetType().Name == (string)parameter || value.GetType().FullName == (string)parameter);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException("TypeToBoolConverter.ConvertBack");
		}
	}
	public class ObjectToStringConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? value : string.Format("{0:" + (string)parameter + "}", value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	public class ColorToBrushConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return new SolidColorBrush((Color)value);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((SolidColorBrush)value).Color;
		}
	}
#if !DXWINDOW
	public abstract class StringIdConverter<T> : IValueConverter where T : struct {
		protected abstract XtraLocalizer<T> Localizer { get; }
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Localizer.GetLocalizedString((T)Enum.Parse(typeof(T), (string)parameter, false));
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class EditorStringIdConverter : StringIdConverter<EditorStringId> {
		protected override XtraLocalizer<EditorStringId> Localizer { get { return EditorLocalizer.Active; } }
	}
	public class BarsStringIdConverter : StringIdConverter<BarsStringId> {
		protected override XtraLocalizer<BarsStringId> Localizer { get { return BarsLocalizer.Active; } }
	}
#endif
	public class MultiplyConverter : IValueConverter {
		public static readonly MultiplyConverter Instance = new MultiplyConverter();
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, System.Globalization.CultureInfo.InvariantCulture);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new Exception();
		}
	#endregion
	}
#if SILVERLIGHT
	public class BooleanToVisibilityConverter : BooleanToVisibilityConverterExtension { }
#endif
	public class BooleanToVisibilityConverterExtension : MarkupExtension, IValueConverter {
#if !SILVERLIGHT
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
#endif
		public bool Invert { get; set; }
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility stateOnFalse = Visibility.Collapsed;
#if !SILVERLIGHT
			if(parameter is string && (string)parameter == "HiddenOnFalse") stateOnFalse = Visibility.Hidden;
#endif
			if(value == null)
				return Invert ? Visibility.Visible : stateOnFalse;
			bool res = (bool)value;
			if(res)
				return Invert ? stateOnFalse : Visibility.Visible;
			return Invert ? Visibility.Visible : stateOnFalse;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility res = (Visibility)value;
			if(res == Visibility.Collapsed)
				return Invert;
			return !Invert;
		}
	#endregion
	}
	public class NullableToVisibilityConverter : MarkupExtension, IValueConverter {
#if !SILVERLIGHT
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
#endif
		public bool Invert { get; set; }
	#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null)
				return Invert? Visibility.Visible: Visibility.Collapsed;
			return Invert? Visibility.Collapsed: Visibility.Visible;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	#endregion
	}
	public class BooleanToDoubleConverter : MarkupExtension, IValueConverter {
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value.GetType() != typeof(bool) || targetType != typeof(double))
				throw new InvalidOperationException();
			return (bool)value == true ? 1.0 : 0.0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	#endregion
#if !SL
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
#endif
	}
#if !SILVERLIGHT
	public class SumConverter : MarkupExtension, IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double result = 0;
			foreach(object value in values) {
				if(value != DependencyProperty.UnsetValue)
					result += (double)value;
			}
			return result;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
#endif
#if !DXWINDOW
	public class ColorOverlayConverter : IValueConverter {
		static byte TransformValue(byte baseValue, byte value) {
			if(value > 128)
				return (byte)(baseValue + (255 - baseValue) * (value - 128) / 128);
			return (byte)(baseValue * value / 128);
		}
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Color c = Colors.Transparent;
			if(parameter is string) {
				c = ColorHelper.CreateColorFromString(parameter as string);
			}
			else if(parameter is Color) {
				c = (Color)parameter;
			}
			Color retValue = Colors.Transparent;
			Color v = Colors.Transparent;
			if(value is Color)
				v = (Color)value;
			else if(value is SolidColorBrush)
				v = ((SolidColorBrush)value).Color;
			retValue = ColorHelper.OverlayColor(c, v);
			if(targetType == typeof(Color))
				return retValue;
			if(targetType == typeof(SolidColorBrush))
				return new SolidColorBrush(retValue);
			if (targetType == typeof(Brush))
				return new SolidColorBrush(retValue);
			return retValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Colors.Gray;
		}
	#endregion
	}
#endif
	public class ControlBrushesToVisibilityConverter : IValueConverter {
		public bool Invert { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Control ctrl = value as Control;
			if(!Invert)
				return ctrl == null || (ctrl.Background == null && ctrl.BorderBrush == null) ? Visibility.Collapsed : Visibility.Visible;
			else
				return ctrl == null || (ctrl.Background == null && ctrl.BorderBrush == null) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public class BoolToThicknessConverter : IValueConverter {
		public Thickness ThicknessFalse { get; set; }
		public Thickness ThicknessTrue { get; set; }
	#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((bool)value) ? ThicknessTrue : ThicknessFalse;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	#endregion
	}
	public class EnumToWidthConverter : IValueConverter {
		public object EnumValue { get; set; }
		public double WidthTrue { get; set; }
		public double WidthFalse { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString() == EnumValue.ToString() ? WidthTrue : WidthFalse;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class DurationToDoubleConverter : IValueConverter {
	#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Duration d = (Duration)value;
			if(!d.HasTimeSpan) {
				return null;
			} else {
				return d.TimeSpan.Seconds * 1000 + d.TimeSpan.Milliseconds;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double milliseconds = (double)value;
			return new Duration(TimeSpan.FromMilliseconds(milliseconds));
		}
	#endregion
	}
	public class BoolToVisibilityViaOpacityConverter : MarkupExtension, IValueConverter {
		public bool Invert { get; set; }
	#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool result = System.Convert.ToBoolean(value);
			if(Invert)
				return result ? 0d : 1d;
			return result ? 1d : 0d;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	#endregion
#if !SL
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
#endif
	}
	public class StringToDoubleConverter : IValueConverter {
	#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return System.Convert.ToDouble(value, culture);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	#endregion
	}
#endif
	public class ListOfObjectToIEnumerableOfStringConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;
			IEnumerable valueEn = (IEnumerable)value;
			return valueEn.OfType<object>().ToList();
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null)
				return null;
			List<object> list = (List<object>)value;
			return list.Cast<string>();
		}
		#endregion
	}
	public class SmartTagCaptionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString() + " Tasks";
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ItemToEnumerableConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? null : new object[] { value };
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Array array = value as Array;
			return array == null ? null : array.Cast<object>().FirstOrDefault();
		}
	}
#if !SL
	public class SearchControlNullTextConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			if(!(values[1] is string) || !((values[0] is string) || (values[0] == null)))
				return null;
			return values[0] == null ? values[1] : values[0];
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#endif
}
