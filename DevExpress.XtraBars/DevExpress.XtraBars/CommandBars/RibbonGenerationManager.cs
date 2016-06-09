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
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	#region RibbonGenerationManager<TControl, TCommandId> (abstract class)
	public abstract class RibbonGenerationManager<TControl, TCommandId> : BarGenerationManagerBase<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		protected RibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController)
			: base(creator, container, barController) {
		}
		protected RibbonControl RibbonControl { get { return BarContainer as RibbonControl; } }
		Type SupportedRibbonPageCategoryType { get { return BarCreator.SupportedRibbonPageCategoryType; } }
		Type SupportedRibbonPageType { get { return BarCreator.SupportedRibbonPageType; } }
		Type SupportedRibbonPageGroupType { get { return BarCreator.SupportedRibbonPageGroupType; } }
		public override BarManager BarManager { get { return RibbonControl.Manager; } }
		protected abstract TCommandId EmptyCommandId { get; }
		protected override BarCreationContextBase CreateBarCreationContext() {
			return new RibbonBarCreationContext();
		}
		protected override void CreateAdditionalInfrastructure(List<BarItem> mergedItems) {
			if (BarCreator.ShouldCreateRibbonMiniToolbar && mergedItems != null && mergedItems.Count > 0) {
				var toolbars = GetRibbonMiniToolbars();
				if (toolbars != null) {
					foreach (var miniToolbar in toolbars) {
						mergedItems.ForEach(item => {
							if (BarCreator.ShouldAddItemToRibbonMiniToolbar(item, miniToolbar))
								miniToolbar.ItemLinks.Add(item);
						});
					}
				}
			}
		}
		CommandBasedRibbonMiniToolbar<TControl, TCommandId>[] GetRibbonMiniToolbars() {
			List<CommandBasedRibbonMiniToolbar<TControl, TCommandId>> result = new List<CommandBasedRibbonMiniToolbar<TControl,TCommandId>>();
			foreach (var item in RibbonControl.MiniToolbars) {
				foreach (var miniToolBarType in BarCreator.SupportedRibbonMiniToolbarTypes) {
					if (item.GetType() == miniToolBarType)
						result.Add(item as CommandBasedRibbonMiniToolbar<TControl, TCommandId>);
				}
				if (result.Count == BarCreator.SupportedRibbonMiniToolbarTypes.Length)
					return result.ToArray();
			}
			List<Type> typesToCreate = new List<Type>();
			foreach (var miniToolBarType in BarCreator.SupportedRibbonMiniToolbarTypes) {
				bool found = false;
				foreach (var item in result) {
					if (miniToolBarType == item.GetType()) {
						found = true;
						break;
					}
				}
				if (!found)
					typesToCreate.Add(miniToolBarType);
			}
			foreach (var item in typesToCreate) {
				var miniToolBar = Activator.CreateInstance(item) as CommandBasedRibbonMiniToolbar<TControl, TCommandId>;
				if (miniToolBar != null) {
					result.Add(miniToolBar);
					miniToolBar.Control = BarController.Control;
					RibbonControl.MiniToolbars.Add(miniToolBar);
					GenerationStrategy.AddToContainer(miniToolBar);
				}
			}
			return result.ToArray();
		}
		protected override Component CreateBarItemGroup(ControlCommandBarCreator creator) {
			CommandBasedRibbonPageGroup pageGroup = GetRibbonPageGroup();
			if (pageGroup == null)
				return null;
			if (pageGroup.Page == null) {
				CommandBasedRibbonPage page = GetRibbonPage();
				if (page.Category == null) {
					CommandBasedRibbonPageCategory category = GetRibbonPageCategory();
					if (category != null) {
						RibbonControl.PageCategories.Add(category);
						if (InsertMode == BarInsertMode.Add)
							category.Pages.Add(page);
						else
							category.Pages.Insert(0, page);
					}
					else {
						if (InsertMode == BarInsertMode.Add)
							RibbonControl.Pages.Add(page);
						else
							RibbonControl.Pages.Insert(0, page);
					}
				}
				page.Groups.Add(pageGroup);
			}
			RibbonControl.SelectedPage = pageGroup.Page;
			return pageGroup;
		}
		protected override void InitializeBarItemGroup(Component barItemGroup, ControlCommandBarControllerBase<TControl, TCommandId> barController) {
			base.InitializeBarItemGroup(barItemGroup, barController);
			ControlCommandBasedRibbonPageGroup<TControl, TCommandId> pageGroup = barItemGroup as ControlCommandBasedRibbonPageGroup<TControl, TCommandId>;
			if (pageGroup == null)
				return;
			pageGroup.Control = barController.Control ;
		}
		protected internal virtual CommandBasedRibbonPageCategory GetRibbonPageCategory() {
			CommandBasedRibbonPageCategory category = FindCommandRibbonPageCategory();
			if (category == null) {
				category = BarCreator.CreateRibbonPageCategoryInstance();
				if (category != null)
					GenerationStrategy.AddToContainer(category);
			}
			return category;
		}
		protected internal virtual CommandBasedRibbonPage GetRibbonPage() {
			CommandBasedRibbonPage page = FindCommandRibbonPage();
			if (page == null) {
				page = BarCreator.CreateRibbonPageInstance();
				GenerationStrategy.AddToContainer(page);
			}
			return page;
		}
		protected internal virtual CommandBasedRibbonPageCategory FindCommandRibbonPageCategory() {
			if (!typeof(CommandBasedRibbonPageCategory).IsAssignableFrom(SupportedRibbonPageCategoryType))
				return null;
			foreach (RibbonPageCategory category in RibbonControl.PageCategories)
				if (SupportedRibbonPageCategoryType.IsAssignableFrom(category.GetType()))
					return (CommandBasedRibbonPageCategory)category;
			return null;
		}
		protected internal virtual CommandBasedRibbonPage FindCommandRibbonPage() {
			if (RibbonControl == null)
				return null;
			CommandBasedRibbonPage page = FindCommandRibbonPage(RibbonControl.Pages);
			if (page != null)
				return page;
			foreach (RibbonPageCategory category in RibbonControl.PageCategories) {
				page = FindCommandRibbonPage(category.Pages);
				if (page != null)
					return page;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPage FindCommandRibbonPage(RibbonPageCollection pages) {
			foreach (RibbonPage page in pages)
				if (SupportedRibbonPageType.IsAssignableFrom(page.GetType()))
					return (CommandBasedRibbonPage)page;
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup GetRibbonPageGroup() {
			CommandBasedRibbonPageGroup pageGroup = FindCommandRibbonPageGroup();
			if (pageGroup != null)
				return pageGroup;
			pageGroup = BarCreator.CreateRibbonPageGroupInstance();
			GenerationStrategy.AddToContainer(pageGroup);
			return pageGroup;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroup() {
			if (RibbonControl == null)
				return null;
			CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroupInPages(RibbonControl.Pages);
			if (result != null)
				return result;
			foreach (RibbonPageCategory category in RibbonControl.PageCategories) {
				result = FindCommandRibbonPageGroupInPages(category.Pages);
				if (result != null)
					return result;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroupInPages(RibbonPageCollection pages) {
			int pagesCount = pages.Count;
			for (int i = 0; i < pagesCount; i++) {
				CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroupInPage(pages[i]);
				if (result != null)
					return result;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroupInPage(RibbonPage page) {
			int groupsCount = page.Groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				RibbonPageGroup group = page.Groups[i];
				if (SupportedRibbonPageGroupType.IsAssignableFrom(group.GetType()))
					return (CommandBasedRibbonPageGroup)group;
			}
			return null;
		}
		protected override List<BarItem> GetBarContainerBarItems() {
			List<BarItem> result = new List<BarItem>();
			RibbonBarItems items = RibbonControl.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
				result.Add(items[i]);
			return result;
		}
		protected override void AddItemsToBarItemGroup(Component barItemGroup, List<BarItem> items) {
			CommandBasedRibbonPageGroup pageGroup = barItemGroup as CommandBasedRibbonPageGroup;
			foreach (BarItem item in items) {
				if(ShouldAddItemToBarItemGroup(pageGroup, item)) AddItemToBarItemGroup(pageGroup, item);
			}
		}
		protected virtual bool ShouldAddItemToBarItemGroup(CommandBasedRibbonPageGroup pageGroup, BarItem item) {
			return true;
		}
		protected override void FilterMergedItems(List<BarItem> mergedItems, Component barItemGroup) {
			CommandBasedRibbonPageGroup pageGroup = barItemGroup as CommandBasedRibbonPageGroup;
			if (pageGroup == null)
				return;
			for (int i = mergedItems.Count - 1; i >= 0; i--) {
				if (ShouldRemoveItem(mergedItems[i], pageGroup))
					mergedItems.RemoveAt(i);
			}
		}
		bool ShouldRemoveItem(BarItem item, CommandBasedRibbonPageGroup pageGroup) {
			ControlCommandBarButtonItem<TControl, TCommandId> controlCommandBarButtonItem = item as ControlCommandBarButtonItem<TControl, TCommandId>;
			if (controlCommandBarButtonItem != null) {
				ControlCommandBasedRibbonPageGroup<TControl, TCommandId> controlCommandPageGroup = pageGroup as ControlCommandBasedRibbonPageGroup<TControl, TCommandId>;
				if (controlCommandPageGroup != null &&
					Object.Equals(controlCommandPageGroup.CommandId, controlCommandBarButtonItem.GetCommandId()) &&
					!Object.Equals(controlCommandPageGroup.CommandId, EmptyCommandId))
					return true;
			}
			return false;
		}
		protected virtual void AddItemToBarItemGroup(CommandBasedRibbonPageGroup pageGroup, BarItem item) {
			IBarButtonGroupMember buttonGroupMember = item as IBarButtonGroupMember;
			if (buttonGroupMember != null) {
				BarButtonGroup buttonGroup = FindButtonGroupByTag(pageGroup, buttonGroupMember.ButtonGroupTag);
				if (buttonGroup != null)
					AddItemLink(buttonGroup.ItemLinks, item);
				else
					AddItemLink(pageGroup.ItemLinks, item);
			}
			else
				AddItemLink(pageGroup.ItemLinks, item);
			IBarSubItem subItem = item as IBarSubItem;
			if (subItem != null) {
				List<BarItem> subItems = subItem.GetSubItems();
				AddBarItems(RibbonControl.Items, subItems.ToArray());
			}
		}
		BarButtonGroup FindButtonGroupByTag(CommandBasedRibbonPageGroup pageGroup, object tag) {
			if (tag == null)
				return null;
			RibbonPageGroupItemLinkCollection links = pageGroup.ItemLinks;
			int count = links.Count;
			for (int i = 0; i < count; i++) {
				BarButtonGroup item = links[i].Item as BarButtonGroup;
				if (item != null && Object.Equals(item.Tag, tag))
					return item;
			}
			return CreateButtonGroup(pageGroup, tag);
		}
		BarButtonGroup CreateButtonGroup(CommandBasedRibbonPageGroup pageGroup, object tag) {
			BarButtonGroup buttonGroup = new BarButtonGroup();
			buttonGroup.Tag = tag;
			pageGroup.ItemLinks.Add(buttonGroup);
			GenerationStrategy.AddToContainer(buttonGroup);
			return buttonGroup;
		}
		protected override void ClearExistingItemsCore() {
			ClearRibbonContent();
			ClearBarManagerContent();
		}
		void ClearRibbonContent() {
			if (RibbonControl == null)
				return;
			ClearRibbonCategories();
			ClearRibbonPages(RibbonControl.Pages);
			if (RibbonControl.Toolbar != null)
				RibbonControl.Toolbar.ItemLinks.Clear();
		}
		void ClearRibbonCategories() {
			for (int i = RibbonControl.PageCategories.Count - 1; i >= 0; i--) {
				ClearRibbonPages(RibbonControl.PageCategories[i].Pages);
				GenerationStrategy.RemoveFromContainer(RibbonControl.PageCategories[i]);
			}
			RibbonControl.PageCategories.Clear();
		}
		void ClearRibbonPages(RibbonPageCollection pages) {
			for (int i = pages.Count - 1; i >= 0; i--) {
				ClearRibbonPageGroups(pages[i].Groups);
				GenerationStrategy.RemoveFromContainer(pages[i]);
			}
			pages.Clear();
		}
		void ClearRibbonPageGroups(RibbonPageGroupCollection groups) {
			for (int i = groups.Count - 1; i >= 0; i--) {
				ClearRibbonPageGroupContent(groups[i]);
				GenerationStrategy.RemoveFromContainer(groups[i]);
			}
			groups.Clear();
		}
		void ClearRibbonPageGroupContent(RibbonPageGroup group) {
			for (int i = group.ItemLinks.Count - 1; i >= 0; i--) {
				BarButtonGroup buttonGroup = group.ItemLinks[i].Item as BarButtonGroup;
				if (buttonGroup != null)
					GenerationStrategy.RemoveFromContainer(buttonGroup);
			}
			group.ItemLinks.Clear();
		}
	}
	#endregion
}
