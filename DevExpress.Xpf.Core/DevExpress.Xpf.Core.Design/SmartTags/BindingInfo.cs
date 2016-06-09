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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using DevExpress.Xpf.Editors.Helpers;
using Guard = Platform::DevExpress.Utils.Guard;
using DevExpress.Data.Utils;
using DevExpress.Design.SmartTags;
using DevExpress.Design;
using System.Collections;
using System.Text.RegularExpressions;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class BindingInfo {
		public BindingInfo(string path, string converterKey, BindingMode mode) {
			Path = path;
			ConverterKey = converterKey;
			Mode = mode;
		}
		public string Path { get; private set; }
		public string ToolTip {
			get {
				List<string> toolTipLines = new List<string>();
				GetToolTipBegin(toolTipLines);
				GetToolTipEnd(toolTipLines);
				return string.Join("\r\n", toolTipLines);
			}
		}
		public string ConverterKey { get; private set; }
		public BindingMode Mode { get; private set; }
		protected virtual void GetToolTipBegin(IList<string> toolTipLines) {
			toolTipLines.Add("Path = " + (string.IsNullOrEmpty(Path) ? "." : Path));
		}
		protected virtual void GetToolTipEnd(IList<string> toolTipLines) {
			if(ConverterKey != null)
				toolTipLines.Add("Converter = " + ConverterKey);
			if(Mode != BindingMode.Default)
				toolTipLines.Add("Mode = " + Mode);
		}
	}
	public class ElementNameBindingInfo : BindingInfo {
		public ElementNameBindingInfo(string path, string elementName, IModelItem item, string converterKey, BindingMode mode)
			: base(path, converterKey, mode) {
			ElementName = elementName;
			Element = DesignTimeObjectModelCreateService.FindObjectInTreeByName(item.Root, elementName);
		}
		public string ElementName { get; private set; }
		public IModelItem Element { get; private set; }
		protected override void GetToolTipBegin(IList<string> toolTipLines) {
			base.GetToolTipBegin(toolTipLines);
			toolTipLines.Add("ElementName = " + ElementName);
		}
	}
	public class StaticResourceBindingInfo : BindingInfo {
		public StaticResourceBindingInfo(string path, string resourceKey, string converterKey, BindingMode mode)
			: base(path, converterKey, mode) {
			ResourceKey = resourceKey;
		}
		public string ResourceKey { get; private set; }
		protected override void GetToolTipBegin(IList<string> toolTipLines) {
			base.GetToolTipBegin(toolTipLines);
			toolTipLines.Add("StaticResource = " + ResourceKey);
		}
	}
	public class RelativeSourceSelfBindingInfo : BindingInfo {
		public RelativeSourceSelfBindingInfo(string path, IModelItem item, string converterKey, BindingMode mode) : base(path, converterKey, mode) {
			Item = item;
		}
		public IModelItem Item { get; private set; }
		protected override void GetToolTipBegin(IList<string> toolTipLines) {
			base.GetToolTipBegin(toolTipLines);
			toolTipLines.Add("RelativeSource Self");
		}
	}
	public class DataContextBindingInfo : BindingInfo {
		public DataContextBindingInfo(string path, IModelItem item, string converterKey, BindingMode mode)
			: base(path, converterKey, mode) {
			DataContext = BindingInfoHelper.GetDataContext(item);
		}
		public DesignTimeObject DataContext { get; private set; }
	}
	public class BindingInfoHelper {
		public static BindingInfo GetBindingInfo(IModelItem value, string propertyName) {
			Guard.ArgumentNotNull(value, "value");
			IModelProperty property = value.Properties.Find(propertyName);
			if(property == null) return null;
			IModelItem propertyValue = property.Value;
			if(propertyValue == null || propertyValue.ItemType != typeof(Binding)) return null;
			string path = GetPath(propertyValue);
			string converterKey = GetConverterKey(propertyValue);
			string elementName = GetElementName(propertyValue);
			BindingMode mode = GetBindingMode(propertyValue);
			if(elementName != null) return new ElementNameBindingInfo(path, elementName, value, converterKey, mode);
			string staticResourceKey = GetStaticResourceKey(propertyValue);
			if(staticResourceKey != null) return new StaticResourceBindingInfo(path, staticResourceKey, converterKey, mode);
			RelativeSourceMode? relativeSourceMode = GetRelativeSourceMode(propertyValue);
			if(relativeSourceMode != null) return relativeSourceMode == RelativeSourceMode.Self ? new RelativeSourceSelfBindingInfo(path, value, converterKey, mode) : null;
			return new DataContextBindingInfo(path, value, converterKey, mode);
		}
		public static DesignTimeObject GetDataContext(IModelItem currentItem) {
			while(currentItem != null) {
				object value = currentItem.GetCurrentValue();
				object dataContext;
				bool hasDataContext = TryGetRuntimeDataContext(value, out dataContext);
				if(hasDataContext) {
					if(dataContext == null) return null;
					IModelItem runtimeTypeProvider = null;
					try {
						runtimeTypeProvider = currentItem.Properties["DataContext"].Value;
					} catch { }
					return new DesignTimeObject(dataContext, runtimeTypeProvider);
				}
				currentItem = currentItem.Parent;
			}
			return null;
		}
		static bool TryGetRuntimeDataContext(object element, out object dataContext) {
			FrameworkElement fe = element as FrameworkElement;
			if(fe != null) {
				dataContext = fe.DataContext;
				return true;
			}
			FrameworkContentElement fce = element as FrameworkContentElement;
			if(fce != null) {
				dataContext = fce.DataContext;
				return true;
			}
			dataContext = null;
			return false;
		}
		class TypeDescriptorContext : ITypeDescriptorContext {
			IContainer ITypeDescriptorContext.Container { get { return null; } }
			object ITypeDescriptorContext.Instance { get { return null; } }
			PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return null; } }
			object IServiceProvider.GetService(Type serviceType) { return null; }
			void ITypeDescriptorContext.OnComponentChanged() { }
			bool ITypeDescriptorContext.OnComponentChanging() { return false; }
		}
		public static IEnumerable<PropertyDescriptor> GetProperties(Type type) {
			if(type.IsArray) return new PropertyDescriptor[] { };
			if(type == typeof(string)) return new PropertyDescriptor[] { };
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			bool showProperties = converter.GetPropertiesSupported(new TypeDescriptorContext()) || converter.GetType() == typeof(TypeConverter);
			if(!showProperties && type != typeof(DateTime)) return new PropertyDescriptor[] { };
			return TypeDescriptor.GetProperties(type, new Attribute[] { BrowsableAttribute.Yes }).Cast<PropertyDescriptor>()
				.Where(x => !x.Attributes.Contains(new Attribute[] {
					new EditorBrowsableAttribute(EditorBrowsableState.Never),
					new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
				}));
		}
		public static string GetElementName(IModelItem binding) {
			IModelProperty elementName = binding.Properties.Find("ElementName");
			if(elementName == null || !elementName.IsSet) return null;
			return (string)elementName.ComputedValue;
		}
		public static string GetStaticResourceKey(IModelItem binding) {
			IModelProperty source = binding.Properties.Find("Source");
			if(source == null || !source.IsSet) return null;
			IModelProperty resourceKey = source.Value.Properties.Find("ResourceKey");
			if(resourceKey == null || !resourceKey.IsSet) return null;
			return resourceKey.ComputedValue as string;
		}
		public static RelativeSourceMode? GetRelativeSourceMode(IModelItem binding) {
			IModelProperty relativeSource = binding.Properties.Find("RelativeSource");
			if(relativeSource == null || !relativeSource.IsSet) return null;
			IModelProperty mode = relativeSource.Value.Properties.Find("Mode");
			if(mode == null || !mode.IsSet) return null;
			return (RelativeSourceMode)mode.ComputedValue;
		}
		public static string GetPath(IModelItem binding) {
			IModelProperty path = binding.Properties.Find("Path");
			if(path == null) return null;
			IModelItem pathValue = path.Value;
			if(pathValue == null) return null;
			IModelProperty pathInternal = pathValue.Properties.Find("Path");
			if(pathInternal == null) return null;
			object pathInternalComputedValue = pathInternal.ComputedValue;
			return pathInternalComputedValue == null ? null : pathInternalComputedValue.ToString();
		}
		public static string GetConverterKey(IModelItem binding) {
			IModelProperty converter = binding.Properties.Find("Converter");
			if(converter == null) return null;
			IModelItem converterValue = converter.Value;
			if(converterValue == null) return null;
			IModelProperty key = converterValue.Properties.Find("ResourceKey");
			if(key == null || !key.IsSet) return null;
			object keyComputedValue = key.ComputedValue;
			return keyComputedValue == null ? null : keyComputedValue.ToString();
		}
		public static BindingMode GetBindingMode(IModelItem binding) {
			return (BindingMode)binding.Properties["Mode"].ComputedValue;
		}
		public static string GetTypeName(Type type) {
			string typeName = type.Name;
			if(!type.IsGenericType) return typeName;
			StringBuilder name = new StringBuilder(typeName);
			int p = name.ToString().IndexOf('`');
			if(p >= 0)
				name.Remove(p, name.Length - p);
			bool first = true;
			foreach(Type genericArgument in type.GetGenericArguments()) {
				if(first) {
					first = false;
					name.Append('<');
				} else {
					name.Append(", ");
				}
				string argumentName = GetTypeName(genericArgument);
				name.Append(argumentName);
			}
			if(!first)
				name.Append('>');
			return name.ToString();
		}
		public static IEnumerable<IModelItem> GetChildElements(IModelItem item) {
			return DesignTimeObjectModelCreateService.GetFrameworkChildren(item);
		}
		public static Dictionary<string, Type> GetResources(IModelItem item) {
			var markupAccessService2012 = item.Context.Services.GetService<IMarkupAccessService>() as IMarkupAccessService2012;
			if(markupAccessService2012 != null)
				return GetResourcesVS2012(item, markupAccessService2012);
			return GetResourcesVS2010(item);
		}
		static Dictionary<string, Type> GetResourcesVS2012(IModelItem item, IMarkupAccessService2012 markupAccessService2012) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			try {
				ISceneNodeModelItem sceneNodeModelItem = markupAccessService2012.GetModelItem(item);
				ISceneNode sceneNode = sceneNodeModelItem.SceneNode;
				IVS2012TypeMetadata objectType = sceneNode.ProjectContext.Metadata.GetType(typeof(object));
				ISceneDocumentNode[] resources = sceneNode.ProjectContext.DesignerContext.ResourceManager.GetResourcesInElementsScope(sceneNode, objectType, true, true).Where(x => x.Type.RuntimeType == typeof(DictionaryEntry)).ToArray();
				foreach(ISceneDocumentNode resource in resources) {
					try {
						ISceneDocumentNode[] properties = resource.Properties.ToArray();
						ISceneDocumentNode key = properties.Where(p => p.SitePropertyKey.Name == "Key").FirstOrDefault();
						if(key == null) continue;
						string keyString = key.ToString();
						int keyValueStart = keyString.IndexOf('(');
						if(keyValueStart <= 0 || keyValueStart >= keyString.Length - 2) continue;
						if(keyString[keyString.Length - 1] != ')') continue;
						string keyValue = keyString.Substring(keyValueStart + 1, keyString.Length - keyValueStart - 2);
						ISceneDocumentNode value = properties.Where(p => p.SitePropertyKey.Name == "Value").FirstOrDefault();
						if(value == null) continue;
						Type valueType = value.Type.RuntimeType;
						if(valueType == null) continue;
						result.Add(keyValue, valueType);
					} catch (Exception e) {
						DebugHelper.Assert(e);
					}
				}
			} catch (Exception e) {
				DebugHelper.Assert(e);
			}
			return result;
		}
		static Dictionary<string, Type> GetResourcesVS2010(IModelItem item) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			for(IModelItem resourcesContainer = item; resourcesContainer != null; resourcesContainer = resourcesContainer.Parent) {
				IModelProperty resources = resourcesContainer.Properties.Find("Resources");
				if(resources == null) continue;
				foreach(IModelItem key in resources.Dictionary.Keys) {
					if(key.ItemType != typeof(string)) continue;
					string keyValue = (string)key.GetCurrentValue();
					int x;
					if(int.TryParse(keyValue, NumberStyles.None, CultureInfo.InvariantCulture, out x)) continue; 
					if(!result.ContainsKey(keyValue))
						result.Add(keyValue, resources.Dictionary[key].ItemType);
				}
			}
			return result;
		}
		public static bool IsReadOnlyProperty(Type type, string path) {
			Guard.ArgumentNotNull(type, "type");
			if(string.IsNullOrEmpty(path)) return true;
			var propDescriptor = FindProperty(type, path);
			if(propDescriptor == null) return false;
			DependencyProperty dependencyProperty = GetDependencyProperty(propDescriptor);
			return dependencyProperty != null ? dependencyProperty.ReadOnly : propDescriptor.IsReadOnly;
		}
		public static DependencyProperty GetDependencyProperty(PropertyDescriptor prop) {
			DependencyPropertyDescriptor depProp = DependencyPropertyDescriptor.FromProperty(prop);
			if(depProp == null)
				return null;
			return depProp.DependencyProperty;
		}
		public static void SetElementNameBinding(IModelItem item, string propertyName, Type propertyOwnerType, string path, string converter, BindingMode mode, UpdateSourceTrigger updateSourceTrigger, IModelItem element, string elementName) {
			ModelPropertyHelper.SetPropertyValue(item, propertyName, () => {
				if(string.IsNullOrEmpty(elementName))
					elementName = DesignTimeObjectModelCreateService.SetName(element);
				return DesignTimeObjectModelCreateService.CreateBindingItem(item, path, null, elementName, converter, null, mode, null, null, updateSourceTrigger);
			}, propertyOwnerType);
		}
		public static void SetRelativeSourceSelfBinding(IModelItem item, string propertyName, Type propertyOwnerType, string path, string converter, BindingMode mode, UpdateSourceTrigger updateSourceTrigger) {
			ModelPropertyHelper.SetPropertyValue(item, propertyName, () => {
				return DesignTimeObjectModelCreateService.CreateBindingItem(item, path, null, null, converter, null, mode, RelativeSourceMode.Self, null, updateSourceTrigger);
			}, propertyOwnerType);
		}
		public static void SetDataContextBinding(IModelItem item, string propertyName, Type propertyOwnerType, string path, string converter, BindingMode mode, UpdateSourceTrigger updateSourceTrigger) {
			ModelPropertyHelper.SetPropertyValue(item, propertyName, () => {
				return DesignTimeObjectModelCreateService.CreateBindingItem(item, path, null, null, converter, null, mode, null, null, updateSourceTrigger);
			}, propertyOwnerType);
		}
		public static void SetStaticResourceBinding(IModelItem item, string propertyName, Type propertyOwnerType, string path, string converter, BindingMode mode, UpdateSourceTrigger updateSourceTrigger, string staticResourceKey) {
			ModelPropertyHelper.SetPropertyValue(item, propertyName, () => {
				return DesignTimeObjectModelCreateService.CreateBindingItem(item, path, null, null, converter, null, mode, null, staticResourceKey, updateSourceTrigger);
			}, propertyOwnerType);
		}
		static PropertyDescriptor FindProperty(Type type, string path) {
			string[] splitPaths = path.Split('.');
			Type propType = type;
			PropertyDescriptor propDescriptor = null;
			foreach(string propName in splitPaths) {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(propType);
				propDescriptor = props[propName];
				if(propDescriptor == null) return null;
				propType = propDescriptor.PropertyType;
			}
			return propDescriptor;
		}
	}
}
