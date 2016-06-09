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
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	public interface IDocumentManagementStrategy : IDisposable {
		DocumentManager Manager { get; }
		ContainerControl Container { get; }
		bool IsValid { get;}
		bool IsOwnerControlEmpty { get; }
		bool IsOwnerControlInvalid { get; set; }
		Control Initialize(ContainerControl container);
		void Destroy(ContainerControl container);
		void Destroy(ContainerControl container, bool handleRecreating);
		void Ensure(Control client);
		void Subscribe(ContainerControl container);
		void UnSubscribe();
		Control GetOwnerControl();
		IntPtr GetOwnerControlHandle();
		bool IsOwnerControlHandleCreated { get; }
		bool IsOwnerControlRecreatingHandle { get; }
		bool CanUpdateLayout();
		void UpdateLayout();
		void Invalidate();  
		void Invalidate(Rectangle rect);
		void Update();
		Control GetActiveChild();
		void Activate(Control child);
		void PostFocus(Control child);
		bool OnInitialized();
		Control GetChild(BaseDocument document);
		void Ensure(BaseDocument document);
		void AddDocumentToHost(BaseDocument document);
		void RemoveDocumentFromHost(BaseDocument document);
		void DockFloatForm(BaseDocument document, Action<Form> patchAction);
		void DockFloatForm(BaseDocument document);
		void DockFloatForm(BaseDocument document, Docking.DockPanel panel);
		bool CheckFocus(int Msg, BaseView view, Control control, IntPtr wParam);
		void PatchControlBeforeAdd(Control control);
		void PatchControlAfterRemove(Control control);
		void PatchDocumentBeforeFloat(BaseDocument document);
	}
	public abstract class DocumentManagementStrategy : IDocumentManagementStrategy {
		DocumentManager managerCore;
		public DocumentManagementStrategy(DocumentManager manager) {
			managerCore = manager;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				OnDispose();
				managerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		public DocumentManager Manager {
			get { return managerCore; }
		}
		protected abstract void OnDispose();
		public abstract ContainerControl Container { get; }
		public bool IsOwnerControlInvalid { get; set; }
		public abstract bool IsValid { get; }
		public abstract bool IsOwnerControlEmpty { get; }
		public abstract Control Initialize(ContainerControl container);
		public void Destroy(ContainerControl container) {
			Destroy(container, false);
		}
		public abstract void Ensure(Control client);
		public abstract void Destroy(ContainerControl container, bool handleRecteating);
		ContainerControl subscription;
		public void Subscribe(ContainerControl container) {
			if(container == null || Manager.IsInitializing) return;
			if(subscription == null) {
				SubscribeCore(container);
				subscription = container;
			}
		}
		public void UnSubscribe() {
			if(subscription != null)
				UnSubscribeCore(subscription);
			subscription = null;
		}
		protected virtual void SubscribeCore(ContainerControl container) {
			container.RightToLeftChanged += container_RightToLeftChanged;
		}
		protected virtual void UnSubscribeCore(ContainerControl container) {
			container.RightToLeftChanged -= container_RightToLeftChanged;
		}
		void container_RightToLeftChanged(object sender, EventArgs e) {
			if(Manager != null) Manager.LayoutChanged();
		}
		IntPtr IDocumentManagementStrategy.GetOwnerControlHandle() {
			Control client = GetOwnerControl();
			return (client != null) ? client.Handle : IntPtr.Zero;
		}
		bool IDocumentManagementStrategy.IsOwnerControlHandleCreated {
			get {
				Control client = GetOwnerControl();
				return (client != null) && client.IsHandleCreated;
			}
		}
		bool IDocumentManagementStrategy.IsOwnerControlRecreatingHandle {
			get {
				Control client = GetOwnerControl();
				return (client != null) && client.RecreatingHandle;
			}
		}
		void IDocumentManagementStrategy.Invalidate() {
			GetOwnerControl().Invalidate();
		}
		void IDocumentManagementStrategy.Invalidate(Rectangle rect) {
			GetOwnerControl().Invalidate(rect);
		}
		void IDocumentManagementStrategy.Update() {
			GetOwnerControl().Update();
		}
		bool IDocumentManagementStrategy.OnInitialized() {
			if(Container != null) {
				Subscribe(Container);
				return true;
			}
			return false;
		}
		void IDocumentManagementStrategy.DockFloatForm(BaseDocument document) {
			DockFloatForm(document, PatchControlBeforeAdd);
		}
		public abstract bool CheckFocus(int Msg, BaseView view, Control control, IntPtr wParam);
		public abstract Control GetOwnerControl();
		public abstract void Activate(Control child);
		public abstract void PostFocus(Control child);
		public abstract Control GetActiveChild();
		public abstract bool CanUpdateLayout();
		public abstract void UpdateLayout();
		public abstract Control GetChild(BaseDocument document);
		public virtual void Ensure(BaseDocument document) { }
		public abstract void AddDocumentToHost(BaseDocument document);
		public abstract void RemoveDocumentFromHost(BaseDocument document);
		public abstract void DockFloatForm(BaseDocument document, Docking.DockPanel panel);
		public abstract void DockFloatForm(BaseDocument document, Action<Form> patchAction);
		public abstract void PatchControlBeforeAdd(Control control);
		public abstract void PatchControlAfterRemove(Control control);
		public abstract void PatchDocumentBeforeFloat(BaseDocument document);
	}
	internal static class Redraw {
		internal static void Lock(Control control) {
			if(control.IsHandleCreated)
				Lock(control.Handle);
		}
		internal static void UnLock(Control control) {
			if(control.IsHandleCreated)
				UnLock(control.Handle);
		}
		internal static void Lock(IntPtr hWnd) {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(
				hWnd, 0x000B, 0, IntPtr.Zero); 
		}
		internal static void UnLock(IntPtr hWnd) {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(
				hWnd, 0x000B, 1, IntPtr.Zero); 
		}
	}
}
