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
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Charts.Native {
	public interface ICoreReference {}
	public static class SortingUtils {
		public static int CompareDoubles(double number1, double number2) {
			if (number1 < number2)
				return -1;
			if (number1 > number2)
				return 1;
			return 0;
		}
	}
	public static class ObjectToStringConversion {
		public static string ObjectToString(object obj) {
			return obj == null ? String.Empty : obj.ToString();
		}
		public static object StringToObject(string str) {
			return String.IsNullOrEmpty(str) ? null : str;
		}
	}
	public class ObjectTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return (value is string) ? ObjectToStringConversion.StringToObject((string)value) : base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(string) ? ObjectToStringConversion.ObjectToString(value) : 
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public static class XtraSerializingUtils {
		const string typeNamePropertyName = "TypeNameSerializable";
		public static object GetContentPropertyInstance(XtraPropertyInfo propertyInfo, Assembly executingAssembly, string typeNamespace) {
			string typeName = propertyInfo.ChildProperties[typeNamePropertyName].Value.ToString();
			Type type = executingAssembly.GetType(typeNamespace + typeName);
			return Activator.CreateInstance(type);
		}
		public static object GetContentPropertyInstance(XtraItemEventArgs e, Assembly executingAssembly, string typeNamespace) {
			return GetContentPropertyInstance(e.Item, executingAssembly, typeNamespace);
		}
	}
	public enum SeriesPointKeyNative {
		Argument = 0,
		Value_1 = 1,
		Value_2 = 2,
		Value_3 = 3,
		Value_4 = 4
	}
}
