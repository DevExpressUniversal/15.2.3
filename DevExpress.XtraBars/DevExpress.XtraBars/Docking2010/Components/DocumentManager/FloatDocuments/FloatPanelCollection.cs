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
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.Views {
	public class FloatPanelCollection : BaseDocumentCollection {
		IDictionary<Docking.DockPanel, BaseDocument> panelLinks;
		public FloatPanelCollection(BaseView view)
			: base(view) {
			panelLinks = new Dictionary<Docking.DockPanel, BaseDocument>();
		}
		protected override void UnregisterElementOnDisposeCollection(BaseDocument element) {
			base.UnregisterElementOnDisposeCollection(element);
			Docking.FloatForm floatForm = element.Form as Docking.FloatForm
				 ?? FindControl(element) as Docking.FloatForm;
			UnSubscribeFloatFormEvents(floatForm);
			UnregisterDockPanel(element);
		}
		protected override void OnBeforeElementAdded(BaseDocument element) {
			element.SetIsFloating(true);
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(BaseDocument element) {
			base.OnElementAdded(element);
			element.SetIsFloating(true);
			Docking.FloatForm floatForm = element.Form as Docking.FloatForm;
			RegisterDockPanel(element, floatForm.FloatLayout.Panel);
			SubscribeFloatFormEvents(floatForm);
			if(lockFloatPanelRegistration == 0)
				Owner.Manager.RegisterFloatPanel(element);
		}
		protected override void OnElementRemoved(BaseDocument element) {
			Docking.FloatForm floatForm = element.Form as Docking.FloatForm
				 ?? FindControl(element) as Docking.FloatForm;
			if(lockFloatPanelRegistration == 0)
				Owner.Manager.UnregisterFloatPanel(floatForm);
			UnSubscribeFloatFormEvents(floatForm);
			UnregisterDockPanel(element);
			base.OnElementRemoved(element);
			element.SetIsFloating(false);
		}
		protected override void RemoveFromContainer(BaseDocument element) { }
		protected override void AddToContainer(BaseDocument element) { }
		void SubscribeDockPanelEvents(Docking.DockPanel dockPanel) {
			if(dockPanel != null) {
				dockPanel.ClosedPanel += OnDockPanelClosed;
			}
		}
		void UnSubscribeDockPanelEvents(Docking.DockPanel dockPanel) {
			if(dockPanel != null) {
				dockPanel.ClosedPanel -= OnDockPanelClosed;
			}
		}
		void SubscribeFloatFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.FormClosed += OnFloatPanelClosed;
				floatForm.Disposed += OnFloatPanelDisposed;
			}
		}
		void UnSubscribeFloatFormEvents(Form floatForm) {
			if(floatForm != null) {
				floatForm.FormClosed -= OnFloatPanelClosed;
				floatForm.Disposed -= OnFloatPanelDisposed;
			}
		}
		void OnDockPanelClosed(object sender, Docking.DockPanelEventArgs e) {
			DisposeDocument(e.Panel);
		}
		void OnFloatPanelDisposed(object sender, EventArgs e) {
			DisposeDocument(sender as Form, false);
		}
		void OnFloatPanelClosed(object sender, FormClosedEventArgs e) {
			DisposeDocument(sender as Form, false);
		}
		protected void DisposeDocument(Docking.DockPanel dockPanel) {
			BaseDocument document;
			if(panelLinks.TryGetValue(dockPanel, out document)) {
				Remove(document);
				Ref.Dispose(ref document);
			}
		}
		protected internal void RegisterDockPanel(BaseDocument element, Docking.DockPanel panel) {
			if(panel != null && !panelLinks.ContainsKey(panel)) {
				panelLinks.Add(panel, element);
				SubscribeDockPanelEvents(panel);
			}
		}
		protected internal void UnregisterDockPanel(BaseDocument element) {
			Docking.DockPanel panel = FindDockPanel(element);
			if(panel != null) {
				UnSubscribeDockPanelEvents(panel);
				panelLinks.Remove(panel);
			}
		}
		protected Docking.DockPanel FindDockPanel(BaseDocument document) {
			foreach(KeyValuePair<Docking.DockPanel, BaseDocument> pair in panelLinks)
				if(pair.Value == document) return pair.Key;
			return null;
		}
		#region IHookController Members
		protected override bool PostFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		#endregion IHookController Members
		int lockFloatPanelRegistration = 0;
		protected internal IDisposable LockFloatPanelRegistration(Docking.DockPanel panel) {
			bool cancelLocking = false;
			if(panel.HasChildren) {
				foreach(var item in panelLinks.Keys) {
					cancelLocking |= panel == GetParentPanel(item);
				}
			}
			else
				cancelLocking = panelLinks.ContainsKey(panel);
			return new FloatPanelRegistrationLocker(this, cancelLocking);
		}
		Docking.DockPanel GetParentPanel(Docking.DockPanel panel) {
			if(panel == null) return null;
			if(panel.ParentPanel != null)
				return GetParentPanel(panel.ParentPanel);
			return panel;
		}
		protected internal IDisposable LockFloatPanelRegistration() {
			return new FloatPanelRegistrationLocker(this, false);
		}
		class FloatPanelRegistrationLocker : IDisposable {
			FloatPanelCollection Panels;
			bool cancelLocking;
			public FloatPanelRegistrationLocker(FloatPanelCollection panels, bool cancelLocking) {
				Panels = panels;
				this.cancelLocking = cancelLocking;
				if(!cancelLocking)
					Panels.lockFloatPanelRegistration++;
			}
			public void Dispose() {
				if(!cancelLocking)
					Panels.lockFloatPanelRegistration--;
				Panels = null;
			}
		}
	}
}
