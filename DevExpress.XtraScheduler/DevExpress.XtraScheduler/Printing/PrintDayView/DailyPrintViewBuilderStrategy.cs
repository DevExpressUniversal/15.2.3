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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class DailyPrintViewBuilderStrategy : SchedulerSinglePrintViewBuilderStrategy {
		public DailyPrintViewBuilderStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle)
			: base(gInfo, control, printStyle) {
		}
		public override PageLayout Layout { get { return PageLayout.OnePage; } }
		protected internal override float SecondLineTextSizeMultiplier { get { return 0.618f; } }
		public override ViewPart CalculateFirstViewPart() {
			return ViewPart.Both;
		}
		public static void SetViewParametersCore(DayPrintView dayPrintView, DayView dayView) {
			dayPrintView.AppointmentDisplayOptions.Assign(dayView.AppointmentDisplayOptions);
			dayPrintView.AppointmentDisplayOptions.ShowShadows = false;
			dayPrintView.WorkTime = dayView.WorkTime.Clone();
			dayPrintView.GroupSeparatorWidth = dayView.GroupSeparatorWidth;
			dayPrintView.StatusLineWidth = dayView.StatusLineWidth;
			dayPrintView.ShowAllDayArea = dayView.ShowAllDayArea;
			dayPrintView.ShowDayHeaders = dayView.ShowDayHeaders;
			dayPrintView.ShowMoreButtons = false;
			dayPrintView.ShowMoreButtonsOnEachColumn = false;
			dayPrintView.ShowWorkTimeOnly = false;
			dayPrintView.GroupSeparatorWidth = dayView.GroupSeparatorWidth;
			dayPrintView.TimeRulers.Clear();
			dayPrintView.TimeRulers.AddRange(dayView.TimeRulers);
			TimeSlotCollection timeSlots = dayPrintView.TimeSlots;
			timeSlots.AddRange(dayView.TimeSlots);
			for (int i = timeSlots.Count - 2; i >= 0; i--) {
				if (timeSlots[i].Value == timeSlots[i + 1].Value)
					timeSlots.RemoveAt(i);
			}
		}
		public static void SetScaleForDayView(SchedulerPrintStyle printStyle, Rectangle bounds, DayPrintView view) {
			bool useActiveDayViewScale = false;
			TimeSlotCollection timeSlots = view.Control.DayView.TimeSlots;
			DailyPrintStyle dailyStyle = printStyle as DailyPrintStyle;
			if (dailyStyle != null) {
				useActiveDayViewScale = dailyStyle.UseActiveViewTimeScale;
				timeSlots = dailyStyle.TimeSlots;
			} else {
				WeeklyPrintStyle weeklyStyle = printStyle as WeeklyPrintStyle;
				if (weeklyStyle != null) {
					useActiveDayViewScale = weeklyStyle.UseActiveViewTimeScale;
					timeSlots = weeklyStyle.TimeSlots;
				}
			}
			if (useActiveDayViewScale) {
				DayView activeDayView = view.Control.ActiveView as DayView;
				if (activeDayView == null)
					activeDayView = view.Control.Views.DayView;
				view.TimeScale = activeDayView.TimeScale;
				view.RecalcPreliminaryLayout(bounds);
				view.RecalcScrollBarVisibility(bounds);
				if (view.ViewInfo.FitIntoBounds) {
					view.RecalcFinalLayout(bounds);
					return;
				}
			}
			int count = timeSlots.Count;
			for (int i = count - 1; i >= 0; i--) {
				view.TimeScale = timeSlots[i].Value;
				view.RecreateViewInfo();
				view.ClearPreliminaryAppointmentsAndCellContainers();
				view.RecalcPreliminaryLayout(bounds);
				view.RecalcScrollBarVisibility(bounds);
				if (view.ViewInfo.FitIntoBounds) {
					view.RecalcFinalLayout(bounds);
					return;
				}
			}
			throw new Exception(SchedulerLocalizer.GetString(SchedulerStringId.Msg_CantFitIntoPage));
		}
		protected internal override TimeInterval AlignInterval(TimeInterval interval, DayOfWeek firstDayOfWeek) {
			return new TimeInterval(interval.Start.Date, DateTimeHelper.DaySpan);
		}
		protected internal override string CalculateFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			return SysDate.ToString(pattern, currentInterval.Start);
		}
		protected internal override string CalculateSecondLineText(TimeInterval currentInterval) {
			return SysDate.ToString("dddd", currentInterval.Start);
		}
		protected internal override SchedulerViewBase CreatePrintView(SchedulerControl control, GraphicsInfo gInfo, ViewPart viewPart) {
			return new DayPrintView(control, gInfo, PrintStyle);
		}
		protected internal override void SetViewParameters(SchedulerViewBase view, TimeInterval currentInterval) {
			base.SetViewParameters(view, currentInterval);
			DayPrintView dayPrintView = (DayPrintView)view;
			DailyPrintStyle dailyPrintStyle = PrintStyle as DailyPrintStyle;
			if (dailyPrintStyle != null)
				dayPrintView.VisibleTime = dailyPrintStyle.PrintTime;
			SchedulerControl control = view.Control;
			DayView dayView = control.DayView;
			SetViewParametersCore(dayPrintView, dayView);
		}
		protected internal override void CalculateViewInfo(Rectangle pageBounds, SchedulerViewBase view) {
			SetScaleForDayView(PrintStyle, pageBounds, (DayPrintView)view);
		}
	}
}
