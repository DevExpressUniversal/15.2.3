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
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public abstract class ChartBitmapContainer : IDisposable {
		static ChartBitmapContainer CreateInstance(Chart chart, Size size, GraphicsQuality graphicsQuality, bool isChart3D) {
			if(isChart3D)
				return new ChartBitmapContainer3D(chart, size, graphicsQuality);
			else
				return new ChartBitmapContainer2D(chart, size, graphicsQuality);
		}
		public static ChartBitmapContainer CreateInstance(Chart chart, Size size, GraphicsQuality graphicsQuality) {
			return CreateInstance(chart, size, graphicsQuality, chart.Is3DDiagram);
		}
		public static Bitmap Draw(Chart chart, Size size, GraphicsQuality graphicsQuality) {
			using (ChartBitmapContainer container = CreateInstance(chart, size, graphicsQuality)) {
				try {
					container.DrawChart(true);
					return container.GetBitmap();					
				}
				catch {
					container.DisposeBitmap();
					return null;
				}
			}
		}
		Chart chart;
		Size size;
		GraphicsQuality graphicsQuality;
		Bitmap bitmap;
		Graphics graphics;
		INativeGraphics nativeGraphics;
		public Size Size { get { return size; } }
		internal INativeGraphics NativeGraphics { get { return nativeGraphics; } }
		public ChartBitmapContainer(Chart chart, Size size, GraphicsQuality graphicsQuality) {
			this.chart = chart;
			this.size = size;
			this.graphicsQuality = graphicsQuality;
			this.bitmap = new Bitmap(size.Width, size.Height);
		}
		void DisposeNativeGraphics() {
			if (this.nativeGraphics != null) {
				this.nativeGraphics.Dispose();
				this.nativeGraphics = null;
			}
		}
		void DisposeGraphics() {
			if (this.graphics != null) {
				this.graphics.Dispose();
				this.graphics = null;
			}
		}
		void Finish() {
			DisposeNativeGraphics();
			DisposeGraphics();
		}
		protected void DisposeBitmap() {
			if (this.bitmap != null) {
				this.bitmap.Dispose();
				this.bitmap = null;
			}
		}
		protected void Start() {
			if (this.bitmap != null) {
				Finish();
				this.graphics = GetBitmapGraphics();
				this.nativeGraphics = this.chart.CreateNativeGraphics(this.graphics, IntPtr.Zero, this.size, this.graphicsQuality);
			}
		}
		protected Graphics CreateGraphicsFromBitmap() {
			return Graphics.FromImage(this.bitmap);
		}
		protected Bitmap PrepareBitmap() {
			if(this.bitmap == null)
				return null;
			Finish();
			return this.bitmap;
		}
		protected abstract Graphics GetBitmapGraphics();
		protected abstract Bitmap GetBitmap();
		public void DrawChart(bool lockDrawingHelper) {
			using (Graphics gr = GetBitmapGraphics()) 
				chart.DrawContent(gr, nativeGraphics, new Rectangle(Point.Empty, size), lockDrawingHelper, true);
		}
		public virtual void Dispose() {
			Finish();
			GC.SuppressFinalize(this);
		}
	}
	public class ChartBitmapContainer2D : ChartBitmapContainer {
		public ChartBitmapContainer2D(Chart chart, Size size, GraphicsQuality graphicsQuality)
			: base(chart, size, graphicsQuality) {
			Start();
		}
		protected override Graphics GetBitmapGraphics() {
			return CreateGraphicsFromBitmap();
		}
		protected override Bitmap GetBitmap() {
			return PrepareBitmap();
		}
	}
	public class ChartBitmapContainer3D : ChartBitmapContainer {
		public static Bitmap Draw(Chart chart, Size size, GraphicsCommand command) {
			using (ChartBitmapContainer3D container = new ChartBitmapContainer3D(chart, size, GraphicsQuality.Highest)) {
				try {
					if (container.NativeGraphics is OpenGLGraphics) {
						container.NativeGraphics.Execute(command);
						return container.GetBitmap();					
					}
				}
				catch {
				}
				container.DisposeBitmap();
				return null;
			}
		}
		Matrix transformMatrix;
		public ChartBitmapContainer3D(Chart chart, Size size, GraphicsQuality graphicsQuality) : base(chart, size, graphicsQuality) {
			InitializeTransform();
			Start();
		}
		void InitializeTransform() {
			this.transformMatrix = new Matrix();
			this.transformMatrix.Translate(0, Size.Height);
			this.transformMatrix.Scale(1, -1);
		}
		void DisposeTransform() {
			if (this.transformMatrix != null) {
				this.transformMatrix.Dispose();
				this.transformMatrix = null;
			}
		}
		void ApplyTransform(Graphics graphics) {
			Matrix grTransform = graphics.Transform;
			grTransform.Multiply(this.transformMatrix, MatrixOrder.Prepend);
			graphics.Transform = grTransform;
		}
		protected override Graphics GetBitmapGraphics() {
			Graphics graphics = CreateGraphicsFromBitmap();
			ApplyTransform(graphics);
			return graphics;
		}
		protected override Bitmap GetBitmap() {
			Bitmap bitmap = PrepareBitmap();
			bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return bitmap;
		}
		public override void Dispose() {
			DisposeTransform();
			base.Dispose();
		}
		public Bitmap GetBitmapCopy() {
			Bitmap bitmap = PrepareBitmap();
			if(bitmap == null)
				return null;
			Bitmap bitmapCopy = (Bitmap)bitmap.Clone();
			bitmapCopy.RotateFlip(RotateFlipType.RotateNoneFlipY);
			Start();
			return bitmapCopy;
		}
	}
}
