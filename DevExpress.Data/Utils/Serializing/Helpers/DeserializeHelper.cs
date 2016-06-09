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
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif //SILVERLIGHT
namespace DevExpress.Utils.Serializing.Helpers {
	public class DeserializeHelper : SerializeHelperBase {
		Exception exception;
		bool resetProperties;
		ObjectConverterImplementation objectConverterImpl;
		public event EventHandler<DeserializeExceptionEventArgs> ExceptionOccurred;
		public DeserializeHelper(object rootObject)
			: this(rootObject, true) {
		}
		public DeserializeHelper(object rootObject, bool resetProperties)
			: this(rootObject, resetProperties, null) {
		}
		public DeserializeHelper(object rootObject, bool resetProperties, SerializationContext context)
			: base(rootObject, context) {
			this.resetProperties = resetProperties;
			SetDefaultObjectConverterImpl();
		}
		public DeserializeHelper() {
			SetDefaultObjectConverterImpl();
		}
		void SetDefaultObjectConverterImpl(){
			this.ObjectConverterImpl = ObjectConverter.Instance;
		}
		public ObjectConverterImplementation ObjectConverterImpl {
			get { return objectConverterImpl; }
			set { objectConverterImpl = value; }
		}
		protected Exception Exception { get { return exception; } }
		protected bool ResetProperties { get { return resetProperties; } }
		public void RemoveProperty(IList store, string propertyName) {
			for(int n = store.Count - 1; n >= 0; n--) {
				XtraPropertyInfo po = store as XtraPropertyInfo;
				if(po != null && po.Name == propertyName) {
					store.RemoveAt(n);
					return;
				}
			}
		}
		static void AddRange(IList<object> where, ICollection what) {
			foreach (object value in what)
				where.Add(value);
		}
		public void DeserializeCollection(XtraSerializableProperty attr, XtraPropertyInfo root, object owner, object collection, OptionsLayoutBase options) {
			ICollection list = collection as ICollection;
			if (list == null || owner == null)
				return;
			List<object> prev = new List<object>();
			if(attr.MergeCollection)
				AddRange(prev, list);
			XtraItemEventArgs args = new XtraItemEventArgs(rootObject, owner, collection, root, options);
			Context.InvokeBeforeDeserializeCollection(args);
			try {
				if (attr.ClearCollection)
					Context.InvokeClearCollection(this, args);
				int count = Context.GetCollectionItemsCount(root);
				if (count < 1)
					return;
				for (int n = 1; n < count + 1; n++) {
					XtraPropertyInfo propItem = root.ChildProperties["Item" + n.ToString()];
					if (propItem == null)
						continue;
					DeserializeCollectionItem(attr, root, owner, collection, propItem, n - 1, options);
				}
				if(attr.MergeCollection) {
					MergeCollection(attr, root, owner, prev, list, options);
				}
			}
			finally {
				Context.InvokeAfterDeserializeCollection(args);
			}
		}
		private void MergeCollection(XtraSerializableProperty attr, XtraPropertyInfo root, object owner, List<object> prevCollection, ICollection newCollection, OptionsLayoutBase options) {
			List<object> newList = new List<object>();
			AddRange(newList, newCollection);
			IEnumerator pe = prevCollection.GetEnumerator();
			IXtraCollectionDeserializationOptionsProvider optionsProvider = owner as IXtraCollectionDeserializationOptionsProvider;
			if(optionsProvider != null && optionsProvider.AddNewItems) {
				while(pe.MoveNext()) {
					if(!ContainsCollectionItem(attr, newCollection, pe.Current, owner) && IsNewItem(attr, root, newCollection, owner, pe.Current)) {
						XtraPropertyInfo item = new XtraPropertyInfo() { Value = pe.Current };
						InsertItemIntoCollection(attr, root, newCollection, item, options, new XtraSetItemIndexEventArgs(null, owner, newCollection, item, newCollection.Count), pe.Current, true);
					}
				}
			}
			if(optionsProvider != null && optionsProvider.RemoveOldItems) {
				foreach(object item in newList) {
					if(!ContainsCollectionItem(attr, prevCollection, item, owner) && IsOldItem(attr, root, newCollection, owner, item)) { 
						XtraPropertyInfo pitem = new XtraPropertyInfo() { Value = item };
						RemoveItemFromCollection(root, new XtraSetItemIndexEventArgs(null, owner, newCollection, pitem, -1));
					}
				}	
			}
		}
		private bool IsOldItem(XtraSerializableProperty attr, XtraPropertyInfo root, ICollection newCollection, object owner, object item) {
			object prevItemId = GetCollectionItemId(attr, item, owner);
			MethodInfo mi = GetMethod(owner, GetMethodName(root.Name, "IsOldItem"));
			if(mi == null) return false;
			XtraOldItemEventArgs e = new XtraOldItemEventArgs(root, owner, newCollection, new XtraPropertyInfo() { Value = item });
			mi.Invoke(owner, new object[] { e });
			return e.OldItem;
		}
		private bool IsNewItem(XtraSerializableProperty attr, XtraPropertyInfo root, ICollection newCollection, object owner, object item) {
			object prevItemId = GetCollectionItemId(attr, item, owner);
			MethodInfo mi = GetMethod(owner, GetMethodName(root.Name, "IsNewItem"));
			if(mi == null) return false;
			XtraNewItemEventArgs e = new XtraNewItemEventArgs(root, owner, newCollection, new XtraPropertyInfo() { Value = item });
			mi.Invoke(owner, new object[] { e });
			return e.NewItem;
		}
		private void RemoveItemFromCollection(XtraPropertyInfo root, XtraSetItemIndexEventArgs e) {
			InvokeRemoveCollectionItem(root.Name, e);
		}
		private bool ContainsCollectionItem(XtraSerializableProperty attr, ICollection prevCollection, object item, object owner) {
			object prevItemId = GetCollectionItemId(attr, item, owner);
			foreach(object citem in prevCollection) {
				object newItemId = GetCollectionItemId(attr, citem, owner);
				if(object.Equals(prevItemId, newItemId))
					return true;
			}
			return false;
		}
		private object GetCollectionItemId(XtraSerializableProperty attr, object item, object owner) {
			IXtraSerializationIdProvider provider = owner as IXtraSerializationIdProvider;
			if(provider != null)
				return provider.GetSerializationId(attr, item);
			PropertyInfo pinfo = item.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
			if(pinfo != null)
				return pinfo.GetValue(item, null);
			return null;
		}
		protected void DeserializeCollectionItem(XtraSerializableProperty attr, XtraPropertyInfo root, object owner, object collection, XtraPropertyInfo item, int index, OptionsLayoutBase options) {
			ICollection coll = collection as ICollection;
			if(coll == null)
				return;
			XtraSetItemIndexEventArgs setArgs = new XtraSetItemIndexEventArgs(rootObject, owner, collection, item, index);
			object valueFromCache = TryGetCollectionItemFromCache(attr, root.Name, item);
			if(valueFromCache != null) {
				item.Value = valueFromCache;
				InvokeSetIndexCollectionItem(root.Name, setArgs);
				return;
			}
			object collItem = null;
			if(attr.UseFindItem) {
				collItem = Context.InvokeFindCollectionItem(this, root.Name, new XtraItemEventArgs(rootObject, owner, collection, item));
			}
			bool newItem = false;
			if(collItem == null && attr.UseCreateItem) {
				collItem = Context.InvokeCreateCollectionItem(this, root.Name, new XtraItemEventArgs(rootObject, owner, collection, item, options, index));
				newItem = collItem != null;
			}
#if!SL
			if(collItem == null && attr.Visibility == XtraSerializationVisibility.SimpleCollection) {
				if(item.PropertyType != null) {
					try {
						collItem = Convert.ChangeType(item.Value, item.PropertyType);
					}
					catch { }
				}
			}
#endif
			InsertItemIntoCollection(attr, root, collection, item, options, setArgs, collItem, newItem);
		}
		private void InsertItemIntoCollection(XtraSerializableProperty attr, XtraPropertyInfo root, object collection, XtraPropertyInfo item, OptionsLayoutBase options, XtraSetItemIndexEventArgs setArgs, object collItem, bool newItem) {
			item.Value = collItem;
			if(collItem != null || item.IsNull) {
				if(attr.Visibility == XtraSerializationVisibility.NameCollection) {
					if(collection is IList) {
						((IList)collection).Add(collItem);
						InvokeSetIndexCollectionItem(root.Name, setArgs);
					}
					return;
				}
				if(attr.Visibility == XtraSerializationVisibility.SimpleCollection) {
					collItem = collItem == null ? null : ValueToObject(item, collItem.GetType());
					item.Value = collItem;
					if(collection is IList) {
						((IList)collection).Add(collItem);
						InvokeSetIndexCollectionItem(root.Name, setArgs);
					}
				}
				else {
					OptionsLayoutBase optionsLayout = newItem ? OptionsLayoutBase.FullLayout : options;
					if(attr.DeserializeCollectionItemBeforeCallSetIndex)
						DeserializeObject(collItem, item.ChildProperties, optionsLayout);
					InvokeSetIndexCollectionItem(root.Name, setArgs);
					if(!attr.DeserializeCollectionItemBeforeCallSetIndex)
						DeserializeObject(collItem, item.ChildProperties, optionsLayout);
				}
			}
		}
		void InvokeSetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			Context.InvokeSetIndexCollectionItem(this, propertyName, e);
		}
		void InvokeRemoveCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			Context.InvokeRemoveCollectionItem(this, propertyName, e);
		}
		public void DeserializeObjects(IList objects, IXtraPropertyCollection store, OptionsLayoutBase options) {
			Context.DeserializeObjectsCore(this, objects, store, options);
		}
		internal static bool CallStartDeserializing(object obj, string layoutVersion) {
			IXtraSerializable xtra = obj as IXtraSerializable;
			if(xtra != null) {
				LayoutAllowEventArgs args = new LayoutAllowEventArgs(layoutVersion);
				xtra.OnStartDeserializing(args);
				if(!args.Allow)
					return true;
			}
			return false;
		}
		internal static void CallEndDeserializing(object obj, string layoutVersion) {
			IXtraSerializable xtra = obj as IXtraSerializable;
			if(xtra != null) {
				xtra.OnEndDeserializing(layoutVersion);
			}
		}
		protected virtual string GetRootVersion() {
			return string.Empty;
		}
		protected void DeserializeObject(object obj, IXtraPropertyCollection store, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			if(store == null)
				return;
			IXtraSerializable2 xtra2 = obj as IXtraSerializable2;
			IXtraSerializableLayout xtraLayout = obj as IXtraSerializableLayout;
			IXtraSerializableLayoutEx xtraLayoutEx = obj as IXtraSerializableLayoutEx;
			string restoredVersion = GetRootVersion();
			if(xtraLayout != null) 
				restoredVersion = GetLayoutVersion(store);
			if(RaiseStartDeserializing(obj, restoredVersion))return;
			try {
				if(xtraLayoutEx != null && ResetProperties)
					xtraLayoutEx.ResetProperties(options);
				if(xtra2 != null)
					xtra2.Deserialize((IList)store);
				else {
					IXtraPartlyDeserializable partlyDeserializable = obj as IXtraPartlyDeserializable;
					if(partlyDeserializable != null)
						partlyDeserializable.Deserialize(RootObject, store);
					List<SerializablePropertyDescriptorPair> pairsList = GetProperties(obj, store);
					if(pairsList == null || pairsList.Count == 0)
						return;
					IList<SerializablePropertyDescriptorPair> propList = SortProps(obj, pairsList);
					foreach(SerializablePropertyDescriptorPair pair in propList) {
						PropertyDescriptor prop = pair.Property;
						try {
							if(!AllowProperty(obj, prop, options, false))
								continue;
							DeserializeProperty(store, obj, pair, parentFlags, options);
						}
						catch(Exception ex) {
							this.exception = ex;
							RaiseExceptionOccurred(ex);
						}
					}
				}
			}
			finally { RaiseEndDeserializing(obj, restoredVersion); }
		}
		protected virtual bool RaiseStartDeserializing(object obj, string restoredLayoutVersion) {
			return CallStartDeserializing(obj, restoredLayoutVersion);
		}
		protected virtual void RaiseEndDeserializing(object obj, string restoredLayoutVersion) {
			CallEndDeserializing(obj, restoredLayoutVersion);
		}
		public void DeserializeObject(object obj, IXtraPropertyCollection store, OptionsLayoutBase options) {
			DeserializeObject(obj, store, XtraSerializationFlags.None, options);
		}
		public void AfterDeserializeRootObject() {
			Context.AfterDeserializeRootObject();
		}
		protected void DeserializeProperty(IXtraPropertyCollection store, object obj, SerializablePropertyDescriptorPair pair, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			PropertyDescriptor prop = pair.Property;
			XtraSerializableProperty attr = pair.Attribute;
			if(attr == null || !attr.Serialize || TryGetValueFromCache(attr, store, obj, prop))
				return;
			Context.ResetProperty(this, obj, prop, attr);
			XtraPropertyInfo bp = FindProperty(store, prop.Name);
			if(bp == null)
				return;
			if(!Context.CanDeserializeProperty(obj, prop) || Context.CustomDeserializeProperty(this, obj, prop, bp))
				return;
			if(attr.SerializeCollection) {
				object collection = prop.GetValue(obj);
				DeserializeCollection(attr, bp, obj, collection, options);
			} else {
				object val;
				if(attr.Visibility == XtraSerializationVisibility.Content && bp.Value == null) {
					val = attr.UseCreateItem ? Context.InvokeCreateContentPropertyValueMethod(this, new XtraItemEventArgs(rootObject, obj, null, bp), prop) : prop.GetValue(obj); 
					if(val == null)
						return;
					DeserializeObject(val, bp.ChildProperties, attr.Flags, options);
					if(prop.PropertyType.IsValueType())
						prop.SetValue(obj, val);
					Context.InvokeAfterDeserialize(this, obj, bp, val);
				} else if(attr.Visibility != XtraSerializationVisibility.Reference) {
					val = ValueToObject(bp, prop.PropertyType);
					if((attr.Flags & XtraSerializationFlags.UseAssign) != 0) {
						MethodInfo mi = GetMethod(obj, GetMethodName(prop.Name, "Assign"));
						if(mi != null) {
							mi.Invoke(obj, new object[] { val });
							return;
						}
					}
					if(prop.Name == "Name")
						RestoreNameProperty(prop, obj, val);
					else
						prop.SetValue(obj, val);
				}
			}
		}
		object ValueToObject(XtraPropertyInfo prop, Type type) {
			prop.SetObjectConverterImpl(ObjectConverterImpl);
			return prop.ValueToObject(type);
		}
		static int GetCacheIndex(XtraPropertyInfo cacheIndexPropetyInfo) {
			if(cacheIndexPropetyInfo == null || (cacheIndexPropetyInfo.ChildProperties != null && cacheIndexPropetyInfo.ChildProperties.Count > 0))
				return -1;
			int index;
			if(int.TryParse(cacheIndexPropetyInfo.Value.ToString(), out index))
				return index;
			return -1;
		}
		object TryGetCollectionItemFromCache(XtraSerializableProperty attr, string name, XtraPropertyInfo item) {
			if(ShouldNotTryCache(attr))
				return null;
			XtraPropertyInfo cacheIndexPropetyInfo = FindProperty(item.ChildProperties, name);
			int index = GetCacheIndex(cacheIndexPropetyInfo);
			if(index == -1)
				return null;
			return GetCachedValue(name, index);
		}
		bool TryGetValueFromCache(XtraSerializableProperty attr, IXtraPropertyCollection store, object obj, PropertyDescriptor prop) {
			if(ShouldNotTryCache(attr))
				return false;
			XtraPropertyInfo cacheIndexPropetyInfo = FindProperty(store, prop.Name);
			int index = GetCacheIndex(cacheIndexPropetyInfo);
			if(index == -1)
				return false;
			prop.SetValue(obj, GetCachedValue(prop.Name, index));
			return true;
		}
		object GetCachedValue(string propertyName, int index) {
			return RootSerializationObject.GetObjectByIndex(propertyName, index);
		}
		void RestoreNameProperty(PropertyDescriptor descriptor, object component, object val) {
			try {
				descriptor.SetValue(component, val);
			} catch {
			}
		}
		public XtraPropertyInfo FindProperty(IXtraPropertyCollection props, string name) {
			return FindPropertyCore(props, name);
		}
		public static string GetLayoutVersion(IXtraPropertyCollection props) {
			XtraPropertyInfo pInfo = FindPropertyCore(props, LayoutPropertyName);
			return (pInfo != null) ? pInfo.Value as string : null;
		}
		static XtraPropertyInfo FindPropertyCore(IXtraPropertyCollection props, string name) {
			XtraPropertyCollection collection = props as XtraPropertyCollection;
			if(collection != null)
				return collection[name];
			foreach(XtraPropertyInfo p in props) {
				if(p.Name == name)
					return p;
			}
			return null;
		}
		void RaiseExceptionOccurred(Exception exception) {
			if(ExceptionOccurred != null) {
				ExceptionOccurred(this, new DeserializeExceptionEventArgs(exception));
			}
		}
	}
	public class DeserializeExceptionEventArgs : EventArgs {
		public Exception Exception { get; private set; }
		public DeserializeExceptionEventArgs(Exception exception) {
			Exception = exception;
		}
	}
	public interface IXtraSerializationIdProvider {
		object GetSerializationId(XtraSerializableProperty property, object item);
	}
	public interface IXtraCollectionDeserializationOptionsProvider {
		bool RemoveOldItems { get; }
		bool AddNewItems { get; }
	}
}
