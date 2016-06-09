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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting.Native;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Windows.Size;
namespace DevExpress.Xpf.Editors {
	public class NativeImage : Control, INativeImageRendererCallback, INotifyPropertyChanged {
		readonly NativeRenderer2 nativeRenderer;
		public static readonly DependencyProperty RendererProperty;
		INativeImageRenderer renderer;
		static NativeImage() {
			RendererProperty = DependencyPropertyRegistrator.Register<NativeImage, INativeImageRenderer>(
				owner => owner.Renderer, null, (control, oldValue, newValue) => control.RendererChanged(oldValue, newValue));
			BackgroundProperty.OverrideMetadata(typeof(NativeImage), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.None, (d, args) => ((NativeImage)d).BackgroundChanged((Brush)args.NewValue)));
		}
		SolidColorBrush cachedBackground;
		void BackgroundChanged(Brush newValue) {
			this.cachedBackground = newValue as SolidColorBrush;
		}
		protected virtual void RendererChanged(INativeImageRenderer oldValue, INativeImageRenderer newValue) {
			oldValue.Do(x => x.ReleaseCallback());
			newValue.Do(x => x.RegisterCallback(this));
			renderer = newValue;
			Invalidate();
		}
		public INativeImageRenderer Renderer {
			get { return renderer; }
			set { SetValue(RendererProperty, value); }
		}
		int fps;
		bool isInitialized = false;
		public int FPS {
			get { return fps; }
			set {
				if (fps != value) {
					fps = value;
					RaisePropertyChanged("FPS");
				}
			}
		}
		public NativeImage() {
			nativeRenderer = new NativeRenderer2(() => this.cachedBackground.Return(x => x.Color.ToWinFormsColor(), () => Color.White));
			SizeChanged += OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			nativeRenderer.Resize(e.NewSize);
			isInitialized = true;
			Invalidate();
		}
		readonly Stopwatch stopwatch = Stopwatch.StartNew();
		int count;
		long last;
		Rect invalidateRect = Rect.Empty;
		bool whole = false;
		bool requiresUpdate = false;
		private void CompositionTarget_Rendering(object sender, EventArgs e) {
			InvalidateVisual();
			long ms = stopwatch.ElapsedMilliseconds;
			if (ms - last > 1000) {
				FPS = (int)(count / ((double)(ms - last) / 1000d));
				count = 0;
				last = ms;
				return;
			}
			count++;
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if (!isInitialized)
				return;
			if (requiresUpdate) {
				nativeRenderer.StartRender();
				nativeRenderer.Render(this.renderer, invalidateRect);
				nativeRenderer.EndRender(invalidateRect, whole);
				whole = false;
				requiresUpdate = false;
				invalidateRect = Rect.Empty;
			}
			drawingContext.DrawImage(nativeRenderer.Source, new Rect(RenderSize));
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string property) {
			PropertyChanged.Do(x => x(this, new PropertyChangedEventArgs(property)));
		}
		public void SetRenderMask(DrawingBrush drawing) {
			OpacityMask = drawing;
		}
		public void Invalidate() {
			InvalidateInternal(new Rect(0, 0, RenderSize.Width, RenderSize.Height), true);
		}
		public void Invalidate(Rect region) {
			InvalidateInternal(region, false);
		}
		void InvalidateInternal(Rect region, bool whole) {
			if (!isInitialized || renderer == null)
				return;
			invalidateRect.Union(region);
			this.requiresUpdate = true;
			this.whole = whole;
			InvalidateVisual();
		}
	}
	public interface INativeImageRendererCallback {
		void Invalidate();
		void Invalidate(Rect region);
		void SetRenderMask(DrawingBrush drawing);
	}
	public interface INativeImageRenderer {
		void RegisterCallback(INativeImageRendererCallback callback);
		void ReleaseCallback();
		void Render(Graphics graphics, Rect invalidateRect, Size renderSize);
	}
	public class VirtualSurfaceImageSourceLikeRenderer : INativeImageRenderer {
		INativeImageRendererCallback callback;
		public void RegisterCallback(INativeImageRendererCallback callback) {
			this.callback = callback;
		}
		public void ReleaseCallback() {
			this.callback = null;
		}
		public void Render(Graphics graphics, Rect invalidateRect, Size renderSize) {
		}
	}
	[SecuritySafeCritical]
	public sealed class NativeRenderer2 : IDisposable {
		#region Win32 Interop
		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr CreateFileMapping(
			IntPtr lpBaseAddress,
			IntPtr lpFileMappingAttributes,
			uint flProtect,
			uint dwMaximumSizeHigh,
			uint dwMaximumSizeLow,
			string lpName);
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr MapViewOfFile(
			IntPtr hFileMappingObject,
			uint dwDesiredAccess,
			uint dwFileOffsetHigh,
			uint dwFileOffsetLow,
			UIntPtr dwNumberOfBytesToMap);
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr hObject);
		const uint FileMapAllAccess = 0xF001F;
		const uint PageReadwrite = 0x04;
		static readonly IntPtr InvalidHandleValue = new IntPtr(-1);
		#endregion
		InteropBitmap source;
		public InteropBitmap Source { get { return source; } }
		Bitmap Bitmap { get; set; }
		int width;
		int height;
		const PixelFormat Format = PixelFormat.Format32bppArgb;
		const int Bpp = 32;
		int stride;
		uint numBytes;
		IntPtr section;
		IntPtr hBuffer;
		bool isDisposed;
		readonly Locker renderLocker;
		INativeRendererImpl nativeRenderer;
		public int MaxCacheSize { get; set; }
		BufferedGraphicsContext bgc;
		bool isZeroSize = false;
		Func<Color> getBackgroundHandler;
		public NativeRenderer2(Func<Color> getBackground) {
			renderLocker = new Locker();
			this.getBackgroundHandler = getBackground;
		}
		~NativeRenderer2() {
			DisposeInternal();
		}
		public void InvalidateSource() {
			if (isDisposed)
				throw new ObjectDisposedException("NativeRenderer");
			Source.Invalidate();
		}
		public void Dispose() {
			DisposeInternal();
			GC.SuppressFinalize(this);
		}
		void DisposeInternal() {
			if (isDisposed)
				return;
			CleanUp();
			isDisposed = true;
		}
		void CleanUp() {
			if (section != IntPtr.Zero) {
				UnmapViewOfFile(section);
				CloseHandle(section);
			}
			bgc.Do(x => x.Dispose());
			bgc = null;
			section = IntPtr.Zero;
			Bitmap.Do(x => x.Dispose());
			Bitmap = null;
		}
		public void StartRender() {
			if (isDisposed)
				throw new ObjectDisposedException("NativeRenderer");
			if (renderLocker.IsLocked)
				throw new ArgumentException("render started");
			renderLocker.Lock();
		}
		public void EndRender(Rect region, bool whole) {
			if (isDisposed)
				throw new ObjectDisposedException("NativeRenderer");
			renderLocker.Unlock();
			Rect bmpRect = new Rect(0, 0, width, height);
			bmpRect.Intersect(region);
			if (IsZeroSize(bmpRect.Size))
				return;
			var rect = bmpRect.ToWinFormsRectangle();
			BitmapData data = Bitmap.LockBits(rect, ImageLockMode.ReadOnly, Format);
			CopyMemoryImpl(data.Scan0, hBuffer, stride, height, rect, whole);
			Bitmap.UnlockBits(data);
			InvalidateSource();
		}
		bool IsZeroSize(Size size) {
			return size.Width < 1 || size.Height < 1;
		}
		static void CopyMemoryImpl(IntPtr source, IntPtr target, int stride, int height, Rectangle region, bool whole) {
			unchecked {
				if (whole) {
					CopyMemory(target, source, (uint)(stride * height));
					return;
				}
				int offset = region.Left * 4;
				int length = region.Width * 4;
				IntPtr pSource = source;
				IntPtr pTarget = target + stride * region.Top + offset;
				for (int i = 0; i < region.Height; i++) {
					pTarget += stride;
					pSource += stride;
					CopyMemory(pTarget, pSource, (uint)length);
				}
			}
		}
		public bool Render(INativeImageRenderer renderer, Rect region) {
			if (isDisposed)
				throw new ObjectDisposedException("NativeRenderer");
			if (!renderLocker.IsLocked)
				throw new ArgumentException("call StartRender");
			if (isZeroSize)
				return true;
			return RenderToBitmap(renderer, region);
		}
		bool RenderToBitmap(INativeImageRenderer renderer, Rect region) {
			bool hasRenderStubs = false;
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero)) {
				Rectangle bitmapRect = new Rectangle(0, 0, width, height);
				using (BufferedGraphics bufferedGraphics = bgc.Allocate(g, bitmapRect)) {
					using (Graphics graphics = bufferedGraphics.Graphics) {
						graphics.Clear(this.getBackgroundHandler());
						hasRenderStubs |= nativeRenderer.RenderToGraphics(graphics, renderer, region, new Size(width, height));
						using (Graphics gr = Graphics.FromImage(Bitmap))
							bufferedGraphics.Render(gr);
					}
				}
			}
			return hasRenderStubs;
		}
		public void UpdateNativeRenderer() {
			nativeRenderer.Do(x => x.Dispose());
			nativeRenderer = CreateNativeRendererImpl();
		}
		public void Resize(Size size) {
			CleanUp();
			UpdateNativeRenderer();
			width = (int)(size.Width);
			height = (int)(size.Height);
			isZeroSize = IsZeroSize(size);
			if (isZeroSize)
				return;
			stride = width * Bpp / 8;
			numBytes = (uint)(height * stride);
			Bitmap = new Bitmap(width, height, Format);
			bgc = new BufferedGraphicsContext();
			section = CreateFileMapping(InvalidHandleValue, IntPtr.Zero, PageReadwrite, 0, numBytes, null);
			hBuffer = MapViewOfFile(section, FileMapAllAccess, 0, 0, (UIntPtr)numBytes);
			source = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(section, width, height, PixelFormats.Bgr32, stride, 0);
		}
		INativeRendererImpl CreateNativeRendererImpl() {
			return new DirectNativeRendererImpl();
		}
		public void UpdateRenderer() {
			Resize(new Size(width, height));
		}
		public void ResetCaches() {
			nativeRenderer.Reset();
		}
	}
	public interface INativeRendererImpl : IDisposable {
		bool RenderToGraphics(Graphics graphics, INativeImageRenderer renderer, Rect invalidateRect, Size totalSize);
		void Reset();
	}
	public abstract class NativeRendererImpl : INativeRendererImpl {
		public void Dispose() {
			DisposeInternal();
		}
		protected virtual void DisposeInternal() {
		}
		public abstract bool RenderToGraphics(Graphics graphics, INativeImageRenderer renderer, Rect invalidateRect, Size totalSize);
		public virtual void Reset() {
		}
		public virtual void InvalidateCaches() {
		}
	}
	public class DirectNativeRendererImpl : NativeRendererImpl {
		public override bool RenderToGraphics(Graphics graphics, INativeImageRenderer renderer, Rect invalidateRect, Size totalSize) {
			renderer.Render(graphics, invalidateRect, totalSize);
			return true;
		}
	}
}
