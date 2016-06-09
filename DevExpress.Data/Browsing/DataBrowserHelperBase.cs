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
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using DevExpress.Data.Helpers;
using System.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.Browsing {
	[Obsolete("There is no need to use it")]
	public class BindingListReadOnlyAttribute : Attribute {
		public static readonly BindingListReadOnlyAttribute Instance = new BindingListReadOnlyAttribute();
	}
	public abstract class DataBrowserHelperBase {
		public virtual PropertyDescriptorCollection GetListItemProperties(object list, PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0)
				return GetListItemProperties(list);
			if(list is Type)
				return GetListItemPropertiesByType((Type)list, listAccessors);
			object target = GetList(list);
			if(target is ITypedList)
				return ((ITypedList)target).GetItemProperties(listAccessors);
			if(target is IEnumerable)
				return GetListItemPropertiesByEnumerable((IEnumerable)target, listAccessors);
			return GetListItemPropertiesByInstance(target, listAccessors, 0);
		}
		public PropertyDescriptorCollection GetListItemProperties(object list) {
			if(list == null)
				return new PropertyDescriptorCollection(null);
			if(list is Type)
				return GetListItemPropertiesByType((Type)list);
			object target = GetList(list);
			if(target is ITypedList)
				return ((ITypedList)target).GetItemProperties(null);
			if(target is IEnumerable) {
				return GetListItemPropertiesByEnumerable((IEnumerable)target);
			}
			return TypeDescriptor.GetProperties(target);
		}
		PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable enumerable) {
			PropertyDescriptorCollection properties = null;
			object firstItemByEnumerable = GetFirstItemByEnumerable(enumerable);
			Type type = enumerable.GetType();
			if(DevExpress.Data.Access.ExpandoPropertyDescriptor.IsDynamicType(type)) {
				return ListDataControllerHelper.GetExpandoObjectProperties(null, enumerable);
			}
			if(typeof(Array).IsAssignableFrom(type))
				properties = GetProperties(type.GetElementType());
			else {
				ITypedList list = enumerable as ITypedList;
				if(list != null)
					properties = list.GetItemProperties(null);
				else {
					PropertyInfo typedIndexer = GetTypedIndexer(type);
					if(typedIndexer != null && (firstItemByEnumerable == null || !IsCustomType(typedIndexer.PropertyType)))
						properties = GetProperties(typedIndexer.PropertyType);
				}
			}
			if(properties != null && properties.Count > 0)
				return properties;
			if(!(enumerable is string)) {
				if(firstItemByEnumerable == null)
					return new PropertyDescriptorCollection(null);
				properties = GetProperties(firstItemByEnumerable);
				if(enumerable is IList || (properties != null && properties.Count != 0))
					return properties;
			}
			return GetProperties(enumerable);
		}
		protected abstract bool IsCustomType(Type type);
		protected abstract PropertyDescriptorCollection GetProperties(object component);
		protected abstract PropertyDescriptorCollection GetProperties(Type componentType);
		static object GetFirstItemByEnumerable(IEnumerable enumerable) {
			IList list = ForceList(enumerable);
			if(list != null)
				return list.Count > 0 ? list[0] : null;
			try {
				return GetFirstItemByEnumerator(enumerable.GetEnumerator());
			} catch(NotSupportedException) {
			} catch(NotImplementedException) {
			}
			return null;
		}
		static object GetFirstItemByEnumerator(IEnumerator enumerator) {
			enumerator.Reset();
			object result = enumerator.MoveNext() ? enumerator.Current : null;
			enumerator.Reset();
			return result;
		}
		static IList ForceList(object enumerable) {
			return enumerable is IList ? (IList)enumerable : FakedListCreator.CreateFakedList(enumerable);
		} 
		static PropertyInfo GetTypedIndexer(Type type) {
			if(!ListTypeHelper.IsListType(type) && !typeof(ITypedList).IsAssignableFrom(type)) {
				return null;
			}
			PropertyInfo info = null;
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			for(int i = 0; i < properties.Length; i++) {
				if(properties[i].GetIndexParameters().Length > 0 && properties[i].PropertyType != typeof(object)) {
					info = properties[i];
					if(info.Name == "Item") {
						return info;
					}
				}
			}
			return info;
		}
		PropertyDescriptorCollection GetListItemPropertiesByType(Type type) {
			return GetProperties(GetListItemType(type));
		}
		static Type GetListItemType(object list) {
			if(list == null) {
				return null;
			}
			Type propertyType = null;
			Type listType = list is Type ? list as Type : list.GetType();
			object listObj = list is Type ? null : list;
			if(typeof(Array).IsAssignableFrom(listType)) {
				return listType.GetElementType();
			}
			PropertyInfo typedIndexer = GetTypedIndexer(listType);
			if(typedIndexer != null) {
				propertyType = typedIndexer.PropertyType;
			} else if(listObj is IEnumerable) {
				propertyType = GetListItemTypeByEnumerable(listObj as IEnumerable);
			}
			if((propertyType == null || propertyType == typeof(object)) && typeof(IEnumerable).IsAssignableFrom(listType)) {
				Type[] genericArguments = ListTypeHelper.FindGenericArguments(listType, delegate(Type item) {
					return typeof(IEnumerable<>) == item;
				});
				propertyType = genericArguments != null ? genericArguments[0] : typeof(object);
			}
			if(propertyType == null) {
				propertyType = listType;
			}
			return propertyType;
		}
		static Type GetListItemTypeByEnumerable(IEnumerable iEnumerable) {
			object firstItemByEnumerable = GetFirstItemByEnumerable(iEnumerable);
			if(firstItemByEnumerable == null) {
				return typeof(object);
			}
			return firstItemByEnumerable.GetType();
		}
		public PropertyDescriptorCollection GetListItemPropertiesByType(Type type, PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0) {
				return GetListItemPropertiesByType(type);
			}
			return GetListItemPropertiesByType(type, listAccessors, 0);
		}
		PropertyDescriptorCollection GetListItemPropertiesByType(Type type, PropertyDescriptor[] listAccessors, int startIndex) {
			Type propertyType = listAccessors[startIndex].PropertyType;
			startIndex++;
			if(startIndex >= listAccessors.Length) {
				return GetListItemPropertiesByType(propertyType);
			}
			return GetListItemPropertiesByType(propertyType, listAccessors, startIndex);
		}
		PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable enumerable, PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0) {
				return GetListItemPropertiesByEnumerable(enumerable);
			}
			ITypedList list = enumerable as ITypedList;
			if(list != null)
				return list.GetItemProperties(listAccessors);
			return GetListItemPropertiesByEnumerable(enumerable, listAccessors, 0);
		}
		PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable iEnumerable, PropertyDescriptor[] listAccessors, int startIndex) {
			object target = null;
			object firstItemByEnumerable = GetFirstItemByEnumerable(iEnumerable);
			if(firstItemByEnumerable != null) {
				target = GetList(listAccessors[startIndex].GetValue(firstItemByEnumerable));
			}
			if(target == null) {
				return GetListItemPropertiesByType(listAccessors[startIndex].PropertyType, listAccessors, startIndex);
			}
			startIndex++;
			IEnumerable enumerable = target as IEnumerable;
			if(enumerable != null) {
				if(startIndex == listAccessors.Length) {
					return GetListItemPropertiesByEnumerable(enumerable);
				}
				return GetListItemPropertiesByEnumerable(enumerable, listAccessors, startIndex);
			}
			return GetListItemPropertiesByInstance(target, listAccessors, startIndex);
		}
		PropertyDescriptorCollection GetListItemPropertiesByInstance(object target, PropertyDescriptor[] listAccessors, int startIndex) {
			if(listAccessors != null && listAccessors.Length > startIndex) {
				object list = listAccessors[startIndex].GetValue(target);
				if(list == null) {
					return GetListItemPropertiesByType(listAccessors[startIndex].PropertyType, listAccessors, startIndex);
				}
				PropertyDescriptor[] descriptorArray = null;
				startIndex++;
				if(listAccessors.Length > startIndex) {
					int num = listAccessors.Length - startIndex;
					descriptorArray = new PropertyDescriptor[num];
					for(int i = 0; i < num; i++) {
						descriptorArray[i] = listAccessors[startIndex + i];
					}
				}
				return GetListItemProperties(list, descriptorArray);
			}
			return GetProperties(target);
		}
		protected virtual object GetList(object list) {
			return list is IListSource ? ((IListSource)list).GetList() : list;
		}
	}
}
