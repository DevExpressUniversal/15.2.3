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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.Web.ASPxScheduler.Reporting;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraScheduler;
	using DevExpress.XtraScheduler.iCalendar;
	public delegate object FetchAppointmentsMethod(FetchAppointmentsEventArgs args);
	public delegate void PersistentObjectCancelMethod(PersistentObjectCancelEventArgs e);
	public class SchedulerExtension : ExtensionBase {
		public SchedulerExtension(SchedulerSettings settings)
			: base(settings) {
		}
		public SchedulerExtension(SchedulerSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxScheduler Control {
			get { return (MVCxScheduler)base.Control; }
		}
		protected internal new SchedulerSettings Settings {
			get { return (SchedulerSettings)base.Settings; }
		}
		FetchAppointmentsMethod FetchAppointmentsMethod { get; set; }
		protected override void ApplySettings(SettingsBase settings) {
			Control.BeginInit();
			base.ApplySettings(settings);
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.EditAppointmentRouteValues = Settings.EditAppointmentRouteValues;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnableChangeVisibleIntervalCallbackAnimation = Settings.EnableChangeVisibleIntervalCallbackAnimation;
			Control.EnableChangeVisibleIntervalGestures = Settings.EnableChangeVisibleIntervalGestures;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.WorkDays.Assign(Settings.WorkDays);
			if(Settings.IsActiveViewTypeAssigned)
				Control.ActiveViewType = Settings.ActiveViewType;
			Control.Views.Assign(Settings.Views);
			Control.OptionsView.Assign(Settings.OptionsView);
			Control.Start = Settings.Start;
			Control.GroupType = Settings.GroupType;
			Control.LimitInterval = Settings.LimitInterval != null ? Settings.LimitInterval.Clone() : null;
			Control.EnablePagingGestures = Settings.EnablePagingGestures;
			Control.Storage.Assign(Settings.Storage);
			Control.OptionsBehavior.Assign(Settings.OptionsBehavior);
			Control.OptionsCustomization.Assign(Settings.OptionsCustomization);
			Control.OptionsCookies.Assign(Settings.OptionsCookies);
			Control.OptionsForms.Assign(Settings.OptionsForms);
			Control.OptionsToolTips.Assign(Settings.OptionsToolTips);
			Control.OptionsMenu.Assign(Settings.OptionsMenu);
			Control.OptionsLoadingPanel.Assign(Settings.OptionsLoadingPanel);
			Control.DateNavigatorId = Settings.DateNavigatorExtensionSettings.Name;
			Control.AfterExecuteCallbackCommand += Settings.AfterExecuteCallbackCommand;
			Control.BeforeExecuteCallbackCommand += Settings.BeforeExecuteCallbackCommand;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.AllowAppointmentDelete += Settings.AllowAppointmentDelete;
			Control.AllowAppointmentEdit += Settings.AllowAppointmentEdit;
			Control.AllowAppointmentResize += Settings.AllowAppointmentResize;
			Control.AllowInplaceEditor += Settings.AllowInplaceEditor;
			Control.AppointmentViewInfoCustomizing += Settings.AppointmentViewInfoCustomizing;
			Control.CustomErrorText += Settings.CustomErrorText;
			Control.CustomizeElementStyle += Settings.CustomizeElementStyle;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.FilterResource += Settings.FilterResourceInternal;
			Control.HtmlTimeCellPrepared += Settings.HtmlTimeCellPrepared;
			Control.InitAppointmentDisplayText += Settings.InitAppointmentDisplayText;
			Control.InitAppointmentImages += Settings.InitAppointmentImages;
			Control.InitClientAppointment += Settings.InitClientAppointment;
			Control.PrepareAppointmentFormPopupContainer += Settings.PrepareAppointmentFormPopupContainer;
			Control.PrepareAppointmentInplaceEditorPopupContainer += Settings.PrepareAppointmentInplaceEditorPopupContainer;
			Control.PrepareGotoDateFormPopupContainer += Settings.PrepareGotoDateFormPopupContainer;
			Control.PopupMenuShowing += Settings.PopupMenuShowing;
			Control.ResourceCollectionCleared += Settings.ResourceCollectionCleared;
			Control.ResourceCollectionLoaded += Settings.ResourceCollectionLoaded;
			Control.QueryWorkTime += Settings.QueryWorkTime;
			Control.AppointmentFormShowing += Settings.AppointmentFormShowing;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			AssignFormRenderProperties();
			#region Views
			#region DayView
			Control.Views.DayView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.DayView.HorizontalResourceHeaderTemplateContent, Settings.Views.DayView.HorizontalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.DayView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.DayView.DateHeaderTemplateContent, Settings.Views.DayView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.DayView.Templates.TimeCellBodyTemplate = ContentControlTemplate<TimeCellBodyTemplateContainer>.Create(
				Settings.Views.DayView.TimeCellBodyTemplateContent, Settings.Views.DayView.TimeCellBodyTemplateContentMethod,
				typeof(TimeCellBodyTemplateContainer), true);
			Control.Views.DayView.Templates.AllDayAreaTemplate = ContentControlTemplate<AllDayAreaTemplateContainer>.Create(
				Settings.Views.DayView.AllDayAreaTemplateContent, Settings.Views.DayView.AllDayAreaTemplateContentMethod,
				typeof(AllDayAreaTemplateContainer), true);
			Control.Views.DayView.Templates.TimeRulerHeaderTemplate = ContentControlTemplate<TimeRulerHeaderTemplateContainer>.Create(
				Settings.Views.DayView.TimeRulerHeaderTemplateContent, Settings.Views.DayView.TimeRulerHeaderTemplateContentMethod,
				typeof(TimeRulerHeaderTemplateContainer), true);
			Control.Views.DayView.Templates.RightTopCornerTemplate = ContentControlTemplate<RightTopCornerTemplateContainer>.Create(
				Settings.Views.DayView.RightTopCornerTemplateContent, Settings.Views.DayView.RightTopCornerTemplateContentMethod,
				typeof(RightTopCornerTemplateContainer), true);
			Control.Views.DayView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.DayView.DayOfWeekHeaderTemplateContent, Settings.Views.DayView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.DayView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.DayView.VerticalResourceHeaderTemplateContent, Settings.Views.DayView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.DayView.Templates.VerticalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.DayView.VerticalAppointmentTemplateContent, Settings.Views.DayView.VerticalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.DayView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.DayView.HorizontalAppointmentTemplateContent, Settings.Views.DayView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.DayView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.DayView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.DayView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.DayView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.DayView.ToolbarViewSelectorTemplateContent, Settings.Views.DayView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.DayView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.DayView.ToolbarViewNavigatorTemplateContent, Settings.Views.DayView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.DayView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.DayView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.DayView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			#endregion
			#region WorkWeekView
			Control.Views.WorkWeekView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				 Settings.Views.WorkWeekView.HorizontalResourceHeaderTemplateContent, Settings.Views.WorkWeekView.HorizontalResourceHeaderTemplateContentMethod,
				 typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.WorkWeekView.DateHeaderTemplateContent, Settings.Views.WorkWeekView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.TimeCellBodyTemplate = ContentControlTemplate<TimeCellBodyTemplateContainer>.Create(
				Settings.Views.WorkWeekView.TimeCellBodyTemplateContent, Settings.Views.WorkWeekView.TimeCellBodyTemplateContentMethod,
				typeof(TimeCellBodyTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.AllDayAreaTemplate = ContentControlTemplate<AllDayAreaTemplateContainer>.Create(
				Settings.Views.WorkWeekView.AllDayAreaTemplateContent, Settings.Views.WorkWeekView.AllDayAreaTemplateContentMethod,
				typeof(AllDayAreaTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.TimeRulerHeaderTemplate = ContentControlTemplate<TimeRulerHeaderTemplateContainer>.Create(
				Settings.Views.WorkWeekView.TimeRulerHeaderTemplateContent, Settings.Views.WorkWeekView.TimeRulerHeaderTemplateContentMethod,
				typeof(TimeRulerHeaderTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.RightTopCornerTemplate = ContentControlTemplate<RightTopCornerTemplateContainer>.Create(
				Settings.Views.WorkWeekView.RightTopCornerTemplateContent, Settings.Views.WorkWeekView.RightTopCornerTemplateContentMethod,
				typeof(RightTopCornerTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.WorkWeekView.DayOfWeekHeaderTemplateContent, Settings.Views.WorkWeekView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.WorkWeekView.VerticalResourceHeaderTemplateContent, Settings.Views.WorkWeekView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.VerticalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.WorkWeekView.VerticalAppointmentTemplateContent, Settings.Views.WorkWeekView.VerticalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.WorkWeekView.HorizontalAppointmentTemplateContent, Settings.Views.WorkWeekView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.WorkWeekView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.WorkWeekView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.WorkWeekView.ToolbarViewSelectorTemplateContent, Settings.Views.WorkWeekView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.WorkWeekView.ToolbarViewNavigatorTemplateContent, Settings.Views.WorkWeekView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.WorkWeekView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.WorkWeekView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.WorkWeekView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			#endregion
			#region WeekView
			Control.Views.WeekView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.WeekView.HorizontalResourceHeaderTemplateContent, Settings.Views.WeekView.HorizontalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.WeekView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.WeekView.DateHeaderTemplateContent, Settings.Views.WeekView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.WeekView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.WeekView.DayOfWeekHeaderTemplateContent, Settings.Views.WeekView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.WeekView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.WeekView.VerticalResourceHeaderTemplateContent, Settings.Views.WeekView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.WeekView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.WeekView.HorizontalAppointmentTemplateContent, Settings.Views.WeekView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.WeekView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.WeekView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.WeekView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.WeekView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.WeekView.ToolbarViewSelectorTemplateContent, Settings.Views.WeekView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.WeekView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.WeekView.ToolbarViewNavigatorTemplateContent, Settings.Views.WeekView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.WeekView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.WeekView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.WeekView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			Control.Views.WeekView.Templates.DateCellHeaderTemplate = ContentControlTemplate<DateCellHeaderTemplateContainer>.Create(
				Settings.Views.WeekView.DateCellHeaderTemplateContent, Settings.Views.WeekView.DateCellHeaderTemplateContentMethod,
				typeof(DateCellHeaderTemplateContainer), true);
			Control.Views.WeekView.Templates.DateCellBodyTemplate = ContentControlTemplate<DateCellBodyTemplateContainer>.Create(
				Settings.Views.WeekView.DateCellBodyTemplateContent, Settings.Views.WeekView.DateCellBodyTemplateContentMethod,
				typeof(DateCellBodyTemplateContainer), true);
			#endregion
			#region FullWeekView
			Control.Views.FullWeekView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				 Settings.Views.FullWeekView.HorizontalResourceHeaderTemplateContent, Settings.Views.FullWeekView.HorizontalResourceHeaderTemplateContentMethod,
				 typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.FullWeekView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.FullWeekView.DateHeaderTemplateContent, Settings.Views.FullWeekView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.FullWeekView.Templates.TimeCellBodyTemplate = ContentControlTemplate<TimeCellBodyTemplateContainer>.Create(
				Settings.Views.FullWeekView.TimeCellBodyTemplateContent, Settings.Views.FullWeekView.TimeCellBodyTemplateContentMethod,
				typeof(TimeCellBodyTemplateContainer), true);
			Control.Views.FullWeekView.Templates.AllDayAreaTemplate = ContentControlTemplate<AllDayAreaTemplateContainer>.Create(
				Settings.Views.FullWeekView.AllDayAreaTemplateContent, Settings.Views.FullWeekView.AllDayAreaTemplateContentMethod,
				typeof(AllDayAreaTemplateContainer), true);
			Control.Views.FullWeekView.Templates.TimeRulerHeaderTemplate = ContentControlTemplate<TimeRulerHeaderTemplateContainer>.Create(
				Settings.Views.FullWeekView.TimeRulerHeaderTemplateContent, Settings.Views.FullWeekView.TimeRulerHeaderTemplateContentMethod,
				typeof(TimeRulerHeaderTemplateContainer), true);
			Control.Views.FullWeekView.Templates.RightTopCornerTemplate = ContentControlTemplate<RightTopCornerTemplateContainer>.Create(
				Settings.Views.FullWeekView.RightTopCornerTemplateContent, Settings.Views.FullWeekView.RightTopCornerTemplateContentMethod,
				typeof(RightTopCornerTemplateContainer), true);
			Control.Views.FullWeekView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.FullWeekView.DayOfWeekHeaderTemplateContent, Settings.Views.FullWeekView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.FullWeekView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.FullWeekView.VerticalResourceHeaderTemplateContent, Settings.Views.FullWeekView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.FullWeekView.Templates.VerticalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.FullWeekView.VerticalAppointmentTemplateContent, Settings.Views.FullWeekView.VerticalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.FullWeekView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.FullWeekView.HorizontalAppointmentTemplateContent, Settings.Views.FullWeekView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.FullWeekView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.FullWeekView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.FullWeekView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.FullWeekView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.FullWeekView.ToolbarViewSelectorTemplateContent, Settings.Views.FullWeekView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.FullWeekView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.FullWeekView.ToolbarViewNavigatorTemplateContent, Settings.Views.FullWeekView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.FullWeekView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.FullWeekView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.FullWeekView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			#endregion
			#region MonthView
			Control.Views.MonthView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.MonthView.HorizontalResourceHeaderTemplateContent, Settings.Views.MonthView.HorizontalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.MonthView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.MonthView.DateHeaderTemplateContent, Settings.Views.MonthView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.MonthView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.MonthView.DayOfWeekHeaderTemplateContent, Settings.Views.MonthView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.MonthView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.MonthView.VerticalResourceHeaderTemplateContent, Settings.Views.MonthView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.MonthView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.MonthView.HorizontalAppointmentTemplateContent, Settings.Views.MonthView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.MonthView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.MonthView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.MonthView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.MonthView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.MonthView.ToolbarViewSelectorTemplateContent, Settings.Views.MonthView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.MonthView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.MonthView.ToolbarViewNavigatorTemplateContent, Settings.Views.MonthView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.MonthView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.MonthView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.MonthView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			Control.Views.MonthView.Templates.DateCellHeaderTemplate = ContentControlTemplate<DateCellHeaderTemplateContainer>.Create(
				Settings.Views.MonthView.DateCellHeaderTemplateContent, Settings.Views.MonthView.DateCellHeaderTemplateContentMethod,
				typeof(DateCellHeaderTemplateContainer), true);
			Control.Views.MonthView.Templates.DateCellBodyTemplate = ContentControlTemplate<DateCellBodyTemplateContainer>.Create(
				Settings.Views.MonthView.DateCellBodyTemplateContent, Settings.Views.MonthView.DateCellBodyTemplateContentMethod,
				typeof(DateCellBodyTemplateContainer), true);
			#endregion
			#region TimelineView
			Control.Views.TimelineView.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.TimelineView.HorizontalResourceHeaderTemplateContent, Settings.Views.TimelineView.HorizontalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.TimelineView.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.Views.TimelineView.DateHeaderTemplateContent, Settings.Views.TimelineView.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Views.TimelineView.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.Views.TimelineView.DayOfWeekHeaderTemplateContent, Settings.Views.TimelineView.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Views.TimelineView.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.Views.TimelineView.VerticalResourceHeaderTemplateContent, Settings.Views.TimelineView.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Views.TimelineView.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.TimelineView.HorizontalAppointmentTemplateContent, Settings.Views.TimelineView.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.TimelineView.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.Views.TimelineView.HorizontalSameDayAppointmentTemplateContent, Settings.Views.TimelineView.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Views.TimelineView.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.Views.TimelineView.ToolbarViewSelectorTemplateContent, Settings.Views.TimelineView.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Views.TimelineView.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.Views.TimelineView.ToolbarViewNavigatorTemplateContent, Settings.Views.TimelineView.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Views.TimelineView.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.Views.TimelineView.ToolbarViewVisibleIntervalTemplateContent, Settings.Views.TimelineView.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
			Control.Views.TimelineView.Templates.TimelineCellBodyTemplate = ContentControlTemplate<TimelineCellBodyTemplateContainer>.Create(
				Settings.Views.TimelineView.TimelineCellBodyTemplateContent, Settings.Views.TimelineView.TimelineCellBodyTemplateContentMethod,
				typeof(TimelineCellBodyTemplateContainer), true);
			Control.Views.TimelineView.Templates.TimelineDateHeaderTemplate = ContentControlTemplate<TimelineDateHeaderTemplateContainer>.Create(
				Settings.Views.TimelineView.TimelineDateHeaderTemplateContent, Settings.Views.TimelineView.TimelineDateHeaderTemplateContentMethod,
				typeof(TimelineDateHeaderTemplateContainer), true);
			#endregion
			#endregion
			Control.Templates.HorizontalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
			   Settings.HorizontalResourceHeaderTemplateContent, Settings.HorizontalResourceHeaderTemplateContentMethod,
			   typeof(ResourceHeaderTemplateContainer), true);
			Control.Templates.DateHeaderTemplate = ContentControlTemplate<DateHeaderTemplateContainer>.Create(
				Settings.DateHeaderTemplateContent, Settings.DateHeaderTemplateContentMethod,
				typeof(DateHeaderTemplateContainer), true);
			Control.Templates.DayOfWeekHeaderTemplate = ContentControlTemplate<DayOfWeekHeaderTemplateContainer>.Create(
				Settings.DayOfWeekHeaderTemplateContent, Settings.DayOfWeekHeaderTemplateContentMethod,
				typeof(DayOfWeekHeaderTemplateContainer), true);
			Control.Templates.VerticalResourceHeaderTemplate = ContentControlTemplate<ResourceHeaderTemplateContainer>.Create(
				Settings.VerticalResourceHeaderTemplateContent, Settings.VerticalResourceHeaderTemplateContentMethod,
				typeof(ResourceHeaderTemplateContainer), true);
			Control.Templates.HorizontalAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.HorizontalAppointmentTemplateContent, Settings.HorizontalAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Templates.HorizontalSameDayAppointmentTemplate = ContentControlTemplate<AppointmentTemplateContainer>.Create(
				Settings.HorizontalSameDayAppointmentTemplateContent, Settings.HorizontalSameDayAppointmentTemplateContentMethod,
				typeof(AppointmentTemplateContainer), true);
			Control.Templates.ToolbarViewSelectorTemplate = ContentControlTemplate<ToolbarViewSelectorTemplateContainer>.Create(
				Settings.ToolbarViewSelectorTemplateContent, Settings.ToolbarViewSelectorTemplateContentMethod,
				typeof(ToolbarViewSelectorTemplateContainer), true);
			Control.Templates.ToolbarViewNavigatorTemplate = ContentControlTemplate<ToolbarViewNavigatorTemplateContainer>.Create(
				Settings.ToolbarViewNavigatorTemplateContent, Settings.ToolbarViewNavigatorTemplateContentMethod,
				typeof(ToolbarViewNavigatorTemplateContainer), true);
			Control.Templates.ToolbarViewVisibleIntervalTemplate = ContentControlTemplate<ToolbarViewVisibleIntervalTemplateContainer>.Create(
				Settings.ToolbarViewVisibleIntervalTemplateContent, Settings.ToolbarViewVisibleIntervalTemplateContentMethod,
				typeof(ToolbarViewVisibleIntervalTemplateContainer), true);
		}
		protected void AssignFormRenderProperties() {
			Control.AppointmentFormTemplateControl = CreateFormTemplateControl<AppointmentFormTemplateContainer>(
			   Settings.OptionsForms.AppointmentFormTemplateContent, Settings.OptionsForms.AppointmentFormTemplateContentMethod);
			Control.AppointmentInplaceEditorFormTemplateControl = CreateFormTemplateControl<AppointmentInplaceEditorTemplateContainer>(
			   Settings.OptionsForms.AppointmentInplaceEditorFormTemplateContent, Settings.OptionsForms.AppointmentInplaceEditorFormTemplateContentMethod);
			Control.GotoDateFormTemplateControl = CreateFormTemplateControl<GotoDateFormTemplateContainer>(
				Settings.OptionsForms.GotoDateFormTemplateContent, Settings.OptionsForms.GotoDateFormTemplateContentMethod);
			Control.RecurrentAppointmentDeleteFormControl = CreateFormTemplateControl<RecurrentAppointmentDeleteFormTemplateContainer>(
				Settings.OptionsForms.RecurrentAppointmentDeleteFormTemplateContent, Settings.OptionsForms.RecurrentAppointmentDeleteFormTemplateContentMethod);
			Control.RecurrentAppointmentEditFormTemplateContentControl = CreateFormTemplateControl<RecurrentAppointmentDeleteFormTemplateContainer>(
				Settings.OptionsForms.RecurrentAppointmentEditFormTemplateContent, Settings.OptionsForms.RecurrentAppointmentEditFormTemplateContentMethod);
			Control.RemindersFormTemplateContentControl = CreateFormTemplateControl<RemindersFormTemplateContainer>(
				Settings.OptionsForms.RemindersFormTemplateContent, Settings.OptionsForms.RemindersFormTemplateContentMethod);
			Control.AppointmentToolTipTemplateControl = ContentControl.Create(Settings.OptionsToolTips.AppointmentToolTipTemplateContent, Settings.OptionsToolTips.AppointmentToolTipTemplateContentMethod);
			Control.AppointmentDragToolTipTemplateControl = ContentControl.Create(Settings.OptionsToolTips.AppointmentDragToolTipTemplateContent, Settings.OptionsToolTips.AppointmentDragToolTipTemplateContentMethod);
			Control.SelectionToolTipTemplateControl = ContentControl.Create(Settings.OptionsToolTips.SelectionToolTipTemplateContent, Settings.OptionsToolTips.SelectionToolTipTemplateContentMethod);
		}
		Control CreateFormTemplateControl<T>(string content, Action<T> contentMethod) where T : Control {
			return !string.IsNullOrEmpty(content) || contentMethod != null ? ContentControl<T>.Create(content, contentMethod, null, typeof(T), true) : null;
		}
		public SchedulerExtension Bind(object appointmentDataObject) {
			return Bind(appointmentDataObject, null);
		}
		public SchedulerExtension Bind(object appointmentDataObject, object resourceDataObject) {
			Control.AppointmentDataSource = appointmentDataObject;
			Control.ResourceDataSource = resourceDataObject;
			Control.DataBind();
			return this;
		}
		public SchedulerExtension Bind(FetchAppointmentsMethod method) {
			return Bind(method, null);
		}
		public SchedulerExtension Bind(FetchAppointmentsMethod fetchAppointmentsMethod, object resourceDataObject) {
			Control.ResourceDataSource = resourceDataObject;
			Control.DataBind();
			FetchAppointmentsMethod = fetchAppointmentsMethod;
			Control.FetchAppointments += FetchAppointmentsEventHandler;
			return this;
		}
		TimeInterval fetchInterval = TimeInterval.Empty;
		void FetchAppointmentsEventHandler(object source, FetchAppointmentsEventArgs e) {
			if(this.fetchInterval.Contains(e.Interval) || e.Interval.Start == DateTime.MinValue)
				return;
			this.fetchInterval = e.Interval;
			object appointmentDataSource = FetchAppointmentsMethod != null ? FetchAppointmentsMethod(e) : null;
			MVCxSchedulerStorage storage = source as MVCxSchedulerStorage;
			if (storage != null && storage.Control != null)
				storage.Control.AppointmentDataSource = appointmentDataSource;
		}
		public SchedulerExtension Bind(PersistentObjectCancelMethod filterAppointmentMethod, PersistentObjectCancelMethod filterResourceMethod) {
			if(filterAppointmentMethod != null)
				Control.FilterAppointment += (sender, e) => {
					filterAppointmentMethod(e);
				};
			if(filterResourceMethod != null)
				Control.FilterResource += (sender, e) => {
					filterResourceMethod(e);
				};
			return this;
		}
		#region ExportToICalendar
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject) {
			return ExportToICalendar(settings, appointmentDataObject, null);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, string fileName) {
			return ExportToICalendar(settings, appointmentDataObject, null, fileName);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, bool saveAsFile) {
			return ExportToICalendar(settings, appointmentDataObject, null, saveAsFile);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, object resourceDataObject) {
			return ExportToICalendar(settings, appointmentDataObject, resourceDataObject, null);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, object resourceDataObject, string fileName) {
			return ExportToICalendar(settings, appointmentDataObject, resourceDataObject, fileName, true);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, object resourceDataObject, bool saveAsFile) {
			return ExportToICalendar(settings, appointmentDataObject, resourceDataObject, null, saveAsFile);
		}
		public static ActionResult ExportToICalendar(SchedulerSettings settings, object appointmentDataObject, object resourceDataObject, string fileName, bool saveAsFile) {
			SchedulerExtension extension = CreateExtension(settings, appointmentDataObject, resourceDataObject);
			iCalendarExporter exporter = CreateICalendarExporter(extension, settings.SettingsExport.ICalendar);
			return ExportUtils.Export(extension, exporter.Export, fileName, saveAsFile, "ics");
		}
		static iCalendarExporter CreateICalendarExporter(SchedulerExtension extension, MVCxICalendarExportSettings exporterSettings) {
			iCalendarExporter exporter = new iCalendarExporter(extension.Control.Storage);
			exporterSettings.AssignToAppointmentExchanger(exporter);
			return exporter;
		}
		#endregion
		#region ImportFromICalendar
		public static T[] ImportFromICalendar<T>(SchedulerSettings settings, Stream stream) {
			iCalendarImporter importer = CreateICalendarImporter(settings);
			MVCxSchedulerStorage storage = (MVCxSchedulerStorage)importer.Storage;
			ArrayList importedAppointments = new ArrayList();
			importer.Import(stream);
			for (int index = 0; index < storage.Appointments.Count; index++) {
				importedAppointments.Add(ConvertAppointmentToDataObjectFormat<T>(storage.Appointments[index], storage));
				if (storage.Appointments[index].IsRecurring && storage.Appointments[index].GetExceptions().Count > 0)
					importedAppointments.AddRange(CastAppointmentsToCustomType<T>(storage.Appointments[index].GetExceptions(), storage));
			}
			return (T[])(importedAppointments.ToArray(typeof(T))); ;
		}
		static iCalendarImporter CreateICalendarImporter(SchedulerSettings settings) {
			SchedulerExtension extension = CreateExtension(settings, null, null);
			iCalendarImporter importer = new iCalendarImporter(extension.Control.Storage);
			settings.SettingsImport.ICalendar.AssignToAppointmentExchanger(importer);
			return importer;
		}
		#endregion
		protected internal static SchedulerExtension CreateExtension(SchedulerSettings settings, FetchAppointmentsMethod fetchAppointmentsMethod, object resourceDataObject) {
			return CreateExtensionCore(settings, ext => ext.Bind(fetchAppointmentsMethod, resourceDataObject));
		}
		protected internal static SchedulerExtension CreateExtension(SchedulerSettings settings, object appointmentDataObject, object resourceDataObject) {
			return CreateExtensionCore(settings, ext => ext.Bind(appointmentDataObject, resourceDataObject));
		}
		static SchedulerExtension CreateExtensionCore(SchedulerSettings settings, Action<SchedulerExtension> bind) {
			SchedulerExtension extension = ExtensionsFactory.InstanceInternal.Scheduler(settings);
			bind(extension);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
		public static ASPxSchedulerControlPrintAdapter GetPrintAdapter(SchedulerSettings settings, object appointmentDataObject) {
			return GetPrintAdapter(settings, appointmentDataObject, null);
		}
		public static ASPxSchedulerControlPrintAdapter GetPrintAdapter(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			SchedulerExtension extension = CreateExtension(settings, appointmentDataObject, recourceDataObject);
			return new ASPxSchedulerControlPrintAdapter(extension.Control);
		}
		public static T ConvertAppointment<T>(Appointment appointment, MVCxAppointmentStorage appointmentStorage) {
			if (appointment == null || appointmentStorage == null)
				return default(T);
			MVCxSchedulerStorage storage = new MVCxSchedulerStorage();
			storage.Appointments.Assign(appointmentStorage);
			return ConvertAppointmentToDataObjectFormat<T>(appointment, storage);
		}
		[Obsolete("This method is now obsolete. Use the GetAppointmentsToInsert method instead.")]
		public static T GetAppointmentToInsert<T>(SchedulerSettings settings, object appointmentDataObject) {
			return GetAppointmentToInsert<T>(settings, appointmentDataObject, null);
		}
		[Obsolete("This method is now obsolete. Use the GetAppointmentsToInsert method instead.")]
		public static T GetAppointmentToInsert<T>(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, appointmentDataObject, recourceDataObject);
			return GetAppointmentsToInsertInternal<T>(extension).FirstOrDefault();
		}
		[Obsolete("This method is now obsolete. Use the GetAppointmentsToInsert method instead.")]
		public static T GetAppointmentToInsert<T>(string schedulerName, object appointmentDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetAppointmentToInsert<T>(schedulerName, appointmentDataObject, null, appointmentStorage, resourceStorage);
		}
		[Obsolete("This method is now obsolete. Use the GetAppointmentsToInsert method instead.")]
		public static T GetAppointmentToInsert<T>(string schedulerName, object appointmentDataObject, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, appointmentDataObject, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToInsertInternal<T>(extension).FirstOrDefault();
		}
		public static T[] GetAppointmentsToInsert<T>(SchedulerSettings settings, object appointmentDataObject) {
			return GetAppointmentsToInsert<T>(settings, appointmentDataObject, null);
		}
		public static T[] GetAppointmentsToInsert<T>(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, appointmentDataObject, recourceDataObject);
			return GetAppointmentsToInsertInternal<T>(extension);
		}
		public static T[] GetAppointmentsToInsert<T>(string schedulerName, object appointmentDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetAppointmentsToInsert<T>(schedulerName, appointmentDataObject, null, appointmentStorage, resourceStorage);
		}
		public static T[] GetAppointmentsToInsert<T>(string schedulerName, object appointmentDataObject, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, appointmentDataObject, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToInsertInternal<T>(extension);
		}
		public static T[] GetAppointmentsToInsert<T>(SchedulerSettings settings, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, fetchAppointmentsMethod, recourceDataObject);
			return GetAppointmentsToInsertInternal<T>(extension);
		}
		public static T[] GetAppointmentsToInsert<T>(string schedulerName, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, fetchAppointmentsMethod, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToInsertInternal<T>(extension);
		}
		static T[] GetAppointmentsToInsertInternal<T>(SchedulerExtension extension) {
			IEditableAppointments command = GetCommandAfterExecuting(extension);
			return CastAppointmentsToCustomType<T>(command.NewAppointments, command.Storage);
		}
		public static T[] GetAppointmentsToUpdate<T>(SchedulerSettings settings, object appointmentDataObject) {
			return GetAppointmentsToUpdate<T>(settings, appointmentDataObject, null);
		}
		public static T[] GetAppointmentsToUpdate<T>(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, appointmentDataObject, recourceDataObject);
			return GetAppointmentsToUpdateInternal<T>(extension);
		}
		public static T[] GetAppointmentsToUpdate<T>(string schedulerName, object appointmentDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetAppointmentsToUpdate<T>(schedulerName, appointmentDataObject, null, appointmentStorage, resourceStorage);
		}
		public static T[] GetAppointmentsToUpdate<T>(string schedulerName, object appointmentDataObject, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, appointmentDataObject, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToUpdateInternal<T>(extension);
		}
		public static T[] GetAppointmentsToUpdate<T>(SchedulerSettings settings, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, fetchAppointmentsMethod, recourceDataObject);
			return GetAppointmentsToUpdateInternal<T>(extension);
		}
		public static T[] GetAppointmentsToUpdate<T>(string schedulerName, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, fetchAppointmentsMethod, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToUpdateInternal<T>(extension);
		}
		static T[] GetAppointmentsToUpdateInternal<T>(SchedulerExtension extension) {
			IEditableAppointments command = GetCommandAfterExecuting(extension);
			return CastAppointmentsToCustomType<T>(command.UpdatedAppointments, command.Storage);
		}
		public static T[] GetAppointmentsToRemove<T>(SchedulerSettings settings, object appointmentDataObject) {
			return GetAppointmentsToRemove<T>(settings, appointmentDataObject, null);
		}
		public static T[] GetAppointmentsToRemove<T>(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, appointmentDataObject, recourceDataObject);
			return GetAppointmentsToRemoveInternal<T>(extension);
		}
		public static T[] GetAppointmentsToRemove<T>(string schedulerName, object appointmentDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetAppointmentsToRemove<T>(schedulerName, appointmentDataObject, null, appointmentStorage, resourceStorage);
		}
		public static T[] GetAppointmentsToRemove<T>(string schedulerName, object appointmentDataObject, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, appointmentDataObject, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToRemoveInternal<T>(extension);
		}
		public static T[] GetAppointmentsToRemove<T>(SchedulerSettings settings, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject) {
			SchedulerExtension extension = GetEditableExtension(settings, fetchAppointmentsMethod, recourceDataObject);
			return GetAppointmentsToRemoveInternal<T>(extension);
		}
		public static T[] GetAppointmentsToRemove<T>(string schedulerName, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			SchedulerExtension extension = GetEditableExtension(schedulerName, fetchAppointmentsMethod, recourceDataObject, appointmentStorage, resourceStorage);
			return GetAppointmentsToRemoveInternal<T>(extension);
		}
		static T[] GetAppointmentsToRemoveInternal<T>(SchedulerExtension extension) {
			IEditableAppointments command = GetCommandAfterExecuting(extension);
			return CastAppointmentsToCustomType<T>(command.DeletedAppointments, command.Storage);
		}
		static T[] CastAppointmentsToCustomType<T>(IEnumerable<Appointment> appointments, MVCxSchedulerStorage storage) {
			ArrayList editableAppointments = new ArrayList();
			foreach (Appointment appointment in appointments) {
				T appointmentObject = ConvertAppointmentToDataObjectFormat<T>(appointment, storage);
				editableAppointments.Add(appointmentObject);
			}
			return (T[])(editableAppointments.ToArray(typeof(T)));
		}
		const string CommandPostfix = "Command";
		IEditableAppointments LastEditableCommand { get; set; }
		static IEditableAppointments GetCommandAfterExecuting(SchedulerExtension extension) {
			if (extension.LastEditableCommand == null) {
				int separatorPosition = MvcUtils.CallbackArgument.IndexOf(":");
				string eventArgument = MvcUtils.CallbackArgument.Substring(separatorPosition + 1);
				CallbackCommandInfo commandInfo = extension.Control.CreateCommandInfo(eventArgument);
				IEditableAppointments command = null;
				if (commandInfo != null) {
					command = extension.Control.CallbackCommandManager.GetCommandForEditableActions(commandInfo) as IEditableAppointments;
					if (command != null) {
						command.Execute(commandInfo.Parameters);
						extension.LastEditableCommand = command;
					}
				}
			}
			return extension.LastEditableCommand;
		}
		static SchedulerExtension GetEditableExtension(string name, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetEditableExtensionCore(name, ext => ext.Bind(fetchAppointmentsMethod, recourceDataObject), appointmentStorage, resourceStorage);
		}
		static SchedulerExtension GetEditableExtension(string name, object appointmentDataObject, object recourceDataObject, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			return GetEditableExtensionCore(name, ext => ext.Bind(appointmentDataObject, recourceDataObject), appointmentStorage, resourceStorage);
		}
		static SchedulerExtension GetEditableExtensionCore(string name, Action<SchedulerExtension> bind, MVCxAppointmentStorage appointmentStorage, MVCxResourceStorage resourceStorage) {
			if (IsExistSchedulerForEditableCommnad(name))
				return (SchedulerExtension)ExtensionManager.RequestExtensions[ExtensionType.Scheduler][name + "_" + CommandPostfix];
			SchedulerExtension scheduler = (SchedulerExtension)ExtensionManager.GetExtension(ExtensionType.Scheduler, name, ExtensionCacheMode.Request, CommandPostfix);
			if (appointmentStorage != null)
				scheduler.Control.Storage.Appointments.Assign(appointmentStorage);
			if (resourceStorage != null)
				scheduler.Control.Storage.Resources.Assign(resourceStorage);
			string activeViewTypeString = HttpUtils.GetValueFromRequest(string.Format("{0}_activeViewType", name));
			string startString = HttpUtils.GetValueFromRequest(string.Format("{0}_start", name));
			string resourceSharingStrings = HttpUtils.GetValueFromRequest(string.Format("{0}_resourceSharing", name));
			if (!string.IsNullOrEmpty(activeViewTypeString))
				scheduler.Control.ActiveViewType = (SchedulerViewType)Enum.Parse(typeof(SchedulerViewType), activeViewTypeString);
			if (!string.IsNullOrEmpty(startString))
				scheduler.Control.Start = HtmlConvertor.FromJSON<System.DateTime>(startString);
			if (!string.IsNullOrEmpty(resourceSharingStrings))
				scheduler.Control.Storage.Appointments.ResourceSharing = bool.Parse(resourceSharingStrings);
			bind(scheduler);
			scheduler.AssignFormRenderProperties();
			scheduler.PrepareControl();
			scheduler.LoadPostData();
			return scheduler;
		}
		static SchedulerExtension GetEditableExtension(SchedulerSettings settings, object appointmentDataObject, object recourceDataObject) {
			return GetEditableExtensionCore(settings, () => CreateExtension(settings, appointmentDataObject, recourceDataObject));
		}
		static SchedulerExtension GetEditableExtension(SchedulerSettings settings, FetchAppointmentsMethod fetchAppointmentsMethod, object recourceDataObject) {
			return GetEditableExtensionCore(settings, () => CreateExtension(settings, fetchAppointmentsMethod, recourceDataObject));
		}
		static SchedulerExtension GetEditableExtensionCore(SchedulerSettings settings, Func<SchedulerExtension> createExtension) {
			if (IsExistSchedulerForEditableCommnad(settings.Name))
				return (SchedulerExtension)ExtensionManager.RequestExtensions[ExtensionType.Scheduler][settings.Name + "_" + CommandPostfix];
			SchedulerExtension extension = createExtension();
			extension.AssignFormRenderProperties();
			ExtensionManager.AddExtensionTypeToCache(ExtensionManager.RequestExtensions, ExtensionType.Scheduler);
			ExtensionManager.RequestExtensions[ExtensionType.Scheduler].Add(settings.Name + "_" + CommandPostfix, extension);
			return extension;
		}
		static bool IsExistSchedulerForEditableCommnad(string name) {
			return ExtensionManager.RequestExtensions.ContainsKey(ExtensionType.Scheduler) &&
				ExtensionManager.RequestExtensions[ExtensionType.Scheduler].ContainsKey(name + "_" + CommandPostfix);
		}
		static T ConvertAppointmentToDataObjectFormat<T>(Appointment appointment, MVCxSchedulerStorage storage) {
			if (appointment == null)
				return default(T);
			T editableAppointment = typeof(T).IsClass ? (T)Activator.CreateInstance(typeof(T)) : default(T);
			foreach (MappingBase mapping in storage.Appointments.ActualMappings) {
				PropertyInfo propertyInfo = editableAppointment.GetType().GetProperty(mapping.Member);
				if(propertyInfo != null && propertyInfo.CanWrite && !propertyInfo.PropertyType.IsArray) {
					object value = ExtensionValueProviderBase.ConvertValue(mapping.GetValue(appointment), propertyInfo.PropertyType);
					propertyInfo.SetValue(editableAppointment, value, null);
				}
			}
			return editableAppointment;
		}
		public static bool HasErrors(string name) {
			var statusInfo = MVCxSchedulerCommandHelper.GetStatusInfo(name);
			return statusInfo != null && statusInfo.Count > 0;
		}
		public SchedulerExtension SetErrorText(string errorText) {
			if(!string.IsNullOrEmpty(errorText))
				SchedulerStatusInfoHelper.AddError(Control, errorText);
			return this;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxScheduler(ViewContext);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			RegisterRelatedExtensions();
			Control.EnsureClientStateLoaded();
			Control.EndInit();
		}
		protected void RegisterRelatedExtensions() {
			if (IsExistDateNavigatorExtension && IsCallback())
				DateNavigatorExtension.RegisterControlInScheduler(Settings);
		}
		bool IsExistDateNavigatorExtension {
			get {
				return !string.IsNullOrEmpty(Settings.DateNavigatorExtensionSettings.Name) && ExtensionsFactory.RenderedExtensions.ContainsKey(Settings.Name);
			}
		}
	}
}
namespace DevExpress.Web.Mvc.Internal{
	public class SchedulerAppointmentEditValues {
		const string
			EditValuesContextRequestKey = "DXMVCSchedulerAptFormValues",
			EditValuesContextCacheKey = "DXSchedulerAptFormValues";
		static SchedulerAppointmentEditValues current;
		public static SchedulerAppointmentEditValues Current {
			get {
				if(current == null)
					current = new SchedulerAppointmentEditValues();
				return current;
			}
		}
		public T GetValue<T>(string fieldName) {
			object valueFromDXExtension;
			if(!EditValues.Contains(fieldName) && TryGetDxExtensionValue(fieldName, out valueFromDXExtension))
				EditValues.Add(fieldName, valueFromDXExtension);
			var value = EditorValueProvider.ConvertValue(EditValues[fieldName], typeof(T));
			return value != null ? (T)value : default(T);
		}
		public bool Contains(string fieldName) {
			object valueFromDXExtension;
			if(EditValues.Contains(fieldName))
				return true;
			return TryGetDxExtensionValue(fieldName, out valueFromDXExtension);
		}
		protected bool TryGetDxExtensionValue(string fieldName, out object value) {
			ModelBindingContext bindingContext = CreateBindingContext(fieldName, null);
			return ExtensionValueProvidersFactory.TryGetValue(bindingContext, out value);
		}
		ModelBindingContext CreateBindingContext(string fieldName, Type propertyType) {
			var bindingContext = new ModelBindingContext();
			bindingContext.ModelName = fieldName;
			if(propertyType != null)
				bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, propertyType);
			bindingContext.ValueProvider = new FormCollection(HttpContext.Current.Request.Params);
			return bindingContext;
		}
		IDictionary EditValues {
			get {
				IDictionary values = HttpUtils.GetContextValue<IDictionary>(EditValuesContextCacheKey, null);
				if(values == null) {
					EditorValueProvider provider = new EditorValueProvider();
					values = provider.GetEditorValues(EditValuesContextRequestKey);
					HttpUtils.SetContextValue(EditValuesContextCacheKey, values);
				}
				return values;
			}
		}
	}
}
