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
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	public class FloatDocumentForm : BaseFloatDocumentForm {
		DocumentManager managerCore;
		BaseDocument documentCore;
		public FloatDocumentForm(BaseDocument document) {
			AssertionException.IsNotNull(document);
			documentCore = document;
			Document.Disposed += OnDocumentDisposed;
			Text = Caption;
		}
		protected override string Caption {
			get { return Document.Caption; }
		}
		protected override Image Image {
			get { return Document.GetActualImage() ?? DevExpress.Utils.ImageCollection.GetImageListImage(Manager.Images, Document.ImageIndex); }
		}
		protected override void OnDispose() {
			if(Document != null) {
				Document.Disposed -= OnDocumentDisposed;
				if(Document.IsDeferredControlLoad)
					documentCore = null;
			}
			Ref.Dispose(ref documentCore);
			managerCore = null;
			base.OnDispose();
		}
		FormWindowState prevState;
		protected override void OnWindowPosChanged(IntPtr param) {
			var state = WindowState;
			if(prevState != state) {
				OnStyleChanged(EventArgs.Empty);
				prevState = state;
			}
		}
		protected override bool OnSCMaximize() {
			if(!IsDisposed && Manager != null && Document != null) {
				if(Owner is IDocumentsHostWindow) 
					return false;
			}
			return base.OnSCMaximize();
		}
		protected override Rectangle CalculateNC(Rectangle rect) {
			if(IsDisposed || Manager == null) return rect;
			if(Document == null || Document.IsDisposing) return rect;
			return base.CalculateNC(rect);
		}
		void OnDocumentDisposed(object sender, EventArgs e) {
			BaseDocument document = sender as BaseDocument;
			if(!document.IsFormDisposingLocked)
				Dispose();
		}
		protected internal override DocumentManager Manager {
			get { return managerCore; }
		}
		internal void SetManager(DocumentManager manager) {
			managerCore = manager;
			if(manager != null) {
				if(Parent == null) {
					Form managerForm = manager.GetContainer().FindForm();
					if(Owner != null && Owner != managerForm) {
						UnsubscribeDisposingFloatForm();
						Owner.RemoveOwnedForm(this);
					}
					Owner = managerForm;
					SubscribeDisposingFloatForm();
				}
			}
		}
		void SubscribeDisposingFloatForm() {
			if(Owner is Docking.FloatForm)
				(Owner as Docking.FloatForm).DisposingFloatForm += DisposingDockPanelFloatForm;
		}
		void UnsubscribeDisposingFloatForm() {
			if(Owner is Docking.FloatForm)
				(Owner as Docking.FloatForm).DisposingFloatForm -= DisposingDockPanelFloatForm;
		}
		void DisposingDockPanelFloatForm(object sender, EventArgs e) {
			UnsubscribeDisposingFloatForm();
			Owner = null;
		}
		internal void SetDocument(BaseDocument document) {
			if(Document == document) return;
			if(Document != null)
				Document.Disposed -= OnDocumentDisposed;
			Document.Control = null;
			documentCore = document;
			if(Document != null)
				Document.Disposed += OnDocumentDisposed;
			InvalidateNC();
		}
		public BaseDocument Document {
			get { return documentCore; }
		}
		protected override BaseDocument GetDocument() {
			return Document;
		}
		protected override bool Borderless {
			get { return Manager == null || (!Document.IsFloating && Document.Borderless); }
		}
		protected override bool HasCloseButton {
			get { return !Borderless && Document.HasCloseButton(); }
		}
		protected override bool HasMaximizeButton {
			get { return !Borderless && Document.HasMaximizeButton(); }
		}
		protected override bool CanUseGlyphSkinning {
			get { return Document.Properties.CanUseGlyphSkinning; }
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if((m.Msg == MSG.WM_SYSCOMMAND) && WinAPIHelper.GetInt(m.WParam) == 0xf012) {
				if((BarNativeMethods.GetKeyState(Keys.Escape) & 0x80) != 0)
					CancelDragging();
			}
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		void CancelDragging() {
			if(Manager != null)
				Manager.CancelDragOperation();
		}
		protected override void OnShowContextMenu(Point pt) {
			Manager.View.Controller.ShowContextMenu(Document, pt);
		}
		protected override bool ShouldCallWndProc() {
			return IsNativeMdiDocument();
		}
		protected bool IsNativeMdiDocument() {
			return Document is Views.NativeMdi.Document && Manager != null && Manager.IsMdiStrategyInUse;
		}		
	}
	public interface IFloatDocumentFormInfoOwner {
		Color BackColor { get; }
		BarAndDockingController GetController();
		void SetCapture();
		void InvalidateNC();
		void ButtonClick(FormCaptionButtonAction formCaptionButtonAction);
		void ShowContextMenu(Point pt);
		ButtonsPanel ButtonsPanel { get; }
		BaseDocument Document { get; }
	}
	public interface IFloatDocumentFormDragMoveContext : IDisposable {
		bool StateChangedWhenMovingMaximized { get; set; }
		void Queue(Action endDragging);
	}
}
