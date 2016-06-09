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
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Commands;
using System.Drawing;
namespace DevExpress.XtraScheduler.UI {
	#region ViewRibbonPage
	public class ViewRibbonPage : ControlCommandBasedRibbonPage {
		public ViewRibbonPage() {
		}
		public ViewRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_PageView); } }
	}
	#endregion
	#region ActiveViewRibbonPageGroup
	public class ActiveViewRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public ActiveViewRibbonPageGroup() {
		}
		public ActiveViewRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupActiveView); } }
	}
	#endregion
	#region SchedulerActiveViewBarCreator
	public class SchedulerActiveViewBarCreator : ControlCommandBarCreator {
		bool skipBarCreation;
		public SchedulerActiveViewBarCreator(bool skipBarCreation) {
			this.skipBarCreation = skipBarCreation;
		}
		public override Type SupportedRibbonPageGroupType { get { return typeof(ActiveViewRibbonPageGroup); } }
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(ActiveViewBar); } }
		public override int DockColumn { get { return 0; } }
		public override int DockRow { get { return 1; } }
		public bool SkipBarCreation { get { return skipBarCreation; } }
		public override Bar CreateBar() {
			if (SkipBarCreation)
				return null;
			return new ActiveViewBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerActiveViewItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ActiveViewRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
	}
	#endregion
	#region ActiveViewBar
	public class ActiveViewBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public ActiveViewBar() {
		}
		public ActiveViewBar(DevExpress.XtraBars.BarManager manager)
			: base(manager) {
		}
		public ActiveViewBar(DevExpress.XtraBars.BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupActiveView); } }
	}
	#endregion
	#region SchedulerActiveViewItemBuilder
	public class SchedulerActiveViewItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new SwitchToDayViewItem());
			items.Add(new SwitchToWorkWeekViewItem());
			items.Add(new SwitchToWeekViewItem());
			items.Add(new SwitchToFullWeekViewItem());
			items.Add(new SwitchToMonthViewItem());
			items.Add(new SwitchToTimelineViewItem());
			items.Add(new SwitchToGanttViewItem());
		}
	}
	#endregion
	#region TimeScaleRibbonPageGroup
	public class TimeScaleRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public TimeScaleRibbonPageGroup() {
		}
		public TimeScaleRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupTimeScale); } }
	}
	#endregion
	#region SchedulerTimeScaleBarCreator
	public class SchedulerTimeScaleBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(TimeScaleRibbonPageGroup); } }
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(TimeScaleBar); } }
		public override int DockColumn { get { return 0; } }
		public override int DockRow { get { return 1; } }
		public override Bar CreateBar() {
			return new TimeScaleBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerTimeScaleItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TimeScaleRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
	}
	#endregion
	#region TimeScaleBar
	public class TimeScaleBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public TimeScaleBar() {
		}
		public TimeScaleBar(DevExpress.XtraBars.BarManager manager)
			: base(manager) {
		}
		public TimeScaleBar(DevExpress.XtraBars.BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupTimeScale); } }
	}
	#endregion
	#region SchedulerTimeScaleItemBuilder
	public class SchedulerTimeScaleItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new SwitchTimeScalesItem());
			ChangeScaleWidthItem scaleWidthItem = new ChangeScaleWidthItem();
			if (!creationContext.IsRibbon)
				scaleWidthItem.UseCommandCaption = true;
			items.Add(scaleWidthItem);
			items.Add(new SwitchTimeScalesCaptionItem());
		}
	}
	#endregion
	#region LayoutRibbonPageGroup
	public class LayoutRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public LayoutRibbonPageGroup() {
		}
		public LayoutRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupLayout); } }
	}
	#endregion
	#region SchedulerLayoutBarCreator
	public class SchedulerLayoutBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(LayoutRibbonPageGroup); } }
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(LayoutBar); } }
		public override int DockColumn { get { return 1; } }
		public override int DockRow { get { return 1; } }
		public override Bar CreateBar() {
			return new LayoutBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerLayoutItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new LayoutRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
	}
	#endregion
	#region LayoutBar
	public class LayoutBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public LayoutBar() {
		}
		public LayoutBar(DevExpress.XtraBars.BarManager manager)
			: base(manager) {
		}
		public LayoutBar(DevExpress.XtraBars.BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupLayout); } }
	}
	#endregion
	#region SchedulerLayoutItemBuilder
	public class SchedulerLayoutItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new SwitchCompressWeekendItem());
			items.Add(new SwitchShowWorkTimeOnlyItem());
			items.Add(new SwitchCellsAutoHeightItem());
			items.Add(new ChangeSnapToCellsUIItem());
		}
	}
	#endregion
	#region PopupMenuBasedItem
	public abstract class PopupMenuBasedItem : SchedulerCommandBarButtonItem {
		PopupMenu popupMenu = new PopupMenu();
		protected PopupMenuBasedItem() {
		}
		protected PopupMenuBasedItem(string caption)
			: base(caption) {
		}
		protected PopupMenuBasedItem(BarManager manager)
			: base(manager) {
		}
		protected PopupMenuBasedItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return popupMenu; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsDropDown { get { return true; } set { } }
		#endregion
		protected void RefreshPopupMenu() {
			DeletePopupItems();
			PopulatePopupMenu();
		}
		void DeletePopupItems() {
			if (this.popupMenu == null)
				return;
			BarItemLinkCollection itemLinks = this.popupMenu.ItemLinks;
			this.popupMenu.BeginUpdate();
			try {
				while (itemLinks.Count > 0)
					itemLinks[0].Item.Dispose();
			} finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected void PopulatePopupMenu() {
			if (this.popupMenu == null)
				return;
			if (SchedulerControl == null)
				return;
			this.popupMenu.BeginUpdate();
			try {
				PopulatePopupMenuCore(this.popupMenu.ItemLinks);
			} finally {
				this.popupMenu.EndUpdate();
			}
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup += OnBeforePopup;
		}
		protected override void UnsubscribeControlEvents() {
			if (this.popupMenu != null)
				this.popupMenu.BeforePopup -= OnBeforePopup;
			base.UnsubscribeControlEvents();
		}
		protected virtual void OnBeforePopup(object sender, CancelEventArgs e) {
			RefreshPopupMenu();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (this.popupMenu != null)
				this.popupMenu.Manager = this.Manager;
		}
		protected abstract void PopulatePopupMenuCore(BarItemLinkCollection barItemLinkCollection);
	}
	#endregion
	#region SwitchTimeScalesCaptionItem
	public class SwitchTimeScalesCaptionItem : PopupMenuBasedItem {
		public SwitchTimeScalesCaptionItem() {
		}
		public SwitchTimeScalesCaptionItem(string caption)
			: base(caption) {
		}
		public SwitchTimeScalesCaptionItem(BarManager manager)
			: base(manager) {
		}
		public SwitchTimeScalesCaptionItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchTimeScalesCaptionUICommand; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithText; } }
		protected override void PopulatePopupMenuCore(BarItemLinkCollection barItemLinkCollection) {
			Command command = CreateCommand();
			if (!command.CanExecute())
				return;
			TimelineView view = SchedulerControl.ActiveView as TimelineView;
			if (view == null)
				return;
			TimeScaleCollection scales = view.Scales;
			foreach (TimeScale scale in scales)
				barItemLinkCollection.Add(new SetTimeScaleCaptionVisibilityMenuItem(SchedulerControl, scale));
		}
	}
	#endregion
	#region SwitchTimeScalesItem
	public class SwitchTimeScalesItem : PopupMenuBasedItem {
		public SwitchTimeScalesItem() {
		}
		public SwitchTimeScalesItem(string caption)
			: base(caption) {
		}
		public SwitchTimeScalesItem(BarManager manager)
			: base(manager) {
		}
		public SwitchTimeScalesItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchTimeScalesUICommand; } }
		protected override void PopulatePopupMenuCore(BarItemLinkCollection links) {
			SchedulerViewType viewType = SchedulerControl.ActiveViewType;
			if (viewType == SchedulerViewType.Timeline || viewType == SchedulerViewType.Gantt)
				PopulatePopupMenuFromTimelineScales(links);
			else if (viewType == SchedulerViewType.Day || viewType == SchedulerViewType.WorkWeek || viewType == SchedulerViewType.FullWeek)
				PopulatePopupMenuFromDayViewScales(links);
		}
		void PopulatePopupMenuFromDayViewScales(BarItemLinkCollection links) {
			DayView view = SchedulerControl.ActiveView as DayView;
			if (view == null)
				return;
			TimeSlotCollection slots = view.TimeSlots;
			if (slots == null)
				return;
			foreach (TimeSlot timeSlot in slots) {
				SetDayViewTimeScaleMenuItem item = new SetDayViewTimeScaleMenuItem(SchedulerControl, timeSlot);
				links.Add(item);
			}
		}
		void PopulatePopupMenuFromTimelineScales(BarItemLinkCollection links) {
			TimelineView view = SchedulerControl.ActiveView as TimelineView;
			if (view == null)
				return;
			IList<TimeScale> scales = view.Scales;
			if (scales == null)
				return;
			foreach (TimeScale timeScale in scales) {
				SetTimelineTimeScaleMenuItem item = new SetTimelineTimeScaleMenuItem(SchedulerControl, timeScale);
				links.Add(item);
			}
		}
	}
	#endregion
	#region SchedulerSimpleMenuItemBase
	public abstract class SchedulerSimpleMenuItemBase : BarCheckItem {
		SchedulerControl scheduler;
		protected SchedulerSimpleMenuItemBase(SchedulerControl scheduler) {
			Guard.ArgumentNotNull(scheduler, "scheduler");
			this.scheduler = scheduler;
		}
		public override string Caption {
			get {
				SchedulerCommand command = CreateCommand();
				return command.MenuCaption;
			}
			set { }
		}
		public SchedulerControl Scheduler { get { return scheduler; } }
		protected override void OnClick(BarItemLink link) {
			SchedulerCommand command = CreateCommand();
			command.Execute();
			UpdateUIState();
		}
		protected void UpdateUIState() {
			SchedulerCommand command = CreateCommand();
			BarCheckItemUIState state = new BarCheckItemUIState(this);
			command.UpdateUIState(state);
		}
		protected abstract SchedulerCommand CreateCommand();
	}
	#endregion
	#region SetDayViewTimeScaleMenuItem
	public class SetDayViewTimeScaleMenuItem : SchedulerSimpleMenuItemBase {
		TimeSlot slot;
		public SetDayViewTimeScaleMenuItem(SchedulerControl scheduler, TimeSlot slot)
			: base(scheduler) {
			Guard.ArgumentNotNull(slot, "slot");
			this.slot = slot;
			UpdateUIState();
		}
		protected override SchedulerCommand CreateCommand() {
			InnerDayView dayView = Scheduler.InnerControl.ActiveView as InnerDayView;
			if (dayView == null)
				return null;
			SwitchTimeScaleCommand command = new SwitchTimeScaleCommand(Scheduler.InnerControl, dayView, this.slot);
			return command;
		}
	}
	#endregion
	#region SetTimelineTimeScaleMenuItem
	public class SetTimelineTimeScaleMenuItem : SchedulerSimpleMenuItemBase {
		TimeScale scale;
		public SetTimelineTimeScaleMenuItem(SchedulerControl scheduler, TimeScale scale)
			: base(scheduler) {
			Guard.ArgumentNotNull(scale, "scale");
			this.scale = scale;
			UpdateUIState();
		}
		protected TimeScale Scale { get { return scale; } }
		protected override SchedulerCommand CreateCommand() {
			return new TimeScaleEnableCommand(Scheduler.InnerControl, Scale);
		}
	}
	#endregion
	#region SetTimeScaleCaptionVisibilityMenuItem
	public class SetTimeScaleCaptionVisibilityMenuItem : SetTimelineTimeScaleMenuItem {
		public SetTimeScaleCaptionVisibilityMenuItem(SchedulerControl scheduler, TimeScale scale)
			: base(scheduler, scale) {
		}
		protected override SchedulerCommand CreateCommand() {
			return new TimeScaleVisibleCommand(Scheduler.InnerControl, Scale);
		}
	}
	#endregion
	#region SwitchShowWorkTimeOnlyItem
	public class SwitchShowWorkTimeOnlyItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchShowWorkTimeOnlyItem() {
		}
		public SwitchShowWorkTimeOnlyItem(string caption)
			: base(caption) {
		}
		public SwitchShowWorkTimeOnlyItem(BarManager manager)
			: base(manager) {
		}
		public SwitchShowWorkTimeOnlyItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchShowWorkTimeOnly; } }
	}
	#endregion
	#region SwitchCompressWeekendItem
	public class SwitchCompressWeekendItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchCompressWeekendItem() {
		}
		public SwitchCompressWeekendItem(string caption)
			: base(caption) {
		}
		public SwitchCompressWeekendItem(BarManager manager)
			: base(manager) {
		}
		public SwitchCompressWeekendItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchCompressWeekend; } }
	}
	#endregion
	#region SwitchCellsAutoHeightItem
	public class SwitchCellsAutoHeightItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchCellsAutoHeightItem() {
		}
		public SwitchCellsAutoHeightItem(string caption)
			: base(caption) {
		}
		public SwitchCellsAutoHeightItem(BarManager manager)
			: base(manager) {
		}
		public SwitchCellsAutoHeightItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchCellsAutoHeight; } }
	}
	#endregion
	#region ChangeScaleWidthItem
	public class ChangeScaleWidthItem : SchedulerCommandBarEditItem<int> {
		public ChangeScaleWidthItem() {
			InitializeItem();
		}
		public ChangeScaleWidthItem(string caption)
			: base(caption) {
			InitializeItem();
		}
		public ChangeScaleWidthItem(BarManager manager)
			: base(manager) {
			InitializeItem();
		}
		public ChangeScaleWidthItem(BarManager manager, string caption)
			: base(manager, caption) {
			InitializeItem();
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ChangeTimelineScaleWidth; } }
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				base.EditValue = Convert.ToInt32(value);
			}
		}
		protected virtual void InitializeItem() {
			UseCommandCaption = true;
		}
		protected override XtraEditors.Repository.RepositoryItem CreateEdit() {
			RepositoryItemSpinEdit edit = new RepositoryItemSpinEdit();
			edit.MinValue = ChangeTimelineScaleWidthUICommand.MinWidth;
			edit.MaxValue = ChangeTimelineScaleWidthUICommand.MaxWidth;
			edit.EditValueChangedFiringMode = XtraEditors.Controls.EditValueChangedFiringMode.Default;
			return edit;
		}
