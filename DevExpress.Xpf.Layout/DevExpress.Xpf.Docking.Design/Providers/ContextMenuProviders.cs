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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
namespace DevExpress.Xpf.Docking.Design {
	enum DefaultLayoutType { 
		Simple, IDE, MDI 
	}
	enum LayoutPanelDock { 
		Fill, Left, Top, Right, Bottom 
	}
	abstract class BaseDockLayoutManagerContextMenuProvider : ContextMenuProviderBase {
		protected ModelItem Item { get; set; }
		protected MenuAction ResetLayoutMenuItem { get; set; }
		protected MenuAction RemoveItemMenuItem { get; set; }
		protected MenuGroup DefaultLayoutMenuGroup { get; set; }
		protected MenuAction ResetPropertiesMenuAction { get; set; }
		protected MenuGroup DefaultMenuGroup { get; set; }
		protected MenuAction FloatMenuItem { get; set; }
		protected MenuGroup AutoHideMenuGroup { get; set; }
		public BaseDockLayoutManagerContextMenuProvider() {
			DefaultMenuGroup = CreateDefaultMenuGroup();
			AutoHideMenuGroup = CreateEnumMenuGroup(DockingLocalizer.GetString(DockingStringId.MenuItemAutoHide), typeof(Dock), false, OnAutoHideExecuted);
			FloatMenuItem = CreateMenuItem(DockingLocalizer.GetString(DockingStringId.MenuItemFloat), OnFloatExecuted, false);
		}
		protected void OnResetLayoutExecuted(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			service.ResetLayout(Item);
		}
		void OnRemoveItemExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.RemoveItem(modelItem);
		}
		protected void OnGroupExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.Group(modelItem);
		}
		protected void CreateResetLayoutMenuItem(MenuGroup group) {
			ResetLayoutMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemResetLayout), false,
				ImageUriHelper.Reset, OnResetLayoutExecuted);
		}
		protected void CreateRemoveItemMenuItem(MenuGroup group) {
			RemoveItemMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemRemoveItem), false, 
				ImageUriHelper.RemoveItem, OnRemoveItemExecute);
		}
		protected MenuAction CreateMenuItem(ObservableCollection<MenuBase> items, string displayName, bool checkable, string imageName, EventHandler<MenuActionEventArgs> execute, bool enabled = true) {
			MenuAction action = CreateMenuItem(items, displayName, checkable, execute);
			if(!string.IsNullOrEmpty(imageName))
				action.ImageUri = ImageUriHelper.GetImageUri(imageName);
			return action;
		}
		protected MenuGroup CreateDefaultMenuGroup() {
			var group = new MenuGroup("LayoutControlMenuItems") { HasDropDown = false };
			DefaultLayoutMenuGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemCreateDefaultLayout), typeof(DefaultLayoutType), false, OnDefaultLayoutExecute);
			ResetPropertiesMenuAction = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemResetCustomization), false, OnResetCustomization);
			ResetPropertiesMenuAction.ImageUri = ImageUriHelper.GetImageUri(ImageUriHelper.Reset);
			return group;
		}
		protected MenuGroup CreateEnumMenuGroup(string displayName, Type enumType, bool checkable, EventHandler<MenuActionEventArgs> execute) {
			var result = new MenuGroup(displayName) { HasDropDown = true };
			foreach(string name in Enum.GetNames(enumType))
				CreateMenuItem(result.Items, name, checkable, execute);
			return result;
		}
		protected MenuAction CreateMenuItem(string displayName, EventHandler<MenuActionEventArgs> execute, bool checkable, bool enabled = true) {
			var result = new MenuAction(displayName);
			result.Checkable = checkable;
			result.Enabled = enabled;
			result.Execute += execute;
			return result;
		}
		void OnDefaultLayoutExecute(object sender, MenuActionEventArgs e) {
			var service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			var defaultLayout = (DefaultLayoutType)GetEnumMenuGroupValue(DefaultLayoutMenuGroup, (MenuAction)sender);
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.RestoreLayout(modelItem, defaultLayout);
		}
		void OnResetCustomization(object sender, MenuActionEventArgs e) {
			var service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			service.ResetProperties();
		}
		protected void OnAutoHideExecuted(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			Dock dock = (Dock)GetEnumMenuGroupValue(AutoHideMenuGroup, (MenuAction)sender);
			service.AutoHideItem(modelItem, dock);
		}
		void OnFloatExecuted(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.FloatItem(modelItem);
		}
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class DockLayoutManagerContextMenuProvider : BaseDockLayoutManagerContextMenuProvider {
		public DockLayoutManagerContextMenuProvider() {
			var group = CreateDefaultMenuGroup();
			Items.Add(group);
		}
		protected override bool IsActive(Microsoft.Windows.Design.Interaction.Selection selection) {
			return (selection.PrimarySelection != null) && selection.PrimarySelection.Is<DockLayoutManager>();
		}
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class LayoutPanelContextMenuProvider : BaseDockLayoutManagerContextMenuProvider {
		LayoutTypes ItemType { get; set; }
		protected override bool IsActive(Microsoft.Windows.Design.Interaction.Selection selection) {
			return (selection.PrimarySelection != null) && selection.PrimarySelection.Is<LayoutPanel>();
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			Item = e.Selection.PrimarySelection;
			BaseLayoutItem layoutItem = Item.As<BaseLayoutItem>();
			var group = new MenuGroup("LayoutPanelMenuItems") { HasDropDown = false };
			if(layoutItem.ItemType == LayoutItemType.Panel) {
				ItemType = LayoutTypes.LayoutPanel;
				AddPanelMenuGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemAddPanel), typeof(LayoutPanelDock), false, OnAddPanelExecute);
			}
			if(layoutItem.ItemType == LayoutItemType.Document) {
				ItemType = LayoutTypes.DocumentPanel;
				AddDocumentMenuAction = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemAddDocument), false,
					ImageUriHelper.AddDocument, OnAddDocumentExecute);
			}
			CreateRemoveItemMenuItem(group);
			RemoveItemMenuItem.DisplayName = DockingLocalizer.GetString(DockingStringId.MenuItemRemovePanel);
			HidePanelMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemHidePanel), false,
				ImageUriHelper.HideItem, OnHidePanelExecute);
			FloatMenuItem.Enabled = !layoutItem.IsFloating;
			group.Items.Add(FloatMenuItem);
			if(layoutItem.ItemType == LayoutItemType.Panel) {
				group.Items.Add(AutoHideMenuGroup);
			}
			CreateResetLayoutMenuItem(group);
			if(layoutItem.ItemType == LayoutItemType.Document) {
				RemoveItemMenuItem.DisplayName = DockingLocalizer.GetString(DockingStringId.MenuItemRemoveDocument);
				HidePanelMenuItem.DisplayName = DockingLocalizer.GetString(DockingStringId.MenuItemHideDocument);
			}
			Items.Clear();
			Items.Add(group);
			Items.Add(DefaultMenuGroup);
		}
		void OnAddPanelExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			LayoutPanelDock dock = (LayoutPanelDock)GetEnumMenuGroupValue(AddPanelMenuGroup, (MenuAction)sender);
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.AddItem(modelItem, new DockTypeValue() { DockType = dock.ToDockType(), IsCenter = true }, ItemType);
		}
		void OnAddDocumentExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			DockType dock = DockType.Fill;
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.AddItem(modelItem, new DockTypeValue() { DockType = dock, IsCenter = true }, ItemType);
		}
		void OnHidePanelExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			ModelItem modelItem = e.Selection.PrimarySelection;
			service.HidePanel(modelItem);
		}
		MenuAction HidePanelMenuItem { get; set; }
		MenuAction AddDocumentMenuAction { get; set; }
		MenuGroup AddPanelMenuGroup { get; set; }
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class LayoutControlItemContextMenuProvider : BaseDockLayoutManagerContextMenuProvider {
		public LayoutControlItemContextMenuProvider() {
			var group = new MenuGroup("LayoutControlItemMenuItems") { HasDropDown = false };
			CaptionHorizontalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemCaptionHorizontalAlignment), typeof(HorizontalAlignment), OnCaptionHorizontalAlignmentExecute);
			CaptionVerticalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemCaptionVerticalAlignment), typeof(VerticalAlignment), OnCaptionVerticalAlignmentExecute);
			ControlHorizontalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemControlHorizontalAlignment), typeof(HorizontalAlignment), OnControlHorizontalAlignmentExecute);
			ControlVerticalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemControlVerticalAlignment), typeof(VerticalAlignment), OnControlVerticalAlignmentExecute);
			CaptionLocationGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemDTCaptionLocation), typeof(CaptionLocation), OnCaptionLocationExecute);
			GroupMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemGroupItems), false,
				ImageUriHelper.Group, OnGroupExecute);
			CreateRemoveItemMenuItem(group);
			CreateResetLayoutMenuItem(group);
			Items.Add(group);
			Items.Add(DefaultMenuGroup);
		}
		protected override bool IsActive(Microsoft.Windows.Design.Interaction.Selection selection) {
			return (selection.PrimarySelection != null) && selection.PrimarySelection.Is<LayoutControlItem>();
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			Item = e.Selection.PrimarySelection;
			SetEnumMenuGroupValue(CaptionHorizontalAlignmentGroup, GetProperty<HorizontalAlignment>(e.Selection.SelectedObjects, "CaptionHorizontalAlignment"));
			SetEnumMenuGroupValue(CaptionVerticalAlignmentGroup, GetProperty<VerticalAlignment>(e.Selection.SelectedObjects, "CaptionVerticalAlignment"));
			SetEnumMenuGroupValue(ControlHorizontalAlignmentGroup, GetProperty<HorizontalAlignment>(e.Selection.SelectedObjects, "ControlHorizontalAlignment"));
			SetEnumMenuGroupValue(ControlVerticalAlignmentGroup, GetProperty<VerticalAlignment>(e.Selection.SelectedObjects, "ControlVerticalAlignment"));
			SetEnumMenuGroupValue(CaptionLocationGroup, GetProperty<CaptionLocation>(e.Selection.SelectedObjects, "CaptionLocation"));
		}
		void OnCaptionHorizontalAlignmentExecute(object sender, MenuActionEventArgs e) {
			HorizontalAlignment alignment = (HorizontalAlignment)GetEnumMenuGroupValue(CaptionHorizontalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "CaptionHorizontalAlignment", alignment);
		}
		void OnCaptionVerticalAlignmentExecute(object sender, MenuActionEventArgs e) {
			VerticalAlignment alignment = (VerticalAlignment)GetEnumMenuGroupValue(CaptionVerticalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "CaptionVerticalAlignment", alignment);
		}
		void OnControlHorizontalAlignmentExecute(object sender, MenuActionEventArgs e) {
			HorizontalAlignment alignment = (HorizontalAlignment)GetEnumMenuGroupValue(ControlHorizontalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "ControlHorizontalAlignment", alignment);
		}
		void OnControlVerticalAlignmentExecute(object sender, MenuActionEventArgs e) {
			VerticalAlignment alignment = (VerticalAlignment)GetEnumMenuGroupValue(ControlVerticalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "ControlVerticalAlignment", alignment);
		}
		void OnCaptionLocationExecute(object sender, MenuActionEventArgs e) {
			CaptionLocation location = (CaptionLocation)GetEnumMenuGroupValue(CaptionLocationGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "CaptionLocation", location);
		}
		MenuGroup CaptionLocationGroup { get; set; }
		MenuAction GroupMenuItem { get; set; }
		MenuGroup CaptionHorizontalAlignmentGroup { get; set; }
		MenuGroup CaptionVerticalAlignmentGroup { get; set; }
		MenuGroup ControlHorizontalAlignmentGroup { get; set; }
		MenuGroup ControlVerticalAlignmentGroup { get; set; }
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class LayoutGroupContextMenuProvider : BaseDockLayoutManagerContextMenuProvider {
		protected override bool IsActive(Microsoft.Windows.Design.Interaction.Selection selection) {
			return (selection.PrimarySelection != null) && selection.PrimarySelection.Is<LayoutGroup>();
		}
		protected LayoutGroup Group { 
			get { return Item.As<LayoutGroup>(); } 
		}
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			Item = e.Selection.PrimarySelection;
			var group = new MenuGroup("LayoutGroupItemMenuItems") { HasDropDown = false };
			if(Group.ItemType == LayoutItemType.Group) {
				if(Group.IsLayoutItem() && !Item.IsLayoutRoot()) {
					StyleGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemDTGroupStyle), typeof(GroupBorderStyle), OnGroupStyleExecute);
					SetEnumMenuGroupValue(StyleGroup, GetProperty<GroupBorderStyle>(e.Selection.SelectedObjects, "GroupBorderStyle"));
				}
				else {
					if(Group.Items.Count == 0) {
						AddPanelMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemAddPanel), false,
							ImageUriHelper.AddDocument, OnAddPanelExecute);
						AddDocumentMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemAddDocument), false,
							ImageUriHelper.AddDocument, OnAddDocumentExecute);
					}
				}
				if(Group.Items.Count > 0) {
					OrientationGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemDTGroupOrientation), typeof(Orientation), OnGroupOrientationExecute);
					SetEnumMenuGroupValue(OrientationGroup, GetProperty<Orientation>(e.Selection.SelectedObjects, "Orientation"));
				}
			}
			if(Group.ItemType == LayoutItemType.DocumentPanelGroup) {
				MDIStyleGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemMDIStyle), typeof(MDIStyle), OnMDIStyleExecute);
				SetEnumMenuGroupValue(MDIStyleGroup, GetProperty<MDIStyle>(e.Selection.SelectedObjects, "MDIStyle"));
				AddDocumentMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemAddDocument), false, 
					ImageUriHelper.AddDocument, OnAddDocumentExecute);
				RemoveAllMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemRemoveAll), false,
					ImageUriHelper.RemoveAll, OnRemoveAllExecute);
			}
			if(Group.ItemType == LayoutItemType.TabPanelGroup) {
				group.Items.Add(AutoHideMenuGroup);
				FloatMenuItem.Enabled = !Group.IsFloating;
				group.Items.Add(FloatMenuItem);
			}
			if(!Item.IsLayoutRoot())
				CreateRemoveItemMenuItem(group);
			CreateResetLayoutMenuItem(group);
			Items.Clear();
			Items.Add(group);
			Items.Add(DefaultMenuGroup);
		}
		void OnMDIStyleExecute(object sender, MenuActionEventArgs e) {
			MDIStyle mdiStyle = (MDIStyle)GetEnumMenuGroupValue(MDIStyleGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "MDIStyle", mdiStyle);
		}
		void OnGroupOrientationExecute(object sender, MenuActionEventArgs e) {
			Orientation orientation = (Orientation)GetEnumMenuGroupValue(OrientationGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "Orientation", orientation);
		}
		void OnGroupStyleExecute(object sender, MenuActionEventArgs e) {
			GroupBorderStyle style = (GroupBorderStyle)GetEnumMenuGroupValue(StyleGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "GroupBorderStyle", style);
		}
		void OnAddPanelExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			service.AddItem(Item, new DockTypeValue() { DockType = DockType.Fill, IsCenter = true }, LayoutTypes.LayoutPanel);
		}
		void OnAddDocumentExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			service.AddItem(Item, new DockTypeValue() { DockType = DockType.Fill, IsCenter = true }, LayoutTypes.DocumentPanel);
		}
		void OnRemoveAllExecute(object sender, MenuActionEventArgs e) {
			DockLayoutManagerDesignService service = e.Context.Services.GetService<DockLayoutManagerDesignService>();
			service.RemoveAll(Item);
		}
		MenuGroup OrientationGroup { get; set; }
		MenuGroup StyleGroup { get; set; }
		MenuAction AddDocumentMenuItem { get; set; }
		MenuAction AddPanelMenuItem { get; set; }
		MenuAction RemoveAllMenuItem { get; set; }
		MenuGroup MDIStyleGroup { get; set; }
	}
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class FixedItemContextMenuProvider : BaseDockLayoutManagerContextMenuProvider {
		public FixedItemContextMenuProvider() {
		}
		protected override bool IsActive(Microsoft.Windows.Design.Interaction.Selection selection) {
			return (selection.PrimarySelection != null) && selection.PrimarySelection.Is<FixedItem>();
		}
		FixedItem FixedItem { get { return Item.As<FixedItem>(); } }
		protected override void OnUpdateItemStatus(MenuActionEventArgs e) {
			Item = e.Selection.PrimarySelection;
			var group = new MenuGroup("FixedItemMenuItems") { HasDropDown = false };
			if(FixedItem.ItemType == LayoutItemType.Label) {
				ContentHorizontalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemContentHorizontalAlignment), typeof(HorizontalAlignment), OnContentHorizontalAlignmentExecute);
				ContentVerticalAlignmentGroup = CreateEnumMenuGroup(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemContentVerticalAlignment), typeof(VerticalAlignment), OnContentVerticalAlignmentExecute);
				SetEnumMenuGroupValue(ContentHorizontalAlignmentGroup, GetProperty<HorizontalAlignment>(e.Selection.SelectedObjects, "CaptionHorizontalAlignment"));
				SetEnumMenuGroupValue(ContentVerticalAlignmentGroup, GetProperty<VerticalAlignment>(e.Selection.SelectedObjects, "CaptionVerticalAlignment"));
			}
			GroupMenuItem = CreateMenuItem(group.Items, DockingLocalizer.GetString(DockingStringId.MenuItemGroupItems), false,
				ImageUriHelper.Group, OnGroupExecute);
			CreateRemoveItemMenuItem(group);
			CreateResetLayoutMenuItem(group);
			Items.Clear();
			Items.Add(group);
			Items.Add(DefaultMenuGroup);
		}
		void OnContentHorizontalAlignmentExecute(object sender, MenuActionEventArgs e) {
			HorizontalAlignment alignment = (HorizontalAlignment)GetEnumMenuGroupValue(ContentHorizontalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "ContentHorizontalAlignment", alignment);
		}
		void OnContentVerticalAlignmentExecute(object sender, MenuActionEventArgs e) {
			VerticalAlignment alignment = (VerticalAlignment)GetEnumMenuGroupValue(ContentVerticalAlignmentGroup, (MenuAction)sender);
			SetProperty(e.Selection.SelectedObjects, "ContentVerticalAlignment", alignment);
		}
		MenuGroup ContentHorizontalAlignmentGroup { get; set; }
		MenuGroup ContentVerticalAlignmentGroup { get; set; }
		MenuAction GroupMenuItem { get; set; }
	}
}
