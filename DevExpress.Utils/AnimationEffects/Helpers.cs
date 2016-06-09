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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils {
	public enum RoundMode { Up, Down }
	internal static class Ref {
		[System.Diagnostics.DebuggerStepThrough]
		public static void Dispose<T>(ref T refToDispose)
			where T : class, IDisposable {
			DisposeCore(refToDispose);
			refToDispose = null;
		}
		static void DisposeCore(IDisposable refToDispose) {
			if(refToDispose != null) refToDispose.Dispose();
		}
	}
	internal sealed class DisposableToken : IDisposable {
		Action onDisposing;
		public DisposableToken(Action onDisposing) {
			this.onDisposing = onDisposing;
		}
		void IDisposable.Dispose() {
			if(onDisposing != null)
				onDisposing();
			onDisposing = null;
			GC.SuppressFinalize(this);
		}
	}
	internal sealed class DisposableObjectsContainer : IDisposable {
		List<IDisposable> disposableObjects;
		public DisposableObjectsContainer() {
			disposableObjects = new List<IDisposable>(8);
		}
		void IDisposable.Dispose() {
			OnDisposing();
			GC.SuppressFinalize(this);
		}
		void OnDisposing() {
			foreach(IDisposable disposable in disposableObjects)
				disposable.Dispose();
			disposableObjects.Clear();
		}
		[System.Diagnostics.DebuggerStepThrough]
		public T Register<T>(T obj) where T : IDisposable {
			if(!object.Equals(obj, null) && !disposableObjects.Contains(obj))
				disposableObjects.Add(obj);
			return obj;
		}
	}
	public static class LoadingImages {
		[ThreadStatic]
		static Image imageCore, imageLineCore, imageBigCore;
		public static Image Image {
			get {
				if(imageCore == null)
					imageCore = CreateImage("loading.gif");
				return imageCore;
			}
		}
		public static Image ImageBig {
			get {
				if(imageBigCore == null)
					imageBigCore = CreateImage("loadingBig.gif");
				return imageBigCore;
			}
		}
		public static Image ImageLine {
			get {
				if(imageLineCore == null)
					imageLineCore = CreateImage("loadingLine.gif");
				return imageLineCore;
			}
		}
		static Image CreateImage(string imageName) {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.Utils.Images." + imageName,
				typeof(LoadingImages).Assembly);
		}
		static object syncLock = new object();
		static Image GetImageCore(ISkinProvider provider) {
			if(provider != null) {
				SkinElement element = CommonSkins.GetSkin(provider)[CommonSkins.SkinLoadingBig];
				if(element != null && element.Image != null && element.Image.Image != null)
					return element.Image.Image;
			}
			return LoadingImages.ImageBig;
		}
		public static Image GetImage(ISkinProvider provider, bool clone) {
			lock(syncLock) {
				Image image = GetImageCore(provider);
				if(!clone) return image;
				if(image == null) return null;
				return image.Clone() as Image;
			}
		}
		public static Image GetImage(ISkinProvider provider) { return GetImage(provider, false); }
	}
	public static class PlacementHelper {
		public static Rectangle Arrange(Size size, Rectangle targetRect, ContentAlignment alignment) {
			double left = GetLeft(size.Width, targetRect, alignment);
			double top = GetTop(size.Height, targetRect, alignment);
			return new Rectangle(new Point(Round(left), Round(top)), size);
		}
		public static Rectangle Arrange(Size size, Rectangle targetRect, ContentAlignment alignment, RoundMode mode) {
			double left = GetLeft(size.Width, targetRect, alignment);
			double top = GetTop(size.Height, targetRect, alignment);
			return new Rectangle(new Point(Round(left, mode), Round(top, mode)), size);
		}
		static int Round(double d) {
			return d > 0 ? (int)(d + 0.5d) : (int)(d - 0.5d);
		}
		static int Round(double d, RoundMode mode) {
			if(Math.Ceiling(d) == Math.Floor(d))
				return (int)d;
			double delta = mode == RoundMode.Down ? -0.5d : 0.5d;
			return (int)(d + delta);
		}
		static double CenterRange(double targetStart, double targetRange, double range) {
			return targetStart + (targetRange - range) * 0.5;
		}
		static Size GetSize(Rectangle targetRect, ContentAlignment alignment) {
			double width = targetRect.Width;
			double height = targetRect.Height;
			switch(alignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.BottomCenter:
					height *= 0.5d;
					break;
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
					width *= 0.5d;
					break;
			}
			return new Size(Round(width), Round(height));
		}
		static double GetLeft(double width, Rectangle targetRect, ContentAlignment alignment) {
			double left = targetRect.Left;
			switch(alignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					left = CenterRange(targetRect.Left, targetRect.Width, width);
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					left = targetRect.Right - width;
					break;
			}
			return left;
		}
		static double GetTop(double height, Rectangle targetRect, ContentAlignment alignment) {
			double top = targetRect.Top;
			switch(alignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					top = CenterRange(targetRect.Top, targetRect.Height, height);
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					top = targetRect.Bottom - height;
					break;
			}
			return top;
		}
	}
	static class CutImageHelper {
		public static Bitmap CropImage(Bitmap source, Rectangle region) {
			if(source == null || region.IsEmpty || region.Width <= 0 || region.Height <= 0) return (Bitmap)source.Clone();
			Bitmap bmp = new Bitmap(region.Width, region.Height);
			using(Graphics g = Graphics.FromImage(bmp))
				g.DrawImage(source, 0, 0, region, GraphicsUnit.Pixel);
			return bmp;
		}
		public static Bitmap[] CutImage(Bitmap source, int countPart, Rectangle bounds, bool vertical) {
			if(countPart <= 1) return new Bitmap[] { (Bitmap)source.Clone() };
			if(vertical) return CutImageVertical(source, countPart, bounds);
			return CutImageHorizontal(source, countPart, bounds);
		}
		static Bitmap[] CutImageHorizontal(Bitmap source, int countPart, Rectangle bounds) {
			List<Bitmap> partsImage = new List<Bitmap>();
			for(int i = 0; i < countPart; i++) {
				int step = (int)Math.Ceiling((double)bounds.Height / countPart);
				Bitmap bmp = CropImage(source, new Rectangle(bounds.X, bounds.Top + step * i, bounds.Width, step));
				partsImage.Add(bmp);
			}
			return partsImage.ToArray();
		}
		static Bitmap[] CutImageVertical(Bitmap source, int countPart, Rectangle bounds) {
			List<Bitmap> partsImage = new List<Bitmap>();
			for(int i = 0; i < countPart; i++) {
				int step = (int)Math.Ceiling((double)bounds.Width / countPart);
				Bitmap bmp = CropImage(source, new Rectangle(bounds.X + step * i, bounds.Top, step, bounds.Height));
				partsImage.Add(bmp);
			}
			return partsImage.ToArray();
		}
	}
	public static class ScreenCaptureHelper {
		private const int msgWM_PRINT = 0x0317;
		public static Rectangle CalcBoundsWindow(IntPtr handle) {
			NativeMethods.RECT rect = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(handle, ref rect);
			return rect.ToRectangle();
		}
		public static Bitmap ResizeImageWithAspect(Bitmap originalBitmap, int newWidth, int maxHeight, bool resizeIfWider) {
			try {
				if(resizeIfWider && originalBitmap.Width <= newWidth)
					newWidth = originalBitmap.Width;
				int newHeight = originalBitmap.Height * newWidth / originalBitmap.Width;
				if(newHeight > maxHeight) {
					newWidth = originalBitmap.Width * maxHeight / originalBitmap.Height;
					newHeight = maxHeight;
				}
				return (Bitmap)originalBitmap.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
			}
			finally {
			}
		}
		public static bool IsActiveXControl(Control control) { return control is WebBrowserBase || control is AxHost; }
		public static Bitmap GetImageFromActiveXControl(IntPtr handle, Size size) {
			IntPtr dc = NativeMethods.GetDC(handle);
			IntPtr memoryDC = NativeMethods.CreateCompatibleDC(dc);
			IntPtr memoryBitmap = NativeMethods.CreateCompatibleBitmap(dc, size.Width, size.Height);
			try {
				NativeMethods.SelectObject(memoryDC, memoryBitmap);
				NativeMethods.BitBlt(memoryDC, 0, 0, size.Width, size.Height, dc, 0, 0, (int)(CopyPixelOperation.SourceCopy | CopyPixelOperation.SourcePaint));
				return Image.FromHbitmap(memoryBitmap);
			}
			finally {
				NativeMethods.DeleteObject(memoryBitmap);
				NativeMethods.DeleteDC(memoryDC);
				NativeMethods.ReleaseDC(handle, dc);
			}
		}
		public static Bitmap GetImageFromControl(IntPtr handle, Size size) {
			Bitmap bmp = new Bitmap(size.Width, size.Height);
			Control control = Control.FromHandle(handle);
			if(control != null) control.DrawToBitmap(bmp, new Rectangle(0, 0, size.Width, size.Height));
			return bmp;
		}
		public static Point GetClientLocation(IntPtr hwnd) {
			var c = new NativeMethods.POINT(0, 0);
			NativeMethods.ClientToScreen(hwnd, ref c);
			var r = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(hwnd, ref r);
			return new Point(c.X - r.Left, c.Y - r.Top);
		}
		public static Image GetRecursiveImageFromControl(Control ctl) {
			if(ctl == null || ctl.Size.Width <= 0 || ctl.Size.Height <= 0)
				return null;
			Image image = (Image)new Bitmap(ctl.Size.Width, ctl.Size.Height);
			using(Graphics gr = Graphics.FromImage(image))
				PrintRecursiveControl(ctl, gr);
			return image;
		}
		static void PrintActiveXControl(IntPtr handle, IntPtr hdc, Rectangle rect) {
			IntPtr dc = NativeMethods.GetDC(handle);
			try {
				NativeMethods.BitBlt(hdc, 0, 0, rect.Width, rect.Height, dc, 0, 0, (int)(CopyPixelOperation.SourceCopy));
			}
			finally {
				NativeMethods.ReleaseDC(handle, dc);
			}
		}
		static IntPtr GetClipRgn(IntPtr hdc) {
			IntPtr clip = NativeMethods.CreateRectRgn(0, 0, 0, 0);
			if(NativeMethods.GetClipRgn(hdc, clip) != 1) {
				NativeMethods.DeleteObject(clip);
				clip = IntPtr.Zero;
			}
			return clip;
		}
		static void PrintControl(Control control, IntPtr hdc, IntPtr memoryDC, Rectangle rect, Rectangle clip) {
			NativeMethods.POINT point;
			NativeMethods.SetViewportOrgEx(memoryDC, rect.X, rect.Y, out point);
			try {
				if(IsActiveXControl(control))
					PrintActiveXControl(control.Handle, memoryDC, rect);
				else {
					NativeMethods.PrintOptions drawOptions =
						NativeMethods.PrintOptions.PRF_CLIENT |
						NativeMethods.PrintOptions.PRF_NONCLIENT | NativeMethods.PrintOptions.PRF_ERASEBKGND;
					NativeMethods.SendMessage(control.Handle, msgWM_PRINT, (int)memoryDC, (IntPtr)drawOptions);
				}
			}
			finally {
				NativeMethods.SetViewportOrgEx(memoryDC, point.X, point.Y, out point);
				NativeMethods.BitBlt(hdc, clip.X, clip.Y, clip.Width, clip.Height, memoryDC, clip.X, clip.Y, (int)(CopyPixelOperation.SourceCopy));
			}
		}
		static void PrintRecursiveControl(Control control, IntPtr hdc, IntPtr memoryDC, Rectangle rect, Rectangle clip) {
			bool doubleBufferEnabled = EnableDoubleBuffer(control, false);
			try {
				PrintControl(control, hdc, memoryDC, rect, clip);
				if(IsActiveXControl(control)) return;
				Point clientOffset = GetClientLocation(control.Handle);
				rect.Offset(clientOffset);
				foreach(Control child in GetChildControlsWithCorrectZOrder(control)) {
					if(!control.Visible || !child.Visible) continue;
					Rectangle childRect = child.Bounds;
					childRect.Offset(rect.Location);
					Rectangle clipRect = Rectangle.Intersect(clip, childRect);
					if(clip.IsEmpty) continue;
					PrintRecursiveControl(child, hdc, memoryDC, childRect, clipRect);
				}
			}
			finally {
				EnableDoubleBuffer(control, doubleBufferEnabled);
			}
		}
		static IList GetChildControlsWithCorrectZOrder(Control control) {
			ArrayList controls = new ArrayList(control.Controls);
			controls.Reverse();
			return controls;
		}
		public static void PrintRecursiveControl(Control control, Graphics gr) {
			if(control == null || control.IsDisposed || !control.Visible)
				return;
			Point offset = GetClientLocation(control.Handle);
			Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
			bounds.Offset(-offset.X, -offset.Y);
			PaintEventArgs pevent = new PaintEventArgs(gr, bounds);
			Rectangle clipBounds = pevent.ClipRectangle;
			IntPtr hdc = gr.GetHdc();
			IntPtr memoryDC = NativeMethods.CreateCompatibleDC(hdc);
			IntPtr memoryBitmap = NativeMethods.CreateCompatibleBitmap(hdc, clipBounds.Width, clipBounds.Height);
			IntPtr oldSelect = IntPtr.Zero;
			try {
				oldSelect = NativeMethods.SelectObject(memoryDC, memoryBitmap);
				PrintRecursiveControl(control, hdc, memoryDC, clipBounds, clipBounds);
			}
			finally {
				NativeMethods.SelectObject(memoryDC, oldSelect);
				NativeMethods.DeleteObject(memoryBitmap);
				NativeMethods.DeleteDC(memoryDC);
				gr.ReleaseHdc(hdc);
			}
		}
		static bool EnableControlStyle(Control control, bool enable, int style) {
			bool wasEnabled = (bool)InvokeMethod(control, "GetStyle", new object[] { style });
			if(wasEnabled != enable)
				InvokeMethod(control, "SetStyle", new object[] { style, enable });
			return wasEnabled;
		}
		static bool EnableDoubleBuffer(Control control, bool enable) {
			return EnableControlStyle(control, enable, (int)ControlConstants.DoubleBuffer);
		}
		static object InvokeMethod(Control control, string method, object[] args) {
			MethodInfo mi = control.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null)
				return mi.Invoke(control, args);
			return null;
		}
	}
}
