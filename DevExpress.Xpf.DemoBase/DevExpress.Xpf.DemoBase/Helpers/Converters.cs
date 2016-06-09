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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.DemoData.Helpers;
using System.Windows.Markup;
#if DEMO
using DevExpress.Internal;
#else
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.DemoBase.Helpers {
	class StringFormatToUriConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null)
				return null;
			return new Uri(string.Format((string)parameter, value), UriKind.Absolute);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	class ColorToBrushConverter : IValueConverter {
		public bool DoNotUseSolidColorBrush { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Color? v = value as Color?;
			if(v == null) return null;
			Color color = (Color)v;
			if(!DoNotUseSolidColorBrush) return new SolidColorBrush(color);
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.GradientStops.Add(new GradientStop() { Color = color });
			return brush;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			SolidColorBrush b = value as SolidColorBrush;
			return b == null ? Colors.Transparent : b.Color;
		}
	}
	public class DateTimeShortPatternCoverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			DateTime? date = value as DateTime?;
			return date == null ? string.Empty : ((DateTime)date).ToShortDateString();
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class HorizontalAlignmentToVisibilityConverter : IValueConverter {
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			HorizontalAlignment? align = value as HorizontalAlignment?;
			return (align != null && (HorizontalAlignment)align == HorizontalAlignment.Stretch) ^ Inverted ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class ScaleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double? d = value as double?;
			if(d == null) return 0.0;
			return (double)d * (double)parameter;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class BoundaryDoubleToBooleanConverter : IValueConverter {
		public double Boundary { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double? d = value as double?;
			if(d == null) return 0.0;
			return (double)d > Boundary;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class BoundaryDoubleConverter : IValueConverter {
		public double Boundary { get; set; }
		public double LowValue { get; set; }
		public double Factor { get; set; }
		public double Base { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double? d = value as double?;
			if(d == null) return 0.0;
			return (double)d < Boundary ? LowValue : Base + Factor * (d - Boundary);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class SummConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double? d = value as double?;
			if(d == null) return 0.0;
			return (double)d + (double)parameter;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class BoundSummConverter : IValueConverter {
		public double BoundValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double? d = value as double?;
			if(d == null) return 0.0;
			double v = (double)d + (double)parameter;
			return v > BoundValue ? BoundValue : v;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class ListItemsCountToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null || !(value is IList)) return Visibility.Collapsed;
			return ((IList)value).Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class NegativeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Convert(value);
		}
		static object Convert(object value) {
			if(value == null || !(value is int)) return null;
			return -(int)value;
		}
	}
	class EqualsToParameterConverter : IValueConverter {
		public bool Inverted { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Inverted ? !value.Equals(parameter) : value.Equals(parameter);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (Inverted ? !(bool)value : (bool)value) ? parameter : GetDefaultValue();
		}
		protected virtual object GetDefaultValue() {
			return null;
		}
	}
	class ObjectToStringCallbackConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? null : (Func<string>)(() => value as string);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	class ThemeNameToListboxItemTitleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string typed = value as string;
			string suffix = "Touch";
			if(typed.EndsWith(suffix))
				return typed.Substring(0, typed.Length - suffix.Length).Trim();
			return typed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
namespace DevExpress.Xpf.DemoBase {
	class LerpConverter : MarkupExtension, IValueConverter {
		public double Offset { get; set; }
		public double Scale { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double val;
			if(value is bool)
				val = ((bool)value) ? 1.0f : 0.0f;
			else
				val = (double)value;
			return Offset + Scale * val;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
	}
	public class AndConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			foreach(var item in values) {
				if(!(item is bool) || !(bool)item)
					return false;
			}
			return true;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
