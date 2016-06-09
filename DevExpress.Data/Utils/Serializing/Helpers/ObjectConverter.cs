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
using System.IO;
#if !DXPORTABLE
using System.Runtime.Serialization;
#endif
using System.Drawing;
using System.Reflection;
using DevExpress.Utils.Serializing;
using System.Globalization;
using System.Text;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if !DXPORTABLE
#if !SILVERLIGHT
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
#else
using DevExpress.Data.Browsing;
#endif
#endif
namespace DevExpress.Utils.Serializing.Helpers {
	public class ObjectConverter {
		public static readonly ObjectConverterImplementation Instance = new ObjectConverterImplementation();
#if !SILVERLIGHT && !DXPORTABLE
		internal static BinaryFormatter BinaryFormatter { get { return Instance.BinaryFormatter; } }
#endif
		public static string ObjectToString(object obj) {
			return Instance.ObjectToString(obj);
		}
		internal static string GetNextPart(string str, ref int index) {
			return Instance.GetNextPart(str, ref index);
		}
		internal static int GetIndexOfDelimiter(string str, int index) {
			return Instance.GetIndexOfDelimiter(str, index);
		}
		public static object StringToObject(string str, Type type) {
			return Instance.StringToObject(str, type);
		}
#if !SILVERLIGHT
		public static string IntStructureToString(object obj) {
			return Instance.IntStructureToString(obj);
		}
		public static object IntStringToStructure(string str, Type type) {
			return Instance.IntStringToStructure(str, type);
		}
#endif
	}
	public class ObjectConverterImplementation {
		ObjectConverters converters;
		public void CopyConvertersTo(ObjectConverterImplementation ocImplTo){
			Converters.CopyTo(ocImplTo.Converters);
		}
#if !SILVERLIGHT && !DXPORTABLE
		BinaryFormatter formatter;
		#region inner classes
		class ColorConverter : IOneTypeObjectConverter {
			System.Drawing.ColorConverter colorConverter;
			public Type Type { get { return typeof(Color); } }
			public ColorConverter() {
				colorConverter = new System.Drawing.ColorConverter();
			}
			public string ToString(object obj) {
				return colorConverter.ConvertToInvariantString(obj);
			}
			public object FromString(string str) {
				return colorConverter.ConvertFromInvariantString(str);
			}
		}
#if !DXPORTABLE
		class DXSerializationBinder : SerializationBinder {
			public override Type BindToType(string assemblyName, string typeName) {
				if(typeName == "DevExpress.XtraPivotGrid.PivotGridFieldFilterValues") 
					assemblyName = AssemblyInfo.SRAssemblyPivotGridCore; 
				typeName = DevExpress.Utils.Design.DXAssemblyResolverEx.GetValidTypeName(typeName);
				assemblyName = DevExpress.Utils.Design.DXAssemblyResolverEx.GetValidAssemblyName(assemblyName);
				Assembly asm = GetAssembly(assemblyName);
				if(asm != null)
					return asm.GetType(typeName, false);
				return null;
			}
			Assembly GetAssembly(string assemblyName) {
				Assembly asm = null;
				try {
					if(assemblyName.ToLower().StartsWith("devexpress"))
						asm = DevExpress.Utils.Design.DXAssemblyResolverEx.FindAssembly(assemblyName);
					if(asm != null)
						return asm;
#if DXWhidbey
#pragma warning disable 612, 618
					asm = Assembly.Load(assemblyName, null);
#pragma warning restore 612, 618
#else
							asm = Assembly.LoadWithPartialName(assemblyName);
#endif
				} catch {
					return null;
				}
				return asm;
			}
		}
#endif
		#endregion
#endif
		public Type ResolveType(string typeName) {
			return Converters.ResolveType(typeName);
		}
		public void RegisterConverter(IOneTypeObjectConverter converter) {
			Converters.RegisterConverter(converter);
		}
		public void UnregisterConverter(Type type) {
			Converters.UnregisterConverter(type);
		}
		public bool CanConvertToString(Type type) {
			return Converters.IsConverterExists(type);
		}
		public bool CanConvertFromString(Type type, string s) {
			IOneTypeObjectConverter converter = Converters.GetConverter(type);
			return converter is IOneTypeObjectConverter2 ? ((IOneTypeObjectConverter2)converter).CanConvertFromString(s) :
				converter != null;
		}
		public string ConvertToString(object obj) {
			return Converters.ConvertToString(obj);
		}
		public object ConvertFromString(Type type, string str) {
			return Converters.ConvertFromString(type, str);
		}
		public ObjectConverterImplementation() {
			converters = new ObjectConverters();
		}
		protected virtual ObjectConverters Converters {
			get { return converters; }
		}
#if !SILVERLIGHT && !DXPORTABLE
		internal BinaryFormatter BinaryFormatter {
			get {
				if(formatter != null)
					return formatter;
				formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				formatter.Binder = new DXSerializationBinder();
				return formatter;
			}
		}
#endif
		TypeConverter GetToStringConverter(Type type) {
			TypeConverter tc = TypeDescriptor.GetConverter(type);
#if SILVERLIGHT
			System.ComponentModel.TypeConverter guidConverter = TypeHelper.GetConverter(type);
			if(guidConverter != null) {
				if(guidConverter.CanConvertTo(typeof(string)) && guidConverter.CanConvertFrom(typeof(string)))
					return guidConverter;
			}
#endif
			if(tc.CanConvertTo(typeof(string)) && tc.CanConvertFrom(typeof(string)))
				return tc;
			return null;
		}
		public virtual string ObjectToString(object obj) {
			if(obj == null)
				return null;
			if(obj is string)
				return (string)obj;
			Type type = obj.GetType();
			if(CanConvertToString(type))
				return ConvertToString(obj);
			if(IsStructure(type))
				return StructureToString(obj);
			if(type.IsArray)
				return ArrayToString(obj, type);
			TypeConverter tc = GetToStringConverter(type);
			if(tc != null)
				return (string)tc.ConvertTo(null, CultureInfo.InvariantCulture, obj, typeof(String));
			if(!IsTypeSerializable(type))
#if !SILVERLIGHT
				return null;
#else
				if(type.IsValueType)
					return Convert.ToString(obj, CultureInfo.InvariantCulture).Replace(',', '.');
				else if(type == typeof(string))
					return (string)obj;
				else return null;
#endif
			return SerialzeWithBinaryFormatter(obj);
		}
		protected internal virtual bool IsTypeSerializable(Type type) {
#if !SILVERLIGHT && !DXPORTABLE
			return type.IsSerializable;
#else
			if((type.GetTypeInfo().Attributes & TypeAttributes.Serializable) != 0) {
				return true;
			}
			return false;
#endif
		}
		protected virtual string SerialzeWithBinaryFormatter(object obj) {
#if !SILVERLIGHT && !DXPORTABLE
			MemoryStream ms = new MemoryStream();
			IFormatter formatter = BinaryFormatter;
			formatter.Serialize(ms, obj);
			return ToBase64String(ms.ToArray());
#else
			return String.Empty;
#endif
		}
		string ArrayToString(object obj, Type t) {
			Array array = (Array)obj;
			StringBuilder result = new StringBuilder();
			result.Append(XtraSerializer.ArrayValue).Append(array.Length).Append(", ");
			for(int i = 0; i < array.Length; i++)
				result.Append(ArrayElementToString(array.GetValue(i)).Replace(",", ",,")).Append(", ");
			return result.ToString();
		}
		string ArrayElementToString(object obj) {
			if(obj == null)
				return "null";
			Type t = obj.GetType();
			if(!t.IsPrimitive() && !(obj is string) && !(obj is DateTime) && !(obj is Decimal)) {
				string objAsString = ObjectToString(obj);
				if(objAsString == null)
					throw new Exception(string.Format("Class {0} should either have a TypeConverter that can convert to/from a string, or implement the ISerializable interface to solve the problem.", obj.GetType()));
				return TypeCode.Object.ToString() + ", " + t.AssemblyQualifiedName.Replace(",", ",,") + ", " + objAsString.Replace(",", ",,");
			}
			TypeCode typeCode = DXTypeExtensions.GetTypeCode(t);
			return typeCode.ToString() + ", " + PrimitiveToString(obj).Replace(",", ",,");
		}
		string PrimitiveToString(object obj) {
			if(obj is double)
				return ((double)obj).ToString("R", CultureInfo.InvariantCulture);
			return Convert.ToString(obj, CultureInfo.InvariantCulture);
		}
		internal string GetNextPart(string str, ref int index) {
			int startIndex = index,
				endIndex = GetIndexOfDelimiter(str, index);
			string result;
			if(endIndex < 0) {
				index = str.Length;
				result = str.Substring(startIndex);
			} else {
				index = endIndex + 2;
				result = str.Substring(startIndex, endIndex - startIndex);
			}
			return result.Replace(",,", ",");
		}
		internal int GetIndexOfDelimiter(string str, int index) {
			for(int i = index; i < str.Length - 1; i++) {
				if(str[i] == ',') {
					switch(str[i + 1]) {
						case ',':
							i++;
							break;
						case ' ':
							return i;
					}
				}
			}
			return -1;
		}
		object StringToArray(string str, Type type) {
			str = str.Substring(XtraSerializer.ArrayValue.Length);
			int length = 0, index = 0, elemIndex = 0;
			if(!Int32.TryParse(GetNextPart(str, ref index), out length))
				return null;
			Array result;
			if(type == typeof(object))
				result = new object[length];
			else
				result = Array.CreateInstance(type.GetElementType(), length);
			while(index < str.Length && elemIndex < length) {
				string nextPart = GetNextPart(str, ref index);
				result.SetValue(StringToArrayElement(nextPart), elemIndex);
				elemIndex++;
			}
			return result;
		}
		object StringToArrayElement(string part) {
			int index = 0;
			string typeCodeStr = GetNextPart(part, ref index);
			if(typeCodeStr == "null")
				return null;
			TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), typeCodeStr, false);
			if(typeCode == TypeCode.Object) {
				string typeName = GetNextPart(part, ref index);
				return StringToObject(GetNextPart(part, ref index), Type.GetType(typeName));
			} else {
				string nextPart = GetNextPart(part, ref index);
				return Convert.ChangeType(nextPart, typeCode, CultureInfo.InvariantCulture);
			}
		}
		string ToBase64String(byte[] array) {
			return string.Concat(XtraSerializer.Base64Value, Convert.ToBase64String(array));
		}
		object Base64ToObject(string str, Type type) {
			byte[] array = Convert.FromBase64String(str.Substring(XtraSerializer.Base64Value.Length));
#if !SILVERLIGHT && !DXPORTABLE
			return BinaryFormatter.Deserialize(new MemoryStream(array));
#else
			return null;
#endif
		}
		public virtual object StringToObject(string str, Type type) {
			if(type.Equals(typeof(string)))
				return str;
			if(string.IsNullOrEmpty(str))
				return null;
			if(str.StartsWith(XtraSerializer.Base64Value))
				return Base64ToObject(str, type);
			if(str.StartsWith(XtraSerializer.ArrayValue))
				return StringToArray(str, type);
			if(CanConvertFromString(type, str))
				return ConvertFromString(type, str);
			if(type.IsEnum())
				return EnumToObject(str, type);
			if(IsStructure(type))
				return StringToStructure(str, type);
			TypeConverter tc = GetToStringConverter(type);
			if(tc != null) {
				object obj = tc.ConvertFrom(null, CultureInfo.InvariantCulture, str);
				if(IsNullable(type))
					return obj;
				if(obj != null && type.IsAssignableFrom(obj.GetType()))
					return obj;
				return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
			}
			return Convert.ChangeType(str, type, CultureInfo.InvariantCulture);
		}
		static bool IsNullable(Type t) {
			return (t.IsGenericType() && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
		}
		static Type[] structTypes = new Type[] { typeof(Point), typeof(PointF), typeof(Size), typeof(SizeF), typeof(Rectangle), typeof(RectangleF) };
		static bool IsStructure(Type t) {
			return Array.IndexOf(structTypes, t) != -1;
		}
		public string IntStructureToString(object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			StringBuilder res = new StringBuilder();
			foreach(PropertyDescriptor prop in props) {
				if(prop.IsReadOnly)
					continue;
				if(!prop.PropertyType.IsPrimitive())
					continue;
				string val = ObjectToString(prop.GetValue(obj));
				if(val == null)
					val = String.Empty;
				res.AppendFormat("{2}{0}={1}", prop.Name, val, res.Length > 0 ? "," : string.Empty);
			}
			return res.ToString(); 
		}
		public object IntStringToStructure(string str, Type type) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(type);
			object res = Activator.CreateInstance(type);
			string[] strs = str.Split(new Char[] { ',', '=' });
			for(int n = 0; n < strs.Length; n += 2) {
				PropertyDescriptor prop = props[strs[n]];
				object realVal = StringToObject(strs[n + 1], prop.PropertyType);
				prop.SetValue(res, realVal);
			}
			return res;
		}
		string StructureToString(object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			StringBuilder res = new StringBuilder();
			foreach(PropertyDescriptor prop in props) {
				if(prop.IsReadOnly)
					continue;
				if(!prop.PropertyType.IsPrimitive())
					continue;
				string val = ObjectToString(prop.GetValue(obj));
				if(val == null)
					val = String.Empty;
				res.AppendFormat("@{2},{0}={1}", prop.Name, val, val.Length);
			}
			return res.ToString(); 
		}
		object StringToStructure(string str, Type type) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(type);
			object res = Activator.CreateInstance(type);
			int n = 0;
			while(n < str.Length) {
				int start = str.IndexOf("@", n);
				if(start == -1)
					break;
				n = start + 1;
				string[] strs = str.Substring(n).Split(new Char[] { ',', '=' });
				if(strs == null || strs.Length < 2)
					break;
				int len = Convert.ToInt32(strs[0]);
				string fname = strs[1];
				start = n + str.Substring(n).IndexOf("=") + 1;
				PropertyDescriptor prop = props[fname];
				if(prop == null)
					continue;
				string val = str.Substring(start, len);
				n = start + len;
				object realVal = StringToObject(val, prop.PropertyType);
				prop.SetValue(res, realVal);
			}
			return res; 
		}
		static object EnumToObject(string str, Type type) {
			return Enum.Parse(type, str, false);
		}
	}
}
