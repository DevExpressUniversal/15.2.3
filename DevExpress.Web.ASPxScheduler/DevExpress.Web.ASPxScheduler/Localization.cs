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
using DevExpress.Utils.Localization;
using System.Resources;
using DevExpress.Web.Localization;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Web.ASPxScheduler.Localization {
	public enum ASPxSchedulerStringId {
		Caption_RecurrenceType_Hourly,
		Caption_RecurrenceType_Daily,
		Caption_RecurrenceType_Weekly,
		Caption_RecurrenceType_Monthly,
		Caption_RecurrenceType_Yearly,
		Caption_RecurrenceRange_NoEndDate,
		Caption_RecurrenceRange_OccurrenceCount,
		Caption_RecurrenceRange_EndBy,
		Caption_Days,
		Caption_Hour,
		Caption_Every,
		Caption_EveryWeekday,
		Caption_RecurEvery,
		Caption_WeeksOn,
		Caption_Day,
		Caption_The,
		Caption_OfEvery,
		Caption_Months,
		Caption_Of,
		Caption_Occurrences,
		Caption_Recurrence,
		Caption_GotoDate,
		Caption_DeleteRecurrentApt,
		Caption_OpenRecurrentApt,
		TooltipViewNavigator_Backward,
		TooltipViewNavigator_Forward,
		TooltipViewNavigator_GotoDate,
		CaptionViewNavigator_Today,
		Caption_ViewVisibleInterval_Format,
		Caption_OperationToolTip,
		Caption_Error,
		Caption_Warning,
		Caption_Info,
		Caption_DetailInfo,
		Caption_ShowMore,
		Caption_LoadError,
		Form_Subject,
		Form_Location,
		Form_Label,
		Form_StartTime,
		Form_EndTime,
		Form_Status,
		Form_AllDayEvent,
		Form_Resource,
		Form_Reminder,
		Form_ButtonOk,
		Form_ButtonCancel,
		Form_ButtonDelete,
		Form_Recurrence,
		Form_Date,
		Form_ShowIn,
		Form_Save,
		Form_OpenEditForm,
		Form_ConfirmDelete,
		Form_ConfirmEdit,
		Form_Series,
		Form_Occurrence,
		Form_SnoozeInfo,
		Form_ButtonDismiss,
		Form_ButtonDismissAll,
		Form_ButtonSnooze
	}
	public class ASPxSchedulerResLocalizer : ASPxResLocalizerBase<ASPxSchedulerStringId> {
		public ASPxSchedulerResLocalizer()
			: base(new ASPxSchedulerLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName { get { return AssemblyInfo.SRAssemblySchedulerWeb; } }
		protected override string ResxName {
			get { return "LocalizationRes"; } 
		}
	}
	public class ASPxSchedulerCoreResLocalizer : ASPxResLocalizerBase<SchedulerStringId> {
		public ASPxSchedulerCoreResLocalizer()
			: base(new SchedulerResLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName { get { return AssemblyInfo.SRAssemblySchedulerCore; } }
		protected override string ResxName { get { return "DevExpress.XtraScheduler.LocalizationRes"; } }
		internal static XtraLocalizer<SchedulerStringId> CreateResLocalizerInstance() {
			return new ASPxSchedulerCoreResLocalizer();
		}
	}
	public class ASPxSchedulerLocalizer : XtraLocalizer<ASPxSchedulerStringId> {
		static ASPxSchedulerLocalizer() {
			ASPxScheduler.EnsureLocalizer();
			if (GetActiveLocalizerProvider() == null)  
				SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxSchedulerStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxSchedulerStringId> CreateResLocalizerInstance() {
			return new ASPxSchedulerResLocalizer();
		}
		public static string GetString(ASPxSchedulerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxSchedulerStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(ASPxSchedulerStringId.Caption_Hour, "hour");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceType_Hourly, "Hourly");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceType_Daily, "Daily");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceType_Weekly, "Weekly");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceType_Monthly, "Monthly");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceType_Yearly, "Yearly");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceRange_EndBy, "End by:");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceRange_NoEndDate, "No end date");
			AddString(ASPxSchedulerStringId.Caption_RecurrenceRange_OccurrenceCount, "End after:");
			AddString(ASPxSchedulerStringId.Caption_Days, "day(s)");
			AddString(ASPxSchedulerStringId.Caption_Day, "Day");
			AddString(ASPxSchedulerStringId.Caption_Every, "Every");
			AddString(ASPxSchedulerStringId.Caption_EveryWeekday, "Every weekday");
			AddString(ASPxSchedulerStringId.Caption_RecurEvery, "Recur every");
			AddString(ASPxSchedulerStringId.Caption_WeeksOn, "week(s) on:");
			AddString(ASPxSchedulerStringId.Caption_The, "The");
			AddString(ASPxSchedulerStringId.Caption_OfEvery, "of  every");
			AddString(ASPxSchedulerStringId.Caption_Months, "month(s)");
			AddString(ASPxSchedulerStringId.Caption_Of, "of");
			AddString(ASPxSchedulerStringId.Caption_Occurrences, "occurrences");
			AddString(ASPxSchedulerStringId.Caption_Recurrence, "Recurrence");
			AddString(ASPxSchedulerStringId.CaptionViewNavigator_Today, "Today");
			AddString(ASPxSchedulerStringId.TooltipViewNavigator_Backward, "Backward");
			AddString(ASPxSchedulerStringId.TooltipViewNavigator_Forward, "Forward");
			AddString(ASPxSchedulerStringId.TooltipViewNavigator_GotoDate, "Go to Date");
			AddString(ASPxSchedulerStringId.Caption_ViewVisibleInterval_Format, "{0:D} &ndash; {1:D}");
			AddString(ASPxSchedulerStringId.Caption_OperationToolTip, "Press ESC to cancel operation");
			AddString(ASPxSchedulerStringId.Caption_Error, "Error");
			AddString(ASPxSchedulerStringId.Caption_Warning, "Warning");
			AddString(ASPxSchedulerStringId.Caption_Info, "Information");
			AddString(ASPxSchedulerStringId.Caption_DetailInfo, "Show detail info");
			AddString(ASPxSchedulerStringId.Caption_ShowMore, "Show more");
			AddString(ASPxSchedulerStringId.Caption_GotoDate, "Go To Date");
			AddString(ASPxSchedulerStringId.Caption_DeleteRecurrentApt, "Confirm Delete");
			AddString(ASPxSchedulerStringId.Caption_OpenRecurrentApt, "Open Recurring Item");
			AddString(ASPxSchedulerStringId.Caption_LoadError, "Can't load {0}");
			AddString(ASPxSchedulerStringId.Form_Subject, "Subject:");
			AddString(ASPxSchedulerStringId.Form_Location, "Location:");
			AddString(ASPxSchedulerStringId.Form_Label, "Label:");
			AddString(ASPxSchedulerStringId.Form_StartTime, "Start time:");
			AddString(ASPxSchedulerStringId.Form_EndTime, "End time:");
			AddString(ASPxSchedulerStringId.Form_Status, "Show time as:");
			AddString(ASPxSchedulerStringId.Form_AllDayEvent, "All day event");
			AddString(ASPxSchedulerStringId.Form_Resource, "Resource:");
			AddString(ASPxSchedulerStringId.Form_Reminder, "Reminder");
			AddString(ASPxSchedulerStringId.Form_ButtonOk, "OK");
			AddString(ASPxSchedulerStringId.Form_ButtonCancel, "Cancel");
			AddString(ASPxSchedulerStringId.Form_ButtonDelete, "Delete");
			AddString(ASPxSchedulerStringId.Form_Recurrence, "Recurrence");
			AddString(ASPxSchedulerStringId.Form_Date, "Date:");
			AddString(ASPxSchedulerStringId.Form_ShowIn, "Show in:");
			AddString(ASPxSchedulerStringId.Form_Save, "Save");
			AddString(ASPxSchedulerStringId.Form_OpenEditForm, "Open Edit Form...");
			AddString(ASPxSchedulerStringId.Form_ConfirmDelete, "Do you want to delete all occurrences of the recurring appointment \"{0}\", or just this one?");
			AddString(ASPxSchedulerStringId.Form_Series, "The series");
			AddString(ASPxSchedulerStringId.Form_Occurrence, "This occurrence");
			AddString(ASPxSchedulerStringId.Form_ConfirmEdit, "\"{0}\" is a recurring appointment. Do you want to open only this occurrence or the series?");
			AddString(ASPxSchedulerStringId.Form_SnoozeInfo, "Click Snooze to be reminded again in:");
			AddString(ASPxSchedulerStringId.Form_ButtonDismissAll, "Dismiss All");
			AddString(ASPxSchedulerStringId.Form_ButtonDismiss, "Dismiss");
			AddString(ASPxSchedulerStringId.Form_ButtonSnooze, "Snooze");
		}
		#endregion
	}
}
