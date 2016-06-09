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
using System.Reflection;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif //SILVERLIGHT
namespace DevExpress.Utils.Serializing.Helpers {
	public class SerializationContext {
		Dictionary<MultiKey, MethodInfo> methodInfos = new Dictionary<MultiKey, MethodInfo>();
		internal MethodInfo GetMethod(object obj, string name) {
			if(obj == null)
				return null;
			MethodInfo methodInfo;
			Type type = obj.GetType();
			MultiKey key = new MultiKey(new object[] { name, type });
			if(methodInfos.TryGetValue(key, out methodInfo))
				return methodInfo;
			while(type != null) {
				methodInfo = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if(methodInfo != null) {
					methodInfos.Add(key, methodInfo);
					return methodInfo;
				}
				type = type.GetBaseType();
			}
			methodInfos.Add(key, null);
			return null;
		}
		internal XtraSerializableProperty GetXtraSerializableProperty(object obj, PropertyDescriptor property) {
			return property.Attributes[typeof(XtraSerializableProperty)] as XtraSerializableProperty;
		}
		protected internal virtual IList<SerializablePropertyDescriptorPair> SortProps(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			DXCollection<SerializablePropertyDescriptorPair> list = new DXCollection<SerializablePropertyDescriptorPair>();
			list.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			IComparer<SerializablePropertyDescriptorPair> comparer = new PropertyDescriptorComparer(this, obj);
			AddSortedProps(list, pairsList, comparer);
			return list;
		}
		protected virtual void AddSortedProps(DXCollection<SerializablePropertyDescriptorPair> list, List<SerializablePropertyDescriptorPair> pairsList, IComparer<SerializablePropertyDescriptorPair> comparer) {
			list.AddRange(pairsList);
			list.Sort(comparer);
		}
		protected internal virtual bool ShouldSerializeProperty(SerializeHelper helper, object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			return true;
		}
		protected internal virtual int GetCollectionItemsCount(XtraPropertyInfo root) {
			return Convert.ToInt32(root.Value);
		}
		protected internal virtual void DeserializeObjectsCore(DeserializeHelper helper, IList objects, IXtraPropertyCollection store, OptionsLayoutBase options) {
			objects = SerializeHelperBase.CheckObjects(objects);
			if(objects == null || store == null)
				return;
			for(int i = 0; i < objects.Count; i++) {
				IXtraPropertyCollection list = GetObjectProperties((XtraObjectInfo)objects[i], store, i);
				if(list != null) {
					helper.DeserializeObject(((XtraObjectInfo)objects[i]).Instance, list, options);
					if(list == store)
						break;
				}
			}
		}
		static IXtraPropertyCollection GetObjectProperties(XtraObjectInfo objectInfo, IXtraPropertyCollection store, int index) {
			IXtraPropertyCollection res = new XtraPropertyCollection();
			for(int n = 0; n < store.Count; n++) {
				XtraPropertyInfo info = store[n] as XtraPropertyInfo;
				if(info.IsXtraObjectInfo) {
					if(objectInfo.Name == info.Value.ToString()) {
						res.AddRange(info.ChildProperties);
						return res;
					}
				} else {
					if(n == 0) {
						return store;
					}
				}
			}
			if(res.Count > 0)
				return res;
			return null;
		}
		protected internal virtual IXtraPropertyCollection SerializeObjectsCore(SerializeHelper helper, IList objects, OptionsLayoutBase options) {
			objects = SerializeHelper.CheckObjects(objects);
			if(objects == null)
				return null;
			IXtraPropertyCollection store = new XtraPropertyCollection();
			foreach(XtraObjectInfo objectInfo in objects) {
				IXtraPropertyCollection res = helper.SerializeObject(objectInfo.Instance, options);
				if(objects.Count > 1) {
					XtraPropertyInfo propInfo = new XtraPropertyInfo(objectInfo);
					propInfo.ChildProperties.AddRange(res);
					store.Add(propInfo);
				} else {
					return res;
				}
			}
			return store;
		}
		protected internal virtual PropertyDescriptorCollection GetProperties(object obj) {
			return GetProperties(obj, null);
		}
		protected internal virtual PropertyDescriptorCollection GetProperties(object obj, IXtraPropertyCollection store) {
			return TypeDescriptor.GetProperties(obj);
		}
		protected internal virtual bool InvokeShouldSerialize(SerializeHelper helper, object obj, PropertyDescriptor property) {
			string propetyName = property.Name;
			IXtraSupportShouldSerialize impl = obj as IXtraSupportShouldSerialize;
			if (impl != null)
				return impl.ShouldSerialize(propetyName);
			MethodInfo mi = helper.GetMethod(obj, helper.GetMethodName(propetyName, "ShouldSerialize"));
			if(mi != null) {
				return (bool)mi.Invoke(obj, null);
			}
			return true;
		}
		protected internal virtual MethodInfo GetShouldSerializeCollectionMethodInfo(SerializeHelper helper, string name, object owner) {
			return helper.GetMethod(owner, helper.GetMethodNameItem(name, "ShouldSerializeCollection"));
		}
		protected internal virtual bool ShouldSerializeCollectionItem(SerializeHelper helper,  object owner, MethodInfo mi, XtraPropertyInfo itemProperty, object item, XtraItemEventArgs e) {
			if(mi == null) return true;
			itemProperty.PropertyType = mi.ReturnType;
			itemProperty.Value = item;
			try {
				return (bool)mi.Invoke(owner, new object[] { e });
			}
			finally { itemProperty.Value = null; }
		}
		protected internal virtual object InvokeCreateContentPropertyValueMethod(DeserializeHelper helper, XtraItemEventArgs e, PropertyDescriptor prop) {
			MethodInfo createMethod = helper.GetMethod(e.Owner, helper.GetMethodName(prop.Name, "Create"));
			object value = null;
			if(createMethod != null) {
				value = createMethod.Invoke(e.Owner, new object[] { e });
				prop.SetValue(e.Owner, value);
			}
			return value;
		}
		protected internal virtual void InvokeBeforeDeserializeCollection(XtraItemEventArgs e) {
			IXtraSupportDeserializeCollection impl = e.Owner as IXtraSupportDeserializeCollection;
			if (impl != null)
				impl.BeforeDeserializeCollection(e.Item.Name, e);
		}
		protected internal virtual void InvokeAfterDeserializeCollection(XtraItemEventArgs e) {
			IXtraSupportDeserializeCollection impl = e.Owner as IXtraSupportDeserializeCollection;
			if (impl != null)
				impl.AfterDeserializeCollection(e.Item.Name, e);
		}
		protected internal virtual void InvokeClearCollection(DeserializeHelper helper, XtraItemEventArgs e) {
			IXtraSupportDeserializeCollection impl = e.Owner as IXtraSupportDeserializeCollection;
			if (impl != null) {
				if(impl.ClearCollection(e.Item.Name, e)) return;
			}
			MethodInfo m = helper.GetMethod(e.Owner, helper.GetMethodName(e.Item.Name, "Clear"));
			if (m != null)
				m.Invoke(e.Owner, new object[] { e });
			else {
				if (e.Collection is IList) {
					((IList)e.Collection).Clear();
				}
			}
		}
		protected internal virtual void InvokeSetIndexCollectionItem(DeserializeHelper helper, string propertyName, XtraSetItemIndexEventArgs e) {
			IXtraSupportDeserializeCollectionItem impl = e.Owner as IXtraSupportDeserializeCollectionItem;
			if (impl != null) {
				impl.SetIndexCollectionItem(propertyName, e);
				return;
			}
			MethodInfo setIndexMethodInfo = helper.GetMethod(e.Owner, helper.GetMethodNameItem(propertyName, "SetIndex"));
			if (setIndexMethodInfo != null)
				setIndexMethodInfo.Invoke(e.Owner, new object[] { e });
		}
		protected internal virtual void InvokeRemoveCollectionItem(DeserializeHelper helper, string propertyName, XtraSetItemIndexEventArgs e) {
			IXtraSupportDeserializeCollectionItemEx impl = e.Owner as IXtraSupportDeserializeCollectionItemEx;
			if(impl != null) {
				impl.RemoveCollectionItem(propertyName, e);
				return;
			}
			MethodInfo setIndexMethodInfo = helper.GetMethod(e.Owner, helper.GetMethodNameItem(propertyName, "Remove"));
			if(setIndexMethodInfo != null)
				setIndexMethodInfo.Invoke(e.Owner, new object[] { e });
		}
		protected internal virtual object InvokeCreateCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			IXtraSupportDeserializeCollectionItem impl = e.Owner as IXtraSupportDeserializeCollectionItem;
			if (impl != null)
				return impl.CreateCollectionItem(propertyName, e);
			MethodInfo createMethodInfo = helper.GetMethod(e.Owner, helper.GetMethodNameItem(propertyName, "Create"));
			if (createMethodInfo != null)
				return createMethodInfo.Invoke(e.Owner, new object[] { e });
			return null;
		}
		protected internal virtual object InvokeFindCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			MethodInfo findMethodInfo = helper.GetMethod(e.Owner, helper.GetMethodNameItem(propertyName, "Find"));
			if(findMethodInfo != null)
				return findMethodInfo.Invoke(e.Owner, new object[] { e });
			return null;
		}
		protected internal virtual void InvokeAfterDeserialize(
			DeserializeHelper helper, object obj, XtraPropertyInfo bp, object value) {
		}
		protected internal virtual void AfterDeserializeRootObject() {
		}
		protected internal virtual void CustomGetSerializableProperties(
			object obj, List<SerializablePropertyDescriptorPair> pairsList, PropertyDescriptorCollection props) {
		}
		protected internal virtual bool AllowProperty(SerializeHelperBase helper, object obj, PropertyDescriptor prop, OptionsLayoutBase options, bool isSerializing) {
			IXtraSerializableLayoutEx xtraLayoutEx = obj as IXtraSerializableLayoutEx;
			if(xtraLayoutEx == null) return true;
			int id = GetPropertyId(helper, prop);
			return (id == -1) || xtraLayoutEx.AllowProperty(options, prop.Name, id);
		}
		protected int GetPropertyId(SerializeHelperBase helper, PropertyDescriptor prop) {
			return helper.GetPropertyId(prop);
		}
		protected internal virtual bool CanDeserializeProperty(object obj, PropertyDescriptor prop) { return (obj != null) && (prop != null); }
		protected internal virtual void ResetProperty(DeserializeHelper helper, object obj, PropertyDescriptor property, XtraSerializableProperty attr) {
			MethodInfo resetMethodInfo = helper.GetMethod(obj, helper.GetMethodName(property.Name, "Reset"));
			if(resetMethodInfo != null)
				resetMethodInfo.Invoke(obj, new object[] { });
		}
		protected internal virtual bool CustomDeserializeProperty(DeserializeHelper helper, object obj, PropertyDescriptor property, XtraPropertyInfo propertyInfo) {
			MethodInfo mi = helper.GetMethod(obj, helper.GetMethodName(property.Name, "Deserialize"));
			if(mi != null) {
				mi.Invoke(obj, new object[] { new XtraEventArgs(propertyInfo) });
				return true;
			}
			return false;
		}
	}
}
