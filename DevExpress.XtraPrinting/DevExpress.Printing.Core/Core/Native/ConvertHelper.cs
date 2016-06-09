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
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class ConvertHelper {
		public static readonly ConvertHelper Instance = new ConvertHelper();
		#region inner classes
		protected class SimpleConverter {
			public virtual bool Convert(string s, out object result) {
				result = s;
				return true;
			}
			static SimpleConverter instance;
			public static SimpleConverter Instance { get { if (instance == null) instance = new SimpleConverter(); return instance; } }
		}
		class DoubleConverter : SimpleConverter {
			public override bool Convert(string s, out object result) {
				double res;
				if (double.TryParse(s, out res)) {
					result = res;
					return true;
				}
				else {
					result = null;
					return false;
				}
			}
			static SimpleConverter instance;
			new public static SimpleConverter Instance { get { if (instance == null) instance = new DoubleConverter(); return instance; } }
		}
		class Int32Converter : SimpleConverter {
			public override bool Convert(string s, out object result) {
				int res;
				if (int.TryParse(s, out res)) {
					result = res;
					return true;
				}
				else {
					result = null;
					return false;
				}
			}
			static SimpleConverter instance;
			new public static SimpleConverter Instance { get { if (instance == null) instance = new Int32Converter(); return instance; } }
		}
		class BooleanConverter : SimpleConverter {
			public override bool Convert(string s, out object result) {
				bool res;
				if (bool.TryParse(s, out res)) {
					result = res;
					return true;
				}
				else {
					result = null;
					return false;
				}
			}
			static SimpleConverter instance;
			new public static SimpleConverter Instance { get { if (instance == null) instance = new BooleanConverter(); return instance; } }
		}
		#endregion
		static readonly Dictionary<Type, Func<object, object>> simpleConvertActions = new Dictionary<Type, Func<object, object>>();
		protected virtual SimpleConverter Converter { get { return null; } set { } }
		protected ConvertHelper() {
		}
		static ConvertHelper() {
			simpleConvertActions.Add(typeof(bool), delegate(object value) { return Convert.ToBoolean(value); });
			simpleConvertActions.Add(typeof(DateTime), delegate(object value) { return Convert.ToDateTime(value); });
			simpleConvertActions.Add(typeof(decimal), delegate(object value) { return Convert.ToDecimal(value); });
			simpleConvertActions.Add(typeof(double), delegate(object value) { return Convert.ToDouble(value); });
			simpleConvertActions.Add(typeof(float), delegate(object value) { return Convert.ToSingle(value); });
			simpleConvertActions.Add(typeof(int), delegate(object value) { return Convert.ToInt32(value); });
			simpleConvertActions.Add(typeof(short), delegate(object value) { return Convert.ToInt16(value); });
			simpleConvertActions.Add(typeof(long), delegate(object value) { return Convert.ToInt64(value); });
			simpleConvertActions.Add(typeof(uint), delegate(object value) { return Convert.ToUInt32(value); });
			simpleConvertActions.Add(typeof(ushort), delegate(object value) { return Convert.ToUInt16(value); });
			simpleConvertActions.Add(typeof(ulong), delegate(object value) { return Convert.ToUInt64(value); });
			simpleConvertActions.Add(typeof(string), delegate(object value) { return Convert.ToString(value); });
		}
		public object GetNativeValue(string s) {
			if (s == null || s.Length == 0)
				return s;
			object res;
			if (Converter != null) {
				if (Converter.Convert(s, out res))
					return res;
			}
			else {
				if (Int32Converter.Instance.Convert(s, out res)) {
					Converter = Int32Converter.Instance;
					return res;
				}
				if (DoubleConverter.Instance.Convert(s, out res)) {
					Converter = DoubleConverter.Instance;
					return res;
				}
				if (BooleanConverter.Instance.Convert(s, out res)) {
					Converter = BooleanConverter.Instance;
					return res;
				}
				Converter = SimpleConverter.Instance;
			}
			return s;
		}
		internal static object ToCodeType(IConvertible obj, object defaultValue) {
			try {
				switch (obj.GetTypeCode()) {
					case TypeCode.Boolean:
						return Convert.ToBoolean(obj);
					case TypeCode.Byte:
						return Convert.ToByte(obj);
					case TypeCode.Char:
						return Convert.ToChar(obj);
					case TypeCode.DateTime:
						return Convert.ToDateTime(obj);
					case TypeCode.Decimal:
						return Convert.ToDecimal(obj);
					case TypeCode.Double:
						return Convert.ToDouble(obj);
					case TypeCode.Int16:
						return Convert.ToInt16(obj);
					case TypeCode.Int32:
						return Convert.ToInt32(obj);
					case TypeCode.Int64:
						return Convert.ToInt64(obj);
					case TypeCode.SByte:
						return Convert.ToSByte(obj);
					case TypeCode.Single:
						return Convert.ToSingle(obj);
					case TypeCode.String:
						return Convert.ToString(obj);
					case TypeCode.UInt16:
						return Convert.ToUInt16(obj);
					case TypeCode.UInt32:
						return Convert.ToUInt32(obj);
					case TypeCode.UInt64:
						return Convert.ToUInt64(obj);
					default:
						return defaultValue;
				}
			}
			catch { return defaultValue; }
		}
		public static object SimpleConvertValue(Type toType, object value) {
			Func<object, object> func;
			if(simpleConvertActions.TryGetValue(toType, out func))
				return func(value);
			return GetDefaultValue(toType);
		}
		internal static object GetDefaultValue(Type type) {
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}
