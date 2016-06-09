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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Collections;
using DevExpress.XtraLayout;
namespace DevExpress.XtraLayout.Utils {
	public class PaddingConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string text1 = value as string;
			if (text1 == null) {
				return base.ConvertFrom(context, culture, value);
			}
			text1 = text1.Trim();
			if (text1.Length == 0) {
				return null;
			}
			if (culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char ch1 = culture.TextInfo.ListSeparator[0];
			char[] chArray1 = new char[] { ch1, ',' } ;
			string[] textArray1 = text1.Split(chArray1);
			int[] numArray1 = new int[textArray1.Length];
			TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(int));
			for (int num1 = 0; num1 < numArray1.Length; num1++) {
				numArray1[num1] = (int) converter1.ConvertFromString(context, culture, textArray1[num1]);
			}
			if (numArray1.Length == 4) {
				return new Padding(numArray1[0], numArray1[1], numArray1[2], numArray1[3]);
			}
			object[] objArray1 = new object[3] { "value", text1, "left, right, top, bottom" } ;
			throw new ArgumentException("format error");
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if (value is Padding) {
				if (destinationType == typeof(string)) {
					Padding padding1 = (Padding) value;
					if (culture == null) {
						culture = CultureInfo.CurrentCulture;
					}
					string text1 = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(int));
					string[] textArray1 = new string[4];
					int num1 = 0;
					textArray1[num1++] = converter1.ConvertToString(context, culture, padding1.Left);
					textArray1[num1++] = converter1.ConvertToString(context, culture, padding1.Right);
					textArray1[num1++] = converter1.ConvertToString(context, culture, padding1.Top);
					textArray1[num1++] = converter1.ConvertToString(context, culture, padding1.Bottom);
					return string.Join(text1, textArray1);
				}
				if (destinationType == typeof(InstanceDescriptor)) {
					Padding padding2 = (Padding) value;
					Type[] typeArray2 = new Type[4] { typeof(int), typeof(int), typeof(int), typeof(int) } ;
					object[] objArray2 = new object[4] { padding2.Left, padding2.Right, padding2.Top, padding2.Bottom } ;
					return new InstanceDescriptor(typeof(Padding).GetConstructor(typeArray2), objArray2);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			if (context == null) {
				throw new ArgumentNullException("context");
			}
			if (propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			Padding padding1 = (Padding) context.PropertyDescriptor.GetValue(context.Instance);
			int num1 = (int) propertyValues["All"];
			if (padding1.All != num1) {
				return new Padding(num1);
			}
			return new Padding((int) propertyValues["Left"], (int) propertyValues["Right"], (int) propertyValues["Top"], (int) propertyValues["Bottom"]);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection1 = TypeDescriptor.GetProperties(typeof(Padding), attributes);
			string[] textArray1 = new string[5] { "All", "Left", "Right", "Top", "Bottom" } ;
			return collection1.Sort(textArray1);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
