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
using DevExpress.Charts.Native;
using System;
namespace DevExpress.XtraCharts.Native {
	public class EllipseAnnotationShape : AnnotationShape {
		public EllipseAnnotationShape(Annotation annotation) : base(annotation) {
		}
		protected override void CalcTangentLines(ZPlaneRectangle bounds, DiagramPoint anchorPoint, out double angle1, out double angle2) {
			MathUtils.CalcEllipseTangentLines(bounds, anchorPoint, out angle1, out angle2);
		}
		protected override void CreateShape(GraphicsPath path, IntersectionInfo positiveIntersection, IntersectionInfo negativeIntersection, ZPlaneRectangle bounds) {
			DiagramPoint positivePoint = (DiagramPoint)positiveIntersection.IntersectionPoint;
			DiagramPoint negativePoint = (DiagramPoint)negativeIntersection.IntersectionPoint;
			double angle1 = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)bounds.Center, (GRealPoint2D)negativePoint);
			double angle2 = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)bounds.Center, (GRealPoint2D)positivePoint);
			double sweepAngle = angle1 > angle2 ? 2 * Math.PI - (angle1 - angle2) : -(angle1 - angle2);
			path.AddArc((RectangleF)bounds, (float)MathUtils.Radian2Degree(angle1), (float)MathUtils.Radian2Degree(sweepAngle));
		}
		protected override IntersectionInfo CalcLineSegmentWithShapeIntersection(DiagramPoint segmentPoint1, DiagramPoint segmentPoint2, ZPlaneRectangle bounds) {
			return GeometricUtils.CalcLineSegmentWithEllipseIntersection((GRealPoint2D)segmentPoint1, (GRealPoint2D)segmentPoint2,
				(GRealPoint2D)bounds.LeftBottom.Point, (GRealPoint2D)bounds.RightTop.Point);
		}
		protected override GraphicsPath CreateGraphicsPath(ZPlaneRectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse((RectangleF)bounds);
			return path;
		}
		protected internal override bool IsPointInsideShape(DiagramPoint point, ZPlaneRectangle bounds) {
			return MathUtils.IsPointInsideEllipse(point, bounds);
		}
		public override Size CalcInnerSize(Size outerSize) {
			bool isHorizontalEllipse = outerSize.Width > outerSize.Height;
			double outerA = isHorizontalEllipse ? outerSize.Width / 2.0 : outerSize.Height / 2.0;
			double outerB = isHorizontalEllipse ? outerSize.Height / 2.0 : outerSize.Width / 2.0;
			double innerB = Math.Sqrt(outerB * outerB / 2);
			double innerA = outerA * innerB / outerB;
			double outerWidth = isHorizontalEllipse ? innerA * 2 : innerB * 2;
			double outerHeight = isHorizontalEllipse ? innerB * 2 : innerA * 2;
			return new Size((int)Math.Abs(outerWidth), (int)Math.Abs(outerHeight));
		}
		public override Size CalcOuterSize(Size innerSize) {
			bool isHorizontalEllipse = innerSize.Width > innerSize.Height;
			double innerA = isHorizontalEllipse ? innerSize.Width / 2.0 : innerSize.Height / 2.0;
			double innerB = isHorizontalEllipse ? innerSize.Height / 2.0 : innerSize.Width / 2.0;
			double e = Math.Sqrt(1 - (innerB * innerB) / (innerA * innerA));
			double outerB = Math.Sqrt(2 * innerB * innerB);
			double outerA = innerA * outerB / innerB;
			double outerWidth = isHorizontalEllipse ? outerA * 2 : outerB * 2;
			double outerHeight = isHorizontalEllipse ? outerB * 2 : outerA * 2;
			return new Size(MathUtils.Ceiling(outerWidth), MathUtils.Ceiling(outerHeight));
		}
	}
}
