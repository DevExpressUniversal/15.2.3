#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection.Emit;
using System.Text;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Utils.Reflection {
	public static class TypeHelper {
		private static readonly Dictionary<Type, TypeData> cache;
		static TypeHelper() {
			cache = new Dictionary<Type, TypeData>();
		}
		private static TypeData GetData(Type type) {
			TypeData data;
			if(!cache.TryGetValue(type, out data)) {
				lock(cache) {
					if(!cache.TryGetValue(type, out data)) {
						data = new TypeData(type);
						cache.Add(type, data);
					}
				}
			}
			return data;
		}
		public static Object CreateInstance(Type type, params Object[] args) {
			return GetData(type).CreateInstance(args);
		}
		public static Boolean CanCreateInstance(Type type, params Type[] argumentTypes) {
			return GetData(type).CanCreateInstance(argumentTypes);
		}
		public static Type[] GetInterfaces(Type type) {
			return type.GetInterfaces();
		}
		public static Boolean IsObsolete(Type type) {
			return GetData(type).IsObsolete;
		}
		public static Boolean IsNullable(Type type) {
			return GetData(type).IsNullable;
		}
		public static Type GetUnderlyingType(Type type) {
			return GetData(type).UnderlyingType;
		}
		public static Boolean ContainsProperty(Type type, String propertyName) {
			return GetData(type).ContainsProperty(propertyName);
		}
		public static PropertyInfo GetProperty(Type type, String propertyName) {
			return GetData(type).GetProperty(propertyName);
		}
		public static PropertyInfo[] GetProperties(Type type) {
			return GetData(type).GetProperties();
		}
		public static Object GetPropertyValue(Object obj, String propertyName) {
			return GetData(obj.GetType()).GetPropertyValue(obj, propertyName);
		}
		public static void SetPropertyValue(Object obj, String propertyName, Object value) {
			GetData(obj.GetType()).SetPropertyValue(obj, propertyName, value);
		}
	}
	sealed class TypeData {
		private delegate Object CreateInstanceDelegate();
		private readonly Type type;
		private Dictionary<String, PropertyInfo> propertyCache;
		private readonly Object propertyCacheLocker;
		private Boolean? isObsolete;
		private Boolean? isNullable;
		private Type underlyingType;
		private Boolean isUnderlyingTypeInitialized;
		private CreateInstanceDelegate createInstance;
		private readonly Object createInstanceLocker;
		private PropertyInfo GetPropertyInfo(String propertyName) {
			PropertyInfo propertyInfo = FindPropertyInfo(propertyName);
			if(propertyInfo == null) {
				String message = String.Format("The '{0}' type does not contain '{1}' public property.", type.FullName, propertyName);
				throw new ArgumentException(message, "propertyName");
			}
			return propertyInfo;
		}
		private PropertyInfo FindPropertyInfo(String propertyName) {
			if(!String.IsNullOrEmpty(propertyName)) {
				EnsurePropertyCache();
				PropertyInfo result;
				if(propertyCache.TryGetValue(propertyName, out result)) {
					return result;
				}
			}
			return null;
		}
		private void EnsurePropertyCache() {
			if(propertyCache == null) {
				lock(propertyCacheLocker) {
					if(propertyCache == null) {
						propertyCache = new Dictionary<String, PropertyInfo>();
						foreach(PropertyInfo propertyInfo in type.GetProperties()) {
							if(!IsIndexedProperty(propertyInfo) && !propertyCache.ContainsKey(propertyInfo.Name)) {
								propertyCache.Add(propertyInfo.Name, propertyInfo);
							}
						}
					}
				}
			}
		}
		private static Boolean IsIndexedProperty(PropertyInfo propertyInfo) {
			ParameterInfo[] parms = propertyInfo.GetIndexParameters();
			return parms.Length > 0;
		}
		private CreateInstanceDelegate GetCreateInstanceDelegate(Type type) {
			CreateInstanceDelegate result = null;
			if(type.IsVisible) {
				ConstructorInfo constrInfo = type.GetConstructor(Type.EmptyTypes);
				if(constrInfo != null) {
					String methodName = "Create_" + type.Name;
					DynamicMethod method = new DynamicMethod(methodName, typeof(Object), null);
					ILGenerator ilGenerator = method.GetILGenerator();
					ilGenerator.Emit(OpCodes.Newobj, constrInfo);
					ilGenerator.Emit(OpCodes.Ret);
					result = (CreateInstanceDelegate)method.CreateDelegate(typeof(CreateInstanceDelegate));
				}
			}
			if(result == null) {
				result = DefaultCreateInstance;
			}
			return result;
		}
		private Object DefaultCreateInstance() {
			return Activator.CreateInstance(type);
		}
		public TypeData(Type type) {
			Guard.ArgumentNotNull(type, "type");
			this.type = type;
			propertyCacheLocker = new Object();
			createInstanceLocker = new Object();
		}
		public Object CreateInstance(params Object[] args) {
			try {
				if(args != null && args.Length == 0) {
					if(createInstance == null) {
						lock(createInstanceLocker) {
							if(createInstance == null) {
								createInstance = GetCreateInstanceDelegate(type);
							}
						}
					}
					return createInstance();
				}
				else {
					return Activator.CreateInstance(type, args);
				}
			}
			catch(Exception e) {
				if(e is TargetInvocationException) {
					e = e.InnerException;
				}
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.AppendFormat("Unable to create an instance of the \"{0}\" type.\nReason: \"{1}\"", type.FullName, e.Message);
				if(e is MissingMethodException) {
					List<String> argTypes = new List<String>();
					foreach(Object arg in args) {
						argTypes.Add(arg == null ? "unknown value type" : arg.GetType().FullName);
					}
					errorMessage.AppendFormat("\n .ctor({0}) is absent.", String.Join(", ", argTypes.ToArray()));
				}
				if(e.InnerException != null) {
					errorMessage.AppendFormat(", \ninner exception: \"{0}\"", e.InnerException.Message);
					errorMessage.AppendFormat(", \ninner exception stack trace: \"{0}\"\nend inner exception stack trace\n", e.InnerException.StackTrace);
				}
				else {
					errorMessage.AppendFormat(", \nstack trace: \"{0}\"\nend stack trace\n", e.StackTrace);
				}
				throw new ObjectCreatingException(type, errorMessage.ToString(), e);
			}
		}
		public Boolean CanCreateInstance(params Type[] argumentTypes) {
			Boolean result = false;
			if(type.IsVisible && !type.IsAbstract && !type.ContainsGenericParameters) {
				result = type.GetConstructor(argumentTypes) != null;
			}
			return result;
		}
		public Boolean ContainsProperty(String propertyName) {
			return FindPropertyInfo(propertyName) != null;
		}
		public PropertyInfo GetProperty(String propertyName) {
			return GetPropertyInfo(propertyName);
		}
		public PropertyInfo[] GetProperties() {
			EnsurePropertyCache();
			PropertyInfo[] result = new PropertyInfo[propertyCache.Count];
			propertyCache.Values.CopyTo(result, 0);
			return result;
		}
		public Object GetPropertyValue(Object obj, String propertyName) {
			return GetPropertyInfo(propertyName).GetValue(obj, null);
		}
		public void SetPropertyValue(Object obj, String propertyName, Object value) {
			GetPropertyInfo(propertyName).SetValue(obj, value, null);
		}
		public Boolean IsObsolete {
			get {
				if(!isObsolete.HasValue) {
					isObsolete = type.IsDefined(typeof(ObsoleteAttribute), false);
				}
				return isObsolete.Value;
			}
		}
		public Boolean IsNullable {
			get {
				if(!isNullable.HasValue) {
					isNullable = UnderlyingType != null;
				}
				return isNullable.Value;
			}
		}
		public Type UnderlyingType {
			get {
				if(!isUnderlyingTypeInitialized) {
					underlyingType = Nullable.GetUnderlyingType(type);
					isUnderlyingTypeInitialized = true;
				}
				return underlyingType;
			}
		}
	}
}
