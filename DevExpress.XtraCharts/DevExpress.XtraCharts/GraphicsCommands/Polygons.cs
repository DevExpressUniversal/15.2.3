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
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public abstract class PolygonGraphicsCommandBase : GraphicsCommand {
		LineStrip vertices;
		protected internal LineStrip Vertices { get { return vertices; } }
		public PolygonGraphicsCommandBase(LineStrip vertices) : base() {
			this.vertices = vertices;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public class BoundedPolygonGraphicsCommand : PolygonGraphicsCommandBase {
		Color color;
		int thickness;
		public BoundedPolygonGraphicsCommand(LineStrip vertices, Color color, int thickness) : base(vertices) {
			this.color = color;
			this.thickness = thickness;
		}
	}
	public class SolidPolygonGraphicsCommand : PolygonGraphicsCommandBase {
		Color color;
		public SolidPolygonGraphicsCommand(LineStrip vertices, Color color) : base(vertices) {
			this.color = color;
		}
	}
	public class GradientPolygonGraphicsCommand : PolygonGraphicsCommandBase {
		PlaneRectangle boundedRectangle;
		Color color;
		Color color2;
		LinearGradientMode gradientMode;
		ZPlaneRectangle BoundedRectangle { get { return boundedRectangle as ZPlaneRectangle; } }
		public GradientPolygonGraphicsCommand(LineStrip vertices, Color color, Color color2, LinearGradientMode gradientMode) : base(vertices) {
			this.color = color;
			this.color2 = color2;
			this.gradientMode = gradientMode;
		}
		public GradientPolygonGraphicsCommand(LineStrip vertices, PlaneRectangle boundedRectangle, Color color, Color color2, LinearGradientMode gradientMode) : this(vertices, color, color2, gradientMode) {
			this.boundedRectangle = boundedRectangle;
		}
	}
	public class CenterGradientPolygonGraphicsCommand : PolygonGraphicsCommandBase {
		static Rectangle GetBoundsForCircle(PointF[] polygon, GRealPoint2D centerPoint) {
			DiagramPoint point1;
			DiagramPoint point2 = new DiagramPoint(centerPoint.X, centerPoint.Y);
			int radius = Int32.MinValue;
			foreach (PointF point in polygon) {
				point1 = new DiagramPoint(point.X, point.Y);
				int length = MathUtils.Ceiling(MathUtils.CalcLength2D(point1, point2));
				if (length > radius)
					radius = length;
			}
			int x = MathUtils.Ceiling(centerPoint.X - radius);
			int y = MathUtils.Ceiling(centerPoint.Y - radius);
			return new Rectangle(x, y, radius * 2, radius * 2);
		}
		PlaneRectangle boundedRectangle;
		Color color;
		Color color2;
		internal ZPlaneRectangle BoundedRectangle { get { return boundedRectangle as ZPlaneRectangle; } }
		public CenterGradientPolygonGraphicsCommand(LineStrip vertices, PlaneRectangle boundedRectangle, Color color, Color color2) : base(vertices) {
			this.boundedRectangle = boundedRectangle;
			this.color = color;
			this.color2 = color2;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
		}
	}
	public class HatchedPolygonGraphicsCommand : PolygonGraphicsCommandBase {
		HatchStyle hatchStyle;
		Color color;
		Color color2;
		public HatchedPolygonGraphicsCommand(LineStrip vertices, HatchStyle hatchStyle, Color color, Color color2) : base(vertices) {
			this.hatchStyle = hatchStyle;
			this.color = color;
			this.color2 = color2;
		}
		public HatchedPolygonGraphicsCommand(LineStrip vertices, HatchStyle hatchStyle, Color color) : this(vertices, hatchStyle, color, Color.Transparent) {
		}
	}
	public class PolygonGraphicsCommand : GraphicsCommand {
		PlanePolygon polygon;
		public PolygonGraphicsCommand(PlanePolygon polygon) {
			if (polygon.Vertices.Length < 3)
				throw new ArgumentException("polygon");
			this.polygon = polygon;
		}
		void DrawVertices() {
			if (polygon.SameNormals)
				GL.Normal3d(polygon.Normal.DX, polygon.Normal.DY, polygon.Normal.DZ);
			if (polygon.SameColors)
				GL.Color4ub(polygon.Color.R, polygon.Color.G, polygon.Color.B, polygon.Color.A);
			for (int i = 0; i < polygon.Vertices.Length; i++) {
				if (!polygon.SameColors) {
					Color color = polygon.Vertices[i].Color;
					GL.Color4ub(color.R, color.G, color.B, color.A);
				}
				if (!polygon.SameNormals) {
					DiagramVector normal = polygon.Vertices[i].Normal;
					GL.Normal3d(normal.DX, normal.DY, normal.DZ);
				}
				DiagramPoint vertex = polygon.Vertices[i];
				GL.Vertex3d(vertex.X, vertex.Y, vertex.Z);
			}
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			int beginCode;
			if (polygon.Vertices.Length == 3)
				beginCode = GL.TRIANGLES;
			else if (polygon.Vertices.Length == 4)
				beginCode = GL.QUADS;
			else
				beginCode = GL.POLYGON;
			GL.Begin(beginCode);
			DrawVertices();
			GL.End();
		}
	}
}
