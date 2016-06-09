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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Docking.Design {
	class DockLayoutManagerModelUpdater {
		public DockLayoutManager Control { get; private set; }
		public ModelItem ModelItem { get; private set; }
		public DockLayoutManagerModelUpdater(ModelItem modelItem, DockLayoutManager control) {
			ModelItem = modelItem;
			Control = control;
			ModelItems = new Dictionary<FrameworkElement, ModelItem>();
			ModelItemChildren = new Dictionary<ModelItem, ModelItemCollection>();
			ModelItemPositions = new Dictionary<ModelItem, int>();
		}
		public void UpdateModel() {
			ModelItems.Add(Control, ModelItem);
			ModelItemCollection floatGroupsModel = ModelItem.Properties["FloatGroups"].Collection;
			ModelItemCollection autoHideGroupsModel = ModelItem.Properties["AutoHideGroups"].Collection;
			ModelItemCollection closedPanelsModel = ModelItem.Properties["ClosedPanels"].Collection;
			if(Control.LayoutRoot != null) {
				if(!ModelItem.Content.IsSet || ModelItem.Content.Value.As<LayoutGroup>() != Control.LayoutRoot)
					ModelItem.Content.SetValue(ModelFactory.CreateItem(Context, Control.LayoutRoot));
				ExtractModelItems(ModelItem.Content.Value, Control.LayoutRoot);
			}
			ExtractModelItems(floatGroupsModel);
			ExtractModelItems(autoHideGroupsModel);
			ExtractModelItems(closedPanelsModel);
			ReadOnlyCollection<BaseLayoutItem> floatGroups = new ReadOnlyCollection<BaseLayoutItem>(Control.FloatGroups.ToArray());
			ReadOnlyCollection<BaseLayoutItem> autoHideGroups = new ReadOnlyCollection<BaseLayoutItem>(Control.AutoHideGroups.ToArray());
			ReadOnlyCollection<BaseLayoutItem> closePanels = new ReadOnlyCollection<BaseLayoutItem>(Control.ClosedPanels.ToArray());
			if(Control.LayoutRoot != null)
				BuildModel(ModelItem.Content.Value, Control.LayoutRoot);
			else
				ModelItem.Content.ClearValue();
			BuildModel(floatGroupsModel, floatGroups);
			BuildModel(autoHideGroupsModel, autoHideGroups);
			BuildModel(closedPanelsModel, closePanels);
			UpdateModelItem(ModelItem.Content.Value, Control.LayoutRoot);
			CheckRemovedModelItems();
			CheckInsertedModelItems();
		}
		protected void BuildModel(ModelItemCollection itemChildren, ReadOnlyCollection<BaseLayoutItem> controlChildren) {
			for(int i = 0; i < controlChildren.Count; i++) {
				var controlChild = controlChildren[i];
				ModelItem itemChild;
				if(!ModelItems.TryGetValue(controlChild, out itemChild)) {
					itemChild = ModelFactory.CreateItem(Context, controlChild);
					itemChildren.Insert(i, itemChild);
				}
				else {
					int itemIndex = itemChildren.IndexOf(itemChild);
					if(itemIndex == -1) {
						if(itemChild.Parent != null) {
							if(ModelItem != itemChild.Parent)
								ModelItemRemoved(itemChild);
							ModelItemChildren[itemChild.Parent].Remove(itemChild);
						}
						itemChildren.Insert(i, itemChild);
						ModelItemInserted(controlChild);
					}
					else
						if(itemIndex != i)
							itemChildren.Move(itemIndex, i);
				}
				UpdateModelItem(itemChild, controlChild);
				if(controlChild is LayoutGroup)
					BuildModel(itemChild, (LayoutGroup)controlChild);
			}
			for(int i = itemChildren.Count - 1; i > controlChildren.Count - 1; i--) {
				ModelItem itemChild = itemChildren[i];
				if(!IsDeleted(itemChild))
					ModelItemRemoved(itemChild);
				RemoveChildrenOfDeletedGroups(itemChild);
				itemChildren.RemoveAt(i);
			}
		}
		protected void BuildModel(ModelItem item, LayoutGroup control) {
			bool isExistingItem = ModelItems.ContainsValue(item);
			ModelItemCollection itemChildren = isExistingItem ? ModelItemChildren[item] : item.Properties["Items"].Collection;
			var controlChildren = control.GetChildren(); 
			for(int i = 0; i < controlChildren.Count; i++) {
				FrameworkElement controlChild = controlChildren[i];
				ModelItem itemChild;
				if(!ModelItems.TryGetValue(controlChild, out itemChild)) {
					itemChild = ModelFactory.CreateItem(Context, controlChild);
					itemChildren.Insert(i, itemChild);
				}
				else {
					int itemIndex = itemChildren.IndexOf(itemChild);
					if(itemIndex == -1) {
						if(itemChild.Parent != null) {
							if(isExistingItem || item.Parent != itemChild.Parent)
								ModelItemRemoved(itemChild);
							ModelItemChildren[itemChild.Parent].Remove(itemChild);
						}
						itemChildren.Insert(i, itemChild);
						if(isExistingItem)
							ModelItemInserted(controlChild);
					}
					else
						if(itemIndex != i)
							itemChildren.Move(itemIndex, i);
				}
				UpdateModelItem(itemChild, controlChild);
				if(controlChild is LayoutGroup && isExistingItem)
					BuildModel(itemChild, (LayoutGroup)controlChild);
			}
			for(int i = itemChildren.Count - 1; i > controlChildren.Count - 1; i--) {
				ModelItem itemChild = itemChildren[i];
				if(isExistingItem && !IsDeleted(itemChild))
					ModelItemRemoved(itemChild);
				RemoveChildrenOfDeletedGroups(itemChild);
				itemChildren.RemoveAt(i);
			}
		}
		protected void RemoveChildrenOfDeletedGroups(ModelItem item) {
			ModelItemCollection itemChildren;
			if(!ModelItemChildren.TryGetValue(item, out itemChildren))
				return;
			bool isDeletedItem = IsDeleted(item);
			for(int i = itemChildren.Count - 1; i >= 0; i--) {
				RemoveChildrenOfDeletedGroups(itemChildren[i]);
				if(isDeletedItem)
					itemChildren.RemoveAt(i);
			}
		}
		protected void ExtractModelItems(ModelItemCollection children) {
			for(int i = 0; i < children.Count; i++) {
				ModelItem child = children[i];
				var element = child.GetCurrentValue() as FrameworkElement;
				ModelItemPositions.Add(child, i);
				if(element is LayoutGroup)
					ExtractModelItems(child, (LayoutGroup)element);
				else
					ModelItems.Add(element, child);
			}
		}
		protected void ExtractModelItems(ModelItem item, LayoutGroup control) {
			ModelItemCollection children = item.Properties["Items"].Collection;
			ModelItems.Add(control, item);
			ModelItemChildren.Add(item, children);
			for(int i = 0; i < children.Count; i++) {
				ModelItem child = children[i];
				var element = child.GetCurrentValue() as FrameworkElement;
				ModelItemPositions.Add(child, i);
				if(element is LayoutGroup)
					ExtractModelItems(child, (LayoutGroup)element);
				else
					ModelItems.Add(element, child);
			}
		}
		protected void UpdateModelItem(ModelItem item, FrameworkElement control) {
			if(control is FloatGroup) {
				FloatGroup floatGroup = (FloatGroup)control;
				if(floatGroup != null) {
					Point floatLocation = MathHelper.Round(floatGroup.FloatLocation);
					Size floatSize = MathHelper.Round(floatGroup.FloatSize);
					item.Properties["FloatLocation"].SetValue(floatLocation);
					item.Properties["FloatSize"].SetValue(floatSize);
				}
			}
			if( control is LayoutGroup) {
				item.UpdateProperty("Orientation", LayoutGroup.OrientationProperty, control);
			}
		}
		protected void CheckInsertedModelItems() {
			if(InsertedItems == null)
				return;
			foreach(BaseLayoutItem element in InsertedItems) {
				if(element.Parent != null)
					((LayoutGroup)element.Parent).Items.Remove(element);
				if(element is FloatGroup)
					Control.FloatGroups.Remove((FloatGroup)element);
			}
		}
		protected void CheckRemovedModelItems() {
			if(RemovedItems == null)
				return;
			foreach(ModelItem parent in RemovedItems.Keys) {
				var dockLayoutManager = GetControl(parent) as DockLayoutManager;
				if(dockLayoutManager != null) {
					List<ModelItem> items = RemovedItems[parent];
					foreach(ModelItem item in items) {
						if(item.Is<FloatGroup>())
							Control.FloatGroups.Add(new FloatGroup());
						if(item.Is<AutoHideGroup>())
							Control.AutoHideGroups.Add(new AutoHideGroup());
					}
				}
				var parentControl = GetControl(parent) as LayoutGroup;
				if(parentControl != null && ((FrameworkElement)parentControl).Parent != null &&
					ModelItemChildren[parent].Count == parentControl.GetChildren().Count)
					foreach(int position in GetModelItemPositions(RemovedItems[parent], true))
						parentControl.Items.Insert(position, CreateDummy(parentControl));
			}
		}
		protected void ModelItemInserted(FrameworkElement element) {
			if(InsertedItems == null)
				InsertedItems = new List<FrameworkElement>();
			InsertedItems.Add(element);
		}
		protected void ModelItemRemoved(ModelItem item) {
			if(RemovedItems == null)
				RemovedItems = new Dictionary<ModelItem, List<ModelItem>>();
			List<ModelItem> items;
			if(!RemovedItems.TryGetValue(item.Parent, out items))
				items = RemovedItems[item.Parent] = new List<ModelItem>();
			items.Add(item);
		}
		protected FrameworkElement GetControl(ModelItem item) {
			foreach(var pair in ModelItems)
				if(pair.Value == item)
					return pair.Key;
			return null;
		}
		protected List<int> GetModelItemPositions(List<ModelItem> items, bool sortPositions) {
			var result = new List<int>();
			foreach(ModelItem item in items)
				result.Add(ModelItemPositions[item]);
			if(sortPositions)
				result.Sort();
			return result;
		}
		protected bool IsDeleted(ModelItem item) {
			return GetControl(item).Parent == null;
		}
		protected EditingContext Context { get { return ModelItem.Context; } }
		protected Dictionary<FrameworkElement, ModelItem> ModelItems { get; private set; }
		protected Dictionary<ModelItem, ModelItemCollection> ModelItemChildren { get; private set; }
		protected Dictionary<ModelItem, int> ModelItemPositions { get; private set; }
		private BaseLayoutItem CreateDummy(LayoutGroup group) {
			switch(group.ItemType) {
				case LayoutItemType.Group:
					if(group.IsControlItemsHost) return new LayoutControlItem();
					break;
				case LayoutItemType.DocumentPanelGroup:
					return new DocumentPanel();
			}
			return new LayoutPanel();
		}
		private List<FrameworkElement> InsertedItems { get; set; }
		private Dictionary<ModelItem, List<ModelItem>> RemovedItems { get; set; }
	}
}
