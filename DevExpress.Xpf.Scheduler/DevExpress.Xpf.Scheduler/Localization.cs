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
using System.Resources;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using DevExpress.XtraScheduler;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerControlStringId
	public enum SchedulerControlStringId {
		Caption_GotoDate,
		Caption_DeleteRecurrentApt,
		Caption_OpenRecurrentApt,
		Caption_TimeRuler,
		Form_RecurrentAppointmentAction_DeleteSeries,
		Form_RecurrentAppointmentAction_DeleteOccurrence,
		Form_RecurrentAppointmentAction_EditSeries,
		Form_RecurrentAppointmentAction_EditOccurrence,
		Form_DeleteRecurrentAppointmentFormMessage,
		Form_EditRecurrentAppointmentFormMessage,
		ButtonCaption_DismissAll,
		ButtonCaption_Dismiss,
		ButtonCaption_OpenItem,
		ButtonCaption_Snooze,
		Form_ClickSnoozeToBeReminderAgainIn,
		Form_Subject,
		Form_Location,
		Form_StartTime,
		Form_StartDate,
		Form_EndTime,
		Form_EndDate,
		Form_Recurrence,
		Form_Resource,
		Form_Label,
		Form_ShowTimeAs,
		Form_Reminder,
		Form_Description,
		Form_AllDayEvent,
		Form_Date,
		Form_ShowIn,
		ButtonCaption_OK,
		ButtonCaption_SaveAndClose,
		ButtonCaption_Cancel,
		ButtonCaption_Delete,
		ButtonCaption_Recurrence,
		ButtonCaption_TimeZones,
		ButtonCaption_Save,
		ButtonCaption_Undo,
		ButtonCaption_Redo,
		ButtonCaption_Next,
		ButtonCaption_Previous,
		Form_DayOrDays,
		Form_Every,
		Form_EveryWeekday,
		Form_Day,
		Form_OfEvery,
		Form_MonthOrMonths,
		Form_The,
		Form_RecurrencePattern,
		Form_RangeOfRecurrence,
		Form_Occurrences,
		Form_Start,
		Form_NoEndDate,
		Form_EndAfter,
		Form_EndBy,
		Form_RecurEvery,
		Form_WeekOrWeeksOn,
		Form_On,
		Form_OnThe,
		Form_of,
		Form_DayNumber,
		Form_DayOfWeek,
		Form_TimeZone,
		Form_CurrentTime,
		Form_AdjustForDaylightSavingTime,
		Form_EveryEditableTextDay,
		Form_AccessibleText_RecurEveryEditableTextWeek,
		Caption_PageCategoryAppointmentTools,
		Caption_PageHome,
		Caption_PageView,
		Caption_PageViewNavigator, 
		Caption_PageViewSelector, 
		Caption_PageGroupBy, 
		Caption_PageFile,
		Caption_PageAppointment,
		Caption_GroupViewNavigator,
		Caption_GroupViewSelector,
		Caption_GroupArrangeView,
		Caption_GroupGroupBy,
		Caption_GroupAppointment,
		Caption_GroupTimeScale,
		Caption_GroupLayout,		
		Caption_GroupCommon,
		Caption_GroupAppointmentActions,
		Caption_GroupAppointmentOptions,
		Form_AccessibleText_EveryNDayText,
		Form_AccessibleText_EveryWeekOfMonthText,
		Form_AccessibleText_EveryNMonthText,
		Form_AccessibleText_EveryDayOfWeekText,
		Form_AccessibleText_EndAfterOccurencesText,
		Caption_PrintingSettings,
		Form_OpenReportTemplate,
		Form_PrintingSettings_Format,
		Form_PrintingSettings_Resources,
		Form_ResourcesKind,
		Form_AvailableResources,
		Form_ResourcesToPrint,
		Form_PrintUsingCustomCollection,
		Form_Preview,
		Caption_IntervalCount,
		Description_IntervalCount,
		PageCaption_Actions,
		PageCaption_Options
	}
	#endregion
	#region SchedulerControlLocalizer
	public class SchedulerControlLocalizer : DXLocalizer<SchedulerControlStringId> {
		static SchedulerControlLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SchedulerControlStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(SchedulerControlStringId.Caption_GotoDate, "Go To Date");
			AddString(SchedulerControlStringId.Caption_DeleteRecurrentApt, "Confirm Delete");
			AddString(SchedulerControlStringId.Caption_OpenRecurrentApt, "Open Recurring Item");
			AddString(SchedulerControlStringId.Caption_TimeRuler, "Time Ruler");
			AddString(SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteSeries, "Delete the series");
			AddString(SchedulerControlStringId.Form_RecurrentAppointmentAction_DeleteOccurrence, "Delete this occurrence");
			AddString(SchedulerControlStringId.Form_RecurrentAppointmentAction_EditSeries, "Edit the series");
			AddString(SchedulerControlStringId.Form_RecurrentAppointmentAction_EditOccurrence, "Edit this occurrence");
			AddString(SchedulerControlStringId.Form_DeleteRecurrentAppointmentFormMessage, "Do you want to delete all occurrences of the recurring appointment \"{0}\", or just this one?");
			AddString(SchedulerControlStringId.Form_EditRecurrentAppointmentFormMessage, "Do you want to edit all occurrences of the recurring appointment \"{0}\", or just this one?");
			AddString(SchedulerControlStringId.ButtonCaption_DismissAll, "Dismiss All");
			AddString(SchedulerControlStringId.ButtonCaption_Dismiss, "Dismiss");
			AddString(SchedulerControlStringId.ButtonCaption_Snooze, "Snooze");
			AddString(SchedulerControlStringId.ButtonCaption_OpenItem, "Open Item");
			AddString(SchedulerControlStringId.Form_ClickSnoozeToBeReminderAgainIn, "Click Snooze to be reminded again in:");
			AddString(SchedulerControlStringId.Form_Subject, "Subject:");
			AddString(SchedulerControlStringId.Form_Location, "Location:");
			AddString(SchedulerControlStringId.Form_StartTime, "Start time:");
			AddString(SchedulerControlStringId.Form_StartDate, "Start date:");
			AddString(SchedulerControlStringId.Form_EndDate, "End date:");
			AddString(SchedulerControlStringId.Form_EndTime, "End time:");
			AddString(SchedulerControlStringId.Form_Recurrence, "Appointment Recurrence");
			AddString(SchedulerControlStringId.Form_Resource, "Resource(s):\u200E");
			AddString(SchedulerControlStringId.Form_Label, "Label:");
			AddString(SchedulerControlStringId.Form_ShowTimeAs, "Show time as:");
			AddString(SchedulerControlStringId.Form_Reminder, "Reminder:");
			AddString(SchedulerControlStringId.Form_Description, "Description:");
			AddString(SchedulerControlStringId.Form_AllDayEvent, "All day event");
			AddString(SchedulerControlStringId.Form_Date, "Date:");
			AddString(SchedulerControlStringId.Form_ShowIn, "Show In:");
			AddString(SchedulerControlStringId.ButtonCaption_OK, "OK");
			AddString(SchedulerControlStringId.ButtonCaption_SaveAndClose, "Save And Close");
			AddString(SchedulerControlStringId.ButtonCaption_Cancel, "Cancel");
			AddString(SchedulerControlStringId.ButtonCaption_Delete, "Delete");
			AddString(SchedulerControlStringId.ButtonCaption_Recurrence, "Recurrence");
			AddString(SchedulerControlStringId.ButtonCaption_TimeZones, "TimeZones");
			AddString(SchedulerControlStringId.ButtonCaption_Save, "Save");
			AddString(SchedulerControlStringId.ButtonCaption_Undo, "Undo");
			AddString(SchedulerControlStringId.ButtonCaption_Redo, "Redo");
			AddString(SchedulerControlStringId.ButtonCaption_Next, "Next");
			AddString(SchedulerControlStringId.ButtonCaption_Previous, "Previous");
			AddString(SchedulerControlStringId.Form_DayOrDays, "day(s)\u200E");
			AddString(SchedulerControlStringId.Form_Every, "Every");
			AddString(SchedulerControlStringId.Form_EveryWeekday, "Every weekday");
			AddString(SchedulerControlStringId.Form_Day, "Day");
			AddString(SchedulerControlStringId.Form_OfEvery, "of every");
			AddString(SchedulerControlStringId.Form_MonthOrMonths, "month(s)\u200E");
			AddString(SchedulerControlStringId.Form_The, "The");
			AddString(SchedulerControlStringId.Form_RecurrencePattern, "Recurrence pattern");
			AddString(SchedulerControlStringId.Form_RangeOfRecurrence, "Recurrence end");
			AddString(SchedulerControlStringId.Form_Occurrences, "occurrences");
			AddString(SchedulerControlStringId.Form_Start, "Start:");
			AddString(SchedulerControlStringId.Form_NoEndDate, "No end date");
			AddString(SchedulerControlStringId.Form_EndAfter, "End after:");
			AddString(SchedulerControlStringId.Form_EndBy, "End by:");
			AddString(SchedulerControlStringId.Form_RecurEvery, "Recur every");
			AddString(SchedulerControlStringId.Form_WeekOrWeeksOn, "week(s) on:");
			AddString(SchedulerControlStringId.Form_On, "On");
			AddString(SchedulerControlStringId.Form_OnThe, "On the");
			AddString(SchedulerControlStringId.Form_of, "of");
			AddString(SchedulerControlStringId.Form_DayNumber, "Day Number");
			AddString(SchedulerControlStringId.Form_DayOfWeek, "Day of Week");
			AddString(SchedulerControlStringId.Form_TimeZone, "Time zone:");
			AddString(SchedulerControlStringId.Form_CurrentTime, "Current Time:");
			AddString(SchedulerControlStringId.Form_AdjustForDaylightSavingTime, "Adjust for daylight saving time");
			AddString(SchedulerControlStringId.Form_EveryEditableTextDay, "Every Editable Text day(s):\u200E");
			AddString(SchedulerControlStringId.Form_AccessibleText_RecurEveryEditableTextWeek, "Recur every Editable Text week(s)\u200E");
			AddString(SchedulerControlStringId.Form_AccessibleText_EveryNDayText, "Day Editable Text of every Editable Text month(s)\u200E");
			AddString(SchedulerControlStringId.Form_AccessibleText_EveryWeekOfMonthText, "The Editable Text Editable Text of every Editable Text month(s)\u200E");
			AddString(SchedulerControlStringId.Form_AccessibleText_EveryNMonthText, "On Editable Text Editable Text");
			AddString(SchedulerControlStringId.Form_AccessibleText_EveryDayOfWeekText, "On the Editable Text Editable Text of Editable Text");
			AddString(SchedulerControlStringId.Form_AccessibleText_EndAfterOccurencesText, "End after Editable Text occurences:");
			AddString(SchedulerControlStringId.Caption_PageCategoryAppointmentTools, "Calendar Tools");
			AddString(SchedulerControlStringId.Caption_PageHome, "Home");
			AddString(SchedulerControlStringId.Caption_PageView, "View");
			AddString(SchedulerControlStringId.Caption_PageViewNavigator, "View Navigator"); 
			AddString(SchedulerControlStringId.Caption_PageViewSelector, "View Selector"); 
			AddString(SchedulerControlStringId.Caption_PageGroupBy, "Group By"); 
			AddString(SchedulerControlStringId.Caption_PageFile, "File");
			AddString(SchedulerControlStringId.Caption_PageAppointment, "Appointment");
			AddString(SchedulerControlStringId.Caption_GroupViewNavigator, "Navigate");
			AddString(SchedulerControlStringId.Caption_GroupViewSelector, "Active View");
			AddString(SchedulerControlStringId.Caption_GroupGroupBy, "Group By");
			AddString(SchedulerControlStringId.Caption_GroupAppointment, "Appointment");
			AddString(SchedulerControlStringId.Caption_GroupArrangeView, "Arrange");
			AddString(SchedulerControlStringId.Caption_GroupTimeScale, "Time Scale");
			AddString(SchedulerControlStringId.Caption_GroupLayout, "Layout");
			AddString(SchedulerControlStringId.Caption_GroupCommon, "Common");
			AddString(SchedulerControlStringId.Caption_GroupAppointmentActions, "Actions");
			AddString(SchedulerControlStringId.Caption_GroupAppointmentOptions, "Options");
			AddString(SchedulerControlStringId.Caption_PrintingSettings, "Print Options");
			AddString(SchedulerControlStringId.Form_OpenReportTemplate, "Open report template (*.schrepx)");
			AddString(SchedulerControlStringId.Form_PrintingSettings_Format, "Format");
			AddString(SchedulerControlStringId.Form_PrintingSettings_Resources, "Resources");
			AddString(SchedulerControlStringId.Form_ResourcesKind, "Resources kind:");
			AddString(SchedulerControlStringId.Form_AvailableResources, "Available resources:");
			AddString(SchedulerControlStringId.Form_ResourcesToPrint, "Resources to print");
			AddString(SchedulerControlStringId.Form_PrintUsingCustomCollection, "Print using the custom collection");
			AddString(SchedulerControlStringId.Form_Preview, "Preview");
			AddString(SchedulerControlStringId.Caption_IntervalCount, "Interval Count");
			AddString(SchedulerControlStringId.Description_IntervalCount, "Change interval count");
			AddString(SchedulerControlStringId.PageCaption_Actions, "Actions");
			AddString(SchedulerControlStringId.PageCaption_Options, "Options");
		}
		#endregion
		public static XtraLocalizer<SchedulerControlStringId> CreateDefaultLocalizer() {
			return new SchedulerControlResXLocalizer();
		}
		public static string GetString(SchedulerControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SchedulerControlStringId> CreateResXLocalizer() {
			return new SchedulerControlResXLocalizer();
		}
	}
	#endregion
	#region SchedulerControlResXLocalizer
	public class SchedulerControlResXLocalizer : DXResXLocalizer<SchedulerControlStringId> {
		public SchedulerControlResXLocalizer()
			: base(new SchedulerControlLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Scheduler.LocalizationRes", typeof(SchedulerControlResXLocalizer).Assembly);
		}
	}
	#endregion
	#region SchedulerControlStringIdConverter
	public class SchedulerControlStringIdConverter : StringIdConverter<SchedulerControlStringId> {
		static SchedulerControlStringIdConverter() {
			SchedulerControlLocalizer.GetString(SchedulerControlStringId.ButtonCaption_Cancel);
		}
		protected override XtraLocalizer<SchedulerControlStringId> Localizer { get { return SchedulerControlLocalizer.Active; } }
	}
	#endregion
	#region SchedulerStringIdConverter
	public class SchedulerStringIdConverter : StringIdConverter<SchedulerStringId> {
		static SchedulerStringIdConverter() {
			SchedulerLocalizer.GetString(SchedulerStringId.Abbr_Day);
		}
		protected override XtraLocalizer<SchedulerStringId> Localizer { get { return SchedulerLocalizer.Active; } }
	}
	#endregion
	#region SchedulerStringIdExtension
	public class SchedulerStringIdExtension : MarkupExtension {
		public SchedulerStringIdExtension(SchedulerStringId stringId) {
			StringId = stringId;
		}
		public SchedulerStringId StringId { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return SchedulerLocalizer.GetString(StringId);
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Native {
	#region ObsoleteText
	internal static class ObsoleteText {
		internal const string SRFormShowingEventArgsSizeToContent = "This property is obsolete. Do not use it.";
		internal const string SRFormOperationHelper = "Use 'DevExpress.Xpf.Scheduler.SchedulerFormBehavior' instead.";
		internal const string SRSchedulerFormBehaviorSetFormTitle = "Use the 'SchedulerFormBehavior.SetTitle' method instead";
	}
	#endregion
}
