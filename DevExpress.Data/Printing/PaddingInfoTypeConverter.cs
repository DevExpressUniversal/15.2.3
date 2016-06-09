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
using System.ComponentModel;
using System.Collections;
#if SL
using DevExpress.Xpf.ComponentModel;
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using DevExpress.Xpf.ComponentModel.Design.Serialization;
#else
using System.ComponentModel.Design.Serialization;
#endif
namespace DevExpress.XtraPrinting.Design {
	public class PaddingInfoTypeConverter : ExpandableObjectConverter {
		static readonly string[] SortOrderPaddingInfoTypeDescriptor = new string[] { "All", "Left", "Right", "Top", "Bottom" };
#if SL
		static int PaddingInfoTypeDescriptorCompare(string propertyDescriptorName) {
			return Array.IndexOf(SortOrderPaddingInfoTypeDescriptor, propertyDescriptorName);
		}
#endif
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection result = TypeDescriptor.GetProperties(typeof(PaddingInfo), attributes);
#if SL
			result.Sort((left, right) => Array.IndexOf(SortOrderPaddingInfoTypeDescriptor, left.Name).CompareTo(Array.IndexOf(SortOrderPaddingInfoTypeDescriptor, right.Name)));
#else
			result = result.Sort(SortOrderPaddingInfoTypeDescriptor);
#endif
			return result;
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if ((destinationType == typeof(string)) && (value is PaddingInfo)) {
				PaddingInfo padding = (PaddingInfo)value;
				return string.Format("{1}{0} {2}{0} {3}{0} {4}", GetListSeparator(culture), padding.Left, padding.Right, padding.Top, padding.Bottom);
			}
			if (destinationType == typeof(InstanceDescriptor)) {
				PaddingInfo padding = (PaddingInfo)value;
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(float) });
				return new InstanceDescriptor(ci, new object[] { padding.Left, padding.Right, padding.Top, padding.Bottom, padding.Dpi });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		string GetListSeparator(System.Globalization.CultureInfo culture) {
			return ValidateCulture(culture).TextInfo.ListSeparator;
		}
		System.Globalization.CultureInfo ValidateCulture(System.Globalization.CultureInfo culture) {
			return culture != null ? culture : System.Globalization.CultureInfo.CurrentCulture;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if (!(value is string)) {
				return base.ConvertFrom(context, culture, value);
			}
			string paddingString = ((string)value).Trim();
			culture = ValidateCulture(culture);
			string listSeparator = culture.TextInfo.ListSeparator;
			string[] paddingStringArray = paddingString.Split(listSeparator[0]);
			int[] paddingNumArray = new int[paddingStringArray.Length];
			if (paddingNumArray.Length != 4)
				throw new ArgumentException("TextParseFailedFormat");
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for (int i = 0; i < paddingNumArray.Length; i++)
				paddingNumArray[i] = (int)converter.ConvertFromString(
#if !SL
					context, culture,
#endif
					paddingStringArray[i]);
			return CreatePaddingInfo(paddingNumArray[0], paddingNumArray[1], paddingNumArray[2], paddingNumArray[3], GetDpi(context.Instance));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
#if !SL
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			PaddingInfo padding = (PaddingInfo)context.PropertyDescriptor.GetValue(context.Instance);
			int all = (int)propertyValues["All"];
			if (padding.All != all) {
				return new PaddingInfo(all, GetDpi(context.Instance));
			}
			object left = propertyValues["Left"];
			object right = propertyValues["Right"];
			object top = propertyValues["Top"];
			object bottom = propertyValues["Bottom"];
			return CreatePaddingInfo(ToInt32(left), ToInt32(right), ToInt32(top), ToInt32(bottom), GetDpi(context.Instance));
		}
#endif
		int ToInt32(object value) {
			return value != null ? Convert.ToInt32(value) : 0;
		}
		float GetDpi(object obj) {
			return obj != null ? obj is Array ? GetDpiFromArray((Array)obj) : GetDpiFromObject(obj) : -1;
		}
		float GetDpiFromArray(Array array) {
			System.Diagnostics.Debug.Assert(array != null);
			foreach (object obj in array) {
				float dpi = GetDpiFromObject(obj);
				if (dpi > 0)
					return dpi;
			}
			return -1;
		}
		float GetDpiFromObject(object obj) {
			System.Diagnostics.Debug.Assert(obj != null);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			foreach (PropertyDescriptor property in properties)
				if (property.PropertyType == typeof(PaddingInfo)) {
					PaddingInfo padding = (PaddingInfo)property.GetValue(obj);
					return padding.Dpi;
				}
			return -1;
		}
		PaddingInfo CreatePaddingInfo(int left, int right, int top, int bottom, float dpi) {
			return new PaddingInfo(left, right, top, bottom, dpi);
		}
#if !SL
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
#endif
	}
}
