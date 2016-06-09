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
	public abstract class SerializeHelperBase {
		protected internal const string LayoutPropertyName = "#LayoutVersion";
		SerializationContext fContext;
		protected object rootObject;
		protected internal SerializationContext Context { get { return fContext; } set { fContext = value; } }
		internal object RootObject { get { return rootObject; } }
		internal IXtraRootSerializationObject RootSerializationObject { get { return RootObject as IXtraRootSerializationObject; } }
		protected SerializeHelperBase(object rootObject) : this(rootObject, null) {
		}
		protected SerializeHelperBase(object rootObject, SerializationContext context) {
			this.rootObject = rootObject;
			fContext = context;
			if(fContext == null)
				fContext = CreateContextFromRoot();
			if(fContext == null)
				fContext = CreateSerializationContext();
		}
		protected virtual SerializationContext CreateContextFromRoot() {
			if(rootObject != null) {
				SerializationContextAttribute attr = TypeDescriptor.GetAttributes(rootObject)[typeof(SerializationContextAttribute)] as SerializationContextAttribute;
				if(attr != null)
					return attr.CreateSerializationContext();
			}
			return null;
		}
		protected virtual SerializationContext CreateSerializationContext() {
			return new SerializationContext();
		}
		protected SerializeHelperBase() 
			: this(null) {
		}
		protected internal virtual IList<SerializablePropertyDescriptorPair> SortProps(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			return fContext.SortProps(obj, pairsList);
		}
		internal static IList CheckObjects(IList objects) {
			if(objects == null || objects.Count == 0)
				return null;
			if(!objects.IsReadOnly)
				return objects;
			List<XtraObjectInfo> list = new List<XtraObjectInfo>();
			foreach(XtraObjectInfo objectInfo in objects) {
				if(objectInfo.Instance == null)
					continue;
				list.Add(objectInfo);
			}
			if(list.Count == 0)
				return null;
			return list.ToArray();
		}
		protected internal int GetPropertyId(PropertyDescriptor property) {
			XtraSerializablePropertyId id = property.Attributes[typeof(XtraSerializablePropertyId)] as XtraSerializablePropertyId;
			return id != null ? id.Id : 0;
		}
		protected internal virtual string GetMethodNameItem(string prop, string action) {
			return GetMethodName(prop, action) + "Item";
		}
		protected internal virtual string GetMethodName(string prop, string action) {
			return "Xtra" + action + prop;
		}
		protected internal virtual MethodInfo GetMethod(object obj, string name) {
			return fContext.GetMethod(obj, name);
		}
		internal XtraSerializableProperty GetXtraSerializableProperty(object obj, PropertyDescriptor property) {
			return fContext.GetXtraSerializableProperty(obj, property);
		}
		protected List<SerializablePropertyDescriptorPair> GetProperties(object obj) {
			return GetProperties(obj, null);
		}
		protected List<SerializablePropertyDescriptorPair> GetProperties(object obj, IXtraPropertyCollection store) {
			PropertyDescriptorCollection props = fContext.GetProperties(obj, store );
			int count = props.Count;
			List<SerializablePropertyDescriptorPair> pairsList = new List<SerializablePropertyDescriptorPair>(count);
			for(int i = 0; i < count; i++) {
				PropertyDescriptor prop = props[i];
				XtraSerializableProperty attribute = GetXtraSerializableProperty(obj, prop);
				pairsList.Add(new SerializablePropertyDescriptorPair(prop, attribute));
			}
			fContext.CustomGetSerializableProperties(obj, pairsList, props);
			return pairsList;
		}
		protected bool ShouldNotTryCache(XtraSerializableProperty attr) {
			return !attr.IsCachedProperty || !(RootObject is IXtraRootSerializationObject);
		}
		internal bool AllowProperty(object obj, PropertyDescriptor prop, OptionsLayoutBase options, bool isSerializing) {
			return fContext.AllowProperty(this, obj, prop, options, isSerializing);
		}
	}
	public class SerializablePropertyDescriptorPair {
		PropertyDescriptor descriptor;
		XtraSerializableProperty attribute;
		public SerializablePropertyDescriptorPair(PropertyDescriptor descriptor, XtraSerializableProperty attribute) {
			this.descriptor = descriptor;
			this.attribute = attribute;
		}
		public PropertyDescriptor Property {
			get { return descriptor; }
		}
		public XtraSerializableProperty Attribute {
			get { return attribute; }
		}
		public override string ToString() {
			string attr = "";
			if(attribute != null) attr = string.Format("{0} {1}", attribute.Order, attribute.Flags);
			return string.Format("{0} ({1}) {2}", descriptor.Name, descriptor.PropertyType.Name, attr);
		}
	}
}
