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
	public class PdfRenderingBezierCurve {
		static double CalcLength(PointF point1, PointF point2) {
			return PdfMathUtils.CalcDistance(new PdfPoint(point1.X, point1.Y), new PdfPoint(point2.X, point2.Y));
		}
		readonly PointF point1;
		readonly PointF controlPoint1;
		readonly PointF controlPoint2;
		readonly PointF point2;
		public PointF Point1 { get { return point1; } }
		public PointF ControlPoint1 { get { return controlPoint1; } }
		public PointF ControlPoint2 { get { return controlPoint2; } }
		public PointF Point2 { get { return point2; } }
		public double Length { get { return CalcLength(point1, controlPoint1) + CalcLength(controlPoint1, controlPoint2) + CalcLength(controlPoint2, point2); }  }
		public PdfRenderingBezierCurve(PointF point1, PointF controlPoint1, PointF controlPoint2, PointF point2) {
			this.point1 = point1;
			this.controlPoint1 = controlPoint1;
			this.controlPoint2 = controlPoint2;
			this.point2 = point2;
		}
		public PointF CalculatePoint(double t) {
			double d = 1 - t;
			double dd = d * d;
			double tt = t * t;
			return new PointF((float)(d * dd  * point1.X + 3 * t * dd  * controlPoint1.X + 3 * tt  * d * controlPoint2.X + t * tt  * point2.X), 
							  (float)(d * dd  * point1.Y + 3 * t * dd  * controlPoint1.Y + 3 * tt  * d * controlPoint2.Y + t * tt  * point2.Y)); 
		}
	}
}
