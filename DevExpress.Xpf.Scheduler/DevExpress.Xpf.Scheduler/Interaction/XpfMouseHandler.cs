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
using System.Windows;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Menu;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Xpf.Scheduler.Internal;
#if !SILVERLIGHT
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentDragDropEffectsWindowsForms = System.Windows.Forms.DragDropEffects;
using PlatformIndependentDragDropEffectsWindows = System.Windows.DragDropEffects;
using PlatformIndependentQueryContinueDragEventArgs = System.Windows.Forms.QueryContinueDragEventArgs;
using PlatformIndependentDragAction = System.Windows.Forms.DragAction;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using System.Windows.Interop;
using DevExpress.Xpf.Core.Core.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentDragAction = DevExpress.Utils.DragAction;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
using PlatformIndependentQueryContinueDragEventArgs = DevExpress.Utils.QueryContinueDragEventArgs;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using PlatformIndependentDragDropEffectsWindowsForms = DevExpress.Utils.DragDropEffects;
using PlatformIndependentDragDropEffectsWindows = DevExpress.Utils.DragDropEffects;
using DevExpress.Xpf.Core.Core.Native;
#endif
#if !SL
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerControlHitInfo : ISchedulerHitInfo {
		internal static SchedulerControlHitInfo Create(ISchedulerHitInfo hitInfo) {
			return new SchedulerControlHitInfo(hitInfo);
		}
		ISchedulerHitInfo innerHitInfo;
		protected SchedulerControlHitInfo(ISchedulerHitInfo hitInfo) {
			this.innerHitInfo = hitInfo;
		}
		public System.Drawing.Point HitPoint { get { return this.innerHitInfo.HitPoint; } }
		public SchedulerHitTest HitTest { get { return this.innerHitInfo.HitTest; } }
		public ISchedulerHitInfo NextHitInfo { get { return this.innerHitInfo.NextHitInfo; } }
		public ISelectableIntervalViewInfo ViewInfo { get { return this.innerHitInfo.ViewInfo; } }
		public bool Contains(SchedulerHitTest types) {
			return this.innerHitInfo.Contains(types);
		}
		public ISchedulerHitInfo FindFirstLayoutHitInfo() {
			return this.innerHitInfo.FindFirstLayoutHitInfo();
		}
		public ISchedulerHitInfo FindHitInfo(SchedulerHitTest types) {
			return this.innerHitInfo.FindHitInfo(types);
		}
		public ISchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
			return this.innerHitInfo.FindHitInfo(types, stopTypes);
		}	   
	}
}
#endif
namespace DevExpress.XtraScheduler.Native {
	public class XpfSchedulerMouseHandler : SchedulerMouseHandler {
		readonly static Point EmptyPoint = new Point(0, 0);
		Point lastDragPoint = EmptyPoint;
		Point lastMousePoint = EmptyPoint;
		DragDropKeyStates lastKeyStates = DragDropKeyStates.None;
		readonly SchedulerControl control;
		readonly XpfSchedulerMenuBuilderUIFactory uiFactory;
		PlatformIndependentDragDropEffectsWindows lastDragDropEffects = PlatformIndependentDragDropEffectsWindows.None;
		public XpfSchedulerMouseHandler(SchedulerControl control)
			: base(control.InnerControl) {
			this.control = control;
			this.uiFactory = new XpfSchedulerMenuBuilderUIFactory();
		}
		protected internal Point LastMousePoint { get { return lastMousePoint; } set { lastMousePoint = value; } }
		protected internal override ISelectableIntervalViewInfo EmptySelectableIntervalViewInfo { get { return SchedulerHitInfo.None.ViewInfo; } }
		internal SchedulerControl XpfControl { get { return control; } }
		protected internal override IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> UiFactory { get { return uiFactory; } }
		public PlatformIndependentDragDropEffectsWindows LastDragDropEffects { get { return lastDragDropEffects; } set { lastDragDropEffects = value; } }
		protected internal override void UpdateHotTrack(ISelectableIntervalViewInfo viewInfo) {
		}
		protected override void CalculateAndSaveHitInfo(PlatformIndependentMouseEventArgs e) {
			this.CurrentHitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(XpfControl, e);
		}
		public override bool OnPopupMenu(System.Drawing.Point pt) {
			return true;
		}
		#region ScrollBackward
		protected internal override void ScrollBackward(PlatformIndependentMouseEventArgs e) {
			if (Suspended)
				return;
			if (XpfControl.DateTimeScrollBarObject == null)
				return;
			IScrollBarAdapter scrollBarAdapter = XpfControl.DateTimeScrollBarObject.ScrollBarAdapter;
			XpfControl.DateTimeScrollController.Scroll(scrollBarAdapter, -1, true);
			this.lastMousePoint = new Point(e.X, e.Y);
			OnDateTimeScroll(e);
		}
		#endregion
		#region ScrollForward
		protected internal override void ScrollForward(PlatformIndependentMouseEventArgs e) {
			if (Suspended)
				return;
			if (XpfControl.DateTimeScrollBarObject == null)
				return;
			IScrollBarAdapter scrollBarAdapter = XpfControl.DateTimeScrollBarObject.ScrollBarAdapter;
			XpfControl.DateTimeScrollController.Scroll(scrollBarAdapter, 1, true);
			this.lastMousePoint = new Point(e.X, e.Y);
			OnDateTimeScroll(e);
		}
		#endregion
		#region OnDateTimeScroll
		public virtual void OnDateTimeScroll(PlatformIndependentMouseEventArgs e) {
			XpfControl.InplaceEditController.DoCommit();
			lastDragPoint = EmptyPoint;
			ISchedulerHitInfo hitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(XpfControl, e);
			State.OnDateTimeScroll(System.Drawing.Point.Empty, hitInfo);
		}
		#endregion
		protected override void StartOfficeScroller(System.Drawing.Point clientPoint) {
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return new SchedulerOfficeScroller(XpfControl);
		}		
		protected internal override MouseHandlerState CreateDefaultState() {
			return new DefaultMouseHandlerXpfState(this);
		}
		protected internal override MouseHandlerState CreateAppointmentDragState(Appointment appointment, ISchedulerHitInfo layoutHitInfo, ISchedulerHitInfo prevLayoutHitInfo) {
			XtraSchedulerDebug.Assert(XpfControl.AllowDrop == true);
			if (XpfControl.DragDropOptions.DragDropMode == DragDropMode.Standard && !IsBrowserHosted() && !((IMouseKeyStateSupport)XpfControl).RightButtonPressed)
				return new AppointmentInternalDragAndDropMouseHandlerState(new SafeAppointment(appointment, Control.Storage), this, layoutHitInfo, prevLayoutHitInfo);
			else
				return new XpfAppointmentMouseDragMouseHandlerState(new SafeAppointment(appointment, Control.Storage), this, layoutHitInfo, prevLayoutHitInfo);
		}
		bool IsBrowserHosted() {
#if WPF
			return BrowserInteropHelper.IsBrowserHosted;
#else
			return true;
#endif
		}
		protected internal override void SwitchToExternalDragAndDropState(System.Drawing.Point mousePosition, ISchedulerHitInfo layoutHitInfo, PlatformIndependentDragEventArgs dragEventArgs) {
			MouseHandlerState newState = new XpfAppointmentExternalDragAndDropMouseHandlerState(dragEventArgs, this, layoutHitInfo);
			SwitchStateCore(newState, mousePosition);
		}
		protected internal override void Redraw() {
		}
		public override SchedulerPopupMenuBuilder CreateDefaultPopupMenuBuilder(ISchedulerHitInfo hitInfo) {
			return new SchedulerDefaultPopupMenuWpfBuilder(UiFactory, XpfControl, hitInfo);
		}
		protected internal override void SetCurrentCursor(PlatformIndependentCursor cursor) {
#if WPF
				System.Windows.Input.Mouse.OverrideCursor = ConvertToPlatformCursor(cursor);
#endif
		}
#if WPF
		protected virtual System.Windows.Input.Cursor ConvertToPlatformCursor(PlatformIndependentCursor cursor) {
			if (cursor == System.Windows.Forms.Cursors.SizeNS)
				return System.Windows.Input.Cursors.SizeNS;
			if (cursor == System.Windows.Forms.Cursors.SizeWE)
				return System.Windows.Input.Cursors.SizeWE;
			return null;
		}
#endif
		protected internal override void UpdateActiveViewSelection(SchedulerViewSelection selection) {
			((IInnerSchedulerViewOwner)XpfControl.ActiveView).UpdateSelection(selection);
		}
		protected internal override void ApplyChangesOnDragAppointments() {
			Control.ApplyChangesCore(SchedulerControlChangeType.SelectionChanged, ChangeActions.RaiseSelectionChanged);
			Control.ApplyChanges(SchedulerControlChangeType.AppointmentDragging);
		}
		public virtual void OnDragEnter(System.Windows.DragEventArgs e) {
			this.lastDragPoint = EmptyPoint;
			this.lastKeyStates = DragDropKeyStates.None;
			Point pt = e.GetPosition(XpfControl);
			CalculateAndSaveHitInfo(pt);
			AutoScroller.Resume();
			State.OnDragEnter(XpfMouseUtils.ConverToNativeDragEventArgs(e, XpfControl));
		}
		public virtual void OnDragOver(System.Windows.DragEventArgs e) {
			if (Suspended)
				return;
			Point pt = e.GetPosition(XpfControl);
			PlatformIndependentDragEventArgs nativeEventArgs = XpfMouseUtils.ConverToNativeDragEventArgs(e, XpfControl);
			DragDropKeyStates keyStates = (DragDropKeyStates)nativeEventArgs.KeyState;
			if (pt != lastDragPoint || keyStates != lastKeyStates) {
				CalculateAndSaveHitInfo(pt);
				State.OnDragOver(nativeEventArgs);
				AutoScroller.OnMouseMove(XpfTypeConverter.FromPlatformPoint(pt));
				this.lastDragPoint = pt;
				this.lastMousePoint = pt;
				this.lastKeyStates = keyStates;
				LastDragDropEffects = (PlatformIndependentDragDropEffectsWindows)nativeEventArgs.Effect;
			}
#if WPF
			e.Effects = LastDragDropEffects;
#endif
		}
		public virtual void OnDragDrop(System.Windows.DragEventArgs e) {
			if (Suspended)
				return;
			this.lastDragPoint = EmptyPoint;
			this.lastKeyStates = DragDropKeyStates.None;
			Point pt = e.GetPosition(XpfControl);
			CalculateAndSaveHitInfo(pt);
			PlatformIndependentDragEventArgs nativeEventArgs = XpfMouseUtils.ConverToNativeDragEventArgs(e, XpfControl);
			State.OnDragDrop(nativeEventArgs);
#if (WPF)
			e.Effects = (PlatformIndependentDragDropEffectsWindows)nativeEventArgs.Effect;
#endif
		}
		public virtual void OnDragLeave(EventArgs e) {
			if (Suspended)
				return;
			this.lastDragPoint = EmptyPoint;
			this.lastKeyStates = DragDropKeyStates.None;
			AutoScroller.Suspend();
			State.OnDragLeave();
		}
		protected virtual void CalculateAndSaveHitInfo(Point pt) {
			CurrentHitInfo = CalculateHitInfo(pt);
		}
		protected internal virtual ISchedulerHitInfo CalculateHitInfo(Point pt) {
			ISchedulerHitInfo hitInfo = SchedulerHitInfo.CreateSchedulerHitInfo(XpfControl, pt);
			return hitInfo;
		}
		protected override AutoScroller CreateAutoScroller() {
			return new SchedulerAutoScroller(this);
		}
		protected override bool IsSelectableIntervalViewInfoEquals(ISelectableIntervalViewInfo info1, ISelectableIntervalViewInfo info2) {
			VisualSelectableIntervalViewInfo visualInfo1 = info1 as VisualSelectableIntervalViewInfo;
			if (visualInfo1 != null)
				return visualInfo1.EqualsIgnoreSelection(info2 as VisualSelectableIntervalViewInfo);
			return base.IsSelectableIntervalViewInfoEquals(info1, info2);
		}
		protected override AppointmentMouseDownState CreateAppointmentMouseDownState(System.Drawing.Point mousePosition, Appointment appointment, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo) {
			return new SchedulerAppointmentMouseDownState(this, appointment, mousePosition, mouseButtons, hitInfo);
		}
		protected internal void ScrollResourceForward() {
			ISchedulerOfficeScroller officeScroller = OfficeScroller as ISchedulerOfficeScroller;
			officeScroller.ScrollVertical(1);
		}
		protected internal void ScrollResourceBackward() {
			ISchedulerOfficeScroller officeScroller = OfficeScroller as ISchedulerOfficeScroller;
			officeScroller.ScrollVertical(-1);
		}
	}
	public class SchedulerAppointmentMouseDownState : AppointmentMouseDownState {
		public SchedulerAppointmentMouseDownState(XpfSchedulerMouseHandler mouseHandler, Appointment appointment, System.Drawing.Point mousePosition, PlatformIndependentMouseButtons mouseButtons, ISchedulerHitInfo hitInfo)
			: base(mouseHandler, appointment, mousePosition, mouseButtons, hitInfo) {
		}
		public XpfSchedulerMouseHandler SchedulerMouseHandler { get { return (XpfSchedulerMouseHandler)MouseHandler; } }
		protected internal override bool CanStartDrag(System.Drawing.Point currentMousePosition) {
			if (PressedMouseButtons != PlatformIndependentMouseButtons.Left)
				return false;
			if (!SchedulerMouseHandler.XpfControl.AllowDrop && SchedulerMouseHandler.XpfControl.DragDropOptions.DragDropMode == DragDropMode.Standard)
				return false;
			return base.CanStartDrag(currentMousePosition);
		}
	}
	public class DefaultMouseHandlerXpfState : DefaultMouseHandlerState {
		public DefaultMouseHandlerXpfState(SchedulerMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override bool TryToHandleButtonsClick(ISchedulerHitInfo hitInfo) {
			return false;
		}
	}
	#region XpfAppointmentMouseDragMouseHandlerState
	public class XpfAppointmentMouseDragMouseHandlerState : AppointmentMouseDragMouseHandlerState {
		readonly IDraggedAppointmentContainer draggedAppointmentContainer;
		public XpfAppointmentMouseDragMouseHandlerState(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(appointment, mouseHandler, hitInfo, prevHitInfo) {
			this.draggedAppointmentContainer = CreateDraggedAppointmentContainer();
		}
		SchedulerControl XpfControl { get { return (SchedulerControl)Control.Owner; } }
		public IDraggedAppointmentContainer DraggedAppointmentContainer { get { return draggedAppointmentContainer; } }
#if WPF
		protected override PlatformIndependentCursor GetCursor(PlatformIndependentDragDropEffectsWindowsForms effects) {
			return PlatformIndependentCursor.Current;
		}
#else
		protected override PlatformIndependentCursor GetCursor(PlatformIndependentDragDropEffectsWindows effects) {
			return System.Windows.Input.Cursors.Arrow;
		}
#endif
		IDraggedAppointmentContainer CreateDraggedAppointmentContainer() {
			if (XpfControl.DragDropOptions.MovementType == MovementType.Smooth)
				return new DraggedAppointmentContainer(XpfControl, this);
			return new EmptyDraggedAppointmentContainer();
		}
		public override void StartDragCore(SchedulerDragData dragData, ISchedulerHitInfo startHitInfo) {
			base.StartDragCore(dragData, startHitInfo);
		}
		protected internal override bool DragAppointmentsCore(ISchedulerHitInfo hitInfo, PlatformIndependentDragDropEffectsWindowsForms effects) {
			bool result = base.DragAppointmentsCore(hitInfo, effects);
			if (CanDragTo(hitInfo)) {
				System.Drawing.Point pt = hitInfo.HitPoint;
				((XpfSchedulerMouseHandler)MouseHandler).LastMousePoint = new Point(pt.X, pt.Y);
				DraggedAppointmentContainer.Move(PrevHitInfo.ViewInfo as VisualAppointmentViewInfo, pt, effects);
			} else
				DraggedAppointmentContainer.Hide();
			return result;
		}
		public override void Finish() {
			base.Finish();
			DraggedAppointmentContainer.Close();
			((XpfSchedulerMouseHandler)MouseHandler).LastMousePoint = new Point(0, 0);
		}
		public override bool OnPopupMenuShowing() {
			return base.OnPopupMenuShowing();
		}
		public override void OnDragDrop(PlatformIndependentDragEventArgs e) {
			base.OnDragDrop(e);
			DragPopupMenuInfo.ChangedAppointments.AddRange(AppointmentChangeHelper.GetChangedAppointments());
			AppointmentChangeHelper.CancelChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			XpfControl.OnPopupMenu(new System.Drawing.Point(10, 10));
		}
	}
	#endregion
	#region AppointmentInternalDragAndDropMouseHandlerState
	public class AppointmentInternalDragAndDropMouseHandlerState : AppointmentInternalDragMouseHandlerStateBase {
		#region Fields
		bool rightMouseDrop;
		bool externalDrop;
		const int KeyStateRightMouseButton = 2;
		readonly IDraggedAppointmentContainer draggedAppointmentContainer;
		#endregion
		public AppointmentInternalDragAndDropMouseHandlerState(SafeAppointment appointment, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo, ISchedulerHitInfo prevHitInfo)
			: base(appointment, mouseHandler, hitInfo, prevHitInfo) {
			this.draggedAppointmentContainer = CreateDraggedAppointmentContainer();
		}
		#region Properties
		protected internal bool RightMouseDrop { get { return rightMouseDrop; } }
		SchedulerControl XpfControl { get { return (SchedulerControl)Control.Owner; } }
		public IDraggedAppointmentContainer DraggedAppointmentContainer { get { return draggedAppointmentContainer; } }
		#endregion
		IDraggedAppointmentContainer CreateDraggedAppointmentContainer() {
			if (XpfControl.DragDropOptions.MovementType == MovementType.Smooth)
				return new DraggedAppointmentContainer(XpfControl, this);
			return new EmptyDraggedAppointmentContainer();
		}
		public override void StartDragCore(SchedulerDragData data, ISchedulerHitInfo startHitInfo) {
			PlatformIndependentDragDropEffectsWindowsForms dropEffect = DoDragDrop(data);
			try {
				AppointmentChangeHelper.CommitDrag(dropEffect, externalDrop);
			} finally {
				DraggedAppointmentContainer.Close();
				MouseHandler.SwitchToDefaultState();
			}
		}
		protected virtual PlatformIndependentDragDropEffectsWindowsForms DoDragDrop(SchedulerDragData data) {
#if (WPF)
			System.Windows.DragDropEffects result = DragDrop.DoDragDrop(XpfControl, new DataObject(data), PlatformIndependentDragDropEffectsWindows.All);
			return (System.Windows.Forms.DragDropEffects)result;
#else
			return PlatformIndependentDragDropEffectsWindows.All;
#endif
		}
		public override void OnDragOver(PlatformIndependentDragEventArgs e) {
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			OnDragCore(e, hitInfo);
			if (CanDragTo(hitInfo))
				DraggedAppointmentContainer.Move(PrevHitInfo.ViewInfo as VisualAppointmentViewInfo, hitInfo.HitPoint, e.Effect);
			else
				DraggedAppointmentContainer.Hide();
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
		public override bool OnDragDropExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, System.Drawing.Point pt, bool copy, bool showPopupMenu) {
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
				e.Effect = PlatformIndependentDragDropEffectsWindowsForms.None;
			}
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
		}
		public override void OnDragLeave() {
			AppointmentChangeHelper.UndoChanges();
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout);
			DraggedAppointmentContainer.Hide();
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
	}
	#endregion
	#region XpfAppointmentExternalDragAndDropMouseHandlerState
	public class XpfAppointmentExternalDragAndDropMouseHandlerState : AppointmentExternalDragAndDropMouseHandlerState {
		readonly IDraggedAppointmentContainer draggedAppointmentContainer;
		public XpfAppointmentExternalDragAndDropMouseHandlerState(PlatformIndependentDragEventArgs dragEventArgs, SchedulerMouseHandler mouseHandler, ISchedulerHitInfo hitInfo)
			: base(dragEventArgs, mouseHandler, hitInfo) {
			this.draggedAppointmentContainer = CreateDraggedAppointmentContainer();
		}
		SchedulerControl XpfControl { get { return (SchedulerControl)Control.Owner; } }
		public IDraggedAppointmentContainer DraggedAppointmentContainer { get { return draggedAppointmentContainer; } }
		IDraggedAppointmentContainer CreateDraggedAppointmentContainer() {
			if (XpfControl.DragDropOptions.MovementType == MovementType.Smooth)
				return new ExternalDraggedAppointmentContainer(XpfControl, this);
			return new EmptyDraggedAppointmentContainer();
		}
		public override void Start() {
			base.Start();
			XpfAppointmentChangeHelper changeHelper = AppointmentChangeHelper as XpfAppointmentChangeHelper;
			if (changeHelper == null)
				return;
		}
		public override void Finish() {
			base.Finish();
			DraggedAppointmentContainer.Close();
		}
		public override void OnDragOver(PlatformIndependentDragEventArgs e) {
			base.OnDragOver(e);
			ISchedulerHitInfo hitInfo = MouseHandler.CurrentHitInfo;
			if (CanDragTo(hitInfo))
				DraggedAppointmentContainer.Move(null, hitInfo.HitPoint, e.Effect);
			else
				DraggedAppointmentContainer.Hide();
		}
	}
	#endregion
}
