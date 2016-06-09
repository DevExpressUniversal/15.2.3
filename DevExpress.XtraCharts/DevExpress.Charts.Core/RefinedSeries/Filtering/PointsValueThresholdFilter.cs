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
	public abstract class ValueThresholdFilterBehavior {
		readonly IList<RefinedPoint> othersPoints = new List<RefinedPoint>();
		IList<RefinedPoint> filteredPoints;
		public IList<RefinedPoint> FilteredPoints { get { return filteredPoints; } }
		public IList<RefinedPoint> OthersPoints { get { return othersPoints; } }
		public void Calculate(IList<RefinedPoint> initialPoints, double valueThreshold) {
			othersPoints.Clear();
			filteredPoints = FilterPoints(initialPoints, valueThreshold);
		}
		protected abstract IList<RefinedPoint> FilterPoints(IList<RefinedPoint> initialPoints, double valueThreshold);
		protected double GetValue(RefinedPoint point) {
			return Math.Abs(point.SeriesPoint.UserValues[0]);
		}
		protected void AddToOthersPoints(RefinedPoint point) {
			othersPoints.Add(point);
		}
	}
	public class PointsValueThresholdFilter : PointsFilter {
		IList<RefinedPoint> filteredPoints;
		AggregatedSeriesPoint othersSeriesPoint;
		RefinedPoint othersPoint;		
		double ThresholdValue { get; set; }
		bool ShowOthers { get; set;}
		string OthersArgument { get; set; }
		PointsFilterType FilterType { get; set; }
		public override bool NeedSortedByArgumentPoints { get { return false; } }
		public PointsValueThresholdFilter(RefinedSeries series) : base(series) {
		}
		ValueThresholdFilterBehavior CreateBehavior() {
			switch (FilterType) {
				case PointsFilterType.TopN:
					return new TopNFilterBehavior();
				case PointsFilterType.MoreOrEqualValue:
					return new MaxValueFilterBehavior();
				case PointsFilterType.MoreOrEqualPercentValue:
					return new PercentMaxValueFilterBehavior();
				default:
					return new TopNFilterBehavior();
			}
		}
		IPointsFilterOptions GetOptions() {
			return RefinedSeries.Series as IPointsFilterOptions;
		}
		void UpdateOthersPoint() {
			if (ShowOthers && othersPoint != null) {
				filteredPoints.Remove(othersPoint);
			}
			if (othersSeriesPoint != null) {
				othersSeriesPoint.Argument = OthersArgument;
				othersPoint = new RefinedPoint(othersSeriesPoint, filteredPoints.Count, othersSeriesPoint.Values[0]);
			} else
				othersPoint = null;
			if (ShowOthers && othersPoint != null) {
				filteredPoints.Add(othersPoint);
			}
		}
		IList<RefinedPoint> FilterPoints(IList<RefinedPoint> initialPoints) {
			othersPoint = null;
			ValueThresholdFilterBehavior behavior = CreateBehavior();
			behavior.Calculate(initialPoints, ThresholdValue);
			if (behavior.OthersPoints.Count > 0) {
				double[] values = CalculateSum(behavior.OthersPoints);
				if (values.Length > 0)
					othersSeriesPoint = new AggregatedSeriesPoint(RefinedSeries, Scale.Qualitative, OthersArgument, values, behavior.OthersPoints, false, true);
			}
			return behavior.FilteredPoints;
		}
		private double[] CalculateSum(IList<RefinedPoint> list) {
			double[] res = new double[] { 0,0,0,0 };
			foreach (RefinedPoint point in list) {
				for (int i = 0; i < 4; i++) {
					if (i < point.SeriesPoint.UserValues.Length)
						res[i] += point.SeriesPoint.UserValues[i];
					else
						res[i] += double.NaN;
				}
			}
			return res;
		}		
		internal bool IsEnabled() {
			IPointsFilterOptions options = GetOptions();
			return options != null && options.Enable;
		}
		protected override void Recalculate(IList<RefinedPoint> initialPoints) {
			if (initialPoints != null) {
				filteredPoints = FilterPoints(initialPoints);
				PointsRecalculated();
			}
		}
		protected override IList<RefinedPoint> GetCachedPoints() {
			return filteredPoints;
		}
		void PointsRecalculated() {
			UpdateOthersPoint();
		}
		public override void ClearCache() {
			filteredPoints = null;
			othersPoint = null;
			othersSeriesPoint = null;
		}
		public override bool Update() {
			bool invalid = false;
			if (Enable != IsEnabled()) {
				Enable = IsEnabled();
				ClearCache();
				invalid = true;
			} else if (Enable) {
				IPointsFilterOptions options = GetOptions();
				if (options.ShowOthers != ShowOthers) {
					ShowOthers = options.ShowOthers;
					if (filteredPoints != null && othersPoint != null) {
						if (ShowOthers)
							filteredPoints.Add(othersPoint);
						else
							filteredPoints.Remove(othersPoint);
					}
					invalid = true;
				}
				if (options.OthersArgument != OthersArgument) {
					this.OthersArgument = options.OthersArgument;
					UpdateOthersPoint();
					invalid = true;
				}
				if (options.ThresholdValue != ThresholdValue) {
					this.ThresholdValue = options.ThresholdValue;
					ClearCache();
					invalid = true;
				}
				if (options.FilterType != FilterType) {
					FilterType = options.FilterType;
					ClearCache();
					invalid = true;
				}
			}			
			return invalid;
		}
		public void ProcessOthersPoint(IPointProcessor processor, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
			if (processor != null && othersPoint != null) {				
				processor.ProcessPoint(othersPoint, argumentMap, valueMap);
			}
		}
	}
}
