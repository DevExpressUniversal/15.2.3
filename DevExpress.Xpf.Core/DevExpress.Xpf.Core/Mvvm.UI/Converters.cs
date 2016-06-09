﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

#if !FREE
using DevExpress.Utils;
#endif
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.Native;
#if !NETFX_CORE
using System.Windows.Data;
using System.Windows.Markup;
using System.Collections.Generic;
using DevExpress.Mvvm.UI.Native;
using System.Reflection;
using System.Collections;
using System.Windows.Media;
using System.Text.RegularExpressions;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using CultureInfo = System.String;
using Windows.UI;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media;
#endif
namespace DevExpress.Mvvm.UI {
#if !NETFX_CORE && !SILVERLIGHT
	public class ReflectionConverter : IValueConverter {
		class TypeUnsetValue { }
		Type convertBackMethodOwner = typeof(TypeUnsetValue);
		class ConvertMethodSignature {
			int valueIndex;
			int targetTypeIndex;
			int parameterIndex;
			int cultureIndex;
			public ConvertMethodSignature(Type[] parameterTypes, int valueIndex, int targetTypeIndex, int parameterIndex, int cultureIndex) {
				ParameterTypes = parameterTypes;
				this.valueIndex = valueIndex;
				this.targetTypeIndex = targetTypeIndex;
				this.parameterIndex = parameterIndex;
				this.cultureIndex = cultureIndex;
			}
			public Type[] ParameterTypes { get; private set; }
			public void AssignArgs(object[] args, object value, Type targetType, object parameter, CultureInfo culture) {
				args[valueIndex] = value;
				if(targetTypeIndex >= 0)
					args[targetTypeIndex] = targetType;
				if(parameterIndex >= 0)
					args[parameterIndex] = parameter;
				if(cultureIndex >= 0)
					args[cultureIndex] = culture;
			}
		}
		static readonly ConvertMethodSignature[] ConvertMethodSignatures = new ConvertMethodSignature[] {
			new ConvertMethodSignature(new Type[] { null }, 0, -1, -1, -1),
			new ConvertMethodSignature(new Type[] { null, typeof(CultureInfo) }, 0, -1, -1, 1),
			new ConvertMethodSignature(new Type[] { null, typeof(Type) }, 0, 1, -1, -1),
			new ConvertMethodSignature(new Type[] { null, null }, 0, -1, 1, -1),
			new ConvertMethodSignature(new Type[] { null, typeof(Type), null }, 0, 1, 2, -1),
			new ConvertMethodSignature(new Type[] { null, typeof(Type), typeof(CultureInfo) }, 0, 1, -1, 2),
			new ConvertMethodSignature(new Type[] { null, null, typeof(CultureInfo) }, 0, -1, 1, 2),
			new ConvertMethodSignature(new Type[] { null, typeof(Type), null, typeof(CultureInfo) }, 0, 1, 2, 3),
		};
		public Type ConvertMethodOwner { get; set; }
		public string ConvertMethod { get; set; }
		public Type ConvertBackMethodOwner {
			get { return convertBackMethodOwner == typeof(TypeUnsetValue) ? ConvertMethodOwner : convertBackMethodOwner; }
			set { convertBackMethodOwner = value; }
		}
		public string ConvertBackMethod { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConvertCore(value, targetType, parameter, culture, ConvertMethodOwner, ConvertMethod);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConvertCore(value, targetType, parameter, culture, ConvertBackMethodOwner, ConvertBackMethod);
		}
		static object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture, Type convertMethodOwner, string convertMethod) {
			if(convertMethodOwner == null) {
				if(convertMethod == null)
					return targetType == null ? value : ConvertByTargetTypeConstructor(value, targetType, parameter, culture);
				else
					return value == null ? null : ConvertBySourceValueMethod(value, targetType, parameter, culture, convertMethod);
			} else {
				if(convertMethod == null)
					return ConvertByConstructor(value, targetType, parameter, culture, convertMethodOwner);
				else
					return ConvertByStaticMethod(value, targetType, parameter, culture, convertMethodOwner, convertMethod);
			}
		}
		static object ConvertByTargetTypeConstructor(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConvertByConstructor(value, targetType, parameter, culture, targetType.GetConstructors());
		}
		static object ConvertByConstructor(object value, Type targetType, object parameter, CultureInfo culture, Type convertMethodOwner) {
			return ConvertByConstructor(value, targetType, parameter, culture, convertMethodOwner.GetConstructors());
		}
		static object ConvertByConstructor(object value, Type targetType, object parameter, CultureInfo culture, IEnumerable<ConstructorInfo> methods) {
			if(value == null && (targetType == null || !targetType.IsValueType || targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))) return null;
			ConstructorInfo convertMethod = methods.Where(c => c.GetParameters().Length == 1).FirstOrDefault();
			if(convertMethod == null)
				convertMethod = methods.Where(c => c.GetParameters().Length > 0 && !c.GetParameters().Skip(1).Any(p => !p.IsOptional)).FirstOrDefault();
			if(convertMethod == null)
				throw new InvalidOperationException();
			ParameterInfo[] parameters = convertMethod.GetParameters();
			object[] args = new object[parameters.Length];
			args[0] = value;
			parameters.Skip(1).Select(p => p.DefaultValue).ToArray().CopyTo(args, 1);
			return convertMethod.Invoke(args);
		}
		static object ConvertBySourceValueMethod(object value, Type targetType, object parameter, CultureInfo culture, string convertMethodName) {
			MethodInfo convertMethod = value.GetType().GetMethod(convertMethodName, new Type[] { });
			if(convertMethod == null)
				convertMethod = value.GetType().GetMethods().Where(c => c.Name == convertMethodName && c.GetParameters().Length > 0 && !c.GetParameters().Any(p => !p.IsOptional)).FirstOrDefault();
			if(convertMethod == null)
				throw new InvalidOperationException();
			ParameterInfo[] parameters = convertMethod.GetParameters();
			object[] args = parameters.Select(p => p.DefaultValue).ToArray();
			return convertMethod.Invoke(value, args);
		}
		static object ConvertByStaticMethod(object value, Type targetType, object parameter, CultureInfo culture, Type convertMethodOwner, string convertMethodName) {
			Tuple<MethodInfo, ConvertMethodSignature> convertMethod = GetMethod(convertMethodOwner.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == convertMethodName));
			if(convertMethod == null)
				throw new InvalidOperationException();
			ParameterInfo[] parameters = convertMethod.Item1.GetParameters();
			object[] args = new object[parameters.Length];
			convertMethod.Item2.AssignArgs(args, value, targetType, parameter, culture);
			for(int i = convertMethod.Item2.ParameterTypes.Length; i < args.Length; ++i)
				args[i] = parameters[i].DefaultValue;
			return convertMethod.Item1.Invoke(null, args);
		}
		static Tuple<MethodInfo, ConvertMethodSignature> GetMethod(IEnumerable<MethodInfo> methods) {
			foreach(var method in methods) {
				ParameterInfo[] parameters = method.GetParameters();
				var variantMatch = ConvertMethodSignatures.Where(v => Match(parameters, v.ParameterTypes)).FirstOrDefault();
				if(variantMatch != null) return new Tuple<MethodInfo, ConvertMethodSignature>(method, variantMatch);
			}
			return null;
		}
		static bool Match(ParameterInfo[] parameterInfo, Type[] parameterTypes) {
			if(parameterTypes.Length > parameterInfo.Length) return false;
			for(int i = parameterTypes.Length; i < parameterInfo.Length; ++i) {
				if(!parameterInfo[i].IsOptional) return false;
			}
			for(int i = 0; i < parameterTypes.Length; ++i) {
				if(!Match(parameterInfo[i].ParameterType, parameterTypes[i])) return false;
			}
			return true;
		}
		static bool Match(Type actual, Type expected) {
			return expected == null || actual == expected;
		}
	}
	public class EnumerableConverter : IValueConverter {
		public Type TargetItemType { get; set; }
		public IValueConverter ItemConverter { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			IEnumerable enumerable = value as IEnumerable;
			if(enumerable == null) return null;
			Type targetItemType = GetTargetItemType(targetType);
			Func<object, object> convertItem = x => ItemConverter == null ? x : ItemConverter.Convert(x, targetItemType, parameter, culture);
			IEnumerable convertedEnumerable = (IEnumerable)Activator.CreateInstance(typeof(EnumerableWrap<>).MakeGenericType(targetItemType), enumerable, convertItem);
			if(targetType == null || targetType.IsAssignableFrom(convertedEnumerable.GetType()))
				return convertedEnumerable;
			else if(targetType.IsInterface)
				return CreateList(targetType, targetItemType, convertedEnumerable);
			else if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>))
				return CreateReadOnlyCollection(targetType, targetItemType, convertedEnumerable);
			else
				return CreateCollection(targetType, targetItemType, convertedEnumerable);
		}
		Type GetTargetItemType(Type targetType) {
			if(TargetItemType != null)
				return TargetItemType;
			if(targetType == null)
				throw new InvalidOperationException();
			var interfaces = new Type[] { targetType }.Where(t => t.IsInterface).Concat(targetType.GetInterfaces());
			Type targetItemType = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(i => i.GetGenericArguments()[0]).FirstOrDefault();
			if(targetItemType == null)
				throw new InvalidOperationException();
			return targetItemType;
		}
		object CreateList(Type targetType, Type itemType, IEnumerable enumerable) {
			if(targetType != null && (targetType == typeof(IEnumerable) || targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
				return enumerable;
			Type collectionType = typeof(List<>).MakeGenericType(itemType);
			if(targetType != null && !targetType.IsAssignableFrom(collectionType))
				throw new NotSupportedCollectionException(targetType);
			return Activator.CreateInstance(collectionType, enumerable);
		}
		object CreateReadOnlyCollection(Type targetType, Type itemType, IEnumerable enumerable) {
			object list = CreateList(null, itemType, enumerable);
			return list.GetType().GetMethod("AsReadOnly").Invoke(list, new object[] { });
		}
		object CreateCollection(Type targetType, Type itemType, IEnumerable enumerable) {
			ConstructorInfo constructor1 = targetType.GetConstructor(new Type[] { typeof(IEnumerable) });
			if(constructor1 != null)
				return constructor1.Invoke(new object[] { enumerable });
			ConstructorInfo constructor2 = targetType.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(itemType) });
			if(constructor2 != null)
				return constructor2.Invoke(new object[] { enumerable });
			return CreateCollectionWithDefaultConstructor(targetType, itemType, enumerable);
		}
		object CreateCollectionWithDefaultConstructor(Type targetType, Type itemType, IEnumerable enumerable) {
			object collection;
			try {
				collection = Activator.CreateInstance(targetType);
			} catch (MissingMethodException e) {
				throw new NotSupportedCollectionException(targetType, null, e);
			}
			IList list = collection as IList;
			if(list != null) {
				foreach(var item in enumerable)
					list.Add(item);
				return list;
			}
			MethodInfo addMethod;
			Type genericListType = typeof(IList<>).MakeGenericType(itemType);
			if(targetType.GetInterfaces().Any(t => t == genericListType)) {
				addMethod = genericListType.GetMethod("Add", new Type[] { itemType });
			} else {
				addMethod = targetType.GetMethod("Add", new Type[] { itemType });
				if(addMethod == null)
					addMethod = targetType.GetMethods().Where(m => m.GetParameters().Length == 1).Where(m => m.GetParameters()[0].ParameterType.IsAssignableFrom(itemType)).FirstOrDefault();
			}
			if(addMethod == null)
				throw new NotSupportedCollectionException(targetType);
			foreach(var item in enumerable)
				addMethod.Invoke(collection, new object[] { item });
			return collection;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class NotSupportedCollectionException : Exception {
		public NotSupportedCollectionException(Type collectionType, string message = null, Exception innerException = null) : base(message, innerException) {
			CollectionType = collectionType;
		}
		public Type CollectionType { get; private set; }
	}
#endif
#if !NETFX_CORE && !FREE
	public class CriteriaOperatorConverter : IValueConverter {
		public string Expression { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(value == null) return null;
			var criteriaOperator = DevExpress.Data.Filtering.CriteriaOperator.Parse(Expression);
			var evaluator = new DevExpress.Data.Filtering.Helpers.ExpressionEvaluator(TypeDescriptor.GetProperties(value), criteriaOperator);
			return evaluator.Evaluate(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
#endif
	public class TypeCastConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return TypeCastHelper.TryCast(value, targetType);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return TypeCastHelper.TryCast(value, targetType);
		}
	}
	public class NumericToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return ConverterHelper.NumericToBoolean(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
	public class StringToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is string))
				return false;
			return !String.IsNullOrEmpty((string)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
	public class ObjectToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value != null ^ Inverse;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
		public bool Inverse { get; set; }
	}
	public class MapItem {
		public MapItem() { }
		public MapItem(object source, object target) {
			Source = source;
			Target = target;
		}
		public object Source { get; set; }
		public object Target { get; set; }
	}
#if !NETFX_CORE
	[ContentProperty("Map")]
#else
	[ContentProperty(Name="Map")]
#endif
	public class ObjectToObjectConverter : IValueConverter {
		public object DefaultSource { get; set; }
		public object DefaultTarget { get; set; }
		public ObservableCollection<MapItem> Map { get; set; }
		public ObjectToObjectConverter() {
			Map = new ObservableCollection<MapItem>();
		}
		static object ParseColor(string str) {
			var rgb = new Regex("^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$");
			var m = rgb.Match(str);
			if(m.Success) {
				return Color.FromArgb(255,
					System.Convert.ToByte(m.Groups[1].ToString(), 16),
					System.Convert.ToByte(m.Groups[2].ToString(), 16),
					System.Convert.ToByte(m.Groups[3].ToString(), 16));
			}
			var argb = new Regex("^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$");
			m = argb.Match(str);
			if(m.Success) {
				return Color.FromArgb(
					System.Convert.ToByte(m.Groups[1].ToString(), 16),
					System.Convert.ToByte(m.Groups[2].ToString(), 16),
					System.Convert.ToByte(m.Groups[3].ToString(), 16),
					System.Convert.ToByte(m.Groups[4].ToString(), 16));
			}
			return null;
		}
		object Coerce(object value, Type targetType) {
			if(value == null || targetType == value.GetType()) {
				return value;
			}
			var nullableType = Nullable.GetUnderlyingType(targetType);
			var coerced = CoerceNonNullable(value, nullableType ?? targetType);
			if(nullableType != null) {
				return Activator.CreateInstance(targetType, coerced);
			}
			return coerced;
		}
		bool IsImplicitXamlConvertion(Type valueType, Type targetType) {
			if(targetType == typeof(Thickness))
				return true;
			if(targetType == typeof(ImageSource) && (valueType == typeof(string) || valueType == typeof(Uri)))
				return true;
			return false;
		}
		object CoerceNonNullable(object value, Type targetType) {
			if (targetType == typeof(SolidColorBrush) ||
				targetType == typeof(Brush) ||
				targetType == typeof(Color)) {
				object res = null;
				if(value is Color) {
					res = (Color)value;
				}
				if (value is string) {
					res = ParseColor((string)value);
				}
				if(res != null) {
					if(targetType == typeof(Color))
						return res;
					return new SolidColorBrush { Color = (Color)res };
				}
			}
			if(targetType == typeof(string)) {
				return value.ToString();
			}
#if !NETFX_CORE
			if(targetType.IsEnum && value is string) {
#else
			if(targetType.IsEnum() && value is string) {
#endif
				return Enum.Parse(targetType, (string)value, false);
			}
			if(IsImplicitXamlConvertion(value.GetType(), targetType))
				return value;
			try {
				return System.Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
			} catch {
				return value;
			}
		}
		public static bool SafeCompare(object left, object right) {
			if(left == null) {
				if(right == null)
					return true;
				return right.Equals(left);
			}
			return left.Equals(right);
		}
		Func<MapItem, bool> MakeMapPredicate(Func<MapItem, object> selector, object value) {
			return mapItem => {
				object source = Coerce(selector(mapItem), (value ?? string.Empty).GetType());
				return SafeCompare(source, value);
			};
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			MapItem entry = Map.FirstOrDefault(MakeMapPredicate(item => item.Source, value));
			return Coerce(entry == null ? DefaultTarget : entry.Target, targetType);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			MapItem entry = Map.FirstOrDefault(MakeMapPredicate(item => item.Target, value));
			return Coerce(entry == null ? DefaultSource : entry.Source, targetType);
		}
	}
	public class BooleanToVisibilityConverter : IValueConverter {
		bool hiddenInsteadOfCollapsed;
		public bool Inverse { get; set; }
#if !NETFX_CORE
		[Obsolete, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HiddenInsteadCollapsed { get { return hiddenInsteadOfCollapsed; } set { hiddenInsteadOfCollapsed = value; } }
#endif
		public bool HiddenInsteadOfCollapsed { get { return hiddenInsteadOfCollapsed; } set { hiddenInsteadOfCollapsed = value; } }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool booleanValue = ConverterHelper.GetBooleanValue(value);
			return ConverterHelper.BooleanToVisibility(booleanValue, Inverse, HiddenInsteadOfCollapsed);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			bool booleanValue = (value is Visibility && (Visibility)value == Visibility.Visible) ^ Inverse;
#if !FREE
			if(targetType == typeof(DefaultBoolean))
				return ConverterHelper.ToDefaultBoolean(booleanValue);
#endif
			return booleanValue;
		}
	}
	public class NumericToVisibilityConverter : IValueConverter {
		bool hiddenInsteadOfCollapsed;
		public bool Inverse { get; set; }
		public bool HiddenInsteadOfCollapsed { get { return hiddenInsteadOfCollapsed; } set { hiddenInsteadOfCollapsed = value; } }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool boolean = ConverterHelper.NumericToBoolean(value);
			return ConverterHelper.BooleanToVisibility(boolean, Inverse, HiddenInsteadOfCollapsed);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
	}
	public class DefaultBooleanToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? booleanValue = ConverterHelper.GetNullableBooleanValue(value);
			if(targetType == typeof(bool)) return booleanValue ?? false;
#if !FREE
			if(targetType == typeof(DefaultBoolean)) return ConverterHelper.ToDefaultBoolean(booleanValue);
#endif
			return booleanValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? booleanValue = ConverterHelper.GetNullableBooleanValue(value);
			if(targetType == typeof(bool)) return booleanValue ?? false;
#if FREE
			return booleanValue;
#else
			if(targetType == typeof(bool?)) return booleanValue;
			return ConverterHelper.ToDefaultBoolean(booleanValue);
#endif
		}
	}
	public class BooleanNegationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool? booleanValue = ConverterHelper.GetNullableBooleanValue(value);
			if(booleanValue != null)
				booleanValue = !booleanValue.Value;
			if(targetType == typeof(bool)) return booleanValue ?? true;
