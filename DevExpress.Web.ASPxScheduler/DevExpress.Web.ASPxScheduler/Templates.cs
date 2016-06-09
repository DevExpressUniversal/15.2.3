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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	#region SchedulerTemplateContainerBase
	public abstract class SchedulerTemplateContainerBase : TemplateContainerBase {
		bool isBinded = false;
		protected SchedulerTemplateContainerBase(int itemIndex, object dataItem)
			: base(itemIndex, dataItem) {
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, CreateCommandEventArgs(source, e));
				return true;
			}
			return base.OnBubbleEvent(source, e);
		}
		protected abstract EventArgs CreateCommandEventArgs(object source, EventArgs e);
		public override void DataBind() {
			if (!this.isBinded)
				base.DataBind();
			this.isBinded = true;
		}
	}
	#endregion
	#region SchedulerCommandEventArgs
	public class SchedulerCommandEventArgs : CommandEventArgs {
		object commandSource = null;
		public SchedulerCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			this.commandSource = commandSource;
		}
		public object CommandSource { get { return commandSource; } }
	}
	#endregion
	#region DateHeaderCommandEventArgs
	public class DateHeaderCommandEventArgs : SchedulerCommandEventArgs {
		WebDateHeader header;
		public DateHeaderCommandEventArgs(WebDateHeader header, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (header == null)
				Exceptions.ThrowArgumentNullException("header");
			this.header = header;
		}
		public WebDateHeader Header { get { return header; } }
	}
	#endregion
	#region DayHeaderCommandEventArgs
	public class DayHeaderCommandEventArgs : SchedulerCommandEventArgs {
		WebDayOfWeekHeader header;
		public DayHeaderCommandEventArgs(WebDayOfWeekHeader header, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (header == null)
				Exceptions.ThrowArgumentNullException("header");
			this.header = header;
		}
		public WebDayOfWeekHeader Header { get { return header; } }
	}
	#endregion
	#region ResourceHeaderCommandEventArgs
	public class ResourceHeaderCommandEventArgs : SchedulerCommandEventArgs {
		WebResourceHeaderBase header;
		public ResourceHeaderCommandEventArgs(WebResourceHeaderBase header, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (header == null)
				Exceptions.ThrowArgumentNullException("header");
			this.header = header;
		}
		public WebResourceHeaderBase Header { get { return header; } }
	}
	#endregion
	#region TimeCellCommandEventArgs
	public class TimeCellBodyCommandEventArgs : SchedulerCommandEventArgs {
		WebTimeCell timeCell;
		public TimeCellBodyCommandEventArgs(WebTimeCell timeCell, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (timeCell == null)
				Exceptions.ThrowArgumentNullException("timeCell");
			this.timeCell = timeCell;
		}
		public WebTimeCell TimeCell { get { return timeCell; } }
	}
	#endregion
	#region AllDayAreaCommandEventArgs
	public class AllDayAreaCommandEventArgs : SchedulerCommandEventArgs {
		WebAllDayAreaCell allDayArea;
		public AllDayAreaCommandEventArgs(WebAllDayAreaCell allDayArea, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (allDayArea == null)
				Exceptions.ThrowArgumentNullException("allDayArea");
			this.allDayArea = allDayArea;
		}
		public WebAllDayAreaCell AllDayArea { get { return allDayArea; } }
	}
	#endregion
	#region SelectionBarCommandEventArgs
	public class SelectionBarCommandEventArgs : SchedulerCommandEventArgs {
		WebSelectionBarCell selectionBar;
		public SelectionBarCommandEventArgs(WebSelectionBarCell selectionBar, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (selectionBar == null)
				Exceptions.ThrowArgumentNullException("selectionBar");
			this.selectionBar = selectionBar;
		}
		public WebSelectionBarCell SelectionBar { get { return selectionBar; } }
	}
	#endregion
	#region DateCellHeaderCommandEventArgs
	public class DateCellHeaderCommandEventArgs : SchedulerCommandEventArgs {
		WebDateHeader header;
		public DateCellHeaderCommandEventArgs(WebDateHeader header, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (header == null)
				Exceptions.ThrowArgumentNullException("dateContent");
			this.header = header;
		}
		public WebDateHeader Header { get { return header; } }
	}
	#endregion
	#region DateCellBodyCommandEventArgs
	public class DateCellBodyCommandEventArgs : SchedulerCommandEventArgs {
		WebDateContent content;
		public DateCellBodyCommandEventArgs(WebDateContent content, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (content == null)
				Exceptions.ThrowArgumentNullException("content");
			this.content = content;
		}
		public WebDateContent Content { get { return content; } }
	}
	#endregion
	#region TimeRulerHoursItemCommandEventArgs
	public class TimeRulerHoursItemCommandEventArgs : SchedulerCommandEventArgs {
		WebTimeRulerHoursItem timeRulerHoursItem;
		public TimeRulerHoursItemCommandEventArgs(WebTimeRulerHoursItem timeRulerHoursItem, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (timeRulerHoursItem == null)
				Exceptions.ThrowArgumentNullException("timeRulerHoursItem");
			this.timeRulerHoursItem = timeRulerHoursItem;
		}
		public WebTimeRulerHoursItem TimeRulerHoursItem { get { return timeRulerHoursItem; } }
	}
	#endregion
	#region TimeRulerMinuteItemCommandEventArgs
	public class TimeRulerMinuteItemCommandEventArgs : SchedulerCommandEventArgs {
		WebTimeRulerMinuteItem timeRulerMinuteItem;
		public TimeRulerMinuteItemCommandEventArgs(WebTimeRulerMinuteItem timeRulerMinuteItem, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (timeRulerMinuteItem == null)
				Exceptions.ThrowArgumentNullException("timeRulerMinuteItem");
			this.timeRulerMinuteItem = timeRulerMinuteItem;
		}
		public WebTimeRulerMinuteItem TimeRulerMinuteItem { get { return timeRulerMinuteItem; } }
	}
	#endregion
	#region TimelineCellBodyCommandEventArgs
	public class TimelineCellBodyCommandEventArgs : SchedulerCommandEventArgs {
		WebSingleTimelineCell timelineCell;
		public TimelineCellBodyCommandEventArgs(WebSingleTimelineCell timelineCell, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (timelineCell == null)
				Exceptions.ThrowArgumentNullException("timelineCell");
			this.timelineCell = timelineCell;
		}
		public WebSingleTimelineCell TimelineCell { get { return timelineCell; } }
	}
	#endregion
	#region TimelineDateHeaderCommandEventArgs
	public class TimelineDateHeaderCommandEventArgs : SchedulerCommandEventArgs {
		WebTimeScaleHeader header;
		public TimelineDateHeaderCommandEventArgs(WebTimeScaleHeader header, object commandSource, CommandEventArgs originalArgs)
			: base(commandSource, originalArgs) {
			if (header == null)
				Exceptions.ThrowArgumentNullException("header");
			this.header = header;
		}
		public WebTimeScaleHeader Header { get { return header; } }
	}
	#endregion
	#region DateHeaderTemplateContainer
	public class DateHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public DateHeaderTemplateContainer(WebDateHeader dateHeader)
			: base(0, dateHeader) {
		}
		#region Properties
		internal WebDateHeader Header { get { return (WebDateHeader)DataItem; } }
		public TimeInterval Interval { get { return Header.Interval; } }
		public ResourceBaseCollection Resources { get { return Header.Resources; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new DateHeaderCommandEventArgs(Header, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region DayOfWeekHeaderTemplateContainer
	public class DayOfWeekHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public DayOfWeekHeaderTemplateContainer(WebDayOfWeekHeader dayHeader)
			: base(0, dayHeader) {
		}
		#region Properties
		internal WebDayOfWeekHeader Header { get { return (WebDayOfWeekHeader)DataItem; } }
		public DayOfWeek DayOfWeek { get { return Header.DayOfWeek; } }
		public Resource Resource { get { return Header.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new DayHeaderCommandEventArgs(Header, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region ResourceHeaderTemplateContainer
	public class ResourceHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public ResourceHeaderTemplateContainer(WebResourceHeaderBase resourceHeader)
			: base(0, resourceHeader) {
		}
		#region Properties
		internal WebResourceHeaderBase Header { get { return (WebResourceHeaderBase)DataItem; } }
		public TimeIntervalCollection Intervals { get { return Header.Intervals; } }
		public Resource Resource { get { return Header.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new ResourceHeaderCommandEventArgs(Header, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region RightTopCornerTemplateContainer
	public class RightTopCornerTemplateContainer : TemplateContainerBase {
		public RightTopCornerTemplateContainer()
			: base(0, new object()) {
		}
	}
	#endregion
	#region AllDayAreaTemplateContainer
	public class AllDayAreaTemplateContainer : SchedulerTemplateContainerBase {
		public AllDayAreaTemplateContainer(WebAllDayAreaCell allDayAreaCell)
			: base(0, allDayAreaCell) {
		}
		#region Properties
		internal WebAllDayAreaCell AllDayArea { get { return (WebAllDayAreaCell)DataItem; } }
		public TimeInterval Interval { get { return AllDayArea.Interval; } }
		public Resource Resource { get { return AllDayArea.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new AllDayAreaCommandEventArgs(AllDayArea, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region SelectionBarTemplateContainer
	public class SelectionBarTemplateContainer : SchedulerTemplateContainerBase {
		public SelectionBarTemplateContainer(WebSelectionBarCell selectionBar)
			: base(0, selectionBar) {
		}
		#region Properties
		internal WebSelectionBarCell SelectionBarCell { get { return (WebSelectionBarCell)DataItem; } }
		public TimeInterval Interval { get { return SelectionBarCell.Interval; } }
		public Resource Resource { get { return SelectionBarCell.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new SelectionBarCommandEventArgs(SelectionBarCell, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region DateCellHeaderTemplateContainer
	public class DateCellHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public DateCellHeaderTemplateContainer(WebDateHeader dateCellHeader)
			: base(0, dateCellHeader) {
		}
		#region Properties
		internal WebDateHeader DateCellHeader { get { return (WebDateHeader)DataItem; } }
		public TimeInterval Interval { get { return DateCellHeader.Interval; } }
		public ResourceBaseCollection Resources { get { return DateCellHeader.Resources; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new DateCellHeaderCommandEventArgs(DateCellHeader, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region DateCellBodyTemplateContainer
	public class DateCellBodyTemplateContainer : SchedulerTemplateContainerBase {
		public DateCellBodyTemplateContainer(WebDateContent dateCellBody)
			: base(0, dateCellBody) {
		}
		#region Properties
		internal WebDateContent Content { get { return (WebDateContent)DataItem; } }
		public TimeInterval Interval { get { return Content.Interval; } }
		public Resource Resource { get { return Content.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new DateCellBodyCommandEventArgs(Content, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region SeparatorTemplateContainer
	public class SeparatorTemplateContainer : TemplateContainerBase {
		public SeparatorTemplateContainer(WebGroupSeparator item)
			: base(0, item) {
		}
	}
	#endregion
	#region TimeRulerHoursItemTemplateContainer
	public class TimeRulerHoursItemTemplateContainer : SchedulerTemplateContainerBase {
		public TimeRulerHoursItemTemplateContainer(WebTimeRulerHoursItem item)
			: base(0, item) {
		}
		#region Properties
		internal WebTimeRulerHoursItem TimeRulerItem { get { return (WebTimeRulerHoursItem)DataItem; } }
		public DateTime Time { get { return TimeRulerItem.Time; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new TimeRulerHoursItemCommandEventArgs(TimeRulerItem, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region TimeRulerMinuteItemTemplateContainer
	public class TimeRulerMinuteItemTemplateContainer : SchedulerTemplateContainerBase {
		public TimeRulerMinuteItemTemplateContainer(WebTimeRulerMinuteItem item)
			: base(0, item) {
		}
		#region Properties
		internal WebTimeRulerMinuteItem TimeRulerItem { get { return (WebTimeRulerMinuteItem)DataItem; } }
		public DateTime Time { get { return TimeRulerItem.Time; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new TimeRulerMinuteItemCommandEventArgs(TimeRulerItem, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region TimeCellBodyTemplateContainer
	public class TimeCellBodyTemplateContainer : SchedulerTemplateContainerBase {
		public TimeCellBodyTemplateContainer(WebTimeCell timeCell)
			: base(0, timeCell) {
		}
		#region Properties
		internal WebTimeCell TimeCellBody { get { return (WebTimeCell)DataItem; } }
		public TimeInterval Interval { get { return TimeCellBody.Interval; } }
		public Resource Resource { get { return TimeCellBody.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new TimeCellBodyCommandEventArgs(TimeCellBody, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region TimelineCellBodyTemplateContainer
	public class TimelineCellBodyTemplateContainer : SchedulerTemplateContainerBase {
		public TimelineCellBodyTemplateContainer(WebSingleTimelineCell timelineCell)
			: base(0, timelineCell) {
		}
		#region Properties
		internal WebSingleTimelineCell TimelineCell { get { return (WebSingleTimelineCell)DataItem; } }
		public TimeInterval Interval { get { return TimelineCell.Interval; } }
		public Resource Resource { get { return TimelineCell.Resource; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new TimelineCellBodyCommandEventArgs(TimelineCell, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region TimelineDateHeaderTemplateContainer
	public class TimelineDateHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public TimelineDateHeaderTemplateContainer(WebTimeScaleHeader header)
			: base(0, header) {
		}
		#region Properties
		internal WebTimeScaleHeader Header { get { return (WebTimeScaleHeader)DataItem; } }
		public TimeInterval Interval { get { return Header.Interval; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new TimelineDateHeaderCommandEventArgs(Header, source, e as CommandEventArgs);
		}
	}
	#endregion
	#region TimeRulerHeaderTemplateContainer
	public class TimeRulerHeaderTemplateContainer : SchedulerTemplateContainerBase {
		public TimeRulerHeaderTemplateContainer(WebTimeRulerHeader header)
			: base(0, header) {
		}
		#region Properties
		internal WebTimeRulerHeader Header { get { return (WebTimeRulerHeader)DataItem; } }
		public TimeRuler TimerRuler { get { return Header.TimeRuler; } }
		#endregion
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new SchedulerCommandEventArgs(source, e as CommandEventArgs);
		}
	}
	#endregion
	#region AppointmentTemplateContainer
	public abstract class AppointmentTemplateContainer : SchedulerTemplateContainerBase {
		readonly AppointmentViewInfo aptViewInfo;
		protected AppointmentTemplateContainer(AppointmentViewInfo aptViewInfo)
			: base(0, aptViewInfo) {
			this.aptViewInfo = aptViewInfo;
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("AppointmentTemplateContainerAppointmentViewInfo")]
#endif
		public AppointmentViewInfo AppointmentViewInfo { get { return aptViewInfo; } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new SchedulerCommandEventArgs(source, e as CommandEventArgs);
		}
	}
	#endregion
	public class HorizontalAppointmentTemplateContainer : AppointmentTemplateContainer {
		HorizontalAppointmentTemplateItems items;
		public HorizontalAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, HorizontalAppointmentTemplateItems templateItems)
			: base(aptViewInfo) {
			this.items = templateItems;
		}
		public HorizontalAppointmentTemplateItems Items { get { return items; } set { items = value; } }
	}
	public class VerticalAppointmentTemplateContainer : AppointmentTemplateContainer {
		VerticalAppointmentTemplateItems items;
		public VerticalAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, VerticalAppointmentTemplateItems templateItems)
			: base(aptViewInfo) {
			this.items = templateItems;
		}
		public VerticalAppointmentTemplateItems Items { get { return items; } set { items = value; } }
	}
	public class SchedulerToolbarTemplateContainerBase : SchedulerTemplateContainerBase {
		ASPxScheduler scheduler;
		public SchedulerToolbarTemplateContainerBase(ASPxScheduler scheduler)
			: base(0, scheduler) {
			this.scheduler = scheduler;
		}
		public ASPxScheduler Scheduler { get { return scheduler; } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return new SchedulerCommandEventArgs(source, e as CommandEventArgs);
		}
	}
	public class ToolbarViewSelectorTemplateContainer : SchedulerToolbarTemplateContainerBase {
		public ToolbarViewSelectorTemplateContainer(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	public class ToolbarViewNavigatorTemplateContainer : SchedulerToolbarTemplateContainerBase {
		public ToolbarViewNavigatorTemplateContainer(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	public class ToolbarViewVisibleIntervalTemplateContainer : SchedulerToolbarTemplateContainerBase {
		public ToolbarViewVisibleIntervalTemplateContainer(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	#region SchedulerTemplates
	public class SchedulerTemplates {
		#region Fields
		ITemplate dateHeaderTemplate;
		ITemplate horizontalResourceHeaderTemplate;
		ITemplate verticalResourceHeaderTemplate;
		ITemplate dayOfWeekHeaderTemplate;
		ITemplate horizontalAppointmentTemplate;
		ITemplate horizontalSameDayAppointmentTemplate;
		ITemplate toolbarViewSelectorTemplate;
		ITemplate toolbarViewNavigatorTemplate;
		ITemplate toolbarViewVisibleIntervalTemplate;
		#endregion
		public SchedulerTemplates() {
		}
		#region Properties
		#region HorizontalResourceHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ResourceHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate HorizontalResourceHeaderTemplate {
			get { return horizontalResourceHeaderTemplate; }
			set {
				horizontalResourceHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DateHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DateHeaderTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ITemplate DateHeaderTemplate {
			get { return dateHeaderTemplate; }
			set {
				dateHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DayOfWeekHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DayOfWeekHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate DayOfWeekHeaderTemplate {
			get { return dayOfWeekHeaderTemplate; }
			set {
				dayOfWeekHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region VerticalResourceHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ResourceHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public virtual ITemplate VerticalResourceHeaderTemplate {
			get { return verticalResourceHeaderTemplate; }
			set {
				verticalResourceHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region HorizontalAppointmentTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(AppointmentTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate HorizontalAppointmentTemplate {
			get { return horizontalAppointmentTemplate; }
			set {
				horizontalAppointmentTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region HorizontalSameDayAppointmentTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(AppointmentTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate HorizontalSameDayAppointmentTemplate {
			get { return horizontalSameDayAppointmentTemplate; }
			set {
				horizontalSameDayAppointmentTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region ToolbarViewSelectorTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ToolbarViewSelectorTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate ToolbarViewSelectorTemplate {
			get { return toolbarViewSelectorTemplate; }
			set {
				toolbarViewSelectorTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region ToolbarViewNavigatorTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ToolbarViewNavigatorTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate ToolbarViewNavigatorTemplate {
			get { return toolbarViewNavigatorTemplate; }
			set {
				toolbarViewNavigatorTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region ToolbarViewVisibleIntervalTemplate
		[
		Browsable(false),
		DefaultValue(null),
		AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ToolbarViewVisibleIntervalTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate ToolbarViewVisibleIntervalTemplate {
			get { return toolbarViewVisibleIntervalTemplate; }
			set {
				toolbarViewVisibleIntervalTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#endregion
		#region Events
		protected internal event EventHandler Changed;
		protected internal virtual void OnChanged() {
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
		#endregion
	}
	#endregion
	#region DayViewTemplates
	public class DayViewTemplates : SchedulerTemplates {
		#region Fields
		ITemplate timeCellBodyTemplate;
		ITemplate allDayAreaTemplate;
		ITemplate timeRulerHoursItemTemplate;
		ITemplate timeRulerMinuteItemTemplate;
		ITemplate timeRulerHeaderTemplate;
		ITemplate rightTopCornerTemplate;
		ITemplate verticalAppointmentTemplate;
		#endregion
		public DayViewTemplates() {
		}
		#region Properties
		#region TimeCellBodyTemplate
		[
		Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimeCellBodyTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ITemplate TimeCellBodyTemplate {
			get { return timeCellBodyTemplate; }
			set {
				timeCellBodyTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region AllDayAreaTemplate
		[
		Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(AllDayAreaTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ITemplate AllDayAreaTemplate {
			get { return allDayAreaTemplate; }
			set {
				allDayAreaTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region TimeRulerHoursItemTemplate
		[
		Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimeRulerHoursItemTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		internal ITemplate TimeRulerHoursItemTemplate {
			get { return timeRulerHoursItemTemplate; }
			set {
				timeRulerHoursItemTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region TimeRulerMinuteItemTemplate
		[
		Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimeRulerMinuteItemTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		internal ITemplate TimeRulerMinuteItemTemplate {
			get { return timeRulerMinuteItemTemplate; }
			set {
				timeRulerMinuteItemTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region TimeRulerHeaderTemplate
		[
		Browsable(false), DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimeRulerHeaderTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ITemplate TimeRulerHeaderTemplate {
			get { return timeRulerHeaderTemplate; }
			set {
				timeRulerHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region RightTopCornerTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(RightTopCornerTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate RightTopCornerTemplate {
			get { return rightTopCornerTemplate; }
			set {
				rightTopCornerTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DayOfWeekHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DayOfWeekHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true)
		]
		public new ITemplate DayOfWeekHeaderTemplate {
			get { return base.DayOfWeekHeaderTemplate; }
			set {
				base.DayOfWeekHeaderTemplate = value;
			}
		}
		#endregion
		#region VerticalResourceHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ResourceHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true)
		]
		public override ITemplate VerticalResourceHeaderTemplate {
			get { return base.VerticalResourceHeaderTemplate; }
			set { base.VerticalResourceHeaderTemplate = value; }
		}
		#endregion
		#region VerticalAppointmentTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(AppointmentTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate VerticalAppointmentTemplate {
			get { return verticalAppointmentTemplate; }
			set {
				verticalAppointmentTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region WeekViewTemplates
	public class WeekViewTemplates : SchedulerTemplates {
		#region Fields
		ITemplate dateCellBodyTemplate;
		ITemplate dateCellHeaderTemplate;
		#endregion
		public WeekViewTemplates() {
		}
		#region Properties
		#region DateCellHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DateCellHeaderTemplateContainer)),
		NotifyParentProperty(true)
		]
		public virtual ITemplate DateCellHeaderTemplate {
			get { return dateCellHeaderTemplate; }
			set {
				dateCellHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DateCellBodyTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(DateCellBodyTemplateContainer)),
		NotifyParentProperty(true)
		]
		public virtual ITemplate DateCellBodyTemplate {
			get { return dateCellBodyTemplate; }
			set {
				dateCellBodyTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DateHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DateHeaderTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new ITemplate DateHeaderTemplate {
			get { return base.DateHeaderTemplate; }
			set {
				base.DateHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region MonthViewTemplates
	public class MonthViewTemplates : WeekViewTemplates {
		public MonthViewTemplates() {
		}
	}
	#endregion
	#region WorkWeekViewTemplates
	public class WorkWeekViewTemplates : DayViewTemplates {
		public WorkWeekViewTemplates() {
		}
	}
	#endregion
	#region TimelineViewTemplates
	public class TimelineViewTemplates : SchedulerTemplates {
		#region Fields
		ITemplate selectionBarTemplate;
		ITemplate timelineCellBodyTemplate;
		ITemplate timelineDateHeaderTemplate;
		#endregion
		public TimelineViewTemplates() {
		}
		#region Properties
		#region SelectionBarTemplate
		[
		Browsable(false), DefaultValue(null),
		EditorBrowsable(EditorBrowsableState.Never),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(SelectionBarTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true)
		]
		internal ITemplate SelectionBarTemplate {
			get { return selectionBarTemplate; }
			set {
				selectionBarTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region TimelineCellBodyTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimelineCellBodyTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate TimelineCellBodyTemplate {
			get { return timelineCellBodyTemplate; }
			set {
				timelineCellBodyTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region TimelineDateHeaderTemplate
		[
		Browsable(false),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TimelineDateHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)
		]
		public ITemplate TimelineDateHeaderTemplate {
			get { return timelineDateHeaderTemplate; }
			set {
				timelineDateHeaderTemplate = value;
				OnChanged();
			}
		}
		#endregion
		#region DayOfWeekHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DayOfWeekHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true)
		]
		public new ITemplate DayOfWeekHeaderTemplate {
			get { return base.DayOfWeekHeaderTemplate; }
			set {
				base.DayOfWeekHeaderTemplate = value;
			}
		}
		#endregion
		#region DateHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DateHeaderTemplateContainer)),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new ITemplate DateHeaderTemplate {
			get { return base.DateHeaderTemplate; }
			set {
				base.DateHeaderTemplate = value;
			}
		}
		#endregion
		#region HorizontalResourceHeaderTemplate
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(ResourceHeaderTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true)
		]
		public new ITemplate HorizontalResourceHeaderTemplate {
			get { return base.HorizontalResourceHeaderTemplate; }
			set {
				base.HorizontalResourceHeaderTemplate = value;
			}
		}
		#endregion
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region TemplatesHelper
	public class TemplatesHelper {
		#region Create
		public static TemplatesHelper Create(SchedulerViewBase view) {
			switch (view.Type) {
				case SchedulerViewType.Day:
					return new DayViewTemplatesHelper((DayView)view);
				case SchedulerViewType.Month:
					return new MonthViewTemplatesHelper((MonthView)view);
				case SchedulerViewType.Timeline:
					return new TimelineViewTemplatesHelper((TimelineView)view);
				case SchedulerViewType.Week:
					return new WeekViewTemplatesHelper((WeekView)view);
				case SchedulerViewType.WorkWeek:
					return new WorkWeekViewTemplatesHelper((WorkWeekView)view);
				case SchedulerViewType.FullWeek:
					return new FullWeekViewTemplatesHelper((FullWeekView)view);
			}
			return null;
		}
		#endregion
		#region Fields
		SchedulerViewBase view;
		#endregion
		protected TemplatesHelper(SchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			this.view = view;
		}
		#region Properties
		internal SchedulerViewBase View { get { return view; } }
		internal SchedulerTemplates ViewTemplates { get { return View.InnerTemplates; } }
		internal SchedulerTemplates ControlTemplates { get { return View.Control.Templates; } }
		#endregion
		#region GetTemplateContainer
		public virtual TemplateContainerBase GetTemplateContainer(WebElementType type, object item) {
			switch (type) {
				case WebElementType.DateHeader:
					return GetDateHeaderTemplateContainer(item);
				case WebElementType.HorizontalResourceHeader:
					return GetHorizontalResourceHeaderTemplateContainer(item);
				case WebElementType.VerticalResourceHeader:
					return GetVerticalResourceHeaderTemplateContainer(item);
				case WebElementType.DayHeader:
					return GetDayOfWeekHeaderTemplateContainer(item);
			}
			return null;
		}
		#endregion
		#region GetDateHeaderTemplateContainer
		protected internal virtual TemplateContainerBase GetDateHeaderTemplateContainer(object item) {
			ITemplate template = ViewTemplates.DateHeaderTemplate;
			if (template == null)
				template = ControlTemplates.DateHeaderTemplate;
			if (template == null)
				return null;
			DateHeaderTemplateContainer container = CreateDateHeaderTemplateContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual DateHeaderTemplateContainer CreateDateHeaderTemplateContainer(object item) {
			return new DateHeaderTemplateContainer((WebDateHeader)item);
		}
		#endregion
		static internal TemplateContainerBase GetToolbarViewSelectorTemplateContainer(ASPxScheduler scheduler) {
			ITemplate template = scheduler.Templates.ToolbarViewSelectorTemplate;
			ITemplate viewTemplate = scheduler.ActiveView.InnerTemplates.ToolbarViewSelectorTemplate;
			if (template == null)
				template = viewTemplate;
			if (template == null)
				return null;
			ToolbarViewSelectorTemplateContainer container = new ToolbarViewSelectorTemplateContainer(scheduler);
			template.InstantiateIn(container);
			return container;
		}
		static internal TemplateContainerBase GetToolbarViewNavigatorTempalteContainer(ASPxScheduler scheduler) {
			ITemplate template = scheduler.Templates.ToolbarViewNavigatorTemplate;
			ITemplate viewTemplate = scheduler.ActiveView.InnerTemplates.ToolbarViewNavigatorTemplate;
			if (template == null)
				template = viewTemplate;
			if (template == null)
				return null;
			ToolbarViewNavigatorTemplateContainer container = new ToolbarViewNavigatorTemplateContainer(scheduler);
			template.InstantiateIn(container);
			return container;
		}
		static internal TemplateContainerBase GetToolbarViewVisibleIntervalTemplateContainer(ASPxScheduler scheduler) {
			ITemplate template = scheduler.Templates.ToolbarViewVisibleIntervalTemplate;
			ITemplate viewTemplate = scheduler.ActiveView.InnerTemplates.ToolbarViewVisibleIntervalTemplate;
			if (template == null)
				template = viewTemplate;
			if (template == null)
				return null;
			ToolbarViewVisibleIntervalTemplateContainer container = new ToolbarViewVisibleIntervalTemplateContainer(scheduler);
			template.InstantiateIn(container);
			return container;
		}
		#region GetHorizontalResourceHeaderTemplateContainer
		protected internal virtual TemplateContainerBase GetHorizontalResourceHeaderTemplateContainer(object item) {
			ITemplate template = ViewTemplates.HorizontalResourceHeaderTemplate;
			if (template == null)
				template = ControlTemplates.HorizontalResourceHeaderTemplate;
			if (template == null)
				return null;
			ResourceHeaderTemplateContainer container = CreateResourceHeaderTemplateContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual ResourceHeaderTemplateContainer CreateResourceHeaderTemplateContainer(object item) {
			return new ResourceHeaderTemplateContainer((WebResourceHeaderBase)item);
		}
		#endregion
		#region GetVerticalResourceHeaderTemplateContainer
		protected internal virtual TemplateContainerBase GetVerticalResourceHeaderTemplateContainer(object item) {
			ITemplate template = ViewTemplates.VerticalResourceHeaderTemplate;
			if (template == null)
				template = ControlTemplates.VerticalResourceHeaderTemplate;
			if (template == null)
				return null;
			ResourceHeaderTemplateContainer container = CreateResourceHeaderTemplateContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		#endregion
		#region GetDayHeaderTemplateContainer
		protected internal virtual TemplateContainerBase GetDayOfWeekHeaderTemplateContainer(object item) {
			ITemplate template = ViewTemplates.DayOfWeekHeaderTemplate;
			if (template == null)
				template = ControlTemplates.DayOfWeekHeaderTemplate;
			if (template == null)
				return null;
			DayOfWeekHeaderTemplateContainer container = CreateDayOfWeekHeaderTemplateContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual DayOfWeekHeaderTemplateContainer CreateDayOfWeekHeaderTemplateContainer(object item) {
			return new DayOfWeekHeaderTemplateContainer((WebDayOfWeekHeader)item);
		}
		#endregion
		#region GetHorizontalAppointmentTemplateContainer
		public AppointmentTemplateContainer GetHorizontalAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, HorizontalAppointmentTemplateItems itemsProperties) {
			AppointmentTemplateContainer container = new HorizontalAppointmentTemplateContainer(aptViewInfo, itemsProperties);
			ITemplate template = ViewTemplates.HorizontalAppointmentTemplate;
			if (template == null)
				template = ControlTemplates.HorizontalAppointmentTemplate;
			if (template != null)
				template.InstantiateIn(container);
			else {
				var horizontalAppointmentControl = new DevExpress.Web.ASPxScheduler.Forms.Internal.HorizontalAppointmentTemplate();
				horizontalAppointmentControl.ParentSkinOwner = View.Control;
				SchedulerUserControl.PrepareUserControl(horizontalAppointmentControl, container);
			}
			return container;
		}
		#endregion
		#region GetHorizontalSameDayAppointmentTemplateContainer
		public AppointmentTemplateContainer GetHorizontalSameDayAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, HorizontalAppointmentTemplateItems itemsProperties) {
			HorizontalAppointmentTemplateContainer container = new HorizontalAppointmentTemplateContainer(aptViewInfo, itemsProperties);
			ITemplate template = ViewTemplates.HorizontalSameDayAppointmentTemplate;
			if (template == null)
				template = ControlTemplates.HorizontalSameDayAppointmentTemplate;
			if (template != null)
				template.InstantiateIn(container);
			else {
				var horizontalAppointmentControl = new DevExpress.Web.ASPxScheduler.Forms.Internal.HorizontalSameDayAppointmentTemplate();
				horizontalAppointmentControl.ParentSkinOwner = View.Control;
				SchedulerUserControl.PrepareUserControl(horizontalAppointmentControl, container);
			}
			return container;
		}
		#endregion
		#region LoadTemplate
		protected internal virtual ITemplate LoadTemplate(string templateName) {
			string templatePath = CommonUtils.GetDefaultFormUrl(templateName, typeof(ASPxScheduler));
			return View.Control.Page.LoadTemplate(templatePath);
		}
		#endregion
		public virtual AppointmentTemplateContainer GetVerticalAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, VerticalAppointmentTemplateItems itemsProperties) {
			return null;
		}
	}
	#endregion
	#region DayViewTemplatesHelper
	public class DayViewTemplatesHelper : TemplatesHelper {
		public DayViewTemplatesHelper(DayView view)
			: base(view) {
		}
		#region Properties
		internal DayViewTemplates Templates { get { return ((DayView)View).Templates; } }
		#endregion
		#region GetTemplateContainer
		public override TemplateContainerBase GetTemplateContainer(WebElementType type, object item) {
			switch (type) {
				case WebElementType.TimeCellBody:
					return GetTimeCellBodyContainer(item);
				case WebElementType.AllDayArea:
					return GetAllDayAreaContainer(item);
				case WebElementType.TimeRulerHours:
					return GetTimeRulerHoursItemContainer(item);
				case WebElementType.TimeRulerMinute:
					return GetTimeRulerMinuteItemContainer(item);
				case WebElementType.TimeRulerHeader:
					return GetTimeRulerHeaderItemContainer(item);
				case WebElementType.RightTopCorner:
					return GetRightTopCornerTemplateContainer();
			}
			return base.GetTemplateContainer(type, item);
		}
		#endregion
		#region GetRightTopCornerTemplate
		protected internal virtual TemplateContainerBase GetRightTopCornerTemplateContainer() {
			ITemplate template = Templates.RightTopCornerTemplate;
			if (template == null)
				return null;
			RightTopCornerTemplateContainer container = CreateRightTopCornerTemplateContainer();
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual RightTopCornerTemplateContainer CreateRightTopCornerTemplateContainer() {
			return new RightTopCornerTemplateContainer();
		}
		#endregion
		#region GetTimeCellBodyContainer
		protected internal virtual TemplateContainerBase GetTimeCellBodyContainer(object item) {
			ITemplate template = Templates.TimeCellBodyTemplate;
			if (template == null)
				return null;
			TimeCellBodyTemplateContainer container = CreateTimeCellBodyContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimeCellBodyTemplateContainer CreateTimeCellBodyContainer(object item) {
			return new TimeCellBodyTemplateContainer((WebTimeCell)item);
		}
		#endregion
		#region GetAllDayAreaContainer
		protected internal virtual TemplateContainerBase GetAllDayAreaContainer(object item) {
			ITemplate template = Templates.AllDayAreaTemplate;
			if (template == null)
				return null;
			AllDayAreaTemplateContainer container = CreateAllDayAreaContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual AllDayAreaTemplateContainer CreateAllDayAreaContainer(object item) {
			return new AllDayAreaTemplateContainer((WebAllDayAreaCell)item);
		}
		#endregion
		#region GetTimeRulerHoursContainer
		protected internal virtual TemplateContainerBase GetTimeRulerHoursItemContainer(object item) {
			ITemplate template = Templates.TimeRulerHoursItemTemplate;
			if (template == null)
				return null;
			TimeRulerHoursItemTemplateContainer container = CreateTimeRulerHoursItemContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimeRulerHoursItemTemplateContainer CreateTimeRulerHoursItemContainer(object item) {
			return new TimeRulerHoursItemTemplateContainer((WebTimeRulerHoursItem)item);
		}
		#endregion
		#region GetTimeRulerMinuteContainer
		protected internal virtual TemplateContainerBase GetTimeRulerMinuteItemContainer(object item) {
			ITemplate template = Templates.TimeRulerMinuteItemTemplate;
			if (template == null)
				return null;
			TimeRulerMinuteItemTemplateContainer container = CreateTimeRulerMinuteItemContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimeRulerMinuteItemTemplateContainer CreateTimeRulerMinuteItemContainer(object item) {
			return new TimeRulerMinuteItemTemplateContainer((WebTimeRulerMinuteItem)item);
		}
		#endregion
		#region GetTimeRulerHeaderItemContainer
		protected internal virtual TemplateContainerBase GetTimeRulerHeaderItemContainer(object item) {
			ITemplate template = Templates.TimeRulerHeaderTemplate;
			if (template == null)
				return null;
			TimeRulerHeaderTemplateContainer container = CreateTimeRulerHeaderItemContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimeRulerHeaderTemplateContainer CreateTimeRulerHeaderItemContainer(object item) {
			return new TimeRulerHeaderTemplateContainer((WebTimeRulerHeader)item);
		}
		#endregion
		public override AppointmentTemplateContainer GetVerticalAppointmentTemplateContainer(AppointmentViewInfo aptViewInfo, VerticalAppointmentTemplateItems itemsProperties) {
			ITemplate template = Templates.VerticalAppointmentTemplate;
			AppointmentTemplateContainer container = new VerticalAppointmentTemplateContainer(aptViewInfo, itemsProperties);
			if (template != null)
				template.InstantiateIn(container);
			else {
				var verticalAppointmentControl = new DevExpress.Web.ASPxScheduler.Forms.Internal.VerticalAppointmentTemplate();
				verticalAppointmentControl.ParentSkinOwner = View.Control;
				SchedulerUserControl.PrepareUserControl(verticalAppointmentControl, container);
			}
			return container;
		}
	}
	#endregion
	#region WeekViewTemplatesHelper
	public class WeekViewTemplatesHelper : TemplatesHelper {
		public WeekViewTemplatesHelper(WeekView view)
			: base(view) {
		}
		#region Properties
		internal WeekViewTemplates Templates { get { return ((WeekView)View).Templates; } }
		#endregion
		#region GetTemplateContainer
		public override TemplateContainerBase GetTemplateContainer(WebElementType type, object item) {
			switch (type) {
				case WebElementType.DateCellBody:
					return GetDateCellBodyContainer(item);
				case WebElementType.DateCellHeader:
					return GetDateCellHeaderContainer(item);
			}
			return base.GetTemplateContainer(type, item);
		}
		#endregion
		#region GetDateCellBodyContainer
		protected internal virtual TemplateContainerBase GetDateCellBodyContainer(object item) {
			ITemplate template = Templates.DateCellBodyTemplate;
			if (template == null)
				return null;
			DateCellBodyTemplateContainer container = CreateDateCellBodyContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual DateCellBodyTemplateContainer CreateDateCellBodyContainer(object item) {
			return new DateCellBodyTemplateContainer((WebDateContent)item);
		}
		#endregion
		#region GetDateCellHeaderContainer
		protected internal virtual TemplateContainerBase GetDateCellHeaderContainer(object item) {
			ITemplate template = Templates.DateCellHeaderTemplate;
			if (template == null)
				return null;
			DateCellHeaderTemplateContainer container = CreateDateCellHeaderContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual DateCellHeaderTemplateContainer CreateDateCellHeaderContainer(object item) {
			return new DateCellHeaderTemplateContainer((WebDateCellHeader)item);
		}
		#endregion
	}
	#endregion
	#region MonthViewTemplatesHelper
	public class MonthViewTemplatesHelper : WeekViewTemplatesHelper {
		public MonthViewTemplatesHelper(MonthView view)
			: base(view) {
		}
	}
	#endregion
	#region WorkWeekViewTemplatesHelper
	public class WorkWeekViewTemplatesHelper : DayViewTemplatesHelper {
		public WorkWeekViewTemplatesHelper(WorkWeekView view)
			: base(view) {
		}
	}
	#endregion
	#region FullWeekViewTemplatesHelper
	public class FullWeekViewTemplatesHelper : DayViewTemplatesHelper {
		public FullWeekViewTemplatesHelper(FullWeekView view)
			: base(view) {
		}
	}
	#endregion
	#region TimelineViewTemplatesHelper
	public class TimelineViewTemplatesHelper : TemplatesHelper {
		public TimelineViewTemplatesHelper(TimelineView view)
			: base(view) {
		}
		#region Properties
		internal TimelineViewTemplates Templates { get { return ((TimelineView)View).Templates; } }
		#endregion
		#region GetTemplateContainer
		public override TemplateContainerBase GetTemplateContainer(WebElementType type, object item) {
			switch (type) {
				case WebElementType.SelectionBar:
					return GetSelectionBarContainer(item);
				case WebElementType.TimelineCellBody:
					return GetTimelineCellBodyContainer(item);
				case WebElementType.TimelineDateHeader:
					return GetTimelineDateHeaderContainer(item);
			}
			return base.GetTemplateContainer(type, item);
		}
		#endregion
		#region GetSelectionBarContainer
		protected internal virtual TemplateContainerBase GetSelectionBarContainer(object item) {
			ITemplate template = Templates.SelectionBarTemplate;
			if (template == null)
				return null;
			SelectionBarTemplateContainer container = CreateSelectionBarContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual SelectionBarTemplateContainer CreateSelectionBarContainer(object item) {
			return new SelectionBarTemplateContainer((WebSelectionBarCell)item);
		}
		#endregion
		#region GetTimelineCellBodyContainer
		protected internal virtual TemplateContainerBase GetTimelineCellBodyContainer(object item) {
			ITemplate template = Templates.TimelineCellBodyTemplate;
			if (template == null)
				return null;
			TimelineCellBodyTemplateContainer container = CreateTimelineCellBodyContainer(item);
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimelineCellBodyTemplateContainer CreateTimelineCellBodyContainer(object item) {
			TimelineCellBodyTemplateContainer container = new TimelineCellBodyTemplateContainer((WebSingleTimelineCell)item);
			return container;
		}
		#endregion
		#region GetTimelineDateHeaderContainer
		protected internal virtual TemplateContainerBase GetTimelineDateHeaderContainer(object item) {
			ITemplate template = Templates.TimelineDateHeaderTemplate;
			if (template == null)
				return null;
			TimelineDateHeaderTemplateContainer container = CreateTimelineDateHeaderContainer(item);
			container.EnableViewState = false;
			template.InstantiateIn(container);
			return container;
		}
		protected internal virtual TimelineDateHeaderTemplateContainer CreateTimelineDateHeaderContainer(object item) {
			TimelineDateHeaderTemplateContainer container = new TimelineDateHeaderTemplateContainer((WebTimeScaleHeader)item);
			return container;
		}
		#endregion
	}
	#endregion
}
