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
using System.Reflection;
using System.Windows.Data;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Utils;
namespace DevExpress.Xpf.Core.Native {
	public static class RenderTriggerHelper {
		public static readonly ReflectionHelper Helper = new ReflectionHelper();
		static readonly Type DefaultValueConverter;
		static readonly Type SystemConvertConverter;
		static RenderTriggerHelper() {
			DefaultValueConverter = typeof(IValueConverter).Assembly.GetType("MS.Internal.Data.DefaultValueConverter");
			SystemConvertConverter = typeof(IValueConverter).Assembly.GetType("MS.Internal.Data.SystemConvertConverter");
		}
		static TypeConverter GetConverter(Type targetType) {
			var getConverterMethod = Helper.GetStaticMethodHandler<Func<Type, TypeConverter>>(DefaultValueConverter, "GetConverter", BindingFlags.NonPublic | BindingFlags.Static);
			return getConverterMethod(targetType);
		}
		static bool CanConvertUsingSystemConverter(Type sourceType, Type targetType) {
			var getConverterMethod = Helper.GetStaticMethodHandler<Func<Type, Type, bool>>(SystemConvertConverter, "CanConvert", BindingFlags.Static | BindingFlags.Public);
			return getConverterMethod(sourceType, targetType);
		}
		static object Convert(Type sourceType, Type targetType, object value) {
			TypeConverter typeConverter;
			Type innerType;
			bool canConvertTo, canConvertFrom;
			bool sourceIsNullable = false;
			bool targetIsNullable = false;
			if (sourceType == targetType || targetType == typeof(object) || targetType.IsAssignableFrom(sourceType) || targetType.IsClass && value == null)
				return value;
			if (sourceType != typeof(object)) {
				if (CanConvertUsingSystemConverter(sourceType, targetType)) 
					return System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
				innerType = Nullable.GetUnderlyingType(sourceType);
				if (innerType != null) {
					sourceType = innerType;
					sourceIsNullable = true;
				}
				innerType = Nullable.GetUnderlyingType(targetType);
				if (innerType != null) {
					targetType = innerType;
					targetIsNullable = true;
				}
				if (sourceIsNullable || targetIsNullable) {
					return Convert(sourceType, targetType, value);
				}
				typeConverter = GetConverter(sourceType);
				canConvertTo = (typeConverter != null) && typeConverter.CanConvertTo(targetType);
				if (canConvertTo) 
					return typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, value, targetType);
				typeConverter = GetConverter(targetType);
				canConvertFrom = (typeConverter != null) && typeConverter.CanConvertFrom(sourceType);
				if (canConvertFrom)
					return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			if (targetType == typeof(string))
				return String.Format("{0}", value);
			typeConverter = GetConverter(targetType);
			return typeConverter.CanConvertFrom(sourceType) ? typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value) : value;
		}
		public static object GetConvertedValue(Type targetType, object value) {
			var sourceType = value == null ? typeof(object) : value.GetType();
			return Convert(sourceType, targetType, value);
		}
		public static object GetConvertedValue(object entity, string property, object value) {
			if (value == null)
				return null;
			Type propertyType = null;
			try {
				propertyType = Helper.GetPropertyType(entity, property);
			}
			catch (Exception ex) {
				ThrowNoProperty(entity, property, ex);
			}
			return GetConvertedValue(propertyType, value);
		}
		public static object GetValue(object entity, string property, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			Guard.ArgumentNotNull(entity, "entity");
			Guard.ArgumentNotNull(property, "property");
			try {
				return Helper.GetPropertyValue(entity, property, bindingFlags);
			}
			catch (Exception ex) {
				return ThrowNoProperty(entity, property, ex);
			}
		}
		public static void SetValue(object entity, string property, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			Guard.ArgumentNotNull(entity, "entity");
			Guard.ArgumentNotNull(property, "property");
			try {
				Helper.SetPropertyValue(entity, property, value, bindingFlags);
			}
			catch (Exception ex) {
				if (ex is NullReferenceException) {
					ThrowNoProperty(entity, property, ex);
				}
				else {
					throw new NotSupportedException(String.Format("Cannot set property '{0}' to an object {1}", property, entity), ex);
				}
			}
		}
		public static void SetConvertedValue(object entity, string property, object value) {
			SetValue(entity, property, GetConvertedValue(Helper.GetPropertyType(entity, property), value));
		}
		public static Type GetPropertyType(object entity, string property, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			Guard.ArgumentNotNull(entity, "entity");
			Guard.ArgumentNotNull(property, "property");
			return Helper.GetPropertyType(entity, property, bindingFlags);
		}
		static object ThrowNoProperty(object entity, string property, Exception ex) {
			throw new ArgumentException(String.Format("Property '{0}' does not exist on object {1}", property, entity), ex);
		}
	}
}
