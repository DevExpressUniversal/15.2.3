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

using System.Collections.Generic;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using System.Windows;
using System;
using System.Reflection;
using System.Diagnostics;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core {
	public static class ThemedElementsDictionary {
		static AssemblyPriorityComparer assemblyComparer = new AssemblyPriorityComparer();
		class ThemeKeysDictionary : Dictionary<string, object> { }
		static Dictionary<string, ThemeKeysDictionary> cache = new Dictionary<string, ThemeKeysDictionary>();
		static List<string> forcedAssemblies = new List<string>();
		public static object GetCachedResourceKey(string themeName, string fullName) {
			ThemeKeysDictionary dict = GetDictionary(GetCorrectedThemeName(themeName));
			object value;
			return dict.TryGetValue(fullName, out value) ? value : null;
		}
		public static void ForceThemeKeysLoading(string themeName) {
			IEnumerable<Assembly> assemblies = AssemblyHelper.GetLoadedAssemblies();
			List<string> filteredAssemblyNames = Filter(assemblies);
			AddThemeAssembly(themeName, filteredAssemblyNames);
			filteredAssemblyNames.Sort(assemblyComparer);
			foreach (string asmName in filteredAssemblyNames) {
				ForceThemeKeysLoadingForAssembly(themeName, asmName);
			}
		}
		public static bool IsCustomThemeAssembly(Assembly assembly) {
			try {
			return AssemblyHelper.IsEntryAssembly(assembly) || AssemblyHelper.HasAttribute(assembly, typeof(DXThemeInfoAttribute));
			} catch {
				return false;
			}
		}
		static void AddThemeAssembly(string themeName, List<string> filteredAssemblyNames) {
			Theme theme = Theme.FindTheme(themeName);
			if (theme != null && theme.Assembly != null) {
				filteredAssemblyNames.Add(theme.Assembly.FullName);
			}
		}
		static List<string> Filter(IEnumerable<Assembly> assemblyNames) {
			List<string> filteredAssemblies = new List<string>();
			foreach (Assembly asm in assemblyNames) {
				bool isCustom = IsCustomThemeAssembly(asm);
				if (isCustom && !filteredAssemblies.Contains(asm.FullName)) {
					filteredAssemblies.Add(asm.FullName);
					continue;
				}
			}
			return filteredAssemblies;
		}
		public static void RegisterThemeType(string themeName, string fullName, object key) {
			ThemeKeysDictionary dictionary = GetDictionary(GetCorrectedThemeName(themeName));
			object oldKey;
			if (dictionary.TryGetValue(fullName, out oldKey)) {
				if (string.IsNullOrEmpty(themeName))
					return;
				if (assemblyComparer.Compare(GetAssemblyFullName(key), GetAssemblyFullName(oldKey)) >= 0) {
					return;
				}
			}
			dictionary[fullName] = key;
		}
		static string GetAssemblyFullName(object key) {
			Type type = key as Type;
			if (type != null) {
				return type.Assembly.FullName;
			}
			ResourceKey resourceKey = key as ResourceKey;
			if (resourceKey != null) {
				return resourceKey.Assembly.FullName;
			}
			return string.Empty;
		}
		public static void ForceThemeKeysLoadingForAssembly(string themeName, string assemblyName) {
			if (forcedAssemblies.Contains(assemblyName))
				return;
			TraceHelper.Write(ThemeManager.TraceSwitch,
				ThemeManager.MethodNameString, MethodInfo.GetCurrentMethod().Name,
				ThemeManager.ThemeNameTraceString, themeName,
				ThemeManager.AssemblyNameTraceString, assemblyName);
			forcedAssemblies.Add(assemblyName);
			Button initialButton = new Button();
			DefaultStyleKeyHelper.SetDefaultStyleKey(initialButton, new DefaultStyleThemeKeyExtension() {
				ThemeName = themeName,
				FullName = "System.Windows.Controls.NonexistantControl" + (assemblyName != null ? "+" + assemblyName : string.Empty),
				AssemblyName = assemblyName
			});
		}
		static ThemeKeysDictionary GetDictionary(string themeName) {
			if (!cache.ContainsKey(GetCorrectedThemeName(themeName))) {
				ThemeKeysDictionary dictionary = new ThemeKeysDictionary();
				cache[GetCorrectedThemeName(themeName)] = dictionary;
			}
			return cache[GetCorrectedThemeName(themeName)];
		}
		static string GetCorrectedThemeName(string themeName) {
			return themeName == null ? string.Empty : themeName;
		}
	}
}
