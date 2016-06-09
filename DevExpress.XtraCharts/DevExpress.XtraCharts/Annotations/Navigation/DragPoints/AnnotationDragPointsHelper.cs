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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public static class AnnotationDragPointsHelper {
		const double minSide = 8;
		static List<AnnotationDragPoint> CalcResizingPoints(Annotation annotation, ZPlaneRectangle bounds) {
			List<AnnotationDragPoint> result = new List<AnnotationDragPoint>();
			ZPlaneRectangle correctedBounds = new ZPlaneRectangle(bounds.Location, bounds.Width - 1, bounds.Height - 1);
			Matrix matrix = new Matrix();
			matrix.RotateAt(annotation.Angle, (PointF)bounds.Center);
			result.Add(new LeftTopDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.LeftBottom)));
			result.Add(new RightTopDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.RightBottom)));
			result.Add(new RightBottomDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.RightTop)));
			result.Add(new LeftBottomDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.LeftTop)));
			if (MathUtils.CalcLength2D(correctedBounds.LeftBottom, correctedBounds.RightBottom) > minSide) {
				result.Add(new TopDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.LeftBottom),
					TransformUtils.ApplyTransform(matrix, correctedBounds.RightBottom)));
				result.Add(new BottomDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.RightTop),
					TransformUtils.ApplyTransform(matrix, correctedBounds.LeftTop)));
			}
			if (MathUtils.CalcLength2D(correctedBounds.LeftBottom, correctedBounds.LeftTop) > minSide) {
				result.Add(new LeftDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.LeftTop), 
					TransformUtils.ApplyTransform(matrix, correctedBounds.LeftBottom)));
				result.Add(new RightDragPoint(annotation, TransformUtils.ApplyTransform(matrix, correctedBounds.RightBottom), 
					TransformUtils.ApplyTransform(matrix, correctedBounds.RightTop)));
			}
			return result;
		}
		static AnnotationDragPoint CalcRotationPoint(Annotation annotation, ZPlaneRectangle bounds) {
			DiagramPoint point1 = new DiagramPoint((bounds.Left + bounds.Right) / 2, bounds.Bottom);
			DiagramPoint point2 = new DiagramPoint((bounds.Left + bounds.Right) / 2, bounds.Bottom - RotationDragPoint.Indent);
			Matrix matrix = new Matrix();
			matrix.RotateAt(annotation.Angle, (PointF)bounds.Center);
			return new RotationDragPoint(annotation, TransformUtils.ApplyTransform(matrix, point1), TransformUtils.ApplyTransform(matrix, point2)); 
		}
		public static List<AnnotationDragPoint> CalcPoints(Annotation annotation, AnnotationShape shape, ZPlaneRectangle bounds, ZPlaneRectangle hitTestBounds, DiagramPoint anchorPoint) {
			List<AnnotationDragPoint> result = new List<AnnotationDragPoint>();
			if (annotation.CanPerformMoving)
				result.Add(new ShapeMovingDragPoint(annotation, shape.CreateHitRegion(hitTestBounds, anchorPoint)));
			if (annotation.RuntimeOperationSelect) {
				if (annotation.CanPerformRotation)
					result.Add(CalcRotationPoint(annotation, bounds));
				if (annotation.CanPerformResizing)
					result.AddRange(CalcResizingPoints(annotation, bounds));
				if (annotation.CanPerformAnchoring)
					result.Add(new AnchoringDragPoint(annotation, anchorPoint));
			}			
			return result;
		}
	}
}
