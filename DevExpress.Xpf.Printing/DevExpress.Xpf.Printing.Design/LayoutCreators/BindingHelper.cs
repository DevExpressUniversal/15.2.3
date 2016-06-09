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
using Microsoft.Windows.Design.Model;
using System.Windows;
using Microsoft.Windows.Design;
namespace DevExpress.Xpf.Printing.Design.LayoutCreators {
	static class ResourceManager {
		public static ModelItem CreateConverter<T>(ModelItem item, string entryKey, params object[] args) where T : class {
			ModelItem converterItem;
			try {
				var converter = Activator.CreateInstance(typeof(T), args);
				converterItem = ModelFactory.CreateItem(item.Context, converter);
			} catch {
				converterItem = ModelFactory.CreateItem(item.Context, typeof(T));
			}
			return AddResource(converterItem, entryKey);
		}
		public static ModelItem AddResource(ModelItem resource, string entryKey) {
			var resources = GetRootResources(resource);
			if(resources == null)
				return null;
			var resourceKey = FindResource(resources.Dictionary, entryKey);
			if(resourceKey != null)
				resources.Dictionary.Remove(entryKey);
			resources.Dictionary.Add(entryKey, resource);
			return resource;
		}
		public static void RemoveResource(ModelItem rootItem, string entryKey) {
			var resources = GetRootResources(rootItem);
			if(resources == null)
				return;
			var resourceKey = FindResource(resources.Dictionary, entryKey);
			if(resourceKey != null)
				resources.Dictionary.Remove(entryKey);
		}
		public static object FindResource(ModelItemDictionary dictionary, string resourceKey) {
			foreach(object k in dictionary.Keys) {
				string dictionaryKey = string.Empty;
				var keyItem = k as ModelItem;
				dictionaryKey = keyItem != null ? keyItem.Source.ComputedValue as string : k as string;
				if(dictionaryKey != resourceKey)
					continue;
				if(!string.IsNullOrEmpty(dictionaryKey)) {
					ModelItem item = dictionary[dictionaryKey];
					if(item != null)
						return dictionaryKey;
				}
			}
			return null;
		}
		public static ModelProperty GetRootResources(ModelItem item) {
			var root = item.Root;
			if(root == null)
				return null;
			if(!typeof(Platform::System.Windows.FrameworkElement).IsAssignableFrom(root.ItemType))
				return null;
			var resources = root.Properties["Resources"];
			if(!resources.IsSet) {
				resources.SetValue(ModelFactory.CreateItem(root.Context, typeof(Platform::System.Windows.ResourceDictionary)));
			}
			return resources;
		}
		public static ModelItem CreateStaticResourceBinding(ModelItem item, string defaultKeyValue) {
			var resourceKey = FindResource(GetRootResources(item).Dictionary, defaultKeyValue);
			var binding = ModelFactory.CreateItem(item.Context, typeof(StaticResourceExtension));
			binding.Properties["ResourceKey"].SetValue(resourceKey == null ? ModelFactory.CreateItem(item.Context, defaultKeyValue) : resourceKey);
			return binding;
		}
	}
}
