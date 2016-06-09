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
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils.Text;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {	
	public enum GraphicsQuality { Lowest, Highest }
	public interface INativeGraphics : IDisposable {
		GraphicsQuality GraphicsQuality { get; set; }
		void Execute(GraphicsCommand command);
	}
	public class GdiPlusGraphics : INativeGraphics {
		Graphics gr;
		public GraphicsQuality GraphicsQuality { get { return GraphicsQuality.Highest; } set {} }
		public Graphics Graphics { get { return gr; } }
		public GdiPlusGraphics(Graphics gr) {
			this.gr = gr;
		}
		public void Dispose() {
		}
		public void Execute(GraphicsCommand command) {
		}
	}
	public class OpenGLGraphics : INativeGraphics {
		static OpenGLGraphics() {
			try {
				GL.Finish(); 
			}
			catch {
			}
		}
		public static void CalculateColorComponents(Color color, float[] result) {
			if (result.Length < 3)
				throw new InternalException("Incorrect array size.");
			result[0] = color.R / 255.0f;
			result[1] = color.G / 255.0f;
			result[2] = color.B / 255.0f;
			if (result.Length > 3)
				result[3] = color.A / 255.0f;
		}
		public static void CalculateColorComponents(Color color, out float red, out float green, out float blue) {
			red = color.R / 255.0f;
			green = color.G / 255.0f;
			blue = color.B / 255.0f;
		}
		public static void CalculateColorComponents(Color color, out float red, out float green, out float blue, out float alpha) {
			CalculateColorComponents(color, out red, out green, out blue);
			alpha = color.A / 255.0f;
		}
		OpenGLGraphicsTextureCache textureCache;
		Graphics gr;
		Graphics nativeGraphics;
		Rectangle bounds;
		IntPtr hdc;
		IntPtr hglrc;
		bool isWindowDC;
		bool isDoubleBuffered;
		GraphicsQuality graphicsQuality = GraphicsQuality.Highest;
		bool nativeDrawing;
		Bitmap bitmap;
		int stencilBufferMaxValue;
		public GraphicsQuality GraphicsQuality { 
			get { return graphicsQuality; } 
			set { graphicsQuality = value; } 
		}
		public bool Initialized { get { return hdc != IntPtr.Zero && hglrc != IntPtr.Zero; } }
		public Graphics NativeGraphics { get { return nativeGraphics; } }
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds == value)
					return;
				bounds = value;
				OnBoundsChanged();
			}
		}
		public Size Size { 
			get { return bounds.Size; } 
			set {
				if(bounds.Size == value)
					return;
				Bounds = new Rectangle(bounds.Location, value);
			} 
		}
		public IntPtr HDC { get { return hdc; } }
		public IntPtr HGLRC { get { return hglrc; } }
		public Graphics GDIPlusGraphics { get { return gr; } }
		public OpenGLGraphicsTextureCache TextureCache { get { return textureCache; } }
		public bool NativeDrawing { get { return nativeDrawing; } }
		public int StencilBufferMaxValue { get { return stencilBufferMaxValue; } }
		public OpenGLGraphics(Graphics gr, IntPtr hdc, Rectangle bounds, OpenGLGraphicsTextureCache textureCache) {
			lock (typeof(OpenGLGraphics)) {
				CreateNativeObjects(bounds.Size);
				this.gr = gr;
				if (hdc == IntPtr.Zero) {
					isWindowDC = false;
					this.hdc = gr.GetHdc();
				}
				else {
					isWindowDC = true;
					this.hdc = hdc;
				}
				this.bounds = bounds;
				this.textureCache = textureCache;
				WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
				pfd.dwFlags = WGL.PFD_SUPPORT_OPENGL | WGL.PFD_GENERIC_ACCELERATED | WGL.PFD_STEREO_DONTCARE;
				if (isWindowDC)
					pfd.dwFlags |= WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_GENERIC_ACCELERATED | WGL.PFD_DOUBLEBUFFER;
				else
					pfd.dwFlags |= WGL.PFD_DRAW_TO_BITMAP;
				pfd.cAccumBits = 64;
				pfd.cStencilBits = 32; 
				int pixelFormat = WGL.ChoosePixelFormat(this.hdc, pfd);
				int count = WGL.DescribePixelFormat(this.hdc, pixelFormat, Marshal.SizeOf(pfd), pfd);
				if (count == 0)
					return;
				if (pixelFormat != 0 && CreateContext(this.hdc, pixelFormat, pfd))
					return;
				for (int i = 1; i <= count; i++) {
					if (WGL.DescribePixelFormat(this.hdc, i, Marshal.SizeOf(pfd), pfd) == 0 ||
						pfd.iPixelType != WGL.PFD_TYPE_RGBA || pfd.cStencilBits < 8 ||
						pfd.cAccumBits < 32 ||
						(pfd.dwFlags & WGL.PFD_SUPPORT_OPENGL) != WGL.PFD_SUPPORT_OPENGL)
							continue;
					if (isWindowDC) {
						if ((pfd.dwFlags & (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_DOUBLEBUFFER)) != (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_DOUBLEBUFFER))
							continue;
					}
					else if ((pfd.dwFlags & WGL.PFD_DRAW_TO_BITMAP) != WGL.PFD_DRAW_TO_BITMAP)
						continue;
					if (CreateContext(this.hdc, i, pfd))
						return;
				}
			}
		}
		void OnBoundsChanged() {
			CreateNativeObjects(bounds.Size);
		}
		bool CreateContext(IntPtr hdc, int pixelFormat, WGL.PIXELFORMATDESCRIPTOR pfd) {
			if (!WGL.SetPixelFormat(hdc, pixelFormat, pfd))
				return false;
			hglrc = WGL.CreateContext(hdc);
			if (hglrc == IntPtr.Zero)
				return false;
			isDoubleBuffered = (pfd.dwFlags & WGL.PFD_DOUBLEBUFFER) == WGL.PFD_DOUBLEBUFFER;
			return true;
		}
		void FinishDrawing() {
			GL.Finish();
			if (isDoubleBuffered)
				WGL.SwapBuffers(hdc);
			WGL.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}
		void Prepare() {
			WGL.MakeCurrent(hdc, hglrc);
			GL.MatrixMode(GL.PROJECTION);
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.MatrixMode(GL.MODELVIEW);
			GL.PushMatrix();
			GL.LoadIdentity();			
		}
		void Restore() {
			GL.MatrixMode(GL.MODELVIEW);
			GL.PopMatrix();
			GL.MatrixMode(GL.PROJECTION);
			GL.PopMatrix();
			WGL.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}
		void DisposeNativeGraphics() {
			if (nativeGraphics != null) {
				nativeGraphics.Dispose();
				nativeGraphics = null;
			}
		}
		void DisposeBitmap() {
			if (bitmap != null) {
				bitmap.Dispose();
				bitmap = null;
			}
		}
		void DrawBitmap() {
			if (bitmap == null)
				return;
			int[] maxTexSize = new int[1];
			GL.GetIntegerv(GL.MAX_TEXTURE_SIZE, maxTexSize);
			uint[] names = new uint[1];
			GL.GenTextures(1, names);
			GL.BindTexture(GL.TEXTURE_2D, names[0]);
			GL.PixelStorei(GL.UNPACK_ALIGNMENT, 1);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.REPEAT);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.REPEAT);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.NEAREST);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.NEAREST);
			GL.TexEnvf(GL.TEXTURE_ENV, GL.TEXTURE_ENV_MODE, GL.REPLACE);
			GL.Enable(GL.TEXTURE_2D);
			GL.MatrixMode(GL.MODELVIEW);
			GL.LoadIdentity();
			GL.MatrixMode(GL.PROJECTION);
			GL.LoadIdentity();
			GL.Ortho(0.0, bounds.Width, bounds.Height, 0.0, -1.0, 1.0);
			GL.Disable(GL.DEPTH_TEST);
			GL.Enable(GL.BLEND);
			GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
			GL.Color4f(0.0f, 0.0f, 0.0f, 0.0f);
			BitmapData srcData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			TextureInfo[] infos = TextureInfo.CalcTextureInfos(srcData, maxTexSize[0]);
			foreach (TextureInfo info in infos) {
				if (info.TexturePointer == IntPtr.Zero)
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TextureArray);
				else
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TexturePointer);
				GL.Begin(GL.QUADS);
				GL.TexCoord2d(0, 0);
				GL.Vertex3d(info.X, info.Y, 0.0);
				GL.TexCoord2d(0, 1);
				GL.Vertex3d(info.X, info.Y + info.Height, 0.0);
				GL.TexCoord2d(1, 1);
				GL.Vertex3d(info.X + info.Width, info.Y + info.Height, 0.0);
				GL.TexCoord2d(1, 0);
				GL.Vertex3d(info.X + info.Width, info.Y, 0.0);
				GL.End();
			}
			bitmap.UnlockBits(srcData);
			GL.Disable(GL.TEXTURE_2D);
			GL.DeleteTextures(1, names);
		}
		void CreateNativeObjects(Size size) {
			DisposeBitmap();
			DisposeNativeGraphics();
			if (size.Width == 0)
				size.Width = 1;
			if (size.Height == 0)
				size.Height = 1;
			bitmap = new Bitmap(GraphicUtils.CalcAlignedTextureSize(size.Width), GraphicUtils.CalcAlignedTextureSize(size.Height));
			nativeGraphics = Graphics.FromImage(bitmap);
		}
		public void SetDrawingType(bool nativeDrawing) {
			if (nativeDrawing != this.nativeDrawing)
				this.nativeDrawing = nativeDrawing;
			if (nativeDrawing)
				nativeGraphics.Clear(Color.Transparent);
		}
		public void Execute(GraphicsCommand command) {
			if (command != null && hdc != IntPtr.Zero && hglrc != IntPtr.Zero)
				lock (this) {
					if (!WGL.MakeCurrent(hdc, hglrc))
						return;
					try {
						GL.Viewport(Bounds.X, bounds.Y, Bounds.Width, Bounds.Height);
						float red, green, blue, alpha;
						CalculateColorComponents(Control.DefaultBackColor, out red, out green, out blue, out alpha);
						GL.ClearColor(red, green, blue, alpha);
						GL.Clear(GL.DEPTH_BUFFER_BIT | GL.STENCIL_BUFFER_BIT | GL.COLOR_BUFFER_BIT);
						GL.Enable(GL.BLEND);
						GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
						GL.ShadeModel(GL.SMOOTH);
						GL.MatrixMode(GL.MODELVIEW);
						GL.LoadIdentity();
						GL.MatrixMode(GL.PROJECTION);
						GL.LoadIdentity();
						GL.Ortho(0.0, Bounds.Width, Bounds.Height, 0.0, -1.0, 1.0);
						int[] bufferSize = new int[1];
						GL.GetIntegerv(GL.STENCIL_BITS, bufferSize);
						stencilBufferMaxValue = (int)Math.Pow(2, bufferSize[0]) - 1;
						nativeDrawing = false;
						command.Execute(this);
						DrawBitmap();
					}
					catch (ThreadAbortException) {
						WGL.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
						throw;
					}
					finally {
						FinishDrawing();
						nativeDrawing = false;
					}
				}
		}
		public void CalculateTransformationMatrix(GraphicsCommand command, double[] matrix) {
			double[] projection = new double[16];
			CalculateMatrices(command, matrix, projection);
		}
		public void CalculateMatrices(GraphicsCommand command, double[] modelView, double[] projection) {
			lock (this) {
				Prepare();
				try {
					command.Execute(this);
					GL.GetDoublev(GL.MODELVIEW_MATRIX, modelView);
					GL.GetDoublev(GL.PROJECTION_MATRIX, projection);
				}
				finally {
					Restore();
				}
			}
		}
		public void Dispose() {
			if (!isWindowDC) {
				if (hdc != IntPtr.Zero) {
					gr.ReleaseHdcInternal(hdc);
					hdc = IntPtr.Zero;
				}
			}
			if (hglrc != IntPtr.Zero) {
				WGL.DeleteContext(hglrc);
				hglrc = IntPtr.Zero;
			}
			DisposeBitmap();
			DisposeNativeGraphics();
		}
	}
}
