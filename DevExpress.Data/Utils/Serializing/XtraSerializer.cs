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
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using System.Reflection;
using System.Collections;
namespace DevExpress.Utils.Serializing {
	public interface ISupportXtraSerializer {
		void SaveLayoutToXml(string xmlFile);
		void SaveLayoutToRegistry(string path);
		void SaveLayoutToStream(Stream stream);
		void RestoreLayoutFromXml(string xmlFile);
		void RestoreLayoutFromStream(Stream stream);
		void RestoreLayoutFromRegistry(string path);
	}
	public class XtraSerializer {
		public XtraSerializer() {
			this.objectConverterImpl = ObjectConverter.Instance;
		}
		ICustomObjectConverter customObjectConverter;
		ObjectConverterImplementation objectConverterImpl;
		protected ObjectConverterImplementation ObjectConverterImpl {
			get { return objectConverterImpl; }
		}
		protected bool HasCustomObjectConverter {
			get { return CustomObjectConverter != null; }
		}
		public ICustomObjectConverter CustomObjectConverter {
			get { return customObjectConverter; }
			set {
				if(customObjectConverter == value)
					return;
				customObjectConverter = value;
				CustomObjectConverterChanged();
			}
		}
		void CustomObjectConverterChanged() {
			if(customObjectConverter == null)
				objectConverterImpl = ObjectConverter.Instance;
			else
				objectConverterImpl = new CustomObjectConverterImplementation(CustomObjectConverter);
		}
		protected static XtraObjectInfo[] CreateXtraObjectInfoArray(object obj) {
			return new XtraObjectInfo[] { new XtraObjectInfo(string.Empty, obj) };
		}
		public const string NullValueString = "~Xtra#NULL";
		public const string Base64Value = "~Xtra#Base64";
		public const string ArrayValue = "~Xtra#Array";
		protected virtual bool Serialize(string path, IXtraPropertyCollection props, string appName) { return false; }
		protected virtual IXtraPropertyCollection Deserialize(string path, string appName, IList objects) { return null; }
		protected virtual bool Serialize(Stream stream, IXtraPropertyCollection props, string appName) { return false; }
		protected virtual IXtraPropertyCollection Deserialize(Stream stream, string appName, IList objects) { return null; }
		public virtual bool CanUseStream { get { return false; } }
		public bool SerializeObject(object obj, object path, string appName) {
			return SerializeObject(obj, path, appName, OptionsLayoutBase.FullLayout);
		}
		public bool SerializeObject(object obj, object path, string appName, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				return SerializeObject(obj, stream, appName, options);
			else
				return SerializeObject(obj, path.ToString(), appName, options);
		}
		public bool SerializeObject(object obj, string path, string appName) {
			return SerializeObject(obj, path, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual bool SerializeObject(object obj, string path, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			SerializeHelper helper = CreateSerializeHelper(obj, true);
			return Serialize(path, helper.SerializeObject(obj, options), appName);
		}
		protected virtual SerializeHelper CreateSerializeHelper(object rootObj, bool useRootObj) {
			return useRootObj ? new SerializeHelper(rootObj) : new SerializeHelper();
		}
		protected virtual DeserializeHelper CreateDeserializeHelper(object rootObj, bool useRootObj) {
			return useRootObj ? new DeserializeHelper(rootObj) : new DeserializeHelper();
		}
		public void DeserializeObject(object obj, object path, string appName) {
			DeserializeObject(obj, path, appName, OptionsLayoutBase.FullLayout);
		}
		public void DeserializeObject(object obj, object path, string appName, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if(stream != null)
				DeserializeObject(obj, stream, appName, options);
			else
				DeserializeObject(obj, path.ToString(), appName, options);
		}
		public void DeserializeObject(object obj, string path, string appName) {
			DeserializeObject(obj, path, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual void DeserializeObject(object obj, string path, string appName, OptionsLayoutBase options) {
			DeserializeObject(obj, Deserialize(path, appName, CreateXtraObjectInfoArray(obj)), options);
		}
		public bool SerializeObject(object obj, Stream stream, string appName) {
			return SerializeObject(obj, stream, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual bool SerializeObject(object obj, Stream stream, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			SerializeHelper helper = CreateSerializeHelper(obj, true);
			return Serialize(stream, helper.SerializeObject(obj, options), appName);
		}
		public void DeserializeObject(object obj, Stream stream, string appName) {
			DeserializeObject(obj, stream, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual void DeserializeObject(object obj, Stream stream, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			DeserializeObject(obj, Deserialize(stream, appName, CreateXtraObjectInfoArray(obj)), options);
		}
		protected virtual void DeserializeObject(object obj, IXtraPropertyCollection store, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			if(store == null)
				return;
			XtraPropertyCollection coll = new XtraPropertyCollection();
			coll.AddRange(store);
			DeserializeHelper helper = CreateDeserializeHelper(obj, true);
			helper.ObjectConverterImpl = ObjectConverterImpl;
			helper.DeserializeObject(obj, coll, options);
			helper.AfterDeserializeRootObject();
		}
		public bool SerializeObjects(XtraObjectInfo[] objects, string path, string appName) {
			return SerializeObjects(objects, path, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual bool SerializeObjects(XtraObjectInfo[] objects, string path, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			SerializeHelper helper = CreateSerializeHelper(null, false);
			return Serialize(path, helper.SerializeObjects(objects, options), appName);
		}
		public void DeserializeObjects(XtraObjectInfo[] objects, string path, string appName) {
			DeserializeObjects(objects, path, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual void DeserializeObjects(IList objects, string path, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			DeserializeHelper helper = new DeserializeHelper();
			helper.ObjectConverterImpl = ObjectConverterImpl;
			helper.DeserializeObjects(objects, Deserialize(path, appName, objects), options);
		}
		public bool SerializeObjects(XtraObjectInfo[] objects, Stream stream, string appName) {
			return SerializeObjects(objects, stream, appName, OptionsLayoutBase.FullLayout);
		}
		public bool SerializeObjects(XtraObjectInfo[] objects, Stream stream, string appName, OptionsLayoutBase options) {
			return SerializeObjects(null, objects, stream, appName, options);
		}
		public virtual bool SerializeObjects(object rootObject, IList objects, Stream stream, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			SerializeHelper helper = CreateSerializeHelper(rootObject, true);
			bool result = Serialize(stream, helper.SerializeObjects(objects, options), appName);
			if(helper.RootSerializationObject != null)
				helper.RootSerializationObject.AfterSerialize();
			return result;
		}
		public void DeserializeObjects(XtraObjectInfo[] objects, Stream stream, string appName) {
			DeserializeObjects(objects, stream, appName, OptionsLayoutBase.FullLayout);
		}
		public virtual void DeserializeObjects(IList objects, Stream stream, string appName, OptionsLayoutBase options) {
			DeserializeObjects(null, objects, stream, appName, options);
		}
		public virtual void DeserializeObjects(object rootObject, IList objects, Stream stream, string appName, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			DeserializeHelper helper = CreateDeserializeHelper(rootObject, true);
			helper.ObjectConverterImpl = ObjectConverterImpl;
			helper.DeserializeObjects(objects, Deserialize(stream, appName, objects), options);
		}
	}
}
