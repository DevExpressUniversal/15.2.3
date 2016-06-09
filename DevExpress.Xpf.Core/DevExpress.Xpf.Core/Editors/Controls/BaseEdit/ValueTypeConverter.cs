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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Editors.Internal {
	public class ValueTypeConverter : IValueConverter {
		string GetValidationError() {
			return EditorLocalizer.GetString(EditorStringId.InvalidValueConversion);
		}
		public Type TargetType { get; set; }
		public IValueConverter Converter { get; set; }
		public BaseValidationError ValidationError { get; set; }
		NullableContainer ConvertValueCache { get; set; }
		NullableContainer ConvertBackValueCache { get; set; }
		public ValueTypeConverter() {
			ConvertValueCache = new NullableContainer();
			ConvertBackValueCache = new NullableContainer();
		}
		Type ToConvertibleType(Type type) {
			if (type == null)
				return typeof(object);
			return typeof(IConvertible).IsAssignableFrom(type) ? type : typeof(object);
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			object result = value;
			if (value != null && TargetType != null)
				result = System.Convert.ChangeType(result, ToConvertibleType(TargetType), culture);
			if (Converter != null)
				result = Converter.Convert(result, targetType, parameter, culture);
			return result;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			object result = value;
			if (value != null && TargetType != null)
				result = System.Convert.ChangeType(result, ToConvertibleType(TargetType), culture);
			if (Converter != null)
				result = Converter.ConvertBack(result, targetType, parameter, culture);
			return result;
		}
		public object Convert(object editValue) {
			if (ConvertBackValueCache.HasValue && object.Equals(ConvertBackValueCache.Value, editValue) && ConvertValueCache.HasValue)
				return ConvertValueCache.Value;
			Reset();
			try {
				object result = Convert(editValue, editValue != null ? editValue.GetType() : null, null, CultureInfo.CurrentCulture);
				ConvertValueCache.SetValue(result);
				object convertResult = ConvertBack(result, result != null ? result.GetType() : null, null, CultureInfo.CurrentCulture);
				ConvertBackValueCache.SetValue(convertResult);
			}
			catch(Exception e) {
				ResetValues();
				ValidationError = new BaseValidationError(GetValidationError(), e, ErrorType.Critical);
			}
			return ConvertValueCache.Value;
		}
		public object ConvertBack(object editValue) {
			if (ConvertValueCache.HasValue && object.Equals(ConvertValueCache.Value, editValue) && ConvertBackValueCache.HasValue)
				return ConvertBackValueCache.Value;
			Reset();
			try {
				object result = ConvertBack(editValue, editValue != null ? editValue.GetType() : null, null, CultureInfo.CurrentCulture);
				ConvertBackValueCache.SetValue(result);
				object convertResult = Convert(result, result != null ? result.GetType() : null, null, CultureInfo.CurrentCulture);
				ConvertValueCache.SetValue(convertResult);
			}
			catch(Exception e) {
				ResetValues();
				ValidationError = new BaseValidationError(GetValidationError(), e, ErrorType.Critical);
			}
			return ConvertBackValueCache.Value;
		}
		void Reset() {
			ValidationError = null;
			ResetValues();
		}
		public void ResetValues() {
			ConvertValueCache.Reset();
			ConvertBackValueCache.Reset();
		}
	}
	public class EditableValueConverter : IValueConverter {
		public IValueConverter Converter { get; set; }
		public EditableValueConverter() {
		}
		public object Convert(object editValue) {
			return Convert(editValue, editValue != null ? editValue.GetType() : null, null, CultureInfo.CurrentCulture);
		}
		public object ConvertBack(object editValue) {
			return ConvertBack(editValue, editValue != null ? editValue.GetType() : null, null, CultureInfo.CurrentCulture);
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (Converter != null)
				return Converter.Convert(value, targetType, parameter, culture);
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (Converter != null)
				return Converter.ConvertBack(value, targetType, parameter, culture);
			return value;
		}
		#endregion
	}
}
