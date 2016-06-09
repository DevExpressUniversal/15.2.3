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
using System.Collections;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
namespace DevExpress.Utils.Base {
	public abstract class BaseProperties : BaseObject, IBaseProperties, IXtraSerializable, IXtraSerializableLayoutEx {
		protected Hashtable propertyBag = new Hashtable();
		protected Hashtable defaultValueBag = new Hashtable();
		protected override void OnDispose() {
			DestroyContentProperties();
		}
		public void Assign(IBaseProperties source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		public void Assign(IPropertiesProvider source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		public IBaseProperties Clone() {
			IBaseProperties clone = CloneCore();
			clone.BeginUpdate();
			clone.Assign(this);
			clone.CancelUpdate();
			return clone;
		}
		public bool ShouldSerialize() {
			return ShouldSerializeCore();
		}
		public void Reset() {
			BeginUpdate();
			ResetCore();
			EndUpdate();
		}
		protected virtual void PropertiesChanged(string property) {
			OnObjectChanged(new PropertyChangedEventArgs(property));
		}
		protected virtual void AssignCore(IPropertiesProvider source) {
			propertyBag.Clear();
			IDictionaryEnumerator enumerator = source.GetProperties();
			while(enumerator.MoveNext()) {
				propertyBag[enumerator.Key] = enumerator.Value;
			}
		}
		public IDictionaryEnumerator GetProperties() {
			return propertyBag.GetEnumerator();
		}
		protected abstract IBaseProperties CloneCore();
		protected virtual bool ShouldSerializeCore() {
			return propertyBag.Count > 0;
		}
		protected virtual void ResetCore() {
			propertyBag.Clear();
		}
		protected void Reset(string property) {
			propertyBag.Remove(property);
		}
		public override string ToString() {
			return DevExpress.Utils.Controls.OptionsHelper.GetObjectText(this, true);
		}
		public T GetValue<T>(string property) {
			return GetValueCore<T>(property);
		}
		public T GetDefaultValue<T>(string property) {
			return GetDefaultValueCore<T>(property);
		}
		public T GetContent<T>(string property) {
			return GetContentCore<T>(property);
		}
		public bool IsDefault(string property) {
			return !propertyBag.ContainsKey(property);
		}
		protected void SetDefaultValueCore<T>(string property, T value) {
			defaultValueBag[property] = value;
		}
		protected T GetValueCore<T>(string property) {
			object value = propertyBag[property];
			if(value != null) return (T)value;
			return GetDefaultValueCore<T>(property);
		}
		protected T GetDefaultValueCore<T>(string property) {
			object defaultValue = defaultValueBag[property];
			return (defaultValue != null) ? (T)defaultValue : default(T);
		}
		protected void SetValueCore<T>(string property, T value) {
			T oldValue = GetValueCore<T>(property);
			if(object.Equals(value, oldValue)) return;
			T defaultValue = GetDefaultValueCore<T>(property);
			if(object.Equals(value, defaultValue))
				propertyBag.Remove(property);
			else propertyBag[property] = value;
			PropertiesChanged(property);
		}
		Hashtable contentProperties = new Hashtable();
		public bool IsContentProperty(string property) {
			return contentProperties.ContainsKey(property);
		}
		protected T GetContentCore<T>(string propertyName) {
			return (T)contentProperties[propertyName];
		}
		protected void SetContentCore<T>(string propertyName, T content) {
			T oldContent = GetContentCore<T>(propertyName);
			if(object.Equals(content, oldContent)) return;
			contentProperties[propertyName] = content;
			PropertiesChanged(propertyName);
		}
		protected void InitContentPropertyCore(string propertyName, object content) {
			contentProperties[propertyName] = content;
		}
		protected void InitContentPropertyCore<T>(string propertyName, T content, Action<T> init) {
			contentProperties[propertyName] = content;
			if(init != null) init(content);
		}
		void DestroyContentProperties() {
			IDictionaryEnumerator enumerator = contentProperties.GetEnumerator();
			while(enumerator.MoveNext()) {
				IDisposable disposable = enumerator.Value as IDisposable;
				if(disposable != null)
					disposable.Dispose();
			}
			contentProperties.Clear();
		}
		#region IXtraSerializable Members
		int serializing, deserializing = 0;
		protected bool IsDeserializing {
			get { return deserializing > 0; }
		}
		protected bool IsSerializing {
			get { return serializing > 0; }
		}
		void IXtraSerializable.OnStartSerializing() { serializing++; }
		void IXtraSerializable.OnEndSerializing() { serializing--; }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) { deserializing++; }
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) { deserializing--; }
		#endregion
		#region IXtraSerializableLayoutEx
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return AllowProperty(options, propertyName, id);
		}
		protected virtual bool AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(IsSerializing)
				return !IsDefault(propertyName) || IsContentProperty(propertyName);
			return true;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			ResetProperties(options);
		}
		protected virtual void ResetProperties(OptionsLayoutBase options) { }
		#endregion
	}
	public abstract class BaseDefaultProperties : BaseProperties, IBaseDefaultProperties {
		IBaseProperties parentPropertiesCore;
		[Browsable(false)]
		public IBaseProperties ParentProperties {
			get { return parentPropertiesCore; }
		}
		public BaseDefaultProperties(IBaseProperties parentProperties) {
			parentPropertiesCore = parentProperties;
		}
		public bool IsDefault<T>(string property, T value) {
			return object.Equals(GetDefaultValueCore<T>(property), value);
		}
		Hashtable converters = new Hashtable();
		public T GetActualValueFromNullable<T>(string property) where T : struct {
			if(IsDefault(property) && ParentProperties != null)
				return ParentProperties.GetValue<T>(property);
			T? value = GetValueCore<T?>(property);
			Converter<T?, T> converter = converters[property] as Converter<T?, T>;
			return (converter != null) ? converter(value) : value.GetValueOrDefault();
		}
		public T GetActualValue<TDefault, T>(string property) {
			if(IsDefault(property) && ParentProperties != null)
				return ParentProperties.GetValue<T>(property);
			TDefault value = GetValueCore<TDefault>(property);
			Converter<TDefault, T> converter = converters[property] as Converter<TDefault, T>;
			return (converter != null) ? converter(value) : (T)((object)value);
		}
		public bool HasValue(string property) {
			if(!IsDefault(property)) return true;
			return (ParentProperties != null) && !ParentProperties.IsDefault(property);
		}
		protected void SetConverter<TDefault, T>(string property, Converter<TDefault, T> converter) {
			converters[property] = converter;
		}
		protected static Converter<DefaultBoolean, bool> GetDefaultBooleanConverter(bool defaultValue) {
			return new Converter<DefaultBoolean, bool>(delegate(DefaultBoolean value) {
				return value == DefaultBoolean.Default ? defaultValue : (value == DefaultBoolean.True);
			});
		}
		protected static Converter<T?, T> GetNullableValueConverter<T>(T defaultValue) where T : struct {
			return new Converter<T?, T>(delegate(T? value) {
				return value.GetValueOrDefault(defaultValue);
			});
		}
		public bool EnsureParentProperties(IBaseProperties parentProperties) {
			if(parentPropertiesCore == null)
				parentPropertiesCore = parentProperties;
			return parentPropertiesCore != null;
		}
	}
	public abstract class BasePropertiesProvider : IPropertiesProvider {
		Hashtable propertyBag = new Hashtable();
		protected void SetValueCore<T>(string property, T value) {
			propertyBag[property] = value;
		}
		protected void ClearValue(string property) {
			propertyBag.Remove(property);
		}
		protected T GetValueCore<T>(string property) {
			return (T)propertyBag[property];
		}
		public IDictionaryEnumerator GetProperties() {
			return propertyBag.GetEnumerator();
		}
		#region static
		static IDictionary<object, IPropertiesProvider> providersBag =
			new Dictionary<object, IPropertiesProvider>();
		public static void Attach(object key, IPropertiesProvider provider) {
			if(key != null) {
				if(key is IPropertiesProvider) return;
				if(provider != null)
					providersBag[key] = provider;
				else providersBag.Remove(key);
			}
		}
		public static void Detach(object key) {
			if(key != null)
				providersBag.Remove(key);
		}
		public static T GetProvider<T>(object key) where T : IPropertiesProvider {
			if(key is T) return (T)key;
			IPropertiesProvider result;
			return providersBag.TryGetValue(key, out result) ? (T)result : default(T);
		}
		#endregion static
	}
}
