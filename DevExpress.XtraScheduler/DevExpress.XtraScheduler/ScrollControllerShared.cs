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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Commands;
using System.Drawing;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
#if !SILVERLIGHT && !WPF
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Services.Internal;
using System.CodeDom.Compiler;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using System.Windows.Controls.Primitives;
using ScrollBarBase = System.Windows.Controls.Primitives.ScrollBar;
#if WPF||SL
using DevExpress.Xpf.Scheduler;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
#endif
namespace DevExpress.XtraScheduler.Native {
	#region IScrollBarAdapter
	public interface IScrollBarAdapter : IBatchUpdateable, IBatchUpdateHandler {
		void Activate();
		void Deactivate();
		void RefreshValuesFromScrollBar();
		bool SynchronizeScrollBarAvoidJump();
		void EnsureSynchronized();
		void ApplyValuesToScrollBar();
		ScrollEventArgs CreateEmulatedScrollEventArgs(ScrollEventType eventType);
		int GetRawScrollBarValue();
		bool SetRawScrollBarValue(int value);
		int GetPageUpRawScrollBarValue();
		int GetPageDownRawScrollBarValue();
		event DateTimeScrollEventHandler Scroll;
		long Minimum { get; set; }
		long Maximum { get; set; }
		long Value { get; set; }
		long LargeChange { get; set; }
		long SmallChange { get; set; }
	}
	#endregion
	#region ScrollBarAdapter (abstract class)
#if !SILVERLIGHT && !WPF
	public abstract class ScrollBarAdapter : IScrollBarAdapter {
#else
	public abstract class ScrollBarAdapter : DependencyObject, IScrollBarAdapter {
#endif
		#region Fields
#if !SILVERLIGHT && !WPF
		ScrollBarBase scrollBar;
#else
		DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper scrollBar;
#endif
		BatchUpdateHelper batchUpdateHelper;
		double factor = 1.0;
		long minimum;
		long maximum;
		long val;
		long largeChange;
		long smallChange;
		#endregion
#if !SILVERLIGHT && !WPF
		protected ScrollBarAdapter(ScrollBarBase scrollBar) {
#else
		protected ScrollBarAdapter(DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper scrollBar) {
#endif
			Guard.ArgumentNotNull(scrollBar, "scrollBar");
			this.scrollBar = scrollBar;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region Properties
		protected internal double Factor { get { return factor; } set { factor = value; } }
		protected internal abstract bool DeferredScrollBarUpdate { get; }
		protected internal abstract bool Synchronized { get; set; }
		#region Minimum
		public long Minimum {
			get { return minimum; }
			set {
				if (minimum.Equals(value))
					return;
				minimum = value;
				ValidateValues();
			}
		}
		#endregion
		#region Maximum
		public long Maximum {
			get { return maximum; }
			set {
				if (maximum.Equals(value))
					return;
				maximum = value;
				ValidateValues();
			}
		}
		#endregion
		#region Value
		public long Value {
			get { return val; }
			set {
				if (val.Equals(value))
					return;
				val = value;
				ValidateValues();
			}
		}
		#endregion
		#region LargeChange
		public long LargeChange {
			get { return largeChange; }
			set {
				if (largeChange.Equals(value))
					return;
				largeChange = value;
				ValidateValues();
			}
		}
		#endregion
		#region SmallChange
		public long SmallChange {
			get { return smallChange; }
			set {
				if (smallChange.Equals(value))
					return;
				smallChange = value;
				ValidateValues();
			}
		}
		#endregion
#if !SILVERLIGHT && !WPF
		protected internal ScrollBarBase ScrollBar { get { return scrollBar; } }
#else
		protected internal DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper ScrollBar { get { return scrollBar; } }
#endif
		#endregion
		#region Events
		#region Scroll
		DateTimeScrollEventHandler onScroll;
		public event DateTimeScrollEventHandler Scroll { add { onScroll += value; } remove { onScroll -= value; } }
		protected internal virtual void RaiseScroll(DateTimeScrollEventArgs args) {
			if (onScroll != null)
				onScroll(ScrollBar, args);
		}
		#endregion
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		public virtual void Activate() {
			RefreshValuesFromScrollBar();
			SubscribeScrollbarEvents();
		}
		public virtual void Deactivate() {
			UnsubscribeScrollbarEvents();
		}
		protected internal virtual void OnLastEndUpdateCore() {
			ValidateValues();
		}
		protected internal virtual void ValidateValues() {
			if (!IsUpdateLocked)
				ValidateValuesCore();
		}
		protected internal virtual void ValidateValuesCore() {
			if (LargeChange < 0)
				Exceptions.ThrowArgumentException("LargeChange", LargeChange);
			if (SmallChange < 0)
				Exceptions.ThrowArgumentException("SmallChange", SmallChange);
			if (Minimum > Maximum)
				this.maximum = Minimum;
			this.largeChange = Math.Min(LargeChange, Maximum - Minimum + 1);
			this.smallChange = Math.Min(SmallChange, LargeChange);
			this.val = Math.Max(Value, Minimum);
			this.val = Math.Min(Value, Maximum - LargeChange + 1);
		}
		protected internal virtual void SubscribeScrollbarEvents() {
			ScrollBar.Scroll += new ScrollEventHandler(OnScroll);
		}
		protected internal virtual void UnsubscribeScrollbarEvents() {
			ScrollBar.Scroll -= new ScrollEventHandler(OnScroll);
		}
		public virtual bool SynchronizeScrollBarAvoidJump() {
			if (ShouldSynchronize()) {
				double relativePos = Value / (float)(Maximum - LargeChange + 1 - Minimum);
				double actualRelativePos = ScrollBar.Value / Math.Max(1.0d, ScrollBar.Maximum - ScrollBar.LargeChange + 1 - ScrollBar.Minimum);
				if (relativePos <= actualRelativePos) {
					ApplyValuesToScrollBarCore();
					XtraSchedulerDebug.Assert(Synchronized);
					return true;
				}
			}
			return false;
		}
		protected internal bool EnsureSynchronizedCore() {
			if (ShouldSynchronize()) {
				ApplyValuesToScrollBarCore();
				XtraSchedulerDebug.Assert(Synchronized);
				return true;
			}
			XtraSchedulerDebug.Assert(Synchronized);
			return false;
		}
		public virtual void EnsureSynchronized() {
			EnsureSynchronizedCore();
		}
		protected internal virtual bool ShouldSynchronize() {
			return DeferredScrollBarUpdate && !Synchronized;
		}
		protected internal virtual void OnScroll(object sender, ScrollEventArgs e) {
#if !SILVERLIGHT && !WPF
			ScrollEventType type = e.Type;
#else
			ScrollEventType type = e.ScrollEventType;
#endif
			DateTimeScrollEventArgs args = new DateTimeScrollEventArgs(type, e.NewValue);
			if (EnsureSynchronizedCore()) {
				int delta = ((int)e.NewValue) - GetRawScrollBarValue();
				args.NewValue = GetRawScrollBarValue() + delta;
				RaiseScroll(args);
			} else
				RaiseScroll(args);
#if !SILVERLIGHT && !WPF
			e.NewValue = (int)args.NewValue;
#endif
		}
		public virtual void ApplyValuesToScrollBar() {
			if (DeferredScrollBarUpdate)
				Synchronized = false;
			else
				ApplyValuesToScrollBarCore();
		}
		protected internal virtual void ApplyValuesToScrollBarCore() {
#if !SILVERLIGHT && !WPF
			if (this.Maximum > (long)int.MaxValue)
				Factor = 1.0 / (1 + (this.Maximum / (long)int.MaxValue));
			else
				Factor = 1.0;
			ScrollBar.BeginUpdate();
			try {
				ScrollBar.Minimum = (int)Math.Round(Factor * this.Minimum);
				ScrollBar.Maximum = (int)Math.Round(Factor * this.Maximum);
				ScrollBar.LargeChange = (int)Math.Round(Factor * this.LargeChange);
				ScrollBar.SmallChange = (int)Math.Round(Factor * this.SmallChange);
				ScrollBar.Value = (int)Math.Round(Factor * this.Value);
			} finally {
				ScrollBar.EndUpdate();
			}
#else
			DoScrollBarAction(delegate {
				ScrollBar.Minimum = (int)Math.Round(Factor * this.Minimum);
				ScrollBar.Maximum = (int)Math.Round(Factor * this.Maximum) - (int)Math.Round(Factor * this.LargeChange) + 1;
				ScrollBar.SmallChange = 1;
				ScrollBar.LargeChange = (int)Math.Round(Factor * this.LargeChange);
				ScrollBar.SmallChange = (int)Math.Round(Factor * this.SmallChange);
				ScrollBar.Value = (int)Math.Round(Factor * this.Value);
				ScrollBar.ViewportSize = ScrollBar.LargeChange;
				ScrollBar.Visibility = this.Maximum > 2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			}, false);
#endif
			this.Synchronized = true;
		}
		public virtual void RefreshValuesFromScrollBar() {
			BeginUpdate();
			try {
				Minimum = (long)Math.Round(ScrollBar.Minimum / Factor);
				Maximum = (long)Math.Round(ScrollBar.Maximum / Factor);
				LargeChange = (long)Math.Round(ScrollBar.LargeChange / Factor);
				SmallChange = (long)Math.Round(ScrollBar.SmallChange / Factor);
				Value = (long)Math.Round(ScrollBar.Value / Factor);
			} finally {
				EndUpdate();
			}
			Synchronized = true;
		}
		public virtual ScrollEventArgs CreateEmulatedScrollEventArgs(ScrollEventType eventType) {
			EnsureSynchronized();
			switch (eventType) {
				case ScrollEventType.First:
					return new ScrollEventArgs(eventType, ScrollBar.Minimum);
				case ScrollEventType.Last:
#if SILVERLIGHT || WPF
					return new ScrollEventArgs(eventType, ScrollBar.Maximum);
#else
					return new ScrollEventArgs(eventType, ScrollBar.Maximum - ScrollBar.LargeChange + 1);
#endif
				case ScrollEventType.SmallIncrement:
					return new ScrollEventArgs(eventType, ScrollBar.Value + ScrollBar.SmallChange); 
				case ScrollEventType.SmallDecrement:
					return new ScrollEventArgs(eventType, ScrollBar.Value - ScrollBar.SmallChange); 
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
#if SILVERLIGHT || WPF
		delegate void EmptyDelegate();
		protected void DoScrollBarAction(Action action, bool waitForCompletion) {
			if (!ScrollBar.Dispatcher.CheckAccess()) {
				if (waitForCompletion) {
					System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
#if WPF
					EmptyDelegate emptyDelegate = delegate { action(); wait.Set(); };
					ScrollBar.Dispatcher.BeginInvoke(emptyDelegate);
#else
					ScrollBar.Dispatcher.BeginInvoke(delegate { action(); wait.Set(); });
#endif
					wait.WaitOne();
				} else
					ScrollBar.Dispatcher.BeginInvoke(action);
			} else
				action();
		}
#endif
		public int GetRawScrollBarValue() {
			EnsureSynchronized();
#if SILVERLIGHT || WPF
			double result = 0;
			DoScrollBarAction(delegate { result = ScrollBar.Value; }, true);
			return (int)result;
#else
			return ScrollBar.Value;
#endif
		}
		public bool SetRawScrollBarValue(int value) {
			XtraSchedulerDebug.Assert(Synchronized);
#if SILVERLIGHT || WPF
			Value = (long)Math.Round(value / Factor);
			if (GetRawScrollBarValue() != value) {
				DoScrollBarAction(delegate { ScrollBar.Value = value; }, false);
				return true;
			} else
				return false;
#else
			if (ScrollBar.Value != value) {
				ScrollBar.Value = value;
				Value = (long)Math.Round(value / Factor);
				return true;
			} else
				return false;
#endif
		}
		public virtual int GetPageUpRawScrollBarValue() {
			EnsureSynchronized();
#if SILVERLIGHT || WPF
			int result = 0;
			DoScrollBarAction(delegate {
				result = (int)(ScrollBar.Value - ScrollBar.LargeChange);
				if (result < ScrollBar.Minimum)
					result = (int)ScrollBar.Minimum;
			}, true);
			return result;
#else
			return Math.Max(ScrollBar.Minimum, ScrollBar.Value - ScrollBar.LargeChange);
#endif
		}
		public virtual int GetPageDownRawScrollBarValue() {
			EnsureSynchronized();
#if SILVERLIGHT || WPF
			int result = 0;
			DoScrollBarAction(delegate {
				result = (int)(ScrollBar.Value + ScrollBar.LargeChange);
				int maxValue = (int)(ScrollBar.Maximum - ScrollBar.LargeChange + 1);
				if (result > maxValue)
					result = maxValue;
			}, true);
			return result;
#else
			return Math.Min(ScrollBar.Maximum - ScrollBar.LargeChange + 1, ScrollBar.Value + ScrollBar.LargeChange);
#endif
		}
	}
	#endregion
	#region SchedulerScrollBarAdapter
	public class SchedulerScrollBarAdapter : ScrollBarAdapter {
#if !SILVERLIGHT && !WPF
		public SchedulerScrollBarAdapter(ScrollBarBase scrollBar)
#else
		public SchedulerScrollBarAdapter(DevExpress.Xpf.Scheduler.Drawing.ScrollBarWrapper scrollBar)
#endif
			: base(scrollBar) {
		}
		protected internal override bool DeferredScrollBarUpdate { get { return false; } }
		protected internal override bool Synchronized { get { return true; } set { } }
	}
	#endregion
#if SILVERLIGHT || WPF
	public enum NavigatorButtonType {
		Custom = 0,
		First = 1,
		PrevPage = 2,
		Prev = 3,
		Next = 4,
		NextPage = 5,
		Last = 6,
		Append = 7,
		Remove = 8,
		Edit = 9,
		EndEdit = 10,
		CancelEdit = 11,
	}
#endif
	#region DateTimeScrollBar
	public class DateTimeScrollBar : IDisposable {
		#region Fields
#if!SILVERLIGHT && !WPF
		ScrollBarBase scrollBar;
		ScrollBarType scrollBarType = ScrollBarType.Vertical;
		bool visible;
#else
#endif
		IScrollBarAdapter scrollBarAdapter;
		bool isDisposed;
		SchedulerControl control;
		#endregion
		public DateTimeScrollBar(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			CreateScrollBar();
			this.scrollBarAdapter = CreateScrollBarAdapter();
			ScrollBarAdapter.Activate();
			SubscribeScrollbarEvents();
		}
		#region Properties
		#region ScrollBar
#if!SILVERLIGHT && !WPF
		public ScrollBarBase ScrollBar { get { return scrollBar; } }
#else
#endif
		public IScrollBarAdapter ScrollBarAdapter { get { return scrollBarAdapter; } }
		#endregion
		#region IsDisposed
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
#if!SILVERLIGHT&&!WPF
		#region Visible
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				this.visible = value;
				scrollBar.Visible = value;
			}
		}
		#endregion
		#region ScrollBarType
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal ScrollBarType ScrollBarType {
			get { return scrollBarType; }
			set {
				if (scrollBarType == value)
					return;
				ScrollBarAdapter.Deactivate();
				UnsubscribeScrollbarEvents();
				scrollBarAdapter = null;
				DestroyScrollBar();
				scrollBarType = value;
				CreateScrollBar(); 
				scrollBarAdapter = CreateScrollBarAdapter();
				ScrollBarAdapter.Activate();
				SubscribeScrollbarEvents();
			}
		}
		#endregion
#endif
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal SchedulerControl Control { get { return control; } }
		#endregion
		#endregion
		#region Events
		DateTimeScrollEventHandler onScroll;
		internal event DateTimeScrollEventHandler Scroll { add { onScroll += value; } remove { onScroll -= value; } }
		protected internal virtual void RaiseScroll(object sender, DateTimeScrollEventArgs e) {
			if (onScroll != null)
				onScroll(sender, e);
			if (ScrollBarAdapter != null) {
				ScrollBarAdapter.SetRawScrollBarValue((int)e.NewValue);
			}
		}
		#endregion
		protected internal virtual void SubscribeScrollbarEvents() {
			ScrollBarAdapter.Scroll += new DateTimeScrollEventHandler(RaiseScroll);
		}
		protected internal virtual void UnsubscribeScrollbarEvents() {
			ScrollBarAdapter.Scroll -= new DateTimeScrollEventHandler(RaiseScroll);
		}
		protected virtual IScrollBarAdapter CreateScrollBarAdapter() {
#if!SILVERLIGHT&&!WPF
			return new SchedulerScrollBarAdapter(scrollBar);
#else
			SchedulerViewType activeViewType = Control.ActiveViewType;
			if (activeViewType == SchedulerViewType.Day || activeViewType == SchedulerViewType.WorkWeek || activeViewType == SchedulerViewType.FullWeek)
				return new DevExpress.Xpf.Scheduler.Native.DayViewScrollBarAdapter(control.DateTimeScrollBar);
			return new SchedulerScrollBarAdapter(control.DateTimeScrollBar);
#endif
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollBarAdapter != null) {
					UnsubscribeScrollbarEvents();
					scrollBarAdapter.Deactivate();
					scrollBarAdapter = null;
				}
#if!SILVERLIGHT&&!WPF
				if (scrollBar != null) {
					DestroyScrollBar();
				}
#endif
				this.control = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DateTimeScrollBar() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void CreateScrollBar() {
#if!SILVERLIGHT&&!WPF
			if (scrollBarType == ScrollBarType.Vertical)
				this.scrollBar = new DevExpress.XtraEditors.VScrollBar();
			else
				this.scrollBar = new DevExpress.XtraEditors.HScrollBar();
			scrollBar.Visible = this.visible;
			scrollBar.ScrollBarAutoSize = true;
			UpdatePaintStyle();
			control.Controls.Add(scrollBar);
#endif
		}
		protected internal virtual void DestroyScrollBar() {
#if!SILVERLIGHT&&!WPF
			scrollBar.Dispose();
			scrollBar = null;
#endif
		}
#if!SILVERLIGHT&&!WPF
		protected internal virtual void UpdatePaintStyle() {
			scrollBar.LookAndFeel.ParentLookAndFeel = control.PaintStyle.UserLookAndFeel;
		}
#endif
	}
	#endregion
	#region ViewDateTimeScrollController
	public abstract class ViewDateTimeScrollController : IDisposable {
		#region Fields
		bool isDisposed;
		SchedulerViewBase view;
		DateTimeScrollBar dateTimeScrollBar;
		SchedulerBlocker scrollBarSubscribtionLocker;
		bool isUpdateScrollValueSuspended = false;
		#endregion
		protected ViewDateTimeScrollController(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.dateTimeScrollBar = view.Control.DateTimeScrollBarObject;
			this.scrollBarSubscribtionLocker = new SchedulerBlocker();
			Guard.ArgumentNotNull(this.dateTimeScrollBar, "dateTimeScrollBar");
			SubscribeScrollBarEvents();
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		public DateTimeScrollBar DateTimeScrollBar { get { return dateTimeScrollBar; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected bool IsUpdateScrollValueSuspended { get { return isUpdateScrollValueSuspended; } }
		#endregion
		#region Events
		DateTimeScrollEventHandler onScroll;
		internal event DateTimeScrollEventHandler ScrollEvent { add { onScroll += value; } remove { onScroll -= value; } }
		protected internal virtual void RaiseScroll(object sender, DateTimeScrollEventArgs e) {
			if (onScroll != null)
				onScroll(sender, e);
		}
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (dateTimeScrollBar != null) {
					UnsubscribeScrollBarEvents();
					this.dateTimeScrollBar = null;
				}
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ViewDateTimeScrollController() {
			Dispose(false);
		}
		#endregion
		protected internal abstract bool IsDateTimeScrollbarVisibilityDependsOnClientSize();
		protected internal abstract bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar);
		protected internal abstract void OnScroll(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e);
		protected internal abstract void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter);
		protected internal abstract void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine);
		protected internal virtual void Scroll(IScrollBarAdapter scrollBarAdapter, long delta) {
			Scroll(scrollBarAdapter, delta, true);
		}
		protected internal virtual void UpdateScrollBarPosition() {
			if (IsUpdateScrollValueSuspended)
				return;
			if (DateTimeScrollBar.ScrollBarAdapter == null) 
				return;
			BeforeUpdateScrollBarPosition();
			BeginUpdate();
			try {
				UpdateScrollBarPositionCore(DateTimeScrollBar.ScrollBarAdapter);
			} finally {
				EndUpdate();
			}
			AfterUpdateScrollBarPosition();
		}
		protected internal virtual void AfterUpdateScrollBarPosition() {
		}
		protected internal virtual void BeforeUpdateScrollBarPosition() {
		}
		protected internal virtual void BeginUpdate() {
			UnsubscribeScrollBarEvents();
			this.scrollBarSubscribtionLocker.Lock();
		}
		protected internal virtual void EndUpdate() {
			this.scrollBarSubscribtionLocker.Unlock();
			SubscribeScrollBarEvents();
		}
		protected internal virtual void SubscribeScrollBarEvents() {
			if (this.scrollBarSubscribtionLocker.IsLocked)
				return;
			DateTimeScrollBar.Scroll += OnDateTimeScroll;
		}
		protected internal virtual void UnsubscribeScrollBarEvents() {
			if (this.scrollBarSubscribtionLocker.IsLocked)
				return;
			DateTimeScrollBar.Scroll -= OnDateTimeScroll;
		}
		protected internal virtual void OnDateTimeScroll(object sender, DateTimeScrollEventArgs e) {
			BeginUpdate();
			try {
				OnScroll(dateTimeScrollBar.ScrollBarAdapter, e);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual long NormalizePosition(long pos, IScrollBarAdapter scrollBarAdapter) {
			if (pos < scrollBarAdapter.Minimum)
				pos = scrollBarAdapter.Minimum;
			long maxValue = scrollBarAdapter.Maximum - scrollBarAdapter.LargeChange + 1;
			if (pos > maxValue)
				pos = maxValue;
			return pos;
		}
		public void SuspendUpdateScrollValue() {
			this.isUpdateScrollValueSuspended = true;
		}
		public void ResumeUpdateScrollValue() {
			this.isUpdateScrollValueSuspended = false;
		}
	}
	#endregion
#if!SILVERLIGHT&&!WPF
	public class DayViewDateTimeScrollController : ViewDateTimeScrollController {
		public DayViewDateTimeScrollController(DayView view)
			: base(view) {
		}
		#region Properties
		public new DayView View { get { return (DayView)base.View; } }
		#endregion
		protected internal override bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return true;
		}
		protected internal override bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar) {
			DayViewInfo viewInfo = View.ViewInfo;
			bool containerScrollBarVisible = viewInfo.ShouldShowContainerScrollBar();
			if (containerScrollBarVisible) {
				scrollBar.Visible = true;
				return true;
			}
			bool prevScrollBarVisibility = scrollBar.Visible;
			scrollBar.Visible = View.DateTimeScrollbarVisible && viewInfo.DateTimeScrollBarVisible;
			return scrollBar.Visible != prevScrollBarVisibility;
		}
		protected internal override void OnScroll(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
			if (e.Type == ScrollEventType.EndScroll)
				return;
			DayViewInfo viewInfo = View.ViewInfo;
			if (viewInfo.Rows.Count > 0) {
				TimeSpan oldTopRowTime = View.TopRowTime;
				View.TopRowTime = viewInfo.Rows[(int)e.NewValue].Interval.Start;
				if (oldTopRowTime != View.TopRowTime) {
					UnsubscribeScrollBarEvents();
				}
			}
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine) {
			long newValue = NormalizePosition(scrollBarAdapter.Value + delta, scrollBarAdapter);
			DayViewInfo viewInfo = View.ViewInfo;
			if (viewInfo.Rows.Count > 0)
				View.TopRowTime = viewInfo.Rows[(int)newValue].Interval.Start;
			scrollBarAdapter.Value = newValue;
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
		protected internal override void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter) {
			scrollBarAdapter.BeginUpdate();
			try {
				scrollBarAdapter.Minimum = 0;
				scrollBarAdapter.Maximum = View.ViewInfo.Rows.Count - 1;
				scrollBarAdapter.Value = DateToPosition(View.TopRowTime);
				scrollBarAdapter.SmallChange = 1;
				scrollBarAdapter.LargeChange = View.ViewInfo.VisibleRows.Count;
			} finally {
				scrollBarAdapter.EndUpdate();
			}
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
		int DateToPosition(TimeSpan date) {
			TimeSpan visibleStart = DateTimeHelper.Floor(View.ActualVisibleTime.Start, View.TimeScale);
			if (date.Ticks - visibleStart.Ticks < 0)
				return 0;
			return DateTimeHelper.Divide(date - visibleStart, View.TimeScale);
		}
	}
	public class WorkWeekViewDateTimeScrollController : DayViewDateTimeScrollController {
		public WorkWeekViewDateTimeScrollController(WorkWeekView view)
			: base(view) {
		}
	}
	public class FullWeekViewDateTimeScrollController : DayViewDateTimeScrollController {
		public FullWeekViewDateTimeScrollController(FullWeekView view)
			: base(view) {
		}
	}
#endif
	public abstract class InfiniteDateTimeScrollControllerBase : ViewDateTimeScrollController {
		DateTime deferredStart = DateTime.MinValue;
		bool isDeferredStart = false;
		TimeInterval scrollRange;
		protected InfiniteDateTimeScrollControllerBase(SchedulerViewBase view)
			: base(view) {
		}
		protected internal TimeInterval ScrollRange { get { return scrollRange; } set { scrollRange = value; } }
#if !WPF && !SL
		protected virtual ToolTipLocation DeferredScrollingToolTipLocation { get { return ToolTipLocation.TopCenter; } }
#endif
		protected internal override void OnScroll(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
			ScrollEventType eventType = e.Type;
			TimeInterval scrollInterval = GetScrollRange();
			long delta = ((int)e.NewValue) - scrollBarAdapter.Value;
			StartOrContinueTrackDeferredStart();
			if (eventType == ScrollEventType.EndScroll) {
				CommitDeferredStart();
				ValidateScrollBarPosition(scrollBarAdapter, e);
				return;
			}
			if (delta == 0)
				delta = CalculateScrollDelta(eventType, e.NewValue, scrollInterval, scrollBarAdapter);
			if (delta != 0) {
				int newValue = MakeScrollToDelta(delta, scrollInterval, eventType, scrollBarAdapter);
				if (newValue != 0)
					e.NewValue = newValue;
			}
		}
		protected internal virtual int MakeScrollToDelta(long scrollDelta, TimeInterval scrollInterval, ScrollEventType eventType, IScrollBarAdapter scrollBarAdapter) {
			return 0;
		}
		protected virtual long CalculateScrollDelta(ScrollEventType eventType, double newValue, TimeInterval scrollInterval, IScrollBarAdapter scrollBarAdapter) {
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
		protected internal virtual void ValidateScrollBarPosition(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
		}
		#region Deferred Start logic
		protected bool IsDeferredScrollingEnabled { get { return View.InnerView.DeferredScrolling.Allow; } }
		void InitializeDeferredStart() {
			this.deferredStart = View.VisibleStart;
		}
		void StartOrContinueTrackDeferredStart() {
			this.isDeferredStart = IsDeferredScrollingEnabled;
		}
		bool SetDeferredStart(DateTime date) {
			if (!this.isDeferredStart)
				return false;
			this.deferredStart = date;
			return true;
		}
		void CommitDeferredStart() {
			if (!this.isDeferredStart)
				return;
			this.isDeferredStart = false;
			SetViewStart(this.deferredStart);
			HideDeferredStart();
		}
		protected virtual void ShowDeferredStart() {
			if (!this.isDeferredStart)
				return;
#if SL || WPF
			ToolTip toolTip = ObtainToolTip();
			toolTip.Content = this.deferredStart.ToShortDateString();
#if !SL
			toolTip.StaysOpen = true;
#endif
			toolTip.IsOpen = true;
			toolTip.IsEnabled = true;
			toolTip.Placement = PlacementMode.Mouse;
			toolTip.Visibility = Visibility.Visible;
#else
			View.Control.ActualToolTipController.ShowHint(this.deferredStart.ToString(), DeferredScrollingToolTipLocation);
#endif
		}
		protected void HideDeferredStart() {
#if SL || WPF
			ToolTip toolTip = ObtainToolTip();
			toolTip.IsOpen = false;
			toolTip.IsEnabled = false;
#else
			View.Control.ActualToolTipController.HideHint();
#endif
		}
		#region ObtainToolTip
#if SL
		ToolTip ObtainToolTip() {
			FrameworkElement toolTipContainer = this.View.Control.DateTimeScrollBar.ScrollBar;
			ToolTip result = ToolTipService.GetToolTip(toolTipContainer) as ToolTip;
			if (result == null) {
				result = new ToolTip();
				ToolTipService.SetToolTip(toolTipContainer, result);
			}
			return result;
		}
#endif
#if WPF
		ToolTip ObtainToolTip() {
			FrameworkElement toolTipContainer = this.View.Control.DateTimeScrollBar.ScrollBar;
			ToolTip result = toolTipContainer.ToolTip as ToolTip;
			if (result == null) {
				result = new ToolTip();
				toolTipContainer.ToolTip = result;
				ToolTipService.SetToolTip(toolTipContainer, result);
			}
			return result;
		}
#endif
		#endregion
		protected DateTime GetExpectedViewStart() {
			return (this.isDeferredStart) ? this.deferredStart : View.VisibleStart;
		}
		#endregion
		protected internal virtual void SetViewStart(DateTime date) {
			if (SetDeferredStart(date))
				return;
			InnerSchedulerViewBase innerView = View.InnerView;
			innerView.SetStartCore(date, View.Control.Selection);
			innerView.RaiseChanged(SchedulerControlChangeType.DateTimeScroll);
		}
		protected internal override void UpdateScrollBarPosition() {
			base.UpdateScrollBarPosition();
			InitializeDeferredStart();
		}
		protected internal virtual TimeInterval GetScrollRange() {
			return TimeInterval.Intersect(ScrollRange, View.LimitInterval);
		}
	}
	#region InfiniteTimelineDateTimeScrollController
	public abstract class InfiniteTimelineDateTimeScrollController : InfiniteDateTimeScrollControllerBase {
		protected InfiniteTimelineDateTimeScrollController(SchedulerViewBase view)
			: base(view) {
		}
		protected internal override bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return false;
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta) {
			Scroll(scrollBarAdapter, delta, true);
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine) {
			if (delta == 0)
				return;
			TimeInterval scrollInterval = GetScrollRange();
			scrollBarAdapter.Value = (int)ScrollCore(scrollBarAdapter, true, delta, scrollInterval);
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
		protected internal virtual bool CanPerformScrollOutsideRange(ScrollEventType type) {
			return type == ScrollEventType.SmallDecrement || type == ScrollEventType.SmallIncrement ||
				type == ScrollEventType.LargeDecrement || type == ScrollEventType.LargeIncrement;
		}
		protected override long CalculateScrollDelta(ScrollEventType eventType, double newValue, TimeInterval scrollInterval, IScrollBarAdapter scrollBarAdapter) {
			long delta = base.CalculateScrollDelta(eventType, newValue, scrollInterval, scrollBarAdapter);
			if (delta != 0)
				return delta;
			switch (eventType) {
				case ScrollEventType.ThumbPosition:
					PerformScrollInsideRange(scrollInterval, (int)newValue);
					return 0;
				default:
					if (View.InnerVisibleIntervals.Start < scrollInterval.Start)
						PerformScrollInsideRange(scrollInterval, 0);
					if (View.InnerVisibleIntervals.Start > scrollInterval.End)
						PerformScrollInsideRange(scrollInterval, scrollBarAdapter.Maximum - scrollBarAdapter.LargeChange + 1);
					if (eventType == ScrollEventType.ThumbTrack)
						ShowDeferredStart();
					return 0;
			}
		}
		protected internal override int MakeScrollToDelta(long scrollDelta, TimeInterval scrollInterval, ScrollEventType eventType, IScrollBarAdapter scrollBarAdapter) {
			bool canPerformScrollOutsideRange = CanPerformScrollOutsideRange(eventType);
			return (int)ScrollCore(scrollBarAdapter, canPerformScrollOutsideRange, scrollDelta, scrollInterval);
		}
		protected internal virtual long ScrollCore(IScrollBarAdapter scrollBarAdapter, bool canPerformScrollOutsideRange, long delta, TimeInterval scrollInterval) {
			if (canPerformScrollOutsideRange && ShouldPerformScrollOutsideRange(scrollBarAdapter, delta, scrollInterval)) {
				PerformScrollOutsideRange(delta);
				long scrollPosition = DateToPosition(scrollInterval, View.InnerVisibleIntervals.Start);
				return NormalizePosition(scrollPosition, scrollBarAdapter);
			} else {
				long scrollPosition = NormalizePosition(scrollBarAdapter.Value + delta, scrollBarAdapter);
				PerformScrollInsideRange(scrollInterval, scrollPosition);
				return scrollPosition;
			}
		}
		protected internal virtual bool ShouldPerformScrollOutsideRange(IScrollBarAdapter scrollBarAdapter, long delta, TimeInterval scrollInterval) {
			if (delta < 0) {
				if (scrollBarAdapter.Value == scrollBarAdapter.Minimum)
					return true;
				if (View.InnerVisibleIntervals.End > scrollInterval.End)
					return true;
			} else if (delta > 0) {
				if (scrollBarAdapter.Value + scrollBarAdapter.LargeChange > scrollBarAdapter.Maximum)
					return true;
				if (View.InnerVisibleIntervals.Start < scrollInterval.Start)
					return true;
			}
			return false;
		}
		protected internal virtual void PerformScrollInsideRange(TimeInterval scrollInterval, long scrollPosition) {
			DateTime date = PositionToDate(scrollInterval, (int)scrollPosition);
			SetViewStart(date);
		}
		protected internal virtual void PerformScrollOutsideRange(long delta) {
			TimeSpan dateOffset = CalcScrollIntervalOverrun(delta);
			DateTime viewVisibleStart = GetExpectedViewStart();
			DateTime date = viewVisibleStart + dateOffset;
			if (date < View.LimitInterval.Start || View.InnerVisibleIntervals.End + dateOffset > View.LimitInterval.End)
				return;
			SetViewStart(date);
		}
		protected internal override void BeforeUpdateScrollBarPosition() {
			CalcScrollRange();
		}
		protected internal virtual void CalcScrollRange() {
			ScrollRange = GetScrollRangeCore(View.InnerVisibleIntervals.Start);
		}
		protected internal abstract TimeInterval GetScrollRangeCore(DateTime date);
		protected internal abstract TimeSpan CalcScrollIntervalOverrun(long delta);
		protected internal abstract DateTime PositionToDate(TimeInterval scrollInterval, int position);
		protected internal abstract long DateToPosition(TimeInterval scrollInterval, DateTime date);
	}
	#endregion
	#region WeekViewDateTimeScrollController
	public class WeekViewDateTimeScrollController : InfiniteTimelineDateTimeScrollController {
#if!SILVERLIGHT&&!WPF
		public WeekViewDateTimeScrollController(WeekView view)
			: base(view) {
		}
#else
		public WeekViewDateTimeScrollController(WeekViewBase view)
			: base(view) {
		}
#endif
		#region Properties
#if!SILVERLIGHT&&!WPF
		public new WeekView View { get { return (WeekView)base.View; } }
#else
		public new WeekViewBase View { get { return (WeekViewBase)base.View; } }
#endif
		protected internal virtual int WeekCount {
			get {
				InnerWeekView innerView = (InnerWeekView)View.InnerView;
				return innerView.WeekCountCore;
			}
		}
		#endregion
		protected internal override bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar) {
#if!SILVERLIGHT&&!WPF
			bool visibilityChanged = scrollBar.Visible != View.DateTimeScrollbarVisible;
			scrollBar.Visible = View.DateTimeScrollbarVisible;
			return visibilityChanged;
#else
			return false;
#endif
		}
		protected internal override TimeInterval GetScrollRangeCore(DateTime date) {
			DayOfWeek firstDayOfWeek = ((IInnerSchedulerViewOwner)View).FirstDayOfWeek;
			DateTime start = date;
			start = start.AddMonths(-6);
			start = DateTimeHelper.GetStartOfWeekUI(start, firstDayOfWeek, true);
			DateTime end = date;
			end = end.AddMonths(6);
			end = end.AddDays(7);
			end = DateTimeHelper.GetStartOfWeekUI(end, firstDayOfWeek, true);
			return new TimeInterval(start, end);
		}
		protected internal override TimeSpan CalcScrollIntervalOverrun(long delta) {
			return TimeSpan.FromTicks(DateTimeHelper.WeekSpan.Ticks * (int)delta);
		}
		protected internal override DateTime PositionToDate(TimeInterval scrollInterval, int position) {
			TimeSpan delta = TimeSpan.FromTicks(DateTimeHelper.WeekSpan.Ticks * (int)position);
			return scrollInterval.Start + delta;
		}
		protected internal override long DateToPosition(TimeInterval scrollInterval, DateTime date) {
			return DateTimeHelper.CalcWeekCount(date - scrollInterval.Start);
		}
		protected internal override void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter) {
			TimeInterval scrollInterval = GetScrollRange();
			int weeksCount = DateTimeHelper.CalcWeekCount(scrollInterval.Duration);
			scrollBarAdapter.BeginUpdate();
			try {
				scrollBarAdapter.Minimum = 0;
				scrollBarAdapter.Maximum = weeksCount - 1;
				scrollBarAdapter.SmallChange = 1;
				scrollBarAdapter.LargeChange = Math.Min(weeksCount, WeekCount);
				int currentWeek = (int)NormalizePosition(DateToPosition(scrollInterval, View.InnerVisibleIntervals.Start), scrollBarAdapter);
				scrollBarAdapter.Value = currentWeek;
			} finally {
				scrollBarAdapter.EndUpdate();
			}
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
	}
	#endregion
	#region MonthViewDateTimeScrollController
	public class MonthViewDateTimeScrollController : WeekViewDateTimeScrollController {
		public MonthViewDateTimeScrollController(MonthView view)
			: base(view) {
		}
		#region Properties
		public new MonthView View { get { return (MonthView)base.View; } }
		#endregion
		protected internal override void OnDateTimeScroll(object sender, DateTimeScrollEventArgs e) {
			base.OnDateTimeScroll(sender, e);
		}
		protected internal override TimeInterval GetScrollRangeCore(DateTime date) {
			DayOfWeek firstDayOfWeek = ((IInnerSchedulerViewOwner)View).FirstDayOfWeek;
			DateTime start = date;
			start = start.AddYears(-1);
			start = DateTimeHelper.GetStartOfWeekUI(start, firstDayOfWeek, View.CompressWeekend);
			DateTime end = date;
			end = end.AddYears(1);
			end = end.AddDays(7);
			end = DateTimeHelper.GetStartOfWeekUI(end, firstDayOfWeek, View.CompressWeekend);
			return new TimeInterval(start, end);
		}
	}
	#endregion    
	public class TimelineViewDateTimeScrollController : InfiniteDateTimeScrollControllerBase  {
		internal const int ScrollPageMultiplier = 7;
		DateTime baseDateTime;
		public TimelineViewDateTimeScrollController(TimelineView view)
			: base(view) {
		}
		#region Properties
		public new TimelineView View { get { return (TimelineView)base.View; } }
		protected internal DateTime BaseDateTime { get { return baseDateTime; } }
		internal int ScrollPageSize { get { return View.InnerVisibleIntervals.Count; } }
		internal virtual TimeInterval VisibleInterval { get { return View.InnerVisibleIntervals.Interval; } }
		internal virtual TimeInterval LimitInterval { get { return View.LimitInterval; } }
		internal virtual TimeScale Scale { get { return ((TimeScaleIntervalCollection)View.InnerVisibleIntervals).Scale; } }
#if !SL && !WPF
		protected override ToolTipLocation DeferredScrollingToolTipLocation { get { return ToolTipLocation.TopCenter; } }
#endif
		#endregion
		protected internal override bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return false;
		}
		protected internal override bool ChangeDateTimeScrollBarVisibilityIfNeeded(DateTimeScrollBar scrollBar) {
#if!SILVERLIGHT&&!WPF
			bool visibilityChanged = scrollBar.Visible != View.DateTimeScrollbarVisible;
			scrollBar.Visible = View.DateTimeScrollbarVisible;
			return visibilityChanged;
#else
			return false;
#endif
		}
		protected internal override void UpdateScrollBarPositionCore(IScrollBarAdapter scrollBarAdapter) {
			this.baseDateTime = VisibleInterval.Start;
			scrollBarAdapter.BeginUpdate();
			try {
				scrollBarAdapter.Minimum = 0;
				scrollBarAdapter.Maximum = CalculateScrollMaximum();
				scrollBarAdapter.SmallChange = 1;
				scrollBarAdapter.LargeChange = CalculateLargetChange();
				scrollBarAdapter.Value = CalculateScrollValue(scrollBarAdapter.Maximum, scrollBarAdapter.LargeChange);
			} finally {
				scrollBarAdapter.EndUpdate();
			}
			scrollBarAdapter.ApplyValuesToScrollBar();
		}
		protected override long CalculateScrollDelta(ScrollEventType eventType, double newValue, TimeInterval scrollInterval, IScrollBarAdapter scrollBarAdapter) {
			long delta = base.CalculateScrollDelta(eventType, newValue, scrollInterval, scrollBarAdapter);
			if (!IsDeferredScrollingEnabled || delta != 0)
				return delta;
			switch (eventType) {
				case ScrollEventType.ThumbPosition:
					PerformScrollInsideRange(scrollInterval, (int)newValue);
					ShowDeferredStart();
					return 0;
				default:
					if (View.InnerVisibleIntervals.Start < scrollInterval.Start)
						PerformScrollInsideRange(scrollInterval, 0);
					if (View.InnerVisibleIntervals.Start > scrollInterval.End)
						PerformScrollInsideRange(scrollInterval, scrollBarAdapter.Maximum - scrollBarAdapter.LargeChange + 1);
					ShowDeferredStart();
					return 0;
			}
		}
		protected internal virtual void PerformScrollInsideRange(TimeInterval scrollInterval, long scrollPosition) {
			DateTime date = PositionToDate((int)scrollPosition);
			SetViewStart(date);
		}
		internal virtual long CalculateScrollMaximum() {
			int scrollMaximum = ScrollPageMultiplier * ScrollPageSize;
			long limitIntervalsCount = CalculateForwardIntervalsCount(LimitInterval.Start, LimitInterval.End, scrollMaximum);
			long largeChange = CalculateLargetChange();
			return limitIntervalsCount < largeChange ? largeChange - 1 : limitIntervalsCount - 1;
		}
		internal virtual int CalculateLargetChange() {
			return ScrollPageSize;
		}
		internal long CalculateScrollValue(long scrollMaximum, long largeChange) {
			long maxValue = Math.Max(0, scrollMaximum - largeChange + 1);
			long scrollCenter = maxValue / 2;
			long availableToStart = CalculateBackwardIntervalsCount(VisibleInterval.Start, LimitInterval.Start, maxValue);
			if (availableToStart < scrollCenter)
				return availableToStart;
			long availableToEnd = CalculateForwardIntervalsCount(VisibleInterval.End, LimitInterval.End, maxValue);
			if (availableToEnd < maxValue - scrollCenter)
				return Math.Max(0, maxValue - availableToEnd);
			return scrollCenter;
		}
		protected internal virtual int GetScrollCenterPosition() {
			return ScrollPageMultiplier / 2 * ScrollPageSize;
		}
		protected internal override int MakeScrollToDelta(long scrollDelta, TimeInterval scrollInterval, ScrollEventType eventType, IScrollBarAdapter scrollBarAdapter) {
			Scroll(scrollBarAdapter, scrollDelta, true);
			return 0;
		}
		protected internal override void Scroll(IScrollBarAdapter scrollBarAdapter, long delta, bool deltaAsLine) {
			DateTime actualStart = GetExpectedViewStart();
			DateTime date = ShiftDate(actualStart, (int)delta);
			scrollBarAdapter.Value += delta;
			scrollBarAdapter.ApplyValuesToScrollBar();
			SetViewStart(date);
		}
		protected internal override void ValidateScrollBarPosition(IScrollBarAdapter scrollBarAdapter, DateTimeScrollEventArgs e) {
			int largeChange = (int)scrollBarAdapter.LargeChange;
			long maxPos = scrollBarAdapter.Maximum - (scrollBarAdapter.LargeChange - 1);
			if (IsMinimumScrollPosition(e.NewValue, scrollBarAdapter.Minimum))
				ValidateBackwardScrollBarPosition(e, largeChange);
			else if (IsMaximumScrollPosition(e.NewValue, maxPos))
				ValidateForwardScrollBarPosition(e, largeChange, maxPos);
		}
		protected virtual bool IsMinimumScrollPosition(double newValue, long minimumPos) {
#if!SILVERLIGHT&&!WPF
			return newValue == minimumPos;
#else
			return newValue <= minimumPos + 1;
#endif
		}
		protected virtual bool IsMaximumScrollPosition(double newValue, long maximumPos) {
#if!SILVERLIGHT&&!WPF
			return newValue == maximumPos;
#else
			return newValue >= maximumPos - 1;
#endif
		}
		internal virtual void ValidateBackwardScrollBarPosition(DateTimeScrollEventArgs e, int largeChange) {
			int offset = (int)CalculateBackwardScrollOffset(largeChange);
			this.baseDateTime = CalculateDateForward(VisibleInterval.Start, GetScrollCenterPosition() - offset);
			e.NewValue = offset;
		}
		internal virtual void ValidateForwardScrollBarPosition(DateTimeScrollEventArgs e, int largeChange, long maxPosition) {
			int offset = (int)CalculateForwardScrollOffset(largeChange);
			this.baseDateTime = CalculateDateBackward(VisibleInterval.Start, GetScrollCenterPosition() - offset);
			e.NewValue = maxPosition - offset;
		}
		internal virtual long CalculateForwardScrollOffset(long largeChange) {
			if (LimitInterval.End.Ticks < VisibleInterval.End.Ticks)
				return 0;
			return CalculateForwardIntervalsCount(VisibleInterval.End, LimitInterval.End, largeChange);
		}
		internal virtual long CalculateBackwardScrollOffset(long largeChange) {
			if (LimitInterval.Start.Ticks > VisibleInterval.Start.Ticks)
				return 0;
			return CalculateBackwardIntervalsCount(VisibleInterval.Start, LimitInterval.Start, largeChange);
		}
		internal virtual long CalculateForwardIntervalsCount(DateTime from, DateTime to, long maxIntervals) {
			long i = 0;
			from = Scale.GetNextDate(from);
			while ((from.Ticks <= to.Ticks) && (i < maxIntervals)) {
				from = Scale.GetNextDate(from);
				i++;
			}
			return i;
		}
		internal virtual long CalculateBackwardIntervalsCount(DateTime from, DateTime to, long maxIntervals) {
			long i = 0;
			from = Scale.GetPrevDate(from);
			while ((from.Ticks >= to.Ticks) && (i < maxIntervals)) {
				from = Scale.GetPrevDate(from);
				i++;
			}
			return i;
		}
		protected internal virtual DateTime PositionToDate(int position) {
			int offset = position - GetScrollCenterPosition();
			return ShiftDate(baseDateTime, offset);
		}
		protected internal virtual DateTime ShiftDate(DateTime date, int offset) {
			if (offset == 0)
				return date;
			return (offset < 0) ? CalculateDateBackward(date, -(int)offset) : CalculateDateForward(date, (int)offset);
		}
		protected internal virtual DateTime CalculateDateBackward(DateTime baseDate, long count) {
			DateTime date = baseDate;
			TimeScale scale = Scale;
			for (long i = 0; i < count; i++) {
				date = scale.GetPrevDate(date);
			}
			return date;
		}
		protected internal virtual DateTime CalculateDateForward(DateTime baseDate, long count) {
			DateTime date = baseDate;
			TimeScale scale = Scale;
			for (long i = 0; i < count; i++)
				date = scale.GetNextDate(date);
			return date;
		}
	}
	#region DateTimeScrollEventHandler
	public delegate void DateTimeScrollEventHandler(object sender, DateTimeScrollEventArgs e);
	#endregion
	#region DateTimeScrollEventArgs
	public class DateTimeScrollEventArgs : EventArgs {
		double newValue;
		ScrollEventType type;
		public DateTimeScrollEventArgs(ScrollEventType type, double newValue) {
			this.type = type;
			this.newValue = newValue;
		}
		public DateTimeScrollEventArgs(ScrollEventType type)
			: this(type, 0) {
		}
		public double NewValue { get { return newValue; } set { newValue = value; } }
		public ScrollEventType Type { get { return type; } }
	}
	#endregion
#if!SILVERLIGHT&&!WPF
	#region ResourceScrollController
	public class ResourceScrollController {
		#region Fields
		Dictionary<NavigatorButtonType, SchedulerCommand> commands;
		SchedulerViewBase view;
		#endregion
		public ResourceScrollController(SchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentException("view", view);
			this.view = view;
			CreateCommands();
		}
		#region Properties
		public SchedulerViewBase View { get { return view; } }
		protected internal Dictionary<NavigatorButtonType, SchedulerCommand> Commands { get { return commands; } }
		#endregion
		protected internal virtual void CreateCommands() {
			this.commands = new Dictionary<NavigatorButtonType, SchedulerCommand>();
			PopulateCommandTable();
		}
		protected internal virtual void PopulateCommandTable() {
			InnerSchedulerControl innerControl = View.Control.InnerControl;
			commands.Add(NavigatorButtonType.Append, new IncrementResourcePerPageCountCommand(innerControl));
			commands.Add(NavigatorButtonType.Remove, new DecrementResourcePerPageCountCommand(innerControl));
			commands.Add(NavigatorButtonType.First, new NavigateFirstResourceCommand(innerControl));
			commands.Add(NavigatorButtonType.PrevPage, new NavigateResourcePageBackwardCommand(innerControl));
			commands.Add(NavigatorButtonType.Prev, new NavigatePrevResourceCommand(innerControl));
			commands.Add(NavigatorButtonType.Next, new NavigateNextResourceCommand(innerControl));
			commands.Add(NavigatorButtonType.NextPage, new NavigateResourcePageForwardCommand(innerControl));
			commands.Add(NavigatorButtonType.Last, new NavigateLastResourceCommand(innerControl));
		}
		protected internal virtual SchedulerCommand LookupCommand(NavigatorButtonType type) {
			SchedulerCommand result;
			if (commands.TryGetValue(type, out result))
				return result;
			else
				return null;
		}
		protected internal virtual bool IsResourceNavigatorActionEnabled(NavigatorButtonType type) {
			SchedulerCommand command = LookupCommand(type);
			if (command != null) {
				DefaultCommandUIState state = new DefaultCommandUIState();
				command.UpdateUIState(state);
				return state.Enabled;
			} else
				return true;
		}
		protected internal virtual void ExecuteResourceNavigatorAction(NavigatorButtonType type) {
			SchedulerCommand command = LookupCommand(type);
			if (command != null)
				command.Execute();
		}
		protected internal virtual void UpdateResourceNavigator(ResourceNavigator resourceNavigator) {
			ScrollBarBase scrollBar = resourceNavigator.NavigatorControl.ScrollBar;
			scrollBar.BeginUpdate();
			try {
				scrollBar.Minimum = 0;
				scrollBar.Maximum = view.FilteredResources.Count - 1;
				scrollBar.SmallChange = 1;
				scrollBar.LargeChange = view.ActualResourcesPerPage;
				scrollBar.Value = view.ActualFirstVisibleResourceIndex;
			} finally {
				scrollBar.EndUpdate();
			}
		}
		protected internal virtual void ResourceNavigatorScrollValueChanged(ResourceNavigator resourceNavigator) {
			ScrollBarBase scrollBar = resourceNavigator.NavigatorControl.ScrollBar;
			view.InnerView.FirstVisibleResourceIndex = scrollBar.Value;
		}
		protected internal void ResourceNavigatorScrollHappend(ScrollEventType type) {
			IAnimationService animationService = view.Control.GetService<IAnimationService>();
			if (animationService == null)
				return;
			if (type == ScrollEventType.ThumbTrack)
				animationService.Lock();
			if (type == ScrollEventType.EndScroll)
				animationService.Unlock();
		}
	}
	#endregion
	public class CellScrollBarsRegistrator : IDisposable {
		#region Fields
		Dictionary<string, int> positions = new Dictionary<string, int>();
		Dictionary<string, ScrollBarBase> scrollBars = new Dictionary<string, ScrollBarBase>();
		SchedulerControl control;
		bool isDisposed;
		#endregion
		public CellScrollBarsRegistrator(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		internal SchedulerControl Control { get { return control; } }
		internal Dictionary<string, int> Positions { get { return positions; } }
		internal Dictionary<string, ScrollBarBase> ScrollBars { get { return scrollBars; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollBars != null) {
					DisposeScrollBars();
					scrollBars = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public void DisposeScrollBars() {
			foreach (ScrollBarBase scrolBar in scrollBars.Values)
				scrolBar.Dispose();
		}
		#endregion
		public virtual void SetScrollPosition(Resource resource, int position) {
			string key = CalculateKey(resource);
			if (Positions.ContainsKey(key))
				Positions[key] = position;
			else {
				Positions.Add(key, position);
			}
		}
		public int GetScrollPosition(Resource resource) {
			string key = CalculateKey(resource);
			return Positions.ContainsKey(key) ? Positions[key] : 0;
		}
		public void ResetScrollBarsVisibility() {
			foreach (ScrollBarBase scrolBar in scrollBars.Values)
				scrolBar.Visible = false;
		}
		public virtual ScrollBarBase GetScrollBar(Resource resource) {
			string key = CalculateKey(resource);
			if (scrollBars.ContainsKey(key))
				return scrollBars[key];
			return CreateScrollBar(key);
		}
		internal virtual string CalculateKey(Resource resource) {
			return resource.Id.ToString();
		}
		internal virtual ScrollBarBase CreateScrollBar(string key) {
			ScrollBarBase scrollBar = new DevExpress.XtraEditors.VScrollBar();
			scrollBars.Add(key, scrollBar);
			control.Controls.Add(scrollBar);
			return scrollBar;
		}
	}
#endif
#if!SILVERLIGHT&&!WPF
	public class ScrollContainerController : IDisposable{
		delegate void CommitInplaceDelegate();
		ScrollBarBase scrollBar;
		SchedulerControl schedulerControl;
		SchedulerViewCellContainer container;
		CellScrollBarsRegistrator registrator;
		int appointmentRowHeight = 0;
		bool isDisposed;
		public ScrollContainerController(SchedulerViewCellContainer container) {
			if (container == null)
				Exceptions.ThrowArgumentNullException("container");
			this.container = container;
		}
		protected internal ScrollBarBase ScrollBar { get { return scrollBar; } }
		protected SchedulerViewCellContainer Container { get { return container; } }
		protected Rectangle ContainerBounds { get { return Container.Bounds; } }
		protected internal int ScrollBarWidth { get { return ScrollBar != null ? scrollBar.Width : 0; } }
		protected internal int AppointmentRowHeight { get { return appointmentRowHeight; } set { appointmentRowHeight = value; } }
		protected internal CellScrollBarsRegistrator Registrator { get { return registrator; } }
		protected internal SchedulerControl SchedulerControl { get { return schedulerControl; } }
		internal bool IsDisposed { get { return isDisposed; } }
		public bool CanScroll { get { return ScrollBar != null; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (scrollBar != null) {
					UnsubscribeScrollbarEvents();
					scrollBar = null;
				}
				registrator = null;
				schedulerControl = null;
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		~ScrollContainerController() {
			Dispose(false);
		}
		public virtual int CalculateScrollOffset() {
			return ScrollBar != null ? ScrollBar.Value * AppointmentRowHeight : 0;
		}
		protected virtual ScrollContainerController CreateCloneInstance() {
			return new ScrollContainerController(Container);
		}
		protected internal virtual void CreateScrollBar(SchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			CellScrollBarsRegistrator registrator = control.CellScrollBarsRegistrator;
			this.scrollBar = registrator.GetScrollBar(Container.Resource);
			this.scrollBar.LookAndFeel.ParentLookAndFeel = control.PaintStyle.UserLookAndFeel;
			this.registrator = registrator;
			this.schedulerControl = control;
		}
		protected internal void UpdateScrollBar(AppointmentViewInfoCollection appointments, Rectangle containerBounds, int aptInterspacing) {
			if (scrollBar == null)
				return;
			UnsubscribeScrollbarEvents();
			ScrollBar.Visible = true;
			ScrollBar.Bounds = CalculateScrollBarBounds(containerBounds);
			this.AppointmentRowHeight = CalculateAppointmentRowHeight(appointments, aptInterspacing);
			UpdateScrollBarPosition(appointments);
			SubscribeScrollbarEvents();
		}
		protected int CalculateActualScrollPosition(int maximum, int visibleRowCount) {
			if (maximum == 0 || visibleRowCount == 0)
				return 0;
			int position = Registrator.GetScrollPosition(container.Resource);
			return NormalizePosition(maximum, visibleRowCount, position);
		}
		protected virtual int NormalizePosition(int maximum, int visibleRowCount, int position) {
			return Math.Min(position, maximum - visibleRowCount + 1);
		}
		protected internal virtual Rectangle CalculateScrollBarBounds(Rectangle containerBounds) {
			Rectangle rect = new Rectangle(containerBounds.Right, containerBounds.Top, ScrollBar.Width, containerBounds.Height);
			return rect;
		}
		protected internal void UpdateScrollBarPosition(AppointmentViewInfoCollection appointments) {
			int appointmentsContentHeight = CalculateContentHeight(appointments);
			bool hasInvisibleApt = appointmentsContentHeight > ContainerBounds.Height;
			int visibleRowCount = hasInvisibleApt ? CalculateScrollPosition(ContainerBounds.Height) : 0;
			int invisibleRowCount = hasInvisibleApt ? CalculateScrollPosition(appointmentsContentHeight - ContainerBounds.Height) : 0;
			int totalRowsCount = invisibleRowCount + visibleRowCount;
			int position = CalculateActualScrollPosition(totalRowsCount, visibleRowCount);
			InitScrollBar(ScrollBar, totalRowsCount, 1, visibleRowCount, position);
		}
		protected internal virtual void SetScrollPosition(int position) {
			scrollBar.Value = position;
			Registrator.SetScrollPosition(container.Resource, position);
		}
		protected internal virtual int CalculateScrollPosition(int y) {
			return AppointmentRowHeight > 0 ? y / AppointmentRowHeight : 0;
		}
		protected internal virtual int CalculateAppointmentRowHeight(AppointmentViewInfoCollection viewInfos, int gapBetweenAppointments) {
			int count = viewInfos.Count;
			if (count == 0)
				return 0;
			int minHeight = viewInfos[0].Bounds.Height;
			for (int i = 1; i < count; i++) {
				Rectangle bounds = viewInfos[i].Bounds;
				minHeight = Math.Min(minHeight, bounds.Height);
			}
			return minHeight > 0 ? minHeight + gapBetweenAppointments : 0;
		}
		protected internal virtual int CalculateContentHeight(AppointmentViewInfoCollection appointments) {
			int contentHeight = 0;
			int top = ContainerBounds.Top;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Rectangle bounds = appointments[i].Bounds;
				contentHeight = Math.Max(contentHeight, bounds.Bottom - top);
			}
			return contentHeight;
		}
		protected void InitScrollBar(ScrollBarBase scrollBar, int maximum, int smallChange, int largeChange, int value) {
			scrollBar.BeginUpdate();
			try {
				scrollBar.Minimum = 0;
				scrollBar.Maximum = maximum;
				scrollBar.SmallChange = smallChange;
				scrollBar.LargeChange = Math.Max(smallChange, largeChange);
				SetScrollPosition(value);
			} finally {
				scrollBar.EndUpdate();
			}
		}
		protected internal virtual void SubscribeScrollbarEvents() {
			ScrollBar.Scroll += new ScrollEventHandler(OnScrollBarScroll);
			ScrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);
		}
		protected void OnScrollBarScroll(object sender, ScrollEventArgs e) {
			ISchedulerStateService service = (ISchedulerStateService)SchedulerControl.GetService(typeof(ISchedulerStateService));
			if (service != null && service.IsInplaceEditorOpened) {
				SchedulerControl.BeginInvoke(new CommitInplaceDelegate(DoCommitInplaceEdit));
			}
		}
		void DoCommitInplaceEdit() {
			SchedulerInplaceEditControllerEx controller = SchedulerControl != null ? SchedulerControl.InplaceEditController : null;
			if (controller == null)
				return;
			try {
				controller.DoCommit();
			} catch {
				controller.DoRollback();
			}
		}
		protected internal virtual void UnsubscribeScrollbarEvents() {
			ScrollBar.Scroll -= new ScrollEventHandler(OnScrollBarScroll);
			ScrollBar.ValueChanged -= new EventHandler(OnScrollBarValueChanged);
		}
		protected virtual void OnScrollBarValueChanged(object sender, EventArgs e) {
			Registrator.SetScrollPosition(container.Resource, ScrollBar.Value);
			Container.RaiseScrollBarValueChanged();
		}
		protected internal void ScrollUp() {
			if (ScrollBar != null)
				SetScrollPosition(0);
		}
		protected internal void ScrollDown() {
			if (ScrollBar != null) {
				int postion = NormalizePosition(ScrollBar.Maximum, ScrollBar.LargeChange, ScrollBar.Maximum);
				SetScrollPosition(postion);
			}
		}
		protected internal bool MakeAppointmentViewInfoVisible(AppointmentViewInfo appointmentViewInfo, AppointmentViewInfoCollection viewInfos) {
			if (ScrollBar == null || appointmentViewInfo == null)
				return false;
			int y = appointmentViewInfo.Bounds.Bottom - ContainerBounds.Top;
			int aptRow = CalculateScrollPosition(y);
			int visibleRowCount = ScrollBar.LargeChange;
			int position = Math.Max(0, aptRow - visibleRowCount);
			int oldPosition = ScrollBar.Value;
			SetScrollPosition(position);
			return oldPosition != scrollBar.Value;
		}
		internal void SetContainer(SchedulerViewCellContainer container) {
			this.container = container;
		}
	}
#endif
}
