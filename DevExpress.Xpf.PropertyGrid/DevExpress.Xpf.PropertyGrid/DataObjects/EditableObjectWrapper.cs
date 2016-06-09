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

using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class EditableObjectWrapper : ICustomTypeDescriptor {
		PropertyDescriptorCollection internalProperties;
		protected internal object Source { get; private set; }
		protected internal Type SourceType { get; private set; }
		protected internal RowHandle Handle { get { return DataGenerator.DataView.GetHandleByFieldName(fieldName); } }
		string fieldName;
		protected internal TypeDescriptionProvider SourceTypeDescriptor { get; private set; }
		protected internal RowDataGenerator DataGenerator { get; private set; }
		PropertyDescriptorCollection InternalProperties {
			get {
				if (internalProperties == null) {
					internalProperties = Source.Return(x => PropertyDescriptorWrapper.GetWrappedProperties(TypeDescriptor.GetProperties(x), this), () => PropertyDescriptorCollection.Empty);
				}
				return internalProperties;
			}
			set { internalProperties = value; }
		}
		public EditableObjectWrapper(object source, RowDataGenerator dataGenerator, RowHandle handle) {
			Source = source;
			DataGenerator = dataGenerator;
			fieldName = dataGenerator.DataView.GetFieldNameByHandle(handle);
			if (source == null)
				return;
			SourceType = source.GetType();			
		}
		public AttributeCollection GetAttributes() {
			return SourceType.Return(x => TypeDescriptor.GetAttributes(x), () => AttributeCollection.Empty);
		}
		public string GetClassName() {
			return SourceType.With(x => TypeDescriptor.GetClassName(x));
		}
		public string GetComponentName() {
			return SourceType.With(x => TypeDescriptor.GetComponentName(x));
		}
		public TypeConverter GetConverter() {
			return SourceType.Return(x => new TypeConverterWrapper(TypeDescriptor.GetConverter(x), DataGenerator, Handle), () => null);
		}
		public EventDescriptor GetDefaultEvent() {
			return SourceType.Return(x => TypeDescriptor.GetDefaultEvent(x), () => null);
		}
		public object GetEditor(Type editorBaseType) {
			return SourceType.Return(x => TypeDescriptor.GetEditor(x, editorBaseType), () => null);
		}
		public EventDescriptorCollection GetEvents(Attribute[] attributes) {
			return SourceType.Return(x => TypeDescriptor.GetEvents(x, attributes), () => null);
		}
		public EventDescriptorCollection GetEvents() {
			return SourceType.Return(x => TypeDescriptor.GetEvents(x), () => null);
		}
		public PropertyDescriptor GetDefaultProperty() {
			return SourceType.Return(x => new PropertyDescriptorWrapper(TypeDescriptor.GetDefaultProperty(x), this), () => null);
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			var collection = Source.Return(x => TypeDescriptor.GetProperties(x, attributes), () => PropertyDescriptorCollection.Empty);
			return new PropertyDescriptorCollection(InternalProperties.OfType<PropertyDescriptorWrapper>().Where(wr => collection.Contains(wr.Source)).ToArray());
		}
		public PropertyDescriptorCollection GetProperties() {
			return InternalProperties;
		}
		public object GetPropertyOwner(PropertyDescriptor pd) {
			return this; 
		}		
	}
	public class PropertyDescriptorWrapper : PropertyDescriptor {
		int nameHash = -1;
		protected internal PropertyDescriptor Source { get; private set; }
		protected internal EditableObjectWrapper ObjectWrapper { get; private set; }
		public RowHandle Handle {
			get {
				RowHandle handle = null;
				var rootFieldName = ObjectWrapper.Handle == null ? "" : ObjectWrapper.DataGenerator.DataView.GetFieldNameByHandle(ObjectWrapper.Handle);
				var currentFieldName = rootFieldName.Return(x => x += String.Format(".{0}", Name), () => Name);
				handle = ObjectWrapper.DataGenerator.DataView.GetHandleByFieldName(currentFieldName);
				return handle;
			}
		}
		public PropertyDescriptorWrapper(PropertyDescriptor source, EditableObjectWrapper objectWrapper)
			: base(source) {
			Source = source;
			ObjectWrapper = objectWrapper;
			nameHash = Source.Return(x => x.Name.GetHashCode(), () => -1);
		}
		protected PropertyDescriptorWrapper(string name, Attribute[] attrs)
			: base(name, attrs) {
		}
		protected PropertyDescriptorWrapper(MemberDescriptor descr)
			: base(descr) {
		}
		protected PropertyDescriptorWrapper(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs) {
		}
		Dictionary<EventHandler, EventHandler> valueChangedHandlers = new Dictionary<EventHandler,EventHandler>();
		public override void AddValueChanged(object component, EventHandler handler) {			
			var customHandler = new EventHandler((d, e) => {
				handler(ObjectWrapper, e);				
			});
			valueChangedHandlers[handler] = customHandler;
			Source.Do(x => x.AddValueChanged(GetComponent(component), customHandler));
		}
		public override AttributeCollection Attributes {
			get { return Source.Return(x => x.Attributes, () => AttributeCollection.Empty); }
		}
		public override bool CanResetValue(object component) {
			return Source.CanResetValue(GetComponent(component));
		}
		public override string Category {
			get { return Source.Return(x => x.Category, () => null); }
		}
		public override Type ComponentType {
			get { return Source.Return(x => x.ComponentType, () => null); }
		}
		public override TypeConverter Converter {
			get { return Source.Return(x => new TypeConverterWrapper(x.Converter, ObjectWrapper.DataGenerator, Handle), () => null); }
		}
		public override string Description {
			get { return Source.Return(x => x.Description, () => null); }
		}
		public override bool DesignTimeOnly {
			get { return Source.Return(x => x.DesignTimeOnly, () => false); }
		}
		public override string DisplayName {
			get { return Source.Return(x => x.DisplayName, () => null); }
		}
		public override bool Equals(object obj) {
			return base.Equals(obj) || Source.Return(x => x.Equals(obj), () => false);
		}
		public override object GetEditor(Type editorBaseType) {
			return Source.Return(x => x.GetEditor(editorBaseType), () => null);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override object GetValue(object component) {
			return Source.Return(x => new EditableObjectWrapper(x.GetValue(GetComponent(component)), ObjectWrapper.DataGenerator, Handle), () => null);
		}
		public override bool IsBrowsable {
			get { return Source.Return(x => x.IsBrowsable, () => true); }
		}
		public override bool IsLocalizable {
			get { return Source.Return(x => x.IsLocalizable, () => true); }
		}
		public override bool IsReadOnly {
			get { return Source.Return(x => x.IsReadOnly, () => false); }
		}
		public override string Name {
			get { return Source.Return(x => x.Name, () => null); }
		}
		protected override int NameHashCode {
			get { return nameHash; }
		}
		public override Type PropertyType {
			get { return Source.Return(x => x.PropertyType, () => null); }
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			var customHandler = valueChangedHandlers[handler];
			Source.Do(x => x.RemoveValueChanged(GetComponent(component), customHandler));
		}
		public override void ResetValue(object component) {
			Source.Do(x => x.ResetValue(GetComponent(component)));
		}
		public override bool ShouldSerializeValue(object component) {
			return Source.Return(x => x.ShouldSerializeValue(GetComponent(component)), () => false);
		}
		public override bool SupportsChangeEvents {
			get { return Source.Return(x => x.SupportsChangeEvents, () => false); }
		}
		public override string ToString() {
			return Source.Return(x => x.ToString(), () => base.ToString());
		}
		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) {
			return Source.Return(x => GetWrappedProperties(x.GetChildProperties(instance, filter), ObjectWrapper), () => PropertyDescriptorCollection.Empty);
		}
		public override void SetValue(object component, object value) {
			if (Source == null || ObjectWrapper == null)
				return;
			if (ObjectWrapper.DataGenerator == null)
				return;
			ObjectWrapper.DataGenerator.SetValue(Handle, value);			
		}
		public static PropertyDescriptorCollection GetWrappedProperties(PropertyDescriptorCollection source, EditableObjectWrapper objectWrapper) {
			if (source == null || source == PropertyDescriptorCollection.Empty)
				return PropertyDescriptorCollection.Empty;
			return new PropertyDescriptorCollection(source.OfType<PropertyDescriptor>().Select(x => new PropertyDescriptorWrapper(x, objectWrapper)).ToArray());
		}
		protected static object GetComponent(object component) {
			if (component is EditableObjectWrapper)
				return ((EditableObjectWrapper)component).Source;
			return component;
		}
	}
	public class TypeConverterWrapper : TypeConverter {
		TypeConverter baseConverter;
		RowDataGenerator generator;
		RowHandle handle;
		public TypeConverterWrapper(TypeConverter baseConverter, RowDataGenerator generator, RowHandle handle) {
			this.baseConverter = baseConverter;
			this.generator = generator;
			this.handle = handle;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return baseConverter.CanConvertFrom(context.With(x=>new TypeDescriptorContextWrapper(x)), sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return baseConverter.CanConvertTo(context.With(x=>new TypeDescriptorContextWrapper(x)), destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return baseConverter.ConvertFrom(context.With(x=>new TypeDescriptorContextWrapper(x)), culture, GetActualValue(value));
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			return baseConverter.ConvertTo(context.With(x=>new TypeDescriptorContextWrapper(x)), culture, GetActualValue(value), destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
			return baseConverter.CreateInstance(context.With(x=>new TypeDescriptorContextWrapper(x)), propertyValues);
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return baseConverter.GetCreateInstanceSupported(context.With(x=>new TypeDescriptorContextWrapper(x)));
		}
		public override int GetHashCode() {
			return baseConverter.GetHashCode();
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return baseConverter.GetProperties(context.With(x=>new TypeDescriptorContextWrapper(x)), value, attributes);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return baseConverter.GetPropertiesSupported(context.With(x=>new TypeDescriptorContextWrapper(x)));
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return baseConverter.GetStandardValues(context.With(x=>new TypeDescriptorContextWrapper(x)));
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return baseConverter.GetStandardValuesExclusive(context.With(x=>new TypeDescriptorContextWrapper(x)));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return baseConverter.GetStandardValuesSupported(context.With(x=>new TypeDescriptorContextWrapper(x)));
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return baseConverter.IsValid(context.With(x=>new TypeDescriptorContextWrapper(x)), value);
		}
		public override string ToString() {
			return base.ToString();
		}
		object GetActualValue(object source) {
			if (source is EditableObjectWrapper) {
				return (source as EditableObjectWrapper).Source;
			}
			return source;
		}
		EditableObjectWrapper Wrap(object source) {
			return new EditableObjectWrapper(source, generator, handle);
		}
	}
	public class TypeDescriptorContextWrapper : ITypeDescriptorContext {
		ITypeDescriptorContext source;
		public TypeDescriptorContextWrapper(ITypeDescriptorContext source) {
			this.source = source;
		}
		#region ITypeDescriptorContext Members
		public IContainer Container {
			get { return source.Container; }
		}
		public object Instance {
			get { return source.Instance; }
		}
		public void OnComponentChanged() {
			source.OnComponentChanged();
		}
		public bool OnComponentChanging() {
			return source.OnComponentChanging();
		}
		public PropertyDescriptor PropertyDescriptor {
			get { return source.PropertyDescriptor; }
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return source.GetService(serviceType);
		}
		#endregion
	}
}
namespace DevExpress.Xpf.PropertyGrid {
	public class EditableObjectConverter : IValueConverter {
		public IValueConverter Converter { get; set; }
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is EditableObjectWrapper) {
				value = ((EditableObjectWrapper)value).Source;
			}
			if (Converter != null)
				return Converter.Convert(value, targetType, parameter, culture);
			return Convert(value, targetType, parameter, culture);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (Converter != null)
				return Converter.ConvertBack(value, targetType, parameter, culture);
			return ConvertBack(value, targetType, parameter, culture);
		}
		protected virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		protected virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
	public class EditableObjectConverterExtension : MarkupExtension {
		static EditableObjectConverter instance;
		public IValueConverter Converter { get; set; }
		public EditableObjectConverterExtension() {
			if (instance == null)
				instance = new EditableObjectConverter();
		}
		public EditableObjectConverterExtension(IValueConverter converter) {
			Converter = converter;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Converter == null ? instance : new EditableObjectConverter() { Converter = Converter };
		}
	}
}
