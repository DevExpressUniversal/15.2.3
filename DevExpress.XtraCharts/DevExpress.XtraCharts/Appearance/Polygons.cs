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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class RectanglePolygon : IPolygon {
		RectangleF rect;
		public RectangleF Rect { get { return rect; } set { rect = value; } }
		public LineStrip Vertices { get { return null; } }
		public RectanglePolygon(RectangleF rect) {
			this.rect = rect;
		}
		public GraphicsPath GetPath() {
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(rect);
			return path;
		}
		public void RenderShadow(IRenderer renderer, Shadow shadow, int shadowSize) {
			shadow.Render(renderer, rect, shadowSize);
		}
		public void Render(IRenderer renderer, FillOptionsBase fillOptions, Color color, Color color2, Color borderColor, int borderThickness) {
			fillOptions.RenderRectangle(renderer, (RectangleF)rect, (RectangleF)rect, color, color2);
			if (borderThickness > 0)
				renderer.DrawRectangle((RectangleF)rect, borderColor, borderThickness);
		}
	}
	public class CirclePolygon : IPolygon {
		RectangleF rect;
		GRealPoint2D center;
		float radius;
		public GRealPoint2D Center { get { return center; } }
		public float Radius { get { return radius; } }
		public RectangleF Rect { get { return rect; } set { rect = value; } }
		public LineStrip Vertices { get { return null; } }
		public CirclePolygon(GRealPoint2D center, float radius) {
			this.center = center;
			this.radius = radius;
			rect = new RectangleF((float)center.X, (float)center.Y, 0, 0);
			rect.Inflate(radius, radius);
			MathUtils.StrongRound(rect);
		}
		public GraphicsPath GetPath() {
			GraphicsPath path = new GraphicsPath();
			if (radius > 0)
				path.AddEllipse((float)center.X - radius, (float)center.Y - radius, radius * 2.0f, radius * 2.0f);
			return path;
		}
		public void RenderShadow(IRenderer renderer, Shadow shadow, int shadowSize) {
			shadow.Render(renderer, center, radius, shadowSize);
		}
		public void Render(IRenderer renderer, FillOptionsBase fillOptions, Color color, Color color2, Color borderColor, int borderThickness) {
			fillOptions.RenderCircle(renderer, new PointF((float)center.X, (float)center.Y), radius, color, color2);
			if (borderThickness > 0)
				renderer.DrawCircle(new Point((int)center.X, (int)center.Y), (int)radius, borderColor, borderThickness);
		}
	}
	public class VariousPolygon : IPolygon {
		RectangleF rect;
		LineStrip vertices;
		public RectangleF Rect { get { return rect; } set { rect = value; } }
		public LineStrip Vertices { get { return vertices; } }
		public VariousPolygon(LineStrip vertices, RectangleF rect) {
			this.vertices = vertices;
			this.rect = rect;
		}
		public virtual GraphicsPath GetPath() {
			GraphicsPath path = new GraphicsPath();
			if (vertices.Count > 1)
				path.AddLines(StripsUtils.Convert(vertices));
			return path;
		}
		public void RenderShadow(IRenderer renderer, Shadow shadow, int shadowSize) {
			shadow.Render(renderer, this, shadowSize);
		}
		public void Render(IRenderer renderer, Color color) {
			renderer.FillPolygon(vertices, color);
		}
		public void Render(IRenderer renderer, FillOptionsBase fillOptions, Color color, Color color2, Color borderColor, int borderThickness) {
			fillOptions.Render(renderer, vertices, rect, color, color2);
			if (borderThickness > 0)
				renderer.DrawPolygon(vertices, borderColor, borderThickness);
		}
		internal static VariousPolygon Intersect(VariousPolygon polygon, Rectangle clipBounds) {
			RectangleF intersect = RectangleF.Intersect(polygon.rect, clipBounds);
			return new VariousPolygon(new LineStrip(new List<GRealPoint2D> { new GRealPoint2D(intersect.Left, intersect.Top), new GRealPoint2D(intersect.Left, intersect.Bottom), new GRealPoint2D(intersect.Right, intersect.Top), new GRealPoint2D(intersect.Right, intersect.Bottom) }), intersect);
		}
	}
}
