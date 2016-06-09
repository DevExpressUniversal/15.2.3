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
using System.Collections.ObjectModel;
namespace DevExpress.Charts.Native {
	public partial class RefinedSeries : IRefinedSeries, INestedDoughnutRefinedSeries {
		readonly ISeries series;
		readonly IRefinedSeriesContainer seriesContainer;
		readonly PointsProcessor pointsProcessor;
		bool isActive;
		int minVisibleIndex;
		int maxVisibleIndex;
		RefinedSeriesGroup valueGroup;
		RefinedSeriesGroup argumentGroup;
		SeriesGroupsInteractionContainer data;
		int interactionIndex;
		bool UseOnlyVisiblePoints {
			get { return (SeriesView != null) && SeriesView.NeedFilterVisiblePoints && !series.LabelsVisibility; }
		}
		internal SeriesInteractionContainer InteractionContainer { get; set; }
		internal bool HasPositivePoints { get { return pointsProcessor.HasPositivePoints; } }
		internal RefinedPointCollectionBase Points {
			get { return pointsProcessor.ActualCollectionManager.Points; }
		}
		public bool IsContainsProcessedPoints {
			get { return pointsProcessor.IsContainsProcessedPoints; }
		}
		public bool IsContainsProcessedNotEmptyPoints {
			get { return pointsProcessor.IsContainsProcessedNotEmptyPoints; }
		}
		public ISeries Series {
			get { return series; }
		}
		public ISeriesView SeriesView {
			get { return Series.SeriesView; }
		}
		public RefinedSeriesGroup ValueGroup {
			get { return valueGroup; }
		}
		public RefinedSeriesGroup ArgumentGroup {
			get { return argumentGroup; }
		}
		public bool IsActive {
			get { return isActive; }
		}
		public int ActiveIndex { get; set; }
		public bool NoArgumentScaleType {
			get { return pointsProcessor.NoArgumentScaleType; }
		}
		public ActualScaleType ArgumentScaleType {
			get {
				if (series.ArgumentScaleType == Scale.Auto)
					return ActualScaleType.Numerical;
				return (ActualScaleType)series.ArgumentScaleType;
			}
		}
		public ActualScaleType ValueScaleType {
			get { return (ActualScaleType)series.ValueScaleType; }
		}
		public RefinedPointCollectionBase PointsSortedBySettings {
			get { return pointsProcessor.ActualCollectionManager.PointsSortedBySettings; }
		}
		public RefinedPointCollectionBase PointsSortedByArgument {
			get { return pointsProcessor.ActualCollectionManager.PointsSortedByArgument; }
		}
		public IList<RefinedPoint> PointsForMap {
			get { return pointsProcessor.GetPointsForMap(); }
		}
		public IList<RefinedPoint> FinalPointsSortedByArgument {
			get {
				return pointsProcessor.GetProcessedPoints(true);
			}
		}
		public IList<RefinedPoint> FinalPoints {
			get {
				return pointsProcessor.GetProcessedPoints(false);
			}
		}
		public double MinArgument {
			get { return FinalPointsSortedByArgument.Count > 0 ? FinalPointsSortedByArgument[0].Argument : double.NaN; }
		}
		public double MaxArgument {
			get { return FinalPointsSortedByArgument.Count > 0 ? FinalPointsSortedByArgument[FinalPointsSortedByArgument.Count - 1].Argument : double.NaN; }
		}
		public RefinedPoint MinArgumentPoint {
			get { return PointsSortedByArgument.Count > 0 ? PointsSortedByArgument[0] : null; }
		}
		public RefinedPoint MaxArgumentPoint {
			get { return PointsSortedByArgument.Count > 0 ? PointsSortedByArgument[PointsSortedByArgument.Count - 1] : null; }
		}
		public int MinVisiblePointIndex {
			get {
				if (UseOnlyVisiblePoints)
					return Math.Max(0, minVisibleIndex);
				return 0;
			}
		}
		public int MaxVisiblePointIndex {
			get {
				if (!IsActive)
					return -1;
				if (UseOnlyVisiblePoints)
					return Math.Min(maxVisibleIndex, FinalPoints.Count - 1);
				return FinalPoints.Count - 1;
			}
		}
		public bool IsPointsAutoGenerated {
			get { return pointsProcessor.ShouldUseAutoGeneratedPoints; }
		}
		public RefinedSeries(ISeries series, IRefinedSeriesContainer seriesController) {
			this.minVisibleIndex = 0;
			this.maxVisibleIndex = -1;
			this.series = series;
			this.seriesContainer = seriesController;
			this.pointsProcessor = new PointsProcessor(this, seriesController);
		}
		#region IRefinedSeries implementation
		bool IRefinedSeries.IsFirstInContainer { get { return seriesContainer != null ? seriesContainer.IsFirstInContainer(this) : false; } }
		IList<RefinedPoint> IRefinedSeries.Points {
			get {
				if (SeriesView != null && SeriesView.ShouldSortPoints)
					return FinalPointsSortedByArgument;
				return FinalPoints;
			}
		}
		int IRefinedSeries.MinVisiblePointIndex {
			get {
				if (SeriesView != null && SeriesView.ShouldSortPoints)
					return MinVisiblePointIndex;
				return 0;
			}
		}
		int IRefinedSeries.MaxVisiblePointIndex {
			get {
				if (SeriesView != null && SeriesView.ShouldSortPoints)
					return MaxVisiblePointIndex;
				return FinalPoints.Count - 1;
			}
		}
		RefinedPoint IRefinedSeries.GetMinPoint(double argument) {
			return pointsProcessor.ActualCollectionManager.GetMinPoint(argument);
		}
		RefinedPoint IRefinedSeries.GetMaxPoint(double argument) {
			return pointsProcessor.ActualCollectionManager.GetMaxPoint(argument);
		}
		bool IRefinedSeries.IsSameContainers(IRefinedSeries refinedSeries) {
			return seriesContainer.HasSameContainers(refinedSeries, this);
		}
		List<RefinedPoint> IRefinedSeries.FindAllPointsWithSameArgument(RefinedPoint refinedPoint) {
			return pointsProcessor.RealCollections.FindAllPointsWithSameArgument(refinedPoint);
		}
		#endregion
		#region INestedDoughnutRefinedSeries
		double INestedDoughnutRefinedSeries.StartOffset {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetStartOffset(interactionIndex) : double.NaN;
			}
		}
		double INestedDoughnutRefinedSeries.StartOffsetInPixels {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetStartOffsetInPixels(interactionIndex) : double.NaN;
			}
		}
		double INestedDoughnutRefinedSeries.HoleRadius {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetHoleRadius(interactionIndex) : double.NaN;
			}
		}
		double INestedDoughnutRefinedSeries.TotalGroupIndentInPixels {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetTotalGroupIndentInPixels(interactionIndex) : double.NaN;
			}
		}
		double INestedDoughnutRefinedSeries.NormalizedWeight {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetNormalizedWeight(interactionIndex) : double.NaN;
			}
		}
		double INestedDoughnutRefinedSeries.ExplodedFactor {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetExplodedFactor(interactionIndex) : double.NaN;
			}
		}
		bool INestedDoughnutRefinedSeries.IsExploded {
			get {
				NestedDoughnutInteractionContainer nestedDoughnutInteractionContainer = data as NestedDoughnutInteractionContainer;
				return nestedDoughnutInteractionContainer != null ? nestedDoughnutInteractionContainer.GetIsExploded(interactionIndex) : false;
			}
		}
		#endregion
		IAxisData GetArgumentAxisData() {
			IXYSeriesView view = SeriesView as IXYSeriesView;
			return view != null ? view.AxisXData : null;
		}
		static int GetIndexByArgument(IList<RefinedPoint> points, double argument, out bool pointIsFound) {
			RefinedPoint fakePoint = new RefinedPoint(null, argument, 0);
			RefinedPointArgumentComparer pointArgumentComparer = new RefinedPointArgumentComparer();
			SortedRefinedPointCollection sortedPoints = points as SortedRefinedPointCollection;
			int index = -1;
			if (sortedPoints != null) {
				RefinedPointsArgumentComparer comparer = new RefinedPointsArgumentComparer();
				index = sortedPoints.BinarySearch(fakePoint, comparer);
			} else if (points is List<RefinedPoint>)
				index = ((List<RefinedPoint>)points).BinarySearch(fakePoint, pointArgumentComparer);
			pointIsFound = true;
			if (index < 0) {
				index = ~index;
				pointIsFound = false;
			}
			return index;
		}
		internal RefinedPoint GetFinalPointByArgument(double argument, out int index) {
			bool isFounded;
			IList<RefinedPoint> finalPoints = FinalPointsSortedByArgument;
			index = GetIndexByArgument(finalPoints, argument, out isFounded);
			if (!isFounded)
				return null;
			return finalPoints[index];
		}
		public void BindToGroups(RefinedSeriesGroup argumentGroup, RefinedSeriesGroup valueGroup) {
			this.argumentGroup = argumentGroup;
			this.valueGroup = valueGroup;
			this.isActive = (valueGroup != null);
		}
		public void UpdateArgumentScale() {
			pointsProcessor.DetectScale();
		}
		public void SetArgumentScale(Scale scale) {
			Series.SetArgumentScaleType(scale);
		}
		public bool UpdateFilters() {
			return pointsProcessor.UpdateFilters();
		}
		public void UpdateData() {
			pointsProcessor.UpdateData();
		}
		public void InvalidateData() {
			pointsProcessor.Invalidate(null);
		}
		public void UpdateRandomPoints() {
			pointsProcessor.UpdateRandomPoints();
		}
		public void ProcessSortingPointKeyUpdate(SeriesPointKeyNative oldSortingKey, SeriesPointKeyNative newSortingKey, SortMode pointsSortingMode) {
			pointsProcessor.ProcessSortingPointKeyUpdate(oldSortingKey, newSortingKey, pointsSortingMode);
		}
		public void ProcessSortingPointModeUpdate(SortMode oldSortingMode, SortMode newSortingMode, SeriesPointKeyNative sortingKey) {
			pointsProcessor.ProcessSortingPointModeUpdate(oldSortingMode, newSortingMode, sortingKey);
		}
		public void UpdateVisiblePointIndexes(IMinMaxValues visualRangeValues) {
			if (visualRangeValues != null) {
				CalculateVisiblePointIndexes(visualRangeValues, out minVisibleIndex, out maxVisibleIndex);
			} else {
				minVisibleIndex = 0;
				maxVisibleIndex = Points.Count - 1;
			}
		}
		public void CalculateVisiblePointIndexes(IMinMaxValues visualRangeValues, out int minIndex, out int maxIndex) {
			IList<RefinedPoint> points = FinalPointsSortedByArgument;
			minIndex = 0;
			maxIndex = -1;
			if (points.Count == 0)
				return;
			bool pointIsFound;
			int index = GetIndexByArgument(points, visualRangeValues.Min, out pointIsFound);
			if (index > (points.Count - 1) && !pointIsFound) {
				minIndex = (points.Count - 1);
				maxIndex = (points.Count - 1);
				return;
			}
			if (index > 0 && !pointIsFound)
				minIndex = index - 1;
			else
				minIndex = index;
			index = GetIndexByArgument(points, visualRangeValues.Max, out pointIsFound);
			if (index <= 0 && !pointIsFound) {
				minIndex = 0;
				maxIndex = 0;
				return;
			} else if (index >= points.Count - 1)
				maxIndex = points.Count - 1;
			else
				maxIndex = index;
		}
		public double GetMinAbsArgument() {
			return pointsProcessor.ActualCollectionManager.GetMinAbsArgument();
		}
		public IList<RefinedPoint> GetDrawingPoints() {
			StackedInteractionContainer container = InteractionContainer as StackedInteractionContainer;
			if (container != null && container.IsContinousView)
				return container.GetStackedPointsForDrawing(this);
			return ((IRefinedSeries)this).Points;
		}
		public override string ToString() {
			return "RefinedSeries: " + series.ToString();
		}
		public void SetGroupsInteraction(SeriesGroupsInteractionContainer data, int index) {
			this.data = data;
			this.interactionIndex = index;
		}
		public void UpdatePointsIndices() {
			pointsProcessor.RealCollections.UpdatePointsIndices();
		}
		internal ChartUpdate UpdatePoints(CollectionUpdateInfo updateInfo) {
			return pointsProcessor.UpdatePoints(updateInfo);
		}
		internal ChartUpdate UpdatePoint(PropertyUpdateInfo<ISeries, ISeriesPoint> seriesPointUpdateInfo) {
			return pointsProcessor.UpdatePoint(seriesPointUpdateInfo);
		}
		internal void RecalculateArgumentScale() {
			pointsProcessor.UpdateArgumentScale();
		}
	}
}
