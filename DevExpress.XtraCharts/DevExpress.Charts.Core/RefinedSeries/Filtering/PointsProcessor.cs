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
	public class PointsProcessor {
		#region Nested Classes:  BasePointProcessor, ValuePointProcessor, XYPointProcessor, XYWPointProcessor, RangeBarPointProcessor, FinancialPointProcessor    
		public class BasePointProcessor : IPointProcessor {
			protected bool CanProcess(AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				return ((argumentMap != null) && (valueMap != null));
			}
			protected double GetValue(ISeriesPoint seriesPoint, int valueIndex, AxisScaleTypeMap valueMap) {
				if (valueMap.ScaleType == ActualScaleType.DateTime)
					return valueMap.NativeToInternal(seriesPoint.DateTimeValues[valueIndex]);
				return valueMap.NativeToInternal(seriesPoint.UserValues[valueIndex]);
			}
			public virtual void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
			}
		}
		public class ValuePointProcessor : BasePointProcessor {
			public override void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				if (CanProcess(argumentMap, valueMap)) {
					ISeriesPoint seriesPoint = point.SeriesPoint;
					IValuePoint valuePoint = (IValuePoint)point;
					valuePoint.Argument = argumentMap.NativeToRefined(seriesPoint.UserArgument);
					valuePoint.Value = GetValue(seriesPoint, 0, valueMap);
					if (double.IsInfinity(valuePoint.Value))
						point.IsEmpty = true;
				}
			}
		}
		public class XYPointProcessor : BasePointProcessor {
			public override void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				if (CanProcess(argumentMap, valueMap)) {
					ISeriesPoint seriesPoint = point.SeriesPoint;
					IXYPoint xyPoint = (IXYPoint)point;
					xyPoint.Argument = argumentMap.NativeToRefined(seriesPoint.UserArgument);
					xyPoint.Value = GetValue(seriesPoint, 0, valueMap);
				}
			}
		}
		public class XYWPointProcessor : BasePointProcessor {
			public override void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				if (CanProcess(argumentMap, valueMap)) {
					ISeriesPoint seriesPoint = point.SeriesPoint;
					IXYWPoint xywPoint = (IXYWPoint)point;
					xywPoint.Argument = argumentMap.NativeToRefined(seriesPoint.UserArgument);
					xywPoint.Value = GetValue(seriesPoint, 0, valueMap);
					xywPoint.Weight = GetValue(seriesPoint, 1, valueMap);
				}
			}
		}
		public class RangeBarPointProcessor : BasePointProcessor {
			public override void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				if (CanProcess(argumentMap, valueMap)) {
					ISeriesPoint seriesPoint = point.SeriesPoint;
					IRangePoint rangeBarPoint = (IRangePoint)point;
					rangeBarPoint.Argument = argumentMap.NativeToRefined(seriesPoint.UserArgument);
					rangeBarPoint.Value1 = GetValue(seriesPoint, 0, valueMap);
					rangeBarPoint.Value2 = GetValue(seriesPoint, 1, valueMap);
				}
			}
		}
		public class FinancialPointProcessor : BasePointProcessor {
			public override void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap) {
				if (CanProcess(argumentMap, valueMap)) {
					ISeriesPoint seriesPoint = point.SeriesPoint;
					IFinancialPoint financialPoint = (IFinancialPoint)point;
					financialPoint.Argument = argumentMap.NativeToRefined(seriesPoint.UserArgument);
					financialPoint.Low = GetValue(seriesPoint, 0, valueMap);
					financialPoint.High = GetValue(seriesPoint, 1, valueMap);
					financialPoint.Open = GetValue(seriesPoint, 2, valueMap);
					financialPoint.Close = GetValue(seriesPoint, 3, valueMap);
				}
			}
		}
		#endregion
		#region RandomPointsState
		public class RandomPointsState {
			readonly ActualScaleType argumentScaleType;
			readonly ActualScaleType valueScaleType;
			readonly DateTimeMeasureUnitNative argumentMeasureUnit;
			readonly DateTimeMeasureUnitNative valueMeasureUnit;
			readonly ISeriesView view;
			readonly ScaleModeNative scaleMode;
			public ActualScaleType ArgumentScaleType { get { return argumentScaleType; } }
			public ActualScaleType ValueScaleType { get { return valueScaleType; } }
			public DateTimeMeasureUnitNative ArgumentMeasureUnit { get { return argumentMeasureUnit; } }
			public DateTimeMeasureUnitNative ValueMeasureUnit { get { return valueMeasureUnit; } }
			public object View { get { return view; } }
			public RandomPointsState(ISeries series) {
				ISeriesView view = series.SeriesView;
				IXYSeriesView xyView = view as IXYSeriesView;
				this.argumentMeasureUnit = xyView != null && xyView.AxisXData != null ? this.argumentMeasureUnit = xyView.AxisXData.DateTimeScaleOptions.MeasureUnit : DateTimeMeasureUnitNative.Day;
				this.valueMeasureUnit = xyView != null &&  xyView.AxisYData != null ? this.valueMeasureUnit = xyView.AxisYData.DateTimeScaleOptions.MeasureUnit : DateTimeMeasureUnitNative.Day;
				this.scaleMode = xyView != null && xyView.AxisXData != null ? this.scaleMode = xyView.AxisXData.DateTimeScaleOptions.ScaleMode : ScaleModeNative.Manual;
				this.argumentScaleType = (ActualScaleType)series.ArgumentScaleType;
				this.valueScaleType = (ActualScaleType)series.ValueScaleType;
				this.view = view;
			}
			public override bool Equals(object obj) {
				RandomPointsState cacheItem = obj as RandomPointsState;
				return cacheItem != null &&
				cacheItem.view == view &&
				cacheItem.valueScaleType == valueScaleType &&
				cacheItem.argumentScaleType == argumentScaleType &&
				(scaleMode == ScaleModeNative.Manual ? cacheItem.argumentMeasureUnit == argumentMeasureUnit : true) &&
				(scaleMode == ScaleModeNative.Manual ? cacheItem.valueMeasureUnit == valueMeasureUnit : true);
			}
			public override int GetHashCode() {
				return view.GetHashCode() ^ argumentMeasureUnit.GetHashCode() ^ valueMeasureUnit.GetHashCode() ^ argumentScaleType.GetHashCode() ^ valueScaleType.GetHashCode();
			}
		}
		#endregion
		readonly IRefinedSeriesContainer seriesContainer;
		readonly CollectionManager realCollections;
		readonly CollectionManager randomCollections;
		readonly Dictionary<Scale, int> scaleCounters = new Dictionary<Scale,int>();
		BasePointProcessor pointProcessor;
		bool noArgumentScaleType;
		RandomPointsState randomPointsState;
		bool? hasPositivePoint = null;
		readonly PointsValueThresholdFilter filter;
		readonly List<PointsFilter> filters = new List<PointsFilter>();
		readonly RefinedSeries refinedSeries;
		readonly Dictionary<object, SortedRefinedPointCollection> sortedPointsCache;
		ISeries Series { get { return refinedSeries.Series; } }
		ISeriesView SeriesView { get { return Series.SeriesView; } }
		public bool HasPositivePoints {
			get {
				if (hasPositivePoint.HasValue)
					return hasPositivePoint.Value;
				for (int i = 0; i < refinedSeries.FinalPoints.Count; i++) { 
					if (((IStackedPoint)refinedSeries.Points[i]).Value > 0) {
						hasPositivePoint = true;
						return hasPositivePoint.Value;
					}
				}
				hasPositivePoint = false;
				return hasPositivePoint.Value;
			}
		}
		public CollectionManager RealCollections { get { return realCollections; } }
		public CollectionManager RandomCollections { get { return randomCollections; } }
		public CollectionManager ActualCollectionManager {
			get {
				if (ShouldUseAutoGeneratedPoints)
					return randomCollections;
				return realCollections;
			}
		}
		public bool ShouldUseAutoGeneratedPoints {
			get { return seriesContainer.IsDesignMode && !seriesContainer.IsContainsProcessedPoints; }
		}
		public bool NoArgumentScaleType { get { return noArgumentScaleType; } }
		public Dictionary<Scale, int> ScaleCounters { get { return scaleCounters; } }
		public bool IsContainsProcessedPoints { get { return ((realCollections.Points != null) && (realCollections.Points.Count > 0)); } }
		public bool IsContainsProcessedNotEmptyPoints {
			get {
				if (realCollections.Points != null) {
					foreach (RefinedPoint point in realCollections.Points) {
						if (!point.IsEmpty)
							return true;
					}
				}
				return false;
			}
		}
		public PointsProcessor(RefinedSeries refinedSeries, IRefinedSeriesContainer seriesContainer) {
			this.refinedSeries = refinedSeries;
			this.seriesContainer = seriesContainer;
			this.randomPointsState = new RandomPointsState(Series);
			realCollections = new CollectionManager(refinedSeries);
			randomCollections = new CollectionManager(refinedSeries);
			this.sortedPointsCache = new Dictionary<object, SortedRefinedPointCollection>();
			this.filter = new PointsValueThresholdFilter(refinedSeries);
			InitFilters();
			UpdateFilters();
			InsurePointProcessor();
			InitCollections();
		}
		void InitCollections() {
			Collection<ISeriesPoint> points = new Collection<ISeriesPoint>();
			if (Series.Points != null)
				foreach (ISeriesPoint point in Series.Points)
					points.Add(point);
			ResetScaleCounters();
			ChartUpdate updateResult = realCollections.UpdateCollections(pointProcessor, SeriesPointCollectionBatchUpdateInfo.CreateInsertInfo(this, Series, points, 0));
			UpdateScaleCounters(updateResult);
			DetectScale();
			IList<ISeriesPoint> randomPoints = null;
			if (Series.SeriesView != null)
				randomPoints = Series.SeriesView.GenerateRandomPoints(Series.ArgumentScaleType, Series.ValueScaleType);
			randomCollections.UpdateCollections(pointProcessor, SeriesPointCollectionBatchUpdateInfo.CreateInsertInfo(this, Series, randomPoints, 0));
		}
		void InitFilters() {
			filters.Add(new WorkdaysPointsFilter(refinedSeries));
			filters.Add(new AggregationPointFilter(refinedSeries));
		}
		void ClearCache() {
			filter.ClearCache();
			foreach (PointsFilter item in filters)
				item.ClearCache();
			sortedPointsCache.Clear();
		}
		internal void UpdateArgumentScale() {
			ResetScaleCounters();
			UpdateScaleCounters();
			DetectScale();
		}
		public bool UpdateFilters() {
			bool result = filter.Update();
			foreach (PointsFilter item in filters) {
				if (result) {
					item.ClearCache();
					item.Update();
				}
				else
					result = result || item.Update();
			}
			return result;			
		}
		IList<RefinedPoint> GetInitialPoints(bool needProcessNewPoint) {
			if (filter.Enable) {
			   IList<RefinedPoint> res = filter.GeFilteredPoints(refinedSeries.PointsSortedBySettings);
			   if (needProcessNewPoint) 
				   filter.ProcessOthersPoint(pointProcessor, refinedSeries.ArgumentGroup != null ? refinedSeries.ArgumentGroup.ScaleMap : null, 
															 refinedSeries.ValueGroup != null ? refinedSeries.ValueGroup.ScaleMap : null);
			   return res;
			}
			return refinedSeries.PointsSortedBySettings;
		}
		SortedRefinedPointCollection GetSortedByArgumentPoints(IList<RefinedPoint> intermediate) {
			SortedRefinedPointCollection sortedPoints;
			if (!sortedPointsCache.ContainsKey(intermediate)) {
				sortedPoints = new SortedRefinedPointCollection(intermediate.Count, new RefinedPointsArgumentAndIndexComparer());
				for (int i = 0; i < intermediate.Count; i++)
					sortedPoints.Add((RefinedPoint)intermediate[i]);
				sortedPointsCache.Add(intermediate, sortedPoints);
			}
			else
				sortedPoints = sortedPointsCache[intermediate];
			return sortedPoints;
		}
		bool ShouldProcessData() {
			foreach (PointsFilter filter in filters)
				if (filter.Enable)
					return true;
			return false;
		}		
		public IList<RefinedPoint> GetPointsForMap() {
			filter.Update();
			return GetInitialPoints(false);
		}
		public IList<RefinedPoint> GetProcessedPoints(bool shouldSortByArgument) {
			UpdateFilters();
			IList<RefinedPoint> initialPoints = GetInitialPoints(true);
			IList<RefinedPoint> intermediate = initialPoints;
			if (intermediate != null) {
				for (int i = 0; i < filters.Count; i++)
					if (filters[i].Enable) {
						if (intermediate != initialPoints)
							intermediate = filters[i].GeFilteredPoints(GetSortedByArgumentPoints(intermediate));
						else
							intermediate = filters[i].GeFilteredPoints(refinedSeries.PointsSortedByArgument);
					}
			}
			if (shouldSortByArgument) {
				if (intermediate == refinedSeries.PointsSortedBySettings)
					return refinedSeries.PointsSortedByArgument;
				return GetSortedByArgumentPoints(intermediate);
			}
			return intermediate;
		}
		void InsurePointProcessor() {
			if (SeriesView != null) {
				Type pointType = SeriesView.PointInterfaceType;
				if (pointType == typeof(IValuePoint))
					pointProcessor = new ValuePointProcessor();
				else if (pointType == typeof(IXYPoint) || pointType == typeof(IStackedPoint) || pointType == typeof(IFullStackedPoint))
					pointProcessor = new XYPointProcessor();
				else if (pointType == typeof(IXYWPoint))
					pointProcessor = new XYWPointProcessor();
				else if (pointType == typeof(IRangePoint))
					pointProcessor = new RangeBarPointProcessor();
				else if (pointType == typeof(IFinancialPoint))
					pointProcessor = new FinancialPointProcessor();
			}
			else 
				pointProcessor = null;
		}
		public void UpdateData() {
			InsurePointProcessor();
			realCollections.ProcessPoints(pointProcessor);
			randomCollections.ProcessPoints(pointProcessor);
			Invalidate(null);			
		}
		public void UpdateRandomPoints() {
			RandomPointsState newState = new RandomPointsState(Series);
			if (!randomPointsState.Equals(newState)) {
				InsurePointProcessor();
				randomPointsState = newState;
				randomCollections.UpdateCollections(pointProcessor, RandomPointsBatchUpdateInfo.CreateResetInfo(Series));
				randomCollections.UpdateCollections(pointProcessor, RandomPointsBatchUpdateInfo.CreateInsertInfo(Series));
			}
		}
		public void ProcessSortingPointKeyUpdate(SeriesPointKeyNative oldSortingKey, SeriesPointKeyNative newSortingKey, SortMode pointsSortingMode) {
			realCollections.ProcessSortingPointKeyUpdate(oldSortingKey, newSortingKey, pointsSortingMode);
			randomCollections.ProcessSortingPointKeyUpdate(oldSortingKey, newSortingKey, pointsSortingMode);
			Invalidate(null);
		}
		public void ProcessSortingPointModeUpdate(SortMode oldSortingMode, SortMode newSortingMode, SeriesPointKeyNative sortingKey) {
			realCollections.ProcessSortingPointModeUpdate(oldSortingMode, newSortingMode, sortingKey);
			randomCollections.ProcessSortingPointModeUpdate(oldSortingMode, newSortingMode, sortingKey);
			Invalidate(null);
		}
		internal ChartUpdate UpdatePoints(CollectionUpdateInfo updateInfo) {
			InsurePointProcessor();
			ChartUpdate updateResult;
			if (updateInfo is RandomPointsBatchUpdateInfo) {
				updateResult = randomCollections.UpdateCollections(pointProcessor, updateInfo);
				if (realCollections.Points.Count > 0)
					updateResult = null;
			}
			else
				updateResult = realCollections.UpdateCollections(pointProcessor, updateInfo);
			if (!(updateInfo is RandomPointsBatchUpdateInfo))
				DetectShouldUpdateScaleMap(updateResult);
			Invalidate(updateResult);
			return updateResult;
		}
		internal ChartUpdate UpdatePoint(PropertyUpdateInfo<ISeries, ISeriesPoint> seriesPointUpdateInfo) {
			InsurePointProcessor();
			ChartUpdate updateResult = realCollections.UpdatePoint(pointProcessor, seriesPointUpdateInfo);
			DetectShouldUpdateScaleMap(updateResult);
			if (!refinedSeries.IsActive)
				return null;
			Invalidate(updateResult);
			return updateResult;
		}
		void DetectShouldUpdateScaleMap(ChartUpdate updateResult) {
			if (updateResult != null)
				UpdateScaleCounters(updateResult);
			ActualScaleType detectedScale = DetectScale();
			if (detectedScale != (ActualScaleType)Series.ArgumentScaleType) {
				if (updateResult != null)
					updateResult.AddUpdate(ChartUpdateType.UpdateScaleMap);
			}
		}
		internal void Invalidate(ChartUpdate updateResult) {
			hasPositivePoint = null;
			ClearCache();
			if (updateResult != null && ShouldProcessData())
				updateResult.AddUpdate(ChartUpdateType.UpdateInteraction);
		}
		void ResetScaleCounters() {
			foreach (var value in Enum.GetValues(typeof(Scale)))
				scaleCounters[(Scale)value] = 0;
		}
		void UpdateScaleCounters() {
			RefinedPointCollectionBase points = realCollections.Points;
			for (int i = 0; i < points.Count; i++) {
				scaleCounters[points[i].ArgumentScale]++;
			}
		}
		Scale DetectScaleWithCounters(Scale initialScale) {
			if (initialScale == Scale.Auto) {
				if ((scaleCounters[Scale.Qualitative] != 0) || ((scaleCounters[Scale.DateTime] != 0) && (scaleCounters[Scale.Numerical] != 0)))
					return Scale.Qualitative;
				if (scaleCounters[Scale.DateTime] != 0)
					return Scale.DateTime;
				if (scaleCounters[Scale.Numerical] != 0)
					return Scale.Numerical;
				return Scale.Auto;
			}
			return initialScale;
		}
		void CheckScale(Scale scale) {
			switch (scale) {
				case Scale.Numerical:
					if ((scaleCounters[Scale.DateTime] != 0) || (scaleCounters[Scale.Qualitative] != 0))
						throw new ArgumentException("Non-numerical data in series. Can't set scale type to Numerical.");
					break;
				case Scale.DateTime:
					if ((scaleCounters[Scale.Numerical] != 0) || (scaleCounters[Scale.Qualitative] != 0))
						throw new ArgumentException("Not DateTime type data in series. Can't set scale type to DateTime.");
					break;
				default:
					break;
			}
		}
		internal ActualScaleType DetectScale() {
			noArgumentScaleType = false;
			Scale userScale = refinedSeries.Series.UserArgumentScaleType;
			RefinedPointCollectionBase points = ActualCollectionManager.Points;
			if (userScale == Scale.Auto) {
				userScale = DetectScaleWithCounters(userScale);
				if (userScale == Scale.Auto) {
					userScale = Scale.Numerical;
					noArgumentScaleType = true;
				}
			}
			refinedSeries.Series.SetArgumentScaleType(userScale);
			return (ActualScaleType)userScale;
		}
		internal void UpdateScaleCounters(ChartUpdate updateResult) {
			BatchPointsUpdate batchResult = updateResult as BatchPointsUpdate;
			SinglePointUpdate singleResult = updateResult as SinglePointUpdate;
			if (singleResult != null) {
				switch (singleResult.Operation) {
					case ChartCollectionOperation.Reset:
						ResetScaleCounters();
						break;
					case ChartCollectionOperation.InsertItem:
						scaleCounters[singleResult.AffectedPoint.ArgumentScale]++;
						break;
					case ChartCollectionOperation.RemoveItem:
						scaleCounters[singleResult.AffectedPoint.CachedArgumentScale]--;
						break;
					case ChartCollectionOperation.UpdateItem:
						scaleCounters[singleResult.AffectedPoint.ArgumentScale]++;
						scaleCounters[singleResult.DeprectedPoint.CachedArgumentScale]--;
						break;
				}
			}
			else if (batchResult != null) {
				switch (batchResult.Operation) {
					case ChartCollectionOperation.Reset:
						ResetScaleCounters();
						break;
					case ChartCollectionOperation.InsertItem:
						RefinedPoint[] affectedPoints = batchResult.AffectedPoints;
						for (int i = 0; i < affectedPoints.Length; i++)
							scaleCounters[affectedPoints[i].ArgumentScale]++;
						break;
					case ChartCollectionOperation.RemoveItem:
						affectedPoints = batchResult.AffectedPoints;
						for (int i = 0; i < affectedPoints.Length; i++)
							scaleCounters[affectedPoints[i].CachedArgumentScale]--;
						break;
				}
			}
		}		
	}
	public interface IPointProcessor {
		void ProcessPoint(RefinedPoint point, AxisScaleTypeMap argumentMap, AxisScaleTypeMap valueMap);
	}
}
