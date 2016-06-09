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
#if !SILVERLIGHT
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif //SILVERLIGHT
namespace DevExpress.Utils.Serializing.Helpers {
	#region SerializeHelper
	public class SerializeHelper : SerializeHelperBase {
		#region static
		public const int UndefinedObjectIndex = -1;
		public const string IndexPropertyName = "Index";
		internal static void CallStartSerializing(object obj) {
			IXtraSerializable xtra = obj as IXtraSerializable;
			if(xtra != null)
				xtra.OnStartSerializing();
		}
		internal static void CallEndSerializing(object obj) {
			IXtraSerializable xtra = obj as IXtraSerializable;
			if(xtra != null)
				xtra.OnEndSerializing();
		}
		internal static void AddIndexPropertyInfo(XtraPropertyInfo propInfo, int index) {
			if(index != UndefinedObjectIndex)
				propInfo.ChildProperties.Add(new XtraPropertyInfo(IndexPropertyName, typeof(int), index));
		}
		#endregion
		public SerializeHelper(object rootObject)
			: this(rootObject, null) {
		}
		public SerializeHelper(object rootObject, SerializationContext context)
			: base(rootObject, context) {
		}
		public SerializeHelper() {
		}
		public IXtraPropertyCollection SerializeObjects(IList objects, OptionsLayoutBase options) {
			return Context.SerializeObjectsCore(this, objects, options);
		}
		public virtual IXtraPropertyCollection SerializeObject(object obj, OptionsLayoutBase options) {
			return SerializeObject(obj, XtraSerializationFlags.None, options);
		}
		public virtual IXtraPropertyCollection SerializeObject(object obj, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			RaiseStartSerializing(obj);
			try {
				return SerializeObjectCore(obj, parentFlags, options);
			}
			finally { RaiseEndSerializing(obj); }
		}
		protected virtual void RaiseStartSerializing(object obj) {
			CallStartSerializing(obj);
		}
		protected virtual void RaiseEndSerializing(object obj) {
			CallEndSerializing(obj);
		}
		protected internal virtual IXtraPropertyCollection SerializeObjectCore(object obj, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			XtraPropertyInfoCollection store = new XtraPropertyInfoCollection();
			store.AddRange(SerializeLayoutVersion(options, obj));
			store.AddRange(PerformManualSerialization(obj));
			List<SerializablePropertyDescriptorPair> pairsList = GetProperties(obj);
			if(pairsList != null && pairsList.Count > 0) {
				IList<SerializablePropertyDescriptorPair> propList = SortProps(obj, pairsList);
				foreach(SerializablePropertyDescriptorPair pair in propList) {
					PropertyDescriptor prop = pair.Property;
					if(AllowProperty(obj, prop, options, true))
						SerializeProperty(store, obj, pair, parentFlags, options);
				}
			}
			return store;
		}
		protected internal virtual XtraPropertyInfoCollection SerializeLayoutVersion(OptionsLayoutBase options, object obj) {
			XtraPropertyInfoCollection result = new XtraPropertyInfoCollection();
			IXtraSerializableLayout xtraLayout = obj as IXtraSerializableLayout;
			if(xtraLayout != null) {
				string layoutVersion = xtraLayout.LayoutVersion;
				if(options != OptionsLayoutBase.FullLayout)
					layoutVersion = options.LayoutVersion;
				result.Add(new XtraPropertyInfo(LayoutPropertyName, typeof(string), layoutVersion));
			}
			return result;
		}
		protected internal virtual XtraPropertyInfo[] PerformManualSerialization(object obj) {
			IXtraSerializable2 xtra2 = obj as IXtraSerializable2;
			if(xtra2 != null)
				return xtra2.Serialize();
			else
				return new XtraPropertyInfo[] { };
		}
		protected internal virtual bool CheckNeedSerialize(object obj, PropertyDescriptor prop, XtraSerializableProperty attr, XtraSerializationFlags parentFlags) {
			if(attr == null || !attr.Serialize || !Context.ShouldSerializeProperty(this, obj, prop, attr))
				return false;
			if(((parentFlags | attr.Flags) & XtraSerializationFlags.DefaultValue) != 0) {
				if(prop.SerializationVisibility == DesignerSerializationVisibility.Visible && !prop.ShouldSerializeValue(obj) &&
					prop.GetType().Name != "InheritedPropertyDescriptor")
					return false;
			}
			return Context.InvokeShouldSerialize(this, obj, prop);
		}
		protected internal virtual void SerializeProperty(XtraPropertyInfoCollection store, object obj, SerializablePropertyDescriptorPair pair, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			PropertyDescriptor prop = pair.Property;
			XtraSerializableProperty attr = pair.Attribute;
			if(!CheckNeedSerialize(obj, prop, attr, parentFlags))
				return;
			if(attr.SerializeCollection)
				SerializePropertyAsCollection(store, obj, prop, options, attr);
			else {
				int index = SerializeHelper.UndefinedObjectIndex;
				if(TrySerializePropertyValueCacheIndex(attr, prop, store, obj, ref index))
					return;
				if(attr.Visibility == XtraSerializationVisibility.Content)
					SerializePropertyAsContent(store, obj, prop, options, attr, index);
				else
					SerializePropertyAsSimple(store, obj, prop);
			}
		}
		bool TrySerializePropertyValueCacheIndex(XtraSerializableProperty attr, PropertyDescriptor prop, IXtraPropertyCollection store, object obj, ref int index) {
			if(ShouldNotTryCache(attr))
				return false;
			object value = prop.GetValue(obj);
			if(value == null)
				return false;
			return TrySerializeCacheIndex(prop.Name, attr, store, value, ref index);
		}
		internal bool TrySerializeCollectionItemCacheIndex(string name, XtraSerializableProperty attr, IXtraPropertyCollection store, object value, ref int index) {
			if(ShouldNotTryCache(attr))
				return false;
			return TrySerializeCacheIndex(name, attr, store, value, ref index);
		}
		bool TrySerializeCacheIndex(string name, XtraSerializableProperty attr, IXtraPropertyCollection store, object value, ref int index) {
			SerializationInfo info = RootSerializationObject.GetIndexByObject(name, value);
			index = info.Index;
			if(!info.Serialized) {
				info.Serialized = true;
				return false;
			}
			store.Add(new XtraPropertyInfo(name, typeof(int), info.Index));
			return true;
		}
		protected virtual XtraPropertyInfo CreateXtraPropertyInfo(PropertyDescriptor prop, object value, bool isKey) {
			return new XtraPropertyInfo(prop.Name, prop.PropertyType, value, isKey);
		}
		protected internal virtual void SerializePropertyAsSimple(XtraPropertyInfoCollection store, object obj, PropertyDescriptor prop) {
			object val = prop.GetValue(obj);
			XtraPropertyInfo propInfo = CreateXtraPropertyInfo(prop, val, false);
			store.Add(propInfo);
		}
		protected internal virtual void SerializePropertyAsContent(XtraPropertyInfoCollection store, object obj, PropertyDescriptor prop, OptionsLayoutBase options, XtraSerializableProperty attr, int index) {
			object val = prop.GetValue(obj);
			if(val == null)
				return;
			IXtraPropertyCollection serializedObject = SerializeObject(val, attr.Flags, options);
			if((attr.Flags & XtraSerializationFlags.DefaultValue) != 0 && serializedObject.Count == 0)
				return;
			XtraPropertyInfo propInfo = CreateXtraPropertyInfo(prop, null, true);
			propInfo.ChildProperties.AddRange(serializedObject);
			AddIndexPropertyInfo(propInfo, index);
			store.Add(propInfo);
		}
		protected internal virtual void SerializePropertyAsCollection(XtraPropertyInfoCollection store, object obj, PropertyDescriptor prop, OptionsLayoutBase options, XtraSerializableProperty attr) {
			object val = prop.GetValue(obj);
			ICollection list = val as ICollection;
			if(list == null)
				return;
			SerializeCollection(attr, prop.Name, store, obj, attr.Flags, options, list);
		}
		protected internal virtual void SerializeCollection(XtraSerializableProperty attr, string name, XtraPropertyInfoCollection props, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options, ICollection list) {
			XtraPropertyInfo root = new XtraPropertyInfo(name, null, 0, true);
			if(list.Count > 0) {
				CollectionItemSerializationStrategy itemSerializationStrategy = CreateCollectionItemSerializationStrategy(attr, name, list, owner, parentFlags, options);
				int itemIndex = 1;
				foreach(object item in list) {
					XtraPropertyInfo itemProperty = itemSerializationStrategy.SerializeCollectionItem(itemIndex, item);
					if(itemProperty != null) {
						root.ChildProperties.Add(itemProperty);
						itemIndex++;
					}
				}
			}
			root.Value = root.ChildProperties.Count;
			props.Add(root);
		}
		protected internal virtual CollectionItemSerializationStrategy CreateCollectionItemSerializationStrategy(XtraSerializableProperty attr, string name, ICollection collection, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
			switch(attr.Visibility) {
				case XtraSerializationVisibility.NameCollection:
					return new CollectionItemSerializationStrategyName(this, name, collection, owner);
				case XtraSerializationVisibility.SimpleCollection:
					return new CollectionItemSerializationStrategySimple(this, name, collection, owner);
				case XtraSerializationVisibility.Collection:
					return new CollectionItemSerializationStrategyCollection(this, name, collection, owner, parentFlags, options, attr);
				default:
					return new CollectionItemSerializationStrategyEmpty(this, name, collection, owner);
			}
		}
	}
	#endregion
}
