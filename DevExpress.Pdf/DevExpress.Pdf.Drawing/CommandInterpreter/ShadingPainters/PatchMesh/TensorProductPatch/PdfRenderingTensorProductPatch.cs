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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfRenderingTensorProductPatch : PdfRenderingPatch {
		static double CalculateB0(double t) {
			double dt = 1 - t;		
			return dt * dt * dt;
		}
		static double CalculateB1(double t) {
			double dt = 1 - t;
			return 3 * t * dt * dt;
		}
		static double CalculateB2(double t) {
			return 3 * t * t * (1 - t);
		}
		static double CalculateB3(double t) {
			return t * t * t;
		}
		static float GetX(PointF point) {
			return point.X;
		}
		static float GetY(PointF point) {
			return point.Y;
		}
		readonly PointF[,] controlPoints;
		public override double Width {
			get { 
				PdfRenderingBezierCurve curve1 = new PdfRenderingBezierCurve(controlPoints[0, 0], controlPoints[1, 0], controlPoints[2, 0], controlPoints[3, 0]);
				PdfRenderingBezierCurve curve2 = new PdfRenderingBezierCurve(controlPoints[0, 1], controlPoints[1, 1], controlPoints[2, 1], controlPoints[3, 1]);
				PdfRenderingBezierCurve curve3 = new PdfRenderingBezierCurve(controlPoints[0, 2], controlPoints[1, 2], controlPoints[2, 2], controlPoints[3, 2]);
				PdfRenderingBezierCurve curve4 = new PdfRenderingBezierCurve(controlPoints[0, 3], controlPoints[1, 3], controlPoints[2, 3], controlPoints[3, 3]);
				return Math.Max(Math.Max(curve1.Length, curve2.Length), Math.Max(curve3.Length, curve4.Length));
			}
		}
		public override double Height {
			get { 
				PdfRenderingBezierCurve curve1 = new PdfRenderingBezierCurve(controlPoints[0, 0], controlPoints[0, 1], controlPoints[0, 2], controlPoints[0, 3]);
				PdfRenderingBezierCurve curve2 = new PdfRenderingBezierCurve(controlPoints[1, 0], controlPoints[1, 1], controlPoints[1, 2], controlPoints[1, 3]);
				PdfRenderingBezierCurve curve3 = new PdfRenderingBezierCurve(controlPoints[2, 0], controlPoints[2, 1], controlPoints[2, 2], controlPoints[2, 3]);
				PdfRenderingBezierCurve curve4 = new PdfRenderingBezierCurve(controlPoints[3, 0], controlPoints[3, 1], controlPoints[3, 2], controlPoints[3, 3]);
				return Math.Max(Math.Max(curve1.Length, curve2.Length), Math.Max(curve3.Length, curve4.Length));
			}
		}
		public PdfRenderingTensorProductPatch(PointF[,] controlPoints, Color[] colors) : base(colors) { 
			this.controlPoints = controlPoints;
		}
		int CalculateCoord(double u, double v, Func<PointF, float> getCoord) {
			double b0u = CalculateB0(u);
			double b1u = CalculateB1(u);
			double b2u = CalculateB2(u);
			double b3u = CalculateB3(u);
			double b0v = CalculateB0(v);
			double b1v = CalculateB1(v);
			double b2v = CalculateB2(v);
			double b3v = CalculateB3(v);
			double coord = getCoord(controlPoints[0, 0]) * b0u * b0v + getCoord(controlPoints[0, 1]) * b0u * b1v + getCoord(controlPoints[0, 2]) * b0u * b2v + getCoord(controlPoints[0, 3]) * b0u * b3v + 
						   getCoord(controlPoints[1, 0]) * b1u * b0v + getCoord(controlPoints[1, 1]) * b1u * b1v + getCoord(controlPoints[1, 2]) * b1u * b2v + getCoord(controlPoints[1, 3]) * b1u * b3v + 
						   getCoord(controlPoints[2, 0]) * b2u * b0v + getCoord(controlPoints[2, 1]) * b2u * b1v + getCoord(controlPoints[2, 2]) * b2u * b2v + getCoord(controlPoints[2, 3]) * b2u * b3v + 
						   getCoord(controlPoints[3, 0]) * b3u * b0v + getCoord(controlPoints[3, 1]) * b3u * b1v + getCoord(controlPoints[3, 2]) * b3u * b2v + getCoord(controlPoints[3, 3]) * b3u * b3v;
			return PdfMathUtils.ToInt32(coord);
		}
		public override Point CalculatePoint(double u, double v) {
			return new Point(CalculateCoord(u, v, GetX), CalculateCoord(u, v, GetY));
		}
	}
}
