﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public class AverageTrueRangeCalculator {
		const int DesignTimePointsCount = 3;
		public bool Calculated { get; private set; }
		public double MinY { get; private set; }
		public double MaxY { get; private set; }
		public AverageTrueRangeCalculator() {
			Calculated = false;
			MinY = 0.0;
			MaxY = 1.0;
		}
		double CalcTrueRange(IFinancialPoint refinedPoint, IFinancialPoint preRefinedPoint) {
			double value1 = refinedPoint.High - refinedPoint.Low;
			double value2 = Math.Abs(refinedPoint.High - preRefinedPoint.Close);
			double value3 = Math.Abs(refinedPoint.Low - preRefinedPoint.Close);
			return Max(value1, value2, value3);
		}
		double Max(double val1, double val2, double val3) {
			double max1 = Math.Max(val1, val2);
			double max2 = Math.Max(max1, val3);
			return max2;
		}
		bool IsParametersCorrect(IRefinedSeries refinedSeries, int pointsCount) {
			return
				refinedSeries != null &&
				refinedSeries.Points != null &&
				pointsCount > 0 &&
				refinedSeries.Points.Count > pointsCount &&
				refinedSeries.SeriesView is IFinancialSeriesView;
		}
		public List<GRealPoint2D> Calculate(IRefinedSeries refinedSeries, int pointsCount) { 
			Calculated = false;
			int actualPointsCount = pointsCount;
			if (refinedSeries != null && refinedSeries.IsPointsAutoGenerated)
				actualPointsCount = DesignTimePointsCount;
			if (!IsParametersCorrect(refinedSeries, actualPointsCount))
				return new List<GRealPoint2D>(0);
			IList<RefinedPoint> refinedPoints = refinedSeries.Points;
			IFinancialPoint firstPoint = refinedPoints[0];
			double adder = firstPoint.High - firstPoint.Low;
			for (int i = 1; i < actualPointsCount; i++) {
				RefinedPoint refinedPoint = refinedPoints[i];
				RefinedPoint preRefinedPoint = refinedPoints[i - 1];
				double trueRange = CalcTrueRange(refinedPoint, preRefinedPoint);
				adder += trueRange;
			}
			double preAtr = adder / actualPointsCount;
			var indicatorPoints = new List<GRealPoint2D>();
			double firstArgument = refinedPoints[actualPointsCount - 1].Argument;
			indicatorPoints.Add(new GRealPoint2D(firstArgument, preAtr));
			double minIndicatorValue = preAtr;
			double maxIndicatorValue = preAtr;
			for (int i = actualPointsCount; i < refinedPoints.Count; i++) {
				RefinedPoint refinedPoint = refinedPoints[i];
				RefinedPoint preRefinedPoint = refinedPoints[i - 1];
				double trueRange = CalcTrueRange(refinedPoint, preRefinedPoint);
				double atr = (preAtr * (actualPointsCount - 1) + trueRange) / actualPointsCount;
				indicatorPoints.Add(new GRealPoint2D(refinedPoint.Argument, atr));
				if (atr > maxIndicatorValue)
					maxIndicatorValue = atr;
				if (atr < minIndicatorValue) 
					minIndicatorValue = atr;
				preAtr = atr;
			}
			Calculated = true;
			MaxY = maxIndicatorValue;
			MinY = minIndicatorValue;
			return indicatorPoints;
		}
	}
}
