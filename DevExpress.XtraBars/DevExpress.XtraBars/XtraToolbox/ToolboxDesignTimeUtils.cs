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

using DevExpress.Utils.Design;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	public class ToolboxElementDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			ToolboxElementBase element = component as ToolboxElementBase;
			if(element == null)
				return Rectangle.Empty;
			return GetBoundsCore(element);
		}
		public Rectangle GetBoundsCore(ToolboxElementBase element) {
			ToolboxControl toolbox = element.Owner as ToolboxControl;
			ToolboxGroup group = element as ToolboxGroup;
			ToolboxItem item = element as ToolboxItem;
			if(group != null) {
				return toolbox.ViewInfo.GetGroupInfo(group).Bounds;
			}
			if(item != null) {
				return toolbox.ViewInfo.GetItemInfo(item).Bounds;
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			ToolboxElementBase element = (ToolboxElementBase)component;
			return element.Owner as Control;
		}
	}
	public class ToolboxDesignTimeManager : BaseDesignTimeManager {
		ContextMenu groupMenu, itemMenu;
		MenuItem groupDelete, itemDelete, groupAddItem;
		public ToolboxDesignTimeManager(ToolboxControl toolbox)
			: base(toolbox, null) {
			this.groupMenu = CreateGroupMenu();
			this.itemMenu = CreateItemMenu();
		}
		const int AddItemId = 4;
		public ToolboxViewInfo ViewInfo {
			get { return Toolbox.ViewInfo; }
		}
		protected virtual ContextMenu CreateGroupMenu() {
			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add(new MenuItem("&Select group", new EventHandler(OnSelectGroup)));
			menu.MenuItems.Add(new MenuItem("Add &group", new EventHandler(OnGroupMenuAddGroup)));
			menu.MenuItems.Add(groupDelete = new MenuItem("&Delete group", new EventHandler(OnGroupMenuDeleteGroup)));
			menu.MenuItems.Add(groupAddItem = new MenuItem("&Add item", new EventHandler(OnGroupMenuAddItem)));
			return menu;
		}
		protected virtual ContextMenu CreateItemMenu() {
			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add(new MenuItem("&Add item", new EventHandler(OnItemMenuAddItem)));
			menu.MenuItems.Add(itemDelete = new MenuItem("&Delete item", new EventHandler(OnItemMenuDeleteItem)));
			return menu;
		}
		public ToolboxControl Toolbox { get { return Owner as ToolboxControl; } }
		public override ISite Site { get { return Toolbox == null ? null : Toolbox.Site; } }
		public override void Dispose() {
			if(this.groupMenu != null)
				this.groupMenu.Dispose();
			if(this.itemMenu != null)
				this.itemMenu.Dispose();
			base.Dispose();
		}
		public override void InvalidateComponent(object component) {
			Toolbox.Invalidate(false);
		}
		protected override void OnDesignTimeSelectionChanged(object component) {
			ToolboxElementBase elem = component as ToolboxElementBase;
			if(elem != null && elem.Owner == Toolbox)
				InvalidateComponent(elem);
		}
		protected internal virtual void OnMouseDown(MouseEventArgs e, ToolboxHitInfo hitInfo) {
			switch(hitInfo.HitTest) {
				case ToolboxHitTest.Group:
					this.menuObject = hitInfo.GroupInfo.Element;
					SelectComponent(hitInfo.GroupInfo.Element);
					if(e.Button == MouseButtons.Right)
						GroupMenu.Show(Toolbox, new Point(e.X, e.Y));
					break;
				case ToolboxHitTest.Item:
					this.menuObject = hitInfo.ItemInfo.Element;
					SelectComponent(hitInfo.ItemInfo.Element);
					if(e.Button == MouseButtons.Right)
						ItemMenu.Show(Toolbox, new Point(e.X, e.Y));
					break;
				case ToolboxHitTest.ExpandButton:
					Toolbox.InvertExpanded();
					break;
				case ToolboxHitTest.ScrollButtonUp:
					ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SetValue(ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SmallChange);
					break;
				case ToolboxHitTest.ScrollButtonDown:
					ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SetValue(-ViewInfo.Toolbox.ScrollController.ItemScroll.SmoothVScroll.SmallChange);
					break;
			}
		}
		protected ContextMenu GroupMenu { get { return groupMenu; } }
		protected ContextMenu ItemMenu { get { return itemMenu; } }
		object menuObject = null;
		protected void OnSelectGroup(object sender, EventArgs e) {
			ToolboxGroup group = this.menuObject as ToolboxGroup;
			Toolbox.SelectedGroup = group;
		}
		protected void OnGroupMenuDeleteGroup(object sender, EventArgs e) {
			ToolboxGroup group = this.menuObject as ToolboxGroup;
			Toolbox.Groups.Remove(group);
			if(group != null)
				group.Dispose();
			Toolbox.SelectedGroup = null;
		}
		protected void OnGroupMenuAddGroup(object sender, EventArgs e) {
			ToolboxGroup group = new ToolboxGroup();
			int insertIndex = Toolbox.Groups.IndexOf((ToolboxGroup)this.menuObject) + 1;
			Toolbox.Groups.Insert(insertIndex, group);
			DesignerHost.Container.Add(group);
			group.Caption = group.Site.Name;
		}
		protected void OnGroupMenuAddItem(object sender, EventArgs e) {
			ToolboxGroup group = this.menuObject as ToolboxGroup;
			if(group == null)
				return;
			AddNewItem(group);
		}
		protected void OnItemMenuAddItem(object sender, EventArgs e) {
			ToolboxItem item = this.menuObject as ToolboxItem;
			if(item == null)
				return;
			for(int i = 0; i < Toolbox.Groups.Count; i++) {
				if(Toolbox.Groups[i].Items.Contains(item)) {
					int insertIndex = Toolbox.Groups[i].Items.IndexOf(item) + 1;
					InsertItem(Toolbox.Groups[i], insertIndex);
					return;
				}
			}
		}
		protected void InsertItem(ToolboxGroup group, int index) {
			ToolboxItem item = new ToolboxItem();
			group.Items.Insert(index, item);
			DesignerHost.Container.Add(item);
			item.Caption = item.Site.Name;
		}
		protected void AddNewItem(ToolboxGroup group) {
			InsertItem(group, group.Items.Count);
		}
		protected void OnItemMenuDeleteItem(object sender, EventArgs e) {
			DeleteItem(menuObject as ToolboxItem);
		}
		protected virtual void DeleteItem(ToolboxItem item) {
			item.OwnerGroup.Items.Remove(item);
			Toolbox.Container.Remove(item);
			item.Dispose();
		}
	}
}
