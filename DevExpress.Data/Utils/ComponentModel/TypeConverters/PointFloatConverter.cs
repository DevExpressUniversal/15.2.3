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
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Utils {
   public class PointFloatConverter : TypeConverter {
		#region System.Drawing.SR access
		static Type SR {
			get { return typeof(PointF).GetAssembly().GetType("System.Drawing.SR"); } 
		}
		static string SRGetString(string str) {
			return (string)SR.GetMethod("GetString", new Type[] { typeof(string) }).Invoke(null, new object[] { str });
		}
		static string SRGetString(string str, params object[] args) {
			return (string)SR.GetMethod("GetString", new Type[] { typeof(string), typeof(object[]) }).Invoke(null, new object[] { str, args });
		}
		#endregion
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if(stringValue == null) {
				return base.ConvertFrom(context, culture, value);
			}
			stringValue = stringValue.Trim();
			if(stringValue.Length == 0) 
				return null;
			if(culture == null)
				culture = CultureInfo.CurrentCulture;
			char listSeparatorChar = culture.TextInfo.ListSeparator[0];
			string[] stringValues = stringValue.Split(new char[] { listSeparatorChar });
			float[] floatValues = new float[stringValues.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
			for(int i = 0; i < floatValues.Length; i++) {
				floatValues[i] = (float)converter.ConvertFromString(context, culture, stringValues[i]);
			}
			if(floatValues.Length != 2)
				throw new ArgumentException(SRGetString("TextParseFailedFormat", new object[] { stringValue, "x" + listSeparatorChar + " y" }));
			return new PointFloat(floatValues[0], floatValues[1]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(destinationType == typeof(string) && value is PointFloat) {
				PointFloat point = (PointFloat)value;
				if(culture == null) {
					culture = CultureInfo.CurrentCulture;
				}
				string separator = culture.TextInfo.ListSeparator + " ";
				TypeConverter converter = GetSingleConverter();
				return string.Join(separator, new string[] { converter.ConvertToString(context, culture, point.X), converter.ConvertToString(context, culture, point.Y)});
			}
			if(destinationType == typeof(InstanceDescriptor) && value is PointFloat) {
				PointFloat point = (PointFloat)value;
				ConstructorInfo constructor = typeof(PointFloat).GetConstructor(new Type[] { typeof(float), typeof(float) });
				if(constructor != null) {
					return new InstanceDescriptor(constructor, new object[] { point.X, point.Y });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			object x = propertyValues["X"];
			object y = propertyValues["Y"];
			if(x == null || y == null || !(x is float) || !(y is float))
				throw new ArgumentException(SRGetString("PropertyValueInvalidEntry"));
			return new PointFloat((float)x, (float)y);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(PointFloat), attributes).Sort(new string[] { "X", "Y" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	   protected virtual TypeConverter GetSingleConverter() {
		   return TypeDescriptor.GetConverter(typeof(float));
	   }
	}
	public class PointFloatConverterForDisplay : PointFloatConverter {
		protected override TypeConverter GetSingleConverter() {
			return new DevExpress.Utils.Design.SingleTypeConverter();
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
