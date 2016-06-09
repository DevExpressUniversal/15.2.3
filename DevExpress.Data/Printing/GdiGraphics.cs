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
using DevExpress.XtraPrinting;
namespace DevExpress.Printing {
	public class GdiGraphicsWrapperBase : IGraphicsBase {
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
		Graphics graphics;
		TransformStack transformStack = new TransformStack();
		protected Graphics Graphics { get { return graphics; } }
		public RectangleF ClipBounds {
			get { return graphics.ClipBounds; }
			set { graphics.SetClip(value); }
		}
		public GraphicsUnit PageUnit {
			get { return graphics.PageUnit; }
			set { graphics.PageUnit = value; }
		}
		public System.Drawing.Drawing2D.SmoothingMode SmoothingMode {
			get { return graphics.SmoothingMode; }
			set { graphics.SmoothingMode = value; }
		}
		public GdiGraphicsWrapperBase(Graphics gr) {
			this.graphics = gr;
		}
		public void ApplyTransformState(MatrixOrder order, bool removeState) {
			Matrix transform = removeState ? this.transformStack.Pop() : this.transformStack.Peek();
			MultiplyTransform(transform, order);
		}
		public virtual void DrawCheckBox(System.Drawing.RectangleF rect, System.Windows.Forms.CheckState state) {
		}
		public void DrawEllipse(Pen pen, RectangleF rect) {
			DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
			lock (pen) {
				graphics.DrawEllipse(pen, x, y, width, height);
			}
		}
		public void DrawImage(Image image, RectangleF bounds, Color underlyingColor) {
			DrawImage(image, bounds);
		}
		public void DrawImage(Image image, RectangleF bounds) {
			lock (image) {
				graphics.DrawImage(image, bounds);
			}
		}
		public void DrawImage(Image image, Point position) {
			lock (image)
				graphics.DrawImage(image, position);
		}
		public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
			DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
		}
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			lock (pen) {
				graphics.DrawLine(pen, x1, y1, x2, y2);
			}
		}
		public void DrawLines(Pen pen, PointF[] points) {
			lock (pen) {
				graphics.DrawLines(pen, points);
			}
		}
		public void DrawRectangle(Pen pen, RectangleF rect) {
			lock (pen) {
				graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
		}
		public void DrawPath(Pen pen, GraphicsPath path) {
			lock (pen) {
				lock (path) {
					graphics.DrawPath(pen, path);
				}
			}
		}
		public void FillPath(Brush brush, GraphicsPath path) {
			lock (brush) {
				lock (path) {
					graphics.FillPath(brush, path);
				}
			}
		}
		public void DrawString(string s, Font font, Brush brush, PointF point) {
			DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), null);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) {
			DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0f, 0f), format);
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds) {
			DrawString(s, font, brush, bounds, null);
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			graphics.DrawString(s, font, brush, bounds, format);
		}
		public void FillEllipse(Brush brush, RectangleF rect) {
			FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}
		public void FillEllipse(Brush brush, float x, float y, float width, float height) {
			lock (brush) {
				graphics.FillEllipse(brush, x, y, width, height);
			}
		}
		public void FillRectangle(Brush brush, float x, float y, float width, float height) {
			graphics.FillRectangle(brush, new RectangleF(new PointF(x, y), new SizeF(width, height)));
		}
		public void FillRectangle(Brush brush, RectangleF bounds) {
			graphics.FillRectangle(brush, bounds);
		}
		public Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public SizeF MeasureString(string text, Font font, GraphicsUnit graphicsUnit) {
			return MeasureString(text, font, new SizeF(0f, 0f), null, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return MeasureString(text, font, new SizeF(width, 999999f), stringFormat, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			graphics.PageUnit = graphicsUnit;
			return graphics.MeasureString(text, font, size, stringFormat);
		}
		public SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			graphics.PageUnit = graphicsUnit;
			return graphics.MeasureString(text, font, location, stringFormat);
		}
		public void ResetTransform() {
			graphics.ResetTransform();
		}
		public void RotateTransform(float angle) {
			RotateTransform(angle, MatrixOrder.Prepend);
		}
		public void RotateTransform(float angle, MatrixOrder order) {
			graphics.RotateTransform(angle, order);
		}
		public void MultiplyTransform(Matrix matrix) {
			graphics.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}
		public void MultiplyTransform(Matrix matrix, MatrixOrder order) {
			lock (matrix) {
				graphics.MultiplyTransform(matrix, order);
			}
		}
		public void SaveTransformState() {
			transformStack.Push(graphics.Transform);
		}
		public void ScaleTransform(float sx, float sy) {
			ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}
		public void ScaleTransform(float sx, float sy, MatrixOrder order) {
			graphics.ScaleTransform(sx, sy, order);
		}
		public void TranslateTransform(float dx, float dy) {
			TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}
		public void TranslateTransform(float dx, float dy, MatrixOrder order) {
			graphics.TranslateTransform(dx, dy, order);
		}
	}
}
