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
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Scheduler.Native {
	public class DateNavigationUtils {
		public static DayIntervalCollection ConvertToDays(TimeIntervalCollection intervals) {
			DayIntervalCollection result = new DayIntervalCollection();
			result.AddRange(intervals);
			return result;
		}
		public static IList<DateTime> ConvertToDateList(TimeIntervalCollection intervals) {
			DayIntervalCollection days = ConvertToDays(intervals);
			return ConvertToDateList(days);
		}
		public static IList<DateTime> ConvertToDateList(DayIntervalCollection days) {
			IList<DateTime> result = new List<DateTime>();
			int count = days.Count;
			for (int i = 0; i < count; i++)
				result.Add(days[i].Start);
			return result;
		}
		public static DayIntervalCollection ConvertToDays(IList<DateTime> dates) {
			DayIntervalCollection result = new DayIntervalCollection();
			int count = dates.Count;
			for (int i = 0; i < count; i++) {
				DateTime current = dates[i];
				result.Add(new TimeInterval(current, current.AddDays(1)));
			}
			return result;
		}
		public static List<DateTime> CreateDateList(DateTime start, DateTime end) {
			TimeSpan span = end - start;
			int count = span.Days;
			List<DateTime> result = new List<DateTime>();
			for (int i = 0; i < count; i++) {
				result.Add(start.AddDays(i));
			}
			return result;
		}
		public static List<DateTime> CreateDateList(IList<DateTime> source, int firstItemCount) {
			int count = Math.Min(source.Count, firstItemCount);
			List<DateTime> result = new List<DateTime>();
			for (int i = 0; i < count; i++) 
				result.Add(source[i]);
			return result;
		}
		public static ObservableCollection<DateTime> CreateDateCollection(Dictionary<DateTime, bool> dates) {
			ObservableCollection<DateTime> result = new ObservableCollection<DateTime>();
			foreach (KeyValuePair<DateTime, bool> item in dates)
				result.Add(item.Key);
			return result;
		}
		public static ObservableCollection<DateTime> ConvertFromIntervalCollection(XtraScheduler.DayIntervalCollection days) {
			ObservableCollection<DateTime> selection = new ObservableCollection<DateTime>();
			int count = days.Count;
			for (int i = 0; i < count; i++) {
				selection.Add(days[i].Start);
			}
			return selection;
		}
		public static DayIntervalCollection ConvertToIntervalCollection(IList<DateTime> dates) {
			XtraScheduler.DayIntervalCollection result = new XtraScheduler.DayIntervalCollection();
			int count = dates.Count;
			for (int i = 0; i < count; i++)
				result.Add(new TimeInterval(dates[i], TimeSpan.Zero));
			return result;
		}
		public static bool IsSolidDateRange(IList<DateTime> dates) {
			int count = dates.Count;
			if (count <= 6)
				return false;
			DateTime prevDate = dates[0];
			for (int i = 1; i < count; i++) {
				DateTime current = dates[i];
				if (current - prevDate != TimeSpan.FromDays(1))
					return false;
				prevDate = current;
			}
			return true;
		}
		public static DateTime GetFirstDate(IList<DateTime> dates) {
			int count = dates.Count;
			return count > 0 ? dates[0] : DateTime.MinValue;
		}
		public static DateTime GetLastDate(IList<DateTime> dates) {
			int count = dates.Count;
			return count > 0 ? dates[count - 1] : DateTime.MinValue;
		}
	}
}
