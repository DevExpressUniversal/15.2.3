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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;
namespace DevExpress.Mvvm.DataAnnotations {
	public class TypeConverterBuilder<T, TProperty, TParentBuilder> : 
		NestedBuilderBase<T, TProperty, TypeConverterWrapperAttribute, TypeConverterBuilder<T, TProperty, TParentBuilder>, TParentBuilder>
		where TParentBuilder : PropertyMetadataBuilderBase<T, TProperty, TParentBuilder> {
		internal TypeConverterBuilder(TParentBuilder parent)
			: base(parent) {
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertToRule<TDestination>(Func<TProperty, TDestination> convertRule) {
			return ConvertToRule((value, culture, context) => convertRule(value));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertToRule<TDestination>(Func<TProperty, CultureInfo, TDestination> convertRule) {
			return ConvertToRule((value, culture, context) => convertRule(value, culture));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertToRule<TDestination>(Func<TProperty, CultureInfo, T, TDestination> convertRule) {
			return ChangeAttribute(x => x.AddConvertToRule(convertRule));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromRule<TSource>(Func<TSource, TProperty> convertRule) {
			return ConvertFromRule<TSource>((value, culture, context) => convertRule(value));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromRule<TSource>(Func<TSource, CultureInfo, TProperty> convertRule) {
			return ConvertFromRule<TSource>((value, culture, context) => convertRule(value, culture));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromRule<TSource>(Func<TSource, CultureInfo, T, TProperty> convertRule) {
			return ChangeAttribute(x => x.AddConvertFromRule(convertRule));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromNullRule(Func<TProperty> convertRule) {
			return ConvertFromNullRule((culture, context) => convertRule());
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromNullRule(Func<CultureInfo, TProperty> convertRule) {
			return ConvertFromNullRule((culture, context) => convertRule(culture));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> ConvertFromNullRule(Func<CultureInfo, T, TProperty> convertRule) {
			return ChangeAttribute(x => x.ConvertFromNullRule = (culture, context) => convertRule(culture, (T)context));
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> StandardValuesProvider(Func<IEnumerable<TProperty>> provider, bool? standardValuesExclusive = null) {
			return StandardValuesProvider(context => provider(), standardValuesExclusive);
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> PropertiesProvider(Func<IEnumerable<PropertyDescriptor>> provider) {
			return ChangeAttribute(x => x.PropertiesProvider = provider);
		}
		public TypeConverterBuilder<T, TProperty, TParentBuilder> StandardValuesProvider(Func<T, IEnumerable<TProperty>> provider, bool? standardValuesExclusive = null) {
			return ChangeAttribute(x => {
				x.StandardValuesProvider = context => provider((T)context).Cast<object>();
				x.StandardValuesExclusive = standardValuesExclusive;
			});
		}
		public TParentBuilder EndTypeConverter() {
			return EndCore();
		}
	}
}
namespace DevExpress.Mvvm.Native {
	public class TypeConverterWrapperAttribute : Attribute {
		readonly Dictionary<Type, Func<object, CultureInfo, object, object>> convertToRules = new Dictionary<Type, Func<object, CultureInfo, object, object>>();
		readonly Dictionary<Type, Func<object, CultureInfo, object, object>> convertFromRules = new Dictionary<Type, Func<object, CultureInfo, object, object>>();
		public Func<CultureInfo, object, object> ConvertFromNullRule { get; set; }
		public Func<object, IEnumerable<object>> StandardValuesProvider { get; set; }
		public Func<IEnumerable<PropertyDescriptor>> PropertiesProvider { get; set; }
		public bool? StandardValuesExclusive { get; set; }
		public Type BaseConverterType { get; set; }
		public void AddConvertToRule<T, TProperty, TDestination>(Func<TProperty, CultureInfo, T, TDestination> convertRule) {
			convertToRules[typeof(TDestination)] = (value, culture, context) => convertRule((TProperty)value, culture, (T)context);
		}
		public void AddConvertFromRule<T, TProperty, TSource>(Func<TSource, CultureInfo, T, TProperty> convertRule) {
			convertFromRules[typeof(TSource)] = (value, culture, context) => convertRule((TSource)value, culture, (T)context);
		}
		public Func<object, CultureInfo, object, object> GetConvertToRule(Type destinationType) {
			return convertToRules.GetValueOrDefault(destinationType);
		}
		public Func<object, CultureInfo, object, object> GetConvertFromRule(Type sourceType) {
			return convertFromRules.GetValueOrDefault(sourceType);
		}
	}
	public static class TypeConverterWrapperAttributeExtensions {
		public static TypeConverter WrapTypeConverter(this TypeConverterWrapperAttribute wrapper, TypeConverter baseConverter) {
			return wrapper.Return(x => new TypeConverterWrapper(wrapper, (x.BaseConverterType == null ? baseConverter : (TypeConverter)Activator.CreateInstance(x.BaseConverterType))), () => baseConverter);
		}
		internal static object GetInstance(this ITypeDescriptorContext context) {
			return context.With(x => x.Instance);
		}
	}
	public class TypeConverterWrapper : TypeConverter {
		readonly TypeConverterWrapperAttribute wrapper;
		readonly TypeConverter baseConverter;
		public TypeConverterWrapper(TypeConverterWrapperAttribute wrapper, TypeConverter baseConverter) {
			this.wrapper = wrapper;
			this.baseConverter = baseConverter ?? new TypeConverter();
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return wrapper.GetConvertToRule(destinationType) != null || baseConverter.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return wrapper.GetConvertToRule(destinationType).Return(x => x(value, culture, context.GetInstance()), () => baseConverter.ConvertTo(context, culture, value, destinationType));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return wrapper.GetConvertFromRule(sourceType) != null || baseConverter.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value != null) {
				var convertFromRule = wrapper.GetConvertFromRule(value.GetType());
				if(convertFromRule != null)
					return convertFromRule(value, culture, context.GetInstance());
			} else {
				if(wrapper.ConvertFromNullRule != null)
					return wrapper.ConvertFromNullRule(culture, context.GetInstance());
			}
			return baseConverter.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return wrapper.StandardValuesProvider.Return(x => new StandardValuesCollection(x(context.GetInstance()).ToArray()), () => baseConverter.GetStandardValues(context));
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return wrapper.StandardValuesExclusive.GetValueOrDefault(baseConverter.GetStandardValuesExclusive(context));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return wrapper.StandardValuesProvider != null ? true : baseConverter.GetStandardValuesSupported(context);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			return baseConverter.CreateInstance(context, propertyValues);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return baseConverter.GetCreateInstanceSupported(context);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return wrapper.PropertiesProvider.Return(x => new PropertyDescriptorCollection(x().ToArray()), () => baseConverter.GetProperties(context, value, attributes));
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return wrapper.PropertiesProvider != null ? true : baseConverter.GetPropertiesSupported(context);
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return baseConverter.IsValid(context, value);
		}
	}
}
