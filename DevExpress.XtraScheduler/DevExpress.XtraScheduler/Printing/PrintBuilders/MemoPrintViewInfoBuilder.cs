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
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class MemoPrintViewInfoBuilder : ReportPrintViewInfoBuilder {
		AppointmentBaseCollection selectedAppointments;
		public MemoPrintViewInfoBuilder(MemoPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
		}
		protected new MemoPrintStyle PrintStyle { get { return (MemoPrintStyle)base.PrintStyle; } }
		public override IPrintableObjectViewInfo CreateViewInfo(Rectangle pageBounds) {
			XtraSchedulerDebug.Assert(Control.SelectedAppointments != null);
			selectedAppointments = Control.SelectedAppointments;
			int count = selectedAppointments.Count;
			for (int i = 0; i < count; i++)
				AddAppointment(selectedAppointments[i]);
			Table.Measure(GInfo.Cache, pageBounds.Width - 1);
			Rectangle pageBoundsInflate = pageBounds;
			pageBoundsInflate.Inflate(new Size(-1, -1));
			Table.Arrange(pageBoundsInflate);
			return Table;
		}
		protected internal virtual void AddAppointment(Appointment appointment) {
			String title = appointment.Subject;
			IAppointmentViewInfo aptViewInfo = new NonVisualAppointmentViewInfo(appointment);
			AppointmentDisplayTextEventArgs args = new AppointmentDisplayTextEventArgs(aptViewInfo, title, appointment.Description);
			Control.RaiseInitAppointmentDisplayText(args);
			AddHeader(appointment);
			AddLine();
			AddSubject(args.Text);
			AddLocation(appointment);
			AddEmptyRow();
			AddTimeIntervalInfo(appointment);
			AddEmptyRow();
			AddRecurrenceInformation(appointment);
			AddEmptyRow();
			AddDetailText(args.Description);
			if (PrintStyle.BreakPageAfterEachItem)
				AddPageBreak();
			else
				AddPaddingDownRow();
		}
		protected internal virtual void AddHeader(Appointment appointment) {
			RowPrintViewInfo headerRow = CreateHeaderRowViewInfo(appointment);
			Table.AddRow(headerRow);
		}
		internal RowPrintViewInfo CreateHeaderRowViewInfo(Appointment appointment) {
			string headerText = GetHeaderText(appointment.ResourceIds);
			TextCellPrintViewInfo headerTextCell = new TextCellPrintViewInfo(headerText, HeaderAppearance);
			RowPrintViewInfo headerRow = new RowPrintViewInfo();
			headerRow.AddCell(headerTextCell);
			return headerRow;
		}
		protected internal virtual string GetHeaderText(AppointmentResourceIdCollection ids) {
			StringBuilder result = new StringBuilder();
			int count = ids.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = Control.DataStorage.Resources.GetResourceById(ids[i]);
				string resourceName = resource.Caption;
				if (String.IsNullOrEmpty(resourceName))
					continue;
				result.Append(resourceName);
				if (i != count - 1)
					result.Append(", ");
			}
			return result.ToString();
		}
		protected internal virtual void AddSubject(string subject) {
			RowPrintViewInfo subjectRow = CreateRow(SchedulerStringId.Caption_RecurrenceSubject, subject);
			Table.AddRow(subjectRow);
		}
		protected internal virtual void AddLocation(Appointment appointment) {
			XtraSchedulerDebug.Assert(appointment != null);
			RowPrintViewInfo locationRow = CreateRow(SchedulerStringId.Caption_RecurrenceLocation, appointment.Location);
			Table.AddRow(locationRow);
		}
		protected internal virtual void AddTimeIntervalInfo(Appointment appointment) {
			TimeZoneHelper helper = Control.InnerControl.TimeZoneHelper;
			TimeInterval clientAppointmentInterval = helper.ToClientTime(((IInternalAppointment)appointment).CreateInterval(), null);
			XtraSchedulerDebug.Assert(Control.DataStorage != null);
			string startTimeText = GetTimeText(clientAppointmentInterval.Start);
			string endTimeText = GetTimeText(clientAppointmentInterval.End);
			string statusText = Control.GetStatus(appointment.StatusKey).DisplayName;
			RowPrintViewInfo timeStartRow = CreateRow(SchedulerStringId.Caption_RecurrenceStartTime, startTimeText);
			RowPrintViewInfo timeEndRow = CreateRow(SchedulerStringId.Caption_RecurrenceEndTime, endTimeText);
			RowPrintViewInfo statusRow = CreateRow(SchedulerStringId.Caption_RecurrenceShowTimeAs, statusText);
			Table.AddRow(timeStartRow);
			Table.AddRow(timeEndRow);
			Table.AddRow(statusRow);
		}
		protected internal virtual string GetTimeText(DateTime date) {
			return String.Format(SchedulerLocalizer.GetString(SchedulerStringId.MemoPrintDateFormat),
				date.ToString("ddd"), date.ToString("d"), DateTimeFormatHelper.DateToShortTimeString(date));
		}
		protected internal virtual void AddRecurrenceInformation(Appointment appointment) {
			string recurrenceDescriptionText = GetRecurrenceDescriptionText(appointment);
			string recurrenceDescriptionPatternText = GetRecurrencePatternText(appointment);
			RowPrintViewInfo recurrenceDescriptionRow = CreateRow(SchedulerStringId.Caption_RecurrencePattern, recurrenceDescriptionText);
			RowPrintViewInfo recurrenceDescriptionPatternRow = string.IsNullOrEmpty(recurrenceDescriptionPatternText) ? null :
																		CreateRow(SchedulerStringId.Caption_RecurrencePattern, recurrenceDescriptionPatternText);
			Table.AddRow(recurrenceDescriptionRow);
			if (recurrenceDescriptionPatternRow == null)
				return;
			Table.AddRow(recurrenceDescriptionPatternRow);
		}
		protected internal virtual string GetRecurrenceDescriptionText(Appointment appointment) {
			if (appointment.RecurrenceInfo == null)
				return SchedulerLocalizer.GetString(SchedulerStringId.Caption_NoneRecurrence);
			switch (appointment.RecurrenceInfo.Type) {
				case RecurrenceType.Daily:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeDaily);
				case RecurrenceType.Hourly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeHourly);
				case RecurrenceType.Minutely:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMinutely);
				case RecurrenceType.Monthly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeMonthly);
				case RecurrenceType.Weekly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeWeekly);
				case RecurrenceType.Yearly:
					return SchedulerLocalizer.GetString(SchedulerStringId.TextRecurrenceTypeYearly);
				default:
					XtraSchedulerDebug.Assert(false);
					return String.Empty;
			}
		}
		protected internal virtual string GetRecurrencePatternText(Appointment appointment) {
			if (appointment.RecurrenceInfo == null)
				return String.Empty;
			RecurrenceDescriptionBuilder reccurenceDescriptor = RecurrenceDescriptionBuilder.CreateInstance(appointment, Control.InnerControl.TimeZoneHelper, Control.FirstDayOfWeek);
			return reccurenceDescriptor.BuildString();
		}
		protected internal virtual void AddDetailText(string descriptionText) {
			if (String.IsNullOrEmpty(descriptionText))
				return;
			RowPrintViewInfo descriptionTextRow = CreateContentRow(descriptionText);
			Table.AddRow(descriptionTextRow);
		}
	}
}
