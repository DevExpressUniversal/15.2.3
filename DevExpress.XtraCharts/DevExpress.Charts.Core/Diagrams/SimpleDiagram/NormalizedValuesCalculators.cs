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
	public abstract class NormalizedValuesCalculator {
		protected static double GetPointValue(ISeriesPoint point) {
			return point.IsEmpty(Scale.Numerical) ? 0 : point.UserValues[0];
		}
		readonly ISeries series;
		double[] normValues;
		protected ISeries Series { get { return series; } }
		protected IList<ISeriesPoint> Points { get { return series.ActualPoints; } }
		public NormalizedValuesCalculator(ISeries series) {
			this.series = series;
			if (Points.Count > 0)
				normValues = CalculateNormValues();
		}
		public double CalculateNormalizedValue(ISeriesPoint point) {
			int pointIndex = Points.IndexOf(point);
			if (pointIndex != -1)
				return normValues[pointIndex];
			else {
				ChartDebug.Fail("The point doesn't exist in the Normalize calculator.");
				return 0;
			}
		}
		protected abstract double[] CalculateNormValues();
	}
	public class PieNormalizedValuesCalculator : NormalizedValuesCalculator {
		public PieNormalizedValuesCalculator(ISeries series) : base(series) { 
		}
		protected override double[] CalculateNormValues() {
			double[] normValues = new double[Points.Count];
			int negativeValuesCount = 0;
			int positiveValuesCount = 0;
			double[] positiveValues = new double[Points.Count];
			double[] negativeValues = new double[Points.Count];
			for (int i = 0; i < Points.Count; i++) {
				double currentValue = GetPointValue(Points[i]);
				if (currentValue < 0) {
					negativeValuesCount++;
					positiveValues[i] = 0;
					negativeValues[i] = currentValue;
				}
				else {
					positiveValues[i] = currentValue;
					negativeValues[i] = 0;
					if (currentValue > 0)
						positiveValuesCount++;
				}
			}
			if (positiveValuesCount > 0)
				normValues = positiveValues;
			else if (negativeValuesCount > 0)
				normValues = negativeValues;
			else {
				normValues = positiveValues;
				for (int i = 0; i < normValues.Length; i++)
					normValues[i] = 1;
			}
			double normSum = 0;
			foreach (double val in normValues)
				normSum += val;
			for (int i = 0; i < normValues.Length; i++)
				normValues[i] /= normSum;
			return normValues;
		}
	}
	public class FunnelNormalizedValuesCalculator : NormalizedValuesCalculator {
		public FunnelNormalizedValuesCalculator(ISeries series) : base(series) {
		}
		protected override double[] CalculateNormValues() {
			Scale valueScaleType = Series.ValueScaleType;
			bool havePositiveOrZeroValue = false;
			foreach (ISeriesPoint point in Points)
				if (!point.IsEmpty(valueScaleType) && point.UserValues[0] >= 0) {
					havePositiveOrZeroValue = true;
					break;
				}
			double[] normValues = new double[Points.Count];
			double extremum = 0;
			if (havePositiveOrZeroValue) 
				for (int i = 0; i < Points.Count; i++) {
					double currentValue = GetPointValue(Points[i]);
					if (currentValue >= 0) {
						normValues[i] = currentValue;
						extremum = Math.Max(extremum, normValues[i]);
					}
					else
						normValues[i] = Double.NaN;
				}
			else
				for (int i = 0; i < Points.Count; i++) {
					double currentValue = GetPointValue(Points[i]);
					normValues[i] = currentValue;
					extremum = Math.Min(extremum, normValues[i]);
				}
			if (extremum != 0) {
				for (int i = 0; i < normValues.Length; i++)
					if (!Double.IsNaN(normValues[i]))
						normValues[i] /= extremum;
			}
			else {
				for (int i = 0; i < normValues.Length; i++)
					if (!double.IsNaN(normValues[i]))
						normValues[i] = 1;
			}
			return normValues;
		}
	}
}
