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
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Native {
	public class TimelineFiniteDateTimeScrollController : ViewDateTimeScrollController {
		public TimelineFiniteDateTimeScrollController(TimelineView view)
			: base(view) {
		}
		TimeInterval VisibleInterval { get { return View.InnerVisibleIntervals.Interval; } }
		TimeScale BaseScale { get { return TimelineView.GetBaseTimeScale(); } }
		TimeInterval LimitInterval { get { return View.LimitInterval; } }
		TimelineView TimelineView { get { return (TimelineView)View; } }
		protected internal override bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return true;
		}
		protected internal override bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar) {
			bool visibilityChanged = scrollBar.Visible != View.DateTimeScrollbarVisible;
			scrollBar.Visible = View.DateTimeScrollbarVisible;
			return visibilityChanged;
		}
		protected internal override void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter) {
			scrollBarAdapter.BeginUpdate();
			try {
				scrollBarAdapter.Minimum = 0;
				scrollBarAdapter.Maximum = CalculateIntervalCount(LimitInterval);
				scrollBarAdapter.SmallChange = 1;
				scrollBarAdapter.LargeChange = CalculateIntervalCount(VisibleInterval) - 1;
				scrollBarAdapter.Value = CalculateScrollValue(scrollBarAdapter.Maximum, scrollBarAdapter.LargeChange);
			} finally {
				scrollBarAdapter.EndUpdate();
			}
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
		long CalculateIntervalCount(TimeInterval interval) {
			DateTime date = interval.Start;
			long count = 0;
			while (BaseScale.HasNextDate(date)) {
				if (date > interval.End)
					break;
				date = BaseScale.GetNextDate(date);
				count++;
			}
			return Math.Max(1, count - 1);
		}
		long CalculateScrollValue(long scrollMaximum, long largeChange) {
			return CalculateIntervalCount(new TimeInterval(LimitInterval.Start, View.VisibleStart));
		}
		protected internal override void OnScroll(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
			ScrollEventType eventType = e.Type;
			long delta = ((int)e.NewValue) - scrollBarAdapter.Value;
			if (delta == 0)
				delta = CalculateScrollDelta(eventType, e.NewValue, scrollBarAdapter);
			if (delta != 0) {
				int newValue = MakeScrollToDelta(delta, eventType, scrollBarAdapter);
				if (newValue != 0)
					e.NewValue = newValue;
			}
		}
		protected internal int MakeScrollToDelta(long scrollDelta, ScrollEventType eventType, IScrollBarAdapter scrollBarAdapter) {
			Scroll(scrollBarAdapter, scrollDelta, true);
			return 0;
		}
		long CalculateScrollDelta(ScrollEventType eventType, double newValue, IScrollBarAdapter scrollBarAdapter) {
			long delta = 0;
			switch (eventType) {
				case ScrollEventType.LargeDecrement:
					delta = -scrollBarAdapter.LargeChange;
					break;
				case ScrollEventType.SmallDecrement:
					delta = -scrollBarAdapter.SmallChange;
					break;
				case ScrollEventType.LargeIncrement:
					delta = scrollBarAdapter.LargeChange;
					break;
				case ScrollEventType.SmallIncrement:
					delta = scrollBarAdapter.SmallChange;
					break;
				default:
					return 0;
			}
			return delta;
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine) {
			DateTime actualStart = View.VisibleStart;
			DateTime date = ShiftDate(actualStart, (int)delta);
			scrollBarAdapter.Value += delta;
			scrollBarAdapter.ApplyValuesToScrollBar();
			SetViewStart(date);
		}
		void SetViewStart(DateTime date) {
			InnerSchedulerViewBase innerView = View.InnerView;
			innerView.SetStartCore(date, View.Control.Selection);
			innerView.RaiseChanged(SchedulerControlChangeType.DateTimeScroll);
		}
		protected internal virtual DateTime ShiftDate(DateTime date, int offset) {
			if (offset == 0)
				return date;
			return (offset < 0) ? CalculateDateBackward(date, -(int)offset) : CalculateDateForward(date, (int)offset);
		}
		DateTime CalculateDateBackward(DateTime baseDate, long count) {
			DateTime date = baseDate;
			TimeScale scale = BaseScale;
			for (long i = 0; i < count; i++) {
				date = scale.GetPrevDate(date);
			}
			return date;
		}
		DateTime CalculateDateForward(DateTime baseDate, long count) {
			DateTime date = baseDate;
			TimeScale scale = BaseScale;
			for (long i = 0; i < count; i++)
				date = scale.GetNextDate(date);
			return date;
		}
	}
}
