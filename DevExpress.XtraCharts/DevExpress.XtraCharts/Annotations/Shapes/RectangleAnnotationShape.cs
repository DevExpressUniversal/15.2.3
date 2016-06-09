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
	public class RectangleAnnotationShape : AnnotationShape {
		public RectangleAnnotationShape(IAnnotationShapeOwner annotation) : base(annotation) {
		}
		static int GetIndexBySegmentKind(SegmentKind segmentKind) {
			switch (segmentKind) {
				case SegmentKind.Left:
					return 3;
				case SegmentKind.Bottom:
					return 2;
				case SegmentKind.Right:
					return 1;
				case SegmentKind.Top:
					return 0;
				default:
					ChartDebug.Fail("Incorrect segment kind.");
					return 0;
			}
		}
		protected override void CalcTangentLines(ZPlaneRectangle bounds, DiagramPoint anchorPoint, out double minAngle, out double maxAngle) {
			minAngle = Double.MaxValue;
			maxAngle = Double.MinValue;
			foreach (Vertex vertex in bounds.Vertices)
				ProsessAngle(GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)anchorPoint, (GRealPoint2D)vertex.Point), ref minAngle, ref maxAngle);
		}
		protected override IntersectionInfo CalcLineSegmentWithShapeIntersection(DiagramPoint segmentPoint1, DiagramPoint segmentPoint2, ZPlaneRectangle bounds) {
			return GeometricUtils.CalcLineSegmentWithRectIntersection((GRealPoint2D)segmentPoint1, (GRealPoint2D)segmentPoint2,
				(GRealPoint2D)bounds.LeftBottom.Point, (GRealPoint2D)bounds.RightTop.Point);
		}
		protected override GraphicsPath CreateGraphicsPath(ZPlaneRectangle bounds) {
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle((RectangleF)bounds);
			return path;
		}
		protected override void CreateShape(GraphicsPath path, IntersectionInfo positiveIntersection, IntersectionInfo negativeIntersection, ZPlaneRectangle bounds) {
			DiagramPoint positivePoint = (DiagramPoint)positiveIntersection.IntersectionPoint;
			DiagramPoint negativePoint = (DiagramPoint)negativeIntersection.IntersectionPoint;
			DiagramPoint[] points = { bounds.LeftBottom, bounds.RightBottom, bounds.RightTop, bounds.LeftTop, bounds.LeftBottom };
			int startIndex = GetIndexBySegmentKind(negativeIntersection.SegmentKind);
			int endIndex = GetIndexBySegmentKind(positiveIntersection.SegmentKind);
			int count = points.Length - 1;
			path.AddLine((PointF)negativePoint, (PointF)points[startIndex + 1]);
			for (int i = (startIndex + 1) % count; i != endIndex; i = (i + 1) % count)
				path.AddLine((PointF)points[i], (PointF)points[i + 1]);
			path.AddLine((PointF)points[endIndex], (PointF)positivePoint);
		}
		protected internal override bool IsPointInsideShape(DiagramPoint point, ZPlaneRectangle bounds) {
			return bounds.Contains(point);
		}
		public override Size CalcInnerSize(Size outerSize) {
			return outerSize;
		}
		public override Size CalcOuterSize(Size innerSize) {
			return innerSize;
		}
	}
}
