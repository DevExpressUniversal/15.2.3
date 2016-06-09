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
	public class XYSeriesContainer : SingleSeriesContainer, ISupportMinMaxValues {
		List<IPointInteraction> interactions = new List<IPointInteraction>();
		double min, max, absMin;
		bool needRecalculate = true;
		bool needRecalculateAbsMin = true;
		public double Max {
			get {
				if (SeriesView != null && interactions.Count > 0) {
					if (needRecalculate)
						Calculate();
					return max;
				}
				return double.NaN;
			}
		}
		public double Min {
			get {
				if (SeriesView != null && interactions.Count > 0) {
					if (needRecalculate)
						Calculate();
					return min;
				}
				return double.NaN;
			}
		}
		public XYSeriesContainer(ISeriesView view)
			: base(view) {
		}
		void Calculate() {
			min = double.MaxValue;
			max = double.MinValue;
			foreach (RefinedPoint point in interactions) {
				double pointMin = ((IPointInteraction)point).GetMinValue(SeriesView);
				double pointMax = ((IPointInteraction)point).GetMaxValue(SeriesView);
				if (!double.IsInfinity(pointMin))
					min = Math.Min(pointMin, min);
				if (!double.IsInfinity(pointMax))
					max = Math.Max(pointMax, max);
			}
			needRecalculate = false;
		}
		void CalculateAbsMin() {
			absMin = double.MaxValue;
			foreach (RefinedPoint point in interactions) {
				double pointMin = Math.Abs(((IPointInteraction)point).GetMinValue(SeriesView));
				double pointMax = Math.Abs(((IPointInteraction)point).GetMaxValue(SeriesView));
				if (pointMin != 0) {
					absMin = Math.Min(pointMin, absMin);
					needRecalculateAbsMin = false;
				}
				if (pointMax != 0) {
					absMin = Math.Min(pointMax, absMin);
					needRecalculateAbsMin = false;
				}
			}
			if (needRecalculateAbsMin)
				absMin = double.NaN;
		}
		protected override void InsertRefinedPoints(int seriesIndex, RefinedSeries series) {
			foreach (RefinedPoint point in series.FinalPoints) {
				InsertRefinedPoint(series, point);
			}
		}
		protected override void RemoveRefinedPoints(int seriesIndex, RefinedSeries series) {
			needRecalculate = true;
			needRecalculateAbsMin = true;
			interactions.Clear();
		}
		protected override void InsertRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			if (point.IsEmpty)
				return;
			if (needRecalculate)
				Calculate();
			double pointMin = ((IPointInteraction)point).GetMinValue(SeriesView);
			double pointMax = ((IPointInteraction)point).GetMaxValue(SeriesView);
			if (!double.IsInfinity(pointMin))
				min = Math.Min(pointMin, min);
			if (!double.IsInfinity(pointMax))
				max = Math.Max(pointMax, max);
			needRecalculateAbsMin = true;
			interactions.Add(point);
		}
		protected override void RemoveRefinedPoint(RefinedSeries refinedSeries, RefinedPoint point) {
			if (point.IsEmpty)
				return;
			double pointMin = ((IPointInteraction)point).GetMinValue(SeriesView);
			double pointMax = ((IPointInteraction)point).GetMaxValue(SeriesView);
			if (!needRecalculate) {
				if (!(pointMin > min))
					needRecalculate = true;
				else if (!(pointMax < max))
					needRecalculate = true;
			}
			needRecalculateAbsMin = true;
			interactions.Remove(point);
		}
		public double GetAbsMinValue() {
			if (SeriesView != null && interactions.Count > 0) {
				if (needRecalculateAbsMin)
					CalculateAbsMin();
				return absMin;
			}
			return double.NaN;
		}
	}
}
