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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionControlGroupDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			AccordionControlElement element = component as AccordionControlElement;
			if(element == null) return Rectangle.Empty;
			return element.Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			AccordionControlElement element = (AccordionControlElement)component;
			return element.AccordionControl;
		}
	}
	public class AccordionControlDesignTimeManager : BaseDesignTimeManager {
		ContextMenu groupMenu, itemMenu;
		MenuItem groupStyleMenu;
		MenuItem groupDelete, itemDelete, groupAddItem;
		public AccordionControlDesignTimeManager(AccordionControl accordionControl)
			: base(accordionControl, null) {
			this.groupMenu = CreateGroupMenu();
			this.itemMenu = CreateItemMenu();
		}
		const int AddItemId = 4;
		protected virtual ContextMenu CreateGroupMenu() {
			ContextMenu menu = new ContextMenu();
			this.groupStyleMenu = new MenuItem("&Change GroupStyle");
			menu.MenuItems.Add(new MenuItem("Add &group", new EventHandler(OnGroupMenuAddGroup)));
			menu.MenuItems.Add(groupDelete = new MenuItem("&Delete group", new EventHandler(OnGroupMenuDeleteGroup)));
			menu.MenuItems.Add(new MenuItem("-"));
			menu.MenuItems.Add(new MenuItem("&Expand group", new EventHandler(OnGroupMenuExpandGroup)));
			menu.MenuItems.Add(groupAddItem = new MenuItem("&Add item", new EventHandler(OnGroupMenuAddItem)));
			menu.MenuItems.Add(this.groupStyleMenu);
			return menu;
		}
		protected virtual ContextMenu CreateItemMenu() {
			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add(new MenuItem("&Add item", new EventHandler(OnItemMenuAddItem)));
			menu.MenuItems.Add(itemDelete = new MenuItem("&Delete item", new EventHandler(OnItemMenuDeleteItem)));
			return menu;
		}
		public AccordionControl AccordionControl { get { return Owner as AccordionControl; } }
		public override ISite Site { get { return AccordionControl == null ? null : AccordionControl.Site; } }
		public override void Dispose() {
			if(this.groupMenu != null) this.groupMenu.Dispose();
			if(this.itemMenu != null) this.itemMenu.Dispose();
			if(this.groupStyleMenu != null) this.groupStyleMenu.Dispose();
			base.Dispose();
		}
		public override void InvalidateComponent(object component) {
			AccordionControl.Invalidate(false);
		}
		protected override void OnDesignTimeSelectionChanged(object component) {
			AccordionControlElement group = component as AccordionControlElement;
			if(group != null && group.AccordionControl == AccordionControl) InvalidateComponent(group);
		}
		public virtual void OnRightClick(MouseEventArgs e) {
			AccordionControlHitInfo hitInfo = AccordionControl.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.HitTest == AccordionControlHitTest.Group) {
				this.menuObject = hitInfo.ItemInfo;
				SelectComponent(hitInfo.ItemInfo);
				GroupMenu.Show(AccordionControl, new Point(e.X, e.Y));
				return;
			}
			if(hitInfo.HitTest == AccordionControlHitTest.Item) {
				this.menuObject = hitInfo.ItemInfo;
				SelectComponent(hitInfo.ItemInfo);
				ItemMenu.Show(AccordionControl, new Point(e.X, e.Y));
				return;
			}
		}
		protected ContextMenu GroupMenu { get { return groupMenu; } }
		protected ContextMenu ItemMenu { get { return itemMenu; } }
		object menuObject = null;
		protected void OnGroupMenuDeleteGroup(object sender, EventArgs e) {
			AccordionControlElement group = this.menuObject as AccordionControlElement;
			if(group != null) group.Dispose();
		}
		protected void OnGroupMenuAddGroup(object sender, EventArgs e) {
			AccordionControlElement group = AccordionControl.AddGroup();
			SelectComponent(group);
		}
		protected void OnGroupMenuExpandGroup(object sender, EventArgs e) {
			AccordionControlElement group = this.menuObject as AccordionControlElement;
			if(group != null) {
				group.Expanded = true;
				AccordionControl.ForceFireChanged();
			}
		}
		protected void OnGroupMenuAddItem(object sender, EventArgs e) {
			AccordionControlElement group = this.menuObject as AccordionControlElement;
			if(group == null) return;
			AddItem(group);
			group.Expanded = true;
		}
		protected void OnItemMenuAddItem(object sender, EventArgs e) {
			AccordionControlElement item = this.menuObject as AccordionControlElement;
			if(item != null) {
				AccordionControlElement newItem = new AccordionControlElement(ElementStyle.Item);
				item.OwnerElement.Elements.Add(newItem);
				if(newItem != null) {
					SelectComponent(newItem);
				}
			}
		}
		protected AccordionControlElement AddItem(AccordionControlElement group) {
			if(group == null) return null;
			AccordionControlElement item = new AccordionControlElement(ElementStyle.Item);
			group.Elements.Add(item);
			return item;
		}
		protected void OnItemMenuDeleteItem(object sender, EventArgs e) {
			AccordionControlElement item = this.menuObject as AccordionControlElement;
			if(item != null) {
				AccordionControlElement group = item.OwnerElement;
				int index = group.Elements.IndexOf(item);
				item.Dispose();
				if(index != -1) {
					index = Math.Min(index, group.Elements.Count - 1);
					if(index != -1) SelectComponent(group.Elements[index]);
				}
			}
		}
	}
}
