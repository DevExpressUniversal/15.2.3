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
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfTriangleListBuilder {
		static PdfColorBlend CreateBlend(PdfBlend blend, PdfColor startColor, PdfColor endColor) {
			PdfColorBlend colorBlend = new PdfColorBlend();
			colorBlend.Positions = blend.Positions;
			colorBlend.Colors = new PdfColor[blend.Positions.Length];
			for (int i = 0; i < blend.Factors.Length; i++) {
				colorBlend.Colors[i] = BlendColor(startColor, endColor, blend.Factors[i]);
			}
			return colorBlend;
		}
		public static PdfTriangleListBuilder Create(PdfPathGradientBrush brush) {
			if (brush.InterpolationColors != null)
				return new PdfMulticolorGradientTriangleListBuilder(brush.CenterPoint, brush.InterpolationColors, brush.FocusScales);
			if (brush.SurroundColors.Length == 1 && (brush.FocusScales.X != 0 || brush.FocusScales.Y != 0)) {
				PdfColor startColor = PdfGraphicsConverter.GdiToPdfColor(brush.SurroundColors[0]);
				PdfColor centerColor = PdfGraphicsConverter.GdiToPdfColor(brush.CenterColor);
				return new PdfMulticolorGradientTriangleListBuilder(brush.CenterPoint, CreateBlend(brush.Blend, startColor, centerColor), brush.FocusScales, centerColor);
			}
			return new PdfDualColorGradientTriangleListBuilder(brush);
		}
		protected static PdfColor BlendColor(PdfColor first, PdfColor second, double factor) {
			double[] result = new double[first.Components.Length];
			for (int i = 0; i < first.Components.Length; i++)
				result[i] = first.Components[i] * (1 - factor) + second.Components[i] * (factor);
			return new PdfColor(result);
		}
		static readonly PdfRange pdfColorRange = new PdfRange(0, 1);
		static readonly PdfRange xRange = new PdfRange(short.MinValue, short.MaxValue);
		static readonly PdfRange yRange = new PdfRange(short.MinValue, short.MaxValue);
		protected static PdfRange PdfColorRange { get { return pdfColorRange; } }
		const int bitsPerCoordinate = 16;
		const int bitsPerComponent = 8;
		const int bitsPerFlag = 8;
		const int bezierTrianglesCount = 30;
		static double CalculateBezierCurveCoordinate(double startCoordinate, double controlCoordinate1, double controlCoordinate2, double endCoordinate, double t) {
			return Math.Pow(1 - t, 3) * startCoordinate + 3 * t * Math.Pow(1 - t, 2) * controlCoordinate1 + 3 * Math.Pow(t, 2) * (1 - t) * controlCoordinate2 +
				Math.Pow(t, 3) * endCoordinate;
		}
		readonly List<PdfTriangle> triangles = new List<PdfTriangle>();
		readonly PdfPoint centerPoint;
		readonly PdfColorEnumerator enumerator;
		PdfPoint subpathEndPoint;
		protected abstract IList<PdfRange> ComponentsRange { get; }
		protected PdfPoint CenterPoint { get { return centerPoint; } }
		protected PdfTriangleListBuilder(PdfPoint centerPoint, Color[] surroundColors) {
			this.centerPoint = centerPoint;
			this.enumerator = new PdfColorEnumerator(surroundColors);
		}
		public void AppendLine(PdfPoint startPoint, PdfPoint endPoint) {
			AddTriangle(startPoint, endPoint, enumerator.Current, enumerator.GetNext());
		}
		public void ClosePath(PdfPoint endPoint, Color endColor) {
			AddTriangle(subpathEndPoint, endPoint, enumerator.Current, endColor);
		}
		public void AppendBezier(PdfPoint startPoint, PdfPoint controlPoint1, PdfPoint controlPoint2, PdfPoint EndPoint) {
			PdfPoint triangleStartPoint = startPoint;
			for (double j = 0; j <= 1; j += 1.0 / bezierTrianglesCount) {
				PdfPoint triangleEndPoint = new PdfPoint(CalculateBezierCurveCoordinate(startPoint.X, controlPoint1.X, controlPoint2.X, EndPoint.X, j),
					CalculateBezierCurveCoordinate(startPoint.Y, controlPoint1.Y, controlPoint2.Y, EndPoint.Y, j));
				AddTriangle(triangleStartPoint, triangleEndPoint, enumerator.Current, enumerator.GetNext());
				triangleStartPoint = triangleEndPoint;
			}
		}
		public PdfShading GetShading(PdfDocumentCatalog documentCatalog) {
			IList<PdfRange> componentsRange = ComponentsRange;
			PdfDecodeRange[] cDecodeRange = new PdfDecodeRange[componentsRange.Count];
			for (int i = 0; i < componentsRange.Count; i++)
				cDecodeRange[i] = new PdfDecodeRange(componentsRange[i].Min, componentsRange[i].Max, bitsPerComponent);
			PdfDecodeRange xDecodeRange = new PdfDecodeRange(xRange.Min, xRange.Max, bitsPerCoordinate);
			PdfDecodeRange yDecodeRange = new PdfDecodeRange(yRange.Min, yRange.Max, bitsPerCoordinate);
			PdfObjectList<PdfCustomFunction> function = CreateFunction(documentCatalog);
			return new PdfFreeFormGourandShadedTriangleMesh(triangles, bitsPerFlag, bitsPerComponent, bitsPerCoordinate, xDecodeRange, yDecodeRange, cDecodeRange, function);
		}
		protected abstract IEnumerable<PdfTriangle> CreateTreangles(PdfPoint startPoint, PdfPoint endPoint, Color startColor, Color endColor);
		protected abstract PdfObjectList<PdfCustomFunction> CreateFunction(PdfDocumentCatalog documentCatalog);
		void AddTriangle(PdfPoint startPoint, PdfPoint endPoint, Color startColor, Color endColor) {
			subpathEndPoint = endPoint;
			triangles.AddRange(CreateTreangles(startPoint, endPoint, startColor, endColor));
		}
	}
}
