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

using System.Windows;
namespace DevExpress.Xpf.Layout.Core.Dragging {
	public class RegularDragServiceState : DragServiceState {
		public RegularDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Regular; }
		}
		internal bool isDragging, isResizing, isReordering, isFloating;
		public override void ProcessMouseDown(IView view, Point point) {
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
				isDragging = InitializeOperation(view, point, dragItem, OperationType.ClientDragging);
				if(isDragging) return;
			}
			if(hitInfo.InResizeBounds) {
				if(view.Type == HostType.Floating) {
					isResizing = InitializeOperation(view, point, dragItem, OperationType.FloatingResizing);
					if(isResizing) return;
				}
				isResizing = InitializeOperation(view, point, dragItem, OperationType.Resizing);
				if(isResizing) return;
			}
		}
		public override void ProcessMouseMove(IView view, Point point) {
			IView dragSource = DragService.DragSource;
			if(dragSource == null) return;
			ILayoutElement dragItem = DragService.DragItem;
			Point dragOrigin = DragService.DragOrigin;
			Point screenPoint = view.ClientToScreen(point);
			Point dragSourcePoint = dragSource.ScreenToClient(screenPoint);
			bool isFloatingMoving = isDragging && dragSource.Type == HostType.Floating;
			int treshold = isFloatingMoving ? 1 : 5;
			if(DragServiceHelper.IsOutOfArea(screenPoint, dragOrigin, treshold)) {
				if(isFloating) {
					if(BeginOperation(OperationType.Floating, dragSource, dragItem, screenPoint)) return;
				}
				if(isDragging) {
					if(dragSource.Type == HostType.Floating) {
						if(BeginOperation(OperationType.FloatingMoving, dragSource, dragItem, screenPoint)) return;
					}
					if(BeginOperation(OperationType.ClientDragging, dragSource, dragItem, dragSourcePoint)) return;
				}
				if(isReordering) {
					if(BeginOperation(OperationType.Reordering, dragSource, dragItem, dragSourcePoint)) return;
				}
			}
			if(isResizing) {
				if(dragSource.Type == HostType.Floating) {
					if(BeginOperation(OperationType.FloatingResizing, dragSource, dragItem, screenPoint)) return;
				}
				if(BeginOperation(OperationType.Resizing, dragSource, dragItem, dragSourcePoint)) return;
			}
		}
		public override void ProcessMouseUp(IView view, Point point) {
			IView dragSource = DragService.DragSource;
			if(isResizing && dragSource != null && dragSource.Type == HostType.Floating)
				ResetOperation(OperationType.FloatingResizing, view);
			DragService.Reset();
			isDragging = isResizing = isReordering = isFloating = false;
		}
	}
	public class FloatingDragServiceState : DragServiceState {
		public FloatingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Floating; }
		}
		public override void ProcessMouseMove(IView view, Point point) {
			Point screenPoint = view.ClientToScreen(point);
			IView floatingView = view.Adapter.GetView(DragService.DragItem);
			if(floatingView != null && floatingView.Type == HostType.Floating) {
				DragService.DragSource = floatingView;
				floatingView.Adapter.UIInteractionService.Activate(floatingView);
				BeginOperation(OperationType.FloatingMoving, floatingView, DragService.DragItem, screenPoint);
				return;
			}
			DragService.SetState(OperationType.Regular);
		}
	}
	public class ClientDragServiceState : DragServiceState {
		public ClientDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.ClientDragging; }
		}
		OperationType lastBehindViewOperation = OperationType.Regular;
		IView lastView;
		public override void ProcessMouseMove(IView view, Point point) {
			LayoutElementHitInfo behindHitInfo = view.Adapter.CalcHitInfo(view, point);
			OperationType currentOperation = behindHitInfo.InReorderingBounds && CheckReordering(view, point) ? OperationType.Reordering : Core.OperationType.ClientDragging;
			if(lastBehindViewOperation != currentOperation) {
				DoLeave(lastView ?? view, lastBehindViewOperation);
				lastBehindViewOperation = currentOperation;
			}
			lastView = view;
			DoDragging(view, point, currentOperation);
		}
		bool CheckReordering(IView view, Point point) {
			if(view is Platform.BaseView) {
				IDragServiceListener reorderingListener = view.GetUIServiceListener<IDragServiceListener>(OperationType.Reordering);
				if(reorderingListener != null) {
					return reorderingListener.CanDrag(point, DragService.DragItem) && ((Platform.BaseView)view).CheckReordering(point);
				}
			}
			return true;
		}
		public override void ProcessMouseUp(IView view, Point point) {
			LayoutElementHitInfo behindHitInfo = view.Adapter.CalcHitInfo(view, point);
			DoDrop(view, point, behindHitInfo.InReorderingBounds && CheckReordering(view, point) ? OperationType.Reordering : Core.OperationType.ClientDragging);
			DragService.SetState(OperationType.Regular);
		}
		protected override IView GetBehindView() {
			return lastView;
		}
	}
	public class ResizingDragServiceState : DragServiceState {
		public ResizingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.Resizing; }
		}
		public override void ProcessMouseMove(IView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IView view, Point point) {
			DoDrop(view, point);
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
		public override void ProcessMouseMove(IView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
		void ProcessComplete(IView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnComplete();
			}
		}
		public override void ProcessComplete(IView view) {
			ProcessComplete(view, OperationType);
		}
	}
	public class FloatingMovingDragServiceState : DragServiceState {
		public FloatingMovingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.FloatingMoving; }
		}
		IView lastBehindView;
		OperationType lastBehindViewOperation = OperationType.Regular;
		protected override IView GetBehindView() {
			return lastBehindView;
		}
		protected override OperationType GetBehindOperation() {
			return lastBehindViewOperation;
		}
		public override void ProcessMouseMove(IView view, Point point) {
			DoDragging(view, point);
			if(DragService.SuspendBehindDragging) return;
			Point screenPoint = view.ClientToScreen(point);
			IView behindView = view.Adapter.GetBehindView(view, screenPoint);
			Core.OperationType behindOperation = Core.OperationType.Regular;
			Point behindPoint = new Point();
			if(behindView != null) {
				behindPoint = view.Adapter.GetBehindViewPoint(view, behindView, screenPoint);
				LayoutElementHitInfo behindHitInfo = behindView.Adapter.CalcHitInfo(behindView, behindPoint);
				if(behindHitInfo.InReorderingBounds && CheckReordering(behindView, behindPoint))
					behindOperation = Core.OperationType.Reordering;
			}
			if(lastBehindView != null && lastBehindView.IsDisposing) lastBehindView = null;
			if(behindView != lastBehindView || lastBehindViewOperation != behindOperation) {
				TryLeaveAndEnter(lastBehindView, lastBehindViewOperation, behindView, behindPoint, behindOperation);
				lastBehindView = behindView;
				lastBehindViewOperation = behindOperation;
			}
			if(behindView != null)
				DoDragging(behindView, behindPoint, behindOperation);
		}
		public override void ProcessMouseUp(IView view, Point point) {
			DoDrop(view, point);
			if(DragService.SuspendBehindDragging) return;
			Point screenPoint = view.ClientToScreen(point);
			IView behindView = view.Adapter.GetBehindView(view, screenPoint);
			if(behindView != null) {
				Point behindPoint = view.Adapter.GetBehindViewPoint(view, behindView, screenPoint);
				LayoutElementHitInfo behindHitInfo = behindView.Adapter.CalcHitInfo(behindView, behindPoint);
				DoDrop(behindView, behindPoint,
						behindHitInfo.InReorderingBounds && CheckReordering(behindView, behindPoint) ? Core.OperationType.Reordering : Core.OperationType.Regular
					);
			}
			DragService.SetState(OperationType.Regular);
		}
		bool CheckReordering(IView view, Point point) {
			if(view is Platform.BaseView) {
				IDragServiceListener reorderingListener = view.GetUIServiceListener<IDragServiceListener>(OperationType.Reordering);
				if(reorderingListener != null) {
					return reorderingListener.CanDrag(point, DragService.DragItem) && ((Platform.BaseView)view).CheckReordering(point);
				}
			}
			return true;
		}
	}
	public class FloatingResizingDragServiceState : DragServiceState {
		public FloatingResizingDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.FloatingResizing; }
		}
		public override void ProcessMouseMove(IView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
	public class NonClientDragServiceState : DragServiceState {
		public NonClientDragServiceState(IDragService service)
			: base(service) {
		}
		public sealed override OperationType OperationType {
			get { return OperationType.NonClientDragging; }
		}
		public override void ProcessMouseMove(IView view, Point point) {
			DoDragging(view, point);
		}
		public override void ProcessMouseUp(IView view, Point point) {
			DoDrop(view, point);
			DragService.SetState(OperationType.Regular);
		}
	}
}
