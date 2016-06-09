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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public static class DayViewCellsCalculatorHelper {
		public static TimeOfDayInterval CreateAlignedVisibleTime(TimeOfDayInterval visibleTime, TimeSpan timeScale, bool visibleTimeSnapMode) {
			TimeSpan start, end;
			if (!visibleTimeSnapMode) {
				start = DateTimeHelper.Floor(visibleTime.Start, timeScale);
				end = DateTimeHelper.Ceil(visibleTime.End, timeScale);
			} else {
				start = visibleTime.Start;
				end = DateTimeHelper.Ceil(visibleTime.End, timeScale, visibleTime.Start);
			}
			return new TimeOfDayInterval(start, end);
		}
		public static int CalculateMinAppointmentHeight(AppearanceObject aptAppearance, object aptImages, AppointmentPainter painter, GraphicsCache cache) {
			int textHeight = aptAppearance.CalcDefaultTextSize(cache.Graphics).Height;
			int imageHeight = ImageCollection.GetImageListSize(aptImages).Height;
			int bordersWithPaddings = painter.GetSameDayTopBorderWidth() + painter.GetSameDayTopContentPadding() + painter.GetSameDayBottomContentPadding() + painter.GetSameDayBottomBorderWidth();
			return Math.Max(textHeight, imageHeight) + bordersWithPaddings;
		}
		public static int CalculateTimeIntervalsCount(TimeOfDayInterval alignedVisibleTime, TimeSpan timeScale) {
			int timeIntervalsCount = DateTimeHelper.Divide(alignedVisibleTime.Duration, timeScale);
			if (alignedVisibleTime.Duration.Ticks % timeScale.Ticks != 0)
				timeIntervalsCount++;
			return timeIntervalsCount;
		}
		public static TimeSpan GetValidEndRowTime(TimeSpan endRowTime, TimeOfDayInterval visibleTime) {
			XtraSchedulerDebug.Assert(endRowTime >= visibleTime.Start);
			return endRowTime;
		}
		public static int CalculateAllDayAreaHeight(AppointmentPainter painter, AppointmentIntermediateViewInfoCollection allDayAppointments, int minRowHeight) {
			int result = CalculateAllDayAreaContentHeight(allDayAppointments) + painter.TopPadding + painter.BottomPadding;
			return Math.Max(result, minRowHeight);
		}
		internal static int CalculateAllDayAreaContentHeight(AppointmentIntermediateViewInfoCollection allDayAppointments) {
			int count = allDayAppointments.Count;
			int result = 0;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, allDayAppointments[i].Bounds.Bottom);
			return result;
		}
		public static void CreatePreliminaryExtendedCells(SchedulerViewCellContainerCollection columns, AppointmentBaseCollection appointments, int minExtendedCellsInColumn, TimeOfDayInterval visibleTime) {
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				DayViewColumn column = (DayViewColumn)columns[i];
				CreatePreliminaryExtendedCellsCore(column, appointments, visibleTime);
			}
			int cellsCount = CalculateMaxExtendedCellsCount(columns, minExtendedCellsInColumn);
			AdjustExtendedCells(columns, cellsCount);
		}
		static void CreatePreliminaryExtendedCellsCore(DayViewColumn column, AppointmentBaseCollection appointments, TimeOfDayInterval visibleTime) {
			SchedulerViewCellBaseCollection extendedCells = GetExtendedCells(column, appointments, visibleTime);
			column.ExtendedCells.Clear();
			column.ExtendedCells.AddRange(extendedCells);
		}
		internal static SchedulerViewCellBaseCollection GetExtendedCells(DayViewColumn column, AppointmentBaseCollection appointments, TimeOfDayInterval visibleTime) {
			SchedulerViewCellBaseCollection result = new SchedulerViewCellBaseCollection();
			TimeInterval interval = column.Interval;
			AppointmentBaseCollection extendedAppointments = GetExtendedAppointments(appointments, interval, column.Resource.Id, visibleTime);
			int count = extendedAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = extendedAppointments[i];
				TimeInterval intersection = interval.Intersect(((IInternalAppointment)apt).CreateInterval());
				string text = GetDisplayPartialText(apt, interval.Start.Date);
				ExtendedCell cell = CreateExtendedCell(apt, column, intersection, text);
				result.Add(cell);
			}
			result.Sort(new ExtendedCellsComparer());
			return result;
		}
		static int CalculateMaxExtendedCellsCount(SchedulerViewCellContainerCollection columns, int minExtendedCellsInColumn) {
			int maxCellsCount = minExtendedCellsInColumn;
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				DayViewColumn column = (DayViewColumn)columns[i];
				maxCellsCount = Math.Max(column.ExtendedCells.Count, maxCellsCount);
			}
			return maxCellsCount;
		}
		static void AdjustExtendedCells(SchedulerViewCellContainerCollection columns, int extendedCellsCount) {
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				CreatePreliminaryEmptyExtendedCells((DayViewColumn)columns[i], extendedCellsCount);
		}
		static void CreatePreliminaryEmptyExtendedCells(DayViewColumn column, int totalExtendedCellsCount) {
			SchedulerViewCellBaseCollection cells = new SchedulerViewCellBaseCollection();
			int count = totalExtendedCellsCount - column.ExtendedCells.Count;
			for (int i = 0; i < count; i++)
				cells.Add(CreateEmptyExtendedCell(column));
			column.ExtendedCells.AddRange(cells);
		}
		static ExtendedCell CreateEmptyExtendedCell(DayViewColumn column) {
			return CreateExtendedCell(null, column, TimeInterval.Empty, String.Empty);
		}
		static ExtendedCell CreateExtendedCell(Appointment apt, DayViewColumn column, TimeInterval interval, string text) {
			ExtendedCell cell = new ExtendedCell(apt, text, interval);
			column.InitializeCell(cell, interval);
			return cell;
		}
		internal static string GetDisplayPartialText(Appointment apt, DateTime date) {
			DateTime startDate = apt.Start < date ? date : apt.Start;
			DateTime endDate = apt.End > date.AddDays(1) ? date : apt.End;
			string s = DateTimeFormatHelper.DateToShortTimeString(startDate);
			if (apt.Duration.Ticks > 0)
				s += "-" + DateTimeFormatHelper.DateToShortTimeString(endDate);
			if (s.Length > 0)
				s += " ";
			return s + GetFormattedDisplayText(apt);
		}
		static string GetFormattedDisplayText(Appointment apt) {
			string s = apt.Subject;
			if (!String.IsNullOrEmpty(apt.Location))
				s += " (" + apt.Location + ")";
			return s;
		}
		internal static AppointmentBaseCollection GetExtendedAppointments(AppointmentBaseCollection appointments, TimeInterval columnInterval, object resourceId, TimeOfDayInterval visibleTime) {
			int count = appointments.Count;
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (apt.LongerThanADay || !((IInternalAppointment)apt).CreateInterval().IntersectsWithExcludingBounds(columnInterval))
					continue;
				if (IsAppointmentVisible(apt, visibleTime))
					continue;
				if (!ResourceBase.InternalMatchIdToResourceIdCollection(apt.ResourceIds, resourceId))
					continue;
				result.Add(apt);
			}
			return result;
		}
		static bool IsAppointmentVisible(Appointment appointment, TimeOfDayInterval visibleTime) {
			if (appointment.LongerThanADay)
				return true;
			TimeSpan actualVisibleTimeStart = TimeSpan.FromTicks(visibleTime.Start.Ticks % DateTimeHelper.DaySpan.Ticks);
			TimeSpan relativeAppointmentStartTime = appointment.Start.TimeOfDay - actualVisibleTimeStart;
			TimeSpan relativeAppointmentEndTime = relativeAppointmentStartTime + appointment.Duration;
			if (relativeAppointmentStartTime.Ticks >= visibleTime.Duration.Ticks)
				return false;
			if (relativeAppointmentEndTime.Ticks <= 0) {
				relativeAppointmentStartTime += DateTimeHelper.DaySpan;
				if (relativeAppointmentStartTime.Ticks >= visibleTime.Duration.Ticks)
					return false;
			}
			return true;
		}
	}
}
