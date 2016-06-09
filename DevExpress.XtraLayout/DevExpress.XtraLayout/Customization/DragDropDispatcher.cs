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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.XtraLayout.Handlers {
	public class DragDropDispatcherFactory {
		public static IDragDropDispatcher CreateIDragDropDispatcher() {
			return new DragDropDispatcher();
		}
		[ThreadStatic]
		static IDragDropDispatcher defaultCore = null;
		public static IDragDropDispatcher Default {
			get {
				if(defaultCore == null)
					defaultCore = CreateIDragDropDispatcher();
				return defaultCore;
			}
		}
		internal static void CheckDisposing(DragDropDispatcher dragDropDispatcher) {
			if(defaultCore == dragDropDispatcher)
				defaultCore = null;
		}
		public static int MinDragSize { get { return 5; } }
	}
	public delegate void OnNonClientSpaceDragEnter();
	public delegate void OnNonClientSpaceDragLeave();
	public delegate void OnNonClientSpaceDrop(Point pt);
	public delegate void OnNonClientSpaceDragging(Point pt);
	public enum DragDropDispatcherState { Regular, ClientDragging, NonClientDragging }
	public interface IDragDropDispatcher : IDisposable {
		int ClientCount { get; }
		DragDropClientGroupDescriptor GetUniqueGroupDescriptor();
		DragDropClientDescriptor RegisterClient(IDragDropDispatcherClient client);
		void UnRegisterClient(DragDropClientDescriptor clientDescriptor);
		void SetClientToolFrame(DragDropClientDescriptor client, IToolFrame toolFrame);
		DragDropDispatcherState State { get; }
		void CancelAllDragOperations();
		DragDropClientDescriptor ActiveReceiver { get; }
		DragDropClientDescriptor ActiveSender { get; }
		BaseLayoutItem DragItem { get; set; }
		bool AllowDragNullItem { get; set; }
		bool AllowOnlySameClientGroupOperations { get; set; }
		bool ProcessKeyEvent(DragDropClientDescriptor sender, KeyEventArgs kea);
		bool ProcessMouseEvent(DragDropClientDescriptor sender, ProcessEventEventArgs pea);
		event OnNonClientSpaceDragging NonClientDragging;
		event OnNonClientSpaceDrop NonClientDrop;
		event OnNonClientSpaceDragEnter NonClientDragEnter;
		event OnNonClientSpaceDragEnter NonClientDragLeave;
	}
	public interface IDragDropDispatcherClient {
		IntPtr ClientHandle { get; }
		DragDropClientDescriptor ClientDescriptor { get; }
		DragDropClientGroupDescriptor ClientGroup { get; }
		Rectangle ScreenBounds { get; }
		Point PointToClient(Point p);
		Point PointToScreen(Point p);
		bool IsActiveAndCanProcessEvent { get; }
		bool AllowProcessDragging { get; set; }
		bool AllowNonItemDrop { get; set; }
		bool IsPointOnItem(Point clientPoint);
		BaseLayoutItem GetItemAtPoint(Point clientPoint);
		BaseLayoutItem ProcessDragItemRequest(Point clientPoint);
		void OnDragModeKeyDown(KeyEventArgs kea);
		void OnDragEnter();
		void OnDragLeave();
		void DoDragging(Point clientPoint);
		void DoDrop(Point clientPoint);
		void DoBeginDrag();
		void DoDragCancel();
		bool ExcludeChildren { get; }
	}
	public abstract class DragDropClientGroupDescriptor { }
	public abstract class DragDropClientDescriptor { }
	internal class DragDropClientGroup : DragDropClientGroupDescriptor {
		internal Guid GroupID = Guid.NewGuid();
	}
	internal class DragDropDispatcherWinAPI {
		public static MouseEventArgs TranslateEventArgsToScreen(DragDropClientWrapper w, MouseEventArgs eaClient) {
			Point screenPoint = w.Client.PointToScreen(eaClient.Location);
			return new MouseEventArgs(eaClient.Button, eaClient.Clicks, screenPoint.X, screenPoint.Y, eaClient.Delta);
		}
		public static MouseEventArgs TranslateEventArgsToClient(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			Point clientPoint = w.Client.PointToClient(eaScreen.Location);
			return new MouseEventArgs(eaScreen.Button, eaScreen.Clicks, clientPoint.X, clientPoint.Y, eaScreen.Delta);
		}
	}
	internal class DragDropClientWrapper : DragDropClientDescriptor, IDisposable {
		IDragDropDispatcherClient client = null;
		List<IToolFrame> toolFrameList = null;
		Control clientAsControl = null;
		public DragDropClientWrapper(IDragDropDispatcherClient client) {
			this.client = client;
			toolFrameList = new List<IToolFrame>();
			clientAsControl = client as Control;
		}
		bool isDisposingCore;
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		public void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				client = null;
				toolFrameList.Clear();
				toolFrameList = null;
			}
		}
		public Control ClientAsControl {
			get { return clientAsControl; }
		}
		public IDragDropDispatcherClient Client { get { return client; } }
		public List<IToolFrame> ToolFrameList {
			get { return toolFrameList; }
			set { toolFrameList = value; }
		}
	}
	internal class DragDropDispatcher : IDragDropDispatcher {
		bool fNeedRetryProcessEvent = false;
		bool fAllowDragNullItem = false;
		bool fBeginDrag = false;
		List<DragDropClientWrapper> clients = null;
		BaseLayoutItem dragItem = null;
		IBaseDragDropDispatcherState dispatcherState = null;
		DragDropClientWrapper wActiveReceiver = null;
		DragDropClientWrapper wActiveSender = null;
		DragDropClientWrapper wLastReceiver = null;
		Control capturedControl = null;
		public DragDropDispatcher() {
			clients = new List<DragDropClientWrapper>();
			dispatcherState = CreateDispatcherState(DragDropDispatcherState.Regular);
		}
		bool isDisposingCore;
		public void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				DragDropDispatcherFactory.CheckDisposing(this);
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		void OnDisposing() {
			dispatcherState = null;
			if(clients != null) {
				clients.Clear();
				clients = null;
			}
		}
		public void SetClientCapture(DragDropClientWrapper w) {
			if(capturedControl != null) ReleaseClientCapture();
			capturedControl = w.ClientAsControl;
			if(capturedControl != null) {
				NativeMethods.SetForegroundWindow(w.Client.ClientHandle);
				capturedControl.Capture = true;
				capturedControl.MouseCaptureChanged += new EventHandler(capturedControl_MouseCaptureChanged);
			}
		}
		void capturedControl_MouseCaptureChanged(object sender, EventArgs e) {
			if(capturedControl != null && !capturedControl.Capture) {
				capturedControl.Capture = true;
			}
		}
		public void ReleaseClientCapture() {
			if(capturedControl != null) {
				capturedControl.MouseCaptureChanged -= new EventHandler(capturedControl_MouseCaptureChanged);
				capturedControl.Capture = false;
				capturedControl = null;
			}
		}
		public event OnNonClientSpaceDragging NonClientDragging;
		public event OnNonClientSpaceDrop NonClientDrop;
		public event OnNonClientSpaceDragEnter NonClientDragEnter;
		public event OnNonClientSpaceDragEnter NonClientDragLeave;
		public DragDropClientDescriptor ActiveReceiver {
			get { return wActiveReceiver; }
		}
		public DragDropClientDescriptor ActiveSender {
			get { return wActiveSender; }
		}
		public bool IsBeginDrag {
			get { return fBeginDrag; }
			set { fBeginDrag = value; }
		}
		bool fAllowOnlySameClientGroupOperations = true;
		public bool AllowOnlySameClientGroupOperations {
			get { return fAllowOnlySameClientGroupOperations; }
			set { fAllowOnlySameClientGroupOperations = value; }
		}
		public bool AllowDragNullItem {
			get { return fAllowDragNullItem; }
			set { fAllowDragNullItem = value; }
		}
		public bool NeedRetryProcessEvent {
			get { return fNeedRetryProcessEvent; }
			set { fNeedRetryProcessEvent = value; }
		}
		public BaseLayoutItem DragItem {
			get { return dragItem; }
			set { dragItem = value; }
		}
		public DragDropClientGroupDescriptor GetUniqueGroupDescriptor() {
			return new DragDropClientGroup();
		}
		public bool ProcessKeyEvent(DragDropClientDescriptor keyReceiver, KeyEventArgs kea) {
			DragDropClientWrapper wKeyReceiver = keyReceiver as DragDropClientWrapper;
			if(wKeyReceiver != null && wKeyReceiver.Client.AllowProcessDragging) {
				wKeyReceiver.Client.OnDragModeKeyDown(kea);
			}
			return false;
		}
		public bool ProcessMouseEvent(DragDropClientDescriptor sender, ProcessEventEventArgs pea) {
			wActiveSender = sender as DragDropClientWrapper;
			if(clients == null || clients.Count == 0 || wActiveSender == null || pea == null) return false;
			if(pea.EventType == EventType.MouseDown || pea.EventType == EventType.MouseMove || pea.EventType == EventType.MouseUp) {
				wLastReceiver = wActiveReceiver;
				MouseEventArgs eaScreen = DragDropDispatcherWinAPI.TranslateEventArgsToScreen(wActiveSender, pea.EventArgs as MouseEventArgs);
				wActiveReceiver = GetEventReceiver(eaScreen.Location);
				CheckMultiLayoutDragging();
				if(wActiveReceiver != null) {
					ProcessClientAreaEvent(pea.EventType, eaScreen);
				}
				else {
					ProcessNonClientAreaEvent(pea.EventType, eaScreen);
				}
			}
			return false;
		}
		private void CheckMultiLayoutDragging() {
			if(AllowOnlySameClientGroupOperations && wActiveReceiver != null) {
				DragDropClientGroup senderGroup = wActiveSender.Client.ClientGroup as DragDropClientGroup;
				DragDropClientGroup receiverGroup = wActiveReceiver.Client.ClientGroup as DragDropClientGroup;
				if(senderGroup != receiverGroup) {
					wActiveReceiver = null;
				}
			}
		}
		bool CheckMultiLayoutDraggingBySenderAndReciver(DragDropClientWrapper reciever) {
			if(!AllowOnlySameClientGroupOperations) return true;
			return wActiveSender.Client.ClientGroup == reciever.Client.ClientGroup;
		} 
		private void ProcessNonClientAreaEvent(EventType eventType, MouseEventArgs eaScreen) {
			NeedRetryProcessEvent = false;
			if(State == DragDropDispatcherState.Regular) return;
			if(State == DragDropDispatcherState.ClientDragging) {
				if(wLastReceiver != null) {
					wLastReceiver.Client.OnDragLeave();
				}
				dispatcherState.OnStateChange(null, DragDropDispatcherState.NonClientDragging);
				SetState(DragDropDispatcherState.NonClientDragging);
				RaiseNonClientSpaceDragEnter();
			}
			ProcessMouseEvent(eventType, wActiveSender, eaScreen);
			if(NeedRetryProcessEvent) ProcessMouseEvent(eventType, wActiveSender, eaScreen);
		}
		private void ProcessClientAreaEvent(EventType eventType, MouseEventArgs eaScreen) {
			NeedRetryProcessEvent = false;
			if(State == DragDropDispatcherState.NonClientDragging) {
				RaiseNonClientSpaceDragLeave();
				dispatcherState.OnStateChange(wActiveReceiver, DragDropDispatcherState.ClientDragging);
				SetState(DragDropDispatcherState.ClientDragging);
				wActiveReceiver.Client.OnDragEnter();
			}
			else if(State == DragDropDispatcherState.ClientDragging) {
				if(wActiveReceiver != null && wLastReceiver != null && wActiveReceiver != wLastReceiver) {
					wLastReceiver.Client.OnDragLeave();
					wActiveReceiver.Client.OnDragEnter();
				}
			}
			ProcessMouseEvent(eventType, wActiveReceiver, eaScreen);
			if(NeedRetryProcessEvent) ProcessMouseEvent(eventType, wActiveReceiver, eaScreen);
		}
		public void CancelAllDragOperations() {
			if(State == DragDropDispatcherState.Regular) return;
			if(State == DragDropDispatcherState.ClientDragging && wActiveReceiver != null) {
				dispatcherState.OnStateChange(wActiveReceiver, DragDropDispatcherState.Regular);
				SetState(DragDropDispatcherState.Regular);
				return;
			}
			if(State == DragDropDispatcherState.NonClientDragging && wActiveSender != null) {
				dispatcherState.OnStateChange(wActiveSender, DragDropDispatcherState.Regular);
				SetState(DragDropDispatcherState.Regular);
				return;
			}
		}
		public DragDropClientWrapper GetEventReceiver(Point screenPoint) {
			IntPtr hWndHit = SafeWindowFromPoint(screenPoint);
			Control controlHit = Control.FromHandle(hWndHit);
			DragDropClientWrapper[] list = clients.ToArray();
			foreach(DragDropClientWrapper wrapper in list) {
				if(!wrapper.ClientAsControl.IsHandleCreated)
					continue;
				bool fIsEventInClientBounds = wrapper.Client.ScreenBounds.Contains(screenPoint);
				bool fIsControlCanBeReceiver = wrapper.ClientAsControl.Visible && wrapper.ClientAsControl.Enabled;
				controlHit = SetControlHitIfWindowIsAdorner(hWndHit, controlHit, wrapper);
				if(fIsEventInClientBounds && fIsControlCanBeReceiver && wrapper.Client.IsActiveAndCanProcessEvent) {
					if(IsClientWindowOnTop(wrapper, controlHit))
						return wrapper;
					if(IsToolFrameOnTop(wrapper.ToolFrameList, controlHit)) {
						bool fClientLevelOverlap = IsClientLevelOverlap(wrapper, screenPoint);
						bool fToolFrameLevelOverlap = IsToolFrameLevelOverlap(wrapper, hWndHit, screenPoint);
						if(!fClientLevelOverlap && !fToolFrameLevelOverlap)
							return wrapper;
					}
					bool excludeForce = IsNestedLayoutElementInDesigner(wrapper, controlHit);
					if(excludeForce) {
						if((wActiveSender != null) && wActiveSender.ClientAsControl == controlHit)
							return wActiveSender;
					}
					if((wrapper.Client.ExcludeChildren || excludeForce) && IsChildControlOnTop(wrapper, controlHit) && CheckMultiLayoutDraggingBySenderAndReciver(wrapper))
						return wrapper;
				}
			}
			return null;
		}
		private static Control SetControlHitIfWindowIsAdorner(IntPtr hWndHit, Control controlHit, DragDropClientWrapper wrapper) {
			if(controlHit != null) return controlHit;
			LayoutControl control = wrapper.ClientAsControl as LayoutControl;
			if(control == null) return controlHit;
			if(control.layoutAdorner == null) return controlHit;
			if(control.layoutAdorner.adornerWindow == null) return controlHit;
			if(control.layoutAdorner.adornerWindow.Handle != hWndHit) return controlHit;
			return control;
		}
		static bool IsNestedLayoutElementInDesigner(DragDropClientWrapper w, Control control) {
			bool isDesignTime = w.ClientAsControl is LayoutControl && w.ClientAsControl.Site != null;
			return isDesignTime && IsNestedLayoutControlChild(w.ClientAsControl as LayoutControl, control);
		}
		static bool IsNestedLayoutControlChild(LayoutControl container, Control control) {
			if(container != null) {
				while(control != null) {
					LayoutControl nestedLayout = control as LayoutControl;
					if(nestedLayout != null)
						return !IsChildInLayoutTree(container, nestedLayout); 
					control = control.Parent;
				}
			}
			return false;
		}
		static bool IsChildInLayoutTree(LayoutControl parent, LayoutControl child) {
			if(child == parent) return false;
			while(child != null) {
				if(child.Parent == parent)
					return true;
				child = child.Parent as LayoutControl;
			}
			return false;
		}
		void ProcessMouseEvent(EventType eventType, DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(w == null || w.IsDisposing) return;
			switch(eventType) {
				case EventType.MouseDown:
					dispatcherState.ProcessMouseDown(w, eaScreen);
					break;
				case EventType.MouseMove:
					dispatcherState.ProcessMouseMove(w, eaScreen);
					break;
				case EventType.MouseUp:
					dispatcherState.ProcessMouseUp(w, eaScreen);
					break;
			}
		}
		public int ClientCount {
			get { return (clients != null) ? clients.Count : 0; }
		}
		public void SetClientToolFrame(DragDropClientDescriptor client, IToolFrame toolFrame) {
			DragDropClientWrapper w = client as DragDropClientWrapper;
			if(w != null && toolFrame != null) {
				if(!w.ToolFrameList.Contains(toolFrame)) {
					w.ToolFrameList.Add(toolFrame);
				}
			}
		}
		public DragDropClientDescriptor RegisterClient(IDragDropDispatcherClient client) {
			if(GetClientWrapper(client) == null) {
				DragDropClientWrapper clientWrapper = new DragDropClientWrapper(client);
				clients.Add(clientWrapper);
				return clientWrapper;
			}
			return null;
		}
		public void UnRegisterClient(DragDropClientDescriptor clientDescriptor) {
			DragDropClientWrapper ddcwp = clientDescriptor as DragDropClientWrapper;
			clients.Remove(ddcwp);
			if(ClientCount == 0 && capturedControl != null) ReleaseClientCapture();
			if(wActiveReceiver == ddcwp) wActiveReceiver = null;
			if(wActiveSender == ddcwp) wActiveSender = null;
			if(wLastReceiver == ddcwp) wLastReceiver = null;
			if(ClientCount == 0) {
				dragItem = null;
				wActiveReceiver = null;
				wActiveSender = null;
				wLastReceiver = null;
			}
		}
		IBaseDragDropDispatcherState CreateDispatcherState(DragDropDispatcherState state) {
			switch(state) {
				case DragDropDispatcherState.ClientDragging: return new DragDropDispatcherClientDraggingState(this); ;
				case DragDropDispatcherState.NonClientDragging: return new DragDropDispatcherNonClientDraggingState(this);
				default: return new DragDropDispatcherRegularState(this);
			}
		}
		public DragDropDispatcherState State { get { return dispatcherState.State; } }
		public void SetState(DragDropDispatcherState state) {
			if(dispatcherState == null || State != state) {
				dispatcherState = CreateDispatcherState(state);
				dispatcherState.OnStateInit();
				if(state == DragDropDispatcherState.Regular) {
					dragItem = null;
					wLastReceiver = null;
					wActiveReceiver = null;
					wActiveSender = null;
				}
			}
		}
		public void RaiseNonClientSpaceDragging(Point draggingPoint) {
			if(NonClientDragging != null) {
				NonClientDragging(draggingPoint);
			}
		}
		public void RaiseNonClientSpaceDrop(Point dropPoint) {
			if(NonClientDrop != null) {
				NonClientDrop(dropPoint);
			}
		}
		public void RaiseNonClientSpaceDragEnter() {
			if(NonClientDragEnter != null) {
				NonClientDragEnter();
			}
		}
		public void RaiseNonClientSpaceDragLeave() {
			if(NonClientDragLeave != null) {
				NonClientDragLeave();
			}
		}
		bool IsToolFrameLevelOverlap(DragDropClientWrapper wrapper, IntPtr hWnd, Point screenPoint) {
			IntPtr current = hWnd;
			while(true) {
				current = NativeMethods.GetWindow(current, NativeMethods.GW_HWNDNEXT);
				if(current == IntPtr.Zero)
					return false;
				NativeMethods.RECT r = new NativeMethods.RECT();
				if(NativeMethods.GetWindowRect(current, ref r)) {
					Rectangle rect = r.ToRectangle();
					if(NativeMethods.IsWindowVisible(current) && rect.Contains(screenPoint)) {
						Control tempControl = Control.FromHandle(current);
						IToolFrame frame = tempControl as IToolFrame;
						Form form = wrapper.ClientAsControl.FindForm();
						IntPtr formHandle = IntPtr.Zero;
						if(form != null) {
							formHandle = form.Handle;
						}
						bool fIsNotToolFrame = (frame == null) || wrapper.ToolFrameList.Contains(frame);
						bool fIsNotClientOwnForm = (formHandle != current);
						return fIsNotToolFrame && fIsNotClientOwnForm;
					}
				}
			}
		}
		bool IsClientLevelOverlap(DragDropClientWrapper wrapper, Point screenPoint) {
			IntPtr handle = wrapper.Client.ClientHandle;
			while(true) {
				handle = NativeMethods.GetWindow(handle, NativeMethods.GW_HWNDPREV);
				if(handle == IntPtr.Zero) return false;
				NativeMethods.RECT r = new NativeMethods.RECT();
				if(NativeMethods.GetWindowRect(handle, ref r)) {
					Rectangle rect = r.ToRectangle();
					if(rect.Contains(screenPoint)) {
						return NativeMethods.IsWindowVisible(handle);
					}
				}
			}
		}
		static bool IsChildControlOnTop(DragDropClientWrapper wrapper, Control control) {
			if(control != null) {
				Control current = control;
				while(current.Parent != null) {
					if(current == wrapper.ClientAsControl)
						return true;
					current = current.Parent;
				}
			}
			return false;
		}
		static bool IsToolFrameOnTop(List<IToolFrame> toolFrameList, Control control) {
			if(control != null) {
				IToolFrame frame = control as IToolFrame;
				return frame != null && toolFrameList.Contains(frame);
			}
			return false;
		}
		public static IntPtr SafeWindowFromPoint(Point screenPoint) {
			Cursor oldCursor = Cursor.Current;
			IntPtr windowHandle = NativeMethods.WindowFromPoint(screenPoint);
			if(Cursor.Current != oldCursor) Cursor.Current = oldCursor;
			return windowHandle;
		}
		bool ContainsInControls(Control controlContainer, Control control) {
			if(controlContainer.Controls.Contains(control)) return true;
			foreach(Control tControl in controlContainer.Controls) {
				if(tControl is IDragDropDispatcherClient) continue;
				if(ContainsInControls(tControl, control)) return true;
			}
			return false;
		}
		bool CheckDTClientWindowOnTop(DragDropClientWrapper wrapper, Control control) {
			return wrapper.ClientAsControl.Site != null && ContainsInControls(wrapper.ClientAsControl, control);
		}
		bool IsClientWindowOnTop(DragDropClientWrapper wrapper, Control control) {
			if(control != null) {
				IDragDropDispatcherClient client = control as IDragDropDispatcherClient;
				if(client != null) {
					return (client.ClientHandle == wrapper.Client.ClientHandle) && wrapper.ClientAsControl.Enabled;
				}
				else {
					if(CheckDTClientWindowOnTop(wrapper, control))
						return true;
				}
			}
			return false;
		}
		DragDropClientWrapper GetClientWrapper(IDragDropDispatcherClient client) {
			foreach(DragDropClientWrapper w in clients) {
				if(w.Client == client) return w;
			}
			return null;
		}
	}
	internal interface IBaseDragDropDispatcherState {
		DragDropDispatcherState State { get; }
		void OnStateInit();
		void OnStateChange(DragDropClientWrapper w, DragDropDispatcherState newState);
		void ProcessMouseDown(DragDropClientWrapper w, MouseEventArgs eaScreen);
		void ProcessMouseMove(DragDropClientWrapper w, MouseEventArgs eaScreen);
		void ProcessMouseUp(DragDropClientWrapper w, MouseEventArgs eaScreen);
	}
	internal class DragDropDispatcherRegularState : IBaseDragDropDispatcherState {
		Point startPoint = Point.Empty;
		DragDropDispatcher ownerDispatcher;
		public DragDropDispatcherRegularState(DragDropDispatcher dispatcher) {
			ownerDispatcher = dispatcher;
		}
		public DragDropDispatcherState State { get { return DragDropDispatcherState.Regular; } }
		public void OnStateInit() { }
		public void OnStateChange(DragDropClientWrapper w, DragDropDispatcherState newState) { }
		public void ProcessMouseDown(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button == MouseButtons.Left) {
				MouseEventArgs eaClient = DragDropDispatcherWinAPI.TranslateEventArgsToClient(w, eaScreen);
				Point downClientPoint = eaClient.Location;
				bool fIsPointOnItem = w.Client.IsPointOnItem(downClientPoint);
				BaseLayoutItem dragItem = null;
				if(fIsPointOnItem) {
					dragItem = w.Client.ProcessDragItemRequest(downClientPoint);
					ownerDispatcher.IsBeginDrag = ((dragItem != null) && dragItem.OptionsCustomization.CanDrag());
				}
				if(ownerDispatcher.IsBeginDrag && w.Client.AllowProcessDragging) {
					startPoint = downClientPoint;
					ownerDispatcher.DragItem = dragItem;
				}
			}
		}
		public void ProcessMouseMove(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button == MouseButtons.Left && ownerDispatcher.IsBeginDrag && w.Client.AllowProcessDragging) {
				MouseEventArgs eaClient = DragDropDispatcherWinAPI.TranslateEventArgsToClient(w, eaScreen);
				Point moveClientPoint = eaClient.Location;
				if(PointIsOutOfStartDragArea(moveClientPoint)) {
					ownerDispatcher.SetClientCapture(w);
					w.Client.DoBeginDrag();
					OnStateChange(w, DragDropDispatcherState.ClientDragging);
					ownerDispatcher.SetState(DragDropDispatcherState.ClientDragging);
					ownerDispatcher.NeedRetryProcessEvent = true;
				}
				else return;
			}
			ownerDispatcher.IsBeginDrag = false;
		}
		public void ProcessMouseUp(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			ownerDispatcher.DragItem = null;
			ownerDispatcher.IsBeginDrag = false;
		}
		bool PointIsOutOfStartDragArea(Point clientPoint) {
			int dx = Math.Abs(startPoint.X - clientPoint.X);
			int dy = Math.Abs(startPoint.Y - clientPoint.Y);
			return Math.Max(dx, dy) > DragDropDispatcherFactory.MinDragSize;
		}
	}
	internal class DragDropDispatcherClientDraggingState : IBaseDragDropDispatcherState {
		DragDropDispatcher ownerDispatcher;
		public DragDropDispatcherClientDraggingState(DragDropDispatcher dispatcher) {
			ownerDispatcher = dispatcher;
		}
		public DragDropDispatcherState State { get { return DragDropDispatcherState.ClientDragging; } }
		public void OnStateInit() {
			if(ownerDispatcher.DragItem == null && !ownerDispatcher.AllowDragNullItem) {
				OnStateChange(ownerDispatcher.ActiveReceiver as DragDropClientWrapper, DragDropDispatcherState.Regular);
				ownerDispatcher.SetState(DragDropDispatcherState.Regular);
			}
		}
		public void OnStateChange(DragDropClientWrapper w, DragDropDispatcherState newState) {
			bool fWasDropOrCancelDrag = (newState == DragDropDispatcherState.Regular);
			if(fWasDropOrCancelDrag) {
				if(w != null) {
					w.Client.DoDragCancel();
					ownerDispatcher.ReleaseClientCapture();
					ownerDispatcher.DragItem = null;
				}
			}
		}
		public void ProcessMouseDown(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			OnStateChange(w, DragDropDispatcherState.Regular);
			ownerDispatcher.SetState(DragDropDispatcherState.Regular);
		}
		public void ProcessMouseMove(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button != MouseButtons.Left) {
				OnStateChange(w, DragDropDispatcherState.Regular);
				ownerDispatcher.SetState(DragDropDispatcherState.Regular);
			}
			else {
				MouseEventArgs eaClient = DragDropDispatcherWinAPI.TranslateEventArgsToClient(w, eaScreen);
				Point moveClientPoint = eaClient.Location;
				if(w.Client.AllowProcessDragging) {
					w.Client.DoDragging(moveClientPoint);
				}
			}
		}
		public void ProcessMouseUp(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button == MouseButtons.Left) {
				MouseEventArgs eaClient = DragDropDispatcherWinAPI.TranslateEventArgsToClient(w, eaScreen);
				Point dropClientPoint = eaClient.Location;
				if(w.Client.IsPointOnItem(dropClientPoint)) {
					BaseLayoutItem dropItem = w.Client.GetItemAtPoint(dropClientPoint);
					bool fAllowDrop = (dropItem != null) && dropItem.OptionsCustomization.CanDrop();
					bool fDropItemIsNotEqualDragItem = (dropItem != ownerDispatcher.DragItem);
					if(fDropItemIsNotEqualDragItem && w.Client.AllowProcessDragging && fAllowDrop) {
						w.Client.DoDrop(dropClientPoint);
					}
				}
				else {
					if(w.Client.AllowNonItemDrop && w.Client.AllowProcessDragging) {
						w.Client.DoDrop(dropClientPoint);
					}
				}
			}
			OnStateChange(w, DragDropDispatcherState.Regular);
			ownerDispatcher.SetState(DragDropDispatcherState.Regular);
		}
	}
	internal class DragDropDispatcherNonClientDraggingState : IBaseDragDropDispatcherState {
		DragDropDispatcher ownerDispatcher;
		public DragDropDispatcherNonClientDraggingState(DragDropDispatcher dispatcher) {
			ownerDispatcher = dispatcher;
		}
		public DragDropDispatcherState State { get { return DragDropDispatcherState.NonClientDragging; } }
		public void OnStateInit() {
			if(ownerDispatcher.DragItem == null && !ownerDispatcher.AllowDragNullItem) {
				OnStateChange(ownerDispatcher.ActiveReceiver as DragDropClientWrapper, DragDropDispatcherState.Regular);
				ownerDispatcher.SetState(DragDropDispatcherState.Regular);
			}
		}
		public void OnStateChange(DragDropClientWrapper w, DragDropDispatcherState newState) {
			bool fWasDropOrCancelDrag = (newState == DragDropDispatcherState.Regular);
			if(fWasDropOrCancelDrag) {
				if(w != null) {
					w.Client.DoDragCancel();
					ownerDispatcher.ReleaseClientCapture();
					ownerDispatcher.DragItem = null;
				}
			}
		}
		public void ProcessMouseDown(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			OnStateChange(w, DragDropDispatcherState.Regular);
			ownerDispatcher.SetState(DragDropDispatcherState.Regular);
		}
		public void ProcessMouseMove(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button != MouseButtons.Left) {
				OnStateChange(w, DragDropDispatcherState.Regular);
				ownerDispatcher.SetState(DragDropDispatcherState.Regular);
			}
			else {
				ownerDispatcher.RaiseNonClientSpaceDragging(eaScreen.Location);
			}
		}
		public void ProcessMouseUp(DragDropClientWrapper w, MouseEventArgs eaScreen) {
			if(eaScreen.Button == MouseButtons.Left) {
				ownerDispatcher.RaiseNonClientSpaceDrop(eaScreen.Location);
			}
			OnStateChange(w, DragDropDispatcherState.Regular);
			ownerDispatcher.SetState(DragDropDispatcherState.Regular);
		}
	}
}
