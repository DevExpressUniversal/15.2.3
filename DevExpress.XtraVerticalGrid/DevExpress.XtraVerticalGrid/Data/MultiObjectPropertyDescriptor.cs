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
using System.Linq;
using System.Collections;
using System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using DevExpress.Utils;
#else
#endif
namespace DevExpress.XtraVerticalGrid.Data {
	class MultiObjectPropertyDescriptor : PropertyDescriptor {
		class MultiObjectAttributes : AttributeCollection {
			MultiObjectPropertyDescriptor owner;
			AttributeCollection[] parentAttributes;
			Hashtable attributes;
			public MultiObjectAttributes(MultiObjectPropertyDescriptor owner)
				: base(null) {
				this.owner = owner;
			}
			public override Attribute this[Type attributeType] {
				get {
					if(parentAttributes == null) {
						parentAttributes = new AttributeCollection[owner.descriptors.Length];
						for(int i = 0; i < parentAttributes.Length; i++)
							parentAttributes[i] = owner.descriptors[i].Attributes;
					}
					if(parentAttributes.Length == 0)
						return GetDefaultAttribute(attributeType);
					Attribute a;
					if(attributes != null) {
						a = (Attribute)attributes[attributeType];
						if(a != null)
							return a;
					}
					a = parentAttributes[0][attributeType];
					if(a == null)
						return null;
					for(int i = 1; i < parentAttributes.Length; i++) {
						if(!a.Equals(parentAttributes[i][attributeType])) {
							a = GetDefaultAttribute(attributeType);
							break;
						}
					}
					if(attributes == null)
						attributes = new Hashtable();
					attributes[attributeType] = a;
					return a;
				}
			}
		}
		PropertyDescriptor[] descriptors;
		object GetOwner(object[] list, int index) {
			object res = list[index];
			ICustomTypeDescriptor custom = res as ICustomTypeDescriptor;
			return custom == null ? res : custom.GetPropertyOwner(descriptors[index]);
		}
		protected override AttributeCollection CreateAttributeCollection() {
			return new MultiObjectAttributes(this);
		}
		public MultiObjectPropertyDescriptor(PropertyDescriptor[] descriptors)
			: base(descriptors[0].Name, null) {
			this.descriptors = descriptors;
		}
		public override object GetEditor(Type editorBaseType) {
			return descriptors[0].GetEditor(editorBaseType);
		}
		public override bool CanResetValue(object component) {
			object[] list = (object[])component;
			for(int i = 0; i < descriptors.Length; i++)
				if(!descriptors[i].CanResetValue(GetOwner(list, i)))
					return false;
			return true;
		}
		public override bool SupportsChangeEvents { get { return descriptors.Any(p => p.SupportsChangeEvents); } }
		public override void AddValueChanged(object component, EventHandler handler) {
			object[] list = (object[])component;
			for (int i = 0; i < descriptors.Length; i++) {
				if (!descriptors[i].SupportsChangeEvents)
					continue;
				descriptors[i].AddValueChanged(GetOwner(list, i), handler);
			}
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			object[] list = (object[])component;
			for (int i = 0; i < descriptors.Length; i++) {
				if (!descriptors[i].SupportsChangeEvents)
					continue;
				descriptors[i].RemoveValueChanged(GetOwner(list, 0), handler);
			}
		}
		public override Type ComponentType {
			get { return descriptors[0].ComponentType; }
		}
		public override object GetValue(object component) {
			object[] list = (object[])component;
			object res = PropertyHelper.GetValue(descriptors[0], GetOwner(list, 0));
			for(int i = 0; i < descriptors.Length; i++) {
				object temp = PropertyHelper.GetValue(descriptors[i], GetOwner(list, i));
				if(res != temp && res != null && !res.Equals(temp))
					return null;
			}
			return res;
		}
		public override bool IsReadOnly {
			get {
				for(int i = 0; i < descriptors.Length; i++)
					if(descriptors[i].IsReadOnly)
						return true;
				return false;
			}
		}
		public override Type PropertyType {
			get { return descriptors[0].PropertyType; }
		}
		public override TypeConverter Converter {
			get {
				return descriptors[0].Converter;
			}
		}
		public override string DisplayName {
			get {
				return descriptors[0].DisplayName;
			}
		}
		public override void ResetValue(object component) {
			object[] list = (object[])component;
			for(int i = 0; i < descriptors.Length; i++)
				descriptors[i].ResetValue(GetOwner(list, i));
		}
		public object[] GetValues(object[] components) {
			object[] list = new object[components.Length];
			for(int i = 0; i < descriptors.Length; i++)
				list[i] = PropertyHelper.GetValue(descriptors[i], GetOwner(components, i));
			return list;
		}
		public override void SetValue(object component, object value) {
			object[] list = (object[])component;
			for(int i = 0; i < descriptors.Length; i++)
				descriptors[i].SetValue(GetOwner(list, i), value);
		}
		public override bool ShouldSerializeValue(object component) {
			object[] list = (object[])component;
			for(int i = 0; i < descriptors.Length; i++)
				if(descriptors[i].ShouldSerializeValue(GetOwner(list, i)))
					return true;
			return false;
		}
		public static PropertyDescriptor GetMultiProp(DescriptorContext context, object[] components, string name, Attribute[] attrs) {
			PropertyDescriptor[] props = new PropertyDescriptor[components.Length];
			PropertyDescriptor firstDescriptor = context.GetProperty(components[0], attrs, name);
			if(!PropertyHelper.AllowMerge(firstDescriptor))
				return null;
			for(int i = 0; i < components.Length; i++) {
				if(i == 0) {
					props[i] = firstDescriptor;
				} else {
					PropertyDescriptor othersDescriptor = context.GetProperty(components[i], attrs, name);
					if(othersDescriptor == null || !firstDescriptor.Equals(othersDescriptor))
						return null;
					props[i] = othersDescriptor;
				}
			}
			return new MultiObjectPropertyDescriptor(props);
		}
		public PropertyDescriptor this[int index] {
			get { return descriptors[index]; }
		}
		public void SetValues(object components, object values) {
			object[] valuesArray = (object[])values;
			object[] componentsArray = (object[])components;
			if(valuesArray.Length != descriptors.Length || valuesArray.Length != componentsArray.Length)
				throw new ArgumentOutOfRangeException();
			for(int i = 0; i < descriptors.Length; i++)
				descriptors[i].SetValue(componentsArray[i], valuesArray[i]);
		}
		public override string Category { get { return this.descriptors[0].Category; } }
	}
}
