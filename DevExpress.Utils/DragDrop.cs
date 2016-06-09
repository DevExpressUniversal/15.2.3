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
using System.ComponentModel.Design;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.DragDrop {
	public enum DragState {Move, None, Remove, Copy};
	[ToolboxItem(false)]
	public class BaseDragHelperForm : Form, ISupportXtraAnimation {
		int framesCount;
		int fadeSpeed;
		public BaseDragHelperForm(int fadeSpeed, int framesCount) {
			this.fadeSpeed = fadeSpeed;
			this.framesCount = framesCount;
			Init();
		}
		protected virtual void SetVisibleInactive(Control control, bool visible) {
			const int HWND_TOPMOST = -1, SWP_NOSIZE = 1, SWP_NOMOVE = 2, SWP_NOACTIVATE = 16, SWP_SHOWWINDOW = 64;
			if(visible) {
				NativeMethods.SetWindowPos(control.Handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0,
					SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
			}
		}
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		double startOpacity = 0;
		bool startOpacityInitialized = false;
		protected override void SetVisibleCore(bool value) {
			if(!Visible && value) {
				if(CanProcessOpacity) {
					if(!startOpacityInitialized) { startOpacity = Opacity; startOpacityInitialized = true; }
					XtraAnimator.Current.AddObject(this, this.GetHashCode(), fadeSpeed, framesCount, new CustomAnimationInvoker(FrameStep));
					Opacity = 0;
				}
			}
			SetVisibleInactive(this, value);
			base.SetVisibleCore(value);
		}
		protected virtual void Init() {
			TransparencyKey = Color.Magenta;
			BackColor = Color.Magenta;
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SuspendLayout();
			ControlBox = false;
			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			ResumeLayout(false);
		}
		protected bool CanProcessOpacity {
			get { return framesCount > 0 && Width > 0 && Height > 0; }
		}
		public void FrameStep(BaseAnimationInfo animInfo) {
			if(CanProcessOpacity) {
				Opacity = startOpacity * ((double)animInfo.CurrentFrame / framesCount);
				Invalidate();
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams tparams = base.CreateParams;
				tparams.Style = -2147483648;
				tparams.ClassStyle |= 0x800;
				if(Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major > 4) {
					tparams.ExStyle += 0x8000000 + 0x00000020;
					return tparams;
				}
				return tparams;
			}
		}
	}
	public class DragManager : IDisposable {
		DragWindow dragWindow = null;
		DragState lastDragState;
		static Cursor dragRemoveCursor;
		public static Cursor DragRemoveCursor {
			get {
				if(dragRemoveCursor == null) dragRemoveCursor = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.DragRemove.cur", typeof(DragManager).Assembly);
				return dragRemoveCursor;
			}
		}
		public event PaintEventHandler PaintDragWindow;
		public DragManager() {
			this.lastDragState = DragState.None;
		}
		public virtual void Dispose() {
			if(this.dragWindow != null) this.dragWindow.Dispose();
			this.dragWindow = null;
		}
		DragWindow DragWindow {
			get {
				if(dragWindow == null) dragWindow = new DragWindow();
				return dragWindow;
			}
		}
		public void DoDragDrop(Size dragWindowSize, Point startPoint) {
			Application.AddMessageFilter(DragWindow);
			try {
				DragWindow.DoDrag(this, dragWindowSize, startPoint);
				while(true) {
					if(!DragWindow.IsDragging) break;
					Application.DoEvents();
					System.Threading.Thread.Sleep(1);
				}
			} finally {
				Application.RemoveMessageFilter(DragWindow);
			}
		}
		public virtual void SetDragCursor(DragState e) {
			if(e == DragState.Remove) 
				Cursor.Current = DragRemoveCursor;
			else {
				if(e == DragState.None) 
					Cursor.Current = Cursors.No;
				else Cursor.Current = Cursors.Default;
			}
		}
		protected virtual DragState GetDragState(Point p) { return DragState.None; }
		protected virtual DragState LastDragState { get { return this.lastDragState; } }
		protected internal virtual void OnDragDrop(Point p) { 
		}
		protected internal virtual void OnDragMove(Point p) { 
			this.lastDragState = GetDragState(p);
			SetDragCursor(this.lastDragState);
		}
		protected internal virtual void OnDragCancel() { 
			this.lastDragState = DragState.None;
		}
		protected internal virtual void RaisePaint(PaintEventArgs e) {
			if(PaintDragWindow != null) 
				PaintDragWindow(this, e);
			else {
				e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
			}
		}
	}
	public class DragWindow : DevExpress.Utils.Win.TopFormBase, IMessageFilter {
		DragManager draggingManager = null;
		Size dragSize;
		Point hotSpot;
		public static double DefaultOpacity = 0.60;
		public DragWindow() {
			this.dragSize = Size.Empty;
			this.hotSpot = Point.Empty;
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.StartPosition = FormStartPosition.Manual;
			this.BackColor = Color.Transparent;
			this.MinimumSize = Size.Empty;
			this.Size = Size.Empty;
			this.Visible = false;
			this.TabStop = false;
			this.Opacity = DragWindow.DefaultOpacity;
			this.Size = Size.Empty;
			this.ShowInTaskbar = false;
		}
		protected override void Dispose(bool disposing) {
			DraggingManager = null;
			base.Dispose(disposing);
		}
		public void MakeTopMost() {
			UpdateZOrder();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified bounds) {
			width = DragSize.Width;
			height = DragSize.Height;
			base.SetBoundsCore(x, y, width, height, bounds);
		}
		protected void InternalMoveBitmap(Point p) {
			p.Offset(-hotSpot.X, -hotSpot.Y);
			Location = p;
		}
		protected override void OnPaintBackground(PaintEventArgs e) { }
		protected override void OnPaint(PaintEventArgs e) {
			if(DraggingManager != null) 
				DraggingManager.RaisePaint(new PaintEventArgs(e.Graphics, ClientRectangle));
			else
				e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Point p = PointToScreen(new Point(e.X, e.Y));
			MoveDrag(p);
			if(DraggingManager != null) DraggingManager.OnDragMove(p);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(DraggingManager == null || e.Button != MouseButtons.Left) return;
			DragManager manager = DraggingManager;
			Point p = PointToScreen(new Point(e.X, e.Y));
			HideDrag();
			manager.OnDragDrop(p);
		}
		public bool IsDragging { get { return DraggingManager != null; } }
		public void DoDrag(DragManager manager, Size dragSize, Point p) {
			HideDrag();
			if(dragSize.IsEmpty) return;
			this.draggingManager = manager;
			this.DragSize = dragSize;
			this.MinimumSize = this.Size = dragSize;
			this.Location = new Point(-10000, -10000);
			this.Visible = true;
			Refresh();
			InternalMoveBitmap(p);
			Capture = true;
			MakeTopMost();
		}
		protected override void OnLostCapture() {
			OnDragCancel();
		}
		protected void OnDragCancel() {
			DragManager manager = DraggingManager;
			HideDrag();
			if(manager != null) manager.OnDragCancel();
		}
		protected void MoveDrag(Point p) { InternalMoveBitmap(p); }
		protected bool HideDrag() {
			if(DraggingManager == null) return false;
			SuspendLayout();
			Size = Size.Empty;
			Visible = false;
			ResumeLayout();
			this.draggingManager = null;
			return true;
		}
		protected Point HotSpot { get { return hotSpot; } set { hotSpot = value; } }
		protected Size DragSize { 
			get { return dragSize; }
			set {
				if(value == Size.Empty) {
					HideDrag();
				} else {
					hotSpot = new Point(value.Width / 2, value.Height / 2);
				}
				dragSize = value;
			}
		}
		protected DragManager DraggingManager {
			get { return draggingManager; } 
			set {
				if(draggingManager != null) HideDrag();
				this.draggingManager = value;
			}
		}
		const int WM_KEYDOWN = 0x0100;
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if (m.Msg == WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape) {
				OnDragCancel();
				return true;
			}
			return false;
		}
	}
}