#if !FREE
			if(targetType == typeof(DefaultBoolean)) return ConverterHelper.ToDefaultBoolean(booleanValue);
#endif
			return booleanValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return Convert(value, targetType, parameter, culture);
		}
	}
	public class FormatStringConverter : IValueConverter {
		public string FormatString { get; set; }
		public TextCaseFormat OutStringCaseFormat { get; set; }
		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return GetFormattedValue(FormatString, value, System.Globalization.CultureInfo.CurrentUICulture, OutStringCaseFormat);
		}
		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
		public static object GetFormattedValue(string formatString, object value, System.Globalization.CultureInfo culture) {
			string displayFormat = GetDisplayFormat(formatString);
			return string.IsNullOrEmpty(displayFormat) ? value : string.Format(culture, displayFormat, value);
		}
		public static object GetFormattedValue(string formatString, object value, System.Globalization.CultureInfo culture,
			TextCaseFormat outStringCaseFormat) {
			object o = GetFormattedValue(formatString, value, culture);
			if(o == null)
				return null;
			switch(outStringCaseFormat) {
				case TextCaseFormat.Lower:
					return o.ToString().ToLower();
				case TextCaseFormat.Upper:
					return o.ToString().ToUpper();
				default:
					return o.ToString();
			}
		}
		public static string GetDisplayFormat(string displayFormat) {
			if(string.IsNullOrEmpty(displayFormat))
				return string.Empty;
			string res = displayFormat;
			if(res.Contains("{"))
				return res;
			return string.Format("{{0:{0}}}", res);
		}
		public enum TextCaseFormat { Default, Lower, Upper }
	}
	public class BooleanToObjectConverter : IValueConverter {
		public object TrueValue { get; set; }
		public object FalseValue { get; set; }
		public object NullValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
#if !FREE
			if(value is DefaultBoolean) {
				var asDefaultBoolean = (DefaultBoolean)value;
				if(asDefaultBoolean == DefaultBoolean.True) {
					value = true;
				} else if(asDefaultBoolean == DefaultBoolean.False) {
					value = false;
				} else {
					value = null;
				}
			}
#endif
			if(value is bool?) {
				value = (bool?)value == true;
			}
			if(value == null) {
				return NullValue;
			}
			if(!(value is bool))
				return null;
			bool asBool = (bool)value;
			return asBool ? TrueValue : FalseValue;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	static class ConverterHelper {
		public static string[] GetParameters(object parameter) {
			string param = parameter as string;
			if(string.IsNullOrEmpty(param))
				return new string[0];
			return param.Split(';');
		}
		public static bool GetBooleanParameter(string[] parameters, string name) {
			foreach(string parameter in parameters) {
				if(string.Equals(parameter, name, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
		public static bool GetBooleanValue(object value) {
			if(value is bool)
				return (bool)value;
			if(value is bool?) {
				bool? nullable = (bool?)value;
				return nullable.HasValue ? nullable.Value : false;
			}
#if !FREE
			if(value is DefaultBoolean)
				return (DefaultBoolean)value == DefaultBoolean.True;
#endif
			return false;
		}
		public static bool? GetNullableBooleanValue(object value) {
			if(value is bool) return (bool)value;
			if(value is bool?) return (bool?)value;
#if !FREE
			if(value is DefaultBoolean) {
				DefaultBoolean defaultBoolean = (DefaultBoolean)value;
				return defaultBoolean == DefaultBoolean.Default ? (bool?)null : defaultBoolean == DefaultBoolean.True;
			}
#endif
			return null;
		}
#if !FREE
		public static DefaultBoolean ToDefaultBoolean(bool? booleanValue) {
			return booleanValue == null ? DefaultBoolean.Default : booleanValue.Value ? DefaultBoolean.True : DefaultBoolean.False;
		}
#endif
		public static bool NumericToBoolean(object value) {
			if(value == null)
				return false;
			try {
				var d = (double)System.Convert.ChangeType(value, typeof(double), null);
				return d != 0d;
			} catch (Exception) { }
			return false;
		}
		public static Visibility BooleanToVisibility(bool booleanValue, bool inverse, bool hiddenInsteadOfCollapsed) {
			return (booleanValue ^ inverse) ?
				Visibility.Visible :
#if !SILVERLIGHT && !NETFX_CORE
 (hiddenInsteadOfCollapsed ? Visibility.Hidden : Visibility.Collapsed);
#else
				Visibility.Collapsed;
#endif
		}
	}
}
