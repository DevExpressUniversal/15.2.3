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
using System.Collections;
using System.Drawing;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraScheduler {
	#region SchedulerMenuItem
	public class SchedulerMenuItem : CommandMenuItem<SchedulerMenuItemId> {
		public SchedulerMenuItem() : this(String.Empty) { }
		public SchedulerMenuItem(string caption) : this(caption, null) { }
		public SchedulerMenuItem(string caption, EventHandler click) : this(caption, click, null) { }
		public SchedulerMenuItem(string caption, EventHandler click, Image image) : this(caption, click, image, null) { }
		public SchedulerMenuItem(string caption, EventHandler click, Image image, EventHandler update)
			: base(caption, click, image, update) {
			this.Id = SchedulerMenuItemId.Custom;
		}
	}
	#endregion
	#region SchedulerMenuCheckItem
	public class SchedulerMenuCheckItem : CommandMenuCheckItem<SchedulerMenuItemId> {
		public SchedulerMenuCheckItem() : this(String.Empty) { }
		public SchedulerMenuCheckItem(string caption) : this(caption, false) { }
		public SchedulerMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged) : this(caption, check, image, checkedChanged, null) { }
		public SchedulerMenuCheckItem(string caption, bool check) : this(caption, check, null) { }
		public SchedulerMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged, EventHandler update)
			: base(caption, check, image, checkedChanged, update) {
			this.Id = SchedulerMenuItemId.Custom;
		}
		public SchedulerMenuCheckItem(string caption, bool check, EventHandler update)
			: base(caption, check, update) {
			this.Id = SchedulerMenuItemId.Custom;
		}
	}
	#endregion
	public class SchedulerPopupMenu : CommandPopupMenu<SchedulerMenuItemId> {
		public SchedulerPopupMenu(EventHandler beforePopup)
			: base(beforePopup) {
			this.Id = SchedulerMenuItemId.Custom;
		}
		public SchedulerPopupMenu()
			: base() {
			this.Id = SchedulerMenuItemId.Custom;
		}
		public SchedulerMenuItem GetMenuItemById(SchedulerMenuItemId id) {
			return GetMenuItemById(id, true);
		}
		public SchedulerMenuItem GetMenuItemById(SchedulerMenuItemId id, bool recursive) {
			return GetMenuItemById(this, id, recursive);
		}
		public SchedulerMenuCheckItem GetMenuCheckItemById(SchedulerMenuItemId id) {
			return GetMenuCheckItemById(id, true);
		}
		public SchedulerMenuCheckItem GetMenuCheckItemById(SchedulerMenuItemId id, bool recursive) {
			return GetMenuCheckItemById(this, id, recursive);
		}
		public SchedulerPopupMenu GetPopupMenuById(SchedulerMenuItemId id) {
			return GetPopupMenuById(id, true);
		}
		public SchedulerPopupMenu GetPopupMenuById(SchedulerMenuItemId id, bool recursive) {
			return GetPopupMenuById(this, id, recursive);
		}
		public static SchedulerMenuItem GetMenuItemById(DXPopupMenu menu, SchedulerMenuItemId id, bool recursive) {
			DXMenuItem item = GetDXMenuItemById(menu, id, recursive);
			return item as SchedulerMenuItem;
		}
		public static SchedulerMenuCheckItem GetMenuCheckItemById(DXPopupMenu menu, SchedulerMenuItemId id, bool recursive) {
			DXMenuItem item = GetDXMenuItemById(menu, id, recursive);
			return item as SchedulerMenuCheckItem;
		}
		public static SchedulerPopupMenu GetPopupMenuById(DXPopupMenu menu, SchedulerMenuItemId id, bool recursive) {
			DXMenuItem item = GetDXMenuItemById(menu, id, recursive);
			return item as SchedulerPopupMenu;
		}
	}
	public sealed class PopupMenuHelper {
		PopupMenuHelper() {
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler) {
			return AddMenuItem(menu, id, text, handler, null, false);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, Image image) {
			return AddMenuItem(menu, id, text, handler, null, false, image);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, bool beginGroup) {
			return AddMenuItem(menu, id, text, handler, null, beginGroup, null);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, bool beginGroup, Image image) {
			return AddMenuItem(menu, id, text, handler, null, beginGroup, image);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, EventHandler update) {
			return AddMenuItem(menu, id, text, handler, update, false);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, EventHandler update, Image image) {
			return AddMenuItem(menu, id, text, handler, update, false, image);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, EventHandler update, bool beginGroup) {
			return AddMenuItem(menu, id, text, handler, update, beginGroup, null);
		}
		public static SchedulerMenuItem AddMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, EventHandler update, bool beginGroup, Image image) {
			SchedulerMenuItem item = new SchedulerMenuItem(text, handler);
			item.Id = id;
			menu.Items.Add(item);
			if (update != null)
				item.Update += update;
			item.BeginGroup = beginGroup;
			if (image != null)
				item.Image = image;
			return item;
		}
		public static SchedulerMenuCheckItem AddMenuCheckItem(SchedulerPopupMenu menu, string text, EventHandler handler) {
			return AddMenuCheckItem(menu, SchedulerMenuItemId.Custom, text, handler, null);
		}
		public static SchedulerMenuCheckItem AddMenuCheckItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler) {
			return AddMenuCheckItem(menu, id, text, handler, null);
		}
		public static SchedulerMenuCheckItem AddMenuCheckItem(SchedulerPopupMenu menu, string text, EventHandler handler, EventHandler update) {
			return AddMenuCheckItem(menu, SchedulerMenuItemId.Custom, text, handler, update);
		}
		public static SchedulerMenuCheckItem AddMenuCheckItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, string text, EventHandler handler, EventHandler update) {
			SchedulerMenuCheckItem item = new SchedulerMenuCheckItem(text);
			item.Id = id;
			menu.Items.Add(item);
			if (handler != null)
				item.CheckedChanged += handler;
			if (update != null)
				item.Update += update;
			return item;
		}
		public static void MoveMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, int to) {
			menu.MoveMenuItem(id, to);
		}
		public static void MoveMenuCheckItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, int to) {
			menu.MoveMenuCheckItem(id, to);
		}
		public static void MoveSubMenuItem(SchedulerPopupMenu menu, SchedulerMenuItemId id, int to) {
			menu.MoveSubMenuItem(id, to);
		}
	}
}
