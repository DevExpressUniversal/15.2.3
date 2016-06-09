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

using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class FreePositionShapeMoving : AnnotationOperation {
		FreePosition ShapePosition { get { return Annotation.ShapePosition as FreePosition; } }
		public FreePositionShapeMoving(Annotation annotation) : base(annotation) {
		}
		void SetIndents(int left, int top, int right, int bottom) {
			switch (ShapePosition.DockCorner) {
				case DockCorner.LeftTop:
				case DockCorner.LeftBottom:
					ShapePosition.SetLeftIndent(left);
					break;
				case DockCorner.RightTop:
				case DockCorner.RightBottom:
					ShapePosition.SetRightIndent(right);
					break;
			}
			switch (ShapePosition.DockCorner) {
				case DockCorner.LeftTop:
				case DockCorner.RightTop:
					ShapePosition.SetTopIndent(top);
					break;
				case DockCorner.LeftBottom:
				case DockCorner.RightBottom:
					ShapePosition.SetBottomIndent(bottom);
					break;
			}
		}
		void CalcNewIndents(int dx, int dy, out int left, out int top, out int right, out int bottom) {
			ShapePosition.GetIndents(out left, out top, out right, out bottom);
			switch (ShapePosition.DockCorner) {
				case DockCorner.LeftTop:
					left += dx;
					top += dy;
					break;
				case DockCorner.RightTop:
					right -= dx;
					top += dy;
					break;
				case DockCorner.RightBottom:
					right -= dx;
					bottom -= dy;
					break;
				case DockCorner.LeftBottom:
					left += dx;
					bottom -= dy;
					break;
				default:
					ChartDebug.Fail("Unknown Doc Corner.");
					break;
			}
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (ShapePosition == null)
				return;
			int left, top, right, bottom;
			CalcNewIndents(dx, dy, out left, out top, out right, out bottom);
			SetIndents(left, top, right, bottom);
		}
	}
	public class RelativePositionShapeMoving : AnnotationOperation {
		RelativePosition ShapePosition { get { return Annotation.ShapePosition as RelativePosition; } }
		public RelativePositionShapeMoving(Annotation annotation) : base(annotation) {
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (ShapePosition == null)
				return;
			DiagramPoint location = DiagramPoint.Offset(ShapePosition.GetShapeLocation(Annotation.LastAnchorPosition), dx, dy, 0);
			DiagramPoint center = new DiagramPoint(location.X + 0.5 * Annotation.Width, location.Y + 0.5 * Annotation.Height);
			double angle = GeometricUtils.CalcBetweenPointsAngle((GRealPoint2D)Annotation.LastAnchorPosition, (GRealPoint2D)center);
			ShapePosition.SetConnectorLength(MathUtils.CalcLength2D(Annotation.LastAnchorPosition, center));
			ShapePosition.SetAngle(MathUtils.Radian2Degree(-angle));
		}
	}
}
