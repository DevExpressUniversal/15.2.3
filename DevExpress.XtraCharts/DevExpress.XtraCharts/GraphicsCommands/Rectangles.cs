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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public abstract class RectangleGraphicsCommand : GraphicsCommand {
		PlaneRectangle rect;
		protected internal PlaneRectangle Rect { get { return this.rect; } }
		protected internal ZPlaneRectangle ZRect { 
			get { 
				ZPlaneRectangle zRect = rect as ZPlaneRectangle; 
				if (zRect == null)
					throw new NotSupportedException("ZRect");
				return zRect;
			} 
		}
		public RectangleGraphicsCommand(PlaneRectangle rect) : base() {
			this.rect = rect;
		}
	}
	public class SolidRectangleGraphicsCommand : RectangleGraphicsCommand {
		Color color;
		internal Color Color { get { return color; } }
		public SolidRectangleGraphicsCommand(PlaneRectangle rect, Color color) : base(rect) {
			this.color = color;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.Color4ub(Color.R, Color.G, Color.B, Color.A);
			DiagramVector normal = Rect.Normal;
			GL.Normal3f((float)normal.DX, (float)normal.DY, (float)normal.DZ);
			Vertex[] vertices = Rect.Vertices;
			GL.Begin(GL.QUADS);
				foreach (DiagramPoint vertex in vertices)
					GL.Vertex3d(vertex.X, vertex.Y, vertex.Z);
			GL.End();
		}
	}
	public class BackgroundSolidRectangleGraphicsCommand : SolidRectangleGraphicsCommand {
		bool blendingEnabled = false;
		public BackgroundSolidRectangleGraphicsCommand(PlaneRectangle rect, Color color) : base(rect, color) {
		}
		protected override void BeforeExecute(OpenGLGraphics gr) {
			if (Color.A == 255) {
				bool[] enableState = new bool[1];
				GL.GetBooleanv(GL.BLEND, enableState);
				if (enableState[0]) {
					GL.Disable(GL.BLEND);
					blendingEnabled = true;
				}
			}
		}
		protected override void AfterExecute(OpenGLGraphics gr) {
			if (blendingEnabled) {
				GL.Enable(GL.BLEND);
				blendingEnabled = false;
			}
		}
	}
	public class GradientRectangleGraphicsCommand : RectangleGraphicsCommand {
		Color color;
		Color color2;
		double colorCoord;
		double color2Coord;
		LinearGradientMode gradientMode;
		ZPlaneRectangle gradientRect;
		internal int ActualStartTexCoord {
			get {
				return double.IsNaN(colorCoord) ? GLHelper.MinTexCoord : GLHelper.ConvertTexCoord((float)colorCoord);
			}
		}
		internal int ActualEndTexCoord {
			get {
				return double.IsNaN(color2Coord) ? GLHelper.MaxTexCoord : GLHelper.ConvertTexCoord((float)color2Coord);
			}
		}
		internal int ActualMidTexCoord {
			get {
				return Math.Abs(ActualStartTexCoord - ActualEndTexCoord) / 2;
			}
		}
		internal Color Color { get { return color; } }
		internal Color Color2 { get { return color2; } }
		internal LinearGradientMode GradientMode { get { return gradientMode; } }
		internal double ColorCoord { get { return colorCoord; } }
		internal double Color2Coord { get { return color2Coord; } }
		public GradientRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2, LinearGradientMode gradientMode) : this(rect, color, color2, double.NaN, double.NaN, gradientMode) {
			this.gradientRect = gradientRect;
		}
		public GradientRectangleGraphicsCommand(PlaneRectangle rect, Color color, Color color2, double colorCoord, double color2Coord, LinearGradientMode gradientMode) : base(rect) {
			this.colorCoord = colorCoord;
			this.color2Coord = color2Coord;
			this.color = color;
			this.color2 = color2;
			this.gradientMode = gradientMode;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			float[] texture = GLHelper.Create1DTexture(Color, Color2);
			GL.Begin(GL.QUADS);
			DiagramVector normal = Rect.Normal;
			GL.Normal3f((float)normal.DX, (float)normal.DY, (float)normal.DZ);
			Vertex[] vertices = Rect.Vertices;
			switch (GradientMode) {
			case LinearGradientMode.Horizontal:
				GLHelper.SetColor(texture, ActualStartTexCoord);
				GL.Vertex3d(vertices[0].X, vertices[0].Y, vertices[0].Z);
				GLHelper.SetColor(texture, ActualEndTexCoord);
				GL.Vertex3d(vertices[1].X, vertices[1].Y, vertices[1].Z);
				GL.Vertex3d(vertices[2].X, vertices[2].Y, vertices[2].Z);
				GLHelper.SetColor(texture, ActualStartTexCoord);
				GL.Vertex3d(vertices[3].X, vertices[3].Y, vertices[3].Z);
				break;
			case LinearGradientMode.Vertical:
				GLHelper.SetColor(texture, ActualEndTexCoord);
				GL.Vertex3d(vertices[0].X, vertices[0].Y, vertices[0].Z);
				GL.Vertex3d(vertices[1].X, vertices[1].Y, vertices[1].Z);
				GLHelper.SetColor(texture, ActualStartTexCoord);
				GL.Vertex3d(vertices[2].X, vertices[2].Y, vertices[2].Z);
				GL.Vertex3d(vertices[3].X, vertices[3].Y, vertices[3].Z);
				break;
			case LinearGradientMode.BackwardDiagonal:
				GLHelper.SetColor(texture, ActualEndTexCoord);
				GL.Vertex3d(vertices[0].X, vertices[0].Y, vertices[0].Z);
				GLHelper.SetColor(texture, ActualMidTexCoord);
				GL.Vertex3d(vertices[1].X, vertices[1].Y, vertices[1].Z);
				GLHelper.SetColor(texture, ActualStartTexCoord);
				GL.Vertex3d(vertices[2].X, vertices[2].Y, vertices[2].Z);
				GLHelper.SetColor(texture, ActualMidTexCoord);
				GL.Vertex3d(vertices[3].X, vertices[3].Y, vertices[3].Z);
				break;
			case LinearGradientMode.ForwardDiagonal:
				GLHelper.SetColor(texture, ActualMidTexCoord);
				GL.Vertex3d(vertices[0].X, vertices[0].Y, vertices[0].Z);
				GLHelper.SetColor(texture, ActualEndTexCoord);
				GL.Vertex3d(vertices[1].X, vertices[1].Y, vertices[1].Z);
				GLHelper.SetColor(texture, ActualMidTexCoord);
				GL.Vertex3d(vertices[2].X, vertices[2].Y, vertices[2].Z);
				GLHelper.SetColor(texture, ActualStartTexCoord);
				GL.Vertex3d(vertices[3].X, vertices[3].Y, vertices[3].Z);
				break;
			}
			GL.End();
		}
	}
	public class HatchedRectangleGraphicsCommand : RectangleGraphicsCommand {
		Color color;
		Color backColor;
		HatchStyle hatchStyle;
		internal Color Color { get { return color; } }
		internal Color BackColor { get { return backColor; } }
		internal HatchStyle HatchStyle { get { return hatchStyle; } }
		public HatchedRectangleGraphicsCommand(PlaneRectangle rect, HatchStyle hatchStyle, Color color, Color backColor) : base(rect) {
			this.hatchStyle = hatchStyle;
			this.color = color;
			this.backColor = backColor;
		}
		public HatchedRectangleGraphicsCommand(PlaneRectangle rect, HatchStyle hatchStyle, Color color) : this(rect, hatchStyle, color, Color.Transparent) {
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			int textureSize = 128;
			using (Bitmap bitmap = new Bitmap(textureSize, textureSize)) {
				using (Graphics graphics = Graphics.FromImage(bitmap)) {
					using (Brush brush = new HatchBrush(hatchStyle, color, backColor))
						graphics.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					Vertex offset = ZRect.Location;
					ZPlaneRectangle rectangle = new ZPlaneRectangle(ZRect);
					rectangle.Offset(-offset.X, -offset.Y, 0.0);
					rectangle.Width = rectangle.Width < textureSize ? textureSize : rectangle.Width + offset.X;
					rectangle.Height = rectangle.Height < textureSize ? textureSize : rectangle.Height + offset.Y;
					using (TiledImageRectangleGraphicsCommand command = new TiledImageRectangleGraphicsCommand(rectangle, bitmap, false))
						command.Execute(gr);
				}
			}
		}
	}
	public class GdiImageGraphicsCommand : GraphicsCommand {
		Rectangle rect;
		Image image;
		public GdiImageGraphicsCommand(Rectangle rect, Image image) {
			this.rect = rect;
			this.image = image;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
		public override void Execute(OpenGLGraphics gr) {
			gr.SetDrawingType(true);
			lock (image)
				gr.NativeGraphics.DrawImage(image, rect);
			gr.SetDrawingType(false);
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			image.Dispose();
			image = null;
		}
	}
	public abstract class ImageRectangleGraphicsCommand : RectangleGraphicsCommand {
		Image image;
		bool disposeImage;
		bool reverseY;
		protected internal Image Image { get { return image; } }
		protected bool ReverseY { get { return this.reverseY; } }
		public ImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage, bool reverseY) : base(rect) {
			this.image = image;
			this.reverseY = reverseY;
			this.disposeImage = disposeImage;
		}
		public ImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage) : this(rect, image, disposeImage, false) {
		}
		TextureInfo[] CreateTexture(OpenGLGraphics gr, Image image, int maxTexSize) {
			TextureInfo[] infos = gr.TextureCache.GetTexture(image, maxTexSize);
			if (infos != null)
				return infos;
			infos = TextureInfo.CalcTextureInfos(image, maxTexSize, new Size((int)Rect.Width, (int)Rect.Height));
			gr.TextureCache.Add(image, maxTexSize, infos);
			return infos;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			lock(Image) {
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
				TextureInfo[] infos = CreateTexture(gr, image, maxTexSize[0]);
				DrawTexture(infos);
				GL.Disable(GL.TEXTURE_2D);
				GL.DeleteTextures(1, names);
			}
		}
		protected virtual void DrawTexture(TextureInfo[] infos) {
		}
		protected override void DisposeInternal() {
			if (disposeImage && image != null) {
				image.Dispose();
				image = null;
			}
			base.DisposeInternal();
		}
	}
	public class StretchedImageRectangleGraphicsCommand : ImageRectangleGraphicsCommand {
		public StretchedImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage, bool reverseY) : base(rect, image, disposeImage, reverseY) {
		}
		public StretchedImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage) : this(rect, image, disposeImage, false) {
		}
		protected override void DrawTexture(TextureInfo[] infos) {
			double widthRatio = Rect.Width / infos[0].TexWidth;
			double heightRatio = Rect.Height / infos[0].TexHeight;
			Vertex[] vertices = Rect.Vertices;
			foreach (TextureInfo info in infos) {
				if (info.TexturePointer == IntPtr.Zero)
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TextureArray);
				else
					GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TexturePointer);
				GL.Begin(GL.QUADS);
				DiagramVector normal = Rect.Normal;
				GL.Normal3f((float)normal.DX, (float)normal.DY, (float)normal.DZ);
				double surfMinX = info.X * widthRatio;
				double surfMaxX = (info.X + info.Width) * widthRatio;
				double surfMinY = ReverseY ? Rect.Location.Y + Rect.Height - info.Y * heightRatio : info.Y * heightRatio;
				double surfMaxY = ReverseY ? Rect.Location.Y + Rect.Height - (info.Y + info.Height) * heightRatio : (info.Y + info.Height) * heightRatio;
				GL.TexCoord2d(0, 0);
				GL.Vertex3d(surfMinX, surfMinY, vertices[0].Z);
				GL.TexCoord2d(0, 1);
				GL.Vertex3d(surfMinX, surfMaxY, vertices[0].Z);
				GL.TexCoord2d(1, 1);
				GL.Vertex3d(surfMaxX, surfMaxY, vertices[0].Z);
				GL.TexCoord2d(1, 0);
				GL.Vertex3d(surfMaxX, surfMinY, vertices[0].Z);
				GL.End();
			}
		}
	}
	public class TiledImageRectangleGraphicsCommand : ImageRectangleGraphicsCommand {
		public TiledImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage, bool reverseY) : base(rect, image, disposeImage, reverseY) {
		}
		public TiledImageRectangleGraphicsCommand(PlaneRectangle rect, Image image, bool disposeImage) : this(rect, image, disposeImage, false) {
		}
		double GetTexMaxX(TextureInfo info, ZPlaneRectangle rect, double scaleRatio) {
			if (info.X * scaleRatio > rect.Width)
				return double.NaN;
			double x = (info.X + info.Width) * scaleRatio;
			if (x > rect.Width)
				return (rect.Width - info.X * scaleRatio) / (info.Width * scaleRatio);
			return 1;
		}
		double GetTexMaxY(TextureInfo info, ZPlaneRectangle rect, double scaleRatio) {
			if (info.Y * scaleRatio > rect.Height)
				return double.NaN;
			double y = (info.Y + info.Height) * scaleRatio;
			if (y > rect.Height)
				return (rect.Height - info.Y * scaleRatio) / (info.Height * scaleRatio);
			return 1;
		}
		ZPlaneRectangle[] SplitRectangle(ZPlaneRectangle rect, double width, double height, bool reverseY) {
			ArrayList list = new ArrayList();
			for (double y = rect.Location.Y; y < rect.Location.Y + rect.Height; y += height) {
				for (double x = rect.Location.X; x < rect.Location.X + rect.Width; x += width) {
					double w = x + width > rect.Location.X + rect.Width ? rect.Location.X + rect.Width - x : width;
					double h = y + height > rect.Location.Y + rect.Height ? rect.Location.Y + rect.Height - y : height;
					list.Add(new ZPlaneRectangle(new DiagramPoint(x, y, rect.Location.Z), w, h));
				}
			}
			if (reverseY) {
				for (int i = 0; i < list.Count; i++) {
					Vertex[] vertices = ((ZPlaneRectangle)list[i]).Vertices;
					DiagramPoint point1 = new DiagramPoint(vertices[0].X, rect.Location.Y + rect.Height - vertices[0].Y, vertices[0].Z);
					DiagramPoint point2 = new DiagramPoint(vertices[2].X, rect.Location.Y + rect.Height - vertices[2].Y, vertices[2].Z);
					list[i] = new ZPlaneRectangle(point1, point2);
				}
			}
			return (ZPlaneRectangle[])list.ToArray(typeof(ZPlaneRectangle));
		}
		protected override void DrawTexture(TextureInfo[] infos) {
			double widthRatio = (double)Image.Width / (double)infos[0].TexWidth;
			double heightRatio = (double)Image.Height / (double)infos[0].TexHeight;
			ZPlaneRectangle[] rects = SplitRectangle(ZRect, Image.Width, Image.Height, ReverseY);
			foreach (ZPlaneRectangle rect in rects) {
				foreach (TextureInfo info in infos) {
					double texMaxX = GetTexMaxX(info, rect, widthRatio);
					double texMaxY = GetTexMaxY(info, rect, heightRatio);
					if (double.IsNaN(texMaxX) || double.IsNaN(texMaxY))
						continue;
					double surfMinX = rect.Location.X + info.X * widthRatio;
					double surfMaxX = surfMinX + info.Width * texMaxX * widthRatio;
					double surfMinY = ReverseY ? rect.Location.Y + rect.Height - info.Y * heightRatio : rect.Location.Y + info.Y * heightRatio;
					double surfMaxY = ReverseY ? surfMinY - info.Height * heightRatio * texMaxY : surfMinY + info.Height * heightRatio * texMaxY;
					if (info.TexturePointer == IntPtr.Zero)
						GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TextureArray);
					else
						GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, info.Width, info.Height, 0, GL.BGRA_EXT, GL.UNSIGNED_BYTE, info.TexturePointer);
					GL.Begin(GL.QUADS);
					DiagramVector normal = Rect.Normal;
					GL.Normal3f((float)normal.DX, (float)normal.DY, (float)normal.DZ);
					GL.TexCoord2d(0, 0);
					GL.Vertex3d(surfMinX, surfMinY, Rect.Location.Z);
					GL.TexCoord2d(0, texMaxY);
					GL.Vertex3d(surfMinX, surfMaxY, Rect.Location.Z);
					GL.TexCoord2d(texMaxX, texMaxY);
					GL.Vertex3d(surfMaxX, surfMaxY, Rect.Location.Z);
					GL.TexCoord2d(texMaxX, 0);
					GL.Vertex3d(surfMaxX, surfMinY, Rect.Location.Z);
					GL.End();
				}
			}
		}
	}
}
