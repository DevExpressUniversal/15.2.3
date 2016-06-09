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
using System.Reflection;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP.AdoWrappers {
	public class ByObjectWrapper : IDisposable {
		internal static OLAPDataType GetDbType(object dataType) {
			if(dataType == null)
				return OLAPDataType.Empty;
			return (OLAPDataType)(ushort)dataType;
		}
		object instance;
		protected readonly Type instanceType;
		public ByObjectWrapper(object instance) {
			this.instance = instance;
			this.instanceType = instance.GetType();
		}
		public object Instance { 
			get { return instance; } 
			protected set { instance = value; } 
		}
		[ThreadStatic]
		static Dictionary<Type, Dictionary<string, MethodInfo>> cache;
		static Dictionary<Type, Dictionary<string, MethodInfo>> Cache {
			get {
				if(cache == null)
					cache = new Dictionary<Type, Dictionary<string, MethodInfo>>();
				return cache;
			}
		}
		internal static MethodInfo GetProperty(string property, Type type) {
			Dictionary<string, MethodInfo> dic = null;
			if(!Cache.TryGetValue(type, out dic)) {
				dic = new Dictionary<string, MethodInfo>();
				Cache.Add(type, dic);
			}
			MethodInfo info = null;
			if(!dic.TryGetValue(property, out info)) {
				info = type.GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).GetGetMethod(true);
				dic.Add(property, info);
			}
			return info;
		}
		protected object FindByStringIndex(string name) {
			return GetIndexedProperty(name, typeof(string), instance, instanceType);
		}
		protected object FindByIntIndex(int index) {
			return GetIndexedProperty(index, typeof(int), instance, instanceType);
		}
		[ThreadStatic]
		static Dictionary<Type, Dictionary<Type, Func<object, object, object>>> indexedCache;
		static Dictionary<Type, Dictionary<Type, Func<object, object, object>>> IndexedCache {
			get {
				if(indexedCache == null)
					indexedCache = new Dictionary<Type,Dictionary<Type, Func<object, object, object>>>();
				return indexedCache;
			}
		}
		protected static object GetIndexedProperty(object val, Type indexType, object instance, Type instanceType) {
			Dictionary<Type, Func<object, object, object>> c1 = null;
			if(!IndexedCache.TryGetValue(instanceType, out c1)) {
				c1 = new Dictionary<Type, Func<object, object, object>>();
				IndexedCache[instanceType] = c1;
			}
			Func<object, object, object> info = null;
			if(!c1.TryGetValue(indexType, out info)) { 
				info = new Getter(instanceType.GetProperty("Item", new Type[] { indexType }).GetGetMethod()).Get();
			}
			c1[indexType] = info;
			return info(instance, val);
		}
		class Getter {
			MethodInfo info;
			object[] getter = new object[1];
			public Getter(MethodInfo info) {
				this.info = info;
			}
			public Func<object, object, object> Get() {
				return (a, b) => { getter[0] = b; return info.Invoke(a, getter); };
			}
		}
		protected object GetPropertyValue(string propertyName, MethodInfo info, params object[] args) {
			try {
				return info.Invoke(instance, args);
			} catch(TargetInvocationException e) {
				throw BaseExceptionWrapper.TryGetInnerException(e);
			}
		}
		protected object GetPropertyValue(string propertyName) {
			return GetPropertyValue(propertyName, GetProperty(propertyName, instanceType), null);
		}
		protected void SetPropertyValue(string propertyName, object value) {
			try {
				instanceType.InvokeMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
					null, instance, new object[] { value });
			} catch(TargetInvocationException e) {
				throw BaseExceptionWrapper.TryGetInnerException(e);
			}
		}
		protected object InvokeMethod(string methodName, params object[] parameters) {
			try {
				return this.instanceType.InvokeMember(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
					null, Instance, parameters);
			} catch(TargetInvocationException e) {
				throw BaseExceptionWrapper.TryGetInnerException(e);
			}
		}
		#region IDisposable Members
		public void Dispose() {
			if(instance == null)
				return;
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			IDisposable disposableInstance = Instance as IDisposable;
			if(disposableInstance != null)
				disposableInstance.Dispose();
			instance = null;
		}
		#endregion
	}
	public class ByVersionWrapper : ByObjectWrapper {
		readonly AdomdVersion version;
		public AdomdVersion Version { get { return version; } }
		public static object CreateInstance(AdomdVersion version, string typeName) {
			Assembly asm = AdomdMetaGetter.LoadAdomdAssembly(version);
			Type type = asm.GetType(typeName);
			return Activator.CreateInstance(type);
		}
		public ByVersionWrapper(AdomdVersion version, string typeName)
			: base(CreateInstance(version, typeName)) {
			this.version = version;
		}
	}
}
