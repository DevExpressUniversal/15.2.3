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
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.LayoutControl;
using Platform::DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl.Design {
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	class LayoutControlModelUpdater {
		public LayoutControlModelUpdater(ModelItem modelItem, LayoutControl control) {
			ModelItem = modelItem;
			Control = control;
			ModelItems = new Dictionary<Platform::System.Windows.FrameworkElement, ModelItem>();
			ModelItemChildren = new Dictionary<ModelItem, ModelItemCollection>();
			ModelItemPositions = new Dictionary<ModelItem, int>();
		}
		public void UpdateModel() {
			ExtractModelItems(ModelItem, Control);
			BuildModel(ModelItem, Control);
			UpdateModelItem(ModelItem, Control);
			CheckRemovedModelItems();
			CheckInsertedModelItems();
		}
		public LayoutControl Control { get; private set; }
		public ModelItem ModelItem { get; private set; }
		protected void BuildModel(ModelItem item, ILayoutGroup control) {
			bool isExistingItem = ModelItems.ContainsValue(item);
			ModelItemCollection itemChildren = isExistingItem ? ModelItemChildren[item] : item.Properties["Children"].Collection;
			FrameworkElements controlChildren = control.GetLogicalChildren(false);
			for (int i = 0; i < controlChildren.Count; i++) {
				Platform::System.Windows.FrameworkElement controlChild = controlChildren[i];
				ModelItem itemChild;
				if (!ModelItems.TryGetValue(controlChild, out itemChild)) {
					itemChild = ModelFactory.CreateItem(Context, controlChild);
					InitModelItem(itemChild, controlChild);
					itemChildren.Insert(i, itemChild);
				}
				else {
					int itemIndex = itemChildren.IndexOf(itemChild);
					if (itemIndex == -1) {
						if (itemChild.Parent != null) {
							if (isExistingItem || item.Parent != itemChild.Parent)
								ModelItemRemoved(itemChild);
							ModelItemChildren[itemChild.Parent].Remove(itemChild); 
						}
						itemChildren.Insert(i, itemChild);
						if (isExistingItem)
							ModelItemInserted(controlChild);
					}
					else
						if (itemIndex != i) 
							itemChildren.Move(itemIndex, i);
				}
				UpdateModelItem(itemChild, controlChild);
				if (controlChild.IsLayoutGroup())
					BuildModel(itemChild, (ILayoutGroup)controlChild);
			}
			for (int i = itemChildren.Count - 1; i > controlChildren.Count - 1; i--) {
				ModelItem itemChild = itemChildren[i];
				if (isExistingItem && !IsDeleted(itemChild))
					ModelItemRemoved(itemChild);
				RemoveChildrenOfDeletedGroups(itemChild);
				itemChildren.RemoveAt(i);
			}
		}
		protected void RemoveChildrenOfDeletedGroups(ModelItem item) {
			ModelItemCollection itemChildren;
			if (!ModelItemChildren.TryGetValue(item, out itemChildren))
				return;
			bool isDeletedItem = IsDeleted(item);
			for (int i = itemChildren.Count - 1; i >= 0; i--) {
				RemoveChildrenOfDeletedGroups(itemChildren[i]);
				if (isDeletedItem)
					itemChildren.RemoveAt(i);
			}
		}
		protected void ExtractModelItems(ModelItem item, ILayoutGroup control) {
			ModelItemCollection children = item.Properties["Children"].Collection;
			ModelItems.Add(control.Control, item);
			ModelItemChildren.Add(item, children);
			for (int i = 0; i < children.Count; i++) {
				ModelItem child = children[i];
				var element = child.GetCurrentValue() as Platform::System.Windows.FrameworkElement;
				ModelItemPositions.Add(child, i);
				if (element.IsLayoutGroup())
					ExtractModelItems(child, (ILayoutGroup)element);
				else
					ModelItems.Add(element, child);
			}
		}
		protected void InitModelItem(ModelItem item, Platform::System.Windows.FrameworkElement control) {
			if (control.IsLayoutGroup()) {
				foreach (string propertyName in ((ILayoutGroup)control).GetDependencyPropertiesWithOverriddenDefaultValue())
					item.Properties[propertyName].ClearValue();
			}
		}
		protected void UpdateModelItem(ModelItem item, Platform::System.Windows.FrameworkElement control) {
			if (control != Control) {
				item.UpdateProperty("HorizontalAlignment", Platform::System.Windows.FrameworkElement.HorizontalAlignmentProperty, control);
				item.UpdateProperty("VerticalAlignment", Platform::System.Windows.FrameworkElement.VerticalAlignmentProperty, control);
			}
			if (control == Control || control.IsLayoutGroup()) {
				item.UpdateProperty("Orientation", LayoutGroup.OrientationProperty, control);
			}
		}
		protected void CheckInsertedModelItems() {
			if (InsertedItems == null)
				return;
			foreach (Platform::System.Windows.FrameworkElement element in InsertedItems)
				if (element.Parent != null)
					((ILayoutGroup)element.Parent).Children.Remove(element);
		}
		protected void CheckRemovedModelItems() {
			if (RemovedItems == null)
				return;
			foreach (ModelItem parent in RemovedItems.Keys) {
				var parentControl = GetControl(parent) as ILayoutGroup;
				if (parentControl != null && parentControl.Control.GetParent() != null &&
					ModelItemChildren[parent].Count == parentControl.GetLogicalChildren(false).Count)
					foreach (int position in GetModelItemPositions(RemovedItems[parent], true))
						parentControl.Children.Insert(position, CreateDummy(position));
			}
		}
		protected void ModelItemInserted(Platform::System.Windows.FrameworkElement element) {
			if (InsertedItems == null)
				InsertedItems = new List<Platform::System.Windows.FrameworkElement>();
			InsertedItems.Add(element);
		}
		protected void ModelItemRemoved(ModelItem item) {
			if (RemovedItems == null)
				RemovedItems = new Dictionary<ModelItem, List<ModelItem>>();
			List<ModelItem> items;
			if (!RemovedItems.TryGetValue(item.Parent, out items))
				items = RemovedItems[item.Parent] = new List<ModelItem>();
			items.Add(item);
		}
		protected Platform::System.Windows.FrameworkElement GetControl(ModelItem item) {
			foreach (var pair in ModelItems)
				if (pair.Value == item)
					return pair.Key;
			return null;
		}
		protected List<int> GetModelItemPositions(List<ModelItem> items, bool sortPositions) {
			var result = new List<int>();
			foreach (ModelItem item in items)
				result.Add(ModelItemPositions[item]);
			if (sortPositions)
				result.Sort();
			return result;
		}
		protected bool IsDeleted(ModelItem item) {
			return GetControl(item).Parent == null;
		}
		protected EditingContext Context { get { return ModelItem.Context; } }
		protected Dictionary<Platform::System.Windows.FrameworkElement, ModelItem> ModelItems { get; private set; }
		protected Dictionary<ModelItem, ModelItemCollection> ModelItemChildren { get; private set; }
		protected Dictionary<ModelItem, int> ModelItemPositions { get; private set; }
		private Platform::System.Windows.UIElement CreateDummy(object data) {
			return new Platform::System.Windows.Controls.TextBlock { Text = data.ToString() };
		}
		private List<Platform::System.Windows.FrameworkElement> InsertedItems { get; set; }
		private Dictionary<ModelItem, List<ModelItem>> RemovedItems { get; set; }
		internal Dictionary<ModelItem, ModelItem[]> StoreLayout() {
			var result = new Dictionary<ModelItem, ModelItem[]>();
			StoreLayout(ModelItem, result);
			return result;
		}
		internal void StoreLayout(ModelItem modelItem, Dictionary<ModelItem, ModelItem[]> storage) {
			ModelItemCollection modelItemChildren = modelItem.Properties["Children"].Collection;
			var children = new ModelItem[modelItemChildren.Count];
			modelItemChildren.CopyTo(children, 0);
			storage.Add(modelItem, children);
			foreach (ModelItem item in modelItemChildren)
				if (item.IsItemOfType(typeof(Platform::DevExpress.Xpf.LayoutControl.LayoutGroup)) &&
					!item.IsItemOfType(typeof(Platform::DevExpress.Xpf.LayoutControl.LayoutControl)))
					StoreLayout(item, storage);
		}
		internal void RestoreLayout(Dictionary<ModelItem, ModelItem[]> storage) {
			foreach (KeyValuePair<ModelItem, ModelItem[]> storageItem in storage) {
				ModelItemCollection modelItemChildren = storageItem.Key.Properties["Children"].Collection;
				ModelItem[] children = storageItem.Value;
				for (int i = 0; i < children.Length; i++) {
					ModelItem child = children[i];
					if (i > modelItemChildren.Count - 1 || modelItemChildren[i] != child) {
						child.Parent.Properties["Children"].Collection.Remove(child);
						modelItemChildren.Insert(i, child);
					}
				}
			}
		}
	}
}
