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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraScheduler.Commands.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region HomeRibbonPage
	public class HomeRibbonPage : ControlCommandBasedRibbonPage {
		public HomeRibbonPage() {
		}
		public HomeRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_PageHome); } }
	}
	#endregion    
	#region ArrangeRibbonPageGroup
	public class ArrangeRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public ArrangeRibbonPageGroup() {
		}
		public ArrangeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { 
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupArrange); } 
		}
	}
	#endregion
	#region NavigatorRibbonPageGroup
	public class NavigatorRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public NavigatorRibbonPageGroup() {
		}
		protected NavigatorRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupNavigator); }
		}
	}
	#endregion
	#region GroupByRibbonPageGroup
	public class GroupByRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public GroupByRibbonPageGroup() {
		}
		public GroupByRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupGroupBy); }
		}
	}
	#endregion
	#region AppointmentRibbonPageGroup
	public class AppointmentRibbonPageGroup : SchedulerControlRibbonPageGroup {		
		public AppointmentRibbonPageGroup() {
		}
		public AppointmentRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupAppointment); }
		}
	}
	#endregion
	#region SchedulerArrangeBarCreator
	public class SchedulerArrangeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ArrangeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ArrangeBar); } }
		public override int DockColumn { get { return 4; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new ArrangeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerArrangeItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ArrangeRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region ArrangeBar
	public class ArrangeBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public ArrangeBar()
			: base() {
		}
		public ArrangeBar(BarManager manager)
			: base(manager) {
		}
		public ArrangeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupArrange); } }
	}
	#endregion
	#region SchedulerArrangeItemBuilder
	public class SchedulerArrangeItemBuilder : CommandBasedBarItemBuilder {
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
	#region SwitchToDayViewItem
	public class SwitchToDayViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToDayViewItem() {
		}
		public SwitchToDayViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToDayViewItem(string caption)
			: base(caption) {
		}
		public SwitchToDayViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToDayView; } }
	}
	#endregion
	#region SwitchToWorkWeekViewItem
	public class SwitchToWorkWeekViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToWorkWeekViewItem() {
		}
		public SwitchToWorkWeekViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToWorkWeekViewItem(string caption)
			: base(caption) {
		}
		public SwitchToWorkWeekViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToWorkWeekView; } }
	}
	#endregion
	#region SwitchToWeekViewItem
	public class SwitchToWeekViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToWeekViewItem() {
		}
		public SwitchToWeekViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToWeekViewItem(string caption)
			: base(caption) {
		}
		public SwitchToWeekViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToWeekView; } }
	}
	#endregion
	#region SwitchToFullWeekViewItem
	public class SwitchToFullWeekViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToFullWeekViewItem() {
		}
		public SwitchToFullWeekViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToFullWeekViewItem(string caption)
			: base(caption) {
		}
		public SwitchToFullWeekViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToFullWeekView; } }
	}
	#endregion
	#region SwitchToMonthViewItem
	public class SwitchToMonthViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToMonthViewItem() {
		}
		public SwitchToMonthViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToMonthViewItem(string caption)
			: base(caption) {
		}
		public SwitchToMonthViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToMonthView; } }
	}
	#endregion
	#region SwitchToTimelineViewItem
	public class SwitchToTimelineViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToTimelineViewItem() {
		}
		public SwitchToTimelineViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToTimelineViewItem(string caption)
			: base(caption) {
		}
		public SwitchToTimelineViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToTimelineView; } }
	}
	#endregion
	#region SwitchToGanttViewItem
	public class SwitchToGanttViewItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public SwitchToGanttViewItem() {
		}
		public SwitchToGanttViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToGanttViewItem(string caption)
			: base(caption) {
		}
		public SwitchToGanttViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId { get { return SchedulerCommandId.SwitchToGanttView; } }
	}
	#endregion
	#region SchedulerNavigatorBarCreator
	public class SchedulerNavigatorBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(NavigatorRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(NavigatorBar); } }
		public override int DockColumn { get { return 3; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new NavigatorBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerViewNavigatorItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new NavigatorRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region NavigatorBar
	public class NavigatorBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public NavigatorBar() {
		}
		public NavigatorBar(BarManager manager)
			: base(manager) {
		}
		public NavigatorBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupNavigator); }
		}
	}
	#endregion
	#region SchedulerViewNavigatorItemBuilder
	public class SchedulerViewNavigatorItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new NavigateViewBackwardItem());
			items.Add(new NavigateViewForwardItem());
			items.Add(new GotoTodayItem());
			items.Add(new ViewZoomInItem());
			items.Add(new ViewZoomOutItem());
		}
	}
	#endregion
	#region NavigateViewBackwardItem
	public class NavigateViewBackwardItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public NavigateViewBackwardItem() {
		}
		public NavigateViewBackwardItem(string caption)
			: base(caption) {
		}
		public NavigateViewBackwardItem(BarManager manager)
			: base(manager) {
		}
		public NavigateViewBackwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.NavigateViewBackward; }
		}
	}
	#endregion
	#region NavigateViewForwardItem
	public class NavigateViewForwardItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public NavigateViewForwardItem() {
		}
		public NavigateViewForwardItem(string caption)
			: base(caption) {
		}
		public NavigateViewForwardItem(BarManager manager)
			: base(manager) {
		}
		public NavigateViewForwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.NavigateViewForward; }
		}
	}
	#endregion
	#region GotoTodayItem
	public class GotoTodayItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public GotoTodayItem() {
		}
		public GotoTodayItem(string caption)
			: base(caption) {
		}
		public GotoTodayItem(BarManager manager)
			: base(manager) {
		}
		public GotoTodayItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.GotoToday; }
		}
	}
	#endregion
	#region ViewZoomInItem
	public class ViewZoomInItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public ViewZoomInItem() {
		}
		public ViewZoomInItem(string caption)
			: base(caption) {
		}
		public ViewZoomInItem(BarManager manager)
			: base(manager) {
		}
		public ViewZoomInItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.ViewZoomIn; }
		}
	}
	#endregion
	#region ViewZoomOutItem
	public class ViewZoomOutItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public ViewZoomOutItem() {
		}
		public ViewZoomOutItem(string caption)
			: base(caption) {
		}
		public ViewZoomOutItem(BarManager manager)
			: base(manager) {
		}
		public ViewZoomOutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.ViewZoomOut; }
		}
	}
	#endregion
	#region SchedulerGroupByBarCreator
	public class SchedulerGroupByBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GroupByRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GroupByBar); } }
		public override int DockColumn { get { return 5; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new GroupByBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerGroupByItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GroupByRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region GroupByBar
	public class GroupByBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public GroupByBar() {
		}
		public GroupByBar(BarManager manager)
			: base(manager) {
		}
		public GroupByBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupGroupBy); }
		}
	}
	#endregion    
	#region SchedulerGroupByItemBuilder
	public class SchedulerGroupByItemBuilder : CommandBasedBarItemBuilder {
		public SchedulerGroupByItemBuilder() {			
		}
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GroupByNoneItem());
			items.Add(new GroupByDateItem());
			items.Add(new GroupByResourceItem());			
		}
	}
	#endregion
	#region GroupByNoneItem
	public class GroupByNoneItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public GroupByNoneItem() {			
		}
		public GroupByNoneItem(string caption)
			: base(caption) {			
		}
		public GroupByNoneItem(BarManager manager)
			: base(manager) {			
		}
		public GroupByNoneItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.SwitchToGroupByNone; }
		}
	}
	#endregion
	#region GroupByResourceItem
	public class GroupByResourceItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public GroupByResourceItem() {
		}
		public GroupByResourceItem(string caption)
			: base(caption) {
		}
		public GroupByResourceItem(BarManager manager)
			: base(manager) {
		}
		public GroupByResourceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.SwitchToGroupByResource; }
		}
	}
	#endregion
	#region GroupByDateItem
	public class GroupByDateItem : ControlCommandBarCheckItem<SchedulerControl, SchedulerCommandId> {
		public GroupByDateItem() {
		}
		public GroupByDateItem(string caption)
			: base(caption) {
		}
		public GroupByDateItem(BarManager manager)
			: base(manager) {
		}
		public GroupByDateItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.SwitchToGroupByDate; }
		}
	}
	#endregion
	#region SchedulerAppointmentBarCreator
	public class SchedulerAppointmentBarCreator : ControlCommandBarCreator {
		public SchedulerAppointmentBarCreator() {
		}
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(AppointmentRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(AppointmentBar); } }
		public override int DockColumn { get { return 2; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new AppointmentBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerAppointmentItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new AppointmentRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region AppointmentBar
	public class AppointmentBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public AppointmentBar() {			
		}
		public AppointmentBar(BarManager manager)
			: base(manager) {			
		}
		public AppointmentBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupAppointment); }
		}
	}
	#endregion
	#region SchedulerAppointmentItemBuilder
	public class SchedulerAppointmentItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new NewAppointmentItem());
			items.Add(new NewRecurringAppointmentItem());
		}
	}
	#endregion
	#region NewAppointmentItem
	public class NewAppointmentItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public NewAppointmentItem() {			
		}
		public NewAppointmentItem(string caption)
			: base(caption) {
		}
		public NewAppointmentItem(BarManager manager)
			: base(manager) {
		}
		public NewAppointmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.NewAppointment; }
		}
	}
	#endregion
	#region NewRecurringAppointmentItem
	public class NewRecurringAppointmentItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public NewRecurringAppointmentItem() {
		}
		public NewRecurringAppointmentItem(string caption)
			: base(caption) {
		}
		public NewRecurringAppointmentItem(BarManager manager)
			: base(manager) {
		}
		public NewRecurringAppointmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.NewRecurringAppointment; }
		}
	}
	#endregion    
}
