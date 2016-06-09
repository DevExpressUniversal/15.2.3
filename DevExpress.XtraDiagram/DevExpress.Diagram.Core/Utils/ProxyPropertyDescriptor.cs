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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Internal;
using DevExpress.Utils;
using System.Diagnostics;
namespace DevExpress.Diagram.Core {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class UseCollectionEditorAttribute : Attribute {
	}
	public static class PropertyDescriptorHelper {
		public static object GetValue(object x, string path) {
			return TypeDescriptor.GetProperties(x)[path].GetValue(x);
		}
		public static readonly IEqualityComparer<PropertyDescriptor> Comparer = new DelegateEqualityComparer<PropertyDescriptor>((a, b) => {
			return object.Equals(a, b) && a.ComponentType == b.ComponentType;
		});
		public static Type GetListGenericParameter(PropertyDescriptor property) {
			if(property.Attributes[typeof(UseCollectionEditorAttribute)] == null)
				return null;
			IEnumerable<Type> types = property.PropertyType.GetInterfaces();
			if(property.PropertyType.IsInterface) 
				types = types.Concat(property.PropertyType.Yield());
			var iListType = types.FirstOrDefault(x => x.IsGenericType && typeof(IList<>).IsAssignableFrom(x.GetGenericTypeDefinition()));
			return iListType.With(x => x.GetGenericArguments().Single());
		}
		public static bool IsComplexProperty(PropertyDescriptor property, object propertyOwner) {
			if(property.PropertyType.IsValueType || property.PropertyType.IsArray) return false;
			var converter = property.Converter;
			if(converter == null) return false;
			var context = new RootDescriptorContext(propertyOwner);
			if(!converter.GetPropertiesSupported(context) || converter.GetCreateInstanceSupported(context)) return false;
			return true;
		}
		public static PropertyDescriptor GetPropertyDescriptor<T, TProperty>(T component, Expression<Func<T, TProperty>> property) {
			return GetPropertyDescriptors(component)[ExpressionHelper.GetPropertyName(property)];
		}
		public static PropertyDescriptorCollection GetPropertyDescriptors(object component, ITypeDescriptorContext context = null, TypeConverter typeConverter = null, Attribute[] attributes = null) {
			context = context ?? new RootDescriptorContext(component);
			typeConverter = typeConverter ?? TypeDescriptor.GetConverter(component);
			return (typeConverter.GetPropertiesSupported(context) ? typeConverter.GetProperties(context, component) : null) ?? TypeDescriptor.GetProperties(component);
		}
		public static PropertyDescriptorCollection GetPropertyValueDescriptors(object component, PropertyDescriptor property) {
			return GetPropertyDescriptors(property.GetValue(component), new RootDescriptorContext(component), property.Converter, null);
		}
		sealed class RootDescriptorContext : ITypeDescriptorContext {
			readonly object instance;
			public RootDescriptorContext(object instance) {
				this.instance = instance;
			}
			IContainer ITypeDescriptorContext.Container { get { return null; } }
			object ITypeDescriptorContext.Instance { get { return instance; } }
			PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return null; } }
			object IServiceProvider.GetService(Type serviceType) { return null; }
			void ITypeDescriptorContext.OnComponentChanged() { }
			bool ITypeDescriptorContext.OnComponentChanging() { return false; }
		}
		public static ITypeDescriptorContext Change(this ITypeDescriptorContext context, object component, PropertyDescriptor property) {
			return context == null ? null : new ProxyTypeDescriptorContext<object, object>(context, model => {
				return component;
			}, property);
		}
		public static PropertyDescriptor AddAttributes(this PropertyDescriptor pd, params Attribute[] attributes) {
			return new PropertyDescriptorWrapper(pd, attributes);
		}
	}
	public sealed class ProxyTypeDescriptorContext<TComponent, TRealComponent> : ITypeDescriptorContext {
		readonly ITypeDescriptorContext context;
		readonly Func<TComponent, TRealComponent> getRealComponent;
		readonly PropertyDescriptor realPropertyDescriptor;
		public ProxyTypeDescriptorContext(ITypeDescriptorContext context, Func<TComponent, TRealComponent> getRealComponent, PropertyDescriptor realPropertyDescriptor) {
			Guard.ArgumentNotNull(context, "context");
			Debug.Assert(context.Instance is TComponent);
			this.context = context;
			this.getRealComponent = getRealComponent;
			this.realPropertyDescriptor = realPropertyDescriptor;
		}
		public IContainer Container { get { return context.Container; } }
		public object Instance { get { return getRealComponent((TComponent)context.Instance); } }
		public PropertyDescriptor PropertyDescriptor { get { return realPropertyDescriptor; } }
		public object GetService(Type serviceType) { return context.GetService(serviceType); }
		public void OnComponentChanged() { context.OnComponentChanged(); }
		public bool OnComponentChanging() { return context.OnComponentChanging(); }
	}
	public class PropertyDescriptorWrapper : PropertyDescriptor {
		public static Func<Type, string, IEnumerable<Attribute>> ExternalAndFluentAPIAttributesProvider { get; set; }
		static Attribute[] GetAttributes(Attribute[] attributes, Type attributesComponentType, string attributesPropertyName, Type attributesPropertyType) {
			if(attributesComponentType == null)
				return attributes;
			var externalAttributes = ExternalAndFluentAPIAttributesProvider.Return(x => x(attributesPropertyType, null).Concat(x(attributesComponentType, attributesPropertyName)), () => Enumerable.Empty<Attribute>());
			return attributes.Union(externalAttributes).ToArray();
		}
		protected readonly PropertyDescriptor baseDescriptor;
		public PropertyDescriptorWrapper(PropertyDescriptor baseDescriptor, bool includeExternalAndFluentAPIAttributes = false)
			: this(baseDescriptor, GetAttributes(baseDescriptor.Attributes.Cast<Attribute>().ToArray(), includeExternalAndFluentAPIAttributes ? baseDescriptor.ComponentType : null, includeExternalAndFluentAPIAttributes ? baseDescriptor.Name : null, includeExternalAndFluentAPIAttributes ? baseDescriptor.PropertyType : null)) { }
		public PropertyDescriptorWrapper(PropertyDescriptor baseDescriptor, Attribute[] attributes)
			: this(baseDescriptor, attributes, null, null, null) { }
		public PropertyDescriptorWrapper(PropertyDescriptor baseDescriptor, Attribute[] attributes, Type attributesComponentType, string attributesPropertyName, Type attributesPropertyType)
			: base(baseDescriptor.Name, GetAttributes(attributes, attributesComponentType, attributesPropertyName, attributesPropertyType)) {
			this.baseDescriptor = baseDescriptor;
		}
		public override bool Equals(object obj) {
			var other = obj as PropertyDescriptorWrapper;
			return other == this || (other != null && object.Equals(other.baseDescriptor, baseDescriptor));
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override Type ComponentType { get { return baseDescriptor.ComponentType; } }
		public override bool IsReadOnly { get { return baseDescriptor.IsReadOnly; } }
		public override Type PropertyType { get { return baseDescriptor.PropertyType; } }
		public override TypeConverter Converter { get { return baseDescriptor.Converter; } }
		public override bool CanResetValue(object component) { return baseDescriptor.CanResetValue(GetRealComponent(component)); }
		public override object GetValue(object component) { return baseDescriptor.GetValue(GetRealComponent(component)); }
		public override void ResetValue(object component) { baseDescriptor.ResetValue(GetRealComponent(component)); }
		public override void SetValue(object component, object value) { baseDescriptor.SetValue(GetRealComponent(component), value); }
		public override bool ShouldSerializeValue(object component) { return baseDescriptor.ShouldSerializeValue(GetRealComponent(component)); }
		public override bool SupportsChangeEvents { get { return baseDescriptor.SupportsChangeEvents; } }
		public override void AddValueChanged(object component, EventHandler handler) { baseDescriptor.AddValueChanged(GetRealComponent(component), handler); }
		public override void RemoveValueChanged(object component, EventHandler handler) { baseDescriptor.RemoveValueChanged(GetRealComponent(component), handler); }
		protected virtual object GetRealComponent(object component) {
			return component;
		}
	}
	[DebuggerDisplay("Content={Name}")]
	public class CombinedPropertyDescriptor : PropertyDescriptorWrapper {
		readonly PropertyDescriptor shouldSerializeDescriptor;
		public CombinedPropertyDescriptor(PropertyDescriptor baseDescriptor, PropertyDescriptor shouldSerializeDescriptor)
			: base(baseDescriptor) {
			if(shouldSerializeDescriptor == null) {
				throw new ArgumentNullException("shouldSerializeDescriptor");
			}
			this.shouldSerializeDescriptor = shouldSerializeDescriptor;
		}
		public override bool ShouldSerializeValue(object component) {
			return shouldSerializeDescriptor.ShouldSerializeValue(component);
		}
	}
	public abstract class TypeConverterWrapper : TypeConverter {
		protected abstract TypeConverter BaseConverter { get; }
		protected abstract ITypeDescriptorContext GetWrapperContext(ITypeDescriptorContext context);
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return BaseConverter.CanConvertFrom(GetWrapperContext(context), sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return BaseConverter.CanConvertTo(GetWrapperContext(context), destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return BaseConverter.ConvertFrom(GetWrapperContext(context), culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return BaseConverter.ConvertTo(GetWrapperContext(context), culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			return BaseConverter.CreateInstance(GetWrapperContext(context), propertyValues);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return BaseConverter.GetCreateInstanceSupported(GetWrapperContext(context));
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return BaseConverter.GetProperties(GetWrapperContext(context), value, attributes);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return BaseConverter.GetPropertiesSupported(GetWrapperContext(context));
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return BaseConverter.GetStandardValues(GetWrapperContext(context));
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return BaseConverter.GetStandardValuesExclusive(GetWrapperContext(context));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return BaseConverter.GetStandardValuesSupported(GetWrapperContext(context));
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return BaseConverter.IsValid(GetWrapperContext(context), value);
		}
	}
	public static class ProxyPropertyDescriptor {
		public static IEnumerable<PropertyDescriptor> GetProxyDescriptors<T>(T component, Func<T, object> getRealComponent, ITypeDescriptorContext realComponentContext = null, TypeConverter realComponentOwnerPropertyConverter = null, Attribute[] attributes = null, Func<PropertyDescriptor, PropertyDescriptor> preparePropertyDescriptor = null) {
			return GetProxyDescriptorsCore(component, getRealComponent, preparePropertyDescriptor ?? ((PropertyDescriptor x) => x), Create, realComponentContext, realComponentOwnerPropertyConverter, attributes);
		}
		public static IEnumerable<PropertyDescriptor> GetProxyDescriptorsCore<T>(T component, Func<T, object> getRealComponent, Func<PropertyDescriptor, PropertyDescriptor> preparePropertyDescriptor, Func<PropertyDescriptor, Func<T, object>, PropertyDescriptor> createMethod, ITypeDescriptorContext realComponentContext, TypeConverter realComponentOwnerPropertyConverter, Attribute[] attributes) {
			return GetPropertyDescriptors(component, getRealComponent, realComponentContext, realComponentOwnerPropertyConverter, attributes).Select(x => createMethod(preparePropertyDescriptor(x), getRealComponent));
		}
		public static PropertyDescriptor Create<T>(PropertyDescriptor property, Func<T, object> getRealComponent) {
			return new ProxyPropertyDescriptorCore<T>(property, getRealComponent);
		}
		static IEnumerable<PropertyDescriptor> GetPropertyDescriptors<T>(T component, Func<T, object> getRealComponent, ITypeDescriptorContext realComponentContext, TypeConverter realComponentOwnerPropertyConverter, Attribute[] attributes) {
			var realComponent = getRealComponent(component);
			return PropertyDescriptorHelper.GetPropertyDescriptors(realComponent, realComponentContext, realComponentOwnerPropertyConverter, attributes).Cast<PropertyDescriptor>();
		}
		public class ProxyPropertyDescriptorCore<T> : PropertyDescriptorWrapper {
			sealed class ProxyTypeConverter : TypeConverterWrapper {
				readonly ProxyPropertyDescriptorCore<T> descriptor;
				public ProxyTypeConverter(ProxyPropertyDescriptorCore<T> descriptor) {
					this.descriptor = descriptor;
				}
				protected override TypeConverter BaseConverter { get { return descriptor.baseDescriptor.Converter; } }
				protected override ITypeDescriptorContext GetWrapperContext(ITypeDescriptorContext context) {
					return context == null ? null : new ProxyTypeDescriptorContext<T, object>(context, descriptor.getRealComponent, descriptor.baseDescriptor);
				}
			}
			readonly Func<T, object> getRealComponent;
			public ProxyPropertyDescriptorCore(PropertyDescriptor property, Func<T, object> getRealComponent)
				: base(property, true) {
				this.getRealComponent = getRealComponent;
			}
			public override TypeConverter Converter { get { return new ProxyTypeConverter(this); } }
			protected sealed override object GetRealComponent(object component) {
				return getRealComponent((T)component);
			}
			public override string ToString() {
				return "ProxyPropertyDescriptor: " + Name;
			}
			#region ValueChanged
			class SubscribedEventHandler {
				readonly object component;
				public SubscribedEventHandler(object component) {
					this.component = component;
				}
				public EventHandler Handler { get; set; }
				public void Raise(object sender, EventArgs e) {
					Handler(component, e);
				}
			}
			Dictionary<Tuple<object, EventHandler>, SubscribedEventHandler> valueChangedHandlers;
			public sealed override void AddValueChanged(object component, EventHandler handler) {
				if(valueChangedHandlers == null)
					valueChangedHandlers = new Dictionary<Tuple<object, EventHandler>, SubscribedEventHandler>();
				var key = new Tuple<object, EventHandler>(component, handler);
				var subscribedHandler = valueChangedHandlers.GetOrAdd(key, () => {
					var baseHandler = new SubscribedEventHandler(component);
					base.AddValueChanged(component, baseHandler.Raise);
					return baseHandler;
				});
				subscribedHandler.Handler += handler;
			}
			public sealed override void RemoveValueChanged(object component, EventHandler handler) {
				if(valueChangedHandlers == null) return;
				var key = new Tuple<object, EventHandler>(component, handler);
				SubscribedEventHandler subscribedHandler;
				if(!valueChangedHandlers.TryGetValue(key, out subscribedHandler)) return;
				subscribedHandler.Handler -= handler;
				if(subscribedHandler.Handler == null) {
					base.RemoveValueChanged(component, subscribedHandler.Raise);
					valueChangedHandlers.Remove(key);
				}
			}
			#endregion
		}
	}
}
