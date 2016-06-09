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

using DevExpress.XtraVerticalGrid.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.Collections;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class NewInstanceConverter : TypeConverter {
		static Dictionary<Type, bool> CanCreateInstanceCache = new Dictionary<Type, bool>();
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			var types = new List<TypeInfo>();
			DescriptorContext descriptorContext = context as DescriptorContext;
			if (descriptorContext == null)
				return new StandardValuesCollection(types);
			IInstanceInitializer initializer = descriptorContext.InstanceInitializer;
			if (initializer != null && initializer.Types != null)
				types.AddRange(initializer.Types);
			else {
				Type type = GetDefaultType(descriptorContext);
				if (type == null)
					return new StandardValuesCollection(types);
				TypeInfo defaultType = new TypeInfo(type);
				if (CanCreateInstance(descriptorContext, defaultType)) {
					types.Add(defaultType);
				}
			}
			return new StandardValuesCollection(types);
		}
		Type GetDefaultType(ITypeDescriptorContext context) {
			if (context == null || context.PropertyDescriptor == null)
				return null;
			Type type = context.PropertyDescriptor.PropertyType;
			if (ListConverter.IsNewItemProperty(context.PropertyDescriptor)) {
				if (CanUseDefaultNewItemInitializer(type)) {
					return type;
				}
				return null;
			}
			else {
				if (type != null &&
					!type.IsAbstract &&
					type != typeof(object) &&
					type != typeof(System.Windows.DependencyObject) &&
					!IsSimple(type) &&
					HasDefaultConstructor(type))
					return type;
			}
			return null;
		}
		static bool IsSimple(Type type) {
			return type != null && (type.IsValueType || type.IsEnum || type.IsPrimitive);
		}
		static bool HasDefaultConstructor(Type type) {
			return type != null && type.GetConstructor(new Type[0]) != null;
		}
		public static bool CanUseDefaultNewItemInitializer(Type type) {
			return !(type == null || type.IsAbstract || type == typeof(object)) &&
				(IsSimple(type) ||
				type == typeof(string) ||
				(!IsSimple(type) && HasDefaultConstructor(type)));
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return GetBaseConverter(context).Return(x => x.CanConvertTo(context, destinationType), () => base.CanConvertTo(context, destinationType));
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			return GetBaseConverter(context).Return(x => x.ConvertTo(context, culture, value, destinationType), () => base.ConvertTo(context, culture, value, destinationType));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(TypeInfo)) {
				return (context as DescriptorContext).Return(x => x.StandardValues.Count > 0, () => false);
			}
			TypeConverter baseConverter = GetBaseConverter(context);
			if (baseConverter != null) {
				return baseConverter.CanConvertFrom(context, sourceType);
			}
			return false;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if (value != null && value is TypeInfo) {
				object newInstance = CreateInstance(context as DescriptorContext, value as TypeInfo);
				if (newInstance != null)
					return newInstance;
			}
			TypeConverter baseConverter = GetBaseConverter(context);
			if (baseConverter != null) {
				return baseConverter.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			TypeConverter baseConverter = GetBaseConverter(context);
			if (baseConverter == null)
				return false;
			return baseConverter.GetPropertiesSupported(context);
		}
		protected TypeConverter GetBaseConverter(ITypeDescriptorContext context) {
			var descriptorContext = context as DescriptorContext;
			if (descriptorContext == null)
				return null;
			TypeConverter baseConverter = descriptorContext.BaseConverter;
			if (baseConverter == null || baseConverter.GetType() == typeof(NewInstanceConverter))
				return null;
			return baseConverter;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			TypeConverter baseConverter = GetBaseConverter(context);
			if (baseConverter == null)
				return null;
			return baseConverter.GetProperties(context, value, attributes);
		}
		bool CanCreateInstanceInternal(DescriptorContext descriptorContext, TypeInfo typeInfo) {
			if (typeInfo != null && HasDefaultConstructor(typeInfo.Type))
				return true;
			try {
				CreateInstance(descriptorContext, typeInfo);
			} catch {
				return false;
			}
			return true;
		}
		bool CanCreateInstance(DescriptorContext descriptorContext, TypeInfo typeInfo) {
			if (typeInfo == null || typeInfo.Type == null)
				return false;
			bool canCreate;
			if (!CanCreateInstanceCache.TryGetValue(typeInfo.Type, out canCreate)) {
				canCreate = CanCreateInstanceInternal(descriptorContext, typeInfo);
				CanCreateInstanceCache.Add(typeInfo.Type, canCreate);
			}
			return canCreate;
		}
		object CreateInstance(DescriptorContext descriptorContext, TypeInfo typeInfo) {
			if (typeInfo == null || descriptorContext == null)
				return null;
			IInstanceInitializer initializer = descriptorContext.InstanceInitializer;
			object newInstance = initializer.Return(x => x.CreateInstance(typeInfo), null) ?? CreateInstanceInternal(typeInfo);			
			return newInstance;
		}
		static object CreateInstanceInternal(TypeInfo typeInfo) {
			if (TypeDescriptor.GetConverter(typeInfo).GetCreateInstanceSupported()) {
				Hashtable values = GetSameLevelValues(TypeDescriptor.GetProperties(typeInfo));
				object newInstance = TypeDescriptor.GetConverter(typeInfo).CreateInstance(values);
				return newInstance;
			}
			if (typeInfo.Type == typeof(string))
				return string.Empty;
			return TypeDescriptor.CreateInstance(null, typeInfo.Type, null, null);
		}
		static Hashtable GetSameLevelValues(PropertyDescriptorCollection properties) {
			Hashtable values = new Hashtable();
			foreach (PropertyDescriptor descriptor in properties) {
				object value = GetDefaultValue(descriptor);
				values[descriptor.Name] = value;
			}
			return values;
		}
		static object GetDefaultValue(PropertyDescriptor descriptor) {
			DefaultValueAttribute defaultAttribute = (DefaultValueAttribute)TypeDescriptor.GetAttributes(descriptor.PropertyType)[typeof(DefaultValueAttribute)];
			if (defaultAttribute != null)
				return defaultAttribute.Value;
			if (descriptor.PropertyType.IsPrimitive) {
				return TypeDescriptor.CreateInstance(null, descriptor.PropertyType, null, null);
			}
			return null;
		}
	}
}
