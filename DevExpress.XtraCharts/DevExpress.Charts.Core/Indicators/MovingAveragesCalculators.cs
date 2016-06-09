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
	public enum MovingAverageKindInternal {
		MovingAverage,
		Envelope,
		MovingAverageAndEnvelope
	}
	public abstract class MovingAverageCalculator {
		bool movingAverageDataCalculated = false;
		List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
		List<GRealPoint2D> upperEnvelopeData  = new List<GRealPoint2D>();
		List<GRealPoint2D> lowerEnvelopeData = new List<GRealPoint2D>();
		bool envelopeDataCalculated = false;
		public bool MovingAverageDataCalculated { get { return movingAverageDataCalculated; } protected set { movingAverageDataCalculated = value; } }
		public bool EnvelopeDataCalculated { get { return envelopeDataCalculated; } }
		public List<GRealPoint2D> MovingAverageData { get { return movingAverageData; } }
		public List<GRealPoint2D> UpperEnvelopeData  { get { return upperEnvelopeData; } }
		public List<GRealPoint2D> LowerEnvelopeData  { get { return lowerEnvelopeData; } }
		public abstract List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel);
		public void CalculateEnvelopeData(List<GRealPoint2D> movingAverageData, double envelopePercent) {
			if (movingAverageData == null)
				return;
			upperEnvelopeData.Clear();
			lowerEnvelopeData.Clear();
			double envelopeFactor = envelopePercent / 100;
			foreach (GRealPoint2D point in movingAverageData) {
				GRealPoint2D upperPoint = new GRealPoint2D(point.X, point.Y + point.Y * envelopeFactor);
				upperEnvelopeData.Add(upperPoint);
				GRealPoint2D lowerPoint = new GRealPoint2D(point.X, point.Y - point.Y * envelopeFactor);
				lowerEnvelopeData.Add(lowerPoint);
			}
		}
		public void Calculate(IRefinedSeries refinedSeries, int pointsCount, ValueLevelInternal valueLevel, MovingAverageKindInternal kind, double envelopePercent) {
			movingAverageData = CalculateMovingAverageData(refinedSeries.Points, pointsCount, valueLevel);
			CalculateEnvelopeData(movingAverageData, envelopePercent);
		}
	}
	public class SimpleMovingAverageCalculator : MovingAverageCalculator{
		public override List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel) {
			MovingAverageDataCalculated = false;
			List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
			if (pointsCount <= 0 || refinedPoints == null)
				return movingAverageData;
			int valuesCount = refinedPoints.Count;
			if (valuesCount > 1) {
				int usingPointsCount = Math.Min(pointsCount, valuesCount);
				double sma = 0.0;
				Queue<double> currentValues = new Queue<double>(usingPointsCount);
				for (int index = 0, divider = 1; index < usingPointsCount; index++, divider++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value =refinedPoint.GetValue(valueLevel);
					sma += value;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, sma / divider));
					currentValues.Enqueue(value);
				}
				sma /= usingPointsCount;
				for (int index = usingPointsCount; index < valuesCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					sma += (value - currentValues.Dequeue()) / usingPointsCount;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, sma));
					currentValues.Enqueue(value);
				}
				MovingAverageDataCalculated = true;
			}
			return movingAverageData;
		} 
	}
	public class WeightedMovingAverageCalculator : MovingAverageCalculator {
		public override List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel) {
			MovingAverageDataCalculated = false;
			List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
			if (pointsCount <= 0 || refinedPoints == null)
				return movingAverageData;
			int valuesCount = refinedPoints.Count;
			if (valuesCount > 1) {
				int usingPointsCount = Math.Min(pointsCount, valuesCount);
				double total = 0.0;
				double sum = 0.0;
				int weight = 0;
				Queue<double> currentValues = new Queue<double>(usingPointsCount);
				for (int index = 0; index < usingPointsCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					currentValues.Enqueue(value);
					total += value;
					sum = 0.0;
					weight = 0;
					int factor = 1;
					foreach (double val in currentValues) {
						sum += val * factor;
						weight += factor;
						factor++;
					}
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, sum / weight));
				}
				for (int index = usingPointsCount; index < valuesCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value =refinedPoint.GetValue(valueLevel);
					sum += value * usingPointsCount - total;
					total += value - currentValues.Dequeue();
					currentValues.Enqueue(value);
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, sum / weight));
				}
				MovingAverageDataCalculated = true;
			}
			return movingAverageData;
		}
	}
	public class ExponentialMovingAverageCalculator : MovingAverageCalculator {
		public override List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel) {
			MovingAverageDataCalculated = false;
			List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
			if (pointsCount <= 0 || refinedPoints == null)
				return movingAverageData;
			int valuesCount = refinedPoints.Count;
			if (valuesCount > 1) {
				int usingPointsCount = Math.Min(pointsCount, valuesCount);
				double ema = 0.0;
				double multiplier = 1;
				for (int index = 0, divider = 2; index < usingPointsCount; index++, divider++) {
					multiplier = 2.0 / divider;
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					ema += (value - ema) * multiplier;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, ema));
				}
				for (int index = usingPointsCount; index < valuesCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					ema += (value - ema) * multiplier;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, ema));
				}
				MovingAverageDataCalculated = true;
			}
			return movingAverageData;
		}
	}
	public class TriangularMovingAverageCalculator : MovingAverageCalculator {
		public override List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel) {
			MovingAverageDataCalculated = false;
			List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
			if (pointsCount <= 0 || refinedPoints == null)
				return movingAverageData;
			int valuesCount = refinedPoints.Count;
			if (valuesCount > 1) {
				int usingPointsCount = Math.Min(pointsCount, valuesCount);
				double sma = 0.0;
				double tma = 0.0;
				Queue<double> currentValues = new Queue<double>(usingPointsCount);
				Queue<double> currentSMA = new Queue<double>(usingPointsCount);
				for (int index = 0, divider = 1; index < usingPointsCount; index++, divider++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					sma += value;
					double actualSMA = sma / divider;
					tma += actualSMA;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, tma / divider));
					currentValues.Enqueue(value);
					currentSMA.Enqueue(actualSMA);
				}
				sma /= usingPointsCount;
				tma /= usingPointsCount;
				for (int index = usingPointsCount; index < valuesCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					sma += (value - currentValues.Dequeue()) / usingPointsCount;
					tma += (sma - currentSMA.Dequeue()) / usingPointsCount;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, tma));
					currentValues.Enqueue(value);
					currentSMA.Enqueue(sma);
				}
				MovingAverageDataCalculated = true;
			}
			return movingAverageData;
		}
	}
	public class TripleExponentialMovingAverageCalculator : MovingAverageCalculator {
		public override List<GRealPoint2D> CalculateMovingAverageData(IList<RefinedPoint> refinedPoints, int pointsCount, ValueLevelInternal valueLevel) {
			MovingAverageDataCalculated = false;
			List<GRealPoint2D> movingAverageData = new List<GRealPoint2D>();
			if (pointsCount <= 0 || refinedPoints == null)
				return movingAverageData;
			int valuesCount = refinedPoints.Count;
			if (valuesCount > 1) {
				int usingPointsCount = Math.Min(pointsCount, valuesCount);
				double ema = 0.0;
				double ema2 = 0.0;
				double ema3 = 0.0;
				double smoothingFactor = 1;
				for (int index = 0, divider = 2; index < usingPointsCount; index++, divider++) {
					smoothingFactor = 2.0 / divider;
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					ema += (value - ema) * smoothingFactor;
					ema2 = smoothingFactor * ema  + (1 - smoothingFactor) * ema2;
					ema3 = smoothingFactor * ema2 + (1 - smoothingFactor) * ema3;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, ema3));
				}
				for (int index = usingPointsCount; index < valuesCount; index++) {
					RefinedPoint refinedPoint = refinedPoints[index];
					double value = refinedPoint.GetValue(valueLevel);
					ema += (value - ema) * smoothingFactor;
					ema2 = smoothingFactor * ema + (1 - smoothingFactor) * ema2;
					ema3 = smoothingFactor * ema2 + (1 - smoothingFactor) * ema3;
					movingAverageData.Add(new GRealPoint2D(refinedPoint.Argument, ema3));
				}
				MovingAverageDataCalculated = true;
			}
			return movingAverageData;
		}
	}
}
