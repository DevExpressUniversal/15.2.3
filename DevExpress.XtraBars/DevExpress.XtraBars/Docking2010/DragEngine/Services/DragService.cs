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
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	class DragService : UIService, IDragService {
		public DragService()
			: this(new DragServiceStateFactory()) {
		}
		public DragService(IDragServiceStateFactory factory) {
			defaultStateFactoryCore = factory;
		}
		IUIView receiverCore;
		public IUIView Receiver {
			get { return receiverCore; }
			private set { receiverCore = value; }
		}
		public OperationType OperationType { get { return State.OperationType; } }
		Point dragOriginCore;
		public Point DragOrigin {
			get { return dragOriginCore; }
			set { dragOriginCore = value; }
		}
		bool suspendBehindDraggingCore;
		public bool SuspendBehindDragging {
			get { return suspendBehindDraggingCore; }
			set { suspendBehindDraggingCore = value; }
		}
		bool nonClientDraggingCore;
		public bool NonClientDragging {
			get { return nonClientDraggingCore; }
			set { nonClientDraggingCore = value; }
		}
		protected internal IUIView LastProcessor;
		protected internal bool RethrowEvent;
		protected override Keys[] GetKeys() {
			return new Keys[] { 
				Keys.Escape, 
				Keys.Control,Keys.ControlKey,
				Keys.LControlKey,Keys.RControlKey,
			};
		}
		protected override bool ProcessMouseOverride(IUIView view, MouseEventType eventType, MouseEventArgs ea) {
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				Point screenPoint = view.ClientToScreen(ea.Location);
				Receiver = adapter.GetView(screenPoint);
				IUIView processor = CalculateEventProcessor(screenPoint, eventType, ea);
				RethrowEvent = false;
				ProcessMouseCore(processor, screenPoint, eventType, ea);
				if(RethrowEvent)
					ProcessMouseCore(processor, screenPoint, eventType, ea);
				LastProcessor = processor;
				return true;
			}
			return false;
		}
		protected override bool ProcessKeyOverride(IUIView view, KeyEventType eventype, Keys key) {
			IUIViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				ProcessKeyCore(Sender, eventype, key);
				return true;
			}
			return false;
		}
		protected override bool ProcessFlickOverride(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) { return false; }
		protected override bool ProcessGestureOverride(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) { return false; }
		protected override void BeginEventOverride(IUIView sender) {
			RethrowEvent = false;
		}
		protected override void EndEventOverride() {
			receiverCore = null;
			RethrowEvent = false;
		}
		protected IUIView CalculateEventProcessor(Point screenPoint, MouseEventType eventType, MouseEventArgs ea) {
			IUIView result = Receiver;
			if(Receiver == null) {
				result = Sender;
				if(OperationType == OperationType.Reordering) {
					OperationType operation = CheckBehavior(Sender, DragItem, OperationType.FloatingMoving, OperationType.NonClientDragging);
					TryLeaveAndEnter(screenPoint, operation);
				}
				if(OperationType == OperationType.NonClientDragging) {
					TryLeaveAndEnter(screenPoint, OperationType.Floating);
				}
			}
			else {
				switch(OperationType) {
					case OperationType.NonClientDragging:
						OperationType nonClientTargetOperation = CheckReorderingOperation(Receiver, DragItem, screenPoint);
						ILayoutElementBehavior nonClientTargetBehavior = Sender.GetElementBehavior(DragItem);
						if(nonClientTargetBehavior != null && nonClientTargetBehavior.AllowDragging && nonClientTargetBehavior.CanDrag(nonClientTargetOperation)) {
							LeaveAndEnter(Sender, OperationType.NonClientDragging, Receiver, screenPoint, nonClientTargetOperation);
						}
						break;
					case OperationType.Docking:
					case OperationType.Resizing:
					case OperationType.Scrolling:
					case OperationType.FloatingMoving:
						result = Sender;
						break;
					case OperationType.Reordering:
						OperationType clientTargetOperation = CheckReorderingOperation(Receiver, DragItem, screenPoint);
						TryLeaveAndEnter(screenPoint, clientTargetOperation);
						result = Sender;
						break;
					case OperationType.Regular:
						if(DragSource != null)
							result = DragSource;
						break;
				}
			}
			return result;
		}
		OperationType CheckReorderingOperation(IUIView target, ILayoutElement dragItem, Point screenPoint) {
			Point targetPoint = target.ScreenToClient(screenPoint);
			LayoutElementHitInfo hitInfo = target.Adapter.CalcHitInfo(target, targetPoint);
			if(hitInfo.InReorderingBounds) return OperationType.Reordering;
			return CheckBehavior(target, dragItem, OperationType.Floating, OperationType.Docking);
		}
		OperationType CheckBehavior(IUIView view, ILayoutElement dragItem, OperationType requested, OperationType allowed) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			return (behavior != null) && behavior.CanDrag(requested) ? requested : allowed;
		}
		void TryLeaveAndEnter(Point screenPoint, OperationType operation) {
			if(LastProcessor == Sender && OperationType == operation) return;
			ILayoutElementBehavior targetBehavior = Sender.GetElementBehavior(DragItem);
			if(targetBehavior != null && targetBehavior.AllowDragging && targetBehavior.CanDrag(operation))
				LeaveAndEnter(LastProcessor, OperationType, Sender, screenPoint, operation);
		}
		protected void LeaveAndEnter(IUIView viewFrom, OperationType from, IUIView viewTo, Point screenPoint, OperationType to) {
			if(viewFrom == viewTo && from == to) return;
			IDragServiceListener listener;
			if(viewFrom != null) {
				listener = viewFrom.GetUIServiceListener<IDragServiceListener>(from);
				if(listener != null) listener.OnLeave();
			}
			if(from != to) {
				SetState(to);
				if(OperationType != to) return;
			}
			if(viewTo != null) {
				listener = viewTo.GetUIServiceListener<IDragServiceListener>(to);
				Point point = viewTo.ScreenToClient(screenPoint);
				if(listener != null && listener.CanDrag(point, DragItem))
					listener.OnEnter(point, DragItem);
			}
		}
		protected virtual void ProcessMouseCore(IUIView processor, Point screenPoint, MouseEventType eventType, MouseEventArgs e) {
			if(processor == null) return;
			Point viewPoint = processor.ScreenToClient(screenPoint);
			bool cancel = false;
			switch(eventType) {
				case MouseEventType.MouseDown:
					cancel = (e.Button != MouseButtons.Left);
					if(!cancel) State.ProcessMouseDown(processor, viewPoint);
					break;
				case MouseEventType.MouseMove:
					if(waitingForExternalDragging)
						waitingForExternalDragging = false;
					cancel = (e.Button != MouseButtons.Left);
					if(!cancel) State.ProcessMouseMove(processor, viewPoint);
					break;
				case MouseEventType.MouseUp:
					cancel = (e.Button != MouseButtons.Left) || waitingForExternalDragging;
					if(!cancel) State.ProcessMouseUp(processor, viewPoint);
					break;
			}
			if(cancel) {
				State.ProcessCancel(processor);
				SetState(OperationType.Regular);
			}
		}
		protected virtual void ProcessKeyCore(IUIView processor, KeyEventType eventype, Keys key) {
			if(eventype == KeyEventType.KeyDown)
				State.ProcessKeyDown(processor, key);
			else
				State.ProcessKeyUp(processor, key);
		}
		IDragServiceStateFactory defaultStateFactoryCore;
		protected IDragServiceStateFactory DefaultStateFactory {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(defaultStateFactoryCore == null)
					defaultStateFactoryCore = ResolveDefaultStateFactory();
				return defaultStateFactoryCore;
			}
		}
		protected IDragServiceStateFactory GetStateFactory() {
			return GetStateFactoryCore() ?? DefaultStateFactory;
		}
		protected virtual IDragServiceStateFactory GetStateFactoryCore() {
			return null;
		}
		protected virtual IDragServiceStateFactory ResolveDefaultStateFactory() {
			return ServiceLocator<IDragServiceStateFactory>.Resolve();
		}
		IDragServiceState stateCore;
		protected IDragServiceState State {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(stateCore == null)
					stateCore = GetStateFactory().Create(this, OperationType.Regular);
				return stateCore;
			}
		}
		public void SetState(OperationType operationType) {
			OperationType prevOperationType = OperationType;
			if(operationType == OperationType.Floating)
				operationType = CheckSuspendFloating(operationType);
			if(operationType == OperationType.Resizing)
				operationType = CheckSuspendResizing(operationType);
			if(prevOperationType == operationType) return;
			stateCore = GetStateFactory().Create(this, operationType);
			switch(operationType) {
				case OperationType.Regular:
					Reset();
					break;
				case OperationType.FloatingMoving:
					CheckSuspendBehindDragging(prevOperationType);
					CheckBeginExternalDragging(prevOperationType);
					break;
				case OperationType.Floating:
				case OperationType.Docking:
				case OperationType.Scrolling:
				case OperationType.Reordering:
				case OperationType.Resizing:
					RethrowEvent = true;
					if(DragSource != null && DragSource.Adapter != null)
						DragSource.Adapter.UIInteractionService.CancelUIInteractionOperation();
					break;
			}
		}
		OperationType CheckSuspendFloating(OperationType operationType) {
			if(DragSource != null && DragItem != null) {
				if(((BaseUIView)DragSource).CanSuspendFloating(DragItem)) {
					operationType = OperationType.Regular;
					State.ProcessCancel(DragSource);
					Reset();
				}
			}
			return operationType;
		}
		OperationType CheckSuspendResizing(OperationType operationType) {
			if(DragSource != null && DragItem != null) {
				if(((BaseUIView)DragSource).CanSuspendResizing(DragItem)) {
					operationType = OperationType.Regular;
					if(DragItem != null)
						DragItem.ResetState();
					State.ProcessCancel(DragSource);
					Reset();
				}
			}
			return operationType;
		}
		void CheckSuspendBehindDragging(OperationType prevOperationType) {
			if(DragSource != null && DragItem != null) {
				if(prevOperationType == OperationType.Regular)
					SuspendBehindDragging = (((BaseUIView)DragSource).CanSuspendBehindDragging(DragItem));
			}
		}
		bool waitingForExternalDragging = false;
		void CheckBeginExternalDragging(OperationType prevOperationType) {
			if(!IsInEvent && DragSource == null && DragItem != null) {
				if(prevOperationType == OperationType.Regular)
					waitingForExternalDragging = true;
			}
		}
		ILayoutElement dragItemCore;
		public ILayoutElement DragItem {
			get { return dragItemCore; }
			set {
				if(dragItemCore == value) return;
				ILayoutElement oldValue = dragItemCore;
				dragItemCore = value;
				OnDragItemChanged(value, oldValue);
			}
		}
		IUIView dragSourceCore;
		public IUIView DragSource {
			get { return dragSourceCore; }
			set {
				if(dragSourceCore == value) return;
				IUIView oldSource = dragSourceCore;
				dragSourceCore = value;
				OnDragSourceChanged(value, oldSource);
			}
		}
		public void CancelDragOperation() {
			State.ProcessCancel(LastProcessor);
			SetState(OperationType.Regular);
			ResetCore();
		}
		public void ReparentDragOperation() {
			State.ProcessCancel(DragSource);
			ResetCore();
		}
		protected override void ResetCore() {
			DragSource = null;
			DragItem = null;
			DragOrigin = InvalidPoint;
			SuspendBehindDragging = false;
		}
		void OnDragItemChanged(ILayoutElement value, ILayoutElement oldValue) {
			if(oldValue != null && oldValue is BaseLayoutElement)
				((BaseLayoutElement)oldValue).IsDragging = false;
			if(value != null && value is BaseLayoutElement)
				((BaseLayoutElement)value).IsDragging = true;
		}
		void OnDragSourceChanged(IUIView value, IUIView oldValue) {
			if(oldValue != null && !oldValue.IsDisposing) {
				oldValue.ReleaseCapture();
			}
			if(value != null) {
				AssertionException.IsFalse(value.IsDisposing);
				value.SetCapture();
			}
		}
	}
}
