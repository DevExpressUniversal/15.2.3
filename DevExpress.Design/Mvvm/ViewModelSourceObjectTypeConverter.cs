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

namespace DevExpress.Utils.MVVM.Design {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Globalization;
	using System.Linq;
	using System.Windows.Forms;
	using System.Windows.Forms.Design;
	public class ViewModelSourceObjectTypeConverter : TypeListConverter {
		#region static
		static ViewModelSourceObjectTypeConverter() {
			HideNamespaces = true;
			ShowAllTypes = false;
		}
		public static bool HideNamespaces { get; set; }
		public static bool ShowAllTypes { get; set; }
		#endregion static
		public const string None = "(none)";
		public ViewModelSourceObjectTypeConverter()
			: base(new Type[0]) {
		}
		public virtual bool CanHideType(Type type) {
			return
				(!type.Namespace.Contains(".ViewModel") && type.Namespace != "ViewModel") ||
				type.Name.EndsWith("Command") ||
				type.Name.EndsWith("Service") ||
				type.Name.EndsWith("Helper") ||
				type.Name.EndsWith("Message") ||
				type.Name.EndsWith("State") ||
				type.Name.EndsWith("Description");
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		SortedList<string, Type> typesCache;
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList<string, Type> list = new SortedList<string, Type>();
			list.Add(None, null);
			try {
				string rootNamespace = GetRootNamespace(context);
				foreach(Type t in GetAllTypes(context)) {
					if(!t.IsPublic || t.IsValueType || t.IsSealed || t.IsInterface)
						continue;
					if(t.ContainsGenericParameters)
						continue;
					if(typeof(Exception).IsAssignableFrom(t) || typeof(Attribute).IsAssignableFrom(t))
						continue;
					if(typeof(Delegate).IsAssignableFrom(t) || typeof(EventArgs).IsAssignableFrom(t))
						continue;
					if(typeof(BehaviorBase).IsAssignableFrom(t))
						continue;
					if(typeof(Control).IsAssignableFrom(t) || typeof(Component).IsAssignableFrom(t))
						continue;
					if(!ShowAllTypes) {
						if(CanHideType(t) && (t.Namespace != rootNamespace))
							continue;
					}
					if(t.Namespace.StartsWith("System.Data.Entity."))
						continue;
					string key = HideNamespaces ? t.Name : t.FullName;
					if(list.ContainsKey(key))
						continue;
					list.Add(key, t);
				}
			}
			catch(Exception e) {
				IUIService s = (IUIService)context.GetService(typeof(IUIService));
				if(s != null) s.ShowError(e);
			}
			typesCache = list;
			return new StandardValuesCollection(list.Values.ToArray());
		}
		static string GetRootNamespace(ITypeDescriptorContext context) {
			string rootNamespace = string.Empty;
			IDesignerHost host = context.Container as IDesignerHost;
			if(host != null && host.RootComponent != null)
				rootNamespace = GetRooNamespace(host);
			return rootNamespace;
		}
		static string GetRooNamespace(IDesignerHost host) {
			return host.RootComponentClassName.Substring(0, host.RootComponentClassName.LastIndexOf('.'));
		}
		static IEnumerable<Type> GetAllTypes(ITypeDescriptorContext context) {
			ITypeDiscoveryService typeDiscovery = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
			return ((typeDiscovery != null) ? typeDiscovery.GetTypes(typeof(object), true).OfType<Type>() : Enumerable.Empty<Type>())
				.Concat(TypesHelper.GetTypesFromSolution(context));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			string typeName = val as string;
			if(typeName != null) {
				if(typeName == None)
					return null;
				Type type;
				if(typesCache != null && typesCache.TryGetValue(typeName, out type))
					return type;
				type = Type.GetType(typeName, false) ??
					TypesHelper.GetTypeFromResolutionService(context, typeName) ??
					TypesHelper.GetTypeFromRootNamespace(context, typeName) ??
					TypesHelper.GetTypeFromRootAssembly(context, typeName) ??
					TypesHelper.GetTypeFromSolutionAssembly(context, typeName);
				if(type != null)
					return type;
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if(destType == typeof(string)) {
				if(val == null)
					return None;
				if(val is Type)
					return HideNamespaces ? ((Type)val).Name : ((Type)val).FullName;
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		static class TypesHelper {
			internal static IEnumerable<Type> GetTypesFromSolution(ITypeDescriptorContext context) {
				List<Type> types = new List<Type>();
				IDesignerHost host = context.Container as IDesignerHost;
				if(host != null) {
					var projectAssemblies = GetProjectAssemblies(host);
					for(int i = 0; i < projectAssemblies.Length; i++) {
						var asmTypes = GetTypesFromAssembly(projectAssemblies[i]);
						for(int t = 0; t < asmTypes.Length; t++)
							yield return asmTypes[t];
					}
				}
			}
			internal static Type GetTypeFromResolutionService(ITypeDescriptorContext context, string typeName) {
				IDesignerHost host = context.Container as IDesignerHost;
				try {
					return DevExpress.Utils.Design.ProjectHelper.TryLoadType(host, typeName);
				}
				catch { return null; }
			}
			internal static Type GetTypeFromRootNamespace(ITypeDescriptorContext context, string typeName) {
				IDesignerHost host = context.Container as IDesignerHost;
				if(host != null)
					return host.GetType(typeName) ?? host.GetType(GetRooNamespace(host) + "." + typeName);
				return null;
			}
			internal static Type GetTypeFromRootAssembly(ITypeDescriptorContext context, string typeName) {
				IDesignerHost host = context.Container as IDesignerHost;
				if(host != null) {
					Type rootType = host.GetType(host.RootComponentClassName);
					if(rootType != null)
						return GetTypeFromAssembly(rootType.Assembly, typeName);
				}
				return null;
			}
			internal static Type GetTypeFromSolutionAssembly(ITypeDescriptorContext context, string typeName) {
				IDesignerHost host = context.Container as IDesignerHost;
				if(host != null) {
					var projectAssemblies = GetProjectAssemblies(host);
					for(int i = 0; i < projectAssemblies.Length; i++) {
						var type = GetTypeFromAssembly(projectAssemblies[i], typeName);
						if(type != null)
							return type;
					}
				}
				return null;
			}
			static Type GetTypeFromAssembly(System.Reflection.Assembly assembly, string typeName) {
				try {
					if(assembly != null)
						return GetType(typeName, assembly.GetTypes());
					return null;
				}
				catch(System.Reflection.ReflectionTypeLoadException e) {
					return GetType(typeName, e.Types);
				}
			}
			static Type[] GetTypesFromAssembly(System.Reflection.Assembly assembly) {
				try {
					if(assembly != null)
						return assembly.GetTypes();
					return Type.EmptyTypes;
				}
				catch(System.Reflection.ReflectionTypeLoadException e) {
					return e.Types;
				}
			}
			static Type GetType(string typeName, Type[] types) {
				for(int i = 0; i < types.Length; i++) {
					if(types[i].Name == typeName || types[i].FullName == typeName)
						return types[i];
				}
				return null;
			}
			static System.Reflection.Assembly[] GetProjectAssemblies(IServiceProvider serviceProvider) {
				try {
					string[] dteProjectAssemblyNames = GetProgectAssemblyNames(serviceProvider);
					return GetProjectAssemblies(dteProjectAssemblyNames);
				}
				catch { return new System.Reflection.Assembly[] { }; }
			}
			static string[] GetProgectAssemblyNames(IServiceProvider serviceProvider) {
				string[] dteProjectAssemblies = new string[] { };
				EnvDTE.DTE dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
				if(dte != null) {
					EnvDTE.Project[] projects = Utils.Design.DTEHelper.GetProjects(dte);
					dteProjectAssemblies = new string[projects.Length];
					for(int i = 0; i < projects.Length; i++) {
						if(projects[i].Properties == null) continue;
						try {
							object objAssemblyName = projects[i].Properties.Item("AssemblyName").Value;
							if(objAssemblyName != null)
								dteProjectAssemblies[i] = objAssemblyName.ToString();
						}
						catch { }
					}
				}
				return dteProjectAssemblies;
			}
			static System.Reflection.Assembly[] GetProjectAssemblies(string[] dteProjectAssemblies) {
				var projectAssemblies = new System.Reflection.Assembly[dteProjectAssemblies.Length];
				try {
					var assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach(var assembly in assemblies) {
						int index = Array.IndexOf(dteProjectAssemblies, assembly.GetName().Name);
						if(index == -1) continue;
						projectAssemblies[index] = assembly;
					}
				}
				catch { }
				return projectAssemblies;
			}
		}
	}
	public class ModelObjectTypeConverter : ViewModelSourceObjectTypeConverter {
		public override bool CanHideType(Type type) {
			return
				(!type.Namespace.Contains(".Model") && type.Namespace != "Model") ||
				type.Name.EndsWith("ViewModel") ||
				type.Name.EndsWith("Command") ||
				type.Name.EndsWith("Service") ||
				type.Name.EndsWith("Helper") ||
				type.Name.EndsWith("Message") ||
				type.Name.EndsWith("State") ||
				type.Name.EndsWith("Description");
		}
	}
}
