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
using System.Linq;
using System.Text;
using DevExpress.XtraScheduler.Native;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Scheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	#region DayViewScrollBarAdapter
	public class DayViewScrollBarAdapter : ScrollBarAdapter {
		private TimeOfDayInterval actualVisibleTime = TimeOfDayInterval.Empty;
		ScrollViewer viewer;
		DevExpress.Xpf.Scheduler.Drawing.PixelSnappedSharedSizePanel cellPanel;
		public DayViewScrollBarAdapter(DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper scrollBar)
			: base(scrollBar) {
				this.viewer = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<ScrollViewer>(scrollBar.ScrollBar);
			this.cellPanel = (DevExpress.Xpf.Scheduler.Drawing.PixelSnappedSharedSizePanel)DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(viewer, "TimeCellPanel");
			BindToScrollBarValue(scrollBar);
			SubscribeScrollViewerEvents();
		}
		#region Property
		public ScrollViewer Viewer { get { return viewer; } }
		public DevExpress.Xpf.Scheduler.Drawing.PixelSnappedSharedSizePanel CellPanel { get { return cellPanel; } }
		protected internal override bool DeferredScrollBarUpdate { get { return false; } }
		protected internal override bool Synchronized { get { return true; } set { } }
		public TimeOfDayInterval ActualVisibleTime { get { return actualVisibleTime; } set { actualVisibleTime = value; } }
		#region Offset
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public static readonly DependencyProperty OffsetProperty = CreateOffsetProperty();
		static DependencyProperty CreateOffsetProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayViewScrollBarAdapter, double>("Offset", 0, (d, e) => d.OnOffsetChanged(e.OldValue, e.NewValue), null);
		}
		private void OnOffsetChanged(double oldValue, double newValue) {
			DateTimeScrollEventArgs args = new DateTimeScrollEventArgs(ScrollEventType.EndScroll, newValue);
			RaiseScroll(args);
		}
		#endregion
		#endregion
		public TimeOfDayInterval GetTimeOffset(TimeOfDayInterval actualVisibleTime) {
			return ScrollViewerHelper.CalcTimeOffset(viewer, actualVisibleTime);
		}
		void BindToScrollBarValue(DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper scrollBar) {
			Binding offsetBinding = new Binding();
			offsetBinding.Source = scrollBar.ScrollBar;
			offsetBinding.Path = new PropertyPath("Value");
			offsetBinding.Mode = BindingMode.TwoWay;
			BindingOperations.SetBinding(this, OffsetProperty, offsetBinding);
		}
		protected internal override void SubscribeScrollbarEvents() {
		}
		protected internal override void UnsubscribeScrollbarEvents() {
		}
		protected internal override void ApplyValuesToScrollBarCore() {
			viewer.ScrollToVerticalOffset(Value);
		}
		void SubscribeScrollViewerEvents() {
			this.viewer.LayoutUpdated += OnViewerLayoutUpdated;
		}
		void UnsubscribeScrollViewerEvents() {
			this.viewer.LayoutUpdated -= OnViewerLayoutUpdated;
		}
		void OnViewerLayoutUpdated(object sender, EventArgs e) {
			RefreshValuesFromScrollBar();
		}
		public override void Deactivate() {
			base.Deactivate();
			UnsubscribeScrollViewerEvents();
			this.viewer = null;
			this.cellPanel = null;
		}
		protected internal override void OnScroll(object sender, ScrollEventArgs e) {
		}
	}
	#endregion
	#region DayViewDateTimeScrollController
	public class DayViewDateTimeScrollController : ViewDateTimeScrollController {
		public DayViewDateTimeScrollController(DayView view)
			: base(view) {
		}
		#region Properties
		public new DayView View { get { return (DayView)base.View; } }
		#endregion
		protected internal override bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return false;
		}
		protected internal override bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar) {
			return false;
		}
		protected internal override void OnScroll(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
			View.TopRowTime = CalculateTimeFromScrollBarPosition(View, scrollBarAdapter);
		}
		internal static TimeSpan CalculateTimeFromScrollBarPosition(DayView view, IScrollBarAdapter scrollBarAdapter) {
			long totalTicks = view.ActualVisibleTime.End.Ticks - view.ActualVisibleTime.Start.Ticks;
			DayViewScrollBarAdapter adapter = (DayViewScrollBarAdapter)scrollBarAdapter;
			long oneTick = totalTicks / (long)adapter.Viewer.ExtentHeight;
			return new TimeSpan(oneTick * (int)adapter.Viewer.VerticalOffset + view.ActualVisibleTime.Start.Ticks);
		}
		protected internal override void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter) {
			DayViewScrollBarAdapter adapter = scrollBarAdapter as DayViewScrollBarAdapter;
			if (adapter != null && adapter.CellPanel != null) {
				System.Windows.Size size = new System.Windows.Size(adapter.Viewer.ExtentWidth, adapter.Viewer.ExtentHeight);
				DateTime selectedStart = View.SelectedInterval.Start;
				TimeSpan actualTimeSpan = View.TopRowTime.Ticks < 0 ? new TimeSpan(selectedStart.Hour, selectedStart.Minute, selectedStart.Second) : View.TopRowTime;
				long customY = (long)SchedulerRectUtils.CalcDateY(actualTimeSpan, View.ActualVisibleTime, size);
				if (customY == adapter.Value)
					return;
				adapter.Value = customY;
				adapter.ApplyValuesToScrollBar();
			}
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine) {
#if !SL
			DayViewScrollBarAdapter adapter = scrollBarAdapter as DayViewScrollBarAdapter;
			if (!deltaAsLine) {
				adapter.Viewer.ScrollToVerticalOffset(adapter.Viewer.VerticalOffset + delta);
				return;
			}
			int count = (int)Math.Abs(delta);
			if (delta > 0)
				Scroll(count, () => adapter.Viewer.LineDown());
			else
				Scroll(count, () => adapter.Viewer.LineUp());
#endif
		}
#if !SL
		void Scroll(int lineCount, Action scrollDelegate) {
			for (int i = 0; i < lineCount; i++) {
				scrollDelegate();
			}
		}
#endif        
	}
	#endregion
}
