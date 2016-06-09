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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Customization;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public abstract class BaseLayoutElementMenu : PopupMenu {
		public static readonly object RuntimeCreatedMenuItem = new object();
		#region static
		static BaseLayoutElementMenu() {
			EventManager.RegisterClassHandler(typeof(FrameworkElement),
					Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
			EventManager.RegisterClassHandler(typeof(BaseLayoutElementMenu),
					FrameworkElement.ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpening));
		}
		static void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			BaseLayoutElementMenu menu = sender as BaseLayoutElementMenu;
			if(menu != null)
				e.Handled = menu.IsOpen;
		}
		static void OnClickThroughThunk(object sender, MouseButtonEventArgs e) {
			if(sender is BaseLayoutElementMenu && DockLayoutManagerHelper.IsPopupRoot(e.OriginalSource)) {
				GeneratePreviewouseDownEvent(((BaseLayoutElementMenu)sender).Container, e);
			}
		}
		static void GeneratePreviewouseDownEvent(DockLayoutManager manager, MouseButtonEventArgs e) {
			if(manager != null) manager.RaiseEvent(
				  new MouseButtonEventArgs(
						  e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice) { RoutedEvent = PreviewMouseDownEvent }
					  );
		}
		#endregion
		public BaseLayoutElementMenu(DockLayoutManager container) {
			Container = container;
			MenuController = new BarManagerMenuController(this);
			Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
		}		
		public DockLayoutManager Container { get; private set; }
		public BaseLayoutElementMenuInfo MenuInfo { get; private set; }
		protected BarManagerMenuController MenuController { get; private set; }
		public BarManagerActionCollection Customizations { get { return MenuController.ActionContainer.Actions; } }
		public void Close() {
			ClosePopup();
		}
		protected abstract BaseLayoutElementMenuInfo CreateMenuInfo();
		protected void CreateItems() {
			if(MenuInfo != null)
				MenuInfo.Uninitialize();
			MenuInfo = CreateMenuInfo();
			if(MenuInfo != null) {
				MenuInfo.Initialize(PlacementTarget);
				MenuInfo.MenuController.Execute();
			}
		}
		public void ClearItems() {
			var items = Items.ToList();
			Items.Clear();			
			foreach(IBarItem it in items) {
				var item = it as BarItem;
				if (item == null)
					continue;
				if(item.Tag == RuntimeCreatedMenuItem) {
					if(item is BarSubItem)
						((BarSubItem)item).Items.Clear();
				}
			}
		}
		public void RemoveItem(BarItem item) {
			Items.Remove(item);
			BarDragProvider.RemoveUnnesessarySeparators(ItemLinks);
		}
		public void MoveItem(BarItem item, int index) {
			List<BarItemLinkBase> itemLinks = new List<BarItemLinkBase>(ItemLinks);
			for(int i = 0; i < itemLinks.Count; i++) {
				BarItemLink link = itemLinks[i] as BarItemLink;
				if(link == null) continue;
				if(link.Item == item) {
					ItemLinks.Move(i, index);
				}
			}
			BarDragProvider.RemoveUnnesessarySeparators(ItemLinks);
		}
		public ReadOnlyCollection<BarItem> GetItems() {
			List<BarItem> list = new List<BarItem>();
			foreach(BarItemLinkBase link in ItemLinks) {
				if(link is BarItemLink && ((BarItemLink)link).Item != null)
					list.Add(((BarItemLink)link).Item);
				if(link is BarSubItemLink) {
					foreach(BarItemLink barItemLink in ((BarSubItemLink)link).ItemLinks) {
						list.Add(barItemLink.Item);
					}
				}
			}
			return new ReadOnlyCollection<BarItem>(list);
		}
		public void AddItem(BarItem barItem, bool isBeginGroup) {
			barItem.Tag = RuntimeCreatedMenuItem;
			if(isBeginGroup) ItemLinks.Add(new BarItemLinkSeparator());
			ItemLinks.Add(barItem.CreateLink());
		}
		public BarButtonItem CreateBarButtonItem(object content, ImageSource glyph, bool beginGroup) {
			BarButtonItem barItem = CreateBarButtonItemCore(content, glyph);
			AddItem(barItem, beginGroup);
			return barItem;
		}
		public BarButtonItem CreateBarButtonItem(MenuItems menuItem, ImageSource glyph, bool beginGroup) {
			BarButtonItem barItem = CreateBarButtonItemCore(menuItem, glyph);
			AddItem(barItem, beginGroup);
			return barItem;
		}
		public BarSubItem CreateBarSubItem(object content, ImageSource glyph, bool beginGroup) {
			BarSubItem barSubItem = CreateBarSubItemCore(content, glyph);
			AddItem(barSubItem, beginGroup);
			return barSubItem;
		}
		public BarSubItem CreateBarSubItem(MenuItems menuItem, ImageSource glyph, bool beginGroup) {
			BarSubItem barSubItem = CreateBarSubItemCore(menuItem, glyph);
			AddItem(barSubItem, beginGroup);
			return barSubItem;
		}
		public BarCheckItem CreateBarCheckItem(object content, bool? isChecked, bool beginGroup) {
			BarCheckItem barItem = CreateBarCheckItemCore(content, isChecked);
			AddItem(barItem, beginGroup);
			return barItem;
		}
		public BarCheckItem CreateBarCheckItem(MenuItems menuItem, bool? isChecked, bool beginGroup) {
			BarCheckItem barItem = CreateBarCheckItemCore(menuItem, isChecked);
			AddItem(barItem, beginGroup);
			return barItem;
		}
		protected internal BarButtonItem CreateBarButtonItemCore(object content, ImageSource glyph) {
			return new BarButtonItem() { IsPrivate = true, Name = CustomizationController.GetUniqueMenuItemName(), Content = content, Glyph = glyph };
		}
		protected internal BarButtonItem CreateBarButtonItemCore(MenuItems menuItem, ImageSource glyph) {
			return new BarButtonItem() { IsPrivate = true, Name = MenuItemHelper.GetUniqueName(menuItem), Content = MenuItemHelper.GetContent(menuItem), Glyph = glyph };
		}
		protected internal BarCheckItem CreateBarCheckItemCore(object content, bool? isChecked) {
			return new BarCheckItem() { IsPrivate = true, Name = CustomizationController.GetUniqueMenuItemName(), Content = content, IsChecked = isChecked };
		}
		protected internal BarCheckItem CreateBarCheckItemCore(MenuItems menuItem, bool? isChecked) {
			return new BarCheckItem() { IsPrivate = true, Name = MenuItemHelper.GetUniqueName(menuItem), Content = MenuItemHelper.GetContent(menuItem), IsChecked = isChecked };
		}
		protected virtual BarSubItem CreateBarSubItemCore(object content, ImageSource glyph) {
			return new BarSubItem() { IsPrivate = true, Name = CustomizationController.GetUniqueMenuItemName(), Content = content, Glyph = glyph };
		}
		protected virtual BarSubItem CreateBarSubItemCore(MenuItems menuItem, ImageSource glyph) {
			return new BarSubItem() { IsPrivate = true, Name = MenuItemHelper.GetUniqueName(menuItem), Content = MenuItemHelper.GetContent(menuItem), Glyph = glyph };
		}
		protected override bool RaiseOpening() {
			bool result = base.RaiseOpening();
			if(result) {
				UpdatePopupPosition();
				Customizations.Clear();
				ClearItems();
				CreateItems();
				ShowingMenuEventArgs ea = new ShowingMenuEventArgs(this) { Source = Container };
				Container.RaiseEvent(ea);
				if(ea.Show) MenuController.Execute();
				else result = false;
			}
			return result;
		}
		void UpdatePopupPosition() {
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			ClearItems();
			Customizations.Clear();
			if(MenuInfo != null)
				MenuInfo.Uninitialize();
			PlacementTarget = null;
		}
		public override void ShowPopup(UIElement control) {
			if(control != null) base.ShowPopup(control);
		}
	}
	public abstract class BaseLayoutElementMenuInfo {
		public BaseLayoutElementMenuInfo(BaseLayoutElementMenu menu) {
			Menu = menu;
		}
		public BaseLayoutElementMenu Menu { get; private set; }
		public UIElement TargetElement { get; set; }
		public void Initialize(UIElement value) {
			TargetElement = value;
			CreateItems();
		}
		public virtual void Uninitialize() {
			TargetElement = null;
		}
		protected abstract void CreateItems();
		protected virtual BarCheckItem CreateBarCheckItem(object content, bool? isChecked, bool beginGroup, ICommand command) {
			BarCheckItem item = Menu.CreateBarCheckItem(content, isChecked, beginGroup);
			CustomizationControllerHelper.AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarCheckItem CreateBarCheckItem(MenuItems menuItem, bool? isChecked, bool beginGroup, ICommand command) {
			BarCheckItem item = Menu.CreateBarCheckItem(menuItem, isChecked, beginGroup);
			CustomizationControllerHelper.AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarButtonItem CreateBarButtonItem(object content, ImageSource glyph, bool beginGroup, ICommand command) {
			BarButtonItem item = Menu.CreateBarButtonItem(content, glyph, beginGroup);
			CustomizationControllerHelper.AssignCommand(item, command, TargetElement);
			return item;
		}
		protected virtual BarButtonItem CreateBarButtonItem(MenuItems menuItem, bool beginGroup, ICommand command) {
			BarButtonItem item = Menu.CreateBarButtonItem(menuItem, MenuItemHelper.GetGlyph(menuItem), beginGroup);
			CustomizationControllerHelper.AssignCommand(item, command, TargetElement);
			return item;
		}
		public abstract BarManagerMenuController MenuController { get; }
		protected BarCheckItem CreateBarCheckSubItem(BarSubItem barSubItem, MenuItems menuItem, bool? isChecked, LayoutControllerCommand command) {
			BarCheckItem item = Menu.CreateBarCheckItemCore(menuItem, isChecked);
			item.Tag = BaseLayoutElementMenu.RuntimeCreatedMenuItem;
			CustomizationControllerHelper.AssignCommand(item, command, TargetElement);
			barSubItem.ItemLinks.Add(item);
			return item;
		}
		protected void CreateStandardCustomizationMenuItems(bool beginGroup) {
			CreateBarButtonItem(MenuItems.EndCustomization, beginGroup, CustomizationControllerHelper.CreateCommand<EndCustomizationCommand>(Menu.Container));
			if(Menu.Container.CustomizationController.IsCustomizationFormVisible) {
				CreateBarButtonItem(MenuItems.HideCustomizationWindow, false, CustomizationControllerHelper.CreateCommand<HideCustomizationFormCommand>(Menu.Container));
			}
			else {
				CreateBarButtonItem(MenuItems.ShowCustomizationWindow, false, CustomizationControllerHelper.CreateCommand<ShowCustomizationFormCommand>(Menu.Container));
			}
		}
		protected void CreateBeginCustomizationMenuItem(bool beginGroup) {
			if(Menu.Container.AllowCustomization)
				CreateBarButtonItem(MenuItems.BeginCustomization, beginGroup, CustomizationControllerHelper.CreateCommand<BeginCustomizationCommand>((Menu.Container)));
		}
	}
	public abstract class ItemContextMenuBase : BaseLayoutElementMenu {
		public BaseLayoutItem Item { get; private set; }
		public ItemContextMenuBase(DockLayoutManager container)
			: base(container) {
		}
		public virtual void Show(BaseLayoutItem item, UIElement placementTarget = null) {
			if(item == null) return;
			Item = item;
			if(placementTarget == null) placementTarget = item;
			ShowPopup(placementTarget);
		}
		protected override void OnClosed(EventArgs e) {
			Item = null;
			base.OnClosed(e);
		}
	}
	public class ItemContextMenu : ItemContextMenuBase {
		public ItemContextMenu(DockLayoutManager container)
			: base(container) {
		}
		public ItemContextMenuInfo ItemMenuInfo {
			get { return MenuInfo as ItemContextMenuInfo; }
		}
		protected override BaseLayoutElementMenuInfo CreateMenuInfo() {
			return new ItemContextMenuInfo(this, Item);
		}
	}
	public class ItemContextMenuInfo : BaseLayoutElementMenuInfo {
		public ItemContextMenuInfo(ItemContextMenu menu, BaseLayoutItem item)
			: base(menu) {
			Item = item;
		}
		public BaseLayoutItem Item { get; private set; }
		public ItemContextMenu ItemMenu { get { return Menu as ItemContextMenu; } }
		protected override void CreateItems() {
			DockLayoutManager Container = ItemMenu.Container;
			bool floating = Item.IsFloating;
			bool autoHidden = Item.IsAutoHidden;
			if(!LayoutItemsHelper.IsDataBound(Item)) {
				ClosingBehavior closingBehavior = DockControllerHelper.GetActualClosingBehavior(Container, Item);
				CreateBarCheckItem(MenuItems.Dockable, !floating && !autoHidden, false, DockControllerHelper.CreateCommand<DockCommand>(Container, Item));
				CreateBarCheckItem(MenuItems.Floating, floating, false, DockControllerHelper.CreateCommand<FloatCommand>(Container, Item));
				CreateBarCheckItem(MenuItems.AutoHide, autoHidden, false, DockControllerHelper.CreateCommand<HideCommand>(Container, Item));
				CreateBarButtonItem(closingBehavior == ClosingBehavior.ImmediatelyRemove ? MenuItems.Close : MenuItems.Hide, false, DockControllerHelper.CreateCommand<CloseCommand>(Container, Item));
				int count = 0;
				LayoutGroup parent = Item.Parent;
				if(parent != null && parent.ItemType == LayoutItemType.DocumentPanelGroup) {
					CreateBarButtonItem(MenuItems.CloseAllButThis, false, DockControllerHelper.CreateCommand<CloseAllButThisCommand>(Container, Item));
					bool prevGroup = DockControllerHelper.GetPreviousNotEmptyDocumentGroup(parent) != null;
					bool nextGroup = DockControllerHelper.GetNextNotEmptyDocumentGroup(parent) != null;
					if(parent.Items.Count > 1) {
						bool fHorz = true;
						bool fVert = true;
						if(prevGroup || nextGroup) {
							fHorz = parent.Parent != null && parent.Parent.Orientation == Orientation.Horizontal;
							fVert = !fHorz;
						}
						if(fVert) CreateBarButtonItem(MenuItems.NewHorizontalTabbedGroup, count++ == 0, DockControllerHelper.CreateCommand<NewVerticalDocumentGroupCommand>(Container, Item));
						if(fHorz) CreateBarButtonItem(MenuItems.NewVerticalTabbedGroup, count++ == 0, DockControllerHelper.CreateCommand<NewHorizontalDocumentGroupCommand>(Container, Item));
					}
					if(prevGroup)
						CreateBarButtonItem(MenuItems.MoveToPreviousTabGroup, count++ == 0, DockControllerHelper.CreateCommand<MoveToPreviousDocumentGroupCommand>(Container, Item));
					if(nextGroup)
						CreateBarButtonItem(MenuItems.MoveToNextTabGroup, count++ == 0, DockControllerHelper.CreateCommand<MoveToNextDocumentGroupCommand>(Container, Item));
				}
			}
			if(Item is LayoutPanel) {
				LayoutGroup root = ((LayoutPanel)Item).Content as LayoutGroup;
				if(root != null) {
					if(Container.IsCustomization) {
						CreateStandardCustomizationMenuItems(true);
						if(Container.RenameHelper.CanRename(Item))
							CreateBarButtonItem(MenuItems.Rename, false, LayoutControllerHelper.CreateCommand<RenameCommand>(Menu.Container, new BaseLayoutItem[] { Item }));
					}
					else CreateBeginCustomizationMenuItem(true);
				}
			}
			if(Menu.Container.CustomizationController.ClosedPanelsBarVisibility != ClosedPanelsBarVisibility.Never) {
				if(Menu.Container.ClosedPanels.Count > 0 || Menu.Container.CustomizationController.IsClosedPanelsVisible) {
					CreateBarCheckItem(MenuItems.ClosedPanels, Menu.Container.CustomizationController.IsClosedPanelsVisible,
							true, CustomizationControllerHelper.CreateCommand(ItemMenu.Container)
						);
				}
			}
		}
		public override BarManagerMenuController MenuController {
			get { return Menu.Container.CustomizationController.ItemContextMenuController; }
		}
		public override void Uninitialize() {
			base.Uninitialize();
			Item = null;
		}
	}
	public class ItemsSelectorMenu : BaseLayoutElementMenu {
		public ItemsSelectorMenu(DockLayoutManager container)
			: base(container) {
		}
		public new BaseLayoutItem[] Items { get; private set; }
		public ItemsSelectorMenuInfo ItemsMenuInfo {
			get { return MenuInfo as ItemsSelectorMenuInfo; }
		}
		public virtual void Show(UIElement source, BaseLayoutItem[] items) {
			Items = items;
			ShowPopup(source);
		}
		protected override BaseLayoutElementMenuInfo CreateMenuInfo() {
			return new ItemsSelectorMenuInfo(this, Items);
		}
		protected override void OnClosed(EventArgs e) {
			Items = null;
			base.OnClosed(e);
		}
	}
	public class ItemsSelectorMenuInfo : BaseLayoutElementMenuInfo {
		public ItemsSelectorMenuInfo(ItemsSelectorMenu menu, BaseLayoutItem[] items)
			: base(menu) {
			Items = items;
		}
		public BaseLayoutItem[] Items { get; private set; }
		public ItemsSelectorMenu ItemsMenu { get { return Menu as ItemsSelectorMenu; } }
		protected override void CreateItems() {
			LayoutGroup group = DockLayoutManager.GetLayoutItem((UIElement)TargetElement) as LayoutGroup;
			bool isInLayout = false;
			bool hasItems = (group == null) || ((group.ItemType != LayoutItemType.Group) && group.IsExpanded && (Items.Length > 0));
			int actionCounter = 0;
			if(group != null) {
				bool isRoot = (group.Parent == null);
				isInLayout = group.GetRoot().IsLayoutRoot;
				if(isInLayout)
					if(Menu.Container.IsCustomization) {
						CreateStandardCustomizationMenuItems(false);
						if(!isRoot) {
							BaseLayoutItem[] items = Menu.Container.LayoutController.Selection.ToArray();
							bool? showCaption = LayoutControllerHelper.GetSameValue(LayoutControlItem.ShowCaptionProperty, items, LayoutControllerHelper.BoolComparer) as bool?;
							bool? hasCaption = LayoutControllerHelper.GetSameValue(BaseLayoutItem.HasCaptionProperty, items, LayoutControllerHelper.BoolComparer) as bool?;
							if(showCaption != null && hasCaption.HasValue && hasCaption.Value)
								CreateBarCheckItem(MenuItems.ShowCaption, (bool?)showCaption, actionCounter++ == 0, LayoutControllerHelper.CreateCommand<ShowCaptionCommand>(Menu.Container, items));
							if(group.GroupBorderStyle == GroupBorderStyle.Tabbed) {
								BarSubItem barSubItem = Menu.CreateBarSubItem(MenuItems.CaptionLocation, null, actionCounter++ == 0);
								CaptionLocation cl = (CaptionLocation)group.CaptionLocation;
								CreateBarCheckSubItem(barSubItem, MenuItems.Left, cl == CaptionLocation.Left, LayoutControllerHelper.CreateCommand<CaptionLocationLeftCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.Right, cl == CaptionLocation.Right, LayoutControllerHelper.CreateCommand<CaptionLocationRightCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.Top, cl == CaptionLocation.Top || cl == CaptionLocation.Default, LayoutControllerHelper.CreateCommand<CaptionLocationTopCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.Bottom, cl == CaptionLocation.Bottom, LayoutControllerHelper.CreateCommand<CaptionLocationBottomCommand>(Menu.Container, items));
							}
							object value = LayoutControllerHelper.GetSameValue(LayoutGroup.OrientationProperty, items, LayoutControllerHelper.CompareOrientation);
							bool hasOnlyGroups = LayoutControllerHelper.HasOnlyGroups(items);
							if(hasOnlyGroups && value != null) {
								BarSubItem barSubItem = Menu.CreateBarSubItem(MenuItems.GroupOrientation, null, actionCounter++ == 0);
								Orientation orientation = (Orientation)value;
								CreateBarCheckSubItem(barSubItem, MenuItems.Horizontal, orientation == Orientation.Horizontal, LayoutControllerHelper.CreateCommand<GroupOrientationHorizontalCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.Vertical, orientation == Orientation.Vertical, LayoutControllerHelper.CreateCommand<GroupOrientationVerticalCommand>(Menu.Container, items));
							}
							value = LayoutControllerHelper.GetSameValue(LayoutGroup.GroupBorderStyleProperty, items, LayoutControllerHelper.CompareGroupBorderStyle);
							if(hasOnlyGroups && value != null) {
								BarSubItem barSubItem = Menu.CreateBarSubItem(MenuItems.Style, null, actionCounter++ == 0);
								GroupBorderStyle style = (GroupBorderStyle)value;
								CreateBarCheckSubItem(barSubItem, MenuItems.StyleNoBorder, style == GroupBorderStyle.NoBorder, LayoutControllerHelper.CreateCommand<SetStyleNoBorderCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.StyleGroup, style == GroupBorderStyle.Group, LayoutControllerHelper.CreateCommand<SetStyleGroupCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.StyleGroupBox, style == GroupBorderStyle.GroupBox, LayoutControllerHelper.CreateCommand<SetStyleGroupBoxCommand>(Menu.Container, items));
								CreateBarCheckSubItem(barSubItem, MenuItems.StyleTabbed, style == GroupBorderStyle.Tabbed, LayoutControllerHelper.CreateCommand<SetStyleTabbedCommand>(Menu.Container, items));
							}
							CreateBarButtonItem(MenuItems.HideItem, true,
								LayoutControllerHelper.CreateCommand<HideItemCommand>(Menu.Container, items));
							if(LayoutItemsHelper.AreInSameGroup(items))
								CreateBarButtonItem(MenuItems.GroupItems, false, LayoutControllerHelper.CreateCommand<GroupCommand>(Menu.Container, items));
							if(items.Length == 1)
								CreateBarButtonItem(MenuItems.Ungroup, false, LayoutControllerHelper.CreateCommand<UngroupCommand>(Menu.Container, items));
							if(Menu.Container.RenameHelper.CanRename(group))
								CreateBarButtonItem(MenuItems.Rename, false, LayoutControllerHelper.CreateCommand<RenameCommand>(Menu.Container, new BaseLayoutItem[] { group }));
						}
					}
					else {
						CreateBeginCustomizationMenuItem(false);
					}
				if(!isRoot && group.ItemType == LayoutItemType.Group && group.GroupBorderStyle == GroupBorderStyle.GroupBox) {
					CreateBarCheckItem(MenuItems.ExpandGroup, group.IsExpanded, false,
						DockControllerHelper.CreateCommand<ExpandCommand>(ItemsMenu.Container, group));
				}
				if(hasItems && group.ItemType != LayoutItemType.FloatGroup)
					for(int i = 0; i < Items.Length; i++) {
						CreateBarCheckItem(Items[i].CustomizationCaption, group.SelectedItem == Items[i], ItemsMenu.ItemLinks.Count > 0 && i == 0,
								DockControllerHelper.CreateCommand<ActivateCommand>(ItemsMenu.Container, Items[i])
							);
					}
			}
			if(Menu.Container.CustomizationController.ClosedPanelsBarVisibility != ClosedPanelsBarVisibility.Never && !isInLayout) {
				if(Menu.Container.ClosedPanels.Count > 0 || Menu.Container.CustomizationController.IsClosedPanelsVisible) {
					CreateBarCheckItem(MenuItems.ClosedPanels, Menu.Container.CustomizationController.IsClosedPanelsVisible,
							Menu.ItemLinks.Count > 0, CustomizationControllerHelper.CreateCommand(ItemsMenu.Container)
						);
				}
			}
		}
		public override BarManagerMenuController MenuController {
			get { return Menu.Container.CustomizationController.ItemsSelectorMenuController; }
		}
		public override void Uninitialize() {
			base.Uninitialize();
			Items = null;
		}
	}
	public class LayoutControlItemCustomizationMenu : BaseLayoutElementMenu {
		public LayoutControlItemCustomizationMenu(DockLayoutManager container) :
			base(container) {
		}
		public new BaseLayoutItem[] Items { get; private set; }
		public LayoutControlItemCustomizationMenuInfo LayoutControlItemCustomizationMenuInfo {
			get { return MenuInfo as LayoutControlItemCustomizationMenuInfo; }
		}
		public virtual void Show(UIElement source, BaseLayoutItem[] Items) {
			this.Items = Items;
			ShowPopup(source);
		}
		protected override BaseLayoutElementMenuInfo CreateMenuInfo() {
			return new LayoutControlItemCustomizationMenuInfo(this, Items);
		}
		protected override void OnClosed(EventArgs e) {
			Items = null;
			base.OnClosed(e);
		}
	}
	public class LayoutControlItemCustomizationMenuInfo : BaseLayoutElementMenuInfo {
		public LayoutControlItemCustomizationMenuInfo(LayoutControlItemCustomizationMenu menu, BaseLayoutItem[] items) :
			base(menu) {
			Items = items;
		}
		public BaseLayoutItem[] Items { set; private get; }
		public LayoutControlItemCustomizationMenu LayoutControlItemMenu { get { return Menu as LayoutControlItemCustomizationMenu; } }
		protected override void CreateItems() {
			CreateStandardCustomizationMenuItems(false);
			int actionCouter = 0;
			bool? showCaption = LayoutControllerHelper.GetSameValue(LayoutControlItem.ShowCaptionProperty, Items, LayoutControllerHelper.BoolComparer) as bool?;
			bool? hasCaption = LayoutControllerHelper.GetSameValue(BaseLayoutItem.HasCaptionProperty, Items, LayoutControllerHelper.BoolComparer) as bool?;
			object captionLocation = LayoutControllerHelper.GetSameValue(LayoutControlItem.CaptionLocationProperty, Items, LayoutControllerHelper.CompareCaptionLocation);
			bool? showControl = LayoutControllerHelper.GetSameValue(LayoutControlItem.ShowControlProperty, Items, LayoutControllerHelper.BoolComparer) as bool?;
			bool? hasControl = LayoutControllerHelper.GetSameValue(LayoutControlItem.HasControlProperty, Items, LayoutControllerHelper.BoolComparer) as bool?;
			bool? hasImage = (bool?)LayoutControllerHelper.GetSameValue(LayoutControlItem.HasImageProperty, Items, LayoutControllerHelper.BoolComparer);
			bool? showImage = (bool?)LayoutControllerHelper.GetSameValue(LayoutControlItem.ShowCaptionImageProperty, Items, LayoutControllerHelper.BoolComparer);
			object imageLocation = LayoutControllerHelper.GetSameValue(LayoutControlItem.CaptionImageLocationProperty, Items, LayoutControllerHelper.CompareImageLocation);
			if(showCaption != null && hasCaption.HasValue && hasCaption.Value)
				CreateBarCheckItem(MenuItems.ShowCaption, (bool?)showCaption, actionCouter++ == 0, LayoutControllerHelper.CreateCommand<ShowCaptionCommand>(LayoutControlItemMenu.Container, Items));
			if(showControl != null && hasControl.HasValue && hasControl.Value)
				CreateBarCheckItem(MenuItems.ShowControl, showControl, actionCouter++ == 0, LayoutControllerHelper.CreateCommand<ShowControlCommand>(LayoutControlItemMenu.Container, Items));
			if(showImage != null && hasImage.HasValue && hasImage.Value)
				CreateBarCheckItem(MenuItems.ShowCaptionImage, showImage, actionCouter++ == 0, LayoutControllerHelper.CreateCommand<ShowCaptionImageCommand>(LayoutControlItemMenu.Container, Items));
			if(captionLocation != null && hasCaption.HasValue && hasCaption.Value && ((bool?)showCaption).HasValue && ((bool?)showCaption).Value) {
				BarSubItem barSubItem = Menu.CreateBarSubItem(MenuItems.CaptionLocation, null, actionCouter++ == 0);
				CaptionLocation cl = (CaptionLocation)captionLocation;
				CreateBarCheckSubItem(barSubItem, MenuItems.Left, cl == CaptionLocation.Left || cl == CaptionLocation.Default, LayoutControllerHelper.CreateCommand<CaptionLocationLeftCommand>(Menu.Container, Items));
				CreateBarCheckSubItem(barSubItem, MenuItems.Right, cl == CaptionLocation.Right, LayoutControllerHelper.CreateCommand<CaptionLocationRightCommand>(Menu.Container, Items));
				CreateBarCheckSubItem(barSubItem, MenuItems.Top, cl == CaptionLocation.Top, LayoutControllerHelper.CreateCommand<CaptionLocationTopCommand>(Menu.Container, Items));
				CreateBarCheckSubItem(barSubItem, MenuItems.Bottom, cl == CaptionLocation.Bottom, LayoutControllerHelper.CreateCommand<CaptionLocationBottomCommand>(Menu.Container, Items));
			}
			if(imageLocation != null && hasImage.HasValue && hasImage.Value && showImage.HasValue && showImage.Value) {
				BarSubItem barSubItem = Menu.CreateBarSubItem(MenuItems.CaptionImageLocation, null, actionCouter++ == 0);
				ImageLocation il = (ImageLocation)imageLocation;
				CreateBarCheckSubItem(barSubItem, MenuItems.BeforeText, il != ImageLocation.AfterText, LayoutControllerHelper.CreateCommand<CaptionImageBeforeTextCommand>(Menu.Container, Items));
				CreateBarCheckSubItem(barSubItem, MenuItems.AfterText, il == ImageLocation.AfterText, LayoutControllerHelper.CreateCommand<CaptionImageAfterTextCommand>(Menu.Container, Items));
			}
			CreateBarButtonItem(MenuItems.HideItem, true, LayoutControllerHelper.CreateCommand<HideItemCommand>(Menu.Container, Items));
			if(LayoutItemsHelper.AreInSameGroup(Items))
				CreateBarButtonItem(MenuItems.GroupItems, false, LayoutControllerHelper.CreateCommand<GroupCommand>(Menu.Container, Items));
			if(Menu.Container.RenameHelper.CanRename(Menu.Container.ActiveLayoutItem))
				CreateBarButtonItem(MenuItems.Rename, false, LayoutControllerHelper.CreateCommand<RenameCommand>(Menu.Container, new BaseLayoutItem[] { Menu.Container.ActiveLayoutItem }));
		}
		public override BarManagerMenuController MenuController {
			get { return Menu.Container.CustomizationController.LayoutControlItemCustomizationMenuController; }
		}
	}
	public class LayoutControlItemContextMenu : ItemContextMenuBase {
		public LayoutControlItemContextMenu(DockLayoutManager container) :
			base(container) {
		}
		public LayoutControlItemContextMenuInfo LayoutControlItemContextMenuInfo {
			get { return MenuInfo as LayoutControlItemContextMenuInfo; }
		}
		protected override BaseLayoutElementMenuInfo CreateMenuInfo() {
			return new LayoutControlItemContextMenuInfo(this, Item);
		}
	}
	public class LayoutControlItemContextMenuInfo : BaseLayoutElementMenuInfo {
		public LayoutControlItemContextMenuInfo(LayoutControlItemContextMenu menu, BaseLayoutItem item) :
			base(menu) {
			Item = item;
		}
		public BaseLayoutItem Item { set; private get; }
		public LayoutControlItemContextMenu LayoutControlItemMenu { get { return Menu as LayoutControlItemContextMenu; } }
		protected override void CreateItems() {
			CreateBeginCustomizationMenuItem(false);
		}
		public override BarManagerMenuController MenuController {
			get { return Menu.Container.CustomizationController.LayoutControlItemContextMenuController; }
		}
	}
	public class HiddenItemContextMenu : ItemContextMenuBase {
		public HiddenItemContextMenu(DockLayoutManager container) :
			base(container) {
		}
		public HiddenItemContextMenuInfo HiddenItemContextMenuInfo {
			get { return MenuInfo as HiddenItemContextMenuInfo; }
		}
		protected override BaseLayoutElementMenuInfo CreateMenuInfo() {
			return new HiddenItemContextMenuInfo(this, Item);
		}
	}
	public class HiddenItemContextMenuInfo : BaseLayoutElementMenuInfo {
		public HiddenItemContextMenuInfo(HiddenItemContextMenu menu, BaseLayoutItem item) :
			base(menu) {
			Item = item;
		}
		public BaseLayoutItem Item { set; private get; }
		public HiddenItemContextMenu HiddenItemMenu { get { return Menu as HiddenItemContextMenu; } }
		protected override void CreateItems() {
			Menu.Container.CustomizationController.CustomizationRoot = Item.GetRoot();
			CreateBarButtonItem(MenuItems.RestoreItem, false, LayoutControllerHelper.CreateCommand<RestoreItemCommand>(Menu.Container, new BaseLayoutItem[] { Item }));
		}
		public override BarManagerMenuController MenuController {
			get { return Menu.Container.CustomizationController.HiddenItemsMenuController; }
		}
	}
}
