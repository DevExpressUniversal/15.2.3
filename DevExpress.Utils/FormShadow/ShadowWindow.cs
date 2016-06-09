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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using FormShadowHelpers = DevExpress.Utils.FormShadow.Helpers;
namespace DevExpress.Utils.FormShadow {
	public enum ShadowWindowTypes { 
		Left, Top, Right, Bottom, Composite 
	}
	public class ShadowWindowLayoutCalculator {
		static Size defaultImageSize = new Size(5, 5);
		public static Size GetImageSize(SkinElement element) {
			if(element == null || element.Image == null || element.Image.Image == null)
				return defaultImageSize;
			return element.Image.GetImageBounds(0).Size;
		}
		public static int GetShadowThick(SkinElement elem, ShadowWindowTypes windowType) {
			return IsHorizontalWindowType(windowType) ? GetImageSize(elem).Width : GetImageSize(elem).Height;
		}
		static bool IsHorizontalWindowType(ShadowWindowTypes windowType) {
			return windowType == ShadowWindowTypes.Left || windowType == ShadowWindowTypes.Right;
		}
		public static Rectangle Calculate(ShadowWindowTypes windowType, Rectangle windowRectagle, SkinShadowWindowPainter painter) {
			Rectangle result = windowRectagle;
			SkinElement element = painter.GetSkinElementByWindowType(windowType);
			int offset = painter.GetOffsetByWindowType(windowType);
			int shadowThick = GetShadowThick(element, windowType);
			if(painter is RibbonFormSkinShadowPainter && painter.IsGlow)
				shadowThick = (int)(shadowThick * 1.5);
			switch(windowType) {
				case ShadowWindowTypes.Left:
					result.X = result.X - shadowThick + offset;
					result.Y = result.Y - shadowThick + offset;
					result.Height += (shadowThick - offset) * 2;
					result.Width = shadowThick;
					return result;
				case ShadowWindowTypes.Top:
					result.X = result.X + offset;
					result.Y = result.Y - shadowThick + offset;
					result.Height = shadowThick;
					result.Width = result.Width - 2 * offset;
					return result;
				case ShadowWindowTypes.Right:
					result.X = result.Right - offset;
					result.Y = result.Y - shadowThick + offset;
					result.Height += (shadowThick - offset) * 2;
					result.Width = shadowThick;
					return result;
				case ShadowWindowTypes.Bottom:
					result.X = result.X + offset;
					result.Y = result.Bottom - offset;
					result.Height = shadowThick;
					result.Width = result.Width - 2 * offset;
					return result;
				case ShadowWindowTypes.Composite:
					Rectangle rectL = Calculate(ShadowWindowTypes.Left, windowRectagle, painter);
					Rectangle rectR = Calculate(ShadowWindowTypes.Right, windowRectagle, painter);
					result.X = rectL.Left;
					result.Y = rectL.Top;
					result.Height = rectL.Height;
					result.Width = rectR.Right - rectL.Left;
					return result;
			}
			return Rectangle.Empty;
		}
	}
	public class CompositeBitmapAttributes {
		public CompositeBitmapAttributes() { }
		public CompositeBitmapAttributes(Size compositeBmp, Size leftBorder, Size topBorder, Size rightBorder, Size bottomBorder, SkinShadowWindowPainter painter, bool isActive) {
			CompositeBmpSize = compositeBmp;
			LeftBorderSize = leftBorder;
			TopBorderSize = topBorder;
			RightBorderSize = rightBorder;
			BottomBorderSize = bottomBorder;
			SkinPainter = painter;
			IsActive = isActive;
		}
		public Size CompositeBmpSize { get; set; }
		public Size LeftBorderSize { get; set; }
		public Size TopBorderSize { get; set; }
		public Size RightBorderSize { get; set; }
		public Size BottomBorderSize { get; set; }
		public SkinShadowWindowPainter SkinPainter { get; set; }
		public bool IsActive { get; set; }
	}
	public class BitmapManager : DisposableObject {
		ShadowWindowTypes windowTypeCore;
		public BitmapManager(ShadowWindowTypes windowType) {
			windowTypeCore = windowType;
		}
		Bitmap cachedBitmapCore;
		Bitmap CachedBitmap {
			get { return cachedBitmapCore; }
			set {
				if(cachedBitmapCore == value) return;
				Ref.Dispose(ref cachedBitmapCore);
				cachedBitmapCore = value;
			}
		}
		public Size CachedBmpSize {
			get {
				if(CachedBitmap == null) return Size.Empty;
				return CachedBitmap.Size;
			}
		}
		public Bitmap GetBitmap(Size size, SkinShadowWindowPainter skinPainter, bool active) {
			if(size.Width == 0 || size.Height == 0) return null;
			if(CachedBitmap != null && CachedBitmap.Size == size) 
				return CachedBitmap;
			CachedBitmap = CreateBorderBitmap(size, windowTypeCore, skinPainter, active);
			return CachedBitmap;
		}
		Bitmap cachedBeakBitmapCore;
		Bitmap CachedBeakBitmap {
			get { return cachedBeakBitmapCore; }
			set {
				if(cachedBeakBitmapCore == value) return;
				Ref.Dispose(ref cachedBeakBitmapCore);
				cachedBeakBitmapCore = value;
			}
		}
		Bitmap cachedBeakBorderBitmapCore;
		Bitmap CachedBeakBorderBitmap {
			get { return cachedBeakBorderBitmapCore; }
			set {
				if(cachedBeakBorderBitmapCore == value) return;
				Ref.Dispose(ref cachedBeakBorderBitmapCore);
				cachedBeakBorderBitmapCore = value;
			}
		}
		BeakFormAlignment beakFormAlignmentCore;
		int beakOffsetCore;
		Bitmap CachedBeakLeftArrowBitmap { get { return GetArrowBitmap(BeakFormAlignment.Right); } }
		Bitmap CachedBeakRightArrowBitmap { get { return GetArrowBitmap(BeakFormAlignment.Left); } }
		Bitmap CachedBeakUpArrowBitmap { get { return GetArrowBitmap(BeakFormAlignment.Bottom); } }
		Bitmap CachedBeakDownArrowBitmap { get { return GetArrowBitmap(BeakFormAlignment.Top); } }
		Dictionary<BeakFormAlignment, Bitmap> arrows;
		Dictionary<BeakFormAlignment, Bitmap> Arrows {
			get {
				if(arrows == null)
					arrows = new Dictionary<BeakFormAlignment, Bitmap>();
				return arrows;
			}
		}
		Bitmap GetArrowBitmap(BeakFormAlignment alignment) {
			Bitmap arrowBitmap = null;
			if(Arrows.TryGetValue(alignment, out arrowBitmap)) {
				return arrowBitmap;
			}
			return null;
		}
		Size GetBeakImageSize(BeakFormSkinShadowPainter painter, BeakFormAlignment alignment) {
			if(painter == null) return Size.Empty;
			SkinElement element = painter.GetSkinElementByBeakAlignment(alignment);
			if(element == null) return Size.Empty;
			return ShadowWindowLayoutCalculator.GetImageSize(element);
		}
		public Bitmap GetCompositeBitmapWithBeak(CompositeBitmapAttributes compositeAttr, BeakFormAlignment alignment, int beakOffset, bool isBeakVisible) {
			var beakImageSize = GetBeakImageSize(compositeAttr.SkinPainter as BeakFormSkinShadowPainter, alignment);
			if(beakImageSize.IsEmpty) return null;
			if(compositeAttr.CompositeBmpSize.Width == 0 || compositeAttr.CompositeBmpSize.Height == 0) return null;
			if(ShouldUseCachedBeakBitmap(compositeAttr.CompositeBmpSize, alignment, beakOffset, compositeAttr.IsActive)) return CachedBeakBitmap;
			beakFormAlignmentCore = alignment;
			beakOffsetCore = beakOffset;
			var result = GetCompositeBitmap(compositeAttr);
			if(result == null) return null;
			var offset = compositeAttr.SkinPainter.GetOffsetByWindowType(ShadowWindowTypes.Left);
			CachedBeakBitmap = new Bitmap(result);
			if(GetArrowBitmap(alignment) == null)
				Arrows[alignment] = CreateBeakBitmap(beakImageSize, alignment, compositeAttr.SkinPainter);
			var point = new Point();
			var border = GetBeakBorderBitmap(compositeAttr);
			using(Graphics g = Graphics.FromImage(CachedBeakBitmap)) {
				if(isBeakVisible) {
					switch(alignment) {
						case BeakFormAlignment.Left:
							point.X = compositeAttr.LeftBorderSize.Width + compositeAttr.TopBorderSize.Width + offset;
							point.Y = compositeAttr.TopBorderSize.Height - offset + beakOffset - CachedBeakRightArrowBitmap.Height / 2;
							g.DrawImageUnscaled(CachedBeakRightArrowBitmap, point);
							break;
						case BeakFormAlignment.Top:
							point.X = compositeAttr.LeftBorderSize.Width - offset + beakOffset - CachedBeakDownArrowBitmap.Width / 2;
							point.Y = compositeAttr.CompositeBmpSize.Height - compositeAttr.BottomBorderSize.Height + offset;
							g.DrawImageUnscaled(CachedBeakDownArrowBitmap, point);
							break;
						case BeakFormAlignment.Right:
							point.X = compositeAttr.LeftBorderSize.Width - offset - CachedBeakLeftArrowBitmap.Width;
							point.Y = compositeAttr.TopBorderSize.Height - offset + beakOffset - CachedBeakLeftArrowBitmap.Height / 2;
							g.DrawImageUnscaled(CachedBeakLeftArrowBitmap, point);
							break;
						case BeakFormAlignment.Bottom:
							point.X = compositeAttr.LeftBorderSize.Width - offset + beakOffset - CachedBeakUpArrowBitmap.Width / 2;
							point.Y = compositeAttr.TopBorderSize.Height - offset - CachedBeakUpArrowBitmap.Height;
							g.DrawImageUnscaled(CachedBeakUpArrowBitmap, point);
							break;
					}
				}
				var beakSkinPainter = compositeAttr.SkinPainter as BeakFormSkinShadowPainter;
				if(beakSkinPainter != null && border != null) {
					int thickness = beakSkinPainter.GetBorderThickness();
					point.X = compositeAttr.LeftBorderSize.Width - offset - thickness;
					point.Y = compositeAttr.TopBorderSize.Height - offset - thickness;
					g.DrawImageUnscaled(border, point);
				}
			}
			return CachedBeakBitmap;
		}
		bool ShouldUseCachedBeakBitmap(Size newSize, BeakFormAlignment newAlignment, int newBeakOffset, bool newActiveState) {
			if(CachedBeakBitmap != null
				&& CachedBeakBitmap.Size == newSize
				&& beakFormAlignmentCore == newAlignment
				&& beakOffsetCore == newBeakOffset
				&& activeCore == newActiveState) return true;
			return false;
		}
		Bitmap CreateBeakBitmap(Size imageSize, BeakFormAlignment alignment, SkinShadowWindowPainter painter) {
			if(imageSize.Width <= 0 || imageSize.Height <= 0) return null;
			var res = new Bitmap(imageSize.Width, imageSize.Height);
			using(Graphics beak = Graphics.FromImage(res)) {
				BeakFormInfo info = new BeakFormInfo();
				info.BeakAlignment = alignment;
				info.IsBorder = false;
				info.Bounds = new Rectangle(Point.Empty, imageSize);
				info.Cache = new GraphicsCache(beak);
				info.State = ObjectState.Normal;
				painter.DrawObject(info);
			}
			return res;
		}
		bool activeCore = false;
		public Bitmap GetBeakBorderBitmap(CompositeBitmapAttributes compositeAttr) {
			if(compositeAttr.CompositeBmpSize.Width == 0 || compositeAttr.CompositeBmpSize.Height == 0) return null;
			int offset = compositeAttr.SkinPainter.GetOffsetByWindowType(ShadowWindowTypes.Left);
			var beakSkinPainter = compositeAttr.SkinPainter as BeakFormSkinShadowPainter;
			if(beakSkinPainter == null) 
				return null;
			int thickness = beakSkinPainter.GetBorderThickness();
			SkinElement element = beakSkinPainter.GetSkinElementByWindowType(ShadowWindowTypes.Left);
			int shadowThick = ShadowWindowLayoutCalculator.GetShadowThick(element, ShadowWindowTypes.Left);
			int width = compositeAttr.CompositeBmpSize.Width - 2 * (shadowThick - offset) + 2 * thickness;
			int height = compositeAttr.CompositeBmpSize.Height - 2 * (shadowThick - offset) + 2 * thickness;
			var actualSize = new Size(width, height);
			if(CachedBeakBorderBitmap != null && CachedBeakBorderBitmap.Size == actualSize) 
				return CachedBeakBorderBitmap;
			Bitmap beakBorderBitmap = new Bitmap(width, height);
			using(Graphics g = Graphics.FromImage(beakBorderBitmap)) {
				using(var cache = new GraphicsCache(g)) {
					BeakFormInfo info = new BeakFormInfo();
					info.IsBorder = true;
					info.Bounds = new Rectangle(Point.Empty, new Size(width, height));
					info.Cache = cache;
					info.State = ObjectState.Normal;
					compositeAttr.SkinPainter.DrawObject(info);
				}
			}
			CachedBeakBorderBitmap = beakBorderBitmap;
			return beakBorderBitmap;
		}
		public void ReleaseCompositeBitmaps() {
			Ref.Dispose(ref cachedBitmapCore);
		}
		public Bitmap GetCompositeBitmap(CompositeBitmapAttributes compositeAttr) {
			var size = compositeAttr.CompositeBmpSize;
			if(size.Width == 0 || size.Height == 0) return null;
			if(CachedBitmap != null && CachedBitmap.Size == size && activeCore == compositeAttr.IsActive)
				return CachedBitmap;
			int offset = compositeAttr.SkinPainter.GetOffsetByWindowType(ShadowWindowTypes.Left);
			int minHeight = compositeAttr.TopBorderSize.Height + compositeAttr.BottomBorderSize.Height;
			int minWidth = compositeAttr.LeftBorderSize.Width + compositeAttr.RightBorderSize.Width;
			if(size.Width < minWidth || size.Height < minHeight) {
				CachedBitmap = null;
				return null;
			}
			Bitmap result = null, leftBmp = null, topBmp = null, rightBmp = null, bottomBmp = null;
			try {
				leftBmp = CreateBorderBitmap(compositeAttr.LeftBorderSize, ShadowWindowTypes.Left, compositeAttr.SkinPainter, compositeAttr.IsActive);
				topBmp = CreateBorderBitmap(compositeAttr.TopBorderSize, ShadowWindowTypes.Top, compositeAttr.SkinPainter, compositeAttr.IsActive);
				rightBmp = CreateBorderBitmap(compositeAttr.RightBorderSize, ShadowWindowTypes.Right, compositeAttr.SkinPainter, compositeAttr.IsActive);
				bottomBmp = CreateBorderBitmap(compositeAttr.BottomBorderSize, ShadowWindowTypes.Bottom, compositeAttr.SkinPainter, compositeAttr.IsActive);
				result = new Bitmap(size.Width, size.Height);
				using(Graphics g = Graphics.FromImage(result)) {
					g.DrawImageUnscaled(leftBmp, 0, 0);
					g.DrawImageUnscaled(topBmp, compositeAttr.LeftBorderSize.Width, 0);
					g.DrawImageUnscaled(rightBmp, compositeAttr.LeftBorderSize.Width + compositeAttr.TopBorderSize.Width, 0);
					g.DrawImageUnscaled(bottomBmp, compositeAttr.LeftBorderSize.Width, size.Height - compositeAttr.BottomBorderSize.Height);
				}
				this.activeCore = compositeAttr.IsActive;
				CachedBitmap = result;
			}
			catch {
				CachedBitmap = null;
				return null; 
			}
			finally {
				Ref.Dispose(ref leftBmp);
				Ref.Dispose(ref topBmp);
				Ref.Dispose(ref rightBmp);
				Ref.Dispose(ref bottomBmp);
			}
			return result;
		}
		Bitmap CreateBorderBitmap(Size imageSize, ShadowWindowTypes type, SkinShadowWindowPainter painter, bool isActive) {
			if(imageSize.Width <= 0 || imageSize.Height <= 0) return null;
			var res = new Bitmap(imageSize.Width, imageSize.Height);
			using(Graphics g = Graphics.FromImage(res)) {
				using(var cache = new GraphicsCache(g)) {
					ShadowWindowInfo info = new ShadowWindowInfo();
					info.WindowType = type;
					info.Active = isActive;
					info.Bounds = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
					info.State = ObjectState.Normal;
					info.Cache = cache;
					painter.DrawObject(info);
				}
			}
			return res;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Ref.Dispose(ref cachedBitmapCore);
				Ref.Dispose(ref cachedBeakBitmapCore);
				Ref.Dispose(ref cachedBeakBorderBitmapCore);
			}
			base.Dispose(disposing);
		}
	}
	[ComVisible(true)]
	public abstract class HwndWrapper : DisposableObject {
		private IntPtr handle;
		private bool isCreatingHandle;
		private Int32 wndClassAtom;
		private Delegate wndProc;
		protected Int32 WindowClassAtom {
			get {
				if(wndClassAtom == 0) {
					wndClassAtom = CreateWindowClassCore();
				}
				return wndClassAtom;
			}
		}
		public IntPtr Handle {
			get {
				EnsureHandle();
				return handle;
			}
		}
		protected abstract bool IsWindowSubclassed { get; }
		protected abstract Int32 CreateWindowClassCore();
		protected abstract void DestroyWindowClassCore();
		[System.Security.SecuritySafeCritical]
		void SubclassWndProc() {
			wndProc = new NativeMethods.WndProc(WndProc);
			IntPtr pWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);
			NativeMethods.SetWindowLong(handle, FormShadowHelpers.NativeHelper.GWL_WNDPROC, pWndProc);
		}
		protected abstract IntPtr CreateWindowCore();
		protected virtual void DestroyWindowCore() {
			if(handle != IntPtr.Zero) {
				NativeMethods.DestroyWindow(handle);
				handle = IntPtr.Zero;
			}
		}
		protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
			return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
		}
		public void EnsureHandle() {
			if(handle == IntPtr.Zero) {
				if(isCreatingHandle) {
					return;
				}
				isCreatingHandle = true;
				try {
					handle = CreateWindowCore();
					if(IsWindowSubclassed) {
						SubclassWndProc();
					}
				}
				finally {
					isCreatingHandle = false;
				}
			}
		}
		protected void DisposeNativeResources() {
			DestroyWindowCore();
			DestroyWindowClassCore();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposed) {
				DisposeNativeResources();
			}
			base.Dispose(disposing);
		}
	}
	public abstract class ShadowWindow : HwndWrapper {
		const int fastMoveDelayDurationMs = 150;
		FormShadow ownerCore;
		protected BitmapManager bmpManager;
		ShadowWindowTypes windowType;
		protected SkinShadowWindowPainter painter;
		byte opacity;
		public ShadowWindow(FormShadow owner, ShadowWindowTypes windowType) {
			ownerCore = owner;
			WindowType = windowType;
			bmpManager = CreateBitmapManager();
			painter = CreatePainter();
			this.opacity = 255;
		}
		public bool IsGlow {
			get {
				if(painter == null) return false;
				return painter.IsGlow;
			}
			set {
				if(painter == null || painter.IsGlow == value) return;
				painter.IsGlow = value;
			}
		}
		public Color ActiveGlowColor {
			get {
				if(painter == null) return Color.Transparent;
				return painter.ActiveGlowColor;
			}
			set {
				if(painter == null || painter.ActiveGlowColor == value) return;
				painter.ActiveGlowColor = value;
			}
		}
		public Color InactiveGlowColor {
			get {
				if(painter == null) return Color.Transparent;
				return painter.InactiveGlowColor;
			}
			set {
				if(painter == null || painter.InactiveGlowColor == value) return;
				painter.InactiveGlowColor = value;
			}
		}
		public int Offset {
			get {
				if(painter == null) return 0;
				var type = WindowType;
				if(type == ShadowWindowTypes.Composite) type = ShadowWindowTypes.Left;
				return painter.GetOffsetByWindowType(type);
			}
		}
		protected ISkinProvider GetISkinProvider() {
			if(ownerCore != null && ownerCore.Form != null && ownerCore.Form is ISkinProvider) {
				ISkinProvider isp = ownerCore.Form as ISkinProvider;
				return isp;
			}
			if(ownerCore != null && ownerCore.Form != null && ownerCore.Form is ISupportLookAndFeel) {
				ISupportLookAndFeel lf = ownerCore.Form as ISupportLookAndFeel;
				return lf.LookAndFeel;
			}
			return null;
		}
		protected abstract SkinShadowWindowPainter CreatePainter();
		protected virtual BitmapManager CreateBitmapManager() { return new BitmapManager(windowType); }
		public ShadowWindowTypes WindowType {
			get { return windowType; }
			set { windowType = value; }
		}
		protected override bool IsWindowSubclassed {
			get { return true; }
		}
		protected internal byte Opacity {
			get { return opacity; }
			set {
				if(opacity == value) return;
				opacity = value;
				OnOpacityChanged();
			}
		}
		protected override IntPtr CreateWindowCore() {
			IntPtr owner = NativeMethods.GetWindow(ownerCore.Form.Handle, NativeMethods.GW_OWNER);
			return NativeMethods.CreateWindowEx(
						FormShadowHelpers.NativeHelper.WS_EX_LAYERED | FormShadowHelpers.NativeHelper.WS_EX_TOOLWINDOW | FormShadowHelpers.NativeHelper.WS_EX_NOACTIVATE ,
						new IntPtr(base.WindowClassAtom),
						string.Empty,
						-(FormShadowHelpers.NativeHelper.WS_VISIBLE | FormShadowHelpers.NativeHelper.WS_MINIMIZE | FormShadowHelpers.NativeHelper.WS_CHILDWINDOW | FormShadowHelpers.NativeHelper.WS_CLIPCHILDREN | FormShadowHelpers.NativeHelper.WS_DISABLED | FormShadowHelpers.NativeHelper.WS_POPUP),
						0, 0, 0, 0,
						owner,
						IntPtr.Zero,
						IntPtr.Zero,
						IntPtr.Zero);
		}
		protected override void DestroyWindowClassCore() {
			if(bmpManager != null)
				bmpManager.Dispose();
			if(moveTimerCore != null) {
				moveTimerCore.Tick -= moveTimerCore_Tick;
				moveTimerCore.Dispose();
			}
			if(painter != null)
				painter.ReleaseAdditionalGlowResources();
			if(sharedWindowClassAtom != 0 && NativeMethods.UnregisterClass(new IntPtr(base.WindowClassAtom), IntPtr.Zero))
				sharedWindowClassAtom = 0;
		}
		protected override Int32 CreateWindowClassCore() {
			return SharedWindowClassAtom;
		}
		bool isError = false;
		static Int32 sharedWindowClassAtom;
		static NativeMethods.WndProc sharedWndProc;
		Int32 SharedWindowClassAtom {
			get {
				if(ShadowWindow.sharedWindowClassAtom == 0)
					ShadowWindow.sharedWindowClassAtom = RegisterWindowClass();
				return ShadowWindow.sharedWindowClassAtom;
			}
		}
		[System.Security.SecuritySafeCritical]
		private Int32 RegisterWindowClass() {
			NativeMethods.WNDCLASS wndClass = new NativeMethods.WNDCLASS();
			wndClass.cbClsExtra = 0;
			wndClass.cbWndExtra = 0;
			wndClass.hbrBackground = IntPtr.Zero;
			wndClass.hCursor = IntPtr.Zero;
			wndClass.hIcon = IntPtr.Zero;
			wndClass.lpfnWndProc = (ShadowWindow.sharedWndProc = new NativeMethods.WndProc(NativeMethods.DefWindowProc));
			wndClass.lpszClassName = "ShadowWindow";
			wndClass.lpszMenuName = null;
			wndClass.style = 0;
			Int32 res = NativeMethods.RegisterClass(ref wndClass);
			if(res == 0) {
				int errCode = Marshal.GetLastWin32Error();
				if(!isError) {
					isError = true;
					throw new Exception("could not register Window class error " + errCode.ToString());
				}
			}
			return res;
		}
		bool isVisible = false;
		public bool IsVisible {
			get { return isVisible; }
			set {
				if(IsVisible == value) return;
				isVisible = value;
				IsVisibleChanged();
			}
		}
		bool shouldUpdate = false;
		private void IsVisibleChanged() {
			shouldUpdate = true;
		}
		bool isActiveCore = true;
		public bool IsActive {
			get { return isActiveCore; }
			set {
				if(isActiveCore == value) return;
				isActiveCore = value;
				OnIsActiveChanged();
			}
		}
		private void OnIsActiveChanged() {
			if(IsVisible)
				RenderShadowWindow(false);
			else
				HideWnd();
		}
		protected Rectangle ShadowWindowRect { get; set; }
		public int Left { get { return ShadowWindowRect.Left; } }
		public int Top { get { return ShadowWindowRect.Top; } }
		public int Width { get { return ShadowWindowRect.Width; } }
		public int Height { get { return ShadowWindowRect.Height; } }
		public bool AllowResizeViaShadows { get; set; }
		public Rectangle TargetCtrlRect { get; set; }
		public void UpdateWindowPos(Rectangle targetCtrlRect) {
			TargetCtrlRect = targetCtrlRect;
			ShadowWindowRect = ShadowWindowLayoutCalculator.Calculate(windowType, targetCtrlRect, painter);
			if(!isTimerWork) 
				UpdateWindowPosCore();
		}
		public void CommitChanges(bool updateVisibility, bool delayWhileMove, bool allowResizeViaShadows) {
			AllowResizeViaShadows = allowResizeViaShadows;
			if(updateVisibility) UpdateWindowPosCore(true);
			if((IsVisible && bmpManager.CachedBmpSize != ShadowWindowRect.Size) || shouldUpdate)
				RenderShadowWindow(delayWhileMove);
		}
		public void RefreshWindow() {
			if(bmpManager != null)
				bmpManager.ReleaseCompositeBitmaps();
			RenderShadowWindow(false);
		}
		bool firstAppearance = true;
		private void UpdateWindowPosCore() {
			UpdateWindowPosCore(false);
		}
		private void UpdateWindowPosCore(bool showWindow) {
			int flags = NativeMethods.SWP.SWP_NOOWNERZORDER | NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOSIZE;
			if(bmpManager.CachedBmpSize != ShadowWindowRect.Size)
				flags |= NativeMethods.SWP.SWP_NOMOVE;
			if((IsVisible && bmpManager.CachedBmpSize != Size.Empty) || firstAppearance || showWindow) {
				flags |= NativeMethods.SWP.SWP_SHOWWINDOW;
				firstAppearance = false;
			}
			else
				flags |= NativeMethods.SWP.SWP_HIDEWINDOW;
			NativeMethods.SetWindowPos(base.Handle, IntPtr.Zero, Left, Top, 0, 0, flags);
		}
		public void HideWnd() {
			int flags = NativeMethods.SWP.SWP_NOOWNERZORDER | NativeMethods.SWP.SWP_NOACTIVATE | NativeMethods.SWP.SWP_NOZORDER | NativeMethods.SWP.SWP_NOSIZE | NativeMethods.SWP.SWP_NOMOVE | NativeMethods.SWP.SWP_HIDEWINDOW;
			NativeMethods.SetWindowPos(base.Handle, IntPtr.Zero, Left, Top, 0, 0, flags);
		}
		[System.Security.SecuritySafeCritical]
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
			Message m = new Message() { Msg = msg, LParam = lParam, WParam = wParam };
			NativeMethods.WINDOWPOS windowPos;
			switch(msg) {
				case MSG.WM_ACTIVATE:
					return IntPtr.Zero;
				case MSG.WM_WINDOWPOSCHANGING:
					windowPos = (NativeMethods.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.WINDOWPOS));
					windowPos.flags |= NativeMethods.SWP.SWP_NOACTIVATE;
					Marshal.StructureToPtr(windowPos, lParam, true);
					break;
				case FormShadowHelpers.NativeHelper.WM_DISPLAYCHANGE:
					if(IsVisible)
						RenderShadowWindow(false);
					break;
				case MSG.WM_NCLBUTTONDOWN:
				case MSG.WM_NCLBUTTONDBLCLK:
				case MSG.WM_NCRBUTTONDOWN:
				case MSG.WM_NCRBUTTONDBLCLK:
				case MSG.WM_NCMBUTTONDOWN:
				case MSG.WM_NCMBUTTONDBLCLK:
				case 171:
				case 173:
					IntPtr targetWindowHandle = this.ownerCore.Form.Handle;
					NativeMethods.SendMessage(targetWindowHandle, 6, new IntPtr(2), IntPtr.Zero);
					NativeMethods.SendMessage(targetWindowHandle, msg, wParam, IntPtr.Zero);
					return IntPtr.Zero;
				case MSG.WM_NCHITTEST:
					if(!AllowResizeViaShadows)
						return new IntPtr(NativeMethods.HT.HTTRANSPARENT);
					int hitTest = NCHitTestResize(hwnd, msg, wParam, lParam);
					return new IntPtr(hitTest);
			}
			return base.WndProc(hwnd, msg, wParam, lParam);
		}
		int SizingAreaSize { get { return 10; } }
		Rectangle GetLeftSizingArea() {
			return new Rectangle(TargetCtrlRect.X - SizingAreaSize, TargetCtrlRect.Y, SizingAreaSize, TargetCtrlRect.Height);
		}
		Rectangle GetRightSizingArea() {
			return new Rectangle(TargetCtrlRect.Right, TargetCtrlRect.Y, SizingAreaSize, TargetCtrlRect.Height);
		}
		Rectangle GetTopSizingArea() {
			return new Rectangle(TargetCtrlRect.X, TargetCtrlRect.Y - SizingAreaSize, TargetCtrlRect.Width, SizingAreaSize);
		}
		Rectangle GetBottomSizingArea() {
			return new Rectangle(TargetCtrlRect.X, TargetCtrlRect.Bottom, TargetCtrlRect.Width, SizingAreaSize);
		}
		Rectangle GetTopLeftSizingArea() {
			return new Rectangle(TargetCtrlRect.X - SizingAreaSize, TargetCtrlRect.Top - SizingAreaSize, SizingAreaSize, SizingAreaSize);
		}
		Rectangle GetTopRightSizingArea() {
			return new Rectangle(TargetCtrlRect.Right, TargetCtrlRect.Top - SizingAreaSize, SizingAreaSize, SizingAreaSize);
		}
		Rectangle GetBottomLeftSizingArea() {
			return new Rectangle(TargetCtrlRect.X - SizingAreaSize, TargetCtrlRect.Bottom, SizingAreaSize, SizingAreaSize);
		}
		Rectangle GetBottomRightSizingArea() {
			return new Rectangle(TargetCtrlRect.Right, TargetCtrlRect.Bottom, SizingAreaSize, SizingAreaSize);
		}
		protected virtual int NCHitTestResize(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
			Point pt = FormShadowHelpers.NativeHelper.GetPoint(lParam);
			if(GetLeftSizingArea().Contains(pt))
				return NativeMethods.HT.HTLEFT;
			if(GetTopLeftSizingArea().Contains(pt))
				return NativeMethods.HT.HTTOPLEFT;
			if(GetTopRightSizingArea().Contains(pt))
				return NativeMethods.HT.HTTOPRIGHT;
			if(GetTopSizingArea().Contains(pt))
				return NativeMethods.HT.HTTOP;
			if(GetRightSizingArea().Contains(pt))
				return NativeMethods.HT.HTRIGHT;
			if(GetBottomLeftSizingArea().Contains(pt))
				return NativeMethods.HT.HTBOTTOMLEFT;
			if(GetBottomRightSizingArea().Contains(pt))
				return NativeMethods.HT.HTBOTTOMRIGHT;
			if(GetBottomSizingArea().Contains(pt))
				return NativeMethods.HT.HTBOTTOM;
			return NativeMethods.HT.HTNOWHERE;
		}
		Timer moveTimerCore;
		bool isTimerWork;
		void moveTimerCore_Tick(object sender, EventArgs e) {
			moveTimerCore.Stop();
			isTimerWork = false;
			RenderShadowWindow(false);
		}
		void StartMoveTimer() {
			if(moveTimerCore != null)
				moveTimerCore.Stop();
			else {
				moveTimerCore = new Timer();
				moveTimerCore.Tick += moveTimerCore_Tick;
				moveTimerCore.Interval = fastMoveDelayDurationMs;
			}
			isTimerWork = true;
			moveTimerCore.Start();
		}
		protected virtual void OnOpacityChanged() {
			RenderShadowWindowCore();
		}
		protected internal void RenderShadowWindow(bool delayWhileMove) {
			if(delayWhileMove || isTimerWork) {
				HideWnd();
				StartMoveTimer();
				return;
			}
			RenderShadowWindowCore();
		}
		protected internal void RenderShadowWindowCore() {
			shouldUpdate = false;
			IntPtr hdcScreen = IntPtr.Zero;
			IntPtr hdcWindow = IntPtr.Zero;
			try {
				hdcScreen = NativeMethods.GetDC(IntPtr.Zero);
				hdcWindow = NativeMethods.CreateCompatibleDC(hdcScreen);
				Draw(hdcWindow);
				ExcludeRegion();
				NativeMethods.BLENDFUNCTION blend = new NativeMethods.BLENDFUNCTION();
				blend.BlendOp = 0;
				blend.BlendFlags = 0;
				blend.SourceConstantAlpha = Opacity;
				blend.AlphaFormat = 1;
				NativeMethods.POINT pointDest = new NativeMethods.POINT { X = Left, Y = Top };
				NativeMethods.SIZE size = new NativeMethods.SIZE { Width = Width, Height = Height };
				NativeMethods.POINT pointSrc = new NativeMethods.POINT { X = 0, Y = 0 };
				NativeMethods.UpdateLayeredWindow(base.Handle, hdcScreen, ref pointDest, ref size, hdcWindow, ref pointSrc, 0, ref blend, 2);
			}
			finally {
				NativeMethods.DeleteDC(hdcWindow);
				NativeMethods.ReleaseDC(IntPtr.Zero, hdcScreen);
			}
		}
		private void ExcludeRegion() {
			if(!ShouldExlcudeRegion) return;
			using(Region region = new Region(new Rectangle(new Point(0, 0), ShadowWindowRect.Size))) {
				Rectangle exRect = Rectangle.Empty;
				if(!AllowResizeViaShadows) {
					var leftBmp = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Left, TargetCtrlRect, painter);
					int offset = leftBmp.Width;
					int excludeWidth = ShadowWindowRect.Width - 2 * offset;
					int excludeHeight = ShadowWindowRect.Height - 2 * offset;
					exRect = new Rectangle(offset, offset, excludeWidth, excludeHeight);
				}
				else 
				exRect = new Rectangle(TargetCtrlRect.X - ShadowWindowRect.X, TargetCtrlRect.Y - ShadowWindowRect.Y, TargetCtrlRect.Width, TargetCtrlRect.Height);
				region.Exclude(exRect);
				using(Graphics g = Graphics.FromHwndInternal(Handle)) {
					var res = NativeMethods.SetWindowRgn(Handle, region.GetHrgn(g), false);
				}
			}
		}
		bool ShouldExlcudeRegion {
			get { return ShadowWindowRect.Size.Width != 0 && ShadowWindowRect.Size.Height != 0; }
		}
		void Draw(IntPtr windowDC) {
			if(windowType != ShadowWindowTypes.Composite) 
				return;
			Rectangle l = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Left, TargetCtrlRect, painter);
			Rectangle t = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Top, TargetCtrlRect, painter);
			Rectangle r = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Right, TargetCtrlRect, painter);
			Rectangle b = ShadowWindowLayoutCalculator.Calculate(ShadowWindowTypes.Bottom, TargetCtrlRect, painter);
			Bitmap bitmapTmp = GetBitmap(new CompositeBitmapAttributes(ShadowWindowRect.Size, l.Size, t.Size, r.Size, b.Size, painter, IsActive));
			if(bitmapTmp == null)
				return;
			IntPtr hBitmap = GetHBitmap(bitmapTmp);
			if(hBitmap == IntPtr.Zero)
				return;
			try {
				NativeMethods.SelectObject(windowDC, hBitmap);
			}
			finally { NativeMethods.DeleteObject(hBitmap); }
		}
		protected static IntPtr GetHBitmap(Bitmap bitmap) {
			IntPtr hBitmap = IntPtr.Zero;
			try {
				hBitmap = bitmap.GetHbitmap(Color.Empty);
			}
			catch { 
			}
			return hBitmap;
		}
		protected virtual Bitmap GetBitmap(CompositeBitmapAttributes attr) {
			if(bmpManager == null) return null;
			return bmpManager.GetCompositeBitmap(attr);
		}
		internal SkinShadowWindowPainter GetPainter() {
			return painter;
		}
	}
	public class XtraFormShadowWindow : ShadowWindow {
		public XtraFormShadowWindow(FormShadow owner, ShadowWindowTypes windowType) : base(owner, windowType) { }
		protected override SkinShadowWindowPainter CreatePainter() {
			return new XtraFormSkinShadowPainter(GetISkinProvider());
		}
	}
	public class ToolbarShadowWindow : ShadowWindow {
		public ToolbarShadowWindow(FormShadow owner, ShadowWindowTypes windowType) : base(owner, windowType) { }
		protected override SkinShadowWindowPainter CreatePainter() {
			return new ToolWindowSkinShadowPainter(GetISkinProvider());
		}
	}
	public class RibbonFormShadowWindow : ShadowWindow {
		public RibbonFormShadowWindow(FormShadow owner, ShadowWindowTypes windowType) : base(owner, windowType) { }
		protected override SkinShadowWindowPainter CreatePainter() {
			return new RibbonFormSkinShadowPainter(GetISkinProvider());
		}
	}
	public class BeakFormShadowWindow : ShadowWindow {
		public BeakFormShadowWindow(FormShadow owner, ShadowWindowTypes windowType) : base(owner, windowType) {
			isBeakVisible = true;
		}
		BeakFormAlignment beakFormAlignmentCore;
		public BeakFormAlignment BeakFormAlignment {
			get { return beakFormAlignmentCore; }
			set {
				if(BeakFormAlignment == value) return;
				beakFormAlignmentCore = value;
				if(IsVisible) RenderShadowWindow(false);
			}
		}
		int beakOffsetCore;
		public int BeakOffset {
			get { return beakOffsetCore; }
			set {
				if(BeakOffset == value) return;
				beakOffsetCore = value;
				if(IsVisible) RenderShadowWindow(false);
			}
		}
		bool isBeakVisible;
		public bool IsBeakVisible {
			get { return isBeakVisible; }
			set {
				if(IsBeakVisible == value) return;
				isBeakVisible = value;
				if(IsVisible) RenderShadowWindow(false);
			}
		}
		protected override SkinShadowWindowPainter CreatePainter() {
			return new BeakFormSkinShadowPainter(GetISkinProvider());
		}
		protected override Bitmap GetBitmap(CompositeBitmapAttributes attr) {
			if(bmpManager == null) return null;
			return bmpManager.GetCompositeBitmapWithBeak(attr, beakFormAlignmentCore, beakOffsetCore, IsBeakVisible);
		}
		protected override BitmapManager CreateBitmapManager() {
			return base.CreateBitmapManager();
		}
	}
}
