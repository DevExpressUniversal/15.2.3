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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Menu;
namespace DevExpress.Xpf.RichEdit {
	#region RichEditHoverMenu
	public class RichEditHoverMenu : Bar, IDXPopupMenu<RichEditCommandId> {
		public RichEditHoverMenu()
			: base() {
		}
		protected override bool ShowWhenBarManagerIsMerged { get { return true; } }
		#region IDXPopupMenu<RichEditCommandId> Members
		public void AddItem(IDXMenuItemBase<RichEditCommandId> item) {
			BarItem dxItem = item as BarItem;
			if (dxItem != null) {
				BarItemLink link = dxItem.CreateLink(true);
				this.ItemLinks.Add(link);
			}
		}
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditHoverMenuId")]
#endif
		public RichEditCommandId Id { get { return RichEditCommandId.None; } set { } }
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditHoverMenuItemsCount")]
#endif
		public int ItemsCount { get { return this.ItemLinks.Count; } }
		#endregion
		#region IDXMenuItemBase<RichEditCommandId> Members
#if !SL
	[DevExpressXpfRichEditLocalizedDescription("RichEditHoverMenuBeginGroup")]
#endif
		public bool BeginGroup { get { return false; } set { } }
		#endregion
	}
	#endregion
}
namespace DevExpress.Xpf.RichEdit.Menu {
	#region RichEditMenuItem
	public class RichEditMenuItem : CommandMenuItem<RichEditCommandId> {
	}
	#endregion
	#region RichEditMenuCheckItem
	public class RichEditMenuCheckItem : CommandMenuCheckItem<RichEditCommandId> {
	}
	#endregion
	#region RichEditPopupMenuOld
	public class RichEditPopupMenuOld : PopupMenu, IDXPopupMenu<RichEditCommandId> {
		public RichEditPopupMenuOld()
			: base() {
		}
		#region IDXPopupMenu<RichEditCommandId> Members
		public void AddItem(IDXMenuItemBase<RichEditCommandId> item) {
			BarItem dxItem = item as BarItem;
			if (dxItem != null) {
				BarItemLink link = dxItem.CreateLink(true);
				this.ItemLinks.Add(link);
			}
		}
		public string Caption { get { return String.Empty; } set {} }
		public RichEditCommandId Id { get { return RichEditCommandId.None; } set { } }
		public int ItemsCount { get { return this.ItemLinks.Count; } }
		bool IDXPopupMenu<RichEditCommandId>.Visible { get { return this.Visibility == System.Windows.Visibility.Visible; } set { if (value) Visibility = System.Windows.Visibility.Visible; else Visibility = System.Windows.Visibility.Collapsed; } }
		#endregion
		#region IDXMenuItemBase<RichEditCommandId> Members
		public bool BeginGroup { get { return false; } set { } }
		#endregion
	}
	#endregion
	#region MenuItemCommandAdapterBase (abstract class)
	public abstract class MenuItemCommandAdapterBase {
		readonly RichEditCommand command;
		protected MenuItemCommandAdapterBase(RichEditCommand command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		public RichEditCommand Command { get { return command; } }
		protected internal virtual void InitializeMenuItem(BarItem item) {
			item.Content = MenuItemHelper.ValidateMenuCaption(Command.MenuCaption);
			if (Command.Image != null)
				item.Glyph = ImageHelper.CreatePlatformImage(Command.Image).Source;
			if(Command.LargeImage != null)
				item.LargeGlyph = ImageHelper.CreatePlatformImage(Command.LargeImage).Source;
			item.Command = MenuItemHelper.WrapCommand(Command);
			item.CommandParameter = Command.Control;
			item.IsPrivate = true;
			UpdateState(item);
		}
		public virtual void UpdateState(BarItem item) {
			UpdateBarItemCommandState(item, command);
		}
		internal static void UpdateBarItemCommandState(BarItem item, Command command) {
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			RichEditBarItemCommandUIState menuItemState = new RichEditBarItemCommandUIState(item);
			menuItemState.Visible = state.Visible;
			menuItemState.Enabled = state.Enabled;
			menuItemState.Checked = state.Checked;
		}
	}
	#endregion
	#region MenuItemCommandAdapter<T> (abstract class)
	public abstract class MenuItemCommandAdapter<T> : MenuItemCommandAdapterBase, IDXMenuItemCommandAdapter<T> where T : struct {
		protected MenuItemCommandAdapter(RichEditCommand command)
			: base(command) {
		}
		public abstract IDXMenuItem<T> CreateMenuItem(DXMenuItemPriority priority);
	}
	#endregion
	#region MenuCheckItemCommandAdapter<T> (abstract class)
	public abstract class MenuCheckItemCommandAdapter<T> : MenuItemCommandAdapterBase, IDXMenuCheckItemCommandAdapter<T> where T : struct {
		protected MenuCheckItemCommandAdapter(RichEditCommand command)
			: base(command) {
		}
		public abstract IDXMenuCheckItem<T> CreateMenuItem(string groupId);
	}
	#endregion
	#region RichEditMenuItemCommandSLAdapter
	public class RichEditMenuItemCommandSLAdapter : MenuItemCommandAdapter<RichEditCommandId> {
		public RichEditMenuItemCommandSLAdapter(RichEditCommand command)
			: base(command) {
		}
		public override IDXMenuItem<RichEditCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			RichEditMenuItem item = new RichEditMenuItem();
			InitializeMenuItem(item);
			return item;
		}
	}
	#endregion
	#region RichEditMenuCheckItemCommandSLAdapter
	public class RichEditMenuCheckItemCommandSLAdapter : MenuCheckItemCommandAdapter<RichEditCommandId> {
		public RichEditMenuCheckItemCommandSLAdapter(RichEditCommand command)
			: base(command) {
		}
		public override IDXMenuCheckItem<RichEditCommandId> CreateMenuItem(string groupId) {
			RichEditMenuCheckItem item = new RichEditMenuCheckItem();
			InitializeMenuItem(item);
			item.GroupIndex = groupId.GetHashCode();
			return item;
		}
	}
	#endregion
	#region RichEditToolMenuItemCommandSLAdapter
	public class RichEditToolMenuItemCommandSLAdapter : MenuItemCommandAdapter<RichEditCommandId> {
		readonly BarManager barManager;
		public RichEditToolMenuItemCommandSLAdapter(RichEditCommand command, BarManager barManager)
			: base(command) {
			Guard.ArgumentNotNull(barManager, "barManager");
			this.barManager = barManager;
		}
		public override IDXMenuItem<RichEditCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			RichEditMenuItem item = new RichEditMenuItem();
			InitializeMenuItem(item);
			return item;
		}
	}
	#endregion
	#region BeginGroupHelper
	public static class BeginGroupHelper {
		public static bool GetBeginGroup(BarItem item) {
			BarItemLinkCollection links = GetParentLinks(item);
			if (links == null)
				return false;
			BarItemLinkBase link = item.Links.Count > 0 ? item.Links[0] : null;
			if (link == null)
				return false;
			int index = links.IndexOf(link);
			if (index <= 0)
				return false;
			return links[index] is BarItemLinkSeparator;
		}
		public static void SetBeginGroup(BarItem item, bool value) {
			if (GetBeginGroup(item) == value)
				return;
			BarItemLinkCollection links = GetParentLinks(item);
			if (links == null)
				return;
			BarItemLinkBase link = item.Links.Count > 0 ? item.Links[0] : null;
			if (link == null)
				return;
			int index = links.IndexOf(link);
			if (index < 0)
				return;
			if (value) {
				if (index > 0)
					links.Insert(index, new BarItemLinkSeparator());
			}
			else {
				if (index <= 0)
					return;
				BarItemLinkSeparator separator = links[index - 1] as BarItemLinkSeparator;
				if (separator != null)
					links.RemoveAt(index - 1);
			}
		}
		static BarItemLinkCollection GetParentLinks(BarItem item) {
			if (item.Links.Count <= 0)
				return null;
			ILinksHolder holder = item.Links[0].Links.Holder;
			if (holder != null)
				return holder.Links;
			else
				return null;
		}
	}
	#endregion
	#region CommandMenuItem<T>
	public class CommandMenuItem<T> : BarButtonItem, IDXMenuItem<T> where T : struct {
		bool beginGroup;
		public CommandMenuItem() {
		}
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
	}
	#endregion
	#region CommandMenuCheckItem<T>
	public class CommandMenuCheckItem<T> : BarCheckItem, IDXMenuCheckItem<T> where T : struct {
		bool beginGroup;
		public CommandMenuCheckItem() {
		}
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
	}
	#endregion
	#region XpfRichEditContentMenuBuilder
	public class XpfRichEditContentMenuBuilder : RichEditContentMenuBuilder {
		public XpfRichEditContentMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			RichEditMenuBuilderInfo builderInfo = (RichEditMenuBuilderInfo)base.CreatePopupMenu();
			RichEditControl control = (RichEditControl)this.Control;
			builderInfo.MenuController = control.MenuController;
			return builderInfo;
		}
		protected internal override RichEditHitTestResult CalculateCursorHitTestResult() {
			return null;
		}
	}
	#endregion
	#region XpfICommandAdapter
	public class XpfICommandAdapter : ICommand {
		readonly Command command;
		public XpfICommandAdapter(Command command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		#region ICommand Members
		public bool CanExecute(object parameter) {
			return this.command.CanExecute();
		}
		public event EventHandler CanExecuteChanged;
		public void Execute(object parameter) {
			this.command.Execute();
		}
		void FakeMethod() {
			CanExecuteChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	#endregion
	#region MenuItemHelper
	public static class MenuItemHelper {
		internal static string ValidateMenuCaption(string caption) {
			return caption.Replace("&", String.Empty);
		}
		public static ICommand WrapCommand(RichEditCommand command) {
			if (command.Id == new RichEditCommandId(0))
				return new XpfICommandAdapter(command);
			else if (command.Id == RichEditCommandId.ReplaceMisspelling)
				return new RichEditUICommand(command.Id, ((ReplaceMisspellingCommand)command).Suggestion);
			else
				return new RichEditUICommand(command.Id);
		}
	}
	#endregion
	#region CommandMenuBuilderInfo<T> (abstract class)
	public abstract class CommandMenuBuilderInfo<T> : IDXPopupMenu<T> where T : struct {
		#region Fields
		readonly List<IDXMenuItemBase<T>> items;
		string caption;
		T id;
		bool beginGroup;
		bool isVisible = true;
		#endregion
		protected CommandMenuBuilderInfo() {
			this.items = new List<IDXMenuItemBase<T>>();
		}
		internal List<IDXMenuItemBase<T>> Items { get { return items; } }
		public BarManagerMenuController MenuController { get; set; }
		#region IDXPopupMenu<SchedulerMenuItemId> Members
		public string Caption { get { return caption; } set { caption = value; } }
		public T Id { get { return id; } set { id = value; } }
		public int ItemsCount { get { return Items.Count; } }
		public bool Visible { get { return isVisible; } set { isVisible = value; } }
		public void AddItem(IDXMenuItemBase<T> item) {
			Items.Add(item);
		}
		#endregion
		#region IDXMenuItemBase<T> Members
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
	}
	#endregion
	public class RichEditMenuBuilderInfo : CommandMenuBuilderInfo<RichEditCommandId> {
	}
	public class RichEditPopupMenu : CustomizablePopupMenuBase {
		readonly RichEditControl control;
		public RichEditPopupMenu(RichEditControl control)
			: base(control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		internal RichEditMenuBuilderInfo MenuBuilderInfo { get; set; }
		internal RichEditControl Control { get { return control; } }
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			RichEditMenuInfo info = new RichEditMenuInfo(this);
			info.MenuBuilderInfo = MenuBuilderInfo;
			return info;
		}
		internal void AddMenuItem(BarItemLinkCollection links, BarItem item) {
			IDXMenuItemBase<RichEditCommandId> itemInfo = item as IDXMenuItem<RichEditCommandId>;
			if (itemInfo != null)
				AddItemCore(item, itemInfo.BeginGroup, links);
			else
				AddItemCore(item, false, links);
		}
		protected override bool RaiseShowMenu() {
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(this);
			Control.RaisePopupMenuShowing(args);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			return args.Menu != null;
		}
	}
	public class RichEditMenuInfo : MenuInfoBase {
		RichEditMenuBuilderInfo menuBuilderInfo;
		int index;
		public RichEditMenuInfo(RichEditPopupMenu menu)
			: base(menu) {
		}
		public RichEditMenuBuilderInfo MenuBuilderInfo { get { return menuBuilderInfo; } set { menuBuilderInfo = value; } }
		public override bool CanCreateItems { get { return true; } }
		public new RichEditPopupMenu Menu { get { return (RichEditPopupMenu)base.Menu; } }
		public override BarManagerMenuController MenuController {
			get {
				if (MenuBuilderInfo != null)
					return MenuBuilderInfo.MenuController;
				return null;
			}
		}
		protected override void CreateItems() {
			index = 0;
			AddMenuItems(Menu.ItemLinks, MenuBuilderInfo);
		}
		void AddMenuItems(BarItemLinkCollection menuLinks, RichEditMenuBuilderInfo menuInfo) {
			menuLinks.BeginUpdate();
			foreach (IDXMenuItemBase<RichEditCommandId> item in menuInfo.Items)
				AddMenuItem(menuLinks, item);
			menuLinks.EndUpdate();
		}
		void AddMenuItem(BarItemLinkCollection menuLinks, IDXMenuItemBase<RichEditCommandId> item) {
			RichEditMenuBuilderInfo menuInfo = item as RichEditMenuBuilderInfo;
			if (menuInfo == null) {
				BarItem barItem = item as BarItem;
				if (barItem == null)
					return;
				if (!barItem.Command.CanExecute(null))
					return;
				Menu.AddMenuItem(menuLinks, barItem);
			}
			else
				AddSubMenu(menuLinks, menuInfo);
		}
		void AddSubMenu(BarItemLinkCollection menuLinks, RichEditMenuBuilderInfo menuInfo) {
			string caption = MenuItemHelper.ValidateMenuCaption(menuInfo.Caption);			
			BarSubItem subItem = Menu.CreateBarSubItem("subMenu" + menuInfo.Id.ToString() + index.ToString(), caption, menuInfo.BeginGroup, null, menuLinks);
			subItem.IsVisible = menuInfo.Visible;
			index++;
			AddMenuItems(subItem.ItemLinks, menuInfo);
		}
	}
}
