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
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public struct FibonacciArc {
		readonly float startAngle;
		readonly float sweepAngle;
		public float StartAngle { get { return startAngle; } }
		public float SweepAngle { get { return sweepAngle; } }
		public FibonacciArc(double startAngle, double endAngle) {
			this.startAngle = (float)MathUtils.Radian2Degree(startAngle);
			this.sweepAngle = (float)(MathUtils.Radian2Degree(endAngle - startAngle) * 2);
		}
	}
	public class FibonacciCircle : List<FibonacciArc> {
		readonly RectangleF bounds;
		readonly float radius;
		public RectangleF Bounds { get { return bounds; } }
		public float Radius { get { return radius; } }
		public FibonacciCircle(RectangleF bounds, float radius) {
			this.bounds = bounds;
			this.radius = radius;
		}
	}
	public class FibonacciArcsBehavior : FibonacciIndicatorBehavior {
		public override bool DefaultShowLevel0 { get { return false; } }
		public override bool DefaultShowLevel100 { get { return false; } }
		public override bool DefaultShowLevel23_6 { get { return false; } }
		public override bool DefaultShowLevel76_4 { get { return false; } }
		public override bool DefaultShowAdditionalLevels { get { return false; } }
		public override bool ShowLevel0PropertyEnabled { get { return false; } }
		public override bool ShowLevel100PropertyEnabled { get { return true; } }
		public override bool ShowAdditionalLevelsPropertyEnabled { get { return false; } }
		public FibonacciArcsBehavior(FibonacciIndicator fibonacciIndicator) : base(fibonacciIndicator) {
		}
		static IList<DiagramPoint> CalculateLevelsPoints(XYDiagramMappingBase diagramMapping, DiagramPoint centralPoint, DiagramPoint level100Point, IList<double> levels) {
			List<DiagramPoint> levelsPoints = new List<DiagramPoint>(levels.Count);
			foreach (double level in levels) {
				double x = centralPoint.X + (level100Point.X - centralPoint.X) * level;
				double y = centralPoint.Y + (level100Point.Y - centralPoint.Y) * level;
				levelsPoints.Add(diagramMapping.GetScreenPointNoRound(x, y));
			}
			return levelsPoints;
		}
		IEnumerable<RotatedTextPainterNearCircleTangent> CalculateLabels(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer, DiagramPoint centerPoint, IList<double> levels, IList<DiagramPoint> levelsPoints, int thickness) {
			FibonacciIndicatorLabel label = FibonacciIndicator.Label;
			Font font = label.Font;
			double halfThickness = Math.Ceiling(thickness / 2.0);
			List<RotatedTextPainterNearCircleTangent> painters = new List<RotatedTextPainterNearCircleTangent>(levels.Count);
			for (int i = 0; i < levels.Count; i++) {
				DiagramPoint levelPoint = levelsPoints[i];
				string text = ConstructLevelText(levels[i]);
				double length = MathUtils.CalcLength2D(centerPoint, levelPoint);
				DiagramPoint location;
				int angle;
				if (diagramMapping.Rotated) {
					location = new DiagramPoint(centerPoint.X + (centerPoint.X > levelPoint.X ? -length : length) - halfThickness, centerPoint.Y);
					angle = 270;
				}
				else {
					location = new DiagramPoint(centerPoint.X, centerPoint.Y + (centerPoint.Y > levelPoint.Y ? -length : length) - halfThickness);
					angle = 0;
				}
				RotatedTextPainterNearCircleTangent painter = new RotatedTextPainterNearCircleTangent(angle, 
					(Point)location, text, textMeasurer.MeasureString(text, font), label, true);
				painters.Add(painter);
			}
			return painters;
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible)
				return null;
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			DiagramPoint centerPoint = MaxPoint;
			DiagramPoint level100Point = MinPoint;
			IList<double> levels = GetLevels();
			IList<double> baseLevels = GetBaseLevels();
			IList<DiagramPoint> levelsPoints = CalculateLevelsPoints(diagramMapping, centerPoint, level100Point, levels);
			IList<DiagramPoint> baseLevelsPoints = CalculateLevelsPoints(diagramMapping, centerPoint, level100Point, baseLevels);
			DiagramPoint screenCenterPoint = diagramMapping.GetScreenPointNoRound(centerPoint.X, centerPoint.Y);
			FibonacciIndicatorLabel label = fibonacciIndicator.Label;
			FibonacciIndicatorLabelsLayout labelsLayout;
			if (label.ActualVisibility) {
				IEnumerable<RotatedTextPainterNearCircleTangent> titles = CalculateLabels(diagramMapping, textMeasurer, screenCenterPoint, 
					levels, levelsPoints, fibonacciIndicator.LineStyle.Thickness);
				IEnumerable<RotatedTextPainterNearCircleTangent> baseLevelTitles = CalculateLabels(diagramMapping, textMeasurer, screenCenterPoint, 
					baseLevels, baseLevelsPoints, fibonacciIndicator.BaseLevelLineStyle.Thickness);
				labelsLayout = new FibonacciIndicatorLabelsLayout(label, titles, baseLevelTitles);
			}
			else
				labelsLayout = null;
			return new FibonacciArcsLayout(fibonacciIndicator, labelsLayout, 
				screenCenterPoint, diagramMapping.Bounds, levelsPoints, baseLevelsPoints);
		}
	}
	public class FibonacciArcsLayout : FibonacciIndicatorLayout {
		static GraphicsPath CreateGraphicsPathBase(IList<FibonacciCircle> circles, int thickness) {
			if (circles.Count == 0)
				return null;
			GraphicsPath path = new GraphicsPath();
			foreach (FibonacciCircle circle in circles)
				foreach (FibonacciArc arc in circle)
					path.AddArc(circle.Bounds, arc.StartAngle, arc.SweepAngle);
			using (Pen pen = new Pen(Color.Empty, thickness))
				path.Widen(pen);
			return path;
		}
		readonly DiagramPoint centerPoint;
		readonly Rectangle mappingBounds;
		readonly IList<FibonacciCircle> levelsCircles;
		readonly IList<FibonacciCircle> baseLevelsCircles;
		public FibonacciArcsLayout(FibonacciIndicator fibonacciIndicator, 
								   FibonacciIndicatorLabelsLayout labelsLayout, 
								   DiagramPoint centerPoint, 
								   Rectangle mappingBounds, 
								   IList<DiagramPoint> levelsPoints, 
								   IList<DiagramPoint> baseLevelsPoints) 
			: base(fibonacciIndicator, labelsLayout) {
			this.centerPoint = centerPoint;
			this.mappingBounds = mappingBounds;
			levelsCircles = CalculateCircles(levelsPoints);
			baseLevelsCircles = CalculateCircles(baseLevelsPoints);
		}
		double CalcAngle(double radius, double x, double y) {
			double dx = x - centerPoint.X;
			double dy = y - centerPoint.Y;
			double angle = Math.Acos(Math.Abs(dx) / radius);
			if (dx >= 0)
				return dy >= 0 ? angle : 2 * Math.PI - angle;
			else
				return dy >= 0 ? Math.PI - angle : Math.PI + angle;
		}
		void AddIntersectionAngles(List<double>angles, double radius, Point point1, Point point2) {
			double x10Diff = point1.X - centerPoint.X;
			double x21Diff = point2.X - point1.X;
			double y10Diff = point1.Y - centerPoint.Y;
			double y21Diff = point2.Y - point1.Y;
			double a = x21Diff * x21Diff + y21Diff * y21Diff;
			double b = x21Diff * x10Diff + y21Diff * y10Diff;
			double d = b * b - a * (x10Diff * x10Diff + y10Diff * y10Diff - radius * radius);
			if (d > 0) {
				d = Math.Sqrt(d);
				double root = (-b + d) / a;
				angles.Add(CalcAngle(radius, x21Diff * root + point1.X, y21Diff * root + point1.Y));
				root = (-b - d) / a;
				angles.Add(CalcAngle(radius, x21Diff * root + point1.X, y21Diff * root + point1.Y));
			}
			else if (d == 0) {
				double root = -b / a;
				if (root >= 0 && root <= 1)
					angles.Add(CalcAngle(radius, x21Diff * root + point1.X, y21Diff * root + point1.Y));
			}
		}
		bool PointInMappingBounds(double radius, double angle) {
			int x = MathUtils.StrongRound(centerPoint.X + radius * Math.Cos(angle));
			int y = MathUtils.StrongRound(centerPoint.Y + radius * Math.Sin(angle));
			return mappingBounds.Contains(new Point(x, y));
		}
		bool ShouldAddArc(double radius, double startAngle, double endAngle) {
			return startAngle < endAngle && PointInMappingBounds(radius, (startAngle + endAngle) / 2);
		}
		FibonacciCircle CreateFibonacciCircle(double radius) {
			float diameter = (float)(radius * 2);
			if (diameter < 0.5f)
				return null;
			RectangleF bounds = new RectangleF((float)(centerPoint.X - radius), (float)(centerPoint.Y - radius), diameter, diameter);
			FibonacciCircle circle = new FibonacciCircle(bounds, (float)radius);
			Point leftTop = new Point(mappingBounds.Left, mappingBounds.Top);
			Point leftBottom = new Point(mappingBounds.Left, mappingBounds.Bottom);
			Point rightTop = new Point(mappingBounds.Right, mappingBounds.Top);
			Point rightBottom = new Point(mappingBounds.Right, mappingBounds.Bottom);
			List<double> angles = new List<double>();
			AddIntersectionAngles(angles, radius, leftTop, rightTop);
			AddIntersectionAngles(angles, radius, rightTop, rightBottom);
			AddIntersectionAngles(angles, radius, rightBottom, leftBottom);
			AddIntersectionAngles(angles, radius, leftBottom, leftTop);
			int count = angles.Count;
			if (count > 1) {
				angles.Sort();
				double startAngle;
				double endAngle = angles[0];
				for (int i = 1; i < count; i++) {
					startAngle = endAngle;
					endAngle = angles[i];
					if (ShouldAddArc(radius, startAngle, endAngle))
						circle.Add(new FibonacciArc(startAngle, endAngle));
				}
				startAngle = angles[0] + 2 * Math.PI;
				if (ShouldAddArc(radius, endAngle, startAngle))
					circle.Add(new FibonacciArc(endAngle, startAngle));
			}
			else if (mappingBounds.Contains((Point)centerPoint) && PointInMappingBounds(radius, Math.PI / 4))
				circle.Add(new FibonacciArc(0, 2 * Math.PI));
			return circle.Count > 0 ? circle : null;
		}
		IList<FibonacciCircle> CalculateCircles(IList<DiagramPoint> levelsPoints) {
			List<FibonacciCircle> circles = new List<FibonacciCircle>(levelsPoints.Count);
			foreach (DiagramPoint levelsPoint in levelsPoints) {
				FibonacciCircle circle = CreateFibonacciCircle(MathUtils.CalcLength2D(centerPoint, levelsPoint));
				if (circle != null)
					circles.Add(circle);
			}
			return circles;
		}
		protected override void RenderBase(IRenderer renderer) {
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			renderer.EnableAntialiasing(true);
			Color color = Color;
			int thickness = Thickness;
			DashStyle dashStyle = fibonacciIndicator.LineStyle.DashStyle;
			foreach (FibonacciCircle circle in levelsCircles) {
				foreach (FibonacciArc arc in circle)
					renderer.DrawArc((Point)centerPoint, circle.Radius, arc.StartAngle, arc.SweepAngle, color, thickness, dashStyle);
			}
			color = BaseLevelColor;
			thickness = BaseLevelThickness;
			dashStyle = fibonacciIndicator.BaseLevelLineStyle.DashStyle;
			foreach (FibonacciCircle circle in baseLevelsCircles) {
				foreach (FibonacciArc arc in circle)
					renderer.DrawArc((Point)centerPoint, circle.Radius, arc.StartAngle, arc.SweepAngle, color, thickness, dashStyle);
			}
			renderer.RestoreAntialiasing();
		}
		public override GraphicsPath CalculateHitTestGraphicsPath() {
			GraphicsPath path = CreateGraphicsPathBase(levelsCircles, Thickness);
			GraphicsPath baseLevelsPath = CreateGraphicsPathBase(baseLevelsCircles, BaseLevelThickness);
			if (path == null)
				return baseLevelsPath;
			if (baseLevelsPath == null)
				return path;
			path.AddPath(baseLevelsPath, false);
			return path;
		}
	}
}
