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
	public abstract class AggregationCalculator {
		const int ValuesCount = 4;
		protected static double[] CreateArray() {
			return CreateArray(0);
		}
		protected static double[] CreateArray(double defaultValue) {
			double[] values = new double[ValuesCount];
			for (int i = 0; i < ValuesCount; i++)
				values[i] = defaultValue;
			return values;
		}
		public static AggregationCalculator Create(AggregateFunctionNative aggregationFunction) {
			switch (aggregationFunction) {
				case AggregateFunctionNative.Minimum:
					return new MinAggregationCalculator();
				case AggregateFunctionNative.Maximum:
					return new MaxAggregationCalculator();
				case AggregateFunctionNative.Sum:
					return new SumAggregationCalculator();
				case AggregateFunctionNative.Count:
					return new CountAggregationCalculator();
				case AggregateFunctionNative.Average:
					return new AverageAggregationCalculator();
				case AggregateFunctionNative.Financial:
					return new FinancialAggregationCalculator();
				case AggregateFunctionNative.None:
				default:
					return null;
			}
		}
		public abstract double[] Calculate(IList<RefinedPoint> points);
	}
	public class MinAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			double[] values = CreateArray(double.MaxValue);
			foreach (RefinedPoint point in points) {
				values[0] = Math.Min(values[0], point.Value1);
				values[1] = Math.Min(values[1], point.Value2);
				values[2] = Math.Min(values[2], point.Value3);
				values[3] = Math.Min(values[3], point.Value4);
			}
			return values;
		}
	}
	public class MaxAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			double[] values = CreateArray(double.MinValue);
			foreach (RefinedPoint point in points) {
				values[0] = Math.Max(values[0], point.Value1);
				values[1] = Math.Max(values[1], point.Value2);
				values[2] = Math.Max(values[2], point.Value3);
				values[3] = Math.Max(values[3], point.Value4);
			}
			return values;
		}
	}
	public class SumAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			double[] values = CreateArray();
			foreach (RefinedPoint point in points) {
				values[0] += point.Value1;
				values[1] += point.Value2;
				values[2] += point.Value3;
				values[3] += point.Value4;
			}
			return values;
		}
	}
	public class AverageAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			double[] values = CreateArray();
			foreach (RefinedPoint point in points) {
				values[0] += point.Value1 / points.Count;
				values[1] += point.Value2 / points.Count;
				values[2] += point.Value3 / points.Count;
				values[3] += point.Value4 / points.Count;
			}
			return values;
		}
	}
	public class CountAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			double[] values = CreateArray();
			values[0] = points.Count;
			values[1] = points.Count;
			values[2] = points.Count;
			values[3] = points.Count;
			return values;
		}
	}
	public class FinancialAggregationCalculator : AggregationCalculator {
		public override double[] Calculate(IList<RefinedPoint> points) {
			RefinedPoint bufferPoint = new RefinedPoint();
			IFinancialPoint financialPoint = (IFinancialPoint)bufferPoint;
			if(points.Count > 0 ) {
				financialPoint.Open = ((IFinancialPoint)points[0]).Open;
				financialPoint.Close = ((IFinancialPoint)points[points.Count - 1]).Close;
				double min = double.MaxValue;
				double max = double.MinValue;
				foreach (IFinancialPoint point in points) {
					min = Math.Min(min, point.Low);
					max = Math.Max(max, point.High);
				}
				financialPoint.High = max;
				financialPoint.Low = min;
			}
			return new double[] { bufferPoint.Value1, bufferPoint.Value2, bufferPoint.Value3, bufferPoint.Value4 };
		}
	}
}
