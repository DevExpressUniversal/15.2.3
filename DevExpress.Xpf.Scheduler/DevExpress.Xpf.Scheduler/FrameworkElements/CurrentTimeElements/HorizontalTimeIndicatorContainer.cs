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

using DevExpress.Office.Internal;
using DevExpress.Xpf.Scheduler.Drawing.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class HorizontalTimeIndicatorContainer : ContentControl {
		protected class IndexPair {
			public IndexPair(int first, int last) {
				First = first;
				Last = last;
			}
			public int First { get; set; }
			public int Last { get; set; }
		}
		public static readonly DependencyProperty IntervalsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalTimeIndicatorContainer, VisualSimpleResourceIntervalCollection>("Intervals", null, (o, e) => o.OnIntervalsChanged(e.OldValue, e.NewValue));
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalTimeIndicatorContainer, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (o, e) => o.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		public static readonly DependencyProperty ViewProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HorizontalTimeIndicatorContainer, SchedulerViewBase>("View", null, FrameworkPropertyMetadataOptions.AffectsArrange);
		static HorizontalTimeIndicatorContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalTimeIndicatorContainer), new FrameworkPropertyMetadata(typeof(HorizontalTimeIndicatorContainer)));
		}
		public HorizontalTimeIndicatorContainer() {
			Now = TimeIndicatorTimerService.Instance.Now;
			SubscribedIntervalItems = new List<VisualSimpleResourceInterval>();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public VisualSimpleResourceIntervalCollection Intervals {
			get { return (VisualSimpleResourceIntervalCollection)GetValue(IntervalsProperty); }
			set { SetValue(IntervalsProperty, value); }
		}
		#region View
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { this.SetValue(ViewProperty, value); }
		}
		#endregion
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		protected DateTime Now { get; set; }
		List<VisualSimpleResourceInterval> SubscribedIntervalItems { get; set; }
		protected override Size ArrangeOverride(Size arrangeSize) {
			UIElement child = GetPresenterChild();
			if (child != null)
				ArrangeChild(child, arrangeSize);
			return arrangeSize;
		}
		UIElement GetPresenterChild() {
			int childrenCount = VisualTreeHelper.GetChildrenCount(this);
			if (childrenCount <= 0)
				return null;
			return VisualTreeHelper.GetChild(this, 0) as UIElement;
		}
		void ArrangeChild(UIElement child, Size arrangeSize) {
			double pos = CalcIndicatorPosition(arrangeSize);
			double position = pos - child.DesiredSize.Height / 2;
			Point location = new Point(0, position);
			Size size = new Size(arrangeSize.Width, child.DesiredSize.Height);
			bool currentTimeInVisibleArea = false;
			if (Intervals != null && TimeIndicatorVisibility != TimeIndicatorVisibility.Never) {
				IndexPair intervalsIndx = FindNowTimeIntervalsIndx(Intervals);
				if (CanShowOnAllIntervals(intervalsIndx != null, TimeIndicatorVisibility))
					intervalsIndx = new IndexPair(0, Intervals.Count - 1);
				if (intervalsIndx != null) {
					currentTimeInVisibleArea = true;
					int intervalCount = Intervals.Count;
					Rect[] intervalBounds = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(arrangeSize, intervalCount, new Thickness(), Orientation.Horizontal);
					Rect intervalsStartBounds = intervalBounds[intervalsIndx.First];
					Rect intervalsEndBounds = intervalBounds[intervalsIndx.Last];
					location.X = intervalsStartBounds.X;
					size.Width = intervalsEndBounds.Right - intervalsStartBounds.Left;
				}
			}
			child.Visibility = (currentTimeInVisibleArea && position >= 0) ? Visibility.Visible : Visibility.Collapsed;
			Rect bounds = new Rect(location, size);
			child.Arrange(bounds);
		}
		protected bool CanShowOnAllIntervals(bool intervalsContainsNowTime, TimeIndicatorVisibility timeIndicatorVisibility) {
			if (timeIndicatorVisibility == TimeIndicatorVisibility.Always)
				return true;
			if (intervalsContainsNowTime && timeIndicatorVisibility == TimeIndicatorVisibility.TodayView)
				return true;
			return false;
		}
		protected IndexPair FindNowTimeIntervalsIndx(VisualSimpleResourceIntervalCollection intervals) {
			IndexPair result = new IndexPair(-1, -1);
			int intervalCount = intervals.Count;
			for (int i = 0; i < intervalCount; i++) {
				VisualSimpleResourceInterval visualInterval = intervals[i];
				if (visualInterval.IntervalCells.Start <= Now && Now <= visualInterval.IntervalCells.End) {
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
		bool IsVisibleIntervalContainsCurrentTime(bool intervalsContainNowTime) {
			if (View == null)
				return intervalsContainNowTime;
			return View.GetVisibleIntervals().Interval.Contains(Now);
		}
		void OnIntervalsChanged(VisualSimpleResourceIntervalCollection oldValue, VisualSimpleResourceIntervalCollection newValue) {
			if (oldValue != null)
				UnsubscribeIntervalsCollection(oldValue);
			if (newValue != null)
				SubscribeIntervalsCollection(newValue);
			InvalidateArrange();
		}
		void SubscribeIntervalsCollection(VisualSimpleResourceIntervalCollection intervalCollection) {
			intervalCollection.CollectionChanged += OnIntervalsCollectionChanged;
			SubscribeIntervalCollectionItems(intervalCollection);
		}
		void UnsubscribeIntervalsCollection(VisualSimpleResourceIntervalCollection intervalCollection) {
			intervalCollection.CollectionChanged -= OnIntervalsCollectionChanged;
			UnsubscribeIntervalCollectionItems(intervalCollection);
		}
		void OnIntervalsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UnsubscribeIntervalCollectionItems(SubscribedIntervalItems);
			SubscribeIntervalCollectionItems(Intervals);
			InvalidateArrange();
		}
		void SubscribeIntervalCollectionItems(IEnumerable<VisualSimpleResourceInterval> intervalCollection) {
			foreach (VisualSimpleResourceInterval visualInterval in intervalCollection) {
				DependencyPropertyChangeHandler.AddHandler(visualInterval, "IntervalCells", OnIntervalStartChanged);
				SubscribedIntervalItems.Add(visualInterval);
			}
		}
		void UnsubscribeIntervalCollectionItems(IEnumerable<VisualSimpleResourceInterval> intervalCollection) {
			foreach (var visualInterval in intervalCollection)
				DependencyPropertyChangeHandler.RemoveHandler(visualInterval, "IntervalCells");
			foreach (var visualInterval in SubscribedIntervalItems)
				DependencyPropertyChangeHandler.RemoveHandler(visualInterval, "IntervalCells");
			SubscribedIntervalItems.Clear();
		}
		void OnIntervalStartChanged() {
			InvalidateArrange();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			TimeIndicatorTimerService.Instance.TimerTick -= OnTimerTick;
			TimeIndicatorTimerService.Instance.TimerTick += OnTimerTick;
			if (Intervals != null) {
				UnsubscribeIntervalsCollection(Intervals);
				SubscribeIntervalsCollection(Intervals);
			}
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			TimeIndicatorTimerService.Instance.TimerTick -= OnTimerTick;
			if (Intervals != null)
				UnsubscribeIntervalsCollection(Intervals);
			else {
				if (Intervals != null)
					UnsubscribeIntervalsCollection(Intervals);
			}
		}
		void OnTimerTick(object sender, TimeIndicatorTickEventArgs e) {
			Now = e.Now;
			InvalidateArrange();
		}
		double CalcIndicatorPosition(Size arrangeBounds) {
			DateTime clientNowTime = GetClientNowTime();
			TimeSpan clientNowTimeOfDay = clientNowTime.TimeOfDay;
			if (Intervals == null || Intervals.Count < 1)
				return 0;
			VisualSimpleResourceInterval interval = Intervals[0];
			DateTime intervalStart = interval.IntervalCells.Start;
			DateTime intervalEnd = interval.IntervalCells.End;
			TimeOfDayInterval timeOfDayInterval = new TimeOfDayInterval(intervalStart.TimeOfDay, intervalStart.TimeOfDay + (intervalEnd - intervalStart));
			return SchedulerRectUtils.CalcDateY(clientNowTimeOfDay, timeOfDayInterval, arrangeBounds);
		}
		DateTime GetClientNowTime() {
			DateTime nowTime = Now;
			TimeZoneHelper timeZoneEngine = GetTimeZoneHelper();
			if (timeZoneEngine == null)
				return nowTime;
			return TimeZoneInfo.ConvertTime(nowTime, TimeZoneInfo.Local, timeZoneEngine.ClientTimeZone);
		}
		TimeZoneHelper GetTimeZoneHelper() {
			if (View == null || View.Control == null)
				return null;
			return View.Control.TimeZoneHelper;
		}
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility timeIndicatorVisibility1, TimeIndicatorVisibility timeIndicatorVisibility2) {
			InvalidateArrange();
		}
	}
}
