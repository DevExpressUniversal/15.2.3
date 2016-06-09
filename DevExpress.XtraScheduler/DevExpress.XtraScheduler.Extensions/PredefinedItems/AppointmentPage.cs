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
using System.Text;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraScheduler.Commands;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.XtraScheduler.UI {
	#region CalendarToolsRibbonPageCategory
	public class CalendarToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<SchedulerControl, SchedulerCommandId> {
		public CalendarToolsRibbonPageCategory() {
			Visible = false;
		}
		protected override SchedulerCommandId EmptyCommandId { get { return SchedulerCommandId.None; } }
		public override SchedulerCommandId CommandId {
			get {
				return SchedulerCommandId.ToolsAppointmentCommandGroup;
			}
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_PageCategoryCalendarTools); } }
	}
	#endregion
	#region AppointmentRibbonPage
	public class AppointmentRibbonPage : ControlCommandBasedRibbonPage {
		public AppointmentRibbonPage() {
		}
		public AppointmentRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_PageAppointment); } }
	}
	#endregion
	#region ActionsRibbonPageGroup
	public class ActionsRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public ActionsRibbonPageGroup() {
		}
		public ActionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupActions); } }
	}
	#endregion
	#region OptionsRibbonPageGroup
	public class OptionsRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public OptionsRibbonPageGroup() {
		}
		public OptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupOptions); }
		}
	}
	#endregion
	#region SchedulerActionsBarCreator
	public class SchedulerActionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(ActionsRibbonPageGroup); } }
		public override Type SupportedRibbonPageType { get { return typeof(AppointmentRibbonPage); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(CalendarToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ActionsBar); } }
		public override int DockColumn { get { return 0; } }
		public override int DockRow { get { return 2; } }
		public override XtraBars.Bar CreateBar() { return new ActionsBar(); }
		public override XtraBars.Commands.CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerActionsBarItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ActionsRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new AppointmentRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new CalendarToolsRibbonPageCategory();
		}
	}
	#endregion
	#region ActionsBar
	public class ActionsBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public ActionsBar() {
		}
		public ActionsBar(DevExpress.XtraBars.BarManager manager)
			: base(manager) {
		}
		public ActionsBar(DevExpress.XtraBars.BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupActions); } }
	}
	#endregion
	#region SchedulerActionsBarItemBuilder
	public class SchedulerActionsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			EditAppointmentQueryItem openAppointmentItem = new EditAppointmentQueryItem();
			openAppointmentItem.AddBarItem(new EditOccurrenceUICommandItem());
			openAppointmentItem.AddBarItem(new EditSeriesUICommandItem());
			items.Add(openAppointmentItem);
			DeleteAppointmentsItem deleteAppointmentItem = new DeleteAppointmentsItem();
			deleteAppointmentItem.AddBarItem(new DeleteOccurrenceItem());
			deleteAppointmentItem.AddBarItem(new DeleteSeriesItem());
			items.Add(deleteAppointmentItem);
			items.Add(new SplitAppointmentItem());
		}
	}
	#endregion
	#region EditAppointmentQueryItem
	public class EditAppointmentQueryItem : ControlCommandBarSubItem<SchedulerControl, SchedulerCommandId> {
		public EditAppointmentQueryItem() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		public EditAppointmentQueryItem(string caption)
			: base(caption) {
		}
		public EditAppointmentQueryItem(BarManager manager)
			: base(manager) {
		}
		public EditAppointmentQueryItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.EditAppointmentUI; } }
		protected override bool GetIsNeedOpenArrow(Command command, ICommandUIState state) {
			return !command.CanExecute() && state.Visible && state.Enabled;
		}
	}
	#endregion
	#region EditOccurrenceUICommandItem
	public class EditOccurrenceUICommandItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public EditOccurrenceUICommandItem() {
		}
		public EditOccurrenceUICommandItem(string caption)
			: base(caption) {
		}
		public EditOccurrenceUICommandItem(BarManager manager)
			: base(manager) {
		}
		public EditOccurrenceUICommandItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.EditOccurrenceUI; }
		}
	}
	#endregion
	#region EditSeriesUICommandItem
	public class EditSeriesUICommandItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public EditSeriesUICommandItem() {
		}
		public EditSeriesUICommandItem(string caption)
			: base(caption) {
		}
		public EditSeriesUICommandItem(BarManager manager)
			: base(manager) {
		}
		public EditSeriesUICommandItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.EditSeriesUI; }
		}
	}
	#endregion
	#region DeleteAppointmentsItem
	public class DeleteAppointmentsItem : ControlCommandBarSubItem<SchedulerControl, SchedulerCommandId> {
		public DeleteAppointmentsItem() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		public DeleteAppointmentsItem(string caption)
			: base(caption) {
		}
		public DeleteAppointmentsItem(BarManager manager)
			: base(manager) {
		}
		public DeleteAppointmentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.DeleteAppointmentsUI; } }
		protected override bool CanOpenMenu {
			get {
				Command command = CreateCommand();
				ICommandUIState state = CreateCommandUIState(command);
				command.UpdateUIState(state);
				return !command.CanExecute() && state.Visible && state.Enabled;
			}
		}
		protected override bool GetIsNeedOpenArrow(Command command, ICommandUIState state) {
			return !command.CanExecute() && state.Visible && state.Enabled;
		}
	}
	#endregion
	#region DeleteSeriesItem
	public class DeleteSeriesItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public DeleteSeriesItem() {
		}
		public DeleteSeriesItem(string caption)
			: base(caption) {
		}
		public DeleteSeriesItem(BarManager manager)
			: base(manager) {
		}
		public DeleteSeriesItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.DeleteSeriesUI; } }
	}
	#endregion
	#region DeleteOccurrenceItem
	public class DeleteOccurrenceItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public DeleteOccurrenceItem() {
		}
		public DeleteOccurrenceItem(string caption)
			: base(caption) {
		}
		public DeleteOccurrenceItem(BarManager manager)
			: base(manager) {
		}
		public DeleteOccurrenceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.DeleteOccurrenceUI; } }
	}
	#endregion
	#region SplitAppointmentItem
	public class SplitAppointmentItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public SplitAppointmentItem() {
		}
		public SplitAppointmentItem(string caption)
			: base(caption) {
		}
		public SplitAppointmentItem(BarManager manager)
			: base(manager) {
		}
		public SplitAppointmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SplitAppointment; } }
	}
	#endregion
	#region SchedulerOptionsBarCreator
	public class SchedulerOptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedBarType { get { return typeof(OptionsBar); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(OptionsRibbonPageGroup); } }
		public override Type SupportedRibbonPageType { get { return typeof(AppointmentRibbonPage); } }
		public override int DockColumn { get { return 1; } }
		public override int DockRow { get { return 2; } }
		public override Bar CreateBar() {
			return new OptionsBar();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new OptionsRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new AppointmentRibbonPage();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerOptionsBarItemBuilder();
		}
	}
	#endregion
	#region OptionsBar
	public class OptionsBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public OptionsBar() {
		}
		public OptionsBar(BarManager manager)
			: base(manager) {
		}
		public OptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupOptions); } }
	}
	#endregion
	#region SchedulerOptionsBarItemBuilder
	public class SchedulerOptionsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeAppointmentStatusItem());
			items.Add(new ChangeAppointmentLabelItem());
			items.Add(new ToggleRecurrenceItem());
			ChangeAppointmentReminderItem editReminderItem = new ChangeAppointmentReminderItem();
			items.Add(editReminderItem);
			if (!creationContext.IsRibbon) {
				editReminderItem.UseCommandCaption = true;
				editReminderItem.PaintStyle = BarItemPaintStyle.Caption;
			}
		}
	}
	#endregion
	#region ToggleRecurrenceItem
	public class ToggleRecurrenceItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public ToggleRecurrenceItem() {
		}
		public ToggleRecurrenceItem(string caption)
			: base(caption) {
		}
		public ToggleRecurrenceItem(BarManager manager)
			: base(manager) {
		}
		public ToggleRecurrenceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ToggleRecurrence; } }
	}
	#endregion
	#region ChangeAppointmentStatusItem
	public class ChangeAppointmentStatusItem : ColorablePopupMenuBasedItem {
		static readonly System.Drawing.Rectangle statusSmallGlyphColorRect = new System.Drawing.Rectangle(3, 3, 4, 10);
		static readonly System.Drawing.Rectangle statusLargeGlyphColorRect = new System.Drawing.Rectangle(5, 5, 7, 22);
		public ChangeAppointmentStatusItem() {
		}
		public ChangeAppointmentStatusItem(string caption)
			: base(caption) {
		}
		public ChangeAppointmentStatusItem(BarManager manager)
			: base(manager) {
		}
		public ChangeAppointmentStatusItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ChangeAppointmentStatusUI; } }
		protected override System.Drawing.Rectangle SmallGlyphColorRect { get { return statusSmallGlyphColorRect; } }
		protected override System.Drawing.Rectangle LargeGlyphColorRect { get { return statusLargeGlyphColorRect; } }
		protected override void PopulatePopupMenuCore(BarItemLinkCollection barItemLinkCollection) {
			if (SchedulerControl.DataStorage == null)
				return;
			AppointmentStatusCollection statuses = SchedulerControl.DataStorage.Appointments.Statuses;
			int count = statuses.Count;
			for (int i = 0; i < count; i++) {
				AppointmentStatus status = statuses.GetByIndex(i);
				barItemLinkCollection.Add(new AppointmentStatusMenuItem(SchedulerControl, status, i));
			}
		}
	}
	#endregion
	#region AppointmentStatusMenuItem
	public class AppointmentStatusMenuItem : SchedulerSimpleMenuItemBase {
		int statusId;
		AppointmentStatus status;
		public AppointmentStatusMenuItem(SchedulerControl scheduler, AppointmentStatus status, int statusId)
			: base(scheduler) {
			Guard.ArgumentNotNull(status, "status");
			this.statusId = statusId;
			this.status = status;
			UpdateUIState();
		}
		public override Image Glyph {
			get { return (CreateCommand() as ChangeAppointmentPropertyImageCommand).Image; }
			set { }
		}
		public override Image LargeGlyph {
			get { return (CreateCommand() as ChangeAppointmentPropertyImageCommand).LargeImage; }
			set { }
		}
		protected override SchedulerCommand CreateCommand() {
			return new ChangeAppointmentStatusCommand(Scheduler, this.status, this.statusId);
		}
	}
	#endregion
	#region ChangeAppointmentLabelItem
	public class ChangeAppointmentLabelItem : ColorablePopupMenuBasedItem {
		static readonly System.Drawing.Rectangle labelSmallGlyphColorRect = new System.Drawing.Rectangle(3, 3, 10, 10);
		static readonly System.Drawing.Rectangle labelLargeGlyphColorRect = new System.Drawing.Rectangle(5, 5, 22, 22);
		public ChangeAppointmentLabelItem() {
		}
		public ChangeAppointmentLabelItem(string caption)
			: base(caption) {
		}
		public ChangeAppointmentLabelItem(BarManager manager)
			: base(manager) {
		}
		public ChangeAppointmentLabelItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ChangeAppointmentLabelUI; } }
		protected override System.Drawing.Rectangle SmallGlyphColorRect { get { return labelSmallGlyphColorRect; } }
		protected override System.Drawing.Rectangle LargeGlyphColorRect { get { return labelLargeGlyphColorRect; } }
		protected override void PopulatePopupMenuCore(BarItemLinkCollection barItemLinkCollection) {
			if (SchedulerControl.DataStorage == null)
				return;
			AppointmentLabelCollection Labels = SchedulerControl.DataStorage.Appointments.Labels;
			int count = Labels.Count;
			for (int i = 0; i < count; i++) {
				AppointmentLabel Label = Labels.GetByIndex(i);
				barItemLinkCollection.Add(new AppointmentLabelMenuItem(SchedulerControl, Label, i));
			}
		}
	}
	#endregion
	#region AppointmentLabelMenuItem
	public class AppointmentLabelMenuItem : SchedulerSimpleMenuItemBase {
		object labelId;
		AppointmentLabel label;
		public AppointmentLabelMenuItem(SchedulerControl scheduler, AppointmentLabel label, object labelId)
			: base(scheduler) {
			Guard.ArgumentNotNull(label, "Label");
			this.labelId = labelId;
			this.label = label;
			UpdateUIState();
		}
		public override Image Glyph {
			get { return (CreateCommand() as ChangeAppointmentPropertyImageCommand).Image; }
			set { }
		}
		public override Image LargeGlyph {
			get { return (CreateCommand() as ChangeAppointmentPropertyImageCommand).LargeImage; }
			set { }
		}
		protected override SchedulerCommand CreateCommand() {
			return new ChangeAppointmentLabelCommand(Scheduler, this.label, this.labelId);
		}
	}
	#endregion
	#region ChangeAppointmentReminderItem
	public class ChangeAppointmentReminderItem : SchedulerCommandBarEditItem<TimeSpan> {
		const int DefaultReminderWidth = 100;
		public ChangeAppointmentReminderItem() {
			InitializeProperties();
		}
		public ChangeAppointmentReminderItem(string caption)
			: base(caption) {
			InitializeProperties();
		}
		public ChangeAppointmentReminderItem(BarManager manager)
			: base(manager) {
			InitializeProperties();
		}
		public ChangeAppointmentReminderItem(BarManager manager, string caption)
			: base(manager, caption) {
			InitializeProperties();
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ChangeAppointmentReminderUI; } }
		[DefaultValue(true)]
		public override bool UseCommandCaption { get { return base.UseCommandCaption; } set { base.UseCommandCaption = value; } }
		void InitializeProperties() {
			UseCommandCaption = true;
		}
		protected override int DefaultEditWidth {
			get {
				return DefaultReminderWidth;
			}
		}
		protected override XtraEditors.Repository.RepositoryItem CreateEdit() {
			RepositoryItemDuration edit = new RepositoryItemDuration();
			edit.ShowEmptyItem = true;
			edit.AllowNullInput = DefaultBoolean.False;
			edit.NullValuePromptShowForEmptyValue = true;
			edit.ValidateOnEnterKey = true;
			return edit;
		}
		protected override bool HandleException(Exception e) {
			OnControlUpdateUI(this, EventArgs.Empty);
			return true;
		}
	}
	#endregion
	#region ReminderStaticItem
	public class ReminderStaticItem : SchedulerBarStaticItem {
		public override SchedulerExtensionsStringId StringId { get { return SchedulerExtensionsStringId.Caption_Reminder; } }
	}
	#endregion
}
