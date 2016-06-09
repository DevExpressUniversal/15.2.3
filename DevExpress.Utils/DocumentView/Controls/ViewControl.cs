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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.DocumentView.Controls {
	[ToolboxItem(false)]
	public class ViewControl : System.Windows.Forms.Control, IMouseWheelSupport, IMouseWheelScrollClient {
		#region fields & properties
		private DocumentViewerBase pc;
		private Form form;
		bool enablePaintBackground;
		GestureHelper gestureHelper;
		private IPage SelectedPage { get { return pc.ViewManager.SelectedPage; }
		}
		protected bool IsMarginResizing {
			get {
				IPage page = SelectedPage;
				return (page == null) ? false : pc.Margins.IsMarginResizing;
			}
		}
		protected IPopupForm PopupForm { get { return pc.PopupForm; } 
		}
		protected bool IsSplitCursor { get { return (Cursor.Equals(Cursors.HSplit) || Cursor.Equals(Cursors.VSplit));}
		}
		public bool EnablePaintBackground {
			get { return enablePaintBackground; }
			set { enablePaintBackground = value; }
		}
		public override Cursor Cursor {
			get {
				return base.Cursor;
			}
			set {
				base.Cursor = value;
				if(CursorSet != null) CursorSet(this, EventArgs.Empty);
			}
		}
		#endregion
		internal event EventHandler CursorSet;
		public ViewControl(DocumentViewerBase pc, IGestureClient gestureClient) {
			InitializeComponent();
			this.pc = pc;
			this.gestureHelper = new GestureHelper(gestureClient);
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				try {
					if(form != null) {
						form.Deactivate -= new EventHandler(OwnerDeactivate);
						form.Load -= new EventHandler(OwnerLoad);
						form = null;
					}
				} catch {
				}
			}
			base.Dispose(disposing);
		}
		protected override void WndProc(ref Message m) {
			if(!gestureHelper.WndProc(ref m))
				base.WndProc(ref m);
			DevExpress.Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected override bool IsInputKey(Keys keyData) {
			bool result = base.IsInputKey(keyData);
			if(result) return true;
			Keys keys = keyData & Keys.KeyCode;
			if(keys == Keys.Tab || keys == Keys.Enter || (keys == Keys.Escape && !IsMarginResizing))
				return false;
			return true;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			try {
				if(form == null) {
					form = FindForm();
					if(form != null)
						form.Load += new EventHandler(OwnerLoad);
				}
				SetFocusInternal();
			} catch {
			}
		}
		void OwnerLoad(object sender, EventArgs e) {
			if(form != null) {
				form.Deactivate += new EventHandler(OwnerDeactivate);
				form.Load -= new EventHandler(OwnerLoad);
				form.Activate();
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.Name = "DocumentViewer";
		}
		#endregion
		protected override void OnMouseDown(MouseEventArgs e) {
			if(pc.DocumentIsEmpty == false) {
				if(IsSplitCursor) {
					Point pt = PointToClient(MousePosition);
					StartMarginResize(SelectedPage, pt);
					return;
				}
			}
			SetFocusInternal();
			base.OnMouseDown(e);
		}
		protected virtual internal void SetFocusInternal() {
			if(GetStyle(ControlStyles.Selectable) && !Focused)
				Focus();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(SelectedPage != null && pc.Margins.IsMarginResizing) {
				PointF pt = PointToClient(MousePosition);
				pc.Margins.DrawMovingSide(pt);
				ShowMarginText(pc.Margins.ActiveMargin);
				return;
			}
			base.OnMouseMove(e);
		}
		private void ShowMarginText(PageMargin pageMargin) {
			if(pageMargin != null)
				PopupForm.ShowText(pageMargin.GetText());
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			EndMarginResize(true);
			base.OnMouseUp(e);
		}
		MouseWheelScrollHelper mouseWheelHelper; 
		protected sealed override void OnMouseWheel(MouseEventArgs e) {
			if(DevExpress.XtraEditors.XtraForm.ProcessSmartMouseWheel(this, e))
				return;
			OnMouseWheelCore(e);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		void OnMouseWheelCore(MouseEventArgs e) {
			if(mouseWheelHelper == null) mouseWheelHelper = new MouseWheelScrollHelper(this);
			mouseWheelHelper.OnMouseWheel(e);
		}
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			pc.ViewOnMouseWheelCore(e);
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return false; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return false; } }
		private void StartMarginResize(IPage page, PointF pt) {
			if(page == null) return;
			Capture = true;
			pc.Margins.StartMarginResize(pt);
			if(PopupForm.IsPopupOwner(this) || pc.Margins.ActiveMargin == null)
				return;
			PopupForm.ShowText(pc.Margins.ActiveMargin.GetText(), Cursor.Position, pc.Margins.ActiveSide, this);
		}
		protected virtual void OwnerDeactivate(object sender, EventArgs e) {
			EndMarginResize(true);
		}
		protected override void OnClick(System.EventArgs e) {
			if( IsMarginResizing ) return;
			base.OnClick(e);
		}
		internal void EndMarginResize(bool applyChanges) {
			Capture = false;
			if(PopupForm != null) PopupForm.HidePopup();
			if(IsMarginResizing) {
				pc.Margins.EndMarginResize(applyChanges);
				Invalidate();
			}
		}
		public void InvalidateRect(RectangleF r, bool update) {
			if(!r.IsEmpty) {
				using(Region region = new Region(r)) {
					Invalidate(region);
					if(update) Update();
				}
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(EnablePaintBackground)
				base.OnPaintBackground(e);
		}
#if DEBUGTEST
		public void Test_PerformMouseMove(MouseEventArgs e) {
			OnMouseMove(e);
		}
		public void Test_PerformMouseDown(MouseEventArgs e) {
			OnMouseDown(e);
		}
		public void Test_PerformMouseUp(MouseEventArgs e) {
			OnMouseUp(e);
		}
		public void Test_PerformClick(EventArgs e) {
			OnClick(e);
		}
#endif
	}
}
