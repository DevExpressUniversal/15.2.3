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
namespace DevExpress.XtraScheduler {
	public interface IRangeControlClientSyncSupport {
		TimeInterval TotalRange { get; }
		double ViewportWidth { get; }
		IScaleBasedRangeControlClientOptions Options { get; }
		void ReInitialize();
		void RefreshRangeControl(bool forceRequestData);
		void SyncRangeControlRange();
		bool CanAdjustRangeControl { get; }
		void AdjustRangeControlRange(RangeControlAdjustEventArgs e);
		void AdjustRangeControlScales(RangeControlAdjustEventArgs e);
	}
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISchedulerControlRangeHelper {
		TimeInterval CalculateAdjustedRange(InnerSchedulerControl innerControl);
		TimeScaleCollection CalculateAdjustedTimeScales(InnerSchedulerControl innerControl);		
	}
	public class SchedulerControlRangeHelper : ISchedulerControlRangeHelper {
		public virtual TimeInterval CalculateAdjustedRange(InnerSchedulerControl innerControl) {
			if (innerControl == null)
				return TimeInterval.Empty;
			TimeInterval visibleInterval = innerControl.ActiveView.GetVisibleIntervals().Interval;
			DateTime start = visibleInterval.Start;
			DateTime startOfMonth = new DateTime(start.Year, start.Month, 1);
			SchedulerViewType activeViewType = innerControl.ActiveViewType;
			switch (activeViewType) {
				case SchedulerViewType.Day:
				case SchedulerViewType.WorkWeek:
				case SchedulerViewType.FullWeek:
					return new TimeInterval(start.AddMonths(-1), start.AddMonths(1));
				case SchedulerViewType.Week:
					return new TimeInterval(startOfMonth.AddMonths(-3), startOfMonth.AddMonths(3));
				case SchedulerViewType.Month:
					return new TimeInterval(startOfMonth.AddMonths(-6), startOfMonth.AddMonths(6));
				case SchedulerViewType.Timeline:
				case SchedulerViewType.Gantt:
					TimeSpan duration = new TimeSpan(visibleInterval.Duration.Ticks * 2);
					return new TimeInterval(visibleInterval.Start - duration, visibleInterval.End + duration);
			}
			return TimeInterval.Empty;
		}
		public virtual TimeScaleCollection CalculateAdjustedTimeScales(InnerSchedulerControl innerControl) {
			TimeScaleCollection result = new TimeScaleCollection();
			if (innerControl == null)
				return result;
			SchedulerViewType activeViewType = innerControl.ActiveViewType;
			switch (activeViewType) {
				case SchedulerViewType.Week:
				case SchedulerViewType.Month:
					result.Add(new TimeScaleWeek());
					result.Add(new TimeScaleMonth());
					break;
				case SchedulerViewType.Timeline:
				case SchedulerViewType.Gantt:
					result.AddRange(innerControl.TimelineView.ActualScales);
					break;
				default:
					result.LoadDefaults();
					break;
			}
			UpdateScalesFirstDayOfWeek(result, innerControl.FirstDayOfWeek);
			return result;
		}
		public static void UpdateScalesFirstDayOfWeek(InnerSchedulerControl innerControl, TimeScaleCollection scales) {
			DayOfWeek firstDayOfWeek = innerControl.FirstDayOfWeek;
			int count = scales.Count;
			for (int i = 0; i < count; i++)
				scales[i].SetFirstDayOfWeek(firstDayOfWeek);
		}
		protected void UpdateScalesFirstDayOfWeek(TimeScaleCollection scales, DayOfWeek firstDayOfWeek) {
			int count = scales.Count;
			for (int i = 0; i < count; i++)
				scales[i].SetFirstDayOfWeek(firstDayOfWeek);
		}
	}
}
