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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class CalendarDetailsPrintViewInfoBuilder : ReportPrintViewInfoBuilder {
		AppointmentBaseCollection printedAppointments;
		TimeInterval currentInterval;
		public CalendarDetailsPrintViewInfoBuilder(CalendarDetailsPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
			currentInterval = null;
			printedAppointments = new AppointmentBaseCollection();
		}
		protected internal new CalendarDetailsPrintStyle PrintStyle { get { return (CalendarDetailsPrintStyle)base.PrintStyle; } }
		protected internal TimeInterval CurrentInerval { get { return currentInterval; } set { currentInterval = value; } }
		protected internal AppointmentBaseCollection PrintedAppointments { get { return printedAppointments; } set { printedAppointments = value; } }
		public override IPrintableObjectViewInfo CreateViewInfo(Rectangle bounds) {
			while (MoveNextDay())
				AddAppointments();
			Table.Measure(GInfo.Cache, bounds.Width);
			Table.ApplyRowLastColumnPadding = true;
			Table.Arrange(bounds);
			return Table;
		}
		protected internal virtual bool MoveNextDay() {
			if (currentInterval == null) {
				currentInterval = new TimeInterval(PrintStyle.StartRangeDate.Date, DateTimeHelper.DaySpan);
				return true;
			}
			currentInterval.Start += DateTimeHelper.DaySpan;
			return currentInterval.Start <= PrintStyle.EndRangeDate; 
		}
		protected internal virtual void AddAppointments() {
			AppointmentBaseCollection appointments = GetCurrentAppointments();
			int appointmentsCount = appointments.Count;
			if (appointmentsCount == 0)
				return;
			if (IsPageBreakRequire(currentInterval.Start))
				AddPageBreak();
			AddDayHeader();
			AddDayAppointments(appointments);
		}
		protected internal virtual AppointmentBaseCollection GetCurrentAppointments() {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			appointments.AddRange(Control.DataStorage.GetAppointments(currentInterval));
			return appointments;
		}
		protected internal virtual void AddDayHeader() {
			string headerText = GetHeaderText();
			RowPrintViewInfo headerRow = CreateHeaderRow(headerText);
			Table.AddRow(headerRow);
		}
		protected internal virtual string GetHeaderText() {
			string dayOfWeek = DateTimeFormatInfo.CurrentInfo.GetDayName(currentInterval.Start.DayOfWeek);
			string date = SysDate.ToString("dd MMMM yyyy", currentInterval.Start);
			return date + Environment.NewLine + dayOfWeek;
		}
		protected internal virtual void AddDayAppointments(AppointmentBaseCollection appointments) {
			int appointmentsCount = appointments.Count;
			for (int i = 0; i < appointmentsCount; i++) {
				AddAppointment(appointments[i]);
				AddEmptyRow();
			}
		}
		protected internal virtual void AddAppointment(Appointment appointment) {
			string timeIntervalText = GetTimeText(appointment);
			string detailText = GetDetailText(appointment);
			RowPrintViewInfo appointmentRow = CreateRow(timeIntervalText, detailText);
			Table.AddRow(appointmentRow);
			printedAppointments.Add(appointment);
		}
		protected internal virtual string GetTimeText(Appointment appointment) {
			XtraSchedulerDebug.Assert(currentInterval.Start.Ticks % DateTimeHelper.DaySpan.Ticks == 0);
			XtraSchedulerDebug.Assert(currentInterval.End.Ticks % DateTimeHelper.DaySpan.Ticks == 0);
			if (appointment.AllDay)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_AllDay);
			if (appointment.SameDay)
				return GetTimeIntervalString(appointment.Start, appointment.End);
			TimeInterval intersect = TimeInterval.Intersect(currentInterval, ((IInternalAppointment)appointment).CreateInterval());
			if (intersect.Duration == DateTimeHelper.DaySpan)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_AllDay);
			DateTime end = intersect.Start == currentInterval.Start ? intersect.End : currentInterval.End;
			return GetTimeIntervalString(intersect.Start, end);
		}
		protected internal virtual string GetTimeIntervalString(DateTime start, DateTime end) {
			return DateTimeFormatHelper.DateToShortTimeString(start) + " - " + DateTimeFormatHelper.DateToShortTimeString(end);
		}
		protected internal virtual string GetDetailText(Appointment appointment) {
			String title = appointment.Subject;
			if (!String.IsNullOrEmpty(appointment.Location))
				title += String.Format(" - {0}", appointment.Location);
			string appointmentDescription = GetAppointmentDescription(appointment);
			AppointmentDisplayTextEventArgs args = new AppointmentDisplayTextEventArgs(new NonVisualAppointmentViewInfo(appointment), title, appointmentDescription);
			Control.RaiseInitAppointmentDisplayText(args);
			StringBuilder result = new StringBuilder();
			result.Append(args.Text);
			if (!String.IsNullOrEmpty(args.Description)) {
				result.Append(Environment.NewLine);
				result.Append(args.Description);
			}
			return result.ToString();
		}
		protected internal string GetAppointmentDescription(Appointment appointment) {
			if (printedAppointments.Contains(appointment))
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_PleaseSeeAbove);
			else
				return appointment.Description;
		}
		protected internal virtual bool IsPageBreakRequire(DateTime date) {
			if (!PrintStyle.UseNewPagePeriod)
				return false;
			switch (PrintStyle.StartNewPagePeriod) {
				case PeriodKind.Day:
					return true;
				case PeriodKind.Week:
					return date.DayOfWeek == Control.FirstDayOfWeek;
				case PeriodKind.Month:
					return date.Day == 1;
				default:
					XtraSchedulerDebug.Assert(false);
					return false;
			}
		}
	}
}
