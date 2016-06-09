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
using System.Security.Cryptography;
using System.Reflection;
using System.Text;
namespace DevExpress.Utils {
	public static class CultureInfoExtensions {
		public static string GetDateSeparator(this CultureInfo culture) {
			return culture.DateTimeFormat.DateSeparator;
		}
		public static string GetTimeSeparator(this CultureInfo culture) {
			return culture.DateTimeFormat.TimeSeparator;
		}
		public static int GetLCID(this CultureInfo culture) {
			return culture.LCID;
		}
		public static CultureInfo GetCultureInfo(string name) {
			return CultureInfo.GetCultureInfo(name);
		}
		public static CultureInfo CreateSpecificCulture(string name) {
			return CultureInfo.CreateSpecificCulture(name);
		}
		public static string[] GetAllDateTimePatterns(this CultureInfo culture) {
			return culture.DateTimeFormat.GetAllDateTimePatterns();
		}
		public static string[] GetAllDateTimePatterns(this CultureInfo culture, char format) {
			return culture.DateTimeFormat.GetAllDateTimePatterns(format);
		}
		public static void SetCurrentCulture(CultureInfo culture) {
			System.Threading.Thread.CurrentThread.CurrentCulture = culture;
		}
		public static void SetCurrentUICulture(CultureInfo culture) {
			System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
		}
	}
	public static class DXMarshal {
		public static int SizeOf(Type type) {
			return System.Runtime.InteropServices.Marshal.SizeOf(type);
		}
		public static int SizeOf<T>() {
			return System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
		}
		public static T PtrToStructure<T>(IntPtr prt) {
			return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(prt, typeof(T));
		}
	}
	public static class DXConvert {
		public static bool IsDBNull(object value) {
			return Convert.IsDBNull(value);
		}
	}
	public static class DXListExtensions {
		public static List<TOutput> ConvertAll<TInput, TOutput>(List<TInput> instance, Converter<TInput, TOutput> converter) {
			return instance.ConvertAll<TOutput>(converter);
		}
	}
	public static class HashAlgorithmExtensions {
		public static byte[] Transform2Blocks(this HashAlgorithm hashAlgorithm, byte[] first, byte[] second) {
			hashAlgorithm.Initialize();
			hashAlgorithm.TransformBlock(first, 0, first.Length, null, 0);
			hashAlgorithm.TransformFinalBlock(second, 0, second.Length);
			return hashAlgorithm.Hash;
		}
	}
	public static class DateTimeExtensions {
		public static DateTime FromOADate(double d) {
			return DateTime.FromOADate(d);
		}
	}
	public static class DXDelegateExtensions {
		public static MethodInfo GetMethodInfo(this Delegate instance) {
			return instance.Method;
		}
	}
	public static class DXMethodInfoExtensions {
		public static Delegate CreateDelegate(this MethodInfo instance, Type delegateType) {
			return Delegate.CreateDelegate(delegateType, instance);
		}
		public static Delegate CreateDelegate(this MethodInfo instance, Type delegateType, object target) {
			return Delegate.CreateDelegate(delegateType, target, instance);
		}
	}
	public static class DXTypeExtensions {
		public const TypeCode TypeCodeDBNull = TypeCode.DBNull;
		public static Assembly GetAssembly(this Type type) {
			return type.Assembly;
		}
		public static TypeCode GetTypeCode(Type type) {
			return Type.GetTypeCode(type);
		}
		public static bool IsEnum(this Type sourceType) {
			return sourceType.IsEnum;
		}
		public static bool IsValueType(this Type sourceType) {
			return sourceType.IsValueType;
		}
		public static bool IsPublic(this Type type) {
			return type.IsPublic;
		}
		public static bool IsNestedPublic(this Type type) {
			return type.IsNestedPublic;
		}
		public static bool IsInterface(this Type type) {
			return type.IsInterface;
		}
		public static bool IsVisible(this Type type) {
			return type.IsVisible;
		}
		public static bool IsClass(this Type type) {
			return type.IsClass;
		}
		public static bool IsAbstract(this Type type) {
			return type.IsAbstract;
		}
		public static bool IsSealed(this Type type) {
			return type.IsAbstract;
		}
		public static bool IsGenericType(this Type type) {
			return type.IsGenericType;
		}
		public static bool IsPrimitive(this Type sourceType) {
			return sourceType.IsPrimitive;
		}
		public static Type GetBaseType(this Type sourceType) {
			return sourceType.BaseType;
		}
	}
	public static class CustomAttributeExtensions {
		public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType) {
			return Attribute.GetCustomAttribute(element, attributeType);
		}
	}
	public static class StringExtensions {
		public static StringComparer ComparerInvariantCulture { get { return StringComparer.InvariantCulture; } }
		public static StringComparer ComparerInvariantCultureIgnoreCase { get { return StringComparer.InvariantCultureIgnoreCase; } }
		public static UnicodeCategory GetUnicodeCategory(this char c) {
			return CharUnicodeInfo.GetUnicodeCategory(c);
		}
		public static int CompareInvariantCultureIgnoreCase(string str1, string str2) {
			return String.Compare(str1, str2, StringComparison.InvariantCultureIgnoreCase);
		}
		public static int CompareInvariantCulture(string str1, string str2) {
			return String.Compare(str1, str2, StringComparison.InvariantCulture);
		}
		public static int CompareInvariantCultureWithOptions(string str1, string str2, CompareOptions options) {
			return CultureInfo.InvariantCulture.CompareInfo.Compare(str1, str2, options);
		}
		public static int CompareWithCultureInfoAndOptions(string str1, string str2, CultureInfo info, CompareOptions options) {
			return String.Compare(str1, str2, info, options);
		}
		public static int IndexOfInvariantCultureIgnoreCase(this string str1, string str2) {
			return str1.IndexOf(str2, StringComparison.InvariantCultureIgnoreCase);
		}
		public static int IndexOfInvariantCulture(this string str1, string str2) {
			return str1.IndexOf(str2, StringComparison.InvariantCulture);
		}
		public static int IndexOfInvariantCultureIgnoreCase(this string str1, string str2, int startIndex) {
			return str1.IndexOf(str2, startIndex, StringComparison.InvariantCultureIgnoreCase);
		}
		public static int IndexOfInvariantCulture(this string str1, string str2, int startIndex) {
			return str1.IndexOf(str2, startIndex, StringComparison.InvariantCulture);
		}
		public static bool StartsWithInvariantCultureIgnoreCase(this string str1, string str2) {
			return str1.StartsWith(str2, StringComparison.InvariantCultureIgnoreCase);
		}
		public static bool StartsWithInvariantCulture(this string str1, string str2) {
			return str1.StartsWith(str2, StringComparison.InvariantCulture);
		}
		public static bool EndsWithInvariantCultureIgnoreCase(this string str1, string str2) {
			return str1.EndsWith(str2, StringComparison.InvariantCultureIgnoreCase);
		}
		public static bool EndsWithInvariantCulture(this string str1, string str2) {
			return str1.EndsWith(str2, StringComparison.InvariantCulture);
		}
	}
}
