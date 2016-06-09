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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public class BaseDocumentCollection : BaseMutableListEx<BaseDocument>,
		DevExpress.Utils.Win.Hook.IHookController {
		BaseView ownerCore;
		IDictionary<Control, BaseDocument> controlLinks;
		public BaseDocumentCollection(BaseView owner) {
			ownerCore = owner;
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
		protected override void OnCreate() {
			base.OnCreate();
			controlLinks = new Dictionary<Control, BaseDocument>();
		}
		protected override void OnDispose() {
			isAttachedToHookManagerCore = false;
			DevExpress.Utils.Win.Hook.HookManager.DefaultManager.RemoveController(this);
			base.OnDispose();
		}
		protected override void UnregisterElementOnDisposeCollection(BaseDocument element) {
			base.UnregisterElementOnDisposeCollection(element);
			UnregisterControl(element);
		}
		protected override bool CanAdd(BaseDocument element) {
			Control control = element.Control;
			return base.CanAdd(element) && (control == null || !controlLinks.ContainsKey(control));
		}
		protected override void OnBeforeElementAdded(BaseDocument element) {
			Owner.BeginUpdate();
			element.SetManager(Owner.Manager);
			if(element.Control == null)
				element.MarkAsDeferredControlLoad();
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(BaseDocument element) {
			element.SetIsFloating(false);
			AddToContainer(element);
			Hook();
			SubscribeFormEvents(element.Form);
			base.OnElementAdded(element);
			RegisterControl(element);
			Owner.EndUpdate();
		}
		protected override void OnBeforeElementRemoved(BaseDocument element) {
			Owner.BeginUpdate();
			base.OnBeforeElementRemoved(element);
		}
		protected override void OnElementRemoved(BaseDocument element) {
			UnHook();
			UnregisterControl(element);
			UnsubscribeFormEvents(element.Form);
			base.OnElementRemoved(element);
			RemoveFromContainer(element);
			element.SetManager(null);
			Owner.EndUpdate();
		}
		protected virtual void SubscribeFormEvents(Form documentForm) {
			if(documentForm != null)
				documentForm.FormClosed += OnDocumentFormClosed;
		}
		protected virtual void UnsubscribeFormEvents(Form documentForm) {
			if(documentForm != null)
				documentForm.FormClosed -= OnDocumentFormClosed;
		}
		void OnDocumentFormClosed(object sender, FormClosedEventArgs e) {
			if(e.CloseReason == CloseReason.UserClosing)
				ProcessFormClosed(sender as Form);
		}
		protected virtual void RemoveFromContainer(BaseDocument element) {
			Owner.RemoveFromContainer(element);
		}
		protected virtual void AddToContainer(BaseDocument element) {
			Owner.AddToContainer(element);
		}
		protected override void OnElementRemoveCanceled(BaseDocument element) {
			base.OnElementRemoveCanceled(element);
			Owner.CancelUpdate();
		}
		protected internal void ReregisterControl(Control control, BaseDocument element) {
			if(control != null)
				controlLinks.Remove(control);
			control = element.Control;
			if(control != null)
				controlLinks.Add(control, element);
		}
		protected internal void RegisterControl(Control control, BaseDocument element) {
			if(control != null)
				controlLinks.Add(control, element);
		}
		protected internal void RegisterControl(BaseDocument element) {
			Control control = element.Control;
			if(control != null)
				controlLinks.Add(control, element);
		}
		protected internal void UnregisterControl(Control control) {
			if(control != null)
				controlLinks.Remove(control);
		}
		protected internal void UnregisterControl(BaseDocument element) {
			Control control = element.Control ?? FindControl(element);
			if(control != null)
				controlLinks.Remove(control);
		}
		public bool TryGetValue(Control control, out BaseDocument document) {
			return controlLinks.TryGetValue(control, out document);
		}
		protected Control FindControl(BaseDocument document) {
			foreach(KeyValuePair<Control, BaseDocument> pair in controlLinks)
				if(pair.Value == document) return pair.Key;
			return null;
		}
		protected static BaseDocument GetFloatDocument(Form floatForm) {
			FloatDocumentForm floatDocumentForm = floatForm as FloatDocumentForm;
			return (floatDocumentForm != null) ? floatDocumentForm.Document : null;
		}
		protected void DisposeDocument(Form form, bool raiseClosed) {
			BaseDocument document = GetFloatDocument(form);
			if(document != null || TryGetValue(form, out document)) {
				using(BeginProcessFormClosed(document)) {
					if(raiseClosed)
						ProcessFormClosedCore(document);
					Remove(document);
					Ref.Dispose(ref document);
				}
			}
		}
		protected virtual IDocumentClosedContext BeginProcessFormClosed(BaseDocument document) {
			return null;
		}
		protected void ProcessFormClosed(Form form) {
			BaseDocument document;
			if(TryGetValue(form, out document))
				ProcessFormClosedCore(document);
		}
		void ProcessFormClosedCore(BaseDocument document) {
			document.EnsureFloatingBounds();
			document.SetIsFormClose();
			if(!Views.BaseView.DocumentClosedContext.RequestRaiseDocumentClosed(Owner, document))
				Owner.RaiseDocumentClosed(document);
		}
		#region HOOKS
		bool isAttachedToHookManagerCore;
		protected bool IsAttachedToHookManager {
			get { return isAttachedToHookManagerCore; }
		}
		protected void Hook() {
			if(!IsDisposing && !IsAttachedToHookManager && Owner.CanProcessHooks()) {
				DevExpress.Utils.Win.Hook.HookManager.DefaultManager.AddController(this);
				isAttachedToHookManagerCore = true;
			}
		}
		protected void UnHook() {
			if(Count == 0) {
				UnHookCore();
			}
		}
		protected internal void UnHookCore() {
			DevExpress.Utils.Win.Hook.HookManager.DefaultManager.RemoveController(this);
			isAttachedToHookManagerCore = false;
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return PostFilterMessageCore(Msg, wnd, HWnd, WParam, LParam);
		}
		bool DevExpress.Utils.Win.Hook.IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			CheckDeactivateApp(Msg, wnd, WParam);
			return PreFilterMessageCore(Msg, wnd, HWnd, WParam, LParam);
		}
		IntPtr DevExpress.Utils.Win.Hook.IHookController.OwnerHandle {
			get { return OwnerHandleCore; }
		}
		protected virtual bool PostFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			CheckFocus(Msg, wnd, WParam);
			return CheckClosing(Msg, HWnd, WParam);
		}
		protected virtual bool PreFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		protected virtual IntPtr OwnerHandleCore { get { return IntPtr.Zero; } }
		#endregion HOOKS
		protected void CheckDeactivateApp(int msg, Control control, IntPtr wParam) {
			if(msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_ACTIVATEAPP && wParam == IntPtr.Zero) {
				if(Owner != null && Owner.Manager != null) {
					if(DocumentsHostContext.GetForm(Owner.Manager) == control)
						Owner.DeactivateApp();
				}
			}
		}
		protected void CheckFocus(int msg, Control control, IntPtr wParam) {
			if(msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_SETFOCUS || msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_KILLFOCUS)
				if(Owner != null && Owner.Manager != null) Owner.Manager.CheckFocus(msg, Owner, control, wParam);
		}
		protected bool CheckClosing(int msg, IntPtr hWnd, IntPtr wParam) {
			if(msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_SYSCOMMAND) {
				if(GetDocument(hWnd) != null) {
					int cmd = WinAPIHelper.GetInt(wParam) & 0xFFF0;
					return Owner.ProcessSysCommand(hWnd, cmd);
				}
			}
			return false;
		}
		protected internal BaseDocument GetDocument(IntPtr hWnd) {
			Control control = Control.FromHandle(hWnd);
			if(control == null) return null;
			if(control is FloatDocumentForm)
				control = ((FloatDocumentForm)control).Document.Control;
			BaseDocument result;
			return controlLinks.TryGetValue(control, out result) ? result : null;
		}
		protected internal Form GetForm(IntPtr hWnd) {
			foreach(BaseDocument document in List) {
				Form form = document.Form;
				if(form != null && form.IsHandleCreated && form.Handle == hWnd)
					return form;
			}
			return null;
		}
		#region WM_CLOSE_Interceptor
		protected internal bool OnSCClose(IntPtr hWnd) {
			BaseDocument document = GetDocument(hWnd);
			if(document != null) {
				bool isFormSCCloseAware = (document.Form != null) && document.Form.IsHandleCreated && document.Form.ControlBox;
				if(!isFormSCCloseAware)
					return false;
				if(Owner.RaiseDocumentClosing(document)) {
					new WM_CLOSE_Blocker(hWnd);
					return true;
				}
				else {
					if(!document.IsFloating)
						new WM_CLOSE_Notifier(hWnd, Owner.BeginRaiseDocumentClosed(document));
				}
			}
			return false;
		}
		abstract class WM_CLOSE_Interceptor {
			const int WM_CLOSE = 0x0010;
			DevExpress.Utils.Drawing.Helpers.IWin32Subclasser subclasser;
			DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.WndProc wndProcRef;
			public WM_CLOSE_Interceptor(IntPtr hWnd) {
				if(hWnd != IntPtr.Zero) {
					wndProcRef = new DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.WndProc(WndProc);
					subclasser = DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.Create(hWnd, wndProcRef);
				}
			}
			bool WndProc(ref Message m) {
				if(m.Msg == WM_CLOSE) {
					try {
						OnWmClose(ref m);
						return true;
					}
					finally {
						Ref.Dispose(ref subclasser);
						wndProcRef = null;
					}
				}
				return false;
			}
			protected abstract void OnWmClose(ref Message m);
		}
		class WM_CLOSE_Notifier : WM_CLOSE_Interceptor {
			IDocumentClosedContext closeContext;
			public WM_CLOSE_Notifier(IntPtr hWnd, IDocumentClosedContext closeContext)
				: base(hWnd) {
				this.closeContext = closeContext;
			}
			protected override void OnWmClose(ref Message m) {
				using(closeContext) {
					DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.DefaultSubclassProc(ref m);
				}
			}
		}
		class WM_CLOSE_Blocker : WM_CLOSE_Interceptor {
			public WM_CLOSE_Blocker(IntPtr hWnd)
				: base(hWnd) {
			}
			protected override void OnWmClose(ref Message m) {
				m.Result = IntPtr.Zero;
			}
		}
		#endregion WM_CLOSE_Interceptor
		#region WM_SYSCOMMAND_Interceptor
		protected class WM_SYSCOMMAND_Interceptor {
			WeakReference viewRef;
			DevExpress.Utils.Drawing.Helpers.IWin32Subclasser subclasser;
			DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.WndProc wndProcRef;
			public WM_SYSCOMMAND_Interceptor(BaseView view) {
				this.viewRef = new WeakReference(view);
				this.wndProcRef = new DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.WndProc(WndProc);
			}
			public void Release(Control form) {
				if(form != null && form.IsHandleCreated && Handle == form.Handle)
					ReleaseHandle();
			}
			public void AssignHandle(IntPtr handle) {
				if(Handle == handle) return;
				if(Handle != IntPtr.Zero)
					ReleaseHandleCore();
				AssignHandleCore(handle);
			}
			public void ReleaseHandle() {
				ReleaseHandleCore();
			}
			void AssignHandleCore(IntPtr handle) {
				Ref.Dispose(ref subclasser);
				subclasser = DevExpress.Utils.Drawing.Helpers.Win32SubclasserFactory.Create(handle, wndProcRef);
			}
			void ReleaseHandleCore() {
				Ref.Dispose(ref subclasser);
				wndProcRef = null;
			}
			public IntPtr Handle {
				get { return (subclasser != null) ? subclasser.Handle : IntPtr.Zero; }
			}
			bool WndProc(ref Message m) {
				if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_SYSCOMMAND) {
					var cmd = (WinAPIHelper.GetInt(m.WParam) & 0xFFF0);
					if(cmd == DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE)
						if(OnSCClose())
							m.Result = IntPtr.Zero;
				}
				return false;
			}
			bool OnSCClose() {
				BaseView view = viewRef.Target as BaseView;
				return (view != null) && view.ProcessSysCommand(Handle, DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE);
			}
		}
		#endregion WM_SYSCOMMAND_Interceptor
	}
	public abstract class BaseDocumentCollection<T, TOwner> : BaseMutableListEx<T, TOwner>
		where T : BaseDocument
		where TOwner : BaseComponent {
		protected BaseDocumentCollection(TOwner owner)
			: base(owner) {
		}
	}
}
