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
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	public interface IDocumentsHostWindow : IDisposable {
		DocumentManager DocumentManager { get; }
		bool DestroyOnRemovingChildren { get; }
		void Show();
		void Hide();
		void Close();
		event CancelEventHandler Closing;
		event EventHandler Closed;
	}
	abstract class FloatDocumentsHostWindow : BaseFloatDocumentForm, IDocumentsHostWindow {
		DocumentManager documentManager;
		BaseView view;
		public FloatDocumentsHostWindow(bool useMdi, ViewType type) {
			InitializeComponent(null, null, DocumentActivationScope.Default, useMdi, type);
			view.DocumentActivated += View_DocumentActivated;
		}
		public FloatDocumentsHostWindow(IDXMenuManager menuManager, BarAndDockingController controller, DocumentActivationScope scope, bool useMdi, ViewType type) {
			InitializeComponent(menuManager, controller, scope, useMdi, type);
			view.DocumentActivated += View_DocumentActivated;
		}
		protected void AssignViewProperties(BaseView parentView) {
			using(BatchUpdate.Enter(view, true))
				view.AssignProperties(parentView);
		}
		IContainer components;
		void InitializeComponent(IDXMenuManager menuManager, BarAndDockingController controller, DocumentActivationScope scope, bool useMdi, ViewType type) {
			this.components = new Container();
			this.documentManager = new DocumentManager(components);
			this.view = documentManager.CreateView(type);
			((ISupportInitialize)this.documentManager).BeginInit();
			((ISupportInitialize)this.view).BeginInit();
			this.SuspendLayout();
			this.documentManager.DocumentActivationScope = scope;
			this.documentManager.MenuManager = menuManager;
			this.documentManager.BarAndDockingController = controller;
			this.documentManager.ContainerControl = useMdi ? null : this;
			this.documentManager.MdiParent = useMdi ? this : null;
			this.documentManager.View = view;
			this.documentManager.ViewCollection.AddRange(new BaseView[] { this.view });
			this.view.FloatingDocumentContainer = FloatingDocumentContainer.DocumentsHost;
			this.Icon = null;
			this.Text = null;
			this.StartPosition = FormStartPosition.Manual;
			((ISupportInitialize)this.view).EndInit();
			((ISupportInitialize)this.documentManager).EndInit();
			this.ResumeLayout(false);
		}
		protected override void OnDispose() {
			view.DocumentActivated -= View_DocumentActivated;
			Ref.Dispose(ref components);
			base.OnDispose();
		}
		void view_DocumentRemoved(object sender, DocumentEventArgs e) {
			if(view.Documents.Count == 0 && e.Document.IsControlDisposeInProgress)
				Close();
		}
		void View_DocumentClosed(object sender, DocumentEventArgs e) {
			if(view.Documents.Count == 0)
				Close();
		}
		void View_DocumentActivated(object sender, DocumentEventArgs e) {
			InvalidateNC();
		}
		DocumentManager IDocumentsHostWindow.DocumentManager {
			get { return documentManager; }
		}
		protected internal override DocumentManager Manager {
			get { return documentManager; }
		}
		public bool DestroyOnRemovingChildren {
			get { return true; }
		}
		protected override string Caption {
			get {
				BaseDocument document = GetActiveDocument(view);
				return document != null ? document.Caption : null;
			}
		}
		protected override Image Image {
			get {
				BaseDocument document = GetActiveDocument(view);
				return document != null ? document.Image : null;
			}
		}
		static BaseDocument GetActiveDocument(BaseView view) {
			return view.ActiveDocument ??
				(view.Documents.Count > 0 ? view.Documents[0] : null);
		}
		protected override void OnShowContextMenu(Point pt) {
		}
	}
	class FloatMdiDocumentsHostWindow : FloatDocumentsHostWindow {
		public FloatMdiDocumentsHostWindow(ViewType type)
			: base(true, type) {
		}
		public FloatMdiDocumentsHostWindow(ViewType type, DocumentManager parentManager)
			: base(parentManager.MenuManager, parentManager.BarAndDockingController, parentManager.DocumentActivationScope, true, type) {
			AssignViewProperties(parentManager.View);
		}
	}
	class FloatContainerControlDocumentsHostWindow : FloatDocumentsHostWindow {
		public FloatContainerControlDocumentsHostWindow(ViewType type)
			: base(false, type) {
		}
		public FloatContainerControlDocumentsHostWindow(ViewType type, DocumentManager parentManager)
			: base(parentManager.MenuManager, parentManager.BarAndDockingController, parentManager.DocumentActivationScope, false, type) {
			AssignViewProperties(parentManager.View);
		}
	}
}
