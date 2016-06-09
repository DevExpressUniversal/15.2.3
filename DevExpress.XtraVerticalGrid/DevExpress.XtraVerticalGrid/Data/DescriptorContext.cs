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

using System.ComponentModel;
using System;
using System.ComponentModel.Design;
using System.Drawing.Design;
namespace DevExpress.XtraVerticalGrid.Data {
	public interface IPropertyDescriptorService {
		PropertyDescriptorCollection GetProperties(object source, ITypeDescriptorContext context, Attribute[] attributes);
	}
	public class DescriptorContext : ITypeDescriptorContext {
		static IPropertyDescriptorService DefaultPropertyDescriptorService = new NullPropertyDescriptorService();
		object obj;
		PropertyDescriptor prop;
		IServiceProvider serviceProvider;
		IComponentChangeService componentChangeService;
		bool? shouldSerializeValue = null;
		string fieldName;
		object value;
		bool hasValue = false;
		Lazy<bool> isGetPropertiesSupported;
		PropertyDescriptorCollection lastProperties;
		IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = PropertyHelper.GetComponentChangeService(this);
				return componentChangeService;
			}
		}
		IPropertyDescriptorService PropertyDescriptorService {
			get {
				IPropertyDescriptorService propertyDescriptorService = GetService(typeof(IPropertyDescriptorService)) as IPropertyDescriptorService;
				if(propertyDescriptorService != null) {
					return propertyDescriptorService;
				}
				return DefaultPropertyDescriptorService;
			}
		}
		public PropertyDescriptor PropertyDescriptor { get { return prop; } }
		public object Instance { get { return obj; } }
		public string FieldName { get { return fieldName; } set { fieldName = value; } }
		public string ParentFieldName { get { return FieldNameHelper.GetParentFieldName(FieldName); } }
		public DescriptorContext ParentContext { get { return Grid != null ? Grid.DataModeHelper.GetDescriptorContext(ParentFieldName) : null; } }
		public bool ShouldSerializeValue {
			get {
				if(shouldSerializeValue == null) {
					shouldSerializeValue = GetShouldSerializeValue();
				}
				return shouldSerializeValue.Value;
			}
		}
		public object Value {
			get {
				if(!this.hasValue) {
					this.hasValue = true;
					value = GetValue();
				}
				return value;
			}
		}
		public bool IsGetPropertiesSupported { get { return this.isGetPropertiesSupported.Value; } }
		PropertyGridControl Grid { get { return serviceProvider as PropertyGridControl; } }
		PGridDataModeHelper DataHelper { get { return Grid.DataModeHelper; } }
		public DescriptorContext(object obj, PropertyDescriptor prop, IServiceProvider serviceProvider) {
			this.obj = obj;
			this.prop = prop;
			this.serviceProvider = serviceProvider;
			this.isGetPropertiesSupported = new Lazy<bool>(() => GetPropertiesSupported());
			this.isValueEditable = new Lazy<bool>(() => GetIsValueEditable());
		}
		protected virtual bool GetPropertiesSupported() {
			if (PropertyDescriptor == null)
				return true;
			if (TypeConverter == null)
				return false;
			return TypeConverter.GetPropertiesSupported(this);
		}
		PropertyDescriptorCollection GetCachedProperties(object source, Attribute[] attributes) {
			if(this.lastProperties != null)
				return this.lastProperties;
			this.lastProperties = PropertyDescriptorService.GetProperties(source, this, attributes);
			return this.lastProperties;
		}
		public PropertyDescriptor GetProperty(object source, Attribute[] attributes, string propertyName) {
			PropertyDescriptorCollection propCollection = GetProperties(source, attributes);
			if (propCollection != null) {
				return propCollection.Find(propertyName, false);
			}
			return null;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return GetProperties(Value, attributes);
		}
		public PropertyDescriptorCollection GetProperties(object source, Attribute[] attributes) {
			if(source == Instance)
				return GetCachedProperties(source, attributes);
			return PropertyDescriptorService.GetProperties(source, this, attributes);
		}
		public void SetValue(object settingValue) {
			if(PropertyDescriptor == null)
				return;
			object component = PropertyHelper.GetPropertyOwner(PropertyDescriptor, Instance);
			PropertyDescriptor.SetValue(component, settingValue);
			this.lastProperties = null;
			this.hasValue = false;
		}
		public bool GetShouldSerializeValue() {
			if(IsImmutable) {
				return PropertyHelper.ShouldSerializeValue(ParentContext);
			}
			return PropertyHelper.ShouldSerializeValue(this);
		}
		object GetValue() {
			if(PropertyDescriptor == null && FieldName == PropertyHelper.RootPropertyName) {
				return Instance;
			}
			return PropertyHelper.GetValue(PropertyDescriptor, Instance);
		}
		bool Equals(Attribute[] attribute1, Attribute[] attribute2) {
			if(attribute1 == null || attribute2 == null)
				return false;
			if(attribute1.Length != attribute2.Length)
				return false;
			for(int i = 0; i < attribute1.Length; i++) {
				if(!attribute1[i].Equals(attribute2[i]))
					return false;
			}
			return true;
		}
		object GetService(Type serviceType) {
			return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
		}
		#region ITypeDescriptorContext Members
		IContainer ITypeDescriptorContext.Container {
			get {
				IComponent component = obj as IComponent;
				if(component != null) {
					ISite site = component.Site;
					if(site != null) {
						return site.Container;
					}
				}
				return null;
			}
		}
		object ITypeDescriptorContext.Instance {
			get { return obj; }
		}
		void ITypeDescriptorContext.OnComponentChanged() {
			if(ComponentChangeService != null) {
				this.ComponentChangeService.OnComponentChanged(Instance, PropertyDescriptor, null, null);
			}
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			if(ComponentChangeService != null) {
				try {
					this.ComponentChangeService.OnComponentChanging(Instance, PropertyDescriptor);
				} catch(CheckoutException exception) {
					if(exception != CheckoutException.Canceled) {
						throw exception;
					}
					return false;
				}
			}
			return true;
		}
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor {
			get { return prop; }
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		#endregion
		bool IsSerializeContentsProp {
			get {
				if(Grid == null)
					return false;
				if(PropertyDescriptor == null)
					return false;
				return PropertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Content;
			}
		}
		public bool ShouldRenderReadOnly {
			get {
				if(IsImmutable) {
					return ParentContext == null ? true : ParentContext.ShouldRenderReadOnly;
				}
				if(ForceReadOnly) {
					return true;
				}
				if(IsPropertyReadOnly && ReadOnlyEditable) {
					if(PropertyType != null && (PropertyType.IsArray || PropertyType.IsPrimitive || PropertyType.IsValueType)) {
						return true;
					}
				}
				return ShouldRenderReadOnlyCore && !IsSerializeContentsProp && !NeedsCustomEditorButton;
			}
		}
		internal bool NeedsDropDownButton {
			get {
				if(PropertyDescriptor == null)
					return false;
				if(GetEditStyle(this) != System.Drawing.Design.UITypeEditorEditStyle.DropDown)
					return false;
				return true;
			}
		}
		bool NeedsCustomEditorButtonCore {
			get {
				if(PropertyDescriptor == null)
					return false;
				return GetEditStyle(this) == System.Drawing.Design.UITypeEditorEditStyle.Modal;
			}
		}
		internal bool NeedsCustomEditorButton {
			get {
				if(!NeedsCustomEditorButtonCore)
					return false;
				if(!IsValueEditable) {
					return ReadOnlyEditable;
				} else {
					return true;
				}
			}
		}
		bool ShouldRenderReadOnlyCore {
			get {
				if(ForceReadOnly)
					return true;
				if(!IsValueEditable)
					return !ReadOnlyEditable;
				else
					return false;
			}
		}
		bool IsPropertyReadOnly {
			get {
				if(IsImmutable)
					return ShouldRenderReadOnly;
				if(PropertyDescriptor == null)
					return true;
				return PropertyDescriptor.IsReadOnly;
			}
		}
		Lazy<bool> isValueEditable;
		public bool IsValueEditable {
			get {
				return isValueEditable.Value;
			}
		}
		bool GetIsValueEditable() {
			if (!IsPropertyReadOnly)
				return IsValueEditableCore;
			else
				return false;
		}
		bool IsValueEditableCore {
			get {
				if(!ForceReadOnly)
					return IsTextEditableCore || Enumerable || NeedsCustomEditorButtonCore || NeedsDropDownButton;
				else
					return false;
			}
		}
		TypeConverter TypeConverter { get { return PropertyHelper.GetConverter(this); } }
		bool IsTextEditable {
			get {
				if(IsValueEditable)
					return IsTextEditableCore;
				else {
					return false;
				}
			}
		}
		bool IsTextEditableCore {
			get {
				return TypeConverter.CanConvertFrom(this, typeof(string)) && !TypeConverter.GetStandardValuesExclusive(this);
			}
		}
		bool EnumerableCore {
			get { return TypeConverter.GetStandardValuesSupported(this); }
		}
		bool Enumerable {
			get {
				if(EnumerableCore)
					return !IsPropertyReadOnly;
				else
					return false;
			}
		}
		public bool IsImmutable {
			get {
				if(ParentContext == null)
					return false;
				return ParentContext.GetCreateInstanceSupported;
			}
		}
		bool IsImmutableObject {
			get {
				return TypeDescriptor.GetAttributes(PropertyType)[typeof(ImmutableObjectAttribute)].Equals((object)ImmutableObjectAttribute.Yes);
			}
		}
		bool ForceReadOnly {
			get {
				if(PropertyDescriptor == null)
					return true;
				if(Instance == null)
					return false;
				ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(Instance)[typeof(ReadOnlyAttribute)];
				bool forceReadOnly = readOnlyAttribute != null && !readOnlyAttribute.IsDefaultAttribute();
				return forceReadOnly;
			}
		}
		bool ReadOnlyEditable {
			get {
				if(PropertyDescriptor == null)
					return false;
				if (NeedsCustomEditorButtonCore) {
					return !IsImmutableObject && !GetCreateInstanceSupported && !PropertyType.IsValueType;
				}
				if (IsGetPropertiesSupported) {
					return !ForceReadOnly && !IsTextEditable && !IsImmutableObject;
				}
				return false;
			}
		}
		public bool GetCreateInstanceSupported {
			get {
				if(PropertyDescriptor == null)
					return false;
				return TypeConverter.GetCreateInstanceSupported(this);
			}
		}
		internal UITypeEditorEditStyle GetEditStyle(DescriptorContext context) {
#if DEBUGTEST
			if(context.PropertyDescriptor == null)
				throw new ArgumentNullException("PropertyDescriptor");
#endif
			UITypeEditor editor = Grid.GetUITypeEditor(context.PropertyDescriptor);
			return editor == null ? UITypeEditorEditStyle.None : editor.GetEditStyle(context);
		}
		Type PropertyType { get { return PropertyDescriptor.PropertyType; } }
	}
	public class NullPropertyDescriptorService : IPropertyDescriptorService {
		PropertyDescriptorCollection IPropertyDescriptorService.GetProperties(object source, ITypeDescriptorContext context, Attribute[] attributes) {
			return PropertyHelper.GetProperties(source, context, attributes);
		}
	}
}
