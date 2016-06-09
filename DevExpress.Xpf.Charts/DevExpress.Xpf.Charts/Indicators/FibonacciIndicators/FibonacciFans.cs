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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class FibonacciFans : FibonacciIndicator {
		public static readonly DependencyProperty ShowLevel0Property = DependencyPropertyManager.Register("ShowLevel0",
		  typeof(bool), typeof(FibonacciFans), new PropertyMetadata(false, ChartElementHelper.Update));
		List<FibonacciLine> fibonacciLines;
		double minFibonacciLineLength;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowLevel0 {
			get { return (bool)GetValue(ShowLevel0Property); }
			set { SetValue(ShowLevel0Property, value); }
		}
		public FibonacciFans() {
			DefaultStyleKey = typeof(FibonacciFans);
		}
		public override Geometry CreateGeometry() {
			GeometryGroup geometry = new GeometryGroup();
			foreach (FibonacciLine line in this.fibonacciLines) {
				geometry.Children.Add(new LineGeometry() { StartPoint = line.ScreenStart.ToPoint(), EndPoint = line.ScreenEnd.ToPoint() });
			}
			return geometry;
		}
		protected internal override void CalculateLayout(IRefinedSeries refinedSeries) {
			base.CalculateLayout(refinedSeries);
			if (Object.Equals(Argument1, Argument2))
				return;
			AxisScaleTypeMap axisXScaleTypeMap = ((IAxisData)XYSeries.ActualAxisX).AxisScaleTypeMap;
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			double maxAxisXValue = ((IAxisData)XYSeries.ActualAxisX).VisualRange.Max;
			FibonacciFansCalculator calculator = new FibonacciFansCalculator();
			this.fibonacciLines = calculator.Calculate(refinedSeries, axisXScaleTypeMap, cultureInfo,
													   Argument1, (ValueLevelInternal)ValueLevel1,
													   Argument2, (ValueLevelInternal)ValueLevel2,
													   maxAxisXValue, GetFibonacciLevels());
			if (!calculator.Calculated)
				return;
			PaneMapping paneMapping = new PaneMapping(Pane, XYSeries);
			double minDistance = double.MaxValue;
			foreach (FibonacciLine line in this.fibonacciLines) {
				Point start = paneMapping.GetDiagramPoint(line.Start);
				Point end = paneMapping.GetDiagramPoint(line.End);
				line.ScreenStart = start.ToGRealPoint2D();
				line.ScreenEnd = end.ToGRealPoint2D();
				TruncateLine(line);
				double distance = GRealPoint2D.CalculateDistance(line.ScreenStart, line.ScreenEnd);
				if (distance < minDistance) {
					minDistance = distance;
				}
			}
			this.minFibonacciLineLength = minDistance;
			Item.IndicatorGeometry = new IndicatorGeometry(this);
		}
		protected internal override void CreateLabelItems() {
			base.CreateLabelItems();
			if (fibonacciLines == null)
				return;
			LabelItems = new List<IndicatorLabelItem>();
			foreach (FibonacciLine line in fibonacciLines) {
				string text = Math.Round(line.Level, 3).ToString();
				IndicatorLabelItem labelItem = new IndicatorLabelItem(this, text, line);
				LabelItems.Add(labelItem);
			}
		}
		protected internal override IndicatorLabelLayout CalculateLabelLayout(IndicatorLabelItem labelItem, Size labelSize, object dataForLayoutCalculation) {
			FibonacciLine fibonacciLine = dataForLayoutCalculation as FibonacciLine;
			if (fibonacciLine == null)
				return null;
			GeometricLine fibonacciGeometricLine = new GeometricLine(fibonacciLine.ScreenStart, fibonacciLine.ScreenEnd);
			double angleOfInclination = Math.Atan2(fibonacciLine.ScreenEnd.Y - fibonacciLine.ScreenStart.Y, fibonacciLine.ScreenEnd.X - fibonacciLine.ScreenStart.X);
			double angleDegree = MathUtils.Radian2Degree(angleOfInclination);
			if (Pane.Rotated) {
				if (angleDegree < -90)
					angleDegree = -90 - angleDegree;
				else
					angleDegree = 90 - angleDegree;
			}
			double labelRotationAngle = CalculateLabelRotationAngle(angleDegree);
			Vector directingVector = new Vector(fibonacciLine.ScreenEnd.X - fibonacciLine.ScreenStart.X, fibonacciLine.ScreenEnd.Y - fibonacciLine.ScreenStart.Y);
			directingVector.Normalize();
			Vector normal = fibonacciGeometricLine.GetNormalizedNormalVector();
			double additionalOffset = Item.LineStyle.Thickness;
			Point centerLabelLocation = fibonacciLine.ScreenStart.ToPoint();
			centerLabelLocation += normal * (additionalOffset + labelSize.Height / 2.0);
			centerLabelLocation += directingVector * (this.minFibonacciLineLength * 2 / 3);
			return new IndicatorLabelLayout(centerLabelLocation, labelSize, labelRotationAngle, new Point(0.5, 0.5));
		}
		protected override IList<double> GetFibonacciLevels() {
			IList<double> fibonacciLevels = base.GetFibonacciLevels();
			if (ShowLevel0)
				fibonacciLevels.Add(0.0);
			return fibonacciLevels;
		}
		protected override Indicator CreateInstance() {
			return new FibonacciFans();
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			FibonacciFans fibonacciFans = indicator as FibonacciFans;
			if (fibonacciFans != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciFans, ShowLevel0Property);
			}
		}
		double CalculateCantingAngleDegree(FibonacciLine line) {
			PaneMapping mapping = new PaneMapping(Pane, XYSeries);
			double angleRadian = Math.Atan((line.ScreenEnd.Y - line.ScreenStart.Y) / (line.ScreenEnd.X - line.ScreenStart.X));
			return MathUtils.Radian2Degree(angleRadian);
		}
		void TruncateLine(FibonacciLine line) {
			GeometricLine top;
			GeometricLine left;
			GeometricLine right;
			GeometricLine bottom;
			if (Pane.Rotated) {
				top = new GeometricLine(new Point(Pane.Viewport.Height, 0), new Point(Pane.Viewport.Height, Pane.Viewport.Width));
				left = new GeometricLine(new Point(0, 0), new Point(Pane.Viewport.Height, 0));
				right = new GeometricLine(new Point(0, Pane.Viewport.Width), new Point(Pane.Viewport.Height, Pane.Viewport.Width));
				bottom = new GeometricLine(new Point(0, 0), new Point(0, Pane.Viewport.Width));
			}
			else {
				top = new GeometricLine(new Point(0, Pane.Viewport.Height), new Point(Pane.Viewport.Width, Pane.Viewport.Height));
				left = new GeometricLine(new Point(0, 0), new Point(0, Pane.Viewport.Height));
				right = new GeometricLine(new Point(Pane.Viewport.Width, 0), new Point(Pane.Viewport.Width, Pane.Viewport.Height));
				bottom = new GeometricLine(new Point(0, 0), new Point(Pane.Viewport.Width, 0));
			}
			GeometricLine fibonacciGeometricLine = new GeometricLine(line.ScreenStart, line.ScreenEnd);
			Point intersectWithBound = CalculateNearest(line, top, left, right, bottom);
			line.ScreenEnd = intersectWithBound.ToGRealPoint2D();
		}
		Point CalculateNearest(FibonacciLine fibonacciLine, GeometricLine top, GeometricLine left, GeometricLine right, GeometricLine bottom) {
			GeometricLine fibLine = new GeometricLine(fibonacciLine.ScreenStart, fibonacciLine.ScreenEnd);
			List<Point> intersectPoints = new List<Point>();
			List<Point> foundPoints = null;
			intersectPoints.Add(GeometricLine.CalculateIntersectionPoint(fibLine, top));
			intersectPoints.Add(GeometricLine.CalculateIntersectionPoint(fibLine, left));
			intersectPoints.Add(GeometricLine.CalculateIntersectionPoint(fibLine, right));
			intersectPoints.Add(GeometricLine.CalculateIntersectionPoint(fibLine, bottom));
			if (fibonacciLine.ScreenEnd.X > fibonacciLine.ScreenStart.X && fibonacciLine.ScreenEnd.Y > fibonacciLine.ScreenStart.Y)
				foundPoints = FindAll(intersectPoints, point => ((point.X > fibonacciLine.ScreenStart.X) && (point.Y > fibonacciLine.ScreenStart.Y)));			
			else if (fibonacciLine.ScreenEnd.X > fibonacciLine.ScreenStart.X && fibonacciLine.ScreenEnd.Y < fibonacciLine.ScreenStart.Y)
				foundPoints = FindAll(intersectPoints, point => ((point.X > fibonacciLine.ScreenStart.X) && (point.Y < fibonacciLine.ScreenStart.Y)));		   
			else if (fibonacciLine.ScreenEnd.X < fibonacciLine.ScreenStart.X && fibonacciLine.ScreenEnd.Y > fibonacciLine.ScreenStart.Y)
				foundPoints = FindAll(intersectPoints, point => ((point.X < fibonacciLine.ScreenStart.X) && (point.Y > fibonacciLine.ScreenStart.Y)));
			else if (fibonacciLine.ScreenEnd.X < fibonacciLine.ScreenStart.X && fibonacciLine.ScreenEnd.Y < fibonacciLine.ScreenStart.Y)
				foundPoints = FindAll(intersectPoints, point => ((point.X < fibonacciLine.ScreenStart.X) && (point.Y < fibonacciLine.ScreenStart.Y)));
			else {
				ChartDebug.Assert(false, "Intersection point not found");
				foundPoints = new List<Point>();
			}
			double minDistance = double.MaxValue;
			Point result = new Point(double.MaxValue, double.MaxValue);
			foreach (Point p in foundPoints) {
				double distance = p.CalculateDistanceTo(fibonacciLine.ScreenStart.ToPoint());
				if (distance < minDistance) {
					minDistance = distance;
					result = p;
				}
			}
			return result;
		}
		double CalculateLabelRotationAngle(double angleDegreeCounterclockwise) {
			if (-90 <= angleDegreeCounterclockwise && angleDegreeCounterclockwise <= 90)
				return -angleDegreeCounterclockwise;
			if (-180 <= angleDegreeCounterclockwise && angleDegreeCounterclockwise < -90)
				return -(180 - Math.Abs(angleDegreeCounterclockwise));
			if (90 < angleDegreeCounterclockwise && angleDegreeCounterclockwise <= 180)
				return 180 - Math.Abs(angleDegreeCounterclockwise);
			ChartDebug.Assert(false, "FibonacciFuns LabelRotationAngle has not calcualted. Given angle is not between -180 and 180 degrees");
			return 0; 
		}
		List<Point> FindAll(List<Point> list, Predicate<Point> predicate) {
			List<Point> result = new List<Point>();
			foreach (Point point in list)
				if (predicate(point))
					result.Add(point);
			return result;
		}
	}
}
