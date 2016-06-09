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
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class CircularAxisMapping : AxisMappingEx {
		class Segment {
			public double StartAngle { get; set; }
			public double FinishAngle { get; set; }
			public double MinValue { get; set; }
			public double MaxValue { get; set; }
			public double ValueDifference { get { return MaxValue - MinValue; } }
			public double SweepAngle { get { return FinishAngle - StartAngle; } } 
		}
		readonly List<Segment> segments = new List<Segment>();
		readonly double axisRadius;
		readonly Point axisCenter;
		readonly double startAngleInRadian;
		readonly CircularDiagramShapeStyle diagramShapeStyle;
		readonly CircularDiagramRotationDirection rotationDirection;
		readonly bool looped;
		readonly double maxAxisValue;
		readonly double minAxisValue;
		public CircularAxisMapping(CircularAxisX2D axis, double radius, IList<double> values, bool looped) : base(axis, 2*Math.PI*radius) {
			CircularDiagram2D diagram = axis.CircularDiagram;
			this.looped = looped;
			this.startAngleInRadian = diagram != null ? MathUtils.Degree2Radian(diagram.StartAngle) : 0.0;
			this.rotationDirection = diagram != null ? diagram.RotationDirection : CircularDiagramRotationDirection.Counterclockwise;
			this.diagramShapeStyle = diagram != null ? diagram.ShapeStyle : CircularDiagramShapeStyle.Circle;
			this.axisCenter = diagram != null ? diagram.ActualViewport.CalcRelativeToLeftTopCenter() : new Point(0, 0);
			this.axisRadius = radius;
			double sweepAngle = 2 * Math.PI / (values.Count - 1);
			for (int i = 0; i < values.Count - 1; i++) {
				double segmentStartAngle;
				double segmentFinishAngle;
				if (rotationDirection == CircularDiagramRotationDirection.Counterclockwise) {
					segmentStartAngle = sweepAngle * i + Math.PI / 2.0 + startAngleInRadian;
					segmentFinishAngle = sweepAngle * (i + 1) + Math.PI / 2.0 + startAngleInRadian;
				}
				else {
					segmentStartAngle = -sweepAngle * i + Math.PI / 2.0 - startAngleInRadian;
					segmentFinishAngle = -sweepAngle * (i + 1) + Math.PI / 2.0 - startAngleInRadian;
				}
				Segment segment = new Segment() {
					MinValue = Math.Min(values[i], values[i + 1]),
					MaxValue = Math.Max(values[i], values[i + 1]), 
					StartAngle  = segmentStartAngle,
					FinishAngle = segmentFinishAngle
				};
				segments.Add(segment);
			}
			if (segments.Count > 0) {
				this.maxAxisValue = segments[segments.Count - 1].MaxValue;
				this.minAxisValue = segments[0].MinValue;
			}
		}
		double GetNormalizedAngleRadian(Segment segment, double normalizedValue) {
			if (segment == null)
				return startAngleInRadian;
			double factor = segment.ValueDifference > 0 ? (normalizedValue - segment.MinValue) / segment.ValueDifference : 0;
			return GeometricUtils.NormalizeRadian(segment.StartAngle + factor * segment.SweepAngle);
		}
		double GetValueScaleByAngle(Segment segment, double angle) {
			Vector startSegmentVector = new Vector(Math.Cos(segment.StartAngle), Math.Sin(segment.StartAngle));
			Vector finishSegmentVector = new Vector(Math.Cos(segment.FinishAngle), Math.Sin(segment.FinishAngle));
			Vector directingVector = new Vector(Math.Cos(angle), Math.Sin(angle));
			Point point1 = Vector.Add(startSegmentVector * axisRadius, axisCenter);
			Point point2 = Vector.Add(finishSegmentVector * axisRadius, axisCenter);
			Point point = Vector.Add(directingVector * axisRadius, axisCenter);
			GRealPoint2D grPoint1 = new GRealPoint2D(point1.X, point1.Y);
			GRealPoint2D grPoint2 = new GRealPoint2D(point2.X, point2.Y);
			GRealPoint2D grCenter = new GRealPoint2D(axisCenter.X, axisCenter.Y);
			GRealPoint2D grPoint = new GRealPoint2D(point.X, point.Y);
			GRealPoint2D? nullableIntersectionPoint = GeometricUtils.CalcLinesIntersection(grPoint1, grPoint2, grCenter, grPoint, false);
			if (nullableIntersectionPoint == null)
				return 1;
			GRealPoint2D intersectionPoint = (GRealPoint2D)nullableIntersectionPoint;
			double len = GeometricUtils.CalcDistance(grCenter, intersectionPoint);
			return len / axisRadius;
		}
		double GetValueScale(Segment segment, double normalizedValue){
			if (segment == null)
				return 1;
			double angle = GetNormalizedAngleRadian(segment, normalizedValue);
			return GetValueScaleByAngle(segment, angle);
		}
		double NormalizeValue(double value) { 
			if(segments.Count > 0) {
				double minValue = segments[0].MinValue;
				double maxValue = segments[segments.Count - 1].MaxValue;
				double range = maxValue - minValue;
				if (range > 0.0) {
					while (value < minValue)
						value += range;
					while (value > maxValue)
						value -= range;
				}
			}
			return value;
		}
		Segment FindSegment(double value) {
			if (value > maxAxisValue || segments.Count == 0 || value < minAxisValue)
				return null;
			if (value == maxAxisValue)
				return segments[segments.Count - 1];
			double segmentLen = segments[0].ValueDifference;
			int segmentindex = (int)((value-minAxisValue) / segmentLen);
			return segments[segmentindex];
		}
		Segment FindSegmentByAngle(double angle) {
			if (segments.Count == 0)
				return null;
			angle = GeometricUtils.NormalizeRadian(angle);
			double segmentLen = 2.0 * Math.PI / segments.Count;
			int segmentindex = (int)((angle) / segmentLen);
			return segments[segmentindex];
		}
		Point GetPoint(double angle, double radius, Point center) {
			double directingVectorXCoord = Math.Cos(angle);
			double directionVectorYCoord = -Math.Sin(angle);
			return new Point(center.X + directingVectorXCoord * radius, center.Y + directionVectorYCoord * radius);
		}
		public double GetNormalizedAngleRadian(double value) {
			double normalizedValue = looped ? NormalizeValue(value) : value;
			Segment segment = FindSegment(normalizedValue);
			return GetNormalizedAngleRadian(segment, normalizedValue);
		}
		public double GetValueScale(double value) {
			if (diagramShapeStyle == CircularDiagramShapeStyle.Circle)
				return 1;
			double normalizedValue = looped ? NormalizeValue(value) : value;
			Segment segment = FindSegment(normalizedValue);
			return GetValueScale(segment, value);
		}
		public double GetValueScaleByAngle(double angle) {
			if (diagramShapeStyle == CircularDiagramShapeStyle.Circle)
				return 1;
			Segment segment = FindSegmentByAngle(angle);
			return GetValueScaleByAngle(segment, angle);
		}
		public Point GetPointOnCircularAxis(double value, Point center) {
			return GetPoint(GetNormalizedAngleRadian(value), axisRadius, center);
		}
		public Point GetPointOnCircularAxis(double value) {
			return GetPointOnCircularAxis(value, axisCenter);
		}
		public bool IsLastValue(double value) {
			return segments.Count > 0 ? segments[segments.Count - 1].MaxValue == value : false;	
		}
		public List<Point> GetMeshPoints(double radius) {
			List<Point> meshPoints = new List<Point>();
			foreach (Segment segment in segments) 
				meshPoints.Add(GetPoint(segment.StartAngle, radius, axisCenter));
			return meshPoints;
		}
		public bool ValueInRange(double value) { 
			double normalizedValue = looped ? NormalizeValue(value) : value;
			return FindSegment(normalizedValue) != null;
		}
	}
}
