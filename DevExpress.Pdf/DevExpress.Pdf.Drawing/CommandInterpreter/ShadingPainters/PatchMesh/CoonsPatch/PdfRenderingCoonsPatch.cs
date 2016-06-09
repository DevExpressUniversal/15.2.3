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
	public class PdfRenderingCoonsPatch : PdfRenderingPatch {
		readonly PdfRenderingBezierCurve left;
		readonly PdfRenderingBezierCurve top;
		readonly PdfRenderingBezierCurve right;
		readonly PdfRenderingBezierCurve bottom;
		public PdfRenderingBezierCurve Left { get { return left; } }
		public PdfRenderingBezierCurve Top { get { return top; } }
		public PdfRenderingBezierCurve Right { get { return right; } }
		public PdfRenderingBezierCurve Bottom { get { return bottom; } }
		public override double Width { get { return Math.Max(top.Length, bottom.Length); } }
		public override double Height { get { return Math.Max(left.Length, right.Length); } }
		public PdfRenderingCoonsPatch(PdfRenderingBezierCurve left, PdfRenderingBezierCurve top, PdfRenderingBezierCurve right, PdfRenderingBezierCurve bottom, Color[] colors) : base(colors) { 
			this.left = left;
			this.top = top;
			this.right = new PdfRenderingBezierCurve(right.Point2, right.ControlPoint2, right.ControlPoint1, right.Point1);
			this.bottom = new PdfRenderingBezierCurve(bottom.Point2, bottom.ControlPoint2, bottom.ControlPoint1, bottom.Point1);
		}
		public override Point CalculatePoint(double u, double v) {
			PointF leftPoint = left.CalculatePoint(v);
			PointF topPoint = top.CalculatePoint(u);
			PointF rightPoint = right.CalculatePoint(v);
			PointF bottomPoint = bottom.CalculatePoint(u);
			PointF point1 = left.Point1;
			PointF point2 = top.Point1;
			PointF point3 = right.Point2;
			PointF point4 = bottom.Point2;
			double x = (1 - v) * bottomPoint.X + v * topPoint.X + (1 - u) * leftPoint.X + u * rightPoint.X - ((1 - v) * ((1 - u) * point1.X + u * point4.X) + v * ((1 - u) * point2.X + u * point3.X));
			double y = (1 - v) * bottomPoint.Y + v * topPoint.Y + (1 - u) * leftPoint.Y + u * rightPoint.Y - ((1 - v) * ((1 - u) * point1.Y + u * point4.Y) + v * ((1 - u) * point2.Y + u * point3.Y));
			return new Point(PdfMathUtils.ToInt32(x), PdfMathUtils.ToInt32(y));
		}
	}
}
