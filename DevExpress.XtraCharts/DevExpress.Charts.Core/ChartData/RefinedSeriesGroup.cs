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

using DevExpress.Charts.ChartData;
using System;
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class RefinedSeriesGroupKey {
		public static bool operator ==(RefinedSeriesGroupKey a, RefinedSeriesGroupKey b) {
			return Object.Equals(a, b);
		}
		public static bool operator !=(RefinedSeriesGroupKey a, RefinedSeriesGroupKey b) {
			return !Object.Equals(a, b);
		}
		readonly IAxisData axisData;
		public IAxisData AxisData { get { return axisData; } }
		public bool IsSeriesGroupKey { get { return axisData == null; } }
		public RefinedSeriesGroupKey(RefinedSeries series, bool isArgumentKey) {
			IXYSeriesView view = series.SeriesView as IXYSeriesView;
			if (view != null)
				axisData = isArgumentKey ? view.AxisXData : view.AxisYData;
			else
				axisData = null;
		}
		public RefinedSeriesGroupKey(IAxisData axisData) {
			this.axisData = axisData;
		}
		public override bool Equals(Object obj) {
			return Equals(obj as RefinedSeriesGroupKey);
		}
		public bool Equals(RefinedSeriesGroupKey key) {
			if (key != null)
				return axisData != null ? axisData == key.axisData : base.Equals(key);
			return false;
		}
		public override int GetHashCode() {
			return axisData != null ? axisData.GetHashCode() : base.GetHashCode();
		}
	}
	public abstract class RefinedSeriesGroup {
		readonly RefinedSeriesGroupKey groupKey;
		readonly List<RefinedSeries> refinedSeries = new List<RefinedSeries>();
		readonly List<RefinedSeries> hiddenRefinedSeries = new List<RefinedSeries>();
		AxisScaleTypeMap scaleMap = null;
		public RefinedSeriesGroupKey GroupKey { get { return groupKey; } }
		public AxisScaleTypeMap ScaleMap { get { return scaleMap; } }
		public List<RefinedSeries> RefinedSeries { get { return refinedSeries; } }
		public bool IsEmpty { get { return refinedSeries.Count == 0; } }
		public RefinedSeriesGroup(RefinedSeriesGroupKey groupKey) {
			this.groupKey = groupKey;
		}
		AxisNumericalMap CreateNumericalMap(RefinedSeries series) {
			double? measureUnit = GetNumericalMeasureUnit();
			return measureUnit.HasValue ? new AxisNumericalMap(measureUnit.Value) : new AxisNumericalMap();
		}
		AxisDateTimeMap CreateDateTimeMap(RefinedSeries series) {
			if (groupKey.AxisData != null && groupKey.AxisData.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Continuous)
				return new AxisDateTimeMap(GetDateTimeMeasureUnit(), groupKey.AxisData.DateTimeScaleOptions.WorkdaysOptions);
			else
				return new AxisDateTimeMap();
		}
		AxisScaleTypeMap CreateScaleMap(RefinedSeries series) {
			if (series != null) {
				switch (GetScaleType(series)) {
					case ActualScaleType.Numerical:
						return CreateNumericalMap(series);
					case ActualScaleType.DateTime:
						return CreateDateTimeMap(series);
					case ActualScaleType.Qualitative:
						return CreateQualitativeMap();
				}
			}
			return null;
		}
		RefinedSeries FindMainSeries() {
			RefinedSeries result = null;
			int minIndex = int.MaxValue;
			foreach (RefinedSeries series in RefinedSeries) {
				int index = series.ActiveIndex;
				if (minIndex > index) {
					minIndex = index;
					result = series;
				}
			}
			if (result != null)
				return result;
			foreach (RefinedSeries series in hiddenRefinedSeries) {
				int index = series.ActiveIndex;
				if (minIndex > index) {
					minIndex = index;
					result = series;
				}
			}
			return result;
		}
		void BindScaleMapToAxis() {
			if (groupKey.AxisData != null) {
				if (scaleMap != null)
					scaleMap.BuildTransformation(groupKey.AxisData);
				groupKey.AxisData.AxisScaleTypeMap = scaleMap;
			}
		}
		protected abstract AxisQualitativeMap CreateQualitativeMap();
		protected abstract ActualScaleType GetScaleType(RefinedSeries series);
		MinMaxValues GetMinMaxValues(IAxisData axisData) {
			MinMaxValues fromSeries = GetMinMaxValuesFromSeries();
			MinMaxValues fromAxisElements = GetMinMaxValuesFromAxisElements(axisData, axisData.AxisScaleTypeMap);
			double min = fromSeries.Min;
			double max = fromSeries.Max;
			if (fromAxisElements.Min != 0)
				min = Math.Min(min, fromAxisElements.Min);
			if (fromAxisElements.Max != 0)
				max = Math.Max(max, fromAxisElements.Max);
			return new MinMaxValues(min, max);
		}
		MinMaxValues GetMinMaxValuesFromAxisElements(IAxisElementContainer elementContainer, IScaleMap map) {
			MinMaxValues minMaxValues = new MinMaxValues(double.MaxValue, double.MinValue);
			if (elementContainer.Strips != null)
				foreach (IStrip strip in elementContainer.Strips)
					if (strip.Visible) {
						minMaxValues.Union(map.NativeToInternal(strip.MinLimit.GetAxisValue()));
						minMaxValues.Union(map.NativeToInternal(strip.MaxLimit.GetAxisValue()));
					}
			if (elementContainer.ScaleBreaks != null)
				foreach (IScaleBreak scaleBreak in elementContainer.ScaleBreaks)
					if (scaleBreak.Visible) {
						minMaxValues = CheckAxisValueContainer(minMaxValues, scaleBreak.Edge1, map);
						minMaxValues = CheckAxisValueContainer(minMaxValues, scaleBreak.Edge2, map);
					}
			if (elementContainer.ConstantLines != null)
				foreach (IConstantLine line in elementContainer.ConstantLines)
					if (line.Visible)
						minMaxValues = CheckAxisValueContainer(minMaxValues, line, map);
			if (elementContainer.CustomLabels != null)
				foreach (ICustomAxisLabel label in elementContainer.CustomLabels)
					if (label.Visible)
						minMaxValues = CheckAxisValueContainer(minMaxValues, label, map);
			return minMaxValues;
		}
		public abstract MinMaxValues GetMinMaxRefined();
		public abstract MinMaxValues GetMinMaxValuesFromSeries();
		public abstract MinMaxValues GetMinMaxValuesFromSeries(IMinMaxValues rangeForFiltering, List<RefinedSeries> seriesForFiltering);
		internal abstract double GetMinAbsValue();
		double GetMinAbsValueFromAxisElements(IAxisElementContainer elementContainer) {
			double min = double.MaxValue;
			AxisScaleTypeMap map = scaleMap;
			if (map == null)
				return min;
			if (elementContainer.Strips != null)
				foreach (IStrip strip in elementContainer.Strips)
					if (strip.Visible) {
						min = CheckMin(min, map.NativeToInternal(strip.MinLimit.GetAxisValue()));
						min = CheckMin(min, map.NativeToInternal(strip.MaxLimit.GetAxisValue()));
					}
			if (elementContainer.ScaleBreaks != null)
				foreach (IScaleBreak scaleBreak in elementContainer.ScaleBreaks)
					if (scaleBreak.Visible) {
						min = CheckMin(min, map.NativeToInternal(scaleBreak.Edge1.GetAxisValue()));
						min = CheckMin(min, map.NativeToInternal(scaleBreak.Edge2.GetAxisValue()));
					}
			if (elementContainer.ConstantLines != null)
				foreach (IConstantLine line in elementContainer.ConstantLines)
					if (line.Visible)
						min = CheckMin(min, map.NativeToInternal(line.GetAxisValue()));
			if (elementContainer.CustomLabels != null)
				foreach (ICustomAxisLabel label in elementContainer.CustomLabels)
					if (label.Visible)
						min = CheckMin(min, map.NativeToInternal(label.GetAxisValue()));
			return min;
		}
		double CheckMin(double min, double value) {
			if (value != 0)
				return Math.Min(min, Math.Abs(value));
			return min;
		}
		public void AddSeries(RefinedSeries series, bool visible) {
			if (visible) {
				if (!refinedSeries.Contains(series)) {
					refinedSeries.Add(series);
					if (hiddenRefinedSeries.Contains(series))
						hiddenRefinedSeries.Remove(series);
				}
			}
			else {
				if (!hiddenRefinedSeries.Contains(series))
					hiddenRefinedSeries.Add(series);
			}
		}
		public void RemoveSeries(Native.RefinedSeries series) {
			refinedSeries.Remove(series);
			hiddenRefinedSeries.Remove(series);
		}
		public void UpdateTransformation() {
			if (GroupKey.AxisData == null)
				return;
			IAxisData axis = GroupKey.AxisData as IAxisData;
			AxisScaleTypeMap map = axis.AxisScaleTypeMap;
			Transformation transformation = map.Transformation;
			if (!transformation.IsIdentity) {
				transformation.Reset();
				MinMaxValues minMaxValues = GetMinMaxValues(axis);
				if ((minMaxValues.Min > 0 && minMaxValues.Max > 0) || (minMaxValues.Min < 0 && minMaxValues.Max < 0) || (minMaxValues.Min == 0 && minMaxValues.Max == 1)) {
					transformation.Update(minMaxValues.Min);
					transformation.Update(minMaxValues.Max);
					MinMaxValues range = GetRange(axis, map);
					transformation.Update(range.Min);
					transformation.Update(range.Max);
				}
				else {
					double min = Math.Min(GetMinAbsValueFromAxisElements(axis), GetMinAbsValue());
					if (double.MaxValue == min || double.MinValue == min)
						min = 1;
					transformation.Update(min);
				}
				if (!minMaxValues.HasValues) {
					ILogarithmic logarithmic = axis as ILogarithmic;
					if (logarithmic != null && logarithmic.Enabled && (axis.VisualRange.MaxValue is double))
						transformation.Update((double)axis.VisualRange.MaxValue);
				}
				transformation.CompleteUpdate();
			}
		}
		MinMaxValues GetRange(IAxisData axis, AxisScaleTypeMap map) {
			if (axis.VisualRange == null)
				return new MinMaxValues(map.NativeToInternal(axis.WholeRange.MinValue), map.NativeToInternal(axis.WholeRange.MaxValue));
			double min = 0;
			if (!axis.VisualRange.AutoCorrectMin && axis.VisualRange.MinValue != null) {
				min = map.NativeToInternal(axis.VisualRange.MinValue);
				min = Math.Max(min, map.NativeToInternal(axis.WholeRange.MinValue));
			}
			else if (axis.WholeRange != null && axis.WholeRange.MinValue != null)
				min = map.NativeToInternal(axis.WholeRange.MinValue);
			double max = 0;
			if (!axis.VisualRange.AutoCorrectMax && axis.VisualRange.MaxValue != null) {
				max = map.NativeToInternal(axis.VisualRange.MaxValue);
				max = Math.Min(max, map.NativeToInternal(axis.WholeRange.MaxValue));
			}
			else if (axis.WholeRange != null && axis.WholeRange.MaxValue != null)
				max = map.NativeToInternal(axis.WholeRange.MaxValue);
			return new MinMaxValues(min, max);
		}
		public void UpdateScaleBreaks() {
			IAutoScaleBreaksContainer scaleBreaksContainer = GroupKey.AxisData as IAutoScaleBreaksContainer;
			if (scaleBreaksContainer != null && scaleBreaksContainer.Enabled) {
				List<IRefinedSeries> seriesList = new List<IRefinedSeries>(refinedSeries.Count);
				foreach (IRefinedSeries series in refinedSeries)
					seriesList.Add(series);
				scaleBreaksContainer.UpdateScaleBreaks(seriesList);
			}
		}
		public virtual void UpdateAxisData() {
			if (!(GroupKey.AxisData is IAxisData))
				return;
			IAxisData axisData = GroupKey.AxisData as IAxisData;
			MinMaxValues valuesForRange = GetMinMaxValues(axisData);
			if (valuesForRange.HasValues && axisData.AxisScaleTypeMap is IPriorScaleMap)
				((IPriorScaleMap)axisData.AxisScaleTypeMap).UpdateMin(valuesForRange.Min);
		}
		MinMaxValues CheckAxisValueContainer(MinMaxValues minMaxValues, IAxisValueContainer valueContainer, IScaleMap map) {
			if (valueContainer.IsEnabled) {
				double value = map.NativeToInternal(valueContainer.GetAxisValue());
				minMaxValues.Union(value);
				minMaxValues.Union(value);
			}
			return minMaxValues;
		}
		double? GetNumericalMeasureUnit() {
			if (groupKey.AxisData != null && groupKey.AxisData.NumericScaleOptions.ScaleMode != ScaleModeNative.Continuous && !double.IsNaN(groupKey.AxisData.NumericScaleOptions.MeasureUnit))
				return groupKey.AxisData.NumericScaleOptions.MeasureUnit;
			return null;
		}
		DateTimeMeasureUnitNative GetDateTimeMeasureUnit() {
			if (groupKey.AxisData != null && groupKey.AxisData.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Continuous)
				return groupKey.AxisData.DateTimeScaleOptions.MeasureUnit;
			return DateTimeMeasureUnitNative.Millisecond;
		}
		bool ShouldRecreateScaleMap(RefinedSeries mainSeries) {
			ActualScaleType newScaleType = GetScaleType(mainSeries);
			if (scaleMap == null || newScaleType != scaleMap.ScaleType)
				return true;
			if (scaleMap.ScaleType == ActualScaleType.Qualitative)
				return true;
			if (scaleMap.ScaleType == ActualScaleType.Numerical)
				return GetNumericalMeasureUnit() != ((AxisNumericalMap)scaleMap).MeasureUnit;
			if (scaleMap.ScaleType == ActualScaleType.DateTime)
				return GetDateTimeMeasureUnit() != ((AxisDateTimeMap)scaleMap).MeasureUnit;
			return false;
		}
		public bool UpdateScaleMap(bool forceCreate) {
			RefinedSeries mainSeries = FindMainSeries();
			bool isScaleMapUpdated = false;
			if (mainSeries != null) {
				ActualScaleType newScaleType = GetScaleType(mainSeries);
				if (ShouldRecreateScaleMap(mainSeries) || forceCreate) {
					scaleMap = CreateScaleMap(mainSeries);
					isScaleMapUpdated = (scaleMap != null);
				}
			}
			else if (scaleMap == null) {
				scaleMap = new AxisNumericalMap();				
				isScaleMapUpdated = true;
			}
			BindScaleMapToAxis();
			return isScaleMapUpdated;
		}
		internal void RemoveAllSeries() {
			refinedSeries.Clear();
		}
		protected internal virtual void UpdateSideMarginsEnable() {
		}
	}
	public class RefinedSeriesGroupByValue : RefinedSeriesGroup {
		readonly IMinMaxValuesCalculator minMaxCalculator;
		readonly List<IAffectsAxisRange> indicators = new List<IAffectsAxisRange>();
		public RefinedSeriesGroupByValue(RefinedSeriesGroupKey groupKey, IMinMaxValuesCalculator minMaxCalculator) : base(groupKey) {
			this.minMaxCalculator = minMaxCalculator;
		}
		protected override AxisQualitativeMap CreateQualitativeMap() {
			return null;
		}
		protected override ActualScaleType GetScaleType(RefinedSeries series) {
			return series.ValueScaleType;
		}
		internal override double GetMinAbsValue() {
			return minMaxCalculator.GetMinAbsValue(RefinedSeries);
		}
		public void AddIndicator(IAffectsAxisRange indicator) {
			indicators.Add(indicator);
		}
		public void ClearIndicators() {
			indicators.Clear();
		}
		public override MinMaxValues GetMinMaxValuesFromSeries() {
			return minMaxCalculator.GetMinMaxValues(RefinedSeries, MinMaxValues.Empty, indicators);
		}
		public override MinMaxValues GetMinMaxValuesFromSeries(IMinMaxValues rangeForFiltering, List<RefinedSeries> seriesForFiltering) {
			if (seriesForFiltering == null)
				return minMaxCalculator.GetMinMaxValues(RefinedSeries, rangeForFiltering, indicators);
			List<RefinedSeries> seriesList = new List<RefinedSeries>();
			foreach (RefinedSeries series in RefinedSeries)
				if (seriesForFiltering.Contains(series))
					seriesList.Add(series);
			return minMaxCalculator.GetMinMaxValues(seriesList, rangeForFiltering, indicators);
		}
		public override MinMaxValues GetMinMaxRefined() {
			MinMaxValues range = GetMinMaxValuesFromSeries();
			if (!range.HasValues)
				return range;
			return new MinMaxValues(ScaleMap.InternalToRefinedExact(range.Min), ScaleMap.InternalToRefinedExact(range.Max));
		}
	}
	public class RefinedSeriesGroupByArgument : RefinedSeriesGroup {
		public RefinedSeriesGroupByArgument(RefinedSeriesGroupKey groupKey)
			: base(groupKey) {
		}
		protected override AxisQualitativeMap CreateQualitativeMap() {
			AxisQualitativeMap result = new AxisQualitativeMap(new List<Object>());
			List<RefinedSeries> unsortedSeries = new List<RefinedSeries>();
			this.RefinedSeries.Sort((x, y) => { return x.ActiveIndex >= y.ActiveIndex ? 1 : -1; });
			foreach (RefinedSeries series in this.RefinedSeries) {
				if (!series.Series.ArePointsSorted)
					unsortedSeries.Add(series);
				else {
					IList<RefinedPoint> points = series.PointsForMap;
					for (int j = 0; j < points.Count; j++) {
						ISeriesPoint point = points[j].SeriesPoint;
						result.AddValue(point.UserArgument);
					}
				}
			}
			foreach (RefinedSeries series in unsortedSeries) {
				for (int j = 0; j < series.FinalPoints.Count; j++) {
					ISeriesPoint point = series.FinalPoints[j].SeriesPoint;
					result.AddValue(point.UserArgument);
				}
			}
			IAxisData axisElementContainer = GroupKey.AxisData;
			if (axisElementContainer != null) {
				object obj;
				if (axisElementContainer.ConstantLines != null)
					foreach (IConstantLine line in axisElementContainer.ConstantLines)
						if (line.IsEnabled) {
							obj = line.GetAxisValue();
							result.AddValue(obj);
						}
				if (axisElementContainer.ScaleBreaks != null)
					foreach (IScaleBreak scaleBreak in axisElementContainer.ScaleBreaks)
						if (scaleBreak.Edge1.IsEnabled && scaleBreak.Edge2.IsEnabled) {
							obj = scaleBreak.Edge1.GetAxisValue();
							result.AddValue(obj);
							obj = scaleBreak.Edge2.GetAxisValue();
							result.AddValue(obj);
						}
				if (axisElementContainer.CustomLabels != null)
					foreach (ICustomAxisLabel label in axisElementContainer.CustomLabels) {
						if (label.IsEnabled) {
							obj = label.GetAxisValue();
							result.AddValue(obj);
						}
					}
				if (axisElementContainer.Strips != null)
					foreach (IStrip strip in axisElementContainer.Strips) {
						if (strip.Visible) {
							if (((IAxisValueContainer)strip.MinLimit).IsEnabled)
								result.Insert(strip.MinLimit.GetAxisValue());
							if (((IAxisValueContainer)strip.MaxLimit).IsEnabled)
								result.AddValue(strip.MaxLimit.GetAxisValue());
						}
					}
				result.SortValues(axisElementContainer.QualitativeScaleComparer);
			}
			return result;
		}
		protected override ActualScaleType GetScaleType(RefinedSeries series) {
			return series.ArgumentScaleType;
		}
		bool HasFullStackedSeries(List<RefinedSeries> refinedSeries) {
			foreach (RefinedSeries series in refinedSeries)
				if (IsFullStackedSeries(series.SeriesView))
					return true;
			return false;
		}
		bool IsFullStackedSeries(ISeriesView view) {
			Type pointType = view.PointInterfaceType;
			return pointType.Equals(typeof(IFullStackedPoint));
		}
		MinMaxValues GetMinMaxByFilteredPoints(IMinMaxValues rangeForFiltering, List<RefinedSeries> seriesList) {
			MinMaxValues range = new MinMaxValues(double.MaxValue, double.MinValue);
			MinMaxValues filter = new MinMaxValues(rangeForFiltering);
			foreach (RefinedSeries series in seriesList) {
				range = range.Union(FindFirstPoint(series, filter, 0, 1));
				range = range.Union(FindFirstPoint(series, filter, series.FinalPointsSortedByArgument.Count - 1, -1));
			}
			return range;
		}
		MinMaxValues FindFirstPoint(RefinedSeries series, MinMaxValues filter, int start, int increment) {
			int pointCount = series.FinalPointsSortedByArgument.Count;
			int index = start;
			MinMaxValues argument = MinMaxValues.Empty;
			while (0 <= index && index < pointCount && argument.IsEmpty) {
				argument = Filter(index, series, filter);
				index = index + increment;
			}
			return argument;
		}
		MinMaxValues Filter(int i, RefinedSeries series, MinMaxValues filter) {
			RefinedPoint point = (RefinedPoint)series.FinalPointsSortedByArgument[i];
			double maxPointValue = series.SeriesView.GetRefinedPointMax(point);
			double minPointValue = series.SeriesView.GetRefinedPointMin(point);
			if (filter.InRange(maxPointValue) || filter.InRange(minPointValue))
				return new MinMaxValues(point.Argument);
			return MinMaxValues.Empty;
		}
		public override MinMaxValues GetMinMaxValuesFromSeries() {
			return GetMinMaxValuesFromSeries(MinMaxValues.Empty, null);
		}
		public override MinMaxValues GetMinMaxValuesFromSeries(IMinMaxValues rangeForFiltering, List<RefinedSeries> seriesForFiltering) {
			List<RefinedSeries> seriesList = new List<RefinedSeries>();
			if (seriesForFiltering != null) {
				foreach (RefinedSeries series in RefinedSeries)
					if (seriesForFiltering.Contains(series))
						seriesList.Add(series);
			}
			else
				seriesList = RefinedSeries;
			if (!MinMaxValues.IsEmptyValue(rangeForFiltering) && !HasFullStackedSeries(RefinedSeries))
				return GetMinMaxByFilteredPoints(rangeForFiltering, seriesList);
			MinMaxValues range = MinMaxValues.Empty;
			GeometryCalculator geometryCalculator = new GeometryCalculator();		  
			foreach (RefinedSeries series in seriesList) {
				range.Union(series.MinArgument);
				range.Union(series.MaxArgument);
				ISplineSeriesView splineView = series.SeriesView as ISplineSeriesView;
				if (splineView != null && splineView.ShouldCorrectRanges) {
					IList<IGeometryStrip> strips = geometryCalculator.CreateStrips(series);
					foreach (IBezierStrip strip in strips) {
						MinMaxValues minMaxSplineValues = strip.CalculateMinMaxArguments();
						range = range.Union(minMaxSplineValues);
					}
				}
			}
			return range;
		}
		public override MinMaxValues GetMinMaxRefined() {
			double min = double.MaxValue;
			double max = double.MinValue;
			RefinedPoint minPoint = null;
			RefinedPoint maxPoint = null;
			foreach (RefinedSeries series in RefinedSeries) {
				RefinedPoint point = series.MinArgumentPoint;
				if (point == null)
					continue;
				double seriesMin = point.Argument;
				if (GeometricUtils.IsValidDouble(seriesMin) && seriesMin < min) {
					min = seriesMin;
					minPoint = point;
				}
				point = series.MaxArgumentPoint;
				double seriesMax = point.Argument;
				if (GeometricUtils.IsValidDouble(seriesMax) && seriesMax > max) {
					max = point.Argument;
					maxPoint = point;
				}
			}
			if (minPoint != null && maxPoint != null) {
				min = ScaleMap.NativeToRefined(minPoint.SeriesPoint.UserArgument);
				max = ScaleMap.NativeToRefined(maxPoint.SeriesPoint.UserArgument);
				return new MinMaxValues(min, max);
			}
			return MinMaxValues.Empty;
		}
		internal override double GetMinAbsValue() {
			double minArgument = double.MaxValue;
			foreach (var series in RefinedSeries) {
				double argument = series.GetMinAbsArgument();
				if (argument > 0 && minArgument > argument)
					minArgument = argument;
			}
			return minArgument;
		}
		public override void UpdateAxisData() {
			base.UpdateAxisData();
			CalculateFilteredIndexes();
		}
		public void CalculateFilteredIndexes() {
			foreach (RefinedSeries series in RefinedSeries) {
				IAxisData axisData = GroupKey.AxisData as IAxisData;
				series.UpdateVisiblePointIndexes((axisData == null) ? null : axisData.VisualRange);
			}
		}
		bool CalcSideMarginsEnable() {
			if (RefinedSeries.Count == 0)
				return true;
			foreach (RefinedSeries series in RefinedSeries) {
				IXYSeriesView view = series.SeriesView as IXYSeriesView;
				if ((view != null) && view.SideMarginsEnabled)
					return true;
			}
			return false;
		}
		bool IsAutoMode(SideMarginMode mode) {
			return mode != SideMarginMode.UserDisable && mode != SideMarginMode.UserEnable;
		}
		protected internal override void UpdateSideMarginsEnable() {
			if (GroupKey.AxisData != null) {
				bool sideMarginsEnable = CalcSideMarginsEnable();
				if (IsAutoMode(GroupKey.AxisData.VisualRange.AutoSideMargins))
					GroupKey.AxisData.VisualRange.AutoSideMargins = sideMarginsEnable ? SideMarginMode.Enable : SideMarginMode.Disable;
				if (IsAutoMode(GroupKey.AxisData.WholeRange.AutoSideMargins))
					GroupKey.AxisData.WholeRange.AutoSideMargins = sideMarginsEnable ? SideMarginMode.Enable : SideMarginMode.Disable;
				if (!sideMarginsEnable) {
					if (IsAutoMode(GroupKey.AxisData.VisualRange.AutoSideMargins))
						GroupKey.AxisData.VisualRange.SideMarginsValue = 0;
					if (IsAutoMode(GroupKey.AxisData.WholeRange.AutoSideMargins))
						GroupKey.AxisData.WholeRange.SideMarginsValue = 0;
				}
			}
		}
	}
}