#if DEBUGTEST
		internal XtraEditors.Repository.RepositoryItem CreateEditInternal() {
			return CreateEdit();
		}
#endif
	}
	#endregion
	#region ChangeSnapToCellsUIItem
	public class ChangeSnapToCellsUIItem : PopupMenuBasedItem {
		public ChangeSnapToCellsUIItem() {
		}
		public ChangeSnapToCellsUIItem(string caption)
			: base(caption) {
		}
		public ChangeSnapToCellsUIItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSnapToCellsUIItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.ChangeSnapToCellsUI; } }
		protected override void PopulatePopupMenuCore(BarItemLinkCollection barItemLinkCollection) {
			barItemLinkCollection.Add(new SnapToCellsMenuItem(SchedulerControl, AppointmentSnapToCellsMode.Always));
			barItemLinkCollection.Add(new SnapToCellsMenuItem(SchedulerControl, AppointmentSnapToCellsMode.Auto));
			barItemLinkCollection.Add(new SnapToCellsMenuItem(SchedulerControl, AppointmentSnapToCellsMode.Disabled));
			barItemLinkCollection.Add(new SnapToCellsMenuItem(SchedulerControl, AppointmentSnapToCellsMode.Never));
		}
	}
	#endregion
	#region SnapToCellsMenuItem
	public class SnapToCellsMenuItem : SchedulerSimpleMenuItemBase {
		AppointmentSnapToCellsMode snapToCells;
		public SnapToCellsMenuItem(SchedulerControl scheduler, AppointmentSnapToCellsMode snapToCells)
			: base(scheduler) {
			Guard.ArgumentNotNull(snapToCells, "snapToCells");
			this.snapToCells = snapToCells;
			UpdateUIState();
		}
		protected override SchedulerCommand CreateCommand() {
			return new SetSnapToCellsCommand(Scheduler.InnerControl, this.snapToCells);
		}
	}
	#endregion
	#region ColorablePopupMenuBasedItem (abstract class)
	public abstract class ColorablePopupMenuBasedItem : PopupMenuBasedItem {
		#region Fields
		Brush brush;
		readonly Rectangle smallGlyphColorRect = new Rectangle(1, 13, 14, 1);
		readonly Rectangle largeGlyphColorRect = new Rectangle(4, 24, 25, 5);
		#endregion
		protected ColorablePopupMenuBasedItem() {
		}
		protected ColorablePopupMenuBasedItem(BarManager manager)
			: base(manager) {
		}
		protected ColorablePopupMenuBasedItem(string caption)
			: base(caption) {
		}
		protected ColorablePopupMenuBasedItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		protected Brush Brush { get { return brush; } set { brush = value; } }
		protected virtual Rectangle SmallGlyphColorRect { get { return smallGlyphColorRect; } }
		protected virtual Rectangle LargeGlyphColorRect { get { return largeGlyphColorRect; } }
		#endregion
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			Command command = CreateCommand();
			if (command == null)
				return;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			UserInterfaceObjectWin uiObject = state.EditValue as UserInterfaceObjectWin;
			if (uiObject == null)
				return;
			Brush = uiObject.Brush;
			DrawColorRectangle();
		}
		protected override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			DrawColorRectangle();
		}
		protected virtual void DrawColorRectangle() {
			if (IsImageExist)
				DrawColorRectangle(Glyph, SmallGlyphColorRect);
			if (IsLargeImageExist)
				DrawColorRectangle(LargeGlyph, LargeGlyphColorRect);
		}
		protected virtual void DrawColorRectangle(Image image, Rectangle rect) {
			Brush fillBrush = Brush != null ? Brush : Brushes.Black;
			DrawColorRectangleCore(image, rect, fillBrush);
		}
		protected virtual void DrawColorRectangleCore(Image image, Rectangle rect, Brush fillBrush) {
			if (Control == null)
				return;
			using (Graphics gr = Graphics.FromImage(image)) {
				lock (fillBrush)
					gr.FillRectangle(fillBrush, rect);
			}
		}
	}
	#endregion
}
