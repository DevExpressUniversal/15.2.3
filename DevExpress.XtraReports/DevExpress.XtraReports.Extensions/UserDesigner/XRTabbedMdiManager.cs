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

using DevExpress.XtraTabbedMdi;
using System.ComponentModel;
using DevExpress.XtraBars.Docking2010.Views;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraReports.UserDesigner.Native;
namespace DevExpress.XtraReports.UserDesigner {
	[
#if !DEBUG
#endif // DEBUG
ToolboxItem(false),
	DesignTimeVisible(false),
	]
	public class XRTabbedMdiManager :  XtraBars.Docking2010.DocumentManager {
		BaseView previousView;
		bool floatDocumentDeactivated = false;
		static object AnyDocumentActivatedEvent = new object();
		internal IXRDesignPanelContainer ActiveContainer { get; private set; }
		internal int FormsCount { get { return View != null ? View.Documents.Count + View.FloatDocuments.Count : 0; } }
		public event EventHandler<DocumentEventArgs> AnyDocumentActivated {
			add { Events.AddHandler(AnyDocumentActivatedEvent, value); }
			remove { Events.RemoveHandler(AnyDocumentActivatedEvent, value); }
		}
		public XRTabbedMdiManager(System.ComponentModel.IContainer container)
			: base(container) {
		}
		public XRTabbedMdiManager()
			: base() {
		}
		internal void Activate(IXRDesignPanelContainer container) {
			Activate(GetDocument((Control)container));
		}
		internal IEnumerable<Control> GetContainers() {
			foreach(BaseDocument document in View.Documents)
				yield return GetDesignPanelContainer(document);
			foreach(BaseDocument document in View.FloatDocuments)
				yield return GetDesignPanelContainer(document);
		}
		Control GetDesignPanelContainer(BaseDocument document) {
			return document.Control;
		}
		internal void SetView(ViewType viewType) {
			BaseView view;
			if(!ViewCollection.TryGetView(viewType, out view)) {
				view = CreateView(viewType);
				ViewCollection.Add(view);
			}
			View = view;
		}
		protected override void Initialize(System.Windows.Forms.ContainerControl container) {
			base.Initialize(container);
			if(MdiParent != null && MdiParent == container)
				MdiParent.Activated += new System.EventHandler(MdiParent_Activated);
		}
		protected override void Destroy(System.Windows.Forms.ContainerControl container) {
			base.Destroy(container);
			if(MdiParent != null && MdiParent == container)
				MdiParent.Activated -= new System.EventHandler(MdiParent_Activated);
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			if(previousView != null) {
				previousView.DocumentActivated -= new DocumentEventHandler(view_DocumentActivated);
				previousView.DocumentDeactivated -= new DocumentEventHandler(view_DocumentDeactivated);
				previousView.DocumentClosed -= new DocumentEventHandler(view_DocumentClosed);
			}
			if(View != null) {
				View.DocumentActivated += new DocumentEventHandler(view_DocumentActivated);
				View.DocumentDeactivated += new DocumentEventHandler(view_DocumentDeactivated);
				View.DocumentClosed += new DocumentEventHandler(view_DocumentClosed);
			}
			previousView = View;
		}
		void view_DocumentActivated(object sender, DocumentEventArgs e) {
			floatDocumentDeactivated = false;
			OnAnyDocumentActivated(e);
		}
		void view_DocumentDeactivated(object sender, DocumentEventArgs e) {
			floatDocumentDeactivated = e.Document == View.ActiveFloatDocument;
		}
		void MdiParent_Activated(object sender, System.EventArgs e) {
			if(floatDocumentDeactivated && View.ActiveDocument != null)
				OnAnyDocumentActivated(new DocumentEventArgs(View.ActiveDocument));
			floatDocumentDeactivated = false;
		}
		void OnAnyDocumentActivated(DocumentEventArgs e) {
			ActiveContainer = e.Document.Control as IXRDesignPanelContainer;
			EventHandler<DocumentEventArgs> handler = (EventHandler<DocumentEventArgs>)this.Events[AnyDocumentActivatedEvent];
			if(handler != null) handler(this, e);
		}
		void view_DocumentClosed(object sender, DocumentEventArgs e) {
			if(View.Documents.Count == 0)
				ActiveContainer = null;
		}
	}
}
namespace DevExpress.XtraBars.Docking2010.Views {
	static class BaseViewCollectionExtentions {
		public static bool TryGetView(this BaseViewCollection collection, ViewType viewType, out BaseView view) {
			view = collection.FindFirst(item => { return item.Type == viewType; });
			return view != null;
		}
	}
}
