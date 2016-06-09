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
namespace DevExpress.Web.Mvc {
	using DevExpress.XtraScheduler;
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.XtraScheduler.Native;
	using DevExpress.Web.ASPxScheduler.Internal;
	using System.ComponentModel;
	public class MVCxSchedulerViewRepository : SchedulerViewRepository {
		public MVCxSchedulerViewRepository()
			: base() {
		}
		public new MVCxSchedulerDayView DayView { get { return (MVCxSchedulerDayView)base.DayView; } }
		public new MVCxSchedulerWorkWeekView WorkWeekView { get { return (MVCxSchedulerWorkWeekView)base.WorkWeekView; } }
		public new MVCxSchedulerWeekView WeekView { get { return (MVCxSchedulerWeekView)base.WeekView; } }
		public new MVCxSchedulerFullWeekView FullWeekView { get { return (MVCxSchedulerFullWeekView)base.FullWeekView; } }
		public new MVCxSchedulerMonthView MonthView { get { return (MVCxSchedulerMonthView)base.MonthView; } }
		public new MVCxSchedulerTimelineView TimelineView { get { return (MVCxSchedulerTimelineView)base.TimelineView; } }
		protected internal new void SetGroupType(SchedulerGroupType type) {
			base.SetGroupType(type);
		}
		protected internal void PrepareViews() {
			CreateViews(null);
			InitializeViews(null);
		}
		protected override DayView CreateDayViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerDayView();
		}
		protected override WorkWeekView CreateWorkWeekViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerWorkWeekView();
		}
		protected override WeekView CreateWeekViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerWeekView();
		}
		protected override MonthView CreateMonthViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerMonthView();
		}
		protected override TimelineView CreateTimelineViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerTimelineView();
		}
		protected override FullWeekView CreateFullWeekViewByControl(ASPxScheduler control) {
			return new MVCxSchedulerFullWeekView();
		}
	}
	public class MVCxSchedulerDayView : DayView {
		public MVCxSchedulerDayView()
			: base(null) {
		}
		public new MVCxTimeRulerCollection TimeRulers { get { return (MVCxTimeRulerCollection)base.TimeRulers; } }
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string TimeCellBodyTemplateContent { get; set; }
		protected internal Action<TimeCellBodyTemplateContainer> TimeCellBodyTemplateContentMethod { get; set; }
		protected internal string AllDayAreaTemplateContent { get; set; }
		protected internal Action<AllDayAreaTemplateContainer> AllDayAreaTemplateContentMethod { get; set; }
		protected internal string TimeRulerMinuteItemTemplateContent { get; set; }
		protected internal Action<TimeRulerMinuteItemTemplateContainer> TimeRulerMinuteItemTemplateContentMethod { get; set; }
		protected internal string TimeRulerHeaderTemplateContent { get; set; }
		protected internal Action<TimeRulerHeaderTemplateContainer> TimeRulerHeaderTemplateContentMethod { get; set; }
		protected internal string RightTopCornerTemplateContent { get; set; }
		protected internal Action<RightTopCornerTemplateContainer> RightTopCornerTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> VerticalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetTimeCellBodyTemplateContent(string content) {
			TimeCellBodyTemplateContent = content;
		}
		public void SetTimeCellBodyTemplateContent(Action<TimeCellBodyTemplateContainer> contentMethod) {
			TimeCellBodyTemplateContentMethod = contentMethod;
		}
		public void SetAllDayAreaTemplateContent(string content) {
			AllDayAreaTemplateContent = content;
		}
		public void SetAllDayAreaTemplateMethod(Action<AllDayAreaTemplateContainer> contentMethod) {
			AllDayAreaTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerMinuteItemTemplateContent(string content) {
			TimeRulerMinuteItemTemplateContent = content;
		}
		public void SetTimeRulerMinuteItemTemplateContent(Action<TimeRulerMinuteItemTemplateContainer> contentMethod) {
			TimeRulerMinuteItemTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerHeaderTemplateContent(string content) {
			TimeRulerHeaderTemplateContent = content;
		}
		public void SetTimeRulerHeaderTemplateContent(Action<TimeRulerHeaderTemplateContainer> contentMethod) {
			TimeRulerHeaderTemplateContentMethod = contentMethod;
		}
		public void SetRightTopCornerTemplateContent(string content) {
			RightTopCornerTemplateContent = content;
		}
		public void SetRightTopCornerTemplateContent(Action<RightTopCornerTemplateContainer> contentMethod) {
			RightTopCornerTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalAppointmentTemplateContent(string content) {
			VerticalAppointmentTemplateContent = content;
		}
		public void SetVerticalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			VerticalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetSelection(TimeInterval interval, Resource resource) {
			base.SetSelection(interval, resource);
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
		protected override InnerSchedulerViewBase CreateInnerView() {
			return new MVCxSchedulerInnerDayView(this, new DayViewProperties());
		}
	}
	public class MVCxSchedulerWorkWeekView : WorkWeekView {
		public MVCxSchedulerWorkWeekView()
			: base(null) {
		}
		public new MVCxTimeRulerCollection TimeRulers { get { return (MVCxTimeRulerCollection)base.TimeRulers; } }
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string TimeCellBodyTemplateContent { get; set; }
		protected internal Action<TimeCellBodyTemplateContainer> TimeCellBodyTemplateContentMethod { get; set; }
		protected internal string AllDayAreaTemplateContent { get; set; }
		protected internal Action<AllDayAreaTemplateContainer> AllDayAreaTemplateContentMethod { get; set; }
		protected internal string TimeRulerMinuteItemTemplateContent { get; set; }
		protected internal Action<TimeRulerMinuteItemTemplateContainer> TimeRulerMinuteItemTemplateContentMethod { get; set; }
		protected internal string TimeRulerHeaderTemplateContent { get; set; }
		protected internal Action<TimeRulerHeaderTemplateContainer> TimeRulerHeaderTemplateContentMethod { get; set; }
		protected internal string RightTopCornerTemplateContent { get; set; }
		protected internal Action<RightTopCornerTemplateContainer> RightTopCornerTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> VerticalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetTimeCellBodyTemplateContent(string content) {
			TimeCellBodyTemplateContent = content;
		}
		public void SetTimeCellBodyTemplateContent(Action<TimeCellBodyTemplateContainer> contentMethod) {
			TimeCellBodyTemplateContentMethod = contentMethod;
		}
		public void SetAllDayAreaTemplateContent(string content) {
			AllDayAreaTemplateContent = content;
		}
		public void SetAllDayAreaTemplateMethod(Action<AllDayAreaTemplateContainer> contentMethod) {
			AllDayAreaTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerMinuteItemTemplateContent(string content) {
			TimeRulerMinuteItemTemplateContent = content;
		}
		public void SetTimeRulerMinuteItemTemplateContent(Action<TimeRulerMinuteItemTemplateContainer> contentMethod) {
			TimeRulerMinuteItemTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerHeaderTemplateContent(string content) {
			TimeRulerHeaderTemplateContent = content;
		}
		public void SetTimeRulerHeaderTemplateContent(Action<TimeRulerHeaderTemplateContainer> contentMethod) {
			TimeRulerHeaderTemplateContentMethod = contentMethod;
		}
		public void SetRightTopCornerTemplateContent(string content) {
			RightTopCornerTemplateContent = content;
		}
		public void SetRightTopCornerTemplateContent(Action<RightTopCornerTemplateContainer> contentMethod) {
			RightTopCornerTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalAppointmentTemplateContent(string content) {
			VerticalAppointmentTemplateContent = content;
		}
		public void SetVerticalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			VerticalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
		protected override InnerSchedulerViewBase CreateInnerView() {
			return new MVCxSchedulerInnerWorkWeekView(this, new WorkWeekViewProperties());
		}
	}
	public class MVCxSchedulerWeekView : WeekView {
		public MVCxSchedulerWeekView()
			: base(null) {
		}
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		protected internal string DateCellHeaderTemplateContent { get; set; }
		protected internal Action<DateCellHeaderTemplateContainer> DateCellHeaderTemplateContentMethod { get; set; }
		protected internal string DateCellBodyTemplateContent { get; set; }
		protected internal Action<DateCellBodyTemplateContainer> DateCellBodyTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		public void SetDateCellHeaderTemplateContent(string content) {
			DateCellHeaderTemplateContent = content;
		}
		public void SetDateCellHeaderTemplateContent(Action<DateCellHeaderTemplateContainer> contentMethod) {
			DateCellHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateCellBodyTemplateContent(string content) {
			DateCellBodyTemplateContent = content;
		}
		public void SetDateCellBodyTemplateContent(Action<DateCellBodyTemplateContainer> contentMethod) {
			DateCellBodyTemplateContentMethod = contentMethod;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetSelection(TimeInterval interval, Resource resource) {
			base.SetSelection(interval, resource);
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
	}
	public class MVCxSchedulerMonthView : MonthView {
		public MVCxSchedulerMonthView()
			: base(null) {
		}
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		protected internal string DateCellHeaderTemplateContent { get; set; }
		protected internal Action<DateCellHeaderTemplateContainer> DateCellHeaderTemplateContentMethod { get; set; }
		protected internal string DateCellBodyTemplateContent { get; set; }
		protected internal Action<DateCellBodyTemplateContainer> DateCellBodyTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		public void SetDateCellHeaderTemplateContent(string content) {
			DateCellHeaderTemplateContent = content;
		}
		public void SetDateCellHeaderTemplateContent(Action<DateCellHeaderTemplateContainer> contentMethod) {
			DateCellHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateCellBodyTemplateContent(string content) {
			DateCellBodyTemplateContent = content;
		}
		public void SetDateCellBodyTemplateContent(Action<DateCellBodyTemplateContainer> contentMethod) {
			DateCellBodyTemplateContentMethod = contentMethod;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetSelection(TimeInterval interval, Resource resource) {
			base.SetSelection(interval, resource);
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
	}
	public class MVCxSchedulerFullWeekView : FullWeekView {
		public MVCxSchedulerFullWeekView()
			: base(null) {
		}
		public new MVCxTimeRulerCollection TimeRulers { get { return (MVCxTimeRulerCollection)base.TimeRulers; } }
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string TimeCellBodyTemplateContent { get; set; }
		protected internal Action<TimeCellBodyTemplateContainer> TimeCellBodyTemplateContentMethod { get; set; }
		protected internal string AllDayAreaTemplateContent { get; set; }
		protected internal Action<AllDayAreaTemplateContainer> AllDayAreaTemplateContentMethod { get; set; }
		protected internal string TimeRulerMinuteItemTemplateContent { get; set; }
		protected internal Action<TimeRulerMinuteItemTemplateContainer> TimeRulerMinuteItemTemplateContentMethod { get; set; }
		protected internal string TimeRulerHeaderTemplateContent { get; set; }
		protected internal Action<TimeRulerHeaderTemplateContainer> TimeRulerHeaderTemplateContentMethod { get; set; }
		protected internal string RightTopCornerTemplateContent { get; set; }
		protected internal Action<RightTopCornerTemplateContainer> RightTopCornerTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> VerticalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetTimeCellBodyTemplateContent(string content) {
			TimeCellBodyTemplateContent = content;
		}
		public void SetTimeCellBodyTemplateContent(Action<TimeCellBodyTemplateContainer> contentMethod) {
			TimeCellBodyTemplateContentMethod = contentMethod;
		}
		public void SetAllDayAreaTemplateContent(string content) {
			AllDayAreaTemplateContent = content;
		}
		public void SetAllDayAreaTemplateMethod(Action<AllDayAreaTemplateContainer> contentMethod) {
			AllDayAreaTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerMinuteItemTemplateContent(string content) {
			TimeRulerMinuteItemTemplateContent = content;
		}
		public void SetTimeRulerMinuteItemTemplateContent(Action<TimeRulerMinuteItemTemplateContainer> contentMethod) {
			TimeRulerMinuteItemTemplateContentMethod = contentMethod;
		}
		public void SetTimeRulerHeaderTemplateContent(string content) {
			TimeRulerHeaderTemplateContent = content;
		}
		public void SetTimeRulerHeaderTemplateContent(Action<TimeRulerHeaderTemplateContainer> contentMethod) {
			TimeRulerHeaderTemplateContentMethod = contentMethod;
		}
		public void SetRightTopCornerTemplateContent(string content) {
			RightTopCornerTemplateContent = content;
		}
		public void SetRightTopCornerTemplateContent(Action<RightTopCornerTemplateContainer> contentMethod) {
			RightTopCornerTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalAppointmentTemplateContent(string content) {
			VerticalAppointmentTemplateContent = content;
		}
		public void SetVerticalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			VerticalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
		protected override InnerSchedulerViewBase CreateInnerView() {
			return new MVCxSchedulerInnerFullWeekView(this, new FullWeekViewProperties());
		}
	}
	public class MVCxSchedulerTimelineView : TimelineView {
		public MVCxSchedulerTimelineView()
			: base(null) {
		}
		protected internal string HorizontalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> HorizontalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string DateHeaderTemplateContent { get; set; }
		protected internal Action<DateHeaderTemplateContainer> DateHeaderTemplateContentMethod { get; set; }
		protected internal string DayOfWeekHeaderTemplateContent { get; set; }
		protected internal Action<DayOfWeekHeaderTemplateContainer> DayOfWeekHeaderTemplateContentMethod { get; set; }
		protected internal string VerticalResourceHeaderTemplateContent { get; set; }
		protected internal Action<ResourceHeaderTemplateContainer> VerticalResourceHeaderTemplateContentMethod { get; set; }
		protected internal string HorizontalAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalAppointmentTemplateContentMethod { get; set; }
		protected internal string HorizontalSameDayAppointmentTemplateContent { get; set; }
		protected internal Action<AppointmentTemplateContainer> HorizontalSameDayAppointmentTemplateContentMethod { get; set; }
		protected internal string ToolbarViewSelectorTemplateContent { get; set; }
		protected internal Action<ToolbarViewSelectorTemplateContainer> ToolbarViewSelectorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewNavigatorTemplateContent { get; set; }
		protected internal Action<ToolbarViewNavigatorTemplateContainer> ToolbarViewNavigatorTemplateContentMethod { get; set; }
		protected internal string ToolbarViewVisibleIntervalTemplateContent { get; set; }
		protected internal Action<ToolbarViewVisibleIntervalTemplateContainer> ToolbarViewVisibleIntervalTemplateContentMethod { get; set; }
		protected internal string TimelineCellBodyTemplateContent { get; set; }
		protected internal Action<TimelineCellBodyTemplateContainer> TimelineCellBodyTemplateContentMethod { get; set; }
		protected internal string TimelineDateHeaderTemplateContent { get; set; }
		protected internal Action<TimelineDateHeaderTemplateContainer> TimelineDateHeaderTemplateContentMethod { get; set; }
		public void SetHorizontalResourceHeaderTemplateContent(string content) {
			HorizontalResourceHeaderTemplateContent = content;
		}
		public void SetHorizontalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			HorizontalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDateHeaderTemplateContent(string content) {
			DateHeaderTemplateContent = content;
		}
		public void SetDateHeaderTemplateContent(Action<DateHeaderTemplateContainer> contentMethod) {
			DateHeaderTemplateContentMethod = contentMethod;
		}
		public void SetDayOfWeekHeaderTemplateContent(string content) {
			DayOfWeekHeaderTemplateContent = content;
		}
		public void SetDayOfWeekHeaderTemplateContent(Action<DayOfWeekHeaderTemplateContainer> contentMethod) {
			DayOfWeekHeaderTemplateContentMethod = contentMethod;
		}
		public void SetVerticalResourceHeaderTemplateContent(string content) {
			VerticalResourceHeaderTemplateContent = content;
		}
		public void SetVerticalResourceHeaderTemplateContent(Action<ResourceHeaderTemplateContainer> contentMethod) {
			VerticalResourceHeaderTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalAppointmentTemplateContent(string content) {
			HorizontalAppointmentTemplateContent = content;
		}
		public void SetHorizontalAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(string content) {
			HorizontalSameDayAppointmentTemplateContent = content;
		}
		public void SetHorizontalSameDayAppointmentTemplateContent(Action<AppointmentTemplateContainer> contentMethod) {
			HorizontalSameDayAppointmentTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewSelectorTemplateContent(string content) {
			ToolbarViewSelectorTemplateContent = content;
		}
		public void SetToolbarViewSelectorTemplateContent(Action<ToolbarViewSelectorTemplateContainer> contentMethod) {
			ToolbarViewSelectorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewNavigatorTemplateContent(string content) {
			ToolbarViewNavigatorTemplateContent = content;
		}
		public void SetToolbarViewNavigatorTemplateContent(Action<ToolbarViewNavigatorTemplateContainer> contentMethod) {
			ToolbarViewNavigatorTemplateContentMethod = contentMethod;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(string content) {
			ToolbarViewVisibleIntervalTemplateContent = content;
		}
		public void SetToolbarViewVisibleIntervalTemplateContent(Action<ToolbarViewVisibleIntervalTemplateContainer> contentMethod) {
			ToolbarViewVisibleIntervalTemplateContentMethod = contentMethod;
		}
		public void SetTimelineCellBodyTemplateContent(string content) {
			TimelineCellBodyTemplateContent = content;
		}
		public void SetTimelineCellBodyTemplateContent(Action<TimelineCellBodyTemplateContainer> contentMethod) {
			TimelineCellBodyTemplateContentMethod = contentMethod;
		}
		public void SetTimelineDateHeaderTemplateContent(string content) {
			TimelineDateHeaderTemplateContent = content;
		}
		public void SetTimelineDateHeaderTemplateContent(Action<TimelineDateHeaderTemplateContainer> contentMethod) {
			TimelineDateHeaderTemplateContentMethod = contentMethod;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void SetSelection(TimeInterval interval, Resource resource) {
			base.SetSelection(interval, resource);
		}
		protected override void CheckExistenceControl(ASPxScheduler control) { }
	}
	public class MVCxSchedulerInnerDayView : InnerDayView {
		protected internal MVCxSchedulerInnerDayView(IInnerSchedulerViewOwner owner, IDayViewProperties properties)
			: base(owner, properties) {
		}
		protected override TimeRulerCollection CreateTimeRulerCollection() {
			return new MVCxTimeRulerCollection();
		}
	}
	public class MVCxSchedulerInnerWorkWeekView : InnerWorkWeekView {
		protected internal MVCxSchedulerInnerWorkWeekView(IInnerSchedulerViewOwner owner, IWorkWeekViewProperties properties)
			: base(owner, properties) {
		}
		protected override TimeRulerCollection CreateTimeRulerCollection() {
			return new MVCxTimeRulerCollection();
		}
	}
	public class MVCxSchedulerInnerFullWeekView : InnerFullWeekView {
		public MVCxSchedulerInnerFullWeekView(IInnerSchedulerViewOwner owner, IFullWeekViewProperties properties)
			: base(owner, properties) {			
		}
		protected override TimeRulerCollection CreateTimeRulerCollection() {
			return new MVCxTimeRulerCollection();
		}
	}
}
