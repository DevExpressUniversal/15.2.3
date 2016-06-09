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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class PdfGourandShadedTriangleMeshPainter : PdfMeshShadingPainter {
		static void DrawGradientLine(Graphics graphics, PointF point1, PointF point2, Color color1, Color color2) {
			using (LinearGradientBrush brush = new LinearGradientBrush(point1, point2, color1, color2)) 
				using (Pen pen = new Pen(brush))
					graphics.DrawLine(pen, point1, point2);
		}
		readonly PdfRenderingTriangle[] triangles;
		public PdfGourandShadedTriangleMeshPainter(PdfGourandShadedTriangleMesh shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
				: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) { 
			IShadingColorConverter colorConverter = ColorConverter;
			IShadingCoordsConverter coordsConverter = CoordsConverter;
			IList<PdfTriangle> shadingTriangles = shading.Triangles;
			int triangleCount = shadingTriangles.Count;
			triangles = new PdfRenderingTriangle[triangleCount];
			for (int i = 0; i < triangleCount; i++) {
				PdfTriangle shadingTriangle = shadingTriangles[i];
				PdfVertex vertex1 = shadingTriangle.Vertex1;
				PdfVertex vertex2 = shadingTriangle.Vertex2;
				PdfVertex vertex3 = shadingTriangle.Vertex3;
				triangles[i] = new PdfRenderingTriangle(new PdfRenderingVertex(coordsConverter.Convert(vertex1.Point), colorConverter.Convert(vertex1.Color.Components)), 
														new PdfRenderingVertex(coordsConverter.Convert(vertex2.Point), colorConverter.Convert(vertex2.Color.Components)), 
														new PdfRenderingVertex(coordsConverter.Convert(vertex3.Point), colorConverter.Convert(vertex3.Color.Components))); 
			}
		}
		protected override void DrawMesh(Graphics graphics) { 
			foreach (PdfRenderingTriangle triangle in triangles) { 
				PdfRenderingVertex vertex1 = triangle.Vertex1;
				PdfRenderingVertex vertex2 = triangle.Vertex2;
				PdfRenderingVertex vertex3 = triangle.Vertex3;
				PointF point1 = vertex1.Point;
				PointF point2 = vertex2.Point;
				PointF point3 = vertex3.Point;
				Color[] colors = new Color[] { vertex1.Color, vertex2.Color, vertex3.Color };
				Color color3 = colors[2];
				if (!point1.Equals(point2) || !point2.Equals(point3))  
				if (Math.Abs(((point1.X - point3.X) * (point2.Y - point3.Y) - (point2.X - point3.X) * (point1.Y - point3.Y))) < 2)  
					if (point1.Equals(point3))
						DrawGradientLine(graphics, point2, point3, colors[1], color3);
					else
						DrawGradientLine(graphics, point1, point3, colors[0], color3);
				else 
					using (GraphicsPath path = new GraphicsPath()) { 
						PointF[] points = new PointF[] { point1, point2, point3 };
						path.AddLines(points);
						using (PathGradientBrush brush = new PathGradientBrush(points)) { 
							brush.CenterColor = color3;
							brush.CenterPoint = point3;
							brush.SurroundColors = colors;
							graphics.FillPath(brush, path);
						}
					}
			}
		}
	}
}
