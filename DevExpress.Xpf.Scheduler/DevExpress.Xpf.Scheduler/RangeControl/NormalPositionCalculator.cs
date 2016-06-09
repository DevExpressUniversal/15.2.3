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
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	public class NormalPositionCalculator {
		const int AdditionalIntervalCount = 5;
		readonly ScaleBasedRangeControlControllerBase controller;
		public NormalPositionCalculator(ScaleBasedRangeControlControllerBase controller) {
			this.controller = controller;
		}
		public ScaleBasedRangeControlControllerBase Controller { get { return controller; } }
		public RulerGroupInfo Calculate(DateTime visibleStart, DateTime visibleEnd) {
			RulerGroupInfo result = new RulerGroupInfo();
			if (Controller.RulerCount <= 0)
				return result;
			int count = Controller.RulerCount;
			List<ISchedulerRangeControlRuler> actualRulers = new List<ISchedulerRangeControlRuler>();
			for (int i = 0; i < count; i++) {
				ISchedulerRangeControlRuler ruler = Controller.Rulers[i];
				TimeScale scale = ruler.Scale;
				if (!scale.Enabled)
					continue;
				actualRulers.Add(ruler);
			}
			int actualRulerCount = actualRulers.Count;
			for (int i = 0; i < actualRulerCount; i++) {
				ISchedulerRangeControlRuler ruler = actualRulers[i];
				TimeScale scale = ruler.Scale;
				bool isBaseScale = (i == actualRulerCount - 1);
				if (!isBaseScale && !scale.Visible)
					continue;
				RulerInfo info = CalculateRulerInfo(visibleStart, visibleEnd, ruler);
				if (isBaseScale)
					result.BaseRuler = info;
				if (scale.Visible)
					result.Rulers.Add(info);
			}
			return result;
		}
		RulerInfo CalculateRulerInfo(DateTime visibleStart, DateTime visibleEnd, ISchedulerRangeControlRuler ruler) {
			RulerInfo info = new RulerInfo();
			info.Ruler = ruler;
			info.Items = CalculateDoubleIntervals(ruler.Scale, visibleStart, visibleEnd);
			return info;
		}
		List<RulerItemInfo> CalculateDoubleIntervals(TimeScale scale, DateTime visibleStart, DateTime visibleEnd) {
			List<RulerItemInfo> result = new List<RulerItemInfo>();
			TimeInterval actualVisibleInterval = CalculateActualVisibleTime(scale, visibleStart, visibleEnd);
			DateTime current = scale.Floor(actualVisibleInterval.Start);
			while (current < actualVisibleInterval.End) {
				DateTime next = scale.GetNextDate(current);
				double leftNormalValue = Controller.GetNormalizedValue(current);
				double rightNormalValue = Controller.GetNormalizedValue(next);
				RulerItemInfo info = new RulerItemInfo();
				info.Interval = new TimeInterval(current, next);
				info.NormalInterval = new DoubleInterval(leftNormalValue, rightNormalValue);
				info.Scale = scale;
				result.Add(info);
				current = next;
			}
			return result;
		}
		TimeInterval CalculateActualVisibleTime(TimeScale scale, DateTime visibleStart, DateTime visibleEnd) {
			double startOffset = Controller.GetComparableValue(visibleStart) - AdditionalIntervalCount * GetComparableStep(scale, visibleStart);
			double endOffset = Controller.GetComparableValue(visibleEnd) + AdditionalIntervalCount * GetComparableStep(scale, visibleEnd);
			DateTime actualStart = (DateTime)Controller.GetRealValue(Math.Max(startOffset, Controller.MinimumComparable));
			DateTime actualEnd = (DateTime)Controller.GetRealValue(Math.Min(endOffset, Controller.MaximumComparable));
			return new TimeInterval(actualStart, actualEnd);
		}
		DoubleInterval CalculateSnappedDoubleInterval(TimeScale scale, TimeInterval interval) {
			double comparableRenderStart = Controller.GetComparableValue(interval.Start);
			DateTime realRegionStart = scale.Floor(Controller.GetRealValue(comparableRenderStart));
			DateTime snappedStart = realRegionStart;
			double comparableSnappedStart = Controller.GetComparableValue(snappedStart);
			double comparableRenderEnd = Controller.GetComparableValue(interval.End);
			DateTime realRegionEnd = scale.Floor(Controller.GetRealValue(comparableRenderEnd));
			DateTime snappedEnd = realRegionEnd;
			snappedEnd = scale.GetNextDate(snappedEnd);
			double comparableSnappedEnd = Controller.GetComparableValue(snappedEnd);
			return new DoubleInterval(comparableSnappedStart, comparableSnappedEnd);
		}
		double GetComparableStep(TimeScale scale, DateTime value) {
			DateTime start = scale.Floor(value);
			DateTime next = scale.GetNextDate(start);
			return Controller.GetComparableValue(next) - Controller.GetComparableValue(start);
		}
	}
	public class RulerInfo {
		public ISchedulerRangeControlRuler Ruler { get; set; }
		public List<RulerItemInfo> Items { get; set; }
		public TimeIntervalCollection GetIntervals() {
			TimeIntervalCollection result = new TimeIntervalCollection();
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				RulerItemInfo item = Items[i];
				result.Add(item.Interval);
			}
			return result;
		}
	}
	public class RulerItemInfo {
		public DoubleInterval NormalInterval { get; set; }
		public TimeInterval Interval { get; set; }
		public TimeScale Scale { get; set; }
	}
	public class RulerGroupInfo {
		public RulerGroupInfo() {
			Rulers = new List<RulerInfo>();
		}
		public List<RulerInfo> Rulers { get; set; }
		public RulerInfo BaseRuler { get; set; }
	}
	public class DoubleInterval {
		public DoubleInterval() {
		}
		public DoubleInterval(double start, double end) {
			Start = start;
			End = end;
		}
		public double Start { get; set; }
		public double End { get; set; }
	}
}
