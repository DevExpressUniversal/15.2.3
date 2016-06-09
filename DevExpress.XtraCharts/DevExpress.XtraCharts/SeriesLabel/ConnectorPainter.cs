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
	public abstract class ConnectorPainterBase {
		readonly DiagramPoint startPoint;
		DiagramPoint finishPoint;
		ZPlaneRectangle bounds;
		public DiagramPoint StartPoint { get { return startPoint; } }
		protected DiagramPoint FinishPoint { get { return finishPoint; } set { finishPoint = value; } }
		protected ZPlaneRectangle Bounds { get { return bounds; } }
		public ConnectorPainterBase(DiagramPoint startPoint, DiagramPoint finishPoint, ZPlaneRectangle bounds) {
			this.startPoint = startPoint;
			this.finishPoint = finishPoint;
			this.bounds = bounds;
		}
		protected abstract void Calculate();
		public abstract void Render(IRenderer renderer, SeriesLabelBase label, Color color);
		public abstract void FillPrimitives(SeriesLabelBase label, Color color, PrimitivesContainer container);
		public void Offset(double dx, double dy) {
			if (bounds != null)
				bounds.Offset(dx, dy, 0);
			finishPoint = DiagramPoint.Offset(finishPoint, dx, dy, 0);
			Calculate();
		}
	}
	public class LineConnectorPainter : ConnectorPainterBase {
		enum RelativeLocation { LessThan, Center, GreaterThan }
		readonly bool flat;
		double angle;
		protected double Angle { get { return angle; } set { angle = value; } }
		protected bool Flat { get { return flat; } }
		public LineConnectorPainter(DiagramPoint startPoint, DiagramPoint finishPoint, double angle, ZPlaneRectangle bounds, bool flat) : base(startPoint, finishPoint, bounds) {
			this.angle = angle;
			this.flat = flat;
		}
		protected void Render(IRenderer renderer, DiagramPoint p1, DiagramPoint p2, double angle, SeriesLabelBase label, Color color) {
			LineStyle lineStyle = label.LineStyle;
			Color actualColor = label.GetActualConnectorColor(color);
			DiagramPoint actualStartPoint = DiagramPoint.Round(p1);
			DiagramPoint actualFinishPoint = DiagramPoint.Round(p2);
			renderer.EnableAntialiasing(double.IsNaN(angle) || !MathUtils.IsAngleNormal(angle));
			renderer.DrawLine((Point)actualStartPoint, (Point)actualFinishPoint, actualColor, lineStyle.Thickness, lineStyle, LineCap.Round); 
			renderer.RestoreAntialiasing();
		}
		protected virtual DiagramPoint GetCalculationPoint() {
			return StartPoint;
		}
		protected override void Calculate() {
			if (flat) {
				DiagramPoint calculationPoint = GetCalculationPoint();
				RelativeLocation xLocation, yLocation;
				if (calculationPoint.X >= Bounds.RightCenter.X)
					xLocation = RelativeLocation.GreaterThan;
				else if (calculationPoint.X <= Bounds.LeftCenter.X)
					xLocation = RelativeLocation.LessThan;
				else
					xLocation = RelativeLocation.Center;
				if (calculationPoint.Y >= Bounds.TopCenter.Y)
					yLocation = RelativeLocation.GreaterThan;
				else if (calculationPoint.Y <= Bounds.BottomCenter.Y)
					yLocation = RelativeLocation.LessThan;
				else
					yLocation = RelativeLocation.Center;
				if (xLocation == RelativeLocation.Center && yLocation == RelativeLocation.Center)
					FinishPoint = DiagramPoint.StrongRound(StartPoint);
				else {
					double x, y;
					if (xLocation == RelativeLocation.GreaterThan)
						x = Bounds.RightCenter.X - 1.0;
					else if (xLocation == RelativeLocation.LessThan)
						x = Bounds.LeftCenter.X;
					else 
						x = Bounds.Center.X;
					if (yLocation == RelativeLocation.GreaterThan)
						y = Bounds.TopCenter.Y - 1.0;
					else if (yLocation == RelativeLocation.LessThan)
						y = Bounds.BottomCenter.Y;
					else 
						y = Bounds.Center.Y;
					FinishPoint = DiagramPoint.StrongRound(new DiagramPoint(x, y));
				}
				angle = Math.Atan2(StartPoint.Y - FinishPoint.Y, FinishPoint.X - StartPoint.X);
			}
		}
		public override void Render(IRenderer renderer, SeriesLabelBase label, Color color) {
			if (flat)
				Render(renderer, StartPoint, FinishPoint, angle, label, color);
		}
		public override void FillPrimitives(SeriesLabelBase label, Color color, PrimitivesContainer container) {
			if (!flat) {
				Color actualColor = label.GetActualConnectorColor(color);
				DiagramPoint p1 = DiagramPoint.StrongRound(StartPoint);
				DiagramPoint p2 = DiagramPoint.StrongRound(FinishPoint);
				Line line = new Line(p1, p2, true, true, new DiagramVector(0.0, 0.0, 1.0), actualColor, LineCap.Round);
				label.LineStyle.SetLineStyle(line);
				container.Add(new Line[] { line }, null);
			}
		}
	}
	public class OriginBaseLineConnectorPainter : LineConnectorPainter {
		DiagramPoint origin;
		public OriginBaseLineConnectorPainter(DiagramPoint origin, DiagramPoint startPoint, DiagramPoint finishPoint, double angle, ZPlaneRectangle bounds, bool flat) : base(startPoint, finishPoint, angle, bounds, flat) {
			this.origin = origin;
		}
		protected override DiagramPoint GetCalculationPoint() {
			return origin;
		}
	}
	public class BrokenLineConnectorPainter : LineConnectorPainter {
		DiagramPoint? middlePoint;
		double startAngle;
		public double StartAngle { get { return startAngle; } }
		public BrokenLineConnectorPainter(DiagramPoint startPoint, DiagramPoint? middlePoint, DiagramPoint finishPoint, double angle)
			: base(startPoint, finishPoint, angle, null, true) {
			this.middlePoint = middlePoint;
			startAngle = angle;
		}
		protected override void Calculate() {
			if (Flat) {
				DiagramPoint p1 = StartPoint;
				DiagramPoint p2 = new DiagramPoint(p1.X + Math.Cos(startAngle), p1.Y - Math.Sin(startAngle));
				DiagramPoint point;
				Intersection intersection = IntersectionUtils.CalcLinesIntersection2D(p1, p2, 
					FinishPoint, DiagramPoint.Offset(FinishPoint, 1, 0, 0), false, 1e-5, out point);
				switch (intersection) {
					case Intersection.Yes:
						if ((point.X >= StartPoint.X && point.X < FinishPoint.X) ||
							(point.X <= StartPoint.X && point.X > FinishPoint.X))
							middlePoint = point;
						else
							goto case Intersection.No;
						break;
					case Intersection.No:
						middlePoint = null;
						Angle = Math.Atan2(StartPoint.Y - FinishPoint.Y, FinishPoint.X - StartPoint.X);
						break;
					case Intersection.Match:
						goto case Intersection.No;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		public override void Render(IRenderer renderer, SeriesLabelBase label, Color color) {
			if (Flat) {
				if (middlePoint == null)
					Render(renderer, StartPoint, FinishPoint, Angle, label, color);
				else {
					Render(renderer, StartPoint, (DiagramPoint)middlePoint, Angle, label, color);
					Render(renderer, (DiagramPoint)middlePoint, FinishPoint, 0, label, color);
				}
			}
		}
	}
}
