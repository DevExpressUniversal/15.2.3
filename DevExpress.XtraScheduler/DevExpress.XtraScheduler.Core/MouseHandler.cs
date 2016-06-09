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
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Commands.Internal;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Services.Implementation;
#if !SILVERLIGHT && !WPF
#else
using System.Windows.Media;
using System.Windows.Controls;
#endif
#if !SILVERLIGHT
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using PlatformIndependentDragAction = System.Windows.Forms.DragAction;
using PlatformIndependentSystemInformation = System.Windows.Forms.SystemInformation;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformIndependentCursors = System.Windows.Forms.Cursors;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
#else
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
using PlatformIndependentDragAction = DevExpress.Utils.DragAction;
using PlatformIndependentSystemInformation = DevExpress.Utils.SystemInformation;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
using PlatformIndependentCursors = System.Windows.Input.Cursors;
using DevExpress.Data;
#endif
#if WPF
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.UI;
#endif
namespace DevExpress.XtraScheduler.Native {
	public static class CursorTypes {
		public static PlatformIndependentCursor Default { get { return defaultCursor; } }
		public static PlatformIndependentCursor SizeAll { get { return sizeAll; } }
#if SILVERLIGHT
		static readonly PlatformIndependentCursor defaultCursor = PlatformIndependentCursors.Arrow;
		static readonly PlatformIndependentCursor sizeAll = PlatformIndependentCursors.Hand;
#else
		static readonly PlatformIndependentCursor defaultCursor = System.Windows.Forms.Cursors.Default;
		static readonly PlatformIndependentCursor sizeAll = System.Windows.Forms.Cursors.SizeAll;
#endif
	}
	#region SchedulerAutoScrollerHotZone (abstract class)
	public abstract class SchedulerAutoScrollerHotZone : AutoScrollerHotZone {
		protected const int MinHotZoneSize = 10;
		readonly SchedulerMouseHandler mouseHandler;
		protected SchedulerAutoScrollerHotZone(SchedulerMouseHandler mouseHandler) {
			Guard.ArgumentNotNull(mouseHandler, "mouseHandler");
			this.mouseHandler = mouseHandler;
		}
		public SchedulerMouseHandler MouseHandler { get { return mouseHandler; } }
		public override bool CanActivate(Point mousePosition) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			return !hitInfo.Contains(SchedulerHitTest.AllDayArea | SchedulerHitTest.ResourceHeader | SchedulerHitTest.DayHeader);
		}
	}
	#endregion
	public class EmptyAutoScroller : AutoScroller {
		public EmptyAutoScroller(MouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override void PopulateHotZones() {
		}
	}
	#region SchedulerMouseHandler (abstract class)
	public abstract class SchedulerMouseHandler : MouseHandler {
		#region Fields
		readonly InnerSchedulerControl control;
		ISelectableIntervalViewInfo clickedViewInfo;
		AppointmentOperationHelper appointmentOperationHelper;
		#endregion
		protected SchedulerMouseHandler(InnerSchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public new SchedulerMouseHandlerState State { get { return (SchedulerMouseHandlerState)base.State; } }
		internal InnerSchedulerControl Control { get { return control; } }
		internal ISelectableIntervalViewInfo ClickedViewInfo { get { return clickedViewInfo; } set { clickedViewInfo = value; } }
		internal virtual bool CanShowToolTip { get { return State.CanShowToolTip; } }
		public AppointmentOperationHelper AppointmentOperationHelper { get { return appointmentOperationHelper; } }
		protected internal abstract ISelectableIntervalViewInfo EmptySelectableIntervalViewInfo { get; }
		protected internal abstract IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> UiFactory { get; }
		#endregion
		public override void Initialize() {
			this.appointmentOperationHelper = new AppointmentOperationHelper(Control);
			base.Initialize();
			clickedViewInfo = EmptySelectableIntervalViewInfo;
		}
		ISchedulerHitInfo _currentHitInfo;
		internal ISchedulerHitInfo CurrentHitInfo { get { return _currentHitInfo; } set { _currentHitInfo = value; } }
		protected override AutoScroller CreateAutoScroller() {
			return new EmptyAutoScroller(this);
		}
		#region HandleMouseWheel
		protected override void HandleMouseWheel(PlatformIndependentMouseEventArgs e) {
			if (IsControlPressed)
				PerformWheelZoom(e);
			else
				PerformWheelScroll(e);
		}
		#endregion
		void PerformWheelDateTimeScroll(PlatformIndependentMouseEventArgs e, bool isHorizontal) {
			int delta = (isHorizontal) ? -e.Delta : e.Delta;
			if (delta == 0)
				return;
			if (delta > 0)
				ScrollBackward(e);
			else
				ScrollForward(e);
		}
		protected internal virtual void PerformWheelScroll(PlatformIndependentMouseEventArgs e) {
#if SL
			PerformWheelDateTimeScroll(e, false);
#else
			OfficeMouseWheelEventArgs ev = e as OfficeMouseWheelEventArgs;
			bool enableAutoWheelScroll = ev != null && this.Control.OptionsBehavior.InnerMouseWheelScrollAction == MouseWheelScrollAction.Auto;
			if (!enableAutoWheelScroll) {
				bool isHorizontal = ev != null && ev.IsHorizontal;
				PerformWheelDateTimeScroll(e, isHorizontal);
				return;
			}
			PerformTwoDirectionsWheelScroll(ev);
#endif
		}
		void PerformTwoDirectionsWheelScroll(OfficeMouseWheelEventArgs ev) {
			ISchedulerOfficeScroller schedulerScroller = OfficeScroller as ISchedulerOfficeScroller;
			if (schedulerScroller == null)
				return;
			if (ev.IsHorizontal)
				schedulerScroller.ScrollHorizontal(ev.Delta);
			else
				schedulerScroller.ScrollVertical(-ev.Delta);
		}
		protected override bool IsDoubleClick(PlatformIndependentMouseEventArgs e) {
			return base.IsDoubleClick(e) &&
				IsSelectableIntervalViewInfoEquals(CurrentHitInfo.ViewInfo, this.clickedViewInfo) &&
				clickedViewInfo != EmptySelectableIntervalViewInfo;
		}
		protected virtual bool IsSelectableIntervalViewInfoEquals(ISelectableIntervalViewInfo info1, ISelectableIntervalViewInfo info2) {
			return info1.Equals(info2);
		}
		protected override void HandleMouseDoubleClick(PlatformIndependentMouseEventArgs e) {
			if (AppointmentDependencyDoubleClick(CurrentHitInfo))
				return;
			if (AppointmentDoubleClick(CurrentHitInfo))
				return;
			if (AllDayAreaDoubleClick(CurrentHitInfo))
				return;
			if (CellDoubleClick(CurrentHitInfo))
				return;
		}
		protected override void HandleMouseTripleClick(PlatformIndependentMouseEventArgs e) {
		}
		protected override void HandleMouseDown(PlatformIndependentMouseEventArgs e) {
			this.clickedViewInfo = CurrentHitInfo.ViewInfo;
			base.HandleMouseDown(e);
		}
		protected override void HandleClickTimerTick() {
			ISelectableIntervalViewInfo clickedViewInfo = ClickedViewInfo;
			StopClickTimer();
			if (Suspended)
				return;
			this.clickedViewInfo = clickedViewInfo;
			State.OnLongMouseDown();
			this.clickedViewInfo = null;
		}
		protected internal virtual bool AppointmentDependencyDoubleClick(ISchedulerHitInfo hitInfo) {
			if (hitInfo.HitTest != SchedulerHitTest.AppointmentDependency)
				return false;
			if (Control.AppointmentDependencySelectionController.SelectedDependencies.Count > 1)
				return false;
			IAppointmentDependencyView depViewInfo = hitInfo.ViewInfo as IAppointmentDependencyView;
			if (depViewInfo == null)
				return false;
			if (depViewInfo.Dependencies.Count == 0)
				return false;
			EditAppointmentDependencyCommand command = new EditAppointmentDependencyCommand(control);
			command.CommandSourceType = CommandSourceType.Mouse;
			command.Execute(depViewInfo.Dependencies[0]);
			return true;
		}
		#region AppointmentDoubleClick
		protected internal virtual bool AppointmentDoubleClick(ISchedulerHitInfo hitInfo) {
			IAppointmentView aptViewInfo = hitInfo.ViewInfo as IAppointmentView;
			if (aptViewInfo == null)
				return false;
			EditAppointmentCommand command = CreateEditAppointmentCommand();
			command.CommandSourceType = CommandSourceType.Mouse;
			command.Execute(aptViewInfo.Appointment);
			return true;
		}
		#endregion
		#region AllDayAreaDoubleClick
		protected internal virtual bool AllDayAreaDoubleClick(ISchedulerHitInfo hitInfo) {
			if (hitInfo.HitTest != SchedulerHitTest.AllDayArea)
				return false;
			NewAllDayAppointmentCommand command = new NewAllDayAppointmentCommand(control);
			command.CommandSourceType = CommandSourceType.Mouse;
			command.Execute();
			return true;
		}
		#endregion
		#region CellDoubleClick
		protected internal virtual bool CellDoubleClick(ISchedulerHitInfo hitInfo) {
			if (hitInfo.HitTest != SchedulerHitTest.Cell)
				return false;
			TimeInterval interval = hitInfo.ViewInfo.Interval;
			TimeInterval alignedInterval = new TimeInterval(DateTimeHelper.Floor(interval.Start, DateTimeHelper.DaySpan), DateTimeHelper.Ceil(interval.End, DateTimeHelper.DaySpan));
			NewAppointmentCommandBase command;
			if (alignedInterval.Equals(interval))
				command = new NewAllDayAppointmentCommand(control);
			else
				command = new NewAppointmentCommand(Control);
			command.CommandSourceType = CommandSourceType.Mouse;
			command.Execute();
			return true;
		}
		#endregion
		protected internal virtual void PerformWheelZoom(PlatformIndependentMouseEventArgs e) {
			SchedulerCommand command = CreateZoomCommand(e);
			command.Execute();
		}
		protected internal virtual SchedulerCommand CreateZoomCommand(PlatformIndependentMouseEventArgs e) {
			if (e.Delta > 0)
				return Control.CreateCommand(SchedulerCommandId.ViewZoomIn);
			else
				return Control.CreateCommand(SchedulerCommandId.ViewZoomOut);
		}
		#region CanShowPopupMenu
		public virtual bool OnPopupMenuShowing() {
			if (Suspended)
				return false;
			return State.OnPopupMenuShowing();
		}
		#endregion
		#region CreatePopupMenuBuilder
		public virtual SchedulerPopupMenuBuilder CreatePopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return State.CreatePopupMenuBuilder(hitInfo);
		}
		#endregion
		#region SwitchToDefaultState
		public override void SwitchToDefaultState() {
			MouseHandlerState newState = CreateDefaultState();
			SwitchStateCore(newState, Point.Empty);
		}
		#endregion
		#region SwitchToCellSelectionState
		protected internal virtual void SwitchToCellSelectionState(Point mousePosition, ISchedulerHitInfo hitInfo, PlatformIndependentMouseButtons button) {
			MouseHandlerState newState = new CellSelectionMouseHandlerState(this, hitInfo, button);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		#region SwitchToAppointmentResizeEdgeMouseDownState
		protected internal virtual void SwitchToAppointmentResizeEdgeMouseDownState(Point mousePosition, Appointment appointment, ISchedulerHitInfo hitInfo) {
			MouseHandlerState newState = new AppointmentResizeEdgeMouseDownState(this, mousePosition, appointment, hitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		protected internal virtual void SwitchToAppointmentResizeState(Point mousePosition, Appointment appointment, AppointmentResizingEdge resizingEdge) {
			MouseHandlerState newState = CreateAppointmentResizeMouseHandlerState(appointment, resizingEdge);
			if (!Control.AppointmentSelectionController.IsAppointmentSelected(appointment))
				return;
			SwitchStateCore(newState, mousePosition);
		}
		protected virtual MouseHandlerState CreateAppointmentResizeMouseHandlerState(Appointment appointment, AppointmentResizingEdge resizingEdge) {
			return new AppointmentResizeMouseHandlerState(this, appointment, resizingEdge);	
		}		
		#region SwitchToAppointmentMouseDownState
		protected internal virtual void SwitchToAppointmentMouseDownState(Point mousePosition, Appointment appointment, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo) {
			MouseHandlerState newState = CreateAppointmentMouseDownState(mousePosition, appointment, mouseButtons, hitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		#region CreateAppointmentMouseDownState
		protected virtual AppointmentMouseDownState CreateAppointmentMouseDownState(Point mousePosition, Appointment appointment, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo) {
			return new AppointmentMouseDownState(this, appointment, mousePosition, mouseButtons, hitInfo);
		}
		#endregion
		#region SwitchToExternalDragAndDropState
		protected internal virtual void SwitchToExternalDragAndDropState(Point mousePosition, ISchedulerHitInfo layoutHitInfo, PlatformIndependentDragEventArgs dragEventArgs) {
			MouseHandlerState newState = new AppointmentExternalDragAndDropMouseHandlerState(dragEventArgs, this, layoutHitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		protected internal void SwitchToExternalDragAndDropOnExternalControlState(SchedulerDragData dragData) {
			MouseHandlerState newState = new AppointmentExternalDragAndDropOnExternalControlMouseHandlerState(dragData, this);
			SwitchStateCore(newState, Point.Empty);
		}
		#region SwitchToInternalDragState
		protected internal virtual void SwitchToInternalDragState(Point mousePosition, Appointment appointment, ISchedulerHitInfo layoutHitInfo, ISchedulerHitInfo prevLayoutHitInfo) {
			MouseHandlerState newState = CreateAppointmentDragState(appointment, layoutHitInfo, prevLayoutHitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		#region SwitchToAppointmentDependencyMouseDownState
		protected internal virtual void SwitchToAppointmentDependencyMouseDownState(Point mousePosition, AppointmentDependencyCollection dependencies, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo) {
			MouseHandlerState newState = new AppointmentDependencyMouseDownState(this, dependencies, mousePosition, mouseButtons, hitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		#endregion
		public override void StopClickTimer() {
			base.StopClickTimer();
			clickedViewInfo = EmptySelectableIntervalViewInfo;
		}
		#region SynchronizeSelectionWithAppointment
		protected internal virtual void SynchronizeSelectionWithAppointment(Appointment appointment, Resource resource) {
			if (appointment == null)
				return;
			SchedulerViewSelection selection = Control.Selection;
			TimeInterval serverSelectionInterval = ((IInternalAppointment)appointment).CreateInterval();
			TimeInterval clientSelectionInterval = Control.TimeZoneHelper.ToClientTime(serverSelectionInterval);
			selection.Interval = clientSelectionInterval;
			if (resource != null) {
				selection.Resource = resource;
			}
			InnerSchedulerViewBase view = control.ActiveView;
			view.SynchronizeSelectionInterval(selection, false);
			view.SynchronizeSelectionResource(selection);
			UpdateActiveViewSelection(selection);
			Redraw();
			Control.RaiseSelectionChanged();
			Control.RaiseUpdateUI();
		}
		#endregion
		protected internal virtual SchedulerPopupMenuBuilder CreateDragPopupMenuBuilder(ISchedulerHitInfo hitInfo, SchedulerDragPopupMenuInfo dragPopupMenuInfo) {
#if !SILVERLIGHT && !WPF
			return new SchedulerAppointmentDragPopupMenuBuilder(UiFactory, Control, dragPopupMenuInfo.DragData, dragPopupMenuInfo.ChangedAppointments);
#else
			return null;
#endif
		}
		protected internal virtual EditAppointmentCommand CreateEditAppointmentCommand() {
			return new EditAppointmentQueryCommand(Control);
		}
		protected internal abstract void UpdateHotTrack(ISelectableIntervalViewInfo viewInfo);
		protected internal abstract void ScrollBackward(PlatformIndependentMouseEventArgs e);
		protected internal abstract void ScrollForward(PlatformIndependentMouseEventArgs e);
		public abstract bool OnPopupMenu(Point pt);
		protected internal abstract MouseHandlerState CreateDefaultState();
		protected internal abstract MouseHandlerState CreateAppointmentDragState(Appointment appointment, ISchedulerHitInfo layoutHitInfo, ISchedulerHitInfo prevLayoutHitInfo);
		protected internal abstract void Redraw();
		public abstract SchedulerPopupMenuBuilder CreateDefaultPopupMenuBuilder(ISchedulerHitInfo hitInfo);
		protected internal abstract void SetCurrentCursor(PlatformIndependentCursor cursor);
		protected internal abstract void UpdateActiveViewSelection(SchedulerViewSelection selection);
		protected internal virtual void ApplyChangesOnDragAppointments() {
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcAppointmentsLayout);
		}
		protected internal virtual AppointmentResizingEdge CalculateResizingEdge(SchedulerHitTest hitTest) {
			if (hitTest == SchedulerHitTest.AppointmentResizingLeftEdge || hitTest == SchedulerHitTest.AppointmentResizingTopEdge)
				return AppointmentResizingEdge.AppointmentStart;
			else
				return AppointmentResizingEdge.AppointmentEnd;
		}
	}
	#endregion
	#region SchedulerMouseHandlerState (abstract class)
	public abstract class SchedulerMouseHandlerState : MouseHandlerState {
		protected SchedulerMouseHandlerState(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public new SchedulerMouseHandler MouseHandler { get { return (SchedulerMouseHandler)base.MouseHandler; } }
		public InnerSchedulerControl Control { get { return MouseHandler.Control; } }
		internal bool IsInternalState { get; set; }
		#endregion
		public virtual SchedulerPopupMenuBuilder CreatePopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return MouseHandler.CreateDefaultPopupMenuBuilder(hitInfo);
		}
		public virtual void OnDateTimeScroll(Point mousePosition, ISchedulerHitInfo hitInfo) {
		}
		public virtual bool OnDragEnterToExternalControl(SchedulerDragData dragData) {
			return false;
		}
		public virtual bool OnDragOverExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, bool copy, bool isDrop) {
			return false;
		}
		public virtual bool OnDragDropExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, Point pt, bool copy, bool showPopupMenu) {
			return false;
		}
		public virtual void OnDragLeaveExternalControl() {
		}
		public virtual bool OnPopupMenu(Point pt, ISchedulerHitInfo hitInfo) {
			return true;
		}
	}
	#endregion
	#region DefaultMouseHandlerState (abstract class)
	public abstract class DefaultMouseHandlerState : SchedulerMouseHandlerState {
		bool defaultMouseCursor = true;
		protected DefaultMouseHandlerState(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return false; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			SetMouseCursor(hitInfo);
			MouseHandler.UpdateHotTrack(hitInfo.ViewInfo);
		}
		#endregion
		#region SetMouseCursor
		protected internal virtual void SetMouseCursor(ISchedulerHitInfo hitInfo) {
			if (SetMoreButtonCursor(hitInfo))
				return;
			if (SetResizeCursor(hitInfo))
				return;
			if (SetMoveCursor(hitInfo))
				return;
			if (!defaultMouseCursor) {
				PlatformIndependentCursor cursor = CursorTypes.Default;
				ChangeCursor(cursor);
			}
		}
		#endregion
		protected internal virtual bool SetMoreButtonCursor(ISchedulerHitInfo hitInfo) {
			if ((hitInfo.HitTest == SchedulerHitTest.MoreButton) || (hitInfo.HitTest == SchedulerHitTest.ScrollMoreButton)) {
				ChangeCursor(PlatformIndependentCursors.Hand);
				return true;
			}
			return false;
		}
		protected internal virtual bool SetResizeCursor(ISchedulerHitInfo hitInfo) {
			if (IsResizingEdge(hitInfo.HitTest)) {
				IAppointmentView aptViewInfo = (IAppointmentView)hitInfo.ViewInfo;
				if (!MouseHandler.AppointmentOperationHelper.CanResizeAppointment(aptViewInfo.Appointment)) {
					if (!defaultMouseCursor)
						ChangeCursor(CursorTypes.Default);
				} else {
					if (hitInfo.HitTest == SchedulerHitTest.AppointmentResizingLeftEdge || hitInfo.HitTest == SchedulerHitTest.AppointmentResizingRightEdge)
						ChangeCursor(PlatformIndependentCursors.SizeWE);
					else
						ChangeCursor(PlatformIndependentCursors.SizeNS);
				}
				return true;
			} else
				return false;
		}
		protected internal virtual bool SetMoveCursor(ISchedulerHitInfo hitInfo) {
			if (hitInfo.HitTest == SchedulerHitTest.AppointmentMoveEdge) {
				IAppointmentView aptViewInfo = (IAppointmentView)hitInfo.ViewInfo;
				if (!MouseHandler.AppointmentOperationHelper.CanDragAppointment(aptViewInfo.Appointment)) {
					if (!defaultMouseCursor)
						ChangeCursor(CursorTypes.Default);
				} else
					ChangeCursor(CursorTypes.SizeAll);
				return true;
			}
			return false;
		}
		#region OnMouseDown
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			SetMouseCursor(hitInfo);
			ChangeState(e, hitInfo);
		}
		#endregion
		#region ChangeState
		protected internal virtual void ChangeState(PlatformIndependentMouseEventArgs e, ISchedulerHitInfo hitInfo) {
			Point mousePosition = new Point(e.X, e.Y);
			SchedulerHitTest hitTest = hitInfo.HitTest;
			if (IsAppointmentContentOrMoveEdge(hitTest)) {
				Appointment appointment = ((IAppointmentView)hitInfo.ViewInfo).Appointment;
				MouseHandler.SwitchToAppointmentMouseDownState(mousePosition, appointment, e.Button, hitInfo);
				return;
			}
			if (ShouldSelectAppointmentDependency(hitTest)) {
				AppointmentDependencyCollection dependencies = ((IAppointmentDependencyView)hitInfo.ViewInfo).Dependencies;
				MouseHandler.SwitchToAppointmentDependencyMouseDownState(mousePosition, dependencies, e.Button, hitInfo);
				return;
			}
			bool rightButtonClick = (e.Button & PlatformIndependentMouseButtons.Right) != 0;
			bool leftButtonClick = (e.Button & PlatformIndependentMouseButtons.Left) != 0;
			if (leftButtonClick) {
				if (TryToHandleButtonsClick(hitInfo))
					return;
				if (IsResizingEdge(hitTest)) {
					Appointment appointment = ((IAppointmentView)hitInfo.ViewInfo).Appointment;
					if (MouseHandler.AppointmentOperationHelper.CanResizeAppointment(appointment))
						MouseHandler.SwitchToAppointmentResizeEdgeMouseDownState(mousePosition, appointment, hitInfo);
					else
						MouseHandler.SwitchToAppointmentMouseDownState(mousePosition, appointment, e.Button, hitInfo);
					return;
				}
			}
			if (rightButtonClick) { 
				if (IsResizingEdge(hitTest)) {
					Appointment appointment = ((IAppointmentView)hitInfo.ViewInfo).Appointment;
					AppointmentMouseDownState newState = new AppointmentMouseDownState(MouseHandler, appointment, mousePosition, e.Button, hitInfo);
					newState.SelectAppointment();
				}
			}
			if (!CanStartCellSelection(hitTest))
				return;
			if (leftButtonClick) {
				MouseHandler.SwitchToCellSelectionState(mousePosition, hitInfo, PlatformIndependentMouseButtons.Left);
				return;
			}
			if (rightButtonClick && Control.OptionsBehavior.SelectOnRightClick) {
				if (ShouldChangeSelectionOnRightClick(hitInfo)) {
					MouseHandler.SwitchToCellSelectionState(mousePosition, hitInfo, PlatformIndependentMouseButtons.Right);
				}
			}
		}
		#endregion
		#region IsAppointmentContentOrMoveEdge
		protected internal virtual bool IsAppointmentContentOrMoveEdge(SchedulerHitTest hitTest) {
			return hitTest == SchedulerHitTest.AppointmentContent || hitTest == SchedulerHitTest.AppointmentMoveEdge;
		}
		#endregion
		#region CanStartCellSelection
		protected internal virtual bool CanStartCellSelection(SchedulerHitTest hitTest) {
			return hitTest == SchedulerHitTest.Cell || hitTest == SchedulerHitTest.AllDayArea ||
					hitTest == SchedulerHitTest.DayHeader || hitTest == SchedulerHitTest.ResourceHeader ||
					hitTest == SchedulerHitTest.SelectionBarCell;
		}
		#endregion
		#region ShouldChangeSelectionOnRightClick
		bool ShouldChangeSelectionOnRightClick(ISchedulerHitInfo hitInfo) {
			if (!hitInfo.ViewInfo.Selected)
				return true;
			Resource selectedResource = Control.Selection.Resource;
			Resource hitResource = hitInfo.ViewInfo.Resource;
			return (selectedResource != hitResource && hitResource != ResourceBase.Empty);
		}
		#endregion
		#region IsResizingEdge
		protected internal virtual bool IsResizingEdge(SchedulerHitTest hitTest) {
			return hitTest == SchedulerHitTest.AppointmentResizingTopEdge || hitTest == SchedulerHitTest.AppointmentResizingBottomEdge ||
				hitTest == SchedulerHitTest.AppointmentResizingLeftEdge || hitTest == SchedulerHitTest.AppointmentResizingRightEdge;
		}
		#endregion
		#region GetResizingEdge
		protected internal virtual AppointmentResizingEdge GetResizingEdge(SchedulerHitTest hitTest) {
			return MouseHandler.CalculateResizingEdge(hitTest);
		}
		#endregion
		bool ShouldSelectAppointmentDependency(SchedulerHitTest hittest) {
			return hittest == SchedulerHitTest.AppointmentDependency;
		}
		#region OnDragEnter
		public override void OnDragEnter(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			ISchedulerHitInfo layoutHitInfo = hitInfo.FindFirstLayoutHitInfo();
			MouseHandler.SwitchToExternalDragAndDropState(Point.Empty, layoutHitInfo, e);
		}
		#endregion
		public override bool OnDragEnterToExternalControl(SchedulerDragData dragData) {
			Guard.ArgumentNotNull(dragData, "dragData");
			MouseHandler.SwitchToExternalDragAndDropOnExternalControlState(dragData);
			return true;
		}
		#region OnLongMouseDown
		public override void OnLongMouseDown() {
			IAppointmentView viewInfo = MouseHandler.ClickedViewInfo as IAppointmentView;
			if (viewInfo == null)
				return;
			if (CanActivateInplaceEditor(viewInfo)) {
				EditAppointmentViaInplaceEditorCommand command = new EditAppointmentViaInplaceEditorCommand(Control);
				command.CommandSourceType = CommandSourceType.Mouse;
				command.Execute(Control.InplaceEditController.EditedAppointment);
			}
		}
		#endregion
		protected internal virtual bool CanActivateInplaceEditor(IAppointmentView viewInfo) {
			Appointment editedAppointment = Control.InplaceEditController.EditedAppointment;
			ISetSchedulerStateService service = Control.GetService(typeof(ISetSchedulerStateService)) as ISetSchedulerStateService;
			return editedAppointment != null && SchedulerUtils.IsAppointmentsEquals(editedAppointment, viewInfo.Appointment) && !service.IsModalFormOpened;
		}
		protected internal virtual void ChangeCursor(PlatformIndependentCursor newCursor) {
			MouseHandler.SetCurrentCursor(newCursor);
			defaultMouseCursor = (newCursor == CursorTypes.Default);
		}
		protected internal abstract bool TryToHandleButtonsClick(ISchedulerHitInfo hitInfo);
	}
	#endregion
	#region CellSelectionMouseHandlerState
	public class CellSelectionMouseHandlerState : SchedulerMouseHandlerState {
		PlatformIndependentMouseButtons button;
		bool shoudUpdateSelection;
		#region CellSelectionMouseHandlerState
		public CellSelectionMouseHandlerState(SchedulerMouseHandler handler, ISchedulerHitInfo hitInfo, PlatformIndependentMouseButtons button)
			: base(handler) {
			this.button = button;
			this.shoudUpdateSelection = Control.SelectedAppointments.Count > 0;
			ProcessCellSelection(hitInfo);
		}
		#endregion
		#region Properties
		protected internal virtual bool IsShiftPressed { get { return KeyboardHandler.IsShiftPressed; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		#endregion
		#region Finish
		public override void Finish() {
			Control.RestoreKeyboardHandler();
			base.Finish();
		}
		#endregion
		#region ProcessCellSelection
		protected virtual void ProcessCellSelection(ISchedulerHitInfo hitInfo) {
			Control.BeginUpdate();
			try {
				Control.SetNewKeyboardHandler(new EmptyKeyboardHandler());
				Control.AppointmentSelectionController.ClearSelection();
				Control.AppointmentDependencySelectionController.ClearSelection();
				ISchedulerHitInfo selectionHitInfo = FindSelectionHitInfo(hitInfo);
				if (!IsShiftPressed)
					CreateNewSelection(selectionHitInfo);
				else
					ContinueSelection(selectionHitInfo);
			} finally {
				Control.EndUpdate();
			}
		}
		#endregion
		#region CreateNewSelection
		protected internal virtual void CreateNewSelection(ISchedulerHitInfo selectionHitInfo) {
			SchedulerViewSelection newSelection = new SchedulerViewSelection();
			ISelectableIntervalViewInfo selectionViewInfo = selectionHitInfo.ViewInfo;
			newSelection.Interval = selectionViewInfo.Interval.Clone();
			if (ShouldChangeSelectionResource(Control.Selection.Resource, selectionViewInfo.Resource))
				newSelection.Resource = selectionViewInfo.Resource;
			else
				newSelection.Resource = Control.Selection.Resource;
			newSelection.FirstSelectedInterval = selectionViewInfo.Interval.Clone();
			if (!Control.Selection.IsEqual(newSelection)) {
				Control.Selection = newSelection;
				UpdateControlSelection();
			} else if (shoudUpdateSelection)
				MouseHandler.UpdateActiveViewSelection(Control.Selection);
		}
		#endregion
		#region ContinueSelection
		protected internal virtual void ContinueSelection(ISchedulerHitInfo selectionHitInfo) {
			selectionHitInfo = selectionHitInfo.FindFirstLayoutHitInfo();
			if (selectionHitInfo.HitTest == SchedulerHitTest.None)
				return;
			TimeInterval interval = selectionHitInfo.ViewInfo.Interval;
			if (interval.Equals(TimeInterval.Empty))
				return;
			TimeInterval newInterval = CalculateSelectionInterval(selectionHitInfo.HitTest, interval);
			if (!newInterval.Equals(Control.Selection.Interval)) {
				Control.Selection.Interval = newInterval;
				UpdateControlSelection();
			}
		}
		#endregion
		#region ShouldChangeSelectionResource
		protected internal virtual bool ShouldChangeSelectionResource(Resource oldResource, Resource newResource) {
			return oldResource == ResourceBase.Empty || newResource != ResourceBase.Empty;
		}
		#endregion
		#region FindSelectionHitInfo
		protected internal virtual ISchedulerHitInfo FindSelectionHitInfo(ISchedulerHitInfo hitInfo) {
			return hitInfo.FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.DayHeader | SchedulerHitTest.ResourceHeader | SchedulerHitTest.AllDayArea | SchedulerHitTest.SelectionBarCell, SchedulerHitTest.GroupSeparator);
		}
		#endregion
		#region UpdateControlSelection
		protected internal virtual void UpdateControlSelection() {
			MouseHandler.UpdateActiveViewSelection(Control.Selection);
			Control.RaiseSelectionChanged();
			Control.RaiseUpdateUI();
			MouseHandler.Redraw();
		}
		#endregion
		#region CalculateSelectionInterval
		protected internal virtual TimeInterval CalculateSelectionInterval(SchedulerHitTest layoutHitTest, TimeInterval hitInterval) {
			TimeInterval firstInterval = Control.Selection.FirstSelectedInterval;
			if (layoutHitTest == SchedulerHitTest.AllDayArea || layoutHitTest == SchedulerHitTest.ResourceHeader || layoutHitTest == SchedulerHitTest.DayHeader)
				firstInterval = DateTimeHelper.ExpandTimeIntervalToFullDays(firstInterval);
			if (firstInterval.Contains(hitInterval))
				return new TimeInterval(firstInterval.Start, hitInterval.End);
			if (hitInterval.Start >= firstInterval.End)
				return new TimeInterval(firstInterval.Start, hitInterval.End);
			else
				return new TimeInterval(hitInterval.Start, firstInterval.End);
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			ContinueSelection(hitInfo);
		}
		#endregion
		#region OnDateTimeScroll
		public override void OnDateTimeScroll(Point mousePosition, ISchedulerHitInfo hitInfo) {
			ContinueSelection(hitInfo);
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			ContinueSelection(hitInfo);
			if ((e.Button & button) != 0)
				MouseHandler.SwitchToDefaultState();
		}
		#endregion
		#region OnMouseDown
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
		}
		#endregion
	}
	#endregion
	#region AppointmentResizeEdgeMouseDownState
	public class AppointmentResizeEdgeMouseDownState : SchedulerMouseHandlerState {
		#region Fields
		Rectangle dragBox;
		Appointment pressedAppointment;
		SchedulerHitTest pressedHitTest;
		ISchedulerHitInfo pressedHitInfo;
		#endregion
		public AppointmentResizeEdgeMouseDownState(SchedulerMouseHandler mouseHandler, Point mousePosition, Appointment appointment, ISchedulerHitInfo hitInfo)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(appointment, "appointment");
			XtraSchedulerDebug.Assert(appointment.Type == AppointmentType.ChangedOccurrence ||
				appointment.Type == AppointmentType.Normal || appointment.Type == AppointmentType.Occurrence);
			this.pressedHitInfo = hitInfo;
			this.pressedHitTest = hitInfo.HitTest;
			bool isValidHitTest = this.pressedHitTest == SchedulerHitTest.AppointmentResizingLeftEdge || this.pressedHitTest == SchedulerHitTest.AppointmentResizingTopEdge ||
				this.pressedHitTest == SchedulerHitTest.AppointmentResizingRightEdge || this.pressedHitTest == SchedulerHitTest.AppointmentResizingBottomEdge;
			if (!isValidHitTest)
				Exceptions.ThrowArgumentException("hitTest", this.pressedHitTest);
			this.pressedAppointment = appointment;
			Size dragSize = PlatformIndependentSystemInformation.DragSize;
			dragBox = new Rectangle(mousePosition.X - dragSize.Width / 2, mousePosition.Y - dragSize.Height / 2, dragSize.Width, dragSize.Height);
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return false; } }
		internal virtual Rectangle DragBox { get { return dragBox; } }
		internal Appointment PressedAppointment { get { return pressedAppointment; } }
		#endregion
		#region OnCancelState
		public override void OnCancelState() {
			MouseHandler.SwitchToDefaultState();
		}
		#endregion
		public override void Start() {
			base.Start();
			SchedulerStateHelper.BeginResize(MouseHandler.Control);
			Control.SetNewKeyboardHandler(new EscapeKeyboardHandler(this));
			SelectAppointment();
		}
		protected internal virtual void SelectAppointment() {
			Control.BeginUpdate();
			try {
				ChangeAppointmentSelection(pressedAppointment);
				SynchronizeSelection();
			} finally {
				Control.EndUpdate();
			}
		}
		protected internal virtual void SynchronizeSelection() {
			ISchedulerHitInfo layoutHitInfo = pressedHitInfo.FindFirstLayoutHitInfo();
			MouseHandler.SynchronizeSelectionWithAppointment(pressedAppointment, layoutHitInfo.ViewInfo.Resource);
		}
		protected internal virtual void ChangeAppointmentSelection(Appointment appointment) {
			Control.AppointmentSelectionController.ApplyChanges(AppointmentSelectionChangeAction.Select, appointment);
		}
		#region Finish
		public override void Finish() {
			SchedulerStateHelper.EndResize(MouseHandler.Control);
			Control.RestoreKeyboardHandler();
			base.Finish();
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			MouseHandler.SwitchToDefaultState();
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			if (CanStartResize(mousePosition)) {
				if (MouseHandler.AppointmentOperationHelper.CanResizeAppointment(pressedAppointment))
					MouseHandler.SwitchToAppointmentResizeState(mousePosition, pressedAppointment, MouseHandler.CalculateResizingEdge(pressedHitTest));
				else
					MouseHandler.SwitchToAppointmentMouseDownState(mousePosition, pressedAppointment, e.Button, MouseHandler.CurrentHitInfo);
			}
		}
		#endregion
		#region CanStartDrag
		protected internal virtual bool CanStartResize(Point currentMousePosition) {
			if (pressedHitTest == SchedulerHitTest.AppointmentResizingTopEdge || pressedHitTest == SchedulerHitTest.AppointmentResizingBottomEdge)
				return currentMousePosition.Y < DragBox.Top || currentMousePosition.Y > DragBox.Bottom;
			if (pressedHitTest == SchedulerHitTest.AppointmentResizingLeftEdge || pressedHitTest == SchedulerHitTest.AppointmentResizingRightEdge)
				return currentMousePosition.X < DragBox.Left || currentMousePosition.X > DragBox.Right;
			return false;
		}
		#endregion
		#region CanShowPopupMenu
		public override bool OnPopupMenuShowing() {
			return false;
		}
		#endregion
	}
	#endregion
	#region AppointmentResizeMouseHandlerState
	public class AppointmentResizeMouseHandlerState : SchedulerMouseHandlerState {
		PlatformIndependentMouseEventArgs prevMouseEventArgs;
		#region AppointmentResizeMouseHandlerState
		public AppointmentResizeMouseHandlerState(SchedulerMouseHandler mouseHandler, Appointment appointment, AppointmentResizingEdge appointmentResizingEdge)
			: base(mouseHandler) {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
			XtraSchedulerDebug.Assert(appointment.Type == AppointmentType.ChangedOccurrence ||
				appointment.Type == AppointmentType.Normal || appointment.Type == AppointmentType.Occurrence);
			Control.AppointmentSelectionController.SelectSingleAppointment(appointment);
			if (Control.AppointmentSelectionController.IsAppointmentSelected(appointment))
				AppointmentChangeHelper.BeginAppointmentsResize(new SafeAppointmentCollection(Control.SelectedAppointments, Control.Storage), appointmentResizingEdge);
		}
		#endregion
		#region Properties
		internal AppointmentChangeHelper AppointmentChangeHelper { get { return Control.AppointmentChangeHelper; } }
		#endregion
		#region OnCancelState
		public override void OnCancelState() {
			AppointmentChangeHelper.CancelChanges();
			MouseHandler.SwitchToDefaultState();
		}
		#endregion
		#region Start
		public override void Start() {
			PrepareControlForResize();
			base.Start();
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsAppointmentResized = true;
			Control.SetNewKeyboardHandler(new EscapeKeyboardHandler(this));
			Control.ActiveView.ExtendedVisibleInterval = Control.ActiveView.GetVisibleIntervals().Interval;
		}
		#endregion
		#region Finish
		public override void Finish() {
			Control.RestoreKeyboardHandler();
			MouseHandler.SetCurrentCursor(CursorTypes.Default);
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsAppointmentResized = false;
			Control.ActiveView.ExtendedVisibleInterval = TimeInterval.Empty;
			base.Finish();
			PrepareControlAfterResize();
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			if (prevMouseEventArgs != null && prevMouseEventArgs.Button == e.Button && prevMouseEventArgs.X == e.X && prevMouseEventArgs.Y == e.Y)
				return;
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			SetEditedAppointmentTimeInterval(hitInfo);
			prevMouseEventArgs = e;
		}
		#endregion
		#region OnDateTimeScroll
		public override void OnDateTimeScroll(Point mousePosition, ISchedulerHitInfo hitInfo) {
			SetEditedAppointmentTimeInterval(hitInfo);
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			SetEditedAppointmentTimeInterval(hitInfo);
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			if ((e.Button & PlatformIndependentMouseButtons.Left) != 0) {
				MouseHandler.Suspend();
				try {
					AppointmentChangeHelper.CommitResize();
				} finally {
					MouseHandler.Resume();
					MouseHandler.SwitchToDefaultState();
					Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
				}
			}
		}
		#endregion
		#region SetEditedAppointmentTimeInterval
		protected internal virtual void SetEditedAppointmentTimeInterval(ISchedulerHitInfo hitInfo) {
			hitInfo = hitInfo.FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.AllDayArea | SchedulerHitTest.SelectionBarCell);
			if (hitInfo.HitTest == SchedulerHitTest.None)
				return;
			AppointmentChangeHelper.Resize(hitInfo.ViewInfo);
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
		}
		#endregion
		#region CanShowPopupMenu
		public override bool OnPopupMenuShowing() {
			return false;
		}
		#endregion
		protected virtual void PrepareControlForResize() { 
		}
		protected virtual void PrepareControlAfterResize() {
		}
	}
	#endregion
	#region AppointmentMouseDownState
	public class AppointmentMouseDownState : SchedulerMouseHandlerState {
		#region Fields
		Rectangle dragBox;
		Appointment pressedAppointment;
		ISchedulerHitInfo pressedHitInfo;
		PlatformIndependentMouseButtons pressedMouseButtons;
		bool deferredSingleAppointmentSelection;
		bool deselectAppointmentOnMouseUp;
		#endregion
		public AppointmentMouseDownState(SchedulerMouseHandler mouseHandler, Appointment appointment, Point mousePosition, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo)
			: base(mouseHandler) {
			Size dragSize = PlatformIndependentSystemInformation.DragSize;
			this.pressedAppointment = appointment;
			this.pressedHitInfo = hitInfo;
			this.pressedMouseButtons = mouseButtons;
			dragBox = new Rectangle(mousePosition.X - dragSize.Width / 2, mousePosition.Y - dragSize.Height / 2, dragSize.Width, dragSize.Height);
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return false; } }
		protected internal virtual KeyState KeyState { get { return KeyboardHandler.KeyState; } }
#if !SILVERLIGHT
		protected internal virtual PlatformIndependentMouseButtons MouseButtonsState { get { return System.Windows.Forms.Control.MouseButtons; } }
#else
		protected internal virtual PlatformIndependentMouseButtons MouseButtonsState { get { return Control.Owner.LeftButtonPressed ? PlatformIndependentMouseButtons.Left : PlatformIndependentMouseButtons.None; } }
#endif
		internal Rectangle DragBox { get { return dragBox; } }
		internal Appointment PressedAppointment { get { return pressedAppointment; } }
		internal ISchedulerHitInfo PressedHitInfo { get { return pressedHitInfo; } }
		internal PlatformIndependentMouseButtons PressedMouseButtons { get { return pressedMouseButtons; } }
		internal bool DeferredSingleAppointmentSelection { get { return deferredSingleAppointmentSelection; } }
		internal bool DeselectAppointmentOnMouseUp { get { return deselectAppointmentOnMouseUp; } set { deselectAppointmentOnMouseUp = value; } }
		#endregion
		#region Start
		public override void Start() {
			base.Start();
			Control.AppointmentDependencySelectionController.ClearSelection();
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.AreAppointmentsDragged = true;
			bool appointmentSelectionChanged = SelectAppointment();
			if (CanUseInplaceEditor(appointmentSelectionChanged))
				Control.InplaceEditController.SetEditedAppointment(pressedAppointment);
			else
				Control.InplaceEditController.ResetEditedAppointment();
		}
		#endregion
		protected internal virtual bool SelectAppointment() {
			bool appointmentSelectionChanged;
			Control.BeginUpdate();
			try {
				appointmentSelectionChanged = ChangeAppointmentSelection(pressedAppointment);
				AfterSelectAppointment(appointmentSelectionChanged);
			} finally {
				Control.EndUpdate();
			}
			return appointmentSelectionChanged;
		}
		protected internal virtual void AfterSelectAppointment(bool appointmentSelectionChanged) {
			ISchedulerHitInfo layoutHitInfo = pressedHitInfo.FindFirstLayoutHitInfo();
			if (appointmentSelectionChanged || (!appointmentSelectionChanged && layoutHitInfo.ViewInfo.Resource.Id != Control.Selection.Resource.Id))
				MouseHandler.SynchronizeSelectionWithAppointment(pressedAppointment, layoutHitInfo.ViewInfo.Resource);
		}
		#region Finish
		public override void Finish() {
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.AreAppointmentsDragged = false;
			base.Finish();
		}
		#endregion
		#region CanUseInplaceEditor
		protected internal virtual bool CanUseInplaceEditor(bool appointmentSelectionChanged) {
			return !appointmentSelectionChanged && !deferredSingleAppointmentSelection && pressedHitInfo.HitTest == SchedulerHitTest.AppointmentContent && pressedMouseButtons == PlatformIndependentMouseButtons.Left;
		}
		#endregion
		#region ChangeAppointmentSelection
		protected internal virtual bool ChangeAppointmentSelection(Appointment appointment) {
			AppointmentSelectionChangeAction changeAction = CalculateSelectionChangeActions(appointment);
			return Control.AppointmentSelectionController.ApplyChanges(changeAction, appointment);
		}
		#endregion
		#region CalculateSelectionChangeActions
		protected internal virtual AppointmentSelectionChangeAction CalculateSelectionChangeActions(Appointment appointment) {
			deferredSingleAppointmentSelection = false;
			if ((pressedMouseButtons & PlatformIndependentMouseButtons.Right) == 0)
				return CalculateSelectionChangeActionLeftButtonPressed(appointment);
			else
				return CalculateSelectionChangeActionRightButtonPressed(appointment);
		}
		#endregion
		#region CalculateSelectionChangeActionLeftButtonPressed
		protected internal virtual AppointmentSelectionChangeAction CalculateSelectionChangeActionLeftButtonPressed(Appointment appointment) {
			KeyState keyState = KeyState;
			if ((keyState & KeyState.CtrlKey) != 0) {
				if (!Control.AppointmentSelectionController.IsAppointmentSelected(appointment)) {
					if (Control.AppointmentSelectionController.AllowAppointmentMultiSelect)
						return AppointmentSelectionChangeAction.Toggle;
					else
						return AppointmentSelectionChangeAction.Select;
				} else {
					deselectAppointmentOnMouseUp = true;
					return AppointmentSelectionChangeAction.None;
				}
			}
			if ((keyState & KeyState.ShiftKey) != 0) {
				if (Control.AppointmentSelectionController.AllowAppointmentMultiSelect)
					return AppointmentSelectionChangeAction.Add;
				else
					return AppointmentSelectionChangeAction.Select;
			}
			if (!Control.AppointmentSelectionController.IsAppointmentSelected(appointment))
				return AppointmentSelectionChangeAction.Select;
			if (Control.SelectedAppointments.Count != 1)
				deferredSingleAppointmentSelection = true;
			return AppointmentSelectionChangeAction.None;
		}
		#endregion
		#region CalculateSelectionChangeActionRightButtonPressed
		protected internal virtual AppointmentSelectionChangeAction CalculateSelectionChangeActionRightButtonPressed(Appointment appointment) {
			KeyState keyState = KeyState;
			if (Control.AppointmentSelectionController.IsAppointmentSelected(appointment))
				return AppointmentSelectionChangeAction.None;
			if ((keyState & (KeyState.CtrlKey | KeyState.ShiftKey)) != 0)
				return AppointmentSelectionChangeAction.Add;
			else
				return AppointmentSelectionChangeAction.Select;
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			KeyState keyState = KeyState;
			if ((e.Button & PlatformIndependentMouseButtons.Left) != 0) {
				if (deselectAppointmentOnMouseUp && (keyState & KeyState.CtrlKey) != 0) {
					if (Control.AppointmentSelectionController.IsAppointmentSelected(pressedAppointment)) {
						Control.AppointmentSelectionController.ApplyChanges(AppointmentSelectionChangeAction.Toggle, pressedAppointment);
						AfterSelectAppointment(true);
						deferredSingleAppointmentSelection = false;
						MouseHandler.StopClickTimer();
					}
					deselectAppointmentOnMouseUp = false;
				}
			}
			if ((MouseButtonsState & (PlatformIndependentMouseButtons.Left | PlatformIndependentMouseButtons.Right)) == 0) {
				ApplyDeferredAppointmentSelection();
				MouseHandler.SwitchToDefaultState();
			}
		}
		#endregion
		#region ApplyDeferredAppointmentSelection
		protected internal virtual void ApplyDeferredAppointmentSelection() {
			if (deferredSingleAppointmentSelection) {
				Control.AppointmentSelectionController.SelectSingleAppointment(pressedAppointment);
				ISchedulerHitInfo layoutHitInfo = pressedHitInfo.FindFirstLayoutHitInfo();
				MouseHandler.SynchronizeSelectionWithAppointment(pressedAppointment, layoutHitInfo.ViewInfo.Resource);
			}
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			if (CanStartDrag(mousePosition)) {
				ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
				MouseHandler.SwitchToInternalDragState(mousePosition, pressedAppointment, hitInfo, pressedHitInfo);
			}
		}
		#endregion
		#region OnDateTimeScroll
		public override void OnDateTimeScroll(Point mousePosition, ISchedulerHitInfo hitInfo) {
			if (CanStartDrag(mousePosition))
				MouseHandler.SwitchToInternalDragState(mousePosition, pressedAppointment, hitInfo, pressedHitInfo);
		}
		#endregion
		#region CanStartDrag
		protected internal virtual bool CanStartDrag(Point currentMousePosition) {
			return !dragBox.Contains(currentMousePosition) && MouseHandler.AppointmentOperationHelper.CanDragAppointment(pressedAppointment);
		}
		#endregion
	}
	#endregion
	#region AppointmentDependencyMouseDownState
	public class AppointmentDependencyMouseDownState : SchedulerMouseHandlerState {
		#region Fields
		ISchedulerHitInfo pressedHitInfo;
		PlatformIndependentMouseButtons pressedMouseButtons;
		AppointmentDependencyCollection dependencies;
		bool deselectDependencyOnMouseUp;
		#endregion
		public AppointmentDependencyMouseDownState(SchedulerMouseHandler mouseHandler, AppointmentDependencyCollection dependencies, Point mousePosition, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo)
			: base(mouseHandler) {
			this.pressedHitInfo = hitInfo;
			this.pressedMouseButtons = mouseButtons;
			this.dependencies = dependencies;
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return false; } }
		protected internal virtual KeyState KeyState { get { return KeyboardHandler.KeyState; } }
		public AppointmentDependencyCollection Dependencies { get { return dependencies; } }
#if !SILVERLIGHT
		protected internal virtual PlatformIndependentMouseButtons MouseButtonsState { get { return System.Windows.Forms.Control.MouseButtons; } }
#else
		protected internal virtual PlatformIndependentMouseButtons MouseButtonsState { get { return Control.Owner.LeftButtonPressed ? PlatformIndependentMouseButtons.Left : PlatformIndependentMouseButtons.None; } }
#endif
		internal ISchedulerHitInfo PressedHitInfo { get { return pressedHitInfo; } }
		internal PlatformIndependentMouseButtons PressedMouseButtons { get { return pressedMouseButtons; } }
		internal bool DeselectDependencyOnMouseUp { get { return deselectDependencyOnMouseUp; } set { deselectDependencyOnMouseUp = value; } }
		#endregion
		public override void Start() {
			base.Start();
			Control.AppointmentSelectionController.ClearSelection();
			SelectAppointmentDependencies(); 
		}
		protected internal virtual bool SelectAppointmentDependencies() {
			bool dependencySelectionChanged = ChangeDependenciesSelection(Dependencies);
			AfterSelectDependency(dependencySelectionChanged);
			return dependencySelectionChanged;
		}
		protected internal virtual bool ChangeDependenciesSelection(AppointmentDependencyBaseCollection dependencies) {
			bool result = false;
			Control.BeginUpdate();
			Control.AppointmentDependencySelectionController.BeginUpdate();
			KeyState keyState = KeyState;
			if ((keyState & KeyState.CtrlKey) == 0 && ((keyState & KeyState.ShiftKey) == 0) && MouseButtonsState != PlatformIndependentMouseButtons.Right) {
				Control.AppointmentDependencySelectionController.ClearSelection();
			}
			try {
				for (int i = 0; i < dependencies.Count; i++) {
					AppointmentDependency dependency = dependencies[i];
					AppointmentDependencySelectionChangeAction changeAction = CalculateSelectionChangeActions(dependency);
					result |= Control.AppointmentDependencySelectionController.ApplyChanges(changeAction, dependency);
				}
			} finally {
				Control.AppointmentDependencySelectionController.EndUpdate();
				Control.EndUpdate();
			}
			return result;
		}
		protected internal virtual AppointmentDependencySelectionChangeAction CalculateSelectionChangeActions(AppointmentDependency dependency) {
			if ((pressedMouseButtons & PlatformIndependentMouseButtons.Right) == 0)
				return CalculateSelectionChangeActionLeftButtonPressed(dependency);
			else
				return CalculateSelectionChangeActionRightButtonPressed(dependency);
		}
		protected internal virtual AppointmentDependencySelectionChangeAction CalculateSelectionChangeActionLeftButtonPressed(AppointmentDependency dependency) {
			KeyState keyState = KeyState;
			if ((keyState & KeyState.CtrlKey) != 0) {
				if (!Control.AppointmentDependencySelectionController.IsAppointmentDependencySelected(dependency))
					return AppointmentDependencySelectionChangeAction.Toggle;
				else {
					deselectDependencyOnMouseUp = true;
					return AppointmentDependencySelectionChangeAction.None;
				}
			}
			if ((keyState & KeyState.ShiftKey) != 0)
				return AppointmentDependencySelectionChangeAction.Add;
			if (!Control.AppointmentDependencySelectionController.IsAppointmentDependencySelected(dependency))
				return AppointmentDependencySelectionChangeAction.Select;
			return AppointmentDependencySelectionChangeAction.None;
		}
		protected internal virtual AppointmentDependencySelectionChangeAction CalculateSelectionChangeActionRightButtonPressed(AppointmentDependency dependency) {
			KeyState keyState = KeyState;
			if (Control.AppointmentDependencySelectionController.IsAppointmentDependencySelected(dependency))
				return AppointmentDependencySelectionChangeAction.None;
			if ((keyState & (KeyState.CtrlKey | KeyState.ShiftKey)) != 0)
				return AppointmentDependencySelectionChangeAction.Add;
			else
				return AppointmentDependencySelectionChangeAction.Select;
		}
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			if ((MouseButtonsState & (PlatformIndependentMouseButtons.Left | PlatformIndependentMouseButtons.Right)) == 0) {
				MouseHandler.SwitchToDefaultState();
			}
		}
		void AfterSelectDependency(bool dependencySelectionChanged) {
			if (Dependencies.Count == 0)
				return;
			Appointment startAppointment = Control.Storage.Appointments.GetAppointmentById(Dependencies[0].ParentId);
			ISchedulerHitInfo layoutHitInfo = pressedHitInfo.FindFirstLayoutHitInfo();
			if (dependencySelectionChanged || (!dependencySelectionChanged && layoutHitInfo.ViewInfo.Resource.Id != Control.Selection.Resource.Id))
				MouseHandler.SynchronizeSelectionWithAppointment(startAppointment, layoutHitInfo.ViewInfo.Resource);
		}
	}
	#endregion
	#region SchedulerDragPopupMenuInfo
	public class SchedulerDragPopupMenuInfo {
		SchedulerDragData dragData;
		AppointmentBaseCollection changedAppointments;
		public SchedulerDragPopupMenuInfo(SchedulerDragData dragData) {
			this.dragData = dragData;
			changedAppointments = new AppointmentBaseCollection();
		}
		public SchedulerDragData DragData { get { return dragData; } }
		public AppointmentBaseCollection ChangedAppointments { get { return changedAppointments; } }
	}
	#endregion
	#region AppointmentDragMouseHandlerStateBase (abstract class)
	public abstract class AppointmentDragMouseHandlerStateBase : SchedulerMouseHandlerState {
		protected AppointmentDragMouseHandlerStateBase(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal AppointmentChangeHelper AppointmentChangeHelper { get { return Control.AppointmentChangeHelper; } }
		protected internal abstract KeyboardHandler CreateKeyboardHandler();
		#region Start
		public override void Start() {
			base.Start();
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.AreAppointmentsDragged = true;
			Control.SetNewKeyboardHandler(CreateKeyboardHandler());
			Control.ActiveView.ExtendedVisibleInterval = Control.ActiveView.GetVisibleIntervals().Interval;
		}
		#endregion
		#region Finish
		public override void Finish() {
			ISetSchedulerStateService setStateService = MouseHandler.Control.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.AreAppointmentsDragged = false;
			Control.RestoreKeyboardHandler();
			Control.ActiveView.ExtendedVisibleInterval = TimeInterval.Empty;
			base.Finish();
		}
		#endregion
		#region OnDragCore
		protected internal virtual void OnDragCore(PlatformIndependentDragEventArgs e, ISchedulerHitInfo hitInfo) {
			if (SchedulerDragData.GetPresent(e.Data))
				DragAppointments(e, hitInfo);
			else
				e.Effect = PlatformIndependentDragDropEffects.None;
		}
		#endregion
		#region DragAppointments
		protected internal virtual void DragAppointments(PlatformIndependentDragEventArgs e, ISchedulerHitInfo hitInfo) {
			e.Effect = DragAppointments(new Point(e.X, e.Y), e.AllowedEffect, (KeyState)e.KeyState, hitInfo);
		}
		#endregion
		#region DragAppointments
		protected internal virtual PlatformIndependentDragDropEffects DragAppointments(Point pt, PlatformIndependentDragDropEffects allowedEffects, KeyState keyState, ISchedulerHitInfo hitInfo) {
			PlatformIndependentDragDropEffects result = GetDragDropEffect(allowedEffects, keyState, hitInfo);
			if (!DragAppointmentsCore(hitInfo, result))
				result = PlatformIndependentDragDropEffects.None;
			MouseHandler.ApplyChangesOnDragAppointments();
			return result;
		}
		#endregion
		#region GetDragDropEffect
		protected internal virtual PlatformIndependentDragDropEffects GetDragDropEffect(PlatformIndependentDragDropEffects allowedEffects, KeyState keyState, ISchedulerHitInfo hitInfo) {
			PlatformIndependentDragDropEffects result = PlatformIndependentDragDropEffects.None;
			if (CanDragTo(hitInfo)) {
				if ((keyState & KeyState.CtrlKey) != 0)
					result = PlatformIndependentDragDropEffects.Copy;
				else
					result = PlatformIndependentDragDropEffects.Move;
				result &= allowedEffects;
			}
			return result;
		}
		#endregion
		#region DragAppointmentsCore
		protected internal virtual bool DragAppointmentsCore(ISchedulerHitInfo hitInfo, PlatformIndependentDragDropEffects result) {
			if (result != PlatformIndependentDragDropEffects.None) {
				ISchedulerHitInfo layoutHitInfo = hitInfo.FindFirstLayoutHitInfo();
				return AppointmentChangeHelper.Drag(layoutHitInfo.ViewInfo, result, hitInfo.HitPoint);
			} else {
				AppointmentChangeHelper.UndoChanges();
				return false;
			}
		}
		#endregion
		#region CanDragTo
		protected internal virtual bool CanDragTo(ISchedulerHitInfo hitInfo) {
			return hitInfo.Contains(SchedulerHitTest.Cell | SchedulerHitTest.AllDayArea);
		}
		#endregion
	}
	#endregion
	public class SafeAppointment {
		Appointment appointment;
		ISchedulerStorageBase storage;
		object id;
		int occuranceIndex = -1;
		public SafeAppointment(Appointment apt)
			: this(apt, null) {
		}
		public SafeAppointment(Appointment apt, ISchedulerStorageBase storage) {
			this.appointment = apt;
			this.id = apt.Id;
			if (apt.IsOccurrence) {
				this.occuranceIndex = apt.RecurrenceIndex;
				this.id = apt.RecurrencePattern.Id;
			}
			this.storage = storage;
		}
		public Appointment Appointment {
			get {
				if (this.id != null && (this.appointment.IsDisposed || (this.appointment.RecurrencePattern != null && this.appointment.RecurrencePattern.IsDisposed))) {
					AppointmentBaseCollection appointments = GetAppointments();
					Appointment visibleAppointment = appointments.Find(apt => object.Equals(apt.Id, this.id));
					if (visibleAppointment == null)
						return null;
					if (this.occuranceIndex < 0)
						appointment = visibleAppointment;
					else
						appointment = visibleAppointment.GetOccurrence(this.occuranceIndex);
				}
				return appointment;
			}
		}
		protected virtual AppointmentBaseCollection GetAppointments() {
			return this.storage.Appointments.Items;
		}
	}
	public class SafeAppointmentCollection : List<SafeAppointment> {
		public SafeAppointmentCollection() {
		}
		public SafeAppointmentCollection(AppointmentBaseCollection appointments, ISchedulerStorageBase storage) {
			Guard.ArgumentNotNull(appointments, "appointments");
			int count = appointments.Count;
			for (int i = 0; i < count; i++)
				Add(new SafeAppointment(appointments[i], storage));
		}
	}
	#region AppointmentInternalDragMouseHandlerStateBase (abstract class)
	public abstract class AppointmentInternalDragMouseHandlerStateBase : AppointmentDragMouseHandlerStateBase {
		readonly SafeAppointment appointment;
		SchedulerDragPopupMenuInfo dragPopupMenuInfo;
		ISchedulerHitInfo hitInfo;
		readonly ISchedulerHitInfo prevHitInfo;
		protected AppointmentInternalDragMouseHandlerStateBase(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(mouseHandler) {
			this.appointment = appointment;
			this.hitInfo = hitInfo;
			this.prevHitInfo = prevHitInfo;
			IsInternalState = true;
		}
		internal SafeAppointment Appointment { get { return appointment; } }
		internal ISchedulerHitInfo HitInfo { get { return hitInfo; } }
		internal ISchedulerHitInfo PrevHitInfo { get { return prevHitInfo; } }
		internal SchedulerDragPopupMenuInfo DragPopupMenuInfo { get { return dragPopupMenuInfo; } }
		public abstract void StartDragCore(SchedulerDragData data, ISchedulerHitInfo startHitInfo);
		#region Start
		public override void Start() {
			base.Start();
			hitInfo = hitInfo.FindHitInfo(SchedulerHitTest.AllDayArea | SchedulerHitTest.Cell);
			if (!CanStart(hitInfo.HitTest)) {
				MouseHandler.SwitchToDefaultState();
				return;
			}
			SchedulerDragData dragData = CreateDragData(appointment);
			if (dragData == null) {
				MouseHandler.SwitchToDefaultState();
				return;
			}
			PrepareControlForDrag();
			dragPopupMenuInfo = new SchedulerDragPopupMenuInfo(dragData);
			AppointmentChangeHelper.BeginInternalDragDrop(dragData, prevHitInfo);
			StartDragCore(dragData, hitInfo);
		}
		protected virtual bool CanStart(SchedulerHitTest schedulerHitTest) {
			return hitInfo.HitTest != SchedulerHitTest.None;
		}
		#endregion
		#region OnCancelState
		public override void OnCancelState() {
			AppointmentChangeHelper.CancelChanges();
			MouseHandler.SwitchToDefaultState();
			PrepareControlAfterDrag();
		}
		#endregion
		#region CreateDragData
		protected internal virtual SchedulerDragData CreateDragData(SafeAppointment primaryAppointment) {
			AppointmentSelectionController appointmentSelectionController = Control.AppointmentSelectionController;
			int primaryAppointmentIndex = appointmentSelectionController.IndexOfSelectedAppointment(primaryAppointment.Appointment);
			if (primaryAppointmentIndex < 0)
				return null;
			appointmentSelectionController.BeginUpdate();
			int selectedAppointmentsCount = appointmentSelectionController.SelectedAppointments.Count;
			for (int i = selectedAppointmentsCount - 1; i >= 0; i--) {
				if (i == primaryAppointmentIndex)
					continue;
				Appointment appointment = appointmentSelectionController.SelectedAppointments[i];
				if (!MouseHandler.AppointmentOperationHelper.CanDragAppointment(appointment))
					appointmentSelectionController.SelectedAppointments.RemoveAt(i);
			}
			appointmentSelectionController.EndUpdate();
			primaryAppointmentIndex = appointmentSelectionController.IndexOfSelectedAppointment(primaryAppointment.Appointment);
			return primaryAppointmentIndex >= 0 ? new SchedulerDragData(Control.SelectedAppointments, primaryAppointmentIndex) : null;
		}
		#endregion
		#region CreatePopupMenuBuilder
		public override SchedulerPopupMenuBuilder CreatePopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return MouseHandler.CreateDragPopupMenuBuilder(hitInfo, DragPopupMenuInfo);
		}
		#endregion
		#region GetCursor
		protected virtual PlatformIndependentCursor GetCursor(PlatformIndependentDragDropEffects effects) {
#if (!SL)
			return DragAndDropCursors.GetCursor(effects);
#else
			return CursorTypes.Default;
#endif
		}
		#endregion
		protected virtual void PrepareControlForDrag() { 
		}
		protected virtual void PrepareControlAfterDrag() { 
		}
	}
	#endregion
	#region AppointmentMouseDragMouseHandlerState
	public class AppointmentMouseDragMouseHandlerState : AppointmentInternalDragMouseHandlerStateBase {
		PlatformIndependentDragDropEffects lastEffect;
		public AppointmentMouseDragMouseHandlerState(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(appointment, mouseHandler, hitInfo, prevHitInfo) {
		}
#if !SILVERLIGHT
		protected virtual PlatformIndependentMouseButtons MouseButtons { get { return System.Windows.Forms.Control.MouseButtons; } }
#else
		protected virtual PlatformIndependentMouseButtons MouseButtons { get { return Control.Owner.LeftButtonPressed ? PlatformIndependentMouseButtons.Left : PlatformIndependentMouseButtons.None; } }
#endif
		internal PlatformIndependentDragDropEffects LastEffect { get { return lastEffect; } }
		public override void OnCancelState() {
			MouseHandler.SetCurrentCursor(CursorTypes.Default);
			base.OnCancelState();
		}
		#region StartDragCore
		public override void StartDragCore(SchedulerDragData dragData, ISchedulerHitInfo startHitInfo) {
			lastEffect = DragAppointments(Point.Empty, startHitInfo);
		}
		#endregion
		#region OnMouseMove
		public override void OnMouseMove(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			lastEffect = DragAppointments(new Point(e.X, e.Y), hitInfo);
		}
		#endregion
		#region OnMouseUp
		public override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			lastEffect = DragAppointments(new Point(e.X, e.Y), hitInfo);
			if ((MouseButtons & (PlatformIndependentMouseButtons.Left | PlatformIndependentMouseButtons.Right)) != 0)
				return;
			MouseHandler.Suspend();
			try {
				AppointmentChangeHelper.CommitDrag(lastEffect, false);
			} finally {
				MouseHandler.Resume();
				MouseHandler.SetCurrentCursor(CursorTypes.Default);
				MouseHandler.SwitchToDefaultState();
			}
		}
		#endregion
		#region OnPopupMenu
		public override bool OnPopupMenu(Point pt, ISchedulerHitInfo hitInfo) {
#if !SILVERLIGHT && !WPF
			if ((MouseButtons & PlatformIndependentMouseButtons.Left) != 0)
				return false;
			DragPopupMenuInfo.ChangedAppointments.AddRange(AppointmentChangeHelper.GetChangedAppointments());
			AppointmentChangeHelper.CancelChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			return true;
#endif
		}
		#endregion        
		#region OnPopupMenuShowing
		public override bool OnPopupMenuShowing() {
			if ((MouseButtons & (PlatformIndependentMouseButtons.Left | PlatformIndependentMouseButtons.Right)) != 0)
				return false;
			AppointmentChangeHelper.CancelChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			MouseHandler.SwitchToDefaultState();
			return true;
		}
		#endregion
		public override void OnMouseDown(PlatformIndependentMouseEventArgs e) {
		}
		#region DragAppointments
		protected internal virtual PlatformIndependentDragDropEffects DragAppointments(Point mousePosition, ISchedulerHitInfo layoutHitInfo) {
			layoutHitInfo = layoutHitInfo.FindFirstLayoutHitInfo();
			PlatformIndependentDragDropEffects effects = DragAppointments(mousePosition, PlatformIndependentDragDropEffects.All, KeyboardHandler.KeyState, layoutHitInfo);
			MouseHandler.SetCurrentCursor(GetCursor(effects));
			return effects;
		}
		#endregion
		#region OnKeyStateChanged
		public override void OnKeyStateChanged(KeyState keyState) {
			if (lastEffect == PlatformIndependentDragDropEffects.None)
				return;
			if ((keyState & KeyState.CtrlKey) != 0)
				lastEffect = PlatformIndependentDragDropEffects.Copy;
			else
				lastEffect = PlatformIndependentDragDropEffects.Move;
			MouseHandler.SetCurrentCursor(GetCursor(lastEffect));
		}
		#endregion
		#region CreateKeyboardHandler
		protected internal override KeyboardHandler CreateKeyboardHandler() {
			return new KeyStateKeyboardHandler(this);
		}
		#endregion
	}
	#endregion
	#region AppointmentExternalDragAndDropMouseHandlerState
	public class AppointmentExternalDragAndDropMouseHandlerState : AppointmentDragMouseHandlerStateBase {
		#region Fields
		protected static readonly int CtrlKey = 8;
		SchedulerDragData dragData;
		PlatformIndependentDragEventArgs dragEventArgs;
		ISchedulerHitInfo hitInfo;
		#endregion
		public AppointmentExternalDragAndDropMouseHandlerState(PlatformIndependentDragEventArgs dragEventArgs, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo)
			: base(mouseHandler) {
			dragData = SchedulerDragData.GetData(dragEventArgs.Data);
			if (dragData == null)
				dragEventArgs.Effect = PlatformIndependentDragDropEffects.None;
			this.dragEventArgs = dragEventArgs;
			this.hitInfo = hitInfo;
		}
		#region Properties
		internal SchedulerDragData DragData { get { return dragData; } }
		internal PlatformIndependentDragEventArgs DragEventArgs { get { return dragEventArgs; } }
		internal ISchedulerHitInfo HitInfo { get { return hitInfo; } }
		#endregion
		public override void Start() {
			base.Start();
			if (dragData != null) {
				AppointmentChangeHelper.BeginExternalDragDrop(dragData);
				DragAppointments(dragEventArgs, hitInfo);
			} else {
				MouseHandler.SwitchToDefaultState();
				dragEventArgs.Effect = PlatformIndependentDragDropEffects.None;
			}
		}
		protected internal override KeyboardHandler CreateKeyboardHandler() {
			return new EmptyKeyboardHandler();
		}
		public override void OnDragOver(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			OnDragCore(e, hitInfo);
		}
		public override void OnDragDrop(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			OnDragCore(e, hitInfo);
			try {
				try {
					AppointmentChangeHelper.CommitDrag(e.Effect, false);
				} finally {
					MouseHandler.SwitchToDefaultState();
				}
			} finally {
				Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			}
		}
		public override void OnDragLeave() {
			AppointmentChangeHelper.CancelChanges();
			MouseHandler.SwitchToDefaultState();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
	}
	#endregion
	#region AppointmentExternalDragAndDropOnExternalControlMouseHandlerState
	public class AppointmentExternalDragAndDropOnExternalControlMouseHandlerState : AppointmentDragMouseHandlerStateBase {
		SchedulerDragData dragData;
		public AppointmentExternalDragAndDropOnExternalControlMouseHandlerState(SchedulerDragData dragData, SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
			if (dragData == null)
				Exceptions.ThrowArgumentNullException("dragData");
			this.dragData = dragData;
		}
		public override void Start() {
			base.Start();
			AppointmentChangeHelper.BeginExternalDragDrop(dragData);
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
			bool result = changedAppointments != null ? OnDragOverExternalControl(dragData, changedAppointments, copy, true) : false;
			try {
				if (result) {
					PlatformIndependentDragDropEffects effect = copy ? PlatformIndependentDragDropEffects.Copy : PlatformIndependentDragDropEffects.Move;
					AppointmentChangeHelper.CommitDrag(effect, true);
				} else
					AppointmentChangeHelper.CommitDrag(PlatformIndependentDragDropEffects.None, true);
			} finally {
				MouseHandler.SwitchToDefaultState();
				Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			}
			return result;
		}
		public override void OnDragLeaveExternalControl() {
			AppointmentChangeHelper.CancelChanges();
			MouseHandler.SwitchToDefaultState();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		protected internal override KeyboardHandler CreateKeyboardHandler() {
			return new EmptyKeyboardHandler();
		}
	}
	#endregion
	public interface ISchedulerOfficeScroller {
		int HorizontalScrollValue { get; }
		int VerticalScrollValue { get; }
		void ScrollVertical(int delta);
		void ScrollHorizontal(int delta);
		void ScrollVerticalByPixel(int delta);
		void ScrollResource(int delta);
	}
}
