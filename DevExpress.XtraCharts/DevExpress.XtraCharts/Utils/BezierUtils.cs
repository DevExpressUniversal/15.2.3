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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class BezierUtils {
		const double magic = 4.5e-2;
		const int maxStepCount = 15;
		public static int CalcStepCount(GRealPoint2D p1, GRealPoint2D p2, double zoomingFactor) {
			double length = MathUtils.CalcLength2D(p1, p2);
			double count = length * magic * zoomingFactor;
			return count > maxStepCount ? maxStepCount : (int)Math.Round(count);
		}
		public static void CalcBezier(GRealPoint2D p1, GRealPoint2D p2, GRealPoint2D p3, GRealPoint2D p4, int stepCount, IList<GRealPoint2D> points) {
			double step = 1.0 / (stepCount + 1);
			double step2 = step * step;
			double step3 = step * step * step;
			double step13 = step * 3.0;
			double step23 = step2 * 3.0;
			double step26 = step2 * 6.0;
			double step36 = step3 * 6.0;
			double tempX1 = p1.X - p2.X * 2.0 + p3.X;
			double tempY1 = p1.Y - p2.Y * 2.0 + p3.Y;
			double tempX2 = (p2.X - p3.X) * 3.0 - p1.X + p4.X;
			double tempY2 = (p2.Y - p3.Y) * 3.0 - p1.Y + p4.Y;
			double dx = (p2.X - p1.X) * step13 + tempX1 * step23 + tempX2 * step3;
			double dy = (p2.Y - p1.Y) * step13 + tempY1 * step23 + tempY2 * step3;
			double ddx = tempX1 * step26 + tempX2 * step36;
			double ddy = tempY1 * step26 + tempY2 * step36;
			double dddx = tempX2 * step36;
			double dddy = tempY2 * step36;
			double x = p1.X;
			double y = p1.Y;
			for (int i = 0; i < stepCount; i++) {
				x += dx;
				y += dy;
				points.Add(new GRealPoint2D(x, y));
				dx += ddx;
				dy += ddy;
				ddx += dddx;
				ddy += dddy;
			}
			points.Add(p4);
		}
		public static LineStrip CalcBezier(BezierStrip strip, double zoomingFactor) {
			LineStrip result = new LineStrip();
			if (strip == null || strip.Count == 0)
				return result;
			GRealPoint2D previousPoint = new GRealPoint2D(strip[0].X, strip[0].Y);
			result.Add(previousPoint);
			if (strip.IsEmpty)
				return result;
			List<GRealPoint2D> drawingPoints = strip.GetPointsForDrawing(true, false);
			for (int i = 3; i < drawingPoints.Count; i += 3) {
				GRealPoint2D cp1 = drawingPoints[i - 2];
				GRealPoint2D cp2 = drawingPoints[i - 1];
				GRealPoint2D point = drawingPoints[i];
				int stepCount = CalcStepCount(previousPoint, point, zoomingFactor);
				CalcBezier(previousPoint, cp1, cp2, point, stepCount, result);
				previousPoint = point;
			}
			return result;
		}
	}
}
