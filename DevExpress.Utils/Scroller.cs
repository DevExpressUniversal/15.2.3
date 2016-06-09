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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Controls {
	public class OfficeScroller : IOfficeScroller {
		public enum ScrollMode { None, Horz, Vert, Full }
		public enum ScrollCursor { Full = 0, Horz = 1, Vert = 2, Left = 3, Right = 4, Up = 5, Down = 6}
		public enum ScrollDirection { None = 0, Left = 3, Right = 4, Up = 5, Down = 6}
		public class ScrollInfoArgs {
			ScrollDirection direction;
			int speed;
			public ScrollInfoArgs(ScrollDirection direction) : this(direction, 0) {	}
			public ScrollInfoArgs(ScrollDirection direction, int speed) {
				this.direction = direction;
				this.speed = speed;
			}
			public ScrollDirection Direction { get { return direction; } set { direction = value; } }
			public int Speed { get { return speed; } set { speed = value; } }
			public int Delta { 
				get { 
					if(Speed == 0 || Direction == ScrollDirection.None) return 0;
					int res = Direction == ScrollDirection.Up || Direction == ScrollDirection.Left ? -1 : 1; 
					if(Speed == 1) res *= 3;
					return res;
				} 
			}
			static ScrollInfoArgs none;
			public static ScrollInfoArgs None {
				get { 
					if(none == null) none = new ScrollInfoArgs(ScrollDirection.None);
					return none;
				}
			}
		}
		static Cursor[] cursors;
		static Cursor[] Cursors {
			get {
				if(cursors == null) {
					cursors = new Cursor[7];
					cursors[(int)ScrollCursor.Full] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollFull.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Horz] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollHorz.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Vert] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollVert.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Left] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollLeft.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Right] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollRight.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Up] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollUp.cur", typeof(OfficeScroller).Assembly);
					cursors[(int)ScrollCursor.Down] = ResourceImageHelper.CreateCursorFromResources("DevExpress.Utils.Cursors.CursorScrollDown.cur", typeof(OfficeScroller).Assembly);
				}
				return cursors;
			}
		}
		[ThreadStatic]
		static ImageCollection scrollImages;
		static ImageCollection ScrollImages {
			get {
				if(scrollImages == null) {
					scrollImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.Scroll.bmp", typeof(OfficeScroller).Assembly, new Size(29, 29), Color.Empty);
				}
				return scrollImages;
			}
		}
		Timer timer;
		protected virtual int DefaultTimerInterval { get { return 20; } }
		public OfficeScroller() {
			this.timer = new Timer();
			this.timer.Interval = DefaultTimerInterval;
			this.timer.Tick += new EventHandler(OnTimerTick);
		}
		public virtual void Dispose() {
			Stop();
			this.timer.Dispose();
		}
		public void Start(Control control) {
			Start(control, Control.MousePosition);
		}
		ScrollerWindow window = null;
		ScrollMode mode = ScrollMode.None;
		Point anchor;
		public virtual void Start(Control control, Point screenPoint) {
			Stop();
			this.anchor = screenPoint;
			this.mode = GetScrollMode(control);
			if(Mode == ScrollMode.None) return;
			Image image = GetScrollBitmap(Mode);
			if(image == null) return;
			this.window = new ScrollerWindow(this, image);
			Window.Location = new Point(screenPoint.X - (Window.Width / 2), screenPoint.Y - (Window.Height / 2));
			Window.Show();
			Window.Capture = true;
			Cursor.Current = ActiveCursor;
			this.timer.Start();
			OnStartScroller();
		}
		public Point Anchor { get { return anchor; } }
		public virtual void Stop() {
			this.timer.Stop();
			if(this.window != null) {
				this.window.Dispose();
				this.window =  null;
			}
			ScrollMode prevMode = Mode;
			this.mode = ScrollMode.None;
			if(prevMode != ScrollMode.None) OnStopScroller();
		}
		public ScrollMode Mode { get { return mode; } }
		public ScrollerWindow Window { get { return window; } }
		protected virtual Image GetScrollBitmap(ScrollMode mode) {
			switch(mode) {
				case ScrollMode.Full : return ScrollImages.Images[0];
				case ScrollMode.Horz : return ScrollImages.Images[1];
				case ScrollMode.Vert : return ScrollImages.Images[2];
			}
			return null;
		}
		public int DeltaX { get { return Control.MousePosition.X - Anchor.X; } }
		public int DeltaY { get { return Control.MousePosition.Y - Anchor.Y; } }
		public Cursor ActiveCursor { get { return GetScrollCursor(ActiveInfo); } }
		public ScrollInfoArgs ActiveInfo { get { return GetScrollInfo(Control.MousePosition); } }
		Rectangle workingArea = Rectangle.Empty;
		Rectangle GetNeutralZone() {
			if(Window == null) return Rectangle.Empty;
			Rectangle res = Window.Bounds;
			if(this.workingArea.IsEmpty) this.workingArea = Screen.GetWorkingArea(Window);
			if(Mode == ScrollMode.Horz) {
				res.Y = this.workingArea.Y;
				res.Height = this.workingArea.Height;
			}
			if(mode == ScrollMode.Vert) {
				res.X = this.workingArea.X;
				res.Width = this.workingArea.Width;
			}
			return res;
		}
		protected internal virtual Cursor GetScrollCursor(ScrollInfoArgs info) {
			if(info.Direction == ScrollDirection.None) {
				switch(Mode) {
					case ScrollMode.Horz : return GetScrollCursor(ScrollCursor.Horz);
					case ScrollMode.Vert : return GetScrollCursor(ScrollCursor.Vert);
				}
				return GetScrollCursor(ScrollCursor.Full);
			}
			return GetScrollCursor((ScrollCursor)info.Direction);
		}
		protected internal virtual Cursor GetScrollCursor(ScrollCursor type) {
			return Cursors[(int)type];
		}
		protected internal virtual ScrollInfoArgs GetScrollInfo(Point pt) {
			if(Window == null || Mode == ScrollMode.None) return ScrollInfoArgs.None;
			Rectangle bounds = GetNeutralZone();
			if(bounds.Contains(pt)) return ScrollInfoArgs.None;
			ScrollInfoArgs res = new ScrollInfoArgs(ScrollDirection.None);
			if(AllowHScroll && (!AllowVScroll || Math.Abs(DeltaX) > Math.Abs(DeltaY))) {
				res.Direction = DeltaX > 0 ? ScrollDirection.Right : ScrollDirection.Left;
				res.Speed = Math.Max(12 - Math.Max(Math.Abs(DeltaX) / 12, 0), 1);
				return res;
			}
			if(AllowVScroll) {
				res.Direction = DeltaY > 0 ? ScrollDirection.Down : ScrollDirection.Up;
				res.Speed = Math.Max(12 - Math.Max(Math.Abs(DeltaY) / 12, 0), 1);
				return res;
			}
			return ScrollInfoArgs.None;
		}
		protected virtual ScrollMode GetScrollMode(Control control) {
			if(control == null || (!AllowHScroll && !AllowVScroll)) return ScrollMode.None;
			if(AllowHScroll && AllowVScroll) return ScrollMode.Full;
			if(AllowHScroll) return ScrollMode.Horz;
			return ScrollMode.Vert;
		}
		protected virtual void OnScroll(ScrollInfoArgs info) {
			if(info.Direction == ScrollDirection.Left || info.Direction == ScrollDirection.Right) 
				OnHScroll(info.Delta);
			if(info.Direction == ScrollDirection.Up || info.Direction == ScrollDirection.Down) 
				OnVScroll(info.Delta);
		}
		int timerTicks = 0;
		protected virtual void OnTimerTick(object sender, EventArgs e) {
			ScrollInfoArgs info = ActiveInfo;
			++this.timerTicks;
			if(info.Speed == 0) return;
			if(this.timerTicks % info.Speed == 0) OnScroll(info);
			((Timer)sender).Interval = DefaultTimerInterval;
		}
		protected virtual void OnStopScroller() { }
		protected virtual void OnStartScroller() { }
		protected virtual void OnHScroll(int delta) { }
		protected virtual void OnVScroll(int delta) { }
		protected virtual bool AllowHScroll { get { return true; } }
		protected virtual bool AllowVScroll { get { return true; } }
		public virtual bool DrawWindowBorder { get { return true; } }
		public class ScrollerWindow : TopFormBase, IMessageFilter {
			Image image;
			OfficeScroller scroller;
			public ScrollerWindow(OfficeScroller scroller, Image image) {
				SetStyle(ControlStyles.UserMouse, true);
				SetStyle(ControlStyles.Selectable, false);
				this.scroller = scroller;
				this.image = image;
				MinimumSize = Image.Size;
				Size = Image.Size;
				GraphicsPath path = new GraphicsPath();
				path.AddEllipse(0, 0, Image.Size.Width, Image.Size.Height);
				Region = BitmapToRegion.ConvertPathToRegion(path);
				Application.AddMessageFilter(this);
			}
			protected override void Dispose(bool disposing) {
				Application.RemoveMessageFilter(this);
				base.Dispose(disposing);
			}
			const int WM_KEYDOWN = 0x0100;
			bool IMessageFilter.PreFilterMessage(ref Message m) {
				if (m.Msg == WM_KEYDOWN) {
					Scroller.Stop();
					return true;
				}
				return false;
			}
			protected OfficeScroller Scroller { get { return scroller; } }
			protected Image Image { get { return image; } }
			protected override void OnPaint(PaintEventArgs e) {
				XPaint.Graphics.DrawImage(e.Graphics, Image, new Rectangle(Point.Empty, Image.Size));
				if(Scroller.DrawWindowBorder) {
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					e.Graphics.DrawEllipse(Pens.Black, 0, 0, Image.Size.Width, Image.Size.Height);
				}
			}
			protected override void OnPaintBackground(PaintEventArgs e) { }
			Point lastPosition = new Point(-999999, 0);
			protected override void OnKeyDown(KeyEventArgs e) {
				Scroller.Stop();
			}
			protected override void OnMouseMove(MouseEventArgs e) {
				if(lastPosition == new Point(e.X, e.Y)) return;
				lastPosition = new Point(e.X, e.Y);
				Cursor.Current = Scroller.ActiveCursor;
				Cursor = Scroller.ActiveCursor;
			}
			bool ignoreCapture = false;
			protected override void OnMouseDown(MouseEventArgs e) {
				base.OnMouseDown(e);
				Scroller.Stop();
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				base.OnMouseUp(e);
				if(e.Button == MouseButtons.Middle) {
					if(ClientRectangle.Contains(e.X, e.Y)) {
						try {
							this.ignoreCapture = true;
							Capture = true;
						} finally {
							this.ignoreCapture = false;
						}
						return;
					}
				}
				Scroller.Stop();
			}
			protected override void WndProc(ref Message m) {
				if(m.Msg == 520) { 
					int num1 = (short) ((int) m.LParam);
					int num2 = ((int) m.LParam) >> 0x10;
					Point p = new Point(num1, num2);
					OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 1, p.X, p.Y, 0));
					return;
				}
				if(m.Msg == MSG.WM_CAPTURECHANGED) {
					if(m.LParam == this.Handle) 
						OnGotCapture();
					else
						OnLostCapture();
					return;
				}
				base.WndProc(ref m);
				if(m.Msg == WM_NCHITTEST) m.Result = new IntPtr(HTTRANSPARENT);
			}
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT =(-1);
			protected override void OnLostCapture() {
				if(this.ignoreCapture) return;
				Scroller.Stop();
			}
		}
	}
}
