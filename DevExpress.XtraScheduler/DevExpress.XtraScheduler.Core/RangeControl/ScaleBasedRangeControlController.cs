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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Native {
	public interface ISchedulerRangeControlRuler {
		TimeScale Scale { get; }
	}
	public class SchedulerRangeControlRuler : ISchedulerRangeControlRuler {
		public SchedulerRangeControlRuler(TimeScale scale) {
			Guard.ArgumentNotNull(scale, "scale");
			Scale = scale;
		}
		public TimeScale Scale { get; private set; }
	}
	public abstract class ScaleBasedRangeControlControllerBase {
		DateTime minimum;
		DateTime maximum;
		double minimumComparable;
		double maximumComparable;
		protected ScaleBasedRangeControlControllerBase() {
			Rulers = new List<ISchedulerRangeControlRuler>();
			ActualScales = new TimeScaleCollection();
		}
		#region Properties
		public List<ISchedulerRangeControlRuler> Rulers { get; private set; }
		public int RulerCount { get { return Rulers.Count; } }
		public IRangeControlClientDataProvider DataProvider { get; protected set; }
		public double MinimumComparable { get { return minimumComparable; } }
		public DateTime Minimum {
			get { return minimum; }
			private set {
				if (minimum == value)
					return;
				minimum = value;
				this.minimumComparable = GetComparableValue(minimum);
			}
		}
		public double MaximumComparable { get { return maximumComparable; } }
		public DateTime Maximum {
			get { return maximum; } 
			private set {
				if (maximum == value)
					return;
				maximum = value;
				this.maximumComparable = GetComparableValue(maximum);
			}
		}
		public int MaxSelectedIntervalCount { get; private set; }
		protected internal TimeScale BaseScale { get { return GetBaseScale(); } }
		protected internal TimeScaleCollection ActualScales { get; private set; }
		#endregion
		protected ISchedulerRangeControlRuler GetBaseRuler() {
			int count = RulerCount;
			if (count <= 0)
				return null;
			return Rulers[count - 1];
		}
		protected TimeScale GetBaseScale() {
			ISchedulerRangeControlRuler ruler = GetBaseRuler();
			if (ruler == null)
				return null;
			return ruler.Scale;
		}
		protected internal void UpdateMaxSelectedIntervalCount(int count) {
			MaxSelectedIntervalCount = count;
		}
		protected internal void UpdateTotalRange(DateTime minimum, DateTime maximum) {
			if (!IsMinMaxTotalRangeValid(minimum, maximum)) {
				Minimum = minimum;
				Maximum = minimum.AddMonths(1);
				return;
			}
			Minimum = minimum;
			Maximum = maximum;
		}
		protected virtual ISchedulerRangeControlRuler CreateRuler(TimeScale timeScale) {
			return new SchedulerRangeControlRuler(timeScale);
		}
		protected bool IsMinMaxTotalRangeValid(DateTime minimum, DateTime maximum) {
			return minimum < maximum;
		}
		protected internal void UpdateVisibleRulers(TimeScaleCollection scales) {
			UpdateActualScales(scales);
			XtraSchedulerDebug.Assert(ActualScales.Count > 0);
			Rulers.Clear();
			for (int i = 0; i < ActualScales.Count; i++) {
				Rulers.Add(CreateRuler(ActualScales[i]));
			}
		}
		protected void UpdateActualScales(TimeScaleCollection scales) {
			ActualScales.BeginUpdate();
			try {
				ActualScales.Clear();
				ActualScales.AddRange(TimeScaleCollectionHelper.SelectVisibleScales(scales));
			} finally {
				ActualScales.EndUpdate();
			}
		}
		public DateTime GetFirstVisibleDate(double normalizedValue) {
			return BaseScale.Floor(GetValue(normalizedValue));
		}
		public DateTime GetLastVisibleDate(double normalizedValue) {
			return BaseScale.GetNextDate(GetValue(normalizedValue));
		}
		internal TimeInterval RoundToWholeInterval(int rulerIndex, DateTime start) {
			ISchedulerRangeControlRuler ruler = Rulers[rulerIndex];
			TimeScale scale = ruler.Scale;
			DateTime roundedStart = scale.Floor(start);
			DateTime roundedEnd = scale.GetNextDate(roundedStart);
			return new TimeInterval(roundedStart, roundedEnd);
		}
		protected internal virtual double CalculateScaleFactor(double itemCount) {
			if (RulerCount == 0)
				return 1.0f;
			ISchedulerRangeControlRuler ruler = Rulers[RulerCount - 1];
			double comparableRangeInterval = GetComparableRangeInterval();
			double comparableScaleDuration = CalculateScaleComparableDuration(ruler);
			return comparableScaleDuration * itemCount / comparableRangeInterval;
		}
		public double CalculateScaleFactorViaIntervals(DateTime start, DateTime end) {
			if (RulerCount == 0)
				return 1.0f;
			double comparableRangeInterval = GetComparableValue(Maximum) - GetComparableValue(Minimum);
			return (GetComparableValue(end) - GetComparableValue(start)) / comparableRangeInterval;
		}
		protected virtual double GetComparableRangeInterval() {
			return GetComparableValue(Maximum) - GetComparableValue(Minimum);
		}
		public double CalculateScaleComparableDuration(ISchedulerRangeControlRuler ruler) {
			DateTime start = ruler.Scale.Floor(Minimum);
			return GetComparableValue(ruler.Scale.GetNextDate(start)) - GetComparableValue(start);
		}
		public DateTime GetNextVisibleDate(int rulerIndex, DateTime dateTime) {
			ISchedulerRangeControlRuler ruler = Rulers[rulerIndex];
			TimeScale scale = ruler.Scale;
			return scale.GetNextDate(scale.Floor(dateTime));
		}
		public string FormatRulerFixedFormatCaption(int rulerIndex, DateTime start, DateTime end) {
			ISchedulerRangeControlRuler ruler = Rulers[rulerIndex];
			TimeScale scale = ruler.Scale;
			DateTime roundedStart = scale.Floor(start);
			DateTime roundedEnd = scale.GetNextDate(roundedStart);
			return scale.FormatCaption(roundedStart, roundedEnd);
		}
		public virtual List<DataItemThumbnailList> CreateThumbnailData(TimeIntervalCollection intervals) {
			return DataProvider.CreateThumbnailData(intervals);
		}
		protected virtual DateTime CalculateMaximumSelectedEnd(DateTime start, int maxSelectedIntervals) {
			TimeScaleFixedInterval fixedInterval = BaseScale as TimeScaleFixedInterval;
			if (fixedInterval != null) {
				long ticks = fixedInterval.Value.Ticks * maxSelectedIntervals;
				TimeSpan duration = TimeSpan.FromTicks(ticks);
				return start + duration;
			}
			DateTime date = start;
			for (int i = 0; i < maxSelectedIntervals; i++) {
				if (BaseScale.HasNextDate(date))
					date = BaseScale.GetNextDate(date);
			}
			return date;
		}
		protected internal virtual void UpdateDataProvider(IRangeControlClientDataProvider dataProvider) {
			DataProvider = dataProvider;
		}
		public virtual void SetHitInterval(TimeInterval interval) {
			XtraSchedulerDebug.Assert(interval != null);
			DataProvider.OnSelectedRangeChanged(interval.Start, interval.End);
		}
		public virtual void OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			if (DataProvider == null)
				return;
			DateTime minimum = CorrectPrecisionError(Convert.ToDateTime(rangeMinimum));
			DateTime maximum = CorrectPrecisionError(Convert.ToDateTime(rangeMaximum));
			DataProvider.OnSelectedRangeChanged(minimum, maximum);
		}
		protected DateTime CorrectPrecisionError(DateTime dt) {
			DateTime roundedDate = dt.Date.AddDays(1);
			long delta = roundedDate.Ticks - dt.Ticks;
			if (delta > 0 && delta < 2)
				return roundedDate;
			return dt;
		}
		public virtual int CalculateScaleIntervalCount(TimeScale scale, DateTime start, DateTime end) {
			TimeScaleFixedInterval fixedInterval = scale as TimeScaleFixedInterval;
			if (fixedInterval != null) {
				long ticks = fixedInterval.Value.Ticks;
				TimeSpan duration = TimeSpan.FromTicks(ticks);
				return DateTimeHelper.Divide(end - start, duration);
			}
			DateTime date = start;
			int n = 0;
			for (; ; ) {
				if (scale.HasNextDate(date))
					date = scale.GetNextDate(date);
				if (date > end)
					break;
				n++;
			}
			return n;
		}
		public double CalculateDesiredScaleFactor() {
			double count = CalcDesiredIntervalCount();
			if (count == 0)
				return 0;
			return CalculateScaleFactor(count + 1);
		}
		public double CalcDesiredIntervalCount() {
			XtraSchedulerDebug.Assert(RulerCount > 0);
			return DataProvider.SyncSupport.ViewportWidth / Math.Max(1, BaseScale.Width);
		}
		public virtual double CalculateMinScaleFactor() {
			double maxCount = CalcMaximumIntervalCount();
			return CalculateScaleFactorCore(maxCount);
		}
		public virtual double CalculateMaxScaleFactor() {
			double minCount = CalcMinimumIntervalCount();
			return CalculateScaleFactorCore(minCount);
		}
		double CalcMinimumIntervalCount() {
			if (DataProvider == null)
				return 1;
			IScaleBasedRangeControlClientOptions options = DataProvider.GetOptions();
			int width = Math.Max(options.MaxIntervalWidth, options.MinIntervalWidth);
			return CalcExpectedIntervalCount(width);
		}
		double CalcMaximumIntervalCount() {
			if (DataProvider == null)
				return 1;
			IScaleBasedRangeControlClientOptions options = DataProvider.GetOptions();
			return CalcExpectedIntervalCount(options.MinIntervalWidth);
		}
		double CalcExpectedIntervalCount(double expectedWidth) {
			return DataProvider.SyncSupport.ViewportWidth / Math.Max(1, expectedWidth);
		}
		protected virtual double CalculateScaleFactorCore(double expectedCount) {
			return expectedCount > 0 ? 1 / CalculateScaleFactor(expectedCount) : 1.0f;
		}
		public abstract DateTime GetValue(double normalizedValue);
		public abstract double GetNormalizedValue(object value);
		public abstract double GetComparableValue(DateTime value);
		public abstract DateTime GetRealValue(double comparableValue);
	}
}
