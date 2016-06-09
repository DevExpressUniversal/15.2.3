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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Bars;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Commands;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Scheduler;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.Office.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	public class SchedulerMenuItemName {
		public static readonly string OpenAppointment = SchedulerMenuItemId.OpenAppointment.ToString();
		public static readonly string PrintAppointment = SchedulerMenuItemId.PrintAppointment.ToString();
		public static readonly string DeleteAppointment = SchedulerMenuItemId.DeleteAppointment.ToString();
		public static readonly string EditSeries = SchedulerMenuItemId.EditSeries.ToString();
		public static readonly string NewAppointment = SchedulerMenuItemId.NewAppointment.ToString();
		public static readonly string NewAllDayEvent = SchedulerMenuItemId.NewAllDayEvent.ToString();
		public static readonly string NewRecurringAppointment = SchedulerMenuItemId.NewRecurringAppointment.ToString();
		public static readonly string NewRecurringEvent = SchedulerMenuItemId.NewRecurringEvent.ToString();
		public static readonly string GotoThisDay = SchedulerMenuItemId.GotoThisDay.ToString();
		public static readonly string GotoToday = SchedulerMenuItemId.GotoToday.ToString();
		public static readonly string GotoDate = SchedulerMenuItemId.GotoDate.ToString();
		public static readonly string OtherSettings = SchedulerMenuItemId.OtherSettings.ToString();
		public static readonly string CustomizeCurrentView = SchedulerMenuItemId.CustomizeCurrentView.ToString();
		public static readonly string CustomizeTimeRuler = SchedulerMenuItemId.CustomizeTimeRuler.ToString();
		public static readonly string StatusSubMenu = SchedulerMenuItemId.StatusSubMenu.ToString();
		public static readonly string LabelSubMenu = SchedulerMenuItemId.LabelSubMenu.ToString();
		public static readonly string RulerMenu = SchedulerMenuItemId.RulerMenu.ToString();
		public static readonly string AppointmentMenu = SchedulerMenuItemId.AppointmentMenu.ToString();
		public static readonly string DefaultMenu = SchedulerMenuItemId.DefaultMenu.ToString();
		public static readonly string RestoreOccurrence = SchedulerMenuItemId.RestoreOccurrence.ToString();
		public static readonly string SwitchViewMenu = SchedulerMenuItemId.SwitchViewMenu.ToString();
		public static readonly string SwitchToDayView = SchedulerMenuItemId.SwitchToDayView.ToString();
		public static readonly string SwitchToWorkWeekView = SchedulerMenuItemId.SwitchToWorkWeekView.ToString();
		public static readonly string SwitchToWeekView = SchedulerMenuItemId.SwitchToWeekView.ToString();
		public static readonly string SwitchToMonthView = SchedulerMenuItemId.SwitchToMonthView.ToString();
		public static readonly string SwitchToFullWeekView = SchedulerMenuItemId.SwitchToFullWeekView.ToString();
		public static readonly string SwitchToTimelineView = SchedulerMenuItemId.SwitchToTimelineView.ToString();
		public static readonly string TimeScaleEnable = SchedulerMenuItemId.TimeScaleEnable.ToString();
		public static readonly string TimeScaleVisible = SchedulerMenuItemId.TimeScaleVisible.ToString();
		public static readonly string SwitchTimeScale = SchedulerMenuItemId.SwitchTimeScale.ToString();
	}
}
namespace DevExpress.Xpf.Scheduler.Menu {
#if SL
	using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
	public class SchedulerPopupMenu : CustomizablePopupMenuBase {
		SchedulerControl scheduler;
		public SchedulerPopupMenu(SchedulerControl scheduler)
			: base(scheduler) {
			Guard.ArgumentNotNull(scheduler, "scheduler");
			this.scheduler = scheduler;
		}
		internal SchedulerMenuBuilderInfo MenuBuilderInfo { get; set; }
		internal SchedulerControl Scheduler { get { return scheduler; } }
		protected override MenuInfoBase CreateMenuInfo(UIElement placementTarget) {
			SchedulerMenuInfo info = new SchedulerMenuInfo(this);
			info.MenuBuilderInfo = MenuBuilderInfo;
			return info;
		}
		internal void AddMenuItem(BarItemLinkCollection links, BarItem item) {
			IDXMenuItemBase<SchedulerMenuItemId> schedulerItem = item as IDXMenuItem<SchedulerMenuItemId>;
			if (schedulerItem != null)
				AddItemCore(item, schedulerItem.BeginGroup, links);
			else
				AddItemCore(item, false, links);
		}
		protected override bool RaiseShowMenu() {
			SchedulerMenuEventArgs e = new SchedulerMenuEventArgs(this) { RoutedEvent = SchedulerControl.PopupMenuShowingEvent };
			Scheduler.RaiseShowMenu(e);
			return !e.Handled;
		}
		protected override void OnIsOpenChanged(DependencyPropertyChangedEventArgs e) {
			if (!IsOpen)
				PlacementTarget = null;
			base.OnIsOpenChanged(e);
		}
		public override void Destroy() {
			this.scheduler = null;
			base.Destroy();
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			System.Windows.Input.Mouse.OverrideCursor = null;
		}
	}
	public abstract class CommandMenuBuilderInfo<T> : IDXPopupMenu<T> where T : struct {
		#region Fields
		readonly List<IDXMenuItemBase<T>> items;
		string caption;
		T id;
		bool beginGroup;
		#endregion
		protected CommandMenuBuilderInfo() {
			this.items = new List<IDXMenuItemBase<T>>();
			Visible = true;
		}
		internal List<IDXMenuItemBase<T>> Items { get { return items; } }
		public BarManagerMenuController MenuController { get; set; }
		#region IDXPopupMenu<SchedulerMenuItemId> Members
		public string Caption { get { return caption; } set { caption = value; } }
		public T Id { get { return id; } set { id = value; } }
		public int ItemsCount { get { return Items.Count; } }
		public bool Visible { get; set; }
		public void AddItem(IDXMenuItemBase<T> item) {
			Items.Add(item);
		}
		#endregion
		#region IDXMenuItemBase<T> Members
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
	}
	public class SchedulerMenuBuilderInfo : CommandMenuBuilderInfo<SchedulerMenuItemId> {
	}
	public class SchedulerMenuInfo : MenuInfoBase {
		SchedulerMenuBuilderInfo menuBuilderInfo;
		public SchedulerMenuInfo(SchedulerPopupMenu menu) : base(menu) { }
		public SchedulerMenuBuilderInfo MenuBuilderInfo { get { return menuBuilderInfo; } set { menuBuilderInfo = value; } }
		public override bool CanCreateItems { get { return true; } }
		public new SchedulerPopupMenu Menu { get { return (SchedulerPopupMenu)base.Menu; } }
		public SchedulerControl SchedulerControl { get { return Menu.Scheduler; } }
		public override BarManagerMenuController MenuController {
			get {
				if (MenuBuilderInfo != null)
					return MenuBuilderInfo.MenuController;
				return null;
			}
		}
		protected override void CreateItems() {
			Menu.Name = MenuBuilderInfo.Id.ToString();
			AddMenuItems(Menu.ItemLinks, MenuBuilderInfo);
		}
		void AddMenuItems(BarItemLinkCollection menuLinks, SchedulerMenuBuilderInfo menuInfo) {
			foreach (IDXMenuItemBase<SchedulerMenuItemId> item in menuInfo.Items)
				AddMenuItem(menuLinks, item);
		}
		void AddMenuItem(BarItemLinkCollection menuLinks, IDXMenuItemBase<SchedulerMenuItemId> item) {
			SchedulerMenuBuilderInfo menuInfo = item as SchedulerMenuBuilderInfo;
			if (menuInfo == null) {
				BarItem barItem = item as BarItem;
				if (barItem == null)
					return;
				if (UpdateMenuItemUIState(barItem))
					Menu.AddMenuItem(menuLinks, barItem);
			} else
				AddSubMenu(menuLinks, menuInfo);
		}
		bool UpdateMenuItemUIState(BarItem barItem) {
			XpfICommandAdapter commandAdapter = barItem.Command as XpfICommandAdapter;
			if (commandAdapter != null)
				commandAdapter.UpdateUIState(barItem);
			else if (!barItem.Command.CanExecute(null))
				return false;
			return true;
		}
		void AddSubMenu(BarItemLinkCollection menuLinks, SchedulerMenuBuilderInfo menuInfo) {
			string caption = MenuItemHelper.ValidateMenuCaption(menuInfo.Caption);
			BarSubItem subItem = Menu.CreateBarSubItem(menuInfo.Id.ToString(), caption, menuInfo.BeginGroup, null, menuLinks);
			AddMenuItems(subItem.ItemLinks, menuInfo);
		}
	}
	public class XpfSchedulerMenuBuilderUIFactory : IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> {
		public IDXMenuItemCommandAdapter<SchedulerMenuItemId> CreateMenuItemAdapter(SchedulerCommand command) {
			return new SchedulerMenuItemCommandAdapter(command);
		}
		public IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> CreateMenuCheckItemAdapter(SchedulerCommand command) {
			return new SchedulerMenuCheckItemCommandAdapter(command);
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreatePopupMenu() {
			return new SchedulerMenuBuilderInfo();
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreateSubMenu() {
			return new SchedulerMenuBuilderInfo();
		}
	}
	public class SchedulerDefaultPopupMenuWpfBuilder : SchedulerDefaultPopupMenuBuilder {
		readonly SchedulerControl control;
		InnerSchedulerViewComparer viewComparer;
		public SchedulerDefaultPopupMenuWpfBuilder(IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, SchedulerControl control, ISchedulerHitInfo hitInfo)
			: base(control.InnerControl, uiFactory, hitInfo) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.viewComparer = new InnerSchedulerViewComparer();
		}
		protected internal SchedulerControl Control { get { return control; } }
		protected internal override IComparer<InnerSchedulerViewBase> ViewsComparer { get { return viewComparer; } }
		public override IDXPopupMenu<SchedulerMenuItemId> CreatePopupMenu() {
			IDXPopupMenu<SchedulerMenuItemId> popupMenu = base.CreatePopupMenu();
			SchedulerMenuBuilderInfo menuBuilderInfo = popupMenu as SchedulerMenuBuilderInfo;
			if (menuBuilderInfo != null) {
				switch (menuBuilderInfo.Id) {
					case SchedulerMenuItemId.AppointmentMenu:
						menuBuilderInfo.MenuController = Control.AppointmentMenuController;
						break;
					case SchedulerMenuItemId.DefaultMenu:
						menuBuilderInfo.MenuController = Control.DefaultMenuController;
						break;
					case SchedulerMenuItemId.RulerMenu:
						menuBuilderInfo.MenuController = Control.TimeRulerMenuController;
						break;
				}
			}
			return popupMenu;
		}
		protected override IDXMenuItem<SchedulerMenuItemId> AddMenuItem(IDXPopupMenu<SchedulerMenuItemId> menu, SchedulerCommand command) {
			return base.AddMenuItem(menu, command);
		}
		#region Commands Creation
		protected internal override SchedulerCommand CreateCustomizeTimeRulerCommand() {
			VisualTimeRuler ruler = (VisualTimeRuler)HitInfo.ViewInfo;
			return new CustomizeTimeRulerCommand(Control, ruler.GetTimeRuler());
		}
		protected internal override SchedulerCommand CreateSwitchTimeScaleCommand(InnerDayView view, TimeSlot slot) {
			return new XpfSwitchTimeScaleCommand(InnerControl, view, slot);
		}
		protected internal override SchedulerCommand CreateAppointmentDependencyCreatingOperationCommand() {
			return null;
		}
		#endregion
		protected internal override SchedulerCommand CreateChangeAppointmentLabelCommand(IAppointmentLabel label, int labelIndex) {
			return new ChangeAppointmentLabelCommand(this.control, (AppointmentLabel)label, labelIndex);
		}
		protected internal override SchedulerCommand CreateChangeAppointmentStatusCommand(IAppointmentStatus status, int statusIndex) {
			return new ChangeAppointmentStatusCommand(this.control, (AppointmentStatus)status, statusIndex);
		}
	}
	public class SchedulerBarCheckItem : BarCheckItem, IDXMenuCheckItem<SchedulerMenuItemId> {
		bool beginGroup;
		#region IDXMenuItemBase<SchedulerMenuItemId> Members
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
	}
	public class SchedulerBarItem : BarButtonItem, IDXMenuItem<SchedulerMenuItemId> {
		bool beginGroup;
		#region IDXMenuItemBase<SchedulerMenuItemId> Members
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
	}
	public class SchedulerMenuCommandAdapterBase {
		SchedulerCommand command;
		public SchedulerMenuCommandAdapterBase(SchedulerCommand command) {
			this.command = command;
		}
		public SchedulerCommand Command { get { return command; } }
		protected void InitializeItem(BarItem item) {
			item.Name = UniqueBarItemNameProvider.GetUniqueItemId(Command);
			item.Content = MenuItemHelper.ValidateMenuCaption(Command.MenuCaption);
			XpfICommandAdapter commandAdapter = MenuItemHelper.WrapCommand(Command);
			item.Command = commandAdapter;
			ImageSource image = MenuItemHelper.LoadImage(Command);
			item.IsPrivate = true;
			if (image != null)
				item.Glyph = image;
			commandAdapter.UpdateUIState(item);
		}
	}
	public class SchedulerMenuCheckItemCommandAdapter : SchedulerMenuCommandAdapterBase, IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> {
		public SchedulerMenuCheckItemCommandAdapter(SchedulerCommand command)
			: base(command) {
		}
		#region IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> Members
		public IDXMenuCheckItem<SchedulerMenuItemId> CreateMenuItem(string groupId) {
			SchedulerBarCheckItem item = new SchedulerBarCheckItem();
			InitializeItem(item);
			item.GroupIndex = groupId.GetHashCode();
			return item;
		}
		#endregion
	}
	public class SchedulerMenuItemCommandAdapter : SchedulerMenuCommandAdapterBase, IDXMenuItemCommandAdapter<SchedulerMenuItemId> {
		public SchedulerMenuItemCommandAdapter(SchedulerCommand command)
			: base(command) {
		}
		#region IDXMenuItemCommandAdapter<SchedulerMenuItemId> Members
		public IDXMenuItem<SchedulerMenuItemId> CreateMenuItem(DXMenuItemPriority priority) {
			SchedulerBarItem item = new SchedulerBarItem();
			InitializeItem(item);
			return item;
		}
		#endregion
	}
	public class XpfICommandAdapter : ICommand {
		Command command;
		public XpfICommandAdapter(Command command) {
			this.command = command;
		}
		public void UpdateUIState(BarItem item) {
			ICommandUIState menuItemState = new BarItemCommandUIState(item);
			this.command.UpdateUIState(menuItemState);
		}
		#region ICommand Members
		public bool CanExecute(object parameter) {
			return this.command.CanExecute();
		}
		public event EventHandler CanExecuteChanged;
		public void Execute(object parameter) {
			this.command.Execute();
		}
		protected void RaiseCanExecuteChanged() {
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	public class UniqueBarItemNameProvider {
		public static void Reset() {
		}
		public static string GetUniqueItemId(SchedulerCommand command) {
			string parameters = GenerateParametersFromCommand(command);
			return String.Format("{0}{1}", command.MenuId, parameters);
		}
		private static string GenerateParametersFromCommand(SchedulerCommand command) {
			if (String.IsNullOrEmpty(command.Parameters))
				return String.Empty;
			char[] result = new char[command.Parameters.Length];
			for (int i = 0; i < command.Parameters.Length; i++) {
				char ch = command.Parameters[i];
				if (Char.IsLetterOrDigit(ch))
					result[i] = ch;
				else
					result[i] = '_';
			}
			return new String(result);
		}
	}
	public static class MenuItemHelper {
		internal static string ValidateMenuCaption(string caption) {
			return caption.Replace("&", String.Empty);
		}
		public static XpfICommandAdapter WrapCommand(Command command) {
			return new XpfICommandAdapter(command);
		}
		public static ImageSource LoadImage(SchedulerCommand command) {
			ChangeAppointmentPropertyImageCommand imageCommand = command as ChangeAppointmentPropertyImageCommand;
			if (imageCommand != null)
				return imageCommand.Image;
			if (command.Image != null)
				return ImageHelper.CreatePlatformImage(command.Image).Source;
			return null;
		}
	}
}
