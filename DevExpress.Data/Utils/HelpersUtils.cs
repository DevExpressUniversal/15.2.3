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
using System.Reflection;
using System.Security;
using Microsoft.Win32;
using System.Collections.ObjectModel;
namespace DevExpress.Data.Utils {
#if !CF && !SL && !DXPORTABLE
	public static class AssemblyCache {
		static Dictionary<string, Assembly> loadedAssemblies;
		static Dictionary<string, Assembly> LoadedAssemblies {
			get {
				if(loadedAssemblies == null)
					loadedAssemblies = new Dictionary<string, Assembly>();
				return loadedAssemblies;
			}
		}
		static bool loading;
		static readonly object padlock = new object();
		public static Assembly LoadDXAssembly(string name) {
			try {
				AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
				assemName.CodeBase = string.Empty;
				assemName.Name = name;
				return Load(assemName);
			} catch {
				return null;
			}
		}
		public static Assembly Load(AssemblyName assemName) {
			lock(padlock) {
				Assembly value;
				if(LoadedAssemblies.TryGetValue(assemName.Name, out value))
					return value;
				value = Assembly.Load(assemName);
				LoadedAssemblies[assemName.Name] = value;
				return value;
			}
		}
		public static Assembly LoadWithPartialName(string partialName) {
			lock(padlock) {
				if(loading)
					return null;
				Assembly value;
				if(LoadedAssemblies.TryGetValue(partialName, out value))
					return value;
				try {
					loading = true;
					value = Helpers.LoadWithPartialName(partialName);
					LoadedAssemblies[partialName] = value;
					return value;
				} finally {
					loading = false;
				}
			}
		}
		public static void Clear() {
			lock(padlock) {
				LoadedAssemblies.Clear();
			}
		}
	}
#endif 
	public class Helpers {
#if !SL && !DXPORTABLE
		public static bool WaitOne(System.Threading.WaitHandle waitHandle, int millisecondsTimeout) {
			return waitHandle.WaitOne(millisecondsTimeout, false);
		}
#else
		public static bool WaitOne(System.Threading.EventWaitHandle waitHandle, int millisecondsTimeout){
			return waitHandle.WaitOne(millisecondsTimeout);
		}
#endif
		readonly static Dictionary<Type, object> enumGenericValuesCache = new Dictionary<Type, object>();
		readonly static Dictionary<Type, List<Enum>> enumValuesCache = new Dictionary<Type, List<Enum>>();
		public static T[] GetEnumValues<T>() {
			return GetEnumValues<T>(true);
		}
		public static T[] GetEnumValues<T>(bool useCache) {
			Type enumType = typeof(T);
			if(useCache) {
				lock(enumGenericValuesCache) {
					object cacheItem;
					if(enumGenericValuesCache.TryGetValue(enumType, out cacheItem)) {
						return ((List<T>)cacheItem).ToArray();
					}
				}
			}
			List<T> enumValues = new List<T>();
#if !SL && !CF
				foreach(T item in Enum.GetValues(enumType)) {
					enumValues.Add(item);
				}
#else
			foreach(FieldInfo fi in enumType.GetFields(BindingFlags.Static | BindingFlags.Public)) {
				enumValues.Add((T)Enum.Parse(enumType, fi.Name, false));
			}
#endif
			if(useCache) {
				lock(enumGenericValuesCache) {
					enumGenericValuesCache[enumType] = enumValues;
				}
			}
			return enumValues.ToArray();
		}
		public static Enum[] GetEnumValues(Type enumType) {
			return GetEnumValues(enumType, true);
		}
		public static Enum[] GetEnumValues(Type enumType, bool useCache) {
			if(useCache) {
				lock(enumValuesCache) {
					List<Enum> cacheItem;
					if(enumValuesCache.TryGetValue(enumType, out cacheItem)) {
						return cacheItem.ToArray();
					}
				}
			}
			List<Enum> enumValues = new List<Enum>();
#if !SL && !CF
			foreach(Enum item in Enum.GetValues(enumType)) {
				enumValues.Add(item);
			}
#else
			foreach(FieldInfo fi in enumType.GetFields(BindingFlags.Static | BindingFlags.Public)) {
				enumValues.Add((Enum)Enum.Parse(enumType, fi.Name, false));
			}
#endif
			if(useCache) {
				lock(enumValuesCache) {
					enumValuesCache[enumType] = enumValues;
				}
			}
			return enumValues.ToArray();
		}
		public static string[] GetEnumNames(Type enumType) {
#if !SL && !CF
			return Enum.GetNames(enumType);
#else
			List<string> enumNames = new List<string>();
			foreach(FieldInfo fi in enumType.GetFields(BindingFlags.Static | BindingFlags.Public)) {
				enumNames.Add(fi.Name);
			}
			return enumNames.ToArray();
#endif
		}
#if !CF && !SL && !DXPORTABLE
		static Version GetFrameworkVersion_3_X(string keyName, string installValueName) {
			Version version = null;
			try {
				object install = Registry.GetValue(keyName, installValueName, -1);
				if (install is int && (int)install == 1) {
					string ver = (string)Registry.GetValue(keyName, "Version", string.Empty);
					version = new Version(ver);
				}
			}
			catch (SecurityException) {
				return new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
			}
			return version;
		}
#if !SL
		public static Assembly LoadWithPartialName(string partialName, System.Security.Policy.Evidence securityEvidence) {
#pragma warning disable 618
			return Assembly.LoadWithPartialName(partialName, securityEvidence);
#pragma warning restore 618
		}
#endif
		public static Assembly LoadWithPartialName(string partialName) {
#pragma warning disable 618
			return Assembly.LoadWithPartialName(partialName);
#pragma warning restore 618
		}
		public static Version GetFrameworkVersion() {
			if (Environment.Version.Major != 2)
				return Environment.Version;
			Version version = GetFrameworkVersion_3_X(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5\", "Install");
			if (version == null) {
				version = GetFrameworkVersion_3_X(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup\", "InstallSuccess");
				if (version == null)
					version = Environment.Version;
			}
			return version;
		}	
#endif
	}
}
namespace DevExpress.Xpo.Helpers {
	public static class XPTypeActivator {
		public static Type GetType(string assemblyName, string typeName) {
			Assembly assembly = null;
#if CF || SL || DXPORTABLE
						try {
#if DXPORTABLE
				assembly = Assembly.Load(new AssemblyName(assemblyName));
#else
				assembly = Assembly.Load(assemblyName);
#endif
						} catch {
								return null;
			}
#else
			string name = assemblyName + ",";
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
				if(a.FullName.StartsWith(name)) {
					assembly = a;
					break;
				}
			}
			if(assembly == null)
				assembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
#endif
				return assembly == null ? null : assembly.GetType(typeName);
		}
		public static void AuxRegistrationInvoker(string auxAssembly, string auxClass, string registerPublicStaticParameterlessMethodName) {
			try {
				Type bonusProvidersRegistrator = XPTypeActivator.GetType(auxAssembly, auxClass);
				if(bonusProvidersRegistrator != null) {
					System.Reflection.MethodInfo registerer = bonusProvidersRegistrator.GetMethod(registerPublicStaticParameterlessMethodName, new Type[0]);
					if(registerer != null) {
						registerer.Invoke(null, null);
					}
				}
			} catch { }
		}
	}
}
