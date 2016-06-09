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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Dragging;
namespace DevExpress.XtraBars.Docking2010.Views {
	public class FloatDocumentCollection : BaseDocumentCollection {
		WM_SYSCOMMAND_Interceptor interceptor;
		public FloatDocumentCollection(BaseView view)
			: base(view) {
			interceptor = new WM_SYSCOMMAND_Interceptor(view);
		}
		protected override void OnDispose() {
			interceptor.ReleaseHandle();
			base.OnDispose();
		}
		protected override void OnBeforeElementAdded(BaseDocument element) {
			element.SetIsFloating(true);
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(BaseDocument element) {
			base.OnElementAdded(element);
			element.SetIsFloating(true);
			Owner.Manager.RegisterFloatDocument(element);
		}
		protected override void OnElementRemoved(BaseDocument element) {
			Control form = element.Form ?? FindControl(element);
			interceptor.Release(form);
			Owner.Manager.UnregisterFloatDocument(form);
			base.OnElementRemoved(element);
			element.SetIsFloating(false);
		}
		protected override void SubscribeFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.Activated += OnFloatMDIChildActivated;
				floatForm.Deactivate += OnFloatMDIChildDeactivated;
				floatForm.FormClosed += OnFloatMdiChildClosed;
				floatForm.Disposed += OnFloatMdiChildDisposed;
			}
		}
		protected override void UnsubscribeFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.Activated -= OnFloatMDIChildActivated;
				floatForm.Deactivate -= OnFloatMDIChildDeactivated;
				floatForm.FormClosed -= OnFloatMdiChildClosed;
				floatForm.Disposed -= OnFloatMdiChildDisposed;
			}
		}
		void OnFloatMDIChildActivated(object sender, EventArgs e) {
			Form floatForm = sender as Form;
			Owner.OnFloatMDIChildActivated(floatForm);
			if(floatForm != null && floatForm.IsHandleCreated)
				interceptor.AssignHandle(floatForm.Handle);
		}
		void OnFloatMDIChildDeactivated(object sender, EventArgs e) {
			interceptor.ReleaseHandle();
			Owner.OnFloatMDIChildDeactivated(sender as Form);
		}
		void OnFloatMdiChildDisposed(object sender, EventArgs e) {
			Form floatForm = sender as Form;
			DisposeDocument(floatForm, false);
			UnsubscribeFormEvents(floatForm);
			if(Owner != null && Owner.Manager != null)
				Owner.Manager.UnregisterFloatDocument(floatForm);
		}
		void OnFloatMdiChildClosed(object sender, FormClosedEventArgs e) {
			Form floatForm = sender as Form;
			interceptor.Release(floatForm);
			DisposeDocument(floatForm, true);
		}
		public bool TryGetFloatDocument(Form floatForm, out BaseDocument document) {
			document = GetFloatDocument(floatForm);
			return (document != null) || TryGetValue(floatForm, out document);
		}
		protected override IDocumentClosedContext BeginProcessFormClosed(BaseDocument document) {
			return Owner.BeginRaiseFloatDocumentClosed(document);
		}
		#region IHookController Members
		protected override bool PostFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return (interceptor.Handle != HWnd) && CheckClosing(Msg, HWnd, WParam);
		}
		protected override bool PreFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			Form floatForm = (wnd is Form) ? GetForm(HWnd) : null;
			if(floatForm != null) {
				ProcessWindowDragging(Msg, floatForm);
				if(ProcessDoubleClick(Msg, floatForm))
					return true;
			}
			return false;
		}
		bool ProcessDoubleClick(int Msg, Form floatForm) {
			if(Msg != 0x00A3 ) return false;
			if(((Control.ModifierKeys & Keys.Control) != 0)) {
				BaseDocument document;
				if(TryGetFloatDocument(floatForm, out document))
					return Owner.Controller.Dock(document);
			}
			return false;
		}
		protected override IntPtr OwnerHandleCore {
			get { return (dragForm != null) ? dragForm.Handle : IntPtr.Zero; }
		}
		Form dragForm;
		void ProcessWindowDragging(int Msg, Form floatForm) {
			if(dragForm == null) {
				if(Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_NCLBUTTONDOWN) {
					dragForm = floatForm;
					ProcessDragging(BeginDragging, dragForm);
				}
			}
			else {
				if(dragForm == floatForm) {
					switch(Msg) {
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEMOVE:
							ProcessDragging(Dragging, dragForm);
							break;
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_NCLBUTTONUP: 
						case DevExpress.Utils.Drawing.Helpers.MSG.WM_LBUTTONUP:
							ProcessDragging(EndDragging, dragForm);
							break;
					}
				}
			}
		}
		void ProcessDragging(dragging action, Form form) {
			DevExpress.Utils.Win.Hook.HookInfo info;
			if(DevExpress.Utils.Win.Hook.HookManager.DefaultManager.HookHash.TryGetValue(DevExpress.Utils.Win.Hook.HookManager.CurrentThread, out info)) {
				action(form, info.GetPoint());
			}
		}
		delegate void dragging(Form form, Point point);
		#endregion
		void BeginDragging(Form form, Point screenPoint) {
			if(NCHitTest(form, screenPoint) == DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTCAPTION) {
				FloatFormUIView view = (FloatFormUIView)Owner.Manager.UIViewAdapter.GetView(form);
				if(view != null) view.BeginExternalDragging(screenPoint);
			}
			else dragForm = null;
		}
		void Dragging(Form form, Point screenPoint) {
			FloatFormUIView view = (FloatFormUIView)Owner.Manager.UIViewAdapter.GetView(form);
			if(view != null) view.ExternalDragging(screenPoint);
		}
		void EndDragging(Form form, Point screenPoint) {
			dragForm = null;
			FloatFormUIView view = (FloatFormUIView)Owner.Manager.UIViewAdapter.GetView(form);
			if(view != null) {
				Action endDragging = () => view.EndExternalDragging(screenPoint);
				var context = BaseFloatDocumentForm.GetDragMoveContext(form);
				if(context != null)
					context.Queue(endDragging);
				else
					endDragging();
			}
		}
		static int NCHitTest(Form form, Point screenPoint) {
			return DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(form.Handle, 0x84, IntPtr.Zero, MAKELPARAM(screenPoint.X, screenPoint.Y));
		}
		static IntPtr MAKELPARAM(int low, int high) {
			return (IntPtr)((high << 0x10) | (low & 0xffff));
		}
		#region Locker
		class Locker : IDisposable {
			FloatDocumentCollection Target;
			public Locker(FloatDocumentCollection collection) {
				Target = collection;
				Target.lockCounter++;
			}
			public void Dispose() {
				if(Target != null) {
					--Target.lockCounter;
					Target = null;
				}
			}
		}
		int lockCounter = 0;
		public IDisposable Lock() {
			return new Locker(this);
		}
		public bool IsLocked {
			get { return lockCounter > 0; }
		}
		#endregion Locker
	}
}
