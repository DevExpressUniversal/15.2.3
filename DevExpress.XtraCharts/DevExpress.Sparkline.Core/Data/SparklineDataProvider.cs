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
namespace DevExpress.Sparkline.Core {
	public class SparklineDataProvider {
		readonly ISparklineExtendedData data;
		readonly List<SparklinePoint> sortedPoints;
		readonly SparklineRangeData filterRange;
		bool allowPaddingCorrection;
		SparklineIndexRange filteredPointRange;
		SparklineIndexRange dataValuePointRange;
		SparklineRangeData dataValueRange;
		SparklineRangeData dataArgumentRange;
		public ISparklineExtendedData Data {
			get { return data; }
		}
		public List<SparklinePoint> SortedPoints {
			get { return sortedPoints; }
		}
		public SparklineIndexRange FilteredPointRange {
			get { return filteredPointRange; }
		}
		public SparklineIndexRange ValuePointRange {
			get { return dataValuePointRange; }
		}
		public SparklineRangeData DataValueRange {
			get { return dataValueRange; }
			set { dataValueRange = value; }
		}
		public SparklineRangeData DataArgumentRange {
			get { return dataArgumentRange; }
		}
		public SparklineRangeData FilterRange {
			get { return filterRange; }
		}
		public bool AllowPaddingCorrection {
			get { return allowPaddingCorrection; }
			set { allowPaddingCorrection = value; }
		}
		public SparklineDataProvider(ISparklineExtendedData data) : this(data, new SparklineRangeData(0, 1)) { }
		public SparklineDataProvider(ISparklineExtendedData data, SparklineRangeData filterRange) {
			this.data = data;
			this.filterRange = filterRange;
			this.allowPaddingCorrection = true;
			sortedPoints = FilterValidPoints(data.Points);
			SparklinePointArgumentComparer comparer = new SparklinePointArgumentComparer();
			sortedPoints.Sort(comparer);
			FilterPointsWithRange();
			FindValueRange();
		}
		List<SparklinePoint> FilterValidPoints(IList<SparklinePoint> points) {
			List<SparklinePoint> result;
			if (points != null) {
				result = new List<SparklinePoint>(points.Count);
				for (int i = 0; i < points.Count; i++) {
					SparklinePoint point = points[i];
					if (SparklineMathUtils.IsValidDouble(point.Argument))
						result.Add(point);
				}
			} else
				result = new List<SparklinePoint>();
			return result;
		}
		void FindValueRange() {
			double minY = double.MaxValue;
			double maxY = double.MinValue;
			dataValuePointRange = new SparklineIndexRange();
			for (int i = 0; i < sortedPoints.Count; i++) {
				double value = sortedPoints[i].Value;
				if (SparklineMathUtils.IsValidDouble(value)) {
					if (value < minY) {
						minY = value;
						dataValuePointRange.Min = i;
					}
					if (value > maxY) {
						maxY = value;
						dataValuePointRange.Max = i;
					}
				}
			}
			dataValueRange = new SparklineRangeData();
			if (dataValuePointRange.IsValid)
				dataValueRange.Set(sortedPoints[dataValuePointRange.Min].Value, sortedPoints[dataValuePointRange.Max].Value);
		}
		void FilterPointsWithRange() {
			if (sortedPoints.Count == 0) {
				filteredPointRange = new SparklineIndexRange(-1, -1);
				dataArgumentRange = new SparklineRangeData();
				return;
			} else if (sortedPoints.Count == 1) {
				filteredPointRange = new SparklineIndexRange(0, 0);
				dataArgumentRange = new SparklineRangeData(sortedPoints[0].Argument, sortedPoints[0].Argument);
				return;
			} else if (sortedPoints.Count == 2) {
				filteredPointRange = new SparklineIndexRange(0, 1);
				dataArgumentRange = new SparklineRangeData(sortedPoints[0].Argument, sortedPoints[1].Argument);
				return;
			}
			dataArgumentRange = new SparklineRangeData(sortedPoints[0].Argument, sortedPoints[sortedPoints.Count - 1].Argument);
			double dataMin = sortedPoints[0].Argument;
			double dataDelta = sortedPoints[sortedPoints.Count - 1].Argument - sortedPoints[0].Argument;
			double rangeMin = dataMin + dataDelta * filterRange.Min;
			double rangeMax = dataMin + dataDelta * filterRange.Max;
			SparklinePointArgumentComparer argumentComparer = new SparklinePointArgumentComparer();
			filteredPointRange = new SparklineIndexRange();
			int minPointIndex = sortedPoints.BinarySearch(new SparklinePoint(rangeMin, double.NaN), argumentComparer);
			filteredPointRange.Min = minPointIndex < 0 ? ~minPointIndex : minPointIndex;
			int maxPointIndex = sortedPoints.BinarySearch(new SparklinePoint(rangeMax, double.NaN), argumentComparer);
			filteredPointRange.Max = maxPointIndex < 0 ? ~maxPointIndex - 1 : maxPointIndex;
		}
	}
}
