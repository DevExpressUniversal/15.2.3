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
using System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils.Design {
	public class PointFTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) {
				return base.ConvertFrom(context, culture, value);
			}
			string str2 = str.Trim();
			if(str2.Length == 0) {
				return null;
			}
			if(culture == null) {
				culture = CultureInfo.CurrentCulture;
			}
			char ch = culture.TextInfo.ListSeparator[0];
			string[] strArray = str2.Split(new char[] { ch });
			float[] numArray = new float[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(float));
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (float)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 2) {
				throw new ArgumentException("value");
			}
			return new PointF(numArray[0], numArray[1]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(destinationType == typeof(string) && value is PointF) {
				PointF ef = (PointF)value;
				if(culture == null) {
					culture = CultureInfo.CurrentCulture;
				}
				string[] strArray = new string[2];
				strArray[0] = SingleTypeConverter.ToString(context, culture, ef.X);
				strArray[1] = SingleTypeConverter.ToString(context, culture, ef.Y);
				string separator = culture.TextInfo.ListSeparator + " ";
				return string.Join(separator, strArray);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			return new PointF((float)propertyValues["X"], (float)propertyValues["Y"]);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return PathProperties(typeof(PointF), attributes).Sort(new string[] { "X", "Y" });
		}
		static PropertyDescriptorCollection PathProperties(Type type, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type, attributes);
			List<PropertyDescriptor> props = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				props.Add(TypeDescriptorHelper.CreateProperty(type,
					propertyDescriptor,
					new Attribute[] { new TypeConverterAttribute(typeof(SingleTypeConverter)) }));
			}
			return new PropertyDescriptorCollection(props.ToArray());
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
