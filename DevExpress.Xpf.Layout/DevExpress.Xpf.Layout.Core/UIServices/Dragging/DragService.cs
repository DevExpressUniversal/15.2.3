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
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Layout.Core.Dragging {
	class DragService : UIService, IDragService {
		public DragService()
			: this(new DragServiceStateFactory()) {
		}
		public DragService(IDragServiceStateFactory factory) {
			defaultStateFactoryCore = factory;
		}
		protected override void OnDispose() {
			Reset();
			adapter = null;
			base.OnDispose();
		}
		public IView Receiver { get; private set; }
		public OperationType OperationType { get { return State.OperationType; } }
		public Point DragOrigin { get; set; }
		public bool SuspendBehindDragging { get; set; }
		protected internal IView LastProcessor { get; set; }
		protected internal bool RethrowEvent { get; private set; }
		protected override System.Windows.Input.Key[] GetKeys() {
			return new System.Windows.Input.Key[] { 
				System.Windows.Input.Key.Escape, 
				System.Windows.Input.Key.LeftCtrl,
				System.Windows.Input.Key.RightCtrl
			};
		}
		protected override bool ProcessMouseOverride(IView view, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) {
			IViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				Point screenPoint = view.ClientToScreen(ea.Point);
				Receiver = adapter.GetView(screenPoint);
				IView processor = CalculateEventProcessor(screenPoint, eventType, ea);
				RethrowEvent = false;
				ProcessMouseCore(processor, screenPoint, eventType, ea);
				if(RethrowEvent)
					ProcessMouseCore(processor, screenPoint, eventType, ea);
				LastProcessor = processor;
				return true;
			}
			return false;
		}
		protected override bool ProcessKeyOverride(IView view, Platform.KeyEventType eventype, System.Windows.Input.Key key) {
			IViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				ProcessKeyCore(Sender, eventype, key);
				return true;
			}
			return false;
		}
		protected override void BeginEventOverride(IView sender) {
			RethrowEvent = false;
		}
		protected override void EndEventOverride() {
			RethrowEvent = false;
		}
		protected IView CalculateEventProcessor(Point screenPoint, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) {
			IView result = Receiver;
			if(Receiver == null) {
				result = Sender;
				if(OperationType == OperationType.ClientDragging || OperationType == OperationType.Reordering) {
					OperationType operation = CheckBehavior(Sender, DragItem, OperationType.FloatingMoving, OperationType.NonClientDragging);
					if(operation == OperationType.NonClientDragging)
						operation = CheckBehavior(Sender, DragItem, OperationType.Floating, OperationType.NonClientDragging);
					ILayoutElementBehavior targetBehavior = Sender.GetElementBehavior(DragItem);
					if(targetBehavior != null && targetBehavior.AllowDragging && targetBehavior.CanDrag(operation)) {
						LeaveAndEnter(LastProcessor, OperationType, Sender, screenPoint, operation);
					}
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
					case OperationType.ClientDragging:
						LeaveAndEnter(LastProcessor, OperationType.ClientDragging, Receiver, screenPoint, OperationType.ClientDragging);
						break;
					case OperationType.Resizing:
					case OperationType.FloatingResizing:
					case OperationType.FloatingMoving:
						result = Sender;
						break;
					case OperationType.Reordering:
						IView sender = Sender;
						OperationType clientTargetOperation = CheckReorderingOperation(Receiver, DragItem, screenPoint);
						ILayoutElementBehavior clientTargetBehavior = sender.GetElementBehavior(DragItem);
						if(eventType != Platform.MouseEventType.MouseUp && clientTargetBehavior != null && clientTargetBehavior.AllowDragging && clientTargetBehavior.CanDrag(clientTargetOperation)) {
							IViewAdapter adapter = sender.Adapter;
							LeaveAndEnter(LastProcessor, OperationType.Reordering, sender, screenPoint, clientTargetOperation);
							if(sender.IsDisposing)
								sender = adapter.GetView(DragItem);
						}
						result = sender;
						break;
				}
			}
			return result;
		}
		OperationType CheckReorderingOperation(IView target, ILayoutElement dragItem, Point screenPoint) {
			Point targetPoint = target.ScreenToClient(screenPoint);
			LayoutElementHitInfo hitInfo = target.Adapter.CalcHitInfo(target, targetPoint);
			if(hitInfo.InReorderingBounds) return OperationType.Reordering;
			return CheckBehavior(target, dragItem, OperationType.Floating, OperationType.ClientDragging);
		}
		OperationType CheckBehavior(IView view, ILayoutElement dragItem, OperationType requested, OperationType allowed) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(dragItem);
			return (behavior != null) && behavior.CanDrag(requested) ? requested : allowed;
		}
		protected void LeaveAndEnter(IView viewFrom, OperationType from, IView viewTo, Point screenPoint, OperationType to) {
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
					listener.OnEnter();
			}
		}
		protected virtual void ProcessMouseCore(IView processor, Point screenPoint, Platform.MouseEventType eventType, Platform.MouseEventArgs e) {
			if(processor == null) return;
			Point viewPoint = processor.ScreenToClient(screenPoint);
			bool cancel = false;
			switch(eventType) {
				case Platform.MouseEventType.MouseDown:
					cancel = (e.Buttons != Platform.MouseButtons.Left);
					if(!cancel) State.ProcessMouseDown(processor, viewPoint);
					break;
				case Platform.MouseEventType.MouseMove:
					cancel = (e.Buttons != Platform.MouseButtons.Left);
					if(!cancel) State.ProcessMouseMove(processor, viewPoint);
					break;
				case Platform.MouseEventType.MouseUp:
					cancel = (e.Buttons != Platform.MouseButtons.None);
					if(!cancel) State.ProcessMouseUp(processor, viewPoint);
					break;
			}
			if(cancel) {
				State.ProcessCancel(processor);
				SetState(Core.OperationType.Regular);
			}
		}
		protected virtual void ProcessKeyCore(IView processor, Platform.KeyEventType eventype, System.Windows.Input.Key key) {
			if(eventype == Platform.KeyEventType.KeyDown)
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
			operationType = CheckCanSuspendOperation(operationType);
			if(prevOperationType == operationType) return;
			stateCore = GetStateFactory().Create(this, operationType);
			switch(operationType) {
				case OperationType.Regular:
					if(prevOperationType != Core.OperationType.Regular)
						EndUpdating();
					Reset();
					break;
				case Core.OperationType.FloatingMoving:
					CheckSuspendBehindDragging(prevOperationType);
					break;
				case OperationType.ClientDragging:
				case OperationType.Floating:
				case OperationType.Resizing:
				case OperationType.Reordering:
					RethrowEvent = true;
					break;
			}
			if(prevOperationType == Core.OperationType.Regular)
				BeginUpdating();
		}
		OperationType CheckCanSuspendOperation(OperationType operationType) {
			bool canSuspendOperation = false;
			switch(operationType) {
				case OperationType.Floating:
					canSuspendOperation = CheckSuspendFloating(operationType);
					break;
				case OperationType.ClientDragging:
					canSuspendOperation = CheckSuspendClientDragging(operationType);
					break;
				case OperationType.Resizing:
				case OperationType.FloatingResizing:
					canSuspendOperation = CheckSuspendResizing(operationType);
					break;
				case OperationType.Reordering:
					canSuspendOperation = CheckSuspendReordering(operationType);
					break;
				case OperationType.FloatingMoving:
					canSuspendOperation = CheckSuspendFloatingMoving(operationType);
					break;
			}
			if(canSuspendOperation) {
				State.ProcessCancel(DragSource);
				operationType = Core.OperationType.Regular;
				Reset();
			}
			return operationType;
		}
		bool CheckSuspendFloating(OperationType operationType) {
			return DragSource != null && DragItem != null && ((Platform.BaseView)DragSource).CanSuspendFloating(DragItem);
		}
		bool CheckSuspendClientDragging(OperationType operationType) {
			return DragSource != null && DragItem != null && ((Platform.BaseView)DragSource).CanSuspendClientDragging(DragItem);
		}
		bool CheckSuspendResizing(OperationType operationType) {
			return DragSource != null && DragItem != null && ((Platform.BaseView)DragSource).CanSuspendResizing(DragItem);
		}
		bool CheckSuspendReordering(OperationType operationType) {
			return DragSource != null && DragItem != null && ((Platform.BaseView)DragSource).CanSuspendReordering(DragItem);
		}
		bool CheckSuspendFloatingMoving(OperationType operationType) {
			return DragSource != null && DragItem != null && ((Platform.BaseView)DragSource).CanSuspendFloatingMoving(DragItem);
		}
		void CheckSuspendBehindDragging(OperationType prevOperationType) {
			if(DragSource != null && DragItem != null) {
				if(prevOperationType == Core.OperationType.Regular)
					SuspendBehindDragging = (((Platform.BaseView)DragSource).CanSuspendBehindDragging(DragItem));
			}
		}
		IViewAdapter adapter;
		object GetNotificationSource() {
			if(adapter == null) {
				IView view = Sender ?? Receiver;
				if(view == null) return null;
				adapter = view.Adapter;
			}
			return (adapter != null) ? adapter.NotificationSource : null;
		}
		void BeginUpdating() {
			if(IsDisposing) return;
			object source = GetNotificationSource();
			NotificationBatch.Updating(source);
		}
		void EndUpdating() {
			if(IsDisposing) return;
			object source = GetNotificationSource();
			NotificationBatch.Updated(source);
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
		IView dragSourceCore;
		public IView DragSource {
			get { return dragSourceCore; }
			set {
				if(dragSourceCore == value) return;
				IView oldSource = dragSourceCore;
				dragSourceCore = value;
				OnDragSourceChanged(value, oldSource);
			}
		}
		void OnReset() {
			State.ProcessComplete(DragSource);
		}
		public void Reset() {
			OnReset();
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
		void OnDragSourceChanged(IView value, IView oldValue) {
			if(oldValue != null && !oldValue.IsDisposing)
				oldValue.ReleaseCapture();
			if(value != null) {
				AssertionException.IsFalse(value.IsDisposing);
				value.SetCapture();
			}
		}
	}
}
