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
	public abstract class DragServiceState : IDragServiceState {
		public IDragService DragService { get; private set; }
		protected DragServiceState(IDragService service) {
			DragService = service;
		}
		public abstract OperationType OperationType { get; }
		OperationType _lastInitializedOperation;
		public virtual void ProcessMouseDown(IView view, Point point) {
			DragService.SetState(OperationType.Regular);
		}
		public virtual void ProcessMouseUp(IView view, Point point) {
			DragService.SetState(OperationType.Regular);
		}
		public virtual void ProcessMouseMove(IView view, Point point) {
			DragService.SetState(OperationType.Regular);
		}
		public virtual void ProcessCancel(IView view) {
			CancelInitializedOperation(view);
			if(OperationType == Core.OperationType.Regular) return;
			DoLeave(GetBehindView(), GetBehindOperation());
			DoCancel(view, OperationType);
		}
		void CancelInitializedOperation(IView view) {
			if(_lastInitializedOperation != OperationType.Regular) {
				IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(_lastInitializedOperation);
				if(listener != null) listener.OnCancel();
				_lastInitializedOperation = OperationType.Regular;
			}
		}
		public virtual void ProcessKeyDown(IView view, System.Windows.Input.Key key) {
			switch(key) {
				case System.Windows.Input.Key.Escape:
					ProcessCancel(view);
					DragService.SetState(OperationType.Regular);
					break;
				case System.Windows.Input.Key.LeftCtrl:
				case System.Windows.Input.Key.RightCtrl:
					DoLeave(view, OperationType);
					DoLeave(GetBehindView(), GetBehindOperation());
					break;
			}
		}
		public virtual void ProcessKeyUp(IView view, System.Windows.Input.Key key) {
			switch(key) {
				case System.Windows.Input.Key.LeftCtrl:
				case System.Windows.Input.Key.RightCtrl:
					DoEnter(view, OperationType);
					DoEnter(GetBehindView(), GetBehindOperation());
					break;
			}
		}
		protected virtual IView GetBehindView() { 
			return null; 
		}
		protected virtual Core.OperationType GetBehindOperation() {
			return Core.OperationType.Regular;
		}
		protected void DoCancel(IView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnCancel();
			}
		}
		protected void DoLeave(IView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnLeave();
			}
		}
		protected void DoEnter(IView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnEnter();
			}
		}
		public virtual void ProcessComplete(IView view) { }
		protected void TryLeaveAndEnter(IView viewFrom, OperationType from, IView viewTo, Point point, OperationType to) {
			IDragServiceListener leaveListener = null;
			if(viewFrom != null) {
				leaveListener = viewFrom.GetUIServiceListener<IDragServiceListener>(from);
			}
			IDragServiceListener enterListener = null;
			bool canEnter = false;
			if(viewTo != null) {
				enterListener = viewTo.GetUIServiceListener<IDragServiceListener>(to);
				canEnter = (enterListener != null) && enterListener.CanDrag(point, DragService.DragItem);
			}
			if(canEnter || (enterListener == null))
				if(leaveListener != null) leaveListener.OnLeave();
			if(canEnter)
				enterListener.OnEnter();
		}
		protected void DoDragging(IView view, Point point, OperationType type) {
			IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
			if(listener != null && listener.CanDrag(point, DragService.DragItem))
				listener.OnDragging(point, DragService.DragItem);
		}
		protected void DoDragging(IView view, Point point) {
			DoDragging(view, point, OperationType);
		}
		protected void DoDrop(IView view, Point point, OperationType type) {
			IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
			if(listener != null) {
				if(listener.CanDrop(point, DragService.DragItem))
					listener.OnDrop(point, DragService.DragItem);
				else listener.OnCancel();
			}
		}
		protected void DoDrop(IView view, Point point) {
			DoDrop(view, point, OperationType);
		}
		protected bool InitializeOperation(IView view, Point point, ILayoutElement dragItem, OperationType type) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			if((behavior != null) && behavior.AllowDragging && behavior.CanDrag(type)) {
				DragService.DragItem = dragItem;
				DragService.DragSource = view;
				DragService.DragOrigin = view.ClientToScreen(point);
				IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
				if(listener != null) listener.OnInitialize(point, dragItem);
				_lastInitializedOperation = type;
				return true;
			}
			_lastInitializedOperation = OperationType.Regular;
			return false;
		}
		protected bool BeginOperation(OperationType operationType, IView view, ILayoutElement dragItem, Point startPoint) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			if((behavior != null) && behavior.AllowDragging && behavior.CanDrag(operationType)) {
				DragService.SetState(operationType);
				if(DragService.OperationType == operationType) {
					dragItem.ResetState();
					IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(operationType);
					if(listener != null) listener.OnBegin(startPoint, dragItem);
					return true;
				}
			}
			return false;
		}
		protected void ResetOperation(OperationType type, IView view) {
			IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
			if(listener != null) listener.OnCancel();
			_lastInitializedOperation = OperationType.Regular;
		}
	}
	public class DragServiceStateFactory : IDragServiceStateFactory {
		public virtual IDragServiceState Create(IDragService service, OperationType operationType) {
			switch(operationType) {
				case OperationType.Regular:
					return new RegularDragServiceState(service);
				case OperationType.Floating:
					return new FloatingDragServiceState(service);
				case OperationType.ClientDragging:
					return new ClientDragServiceState(service);
				case OperationType.Resizing:
					return new ResizingDragServiceState(service);
				case OperationType.Reordering:
					return new ReorderingDragServiceState(service);
				case OperationType.FloatingMoving:
					return new FloatingMovingDragServiceState(service);
				case OperationType.FloatingResizing:
					return new FloatingResizingDragServiceState(service);
				case OperationType.NonClientDragging:
					return new NonClientDragServiceState(service);
			}
			return null;
		}
	}
	public static class DragServiceHelper {
		public static bool IsOutOfArea(Point pt, Point startPoint, int threshold) {
			double dx = pt.X - startPoint.X;
			double dy = pt.Y - startPoint.Y;
			return !MathHelper.IsZero(dx, threshold) || !MathHelper.IsZero(dy, threshold);
		}
	}
}
