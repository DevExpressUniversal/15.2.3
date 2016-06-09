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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
#if !SILVERLIGHT
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
#else
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerAutoScroller
	public class SchedulerAutoScroller : AutoScroller {
		System.Drawing.Point mousePosition;
		public SchedulerAutoScroller(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new XpfSchedulerMouseHandler MouseHandler { get { return (XpfSchedulerMouseHandler)base.MouseHandler; } }
		public System.Drawing.Point MousePosition { get { return mousePosition; } }
		public DevExpress.Xpf.Scheduler.SchedulerControl Control { get { return MouseHandler.XpfControl; } }
		public bool IsActivated { get; set; }
		protected override void PopulateHotZones() {
			ISchedulerViewInfoBase viewInfo = this.MouseHandler.XpfControl.ActiveView.ViewInfo;
			if (viewInfo is DayViewInfoBase) {
				HotZones.Add(new SchedulerDayViewScrollBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerDayViewScrollForwardHotZone(MouseHandler));
			} else if (viewInfo is DevExpress.Xpf.Scheduler.Native.TimelineViewInfoBase) {
				HotZones.Add(new SchedulerTimelineViewScrollBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollResourceBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollResourceForwardHotZone(MouseHandler));
			}
		}
		public override void OnMouseMove(System.Drawing.Point pt) {
			this.mousePosition = pt;
			base.OnMouseMove(pt);
		}
		protected override void StartTimer() {
			base.StartTimer();
			IsActivated = true;
		}
		protected override void StopTimer() {
			base.StopTimer();
			IsActivated = false;
		}
	}
	#endregion
	#region SchedulerDayViewAutoScrollerHotZone (abstract class)
	public abstract class SchedulerDayViewAutoScrollerHotZone : SchedulerAutoScrollerHotZone {
		readonly ScrollViewer scrollViewer;
		const float speedFactor = 2F;
		protected SchedulerDayViewAutoScrollerHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.scrollViewer = ObtainScrollViewer();
		}
		public ScrollViewer ScrollViewer { get { return scrollViewer; } }
		public SchedulerAutoScroller AutoScroller { get { return (SchedulerAutoScroller)MouseHandler.AutoScroller; } }
		protected virtual ScrollViewer ObtainScrollViewer() {
			return ((IVisualDayView)AutoScroller.Control.ActiveView.VisualViewInfo).DayViewScrollViewer;
		}
		protected internal virtual Rect CalculateRelativeBounds() {
			GeneralTransform gt = ScrollViewer.TransformToVisual(AutoScroller.Control);
			return gt.TransformBounds(new Rect(0, 0, ScrollViewer.ViewportWidth, ScrollViewer.ViewportHeight));
		}
		public override void PerformAutoScroll() {
			System.Drawing.Point mousePosition = AutoScroller.MousePosition;
			double delta = CalculateScrollOffset(mousePosition.Y) * speedFactor;
			ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + delta);
			AutoScroller.MouseHandler.OnDateTimeScroll(CreateMouseEventArgs(mousePosition, delta));
		}
		PlatformIndependentMouseEventArgs CreateMouseEventArgs(System.Drawing.Point mousePosition, double delta) {
			return new PlatformIndependentMouseEventArgs(PlatformIndependentMouseButtons.None, 0, mousePosition.X, mousePosition.Y, (int)delta);
		}
		protected abstract double CalculateScrollOffset(int verticalPosition);
	}
	#endregion
	#region SchedulerDayViewScrollBackwardHotZone
	public class SchedulerDayViewScrollBackwardHotZone : SchedulerDayViewAutoScrollerHotZone {
		public SchedulerDayViewScrollBackwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			Rect relativeBounds = CalculateRelativeBounds();
			return new System.Drawing.Rectangle(0, (int)relativeBounds.Top, int.MaxValue, 20);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition != System.Drawing.Point.Empty && mousePosition.Y <= bounds.Bottom)
				return System.Drawing.Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top && mousePosition.Y <= Bounds.Bottom;
		}
		protected override double CalculateScrollOffset(int verticalPosition) {
			return Math.Max(verticalPosition - Bounds.Bottom, -20);
		}
	}
	#endregion
	#region SchedulerDayViewScrollForwardHotZone
	public class SchedulerDayViewScrollForwardHotZone : SchedulerDayViewAutoScrollerHotZone {
		public SchedulerDayViewScrollForwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			Rect relativeBounds = CalculateRelativeBounds();
			return new System.Drawing.Rectangle(0, (int)relativeBounds.Bottom - 20, int.MaxValue, 20);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition != System.Drawing.Point.Empty && mousePosition.Y >= bounds.Top)
				return System.Drawing.Rectangle.FromLTRB(bounds.Left, mousePosition.Y + 1, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		protected override double CalculateScrollOffset(int verticalPosition) {
			return Math.Min(verticalPosition - Bounds.Top, 20);
		}
	}
	#endregion
	#region SchedulerTimelineViewAutoScrollerHotZone (abstract class)
	public abstract class SchedulerTimelineViewAutoScrollerHotZone : SchedulerAutoScrollerHotZone {
		readonly DevExpress.Xpf.Scheduler.Native.TimelineViewInfoBase viewInfo;
		protected SchedulerTimelineViewAutoScrollerHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.viewInfo = (DevExpress.Xpf.Scheduler.Native.TimelineViewInfoBase)mouseHandler.XpfControl.ActiveView.ViewInfo;
		}
		public DevExpress.Xpf.Scheduler.Native.TimelineViewInfoBase ViewInfo { get { return viewInfo; } }
	}
	#endregion
	#region SchedulerTimelineViewScrollBackwardHotZone
	public class SchedulerTimelineViewScrollBackwardHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		public SchedulerTimelineViewScrollBackwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			return new System.Drawing.Rectangle(20, 0, 20, int.MaxValue);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition.X <= bounds.Right)
				return System.Drawing.Rectangle.FromLTRB(bounds.Left, bounds.Top, mousePosition.X - 1, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X <= Bounds.Right;
		}
		public override void PerformAutoScroll() {
			Point pt = ((XpfSchedulerMouseHandler)MouseHandler).LastMousePoint;
			MouseHandler.ScrollBackward(new PlatformIndependentMouseEventArgs(PlatformIndependentMouseButtons.None, 0, (int)(pt.X), (int)(pt.Y), 0));
		}
	}
	#endregion
	#region SchedulerTimelineViewScrollForwardHotZone
	public class SchedulerTimelineViewScrollForwardHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		public SchedulerTimelineViewScrollForwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			return new System.Drawing.Rectangle((int)ViewInfo.Control.ActualWidth - 20, 0, 20, int.MaxValue);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition.X >= bounds.Left)
				return System.Drawing.Rectangle.FromLTRB(mousePosition.X + 1, bounds.Top, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X >= Bounds.Left;
		}
		public override void PerformAutoScroll() {
			Point pt = ((XpfSchedulerMouseHandler)MouseHandler).LastMousePoint;
			MouseHandler.ScrollForward(new PlatformIndependentMouseEventArgs(PlatformIndependentMouseButtons.None, 0, (int)(pt.X), (int)(pt.Y), 0));
		}
	}
	#endregion
	public abstract class SchedulerTimelineViewSlowAutoScrollerHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		int idleCounter = 0;
		protected SchedulerTimelineViewSlowAutoScrollerHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public sealed override void PerformAutoScroll() {
			this.idleCounter++;
			if (this.idleCounter < 3) {
				return;
			}
			this.idleCounter = 0;
			Execute();
		}
		protected abstract void Execute();
	}
	public class SchedulerTimelineViewScrollResourceBackwardHotZone : SchedulerTimelineViewSlowAutoScrollerHotZone {
		public SchedulerTimelineViewScrollResourceBackwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			Rect hotZoneBounds = ViewInfo.Control.HotZoneBounds;
			return System.Drawing.Rectangle.FromLTRB(0, int.MinValue / 4, (int)ViewInfo.Control.ActualWidth, (int)hotZoneBounds.Top);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition.Y <= bounds.Bottom)
				return System.Drawing.Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y + 1);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y <= Bounds.Bottom;
		}
		protected override void Execute() {
			XpfSchedulerMouseHandler platformMouseHandler = (XpfSchedulerMouseHandler)MouseHandler;
			platformMouseHandler.ScrollResourceBackward();
		}
	}
	public class SchedulerTimelineViewScrollResourceForwardHotZone : SchedulerTimelineViewSlowAutoScrollerHotZone {
		public SchedulerTimelineViewScrollResourceForwardHotZone(XpfSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override System.Drawing.Rectangle CalculateHotZoneBounds() {
			Rect hotZoneBounds = ViewInfo.Control.HotZoneBounds;
			return System.Drawing.Rectangle.FromLTRB((int)hotZoneBounds.Left, (int)hotZoneBounds.Bottom, (int)hotZoneBounds.Right, int.MaxValue / 4);
		}
		protected override System.Drawing.Rectangle AdjustHotZoneBounds(System.Drawing.Rectangle bounds, System.Drawing.Point mousePosition) {
			if (mousePosition.Y >= bounds.Top)
				return System.Drawing.Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(System.Drawing.Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		protected override void Execute() {
			XpfSchedulerMouseHandler platformMouseHandler = (XpfSchedulerMouseHandler)MouseHandler;
			platformMouseHandler.ScrollResourceForward();
		}
	}
}
