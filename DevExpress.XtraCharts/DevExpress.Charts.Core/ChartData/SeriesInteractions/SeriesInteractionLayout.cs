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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class PointInteractionCollection : IEnumerable<IPointInteraction> {
		readonly Comparer<IPointInteraction> comparer;
		readonly List<IPointInteraction> innerList;
		public IPointInteraction this[int index] {
			get { return innerList[index]; }
			set { innerList[index] = value; }
		}
		public int Count { get { return innerList.Count; } }
		public PointInteractionCollection(Comparer<IPointInteraction> comparer) {
			this.innerList = new List<IPointInteraction>();
			this.comparer = comparer;
		}
		#region IEnumerator<PointInteraction>
		IEnumerator<IPointInteraction> IEnumerable<IPointInteraction>.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		public int BinarySearch(IPointInteraction item) {
			return innerList.BinarySearch(item, comparer);
		}
		public int IndexOf(IPointInteraction item) {
			int index = BinarySearch(item);
			if (index < 0)
				return -1;
			for (int i = index; (i < Count) && (comparer.Compare(innerList[index], innerList[i]) == 0); i++) {
				if (Object.ReferenceEquals(item, innerList[i]))
					return i;
			}
			for (int i = index - 1; (i >= 0) && (comparer.Compare(innerList[index], innerList[i]) == 0); i--) {
				if (Object.ReferenceEquals(item, innerList[i]))
					return i;
			}
			return -1;
		}
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
		}
		public void Add(IPointInteraction item) {
			int index = BinarySearch(item);
			if (index < 0)
				index = ~index;
			innerList.Insert(index, item);
		}
		public void Insert(int index, IPointInteraction interaction) {
			innerList.Insert(index, interaction);
		}
		public void Clear() {
			innerList.Clear();
		}
		public void Sort() {
			innerList.Sort(comparer);
		}
		public bool Contains(IPointInteraction item) {
			return IndexOf(item) >= 0;
		}
		public bool Remove(IPointInteraction item) {
			int index = IndexOf(item);
			if (index >= 0)
				RemoveAt(index);
			return index >= 0;
		}
	}
	public abstract class BasePointInteraction : IPointInteraction {
		public abstract int Count { get; }
		public virtual bool IsEmpty { get { return Count == 0; } }
		public virtual double Argument { get { return double.NaN; } }
		#region IPointInteraction
		int IPointInteraction.Count { get { return Count; } }
		bool IPointInteraction.IsEmpty { get { return IsEmpty; } }
		double IPointInteraction.Argument { get { return Argument; } }
		double IPointInteraction.GetMinValue(ISeriesView seriesView) {
			return GetMinValue(seriesView);
		}
		double IPointInteraction.GetMaxValue(ISeriesView seriesView) {
			return GetMaxValue(seriesView);
		}
		double IPointInteraction.GetMinAbsValue(ISeriesView seriesView) {
			return GetMinAbsValue(seriesView);
		}
		#endregion
		protected int InsertPointToList(List<RefinedPoint> list, Comparer<RefinedPoint> comparer, RefinedPoint point) {
			int index = list.BinarySearch(point, comparer);
			if (index < 0)
				index = ~index;
			list.Insert(index, point);
			return index;
		}
		protected int RemovePointFromList(List<RefinedPoint> list, Comparer<RefinedPoint> comparer, RefinedPoint point) {
			int index = list.BinarySearch(point, comparer);
			if (index >= 0)
				list.RemoveAt(index);
			return index;
		}
		protected void UpdatePointsInteractions(List<RefinedPoint> list) {
			UpdatePointsInteractions(list, 0);
		}
		protected void UpdatePointsInteractions(List<RefinedPoint> list, int startIndex) {
			for (int i = startIndex; i < list.Count; i++) {
				if (list[i] != null)
					list[i].SetInteraction(this, i);
			}
		}
		public abstract double GetMinValue(ISeriesView seriesView);
		public abstract double GetMaxValue(ISeriesView seriesView);
		public abstract double GetMinAbsValue(ISeriesView seriesView);
	}
	public class SimplePointInteraction : BasePointInteraction {
		readonly RefinedPointsValueComparer comparer = new RefinedPointsValueComparer();
		readonly List<RefinedPoint> points = new List<RefinedPoint>();
		double positiveValuesSum = double.NaN;
		double negativeValuesSum = double.NaN;
		public double MinValue { get { return points.Count > 0 ? ((IValuePoint)points[0]).Value : double.NaN; } }
		public double MaxValue { get { return points.Count > 0 ? ((IValuePoint)points[points.Count - 1]).Value : double.NaN; } }
		public double PositiveValuesSum { get { return positiveValuesSum; } }
		public double NegativeValuesSum { get { return negativeValuesSum; } }
		public override int Count { get { return points.Count; } }
		public int GetNotEmptyPointCount() {
			int count = 0;
			foreach (var point in points)
				if (!point.IsEmpty)
					count++;
			return count;
		}
		public SimplePointInteraction() {
		}
		public void AddPoint(RefinedPoint point) {
			int index = InsertPointToList(points, comparer, point);
			UpdatePointsInteractions(points, index);
			double value = ((IValuePoint)point).Value;
			if (GeometricUtils.IsValidDouble(value)) {
				if (value < 0) {
					if (double.IsNaN(negativeValuesSum))
						negativeValuesSum = 0;
					negativeValuesSum += value;
				}
				else {
					if (double.IsNaN(positiveValuesSum))
						positiveValuesSum = 0;
					positiveValuesSum += value;
				}
			}
		}
		public void RemovePoint(RefinedPoint point) {
			point.ResetInteraction();
			int index = RemovePointFromList(points, comparer, point);
			if (index >= 0) {
				UpdatePointsInteractions(points, index);
				double value = ((IValuePoint)point).Value;
				if (GeometricUtils.IsValidDouble(value)) {
					if (value < 0)
						negativeValuesSum -= value;
					else
						positiveValuesSum -= value;
				}
			}
		}
		public void Clear() {
			foreach (RefinedPoint point in points)
				point.ResetInteraction();
			points.Clear();
			positiveValuesSum = double.NaN;
			negativeValuesSum = double.NaN;
		}
		public override double GetMinValue(ISeriesView seriesView) {
			return double.NaN;
		}
		public override double GetMaxValue(ISeriesView seriesView) {
			return double.NaN;
		}
		public override double GetMinAbsValue(ISeriesView seriesView) {
			return double.NaN;
		}
	}
	public class RangePointInteraction : BasePointInteraction {
		readonly RefinedPointsRangeValue1Comparer value1Comparer = new RefinedPointsRangeValue1Comparer();
		readonly RefinedPointsRangeValue2Comparer value2Comparer = new RefinedPointsRangeValue2Comparer();
		readonly List<RefinedPoint> pointsByValue1 = new List<RefinedPoint>();
		readonly List<RefinedPoint> pointsByValue2 = new List<RefinedPoint>();
		public override int Count { get { return pointsByValue1.Count; } }
		public RangePointInteraction() {
		}
		public void AddPoint(RefinedPoint point) {
			InsertPointToList(pointsByValue1, value1Comparer, point);
			InsertPointToList(pointsByValue2, value2Comparer, point);
			UpdatePointsInteractions(pointsByValue1);
		}
		public void RemovePoint(RefinedPoint point) {
			point.ResetInteraction();
			RemovePointFromList(pointsByValue1, value1Comparer, point);
			RemovePointFromList(pointsByValue2, value2Comparer, point);
			UpdatePointsInteractions(pointsByValue1);
		}
		public void Clear() {
			foreach (RefinedPoint point in pointsByValue1)
				point.ResetInteraction();
			pointsByValue1.Clear();
			pointsByValue2.Clear();
		}
		public override double GetMinValue(ISeriesView seriesView) {
			double minValue = double.NaN;
			if (pointsByValue1.Count > 0)
				minValue = Math.Min(((IRangePoint)pointsByValue1[0]).Value1, ((IRangePoint)pointsByValue2[0]).Value2);
			return minValue;
		}
		public override double GetMaxValue(ISeriesView seriesView) {
			double maxValue = double.NaN;
			if (pointsByValue1.Count > 0) {
				int index = pointsByValue1.Count - 1;
				maxValue = Math.Max(((IRangePoint)pointsByValue1[index]).Value1, ((IRangePoint)pointsByValue2[index]).Value2);
			}
			return maxValue;
		}
		public override double GetMinAbsValue(ISeriesView seriesView) {
			double minAbsValue = double.NaN;
			if (pointsByValue1.Count > 0)
				minAbsValue = Math.Min(Math.Abs(((IRangePoint)pointsByValue1[0]).Value1), Math.Abs(((IRangePoint)pointsByValue2[0]).Value2));
			return minAbsValue;
		}
	}
	public class XYWPointInteraction : BasePointInteraction {
		readonly RefinedPointsWeightComparer weightComparer = new RefinedPointsWeightComparer();
		readonly RefinedPointsValueComparer valueComparer = new RefinedPointsValueComparer();
		readonly List<RefinedPoint> pointsByWeights = new List<RefinedPoint>();
		readonly List<RefinedPoint> pointsByValues = new List<RefinedPoint>();
		readonly IXYWSeriesView view;
		public double MinWeight { get { return pointsByWeights.Count > 0 ? ((IXYWPoint)pointsByWeights[0]).Weight : double.NaN; } }
		public double MaxWeight { get { return pointsByWeights.Count > 0 ? ((IXYWPoint)pointsByWeights[pointsByWeights.Count - 1]).Weight : double.NaN; } }
		public override int Count { get { return pointsByValues.Count; } }
		public XYWPointInteraction(IXYWSeriesView view) {
			this.view = view;
		}
		public void AddPoint(RefinedPoint point) {
			InsertPointToList(pointsByValues, valueComparer, point);
			InsertPointToList(pointsByWeights, weightComparer, point);
			UpdatePointsInteractions(pointsByValues);
		}
		public void RemovePoint(RefinedPoint point) {
			point.ResetInteraction();
			RemovePointFromList(pointsByValues, valueComparer, point);
			RemovePointFromList(pointsByWeights, weightComparer, point);
			UpdatePointsInteractions(pointsByValues);
		}
		public void Clear() {
			foreach (RefinedPoint point in pointsByValues)
				point.ResetInteraction();
			pointsByValues.Clear();
			pointsByWeights.Clear();
		}
		public double GetXYWPointSize(IXYWPoint point) {
			double weightDiapasonLength = MaxWeight - MinWeight;
			if (weightDiapasonLength == 0)
				return view.MaxSize;
			double normalizedWeight = (point.Weight - MinWeight) / weightDiapasonLength;
			return (view.MaxSize - view.MinSize) * normalizedWeight + view.MinSize;
		}
		public override double GetMinValue(ISeriesView seriesView) {
			return pointsByValues.Count > 0 ? seriesView.GetRefinedPointMin(pointsByValues[0]) : double.NaN;
		}
		public override double GetMaxValue(ISeriesView seriesView) {
			return pointsByValues.Count > 0 ? seriesView.GetRefinedPointMax(pointsByValues[pointsByValues.Count - 1]) : double.NaN;
		}
		public override double GetMinAbsValue(ISeriesView seriesView) {
			return pointsByValues.Count > 0 ? seriesView.GetRefinedPointAbsMin(pointsByValues[0]) : double.NaN;
		}
	}
	public class StackedPointInteraction : BasePointInteraction {
		readonly List<RefinedSeries> series;
		readonly List<RefinedPoint> points;
		readonly List<double> startValues;
		readonly double argument;
		double minValue;
		double maxValue;
		double positiveSum;
		double negativeSum;
		double minAbsValue;
		bool shouldRecalculate;
		bool isSuplyInteraction;
		bool allSeriesNonPositive;
		protected double PositiveSum { get { return positiveSum; } }
		protected double NegativeSum { get { return negativeSum; } }
		protected ISeriesView SeriesView { get { return series.Count > 0 ? series[0].SeriesView : null; } }
		public virtual double MinValue {
			get {
				Recalculate();
				return minValue;
			}
		}
		public virtual double MaxValue {
			get {
				Recalculate();
				return maxValue;
			}
		}
		public virtual double MinAbsValue {
			get {
				Recalculate();
				return minAbsValue;
			}
		}
		public bool HasEmptyPoints {
			get {
				foreach (RefinedPoint point in points)
					if (point == null || point.IsEmpty)
						return true;
				return false;
			}
		}
		public bool IsSupplyInteraction { get { return isSuplyInteraction; } set { isSuplyInteraction = value; } }
		public override int Count { get { return points.Count; } }
		public override bool IsEmpty {
			get {
				foreach (RefinedPoint point in points)
					if (point != null)
						return false;
				return true;
			}
		}
		public override double Argument { get { return argument; } }
		public List<RefinedSeries> Series { get { return series; } }
		public RefinedPoint this[int index] { get { return points[index]; } set { points[index] = value; } }
#if DEBUGTEST
		internal List<RefinedPoint> Points { get { return points; } }
#endif
		public StackedPointInteraction(List<RefinedSeries> series, int seriesIndex, RefinedPoint point) {
			argument = point.Argument;
			points = new List<RefinedPoint>(series.Count);
			startValues = new List<double>(series.Count);
			this.series = new List<RefinedSeries>(series.Count);
			for (int i = 0; i < series.Count; i++) {
				this.series.Add(series[i]);
				if (i == seriesIndex) {
					points.Add(point);
					startValues.Add(0);
					point.SetInteraction(this, i);
				}
				else {
					points.Add(null);
					startValues.Add(double.NaN);
				}
			}
			shouldRecalculate = true;
		}
		bool CalculateAllSeriesNonPositive() {
			for(int i = 0; i < series.Count; i++) {
				if(series[i].SeriesView is IStackedView && !(series[i].SeriesView is IBarSeriesView) && series[i].HasPositivePoints)
					return false;
			}
			return true;
		}
		double GetSeriesPointValue(RefinedPoint point) {
			double pointValue = SeriesView.GetRefinedPointMax(point);
			if(!GeometricUtils.IsValidDouble(pointValue) || (pointValue < 0 && !allSeriesNonPositive))
				pointValue = 0;
			return pointValue;
		}
		void Recalculate() {
			if (!shouldRecalculate || SeriesView == null)
				return;
			minValue = double.PositiveInfinity;
			maxValue = double.NegativeInfinity;
			positiveSum = 0;
			negativeSum = 0;
			this.allSeriesNonPositive = CalculateAllSeriesNonPositive();
			for (int i = 0; i < points.Count; i++) {
				RefinedPoint point = points[i];
				if (point == null)
					continue;
				double pointValue = GetSeriesPointValue(point);
				double startValue;
				if (pointValue < 0) {
					startValue = negativeSum;
					startValues[i] = negativeSum;
					negativeSum += pointValue;
				}
				else {
					startValue = positiveSum;
					startValues[i] = positiveSum;
					positiveSum += pointValue;
				}
				double value = startValue + pointValue;
				minValue = Math.Min(minValue, value);
				maxValue = Math.Max(maxValue, value);
			}
			minAbsValue = double.MaxValue;
			for (int i = 0; i < points.Count; i++) {
				if (points[i] == null)
					continue;
				minAbsValue = Math.Min(minAbsValue, Math.Abs(SeriesView.GetRefinedPointAbsMin(points[i])));
			}
			shouldRecalculate = false;
		}
		public void InsertSeries(RefinedSeries series, int seriesIndex, RefinedPoint point) {
			if (seriesIndex < this.series.Count) {
				if (this.series[seriesIndex] != series) {
					this.series.Insert(seriesIndex, series);
					points.Insert(seriesIndex, point);
					startValues.Insert(seriesIndex, double.NaN);
				}
				else
					points[seriesIndex] = point;
				UpdatePointsInteractions(points);
			}
			else {
				this.series.Add(series);
				points.Add(point);
				startValues.Add(double.NaN);
				if (point != null)
					point.SetInteraction(this, seriesIndex);
			}
			shouldRecalculate = true;
		}
		public void RemoveSeries(int seriesIndex) {
			if (seriesIndex >= points.Count)
				throw new ArgumentException();
			points.RemoveAt(seriesIndex);
			startValues.RemoveAt(seriesIndex);
			series.RemoveAt(seriesIndex);
			UpdatePointsInteractions(points);
			shouldRecalculate = true;
		}
		public void InsertPoint(int seriesIndex, RefinedPoint point) {
			if (seriesIndex >= points.Count)
				throw new ArgumentException();
			if (points[seriesIndex] == null || !point.IsEmpty) {
				points[seriesIndex] = point;
				point.SetInteraction(this, seriesIndex);
				shouldRecalculate = true;
			}
		}
		public void RemovePoint(int seriesIndex) {
			if (seriesIndex >= points.Count)
				throw new ArgumentException();
			if (points[seriesIndex] != null)
				points[seriesIndex].ResetInteraction();
			points[seriesIndex] = null;
			shouldRecalculate = true;
		}
		public override double GetMinValue(ISeriesView seriesView) {
			return MinValue;
		}
		public override double GetMaxValue(ISeriesView seriesView) {
			return MaxValue;
		}
		public override double GetMinAbsValue(ISeriesView seriesView) {
			return MinAbsValue;
		}
		public virtual double GetStackedPointMinValue(int pointIndex) {
			Recalculate();
			if (pointIndex < startValues.Count)
				return startValues[pointIndex];
			else
				return double.NaN;
		}
		public virtual double GetStackedPointMaxValue(int pointIndex) {
			Recalculate();
			ISeriesView seriesView = SeriesView;
			return seriesView != null ? startValues[pointIndex] + GetSeriesPointValue(points[pointIndex]) : double.NaN;
		}
	}
	public class FullStackedPointInteraction : StackedPointInteraction {
		public override double MaxValue { get { return base.MaxValue <= 0 ? 0 : 1; } }
		public override double MinValue { get { return base.MinValue >= 0 ? 0 : -1; } }
		public FullStackedPointInteraction(List<RefinedSeries> series, int seriesIndex, RefinedPoint point)
			: base(series, seriesIndex, point) {
		}
		double GetNormalizedValue(double value) {
			ISeriesView seriesView = SeriesView;
			if (seriesView != null) {
				if (GeometricUtils.IsValidDouble(value)) {
					if (value < 0 && NegativeSum != 0)
						return -value / NegativeSum;
					if (value > 0 && PositiveSum != 0)
						return value / PositiveSum;
					return 0;
				}
			}
			return double.NaN;
		}
		public override double GetStackedPointMinValue(int pointIndex) {
			double minValue = base.GetStackedPointMinValue(pointIndex);
			return GetNormalizedValue(minValue);
		}
		public override double GetStackedPointMaxValue(int pointIndex) {
			double minValue = base.GetStackedPointMaxValue(pointIndex);
			return GetNormalizedValue(minValue);
		}
	}
	public class SideBySideGroup : SideBySideGroupBase {
		readonly Dictionary<RefinedPoint, RefinedSeries> points = new Dictionary<RefinedPoint, RefinedSeries>();
		public override bool IsEmpty { get { return points.Count == 0; } }
		public SideBySideGroup(RefinedSeries series) : base(series) {
		}
		public void AddPoint(RefinedSeries series, RefinedPoint point) {
			if (!points.ContainsKey(point))
				points.Add(point, series);
		}
		public void RemovePoint(RefinedPoint point) {
			points.Remove(point);
		}
		public override void UpdateInteractionKeys(int index, SideBySideInteractionBase interaction) {
			foreach (RefinedPoint point in points.Keys)
				point.SetSeriesGroupsInteraction(interaction, index);
		}
		public override void RemoveSeries(RefinedSeries series) {
			List<RefinedPoint> pointsToRemove = new List<RefinedPoint>();
			foreach (RefinedPoint point in points.Keys) {
				if (points[point] == series)
					pointsToRemove.Add(point);
			}
			foreach (RefinedPoint point in pointsToRemove)
				points.Remove(point);
			base.RemoveSeries(series);
		}
	}
	public class SideBySideSeriesGroup : SideBySideGroupBase {
		public SideBySideSeriesGroup(RefinedSeries series) : base(series) {
		}
		public override void UpdateInteractionKeys(int index, SideBySideInteractionBase interaction) {
			foreach (RefinedSeries series in SeriesList) {
				IList<RefinedPoint> finalPoints = series.FinalPoints;
				foreach (RefinedPoint point in finalPoints)
					point.SetSeriesGroupsInteraction(interaction, index);
			}
		}
	}
	public abstract class SideBySideGroupBase {
		readonly object groupKey;
		readonly List<RefinedSeries> seriesList = new List<RefinedSeries>();
		public object GroupKey { get { return groupKey; } }
		public double BarWidth { get; set; }
		public double BarDistance { get; set; }
		public int FixedOffset { get; set; }
		public virtual bool IsEmpty { get { return seriesList.Count == 0; } }
		public List<RefinedSeries> SeriesList { get { return seriesList; } }
		public SideBySideGroupBase(RefinedSeries series) {
			ISideBySideStackedBarSeriesView view = series.SeriesView as ISideBySideStackedBarSeriesView;
			if (view != null)
				groupKey = view.StackedGroup;
		}
		public void AddSeries(RefinedSeries series) {
			seriesList.Add(series);
		}
		public bool IsCompatible(RefinedSeries series) {
			ISideBySideStackedBarSeriesView view = series.SeriesView as ISideBySideStackedBarSeriesView;
			if (view != null)
				return view.StackedGroup == groupKey;
			return false;
		}
		public virtual void RemoveSeries(RefinedSeries series) {
			seriesList.Remove(series);
		}
		public abstract void UpdateInteractionKeys(int index, SideBySideInteractionBase interaction);
	}
	public abstract class SideBySideInteractionBase : IPointInteraction {
		readonly List<SideBySideGroupBase> groups = new List<SideBySideGroupBase>();
		List<RefinedSeries> series;
		bool shouldRecalculate = true;
		bool equalBarWidth = false;
		protected List<SideBySideGroupBase> Groups { get { return groups; } }
		protected virtual double Argument { get { return double.NaN; } }
		public bool IsEmpty {
			get {
				foreach (SideBySideGroupBase group in groups)
					if (!group.IsEmpty)
						return false;
				return true;
			}
		}
		public bool EqualBarWidth {
			get {
				return equalBarWidth;
			}
			set {
				equalBarWidth = value;
			}
		}
		public List<RefinedSeries> Series {
			get {
				if (series == null) {
					series = new List<RefinedSeries>();
					foreach (SideBySideGroupBase group in groups)
						foreach (RefinedSeries currentSeries in group.SeriesList)
							series.Add(currentSeries);
				}
				return series;
			}
		}
		public SideBySideInteractionBase(bool equalBarWidth) {
			this.equalBarWidth = equalBarWidth;
		}
		#region IPointInteraction
		int IPointInteraction.Count { get { return groups.Count; } }
		double IPointInteraction.Argument { get { return Argument; } }
		bool IPointInteraction.IsEmpty { get { return IsEmpty; } }
		double IPointInteraction.GetMinValue(ISeriesView seriesView) {
			return double.NaN;
		}
		double IPointInteraction.GetMaxValue(ISeriesView seriesView) {
			return double.NaN;
		}
		double IPointInteraction.GetMinAbsValue(ISeriesView seriesView) {
			return double.NaN;
		}
		#endregion
		SideBySideGroupBase FindSideBySideStackedGroup(RefinedSeries series) {
			ISideBySideStackedBarSeriesView view = series.SeriesView as ISideBySideStackedBarSeriesView;
			if (view != null) {
				foreach (SideBySideGroupBase group in Groups)
					if (Object.Equals(group.GroupKey, view.StackedGroup))
						return group;
			}
			return null;
		}
		void Recalculate() {
			if (shouldRecalculate && Groups.Count > 0) {
				double totalBarWidth = 0;
				double totalBarDistance = 0;
				int totalFixedOffset = 0;
				List<SideBySideGroupBase> validGroups = new List<SideBySideGroupBase>();
				for (int i = 0; i < Groups.Count; i++) {
					if (!Groups[i].IsEmpty)
						validGroups.Add(Groups[i]);
				}
				if (validGroups.Count > 0) {
					for (int i = 0; i < validGroups.Count; i++) {
						ISideBySideBarSeriesView view = (ISideBySideBarSeriesView)validGroups[i].SeriesList[0].SeriesView;
						double barWidth;
						if (equalBarWidth)
							barWidth = view.BarWidth / Groups.Count;
						else
							barWidth = view.BarWidth / validGroups.Count;
						int barFixedDistance = view.BarDistanceFixed + 1;
						validGroups[i].BarWidth = barWidth;
						validGroups[i].BarDistance = i == 0 ? 0 : totalBarDistance + 0.5 * barWidth;
						validGroups[i].FixedOffset = totalFixedOffset;
						totalBarWidth += i == 0 ? 0 : barWidth;
						totalBarDistance += i == 0 ? 0.5 * barWidth : barWidth;
						totalFixedOffset += i < validGroups.Count - 1 ? barFixedDistance : 0;
					}
					double halfOfTotalBarWidth = 0.5 * totalBarWidth;
					int halfFixedOffset = GeometricUtils.StrongRound(0.5 * totalFixedOffset);
					for (int i = 0; i < validGroups.Count; i++) {
						validGroups[i].BarDistance -= halfOfTotalBarWidth;
						validGroups[i].FixedOffset -= halfFixedOffset;
					}
				}
			}
			shouldRecalculate = false;
		}
		void UpdateInteractionKeys() {
			for (int i = 0; i < Groups.Count; i++) {
				Groups[i].UpdateInteractionKeys(i, this);
			}
		}
		protected int GetGroupIndex(RefinedSeries series) {
			for (int i = 0; i < groups.Count; i++)
				if (groups[i].SeriesList.Contains(series))
					return i;
			return -1;
		}
		protected abstract SideBySideGroupBase CreateGroup(RefinedSeries series);
		protected virtual void OnInsertPoint(int groupIndex, RefinedSeries series, RefinedPoint point) {
		}
		public void Invalidate() {
			shouldRecalculate = true;
		}
		public void AddSeries(RefinedSeries series) {
			SideBySideGroupBase group = FindSideBySideStackedGroup(series);
			if (group == null) {
				group = CreateGroup(series);
				groups.Add(group);
			}
			group.AddSeries(series);
			UpdateInteractionKeys();
			this.series = null;
		}
		public void InsertPoint(RefinedSeries series, RefinedPoint point) {
			int index = GetGroupIndex(series);
			if (index >= 0)
				point.SetSeriesGroupsInteraction(this, index);
			OnInsertPoint(index, series, point);
			this.series = null;
		}
		public virtual void RemoveSeries(RefinedSeries series) {
			int index = GetGroupIndex(series);
			if (index >= 0) {
				SideBySideGroupBase group = Groups[index];
				group.RemoveSeries(series);
				if (group.IsEmpty)
					groups.Remove(group);
			}
			UpdateInteractionKeys(); 
			this.series = null;
		}
		public virtual void RemovePoint(RefinedSeries series, RefinedPoint point) {
			point.ResetSeriesGroupsInteraction();
			this.series = null;
		}
		public double GetBarWidth(int index) {
			Recalculate();
			return index < groups.Count ? groups[index].BarWidth : 0;
		}
		public double GetBarDistance(int index) {
			Recalculate();
			return index < groups.Count ? groups[index].BarDistance : 0;
		}
		public int GetFixedOffset(int index) {
			Recalculate();
			return index < groups.Count ? groups[index].FixedOffset : 0;
		}
	}
	public class SideBySidePointInteraction : SideBySideInteractionBase {
		readonly double argument;
		protected override double Argument {
			get { return argument; }
		}
		public SideBySidePointInteraction(double argument, bool equalBarWidth) : base(equalBarWidth) {
			this.argument = argument;
		}
		protected override SideBySideGroupBase CreateGroup(RefinedSeries series) {
			return new SideBySideGroup(series);
		}
		protected override void OnInsertPoint(int groupIndex, RefinedSeries series, RefinedPoint point) {
			if (groupIndex >= 0)
				((SideBySideGroup)Groups[groupIndex]).AddPoint(series, point);
		}
		public override void RemovePoint(RefinedSeries series, RefinedPoint point) {
			int groupIndex = GetGroupIndex(series);
			if (groupIndex >= 0) {
				SideBySideGroup group = (SideBySideGroup)Groups[groupIndex];
				group.RemovePoint(point);
			}
			base.RemovePoint(series, point);
		}
	}
	public class SideBySideSeriesInteraction : SideBySideInteractionBase {
		public SideBySideSeriesInteraction() : base(true) {
		}
		protected override SideBySideGroupBase CreateGroup(RefinedSeries series) {
			return new SideBySideSeriesGroup(series);
		}
	}
}
