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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class GradientPainterBase : IGradientPainter {
		protected IRenderer renderer;
		PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			for (int i = 0; i < polygon.Vertices.Length; i++)
				polygon.Vertices[i].Color = CalcVertexColor(polygon.Vertices[i], gradientRect, color, color2);
			return new PlanePolygon[] { polygon };
		}
		protected GradientPainterBase(IRenderer assignRenderer) {
			renderer = assignRenderer;
		}
		#region IGradientPainter Direct Rendering Adoption
		void IGradientPainter.RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			RenderRectangle(rect, gradientRect, color, color2);
		}
		void IGradientPainter.RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			boundRectangle.Inflate(1, 1);
			renderer.SetClipping(boundRectangle, CombineMode.Intersect);
			RenderStrip(vertices, boundRectangle, color, color2);
			renderer.RestoreClipping();
		}
		void IGradientPainter.RenderBezier(BezierRangeStrip strip, Color color, Color color2) {
			GRealRect2D boundingRectangle = strip.GetBoundingRectangle();
			boundingRectangle.Inflate(1, 1);
			RectangleF bounds = new RectangleF((float)boundingRectangle.Left, (float)boundingRectangle.Top, (float)boundingRectangle.Width, (float)boundingRectangle.Height);
			renderer.SetClipping(bounds, CombineMode.Intersect);
			RenderBezier(strip, bounds, color, color2);
			renderer.RestoreClipping();
		}
		void IGradientPainter.RenderCircle(PointF center, float radius, Color color, Color color2) {
			RectangleF boundingRectangle = new RectangleF(center.X, center.Y, 0, 0);
			boundingRectangle.Inflate(radius + 1, radius + 1);
			renderer.SetClipping(boundingRectangle, CombineMode.Intersect);
			RenderCircle(center, radius, color, color2);
			renderer.RestoreClipping();
		}
		void IGradientPainter.RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			RectangleF boundingRectangle = new RectangleF(center.X, center.Y, 0, 0);
			boundingRectangle.Inflate(semiAxisX + 1.0f, semiAxisY + 1.0f);
			renderer.SetClipping(boundingRectangle, CombineMode.Intersect);
			RenderEllipse(center, semiAxisX, semiAxisY, color, color2);
			renderer.RestoreClipping();
		}
		void IGradientPainter.RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF boundingRectangle = new RectangleF(center.X, center.Y, 0, 0);
			boundingRectangle.Inflate(majorSemiAxis + 1, minorSemiAxis + 1);
			renderer.SetClipping(boundingRectangle, CombineMode.Intersect);
			RenderPie(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2);
			renderer.RestoreClipping();
		}
		void IGradientPainter.RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			RenderPath(path, gradientRect, color, color2);
		}
		#endregion
		#region IGradientPainter Graphics Commands Adoption
		GraphicsCommand IGradientPainter.CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return CreateRectangleGraphicsCommand(rect, gradientRect, color, color2);
		}
		GraphicsCommand IGradientPainter.CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			boundRectangle = ZPlaneRectangle.Inflate(boundRectangle, 1, 1);
			GraphicsCommand command = new IntersectClippingGraphicsCommand(boundRectangle);
			command.AddChildCommand(CreateGraphicsCommand(vertices, boundRectangle, color, color2));
			return command;
		}
		GraphicsCommand IGradientPainter.CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			ZPlaneRectangle rect = ZPlaneRectangle.Inflate(new ZPlaneRectangle(new DiagramPoint(center.X, center.Y), 0, 0), majorSemiAxis + 1, minorSemiAxis + 1);
			GraphicsCommand command = new IntersectClippingGraphicsCommand(rect);
			command.AddChildCommand(CreatePieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2));
			return command;
		}
		#endregion
		#region IGradientPainter Miscellaneous Adoption
		void IGradientPainter.FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			if (!polygon.Rect.AreWidthAndHeightPositive())
				return;
			Rectangle rect = MathUtils.StrongRound(polygon.Rect);
			RectangleF oldClip = gr.ClipBounds;
			if (!(polygon is RectanglePolygon)) {
				rect = GraphicUtils.InflateRect(rect, 1, 1);
				polygon.Rect = (RectangleF)rect;
			}
			gr.SetClip(Rectangle.Intersect(Rectangle.Round(oldClip), rect));
			try {
				if (polygon is RectanglePolygon) {
					rect = GraphicUtils.InflateRect(rect, 1, 1);
					polygon.Rect = (RectangleF)rect;
				}
				FillPolygon(gr, polygon, color, color2);
			}
			finally {
				gr.SetClip(oldClip);
			}
		}
		PlanePolygon[] IGradientPainter.FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2) {
			if (color == color2) {
				polygon.SameColors = true;
				polygon.Color = color;
				return new PlanePolygon[] { polygon };
			}
			else {
				polygon.SameColors = false;
				return FillPlanePolygon(polygon, gradientRect, color, color2);
			}
		}
		#endregion
		#region Direct Rendering Methods
		protected abstract void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2);
		protected abstract void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2);
		protected abstract void RenderCircle(PointF center, float radius, Color color, Color color2);
		protected abstract void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2);
		protected abstract void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		protected abstract void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		#endregion
		#region Graphics Commands
		protected abstract GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected abstract GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2);
		protected abstract GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		#endregion
		protected abstract void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2);
		protected abstract Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2);
	}
	public abstract class GradientPainter : GradientPainterBase {
		protected GradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected abstract Brush CreateBrush(IPolygon polygon, Color color, Color color2);
		protected override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			using (Brush brush = CreateBrush(polygon, color, color2))
				using (GraphicsPath path = polygon.GetPath())
					gr.FillPath(brush, path);
		}
		protected RectangleF CorrectPieGradientRectangle(RectangleF rect) {
			if (!rect.AreWidthAndHeightPositive())
				return Rectangle.Empty;
			if (renderer.Transform.Elements[3] == -1)
				rect.Y++;
			else
				rect.Y--;
			rect.Height += 2;
			rect.X--;
			rect.Width += 2;
			return rect;
		}
	}	
	public class TopToBottomGradientPainter : GradientPainter {
		public TopToBottomGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color, color2, LinearGradientMode.Vertical);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			return GraphicUtils.CalcGradientColor(color, color2, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color, color2, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2, LinearGradientMode.Vertical);
		}
		#endregion
	}
	public class BottomToTopGradientPainter : GradientPainter {
		public BottomToTopGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color2, color, LinearGradientMode.Vertical);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			return GraphicUtils.CalcGradientColor(color2, color, k);
		}
		#region Direct Rendefing Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color2, color, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color2, color, LinearGradientMode.Vertical);
		}
		#endregion
	}
	public class LeftToRightGradientPainter : GradientPainter {
		public LeftToRightGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			double k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(color2, color, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color, color2, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2, LinearGradientMode.Horizontal);
		}
		#endregion
	}
	public class RightToLeftGradientPainter : GradientPainter {
		public RightToLeftGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			double k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(color, color2, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color2, color, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color2, color, LinearGradientMode.Horizontal);
		}
		#endregion
	}
	public class TopLeftToBottomRightGradientPainter : GradientPainter {
		public TopLeftToBottomRightGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color color3 = GraphicUtils.CalcGradientColor(color, color2, 0.5);
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			Color c1 = GraphicUtils.CalcGradientColor(color, color3, k);
			Color c2 = GraphicUtils.CalcGradientColor(color3, color2, k);
			k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(c2, c1, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2, LinearGradientMode.ForwardDiagonal);
		}
		#endregion
	}
	public class BottomRightToTopLeftGradientPainter : GradientPainter {
		public BottomRightToTopLeftGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color color3 = GraphicUtils.CalcGradientColor(color, color2, 0.5);
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			Color c1 = GraphicUtils.CalcGradientColor(color3, color, k);
			Color c2 = GraphicUtils.CalcGradientColor(color2, color3, k);
			k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(c1, c2, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color2, color, LinearGradientMode.ForwardDiagonal);
		}
		#endregion
	}
	public class TopRightToBottomLeftGradientPainter : GradientPainter {
		public TopRightToBottomLeftGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color color3 = GraphicUtils.CalcGradientColor(color, color2, 0.5);
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			Color c1 = GraphicUtils.CalcGradientColor(color, color3, k);
			Color c2 = GraphicUtils.CalcGradientColor(color3, color2, k);
			k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(c1, c2, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color, color2, LinearGradientMode.BackwardDiagonal);
		}
		#endregion
	}
	public class BottomLeftToTopRightGradientPainter : GradientPainter {
		public BottomLeftToTopRightGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			return new LinearGradientBrush(polygon.Rect, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color color3 = GraphicUtils.CalcGradientColor(color, color2, 0.5);
			double k = (gradientRect.Top - vertex.Y) / gradientRect.Height;
			Color c1 = GraphicUtils.CalcGradientColor(color3, color, k);
			Color c2 = GraphicUtils.CalcGradientColor(color2, color3, k);
			k = (gradientRect.Right - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(c2, c1, k);
		}
		#region Direct Rendering Methods
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillBezier(strip, boundRectangle, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			RectangleF gradient = CorrectPieGradientRectangle(gradientBounds);
			using (GraphicsPath path = GraphicUtils.CreatePieGraphicsPath(new GRealPoint2D(center.X, center.Y), majorSemiAxis, minorSemiAxis, holePercent, startAngle, sweepAngle))
				renderer.FillPath(path, gradient, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new GradientPolygonGraphicsCommand(vertices, boundRectangle, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new GradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, gradientBounds, color2, color, LinearGradientMode.BackwardDiagonal);
		}
		#endregion
	}
	public abstract class CenterHorizontalGradientPainter : GradientPainterBase {
		public CenterHorizontalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		void CalcHalfRectangles(ZPlaneRectangle rect, out ZPlaneRectangle leftRect, out ZPlaneRectangle rightRect) {
			int width = Convert.ToInt32(rect.Width);
			int leftHalf = width / 2 + 1;
			int rightHalf = width - leftHalf;
			if (rightHalf < 0) 
				rightHalf = 0;
			leftRect = new ZPlaneRectangle(rect.Location, leftHalf, rect.Height);
			rightRect = new ZPlaneRectangle(new DiagramPoint(rect.Location.X + leftHalf, rect.Location.Y), rightHalf, rect.Height);
		}
		void FillLeftRectangle(Graphics gr, Rectangle rect, Color color, Color color2) {
			using(Brush brush = CreateLeftBrush(rect, color, color2)) {
				gr.FillRectangle(brush, rect);
			}
		}
		void FillRightRectangle(Graphics gr, Rectangle rect, Color color, Color color2) {
			RectangleF oldClip = gr.ClipBounds;
			gr.SetClip(Rectangle.Intersect(GraphicUtils.RoundRectangle(oldClip), rect));			
			try {			
				Rectangle r = new Rectangle(rect.X - 1, rect.Y, rect.Width + 1, rect.Height);
				Brush brush = CreateRightBrush(r, color, color2);
				using(brush) {
					gr.FillRectangle(brush, r);
				}
			} finally {
				gr.SetClip(oldClip);
			}
		}
		protected override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			ZPlaneRectangle leftRect, rightRect;
			CalcHalfRectangles((ZPlaneRectangle)polygon.Rect, out leftRect, out rightRect);
			FillRightRectangle(gr, (Rectangle)(ZPlaneRectangle)rightRect, color, color2);
			FillLeftRectangle(gr, (Rectangle)(ZPlaneRectangle)leftRect, color, color2);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color c, c2;
			GetColors(color, color2, out c, out c2);
			double k = 2 * Math.Abs(gradientRect.Center.X - vertex.X) / gradientRect.Width;
			return GraphicUtils.CalcGradientColor(c2, c, k);
		}
		protected abstract void GetColors(Color color, Color color2, out Color c, out Color c2);
		protected abstract Brush CreateLeftBrush(Rectangle rect, Color color, Color color2);
		protected abstract Brush CreateRightBrush(Rectangle rect, Color color, Color color2);
		#region Direct Rendering Methods
		protected abstract void RenderLeftRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderRightRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderLeftEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2);
		protected abstract void RenderRightEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2);
		protected abstract void RenderLeftPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderRightPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(gradientRect, SplitRectangleMode.Horizontal, true);
			if (parts.Length == 0)
				return;
			parts[1].X--;
			parts[1].Width++;
			renderer.SetClipping(parts[1], CombineMode.Intersect);
			RenderRightRectangle(renderer, RectangleF.Intersect(rect, parts[1]), parts[1], color, color2);
			renderer.RestoreClipping();
			RenderLeftRectangle(renderer, RectangleF.Intersect(rect, parts[0]), parts[0], color, color2);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(GraphicUtils.CalcBoundRectangle(new GRealPoint2D(center.X, center.Y), semiAxisX, semiAxisY), SplitRectangleMode.Horizontal, true);
			renderer.SetClipping(new RectangleF(parts[1].Left, parts[1].Bottom - 1, parts[1].Width + 1, parts[1].Height + 2), CombineMode.Intersect);
			RenderRightEllipse(renderer, center, semiAxisX, semiAxisY, parts[1], color, color2);
			renderer.RestoreClipping();
			renderer.SetClipping(new RectangleF(parts[0].Left - 1, parts[0].Bottom - 1, parts[0].Width + 2, parts[0].Height + 2), CombineMode.Intersect);
			RenderLeftEllipse(renderer, center, semiAxisX, semiAxisY, parts[1], color, color2);
			renderer.RestoreClipping();
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(gradientRect, SplitRectangleMode.Horizontal, true);
			parts[0].Width++;
			renderer.SetClipping(new RectangleF(parts[1].Left, parts[1].Bottom - 1, parts[1].Width + 1, parts[1].Height + 2));
			parts[1].Offset(-1, 0);
			parts[1].Width++;
			RenderRightPath(renderer, path, parts[1], color, color2);
			renderer.RestoreClipping();
			renderer.SetClipping(new RectangleF(parts[0].Left - 1, parts[0].Bottom - 1, parts[0].Width + 1, parts[0].Height + 2));
			RenderLeftPath(renderer, path, parts[0], color, color2);
			renderer.RestoreClipping();
		}
		#endregion
		#region Graphics Commands
		protected abstract GraphicsCommand CreateLeftRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected abstract GraphicsCommand CreateRightRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			ZPlaneRectangle leftRect, rightRect;
			CalcHalfRectangles(gradientRect, out leftRect, out rightRect);
			GraphicsCommand command = new ContainerGraphicsCommand();
			IntersectClippingGraphicsCommand rightCommand = new IntersectClippingGraphicsCommand(rightRect);
			rightRect = new ZPlaneRectangle(new DiagramPoint(rightRect.Location.X - 1, rightRect.Location.Y), rightRect.Width + 1, rightRect.Height);
			rightCommand.AddChildCommand(CreateRightRectangleGraphicsCommand((ZPlaneRectangle)Rectangle.Intersect((Rectangle)rect, (Rectangle)rightRect), rightRect, color, color2));
			command.AddChildCommand(rightCommand);
			command.AddChildCommand(CreateLeftRectangleGraphicsCommand((ZPlaneRectangle)Rectangle.Intersect((Rectangle)rect, (Rectangle)leftRect), leftRect, color, color2));
			return command;
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return null;
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return null;
		}
		#endregion
	}
	public class FromCenterHorizontalGradientPainter : CenterHorizontalGradientPainter {
		public FromCenterHorizontalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateLeftBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override Brush CreateRightBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void GetColors(Color color, Color color2, out Color c, out Color c2) {
			c = color2;
			c2 = color;
		}
		#region Direct Rendering Methods
		protected override void RenderLeftRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderRightRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderLeftEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderRightEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderLeftPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Horizontal);		  
		}
		protected override void RenderRightPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateLeftRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreateRightRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		#endregion
	}
	public class ToCenterHorizontalGradientPainter : CenterHorizontalGradientPainter {
		public ToCenterHorizontalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateLeftBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override Brush CreateRightBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void GetColors(Color color, Color color2, out Color c, out Color c2) {
			c = color;
			c2 = color2;
		}
		#region Direct Rendering Methods
		protected override void RenderLeftRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderRightRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderLeftEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderRightEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, color2, color, LinearGradientMode.Horizontal);
		}
		protected override void RenderLeftPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override void RenderRightPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateLeftRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Horizontal);
		}
		protected override GraphicsCommand CreateRightRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Horizontal);
		}
		#endregion
	}
	public abstract class CenterVerticalGradientPainter : GradientPainterBase {
		public CenterVerticalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		void CalcHalfRectangles(ZPlaneRectangle rect, out ZPlaneRectangle topRect, out ZPlaneRectangle bottomRect) {
			int height = Convert.ToInt32(rect.Height);
			int topHalf = height / 2 + 1;
			int bottomHalf = height - topHalf;
			if (bottomHalf < 0) 
				bottomHalf = 0;
			topRect = new ZPlaneRectangle(rect.Location, rect.Width, topHalf);
			bottomRect = new ZPlaneRectangle(new DiagramPoint(rect.Location.X, rect.Location.Y + topHalf), rect.Width, bottomHalf);
		}
		void FillTopRectangle(Graphics gr, Rectangle rect, Color color, Color color2) {
			using(Brush brush = CreateTopBrush(rect, color, color2)) {
				gr.FillRectangle(brush, rect);
			}
		}
		void FillBottomRectangle(Graphics gr, Rectangle rect, Color color, Color color2) {
			RectangleF oldClip = gr.ClipBounds;
			gr.SetClip(Rectangle.Intersect(GraphicUtils.RoundRectangle(oldClip), rect));			
			try {			
				Rectangle r = new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height + 1);
				Brush brush = CreateBottomBrush(r, color, color2);
				using(brush) {
					gr.FillRectangle(brush, r);
				}
			} finally {
				gr.SetClip(oldClip);
			}
		}
		protected abstract void GetColors(Color color, Color color2, out Color c, out Color c2);
		protected abstract Brush CreateTopBrush(Rectangle rect, Color color, Color color2);
		protected abstract Brush CreateBottomBrush(Rectangle rect, Color color, Color color2);
		protected override void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2) {
			ZPlaneRectangle topRect, bottomRect;
			CalcHalfRectangles((ZPlaneRectangle)polygon.Rect, out topRect, out bottomRect);
			FillBottomRectangle(gr, (Rectangle)(ZPlaneRectangle)bottomRect, color, color2);
			FillTopRectangle(gr, (Rectangle)(ZPlaneRectangle)topRect, color, color2);
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			Color c, c2;
			GetColors(color, color2, out c, out c2);
			double k = 2 * Math.Abs(gradientRect.Center.Y - vertex.Y) / gradientRect.Height;
			return GraphicUtils.CalcGradientColor(c2, c, k);
		}
		#region Direct Rendering Methods
		protected abstract void RenderTopRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderBottomRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderTopEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2);
		protected abstract void RenderBottomEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2);
		protected abstract void RenderTopPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		protected abstract void RenderBottomPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(gradientRect, SplitRectangleMode.Vertical, false);
			renderer.SetClipping(parts[1], CombineMode.Intersect);
			RenderBottomRectangle(renderer, RectangleF.Intersect(rect, parts[1]), parts[1], color, color2);
			renderer.RestoreClipping();
			RenderTopRectangle(renderer, RectangleF.Intersect(rect, parts[0]), parts[0], color, color2);
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(GraphicUtils.CalcBoundRectangle(new GRealPoint2D(center.X, center.Y), semiAxisX, semiAxisY), SplitRectangleMode.Vertical, true);
			renderer.SetClipping(new RectangleF(parts[0].Left - 1, parts[0].Bottom - 1, parts[0].Width + 2, parts[0].Height + 2), CombineMode.Intersect);
			RenderTopEllipse(renderer, center, semiAxisX, semiAxisY, parts[0], color, color2);
			renderer.RestoreClipping();
			renderer.SetClipping(new RectangleF(parts[1].Left - 1, parts[1].Bottom, parts[1].Width + 2, parts[1].Height + 1), CombineMode.Intersect);
			RenderBottomEllipse(renderer, center, semiAxisX, semiAxisX, parts[1], color, color2);
			renderer.RestoreClipping();
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			RectangleF[] parts = GraphicUtils.SplitRectangle(gradientRect, SplitRectangleMode.Vertical, true);
			parts[0].Height++;
			renderer.SetClipping(new RectangleF(parts[0].Left - 1, parts[0].Bottom - 1, parts[0].Width + 2, parts[0].Height + 1), CombineMode.Intersect);
			RenderTopPath(renderer, path, parts[0], color, color2);
			renderer.RestoreClipping();
			renderer.SetClipping(new RectangleF(parts[1].Left - 1, parts[1].Bottom, parts[1].Width + 2, parts[1].Height + 1), CombineMode.Intersect);
			parts[1].Offset(0, -1);
			parts[1].Height++;
			RenderBottomPath(renderer, path, parts[1], color, color2);
			renderer.RestoreClipping();
		}
		#endregion
		#region Graphics Commands
		protected abstract GraphicsCommand CreateTopRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected abstract GraphicsCommand CreateBottomRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			ZPlaneRectangle topRect, bottomRect;
			CalcHalfRectangles(gradientRect, out topRect, out bottomRect);
			IntersectClippingGraphicsCommand bottomCommand = new IntersectClippingGraphicsCommand(bottomRect);
			bottomRect = new ZPlaneRectangle(new DiagramPoint(bottomRect.Location.X, bottomRect.Location.Y - 1), bottomRect.Width, bottomRect.Height + 1);
			bottomCommand.AddChildCommand(CreateBottomRectangleGraphicsCommand((ZPlaneRectangle)Rectangle.Intersect((Rectangle)rect, (Rectangle)bottomRect), bottomRect, color, color2));
			GraphicsCommand command = new ContainerGraphicsCommand();
			command.AddChildCommand(bottomCommand);
			command.AddChildCommand(CreateTopRectangleGraphicsCommand((ZPlaneRectangle)Rectangle.Intersect((Rectangle)rect, (Rectangle)topRect), topRect, color, color2));
			return command;
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return null;
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return null;
		}
		#endregion
	}
	public class FromCenterVerticalGradientPainter : CenterVerticalGradientPainter {
		public FromCenterVerticalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateTopBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color2, color, LinearGradientMode.Vertical);
		}
		protected override Brush CreateBottomBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color, color2, LinearGradientMode.Vertical);
		}
		protected override void GetColors(Color color, Color color2, out Color c, out Color c2) {
			c = color2;
			c2 = color;
		}
		#region Direct Rendering Methods
		protected override void RenderTopRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderBottomRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderTopEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, gradientBounds, color, color2, LinearGradientMode.Vertical); 
		}
		protected override void RenderBottomEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, gradientBounds, color2, color, LinearGradientMode.Vertical); 
		}
		protected override void RenderTopPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderBottomPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateTopRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle  gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreateBottomRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		#endregion
	}
	public class ToCenterVerticalGradientPainter : CenterVerticalGradientPainter {
		public ToCenterVerticalGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Brush CreateTopBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color, color2, LinearGradientMode.Vertical);
		}
		protected override Brush CreateBottomBrush(Rectangle rect, Color color, Color color2) {
			return new LinearGradientBrush(rect, color2, color, LinearGradientMode.Vertical);
		}
		protected override void GetColors(Color color, Color color2, out Color c, out Color c2) {
			c = color;
			c2 = color2;
		}
		#region Direct Rendering Methods
		protected override void RenderTopRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderBottomRectangle(IRenderer renderer, RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillRectangle(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderTopEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, gradientBounds, color2, color, LinearGradientMode.Vertical);
		}
		protected override void RenderBottomEllipse(IRenderer renderer, PointF center, float semiAxisX, float semiAxisY, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, gradientBounds, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderTopPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override void RenderBottomPath(IRenderer renderer, GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			renderer.FillPath(path, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateTopRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color, color2, LinearGradientMode.Vertical);
		}
		protected override GraphicsCommand CreateBottomRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return new GradientRectangleGraphicsCommand(rect, gradientRect, color2, color, LinearGradientMode.Vertical);
		}
		#endregion
	}
	public abstract class RadialGradientPainter : GradientPainter {
		public RadialGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected abstract Color GetCenterColor(Color color, Color color2);
		protected abstract Color GetSurroundColor(Color color, Color color2);
		protected override Brush CreateBrush(IPolygon polygon, Color color, Color color2) {
			PathGradientBrush brush = new PathGradientBrush(polygon.GetPath());
			brush.CenterColor = GetCenterColor(color, color2);
			brush.SurroundColors = new Color[] {GetSurroundColor(color, color2)};
			brush.CenterPoint = new Point(
				Convert.ToInt32(polygon.Rect.Location.X + polygon.Rect.Width / 2), 
				Convert.ToInt32(polygon.Rect.Location.Y + polygon.Rect.Height / 2));
			return brush;
		}
		protected override Color CalcVertexColor(Vertex vertex, PlaneRectangle gradientRect, Color color, Color color2) {
			return color;
		}
		#region Direct Rendering Commands
		protected override void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2) {
		}
		protected override void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2) {
			renderer.FillPolygonGradient(vertices, boundRectangle, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		protected override void RenderBezier(BezierRangeStrip strip, RectangleF boundRectangle, Color color, Color color2) {
		}
		protected override void RenderCircle(PointF center, float radius, Color color, Color color2) {
			renderer.FillCircle(center, radius, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		protected override void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2) {
			renderer.FillEllipse(center, semiAxisX, semiAxisY, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		protected override void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			renderer.FillPie(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		protected override void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2) {
			if (path != null)
				path.Dispose();
		}
		#endregion
		#region Graphics Commands
		protected override GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2) {
			return null;
		}
		protected override GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2) {
			return new CenterGradientPolygonGraphicsCommand(vertices, boundRectangle, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		protected override GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2) {
			return new CenterGradientPieGraphicsCommand(center, majorSemiAxis, minorSemiAxis, startAngle, sweepAngle, depth, holePercent, GetCenterColor(color, color2), GetSurroundColor(color, color2));
		}
		#endregion
	}
	class FromCenterGradientPainter : RadialGradientPainter {
		public FromCenterGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Color GetCenterColor(Color color1, Color color2) {
			return color1;
		}
		protected override Color GetSurroundColor(Color color1, Color color2) {
			return color2;
		}
	}
	class ToCenterGradientPainter : RadialGradientPainter {
		public ToCenterGradientPainter(IRenderer renderer) : base(renderer) {
		}
		protected override Color GetCenterColor(Color color1, Color color2) {
			return color2;
		}
		protected override Color GetSurroundColor(Color color1, Color color2) {
			return color1;
		}
	}
}
