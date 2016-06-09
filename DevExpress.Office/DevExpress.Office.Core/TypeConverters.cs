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
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
namespace DevExpress.Office.Design {
	public class EnumLikeStructTypeConverter<T> : TypeConverter {
		static readonly Dictionary<T, FieldInfo> fieldsTable = CreateFieldsTable();
		static readonly Dictionary<T, PropertyInfo> propertiesTable = CreatePropertiesTable();
#if !SL
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<T> values = new List<T>();
			foreach (T format in fieldsTable.Keys)
				values.Add(format);
			foreach (T format in propertiesTable.Keys)
				values.Add(format);
			return new TypeConverter.StandardValuesCollection(values);
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual System.ComponentModel.Design.Serialization.InstanceDescriptor ConvertToInstanceDescriptor(ITypeDescriptorContext context, T value) {
			FieldInfo field;
			if (fieldsTable.TryGetValue(value, out field))
				return new System.ComponentModel.Design.Serialization.InstanceDescriptor(field, null);
			else
				return null;
		}
#endif
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(String))
				return true;
#if !SL
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
#endif
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(String))
				return ConvertToStringCore(context, culture, (T)value);
#if !SL
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return ConvertToInstanceDescriptor(context, (T)value);
#endif
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(String))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value.GetType() == typeof(String))
				return ConvertFromStringCore(context, culture, (string)value);
			return base.ConvertFrom(context, culture, value);
		}
		static Dictionary<T, FieldInfo> CreateFieldsTable() {
			Dictionary<T, FieldInfo> result = new Dictionary<T, FieldInfo>();
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
			int count = fields.Length;
			for (int i = 0; i < count; i++) {
				FieldInfo field = fields[i];
				if (field.FieldType == typeof(T) && field.IsInitOnly) {
					object[] attributes = field.GetCustomAttributes(typeof(BrowsableAttribute), true);
					if (attributes == null || attributes.Length <= 0 || ((BrowsableAttribute)attributes[0]).Browsable) {
						T format = (T)field.GetValue(null);
						result.Add(format, field);
					}
				}
			}
			return result;
		}
		static Dictionary<T, PropertyInfo> CreatePropertiesTable() {
			Dictionary<T, PropertyInfo> result = new Dictionary<T, PropertyInfo>();
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
			int count = properties.Length;
			for (int i = 0; i < count; i++) {
				PropertyInfo property = properties[i];
				if (property.PropertyType == typeof(T) && property.CanWrite) {
					T format = (T)property.GetValue(null, new object[] { });
					result.Add(format, property);
				}
			}
			return result;
		}
		protected internal virtual string ConvertToStringCore(ITypeDescriptorContext context, CultureInfo culture, T value) {
			FieldInfo field;
			if (fieldsTable.TryGetValue(value, out field))
				return field.Name;
			else
				return value.ToString();
		}
		protected internal virtual T ConvertFromStringCore(ITypeDescriptorContext context, CultureInfo culture, string value) {
			foreach (FieldInfo field in fieldsTable.Values) {
				if (field.Name == value)
					return (T)field.GetValue(null);
			}
			return default(T);
		}
	}
#if !SL
	public class EncodingComparer : IComparer<Encoding> {
		#region IComparer<Encoding> Members
		public int Compare(Encoding x, Encoding y) {
			return Comparer<string>.Default.Compare(x.EncodingName, y.EncodingName);
		}
		#endregion
	}
	public class EncodingConverter : EnumLikeStructTypeConverter<Encoding> {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			EncodingInfo[] encodingInfos = Encoding.GetEncodings();
			int count = encodingInfos.Length;
			List<Encoding> values = new List<Encoding>(count);
			for (int i = 0; i < count; i++)
				values.Add(encodingInfos[i].GetEncoding());
			values.Sort(new EncodingComparer());
			return new TypeConverter.StandardValuesCollection(values);
		}
		protected internal override string ConvertToStringCore(ITypeDescriptorContext context, CultureInfo culture, Encoding value) {
			if (value != null)
				return value.EncodingName;
			else
				return base.ConvertToStringCore(context, culture, value);
		}
		protected internal override Encoding ConvertFromStringCore(ITypeDescriptorContext context, CultureInfo culture, string value) {
			EncodingInfo[] encodingInfos = Encoding.GetEncodings();
			int count = encodingInfos.Length;
			for (int i = 0; i < count; i++) {
				Encoding encoding = encodingInfos[i].GetEncoding();
				if (String.Compare(encoding.EncodingName, value, true) == 0)
					return encoding;
			}
			return Encoding.Default;
		}
	}
#endif
}
