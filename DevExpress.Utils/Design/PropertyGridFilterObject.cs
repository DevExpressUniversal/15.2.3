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
using System.ComponentModel;
using System.Runtime.Serialization;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.Design {
	public enum FilterObjectFilterPropertiesType { Include, Exclude }
	public class FilterObject : ICustomTypeDescriptor, IDXObjectWrapper {
		object sourceObject;
		object[] sampleObjects;
		string[] supportedProperties;
		ArrayList properties;
		ArrayList events;
		FilterObjectFilterPropertiesType filterPropertiesType = FilterObjectFilterPropertiesType.Include;
		public FilterObject(object sourceObject, string[] supportedProperties) : this(sourceObject, supportedProperties, FilterObjectFilterPropertiesType.Include) { }
		public FilterObject(object sourceObject, string[] supportedProperties, FilterObjectFilterPropertiesType filterPropertiesType) : this(sourceObject, null, supportedProperties, filterPropertiesType) {}
		public FilterObject(object sourceObject, object[] sampleObjects, string[] supportedProperties) : this(sourceObject, sampleObjects, supportedProperties, FilterObjectFilterPropertiesType.Include) { }
		public FilterObject(object sourceObject, object[] sampleObjects, string[] supportedProperties, FilterObjectFilterPropertiesType filterPropertiesType) {
			this.sourceObject = sourceObject;
			this.sampleObjects = sampleObjects;
			this.supportedProperties = supportedProperties != null ? supportedProperties : new string[0];
			this.properties = new ArrayList();
			this.events = new ArrayList();
			this.filterPropertiesType = filterPropertiesType;
			CreatePropertyDescriptors();
			CreateEventDescriptors();
		}
		public object SourceObject { get { return sourceObject; } }
		public object[] SampleObjects { get {	return sampleObjects; } }
		public PropertyDescriptorCollection Properties {
			get {
				return new PropertyDescriptorCollection(properties.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
			}
		}
		public EventDescriptorCollection Events {
			get {
				return new EventDescriptorCollection(events.ToArray(typeof(EventDescriptor)) as EventDescriptor[]);
			}
		}
		public int CustomPropertiesCount { get { return supportedProperties.Length; } }
		public string[] SupportedProperties { get { return supportedProperties; } }
		public override string ToString() { return CustomPropertiesCount == 0 ? SourceObject.ToString() : String.Empty;  }
		protected virtual bool IsNestedPropertyDescriptor(PropertyDescriptor pd) {
			if(pd.SerializationVisibility == DesignerSerializationVisibility.Content) return true;
			DevExpress.Utils.Serializing.XtraSerializableProperty attr = pd.Attributes[typeof(DevExpress.Utils.Serializing.XtraSerializableProperty)] as DevExpress.Utils.Serializing.XtraSerializableProperty;
			return attr != null && attr.Visibility == Serializing.XtraSerializationVisibility.Content;
		}
		void CreatePropertyDescriptors() {
			PropertyDescriptorCollection collection =  TypeDescriptor.GetProperties(SourceObject, false);
			for(int i = 0; i < collection.Count; i ++) {
				if(IsPropertySupported(collection[i])) {
					if(IsNestedPropertyDescriptor(collection[i])) {
						NestedObjectPropertyDescriptor nested = CreateNestedDescriptor(collection[i]);
						if(nested.PropertiesCount == 0) continue;
						properties.Add(nested);
					} else
						properties.Add(new FilterObjectPropertyDescriptor(sourceObject, SampleObjects, collection[i]));
				}
			}
		}
		protected virtual NestedObjectPropertyDescriptor CreateNestedDescriptor(PropertyDescriptor propertyDescriptor) {
			return new NestedObjectPropertyDescriptor(sourceObject, SampleObjects, propertyDescriptor, GetChildProperties(propertyDescriptor));
		}
		void CreateEventDescriptors() {
			EventDescriptorCollection collection =  TypeDescriptor.GetEvents(SourceObject);
			for(int i = 0; i < collection.Count; i ++) {
				if(IsEventSupported(collection[i])) {
					events.Add(new FilterObjectEventDescriptor(sourceObject, collection[i]));
				}
			}
		}
		protected virtual bool IsSupportPropertyDescriptor(PropertyDescriptor propDescriptor) {
			if(!propDescriptor.IsBrowsable) 
				return false;
			return true;
		}
		protected virtual bool IsPropertySupported(PropertyDescriptor propDescriptor) {
			if(!IsSupportPropertyDescriptor(propDescriptor)) return false;
			if(CustomPropertiesCount == 0) return true;
			string[] names = new string[this.supportedProperties.Length];
			for(int i = 0; i < names.Length; i ++) {
				string st = this.supportedProperties[i];
				if(st.IndexOf('.') > -1)
					names[i] = st.Substring(0, st.IndexOf('.'));
				else names[i] = st;
			}
			for(int i = 0; i < names.Length; i ++) {
				if(names[i] == propDescriptor.Name)
					return IsIncluded;
			}
			return !IsIncluded;
		}
		protected bool IsIncluded { get { return this.filterPropertiesType == FilterObjectFilterPropertiesType.Include; } }
		protected virtual string[] GetChildProperties(PropertyDescriptor propDescriptor) {
			ArrayList childs = new ArrayList();
			string name = propDescriptor.Name + '.';
			for(int i = 0; i < this.supportedProperties.Length; i ++) {
				string st = this.supportedProperties[i];
				if(st.IndexOf(name) == 0) 
					childs.Add(st.Substring(name.Length, st.Length - name.Length));
			}
			return (string[])childs.ToArray(typeof(string));
		}
		protected virtual bool IsEventSupported(EventDescriptor eventDescriptor) {
			if(!eventDescriptor.IsBrowsable) return false;
			for(int i = 0; i < this.supportedProperties.Length; i ++) {
				if(this.supportedProperties[i] == eventDescriptor.Name)
					return true;
			}
			return false;
		}
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(SourceObject, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(SourceObject, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter()  { return TypeDescriptor.GetConverter(SourceObject, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(SourceObject, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(SourceObject, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(SourceObject, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { 
			return Events;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return ((ICustomTypeDescriptor)this).GetEvents();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return SourceObject; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) { 
			return ((ICustomTypeDescriptor)this).GetProperties();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { 
			return Properties;
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(SourceObject, true);
		}
		object IDXObjectWrapper.SourceObject { get {	return SourceObject; } }
	}
	public class FilterObjectPropertyDescriptor : PropertyDescriptor {
		object sourceObject;
		object[] sampleObjects;
		PropertyDescriptor sourceDescriptor;
		public FilterObjectPropertyDescriptor(object sourceObject, object[] sampleObjects, PropertyDescriptor sourceDescriptor) : base(sourceDescriptor.Name, null) {
			this.sourceObject = sourceObject;
			this.sampleObjects = sampleObjects;
			this.sourceDescriptor = sourceDescriptor;
		}
		public object SourceObject { get { return sourceObject; } }
		public object[] SampleObjects { get {	return sampleObjects; } }
		public PropertyDescriptor SourceDescriptor { get { return sourceDescriptor; } }
		public override bool CanResetValue(object component) {
			return SourceDescriptor.CanResetValue(SourceObject);
		}
		public override object GetValue(object component) {
			return SourceDescriptor.GetValue(SourceObject);
		}
		public override void SetValue(object component, object val) {
			SourceDescriptor.SetValue(SourceObject, val);
			if(SampleObjects != null) {
				for(int i = 0; i < SampleObjects.Length; i ++)
					SourceDescriptor.SetValue(SampleObjects[i], val);
			}
		}
		public override TypeConverter Converter { 	get { return SourceDescriptor.Converter; } }
		public override string DisplayName { get { return SourceDescriptor.DisplayName; } }
		public override object GetEditor(Type editorBaseType) { 
			return SourceDescriptor.GetEditor(editorBaseType); 
		}
		public override bool IsReadOnly {get { return SourceDescriptor.IsReadOnly; } }
		public override string Name { get { return SourceDescriptor.Name; } }
		public override Type ComponentType { get { return SourceObject.GetType(); } }
		public override Type PropertyType { get { return SourceDescriptor.PropertyType; } }
		public override void ResetValue(object component) {
			sourceDescriptor.ResetValue(SourceObject);
			if(SampleObjects != null) {
				for(int i = 0; i < SampleObjects.Length; i ++)
					SourceDescriptor.ResetValue(SampleObjects[i]);
			}
		}
		public override AttributeCollection Attributes { get { return SourceDescriptor.Attributes; } }
		public override bool ShouldSerializeValue(object component) { 
			return SourceDescriptor.ShouldSerializeValue(GetComponent(component)); 
		}
		protected object GetComponent(object component) {
			FilterObject fo = component as FilterObject;
			if(fo != null) return fo.SourceObject;
			return component;
		}
	}
	public class FilterObjectEventDescriptor : EventDescriptor {
		object sourceObject;
		EventDescriptor sourceDescriptor;
		public FilterObjectEventDescriptor(object sourceObject, EventDescriptor sourceDescriptor) : base(sourceDescriptor.Name, null) {
			this.sourceObject = sourceObject;
			this.sourceDescriptor = sourceDescriptor;
		}
		public object SourceObject { get { return sourceObject; } }
		public EventDescriptor SourceDescriptor { get { return sourceDescriptor; } }
		public override void AddEventHandler(object component, Delegate value) {
			SourceDescriptor.AddEventHandler(SourceObject, value);
		}
		public override void RemoveEventHandler(object component, Delegate value) {
			SourceDescriptor.RemoveEventHandler(SourceObject, value);
		}
		public override bool IsMulticast { get {return SourceDescriptor.IsMulticast;} }
		public override string DisplayName { get { return SourceDescriptor.DisplayName; } }
		public override string Name { get { return SourceDescriptor.Name; } }
		public override Type ComponentType { get { return SourceObject.GetType(); } }
		public override Type EventType { get { return SourceDescriptor.EventType; } }
		public override AttributeCollection Attributes { get { return SourceDescriptor.Attributes; } }
	}
	public class NestedObjectPropertyDescriptor : FilterObjectPropertyDescriptor {
		FilterObject filter;
		public NestedObjectPropertyDescriptor(object sourceObject, object[] sampleObjects, PropertyDescriptor sourceDescriptor, string[] childProperties) 
			: base(sourceObject, sampleObjects, sourceDescriptor) {
				this.filter = CriateFilterObject(sourceObject, sampleObjects, sourceDescriptor, childProperties);
		}
		protected virtual FilterObject CriateFilterObject(object sourceObject, object[] sampleObjects, PropertyDescriptor sourceDescriptor, string[] childProperties) {
			return new FilterObject(sourceDescriptor.GetValue(sourceObject), GetSampleObjectValues(sourceDescriptor, sampleObjects), childProperties);
		}
		public NestedObjectPropertyDescriptor(FilterObject filter, object sourceObject, object[] sampleObjects, PropertyDescriptor sourceDescriptor, string[] childProperties)
			: base(sourceObject, sampleObjects, sourceDescriptor) {
			this.filter = filter;
		}
		public int PropertiesCount { get { return Filter.Properties.Count; } }
		public static object[] GetSampleObjectValues(PropertyDescriptor descriptor, object[] sampleObjects) {
			if(sampleObjects == null) return null;
			object[] objectsValues = new Object[sampleObjects.Length];
			for(int i = 0; i < objectsValues.Length; i ++) {
				objectsValues[i] = descriptor.GetValue(sampleObjects[i]);
			}
			return objectsValues;
		}
		public override bool CanResetValue(object component) {
			if(Filter.CustomPropertiesCount == 0) 
				return base.CanResetValue(component);
			PropertyDescriptorCollection properties = Filter.Properties;
			for(int i = 0; i < properties.Count; i ++)
				if(!properties[i].CanResetValue(component))				
					return false;
			return true;
		}
		public override object GetValue(object component) {
			return Filter;
		}
		public override void SetValue(object component, object val) {
		}
		public override bool IsReadOnly { get { return true; } }
		public override void ResetValue(object component) {
			if(Filter.CustomPropertiesCount == 0) {
				base.ResetValue(component);
				return;
			}
			PropertyDescriptorCollection properties = Filter.Properties;
			for(int i = 0; i < properties.Count; i ++) {
				properties[i].ResetValue(component);
			}
		}
		public FilterObject Filter { get { return filter; } }
	}
}
