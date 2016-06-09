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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	class NoDocumentsStrategy : DocumentManagementStrategy {
		IDisposable clientControlSubscriber;
		public NoDocumentsStrategy(DocumentManager manager)
			: base(manager) {
		}
		protected override void OnDispose() {
			Ref.Dispose(ref clientControlSubscriber);
		}
		public override bool IsValid {
			get { return Manager.ClientControl != null; }
		}
		public sealed override bool IsOwnerControlEmpty {
			get { return true; }
		}
		ContainerControl containerCore;
		public override ContainerControl Container {
			get {
				if(containerCore == null) {
					if(Manager.Container != null)
						containerCore = FindContainer();
				}
				return containerCore;
			}
		}
		ContainerControl FindContainer() {
			foreach(IComponent component in Manager.Container.Components) {
				Docking.DockManager dockManager = component as Docking.DockManager;
				if(dockManager != null && dockManager.Form != null)
					return CheckAndSetContainer(dockManager);
			}
			Control control = Manager.ClientControl;
			while(control != null) {
				foreach(Control childControl in control.Controls) {
					Docking.DockPanel panel = childControl as Docking.DockPanel;
					if(panel != null && panel.DockManager != null)
						return CheckAndSetContainer(panel.DockManager);
				}
				control = control.Parent;
			}
			return null;
		}
		ContainerControl CheckAndSetContainer(Docking.DockManager dockManager) {
			if(this.Manager.dockManagerCore == null)
				this.Manager.dockManagerCore = dockManager;
			if(dockManager.documentManagerCore == null)
				dockManager.documentManagerCore = Manager;
			return dockManager.Form;
		}
		public override Control Initialize(ContainerControl container) {
			if(Manager.ClientControl == null)
				throw new NonDocumentModeInitializationException();
			clientControlSubscriber = CreateClientControlSubscriber(Manager.ClientControl);
			return Manager.ClientControl;
		}
		public override void Destroy(ContainerControl container, bool handleRecreating) {
			if(!handleRecreating)
				Ref.Dispose(ref clientControlSubscriber);
			else IsOwnerControlInvalid = true;
		}
		public override void Ensure(Control client) {
			Ref.Dispose(ref clientControlSubscriber);
			clientControlSubscriber = CreateClientControlSubscriber(client);
		}
		protected virtual IDisposable CreateClientControlSubscriber(Control clientControl) {
			return new ClientControlSubscriber(clientControl, Manager);
		}
		public override bool CheckFocus(int Msg, BaseView view, Control control, IntPtr wParam) {
			throw new NotSupportedException();
		}
		public override Control GetOwnerControl() {
			return Manager.ClientControl;
		}
		public override void Activate(Control child) {
			throw new NotSupportedException();
		}
		public override void PostFocus(Control child) {
			throw new NotSupportedException();
		}
		public override Control GetActiveChild() {
			throw new NotSupportedException();
		}
		public override bool CanUpdateLayout() {
			return Manager.ClientControl != null;
		}
		public override void UpdateLayout() {
		}
		public override Control GetChild(BaseDocument document) {
			throw new NotSupportedException();
		}
		public override void AddDocumentToHost(BaseDocument document) {
			throw new NotSupportedException();
		}
		public override void RemoveDocumentFromHost(BaseDocument document) {
			throw new NotSupportedException();
		}
		public override void DockFloatForm(BaseDocument document, Action<Form> patchAction) {
			throw new NotSupportedException();
		}
		public override void DockFloatForm(BaseDocument document, Docking.DockPanel panel) {
			throw new NotSupportedException();
		}
		public override void PatchControlBeforeAdd(Control control) {
			throw new NotSupportedException();
		}
		public override void PatchControlAfterRemove(Control control) {
			throw new NotSupportedException();
		}
		public override void PatchDocumentBeforeFloat(BaseDocument document) {
			throw new NotSupportedException();
		}
	}
	interface IClientControlListener {
		void VisibleChanged(bool visible);
		void HandleCreated();
		void HandleDestroyed();
		void Resize(Rectangle bounds);
		void OnDisposed();
	}
	class ClientControlSubscriber : IDisposable {
		public IClientControlListener Listener;
		protected Control ClientControl;
		public ClientControlSubscriber(Control clientControl, IClientControlListener listener) {
			Listener = listener;
			ClientControl = clientControl;
			if(ClientControl != null)
				Subscribe();
		}
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				if(ClientControl != null)
					UnSubscribe();
				ClientControl = null;
				Listener = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void Subscribe() {
			ClientControl.VisibleChanged += ClientControl_VisibleChanged;
			ClientControl.Resize += ClientControl_Resize;
			ClientControl.HandleCreated += ClientControl_HandleCreated;
			ClientControl.HandleDestroyed += ClientControl_HandleDestroyed;
			ClientControl.Disposed += ClientControl_Disposed;
		}
		protected virtual void UnSubscribe() {
			ClientControl.VisibleChanged -= ClientControl_VisibleChanged;
			ClientControl.Resize -= ClientControl_Resize;
			ClientControl.HandleCreated -= ClientControl_HandleCreated;
			ClientControl.HandleDestroyed -= ClientControl_HandleDestroyed;
			ClientControl.Disposed -= ClientControl_Disposed;
		}
		void ClientControl_Disposed(object sender, EventArgs e) {
			Listener.OnDisposed();
		}
		void ClientControl_HandleDestroyed(object sender, EventArgs e) {
			Listener.HandleDestroyed();
		}
		void ClientControl_HandleCreated(object sender, EventArgs e) {
			Listener.HandleCreated();
		}
		void ClientControl_Resize(object sender, EventArgs e) {
			if(!isDisposing) 
				Listener.Resize(new Rectangle(Point.Empty, ClientControl.Size));
		}
		bool visible;
		void ClientControl_VisibleChanged(object sender, EventArgs e) {
			bool clientControlVisible = ClientControl.Visible;
			if(visible != clientControlVisible) {
				Listener.VisibleChanged(clientControlVisible);
				if(clientControlVisible)
					Listener.Resize(new Rectangle(Point.Empty, ClientControl.Size));
			}
			visible = clientControlVisible;
		}
	}
}
