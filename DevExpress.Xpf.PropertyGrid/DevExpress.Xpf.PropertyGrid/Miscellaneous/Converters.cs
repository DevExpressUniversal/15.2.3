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
using System.Windows.Markup;
using System.Collections;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public class PopupMenuActualWidthToHorizontalOffsetConverter : MarkupExtension, IValueConverter {
		public PopupMenuActualWidthToHorizontalOffsetConverter() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double width = (double)value;
			double boundingWidth = (double)parameter;
			return boundingWidth - width;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Convert(value, targetType, parameter, culture);
		}
	}
	public class GridLengthToDoubleConverterExtension : MarkupExtension, IValueConverter {
		public GridLengthToDoubleConverterExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var length = (System.Windows.GridLength)value;
			if (!length.IsAbsolute) return double.NaN;
			return length.Value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double length = (double)value;
			if (double.IsNaN(length)) return new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
			return new System.Windows.GridLength(length, System.Windows.GridUnitType.Pixel);
		}
	}
	public class CheckEqualityConverter : MarkupExtension, IValueConverter {
		public CheckEqualityConverter() {
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			bool useTypeConverter = true;
			var converter = TypeDescriptor.GetConverter(targetType);  
			useTypeConverter &=converter!=null && parameter!=null;
			if(useTypeConverter)
				useTypeConverter &= converter.CanConvertFrom(parameter.GetType());
			return object.Equals(value, parameter) || !useTypeConverter ? false : object.Equals(value, converter.ConvertFrom(parameter));
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (value is bool && (bool)value) ? parameter : null;
		}
	}
	public class EnumToCollectionExtension : MarkupExtension {
		public Type EnumType { get; set; }
		public EnumToCollectionExtension() { }
		public EnumToCollectionExtension(Type enumType) {
			if (enumType != null && !enumType.IsSubclassOf(typeof(Enum)))
				throw new ArgumentException();
			EnumType = EnumType;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return EnumType == null ? null : Enum.GetValues(EnumType);
		}
	}
	public class RowStateCheckerConverterExtension : MarkupExtension, IValueConverter {
		public RowControlVisualStates ExistingStates { get; set; }
		public RowControlVisualStates MissingStates { get; set; }
		public RowStateCheckerConverterExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {			
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return (((RowControlVisualStates)value & ExistingStates) == ExistingStates) && (((RowControlVisualStates)value & MissingStates) == 0); }
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { return RowControlVisualStates.Normal; }
	}	
	public class DescriptionLocationConverterExtension : MarkupExtension, IValueConverter {
		public object TrueValue { get; set; }
		public object FalseValue { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public DescriptionLocationConverterExtension() {
			TrueValue = true;
			FalseValue = false;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var dvalue = (DescriptionLocation)value;
			var dparam = (DescriptionLocation)parameter;
			return ((dvalue & dparam) != 0) ? TrueValue : FalseValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	class TypeInfoToStringValueConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (value as TypeInfo).With(x => x.Name);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
