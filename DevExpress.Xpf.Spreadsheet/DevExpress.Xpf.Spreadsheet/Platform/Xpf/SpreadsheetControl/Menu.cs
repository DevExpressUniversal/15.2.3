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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Spreadsheet.Menu;
using DevExpress.Xpf.Spreadsheet.Menu.Internal;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Menu;
namespace DevExpress.Xpf.Spreadsheet.Menu {
	[ContentPropertyAttribute("Customizations")]
	public class SpreadsheetMenuCustomization : DependencyObject {
		public static readonly DependencyProperty MenuTypeProperty;
		public static readonly DependencyProperty CustomizationsProperty;
		static SpreadsheetMenuCustomization() {
			MenuTypeProperty =
				DependencyProperty.Register("MenuType", typeof(SpreadsheetMenuType?), typeof(SpreadsheetMenuCustomization), new FrameworkPropertyMetadata(null));
			CustomizationsProperty =
				DependencyProperty.Register("Customizations", typeof(ObservableCollection<IBarManagerControllerAction>), typeof(SpreadsheetMenuCustomization));
		}
		public SpreadsheetMenuCustomization() {
			Customizations = new ObservableCollection<IBarManagerControllerAction>();
		}
		public SpreadsheetMenuType? MenuType {
			get { return (SpreadsheetMenuType?)GetValue(MenuTypeProperty); }
			set { SetValue(MenuTypeProperty, value); }
		}
		public ObservableCollection<IBarManagerControllerAction> Customizations {
			get { return (ObservableCollection<IBarManagerControllerAction>)GetValue(CustomizationsProperty); }
			set { SetValue(CustomizationsProperty, value); }
		}
	}
	public class PopupMenuShowingEventArgs : EventArgs {
		PopupMenu menu;
		SpreadsheetPopupMenu originMenu;
		public PopupMenuShowingEventArgs(SpreadsheetPopupMenu menu, SpreadsheetMenuType menuType) {
			Guard.ArgumentNotNull(menu, "menu");
			this.menu = menu;
			this.originMenu = menu;
			this.MenuType = menuType;
		}
		public PopupMenu Menu { get { return menu; } set { menu = value; } }
		public BarManagerActionCollection Customizations { get { return originMenu.Customizations; } }
		public SpreadsheetMenuType MenuType { get; private set; }
	}
	public class RemoveSpreadsheetCommandAction : RemoveBarItemLinkAction {
		public static readonly DependencyProperty IdProperty;
		static RemoveSpreadsheetCommandAction() {
			IdProperty = DependencyProperty.Register("Id", typeof(SpreadsheetCommandId), typeof(RemoveSpreadsheetCommandAction));
		}
		public SpreadsheetCommandId Id {
			get { return (SpreadsheetCommandId)GetValue(IdProperty); }
			set { SetValue(IdProperty, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			var holder = GetTarget(Target);
			if(holder == null)
				return;
			foreach(var link in holder.Links.Where(x => CheckLink(x, Id)).ToArray()) {
				holder.Links.Remove(link);
			}
		}
		protected virtual bool CheckLink(BarItemLinkBase link, SpreadsheetCommandId ID) {
			var uiCommand = (link as BarItemLink).With(x => x.Item).With(x => x.Command as SpreadsheetUICommand);
			if (uiCommand == null) return false;
			return uiCommand.CommandId == ID;
		}
	}
	public class SpreadsheetPopupMenu : CustomizablePopupMenuBase {
		readonly SpreadsheetControl control;
		public SpreadsheetPopupMenu(SpreadsheetControl control)
			: base(control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		internal SpreadsheetMenuBuilderInfo MenuBuilderInfo { get; set; }
		internal SpreadsheetControl Control { get { return control; } }
		internal SpreadsheetMenuType MenuType { get; set; }
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			SpreadsheetMenuInfo info = new SpreadsheetMenuInfo(this);
			info.MenuBuilderInfo = MenuBuilderInfo;
			return info;
		}
		internal void AddMenuItem(BarItemLinkCollection links, BarItem item) {
			IDXMenuItemBase<SpreadsheetCommandId> itemInfo = item as IDXMenuItem<SpreadsheetCommandId>;
			if (itemInfo != null)
				AddItemCore(item, itemInfo.BeginGroup, links);
			else
				AddItemCore(item, false, links);
		}
		protected override bool RaiseShowMenu() {			
			if(control.MenuCustomizations != null){
				var menuCustomization = control.MenuCustomizations.Where(m => m.MenuType == null || m.MenuType.Value == MenuType).Select(m => m).ToList();
				foreach(var customuzations in menuCustomization.Select(x => x.Customizations)){
					foreach (var c in customuzations)
						this.Customizations.Add(c);
				}
			}
			PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(this, MenuType);
			control.OnPopupContextMenuShowing(args);
			if (args.Menu == null)
				return false;
			else if (args.Menu != this) {
				ItemLinks.Clear();
				((ILinksHolder)this).Merge(args.Menu);
			}
			return base.RaiseShowMenu();
		}
		private Point GetMenuPosition() {
			return new Point(HorizontalOffset, VerticalOffset);
		}
	}
}
namespace DevExpress.Xpf.Spreadsheet.Menu.Internal {
	public class SpreadsheetMenuInfo : MenuInfoBase {
		SpreadsheetMenuBuilderInfo menuBuilderInfo;
		public SpreadsheetMenuInfo(SpreadsheetPopupMenu menu)
			: base(menu) {
		}
		public SpreadsheetMenuBuilderInfo MenuBuilderInfo { get { return menuBuilderInfo; } set { menuBuilderInfo = value; } }
		public override bool CanCreateItems { get { return true; } }
		public new SpreadsheetPopupMenu Menu { get { return (SpreadsheetPopupMenu)base.Menu; } }
		public override BarManagerMenuController MenuController {
			get {
				if (MenuBuilderInfo != null)
					return MenuBuilderInfo.MenuController;
				return null;
			}
		}
		protected override void CreateItems() {
			CreateItemsCore();
		}
		internal void CreateItemsCore() {
			AddMenuItems(Menu.ItemLinks, MenuBuilderInfo);
		}
		void AddMenuItems(BarItemLinkCollection menuLinks, SpreadsheetMenuBuilderInfo menuInfo) {
			menuLinks.BeginUpdate();
			foreach (IDXMenuItemBase<SpreadsheetCommandId> item in menuInfo.Items)
				AddMenuItem(menuLinks, item);
			menuLinks.EndUpdate();
		}
		void AddMenuItem(BarItemLinkCollection menuLinks, IDXMenuItemBase<SpreadsheetCommandId> item) {
			SpreadsheetMenuBuilderInfo menuInfo = item as SpreadsheetMenuBuilderInfo;
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
		void AddSubMenu(BarItemLinkCollection menuLinks, SpreadsheetMenuBuilderInfo menuInfo) {
			string caption = MenuItemHelper.ValidateMenuCaption(menuInfo.Caption);
			BarSubItem subItem = Menu.CreateBarSubItem("subMenu" + menuInfo.Id.ToString(), caption, menuInfo.BeginGroup, null, menuLinks);
			subItem.IsVisible = menuInfo.Visible;
			AddMenuItems(subItem.ItemLinks, menuInfo);
		}
	}
	#region SpreadsheetMenuItem
	public class SpreadsheetMenuItem : CommandMenuItem<SpreadsheetCommandId> {
	}
	#endregion
	#region SpreadsheetMenuCheckItem
	public class SpreadsheetMenuCheckItem : CommandMenuCheckItem<SpreadsheetCommandId> {
	}
	#endregion
	#region MenuItemCommandAdapterBase (abstract class)
	public abstract class MenuItemCommandAdapterBase {
		readonly SpreadsheetCommand command;
		protected MenuItemCommandAdapterBase(SpreadsheetCommand command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		public SpreadsheetCommand Command { get { return command; } }
		protected internal virtual void InitializeMenuItem(BarItem item) {
			item.Content = MenuItemHelper.ValidateMenuCaption(Command.MenuCaption);
			if (Command.Image != null)
				item.Glyph = ImageHelper.CreatePlatformImage(Command.Image).Source;
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
			SpreadsheetBarItemCommandUIState menuItemState = new SpreadsheetBarItemCommandUIState(item);
			menuItemState.Visible = state.Visible;
			menuItemState.Enabled = state.Enabled;
			menuItemState.Checked = state.Checked;
		}
	}
	#endregion
	#region MenuItemCommandAdapter<T> (abstract class)
	public abstract class MenuItemCommandAdapter<T> : MenuItemCommandAdapterBase, IDXMenuItemCommandAdapter<T> where T : struct {
		protected MenuItemCommandAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public abstract IDXMenuItem<T> CreateMenuItem(DXMenuItemPriority priority);
	}
	#endregion
	#region MenuCheckItemCommandAdapter<T> (abstract class)
	public abstract class MenuCheckItemCommandAdapter<T> : MenuItemCommandAdapterBase, IDXMenuCheckItemCommandAdapter<T> where T : struct {
		protected MenuCheckItemCommandAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public abstract IDXMenuCheckItem<T> CreateMenuItem(string groupId);
	}
	#endregion
	#region SpreadsheetMenuItemCommandSLAdapter
	public class SpreadsheetMenuItemCommandSLAdapter : MenuItemCommandAdapter<SpreadsheetCommandId> {
		public SpreadsheetMenuItemCommandSLAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public override IDXMenuItem<SpreadsheetCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			SpreadsheetMenuItem item = new SpreadsheetMenuItem();
			InitializeMenuItem(item);
			return item;
		}
	}
	#endregion
	#region SpreadsheetMenuCheckItemCommandSLAdapter
	public class SpreadsheetMenuCheckItemCommandSLAdapter : MenuCheckItemCommandAdapter<SpreadsheetCommandId> {
		public SpreadsheetMenuCheckItemCommandSLAdapter(SpreadsheetCommand command)
			: base(command) {
		}
		public override IDXMenuCheckItem<SpreadsheetCommandId> CreateMenuItem(string groupId) {
			SpreadsheetMenuCheckItem item = new SpreadsheetMenuCheckItem();
			InitializeMenuItem(item);
			item.GroupIndex = groupId.GetHashCode();
			return item;
		}
	}
	#endregion
	#region SpreadsheetToolMenuItemCommandSLAdapter
	public class SpreadsheetToolMenuItemCommandSLAdapter : MenuItemCommandAdapter<SpreadsheetCommandId> {
		readonly BarManager barManager;
		public SpreadsheetToolMenuItemCommandSLAdapter(SpreadsheetCommand command, BarManager barManager)
			: base(command) {
			Guard.ArgumentNotNull(barManager, "barManager");
			this.barManager = barManager;
		}
		public override IDXMenuItem<SpreadsheetCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			SpreadsheetMenuItem item = new SpreadsheetMenuItem();
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
	#region XpfSpreadsheetContentMenuBuilder
	public class XpfSpreadsheetContentMenuBuilder : SpreadsheetContentMenuBuilder {
		public XpfSpreadsheetContentMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			SpreadsheetMenuBuilderInfo builderInfo = (SpreadsheetMenuBuilderInfo)base.CreatePopupMenu();
			SpreadsheetControl control = (SpreadsheetControl)this.Control;
			builderInfo.MenuController = control.GetMenuController();
			return builderInfo;
		}
		protected internal override void AddChartChangeTypeMenuItem(IDXPopupMenu<SpreadsheetCommandId> menu, InnerSpreadsheetControl innerControl) {
		}
		protected internal override void AddChartSelectDataMenuItem(IDXPopupMenu<SpreadsheetCommandId> menu, InnerSpreadsheetControl innerControl) {
		}
	}
	public class XpfSpreadsheetTabSelectorMenuBuilder : SpreadsheetTabSelectorMenuBuilder {
		public XpfSpreadsheetTabSelectorMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			SpreadsheetMenuBuilderInfo builderInfo = (SpreadsheetMenuBuilderInfo)base.CreatePopupMenu();
			SpreadsheetControl control = (SpreadsheetControl)this.Control;
			builderInfo.MenuController = control.GetMenuController();
			return builderInfo;
		}
	}
	public class XpfSpreadsheetAutoFilterMenuBuilder : SpreadsheetAutoFilterMenuBuilder {
		public XpfSpreadsheetAutoFilterMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			SpreadsheetMenuBuilderInfo builderInfo = (SpreadsheetMenuBuilderInfo)base.CreatePopupMenu();
			SpreadsheetControl control = (SpreadsheetControl)this.Control;
			builderInfo.MenuController = control.GetMenuController();
			return builderInfo;
		}
	}
	public class XpfSpreadsheetPivotTableAutoFilterMenuBuilder : SpreadsheetPivotTableAutoFilterMenuBuilder {
		public XpfSpreadsheetPivotTableAutoFilterMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			SpreadsheetMenuBuilderInfo builderInfo = (SpreadsheetMenuBuilderInfo)base.CreatePopupMenu();
			SpreadsheetControl control = (SpreadsheetControl)this.Control;
			builderInfo.MenuController = control.GetMenuController();
			return builderInfo;
		}
	}
	public class XpfSpreadsheetPivotTableMenuBuilder : SpreadsheetPivotTableMenuBuilder {
		public XpfSpreadsheetPivotTableMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			SpreadsheetMenuBuilderInfo builderInfo = (SpreadsheetMenuBuilderInfo)base.CreatePopupMenu();
			SpreadsheetControl control = (SpreadsheetControl)this.Control;
			builderInfo.MenuController = control.GetMenuController();
			return builderInfo;
		}
	}
	#endregion
	#region XpfICommandAdapter
	public class XpfICommandAdapter : System.Windows.Input.ICommand {
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
		public static System.Windows.Input.ICommand WrapCommand(SpreadsheetCommand command) {
			if (command.Id == new SpreadsheetCommandId(0))
				return new XpfICommandAdapter(command);
			else
				return new SpreadsheetUICommand(command.Id);
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
		public bool Visible {
			get { return isVisible; }
			set { isVisible = value; }
		}
		public void AddItem(IDXMenuItemBase<T> item) {
			Items.Add(item);
		}
		#endregion
		#region IDXMenuItemBase<T> Members
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
	}
	#endregion
	public class SpreadsheetMenuBuilderInfo : CommandMenuBuilderInfo<SpreadsheetCommandId> {
	}
}
