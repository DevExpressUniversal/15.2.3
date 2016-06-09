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
	abstract class WeeklyPrintViewBuilderStrategy : SchedulerSinglePrintViewBuilderStrategy {
		public WeeklyPrintViewBuilderStrategy(GraphicsInfo gInfo, SchedulerControl control, PrintStyleWithResourceOptions printStyle)
			: base(gInfo, control, printStyle) {
		}
		public override PageLayout Layout {
			get {
				WeeklyPrintStyle weeklyPrintStyle = PrintStyle as WeeklyPrintStyle;
				return weeklyPrintStyle != null ? weeklyPrintStyle.Layout : PageLayout.OnePage;
			}
		}
		public override ViewPart CalculateFirstViewPart() {
			if (Layout == PageLayout.OnePage)
				return ViewPart.Both;
			else
				return ViewPart.Left;
		}
		protected internal override TimeInterval AlignInterval(TimeInterval interval, DayOfWeek firstDayOfWeek) {
			WeeklyPrintStyle weeklyPrintStyle = PrintStyle as WeeklyPrintStyle;
			if (firstDayOfWeek == DayOfWeek.Sunday && (weeklyPrintStyle == null || weeklyPrintStyle.ArrangeDays == ArrangeDaysKind.TopToBottom))
				firstDayOfWeek = DayOfWeek.Monday;
			DateTime start = DateTimeHelper.GetStartOfWeekUI(interval.Start.Date, firstDayOfWeek);
			return new TimeInterval(start, DateTimeHelper.WeekSpan);
		}
		protected internal override string CalculateFirstLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			pattern = DateTimeFormatHelper.StripYear(pattern);
			return String.Format("{0} -", SysDate.ToString(pattern, currentInterval.Start));
		}
		protected internal override string CalculateSecondLineText(TimeInterval currentInterval) {
			DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
			string pattern = DateTimeFormatHelper.StripDayOfWeek(dtfi.LongDatePattern);
			return SysDate.ToString(pattern, currentInterval.End.AddDays(-1));
		}
	}
}
