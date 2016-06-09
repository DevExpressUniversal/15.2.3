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
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public class RegularDragServiceState : DragServiceState {
		public RegularDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Regular; }
		}
		internal bool isDragging, isReordering, isResizing, isFloating, isScrolling;
		public override void ProcessMouseDown(IUIView view, Point point) {
			LayoutElementHitInfo hitInfo = view.Adapter.CalcHitInfo(view, point);
			ILayoutElement dragItem = view.GetDragItem(hitInfo.Element);
			if(dragItem == null || hitInfo.InControlBox) return;
			if(hitInfo.InReorderingBounds) {
				isReordering = InitializeOperation(view, point, dragItem, OperationType.Reordering);
				if(isReordering) return;
			}
			if(hitInfo.InDragBounds) {
				isFloating = InitializeOperation(view, point, dragItem, OperationType.Floating);
				if(isFloating) return;
				isDragging = InitializeOperation(view, point, dragItem, OperationType.FloatingMoving);
				if(isDragging) return;
			}
			if(hitInfo.InResizeBounds) {
				isResizing = InitializeOperation(view, point, dragItem, OperationType.Resizing);
			}
			if(hitInfo.InScrollBounds) {
				isScrolling = InitializeOperation(view, point, dragItem, OperationType.Scrolling);
			}
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			IUIView dragSource = DragService.DragSource;
			if(dragSource == null) return;
			ILayoutElement dragItem = DragService.DragItem;
			Point dragOrigin = DragService.DragOrigin;
			Point screenPoint = view.ClientToScreen(point);
			Point dragSourcePoint = dragSource.ScreenToClient(screenPoint);
			int threshold = isResizing || isScrolling ? 1 : 5;
			if(DragServiceHelper.IsOutOfArea(screenPoint, dragOrigin, threshold)) {
				if(isFloating) {
					if(BeginOperation(OperationType.Floating, dragSource, dragItem, screenPoint)) return;
				}
				if(isDragging) {
					if(dragSource.Type == HostType.Floating) {
						if(BeginOperation(OperationType.FloatingMoving, dragSource, dragItem, screenPoint)) return;
					}
				}
				if(isReordering) {
					if(BeginOperation(OperationType.Reordering, dragSource, dragItem, dragSourcePoint)) return;
				}
				if(isResizing) {
					if(BeginOperation(OperationType.Resizing, dragSource, dragItem, dragSourcePoint)) return;
				}
				if(isScrolling) {
					if(BeginOperation(OperationType.Scrolling, dragSource, dragItem, dragSourcePoint)) return;
				}
			}
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DragService.Reset();
			CancelCore();
		}
		public override void ProcessCancel(IUIView view) {
			base.ProcessCancel(view);
			CancelCore();
		}
		protected void CancelCore() {
			isDragging = isReordering = isFloating = isResizing = isScrolling = false;
		}
	}
	public class FloatingDragServiceState : DragServiceState {
		public FloatingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Floating; }
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			Point screenPoint = view.ClientToScreen(point);
			IUIView floatingView = view.Adapter.GetView(DragService.DragItem);
			if(floatingView != null && floatingView.Type == HostType.Floating) {
				DragService.DragSource = floatingView;
				floatingView.Adapter.UIInteractionService.Activate(floatingView);
				BeginOperation(OperationType.FloatingMoving, floatingView, DragService.DragItem, screenPoint);
				return;
			}
			DragService.SetState(OperationType.Regular);
		}
	}
	public class ReorderingDragServiceState : DragServiceState {
		public ReorderingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Reordering; }
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
	public class DockingDragServiceState : DragServiceState {
		public DockingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Docking; }
		}
		IUIView lastTargetView;
		OperationType lastTargetViewOperation = OperationType.Regular;
		protected override IUIView GetBehindView() {
			return lastTargetView;
		}
		protected override OperationType GetBehindOperation() {
			return lastTargetViewOperation;
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			DoDragging(view, point);
			if(DragService.SuspendBehindDragging) return;
			OperationType targetOperation = OperationType.Regular;
			LayoutElementHitInfo targetHitInfo = view.Adapter.CalcHitInfo(view, point);
			if(targetHitInfo.InReorderingBounds && CheckReordering(view, point))
				targetOperation = OperationType.Reordering;
			if(lastTargetView != view || lastTargetViewOperation != targetOperation) {
				TryLeaveAndEnter(view, lastTargetViewOperation, view, point, targetOperation);
				lastTargetView = view;
				lastTargetViewOperation = targetOperation;
			}
			DoDragging(view, point, targetOperation);
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DoDrop(view, point);
			if(!view.IsDisposing && !DragService.SuspendBehindDragging) {
				LayoutElementHitInfo targetHitInfo = view.Adapter.CalcHitInfo(view, point);
				DoDrop(view, point, targetHitInfo.InReorderingBounds && CheckReordering(view, point) ?
						OperationType.Reordering : OperationType.Regular);
			}
			DragService.SetState(OperationType.Regular);
		}
		bool CheckReordering(IUIView view, Point point) {
			if(view is BaseUIView) {
				IDragServiceListener reorderingListener = view.GetUIServiceListener<IDragServiceListener>(OperationType.Reordering);
				if(reorderingListener != null) {
					return reorderingListener.CanDrag(point, DragService.DragItem) && ((BaseUIView)view).CheckReordering(point);
				}
			}
			return true;
		}
		public override void ProcessCancel(IUIView view) {
			base.ProcessCancel(view);
			lastTargetView = null;
			lastTargetViewOperation = OperationType.Regular;
		}
	}
	public class ResizingDragServiceState : DragServiceState {
		public ResizingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Resizing; }
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
	public class ScrollingDragServiceState : DragServiceState {
		public ScrollingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Scrolling; }
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
	public class FloatingMovingDragServiceState : DragServiceState {
		public FloatingMovingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.FloatingMoving; }
		}
		IUIView lastBehindView;
		OperationType lastBehindViewOperation = OperationType.Regular;
		protected override IUIView GetBehindView() {
			return lastBehindView;
		}
		protected override OperationType GetBehindOperation() {
			return lastBehindViewOperation;
		}
		bool mouseUpWithoutMouseMove = true;
		public override void ProcessMouseMove(IUIView view, Point point) {
			mouseUpWithoutMouseMove = false;
			DoDragging(view, point);
			if(DragService.SuspendBehindDragging) return;
			Point screenPoint = view.ClientToScreen(point);
			IUIView behindView = view.Adapter.GetBehindView(view, screenPoint);
			OperationType behindOperation = OperationType.Regular;
			Point behindPoint = Point.Empty;
			if(behindView != null) {
				behindPoint = behindView.ScreenToClient(screenPoint);
				LayoutElementHitInfo behindHitInfo = behindView.Adapter.CalcHitInfo(behindView, behindPoint);
				if(behindHitInfo.InReorderingBounds && CheckReordering(behindView, behindPoint))
					behindOperation = OperationType.Reordering;
			}
			if(!DragService.NonClientDragging && behindView == null) {
				DragService.NonClientDragging = true;
				DoEnter(screenPoint, view, OperationType.NonClientDragging);
			}
			if(DragService.NonClientDragging && behindView != null) {
				DragService.NonClientDragging = false;
				DoLeave(view, OperationType.NonClientDragging);
			}
			if(behindView != lastBehindView || lastBehindViewOperation != behindOperation) {
				TryLeaveAndEnter(lastBehindView, lastBehindViewOperation, behindView, behindPoint, behindOperation);
				lastBehindView = behindView;
				lastBehindViewOperation = behindOperation;
			}
			if(behindView != null)
				DoDragging(behindView, behindPoint, behindOperation);
			else {
				if(DragService.NonClientDragging)
					DoDragging(view, screenPoint, OperationType.NonClientDragging);
			}
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			using(var context = DragOperationContext.Create()) {
				DoDrop(view, point);
				if(!view.IsDisposing && !DragService.SuspendBehindDragging) {
					Point screenPoint = view.ClientToScreen(point);
					IUIView behindView = view.Adapter.GetBehindView(view, screenPoint);
					if(behindView != null) {
						if(DragService.NonClientDragging) {
							DoLeave(view, OperationType.NonClientDragging);
							DragService.NonClientDragging = false;
							mouseUpWithoutMouseMove = true;
						}
						context.Reset();
						Point behindPoint = behindView.ScreenToClient(screenPoint);
						LayoutElementHitInfo behindHitInfo = behindView.Adapter.CalcHitInfo(behindView, behindPoint);
						OperationType behindOperation = behindHitInfo.InReorderingBounds
							&& CheckReordering(behindView, behindPoint) ? OperationType.Reordering : OperationType.Regular;
						if(mouseUpWithoutMouseMove)
							DoEnter(behindPoint, behindView, behindOperation);
						DoDrop(behindView, behindPoint, behindOperation);
					}
					else {
						if(DragService.NonClientDragging) {
							DoDrop(view, screenPoint, OperationType.NonClientDragging);
							DragService.NonClientDragging = false;
						}
					}
				}
			}
			if(view.IsDisposing && DragService.NonClientDragging) 
				DragService.NonClientDragging = false;
			DragService.SetState(OperationType.Regular);
		}
		bool CheckReordering(IUIView view, Point point) {
			if(view is BaseUIView) {
				IDragServiceListener reorderingListener = view.GetUIServiceListener<IDragServiceListener>(OperationType.Reordering);
				if(reorderingListener != null) {
					return reorderingListener.CanDrag(point, DragService.DragItem) && ((BaseUIView)view).CheckReordering(point);
				}
			}
			return true;
		}
		public override void ProcessCancel(IUIView view) {
			using(var context = DragOperationContext.Create()) {
				base.ProcessCancel(view);
			}
			lastBehindView = null;
			lastBehindViewOperation = OperationType.Regular;
		}
	}
	public class NonClientDragServiceState : DragServiceState {
		public NonClientDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.NonClientDragging; }
		}
		public override void ProcessMouseMove(IUIView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IUIView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
	public static class DragOperationContext {
		public static IDragOperationContext Create() {
			return new OperationContext();
		}
		public static IDragOperationContext Current { get; private set; }
		#region
		class OperationContext : IDragOperationContext {
			public OperationContext() {
				Current = this;
			}
			int actionRegistered;
			DragOperationContextAction action;
			public void RegisterAction(DragOperationContextAction action) {
				if(0 == actionRegistered++)
					this.action = action;
			}
			public void Dispose() {
				Flush();
				Current = null;
			}
			public void Flush() {
				if(action != null)
					action();
				action = null;
			}
			public void Reset() {
				actionRegistered = 0;
				action = null;
			}
		}
		#endregion
	}
}
