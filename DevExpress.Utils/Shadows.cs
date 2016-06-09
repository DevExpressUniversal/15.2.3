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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System.Security;
namespace DevExpress.Utils.Win {
	[ToolboxItem(false)]
	public class TopFormBase : Form {
		public TopFormBase() {
			this.Parent = null;
			this.TopLevel = true;
			this.ControlBox = false;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.Visible = false;
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		public virtual bool ICapture {
			get { return Capture; }
			set { Capture = value; 
			}
		}
		public virtual Rectangle RealBounds {
			get { return Rectangle.Empty; }
			set {
			}
		}
		const int WS_EX_TOOLWINDOW = 0x00000080;
		protected override CreateParams CreateParams {
			get {
				CreateParams res = base.CreateParams;
				res.ExStyle |= WS_EX_TOOLWINDOW;
				return res;
			}
		}
		protected virtual bool IsTopMost { get { return true; } }
		[SecuritySafeCritical]
		protected virtual void UpdateZOrder(IntPtr after) {
			if(!Visible) return;
			if(after == IntPtr.Zero)
				after = InsertAfterWindow;
			uint flags = SWP_NOACTIVATE;
			Rectangle realBounds = RealBounds;
			flags |= SWP_NOSIZE | SWP_NOMOVE;
			SetWindowPos(this.Handle, after,
				realBounds.X, realBounds.Y, realBounds.Width,realBounds.Height, flags);
		}
		protected virtual IntPtr InsertAfterWindow {
			get { return (IntPtr)(IsTopMost ? HWND_TOPMOST : HWND_TOP); }
		}
		protected void SetVisibleCoreBase(bool newVisible) {
			base.SetVisibleCore(newVisible);
		}
#if DXWhidbey
		protected override bool ShowWithoutActivation { get { return true; } }
		[SecuritySafeCritical]
		protected override void SetVisibleCore(bool newVisible) {
			SetVisibleCoreBase(newVisible);
			if(!newVisible) return;
			if(!IsHandleCreated) return;
			uint flags = SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOMOVE;
			SetWindowPos(Handle, InsertAfterWindow, 0, 0, 0, 0, flags);
		}
#else
		protected override void SetVisibleCore(bool newVisible) {
			if(!newVisible)
				base.SetVisibleCore(newVisible);
			else {
				CreateControl();
				uint flags = SWP_NOACTIVATE | SWP_SHOWWINDOW;
				Rectangle realBounds = RealBounds;
				flags |= SWP_NOSIZE | SWP_NOMOVE;
				SetWindowPos(this.Handle, InsertAfterWindow,
					realBounds.X, realBounds.Y, realBounds.Width,realBounds.Height, flags);
				ShowWindow(this.Handle, 8);
				MethodInfo mi = typeof(Control).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
				if(mi != null) mi.Invoke(this, new Object[] { 2, true});
				OnVisibleChanged(EventArgs.Empty);
				OnLoad(EventArgs.Empty);
			}
		}
#endif
		protected const int MA_NOACTIVATE = 3;
		protected virtual bool AllowMouseActivate { get { return false; } }
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			switch(m.Msg) {
				case MSG.WM_GETMINMAXINFO:
					WMGetMinMazInfo(ref m);
					return;
				case MSG.WM_MOUSEACTIVATE: 
					if(!AllowMouseActivate) {
						m.Result = (IntPtr)MA_NOACTIVATE;
						return;
					}
					break;
				case MSG.WM_LBUTTONDBLCLK: 
					OnDoubleClick(EventArgs.Empty);
					return;
				case MSG.WM_LBUTTONDOWN: 
					break;
				case MSG.WM_CAPTURECHANGED: 
					if(m.LParam == this.Handle) 
						OnGotCapture();
					else
						OnLostCapture();
					break;
			}
			base.WndProc(ref m);
		}
		[SecuritySafeCritical]
		private void WMGetMinMazInfo(ref Message m) {
			base.WndProc(ref m);
			NativeMethods.MINMAXINFO info = NativeMethods.MINMAXINFO.GetFrom(m.LParam); 
			info.ptMinTrackSize = new NativeMethods.POINT(1, 1);
			Marshal.StructureToPtr(info, m.LParam, true);
			m.Result = IntPtr.Zero;
		}
		protected virtual void OnLostCapture() { }
		protected virtual void OnGotCapture() { }
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
		internal const int SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOACTIVATE = 0x0010,
			SWP_SHOWWINDOW = 0x0040,
			HWND_TOP = 0,
			HWND_TOPMOST = -1;
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
			int X, int Y, int cx, int cy, uint uFlags);
	}
	[ToolboxItem(false)]
	public class CustomTopForm : TopFormBase {
		public CustomTopForm() {
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= 0x0800; 
				if(HasSystemShadow && (System.Environment.OSVersion.Version.Major > 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor > 0)))
					cp.ClassStyle |= 0x20000; 
				return cp;
			}
		}
		protected virtual bool HasSystemShadow { get { return false; } } 
	}
	[ToolboxItem(false)]
	public class StickLookAndFeelForm : CustomTopForm {
		EmptySkinElementPainter painter = new EmptySkinElementPainter();
		public StickLookAndFeelForm() {
			UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;
			UpdateRegion();
		}
		protected override void Dispose(bool disposing) {
			UserLookAndFeel.Default.StyleChanged -= Default_StyleChanged;
			base.Dispose(disposing);
		}
		void Default_StyleChanged(object sender, EventArgs e) {
			UpdateRegion();
		}
		protected override void OnPaint(PaintEventArgs e) {
			Skin skin = BarSkins.GetSkin(TargetLookAndFeel);
			GraphicsCache cache = new GraphicsCache(e);
			DrawContent(cache, skin);
			DrawTopElement(cache, skin);
			base.OnPaint(e);
		}
		Rectangle GetTopElementRectangle() {
			Rectangle r = this.ClientRectangle;
			return new Rectangle(r.X, r.Y, r.Width, 10);
		}
		protected virtual void DrawContent(GraphicsCache graphicsCache, Skin skin) {
			SkinElement element = skin[BarSkins.SkinAlertWindow];
			SkinElementInfo eInfo = new SkinElementInfo(element, this.ClientRectangle);
			ObjectPainter.DrawObject(graphicsCache, SkinElementPainter.Default, eInfo);
		}
		protected virtual void DrawTopElement(GraphicsCache graphicsCache, Skin skin) {
			SkinElement element = skin[BarSkins.SkinAlertCaptionTop];
			SkinElementInfo eInfo = new SkinElementInfo(element, GetTopElementRectangle());
			ObjectPainter.DrawObject(graphicsCache, painter, eInfo);
		}
		protected internal void UpdateRegion() {
			SkinElement se = BarSkins.GetSkin(TargetLookAndFeel)[BarSkins.SkinAlertWindow];
			if(se == null) {
				this.Region = null;
				return;
			}
			int cornerRadius = se.Properties.GetInteger(BarSkins.SkinAlertWindowCornerRadius);
			if(cornerRadius == 0) this.Region = null;
			else this.Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, this.Size), cornerRadius);
		}
		protected override bool HasSystemShadow { get { return true; } }
		protected virtual UserLookAndFeel TargetLookAndFeel { get { return UserLookAndFeel.Default; } }
		class EmptySkinElementPainter : SkinElementPainter {
			protected override void DrawSkinForeground(SkinElementInfo ee) { }
		}
	}
	public class ShadowManager : IDisposable {
		Form form;
		Rectangle creator;
		bool visible = false;
		const int rightShadow = 0, bottomShadow = 1, creatorRightShadow = 2, creatorBottomShadow = 3;
		int shadowSize = Shadow.DefaultShadowSize;
		Hashtable shadows = null;
		public ShadowManager(Form form) {
			this.creator = Rectangle.Empty;
			this.form = form;
			this.form.VisibleChanged += new EventHandler(OnForm_VisibleChanged);
			this.form.Move += new EventHandler(OnForm_Move);
		}
		public virtual void Dispose() {
			if(Form != null) {
				Hide();
				Form.Move -= new EventHandler(OnForm_Move);
				Form.VisibleChanged -= new EventHandler(OnForm_VisibleChanged);
				DestroyShadows();
				this.form = null;		
			}
		}
		public int ShadowSize { get { return shadowSize; } set { shadowSize = value; } }
		protected virtual void DestroyShadows() {
			if(this.shadows == null) return;
			foreach(DictionaryEntry entry in this.shadows) {
				(entry.Value as Shadow).Dispose();
			}
			this.shadows.Clear();
		}
		protected void DestroyShadow(int shadow) {
			Shadow sh = GetShadow(shadow);
			if(sh != null) {
				sh.Dispose();
				Shadows.Remove(sh);
			}
		}
		protected Hashtable Shadows { 
			get { 
				if(shadows == null) shadows = new Hashtable();
				return shadows;
			}
		}
		protected virtual Shadow CreateShadow(int shadow) {
			Shadow res = GetShadow(shadow);
			if(res == null) Shadows[shadow] = res = new Shadow(shadow % 2 != 0, 0, Form);
			return res;
		}
		protected Shadow GetShadow(int shadow) {
			if(this.shadows == null) return null;
			return Shadows[shadow] as Shadow;
		}
		void OnForm_VisibleChanged(object sender, EventArgs e) {
			if(!Form.Visible) Hide();
		}
		void OnForm_Move(object sender, EventArgs e) {
			if(Visible) Move(CreatorBounds);
		}
		public Form Form { get { return form; } }
		protected Rectangle CreatorBounds { 
			get { return creator; } 
			set { creator = value; }
		}
		public bool Visible { get { return visible; } }
		public void Show() { Show(Rectangle.Empty); }
		public virtual void Show(Rectangle creatorBounds) {
			if(Visible || !CanShowShadow) return;
			this.visible = true;
			UpdateShadowBounds();
			UpdateShadowRegions();
			foreach(DictionaryEntry entry in Shadows) {
				ShowShadow(entry.Value as Shadow);
			}
		}
		protected virtual void ShowShadow(Shadow shadow) {
			if(Form != null) Form.AddOwnedForm(shadow);
			shadow.ShowShadow();
		}
		public virtual void Move(Rectangle creatorBounds) {
			this.creator = creatorBounds;
			if(!Visible) return;
			UpdateShadowBounds();
			UpdateShadowRegions();
			foreach(DictionaryEntry entry in Shadows) {
				(entry.Value as Shadow).MoveShadow();
			}
		}
		protected virtual void UpdateShadowBounds() {
			Rectangle bounds = Form.Bounds;
			Rectangle vertRect = new Rectangle(bounds.Right, bounds.Top + ShadowSize, ShadowSize, bounds.Height);
			Rectangle horzRect = new Rectangle(bounds.X + ShadowSize, bounds.Bottom, bounds.Width - ShadowSize, ShadowSize);
			CreateShadow(rightShadow).RealBounds = vertRect;
			CreateShadow(bottomShadow).RealBounds = horzRect;
			Rectangle or = CreatorBounds;
			if(or.IsEmpty) {
				DestroyShadow(creatorBottomShadow);
				DestroyShadow(creatorRightShadow);
			} else {
				CreateShadow(creatorRightShadow).RealBounds = new Rectangle(or.Right, or.Top + ShadowSize, ShadowSize, or.Height);
				CreateShadow(creatorBottomShadow).RealBounds = new Rectangle(or.X + ShadowSize, or.Bottom, or.Width - ShadowSize, ShadowSize);
			}
		}
		public virtual void Hide() {
			if(!Visible) return;
			for(int n = Shadows.Count - 1; n >= 0; n--) {
				if(Shadows.Count > n) (Shadows[n] as Shadow).HideShadow();
			}
			this.visible = false;
		}
		protected virtual void UpdateShadowRegions() {
			if(!Visible || this.shadows == null) return;
			foreach(DictionaryEntry entry in this.shadows) {
				Shadow shadow = entry.Value as Shadow;
				shadow.Region = GetShadowRegion(shadow.RealBounds);
			}
		}
		Region GetShadowRegion(Rectangle shadow) {
			if(Form == null) return null;
			Rectangle i1 = CheckShadowIntersects(shadow, Form.Bounds),
				i2 = CheckShadowIntersects(shadow, CreatorBounds);
			if(!i1.IsEmpty || !i2.IsEmpty) {
				Region reg = new Region(new Rectangle(Point.Empty, shadow.Size));
				if(!i1.IsEmpty) reg.Exclude(i1);
				if(!i2.IsEmpty) reg.Exclude(i2);
				return reg;
			}
			return null;
		}
		Rectangle CheckShadowIntersects(Rectangle shadow, Rectangle rect) {
			if(rect.IsEmpty || shadow.IsEmpty || !rect.IntersectsWith(shadow)) return Rectangle.Empty;
			Rectangle r = rect;
			r.Offset(-shadow.X, -shadow.Y);
			if(r.X < 0) {
				r.Width += r.X;
				r.X = 0;
			}
			if(r.Y < 0) {
				r.Height += r.Y;
				r.Y = 0;
			}
			if(r.Width > 0 && r.Height > 0) return r;
			return Rectangle.Empty;
		}
		protected virtual bool CanShowShadow { 
			get { return Form != null && Form.Visible && !Form.Bounds.IsEmpty && !Form.Disposing; }
		}
		public void Move() { Move(CreatorBounds); }
		public virtual void Update() {
			Move();
		}
	}
	[ToolboxItem(false)]
	public class Shadow : CustomTopForm {
		bool horizontalShadow;
		Rectangle realBounds;
		Brush brush;
		Form owner;
		[ThreadStatic]
		static Brush vertBrush, horzBrush, cornerBrush;
		static Brush VertBrush {
			get {
				if(vertBrush == null) vertBrush = CreateBrush(false, 4);
				return vertBrush;
			}
		}
		static Brush HorzBrush {
			get {
				if(horzBrush == null) horzBrush = CreateBrush(true, 4);
				return horzBrush;
			}
		}
		static Brush CornerBrush {
			get {
				if(cornerBrush == null) cornerBrush = new LinearGradientBrush(new Rectangle(0, 0, 4, 4), Color.Black, Color.Gray, LinearGradientMode.BackwardDiagonal);
				return cornerBrush;
			}
		}
		static Brush CreateBrush(bool horz, int width) {
			Rectangle r = new Rectangle(0, 0, (horz ? width : 4), (horz ? 4 : width));
			Brush br = new LinearGradientBrush(r, Color.Black, Color.Gray, (!horz ? LinearGradientMode.Horizontal : System.Drawing.Drawing2D.LinearGradientMode.Vertical));
			return br;
		}
		public Shadow(bool horz, int width, Form owner) {
			this.SetStyle(ControlStyles.ResizeRedraw, false);
			this.owner = owner;
			this.Visible = false;
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			RectangleF r = new RectangleF(0, 0, (horz ? width : 4), (horz ? 4 : width));
			brush = (horz ? HorzBrush : VertBrush);
			this.Bounds = new Rectangle(-6000, -6000, 1,1);
			this.Opacity = 0.20;
			this.horizontalShadow = horz;
			HideShadow();
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.Style &= (~0x00800000); 
				return cp;
			}
		}
		public void HideShadow() {
			this.realBounds = new Rectangle(-6000, -6000, 0, 0);
			Bounds = realBounds;
			Visible = false;
		}
		public void MoveShadow() {
			if(RealBounds == Bounds) return;
			Bounds = RealBounds;
		}
		public void ShowShadow() {
			Visible = true;
			Bounds = RealBounds;
			UpdateZOrder(new IntPtr(-1)); 
			Refresh();
		} 
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			switch(m.Msg) {
				case 0x83:
					m.Result = new IntPtr(0);
					return;
			}
			base.WndProc(ref m);
		}
		protected override void OnPaint(PaintEventArgs e) {
			bool needHide = NeedHideCursor(this);
			if(needHide) Cursor.Hide();
			Rectangle r = ClientRectangle;
			if(!this.horizontalShadow) {
				r.Height -= 4;								
			}
			e.Graphics.FillRectangle(brush, r);
			if(!this.horizontalShadow) {
				e.Graphics.FillRectangle(Brushes.White, new Rectangle(r.X + 3, r.Y, 1, 1));
				r.Y = r.Bottom;
				r.Height = 4;								
				e.Graphics.FillRectangle(CornerBrush, r);
				e.Graphics.FillRectangle(Brushes.White, new Rectangle(r.X + 1, r.Y + 1, 3, 3));
				e.Graphics.FillRectangle(CornerBrush, new Rectangle(r.X + 1, r.Y + 1, 2, 2));
				e.Graphics.FillRectangle(Brushes.Gray, new Rectangle(r.X, r.Y + 3, 2, 1));
				e.Graphics.FillRectangle(Brushes.Gray, new Rectangle(r.X + 3, r.Y, 1, 2));
			} else {
				e.Graphics.FillRectangle(Brushes.White, new Rectangle(r.X, r.Y + 3, 1, 1));
			}
			if(needHide) Cursor.Show();
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
		}
		public override Rectangle RealBounds {
			get { return realBounds; }
			set {
				realBounds = value;
			}
		}
		internal static bool NeedHideCursor(Control control) {
			if(!control.Visible) return false;
			if(Cursor.Current == null) return false;
			Size size = Cursor.Current.Size;
			Point p = control.PointToClient(Cursor.Position);
			p.Offset(size.Width, size.Height);
			Rectangle r = control.ClientRectangle;
			r.Offset(size.Width, size.Height);
			r.Inflate(size);
			if(r.Contains(p)) return true;
			return false;
		}
		public static int DefaultShadowSize { get { return 4; } }
		public static void CreateShadows(ArrayList shadows, int shadowSize, bool canShow, Form form, Rectangle creatorRect) {
			CreateShadows(shadows, shadowSize, canShow, form, creatorRect, Rectangle.Empty);
		}
		public static void CreateShadows(ArrayList shadows, int shadowSize, bool canShow, Form form, Rectangle creatorRect1, Rectangle creatorRect2) {
			if(!canShow || !form.Visible || form.Bounds.IsEmpty || form.Disposing) {
				HideShadows(shadows);
				return;
			}
			Rectangle bounds = new Rectangle(form.PointToScreen(Point.Empty), form.ClientSize);
			Rectangle vertRect = new Rectangle(bounds.Right, bounds.Top + shadowSize, shadowSize, bounds.Height);
			Rectangle horzRect = new Rectangle(bounds.X + shadowSize, bounds.Bottom, bounds.Width - shadowSize, shadowSize);
			if(shadows.Count == 0) {
				Shadow vertShadow = new Shadow(false, shadowSize, form);
				Shadow horzShadow = new Shadow(true, shadowSize, form);
				shadows.Add(vertShadow);
				shadows.Add(horzShadow);
			}
			vertRect = CheckShadowRectangle(vertRect, creatorRect1, true);
			vertRect = CheckShadowRectangle(vertRect, creatorRect2, true);
			horzRect = CheckShadowRectangle(horzRect, creatorRect1, false);
			horzRect = CheckShadowRectangle(horzRect, creatorRect2, false);
			(shadows[0] as Shadow).RealBounds = vertRect;
			(shadows[1] as Shadow).RealBounds = horzRect;
			UpdateShadows(shadows, creatorRect1, creatorRect2, shadowSize);
			ShowShadows(shadows);
		}
		static Rectangle CheckRectangle(Rectangle r) {
			if(r.X < 0) {
				r.Width += r.X;
				r.X = 0;
			}
			if(r.Y < 0) {
				r.Height += r.Y;
				r.Y = 0;
			}
			return r;
		}
		public static void UpdateShadows(ArrayList shadows, Rectangle creatorRect, int shadowSize) {
			UpdateShadows(shadows, creatorRect, Rectangle.Empty, shadowSize);
		}
		public static void UpdateShadows(ArrayList shadows, Rectangle creatorRect1, Rectangle creatorRect2, int shadowSize) {
		}
		static Rectangle CheckShadowRectangle(Rectangle shadow, Rectangle rect, bool vert) {
			if(!shadow.IntersectsWith(rect)) return shadow;
			if(vert) {
				if(shadow.Y <= rect.Y) {
					shadow.Height = rect.Y - shadow.Y;
				} else {
					int delta = shadow.Y - rect.Y;
					shadow.Y += delta;
					shadow.Height -= delta;
				}
			} else {
				if(shadow.X <= rect.X) {
					shadow.Width = rect.X - shadow.X;
				} else {
					int delta = shadow.X - rect.X;
					shadow.X += delta;
					shadow.Width -= delta;
				}
			}
			return shadow;
		}
		static Rectangle CheckShadowIntersects(Rectangle shadow, Rectangle rect) {
			if(!rect.IntersectsWith(shadow)) return Rectangle.Empty;
			Rectangle r = rect;
			r.Offset(-shadow.X, -shadow.Y);
			r = CheckRectangle(r);
			if(r.Width > 0 && r.Height > 0) return r;
			return Rectangle.Empty;
		}
		public static void ShowShadows(ArrayList shadows) {
			ShowShadows(shadows, true);
		}
		public static void HideShadows(ArrayList shadows) {
			ShowShadows(shadows, false);
		}
		public static void ShowShadows(ArrayList shadows, bool show) {
			foreach(Shadow form in shadows) {
				if(show) {
					if(form.owner != null) form.owner.AddOwnedForm(form);
					form.ShowShadow();
				}
				else
					form.HideShadow();
			}
		}
		public static void DestroyShadows(ArrayList shadows) {
			if(shadows == null) return;
			foreach(Shadow shadow in shadows) {
				shadow.Dispose();
			}
			shadows.Clear();
		}
	}
}
