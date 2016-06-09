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
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public abstract class DragServiceState : IDragServiceState {
		IDragService dragServiceCore;
		public IDragService DragService {
			get { return dragServiceCore; }
			private set { dragServiceCore = value; }
		}
		protected DragServiceState(IDragService service) {
			DragService = service;
		}
		public abstract OperationType OperationType { get; }
		public virtual void ProcessMouseDown(IUIView view, Point point) {
			DragService.SetState(OperationType.Regular);
		}
		public virtual void ProcessMouseUp(IUIView view, Point point) {
			DragService.SetState(OperationType.Regular);
		}
		Point lastPoint;
		public virtual void ProcessMouseMove(IUIView view, Point point) {
			lastPoint = point;
			DragService.SetState(OperationType.Regular);
		}
		public virtual void ProcessCancel(IUIView view) {
			if(OperationType == OperationType.Regular) return;
			DoLeave(GetBehindView(), GetBehindOperation());
			DoCancel(view, OperationType);
			if(DragService.NonClientDragging) {
				DoCancel(view, OperationType.NonClientDragging);
				DragService.NonClientDragging = false;
			}
		}
		public virtual void ProcessKeyDown(IUIView view, Keys key) {
			switch(key) {
				case Keys.Escape:
					ProcessCancel(view);
					DragService.SetState(OperationType.Regular);
					break;
			}
		}
		public virtual void ProcessKeyUp(IUIView view, Keys key) {
		}
		protected virtual IUIView GetBehindView() {
			return null;
		}
		protected virtual OperationType GetBehindOperation() {
			return OperationType.Regular;
		}
		protected void DoCancel(IUIView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnCancel();
			}
		}
		protected void DoLeave(IUIView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnLeave();
			}
		}
		protected void DoEnter(Point point, IUIView view, OperationType operation) {
			IDragServiceListener listener;
			if(view != null) {
				listener = view.GetUIServiceListener<IDragServiceListener>(operation);
				if(listener != null)
					listener.OnEnter(point, DragService.DragItem);
			}
		}
		protected void TryLeaveAndEnter(IUIView viewFrom, OperationType from, IUIView viewTo, Point point, OperationType to) {
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
			if(canEnter || (enterListener == null)) {
				bool canLeave = (from != to) || (viewFrom != viewTo);
				if(leaveListener != null && canLeave)
					leaveListener.OnLeave();
			}
			if(canEnter)
				enterListener.OnEnter(point, DragService.DragItem);
		}
		protected void DoDragging(IUIView view, Point point, OperationType type) {
			IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
			if(listener != null && listener.CanDrag(point, DragService.DragItem))
				listener.OnDragging(point, DragService.DragItem);
		}
		protected void DoDragging(IUIView view, Point point) {
			DoDragging(view, point, OperationType);
		}
		protected void DoDrop(IUIView view, Point point, OperationType type) {
			IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(type);
			if(listener != null) {
				if(listener.CanDrop(point, DragService.DragItem))
					listener.OnDrop(point, DragService.DragItem);
				else listener.OnCancel();
			}
		}
		protected void DoDrop(IUIView view, Point point) {
			DoDrop(view, point, OperationType);
		}
		protected bool InitializeOperation(IUIView view, Point point, ILayoutElement dragItem, OperationType type) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			if((behavior != null) && behavior.AllowDragging && behavior.CanDrag(type)) {
				DragService.DragItem = dragItem;
				DragService.DragSource = view;
				DragService.DragOrigin = view.ClientToScreen(point);
				return true;
			}
			return false;
		}
		protected bool BeginOperation(OperationType operationType, IUIView view, ILayoutElement dragItem, Point startPoint) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			if((behavior != null) && behavior.AllowDragging && behavior.CanDrag(operationType)) {
				DragService.SetState(operationType);
				if(DragService.OperationType == operationType) {
					IDragServiceListener listener = view.GetUIServiceListener<IDragServiceListener>(operationType);
					if(listener != null) listener.OnBegin(startPoint, dragItem);
					return true;
				}
			}
			return false;
		}
	}
	public class DragServiceStateFactory : IDragServiceStateFactory {
		public virtual IDragServiceState Create(IDragService service, OperationType operationType) {
			switch(operationType) {
				case OperationType.Regular:
					return new RegularDragServiceState(service);
				case OperationType.Floating:
					return new FloatingDragServiceState(service);
				case OperationType.Docking:
					return new DockingDragServiceState(service);
				case OperationType.Reordering:
					return new ReorderingDragServiceState(service);
				case OperationType.Resizing:
					return new ResizingDragServiceState(service);
				case OperationType.Scrolling:
					return new ScrollingDragServiceState(service);
				case OperationType.FloatingMoving:
					return new FloatingMovingDragServiceState(service);
				case OperationType.NonClientDragging:
					return new NonClientDragServiceState(service);
			}
			return null;
		}
	}
	public static class DragServiceHelper {
		public static bool IsOutOfArea(Point pt, Point startPoint, int threshold) {
			return Math.Abs(pt.X - startPoint.X) > threshold || Math.Abs(pt.Y - startPoint.Y) > threshold;
		}
	}
}
