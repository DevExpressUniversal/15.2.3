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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Commands;
using System.ComponentModel;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.Web.ASPxScheduler {
	public class ASPxSchedulerMenuItem : MenuItem, IDXMenuItem<SchedulerMenuItemId>, IDXMenuCheckItem<SchedulerMenuItemId> {
	}
	[ToolboxItem(false)]
	public class ASPxSchedulerPopupMenu : ASPxPopupMenu, IDXPopupMenu<SchedulerMenuItemId> {
		#region Fields
		string caption = String.Empty;
		SchedulerMenuItemId menuItemId;
		bool beginGroup;
		#endregion
		public ASPxSchedulerPopupMenu(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		#region IDXPopupMenu Members
		void IDXPopupMenu<SchedulerMenuItemId>.AddItem(IDXMenuItemBase<SchedulerMenuItemId> item) {
			ASPxSchedulerPopupMenu popupMenu = item as ASPxSchedulerPopupMenu;
			if (popupMenu == null)
				this.Items.Add((MenuItem)item);
			else {
				MenuItem menuItem = new MenuItem(ASPxMenuItemHelper.ValidateMenuCaption(popupMenu.Caption), popupMenu.MenuId.ToString());
				menuItem.Items.Assign(((ASPxPopupMenu)popupMenu).Items);
				menuItem.BeginGroup = popupMenu.BeginGroup;
				this.Items.Add(menuItem);
			}
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPopupMenuCaption")]
#endif
		public string Caption { get { return caption; } set { caption = value; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPopupMenuItemsCount")]
#endif
		public int ItemsCount { get { return this.Items.Count; } }
		[CLSCompliant(false), Obsolete("Use the MenuId property instead.", false)]
		public SchedulerMenuItemId Id { get { return MenuId; } set { MenuId = value; } }
		public SchedulerMenuItemId MenuId { get { return menuItemId; } set { menuItemId = value; } }
		SchedulerMenuItemId IDXPopupMenu<SchedulerMenuItemId>.Id { get { return MenuId; } set { MenuId = value; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPopupMenuEnableViewState")]
#endif
		public override bool EnableViewState { get { return false; } }
		#endregion
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		#region ISchedulerMenuItemBase Members
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("ASPxSchedulerPopupMenuBeginGroup")]
#endif
		public bool BeginGroup { get { return beginGroup; } set { beginGroup = value; } }
		#endregion
		#region CreateStyles
		protected override StylesBase CreateStyles() {
			return new SchedulerMenuStyles(this);
		}
		#endregion
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class ASPxMenuItemInfo {
		string name;
		string parameters;
		public ASPxMenuItemInfo(string name, string parameters) {
			this.name = name;
			this.parameters = parameters;
		}
		public string Name { get { return name; } }
		public string Parameters { get { return parameters; } }
	}
	public static class ASPxMenuItemHelper {
		internal static string GenerateMenuItemName(SchedulerCommand command) {
			if (String.IsNullOrEmpty(command.Parameters))
				return command.MenuId.ToString();
			else
				return String.Format("{0}!{1}", command.MenuId, command.Parameters);
		}
		internal static ASPxMenuItemInfo ParseMenuItemName(string name) {
			string[] result = name.Split('!');
			int count = result.Length;
			if (count <= 0)
				Exceptions.ThrowInternalException();
			string id = result[0];
			string parameters = String.Empty;
			for (int i = 1; i < count; i++)
				parameters += result[i];
			return new ASPxMenuItemInfo(id, parameters);
		}
		internal static string GenerateMenuCaption(SchedulerCommand command) {
			return ValidateMenuCaption(command.MenuCaption);
		}
		internal static string ValidateMenuCaption(string caption) {
			return caption.Replace("&", String.Empty);
		}
	}
	#region ASPxSchedulerMenuItemCommandAdapter
	public class ASPxSchedulerMenuItemCommandAdapter : IDXMenuItemCommandAdapter<SchedulerMenuItemId> {
		SchedulerCommand command;
		public ASPxSchedulerMenuItemCommandAdapter(SchedulerCommand command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		internal SchedulerCommand Command { get { return command; } }
		public virtual IDXMenuItem<SchedulerMenuItemId> CreateMenuItem(DXMenuItemPriority priority) {
			ASPxSchedulerMenuItem item = new ASPxSchedulerMenuItem();
			item.Text = ASPxMenuItemHelper.GenerateMenuCaption(command);
			item.Name = ASPxMenuItemHelper.GenerateMenuItemName(command);
			if (!String.IsNullOrEmpty(command.ImageName)) {
				ASPxScheduler control = (ASPxScheduler)command.InnerControl.Owner;
				ImageProperties image = control.Images.Menu.GetImageProperties(control.Page, command.ImageName);
				if (image != null) {
					item.Image.Assign(image);
				}
			}
			return item;
		}
	}
	#endregion
	#region ASPxSchedulerMenuCheckItemCommandAdapter
	public class ASPxSchedulerMenuCheckItemCommandAdapter : IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> {
		SchedulerCommand command;
		public ASPxSchedulerMenuCheckItemCommandAdapter(SchedulerCommand command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		internal SchedulerCommand Command { get { return command; } }
		public virtual IDXMenuCheckItem<SchedulerMenuItemId> CreateMenuItem(string groupId) {
			ASPxSchedulerMenuItem item = new ASPxSchedulerMenuItem();
			item.Text = ASPxMenuItemHelper.GenerateMenuCaption(command);
			item.Name = ASPxMenuItemHelper.GenerateMenuItemName(command);
			item.GroupName = groupId;
			return item;
		}
	}
	#endregion
	public class ASPxSchedulerFakeCommand : SchedulerCommand {
		SchedulerStringId stringId;
		SchedulerMenuItemId menuItemId;
		string caption;
		string imageName = String.Empty;
		public ASPxSchedulerFakeCommand(InnerSchedulerControl control, SchedulerMenuItemId menuItemId, SchedulerStringId stringId)
			: this(control, menuItemId, stringId, String.Empty) {
		}
		public ASPxSchedulerFakeCommand(InnerSchedulerControl control, SchedulerMenuItemId menuItemId, SchedulerStringId stringId, string imageName)
			: base(control) {
			this.menuItemId = menuItemId;
			this.stringId = stringId;
			this.imageName = imageName;
		}
		public ASPxSchedulerFakeCommand(InnerSchedulerControl control, SchedulerMenuItemId menuItemId, string caption)
			: base(control) {
			this.menuItemId = menuItemId;
			this.caption = caption;
		}
		public override SchedulerMenuItemId MenuId { get { return menuItemId; } }
		public override SchedulerStringId MenuCaptionStringId { get { return stringId; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string Description { get { return String.Empty; } }
		public override string MenuCaption { get { return String.IsNullOrEmpty(caption) ? SchedulerLocalizer.GetString(MenuCaptionStringId) : caption; } }
		public override string ImageName { get { return imageName; } }
		public override void ForceExecute(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	public class ASPxSchedulerMenuBuilderUIFactory : IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> {
		readonly ASPxScheduler control;
		public ASPxSchedulerMenuBuilderUIFactory(ASPxScheduler control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public IDXMenuItemCommandAdapter<SchedulerMenuItemId> CreateMenuItemAdapter(SchedulerCommand command) {
			return new ASPxSchedulerMenuItemCommandAdapter(command);
		}
		public IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> CreateMenuCheckItemAdapter(SchedulerCommand command) {
			return new ASPxSchedulerMenuCheckItemCommandAdapter(command);
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreatePopupMenu() {
			ASPxSchedulerPopupMenu menu = new ASPxSchedulerPopupMenu(control);
			menu.ParentSkinOwner = control;
			return menu;
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreateSubMenu() {
			return CreatePopupMenu();
		}
	}
	#region ASPxSchedulerDefaultPopupMenuBuilder
	public class ASPxSchedulerDefaultPopupMenuBuilder : SchedulerDefaultPopupMenuBuilder {
		class WebSchedulerHitInfoWrapper : ISchedulerHitInfo {
			readonly SchedulerHitTest hitTest;
			public WebSchedulerHitInfoWrapper(SchedulerHitTest hitTest) {
				this.hitTest = hitTest;
			}
			#region ISchedulerHitInfo Members
			public SchedulerHitTest HitTest { get { return hitTest; } }
			public ISchedulerHitInfo NextHitInfo { get { return null; } }
			public ISelectableIntervalViewInfo ViewInfo { get { return null; } }
			public Point HitPoint { get { return Point.Empty; } }
			public bool Contains(SchedulerHitTest types) {
				return (hitTest & types) != 0;
			}
			public ISchedulerHitInfo FindFirstLayoutHitInfo() {
				return null;
			}
			public ISchedulerHitInfo FindHitInfo(SchedulerHitTest types) {
				return null;
			}
			public ISchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
				return null;
			}
			#endregion
		}
		ASPxScheduler control;
		public ASPxSchedulerDefaultPopupMenuBuilder(IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, ASPxScheduler control, SchedulerHitTest hitTest)
			: base(control.InnerControl, uiFactory, new WebSchedulerHitInfoWrapper(hitTest)) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			this.control = control;
		}
		protected internal ASPxScheduler Control { get { return control; } }
		protected internal override void PopulateTimeRulerPopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			PopulateDefaultPopupMenu(menu);
			AppendTimeSlotsMenuItems(menu, (InnerDayView)InnerControl.ActiveView);
		}
		#region Commands Creation
		protected internal override SchedulerCommand CreateEditAppointmentCommand() {
			return new WebEditAppointmentCommand(Control);
		}
		protected internal override SchedulerCommand CreateEditRecurrencePatternCommand() {
			return new EditRecurrencePatternCommand(InnerControl);
		}
		protected internal override SchedulerCommand CreateDeleteAppointmentsCommand() {
			return new WebDeleteAppointmentsCommand(Control);
		}
		protected internal override SchedulerCommand CreateGotoThisDayCommand() {
			return new GotoThisDayCommand(InnerControl, DateTime.MinValue);
		}
		protected internal override SchedulerCommand CreateGotoDateCommand() {
			return new WebGotoDateCommand(Control);
		}
		protected internal override SchedulerCommand CreateCustomizeTimeRulerCommand() {
			return new ASPxSchedulerFakeCommand(InnerControl, SchedulerMenuItemId.CustomizeTimeRuler, SchedulerStringId.MenuCmd_CustomizeTimeRuler);
		}
		protected internal override SchedulerCommand CreateNewAppointmentCommand() {
			return new WebNewAppointmentCommand(Control);
		}
		protected internal override SchedulerCommand CreateNewAllDayAppointmentCommand() {
			return new NewAllDayAppointmentCommand(InnerControl);
		}
		protected internal override SchedulerCommand CreateNewRecurringAppointmentCommand() {
			return new WebNewRecurringAppointmentCommand(Control);
		}
		protected internal override SchedulerCommand CreateNewRecurringAllDayAppointmentCommand() {
			return new NewRecurringAllDayAppointmentCommand(InnerControl);
		}
		protected internal override SchedulerCommand CreateAppointmentDependencyCreatingOperationCommand() {
			return null;
		}
		protected internal override SchedulerCommand CreateChangeAppointmentStatusCommand(IAppointmentStatus status, int statusIndex) {
			return new ChangeAppointmentStatusCommand(Control, (AppointmentStatus)status, statusIndex);
		}
		protected internal override SchedulerCommand CreateChangeAppointmentLabelCommand(IAppointmentLabel label, int labelIndex) {
			return new ChangeAppointmentLabelCommand(Control, (AppointmentLabel)label, labelIndex);
		}
		#endregion
	}
	#endregion
}
