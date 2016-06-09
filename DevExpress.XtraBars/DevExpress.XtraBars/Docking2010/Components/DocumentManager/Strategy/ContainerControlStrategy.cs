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
using System.Windows.Forms;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	public class ContainerControlManagementStrategy : DocumentManagementStrategy {
		internal const string AddToDocumentHostError =
			"A Form cannot be used as Document's content if the MDI mode is not active.\r\n" +
			"Use a UserControl instead or enable the MDI mode via the DocumentManager.MdiParent property.";
		public ContainerControlManagementStrategy(DocumentManager manager)
			: base(manager) {
		}
		protected override void OnDispose() {
			UnSubscribeHostEvents(Host);
			Ref.Dispose(ref hostCore);
		}
		public override bool IsValid {
			get { return Manager.ContainerControl != null; }
		}
		public sealed override ContainerControl Container {
			get { return Manager.ContainerControl; }
		}
		IDocumentsHost hostCore;
		public IDocumentsHost Host {
			get { return hostCore; }
		}
		protected virtual IDocumentsHost CreateDocumentsHost(IDocumentsHostOwner owner) {
			return Manager.View != null ? (Manager as IViewRegistrator).CreateDocumentsHost(owner) as IDocumentsHost : new DocumentsHost(owner);
		}
		protected virtual void SubscribeHostEvents(IDocumentsHost host) {
			if(host != null)
				host.DocumentContainerActivate += OnDocumentContainerActivate;
		}
		protected virtual void UnSubscribeHostEvents(IDocumentsHost host) {
			if(host != null)
				host.DocumentContainerActivate -= OnDocumentContainerActivate;
		}
		public sealed override Control Initialize(ContainerControl container) {
			hostCore = GetDocumentsHost(container);
			if(hostCore == null) {
				hostCore = CreateDocumentsHost(Manager);
				SubscribeHostEvents(Host);
				container.Controls.Add(Host as Control);
				container.Controls.SetChildIndex(Host as Control, 0);
			}
			return hostCore as Control;
		}
		public sealed override void Destroy(ContainerControl container, bool handleRecreating) {
			if(!handleRecreating)
				UnSubscribeHostEvents(Host);
			else IsOwnerControlInvalid = true;
		}
		public sealed override void Ensure(Control client) {
			SubscribeHostEvents(Host);
		}
		public sealed override Control GetOwnerControl() {
			return Host as Control;
		}
		public sealed override bool IsOwnerControlEmpty {
			get { return Host == null || !Host.IsHandleCreated || Host.Controls.Count == 0; }
		}
		public sealed override void UpdateLayout() {
			if(Host != null && Host.IsHandleCreated)
				Host.UpdateLayout();
		}
		public sealed override bool CanUpdateLayout() {
			if(Host == null || IsDisposing)
				return false;
			if(!Host.IsHandleCreated)
				return false;
			if(Manager.ContainerControl == null)
				return false;
			return true;
		}
		protected override void SubscribeCore(ContainerControl container) {
			base.SubscribeCore(container);
			IDocumentsHost host = Host ?? GetDocumentsHost(container);
			SubscribeHostEvents(host);
		}
		protected override void UnSubscribeCore(ContainerControl container) {
			IDocumentsHost host = Host ?? GetDocumentsHost(container);
			UnSubscribeHostEvents(host);
			base.UnSubscribeCore(container);
		}
		protected IDocumentsHost GetDocumentsHost(ContainerControl container) {
			IDocumentsHost host = DocumentsHost.GetDocumentsHost(container);
			if(host != null && host.Owner == Manager)
				return host;
			return null;
		}
		void OnDocumentContainerActivate(object sender, EventArgs e) {
			Manager.InvokePatchActiveChildren(Manager.ContainerControl);
			DocumentContainer activeChild = Host.ActiveContainer;
			if(activeChild != null) {
				SelectDocumentContainerContent(activeChild);
				if(!Manager.View.IgnoreActiveFormOnActivation)
					Manager.View.ActivateDocument(GetContent(activeChild));
			}
		}
		void SelectDocumentContainerContent(DocumentContainer activeChild) {
			if(!activeChild.Focused) return;
			Control content = CheckNestedContent(GetContent(activeChild));
			if(isInSetFocus == 0)
				DocumentManager.SelectNextControl(content);
			else
				Manager.QueueSelectNextControl(content);
		}
		internal static Control GetContent(DocumentContainer container) {
			return (container != null && container.Document != null) ? container.Document.Control : null;
		}
		internal static Control CheckNestedContent(Control content) {
			var nestedHost = DocumentsHost.GetDocumentsHostCore(content);
			if(nestedHost != null) {
				var nestedContent = GetContent(nestedHost.ActiveContainer);
				if(nestedContent != null)
					content = CheckNestedContent(nestedContent);
			}
			return content;
		}
		public sealed override Control GetActiveChild() {
			return (Host != null) ? Host.ActiveContainer : null;
		}
		public override void Activate(Control child) {
			DocumentContainer container = child as DocumentContainer;
			if(container != null) {
				Control content = CheckNestedContent(GetContent(container));
				if(isInSetFocus == 0) {
					Manager.QueueSelectNextControl(null);
					container.Select();
				}
				else Manager.QueueSelectNextControl(content);
			}
			if(Host != null) 
				Host.SetActiveContainer(container);
		}
		public override void PostFocus(Control child) {
			if(Host == null) return;
			if(isInSetFocus == 0) {
				DevExpress.Utils.Drawing.Helpers.NativeMethods.PostMessage(Host.Handle,
					DevExpress.Utils.Drawing.Helpers.MSG.WM_SETFOCUS,
					child.Handle, IntPtr.Zero);
			}
			Host.SetActiveContainer(child as DocumentContainer);
		}
		public sealed override Control GetChild(BaseDocument document) {
			DocumentContainer container = document.Control.Parent as DocumentContainer;
			if(container == null && document.IsDockPanel && !document.IsVisible)
				container = document.GetDockPanel().parentContainer;
			return container;
		}
		public sealed override void Ensure(BaseDocument document) {
			document.MarkAsNonMDI();
		}
		public sealed override void AddDocumentToHost(BaseDocument document) {
			if(document.Control is Form)
				throw new NotSupportedException(AddToDocumentHostError);
			DocumentContainer container = document.Control.Parent as DocumentContainer;
			container.SuspendLayout();
			container.Parent = null;
			container.Dock = DockStyle.None;
			container.SetManager(Manager);
			if(Manager.View != null && !Manager.View.AllowMdiLayout)
				container.Location = new System.Drawing.Point(-container.Width, -container.Height);
			Host.Controls.Add(container);
			container.ResumeLayout();
			container.Show();
		}
		int iRemovingFromHost = 0;
		public sealed override void RemoveDocumentFromHost(BaseDocument document) {
			iRemovingFromHost++;
			try {
				DocumentContainer container = document.Control.Parent as DocumentContainer;
				if(!document.IsDockPanel) {
					if(!document.FloatSize.HasValue)
						document.Form.ClientSize = container.ClientSize;
					Host.Controls.Remove(container);
					container.Dock = DockStyle.Fill;
					container.Parent = document.Form;
					UpdateRibbonVisibility(container);
				}
				else {
					Docking.DockPanel panel = document.GetDockPanel();
					container = container ?? panel.parentContainer; 
					panel.Parent = document.Form;
					panel.ControlDock = DockStyle.None;
					if(panel.DockLayout != null)
						panel.DockLayout.AdjustControlBounds();
					document.controlCore = document.Form;
					Manager.View.Documents.ReregisterControl(panel, document);
					Host.Controls.Remove(container);
					container.LockDocumentDisposing();
					Ref.Dispose(ref container);
				}
			}
			finally { iRemovingFromHost--; }
		}
		public sealed override void DockFloatForm(BaseDocument document, Action<Form> patchAction) {
			if(!document.IsDockPanel) {
				DocumentContainer container = document.Control.Parent as DocumentContainer;
				container.Parent = null;
				container.Dock = DockStyle.None;
				container.SetManager(Manager);
				if(Manager.View != null && !Manager.View.AllowMdiLayout)
					container.Location = new System.Drawing.Point(-container.Width, -container.Height);
				Host.Controls.Add(container);
				SelectContainer(container);
				UpdateRibbonVisibility(container);
			}
			else {
				Docking.DockPanel panel = document.GetDockPanel();
				if(panel == null || document.IsDisposing)
					return;
				DockToNewDocumentContainer(document, panel);
			}
			Hide(document.Form.Handle);
		}
		void UpdateRibbonVisibility(DocumentContainer container) {
			if(Manager.RibbonAndBarsMergeStyle != RibbonAndBarsMergeStyle.WhenNotFloating) return;
			container.UpdateInnerRibbonVisibility();
		}
		public sealed override void DockFloatForm(BaseDocument document, Docking.DockPanel panel) {
			DockToNewDocumentContainer(document, panel);
		}
		protected void DockToNewDocumentContainer(BaseDocument document, Docking.DockPanel panel) {
			DocumentContainer container = Manager.View.CreateDocumentContainer(document);
			document.controlCore = panel;
			Manager.View.Documents.ReregisterControl(panel.FloatForm, document);
			panel.Parent = container;
			panel.ControlDock = DockStyle.Fill;
			document.Control.Dock = DockStyle.Fill;
			if(Manager.View != null && !Manager.View.AllowMdiLayout)
				container.Location = new System.Drawing.Point(-container.Width, -container.Height);
			Host.Controls.Add(container);
			panel.DockLayout.LayoutChanged();
			if(panel.ActivateWhenDockingAsMdiDocument)
				SelectContainer(container);
		}
		void SelectContainer(DocumentContainer container) {
			var hostsContext = Manager.GetDocumentsHostContext();
			if(hostsContext != null) {
				if(DocumentsHostContext.CheckHostFormActive(Manager, (hostForm) =>
				{
					container.Select();
					hostForm.Select();
				})) return;
			}
			container.Select();
		}
		int isInSetFocus = 0;
		public sealed override bool CheckFocus(int Msg, BaseView view, Control control, IntPtr wParam) {
			DocumentContainer container = control as DocumentContainer;
			switch(Msg) {
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_KILLFOCUS:
					if(container == null && control != null)
						container = DocumentContainer.FromControl(control);
					if(container != null && container.Parent != null) {
						Control target = WinAPIHelper.FindControl(wParam);
						DocumentContainer targetContainer = DocumentContainer.FromControl(target);
						if(view.IsFocused) {
							if(targetContainer != null) {
								if(DocumentsHostContext.IsChild(targetContainer, (Control)Host))
									return true;
								else {
									if(targetContainer.Parent != null)
										return view.FocusLeave();
								}
							}
							if((target == Host || target == Manager.GetContainer())
								&& (view.IsDocumentClosing(Host.ActiveDocument) || iRemovingFromHost > 0))
								return true;
						}
						if(target == null || container.FindForm() == target.FindForm())
							return view.FocusLeave();
					}
					break;
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_SETFOCUS:
					isInSetFocus++;
					try {
						if(view.IsFocused) {
							if((control == Host || control == Manager.GetContainer())
								&& (view.IsDocumentClosing(Host.ActiveDocument) || iRemovingFromHost > 0)) {
								if(Host.ActivatePreviousDocument(Host.ActiveDocument, view))
									return true;
							}
						}
						if(control == null)
							control = WinAPIHelper.FindControl(wParam);
						if(container == null && control != null)
							container = DocumentContainer.FromControl(control);
						if(container != null && container.Parent != null) {
							if(container.FindForm() == DocumentsHostContext.GetForm(Manager)) {
								BaseDocument activeDocument = view.ActiveDocument;
								if(activeDocument == null ||
									(activeDocument.IsControlLoaded && activeDocument.Control.Parent != container)) {
									if(Host != null && !container.IsDisposing) {
										DocumentContainer parentContainer = container;
										while(parentContainer != null && !parentContainer.IsDisposing) {
											var parentHost = parentContainer.Parent;
											if(Host != parentHost)
												parentContainer = DocumentContainer.FromControl(parentHost);
											else {
												Host.SetActiveContainer(parentContainer);
												break;
											}
										}
									}
								}
								if(DocumentsHostContext.IsChild(container, (Control)Host))
									return view.FocusEnter();
							}
							else {
								if(container.Parent is FloatDocumentForm)
									view.FocusLeave();
							}
						}
						else return view.FocusLeave();
					}
					finally { isInSetFocus--; }
					break;
			}
			return false;
		}
		public sealed override void PatchControlBeforeAdd(Control control) { }
		public sealed override void PatchControlAfterRemove(Control control) { }
		public sealed override void PatchDocumentBeforeFloat(BaseDocument document) {
			DocumentContainer container = document.Control.Parent as DocumentContainer;
			if(!document.IsDockPanel) {
				if(!document.FloatSize.HasValue)
					document.Form.ClientSize = container.ClientSize;
				container.Dock = DockStyle.Fill;
				container.Parent = document.Form;
			}
			else {
				Docking.DockPanel panel = document.GetDockPanel();
				panel.Parent = document.Form;
				panel.ControlDock = DockStyle.None;
				if(panel.DockLayout != null)
					panel.DockLayout.AdjustControlBounds();
				document.controlCore = document.Form;
				Manager.View.Documents.ReregisterControl(panel, document);
				container.LockDocumentDisposing();
				Ref.Dispose(ref container);
			}
		}
		internal static void Hide(IntPtr handle) {
			int flags = DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_HIDEWINDOW |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOREDRAW |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOMOVE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOSIZE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOOWNERZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOACTIVATE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOSENDCHANGING;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, flags);
		}
		public static void ProcessNC(IntPtr handle) {
			BarNativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
				WinAPI.SWP_FRAMECHANGED | WinAPI.SWP_NOACTIVATE | WinAPI.SWP_NOCOPYBITS |
				WinAPI.SWP_NOMOVE | WinAPI.SWP_NOOWNERZORDER | WinAPI.SWP_NOSIZE | WinAPI.SWP_NOZORDER);
		}
	}
	class NestedDocumentManagerUpdateContext : IDisposable {
		DocumentManager childManager;
		public NestedDocumentManagerUpdateContext(Control child) {
			var host = DocumentsHost.GetDocumentsHostCore(child);
			if(host != null) {
				childManager = host.Owner as DocumentManager;
				if(childManager != null)
					childManager.lockUpdateLayout++;
			}
		}
		public void Dispose() {
			if(childManager != null) {
				if(--childManager.lockUpdateLayout == 0)
					childManager.LayoutChanged();
			}
			childManager = null;
		}
	}
}
