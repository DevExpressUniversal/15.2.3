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
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	public class MDIClientManagementStrategy : DocumentManagementStrategy {
		IMdiClientSubscriber mdiClientSubscriber;
		IMdiClientSubclasser mdiSubclasserCore;
		public MDIClientManagementStrategy(DocumentManager manager)
			: base(manager) {
		}
		protected override void OnDispose() {
			Ref.Dispose(ref mdiClientSubscriber);
			Ref.Dispose(ref mdiSubclasserCore);
		}
		public override bool IsValid {
			get { return Manager.MdiParent != null; }
		}
		public sealed override ContainerControl Container {
			get { return Manager.MdiParent; }
		}
		protected internal IMdiClientSubclasser Subclasser {
			get { return mdiSubclasserCore; }
		}
		public sealed override bool IsOwnerControlEmpty {
			get { return Manager.MdiParent.MdiChildren.Length == 0; }
		}
		protected virtual IMdiClientSubclasser CreateSubclasser(MdiClient client) {
			return (Manager.Site != null && Manager.Site.DesignMode) ?
				(IMdiClientSubclasser)new DocumentManagerMdiClientDesignTimeSubclasser(client, Manager) :
				(IMdiClientSubclasser)new DocumentManagerMdiClientSubclasser(client, Manager);
		}
		protected virtual IMdiClientSubscriber CreateMdiClientSubscriber(MdiClient mdiClient) {
			return new MdiClientSubscriber(mdiClient, Manager);
		}
		public override Control Initialize(ContainerControl container) {
			Form mdiParent = container as Form;
			mdiParent.IsMdiContainer = true;
			MdiClient client = MdiClientSubclasser.GetMdiClient(mdiParent);
			mdiSubclasserCore = CreateSubclasser(client);
			mdiClientSubscriber = CreateMdiClientSubscriber(client);
			return client;
		}
		public override void Destroy(ContainerControl container, bool handleRecreating) {
			if(!handleRecreating)
				Ref.Dispose(ref mdiSubclasserCore);
			else IsOwnerControlInvalid = true;
		}
		public override void Ensure(Control client) {
			MdiClient mdiClient = client as MdiClient;
			if(mdiClientSubscriber != null)
				mdiClientSubscriber.Ensure(mdiClient);
			else
				mdiClientSubscriber = CreateMdiClientSubscriber(mdiClient);
		}
		protected override void SubscribeCore(ContainerControl container) {
			base.SubscribeCore(container);
			((Form)container).MdiChildActivate += OnMdiChildActivate;
		}
		protected override void UnSubscribeCore(ContainerControl container) {
			((Form)container).MdiChildActivate -= OnMdiChildActivate;
			base.UnSubscribeCore(container);
		}
#pragma warning disable 0618
		void OnMdiChildActivate(object sender, EventArgs e) {
			Manager.InvokePatchActiveChildren(Manager.MdiParent);
			Form activeChild = GetActiveChild(Manager.MdiParent);
			if(activeChild != null && !activeChild.Disposing) {
				if(activeChild.ActiveControl == null || !activeChild.ActiveControl.Visible)
					DocumentManager.SelectNextControl(activeChild);
				Control controlToActivate = activeChild;
				FloatDocumentForm floatDocumentForm = activeChild as FloatDocumentForm;
				if(floatDocumentForm != null) {
					if(floatDocumentForm.IsDisposing) {
						DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.DisableBitmapAnimation = false;
						return;
					}
					controlToActivate = floatDocumentForm.Document.Control;
				}
				if(!Manager.View.IgnoreActiveFormOnActivation) {
					IContainerControl container = Manager.MdiParent as IContainerControl;
					if(container != null && container.ActiveControl != activeChild)
						container.ActivateControl(activeChild);
					Manager.View.ActivateDocument(controlToActivate);
				}
				DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.DisableBitmapAnimation = false;
			}
		}
#pragma warning restore 0618
		public sealed override Control GetOwnerControl() {
			return (Subclasser != null) ? Subclasser.ClientWindow : null;
		}
		public override void UpdateLayout() {
			Subclasser.ProcessNC();
		}
		public override bool CanUpdateLayout() {
			if(Subclasser == null || IsDisposing)
				return false;
			if(!Subclasser.ClientWindow.IsHandleCreated)
				return false;
			if(Manager.MdiParent == null)
				return false;
			if(DevExpress.Utils.Mdi.MdiClientSubclasserService.IsUpdatingMdiClient(Subclasser.ClientWindow)) {
				if(!Subclasser.ClientWindow.Bounds.IsEmpty || !Manager.Bounds.IsEmpty)
					return false;
			}
			return true;
		}
		public override Control GetActiveChild() {
			return GetActiveChild(Manager.MdiParent);
		}
		public override void Activate(Control child) {
			if(Manager.View.AllowMdiLayout)
				DocumentManagerMdiClientSubclasser.SendMdiNext(Subclasser, child);
			else {
				if(child != null)
					child.Select();
			}
		}
		public override void PostFocus(Control child) {
			MdiClient mdiClient = Subclasser.ClientWindow;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.PostMessage(mdiClient.Handle,
				DevExpress.Utils.Drawing.Helpers.MSG.WM_SETFOCUS,
				child.Handle, IntPtr.Zero);
		}
		Form GetActiveChild(Form mdiParent) {
			return (mdiParent != null) ? mdiParent.ActiveMdiChild : null;
		}
		public override Control GetChild(BaseDocument document) {
			return document.Form;
		}
		public sealed override void AddDocumentToHost(BaseDocument document) {
			FloatDocumentForm floatForm = document.Form as FloatDocumentForm;
			if(floatForm != null)
				floatForm.SetManager(Manager);
			document.Form.MdiParent = Manager.MdiParent;
			if(Manager.View != null && !Manager.View.AllowMdiLayout)
				document.Form.Location = new Point(-document.Form.Width, -document.Form.Height);
			document.Form.Show();
		}
		public sealed override void RemoveDocumentFromHost(BaseDocument document) {
			document.Form.MdiParent = null;
		}
		public sealed override void DockFloatForm(BaseDocument document, Docking.DockPanel panel) {
			using(var context = new ChildDocumentManagerBeginInvokeContext(panel.FloatForm)) {
				context.RequestPatchActiveChildren();
				panel.FloatForm.MdiParent = Manager.MdiParent;
				panel.FloatForm.Visible = true;
			}
		}
		public sealed override void DockFloatForm(BaseDocument document, Action<Form> patchAction) {
			Form form = document.Form;
			if(form != null) {
				using(var context = new ChildDocumentManagerBeginInvokeContext(form)) {
					if(form.MdiParent == Manager.MdiParent)
						patchAction(form);
					else {
						context.RequestPatchActiveChildren();
						form.MdiParent = Manager.MdiParent;
					}
					form.Visible = true;
				}
			}
		}
		public sealed override bool CheckFocus(int Msg, BaseView view, Control control, IntPtr wParam) {
			Form form = control as Form;
			switch(Msg) {
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_KILLFOCUS:
					form = CheckForm(control, form);
					if(form != null && form.IsMdiChild) {
						Control target = WinAPIHelper.FindControl(wParam);
						if(target == Subclasser.ClientWindow && view.IsFocused)
							return true;
						if(target == null || form.MdiParent == target.FindForm())
							return view.FocusLeave();
					}
					break;
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_SETFOCUS:
					if(control == null)
						control = WinAPIHelper.FindControl(wParam);
					if(control == Subclasser.ClientWindow && view.IsFocused)
						return true;
					form = CheckForm(control, form);
					if(form != null && form.IsMdiChild) {
						if(form.MdiParent == Manager.MdiParent) {
							if(view.ActiveDocument == null || view.ActiveDocument.Form != form) {
								Control activeChild = form;
								FloatDocumentForm floatForm = form as FloatDocumentForm;
								if(floatForm != null && floatForm.Document != null)
									activeChild = floatForm.Document.Control;
								if(!activeChild.Disposing && activeChild.IsHandleCreated) {
									MdiClient mdiClient = activeChild.Parent as MdiClient;
									if(mdiClient != null && mdiClient.IsHandleCreated) {
										NativeMethods.SendMessage(mdiClient.Handle,
											 MSG.WM_MDIACTIVATE, activeChild.Handle, IntPtr.Zero);
									}
								}
							}
							return view.FocusEnter();
						}
					}
					else return view.FocusLeave();
					break;
			}
			return false;
		}
		protected Form CheckForm(Control control, Form form) {
			return DocumentsHostContext.CheckForm(control, form);
		}
		public sealed override void PatchControlBeforeAdd(Control control) {
			Form childForm = control as Form;
			if(childForm != null) {
				bool alreadyRegistered = FormBorderInfo.Contains(childForm);
				if(Manager.InMdiClientControlAdded || !alreadyRegistered)
					FormBorderInfo.Attach(childForm);
				if(!alreadyRegistered) {
					childForm.MinimizeBox = false;
					childForm.MaximizeBox = false;
					childForm.MinimumSize = Size.Empty;
					childForm.MaximumSize = Size.Empty;
					childForm.FormBorderStyle = FormBorderStyle.None;
					childForm.WindowState = FormWindowState.Normal;
					childForm.Dock = DockStyle.None;
					childForm.StartPosition = FormStartPosition.Manual;
					childForm.Location = new Point(-childForm.Width, -childForm.Height);
				}
			}
		}
		public sealed override void PatchControlAfterRemove(Control control) {
			Form childForm = control as Form;
			if(childForm != null) {
				FormBorderStyle borderStyle = (childForm is Docking.FloatForm)
					? FormBorderStyle.None : FormBorderStyle.Sizable;
				DevExpress.Utils.Mdi.MdiChildHelper.PatchAfterUnregister(childForm, borderStyle);
				childForm.Location = Point.Empty;
				FormBorderInfo.Detach(childForm);
			}
		}
		public sealed override void PatchDocumentBeforeFloat(BaseDocument document) {
			document.Form.MdiParent = null;
		}
	}
	class DocumentManagerMdiClientDesignTimeSubclasser : MdiClientSubclasser {
		public DocumentManagerMdiClientDesignTimeSubclasser(MdiClient client, DocumentManager manager)
			: base(client, manager) {
		}
		bool isRegionValid;
		protected override void DoResize(ref Message m) {
			base.DoResize(ref m);
			if(setRegion == 0)
				isRegionValid = false;
		}
		int setRegion = 0;
		protected override void DoEraseBackground(ref Message m) {
			if(!isRegionValid && setRegion == 0) {
				setRegion++;
				NativeMethods.SetWindowRgn(Handle, IntPtr.Zero, false);
				setRegion--;
				isRegionValid = true;
			}
			m.Result = new IntPtr(1);
		}
		protected override void DoPaint(ref Message m) {
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			try {
				IntPtr dc = NativeMethods.BeginPaint(Handle, ref ps);
				using(Graphics g = Graphics.FromHdcInternal(dc)) {
					Owner.Paint(g);
				}
			}
			finally {
				NativeMethods.EndPaint(Handle, ref ps);
				m.Result = IntPtr.Zero;
			}
		}
	}
	class DocumentManagerMdiClientSubclasser : MdiClientSubclasser, ISysCommandListener {
		public DocumentManagerMdiClientSubclasser(MdiClient client, DocumentManager manager)
			: base(client, manager) {
		}
		public static void SendMdiNext(IMdiClientSubclasser subclasser, Control child) {
			((MdiClientSubclasser)subclasser).Ignore_WM_MDINEXT++;
			try {
				Control nextChild = null;
				int index = subclasser.ClientWindow.Controls.IndexOf(child);
				if(index == 0)
					nextChild = subclasser.ClientWindow.Controls[1];
				else
					nextChild = subclasser.ClientWindow.Controls[index - 1];
				IntPtr direction = new IntPtr(index == 0 ? 1 : 0);
				NativeMethods.SendMessage(subclasser.ClientWindow.Handle, MSG.WM_MDINEXT, nextChild.Handle, direction);
			}
			finally { ((MdiClientSubclasser)subclasser).Ignore_WM_MDINEXT--; }
		}
		Form parentNotifyChild;
		protected override void DoParentNotify(ref Message m) {
			var msg = WinAPIHelper.GetInt(m.WParam) & 0xFFFF;
			Point? msgLocation = (msg > 0x200) ? new Point?(WinAPIHelper.GetPoint(m.LParam)) : null;
			if(msgLocation.HasValue && ClientWindow.IsHandleCreated)
				this.parentNotifyChild = ClientWindow.GetChildAtPoint(msgLocation.Value) as Form;
			base.DoParentNotify(ref m);
			if(msg == MSG.WM_LBUTTONDOWN && msgLocation.HasValue) {
				if(ParentNotifyHelper.ShouldSelectChild(ClientWindow, msgLocation.Value, parentNotifyChild)) {
					if(parentNotifyChild != parentNotifyChild.MdiParent.ActiveMdiChild)
						parentNotifyChild.Select();
				}
			}
			this.parentNotifyChild = null;
		}
		protected override void DoNCActivate(ref Message m) {
			BaseView view = GetView();
			if(parentNotifyChild != null && (view != null) && (view.Type == ViewType.NativeMdi))
				DoBaseWndProc(ref m);
			else base.DoNCActivate(ref m);
		}
		protected override void DoResize(ref Message m) {
			ClientWindow.Invalidate(false);
			base.DoResize(ref m);
		}
		protected override void DoEraseBackground(ref Message m) {
			m.Result = new IntPtr(1);
		}
		protected override void DoPaint(ref Message m) {
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			try {
				IntPtr dc = NativeMethods.BeginPaint(Handle, ref ps);
				using(Graphics g = Graphics.FromHdc(dc)) {
					Owner.Paint(g);
				}
			}
			finally {
				NativeMethods.EndPaint(Handle, ref ps);
				m.Result = IntPtr.Zero;
			}
		}
		protected override void DoPrint(ref Message m) {
			try {
				using(Graphics g = Graphics.FromHdc(m.WParam))
					Owner.Paint(g);
			}
			finally { m.Result = IntPtr.Zero; }
		}
		void ISysCommandListener.PreviewMessage(IntPtr hWnd, int cmd) {
			BaseView view = GetView();
			if(view != null && view.CanProcessSysCommand(hWnd, cmd))
				view.ProcessSysCommand(hWnd, cmd);
		}
		BaseView GetView() {
			return (Owner != null) ? ((DocumentManager)Owner).View : null;
		}
	}
	class ChildDocumentManagerBeginInvokeContext : IDisposable {
		DocumentManager childManager;
		public ChildDocumentManagerBeginInvokeContext(Form childForm) {
			var host = DocumentsHost.GetDocumentsHost(childForm);
			if(host != null) {
				childManager = host.Owner as DocumentManager;
				if(childManager != null)
					childManager.lockPatchActiveChildrenInChildManager++;
			}
		}
		public void Dispose() {
			if(childManager != null) {
				if(--childManager.lockPatchActiveChildrenInChildManager == 0) {
					if(requestPatch > 0)
						childManager.InvokePatchActiveChildren(childManager.GetContainer());
				}
			}
			childManager = null;
		}
		int requestPatch;
		public void RequestPatchActiveChildren() {
			requestPatch++;
		}
	}
}
