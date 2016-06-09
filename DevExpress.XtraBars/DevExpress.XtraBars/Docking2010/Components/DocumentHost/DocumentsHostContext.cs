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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views {
	internal class DocumentsHostContext : IDisposable, DevExpress.Utils.Win.Hook.IHookController {
		internal DocumentManager startupManager;
		internal IList<DocumentManager> managers;
#if DEBUGTEST
		internal IDictionary<BaseDocument, IDocumentsHostWindow> deleteQueue;
		internal Form startupWindow;
#else
		IDictionary<BaseDocument, IDocumentsHostWindow> deleteQueue;
		Form startupWindow;
#endif
		public DocumentsHostContext(DocumentManager manager) {
			deleteQueue = new Dictionary<BaseDocument, IDocumentsHostWindow>();
			managers = new List<DocumentManager>();
			snappingQueue = new Dictionary<IntPtr, SnapInfo>();
			managers.Add(manager);
			this.startupManager = manager;
			this.startupWindow = GetForm(manager);
			Subscribe(startupWindow);
		}
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		void Release() {
			if(managers.Count == 0)
				((IDisposable)this).Dispose();
		}
		void OnDisposing() {
			DevExpress.Utils.Win.Hook.HookManager.DefaultManager.RemoveController(this);
			if(startupWindow != null) {
				UnSubscribe(startupWindow);
				if(!IsFormClosing(startupWindow) && !IsFormUsedByTopLevelManager(startupWindow, startupManager))
					startupWindow.Close();
			}
			this.startupManager = null;
			this.isAttachedToHookManagerCore = false;
		}
		static System.Reflection.PropertyInfo pInfoIsClosing;
		static bool IsFormClosing(Form form) {
			if(form == null) 
				return false;
			if(pInfoIsClosing == null)
				pInfoIsClosing = typeof(Form).GetProperty("IsClosing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return ((bool)pInfoIsClosing.GetValue(form, null));
		}
		static bool IsFormUsedByTopLevelManager(Form form, DocumentManager childManager) {
			return (childManager != null) && DocumentManager.FromControl(form) != childManager;
		}
		void Subscribe(Form form) {
			if(form == null) return;
			form.Closing += hostWindow_Closing;
			form.Closed += hostWindow_Closed;
		}
		void UnSubscribe(Form form) {
			if(form == null) return;
			form.Closing -= hostWindow_Closing;
			form.Closed -= hostWindow_Closed;
		}
		public bool TryGetFloatDocument(Control control, out BaseDocument document) {
			document = null;
			foreach(var manager in managers) {
				if(manager.View.FloatDocuments.TryGetValue(control, out document))
					return true;
			}
			return false;
		}
		public int GetSecondaryHostsCount() {
			if(startupWindow is IDocumentsHostWindow) return 0;
			return managers.Where(IsSecondaryManager).Count();
		}
		public bool CloseAll() {
			if(startupWindow is IDocumentsHostWindow)
				return false;
			var secondaryManagers = managers.Where(IsSecondaryManager).ToArray();
			bool closed = false;
			for(int i = 0; i < secondaryManagers.Length; i++) {
				var window = GetDocumentsHostWindow(secondaryManagers[i]);
				if(window != null) {
					Close(window);
					closed = true;
				}
			}
			return closed;
		}
		public bool CanCloseAllButThis(BaseDocument document, DocumentManager manager) {
			Predicate<BaseDocument> canClose = (d) => (d != document) && d.Properties.CanClose;
			return managers.Any(m => (m != manager) && (m.View.Documents.Exists(canClose) || m.View.FloatDocuments.Exists(canClose)));
		}
		bool IsSecondaryManager(DocumentManager manager) {
			return manager != startupManager;
		}
		public void QueueDelete(BaseDocument document, IDocumentsHostWindow hostWindow) {
			deleteQueue.Add(document, hostWindow);
			DocumentManager hostManager = hostWindow.DocumentManager;
			UnRegisterHostWindowUIView(hostManager, hostWindow);
			hostManager.HitTestEnabled = false;
			Form hostForm = ((Form)hostWindow);
			Docking2010.Dragging.SnapHelper.RemoveSnap(hostForm.Handle);
			hostForm.Owner = null;
			hostForm.WindowState = FormWindowState.Normal;
			hostWindow.Hide();
			if(hostWindow == startupWindow && managers.Count > 1) {
				startupManager = GetNextDocumentManager(startupManager);
				startupWindow = GetForm(startupManager);
			}
		}
		public IDocumentsHostWindow DequeueOnDocking(BaseDocument document) {
			IDocumentsHostWindow hostWindow;
			if(deleteQueue.TryGetValue(document, out hostWindow)) {
				deleteQueue.Remove(document);
				document.Form.Owner = null;
				if(hostWindow != startupWindow) {
					if(document.Container != null)
						document.Container.Remove(document);
					DocumentManager hostManager = hostWindow.DocumentManager;
					UnRegisterHostWindowUIView(hostManager, hostWindow);
					Close(hostWindow);
				}
			}
			return hostWindow;
		}
		public IDocumentsHostWindow DequeueOnEndDragging(BaseDocument document, BaseView view) {
			IDocumentsHostWindow hostWindow;
			Form floatForm = document.Form;
			if(!deleteQueue.TryGetValue(document, out hostWindow)) {
				hostWindow = view.RaiseCustomDocumentsHostWindow(document);
				if(hostWindow != null) {
					SetupInitialBounds(view, (Form)hostWindow, floatForm, false);
					Subscribe((Form)hostWindow);
				}
				else {
					SetupSnapping(floatForm);
					floatForm.Owner = null;
					return null;
				}
			}
			else {
				SetupInitialBounds(view, (Form)hostWindow, floatForm, true);
				deleteQueue.Remove(document);
			}
			DocumentManager hostManager = hostWindow.DocumentManager;
			Register(hostManager, hostWindow);
			hostManager.MakeChild(startupManager);
			RegisterHostWindowUIView(hostManager, hostWindow);
			return hostWindow;
		}
		void RegisterHostWindowUIView(DocumentManager manager, IDocumentsHostWindow window) {
			DragEngine.IUIView view = CreateDocumentsHostWindowUIView(window);
			manager.UIViewAdapter.Views.Add(view);
		}
		void UnRegisterHostWindowUIView(DocumentManager manager, IDocumentsHostWindow window) {
			DragEngine.IUIView view = manager.UIViewAdapter.GetView(window);
			manager.UIViewAdapter.Views.Remove(view);
			Ref.Dispose(ref view);
		}
		void Register(DocumentManager manager, IDocumentsHostWindow hostWindow) {
			if(!managers.Contains(manager)) {
				managers.Add(manager);
				SubscribeHostViewEvents(hostWindow.DocumentManager);
				RaiseRegister(hostWindow);
				Hook();
			}
		}
		void Unregister(DocumentManager manager, IDocumentsHostWindow hostWindow) {
			if(managers.Contains(manager))
				RaiseUnregister(hostWindow);
			if(managers.Remove(manager)) {
				UnsubscribeHostViewEvents(hostWindow.DocumentManager);
				UnHook();
				Release();
			}
		}
		void SubscribeHostViewEvents(DocumentManager hostManager) {
			hostManager.View.DocumentClosed += HostView_DocumentClosed;
			hostManager.View.DocumentRemoved += HostView_DocumentRemoved;
		}
		void UnsubscribeHostViewEvents(DocumentManager hostManager) {
			hostManager.View.DocumentClosed -= HostView_DocumentClosed;
			hostManager.View.DocumentRemoved -= HostView_DocumentRemoved;
		}
		bool CanDestroyAutomatically(IDocumentsHostWindow hostWindow, bool documentDisposed) {
			var view = hostWindow.DocumentManager.View;
			return (view.Documents.Count == 0) && hostWindow.DestroyOnRemovingChildren
				&& view.RaiseEmptyDocumentsHostWindow(hostWindow, !(hostWindow is FloatDocumentsHostWindow), documentDisposed);
		}
		void HostView_DocumentClosed(object sender, DocumentEventArgs e) {
			IDocumentsHostWindow hostWindow = GetDocumentsHostWindow(((BaseView)sender).Manager);
			if((hostWindow != null) && CanDestroyAutomatically(hostWindow, false))
				Close(hostWindow);
		}
		void HostView_DocumentRemoved(object sender, DocumentEventArgs e) {
			IDocumentsHostWindow hostWindow = GetDocumentsHostWindow(((BaseView)sender).Manager);
			bool documentDisposed = true;
			if((hostWindow != null) && IsDisposeInProgress((BaseView)sender, e.Document, ref documentDisposed) && CanDestroyAutomatically(hostWindow, documentDisposed))
				Close(hostWindow);
		}
		bool IsDisposeInProgress(BaseView view, BaseDocument document, ref bool documentDisposed) {
			if(document.IsControlDisposeInProgress)
				return true;
			if(!view.IsDocumentClosing(document)) {
				if(document.IsFormClose) {
					documentDisposed = false;
					return true;
				}
				return document.IsFormDisposing;
			}
			return false;
		}
		static WeakReference closingHostWindowRef;
		internal static bool IsClosing(Control control) {
			return closingHostWindowRef != null && closingHostWindowRef.Target == control;
		}
		void Close(IDocumentsHostWindow hostWindow) {
			closingHostWindowRef = new WeakReference(hostWindow);
			hostWindow.Close();
			closingHostWindowRef = null;
		}
		void RaiseRegister(IDocumentsHostWindow hostWindow) {
			foreach(DocumentManager manager in managers) {
				if(manager.View != null) manager.View.RaiseRegisterDocumentsHostWindow(hostWindow);
			}
		}
		void RaiseUnregister(IDocumentsHostWindow hostWindow) {
			foreach(DocumentManager manager in managers) {
				if(manager.View != null) manager.View.RaiseUnregisterDocumentsHostWindow(hostWindow);
			}
		}
		protected DragEngine.IUIView CreateDocumentsHostWindowUIView(IDocumentsHostWindow window) {
			return new Dragging.DocumentsHostWindowUIView(window);
		}
		void SetupInitialBounds(BaseView view, Form hostWindowForm, Form floatForm, bool preserveSize) {
			if(startupWindow != null && !(startupWindow is IDocumentsHostWindow))
				hostWindowForm.Owner = startupWindow;
			bool isSmartClientSizeCalculation = false;
			hostWindowForm.StartPosition = FormStartPosition.Manual;
			Form parentHostWindowForm = DocumentsHostContext.GetForm(view.Manager);
			int m = view.Painter.GetRootMargin();
			Point clientOffset = Point.Empty;
			Point location = Point.Empty;
			Rectangle floatingBounds = view.GetFloatingBounds(floatForm.Bounds);
			if(parentHostWindowForm != null) {
				clientOffset = parentHostWindowForm.PointToClient(parentHostWindowForm.Location);
				if(parentHostWindowForm.GetType() == hostWindowForm.GetType()) {
					Rectangle b = view.Manager.GetOwnerControl().Bounds;
					Rectangle c = parentHostWindowForm.ClientRectangle;
					if(!preserveSize) hostWindowForm.ClientSize = new Size(
						m + b.Left - c.Left + floatingBounds.Width + c.Right - b.Right + m,
						m + b.Top - c.Top + floatingBounds.Height + c.Bottom - b.Bottom + m);
					location = new Point(floatingBounds.Left + clientOffset.X, floatingBounds.Top + clientOffset.Y)
						- new Size(m + b.Left - c.Left, m + b.Top - c.Top);
					isSmartClientSizeCalculation = true;
				}
			}
			if(!isSmartClientSizeCalculation) {
				if(!preserveSize) hostWindowForm.ClientSize = new Size(
						m + floatingBounds.Width + m,
						m + floatingBounds.Height + m);
				location = new Point(floatingBounds.Left + clientOffset.X, floatingBounds.Top + clientOffset.Y)
						- new Size(m, m);
			}
			Rectangle workingArea = Screen.GetWorkingArea(floatForm.Location);
			if(!workingArea.Contains(location))
				location = workingArea.Location;
			hostWindowForm.Location = location;
			SetupSnapping(hostWindowForm, floatForm, clientOffset);
		}
		void SetupSnapping(Form floatForm) {
			SnapInfo snapInfo;
			if(snappingQueue.TryGetValue(floatForm.Handle, out snapInfo)) {
				snappingQueue.Remove(floatForm.Handle);
				switch(snapInfo.Hint) {
					case Customization.DockHint.SnapScreen:
						floatForm.WindowState = FormWindowState.Maximized;
						break;
					case Customization.DockHint.SnapLeft:
					case Customization.DockHint.SnapRight:
					case Customization.DockHint.SnapBottom:
						Docking2010.Dragging.SnapHelper.AddSnap(floatForm.Handle, snapInfo.Point, floatForm.Bounds);
						floatForm.Bounds = snapInfo.Rect;
						break;
				}
			}
		}
		void SetupSnapping(Form hostWindowForm, Form floatForm, Point clientOffset) {
			SnapInfo snapInfo;
			if(snappingQueue.TryGetValue(floatForm.Handle, out snapInfo)) {
				snappingQueue.Remove(floatForm.Handle);
				switch(snapInfo.Hint) {
					case Customization.DockHint.SnapScreen:
						FloatDocumentsHostWindow floatDocumentsHostWindow = hostWindowForm as FloatDocumentsHostWindow;
						if(floatDocumentsHostWindow != null)
							floatDocumentsHostWindow.CheckMaximizedBounds();
						hostWindowForm.WindowState = FormWindowState.Maximized;
						break;
					case Customization.DockHint.SnapLeft:
					case Customization.DockHint.SnapRight:
					case Customization.DockHint.SnapBottom:
						Docking2010.Dragging.SnapHelper.AddSnap(hostWindowForm.Handle, snapInfo.Point, floatForm.Bounds);
						hostWindowForm.Bounds = snapInfo.Rect;
						break;
				}
			}
		}
		void hostWindow_Closing(object sender, CancelEventArgs e) {
			IDocumentsHostWindow hostWindow = sender as IDocumentsHostWindow;
			if(startupWindow == hostWindow) {
				if(managers.Count > 1) {
					startupManager = GetNextDocumentManager(startupManager);
					startupWindow = GetForm(startupManager);
				}
				else startupWindow = null;
			}
			if(hostWindow != null)
				hostWindow.DocumentManager.MoveFloatFormsTo(startupManager);
		}
		void hostWindow_Closed(object sender, EventArgs e) {
			IDocumentsHostWindow hostWindow = sender as IDocumentsHostWindow;
			UnSubscribe((Form)sender);
			if(hostWindow != null) {
				DocumentManager manager = hostWindow.DocumentManager;
				if(manager.ActivationInfo != null)
					manager.ActivationInfo.Detach(hostWindow);
				Unregister(manager, hostWindow);
				UnRegisterHostWindowUIView(manager, hostWindow);
			}
		}
		DocumentManager GetNextDocumentManager(DocumentManager manager) {
			DocumentManager nextManager = null;
			for(int i = 0; i < managers.Count; i++) {
				if(manager == managers[i] || managers[i].IsDisposing) continue;
				nextManager = managers[i];
				break;
			}
			return nextManager;
		}
		public static bool IsEmpty(IDocumentsHostWindow window) {
			return (window != null) && window.DocumentManager.GetOwnerControlEmpty();
		}
		public static IDocumentsHostWindow GetDocumentsHostWindow(DocumentManager manager) {
			return GetForm(manager) as IDocumentsHostWindow;
		}
		internal static bool IsChild(DocumentManager manager) {
			var context = manager.GetDocumentsHostContext();
			return (context != null) && context.managers.Contains(manager) && (context.startupManager != manager);
		}
		internal static bool IsParented(DocumentManager manager) {
			var context = manager.GetDocumentsHostContext();
			if(context != null && context.startupManager != manager)
				return !(context.startupWindow is IDocumentsHostWindow);
			return false;
		}
		internal static Form GetParentForm(DocumentManager manager) {
			var context = manager.GetDocumentsHostContext();
			if(context != null && context.startupManager != manager) {
				if(!(context.startupWindow is IDocumentsHostWindow))
					return context.startupWindow;
			}
			return GetForm(manager);
		}
		internal static bool IsChild(Control control, Control container) {
			while(control != null) {
				if(control == container)
					return true;
				control = control.Parent;
			}
			return false;
		}
		internal static bool CheckHostFormActive(DocumentManager manager) {
			return CheckHostFormActive(manager, (hostForm) =>
			{
				hostForm.Activate();
				return true;
			});
		}
		internal static bool CheckHostFormActive(DocumentManager manager, Action<Form> hostFormAction) {
			return CheckHostFormActive(manager, (hostForm) =>
			{
				hostFormAction(hostForm);
				return true;
			});
		}
		internal static bool CheckHostFormActive(DocumentManager manager, Func<Form, bool> hostFormActionWithResult) {
			var focusedControl = WinAPIHelper.FindControl(BarNativeMethods.GetFocus());
			if(focusedControl != null) {
				var hostForm = DocumentsHostContext.GetForm(manager);
				if(hostForm != null && !IsChildEx(focusedControl, hostForm))
					return hostFormActionWithResult(hostForm);
			}
			return false;
		}
		internal static bool ContainsFocus(DocumentContainer container) {
			if(container.ContainsFocus) return true;
			var focusedControl = WinAPIHelper.FindControl(BarNativeMethods.GetFocus());
			return (focusedControl != null) && IsChildEx(focusedControl, container);
		}
		static bool IsChildEx(Control control, Control container) {
			while(control != null) {
				if(control == container)
					return true;
				var popupForm = control as DevExpress.XtraEditors.Popup.PopupBaseForm;
				if(popupForm != null)
					control = popupForm.OwnerEdit;
				else
					control = control.Parent;
			}
			return false;
		}
		internal static Form GetForm(DocumentManager manager) {
			ContainerControl container = (manager != null) ? manager.GetContainer() : null;
			return (container != null) ? container.FindForm() : null;
		}
		internal static Form CheckForm(Control control, Form form) {
			form = CheckParentForm(control, form);
			form = CheckTopLevelForm(form);
			form = CheckPopupBaseForm(form);
			return form;
		}
		static Form CheckParentForm(Control control, Form form) {
			if(form == null && control != null)
				form = control.FindForm();
			return form;
		}
		static Form CheckTopLevelForm(Form form) {
			if(form != null && !form.IsMdiChild && !form.TopLevel)
				return GetParentForm(form.Parent, form);
			return form;
		}
		static Form CheckPopupBaseForm(Form form) {
			var popupForm = form as DevExpress.XtraEditors.Popup.PopupBaseForm;
			if(popupForm != null)
				return GetParentForm(popupForm.OwnerEdit, form);
			return form;
		}
		static Form GetParentForm(Control control, Form form) {
			return (control != null) ? control.FindForm() : form;
		}
		#region IHookController Members
		bool isAttachedToHookManagerCore;
		protected bool IsAttachedToHookManager {
			get { return isAttachedToHookManagerCore; }
		}
		protected void Hook() {
			if(isDisposing || IsAttachedToHookManager) return;
			if(managers.Count > 1) {
				DevExpress.Utils.Win.Hook.HookManager.DefaultManager.AddController(this);
				isAttachedToHookManagerCore = true;
			}
		}
		protected void UnHook() {
			if(managers.Count <= 1)
				UnHookCore();
		}
		protected internal void UnHookCore() {
			DevExpress.Utils.Win.Hook.HookManager.DefaultManager.RemoveController(this);
			isAttachedToHookManagerCore = false;
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			IDocumentsHostWindow window = wnd as IDocumentsHostWindow;
			if(window != null)
				ProcessWindowDragging(Msg, window);
			return false;
		}
		IntPtr DevExpress.Utils.Win.Hook.IHookController.OwnerHandle {
			get { return OwnerHandleCore; }
		}
		#endregion
		#region HostWindowDragging
		IDocumentsHostWindow dragWindow;
		protected IntPtr OwnerHandleCore {
			get { return (dragWindow != null) ? ((Control)dragWindow).Handle : IntPtr.Zero; }
		}
		delegate void HostWindowDragging(IDocumentsHostWindow window, Point point);
		void ProcessWindowDragging(int Msg, IDocumentsHostWindow window) {
			if(dragWindow == null) {
				if(Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_NCLBUTTONDOWN) {
					dragWindow = window;
					ProcessDragging(BeginDragging, dragWindow);
				}
			}
			else {
				if(dragWindow == window) {
					switch(Msg) {
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEMOVE:
							ProcessDragging(Dragging, dragWindow);
							break;
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_NCLBUTTONUP: 
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_LBUTTONUP:
							ProcessDragging(EndDragging, dragWindow);
							break;
					}
				}
			}
		}
		void ProcessDragging(HostWindowDragging action, IDocumentsHostWindow window) {
			DevExpress.Utils.Win.Hook.HookInfo info;
			if(DevExpress.Utils.Win.Hook.HookManager.DefaultManager.HookHash.TryGetValue(DevExpress.Utils.Win.Hook.HookManager.CurrentThread, out info)) {
				action(window, info.GetPoint());
			}
		}
		void BeginDragging(IDocumentsHostWindow window, Point screenPoint) {
			if(NCHitTest(window, screenPoint) == DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTCAPTION) {
				var view = (Dragging.DocumentsHostWindowUIView)startupManager.UIViewAdapter.GetView(window);
				if(view != null) view.BeginExternalDragging(screenPoint);
			}
			else dragWindow = null;
		}
		void Dragging(IDocumentsHostWindow window, Point screenPoint) {
			var view = (Dragging.DocumentsHostWindowUIView)startupManager.UIViewAdapter.GetView(window);
			if(view != null) view.ExternalDragging(screenPoint);
		}
		void EndDragging(IDocumentsHostWindow window, Point screenPoint) {
			dragWindow = null;
			var view = (Dragging.DocumentsHostWindowUIView)startupManager.UIViewAdapter.GetView(window);
			if(view != null) view.EndExternalDragging(screenPoint);
		}
		static int NCHitTest(IDocumentsHostWindow window, Point screenPoint) {
			return DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(((Control)window).Handle, 0x84, IntPtr.Zero, MAKELPARAM(screenPoint.X, screenPoint.Y));
		}
		static IntPtr MAKELPARAM(int low, int high) {
			return (IntPtr)((high << 0x10) | (low & 0xffff));
		}
		#endregion HostWindowDragging
		IDictionary<IntPtr, SnapInfo> snappingQueue;
		internal static bool QueueScreenSnapping(DocumentManager manager, Form floatForm) {
			return QueueSnapping(manager, floatForm, Customization.DockHint.SnapScreen, Point.Empty, Rectangle.Empty);
		}
		internal static bool QueueSnapping(DocumentManager manager, Form floatForm, Customization.DockHint snapHint, Point snapPoint, Rectangle snapRect) {
			BaseView view = manager.View;
			if(!(floatForm is IDocumentsHostWindow) && !(floatForm is Docking.FloatForm) && view.FloatingDocumentContainer == FloatingDocumentContainer.DocumentsHost) {
				DocumentsHostContext context = manager.EnsureDocumentsHostContext();
				if(!context.snappingQueue.ContainsKey(floatForm.Handle))
					context.snappingQueue.Add(floatForm.Handle, new SnapInfo(snapHint, snapPoint, snapRect));
				return true;
			}
			else {
				if(snapHint == Customization.DockHint.SnapScreen) {
					DevExpress.Skins.XtraForm.FormPainter.PostSysCommand(floatForm, floatForm.Handle, DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MAXIMIZE);
				}
			}
			return false;
		}
		class SnapInfo {
			public readonly Customization.DockHint Hint;
			public readonly Point Point;
			public readonly Rectangle Rect;
			public SnapInfo(Customization.DockHint hint, Point point, Rectangle rect) {
				this.Hint = hint;
				this.Point = point;
				this.Rect = rect;
			}
		}
	}
}
namespace DevExpress.XtraBars.Docking2010 {
	using System.Linq;
	public static class DocumentsHostWindowExtension {
		public static IDocumentsHostWindow GetDocumentsHostWindow(this Views.BaseDocument document) {
			if(document == null || document.Manager == null || document.IsDisposing) return null;
			return GetDocumentsHostWindow(document.Manager);
		}
		public static IDocumentsHostWindow GetDocumentsHostWindow(this Views.BaseView view) {
			if(view == null || view.Manager == null || view.IsDisposing) return null;
			return GetDocumentsHostWindow(view.Manager);
		}
		public static IDocumentsHostWindow GetDocumentsHostWindow(this DocumentManager manager) {
			if(manager == null || manager.IsDisposing) return null;
			return Views.DocumentsHostContext.GetDocumentsHostWindow(manager);
		}
		public static IEnumerable<IDocumentsHostWindow> GetDocumentsHostWindows(this DocumentManager manager) {
			if(manager == null || manager.IsDisposing) return null;
			var context = manager.GetDocumentsHostContext();
			return (context != null) ? context.managers.Select(m => GetDocumentsHostWindow(m)).ToArray() : null;
		}
	}
}
