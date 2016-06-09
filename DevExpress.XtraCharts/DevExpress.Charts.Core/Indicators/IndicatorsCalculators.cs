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
using System.Globalization;
namespace DevExpress.Charts.Native {
	public enum ValueLevelInternal {
		Value,
		Value_1,
		Value_2,
		Low,
		High,
		Open,
		Close,
		Weight
	}
	public abstract class RegressionCalculator {
		protected Tuple<double, double> CalculateKAndB(IRefinedSeries series, ValueLevelInternal valueLevel) {
			double k = 0, b = 0;
			if (series.Points.Count > 1) {
				double minArgument = series.MinArgument;
				double xSum = 0;
				double x2Sum = 0;
				double ySum = 0;
				double xySum = 0;
				int count = 0;
				foreach (RefinedPoint refinedPoint in series.Points)
					if (!refinedPoint.IsEmpty) {
						double x = refinedPoint.Argument - minArgument;
						double y = refinedPoint.GetValue(valueLevel);
						xSum += x;
						x2Sum += x * x;
						ySum += y;
						xySum += x * y;
						count++;
					}
				double divisor = x2Sum * count - xSum * xSum;
				if (divisor != 0) {
					k = (xySum * count - ySum * xSum) / divisor;
					b = (ySum * x2Sum - xSum * xySum) / divisor - minArgument * k;
					return new Tuple<double, double>(k, b);
				}
			}
			return null;
		}
	}
	public class RegressionLineCalculator : RegressionCalculator {
		public Tuple<GRealPoint2D, GRealPoint2D> Calculate(IRefinedSeries series, double x1, double x2, ValueLevelInternal valueLevel) {
			Tuple<double, double> kAndB = CalculateKAndB(series, valueLevel);
			if (kAndB != null) {
				double y1 = kAndB.Item1 * x1 + kAndB.Item2;
				double y2 = kAndB.Item1 * x2 + kAndB.Item2;
				return new Tuple<GRealPoint2D, GRealPoint2D>(new GRealPoint2D(x1, y1), new GRealPoint2D(x2, y2));
			}
			return null;
		}
	}
	public class LogarithmicRegressionCalculator : RegressionCalculator {
		public List<GRealPoint2D> Calculate(IRefinedSeries series, ValueLevelInternal valueLevel) {
			List<GRealPoint2D> regressionPoints = new List<GRealPoint2D>();
			Tuple<double, double> kAndB = CalculateKAndB(series, valueLevel);
			if (kAndB != null) {
				foreach (RefinedPoint refinedPoint in series.Points) {
					double y = kAndB.Item1 * Math.Log(refinedPoint.Argument) + kAndB.Item2;
					regressionPoints.Add(new GRealPoint2D(refinedPoint.Argument, y));
				}
			}
			return regressionPoints;
		}
	}
	public class ExponentialRegressionCalculator : RegressionCalculator {
		public List<GRealPoint2D> Calculate(IRefinedSeries series, ValueLevelInternal valueLevel) {
			List<GRealPoint2D> regressionPoints = new List<GRealPoint2D>();
			Tuple<double, double> kAndB = CalculateKAndB(series, valueLevel);
			if (kAndB != null) {
				foreach (RefinedPoint refinedPoint in series.Points) {
					double y = kAndB.Item1 * Math.Pow(Math.E, refinedPoint.Argument) + kAndB.Item2;
					regressionPoints.Add(new GRealPoint2D(refinedPoint.Argument, y));
				}
			}
			return regressionPoints;
		}
	}
	public abstract class FinancialIndicatorCalculator {
		public bool Calculated { get; protected set; }
		protected bool IsStartPointAboveEndPoint(GRealPoint2D startPoint, GRealPoint2D endPoint) {
			return startPoint.Y > endPoint.Y;
		}
		protected RefinedPoint GetPointByArgument(AxisScaleTypeMap axisXScaleTypeMap, IRefinedSeries seriesInfo, object argument, CultureInfo cultureInfo) {
			double internalArgument = axisXScaleTypeMap.NativeToFinalWithConvertion(argument, cultureInfo);
			if (!double.IsNaN(internalArgument))
				return seriesInfo.GetMinPoint(axisXScaleTypeMap.InternalToRefined(internalArgument));
			return null;
		}
		protected void CompareRefinedPointsByArgument(RefinedPoint refinedPoint1, ValueLevelInternal valueLevel1,
												   RefinedPoint refinedPoint2, ValueLevelInternal valueLevel2,
												   out RefinedPoint leftPointInfo, out ValueLevelInternal leftValueLevel,
												   out RefinedPoint rightPointInfo, out ValueLevelInternal rightValueLevel) {
			if (refinedPoint1.Argument < refinedPoint2.Argument) {
				leftPointInfo = refinedPoint1;
				leftValueLevel = valueLevel1;
				rightPointInfo = refinedPoint2;
				rightValueLevel = valueLevel2;
			}
			else {
				leftPointInfo = refinedPoint2;
				leftValueLevel = valueLevel2;
				rightPointInfo = refinedPoint1;
				rightValueLevel = valueLevel1;
			}
		}
	}
	public class FibonacciRetracementCalculator : FinancialIndicatorCalculator {
		public List<FibonacciLine> CalculateLines(GRealPoint2D startPoint, GRealPoint2D endPoint, IList<double> levels) {
			List<FibonacciLine> fibonacciLines = new List<FibonacciLine>();
			double hundredPercentInterval = Math.Abs(startPoint.Y - endPoint.Y);
			double multiplier = IsStartPointAboveEndPoint(startPoint, endPoint) ? -1 : 1;
			foreach (double level in levels) {
				double y = endPoint.Y - multiplier * hundredPercentInterval * level;
				FibonacciLine line = new FibonacciLine(level, startPoint.X, y, endPoint.X, y);
				fibonacciLines.Add(line);
			}
			Calculated = true;
			return fibonacciLines;
		}
		public List<FibonacciLine> Calculate(IRefinedSeries seriesInfo, AxisScaleTypeMap axisXScaleTypeMap, CultureInfo cultureInfo,
											  object argument1, ValueLevelInternal valueLevel1,
											  object argument2, ValueLevelInternal valueLevel2,
											  double axisXMaxValueInternal, IList<double> levels) {
			Calculated = false;
			if (seriesInfo == null || axisXScaleTypeMap == null || argument1 == null || argument2 == null || levels == null)
				return new List<FibonacciLine>();
			RefinedPoint refinedPoint1 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument1, cultureInfo);
			RefinedPoint refinedPoint2 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument2, cultureInfo);
			if (refinedPoint1 == null || refinedPoint2 == null || refinedPoint1.Argument == refinedPoint2.Argument)
				return new List<FibonacciLine>();
			RefinedPoint leftRefinedPoint, rightRefinedPoint;
			ValueLevelInternal leftValueLevel, rightValueLevel;
			CompareRefinedPointsByArgument(refinedPoint1, valueLevel1, refinedPoint2, valueLevel2,
										out leftRefinedPoint, out leftValueLevel, out rightRefinedPoint, out rightValueLevel);
			double leftValue = leftRefinedPoint.GetValue(leftValueLevel);
			double rightValue = rightRefinedPoint.GetValue(rightValueLevel);
			GRealPoint2D leftPoint = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(leftRefinedPoint.Argument), leftValue);
			GRealPoint2D rightPoint = new GRealPoint2D(axisXMaxValueInternal, rightValue);
			return CalculateLines(leftPoint, rightPoint, levels);
		}
	}
	public class FibonacciFansCalculator : FinancialIndicatorCalculator {
		public List<FibonacciLine> CalculateLines(GRealPoint2D startPoint, GRealPoint2D endPoint, IList<double> levels) {
			List<FibonacciLine> fibonacciLines = new List<FibonacciLine>();
			double hundredPercentInterval = Math.Abs(startPoint.Y - endPoint.Y);
			double multiplier = IsStartPointAboveEndPoint(startPoint, endPoint) ? -1 : 1;
			foreach (double level in levels) {
				double y = endPoint.Y - multiplier * hundredPercentInterval * level;
				FibonacciLine line = new FibonacciLine(level, startPoint, endPoint.X, y);
				fibonacciLines.Add(line);
			}
			Calculated = true;
			return fibonacciLines;
		}
		public List<FibonacciLine> Calculate(IRefinedSeries seriesInfo, AxisScaleTypeMap axisXScaleTypeMap, CultureInfo cultureInfo,
											  object argument1, ValueLevelInternal valueLevel1,
											  object argument2, ValueLevelInternal valueLevel2,
											  double maxAxisXCoord, IList<double> levels) {
			Calculated = false;
			if (seriesInfo == null || axisXScaleTypeMap == null || argument1 == null || argument2 == null || levels == null)
				return new List<FibonacciLine>();
			RefinedPoint pointInfo1 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument1, cultureInfo);
			RefinedPoint pointInfo2 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument2, cultureInfo);
			if (pointInfo1 == null || pointInfo2 == null)
				return new List<FibonacciLine>();
			RefinedPoint leftRefinedPoint, rightRefinedPoint;
			ValueLevelInternal leftValueLevel, rightValueLevel;
			CompareRefinedPointsByArgument(pointInfo1, valueLevel1, pointInfo2, valueLevel2,
										out leftRefinedPoint, out leftValueLevel, out rightRefinedPoint, out rightValueLevel);
			double leftValue = leftRefinedPoint.GetValue(valueLevel1);
			double rightValue = rightRefinedPoint.GetValue(valueLevel2);
			GRealPoint2D leftDiagramPoint = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(leftRefinedPoint.Argument), leftValue);
			GRealPoint2D rightDiagramPoint = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(rightRefinedPoint.Argument), rightValue);
			List<FibonacciLine> lines = CalculateLines(leftDiagramPoint, rightDiagramPoint, levels);
			foreach (FibonacciLine line in lines) {
				double yOutermost = (line.End.Y - line.Start.Y) * (maxAxisXCoord - leftDiagramPoint.X) / (rightDiagramPoint.X - leftDiagramPoint.X) + leftDiagramPoint.Y;
				line.End = new GRealPoint2D(maxAxisXCoord, yOutermost);
			}
			return lines;
		}
	}
	public class FibonacciArcsCalculator : FinancialIndicatorCalculator {
		public List<FibonacciCircle> Calculate(IRefinedSeries seriesInfo, AxisScaleTypeMap axisXScaleTypeMap, CultureInfo cultureInfo,
											object argument1, ValueLevelInternal valueLevel1,
											object argument2, ValueLevelInternal valueLevel2,
											double maxAxisXCoord, IList<double> levels) {
			Calculated = false;
			List<FibonacciCircle> fibonacciCircles = new List<FibonacciCircle>();
			if (seriesInfo == null || axisXScaleTypeMap == null || argument1 == null || argument2 == null || levels == null)
				return fibonacciCircles;
			RefinedPoint refinedPoint1 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument1, cultureInfo);
			RefinedPoint refinedPoint2 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument2, cultureInfo);
			if (refinedPoint1 == null || refinedPoint2 == null)
				return fibonacciCircles;
			RefinedPoint leftRefinedPoint, rightRefinedPoint;
			ValueLevelInternal leftValueLevel, rightValueLevel;
			CompareRefinedPointsByArgument(refinedPoint1, valueLevel1, refinedPoint2, valueLevel2,
										out leftRefinedPoint, out leftValueLevel, out rightRefinedPoint, out rightValueLevel);
			double leftValue = leftRefinedPoint.GetValue(valueLevel1);
			double rightValue = rightRefinedPoint.GetValue(valueLevel2);
			GRealPoint2D leftDiagramPoint = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(leftRefinedPoint.Argument), leftValue);
			GRealPoint2D rightDiagramPoint = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(rightRefinedPoint.Argument), rightValue);
			double distance = GRealPoint2D.CalculateDistance(leftDiagramPoint, rightDiagramPoint);
			GRealVector2D directingVector = new GRealVector2D(rightDiagramPoint, leftDiagramPoint);
			directingVector.Normalize();
			foreach (double level in levels) {
				double radius = level * distance;
				GRealPoint2D pointInCircle = rightDiagramPoint + directingVector * radius;
				FibonacciCircle circle = new FibonacciCircle(rightDiagramPoint, pointInCircle, level);
				fibonacciCircles.Add(circle);
			}
			Calculated = true;
			return fibonacciCircles;
		}
	}
	public class TrendLineCalculator : FinancialIndicatorCalculator {
		public GRealLine2D Calculate(IRefinedSeries seriesInfo, AxisScaleTypeMap axisXScaleTypeMap, CultureInfo cultureInfo,
									 object argument1, ValueLevelInternal valueLevel1,
									 object argument2, ValueLevelInternal valueLevel2,
									 bool extrapolateToInfinity, double axisXMaxValue) {
			Calculated = false;
			GRealLine2D line = new GRealLine2D();
			if (seriesInfo == null || axisXScaleTypeMap == null || argument1 == null || argument2 == null)
				return line;
			RefinedPoint refinedPoint1 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument1, cultureInfo);
			RefinedPoint refinedPoint2 = GetPointByArgument(axisXScaleTypeMap, seriesInfo, argument2, cultureInfo);
			if (refinedPoint1 == null || refinedPoint2 == null)
				return line;
			double value1 = refinedPoint1.GetValue(valueLevel1);
			double value2 = refinedPoint2.GetValue(valueLevel2);
			GRealPoint2D diagramPoint1 = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(refinedPoint1.Argument), value1);
			GRealPoint2D diagramPoint2 = new GRealPoint2D(axisXScaleTypeMap.RefinedToInternal(refinedPoint2.Argument), value2);
			Calculated = true;
			if (!extrapolateToInfinity)
				return new GRealLine2D(diagramPoint1, diagramPoint2);
			GRealPoint2D leftDiagramPoint;
			GRealPoint2D rightDiagramPoint;
			if (diagramPoint1.X < diagramPoint2.X) {
				leftDiagramPoint = diagramPoint1;
				rightDiagramPoint = diagramPoint2;
			}
			else {
				leftDiagramPoint = diagramPoint2;
				rightDiagramPoint = diagramPoint1;
			}
			double yMax;
			if (leftDiagramPoint.Y < rightDiagramPoint.Y)
				yMax = (axisXMaxValue - leftDiagramPoint.X) * (rightDiagramPoint.Y - leftDiagramPoint.Y) / (rightDiagramPoint.X - leftDiagramPoint.X) + leftDiagramPoint.Y;
			else
				yMax = -(axisXMaxValue - leftDiagramPoint.X) * (leftDiagramPoint.Y - rightDiagramPoint.Y) / (rightDiagramPoint.X - leftDiagramPoint.X) + leftDiagramPoint.Y;
			return new GRealLine2D(leftDiagramPoint, new GRealPoint2D(axisXMaxValue, yMax));
		}
	}
	public class MedianPriceCalculator {
		public bool Calculated {
			get; private set;
		}
		public MedianPriceCalculator() {
			Calculated = false;
		}
		bool IsParameterCorrect(IRefinedSeries refinedSeries) {
			return refinedSeries != null &&
				   refinedSeries.SeriesView is IFinancialSeriesView &&
				   refinedSeries.Points != null &&
				   refinedSeries.Points.Count > 0;
		}
		public List<GRealPoint2D> Calculate(IRefinedSeries refinedSeries) {
			Calculated = false;
			if (!IsParameterCorrect(refinedSeries))
				return new List<GRealPoint2D>(0);
			IList<RefinedPoint> refinedPoints = refinedSeries.Points;
			var indicatorPoints = new List<GRealPoint2D>(refinedPoints.Count);
			for (int i = 0; i < refinedPoints.Count; i++) {
				IFinancialPoint financialPoint = refinedPoints[i];
				double median = (financialPoint.High + financialPoint.Low) / 2.0;
				indicatorPoints.Add(new GRealPoint2D(financialPoint.Argument, median));
			}
			Calculated = true;
			return indicatorPoints;
		}
	}
	public class TypicalPriceCalculator {
		public bool Calculated {
			get; private set;
		}
		public TypicalPriceCalculator() {
			Calculated = false;
		}
		bool IsParameterCorrect(IRefinedSeries refinedSeries) {
			return refinedSeries != null &&
				   refinedSeries.SeriesView is IFinancialSeriesView &&
				   refinedSeries.Points != null &&
				   refinedSeries.Points.Count > 0;
		}
		public List<GRealPoint2D> Calculate(IRefinedSeries refinedSeries) {
			Calculated = false;			
			if (!IsParameterCorrect(refinedSeries))
				return new List<GRealPoint2D>(0);
			IList<RefinedPoint> refinedPoints = refinedSeries.Points;
			var indicatorPoints = new List<GRealPoint2D>(refinedPoints.Count);
			for (int i = 0; i < refinedPoints.Count; i++) {
				IFinancialPoint financialPoint = refinedPoints[i];
				double median = (financialPoint.High + financialPoint.Low + financialPoint.Close) / 3.0;
				indicatorPoints.Add(new GRealPoint2D(financialPoint.Argument, median));
			}
			Calculated = true;
			return indicatorPoints;
		}
	}
	public class WeightedCloseCalculator {
		public bool Calculated {
			get; private set;
		}
		public WeightedCloseCalculator() {
			Calculated = false;
		}
		bool IsParameterCorrect(IRefinedSeries refinedSeries) {
			return refinedSeries != null &&
				   refinedSeries.SeriesView is IFinancialSeriesView &&
				   refinedSeries.Points != null &&
				   refinedSeries.Points.Count > 0;
		}
		public List<GRealPoint2D> Calculate(IRefinedSeries refinedSeries) {
			Calculated = false;
			if (!IsParameterCorrect(refinedSeries))
				return new List<GRealPoint2D>(0);
			IList<RefinedPoint> refinedPoints = refinedSeries.Points;
			var indicatorPoints = new List<GRealPoint2D>(refinedPoints.Count);
			for (int i = 0; i < refinedPoints.Count; i++) {
				IFinancialPoint financialPoint = refinedPoints[i];
				double median = (financialPoint.High + financialPoint.Low + 2 * financialPoint.Close) / 4.0;
				indicatorPoints.Add(new GRealPoint2D(financialPoint.Argument, median));
			}
			Calculated = true;
			return indicatorPoints;
		}
	}
}
