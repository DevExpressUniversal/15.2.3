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

using System.Collections;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.ComponentModel;
#if Win
using System.Drawing.Design;
#endif
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using DevExpress.Utils;
#else
#endif
namespace DevExpress.XtraVerticalGrid.Data {
	static class PropertyHelper {
		public const string RootPropertyName = "";
#if Win
		public static ICollection GetStandardValues(ITypeDescriptorContext context, out bool exclusive) {
			try {
				exclusive = AreExclusiveValues(context);
				return GetConverter(context).GetStandardValues(context);
			} catch {
				exclusive = false;
				return null;
			}
		}
		public static bool CanConvertString(ITypeDescriptorContext context) {
			TypeConverter converter = GetConverter(context);
			return converter.CanConvertFrom(context, typeof(string)) && converter.CanConvertTo(context, typeof(string));
		}
		public static string ConvertToString(ITypeDescriptorContext context, object value) {
			PropertyDescriptor pd = context.PropertyDescriptor;
			if(pd != null) {
				try {
					return Convert.ToString(GetConverter(context).ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string)));
				} catch {
					return String.Empty;
				}
			}
			return null;
		}
		public static bool AllowTextEdit(ITypeDescriptorContext context) {
			return CanConvertString(context) && !AreExclusiveValues(context);
		}
		public static bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return GetConverter(context).GetStandardValuesSupported(context);
		}
		public static bool CanConvertTo(ITypeDescriptorContext context, Type type) {
			return GetConverter(context).CanConvertTo(context, type);
		}
		public static object GetSingleValue(PropertyGridControl pGrid, string propertyName) {
			DescriptorContext context = pGrid.DataModeHelper.GetSingleDescriptorContext(propertyName);
			if(IsRoot(propertyName))
				return context.Instance;
			return GetValue(context.PropertyDescriptor, context.Instance);
		}
		public static PropertyDescriptorCollection GetProperties(object source, ITypeDescriptorContext context, Attribute[] attributes) {
			TypeConverter converter = GetConverter(context, source);
			if(!(converter == null || context == null) && converter.GetPropertiesSupported(context)) {
				try {
					return converter.GetProperties(context, source, attributes);
				} catch {
					return null;
				}
			} else
				return GetProperties(source, attributes);
		}
		public static bool GetPropertiesSupported(object source, ITypeDescriptorContext context) {
			TypeConverter converter = GetConverter(context, source);
			if(converter == null)
				return false;
			return converter.GetPropertiesSupported(context);
		}
		static PropertyDescriptorCollection GetProperties(object source, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(source, attributes, false);
		}
		public static bool NotifyParentProperty(ITypeDescriptorContext context) {
			if (context == null || context.PropertyDescriptor == null)
				return false;
			return ((NotifyParentPropertyAttribute)context.PropertyDescriptor.Attributes[typeof(NotifyParentPropertyAttribute)]).NotifyParent;
		}
		public static IComponentChangeService GetComponentChangeService(IServiceProvider provider) {
			return (provider == null) ? null : (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
		}
		public static bool IsPassword(DescriptorContext context) {
			if(context == null || context.PropertyDescriptor == null)
				return false;
			PasswordPropertyTextAttribute attribute = context.PropertyDescriptor.Attributes[typeof(PasswordPropertyTextAttribute)] as PasswordPropertyTextAttribute;
			return attribute != null && attribute.Password;
		}
		public static bool NeedsCustomEditorButton(UITypeEditorEditStyle editStyle) {
			return editStyle == UITypeEditorEditStyle.DropDown || editStyle == UITypeEditorEditStyle.Modal;
		}
		public static void Reset(DescriptorContext context) {
			context.PropertyDescriptor.ResetValue(context.Instance);
		}
		static bool AreExclusiveValues(ITypeDescriptorContext context) {
			return GetConverter(context).GetStandardValuesExclusive(context);
		}
#endif
		public static bool ShouldSerializeValue(ITypeDescriptorContext context) {
			if (context == null || context.PropertyDescriptor == null || context.Instance == null)
				return true;
			return context.PropertyDescriptor.ShouldSerializeValue(context.Instance);
		}
		public static bool IsRoot(string propertyName) {
			return propertyName == RootPropertyName;
		}
		public static object GetValue(PropertyDescriptor pd, object component) {
			object value = null;
			if(component == null || pd == null)
				return value;
			try {
				component = GetPropertyOwner(pd, component);
				value = pd.GetValue(component);
			} catch(Exception e) {
				value = GetUnwindedException(e).Message;
			}
			return value;
		}
		public static object GetPropertyOwner(PropertyDescriptor pd, object component) {
			ICustomTypeDescriptor typeDescriptor = component as ICustomTypeDescriptor;
			if(typeDescriptor == null)
				return component;
			return component = typeDescriptor.GetPropertyOwner(pd);
		}
		public static bool AllowMerge(PropertyDescriptor descriptor) {
			if(descriptor == null)
				return false;
			MergablePropertyAttribute mergablePropertyAttribute = descriptor.Attributes[typeof(MergablePropertyAttribute)] as MergablePropertyAttribute;
			if(mergablePropertyAttribute == null)
				return MergablePropertyAttribute.Default.AllowMerge;
			return mergablePropertyAttribute.AllowMerge;
		}
		public static Exception GetUnwindedException(Exception e) {
			Exception result = e;
			if(e is System.Reflection.TargetInvocationException)
				result = e.InnerException;
			string message = result.Message;
			while(string.IsNullOrEmpty(message) && result.InnerException != null) {
				message = result.InnerException.Message;
				result = result.InnerException;
			}
			return result;
		}
		public static void ThrowUnwindedException(Exception e) {
			throw GetUnwindedException(e);
		}
		public static object TryConvertFromDifferentType(ITypeDescriptorContext context, object value) {
			if(value == null)
				return value;
			object result = value;
			Type valueType = value.GetType();
			if(valueType != GetPropertyDescriptor(context).PropertyType) {
				if(CanConvertFrom(context, valueType))
					result = ConvertFrom(context, value);
				else {
					if(valueType.IsPrimitive || value is IConvertible) {
						Type conversionType = GetPropertyDescriptor(context).PropertyType;
						Type underlyingType = GetUnderlyingType(conversionType, context);
						result = Convert.ChangeType(value, underlyingType != null ? underlyingType : conversionType, null);
					}
				}
			}
			return result;
		}
		static Type GetUnderlyingType(Type nullableType, ITypeDescriptorContext context) {
			if(!nullableType.IsGenericType) {
				return null;
			}
			return nullableType.GetGenericArguments()[0];
		}
		public static bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			return GetConverter(context).CanConvertFrom(context, type);
		}
		public static TypeConverter GetConverter(ITypeDescriptorContext context) {
			TypeConverter converter = GetPropertyDescriptor(context).Converter;
			if(converter == null) {
				object value = GetValue(GetPropertyDescriptor(context), context.Instance);
				converter = (value == null) ? TypeDescriptor.GetConverter(GetPropertyDescriptor(context).PropertyType) : TypeDescriptor.GetConverter(value);
			}
			return converter;
		}
		public static object ConvertFrom(ITypeDescriptorContext context, object value) {
			return GetConverter(context).ConvertFrom(context, CultureInfo.CurrentCulture, value);
		}
		public static object CreateInstance(object value, DescriptorContext context, DescriptorContext parentContext, Attribute[] attributes) {
			Hashtable values = GetSameLevelValues(context, parentContext.GetProperties(attributes));
			values[context.PropertyDescriptor.Name] = value;
			object newInstance = PropertyHelper.GetConverter(parentContext).CreateInstance(parentContext, values);
			parentContext.SetValue(newInstance);
			return newInstance;
		}
		public static TypeConverter GetConverter(ITypeDescriptorContext context, object source) {
			if(context == null)
				return null;
			bool IsRoot = GetPropertyDescriptor(context) == null;
			return IsRoot ? GetConverter(source) : GetConverter(context);
		}
		static TypeConverter GetConverter(object component) {
			if(component == null)
				return null;
			return TypeDescriptor.GetConverter(component);
		}
		static Hashtable GetSameLevelValues(ITypeDescriptorContext context, PropertyDescriptorCollection properties) {
			Hashtable values = new Hashtable();
			foreach(PropertyDescriptor descriptor in properties) {
				values[descriptor.Name] = PropertyHelper.GetValue(descriptor, context.Instance);
			}
			return values;
		}
#if SL
		static PropertyDescriptor GetPropertyDescriptor(ITypeDescriptorContext context) {
			DescriptorContext descriptorContext  = context as DescriptorContext;
			return descriptorContext.PropertyDescriptor;
		}
#else
		static PropertyDescriptor GetPropertyDescriptor(ITypeDescriptorContext context) {
			return context.PropertyDescriptor;
		}
#endif
	}
}
