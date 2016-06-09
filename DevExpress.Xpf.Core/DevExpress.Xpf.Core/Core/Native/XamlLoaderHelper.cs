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
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using DevExpress.Utils;
namespace DevExpress.Xpf.Utils.Native {
	public static class XamlLoaderHelper<T> where T : class {
		public static Assembly EntryAssembly { get { return AssemblyHelper.EntryAssembly; } }
		[ThreadStatic]
		static Dictionary<Uri, T> cache;
		public static T LoadFromResource(Uri path) {
			try {
				if(typeof(T) == typeof(ResourceDictionary)) {
					if(cache == null)
						cache = new Dictionary<Uri, T>();
					T res;
					if(!cache.TryGetValue(path, out res)) {
						res = Application.LoadComponent(path) as T;
						cache.Add(path, res);
					}
					return ResourceDictionaryHelper.CloneResourceDictionary(res as ResourceDictionary) as T;
				}
				else
					return Application.LoadComponent(path) as T;
			}
			catch { }
			return null;
		}
		public static T LoadFromApplicationResources(string path) {
			return LoadFromResource(new Uri(path, UriKind.RelativeOrAbsolute));
		}
		public static T LoadFromResource(Type type, string source) {
			string path = "/" + GetAssemblyName(type).Replace(" ", "") + ";component/" + source;
			if(!path.EndsWith(".xaml"))
				path += ".xaml";
			return LoadFromResource(new Uri(path, UriKind.RelativeOrAbsolute));
		}
		public static T LoadFromAssembly(Assembly asm, string source) {
			string path = "/" + asm.FullName.Replace(" ", "") + ";component/" + source;
			if(!path.EndsWith(".xaml"))
				path += ".xaml";
			return LoadFromResource(new Uri(path, UriKind.RelativeOrAbsolute));
		}
		public static T LoadFromAssembly(string assemblyName, string source) {
			Assembly[] asmList = AppDomain.CurrentDomain.GetAssemblies();
			Assembly curr = null;
			foreach(Assembly asm in asmList) {
				if(asm.FullName.Contains(assemblyName)) {
					curr = asm;
					break;
				}
			}
			if(curr == null) return null;
			return LoadFromAssembly(curr, source);
		}
		public static T LoadFromFile(string source) {
			try {
				using(XmlReader reader = new XmlTextReader(source)) {
					return XamlReader.Load(reader) as T;
				}
			}
			catch { }
			return null;
		}
		static Dictionary<Type, String> AssemblyNameCache = new Dictionary<Type, string>();
		static string GetAssemblyName(Type type) {
			if(AssemblyNameCache.ContainsKey(type)) return AssemblyNameCache[type];
			string result = string.Empty;
			object[] attributes = type.GetCustomAttributes(typeof(SupportDXThemeAttribute), true);
			if(attributes != null && attributes.Length > 0) {
				Type baseType = ((SupportDXThemeAttribute)attributes[0]).TypeInAssembly;
				if(baseType != null)
					result = baseType.Assembly.FullName;
				else
					result = type.Assembly.FullName;
			}
			else result = type.Assembly.FullName;
			AssemblyNameCache[type] = result;
			return result;
		}
	}
}
