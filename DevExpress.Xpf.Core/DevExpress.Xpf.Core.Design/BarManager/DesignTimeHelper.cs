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
using System.Linq;
using System.Windows;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Core.Native;
using System.Collections;
using DevExpress.Mvvm.Native;
#if SL
using System.Windows.Media;
using UIElement = Platform::System.Windows.UIElement;
#endif
namespace DevExpress.Xpf.Core.Design {
	public static class BarManagerDesignTimeHelper {
		public static void AddBar(ModelItem barManager, ModelItem bar) {
			if(barManager == null || bar == null) return;
			using(ModelEditingScope scope = barManager.BeginEdit("Add a Bar")) {
				barManager.Properties["Bars"].Collection.Add(bar);
				scope.Complete();
			}
		}
		public static void AddBarItem(ModelItem barManager, ModelItem barItem) {
			if(barManager == null || barItem == null) return;
			using(ModelEditingScope scope = barManager.BeginEdit("Add a BarItem")) {
				barManager.Properties["Items"].Collection.Add(barItem);
				scope.Complete();
			}
		}
		public static void AddBarItemLink(ModelProperty property, ModelItem child) {
			if(property == null || child == null || !property.IsCollection) return;
			using(var scope = property.Parent.BeginEdit("Add BarItemLink")) {
				property.Collection.Add(child);
				child.ResetLayout();
				scope.Complete();
			}
		}		
		public static ModelItem CreateBarItem(EditingContext editingContext, Type barItemType) {
			if(!barItemType.IsSubclassOf(typeof(BarItem)))
				throw new InvalidOperationException("Incorrect type for a creating a new BarItem");
			return ModelFactory.CreateItem(editingContext, barItemType, CreateOptions.InitializeDefaults);
		}
		public static ModelItem CreateBarEditItem(EditingContext context, Type barEditItemSettingsType) {
			if(!barEditItemSettingsType.IsSubclassOf(typeof(BaseEditSettings))) throw new InvalidOperationException("Incorrect type for a creating a new BarEditSettings");
			ModelItem editItem = CreateBarItem(context, typeof(BarEditItem));
			ModelItem editSettingsItem = CreateEditSettings(context, barEditItemSettingsType);
			editItem.Properties["EditSettings"].SetValue(editSettingsItem);
			return editItem;
		}
		public static ModelItem CreateBarItemLink(ModelItem barItem) {
			Type itemLinkType = BarItemLinkCreator.Default.GetItemType(barItem.ItemType);
			ModelItem itemLink = ModelFactory.CreateItem(barItem.Context, itemLinkType, null);
			string barItemName = barItem.Name;
			if(string.IsNullOrEmpty(barItemName)) {
				using(var scope = barItem.BeginEdit()) {
					barItemName = GetUniqueName(barItem);
					barItem.Properties["Name"].SetValue(barItemName);
					scope.Complete();
				}
			}
			itemLink.Properties["BarItemName"].SetValue(barItemName);
			return itemLink;
		}
		public static ModelItem FindBarManagerInParent(ModelItem from) {
			return FindParentByType<BarManager>(from);
		}
		public static ModelItem FindParentByType(Type type, ModelItem from) {
			ModelItem item = from;
			while(item != null) {
				if(item.IsItemOfType(type))
					break;
				item = item.Parent;
			}
			return item;
		}
		public static T FindVisualParentByType<T>(ModelItem barItemLink) where T : class {
			foreach(var item in GetBarItemLinkControls(barItemLink)) {
				return LayoutHelper.FindLayoutOrVisualParentObject<T>(item);
			}
			return null;
		}
		public static ModelItem FindParentByType<T>(ModelItem from) {
			ModelItem item = from;
			while(item != null) {
				if(item.IsItemOfType(typeof(T)))
					break;
				item = item.Parent;
			}
			return item;
		}
		public static IEnumerable<string> GetAvailableBarItemsNames(ModelItem barManager, ModelItem barItemLink) {
			List<string> names = new List<string>();
			if(barManager == null || !barManager.IsItemOfType(typeof(BarManager)) || barItemLink == null)
				return names;
			Type linkType = barItemLink.ItemType;
			bool isBarItemLink = linkType == typeof(BarItemLink);
			foreach(var item in GetBarItems(barManager)) {
				string name = item.Name ?? item.Properties["Name"].ComputedValue as string;
				if(string.IsNullOrEmpty(name))
					continue;
				if(isBarItemLink || linkType == BarItemLinkCreator.Default.GetItemType(item.ItemType))
					names.Add(name);
			}
			return names;
		}
		public static ModelItemCollection GetBarItems(ModelItem barManager) {
			return barManager.Properties["Items"].Collection;
		}
		public static ModelItem GetBarItemFromLink(ModelItem barItemLink) {
			ModelItem barItem = GetModelItem(barItemLink, barItemLink.Properties["Item"].ComputedValue);
			return barItem != null ? barItem : GetBarItemFromLinkByName(barItemLink);
		}
		public static IEnumerable<ModelItem> GetBarManagerItems(ModelItem barManager) {
			List<ModelItem> result = new List<ModelItem>();
			ModelService service = barManager.Context.Services.GetService<ModelService>();
			foreach(ModelItem item in service.Find(barManager, typeof(BarItem))) {
				if(item.Properties["Name"].ComputedValue != null) {
					result.Add(item);
				}
			}
			return result;
		}
		public static IEnumerable<ModelItem> GetBarItemLinks(ModelItem barItem) {
			IEnumerable<ModelItem> empty = Enumerable.Empty<ModelItem>();
			if(barItem == null) 
				return empty;
			var barItemValue = barItem.GetCurrentValue() as BarItem;
			ModelService service = barItem.Context.Services.GetService<ModelService>();
			if(service == null || barItemValue == null || barItemValue.Links == null)
				return empty;
			return barItemValue.Links.Select(link => service.Find(barItem.Root, link)).Where(link => link != null);
		}
		public static IEnumerable<BarItemLinkInfo> GetLinkInfos(ModelItem barItemLink) {
			if(barItemLink == null || !barItemLink.IsItemOfType(typeof(BarItemLinkBase)))
				return new List<BarItemLinkInfo>();
			return barItemLink.Properties["LinkInfos"].ComputedValue as IEnumerable<BarItemLinkInfo>;
		}
		public static IEnumerable<BarItemLinkControl> GetBarItemLinkControls(ModelItem barItemLink) {
			var result = Enumerable.Empty<BarItemLinkControl>();
			if(barItemLink == null || !barItemLink.IsItemOfType(typeof(BarItemLink)))
				return result;
			var linkInfos = (IEnumerable<BarItemLinkInfo>)barItemLink.Properties["LinkInfos"].ComputedValue;
			if(linkInfos == null)
				return result;
			return linkInfos.Select(info => info.LinkControl).OfType<BarItemLinkControl>();
		}
		public static IEnumerable<BarItemLinkControl> GetCommonBarItemLinkControls(ModelItem barItem) {
			var barItemValue = barItem.With(item => item.GetCurrentValue() as BarItem);
			if (barItemValue == null)
				return Enumerable.Empty<BarItemLinkControl>();
			return barItemValue.Links.Where(link => link.CommonBarItemCollectionLink).SelectMany(link => link.LinkInfos.Select(info => info.LinkControl).OfType<BarItemLinkControl>());
		}
		public static ModelItem GetModelItem(ModelItem root, object source) {
			if(root == null || source == null) return null;
			ModelService service = root.Context.Services.GetService<ModelService>();
			if(service == null) return null;
			return service.Find(root.Root, source);
		}
		public static int GetIndexInCollection(ModelItem barItemLink) {
			if(barItemLink == null || barItemLink.Source == null || barItemLink.Source.Parent == null || !barItemLink.Source.Parent.Source.IsCollection)
				return -1;
			return barItemLink.Source.Parent.Source.Collection.IndexOf(barItemLink);
		}
		public static ModelItem GetModelItemFromBarItemLink<T>(ModelItem barItemLink) where T : class {
			if(barItemLink == null || !barItemLink.IsItemOfType(typeof(BarItemLink))) return null;
			ModelItem result = null;
			T control;
			foreach(BarItemLinkControl linkControl in BarManagerDesignTimeHelper.GetBarItemLinkControls(barItemLink)) {
				control = LayoutHelper.FindParentObject<T>(linkControl);
				if(control != null) {
					result = GetModelItem(barItemLink, control);
					break; 
				}
			}
			return result;
		}
		public static ModelItem GetModelItemFromCommonBarItem<T>(ModelItem barItem) where T : class {
			if(barItem == null || !barItem.IsItemOfType(typeof(BarItem))) return null;
			ModelItem result = null;
			T control;
			foreach(BarItemLinkControl linkControl in BarManagerDesignTimeHelper.GetCommonBarItemLinkControls(barItem)) {
				control = LayoutHelper.FindParentObject<T>(linkControl);
				if(control != null) {
					result = GetModelItem(barItem, control);
					break;
				}
			}
			return result;
		}
		public static IEnumerable<ModelItem> GetBarContainers(ModelItem barManager) {
			List<ModelItem> result = new List<ModelItem>();
			if(barManager == null || !barManager.IsItemOfType(typeof(BarManager))) return result;
			ModelService service = barManager.Context.Services.GetService<ModelService>();
			if(service == null) return result;
			foreach(ModelItem container in service.Find(barManager, typeof(BarContainerControl))) {
				result.Add(container);
			}
			return result;
		}
		public static string GetUniqueName(ModelItem item, ModelItem scope = null, string prefix = null) {
			if(item == null && scope == null)
				throw new InvalidOperationException("item and scope parameters are both NULL");
			if(item == null && prefix == null)
				throw new InvalidOperationException("item and prefix parameters are both NULL");
			ModelService service = item == null ? scope.Context.Services.GetService<ModelService>() : item.Context.Services.GetService<ModelService>();
			if(service == null) return string.Empty;
			if(scope == null)
				scope = item.Root;
			List<ModelItem> existingItems = null;
			if(item != null)
				existingItems = new List<ModelItem>(service.Find(scope, item.ItemType));
			else
				existingItems = new List<ModelItem>(service.Find(scope, (t) => true ));
			if(prefix != null) {
				if(!existingItems.Exists((existItem) => { return existItem.Name == prefix; }))
					return prefix;
			}
			string namePrefix = prefix == null ? GetNamePrefix(item.ItemType.Name) : prefix;
			int nameIndex = 0;
			string uniqueName = string.Empty;
			do {
				uniqueName = string.Format("{0}{1}", namePrefix, ++nameIndex);
			} while(existingItems.Exists((existItem) => { return existItem.Name == uniqueName; }));
			return uniqueName;
		}
		public static bool IsCreateStandardLayout(ModelItem barManager) {
			if(barManager == null || !barManager.IsItemOfType(typeof(BarManager))) return false;
			return (bool)barManager.Properties["CreateStandardLayout"].ComputedValue;
		}
		public static bool IsMainMenuExist(ModelItem barManager) {
			return IsBarExist(barManager, "IsMainMenu");
		}
		public static bool IsStatusBarExist(ModelItem barManager) {
			return IsBarExist(barManager, "IsStatusBar");
		}
		public static void RemoveBar(ModelItem bar) {
			ModelItem barManager = FindBarManagerInParent(bar);
			if(barManager == null) return;
			barManager.Properties["Bars"].Collection.Remove(bar);
		}
		public static void RemoveBarItem(ModelItem barItem) {
			IEnumerable<ModelItem> links = GetBarItemLinks(barItem).ToList();
			if(links.Count() > 1 && ConfirmBarItemRemovingWithLinks(barItem) != MessageBoxResult.Yes) {
				return;
			}
			foreach(ModelItem link in links) {
				RemoveBarItemLink(link);
			}
			ModelProperty targetProperty = GetBarItemsCollectionProperty(barItem);
			if(targetProperty != null)
				targetProperty.Collection.Remove(barItem);
		}
		static ModelProperty GetBarItemsCollectionProperty(ModelItem barItem) {
			if(barItem == null || barItem.Parent == null)
				return null;
			if(barItem.Source == null) {
				ModelItem parent = barItem.Parent;
				string propertyName = "Items";
				if(parent.IsItemOfType(typeof(IRibbonControl))) {
					if(parent.Properties["PageHeaderItems"].Collection.Contains(barItem))
						propertyName = "PageHeaderItems";
					else if(parent.Properties["ToolbarItems"].Collection.Contains(barItem))
						propertyName = "ToolbarItems";
				} else if(parent.IsItemOfType(typeof(IRibbonStatusBarControl))) {
					propertyName = parent.Properties["LeftItems"].Collection.Contains(barItem) ?
					   "LeftItems" : "RightItems";
				}
				return parent.Properties[propertyName];
			} else
				return barItem.Source.Parent.Source;
		}
		public static MessageBoxResult ConfirmBarItemRemovingWithLinks(ModelItem barItem) {
			string message = string.Format("There are links that refer to {0} item. Deleting item will delete all its links. Do you want to continue?", barItem.Name);
			return MessageBox.Show(message, "BarItem deleting", MessageBoxButton.YesNo, MessageBoxImage.Question);
		}
		public static void RemoveBarItemLink(ModelItem barItemLink) {
			if(barItemLink == null || barItemLink.Parent == null) return;
			using(var scope = barItemLink.BeginEdit(string.Format("Remove the {0}", barItemLink.ItemType.Name))) {
				if(barItemLink.Source != null)
					barItemLink.Source.Parent.Source.Collection.Remove(barItemLink);
				else {
					ModelItem parent = barItemLink.Parent;
					string propertyName = string.Empty;
					if(parent.IsItemOfType(typeof(ILinksHolder)))
						propertyName = "ItemLinks";
					else if(parent.IsItemOfType(typeof(IRibbonControl))) {
						if(parent.Properties["PageHeaderItemLinks"].Collection.Contains(barItemLink))
							propertyName = "PageHeaderItemLinks";
						else if(parent.Properties["PageHeaderItems"].Collection.Contains(barItemLink))
							propertyName = "PageHeaderItems";
						else if(parent.Properties["ToolbarItemLinks"].Collection.Contains(barItemLink))
							propertyName = "ToolbarItemLinks";
						else if(parent.Properties["ToolbarItems"].Collection.Contains(barItemLink))
							propertyName = "ToolbarItems";
					} else if(parent.IsItemOfType(typeof(IRibbonStatusBarControl))) {
						propertyName = parent.Properties["LeftItemLinks"].Collection.Contains(barItemLink) ?
						   "LeftItemLinks" : "RightItemLinks";
					}
					if(!string.IsNullOrEmpty(propertyName))
						parent.Properties[propertyName].Collection.Remove(barItemLink);
				}
				scope.Complete();
			}
		}
		public static void RenameBarItemLinks(ModelItem barItem) {
			using(var scope = barItem.BeginEdit(string.Format("Rename the {0}", barItem.ItemType.Name))) {
				foreach(ModelItem link in GetBarItemLinks(barItem)) {
					link.Properties["BarItemName"].SetValue(barItem.Name);
				}
				scope.Complete();
			}
		}
		public static void ResetLayout(this ModelItem item) {
			List<string> propNames = new List<string>(new string[] { "VerticalAlignment", "HorizontalAlignment", "Width", "Height", "Margin" });
			foreach(string name in propNames) {
				ModelProperty prop = item.Properties.Find(name);
				if(prop != null) prop.ClearValue();
			}
		}
		public static void SetBarContainerType(ModelItem bar, BarContainerType containerType) {
			if(!bar.IsItemOfType(typeof(Bar))) return;
			using(ModelEditingScope scope = bar.BeginEdit("Set ContainerType")) {
				bar.Properties["DockInfo"].Value.Properties["ContainerName"].ClearValue();
				bar.Properties["DockInfo"].Value.Properties["ContainerType"].SetValue(containerType);
				scope.Complete();
			}
		}
		internal static ModelItem ConvertToMainMenuBar(ModelItem bar) {
			bar.Properties["IsStatusBar"].ClearValue();
			bar.Properties["IsMainMenu"].SetValue(true);
			SetDockInfo(bar, BarContainerType.Top);
			return bar;
		}
		internal static ModelItem ConvertToSimpleBar(ModelItem bar) {
			bar.Properties["IsStatusBar"].ClearValue();
			bar.Properties["IsMainMenu"].ClearValue();
			SetDockInfo(bar, BarContainerType.Top);
			return bar;
		}
		internal static ModelItem ConvertToStatusBar(ModelItem bar) {
			bar.Properties["IsMainMenu"].ClearValue();
			bar.Properties["IsStatusBar"].SetValue(true);
			SetDockInfo(bar, BarContainerType.Bottom);
			return bar;
		}
		static ModelItem CreateEditSettings(EditingContext editingContext, Type barEditItemSettingsType) {
			return ModelFactory.CreateItem(editingContext, barEditItemSettingsType, CreateOptions.InitializeDefaults);
		}
		static ModelItem GetBarItemFromLinkByName(ModelItem barItemLink) {
			ModelService service = barItemLink == null ? null : barItemLink.Context.Services.GetService<ModelService>();
			if(service == null)
				return null;
			string barItemName = (string)barItemLink.Properties["BarItemName"].ComputedValue;
			if(string.IsNullOrEmpty(barItemName))
				return null;
			return service.FromName(barItemLink.Root, barItemName);
		}
		static string GetNamePrefix(string typeName) {
			return string.Format("{0}{1}", (char)(typeName[0] + 32), typeName.Substring(1));
		}
		static void SetDockInfo(ModelItem bar, Platform::DevExpress.Xpf.Bars.BarContainerType barContainerType) {
			ModelItem barDockInfo = ModelFactory.CreateItem(bar.Context, typeof(BarDockInfo), CreateOptions.None);
			barDockInfo.Properties["ContainerType"].SetValue(barContainerType);
			bar.Properties["DockInfo"].SetValue(barDockInfo);
		}
		static bool IsBarExist(ModelItem barManager, string propertyName ) {
			if(barManager == null) return false;
			foreach(ModelItem bar in barManager.Properties["Bars"].Collection) {
				if((bool)bar.Properties[propertyName].ComputedValue)
					return false;
			}
			return true;
		}
	}
	public static class SelectionHelper {
		public static void UpdateSelection(ModelItem source, ModelProperty property) {
			if(source == null || source.Parent == null || property == null
				|| !property.IsCollection) return;
			ModelItemCollection items = property.Collection;
			Selection selection;
			if(items.Count > 1) {
				int idx = items.IndexOf(source);
				ModelItem nextSelection;
				if(idx < items.Count - 1) nextSelection = items[idx + 1];
				else if(idx > 0) nextSelection = items[idx - 1];
				else nextSelection = items[0];
				selection = new Selection(nextSelection);
			} else selection = new Selection(source.Parent);
			source.Context.Items.SetValue(selection);
		}
		public static void SetSelection(EditingContext context, ModelItem newSelection) {
			if(newSelection == null || context == null) return;
			SelectionOperations.SelectOnly(context, newSelection);
		}
	}
	public static class ViewItemHelper {
		public static ViewItem GetVisualParent<T>(ViewItem child) {
			ViewItem result = child;
			while(result != null) {
				if(result.PlatformObject is T) break;
				result = result.VisualParent;
			}
			return result;
		}
		public static ViewItem GetLogicalParent<T>(ViewItem child) {
			ViewItem result = child;
			while(result != null) {
				if(result.PlatformObject is T) break;
				result = result.LogicalParent;
			}
			return result;
		}
		public static ViewItem GetVisualChild<T>(ViewItem parent) {
			ViewItem result = null;
			foreach(ViewItem child in parent.VisualChildren) {
				if(child.PlatformObject is T) { result = child; break; }
				result = GetVisualChild<T>(child);
				if(result != null) break;
			}
			return result;
		}
		public static ViewItem GetLogicalChild<T>(ViewItem parent) {
			ViewItem result = null;
			foreach(ViewItem child in parent.VisualChildren) {
				if(child.PlatformObject is T) { result = child; break; }
				result = GetLogicalChild<T>(child);
				if(result != null) break;
			}
			return result;
		}
		public static ViewItem GetViewItem(ViewItem root, UIElement from) {
			if(root == null || from == null) return null;
			ViewItem result = null;
			if(root.PlatformObject == from) return root;
			foreach(ViewItem item in root.VisualChildren) {
				result = GetViewItem(item, from);
				if(result != null) break;
			}
			return result;
		}
		public static ModelItem GetVisualParentWithView(EditingContext context, ViewItem from) {
			ViewService service = context.Services.GetService<ViewService>();
			if(from == null || from.VisualParent == null || service == null) return null;
			ViewItem item = from.VisualParent;
			while(item != null) {
				ModelItem res = service.GetModel(item);
				if(res != null) return res;
				item = item.VisualParent;
			}
			return null;
		}
		public static ModelItem GetLogicalParentWithView(ModelItem from) {
			ModelItem item = from;
			while(item != null) {
				if(item.View != null) return item;
				item = item.Parent;
			}
			return null;
		}
		public static ModelItem GetParentModelItem<T>(EditingContext context, ViewItem child) {
			ViewItem result = GetVisualParent<T>(child);
			ViewService service = context.Services.GetService<ViewService>();
			if(result != null && service != null) {
				return service.GetModel(result);
			}
			return null;
		}
	}
#if SL
	public static class ResolutionHelper {
		public static void GetTransformCoefficient(DependencyObject dependencyObject, out double resX, out double resY) {
			PresentationSource source = PresentationSource.FromDependencyObject(dependencyObject);
			if(source == null || source.CompositionTarget == null) {
				resX = 1d;
				resY = 1d;
				return;
			}
			Matrix matrix = source.CompositionTarget.TransformToDevice;
			resX = matrix.M11;
			resY = matrix.M22;
		}
	}
#endif
}
