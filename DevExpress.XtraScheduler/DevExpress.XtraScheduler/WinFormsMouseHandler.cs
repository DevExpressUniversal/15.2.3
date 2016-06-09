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
using System.Collections;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Commands.Internal;
#if !SILVERLIGHT
using System.Windows.Forms;
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using PlatformIndependentDragAction = System.Windows.Forms.DragAction;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
#else
using PlatformIndependentIDataObject = DevExpress.Utils.IDataObject;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
using PlatformIndependentDragAction = DevExpress.Utils.DragAction;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerMonthViewAutoScrollerHotZone (abstract class)
	public abstract class SchedulerMonthViewAutoScrollerHotZone : SchedulerAutoScrollerHotZone {
		readonly MonthViewInfo viewInfo;
		protected SchedulerMonthViewAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.viewInfo = (MonthViewInfo)mouseHandler.WinControl.ActiveView.ViewInfo;
		}
		public MonthViewInfo ViewInfo { get { return viewInfo; } }
	}
	#endregion
	#region SchedulerDayViewAutoScrollerHotZone (abstract class)
	public abstract class SchedulerDayViewAutoScrollerHotZone : SchedulerAutoScrollerHotZone {
		readonly DayViewInfo viewInfo;
		protected SchedulerDayViewAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.viewInfo = (DayViewInfo)mouseHandler.WinControl.ActiveView.ViewInfo;
		}
		public DayViewInfo ViewInfo { get { return viewInfo; } }
	}
	#endregion
	#region SchedulerDayViewScrollBackwardHotZone
	public class SchedulerDayViewScrollBackwardHotZone : SchedulerDayViewAutoScrollerHotZone {
		public SchedulerDayViewScrollBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			DayViewRowCollection visibleRows = ViewInfo.VisibleRows;
			int count = visibleRows.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle rect = visibleRows[0].Bounds;
			return Rectangle.FromLTRB(rect.Left, int.MinValue / 4, rect.Right, rect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.Y <= bounds.Bottom)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y <= Bounds.Bottom;
		}
		public override void PerformAutoScroll() {
			MouseHandler.ScrollBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	#endregion
	#region SchedulerDayViewScrollForwardHotZone
	public class SchedulerDayViewScrollForwardHotZone : SchedulerDayViewAutoScrollerHotZone {
		public SchedulerDayViewScrollForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			DayViewRowCollection visibleRows = ViewInfo.VisibleRows;
			int count = visibleRows.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle rect = visibleRows[count - 1].Bounds;
			return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, int.MaxValue / 4);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.Y >= bounds.Top)
				return Rectangle.FromLTRB(bounds.Left, mousePosition.Y + 1, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		public override void PerformAutoScroll() {
			MouseHandler.ScrollForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	#endregion
	#region SchedulerTimelineViewAutoScrollerHotZone (abstract class)
	public abstract class SchedulerTimelineViewAutoScrollerHotZone : SchedulerAutoScrollerHotZone {
		readonly TimelineViewInfo viewInfo;
		protected SchedulerTimelineViewAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.viewInfo = (TimelineViewInfo)mouseHandler.WinControl.ActiveView.ViewInfo;
		}
		public TimelineViewInfo ViewInfo { get { return viewInfo; } }
		protected internal virtual SchedulerViewCellBaseCollection GetActiveCells() {
			if (ViewInfo.View.SelectionBar.Visible)
				return ViewInfo.SelectionBar.Cells;
			int count = ViewInfo.CellContainers.Count;
			return count > 0 ? ViewInfo.CellContainers[0].Cells : new SchedulerViewCellBaseCollection();
		}
	}
	#endregion
	#region SchedulerTimelineViewScrollBackwardHotZone
	public class SchedulerTimelineViewScrollBackwardHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		public SchedulerTimelineViewScrollBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle rect = cells[0].Bounds;
			return Rectangle.FromLTRB(int.MinValue / 4, rect.Top, rect.Right, rect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.X <= bounds.Right)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, mousePosition.X - 1, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X <= Bounds.Right;
		}
		public override void PerformAutoScroll() {
			MouseHandler.ScrollBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	#endregion
	#region SchedulerTimelineViewScrollForwardHotZone
	public class SchedulerTimelineViewScrollForwardHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		public SchedulerTimelineViewScrollForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle rect = cells[count - 1].Bounds;
			return Rectangle.FromLTRB(rect.Left, rect.Top, int.MaxValue / 4, rect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.X >= bounds.Left)
				return Rectangle.FromLTRB(mousePosition.X + 1, bounds.Top, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X >= Bounds.Left;
		}
		public override void PerformAutoScroll() {
			MouseHandler.ScrollForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	#endregion
	#region SchedulerAutoScroller
	public class SchedulerAutoScroller : AutoScroller {
		public SchedulerAutoScroller(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new WinFormsSchedulerMouseHandler MouseHandler { get { return (WinFormsSchedulerMouseHandler)base.MouseHandler; } }
		protected override void PopulateHotZones() {
#if !SILVERLIGHT && !WPF
			SchedulerViewInfoBase viewInfo = this.MouseHandler.WinControl.ActiveView.ViewInfo;
			if (viewInfo is DayViewInfo) {
				HotZones.Add(new SchedulerDayViewScrollBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerDayViewScrollForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerDayViewScrollResourceForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerDayViewScrollResourceBackwardHotZone(MouseHandler));
			} else if (viewInfo is MonthViewInfo) {
				HotZones.Add(new SchedulerMonthViewGroupByDateScrollResourceForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerMonthViewGroupByDateScrollResourceBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerMonthViewGroupByResourceBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerMonthViewGroupByResourceForwardHotZone(MouseHandler));
			} else if (viewInfo is TimelineViewInfo) {
				HotZones.Add(new SchedulerTimelineViewScrollBackwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollResourceForwardHotZone(MouseHandler));
				HotZones.Add(new SchedulerTimelineViewScrollResourceBackwardHotZone(MouseHandler));
			}
#endif
		}
	}
	#endregion
	#region SchedulerHotTrackController
	public class SchedulerHotTrackController {
		#region Fields
		readonly SchedulerControl control;
		SelectableIntervalViewInfo hotTrack;
		#endregion
		public SchedulerHotTrackController(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public SelectableIntervalViewInfo HotTrack { get { return hotTrack; } }
		public SchedulerControl Control { get { return control; } }
		#endregion
		public virtual void UpdateHotTrack(ISelectableIntervalViewInfo viewInfo) {
			ResetCurrentHotTrack(viewInfo);
			SetCurrentHotTrack((SelectableIntervalViewInfo)viewInfo);
		}
		protected internal virtual void ResetCurrentHotTrack(ISelectableIntervalViewInfo viewInfo) {
			if (HotTrack == null || Object.ReferenceEquals(HotTrack, viewInfo))
				return;
			UpdateHotTracked(false);
		}
		protected internal virtual void SetCurrentHotTrack(SelectableIntervalViewInfo viewInfo) {
			if (Object.ReferenceEquals(HotTrack, viewInfo))
				return;
			if (viewInfo != null && viewInfo.AllowHotTrack) {
				this.hotTrack = viewInfo;
				UpdateHotTracked(true);
			} else
				this.hotTrack = null;
		}
		protected internal virtual void UpdateHotTracked(bool val) {
			XtraSchedulerDebug.Assert(HotTrack != null);
			HotTrack.HotTrackedInternal = val;
			Control.Invalidate(HotTrack.Bounds);
		}
	}
	#endregion
	public class WinFormsSchedulerMouseHandler : SchedulerMouseHandler {
		readonly SchedulerControl control;
		readonly IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory;
		SchedulerHotTrackController hotTrackController;
		Point lastDragPoint = Point.Empty;
		public WinFormsSchedulerMouseHandler(SchedulerControl control)
			: base(control.InnerControl) {
			this.control = control;
			this.uiFactory = new WinFormsSchedulerMenuBuilderUIFactory();
		}
		internal SchedulerControl WinControl { get { return control; } }
		internal SchedulerViewInfoBase ViewInfo { get { return WinControl.ActiveView.ViewInfo; } }
		internal SchedulerHotTrackController HotTrackController { get { return hotTrackController; } }
		protected internal override ISelectableIntervalViewInfo EmptySelectableIntervalViewInfo { get { return SchedulerHitInfo.None.ViewInfo; } }
		protected internal override IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> UiFactory { get { return uiFactory; } }
		SchedulerOptionsBehavior OptionsBehaviorWin { get { return this.Control.OptionsBehavior as SchedulerOptionsBehavior; } }
		protected override void Dispose(bool disposing) {
			try {
				if (hotTrackController != null) {
					hotTrackController = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		public override void Initialize() {
			this.hotTrackController = CreateHotTrackController(WinControl);
			base.Initialize();
		}
		#region CreateHotTrackController
		protected internal virtual SchedulerHotTrackController CreateHotTrackController(SchedulerControl control) {
			return new SchedulerHotTrackController(control);
		}
		#endregion
		#region UpdateHotTrack
		protected internal override void UpdateHotTrack(ISelectableIntervalViewInfo viewInfo) {
			HotTrackController.UpdateHotTrack(viewInfo);
		}
		#endregion
		protected override AutoScroller CreateAutoScroller() {
			return new SchedulerAutoScroller(this);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return new SchedulerOfficeScroller(WinControl);
		}
		protected override void CalculateAndSaveHitInfo(MouseEventArgs e) {
			CalculateAndSaveHitInfo(new Point(e.X, e.Y));
		}
		protected virtual void CalculateAndSaveHitInfo(Point pt) {
			this.CurrentHitInfo = CalculateHitInfo(pt);
		}
		#region OnDragEnter
		public virtual void OnDragEnter(PlatformIndependentDragEventArgs e) {
			lastDragPoint = Point.Empty;
			Point screenPoint = new Point(e.X, e.Y);
			Point pt = WinControl.PointToClient(screenPoint);
			CalculateAndSaveHitInfo(pt);
			AutoScroller.Resume();
			State.OnDragEnter(e);
		}
		#endregion
		public virtual bool OnDragEnterToExternalControl(SchedulerDragData dragData) {
			return State.OnDragEnterToExternalControl(dragData);
		}
		public virtual bool OnDragDropExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, Point pt, bool copy, bool showPopupMenu) {
			return State.OnDragDropExternalControl(dragData, changedAppointments, pt, copy, showPopupMenu);
		}
		public virtual bool OnDragOverExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, bool copy) {
			return State.OnDragOverExternalControl(dragData, changedAppointments, copy, false);
		}
		public virtual void OnDragLeaveExternalControl() {
			State.OnDragLeaveExternalControl();
		}
		#region OnDragOver
		public virtual void OnDragOver(PlatformIndependentDragEventArgs e) {
			if (Suspended)
				return;
			Point screenPoint = new Point(e.X, e.Y);
			Point pt = WinControl.PointToClient(screenPoint);
			if (pt != lastDragPoint) {
				CalculateAndSaveHitInfo(pt);
				State.OnDragOver(e);
				AutoScroller.OnMouseMove(pt);
				lastDragPoint = pt;
			}
		}
		#endregion
		#region OnDragDrop
		public virtual void OnDragDrop(PlatformIndependentDragEventArgs e) {
			if (Suspended)
				return;
			lastDragPoint = Point.Empty;
			Point screenPoint = new Point(e.X, e.Y);
			Point pt = WinControl.PointToClient(screenPoint);
			CalculateAndSaveHitInfo(pt);
			State.OnDragDrop(e);
		}
		#endregion
		#region OnDragLeave
		public virtual void OnDragLeave(EventArgs e) {
			if (Suspended)
				return;
			lastDragPoint = Point.Empty;
			AutoScroller.Suspend();
			State.OnDragLeave();
		}
		#endregion
		#region OnGiveFeedback
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if (Suspended)
				return;
			State.OnGiveFeedback(e);
		}
		#endregion
		#region OnQueryContinueDrag
		protected internal virtual void OnQueryContinueDrag(PlatformIndependentQueryContinueDragEventArgs e) {
			if (Suspended)
				return;
			State.OnQueryContinueDrag(e);
		}
		#endregion
		#region CalculateHitInfo
		protected internal virtual ISchedulerHitInfo CalculateHitInfo(Point pt) {
			ISchedulerHitInfo hitInfo = ViewInfo.CalculateHitInfo(pt);
			return hitInfo;
		}
		#endregion
		#region OnPopupMenu
		public override bool OnPopupMenu(Point pt) {
			ISchedulerHitInfo hitInfo = CalculateHitInfo(pt);
			return State.OnPopupMenu(pt, hitInfo);
		}
		#endregion
		protected internal void ScrollResourceForward(PlatformIndependentMouseEventArgs e) {
			ISchedulerOfficeScroller officeScroller = OfficeScroller as ISchedulerOfficeScroller;
			WinControl.AnimationManager.SetRestrictions();
			officeScroller.ScrollResource(1);
			WinControl.AnimationManager.Unlock();
		}
		protected internal void ScrollResourceBackward(PlatformIndependentMouseEventArgs e) {
			ISchedulerOfficeScroller officeScroller = OfficeScroller as ISchedulerOfficeScroller;
			WinControl.AnimationManager.SetRestrictions();
			officeScroller.ScrollResource(-1);
			WinControl.AnimationManager.Unlock();
		}
		#region ScrollBackward
		protected internal override void ScrollBackward(PlatformIndependentMouseEventArgs e) {
			if (Suspended)
				return;
			IScrollBarAdapter scrollBarAdapter = WinControl.DateTimeScrollBarObject.ScrollBarAdapter;
			WinControl.DateTimeScrollController.Scroll(scrollBarAdapter, -1);
			OnDateTimeScroll();
		}
		#endregion
		#region ScrollForward
		protected internal override void ScrollForward(PlatformIndependentMouseEventArgs e) {
			if (Suspended)
				return;
			IScrollBarAdapter scrollBarAdapter = WinControl.DateTimeScrollBarObject.ScrollBarAdapter;
			WinControl.DateTimeScrollController.Scroll(scrollBarAdapter, 1);
			OnDateTimeScroll();
		}
		#endregion
		#region OnDateTimeScroll
		public virtual void OnDateTimeScroll() {
			WinControl.InplaceEditController.DoCommit();
			lastDragPoint = Point.Empty;
			Point pt = WinControl.PointToClient(System.Windows.Forms.Control.MousePosition);
			ISchedulerHitInfo hitInfo = CalculateHitInfo(pt);
			State.OnDateTimeScroll(pt, hitInfo);
		}
		#endregion
		protected override void StartOfficeScroller(Point clientPoint) {
			Point screenPoint = WinControl.PointToScreen(clientPoint);
			OfficeScroller.Start(WinControl, screenPoint);
		}
		protected internal override MouseHandlerState CreateDefaultState() {
			return new DefaultMouseHandlerWinState(this);
		}
		protected internal override MouseHandlerState CreateAppointmentDragState(Appointment appointment, ISchedulerHitInfo layoutHitInfo, ISchedulerHitInfo prevLayoutHitInfo) {
			XtraSchedulerDebug.Assert(WinControl.AllowDrop == true);
			if (WinControl.DragDropMode == DragDropMode.Standard) {
#if !SL
				if (RemoteDesktopDetector.IsRemoteSession)
					return new AppointmentRemoteDesktopInternalDragAndDropMouseHandlerState(new SafeAppointment(appointment, Control.Storage), this, layoutHitInfo, prevLayoutHitInfo);
#endif
				return new AppointmentInternalDragAndDropMouseHandlerState(new SafeAppointment(appointment, Control.Storage), this, layoutHitInfo, prevLayoutHitInfo);
			} else
				return new AppointmentMouseDragMouseHandlerState(new SafeAppointment(appointment, Control.Storage), this, layoutHitInfo, prevLayoutHitInfo);
		}
		protected internal override void Redraw() {
			WinControl.Invalidate();
		}
		public override SchedulerPopupMenuBuilder CreateDefaultPopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return new SchedulerDefaultPopupMenuWinBuilder(UiFactory, WinControl, hitInfo);
		}
		protected internal override void SetCurrentCursor(PlatformIndependentCursor cursor) {
			Cursor.Current = cursor;
		}
		protected internal override void UpdateActiveViewSelection(SchedulerViewSelection selection) {
			SchedulerViewInfoBase viewInfo = WinControl.ActiveView.ViewInfo;
			viewInfo.UpdateSelection(selection);
		}
		protected internal override void ApplyChangesOnDragAppointments() {
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
		}
		protected override MouseHandlerState CreateAppointmentResizeMouseHandlerState(Appointment appointment, AppointmentResizingEdge resizingEdge) {
			return new WinAppointmentResizeMouseHandlerState(this, appointment, resizingEdge);
		}
	}
	public class WinAppointmentResizeMouseHandlerState : AppointmentResizeMouseHandlerState {
		public WinAppointmentResizeMouseHandlerState(SchedulerMouseHandler mouseHandler, Appointment appointment, AppointmentResizingEdge appointmentResizingEdge)
			: base(mouseHandler, appointment, appointmentResizingEdge) {
		}
		SchedulerControl PlatformControl { get { return (SchedulerControl)Control.Owner; } }
		protected override void PrepareControlForResize() {
			base.PrepareControlForResize();
			IViewAsyncSupport viewAsyncSupport = PlatformControl.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null)
				return;
			viewAsyncSupport.ForceSyncMode();
		}
		protected override void PrepareControlAfterResize() {
			base.PrepareControlAfterResize();
			IViewAsyncSupport viewAsyncSupport = PlatformControl.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null)
				return;
			viewAsyncSupport.ResetForceSyncMode();
		}		
	}
	public class DefaultMouseHandlerWinState : DefaultMouseHandlerState {
		public DefaultMouseHandlerWinState(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override bool TryToHandleButtonsClick(ISchedulerHitInfo hitInfo) {
			SchedulerHitTest hitTest = hitInfo.HitTest;
			if (hitTest == SchedulerHitTest.MoreButton) {
				OnMoreButtonClick((MoreButton)hitInfo.ViewInfo);
				return true;
			}
			if (hitTest == SchedulerHitTest.ScrollMoreButton) {
				OnScrollMoreButtonClick((ScrollMoreButton)hitInfo.ViewInfo);
				return true;
			}
			if (hitTest == SchedulerHitTest.NavigationButton) {
				OnNavigationButtonClick((NavigationButton)hitInfo.ViewInfo);
				return true;
			}
			return false;
		}
		protected internal virtual void OnMoreButtonClick(MoreButton moreButton) {
			SchedulerCommand command = CreateNavigateMoreButtonCommand(moreButton);
			command.Execute();
		}
		protected internal virtual void OnScrollMoreButtonClick(ScrollMoreButton scrollMoreButton) {
			SchedulerControl control = (SchedulerControl)Control.Owner;
			NavigateScrollMoreButtonCommand command = new NavigateScrollMoreButtonCommand(control, scrollMoreButton.ScrollContainer.ScrollController, scrollMoreButton.GoUp);
			command.Execute();
		}
		protected internal virtual void OnNavigationButtonClick(NavigationButton navigationButton) {
			if (!navigationButton.Enabled)
				return;
			SchedulerControl control = (SchedulerControl)Control.Owner;
			TimeInterval interval = navigationButton.Interval;
			control.BeginUpdate();
			try {
				control.ActiveView.GotoTimeInterval(interval);
			} finally {
				control.EndUpdate();
			}
			SelectAppointmentByInterval(interval, navigationButton.Resource);
		}
		protected internal virtual SchedulerCommand CreateNavigateMoreButtonCommand(MoreButton moreButton) {
			SchedulerControl control = (SchedulerControl)Control.Owner;
			DayView dayView = control.ActiveView as DayView;
			if (dayView == null)
				return new NavigateMoreButtonWinCommand(control, moreButton.Interval, moreButton.Resource, moreButton.TargetViewStart);
			else
				return new NavigateMoreButtonDayViewWinCommand(control, moreButton.Interval, moreButton.Resource, moreButton.TargetViewStart);
		}
		protected internal virtual void SelectAppointmentByInterval(TimeInterval interval, Resource resource) {
			Appointment apt = Control.ActiveView.FilteredAppointments.FindAppointmentExact(interval);
			if (apt != null) {
				SchedulerControl control = (SchedulerControl)Control.Owner;
				control.ActiveView.SelectAppointment(apt, resource);
			}
		}
	}
	public class AppointmentRemoteDesktopInternalDragAndDropMouseHandlerState : AppointmentInternalDragAndDropMouseHandlerState {
		public AppointmentRemoteDesktopInternalDragAndDropMouseHandlerState(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(appointment, mouseHandler, hitInfo, prevHitInfo) {
		}
		protected override bool CanStart(SchedulerHitTest schedulerHitTest) {
			return true;
		}
		protected override PlatformIndependentDragDropEffects DoDragDrop(SchedulerDragData data) {
			AppointmentChangeHelper.UndoChanges();
			return base.DoDragDrop(data);
		}
		public override void OnDragEnter(PlatformIndependentDragEventArgs e) {
			base.OnDragEnter(e);
			AppointmentChangeHelper.UndoChanges();
		}
	}
	#region AppointmentInternalDragAndDropMouseHandlerState
	public class AppointmentInternalDragAndDropMouseHandlerState : AppointmentInternalDragMouseHandlerStateBase {
		#region Fields
		bool rightMouseDrop;
		bool externalDrop;
		const int KeyStateRightMouseButton = 2;
		#endregion
		public AppointmentInternalDragAndDropMouseHandlerState(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(appointment, mouseHandler, hitInfo, prevHitInfo) {
		}
		#region Properties
		protected internal bool RightMouseDrop { get { return rightMouseDrop; } }
		SchedulerControl WinControl { get { return (SchedulerControl)Control.Owner; } }
		#endregion
		public override void StartDragCore(SchedulerDragData data, ISchedulerHitInfo startHitInfo) {
			PlatformIndependentDragDropEffects dropEffect = DoDragDrop(data);
			try {
				AppointmentChangeHelper.CommitDrag(dropEffect, externalDrop);
			} finally {
				MouseHandler.SwitchToDefaultState();
			}
		}
		protected virtual PlatformIndependentDragDropEffects DoDragDrop(SchedulerDragData data) {
#if !SILVERLIGHT && !WPF
			return WinControl.DoDragDrop(new DataObject(data), PlatformIndependentDragDropEffects.All);
#else
			return PlatformIndependentDragDropEffects.None;
#endif
		}
		public override void OnDragOver(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			OnDragCore(e, hitInfo);
		}
		public override void OnDragEnter(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			OnDragCore(e, hitInfo);
		}
		public override bool OnDragEnterToExternalControl(SchedulerDragData dragData) {
			return false;
		}
		public override bool OnDragOverExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, bool copy, bool isDrop) {
			if (dragData == null)
				Exceptions.ThrowArgumentNullException("dragData");
			if (changedAppointments == null)
				Exceptions.ThrowArgumentNullException("changedAppointments");
			if (dragData.Appointments.Count != changedAppointments.Count)
				return false;
			bool result = AppointmentChangeHelper.DragOnExternalControl(dragData, changedAppointments, copy, isDrop);
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			return result;
		}
		public override bool OnDragDropExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, Point pt, bool copy, bool showPopupMenu) {
			externalDrop = true;
			if (changedAppointments == null) {
				return false;
			}
			bool result = OnDragOverExternalControl(dragData, changedAppointments, copy, !showPopupMenu);
			if (showPopupMenu && result) {
#if !SILVERLIGHT && !WPF
				DragPopupMenuInfo.ChangedAppointments.AddRange(AppointmentChangeHelper.GetChangedAppointments());
#endif
				AppointmentChangeHelper.CancelChanges();
#if !SILVERLIGHT && !WPF
				Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
				Point pt2 = WinControl.PointToClient(pt);
				WinControl.OnPopupMenu(pt2);
#endif
				return false;
			} else
				return result;
		}
		public override void OnDragDrop(PlatformIndependentDragEventArgs e) {
			if (rightMouseDrop) {
#if !SILVERLIGHT && !WPF
				DragPopupMenuInfo.ChangedAppointments.AddRange(AppointmentChangeHelper.GetChangedAppointments());
				AppointmentChangeHelper.CancelChanges();
				Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
				Point pt = WinControl.PointToClient(new Point(e.X, e.Y));
				WinControl.OnPopupMenu(pt);
				e.Effect = PlatformIndependentDragDropEffects.None;
#endif
				return;
			}
			if (SchedulerDragData.GetPresent(e.Data)) {
				ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
				DragAppointments(e, hitInfo);
			} else {
				AppointmentChangeHelper.CancelChanges();
				e.Effect = PlatformIndependentDragDropEffects.None;
			}
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		public override void OnDragLeave() {
			AppointmentChangeHelper.UndoChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		public override void OnDragLeaveExternalControl() {
			AppointmentChangeHelper.UndoChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		public override void OnQueryContinueDrag(PlatformIndependentQueryContinueDragEventArgs e) {
			if (e.EscapePressed)
				e.Action = PlatformIndependentDragAction.Cancel;
			if (e.Action == PlatformIndependentDragAction.Continue)
				rightMouseDrop = (e.KeyState & KeyStateRightMouseButton) != 0;
		}
		protected internal override KeyboardHandler CreateKeyboardHandler() {
			return new EmptyKeyboardHandler();
		}
		protected override void PrepareControlForDrag() {
			base.PrepareControlForDrag();
			IViewAsyncSupport viewAsyncSupport = WinControl.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null)
				return;
			viewAsyncSupport.ForceSyncMode();
		}
		protected override void PrepareControlAfterDrag() {
			base.PrepareControlAfterDrag();
			IViewAsyncSupport viewAsyncSupport = WinControl.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null)
				return;
			viewAsyncSupport.ResetForceSyncMode();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public abstract class SchedulerDayViewSlowAutoScrollerHotZone : SchedulerDayViewAutoScrollerHotZone {
		int idleCounter = 0;
		protected SchedulerDayViewSlowAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public sealed override void PerformAutoScroll() {
			this.idleCounter++;
			if (this.idleCounter < 3) {
				return;
			}
			this.idleCounter = 0;
			bool savedForceQueryAppointments = ViewInfo.View.Control.InnerControl.ForceQueryAppointments;
			ViewInfo.View.Control.InnerControl.ForceQueryAppointments = savedForceQueryAppointments;
			try {
				Execute();
			} finally {
				ViewInfo.View.Control.InnerControl.ForceQueryAppointments = true;
			}
		}
		protected abstract void Execute();
	}
	public class SchedulerDayViewScrollResourceBackwardHotZone : SchedulerDayViewSlowAutoScrollerHotZone {
		public SchedulerDayViewScrollResourceBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellContainerCollection containers = ViewInfo.CellContainers;
			if (containers.Count <= 0)
				return Rectangle.Empty;
			SchedulerViewCellContainer lastContainer = containers[0];
			Rectangle rect = lastContainer.Bounds;
			return Rectangle.FromLTRB(int.MinValue / 4, rect.Top, rect.Left + 10, rect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.X <= bounds.Right)
				return Rectangle.FromLTRB(mousePosition.X - 1, bounds.Top, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X < Bounds.Right;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	public class SchedulerDayViewScrollResourceForwardHotZone : SchedulerDayViewSlowAutoScrollerHotZone {
		public SchedulerDayViewScrollResourceForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			int containerCount = ViewInfo.CellContainers.Count;
			if (containerCount <= 0)
				return Rectangle.Empty;
			SchedulerViewCellContainer lastContainer = ViewInfo.CellContainers[containerCount - 1];
			Rectangle rect = lastContainer.Bounds;
			return Rectangle.FromLTRB(rect.Right, rect.Top, int.MaxValue / 4, rect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.X >= bounds.Left)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X >= Bounds.Left;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	public abstract class SchedulerTimelineViewSlowAutoScrollerHotZone : SchedulerTimelineViewAutoScrollerHotZone {
		int idleCounter = 0;
		protected SchedulerTimelineViewSlowAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public sealed override void PerformAutoScroll() {
			this.idleCounter++;
			if (this.idleCounter < 3) {
				return;
			}
			this.idleCounter = 0;
			bool savedForceQueryAppointments = ViewInfo.View.Control.InnerControl.ForceQueryAppointments;
			ViewInfo.View.Control.InnerControl.ForceQueryAppointments = savedForceQueryAppointments;
			try {
				Execute();
			} finally {
				ViewInfo.View.Control.InnerControl.ForceQueryAppointments = true;
			}
		}
		protected abstract void Execute();
	}
	public class SchedulerTimelineViewScrollResourceForwardHotZone : SchedulerTimelineViewSlowAutoScrollerHotZone {
		public SchedulerTimelineViewScrollResourceForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override SchedulerViewCellBaseCollection GetActiveCells() {
			int count = ViewInfo.CellContainers.Count;
			return count > 0 ? ViewInfo.CellContainers[count - 1].Cells : new SchedulerViewCellBaseCollection();
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle firstCellRect = cells[0].Bounds;
			Rectangle lastCellRect = cells[count - 1].Bounds;
			return Rectangle.FromLTRB(firstCellRect.Left, firstCellRect.Bottom, lastCellRect.Right, int.MaxValue / 4);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y >= bounds.Top)
				return Rectangle.FromLTRB(bounds.Left, mousePosition.Y + 1, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	public class SchedulerTimelineViewScrollResourceBackwardHotZone : SchedulerTimelineViewSlowAutoScrollerHotZone {
		public SchedulerTimelineViewScrollResourceBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override SchedulerViewCellBaseCollection GetActiveCells() {
			int count = ViewInfo.CellContainers.Count;
			return count > 0 ? ViewInfo.CellContainers[0].Cells : new SchedulerViewCellBaseCollection();
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle firstCellRect = cells[0].Bounds;
			Rectangle lastCellRect = cells[count - 1].Bounds;
			return Rectangle.FromLTRB(firstCellRect.Left, int.MinValue / 4, lastCellRect.Right, lastCellRect.Bottom);
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y <= bounds.Bottom)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y <= Bounds.Bottom;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	public abstract class SchedulerMonthViewSlowAutoScrollerHotZone : SchedulerMonthViewAutoScrollerHotZone {
		int idleCounter = 0;
		protected SchedulerMonthViewSlowAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public sealed override void PerformAutoScroll() {
			this.idleCounter++;
			if (this.idleCounter < 3) {
				return;
			}
			this.idleCounter = 0;
			bool savedForceQueryAppointments = ViewInfo.View.Control.InnerControl.ForceQueryAppointments;
			ViewInfo.View.Control.InnerControl.ForceQueryAppointments = savedForceQueryAppointments;
			try {
				Execute();
			} finally {
				ViewInfo.View.Control.InnerControl.ForceQueryAppointments = true;
			}
		}
		protected abstract void Execute();
	}
	public abstract class SchedulerMonthViewGroupByResourceSlowAutoScrollerHotZone : SchedulerMonthViewSlowAutoScrollerHotZone {
		protected SchedulerMonthViewGroupByResourceSlowAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return ViewInfo.View.GroupType == SchedulerGroupType.Resource;
		}
	}
	public class SchedulerMonthViewGroupByResourceBackwardHotZone : SchedulerMonthViewGroupByResourceSlowAutoScrollerHotZone {
		public SchedulerMonthViewGroupByResourceBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X <= Bounds.Right;
		}
		protected override Rectangle CalculateHotZoneBounds() {
			int weekCount = ViewInfo.WeekCount;
			Rectangle firstCellRect = ViewInfo.CellContainers[0].Cells[0].Bounds;
			Rectangle lastCellRect = ViewInfo.CellContainers[weekCount - 1].Cells[0].Bounds;
			return Rectangle.FromLTRB(int.MinValue, firstCellRect.Top, lastCellRect.Left, lastCellRect.Bottom);
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.X <= bounds.Right)
				return Rectangle.FromLTRB(mousePosition.X - 1, bounds.Top, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
	}
	public class SchedulerMonthViewGroupByResourceForwardHotZone : SchedulerMonthViewGroupByResourceSlowAutoScrollerHotZone {
		public SchedulerMonthViewGroupByResourceForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			int weekCount = ViewInfo.WeekCount;
			SchedulerViewCellContainerCollection containers = ViewInfo.CellContainers;
			int containerCount = containers.Count;
			SchedulerViewCellBaseCollection firstCells = containers[containerCount - weekCount].Cells;
			Rectangle firstCellRect = firstCells[0].Bounds;
			SchedulerViewCellBaseCollection lastCells = ViewInfo.CellContainers[containerCount - 1].Cells;
			int lastCellCount = lastCells.Count;
			Rectangle lastCellRect = lastCells[lastCellCount - 1].Bounds;
			return Rectangle.FromLTRB(lastCellRect.Right, firstCellRect.Top, int.MaxValue, lastCellRect.Bottom);
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition != Point.Empty && mousePosition.X >= bounds.Left)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.X >= Bounds.Left;
		}
	}
	public abstract class SchedulerMonthViewGroupByDateSlowAutoScrollerHotZone : SchedulerMonthViewSlowAutoScrollerHotZone {
		protected SchedulerMonthViewGroupByDateSlowAutoScrollerHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return ViewInfo.View.GroupType == SchedulerGroupType.Date;
		}
	}
	public class SchedulerMonthViewGroupByDateScrollResourceBackwardHotZone : SchedulerMonthViewGroupByDateSlowAutoScrollerHotZone {
		public SchedulerMonthViewGroupByDateScrollResourceBackwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle firstCellRect = cells[0].Bounds;
			Rectangle lastCellRect = cells[count - 1].Bounds;
			return Rectangle.FromLTRB(firstCellRect.Left, int.MinValue / 4, lastCellRect.Right, lastCellRect.Bottom);
		}
		SchedulerViewCellBaseCollection GetActiveCells() {
			int count = ViewInfo.CellContainers.Count;
			return count > 0 ? ViewInfo.CellContainers[0].Cells : new SchedulerViewCellBaseCollection();
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y <= bounds.Bottom)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y <= Bounds.Bottom;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceBackward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
	public class SchedulerMonthViewGroupByDateScrollResourceForwardHotZone : SchedulerMonthViewGroupByDateSlowAutoScrollerHotZone {
		public SchedulerMonthViewGroupByDateScrollResourceForwardHotZone(WinFormsSchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			SchedulerViewCellBaseCollection cells = GetActiveCells();
			int count = cells.Count;
			if (count == 0)
				return Rectangle.Empty;
			Rectangle firstCellRect = cells[0].Bounds;
			Rectangle lastCellRect = cells[count - 1].Bounds;
			return Rectangle.FromLTRB(firstCellRect.Left, firstCellRect.Bottom, lastCellRect.Right, int.MaxValue / 4);
		}
		SchedulerViewCellBaseCollection GetActiveCells() {
			int count = ViewInfo.CellContainers.Count;
			return count > 0 ? ViewInfo.CellContainers[count - 1].Cells : new SchedulerViewCellBaseCollection();
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y >= bounds.Top)
				return Rectangle.FromLTRB(bounds.Left, mousePosition.Y + 1, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			if (!base.CanActivate(mousePosition))
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		protected override void Execute() {
			WinFormsSchedulerMouseHandler platformMouseHandler = MouseHandler as WinFormsSchedulerMouseHandler;
			platformMouseHandler.ScrollResourceForward(new System.Windows.Forms.MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
		}
	}
}
