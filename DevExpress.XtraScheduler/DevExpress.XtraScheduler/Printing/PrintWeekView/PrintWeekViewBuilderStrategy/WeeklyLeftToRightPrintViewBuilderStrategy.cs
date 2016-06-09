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
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class WeeklyLeftToRightPrintViewBuilderStrategy : WeeklyPrintViewBuilderStrategy {
		public WeeklyLeftToRightPrintViewBuilderStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle)
			: base(gInfo, control, printStyle) {
		}
		protected internal override SchedulerViewBase CreatePrintView(SchedulerControl control, GraphicsInfo gInfo, ViewPart viewPart) {
			return new DayPrintView(control, gInfo, PrintStyle);
		}
		protected internal override TimeIntervalCollection CalculateVisibleIntervals(TimeInterval currentInterval, ViewPart viewPart) {
			TimeIntervalCollection interval = new TimeIntervalCollection();
			int dayCount = CalculateDayCount(viewPart);
			TimeSpan delta = viewPart == ViewPart.Right ? TimeSpan.FromDays(-1) : TimeSpan.FromDays(1);
			DateTime date = viewPart == ViewPart.Right ? currentInterval.Start.AddDays(6) : currentInterval.Start;
			while (interval.Count < dayCount) {
				if (ShouldPrintDate(date)) {
					TimeInterval newInterval = new TimeInterval(date, DateTimeHelper.DaySpan);
					interval.Add(newInterval);
				}
				date += delta;
			}
			return interval;
		}
		protected internal virtual bool ShouldPrintDate(DateTime date) {
			if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
				return true;
			WeeklyPrintStyle printStyle = PrintStyle as WeeklyPrintStyle;
			if (printStyle != null && !printStyle.PrintWeekends)
				return false;
			else
				return true;
		}
		protected internal virtual int CalculateDayCount(ViewPart part) {
			WeeklyPrintStyle weeklyPrintStyle = PrintStyle as WeeklyPrintStyle;
			if (weeklyPrintStyle == null)
				return 7;
			switch (part) {
				case ViewPart.Both:
					return weeklyPrintStyle.PrintWeekends ? 7 : 5;
				case ViewPart.Left:
					return weeklyPrintStyle.PrintWeekends ? 3 : 2;
				case ViewPart.Right:
					return weeklyPrintStyle.PrintWeekends ? 4 : 3;
				default:
					XtraSchedulerDebug.Assert(false);
					return 0;
			}
		}
		protected internal override void SetViewParameters(SchedulerViewBase view, TimeInterval currentInterval) {
			base.SetViewParameters(view, currentInterval);
			DayPrintView dayPrintView = (DayPrintView)view;
			WeeklyPrintStyle weeklyPrintStyle = PrintStyle as WeeklyPrintStyle;
			if (weeklyPrintStyle != null)
				dayPrintView.VisibleTime = weeklyPrintStyle.PrintTime;
			SchedulerControl control = view.Control;
			DayView dayView = control.DayView;
			DailyPrintViewBuilderStrategy.SetViewParametersCore(dayPrintView, dayView);
		}
		protected internal override void CalculateViewInfo(Rectangle pageBounds, SchedulerViewBase view) {
			DayPrintView dayView = (DayPrintView)view;
			DailyPrintViewBuilderStrategy.SetScaleForDayView(PrintStyle, pageBounds, dayView);
		}
	}
}
