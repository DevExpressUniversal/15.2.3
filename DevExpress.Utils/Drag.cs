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
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Internal {
	public class DXLayeredWindow : NativeWindow, ILayeredWindow {
		const int WS_CAPTION = 0x00C00000, WS_BORDER = 0x00800000, WS_DLGFRAME = 0x00400000, WS_VSCROLL = 0x00200000,
			WS_HSCROLL = 0x00100000, WS_SYSMENU = 0x00080000, WS_THICKFRAME = 0x00040000,
			WS_EX_TOOLWINDOW = 0x00000080, WS_POPUP = unchecked((int)0x80000000),
			WS_EX_WINDOWEDGE = 0x00000100,
			WS_EX_LAYERED = 0x80000,
			WS_EX_TRANSPARENT = 0x20,
			LWA_COLORKEY = 1, LWA_ALPHA = 2;
		Rectangle bounds = Rectangle.Empty;
		bool visible = false;
		byte alpha = 255;
		Color transparencyKey = Color.Magenta;
		public Color TransparencyKey {
			get { return transparencyKey; }
			set { transparencyKey = value; }
		}
		public byte Alpha {
			get { return alpha; }
			set { alpha = value; }
		}
		IntPtr lastAfter = IntPtr.Zero;
		protected virtual bool IsLayeredWindow { get { return true; } } 
		public virtual bool IsTransparent { get { return true; } }
		IWin32Window topLevelCore;
		IWin32Window ILayeredWindow.TopLevel {
			get { return topLevelCore; }
		}
		public void Create(IWin32Window topLevel) {
			if(IsCreated) return;
			CreateParams cp = new CreateParams();
			topLevelCore = topLevel;
			cp.Parent = topLevel.Handle;
			cp.Style = WS_POPUP;
			if(IsLayeredWindow)
				cp.ExStyle = WS_EX_LAYERED;
			if(IsTransparent)
				cp.ExStyle |= WS_EX_TRANSPARENT;
			cp.Caption = null;
			CreateHandle(cp);
			if(!IsLayeredWindow) return;
			UpdateLayeredWindow();
		}
		public override void DestroyHandle() {
			base.DestroyHandle();
			topLevelCore = null;
		}
		protected virtual void UpdateLayeredWindow() {
			if(TransparencyKey.IsEmpty) {
				if(alpha != 255)
					NativeMethods.SetLayeredWindowAttributes(Handle, 0, alpha, LWA_ALPHA);
			}
			else {
				NativeMethods.SetLayeredWindowAttributes(Handle, ColorTranslator.ToWin32(TransparencyKey), alpha, (alpha != 255 ? LWA_ALPHA : 0) | LWA_COLORKEY);
			}
		}
		public static bool IsAllowLayeredWindow {
			get { return OSFeature.Feature.IsPresent(OSFeature.LayeredWindows); }
		}
		protected virtual IntPtr InsertAfterWindow { get { return new IntPtr(-1); } }
		public bool IsCreated { get { return Handle != IntPtr.Zero; } }
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				if(value)
					Show();
				else
					Hide();
				this.visible = value;
			}
		}
		internal void Reset() {
			lastSize = Size.Empty;
		}
		public void Show() { ShowCore(); }
		public void Show(Point location) {
			if(IsCreated && Visible && location == Bounds.Location) return;
			this.bounds.Location = location;
			ShowCore();
		}
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds == value) return;
				bounds = value;
				if(Visible && IsCreated) ShowCore();
			}
		}
		protected void SetSizeCore(Size size) {
			bounds.Size = size;
		}
		Size lastSize = Size.Empty;
		Rectangle lastBounds = Rectangle.Empty;
		protected void ShowCore() {
			if(!IsCreated) return;
			Rectangle validatedBounds = ValidateBounds(Bounds);
			if(Visible && validatedBounds == lastBounds) return;
			this.visible = true;
			int flags = NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_SHOWWINDOW | NativeMethods.SWP.SWP_DRAWFRAME;
			if(this.lastSize == validatedBounds.Size) {
				flags |= NativeMethods.SWP.SWP_NOSIZE;
			}
			this.lastSize = validatedBounds.Size;
			this.lastAfter = InsertAfterWindow;
			this.lastBounds = validatedBounds;
			if(validatedBounds.IsEmpty) flags = NativeMethods.SWP.SWP_HIDEWINDOW;
			if(InsertAfterWindow.ToInt32() != -1) {
				NativeMethods.SetWindowPos(Handle, InsertAfterWindow, validatedBounds.X, validatedBounds.Y, validatedBounds.Width, validatedBounds.Height, flags | NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOACTIVATE);
				if(flags != NativeMethods.SWP.SWP_HIDEWINDOW)
					NativeMethods.SetWindowPos(InsertAfterWindow, Handle, validatedBounds.X, validatedBounds.Y, validatedBounds.Width, validatedBounds.Height, flags | NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOMOVE);
			}
			else {
				NativeMethods.SetWindowPos(Handle, InsertAfterWindow, validatedBounds.X, validatedBounds.Y, validatedBounds.Width, validatedBounds.Height, flags);
			}
			OnVisibleChanged();
		}
		protected virtual Rectangle ValidateBounds(Rectangle bounds) {
			return bounds;
		}
		public virtual void Hide() {
			if(!IsCreated || !Visible) return;
			this.visible = false;
			NativeMethods.SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_HIDEWINDOW | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_NOZORDER);
			OnVisibleChanged();
		}
		protected virtual void OnVisibleChanged() { }
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_NCHITTEST:
					NCHitTest(ref m);
					return;
				case MSG.WM_PAINT:
					WMPaint(ref m);
					return;
				case MSG.WM_ERASEBKGND:
					WMEraseBkgnd(ref m);
					return;
			}
			base.WndProc(ref m);
			DevExpress.Utils.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void NCHitTest(ref Message m) {
			m.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT);
		}
		void WMPaint(ref Message m) {
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			IntPtr dc = NativeMethods.BeginPaint(Handle, ref ps);
			OnPaint(dc, ps.rcPaint.ToRectangle());
			NativeMethods.EndPaint(Handle, ref ps);
			m.Result = IntPtr.Zero;
		}
		void WMEraseBkgnd(ref Message m) {
			OnPaint(m.WParam, Rectangle.Empty);
			m.Result = IntPtr.Zero;
		}
		void OnPaint(IntPtr dc, Rectangle clipRect) {
			using(Graphics g = Graphics.FromHdc(dc)) {
				OnDraw(g);
			}
		}
		protected virtual bool UseDoubleBuffer { get { return false; } }
		protected void OnDraw(Graphics g) {
			if(UseDoubleBuffer) {
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, new Rectangle(Point.Empty, Size))) {
					DrawBackground(bg.Graphics);
					DrawForeground(bg.Graphics);
					bg.Render();
				}
			}
			else {
				DrawBackground(g);
				DrawForeground(g);
			}
		}
		protected virtual void DrawForeground(Graphics g) { }
		protected virtual void DrawBackground(Graphics g) {
			g.Clear(TransparencyKey);
		}
		public virtual Size Size {
			get { return bounds.Size; }
			set {
				if(Size == value) return;
				bounds.Size = value;
				OnWindowChanged();
			}
		}
		protected virtual void OnWindowChanged() {
			if(!IsCreated || !Visible) return;
			ShowCore();
		}
		public virtual void Invalidate() {
			if(!IsCreated || !Visible) return;
			NativeMethods.InvalidateRgn(Handle, IntPtr.Zero, true);
		}
	}
	public class DXSkinLayeredWindow : DXLayeredWindow {
		SkinImage skinImage = null;
		int imageIndex;
		public DXSkinLayeredWindow() { }
		public SkinImage SkinImage {
			get { return skinImage; }
			set {
				skinImage = value;
				CreateSkinElement();
				SetSizeCore(Size);
				OnWindowChanged();
			}
		}
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
			}
		}
		XSkinElement element;
		void CreateSkinElement() {
			SkinBuilderElementInfo bi = new SkinBuilderElementInfo();
			bi.Image = SkinImage;
			this.element = new XSkinElement(bi);
		}
		public override Size Size {
			get { return SkinImage.GetImageBounds(0).Size; }
		}
		protected override void DrawForeground(Graphics g) {
			if(element == null) return;
			SkinElementInfo info = new SkinElementInfo(this.element, new Rectangle(Point.Empty, Size));
			info.ImageIndex = ImageIndex;
			using(GraphicsCache cache = new GraphicsCache(g)) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
		}
		internal class XSkinElement : SkinElement {
			internal XSkinElement(SkinBuilderElementInfo info) : base(null, "Dummy", info) { }
		}
	}
	public static class LayeredWindowMessanger {
		public const int WM_USER = 0x0400;
		public const int WM_CLOSE = 0x0010;
		public const int WM_INVALIDATE = WM_USER + 0x00;
		public const int WM_CREATE = 0x0001;
		public const int WM_DESTROY = 0x0002;
		public static void PeekAllMessage(IntPtr hWnd, int msg) {
			NativeMethods.NativeMessage nMSG;
			while(NativeMethods.PeekMessage(out nMSG, hWnd, msg, 0, NativeMethods.PM_REMOVE)) ;
		}
		public static void PostInvalidate(IntPtr hWnd) {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.PostMessage(hWnd,
				WM_INVALIDATE, IntPtr.Zero, IntPtr.Zero);
		}
		public static void PostClose(IntPtr hWnd) {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.PostMessage(hWnd,
				WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
		}
	}
	public abstract class DXLayeredWindowEx : NativeWindow, IDisposable {
		const int
			WS_POPUP = unchecked((int)0x80000000),
			WS_EX_LAYERED = 0x80000,
			ULW_ALPHA = 0x02;
		byte AC_SRC_OVER = 0x00;
		byte AC_SRC_ALPHA = 0x01;
		byte alphaCore = 255;
		Rectangle bounds = Rectangle.Empty;
		bool isVisibleCore = false;
		Control ownerCore;
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
		}
		protected virtual void OnDisposing() {
			if(IsCreated)
				DestroyHandle();
		}
		public byte Alpha {
			get { return alphaCore; }
			set { alphaCore = value; }
		}
		protected virtual bool AllowSafeSize { get { return false; } }
		public void Create(IWin32Window topLevel) {
			ownerCore = topLevel as Control;
			Create(topLevel.Handle);
		}
		IntPtr handleCore = IntPtr.Zero;
		public void Create(IntPtr handle) {
			if(IsCreated) return;
			this.handleCore = handle;
			CreateParams cp = new CreateParams();
			cp.Parent = handle;
			cp.Style = WS_POPUP;
			cp.ExStyle = WS_EX_LAYERED;
			cp.Caption = null;
			CreateHandle(cp);
		}
		public bool IsCreated {
			get { return Handle != IntPtr.Zero; }
		}
		public bool IsVisible {
			get { return isVisibleCore; }
		}
		public Rectangle Bounds {
			get { return bounds; }
		}
		public void Show(Point location) {
			if(IsCreated && IsVisible && location == Bounds.Location) return;
			this.bounds.Location = location;
			ShowCore();
		}
		protected virtual IntPtr hWndInsertAfter { get { return new IntPtr(-1); } }
		Size lastSize = Size.Empty;
		[SecuritySafeCritical]
		protected void ShowCore() {
			if(!IsCreated) return;
			this.isVisibleCore = true;
			int flags = NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_SHOWWINDOW | NativeMethods.SWP.SWP_DRAWFRAME |
				NativeMethods.SWP.SWP_NOOWNERZORDER;
			if(this.lastSize == Size) {
				flags |= NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOZORDER;
			}
			this.lastSize = Size;
			NativeMethods.SetWindowPos(Handle, hWndInsertAfter, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, flags);
		}
		[SecuritySafeCritical]
		public void Hide() {
			if(!IsCreated || !IsVisible) return;
			NativeMethods.SetWindowPos(Handle, hWndInsertAfter, 0, 0, 0, 0,
				NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_HIDEWINDOW | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOMOVE
				| NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOOWNERZORDER);
			this.isVisibleCore = false;
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_NCHITTEST:
					NCHitTest(ref m);
					return;
			}
			base.WndProc(ref m);
		}
		protected virtual void NCHitTest(ref Message m) {
			m.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT);
		}
		public virtual Size Size {
			get { return bounds.Size; }
			set {
				if(Size == value) return;
				bounds.Size = value;
				OnWindowChanged();
			}
		}
		Size GetSafeSize() {
			if(!AllowSafeSize) return Bounds.Size;
			Size safeSize = new Size(150, 150);
			if(safeSize.Width < bounds.Width)
				safeSize.Width = bounds.Width;
			if(safeSize.Height < bounds.Height)
				safeSize.Height = bounds.Height;
			return safeSize;
		}
		protected virtual void OnWindowChanged() {
			if(!IsCreated || !IsVisible) return;
			ShowCore();
		}
		protected virtual Point GetPaintOffset() {
			return new Point(0, 0);
		}
		protected virtual Control CheckOwner() {
			return ownerCore;
		}
		volatile int updateRequested = 0;
		public void Invalidate() {
			if(updateRequested > 0) return;
			Control owner = CheckOwner();
			if(owner == null) {
				Update();
				return;
			}
			updateRequested++;
			owner.BeginInvoke(new MethodInvoker(UpdateAsync));
		}
		void UpdateAsync() {
			Update();
			updateRequested = 0;
		}
		public void Update() {
			if(CheckBounds()) return;
			UpdateLayeredWindowCore(DrawToBackBuffer);
		}
		public void Clear() {
			if(CheckBounds()) return;
			UpdateLayeredWindowCore(null);
		}
		protected void UpdateLayeredWindowCore(Action<IntPtr> updateBackBufferCallback) {
			IntPtr screenDC = NativeMethods.GetDC(IntPtr.Zero);
			IntPtr backBufferDC = NativeMethods.CreateCompatibleDC(screenDC);
			Size safeSize = GetSafeSize();
			IntPtr hBufferBitmap = Create32bppDIBSection(screenDC, safeSize.Width, safeSize.Height);
			IntPtr tmp = IntPtr.Zero;
			try {
				tmp = NativeMethods.SelectObject(backBufferDC, hBufferBitmap);
				if(updateBackBufferCallback != null)
					updateBackBufferCallback(backBufferDC);
				NativeMethods.POINT newLocation = new NativeMethods.POINT(Bounds.Location);
				NativeMethods.SIZE newSize = new NativeMethods.SIZE(safeSize);
				NativeMethods.POINT sourceLocation = new NativeMethods.POINT(0, 0);
				NativeMethods.BLENDFUNCTION blend = new NativeMethods.BLENDFUNCTION();
				blend.BlendOp = AC_SRC_OVER;
				blend.BlendFlags = 0;
				blend.SourceConstantAlpha = Alpha;
				blend.AlphaFormat = AC_SRC_ALPHA;
				NativeMethods.UpdateLayeredWindow(Handle, screenDC, ref newLocation, ref newSize, backBufferDC, ref sourceLocation, 0, ref blend, ULW_ALPHA);
			}
			finally {
				NativeMethods.SelectObject(backBufferDC, tmp);
				NativeMethods.DeleteObject(hBufferBitmap);
				NativeMethods.DeleteDC(backBufferDC);
				NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
			}
		}
		[SecuritySafeCritical]
		protected void DrawToBackBuffer(IntPtr backBufferDC) {
			Rectangle rect = new Rectangle(Point.Empty, GetSafeSize());
			using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(backBufferDC, rect)) {
				Point paintOffset = GetPaintOffset();
				bg.Graphics.TranslateTransform(paintOffset.X, paintOffset.Y);
				using(GraphicsCache cache = new GraphicsCache(bg.Graphics))
					DrawCore(cache);
				bg.Render();
			}
		}
		bool CheckBounds() {
			if(Bounds.Width <= 0 || Bounds.Height <= 0) return true;
			return false;
		}
		internal static IntPtr Create32bppDIBSection(IntPtr hDC, int w, int h) {
			NativeMethods.BITMAPINFO_SMALL bitmapInfo = new NativeMethods.BITMAPINFO_SMALL();
			bitmapInfo.biSize = Marshal.SizeOf(bitmapInfo);
			bitmapInfo.biBitCount = 0x20;
			bitmapInfo.biPlanes = 1;
			bitmapInfo.biWidth = w;
			bitmapInfo.biHeight = h;
			return NativeMethods.CreateDIBSection(hDC, ref bitmapInfo, 0, 0, IntPtr.Zero, 0);
		}
		protected abstract void DrawCore(GraphicsCache cache);
	}
	public class DXLayeredImageWindow : DXLayeredWindowEx {
		bool isActive;
		Image image;
		Timer delayedClosingTimer;
		Control parent;
		public DXLayeredImageWindow(Image image, Control parent) {
			this.image = image;
			this.parent = parent;
			this.Size = image.Size;
			this.delayedClosingTimer = CreateDelayedClosingTimer();
		}
		protected virtual Timer CreateDelayedClosingTimer() {
			Timer timer = new Timer();
			timer.Tick += OnTimerTick;
			return timer;
		}
		bool tickPerformed = true;
		void OnTimerTick(object sender, EventArgs e) {
			if(tickPerformed) return;
			tickPerformed = true;
			Close();
		}
		public new void Show(Point pos) {
			isActive = true;
			base.Create(ParentHandle);
			base.Show(pos);
			Update();
		}
		protected IntPtr ParentHandle {
			get {
				if(parent != null && parent.IsHandleCreated)
					return parent.Handle;
				return IntPtr.Zero;
			}
		}
		public void Close() {
			isActive = false;
			Dispose();
		}
		public void Close(int delay) {
			tickPerformed = false;
			delayedClosingTimer.Interval = delay;
			delayedClosingTimer.Start();
		}
		protected override void DrawCore(GraphicsCache cache) {
			if(image != null) cache.Graphics.DrawImage(image, Point.Empty);
		}
		protected override void OnDisposing() {
			this.image = null;
			this.parent = null;
			if(this.delayedClosingTimer != null) this.delayedClosingTimer.Dispose();
			this.delayedClosingTimer = null;
			base.OnDisposing();
		}
		public bool IsActive { get { return isActive; } }
	}
	public class DragArrowsHelper : IDisposable {
		Control owner;
		DXSkinLayeredWindow arrow1, arrow2;
		bool visible = false;
		bool isVertical = true;
		Point position1, position2;
		SkinImage arrows;
		public DragArrowsHelper(UserLookAndFeel lookAndFeel, Control owner)
			: this(owner, CheckArrowsSkinImage(GetArrowsSkinElementImageByLF(lookAndFeel))) {
		}
		public DragArrowsHelper(ISkinProvider provider, Control owner)
			: this(owner, CheckArrowsSkinImage(GetArrowsSkinElementImage(provider))) {
		}
		public DragArrowsHelper(Control owner, Bitmap bitmap)
			: this(owner, CheckArrowsImage(bitmap)) {
		}
		protected DragArrowsHelper(Control owner, SkinImage arrows) {
			this.arrows = CheckArrowsSkinImage(arrows);
			this.position2 = this.position1 = Point.Empty;
			this.owner = owner;
			this.arrow1 = new DXSkinLayeredWindow();
			this.arrow2 = new DXSkinLayeredWindow();
			this.arrow1.SkinImage = arrows;
			this.arrow2.SkinImage = arrows;
		}
		[ThreadStatic]
		static Bitmap defaultDragArrows;
		static Bitmap DefaultDragArrows {
			get {
				if(defaultDragArrows == null) {
					defaultDragArrows = (Bitmap)ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.DragArrows.png", typeof(DragArrowsHelper).Assembly);
					defaultDragArrows.MakeTransparent();
				}
				return defaultDragArrows;
			}
		}
		static SkinImage CheckArrowsImage(Image bitmap) {
			if(bitmap == null) bitmap = DefaultDragArrows;
			SkinImage i = new SkinImage(bitmap);
			i.ImageCount = 4;
			i.Layout = SkinImageLayout.Horizontal;
			return i;
		}
		static SkinImage CheckArrowsSkinImage(SkinImage image) {
			if(image == null || image.Image == null) {
				return CheckArrowsImage(null);
			}
			return image;
		}
		public static SkinImage GetArrowsSkinElementImageByLF(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return null;
			return GetArrowsSkinElementImage(lookAndFeel);
		}
		public static SkinImage GetArrowsSkinElementImage(ISkinProvider provider) {
			Skin skin = CommonSkins.GetSkin(provider);
			if(skin == null) return null;
			SkinElement res = skin[CommonSkins.SkinDropArrows];
			return res == null ? null : res.Image;
		}
		public static bool IsAllow {
			get { return SkinManager.AllowArrowDragIndicators && DXLayeredWindow.IsAllowLayeredWindow; }
		}
		Size GetImageSize() {
			Size res = this.arrows.GetImageBounds(0).Size;
			if(res.Width < 2) res.Width = 2;
			if(res.Height < 2) res.Height = 2;
			return res;
		}
		protected Control Owner { get { return owner; } }
		protected void Create() {
			if(!Owner.IsHandleCreated) return;
			if(arrow1.IsCreated) return;
			this.arrow1.Create(Owner);
			this.arrow2.Create(Owner);
		}
		public void Show(bool isVertical, Point pos1, Point pos2) {
			Create();
			this.isVertical = isVertical;
			this.position1 = UpdatePosition(pos1, true);
			this.position2 = UpdatePosition(pos2, false);
			arrow1.ImageIndex = isVertical ? 1 : 3;
			arrow2.ImageIndex = isVertical ? 0 : 2;
			Show();
		}
		Point UpdatePosition(Point pos, bool isBottom) {
			Point res = pos;
			if(IsVertical) {
				res.X -= GetImageSize().Width / 2;
				if(isBottom) res.Y -= GetImageSize().Height;
			}
			else {
				res.Y -= GetImageSize().Height / 2;
				if(isBottom) res.X -= GetImageSize().Width;
			}
			return res;
		}
		protected void Show() {
			arrow1.Show(position1);
			arrow2.Show(position2);
		}
		public void Reset() {
			if(arrow1 != null) arrow1.Reset();
			if(arrow2 != null) arrow2.Reset();
		}
		public bool IsVertical { get { return isVertical; } }
		public bool IsHorizontal { get { return !IsVertical; } }
		public bool IsVisible { get { return visible; } }
		public void Hide() {
			this.arrow2.Hide();
			this.arrow1.Hide();
			this.visible = false;
		}
		public virtual void Dispose() {
			if(arrow1 != null) {
				if(arrow1.IsCreated) arrow1.DestroyHandle();
			}
			if(arrow2 != null) {
				if(arrow2.IsCreated) arrow2.DestroyHandle();
			}
			this.owner = null;
		}
	}
	public interface ILayeredWindow {
		IWin32Window TopLevel { get; }
	}
	public enum LayeredWindowNotificationType {
		Hidden,
		Reparented,
	}
	public interface ILayeredWindowNotification {
		IntPtr Handle { get; }
		LayeredWindowNotificationType Type { get; }
	}
	public interface ILayeredWindowNotificationSource :
		IObservable<ILayeredWindowNotification>, IDisposable {
		void NotifyHidden(IntPtr parent);
		void NotityReparented(IntPtr parent);
	}
	public class LayeredWindowNotificationSource {
		#region static
		static readonly object syncRoot = new object();
		static readonly IDictionary<IntPtr, ILayeredWindowNotificationSource> notifiers = new Dictionary<IntPtr, ILayeredWindowNotificationSource>();
		static void Release(IntPtr handle) {
			lock(syncRoot) notifiers.Remove(handle);
		}
		public static void Register(IntPtr handle, ILayeredWindowNotificationSource notifier) {
			if(handle == IntPtr.Zero || notifier == null) return;
			lock(syncRoot) {
				ILayeredWindowNotificationSource existingNotifier;
				if(!notifiers.TryGetValue(handle, out existingNotifier) && notifiers.Values.Contains(notifier))
					notifiers.Add(handle, notifier);
			}
		}
		public static void Unregister(IntPtr handle, ILayeredWindowNotificationSource notifier) {
			if(handle == IntPtr.Zero || notifier == null) return;
			lock(syncRoot) {
				if(notifiers.Values.Contains(notifier))
					notifiers.Remove(handle);
			}
		}
		public static ILayeredWindowNotificationSource Register(IntPtr handle, bool controlNotifier = false) {
			if(handle == IntPtr.Zero)
				return null;
			lock(syncRoot) {
				ILayeredWindowNotificationSource notifier;
				if(!notifiers.TryGetValue(handle, out notifier)) {
					if(controlNotifier)
						notifier = new ControlNotifier(handle, (h) => Release(h));
					else
						notifier = new Notifier(handle, (h) => Release(h));
					notifiers.Add(handle, notifier);
				}
				return notifier;
			}
		}
		public static IObservable<ILayeredWindowNotification> FromHandle(IntPtr handle) {
			while(handle != IntPtr.Zero) {
				lock(syncRoot) {
					ILayeredWindowNotificationSource notifier;
					if(notifiers.TryGetValue(handle, out notifier))
						return notifier;
				}
				handle = Gesture.SwipeGestureHelper.GetParent(handle);
			}
			return null;
		}
		#endregion
		class ControlNotifier : Notifier, IWin32Window {
			internal ControlNotifier(IntPtr handle, Action<IntPtr> releaseAction)
				: base(handle, releaseAction) {
			}
		}
		class Notifier : ILayeredWindowNotificationSource {
			IntPtr handle;
			Action<IntPtr> releaseAction;
			List<IObserver<ILayeredWindowNotification>> observers;
			internal Notifier(IntPtr handle, Action<IntPtr> releaseAction) {
				this.handle = handle;
				this.observers = new List<IObserver<ILayeredWindowNotification>>();
				this.releaseAction = releaseAction;
			}
			bool isDisposing;
			void IDisposable.Dispose() {
				if(!isDisposing) {
					isDisposing = true;
					OnCompleted();
					observers.Clear();
					observers = null;
					if(releaseAction != null)
						releaseAction(Handle);
					releaseAction = null;
				}
				GC.SuppressFinalize(this);
			}
			public IntPtr Handle {
				get { return handle; }
			}
			bool IsDisposing {
				get { return isDisposing; }
			}
			void OnCompleted() {
				foreach(var observer in observers.ToArray())
					observer.OnCompleted();
			}
			void OnNext(ILayeredWindowNotification notification) {
				CallOnNext(notification);
				var children = GetChildren(notification.Handle);
				foreach(Notifier child in children)
					child.CallOnNext(notification);
			}
			void CallOnNext(ILayeredWindowNotification notification) {
				if(IsDisposing) return;
				foreach(var observer in observers.ToArray()) {
					var window = observer as ILayeredWindow;
					if(window != null && window.TopLevel != null) {
						if(!IsChild(window.TopLevel.Handle, notification.Handle))
							continue;
					}
					observer.OnNext(notification);
				}
			}
			IEnumerable<ILayeredWindowNotificationSource> GetChildren(IntPtr parent) {
				List<ILayeredWindowNotificationSource> children = new List<ILayeredWindowNotificationSource>();
				lock(syncRoot) {
					foreach(var p in notifiers) {
						if(p.Key == Handle) continue;
						var windowNotifier = p.Value as IWin32Window;
						if(windowNotifier != null && IsChild(windowNotifier.Handle, parent))
							if(!children.Contains(p.Value)) children.Add(p.Value);
					}
				}
				return children;
			}
			static bool IsChild(IntPtr handle, IntPtr parent) {
				while(handle != IntPtr.Zero) {
					if(handle == parent)
						return true;
					handle = Gesture.SwipeGestureHelper.GetParent(handle);
				}
				return false;
			}
			void ILayeredWindowNotificationSource.NotifyHidden(IntPtr parent) {
				OnNext(new HiddenNotification(parent));
			}
			void ILayeredWindowNotificationSource.NotityReparented(IntPtr parent) {
				OnNext(new ReparentedNotification(parent));
			}
			IDisposable IObservable<ILayeredWindowNotification>.Subscribe(IObserver<ILayeredWindowNotification> observer) {
				return new Subscription(this, observer);
			}
			class Subscription : IDisposable {
				List<IObserver<ILayeredWindowNotification>> observers;
				IObserver<ILayeredWindowNotification> observer;
				public Subscription(Notifier notifier, IObserver<ILayeredWindowNotification> observer) {
					this.observers = notifier.observers;
					this.observer = observer;
					if(observers != null && observer != null)
						if(!observers.Contains(observer)) observers.Add(observer);
				}
				public void Dispose() {
					if(observers != null && observer != null)
						observers.Remove(observer);
					observers = null;
					observer = null;
				}
			}
		}
		#region Notifications
		abstract class Notification : ILayeredWindowNotification {
			protected Notification(IntPtr handle, LayeredWindowNotificationType type) {
				this.Handle = handle;
				this.Type = type;
			}
			public IntPtr Handle {
				get;
				private set;
			}
			public LayeredWindowNotificationType Type {
				get;
				private set;
			}
		}
		class HiddenNotification : Notification {
			public HiddenNotification(IntPtr handle)
				: base(handle, LayeredWindowNotificationType.Hidden) {
			}
		}
		class ReparentedNotification : Notification {
			public ReparentedNotification(IntPtr handle)
				: base(handle, LayeredWindowNotificationType.Reparented) {
			}
		}
		#endregion Notificetions
	}
}
