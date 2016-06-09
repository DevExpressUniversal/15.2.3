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

using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class ExplodeFilter {
		public static ExplodeFilter CreateInstance(PieSeriesViewBase view) {
			switch (view.ExplodeMode) {
				case PieExplodeMode.MinValue:
					return new MinExplodeFilter(view);
				case PieExplodeMode.MaxValue:
					return new MaxExplodeFilter(view);
				case PieExplodeMode.All:
					return new AllExplodeFilter(view);
				case PieExplodeMode.UseFilters:
					return new UseFiltersExplodeFilter(view);
				case PieExplodeMode.UsePoints:
					return new UsePointsExplodeFilter(view);
				case PieExplodeMode.Others:
					return new OthersExplodeFilter(view);
				default:
					return null;
			}
		}
		PieSeriesViewBase view;
		protected PieSeriesViewBase View { get { return view; } }
		protected ExplodeFilter(PieSeriesViewBase view) {
			this.view = view;
		}
		public abstract List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points);
		public abstract bool CheckPoint(RefinedPoint point);
	}
	public class MinExplodeFilter : ExplodeFilter {
		public MinExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			double minValue = double.MaxValue;
			foreach (SeriesPoint point in points) {
				if (!point.IsEmpty) {
					double value = point.Values[0];
					if (value < minValue)
						minValue = value;
				}
			}
			List<ISeriesPoint> filteredPoints = new List<ISeriesPoint>();
			foreach (SeriesPoint point in points)
				if (!point.IsEmpty && point.Values[0] == minValue)
					filteredPoints.Add(point);
			return filteredPoints;
		}
		public override bool CheckPoint(RefinedPoint point) {
			return ((IPiePoint)point).IsMinPoint;
		}
	}
	public class MaxExplodeFilter : ExplodeFilter {
		public MaxExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			double maxValue = double.MinValue;
			foreach (SeriesPoint point in points) {
				if (!point.IsEmpty) {
					double value = point.Values[0];
					if (value > maxValue)
						maxValue = value;
				}
			}
			List<ISeriesPoint> filteredPoints = new List<ISeriesPoint>();
			foreach (SeriesPoint point in points) {
				if (!point.IsEmpty && point.Values[0] == maxValue)
					filteredPoints.Add(point);
			}
			return filteredPoints;
		}
		public override bool CheckPoint(RefinedPoint point) {
			return ((IPiePoint)point).IsMaxPoint;
		}
	}
	public class AllExplodeFilter : ExplodeFilter {
		public AllExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			return new List<ISeriesPoint>(points);
		}
		public override bool CheckPoint(RefinedPoint point) {
			return true;
		}
	}
	public class UseFiltersExplodeFilter : ExplodeFilter {
		public UseFiltersExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			List<ISeriesPoint> filteredPoints = new List<ISeriesPoint>();
			foreach (SeriesPoint point in points)
				if (View.ExplodedPointsFilters.CheckSeriesPoint(point))
					filteredPoints.Add(point);
			return filteredPoints;
		}
		public override bool CheckPoint(RefinedPoint point) {
			return View.ExplodedPointsFilters.CheckSeriesPoint(SeriesPoint.GetSeriesPoint(point.SeriesPoint));
		}
	}
	public class UsePointsExplodeFilter : ExplodeFilter {
		public UsePointsExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		bool ContainsInFilter(SeriesPoint point) {
			if (point.IsEmpty)
				return false;
			foreach (SeriesPoint pointInFilter in View.ExplodedPoints) {
				if (SeriesPoint.EqualsBySourcePoints(pointInFilter, point))
					return true;
			}
			return false;
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			List<ISeriesPoint> filteredPoints = new List<ISeriesPoint>();
			foreach (SeriesPoint point in points)
				if (ContainsInFilter(point))
					filteredPoints.Add(point);
			return filteredPoints;
		}
		public override bool CheckPoint(RefinedPoint point) {
			if (point.IsEmpty)
				return false;
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(point.SeriesPoint);
			return ContainsInFilter(seriesPoint);
		}
	}
	public class OthersExplodeFilter : ExplodeFilter {
		public OthersExplodeFilter(PieSeriesViewBase view)
			: base(view) {
		}
		public override List<ISeriesPoint> FilterPoints(IList<ISeriesPoint> points) {
			List<ISeriesPoint> filteredPoints = new List<ISeriesPoint>();
			foreach (SeriesPoint point in points)
				if (point.IsAuxiliary)
					filteredPoints.Add(point);
			return filteredPoints;
		}
		public override bool CheckPoint(RefinedPoint point) {
			return (SeriesPoint.GetSeriesPoint(point.SeriesPoint)).IsAuxiliary;
		}
	}
}
