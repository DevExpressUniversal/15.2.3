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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Serialization {
	public class ReportSerializationContextBase : PrintingSystemSerializationContext {
		static readonly object padlock = new object();
		readonly static Dictionary<ReflectionKey, MethodInfo> methodDict = new Dictionary<ReflectionKey, MethodInfo>();
		protected internal override IList<SerializablePropertyDescriptorPair> SortProps(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			return SortPropsCore(obj, pairsList);
		}
		protected internal override bool InvokeShouldSerialize(SerializeHelper helper, object obj, PropertyDescriptor property) {
			return base.InvokeShouldSerialize(helper, obj, property) && InvokeShouldSerialize(obj, property.Name);
		}
		static bool InvokeShouldSerialize(object obj, string propertyName) {
			MethodInfo methodInfo = null;
			lock(padlock) {
				ReflectionKey key = new ReflectionKey(obj.GetType(), propertyName);
				if(!methodDict.TryGetValue(key, out methodInfo)) {
					methodInfo = FindMethodInfo(obj.GetType(), propertyName);
					methodDict.Add(key, methodInfo);
				}
			}
			return methodInfo != null ? (bool)methodInfo.Invoke(obj, null) : true;
		}
		static MethodInfo FindMethodInfo(Type type, string propertyName) {
			if(type == typeof(object))
				return null;
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			MethodInfo methodInfo = (MethodInfo)type.GetMethod("ShouldSerializeXml" + propertyName, flags);
			if(methodInfo == null)
				methodInfo = (MethodInfo)type.GetMethod("ShouldSerialize" + propertyName, flags);
			return methodInfo ?? FindMethodInfo(type.BaseType, propertyName);
		}
	}
	public interface IRootXmlObject : IExtensionsProvider {
		Collection<IObject> ObjectStorage { get; }
		void PerformAsLoad(Action0 action);
	}
	public class XtraReportsSerializationContext : ReportSerializationContextBase {
		readonly ReferenceSerializationHelper serializationHelper = new ReferenceSerializationHelper();
		readonly ReferenceDeserializationHelper deserializationHelper = new ReferenceDeserializationHelper();
		IRootXmlObject rootObject;
		public IRootXmlObject RootObject {
			get {
				return rootObject;
			}
			set {
				rootObject = value;
				if(rootObject != null) 
					rootObject.ObjectStorage.Clear();
			}
		}
		public Collection<IObject> ObjectStorage {
			get { return RootObject.ObjectStorage; }
		}
		public List<object> ReferencedObjects {
			get { return serializationHelper.ReferencedObjects; }
		}
		public void FillObjectStorage() {
			foreach(var item in ObjectStorage)
				serializationHelper.SerializedObjects.Remove(item);
			ObjectStorage.Clear();
			foreach(object item in serializationHelper.ReferencedObjects) {
				if(!serializationHelper.IsSerilizedObject(item)) {
					IObject obj = item is IObject ? (IObject)item : CreateObjectStorageItem(item);
					ObjectStorage.Add(obj);
				}
			}
		}
		public string GetReference(object obj) {
			return serializationHelper.GetReference(obj);
		}
		protected internal override void ResetProperty(DeserializeHelper helper, object obj, PropertyDescriptor prop, XtraSerializableProperty attr) {
			if(!ReferenceEquals(obj, RootObject))
				return;
			DefaultValueAttribute valueAttr = prop.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
			if(valueAttr != null)
				prop.SetValue(obj, valueAttr.Value);
			else {
				MethodInfo resetMethod = prop.ComponentType.GetMethod("Reset" + prop.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if(resetMethod != null)
					resetMethod.Invoke(obj, null);
			}
		}
		protected internal override bool InvokeShouldSerialize(SerializeHelper helper, object obj, PropertyDescriptor property) {
			if(base.InvokeShouldSerialize(helper, obj, property)) {
				serializationHelper.OnPropertySerialize(obj, property);
				if(property.GetValue(obj) == ObjectStorage)
					FillObjectStorage();
				return true;
			}
			return false;
		}
		protected internal override PropertyDescriptorCollection GetProperties(object obj, IXtraPropertyCollection store) {
			PropertyDescriptorCollection properties = base.GetProperties(obj, store);
			if(store != null && CanProcessObject(obj))
				deserializationHelper.ProcessProperties(obj, store);
			else if(CanProcessObject(obj))
				return serializationHelper.ProcessProperties(obj, properties);
			return properties;
		}
		protected internal override void AfterDeserializeRootObject() {
			RootObject.PerformAsLoad(() => {
				foreach(IObject item in ObjectStorage) {
					ObjectStorageInfo storageInfo = item as ObjectStorageInfo;
					if(storageInfo != null) {
						object obj = DeserializeObject(storageInfo, PrintingSystemXmlSerializer.ObjectConverterInstance);
						deserializationHelper.AddReferencedObject(obj, storageInfo.Ref);
					}
				}
				deserializationHelper.AssignReferecedObjects();
			});
		}
		static bool CanProcessObject(object obj) {
			return !(obj is ObjectStorageInfo);
		}
		object DeserializeObject(ObjectStorageInfo storageInfo, ObjectConverterImplementation converterInstance) {
			if(storageInfo.Content != XtraSerializer.NullValueString) {
				object result;
				if(SerializationService.DeserializeObject(storageInfo.Content, storageInfo.Type, out result, RootObject))
					return result;
				else if(storageInfo.Type == typeof(Type).FullName) {
					Type type = Type.GetType(storageInfo.Content)
						?? TypeResolver.GetType(storageInfo.Content);
					if(type != null) return type;
				} else {
					Type type = Type.GetType(storageInfo.Type)
						?? TypeResolver.GetType(storageInfo.Type);
					if(type != null) {
						if(type.IsPrimitive || type.Equals(typeof(string)) || type == typeof(System.Decimal) || typeof(Enum).IsAssignableFrom(type))
							return XmlXtraSerializer.XmlStringToObject(storageInfo.Content, type);
						else if(converterInstance.CanConvertFromString(type, storageInfo.Content))
							return converterInstance.ConvertFromString(type, storageInfo.Content);
					}
				}
			}
			return XtraSerializer.NullValueString;
		}
		ObjectStorageInfo CreateObjectStorageItem(object obj) {
			string result;
			if(SerializationService.SerializeObject(obj, out result, RootObject))
				return CreateObjectStorageItem(result, obj.GetType(), serializationHelper.GetReference(obj));
			else if(obj is Type)
				return CreateObjectStorageItem(((Type)obj).FullName, typeof(Type), serializationHelper.GetReference(obj));
			else {
				result = SerializeObject(obj, PrintingSystemXmlSerializer.ObjectConverterInstance);
				return CreateObjectStorageItem(result, obj.GetType(), serializationHelper.GetReference(obj));
			}
		}
		static string SerializeObject(object obj, ObjectConverterImplementation converterInstance) {
			return (obj.GetType().IsPrimitive || obj is string || obj.GetType() == typeof(System.Decimal) || obj is Enum) ? XmlXtraSerializer.XmlObjectToString(obj)
				: converterInstance.CanConvertToString(obj.GetType()) ? converterInstance.ConvertToString(obj)
					: XmlXtraSerializer.NullValueString;
		}
		static ObjectStorageInfo CreateObjectStorageItem(string result, Type type, string reference) {
			return new ObjectStorageInfo(type.FullName, result, reference);
		}
	}
}
