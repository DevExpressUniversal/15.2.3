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

using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm.UI.Native {
	public class DependencyPropertyRegistrator<T> {
		protected DependencyPropertyRegistrator() { }
		public static DependencyPropertyRegistrator<T> New() {
			return new DependencyPropertyRegistrator<T>();
		}
		public DependencyPropertyRegistrator<T> AddOwner<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, DependencyProperty sourceProperty) {
			return AddOwner(property, out propertyField, sourceProperty, null);
		}
		public DependencyPropertyRegistrator<T> AddOwner<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, DependencyProperty sourceProperty, TProperty defaultValue, Action<T> changedCallback = null) {
			return AddOwner(property, out propertyField, sourceProperty, CreateFrameworkMetadata(defaultValue, changedCallback));
		}
		public DependencyPropertyRegistrator<T> AddOwner<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, DependencyProperty sourceProperty, TProperty defaultValue, Action<T, DependencyPropertyChangedEventArgs> changedCallback) {
			return AddOwner(property, out propertyField, sourceProperty, CreateFrameworkMetadata(defaultValue, changedCallback));
		}
		public DependencyPropertyRegistrator<T> AddOwner<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, DependencyProperty sourceProperty, TProperty defaultValue, FrameworkPropertyMetadataOptions frameworkOptions) {
			return AddOwner(property, out propertyField, sourceProperty, CreateMetadata(defaultValue, (Action<T>)null, null, frameworkOptions));
		}
		DependencyPropertyRegistrator<T> AddOwner<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, DependencyProperty sourceProperty, PropertyMetadata metadata) {
			propertyField = sourceProperty.AddOwner(typeof(T), metadata);
			return this;
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, (Action<T>)null, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, (Action<T>)null, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, Action<T> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, Action<T, DependencyPropertyChangedEventArgs> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, Action<T> changedCallback, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, TProperty defaultValue, Action<T, DependencyPropertyChangedEventArgs> changedCallback, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return Register(property, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		DependencyPropertyRegistrator<T> Register<TProperty>(Expression<Func<T, TProperty>> property, out DependencyProperty propertyField, PropertyMetadata metadata) {
			propertyField = DependencyProperty.Register(GetPropertyName(property), typeof(TProperty), typeof(T), metadata);
			return this;
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, (Action<T>)null, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, (Action<T>)null, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<T> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<T, DependencyPropertyChangedEventArgs> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<T> changedCallback, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<T, DependencyPropertyChangedEventArgs> changedCallback, Func<T, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) {
			return RegisterReadOnly(property, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		DependencyPropertyRegistrator<T> RegisterReadOnly<TProperty>(Expression<Func<T, TProperty>> property, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, PropertyMetadata metadata) {
			propertyFieldKey = DependencyProperty.RegisterReadOnly(GetPropertyName(property), typeof(TProperty), typeof(T), metadata);
			propertyField = propertyFieldKey.DependencyProperty;
			return this;
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, (Action<TOwner>)null, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, (Action<TOwner>)null, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttached(getMethodName, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		DependencyPropertyRegistrator<T> RegisterAttached<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyProperty propertyField, PropertyMetadata metadata) where TOwner : DependencyObject {
			propertyField = DependencyProperty.RegisterAttached(GetPropertyNameFromMethod(getMethodName), typeof(TProperty), typeof(T), metadata);
			return this;
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, (Action<TOwner>)null, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, (Action<TOwner>)null, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, null, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		public DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions = null) where TOwner : DependencyObject {
			return RegisterAttachedReadOnly(getMethodName, out propertyFieldKey, out propertyField, CreateMetadata(defaultValue, changedCallback, coerceCallback, frameworkOptions));
		}
		DependencyPropertyRegistrator<T> RegisterAttachedReadOnly<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> getMethodName, out DependencyPropertyKey propertyFieldKey, out DependencyProperty propertyField, PropertyMetadata metadata) where TOwner : DependencyObject {
			propertyFieldKey = DependencyProperty.RegisterAttachedReadOnly(GetPropertyNameFromMethod(getMethodName), typeof(TProperty), typeof(T), metadata);
			propertyField = propertyFieldKey.DependencyProperty;
			return this;
		}
		public DependencyPropertyRegistrator<T> OverrideDefaultStyleKey() {
			DefaultStyleKeyHelper.DefaultStyleKeyProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(typeof(T)));
			return this;
		}
		public DependencyPropertyRegistrator<T> OverrideMetadata(DependencyProperty propertyField, Action<T> changedCallback = null) {
			propertyField.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(ToHandler(changedCallback)));
			return this;
		}
		public DependencyPropertyRegistrator<T> OverrideMetadata(DependencyProperty propertyField, object defaultValue, Action<T> changedCallback = null) {
			propertyField.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(defaultValue, ToHandler(changedCallback)));
			return this;
		}
		public DependencyPropertyRegistrator<T> OverrideMetadata<TProperty>(DependencyProperty propertyField, Action<T> changedCallback, Func<T, TProperty, TProperty> coerceCallback) {
			propertyField.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(ToHandler(changedCallback), ToCoerce(coerceCallback)));
			return this;
		}
		public DependencyPropertyRegistrator<T> FixPropertyValue(DependencyProperty propertyField, object value) {
			propertyField.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(value, null, (d, o) => value));
			return this;
		}
		static PropertyMetadata CreateFrameworkMetadata<TOwner, TProperty>(TProperty defaultValue, Action<TOwner> changedCallback) {
			var handler = ToHandler(changedCallback);
			return new FrameworkPropertyMetadata(defaultValue, handler);
		}
		static PropertyMetadata CreateFrameworkMetadata<TOwner, TProperty>(TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback) {
			var handler = ToHandler(changedCallback);
			return new FrameworkPropertyMetadata(defaultValue, handler);
		}
		static PropertyMetadata CreateMetadata<TOwner, TProperty>(TProperty defaultValue, Action<TOwner> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions) {
			var handler = ToHandler(changedCallback);
			var coerce = ToCoerce(coerceCallback);
			return frameworkOptions == null ? new PropertyMetadata(defaultValue, handler, coerce) : new FrameworkPropertyMetadata(defaultValue, frameworkOptions.Value, handler, coerce);
		}
		static PropertyMetadata CreateMetadata<TOwner, TProperty>(TProperty defaultValue, Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback, Func<TOwner, TProperty, TProperty> coerceCallback, FrameworkPropertyMetadataOptions? frameworkOptions) {
			var handler = ToHandler(changedCallback);
			var coerce = ToCoerce(coerceCallback);
			return frameworkOptions == null ? new PropertyMetadata(defaultValue, handler, coerce) : new FrameworkPropertyMetadata(defaultValue, frameworkOptions.Value, handler, coerce);
		}
		static PropertyChangedCallback ToHandler<TOwner>(Action<TOwner> changedCallback) {
			return (d, e) => changedCallback.Do(x => x((TOwner)(object)d));
		}
		static PropertyChangedCallback ToHandler<TOwner>(Action<TOwner, DependencyPropertyChangedEventArgs> changedCallback) {
			return (d, e) => changedCallback.Do(x => x((TOwner)(object)d, e));
		}
		static CoerceValueCallback ToCoerce<TOwner, TProperty>(Func<TOwner, TProperty, TProperty> coerceCallback) {
			if(coerceCallback == null)
				return null;
			return (d, o) => coerceCallback((TOwner)(object)d, (TProperty)o);
		}
		internal static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> property) {
			return (property.Body as MemberExpression).Member.Name;
		}
		static string GetPropertyNameFromMethod<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> method) {
			string name = (method.Body as MethodCallExpression).Method.Name;
			if(!name.StartsWith("Get"))
				throw new ArgumentException();
			return name.Substring(3);
		}
		class DefaultStyleKeyHelper : FrameworkElement {
			public static new DependencyProperty DefaultStyleKeyProperty { get { return FrameworkElement.DefaultStyleKeyProperty; } }
		}
	}
	public static class ServiceTemplatePropertyRegistrator {
		public static DependencyPropertyRegistrator<T> RegisterServiceTemplateProperty<T, TService>(this DependencyPropertyRegistrator<T> registrator, Expression<Func<T, DataTemplate>> property, out DependencyProperty propertyField, out Action<T, Action<TService>> serviceActionExecutor, Action<T> changedCallback = null) where TService : class {
			Action<T, Func<T, T>, Action<TService>> serviceActionExecutorCore;
			registrator.RegisterServiceTemplateProperty(property, out propertyField, out serviceActionExecutorCore, changedCallback);
			serviceActionExecutor = (owner, serviceAction) => serviceActionExecutorCore(owner, x => x, serviceAction);
			return registrator;
		}
		public static DependencyPropertyRegistrator<T> RegisterServiceTemplateProperty<T, TServiceOwner, TService>(this DependencyPropertyRegistrator<T> registrator, Expression<Func<T, DataTemplate>> property, out DependencyProperty propertyField, out Action<T, Func<T, TServiceOwner>, Action<TService>> serviceActionExecutor, Action<T> changedCallback = null) where TService : class {
			propertyField = AssignableServiceHelper2<T, TService>.RegisterServiceTemplateProperty(DependencyPropertyRegistrator<T>.GetPropertyName(property), changedCallback);
			var propertyFieldRef = propertyField;
			serviceActionExecutor = (owner, getServiceOwner, serviceAction) => {
				var propertyOwner = (DependencyObject)(object)owner;
				if(!IsInitialized(propertyOwner)) {
					var element = (ISupportInitialize)propertyOwner;
					element.BeginInit();
					element.EndInit();
				}
				AssignableServiceHelper2<T, TService>.DoServiceAction((DependencyObject)(object)getServiceOwner(owner), (DataTemplate)propertyOwner.GetValue(propertyFieldRef), serviceAction);
			};
			return registrator;
		}
		static bool IsInitialized(DependencyObject d) {
			var fe = d as FrameworkElement;
			if(fe != null) return fe.IsInitialized;
			var fce = d as FrameworkContentElement;
			if(fce != null) return fce.IsInitialized;
			return true;
		}
	}
	public abstract class Control<TControl> : Control where TControl : Control<TControl> {
		static Control() {
			DependencyPropertyRegistrator<TControl>.New()
				.OverrideDefaultStyleKey()
			;
		}
	}
}
