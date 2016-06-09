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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing {
	public class DataBrowserHelper : DataBrowserHelperBase {
		readonly static Attribute[] browsableAttributes = new Attribute[] { new BrowsableAttribute(true) };
		readonly bool suppressListFilling;
		public DataBrowserHelper()
			: this(false) {
		}
		public DataBrowserHelper(bool suppressListFilling) {
			this.suppressListFilling = suppressListFilling;
		}
		public override PropertyDescriptorCollection GetListItemProperties(object list, PropertyDescriptor[] listAccessors) {
#if !DXPORTABLE
			BindingSource bindingSource = list as BindingSource;
			if(suppressListFilling && bindingSource != null) {
				Type dataSourceType = bindingSource.DataSource as Type;
				if(dataSourceType != null && !typeof(ITypedList).IsAssignableFrom(dataSourceType) && !typeof(IListSource).IsAssignableFrom(dataSourceType)) {
					if(!string.IsNullOrEmpty(bindingSource.DataMember)) {
						PropertyDescriptor descriptor = TypeDescriptor.GetProperties(dataSourceType)[bindingSource.DataMember];
						if(descriptor != null)
							listAccessors = ArrayHelper.InsertItem<PropertyDescriptor>(listAccessors ?? new PropertyDescriptor[] { }, descriptor, 0);
					}
					return base.GetListItemProperties(dataSourceType, listAccessors);
				} else if(dataSourceType == null) {
					object target = GetList(bindingSource.DataSource);
					if(target is ITypedList && !string.IsNullOrEmpty(bindingSource.DataMember))
						listAccessors = ApplyNameToArray(target, bindingSource.DataMember, listAccessors);
					return base.GetListItemProperties(target, listAccessors);
				}
			}
#endif
			return base.GetListItemProperties(list, listAccessors);
		}
		PropertyDescriptor[] ApplyNameToArray(object component, string name, PropertyDescriptor[] listAccessors) {
			if(!string.IsNullOrEmpty(name)) {
				PropertyDescriptor descriptor = GetListItemProperties(component).Find(name, true);
				if(descriptor == null)
					throw new ArgumentException("name");
				return ArrayHelper.InsertItem<PropertyDescriptor>(listAccessors ?? new PropertyDescriptor[] { }, descriptor, 0); 
			}
			return listAccessors;
		}
		protected override object GetList(object list) {
			if(suppressListFilling && list is IListSource && list is IQueryable) {
				IList suppressedList = FakedListCreator.CreateGenericList(list);
				if(suppressedList != null) return suppressedList;
			}
			return base.GetList(list);
		}
		protected override bool IsCustomType(Type type) {
			return typeof(ICustomTypeDescriptor).IsAssignableFrom(type);
		}
		protected override PropertyDescriptorCollection GetProperties(object component) {
			return TypeDescriptor.GetProperties(component, browsableAttributes);
		}
		protected override PropertyDescriptorCollection GetProperties(Type componentType) {
			if(componentType.IsInterface()) {
				List<PropertyDescriptor> properties = GetInterfaceProperties(componentType);
				return new PropertyDescriptorCollection(properties.ToArray());
			}
			return TypeDescriptor.GetProperties(componentType, browsableAttributes);
		}
		List<PropertyDescriptor> GetInterfaceProperties(Type componentType) {
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			properties.AddRange(TypeDescriptor.GetProperties(componentType).Cast<PropertyDescriptor>());
			foreach(Type type in componentType.GetInterfaces())
				properties.AddRange(TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>());
			return properties;	
		}
	}
}
