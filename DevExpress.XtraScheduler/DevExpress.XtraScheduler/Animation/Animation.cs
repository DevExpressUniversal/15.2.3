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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Services;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using DevExpress.XtraScheduler.Services.Internal;
using System.Collections.Generic;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraScheduler.Animation.Native;
namespace DevExpress.XtraScheduler.Native {
	#region AnimationJob
	public class AnimationJob : IDisposable {
#if DEBUGTEST
		static int AnimationDelay = (DevExpress.XtraScheduler.Tests.TestEnvironment.AnimationDelay.HasValue) ? DevExpress.XtraScheduler.Tests.TestEnvironment.AnimationDelay.Value : 300;
#else
		const int AnimationDelay = 300;
#endif
		DateTime startTime;
		TimeSpan duration = TimeSpan.FromMilliseconds(AnimationDelay);
		IServiceContainer serviceContainer;
		Timer timer;
		AnimationEffect animation;
		IKeyboardHandlerService keyboardHandlerService;
		IMouseHandlerService mouseHandlerService;
		public AnimationJob(IServiceContainer serviceContainer, AnimationEffect animation) {
			this.serviceContainer = serviceContainer;
			this.animation = animation;
			IsFreeze = false;
		}
		#region Properties
		public DateTime StartTime { get { return startTime; } }
		public TimeSpan Duration { get { return duration; } set { duration = value; } }
		public IServiceContainer ServiceContainer { get { return serviceContainer; } }
		public bool IsFreeze { get; set; }
		#endregion
		#region Events
		#region Complete
		public event EventHandler Complete;
		void RaiseComplete() {
			if (Complete == null)
				return;
			Complete(this, EventArgs.Empty);
		}
		#endregion
		#region Repaint
		public event EventHandler Repaint;
		void RaiseRepaint() {
			if (Repaint == null)
				return;
			Repaint(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		public void Start() {
			SetAnimationState(true);
			SuspendUserInput();
			this.startTime = DateTime.Now;
			this.timer = new Timer();
			this.timer.Tick += OnTimerTick;
			this.timer.Interval = 10;
			this.timer.Start();
		}	 
		void SetAnimationState(bool isAnimationActive) {
			ISetSchedulerStateService service = ServiceContainer.GetService(typeof(ISetSchedulerStateService)) as ISetSchedulerStateService;
			if (service == null)
				return;
			service.IsAnimationActive = isAnimationActive;
		}
		void SuspendUserInput() {
			IServiceContainer container = serviceContainer as IServiceContainer;
			if (container == null)
				return;
			this.mouseHandlerService = container.GetService(typeof(IMouseHandlerService)) as IMouseHandlerService;
			if (mouseHandlerService != null) {
				container.RemoveService(typeof(IMouseHandlerService));
			}
			this.keyboardHandlerService = container.GetService(typeof(IKeyboardHandlerService)) as IKeyboardHandlerService;
			if (this.keyboardHandlerService != null) {
				container.RemoveService(typeof(IKeyboardHandlerService));
				container.AddService(typeof(IKeyboardHandlerService), new AnimationKeyboardService(this.keyboardHandlerService, this));
			}		   
		}
		void ResumeUserInput() {
			IServiceContainer container = serviceContainer as IServiceContainer;
			if (container == null)
				return;
			if (this.keyboardHandlerService != null) {
				container.RemoveService(typeof(IKeyboardHandlerService));
				container.AddService(typeof(IKeyboardHandlerService), this.keyboardHandlerService);
			}
			ResumeMouseOnlyInput();
			this.keyboardHandlerService = null;
		}
		void ResumeMouseOnlyInput() {
			IServiceContainer container = serviceContainer as IServiceContainer;
			if (container == null)
				return;
			if (this.mouseHandlerService != null)
				container.AddService(typeof(IMouseHandlerService), this.mouseHandlerService);
			this.mouseHandlerService = null;
		}
		void OnTimerTick(object sender, EventArgs e) {
			if (IsFreeze)
				return;
			UpdateAnimationFrame();
		}
		private void UpdateAnimationFrame() {
			DateTime now = DateTime.Now;
			if (now > StartTime + Duration)
				Stop();
			RaiseRepaint();
		}
		public bool OnPaint(PaintEventArgs e) {
			if (IsFreeze)
				return false;
			DateTime now = DateTime.Now;
			float t = (now - StartTime).Ticks / (float)Duration.Ticks;
			t = Math.Min(1.0f, Math.Max(0.0f, t));
			OnPaintCore(e.Graphics, t);
			return true;
		}
		public void OnPaintCore(Graphics gr, float t) {
			animation.OnPaint(gr, t);
		}
		public void Stop() {
			DisposeTimer();
			ResumeUserInput();
			SetAnimationState(false);
			RaiseComplete();
		}
		private void DisposeTimer() {
			if (this.timer == null)
				return;
			this.timer.Stop();
			this.timer.Tick -= OnTimerTick;
			this.timer.Dispose();
			this.timer = null;
		}
		public void Freese() {
			IsFreeze = true;
			DisposeTimer();
			RaiseRepaint();
			ResumeMouseOnlyInput();
		}
		#region IDisposable implementation
		public void Dispose() {
			if (this.timer != null) {
				this.timer.Dispose();
				this.timer = null;
			}
		}
		#endregion
	}
	#endregion
	#region SchedulerAnimationInfo
	public class SchedulerAnimationInfo {
		public static SchedulerAnimationInfo Create(SchedulerControl scheduler) {
			switch (scheduler.ActiveViewType) {
				case SchedulerViewType.Day:
					return new DayViewAnimationInfo(scheduler.DayView);
				case SchedulerViewType.WorkWeek:
					return new WorkWeekViewAnimationInfo(scheduler.WorkWeekView);
				case SchedulerViewType.Week:
					return new WeekViewAnimationInfo(scheduler.WeekView);
				case SchedulerViewType.Month:
					return new MonthViewAnimationInfo(scheduler.MonthView);
				case SchedulerViewType.Timeline:
					return new TimelineViewAnimationInfo(scheduler.TimelineView);
				case SchedulerViewType.Gantt:
					return new GanttViewAnimationInfo(scheduler.GanttView);
				default:
					return new SchedulerAnimationInfo(scheduler);
			}
		}
		Size controlSize;
		SchedulerViewType viewType = (SchedulerViewType)(-1);
		TimeInterval interval;
		Bitmap bitmap;
		SchedulerGroupType groupType;
		int firstVisibleResourceIndex;
		int resourcesPerPage;
		public SchedulerAnimationInfo(SchedulerControl scheduler) {
			ViewType = scheduler.ActiveViewType;
			Interval = scheduler.ActiveView.GetVisibleIntervals().Interval.Clone();
			ControlSize = scheduler.Size;
			GroupType = scheduler.GroupType;
			FirstVisibleResourceIndex = scheduler.ActiveView.FirstVisibleResourceIndex;
			ResourcesPerPage = scheduler.ActiveView.ResourcesPerPage;
		}
		public int FirstVisibleResourceIndex { get { return firstVisibleResourceIndex; } set { firstVisibleResourceIndex = value; } }
		public int ResourcesPerPage { get { return resourcesPerPage; } set { resourcesPerPage = value; } }
		public Size ControlSize { get { return controlSize; } set { controlSize = value; } }
		public SchedulerViewType ViewType { get { return viewType; } set { viewType = value; } }
		public TimeInterval Interval { get { return interval; } set { interval = value; } }
		public Bitmap Bitmap { get { return bitmap; } set { bitmap = value; } }
		public SchedulerGroupType GroupType { get { return groupType; } set { groupType = value; } }
	}
	#endregion
	public abstract class SchedulerViewAnimationInfo<T> : SchedulerAnimationInfo where T : SchedulerViewBase {
		protected SchedulerViewAnimationInfo(T view)
			: base(view.Control) {
			Initialize(view);
		}
		protected abstract void Initialize(T view);
	}
	public class DayViewAnimationInfo : SchedulerViewAnimationInfo<DayView> {
		int dayCount;
		bool showAllDayArea;
		bool showDayHeaders;
		bool showWorkTimeOnly;
		public DayViewAnimationInfo(DayView view)
			: base(view) {
		}
		public int DayCount { get { return dayCount; } set { dayCount = value; } }
		public bool ShowAllDayArea { get { return showAllDayArea; } set { showAllDayArea = value; } }
		public bool ShowDayHeaders { get { return showDayHeaders; } set { showDayHeaders = value; } }
		public bool ShowWorkTimeOnly { get { return showWorkTimeOnly; } set { showWorkTimeOnly = value; } }
		protected override void Initialize(DayView view) {
			DayCount = view.DayCount;
			ShowAllDayArea = view.ShowAllDayArea;
			ShowDayHeaders = view.ShowDayHeaders;
			ShowWorkTimeOnly = view.ShowWorkTimeOnly;
		}
	}
	public class WorkWeekViewAnimationInfo : DayViewAnimationInfo {
		WeekDays days;
		public WorkWeekViewAnimationInfo(WorkWeekView view)
			: base(view) {
		}
		public WeekDays Days { get { return days; } set { days = value; } }
		protected override void Initialize(DayView view) {
			WorkWeekView workWeekView = (WorkWeekView)view;
			Days = workWeekView.Control.WorkDays.GetWeekDays();
		}
	}
	public class WeekViewAnimationInfo : SchedulerViewAnimationInfo<WeekView> {
		public WeekViewAnimationInfo(WeekView view)
			: base(view) {
		}
		protected override void Initialize(WeekView view) {
		}
	}
	public class MonthViewAnimationInfo : WeekViewAnimationInfo {
		int weekCount;
		bool compressWeekend;
		public MonthViewAnimationInfo(MonthView view)
			: base(view) {
		}
		public int WeekCount { get { return weekCount; } set { weekCount = value; } }
		public bool CompressWeekend { get { return compressWeekend; } set { compressWeekend = value; } }
		protected override void Initialize(WeekView view) {
			MonthView monthView = (MonthView)view;
			WeekCount = monthView.WeekCount;
			CompressWeekend = monthView.CompressWeekend;
		}
	}
	public class TimelineViewAnimationInfo : SchedulerViewAnimationInfo<TimelineView> {
		public TimelineViewAnimationInfo(TimelineView view)
			: base(view) {
		}
		protected override void Initialize(TimelineView view) {
		}
	}
	public class GanttViewAnimationInfo : TimelineViewAnimationInfo {
		public GanttViewAnimationInfo(TimelineView view)
			: base(view) {
		}
		protected override void Initialize(TimelineView view) {
		}
	}
	public static class AnimationDrawHelper {
		public static void DrawImageWithTransparency(Graphics gr, Image image, float alpha, int x, int y, Rectangle scrollBounds) {
			Point[] points = new Point[] { new Point(x, y), new Point(x + scrollBounds.Width, y), new Point(x, y + scrollBounds.Height) };
			using (ImageAttributes currentAttributes = new ImageAttributes()) {
				ColorMatrix currentColorMatrix = new ColorMatrix();
				currentColorMatrix.Matrix33 = alpha;
				currentAttributes.SetColorMatrix(currentColorMatrix);
				gr.DrawImage(image, points, scrollBounds, GraphicsUnit.Pixel, currentAttributes);
			}
		}
	}
}
