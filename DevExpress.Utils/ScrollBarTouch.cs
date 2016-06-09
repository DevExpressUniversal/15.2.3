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
using DevExpress.Utils.Internal;
namespace DevExpress.XtraEditors.Internal {
	public class FloatingScrollbarBase : DXLayeredWindow, IObserver<ILayeredWindowNotification> {
		Timer timer;
		Timer autoShowHideTimer;
		const int AutoHideInterval = 1500;
		public FloatingScrollbarBase() {
			TransparencyKey = Color.White;
			Alpha = 255;
		}
		public override bool IsTransparent {
			get { return false; }
		}
		protected internal virtual IScrollBar Owner {
			get { return null; }
		}
		public void CleanUp() {
			if(IsCreated)
				DestroyHandle();
			DestroyTimer();
			DestroyAutoShowHideTimer();
			DestroyBuffer();
		}
		IDisposable observableSubscription;
		public override void CreateHandle(CreateParams cp) {
			base.CreateHandle(cp);
			var notificationSource = LayeredWindowNotificationSource.FromHandle(cp.Parent);
			if(notificationSource != null)
				observableSubscription = notificationSource.Subscribe(this);
		}
		public override void DestroyHandle() {
			DevExpress.Utils.Ref.Dispose(ref observableSubscription);
			base.DestroyHandle();
		}
		void IObserver<ILayeredWindowNotification>.OnCompleted() {
			observableSubscription = null;
		}
		void IObserver<ILayeredWindowNotification>.OnNext(ILayeredWindowNotification notification) {
			if(notification.Type == LayeredWindowNotificationType.Hidden)
				Hide();
			if(notification.Type == LayeredWindowNotificationType.Reparented)
				CleanUp();
		}
		void IObserver<ILayeredWindowNotification>.OnError(Exception error) {
			DevExpress.Utils.Ref.Dispose(ref observableSubscription);
		}
		protected override IntPtr InsertAfterWindow {
			get {
				if(Owner != null && Owner.Parent != null && Owner.Parent.IsHandleCreated) {
					var form = Owner.Parent.FindForm();
					if(form != null && form.IsHandleCreated)
						return Owner.Parent.Handle;
				}
				return base.InsertAfterWindow;
			}
		}
		protected override void NCHitTest(ref Message m) {
			m.Result = new IntPtr(NativeMethods.HT.HTCLIENT);
		}
		public virtual bool AllowAutoHide {
			get { return true; }
		}
		public virtual void OnScrollChanged() {
			Draw();
			if(AllowAutoHide) {
				DestroyAutoShowHideTimer();
				autoShowHideTimer = new Timer();
				autoShowHideTimer.Tick += OnAutoHideTimer;
				autoShowHideTimer.Interval = AutoHideInterval;
				autoShowHideTimer.Start();
			}
		}
		public override void Invalidate() {
			if(Visible) Draw();
		}
		Bitmap buffer;
		protected virtual void Draw() {
			if(Size.Width < 1 || Size.Height < 1) return;
			using(Bitmap bitmap = CheckBuffer(Size.Width, Size.Height)) {
				using(Graphics g = Graphics.FromImage(bitmap)) {
					DrawBackgroundCore(g);
					UpdateLayered(bitmap);
				}
			}
		}
		protected override Rectangle ValidateBounds(Rectangle bounds) {
			if(Owner == null || Owner.Parent == null || !Owner.Parent.IsHandleCreated) return bounds;
			Form f = Owner.Parent.FindForm();
			if(f == null) return bounds;
			Rectangle form = f.RectangleToScreen(f.ClientRectangle);
			Rectangle b = bounds;
			if(form.Contains(b)) return bounds;
			if(!form.IntersectsWith(b)) return Rectangle.Empty;
			if(b.X < form.X) {
				b.Width = b.Width - (form.X - b.X);
				b.X = form.X;
			}
			if(b.Y < form.Y) {
				b.Height = b.Height - (form.Y - b.Y);
				b.Y = form.Y;
			}
			if(b.Right > form.Right) b.Width = form.Right - b.X;
			if(b.Bottom > form.Bottom) b.Height = form.Bottom - b.Y;
			if(b.Width < 1 || b.Height < 1) return Rectangle.Empty;
			return b;
		}
		private Bitmap CheckBuffer(int width, int height) {
			if(width < 1 || height < 1) return null;
			if(buffer == null) return new Bitmap(width, height);
			if(buffer.Width < width || buffer.Height < height) {
				buffer.Dispose();
				buffer = new Bitmap(width, height);
			}
			return buffer;
		}
		protected override void UpdateLayeredWindow() {
			Draw();
		}
		void DrawBackgroundCore(Graphics g) {
			g.Clear(Color.Transparent);
			using(var brush = new SolidBrush(Color.FromArgb(1, Color.LightGray))) {
				Rectangle bounds = new Rectangle(Point.Empty, Size);
				g.FillRectangle(brush, bounds);
			}
			PaintEventArgs e = new PaintEventArgs(g, Rectangle.Empty);
			Owner.ProcessPaint(e);
		}
		void OnAutoHideTimer(object sender, EventArgs e) {
			DestroyAutoShowHideTimer();
			Hide();
			DestroyTimer();
		}
		public override void Hide() {
			base.Hide();
			DestroyTimer();
		}
		protected override void OnVisibleChanged() {
			base.OnVisibleChanged();
			if(Visible) {
				if(Owner != null && Owner.Parent != null) {
					Owner.Parent.LocationChanged += OnOwnerSizeChanged;
					Owner.Parent.SizeChanged += OnOwnerSizeChanged;
					if(Owner.Parent.FindForm() != null) Owner.Parent.FindForm().Move += OnOwnerSizeChanged;
				}
				if(timer == null) {
					timer = new Timer();
					timer.Tick += OnTimerTick;
					timer.Interval = 100;
					timer.Start();
				}
			}
			else {
				if(Owner != null && Owner.Parent != null) {
					Owner.Parent.LocationChanged -= OnOwnerSizeChanged;
					Owner.Parent.SizeChanged -= OnOwnerSizeChanged;
					if(Owner.Parent.FindForm() != null) Owner.Parent.FindForm().Move -= OnOwnerSizeChanged;
				}
			}
		}
		void OnOwnerSizeChanged(object sender, EventArgs e) {
			Hide();
			if(Owner is ScrollTouchBase) { 
				((ScrollTouchBase)Owner).OnParentResized();
			}
		}
		void DestroyTimer() {
			DevExpress.Utils.Ref.Dispose(ref timer);
		}
		void DestroyAutoShowHideTimer() {
			DevExpress.Utils.Ref.Dispose(ref autoShowHideTimer);
		}
		void DestroyBuffer() {
			DevExpress.Utils.Ref.Dispose(ref buffer);
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_PAINT:
					DefWndProc(ref m);
					m.Result = new IntPtr(0);
					return;
				case MSG.WM_NCPAINT:
					m.Result = new IntPtr(0);
					return;
				case MSG.WM_ERASEBKGND:
					m.Result = new IntPtr(1);
					return;
				case MSG.WM_MOUSEACTIVATE:
					m.Result = new IntPtr(3); 
					return;
				case MSG.WM_SETFOCUS:
					break;
				case 675:
					this.WmMouseLeave(ref m);
					break;
				case 512:
					this.WmMouseMove(ref m);
					return;
				case 513:
					this.WmMouseDown(ref m, MouseButtons.Left, 1);
					return;
				case 514:
					this.WmMouseUp(ref m, MouseButtons.Left, 1);
					return;
				case 515:
					this.WmMouseDown(ref m, MouseButtons.Left, 2);
					return;
				case 516:
					this.WmMouseDown(ref m, MouseButtons.Right, 1);
					return;
				case 517:
					this.WmMouseUp(ref m, MouseButtons.Right, 1);
					return;
				case 518:
					this.WmMouseDown(ref m, MouseButtons.Right, 2);
					return;
				case 519:
					this.WmMouseDown(ref m, MouseButtons.Middle, 1);
					return;
				case 520:
					this.WmMouseUp(ref m, MouseButtons.Middle, 1);
					return;
				case 521:
					this.WmMouseDown(ref m, MouseButtons.Middle, 2);
					return;
				case 522:
					this.WmMouseWheel(ref m);
					return;
				case 533:
					this.WmCaptureChanged(ref m);
					return;
			}
			base.WndProc(ref m);
		}
		void WmMouseLeave(ref Message m) {
			this.trackMouse = false;
			OnMouseLeave(EventArgs.Empty);
		}
		void WmCaptureChanged(ref Message m) {
			if(!Capture) {
			}
		}
		void WmMouseWheel(ref Message m) {
		}
		void WmMouseUp(ref Message m, MouseButtons mouseButtons, int clicks) {
			OnMouseUp(new MouseEventArgs(mouseButtons, clicks, NativeMethods.FromMouseLParam(ref m).X, NativeMethods.FromMouseLParam(ref m).Y, 0));
			Capture = false;
		}
		void WmMouseDown(ref Message m, MouseButtons mouseButtons, int clicks) {
			Capture = true;
			OnMouseDown(new MouseEventArgs(mouseButtons, clicks, NativeMethods.FromMouseLParam(ref m).X, NativeMethods.FromMouseLParam(ref m).Y, 0));
		}
		bool trackMouse = false;
		void TrackMouseLeaveMessage() {
			if(trackMouse) return;
			NativeMethods.TRACKMOUSEEVENTStruct track = new NativeMethods.TRACKMOUSEEVENTStruct();
			track.dwFlags = 3;
			track.hwndTrack = Handle;
			if(!NativeMethods.TrackMouseEvent(track)) {
				return;
			}
			trackMouse = true;
			OnMouseEnter(EventArgs.Empty);
		}
		void WmMouseMove(ref Message m) {
			TrackMouseLeaveMessage();
			Point p = NativeMethods.FromMouseLParam(ref m);
			OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, p.X, p.Y, 0));
		}
		void OnMouseDown(MouseEventArgs e) {
			if(Owner != null) Owner.ProcessMouseDown(e);
		}
		void OnMouseMove(MouseEventArgs e) {
			Cursor.Current = Cursors.Arrow;
			if(Owner != null) Owner.ProcessMouseMove(e);
		}
		void OnMouseUp(MouseEventArgs e) {
			if(Owner != null) Owner.ProcessMouseUp(e);
		}
		protected virtual void OnMouseEnter(EventArgs e) {
			Owner.ProcessMouseEnter();
			Draw();
		}
		protected virtual void OnMouseLeave(EventArgs e) {
			Owner.ProcessMouseLeave(); 
			Draw();
		}
		internal bool Capture {
			get {
				return Handle != IntPtr.Zero && DevExpress.Utils.Drawing.Helpers.NativeMethods.GetCapture() == this.Handle;
			}
			set {
				if(Capture == value) return;
				if(value) {
					NativeMethods.SetCapture(Handle);
					return;
				}
				NativeMethods.ReleaseCapture();
			}
		}
		bool CheckVisible(Control control) {
			if(!DevExpress.Utils.Win.FormExtensions.FormIsCreated(control)) return false;
			Control checkControl = control;
			while(checkControl != null) {
				if(!checkControl.Visible) return false;
				checkControl = checkControl.Parent;
			}
			return true;
		}
		void OnTimerTick(object sender, EventArgs e) {
			if(Owner == null || Owner.Parent == null || !Owner.Parent.IsHandleCreated) {
				DestroyTimer();
				return;
			}
			if(!CheckVisible(Owner.Parent)) {
				Hide();
				DestroyTimer();
			}
		}
		protected override void DrawForeground(Graphics g) {
		}
		protected override void DrawBackground(Graphics g) {
		}
		protected virtual bool IsHorizontal { get { return false; } }
		protected override bool UseDoubleBuffer {
			get {
				return true;
			}
		}
		void UpdateLayered(Bitmap bmp) {
			if(bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) return;
			Rectangle bounds = ValidateBounds(Bounds);
			if(bounds.IsEmpty) return;
			IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
			IntPtr memDc = NativeMethods.CreateCompatibleDC(screenDc);
			IntPtr hBmp = bmp.GetHbitmap(Color.FromArgb(0));
			IntPtr tmp = NativeMethods.SelectObject(memDc, hBmp);
			var blendFunc = new NativeMethods.BLENDFUNCTION
			{
				BlendOp = 0,
				BlendFlags = 0,
				SourceConstantAlpha = 0xff,
				AlphaFormat = 1
			};
			var loc = new NativeMethods.POINT(bounds.Left, bounds.Top);   
			var size = new NativeMethods.SIZE(bounds.Width, bounds.Height);	
			var sourceLoc = new NativeMethods.POINT(0, 0);  
			bool flag = NativeMethods.UpdateLayeredWindow(
				this.Handle,
				screenDc,
				ref loc,   
				ref size,	
				memDc,
				ref sourceLoc,  
				0,
				ref blendFunc,
				2);
			NativeMethods.SelectObject(memDc, tmp);
			NativeMethods.DeleteObject(hBmp);
			NativeMethods.DeleteDC(memDc);
			NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
		}
	}
	public class FloatingScrollbar : FloatingScrollbarBase {
		IScrollBar owner;
		public FloatingScrollbar(IScrollBar owner) {
			this.owner = owner;
		}
		protected internal override IScrollBar Owner { get { return owner; } }
	}
}
