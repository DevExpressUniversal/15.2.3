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
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	internal class MonthlyPrintViewBuilderStrategy : SchedulerSinglePrintViewBuilderStrategy {
		public MonthlyPrintViewBuilderStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle)
			: base(gInfo, control, printStyle) {
		}
		public override PageLayout Layout {
			get {
				MonthlyPrintStyle monthlyPrintStyle = PrintStyle as MonthlyPrintStyle;
				return monthlyPrintStyle != null ? monthlyPrintStyle.Layout : PageLayout.OnePage;
			}
		}
		public bool OneMonthPerPage {
			get {
				MonthlyPrintStyle monthlyPrintStyle = PrintStyle as MonthlyPrintStyle;
				return monthlyPrintStyle != null ? monthlyPrintStyle.OneMonthPerPage : false;
			}
		}
		public bool PrintWeekends {
			get {
				MonthlyPrintStyle monthlyPrintStyle = PrintStyle as MonthlyPrintStyle;
				return monthlyPrintStyle != null ? monthlyPrintStyle.PrintWeekends : true;
			}
		}
		protected internal override SchedulerViewBase CreatePrintView(SchedulerControl control, GraphicsInfo gInfo, ViewPart viewPart) {
			return new MonthPrintView(control, viewPart, gInfo, PrintStyle);
		}
		public override ViewPart CalculateFirstViewPart() {
			if (Layout == PageLayout.OnePage)
				return ViewPart.Both;
			else
				return ViewPart.Left;
		}
		public override TimeInterval CalculateNextInterval(TimeInterval currentInterval) {
			DateTime currentIntervalEnd = currentInterval.End;
			return new TimeInterval(currentIntervalEnd, currentIntervalEnd.AddMonths(1));
		}
		protected internal override void SetViewParameters(SchedulerViewBase view, TimeInterval currentInterval) {
			base.SetViewParameters(view, currentInterval);
			MonthPrintView monthPrintView = (MonthPrintView)view;
			SchedulerControl control = view.Control;
			MonthView monthView = control.MonthView;
			monthPrintView.AppointmentDisplayOptions.Assign(monthView.AppointmentDisplayOptions);
			monthPrintView.ShowMoreButtons = monthView.ShowMoreButtons;
			monthPrintView.GroupSeparatorWidth = monthView.GroupSeparatorWidth;
			monthPrintView.ShowMoreButtons = true;
			MonthlyPrintStyle monthlyPrintStyle = PrintStyle as MonthlyPrintStyle;
			if (monthlyPrintStyle != null) {
				monthPrintView.SingleMonthOnly = monthlyPrintStyle.OneMonthPerPage;
				monthPrintView.Month = currentInterval.Start;
				if (monthPrintView.CompressWeekend != monthlyPrintStyle.CompressWeekend) {
					monthPrintView.CompressWeekend = monthlyPrintStyle.CompressWeekend;
					((WeekIntervalCollection)monthPrintView.InnerVisibleIntervals).CompressWeekend = monthlyPrintStyle.CompressWeekend;
					TimeIntervalCollection intervals = new TimeIntervalCollection();
					intervals.Add(currentInterval);
					monthPrintView.SetVisibleIntervals(intervals);
					monthPrintView.InnerView.UpdateVisibleIntervals(Control.Selection);
				}
				monthPrintView.ShowWeekend = monthlyPrintStyle.PrintWeekends;
			} else
				monthPrintView.SingleMonthOnly = false;
		}
		protected internal override string CalculateFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			pattern = DateTimeFormatHelper.StripDay(pattern);
			return SysDate.ToString(pattern, currentInterval.Start);
		}
		protected internal override string CalculateSecondLineText(TimeInterval currentInterval) {
			return String.Empty;
		}
		protected internal override TimeInterval AlignInterval(TimeInterval interval, DayOfWeek firstDayOfWeek) {
			DateTime start = CalculateStartDate(interval.Start.Date);
			if (OneMonthPerPage && !PrintWeekends) {
				DateTime weekStart = DateTimeHelper.GetStartOfWeekUI(start, firstDayOfWeek);
				DateTime nextWeek = weekStart.AddDays(7);
				DateTime date = start;
				while (date < nextWeek) {
					if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
						return new TimeInterval(start, start.AddMonths(1));
					date += DateTimeHelper.DaySpan;
				}
				return new TimeInterval(nextWeek, start.AddMonths(1));
			} else
				return new TimeInterval(start, start.AddMonths(1));
		}
		DateTime CalculateStartDate(DateTime startRangeDate) {
			return new DateTime(startRangeDate.Year, startRangeDate.Month, 1);
		}
	}
}
