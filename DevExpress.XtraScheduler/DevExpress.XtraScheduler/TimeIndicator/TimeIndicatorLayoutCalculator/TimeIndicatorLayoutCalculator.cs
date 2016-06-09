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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class IndexPair {
		public IndexPair(int first, int last) {
			First = first;
			Last = last;
		}
		public int First { get; set; }
		public int Last { get; set; }
	}
	public class TimeIndicatorLayoutCalculator {
		#region Fields
		SchedulerViewInfoBase viewInfo;
		TimeIndicatorVisibility visibility;
		#endregion
		public TimeIndicatorLayoutCalculator(SchedulerViewInfoBase viewInfo, TimeIndicatorVisibility visibility) {
			this.viewInfo = viewInfo;
			this.visibility = visibility;
		}
		#region Properties
		protected virtual SchedulerViewInfoBase ViewInfo { get { return viewInfo; } }
		protected virtual TimeIndicatorVisibility Visibility { get { return visibility; } }
		DateTime CurrentClientTime { get { return TimeZoneEngine.ConvertTime(Now, TimeZoneEngine.Local, ViewInfo.TimeZoneHelper.ClientTimeZone); } }
		#endregion
		protected internal virtual void RecalcTimeIndicatorLayout(TimeIndicatorViewInfo timeIndicator) {
			timeIndicator.DisposeItems();
			timeIndicator.Items.Clear();
			CreateTimeIndicator(timeIndicator);
			DateTime currentClientTime = CurrentClientTime;
			timeIndicator.Interval = new TimeInterval(currentClientTime, currentClientTime);
		}
		protected virtual void CreateTimeIndicator(TimeIndicatorViewInfo timeIndicator) {
			if (Visibility == TimeIndicatorVisibility.Never)
				return;
			timeIndicator.Items.AddRange(CreateTimeIndicatorCore(CurrentClientTime));
		}
		protected virtual ViewInfoItemCollection CreateTimeIndicatorCore(DateTime now) {
			return new ViewInfoItemCollection();
		}
		protected virtual DateTime Now {
			get {
#if DEBUGTEST
				return DevExpress.XtraScheduler.Tests.TestEnvironment.GetNowTime();
#else
				return DateTime.Now;
#endif
			}
		}
		protected virtual IndexPair FindNowTimeIntervalsIndx(TimeIntervalCollection intervals) {
			IndexPair result = new IndexPair(-1, -1);
			int intervalCount = intervals.Count;
			for (int i = 0; i < intervalCount; i++) {
				TimeInterval visualInterval = intervals[i];
				if (visualInterval.Start <= Now && Now <= visualInterval.End) {
					if (result.First < 0)
						result.First = i;
					result.Last = i;
				} else if (result.Last > -1)
					break;
			}
			if (result.First < 0)
				return null;
			return result;
		}
		protected virtual TimeSpan CalcTimeOfDay(DateTime date) {
			TimeSpan result = date.TimeOfDay;
			if (result < ViewInfo.View.VisibleStart.TimeOfDay)
				result += DateTimeHelper.DaySpan;
			return result;
		}
		protected virtual bool CanShowOnAllIntervals(bool intervalsContainsNowTime, TimeIndicatorVisibility visibility) {
			if (visibility == TimeIndicatorVisibility.Always)
				return true;
			if (intervalsContainsNowTime && visibility == TimeIndicatorVisibility.TodayView)
				return true;
			return false;
		}
	}
}
