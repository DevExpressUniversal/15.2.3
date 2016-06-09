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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Styles {
	static class StyleHelper {
		static System.Reflection.Assembly TargetAssembly = typeof(StyleHelper).GetAssembly();
		static IDictionary<string, Type> typeCache = new Dictionary<string, Type>();
		public static Type ResolveType(string typeName) {
			Type result;
			if(!typeCache.TryGetValue(typeName, out result)) {
				result = TargetAssembly.GetType(typeName, true, false);
				typeCache.Add(typeName, result);
			}
			return result;
		}
		public static PropertyDescriptor GetProperty(string path, PropertyDescriptorCollection properties) {
			if(string.IsNullOrEmpty(path)) return null;
			int pathSeparatorPos = path.IndexOf('.');
			if(pathSeparatorPos > 0) {
				string rootPath = path.Substring(0, pathSeparatorPos);
				path = path.Substring(pathSeparatorPos + 1);
				PropertyDescriptor rootDescriptor = properties[rootPath];
				if(rootDescriptor != null) 
					return GetProperty(path, rootDescriptor.GetChildProperties());
				return GetCollectionItemProperty(path, properties, rootPath);
			}
			return properties[path];
		}
		static PropertyDescriptor GetCollectionDescriptor(string path, PropertyDescriptorCollection properties, out int index) {
			index = -1;
			int openBracketPos = path.IndexOf('[');
			if(openBracketPos > 0) {
				string collectionPath = path.Substring(0, openBracketPos);
				int closeBracketPos = path.IndexOf(']');
				int indexPos=openBracketPos+1;
				string indexStr = path.Substring(indexPos, closeBracketPos - indexPos);
				int.TryParse(indexStr, out index);
				return properties[collectionPath];
			}
			return null;
		}
		static PropertyDescriptor GetCollectionItemProperty(string path, PropertyDescriptorCollection properties, string rootPath) {
			int index;
			PropertyDescriptor collectionDescriptor = GetCollectionDescriptor(rootPath, properties, out index);
			if(collectionDescriptor != null) {
				Type collectionItemType = GetCollectionItemType(collectionDescriptor.PropertyType);
				if(collectionItemType != null) {
					PropertyDescriptorCollection itemProperties = TypeDescriptor.GetProperties((Type)collectionItemType);
					return GetProperty(path, itemProperties);
				}
			}
			return null;
		}
		static Type GetCollectionItemType(Type collectionType) {
			if(collectionType == null) return null;
			if(collectionType.IsGenericType()) {
				Type[] args = collectionType.GetGenericArguments();
				if(args.Length == 1)
					return args[0];
				return null;
			}
			return GetCollectionItemType(collectionType.GetBaseType());
		}
		public static void SetValue(object target, string path, object value) {
			if(target == null || string.IsNullOrEmpty(path)) return;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(target);
			int pathSeparatorPos = path.IndexOf('.');
			if(pathSeparatorPos > 0) {
				string rootPath = path.Substring(0, pathSeparatorPos);
				path = path.Substring(pathSeparatorPos + 1);
				PropertyDescriptor rootDescriptor = properties[rootPath];
				if(rootDescriptor != null) {
					target = rootDescriptor.GetValue(target);
					SetValue(target, path, value);
				}
				int index;
				PropertyDescriptor collectionDescriptor = GetCollectionDescriptor(rootPath, properties, out index);
				if(collectionDescriptor != null) {
					target = collectionDescriptor.GetValue(target);
					IList list = target as IList;
					if(list != null) {
						target = (index >= 0 && index < list.Count) ? list[index] : null;
						SetValue(target, path, value);
					}
				}
			}
			else SetValueCore(target, properties[path], value);
		}
		static void SetValueCore(object target, PropertyDescriptor pd, object value) {
			if(target != null && pd != null && !pd.IsReadOnly) {
				if(value is string && pd.PropertyType != typeof(string))
					value = ObjectConverter.Instance.StringToObject((string)value, pd.PropertyType);
				pd.SetValue(target, value);
			}
		}
	}
	public interface IStyleMap {
		bool IsStyleProperty(string propertyName, Type type);
		bool IsStyleProperty(string propertyName, string propertyPath);
		bool TryGetType(string typeName, out Type type);
		bool TryGetKnownTypePropertyValue(string path, IXtraPropertyCollection store, out object value);
		IDictionary<Type, string[]> AllowedProperties { get;}
		IDictionary<string, string> PropertyPathAliases { get;}
		void EnsureDefaultStyles(StyleCollection styles);
	}
	public class StyleReader {
		IStyleMap StyleMap;
		public StyleReader(IStyleMap styleMap) {
			this.StyleMap = styleMap;
		}
		public void Read(IXtraPropertyCollection store, StyleCollection styles) {
			if(StyleMap == null || store == null) return;
			ReadLevel(store, styles, StyleMap);
			StyleMap.EnsureDefaultStyles(styles);
		}
		void ReadLevel(IXtraPropertyCollection store, StyleCollection styles, IStyleMap styleMap) {
			Style style = CreateStyle(styles, store, styleMap);
			foreach(XtraPropertyInfo info in store) {
				string property = info.Name;
				if(style != null && styleMap.IsStyleProperty(property, style.TargetType)) {
					if(!info.HasChildren)
						style.Setters.Add(property, info.Value);
					else ReadChildPropertyLevel(info, info.ChildProperties, style, property, styleMap);
					continue;
				}
				if(info.HasChildren)
					ReadLevel(info.ChildProperties, styles, styleMap);
			}
		}
		void ReadChildPropertyLevel(XtraPropertyInfo parentInfo, IXtraPropertyCollection store, Style style, string rootPath, IStyleMap styleMap) {
			object value;
			foreach(XtraPropertyInfo info in store) {
				string property = info.Name;
				string path = string.Format("{0}.{1}", rootPath, info.Name);
				if(parentInfo.IsKey && parentInfo.Value is int) {
					string indexStr = info.Name.Replace("Item", string.Empty);
					int index;
					if(int.TryParse(indexStr, out index)) {
						path = string.Format("{0}[{1}]", rootPath, --index);
						ReadChildPropertyLevel(info, info.ChildProperties, style, path, styleMap);
					}
				}
				if(styleMap.IsStyleProperty(property, rootPath)) {
					if(!info.HasChildren) {
						if(styleMap.TryGetKnownTypePropertyValue(property, store, out value))
							style.Setters.Add(ReplaceAliases(path, styleMap), value);
						else style.Setters.Add(ReplaceAliases(path, styleMap), info.Value);
					}
					else {
						if(styleMap.TryGetKnownTypePropertyValue(property, store, out value))
							style.Setters.Add(ReplaceAliases(path, styleMap), value);
						else ReadChildPropertyLevel(info, info.ChildProperties, style, path, styleMap);
					}
				}
			}
		}
		Style CreateStyle(StyleCollection styles, IXtraPropertyCollection store, IStyleMap styleMap) {
			Style result = null;
			XtraPropertyInfo typeInfo = store["TypeNameEx"];
			if(typeInfo != null) {
				Type type;
				string targetType = (string)typeInfo.Value;
				if(styleMap.TryGetType(targetType, out type)) {
					if(!styles.Contains(type)) {
						result = new Style(type);
						AllowProperties(result, styleMap);
						styles.Add(result);
					}
				}
			}
			return result;
		}
		static string ReplaceAliases(string path, IStyleMap styleMap) {
			string[] pathElements = path.Split('.');
			string property; string alias;
			for(int i = 0; i < pathElements.Length; i++) {
				property = pathElements[i];
				pathElements[i] = styleMap.PropertyPathAliases.TryGetValue(property, out alias) ? alias : property;
			}
			return string.Join(".", pathElements);
		}
		static void AllowProperties(Style style, IStyleMap styleMap) {
			style.Setters.AllowProperties(styleMap.AllowedProperties[style.TargetType]);
		}
	}
	public static class StyleCollectionHelper {
		static IDictionary<StyleCollectionKey, string> resourcesCore;
		public static IDictionary<StyleCollectionKey, string> Resources {
			get { return GetResources(ServiceProvider); }
		}
		public static IDictionary<StyleCollectionKey, string> GetResources(IServiceProvider serviceProvider) {
			if(resourcesCore == null) {
				resourcesCore = new Dictionary<StyleCollectionKey, string>();
				StyleResourcesHelper.BuildResourcesMap(resourcesCore, serviceProvider);
			}
			return resourcesCore;
		}
		static DevExpress.Utils.IoC.IntegrityContainer gaugesIntergityContainer = new Utils.IoC.IntegrityContainer();
		public static IServiceProvider ServiceProvider {
			get { return gaugesIntergityContainer; }
		}
		public static void Register<TService, Service>() where Service : TService {
			gaugesIntergityContainer.RegisterType<TService, Service>();
		}
		public static TService GetService<TService>(IServiceProvider serviceProvider = null) {
			return (TService)(serviceProvider ?? ServiceProvider).GetService(typeof(TService));
		}
		public static StyleCollection Load(string scope, string name, string tag, IServiceProvider serviceProvider) {
			if(!string.IsNullOrEmpty(scope)) {
				StyleCollectionKey key = new StyleCollectionKey(scope);
				key.Name = StyleCollectionKey.CheckThemeName(name, serviceProvider);
				if(key.Name != null) {
					key.Tag = tag;
					return Load(key, serviceProvider);
				}
			}
			return null;
		}
		public static StyleCollection Load(string path, IServiceProvider serviceProvider) {
			if(string.IsNullOrEmpty(path)) return null;
			string[] parts = path.Split('.');
			if(parts.Length == 2 || parts.Length == 3) {
				StyleCollectionKey key = new StyleCollectionKey(parts[0]);
				key.Name = StyleCollectionKey.CheckThemeName(parts[1], serviceProvider);
				if(key.Name != null) {
					if(parts.Length == 3)
						key.Tag = parts[2];
					return Load(key, serviceProvider);
				}
			}
			return null;
		}
		public static StyleCollection Load(StyleCollectionKey key, IServiceProvider serviceProvider) {
			if(key == null) return null;
			key.Name = StyleCollectionKey.CheckThemeName(key.Name, serviceProvider);
			StyleCollection styles = null;
			string path;
			if(GetResources(serviceProvider).TryGetValue(key, out path)) {
				styles = new StyleCollection(key);
				var resourcesProvider = GetService<IStyleResourceProvider>(serviceProvider);
				var assembly = resourcesProvider.GetAssembly();
				using(Stream stream = assembly.GetManifestResourceStream(path)) {
					IXtraPropertyCollection store = PresetDeserializer.Read(stream);
					byte[] layoutData = (byte[])store["LayoutInfo"].Value;
					ReadStyles(layoutData, styles);
				}
			}
			return styles;
		}
		static void ReadStyles(byte[] layout, StyleCollection styles) {
			using(MemoryStream ms = new MemoryStream(layout)) {
				IStyleMap styleMap = GaugesStyleMapService.Resolve();
				new StyleReader(styleMap).Read(PresetDeserializer.Read(ms), styles);
			}
		}
		static class PresetDeserializer {
			class BinaryXtraSerializeReader : DevExpress.Utils.Serializing.BinaryXtraSerializer {
				public IXtraPropertyCollection Read(Stream source, string appName) {
					return Deserialize(source, appName, null);
				}
			}
			public static IXtraPropertyCollection Read(Stream stream) {
				return new BinaryXtraSerializeReader().Read(stream, "GaugePreset");
			}
		}
	}
}
