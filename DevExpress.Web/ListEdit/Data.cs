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
using DevExpress.Web.Internal;
using System.Collections.Generic;
using System.Collections;
using System.Data;
namespace DevExpress.Web.Internal {
	public interface IMulticolumnListEditDataSettings {
		string[] VisibleColumnFieldNames { get; }
		string ImageUrlField { get; }
		string TextField { get; }
		string ValueField { get; }
		bool DesignMode { get; }
		string GetTextFormatString();
	}
	public abstract class ListEditDataItemWrapper : ICustomTypeDescriptor {
		private IMulticolumnListEditDataSettings dataSettings;
		public ListEditDataItemWrapper(IMulticolumnListEditDataSettings dataSettings) {
			this.dataSettings = dataSettings;
		}
		public abstract object DataItem { get; }
		public virtual object Value {
			get {
				string actualValueFieldName = GetActualValueFieldName();
				return GetItemValue(actualValueFieldName);
			}
			set {
				string actualValueFieldName = GetActualValueFieldName();
				SetValue(actualValueFieldName, value);
			}
		}
		public string Text {
			get { return string.Format(DataSettings.GetTextFormatString(), GetVisibleColumnValues()); }
		}
		public virtual string ImageUrl {
			get { return GetImageUrl(DataSettings.ImageUrlField); }
			set { SetValue(DataSettings.ImageUrlField, value); }
		}
		protected IMulticolumnListEditDataSettings DataSettings {
			get { return dataSettings; }
		}
		public abstract object GetValue(string fieldName);
		public abstract void SetValue(string fieldName, object value);
		protected abstract PropertyDescriptorCollection GetPropertiesCore();
		protected abstract object GetItemValue(string actualValueFieldName);
		protected abstract string GetImageUrl(string actualValueFieldName);
		protected string GetActualValueFieldName() {
			return ListEditHelper.GetActualValueFieldName(DataSettings.ValueField, DataSettings.TextField);
		}
		public object[] GetVisibleColumnValues() {
			string[] visibleColumnFieldNames = DataSettings.VisibleColumnFieldNames;
			object[] values = new object[visibleColumnFieldNames.Length];
			for(int i = 0; i < visibleColumnFieldNames.Length; i++)
				values[i] = GetValue(visibleColumnFieldNames[i]);
			return values;
		}
		#region ICustomTypeDescriptor Members
		public AttributeCollection GetAttributes() {
			return new AttributeCollection(null);
		}
		public string GetClassName() {
			return null;
		}
		public string GetComponentName() {
			return null;
		}
		public TypeConverter GetConverter() {
			return null;
		}
		public EventDescriptor GetDefaultEvent() {
			return null;
		}
		public PropertyDescriptor GetDefaultProperty() {
			return null;
		}
		public object GetEditor(Type editorBaseType) {
			return null;
		}
		public EventDescriptorCollection GetEvents(Attribute[] attributes) {
			return new EventDescriptorCollection(null);
		}
		public EventDescriptorCollection GetEvents() {
			return new EventDescriptorCollection(null);
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return GetPropertiesCore();
		}
		public PropertyDescriptorCollection GetProperties() {
			return GetPropertiesCore();
		}
		public object GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
	}
	public class ListEditBoundDataItemWrapper : ListEditDataItemWrapper {
		PropertyDescriptorCollection properties;
		object dataItem;
		public ListEditBoundDataItemWrapper(IMulticolumnListEditDataSettings dataSettings, object dataItem)
			: base(dataSettings) {
			this.dataItem = dataItem;
		}
		public override object DataItem {
			get { return dataItem; }
		}
		protected PropertyDescriptorCollection Properties {
			get { return properties; }
		}
		public override object GetValue(string fieldName) {
			return DataUtils.GetFieldValue(DataItem, fieldName, true, DataSettings.DesignMode);
		}
		public override void SetValue(string fieldName, object value) {
			ReflectionUtils.SetPropertyValue(DataItem, fieldName, value);
		}
		protected override object GetItemValue(string actualValueFieldName) {
			return ListEditHelper.GetDataItemValue(DataItem, actualValueFieldName, DataSettings.ValueField, DataSettings.TextField, DataSettings.DesignMode);
		}
		protected override string GetImageUrl(string actualValueFieldName) {
			return ListEditHelper.GetDataItemImageUrl(DataItem, actualValueFieldName, DataSettings.ImageUrlField, DataSettings.DesignMode);
		}
		protected override PropertyDescriptorCollection GetPropertiesCore() {
			if(Properties == null) {
				ICustomTypeDescriptor customDescriptor = DataItem as ICustomTypeDescriptor;
				PropertyDescriptorCollection collection = customDescriptor != null ? customDescriptor.GetProperties()
					: TypeDescriptor.GetProperties(DataItem);
				CreatePropertyDescriptorCollection(collection);
			}
			return Properties;
		}
		protected void CreatePropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			int count = collection.Count;
			PropertyDescriptor[] props = new PropertyDescriptor[count];
			for(int i = 0; i < count; i++) {
				props[i] = new ListEditDataItemPropertyDescriptor(collection[i].Name);
			}
			this.properties = new PropertyDescriptorCollection(props);
		}
	}
	public class ListEditUnboundDataItemWrapper : ListEditDataItemWrapper {
		Dictionary<string, object> innerStorage = new Dictionary<string, object>();
		PropertyDescriptorCollection properties;
		public ListEditUnboundDataItemWrapper(IMulticolumnListEditDataSettings dataSettings)
			: base(dataSettings) {
		}
		public override object DataItem {
			get { return InnerStorage; }
		}
		protected Dictionary<string, object> InnerStorage {
			get { return innerStorage; }
		}
		public override object GetValue(string fieldName) {
			if(InnerStorage.ContainsKey(fieldName))
				return InnerStorage[fieldName];
			return null;
		}
		public override void SetValue(string fieldName, object value) {
			if(!InnerStorage.ContainsKey(fieldName))
				this.properties = null;
			InnerStorage[fieldName] = value;
		}
		protected override object GetItemValue(string actualValueFieldName) {
			return GetValue(actualValueFieldName);
		}
		protected override string GetImageUrl(string actualValueFieldName) {
			return GetValue(actualValueFieldName).ToString();
		}
		protected override PropertyDescriptorCollection GetPropertiesCore() {
			if(this.properties == null)
				CreatePropertyDescriptorCollection();
			return this.properties;
		}
		protected void CreatePropertyDescriptorCollection() {
			PropertyDescriptor[] props = new PropertyDescriptor[innerStorage.Count];
			int index = 0;
			foreach(string fieldName in innerStorage.Keys) {
				props[index] = new ListEditDataItemPropertyDescriptor(fieldName);
				index++;
			}
			this.properties = new PropertyDescriptorCollection(props);
		}
	}
	public class ListEditDataItemPropertyDescriptor : PropertyDescriptor {
		public ListEditDataItemPropertyDescriptor(string fieldName)
			: base(fieldName, null) {
		}
		public override bool CanResetValue(object component) { return false; }
		public override bool IsReadOnly { get { return false; } }
		public override Type ComponentType { get { return typeof(ListEditDataItemWrapper); } }
		public override Type PropertyType { get { return typeof(Object); } }
		public override object GetValue(object component) {
			return (component as ListEditDataItemWrapper).GetValue(Name);
		}
		public override void SetValue(object component, object value) {
			(component as ListEditDataItemWrapper).SetValue(Name, value);
		}
		public override void ResetValue(object component) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
}
