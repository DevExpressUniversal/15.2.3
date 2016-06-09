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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	public enum BarInsertMode {
		Add,
		Insert
	}
	#region BarGenerationManagerBase<TControl, TCommandId> (abstract class)
	public abstract class BarGenerationManagerBase<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		BarGenerationStrategy generationStrategy;
		ControlCommandBarCreator barCreator;
		Component barContainer;
		ControlCommandBarControllerBase<TControl, TCommandId> barController;
		BarCreationContextBase creationContext;
		BarInsertMode insertMode;
		protected BarGenerationManagerBase(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController) {
			this.barCreator = creator;
			this.barContainer = container;
			this.barController = barController;
			this.creationContext = CreateBarCreationContext();
		}
		public ControlCommandBarCreator BarCreator { get { return barCreator; } }
		public Component BarContainer { get { return barContainer; } }
		public ControlCommandBarControllerBase<TControl, TCommandId> BarController { get { return barController; } }
		protected BarGenerationStrategy GenerationStrategy { get { return generationStrategy; } }
		public abstract BarManager BarManager { get; }
		protected virtual BarCreationContextBase CreationContext { get { return creationContext; } }
		public BarInsertMode InsertMode { get { return insertMode; } set { insertMode = value; } }
		public bool CreateBar(BarGenerationStrategy generationStrategy) {
			this.generationStrategy = generationStrategy;
			Component barItemGroup = CreateBarItemGroup(BarCreator);
			if (barItemGroup == null)
				return false;
			InitializeBarItemGroup(barItemGroup, BarController);
			List<BarItem> newBarItems = GetNewGroupBarItems(BarCreator);
			List<BarItem> containerBarItems = GetBarContainerBarItems();
			List<BarItem> intersectedItems = GetIntersectedItems(containerBarItems, BarController.BarItems);
			List<BarItem> mergedItems = MergeExistingAndNewBarItems(intersectedItems, newBarItems);
			FilterMergedItems(mergedItems, barItemGroup);
			BeginUpdate();
			try {
				AddBarItemsCore(mergedItems, containerBarItems);
				AddItemsToBarItemGroup(barItemGroup, mergedItems);
			}
			finally {
				EndUpdate();
			}
			CreateAdditionalInfrastructure(mergedItems);
			return true;
		}
		protected virtual void CreateAdditionalInfrastructure(List<BarItem> mergedItems) {
		}
		protected internal virtual void BeginUpdate() {
			if (BarManager != null)
				BarManager.BeginUpdate();
		}
		protected internal virtual void EndUpdate() {
			if (BarManager != null)
				BarManager.EndUpdate();
		}
		public void ClearExistingItems(BarGenerationStrategy generationStrategy) {
			this.generationStrategy = generationStrategy;
			ClearExistingItemsCore();
		}
		protected abstract void ClearExistingItemsCore();
		protected virtual void FilterMergedItems(List<BarItem> mergedItems, Component barItemGroup) {
		}
		protected internal virtual void AddBarItemsCore(List<BarItem> items, List<BarItem> containerBarItems) {
			foreach (BarItem item in items) {
				if (!containerBarItems.Contains(item)) {
					BarController.BarItems.Add(item);
					AddBarItemToContainer(item);
					IBarSubItem subItem = item as IBarSubItem;
					if (subItem != null) {
						List<BarItem> subItems = subItem.GetSubItems();
						AddBarItemsCore(subItems, containerBarItems);
						BarSubItem subItem1 = item as BarSubItem;
						if (subItem1 != null)
							foreach (BarItem i in subItems)
								AddItemLink(subItem1.ItemLinks, i);
					}
				}
			}
		}
		protected virtual void AddBarItemToContainer(BarItem item) {
			GenerationStrategy.AddToContainer(item);
			BarButtonItem btnItem = item as BarButtonItem;
			if (btnItem != null) {
				GalleryDropDown dropDown = btnItem.DropDownControl as GalleryDropDown; 
				if (dropDown != null) {
					GenerationStrategy.AddToContainer(dropDown);
					dropDown.Manager = BarManager;
				}
			}
		}
		protected virtual void InitializeBarItemGroup(Component barItemGroup, ControlCommandBarControllerBase<TControl, TCommandId> BarController) {
		}
		List<BarItem> GetNewGroupBarItems(ControlCommandBarCreator creator) {
			List<BarItem> items = new List<BarItem>();
			creator.PopulateItems(items, CreationContext);
			return items;
		}
		List<BarItem> GetIntersectedItems(List<BarItem> barManagerItems, List<BarItem> controllerItems) {
			List<BarItem> result = new List<BarItem>();
			int count = barManagerItems.Count;
			for (int i = 0; i < count; i++) {
				BarItem barManagerItem = barManagerItems[i];
				int itemIndx = controllerItems.IndexOf(barManagerItem);
				if (itemIndx != -1)
					result.Add(barManagerItem);
			}
			return result;
		}
		protected List<BarItem> MergeExistingAndNewBarItems(List<BarItem> existedItems, List<BarItem> newItems) {
			List<BarItem> result = new List<BarItem>();
			int count = newItems.Count;
			for (int i = 0; i < count; i++) {
				BarItem item = newItems[i];
				BarItem existingItem = GetSameBarItem(existedItems, item);
				if (existingItem != null)
					result.Add(existingItem);
				else
					result.Add(item);
			}
			return result;
		}
		protected void AddItemLink(BarItemLinkCollection links, BarItem item) {
			BarItem existingBarItem = GetSameBarItem(links, item);
			if (existingBarItem != null) 
				return;
			existingBarItem = GetSameBarItem(BarManager.Items, item);
			BarItemLink link;
			if (existingBarItem == null)
				link = links.Add(item); 
			else
				link = links.Add(existingBarItem); 
			PerformItemLinkSetup(link);
		}
		void PerformItemLinkSetup(BarItemLink link) {
			if (link == null)
				return;
			ICommandBarItem barItem = link.Item as ICommandBarItem;
			if (barItem != null) {
				GenerationStrategy.OnComponentChanging(link, null);
				SetupItemLink(barItem, link);
				GenerationStrategy.OnComponentChanged(link, null, null, null);
			}
		}
		protected internal virtual void SetupItemLink(ICommandBarItem barItem, BarItemLink link) {
			IBarDefaultLinkSettings defaultLinkSettings = barItem as IBarDefaultLinkSettings;
			if (defaultLinkSettings != null)
				link.ActAsButtonGroup = defaultLinkSettings.ActAsButtonGroup;
		}
		protected void AddBarItems(BarItems where, BarItem[] items) {
			int count = items.Length;
			for (int i = 0; i < count; i++) {
				IBarSubItem subItem = items[i] as IBarSubItem;
				if (subItem != null) {
					List<BarItem> subItems = subItem.GetSubItems();
					AddBarItems(where, subItems.ToArray());
				}
				AddBarItem(where, items[i]);
			}
		}
		protected void AddBarItem(BarItems where, BarItem item) {
			if (GetSameBarItem(where, item) != null)
				return;
			where.Add(item);
		}
		BarItem GetSameBarItem(BarItemLinkCollection links, BarItem item) {
			int count = links.Count;
			for (int i = 0; i < count; i++)
				if (AreSameBarItems(links[i].Item, item))
					return links[i].Item;
			return null;
		}
		BarItem GetSameBarItem(List<BarItem> items, BarItem item) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				if (AreSameBarItems(items[i], item))
					return items[i];
			return null;
		}
		BarItem GetSameBarItem(BarItems items, BarItem item) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				if (AreSameBarItems(items[i], item))
					return items[i];
			return null;
		}
		bool AreSameBarItems(BarItem item1, BarItem item2) {
			ICommandBarItem commandBarItem1 = item1 as ICommandBarItem;
			if (commandBarItem1 == null)
				return false;
			ICommandBarItem commandBarItem2 = item2 as ICommandBarItem;
			if (commandBarItem2 == null)
				return false;
			return commandBarItem1.IsEqual(commandBarItem2);
		}
		protected void ClearBarManagerContent() {
			if (BarManager != null) {
				for (int i = BarManager.Items.Count - 1; i >= 0; i--)
					GenerationStrategy.RemoveFromContainer(BarManager.Items[i]);
				BarManager.Items.Clear();
				for (int i = BarManager.Bars.Count - 1; i >= 0; i--)
					GenerationStrategy.RemoveFromContainer(BarManager.Bars[i]);
				BarManager.Bars.Clear();
			}
		}
		protected abstract List<BarItem> GetBarContainerBarItems();
		protected abstract void AddItemsToBarItemGroup(Component barItemGroup, List<BarItem> items);
		protected abstract Component CreateBarItemGroup(ControlCommandBarCreator creator);
		protected abstract BarCreationContextBase CreateBarCreationContext();
	}
	#endregion
}
