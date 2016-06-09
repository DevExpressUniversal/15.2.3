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
	public class SideMarginCalculator {
		internal const double defaultSideMarginValue = 0.5;
		public static double CalcSideMargin(IAxisData axis, MinMaxValues internalValues, bool autoCorrectMin, bool autoCorrectMax, IEnumerable<IRefinedSeries> refinedSeries) {
			SideMarginCalculator calculator = new SideMarginCalculator(axis);
			return calculator.CalcSideMargin(internalValues, refinedSeries, autoCorrectMin, autoCorrectMax);
		}
		readonly IAxisData axis;
		ActualScaleType ScaleType { get { return axis.AxisScaleTypeMap.ScaleType; } }
		bool ShowZeroLevel {
			get {
				return !axis.IsArgumentAxis && axis.WholeRange.AlwaysShowZeroLevel && axis.VisualRange.AlwaysShowZeroLevel;
			}
		}
		SideMarginCalculator(IAxisData axis) {
			this.axis = axis;
		}
		double CalcSideMargin(MinMaxValues internalValues, IEnumerable<IRefinedSeries> refinedSeries, bool autoCorrectMin, bool autoCorrectMax) {
			double min = internalValues.Min;
			double max = internalValues.Max;
			double sideMargin = 0;
			if (double.IsNaN(min) || double.IsNaN(max))
				return 0;
			if (ShowZeroLevel) {
				double zeroLevel = GetZeroLevel(internalValues);
				if (min > zeroLevel && autoCorrectMin)
					min = zeroLevel;
				if (max < zeroLevel && autoCorrectMax)
					max = zeroLevel;
			}
			sideMargin = CalculateSideMarginBySeries(min, max, refinedSeries);
			if (IsOverflow(internalValues, sideMargin))
				sideMargin = Math.Abs(internalValues.Max) * 1e-5;
			return sideMargin;
		}
		double CalculateSideMarginBySeries(double min, double max, IEnumerable<IRefinedSeries> refinedSeries) {
			double sideMarginByRange = 0;
			double sideMarginByView = 0;
			IList<IMinMaxValues> rangeLimitsList = GetScaleBreakesRanges(min, max);
			if (rangeLimitsList != null) {
				foreach (IMinMaxValues range in rangeLimitsList)
					sideMarginByRange += CalculateSideMarginByRange(range.Min, range.Max);
			}
			else {
				sideMarginByRange = CalculateSideMarginByRange(min, max);
			}
			if (rangeLimitsList != null && rangeLimitsList.Count > 0)
				sideMarginByView = CalculateSideMarginByView(refinedSeries, rangeLimitsList[0].Min, rangeLimitsList[0].Max);
			else
				sideMarginByView = CalculateSideMarginByView(refinedSeries, min, max);
			double sideMargin = Math.Max(sideMarginByView, sideMarginByRange);
			if (axis.IsArgumentAxis) {
				if (ScaleType == ActualScaleType.Numerical && min != max)
					return sideMargin;
				return Math.Max(sideMargin, defaultSideMarginValue);
			}
			else
				return sideMargin == 0 ? defaultSideMarginValue : sideMargin;
		}
		double CalculateSideMarginByView(IEnumerable<IRefinedSeries> refinedSeries, double min, double max) {
			double sideMargin = 0.0;
			if (axis.IsArgumentAxis) {
				sideMargin = CalcAxisXSideMagrin(refinedSeries, min, max);
				if (sideMargin == 0)
					return 0;
				sideMargin += (max - min) * 0.01;
				return Math.Max(sideMargin, defaultSideMarginValue);
			}
			else {
				sideMargin = (max - min) * 0.01;
				return sideMargin;
			}
		}
		double CalcAxisXSideMagrin(IEnumerable<IRefinedSeries> refinedSeries, double min, double max) {
			double sideMargin = 0.0;
			if (refinedSeries == null)
				return sideMargin;
			foreach (IRefinedSeries series in refinedSeries) {
				if (series.Points.Count == 0)
					continue;
				double barWidthCorrection = GetBarWidthCorrection(series);
				sideMargin = Math.Max(sideMargin, barWidthCorrection);
				double markerSizeCorrection = CalcAxisXSideMarginForBubble(series, min, max);
				sideMargin = Math.Max(sideMargin, markerSizeCorrection);
			}
			return sideMargin;
		}
		double GetBarWidthCorrection(IRefinedSeries series) {
			ISeriesView seriesView = series.Series != null ? series.SeriesView : null;
			IBarSeriesView barView = seriesView as IBarSeriesView;
			if (barView != null)
				return barView.BarWidth / 2;
			return 0;
		}
		double CalcAxisXSideMarginForBubble(IRefinedSeries series, double min, double max) {
			ISeriesView seriesView = series.Series != null ? series.SeriesView : null;
			var bubbleSeriesView = seriesView as IXYWSeriesView;
			if (bubbleSeriesView != null)
				return 0.5 * bubbleSeriesView.GetSideMargins(min, max);
			return 0;
		}
		IList<IMinMaxValues> GetScaleBreakesRanges(double min, double max) {
			return axis.CalculateRangeLimitsList(min, max);
		}
		double CalculateSideMarginByRange(double min, double max) {
			if (axis.IsArgumentAxis) {
				if (ScaleType == ActualScaleType.Numerical)
					return CalculateDefaultArgumentAxisSideMargin(min, max);
				else
					return 0;
			}
			else
				return CalculateDefaultValueAxisSideMargin(min, max);
		}
		double CalculateDefaultArgumentAxisSideMargin(double min, double max) {
			return (max - min) / 15;
		}
		double CalculateDefaultValueAxisSideMargin(double min, double max) {
			double delta = max - min;
			if (delta == 0)
				delta = Math.Abs(max);
			return delta / 10;
		}
		double GetZeroLevel(MinMaxValues internalValues) {
			if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime)
				return internalValues.Min;
			return 0;
		}
		bool IsOverflow(MinMaxValues internalValues, double sideMargin) {
			return internalValues.Min + sideMargin == internalValues.Min || internalValues.Max + sideMargin == internalValues.Max;
		}
	}
	public class SideMarginLogarithmicCalculator {
		const double defaultSideMarginValue = 0.5;
		public static double CalcSideMargin(IAxisData axis, MinMaxValues internalValues, IEnumerable<RefinedSeries> refinedSeries) {
			SideMarginLogarithmicCalculator calculator = new SideMarginLogarithmicCalculator(axis);
			return calculator.CalcSideMargin(internalValues, refinedSeries);
		}
		readonly IAxisData axis;
		bool ShowZeroLevel {
			get {
				return !axis.IsArgumentAxis && axis.WholeRange.AlwaysShowZeroLevel && axis.VisualRange.AlwaysShowZeroLevel;
			}
		}
		SideMarginLogarithmicCalculator(IAxisData axis) {
			this.axis = axis;
		}
		double CalcSideMargin(MinMaxValues internalValues, IEnumerable<RefinedSeries> refinedSeries) {
			double min = internalValues.Min;
			double max = internalValues.Max;
			double sideMargin = 0;
			if (ShowZeroLevel) {
				double zeroLevel = GetZeroLevel(internalValues);
				if (min > zeroLevel)
					min = zeroLevel;
				if (max < zeroLevel)
					max = zeroLevel;
			}
			min = axis.AxisScaleTypeMap.Transformation.TransformForward(min);
			max = axis.AxisScaleTypeMap.Transformation.TransformForward(max);
			double delta = max - min;
			if (delta == 0) {
				if (axis.IsArgumentAxis)
					sideMargin = Math.Abs(max) / 15;
				else
					sideMargin = Math.Abs(max) / 10;
			}
			else {
				if (axis.IsArgumentAxis)
					sideMargin = delta / 15;
				else
					sideMargin = delta / 10;
			}
			if (IsOverflow(min, max, sideMargin, axis.AxisScaleTypeMap.Transformation))
				sideMargin = CalcOverflowedSideMargin(min, max, sideMargin, axis.AxisScaleTypeMap.Transformation);
			return sideMargin;
		}
		double GetZeroLevel(MinMaxValues internalValues) {
			if (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime)
				return internalValues.Min;
			return 0;
		}
		bool IsOverflow(double min, double max, double sideMargin, Transformation transformation) {
			return Double.IsNegativeInfinity(transformation.TransformBackward(min - sideMargin)) || Double.IsPositiveInfinity(transformation.TransformBackward(max + sideMargin));
		}
		double CalcOverflowedSideMargin(double min, double max, double sideMargin, Transformation transformation) {
			double transformedMin = transformation.TransformForward(Double.MinValue);
			double transformedMax = transformation.TransformForward(Double.MaxValue);
			double lowSideMargin = min - transformedMin;
			double highSideMargin = transformedMax - max;
			return Math.Min(sideMargin, Math.Min(lowSideMargin, highSideMargin));
		}
	}
}
