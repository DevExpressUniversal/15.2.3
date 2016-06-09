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
	public enum AggregateFunctionNative {
		None,
		Average,
		Minimum,
		Maximum,
		Sum,
		Count,
		Financial
	}
	public struct RefinedPointsCluster {
		readonly IList<RefinedPoint> points;
		readonly IList<RefinedPoint> nonEmptyPoints;
		readonly double argument;
		public IList<RefinedPoint> Points { get { return points; } }
		public IList<RefinedPoint> NonEmptyPoints { get { return nonEmptyPoints; } }
		public double Argument { get { return argument; } }
		public bool IsEmptyPoints { get { return nonEmptyPoints.Count == 0 && points.Count > 0; } }
		public RefinedPointsCluster(double argument) {
			this.points = new List<RefinedPoint>();
			this.nonEmptyPoints = new List<RefinedPoint>();
			this.argument = argument;
		}
		public void AddPoint(RefinedPoint point) {
			points.Add(point);
			if (!point.IsEmpty) 
				nonEmptyPoints.Add(point);
		}
	}
	public class AggregationPointFilter : PointsFilter {
		readonly Dictionary<object, IList<RefinedPoint>> pointsCache = new Dictionary<object, IList<RefinedPoint>>();
		ActualScaleType scaleType;
		AggregateFunctionNative aggregateFunction;
		ProcessMissingPointsModeNative processMissingPoints;
		object measureUnit;
		public override bool NeedSortedByArgumentPoints { get { return true; } }
		public AggregationPointFilter(RefinedSeries refinedSeries) : base(refinedSeries) {
		}
		bool IsEnabled() {
			IAxisData axisData = GetAxisXData();
			if (axisData != null)
				switch (RefinedSeries.ArgumentScaleType) {
					case ActualScaleType.Numerical:
						return axisData.NumericScaleOptions.ScaleMode != ScaleModeNative.Continuous && !double.IsNaN(axisData.NumericScaleOptions.MeasureUnit);
					case ActualScaleType.DateTime:
						return axisData.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Continuous;				
				}
			return false;
		}
		IAxisData GetAxisXData() {
			IXYSeriesView view = RefinedSeries.SeriesView as IXYSeriesView;
			return view != null ? view.AxisXData : null;
		}
		IAxisData GetAxisYData() {
			IXYSeriesView view = RefinedSeries.SeriesView as IXYSeriesView;
			return view != null ? view.AxisYData : null;
		}
		ProcessMissingPointsModeNative GetProcessMissingPoints() {
			IAxisData axisData = GetAxisXData();
			if (axisData != null) {
				switch (RefinedSeries.ArgumentScaleType) {
					case ActualScaleType.Numerical:
						return axisData.NumericScaleOptions.ProcessMissingPoints;
					case ActualScaleType.DateTime:
						return axisData.DateTimeScaleOptions.ProcessMissingPoints;
				}
			}
			return ProcessMissingPointsModeNative.Skip;
		}
		object GetMeasureUnit() {
			IAxisData axisData = GetAxisXData();
			if (RefinedSeries.ArgumentScaleType == ActualScaleType.Numerical)
				return axisData.NumericScaleOptions.MeasureUnit;
			else if (RefinedSeries.ArgumentScaleType == ActualScaleType.DateTime)
				return axisData.DateTimeScaleOptions.MeasureUnit;
			return null;
		}
		AggregateFunctionNative GetAggregateFunction() {
			IAxisData axisData = GetAxisXData();
			if (axisData.AxisScaleTypeMap.ScaleType == ActualScaleType.Numerical)
				return axisData.NumericScaleOptions.AggregateFunction;
			return axisData.DateTimeScaleOptions.AggregateFunction;
		}
		Scale ConvertToScale(ActualScaleType scaleType) {
			switch(scaleType) {
				case ActualScaleType.Numerical:
					return Scale.Numerical;
				case ActualScaleType.DateTime:
					return Scale.DateTime;
				case ActualScaleType.Qualitative:
				default:
					return Scale.Qualitative;
			}
		}
		protected override void Recalculate(IList<RefinedPoint> initialPoints) {
			if(initialPoints != null) {
				if(initialPoints.Count == 0)
					pointsCache.Add(measureUnit, new List<RefinedPoint>());
				else {
					AxisScaleTypeMap axisXScaleMap = GetAxisXData().AxisScaleTypeMap;
					MissingArgumentsCalculator missingArgumentsCalculator = new MissingArgumentsCalculator(axisXScaleMap, RefinedSeries.Series);
					IList<RefinedPoint> processedPoints = missingArgumentsCalculator.FillMissingArguments(initialPoints, processMissingPoints);
					RefinedPointsAggregator aggregator = new RefinedPointsAggregator(axisXScaleMap);
					IList<RefinedPoint> points = GetAggregatedPoints(aggregator.ClusterPoints(processedPoints));
					pointsCache.Add(measureUnit, points);					
				}
			}
		}
		IList<RefinedPoint> GetAggregatedPoints(IList<RefinedPointsCluster> clusteredPoints) {
			List<RefinedPoint> refinedPoints = new List<RefinedPoint>();
			AggregationCalculator calculator = AggregationCalculator.Create(aggregateFunction);
			for (int i = 0; i < clusteredPoints.Count; i++) {
				if (calculator != null) 
					refinedPoints.Add(CreateAggregatedPoint(clusteredPoints[i], calculator));
				else {
					foreach (RefinedPoint point in clusteredPoints[i].Points) {
						if (point is RefinedPoint)
							refinedPoints.Add(new RefinedPoint((RefinedPoint)point, clusteredPoints[i].Argument));
					}
				}
			}
			return refinedPoints;
		}
		RefinedPoint CreateAggregatedPoint(RefinedPointsCluster cluster, AggregationCalculator calculator) {
			AxisScaleTypeMap axisXScaleTypeMap = GetAxisXData().AxisScaleTypeMap;
			AxisScaleTypeMap axisYScaleTypeMap = GetAxisYData().AxisScaleTypeMap;
			double[] values = cluster.NonEmptyPoints.Count > 0 ? calculator.Calculate(cluster.NonEmptyPoints) : new double[] {0,0,0,0};
			Scale scale = ConvertToScale(RefinedSeries.ArgumentScaleType);
			AggregatedSeriesPoint seriesPoint = null;
			object nativeArgument = axisXScaleTypeMap.InternalToNative(cluster.Argument);
			if(axisYScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
				DateTime[] dateTimeValues = new DateTime[values.Length];
				for(int i = 0; i < values.Length; i++) {
					if(!double.IsNaN(values[i]))
						dateTimeValues[i] = (DateTime)axisYScaleTypeMap.InternalToNative(values[i]);
				}
				seriesPoint = new AggregatedSeriesPoint(RefinedSeries, scale, nativeArgument, dateTimeValues, values, cluster.Points, cluster.IsEmptyPoints);
			} else
				seriesPoint = new AggregatedSeriesPoint(RefinedSeries, scale, nativeArgument, values, cluster.Points, cluster.IsEmptyPoints, false);
			return new RefinedPoint(seriesPoint, cluster.Argument, values) { IsEmpty = seriesPoint.IsEmpty };
		}
		protected override IList<RefinedPoint> GetCachedPoints() {
			object measureUnit = GetMeasureUnit();
			if (pointsCache.ContainsKey(measureUnit))
				return pointsCache[measureUnit];
			return null;
		}
		public override void ClearCache() {
			pointsCache.Clear();
			this.RefinedSeries.Series.ClearColorCache();
		}
		public override bool Update() {
			bool invalidate = false;   
			if (Enable != IsEnabled()) { 
				Enable = IsEnabled();			
				invalidate = true;
				ClearCache();
			} else if (Enable) {
				if (scaleType != RefinedSeries.ArgumentScaleType || aggregateFunction != GetAggregateFunction() || processMissingPoints != GetProcessMissingPoints()) {
					invalidate = true;
					ClearCache();					
				}
				if (!Object.Equals(measureUnit,GetMeasureUnit()))
					invalidate = true;				
			}
			if (Enable) {
				scaleType = RefinedSeries.ArgumentScaleType;
				aggregateFunction = GetAggregateFunction();
				measureUnit = GetMeasureUnit();
				processMissingPoints = GetProcessMissingPoints();
			}
			return invalidate;
		}
	}
	public class RefinedPointsAggregator {
		readonly AxisScaleTypeMap axisXScaleMap;
		public RefinedPointsAggregator(AxisScaleTypeMap axisXScaleMap) {
			this.axisXScaleMap = axisXScaleMap;
		}
		internal IList<RefinedPointsCluster> ClusterPoints(IList<RefinedPoint> points) {
			List<RefinedPointsCluster> clusteredPoints = new List<RefinedPointsCluster>();
			double lastAlignedArgument = double.NaN;
			double currentArgument = double.NaN;
			for(int i = 0; i < points.Count; i++) {
				currentArgument = axisXScaleMap.RefinedToInternal(points[i].Argument);
				if(currentArgument != lastAlignedArgument) {
					clusteredPoints.Add(new RefinedPointsCluster(currentArgument));
					lastAlignedArgument = currentArgument;
				}
				clusteredPoints[clusteredPoints.Count - 1].AddPoint(points[i]);
			}
			return clusteredPoints;
		}
	}
}
