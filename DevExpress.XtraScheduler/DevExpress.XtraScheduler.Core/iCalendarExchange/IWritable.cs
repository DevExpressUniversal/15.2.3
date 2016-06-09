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

using DevExpress.XtraScheduler.iCalendar.Components;
using DevExpress.XtraScheduler.iCalendar.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.iCalendar.Internal {
	public interface IWritable {
		void WriteToStream(iCalendarWriter cw);
	}
	public static class WritableExtension {
		public static void WriteToString(this VAlarmCollection alarms, iCalendarWriter cw) {
			WriteToStream(alarms as IWritable, cw);
		}
		public static void WriteToStream(this iCalendarBodyItem bodyItem, iCalendarWriter cw) {
			WriteToStream(bodyItem as IWritable, cw);
		}
		public static void WriteToStream(this VAlarm alarm, iCalendarWriter cw) {
			WriteToStream(alarm as IWritable, cw);
		}
		public static void WriteToStream(this iCalendarComponentBase alarm, iCalendarWriter cw) {
			WriteToStream(alarm as IWritable, cw);
		}
		public static void WriteToStream(this iCalendarPropertyBase o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRulePropertyCollection o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this iCalendarParameterBase o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this CustomParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleFrequencyParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleIntervalParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleBySecondParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByMinuteParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByHourParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByDayParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByMonthDayParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByYearDayParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleByWeekNoParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleBySetPosParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceRuleWeekStartParameter o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this TimeZoneNamePropertyCollection o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this RecurrenceDateTimePropertyCollection o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		public static void WriteToStream(this VTimeZoneRuleCollection o, iCalendarWriter cw) {
			WriteToStream(o as IWritable, cw);
		}
		static void WriteToStream(this IWritable impl, iCalendarWriter cw) {
			if (impl == null)
				return;
			impl.WriteToStream(cw);
		}
	}
}
