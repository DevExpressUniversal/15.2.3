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
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public class FormatStringConverter : MarkupExtension, IValueConverter {
		public static string GetDisplayFormat(string displayFormat) {
			if(string.IsNullOrEmpty(displayFormat))
				return string.Empty;
			string res = displayFormat;
			if(res.Contains("{"))
				return res;
			return string.Format("{{0:{0}}}", res);
		}
		public static object GetFormattedValue(string formatString, object value, System.Globalization.CultureInfo culture) {
			string displayFormat = GetDisplayFormat(formatString);
			return string.IsNullOrEmpty(displayFormat) ? value : string.Format(culture, displayFormat, value);
		}
		public FormatStringConverter() {
		}
		public FormatStringConverter(string formatString) {
			FormatString = formatString;
		}
		public IValueConverter Converter { get; set; }
		public string FormatString { get; set; }
		#region IValueConverter Members
		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(Converter != null)
				value = Converter.Convert(value, targetType, parameter, culture);
			return GetFormattedValue(FormatString ?? parameter as string, value, culture);
		}
		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class DefaultBooleanToNullableBooleanConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ((DefaultBoolean)value).ToBool();
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Nullable<bool> boolValue = (Nullable<bool>)value;
			if(!boolValue.HasValue)
				return DefaultBoolean.Default;
			return boolValue.Value ? DefaultBoolean.True : DefaultBoolean.False;
		}
		#endregion
	}
}
