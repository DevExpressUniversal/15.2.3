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
using System.Windows.Media;
using System.Security;
using System.Reflection;
#if DXWINDOW
using DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers;
using DecoratorNativeMethods = DevExpress.Internal.DXWindow.Core.HandleDecorator.Helpers.NativeMethods;
using DevExpress.Internal.DXWindow.Data;
#else
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Xpf.Core.HandleDecorator.Helpers;
using DecoratorNativeMethods = DevExpress.Xpf.Core.HandleDecorator.Helpers.NativeMethods;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Core.HandleDecorator {
#else
namespace DevExpress.Xpf.Core.HandleDecorator {
#endif
	public enum HandleDecoratorWindowTypes {
		Left, Top, Right, Bottom, Composite
	}
	public class HandleDecoratorWindowLayoutCalculator {
		static Size defaultImageSize = new Size(5, 5);
		public static Size GetImageSize(ThemeElementImage element) {
			if(element == null || element.Image == null)
				return defaultImageSize;
			return element.GetImageBounds(0).Size;
		}
		static int GetDecoratorThick(ThemeElementImage elementImage, HandleDecoratorWindowTypes windowType) {
			return IsHorizontalWindowType(windowType) ? GetImageSize(elementImage).Width : GetImageSize(elementImage).Height;
		}
		static bool IsHorizontalWindowType(HandleDecoratorWindowTypes windowType) {
			return windowType == HandleDecoratorWindowTypes.Left || windowType == HandleDecoratorWindowTypes.Right;
		}
		public static Rectangle Calculate(HandleDecoratorWindowTypes windowType, Rectangle windowRectagle, ThemeElementPainter painter) {
			if(painter == null) return Rectangle.Empty;
			Rectangle result = windowRectagle;
			int decoratorThick = (int)(GetDecoratorThick(painter.GetElementImageByWindowType(windowType), windowType) * painter.ScaleFactor);
			int offset = painter.GetOffsetByWindowType(windowType);
			switch(windowType) {
				case HandleDecoratorWindowTypes.Left:
					result.X = result.X - decoratorThick + offset;
					result.Y = result.Y - decoratorThick + offset;
					result.Height += (decoratorThick - offset) * 2;
					result.Width = decoratorThick;
					return result;
				case HandleDecoratorWindowTypes.Top:
					result.X = result.X + offset;
					result.Y = result.Y - decoratorThick + offset;
					result.Height = decoratorThick;
					result.Width = result.Width - 2 * offset;
					return result;
				case HandleDecoratorWindowTypes.Right:
					result.X = result.Right - offset;
					result.Y = result.Y - decoratorThick + offset;
					result.Height += (decoratorThick - offset) * 2;
					result.Width = decoratorThick;
					return result;
				case HandleDecoratorWindowTypes.Bottom:
					result.X = result.X + offset;
					result.Y = result.Bottom - offset;
					result.Height = decoratorThick;
					result.Width = result.Width - 2 * offset;
					return result;
				case HandleDecoratorWindowTypes.Composite:
					Rectangle rectL = Calculate(HandleDecoratorWindowTypes.Left, windowRectagle, painter);
					Rectangle rectR = Calculate(HandleDecoratorWindowTypes.Right, windowRectagle, painter);
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
		public CompositeBitmapAttributes(Size compositeBmp, Size leftBorder, Size topBorder, Size rightBorder, Size bottomBorder, ThemeElementPainter painter, bool isActive) {
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
		public ThemeElementPainter SkinPainter { get; set; }
		public bool IsActive { get; set; }
	}
	public class BitmapManager : DisposableObject {
		HandleDecoratorWindowTypes windowTypeCore;
		public BitmapManager(HandleDecoratorWindowTypes windowType) {
			windowTypeCore = windowType;
		}
		Bitmap cachedBitmapCore = null;
		internal Bitmap CachedBitmap {
			get { return cachedBitmapCore; }
			set {
				if(cachedBitmapCore != value && cachedBitmapCore != null) cachedBitmapCore.Dispose();
				cachedBitmapCore = value;
			}
		}
		public Size CachedBmpSize {
			get {
				if(CachedBitmap == null) return Size.Empty;
				return CachedBitmap.Size;
			}
		}
		Bitmap cachedBeakBitmapCore = null;
		Bitmap CachedBeakBitmap {
			get { return cachedBeakBitmapCore; }
			set {
				if(cachedBeakBitmapCore != value && cachedBeakBitmapCore != null) cachedBeakBitmapCore.Dispose();
				cachedBeakBitmapCore = value;
			}
		}
		bool activeCore = true;
		public Bitmap GetCompositeBitmap(CompositeBitmapAttributes compositeAttr) {
			if(compositeAttr.CompositeBmpSize.Width == 0 || compositeAttr.CompositeBmpSize.Height == 0) return null;
			if(CachedBitmap != null && CachedBitmap.Size == compositeAttr.CompositeBmpSize && activeCore == compositeAttr.IsActive) return CachedBitmap;
			activeCore = compositeAttr.IsActive;
			int minHeight = compositeAttr.TopBorderSize.Height + compositeAttr.BottomBorderSize.Height;
			int minWidth = compositeAttr.LeftBorderSize.Width + compositeAttr.RightBorderSize.Width;
			if(compositeAttr.CompositeBmpSize.Width < minWidth || compositeAttr.CompositeBmpSize.Height < minHeight) {
				CachedBitmap = null;
				return null;
			}
			var leftBmp = CreateBorderBitmap(compositeAttr.LeftBorderSize, HandleDecoratorWindowTypes.Left, compositeAttr.SkinPainter, compositeAttr.IsActive);
			var topBmp = CreateBorderBitmap(compositeAttr.TopBorderSize, HandleDecoratorWindowTypes.Top, compositeAttr.SkinPainter, compositeAttr.IsActive);
			var rightBmp = CreateBorderBitmap(compositeAttr.RightBorderSize, HandleDecoratorWindowTypes.Right, compositeAttr.SkinPainter, compositeAttr.IsActive);
			var bottomBmp = CreateBorderBitmap(compositeAttr.BottomBorderSize, HandleDecoratorWindowTypes.Bottom, compositeAttr.SkinPainter, compositeAttr.IsActive);
			try {
				Bitmap result = new Bitmap(compositeAttr.CompositeBmpSize.Width, compositeAttr.CompositeBmpSize.Height);
				using(Graphics g = Graphics.FromImage(result)) {
					g.DrawImageUnscaled(leftBmp, new Point(0, 0));
					g.DrawImageUnscaled(topBmp, new Point(compositeAttr.LeftBorderSize.Width, 0));
					g.DrawImageUnscaled(rightBmp, new Point(compositeAttr.LeftBorderSize.Width + compositeAttr.TopBorderSize.Width, 0));
					g.DrawImageUnscaled(bottomBmp, new Point(compositeAttr.LeftBorderSize.Width, compositeAttr.CompositeBmpSize.Height - compositeAttr.BottomBorderSize.Height));
				}
				CachedBitmap = result;
				return result;
			}
			catch(Exception e) {
				System.Diagnostics.Debug.WriteLine("Composite bitmap creation failed: " + e.Message);
				CachedBitmap = null;
				return null;
			}
		}
		Bitmap CreateBorderBitmap(Size imageSize, HandleDecoratorWindowTypes type, ThemeElementPainter painter, bool isActive) {
			try {
				Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
				using(Graphics g = Graphics.FromImage(result)) {
					ThemeElementInfo info = new ThemeElementInfo();
					info.WindowType = type;
					info.Active = isActive;
					info.Bounds = new Rectangle(0, 0, imageSize.Width, imageSize.Height);
					info.ImageIndex = info.Active ? 0 : 1;
					painter.DrawObject(info, g);
					info = null;
				}
				return result;
			}
			catch(Exception e) {
				System.Diagnostics.Debug.WriteLine(type.ToString() + " border bitmap creation failed: " + e.Message);
				return new Bitmap(1, 1);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				CachedBitmap = null;
			}
			base.Dispose(disposing);
		}
	}
	[ComVisible(true)]
	public abstract class HwndWrapper : DisposableObject {
		private IntPtr handle;
		private bool isCreatingHandle;
		bool cantCreateWindow;
		private int wndClassAtom;
		private Delegate wndProc;
		protected int WindowClassAtom {
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
		protected abstract int CreateWindowClassCore();
		protected abstract void DestroyWindowClassCore();
		[System.Security.SecuritySafeCritical]
		void SubclassWndProc() {
			wndProc = new DecoratorNativeMethods.WndProc(WndProc);
			IntPtr pWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);
			DecoratorNativeMethods.SetWindowLong(handle, NativeHelper.GWL_WNDPROC, pWndProc);
		}
		protected abstract IntPtr CreateWindowCore();
		protected virtual void DestroyWindowCore() {
			if(handle != IntPtr.Zero) {
				DecoratorNativeMethods.DestroyWindow(handle);
				handle = IntPtr.Zero;
			}
		}
		protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
			return DecoratorNativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
		}
		public void EnsureHandle() {
			if(cantCreateWindow) return;
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
				catch {
					handle = IntPtr.Zero;
					cantCreateWindow = true;
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
	public abstract class HandleDecoratorWindow : HwndWrapper {
		const int fastMoveDelayDurationMs = 150;
		Decorator ownerCore;
		internal protected BitmapManager bmpManager;
		HandleDecoratorWindowTypes windowType;
		protected ThemeElementPainter painter;
		public HandleDecoratorWindow(Decorator owner, HandleDecoratorWindowTypes windowType, bool startupActiveState) {
			ownerCore = owner;
			isActiveCore = startupActiveState;
			WindowType = windowType;
			bmpManager = CreateBitmapManager();
			painter = CreatePainter(ownerCore);
		}
		ThemeElementPainter CreatePainter(Decorator owner) {
			return new ThemeElementPainter(owner);
		}
		protected virtual BitmapManager CreateBitmapManager() { return new BitmapManager(windowType); }
		public HandleDecoratorWindowTypes WindowType {
			get { return windowType; }
			set { windowType = value; }
		}
		protected override bool IsWindowSubclassed {
			get { return true; }
		}
		protected override IntPtr CreateWindowCore() {
			IntPtr owner = DecoratorNativeMethods.GetWindow(NativeHelper.GetHandle(ownerCore.Control), DecoratorNativeMethods.GW_OWNER);
			return DecoratorNativeMethods.CreateWindowEx(
						NativeHelper.WS_EX_LAYERED | NativeHelper.WS_EX_TOOLWINDOW | NativeHelper.WS_EX_NOACTIVATE | NativeHelper.WS_EX_TRANSPARENT,
						new IntPtr(base.WindowClassAtom),
						string.Empty,
						-(NativeHelper.WS_VISIBLE | NativeHelper.WS_MINIMIZE | NativeHelper.WS_CHILDWINDOW | NativeHelper.WS_CLIPCHILDREN | NativeHelper.WS_DISABLED),
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
			if(sharedWindowClassAtom != 0 && DecoratorNativeMethods.UnregisterClass(new IntPtr(base.WindowClassAtom), IntPtr.Zero))
				sharedWindowClassAtom = 0;
		}
		protected override int CreateWindowClassCore() {
			return SharedWindowClassAtom;
		}
		static int sharedWindowClassAtom;
		static DecoratorNativeMethods.WndProc sharedWndProc;
		int SharedWindowClassAtom {
			get {
				if(sharedWindowClassAtom == 0)
					sharedWindowClassAtom = RegisterWindowClass();
				return sharedWindowClassAtom;
			}
		}
		const string className = "HwndDecoratorWindow";
		[SecuritySafeCritical]
		private int RegisterWindowClass() {
			DecoratorNativeMethods.WNDCLASS wndClass = new DecoratorNativeMethods.WNDCLASS();
			wndClass.cbClsExtra = 0;
			wndClass.cbWndExtra = 0;
			wndClass.hbrBackground = IntPtr.Zero;
			wndClass.hCursor = IntPtr.Zero;
			wndClass.hIcon = IntPtr.Zero;
			wndClass.lpfnWndProc = (HandleDecoratorWindow.sharedWndProc = new DecoratorNativeMethods.WndProc(DecoratorNativeMethods.DefWindowProc));
			wndClass.lpszClassName = className;
			wndClass.lpszMenuName = null;
			wndClass.style = 0;
			DecoratorNativeMethods.WNDCLASS wndCheck = new DecoratorNativeMethods.WNDCLASS();
			IntPtr hInstance = IntPtr.Zero;
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if(entryAssembly != null) {
				Module[] modules = entryAssembly.GetModules();
				if(modules.Length > 0)
					hInstance = Marshal.GetHINSTANCE(Assembly.GetEntryAssembly().GetModules()[0]);
			}
			int res = DecoratorNativeMethods.GetClassInfo(hInstance, className, ref wndCheck);
			if(res == 0) {
				res = DecoratorNativeMethods.RegisterClass(ref wndClass);
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
		bool isActiveCore = false;
		public bool IsActive {
			get { return isActiveCore; }
			set {
				if(isActiveCore == value) return;
				isActiveCore = value;
				if(IsVisible) RenderDecoratorWindow(false);
			}
		}
		protected Rectangle DecoratorWindowRect { get; set; }
		public int Left { get { return DecoratorWindowRect.Left; } }
		public int Top { get { return DecoratorWindowRect.Top; } }
		public int Width { get { return DecoratorWindowRect.Width; } }
		public int Height { get { return DecoratorWindowRect.Height; } }
		protected Rectangle TargetCtrlRect { get; set; }
		public void UpdateWindowPos(Rectangle targetCtrlRect) {
			TargetCtrlRect = targetCtrlRect;
			painter.CalculateAndSetScaleFactor(targetCtrlRect.Size);
			DecoratorWindowRect = HandleDecoratorWindowLayoutCalculator.Calculate(windowType, targetCtrlRect, painter);
			if(!isTimerWork)
				UpdateWindowPosCore();
		}
		public void CommitChanges(bool updateVisibility, bool delayWhileMove) {
			if(updateVisibility) UpdateWindowPosCore(true);
			if(IsVisible && (bmpManager.CachedBmpSize != DecoratorWindowRect.Size || shouldUpdate))
				RenderDecoratorWindow(delayWhileMove);
		}
		bool firstAppearance = true;
		private void UpdateWindowPosCore() {
			UpdateWindowPosCore(false);
		}
		private void UpdateWindowPosCore(bool showWindow) {
			int flags = DecoratorNativeMethods.SWP.SWP_NOOWNERZORDER | DecoratorNativeMethods.SWP.SWP_NOACTIVATE | DecoratorNativeMethods.SWP.SWP_NOZORDER | DecoratorNativeMethods.SWP.SWP_NOSIZE;
			if(bmpManager.CachedBmpSize != DecoratorWindowRect.Size)
				flags |= DecoratorNativeMethods.SWP.SWP_NOMOVE;
			if((IsVisible && bmpManager.CachedBmpSize != Size.Empty) || firstAppearance || showWindow) {
				flags |= DecoratorNativeMethods.SWP.SWP_SHOWWINDOW;
				firstAppearance = false;
			} else
				flags |= DecoratorNativeMethods.SWP.SWP_HIDEWINDOW;
			DecoratorNativeMethods.SetWindowPos(base.Handle, IntPtr.Zero, Left, Top, 0, 0, flags);
		}
		public void HideWnd() {
			int flags = DecoratorNativeMethods.SWP.SWP_NOOWNERZORDER | DecoratorNativeMethods.SWP.SWP_NOACTIVATE | DecoratorNativeMethods.SWP.SWP_NOZORDER | DecoratorNativeMethods.SWP.SWP_NOSIZE | DecoratorNativeMethods.SWP.SWP_NOMOVE | DecoratorNativeMethods.SWP.SWP_HIDEWINDOW;
			DecoratorNativeMethods.SetWindowPos(base.Handle, IntPtr.Zero, Left, Top, 0, 0, flags);
		}
		[System.Security.SecuritySafeCritical]
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) {
			switch(msg) {
				case MSG.WM_ACTIVATE: {
						return IntPtr.Zero;
					}
				case MSG.WM_WINDOWPOSCHANGING: {
						DecoratorNativeMethods.WINDOWPOS windowPos = (DecoratorNativeMethods.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(DecoratorNativeMethods.WINDOWPOS));
						windowPos.flags |= DecoratorNativeMethods.SWP.SWP_NOACTIVATE;
						Marshal.StructureToPtr(windowPos, lParam, true);
						break;
					}
				case NativeHelper.WM_DISPLAYCHANGE: {
						if(IsVisible)
							RenderDecoratorWindow(false);
						break;
					}
				case MSG.WM_NCHITTEST: {
						return new IntPtr(DecoratorNativeMethods.HT.HTTRANSPARENT);
					}
			}
			return base.WndProc(hwnd, msg, wParam, lParam);
		}
		Timer moveTimerCore;
		bool isTimerWork;
		void moveTimerCore_Tick(object sender, EventArgs e) {
			moveTimerCore.Stop();
			isTimerWork = false;
			RenderDecoratorWindow(false);
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
		internal void RenderDecoratorWindow(bool delayWhileMove) {
			if(delayWhileMove || isTimerWork) {
				HideWnd();
				StartMoveTimer();
				return;
			}
			shouldUpdate = false;
			IntPtr hdcScreen = DecoratorNativeMethods.GetDC(IntPtr.Zero);
			if(hdcScreen == IntPtr.Zero) {
				return;
			}
			IntPtr hdcWindow = DecoratorNativeMethods.CreateCompatibleDC(hdcScreen);
			if(hdcWindow == IntPtr.Zero) {
				return;
			}
			var blend = new DecoratorNativeMethods.BLENDFUNCTION();
			blend.BlendOp = 0;
			blend.BlendFlags = 0;
			blend.SourceConstantAlpha = 255;
			blend.AlphaFormat = 1;
			Draw(hdcWindow);
			UpdateWindowPos(TargetCtrlRect);
			DecoratorNativeMethods.POINT pointDest = new DecoratorNativeMethods.POINT {
				X = Left,
				Y = Top
			};
			DecoratorNativeMethods.SIZE size = new DecoratorNativeMethods.SIZE {
				Width = Width,
				Height = Height
			};
			DecoratorNativeMethods.POINT pointSrc = new DecoratorNativeMethods.POINT {
				X = 0,
				Y = 0
			};
			ExcludeRegion();
			DecoratorNativeMethods.UpdateLayeredWindow(base.Handle, hdcScreen, ref pointDest, ref size, hdcWindow, ref pointSrc, 0, ref blend, 2);
			DecoratorNativeMethods.DeleteObject(hdcWindow);
			DecoratorNativeMethods.DeleteObject(hdcScreen);
		}
		private void ExcludeRegion() {
			if(!ShouldExlcudeRegion) return;
			using(Region region = new Region(new Rectangle(new Point(0, 0), DecoratorWindowRect.Size))) {
				var leftBmp = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Left, TargetCtrlRect, painter);
				int offset = leftBmp.Width;
				int excludeWidth = DecoratorWindowRect.Width - 2 * offset;
				int excludeHeight = DecoratorWindowRect.Height - 2 * offset;
				Rectangle exRect = new Rectangle(offset, offset, excludeWidth, excludeHeight);
				region.Exclude(exRect);
				using(Graphics g = Graphics.FromHwndInternal(Handle)) {
					var res = DecoratorNativeMethods.SetWindowRgn(Handle, region.GetHrgn(g), true);
				}
			}
		}
		bool ShouldExlcudeRegion { get { return DecoratorWindowRect.Size.Width != 0 && DecoratorWindowRect.Size.Height != 0; } }
		void Draw(IntPtr windowDC) {
			if(windowType != HandleDecoratorWindowTypes.Composite) 
				return;
			Bitmap bitmapTmp = null;
			var leftBmp = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Left, TargetCtrlRect, painter);
			var topBmp = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Top, TargetCtrlRect, painter);
			var rightBmp = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Right, TargetCtrlRect, painter);
			var bottomBmp = HandleDecoratorWindowLayoutCalculator.Calculate(HandleDecoratorWindowTypes.Bottom, TargetCtrlRect, painter);
			bitmapTmp = GetBitmap(new CompositeBitmapAttributes(DecoratorWindowRect.Size, leftBmp.Size, topBmp.Size, rightBmp.Size, bottomBmp.Size, painter, IsActive));
			if(bitmapTmp == null)
				return;
			IntPtr pBitmap = IntPtr.Zero;
			try {
				pBitmap = bitmapTmp.GetHbitmap(System.Drawing.Color.Empty);
				DecoratorNativeMethods.SelectObject(windowDC, pBitmap);
			}
			catch(Exception e) {
				System.Diagnostics.Debug.WriteLine("GetHbitmap failed: " + e.Message);
			}
			finally {
				DecoratorNativeMethods.DeleteObject(pBitmap);
				bitmapTmp = null;
			}
		}
		protected virtual Bitmap GetBitmap(CompositeBitmapAttributes attr) {
			if(bmpManager == null) return null;
			return bmpManager.GetCompositeBitmap(attr);
		}
		internal ThemeElementPainter GetPainter() {
			if(painter == null)
				painter = CreatePainter(ownerCore);
			return painter;
		}
	}
	public class FormDecoratorWindow : HandleDecoratorWindow {
		public FormDecoratorWindow(Decorator owner, HandleDecoratorWindowTypes windowType, bool startupActive) : base(owner, windowType, startupActive) { }
	}
}
