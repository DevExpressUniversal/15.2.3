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
namespace DevExpress.XtraCharts.Native {
	public abstract class AnnotationOperation {
		readonly Annotation annotation;
		protected Annotation Annotation { get { return annotation; } }
		public AnnotationOperation(Annotation annotation) {
			this.annotation = annotation;
		}
		protected void CorrectParamsByAngle(ref int dx, ref int dy) {
			if (Annotation == null)
				return;
			DiagramPoint point = new DiagramPoint(dx, dy);
			Matrix matrix = new Matrix();
			matrix.Rotate(-Annotation.Angle);
			point = TransformUtils.ApplyTransform(matrix, point);
			dx = MathUtils.StrongRound(point.X);
			dy = MathUtils.StrongRound(point.Y);
		}
		public abstract void Run(int x, int y, int dx, int dy);
	}
	public class RotationOperation : AnnotationOperation {
		public RotationOperation(Annotation annotation) : base(annotation) {			
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null)
				return;
			if (x < 0 || y < 0)
				return;
			GRealPoint2D center = new GRealPoint2D(Annotation.LastLocation.X + 0.5 * Annotation.Width, Annotation.LastLocation.Y + 0.5 * Annotation.Height);
			GRealPoint2D point = new GRealPoint2D(x + dx, y + dy);
			double angle = MathUtils.Radian2Degree(GeometricUtils.CalcBetweenPointsAngle(center, point));
			Annotation.SetAngle(MathUtils.StrongRound(MathUtils.NormalizeDegree(angle + 90)));
		}
	}
	public abstract class ResizingOperation : AnnotationOperation {
		readonly ResizeDirection resizeDirection;
		protected ResizeDirection ResizeDirection { get { return resizeDirection; } }
		public ResizingOperation(Annotation annotation, ResizeDirection resizeDirection) : base(annotation) {
			this.resizeDirection = resizeDirection;
		}
		protected DiagramPoint CalcCursorPoint(int x, int y, int dx, int dy) {
			DiagramPoint point = new DiagramPoint(x + dx, y + dy);
			DiagramPoint center = DiagramPoint.Offset(Annotation.LastLocation, 0.5 * Annotation.Width, 0.5 * Annotation.Height, 0);
			Matrix matrix = new Matrix();
			matrix.RotateAt(-Annotation.Angle, (PointF)center);
			return TransformUtils.ApplyTransform(matrix, point);
		}
		protected void CorrectSize(ref int width, ref int height) {
			if (width < 1)
				width = 1;
			if (height < 1)
				height = 1;
		}
	}   
}
