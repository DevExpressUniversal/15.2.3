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
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Localization {
	#region SchedulerStringId
	public enum SchedulerStringId {
		DefaultToolTipStringFormat_SplitAppointment,
		Msg_IsNotValid,
		Msg_InvalidDayOfWeekForDailyRecurrence,
		Msg_InternalError,
		Msg_NoMappingForObject,
		Msg_XtraSchedulerNotAssigned,
		Msg_InvalidTimeOfDayInterval,
		Msg_OverflowTimeOfDayInterval,
		Msg_LoadCollectionFromXml,
		AppointmentLabel_None,
		AppointmentLabel_Important,
		AppointmentLabel_Business,
		AppointmentLabel_Personal,
		AppointmentLabel_Vacation,
		AppointmentLabel_MustAttend,
		AppointmentLabel_TravelRequired,
		AppointmentLabel_NeedsPreparation,
		AppointmentLabel_Birthday,
		AppointmentLabel_Anniversary,
		AppointmentLabel_PhoneCall,
		Appointment_StartContinueText,
		Appointment_EndContinueText,
		Msg_InvalidEndDate,
		Caption_Appointment,
		Caption_Event,
		Caption_UntitledAppointment,
		Caption_ReadOnly,
		Caption_WeekDaysEveryDay,
		Caption_WeekDaysWeekendDays,
		Caption_WeekDaysWorkDays,
		Caption_WeekOfMonthFirst,
		Caption_WeekOfMonthSecond,
		Caption_WeekOfMonthThird,
		Caption_WeekOfMonthFourth,
		Caption_WeekOfMonthLast,
		Msg_InvalidDayCount,
		Msg_InvalidDayCountValue,
		Msg_InvalidWeekCount,
		Msg_InvalidWeekCountValue,
		Msg_InvalidMonthCount,
		Msg_InvalidMonthCountValue,
		Msg_InvalidYearCount,
		Msg_InvalidYearCountValue,
		Msg_InvalidOccurrencesCount,
		Msg_InvalidOccurrencesCountValue,
		Msg_InvalidDayNumber,
		Msg_InvalidDayNumberValue,
		Msg_WarningDayNumber,
		Msg_InvalidDayOfWeek,
		Msg_WarningAppointmentDeleted,
		MenuCmd_None,
		MenuCmd_OpenAppointment,
		DescCmd_OpenAppointment,
		MenuCmd_PrintAppointment,
		MenuCmd_DeleteAppointment,
		DescCmd_DeleteAppointment,
		MenuCmd_EditSeries,
		MenuCmd_RestoreOccurrence,
		MenuCmd_NewAppointment,
		DescCmd_NewAppointment,
		MenuCmd_NewAllDayEvent,
		MenuCmd_NewRecurringAppointment,
		DescCmd_NewRecurringAppointment,
		MenuCmd_NewRecurringEvent,
		MenuCmd_EditAppointmentDependency,
		DescCmd_EditAppointmentDependency,
		MenuCmd_DeleteAppointmentDependency,
		DescCmd_DeleteAppointmentDependency,
		MenuCmd_GotoThisDay,
		MenuCmd_GotoToday,
		DescCmd_GotoToday,
		MenuCmd_GotoDate,
		MenuCmd_OtherSettings,
		MenuCmd_CustomizeCurrentView,
		MenuCmd_CustomizeTimeRuler,
		MenuCmd_5Minutes,
		MenuCmd_6Minutes,
		MenuCmd_10Minutes,
		MenuCmd_15Minutes,
		MenuCmd_20Minutes,
		MenuCmd_30Minutes,
		MenuCmd_60Minutes,
		MenuCmd_SwitchViewMenu,
		MenuCmd_SwitchToDayView,
		MenuCmd_SwitchToWorkWeekView,
		MenuCmd_SwitchToWeekView,
		MenuCmd_SwitchToMonthView,
		MenuCmd_SwitchToTimelineView,
		MenuCmd_SwitchToFullWeekView,
		MenuCmd_SwitchToGroupByNone,
		MenuCmd_SwitchToGroupByResource,
		MenuCmd_SwitchToGroupByDate,
		MenuCmd_SwitchToGanttView,
		MenuCmd_TimeScalesMenu,
		MenuCmd_TimeScaleCaptionsMenu,
		MenuCmd_TimeScaleHour,
		MenuCmd_TimeScaleDay,
		MenuCmd_TimeScaleWeek,
		MenuCmd_TimeScaleMonth,
		MenuCmd_TimeScaleQuarter,
		MenuCmd_TimeScaleYear,
		MenuCmd_ShowTimeAs,
		DescCmd_ShowTimeAs,
		MenuCmd_Free,
		MenuCmd_Busy,
		MenuCmd_Tentative,
		MenuCmd_OutOfOffice,
		MenuCmd_WorkingElsewhere,
		MenuCmd_LabelAs,
		DescCmd_LabelAs,
		MenuCmd_AppointmentLabelNone,
		MenuCmd_AppointmentLabelImportant,
		MenuCmd_AppointmentLabelBusiness,
		MenuCmd_AppointmentLabelPersonal,
		MenuCmd_AppointmentLabelVacation,
		MenuCmd_AppointmentLabelMustAttend,
		MenuCmd_AppointmentLabelTravelRequired,
		MenuCmd_AppointmentLabelNeedsPreparation,
		MenuCmd_AppointmentLabelBirthday,
		MenuCmd_AppointmentLabelAnniversary,
		MenuCmd_AppointmentLabelPhoneCall,
		MenuCmd_AppointmentMove,
		MenuCmd_AppointmentCopy,
		MenuCmd_AppointmentCancel,
		Caption_5Minutes,
		Caption_6Minutes,
		Caption_10Minutes,
		Caption_15Minutes,
		Caption_20Minutes,
		Caption_30Minutes,
		Caption_60Minutes,
		Caption_Free,
		Caption_Busy,
		Caption_Tentative,
		Caption_WorkingElsewhere,
		Caption_OutOfOffice,
		ViewDisplayName_Day,
		ViewDisplayName_WorkDays,
		ViewDisplayName_Week,
		ViewDisplayName_Month,
		ViewDisplayName_Timeline,
		ViewDisplayName_FullWeek,
		ViewDisplayName_Gantt,
		ViewShortDisplayName_Day,
		ViewShortDisplayName_WorkDays,
		ViewShortDisplayName_Week,
		ViewShortDisplayName_Month,
		ViewShortDisplayName_Timeline,
		ViewShortDisplayName_FullWeek,
		ViewShortDisplayName_Gantt,
		TimeScaleDisplayName_Hour,
		TimeScaleDisplayName_Day,
		TimeScaleDisplayName_Week,
		TimeScaleDisplayName_Month,
		TimeScaleDisplayName_Quarter,
		TimeScaleDisplayName_Year,
		Abbr_MinutesShort1,
		Abbr_MinutesShort2,
		Abbr_Minute,
		Abbr_Minutes,
		Abbr_HoursShort,
		Abbr_Hour,
		Abbr_Hours,
		Abbr_DaysShort,
		Abbr_Day,
		Abbr_Days,
		Abbr_WeeksShort,
		Abbr_Week,
		Abbr_Weeks,
		Abbr_Month,
		Abbr_Months,
		Abbr_Year,
		Abbr_Years,
		Caption_Reminder,
		Caption_Reminders,
		Caption_StartTime,
		Caption_NAppointmentsAreSelected,
		Format_TimeBeforeStart,
		Msg_Conflict,
		Msg_InvalidAppointmentDuration,
		Msg_InvalidReminderTimeBeforeStart,
		TextDuration_FromTo,
		TextDuration_FromForDays,
		TextDuration_FromForDaysHours,
		TextDuration_FromForDaysMinutes,
		TextDuration_FromForDaysHoursMinutes,
		TextDuration_ForPattern,
		TextDailyPatternString_EveryDay,
		TextDailyPatternString_EveryDays,
		TextDailyPatternString_EveryWeekDay,
		TextDailyPatternString_EveryWeekend,
		TextWeekly_0Day,
		TextWeekly_1Day,
		TextWeekly_2Day,
		TextWeekly_3Day,
		TextWeekly_4Day,
		TextWeekly_5Day,
		TextWeekly_6Day,
		TextWeekly_7Day,
		TextWeeklyPatternString_EveryWeek,
		TextWeeklyPatternString_EveryWeeks,
		TextMonthlyPatternString_SubPattern,
		TextMonthlyPatternString1,
		TextMonthlyPatternString2,
		TextYearlyPattern_YearString1,
		TextYearlyPattern_YearString2,
		TextYearlyPattern_YearsString1,
		TextYearlyPattern_YearsString2,
		Caption_AllDay,
		Caption_PleaseSeeAbove,
		Caption_RecurrenceSubject,
		Caption_RecurrenceLocation,
		Caption_RecurrenceStartTime,
		Caption_RecurrenceEndTime,
		Caption_RecurrenceShowTimeAs,
		Caption_Recurrence,
		Caption_RecurrencePattern,
		Caption_NoneRecurrence,
		MemoPrintDateFormat,
		Caption_EmptyResource,
		Caption_DailyPrintStyle,
		Caption_WeeklyPrintStyle,
		Caption_MonthlyPrintStyle,
		Caption_TrifoldPrintStyle,
		Caption_CalendarDetailsPrintStyle,
		Caption_MemoPrintStyle,
		Caption_ColorConverterFullColor,
		Caption_ColorConverterGrayScale,
		Caption_ColorConverterBlackAndWhite,
		Caption_ResourceNone,
		Caption_ResourceAll,
		PrintPageSetupFormatTabControlCustomizeShading,
		PrintPageSetupFormatTabControlSizeAndFontName,
		PrintRangeControlInvalidDate,
		PrintCalendarDetailsControlDayPeriod,
		PrintCalendarDetailsControlWeekPeriod,
		PrintCalendarDetailsControlMonthPeriod,
		PrintMonthlyOptControlOnePagePerMonth,
		PrintMonthlyOptControlTwoPagesPerMonth,
		PrintTimeIntervalControlInvalidDuration,
		PrintTimeIntervalControlInvalidStartEndTime,
		PrintTriFoldOptControlDailyCalendar,
		PrintTriFoldOptControlWeeklyCalendar,
		PrintTriFoldOptControlMonthlyCalendar,
		PrintWeeklyOptControlOneWeekPerPage,
		PrintWeeklyOptControlTwoWeekPerPage,
		PrintPageSetupFormCaption,
		PrintMoreItemsMsg,
		PrintNoPrintersInstalled,
		Caption_FirstVisibleResources,
		Caption_PrevVisibleResourcesPage,
		Caption_PrevVisibleResources,
		Caption_NextVisibleResources,
		Caption_NextVisibleResourcesPage,
		Caption_LastVisibleResources,
		Caption_IncreaseVisibleResourcesCount,
		Caption_DecreaseVisibleResourcesCount,
		Caption_ShadingApplyToAllDayArea,
		Caption_ShadingApplyToAppointments,
		Caption_ShadingApplyToAppointmentStatuses,
		Caption_ShadingApplyToHeaders,
		Caption_ShadingApplyToTimeRulers,
		Caption_ShadingApplyToCells,
		Msg_InvalidSize,
		Msg_ApplyToAllStyles,
		Msg_MemoPrintNoSelectedItems,
		Caption_AllResources,
		Caption_VisibleResources,
		Caption_OnScreenResources,
		Caption_GroupByNone,
		Caption_GroupByDate,
		Caption_GroupByResources,
		Msg_InvalidInputFile,
		TextRecurrenceTypeDaily,
		TextRecurrenceTypeWeekly,
		TextRecurrenceTypeMonthly,
		TextRecurrenceTypeYearly,
		TextRecurrenceTypeMinutely,
		TextRecurrenceTypeHourly,
		Msg_Warning,
		Msg_CantFitIntoPage,
		Msg_PrintStyleNameExists,
		Msg_OutlookCalendarNotFound,
		Caption_PrevAppointment,
		Caption_NextAppointment,
		DisplayName_Appointment,
		Format_CopyOf,
		Format_CopyNOf,
		Msg_MissingRequiredMapping,
		Msg_MissingMappingMember,
		Msg_DuplicateMappingMember,
		Msg_InconsistentRecurrenceInfoMapping,
		Msg_IncorrectMappingsQuestion,
		Msg_DuplicateCustomFieldMappings,
		Msg_MappingsCheckPassedOk,
		Caption_SetupAppointmentMappings,
		Caption_SetupResourceMappings,
		Caption_SetupDependencyMappings,
		Caption_ModifyAppointmentMappingsTransactionDescription,
		Caption_ModifyResourceMappingsTransactionDescription,
		Caption_ModifyAppointmentDependencyMappingsTransactionDescription,
		Caption_MappingsValidation,
		Caption_MappingsWizard,
		Caption_CheckMappings,
		Caption_SetupAppointmentStorage,
		Caption_SetupResourceStorage,
		Caption_SetupAppointmentDependencyStorage,
		Caption_ModifyAppointmentStorageTransactionDescription,
		Caption_ModifyResourceStorageTransactionDescription,
		Caption_ModifyAppointmentDependencyStorageTransactionDescription,
		Caption_DayViewDescription,
		Caption_WorkWeekViewDescription,
		Caption_WeekViewDescription,
		Caption_MonthViewDescription,
		Caption_TimelineViewDescription,
		Caption_FullWeekViewDescription,
		Caption_GanttViewDescription,
		Caption_GroupByNoneDescription,
		Caption_GroupByDateDescription,
		Caption_GroupByResourceDescription,
		Msg_iCalendar_NotValidFile,
		Msg_iCalendar_AppointmentsImportWarning,
		MenuCmd_NavigateBackward,
		MenuCmd_NavigateForward,
		DescCmd_NavigateBackward,
		DescCmd_NavigateForward,
		MenuCmd_ViewZoomIn,
		MenuCmd_ViewZoomOut,
		DescCmd_ViewZoomIn,
		DescCmd_ViewZoomOut,
		DescCmd_SplitAppointment,
		Caption_SplitAppointment,
		VS_SchedulerReportsToolboxCategoryName,
		UD_SchedulerReportsToolboxCategoryName,
		Reporting_NotAssigned_TimeCells,
		Reporting_NotAssigned_View,
		Msg_RecurrenceExceptionsWillBeLost,
		DescCmd_CreateAppointmentDependency,
		MenuCmd_CreateAppointmentDependency,
		Caption_AppointmentDependencyTypeFinishToStart,
		Caption_AppointmentDependencyTypeStartToStart,
		Caption_AppointmentDependencyTypeFinishToFinish,
		Caption_AppointmentDependencyTypeStartToFinish,
		TextAppointmentSnapToCells_Always,
		TextAppointmentSnapToCells_Auto,
		TextAppointmentSnapToCells_Disabled,
		TextAppointmentSnapToCells_Never,
		MenuCmd_PrintPreview,
		DescCmd_PrintPreview,
		MenuCmd_Print,
		DescCmd_Print,
		MenuCmd_PrintPageSetup,
		DescCmd_PrintPageSetup,
		DescCmd_TimeScalesMenu,
		MenuCmd_ShowWorkTimeOnly,
		DescCmd_ShowWorkTimeOnly,
		MenuCmd_CompressWeekend,
		DescCmd_CompressWeekend,
		MenuCmd_CellsAutoHeight,
		DescCmd_CellsAutoHeight,
		MenuCmd_ToggleRecurrence,
		DescCmd_ToggleRecurrence,
		MenuCmd_ChangeAppointmentReminderUI,
		DescCmd_ChangeAppointmentReminderUI,
		Caption_NoneReminder,
		DescCmd_ChangeTimelineScaleWidth,
		MenuCmd_ChangeTimelineScaleWidth,
		MenuCmd_OpenSchedule,
		DescCmd_OpenSchedule,
		MenuCmd_SaveSchedule,
		DescCmd_SaveSchedule,
		MenuCmd_ChangeSnapToCellsUI,
		DescCmd_ChangeSnapToCellsUI,
		MenuCmd_OpenOccurrence,
		DescCmd_OpenOccurrence,
		MenuCmd_OpenSeries,
		DescCmd_OpenSeries,
		MenuCmd_DeleteOccurrence,
		DescCmd_DeleteOccurrence,
		MenuCmd_DeleteSeries,
		DescCmd_DeleteSeries,
		DateTimeAutoFormat_WithoutYear,
		DateTimeAutoFormat_Week,
		Msg_SaveBeforeClose
	}
	#endregion
	public class SchedulerResLocalizer : XtraResXLocalizer<SchedulerStringId> {
		public SchedulerResLocalizer()
			: base(new SchedulerLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraScheduler.LocalizationRes", typeof(SchedulerResLocalizer).Assembly);
		}
	}
	public class SchedulerLocalizer : XtraLocalizer<SchedulerStringId> {
		static SchedulerLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SchedulerStringId>(CreateDefaultLocalizer()));
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerLocalizerActive")]
#endif
		public static new XtraLocalizer<SchedulerStringId> Active {
			get { return XtraLocalizer<SchedulerStringId>.Active; }
			set { XtraLocalizer<SchedulerStringId>.Active = value; }
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(SchedulerStringId.Msg_IsNotValid, "'{0}' is not a valid value for '{1}'");
			AddString(SchedulerStringId.Msg_InvalidDayOfWeekForDailyRecurrence, "Invalid day of week for a daily recurrence. Only WeekDays.EveryDay, WeekDays.WeekendDays and WeekDays.WorkDays are valid in this context.");
			AddString(SchedulerStringId.Msg_InternalError, "Internal error!");
			AddString(SchedulerStringId.Msg_NoMappingForObject, "The following required mappings for the object \r\n {0} are not assigned");
			AddString(SchedulerStringId.Msg_XtraSchedulerNotAssigned, "The SchedulerStorage component is not assigned to the SchedulerControl");
			AddString(SchedulerStringId.Msg_InvalidTimeOfDayInterval, "Invalid duration for the TimeOfDayInterval");
			AddString(SchedulerStringId.Msg_OverflowTimeOfDayInterval, "Invalid value for the TimeOfDayInterval. Should be less than or equal to a day");
			AddString(SchedulerStringId.Msg_LoadCollectionFromXml, "The scheduler needs to be in unbound mode to load collection items from xml");
			AddString(SchedulerStringId.AppointmentLabel_None, "None");
			AddString(SchedulerStringId.AppointmentLabel_Important, "Important");
			AddString(SchedulerStringId.AppointmentLabel_Business, "Business");
			AddString(SchedulerStringId.AppointmentLabel_Personal, "Personal");
			AddString(SchedulerStringId.AppointmentLabel_Vacation, "Vacation");
			AddString(SchedulerStringId.AppointmentLabel_MustAttend, "Must Attend");
			AddString(SchedulerStringId.AppointmentLabel_TravelRequired, "Travel Required");
			AddString(SchedulerStringId.AppointmentLabel_NeedsPreparation, "Needs Preparation");
			AddString(SchedulerStringId.AppointmentLabel_Birthday, "Birthday");
			AddString(SchedulerStringId.AppointmentLabel_Anniversary, "Anniversary");
			AddString(SchedulerStringId.AppointmentLabel_PhoneCall, "Phone Call");
			AddString(SchedulerStringId.Appointment_StartContinueText, "From {0}");
			AddString(SchedulerStringId.Appointment_EndContinueText, "To {0}");
			AddString(SchedulerStringId.Msg_InvalidEndDate, "The date you entered occurs before the start date.");
			AddString(SchedulerStringId.Caption_Appointment, "{0} - Appointment");
			AddString(SchedulerStringId.Caption_Event, "{0} - Event");
			AddString(SchedulerStringId.Caption_UntitledAppointment, "Untitled");
			AddString(SchedulerStringId.Caption_ReadOnly, " [Read only]");
			AddString(SchedulerStringId.Caption_WeekDaysEveryDay, "Day");
			AddString(SchedulerStringId.Caption_WeekDaysWeekendDays, "Weekend day");
			AddString(SchedulerStringId.Caption_WeekDaysWorkDays, "Weekday");
			AddString(SchedulerStringId.Caption_WeekOfMonthFirst, "First");
			AddString(SchedulerStringId.Caption_WeekOfMonthSecond, "Second");
			AddString(SchedulerStringId.Caption_WeekOfMonthThird, "Third");
			AddString(SchedulerStringId.Caption_WeekOfMonthFourth, "Fourth");
			AddString(SchedulerStringId.Caption_WeekOfMonthLast, "Last");
			AddString(SchedulerStringId.Msg_InvalidDayCount, "Invalid day count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidDayCountValue, "Invalid day count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidWeekCount, "Invalid week count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidWeekCountValue, "Invalid week count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidMonthCount, "Invalid month count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidMonthCountValue, "Invalid month count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidYearCount, "Invalid year count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidYearCountValue, "Invalid year count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidOccurrencesCount, "Invalid occurrences count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidOccurrencesCountValue, "Invalid occurrences count. Please enter a positive integer value.");
			AddString(SchedulerStringId.Msg_InvalidDayNumber, "Invalid day number. Please enter an integer value from 1 to {0}.");
			AddString(SchedulerStringId.Msg_InvalidDayNumberValue, "Invalid day number. Please enter an integer value from 1 to {0}.");
			AddString(SchedulerStringId.Msg_WarningDayNumber, "Some months have fewer than {0} days. For these months, the occurrences will fall on the last day of the month.");
			AddString(SchedulerStringId.Msg_InvalidDayOfWeek, "No day selected. Please select at least one day in the week.");
			AddString(SchedulerStringId.Msg_WarningAppointmentDeleted, "The appointment has been deleted by another user.");
			AddString(SchedulerStringId.MenuCmd_None, "");
			AddString(SchedulerStringId.MenuCmd_OpenAppointment, "&Open");
			AddString(SchedulerStringId.DescCmd_OpenAppointment, "Open the selected appointment.");
			AddString(SchedulerStringId.MenuCmd_PrintAppointment, "&Print");
			AddString(SchedulerStringId.MenuCmd_DeleteAppointment, "&Delete");
			AddString(SchedulerStringId.DescCmd_DeleteAppointment, "Delete the selected appointment(s).");
			AddString(SchedulerStringId.MenuCmd_DeleteOccurrence, "Delete Occurrence");
			AddString(SchedulerStringId.DescCmd_DeleteOccurrence, "Delete Occurrence.");
			AddString(SchedulerStringId.MenuCmd_DeleteSeries, "Delete Series");
			AddString(SchedulerStringId.DescCmd_DeleteSeries, "Delete Series.");
			AddString(SchedulerStringId.MenuCmd_EditSeries, "&Edit Series");
			AddString(SchedulerStringId.MenuCmd_RestoreOccurrence, "&Restore Default State");
			AddString(SchedulerStringId.MenuCmd_NewAppointment, "New App&ointment");
			AddString(SchedulerStringId.DescCmd_NewAppointment, "Create a new appointment.");
			AddString(SchedulerStringId.MenuCmd_NewAllDayEvent, "New All Day &Event");
			AddString(SchedulerStringId.MenuCmd_NewRecurringAppointment, "New Recurring &Appointment");
			AddString(SchedulerStringId.DescCmd_NewRecurringAppointment, "Create a new recurring appointment.");
			AddString(SchedulerStringId.MenuCmd_NewRecurringEvent, "New Recurring E&vent");
			AddString(SchedulerStringId.MenuCmd_EditAppointmentDependency, "&Edit");
			AddString(SchedulerStringId.DescCmd_EditAppointmentDependency, "Edit appointment dependency.");
			AddString(SchedulerStringId.MenuCmd_DeleteAppointmentDependency, "&Delete");
			AddString(SchedulerStringId.DescCmd_DeleteAppointmentDependency, "Delete appointment dependency.");
			AddString(SchedulerStringId.MenuCmd_NavigateBackward, "Backward");
			AddString(SchedulerStringId.MenuCmd_NavigateForward, "Forward");
			AddString(SchedulerStringId.DescCmd_NavigateBackward, "Step back in time as suggested by the current view.");
			AddString(SchedulerStringId.DescCmd_NavigateForward, "Advance forward in time as suggested by the current view.");
			AddString(SchedulerStringId.MenuCmd_ViewZoomIn, "Zoom In");
			AddString(SchedulerStringId.MenuCmd_ViewZoomOut, "Zoom Out");
			AddString(SchedulerStringId.DescCmd_ViewZoomIn, "Perform scaling up to display content in more detail.");
			AddString(SchedulerStringId.DescCmd_ViewZoomOut, "Perform scaling down to display a broader look of the View.");
			AddString(SchedulerStringId.MenuCmd_GotoThisDay, "Go to This &Day");
			AddString(SchedulerStringId.MenuCmd_GotoToday, "Go to &Today");
			AddString(SchedulerStringId.DescCmd_GotoToday, "Change the date displayed in the current view to the current date.");
			AddString(SchedulerStringId.MenuCmd_GotoDate, "&Go to Date...");
			AddString(SchedulerStringId.MenuCmd_OtherSettings, "Other Sett&ings...");
			AddString(SchedulerStringId.MenuCmd_CustomizeCurrentView, "&Customize Current View...");
			AddString(SchedulerStringId.MenuCmd_CustomizeTimeRuler, "Customize Time Ruler...");
			AddString(SchedulerStringId.MenuCmd_5Minutes, "&5 Minutes");
			AddString(SchedulerStringId.MenuCmd_6Minutes, "&6 Minutes");
			AddString(SchedulerStringId.MenuCmd_10Minutes, "10 &Minutes");
			AddString(SchedulerStringId.MenuCmd_15Minutes, "&15 Minutes");
			AddString(SchedulerStringId.MenuCmd_20Minutes, "&20 Minutes");
			AddString(SchedulerStringId.MenuCmd_30Minutes, "&30 Minutes");
			AddString(SchedulerStringId.MenuCmd_60Minutes, "6&0 Minutes");
			AddString(SchedulerStringId.MenuCmd_SwitchViewMenu, "Change View To");
			AddString(SchedulerStringId.MenuCmd_SwitchToDayView, "&Day View");
			AddString(SchedulerStringId.MenuCmd_SwitchToWorkWeekView, "Wo&rk Week View");
			AddString(SchedulerStringId.MenuCmd_SwitchToWeekView, "&Week View");
			AddString(SchedulerStringId.MenuCmd_SwitchToMonthView, "&Month View");
			AddString(SchedulerStringId.MenuCmd_SwitchToTimelineView, "&Timeline View");
			AddString(SchedulerStringId.MenuCmd_SwitchToFullWeekView, "&Full Week View");
			AddString(SchedulerStringId.MenuCmd_SwitchToGroupByNone, "&Group by None");
			AddString(SchedulerStringId.MenuCmd_SwitchToGroupByResource, "&Group by Resource");
			AddString(SchedulerStringId.MenuCmd_SwitchToGroupByDate, "&Group by Date");
			AddString(SchedulerStringId.MenuCmd_SwitchToGanttView, "&Gantt View");
			AddString(SchedulerStringId.MenuCmd_TimeScalesMenu, "&Time Scales");
			AddString(SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu, "Time Scale &Captions");
			AddString(SchedulerStringId.MenuCmd_TimeScaleHour, "&Hour");
			AddString(SchedulerStringId.MenuCmd_TimeScaleDay, "&Day");
			AddString(SchedulerStringId.MenuCmd_TimeScaleWeek, "&Week");
			AddString(SchedulerStringId.MenuCmd_TimeScaleMonth, "&Month");
			AddString(SchedulerStringId.MenuCmd_TimeScaleQuarter, "&Quarter");
			AddString(SchedulerStringId.MenuCmd_TimeScaleYear, "&Year");
			AddString(SchedulerStringId.MenuCmd_ShowTimeAs, "&Show Time As");
			AddString(SchedulerStringId.DescCmd_ShowTimeAs, "Change the selected appointment status.");
			AddString(SchedulerStringId.MenuCmd_Free, "&Free");
			AddString(SchedulerStringId.MenuCmd_Busy, "&Busy");
			AddString(SchedulerStringId.MenuCmd_Tentative, "&Tentative");
			AddString(SchedulerStringId.MenuCmd_OutOfOffice, "&Out Of Office");
			AddString(SchedulerStringId.MenuCmd_WorkingElsewhere, "&Working Elsewhere");
			AddString(SchedulerStringId.MenuCmd_LabelAs, "&Label As");
			AddString(SchedulerStringId.DescCmd_LabelAs, "Change the selected appointment label.");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelNone, "&None");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelImportant, "&Important");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelBusiness, "&Business");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelPersonal, "&Personal");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelVacation, "&Vacation");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelMustAttend, "Must &Attend");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelTravelRequired, "&Travel Required");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelNeedsPreparation, "&Needs Preparation");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelBirthday, "&Birthday");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelAnniversary, "&Anniversary");
			AddString(SchedulerStringId.MenuCmd_AppointmentLabelPhoneCall, "Phone &Call");
			AddString(SchedulerStringId.MenuCmd_AppointmentMove, "Mo&ve");
			AddString(SchedulerStringId.MenuCmd_AppointmentCopy, "&Copy");
			AddString(SchedulerStringId.MenuCmd_AppointmentCancel, "C&ancel");
			AddString(SchedulerStringId.Caption_5Minutes, "5 Minutes");
			AddString(SchedulerStringId.Caption_6Minutes, "6 Minutes");
			AddString(SchedulerStringId.Caption_10Minutes, "10 Minutes");
			AddString(SchedulerStringId.Caption_15Minutes, "15 Minutes");
			AddString(SchedulerStringId.Caption_20Minutes, "20 Minutes");
			AddString(SchedulerStringId.Caption_30Minutes, "30 Minutes");
			AddString(SchedulerStringId.Caption_60Minutes, "60 Minutes");
			AddString(SchedulerStringId.Caption_Free, "Free");
			AddString(SchedulerStringId.Caption_Busy, "Busy");
			AddString(SchedulerStringId.Caption_Tentative, "Tentative");
			AddString(SchedulerStringId.Caption_WorkingElsewhere, "Working Elsewhere");
			AddString(SchedulerStringId.Caption_OutOfOffice, "Out Of Office");
			AddString(SchedulerStringId.ViewDisplayName_Day, "Day Calendar");
			AddString(SchedulerStringId.ViewDisplayName_WorkDays, "Work Week Calendar");
			AddString(SchedulerStringId.ViewDisplayName_Week, "Week Calendar");
			AddString(SchedulerStringId.ViewDisplayName_Month, "Month Calendar");
			AddString(SchedulerStringId.ViewDisplayName_Timeline, "Timeline Calendar");
			AddString(SchedulerStringId.ViewDisplayName_Gantt, "Gantt View");
			AddString(SchedulerStringId.ViewDisplayName_FullWeek, "Full Week Calendar");
			AddString(SchedulerStringId.ViewShortDisplayName_Day, "Day");
			AddString(SchedulerStringId.ViewShortDisplayName_WorkDays, "Work Week");
			AddString(SchedulerStringId.ViewShortDisplayName_Week, "Week");
			AddString(SchedulerStringId.ViewShortDisplayName_Month, "Month");
			AddString(SchedulerStringId.ViewShortDisplayName_Timeline, "Timeline");
			AddString(SchedulerStringId.ViewShortDisplayName_Gantt, "Gantt");
			AddString(SchedulerStringId.ViewShortDisplayName_FullWeek, "Full Week");
			AddString(SchedulerStringId.TimeScaleDisplayName_Hour, "Hour");
			AddString(SchedulerStringId.TimeScaleDisplayName_Day, "Day");
			AddString(SchedulerStringId.TimeScaleDisplayName_Week, "Week");
			AddString(SchedulerStringId.TimeScaleDisplayName_Month, "Month");
			AddString(SchedulerStringId.TimeScaleDisplayName_Quarter, "Quarter");
			AddString(SchedulerStringId.TimeScaleDisplayName_Year, "Year");
			AddString(SchedulerStringId.Abbr_MinutesShort1, "m");
			AddString(SchedulerStringId.Abbr_MinutesShort2, "min");
			AddString(SchedulerStringId.Abbr_Minute, "minute");
			AddString(SchedulerStringId.Abbr_Minutes, "minutes");
			AddString(SchedulerStringId.Abbr_HoursShort, "h");
			AddString(SchedulerStringId.Abbr_Hour, "hour");
			AddString(SchedulerStringId.Abbr_Hours, "hours");
			AddString(SchedulerStringId.Abbr_DaysShort, "d");
			AddString(SchedulerStringId.Abbr_Day, "day");
			AddString(SchedulerStringId.Abbr_Days, "days");
			AddString(SchedulerStringId.Abbr_WeeksShort, "w");
			AddString(SchedulerStringId.Abbr_Week, "week");
			AddString(SchedulerStringId.Abbr_Weeks, "weeks");
			AddString(SchedulerStringId.Abbr_Month, "month");
			AddString(SchedulerStringId.Abbr_Months, "months");
			AddString(SchedulerStringId.Abbr_Year, "year");
			AddString(SchedulerStringId.Abbr_Years, "years");
			AddString(SchedulerStringId.Caption_Reminder, "{0} Reminder");
			AddString(SchedulerStringId.Caption_Reminders, "{0} Reminders");
			AddString(SchedulerStringId.Caption_StartTime, "Start time: {0}");
			AddString(SchedulerStringId.Caption_NAppointmentsAreSelected, "{0} appointments are selected");
			AddString(SchedulerStringId.Format_TimeBeforeStart, "{0} before start");
			AddString(SchedulerStringId.Msg_Conflict, "An edited appointment conflicts with one or several other appointments.");
			AddString(SchedulerStringId.Msg_InvalidAppointmentDuration, "Invalid value specified for the interval duration. Please enter a positive value.");
			AddString(SchedulerStringId.Msg_InvalidReminderTimeBeforeStart, "Invalid value specified for the before event reminder's time. Please enter a positive value.");
			AddString(SchedulerStringId.TextDuration_FromTo, "from {0} to {1}");
			AddString(SchedulerStringId.TextDuration_FromForDays, "from {0} for {1} ");
			AddString(SchedulerStringId.TextDuration_FromForDaysHours, "from {0} for {1} {2}");
			AddString(SchedulerStringId.TextDuration_FromForDaysMinutes, "from {0} for {1} {3}");
			AddString(SchedulerStringId.TextDuration_FromForDaysHoursMinutes, "from {0} for {1} {2} {3}");
			AddString(SchedulerStringId.TextDuration_ForPattern, "{0} {1}");
			AddString(SchedulerStringId.TextDailyPatternString_EveryDay, "every {1} {0}");
			AddString(SchedulerStringId.TextDailyPatternString_EveryDays, "every {2} {1} {0}");
			AddString(SchedulerStringId.TextDailyPatternString_EveryWeekDay, "every weekday {0}");
			AddString(SchedulerStringId.TextDailyPatternString_EveryWeekend, "every weekend {0}");
			AddString(SchedulerStringId.TextWeekly_0Day, "unspecified day of week");
			AddString(SchedulerStringId.TextWeekly_1Day, "{0}");
			AddString(SchedulerStringId.TextWeekly_2Day, "{0} and {1}");
			AddString(SchedulerStringId.TextWeekly_3Day, "{0}, {1}, and {2}");
			AddString(SchedulerStringId.TextWeekly_4Day, "{0}, {1}, {2}, and {3}");
			AddString(SchedulerStringId.TextWeekly_5Day, "{0}, {1}, {2}, {3}, and {4}");
			AddString(SchedulerStringId.TextWeekly_6Day, "{0}, {1}, {2}, {3}, {4}, and {5}");
			AddString(SchedulerStringId.TextWeekly_7Day, "{0}, {1}, {2}, {3}, {4}, {5}, and {6}");
			AddString(SchedulerStringId.TextWeeklyPatternString_EveryWeek, "every {3} {0}");
			AddString(SchedulerStringId.TextWeeklyPatternString_EveryWeeks, "every {1} {2} on {3} {0}");
			AddString(SchedulerStringId.TextMonthlyPatternString_SubPattern, "of every {0} {1} {2}");
			AddString(SchedulerStringId.TextMonthlyPatternString1, "day {3} {0}");
			AddString(SchedulerStringId.TextMonthlyPatternString2, "the {1} {2} {0}");
			AddString(SchedulerStringId.TextYearlyPattern_YearString1, "every {3} {4} {0}");
			AddString(SchedulerStringId.TextYearlyPattern_YearString2, "the {5} {6} of {3} {0}");
			AddString(SchedulerStringId.TextYearlyPattern_YearsString1, "{3} {4} of every {1} {2} {0}");
			AddString(SchedulerStringId.TextYearlyPattern_YearsString2, "the {5} {6} of {3} every {1} {2} {0}");
			AddString(SchedulerStringId.Caption_AllDay, "All day");
			AddString(SchedulerStringId.Caption_PleaseSeeAbove, "Please see above");
			AddString(SchedulerStringId.Caption_RecurrenceSubject, "Subject:");
			AddString(SchedulerStringId.Caption_RecurrenceLocation, "Location:");
			AddString(SchedulerStringId.Caption_RecurrenceStartTime, "Start:");
			AddString(SchedulerStringId.Caption_RecurrenceEndTime, "End:");
			AddString(SchedulerStringId.Caption_RecurrenceShowTimeAs, "Show Time As:");
			AddString(SchedulerStringId.Caption_Recurrence, "Recurrence:");
			AddString(SchedulerStringId.Caption_RecurrencePattern, "Recurrence Pattern:");
			AddString(SchedulerStringId.Caption_NoneRecurrence, "(none)");
			AddString(SchedulerStringId.TextAppointmentSnapToCells_Always, "Always");
			AddString(SchedulerStringId.TextAppointmentSnapToCells_Auto, "Auto");
			AddString(SchedulerStringId.TextAppointmentSnapToCells_Disabled, "Disabled");
			AddString(SchedulerStringId.TextAppointmentSnapToCells_Never, "Never");
			AddString(SchedulerStringId.MemoPrintDateFormat, "{0} {1} {2}");
			AddString(SchedulerStringId.Caption_EmptyResource, "(Any)");
			AddString(SchedulerStringId.Caption_DailyPrintStyle, "Daily Style");
			AddString(SchedulerStringId.Caption_WeeklyPrintStyle, "Weekly Style");
			AddString(SchedulerStringId.Caption_MonthlyPrintStyle, "Monthly Style");
			AddString(SchedulerStringId.Caption_TrifoldPrintStyle, "Tri-fold Style");
			AddString(SchedulerStringId.Caption_CalendarDetailsPrintStyle, "Calendar Details Style");
			AddString(SchedulerStringId.Caption_MemoPrintStyle, "Memo Style");
			AddString(SchedulerStringId.Caption_ColorConverterFullColor, "Full Color");
			AddString(SchedulerStringId.Caption_ColorConverterGrayScale, "Gray Scale");
			AddString(SchedulerStringId.Caption_ColorConverterBlackAndWhite, "Black And White");
			AddString(SchedulerStringId.Caption_ResourceNone, "(None)");
			AddString(SchedulerStringId.Caption_ResourceAll, "(All)");
			AddString(SchedulerStringId.PrintPageSetupFormatTabControlCustomizeShading, "<Customize...>");
			AddString(SchedulerStringId.PrintPageSetupFormatTabControlSizeAndFontName, "{0} pt. {1}");
			AddString(SchedulerStringId.PrintRangeControlInvalidDate, "End date must be greater or equal to the start date");
			AddString(SchedulerStringId.PrintCalendarDetailsControlDayPeriod, "Day");
			AddString(SchedulerStringId.PrintCalendarDetailsControlWeekPeriod, "Week");
			AddString(SchedulerStringId.PrintCalendarDetailsControlMonthPeriod, "Month");
			AddString(SchedulerStringId.PrintMonthlyOptControlOnePagePerMonth, "1 page/month");
			AddString(SchedulerStringId.PrintMonthlyOptControlTwoPagesPerMonth, "2 pages/month");
			AddString(SchedulerStringId.PrintTimeIntervalControlInvalidDuration, "Duration must be not greater than a day and greater than 0");
			AddString(SchedulerStringId.PrintTimeIntervalControlInvalidStartEndTime, "End time must be greater than the start time");
			AddString(SchedulerStringId.PrintTriFoldOptControlDailyCalendar, "Daily Calendar");
			AddString(SchedulerStringId.PrintTriFoldOptControlWeeklyCalendar, "Weekly Calendar");
			AddString(SchedulerStringId.PrintTriFoldOptControlMonthlyCalendar, "Monthly Calendar");
			AddString(SchedulerStringId.PrintWeeklyOptControlOneWeekPerPage, "1 page/week");
			AddString(SchedulerStringId.PrintWeeklyOptControlTwoWeekPerPage, "2 pages/week");
			AddString(SchedulerStringId.PrintPageSetupFormCaption, "Print Options: {0}");
			AddString(SchedulerStringId.PrintMoreItemsMsg, "More items...");
			AddString(SchedulerStringId.PrintNoPrintersInstalled, "No printers installed");
			AddString(SchedulerStringId.Caption_FirstVisibleResources, "First");
			AddString(SchedulerStringId.Caption_PrevVisibleResourcesPage, "Previous Page");
			AddString(SchedulerStringId.Caption_PrevVisibleResources, "Previous");
			AddString(SchedulerStringId.Caption_NextVisibleResources, "Next");
			AddString(SchedulerStringId.Caption_NextVisibleResourcesPage, "Next Page");
			AddString(SchedulerStringId.Caption_LastVisibleResources, "Last");
			AddString(SchedulerStringId.Caption_IncreaseVisibleResourcesCount, "Increase visible resources count");
			AddString(SchedulerStringId.Caption_DecreaseVisibleResourcesCount, "Decrease visible resources count");
			AddString(SchedulerStringId.Caption_ShadingApplyToAllDayArea, "All Day Area");
			AddString(SchedulerStringId.Caption_ShadingApplyToAppointments, "Appointments");
			AddString(SchedulerStringId.Caption_ShadingApplyToAppointmentStatuses, "Appointment statuses");
			AddString(SchedulerStringId.Caption_ShadingApplyToHeaders, "Headers");
			AddString(SchedulerStringId.Caption_ShadingApplyToTimeRulers, "Time Rulers");
			AddString(SchedulerStringId.Caption_ShadingApplyToCells, "Cells");
			AddString(SchedulerStringId.Msg_InvalidSize, "Invalid value specified for the size.");
			AddString(SchedulerStringId.Msg_ApplyToAllStyles, "Apply current printer settings to all styles?");
			AddString(SchedulerStringId.Msg_MemoPrintNoSelectedItems, "Cannot print unless an item is selected. Select an item, and then try to print again.");
			AddString(SchedulerStringId.Caption_AllResources, "All resources");
			AddString(SchedulerStringId.Caption_VisibleResources, "Visible resources");
			AddString(SchedulerStringId.Caption_OnScreenResources, "OnScreen resources");
			AddString(SchedulerStringId.Caption_GroupByNone, "None");
			AddString(SchedulerStringId.Caption_GroupByDate, "Date");
			AddString(SchedulerStringId.Caption_GroupByResources, "Resources");
			AddString(SchedulerStringId.Msg_InvalidInputFile, "Input file is invalid");
			AddString(SchedulerStringId.TextRecurrenceTypeDaily, "Daily");
			AddString(SchedulerStringId.TextRecurrenceTypeWeekly, "Weekly");
			AddString(SchedulerStringId.TextRecurrenceTypeMonthly, "Monthly");
			AddString(SchedulerStringId.TextRecurrenceTypeYearly, "Yearly");
			AddString(SchedulerStringId.TextRecurrenceTypeMinutely, "Minutely");
			AddString(SchedulerStringId.TextRecurrenceTypeHourly, "Hourly");
			AddString(SchedulerStringId.Msg_Warning, "Warning!");
			AddString(SchedulerStringId.Msg_CantFitIntoPage, "It's impossible to fit the printing output into a single page using the current printing settings. Please try to increase the page height or decrease the PrintTime interval.");
			AddString(SchedulerStringId.Msg_PrintStyleNameExists, "The style name '{0}' already exists. Type another name.");
			AddString(SchedulerStringId.Msg_OutlookCalendarNotFound, "The '{0}' calendar is not found.");
			AddString(SchedulerStringId.Caption_PrevAppointment, "Previous Appointment");
			AddString(SchedulerStringId.Caption_NextAppointment, "Next Appointment");
			AddString(SchedulerStringId.DisplayName_Appointment, "Appointment");
			AddString(SchedulerStringId.Format_CopyOf, "Copy of {0}");
			AddString(SchedulerStringId.Format_CopyNOf, "Copy ({0}) of {1}");
			AddString(SchedulerStringId.Msg_MissingRequiredMapping, "The required mapping for the '{0}' property is missing.");
			AddString(SchedulerStringId.Msg_MissingMappingMember, "Missing '{1}' member of the '{0}' property mapping.");
			AddString(SchedulerStringId.Msg_DuplicateMappingMember, "The '{0}' member mapping is not unique: ");
			AddString(SchedulerStringId.Msg_InconsistentRecurrenceInfoMapping, "To support recurrence you must map both RecurrenceInfo and Type members.");
			AddString(SchedulerStringId.Msg_IncorrectMappingsQuestion, "Incorrect mappings. Continue anyway?\r\nDetails:\r\n");
			AddString(SchedulerStringId.Msg_DuplicateCustomFieldMappings, "Duplicate custom field name. Revise the mappings: \r\n{0}");
			AddString(SchedulerStringId.Msg_MappingsCheckPassedOk, "Mappings are correct!");
			AddString(SchedulerStringId.Caption_SetupAppointmentMappings, "Setup Appointment Mappings");
			AddString(SchedulerStringId.Caption_SetupResourceMappings, "Setup Resource Mappings");
			AddString(SchedulerStringId.Caption_SetupDependencyMappings, "Setup Dependency Mappings");
			AddString(SchedulerStringId.Caption_ModifyAppointmentMappingsTransactionDescription, "Modify Appointment Mappings");
			AddString(SchedulerStringId.Caption_ModifyResourceMappingsTransactionDescription, "Modify Resource Mappings");
			AddString(SchedulerStringId.Caption_ModifyAppointmentDependencyMappingsTransactionDescription, "Modify Appointment Dependency Mappings");
			AddString(SchedulerStringId.Caption_MappingsValidation, "Mappings Validation");
			AddString(SchedulerStringId.Caption_MappingsWizard, "Mappings Wizard...");
			AddString(SchedulerStringId.Caption_CheckMappings, "Check Mappings");
			AddString(SchedulerStringId.Caption_SetupAppointmentStorage, "Setup Appointment Storage");
			AddString(SchedulerStringId.Caption_SetupResourceStorage, "Setup Resource Storage");
			AddString(SchedulerStringId.Caption_SetupAppointmentDependencyStorage, "Setup Dependency Storage");
			AddString(SchedulerStringId.Caption_ModifyAppointmentStorageTransactionDescription, "Modify Appointment Storage");
			AddString(SchedulerStringId.Caption_ModifyResourceStorageTransactionDescription, "Modify Resource Storage");
			AddString(SchedulerStringId.Caption_ModifyAppointmentDependencyStorageTransactionDescription, "Modify AppointmentDependency Storage");
			AddString(SchedulerStringId.Caption_DayViewDescription, "Switch to the Day view. The most detailed view of appointments for a specific day(s).");
			AddString(SchedulerStringId.Caption_WorkWeekViewDescription, "Switch to the Work Week view. Detailed view for the working days in a certain week.");
			AddString(SchedulerStringId.Caption_WeekViewDescription, "Switch to the Week view. Arranges appointments for a particular week in a compact form.");
			AddString(SchedulerStringId.Caption_MonthViewDescription, "Switch to the Month (Multi-Week) view. Calendar view useful for long-term plans.");
			AddString(SchedulerStringId.Caption_TimelineViewDescription, "Switch to the Timeline view. Plots appointments in relation to time.");
			AddString(SchedulerStringId.Caption_GanttViewDescription, "Switch to the Gantt View. Project management view that shows appointments and their dependencies in relation to time.");
			AddString(SchedulerStringId.Caption_FullWeekViewDescription, "Switch to the Full Week View. Arranges appointments for a particular week in a compact form.");
			AddString(SchedulerStringId.Caption_GroupByNoneDescription, "Ungroup appointments.");
			AddString(SchedulerStringId.Caption_GroupByResourceDescription, "Group appointments by resource.");
			AddString(SchedulerStringId.Caption_GroupByDateDescription, "Group appointments by date.");
			AddString(SchedulerStringId.MenuCmd_OpenOccurrence, "Open Occurrence");
			AddString(SchedulerStringId.DescCmd_OpenOccurrence, "Open this meeting occurrence.");
			AddString(SchedulerStringId.MenuCmd_OpenSeries, "Open Series");
			AddString(SchedulerStringId.DescCmd_OpenSeries, "Open this meeting series.");
			AddString(SchedulerStringId.Msg_iCalendar_NotValidFile, "Invalid Internet Calendar file");
			AddString(SchedulerStringId.Msg_iCalendar_AppointmentsImportWarning, "Cannot import some appointment");
			AddString(SchedulerStringId.DescCmd_SplitAppointment, "Split the selected appointment in two by dragging a splitter line.");
			AddString(SchedulerStringId.Caption_SplitAppointment, "Split");
			AddString(SchedulerStringId.DefaultToolTipStringFormat_SplitAppointment, "{0} : step {1}");
			AddString(SchedulerStringId.VS_SchedulerReportsToolboxCategoryName, "DX.{0}: Scheduler Reporting");
			AddString(SchedulerStringId.UD_SchedulerReportsToolboxCategoryName, "Scheduler Controls");
			AddString(SchedulerStringId.Reporting_NotAssigned_TimeCells, "Required TimeCells control is not assigned");
			AddString(SchedulerStringId.Reporting_NotAssigned_View, "Required View component is not assigned");
			AddString(SchedulerStringId.Msg_RecurrenceExceptionsWillBeLost, "Any exceptions associated with this recurring appointment will be lost. Proceed?");
			AddString(SchedulerStringId.DescCmd_CreateAppointmentDependency, "Create dependency between appointments");
			AddString(SchedulerStringId.MenuCmd_CreateAppointmentDependency, "Create Dependency");
			AddString(SchedulerStringId.Caption_AppointmentDependencyTypeFinishToStart, "Finish-to-start (FS)");
			AddString(SchedulerStringId.Caption_AppointmentDependencyTypeStartToStart, "Start-to-start (SS)");
			AddString(SchedulerStringId.Caption_AppointmentDependencyTypeFinishToFinish, "Finish-to-finish (FF)");
			AddString(SchedulerStringId.Caption_AppointmentDependencyTypeStartToFinish, "Start-to-finish (SF)");
			AddString(SchedulerStringId.MenuCmd_PrintPreview, "Print &Preview");
			AddString(SchedulerStringId.DescCmd_PrintPreview, "Preview and make changes to pages before printing."); 
			AddString(SchedulerStringId.MenuCmd_Print, "Quick Print");
			AddString(SchedulerStringId.DescCmd_Print, "Send the schedule directly to the default printer without making changes."); 
			AddString(SchedulerStringId.MenuCmd_PrintPageSetup, "Page &Setup");
			AddString(SchedulerStringId.DescCmd_PrintPageSetup, "Customize the page appearance and configure various printing options.");
			AddString(SchedulerStringId.DescCmd_TimeScalesMenu, "Change the time scale.");
			AddString(SchedulerStringId.MenuCmd_ShowWorkTimeOnly, "Working Hours");
			AddString(SchedulerStringId.DescCmd_ShowWorkTimeOnly, "Show only working hours in the calendar.");
			AddString(SchedulerStringId.MenuCmd_CompressWeekend, "Compress Weekend");
			AddString(SchedulerStringId.DescCmd_CompressWeekend, "Show Saturday and Sunday compressed into a single column.");
			AddString(SchedulerStringId.MenuCmd_CellsAutoHeight, "Cell Auto Height");
			AddString(SchedulerStringId.DescCmd_CellsAutoHeight, "Enable a time cell to automatically adjust its size to accommodate appointments it contains.");
			AddString(SchedulerStringId.MenuCmd_ToggleRecurrence, "Recurrence");
			AddString(SchedulerStringId.DescCmd_ToggleRecurrence, "Make the selected appointment recurring, or edit the recurrence pattern. ");
			AddString(SchedulerStringId.MenuCmd_ChangeAppointmentReminderUI, "Reminder");
			AddString(SchedulerStringId.DescCmd_ChangeAppointmentReminderUI, "Choose when to be reminded of the selected appointment.");
			AddString(SchedulerStringId.MenuCmd_ChangeTimelineScaleWidth, "Scale Width");
			AddString(SchedulerStringId.DescCmd_ChangeTimelineScaleWidth, "Specify column width in pixels for the base scale.");
			AddString(SchedulerStringId.MenuCmd_SaveSchedule, "Save");
			AddString(SchedulerStringId.DescCmd_SaveSchedule, "Save a schedule to a file (.ics).");
			AddString(SchedulerStringId.MenuCmd_OpenSchedule, "Open");
			AddString(SchedulerStringId.DescCmd_OpenSchedule, "Import a schedule from a file (.ics).");
			AddString(SchedulerStringId.MenuCmd_ChangeSnapToCellsUI, "Snap to Cells");
			AddString(SchedulerStringId.DescCmd_ChangeSnapToCellsUI, "Specify a snapping mode for displaying appointments within time cells.");
			AddString(SchedulerStringId.Caption_NoneReminder, "None");
			AddString(SchedulerStringId.DateTimeAutoFormat_WithoutYear, "");
			AddString(SchedulerStringId.DateTimeAutoFormat_Week, "");
			AddString(SchedulerStringId.Msg_SaveBeforeClose, "Want to save your changes?");
		}
		#endregion
		public static XtraLocalizer<SchedulerStringId> CreateDefaultLocalizer() {
			return new SchedulerResLocalizer();
		}
		public static string GetString(SchedulerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SchedulerStringId> CreateResXLocalizer() {
			return new SchedulerResLocalizer();
		}
		protected override void AddString(SchedulerStringId id, string str) {
			Dictionary<SchedulerStringId, string> table = XtraLocalizierHelper<SchedulerStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
}
