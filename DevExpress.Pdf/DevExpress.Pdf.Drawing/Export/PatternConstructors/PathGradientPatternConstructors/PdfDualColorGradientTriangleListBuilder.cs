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
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfDualColorGradientTriangleListBuilder : PdfTriangleListBuilder {
		const double lineStep = 0.5;
		static PdfPoint GetPoint(double position, PdfPoint startPoint, PdfPoint endPoint) {
			double x = startPoint.X + (endPoint.X - startPoint.X) * (position);
			double y = startPoint.Y + (endPoint.Y - startPoint.Y) * (position);
			return new PdfPoint(x, y);
		}
		readonly PdfColor centerColor;
		readonly PdfBlend blend;
		public PdfDualColorGradientTriangleListBuilder(PdfPathGradientBrush brush)
			: base(brush.CenterPoint, brush.SurroundColors) {
			blend = brush.Blend;
			centerColor = PdfGraphicsConverter.GdiToPdfColor(brush.CenterColor);
		}
		protected override IList<PdfRange> ComponentsRange { get { return new[] { PdfColorRange, PdfColorRange, PdfColorRange }; } }
		protected override IEnumerable<PdfTriangle> CreateTreangles(PdfPoint startPoint, PdfPoint endPoint, Color startColor, Color endColor) {
			PdfColor pdfStartColor = PdfGraphicsConverter.GdiToPdfColor(startColor);
			PdfColor pdfEndColor = PdfGraphicsConverter.GdiToPdfColor(endColor);
			List<PdfTriangle> triangles = new List<PdfTriangle>();
			PdfPoint centerPoint = CenterPoint;
			PdfVertex prevStartVertex = new PdfVertex(startPoint, BlendColor(pdfStartColor, centerColor, blend.Factors[0]));
			PdfVertex prevEndVertex = new PdfVertex(endPoint, BlendColor(pdfEndColor, centerColor, blend.Factors[0]));
			for (int i = 0; i < blend.Positions.Length - 2; i++) {
				PdfPoint newStartPoint = GetPoint(blend.Positions[i + 1], startPoint, CenterPoint);
				PdfPoint newEndPoint = GetPoint(blend.Positions[i + 1], endPoint, CenterPoint);
				PdfColor newStartColor = BlendColor(pdfStartColor, centerColor, blend.Factors[i + 1]);
				PdfColor newEndColor = BlendColor(pdfEndColor, centerColor, blend.Factors[i + 1]);
				PdfVertex nextStartVertex = new PdfVertex(newStartPoint, newStartColor);
				PdfVertex nextEndVertex = new PdfVertex(newEndPoint, newEndColor);
				triangles.AddRange(DrawPolygon(prevStartVertex, prevEndVertex, nextStartVertex, nextEndVertex));
				prevStartVertex = nextStartVertex;
				prevEndVertex = nextEndVertex;
			}
			PdfColor centerVertexColor = BlendColor(BlendColor(pdfStartColor, pdfEndColor, 0.5), centerColor, blend.Factors[blend.Factors.Length - 1]);
			triangles.Add(new PdfTriangle(prevStartVertex, new PdfVertex(CenterPoint, centerVertexColor), prevEndVertex));
			return triangles;
		}
		IEnumerable<PdfTriangle> DrawPolygon(PdfVertex a, PdfVertex b, PdfVertex c, PdfVertex d) {
			List<PdfTriangle> triangles = new List<PdfTriangle>();
			PdfPoint prevStartPointLoc = a.Point;
			PdfPoint prevEndPointLoc = b.Point;
			PdfColor prevStartColorLoc = a.Color;
			PdfColor prevEndColorLoc = b.Color;
			double max = PdfMathUtils.Max(PdfMathUtils.CalcDistance(a.Point, c.Point), PdfMathUtils.CalcDistance(b.Point, d.Point));
			double polygonsCount = Math.Ceiling(max / lineStep);
			PdfVertex prevStartVertex = a;
			PdfVertex prevEndVertex = b;
			for (int j = 1; j <= (int)polygonsCount; j++) {
				double factor = j / polygonsCount;
				PdfVertex newStartVertex = new PdfVertex(GetPoint(factor, a.Point, c.Point), BlendColor(a.Color, c.Color, factor));
				PdfVertex newEndVertex = new PdfVertex(GetPoint(factor, b.Point, d.Point), BlendColor(b.Color, d.Color, factor));
				triangles.Add(new PdfTriangle(newStartVertex, prevStartVertex, prevEndVertex));
				triangles.Add(new PdfTriangle(newStartVertex, newEndVertex, prevEndVertex));
				prevStartVertex = newStartVertex;
				prevEndVertex = newEndVertex;
			}
			return triangles;
		}
		protected override PdfObjectList<PdfCustomFunction> CreateFunction(PdfDocumentCatalog documentCatalog) {
			return null;
		}
	}
}
