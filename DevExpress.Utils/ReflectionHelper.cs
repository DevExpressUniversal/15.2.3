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
using System.Reflection;
namespace DevExpress.Utils.Helpers {
	public static class ReflectionHelper {
		public static void InvokeStaticMethod(string asmName, string typeName, string methodName, object[] args) {
			Assembly assembly = GetDomainAssembly(asmName);
			if(assembly == null) return;
			Type type = assembly.GetType(typeName);
			if(type == null) return;
			MethodInfo mi = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
			if(mi != null) mi.Invoke(null, args);
		}
		static Assembly GetDomainAssembly(string assemblyName) {
			Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
			for(int i = 0; i < asms.Length; i++) {
				AssemblyName asmName = SafeGetAssemblyName(asms[i]);
				if(assemblyName != null && string.Equals(asmName.Name, assemblyName, StringComparison.Ordinal)) return asms[i];
			}
			return null;
		}
		static AssemblyName SafeGetAssemblyName(Assembly asm) {
			AssemblyName assemblyName = null;
			try {
				assemblyName = asm.GetName();
			}
			catch { }
			return assemblyName;
		}
		public static void InvokeStaticMethod(string asmName, string typeName, string methodName) {
			InvokeStaticMethod(asmName, typeName, methodName, new object[] { });
		}
		static BindingFlags CheckStaticInstance(BindingFlags flags) {
			if((flags & BindingFlags.Static) != 0) flags &= ~BindingFlags.Instance;
			return flags;
		}
		public static object InvokeMethod<T>(object instance, string methodName, BindingFlags flags, params object[] args) {
			return InvokeMethod(typeof(T), instance, methodName, flags, args);
		}
		public static object InvokeMethod(Type type, object instance, string methodName, BindingFlags flags, params object[] args) {
			flags = CheckStaticInstance(BindingFlags.InvokeMethod | BindingFlags.Instance);
			var mi = type.GetMethod(methodName, flags);
			if(mi != null) return mi.Invoke(instance, args);
			return false;
		}
		public static object InvokeStaticMethod<T>(string methodName, BindingFlags flags, params object[] args) {
			return InvokeMethod<T>(null, methodName, flags | BindingFlags.Static, args);
		}
		public static object GetPropertyValue<T>(object instance, string propertyName, BindingFlags flags, params object[] index) {
			flags = CheckStaticInstance(BindingFlags.InvokeMethod | BindingFlags.Instance);
			var pi = typeof(T).GetProperty(propertyName, flags);
			if(pi != null) return pi.GetValue(instance, index);
			return null;
		}
		public static bool SetPropertyValue<T>(object instance, string propertyName, BindingFlags flags, object value, params object[] index) {
			flags = CheckStaticInstance(BindingFlags.InvokeMethod | BindingFlags.Instance);
			var pi = typeof(T).GetProperty(propertyName, flags);
			if(pi != null) {
				pi.SetValue(instance, value, index);
				return true;
			}
			return false;
		}
		public static object GetFieldValue<T>(object instance, string fieldName, BindingFlags flags) {
			flags = CheckStaticInstance(BindingFlags.InvokeMethod | BindingFlags.Instance);
			var fi = typeof(T).GetField(fieldName, flags);
			if(fi != null) return fi.GetValue(instance); ;
			return null;
		}
		public static object GetStaticFieldValue<T>(string fieldName, BindingFlags flags) {
			return GetFieldValue<T>(null, fieldName, flags | BindingFlags.Static);
		}
		public static bool SetFieldValue<T>(object instance, string fieldName, object value, BindingFlags flags) {
			flags = CheckStaticInstance(BindingFlags.InvokeMethod | BindingFlags.Instance);
			var fi = typeof(T).GetField(fieldName, flags);
			if(fi != null) {
				fi.SetValue(instance, value);
				return true;
			}
			return false;
		}
	}
}
