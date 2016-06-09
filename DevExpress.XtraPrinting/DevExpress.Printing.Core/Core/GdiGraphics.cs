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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	public class GdiGraphics : GraphicsBase, IGraphics {
		#region inner classes
		class TransformStack {
			Stack stack = new Stack();
			public void Push(Matrix transform) {
				this.stack.Push(transform);
			}
			public Matrix Pop() {
				return (Matrix)this.stack.Pop();
			}
			public Matrix Peek() {
				return (Matrix)this.stack.Peek();
			}
		}
		#endregion
		private static Image checkImage;
		private static Image uncheckImage;
		private static Image checkGrayImage;
		static GdiGraphics() {
			lock(typeof(GdiGraphics)) {
				checkImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Checked);
				uncheckImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Unchecked);
				checkGrayImage = CheckBoxImageHelper.GetCheckBoxImage(CheckState.Indeterminate);
			}
		}
		private Graphics gr;
		TransformStack transformStack = new TransformStack();
		protected GraphicsModifier modifier;
		public Graphics Graphics {
			get { return gr; }
		}
		public float Dpi {
			get { return gr.DpiX; }
		}
		public RectangleF ClipBounds {
			get { return gr.ClipBounds; }
			set { gr.SetClip(value); }
		}
		public Region Clip {
			get { return gr.Clip; }
			set { gr.Clip = value; }
		}
		public GraphicsUnit PageUnit {
			get { return gr.PageUnit; }
			set { modifier.SetPageUnit(gr, value); }
		}
		public Matrix Transform { get { return gr.Transform; } set { gr.Transform = value; } }
		public SmoothingMode SmoothingMode { get { return gr.SmoothingMode; } set { gr.SmoothingMode = value; } }
		public GdiGraphics(Graphics gr, PrintingSystemBase ps) : base(ps) {
			this.gr = gr;
			modifier = ((IServiceProvider)ps).GetService(typeof(GraphicsModifier)) as GraphicsModifier;
			SetPageUnit();
			ForceUpdateClipBounds(gr);
		}
		static void ForceUpdateClipBounds(Graphics gr) {
			System.Drawing.Drawing2D.Matrix transform = gr.Transform;
			gr.ResetTransform();
			gr.Transform = transform;
		}
		protected virtual void SetPageUnit() {
			PageUnit = GraphicsUnit.Document;
		}
		public void FillRectangle(Brush brush, RectangleF bounds) {
			FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}
		public void FillRectangle(Brush brush, float x, float y, float width, float height) {
			lock(brush) {
				gr.FillRectangle(brush, x, y, width, height);
			}
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds) {
			DrawString(s, font, brush, bounds, null);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point) {
			DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) {
			DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
		}
		public virtual void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			EnsureStringFormat(font, bounds, gr.PageUnit, format, validFormat => {
				modifier.DrawString(gr, s, font, brush, bounds, validFormat);
			});
		}
		public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
			DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
		}
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			lock(pen) {
				gr.DrawLine(pen, x1, y1, x2, y2);
			}
		}
		public void DrawLines(Pen pen, PointF[] points) {
			lock(pen) {
				gr.DrawLines(pen, points);
			}
		}
		public void DrawImage(Image image, RectangleF bounds, Color underlyingColor) {
			DrawImage(image, bounds);
		}
		public virtual void DrawImage(Image image, RectangleF bounds) {
			lock(image) {
				modifier.DrawImage(gr, image, bounds);
			}
		}
		public virtual void DrawImage(Image image, Point position) {
			lock(image)
				modifier.DrawImage(gr, image, position);
		}
		public void DrawRectangle(Pen pen, RectangleF rect) {
			lock(pen) {
				gr.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
		}
		public void DrawPath(Pen pen, GraphicsPath path) {
			lock(pen) {
				lock(path) {
					gr.DrawPath(pen, path);
				}
			}
		}
		public void FillPath(Brush brush, GraphicsPath path) {
			lock(brush) {
				lock(path) {
					gr.FillPath(brush, path);
				}
			}
		}
		public void DrawCheckBox(RectangleF rect, CheckState state) {
			Image image = (state & CheckState.Checked) != 0 ? checkImage :
				(state & CheckState.Indeterminate) != 0 ? checkGrayImage :
				uncheckImage;
			lock(image) {
				gr.DrawImageUnscaled(image, Rectangle.Round(rect).Location);
			}
		}
		public void DrawEllipse(Pen pen, RectangleF rect) {
			DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
			lock(pen) {
				gr.DrawEllipse(pen, x, y, width, height);
			}
		}
		public void FillEllipse(Brush brush, RectangleF rect) {
			FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void FillEllipse(Brush brush, float x, float y, float width, float height) {
			lock(brush) {
				gr.FillEllipse(brush, x, y, width, height);
			}
		}
		public SizeF MeasureString(string text, Font font, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, width, stringFormat, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, size, stringFormat, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, location, stringFormat, graphicsUnit);
		}
		public void ResetTransform() {
			gr.ResetTransform();
		}
		public void MultiplyTransform(Matrix matrix) {
			gr.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}
		public void MultiplyTransform(Matrix matrix, MatrixOrder order) {
			lock(matrix) {
				gr.MultiplyTransform(matrix, order);
			}
		}
		public void RotateTransform(float angle) {
			RotateTransform(angle, MatrixOrder.Prepend);
		}
		public void RotateTransform(float angle, MatrixOrder order) {
			gr.RotateTransform(angle, order);
		}
		public void ScaleTransform(float sx, float sy) {
			ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}
		public void ScaleTransform(float sx, float sy, MatrixOrder order) {
			modifier.ScaleTransform(gr, sx, sy, order);
		}
		public void TranslateTransform(float dx, float dy) {
			TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}
		public void TranslateTransform(float dx, float dy, MatrixOrder order) {
			gr.TranslateTransform(dx, dy, order);
		}
		public void SaveTransformState() {
			this.transformStack.Push(this.gr.Transform);
		}
		public void ApplyTransformState(MatrixOrder order, bool removeState) {
			Matrix transform = removeState ? this.transformStack.Pop() : this.transformStack.Peek();
			MultiplyTransform(transform, order);
		}
		public virtual void Dispose() {
			if(modifier != null) {
				modifier.OnGraphicsDispose();
				modifier = null;
			}
		}
	}
}
namespace DevExpress.XtraPrinting.Native {
	using System.Runtime.InteropServices;
	public class ImageGraphics : GdiGraphics {
		Image img;
		public ImageGraphics(Image img, PrintingSystemBase ps)
			: base(Graphics.FromImage(img), ps) {
			this.img = img;
		}
		public override void DrawImage(Image image, Point position) {
			if(!(image is System.Drawing.Imaging.Metafile && img is System.Drawing.Imaging.Metafile)) {
				base.DrawImage(image, position);
				return;
			}
			GraphicsUnit savedPageUnit = Graphics.PageUnit;
			try {
				Graphics.PageUnit = GraphicsUnit.Pixel;
				base.DrawImage(image, GraphicsUnitConverter.Convert(position, savedPageUnit, Graphics.PageUnit));
			} finally {
				Graphics.PageUnit = savedPageUnit;
			}
		}
		public override void DrawImage(Image image, RectangleF bounds) {
			if(!(image is System.Drawing.Imaging.Metafile && img is System.Drawing.Imaging.Metafile)) {
				base.DrawImage(image, bounds);
				return;
			}
			GraphicsUnit savedPageUnit = Graphics.PageUnit;
			try {
				Graphics.PageUnit = GraphicsUnit.Pixel;
				base.DrawImage(image, GraphicsUnitConverter.Convert(bounds, savedPageUnit, Graphics.PageUnit));
			} finally {
				Graphics.PageUnit = savedPageUnit;
			}
		}
		public override void Dispose() {
			base.Dispose();
			Graphics.Dispose();
		}
	}
}
