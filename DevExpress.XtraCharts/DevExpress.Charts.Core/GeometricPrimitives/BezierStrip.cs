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
namespace DevExpress.Charts.Native {
	public static class LargeScaleHelper {
		const int mistakeCriterion = 0x7FFFF8;
		static bool IsValid(double value) {
			return value <= mistakeCriterion && value >= -mistakeCriterion;
		}
		static bool IsValid(GRealPoint2D point) {
			return IsValid(point.X) && IsValid(point.Y);
		}
		static double Validate(double value) {
			return value > mistakeCriterion ? mistakeCriterion : value < -mistakeCriterion ? -mistakeCriterion : value;
		}
		static GRealPoint2D Validate(GRealPoint2D point) {
			if (IsValid(point.X))
				return IsValid(point.Y) ? point : new GRealPoint2D(point.X, Validate(point.Y));
			if (IsValid(point.Y))
				return new GRealPoint2D(Validate(point.X), point.Y);
			return Math.Abs(point.X) > Math.Abs(point.Y) ?
				new GRealPoint2D(Validate(point.X), point.Y * Math.Abs(mistakeCriterion / point.X)) :
				new GRealPoint2D(point.X * Math.Abs(mistakeCriterion / point.Y), Validate(point.Y));
		}
		public static bool IsValid(LineStrip strip) {
			foreach (GRealPoint2D point in strip)
				if (!IsValid(point))
					return false;
			return true;
		}
		public static bool IsValid(List<IGeometryStrip> strips) {
			foreach (LineStrip strip in strips)
				if (!IsValid(strip))
					return false;
			return true;
		}
		public static void Validate(List<GRealPoint2D> points) {
			for (int i = 0; i < points.Count; i++)
				if (!IsValid(points[i]))
					points[i] = Validate(points[i]);
		}
		public static GRealRect2D Validate(GRealRect2D rect) {
			return (rect.Width <= mistakeCriterion && rect.Height <= mistakeCriterion) ? rect :
				new GRealRect2D(rect.Left, rect.Top, Validate(rect.Width), Validate(rect.Height));
		}
	}
	public class BezierCalculator {
		public static List<GRealPoint2D> Calculate(List<GRealPoint2D> points, double lineTension) {
			List<GRealPoint2D> resultPoints = new List<GRealPoint2D>();
			if (points.Count > 1) {
				int pointsCount = points.Count;
				double correctedTension = lineTension / 6;
				resultPoints.Add(points[0]);
				resultPoints.Add(CalculateCurveBezierEndPoint(points[0], points[1], correctedTension));
				for (int i = 0; i < pointsCount - 2; i++) {
					GRealPoint2D diff = new GRealPoint2D(points[i + 2].X - points[i].X, points[i + 2].Y - points[i].Y);
					double length1 = GeometricUtils.CalcDistance(points[i], points[i + 1]);
					double length2 = GeometricUtils.CalcDistance(points[i + 1], points[i + 2]);
					if (length1 > 0)
						resultPoints.Add(new GRealPoint2D(points[i + 1].X - correctedTension * diff.X, points[i + 1].Y - correctedTension * diff.Y));
					else
						resultPoints.Add(points[i + 1]);
					resultPoints.Add(points[i + 1]);
					if (length2 > 0)
						resultPoints.Add(new GRealPoint2D(points[i + 1].X + correctedTension * diff.X, points[i + 1].Y + correctedTension * diff.Y));
					else
						resultPoints.Add(points[i + 1]);
				}
				resultPoints.Add(CalculateCurveBezierEndPoint(points[pointsCount - 1], points[pointsCount - 2], correctedTension));
				resultPoints.Add(points[pointsCount - 1]);
			}
			else if (points.Count > 0)
				resultPoints.Add(points[0]);		  
			return resultPoints;
		}
		static GRealPoint2D CalculateCurveBezierEndPoint(GRealPoint2D endPoint, GRealPoint2D adjPoint, double tension) {
			return new GRealPoint2D(tension * (adjPoint.X - endPoint.X) + endPoint.X, tension * (adjPoint.Y - endPoint.Y) + endPoint.Y);
		}
	}
	public class BezierStrip : LineStrip, IBezierStrip {
		public static MinMaxValues CalculateMinMaxValues(IList<GRealPoint2D> points) {
			double min = double.MaxValue;
			double max = double.MinValue;
			for (int i = 0; i < points.Count; i++) {
				double value = points[i].Y;
				if (value < min)
					min = value;
				if (value > max)
					max = value;
			}
			return new MinMaxValues(min, max);
		}
		public static MinMaxValues CalculateMinMaxArguments(IList<GRealPoint2D> points) {
			double min = double.MaxValue;
			double max = double.MinValue;
			for (int i = 0; i < points.Count; i++) {
				double argument = points[i].X;
				if (argument < min)
					min = argument;
				if (argument > max)
					max = argument;
			}
			return new MinMaxValues(min, max);
		}
		bool clipLargeValues = true;
		double lineTension;
		List<GRealPoint2D> drawingPoints;
		ITransformation transformationX;
		ITransformation transformationY;
		public bool ClipLargeValues { get { return clipLargeValues; } set { clipLargeValues = value; } }
		public double LineTension { get { return lineTension; } }
		public List<GRealPoint2D> DrawingPoints { get { return drawingPoints; } }
		public BezierStrip()
			: base() {
			lineTension = 0.8;
		}
		public BezierStrip(double lineTension, ITransformation transformationX, ITransformation transformationY)
			: this(lineTension) {
			this.transformationX = transformationX;
			this.transformationY = transformationY;
		}
		public BezierStrip(double lineTension) {
			this.lineTension = lineTension;
		}
		List<GRealPoint2D> TransformPointsForward(List<GRealPoint2D> points) {
			List<GRealPoint2D> resultPoints = new List<GRealPoint2D>();
			foreach (GRealPoint2D point in points) {
				double x = (transformationX != null) ? transformationX.TransformForward(point.X) : point.X;
				double y = (transformationY != null) ? transformationY.TransformForward(point.Y) : point.Y;
				resultPoints.Add(new GRealPoint2D(x, y));
			}
			return resultPoints;
		}
		List<GRealPoint2D> TransformPointsBackward(List<GRealPoint2D> points) {
			List<GRealPoint2D> resultPoints = new List<GRealPoint2D>();
			foreach (GRealPoint2D point in points) {
				double x = (transformationX != null) ? transformationX.TransformBackward(point.X) : point.X;
				double y = (transformationY != null) ? transformationY.TransformBackward(point.Y) : point.Y;
				resultPoints.Add(new GRealPoint2D(x, y));
			}
			return resultPoints;
		}
		public List<GRealPoint2D> GetPointsForDrawing(bool completeFilling, bool clip) {
			if (completeFilling)
				CompleteFilling();
			if (clip && clipLargeValues) {
				List<GRealPoint2D> points = drawingPoints.GetRange(0, drawingPoints.Count);
				LargeScaleHelper.Validate(points);
				return points;
			}
			return drawingPoints;
		}
		public List<GRealPoint2D> GetPointsForDrawingWithoutValidate(bool completeFilling, bool clip) {
			if (completeFilling)
				CompleteFilling();
			if (clip) {
				List<GRealPoint2D> points = drawingPoints.GetRange(0, drawingPoints.Count);
				return points;
			}
			return drawingPoints;
		}
		public void SetPointsForDrawing(List<GRealPoint2D> points) {
			drawingPoints = points;
		}
		public override LineStrip CreateInstance() {
			return new BezierStrip(lineTension);
		}
		public override void CompleteFilling() {
			if (drawingPoints == null) {
				List<GRealPoint2D> transformedPoints = TransformPointsForward(this);
				List<GRealPoint2D> bezierPoints = BezierCalculator.Calculate(transformedPoints, LineTension);
				drawingPoints = TransformPointsBackward(bezierPoints);
			}
		}
		public override void Extend(GRealPoint2D point, bool toLeft) {
			if (IsEmpty)
				base.Extend(point, toLeft);
			if (toLeft) {
				Insert(0, this[0]);
				Insert(0, point);
				Insert(0, point);
			}
			else {
				Add(this[Count - 1]);
				Add(point);
				Add(point);
			}
			if (drawingPoints != null)
				if (toLeft) {
					GRealPoint2D leftPoint = drawingPoints[0];
					drawingPoints.Insert(0, leftPoint);
					drawingPoints.Insert(0, leftPoint);
					drawingPoints.Insert(0, leftPoint);
					drawingPoints.Insert(0, GeometricUtils.CalcPointInLine(leftPoint, point, 1 / 3));
					drawingPoints.Insert(0, GeometricUtils.CalcPointInLine(leftPoint, point, 1 / 1.5));
					drawingPoints.Insert(0, point);
					drawingPoints.Insert(0, point);
					drawingPoints.Insert(0, point);
					drawingPoints.Insert(0, point);
				}
				else {
					GRealPoint2D rightPoint = drawingPoints[drawingPoints.Count - 1];
					drawingPoints.Add(rightPoint);
					drawingPoints.Add(rightPoint);
					drawingPoints.Add(rightPoint);
					drawingPoints.Add(GeometricUtils.CalcPointInLine(rightPoint, point, 1 / 3));
					drawingPoints.Add(GeometricUtils.CalcPointInLine(rightPoint, point, 1 / 1.5));
					drawingPoints.Add(point);
					drawingPoints.Add(point);
					drawingPoints.Add(point);
					drawingPoints.Add(point);
				}
		}
		public override LineStrip ExtractSubStrip(int startIndex, int endIndex) {
			LineStrip strip = base.ExtractSubStrip(startIndex, endIndex);
			BezierStrip bezierStrip = strip as BezierStrip;
			if (bezierStrip != null && drawingPoints != null && drawingPoints.Count > 0) {
				int startDrawingPointsIndex = startIndex * 3;
				int count = endIndex * 3 - startDrawingPointsIndex + 1;
				List<GRealPoint2D> extractedDrawingPoints = new List<GRealPoint2D>(count);
				extractedDrawingPoints.AddRange(drawingPoints.GetRange(startDrawingPointsIndex, count));
				bezierStrip.drawingPoints = extractedDrawingPoints;
			}
			return strip;
		}
		public override void Substiture(int startIndex, int endIndex, LineStrip lineStrip) {
			base.Substiture(startIndex, endIndex, lineStrip);
			BezierStrip bezierStrip = lineStrip as BezierStrip;
			if (bezierStrip != null && drawingPoints != null && bezierStrip.drawingPoints != null) {
				int startDrawingPointsIndex = startIndex * 3;
				drawingPoints.RemoveRange(startDrawingPointsIndex, endIndex * 3 - startDrawingPointsIndex + 1);
				drawingPoints.InsertRange(startDrawingPointsIndex, bezierStrip.drawingPoints);
			}
			else
				drawingPoints = null;
		}
		public override LineStrip CreateUniqueStrip() {
			CompleteFilling();
			BezierStrip strip = new BezierStrip(lineTension);
			strip.AddRange(this);
			if (drawingPoints != null)
				strip.drawingPoints = new List<GRealPoint2D>(drawingPoints);
			return strip;
		}
		public MinMaxValues CalculateMinMaxValues() {
			List<GRealPoint2D> points = GetPointsForDrawing(true, false);
			return CalculateMinMaxValues(points);
		}
		public MinMaxValues CalculateMinMaxArguments() {
			List<GRealPoint2D> points = GetPointsForDrawing(true, false);
			return CalculateMinMaxArguments(points);
		}
	}
}
